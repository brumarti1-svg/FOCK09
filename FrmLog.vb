' ============================================================
' FrmLog.vb — convertito da Prog0347-FOCC10\frmLog.frm (VB6)
' Telespazio FOC-C10 — DYNATRON
' Base: il porting .NET di FOC6. Differenze di C10 in
' AggiornaDisplay: niente spie canali 7-8, Lock/Pol/LNA letti dai
' byte di stato ACU invece che da frmRicezione/LockRic, allarmi
' prelimit polarizzatore (17/18) e LblPolFault attivi, formato
' frequenze "###00.000". Le note sotto sono quelle originali di
' FOC6 (dove dicono "disattivati" per Pol/LNA, in C10 non vale).
' ============================================================
' Pannello di telemetria estesa: NON è un semplice log (nome
' legacy "Service"/frmLog), ma una seconda schermata che mostra
' sia un mirror dei dati già su frmRicezione sia sensori che non
' trovavano posto lì per mancanza di spazio (LNA, Polarizzazione,
' Major/Minor Fault Az/El, Stow/Aligned, Drive Status, Switches,
' Back-up status, Frequenza/Potenza RF) — più la tabella allarmi
' (Datlog/tdbLog nel VB6, stesso schema già usato da frmRicezione).
' In BTS1 non serve: lì tutto entra in un'unica maschera.
'
' AggiornaDisplay() è portata quasi riga per riga dal VB6
' originale (Function AggiornaDisplay, non un Sub — da qui
' l'equivoco iniziale sul nome). I rami già commentati/disattivati
' nel VB6 (LblPolFault, TxtPolErr, parte della logica Pol/LNA via
' vBytesold(10)) restano disattivati qui per lo stesso motivo.
'
' I bottoni comando (CmdSTBY/CmdStepTrack/ecc.) duplicano
' esattamente la stessa logica già presente su frmRicezione: invece
' di riscriverla (rischio di divergenza), delegano alle Sub
' pubbliche già esistenti lì.
' ============================================================

Partial Public Class FrmLog
    Inherits System.Windows.Forms.Form

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub FrmLog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Campi di sola presentazione: giallo se sconosciuti, 128,255,255 se noti
        ColoriStandard.RegistraDati(TxtMode, LblCommand, LblSatellite, TxtAz, LblAzimuth, LblAzError,
            LblAzSpeed, LblAzTBrk, TxtEl, LblElevazione, LblElError, LblElSpeed,
            LblElTBrk, LblAzStow, LblAzAligned, LblElStow, LblElAligned, LblElHand,
            TxtPol, LblPol, LblPol1, LblLNA, TxtPolErr, TxtLev,
            TxtAzEr, TxtElEr, LblLev, LblWarning, LblStepTrack, TxtFreqBeacon,
            LblBackUp, LblSwitches, TxtFrequency, TxtPowerOut)
        ' Soglie Auto Track e offset massimo (Form_Load del VB6).
        TxtSogliaAuto.Text = CStr(g_ThrAuto)
        TxtThrAz.Text = CStr(g_ThrAz)
        TxtThrEl.Text = CStr(g_ThrEl)
        TxtOffsetMax.Text = CStr(Max_Offset)
        AggiornaGriglia()
    End Sub

    Private Sub FrmLog_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        FormResizer.Init(Me, New Size(1339, 788))
        FormResizer.Resize(Me)
    End Sub

    Private Sub FrmLog_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        FormResizer.Resize(Me)
    End Sub

    ' -------------------------------------------------------
    ' Griglia allarmi — stesso schema di frmRicezione.AggiornaGriglia
    ' (il VB6 originale legge "error_messages", ma quella tabella non
    ' viene più scritta da nessuno: WriteError/ScriviLOG già portati
    ' scrivono su "tmp_error_messages", stesso nome usato da BTS1).
    ' -------------------------------------------------------
    Private Sub AggiornaGriglia()
        Try
            Dim dt As New System.Data.DataTable()
            Dim conn As System.Data.SQLite.SQLiteConnection = CType(dbOrbite, DAODatabase).Connection
            Using Cmd As New System.Data.SQLite.SQLiteCommand(
                "SELECT MSG_date, Error FROM tmp_error_messages WHERE Level = 1 ORDER BY MSG_date DESC",
                conn)
                Using dr = Cmd.ExecuteReader()
                    dt.Columns.Add("Data")
                    dt.Columns.Add("Messaggio")
                    Do While dr.Read()
                        dt.Rows.Add(CStr(dr(0)), CStr(dr(1)))
                    Loop
                End Using
            End Using
            TdbLog.DataSource = dt
            If TdbLog.Columns.Contains("Data") Then TdbLog.Columns("Data").Width = 180
            If TdbLog.Columns.Contains("Messaggio") Then TdbLog.Columns("Messaggio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Catch
        End Try
    End Sub

    Private Sub CmdReset_Click(sender As Object, e As EventArgs) Handles CmdReset.Click
        Try
            DbExecute("Delete * from [tmp_error_messages]")
        Catch
        End Try
        AggiornaGriglia()
    End Sub

    ' -------------------------------------------------------
    ' Comandi — duplicati di frmRicezione, delegano alle Sub
    ' pubbliche già esistenti lì per non duplicare la logica di
    ' invio protocollo (CheckSum/InvioACU, guardie ACU Local/Alarm).
    ' -------------------------------------------------------
    Private Sub CmdSTBY_Click(sender As Object, e As EventArgs) Handles CmdSTBY.Click
        FrmRicezione.CmdSTBY_Click(sender, e)
    End Sub

    Private Sub CmdStepTrack_Click(sender As Object, e As EventArgs) Handles CmdStepTrack.Click
        FrmRicezione.CmdStepTrack_Click(sender, e)
    End Sub

    Private Sub CmdAutoTrack_Click(sender As Object, e As EventArgs) Handles CmdAutoTrack.Click
        FrmRicezione.CmdAutoTrack_Click(sender, e)
    End Sub

    Private Sub CmdAutoSearch_Click(sender As Object, e As EventArgs) Handles CmdAutoSearch.Click
        FrmRicezione.CmdAutoSearch_Click(sender, e)
    End Sub

    Private Sub CmdAugmented_Click(sender As Object, e As EventArgs) Handles CmdAugmented.Click
        FrmRicezione.CmdAugmented_Click(sender, e)
    End Sub

    Private Sub CmdProgramTrack_Click(sender As Object, e As EventArgs) Handles CmdProgramTrack.Click
        FrmRicezione.CmdProgramTrack_Click(sender, e)
    End Sub

    Private Sub CmdCmdPos_Click(sender As Object, e As EventArgs) Handles CmdCmdPos.Click
        FrmRicezione.CmdCmdPos_Click(sender, e)
    End Sub

    Private Sub CmdOffset_Click(sender As Object, e As EventArgs) Handles CmdOffset.Click
        FrmRicezione.CmdOffset_Click(sender, e)
    End Sub

    ' -------------------------------------------------------
    ' Menu — clone di mnuMain/mnuFile/mnuDacu1/mnuComandi/mnuPreset/
    ' mnuInter (frmLog.frm reale). Dove il comando esiste già in
    ' frmRicezione (stessa logica di invio protocollo/guardie ACU
    ' Local/Alarm), si richiama da lì per non duplicarla; le voci
    ' prive di handler qui sono prive di handler anche nel menu
    ' già portato di frmRicezione (nessuna regressione).
    ' -------------------------------------------------------
    Private Sub MnuConfiguration_Click(sender As Object, e As EventArgs) Handles MnuConfiguration.Click
        FrmConfigurazione.Show()
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As EventArgs) Handles MnuExit.Click
        FrmParametri.Close()
        FrmPhase.Close()
        Me.Close()
    End Sub

    Private Sub MnuStandBy_Click(sender As Object, e As EventArgs) Handles MnuStandBy.Click
        FrmRicezione.CmdSTBY_Click(sender, e)
    End Sub

    Private Sub MnuStepTrack_Click(sender As Object, e As EventArgs) Handles MnuStepTrack.Click
        FrmRicezione.CmdStepTrack_Click(sender, e)
    End Sub

    Private Sub MnuMemoryTrack_Click(sender As Object, e As EventArgs) Handles MnuMemoryTrack.Click
        FrmRicezione.MnuComMemoryTrack_Click(sender, e)
    End Sub

    Private Sub MnuSatelliteCmd_Click(sender As Object, e As EventArgs) Handles MnuSatelliteCmd.Click
        FrmRicezione.MnuComSatellite_Click(sender, e)
    End Sub

    Private Sub MnuStow_Click(sender As Object, e As EventArgs) Handles MnuStow.Click
        FrmRicezione.MnuComStow_Click(sender, e)
    End Sub

    Private Sub MnuUnstow_Click(sender As Object, e As EventArgs) Handles MnuUnstow.Click
        FrmRicezione.MnuComUnstow_Click(sender, e)
    End Sub

    Private Sub MnuPresetAll_Click(sender As Object, e As EventArgs) Handles MnuPresetAll.Click
        i_Assi = 0
        FrmPreset.Show()
    End Sub

    Private Sub MnuPresetAz_Click(sender As Object, e As EventArgs) Handles MnuPresetAz.Click
        i_Assi = 1
        FrmPreset.Show()
    End Sub

    Private Sub MnuPresetEl_Click(sender As Object, e As EventArgs) Handles MnuPresetEl.Click
        i_Assi = 2
        FrmPreset.Show()
    End Sub

    Private Sub MnuSatelliteData_Click(sender As Object, e As EventArgs) Handles MnuSatelliteData.Click
        FrmTabSat.Show()
    End Sub

    Private Sub MnuMemoryTrackData_Click(sender As Object, e As EventArgs) Handles MnuMemoryTrackData.Click
        FrmMemoryTrack.Show()
    End Sub

    Private Sub MnuAntenna_Click(sender As Object, e As EventArgs) Handles MnuAntenna.Click
        FrmAntenne.Show()
    End Sub

    Private Sub MnuSatelliteMenu_Click(sender As Object, e As EventArgs) Handles MnuSatelliteMenu.Click
        FrmSatelliti.Show()
    End Sub

    Private Sub MnuEphemeris_Click(sender As Object, e As EventArgs) Handles MnuEphemeris.Click
        ' Effemeridi del satellite operativo (come Ephemeris > Satellite di frmRicezione)
        If Algoritmo = 3 Then
            FrmTrackTable.Show()
        Else
            i_Ephemeris = 2
            FrmEffemeridi.Show()
        End If
    End Sub

    Private Sub MnuParameterACU_Click(sender As Object, e As EventArgs) Handles MnuParameterACU.Click
        FrmParametri.Show()
    End Sub

    Private Sub MnuPhase_Click(sender As Object, e As EventArgs) Handles MnuPhase.Click
        b_Scelta = True
        FrmPhase.Show()
    End Sub

    Private Sub MnuInfo_Click(sender As Object, e As EventArgs) Handles MnuInfo.Click
        FrmSplash.Show()
    End Sub

    ' -------------------------------------------------------
    ' Timer di refresh (era Interval=1000 nel VB6)
    ' -------------------------------------------------------
    Private Sub TmrAggiorna_Tick(sender As Object, e As EventArgs) Handles TmrAggiorna.Tick
        AggiornaDisplay()
    End Sub

    Private Sub TmrAlarm_Tick(sender As Object, e As EventArgs) Handles TmrAlarm.Tick
        LblWarning.Text = ""
        LblWarning.Visible = False
        TmrAlarm.Enabled = False
    End Sub

    ' -------------------------------------------------------
    ' Porting diretto di "Function AggiornaDisplay() As Boolean"
    ' (VB6, Prog0357-FOC6\frmLog.frm) — stessi bit vBytesold già
    ' letti/aggiornati dal ciclo dati di frmRicezione (nessun nuovo
    ' parsing del protocollo ACU qui, solo lettura dello stato
    ' già pronto).
    ' -------------------------------------------------------
    Private Sub AggiornaDisplay()
        Try
            LblConfCNL1in.BackColor = FrmRicezione.LblConfCNL1in.BackColor
            LblConfCNL2in.BackColor = FrmRicezione.LblConfCNL2in.BackColor
            LblConfCNL3in.BackColor = FrmRicezione.LblConfCNL3in.BackColor
            LblConfCNL4in.BackColor = FrmRicezione.LblConfCNL4in.BackColor
            LblConfCNL5in.BackColor = FrmRicezione.LblConfCNL5in.BackColor
            LblConfCNL6in.BackColor = FrmRicezione.LblConfCNL6in.BackColor
            LblAzimuth.Text = FrmRicezione.LblAzimuth.Text
            LblElevazione.Text = FrmRicezione.LblElevazione.Text
            LblAzError.Text = FrmRicezione.LblAzError.Text
            LblAzError.ForeColor = FrmRicezione.LblAzError.ForeColor
            LblElError.Text = FrmRicezione.LblElError.Text
            LblElError.ForeColor = FrmRicezione.LblElError.ForeColor
            LblSatellite.Text = FrmRicezione.LblSatellite.Text

            ' C10: Lock, Polarizzazione e LNA non sono copiati da
            ' frmRicezione/LockRic come in FOC6, ma letti più sotto dai
            ' byte di stato ACU (vBytesold(9), (10)) e da s_POL.

            If CampoOp = "*" Then
                Select Case (s_Mod And &HF)
                    Case 0 : TxtMode.Text = "Stand By"
                    Case 1 : TxtMode.Text = "Preset"
                    Case 2 : TxtMode.Text = "Manual Slew"
                    Case 3 : TxtMode.Text = "Manual Velocity"
                    Case 4 : TxtMode.Text = "Monopulse"
                    Case 5 : TxtMode.Text = "Step Track"
                    Case 6 : TxtMode.Text = "Memory Track"
                    Case 7 : TxtMode.Text = "Automatic Acquire"
                    Case 8 : TxtMode.Text = "Satellite"
                    Case 9 : TxtMode.Text = "Wait"
                    Case 10 : TxtMode.Text = ""
                    Case 11 : TxtMode.Text = "Automatic Stow"
                    Case 12 : TxtMode.Text = "Automatic Un-Stow"
                    Case 13 : TxtMode.Text = "Pin In"
                    Case 14 : TxtMode.Text = "Pin Out"
                    Case 15 : TxtMode.Text = "Step Preset"
                End Select

                If (vBytesold(1) And BIT6) = BIT6 Then
                    TxtACU.BackColor = Color.Lime
                    TxtACU.Text = "REM"
                Else
                    TxtACU.BackColor = Color.Red
                    TxtACU.Text = "LOC"
                End If

                LblSumAlarm.BackColor = If((vBytesold(1) And BIT5) = BIT5, Color.Red, Color.Lime)

                LblAllarme1.BackColor = If((vBytesold(3) And BIT1) = BIT1, Color.Lime, Color.Red)
                LblAllarme2.BackColor = If((vBytesold(3) And BIT3) = BIT3, Color.Lime, Color.Red)
                LblAllarme3.BackColor = If((vBytesold(2) And BIT3) = BIT3, Color.Red, Color.Lime)
                LblAllarme4.BackColor = If((vBytesold(2) And BIT4) = BIT4, Color.Red, Color.Lime)
                LblAllarme5.BackColor = If((vBytesold(3) And BIT2) = BIT2, Color.Lime, Color.Red)
                LblAllarme6.BackColor = If((vBytesold(3) And BIT4) = BIT4, Color.Lime, Color.Red)
                LblAllarme7.BackColor = If((vBytesold(2) And BIT1) = BIT1, Color.Red, Color.Lime)
                LblAllarme8.BackColor = If((vBytesold(2) And BIT2) = BIT2, Color.Red, Color.Lime)
                ' Indice 9 (HIGH ALARM) disattivato anche nel VB6 originale (riga commentata).
                LblAllarme10.BackColor = If((vBytesold(12) And BIT3) = BIT3, Color.Lime, Color.Red)
                LblAllarme11.BackColor = If((vBytesold(12) And BIT1) = BIT1, Color.Lime, Color.Red)
                LblAllarme12.BackColor = If((vBytesold(11) And BIT1) = BIT1, Color.Red, Color.Lime)
                LblAllarme13.BackColor = If((vBytesold(11) And BIT2) = BIT2, Color.Red, Color.Lime)
                LblAllarme14.BackColor = If((vBytesold(11) And BIT4) = BIT4, Color.Red, Color.Lime)
                LblAllarme15.BackColor = If((vBytesold(11) And BIT3) = BIT3, Color.Red, Color.Lime)
                LblAllarme16.BackColor = If((vBytesold(8) And BIT3) = BIT3, Color.Lime, Color.Red)
                ' C10: indici 17/18 (CCW/CW Pol software prelimit) attivi
                ' (in FOC6 erano commentati nel VB6).
                LblAllarme17.BackColor = If((vBytesold(2) And BIT6) = BIT6, Color.Red, Color.Lime)
                LblAllarme18.BackColor = If((vBytesold(2) And BIT5) = BIT5, Color.Red, Color.Lime)

                LblAzMaFault.BackColor = If((vBytesold(4) And BIT1) = BIT1, Color.Lime, Color.Red)
                LblAzMiFault.BackColor = If((vBytesold(4) And BIT3) = BIT3, Color.Lime, Color.Red)
                LblElMaFault.BackColor = If((vBytesold(4) And BIT2) = BIT2, Color.Lime, Color.Red)
                LblElMiFault.BackColor = If((vBytesold(4) And BIT4) = BIT4, Color.Lime, Color.Red)

                LblAzDR.Text = If((vBytesold(5) And BIT1) = BIT1, "ON ", "OFF ")
                LblAzDR.BackColor = If((vBytesold(5) And BIT1) = BIT1, Color.Lime, Color.Red)
                LblPCU.Text = If((vBytesold(5) And BIT2) = BIT2, "PCU", "ACU")
                LblPCU.BackColor = If((vBytesold(5) And BIT2) = BIT2, Color.Red, Color.Lime)

                LblElTBrk.Text = If((vBytesold(5) And BIT3) = BIT3, "RELEASED", "APPLIED ")
                LblAzTBrk.Text = If((vBytesold(5) And BIT4) = BIT4, "RELEASED", "APPLIED ")

                LblEmergency.BackColor = If((vBytesold(6) And BIT3) = BIT3, Color.Lime, Color.Red)
                LblEmergency1.BackColor = If((vBytesold(6) And BIT4) = BIT4, Color.Lime, Color.Red)

                LblAzStow.Text = ""
                LblAzAligned.Text = ""
                LblElStow.Text = ""
                LblElAligned.Text = ""
                If (vBytesold(7) And BIT1) = BIT1 Then LblAzStow.Text = "Extended"
                If (vBytesold(7) And BIT2) = BIT2 Then LblAzStow.Text = "Retracted"
                If (vBytesold(7) And BIT3) = BIT3 Then LblAzAligned.Text = "Aligned"
                If (vBytesold(7) And BIT4) = BIT4 Then LblElStow.Text = "Extended"
                If (vBytesold(8) And BIT1) = BIT1 Then LblElStow.Text = "Retracted"
                If (vBytesold(8) And BIT2) = BIT2 Then LblElAligned.Text = "Aligned"
                ' C10: fault polarizzatore attivo (commentato nel VB6 di FOC6).
                LblPolFault.BackColor = If((vBytesold(8) And BIT4) = BIT4, Color.Red, Color.Lime)

                LblElHand.Text = If((vBytesold(9) And BIT2) = BIT2, "CW", "CCW")
                LblLowAngle.BackColor = If((vBytesold(9) And BIT3) = BIT3, Color.Red, Color.Lime)

                LblLock.BackColor = If((vBytesold(9) And BIT4) = BIT4, Color.Lime, Color.Red)
                LblLock.Text = If((vBytesold(9) And BIT4) = BIT4, "LOCK", "UNLOCK")

                If (vBytesold(15) And BIT6) = BIT6 Then
                    LblStepTrack.Visible = True
                    Dim iStepTrack As Integer = (vBytesold(15) And &H1F)
                    Select Case iStepTrack
                        Case 0 : LblStepTrack.Text = "---"
                        Case 1 : LblStepTrack.Text = "MM"
                        Case 2 : LblStepTrack.Text = "BRK"
                        Case 3 : LblStepTrack.Text = "EL"
                        Case 4 : LblStepTrack.Text = "MIS"
                        Case 5 : LblStepTrack.Text = "MIS"
                        Case 6 : LblStepTrack.Text = "EL"
                        Case 7 : LblStepTrack.Text = "BRK"
                        Case 8 : LblStepTrack.Text = "AZ"
                        Case 9 : LblStepTrack.Text = "MIS"
                        Case 10 : LblStepTrack.Text = "MIS"
                        Case 11 : LblStepTrack.Text = "AZ"
                        Case 12 : LblStepTrack.Text = "MM"
                        Case Else : LblStepTrack.Text = ""
                    End Select
                Else
                    LblStepTrack.Visible = False
                End If

                If LblLock.Text = "LOCK" Then
                    If IsNumeric(s_Lev) Then LblLev.Text = Format$(Val(s_Lev), "##0.00")
                Else
                    LblLev.Text = "-150.00"
                End If
                If IsNumeric(s_Lev) Then TxtLev.Text = Format$(Val(s_Lev), "##0.00")
                ' Posizione Az/El e angolo di polarizzazione dalle stringhe
                ' ACU (millesimi / decimi di grado). Presenti nel VB6 sia di
                ' C10 sia di FOC6, non portate in FOC6 .NET.
                If IsNumeric(Mid$(s_Az, 1, 6)) Then TxtAz.Text = Format$(Val(Mid$(s_Az, 1, 6)) / 1000, "000.000")
                If IsNumeric(Mid$(s_El, 1, 5)) Then TxtEl.Text = Format$(Val(Mid$(s_El, 1, 5)) / 1000, "00.000")
                If IsNumeric(Mid$(s_POL, 1, 4)) Then TxtPol.Text = Format$(Val(Mid$(s_POL, 1, 4)) / 10, "00.0")
                TxtAzEr.Text = s_LevAz
                TxtElEr.Text = s_LevEl
                LblAzSpeed.Text = s_AzTS
                LblElSpeed.Text = s_ElTS

                ' LNA in linea dal byte 10
                If (vBytesold(10) And BIT1) = BIT1 Then
                    LblLNA.Text = "1"
                ElseIf (vBytesold(10) And BIT2) = BIT2 Then
                    LblLNA.Text = "2"
                ElseIf (vBytesold(10) And BIT3) = BIT3 Then
                    LblLNA.Text = "3"
                End If
                ' K09, come frmRicezione (fa testo): polarizzazione dell'errore dal
                ' BIT4 del byte 10; senso dal BIT3 del byte 9 (V/H in lineare,
                ' RHCP/LHCP in circolare). Il "Running" in Preset polarizzazione del
                ' frmLog VB6 non c'è nel frmRicezione VB6 e non viene portato.
                TxtPolErr.Text = If((vBytesold(10) And BIT4) = BIT4, "RHCP", "LHCP")
                Dim senso As Boolean = (vBytesold(9) And BIT3) = BIT3
                Select Case i_Pol
                    Case 0
                        TxtPol.Visible = True
                        LblPol1.Text = "Linear"
                        LblPol.Text = If(senso, "Vertical", "Horizontal")
                    Case 1
                        TxtPol.Visible = False
                        LblPol1.Text = "Circular"
                        LblPol.Text = If(senso, "RHCP", "LHCP")
                End Select
                If IsNumeric(Mid$(s_POL, 1, 4)) Then TxtPol.Text = Format$(Val(Mid$(s_POL, 1, 4)) / 10, "#00.0")
            End If

            Select Case i_BackupAuto
                Case "S" : LblBackUp.Text = "Stand By"
                Case "P" : LblBackUp.Text = "Preset"
                Case "Q" : LblBackUp.Text = "Auto Acquire"
                Case "M" : LblBackUp.Text = "Memory Track"
                Case "T" : LblBackUp.Text = "Step-Track"
                Case "W" : LblBackUp.Text = "Wait"
                Case "B" : LblBackUp.Text = "Preset on Back-up"
                Case Else : LblBackUp.Text = ""
            End Select
            Select Case i_Switches
                Case "A" : LblSwitches.Text = "Autotrack"
                Case "T" : LblSwitches.Text = "Step-Track"
                Case "S" : LblSwitches.Text = "Stand-By"
                Case "P" : LblSwitches.Text = "Preset"
                Case Else : LblSwitches.Text = ""
            End Select
            Select Case i_Status
                Case OP_STANDBY : LblCommand.Text = "Stand By"
                Case OP_STEPTRACK : LblCommand.Text = "Step Track"
                Case OP_PROGRAMTRACK : LblCommand.Text = "Program Track"
                Case OP_AUTOTRACK : LblCommand.Text = "Auto Track"
                Case OP_AUGMENTED : LblCommand.Text = "Augmented"
                Case OP_COMMANDPOSITION : LblCommand.Text = "Command Position"
                Case OP_MEMORYTRACK : LblCommand.Text = "Memory Track"
                Case OP_AUTOMATICAQUIRE : LblCommand.Text = "Automatic Aquire"
                Case Else : LblCommand.Text = ""
            End Select

            ' K09: solo la frequenza beacon (Frequency out/Power Out non aggiornati, nascosti)
            If IsNumeric(f_FrqBeacon) Then TxtFreqBeacon.Text = Format$(f_FrqBeacon, "###00.000")

            If i_Alarm Then AggiornaGriglia()
        Catch
        End Try
    End Sub


End Class