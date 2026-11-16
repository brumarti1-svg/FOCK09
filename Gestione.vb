Imports System.IO
Module Gestione

    Dim tmpSQL As String
    ' ***********************************************************
    Dim nlen As Integer
    Dim nSum As Integer
    Dim errTime As Integer
    ' 06/12/2016
    Dim rs2 As Object





    Public Function PredisponiAcquisizione() As Boolean
        Dim tmpSQL As String, i As Integer

        PredisponiAcquisizione = False
        ' C10: azzera tutti i dati della fase prima di rileggerli
        t_Pol = ""
        f_FrqBeacon = 0
        PhaseValue1 = 0
        i_Lna = 0
        g_Sat = ""
        g_Frequenza = 0
        g_PhaseEl = 0
        g_PhaseAz = 0
        g_LNA = 0
        g_Polar = ""
        g_PolErr = ""
        g_RevAz = ""
        g_CirLin = ""
        g_ValPol = "0"
        rs_Orbite = dbOrbite.OpenRecordset("Antenne", dbOpenDynaset)
        If rs_Orbite.BOF Or Antenna = "" Then
            '        Response = MessageBox.Show("Antenna sconosciuta " & Antenna, vbOKOnly)
            FrmMain.stpMessaggio.Text = "No information available for antenna "
            FrmMain.TmrMsg.Enabled = True
            '        frmConfigurazione.Show (vbModal)
            b_Calcola = False
            Exit Function
        Else
            rs_Orbite.FindFirst("Antenna ='" & Antenna & "'")
            AzOffset1 = 0
            ElOffset1 = 0
            If Not rs_Orbite.NoMatch Then
                LATSTA11 = rs_Orbite!latitudine
                LONSTA11 = rs_Orbite!Longitudine
                If Not IsDBNull(rs_Orbite!Altezza) Then
                    h = rs_Orbite!Altezza / 1000
                Else
                    h = 0
                End If
                AzOffset1 = rs_Orbite!AzOffset
                ElOffset1 = rs_Orbite!ElOffset
            Else
                '            Response = MessageBox.Show("Non ci sono dati per l'antenna " & Antenna, vbOKOnly)
                FrmMain.stpMessaggio.Text = "No information available for antenna "
                FrmMain.TmrMsg.Enabled = True
                b_Calcola = False
                Exit Function
            End If
        End If
        ' C10: dati della fase attiva (Fasi.Ultima) e comando al
        ' polarizzatore "Z <modo><angolo>" sulla seriale PCU.
        tmpSQL = "SELECT * FROM [Fasi] where Ultima"
        rs_App = dbOrbite.OpenRecordset(tmpSQL, dbOpenDynaset)
        If Not rs_App.BOF Then
            t_Pol = If(IsDBNull(rs_App!POL), "RHCP", CStr(rs_App!POL))
            g_Frequenza = If(IsNumeric(rs_App!FREQUENZA), Format$(CDbl(rs_App!FREQUENZA), "#0000.0#"), "0")
            g_ValPol = If(IsNumeric(rs_App!polangle), CStr(rs_App!polangle), "0")
            PhaseValue1 = If(IsNumeric(rs_App!faseEl), CDbl(rs_App!faseEl), 0)
            PhaseValue2 = If(IsNumeric(rs_App!faseAz), CDbl(rs_App!faseAz), 0)
            i_Lna = If(IsNumeric(rs_App!LNA), CInt(rs_App!LNA), 0)
            g_PhaseEl = PhaseValue1
            g_PhaseAz = PhaseValue2
            g_LNA = i_Lna
            g_Polar = t_Pol
            g_RevAz = If(IsDBNull(rs_App!RevAzErr), "", CStr(rs_App!RevAzErr))
            g_CirLin = If(IsDBNull(rs_App!CirLin), "", CStr(rs_App!CirLin))
            '21/02/2019 Controllo del modo del Polarizzatore
            If g_CirLin = "Linear" Then
                d_Pol = "L"
            ElseIf g_CirLin = "Circular" Then
                d_Pol = "C"
            Else
                d_Pol = "S"
            End If
            Dim valPol As Double
            If Double.TryParse(CStr(g_ValPol).Replace(",", "."), Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, valPol) Then
                Dim s_Polar As String = "Z " & d_Pol & If(valPol < 0, "-", "+") & Math.Abs(valPol).ToString("00.0", Globalization.CultureInfo.InvariantCulture)
                i = Len(s_Polar) + 6
                b_int(PCU) = True
                b_Ricezione(PCU) = True
                Invio(CheckSum(i.ToString("00") & s_Polar), PCU)
            End If
        End If
        rs_App.Close
        rs_Orbite = dbOrbite.OpenRecordset("Satelliti", dbOpenDynaset)
        If rs_Orbite Is Nothing OrElse rs_Orbite.BOF Or s_Satellite = "" Or s_Satellite = "REMOTE" Then

            '        Response = MessageBox.Show("There is not information for the Satellite " & s_Satellite, vbOKOnly)
            FrmMain.stpMessaggio.Text = "No information available for satellite "
            FrmMain.TmrMsg.Enabled = True
            s_Satellite = "REMOTE"
            b_Calcola = False
            Exit Function
        Else
            rs_Orbite.FindFirst("NomeSatellite ='" & s_Satellite & "'")
        If Not rs_Orbite.NoMatch Then
                ' C10: angolo di polarizzazione dal catalogo satelliti
                g_ValPol = If(IsNumeric(rs_Orbite!polangle), CStr(rs_Orbite!polangle), "0")
                Algoritmo = rs_Orbite!Algoritmo
                If rs_Orbite!Algoritmo = 0 Or rs_Orbite!Algoritmo = 2 Then
                    AggAlgo = rs_Orbite!Algoritmo
                    b_Calcola = InitPar11()
                    ' C10: azzurro se calcolabile, giallo se no
                    FrmRicezione.LblSatellite.BackColor = If(b_Calcola, COL_DATO_NOTO, COL_DATO_IGNOTO)
                ElseIf rs_Orbite!Algoritmo = 1 Then
                    LATSTA = LATSTA11 * GradiRad
                    LONSTA = LONSTA11 * GradiRad
                    If Not IsDBNull(rs_Orbite!FileAgg) And rs_Orbite!FileAgg <> "" Then
                        s_TwoLine = rs_Orbite!FileAgg
                        i = InStr(1, s_TwoLine, ":", 1)
                        If i = 0 Then s_TwoLine = Application.StartupPath & "\" & s_TwoLine
                    End If
                    g_Verso = (rs_Orbite!Verso?.ToString().ToUpper() = "VERO" OrElse rs_Orbite!Verso?.ToString().ToUpper() = "TRUE")  '08/11/2025
                ElseIf rs_Orbite!Algoritmo = 3 Then
                    'Predisponi la tabella Memory-track per le stelle o pianeti
                    s_FileStars = rs_Orbite!FileAgg
                    SatChange = True
                    RiempiMemory(s_FileStars)
                    b_Calcola = True
                    FrmRicezione.LblSatellite.BackColor = COL_DATO_NOTO     ' C10 (VB6 &HFFFF00)
                ElseIf rs_Orbite!Algoritmo = 4 Then
                    b_Calcola = True
                    FrmRicezione.LblSatellite.BackColor = COL_DATO_NOTO     ' C10 (VB6 &HFFFF00)
                End If
            Else
                '            Response = MessageBox.Show("There is not information for the Satellite " & Antenna, vbOKOnly)
                FrmMain.stpMessaggio.Text = "No information available for satellite "
                FrmMain.TmrMsg.Enabled = True
                '            frmSatelliti.Show
                '            frmConfigurazione.Show
                b_Calcola = False
                FrmRicezione.LblSatellite.BackColor = Color.Yellow
                g_Verso = False
                Exit Function
            End If
        End If
        '    d_DataInizioProg = ""
        If FrmMain.stpProgram.Text <> " " Then               '02/07/2025
            PTK = 0
        End If
        grado = 2                   'imposta regressione di secondo grado
        ' frmRegressione.opt2/opt1/opt3 - radio buttons not migrated (designer missing)
        NumProMax = 30
        rs_Orbite.Close
        PredisponiAcquisizione = True
    End Function


    ''' <summary>
    ''' Scrittura su porta seriale del canale Ind — porting del ramo
    ''' seriale di Invio nel VB6 di C10 (Gestione.bas): se la porta è
    ''' chiusa prova a riaprirla con PortReset, altrimenti segnala
    ''' "Serial Port nr. N not connected". ComPort() è indicizzato per
    ''' canale, come frmRicezione.ComPort(Ind) nel VB6. Usata anche da
    ''' InvioACU (libreria comune) tramite InvioSerialeHook.
    ''' </summary>
    Public Function ScriviSeriale(Ind As Integer, s_send As String) As Boolean
        Try
#If SIMULAZIONE Then
            ' Simulazione: senza porta ACU reale il comando va all'ACU simulata
            ' (frame iniettati da FrmMain.TmrOrologio_Tick). Prima PortReset
            ' falliva e rimetteva il canale in PCS_FAIL a ogni comando: lo Spiral
            ' Acquire si fermava con "ACU non in linea" a ogni passo.
            If Ind = ACU AndAlso (ComPort(Ind) Is Nothing OrElse Not ComPort(Ind).IsOpen) Then
                Port_Control(Ind).Rec_State = PCR_WAIT
                Return True
            End If
#End If
            If ComPort(Ind) Is Nothing OrElse Not ComPort(Ind).IsOpen Then
                FrmMain.PortReset(Ind)
            End If
            If ComPort(Ind) IsNot Nothing AndAlso ComPort(Ind).IsOpen Then
                ComPort(Ind).Write(s_send)
                Port_Control(Ind).Rec_State = PCR_WAIT
                Return True
            End If
            FrmMain.stpMessaggio.Text = "Serial Port nr. " & Ind & " not connected"
            FrmMain.TmrMsg.Enabled = True
        Catch ex As Exception
            WriteError(0, "ScriviSeriale(canale=" & Ind & "): " & ex.Message)
        End Try
        Return False
    End Function

    Public Function Invio(s_send As String, Ind As Integer) As Boolean
        Dim Port As Integer, i_lun As Integer
        Try
            ' frmRicezione.LblRisposta not migrated
            Invio = True
            If s_send = "" Then Exit Function

            If Port_Control(Ind).Socket <> "S" Then
                ScriviSeriale(Ind, s_send)
            Else
                Select Case Ind
                    Case ACU
                        SendMain(Ind, s_send)
                        Port_Control(Ind).Rec_State = PCR_WAIT
                    Case MAC
                        ' Era SendMain(2, ...): instradava i dati MAC/MCS sul
                        ' socket dell'ACU invece del proprio (SendMac), mai
                        ' notato perché in simulazione InvioACU non passa
                        ' mai da qui.
                        SendMac(MAC, s_send)
                    Case RX
                        ' Mancava del tutto: l'invio sul canale RX (sotto-
                        ' socket di controllo, 2 = RxCtrl) non veniva fatto.
                        SendRX(2, s_send)
                    Case GRF
                        SendDC(GRF, s_send)
                        FrmRicezione.TmrTimeoutMSG(Ind).Enabled = False
                        FrmRicezione.TmrTimeoutMSG(Ind).Enabled = True
                    Case INTELSAT
                        ' Canale aggiuntivo (trasmissione dati taggati):
                        ' fuori dai 4 array principali di SocketManager
                        ' (dimensionati su ACU/MAC/RX/GRF), mai implementato.
                        WriteError(0, "Invio: canale INTELSAT non supportato da SocketManager")
                End Select
            End If
            i_lun = Len(s_send)
            '    If g_Log And ind = ACU Then ScriviLOG Mid$(s_send, 4, i_lun - 6), 0
            If g_Log And Ind = ACU Then ScriviLOG(s_send, 0)
            If Ind = ACU Then
                FrmRicezione.TmrTimeoutMSG(Ind).Enabled = False
                FrmRicezione.TmrTimeoutMSG(Ind).Enabled = True
                ScriviLOG1(s_send, 0)
            End If
        Catch ex As Exception
            WriteError(0, "Invio: " & ex.Message)
        End Try
    End Function

    Public Function Inizializza() As Boolean
        Dim MyWeekDay As Integer, MyMonth As String
        Dim FreeSpace As Long
        Dim SectorsPerCluster As Long
        Dim BytesPerSector As Long
        Dim NumberOfFreeClusters As Long
        Dim TotalNumberOfClusters As Long
        '06/12/2016 Aggiunte le variabili per stabilire l'ultima data di aggiornamento dei file di LOG
        Dim d_file As Date
        Dim hFile As Long, rval As Long
        Dim buff As OFSTRUCT
        Dim ctime As FILETIME, atime As FILETIME, wtime As FILETIME
        Dim ftime As SYSTEMTIME
        ' fso = FileSystemObject replaced with System.IO.Directory
        '01/02/2019 Aggiunto per differenziare i giorni nelle varie settimane
        Dim DateRep As String

        Try

        Inizializza = True
        s_Ora = LeggiFileIni("Default", "Ora")
        ' TimeZoneOffset legge TZInfo/TZStructure (Time.vb), popolati solo
        ' da Refresh() tramite GetTimeZoneInformation: senza questa chiamata
        ' restano a zero (default struct) e TimeZoneOffset vale sempre 0,
        ' a prescindere dal fix su i_Offset — Refresh() non veniva mai
        ' invocata da nessuna parte nel progetto .NET.
        Refresh()
        i_Off = TimeZoneOffset
        ' i_Offset è lo stesso identico valore sotto un nome diverso: nel
        ' VB6 originale esisteva solo "i_Offset" (Gestione.bas: "i_Offset =
        ' TimeZoneOffset"), usato ovunque. Nel porting .NET è stato diviso
        ' in due variabili distinte (i_Off e i_Offset, AppState.vb) senza
        ' accorgersi che esistevano già entrambe: solo i_Off veniva
        ' valorizzato, i_Offset restava sempre 0 (default Integer).
        ' Risultato: tutti i punti che usano i_Offset (frmSimulazione,
        ' frmAcquire, frmGrafico, frmNorad, frm11Paramtri, Frm11Elementi)
        ' non convertivano mai Now (locale) in UTC — "Now.AddHours(-i_Offset)"
        ' restituiva sempre l'ora locale invariata.
        i_Offset = TimeZoneOffset
        Antenna = LeggiFileIni("Default", "Antenna")
        s_Satellite = LeggiFileIni("Default", "Satellite")
        PredisponiAcquisizione()
        DateRep = Format$(NowUTC(), "_dd")
        If Algoritmo = 1 Then
            Define_Derived_Constants()
            b_Sat = Convert_Satellite_Data(s_Satellite)
            '        If b_Sat Then
            '            If ideep = 0 Then
            '                InitSGP
            '        InitSGP4
            '            Else
            '                InitSGP
            '        InitSDP4
            '            End If
            '        End If
            b_Calcola = b_Sat
        ElseIf Algoritmo = 0 Or Algoritmo = 2 Then
            b_Calcola = InitPar11()
        Else
            b_Calcola = True
        End If
        PTK = &H0
        '    d_DataInizioProg = "31/12/3000 10.00.00"
        d_DataInizioProg = ""
        FrmMain.stpProgram.Text = d_DataInizioProg
        NumMsg = 1
        MyMonth = MonthName(Month(Now.AddHours(-CDbl(i_Off))))
        CreaFilesLog(MyMonth, DateRep)
        Response = GetDiskFreeSpace(Left$(Application.StartupPath, 3), SectorsPerCluster, BytesPerSector, NumberOfFreeClusters, TotalNumberOfClusters)
        FreeSpace = (NumberOfFreeClusters / 1048576) * BytesPerSector * SectorsPerCluster
        FrmMain.stpDisco.Text = FreeSpace / 1000 & " Gb"

        Response = GetDiskFreeSpace(Left$(Application.StartupPath, 3), SectorsPerCluster, BytesPerSector, NumberOfFreeClusters, TotalNumberOfClusters)
        FreeSpace = (NumberOfFreeClusters / 1048576) * BytesPerSector * SectorsPerCluster
        FrmMain.stpDisco.Text = FreeSpace / 1000 & " Gb"
        ' frmRicezione.MnuFile VB6 control array not migrated - default to OFF
        FrmMain.stpPrint.Text = "OFF"
        FrmMain.stpBeep.Text = "OFF"

        g_strLastConnection = ""
        i_Status = OP_STANDBY
        vBytesold(1) = &H20
        vBytesold(2) = &H0
        vBytesold(3) = &HF
        vBytesold(4) = &HF
        vBytesold(5) = &HD
        vBytesold(6) = &HC
        vBytesold(7) = &H2
        vBytesold(8) = &HD
        vBytesold(9) = &HB
        vBytesold(10) = &H2
        vBytesold(11) = &H0
        vBytesold(12) = &H2
        vBytesold(13) = &H0
        vBytesold(14) = &H0
        vBytesold(15) = &H0
        '    LanReset1 LanMon
        '    Attesa 0.3
        '    frmRicezione.sckMain(0).Connect SCK_REMOTE_HOST, SCK_LOCAL_PORT
        Catch ex As Exception
            WriteError(0, "Inizializza: " & ex.Message)
        End Try
    End Function

    Public Function Pulisci() As Boolean
        Pulisci = True
        ' frmRicezione.LblAllarme control array not migrated
        ' frmRicezione.TxtAz/TxtEl not migrated
    End Function

    Public Function OrdineKey(Code As Integer, Maschera As Integer) As Boolean
        OrdineKey = True
        Select Case Code
            Case Keys.F1
                'standby
                b_Ricezione(ACU) = False
                PTK = &H0
                '            d_DataInizioProg = CDate("31/12/3000 00.00.00")
                d_DataInizioProg = ""
                FrmMain.stpProgram.Text = d_DataInizioProg
                If Maschera = 0 Then
                    ' frmRicezione.TxtMode not migrated
                    FrmRicezione.LblAzimuth.Text = ""
                    FrmRicezione.LblElevazione.Text = ""
                    FrmRicezione.LblAzimuthC.Text = ""
                    FrmRicezione.LblElevazioneC.Text = ""
                End If
                '    LblInvio.Text = ""
                b_int(ACU) = True
                InvioACU(Chr(STX) & "08 S79" & Chr(ETX))
                FrmRicezione.LblCommand.Text = "Stand By"
                i_Status = OP_STANDBY
 '           PredisponiAcquisizione
            Case Keys.F2
                'command position
                i_Assi = 0
                FrmPreset.Show()
                i_Status = OP_STANDBY
            Case Keys.F3
                'step track si/no
                ' C10: flag impostati prima dell'invio (VB6 di C10)
                b_Ricezione(ACU) = False
                PTK = &H0
                d_DataInizioProg = ""
                FrmMain.stpProgram.Text = d_DataInizioProg
                b_int(ACU) = True
                InvioACU(Chr(STX) & "09 ST2C" & Chr(ETX))
                i_Status = OP_STEPTRACK
                FrmRicezione.LblCommand.Text = "Step Track"
            Case Keys.F4
                'autotrack si/no
                ' C10: flag impostati prima dell'invio (VB6 di C10)
                SEND = CheckSum("10 AP ")
                b_Ricezione(ACU) = False
                b_int(ACU) = True
                InvioACU(SEND)
                PTK = &H0
                d_DataInizioProg = ""
                FrmMain.stpProgram.Text = d_DataInizioProg
                i_Status = OP_AUTOTRACK
                FrmRicezione.LblCommand.Text = "Autotrack"
            Case Keys.F6
            'PLL Re-sweep
            Case Keys.F7
            'Loggin on
            Case Keys.F8
                If Maschera = 0 Then
                    'clear alarm
                    DbExecute("Delete * from [tmp_error_messages]")
                    ' Datlog/tdbLog controls not migrated
                End If
        End Select
    End Function

    ''' <summary>
    ''' Analisi della risposta ACU — porting diretto di ValoreCampoMAC dal
    ''' VB6 di C10 (Prog0347-FOCC10\Gestione.bas). Il protocollo ACU di C10
    ''' è lo stesso di FOC6/C03/K09 (VB6), NON quello di BTS1: il .NET di
    ''' FOC6 da cui è partito questo progetto usava per errore il parser
    ''' BTS1 (campi in posizioni diverse). Formato della risposta "*":
    '''   8-10 giorno giuliano, 11-19 ora hhmmssmmm, 7 bit1 = ora da IRIG
    '''   20 modo operativo/REM-LOC, 21 backup autotrack, 22 switches
    '''   23 SW prelimit, 24 major/minor fault, 25 HW prelimit/limit
    '''   26 emergency, 27 drive/freni, 28 pin El/pol fault, 29 pin Az
    '''   30 LNA e polarizzazione, 31 hand crank/low angle, 32 memorie
    '''   33-38 Az, 39-43 El, 44-47 Pol, 48 RX alarms
    '''   49-51 livello somma, 52-54/55-57 errori Az/El, 58-63 velocità
    '''   64 direzione motori, 65 assi abilitati, 66 stato step track
    ''' I livelli sono codificati in 3 byte (CalcoloVolt).
    ''' </summary>
    Public Function ValoreCampoMAC(Messaggio As String) As Boolean
        Dim d_Date As Date
        Dim yy As String = "", mm As String = "", gg As String = ""
        Dim irig_hour As Integer, irig_min As Integer, irig_sec As Integer, irig_millsec As Integer
        Dim julian_epoch1 As Double, irig_Acu As Double
        Dim i_AzDeg As String, i_ElDeg As String
        s_MsgAcu = ""
        ValoreCampoMAC = True
        Try
            CampoOp = Mid$(Messaggio, 6, 1)
            If b_MCS Then
                ' Risposte a richieste arrivate dall'MCS: si inoltrano all'MCS
                If CampoOp = "?" OrElse CampoOp = "#" OrElse CampoOp = ">" OrElse CampoOp = "<" OrElse CampoOp = "@" Then
                    Invio(Messaggio, MAC)
                    b_MCS = False
                    Exit Function
                End If
            Else
                If CampoOp = "?" OrElse CampoOp = "#" Then
                    s_Parametri = Messaggio
                    FrmParametri.ValoreParametri(s_Parametri)
                    Exit Function
                ElseIf CampoOp = ">" Then
                    s_Parametri = Messaggio
                    FrmMemoryTrack.ValoreMemory(s_Parametri)
                    Exit Function
                ElseIf CampoOp = "<" Then
                    s_Parametri = Messaggio
                    FrmTabSat.ValoreSat(s_Parametri)
                    Exit Function
                ElseIf b_Parametri Then
                    Exit Function
                End If
            End If

            If CampoOp = "*" Then
                ' Controllo che l'orologio ACU avanzi (11/11/2024)
                irig_sec = CInt(Val(Mid$(Messaggio, 15, 2)))
                irig_millsec = CInt(Val(Mid$(Messaggio, 17, 3)))
                irig_Acu = Val(Mid$(Messaggio, 15, 5))
                If irig_Acu = ACUOldMilSec Then
                    errTime += 1
                    If errTime = 3 Then
                        FrmMain.stpData.Text = ""
                        FrmRicezione.LblConfCNL1in.BackColor = Color.Red
                        TagDato = ""
                        BuffStatus = ""
                        ScriviAlarm("Time ACU not valid", 1)
                        errTime = 0
                    End If
                Else
                    errTime = 0
                End If
                ACUOldMilSec = irig_Acu

                s_Az = 0 : s_El = 0 : s_POL = 0 : s_Lev = 0 : s_LevAz = 0 : s_LevEl = 0
                s_Mod = 0 : s_AzTS = 0 : s_ElTS = 0 : s_Appoggio = "" : s_Appoggio1 = ""
                i_Pol = 0
                i_Alarm = False
                s_StatusPTIC(1) = 1         'ptic control
                s_StatusPTIC(4) = 0
                s_StatusPTIC(14) = &HF      'tutti abilitati

                vBytes(1) = Asc(Mid$(Messaggio, 20, 1))     ' Modo operativo, REM/LOC, alarm code
                vBytes(2) = Asc(Mid$(Messaggio, 23, 1))     ' SW prelimiti
                vBytes(3) = Asc(Mid$(Messaggio, 25, 1))     ' H/W prelimit, H/W limit
                vBytes(4) = Asc(Mid$(Messaggio, 24, 1))     ' Major Minor Fault
                vBytes(5) = Asc(Mid$(Messaggio, 27, 1))     ' Drive, Brackes Az El
                vBytes(6) = Asc(Mid$(Messaggio, 26, 1))     ' Emergency Stop
                vBytes(7) = Asc(Mid$(Messaggio, 29, 1))     ' Az Stow pin, El Extended
                vBytes(8) = Asc(Mid$(Messaggio, 28, 1))     ' El Stow Pin, Pol Limit e Fault
                vBytes(9) = Asc(Mid$(Messaggio, 31, 1))     ' Az El Hand-crank interlock, Low angle
                vBytes(10) = Asc(Mid$(Messaggio, 30, 1))    ' LNA e polarizzazione
                vBytes(11) = Asc(Mid$(Messaggio, 32, 1))    ' Memory alarms
                vBytes(12) = Asc(Mid$(Messaggio, 48, 1))    ' Rx alarms
                vBytes(13) = Asc(Mid$(Messaggio, 64, 1))    ' AC motors direction commands
                vBytes(14) = Asc(Mid$(Messaggio, 65, 1))    ' Axis enable
                vBytes(15) = Asc(Mid$(Messaggio, 66, 1))    ' Step track status
                s_Az = Mid$(Messaggio, 33, 6)               ' Azimuth
                s_El = Mid$(Messaggio, 39, 5)               ' Elevazione
                s_POL = Mid$(Messaggio, 44, 4)              ' Polarizzazione
                s_Lev2 = Mid$(Messaggio, 49, 3)             ' Livello somma (beacon)
                s_LevAz = Mid$(Messaggio, 52, 3)            ' Livello errore azimuth
                s_LevEl = Mid$(Messaggio, 55, 3)            ' Livello errore elevazione
                s_Mod = Asc(Mid$(Messaggio, 20, 1))         ' Modo operativo
                s_AzTS = Mid$(Messaggio, 58, 3)
                s_ElTS = Mid$(Messaggio, 61, 3)
                s_Appoggio = Mid$(Messaggio, 21, 1)
                s_Appoggio1 = Mid$(Messaggio, 22, 1)

                ' Modo del polarizzatore: tipo da vBytes(9) bit1, "in
                ' posizione" da vBytes(10) bit3, altrimenti Stand-by
                i_Pol = CByte(vBytes(9) And BIT1)
                If (vBytes(10) And BIT3) = BIT3 Then
                    d_Pol = If(i_Pol = 0, "L", "C")
                Else
                    d_Pol = "S"
                End If

                Select Case (s_Mod And &HF)
                    Case 0
                        s_StatusPTIC(2) = 0
                    Case 1
                        If i_Status = OP_AUGMENTED And i_ProgTrack = 2 Then
                            s_StatusPTIC(2) = 12
                            s_StatusPTIC(4) = 4
                        ElseIf i_Status = OP_AUGMENTED And i_ProgTrack = 1 Then
                            s_StatusPTIC(2) = 9
                            s_StatusPTIC(4) = 8
                        ElseIf i_Status = OP_PROGRAMTRACK Then
                            s_StatusPTIC(2) = 9
                            s_StatusPTIC(4) = 8
                        Else
                            s_StatusPTIC(2) = 4
                        End If
                    Case 4
                        ' (VB6 C10: il secondo ramo ripete "i_ProgTrack = 2"
                        ' e non viene mai preso: comportamento mantenuto)
                        If i_Status = OP_AUGMENTED And i_ProgTrack = 2 Then
                            s_StatusPTIC(2) = 12
                        Else
                            s_StatusPTIC(2) = 2
                        End If
                        s_StatusPTIC(4) = 1
                    Case 5
                        s_StatusPTIC(2) = 5
                        s_StatusPTIC(4) = 2
                    Case 6
                        s_StatusPTIC(2) = 7
                    Case 7
                        s_StatusPTIC(2) = 8
                End Select

                ' Data/ora ACU: giorno giuliano (pos 8-10) + hhmm (11-14)
                irig_hour = CInt(Val(Mid$(Messaggio, 11, 2)))
                irig_min = CInt(Val(Mid$(Messaggio, 13, 2)))
                julian_epoch1 = Julian_Date_of_Year(Year(Now)) + Val(Mid$(Messaggio, 8, 3)) + Fraction_of_Day(irig_hour, irig_min, irig_sec, 0)
                d_Date = Calendar_Date(julian_epoch1, 0)
                ' A cavallo d'anno (data ACU di dicembre, PC già a gennaio)
                If d_Date.Month = 12 AndAlso Now.AddHours(-i_Offset).Month = 1 Then
                    yy = (Year(Now) - 1).ToString().Substring(2, 2)
                Else
                    yy = Year(Now).ToString().Substring(2, 2)
                End If
                mm = d_Date.Month.ToString("00")
                gg = d_Date.Day.ToString("00")
                s_MsgAcu = d_Date.Day.ToString("00") & irig_hour.ToString("00") & irig_min.ToString("00") & irig_sec.ToString("00") & irig_millsec.ToString("000")
                d_Date = New Date(d_Date.Year, d_Date.Month, d_Date.Day, irig_hour Mod 24, irig_min Mod 60, irig_sec Mod 60)
                Dim d_DateTxt As String = d_Date.ToString("dd/MM/yy HH.mm.ss", Globalization.CultureInfo.InvariantCulture)
                g_Irig = "A"
                If (Asc(Mid$(Messaggio, 7, 1)) And BIT1) <> 0 Then
                    g_Irig = "I"
                    FrmMain.stpData.Text = d_DateTxt & " IRIG"
                Else
                    FrmMain.stpData.Text = d_DateTxt & " ACU"
                End If
                OraACU = d_Date
                If Ptic = "A" Then
                    TagDato = s_Az & "0" & s_El & Mid$(Messaggio, 11, 8)
                Else
                    TagDato = s_Az & "0" & s_El & yy & mm & gg & Mid$(Messaggio, 11, 8)
                End If
                If s_Appoggio <> "@" Then i_BackupAuto = s_Appoggio
                i_Switches = s_Appoggio1

                'M&C ACU
                If (vBytes(1) And BIT6) = BIT6 Then
                    s_ACULR = "R"
                    If (vBytes(1) And BIT6) <> (vBytesold(1) And BIT6) Then ScriviAlarm("ACU REM", 0)
                Else
                    s_ACULR = "L"
                    If (vBytes(1) And BIT6) <> (vBytesold(1) And BIT6) Then ScriviAlarm("ACU LOC", 0)
                End If

                ' Prelimiti software (vBytes(2))
                AllarmeSeAttivo(2, BIT3, "Azimuth CW Software Prelimit alarm", 1)
                AllarmeSeAttivo(2, BIT4, "Azimuth CCW Software Prelimit alarm", 1)
                AllarmeSeAttivo(2, BIT1, "Elevation Up Software Prelimit alarm", 1)
                AllarmeSeAttivo(2, BIT2, "Elevation Down Software Prelimit alarm", 1)
                AllarmeSeAttivo(2, BIT6, "Polatization CW Software Prelimit alarm", 1)
                AllarmeSeAttivo(2, BIT5, "Polarization CCW Software Prelimit alarm", 1)

                ' Prelimiti/limiti hardware (vBytes(3), attivi a bit = 0)
                If (vBytes(3) And BIT1) <> BIT1 Then
                    s_StatusPTIC(9) = s_StatusPTIC(9) Or &H2
                    If (vBytes(3) And BIT1) <> (vBytesold(3) And BIT1) Then ScriviAlarm("Azimuth Prelimit alarm", 1)
                Else
                    s_StatusPTIC(9) = s_StatusPTIC(9) And &HD
                End If
                If (vBytes(3) And BIT2) <> BIT2 Then
                    s_StatusPTIC(7) = s_StatusPTIC(7) Or &H2
                    If (vBytes(3) And BIT2) <> (vBytesold(3) And BIT2) Then ScriviAlarm("Elevation Prelimit alarm", 1)
                Else
                    s_StatusPTIC(7) = s_StatusPTIC(7) And &HD
                End If
                If (vBytes(3) And BIT3) <> BIT3 Then
                    s_StatusPTIC(9) = s_StatusPTIC(9) Or &H1
                    i_Alarm = True
                    If (vBytes(3) And BIT3) <> (vBytesold(3) And BIT3) Then ScriviAlarm("Azimuth Limit alarm", 1)
                Else
                    s_StatusPTIC(9) = s_StatusPTIC(9) And &HE
                End If
                If (vBytes(3) And BIT4) <> BIT4 Then
                    i_Alarm = True
                    s_StatusPTIC(7) = s_StatusPTIC(7) Or &H1
                    If (vBytes(3) And BIT4) <> (vBytesold(3) And BIT4) Then ScriviAlarm("Elevation Limit alarm", 1)
                Else
                    s_StatusPTIC(7) = s_StatusPTIC(7) And &HE
                End If

                ' Major/minor fault (vBytes(4), attivi a bit = 0)
                If (vBytes(4) And BIT1) <> BIT1 Then
                    i_Alarm = True
                    s_StatusPTIC(9) = s_StatusPTIC(9) Or &H8
                    If (vBytes(4) And BIT1) <> (vBytesold(4) And BIT1) Then ScriviAlarm("Azimuth Major Fault", 1)
                Else
                    s_StatusPTIC(9) = s_StatusPTIC(9) And &H7
                End If
                If (vBytes(4) And BIT2) <> BIT2 Then
                    i_Alarm = True
                    s_StatusPTIC(7) = s_StatusPTIC(7) Or &H8
                    If (vBytes(4) And BIT2) <> (vBytesold(4) And BIT2) Then ScriviAlarm("Elevation Major Fault", 1)
                Else
                    s_StatusPTIC(7) = s_StatusPTIC(7) And &H7
                End If
                If (vBytes(4) And BIT3) <> BIT3 Then
                    s_StatusPTIC(9) = s_StatusPTIC(9) Or &H4
                    If (vBytes(4) And BIT3) <> (vBytesold(4) And BIT3) Then ScriviAlarm("Azimuth Minor Fault", 1)
                Else
                    s_StatusPTIC(9) = s_StatusPTIC(9) And &HB
                End If
                If (vBytes(4) And BIT4) <> BIT4 Then
                    s_StatusPTIC(7) = s_StatusPTIC(7) Or &H4
                    If (vBytes(4) And BIT4) <> (vBytesold(4) And BIT4) Then ScriviAlarm("Elevation Minor Fault", 1)
                Else
                    s_StatusPTIC(7) = s_StatusPTIC(7) And &HB
                End If

                ' Drive / PCU / freni (vBytes(5))
                If (vBytes(5) And BIT1) <> BIT1 AndAlso (vBytes(5) And BIT1) <> (vBytesold(5) And BIT1) Then ScriviAlarm("Drive ON", 0)
                AllarmeSeAttivo(5, BIT2, "PCU Control ON", 1)
                If (vBytes(5) And BIT3) <> BIT3 Then
                    s_StatusPTIC(8) = s_StatusPTIC(8) Or &H1
                Else
                    s_StatusPTIC(8) = s_StatusPTIC(8) And &HE
                End If
                If (vBytes(5) And BIT4) <> BIT4 Then
                    s_StatusPTIC(10) = s_StatusPTIC(10) Or &H1
                Else
                    s_StatusPTIC(10) = s_StatusPTIC(10) And &HE
                End If

                ' Emergency stop (vBytes(6), attivi a bit = 0)
                If (vBytes(6) And BIT3) <> BIT3 Then
                    i_Alarm = True
                    s_StatusPTIC(5) = s_StatusPTIC(5) Or &H1
                    If (vBytes(6) And BIT3) <> (vBytesold(6) And BIT3) Then ScriviAlarm("Emergency Stop In", 1)
                Else
                    s_StatusPTIC(5) = s_StatusPTIC(5) And &HE
                End If
                If (vBytes(6) And BIT4) <> BIT4 Then
                    i_Alarm = True
                    If (vBytes(6) And BIT4) <> (vBytesold(6) And BIT4) Then ScriviAlarm("Emergency Stop Out", 1)
                End If

                ' Pin di elevazione (vBytes(7) bit4, vBytes(8) bit1/bit2)
                If (vBytes(7) And BIT4) = BIT4 Then
                    s_StatusPTIC(8) = s_StatusPTIC(8) Or &H4
                    If (vBytes(7) And BIT4) <> (vBytesold(7) And BIT4) Then ScriviAlarm("Elevation Pin Extended", 0)
                Else
                    s_StatusPTIC(8) = s_StatusPTIC(8) And &HB
                End If
                If (vBytes(8) And BIT1) = BIT1 Then
                    s_StatusPTIC(8) = s_StatusPTIC(8) Or &H8
                    If (vBytes(8) And BIT1) <> (vBytesold(8) And BIT1) Then ScriviAlarm("Elevation Pin retracted", 0)
                Else
                    s_StatusPTIC(8) = s_StatusPTIC(8) And &H7
                End If
                If (vBytes(8) And BIT2) = BIT2 Then
                    s_StatusPTIC(8) = s_StatusPTIC(8) Or &H2
                    If (vBytes(8) And BIT2) <> (vBytesold(8) And BIT2) Then ScriviAlarm("Elevation Pin Aligned", 0)
                Else
                    s_StatusPTIC(8) = s_StatusPTIC(8) And &HD
                End If

                ' Allarmi memoria (vBytes(11))
                AllarmeSeAttivo(11, BIT1, "Memory Parameters alarm", 0)
                AllarmeSeAttivo(11, BIT2, "Memory track alarm", 0)
                AllarmeSeAttivo(11, BIT4, "Memory satellite data alarm", 0)
                AllarmeSeAttivo(11, BIT3, "Memory stow unstow alarm", 0)

                ' Ricevitore (vBytes(12))
                If (vBytes(12) And BIT1) <> BIT1 AndAlso (vBytes(12) And BIT1) <> (vBytesold(12) And BIT1) Then
                    ScriviAlarm("Rx alarm", If(i_LockBell = 1, 1, 0))
                End If
                If (vBytes(12) And BIT2) <> (vBytesold(12) And BIT2) Then
                    ScriviAlarm("Monopulse UnLock", 0)
                End If

                For i = 1 To 15
                    vBytesold(i) = vBytes(i)
                Next i
                FrmRicezione.AggiornaAllarmiMemoria()

                ' Livelli e velocità: 3 byte codificati (CalcoloVolt), ±10 V
                If IsNumeric(s_AzTS) Then s_AzTS = Mid$(s_AzTS, 1, 1) + Format$(CDbl(Mid$(s_AzTS, 2, 2)), "0")
                If IsNumeric(s_ElTS) Then s_ElTS = Mid$(s_ElTS, 1, 1) + Format$(CDbl(Mid$(s_ElTS, 2, 2)), "0")
                s_LevAz = Format$(CalcoloVolt(s_LevAz) - 10, "0.0#")
                s_LevEl = Format$(CalcoloVolt(s_LevEl) - 10, "0.0#")
                s_Lev = Format$((CalcoloVolt(s_Lev2) - 10) * Pendenza + Valore0, "##0.0#")
                s_AzTS = Format$(10 - CalcoloVolt(s_AzTS), "0.0#")
                s_ElTS = Format$(10 - CalcoloVolt(s_ElTS), "0.0#")
                i_AzDeg = Format$(Math.Abs(Val(s_LevAz) * 1000), "00000")
                i_ElDeg = Format$(Math.Abs(Val(s_LevEl) * 1000), "00000")
                i_AzDeg = If(Val(s_LevAz) > 0, "+", "-") & i_AzDeg
                i_ElDeg = If(Val(s_LevEl) > 0, "+", "-") & i_ElDeg
                TagError = i_AzDeg & i_ElDeg & Mid$(Messaggio, 11, 8)
                If i_Alarm Then
                    If i_Status <> OP_STANDBY Then FrmRicezione.CmdSTBY_Click(Nothing, EventArgs.Empty)       '04/02/2026
                End If

            ElseIf CampoOp = "@" Then
                ' Configurazione ACU: programma, nome, firmware. Il form
                ' .NET non ha i campi dedicati del VB6 (txtNumProg/
                ' txtNomeAcu/txtVersione): si conservano e si mostrano
                ' nella riga messaggi della configurazione, se aperta.
                s_AcuNumProg = Mid$(Messaggio, 7, 4)
                s_AcuNome = Mid$(Messaggio, 11, 6)
                s_AcuVersione = Val(Mid$(Messaggio, 17, 2)) & "." & Val(Mid$(Messaggio, 19, 2))
                If FrmConfigurazione.Visible Then
                    FrmConfigurazione.LblMSG.Text = "Prog. Nr. " & s_AcuNumProg & "   ACU " & s_AcuNome & "   Firmware " & s_AcuVersione
                End If
            End If

            ' Proiezione (aggiunta del porting .NET, comune a BTS1)
            If i_Status = OP_PROIEZ Then
                If IsNumeric(s_Az) And IsNumeric(s_El) Then
                    If Countpts() > NumProMax Then Removepts(0)
                    Addpts(NowUTC().ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture), CDbl(s_Az), CDbl(s_El))
                End If
            End If
        Catch ex As Exception
            FrmMain.stpMessaggio.Text = "Buffer error: " & ex.Message
            FrmMain.TmrMsg.Enabled = True
            WriteError(0, "ValoreCampoMAC: " & ex.Message)
        End Try
    End Function

    ''' <summary>Allarme sul fronte di salita di un bit attivo a 1 di vBytes(idx).</summary>
    Private Sub AllarmeSeAttivo(idx As Integer, bit As Integer, testo As String, livello As Integer)
        If (vBytes(idx) And bit) = bit AndAlso (vBytes(idx) And bit) <> (vBytesold(idx) And bit) Then
            ScriviAlarm(testo, livello)
        End If
    End Sub

    ' Dati della risposta "@" (configurazione ACU)
    Public s_AcuNumProg As String = ""
    Public s_AcuNome As String = ""
    Public s_AcuVersione As String = ""


    Public Function RefreshTLE() As Boolean
        Dim Agg As Boolean
        Dim d_nonvalido As Date
        Try
        RefreshTLE = False
        rs_Orbite = dbOrbite.OpenRecordset("Satelliti", dbOpenDynaset)
        If rs_Orbite.BOF Or s_Satellite = "" Or s_Satellite = "REMOTE" Then
            Exit Function
        Else
            rs_Orbite.FindFirst("NomeSatellite ='" & s_Satellite & "'")
            If Not rs_Orbite.NoMatch Then
                If Not IsDBNull(rs_Orbite!FileAgg) And rs_Orbite!FileAgg <> "" Then
                    s_TwoLine = rs_Orbite!FileAgg
                    i = InStr(1, s_TwoLine, ":", 1)
                    If i = 0 Then s_TwoLine = Application.StartupPath & "\" & s_TwoLine
                    Agg = AggiornaTLE(s_TwoLine, 2)
                    If Agg Then
                        RefreshTLE = True
                    Else
                        Exit Function
                    End If
                Else
                    Exit Function
                End If
                g_Verso = (rs_Orbite!Verso?.ToString().ToUpper() = "VERO" OrElse rs_Orbite!Verso?.ToString().ToUpper() = "TRUE")  '08/11/2025
            Else
                Exit Function
            End If
        End If
        Define_Derived_Constants()
        b_Sat = Convert_Satellite_Data(s_Satellite)
        b_Calcola = b_Sat
        i_Cont = 0
        PredisponiAcquisizione()
        Catch ex As Exception
            WriteError(0, "RefreshTLE: " & ex.Message)
        End Try
    End Function

    Public Function ScriviLOG(Messaggio As String, Tipo As Integer) As Boolean
        'Questa routine scrive tutto il messaggio inviato dalla ACU
        ' Tipo = 0   E' un comando del monitor e control
        ' Tipo = 1   E' una risposta della ACU
        ' scrive sul file di log ACU.Txt e sulla stampante il messaggio
        Dim Buffer As String
        Dim Ind As Integer, i_lun As Integer, s_msg As String, Mess As String
        Dim lunMax As Integer, pos As Integer, i As Integer
        Dim i_Conto As Integer
        '    Dim s_Ch As String
        Dim FreeSpace As Long
        Dim SectorsPerCluster As Long
        Dim BytesPerSector As Long
        Dim NumberOfFreeClusters As Long
        Dim TotalNumberOfClusters As Long, Port As Integer
        Try
        ScriviLOG = True
        If b_Parametri Then Exit Function
        Response = GetDiskFreeSpace(Left$(Application.StartupPath, 3), SectorsPerCluster, BytesPerSector, NumberOfFreeClusters, TotalNumberOfClusters)
        FreeSpace = (NumberOfFreeClusters / 1048576) * BytesPerSector * SectorsPerCluster
        If FreeSpace < 10 Then
            FrmMain.stpMessaggio.Text = "Disk Full"
            FrmMain.TmrMsg.Enabled = True
            Exit Function
        End If

        lunMax = 75
        '    ScriviLOG1 Messaggio, tipo
        ' preparo il buffer per la scrittura
        If Tipo = 0 Then
            Mess = Mid$(Messaggio, 6, 1)
            Select Case Mess
                Case "*"
                    Messaggio &= "Richiesta stato"
                Case "?"
                    Messaggio &= "Richiesta parametri"
                Case ">"
                    Messaggio &= "Richiesta Memory Track Data"
                Case "<"
                    Messaggio &= "Richiesta Satellite Data"
                Case ""
                    Messaggio &= "Richiesta Stow/Unstow Data"
                Case "@"
                    Messaggio &= "Richiesta Configurazione ACU"
                Case "!"
                    Messaggio &= "Richiesta Allarmi ACU"
            End Select
            Buffer = "M " & NowUTC().ToString("dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) & "  " & Messaggio
        Else
            Buffer = Messaggio
            Mess = Mid$(Buffer, 1, 1)
            Select Case Mess
                Case "*"
                    Messaggio &= "Status "
                    Select Case s_Mod And &HF
                        Case 0
                            Messaggio &= "Stand By "
                        Case 1
                            Messaggio &= "Pre "
                        Case 2
                            Messaggio &= "Manual slew "
                        Case 3
                            Messaggio &= "Manual Velocity "
                        Case 4
                            Messaggio &= "Monopulse Autotrack "
                        Case 5
                            Messaggio &= "Step Track "
                        Case 6
                            Messaggio &= "Memory Track "
                        Case 7
                            Messaggio &= "Automatic Acquire "
                        Case 8
                            Messaggio &= "Satellite "
                        Case 9
                            Messaggio &= "Memory interpolation step-track "
                        Case 10
                            Messaggio &= "Memory Rate "
                        Case 11
                            Messaggio &= "Automatic Stow "
                        Case 12
                            Messaggio &= "Automatic Un-Stow "
                        Case 13
                            Messaggio &= "Pin In "
                        Case 14
                            Messaggio &= "Pin Out "
                        Case 15
                            Messaggio &= "Step Pre "
                    End Select
                    Messaggio &= "ACU " & IIf((s_Mod And BIT6) = BIT6, "REM", "LOC") & Chr(CR) & vbLf
                    Messaggio &= "Az. " & Format$(CDbl(Mid$(s_Az, 1, 6)) / 1000, "000.000") & Chr(CR) & vbLf
                    Messaggio &= "El. " & Format$(CDbl(Mid$(s_El, 1, 5)) / 1000, "00.000") & Chr(CR) & vbLf
                    '                Messaggio &=   "Canale Beacon " & s_Ch & Chr(CR) & vbLf
                    Messaggio &= "Livello " & s_Lev & Chr(CR) & vbLf
                    Messaggio &= IIf((vBytesold(12) And BIT1) = BIT1, "LOW SIGNAL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(12) And BIT3) = BIT3, "LOW ALARM" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(12) And BIT5) = BIT5, "HIGH ALARM" & Chr(CR) & vbLf, "")
                    Messaggio &= "ALARM: " & Chr(CR) & vbLf
                    Messaggio &= IIf((vBytesold(2) And BIT1) = BIT1, "UPSL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(2) And BIT2) = BIT2, "DWSL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(2) And BIT3) = BIT3, "CWSL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(2) And BIT4) = BIT4, "CCWSL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(3) And BIT1) <> BIT1, "AZP" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(3) And BIT2) <> BIT2, "ELP" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(3) And BIT3) <> BIT3, "AZL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(3) And BIT4) <> BIT4, "ELL" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(4) And BIT1) <> BIT1, "AZMAF" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(4) And BIT2) <> BIT2, "ELMAF" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(4) And BIT3) <> BIT3, "AZMIF" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(4) And BIT4) <> BIT4, "ELMIF" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(5) And BIT1) <> BIT1, "DRIVE ON" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(5) And BIT2) = BIT2, "PCU" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(5) And BIT3) <> BIT3, "AZ BRACKES" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(5) And BIT4) <> BIT4, "EL BRACKES" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(6) And BIT3) <> BIT3, "EM.STOP IN" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(6) And BIT4) <> BIT4, "EM.STOP OUT" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(7) And BIT1) = BIT1, "AZ STOW PIN EXTENDED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(7) And BIT2) = BIT2, "AZ STOW PIN RETRACTED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(7) And BIT3) = BIT3, "AZ STOW PIN ALIGNED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(7) And BIT4) = BIT4, "EL STOW PIN EXTENDED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(8) And BIT1) = BIT1, "EL STOW PIN RETRACTED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(8) And BIT2) = BIT2, "EL STOW PIN ALIGNED" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(8) And BIT3) = BIT3, "POL LIMIT" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(8) And BIT4) = BIT4, "POL FAULT" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(9) And BIT1) = BIT1, "AZ HAND-CRANK INTERLOCK" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(9) And BIT2) = BIT2, "EL HAND-CRANK INTERLOCK" & Chr(CR) & vbLf, "")
                    Messaggio &= IIf((vBytesold(9) And BIT3) = BIT3, "LOW ANGLE" & Chr(CR) & vbLf, "")
                    Messaggio &= "MEMORY ALM: " & Chr(CR) & vbLf
                    Messaggio &= IIf((vBytesold(11) And BIT1) = BIT1, "PARAM, ", "")
                    Messaggio &= IIf((vBytesold(11) And BIT2) = BIT2, "TRACK DATA, ", "")
                    Messaggio &= IIf((vBytesold(11) And BIT4) = BIT4, "SAT DATA, ", "")
                    Messaggio &= IIf((vBytesold(11) And BIT5) = BIT5, "STOW/UNSTOW ", "")
                Case "?"
                    Messaggio &= "Parametri"
                    pos = 2
                    Messaggio &= " Step = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " IT = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " AT = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " ET = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " SS = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " XT = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " YT = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " RT = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " CY = " & Mid$(Buffer, pos, 2) & Chr(CR) & vbLf
                    pos += 2
                    Messaggio &= " BM = " & Mid$(Buffer, pos, 1)
                    pos += 1
                    Messaggio &= " ATTS = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " ATTA = " & Mid$(Buffer, pos, 2)
                    pos += 4
                    Messaggio &= " ATTE = " & Mid$(Buffer, pos, 2) & Chr(CR) & vbLf
                    pos += 2
                    Messaggio &= " ALI = " & Mid$(Buffer, pos, 2)
                    pos += 2
                    Messaggio &= " ELI = " & Mid$(Buffer, pos, 2)
                    pos = 42
                    Messaggio &= " SPCC = " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                    pos += 5
                    Messaggio &= " SPCW = " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                    pos += 5
                    Messaggio &= " SPDW = " & Mid$(Buffer, pos, 2) & "." & Mid$(Buffer, pos + 2, 2)
                    pos += 4
                    Messaggio &= " SPUP = " & Mid$(Buffer, pos, 2) & "." & Mid$(Buffer, pos + 2, 2)
                    pos += 4
                    Messaggio &= " SPPCC = " & Mid$(Buffer, pos, 2) & "." & Mid$(Buffer, pos + 2, 2)
                    pos += 4
                    Messaggio &= " SPPCW = " & Mid$(Buffer, pos, 2) & "." & Mid$(Buffer, pos + 2, 2) & Chr(CR) & vbLf
                    pos += 4
                    Messaggio &= " AOS = " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                    pos = 233
                    Messaggio &= " EOS = " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                    pos = 406
                    Messaggio &= " POS = " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                Case ">"
                    Messaggio &= "Memory Track Data " & Chr(CR) & vbLf
                    pos = 2
                    For i = 1 To 96
                        Messaggio &= Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2) & " "
                        Messaggio &= Mid$(Buffer, pos + 5, 2) & "." & Mid$(Buffer, pos + 7, 2) & Chr(CR) & vbLf
                        pos += 9
                    Next
                Case "<"
                    Messaggio &= "Satellite Data " & Chr(CR) & vbLf
                    pos = 2
                    For i = 1 To 40
                        Messaggio &= "SAT NR.: " & Str(i) & " " & Mid$(Buffer, pos, 3) & "." & Mid$(Buffer, pos + 3, 2)
                        Messaggio &= " " & Mid$(Buffer, pos + 5, 2) & "." & Mid$(Buffer, pos + 7, 2)
                        Messaggio &= " " & Mid$(Buffer, pos + 8, 2) & "." & Mid$(Buffer, pos + 10, 2) & Chr(CR) & vbLf
                        pos += 13
                    Next
                Case ""
                    Messaggio &= "Stow/Unstow Data "
                    Messaggio &= " WSAL = " & Mid$(Buffer, 2, 3) & "." & Mid$(Buffer, 2 + 3, 2)
                    Messaggio &= " WSEL = " & Mid$(Buffer, 7, 2) & "." & Mid$(Buffer, 7 + 2, 2)
                    Messaggio &= " USAZ = " & Mid$(Buffer, 11, 3) & "." & Mid$(Buffer, 11 + 3, 2)
                    Messaggio &= " USEL = " & Mid$(Buffer, 16, 2) & "." & Mid$(Buffer, 16 + 2, 2)
                Case "@"
                    Messaggio &= "Configurazione ACU: "
                    Messaggio &= "Programma Nr. " & Mid$(Buffer, 1, 4) & " "
                    Messaggio &= "NomeACU " & Mid$(Buffer, 5, 6) & " "
                    Messaggio &= "Versione Nr. " & Val(Mid$(Buffer, 10, 2)) & "." & Val(Mid$(Buffer, 12, 2))
                Case "!"
                    Messaggio &= "Allarmi ACU: "
                    pos = 2
                    For i = 1 To 16
                        Messaggio &= " " & Mid$(Buffer, pos, 40) & Chr(CR) & vbLf
                        pos += 40
                    Next
            End Select
            Buffer = "D " & NowUTC().ToString("dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) & Chr(CR) & vbLf & Messaggio
        End If
        ' scrivo il messaggio sul file di log
        ' apro il file di log se non esiste lo creo
        FileOpen(1, Application.StartupPath + "\" + Canali(ACU).File_Log, OpenMode.Append)
        '    FileOpen(1, Canali(ACU).File_Log, OpenMode.Append)
        '    If Tipo = 1 And Mess = "<" Then
        PrintLine(1, Buffer)
        '    Else
        '        i_lun = Int(Len(Buffer) / lunMax)
        '        s_msg = ""
        '        If i_lun > 0 Then
        '            For ind = 1 To i_lun
        '                i_Conto = 1 + ((ind - 1) * lunMax)
        '                If ind = 1 Then
        '                    s_msg = Mid$(Buffer, i_Conto, lunMax)
        '                Else
        '                    s_msg = "    " + Mid$(Buffer, i_Conto, lunMax)
        '                End If
        '                print #1, s_msg
        '            Next
        '            i_lun = Len(Buffer) Mod lunMax
        '            If i_lun > 0 Then
        '                i_Conto = 1 + ((ind - 1) * lunMax)
        '                s_msg = "    " + Mid$(Buffer, i_Conto, i_lun)
        '                print #1, s_msg
        '            End If
        '        Else
        '            print #1, Buffer
        '        End If
        '    End If
        FileClose(1)
        Catch ex As Exception
            WriteError(0, "ScriviLOG: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Porting di InvioRic dal VB6 di C10 (Gestione.bas): invia al Down
    ''' Converter la frequenza beacon (in Hz, "FE"), al polarizzatore
    ''' l'angolo della fase (seriale PCU, "Z <modo><angolo>") e al
    ''' ricevitore elevazione di fase, Rev Az e frequenza d'ingresso
    ''' (InvioPhase), ciascuno solo se diverso dal valore a video.
    ''' Differenza da FOC6: lì la polarizzazione andava all'ACU ("PLP")
    ''' o via SNMP; il controllo "PolarizationAngle" del ricevitore
    ''' resta uguale.
    ''' </summary>
    Public Sub InvioRic()
        Dim inv As System.Globalization.CultureInfo = System.Globalization.CultureInfo.InvariantCulture
        Try
            FrmRicezione.TmrRic.Enabled = False

            ' Frequenza beacon al Down Converter
            If IsNumeric(g_Frequenza) AndAlso Val(g_Frequenza) > 0 Then
                If FrmRicezione.TxtFrqBeacon.Text <> Format$(Val(g_Frequenza), "#0000.000") Then
                    Dim d_FrqBeacon As Double = Val(g_Frequenza) * 100000      ' in Hz
                    Send_GRF = CheckDC("{" & Chr(AddressDC) & "FE" & Format$(d_FrqBeacon, "0000000000") & "}")
                    b_Ricezione(GRF) = False
                    Invio(Send_GRF, GRF)
                    FrmRicezione.TxtFrqBeacon.Text = ""
                    Attesa(0.3)
                    b_frequency = True                  '12/11/2022 inviato ordine di frequenza
                End If
            End If

            ' Polarizzazione (solo in lineare), seriale PCU
            Dim valPol As Double
            If g_CirLin = "Linear" AndAlso Double.TryParse(CStr(g_ValPol).Replace(",", "."), Globalization.NumberStyles.Float, inv, valPol) Then
                If valPol.ToString("0.##", inv) <> FrmRicezione.TxtPol.Text.Replace(",", ".") Then
                    Dim s_Polar As String = "Z " & d_Pol & If(valPol < 0, "-", "+") & Math.Abs(valPol).ToString("00.0", inv)
                    Dim lunghezza As Integer = Len(s_Polar) + 6
                    b_int(PCU) = True
                    b_Ricezione(PCU) = True
                    Invio(CheckSum(lunghezza.ToString("00") & s_Polar), PCU)
                End If
            End If

            ' Ricevitore: elevazione di fase
            If IsNumeric(g_PhaseEl) AndAlso Val(g_PhaseEl) > 0 AndAlso g_PhaseEl <> FrmRicezione.TxtPhaseEl.Text Then
                InvioPhase("+" & g_PhaseEl, 0, 5, ComponentCode)
            End If

            ' Ricevitore: Rev Az
            If g_RevAz = "N" OrElse g_RevAz = "Y" Then
                If g_RevAz <> FrmRicezione.LblRevAz_ctrl.Text Then
                    FrmRicezione.LblRevAz_ctrl.Text = ""
                    InvioPhase(If(g_RevAz = "Y", "1", "0"), 0, 6, ComponentCode)
                End If
            End If

            ' Ricevitore: frequenza d'ingresso
            If IsNumeric(g_Frequenza) AndAlso Val(g_Frequenza) > 0 AndAlso Val(g_Frequenza) <> Val(FrmRicezione.TxtInpFreq.Text) Then
                InvioPhase(Format$(Val(g_Frequenza) * 1000, "00000000"), 0, 10, ComponentCode)
            End If

            ' Ricevitore: angolo di polarizzazione
            If IsNumeric(g_ValPol) AndAlso Val(g_ValPol) <> Val(Format$(PolarizationAngle, "0.00")) Then
                InvioPhase(g_ValPol, 1, 7, ComponentCode)
            End If
        Catch ex As Exception
            WriteError(0, "InvioRic: " & ex.Message)
        End Try
        g_NewFrequency = False
    End Sub
    Public Function InvioFrequenza()
        Dim rs_InvioFrequenza As Object
        Dim OffAz As System.Single, OffEl As System.Single, valoff As String = ""
        Try
        If Not IsNumeric(g_Frequenza) Or Val(g_Frequenza) = 0 Then Exit Function
        f_FrqBeacon = 0
        '          Attesa 0.3
        '          InvioPhase Format$(d_FrqBeacon, "00000000"), 0, 10, ComponentCode
        'frmRicezione.TxtFrqBeacon = ""
        ' LblPol/TxtLna/LblLNA not migrated - use globals
        If g_Polar = "" Then g_Polar = "RHCP"
        '            If frmRicezione.TxtPolErr = "" Then
        '                    frmRicezione.TxtPolErr = "RHCP"
        '            End If
        '            g_PolErr = frmRicezione.TxtPolErr
        ' frmRicezione.TxtLna not migrated
        '            tmpSQL = "SELECT * FROM [Fasi] where Frequenza = '" & Format$(g_Frequenza, "#000.00#") & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "' and RevAzErr = '" & g_RevAz & "' and PolErr = '" & g_PolErr & "'"
        tmpSQL = "SELECT * FROM [Fasi] where CDbl(Frequenza) = '" & CDbl(g_Frequenza) & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "'"
        '"  'and RevAzErr = '" & g_RevAz & "'"
        rs_InvioFrequenza = dbOrbite.OpenRecordset(tmpSQL, dbOpenDynaset)
        If Not rs_InvioFrequenza.BOF Then
            rs_InvioFrequenza.MoveFirst
            rs_InvioFrequenza.Edit

            If IsNumeric(rs_InvioFrequenza!faseEl) Then g_PhaseEl = rs_InvioFrequenza!faseEl
            If Not IsDBNull(rs_InvioFrequenza!faseAz) Then g_PhaseAz = rs_InvioFrequenza!faseAz
            g_LNA = rs_InvioFrequenza!LNA
            '                                If IsNumeric(rs_InvioFrequenza!PolAngle) Then g_ValPol = rs_InvioFrequenza!PolAngle
            If Not IsDBNull(rs_InvioFrequenza!CirLin) Then
                g_CirLin = rs_InvioFrequenza!CirLin
            Else
                g_CirLin = "Circular"
            End If
            If g_CirLin = "Linear" Then
                d_Pol = "L"
            Else
                d_Pol = "C"
            End If
            If Not IsDBNull(rs_InvioFrequenza!POL) Then
                g_Polar = rs_InvioFrequenza!POL
            Else
                g_Polar = "RHCP"
            End If

            '                If Not IsDBNull(rs_InvioFrequenza!PolErr) And rs_InvioFrequenza!PolErr <> "" Then
            '                    g_PolErr = rs_InvioFrequenza!PolErr
            '                Else
            '                    g_PolErr = "RHCP"
            '                End If
            If Not IsDBNull(rs_InvioFrequenza!RevAzErr) Then
                g_RevAz = rs_InvioFrequenza!RevAzErr
            Else
                g_RevAz = ""
            End If
            ' frmRicezione.TxtPhase/TxtPhAz/TxtRevAz/LblPolRic not migrated
            'frmRicezione.TxtFrqBeacon = ""
            '                frmRicezione.TxtPolErr = g_PolErr
            If IsNumeric(g_PhaseEl) Then
                PhaseValue1 = g_PhaseEl
            Else
                PhaseValue1 = 0
            End If
            If IsNumeric(g_PhaseAz) Then
                PhaseValue2 = g_PhaseAz
            Else
                PhaseValue2 = 0
            End If
            If IsNumeric(g_LNA) Then
                i_Lna = g_LNA
            Else
                i_Lna = 1
            End If
            t_Pol = g_Polar
            '                 If IsNumeric(g_Frequenza) Then
            '                     f_FrqBeacon = g_Frequenza
            '                 Else
            '                     f_FrqBeacon = 0
            '                 End If
            If IsNumeric(g_PhaseAz) Then
                PhaseValue2 = g_PhaseAz
            Else
                PhaseValue2 = 0
            End If
            If IsNumeric(g_LNA) Then
                i_Lna = g_LNA
            Else
                i_Lna = 0
            End If
        Else
            '15/04/2022
            If g_ChangePhase = "Y" Then
                rs_InvioFrequenza.Close
                Exit Function
            End If
            '22/11/2022
            PhaseValue1 = 0
            g_PhaseEl = 0
            g_PhaseAz = 0
            g_RevAz = ""
            ' frmRicezione.TxtPhase/TxtPhAz/TxtRevAz not migrated
        End If
        rs_InvioFrequenza.Close
        DbExecute("UPDATE [Fasi]  Ultima = " & vbUnchecked)
        b_Checked = False
        If s_Satellite = "REMOTE" Then
            tmpSQL = "SELECT * FROM [Fasi] where Sat ='" & FrmRicezione.LblSatellite.Text & "'"  'Nel caso che il satellite sia "REMOTE" imposto una sola stringa di frequenza
            rs_InvioFrequenza = dbOrbite.OpenRecordset(tmpSQL, dbOpenDynaset)
            If rs_InvioFrequenza.BOF Then
                rs_InvioFrequenza.AddNew
            Else
                rs_InvioFrequenza.Edit
            End If
            rs_InvioFrequenza!sat = FrmRicezione.LblSatellite.Text
            rs_InvioFrequenza!FREQUENZA = Format$(g_Frequenza, "#000.00#")
            rs_InvioFrequenza!POL = g_Polar
            If IsNumeric(g_LNA) Then
                rs_InvioFrequenza!LNA = g_LNA
            Else
                rs_InvioFrequenza!LNA = 1
            End If
            If IsNumeric(g_PhaseEl) Then rs_InvioFrequenza!faseEl = g_PhaseEl
            '               If IsNumeric(g_PhaseAz) Then rs_InvioFrequenza!faseAz = g_PhaseAz
            If Not IsDBNull(g_RevAz) Then rs_InvioFrequenza!RevAzErr = g_RevAz
            ' LblPol1/TxtPol not migrated - use g_CirLin/g_ValPol
            rs_InvioFrequenza!CirLin = "Circular"
            If g_CirLin = "Circular" Then
                If IsNumeric(g_ValPol) Then rs_InvioFrequenza!polangle = g_ValPol
            Else
                rs_InvioFrequenza!polangle = 0
            End If
            g_Frequenza = Format$(rs_InvioFrequenza!FREQUENZA, "#000.00#")
            g_PhaseEl = rs_InvioFrequenza!faseEl
            '                g_PhaseAz = rs_InvioFrequenza!faseAz
            g_LNA = rs_InvioFrequenza!LNA
            If IsNumeric(rs_InvioFrequenza!polangle) Then g_ValPol = rs_InvioFrequenza!polangle
            '                If Not IsDBNull(rs_InvioFrequenza!CirLin) Then
            g_CirLin = "Circular"
            '               Else
            '                    g_CirLin = ""
            '                End If
            If Not IsDBNull(rs_InvioFrequenza!POL) Then
                g_Polar = rs_InvioFrequenza!POL
            Else
                g_Polar = ""
            End If
            If Not IsDBNull(rs_InvioFrequenza!RevAzErr) Then
                g_RevAz = rs_InvioFrequenza!RevAzErr
            Else
                g_RevAz = ""
            End If
            'frmRicezione.TxtFrqBeacon = ""
            ' frmRicezione.TxtPhase/TxtPhAz/TxtLna/LblPolRic/TxtRevAz not migrated
            t_Pol = g_Polar
            '                 If IsNumeric(g_Frequenza) Then
            '                     f_FrqBeacon = g_Frequenza
            '                 Else
            '                     f_FrqBeacon = 0
            '                 End If
            If IsNumeric(g_PhaseEl) Then
                PhaseValue1 = g_PhaseEl
            Else
                PhaseValue1 = 0
            End If
            If IsNumeric(g_PhaseAz) Then
                PhaseValue2 = g_PhaseAz
            Else
                PhaseValue2 = 0
            End If
            If IsNumeric(g_LNA) Then
                i_Lna = g_LNA
            Else
                i_Lna = 0
            End If
        Else
            '15/04/2022
            If g_ChangePhase = "Y" Then
                '                    DbExecute("UPDATE [Fasi]  Ultima = " & vbUnchecked)
                ' LblPol/TxtLna not migrated - use globals
                tmpSQL = "SELECT * FROM [Fasi] where Sat ='" & FrmRicezione.LblSatellite.Text & "' and cdbl(Frequenza) = '" & CDbl(Format$(g_Frequenza, "#000.00")) & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "'"
                rs_InvioFrequenza = dbOrbite.OpenRecordset(tmpSQL, dbOpenDynaset)
                If Not rs_InvioFrequenza.BOF Then
                    rs_InvioFrequenza.MoveFirst
                    rs_InvioFrequenza.Edit
                    If IsNumeric(rs_InvioFrequenza!faseEl) And Val(rs_InvioFrequenza!faseEl) <> 0 Then g_PhaseEl = rs_InvioFrequenza!faseEl
                    If Not IsDBNull(rs_InvioFrequenza!faseAz) Then g_PhaseAz = rs_InvioFrequenza!faseAz
                    g_LNA = rs_InvioFrequenza!LNA
                    If IsNumeric(rs_InvioFrequenza!polangle) Then g_ValPol = rs_InvioFrequenza!polangle
                    '                       If Not IsDBNull(rs_InvioFrequenza!CirLin) Then
                    g_CirLin = "Circular"
                    '                        Else
                    '                            g_CirLin = ""
                    '                        End If
                    If Not IsDBNull(rs_InvioFrequenza!POL) Then
                        g_Polar = rs_InvioFrequenza!POL
                    Else
                        g_Polar = ""
                    End If
                    If Not IsDBNull(rs_InvioFrequenza!RevAzErr) Then
                        g_RevAz = rs_InvioFrequenza!RevAzErr
                    Else
                        g_RevAz = ""
                    End If
                    ' frmRicezione.TxtPhase/TxtPhAz/TxtRevAz/LblPolRic not migrated
                    If IsNumeric(g_PhaseEl) Then
                        PhaseValue1 = g_PhaseEl
                    Else
                        PhaseValue1 = 0
                    End If
                    If IsNumeric(g_PhaseAz) Then
                        PhaseValue2 = g_PhaseAz
                    Else
                        PhaseValue2 = 0
                    End If
                    If IsNumeric(g_LNA) Then
                        i_Lna = g_LNA
                    Else
                        i_Lna = 1
                    End If
                    '                t_Pol = g_Polar
                    '                 If IsNumeric(g_Frequenza) Then
                    '                     f_FrqBeacon = g_Frequenza
                    '                 Else
                    '                     f_FrqBeacon = 0
                    '                 End If
                Else
                    rs_InvioFrequenza.Update
                    rs_InvioFrequenza.Close
                    Exit Function
                End If
                '15/04/2022
            Else
                DbExecute("UPDATE [Fasi]  Ultima = " & vbUnchecked)
                b_Checked = False
                tmpSQL = "SELECT * FROM [Fasi] where Sat ='" & FrmRicezione.LblSatellite.Text & "' and cdbl(Frequenza) = '" & CDbl(Format$(g_Frequenza, "#000.00")) & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "'"
                rs_InvioFrequenza = dbOrbite.OpenRecordset(tmpSQL, dbOpenDynaset)
                If Not rs_InvioFrequenza.BOF Then
                    rs_InvioFrequenza.MoveFirst
                    rs_InvioFrequenza.Edit
                    If IsNumeric(rs_InvioFrequenza!faseEl) And Val(rs_InvioFrequenza!faseEl) <> 0 Then g_PhaseEl = rs_InvioFrequenza!faseEl
                    If Not IsDBNull(rs_InvioFrequenza!faseAz) Then g_PhaseAz = rs_InvioFrequenza!faseAz
                    g_LNA = rs_InvioFrequenza!LNA
                    If IsNumeric(rs_InvioFrequenza!polangle) Then g_ValPol = rs_InvioFrequenza!polangle
                    '                       If Not IsDBNull(rs_InvioFrequenza!CirLin) Then
                    g_CirLin = "Circular"
                    '                        Else
                    '                            g_CirLin = ""
                    '                        End If
                    If Not IsDBNull(rs_InvioFrequenza!POL) Then
                        g_Polar = rs_InvioFrequenza!POL
                    Else
                        g_Polar = ""
                    End If
                    If Not IsDBNull(rs_InvioFrequenza!RevAzErr) Then
                        g_RevAz = rs_InvioFrequenza!RevAzErr
                    Else
                        g_RevAz = ""
                    End If
                    ' frmRicezione.TxtPhase/TxtPhAz/TxtRevAz/LblPolRic not migrated
                    If IsNumeric(g_PhaseEl) Then
                        PhaseValue1 = g_PhaseEl
                    Else
                        PhaseValue1 = 0
                    End If
                    If IsNumeric(g_PhaseAz) Then
                        PhaseValue2 = g_PhaseAz
                    Else
                        PhaseValue2 = 0
                    End If
                    If IsNumeric(g_LNA) Then
                        i_Lna = g_LNA
                    Else
                        i_Lna = 1
                    End If
                    '                t_Pol = g_Polar
                    '                 If IsNumeric(g_Frequenza) Then
                    '                     f_FrqBeacon = g_Frequenza
                    '                 Else
                    '                     f_FrqBeacon = 0
                    '                 End If
                Else
                    rs_InvioFrequenza.AddNew
                    rs_InvioFrequenza!sat = FrmRicezione.LblSatellite.Text
                    rs_InvioFrequenza!FREQUENZA = Format$(g_Frequenza, "#000.00#")
                    rs_InvioFrequenza!POL = g_Polar
                    If IsNumeric(g_LNA) Then
                        rs_InvioFrequenza!LNA = g_LNA
                    Else
                        rs_InvioFrequenza!LNA = 1
                    End If
                    If Not IsDBNull(g_PolErr) Then rs_InvioFrequenza!PolErr = g_PolErr
                    If IsNumeric(g_PhaseEl) Then rs_InvioFrequenza!faseEl = g_PhaseEl
                    '               If IsNumeric(g_PhaseAz) Then rs_InvioFrequenza!faseAz = g_PhaseAz
                    If Not IsDBNull(g_RevAz) Then rs_InvioFrequenza!RevAzErr = g_RevAz
                    ' LblPol1/TxtPol not migrated - use g_CirLin/g_ValPol
                    rs_InvioFrequenza!CirLin = g_CirLin
                    If g_CirLin = "Linear" Then
                        If IsNumeric(g_ValPol) Then rs_InvioFrequenza!polangle = g_ValPol
                    Else
                        rs_InvioFrequenza!polangle = 0
                    End If
                End If
            End If
        End If
        rs_InvioFrequenza!Ultima = vbChecked
        b_Checked = True
        If Not IsDBNull(rs_InvioFrequenza!OffsetAz) Then OffAz = rs_InvioFrequenza!OffsetAz
        If Not IsDBNull(rs_InvioFrequenza!OffsetEl) Then OffEl = rs_InvioFrequenza!OffsetEl
        rs_InvioFrequenza.Update
        rs_InvioFrequenza.Close
        If IsNumeric(OffAz) And OffAz <> 0 Then
            If OffAz >= 0 And OffAz <= 99 Then
                valoff = CheckSum("14MOFA +" & Format$(Math.Abs(OffAz), "00"))
            ElseIf OffAz >= -99 And OffAz < 0 Then
                valoff = CheckSum("14MOFA -" & Format$(Math.Abs(OffAz), "00"))
            End If
            b_Ricezione(ACU) = False
            b_int(ACU) = True
            i_Cont = 2
            Attesa(0.2)
            InvioACU(valoff)
        End If
        If IsNumeric(OffEl) And OffEl <> 0 Then
            If OffEl >= 0 And OffEl <= 99 Then
                valoff = CheckSum("14MOFE +" & Format$(Math.Abs(OffEl), "00"))
            ElseIf OffEl >= -99 And OffEl < 0 Then
                valoff = CheckSum("14MOFE -" & Format$(Math.Abs(OffEl), "00"))
            End If
            b_Ricezione(ACU) = False
            b_int(ACU) = True
            i_Cont = 2
            Attesa(0.2)
            InvioACU(valoff)
        End If
        i_Cont = 0
        InvioRic()
        '        InvioRic
        ' frmRicezione.TxtInpFrequency not migrated
        FrmRicezione.TmrRic.Enabled = True
        FrmRicezione.TxtOrdini.Focus()
        Catch ex As Exception
            WriteError(0, "InvioFrequenza: " & ex.Message)
        End Try
    End Function
    Public Function CambioFreq(Freq As Double) As Boolean
        Dim d_FrqBeacon As Double, s_FrqBeacon As String, OffAz As System.Single, OffEl As System.Single, valoff As String = "", ii As Double
        CambioFreq = True
        If IsNumeric(Freq) And Freq > 0 Then
            d_FrqBeacon = Freq      'porto tutto a kHerz
            g_Frequenza = Format$(Freq / 1000, "#000.0#")                     '17/10/2022 (formato C10)
            InvioFrequenza()
            ' frmRicezione.TxtFrqBeacon/CmbBcnFrequency not migrated
            f_FrqBeacon = 0
        End If
    End Function
    Public Sub AverageLev(Level As String)
        'Questa funzione è dedicata alla media dei valori del beacon tra uno spostamento
        'e l'altro durante lo step track
        'Sposta i valori in tabella e calcola la media puntuale
        Dim LevAv As Double
        Dim AvCont As Integer
        Dim i As Double
        LevAv = 0
        AvCont = 0
        d_LevAve = Level                            'Per evitare un valore di media non valido
        For i = 1 To 9
            d_LevAv(i - 1) = d_LevAv(i)
            If d_LevAv(i - 1) <> &HFF Then
                LevAv += d_LevAv(i - 1)
                AvCont += 1
            End If
        Next i
        d_LevAv(i - 1) = Val(Level)
        AvCont += 1
        LevAv += d_LevAv(i - 1)
        If AvCont = 0 Then Exit Sub
        d_LevAve = LevAv / AvCont
    End Sub

    ' ---------------------------
    ' Funzione: IsArrayEmpty
    ' ---------------------------
    Public Function IsArrayEmpty() As Boolean
        On Error Resume Next
        IsArrayEmpty = (UBound(pts) < LBound(pts))
        If Err.Number <> 0 Then
            IsArrayEmpty = True
            Err.Clear()
        End If
    End Function

    ' ---------------------------
    ' Funzione: Countpts
    ' ---------------------------
    Public Function Countpts() As Long
        If IsArrayEmpty() Then
            Countpts = 0
        Else
            Countpts = UBound(pts) - LBound(pts) + 1
        End If
    End Function

    ' ---------------------------
    ' Procedura: Addpts
    ' ---------------------------
    Public Sub Addpts(data As String, az As String, el As String)
        Dim newIndex As Long

        If IsArrayEmpty() Then
            ReDim pts(0)
            newIndex = 0
        Else
            newIndex = UBound(pts) + 1
            ReDim Preserve pts(newIndex)
        End If
        pts(newIndex).T = ParseDateTime(data)
        pts(newIndex).az = CDbl(Mid$(s_Az, 1, 6)) / 1000
        pts(newIndex).el = CDbl(Mid$(s_El, 1, 5)) / 1000
    End Sub

    ' ---------------------------
    ' Procedura: Removepts
    ' ---------------------------
    Public Sub Removepts(ByVal Index As Long)
        Dim i As Long

        If IsArrayEmpty() Then Exit Sub
        If Index < LBound(pts) Then Exit Sub
        'Or Index > UBound(pts)
        For i = Index To UBound(pts) - 1
            pts(i) = pts(i + 1)
        Next i

        If UBound(pts) > 0 Then
            ReDim Preserve pts(UBound(pts) - 1)
        Else
            Erase pts
        End If
    End Sub

    ' ---------------------------
    ' Funzione: Getpts
    ' ---------------------------
    Public Function Getpts(ByVal Index As Long) As TrackPoint
        If Not IsArrayEmpty() Then
            If Index >= LBound(pts) And Index <= UBound(pts) Then
                Getpts = pts(Index)
            End If
        End If
    End Function

    ' ---------------------------
    ' Procedura: SaveptsToFile
    ' ---------------------------
    Public Sub SaveptsToFile(ByVal filePath As String)
        Dim F As Integer
        Dim i As Long

        If IsArrayEmpty() Then Exit Sub

        F = FreeFile()
        FileOpen(F, filePath, OpenMode.Binary, OpenAccess.Write)
        For i = LBound(pts) To UBound(pts)
            FilePut(F, pts(i))
        Next i
        FileClose(F)
    End Sub


    Private Const cmdPhasing As Integer = 0        'comando per dati phasing (Ricevitore.bas)

    ''' <summary>
    ''' Un tentativo di connessione per canale RX già in corso (vedi
    ''' ConnettiRXSenzaBloccare) — senza questa guardia, TmrPhase (ogni
    ''' 400ms) ne lanciava uno NUOVO ad ogni tick per MonComponent (LanMon)
    ''' e uno per Phasing (LanPhase), senza aspettare che il precedente
    ''' finisse: con il sotto-socket mai connesso (assenza di hardware
    ''' reale), ogni thread vive ~2s (SocketManager.ConnectRX, ".Wait(2000)")
    ''' ma uno nuovo parte ogni 400ms — a regime, ~5 thread per canale
    ''' sempre in volo, per tutta la durata della sessione, in continuo
    ''' accumulo. Causa più probabile dei blocchi di diversi secondi sul
    ''' thread UI osservati in test (TmrOrologio_Tick, gap fino a 6-7s) e
    ''' del peggioramento percepito con l'andare del tempo: non è il
    ''' singolo blocco da 2s (già portato fuori dal thread UI), è l'overhead
    ''' crescente di migliaia di thread OS creati e distrutti in rapida
    ''' sequenza (scheduler, ThreadPool, GC) che finisce per intaccare
    ''' anche il thread UI.
    ''' </summary>
    Private ReadOnly tentativoConnessioneRXInCorso As Boolean() = New Boolean(6) {}

    ''' <summary>
    ''' Avvia una ConnectRX senza bloccare il chiamante: SocketManager.ConnectRX
    ''' fa un ConnectAsync(...).Wait(2000), cioè blocca per fino a 2 secondi
    ''' il thread che la chiama. MonComponent/Phasing/InvioPhase sono
    ''' richiamate da timer (TmrPhase/tmrPhasing) direttamente sul thread UI:
    ''' quando il sotto-socket non è mai connesso (es. in assenza di
    ''' hardware reale), il tentativo si ripete ad ogni tick e blocca
    ''' ripetutamente l'interfaccia per ~2s, con effetti a cascata su tutto
    ''' ciò che aspetta il thread UI (incluso il server HTTP integrato,
    ''' HttpServer\AntennaHttpServer.vb, osservato con ritardi fino a oltre
    ''' un minuto). Qui il tentativo di connessione parte su un thread
    ''' separato e il chiamante prosegue subito: se non è ancora connesso,
    ''' semplicemente non invia questa volta, riprova al tick successivo.
    ''' Un solo tentativo per canale alla volta (tentativoConnessioneRXInCorso):
    ''' se uno è già in volo, questa chiamata non ne lancia un secondo.
    ''' </summary>
    Private Sub ConnettiRXSenzaBloccare(ind As Integer)
        If tentativoConnessioneRXInCorso(ind) Then Return
        tentativoConnessioneRXInCorso(ind) = True
        Dim host As String = LanRx(ind).IPAdress
        Dim port As Integer = CInt(LanRx(ind).LanPort)
        Dim t As New Threading.Thread(Sub()
                                           Try
                                               SocketManager.ConnectRX(ind, host, port)
                                           Finally
                                               tentativoConnessioneRXInCorso(ind) = False
                                           End Try
                                       End Sub)
        t.IsBackground = True
        t.Start()
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas MonComponent: richiede al DTR (canale LanMon)
    ''' lo stato del componente indicato (Monitor / IF1). Era uno stub vuoto:
    ''' TmrPhase (frmMain) la chiamava regolarmente senza interrogare nulla,
    ''' quindi i campi di monitor del ricevitore restavano sempre invariati.
    ''' </summary>
    Public Sub MonComponent(code As Integer)
        Try
            If Not SocketManager.IsRXConnected(LanMon) Then
                ConnettiRXSenzaBloccare(LanMon)
            End If
            Dim tRic As Boolean = FrmRicevitore.tmrPhasing.Enabled
            Dim rxBuffer As String = IntToString(rx_Header)
            rxBuffer &= IntToString(LenMesMon)
            rxBuffer &= IntToString(0)
            rxBuffer &= IntToString(code)
            rxBuffer &= IntToString(rx_Postamble)
            LanPortRx = RxMon
            SocketManager.InvioRX(LanMon, rxBuffer)
            If tRic Then FrmRicevitore.tmrPhasing.Enabled = True
        Catch ex As Exception
            WriteError(0, "MonComponent: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas Phasing: richiede al DTR (canale LanPhase)
    ''' l'emissione ciclica della fase. Era uno stub vuoto: TmrPhase
    ''' (frmMain) la chiamava regolarmente senza inviare nulla, quindi il
    ''' DTR non emetteva mai la fase.
    ''' </summary>
    Public Sub Phasing()
        Try
            Dim tRic As Boolean = FrmRicevitore.tmrPhasing.Enabled
            If Not SocketManager.IsRXConnected(LanPhase) Then
                ConnettiRXSenzaBloccare(LanPhase)
            End If
            Dim rxBuffer As String = IntToString(rx_Header)
            rxBuffer &= IntToString(LenMesPhasing)
            rxBuffer &= IntToString(0)
            rxBuffer &= IntToString(cmdPhasing)
            rxBuffer &= DecimalToFloat(i_InterRic)
            Dim zero As String = IntToString(0)
            rxBuffer &= zero & zero & zero & zero & zero
            rxBuffer &= IntToString(rx_Postamble)
            RequestDTR = 1
            LanPortRx = RxPhase
            If SocketManager.IsRXConnected(LanPhase) Then SocketManager.InvioRX(LanPhase, rxBuffer)
            If tRic Then FrmRicevitore.tmrPhasing.Enabled = True
        Catch ex As Exception
            WriteError(0, "Phasing: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting diretto da VB6 (Ricevitore.bas): connette o mette in ascolto
    ''' il singolo sotto-socket RX Ind (1..6: Mon/Ctrl/Rst/Log/Phase/Cal)
    ''' tramite SocketManager. Era uno stub vuoto: per questo i sotto-socket
    ''' RX — inclusi i due connessi all'avvio da IniPort_Control
    ''' (LanReset1(LanMon)/LanReset1(LanPhase)) — non venivano mai aperti
    ''' davvero, a prescindere dalla configurazione.
    ''' </summary>
    ''' <summary>
    ''' Tenta la connessione TCP (client) per un sotto-canale LanRx, senza
    ''' toccare alcun controllo della UI: estratta da LanReset1 per poterla
    ''' lanciare in Task.Run in parallelo (vedi IniPort_Control, insieme a
    ''' TentaConnessioneLan per ACU/MAC/GRF) — SetLabelColor in LanReset1
    ''' non è thread-safe, qui resta fuori apposta.
    ''' </summary>
    Public Function TentaConnessioneLanRX(Ind As Integer) As Boolean
        Return SocketManager.ConnectRX(Ind, LanRx(Ind).IPAdress, CInt(LanRx(Ind).LanPort))
    End Function

    ''' <summary>
    ''' connessoPrecalcolato: se valorizzato (da IniPort_Control, che lancia
    ''' TentaConnessioneLanRX in parallelo insieme ai canali ACU/MAC/GRF
    ''' prima di chiamare LanReset1), salta un secondo tentativo di
    ''' connessione e usa direttamente l'esito già noto. Nothing (default)
    ''' per tutte le altre chiamate — comportamento identico a prima,
    ''' connette qui.
    ''' </summary>
    Public Sub LanReset1(Ind As Integer, Optional connessoPrecalcolato As Boolean? = Nothing)
        Try
            If LanRx(Ind).Listen = "S" Then
                Dim connesso As Boolean = If(connessoPrecalcolato.HasValue, connessoPrecalcolato.Value, TentaConnessioneLanRX(Ind))
                If connesso Then
                    LanRx(Ind).Port_State = PCS_NORMAL
                Else
                    LanRx(Ind).Port_State = PCS_FAIL
                    FrmMain.frmRicezioneInstance.SetLabelColor("LblConfCNL3in", PCS_STOP_COL)
                End If
            Else
                SocketManager.ListenRX(Ind, CInt(LanRx(Ind).LanPort))
            End If
            LanRx(Ind).Buffer = ""
            LanRx(Ind).Rec_State = PCR_WAIT
        Catch ex As Exception
            WriteError(0, "LanReset1 " & Ind & ": " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas InvioPhase: invia al DTR (canale LanCtrl) un
    ''' comando di impostazione fase/guadagno per il componente indicato
    ''' (mode 0 = intero, mode 1 = float IEEE-754). Era uno stub vuoto: i
    ''' comandi di calibrazione guadagno/fase (TxtAzGain1/TxtElGain1 e le
    ''' manopole fase in frmRicevitore) non avevano alcun effetto.
    ''' </summary>
    Public Sub InvioPhase(data As Object, mode As Integer, command As Integer, componentCode As Integer)
        Try
            If Not SocketManager.IsRXConnected(LanCtrl) Then
                ConnettiRXSenzaBloccare(LanCtrl)
            End If
            If Not SocketManager.IsRXConnected(LanCtrl) Then Exit Sub
            Dim tRic As Boolean = FrmRicevitore.tmrPhasing.Enabled
            Dim rxBuffer As String = IntToString(rx_Header)
            rxBuffer &= IntToString(32)
            rxBuffer &= IntToString(1)
            rxBuffer &= IntToString(componentCode)
            rxBuffer &= IntToString(1)
            rxBuffer &= IntToString(command)
            Dim valore As Double
            If TypeOf data Is String Then
                valore = Val(CStr(data))       'Val riconosce sempre "." come separatore decimale, a prescindere dal locale
            Else
                valore = Convert.ToDouble(data)
            End If
            If mode = 0 Then
                rxBuffer &= IntToString(CInt(valore))
            Else
                rxBuffer &= DecimalToFloat(CSng(valore))
            End If
            rxBuffer &= IntToString(rx_Postamble)
            RequestDTR = 0
            LanPortRx = RxCtrl
            Dim prevIRic As Integer = i_ric
            i_ric = 3
            SocketManager.InvioRX(LanCtrl, rxBuffer)
            i_ric = prevIRic
            If tRic Then FrmRicevitore.tmrPhasing.Enabled = True
        Catch ex As Exception
            WriteError(0, "InvioPhase: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas InvioPacket: invia al DTR (canale LanCtrl)
    ''' un pacchetto con fase e frequenza (la polarizzazione era già esclusa
    ''' nell'originale, righe commentate - qui riportata identica).
    ''' NON RICHIAMATA DA NESSUNA PARTE, esattamente come nel VB6 originale
    ''' (zero chiamanti in tutto il programma sorgente): portata su richiesta
    ''' esplicita, pronta per essere attivata il giorno in cui serva un invio
    ''' combinato fase+frequenza in un unico pacchetto invece dei canali
    ''' separati già in uso operativo (fase: InvioPhase/LanCtrl; frequenza:
    ''' InvioRic/GRF; polarizzazione: InvioRic/ACU).
    ''' </summary>
    Public Sub InvioPacket(Phase As String, Frequency As String, Polar As String)
        Try
            Dim lun As Integer = 48
            Dim par As Integer = &H3
            If Not IsNumeric(Phase) Then
                lun -= 8
                par -= 1
            End If
            If Not IsNumeric(Frequency) Then
                lun -= 8
                par -= 1
            End If
            ' Polar escluso dal pacchetto: così anche nell'originale (righe
            ' commentate), mai attivato - parametro tenuto per compatibilità
            ' di firma col chiamante futuro.
            If par = 0 Then Exit Sub ' nessuna informazione utile da inviare
            Dim rxBuffer As String = IntToString(rx_Header)
            rxBuffer &= IntToString(lun)
            rxBuffer &= IntToString(1) ' codice comando
            rxBuffer &= IntToString(ComponentCode)
            rxBuffer &= IntToString(par)
            If IsNumeric(Phase) Then
                rxBuffer &= IntToString(5)
                rxBuffer &= IntToString(CInt(Val(Phase)))
            End If
            If IsNumeric(Frequency) Then
                rxBuffer &= IntToString(10)
                rxBuffer &= IntToString(CInt(Val(Frequency)))
            End If
            rxBuffer &= IntToString(rx_Postamble)
            RequestDTR = 0
            LanPortRx = RxCtrl
            SocketManager.InvioRX(LanCtrl, rxBuffer)
        Catch ex As Exception
            WriteError(0, "InvioPacket: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas CalibrationTableRequest: richiede al DTR
    ''' (canale LanCal) la tabella di calibrazione corrente (CalibrationType
    ''' = 0 o 1, seleziona quale). NON RICHIAMATA DA NESSUNA PARTE: nel VB6
    ''' originale l'unica chiamata (in Form_Activate di frmRicezione) era
    ''' commentata, quindi mai eseguita nemmeno in produzione. Portata su
    ''' richiesta esplicita, pronta per essere attivata (es. da un bottone o
    ''' voce di menu dedicati) il giorno in cui serva operativamente - va
    ''' anche decisa/implementata la destinazione della tabella ricevuta in
    ''' risposta, oggi non prevista in nessuna schermata.
    ''' </summary>
    Public CalibrationType As Integer

    Public Sub CalibrationTableRequest()
        Try
            Dim rxBuffer As String = IntToString(rx_Header)
            rxBuffer &= IntToString(LenMesCalTable)
            rxBuffer &= IntToString(0) ' codice comando
            rxBuffer &= IntToString(CalibrationType)
            Dim zero As String = IntToString(0)
            rxBuffer &= zero & zero & zero & zero & zero & zero & zero & zero & zero
            rxBuffer &= IntToString(rx_Postamble)
            RequestDTR = If(CalibrationType = 0, 3, 4)
            LanPortRx = RxCal
            SocketManager.InvioRX(LanCal, rxBuffer)
        Catch ex As Exception
            WriteError(0, "CalibrationTableRequest: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Porting di Ricevitore.bas IntToString: codifica un intero a 32 bit nei
    ''' 4 byte "grezzi" big-endian del protocollo binario RX, incapsulati in
    ''' una String di 4 caratteri (un byte 0-255 per carattere) — esattamente
    ''' come Winsock SendData/GetData trattava le stringhe in VB6.
    ''' </summary>
    Public Function IntToString(sDec As Integer) As String
        Dim b() As Byte = BitConverter.GetBytes(sDec)
        If BitConverter.IsLittleEndian Then Array.Reverse(b)
        Return ChrW(b(0)) & ChrW(b(1)) & ChrW(b(2)) & ChrW(b(3))
    End Function

    ''' <summary>
    ''' Porting di Ricevitore.bas StringToInt: decodifica i 4 byte "grezzi"
    ''' big-endian del protocollo binario RX in un intero a 32 bit.
    ''' La versione precedente (Val(s)) interpretava il testo invece dei
    ''' byte: su qualunque risposta reale del ricevitore restituiva sempre 0,
    ''' quindi l'intero parsing in frmRicevitore.LblInputRx_Change era inerte.
    ''' </summary>
    Public Function StringToInt(s As String) As Integer
        If Len(s) < 4 Then Return 0
        Dim b(3) As Byte
        For i As Integer = 0 To 3
            b(i) = CByte(AscW(s(i)) And &HFF)
        Next
        If BitConverter.IsLittleEndian Then Array.Reverse(b)
        Return BitConverter.ToInt32(b, 0)
    End Function

    ''' <summary>
    ''' Porting di Ricevitore.bas DecimalToFloat: codifica un Single nei 4
    ''' byte IEEE-754 "grezzi" big-endian del protocollo binario RX.
    ''' </summary>
    Public Function DecimalToFloat(sDec As Single) As String
        Dim b() As Byte = BitConverter.GetBytes(sDec)
        If BitConverter.IsLittleEndian Then Array.Reverse(b)
        Return ChrW(b(0)) & ChrW(b(1)) & ChrW(b(2)) & ChrW(b(3))
    End Function

    ''' <summary>
    ''' Porting di Ricevitore.bas FloatToDecimal: decodifica i 4 byte
    ''' IEEE-754 "grezzi" big-endian del protocollo binario RX in un Single.
    ''' La versione precedente (Val(s)) interpretava il testo invece dei
    ''' byte: su qualunque risposta reale restituiva sempre 0 (livello
    ''' beacon, errori Az/El, polarizzazione, guadagni, frequenza IF...).
    ''' </summary>
    Public Function FloatToDecimal(s As String) As Double
        If Len(s) < 4 Then Return 0
        Dim b(3) As Byte
        For i As Integer = 0 To 3
            b(i) = CByte(AscW(s(i)) And &HFF)
        Next
        If BitConverter.IsLittleEndian Then Array.Reverse(b)
        Return BitConverter.ToSingle(b, 0)
    End Function

    ' Crea la directory del mese e i file di log giornalieri.
    ' Chiamato sia all'avvio (Inizializza) sia alla rotazione giornaliera (TmrOrologio).
    ' Usa Try/Catch esplicito invece di "On Error Resume Next" per non mascherare errori.
    Public Sub CreaFilesLog(MyMonth As String, DateRep As String)
        Try
            Dim monthDir As String = System.IO.Path.Combine(Application.StartupPath, MyMonth)
            If Not System.IO.Directory.Exists(monthDir) Then
                System.IO.Directory.CreateDirectory(monthDir)
            End If

            f_Log = MyMonth & "\" & MyMonth & DateRep & ".Txt"
            f_Log1 = MyMonth & "\" & MyMonth & "1" & DateRep & ".Txt"
            f_Log2 = MyMonth & "\" & MyMonth & "2" & DateRep & ".Txt"
            f_Log3 = MyMonth & "\" & MyMonth & "3" & DateRep & ".Txt"
            f_Log4 = "ErrSoft.Txt"

            ' Crea i file se non esistono (Append = non distrugge dati esistenti)
            For Each rel In {f_Log, f_Log1, f_Log2, f_Log3, f_Log4}
                Dim fullPath As String = System.IO.Path.Combine(Application.StartupPath, rel)
                If Not System.IO.File.Exists(fullPath) Then
                    System.IO.File.WriteAllText(fullPath, "")
                End If
            Next
        Catch ex As Exception
            ' Errore visibile in debug — non silenzioso come "On Error Resume Next"
            System.Diagnostics.Debug.WriteLine("CreaFilesLog: " & ex.Message)
        End Try
    End Sub

End Module

