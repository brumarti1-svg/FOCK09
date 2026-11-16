' ============================================================
' frmPol1.vb — convertito da ComuniIntelsatImmersatnew\frmPol1.frm (VB6, usato da K09)
' Telespazio FOC-K09 — DYNATRON
' UI di comando del polarizzatore, aperta dal bottone "POL" su
' frmRicezione.
'
' Differenze rispetto a C10 (da cui è partito il porting): K09 non
' usa la PCU; l'angolo va all'ACU con il messaggio "PLP <angolo>"
' incorniciato da CheckSum, senza modo (niente S/L/C: Mode e legenda
' nascosti), e viene memorizzato in Satelliti.PolAngle del satellite
' corrente e in g_ValPol (C03 invece lo salva nella fase attiva).
' I tasti +/- e le frecce cambiano solo il valore a video: il comando parte con OK (nel VB6 comune il cmdOK_Click in
' coda a cmdUp/cmdDown è commentato). Un decimale, ±99.9.
' Il valore corrente arriva dall'ACU in s_POL ("+123" = +12.3°).
'
' Separatore decimale: l'angolo si formatta sempre con il punto
' (InvariantCulture), come si aspetta il protocollo ACU. L'input
' dell'operatore accetta sia "." sia ",".
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
        frmRicezione.tmrTimeoutMSG(ACU).Enabled = False
        frmRicezione.tmrTimeoutMSG(ACU).Enabled = True
        Me.KeyPreview = False
        Me.Close()
    End Sub

    Private Sub CmdOK_Click(sender As Object, e As EventArgs) Handles CmdOK.Click
        Dim angolo As Double
        If Not LeggiAngolo(MskElevazione.Text, angolo) OrElse Math.Abs(angolo) > 99.9 Then
            MskElevazione.Text = ""
            Return
        End If

        ' Angolo nel catalogo, per il satellite corrente (VB6 comune)
        Try
            Dim rs2 = dbOrbite.OpenRecordset("SELECT * FROM [Satelliti] order by NomeSatellite", dbOpenDynaset)
            rs2.FindFirst("NomeSatellite ='" & s_Satellite & "'")
            If Not rs2.NoMatch Then
                rs2.Edit()
                rs2!PolAngle = angolo
                rs2.Update()
            End If
            rs2.Close()
        Catch ex As Exception
            WriteError(0, "frmPol1 - Satelliti: " & ex.Message)
        End Try

        b_Ricezione(ACU) = False
        b_int(ACU) = True
        i_Cont = 2
        Attesa(0.2)

        MskElevazione.Text = FormattaAngolo(angolo)
        Dim s_program As String = "PLP " & MskElevazione.Text
        g_ValPol = MskElevazione.Text
        Dim lunghezza As Integer = Len(s_program) + 6
        frmRicezione.tmrTimeoutMSG(ACU).Enabled = False
        SEND_ACU = CheckSum(lunghezza.ToString("00") & s_program)
        InvioACU(SEND_ACU)
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
        If PolCorrente(corrente) Then
            MskElevazione.Text = FormattaAngolo(corrente)
        Else
            MskElevazione.Text = "+00.0"                 ' VB6 comune
        End If
        CmbInc.Text = "0.1"
        TxtFreccia.Focus()
        Me.KeyPreview = True
    End Sub

    ' +/- : cambiano solo il valore a video, l'invio è con OK (K09).
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
