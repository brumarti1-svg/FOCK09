' Generato da frmRicezione.frm (VB6) — coordinate convertite: twips / 15 = pixel
' Form: 1064x588 px, MDI Child, WindowState=Maximized
' Controlli ignorati: Winsock, MSComm, TDBGrid, VB.Data, Image (rimpiazzati in codice)
' Array VB6 (LblAllarme[n], LblInputBuffer[n]) → controlli individuali o non inclusi
Partial Public Class FrmRicezione
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.tmrAlarm = New System.Windows.Forms.Timer(Me.components)
        Me.tmrTimeoutMsg1 = New System.Windows.Forms.Timer(Me.components)
        Me.TmrRic = New System.Windows.Forms.Timer(Me.components)
        Me.TmrSpiralAcquire = New System.Windows.Forms.Timer(Me.components)
        Me.tmrMsgTimeout2 = New System.Windows.Forms.Timer(Me.components)
        Me.tmrTimeoutMsg3 = New System.Windows.Forms.Timer(Me.components)
        Me.tmrTimeoutMsg4 = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuConfiguration = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPrint = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuBell = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuACU = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCommand = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStandBy = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStepTrack = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemoryTrack = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemInterpStep = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemRecEnable = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemRecDisable = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuProgramTrack = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSinusoidal = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuResetACU = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteCmd = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStow = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuUnstow = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuBackupPT = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCommandFree = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuOffsetCmd = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuRequest = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuReqStatus = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuReqParameter = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemoryTrackData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStowUnstowData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuUpdateTLE = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTLEManual = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTLETwoLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTLEThreeLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTLETableTrack = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPhase = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuReceiver = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuBeaconGraphic = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuAntenna = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphemerisMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphCassiopea = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphMoon = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphSatellite = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphSun = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphVenus = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuDownConverter = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuParameterACU = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuParameterArchive = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSimulation = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTraceOrbit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphemerisOrbit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuOrbitCartesian = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuOrbitPolar = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuAcquireTrace = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuAlarm = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuURL = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuInfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip2 = New System.Windows.Forms.MenuStrip()
        Me.pnlHeaders = New System.Windows.Forms.Panel()
        Me.LblHdrDis = New System.Windows.Forms.Label()
        Me.LblHdrAntenna = New System.Windows.Forms.Label()
        Me.LblHdrError = New System.Windows.Forms.Label()
        Me.LblHdrCommand = New System.Windows.Forms.Label()
        Me.LblHdrProgram = New System.Windows.Forms.Label()
        Me.LblHdrOffset = New System.Windows.Forms.Label()
        Me.LblHdrSwLimit = New System.Windows.Forms.Label()
        Me.LblHdrBrk = New System.Windows.Forms.Label()
        Me.pnlAzimuth = New System.Windows.Forms.Panel()
        Me.LblAzTag = New System.Windows.Forms.Label()
        Me.LblAzimuth = New System.Windows.Forms.Label()
        Me.TxtAzEr = New System.Windows.Forms.Label()
        Me.LblAzimuthC = New System.Windows.Forms.Label()
        Me.LblAzError = New System.Windows.Forms.Label()
        Me.LblAzErrEn = New System.Windows.Forms.Label()
        Me.chkAz = New System.Windows.Forms.CheckBox()
        Me.TxtAz = New System.Windows.Forms.TextBox()
        Me.LblAllarme4 = New System.Windows.Forms.Label()
        Me.LblAllarme5 = New System.Windows.Forms.Label()
        Me.LblAzTBrk = New System.Windows.Forms.Label()
        Me.pnlElevazione = New System.Windows.Forms.Panel()
        Me.LblElTag = New System.Windows.Forms.Label()
        Me.TxtEl = New System.Windows.Forms.TextBox()
        Me.LblElevazione = New System.Windows.Forms.Label()
        Me.TxtElEr = New System.Windows.Forms.Label()
        Me.LblElevazioneC = New System.Windows.Forms.Label()
        Me.LblElError = New System.Windows.Forms.Label()
        Me.LblElErrEn = New System.Windows.Forms.Label()
        Me.chkEl = New System.Windows.Forms.CheckBox()
        Me.LblAllarme12 = New System.Windows.Forms.Label()
        Me.LblAllarme13 = New System.Windows.Forms.Label()
        Me.LblElTBrk = New System.Windows.Forms.Label()
        Me.pnlComandi = New System.Windows.Forms.Panel()
        Me.CmdSTBY = New System.Windows.Forms.Button()
        Me.CmdCmdPos = New System.Windows.Forms.Button()
        Me.CmdStepTrack = New System.Windows.Forms.Button()
        Me.CmdAutoTrack = New System.Windows.Forms.Button()
        Me.CmdAugmented = New System.Windows.Forms.Button()
        Me.CmdProgramTrack = New System.Windows.Forms.Button()
        Me.CmdOffset = New System.Windows.Forms.Button()
        Me.CmdAutoSearch = New System.Windows.Forms.Button()
        Me.CmdSpirAcq = New System.Windows.Forms.Button()
        Me.CmdProi = New System.Windows.Forms.Button()
        Me.LblStepTrack = New System.Windows.Forms.Label()
        Me.CmdPhase = New System.Windows.Forms.Button()
        Me.pnlAcuStatus = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.LblAcuStatusTag = New System.Windows.Forms.Label()
        Me.TxtACU_ctrl = New System.Windows.Forms.Label()
        Me.CmdPC1_ctrl = New System.Windows.Forms.Button()
        Me.LblPC1_ctrl = New System.Windows.Forms.Label()
        Me.CmdLockBell_ctrl = New System.Windows.Forms.Button()
        Me.LblLock = New System.Windows.Forms.Label()
        Me.pnlReceiver = New System.Windows.Forms.Panel()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblPhaseElTag = New System.Windows.Forms.Label()
        Me.TxtPhaseEl = New System.Windows.Forms.Label()
        Me.LblRevAzTag = New System.Windows.Forms.Label()
        Me.LblRevAz_ctrl = New System.Windows.Forms.Label()
        Me.LblRFTag = New System.Windows.Forms.Label()
        Me.TxtInpFreq = New System.Windows.Forms.Label()
        Me.TxtPhAz = New System.Windows.Forms.TextBox()
        Me.LblPolRic = New System.Windows.Forms.Label()
        Me.TxtLna = New System.Windows.Forms.TextBox()
        Me.LblCommand = New System.Windows.Forms.Label()
        Me.LblPhase = New System.Windows.Forms.Label()
        Me.LblConfCNL1in = New System.Windows.Forms.Label()
        Me.LblConfCNL2in = New System.Windows.Forms.Label()
        Me.LblConfCNL3in = New System.Windows.Forms.Label()
        Me.LblConfCNL4in = New System.Windows.Forms.Label()
        Me.LblConfCNL6in = New System.Windows.Forms.Label()
        Me.Manager1 = New System.Windows.Forms.PictureBox()
        Me.CmdSum = New System.Windows.Forms.Button()
        Me.LblEmergency = New System.Windows.Forms.Label()
        Me.LblEmergency1 = New System.Windows.Forms.Label()
        Me.LblSatellite = New System.Windows.Forms.ComboBox()
        Me.LblWarning = New System.Windows.Forms.Label()
        Me.TxtOrdini = New System.Windows.Forms.TextBox()
        Me.BarBeaconCtrl = New System.Windows.Forms.ProgressBar()
        Me.LblAcuMode = New System.Windows.Forms.Label()
        Me.TxtMode_ctrl = New System.Windows.Forms.TextBox()
        Me.LblSatTag = New System.Windows.Forms.Label()
        Me.LblSumAlarm_ctrl = New System.Windows.Forms.Label()
        Me.LblLev_ctrl = New System.Windows.Forms.Label()
        Me.SS_Intestazione = New System.Windows.Forms.Label()
        Me.LblPol1 = New System.Windows.Forms.Label()
        Me.TxtPolErr = New System.Windows.Forms.TextBox()
        Me.LblPol = New System.Windows.Forms.Label()
        Me.TxtPol = New System.Windows.Forms.TextBox()
        Me.TxtFrqBeacon = New System.Windows.Forms.TextBox()
        Me.TxtPhase = New System.Windows.Forms.TextBox()
        Me.TxtRevAz = New System.Windows.Forms.TextBox()
        Me.LblLNA = New System.Windows.Forms.Label()
        Me.LblLNATag = New System.Windows.Forms.Label()
        Me.LblPolErrTag = New System.Windows.Forms.Label()
        Me.PnlPol = New System.Windows.Forms.Panel()
        Me.CmdPol1 = New System.Windows.Forms.Button()
        Me.LblBcnLevelTag = New System.Windows.Forms.Label()
        Me.LblBcnLevelUnit = New System.Windows.Forms.Label()
        Me.LblMemParamTag = New System.Windows.Forms.Label()
        Me.LblMemParamInd = New System.Windows.Forms.Label()
        Me.LblMemDataTag = New System.Windows.Forms.Label()
        Me.LblMemDataInd = New System.Windows.Forms.Label()
        Me.LblSatDataTag = New System.Windows.Forms.Label()
        Me.LblSatDataInd = New System.Windows.Forms.Label()
        Me.LblStowUnstowTag = New System.Windows.Forms.Label()
        Me.LblStowUnstowInd = New System.Windows.Forms.Label()
        Me.LblBcnFreq = New System.Windows.Forms.Label()
        Me.CmbBcnFreq = New System.Windows.Forms.ComboBox()
        Me.LblBcnMax_ctrl = New System.Windows.Forms.TextBox()
        Me.LblBcnMin_ctrl = New System.Windows.Forms.TextBox()
        Me.LblPcCmd = New System.Windows.Forms.Label()
        Me.CmdLoad = New System.Windows.Forms.Button()
        Me.LblConfCNL5in = New System.Windows.Forms.Label()
        Me.TxtCoorOff = New System.Windows.Forms.TextBox()
        Me.TxtThrEl = New System.Windows.Forms.TextBox()
        Me.TxtThrAz = New System.Windows.Forms.TextBox()
        Me.CmdSound = New System.Windows.Forms.Button()
        Me.CmdReset = New System.Windows.Forms.Button()
        Me.LblSwitches = New System.Windows.Forms.Label()
        Me.LblSwitchesTag = New System.Windows.Forms.Label()
        Me.LblBackUp = New System.Windows.Forms.Label()
        Me.LblBackUpTag = New System.Windows.Forms.Label()
        Me.LblRisposta = New System.Windows.Forms.Label()
        Me.LblErrAz = New System.Windows.Forms.Label()
        Me.LblErrEl = New System.Windows.Forms.Label()
        Me.LblRVR1 = New System.Windows.Forms.Label()
        Me.LblRVR2 = New System.Windows.Forms.Label()
        Me.LblRVR3 = New System.Windows.Forms.Label()
        Me.LblRVR4 = New System.Windows.Forms.Label()
        Me.LblRVR6 = New System.Windows.Forms.Label()
        Me.LblInputBuffer2 = New System.Windows.Forms.Label()
        Me.LblInputBuffer3 = New System.Windows.Forms.Label()
        Me.LblInputBuffer4 = New System.Windows.Forms.Label()
        Me.LblInputBuffer5 = New System.Windows.Forms.Label()
        Me.TxtOffsetMax = New System.Windows.Forms.TextBox()
        Me.LblOffsetMaxTag = New System.Windows.Forms.Label()
        Me.TxtSogliaAuto = New System.Windows.Forms.TextBox()
        Me.LblThrAutoTag = New System.Windows.Forms.Label()
        Me.LblThrAzTag = New System.Windows.Forms.Label()
        Me.LblThrElTag = New System.Windows.Forms.Label()
        Me.picLogoLeft = New System.Windows.Forms.PictureBox()
        Me.picLogoRight = New System.Windows.Forms.PictureBox()
        Me.tdbLog = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtPhasAz = New System.Windows.Forms.TextBox()
        Me.MenuStrip1.SuspendLayout()
        Me.pnlHeaders.SuspendLayout()
        Me.pnlAzimuth.SuspendLayout()
        Me.pnlElevazione.SuspendLayout()
        Me.pnlComandi.SuspendLayout()
        Me.pnlAcuStatus.SuspendLayout()
        Me.pnlReceiver.SuspendLayout()
        CType(Me.Manager1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PnlPol.SuspendLayout()
        CType(Me.picLogoLeft, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLogoRight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tdbLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrAlarm
        '
        Me.tmrAlarm.Interval = 2000
        '
        'tmrTimeoutMsg1
        '
        Me.tmrTimeoutMsg1.Interval = 30000
        '
        'TmrRic
        '
        Me.TmrRic.Interval = 500
        '
        'TmrSpiralAcquire
        '
        '
        'tmrMsgTimeout2
        '
        Me.tmrMsgTimeout2.Interval = 60000
        '
        'tmrTimeoutMsg3
        '
        Me.tmrTimeoutMsg3.Interval = 60000
        '
        'tmrTimeoutMsg4
        '
        Me.tmrTimeoutMsg4.Interval = 1000
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuFile, Me.MnuACU, Me.MnuUpdateTLE, Me.MnuPhase, Me.MnuReceiver, Me.MnuBeaconGraphic, Me.MnuAntenna, Me.MnuEphemerisMenu, Me.MnuSatelliteMenu, Me.MnuDownConverter, Me.MnuParameterACU, Me.MnuParameterArchive, Me.MnuSimulation, Me.MnuTraceOrbit, Me.MnuEphemerisOrbit, Me.MnuAcquireTrace, Me.MnuAlarm, Me.MnuURL, Me.MnuInfo})
        Me.MenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1339, 52)
        Me.MenuStrip1.TabIndex = 0
        '
        'MnuFile
        '
        Me.MnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuConfiguration, Me.MnuPrint, Me.MnuBell, Me.MnuExit})
        Me.MnuFile.Name = "MnuFile"
        Me.MnuFile.Size = New System.Drawing.Size(46, 24)
        Me.MnuFile.Text = "File"
        '
        'MnuConfiguration
        '
        Me.MnuConfiguration.Name = "MnuConfiguration"
        Me.MnuConfiguration.Size = New System.Drawing.Size(183, 26)
        Me.MnuConfiguration.Text = "Configuration"
        '
        'MnuPrint
        '
        Me.MnuPrint.Enabled = False
        Me.MnuPrint.Name = "MnuPrint"
        Me.MnuPrint.Size = New System.Drawing.Size(183, 26)
        Me.MnuPrint.Text = "Print"
        '
        'MnuBell
        '
        Me.MnuBell.Name = "MnuBell"
        Me.MnuBell.Size = New System.Drawing.Size(183, 26)
        Me.MnuBell.Text = "Bell"
        '
        'MnuExit
        '
        Me.MnuExit.Name = "MnuExit"
        Me.MnuExit.Size = New System.Drawing.Size(183, 26)
        Me.MnuExit.Text = "Exit"
        '
        'MnuACU
        '
        Me.MnuACU.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuCommand, Me.MnuRequest})
        Me.MnuACU.Name = "MnuACU"
        Me.MnuACU.Size = New System.Drawing.Size(52, 24)
        Me.MnuACU.Text = "ACU"
        '
        'MnuCommand
        '
        Me.MnuCommand.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuStandBy, Me.MnuStepTrack, Me.MnuMemoryTrack, Me.MnuMemInterpStep, Me.MnuMemRecEnable, Me.MnuMemRecDisable, Me.MnuProgramTrack, Me.MnuSinusoidal, Me.MnuResetACU, Me.MnuSatelliteCmd, Me.MnuStow, Me.MnuUnstow, Me.MnuBackupPT, Me.MnuCommandFree, Me.MnuOffsetCmd})
        Me.MnuCommand.Name = "MnuCommand"
        Me.MnuCommand.Size = New System.Drawing.Size(161, 26)
        Me.MnuCommand.Text = "Command"
        '
        'MnuStandBy
        '
        Me.MnuStandBy.Enabled = False
        Me.MnuStandBy.Name = "MnuStandBy"
        Me.MnuStandBy.Size = New System.Drawing.Size(309, 26)
        Me.MnuStandBy.Text = "Stand By"
        '
        'MnuStepTrack
        '
        Me.MnuStepTrack.Enabled = False
        Me.MnuStepTrack.Name = "MnuStepTrack"
        Me.MnuStepTrack.Size = New System.Drawing.Size(309, 26)
        Me.MnuStepTrack.Text = "Step Track"
        '
        'MnuMemoryTrack
        '
        Me.MnuMemoryTrack.Name = "MnuMemoryTrack"
        Me.MnuMemoryTrack.Size = New System.Drawing.Size(309, 26)
        Me.MnuMemoryTrack.Text = "Memory Track"
        '
        'MnuMemInterpStep
        '
        Me.MnuMemInterpStep.Name = "MnuMemInterpStep"
        Me.MnuMemInterpStep.Size = New System.Drawing.Size(309, 26)
        Me.MnuMemInterpStep.Text = "Memory Interpolation Step Track"
        '
        'MnuMemRecEnable
        '
        Me.MnuMemRecEnable.Name = "MnuMemRecEnable"
        Me.MnuMemRecEnable.Size = New System.Drawing.Size(309, 26)
        Me.MnuMemRecEnable.Text = "Memory Recording - Enable"
        '
        'MnuMemRecDisable
        '
        Me.MnuMemRecDisable.Name = "MnuMemRecDisable"
        Me.MnuMemRecDisable.Size = New System.Drawing.Size(309, 26)
        Me.MnuMemRecDisable.Text = "Memory Recording - Disable"
        '
        'MnuProgramTrack
        '
        Me.MnuProgramTrack.Enabled = False
        Me.MnuProgramTrack.Name = "MnuProgramTrack"
        Me.MnuProgramTrack.Size = New System.Drawing.Size(309, 26)
        Me.MnuProgramTrack.Text = "Program Track"
        '
        'MnuSinusoidal
        '
        Me.MnuSinusoidal.Enabled = False
        Me.MnuSinusoidal.Name = "MnuSinusoidal"
        Me.MnuSinusoidal.Size = New System.Drawing.Size(309, 26)
        Me.MnuSinusoidal.Text = "Sinusoidal Regression"
        '
        'MnuResetACU
        '
        Me.MnuResetACU.Name = "MnuResetACU"
        Me.MnuResetACU.Size = New System.Drawing.Size(309, 26)
        Me.MnuResetACU.Text = "Reset ACU"
        '
        'MnuSatelliteCmd
        '
        Me.MnuSatelliteCmd.Name = "MnuSatelliteCmd"
        Me.MnuSatelliteCmd.Size = New System.Drawing.Size(309, 26)
        Me.MnuSatelliteCmd.Text = "Satellite"
        '
        'MnuStow
        '
        Me.MnuStow.Name = "MnuStow"
        Me.MnuStow.Size = New System.Drawing.Size(309, 26)
        Me.MnuStow.Text = "Stow"
        '
        'MnuUnstow
        '
        Me.MnuUnstow.Name = "MnuUnstow"
        Me.MnuUnstow.Size = New System.Drawing.Size(309, 26)
        Me.MnuUnstow.Text = "Unstow"
        '
        'MnuBackupPT
        '
        Me.MnuBackupPT.Enabled = False
        Me.MnuBackupPT.Name = "MnuBackupPT"
        Me.MnuBackupPT.Size = New System.Drawing.Size(309, 26)
        Me.MnuBackupPT.Text = "Backup for Program Track"
        '
        'MnuCommandFree
        '
        Me.MnuCommandFree.Enabled = False
        Me.MnuCommandFree.Name = "MnuCommandFree"
        Me.MnuCommandFree.Size = New System.Drawing.Size(309, 26)
        Me.MnuCommandFree.Text = "Command Free"
        '
        'MnuOffsetCmd
        '
        Me.MnuOffsetCmd.Enabled = False
        Me.MnuOffsetCmd.Name = "MnuOffsetCmd"
        Me.MnuOffsetCmd.Size = New System.Drawing.Size(309, 26)
        Me.MnuOffsetCmd.Text = "Offset"
        '
        'MnuRequest
        '
        Me.MnuRequest.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuReqStatus, Me.MnuReqParameter, Me.MnuSatelliteData, Me.MnuMemoryTrackData, Me.MnuStowUnstowData})
        Me.MnuRequest.Name = "MnuRequest"
        Me.MnuRequest.Size = New System.Drawing.Size(161, 26)
        Me.MnuRequest.Text = "Request"
        '
        'MnuReqStatus
        '
        Me.MnuReqStatus.Enabled = False
        Me.MnuReqStatus.Name = "MnuReqStatus"
        Me.MnuReqStatus.Size = New System.Drawing.Size(221, 26)
        Me.MnuReqStatus.Text = "Status"
        '
        'MnuReqParameter
        '
        Me.MnuReqParameter.Enabled = False
        Me.MnuReqParameter.Name = "MnuReqParameter"
        Me.MnuReqParameter.Size = New System.Drawing.Size(221, 26)
        Me.MnuReqParameter.Text = "Parameter"
        '
        'MnuSatelliteData
        '
        Me.MnuSatelliteData.Name = "MnuSatelliteData"
        Me.MnuSatelliteData.Size = New System.Drawing.Size(221, 26)
        Me.MnuSatelliteData.Text = "Satellite Data"
        '
        'MnuMemoryTrackData
        '
        Me.MnuMemoryTrackData.Name = "MnuMemoryTrackData"
        Me.MnuMemoryTrackData.Size = New System.Drawing.Size(221, 26)
        Me.MnuMemoryTrackData.Text = "Memory Track Data"
        '
        'MnuStowUnstowData
        '
        Me.MnuStowUnstowData.Enabled = False
        Me.MnuStowUnstowData.Name = "MnuStowUnstowData"
        Me.MnuStowUnstowData.Size = New System.Drawing.Size(221, 26)
        Me.MnuStowUnstowData.Text = "Stow/Unstow data"
        '
        'MnuUpdateTLE
        '
        Me.MnuUpdateTLE.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuTLEManual, Me.MnuTLETwoLine, Me.MnuTLEThreeLine, Me.MnuTLETableTrack})
        Me.MnuUpdateTLE.Name = "MnuUpdateTLE"
        Me.MnuUpdateTLE.Size = New System.Drawing.Size(99, 24)
        Me.MnuUpdateTLE.Text = "Update TLE"
        '
        'MnuTLEManual
        '
        Me.MnuTLEManual.Name = "MnuTLEManual"
        Me.MnuTLEManual.Size = New System.Drawing.Size(165, 26)
        Me.MnuTLEManual.Text = "Manual"
        '
        'MnuTLETwoLine
        '
        Me.MnuTLETwoLine.Name = "MnuTLETwoLine"
        Me.MnuTLETwoLine.Size = New System.Drawing.Size(165, 26)
        Me.MnuTLETwoLine.Text = "Two Line"
        '
        'MnuTLEThreeLine
        '
        Me.MnuTLEThreeLine.Name = "MnuTLEThreeLine"
        Me.MnuTLEThreeLine.Size = New System.Drawing.Size(165, 26)
        Me.MnuTLEThreeLine.Text = "Three Line"
        '
        'MnuTLETableTrack
        '
        Me.MnuTLETableTrack.Name = "MnuTLETableTrack"
        Me.MnuTLETableTrack.Size = New System.Drawing.Size(165, 26)
        Me.MnuTLETableTrack.Text = "Table Track"
        '
        'MnuPhase
        '
        Me.MnuPhase.Name = "MnuPhase"
        Me.MnuPhase.Size = New System.Drawing.Size(61, 24)
        Me.MnuPhase.Text = "Phase"
        '
        'MnuReceiver
        '
        Me.MnuReceiver.Name = "MnuReceiver"
        Me.MnuReceiver.Size = New System.Drawing.Size(79, 24)
        Me.MnuReceiver.Text = "Receiver"
        '
        'MnuBeaconGraphic
        '
        Me.MnuBeaconGraphic.Name = "MnuBeaconGraphic"
        Me.MnuBeaconGraphic.Size = New System.Drawing.Size(127, 24)
        Me.MnuBeaconGraphic.Text = "Beacon Graphic"
        '
        'MnuAntenna
        '
        Me.MnuAntenna.Name = "MnuAntenna"
        Me.MnuAntenna.Size = New System.Drawing.Size(78, 24)
        Me.MnuAntenna.Text = "Antenna"
        '
        'MnuEphemerisMenu
        '
        Me.MnuEphemerisMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuEphCassiopea, Me.MnuEphMoon, Me.MnuEphSatellite, Me.MnuEphSun, Me.MnuEphVenus})
        Me.MnuEphemerisMenu.Name = "MnuEphemerisMenu"
        Me.MnuEphemerisMenu.Size = New System.Drawing.Size(92, 24)
        Me.MnuEphemerisMenu.Text = "Ephemeris"
        '
        'MnuEphCassiopea
        '
        Me.MnuEphCassiopea.Name = "MnuEphCassiopea"
        Me.MnuEphCassiopea.Size = New System.Drawing.Size(190, 26)
        Me.MnuEphCassiopea.Text = "Cassiopea Alfa"
        '
        'MnuEphMoon
        '
        Me.MnuEphMoon.Name = "MnuEphMoon"
        Me.MnuEphMoon.Size = New System.Drawing.Size(190, 26)
        Me.MnuEphMoon.Text = "Moon"
        '
        'MnuEphSatellite
        '
        Me.MnuEphSatellite.Name = "MnuEphSatellite"
        Me.MnuEphSatellite.Size = New System.Drawing.Size(190, 26)
        Me.MnuEphSatellite.Text = "Satellite"
        '
        'MnuEphSun
        '
        Me.MnuEphSun.Name = "MnuEphSun"
        Me.MnuEphSun.Size = New System.Drawing.Size(190, 26)
        Me.MnuEphSun.Text = "Sun"
        '
        'MnuEphVenus
        '
        Me.MnuEphVenus.Name = "MnuEphVenus"
        Me.MnuEphVenus.Size = New System.Drawing.Size(190, 26)
        Me.MnuEphVenus.Text = "Venus"
        '
        'MnuSatelliteMenu
        '
        Me.MnuSatelliteMenu.Name = "MnuSatelliteMenu"
        Me.MnuSatelliteMenu.Size = New System.Drawing.Size(77, 24)
        Me.MnuSatelliteMenu.Text = "Satellite"
        '
        'MnuDownConverter
        '
        Me.MnuDownConverter.Name = "MnuDownConverter"
        Me.MnuDownConverter.Size = New System.Drawing.Size(130, 24)
        Me.MnuDownConverter.Text = "Down Converter"
        '
        'MnuParameterACU
        '
        Me.MnuParameterACU.Name = "MnuParameterACU"
        Me.MnuParameterACU.Size = New System.Drawing.Size(123, 24)
        Me.MnuParameterACU.Text = "Parameter ACU"
        '
        'MnuParameterArchive
        '
        Me.MnuParameterArchive.Enabled = False
        Me.MnuParameterArchive.Name = "MnuParameterArchive"
        Me.MnuParameterArchive.Size = New System.Drawing.Size(143, 24)
        Me.MnuParameterArchive.Text = "Parameter Archive"
        Me.MnuParameterArchive.Visible = False
        '
        'MnuSimulation
        '
        Me.MnuSimulation.Name = "MnuSimulation"
        Me.MnuSimulation.Size = New System.Drawing.Size(94, 24)
        Me.MnuSimulation.Text = "Simulation"
        '
        'MnuTraceOrbit
        '
        Me.MnuTraceOrbit.Name = "MnuTraceOrbit"
        Me.MnuTraceOrbit.Size = New System.Drawing.Size(96, 24)
        Me.MnuTraceOrbit.Text = "Trace Orbit"
        '
        'MnuEphemerisOrbit
        '
        Me.MnuEphemerisOrbit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuOrbitCartesian, Me.MnuOrbitPolar})
        Me.MnuEphemerisOrbit.Name = "MnuEphemerisOrbit"
        Me.MnuEphemerisOrbit.Size = New System.Drawing.Size(130, 24)
        Me.MnuEphemerisOrbit.Text = "Ephemeris Orbit"
        '
        'MnuOrbitCartesian
        '
        Me.MnuOrbitCartesian.Name = "MnuOrbitCartesian"
        Me.MnuOrbitCartesian.Size = New System.Drawing.Size(184, 26)
        Me.MnuOrbitCartesian.Text = "Cartesian Axis"
        '
        'MnuOrbitPolar
        '
        Me.MnuOrbitPolar.Name = "MnuOrbitPolar"
        Me.MnuOrbitPolar.Size = New System.Drawing.Size(184, 26)
        Me.MnuOrbitPolar.Text = "Polar"
        '
        'MnuAcquireTrace
        '
        Me.MnuAcquireTrace.Name = "MnuAcquireTrace"
        Me.MnuAcquireTrace.Size = New System.Drawing.Size(113, 24)
        Me.MnuAcquireTrace.Text = "Acquire Trace"
        Me.MnuAcquireTrace.Visible = False
        '
        'MnuAlarm
        '
        Me.MnuAlarm.Name = "MnuAlarm"
        Me.MnuAlarm.Size = New System.Drawing.Size(63, 24)
        Me.MnuAlarm.Text = "Alarm"
        '
        'MnuURL
        '
        Me.MnuURL.Name = "MnuURL"
        Me.MnuURL.Size = New System.Drawing.Size(49, 24)
        Me.MnuURL.Text = "URL"
        '
        'MnuInfo
        '
        Me.MnuInfo.Name = "MnuInfo"
        Me.MnuInfo.Size = New System.Drawing.Size(30, 24)
        Me.MnuInfo.Text = "?"
        '
        'MenuStrip2
        '
        Me.MenuStrip2.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip2.Location = New System.Drawing.Point(0, 52)
        Me.MenuStrip2.Name = "MenuStrip2"
        Me.MenuStrip2.Size = New System.Drawing.Size(1339, 24)
        Me.MenuStrip2.TabIndex = 1
        '
        'pnlHeaders
        '
        Me.pnlHeaders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlHeaders.Controls.Add(Me.LblHdrDis)
        Me.pnlHeaders.Controls.Add(Me.LblHdrAntenna)
        Me.pnlHeaders.Controls.Add(Me.LblHdrError)
        Me.pnlHeaders.Controls.Add(Me.LblHdrCommand)
        Me.pnlHeaders.Controls.Add(Me.LblHdrProgram)
        Me.pnlHeaders.Controls.Add(Me.LblHdrOffset)
        Me.pnlHeaders.Controls.Add(Me.LblHdrSwLimit)
        Me.pnlHeaders.Controls.Add(Me.LblHdrBrk)
        Me.pnlHeaders.Location = New System.Drawing.Point(12, 160)
        Me.pnlHeaders.Name = "pnlHeaders"
        Me.pnlHeaders.Size = New System.Drawing.Size(804, 29)
        Me.pnlHeaders.TabIndex = 2
        '
        'LblHdrDis
        '
        Me.LblHdrDis.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrDis.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrDis.Location = New System.Drawing.Point(61, 0)
        Me.LblHdrDis.Name = "LblHdrDis"
        Me.LblHdrDis.Size = New System.Drawing.Size(32, 18)
        Me.LblHdrDis.TabIndex = 0
        Me.LblHdrDis.Text = "Dis."
        Me.LblHdrDis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrAntenna
        '
        Me.LblHdrAntenna.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrAntenna.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrAntenna.Location = New System.Drawing.Point(107, 5)
        Me.LblHdrAntenna.Name = "LblHdrAntenna"
        Me.LblHdrAntenna.Size = New System.Drawing.Size(90, 18)
        Me.LblHdrAntenna.TabIndex = 1
        Me.LblHdrAntenna.Text = "Antenna"
        Me.LblHdrAntenna.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrError
        '
        Me.LblHdrError.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrError.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrError.Location = New System.Drawing.Point(228, 5)
        Me.LblHdrError.Name = "LblHdrError"
        Me.LblHdrError.Size = New System.Drawing.Size(46, 18)
        Me.LblHdrError.TabIndex = 2
        Me.LblHdrError.Text = "Error"
        Me.LblHdrError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrCommand
        '
        Me.LblHdrCommand.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrCommand.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrCommand.Location = New System.Drawing.Point(287, 5)
        Me.LblHdrCommand.Name = "LblHdrCommand"
        Me.LblHdrCommand.Size = New System.Drawing.Size(75, 18)
        Me.LblHdrCommand.TabIndex = 3
        Me.LblHdrCommand.Text = "Command"
        Me.LblHdrCommand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrProgram
        '
        Me.LblHdrProgram.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrProgram.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrProgram.Location = New System.Drawing.Point(378, 5)
        Me.LblHdrProgram.Name = "LblHdrProgram"
        Me.LblHdrProgram.Size = New System.Drawing.Size(68, 18)
        Me.LblHdrProgram.TabIndex = 4
        Me.LblHdrProgram.Text = "Program"
        Me.LblHdrProgram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrOffset
        '
        Me.LblHdrOffset.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrOffset.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrOffset.Location = New System.Drawing.Point(476, 5)
        Me.LblHdrOffset.Name = "LblHdrOffset"
        Me.LblHdrOffset.Size = New System.Drawing.Size(70, 18)
        Me.LblHdrOffset.TabIndex = 5
        Me.LblHdrOffset.Text = "Offset"
        Me.LblHdrOffset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrSwLimit
        '
        Me.LblHdrSwLimit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrSwLimit.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrSwLimit.Location = New System.Drawing.Point(596, 5)
        Me.LblHdrSwLimit.Name = "LblHdrSwLimit"
        Me.LblHdrSwLimit.Size = New System.Drawing.Size(70, 18)
        Me.LblHdrSwLimit.TabIndex = 9
        Me.LblHdrSwLimit.Text = "SW Limit"
        Me.LblHdrSwLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblHdrBrk
        '
        Me.LblHdrBrk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblHdrBrk.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblHdrBrk.Location = New System.Drawing.Point(718, 5)
        Me.LblHdrBrk.Name = "LblHdrBrk"
        Me.LblHdrBrk.Size = New System.Drawing.Size(39, 18)
        Me.LblHdrBrk.TabIndex = 11
        Me.LblHdrBrk.Text = "BRK"
        Me.LblHdrBrk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlAzimuth
        '
        Me.pnlAzimuth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlAzimuth.Controls.Add(Me.LblAzTag)
        Me.pnlAzimuth.Controls.Add(Me.LblAzimuth)
        Me.pnlAzimuth.Controls.Add(Me.TxtAzEr)
        Me.pnlAzimuth.Controls.Add(Me.LblAzimuthC)
        Me.pnlAzimuth.Controls.Add(Me.LblAzError)
        Me.pnlAzimuth.Controls.Add(Me.LblAzErrEn)
        Me.pnlAzimuth.Controls.Add(Me.chkAz)
        Me.pnlAzimuth.Controls.Add(Me.TxtAz)
        Me.pnlAzimuth.Controls.Add(Me.LblAllarme4)
        Me.pnlAzimuth.Controls.Add(Me.LblAllarme5)
        Me.pnlAzimuth.Controls.Add(Me.LblAzTBrk)
        Me.pnlAzimuth.Location = New System.Drawing.Point(16, 195)
        Me.pnlAzimuth.Name = "pnlAzimuth"
        Me.pnlAzimuth.Size = New System.Drawing.Size(800, 51)
        Me.pnlAzimuth.TabIndex = 3
        '
        'LblAzTag
        '
        Me.LblAzTag.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzTag.Location = New System.Drawing.Point(4, 9)
        Me.LblAzTag.Name = "LblAzTag"
        Me.LblAzTag.Size = New System.Drawing.Size(65, 27)
        Me.LblAzTag.TabIndex = 0
        Me.LblAzTag.Text = "AZ"
        '
        'LblAzimuth
        '
        Me.LblAzimuth.BackColor = System.Drawing.Color.Yellow
        Me.LblAzimuth.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzimuth.Location = New System.Drawing.Point(285, 6)
        Me.LblAzimuth.Name = "LblAzimuth"
        Me.LblAzimuth.Size = New System.Drawing.Size(82, 32)
        Me.LblAzimuth.TabIndex = 2
        Me.LblAzimuth.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtAzEr
        '
        Me.TxtAzEr.BackColor = System.Drawing.Color.Yellow
        Me.TxtAzEr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtAzEr.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Bold)
        Me.TxtAzEr.Location = New System.Drawing.Point(227, 9)
        Me.TxtAzEr.Name = "TxtAzEr"
        Me.TxtAzEr.Size = New System.Drawing.Size(49, 27)
        Me.TxtAzEr.TabIndex = 3
        Me.TxtAzEr.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblAzimuthC
        '
        Me.LblAzimuthC.BackColor = System.Drawing.Color.Yellow
        Me.LblAzimuthC.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzimuthC.Location = New System.Drawing.Point(373, 6)
        Me.LblAzimuthC.Name = "LblAzimuthC"
        Me.LblAzimuthC.Size = New System.Drawing.Size(82, 32)
        Me.LblAzimuthC.TabIndex = 4
        Me.LblAzimuthC.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblAzError
        '
        Me.LblAzError.BackColor = System.Drawing.Color.Yellow
        Me.LblAzError.Location = New System.Drawing.Point(461, 9)
        Me.LblAzError.Name = "LblAzError"
        Me.LblAzError.Size = New System.Drawing.Size(48, 27)
        Me.LblAzError.TabIndex = 5
        Me.LblAzError.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblAzErrEn
        '
        Me.LblAzErrEn.BackColor = System.Drawing.Color.Yellow
        Me.LblAzErrEn.Location = New System.Drawing.Point(515, 9)
        Me.LblAzErrEn.Name = "LblAzErrEn"
        Me.LblAzErrEn.Size = New System.Drawing.Size(48, 27)
        Me.LblAzErrEn.TabIndex = 6
        '
        'chkAz
        '
        Me.chkAz.Location = New System.Drawing.Point(75, 14)
        Me.chkAz.Name = "chkAz"
        Me.chkAz.Size = New System.Drawing.Size(14, 16)
        Me.chkAz.TabIndex = 0
        '
        'TxtAz
        '
        Me.TxtAz.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TxtAz.Font = New System.Drawing.Font("Arial", 22.2!, System.Drawing.FontStyle.Bold)
        Me.TxtAz.Location = New System.Drawing.Point(99, 3)
        Me.TxtAz.Name = "TxtAz"
        Me.TxtAz.Size = New System.Drawing.Size(125, 50)
        Me.TxtAz.TabIndex = 1
        Me.TxtAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblAllarme4
        '
        Me.LblAllarme4.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme4.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme4.ForeColor = System.Drawing.Color.White
        Me.LblAllarme4.Location = New System.Drawing.Point(583, 15)
        Me.LblAllarme4.Name = "LblAllarme4"
        Me.LblAllarme4.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme4.TabIndex = 204
        Me.LblAllarme4.Text = "CCW"
        Me.LblAllarme4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme5
        '
        Me.LblAllarme5.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme5.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme5.ForeColor = System.Drawing.Color.White
        Me.LblAllarme5.Location = New System.Drawing.Point(624, 15)
        Me.LblAllarme5.Name = "LblAllarme5"
        Me.LblAllarme5.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme5.TabIndex = 205
        Me.LblAllarme5.Text = "CW"
        Me.LblAllarme5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAzTBrk
        '
        Me.LblAzTBrk.BackColor = System.Drawing.Color.Yellow
        Me.LblAzTBrk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzTBrk.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.LblAzTBrk.Location = New System.Drawing.Point(689, 11)
        Me.LblAzTBrk.Name = "LblAzTBrk"
        Me.LblAzTBrk.Size = New System.Drawing.Size(87, 22)
        Me.LblAzTBrk.TabIndex = 208
        Me.LblAzTBrk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlElevazione
        '
        Me.pnlElevazione.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlElevazione.Controls.Add(Me.LblElTag)
        Me.pnlElevazione.Controls.Add(Me.TxtEl)
        Me.pnlElevazione.Controls.Add(Me.LblElevazione)
        Me.pnlElevazione.Controls.Add(Me.TxtElEr)
        Me.pnlElevazione.Controls.Add(Me.LblElevazioneC)
        Me.pnlElevazione.Controls.Add(Me.LblElError)
        Me.pnlElevazione.Controls.Add(Me.LblElErrEn)
        Me.pnlElevazione.Controls.Add(Me.chkEl)
        Me.pnlElevazione.Controls.Add(Me.LblAllarme12)
        Me.pnlElevazione.Controls.Add(Me.LblAllarme13)
        Me.pnlElevazione.Controls.Add(Me.LblElTBrk)
        Me.pnlElevazione.Location = New System.Drawing.Point(16, 262)
        Me.pnlElevazione.Name = "pnlElevazione"
        Me.pnlElevazione.Size = New System.Drawing.Size(800, 53)
        Me.pnlElevazione.TabIndex = 4
        '
        'LblElTag
        '
        Me.LblElTag.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Bold)
        Me.LblElTag.Location = New System.Drawing.Point(3, 10)
        Me.LblElTag.Name = "LblElTag"
        Me.LblElTag.Size = New System.Drawing.Size(60, 27)
        Me.LblElTag.TabIndex = 0
        Me.LblElTag.Text = "EL"
        '
        'TxtEl
        '
        Me.TxtEl.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TxtEl.Font = New System.Drawing.Font("Arial", 22.2!, System.Drawing.FontStyle.Bold)
        Me.TxtEl.Location = New System.Drawing.Point(96, 3)
        Me.TxtEl.Name = "TxtEl"
        Me.TxtEl.Size = New System.Drawing.Size(125, 50)
        Me.TxtEl.TabIndex = 1
        Me.TxtEl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblElevazione
        '
        Me.LblElevazione.BackColor = System.Drawing.Color.Yellow
        Me.LblElevazione.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblElevazione.Location = New System.Drawing.Point(282, 7)
        Me.LblElevazione.Name = "LblElevazione"
        Me.LblElevazione.Size = New System.Drawing.Size(82, 32)
        Me.LblElevazione.TabIndex = 2
        Me.LblElevazione.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtElEr
        '
        Me.TxtElEr.BackColor = System.Drawing.Color.Yellow
        Me.TxtElEr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtElEr.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Bold)
        Me.TxtElEr.Location = New System.Drawing.Point(229, 10)
        Me.TxtElEr.Name = "TxtElEr"
        Me.TxtElEr.Size = New System.Drawing.Size(49, 27)
        Me.TxtElEr.TabIndex = 3
        Me.TxtElEr.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblElevazioneC
        '
        Me.LblElevazioneC.BackColor = System.Drawing.Color.Yellow
        Me.LblElevazioneC.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblElevazioneC.Location = New System.Drawing.Point(370, 7)
        Me.LblElevazioneC.Name = "LblElevazioneC"
        Me.LblElevazioneC.Size = New System.Drawing.Size(82, 32)
        Me.LblElevazioneC.TabIndex = 4
        Me.LblElevazioneC.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblElError
        '
        Me.LblElError.BackColor = System.Drawing.Color.Yellow
        Me.LblElError.Location = New System.Drawing.Point(461, 11)
        Me.LblElError.Name = "LblElError"
        Me.LblElError.Size = New System.Drawing.Size(48, 27)
        Me.LblElError.TabIndex = 5
        Me.LblElError.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblElErrEn
        '
        Me.LblElErrEn.BackColor = System.Drawing.Color.Yellow
        Me.LblElErrEn.Location = New System.Drawing.Point(515, 10)
        Me.LblElErrEn.Name = "LblElErrEn"
        Me.LblElErrEn.Size = New System.Drawing.Size(48, 27)
        Me.LblElErrEn.TabIndex = 6
        '
        'chkEl
        '
        Me.chkEl.Location = New System.Drawing.Point(70, 14)
        Me.chkEl.Name = "chkEl"
        Me.chkEl.Size = New System.Drawing.Size(14, 16)
        Me.chkEl.TabIndex = 0
        '
        'LblAllarme12
        '
        Me.LblAllarme12.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme12.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme12.ForeColor = System.Drawing.Color.White
        Me.LblAllarme12.Location = New System.Drawing.Point(583, 14)
        Me.LblAllarme12.Name = "LblAllarme12"
        Me.LblAllarme12.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme12.TabIndex = 214
        Me.LblAllarme12.Text = "UP"
        Me.LblAllarme12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme13
        '
        Me.LblAllarme13.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme13.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme13.ForeColor = System.Drawing.Color.White
        Me.LblAllarme13.Location = New System.Drawing.Point(624, 14)
        Me.LblAllarme13.Name = "LblAllarme13"
        Me.LblAllarme13.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme13.TabIndex = 215
        Me.LblAllarme13.Text = "DW"
        Me.LblAllarme13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblElTBrk
        '
        Me.LblElTBrk.BackColor = System.Drawing.Color.Yellow
        Me.LblElTBrk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElTBrk.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.LblElTBrk.Location = New System.Drawing.Point(686, 12)
        Me.LblElTBrk.Name = "LblElTBrk"
        Me.LblElTBrk.Size = New System.Drawing.Size(85, 22)
        Me.LblElTBrk.TabIndex = 216
        Me.LblElTBrk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlComandi
        '
        Me.pnlComandi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlComandi.Controls.Add(Me.CmdSTBY)
        Me.pnlComandi.Controls.Add(Me.CmdCmdPos)
        Me.pnlComandi.Controls.Add(Me.CmdStepTrack)
        Me.pnlComandi.Controls.Add(Me.CmdAutoTrack)
        Me.pnlComandi.Controls.Add(Me.CmdAugmented)
        Me.pnlComandi.Controls.Add(Me.CmdProgramTrack)
        Me.pnlComandi.Controls.Add(Me.CmdOffset)
        Me.pnlComandi.Controls.Add(Me.CmdAutoSearch)
        Me.pnlComandi.Controls.Add(Me.CmdSpirAcq)
        Me.pnlComandi.Controls.Add(Me.CmdProi)
        Me.pnlComandi.Controls.Add(Me.LblStepTrack)
        Me.pnlComandi.Location = New System.Drawing.Point(21, 460)
        Me.pnlComandi.Name = "pnlComandi"
        Me.pnlComandi.Size = New System.Drawing.Size(960, 76)
        Me.pnlComandi.TabIndex = 5
        '
        'CmdSTBY
        '
        Me.CmdSTBY.Location = New System.Drawing.Point(5, 9)
        Me.CmdSTBY.Name = "CmdSTBY"
        Me.CmdSTBY.Size = New System.Drawing.Size(87, 56)
        Me.CmdSTBY.TabIndex = 0
        Me.CmdSTBY.Text = "Stand By (F1)"
        '
        'CmdCmdPos
        '
        Me.CmdCmdPos.Location = New System.Drawing.Point(94, 10)
        Me.CmdCmdPos.Name = "CmdCmdPos"
        Me.CmdCmdPos.Size = New System.Drawing.Size(87, 56)
        Me.CmdCmdPos.TabIndex = 1
        Me.CmdCmdPos.Text = "Cmd Pos (F2)"
        '
        'CmdStepTrack
        '
        Me.CmdStepTrack.Location = New System.Drawing.Point(183, 10)
        Me.CmdStepTrack.Name = "CmdStepTrack"
        Me.CmdStepTrack.Size = New System.Drawing.Size(87, 56)
        Me.CmdStepTrack.TabIndex = 2
        Me.CmdStepTrack.Text = "Step Track (F3)"
        '
        'CmdAutoTrack
        '
        Me.CmdAutoTrack.Location = New System.Drawing.Point(325, 10)
        Me.CmdAutoTrack.Name = "CmdAutoTrack"
        Me.CmdAutoTrack.Size = New System.Drawing.Size(87, 56)
        Me.CmdAutoTrack.TabIndex = 3
        Me.CmdAutoTrack.Text = "Auto Track (F4)"
        '
        'CmdAugmented
        '
        Me.CmdAugmented.Location = New System.Drawing.Point(414, 10)
        Me.CmdAugmented.Name = "CmdAugmented"
        Me.CmdAugmented.Size = New System.Drawing.Size(87, 56)
        Me.CmdAugmented.TabIndex = 4
        Me.CmdAugmented.Text = "Augmented"
        '
        'CmdProgramTrack
        '
        Me.CmdProgramTrack.Location = New System.Drawing.Point(503, 10)
        Me.CmdProgramTrack.Name = "CmdProgramTrack"
        Me.CmdProgramTrack.Size = New System.Drawing.Size(87, 56)
        Me.CmdProgramTrack.TabIndex = 5
        Me.CmdProgramTrack.Text = "Program Track"
        '
        'CmdOffset
        '
        Me.CmdOffset.Location = New System.Drawing.Point(592, 10)
        Me.CmdOffset.Name = "CmdOffset"
        Me.CmdOffset.Size = New System.Drawing.Size(87, 56)
        Me.CmdOffset.TabIndex = 6
        Me.CmdOffset.Text = "Offset"
        Me.CmdOffset.Visible = False
        '
        'CmdAutoSearch
        '
        Me.CmdAutoSearch.Location = New System.Drawing.Point(681, 14)
        Me.CmdAutoSearch.Name = "CmdAutoSearch"
        Me.CmdAutoSearch.Size = New System.Drawing.Size(87, 56)
        Me.CmdAutoSearch.TabIndex = 7
        Me.CmdAutoSearch.Text = "Auto Search"
        '
        'CmdSpirAcq
        '
        Me.CmdSpirAcq.Location = New System.Drawing.Point(770, 14)
        Me.CmdSpirAcq.Name = "CmdSpirAcq"
        Me.CmdSpirAcq.Size = New System.Drawing.Size(87, 56)
        Me.CmdSpirAcq.TabIndex = 8
        Me.CmdSpirAcq.Text = "Spiral Acquire"
        '
        'CmdProi
        '
        Me.CmdProi.Location = New System.Drawing.Point(859, 14)
        Me.CmdProi.Name = "CmdProi"
        Me.CmdProi.Size = New System.Drawing.Size(87, 56)
        Me.CmdProi.TabIndex = 9
        Me.CmdProi.Text = "Proj"
        '
        'LblStepTrack
        '
        Me.LblStepTrack.BackColor = System.Drawing.Color.Yellow
        Me.LblStepTrack.Location = New System.Drawing.Point(272, 25)
        Me.LblStepTrack.Name = "LblStepTrack"
        Me.LblStepTrack.Size = New System.Drawing.Size(51, 27)
        Me.LblStepTrack.TabIndex = 10
        Me.LblStepTrack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LblStepTrack.Visible = False
        '
        'CmdPhase
        '
        Me.CmdPhase.Location = New System.Drawing.Point(1271, 601)
        Me.CmdPhase.Name = "CmdPhase"
        Me.CmdPhase.Size = New System.Drawing.Size(33, 20)
        Me.CmdPhase.TabIndex = 23
        '
        'pnlAcuStatus
        '
        Me.pnlAcuStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlAcuStatus.Controls.Add(Me.Label8)
        Me.pnlAcuStatus.Controls.Add(Me.LblAcuStatusTag)
        Me.pnlAcuStatus.Controls.Add(Me.TxtACU_ctrl)
        Me.pnlAcuStatus.Controls.Add(Me.CmdPC1_ctrl)
        Me.pnlAcuStatus.Controls.Add(Me.LblPC1_ctrl)
        Me.pnlAcuStatus.Controls.Add(Me.CmdLockBell_ctrl)
        Me.pnlAcuStatus.Controls.Add(Me.LblLock)
        Me.pnlAcuStatus.Location = New System.Drawing.Point(471, 352)
        Me.pnlAcuStatus.Name = "pnlAcuStatus"
        Me.pnlAcuStatus.Size = New System.Drawing.Size(698, 36)
        Me.pnlAcuStatus.TabIndex = 6
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(303, 9)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(85, 16)
        Me.Label8.TabIndex = 26
        Me.Label8.Text = "RCV Status"
        '
        'LblAcuStatusTag
        '
        Me.LblAcuStatusTag.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.LblAcuStatusTag.Location = New System.Drawing.Point(6, 9)
        Me.LblAcuStatusTag.Name = "LblAcuStatusTag"
        Me.LblAcuStatusTag.Size = New System.Drawing.Size(57, 18)
        Me.LblAcuStatusTag.TabIndex = 0
        Me.LblAcuStatusTag.Text = "ACU Status"
        '
        'TxtACU_ctrl
        '
        Me.TxtACU_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.TxtACU_ctrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtACU_ctrl.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtACU_ctrl.Location = New System.Drawing.Point(68, 7)
        Me.TxtACU_ctrl.Name = "TxtACU_ctrl"
        Me.TxtACU_ctrl.Size = New System.Drawing.Size(55, 22)
        Me.TxtACU_ctrl.TabIndex = 1
        '
        'CmdPC1_ctrl
        '
        Me.CmdPC1_ctrl.BackColor = System.Drawing.SystemColors.Control
        Me.CmdPC1_ctrl.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.CmdPC1_ctrl.Location = New System.Drawing.Point(137, 3)
        Me.CmdPC1_ctrl.Name = "CmdPC1_ctrl"
        Me.CmdPC1_ctrl.Size = New System.Drawing.Size(54, 26)
        Me.CmdPC1_ctrl.TabIndex = 2
        Me.CmdPC1_ctrl.Text = "PC1"
        Me.CmdPC1_ctrl.UseVisualStyleBackColor = False
        '
        'LblPC1_ctrl
        '
        Me.LblPC1_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.LblPC1_ctrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblPC1_ctrl.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblPC1_ctrl.Location = New System.Drawing.Point(194, 7)
        Me.LblPC1_ctrl.Name = "LblPC1_ctrl"
        Me.LblPC1_ctrl.Size = New System.Drawing.Size(51, 22)
        Me.LblPC1_ctrl.TabIndex = 3
        '
        'CmdLockBell_ctrl
        '
        Me.CmdLockBell_ctrl.Font = New System.Drawing.Font("Arial", 7.8!)
        Me.CmdLockBell_ctrl.Location = New System.Drawing.Point(455, 6)
        Me.CmdLockBell_ctrl.Name = "CmdLockBell_ctrl"
        Me.CmdLockBell_ctrl.Size = New System.Drawing.Size(49, 22)
        Me.CmdLockBell_ctrl.TabIndex = 4
        Me.CmdLockBell_ctrl.Text = "Bell"
        '
        'LblLock
        '
        Me.LblLock.BackColor = System.Drawing.Color.Yellow
        Me.LblLock.Location = New System.Drawing.Point(378, 5)
        Me.LblLock.Name = "LblLock"
        Me.LblLock.Size = New System.Drawing.Size(53, 24)
        Me.LblLock.TabIndex = 25
        Me.LblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlReceiver
        '
        Me.pnlReceiver.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlReceiver.Controls.Add(Me.Label10)
        Me.pnlReceiver.Controls.Add(Me.Label5)
        Me.pnlReceiver.Controls.Add(Me.Label4)
        Me.pnlReceiver.Controls.Add(Me.LblPhaseElTag)
        Me.pnlReceiver.Controls.Add(Me.TxtPhaseEl)
        Me.pnlReceiver.Controls.Add(Me.LblRevAzTag)
        Me.pnlReceiver.Controls.Add(Me.LblRevAz_ctrl)
        Me.pnlReceiver.Controls.Add(Me.LblRFTag)
        Me.pnlReceiver.Controls.Add(Me.TxtInpFreq)
        Me.pnlReceiver.Controls.Add(Me.TxtPhAz)
        Me.pnlReceiver.Controls.Add(Me.LblPolRic)
        Me.pnlReceiver.Controls.Add(Me.TxtLna)
        Me.pnlReceiver.Location = New System.Drawing.Point(614, 633)
        Me.pnlReceiver.Name = "pnlReceiver"
        Me.pnlReceiver.Size = New System.Drawing.Size(718, 41)
        Me.pnlReceiver.TabIndex = 8
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(450, 14)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(36, 16)
        Me.Label10.TabIndex = 50
        Me.Label10.Text = "LNA"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(585, 10)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(27, 16)
        Me.Label5.TabIndex = 48
        Me.Label5.Text = "Pol"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(113, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(22, 16)
        Me.Label4.TabIndex = 47
        Me.Label4.Text = "Az"
        '
        'LblPhaseElTag
        '
        Me.LblPhaseElTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblPhaseElTag.Location = New System.Drawing.Point(5, 17)
        Me.LblPhaseElTag.Name = "LblPhaseElTag"
        Me.LblPhaseElTag.Size = New System.Drawing.Size(56, 18)
        Me.LblPhaseElTag.TabIndex = 0
        Me.LblPhaseElTag.Text = "Phase El"
        Me.LblPhaseElTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPhaseEl
        '
        Me.TxtPhaseEl.BackColor = System.Drawing.Color.Yellow
        Me.TxtPhaseEl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtPhaseEl.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.TxtPhaseEl.Location = New System.Drawing.Point(65, 14)
        Me.TxtPhaseEl.Name = "TxtPhaseEl"
        Me.TxtPhaseEl.Size = New System.Drawing.Size(42, 22)
        Me.TxtPhaseEl.TabIndex = 1
        Me.TxtPhaseEl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRevAzTag
        '
        Me.LblRevAzTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblRevAzTag.Location = New System.Drawing.Point(198, 15)
        Me.LblRevAzTag.Name = "LblRevAzTag"
        Me.LblRevAzTag.Size = New System.Drawing.Size(53, 18)
        Me.LblRevAzTag.TabIndex = 2
        Me.LblRevAzTag.Text = "Rev Az"
        Me.LblRevAzTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRevAz_ctrl
        '
        Me.LblRevAz_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.LblRevAz_ctrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblRevAz_ctrl.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.LblRevAz_ctrl.Location = New System.Drawing.Point(270, 14)
        Me.LblRevAz_ctrl.Name = "LblRevAz_ctrl"
        Me.LblRevAz_ctrl.Size = New System.Drawing.Size(25, 22)
        Me.LblRevAz_ctrl.TabIndex = 3
        Me.LblRevAz_ctrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRFTag
        '
        Me.LblRFTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblRFTag.Location = New System.Drawing.Point(292, 14)
        Me.LblRFTag.Name = "LblRFTag"
        Me.LblRFTag.Size = New System.Drawing.Size(78, 18)
        Me.LblRFTag.TabIndex = 4
        Me.LblRFTag.Text = "RF (MHz)"
        Me.LblRFTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtInpFreq
        '
        Me.TxtInpFreq.BackColor = System.Drawing.Color.Yellow
        Me.TxtInpFreq.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtInpFreq.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.TxtInpFreq.Location = New System.Drawing.Point(374, 12)
        Me.TxtInpFreq.Name = "TxtInpFreq"
        Me.TxtInpFreq.Size = New System.Drawing.Size(70, 22)
        Me.TxtInpFreq.TabIndex = 5
        Me.TxtInpFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtPhAz
        '
        Me.TxtPhAz.BackColor = System.Drawing.Color.Yellow
        Me.TxtPhAz.Location = New System.Drawing.Point(141, 12)
        Me.TxtPhAz.Name = "TxtPhAz"
        Me.TxtPhAz.Size = New System.Drawing.Size(51, 22)
        Me.TxtPhAz.TabIndex = 46
        Me.TxtPhAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPhAz.Visible = False
        '
        'LblPolRic
        '
        Me.LblPolRic.BackColor = System.Drawing.Color.Yellow
        Me.LblPolRic.Location = New System.Drawing.Point(625, 10)
        Me.LblPolRic.Name = "LblPolRic"
        Me.LblPolRic.Size = New System.Drawing.Size(78, 22)
        Me.LblPolRic.TabIndex = 42
        '
        'TxtLna
        '
        Me.TxtLna.BackColor = System.Drawing.Color.Yellow
        Me.TxtLna.Location = New System.Drawing.Point(488, 10)
        Me.TxtLna.Name = "TxtLna"
        Me.TxtLna.Size = New System.Drawing.Size(37, 22)
        Me.TxtLna.TabIndex = 44
        Me.TxtLna.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblCommand
        '
        Me.LblCommand.BackColor = System.Drawing.Color.Yellow
        Me.LblCommand.ForeColor = System.Drawing.SystemColors.Window
        Me.LblCommand.Location = New System.Drawing.Point(506, 120)
        Me.LblCommand.Name = "LblCommand"
        Me.LblCommand.Size = New System.Drawing.Size(219, 24)
        Me.LblCommand.TabIndex = 12
        Me.LblCommand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblPhase
        '
        Me.LblPhase.BackColor = System.Drawing.Color.Yellow
        Me.LblPhase.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblPhase.Location = New System.Drawing.Point(1127, 603)
        Me.LblPhase.Name = "LblPhase"
        Me.LblPhase.Size = New System.Drawing.Size(128, 17)
        Me.LblPhase.TabIndex = 24
        Me.LblPhase.Text = "Auto Change Phase"
        '
        'LblConfCNL1in
        '
        Me.LblConfCNL1in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL1in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL1in.Location = New System.Drawing.Point(935, 244)
        Me.LblConfCNL1in.Name = "LblConfCNL1in"
        Me.LblConfCNL1in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL1in.TabIndex = 28
        Me.LblConfCNL1in.Text = "ACU"
        Me.LblConfCNL1in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL2in
        '
        Me.LblConfCNL2in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL2in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL2in.Location = New System.Drawing.Point(1028, 296)
        Me.LblConfCNL2in.Name = "LblConfCNL2in"
        Me.LblConfCNL2in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL2in.TabIndex = 29
        Me.LblConfCNL2in.Text = "MCS"
        Me.LblConfCNL2in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL3in
        '
        Me.LblConfCNL3in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL3in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL3in.Location = New System.Drawing.Point(933, 296)
        Me.LblConfCNL3in.Name = "LblConfCNL3in"
        Me.LblConfCNL3in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL3in.TabIndex = 30
        Me.LblConfCNL3in.Text = "RCV"
        Me.LblConfCNL3in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL4in
        '
        Me.LblConfCNL4in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL4in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL4in.Location = New System.Drawing.Point(984, 244)
        Me.LblConfCNL4in.Name = "LblConfCNL4in"
        Me.LblConfCNL4in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL4in.TabIndex = 31
        Me.LblConfCNL4in.Text = "D/C"
        Me.LblConfCNL4in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL6in
        '
        Me.LblConfCNL6in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL6in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL6in.Location = New System.Drawing.Point(982, 296)
        Me.LblConfCNL6in.Visible = False   ' K09: nascosto nel VB6
        Me.LblConfCNL6in.Name = "LblConfCNL6in"
        Me.LblConfCNL6in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL6in.TabIndex = 55
        Me.LblConfCNL6in.Text = "POL"
        Me.LblConfCNL6in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Manager1
        '
        Me.Manager1.Location = New System.Drawing.Point(731, 713)
        Me.Manager1.Name = "Manager1"
        Me.Manager1.Size = New System.Drawing.Size(96, 38)
        Me.Manager1.TabIndex = 58
        Me.Manager1.TabStop = False
        Me.Manager1.Visible = False
        '
        'CmdSum
        '
        Me.CmdSum.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.CmdSum.Location = New System.Drawing.Point(930, 160)
        Me.CmdSum.Name = "CmdSum"
        Me.CmdSum.Size = New System.Drawing.Size(125, 26)
        Me.CmdSum.TabIndex = 59
        Me.CmdSum.Text = "Sum.Alarm"
        Me.CmdSum.UseVisualStyleBackColor = False
        '
        'LblEmergency
        '
        Me.LblEmergency.BackColor = System.Drawing.Color.Yellow
        Me.LblEmergency.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblEmergency.Location = New System.Drawing.Point(1064, 195)
        Me.LblEmergency.Name = "LblEmergency"
        Me.LblEmergency.Size = New System.Drawing.Size(35, 41)
        Me.LblEmergency.TabIndex = 60
        Me.LblEmergency.Text = "IN"
        Me.LblEmergency.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblEmergency1
        '
        Me.LblEmergency1.BackColor = System.Drawing.Color.Yellow
        Me.LblEmergency1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblEmergency1.Location = New System.Drawing.Point(1105, 193)
        Me.LblEmergency1.Name = "LblEmergency1"
        Me.LblEmergency1.Size = New System.Drawing.Size(44, 41)
        Me.LblEmergency1.TabIndex = 61
        Me.LblEmergency1.Text = "OUT"
        Me.LblEmergency1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblSatellite
        '
        Me.LblSatellite.BackColor = System.Drawing.Color.Yellow
        Me.LblSatellite.Location = New System.Drawing.Point(930, 123)
        Me.LblSatellite.Name = "LblSatellite"
        Me.LblSatellite.Size = New System.Drawing.Size(219, 24)
        Me.LblSatellite.TabIndex = 14
        '
        'LblWarning
        '
        Me.LblWarning.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblWarning.Location = New System.Drawing.Point(550, 568)
        Me.LblWarning.BackColor = System.Drawing.Color.Yellow
        Me.LblWarning.Name = "LblWarning"
        Me.LblWarning.Size = New System.Drawing.Size(390, 27)
        Me.LblWarning.TabIndex = 27
        Me.LblWarning.Visible = False
        '
        'TxtOrdini
        '
        Me.TxtOrdini.BackColor = System.Drawing.Color.White
        Me.TxtOrdini.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.TxtOrdini.ForeColor = System.Drawing.Color.White
        Me.TxtOrdini.Location = New System.Drawing.Point(20, 707)
        Me.TxtOrdini.Name = "TxtOrdini"
        Me.TxtOrdini.Size = New System.Drawing.Size(384, 30)
        Me.TxtOrdini.TabIndex = 34
        '
        'BarBeaconCtrl
        '
        Me.BarBeaconCtrl.Location = New System.Drawing.Point(1028, 498)
        Me.BarBeaconCtrl.Maximum = 2000
        Me.BarBeaconCtrl.Minimum = 100
        Me.BarBeaconCtrl.Name = "BarBeaconCtrl"
        Me.BarBeaconCtrl.Size = New System.Drawing.Size(240, 24)
        Me.BarBeaconCtrl.TabIndex = 17
        Me.BarBeaconCtrl.Value = 100
        '
        'LblAcuMode
        '
        Me.LblAcuMode.Location = New System.Drawing.Point(56, 128)
        Me.LblAcuMode.Name = "LblAcuMode"
        Me.LblAcuMode.Size = New System.Drawing.Size(65, 18)
        Me.LblAcuMode.TabIndex = 9
        Me.LblAcuMode.Text = "ACU MODE"
        '
        'TxtMode_ctrl
        '
        Me.TxtMode_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.TxtMode_ctrl.Location = New System.Drawing.Point(144, 123)
        Me.TxtMode_ctrl.Name = "TxtMode_ctrl"
        Me.TxtMode_ctrl.Size = New System.Drawing.Size(162, 22)
        Me.TxtMode_ctrl.TabIndex = 10
        '
        'LblSatTag
        '
        Me.LblSatTag.Location = New System.Drawing.Point(805, 124)
        Me.LblSatTag.Name = "LblSatTag"
        Me.LblSatTag.Size = New System.Drawing.Size(119, 18)
        Me.LblSatTag.TabIndex = 13
        Me.LblSatTag.Text = "SATELLITE"
        '
        'LblSumAlarm_ctrl
        '
        Me.LblSumAlarm_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.LblSumAlarm_ctrl.ForeColor = System.Drawing.SystemColors.Control
        Me.LblSumAlarm_ctrl.Location = New System.Drawing.Point(1061, 160)
        Me.LblSumAlarm_ctrl.Name = "LblSumAlarm_ctrl"
        Me.LblSumAlarm_ctrl.Size = New System.Drawing.Size(59, 22)
        Me.LblSumAlarm_ctrl.TabIndex = 16
        Me.LblSumAlarm_ctrl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblLev_ctrl
        '
        Me.LblLev_ctrl.BackColor = System.Drawing.Color.Yellow
        Me.LblLev_ctrl.Location = New System.Drawing.Point(1153, 449)
        Me.LblLev_ctrl.Name = "LblLev_ctrl"
        Me.LblLev_ctrl.Size = New System.Drawing.Size(72, 24)
        Me.LblLev_ctrl.TabIndex = 26
        Me.LblLev_ctrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'SS_Intestazione
        '
        Me.SS_Intestazione.BackColor = System.Drawing.Color.Transparent
        Me.SS_Intestazione.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SS_Intestazione.Font = New System.Drawing.Font("Arial", 16.2!, System.Drawing.FontStyle.Bold)
        Me.SS_Intestazione.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.SS_Intestazione.Location = New System.Drawing.Point(485, 58)
        Me.SS_Intestazione.Name = "SS_Intestazione"
        Me.SS_Intestazione.Size = New System.Drawing.Size(339, 35)
        Me.SS_Intestazione.TabIndex = 37
        Me.SS_Intestazione.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblPol1
        '
        Me.LblPol1.BackColor = System.Drawing.Color.Yellow
        Me.LblPol1.Location = New System.Drawing.Point(154, 49)
        Me.LblPol1.Name = "LblPol1"
        Me.LblPol1.Size = New System.Drawing.Size(90, 22)
        Me.LblPol1.TabIndex = 38
        '
        'TxtPolErr
        '
        Me.TxtPolErr.BackColor = System.Drawing.Color.Yellow
        Me.TxtPolErr.Location = New System.Drawing.Point(329, 10)
        Me.TxtPolErr.Name = "TxtPolErr"
        Me.TxtPolErr.Size = New System.Drawing.Size(56, 22)
        Me.TxtPolErr.TabIndex = 39
        Me.TxtPolErr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblPol
        '
        Me.LblPol.BackColor = System.Drawing.Color.Yellow
        Me.LblPol.Location = New System.Drawing.Point(154, 17)
        Me.LblPol.Name = "LblPol"
        Me.LblPol.Size = New System.Drawing.Size(90, 22)
        Me.LblPol.TabIndex = 40
        '
        'TxtPol
        '
        Me.TxtPol.BackColor = System.Drawing.Color.Yellow
        Me.TxtPol.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Bold)
        Me.TxtPol.Location = New System.Drawing.Point(73, 21)
        Me.TxtPol.Name = "TxtPol"
        Me.TxtPol.Size = New System.Drawing.Size(78, 42)
        Me.TxtPol.TabIndex = 41
        Me.TxtPol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtFrqBeacon
        '
        Me.TxtFrqBeacon.BackColor = System.Drawing.Color.Yellow
        Me.TxtFrqBeacon.Location = New System.Drawing.Point(1028, 544)
        Me.TxtFrqBeacon.Name = "TxtFrqBeacon"
        Me.TxtFrqBeacon.Size = New System.Drawing.Size(87, 22)
        Me.TxtFrqBeacon.TabIndex = 43
        Me.TxtFrqBeacon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtPhase
        '
        Me.TxtPhase.BackColor = System.Drawing.Color.Yellow
        Me.TxtPhase.Location = New System.Drawing.Point(768, 600)
        Me.TxtPhase.Name = "TxtPhase"
        Me.TxtPhase.Size = New System.Drawing.Size(51, 22)
        Me.TxtPhase.TabIndex = 45
        Me.TxtPhase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtRevAz
        '
        Me.TxtRevAz.BackColor = System.Drawing.Color.Yellow
        Me.TxtRevAz.Location = New System.Drawing.Point(1048, 601)
        Me.TxtRevAz.Name = "TxtRevAz"
        Me.TxtRevAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtRevAz.Size = New System.Drawing.Size(51, 22)
        Me.TxtRevAz.TabIndex = 47
        '
        'LblLNA
        '
        Me.LblLNA.BackColor = System.Drawing.Color.Yellow
        Me.LblLNA.Location = New System.Drawing.Point(336, 49)
        Me.LblLNA.Name = "LblLNA"
        Me.LblLNA.Size = New System.Drawing.Size(37, 22)
        Me.LblLNA.TabIndex = 48
        '
        'LblLNATag
        '
        Me.LblLNATag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblLNATag.Location = New System.Drawing.Point(274, 55)
        Me.LblLNATag.Name = "LblLNATag"
        Me.LblLNATag.Size = New System.Drawing.Size(40, 16)
        Me.LblLNATag.TabIndex = 0
        Me.LblLNATag.Text = "LNA"
        '
        'LblPolErrTag
        '
        Me.LblPolErrTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblPolErrTag.Location = New System.Drawing.Point(245, 14)
        Me.LblPolErrTag.Name = "LblPolErrTag"
        Me.LblPolErrTag.Size = New System.Drawing.Size(75, 22)
        Me.LblPolErrTag.TabIndex = 0
        Me.LblPolErrTag.Text = "Pol.Err."
        Me.LblPolErrTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'PnlPol
        '
        Me.PnlPol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PnlPol.Controls.Add(Me.LblPol1)
        Me.PnlPol.Controls.Add(Me.TxtPolErr)
        Me.PnlPol.Controls.Add(Me.LblPolErrTag)
        Me.PnlPol.Controls.Add(Me.LblPol)
        Me.PnlPol.Controls.Add(Me.TxtPol)
        Me.PnlPol.Controls.Add(Me.LblLNA)
        Me.PnlPol.Controls.Add(Me.LblLNATag)
        Me.PnlPol.Controls.Add(Me.CmdPol1)
        Me.PnlPol.Location = New System.Drawing.Point(20, 331)
        Me.PnlPol.Name = "PnlPol"
        Me.PnlPol.Size = New System.Drawing.Size(417, 90)
        Me.PnlPol.TabIndex = 0
        '
        'CmdPol1
        '
        Me.CmdPol1.Location = New System.Drawing.Point(6, 32)
        Me.CmdPol1.Name = "CmdPol1"
        Me.CmdPol1.Size = New System.Drawing.Size(66, 23)
        Me.CmdPol1.TabIndex = 0
        Me.CmdPol1.Text = "POL"
        '
        'LblBcnLevelTag
        '
        Me.LblBcnLevelTag.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBcnLevelTag.Location = New System.Drawing.Point(1059, 452)
        Me.LblBcnLevelTag.Name = "LblBcnLevelTag"
        Me.LblBcnLevelTag.Size = New System.Drawing.Size(88, 18)
        Me.LblBcnLevelTag.TabIndex = 40
        Me.LblBcnLevelTag.Text = "BCN Level"
        Me.LblBcnLevelTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblBcnLevelUnit
        '
        Me.LblBcnLevelUnit.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBcnLevelUnit.Location = New System.Drawing.Point(1231, 451)
        Me.LblBcnLevelUnit.Name = "LblBcnLevelUnit"
        Me.LblBcnLevelUnit.Size = New System.Drawing.Size(42, 18)
        Me.LblBcnLevelUnit.TabIndex = 41
        Me.LblBcnLevelUnit.Text = "dBm"
        '
        'LblMemParamTag
        '
        Me.LblMemParamTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblMemParamTag.Location = New System.Drawing.Point(901, 407)
        Me.LblMemParamTag.Name = "LblMemParamTag"
        Me.LblMemParamTag.Size = New System.Drawing.Size(67, 18)
        Me.LblMemParamTag.TabIndex = 42
        Me.LblMemParamTag.Text = "Memory: Param."
        '
        'LblMemParamInd
        '
        Me.LblMemParamInd.BackColor = System.Drawing.Color.Yellow
        Me.LblMemParamInd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblMemParamInd.Location = New System.Drawing.Point(975, 404)
        Me.LblMemParamInd.Name = "LblMemParamInd"
        Me.LblMemParamInd.Size = New System.Drawing.Size(8, 22)
        Me.LblMemParamInd.TabIndex = 43
        '
        'LblMemDataTag
        '
        Me.LblMemDataTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblMemDataTag.Location = New System.Drawing.Point(1019, 407)
        Me.LblMemDataTag.Name = "LblMemDataTag"
        Me.LblMemDataTag.Size = New System.Drawing.Size(35, 18)
        Me.LblMemDataTag.TabIndex = 44
        Me.LblMemDataTag.Text = "Data"
        Me.LblMemDataTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblMemDataInd
        '
        Me.LblMemDataInd.BackColor = System.Drawing.Color.Yellow
        Me.LblMemDataInd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblMemDataInd.Location = New System.Drawing.Point(1064, 406)
        Me.LblMemDataInd.Name = "LblMemDataInd"
        Me.LblMemDataInd.Size = New System.Drawing.Size(8, 22)
        Me.LblMemDataInd.TabIndex = 45
        '
        'LblSatDataTag
        '
        Me.LblSatDataTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblSatDataTag.Location = New System.Drawing.Point(1083, 408)
        Me.LblSatDataTag.Name = "LblSatDataTag"
        Me.LblSatDataTag.Size = New System.Drawing.Size(56, 18)
        Me.LblSatDataTag.TabIndex = 46
        Me.LblSatDataTag.Text = "Sat.Data"
        '
        'LblSatDataInd
        '
        Me.LblSatDataInd.BackColor = System.Drawing.Color.Yellow
        Me.LblSatDataInd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblSatDataInd.Location = New System.Drawing.Point(1143, 406)
        Me.LblSatDataInd.Name = "LblSatDataInd"
        Me.LblSatDataInd.Size = New System.Drawing.Size(8, 22)
        Me.LblSatDataInd.TabIndex = 47
        '
        'LblStowUnstowTag
        '
        Me.LblStowUnstowTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblStowUnstowTag.Location = New System.Drawing.Point(1157, 408)
        Me.LblStowUnstowTag.Name = "LblStowUnstowTag"
        Me.LblStowUnstowTag.Size = New System.Drawing.Size(93, 18)
        Me.LblStowUnstowTag.TabIndex = 48
        Me.LblStowUnstowTag.Text = "Stow/Unstow"
        '
        'LblStowUnstowInd
        '
        Me.LblStowUnstowInd.BackColor = System.Drawing.Color.Yellow
        Me.LblStowUnstowInd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblStowUnstowInd.Location = New System.Drawing.Point(1253, 408)
        Me.LblStowUnstowInd.Name = "LblStowUnstowInd"
        Me.LblStowUnstowInd.Size = New System.Drawing.Size(8, 22)
        Me.LblStowUnstowInd.TabIndex = 49
        '
        'LblBcnFreq
        '
        Me.LblBcnFreq.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBcnFreq.Location = New System.Drawing.Point(866, 545)
        Me.LblBcnFreq.Name = "LblBcnFreq"
        Me.LblBcnFreq.Size = New System.Drawing.Size(152, 22)
        Me.LblBcnFreq.TabIndex = 20
        Me.LblBcnFreq.Text = "Beacon Frequency"
        '
        'CmbBcnFreq
        '
        Me.CmbBcnFreq.Location = New System.Drawing.Point(1212, 541)
        Me.CmbBcnFreq.Name = "CmbBcnFreq"
        Me.CmbBcnFreq.Size = New System.Drawing.Size(94, 24)
        Me.CmbBcnFreq.TabIndex = 22
        '
        'LblBcnMax_ctrl
        '
        Me.LblBcnMax_ctrl.Location = New System.Drawing.Point(1272, 501)
        Me.LblBcnMax_ctrl.Name = "LblBcnMax_ctrl"
        Me.LblBcnMax_ctrl.Size = New System.Drawing.Size(34, 22)
        Me.LblBcnMax_ctrl.TabIndex = 19
        Me.LblBcnMax_ctrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblBcnMin_ctrl
        '
        Me.LblBcnMin_ctrl.Location = New System.Drawing.Point(987, 498)
        Me.LblBcnMin_ctrl.Name = "LblBcnMin_ctrl"
        Me.LblBcnMin_ctrl.Size = New System.Drawing.Size(34, 22)
        Me.LblBcnMin_ctrl.TabIndex = 18
        Me.LblBcnMin_ctrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblPcCmd
        '
        Me.LblPcCmd.Location = New System.Drawing.Point(423, 123)
        Me.LblPcCmd.Name = "LblPcCmd"
        Me.LblPcCmd.Size = New System.Drawing.Size(77, 18)
        Me.LblPcCmd.TabIndex = 11
        Me.LblPcCmd.Text = "PC COMMAND"
        '
        'CmdLoad
        '
        Me.CmdLoad.Location = New System.Drawing.Point(1151, 123)
        Me.CmdLoad.Name = "CmdLoad"
        Me.CmdLoad.Size = New System.Drawing.Size(40, 24)
        Me.CmdLoad.TabIndex = 35
        Me.CmdLoad.Text = "Exe"
        '
        'LblConfCNL5in
        '
        Me.LblConfCNL5in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL5in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL5in.Location = New System.Drawing.Point(1033, 245)
        Me.LblConfCNL5in.Name = "LblConfCNL5in"
        Me.LblConfCNL5in.Size = New System.Drawing.Size(43, 29)
        Me.LblConfCNL5in.TabIndex = 36
        Me.LblConfCNL5in.Text = "INT"
        Me.LblConfCNL5in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtCoorOff
        '
        Me.TxtCoorOff.Location = New System.Drawing.Point(415, 746)
        Me.TxtCoorOff.Name = "TxtCoorOff"
        Me.TxtCoorOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtCoorOff.Size = New System.Drawing.Size(44, 22)
        Me.TxtCoorOff.TabIndex = 0
        Me.TxtCoorOff.Visible = False
        '
        'TxtThrEl
        '
        Me.TxtThrEl.Location = New System.Drawing.Point(1174, 713)
        Me.TxtThrEl.Name = "TxtThrEl"
        Me.TxtThrEl.Size = New System.Drawing.Size(51, 22)
        Me.TxtThrEl.TabIndex = 0
        Me.TxtThrEl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtThrAz
        '
        Me.TxtThrAz.Location = New System.Drawing.Point(1174, 680)
        Me.TxtThrAz.Name = "TxtThrAz"
        Me.TxtThrAz.Size = New System.Drawing.Size(51, 22)
        Me.TxtThrAz.TabIndex = 0
        Me.TxtThrAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CmdSound
        '
        Me.CmdSound.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.CmdSound.Location = New System.Drawing.Point(444, 666)
        Me.CmdSound.Name = "CmdSound"
        Me.CmdSound.Size = New System.Drawing.Size(61, 32)
        Me.CmdSound.TabIndex = 0
        Me.CmdSound.Text = "Alarm Ack"
        '
        'CmdReset
        '
        Me.CmdReset.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.CmdReset.Location = New System.Drawing.Point(440, 569)
        Me.CmdReset.Name = "CmdReset"
        Me.CmdReset.Size = New System.Drawing.Size(73, 34)
        Me.CmdReset.TabIndex = 0
        Me.CmdReset.Text = "Clear Table (F8)"
        '
        'LblSwitches
        '
        Me.LblSwitches.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblSwitches.Location = New System.Drawing.Point(580, 761)
        Me.LblSwitches.BackColor = System.Drawing.Color.Yellow
        Me.LblSwitches.Name = "LblSwitches"
        Me.LblSwitches.Size = New System.Drawing.Size(155, 18)
        Me.LblSwitches.TabIndex = 0
        Me.LblSwitches.Visible = False
        '
        'LblSwitchesTag
        '
        Me.LblSwitchesTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblSwitchesTag.Location = New System.Drawing.Point(424, 745)
        Me.LblSwitchesTag.Name = "LblSwitchesTag"
        Me.LblSwitchesTag.Size = New System.Drawing.Size(150, 18)
        Me.LblSwitchesTag.TabIndex = 0
        Me.LblSwitchesTag.Text = "Preset switches"
        Me.LblSwitchesTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblSwitchesTag.Visible = False
        '
        'LblBackUp
        '
        Me.LblBackUp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblBackUp.Location = New System.Drawing.Point(1000, 746)
        Me.LblBackUp.BackColor = System.Drawing.Color.Yellow
        Me.LblBackUp.Name = "LblBackUp"
        Me.LblBackUp.Size = New System.Drawing.Size(155, 18)
        Me.LblBackUp.TabIndex = 0
        '
        'LblBackUpTag
        '
        Me.LblBackUpTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblBackUpTag.Location = New System.Drawing.Point(805, 746)
        Me.LblBackUpTag.Name = "LblBackUpTag"
        Me.LblBackUpTag.Size = New System.Drawing.Size(190, 18)
        Me.LblBackUpTag.TabIndex = 0
        Me.LblBackUpTag.Text = "Back-Up AutoTrack"
        Me.LblBackUpTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblRisposta
        '
        Me.LblRisposta.BackColor = System.Drawing.Color.Yellow
        Me.LblRisposta.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblRisposta.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblRisposta.Location = New System.Drawing.Point(413, 709)
        Me.LblRisposta.Name = "LblRisposta"
        Me.LblRisposta.Size = New System.Drawing.Size(56, 22)
        Me.LblRisposta.TabIndex = 0
        '
        'LblErrAz
        '
        Me.LblErrAz.Location = New System.Drawing.Point(946, 490)
        Me.LblErrAz.Name = "LblErrAz"
        Me.LblErrAz.Size = New System.Drawing.Size(25, 20)
        Me.LblErrAz.TabIndex = 0
        '
        'LblErrEl
        '
        Me.LblErrEl.Location = New System.Drawing.Point(837, 497)
        Me.LblErrEl.Name = "LblErrEl"
        Me.LblErrEl.Size = New System.Drawing.Size(25, 20)
        Me.LblErrEl.TabIndex = 0
        '
        'LblRVR1
        '
        Me.LblRVR1.Location = New System.Drawing.Point(792, 432)
        Me.LblRVR1.Name = "LblRVR1"
        Me.LblRVR1.Size = New System.Drawing.Size(15, 25)
        Me.LblRVR1.TabIndex = 0
        '
        'LblRVR2
        '
        Me.LblRVR2.Location = New System.Drawing.Point(794, 463)
        Me.LblRVR2.Name = "LblRVR2"
        Me.LblRVR2.Size = New System.Drawing.Size(15, 25)
        Me.LblRVR2.TabIndex = 0
        '
        'LblRVR3
        '
        Me.LblRVR3.Location = New System.Drawing.Point(794, 494)
        Me.LblRVR3.Name = "LblRVR3"
        Me.LblRVR3.Size = New System.Drawing.Size(15, 25)
        Me.LblRVR3.TabIndex = 0
        '
        'LblRVR4
        '
        Me.LblRVR4.Location = New System.Drawing.Point(809, 400)
        Me.LblRVR4.Name = "LblRVR4"
        Me.LblRVR4.Size = New System.Drawing.Size(15, 25)
        Me.LblRVR4.TabIndex = 0
        '
        'LblRVR6
        '
        Me.LblRVR6.Location = New System.Drawing.Point(422, 41)
        Me.LblRVR6.Name = "LblRVR6"
        Me.LblRVR6.Size = New System.Drawing.Size(15, 25)
        Me.LblRVR6.TabIndex = 0
        '
        'LblInputBuffer2
        '
        Me.LblInputBuffer2.Location = New System.Drawing.Point(773, 497)
        Me.LblInputBuffer2.Name = "LblInputBuffer2"
        Me.LblInputBuffer2.Size = New System.Drawing.Size(15, 25)
        Me.LblInputBuffer2.TabIndex = 0
        '
        'LblInputBuffer3
        '
        Me.LblInputBuffer3.Location = New System.Drawing.Point(775, 463)
        Me.LblInputBuffer3.Name = "LblInputBuffer3"
        Me.LblInputBuffer3.Size = New System.Drawing.Size(15, 25)
        Me.LblInputBuffer3.TabIndex = 0
        '
        'LblInputBuffer4
        '
        Me.LblInputBuffer4.Location = New System.Drawing.Point(790, 400)
        Me.LblInputBuffer4.Name = "LblInputBuffer4"
        Me.LblInputBuffer4.Size = New System.Drawing.Size(15, 25)
        Me.LblInputBuffer4.TabIndex = 0
        '
        'LblInputBuffer5
        '
        Me.LblInputBuffer5.Location = New System.Drawing.Point(811, 466)
        Me.LblInputBuffer5.Name = "LblInputBuffer5"
        Me.LblInputBuffer5.Size = New System.Drawing.Size(15, 25)
        Me.LblInputBuffer5.TabIndex = 0
        '
        'TxtOffsetMax
        '
        Me.TxtOffsetMax.Location = New System.Drawing.Point(1000, 713)
        Me.TxtOffsetMax.Name = "TxtOffsetMax"
        Me.TxtOffsetMax.Size = New System.Drawing.Size(78, 22)
        Me.TxtOffsetMax.TabIndex = 0
        Me.TxtOffsetMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblOffsetMaxTag
        '
        Me.LblOffsetMaxTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblOffsetMaxTag.Location = New System.Drawing.Point(885, 713)
        Me.LblOffsetMaxTag.Name = "LblOffsetMaxTag"
        Me.LblOffsetMaxTag.Size = New System.Drawing.Size(110, 22)
        Me.LblOffsetMaxTag.TabIndex = 0
        Me.LblOffsetMaxTag.Text = "Offset Max"
        Me.LblOffsetMaxTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TxtSogliaAuto
        '
        Me.TxtSogliaAuto.Location = New System.Drawing.Point(1000, 680)
        Me.TxtSogliaAuto.Name = "TxtSogliaAuto"
        Me.TxtSogliaAuto.Size = New System.Drawing.Size(75, 22)
        Me.TxtSogliaAuto.TabIndex = 0
        Me.TxtSogliaAuto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblThrAutoTag
        '
        Me.LblThrAutoTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblThrAutoTag.Location = New System.Drawing.Point(825, 680)
        Me.LblThrAutoTag.Name = "LblThrAutoTag"
        Me.LblThrAutoTag.Size = New System.Drawing.Size(170, 22)
        Me.LblThrAutoTag.TabIndex = 0
        Me.LblThrAutoTag.Text = "Augum/Auto Track Thr."
        Me.LblThrAutoTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblThrAzTag
        '
        Me.LblThrAzTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblThrAzTag.Location = New System.Drawing.Point(1081, 680)
        Me.LblThrAzTag.Name = "LblThrAzTag"
        Me.LblThrAzTag.Size = New System.Drawing.Size(88, 22)
        Me.LblThrAzTag.TabIndex = 0
        Me.LblThrAzTag.Text = "Az Error Thr."
        Me.LblThrAzTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblThrElTag
        '
        Me.LblThrElTag.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold)
        Me.LblThrElTag.Location = New System.Drawing.Point(1084, 713)
        Me.LblThrElTag.Name = "LblThrElTag"
        Me.LblThrElTag.Size = New System.Drawing.Size(85, 22)
        Me.LblThrElTag.TabIndex = 0
        Me.LblThrElTag.Text = "El Error Thr."
        Me.LblThrElTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'picLogoLeft
        '
        Me.picLogoLeft.BackColor = System.Drawing.Color.WhiteSmoke
        Me.picLogoLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picLogoLeft.Location = New System.Drawing.Point(12, 63)
        Me.picLogoLeft.Name = "picLogoLeft"
        Me.picLogoLeft.Size = New System.Drawing.Size(302, 54)
        Me.picLogoLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogoLeft.TabIndex = 0
        Me.picLogoLeft.TabStop = False
        '
        'picLogoRight
        '
        Me.picLogoRight.BackColor = System.Drawing.Color.WhiteSmoke
        Me.picLogoRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picLogoRight.Location = New System.Drawing.Point(1028, 63)
        Me.picLogoRight.Name = "picLogoRight"
        Me.picLogoRight.Size = New System.Drawing.Size(213, 52)
        Me.picLogoRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogoRight.TabIndex = 0
        Me.picLogoRight.TabStop = False
        '
        'tdbLog
        '
        Me.tdbLog.AllowUserToAddRows = False
        Me.tdbLog.AllowUserToDeleteRows = False
        Me.tdbLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tdbLog.GridColor = System.Drawing.SystemColors.ButtonHighlight
        Me.tdbLog.Location = New System.Drawing.Point(21, 568)
        Me.tdbLog.Name = "tdbLog"
        Me.tdbLog.ReadOnly = True
        Me.tdbLog.RowHeadersVisible = False
        Me.tdbLog.RowHeadersWidth = 51
        Me.tdbLog.Size = New System.Drawing.Size(384, 130)
        Me.tdbLog.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(1135, 544)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 16)
        Me.Label1.TabIndex = 50
        Me.Label1.Text = "Mhz"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(978, 607)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 16)
        Me.Label2.TabIndex = 51
        Me.Label2.Text = "Rev. Az."
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(693, 603)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(69, 16)
        Me.Label3.TabIndex = 52
        Me.Label3.Text = "Phase El"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(932, 205)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(57, 16)
        Me.Label7.TabIndex = 54
        Me.Label7.Text = "E. Stop"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(825, 605)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(72, 16)
        Me.Label6.TabIndex = 62
        Me.Label6.Text = "Phase Az"
        '
        'TxtPhasAz
        '
        Me.TxtPhasAz.BackColor = System.Drawing.Color.Yellow
        Me.TxtPhasAz.Location = New System.Drawing.Point(900, 601)
        Me.TxtPhasAz.Name = "TxtPhasAz"
        Me.TxtPhasAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPhasAz.Size = New System.Drawing.Size(81, 22)
        Me.TxtPhasAz.TabIndex = 63
        '
        'FrmRicezione
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1339, 788)
        Me.ControlBox = False
        Me.Controls.Add(Me.TxtPhasAz)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LblWarning)
        Me.Controls.Add(Me.LblConfCNL5in)
        Me.Controls.Add(Me.MenuStrip2)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.pnlHeaders)
        Me.Controls.Add(Me.pnlAzimuth)
        Me.Controls.Add(Me.pnlElevazione)
        Me.Controls.Add(Me.pnlComandi)
        Me.Controls.Add(Me.pnlAcuStatus)
        Me.Controls.Add(Me.pnlReceiver)
        Me.Controls.Add(Me.LblAcuMode)
        Me.Controls.Add(Me.TxtMode_ctrl)
        Me.Controls.Add(Me.LblPcCmd)
        Me.Controls.Add(Me.LblCommand)
        Me.Controls.Add(Me.LblSatTag)
        Me.Controls.Add(Me.LblSatellite)
        Me.Controls.Add(Me.LblSumAlarm_ctrl)
        Me.Controls.Add(Me.BarBeaconCtrl)
        Me.Controls.Add(Me.LblBcnMin_ctrl)
        Me.Controls.Add(Me.LblBcnMax_ctrl)
        Me.Controls.Add(Me.LblBcnFreq)
        Me.Controls.Add(Me.CmbBcnFreq)
        Me.Controls.Add(Me.CmdPhase)
        Me.Controls.Add(Me.LblPhase)
        Me.Controls.Add(Me.LblLev_ctrl)
        Me.Controls.Add(Me.SS_Intestazione)
        Me.Controls.Add(Me.TxtFrqBeacon)
        Me.Controls.Add(Me.TxtPhase)
        Me.Controls.Add(Me.TxtRevAz)
        Me.Controls.Add(Me.PnlPol)
        Me.Controls.Add(Me.LblBcnLevelTag)
        Me.Controls.Add(Me.LblBcnLevelUnit)
        Me.Controls.Add(Me.LblMemParamTag)
        Me.Controls.Add(Me.LblMemParamInd)
        Me.Controls.Add(Me.LblMemDataTag)
        Me.Controls.Add(Me.LblMemDataInd)
        Me.Controls.Add(Me.LblSatDataTag)
        Me.Controls.Add(Me.LblSatDataInd)
        Me.Controls.Add(Me.LblStowUnstowTag)
        Me.Controls.Add(Me.LblStowUnstowInd)
        Me.Controls.Add(Me.LblConfCNL1in)
        Me.Controls.Add(Me.LblConfCNL2in)
        Me.Controls.Add(Me.LblConfCNL3in)
        Me.Controls.Add(Me.LblConfCNL4in)
        Me.Controls.Add(Me.LblConfCNL6in)
        Me.Controls.Add(Me.Manager1)
        Me.Controls.Add(Me.CmdSum)
        Me.Controls.Add(Me.LblEmergency)
        Me.Controls.Add(Me.LblEmergency1)
        Me.Controls.Add(Me.TxtOrdini)
        Me.Controls.Add(Me.CmdLoad)
        Me.Controls.Add(Me.TxtCoorOff)
        Me.Controls.Add(Me.TxtThrEl)
        Me.Controls.Add(Me.LblThrElTag)
        Me.Controls.Add(Me.TxtThrAz)
        Me.Controls.Add(Me.LblThrAzTag)
        Me.Controls.Add(Me.CmdSound)
        Me.Controls.Add(Me.CmdReset)
        Me.Controls.Add(Me.LblSwitches)
        Me.Controls.Add(Me.LblSwitchesTag)
        Me.Controls.Add(Me.LblBackUp)
        Me.Controls.Add(Me.LblBackUpTag)
        Me.Controls.Add(Me.LblRisposta)
        Me.Controls.Add(Me.LblErrAz)
        Me.Controls.Add(Me.LblErrEl)
        Me.Controls.Add(Me.LblRVR1)
        Me.Controls.Add(Me.LblRVR2)
        Me.Controls.Add(Me.LblRVR3)
        Me.Controls.Add(Me.LblRVR4)
        Me.Controls.Add(Me.LblRVR6)
        Me.Controls.Add(Me.LblInputBuffer2)
        Me.Controls.Add(Me.LblInputBuffer3)
        Me.Controls.Add(Me.LblInputBuffer4)
        Me.Controls.Add(Me.LblInputBuffer5)
        Me.Controls.Add(Me.TxtOffsetMax)
        Me.Controls.Add(Me.LblOffsetMaxTag)
        Me.Controls.Add(Me.TxtSogliaAuto)
        Me.Controls.Add(Me.LblThrAutoTag)
        Me.Controls.Add(Me.tdbLog)
        Me.Controls.Add(Me.picLogoLeft)
        Me.Controls.Add(Me.picLogoRight)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FrmRicezione"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.pnlHeaders.ResumeLayout(False)
        Me.pnlAzimuth.ResumeLayout(False)
        Me.pnlAzimuth.PerformLayout()
        Me.pnlElevazione.ResumeLayout(False)
        Me.pnlElevazione.PerformLayout()
        Me.pnlComandi.ResumeLayout(False)
        Me.pnlAcuStatus.ResumeLayout(False)
        Me.pnlAcuStatus.PerformLayout()
        Me.pnlReceiver.ResumeLayout(False)
        Me.pnlReceiver.PerformLayout()
        CType(Me.Manager1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PnlPol.ResumeLayout(False)
        Me.PnlPol.PerformLayout()
        CType(Me.picLogoLeft, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLogoRight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tdbLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    ' ── Controls with WithEvents (Handles clauses) ────────────
    Friend WithEvents CmdOffset As System.Windows.Forms.Button
    Friend WithEvents CmdSTBY As System.Windows.Forms.Button
    Friend WithEvents CmdProgramTrack As System.Windows.Forms.Button
    Friend WithEvents CmdAugmented As System.Windows.Forms.Button
    Friend WithEvents CmdPhase As System.Windows.Forms.Button
    Friend WithEvents TmrRic As System.Windows.Forms.Timer
    Friend WithEvents TmrSpiralAcquire As System.Windows.Forms.Timer
    ' ── Label controls (stato antenna) ───────────────────────
    Friend LblAzimuth As System.Windows.Forms.Label
    Friend LblElevazione As System.Windows.Forms.Label
    Friend LblAzimuthC As System.Windows.Forms.Label
    Friend LblElevazioneC As System.Windows.Forms.Label
    Friend LblAzError As System.Windows.Forms.Label
    Friend LblElError As System.Windows.Forms.Label
    Friend LblAzErrEn As System.Windows.Forms.Label
    Friend LblElErrEn As System.Windows.Forms.Label
    Friend LblCommand As System.Windows.Forms.Label
    Friend LblStepTrack As System.Windows.Forms.Label
    Friend LblLock As System.Windows.Forms.Label
    Friend LblPhase As System.Windows.Forms.Label
    ' ── Stato canali ─────────────────────────────────────────
    Friend LblConfCNL1in As System.Windows.Forms.Label
    Friend LblConfCNL2in As System.Windows.Forms.Label
    Friend LblConfCNL3in As System.Windows.Forms.Label
    Friend LblConfCNL4in As System.Windows.Forms.Label
    Friend WithEvents LblSatellite As System.Windows.Forms.ComboBox
    Friend LblWarning As System.Windows.Forms.Label
    ' ── TextBox ───────────────────────────────────────────────
    Friend TxtAz As System.Windows.Forms.TextBox
    Friend TxtEl As System.Windows.Forms.TextBox
    Friend TxtOrdini As System.Windows.Forms.TextBox
    ' ── Menu top-level (mnuMain[0..19]) ─────────────────────
    Friend WithEvents MnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuUpdateTLE As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuReceiver As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPhase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuBeaconGraphic As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuAntenna As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphemerisMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuDownConverter As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuParameterACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuParameterArchive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSimulation As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuTraceOrbit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphemerisOrbit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuAcquireTrace As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuAlarm As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuURL As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuInfo As System.Windows.Forms.ToolStripMenuItem
    ' ── Sub-menu File ────────────────────────────────────────
    Friend WithEvents MnuConfiguration As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuBell As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuExit As System.Windows.Forms.ToolStripMenuItem
    ' ── Sub-menu ACU ─────────────────────────────────────────
    Friend WithEvents MnuCommand As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStandBy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStepTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemoryTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemInterpStep As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemRecEnable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemRecDisable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuProgramTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSinusoidal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuResetACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteCmd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuUnstow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuBackupPT As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuCommandFree As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuOffsetCmd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuRequest As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuReqStatus As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuReqParameter As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemoryTrackData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStowUnstowData As System.Windows.Forms.ToolStripMenuItem
    ' ── Sub-menu TLE ─────────────────────────────────────────
    Friend WithEvents MnuTLEManual As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuTLETwoLine As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuTLEThreeLine As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuTLETableTrack As System.Windows.Forms.ToolStripMenuItem
    ' ── Sub-menu Ephemeris ────────────────────────────────────
    Friend WithEvents MnuEphCassiopea As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphMoon As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphSatellite As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphSun As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphVenus As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuOrbitCartesian As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuOrbitPolar As System.Windows.Forms.ToolStripMenuItem
    ' MnuMain() array legacy — non più usato
    Friend MnuMain() As System.Windows.Forms.ToolStripMenuItem
    ' ── Container (Frame VB6) ─────────────────────────────────
    Friend pnlHeaders As System.Windows.Forms.Panel
    Friend pnlAzimuth As System.Windows.Forms.Panel
    Friend pnlElevazione As System.Windows.Forms.Panel
    Friend pnlComandi As System.Windows.Forms.Panel
    Friend pnlAcuStatus As System.Windows.Forms.Panel
    Friend pnlReceiver As System.Windows.Forms.Panel
    ' ── Serial ports ─────────────────────────────────────────
    ' SerialPort1..4 rimossi — le porte seriali usano ComPort() in AppState
    ' ── Timers ────────────────────────────────────────────────
    Friend WithEvents tmrAlarm As System.Windows.Forms.Timer
    Friend WithEvents tmrTimeoutMsg1 As System.Windows.Forms.Timer
    Friend WithEvents tmrMsgTimeout2 As System.Windows.Forms.Timer
    Friend WithEvents tmrTimeoutMsg3 As System.Windows.Forms.Timer
    Friend WithEvents tmrTimeoutMsg4 As System.Windows.Forms.Timer
    ' ── Entity connections ────────────────────────────────────
    ' EntityConnection1/2/3 rimossi — artefatti migrazione, non usati
    ' ── Menu strips — DEVONO essere qui per il VS Designer ────
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents MenuStrip2 As System.Windows.Forms.MenuStrip
    Friend WithEvents LblAzTag As Label
    Friend WithEvents TxtAzEr As Label
    Friend WithEvents LblElTag As Label
    Friend WithEvents TxtElEr As Label
    Friend WithEvents CmdCmdPos As Button
    Friend WithEvents CmdStepTrack As Button
    Friend WithEvents CmdAutoTrack As Button
    Friend WithEvents CmdAutoSearch As Button
    Friend WithEvents CmdSpirAcq As Button
    Friend WithEvents CmdProi As Button
    Friend WithEvents LblAcuStatusTag As Label
    Friend WithEvents TxtACU_ctrl As Label
    Friend WithEvents CmdPC1_ctrl As Button
    Friend WithEvents LblPC1_ctrl As Label
    Friend WithEvents CmdLockBell_ctrl As Button
    Friend WithEvents LblPhaseElTag As Label
    Friend WithEvents TxtPhaseEl As Label
    Friend WithEvents LblRevAzTag As Label
    Friend WithEvents LblRevAz_ctrl As Label
    Friend WithEvents LblRFTag As Label
    Friend WithEvents TxtInpFreq As Label
    Friend WithEvents BarBeaconCtrl As ProgressBar
    Friend WithEvents LblAcuMode As Label
    Friend WithEvents TxtMode_ctrl As TextBox
    Friend WithEvents LblSatTag As Label
    Friend WithEvents LblSumAlarm_ctrl As Label
    Friend WithEvents LblLev_ctrl As Label
    Friend WithEvents SS_Intestazione As Label
    Friend WithEvents LblPol1 As Label
    Friend WithEvents TxtPolErr As TextBox
    Friend WithEvents LblPol As Label
    Friend WithEvents TxtPol As TextBox
    Friend WithEvents LblPolRic As Label
    Friend WithEvents TxtFrqBeacon As TextBox
    Friend WithEvents TxtLna As TextBox
    Friend WithEvents TxtPhase As TextBox
    Friend WithEvents TxtPhAz As TextBox
    Friend WithEvents TxtRevAz As TextBox
    Friend WithEvents LblLNA As Label
    Friend LblLNATag As Label
    Friend LblPolErrTag As Label
    Friend PnlPol As System.Windows.Forms.Panel
    Friend LblBcnLevelTag As Label
    Friend LblBcnLevelUnit As Label
    Friend LblMemParamTag As Label
    Friend LblMemParamInd As Label
    Friend LblMemDataTag As Label
    Friend LblMemDataInd As Label
    Friend LblSatDataTag As Label
    Friend LblSatDataInd As Label
    Friend LblStowUnstowTag As Label
    Friend LblStowUnstowInd As Label
    Friend WithEvents LblBcnFreq As Label
    Friend WithEvents CmbBcnFreq As ComboBox
    Friend WithEvents LblBcnMax_ctrl As TextBox
    Friend WithEvents LblBcnMin_ctrl As TextBox
    Friend WithEvents LblPcCmd As Label
    Friend WithEvents CmdLoad As System.Windows.Forms.Button
    Friend WithEvents LblConfCNL5in As Label
    ' LblLNA, LblPol1, TxtPolErr, LblPol, TxtPol, BarBeacon, Datlog, tdbLog
    ' dichiarati in frmRicezione.vb — non ridichiarare qui
    ' ── Controlli aggiunti dalla migrazione VB6 ───────────────
    Friend WithEvents CmdSound As System.Windows.Forms.Button
    Friend WithEvents CmdReset As System.Windows.Forms.Button
    Friend WithEvents CmdPol1 As System.Windows.Forms.Button
    Friend WithEvents chkEl As System.Windows.Forms.CheckBox
    Friend WithEvents chkAz As System.Windows.Forms.CheckBox
    Friend TxtCoorOff As System.Windows.Forms.TextBox
    Friend TxtThrEl As System.Windows.Forms.TextBox
    Friend LblThrElTag As System.Windows.Forms.Label
    Friend TxtThrAz As System.Windows.Forms.TextBox
    Friend LblThrAzTag As System.Windows.Forms.Label
    Friend TxtOffsetMax As System.Windows.Forms.TextBox
    Friend LblOffsetMaxTag As System.Windows.Forms.Label
    Friend TxtSogliaAuto As System.Windows.Forms.TextBox
    Friend LblThrAutoTag As System.Windows.Forms.Label
    Friend LblSwitches As System.Windows.Forms.Label
    Friend LblSwitchesTag As System.Windows.Forms.Label
    Friend LblBackUp As System.Windows.Forms.Label
    Friend LblBackUpTag As System.Windows.Forms.Label
    Friend LblRisposta As System.Windows.Forms.Label
    Friend LblErrAz As System.Windows.Forms.Label
    Friend LblErrEl As System.Windows.Forms.Label
    Friend LblRVR1 As System.Windows.Forms.Label
    Friend LblRVR2 As System.Windows.Forms.Label
    Friend LblRVR3 As System.Windows.Forms.Label
    Friend LblRVR4 As System.Windows.Forms.Label
    Friend LblRVR6 As System.Windows.Forms.Label
    Friend LblInputBuffer2 As System.Windows.Forms.Label
    Friend LblInputBuffer3 As System.Windows.Forms.Label
    Friend LblInputBuffer4 As System.Windows.Forms.Label
    Friend LblInputBuffer5 As System.Windows.Forms.Label
    Friend LblHdrDis As System.Windows.Forms.Label
    Friend LblHdrAntenna As System.Windows.Forms.Label
    Friend LblHdrError As System.Windows.Forms.Label
    Friend LblHdrCommand As System.Windows.Forms.Label
    Friend LblHdrProgram As System.Windows.Forms.Label
    Friend LblHdrOffset As System.Windows.Forms.Label
    Friend LblHdrSwLimit As System.Windows.Forms.Label
    Friend LblHdrBrk As System.Windows.Forms.Label
    Friend LblAllarme4 As System.Windows.Forms.Label
    Friend LblAllarme5 As System.Windows.Forms.Label
    Friend LblAllarme12 As System.Windows.Forms.Label
    Friend LblAllarme13 As System.Windows.Forms.Label
    ' ── Brake / Speed Az/El ──────────────────────────────────
    Friend LblAzTBrk As System.Windows.Forms.Label
    Friend LblElTBrk As System.Windows.Forms.Label
    ' ── Loghi (Image1/Image2 VB6) ────────────────────────────
    Friend picLogoLeft As System.Windows.Forms.PictureBox
    Friend picLogoRight As System.Windows.Forms.PictureBox
    ' ── Grid allarmi (ex TDBGrid VB6) ────────────────────────
    Friend tdbLog As System.Windows.Forms.DataGridView
    Friend colData As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend colMsg As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label7 As Label
    ' ── Controlli specifici FOC6 (canali 6-8, SNMP, Sum.Alarm, CH10, emergency) ──
    ' LblConfCNL5in esiste già (ereditato da BTS1, Text="INT" — canale
    ' INTELSAT, presente ma inutilizzato anche lì).
    Friend LblConfCNL6in As Label
    Friend Manager1 As System.Windows.Forms.PictureBox
    Friend WithEvents CmdSum As System.Windows.Forms.Button
    Friend LblEmergency As Label
    Friend LblEmergency1 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtPhasAz As TextBox
    Friend WithEvents Label10 As Label
End Class
