' ============================================================
' frmRicezione.Protocollo.vb
' Telespazio BTS1 — DYNATRON
' ============================================================
' Partial class dedicata al parsing dei protocolli:
'   - AnalisiOrdini  → dispatcher principale per canale
'   - ParseACU       → telemetria antenna (Az/El, stato, allarmi)
'   - ParseMAC       → messaggi MCS/MAC
'   - ParseRX        → ricevitore (beacon, lock, fase)
'   - ParseGRF       → Down Converter (frequenza, LO)
'   - (comandi in uscita: CheckSum/CheckDC + Invio, vedi Gestione e frmDownConverter)
' ============================================================

Imports System.Text
Imports System.Globalization

Partial Public Class FrmRicezione

    ' -------------------------------------------------------
    ' Costanti protocollo ACU (esempio struttura messaggio)
    ' STX/ETX come delimitatori di frame
    ' -------------------------------------------------------
    Private Const STX As Char = Chr(2)
    Private Const ETX As Char = Chr(3)
    Private Const ACK As Char = Chr(6)
    Private Const NAK As Char = Chr(21)

    ' Lunghezza attesa messaggi per canale
    Private Const LUNGH_ACU As Integer = 98
    Private Const LUNGH_MAC As Integer = 32
    ' MenuStrip1, MenuStrip2 dichiarati in frmRicezione.Designer.vb
    Private Const LUNGH_GRF As Integer = 48

    ' -------------------------------------------------------
    ' SEZIONE 1 — Dispatcher principale
    ' -------------------------------------------------------

    ''' <summary>
    ''' Punto di ingresso per l'analisi dei dati ricevuti.
    ''' Smista al parser corretto in base al canale (Ind).
    ''' Sostituisce e completa lo stub AnalisiOrdini di frmRicezione.vb.
    ''' </summary>
    Public Shadows Sub AnalisiOrdini(Ind As Integer, buffer As String)
        ' Solo dispatcher: ogni branch (AnalisiACUReale/MAC/RX/GRF) ha già
        ' il proprio gestore d'errore, quindi non serve qui.
        If String.IsNullOrEmpty(buffer) Then Return

        Select Case Ind
            Case ACU : AnalisiACUReale(Ind, buffer)
            Case MAC : AnalisiMAC(Ind, buffer)
            Case RX : AnalisiRX(Ind, buffer)
            Case GRF : AnalisiGRF(Ind, buffer)
            Case PCU : AnalisiPCU(Ind, buffer)
            Case INTELSAT
                ' VB6 ProtIntelsat: analisi carattere per carattere
                Port_Control(Ind).Buffer = ""
                For Each c As Char In buffer
                    ProtIntelsat(c)
                Next
            Case Else
                ' Canali senza analisi: il buffer non deve crescere all'infinito
                Port_Control(Ind).Buffer = ""
        End Select
    End Sub

    ' Richiesta INTELSAT in costruzione (VB6: Port_Control(INTELSAT).Buffer,
    ' qui separato perché AnalisiOrdini riceve ogni volta i nuovi caratteri)
    Private bufferIntelsat As String = ""

    ''' <summary>
    ''' Canale INTELSAT (C10, seriale) — porting di ProtIntelsat. Richieste
    ''' di al più 5 caratteri terminate da CR, con un eventuale carattere di
    ''' testata prima del codice (ripetuto nella risposta):
    '''   DS stato PTIC (s_StatusPTIC, 18 o 20 cifre esadecimali, poi azzerato)
    '''   DP posizione (TagDato, una sola volta)   FD frequenza beacon
    '''   DE errori di tracking (TagError)          DT tipo "2201"
    '''   DD livello beacon in millesimi di dB
    ''' Risposta terminata da CR LF, registrata con ScriviLOG4.
    ''' Non implementato nel .NET di FOC6.
    ''' </summary>
    Private Sub ProtIntelsat(carattere As Char)
        Try
            If carattere = vbLf Then
                bufferIntelsat = ""
                Return
            End If
            bufferIntelsat &= carattere
            If bufferIntelsat.Length > 5 Then
                bufferIntelsat = ""
                TmrTimeoutMSG(INTELSAT).Enabled = False
                TmrTimeoutMSG(INTELSAT).Enabled = True
                Return
            End If
            If carattere <> vbCr Then Return

            Dim testata As String = ""
            Dim codice As String
            Dim primo As String = bufferIntelsat.Substring(0, 1).ToUpperInvariant()
            If primo <> "D" AndAlso primo <> "F" Then
                testata = bufferIntelsat.Substring(0, 1)
                codice = Mid$(bufferIntelsat, 2, 2)
            Else
                codice = Mid$(bufferIntelsat, 1, 2)
            End If
            bufferIntelsat = ""

            Dim risposta As String = ""
            Select Case codice.ToUpperInvariant()
                Case "DS"
                    risposta = "DS"
                    Dim n As Integer = If(Ptic = "A", 18, 20)
                    For i As Integer = 1 To n
                        risposta &= Hex(s_StatusPTIC(i))
                    Next
                    risposta = testata & risposta & vbCr & vbLf
                    For i As Integer = 1 To 20
                        s_StatusPTIC(i) = 0
                    Next
                Case "DP"
                    If TagDato = "" Then Return
                    risposta = testata & "DP" & TagDato & vbCr & vbLf
                    TagDato = ""
                Case "FD"
                    risposta = testata & "FD" & g_Frequenza & vbCr & vbLf
                Case "DE"
                    risposta = testata & "DE" & TagError & vbCr & vbLf
                Case "DT"
                    risposta = testata & "DT2201" & vbCr & vbLf
                Case "DD"
                    If IsNumeric(s_Lev1) Then
                        risposta = testata & "DD10" & Format$(Val(s_Lev1) * 1000, "000000") & vbCr & vbLf
                    Else
                        risposta = testata & "DD10" & vbCr & vbLf
                    End If
            End Select
            LblConfCNL5in.BackColor = COL_OK
            Port_Control(INTELSAT).Port_State = PCS_NORMAL
            TmrTimeoutMSG(INTELSAT).Enabled = False
            TmrTimeoutMSG(INTELSAT).Enabled = True
            If risposta <> "" Then
                Invio(risposta, INTELSAT)
                ScriviLOG4(risposta, 1)
            End If
        Catch ex As Exception
            WriteError(0, "ProtIntelsat: " & ex.Message)
            bufferIntelsat = ""
        End Try
    End Sub

    ''' <summary>
    ''' Polarizzatore (C10, seriale PCU) — VB6 ProtMAC con Index = PCU: ACK
    ''' o frame STX...ETX con lunghezza/checksum validi (VerDatiMAC) mettono
    ''' il canale in linea (spia verde, timeout fermato); NAK chiude la
    ''' richiesta. Il contenuto della risposta non viene usato (lblRVR
    ''' Case 6 vuoto nel VB6): l'angolo arriva dallo stato ACU (s_POL).
    ''' </summary>
    Private Sub AnalisiPCU(Ind As Integer, ByRef buffer As String)
        Try
            Dim inLinea As Boolean = False
            If buffer.Contains(ACK) Then
                inLinea = True
                buffer = buffer.Replace(CStr(ACK), "")
            End If
            If buffer.Contains(NAK) Then
                TmrTimeoutMSG(PCU).Enabled = False
                buffer = buffer.Replace(CStr(NAK), "")
            End If
            Do While buffer.Contains(ETX)
                Dim posEtx As Integer = buffer.IndexOf(ETX)
                Dim posStx As Integer = buffer.LastIndexOf(STX, posEtx)
                Dim frame As String = If(posStx >= 0, buffer.Substring(posStx, posEtx - posStx + 1), "")
                buffer = buffer.Substring(posEtx + 1)
                If frame.Length > 3 Then
                    If VerDatiMAC(frame) Then
                        inLinea = True
                    Else
                        Port_Control(PCU).CKS_Error += 1
                    End If
                End If
            Loop
            If buffer.Length > 0 AndAlso buffer.IndexOf(STX) < 0 Then buffer = ""
            If inLinea Then
                b_Ricezione(PCU) = False
                Port_Control(PCU).Port_State = PCS_NORMAL
                Port_Control(PCU).Rec_State = PCR_WAIT
                Port_Control(PCU).CKS_Error = 0
                TmrTimeoutMSG(PCU).Enabled = False
                LblConfCNL6in.BackColor = COL_OK
            End If
        Catch ex As Exception
            WriteError(0, "AnalisiPCU: " & ex.Message)
            buffer = ""
        End Try
        Port_Control(Ind).Buffer = buffer
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 2 — Parser ACU (telemetria antenna)
    ' -------------------------------------------------------

    ''' <summary>
    ''' VerDatiMAC (VB6 C10): la lunghezza dichiarata (pos 2-5) deve
    ''' coincidere con quella del frame e il checksum (XOR di tutti i
    ''' caratteri da STX all'ultimo dato, in esadecimale) con quello ricevuto.
    ''' </summary>
    Public Shared Function VerDatiMAC(frame As String) As Boolean
        If frame.Length < 8 Then Return False
        If Val(Mid$(frame, 2, 4)) <> frame.Length Then Return False
        Dim cks As Integer = 0
        For i As Integer = 0 To frame.Length - 4
            cks = cks Xor AscW(frame(i))
        Next
        Return (cks And &HFF) = CInt(Val("&H" & Mid$(frame, frame.Length - 2, 2)))
    End Function

    ''' <summary>
    ''' ACK/NAK singoli dall'ACU (VB6 ProtMAC, Case 6/21): confermano o
    ''' rifiutano l'ultimo comando. Liberano la richiesta in corso (i_Cont
    ''' 2 -> 1, canale in standby); con ACK e nessuna risposta attesa la
    ''' spia ACU torna verde. Se il comando era un ordine MCS (b_int(ACU)
    ''' = False) ACK/NAK vengono inoltrati all'MCS. Senza questa gestione,
    ''' dopo un ordine MCS i_Cont restava 2 e il polling si fermava. I
    ''' caratteri 6/21 non compaiono mai dentro un frame (dati e checksum
    ''' sono ASCII stampabili), quindi vengono tolti dal buffer.
    ''' </summary>
    Private Sub GestisciAckNakACU(ByRef buffer As String)
        If buffer.IndexOf(ACK) < 0 AndAlso buffer.IndexOf(NAK) < 0 Then Return
        For Each c As Char In buffer
            If c = ACK Then
                Port_Control(ACU).CKS_Error = 0
                If Not b_Ricezione(ACU) Then
                    If i_Cont = 2 Then i_Cont = 1
                    Port_Control(ACU).Rec_State = PCR_STANDBY
                    Port_Control(ACU).Port_State = PCS_OK
                    ImpostaIndicatoreCanale(LblConfCNL1in, PCS_OK)
                    TmrTimeoutMSG(ACU).Enabled = False
                Else
                    Port_Control(ACU).Rec_State = PCR_RECEIVE
                End If
                If Not b_int(ACU) Then Invio(ACK, MAC)
            ElseIf c = NAK Then
                b_Ricezione(ACU) = False
                Port_Control(ACU).Rec_State = PCR_STANDBY
                If i_Cont = 2 Then i_Cont = 1
                If Not b_int(ACU) Then Invio(NAK, MAC)
                TmrTimeoutMSG(ACU).Enabled = False
            End If
        Next
        buffer = buffer.Replace(CStr(ACK), "").Replace(CStr(NAK), "")
    End Sub

    ''' <summary>
    ''' Estrae i frame ACU dal buffer, li verifica (VerDatiMAC) e li passa a
    ''' ValoreCampoMAC (porting dal VB6 di C10).
    ''' </summary>
    Private Sub AnalisiACUReale(Ind As Integer, ByRef buffer As String)
        Try
            GestisciAckNakACU(buffer)
            Do While buffer.Contains(ETX)
                Dim posEtx As Integer = buffer.IndexOf(ETX)
                ' C10 (VB6 frmRicezione/VerDatiMAC): frame = STX + lunghezza
                ' totale su 4 cifre + dati + checksum XOR (2 hex) + ETX.
                ' Eventuale sporcizia prima di STX viene scartata.
                Dim posStx As Integer = buffer.LastIndexOf(STX, posEtx)
                Dim frame As String = If(posStx >= 0, buffer.Substring(posStx, posEtx - posStx + 1), "")
                buffer = buffer.Substring(posEtx + 1)
                If frame.Length <= 3 Then Continue Do
                If Not VerDatiMAC(frame) Then
                    Port_Control(Ind).CKS_Error += 1
                    WriteError(0, "ACU: messaggio scartato (lunghezza/checksum): " & frame.Replace(CStr(STX), "<STX>").Replace(CStr(ETX), "<ETX>"))
                    Continue Do
                End If
                ' Messaggio senza checksum ed ETX (stesso Left$(Buffer, Len-3) del VB6)
                Dim messaggio As String = frame.Substring(0, frame.Length - 3)
                ' VB6: una risposta valida libera la richiesta di stato in
                ' corso (b_Ricezione) e riporta il canale in attesa.
                If i_Cont = 2 Then i_Cont = 1
                Port_Control(ACU).Rec_State = PCR_STANDBY
                Port_Control(ACU).CKS_Error = 0
                b_Ricezione(ACU) = False
                ScriviLOG1(Mid$(frame, 6, frame.Length - 8), 1)
                ' Ultimo stato ACU completo, rilanciato all'MCS sulla sua
                ' richiesta "*" (VB6: BuffStatus = Buffer). Prima non veniva
                ' mai valorizzato e l'MCS riceveva sempre NAK.
                If Mid$(frame, 6, 1) = "*" Then BuffStatus = frame
                ' VB6: con un ordine MCS in corso il parser riceve il frame
                ' completo (checksum + ETX), che inoltra così com'è all'MCS.
                If b_MCS Then messaggio = frame

                If messaggio.Length > 0 Then
                    ValoreCampoMAC(messaggio)
                    ' Un messaggio terminato correttamente da ETX conferma il
                    ' canale in linea (stesso principio già usato per RX/GRF).
                    Port_Control(ACU).Port_State = PCS_OK
                    ImpostaIndicatoreCanale(LblConfCNL1in, PCS_OK)
                    b_DatiAntenna = True
                    AggiornaDati()
                End If
            Loop

            Port_Control(Ind).Buffer = buffer
        Catch ex As Exception
            WriteError(0, "AnalisiACUReale: " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 3 — Parser MAC / MCS
    ' -------------------------------------------------------

    ''' <summary>
    ''' Analisi messaggi MCS/MAC — porting diretto da VB6 (frmRicezione.frm,
    ''' lblRVR_Change, Case 2 'MAC'). Il protocollo reale non è testo libero
    ''' "MSG_TYPE CAMPO1 CAMPO2" (quello usato dalla versione precedente di
    ''' questa Sub non ha riscontro nel VB6): è un frame binario a campi
    ''' fissi, STX iniziale, smistato per carattere di comando (posizione 4)
    ''' — e soprattutto include l'inoltro dei comandi MCS verso l'ACU
    ''' (OrdiniMcs, già presente in Ricezione.vb ma mai chiamato), mai
    ''' implementato in questa versione.
    ''' </summary>
    Private Sub AnalisiMAC(Ind As Integer, ByRef buffer As String)
        Try
            TmrTimeoutMSG(Ind).Enabled = False
            TmrTimeoutMSG(Ind).Enabled = True
            b_Ricezione(ACU) = False

            ' "16B " = comando di cambio frequenza inoltrato dall'MCS al
            ' Down Converter: se il canale GRF è operativo lo si accetta
            ' (riconnette il sotto-socket di controllo RX e marca la nuova
            ' frequenza da applicare), altrimenti NAK.
            If buffer.Contains("16B ") Then
                If Port_Control(GRF).Port_State = PCS_OK Then
                    Invio(ACK, MAC)
                    b_int(GRF) = True
                    ' Riconnette il sotto-socket di controllo RX tramite
                    ' SocketManager (le uniche connessioni RX realmente
                    ' gestite — vedi LanReset1): non più i campi
                    ' sckRX/streamRX di questa classe, mai più popolati da
                    ' nessuno dopo la rimozione del vecchio ConnettiTCP.
                    SocketManager.ConnectRX(LanCtrl, LanRx(LanCtrl).IPAdress, CInt(LanRx(LanCtrl).LanPort))
                    If g_ChangePhase = "N" Then
                        s_Satellite = "REMOTE"
                        If LblSatellite IsNot Nothing Then LblSatellite.Text = "REMOTE"
                    End If
                    If buffer.Length >= 14 Then i_FreqNew = CInt(Val(buffer.Substring(5, 8)))
                    g_FreqNew = True
                Else
                    Invio(NAK, MAC)
                End If
                buffer = ""
                Port_Control(Ind).Buffer = buffer
                Return
            End If

            ' C10: ordine al polarizzatore ("14Z ...") inoltrato sulla seriale PCU
            If buffer.Contains("14Z ") Then
                If LblConfCNL6in.BackColor = COL_OK Then
                    Invio(ACK, MAC)
                    b_int(PCU) = True
                    b_Ricezione(PCU) = True
                    Invio(buffer, PCU)
                Else
                    Invio(NAK, MAC)
                End If
                buffer = ""
                Port_Control(Ind).Buffer = buffer
                Return
            End If

            b_int(ACU) = False
            Dim campo As String = If(buffer.Length >= 4, buffer.Substring(3, 1), "")

            ' Comandi MCS diretti (solo se il PC1 è attivo/remoto): Program
            ' Track, Back-up da Program Track, Stand-By. "08 S" NON esce
            ' subito (come nel VB6): prosegue nel dispatch principale sotto.
            If b_StatusPC1 Then
                If buffer.Contains("PTK") AndAlso b_Calcola Then
                    Invio(ACK, Ind)
                    PTK = &H10
                    i_Status = OP_PROGRAMTRACK
                    i_ProgTrack = 1
                    AzOffset = 0
                    ElOffset = 0
                    LblAzError.Text = CStr(AzOffset)
                    LblElError.Text = CStr(ElOffset)
                    LblCommand.Text = "Program Track"
                    If CmdOffset IsNot Nothing Then
                        CmdOffset.Enabled = True : CmdOffset.Visible = True
                    End If
                    buffer = ""
                    Port_Control(Ind).Buffer = buffer
                    Return
                End If
                If buffer.Contains("PBM") AndAlso b_Calcola Then
                    Invio(ACK, Ind)
                    If buffer.Length >= 8 Then
                        BackUpPT = buffer.Substring(7, 1)
                        ScriviFileIni("Default", "BackUpPT", BackUpPT)
                    End If
                    buffer = ""
                    Port_Control(Ind).Buffer = buffer
                    Return
                End If
                If buffer.Contains("08 S") Then
                    PTK = &H0
                    d_DataInizioProg = ""
                    FrmMain.stpProgram.Text = d_DataInizioProg
                    LblAzimuth.Text = "" : LblElevazione.Text = ""
                    LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
                    LblAzError.Text = "" : LblElError.Text = ""
                    AzOffset = 0 : ElOffset = 0             ' C10
                End If
            End If

            If buffer.Length > 0 AndAlso buffer.Substring(0, 1) = STX Then
                Select Case campo
                    Case "*"
                        ' Richiesta di stato: relay dell'ultimo stato ACU
                        ' ricevuto (BuffStatus), con un bit PC1 corretto se
                        ' il PC1 non è attivo.
                        If BuffStatus <> "" Then
                            Invio(ACK, Ind)
                            Dim daInviare As String = BuffStatus
                            If Not b_StatusPC1 AndAlso daInviare.Length >= 20 Then
                                Dim pc1 As Integer = Asc(daInviare.Substring(19, 1))
                                daInviare = daInviare.Substring(0, 19) & Chr(pc1 And &HDF) & daInviare.Substring(20)
                                daInviare = daInviare.Substring(1, daInviare.Length - 4)
                                daInviare = CheckSum(daInviare)
                            End If
                            Invio(daInviare, Ind)
                            BuffStatus = ""
                        Else
                            ' VB6: NAK e riapertura del canale ACU (prima
                            ' veniva riaperto il canale MCS)
                            Invio(NAK, Ind)
                            If Port_Control(ACU).Socket <> "S" Then
                                FrmMain.PortReset(ACU)
                            Else
                                FrmMain.LanResetAsync(ACU)
                            End If
                        End If

                    Case "["
                        ' Richiesta stato ausiliaria (frequenza, potenza,
                        ' polarizzazione, stato PC1/allarmi, beacon):
                        ' costruzione del frame binario di risposta.
                        Invio(ACK, Ind)
                        Dim pc1Status As Integer = 0
                        Dim sStat As Integer = (CInt(Val(s_Mod)) And &HF) Or &H40
                        If i_Status = OP_PROGRAMTRACK Then
                            If Algoritmo = 1 Then
                                sStat = &H50
                            ElseIf Algoritmo = 0 OrElse Algoritmo = 2 Then
                                sStat = &H51
                            End If
                        ElseIf i_Status = OP_AUGMENTED Then
                            sStat = &H52
                        End If
                        ' C10: frequenza da g_Frequenza (kHz, 8 cifre), segno
                        ' della potenza solo se positiva, modo polarizzatore dai
                        ' byte di stato ACU e angolo di polarizzazione a 5
                        ' caratteri (campo assente nella versione FOC6/BTS1, che
                        ' rendeva il messaggio più corto della lunghezza 0035).
                        Dim freq As String = Format$(Val(g_Frequenza) * 1000, "00000000")
                        Dim potenza As String = Format$(f_PowerOut * 10, "000")
                        If f_PowerOut >= 0 Then potenza = "+" & potenza
                        Dim modoPol As String = "C"
                        If (vBytesold(10) And BIT3) = BIT3 Then
                            modoPol = If(i_Pol = 0, "L", "C")
                        Else
                            modoPol = "S"
                        End If
                        d_Pol = modoPol
                        Dim polField As String = "+0000"
                        If IsNumeric(Mid$(s_POL, 1, 4)) Then
                            Dim polVal As Double = Val(Mid$(s_POL, 1, 4))
                            polField = If(polVal >= 0, "+", "-") & Math.Abs(polVal).ToString("0000")
                        End If
                        Dim alarmPc As Integer = 0
                        If LblConfCNL1in.BackColor <> COL_OK OrElse LblConfCNL3in.BackColor <> COL_OK OrElse i_Alarm Then alarmPc = 1
                        Dim i As Integer
                        If PTK = 0 Then
                            i = 0
                        ElseIf PTK > 9 AndAlso PTK < 20 Then
                            i = &H1
                        ElseIf PTK = 20 Then
                            i = &H2
                        Else
                            i = &H3
                        End If
                        pc1Status = (pc1Status And &HF9) Or (i * &H2)
                        If b_StatusPC1 Then
                            pc1Status = (pc1Status And &HFE) Or &H1
                        Else
                            pc1Status = pc1Status And &HFE
                        End If
                        pc1Status = (pc1Status And &HDF) Or (alarmPc * &H20)
                        pc1Status = pc1Status Or &H40
                        Dim pc2Status As String = Chr(pc1Status)
                        Dim sBeac As String = "+0000"
                        If IsNumeric(s_Lev1) Then                 ' C10: livello del ricevitore (s_Lev1)
                            sBeac = Format$(Val(s_Lev1) * 10, "#0000")
                            If Val(s_Lev1) >= 0 Then sBeac = "+" & sBeac
                        End If
                        ' C10: modo polarizzatore e angolo (5 caratteri) presenti
                        Dim risposta As String = CheckSum("0035[" & Chr(sStat) & f_DCType & freq & potenza & d_Pol & polField & pc2Status & sBeac)
                        Invio(risposta, Ind)

                    Case Else
                        ' Non inoltrare ordini se ACU in locale o PC1 non attivo.
                        If (s_ACULR = "L" OrElse Not b_StatusPC1) AndAlso campo <> "*" AndAlso campo <> "[" Then
                            Invio(NAK, MAC)
                        Else
                            ' C10: l'ordine viene prima validato (OrdiniMcs ->
                            ' g_aggOrdini); se valido va all'ACU con b_MCS, così
                            ' la risposta dell'ACU torna all'MCS (ValoreCampoMAC),
                            ' altrimenti NAK.
                            If buffer.Length >= 6 Then OrdiniMcs(buffer.Substring(3, buffer.Length - 6))
                            If g_aggOrdini Then
                                b_Ricezione(ACU) = False
                                b_int(ACU) = False
                                i_Cont = 2
                                b_MCS = True
                                InvioACU(buffer)
                            Else
                                Invio(NAK, MAC)
                                b_MCS = False
                            End If
                        End If
                End Select
                LblConfCNL2in.ForeColor = Color.Black
            End If
        Catch ex As Exception
            WriteError(0, "AnalisiMAC: " & ex.Message)
        End Try
        buffer = ""
        Port_Control(Ind).Buffer = buffer
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 4 — Parser RX (Ricevitore)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Analisi messaggi ricevitore.
    ''' I dati arrivano su più sotto-canali; qui gestiamo il canale Mon (1).
    ''' </summary>
    Private Sub AnalisiRX(Ind As Integer, ByRef buffer As String)
        ' Stesso ragionamento di AnalisiMAC: solo operazioni su stringa già
        ' valida. DecodificaMessaggioRX ha già il proprio Try/Catch.
        Dim separatori() As String = {vbCrLf, vbLf, vbCr}
        Dim linee() As String = buffer.Split(separatori, StringSplitOptions.RemoveEmptyEntries)

        Dim ultimoSep As Integer = buffer.LastIndexOfAny({Chr(13), Chr(10)})
        buffer = If(ultimoSep >= 0, buffer.Substring(ultimoSep + 1), buffer)
        Port_Control(Ind).Buffer = buffer

        For Each linea As String In linee
            DecodificaMessaggioRX(linea.Trim())
        Next
    End Sub

    ''' <summary>
    ''' Decodifica un singolo messaggio ricevitore.
    ''' Formato: "CAMPO=VALORE"  oppure struttura proprietaria.
    ''' </summary>
    Private Sub DecodificaMessaggioRX(linea As String)
        Try
            If linea.Length < 3 Then Return

            Dim sep As Integer = linea.IndexOf("="c)
            If sep < 1 Then Return

            Dim chiave As String = linea.Substring(0, sep).ToUpperInvariant().Trim()
            Dim valore As String = linea.Substring(sep + 1).Trim()

            ' Una riga "CAMPO=VALORE" correttamente scomposta è il primo
            ' scambio valido del protocollo RX (stesso principio di
            ' DecodificaTelemetriaACU per l'ACU): conferma il canale in linea.
            Port_Control(RX).Port_State = PCS_OK
            ImpostaIndicatoreCanale(LblConfCNL3in, PCS_OK)

            Select Case chiave
                Case "LEVEL", "BEACON"
                    If IsNumeric(valore) Then
                        BeaconLevel = CDbl(valore)
                        AggiornaRicevitore()
                    End If

                Case "LOCK"
                    b_Lock = (valore.ToUpperInvariant() = "1" OrElse
                              valore.ToUpperInvariant() = "TRUE" OrElse
                              valore.ToUpperInvariant() = "YES")
                    AggiornaRicevitore()

                Case "PHASE"
                    If IsNumeric(valore) Then
                        g_Phase = CDbl(valore)
                        AggiornaFase()
                    End If

                Case "POL"
                    If TxtPol IsNot Nothing Then
                        Me.Invoke(Sub() TxtPol.Text = valore)
                    End If

                Case "POLERR"
                    If TxtPolErr IsNot Nothing Then
                        Me.Invoke(Sub() TxtPolErr.Text = valore)
                    End If

                Case "ALARM"
                    ScriviAlarm("RX: " & valore, 2)
            End Select
        Catch ex As Exception
            WriteError(0, "DecodificaMessaggioRX: " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 5 — Parser GRF / Down Converter
    ' -------------------------------------------------------

    ''' <summary>
    ''' Analisi messaggi Down Converter — porting da VB6 C10 (ProtDC +
    ''' lblRVR_Change Case 4). Frame: "{" + indirizzo + tipo (2 caratteri)
    ''' + dati + "}" + 1 carattere di checksum. Tipi di risposta di C10
    ''' (schema a 2 caratteri, come BTS1): TA/TB attenuatori, SB stato,
    ''' QT alimentazione (tutti a FrmDownConverter.PresentaDown), FE
    ''' frequenza in Hz su 9 cifre. Messaggi brevi: errori a/b/g.
    ''' Il checksum non viene verificato: nel VB6 VerDatiDC confronta il
    ''' numero con il carattere "}" e, sotto On Error Resume Next, risulta
    ''' sempre vero — comportamento mantenuto.
    ''' </summary>
    Private Sub AnalisiGRF(Ind As Integer, ByRef buffer As String)
        Try
            Do
                Dim inizio As Integer = buffer.IndexOf("{"c)
                If inizio < 0 Then
                    buffer = ""
                    Exit Do
                End If
                Dim fine As Integer = buffer.IndexOf("}"c, inizio)
                ' serve anche il carattere di checksum dopo "}"
                If fine < 0 OrElse fine + 1 >= buffer.Length Then
                    buffer = buffer.Substring(inizio)
                    Exit Do
                End If
                Dim frame As String = buffer.Substring(inizio, fine - inizio + 2)
                buffer = buffer.Substring(fine + 2)
                ElaboraRispostaDC(frame)
                Port_Control(GRF).Port_State = PCS_OK
                Port_Control(GRF).CKS_Error = 0
                TmrTimeoutMSG(GRF).Enabled = False
                ImpostaIndicatoreCanale(LblConfCNL4in, PCS_OK)
            Loop
        Catch ex As Exception
            WriteError(0, "AnalisiGRF: " & ex.Message)
            buffer = ""
        End Try
        Port_Control(Ind).Buffer = buffer
    End Sub

    Private Sub ElaboraRispostaDC(frame As String)
        If frame.Length > 5 Then
            Select Case Mid$(frame, 3, 2)
                Case "TA", "TB", "SB", "QT"
                    FrmDownConverter.PresentaDown(frame)
                Case "FE"
                    Dim freq As String = Mid$(frame, 5, 9)
                    If IsNumeric(freq) AndAlso Val(freq) <> 0 Then
                        f_FrqBeacon = CSng(Val(freq) / 100000)          ' Hz*100 -> MHz
                        g_Frequenza = Format$(f_FrqBeacon, "###00.0#")
                        If f_FrqBeacon > 0 Then TxtFrqBeacon.Text = Format$(f_FrqBeacon, "#0000.000")   '29/10/2022
                    End If
            End Select
        Else
            Select Case Mid$(frame, 3, 1)
                Case "a" : MainForm.MessaggioStatus = "DC Syntax Error"
                Case "b" : MainForm.MessaggioStatus = "DC Out of range"
                Case "g" : MainForm.MessaggioStatus = "DC General Error"
                Case Else : Return
            End Select
            MainForm.TmrMsg.Enabled = True
        End If
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 6 — Costruzione comandi in uscita
    ' -------------------------------------------------------




    ' -------------------------------------------------------
    ' SEZIONE 7 — Utilità protocollo
    ' -------------------------------------------------------

    ''' <summary>
    ''' Calcola checksum XOR su stringa ASCII (esclude STX iniziale).
    ''' </summary>
    Private Shared Function CalcolaChecksum(testo As String) As Integer
        Dim chk As Integer = 0
        For Each c As Char In testo
            If c <> STX Then chk = chk Xor Asc(c)
        Next
        Return chk
    End Function

    ''' <summary>
    ''' Verifica il checksum di un frame ricevuto.
    ''' </summary>
    Private Shared Function VerificaChecksum(frame As String) As Boolean
        If frame.Length < 3 Then Return False
        ' Gli ultimi 2 caratteri sono il checksum atteso in HEX
        Dim checksumRicevuto As Integer = Convert.ToInt32(frame.Substring(frame.Length - 2), 16)
        Dim payload As String = frame.Substring(0, frame.Length - 2)
        Return CalcolaChecksum(payload) = checksumRicevuto
    End Function

    ''' <summary>
    ''' Svuota il buffer di ricezione di un canale.
    ''' </summary>
    Public Sub SvuotaBuffer(Ind As Integer)
        Port_Control(Ind).Buffer = ""
    End Sub

End Class
