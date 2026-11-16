' ============================================================
' frmRicezione.Socket.vb
' Telespazio BTS1 — DYNATRON
' ============================================================
' Partial class dedicata a:
'   - Invio dati (Invio), per TCP (via SocketManager) e seriale
'   - Ricezione dati seriale (SerialDataReceived, evento DataReceived)
' Le connessioni TCP (Connect/Listen) e la ricezione asincrona TCP sono
' gestite da SocketManager.vb, l'unico percorso realmente usato da
' IniPort_Control/LanReset: questo file conteneva in passato una seconda
' implementazione parallela (ConnettiCanale/ConnettiTCP/
' CallbackRicezioneMain/Mac/RX/DC/AvviaRicezioneAsincrona/
' TentaRiconnessione/RiconnettiFalliti) mai chiamata da nessuna parte —
' rimossa perché solo fonte di confusione (si rischiava di correggere
' codice che non gira mai, come successo durante questa stessa sessione).
' ============================================================

Imports System.IO.Ports
Imports System.Text

Partial Public Class FrmRicezione

    ' -------------------------------------------------------
    ' Invio dati (TCP + Seriale)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Invia un comando al canale Ind. Sostituisce la funzione Invio() del
    ''' VB6 originale. Per TCP passa da SocketManager (le connessioni reali
    ''' sono le sue).
    ''' </summary>
    Public Sub Invio(comando As String, Ind As Integer)
        Try
            If Port_Control(Ind).Socket = "S" Then
                Select Case Ind
                    Case ACU : SocketManager.SendMain(Ind, comando)
                    Case MAC : SocketManager.SendMac(Ind, comando)
                    Case RX : SocketManager.InvioRX(2, comando) ' sotto-socket di controllo RX
                    Case GRF : SocketManager.SendDC(Ind, comando)
                End Select
            Else
                ' Invio seriale (C10: ComPort indicizzato per canale, vedi
                ' Gestione.ScriviSeriale — prima si usava il numero COM).
                ScriviSeriale(Ind, comando)
            End If
        Catch ex As Exception
            WriteError(0, "Invio(canale=" & Ind & "): " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Ricezione seriale
    ' -------------------------------------------------------

    ''' <summary>
    ''' Handler ricezione dati seriale (evento DataReceived), agganciato da
    ''' FrmMain.PortReset. Eseguito su thread separato — usa Invoke per
    ''' accedere alla UI.
    ''' </summary>
    Public Sub SerialDataReceived(sender As Object, e As SerialDataReceivedEventArgs)
        Try
            Dim sp As SerialPort = CType(sender, SerialPort)
            ' Canale = indice in ComPort() dell'oggetto che ha ricevuto
            ' (in C10 più canali possono dichiarare lo stesso COM nell'ini).
            Dim Ind As Integer = Array.IndexOf(ComPort, sp)
            If Ind < 1 Then Ind = CanaleDaPorta(CInt(sp.PortName.Replace("COM", "")))
            If Ind < 1 Then Return
            Dim testo As String = sp.ReadExisting()
            Port_Control(Ind).Buffer &= testo
            Me.Invoke(Sub()
                          AnalisiOrdini(Ind, Port_Control(Ind).Buffer)
                          TmrTimeoutMSG(Ind).Enabled = False
                          TmrTimeoutMSG(Ind).Enabled = True
                      End Sub)
        Catch ex As Exception
            WriteError(0, "SerialDataReceived: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Trova il canale (Ind) a partire dal numero porta COM.
    ''' </summary>
    Private Function CanaleDaPorta(portNum As Integer) As Integer
        For i As Integer = 1 To NumCom
            If Canali(i).Port_IN = portNum Then Return i
        Next
        Return -1
    End Function

    ''' <summary>Converte stringa parity da INI in enum Parity.</summary>
    Public Shared Function ParseParity(s As String) As Parity
        Select Case s.ToUpperInvariant().Trim()
            Case "E" : Return Parity.Even
            Case "O" : Return Parity.Odd
            Case "M" : Return Parity.Mark
            Case "S" : Return Parity.Space
            Case Else : Return Parity.None
        End Select
    End Function

    ''' <summary>Converte stringa stop bits da INI in enum StopBits.</summary>
    Public Shared Function ParseStopBits(s As String) As StopBits
        Select Case s.Trim()
            Case "2" : Return StopBits.Two
            Case "1.5" : Return StopBits.OnePointFive
            Case Else : Return StopBits.One
        End Select
    End Function

End Class
