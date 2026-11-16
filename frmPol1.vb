' ============================================================
' frmPol1.vb — convertito da Prog0347-FOCC10\frmPol1.frm (VB6)
' Telespazio FOC-C10 — DYNATRON
' UI di comando del polarizzatore, aperta dal bottone "POL" su
' frmRicezione.
'
' Differenza genuina rispetto a FOC6: FOC6 comandava il
' polarizzatore via SNMP (frmManager) con tipo Linear/Circular e
' due decimali; C10 lo comanda sulla seriale del canale PCU con il
' messaggio "Z <modo><angolo>" incorniciato da CheckSum, modo
' S (Stand-By) / L (Linear) / C (Circular), un decimale (±99.9).
' Il valore corrente arriva dall'ACU in s_POL ("+123" = +12.3°).
'
' Separatore decimale: il VB6 usava Format$, che segue il locale
' di Windows (su un PC it-IT avrebbe inviato "12,3"); qui l'angolo
' si formatta sempre con il punto (InvariantCulture), come si
' aspetta il protocollo seriale. L'input dell'operatore si legge
' con Val (accetta solo il punto) dopo aver sostituito l'eventuale
' virgola.
' ============================================================

Imports System.Globalization

Public Class FrmPol1

    ' ── Variabili (erano Dim a livello di modulo in VB6) ──────
    Private ElPro As Double
    Private PTKold As Integer
    Private i_StatusOld As Integer

    ''' <summary>Legge l'angolo digitato accettando sia "." sia ",".</summary>
    Private Function LeggiAngolo(testo As String, ByRef valore As Double) As Boolean
        Dim t As String = testo.Trim().Replace(",", ".")
        If t = "" OrElse t = "." OrElse t = "+" OrElse t = "-" Then Return False
        Return Double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, valore)
    End Function

    ''' <summary>Angolo con segno esplicito e un decimale: "+12.3" / "-05.0".</summary>
    Private Shared Function FormattaAngolo(valore As Double) As String
        Dim segno As String = If(valore < 0, "-", "+")
        Return segno & Math.Abs(valore).ToString("00.0", CultureInfo.InvariantCulture)
    End Function

    ''' <summary>Valore corrente dall'ACU (s_POL = decimi di grado con segno), se valido.</summary>
    Private Shared Function PolCorrente(ByRef valore As Double) As Boolean
        Dim decimi As Double
        If String.IsNullOrWhiteSpace(s_POL) Then Return False
        If Not Double.TryParse(s_POL.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, decimi) Then Return False
        valore = decimi / 10
        Return True
    End Function

    Private Sub CmdAnnulla_Click(sender As Object, e As EventArgs) Handles CmdAnnulla.Click
        i_Inter = CSng(Val(LeggiFileIni("Canale1", "Inter")))
        i_Cont = 1
        frmRicezione.tmrTimeoutMSG(PCU).Enabled = False
        frmRicezione.tmrTimeoutMSG(PCU).Enabled = True
        Me.KeyPreview = False
        Me.Close()
    End Sub

    Private Sub CmdOK_Click(sender As Object, e As EventArgs) Handles CmdOK.Click
        d_Pol = CmbMode.Text

        Dim angolo As Double
        If Not LeggiAngolo(MskElevazione.Text, angolo) OrElse Math.Abs(angolo) > 99.9 Then
            MskElevazione.Text = ""
            Return
        End If

        b_int(PCU) = True
        b_Ricezione(PCU) = True
        MskElevazione.Text = FormattaAngolo(angolo)
        Dim s_program As String = "Z " & CmbMode.Text & MskElevazione.Text
        g_ValPol = MskElevazione.Text
        Dim lunghezza As Integer = Len(s_program) + 6
        frmRicezione.tmrTimeoutMSG(PCU).Enabled = False
        Dim s_Pola As String = CheckSum(lunghezza.ToString("00") & s_program)
        Invio(s_Pola, PCU)
        Me.KeyPreview = True
        TxtFreccia.Focus()
    End Sub

    Private Sub FrmPol1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CmbInc.Text = "0.1"
        PTKold = PTK
        PTK = 0
        d_DataInizioProg = ""
        FrmMain.stpProgram.Text = d_DataInizioProg
        i_StatusOld = i_Status
        i_Status = OP_COMMANDPOSITION
        frmRicezione.LblCommand.Text = "Polarization Position"
        Me.KeyPreview = True
        i_Cont = 0
    End Sub

    Private Sub FrmPol1_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Dim corrente As Double
        If PolCorrente(corrente) Then MskElevazione.Text = FormattaAngolo(corrente)
        CmbInc.Text = "0.1"
        If CmbMode.Items.Contains(d_Pol) Then CmbMode.Text = d_Pol
        TxtFreccia.Focus()
        Me.KeyPreview = True
    End Sub

    ' +/- : nel VB6 di C10 ogni passo invia subito il comando
    ' (cmdOK_Click in coda a cmdUp/cmdDown), a differenza di FOC6.
    Private Sub CmdDown_Click(sender As Object, e As EventArgs) Handles CmdDown.Click
        Passo(-1)
    End Sub

    Private Sub CmdUp_Click(sender As Object, e As EventArgs) Handles CmdUp.Click
        Passo(+1)
    End Sub

    Private Sub Passo(verso As Integer)
        Me.KeyPreview = True
        Dim base As Double, incremento As Double
        If Not LeggiAngolo(MskElevazione.Text, base) AndAlso Not PolCorrente(base) Then
            TxtFreccia.Focus()
            Return
        End If
        If Not LeggiAngolo(CmbInc.Text, incremento) Then incremento = 0.1
        ElPro = base + verso * incremento
        If ElPro > 99.9 Then ElPro = 99.9
        If ElPro < -99.9 Then ElPro = -99.9
        MskElevazione.Text = FormattaAngolo(ElPro)
        CmdOK_Click(Nothing, EventArgs.Empty)
    End Sub

    ' Freccia Su/Giù inoltrata ai bottoni +/- — sia dal form (focus
    ' su un altro controllo) sia da TxtFreccia (focus-catcher dedicato,
    ' stessa identica gestione nel VB6 originale).
    Private Sub FrmPol1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, TxtFreccia.KeyDown
        If e.KeyCode = Keys.Up Then
            CmdUp_Click(Nothing, EventArgs.Empty)
            e.Handled = True
        ElseIf e.KeyCode = Keys.Down Then
            CmdDown_Click(Nothing, EventArgs.Empty)
            e.Handled = True
        End If
    End Sub

End Class
