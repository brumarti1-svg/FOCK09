' ============================================================
' frmConfigurazione.vb — convertito da Prog0347-FOCC10\frmConfigurazione.Frm (VB6)
' Telespazio FOC-C10 — DYNATRON
' Base: il porting .NET di FOC6 (stesso form VB6 a meno di 41 righe).
' Differenze di C10 rispetto a FOC6: 6 canali invece di 9 (i controlli
' dei canali 7-9 LNA/FILTER/MODALITA' POL. e gli Address LNA/SCU sono
' stati tolti dal designer: device non previsti nell'ini di C10).
'
' Note originali di FOC6:
' Traccia strutturale: BTS1\frmConfigurazione.vb (stessa origine VB6).
' Differenza genuina rispetto a BTS1: 9 canali (non 5), con Address LNA/SCU
' aggiuntivi (TxtAddressLNA/TxtAddressSCU) oltre ad Address DC — nel VB6
' originale erano control array (cmbMaster/txtIndLan/... con Index 1..9),
' qui mappati su controlli singoli numerati come già fatto in BTS1.
' ============================================================

Public Class FrmConfigurazione

    Friend TxtIndLan() As TextBox
    Friend TxtPorta() As TextBox
    Friend TxtInter() As TextBox
    Friend CmbPorCNLin() As ComboBox
    Friend CmbVelCNLin() As ComboBox
    Friend CmbParCNLin() As ComboBox
    Friend CmbDatCNLin() As ComboBox
    Friend CmbStoCNLin() As ComboBox
    Friend CmbSocket() As ComboBox
    Friend CmbMaster() As ComboBox
    Friend mskTimeOUT() As TextBox

    Dim tmpDato As String, tmpSezione As String, tmpPorta As String

    Private Sub InitArrays()
        TxtIndLan = New TextBox() {Nothing, TxtIndLan1, TxtIndLan2, TxtIndLan3, TxtIndLan4, TxtIndLan5, TxtIndLan6}
        TxtPorta = New TextBox() {Nothing, TxtPorta1, TxtPorta2, TxtPorta3, TxtPorta4, TxtPorta5, TxtPorta6}
        TxtInter = New TextBox() {Nothing, TxtInter1, TxtInter2, TxtInter3, TxtInter4, TxtInter5, TxtInter6}
        CmbPorCNLin = New ComboBox() {Nothing, CmbPorCNLin1, CmbPorCNLin2, CmbPorCNLin3, CmbPorCNLin4, CmbPorCNLin5, CmbPorCNLin6}
        CmbVelCNLin = New ComboBox() {Nothing, CmbVelCNLin1, CmbVelCNLin2, CmbVelCNLin3, CmbVelCNLin4, CmbVelCNLin5, CmbVelCNLin6}
        CmbParCNLin = New ComboBox() {Nothing, CmbParCNLin1, CmbParCNLin2, CmbParCNLin3, CmbParCNLin4, CmbParCNLin5, CmbParCNLin6}
        CmbDatCNLin = New ComboBox() {Nothing, CmbDatCNLin1, CmbDatCNLin2, CmbDatCNLin3, CmbDatCNLin4, CmbDatCNLin5, CmbDatCNLin6}
        CmbStoCNLin = New ComboBox() {Nothing, CmbStoCNLin1, CmbStoCNLin2, CmbStoCNLin3, CmbStoCNLin4, CmbStoCNLin5, CmbStoCNLin6}
        CmbSocket = New ComboBox() {Nothing, CmbSocket1, CmbSocket2, CmbSocket3, CmbSocket4, CmbSocket5, CmbSocket6}
        CmbMaster = New ComboBox() {Nothing, CmbMaster1, CmbMaster2, CmbMaster3, CmbMaster4, CmbMaster5, CmbMaster6}
        mskTimeOUT = New TextBox() {Nothing, mskTimeOUT1, mskTimeOUT2, mskTimeOUT3, mskTimeOUT4, mskTimeOUT5, mskTimeOUT6}

        For i As Integer = 1 To NumCom
            Dim idx As Integer = i
            AddHandler CmbPorCNLin(idx).Click, Sub(s, e) CmbPorCNLin_Click(idx)
            AddHandler TxtInter(idx).KeyUp, Sub(s As Object, e As KeyEventArgs) TxtInter_KeyUp(idx, e.KeyCode, 0)
        Next
    End Sub

    Private Sub ImpostaValoriAttuali()
        Dim Ind As Integer

        For Ind = 1 To NumCom
            tmpSezione = "Canale" + Trim(Str(Ind))
            tmpPorta = LeggiFileIni(tmpSezione, "PortaIN")
            If tmpPorta <> "" Then
                ImpComboBox(CmbPorCNLin(Ind), tmpPorta)
                tmpDato = LeggiFileIni(tmpSezione, "Vel")
                ImpComboBox(CmbVelCNLin(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "Par")
                ImpComboBox(CmbParCNLin(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "BitDati")
                ImpComboBox(CmbDatCNLin(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "BitStop")
                ImpComboBox(CmbStoCNLin(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "TimeOUT")
                mskTimeOUT(Ind).Text = CDbl(Val(tmpDato)).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)
                tmpDato = LeggiFileIni(tmpSezione, "Ind_IP")
                TxtIndLan(Ind).Text = tmpDato
                tmpDato = LeggiFileIni(tmpSezione, "Socket")
                ImpComboBox(CmbSocket(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "Master")
                ImpComboBox(CmbMaster(Ind), tmpDato)
                tmpDato = LeggiFileIni(tmpSezione, "Porta")
                TxtPorta(Ind).Text = tmpDato
                tmpDato = LeggiFileIni(tmpSezione, "Inter")
                TxtInter(Ind).Text = tmpDato
            End If
        Next Ind
        tmpSezione = "Default"
        tmpDato = LeggiFileIni(tmpSezione, "ProtEntity")
        CmbProtEntity.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "Log")
        TxtLog.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "Validita")
        TxtValidita.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "ValTLE")
        TxtValTLE.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "Tolleranza")
        TxtTolleranza.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "ScalaMin")
        TxtMin.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "ScalaMax")
        TxtMax.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "Pendenza")
        TxtPendenza.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "Pendenza1")
        TxtPendenza1.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "Internet")
        CmbInternet.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "ChangePhase")
        If tmpDato = "N" Then
            CmbChangePhase.Text = "Y"
        Else
            CmbChangePhase.Text = "N"
        End If
        tmpDato = LeggiFileIni(tmpSezione, "Valore0")
        TxtCentro.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "IT")
        TxtIT.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "AT")
        TxtAT.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "ET")
        TxtET.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "XT")
        TxtXT.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "YT")
        TxtYT.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "RT")
        TxtRT.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "CY")
        TxtCY.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "BM")
        TxtBM.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "BackUPPT")
        Select Case tmpDato
            Case "P"
                TxtBackUp.Text = "Program Track"
                BackPT = &H4
            Case "S"
                TxtBackUp.Text = "Stand-By"
                BackPT = &H0
            Case "T"
                TxtBackUp.Text = "Step Track"
                BackPT = &H20
        End Select
        tmpDato = LeggiFileIni(tmpSezione, "BackUPAutoT")
        Select Case tmpDato
            Case "S"
                TxtAutoTrackBackUp.Text = "Stand By"
            Case "Q"
                TxtAutoTrackBackUp.Text = "Auto Acquire"
            Case "M"
                TxtAutoTrackBackUp.Text = "Memory Track"
            Case "P"
                TxtAutoTrackBackUp.Text = "Preset"
            Case "T"
                TxtAutoTrackBackUp.Text = "Step Track"
        End Select

        tmpDato = LeggiFileIni(tmpSezione, "ThrAuto")
        TxtThrAuto.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "MaxErrAz")
        TxtThrAz.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "MaxErrEl")
        TxtThrEl.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "MaxOffset")
        TxtOffset.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "AddressDC")
        TxtAddressDC.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "MinEl")
        TxtElMin.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "IncEl")
        TxtAuto.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "IncEl1")
        TxtAugmented.Text = tmpDato
        tmpDato = LeggiFileIni(tmpSezione, "CoorOff")
        TxtCoorOff.Text = tmpDato
        tmpDato = LeggiFileIni("Default", "Antenna")
        ImpComboBox(CmbAntenna, tmpDato)
        tmpDato = LeggiFileIni("Default", "Satellite")
        ImpComboBox(CmbSatellite, tmpDato)
        tmpDato = LeggiFileIni("Default", "Ptic")
        ImpComboBox(CmbPtic, tmpDato)
        tmpDato = LeggiFileIni("Default", "IntLog")
        TxtStepLog.Text = tmpDato
        tmpDato = LeggiFileIni("Server", "Directory")
        TxtDirectory.Text = tmpDato
        g_DirPar = TxtDirectory.Text
        tmpDato = LeggiFileIni("Server", "BackUp")
        TxtDirBackUp.Text = tmpDato
        g_DirBackUp = TxtDirBackUp.Text
        If g_Log Then
            Me.LblLog.BackColor = Color.Lime
            Me.CmdLOG.Text = "NO"
        Else
            Me.LblLog.BackColor = Color.Red
            Me.CmdLOG.Text = "YES"
        End If
        Me.TxtIntCom.Text = CStr(i_IntCom)
    End Sub

    Private Sub ScriviNuoviValori()
        Dim Ind As Integer

        For Ind = 1 To NumCom
            tmpSezione = "Canale" + Trim(Str(Ind))
            tmpPorta = CmbPorCNLin(Ind).Text
            ScriviFileIni(tmpSezione, "PortaIN", Trim(tmpPorta))
            ScriviFileIni(tmpSezione, "Vel", Trim(CmbVelCNLin(Ind).Text))
            ScriviFileIni(tmpSezione, "Par", Trim(CmbParCNLin(Ind).Text))
            ScriviFileIni(tmpSezione, "BitDati", Trim(CmbDatCNLin(Ind).Text))
            ScriviFileIni(tmpSezione, "BitStop", Trim(CmbStoCNLin(Ind).Text))
            ScriviFileIni(tmpSezione, "TimeOut", CDbl(Val(mskTimeOUT(Ind).Text)).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture))
            ScriviFileIni(tmpSezione, "Ind_IP", Trim(TxtIndLan(Ind).Text))
            ScriviFileIni(tmpSezione, "Socket", Trim(CmbSocket(Ind).Text))
            ScriviFileIni(tmpSezione, "Master", Trim(CmbMaster(Ind).Text))
            ScriviFileIni(tmpSezione, "Porta", Trim(TxtPorta(Ind).Text))
            ScriviFileIni(tmpSezione, "Inter", Trim(TxtInter(Ind).Text))
            Select Case Ind
                Case 1
                    i_Inter = TxtInter(Ind).Text
            End Select
        Next Ind
        tmpSezione = "Default"
        ScriviFileIni(tmpSezione, "ProtEntity", Trim(CmbProtEntity.Text))
        ScriviFileIni(tmpSezione, "Log", Trim(TxtLog.Text))
        ScriviFileIni(tmpSezione, "IntCom", Trim(TxtIntCom.Text))
        ScriviFileIni(tmpSezione, "Validita", Trim(TxtValidita.Text))
        ScriviFileIni(tmpSezione, "ValTLE", Trim(TxtValTLE.Text))
        ScriviFileIni(tmpSezione, "BackUpPT", Trim(BackUpPT))
        ScriviFileIni(tmpSezione, "BackUpAutoT", Trim(BackUpAutoT))
        ScriviFileIni(tmpSezione, "Tolleranza", (TxtTolleranza.Text))
        ScriviFileIni(tmpSezione, "ThrAuto", Trim(TxtThrAuto.Text))
        ScriviFileIni(tmpSezione, "MaxErrAz", Trim(TxtThrAz.Text))
        ScriviFileIni(tmpSezione, "MaxErrEl", Trim(TxtThrEl.Text))
        ScriviFileIni(tmpSezione, "MaxOffset", Trim(TxtOffset.Text))
        ScriviFileIni(tmpSezione, "CoorOff", Trim(TxtCoorOff.Text))
        ScriviFileIni(tmpSezione, "ScalaMax", Trim(TxtMax.Text))
        ScriviFileIni(tmpSezione, "ScalaMin", Trim(TxtMin.Text))
        ScriviFileIni(tmpSezione, "MinEl", TxtElMin.Text)
        ScriviFileIni(tmpSezione, "AddressDC", TxtAddressDC.Text)
        AddressDC = TxtAddressDC.Text
        ScriviFileIni(tmpSezione, "IncEl", TxtAuto.Text)
        ScriviFileIni(tmpSezione, "IncEl1", TxtAugmented.Text)
        ScalaMax = Trim(TxtMax.Text)
        ScalaMin = Trim(TxtMin.Text)
        Pendenza = Trim(TxtPendenza.Text)
        Pendenza1 = Trim(TxtPendenza1.Text)
        Valore0 = Trim(TxtCentro.Text)
        ScriviFileIni(tmpSezione, "Pendenza", Trim(TxtPendenza.Text))
        ScriviFileIni(tmpSezione, "Pendenza1", Trim(TxtPendenza1.Text))
        ScriviFileIni(tmpSezione, "Valore0", Trim(TxtCentro.Text))
        ScriviFileIni(tmpSezione, "Internet", CmbInternet.Text)
        If CmbChangePhase.Text = "N" Then
            ScriviFileIni(tmpSezione, "ChangePhase", "Y")
        Else
            ScriviFileIni(tmpSezione, "ChangePhase", "N")
        End If
        ScriviFileIni(tmpSezione, "IT", TxtIT.Text)
        ScriviFileIni(tmpSezione, "AT", TxtAT.Text)
        ScriviFileIni(tmpSezione, "ET", TxtET.Text)
        ScriviFileIni(tmpSezione, "XT", TxtXT.Text)
        ScriviFileIni(tmpSezione, "YT", TxtYT.Text)
        ScriviFileIni(tmpSezione, "RT", TxtRT.Text)
        ScriviFileIni(tmpSezione, "CY", TxtCY.Text)
        ScriviFileIni(tmpSezione, "BM", TxtBM.Text)
        g_ThrAuto = Trim(TxtThrAuto.Text)
        g_ThrAz = Trim(TxtThrAz.Text)
        g_ThrEl = Trim(TxtThrEl.Text)
        ScriviFileIni("Server", "Directory", Trim(TxtDirectory.Text))
        ScriviFileIni("Server", "BackUp", TxtDirBackUp.Text)
        ScriviFileIni("Default", "Antenna", Trim(CmbAntenna.Text))
        ScriviFileIni("Default", "Satellite", Trim(CmbSatellite.Text))
        ScriviFileIni("Default", "Ptic", (CmbPtic.Text))
        ScriviFileIni("Default", "Intlog", Trim(TxtStepLog.Text))
        g_DirPar = Trim(TxtDirectory.Text)
        i_Log = Trim(TxtStepLog.Text)
        g_Server = LeggiFileIni("Server", "directory")
        g_Database = Application.StartupPath & "\Orbite.db"
        g_DirPar = TxtDirectory.Text
        g_DirBackUp = TxtDirBackUp.Text
    End Sub

    Private Sub CmbPorCNLin_Click(Index As Integer)
        LblMSG.Text = ""
    End Sub

    Private Sub Salva()
        Try
            i_Cont = 2
            ScriviNuoviValori()
            frmRicezione.Close()
            i_Cont = 1
            Me.Close()
            frmRicezione.Show()
        Catch ex As Exception
            WriteError(0, "Salva: " & ex.Message)
        End Try
    End Sub

    Private Sub PreparaForm()
        Dim Ind As Integer

        For Ind = 1 To NumCom
            CmbPorCNLin(Ind).Items.Clear()
            CmbPorCNLin(Ind).Items.Add("COM1")
            CmbPorCNLin(Ind).Items.Add("COM2")
            CmbPorCNLin(Ind).Items.Add("COM3")
            CmbPorCNLin(Ind).Items.Add("COM4")
            CmbPorCNLin(Ind).Items.Add("COM5")
            CmbPorCNLin(Ind).Items.Add("COM6")
            CmbPorCNLin(Ind).Items.Add("COM7")
            CmbPorCNLin(Ind).Items.Add("COM8")
            CmbPorCNLin(Ind).Items.Add("COM9")
            CmbPorCNLin(Ind).Items.Add("COM10")
            CmbPorCNLin(Ind).Items.Add("COM11")
            CmbPorCNLin(Ind).Items.Add("")
            CmbVelCNLin(Ind).Items.Add("1200")
            CmbVelCNLin(Ind).Items.Add("2400")
            CmbVelCNLin(Ind).Items.Add("4800")
            CmbVelCNLin(Ind).Items.Add("9600")
            CmbVelCNLin(Ind).Items.Add("14400")
            CmbVelCNLin(Ind).Items.Add("19200")
            CmbVelCNLin(Ind).Items.Add("38400")
            CmbParCNLin(Ind).Items.Add("E")
            CmbParCNLin(Ind).Items.Add("N")
            CmbParCNLin(Ind).Items.Add("O")
            CmbDatCNLin(Ind).Items.Add("7")
            CmbDatCNLin(Ind).Items.Add("8")
            CmbStoCNLin(Ind).Items.Add("1")
            CmbStoCNLin(Ind).Items.Add("2")
            mskTimeOUT(Ind).Text = "00"
        Next

        rs_Orbite = dbOrbite.OpenRecordset("Antenne", dbOpenDynaset)
        CmbAntenna.Items.Clear()
        Do While Not rs_Orbite.EOF
            If Not IsDBNull(rs_Orbite!Antenna) Then
                CmbAntenna.Items.Add(rs_Orbite!Antenna)
            End If
            rs_Orbite.MoveNext
        Loop
        rs_Orbite = dbOrbite.OpenRecordset("Satelliti", dbOpenDynaset)
        CmbSatellite.Items.Clear()
        Do While Not rs_Orbite.EOF
            If Not IsDBNull(rs_Orbite!NomeSatellite) Then
                CmbSatellite.Items.Add(rs_Orbite!NomeSatellite)
            End If
            rs_Orbite.MoveNext
        Loop
        rs_Orbite.Close()
        If g_Log Then
            Me.LblLog.BackColor = Color.Lime
            Me.CmdLOG.Text = "NO"
        Else
            Me.LblLog.BackColor = Color.Red
            Me.CmdLOG.Text = "YES"
        End If
    End Sub

    Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
        MnuMain_Click(0)
    End Sub
    Private Sub mnuSalva_Click(sender As Object, e As EventArgs) Handles mnuSalva.Click
        MnuMain_Click(1)
    End Sub
    Private Sub mnuConfigura_Click(sender As Object, e As EventArgs) Handles mnuConfigura.Click
        MnuMain_Click(2)
    End Sub
    Private Sub mnuBackupPT_Click(sender As Object, e As EventArgs) Handles mnuBackupPT.Click
        MnuMain_Click(3)
    End Sub
    Private Sub mnuAutoTrack_Click(sender As Object, e As EventArgs) Handles mnuAutoTrack.Click
        MnuMain_Click(4)
    End Sub

    Private Sub MnuMain_Click(Index As Integer)
        Select Case Index
            Case 0
                Me.Close()
            Case 1
                Salva()
            Case 2
                i_Cont = 2
                SEND = CheckSum("07@")
                b_int(ACU) = True
                b_Ricezione(ACU) = True
                InvioACU(SEND)
            Case 3
                frmBackPT.ShowDialog()
                Select Case BackUpPT
                    Case "P"
                        TxtBackUp.Text = "Program Track"
                    Case "S"
                        TxtBackUp.Text = "Stand-By"
                    Case "T"
                        TxtBackUp.Text = "Step Track"
                End Select
            Case 4
                Dim fATB As New frmAutoTrackBackPT() : fATB.ShowDialog()
                Select Case BackUpAutoT
                    Case "S"
                        TxtAutoTrackBackUp.Text = "Stand By"
                    Case "Q"
                        TxtAutoTrackBackUp.Text = "Auto Acquire"
                    Case "M"
                        TxtAutoTrackBackUp.Text = "Memory Track"
                    Case "P"
                        TxtAutoTrackBackUp.Text = "Preset"
                    Case "T"
                        TxtAutoTrackBackUp.Text = "Step Track"
                End Select
        End Select
    End Sub

    Private Sub CmdLOG_Click() Handles CmdLOG.Click
        Dim s_msg As String, i_lun As Integer
        Try
            If CmdLOG.Text = "YES" Then
                CmdLOG.Text = "NO"
                LblLog.BackColor = Color.Lime
                g_Log = True
                Dim logPath1 As String = Application.StartupPath & "\" & Canali(ACU).File_Log
                System.IO.File.WriteAllText(logPath1, "")
                s_msg = Canali(ACU).File_Log
                i_lun = InStr(1, s_msg, ".", 1)
                s_msg = Mid$(s_msg, 1, i_lun - 1) & "1" & Mid$(s_msg, i_lun, 4)
                Dim logPath2 As String = Application.StartupPath & "\" & s_msg
                System.IO.File.WriteAllText(logPath2, "")
            Else
                CmdLOG.Text = "YES"
                LblLog.BackColor = Color.Red
                g_Log = False
            End If
        Catch ex As Exception
            WriteError(0, "CmdLOG_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub TxtCoorOff_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtCoorOff.KeyUp
        tmpSezione = "Default"
        If e.KeyCode = Keys.Return And IsNumeric(TxtCoorOff.Text) Then
            CoorOff = CDbl(TxtCoorOff.Text)
            ScriviFileIni(tmpSezione, "CoorOff", Str(CoorOff))
        End If
    End Sub

    Private Sub TxtIntCom_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtIntCom.KeyUp
        tmpSezione = "Default"
        If e.KeyCode = Keys.Return And IsNumeric(TxtIntCom.Text) Then
            i_IntCom = CDbl(TxtIntCom.Text)
            ScriviFileIni(tmpSezione, "IntCom", Str(i_IntCom))
        End If
    End Sub

    Private Sub TxtInter_KeyUp(Index As Integer, KeyCode As Integer, Shift As Integer)
        tmpSezione = "Canale" + Trim(Str(Index))
        If KeyCode = CR And IsNumeric(TxtInter(Index).Text) Then
            i_Inter = CDbl(TxtInter(Index).Text)
            ScriviFileIni(tmpSezione, "Inter", Str(i_Inter))
        End If
    End Sub

    Private Sub FrmConfigurazione_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Campi di sola presentazione: giallo se sconosciuti, 128,255,255 se noti
        ColoriStandard.RegistraDati(TxtBackUp, TxtAutoTrackBackUp)
        InitArrays()
        PreparaForm()
        ImpostaValoriAttuali()
    End Sub

    Private Sub TxtOffset_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtOffset.KeyUp
        tmpSezione = "Default"
        If e.KeyCode = Keys.Return And IsNumeric(TxtOffset.Text) Then
            Max_Offset = CDbl(TxtOffset.Text)
            ScriviFileIni(tmpSezione, "MaxOffset", Str(Max_Offset))
        End If
    End Sub

    Private Sub TxtStepLog_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtStepLog.KeyUp
        tmpSezione = "Default"
        If e.KeyCode = Keys.Return And IsNumeric(TxtStepLog.Text) Then
            i_Log = TxtStepLog.Text
            ScriviFileIni(tmpSezione, "Intlog", TxtStepLog.Text)
        End If
    End Sub

End Class
