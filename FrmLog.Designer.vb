Partial Class FrmLog
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

    ' ============================================================
    ' Layout clonato da frmLog.frm (VB6 reale): stesse 4 sezioni/
    ' Frame del form originale (Frame4=griglia AZ/EL, Frame1=riga POL,
    ' Frame6=riga stato M/C-SUM-RCV-BCN + Em.stop/MDU, frmComandi=
    ' pulsanti Mode), posizioni derivate dai twip reali (/15), stesso
    ' ordine orizzontale e stessi colori di frmRicezione/VB6.
    ' ============================================================
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.TmrAggiorna = New System.Windows.Forms.Timer(Me.components)
        Me.TmrAlarm = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuConfiguration = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.MnuPreset = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPresetAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPresetAz = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPresetEl = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPresetPol = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuResetACU = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteCmd = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStow = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuUnstow = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuBackupPT = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCommandFree = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuRequest = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuReqStatus = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuReqParameter = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuMemoryTrackData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuStowUnstowData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuGraphic = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuAntenna = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSatelliteMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuEphemeris = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuParameterTable = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuParameterACU = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuParameterArchive = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPhase = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuInfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.LblServiceBanner = New System.Windows.Forms.Label()
        Me.LblOperationModeTag = New System.Windows.Forms.Label()
        Me.TxtMode = New System.Windows.Forms.TextBox()
        Me.LblPcCommandTag = New System.Windows.Forms.Label()
        Me.LblCommand = New System.Windows.Forms.Label()
        Me.LblSatelliteTag = New System.Windows.Forms.Label()
        Me.LblSatellite = New System.Windows.Forms.Label()
        Me.LblSumAlarmTag = New System.Windows.Forms.Label()
        Me.LblSumAlarm = New System.Windows.Forms.Label()
        Me.PnlGrid = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblHdrAntenna = New System.Windows.Forms.Label()
        Me.LblHdrCommand = New System.Windows.Forms.Label()
        Me.LblHdrOffset = New System.Windows.Forms.Label()
        Me.LblHdrSpeed = New System.Windows.Forms.Label()
        Me.LblHdrPlHw = New System.Windows.Forms.Label()
        Me.LblHdrLimitHw = New System.Windows.Forms.Label()
        Me.LblHdrPlSw = New System.Windows.Forms.Label()
        Me.LblHdrBrk = New System.Windows.Forms.Label()
        Me.LblHdrMajorF = New System.Windows.Forms.Label()
        Me.LblHdrMinorF = New System.Windows.Forms.Label()
        Me.LblAzTag = New System.Windows.Forms.Label()
        Me.TxtAz = New System.Windows.Forms.TextBox()
        Me.LblAzimuth = New System.Windows.Forms.Label()
        Me.LblAzError = New System.Windows.Forms.Label()
        Me.LblAzSpeed = New System.Windows.Forms.Label()
        Me.LblAzSpeedUnit = New System.Windows.Forms.Label()
        Me.LblAllarme1 = New System.Windows.Forms.Label()
        Me.LblAllarme2 = New System.Windows.Forms.Label()
        Me.LblAllarme3 = New System.Windows.Forms.Label()
        Me.LblAllarme4 = New System.Windows.Forms.Label()
        Me.LblAzTBrk = New System.Windows.Forms.Label()
        Me.LblAzMaFault = New System.Windows.Forms.Label()
        Me.LblAzMiFault = New System.Windows.Forms.Label()
        Me.LblElTag = New System.Windows.Forms.Label()
        Me.TxtEl = New System.Windows.Forms.TextBox()
        Me.LblElevazione = New System.Windows.Forms.Label()
        Me.LblElError = New System.Windows.Forms.Label()
        Me.LblElSpeed = New System.Windows.Forms.Label()
        Me.LblElSpeedUnit = New System.Windows.Forms.Label()
        Me.LblAllarme5 = New System.Windows.Forms.Label()
        Me.LblAllarme6 = New System.Windows.Forms.Label()
        Me.LblAllarme7 = New System.Windows.Forms.Label()
        Me.LblAllarme8 = New System.Windows.Forms.Label()
        Me.LblElTBrk = New System.Windows.Forms.Label()
        Me.LblElMaFault = New System.Windows.Forms.Label()
        Me.LblElMiFault = New System.Windows.Forms.Label()
        Me.LblAzStow = New System.Windows.Forms.Label()
        Me.LblAzAligned = New System.Windows.Forms.Label()
        Me.LblElStow = New System.Windows.Forms.Label()
        Me.LblElAligned = New System.Windows.Forms.Label()
        Me.LblElHand = New System.Windows.Forms.Label()
        Me.PnlPol = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblPolTag = New System.Windows.Forms.Label()
        Me.TxtPol = New System.Windows.Forms.Label()
        Me.LblPol = New System.Windows.Forms.Label()
        Me.LblPol1 = New System.Windows.Forms.TextBox()
        Me.LblLNATag = New System.Windows.Forms.Label()
        Me.LblLNA = New System.Windows.Forms.Label()
        Me.TxtPolErr = New System.Windows.Forms.TextBox()
        Me.LblPolFault = New System.Windows.Forms.Label()
        Me.LblAllarme16 = New System.Windows.Forms.Label()
        Me.LblAllarme17 = New System.Windows.Forms.Label()
        Me.LblAllarme18 = New System.Windows.Forms.Label()
        Me.PnlStatus = New System.Windows.Forms.Panel()
        Me.LblMCTag = New System.Windows.Forms.Label()
        Me.TxtACU = New System.Windows.Forms.Label()
        Me.LblSumTag = New System.Windows.Forms.Label()
        Me.TxtLev = New System.Windows.Forms.Label()
        Me.LblVTagSum = New System.Windows.Forms.Label()
        Me.LblAzErTag = New System.Windows.Forms.Label()
        Me.TxtAzEr = New System.Windows.Forms.TextBox()
        Me.LblVTagAzEr = New System.Windows.Forms.Label()
        Me.LblElErTag = New System.Windows.Forms.Label()
        Me.TxtElEr = New System.Windows.Forms.TextBox()
        Me.LblVTagElEr = New System.Windows.Forms.Label()
        Me.LblRcvStatusTag = New System.Windows.Forms.Label()
        Me.LblLock = New System.Windows.Forms.Label()
        Me.LblBcnLevelTag = New System.Windows.Forms.Label()
        Me.LblLev = New System.Windows.Forms.Label()
        Me.LblAllarme11 = New System.Windows.Forms.Label()
        Me.LblAllarme10 = New System.Windows.Forms.Label()
        Me.LblAllarme9 = New System.Windows.Forms.Label()
        Me.LblEmStopTag = New System.Windows.Forms.Label()
        Me.LblEmergency = New System.Windows.Forms.Label()
        Me.LblEmergency1 = New System.Windows.Forms.Label()
        Me.LblMduTag = New System.Windows.Forms.Label()
        Me.LblAzDR = New System.Windows.Forms.Label()
        Me.LblPCU = New System.Windows.Forms.Label()
        Me.LblLowAngleTag = New System.Windows.Forms.Label()
        Me.LblLowAngle = New System.Windows.Forms.Label()
        Me.LblMemoryTag = New System.Windows.Forms.Label()
        Me.LblAllarme12 = New System.Windows.Forms.Label()
        Me.LblMemDataTag = New System.Windows.Forms.Label()
        Me.LblAllarme13 = New System.Windows.Forms.Label()
        Me.LblSatDataTag = New System.Windows.Forms.Label()
        Me.LblAllarme14 = New System.Windows.Forms.Label()
        Me.LblStowUnstowTag = New System.Windows.Forms.Label()
        Me.LblAllarme15 = New System.Windows.Forms.Label()
        Me.LblWarning = New System.Windows.Forms.Label()
        Me.FrmComandi = New System.Windows.Forms.GroupBox()
        Me.CmdSTBY = New System.Windows.Forms.Button()
        Me.CmdCmdPos = New System.Windows.Forms.Button()
        Me.CmdStepTrack = New System.Windows.Forms.Button()
        Me.CmdAutoTrack = New System.Windows.Forms.Button()
        Me.CmdAugmented = New System.Windows.Forms.Button()
        Me.CmdProgramTrack = New System.Windows.Forms.Button()
        Me.CmdOffset = New System.Windows.Forms.Button()
        Me.CmdAutoSearch = New System.Windows.Forms.Button()
        Me.LblStepTrack = New System.Windows.Forms.Label()
        Me.LblFrqBeaconTag = New System.Windows.Forms.Label()
        Me.TxtFreqBeacon = New System.Windows.Forms.TextBox()
        Me.LblMHzTag1 = New System.Windows.Forms.Label()
        Me.LblAzErrThrTag = New System.Windows.Forms.Label()
        Me.TxtThrAz = New System.Windows.Forms.TextBox()
        Me.LblAutoTrackThrTag = New System.Windows.Forms.Label()
        Me.TxtSogliaAuto = New System.Windows.Forms.TextBox()
        Me.LblDbmTag1 = New System.Windows.Forms.Label()
        Me.LblElErrThrTag = New System.Windows.Forms.Label()
        Me.TxtThrEl = New System.Windows.Forms.TextBox()
        Me.CmdReset = New System.Windows.Forms.Button()
        Me.LblOffsetMaxTag = New System.Windows.Forms.Label()
        Me.TxtOffsetMax = New System.Windows.Forms.TextBox()
        Me.LblGradiTag = New System.Windows.Forms.Label()
        Me.LblBackupAutoTrackTag = New System.Windows.Forms.Label()
        Me.LblBackUp = New System.Windows.Forms.Label()
        Me.LblPresetSwitchesTag = New System.Windows.Forms.Label()
        Me.LblSwitches = New System.Windows.Forms.Label()
        Me.TxtFrequency = New System.Windows.Forms.TextBox()
        Me.TxtPowerOut = New System.Windows.Forms.TextBox()
        Me.TdbLog = New System.Windows.Forms.DataGridView()
        Me.LblConfCNL1in = New System.Windows.Forms.Label()
        Me.LblConfCNL3in = New System.Windows.Forms.Label()
        Me.LblConfCNL4in = New System.Windows.Forms.Label()
        Me.LblConfCNL6in = New System.Windows.Forms.Label()
        Me.LblConfCNL2in = New System.Windows.Forms.Label()
        Me.LblConfCNL5in = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.MenuStrip1.SuspendLayout()
        Me.PnlGrid.SuspendLayout()
        Me.PnlPol.SuspendLayout()
        Me.PnlStatus.SuspendLayout()
        Me.FrmComandi.SuspendLayout()
        CType(Me.TdbLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TmrAggiorna
        '
        Me.TmrAggiorna.Enabled = True
        Me.TmrAggiorna.Interval = 1000
        '
        'TmrAlarm
        '
        Me.TmrAlarm.Interval = 2000
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuFile, Me.MnuACU, Me.MnuGraphic, Me.MnuAntenna, Me.MnuSatelliteMenu, Me.MnuEphemeris, Me.MnuParameterTable, Me.MnuParameterACU, Me.MnuParameterArchive, Me.MnuPhase, Me.MnuInfo})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1339, 28)
        Me.MenuStrip1.TabIndex = 0
        '
        'MnuFile
        '
        Me.MnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuConfiguration, Me.MnuExit})
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
        Me.MnuCommand.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuStandBy, Me.MnuStepTrack, Me.MnuMemoryTrack, Me.MnuMemInterpStep, Me.MnuMemRecEnable, Me.MnuMemRecDisable, Me.MnuProgramTrack, Me.MnuSinusoidal, Me.MnuPreset, Me.MnuResetACU, Me.MnuSatelliteCmd, Me.MnuStow, Me.MnuUnstow, Me.MnuBackupPT, Me.MnuCommandFree})
        Me.MnuCommand.Name = "MnuCommand"
        Me.MnuCommand.Size = New System.Drawing.Size(161, 26)
        Me.MnuCommand.Text = "Command"
        '
        'MnuStandBy
        '
        Me.MnuStandBy.Name = "MnuStandBy"
        Me.MnuStandBy.Size = New System.Drawing.Size(309, 26)
        Me.MnuStandBy.Text = "Stand By"
        '
        'MnuStepTrack
        '
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
        Me.MnuMemRecEnable.Text = "Memory Recording - Execute "
        '
        'MnuMemRecDisable
        '
        Me.MnuMemRecDisable.Name = "MnuMemRecDisable"
        Me.MnuMemRecDisable.Size = New System.Drawing.Size(309, 26)
        Me.MnuMemRecDisable.Text = "Memory Recording - Abort "
        '
        'MnuProgramTrack
        '
        Me.MnuProgramTrack.Enabled = False
        Me.MnuProgramTrack.Name = "MnuProgramTrack"
        Me.MnuProgramTrack.Size = New System.Drawing.Size(309, 26)
        Me.MnuProgramTrack.Text = "Program Track"
        Me.MnuProgramTrack.Visible = False
        '
        'MnuSinusoidal
        '
        Me.MnuSinusoidal.Enabled = False
        Me.MnuSinusoidal.Name = "MnuSinusoidal"
        Me.MnuSinusoidal.Size = New System.Drawing.Size(309, 26)
        Me.MnuSinusoidal.Text = "Sinusoidal Regression"
        Me.MnuSinusoidal.Visible = False
        '
        'MnuPreset
        '
        Me.MnuPreset.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuPresetAll, Me.MnuPresetAz, Me.MnuPresetEl, Me.MnuPresetPol})
        Me.MnuPreset.Name = "MnuPreset"
        Me.MnuPreset.Size = New System.Drawing.Size(309, 26)
        Me.MnuPreset.Text = "Preset"
        '
        'MnuPresetAll
        '
        Me.MnuPresetAll.Name = "MnuPresetAll"
        Me.MnuPresetAll.Size = New System.Drawing.Size(170, 26)
        Me.MnuPresetAll.Text = "All Axis"
        '
        'MnuPresetAz
        '
        Me.MnuPresetAz.Name = "MnuPresetAz"
        Me.MnuPresetAz.Size = New System.Drawing.Size(170, 26)
        Me.MnuPresetAz.Text = "Azimuth"
        '
        'MnuPresetEl
        '
        Me.MnuPresetEl.Name = "MnuPresetEl"
        Me.MnuPresetEl.Size = New System.Drawing.Size(170, 26)
        Me.MnuPresetEl.Text = "Elevation"
        '
        'MnuPresetPol
        '
        Me.MnuPresetPol.Enabled = False
        Me.MnuPresetPol.Name = "MnuPresetPol"
        Me.MnuPresetPol.Size = New System.Drawing.Size(170, 26)
        Me.MnuPresetPol.Text = "Polarization"
        Me.MnuPresetPol.Visible = False   ' VB6 K09: visibile ma senza effetto (frmPreset non gestisce i_Assi = 3)
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
        Me.MnuCommandFree.Visible = False
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
        Me.MnuReqStatus.Size = New System.Drawing.Size(217, 26)
        Me.MnuReqStatus.Text = "Status"
        Me.MnuReqStatus.Visible = False
        '
        'MnuReqParameter
        '
        Me.MnuReqParameter.Enabled = False
        Me.MnuReqParameter.Name = "MnuReqParameter"
        Me.MnuReqParameter.Size = New System.Drawing.Size(217, 26)
        Me.MnuReqParameter.Text = "Parameter"
        Me.MnuReqParameter.Visible = False
        '
        'MnuSatelliteData
        '
        Me.MnuSatelliteData.Name = "MnuSatelliteData"
        Me.MnuSatelliteData.Size = New System.Drawing.Size(217, 26)
        Me.MnuSatelliteData.Text = "Satellite data"
        '
        'MnuMemoryTrackData
        '
        Me.MnuMemoryTrackData.Name = "MnuMemoryTrackData"
        Me.MnuMemoryTrackData.Size = New System.Drawing.Size(217, 26)
        Me.MnuMemoryTrackData.Text = "Memory track data"
        '
        'MnuStowUnstowData
        '
        Me.MnuStowUnstowData.Enabled = False
        Me.MnuStowUnstowData.Name = "MnuStowUnstowData"
        Me.MnuStowUnstowData.Size = New System.Drawing.Size(217, 26)
        Me.MnuStowUnstowData.Text = "Stow/Unstow data"
        Me.MnuStowUnstowData.Visible = False
        '
        'MnuGraphic
        '
        Me.MnuGraphic.Enabled = False
        Me.MnuGraphic.Name = "MnuGraphic"
        Me.MnuGraphic.Size = New System.Drawing.Size(74, 24)
        Me.MnuGraphic.Text = "Graphic"
        Me.MnuGraphic.Visible = False
        '
        'MnuAntenna
        '
        Me.MnuAntenna.Name = "MnuAntenna"
        Me.MnuAntenna.Size = New System.Drawing.Size(78, 24)
        Me.MnuAntenna.Text = "Antenne"
        '
        'MnuSatelliteMenu
        '
        Me.MnuSatelliteMenu.Name = "MnuSatelliteMenu"
        Me.MnuSatelliteMenu.Size = New System.Drawing.Size(77, 24)
        Me.MnuSatelliteMenu.Text = "Satellite"
        '
        'MnuEphemeris
        '
        Me.MnuEphemeris.Name = "MnuEphemeris"
        Me.MnuEphemeris.Size = New System.Drawing.Size(92, 24)
        Me.MnuEphemeris.Text = "Ephemeris"
        '
        'MnuParameterTable
        '
        Me.MnuParameterTable.Enabled = False
        Me.MnuParameterTable.Name = "MnuParameterTable"
        Me.MnuParameterTable.Size = New System.Drawing.Size(129, 24)
        Me.MnuParameterTable.Text = "Parameter Table"
        Me.MnuParameterTable.Visible = False
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
        'MnuPhase
        '
        Me.MnuPhase.Name = "MnuPhase"
        Me.MnuPhase.Size = New System.Drawing.Size(61, 24)
        Me.MnuPhase.Text = "Phase"
        '
        'MnuInfo
        '
        Me.MnuInfo.Name = "MnuInfo"
        Me.MnuInfo.Size = New System.Drawing.Size(30, 24)
        Me.MnuInfo.Text = "?"
        '
        'LblServiceBanner
        '
        Me.LblServiceBanner.BackColor = System.Drawing.SystemColors.Control
        Me.LblServiceBanner.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblServiceBanner.Font = New System.Drawing.Font("Arial", 16.0!, System.Drawing.FontStyle.Bold)
        Me.LblServiceBanner.ForeColor = System.Drawing.Color.DarkBlue
        Me.LblServiceBanner.Location = New System.Drawing.Point(571, 38)
        Me.LblServiceBanner.Name = "LblServiceBanner"
        Me.LblServiceBanner.Size = New System.Drawing.Size(205, 33)
        Me.LblServiceBanner.TabIndex = 0
        Me.LblServiceBanner.Text = "SERVICE"
        Me.LblServiceBanner.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOperationModeTag
        '
        Me.LblOperationModeTag.AutoSize = True
        Me.LblOperationModeTag.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LblOperationModeTag.Location = New System.Drawing.Point(19, 86)
        Me.LblOperationModeTag.Name = "LblOperationModeTag"
        Me.LblOperationModeTag.Size = New System.Drawing.Size(134, 16)
        Me.LblOperationModeTag.TabIndex = 1
        Me.LblOperationModeTag.Text = "OPERATION MODE"
        '
        'TxtMode
        '
        Me.TxtMode.BackColor = System.Drawing.Color.Yellow
        Me.TxtMode.Location = New System.Drawing.Point(151, 83)
        Me.TxtMode.Name = "TxtMode"
        Me.TxtMode.ReadOnly = True
        Me.TxtMode.Size = New System.Drawing.Size(162, 22)
        Me.TxtMode.TabIndex = 2
        '
        'LblPcCommandTag
        '
        Me.LblPcCommandTag.AutoSize = True
        Me.LblPcCommandTag.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LblPcCommandTag.Location = New System.Drawing.Point(443, 93)
        Me.LblPcCommandTag.Name = "LblPcCommandTag"
        Me.LblPcCommandTag.Size = New System.Drawing.Size(106, 16)
        Me.LblPcCommandTag.TabIndex = 3
        Me.LblPcCommandTag.Text = "PC COMMAND"
        '
        'LblCommand
        '
        Me.LblCommand.BackColor = System.Drawing.Color.Yellow
        Me.LblCommand.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblCommand.Location = New System.Drawing.Point(576, 90)
        Me.LblCommand.Name = "LblCommand"
        Me.LblCommand.Size = New System.Drawing.Size(205, 22)
        Me.LblCommand.TabIndex = 4
        Me.LblCommand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblSatelliteTag
        '
        Me.LblSatelliteTag.AutoSize = True
        Me.LblSatelliteTag.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LblSatelliteTag.Location = New System.Drawing.Point(992, 86)
        Me.LblSatelliteTag.Name = "LblSatelliteTag"
        Me.LblSatelliteTag.Size = New System.Drawing.Size(82, 16)
        Me.LblSatelliteTag.TabIndex = 5
        Me.LblSatelliteTag.Text = "SATELLITE:"
        '
        'LblSatellite
        '
        Me.LblSatellite.BackColor = System.Drawing.Color.Yellow
        Me.LblSatellite.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblSatellite.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold)
        Me.LblSatellite.ForeColor = System.Drawing.Color.Green
        Me.LblSatellite.Location = New System.Drawing.Point(1089, 83)
        Me.LblSatellite.Name = "LblSatellite"
        Me.LblSatellite.Size = New System.Drawing.Size(243, 22)
        Me.LblSatellite.TabIndex = 6
        Me.LblSatellite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblSumAlarmTag
        '
        Me.LblSumAlarmTag.AutoSize = True
        Me.LblSumAlarmTag.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblSumAlarmTag.Location = New System.Drawing.Point(1103, 38)
        Me.LblSumAlarmTag.Name = "LblSumAlarmTag"
        Me.LblSumAlarmTag.Size = New System.Drawing.Size(101, 18)
        Me.LblSumAlarmTag.TabIndex = 7
        Me.LblSumAlarmTag.Text = "SUM.ALARM:"
        '
        'LblSumAlarm
        '
        Me.LblSumAlarm.BackColor = System.Drawing.Color.Yellow
        Me.LblSumAlarm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblSumAlarm.Location = New System.Drawing.Point(1222, 34)
        Me.LblSumAlarm.Name = "LblSumAlarm"
        Me.LblSumAlarm.Size = New System.Drawing.Size(18, 22)
        Me.LblSumAlarm.TabIndex = 8
        '
        'PnlGrid
        '
        Me.PnlGrid.BackColor = System.Drawing.SystemColors.Control
        Me.PnlGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PnlGrid.Controls.Add(Me.Label3)
        Me.PnlGrid.Controls.Add(Me.Label2)
        Me.PnlGrid.Controls.Add(Me.LblHdrAntenna)
        Me.PnlGrid.Controls.Add(Me.LblHdrCommand)
        Me.PnlGrid.Controls.Add(Me.LblHdrOffset)
        Me.PnlGrid.Controls.Add(Me.LblHdrSpeed)
        Me.PnlGrid.Controls.Add(Me.LblHdrPlHw)
        Me.PnlGrid.Controls.Add(Me.LblHdrLimitHw)
        Me.PnlGrid.Controls.Add(Me.LblHdrPlSw)
        Me.PnlGrid.Controls.Add(Me.LblHdrBrk)
        Me.PnlGrid.Controls.Add(Me.LblHdrMajorF)
        Me.PnlGrid.Controls.Add(Me.LblHdrMinorF)
        Me.PnlGrid.Controls.Add(Me.LblAzTag)
        Me.PnlGrid.Controls.Add(Me.TxtAz)
        Me.PnlGrid.Controls.Add(Me.LblAzimuth)
        Me.PnlGrid.Controls.Add(Me.LblAzError)
        Me.PnlGrid.Controls.Add(Me.LblAzSpeed)
        Me.PnlGrid.Controls.Add(Me.LblAzSpeedUnit)
        Me.PnlGrid.Controls.Add(Me.LblAllarme1)
        Me.PnlGrid.Controls.Add(Me.LblAllarme2)
        Me.PnlGrid.Controls.Add(Me.LblAllarme3)
        Me.PnlGrid.Controls.Add(Me.LblAllarme4)
        Me.PnlGrid.Controls.Add(Me.LblAzTBrk)
        Me.PnlGrid.Controls.Add(Me.LblAzMaFault)
        Me.PnlGrid.Controls.Add(Me.LblAzMiFault)
        Me.PnlGrid.Controls.Add(Me.LblElTag)
        Me.PnlGrid.Controls.Add(Me.TxtEl)
        Me.PnlGrid.Controls.Add(Me.LblElevazione)
        Me.PnlGrid.Controls.Add(Me.LblElError)
        Me.PnlGrid.Controls.Add(Me.LblElSpeed)
        Me.PnlGrid.Controls.Add(Me.LblElSpeedUnit)
        Me.PnlGrid.Controls.Add(Me.LblAllarme5)
        Me.PnlGrid.Controls.Add(Me.LblAllarme6)
        Me.PnlGrid.Controls.Add(Me.LblAllarme7)
        Me.PnlGrid.Controls.Add(Me.LblAllarme8)
        Me.PnlGrid.Controls.Add(Me.LblElTBrk)
        Me.PnlGrid.Controls.Add(Me.LblElMaFault)
        Me.PnlGrid.Controls.Add(Me.LblElMiFault)
        Me.PnlGrid.Controls.Add(Me.LblAzStow)
        Me.PnlGrid.Controls.Add(Me.LblAzAligned)
        Me.PnlGrid.Controls.Add(Me.LblElStow)
        Me.PnlGrid.Controls.Add(Me.LblElAligned)
        Me.PnlGrid.Controls.Add(Me.LblElHand)
        Me.PnlGrid.Location = New System.Drawing.Point(17, 125)
        Me.PnlGrid.Name = "PnlGrid"
        Me.PnlGrid.Size = New System.Drawing.Size(1310, 97)
        Me.PnlGrid.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(1185, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 16)
        Me.Label3.TabIndex = 43
        Me.Label3.Text = "Sector"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(799, 6)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(36, 16)
        Me.Label2.TabIndex = 42
        Me.Label2.Text = "Stow"
        '
        'LblHdrAntenna
        '
        Me.LblHdrAntenna.Location = New System.Drawing.Point(65, 9)
        Me.LblHdrAntenna.Name = "LblHdrAntenna"
        Me.LblHdrAntenna.Size = New System.Drawing.Size(80, 16)
        Me.LblHdrAntenna.TabIndex = 0
        Me.LblHdrAntenna.Text = "Antenna"
        '
        'LblHdrCommand
        '
        Me.LblHdrCommand.Location = New System.Drawing.Point(153, 9)
        Me.LblHdrCommand.Name = "LblHdrCommand"
        Me.LblHdrCommand.Size = New System.Drawing.Size(90, 16)
        Me.LblHdrCommand.TabIndex = 1
        Me.LblHdrCommand.Text = "Command"
        '
        'LblHdrOffset
        '
        Me.LblHdrOffset.Location = New System.Drawing.Point(248, 9)
        Me.LblHdrOffset.Name = "LblHdrOffset"
        Me.LblHdrOffset.Size = New System.Drawing.Size(73, 16)
        Me.LblHdrOffset.TabIndex = 2
        Me.LblHdrOffset.Text = "Offset"
        '
        'LblHdrSpeed
        '
        Me.LblHdrSpeed.Location = New System.Drawing.Point(326, 9)
        Me.LblHdrSpeed.Name = "LblHdrSpeed"
        Me.LblHdrSpeed.Size = New System.Drawing.Size(60, 16)
        Me.LblHdrSpeed.TabIndex = 3
        Me.LblHdrSpeed.Text = "Speed"
        '
        'LblHdrPlHw
        '
        Me.LblHdrPlHw.Location = New System.Drawing.Point(451, 4)
        Me.LblHdrPlHw.Name = "LblHdrPlHw"
        Me.LblHdrPlHw.Size = New System.Drawing.Size(48, 16)
        Me.LblHdrPlHw.TabIndex = 4
        Me.LblHdrPlHw.Text = "Pl Hw"
        '
        'LblHdrLimitHw
        '
        Me.LblHdrLimitHw.Location = New System.Drawing.Point(499, 4)
        Me.LblHdrLimitHw.Name = "LblHdrLimitHw"
        Me.LblHdrLimitHw.Size = New System.Drawing.Size(70, 16)
        Me.LblHdrLimitHw.TabIndex = 5
        Me.LblHdrLimitHw.Text = "Limit Hw"
        '
        'LblHdrPlSw
        '
        Me.LblHdrPlSw.Location = New System.Drawing.Point(634, 4)
        Me.LblHdrPlSw.Name = "LblHdrPlSw"
        Me.LblHdrPlSw.Size = New System.Drawing.Size(80, 16)
        Me.LblHdrPlSw.TabIndex = 6
        Me.LblHdrPlSw.Text = "Pl Sw"
        '
        'LblHdrBrk
        '
        Me.LblHdrBrk.Location = New System.Drawing.Point(1004, 10)
        Me.LblHdrBrk.Name = "LblHdrBrk"
        Me.LblHdrBrk.Size = New System.Drawing.Size(40, 16)
        Me.LblHdrBrk.TabIndex = 7
        Me.LblHdrBrk.Text = "Brk"
        '
        'LblHdrMajorF
        '
        Me.LblHdrMajorF.Location = New System.Drawing.Point(1064, 10)
        Me.LblHdrMajorF.Name = "LblHdrMajorF"
        Me.LblHdrMajorF.Size = New System.Drawing.Size(55, 16)
        Me.LblHdrMajorF.TabIndex = 8
        Me.LblHdrMajorF.Text = "Major f."
        '
        'LblHdrMinorF
        '
        Me.LblHdrMinorF.Location = New System.Drawing.Point(1124, 10)
        Me.LblHdrMinorF.Name = "LblHdrMinorF"
        Me.LblHdrMinorF.Size = New System.Drawing.Size(55, 16)
        Me.LblHdrMinorF.TabIndex = 9
        Me.LblHdrMinorF.Text = "Minor f."
        '
        'LblAzTag
        '
        Me.LblAzTag.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzTag.Location = New System.Drawing.Point(7, 23)
        Me.LblAzTag.Name = "LblAzTag"
        Me.LblAzTag.Size = New System.Drawing.Size(38, 20)
        Me.LblAzTag.TabIndex = 10
        Me.LblAzTag.Text = "AZ"
        '
        'TxtAz
        '
        Me.TxtAz.BackColor = System.Drawing.Color.Yellow
        Me.TxtAz.Location = New System.Drawing.Point(65, 26)
        Me.TxtAz.Name = "TxtAz"
        Me.TxtAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtAz.ReadOnly = True
        Me.TxtAz.Size = New System.Drawing.Size(82, 22)
        Me.TxtAz.TabIndex = 11
        '
        'LblAzimuth
        '
        Me.LblAzimuth.BackColor = System.Drawing.Color.Yellow
        Me.LblAzimuth.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzimuth.Location = New System.Drawing.Point(153, 26)
        Me.LblAzimuth.Name = "LblAzimuth"
        Me.LblAzimuth.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblAzimuth.Size = New System.Drawing.Size(90, 22)
        Me.LblAzimuth.TabIndex = 12
        '
        'LblAzError
        '
        Me.LblAzError.BackColor = System.Drawing.Color.Yellow
        Me.LblAzError.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzError.Location = New System.Drawing.Point(248, 26)
        Me.LblAzError.Name = "LblAzError"
        Me.LblAzError.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblAzError.Size = New System.Drawing.Size(73, 22)
        Me.LblAzError.TabIndex = 13
        '
        'LblAzSpeed
        '
        Me.LblAzSpeed.BackColor = System.Drawing.Color.Yellow
        Me.LblAzSpeed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzSpeed.Location = New System.Drawing.Point(326, 26)
        Me.LblAzSpeed.Name = "LblAzSpeed"
        Me.LblAzSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblAzSpeed.Size = New System.Drawing.Size(49, 22)
        Me.LblAzSpeed.TabIndex = 14
        '
        'LblAzSpeedUnit
        '
        Me.LblAzSpeedUnit.Location = New System.Drawing.Point(379, 29)
        Me.LblAzSpeedUnit.Name = "LblAzSpeedUnit"
        Me.LblAzSpeedUnit.Size = New System.Drawing.Size(16, 16)
        Me.LblAzSpeedUnit.TabIndex = 15
        Me.LblAzSpeedUnit.Text = "V"
        '
        'LblAllarme1
        '
        Me.LblAllarme1.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme1.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme1.Location = New System.Drawing.Point(456, 23)
        Me.LblAllarme1.Name = "LblAllarme1"
        Me.LblAllarme1.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme1.TabIndex = 16
        Me.LblAllarme1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme2
        '
        Me.LblAllarme2.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme2.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme2.Location = New System.Drawing.Point(511, 23)
        Me.LblAllarme2.Name = "LblAllarme2"
        Me.LblAllarme2.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme2.TabIndex = 17
        Me.LblAllarme2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme3
        '
        Me.LblAllarme3.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme3.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme3.Location = New System.Drawing.Point(612, 23)
        Me.LblAllarme3.Name = "LblAllarme3"
        Me.LblAllarme3.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme3.TabIndex = 18
        Me.LblAllarme3.Text = "CW"
        Me.LblAllarme3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme4
        '
        Me.LblAllarme4.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme4.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme4.Location = New System.Drawing.Point(653, 23)
        Me.LblAllarme4.Name = "LblAllarme4"
        Me.LblAllarme4.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme4.TabIndex = 19
        Me.LblAllarme4.Text = "CCW"
        Me.LblAllarme4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAzTBrk
        '
        Me.LblAzTBrk.BackColor = System.Drawing.Color.Yellow
        Me.LblAzTBrk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzTBrk.Location = New System.Drawing.Point(975, 29)
        Me.LblAzTBrk.Name = "LblAzTBrk"
        Me.LblAzTBrk.Size = New System.Drawing.Size(82, 18)
        Me.LblAzTBrk.TabIndex = 20
        '
        'LblAzMaFault
        '
        Me.LblAzMaFault.BackColor = System.Drawing.Color.Yellow
        Me.LblAzMaFault.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzMaFault.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzMaFault.Location = New System.Drawing.Point(1071, 29)
        Me.LblAzMaFault.Name = "LblAzMaFault"
        Me.LblAzMaFault.Size = New System.Drawing.Size(39, 18)
        Me.LblAzMaFault.TabIndex = 21
        Me.LblAzMaFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAzMiFault
        '
        Me.LblAzMiFault.BackColor = System.Drawing.Color.Yellow
        Me.LblAzMiFault.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzMiFault.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAzMiFault.Location = New System.Drawing.Point(1133, 29)
        Me.LblAzMiFault.Name = "LblAzMiFault"
        Me.LblAzMiFault.Size = New System.Drawing.Size(39, 18)
        Me.LblAzMiFault.TabIndex = 22
        Me.LblAzMiFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblElTag
        '
        Me.LblElTag.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LblElTag.Location = New System.Drawing.Point(7, 57)
        Me.LblElTag.Name = "LblElTag"
        Me.LblElTag.Size = New System.Drawing.Size(52, 24)
        Me.LblElTag.TabIndex = 23
        Me.LblElTag.Text = "EL"
        '
        'TxtEl
        '
        Me.TxtEl.BackColor = System.Drawing.Color.Yellow
        Me.TxtEl.Location = New System.Drawing.Point(65, 62)
        Me.TxtEl.Name = "TxtEl"
        Me.TxtEl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtEl.ReadOnly = True
        Me.TxtEl.Size = New System.Drawing.Size(82, 22)
        Me.TxtEl.TabIndex = 24
        '
        'LblElevazione
        '
        Me.LblElevazione.BackColor = System.Drawing.Color.Yellow
        Me.LblElevazione.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElevazione.Location = New System.Drawing.Point(153, 62)
        Me.LblElevazione.Name = "LblElevazione"
        Me.LblElevazione.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblElevazione.Size = New System.Drawing.Size(90, 22)
        Me.LblElevazione.TabIndex = 25
        '
        'LblElError
        '
        Me.LblElError.BackColor = System.Drawing.Color.Yellow
        Me.LblElError.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElError.Location = New System.Drawing.Point(248, 62)
        Me.LblElError.Name = "LblElError"
        Me.LblElError.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblElError.Size = New System.Drawing.Size(73, 22)
        Me.LblElError.TabIndex = 26
        '
        'LblElSpeed
        '
        Me.LblElSpeed.BackColor = System.Drawing.Color.Yellow
        Me.LblElSpeed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElSpeed.Location = New System.Drawing.Point(326, 62)
        Me.LblElSpeed.Name = "LblElSpeed"
        Me.LblElSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblElSpeed.Size = New System.Drawing.Size(49, 22)
        Me.LblElSpeed.TabIndex = 27
        '
        'LblElSpeedUnit
        '
        Me.LblElSpeedUnit.Location = New System.Drawing.Point(379, 65)
        Me.LblElSpeedUnit.Name = "LblElSpeedUnit"
        Me.LblElSpeedUnit.Size = New System.Drawing.Size(16, 16)
        Me.LblElSpeedUnit.TabIndex = 28
        Me.LblElSpeedUnit.Text = "V"
        '
        'LblAllarme5
        '
        Me.LblAllarme5.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme5.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme5.Location = New System.Drawing.Point(456, 59)
        Me.LblAllarme5.Name = "LblAllarme5"
        Me.LblAllarme5.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme5.TabIndex = 29
        Me.LblAllarme5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme6
        '
        Me.LblAllarme6.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme6.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme6.Location = New System.Drawing.Point(511, 59)
        Me.LblAllarme6.Name = "LblAllarme6"
        Me.LblAllarme6.Size = New System.Drawing.Size(38, 18)
        Me.LblAllarme6.TabIndex = 30
        Me.LblAllarme6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme7
        '
        Me.LblAllarme7.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme7.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme7.Location = New System.Drawing.Point(612, 59)
        Me.LblAllarme7.Name = "LblAllarme7"
        Me.LblAllarme7.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme7.TabIndex = 31
        Me.LblAllarme7.Text = "UP"
        Me.LblAllarme7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme8
        '
        Me.LblAllarme8.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme8.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme8.Location = New System.Drawing.Point(653, 59)
        Me.LblAllarme8.Name = "LblAllarme8"
        Me.LblAllarme8.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme8.TabIndex = 32
        Me.LblAllarme8.Text = "DW"
        Me.LblAllarme8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblElTBrk
        '
        Me.LblElTBrk.BackColor = System.Drawing.Color.Yellow
        Me.LblElTBrk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElTBrk.Location = New System.Drawing.Point(975, 65)
        Me.LblElTBrk.Name = "LblElTBrk"
        Me.LblElTBrk.Size = New System.Drawing.Size(82, 18)
        Me.LblElTBrk.TabIndex = 33
        '
        'LblElMaFault
        '
        Me.LblElMaFault.BackColor = System.Drawing.Color.Yellow
        Me.LblElMaFault.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElMaFault.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblElMaFault.Location = New System.Drawing.Point(1071, 65)
        Me.LblElMaFault.Name = "LblElMaFault"
        Me.LblElMaFault.Size = New System.Drawing.Size(39, 18)
        Me.LblElMaFault.TabIndex = 34
        Me.LblElMaFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblElMiFault
        '
        Me.LblElMiFault.BackColor = System.Drawing.Color.Yellow
        Me.LblElMiFault.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblElMiFault.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblElMiFault.Location = New System.Drawing.Point(1133, 65)
        Me.LblElMiFault.Name = "LblElMiFault"
        Me.LblElMiFault.Size = New System.Drawing.Size(39, 18)
        Me.LblElMiFault.TabIndex = 35
        Me.LblElMiFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAzStow
        '
        Me.LblAzStow.BackColor = System.Drawing.Color.Yellow
        Me.LblAzStow.Location = New System.Drawing.Point(755, 24)
        Me.LblAzStow.Visible = False   ' K09: nascosto nel VB6
        Me.LblAzStow.Name = "LblAzStow"
        Me.LblAzStow.Size = New System.Drawing.Size(75, 18)
        Me.LblAzStow.TabIndex = 36
        '
        'LblAzAligned
        '
        Me.LblAzAligned.BackColor = System.Drawing.Color.Yellow
        Me.LblAzAligned.Location = New System.Drawing.Point(836, 23)
        Me.LblAzAligned.Visible = False   ' K09: nascosto nel VB6
        Me.LblAzAligned.Name = "LblAzAligned"
        Me.LblAzAligned.Size = New System.Drawing.Size(63, 18)
        Me.LblAzAligned.TabIndex = 37
        '
        'LblElStow
        '
        Me.LblElStow.BackColor = System.Drawing.Color.Yellow
        Me.LblElStow.Location = New System.Drawing.Point(755, 61)
        Me.LblElStow.Name = "LblElStow"
        Me.LblElStow.Size = New System.Drawing.Size(75, 18)
        Me.LblElStow.TabIndex = 38
        '
        'LblElAligned
        '
        Me.LblElAligned.BackColor = System.Drawing.Color.Yellow
        Me.LblElAligned.Location = New System.Drawing.Point(836, 61)
        Me.LblElAligned.Name = "LblElAligned"
        Me.LblElAligned.Size = New System.Drawing.Size(63, 18)
        Me.LblElAligned.TabIndex = 39
        '
        'LblElHand
        '
        Me.LblElHand.BackColor = System.Drawing.Color.Yellow
        Me.LblElHand.Location = New System.Drawing.Point(1194, 29)
        Me.LblElHand.Name = "LblElHand"
        Me.LblElHand.Size = New System.Drawing.Size(40, 18)
        Me.LblElHand.TabIndex = 40
        '
        'PnlPol
        '
        Me.PnlPol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PnlPol.Controls.Add(Me.Label1)
        Me.PnlPol.Controls.Add(Me.LblPolTag)
        Me.PnlPol.Controls.Add(Me.TxtPol)
        Me.PnlPol.Controls.Add(Me.LblPol)
        Me.PnlPol.Controls.Add(Me.LblPol1)
        Me.PnlPol.Controls.Add(Me.LblLNATag)
        Me.PnlPol.Controls.Add(Me.LblLNA)
        Me.PnlPol.Controls.Add(Me.TxtPolErr)
        Me.PnlPol.Controls.Add(Me.LblPolFault)
        Me.PnlPol.Controls.Add(Me.LblAllarme16)
        Me.PnlPol.Controls.Add(Me.LblAllarme17)
        Me.PnlPol.Controls.Add(Me.LblAllarme18)
        Me.PnlPol.Location = New System.Drawing.Point(17, 240)
        Me.PnlPol.Name = "PnlPol"
        Me.PnlPol.Size = New System.Drawing.Size(1310, 51)
        Me.PnlPol.TabIndex = 10
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(438, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 16)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Pol.Err."
        '
        'LblPolTag
        '
        Me.LblPolTag.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LblPolTag.Location = New System.Drawing.Point(7, 12)
        Me.LblPolTag.Name = "LblPolTag"
        Me.LblPolTag.Size = New System.Drawing.Size(52, 27)
        Me.LblPolTag.TabIndex = 0
        Me.LblPolTag.Text = "POL"
        Me.LblPolTag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtPol
        '
        Me.TxtPol.BackColor = System.Drawing.Color.Yellow
        Me.TxtPol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtPol.Location = New System.Drawing.Point(90, 9)
        Me.TxtPol.Name = "TxtPol"
        Me.TxtPol.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.TxtPol.Size = New System.Drawing.Size(73, 27)
        Me.TxtPol.TabIndex = 1
        '
        'LblPol
        '
        Me.LblPol.BackColor = System.Drawing.Color.Yellow
        Me.LblPol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblPol.Location = New System.Drawing.Point(172, 9)
        Me.LblPol.Name = "LblPol"
        Me.LblPol.Size = New System.Drawing.Size(92, 27)
        Me.LblPol.TabIndex = 2
        '
        'LblPol1
        '
        Me.LblPol1.BackColor = System.Drawing.Color.Yellow
        Me.LblPol1.Location = New System.Drawing.Point(270, 11)
        Me.LblPol1.Name = "LblPol1"
        Me.LblPol1.ReadOnly = True
        Me.LblPol1.Size = New System.Drawing.Size(73, 22)
        Me.LblPol1.TabIndex = 3
        '
        'LblLNATag
        '
        Me.LblLNATag.Location = New System.Drawing.Point(342, 13)
        Me.LblLNATag.Name = "LblLNATag"
        Me.LblLNATag.Size = New System.Drawing.Size(49, 18)
        Me.LblLNATag.TabIndex = 4
        Me.LblLNATag.Text = "LNA"
        '
        'LblLNA
        '
        Me.LblLNA.BackColor = System.Drawing.Color.Yellow
        Me.LblLNA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblLNA.Location = New System.Drawing.Point(395, 11)
        Me.LblLNA.Name = "LblLNA"
        Me.LblLNA.Size = New System.Drawing.Size(22, 22)
        Me.LblLNA.TabIndex = 5
        '
        'TxtPolErr
        '
        Me.TxtPolErr.BackColor = System.Drawing.Color.Yellow
        Me.TxtPolErr.Location = New System.Drawing.Point(505, 11)
        Me.TxtPolErr.Name = "TxtPolErr"
        Me.TxtPolErr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPolErr.ReadOnly = True
        Me.TxtPolErr.Size = New System.Drawing.Size(54, 22)
        Me.TxtPolErr.TabIndex = 6
        '
        'LblPolFault
        '
        Me.LblPolFault.Location = New System.Drawing.Point(776, 12)
        Me.LblPolFault.BackColor = System.Drawing.Color.Yellow
        Me.LblPolFault.Name = "LblPolFault"
        Me.LblPolFault.Size = New System.Drawing.Size(44, 20)
        Me.LblPolFault.TabIndex = 7
        Me.LblPolFault.Text = "Fault"
        '
        'LblAllarme16
        '
        Me.LblAllarme16.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme16.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme16.Location = New System.Drawing.Point(570, 12)
        Me.LblAllarme16.Name = "LblAllarme16"
        Me.LblAllarme16.Size = New System.Drawing.Size(38, 20)
        Me.LblAllarme16.TabIndex = 8
        Me.LblAllarme16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LblAllarme16.Visible = False
        '
        'LblAllarme17
        '
        Me.LblAllarme17.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme17.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme17.Location = New System.Drawing.Point(662, 12)
        Me.LblAllarme17.Name = "LblAllarme17"
        Me.LblAllarme17.Size = New System.Drawing.Size(37, 20)
        Me.LblAllarme17.TabIndex = 9
        Me.LblAllarme17.Text = "CCW"
        Me.LblAllarme17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme18
        '
        Me.LblAllarme18.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme18.Font = New System.Drawing.Font("Arial", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LblAllarme18.Location = New System.Drawing.Point(622, 12)
        Me.LblAllarme18.Name = "LblAllarme18"
        Me.LblAllarme18.Size = New System.Drawing.Size(38, 20)
        Me.LblAllarme18.TabIndex = 10
        Me.LblAllarme18.Text = "CW"
        Me.LblAllarme18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PnlStatus
        '
        Me.PnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PnlStatus.Controls.Add(Me.LblMCTag)
        Me.PnlStatus.Controls.Add(Me.TxtACU)
        Me.PnlStatus.Controls.Add(Me.LblSumTag)
        Me.PnlStatus.Controls.Add(Me.TxtLev)
        Me.PnlStatus.Controls.Add(Me.LblVTagSum)
        Me.PnlStatus.Controls.Add(Me.LblAzErTag)
        Me.PnlStatus.Controls.Add(Me.TxtAzEr)
        Me.PnlStatus.Controls.Add(Me.LblVTagAzEr)
        Me.PnlStatus.Controls.Add(Me.LblElErTag)
        Me.PnlStatus.Controls.Add(Me.TxtElEr)
        Me.PnlStatus.Controls.Add(Me.LblVTagElEr)
        Me.PnlStatus.Controls.Add(Me.LblRcvStatusTag)
        Me.PnlStatus.Controls.Add(Me.LblLock)
        Me.PnlStatus.Controls.Add(Me.LblBcnLevelTag)
        Me.PnlStatus.Controls.Add(Me.LblLev)
        Me.PnlStatus.Controls.Add(Me.LblAllarme11)
        Me.PnlStatus.Controls.Add(Me.LblAllarme10)
        Me.PnlStatus.Controls.Add(Me.LblAllarme9)
        Me.PnlStatus.Controls.Add(Me.LblEmStopTag)
        Me.PnlStatus.Controls.Add(Me.LblEmergency)
        Me.PnlStatus.Controls.Add(Me.LblEmergency1)
        Me.PnlStatus.Controls.Add(Me.LblMduTag)
        Me.PnlStatus.Controls.Add(Me.LblAzDR)
        Me.PnlStatus.Controls.Add(Me.LblPCU)
        Me.PnlStatus.Controls.Add(Me.LblLowAngleTag)
        Me.PnlStatus.Controls.Add(Me.LblLowAngle)
        Me.PnlStatus.Controls.Add(Me.LblMemoryTag)
        Me.PnlStatus.Controls.Add(Me.LblAllarme12)
        Me.PnlStatus.Controls.Add(Me.LblMemDataTag)
        Me.PnlStatus.Controls.Add(Me.LblAllarme13)
        Me.PnlStatus.Controls.Add(Me.LblSatDataTag)
        Me.PnlStatus.Controls.Add(Me.LblAllarme14)
        Me.PnlStatus.Controls.Add(Me.LblStowUnstowTag)
        Me.PnlStatus.Controls.Add(Me.LblAllarme15)
        Me.PnlStatus.Controls.Add(Me.LblWarning)
        Me.PnlStatus.Location = New System.Drawing.Point(17, 312)
        Me.PnlStatus.Name = "PnlStatus"
        Me.PnlStatus.Size = New System.Drawing.Size(1310, 92)
        Me.PnlStatus.TabIndex = 11
        '
        'LblMCTag
        '
        Me.LblMCTag.Location = New System.Drawing.Point(8, 19)
        Me.LblMCTag.Name = "LblMCTag"
        Me.LblMCTag.Size = New System.Drawing.Size(36, 18)
        Me.LblMCTag.TabIndex = 0
        Me.LblMCTag.Text = "M/C"
        '
        'TxtACU
        '
        Me.TxtACU.BackColor = System.Drawing.Color.Yellow
        Me.TxtACU.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtACU.Location = New System.Drawing.Point(50, 18)
        Me.TxtACU.Name = "TxtACU"
        Me.TxtACU.Size = New System.Drawing.Size(44, 22)
        Me.TxtACU.TabIndex = 1
        Me.TxtACU.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblSumTag
        '
        Me.LblSumTag.Location = New System.Drawing.Point(106, 20)
        Me.LblSumTag.Name = "LblSumTag"
        Me.LblSumTag.Size = New System.Drawing.Size(32, 18)
        Me.LblSumTag.TabIndex = 2
        Me.LblSumTag.Text = "SUM"
        '
        'TxtLev
        '
        Me.TxtLev.BackColor = System.Drawing.Color.Yellow
        Me.TxtLev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TxtLev.Location = New System.Drawing.Point(144, 18)
        Me.TxtLev.Name = "TxtLev"
        Me.TxtLev.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.TxtLev.Size = New System.Drawing.Size(51, 22)
        Me.TxtLev.TabIndex = 3
        '
        'LblVTagSum
        '
        Me.LblVTagSum.Location = New System.Drawing.Point(199, 20)
        Me.LblVTagSum.Name = "LblVTagSum"
        Me.LblVTagSum.Size = New System.Drawing.Size(16, 16)
        Me.LblVTagSum.TabIndex = 4
        Me.LblVTagSum.Text = "V"
        '
        'LblAzErTag
        '
        Me.LblAzErTag.Location = New System.Drawing.Point(218, 20)
        Me.LblAzErTag.Name = "LblAzErTag"
        Me.LblAzErTag.Size = New System.Drawing.Size(37, 18)
        Me.LblAzErTag.TabIndex = 5
        Me.LblAzErTag.Text = "Az/Er"
        '
        'TxtAzEr
        '
        Me.TxtAzEr.BackColor = System.Drawing.Color.Yellow
        Me.TxtAzEr.Location = New System.Drawing.Point(262, 18)
        Me.TxtAzEr.Name = "TxtAzEr"
        Me.TxtAzEr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtAzEr.ReadOnly = True
        Me.TxtAzEr.Size = New System.Drawing.Size(39, 22)
        Me.TxtAzEr.TabIndex = 6
        '
        'LblVTagAzEr
        '
        Me.LblVTagAzEr.Location = New System.Drawing.Point(305, 20)
        Me.LblVTagAzEr.Name = "LblVTagAzEr"
        Me.LblVTagAzEr.Size = New System.Drawing.Size(16, 16)
        Me.LblVTagAzEr.TabIndex = 7
        Me.LblVTagAzEr.Text = "V"
        '
        'LblElErTag
        '
        Me.LblElErTag.Location = New System.Drawing.Point(334, 20)
        Me.LblElErTag.Name = "LblElErTag"
        Me.LblElErTag.Size = New System.Drawing.Size(32, 18)
        Me.LblElErTag.TabIndex = 8
        Me.LblElErTag.Text = "El/Er"
        '
        'TxtElEr
        '
        Me.TxtElEr.BackColor = System.Drawing.Color.Yellow
        Me.TxtElEr.Location = New System.Drawing.Point(370, 18)
        Me.TxtElEr.Name = "TxtElEr"
        Me.TxtElEr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtElEr.ReadOnly = True
        Me.TxtElEr.Size = New System.Drawing.Size(39, 22)
        Me.TxtElEr.TabIndex = 9
        '
        'LblVTagElEr
        '
        Me.LblVTagElEr.Location = New System.Drawing.Point(413, 20)
        Me.LblVTagElEr.Name = "LblVTagElEr"
        Me.LblVTagElEr.Size = New System.Drawing.Size(16, 16)
        Me.LblVTagElEr.TabIndex = 10
        Me.LblVTagElEr.Text = "V"
        '
        'LblRcvStatusTag
        '
        Me.LblRcvStatusTag.Location = New System.Drawing.Point(439, 20)
        Me.LblRcvStatusTag.Name = "LblRcvStatusTag"
        Me.LblRcvStatusTag.Size = New System.Drawing.Size(75, 18)
        Me.LblRcvStatusTag.TabIndex = 11
        Me.LblRcvStatusTag.Text = "RCV Status"
        '
        'LblLock
        '
        Me.LblLock.BackColor = System.Drawing.Color.Yellow
        Me.LblLock.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblLock.Location = New System.Drawing.Point(523, 18)
        Me.LblLock.Name = "LblLock"
        Me.LblLock.Size = New System.Drawing.Size(70, 22)
        Me.LblLock.TabIndex = 12
        Me.LblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblBcnLevelTag
        '
        Me.LblBcnLevelTag.Location = New System.Drawing.Point(612, 20)
        Me.LblBcnLevelTag.Name = "LblBcnLevelTag"
        Me.LblBcnLevelTag.Size = New System.Drawing.Size(94, 18)
        Me.LblBcnLevelTag.TabIndex = 13
        Me.LblBcnLevelTag.Text = "BCN Level"
        '
        'LblLev
        '
        Me.LblLev.BackColor = System.Drawing.Color.Yellow
        Me.LblLev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblLev.Location = New System.Drawing.Point(709, 18)
        Me.LblLev.Name = "LblLev"
        Me.LblLev.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.LblLev.Size = New System.Drawing.Size(49, 22)
        Me.LblLev.TabIndex = 14
        '
        'LblAllarme11
        '
        Me.LblAllarme11.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme11.Location = New System.Drawing.Point(842, 20)
        Me.LblAllarme11.Name = "LblAllarme11"
        Me.LblAllarme11.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme11.TabIndex = 15
        Me.LblAllarme11.Text = "RX"
        Me.LblAllarme11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblAllarme10
        '
        Me.LblAllarme10.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme10.Location = New System.Drawing.Point(886, 20)
        Me.LblAllarme10.Name = "LblAllarme10"
        Me.LblAllarme10.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme10.TabIndex = 16
        Me.LblAllarme10.Text = "LOW"
        Me.LblAllarme10.Visible = False
        '
        'LblAllarme9
        '
        Me.LblAllarme9.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme9.Location = New System.Drawing.Point(930, 20)
        Me.LblAllarme9.Name = "LblAllarme9"
        Me.LblAllarme9.Size = New System.Drawing.Size(40, 18)
        Me.LblAllarme9.TabIndex = 17
        Me.LblAllarme9.Text = "HIGH"
        Me.LblAllarme9.Visible = False
        '
        'LblEmStopTag
        '
        Me.LblEmStopTag.Location = New System.Drawing.Point(16, 59)
        Me.LblEmStopTag.Name = "LblEmStopTag"
        Me.LblEmStopTag.Size = New System.Drawing.Size(68, 15)
        Me.LblEmStopTag.TabIndex = 18
        Me.LblEmStopTag.Text = "Em. stop"
        '
        'LblEmergency
        '
        Me.LblEmergency.BackColor = System.Drawing.Color.Yellow
        Me.LblEmergency.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblEmergency.ForeColor = System.Drawing.Color.White
        Me.LblEmergency.Location = New System.Drawing.Point(94, 55)
        Me.LblEmergency.Name = "LblEmergency"
        Me.LblEmergency.Size = New System.Drawing.Size(27, 22)
        Me.LblEmergency.TabIndex = 19
        Me.LblEmergency.Text = "IN"
        Me.LblEmergency.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblEmergency1
        '
        Me.LblEmergency1.BackColor = System.Drawing.Color.Yellow
        Me.LblEmergency1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblEmergency1.ForeColor = System.Drawing.Color.White
        Me.LblEmergency1.Location = New System.Drawing.Point(128, 55)
        Me.LblEmergency1.Name = "LblEmergency1"
        Me.LblEmergency1.Size = New System.Drawing.Size(30, 22)
        Me.LblEmergency1.TabIndex = 20
        Me.LblEmergency1.Text = "OUT"
        Me.LblEmergency1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblMduTag
        '
        Me.LblMduTag.Location = New System.Drawing.Point(171, 59)
        Me.LblMduTag.Name = "LblMduTag"
        Me.LblMduTag.Size = New System.Drawing.Size(37, 15)
        Me.LblMduTag.TabIndex = 21
        Me.LblMduTag.Text = "MDU"
        '
        'LblAzDR
        '
        Me.LblAzDR.BackColor = System.Drawing.Color.Yellow
        Me.LblAzDR.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAzDR.Location = New System.Drawing.Point(210, 55)
        Me.LblAzDR.Name = "LblAzDR"
        Me.LblAzDR.Size = New System.Drawing.Size(49, 22)
        Me.LblAzDR.TabIndex = 22
        '
        'LblPCU
        '
        Me.LblPCU.BackColor = System.Drawing.Color.Yellow
        Me.LblPCU.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblPCU.Location = New System.Drawing.Point(270, 55)
        Me.LblPCU.Name = "LblPCU"
        Me.LblPCU.Size = New System.Drawing.Size(49, 22)
        Me.LblPCU.TabIndex = 23
        '
        'LblLowAngleTag
        '
        Me.LblLowAngleTag.Location = New System.Drawing.Point(325, 59)
        Me.LblLowAngleTag.Name = "LblLowAngleTag"
        Me.LblLowAngleTag.Size = New System.Drawing.Size(49, 15)
        Me.LblLowAngleTag.TabIndex = 24
        Me.LblLowAngleTag.Text = "Low Angle"
        Me.LblLowAngleTag.Visible = False
        '
        'LblLowAngle
        '
        Me.LblLowAngle.BackColor = System.Drawing.Color.Yellow
        Me.LblLowAngle.Location = New System.Drawing.Point(382, 55)
        Me.LblLowAngle.Name = "LblLowAngle"
        Me.LblLowAngle.Size = New System.Drawing.Size(44, 22)
        Me.LblLowAngle.TabIndex = 25
        Me.LblLowAngle.Visible = False
        '
        'LblMemoryTag
        '
        Me.LblMemoryTag.Location = New System.Drawing.Point(615, 57)
        Me.LblMemoryTag.Name = "LblMemoryTag"
        Me.LblMemoryTag.Size = New System.Drawing.Size(109, 18)
        Me.LblMemoryTag.TabIndex = 26
        Me.LblMemoryTag.Text = "Memory: Param."
        '
        'LblAllarme12
        '
        Me.LblAllarme12.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme12.Location = New System.Drawing.Point(730, 55)
        Me.LblAllarme12.Name = "LblAllarme12"
        Me.LblAllarme12.Size = New System.Drawing.Size(8, 22)
        Me.LblAllarme12.TabIndex = 27
        '
        'LblMemDataTag
        '
        Me.LblMemDataTag.Location = New System.Drawing.Point(745, 57)
        Me.LblMemDataTag.Name = "LblMemDataTag"
        Me.LblMemDataTag.Size = New System.Drawing.Size(65, 18)
        Me.LblMemDataTag.TabIndex = 28
        Me.LblMemDataTag.Text = "Mem.Data"
        '
        'LblAllarme13
        '
        Me.LblAllarme13.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme13.Location = New System.Drawing.Point(810, 55)
        Me.LblAllarme13.Name = "LblAllarme13"
        Me.LblAllarme13.Size = New System.Drawing.Size(8, 22)
        Me.LblAllarme13.TabIndex = 29
        '
        'LblSatDataTag
        '
        Me.LblSatDataTag.Location = New System.Drawing.Point(819, 57)
        Me.LblSatDataTag.Name = "LblSatDataTag"
        Me.LblSatDataTag.Size = New System.Drawing.Size(56, 18)
        Me.LblSatDataTag.TabIndex = 30
        Me.LblSatDataTag.Text = "Sat.Data"
        '
        'LblAllarme14
        '
        Me.LblAllarme14.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme14.Location = New System.Drawing.Point(877, 55)
        Me.LblAllarme14.Name = "LblAllarme14"
        Me.LblAllarme14.Size = New System.Drawing.Size(8, 22)
        Me.LblAllarme14.TabIndex = 31
        '
        'LblStowUnstowTag
        '
        Me.LblStowUnstowTag.Location = New System.Drawing.Point(894, 57)
        Me.LblStowUnstowTag.Name = "LblStowUnstowTag"
        Me.LblStowUnstowTag.Size = New System.Drawing.Size(85, 18)
        Me.LblStowUnstowTag.TabIndex = 32
        Me.LblStowUnstowTag.Text = "Stow/Unstow"
        '
        'LblAllarme15
        '
        Me.LblAllarme15.BackColor = System.Drawing.Color.Yellow
        Me.LblAllarme15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblAllarme15.Location = New System.Drawing.Point(980, 55)
        Me.LblAllarme15.Name = "LblAllarme15"
        Me.LblAllarme15.Size = New System.Drawing.Size(8, 22)
        Me.LblAllarme15.TabIndex = 33
        '
        'LblWarning
        '
        Me.LblWarning.ForeColor = System.Drawing.Color.Red
        Me.LblWarning.Location = New System.Drawing.Point(16, 116)
        Me.LblWarning.BackColor = System.Drawing.Color.Yellow
        Me.LblWarning.Name = "LblWarning"
        Me.LblWarning.Size = New System.Drawing.Size(300, 20)
        Me.LblWarning.TabIndex = 34
        Me.LblWarning.Visible = False
        '
        'FrmComandi
        '
        Me.FrmComandi.Controls.Add(Me.CmdSTBY)
        Me.FrmComandi.Controls.Add(Me.CmdCmdPos)
        Me.FrmComandi.Controls.Add(Me.CmdStepTrack)
        Me.FrmComandi.Controls.Add(Me.CmdAutoTrack)
        Me.FrmComandi.Controls.Add(Me.CmdAugmented)
        Me.FrmComandi.Controls.Add(Me.CmdProgramTrack)
        Me.FrmComandi.Controls.Add(Me.CmdOffset)
        Me.FrmComandi.Controls.Add(Me.CmdAutoSearch)
        Me.FrmComandi.Controls.Add(Me.LblStepTrack)
        Me.FrmComandi.Location = New System.Drawing.Point(17, 421)
        Me.FrmComandi.Name = "FrmComandi"
        Me.FrmComandi.Size = New System.Drawing.Size(865, 96)
        Me.FrmComandi.TabIndex = 12
        Me.FrmComandi.TabStop = False
        Me.FrmComandi.Text = "Mode"
        '
        'CmdSTBY
        '
        Me.CmdSTBY.Location = New System.Drawing.Point(7, 26)
        Me.CmdSTBY.Name = "CmdSTBY"
        Me.CmdSTBY.Size = New System.Drawing.Size(95, 64)
        Me.CmdSTBY.TabIndex = 0
        Me.CmdSTBY.Text = "Stand By (F1)"
        '
        'CmdCmdPos
        '
        Me.CmdCmdPos.Location = New System.Drawing.Point(107, 26)
        Me.CmdCmdPos.Name = "CmdCmdPos"
        Me.CmdCmdPos.Size = New System.Drawing.Size(95, 64)
        Me.CmdCmdPos.TabIndex = 1
        Me.CmdCmdPos.Text = "Cmd Pos (F2)"
        '
        'CmdStepTrack
        '
        Me.CmdStepTrack.Location = New System.Drawing.Point(207, 26)
        Me.CmdStepTrack.Name = "CmdStepTrack"
        Me.CmdStepTrack.Size = New System.Drawing.Size(95, 64)
        Me.CmdStepTrack.TabIndex = 2
        Me.CmdStepTrack.Text = "Step Track (F3)"
        '
        'CmdAutoTrack
        '
        Me.CmdAutoTrack.Location = New System.Drawing.Point(356, 26)
        Me.CmdAutoTrack.Name = "CmdAutoTrack"
        Me.CmdAutoTrack.Size = New System.Drawing.Size(95, 64)
        Me.CmdAutoTrack.TabIndex = 3
        Me.CmdAutoTrack.Text = "Auto Track (F4)"
        '
        'CmdAugmented
        '
        Me.CmdAugmented.Location = New System.Drawing.Point(456, 26)
        Me.CmdAugmented.Name = "CmdAugmented"
        Me.CmdAugmented.Size = New System.Drawing.Size(95, 64)
        Me.CmdAugmented.TabIndex = 4
        Me.CmdAugmented.Text = "Augmented"
        '
        'CmdProgramTrack
        '
        Me.CmdProgramTrack.Location = New System.Drawing.Point(556, 26)
        Me.CmdProgramTrack.Name = "CmdProgramTrack"
        Me.CmdProgramTrack.Size = New System.Drawing.Size(95, 64)
        Me.CmdProgramTrack.TabIndex = 5
        Me.CmdProgramTrack.Text = "Program Track"
        '
        'CmdOffset
        '
        Me.CmdOffset.Location = New System.Drawing.Point(656, 26)
        Me.CmdOffset.Name = "CmdOffset"
        Me.CmdOffset.Size = New System.Drawing.Size(95, 64)
        Me.CmdOffset.TabIndex = 6
        Me.CmdOffset.Text = "Offset"
        Me.CmdOffset.Visible = False
        '
        'CmdAutoSearch
        '
        Me.CmdAutoSearch.Location = New System.Drawing.Point(756, 26)
        Me.CmdAutoSearch.Name = "CmdAutoSearch"
        Me.CmdAutoSearch.Size = New System.Drawing.Size(95, 64)
        Me.CmdAutoSearch.TabIndex = 7
        Me.CmdAutoSearch.Text = "Auto Search"
        '
        'LblStepTrack
        '
        Me.LblStepTrack.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblStepTrack.Location = New System.Drawing.Point(307, 41)
        Me.LblStepTrack.BackColor = System.Drawing.Color.Yellow
        Me.LblStepTrack.Name = "LblStepTrack"
        Me.LblStepTrack.Size = New System.Drawing.Size(44, 25)
        Me.LblStepTrack.TabIndex = 8
        Me.LblStepTrack.Visible = False
        '
        'LblFrqBeaconTag
        '
        Me.LblFrqBeaconTag.Location = New System.Drawing.Point(841, 590)
        Me.LblFrqBeaconTag.Name = "LblFrqBeaconTag"
        Me.LblFrqBeaconTag.Size = New System.Drawing.Size(85, 18)
        Me.LblFrqBeaconTag.TabIndex = 15
        Me.LblFrqBeaconTag.Text = "Frq. Beacon"
        '
        'TxtFreqBeacon
        '
        Me.TxtFreqBeacon.BackColor = System.Drawing.Color.Yellow
        Me.TxtFreqBeacon.Location = New System.Drawing.Point(938, 585)
        Me.TxtFreqBeacon.Name = "TxtFreqBeacon"
        Me.TxtFreqBeacon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtFreqBeacon.ReadOnly = True
        Me.TxtFreqBeacon.Size = New System.Drawing.Size(80, 22)
        Me.TxtFreqBeacon.TabIndex = 16
        '
        'LblMHzTag1
        '
        Me.LblMHzTag1.Location = New System.Drawing.Point(1020, 590)
        Me.LblMHzTag1.Name = "LblMHzTag1"
        Me.LblMHzTag1.Size = New System.Drawing.Size(42, 16)
        Me.LblMHzTag1.TabIndex = 17
        Me.LblMHzTag1.Text = "MHz"
        '
        'LblAzErrThrTag
        '
        Me.LblAzErrThrTag.Location = New System.Drawing.Point(1084, 590)
        Me.LblAzErrThrTag.Name = "LblAzErrThrTag"
        Me.LblAzErrThrTag.Size = New System.Drawing.Size(94, 18)
        Me.LblAzErrThrTag.TabIndex = 18
        Me.LblAzErrThrTag.Text = "Az Error Thr."
        '
        'TxtThrAz
        '
        Me.TxtThrAz.BackColor = System.Drawing.SystemColors.Control
        Me.TxtThrAz.Location = New System.Drawing.Point(1202, 586)
        Me.TxtThrAz.Name = "TxtThrAz"
        Me.TxtThrAz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtThrAz.Size = New System.Drawing.Size(51, 22)
        Me.TxtThrAz.TabIndex = 19
        '
        'LblAutoTrackThrTag
        '
        Me.LblAutoTrackThrTag.Location = New System.Drawing.Point(1084, 561)
        Me.LblAutoTrackThrTag.Name = "LblAutoTrackThrTag"
        Me.LblAutoTrackThrTag.Size = New System.Drawing.Size(99, 18)
        Me.LblAutoTrackThrTag.TabIndex = 20
        Me.LblAutoTrackThrTag.Text = "AutoTrack Thr."
        '
        'TxtSogliaAuto
        '
        Me.TxtSogliaAuto.BackColor = System.Drawing.SystemColors.Control
        Me.TxtSogliaAuto.Location = New System.Drawing.Point(1202, 555)
        Me.TxtSogliaAuto.Name = "TxtSogliaAuto"
        Me.TxtSogliaAuto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtSogliaAuto.Size = New System.Drawing.Size(51, 22)
        Me.TxtSogliaAuto.TabIndex = 21
        '
        'LblDbmTag1
        '
        Me.LblDbmTag1.Location = New System.Drawing.Point(1268, 559)
        Me.LblDbmTag1.Name = "LblDbmTag1"
        Me.LblDbmTag1.Size = New System.Drawing.Size(38, 16)
        Me.LblDbmTag1.TabIndex = 22
        Me.LblDbmTag1.Text = "dBm"
        '
        'LblElErrThrTag
        '
        Me.LblElErrThrTag.Location = New System.Drawing.Point(1084, 619)
        Me.LblElErrThrTag.Name = "LblElErrThrTag"
        Me.LblElErrThrTag.Size = New System.Drawing.Size(94, 18)
        Me.LblElErrThrTag.TabIndex = 23
        Me.LblElErrThrTag.Text = "El Error Thr."
        '
        'TxtThrEl
        '
        Me.TxtThrEl.BackColor = System.Drawing.SystemColors.Control
        Me.TxtThrEl.Location = New System.Drawing.Point(1202, 617)
        Me.TxtThrEl.Name = "TxtThrEl"
        Me.TxtThrEl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtThrEl.Size = New System.Drawing.Size(51, 22)
        Me.TxtThrEl.TabIndex = 24
        '
        'CmdReset
        '
        Me.CmdReset.Location = New System.Drawing.Point(465, 557)
        Me.CmdReset.Name = "CmdReset"
        Me.CmdReset.Size = New System.Drawing.Size(75, 34)
        Me.CmdReset.TabIndex = 14
        Me.CmdReset.Text = "Clear Alarm (F8)"
        '
        'LblOffsetMaxTag
        '
        Me.LblOffsetMaxTag.Location = New System.Drawing.Point(1084, 652)
        Me.LblOffsetMaxTag.Name = "LblOffsetMaxTag"
        Me.LblOffsetMaxTag.Size = New System.Drawing.Size(94, 18)
        Me.LblOffsetMaxTag.TabIndex = 25
        Me.LblOffsetMaxTag.Text = "Offset Max"
        '
        'TxtOffsetMax
        '
        Me.TxtOffsetMax.BackColor = System.Drawing.SystemColors.Control
        Me.TxtOffsetMax.Location = New System.Drawing.Point(1202, 648)
        Me.TxtOffsetMax.Name = "TxtOffsetMax"
        Me.TxtOffsetMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtOffsetMax.Size = New System.Drawing.Size(51, 22)
        Me.TxtOffsetMax.TabIndex = 26
        '
        'LblGradiTag
        '
        Me.LblGradiTag.Location = New System.Drawing.Point(1270, 652)
        Me.LblGradiTag.Name = "LblGradiTag"
        Me.LblGradiTag.Size = New System.Drawing.Size(36, 16)
        Me.LblGradiTag.TabIndex = 27
        Me.LblGradiTag.Text = "gradi"
        '
        'LblBackupAutoTrackTag
        '
        Me.LblBackupAutoTrackTag.Location = New System.Drawing.Point(840, 681)
        Me.LblBackupAutoTrackTag.Name = "LblBackupAutoTrackTag"
        Me.LblBackupAutoTrackTag.Size = New System.Drawing.Size(157, 18)
        Me.LblBackupAutoTrackTag.TabIndex = 28
        Me.LblBackupAutoTrackTag.Text = "BACK-UP AUTOTRACK"
        '
        'LblBackUp
        '
        Me.LblBackUp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblBackUp.Location = New System.Drawing.Point(1001, 681)
        Me.LblBackUp.BackColor = System.Drawing.Color.Yellow
        Me.LblBackUp.Name = "LblBackUp"
        Me.LblBackUp.Size = New System.Drawing.Size(155, 18)
        Me.LblBackUp.TabIndex = 29
        '
        'LblPresetSwitchesTag
        '
        Me.LblPresetSwitchesTag.Location = New System.Drawing.Point(840, 707)
        Me.LblPresetSwitchesTag.Name = "LblPresetSwitchesTag"
        Me.LblPresetSwitchesTag.Size = New System.Drawing.Size(157, 18)
        Me.LblPresetSwitchesTag.TabIndex = 30
        Me.LblPresetSwitchesTag.Text = "PRESET SWITCHES"
        '
        'LblSwitches
        '
        Me.LblSwitches.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblSwitches.Location = New System.Drawing.Point(1003, 707)
        Me.LblSwitches.BackColor = System.Drawing.Color.Yellow
        Me.LblSwitches.Name = "LblSwitches"
        Me.LblSwitches.Size = New System.Drawing.Size(155, 18)
        Me.LblSwitches.TabIndex = 31
        '
        'TxtFrequency
        '
        Me.TxtFrequency.BackColor = System.Drawing.Color.Yellow
        Me.TxtFrequency.Location = New System.Drawing.Point(938, 557)
        Me.TxtFrequency.Name = "TxtFrequency"
        Me.TxtFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtFrequency.ReadOnly = True
        Me.TxtFrequency.Size = New System.Drawing.Size(80, 22)
        Me.TxtFrequency.TabIndex = 32
        Me.TxtFrequency.Visible = False
        '
        'TxtPowerOut
        '
        Me.TxtPowerOut.BackColor = System.Drawing.Color.Yellow
        Me.TxtPowerOut.Location = New System.Drawing.Point(938, 616)
        Me.TxtPowerOut.Name = "TxtPowerOut"
        Me.TxtPowerOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtPowerOut.ReadOnly = True
        Me.TxtPowerOut.Size = New System.Drawing.Size(80, 22)
        Me.TxtPowerOut.TabIndex = 33
        Me.TxtPowerOut.Visible = False
        '
        'TdbLog
        '
        Me.TdbLog.AllowUserToAddRows = False
        Me.TdbLog.AllowUserToDeleteRows = False
        Me.TdbLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TdbLog.Location = New System.Drawing.Point(17, 554)
        Me.TdbLog.Name = "TdbLog"
        Me.TdbLog.ReadOnly = True
        Me.TdbLog.RowHeadersWidth = 51
        Me.TdbLog.Size = New System.Drawing.Size(430, 140)
        Me.TdbLog.TabIndex = 13
        '
        'LblConfCNL1in
        '
        Me.LblConfCNL1in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL1in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL1in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL1in.Location = New System.Drawing.Point(19, 708)
        Me.LblConfCNL1in.Name = "LblConfCNL1in"
        Me.LblConfCNL1in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL1in.TabIndex = 34
        Me.LblConfCNL1in.Text = "ACU"
        Me.LblConfCNL1in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL3in
        '
        Me.LblConfCNL3in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL3in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL3in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL3in.Location = New System.Drawing.Point(70, 708)
        Me.LblConfCNL3in.Name = "LblConfCNL3in"
        Me.LblConfCNL3in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL3in.TabIndex = 35
        Me.LblConfCNL3in.Text = "RCV"
        Me.LblConfCNL3in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL4in
        '
        Me.LblConfCNL4in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL4in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL4in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL4in.Location = New System.Drawing.Point(121, 708)
        Me.LblConfCNL4in.Name = "LblConfCNL4in"
        Me.LblConfCNL4in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL4in.TabIndex = 36
        Me.LblConfCNL4in.Text = "D/C"
        Me.LblConfCNL4in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL6in
        '
        Me.LblConfCNL6in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL6in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL6in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL6in.Location = New System.Drawing.Point(172, 708)
        Me.LblConfCNL6in.Visible = False   ' K09: nascosto nel VB6
        Me.LblConfCNL6in.Name = "LblConfCNL6in"
        Me.LblConfCNL6in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL6in.TabIndex = 37
        Me.LblConfCNL6in.Text = "POL"
        Me.LblConfCNL6in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL2in
        '
        Me.LblConfCNL2in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL2in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL2in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL2in.Location = New System.Drawing.Point(223, 708)
        Me.LblConfCNL2in.Name = "LblConfCNL2in"
        Me.LblConfCNL2in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL2in.TabIndex = 38
        Me.LblConfCNL2in.Text = "MCS"
        Me.LblConfCNL2in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblConfCNL5in
        '
        Me.LblConfCNL5in.BackColor = System.Drawing.Color.Yellow
        Me.LblConfCNL5in.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblConfCNL5in.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblConfCNL5in.Location = New System.Drawing.Point(274, 708)
        Me.LblConfCNL5in.Name = "LblConfCNL5in"
        Me.LblConfCNL5in.Size = New System.Drawing.Size(44, 27)
        Me.LblConfCNL5in.TabIndex = 39
        Me.LblConfCNL5in.Text = "INT"
        Me.LblConfCNL5in.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(841, 563)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 16)
        Me.Label4.TabIndex = 42
        Me.Label4.Text = "Frequency out"
        Me.Label4.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(841, 615)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(68, 16)
        Me.Label5.TabIndex = 43
        Me.Label5.Text = "Power Out"
        Me.Label5.Visible = False
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(1020, 561)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(42, 16)
        Me.Label6.TabIndex = 44
        Me.Label6.Text = "MHz"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(1024, 619)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 16)
        Me.Label7.TabIndex = 45
        Me.Label7.Text = "dBm"
        Me.Label7.Visible = False
        '
        'FrmLog
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1339, 788)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LblServiceBanner)
        Me.Controls.Add(Me.LblOperationModeTag)
        Me.Controls.Add(Me.TxtMode)
        Me.Controls.Add(Me.LblPcCommandTag)
        Me.Controls.Add(Me.LblCommand)
        Me.Controls.Add(Me.LblSatelliteTag)
        Me.Controls.Add(Me.LblSatellite)
        Me.Controls.Add(Me.LblSumAlarmTag)
        Me.Controls.Add(Me.LblSumAlarm)
        Me.Controls.Add(Me.PnlGrid)
        Me.Controls.Add(Me.PnlPol)
        Me.Controls.Add(Me.PnlStatus)
        Me.Controls.Add(Me.FrmComandi)
        Me.Controls.Add(Me.TdbLog)
        Me.Controls.Add(Me.CmdReset)
        Me.Controls.Add(Me.LblFrqBeaconTag)
        Me.Controls.Add(Me.TxtFreqBeacon)
        Me.Controls.Add(Me.LblMHzTag1)
        Me.Controls.Add(Me.LblAzErrThrTag)
        Me.Controls.Add(Me.TxtThrAz)
        Me.Controls.Add(Me.LblAutoTrackThrTag)
        Me.Controls.Add(Me.TxtSogliaAuto)
        Me.Controls.Add(Me.LblDbmTag1)
        Me.Controls.Add(Me.LblElErrThrTag)
        Me.Controls.Add(Me.TxtThrEl)
        Me.Controls.Add(Me.LblOffsetMaxTag)
        Me.Controls.Add(Me.TxtOffsetMax)
        Me.Controls.Add(Me.LblGradiTag)
        Me.Controls.Add(Me.LblBackupAutoTrackTag)
        Me.Controls.Add(Me.LblBackUp)
        Me.Controls.Add(Me.LblPresetSwitchesTag)
        Me.Controls.Add(Me.LblSwitches)
        Me.Controls.Add(Me.TxtFrequency)
        Me.Controls.Add(Me.TxtPowerOut)
        Me.Controls.Add(Me.LblConfCNL1in)
        Me.Controls.Add(Me.LblConfCNL3in)
        Me.Controls.Add(Me.LblConfCNL4in)
        Me.Controls.Add(Me.LblConfCNL6in)
        Me.Controls.Add(Me.LblConfCNL2in)
        Me.Controls.Add(Me.LblConfCNL5in)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FrmLog"
        Me.Text = "Service"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.PnlGrid.ResumeLayout(False)
        Me.PnlGrid.PerformLayout()
        Me.PnlPol.ResumeLayout(False)
        Me.PnlPol.PerformLayout()
        Me.PnlStatus.ResumeLayout(False)
        Me.PnlStatus.PerformLayout()
        Me.FrmComandi.ResumeLayout(False)
        CType(Me.TdbLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TmrAggiorna As System.Windows.Forms.Timer
    Friend WithEvents TmrAlarm As System.Windows.Forms.Timer

    Friend LblServiceBanner As System.Windows.Forms.Label
    Friend LblOperationModeTag As System.Windows.Forms.Label
    Friend TxtMode As System.Windows.Forms.TextBox
    Friend LblPcCommandTag As System.Windows.Forms.Label
    Friend LblCommand As System.Windows.Forms.Label
    Friend LblSatelliteTag As System.Windows.Forms.Label
    Friend LblSatellite As System.Windows.Forms.Label
    Friend LblSumAlarmTag As System.Windows.Forms.Label
    Friend LblSumAlarm As System.Windows.Forms.Label

    Friend PnlGrid As System.Windows.Forms.Panel
    Friend LblHdrAntenna As System.Windows.Forms.Label
    Friend LblHdrCommand As System.Windows.Forms.Label
    Friend LblHdrOffset As System.Windows.Forms.Label
    Friend LblHdrSpeed As System.Windows.Forms.Label
    Friend LblHdrPlHw As System.Windows.Forms.Label
    Friend LblHdrLimitHw As System.Windows.Forms.Label
    Friend LblHdrPlSw As System.Windows.Forms.Label
    Friend LblHdrBrk As System.Windows.Forms.Label
    Friend LblHdrMajorF As System.Windows.Forms.Label
    Friend LblHdrMinorF As System.Windows.Forms.Label
    Friend LblAzTag As System.Windows.Forms.Label
    Friend TxtAz As System.Windows.Forms.TextBox
    Friend LblAzimuth As System.Windows.Forms.Label
    Friend LblAzError As System.Windows.Forms.Label
    Friend LblAzSpeed As System.Windows.Forms.Label
    Friend LblAzSpeedUnit As System.Windows.Forms.Label
    Friend LblAllarme1 As System.Windows.Forms.Label
    Friend LblAllarme2 As System.Windows.Forms.Label
    Friend LblAllarme3 As System.Windows.Forms.Label
    Friend LblAllarme4 As System.Windows.Forms.Label
    Friend LblAzTBrk As System.Windows.Forms.Label
    Friend LblAzMaFault As System.Windows.Forms.Label
    Friend LblAzMiFault As System.Windows.Forms.Label
    Friend LblElTag As System.Windows.Forms.Label
    Friend TxtEl As System.Windows.Forms.TextBox
    Friend LblElevazione As System.Windows.Forms.Label
    Friend LblElError As System.Windows.Forms.Label
    Friend LblElSpeed As System.Windows.Forms.Label
    Friend LblElSpeedUnit As System.Windows.Forms.Label
    Friend LblAllarme5 As System.Windows.Forms.Label
    Friend LblAllarme6 As System.Windows.Forms.Label
    Friend LblAllarme7 As System.Windows.Forms.Label
    Friend LblAllarme8 As System.Windows.Forms.Label
    Friend LblElTBrk As System.Windows.Forms.Label
    Friend LblElMaFault As System.Windows.Forms.Label
    Friend LblElMiFault As System.Windows.Forms.Label
    Friend LblAzStow As System.Windows.Forms.Label
    Friend LblAzAligned As System.Windows.Forms.Label
    Friend LblElStow As System.Windows.Forms.Label
    Friend LblElAligned As System.Windows.Forms.Label
    Friend LblElHand As System.Windows.Forms.Label

    Friend PnlPol As System.Windows.Forms.Panel
    Friend LblPolTag As System.Windows.Forms.Label
    Friend TxtPol As System.Windows.Forms.Label
    Friend LblPol As System.Windows.Forms.Label
    Friend LblPol1 As System.Windows.Forms.TextBox
    Friend LblLNATag As System.Windows.Forms.Label
    Friend LblLNA As System.Windows.Forms.Label
    Friend TxtPolErr As System.Windows.Forms.TextBox
    Friend LblPolFault As System.Windows.Forms.Label
    Friend LblAllarme16 As System.Windows.Forms.Label
    Friend LblAllarme17 As System.Windows.Forms.Label
    Friend LblAllarme18 As System.Windows.Forms.Label

    Friend PnlStatus As System.Windows.Forms.Panel
    Friend LblMCTag As System.Windows.Forms.Label
    Friend TxtACU As System.Windows.Forms.Label
    Friend LblSumTag As System.Windows.Forms.Label
    Friend TxtLev As System.Windows.Forms.Label
    Friend LblVTagSum As System.Windows.Forms.Label
    Friend LblAzErTag As System.Windows.Forms.Label
    Friend TxtAzEr As System.Windows.Forms.TextBox
    Friend LblVTagAzEr As System.Windows.Forms.Label
    Friend LblElErTag As System.Windows.Forms.Label
    Friend TxtElEr As System.Windows.Forms.TextBox
    Friend LblVTagElEr As System.Windows.Forms.Label
    Friend LblRcvStatusTag As System.Windows.Forms.Label
    Friend LblLock As System.Windows.Forms.Label
    Friend LblBcnLevelTag As System.Windows.Forms.Label
    Friend LblLev As System.Windows.Forms.Label
    Friend LblAllarme11 As System.Windows.Forms.Label
    Friend LblAllarme10 As System.Windows.Forms.Label
    Friend LblAllarme9 As System.Windows.Forms.Label
    Friend LblEmStopTag As System.Windows.Forms.Label
    Friend LblEmergency As System.Windows.Forms.Label
    Friend LblEmergency1 As System.Windows.Forms.Label
    Friend LblMduTag As System.Windows.Forms.Label
    Friend LblAzDR As System.Windows.Forms.Label
    Friend LblPCU As System.Windows.Forms.Label
    Friend LblLowAngleTag As System.Windows.Forms.Label
    Friend LblLowAngle As System.Windows.Forms.Label
    Friend LblMemoryTag As System.Windows.Forms.Label
    Friend LblAllarme12 As System.Windows.Forms.Label
    Friend LblMemDataTag As System.Windows.Forms.Label
    Friend LblAllarme13 As System.Windows.Forms.Label
    Friend LblSatDataTag As System.Windows.Forms.Label
    Friend LblAllarme14 As System.Windows.Forms.Label
    Friend LblStowUnstowTag As System.Windows.Forms.Label
    Friend LblAllarme15 As System.Windows.Forms.Label
    Friend LblWarning As System.Windows.Forms.Label

    Friend FrmComandi As System.Windows.Forms.GroupBox
    Friend WithEvents CmdSTBY As System.Windows.Forms.Button
    Friend WithEvents CmdCmdPos As System.Windows.Forms.Button
    Friend WithEvents CmdStepTrack As System.Windows.Forms.Button
    Friend WithEvents CmdAutoTrack As System.Windows.Forms.Button
    Friend WithEvents CmdAugmented As System.Windows.Forms.Button
    Friend WithEvents CmdProgramTrack As System.Windows.Forms.Button
    Friend WithEvents CmdOffset As System.Windows.Forms.Button
    Friend WithEvents CmdAutoSearch As System.Windows.Forms.Button
    Friend LblStepTrack As System.Windows.Forms.Label

    Friend WithEvents TdbLog As System.Windows.Forms.DataGridView
    Friend WithEvents CmdReset As System.Windows.Forms.Button

    Friend LblFrqBeaconTag As System.Windows.Forms.Label
    Friend TxtFreqBeacon As System.Windows.Forms.TextBox
    Friend LblMHzTag1 As System.Windows.Forms.Label
    Friend LblAzErrThrTag As System.Windows.Forms.Label
    Friend TxtThrAz As System.Windows.Forms.TextBox
    Friend LblAutoTrackThrTag As System.Windows.Forms.Label
    Friend TxtSogliaAuto As System.Windows.Forms.TextBox
    Friend LblDbmTag1 As System.Windows.Forms.Label
    Friend LblElErrThrTag As System.Windows.Forms.Label
    Friend TxtThrEl As System.Windows.Forms.TextBox
    Friend LblOffsetMaxTag As System.Windows.Forms.Label
    Friend TxtOffsetMax As System.Windows.Forms.TextBox
    Friend LblGradiTag As System.Windows.Forms.Label
    Friend LblBackupAutoTrackTag As System.Windows.Forms.Label
    Friend LblBackUp As System.Windows.Forms.Label
    Friend LblPresetSwitchesTag As System.Windows.Forms.Label
    Friend LblSwitches As System.Windows.Forms.Label
    Friend TxtFrequency As System.Windows.Forms.TextBox
    Friend TxtPowerOut As System.Windows.Forms.TextBox

    Friend LblConfCNL1in As System.Windows.Forms.Label
    Friend LblConfCNL2in As System.Windows.Forms.Label
    Friend LblConfCNL3in As System.Windows.Forms.Label
    Friend LblConfCNL4in As System.Windows.Forms.Label
    Friend LblConfCNL5in As System.Windows.Forms.Label
    Friend LblConfCNL6in As System.Windows.Forms.Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents MnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuConfiguration As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuCommand As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStandBy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStepTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemoryTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemInterpStep As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemRecEnable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemRecDisable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuProgramTrack As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSinusoidal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPreset As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPresetAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPresetAz As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPresetEl As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPresetPol As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuResetACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteCmd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuUnstow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuBackupPT As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuCommandFree As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuRequest As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuReqStatus As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuReqParameter As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuMemoryTrackData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuStowUnstowData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuGraphic As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuAntenna As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSatelliteMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuEphemeris As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuParameterTable As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuParameterACU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuParameterArchive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPhase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuInfo As System.Windows.Forms.ToolStripMenuItem
End Class
