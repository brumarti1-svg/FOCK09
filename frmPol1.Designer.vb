<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmPol1
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
        Me.LblPolarization = New System.Windows.Forms.Label()
        Me.MskElevazione = New System.Windows.Forms.TextBox()
        Me.CmdDown = New System.Windows.Forms.Button()
        Me.CmdUp = New System.Windows.Forms.Button()
        Me.LblType = New System.Windows.Forms.Label()
        Me.LblLegenda = New System.Windows.Forms.Label()
        Me.CmbMode = New System.Windows.Forms.ComboBox()
        Me.CmdOK = New System.Windows.Forms.Button()
        Me.LblStepIncrement = New System.Windows.Forms.Label()
        Me.CmbInc = New System.Windows.Forms.ComboBox()
        Me.CmdAnnulla = New System.Windows.Forms.Button()
        Me.TxtFreccia = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'LblPolarization
        '
        Me.LblPolarization.AutoSize = True
        Me.LblPolarization.Location = New System.Drawing.Point(10, 13)
        Me.LblPolarization.Name = "LblPolarization"
        Me.LblPolarization.Text = "Polarization"
        '
        'MskElevazione
        '
        Me.MskElevazione.Location = New System.Drawing.Point(150, 10)
        Me.MskElevazione.Name = "MskElevazione"
        Me.MskElevazione.Size = New System.Drawing.Size(70, 22)
        Me.MskElevazione.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CmdDown
        '
        Me.CmdDown.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdDown.Location = New System.Drawing.Point(150, 40)
        Me.CmdDown.Name = "CmdDown"
        Me.CmdDown.Size = New System.Drawing.Size(30, 28)
        Me.CmdDown.Text = "-"
        '
        'CmdUp
        '
        Me.CmdUp.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdUp.Location = New System.Drawing.Point(190, 40)
        Me.CmdUp.Name = "CmdUp"
        Me.CmdUp.Size = New System.Drawing.Size(30, 28)
        Me.CmdUp.Text = "+"
        '
        'LblType
        '
        Me.LblType.AutoSize = True
        Me.LblType.Location = New System.Drawing.Point(10, 80)
        Me.LblType.Name = "LblType"
        Me.LblType.Visible = False   ' K09: nessun modo, solo angolo (PLP all'ACU)
        Me.LblType.Text = "Mode"
        '
        'CmbMode
        '
        Me.CmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbMode.Items.AddRange(New Object() {"S", "L", "C"})
        Me.CmbMode.Location = New System.Drawing.Point(120, 77)
        Me.CmbMode.Name = "CmbMode"
        Me.CmbMode.Visible = False   ' K09: nessun modo, solo angolo (PLP all'ACU)
        Me.CmbMode.Size = New System.Drawing.Size(50, 23)
        '
        'LblLegenda
        '
        Me.LblLegenda.AutoSize = True
        Me.LblLegenda.Location = New System.Drawing.Point(180, 80)
        Me.LblLegenda.Name = "LblLegenda"
        Me.LblLegenda.Visible = False   ' K09: nessun modo, solo angolo (PLP all'ACU)
        Me.LblLegenda.Text = "S Stand-By   L Linear   C Circular"
        '
        'CmdOK
        '
        Me.CmdOK.Location = New System.Drawing.Point(80, 112)
        Me.CmdOK.Name = "CmdOK"
        Me.CmdOK.Size = New System.Drawing.Size(90, 28)
        Me.CmdOK.Text = "Execute"
        '
        'LblStepIncrement
        '
        Me.LblStepIncrement.AutoSize = True
        Me.LblStepIncrement.Location = New System.Drawing.Point(10, 152)
        Me.LblStepIncrement.Name = "LblStepIncrement"
        Me.LblStepIncrement.Text = "Step Increment"
        '
        'CmbInc
        '
        Me.CmbInc.Items.AddRange(New Object() {"0.1", "0.5", "1.0"})
        Me.CmbInc.Location = New System.Drawing.Point(150, 149)
        Me.CmbInc.Name = "CmbInc"
        Me.CmbInc.Size = New System.Drawing.Size(65, 23)
        '
        'CmdAnnulla
        '
        Me.CmdAnnulla.Location = New System.Drawing.Point(85, 185)
        Me.CmdAnnulla.Name = "CmdAnnulla"
        Me.CmdAnnulla.Size = New System.Drawing.Size(70, 28)
        Me.CmdAnnulla.Text = "Exit"
        '
        'TxtFreccia
        '
        Me.TxtFreccia.Location = New System.Drawing.Point(10, 188)
        Me.TxtFreccia.Name = "TxtFreccia"
        Me.TxtFreccia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TxtFreccia.Size = New System.Drawing.Size(20, 22)
        Me.TxtFreccia.TabStop = False
        '
        'FrmPol1
        '
        Me.ClientSize = New System.Drawing.Size(400, 230)
        Me.ControlBox = False
        Me.Controls.Add(Me.LblPolarization)
        Me.Controls.Add(Me.MskElevazione)
        Me.Controls.Add(Me.CmdDown)
        Me.Controls.Add(Me.CmdUp)
        Me.Controls.Add(Me.LblType)
        Me.Controls.Add(Me.CmbMode)
        Me.Controls.Add(Me.LblLegenda)
        Me.Controls.Add(Me.CmdOK)
        Me.Controls.Add(Me.LblStepIncrement)
        Me.Controls.Add(Me.CmbInc)
        Me.Controls.Add(Me.CmdAnnulla)
        Me.Controls.Add(Me.TxtFreccia)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "FrmPol1"
        Me.Text = "               Polarization Position"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents LblPolarization As System.Windows.Forms.Label
    Friend WithEvents MskElevazione As System.Windows.Forms.TextBox
    Friend WithEvents CmdDown As System.Windows.Forms.Button
    Friend WithEvents CmdUp As System.Windows.Forms.Button
    Friend WithEvents LblType As System.Windows.Forms.Label
    Friend WithEvents CmbMode As System.Windows.Forms.ComboBox
    Friend WithEvents LblLegenda As System.Windows.Forms.Label
    Friend WithEvents CmdOK As System.Windows.Forms.Button
    Friend WithEvents LblStepIncrement As System.Windows.Forms.Label
    Friend WithEvents CmbInc As System.Windows.Forms.ComboBox
    Friend WithEvents CmdAnnulla As System.Windows.Forms.Button
    Friend WithEvents TxtFreccia As System.Windows.Forms.TextBox

End Class
