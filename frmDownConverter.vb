' ============================================================
' frmDownConverter.vb — convertito da Prog0347-FOCC10\frmDownConverter.frm (VB6)
' Telespazio FOC-C10 — DYNATRON
' Base: BTS1 (stesso Down Converter, stesso schema di risposta
' "S"/"T"/"Q"). Differenze di C10 rispetto a BTS1: le attenuazioni
' si impostano con il framing CheckDC "{<addr>TA<valore*10>}" /
' "{<addr>TB<valore*10>}" invece di "*CSGA:0/1,..."; la frequenza
' si formatta con "#0000.0#" (BTS1: "#0000.00#").
' ============================================================

Public Class FrmDownConverter

    ' VB.NET P/Invoke: hwnd parameter changed from Long to IntPtr
    Private Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (ByVal hwnd As IntPtr, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
    Dim Send_GRF As String

    Private Sub CmdWeb_Click(sender As Object, e As EventArgs) Handles CmdWeb.Click
        Const SW_NORMAL = 1
        Dim X As Long
        X = ShellExecute(Me.Handle, "Open", "http://" & g_IPDC, vbNullString, vbNullString, SW_NORMAL)
    End Sub

    Private Sub frmDownConverter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Campi di sola presentazione: giallo se sconosciuti, 128,255,255 se noti
        ColoriStandard.RegistraDati(LblAtt1, LblFreq, LblAtt2)
        LblFreq.Text = ""
        LblAtt1.Text = ""
        LblAtt2.Text = ""
        b_down = True
    End Sub

    Private Sub MnuClose_Click(sender As Object, e As EventArgs) Handles MnuClose.Click
        Me.Close()
        b_down = False
    End Sub
    Public Function PresentaDown(Buffer As String)
        Dim Status2 As String, Tipo As String
        PresentaDown = True
        LblFreq.Text = f_FrqBeacon
        Status2 = Mid$(Buffer, 3, 1)
        Select Case Status2
            Case "S"       'Controllo se risposta di stato
 '               Case "F"      'Controllo se risposta su richiesta frequenza
 '                   LblFreq.Text = Format$(Mid$(Buffer, 7, 12) / 10000, "#00.000")
            Case "T"        'Controllo se risposta su richiesta dei valori di attenuazione
                Tipo = Mid$(Buffer, 4, 1)
                Select Case Tipo
                    Case "A"
                        LblAtt1.Text = Mid$(Buffer, 5, 3)
                        If IsNumeric(LblAtt1.Text) Then LblAtt1.Text = Format$(CDbl(LblAtt1.Text) / 10, "#0.0")
                    Case "B"
                        LblAtt2.Text = Mid$(Buffer, 5, 3)
                        If IsNumeric(LblAtt2.Text) Then LblAtt2.Text = Format$(CDbl(LblAtt2.Text) / 10, "#0.0")
                End Select
            Case "Q"        'Controllo se risposta su richiesta dei valori di equalizzazione

        End Select
    End Function


    Private Sub TxtAtt1_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtAtt1.KeyUp
        Dim Valore As String
        If e.KeyCode = Keys.Return Then
            ' Prima CDbl(.Text): eccezione con campo vuoto o non numerico.
            Dim v As Double
            If LeggiDecimale(TxtAtt1.Text, v) AndAlso v >= 0 AndAlso v <= 30 Then
                Valore = Format$(v * 10, "000")
                b_int(GRF) = True
                b_Ricezione(GRF) = True
                Send_GRF = CheckDC("{" & Chr(AddressDC) & "TA" & Valore & "}")
                Send_GRF = Send_GRF & vbCr & vbLf
                Invio(Send_GRF, GRF)
            End If
        End If
    End Sub

    Private Sub TxtAtt2_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtAtt2.KeyUp
        Dim Valore As String
        If e.KeyCode = Keys.Return Then
            ' Prima CDbl(.Text): eccezione con campo vuoto o non numerico.
            Dim v As Double
            If LeggiDecimale(TxtAtt2.Text, v) AndAlso v >= 0 AndAlso v <= 30 Then
                Valore = Format$(v * 10, "000")
                b_int(GRF) = True
                b_Ricezione(GRF) = True
                Send_GRF = CheckDC("{" & Chr(AddressDC) & "TB" & Valore & "}")
                Send_GRF = Send_GRF & vbCr & vbLf
                Invio(Send_GRF, GRF)
            End If
        End If
    End Sub

    Private Sub TxtFreq_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtFreq.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtFreq.Text) Then
                ' Format$(stringa, "...") non converte in numero come in VB6:
                ' restituisce la picture letterale invece del valore. Va
                ' passato il valore numerico (CDbl), non la stringa .Text.
                g_Frequenza = Format$(CDbl(TxtFreq.Text), "#0000.0#")
                InvioFrequenza()
            End If
        End If
    End Sub
    Public Sub CmdATA()
        b_int(GRF) = True
        b_Ricezione(GRF) = True
        Send_GRF = CheckDC("{" & Chr(AddressDC) & "TA?}")
        Send_GRF = Send_GRF & vbCr & vbLf
        Invio(Send_GRF, GRF)
End Sub
    Public Sub CmdATB()
        b_int(GRF) = True
        b_Ricezione(GRF) = True
        Send_GRF = CheckDC("{" & Chr(AddressDC) & "TB?}")
        Send_GRF = Send_GRF & vbCr & vbLf
        Invio(Send_GRF, GRF)
End Sub
    Public Sub CmdSTA()
        b_int(GRF) = True
        b_Ricezione(GRF) = True
        Send_GRF = CheckDC("{" & Chr(AddressDC) & "SB}")
        Send_GRF = Send_GRF & vbCr & vbLf
        Invio(Send_GRF, GRF)
End Sub
    Public Sub CmdVLT(Tipo As String)
        b_int(GRF) = True
        b_Ricezione(GRF) = True
        Send_GRF = CheckDC("{" & Chr(AddressDC) & "QT" & Tipo & "}")
        Send_GRF = Send_GRF & vbCr & vbLf
        Invio(Send_GRF, GRF)
End Sub


End Class

