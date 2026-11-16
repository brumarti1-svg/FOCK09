<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
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
        Me.TmrMsg = New System.Windows.Forms.Timer(Me.components)
        Me.TmrOrologio = New System.Windows.Forms.Timer(Me.components)
        Me.TmrBeep = New System.Windows.Forms.Timer(Me.components)
        Me.TmrPhase = New System.Windows.Forms.Timer(Me.components)
        Me.StbMain1 = New System.Windows.Forms.StatusStrip()
        Me.stpMessaggio = New System.Windows.Forms.ToolStripStatusLabel()
        Me.stpPrint = New System.Windows.Forms.ToolStripStatusLabel()
        Me.stpBeep = New System.Windows.Forms.ToolStripStatusLabel()
        Me.stpDisco = New System.Windows.Forms.ToolStripStatusLabel()
        Me.stpData = New System.Windows.Forms.ToolStripStatusLabel()
        Me.stpProgram = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StbMain1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TmrMsg
        '
        Me.TmrMsg.Interval = 45000
        '
        'TmrOrologio
        '
        Me.TmrOrologio.Interval = 50
        '
        'TmrBeep
        '
        Me.TmrBeep.Interval = 500
        '
        'TmrPhase
        '
        Me.TmrPhase.Interval = 400
        '
        'StbMain1
        '
        Me.StbMain1.AutoSize = False
        Me.StbMain1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.StbMain1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StbMain1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.stpMessaggio, Me.stpPrint, Me.stpBeep, Me.stpDisco, Me.stpData, Me.stpProgram})
        Me.StbMain1.Location = New System.Drawing.Point(0, 522)
        Me.StbMain1.Name = "StbMain1"
        Me.StbMain1.Size = New System.Drawing.Size(1431, 35)
        Me.StbMain1.TabIndex = 0
        '
        'stpMessaggio
        '
        Me.stpMessaggio.AutoSize = False
        Me.stpMessaggio.Name = "stpMessaggio"
        Me.stpMessaggio.Size = New System.Drawing.Size(700, 29)
        '
        'stpPrint
        '
        Me.stpPrint.AutoSize = False
        Me.stpPrint.Name = "stpPrint"
        Me.stpPrint.Size = New System.Drawing.Size(130, 29)
        Me.stpPrint.Visible = False
        '
        'stpBeep
        '
        Me.stpBeep.AutoSize = False
        Me.stpBeep.Name = "stpBeep"
        Me.stpBeep.Size = New System.Drawing.Size(130, 29)
        '
        'stpDisco
        '
        Me.stpDisco.AutoSize = False
        Me.stpDisco.IsLink = True
        Me.stpDisco.Name = "stpDisco"
        Me.stpDisco.Size = New System.Drawing.Size(195, 29)
        '
        'stpData
        '
        Me.stpData.AutoSize = False
        Me.stpData.Name = "stpData"
        Me.stpData.Size = New System.Drawing.Size(490, 29)
        '
        'stpProgram
        '
        Me.stpProgram.AutoSize = False
        Me.stpProgram.Name = "stpProgram"
        Me.stpProgram.Size = New System.Drawing.Size(435, 29)
        '
        'FrmMain
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1431, 557)
        Me.Controls.Add(Me.StbMain1)
        Me.IsMdiContainer = True
        Me.Name = "FrmMain"
        Me.Text = "Interfaccia Grafica C-10"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.StbMain1.ResumeLayout(False)
        Me.StbMain1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TmrMsg      As System.Windows.Forms.Timer
    Friend WithEvents TmrOrologio As System.Windows.Forms.Timer
    Friend WithEvents TmrBeep     As System.Windows.Forms.Timer
    Friend WithEvents TmrPhase    As System.Windows.Forms.Timer
    Friend StbMain1               As System.Windows.Forms.StatusStrip
    Friend stpMessaggio           As System.Windows.Forms.ToolStripStatusLabel
    Friend stpPrint               As System.Windows.Forms.ToolStripStatusLabel
    Friend stpBeep                As System.Windows.Forms.ToolStripStatusLabel
    Friend stpDisco               As System.Windows.Forms.ToolStripStatusLabel
    Friend stpData                As System.Windows.Forms.ToolStripStatusLabel
    Friend stpProgram             As System.Windows.Forms.ToolStripStatusLabel

End Class
