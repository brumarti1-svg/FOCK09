' ============================================================
' frmRicezione.vb â€” Form principale BTS1 DYNATRON
' Convertito da frmRicezione.frm (VB6/DAO) â†’ SQLite/.NET
' Struttura partial class:
'   frmRicezione.vb           â€” Load, Activate, LeggiParametri, pulsanti, menu, satellite
'   frmRicezione.Designer.vb  â€” InitializeComponent
'   frmRicezione.Visualizzazione.vb â€” aggiornamento display Az/El, canali, beacon
'   frmRicezione.Socket.vb    â€” eventi socket / SerialPort
'   frmRicezione.Protocollo.vb â€” ProtMAC, ProtDC, VerDati
' ============================================================

Partial Public Class FrmRicezione
    Inherits System.Windows.Forms.Form

    ' -------------------------------------------------------
    ' Variabili di istanza (erano Dim a livello form VB6)
    ' -------------------------------------------------------
    Private i_Blink As Integer
    Private Az_prov As Double, El_Prov As Double
    Private Const Lungh As Integer = 98
    Private rx_Buffer As String
    Private d_Valore As String, tmpSQL As String
    Private rs2 As Object
    Private julian_epoch1 As Double, b_Check As Boolean, s_url As String
    Private b_DatiAntenna As Boolean = False
    Private i As Integer, lun As Integer
    ' Vedi FrmRicezione_Shown: durante il toggle Normal->Maximized per il
    ' merge del menu MDI, FrmRicezione_Resize non deve richiamare
    ' FormResizer.Resize (riattiverebbe il ridisegno mostrando per un
    ' istante il layout alla dimensione "Normal" non ancora riscalato).
    Private b_SospendiResize As Boolean = False

    ' Contatori connessioni socket
    Friend intSockCnt As Integer
    Friend intSock1Cnt As Integer

    ' -------------------------------------------------------
    ' Alias per controlli rinominati nel Designer
    ' (mantenuti per compatibilitÃ  con il codice VB6 migrato)
    ' -------------------------------------------------------
    Friend ReadOnly Property BarBeacon As ProgressBar
        Get
            Return BarBeaconCtrl
        End Get
    End Property
    Friend ReadOnly Property CmbBcnFrequency As ComboBox
        Get
            Return CmbBcnFreq
        End Get
    End Property
    Friend ReadOnly Property CmdLockBell As Button
        Get
            Return CmdLockBell_ctrl
        End Get
    End Property
    Friend ReadOnly Property LblBcnMax As TextBox
        Get
            Return LblBcnMax_ctrl
        End Get
    End Property
    Friend ReadOnly Property LblBcnMin As TextBox
        Get
            Return LblBcnMin_ctrl
        End Get
    End Property
    Friend ReadOnly Property LblPC1 As Label
        Get
            Return LblPC1_ctrl
        End Get
    End Property
    Friend ReadOnly Property TxtMode As TextBox
        Get
            Return TxtMode_ctrl
        End Get
    End Property

    ' -------------------------------------------------------
    ' Controlli ricevitore â€” dichiarati qui perchÃ© non generati
    ' dal Designer (ereditÃ  della struttura VB6 originale)
    ' -------------------------------------------------------
    ' LblPol1, TxtPolErr, LblPol, TxtPol, LblPolRic, TxtFrqBeacon, TxtLna,
    ' TxtPhase, TxtPhAz, TxtRevAz, LblLNA, SS_Intestazione ora dichiarati e
    ' posizionati nel Designer (frmRicezione.Designer.vb)
    ' Timer array (indicizzato per canale: ACU=1, MAC=2, RX=3, GRF=4)
    Friend TmrTimeoutMSG() As System.Windows.Forms.Timer

    ' -------------------------------------------------------
    ' Le connessioni TCP reali (ACU/MAC/RX/GRF) sono gestite da
    ' SocketManager.vb — i campi sckMain/sckMac/sckRX/sckDC e
    ' streamMain/streamMac/streamRX/streamDC/bufMain/bufMac/bufRX/bufDC
    ' che stavano qui appartenevano alla vecchia implementazione parallela
    ' in Socket.vb (ConnettiTCP/CallbackRicezioneXXX), mai chiamata da
    ' nessuna parte e rimossa.
    ' -------------------------------------------------------

    ' -------------------------------------------------------
    ' Costruttore
    ' -------------------------------------------------------
    Public Sub New()
        InitializeComponent()
        KeyPreview = True

        ' Timer array canali (0..NumCom)
        ReDim TmrTimeoutMSG(NumCom)
        For idx As Integer = 0 To NumCom
            TmrTimeoutMSG(idx) = New System.Windows.Forms.Timer() With {.Interval = 30000, .Enabled = False}
            AddHandler TmrTimeoutMSG(idx).Tick, AddressOf TmrTimeoutMSG_Tick
        Next

        ' Associa a LblSatellite (ComboBox nel Designer)
        AddHandler LblSatellite.SelectedIndexChanged, AddressOf LblSatellite_SelectedIndexChanged
    End Sub

    ''' <summary>
    ''' Timeout di un canale — porting di tmrTimeoutMSG_Timer dal VB6 di C10.
    ''' Tutto indicizzato per canale. Seriale: la porta viene riaperta a
    ''' ogni timeout (PortReset). ACU: b_Ricezione torna False (necessario
    ''' per la ripresa del polling di stato), dopo 4 timeout spia di stop,
    ''' b_Parametri, TagDato e data ACU azzerati. GRF/PCU: spia di stop e
    ''' messaggio dopo 4 timeout consecutivi. MCS e INTELSAT: subito.
    ''' Differenza voluta dal VB6: un canale LAN (ACU/GRF) dopo 4 timeout
    ''' viene riconnesso (LanResetAsync); il VB6 di C10 non lo faceva e una
    ''' LAN caduta non si sarebbe più ripresa.
    ''' </summary>
    Private Sub TmrTimeoutMSG_Tick(sender As Object, e As EventArgs)
        Try
            Dim tmr As Timer = TryCast(sender, Timer)
            If tmr Is Nothing OrElse TmrTimeoutMSG Is Nothing Then Exit Sub
            Dim Index As Integer = Array.IndexOf(TmrTimeoutMSG, tmr)
            If Index < 1 OrElse Index > NumCom Then Exit Sub
#If SIMULAZIONE Then
            ' ACU simulata: i frame li inietta FrmMain.TmrOrologio_Tick circa ogni
            ' 0.5-0.7 s, più del TimeOut di [Canale1] (0.2 s su C10, 0.4 su FOC6,
            ' tarato sulla risposta dell'ACU vera al polling). Il timeout scattava
            ' tra un frame e l'altro e rimetteva il canale in STOP/FAIL: Spiral
            ' Acquire segnalava "ACU non in linea" ogni pochi secondi.
            If Index = ACU Then Exit Sub
#End If

            Dim spie As Label() = {Nothing, LblConfCNL1in, LblConfCNL2in, LblConfCNL3in, LblConfCNL4in, LblConfCNL5in, LblConfCNL6in}
            Dim seriale As Boolean = Port_Control(Index).Socket <> "S"

            If Port_Control(Index).Port_State = PCS_NOT_OP Then
                If Index <> RX Then
                    spie(Index).BackColor = PCS_STOP_COL
                    spie(Index).ForeColor = Color.Black
                End If
                TmrTimeoutMSG(Index).Enabled = True
                Exit Sub
            End If

            Port_Control(Index).Buffer = ""
            Port_Control(Index).Rec_State = PCR_WAIT
            If seriale Then FrmMain.PortReset(Index)
            Port_Control(Index).Port_State = PCS_STOP
            TmrTimeoutMSG(Index).Enabled = False

            Dim s_Device As String = ""
            Select Case Index
                Case ACU
                    b_Ricezione(ACU) = False
                    If Not b_Parametri Then
                        OraACU = CDate("00:00:00")
                        i_Cont = 0
                        Port_Control(Index).Rec_State = PCR_STANDBY
                    End If
                    If Port_Control(Index).CKS_Error > 3 Then
                        If LblConfCNL1in.BackColor <> PCS_STOP_COL Then
                            LblConfCNL1in.BackColor = PCS_STOP_COL
                            LblConfCNL1in.ForeColor = Color.Black
                            s_Device = " ACU "
                        End If
                        b_Parametri = False
                        TagDato = ""
                        FrmMain.stpData.Text = ""
                        Port_Control(Index).CKS_Error = 0
                        If Not seriale Then FrmMain.LanResetAsync(Index)
                    Else
                        Port_Control(Index).CKS_Error += 1
                    End If

                Case MAC
                    FrmMain.LanResetAsync(MAC)
                    TmrTimeoutMSG(MAC).Enabled = True
                    If LblConfCNL2in.BackColor <> PCS_STOP_COL Then
                        LblConfCNL2in.BackColor = PCS_STOP_COL
                        LblConfCNL2in.ForeColor = Color.Black
                        s_Device = " MCS "
                    End If

                Case RX
                    LblConfCNL3in.BackColor = PCS_STOP_COL

                Case GRF
                    If Port_Control(Index).CKS_Error > 3 Then
                        If LblConfCNL4in.BackColor <> PCS_STOP_COL Then
                            LblConfCNL4in.BackColor = PCS_STOP_COL
                            LblConfCNL4in.ForeColor = Color.Black
                            s_Device = " D/C "
                        End If
                        Port_Control(Index).CKS_Error = 0
                        If Not seriale Then FrmMain.LanResetAsync(Index)
                    Else
                        Port_Control(Index).CKS_Error += 1
                    End If

                Case INTELSAT
                    If LblConfCNL5in.BackColor <> PCS_STOP_COL Then
                        LblConfCNL5in.BackColor = PCS_STOP_COL
                        LblConfCNL5in.ForeColor = Color.Black
                        s_Device = " INTELSAT "
                    End If
                    TmrTimeoutMSG(INTELSAT).Enabled = True

                Case PCU
                    If Port_Control(Index).CKS_Error > 3 Then
                        If LblConfCNL6in.BackColor <> PCS_STOP_COL Then
                            LblConfCNL6in.BackColor = PCS_STOP_COL
                            LblConfCNL6in.ForeColor = Color.Black
                            s_Device = " Polarization "
                        End If
                        Port_Control(Index).CKS_Error = 0
                    Else
                        Port_Control(Index).CKS_Error += 1
                    End If
            End Select

            If s_Device <> "" Then
                MainForm.MessaggioStatus = "Time out " & s_Device
                ScriviAlarm("Time out " & s_Device, 1)
                AggiornaGriglia()
                MainForm.TmrMsg.Enabled = True
            End If
        Catch ex As Exception
            WriteError(0, "TmrTimeoutMSG_Tick: " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Sub pubbliche richiamate da altri form/moduli
    ' -------------------------------------------------------

    ''' Chiude il sotto-socket RX[ind]. Le connessioni RX reali sono quelle
    ''' di SocketManager (vedi LanReset1): i campi sckRX/streamRX di questa
    ''' classe non vengono più popolati da nessuno.
    Public Sub SckRX_Close(ind As Integer)
        SocketManager.CloseRX(ind)
    End Sub

    ''' <summary>C10: richiesta della frequenza al Down Converter ("FE?").</summary>
    Public Sub CmdGRF()
        b_int(GRF) = True : b_Ricezione(GRF) = True
        Send_GRF = CheckDC("{" & Chr(AddressDC) & "FE?}") & vbCr & vbLf
        Invio(Send_GRF, GRF)
    End Sub

    ''' <summary>C10: richiesta di stato al polarizzatore (seriale PCU).</summary>
    Public Sub CmdPol()
        b_int(PCU) = True : b_Ricezione(PCU) = True
        Invio(ChrW(2) & "07!24" & ChrW(3), PCU)     ' STX "07!24" ETX
    End Sub

    ''' <summary>
    ''' Stato della voce di menu Bell (MnuBell): equivalente .NET di
    ''' "frmRicezione.mnuFile(2).Checked" nel VB6 originale di FOC6
    ''' (frmMain.frm, tmrBeep_Timer) — stessa voce di menu già portata
    ''' per BTS1 (MnuFileBell_Click).
    ''' </summary>
    Public Function BeepAbilitato() As Boolean
        Return MnuBell IsNot Nothing AndAlso MnuBell.Checked
    End Function

    ''' <summary>
    ''' Controllo di congruità tra frequenza beacon comandata
    ''' (CmbBcnFreq) e quella effettivamente impostata (TxtFrqBeacon):
    ''' se diverse, invia il comando GRF di cambio frequenza. Porting
    ''' diretto da Prog0357-FOC6\frmMain.frm (tmrOrologio_Timer,
    ''' righe 1081-1097 — "Controllo congruità tra valore comandato e
    ''' ricevuto", 29/10/2022).
    ''' </summary>
    Public Sub ControllaFrequenzaBeacon()
        If Not b_frequency Then Return
        If LblConfCNL4in.BackColor <> COL_OK OrElse Not IsNumeric(CmbBcnFreq.Text) Then Return
        If TxtFrqBeacon.Text <> CmbBcnFreq.Text Then
            ' C10: frequenza in Hz con il comando "FE" e CheckDC (FOC6: kHz, "F").
            Dim dFrqBeacon As Double = Val(CmbBcnFreq.Text) * 100000
            Send_GRF = CheckDC("{" & Chr(AddressDC) & "FE" & Format$(dFrqBeacon, "0000000000") & "}")
            b_Ricezione(GRF) = False
            Invio(Send_GRF, GRF)
            TxtFrqBeacon.Text = ""
            TxtOrdini.Focus()
        Else
            b_frequency = False
        End If
    End Sub

    ''' Ciclo di calcolo e invio comandi (chiamato dal timer TmrRic).
    ''' Lo Spiral/Automatic Acquire ha il proprio calcolo dedicato
    ''' (Calcolo_Aquire, via TmrSpiralAcquire): se TmrRic chiamasse comunque
    ''' Calcolo_Coordinate qui, sovrascriverebbe subito dopo ogni comando
    ''' Preset/Program impostato da Calcolo_Aquire con il tracciamento da
    ''' effemeridi, dato che Calcolo_Coordinate non sa nulla di b_Acquire.
    Public Sub CalcolaEInvia()
        ' OP_PROIEZ escluso come gli altri modi "a calcolo proprio": senza
        ' questo, ogni 400ms (TmrRic) questa funzione richiama
        ' Calcolo_Coordinate e sovrascrive LblAzimuth/LblElevazione — gli
        ' stessi campi che InvioPro (Proiezione.vb) aggiorna con il
        ' risultato della regressione/media sui punti bufferizzati, molto
        ' meno di frequente. Stesso bug già risolto per Preset/Spiral/
        ' Automatic Acquire, qui mancava per Proiezione.
        ' OP_MEMORYTRACK/OP_STEPTRACK esclusi per lo stesso motivo: anche lì
        ' non ha senso ricalcolare/mostrare una posizione target, l'ACU
        ' segue la memoria o lo step di segnale, non un calcolo orbitale.
        If i_Status = OP_STANDBY OrElse i_Status = OP_SPIRALACQUIRE OrElse i_Status = OP_AUTOMATICAQUIRE OrElse i_Status = OP_PROIEZ OrElse i_Status = OP_COMMANDPOSITION OrElse i_Status = OP_MEMORYTRACK OrElse i_Status = OP_STEPTRACK Then Return
        Try
            If Ricezione.Calcolo_Coordinate(NowUTC(), 0) Then
                b_DatiAntenna = True
                AggiornaAntenna()
                AggiornaErroriTracking()
            End If
        Catch
        End Try
    End Sub


    ' -------------------------------------------------------
    ' FORM LOAD
    ' -------------------------------------------------------
    Private Sub FrmRicezione_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Campi di sola presentazione: giallo se sconosciuti, 128,255,255 se noti
        ColoriStandard.RegistraDati(LblAzimuth, TxtAzEr, LblAzimuthC, LblAzError, LblAzTBrk, LblElevazione,
            TxtElEr, LblElevazioneC, LblElError, LblElTBrk, TxtPhaseEl, LblRevAz_ctrl,
            TxtInpFreq, TxtPhAz, LblPolRic, TxtLna, TxtMode_ctrl, LblPol1,
            TxtPolErr, LblPol, TxtPol, TxtFrqBeacon, TxtPhase, TxtRevAz,
            LblLNA, LblSwitches, LblBackUp, LblRisposta, TxtPhasAz)
        ' STARTUP_TIMING: strumentazione temporanea (vedi frmMain.vb).
        Dim swLoad As New System.Diagnostics.Stopwatch()
        swLoad.Start()
        Dim lastCk As Long = 0
        Dim Ck As Action(Of String) =
            Sub(nome As String)
                Dim ora As Long = swLoad.ElapsedMilliseconds
                WriteError(0, "STARTUP_TIMING   frmRicezione.Load/" & nome & ": " & (ora - lastCk) & " ms (totale " & ora & " ms)")
                lastCk = ora
            End Sub

        ' Sospende il ridisegno il prima possibile: senza questo, tra Load e
        ' Shown la finestra mostra per un istante il layout di progettazione
        ' (i vari quadri nelle posizioni non ancora riscalate) prima che
        ' FormResizer.Resize() in Shown li sistemi. Resize() riattiva da sola
        ' il ridisegno alla fine, a controlli già riposizionati.
        FormResizer.SuppressRedraw(Me)

        ' i_Status impostato prima di LeggiParametri (il Select Case lo usa)
        i_Status = OP_STANDBY

        ' Popola DataGridView log da DB
        Try
            AggiornaGriglia()
        Catch
        End Try
        Ck("AggiornaGriglia")

        LeggiParametri()
        Ck("LeggiParametri")
        CType(Me.MdiParent, FrmMain).IniPort_Control()
        Ck("IniPort_Control")


        ' Reset display DOPO la config: annulla ogni residuo della sessione precedente
        ResetVisualizzazione()
        AggiornaComando("Stand By")
        ' ResetVisualizzazione riporta LblSatellite a giallo incondizionatamente
        ' (COL_WARN), cancellando il verde appena applicato da LeggiParametri
        ' -> PopolaLblSatellite per Sole/Luna/Venere/Cassiopeia (Algoritmo=4):
        ' per questi non ha senso lo stato "in attesa TLE", quindi si
        ' riapplica subito dopo.
        ApplicaSatelliteSeAlgoritmo4()
        Ck("ResetVisualizzazione+AggiornaComando")

        ' Carica loghi (estratti da frmRicezione.frx VB6, copiati in Resources\)
        Try
            Dim resPath As String = System.IO.Path.Combine(Application.StartupPath, "Resources")
            Dim fLeft As String = System.IO.Path.Combine(resPath, "imgLogoLeft.jpg")
            Dim fRight As String = System.IO.Path.Combine(resPath, "imgLogoRight.bmp")
            If System.IO.File.Exists(fLeft) Then picLogoLeft.Image = Image.FromFile(fLeft)
            If System.IO.File.Exists(fRight) Then picLogoRight.Image = Image.FromFile(fRight)
        Catch
        End Try
        Ck("Loghi")

        intSockCnt = 0
        intSock1Cnt = 0

        CType(Me.MdiParent, FrmMain).TmrOrologio.Enabled = True
        Exit Sub
ErrLoad:
        WriteError(Err.Number, "frmRicezione Load " & Err.Description)
    End Sub

    ' -------------------------------------------------------
    ' FORM SHOWN / RESIZE — adatta i controlli alla reale
    ' risoluzione dello schermo (vedi FormResizer.vb)
    ' -------------------------------------------------------
    Private Sub FrmRicezione_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' STARTUP_TIMING: strumentazione temporanea (vedi frmMain.vb).
        Dim swShown As New System.Diagnostics.Stopwatch()
        swShown.Start()
        WriteError(0, "STARTUP_TIMING   frmRicezione.Shown: inizio")

        ' 1339x788 = ClientSize di progettazione nel Designer (vedi frmRicezione.Designer.vb)
        FormResizer.Init(Me, New Size(1339, 788))
        FormResizer.Resize(Me)
        WriteError(0, "STARTUP_TIMING   frmRicezione.Shown/FormResizer.Init+Resize: " & swShown.ElapsedMilliseconds & " ms")
        ' Al primo Shown con la finestra già massimizzata, la fusione
        ' (merge) del MenuStrip nella cornice MDI a volte non avviene:
        ' non è un problema di sola ripittura (Refresh/Invalidate non
        ' bastano), va rifatta la fusione stessa. L'unico modo affidabile
        ' è forzare un breve ciclo Normal->Maximized, che internamente
        ' rifà il merge da capo.
        If Me.MdiParent IsNot Nothing AndAlso Me.WindowState = FormWindowState.Maximized Then
            ' Sospeso anche il ridisegno della form MDI ospitante: durante il
            ' passaggio per lo stato Normal, l'area client di frmMain dietro
            ' la finestra figlia (temporaneamente più piccola) si ridisegna
            ' mostrando per un istante lo sfondo/bordo del riquadro. Senza
            ' questa sospensione anche il fix su "Me" non basta a nasconderlo.
            Dim mdiParentForm As Form = Me.MdiParent
            FormResizer.SuppressRedraw(Me)
            FormResizer.SuppressRedraw(mdiParentForm)
            b_SospendiResize = True
            mdiParentForm.SuspendLayout()
            Me.WindowState = FormWindowState.Normal
            Me.WindowState = FormWindowState.Maximized
            mdiParentForm.ResumeLayout()
            b_SospendiResize = False
            FormResizer.Resize(Me)
            FormResizer.ResumeRedraw(mdiParentForm)
        End If
        WriteError(0, "STARTUP_TIMING   frmRicezione.Shown: fine, totale " & swShown.ElapsedMilliseconds & " ms")
    End Sub

    Private Sub FrmRicezione_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If b_SospendiResize Then Exit Sub
        FormResizer.Resize(Me)
    End Sub

    ' -------------------------------------------------------
    ' FORM ACTIVATED
    ' -------------------------------------------------------
    Private Sub FrmRicezione_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated

        b_Parametri = False

        ' Impostazione display soglie
        If TxtSogliaAuto IsNot Nothing Then TxtSogliaAuto.Text = CStr(g_ThrAuto)
        If TxtThrAz IsNot Nothing Then TxtThrAz.Text = CStr(g_ThrAz)
        If TxtThrEl IsNot Nothing Then TxtThrEl.Text = CStr(g_ThrEl)
        If TxtOffsetMax IsNot Nothing Then TxtOffsetMax.Text = CStr(Max_Offset)
        If TxtCoorOff IsNot Nothing Then TxtCoorOff.Text = CStr(CoorOff)

        ' Fase
        If g_ChangePhase = "Y" Then
            CmdPhase.Text = "N"
            LblPhase.BackColor = COL_VALIDO
        Else
            CmdPhase.Text = "Y"
            LblPhase.BackColor = Color.Yellow
        End If

        ' Stato PC1
        If Not b_StatusPC1 Then
            LblPC1.Text = "LOC"
            LblPC1.BackColor = Color.Yellow
        Else
            LblPC1.Text = "REM"
            LblPC1.BackColor = COL_VALIDO
        End If

        b_frequency = False

        ' Beacon Bar
        If IsNumeric(ScalaMin) AndAlso IsNumeric(ScalaMax) AndAlso CDbl(ScalaMax) > CDbl(ScalaMin) Then
            If CDbl(ScalaMin) >= 0 Then
                BarBeacon.Minimum = CInt(ScalaMin)
                BarBeacon.Maximum = CInt(ScalaMax)
            Else
                BarBeacon.Minimum = 0
                BarBeacon.Maximum = CInt(Math.Abs(CDbl(ScalaMax) - CDbl(ScalaMin)))
            End If
            BarBeacon.Value = BarBeacon.Minimum
        End If

        ' Visualizzazione parametri ricevitore
        If LblPolRic IsNot Nothing Then LblPolRic.Text = t_Pol
        If TxtLna IsNot Nothing Then TxtLna.Text = CStr(i_Lna)
        If TxtFrqBeacon IsNot Nothing Then TxtFrqBeacon.Text = ""
        If TxtPhase IsNot Nothing Then TxtPhase.Text = CStr(PhaseValue1)
        If TxtPhAz IsNot Nothing Then TxtPhAz.Text = CStr(PhaseValue2)
        If TxtRevAz IsNot Nothing Then TxtRevAz.Text = CStr(g_RevAz)
        If IsNumeric(g_Frequenza) AndAlso CDbl(g_Frequenza) > 0 Then
            If CmbBcnFrequency IsNot Nothing Then CmbBcnFrequency.Text = Format(CDbl(g_Frequenza), "#0000.000")
        End If

        ' Titolo intestazione
        If SS_Intestazione IsNot Nothing Then SS_Intestazione.Text = "ACU - " & Antenna

        ' Timer
        If TmrTimeoutMSG IsNot Nothing AndAlso TmrTimeoutMSG(MAC) IsNot Nothing Then
            TmrTimeoutMSG(MAC).Enabled = False
            TmrTimeoutMSG(MAC).Enabled = True
        End If
        If TmrTimeoutMSG IsNot Nothing AndAlso TmrTimeoutMSG(GRF) IsNot Nothing Then
            TmrTimeoutMSG(GRF).Enabled = False
            TmrTimeoutMSG(GRF).Enabled = True
        End If

        If TmrRic IsNot Nothing Then TmrRic.Interval = 400

        i_Cont = 0
        i_ric = 0
        If TmrRic IsNot Nothing Then TmrRic.Enabled = True
        g_NewFrequency = True
        g_NewLna = False
        NoPhasing = False

        If CmdLockBell IsNot Nothing Then CmdLockBell.Text = "Bell"
        i_LockBell = 1
        i_Alarm = True

        Try
            frmGrafico.TmrGrafico.Interval = 1000
            frmGrafico.TmrGrafico.Enabled = True
        Catch
        End Try

        b_Ricezione(ACU) = False

        Try
            CType(Me.MdiParent, FrmMain).TmrPhase.Enabled = True
        Catch
        End Try

        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
        Exit Sub
RicActive:
        If Err.Number = 0 OrElse Err.Number = 20 Then Exit Sub
        WriteError(Err.Number, "frmRicezione - Activate " & Err.Description)
        Err.Clear()
    End Sub

    ' -------------------------------------------------------
    ' LEGGI PARAMETRI DA FILE INI
    ' Conversione fedele al VB6 originale.
    ' NOTA: usa Val(Right(Port, lun)) per estrarre il numero
    ' di porta senza problemi di indice/lunghezza.
    ' -------------------------------------------------------
    Public Sub LeggiParametri()
        Dim Ind As Integer
        Dim Port As String, Sezione As String
        Dim a As String


        For Ind = 1 To NumCom
            Sezione = "Canale" & Ind.ToString().Trim()
            Port_Control(Ind).Socket = LeggiFileIni(Sezione, "Socket")
            Port = LeggiFileIni("Canale" & Ind.ToString().Trim(), "PortaIN")

            If Port <> "" Then
                lun = Port.Length - 3
                Dim portNum As Integer = CInt(Val(Strings.Right(Port, lun)))
                Canali(Ind).Port_IN = portNum

                If Port_Control(Ind).Socket <> "S" Then
                    ' Configurazione seriale
                    Dim vel As String = LeggiFileIni(Sezione, "Vel")
                    Dim par As String = LeggiFileIni(Sezione, "Par")
                    Dim bits As String = LeggiFileIni(Sezione, "BitDati")
                    Dim stopBits As String = LeggiFileIni(Sezione, "BitStop")
                    ' C10: indicizzato per canale come nel VB6
                    ' (Port_Control(Ind).Port_settings), non per numero COM.
                    Port_Control(Ind).Port_settings = vel & "," & par & "," & bits & "," & stopBits
                    Port_Control(Ind).Time_out = CDbl(Val(LeggiFileIni(Sezione, "TimeOut")))
                Else
                    ' Configurazione LAN
                    Port_Control(Ind).IPAdress = LeggiFileIni(Sezione, "Ind_IP")
                    Port_Control(Ind).LanPort = CDbl(Val(LeggiFileIni(Sezione, "Porta")))
                    Port_Control(Ind).Listen = LeggiFileIni(Sezione, "Master")
                    Port_Control(Ind).Time_out = CDbl(Val(LeggiFileIni(Sezione, "TimeOut")))

                    ' La connessione/listen via SocketManager per ACU/MAC/GRF
                    ' viene fatta da IniPort_Control (chiamata subito dopo in
                    ' FrmRicezione_Load) tramite LanReset, che in più
                    ' controlla l'esito (IsConnected/IsMacConnected/
                    ' IsDCConnected) e aggiorna stato/colori/allarmi: qui
                    ' sarebbe una connessione duplicata e incompleta,
                    ' solo persa in attesa di rete.
                    Select Case Ind
                        Case RX
                            LanPortRx = CInt(Port_Control(Ind).LanPort)
                            If Port_Control(Ind).Listen = "S" Then
                                LanRx(1).LanPort = RxMon
                                LanRx(2).LanPort = RxCtrl
                                LanRx(3).LanPort = RxRst
                                LanRx(4).LanPort = RxLog
                                LanRx(5).LanPort = RxPhase
                                LanRx(6).LanPort = RxCal
                                For i = 1 To 6
                                    LanRx(i).Listen = Port_Control(Ind).Listen
                                    LanRx(i).Time_out = Port_Control(Ind).Time_out
                                    LanRx(i).Socket = Port_Control(Ind).Socket
                                    LanRx(i).IPAdress = Port_Control(Ind).IPAdress
                                Next
                            End If
                    End Select
                End If

                ' Intervalli polling canale
                Select Case Ind
                    Case ACU
                        i_Inter = CSng(Val(LeggiFileIni(Sezione, "Inter")))
                    Case RX
                        i_InterRic = CSng(Val(LeggiFileIni(Sezione, "Inter")))
                        Try
                            CType(Me.MdiParent, FrmMain).TmrPhase.Interval = CInt(i_InterRic * 1000) \ 2
                        Catch
                        End Try
                    Case GRF
                        i_InterFre = CInt(Val(LeggiFileIni(Sezione, "Inter")))
                    Case PCU
                        i_InterPol = CSng(Val(LeggiFileIni(Sezione, "Inter")))      ' C10: polling polarizzatore
                End Select

                ' Timeout timer canale
                Dim tmo As Integer = CInt(Port_Control(Ind).Time_out * 1000)
                If TmrTimeoutMSG IsNot Nothing AndAlso TmrTimeoutMSG(Ind) IsNot Nothing Then
                    TmrTimeoutMSG(Ind).Interval = If(tmo > 0, tmo, 30000)
                End If
            End If
        Next Ind

        ' Parametri generali da [Default]
        Canali(ACU).Prot_entity = LeggiFileIni("Default", "ProtEntity")
        Canali(ACU).File_Log = LeggiFileIni("Default", "Log")
        i_IntCom = CSng(Val(LeggiFileIni("Default", "IntCom")))
        g_Validi = CInt(Val(LeggiFileIni("Default", "Validita")))
        g_ValTLE = CInt(Val(LeggiFileIni("Default", "ValTLE")))
        AddressDC = LeggiFileIni("Default", "AddressDC")
        BackUpPT = LeggiFileIni("Default", "BackUpPT")
        BackUpAutoT = LeggiFileIni("Default", "BackUpAutoT")
        g_ThrAuto = CDbl(Val(LeggiFileIni("Default", "ThrAuto")))
        g_ThrAz = CDbl(Val(LeggiFileIni("Default", "MaxErrAz")))
        g_ThrEl = CDbl(Val(LeggiFileIni("Default", "MaxErrEl")))
        ' i_LockThr veniva letto SOLO all'apertura di FrmVisualAcquire/
        ' frmAcquire (lazy) - se l'utente osserva il Lock su frmRicezione
        ' senza aver ancora aperto quelle form, restava al default CLR 0
        ' invece del valore configurato nell'ini, facendo apparire UNLOCK
        ' qualunque livello di beacon negativo (segnalato: "-55 con soglia
        ' -70 segna unlock"). Letto qui subito, come le altre soglie.
        i_LockThr = CInt(Val(LeggiFileIni("Acquire", "LockThr")))
        g_ElMin = CDbl(Val(LeggiFileIni("Default", "MinEl")))
        i_Log = CInt(Val(LeggiFileIni("Default", "IntLog")))
        Ptic = LeggiFileIni("Default", "Ptic")
        Ptic = "P"   ' forzato per INTELSAT

        ' Beacon scale
        ScalaMax = CDbl(Val(LeggiFileIni("Default", "ScalaMax")))
        ScalaMin = CDbl(Val(LeggiFileIni("Default", "ScalaMin")))
        If LblBcnMax IsNot Nothing Then LblBcnMax.Text = CStr(ScalaMax)
        If LblBcnMin IsNot Nothing Then LblBcnMin.Text = CStr(ScalaMin)
        Dim scMax As Integer = CInt(Math.Abs(ScalaMax - ScalaMin))
        BarBeacon.Minimum = 0
        BarBeacon.Maximum = If(scMax > 0, scMax, 100)

        ' Pendenza beacon
        Pendenza = CDbl(Val(LeggiFileIni("Default", "Pendenza")))
        Pendenza1 = CDbl(Val(LeggiFileIni("Default", "Pendenza1")))

        ' Offset
        Max_Offset = CDbl(Val(LeggiFileIni("Default", "MaxOffset")))
        CoorOff = CDbl(Val(LeggiFileIni("Default", "CoorOff")))
        Valore0 = CDbl(Val(LeggiFileIni("Default", "Valore0")))

        ' Backup Program Track mode
        Select Case BackUpPT
            Case "P" : BackPT = &H4
            Case "R" : BackPT = &H24
            Case "S" : BackPT = &H0
            Case "T" : BackPT = &H20
        End Select

        ' Tolleranza e varie
        l_Tolleranza = CDbl(Val(LeggiFileIni("Default", "Tolleranza")))
        s_Ora = LeggiFileIni("Default", "Ora")
        CN = CInt(Val(LeggiFileIni("Default", "SinReg")))
        Logo = LeggiFileIni("Logo", "Pic")
        Antenna = LeggiFileIni("Default", "Antenna")
        s_Satellite = LeggiFileIni("Default", "Satellite")

        ' Step Track parameters (da 18/12/2024)
        i_IT = CInt(Val(LeggiFileIni("Default", "IT")))
        i_AT = CInt(Val(LeggiFileIni("Default", "AT")))
        i_ET = CInt(Val(LeggiFileIni("Default", "ET")))
        i_XT = CInt(Val(LeggiFileIni("Default", "XT")))
        i_YT = CInt(Val(LeggiFileIni("Default", "YT")))
        i_InterST = CInt(Val(LeggiFileIni("Default", "RT"))) * 60
        i_CY = CInt(Val(LeggiFileIni("Default", "CY")))
        i_BM = LeggiFileIni("Default", "BM")

        If s_Satellite = "" Then s_Satellite = "REMOTE"

        g_DirPar = LeggiFileIni("Server", "Directory")
        g_DirBackUp = LeggiFileIni("Server", "BackUp")
        g_Assi = 2

        ' Canale MAC
        Canali(MAC).Prot_entity = LeggiFileIni("Default", "ProtEntity")
        Canali(MAC).File_Log = LeggiFileIni("Default", "Log")

        ' Popola combo satellite
        PopolaLblSatellite()

        ' Phase change display
        a = LeggiFileIni("Default", "ChangePhase")
        If a = "N" Then
            CmdPhase.Text = "N"
            LblPhase.BackColor = COL_VALIDO
            g_ChangePhase = "Y"
        Else
            CmdPhase.Text = "Y"
            LblPhase.BackColor = Color.Yellow
            g_ChangePhase = "N"
        End If

        ' Stato operativo iniziale
        Select Case i_Status
            Case OP_STANDBY
                LblCommand.Text = "Stand By"
                i_ProgTrack = 0
            Case OP_STEPTRACK
                LblCommand.Text = "Step Track"
                LblStepTrack.Visible = True
                i_ProgTrack = 0
            Case OP_PROGRAMTRACK
                i_ProgTrack = 1
                LblCommand.Text = "Program Track"
                If CmdOffset IsNot Nothing Then
                    CmdOffset.Enabled = True
                    CmdOffset.Visible = True
                End If
            Case OP_AUTOTRACK
                LblCommand.Text = "Auto Track"
                i_ProgTrack = 2
            Case OP_AUGMENTED
                i_ProgTrack = 4
                If CmdOffset IsNot Nothing Then
                    CmdOffset.Enabled = True
                    CmdOffset.Visible = True
                End If
                LblCommand.Text = "Augmented"
            Case OP_MEMORYTRACK
                LblCommand.Text = "Memory Track"
        End Select

        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' -------------------------------------------------------
    ' Popola la ComboBox satellite con tutti i satelliti del DB
    ' -------------------------------------------------------
    Public Sub PopolaLblSatellite()
        If dbOrbite Is Nothing Then Exit Sub
        LblSatellite.Items.Clear()
        Dim rs As Object = dbOrbite.OpenRecordset("SELECT * FROM [Satelliti] ORDER BY NomeSatellite", dbOpenDynaset)
        Do While Not rs.EOF
            If Not IsDBNull(rs!NomeSatellite) Then
                LblSatellite.Items.Add(CStr(rs!NomeSatellite))
            End If
            rs.MoveNext
        Loop
        rs.Close()
        If s_Satellite <> "" Then
            LblSatellite.Text = s_Satellite
            ' Se il satellite ripristinato (es. da file INI) è Sole/Luna/
            ' Venere/Cassiopeia, applica subito il colore verde: impostare
            ' .Text qui non genera un "cambio" utile per
            ' SelectedIndexChanged (il testo coincide già con s_Satellite),
            ' quindi senza questa chiamata il campo restava giallo (colore
            ' di default nel Designer) finché l'utente non selezionava
            ' manualmente un'altra voce e poi tornava su questa.
            ApplicaSatelliteSeAlgoritmo4()
        End If
    End Sub

    ' -------------------------------------------------------
    ' Aggiorna DataGridView log allarmi
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
            tdbLog.DataSource = dt
            ' Larghezza colonne: tdbLog genera le colonne automaticamente dai
            ' dati (colData/colMsg nel Designer sono dichiarate ma mai
            ' collegate), quindi va impostata qui dopo il binding. "Data" a
            ' larghezza fissa (le basta per "dd/MM/yyyy HH:mm:ss"),
            ' "Messaggio" riempie lo spazio restante.
            If tdbLog.Columns.Contains("Data") Then tdbLog.Columns("Data").Width = 180
            If tdbLog.Columns.Contains("Messaggio") Then tdbLog.Columns("Messaggio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Catch
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Aggiorna dati visualizzati sul form
    ' -------------------------------------------------------
    Public Sub AggiornaDati()
        AggiornaTuttiDati()
        GestisciAugmentedAutotrack()
    End Sub

    ' -------------------------------------------------------
    ' Porting diretto da VB6 (frmRicezione.frm, lblRVR_Change, righe
    ' 6719-6793): handoff automatico Augmented -> Autotrack quando il
    ' beacon resta agganciato entro soglia (invia "09 AP" e passa
    ' i_ProgTrack a 2), con ritorno a 1 (ricatturando l'errore corrente
    ' come nuovo AzOffset/ElOffset) se il lock peggiora, più il clamp
    ' dell'errore di tracking a ±Max_Offset. Mai portato: finora
    ' AggiornaErroriTracking calcolava solo l'errore per la
    ' visualizzazione, senza handoff né clamp — qui lo si sovrascrive
    ' con la versione corretta quando le condizioni la riguardano.
    ' -------------------------------------------------------
    Public Sub GestisciAugmentedAutotrack()
        If i_Status <> OP_AUGMENTED Then Exit Sub
        If AzInibhit AndAlso ElInibhit Then Exit Sub
        If Not IsNumeric(s_LevAz1) OrElse Not IsNumeric(s_LevEl1) Then Exit Sub

        If Not IsNumeric(LblAzError.Text) Then LblAzError.Text = "0"
        If Not IsNumeric(LblElError.Text) Then LblElError.Text = "0"

        If i_ProgTrack <> 4 Then
            If Val(s_Lev1) > g_ThrAuto AndAlso RTrim(LblLock.Text) = "LOCK" AndAlso
               Math.Abs(Val(s_LevAz1)) < g_ThrAz AndAlso Math.Abs(Val(s_LevEl1)) < g_ThrEl Then
                If i_ProgTrack = 1 Then
                    TmrTimeoutMSG(ACU).Enabled = True
                    b_Ricezione(ACU) = False
                    b_int(ACU) = True
                    Dim send As String
                    If AzInibhit Then
                        send = CheckSum("09EA ")
                    ElseIf ElInibhit Then
                        send = CheckSum("09AA ")
                    Else
                        send = CheckSum("09 AP")
                    End If
                    InvioACU(send)
                    i_ProgTrack = 2
                    If CmdOffset IsNot Nothing Then
                        CmdOffset.Enabled = False : CmdOffset.Visible = False
                    End If
                End If
                ' L'errore si calcola sul valore calcolato dal program track
                ' (AzCalc/ElCalc), non su quello con offset già applicato.
                If IsNumeric(AzCalc) Then
                    LblAzError.Text = Format$(Val(TxtAz.Text) - AzCalc, "0.00")
                    If Math.Abs(Val(LblAzError.Text)) > Math.Abs(Max_Offset) Then
                        LblAzError.Text = Format$(Math.Sign(Val(LblAzError.Text)) * Math.Abs(Max_Offset), "0.00")
                    End If
                End If
                If IsNumeric(ElCalc) Then
                    LblElError.Text = Format$(Val(TxtEl.Text) - ElCalc, "0.00")
                    If Math.Abs(Val(LblElError.Text)) > Math.Abs(Max_Offset) Then
                        LblElError.Text = Format$(Math.Sign(Val(LblElError.Text)) * Math.Abs(Max_Offset), "0.00")
                    End If
                End If
            Else
                ' Non ci sono più i requisiti per l'autotrack: torna al
                ' calcolo da programma, ricatturando l'errore attuale come
                ' nuovo offset di partenza.
                If i_ProgTrack = 2 Then
                    If IsNumeric(LblAzError.Text) Then AzOffset = Val(LblAzError.Text)
                    If IsNumeric(LblElError.Text) Then ElOffset = Val(LblElError.Text)
                    If CmdOffset IsNot Nothing Then
                        CmdOffset.Enabled = True : CmdOffset.Visible = True
                    End If
                    i_ProgTrack = 1
                End If
            End If
        ElseIf i_ProgTrack = 4 Then
            ' Convergenza iniziale verso il punto calcolato: appena l'antenna
            ' reale si avvicina entro ±Max_Offset si passa al controllo normale.
            If IsNumeric(LblElevazione.Text) AndAlso IsNumeric(LblAzimuth.Text) Then
                Dim tmpErrAz As Double = Val(TxtAz.Text) - (Val(LblAzimuth.Text) - Val(LblAzError.Text))
                Dim tmpErrEl As Double = Val(TxtEl.Text) - (Val(LblElevazione.Text) - Val(LblElError.Text))
                If Math.Abs(tmpErrAz) < Math.Abs(Max_Offset) AndAlso Math.Abs(tmpErrEl) < Math.Abs(Max_Offset) Then
                    i_ProgTrack = 1
                End If
            End If
        End If
    End Sub

    ' -------------------------------------------------------
    ' Utility: cambia colore di una label per nome
    ' -------------------------------------------------------
    Public Sub SetLabelColor(labelName As String, color As Color)
        Dim ctrl As Control = Me.Controls.Find(labelName, True).FirstOrDefault()
        If ctrl IsNot Nothing Then ctrl.BackColor = color
    End Sub

    ' -------------------------------------------------------
    ' Utility: imposta intervallo timer timeout
    ' -------------------------------------------------------
    Public Sub SetTimeoutInterval(ind As Integer, ms As Integer)
        If ind >= 0 AndAlso ind <= NumCom Then
            TmrTimeoutMSG(ind).Interval = If(ms > 0, ms, 30000)
        End If
    End Sub

    ' =======================================================
    ' PULSANTI COMANDO ANTENNA
    ' =======================================================

    ' --- STAND BY ---
    Public Sub CmdSTBY_Click(sender As Object, e As EventArgs) Handles CmdSTBY.Click
        AzOffset = 0 : ElOffset = 0 : AzCalc = 0 : ElCalc = 0
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False

        If s_ACULR = "L" Then
            MostraWarning("ACU in Local") : Return
        End If
        If AzInibhit AndAlso ElInibhit Then Return

        b_Ricezione(ACU) = False
        b_int(ACU) = True
        i_Cont = 2
        InvioACU(STX & "08 S79" & ETX)
        PTK = &H0
        d_DataInizioProg = ""
        MainForm.MessaggioStatus = ""
        LblStepTrack.Visible = False
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = False : CmdOffset.Visible = False
        End If
        b_StepTrack = False
        If TxtMode IsNot Nothing Then TxtMode.Text = ""
        LblCommand.Text = "Stand By"
        i_Status = OP_STANDBY
    End Sub

    ' --- COMMAND POSITION ---
    Public Sub CmdCmdPos_Click(sender As Object, e As EventArgs) Handles CmdCmdPos.Click
        AzOffset = 0 : ElOffset = 0 : AzCalc = 0 : ElCalc = 0
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False
        b_StepTrack = False

        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return

        b_int(ACU) = True
        b_Ricezione(ACU) = False
        If AzInibhit AndAlso ElInibhit Then Return

        If AzInibhit Then
            i_Assi = 2
        ElseIf ElInibhit Then
            i_Assi = 1
        Else
            i_Assi = 0
        End If

        b_ClosePreset = False
        i_ProgTrack = 0
        ' Show(Me) imposta frmRicezione come owner: senza owner (diversamente
        ' da VB6, dove l'ownership tra le form è implicita) il successivo
        ' TxtOrdini.Focus() riattiva frmRicezione e la porta sopra a
        ' FrmPreset, che resta aperta ma nascosta dietro.
        FrmPreset.Show(Me)
    End Sub

    ' --- STEP TRACK ---
    Public Sub CmdStepTrack_Click(sender As Object, e As EventArgs) Handles CmdStepTrack.Click
        AzOffset = 0 : ElOffset = 0 : AzCalc = 0 : ElCalc = 0
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False

        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return

        b_ClosePreset = True
        FrmPreset.Close()
        If AzInibhit AndAlso ElInibhit Then Return

        ' Step Track è un modo operativo dell'ACU, non un calcolo da fare in
        ' BTS1 (a differenza di Calcolo_StepTrack/TmrSpiralAcquire, usato
        ' prima qui: calcolava e inviava lui stesso i singoli passi). Si
        ' invia solo il comando di modo — come Stand By/Memory Track — e si
        ' aspetta che sia l'ACU a cercare il punto ottimale, alimentando il
        ' campo errore con le risposte reali dell'ACU. Pattern "09EST"/
        ' "09AST"/"09 ST2C" confermato dal VB6 originale (frmRicezione.frm,
        ' cmdStepTrack_Click): era presente ma commentato lì, sostituito
        ' dal calcolo software — qui si fa la scelta opposta su richiesta
        ' esplicita, perché Step Track resti un comando di modo puro.
        b_int(ACU) = True
        b_Ricezione(ACU) = False
        i_Cont = 2
        If AzInibhit Then
            InvioACU(CheckSum("09EST"))
        ElseIf ElInibhit Then
            InvioACU(CheckSum("09AST"))
        Else
            InvioACU(STX & "09 ST2C" & ETX)
        End If

        PTK = &H0
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = False : CmdOffset.Visible = False
        End If
        i_Status = OP_STEPTRACK
        LblCommand.Text = "Step Track"
        LblStepTrack.Visible = True
        i_ProgTrack = 0
        b_LockOn = True
        b_StepTrack = True
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' --- AUTO TRACK ---
    Public Sub CmdAutoTrack_Click(sender As Object, e As EventArgs) Handles CmdAutoTrack.Click
        AzOffset = 0 : ElOffset = 0
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False
        b_StepTrack = False

        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return

        b_ClosePreset = True : FrmPreset.Close()
        b_Ricezione(ACU) = False : b_int(ACU) = True
        If AzInibhit AndAlso ElInibhit Then Return

        Dim send As String
        If AzInibhit Then
            send = CheckSum("09EA ")
        ElseIf ElInibhit Then
            send = CheckSum("09AA ")
        Else
            send = CheckSum("09 AP")
        End If
        InvioACU(send)
        PTK = &H0
        d_DataInizioProg = ""
        MainForm.MessaggioStatus = ""
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = False : CmdOffset.Visible = False
        End If
        AzOffset = 0 : ElOffset = 0
        i_Status = OP_AUTOTRACK
        LblCommand.Text = "Auto Track"
        i_ProgTrack = 2
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' --- AUGMENTED AUTO TRACK ---
    ' Attiva lo stato "Augmented"/"Program Track" dopo il passaggio dalla
    ' maschera FrmProgram (controllo coordinate + autorizzazione operatore),
    ' esattamente come nel VB6 originale dopo "frmProgram.Show vbModal".
    Private Sub AttivaAugmented()
        d_DataInizioProg = ""
        FrmMain.stpProgram.Text = d_DataInizioProg
        i_Status = OP_AUGMENTED
        i_ProgTrack = If(s_OpMode <> "Monopulse", 4, 2)
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = True : CmdOffset.Visible = True
        End If
        LblCommand.Text = "Augmented"
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False
    End Sub

    Private Sub AttivaProgramTrack()
        If i_Status <> OP_AUGMENTED Then
            AzOffset = 0 : ElOffset = 0
        Else
            If IsNumeric(LblAzError.Text) Then AzOffset = CDbl(LblAzError.Text)
            If IsNumeric(LblElError.Text) Then ElOffset = CDbl(LblElError.Text)
        End If
        LblAzError.Text = CStr(AzOffset)
        LblElError.Text = CStr(ElOffset)
        i_Status = OP_PROGRAMTRACK
        i_ProgTrack = 1
        LblCommand.Text = "Program Track"
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = True : CmdOffset.Visible = True
        End If
        ReDim pts(0)
    End Sub

    Public Sub CmdAugmented_Click(sender As Object, e As EventArgs) Handles CmdAugmented.Click
        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return
        If Not b_Calcola Then
            MainForm.MessaggioStatus = "Augmented not is possible"
            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
            Return
        End If

        If i_Status <> OP_AUGMENTED AndAlso i_Status <> OP_PROGRAMTRACK Then
            ' Non ancora in Augmented/Program Track: passa dal controllo
            ' coordinate + autorizzazione operatore (FrmProgram) prima di
            ' avviare davvero il tracking calcolato dal programma.
            If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
            FrmProgram.ShowDialog(Me)
            If b_Abort Then Return
        Else
            ' Già in uno dei due modi calcolati: passaggio diretto, senza
            ' richiedere una nuova autorizzazione.
            AttivaAugmented()
            Return
        End If

        If PTK = &H10 Then
            b_ClosePreset = True : FrmPreset.Close()
            AttivaAugmented()
            b_StepTrack = False
        Else
            If PTK = &H0 AndAlso IsDate(d_DataInizioProg) Then
                LblCommand.Text = "Augmented Enabled"
                i_StatusDiff = OP_AUGMENTED
            End If
        End If

        i_Inter = CSng(Val(LeggiFileIni("Canale1", "Inter")))
        i_Cont = 1
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' --- PROGRAM TRACK ---
    Public Sub CmdProgramTrack_Click(sender As Object, e As EventArgs) Handles CmdProgramTrack.Click
        If i_Status = OP_PROGRAMTRACK Then Return
        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return
        If Not b_Calcola Then
            MainForm.MessaggioStatus = "Program track not is possible"
            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
            GoTo ExitProgTrack
        End If

        If i_Status <> OP_AUGMENTED AndAlso i_Status <> OP_PROGRAMTRACK Then
            ' Non ancora in Augmented/Program Track: stesso controllo
            ' coordinate + autorizzazione operatore usato da Augmented.
            FrmProgram.ShowDialog(Me)
            If b_Abort Then Return
        Else
            AttivaProgramTrack()
            GoTo ExitProgTrack
        End If

        If PTK = &H10 Then
            b_ClosePreset = True : FrmPreset.Close()
            AttivaProgramTrack()
        Else
            If PTK = &H0 AndAlso IsDate(d_DataInizioProg) Then
                LblCommand.Text = "Program Track Enabled"
                i_StatusDiff = OP_PROGRAMTRACK
            End If
        End If
ExitProgTrack:
        b_StepTrack = False
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' --- AUTO SEARCH ---
    Public Sub CmdAutoSearch_Click(sender As Object, e As EventArgs) Handles CmdAutoSearch.Click
        AzOffset = 0 : ElOffset = 0
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False
        b_StepTrack = False

        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return

        b_ClosePreset = True : FrmPreset.Close()
        If AzInibhit AndAlso ElInibhit Then Return

        b_int(ACU) = True : b_Ricezione(ACU) = False
        Dim send As String
        If AzInibhit Then
            send = CheckSum("10EAQ ")
        ElseIf ElInibhit Then
            send = CheckSum("10AAQ ")
        Else
            send = CheckSum("10 AQ ")
        End If
        InvioACU(send)
        PTK = &H0
        If CmdOffset IsNot Nothing Then
            CmdOffset.Enabled = False : CmdOffset.Visible = False
        End If
        AzOffset = 0 : ElOffset = 0
        i_Status = OP_AUTOMATICAQUIRE
        LblCommand.Text = "Automatic Acquire"
        i_ProgTrack = 7
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' --- SPIRAL ACQUIRE ---
    Private Sub CmdSpirAcq_Click(sender As Object, e As EventArgs) Handles CmdSpirAcq.Click
        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return
        If AzInibhit AndAlso ElInibhit Then Return

        ' frmRicezioneRef non veniva mai assegnato: tutto il codice di
        ' frmAcquire che lo usa è dietro un "If frmRicezioneRef IsNot Nothing"
        ' e restava sempre no-op (timer mai avviato, campi mai copiati).
        frmAcquire.frmRicezioneRef = Me
        frmAcquire.ShowDialog(Me)
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        AzCalc = 0 : ElCalc = 0
        AzOffset = 0 : ElOffset = 0
        b_RangeAz = False : b_RangeEl = False
        LblCommand.Text = "Spiral Acquire"
        b_StepTrack = False
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
    End Sub

    ' -------------------------------------------------------
    ' TIMER SPIRAL/AUTOMATIC ACQUIRE e STEP TRACK — dispatcher
    ' periodico (porta a compimento la migrazione da VB6 di
    ' tmrSpiralAcquire_Timer: il Tick non aveva alcun gestore,
    ' quindi né l'Acquire né lo Step Track avanzavano mai dopo
    ' l'avvio).
    ' -------------------------------------------------------
    Private Sub TmrSpiralAcquire_Tick(sender As Object, e As EventArgs) Handles TmrSpiralAcquire.Tick
        If i_Status = OP_AUTOMATICAQUIRE OrElse i_Status = OP_SPIRALACQUIRE Then
            ' Il calcolo dei passi della spirale avviene solo se l'ACU non sta
            ' già ricevendo un messaggio ed è confermato in linea (PCS_OK,
            ' impostato in DecodificaTelemetriaACU al primo frame valido).
            If Port_Control(ACU).Port_State <> PCS_OK Then
                ' Prima il tick veniva saltato in silenzio: la ricerca
                ' restava ferma senza alcuna indicazione per l'operatore.
                SegnalaAcquireNonPossibile("ACU offline (no valid telemetry received)")
            ElseIf Port_Control(ACU).Rec_State <> PCR_RECEIVE Then
                ' (Rec_State = PCR_RECEIVE: messaggio in arrivo, si rinvia
                ' al tick successivo - non è una condizione d'errore.)
                Dim mSec As Integer = NowUTC().Millisecond
                Dim dataCalc As Date = NowUTC().AddSeconds(DeltaTime)
                Calcolo_Aquire(dataCalc, mSec)
            End If
            ' "FrmVisualAcquire.Visible" da solo, se la form non è mai stata
            ' aperta, ne crea comunque un'istanza di default nascosta (pattern
            ' VB.NET) solo per leggere la proprietà. Si cerca invece tra le
            ' form realmente aperte, senza istanziare nulla se non è aperta.
            Dim frmVisualAperta As FrmVisualAcquire = Nothing
            For Each f As Form In My.Application.OpenForms
                If TypeOf f Is FrmVisualAcquire Then
                    frmVisualAperta = CType(f, FrmVisualAcquire)
                    Exit For
                End If
            Next
            If frmVisualAperta IsNot Nothing AndAlso frmVisualAperta.Visible Then frmVisualAperta.VisualizzaGrafico()
        Else
            Calcolo_StepTrack()
        End If
    End Sub

    ' --- PROIEZIONE ---
    Private Sub CmdProi_Click(sender As Object, e As EventArgs) Handles CmdProi.Click
        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return

        d_DataInizioProg = ""
        MainForm.MessaggioStatus = ""
        FrmRegressione.ShowDialog(Me)

        Select Case grado
            Case 1 : NumProMax = 15
            Case 2 : NumProMax = 30
            ' Grado 3 (cubico): 45 punti (~50s alla cadenza di telemetria
            ' simulata) erano una finestra troppo ampia per un passaggio
            ' veloce — il moto reale non è polinomiale su una scala così
            ' lunga, quindi il fit si adattava bene ai punti ma estrapolava
            ' in modo assurdo appena fuori (es. azimut ed elevazione anche
            ' di decine di gradi sbagliati). Ridotta a una finestra più
            ' locale, dove l'approssimazione cubica resta valida.
            Case 3 : NumProMax = 20
            ' Grado 4 (media mobile, bersaglio quasi fermo): non serve una
            ' finestra lunga come per i fit polinomiali — 60 punti (oltre un
            ' minuto all'attuale cadenza di telemetria simulata, ~1,1s/punto)
            ' erano solo un'attesa inutile prima di vedere il primo risultato.
            Case 4 : NumProMax = 10
        End Select

        i_Status = OP_PROIEZ
        LblCommand.Text = "Projection"
        i_AzOffsetLock = 0
        LblAzimuth.Text = "" : LblElevazione.Text = ""
        LblAzimuthC.Text = "" : LblElevazioneC.Text = ""
        LblAzError.Text = "" : LblElError.Text = ""
        LblAzErrEn.Text = "" : LblElErrEn.Text = ""
        If TmrSpiralAcquire IsNot Nothing Then TmrSpiralAcquire.Enabled = False
        b_StepTrack = False
        i_Inter = CSng(Val(LeggiFileIni("Canale1", "Inter")))
        i_Cont = 1
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
        Exit Sub
Proiez:
        If Err.Number = 0 OrElse Err.Number = 20 Then Exit Sub
        WriteError(Err.Number, "frmRicezione - CmdProi " & Err.Description)
        Err.Clear() : Resume Next
    End Sub

    ' -------------------------------------------------------
    ' Pulsanti secondari
    ' -------------------------------------------------------

    Private Sub CmdLockBell_Click(sender As Object, e As EventArgs) Handles CmdLockBell_ctrl.Click
        If i_LockBell = 1 Then
            i_LockBell = 0
            CmdLockBell.Text = "No Bell"
        Else
            CmdLockBell.Text = "Bell"
            i_LockBell = 1
        End If
    End Sub

    Public Sub CmdOffset_Click(sender As Object, e As EventArgs) Handles CmdOffset.Click
        i_ProgTrack = 1
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
        FrmOffset.Show()
    End Sub

    ' Apre FrmLog (pannello di telemetria estesa, era "frmLog.Show" in
    ' cmdSum_Click del VB6 originale).
    Private Sub CmdSum_Click(sender As Object, e As EventArgs) Handles CmdSum.Click
        If TxtOrdini IsNot Nothing Then TxtOrdini.Focus()
        FrmLog.MdiParent = MainForm
        FrmLog.WindowState = FormWindowState.Maximized
        FrmLog.Show()
    End Sub

    Private Sub CmdPC1_Click(sender As Object, e As EventArgs) Handles CmdPC1_ctrl.Click
        If b_StatusPC1 Then
            LblPC1.Text = "LOC"
            LblPC1.BackColor = Color.Yellow
            b_StatusPC1 = False
        Else
            LblPC1.Text = "REM"
            LblPC1.BackColor = COL_VALIDO
            b_StatusPC1 = True
        End If
    End Sub

    Private Sub CmdReset_Click(sender As Object, e As EventArgs) Handles CmdReset.Click
        Try
            DbExecute("DELETE FROM [tmp_error_messages]")
            AggiornaGriglia()
        Catch
        End Try
    End Sub

    Private Sub CmdSound_Click(sender As Object, e As EventArgs) Handles CmdSound.Click
        Try
            CType(Me.MdiParent, FrmMain).TmrBeep.Enabled = False
        Catch
        End Try
    End Sub

    Private Sub CmdPhase_Click(sender As Object, e As EventArgs) Handles CmdPhase.Click
        If g_ChangePhase = "Y" Then
            g_ChangePhase = "N"
            CmdPhase.Text = "Y"
            LblPhase.BackColor = Color.Yellow
        Else
            g_ChangePhase = "Y"
            CmdPhase.Text = "N"
            LblPhase.BackColor = COL_VALIDO
        End If
    End Sub

    Private Sub ChkAz_Click(sender As Object, e As EventArgs) Handles chkAz.CheckedChanged
        If chkAz.Checked Then
            InvioACU(CheckSum("08AS"))
            AzInibhit = True
            TxtAz.BackColor = Color.Yellow
        Else
            AzInibhit = False
            TxtAz.BackColor = COL_DATO_NOTO
        End If
    End Sub

    Private Sub ChkEl_Click(sender As Object, e As EventArgs) Handles chkEl.CheckedChanged
        If chkEl.Checked Then
            InvioACU(CheckSum("08ES"))
            ElInibhit = True
            TxtEl.BackColor = Color.Yellow
        Else
            ElInibhit = False
            TxtEl.BackColor = COL_DATO_NOTO
        End If
    End Sub

    ' -------------------------------------------------------
    ' CARICAMENTO SATELLITE (CmdLoad)
    ' -------------------------------------------------------
    Private Sub CmdLoad_Click(sender As Object, e As EventArgs) Handles CmdLoad.Click
        Dim Agg As Boolean

        ScriviFileIni("Default", "Satellite", LblSatellite.Text)
        s_Satellite = LblSatellite.Text

        Dim rs_Load As Object = dbOrbite.OpenRecordset("Satelliti", dbOpenDynaset)
        If rs_Load.BOF OrElse String.IsNullOrEmpty(s_Satellite) OrElse s_Satellite = "REMOTE" Then
            MainForm.MessaggioStatus = "No information available for satellite "
            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
            b_Calcola = False
            LblSatellite.BackColor = Color.Yellow
            GoTo ExitLoad2
        Else
            rs_Load.FindFirst("NomeSatellite ='" & s_Satellite & "'")
            If Not rs_Load.NoMatch Then
                Algoritmo = CInt(rs_Load!Algoritmo)

                If rs_Load!Algoritmo = 0 OrElse rs_Load!Algoritmo = 2 Then
                    AggAlgo = CInt(rs_Load!Algoritmo)
                    b_Calcola = InitPar11()
                    If b_Calcola Then LblSatellite.BackColor = COL_DATO_NOTO

                ElseIf rs_Load!Algoritmo = 1 Then
                    If Not IsDBNull(rs_Load!FileAgg) AndAlso CStr(rs_Load!FileAgg) <> "" Then
                        s_TwoLine = CStr(rs_Load!FileAgg)
                        Dim idx As Integer = InStr(1, s_TwoLine, ":", 1)
                        If idx = 0 Then s_TwoLine = Application.StartupPath & "\" & s_TwoLine
                        Agg = AggiornaTLE(s_TwoLine, 2)
                        If Not Agg Then
                            LblSatellite.BackColor = Color.Yellow
                            b_Sat = False : b_Calcola = False
                            MainForm.MessaggioStatus = "TLE loading failed for satellite " & s_Satellite & " (file: " & s_TwoLine & ")"
                            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
                        Else
                            LblSatellite.BackColor = COL_DATO_NOTO
                            b_Sat = True : b_Calcola = True
                            g_Verso = CBool(rs_Load!Verso?.ToString().ToUpper() = "VERO" OrElse
                                           rs_Load!Verso?.ToString().ToUpper() = "TRUE")
                        End If
                    Else
                        LblSatellite.BackColor = COL_DATO_NOTO
                    End If

                ElseIf rs_Load!Algoritmo = 3 Then
                    s_FileStars = CStr(rs_Load!FileAgg)
                    SatChange = True
                    RiempiMemory(s_FileStars)
                    LblSatellite.BackColor = COL_DATO_NOTO
                    b_Calcola = True

                ElseIf rs_Load!Algoritmo = 4 Then
                    LblSatellite.BackColor = COL_DATO_NOTO
                    b_Calcola = True
                End If
            Else
                MainForm.MessaggioStatus = "No information available for satellite "
                If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
                LblSatellite.BackColor = Color.Yellow
                b_Calcola = False
                GoTo ExitLoad2
            End If
        End If
        rs_Load.Close()

        ' Selezione fase/frequenza/polarizzazione (frmPhase, era vbModal in VB6).
        ' La tabella Fasi può legittimamente non avere righe per questo
        ' satellite (non è un errore): si filtra già qui per satellite,
        ' così se non c'è nulla da scegliere si salta la maschera Fasi
        ' invece di aprirla vuota e trattare l'uscita come un annullamento.
        Dim OffAz As Single = 0, OffEl As Single = 0
        Dim rs_Fasi As Object = dbOrbite.OpenRecordset("SELECT * FROM [Fasi] WHERE Sat='" & s_Satellite & "'", dbOpenDynaset)
        If Not rs_Fasi.BOF AndAlso Algoritmo <> 3 AndAlso Algoritmo <> 4 Then
            b_Scelta = False
            Dim dlgPhase As New FrmPhase()
            dlgPhase.ShowDialog()
            If dlgPhase.Response = 1 Then
                DbExecute("UPDATE [Fasi] set Ultima = " & vbUnchecked)
                b_Checked = False
                rs_Fasi = dbOrbite.OpenRecordset("Fasi", dbOpenDynaset)
                rs_Fasi.FindFirst("Sat ='" & s_Satellite & "' and val(frequenza) = '" & Val(g_Frequenza) & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "'")
                If Not rs_Fasi.NoMatch Then
                    rs_Fasi.Edit()
                    rs_Fasi!Ultima = vbChecked
                    b_Checked = True
                    If Not IsDBNull(rs_Fasi!OffsetAz) Then OffAz = CSng(rs_Fasi!OffsetAz)
                    If Not IsDBNull(rs_Fasi!OffsetEl) Then OffEl = CSng(rs_Fasi!OffsetEl)
                    rs_Fasi.Update()
                Else
                    rs_Fasi.MoveFirst()
                    rs_Fasi.FindFirst("Sat ='" & s_Satellite & "'")
                    If rs_Fasi.EOF Then
                        rs_Fasi.AddNew()
                        rs_Fasi!Ultima = vbChecked
                        b_Checked = True
                        rs_Fasi!sat = s_Satellite
                        rs_Fasi.Update()
                    End If
                End If
                rs_Fasi.Close()
                If TxtPhase IsNot Nothing Then TxtPhase.Text = g_PhaseEl
                If TxtPhAz IsNot Nothing Then TxtPhAz.Text = g_PhaseAz
                If TxtLna IsNot Nothing Then TxtLna.Text = g_LNA
                If LblPolRic IsNot Nothing Then LblPolRic.Text = g_Polar
                t_Pol = g_Polar
                If TxtRevAz IsNot Nothing Then TxtRevAz.Text = g_RevAz
                PhaseValue1 = If(IsNumeric(g_PhaseEl), CSng(Val(g_PhaseEl)), 0)
                PhaseValue2 = If(IsNumeric(g_PhaseAz), CSng(Val(g_PhaseAz)), 0)
                i_Lna = If(IsNumeric(g_LNA), g_LNA, "1")
                g_NewFrequency = True
            Else
                ' Exit: esce senza scegliere una fase, l'antenna non viene abilitata
                g_Frequenza = "" : g_PhaseEl = "" : g_PhaseAz = "" : g_LNA = "" : g_Polar = "" : g_PolErr = "" : g_RevAz = ""
                If TxtPhase IsNot Nothing Then TxtPhase.Text = g_PhaseEl
                If TxtPhAz IsNot Nothing Then TxtPhAz.Text = g_PhaseAz
                If TxtLna IsNot Nothing Then TxtLna.Text = g_LNA
                If LblPolRic IsNot Nothing Then LblPolRic.Text = g_Polar
                t_Pol = g_Polar
                If TxtRevAz IsNot Nothing Then TxtRevAz.Text = g_RevAz
                LblSatellite.BackColor = Color.Yellow
                MainForm.MessaggioStatus = "No phase/frequency selected for satellite " & s_Satellite & " — load cancelled"
                If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
                Exit Sub
            End If
        Else
            rs_Fasi.Close()
            g_Frequenza = "" : g_PhaseEl = "" : g_PhaseAz = "" : g_LNA = "" : g_Polar = "" : g_PolErr = "" : g_RevAz = ""
            If TxtFrqBeacon IsNot Nothing Then TxtFrqBeacon.Text = ""
            If TxtPhase IsNot Nothing Then TxtPhase.Text = g_PhaseEl
            If TxtPhAz IsNot Nothing Then TxtPhAz.Text = g_PhaseAz
            If TxtLna IsNot Nothing Then TxtLna.Text = g_LNA
            If LblPolRic IsNot Nothing Then LblPolRic.Text = g_Polar
            t_Pol = g_Polar
            If TxtRevAz IsNot Nothing Then TxtRevAz.Text = g_RevAz
        End If

        ' Ricarica parametri TLE se algo 1
        If Algoritmo = 1 Then
            Define_Derived_Constants()
            b_Sat = Convert_Satellite_Data(s_Satellite)
            b_Calcola = b_Sat
            If b_Calcola Then LblSatellite.BackColor = COL_DATO_NOTO
        ElseIf Algoritmo = 0 OrElse Algoritmo = 2 Then
            b_Calcola = InitPar11()
            If b_Calcola Then LblSatellite.BackColor = COL_DATO_NOTO
        Else
            b_Calcola = True
            LblSatellite.BackColor = COL_DATO_NOTO
        End If

        ' Riallinea "Ultima" e invia gli offset di fase all'ACU
        DbExecute("UPDATE [Fasi] set Ultima = " & vbUnchecked)
        b_Checked = False
        Dim rs_End As Object = dbOrbite.OpenRecordset("Fasi", dbOpenDynaset)
        rs_End.FindFirst("sat ='" & LblSatellite.Text & "' and frequenza = '" & Format(Val(g_Frequenza), "#000.000") & "' and Pol = '" & g_Polar & "' and Lna ='" & g_LNA & "'")
        If Not rs_End.NoMatch Then
            rs_End.Edit()
            rs_End!Ultima = vbChecked
            b_Checked = True
            rs_End.Update()
        End If
        If Not rs_End.EOF AndAlso Not IsDBNull(rs_End!OffsetAz) Then OffAz = CSng(rs_End!OffsetAz)
        If Not rs_End.EOF AndAlso Not IsDBNull(rs_End!OffsetEl) Then OffEl = CSng(rs_End!OffsetEl)
        rs_End.Close()

        Dim valoffset As String
        valoffset = " "
        If OffAz <> 0 Then
            If OffAz >= 0 AndAlso OffAz <= 99 Then
                valoffset = CheckSum("14MOFA +" & Format(Math.Abs(OffAz), "00"))
            ElseIf OffAz >= -99 AndAlso OffAz < 0 Then
                valoffset = CheckSum("14MOFA -" & Format(Math.Abs(OffAz), "00"))
            End If
            i_Cont = 2
            Attesa(0.2)
            InvioACU(valoffset)
        End If
        If OffEl <> 0 Then
            If OffEl >= 0 AndAlso OffEl <= 99 Then
                valoffset = CheckSum("14MOFE +" & Format(Math.Abs(OffEl), "00"))
            ElseIf OffEl >= -99 AndAlso OffEl < 0 Then
                valoffset = CheckSum("14MOFE -" & Format(Math.Abs(OffEl), "00"))
            End If
            i_Cont = 2
            Attesa(0.2)
            InvioACU(valoffset)
        End If
        i_Cont = 0
        If IsNumeric(g_Frequenza) AndAlso Val(g_Frequenza) > 0 Then
            CmbBcnFreq.Text = Format(Val(g_Frequenza), "#0000.000")
        End If
        InvioRic()
        g_SatelliteCaricato = s_Satellite   ' caricamento completato: Trace Orbit riparte da zero
        PredisponiAcquisizione()
        TxtOrdini.Focus()

ExitLoad2:
        Exit Sub
ErrLoad2:
        If Err.Number = 0 OrElse Err.Number = 20 Then Exit Sub
        WriteError(Err.Number, "frmRicezione - CmdLoad " & Err.Description)
        Err.Clear() : Resume Next
    End Sub

    ' -------------------------------------------------------
    ' Selezione satellite nella ComboBox
    ' -------------------------------------------------------
    Private Sub LblSatellite_SelectedIndexChanged(sender As Object, e As EventArgs)   ' wired via AddHandler in Load â€” LblSatellite non ha WithEvents nel Designer
        Dim giaAttivo As Boolean = (LblSatellite.Text = s_Satellite)

        ' Sole/Luna/Venere/Cassiopeia (Algoritmo=4): niente TLE da caricare
        ' né fase/frequenza da scegliere, quindi si applicano subito alla
        ' sola selezione, senza richiedere "Exe" — a differenza dei
        ' satelliti veri (TLE/11 parametri), che restano "in attesa"
        ' (giallo) finché non si preme Exe. Il controllo gira anche se il
        ' testo coincide già con s_Satellite (es. al riavvio, quando
        ' PopolaLblSatellite ripristina l'ultimo satellite salvato): senza
        ' questo, il colore restava quello di default del Designer
        ' (giallo) e un nuovo SelectedIndexChanged non scattava più,
        ' perché la ComboBox non segnala un "cambio" se riselezioni la
        ' voce già attiva.
        If ApplicaSatelliteSeAlgoritmo4() Then Exit Sub
        If giaAttivo Then Exit Sub

        LblSatellite.BackColor = Color.Yellow
    End Sub

    ' Se il satellite indicato in LblSatellite.Text ha Algoritmo=4 (Sole,
    ' Luna, Venere, Cassiopeia: nessun TLE/fase da caricare), lo applica
    ' subito (s_Satellite, Algoritmo, b_Calcola, verde) e restituisce True.
    ' Altrimenti non tocca nulla e restituisce False.
    Private Function ApplicaSatelliteSeAlgoritmo4() As Boolean
        If dbOrbite Is Nothing OrElse String.IsNullOrEmpty(LblSatellite.Text) Then Return False
        Dim rs_Sel As Object = dbOrbite.OpenRecordset("Satelliti", dbOpenDynaset)
        If Not rs_Sel.BOF Then
            rs_Sel.FindFirst("NomeSatellite ='" & LblSatellite.Text & "'")
            If Not rs_Sel.NoMatch AndAlso CInt(rs_Sel!Algoritmo) = 4 Then
                s_Satellite = LblSatellite.Text
                Algoritmo = 4
                b_Calcola = True
                ScriviFileIni("Default", "Satellite", s_Satellite)
                g_SatelliteCaricato = s_Satellite
                LblSatellite.BackColor = COL_DATO_NOTO
                rs_Sel.Close()
                Return True
            End If
        End If
        rs_Sel.Close()
        Return False
    End Function

    ' -------------------------------------------------------
    ' Beacon scale â€” modifica da tastiera (ENTER)
    ' -------------------------------------------------------
    Private Sub LblBcnMax_KeyUp(sender As Object, e As KeyEventArgs) Handles LblBcnMax_ctrl.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(LblBcnMax.Text) Then
                ScalaMax = CDbl(LblBcnMax.Text)
                ScriviFileIni("Default", "ScalaMax", LblBcnMax.Text)
                AggiornaBcnBar()
            Else
                LblBcnMax.Text = ""
            End If
        End If
    End Sub

    Private Sub LblBcnMin_KeyUp(sender As Object, e As KeyEventArgs) Handles LblBcnMin_ctrl.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(LblBcnMin.Text) Then
                ScalaMin = CDbl(LblBcnMin.Text)
                ScriviFileIni("Default", "ScalaMin", LblBcnMin.Text)
                AggiornaBcnBar()
            Else
                LblBcnMin.Text = ""
            End If
        End If
    End Sub

    Private Sub AggiornaBcnBar()
        If Not (IsNumeric(ScalaMin) AndAlso IsNumeric(ScalaMax) AndAlso ScalaMax > ScalaMin) Then Return
        BarBeacon.Minimum = 0
        BarBeacon.Maximum = CInt(Math.Abs(CDbl(ScalaMax) - CDbl(ScalaMin)))
        Dim diff As Double = CDbl(ScalaMax) - CDbl(ScalaMin)
        If IsNumeric(s_Lev1) Then
            If CDbl(s_Lev1) > CDbl(ScalaMax) Then
                BarBeacon.Value = BarBeacon.Maximum
            ElseIf CDbl(s_Lev1) < CDbl(ScalaMin) Then
                BarBeacon.Value = 0
            Else
                BarBeacon.Value = CInt(Math.Abs(CDbl(s_Lev1) - CDbl(ScalaMin)))
            End If
        End If
    End Sub

    ' =======================================================
    ' MENU HANDLERS
    ' =======================================================

    ' --- Menu File ---
    Private Sub MnuFileConf_Click(sender As Object, e As EventArgs) Handles MnuConfiguration.Click
        FrmConfigurazione.Show()
    End Sub

    Private Sub MnuFileBell_Click(sender As Object, e As EventArgs) Handles MnuBell.Click
        MnuBell.Checked = Not MnuBell.Checked
        MainForm.MessaggioStatus = If(MnuBell.Checked, "Bell ON", "Bell OFF")
    End Sub

    Private Sub MnuFileExit_Click(sender As Object, e As EventArgs) Handles MnuExit.Click
        b_ClosePreset = True
        FrmPreset.Close()
        Application.Exit()
    End Sub

    ' --- Menu Receiver ---
    Private Sub MnuMainReceiver_Click(sender As Object, e As EventArgs) Handles MnuReceiver.Click
        FrmRicevitore.Close()
        FrmRicevitore.Show()
    End Sub

    ' --- Menu Phase ---
    Private Sub MnuMainPhase_Click(sender As Object, e As EventArgs) Handles MnuPhase.Click
        ' Mostra solo le fasi del satellite corrente (coerente con l'altro punto
        ' di apertura di FrmPhase, la fasatura in avvio Program/AutoTrack).
        b_Scelta = False
        FrmPhase.Show()
    End Sub

    ' --- Menu Beacon Graphic ---
    Private Sub MnuMainGraphic_Click(sender As Object, e As EventArgs) Handles MnuBeaconGraphic.Click
        frmGrafico.frmRicezioneRef = Me
        frmGrafico.Show()
    End Sub

    ' --- Menu Antenna ---
    Private Sub MnuMainAntenna_Click(sender As Object, e As EventArgs) Handles MnuAntenna.Click
        FrmAntenne.Show()
    End Sub

    ' --- Menu Satellite ---
    Private Sub MnuMainSatellite_Click(sender As Object, e As EventArgs) Handles MnuSatelliteMenu.Click
        FrmSatelliti.Show()
    End Sub

    ' --- Menu Down Converter ---
    Private Sub MnuMainDC_Click(sender As Object, e As EventArgs) Handles MnuDownConverter.Click
        FrmDownConverter.ShowDialog(Me)
    End Sub

    ' --- Menu Parameter ACU ---
    Private Sub MnuMainParACU_Click(sender As Object, e As EventArgs) Handles MnuParameterACU.Click
        g_AggPar = True
        FrmParametri.Show()
    End Sub

    ' --- Menu Simulation ---
    Private Sub MnuMainSim_Click(sender As Object, e As EventArgs) Handles MnuSimulation.Click
        FrmSimulazione.Show()
    End Sub

    ' --- Menu Trace Orbit (Polare) ---
    Private Sub MnuMainPolar_Click(sender As Object, e As EventArgs) Handles MnuTraceOrbit.Click
        g_GrafPol = False
        FrmPolare.Show()
        FrmPolare.Activate()   ' se era già aperta: in primo piano, Activated rilegge g_GrafPol
    End Sub

    ' --- Menu Ephemeris Orbit > Polar (grafico polare storico da archivio) ---
    Private Sub MnuOrbitPolar_Click(sender As Object, e As EventArgs) Handles MnuOrbitPolar.Click
        g_GrafPol = True
        FrmPolare.Show()
        FrmPolare.Activate()   ' se era già aperta: in primo piano, Activated rilegge g_GrafPol
    End Sub

    ' --- Menu Ephemeris Orbit > Cartesian Axis (grafico storico Azimuth/Elevazione) ---
    Private Sub MnuOrbitCartesian_Click(sender As Object, e As EventArgs) Handles MnuOrbitCartesian.Click
        FrmVisualizzazione.Show()
    End Sub

    ' --- Menu Acquire Trace ---
    Private Sub MnuMainAcquire_Click(sender As Object, e As EventArgs) Handles MnuAcquireTrace.Click
        Try
            FrmVisualAcquire.Show()
        Catch ex As Exception
            WriteError(0, "MnuMainAcquire_Click (FrmVisualAcquire.Show): " & ex.Message)
        End Try
    End Sub

    ' --- Menu Alarm ---
    Private Sub MnuMainAlarm_Click(sender As Object, e As EventArgs) Handles MnuAlarm.Click
        frmAllarmi.Show()
    End Sub

    ' --- Menu URL ---
    Private Sub MnuMainURL_Click(sender As Object, e As EventArgs) Handles MnuURL.Click
        FrmUrl.Show()
    End Sub

    ' --- Menu About ---
    Private Sub MnuMainAbout_Click(sender As Object, e As EventArgs) Handles MnuInfo.Click
        FrmSplash.Show()
    End Sub

    ' --- Menu Ephemeris ---
    Private Sub MnuEphCassiopea_Click(sender As Object, e As EventArgs) Handles MnuEphCassiopea.Click
        i_Ephemeris = 0 : FrmEffemeridi.ShowDialog(Me)
    End Sub

    Private Sub MnuEphMoon_Click(sender As Object, e As EventArgs) Handles MnuEphMoon.Click
        i_Ephemeris = 1 : FrmEffemeridi.ShowDialog(Me)
    End Sub

    Private Sub MnuEphSatellite_Click(sender As Object, e As EventArgs) Handles MnuEphSatellite.Click
        If Algoritmo = 3 Then
            FrmTrackTable.Show()
        Else
            i_Ephemeris = 2
            FrmEffemeridi.ShowDialog(Me)
        End If
    End Sub

    Private Sub MnuEphSun_Click(sender As Object, e As EventArgs) Handles MnuEphSun.Click
        i_Ephemeris = 3 : FrmEffemeridi.ShowDialog(Me)
    End Sub

    Private Sub MnuEphVenus_Click(sender As Object, e As EventArgs) Handles MnuEphVenus.Click
        i_Ephemeris = 4 : FrmEffemeridi.ShowDialog(Me)
    End Sub

    ' --- Menu Update TLE ---
    Private Sub MnuTLEManual_Click(sender As Object, e As EventArgs) Handles MnuTLEManual.Click
        s_Satellite = LblSatellite.Text
        FrmNorad.ShowDialog(Me)
        Define_Derived_Constants()
        b_Sat = Convert_Satellite_Data(s_Satellite)
        b_Calcola = b_Sat
        LblSatellite.BackColor = If(b_Calcola, COL_DATO_NOTO, COL_DATO_IGNOTO)
    End Sub

    Private Sub MnuTLETwoLine_Click(sender As Object, e As EventArgs) Handles MnuTLETwoLine.Click
        frmBrowse.ShowDialog(Me)
        If g_Parametri <> "" Then
            Dim agg As Boolean = AggiornaTLE(g_Parametri, 1)
            LblSatellite.BackColor = If(agg, COL_DATO_NOTO, COL_DATO_IGNOTO)
        End If
    End Sub

    Private Sub MnuTLEThreeLine_Click(sender As Object, e As EventArgs) Handles MnuTLEThreeLine.Click
        frmBrowse.ShowDialog(Me)
        If g_Parametri <> "" Then
            Dim agg As Boolean = AggiornaTLE(g_Parametri, 2)
            LblSatellite.BackColor = If(agg, COL_DATO_NOTO, COL_DATO_IGNOTO)
        End If
    End Sub

    Private Sub MnuTLETable_Click(sender As Object, e As EventArgs) Handles MnuTLETableTrack.Click
        frmBrowse.ShowDialog(Me)
        If g_Parametri <> "" Then
            Dim agg As Boolean = RiempiMemory(g_Parametri)
            LblSatellite.BackColor = If(agg, COL_DATO_NOTO, COL_DATO_IGNOTO)
        End If
    End Sub

    ' --- Menu ACU Comandi ---
    Private Sub MnuComStandBy_Click(sender As Object, e As EventArgs) Handles MnuStandBy.Click
        CmdSTBY_Click(sender, e)
    End Sub

    Private Sub MnuComStepTrack_Click(sender As Object, e As EventArgs) Handles MnuStepTrack.Click
        CmdStepTrack_Click(sender, e)
    End Sub

    Public Sub MnuComMemoryTrack_Click(sender As Object, e As EventArgs) Handles MnuMemoryTrack.Click
        If s_ACULR = "L" Then MostraWarning("ACU in Local") : Return
        If i_Alarm Then MostraWarning("Command Inhibit") : Return
        b_ClosePreset = True : FrmPreset.Close()
        b_int(ACU) = True : b_Ricezione(ACU) = False
        Dim send As String
        If AzInibhit Then
            send = CheckSum("08EM")
        ElseIf ElInibhit Then
            send = CheckSum("08AM")
        Else
            send = CheckSum("08 M")
        End If
        InvioACU(send)
        PTK = &H0
        d_DataInizioProg = ""
        LblCommand.Text = "Memory Track"
        i_Status = OP_MEMORYTRACK
    End Sub

    Public Sub MnuComSatellite_Click(sender As Object, e As EventArgs) Handles MnuSatelliteCmd.Click
        CmdLoad_Click(sender, e)
    End Sub

    Public Sub MnuComStow_Click(sender As Object, e As EventArgs) Handles MnuStow.Click
        InvioACU(CheckSum("07WD"))
        LblCommand.Text = "Stow"
    End Sub

    Public Sub MnuComUnstow_Click(sender As Object, e As EventArgs) Handles MnuUnstow.Click
        InvioACU(CheckSum("07WE"))
        LblCommand.Text = "Unstow"
    End Sub

    ' --- Menu ACU Richieste ---
    Private Sub MnuInterStatus_Click(sender As Object, e As EventArgs) Handles MnuReqStatus.Click
        b_Ricezione(ACU) = True : b_int(ACU) = True
        InvioACU(STX & "07*2F" & ETX)
    End Sub

    Private Sub MnuInterParameter_Click(sender As Object, e As EventArgs) Handles MnuReqParameter.Click
        b_Ricezione(ACU) = True : b_int(ACU) = True
        InvioACU(STX & "07?3A" & ETX)
    End Sub

    Private Sub MnuInterSatData_Click(sender As Object, e As EventArgs) Handles MnuSatelliteData.Click
        FrmTabSat.Show()
    End Sub



    Private Sub MnuInterMemTrack_Click(sender As Object, e As EventArgs) Handles MnuMemoryTrackData.Click
        FrmMemoryTrack.Show()
    End Sub

    ' --- Menu Preset (wired via AddHandler in Load) ---
    Private Sub MnuPreset_Click(sender As Object, e As EventArgs)
        FrmPreset.Show(Me)
    End Sub

    ' -------------------------------------------------------
    ' KEY DOWN â€” tasti funzione
    ' -------------------------------------------------------
    Private Sub FrmRicezione_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1 To Keys.F8
                OrdineKey(CInt(e.KeyCode), 0)
                e.Handled = True
        End Select
    End Sub

    ' -------------------------------------------------------
    ' Timer allarme (lampeggio warning)
    ' -------------------------------------------------------
    Private Sub TmrAlarm_Tick(sender As Object, e As EventArgs) Handles tmrAlarm.Tick
        ' VB6: tmrAlarm è un timer one-shot che cancella il warning e si
        ' autodisabilita (non un lampeggio — riusava per errore i_Blink,
        ' condiviso con la funzione generica di blink degli indicatori,
        ' lasciando il warning visibile a intermittenza per sempre).
        MostraWarning("")
    End Sub

    ' MostraWarning Ã¨ definita in frmRicezione.Visualizzazione.vb (Public)

    ' -------------------------------------------------------
    ' Timer ricezione principale
    ' -------------------------------------------------------
    Private Sub TmrRic_Tick(sender As Object, e As EventArgs) Handles TmrRic.Tick
        Try
            CalcolaEInvia()
        Catch
        End Try
    End Sub

End Class