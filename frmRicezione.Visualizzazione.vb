' ============================================================
' frmRicezione.Visualizzazione.vb
' Telespazio BTS1 â€” DYNATRON
' ============================================================
' Partial class dedicata esclusivamente alla visualizzazione:
'   - Aggiornamento label antenna (Az/El, errori, comando)
'   - Indicatori di stato canali (CNL1..CNL4)
'   - Beacon / livello segnale
'   - Stato fase e lock ricevitore
'   - Blink indicatori
'   - Metodi helper colore/testo
' Nessuna logica di protocollo o comunicazione in questo file.
' ============================================================

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class FrmRicezione

    ' -------------------------------------------------------
    ' Costanti colore di stato (consistenza visiva)
    ' -------------------------------------------------------
    Private Shared ReadOnly COL_OK       As Color = COL_VALIDO
    Private Shared ReadOnly COL_WARN     As Color = COL_NON_COLLEGATO
    Private Shared ReadOnly COL_FAIL     As Color = COL_ALLARME
    Private Shared ReadOnly COL_IDLE     As Color = Color.Gray
    Private Shared ReadOnly COL_BLINK_ON As Color = Color.Orange
    Private Shared ReadOnly COL_BLINK_OFF As Color = Color.DarkOrange

    ' -------------------------------------------------------
    ' Aggiorna tutti i dati visualizzati sul form
    ' (da chiamare dal timer principale o dopo ogni ricezione)
    ' -------------------------------------------------------
    Public Sub AggiornaTuttiDati()
        AggiornaAntenna()
        AggiornaErroriTracking()
        AggiornaAllarmi()
        AggiornaModoOperativo()
        AggiornaStatoCanali()
        AggiornaBeaconLock()
        AggiornaRicevitore()
        AggiornaFase()
        AggiornaPolLnaDaACU()
    End Sub

    ''' <summary>
    ''' C10 (VB6 AggiornaDati): LNA in linea e polarizzazione letti dal byte
    ''' 10 dello stato ACU (in FOC6 venivano dai cassetti LNA/SCU, assenti
    ''' in C10). Un cambio di LNA o di polarizzazione imposta g_NewLna:
    ''' con ChangePhase = "Y" frmMain carica e invia la fase corrispondente.
    ''' </summary>
    Public Sub AggiornaPolLnaDaACU()
        If Not b_DatiAntenna OrElse CampoOp <> "*" Then Return
        Dim ricChange As Boolean = False

        If IsNumeric(Mid$(s_Az, 1, 6)) Then TxtAz.Text = Format$(Val(Mid$(s_Az, 1, 6)) / 1000, "000.000")
        If IsNumeric(Mid$(s_El, 1, 5)) Then TxtEl.Text = Format$(Val(Mid$(s_El, 1, 5)) / 1000, "00.000")
        TxtAzEr.Text = Format$(Val(s_LevAz), "###0.00")   ' K09: due decimali
        TxtElEr.Text = Format$(Val(s_LevEl), "###0.00")

        Dim lnaPrima As String = LblLNA.Text
        If (vBytesold(10) And BIT1) = BIT1 Then
            LblLNA.Text = "1"
        ElseIf (vBytesold(10) And BIT2) = BIT2 Then
            LblLNA.Text = "2"
        ElseIf (vBytesold(10) And BIT3) = BIT3 Then
            LblLNA.Text = "3"
        End If
        If lnaPrima <> LblLNA.Text Then ricChange = True
        g_LNA = LblLNA.Text

        ' K09: polarizzazione dell'errore (Pol.Err.) dal BIT4 del byte 10; un
        ' cambio fa ricaricare la fase (le fasi sono indicizzate per Pol.Err.)
        Dim polErrPrima As String = TxtPolErr.Text
        TxtPolErr.Text = If((vBytesold(10) And BIT4) = BIT4, "RHCP", "LHCP")
        If polErrPrima <> TxtPolErr.Text AndAlso polErrPrima <> "" Then ricChange = True
        g_PolErr = TxtPolErr.Text

        ' K09: senso della polarizzazione dal BIT3 del byte 9: Vertical/Horizontal
        ' in lineare, RHCP/LHCP in circolare (nessun "Running": il controllo
        ' del Preset polarizzazione è commentato nel VB6 di frmRicezione)
        Dim polPrima As String = LblPol.Text
        Dim senso As Boolean = (vBytesold(9) And BIT3) = BIT3
        Select Case i_Pol
            Case 0
                LblPol1.Text = "Linear"
                LblPol.Text = If(senso, "Vertical", "Horizontal")
                If IsNumeric(Mid$(s_POL, 1, 4)) Then TxtPol.Text = Format$(Val(Mid$(s_POL, 1, 4)) / 10, "#00.0")
            Case 1
                TxtPol.Text = ""
                LblPol1.Text = "Circular"
                LblPol.Text = If(senso, "RHCP", "LHCP")
        End Select
        If polPrima <> LblPol.Text Then ricChange = True
        g_Polar = LblPol.Text

        If ricChange Then g_NewLna = True
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 2c — Modo operativo, MDU, Back-up Auto Track
    ' -------------------------------------------------------

    ''' <summary>
    ''' Porting diretto da VB6 frmRicezione.frm (AggiornaDati, righe 6574-
    ''' 6607, 6697-6698, 6892-6921): testo del modo operativo (TxtMode/
    ''' s_OpMode), indicatore MDU (Local/Remote drive status) e le due
    ''' label di stato Back-up Auto Track/Switches, mai portate.
    ''' </summary>
    Public Sub AggiornaModoOperativo()
        If Not b_DatiAntenna Then Return

        If TxtMode IsNot Nothing Then
            Select Case (s_Mod And &HF)
                Case 0 : TxtMode.Text = "Stand By"
                Case 1 : TxtMode.Text = "Preset"
                Case 2 : TxtMode.Text = "Manual Slew"
                Case 3 : TxtMode.Text = "Manual Velocity"
                Case 4 : TxtMode.Text = " Monopulse"
                Case 5 : TxtMode.Text = "Step Track"
                Case 6 : TxtMode.Text = "Memory Track"
                Case 7 : TxtMode.Text = "Automatic Acquire"
                Case 8 : TxtMode.Text = "Satellite"
                Case 9 : TxtMode.Text = ""
                Case 10 : TxtMode.Text = "Memory rate"
                Case 11 : TxtMode.Text = "Automatic Stow"
                Case 12 : TxtMode.Text = "Automatic Un-Stow"
                Case 13 : TxtMode.Text = "Pin In"
                Case 14 : TxtMode.Text = "Pin Out"
                Case 15 : TxtMode.Text = "Step Preset"
            End Select
            s_OpMode = TxtMode.Text
        End If

        ' M&C ACU (Remote/Local) — mai portato da VB6, restava sempre vuoto.
        If TxtACU_ctrl IsNot Nothing Then
            TxtACU_ctrl.Text = If((vBytesold(1) And BIT6) = BIT6, "REM", "LOC")
            TxtACU_ctrl.BackColor = If((vBytesold(1) And BIT6) = BIT6, COL_OK, COL_FAIL)
        End If

        ' Sum.Alarm — porting diretto da VB6 (frmRicezione.frm:6571), mai
        ' collegato in .NET: restava sempre col colore di default del
        ' Designer (giallo), indipendentemente dallo stato reale.
        ' Come nel VB6 si colora l'indicatore LblSumAlarm_ctrl (lblSumAlarm);
        ' CmdSum resta un normale bottone grigio che apre FrmLog.
        If LblSumAlarm_ctrl IsNot Nothing Then
            LblSumAlarm_ctrl.BackColor = If((vBytesold(1) And BIT5) = BIT5, COL_FAIL, COL_OK)
        End If

        ' Back-up Auto Track (riquadri in basso a destra).
        If LblBackUp IsNot Nothing Then
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
        End If

        If LblSwitches IsNot Nothing Then
            Select Case i_Switches
                Case "T" : LblSwitches.Text = "Step-Track"
                Case "S" : LblSwitches.Text = "Stand-By"
                Case "P" : LblSwitches.Text = "Preset"
                Case Else : LblSwitches.Text = ""
            End Select
        End If
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 2d — Livello beacon e Lock (aggiornamento VB6 dell'11/10/2026)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Porting dall'aggiornamento dell'11/10/2026 di VB6 AggiornaDati:
    ''' livello beacon (LblLev_ctrl/BarBeacon), con fallback sui dati grezzi
    ''' ACU quando il canale ricevitore (RX, LblConfCNL3in) non è collegato.
    ''' "PCS_NORMAL_COL"/"vbGreen" del VB6 sono stati unificati su COL_OK: è
    ''' l'unico colore che ImpostaIndicatoreCanale assegna davvero a
    ''' LblConfCNL3in quando il canale è OK — un confronto letterale con
    ''' quelle costanti VB6 non sarebbe mai risultato vero in questo port
    ''' (ImpostaIndicatoreCanale non le usa). Richiede AggiornaStatoCanali()
    ''' già eseguita in questo stesso ciclo.
    ''' Il Lock (LblLock) NON dipende dal collegamento RX: nel VB6 originale
    ''' (frmRicezione.frm riga 6704) è calcolato in modo identico e
    ''' incondizionato dal solo bit ACU vBytesold(13) BIT2, sempre — una
    ''' precedente versione di questo port introduceva qui un ramo
    ''' alternativo con vBytesold(9) BIT4 quando RX non è collegato: quel
    ''' bit nel VB6 è l'allarme dei motori tracking (lblAzM2TS/lblElM2TS),
    ''' estraneo al Lock, e produceva un falso UNLOCK con RX scollegato.
    ''' </summary>
    Public Sub AggiornaBeaconLock()
        If Not b_DatiAntenna Then Return

        If LblLev_ctrl IsNot Nothing Then LblLev_ctrl.BackColor = COL_OK
        If LblLev_ctrl IsNot Nothing AndAlso IsNumeric(s_Lev1) Then LblLev_ctrl.Text = s_Lev1

        ' LockRic ricalcolato a ogni ciclo dal bit reale ACU (C10: vBytesold(9)
        ' BIT4) — era solo letto (mai scritto) prima di questa modifica,
        ' quindi restava sempre vuoto.
        If (vBytesold(9) And BIT4) = BIT4 Then          ' C10: lock = byte 9 bit4 (BTS1: byte 13 bit2 a 0)
            LockRic = "LOCK"
            s_StatusPTIC(3) = s_StatusPTIC(3) Or &H2
        Else
            LockRic = "UNLOCK"
            s_StatusPTIC(3) = s_StatusPTIC(3) And &HD
        End If
        If LblLock IsNot Nothing Then
            LblLock.Text = LockRic
            LblLock.BackColor = If(LockRic = "LOCK", COL_OK, COL_FAIL)
        End If

        Dim rxCollegato As Boolean = (LblConfCNL3in IsNot Nothing AndAlso LblConfCNL3in.BackColor = COL_OK)

        If rxCollegato Then
            ' Confronto comandato/effettivo: frequenza, LNA, polarizzazione
            If TxtFrqBeacon IsNot Nothing AndAlso TxtInpFreq IsNot Nothing AndAlso
               IsNumeric(TxtFrqBeacon.Text) AndAlso Val(TxtFrqBeacon.Text) > 0 Then
                TxtInpFreq.ForeColor = If(TxtFrqBeacon.Text <> TxtInpFreq.Text, COL_FAIL, Color.Black)
            End If
            If LblLNA IsNot Nothing AndAlso TxtLna IsNot Nothing Then
                TxtLna.ForeColor = If(LblLNA.Text <> TxtLna.Text, COL_FAIL, Color.Black)
            End If
            If LblPol IsNot Nothing AndAlso LblPolRic IsNot Nothing Then
                LblPolRic.ForeColor = If(LblPol.Text <> LblPolRic.Text, COL_FAIL, Color.Black)
            End If
            If TxtInpFreq IsNot Nothing Then
                TxtInpFreq.Text = Format$(RfInputFrequency / 1000, "#0000.000")
            End If
        Else
            ' RX non collegato: usa i valori grezzi dal buffer ACU
            ' (s_Lev/s_LevAz/s_LevEl) al posto di quelli normalmente
            ' forniti dal ricevitore (s_Lev1/s_LevAz1/s_LevEl1).
            If LblLev_ctrl IsNot Nothing Then LblLev_ctrl.BackColor = COL_WARN
            s_LevAz1 = s_LevAz
            s_LevEl1 = s_LevEl
            s_Lev1 = s_Lev
            If LblLev_ctrl IsNot Nothing AndAlso IsNumeric(s_Lev1) Then LblLev_ctrl.Text = s_Lev1

            If IsNumeric(s_Lev1) AndAlso BarBeacon IsNot Nothing Then
                Dim lvl1 As Double = Val(s_Lev1)
                If lvl1 > ScalaMax Then
                    BarBeacon.Value = BarBeacon.Maximum
                ElseIf lvl1 < ScalaMin Then
                    BarBeacon.Value = BarBeacon.Minimum
                Else
                    BarBeacon.Value = CInt(Math.Max(BarBeacon.Minimum, Math.Min(BarBeacon.Maximum, Math.Abs(lvl1 - ScalaMin))))
                End If
            End If
        End If

        ' Primo aggancio del lock (soglia superata, non ancora agganciato) —
        ' controllato sia sul valore grezzo ACU (s_Lev) sia su quello del
        ' ricevitore (s_Lev1), nell'ordine del VB6 originale.
        If LblLock IsNot Nothing Then
            If LblLock.Text = "LOCK" AndAlso Not b_Lock AndAlso IsNumeric(s_Lev) AndAlso Val(s_Lev) >= i_LockThr Then b_Lock = True
            If LblLock.Text = "LOCK" AndAlso Not b_Lock AndAlso IsNumeric(s_Lev1) AndAlso Val(s_Lev1) >= i_LockThr Then b_Lock = True
            LblLock.BackColor = If((vBytesold(9) And BIT4) = BIT4, COL_OK, COL_FAIL)       ' C10: byte 9 bit4
        End If
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 1 â€” Posizione antenna (Az / El calcolati e comandati)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna azimuth ed elevazione calcolati (da orbita/TLE).
    ''' </summary>
    Public Sub AggiornaAntenna()
        If Not b_DatiAntenna Then Return

        ' Colonna "Antenna": valore così come arriva dall'ACU (s_Az/s_El,
        ' valorizzate da ValoreCampoMAC), non elaborato dal programma.
        ' "Command" e "Program" (LblAzimuth(C)/LblElevazione(C)) restano
        ' puliti qui: si valorizzano altrove, solo per le operazioni che
        ' comandano davvero l'antenna.
        ' Formattazione con Format$ (locale corrente, virgola decimale) per
        ' coerenza con Command/Program/Offset e tutti gli altri campi
        ' numerici del form: prima usava InvariantCulture (punto), unico
        ' campo con separatore diverso dal resto della maschera.
        If TxtAz IsNot Nothing AndAlso IsNumeric(s_Az) Then
            TxtAz.Text = Format$(Val(s_Az) / 1000, "000.000")
        End If
        If TxtEl IsNot Nothing AndAlso IsNumeric(s_El) Then
            TxtEl.Text = Format$(Val(s_El) / 1000, "00.000")
        End If

        ' Colonna "Error": livello errore di tracking Az/El, dal buffer ACU
        ' (s_LevAz/s_LevEl, nessuna scala — porting diretto da VB6
        ' "TxtAzEr = s_LevAz" in frmRicezione.frm/lblRVR_Change).
        If TxtAzEr IsNot Nothing Then TxtAzEr.Text = Format$(Val(s_LevAz), "###0.00")   ' K09: due decimali
        If TxtElEr IsNot Nothing Then TxtElEr.Text = Format$(Val(s_LevEl), "###0.00")

        ' Freni di tracking (VB6 frmRicezione di FOCK09, fa testo per la lettura):

        ' vBytesold(5), El = BIT3, Az = BIT4; "RELEASED" serve anche all'Augmented.

        If LblElTBrk IsNot Nothing Then LblElTBrk.Text = If((vBytesold(5) And BIT3) = BIT3, "RELEASED", "APPLIED ")

        If LblAzTBrk IsNot Nothing Then LblAzTBrk.Text = If((vBytesold(5) And BIT4) = BIT4, "RELEASED", "APPLIED ")

    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 2 â€” Errori di tracking (Az/El)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna le label di errore Az/El e cambia colore se fuori soglia.
    ''' </summary>
    Public Sub AggiornaErroriTracking()
        If Not b_DatiAntenna Then Return

        ' LblAzErrEn/LblElErrEn mostrano l'incremento di offset calcolato
        ' da Calcolo_Aquire (Ricezione.vb): visibili solo quando la
        ' funzione Acquire è davvero attiva (stessa condizione con cui
        ' frmRicezione invoca Calcolo_Aquire), invisibili in ogni altro
        ' stato — richiesto esplicitamente dall'utente. Il controllo va
        ' prima del Return sotto, che esclude apposta questi stessi stati
        ' dal resto della funzione.
        Dim acquireAttivo As Boolean = (i_Status = OP_AUTOMATICAQUIRE OrElse i_Status = OP_SPIRALACQUIRE)
        If LblAzErrEn IsNot Nothing Then LblAzErrEn.Visible = acquireAttivo
        If LblElErrEn IsNot Nothing Then LblElErrEn.Visible = acquireAttivo

        ' AzCalc/ElCalc li mantiene aggiornati solo Calcolo_Coordinate,
        ' chiamato da CalcolaEInvia — che però lo esclude apposta in questi
        ' stessi stati (Standby, Spiral Acquire, Automatic Acquire,
        ' Proiezione: ciascuno calcola/comanda per conto proprio). Senza un
        ' valore comandato aggiornato, l'offset qui sotto finirebbe per
        ' confrontare l'Antenna con un valore vecchio o a zero, mostrando
        ' un falso allarme rosso (osservato per la prima volta in
        ' Proiezione, ma capita in ognuno di questi stati — es. tornando in
        ' Standby quando il satellite scende sotto l'orizzonte). Stesso
        ' elenco di esclusioni di CalcolaEInvia, per restare coerenti
        ' (incluso OP_MEMORYTRACK/OP_STEPTRACK, mancanti qui come in quella).
        If i_Status = OP_STANDBY OrElse i_Status = OP_SPIRALACQUIRE OrElse i_Status = OP_AUTOMATICAQUIRE OrElse i_Status = OP_PROIEZ OrElse i_Status = OP_COMMANDPOSITION OrElse i_Status = OP_MEMORYTRACK OrElse i_Status = OP_STEPTRACK Then Return

        ' Formula originale VB6 (frmRicezione.frm riga 6798):
        ' "lblAzError.Caption = Format$(TxtAz - AzCalc, "0.00")" — errore di
        ' puntamento reale (antenna) meno calcolato, non "AzCalc - AzComandato"
        ' (operandi e segno diversi, valore quindi errato nel port attuale).
        Dim errAz As Double = Val(TxtAz.Text) - AzCalc
        Dim errEl As Double = Val(TxtEl.Text) - ElCalc

        If LblAzError IsNot Nothing Then
            LblAzError.Text     = FormatCoord(errAz, "+00.000;-00.000")
            LblAzError.ForeColor = If(Math.Abs(errAz) > g_ThrAz, COL_FAIL, COL_OK)
        End If

        If LblElError IsNot Nothing Then
            LblElError.Text     = FormatCoord(errEl, "+00.000;-00.000")
            LblElError.ForeColor = If(Math.Abs(errEl) > g_ThrEl, COL_FAIL, COL_OK)
        End If

        ' Label enable/disable errori (etichette ausiliarie)
        If LblAzErrEn IsNot Nothing Then
            LblAzErrEn.BackColor = If(Math.Abs(errAz) > g_ThrAz, COL_FAIL, COL_OK)
        End If
        If LblElErrEn IsNot Nothing Then
            LblElErrEn.BackColor = If(Math.Abs(errEl) > g_ThrEl, COL_FAIL, COL_OK)
        End If

    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 2b — Allarmi prelimite/limite hardware e stow pin (Az/El)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna gli indicatori LblAllarme4/5/12/13 (Drive Status/stow pin
    ''' Az/El, gli unici presenti nel form reale FOC6), porting diretto da
    ''' VB6 frmRicezione.frm righe 6629-6669: vBytes(2)=Drive Status. Bit
    ''' acceso = allarme (rosso), spento = OK (verde).
    ''' </summary>
    Public Sub AggiornaAllarmi()
        If Not b_DatiAntenna Then Return

        If LblAllarme4 IsNot Nothing Then LblAllarme4.BackColor = If((vBytes(2) And BIT4) = BIT4, COL_FAIL, COL_OK)
        If LblAllarme5 IsNot Nothing Then LblAllarme5.BackColor = If((vBytes(2) And BIT3) = BIT3, COL_FAIL, COL_OK)
        If LblAllarme12 IsNot Nothing Then LblAllarme12.BackColor = If((vBytes(2) And BIT1) = BIT1, COL_FAIL, COL_OK)
        If LblAllarme13 IsNot Nothing Then LblAllarme13.BackColor = If((vBytes(2) And BIT2) = BIT2, COL_FAIL, COL_OK)
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 3 â€” Stato canali di comunicazione (CNL1..CNL4)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna il colore degli indicatori di stato per tutti i canali.
    ''' Canale 1=ACU, 2=MAC/MCS, 3=RX, 4=GRF/Down-Converter.
    ''' </summary>
    Public Sub AggiornaStatoCanali()

        ImpostaIndicatoreCanale(LblConfCNL1in, Port_Control(ACU).Port_State)
        ImpostaIndicatoreCanale(LblConfCNL2in, Port_Control(MAC).Port_State)
        ImpostaIndicatoreCanale(LblConfCNL3in, Port_Control(RX).Port_State)
        ImpostaIndicatoreCanale(LblConfCNL4in, Port_Control(GRF).Port_State)

    End Sub

    ''' <summary>
    ''' Mappa lo stato porta (PCS_*) al colore dell'indicatore label.
    ''' </summary>
    Private Sub ImpostaIndicatoreCanale(Lbl As Label, stato As Integer)
        If Lbl Is Nothing Then Return
        Select Case stato
            Case PCS_OK
                Lbl.BackColor = COL_OK
            Case PCS_FAIL
                Lbl.BackColor = COL_FAIL
            Case PCS_STOP
                Lbl.BackColor = COL_WARN   ' VB6: PCS_STOP_COL = vbYellow
            Case Else   ' PCS_INIT / non inizializzato
                Lbl.BackColor = COL_WARN
        End Select
    End Sub

    ''' <summary>
    ''' Aggiorna gli indicatori Memory/Param, Data, Sat.Data, Stow/Unstow
    ''' in base ai bit del byte di stato vBytesold(11) (C10) (vedi Gestione.vb).
    ''' Rosso = allarme attivo, Verde = ok.
    ''' </summary>
    Public Sub AggiornaAllarmiMemoria()
        LblMemParamInd.BackColor = If((vBytesold(11) And BIT1) = BIT1, COL_FAIL, COL_OK)
        LblMemDataInd.BackColor = If((vBytesold(11) And BIT2) = BIT2, COL_FAIL, COL_OK)
        LblSatDataInd.BackColor = If((vBytesold(11) And BIT4) = BIT4, COL_FAIL, COL_OK)
        LblStowUnstowInd.BackColor = If((vBytesold(11) And BIT3) = BIT3, COL_FAIL, COL_OK)
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 4 â€” Ricevitore (Beacon, Lock, Phase)
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna gli indicatori del ricevitore: Barra beacon, lock, fase.
    ''' </summary>
    Public Sub AggiornaRicevitore()

        ' Livello beacon sulla progress Bar — solo quando il canale RX è
        ' davvero collegato (BeaconLevel arriva dal protocollo RX reale,
        ' frmRicezione.Protocollo.vb "LEVEL"/"BEACON"): altrimenti il
        ' valore sul BarBeacon è quello di fallback già impostato da
        ' AggiornaBeaconLock() dai dati grezzi ACU.
        If BarBeacon IsNot Nothing AndAlso LblConfCNL3in IsNot Nothing AndAlso LblConfCNL3in.BackColor = COL_OK Then
            Dim lvl As Integer = CInt(Math.Max(0, Math.Min(100, BeaconLevel)))
            BarBeacon.Value = lvl
        End If

        ' Lock ricevitore: Text/BackColor ora gestiti da AggiornaBeaconLock()
        ' (bit reali ACU/RX, non più dal solo flag b_Lock). ForeColor resta
        ' qui perché AggiornaBeaconLock non lo tocca.
        If LblLock IsNot Nothing Then
            LblLock.ForeColor = If(b_Lock, Color.Black, Color.White)
        End If

        ' Polarizzazione: in C10 LblPol mostra RHCP/LHCP dallo stato ACU
        ' (AggiornaPolLnaDaACU) e non va sovrascritta con "POL:": il confronto
        ' prima/dopo segnalerebbe un cambio a ogni ciclo (g_NewLna).

        ' Errore di polarizzazione
        If LblPol1 IsNot Nothing AndAlso txtPolErr IsNot Nothing Then
            Dim polErr As Double = CDbl(Val(txtPolErr.Text))
            txtPolErr.ForeColor = If(Math.Abs(polErr) > 2.0, COL_FAIL, Color.Black)
        End If

    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 5 â€” Fase e Step-Track
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna l'indicatore di cambio fase (CmdPhase / LblPhase).
    ''' </summary>
    Public Sub AggiornaFase()

        If LblPhase Is Nothing Then Return

        If g_ChangePhase = "Y" Then
            LblPhase.BackColor = COL_OK
            If CmdPhase IsNot Nothing Then CmdPhase.Text = "N"
        Else
            LblPhase.BackColor = COL_WARN
            If CmdPhase IsNot Nothing Then CmdPhase.Text = "Y"
        End If

        ' Step-Track
        If LblStepTrack IsNot Nothing Then
            LblStepTrack.Text = If(b_StepTrack, "STEP", "---")
            LblStepTrack.BackColor = If(b_StepTrack, COL_OK, COL_IDLE)
        End If

    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 6 â€” Comando corrente
    ' -------------------------------------------------------

    ''' <summary>
    ''' Visualizza il comando operativo corrente nella label dedicata.
    ''' </summary>
    Public Sub AggiornaComando(testo As String, Optional colore As Color = Nothing)
        If LblCommand Is Nothing Then Return

        LblCommand.Text = testo

        If colore = Nothing OrElse colore = Color.Empty Then
            ' Dato di sola presentazione (modo comandato, VB6 senza colori per modo):
            ' 128,255,255 se noto, giallo se "---"
            LblCommand.BackColor = If(ColoriStandard.DatoNoto(testo), COL_DATO_NOTO, COL_DATO_IGNOTO)
        Else
            LblCommand.BackColor = colore
        End If
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 7 â€” Warning / Allarmi
    ' -------------------------------------------------------

    ''' <summary>
    ''' Mostra un messaggio di warning nella label dedicata.
    ''' Passare stringa vuota per cancellare il warning.
    ''' </summary>
    Public Sub MostraWarning(messaggio As String)
        If LblWarning Is Nothing Then Return

        If String.IsNullOrEmpty(messaggio) Then
            LblWarning.Text      = ""
            LblWarning.BackColor = COL_IDLE
            LblWarning.Visible   = False
            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = False
        Else
            LblWarning.Text      = messaggio
            LblWarning.BackColor = COL_WARN
            LblWarning.ForeColor = Color.Black
            LblWarning.Visible   = True
            ' Riavvia il timer di auto-cancellazione (VB6: tmrAlarm, one-shot
            ' da 2s) a ogni nuovo warning, invece di affidarsi a ciascun
            ' chiamante per riabilitarlo separatamente (alcuni lo facevano,
            ' altri no, lasciando il warning visibile indefinitamente).
            If tmrAlarm IsNot Nothing Then tmrAlarm.Enabled = True
        End If
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 8 â€” Blink indicatori
    ' -------------------------------------------------------

    ''' <summary>
    ''' Fa lampeggiare un indicatore label (chiamare periodicamente dal timer).
    ''' </summary>
    Public Sub BlinkIndicatore(Lbl As Label)
        If Lbl Is Nothing Then Return
        i_Blink = 1 - i_Blink   ' toggle 0/1
        Lbl.BackColor = If(i_Blink = 1, COL_BLINK_ON, COL_BLINK_OFF)
    End Sub

    ''' <summary>
    ''' Spegne il blink e porta l'indicatore al colore specificato.
    ''' </summary>
    Public Sub StopBlink(Lbl As Label, coloreFinale As Color)
        If Lbl Is Nothing Then Return
        i_Blink = 0
        Lbl.BackColor = coloreFinale
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 9 â€” Metodi helper generici
    ' -------------------------------------------------------

    ''' <summary>
    ''' Imposta il BackColor di un controllo cercandolo per nome.
    ''' GiÃ  dichiarato in frmRicezione.vb â€” ridefinito qui come overload
    ''' con parametro Color tipizzato.
    ''' </summary>
    Public Sub ImpostaColoreLabel(nomeLabel As String, colore As Color)
        Dim ctrl As Control = Me.Controls.Find(nomeLabel, True).FirstOrDefault()
        If ctrl IsNot Nothing Then ctrl.BackColor = colore
    End Sub

    ''' <summary>
    ''' Imposta il testo di un qualsiasi controllo cercandolo per nome.
    ''' </summary>
    Public Sub ImpostaTesto(nomeControllo As String, testo As String)
        Dim ctrl As Control = Me.Controls.Find(nomeControllo, True).FirstOrDefault()
        If ctrl IsNot Nothing Then ctrl.Text = testo
    End Sub

    ''' <summary>
    ''' Abilita o disabilita un pulsante cercandolo per nome.
    ''' </summary>
    Public Sub ImpostaAbilitazione(nomeControllo As String, abilitato As Boolean)
        Dim ctrl As Control = Me.Controls.Find(nomeControllo, True).FirstOrDefault()
        If ctrl IsNot Nothing Then ctrl.Enabled = abilitato
    End Sub

    ''' <summary>
    ''' Formatta una coordinata angolare con gestione eccezione.
    ''' </summary>
    Private Shared Function FormatCoord(valore As Double, formato As String) As String
        Try
            Return valore.ToString(formato, System.Globalization.CultureInfo.InvariantCulture)
        Catch
            Return "---"
        End Try
    End Function

    ' -------------------------------------------------------
    ' SEZIONE 10 â€” Aggiornamento satellite selezionato
    ' -------------------------------------------------------

    ''' <summary>
    ''' Aggiorna il colore dello sfondo del selettore satellite.
    ''' Verde = satellite valido, Giallo = in attesa TLE.
    ''' </summary>
    Public Sub AggiornaSatellite(valido As Boolean)
        If LblSatellite Is Nothing Then Return
        LblSatellite.BackColor = If(valido, COL_DATO_NOTO, COL_DATO_IGNOTO)
    End Sub

    ''' <summary>
    ''' Popola la combo satellite con la lista aggiornata.
    ''' </summary>
    Public Sub PopolaSatelliti(nomi As IEnumerable(Of String))
        If LblSatellite Is Nothing Then Return
        Dim combo As ComboBox = CType(LblSatellite, ComboBox)
        Dim selezionato As String = combo.Text
        combo.BeginUpdate()
        combo.Items.Clear()
        For Each nome As String In nomi
            combo.Items.Add(nome)
        Next
        ' Ripristina selezione precedente se ancora presente
        Dim idx As Integer = combo.FindStringExact(selezionato)
        If idx >= 0 Then combo.SelectedIndex = idx
        combo.EndUpdate()
    End Sub

    ' -------------------------------------------------------
    ' SEZIONE 11 â€” Reset visivo completo
    ' -------------------------------------------------------

    ''' <summary>
    ''' Riporta tutti gli indicatori allo stato iniziale (giallo = non inizializzato).
    ''' Chiamare al Form Load o a ogni nuova sessione.
    ''' </summary>
    Public Sub ResetVisualizzazione()
        b_DatiAntenna = False

        ' Canali in attesa
        For Each Lbl As Label In {LblConfCNL1in, LblConfCNL2in,
                                   LblConfCNL3in, LblConfCNL4in}
            If Lbl IsNot Nothing Then Lbl.BackColor = COL_WARN
        Next

        ' Satellite
        If LblSatellite IsNot Nothing Then LblSatellite.BackColor = COL_WARN

        ' Valori numerici
        If LblAzimuth IsNot Nothing Then LblAzimuth.Text = "---"
        If LblElevazione IsNot Nothing Then LblElevazione.Text = "---"
        If LblAzimuthC IsNot Nothing Then LblAzimuthC.Text = "---"
        If LblElevazioneC IsNot Nothing Then LblElevazioneC.Text = "---"
        If LblAzError IsNot Nothing Then
            LblAzError.Text = "---"
            LblAzError.ForeColor = SystemColors.ControlText
        End If
        If LblElError IsNot Nothing Then
            LblElError.Text = "---"
            LblElError.ForeColor = SystemColors.ControlText
        End If

        ' Lock
        If LblLock IsNot Nothing Then
            LblLock.Text = "NO LOCK"
            LblLock.BackColor = COL_IDLE
        End If

        ' Fase
        If LblPhase IsNot Nothing Then LblPhase.BackColor = COL_WARN

        ' Warning
        MostraWarning("")

        ' Comando
        AggiornaComando("---")
    End Sub

End Class


