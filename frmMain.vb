
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

' ============================================================
' frmMain.vb — convertito da Prog0347-FOCC10\frmMain.frm (VB6)
' Telespazio FOC-C10 — DYNATRON
' Base: il porting .NET di FOC6 (avvio non bloccante, SocketManager,
' simulazione ACU). Differenze di C10 applicate rispetto a FOC6:
'   - 6 canali: niente LNA/SCU (porte, socket, polling, spie)
'   - porta seriale in errore: spia rossa (PCS_FAIL_COL) e allarme
'     anche per PCU; allarme GRF "GRF Com Error"
'   - polling Down Converter: frequenza (CmdGRF "FE?") e attenuazioni
'     A/B (FrmDownConverter.CmdATA/CmdATB) invece di stato/tensioni
'   - polling periodico del polarizzatore (CmdPol, ogni i_InterPol s)
'   - richiesta di stato ACU solo se la precedente ha avuto risposta
'     (Not b_Ricezione(ACU)), come nel VB6 di C10
'   - controllo orario ACU con soglia 10 s e controllo della data;
'     con g_Irig = "A" (ora dall'orologio ACU, impostato dal parser)
'     riallinea l'ACU con CmdTime/CmdDate, altrimenti allarme
'   - con b_Parametri attivo il timeout ACU via LAN è 900 ms
' ============================================================

Partial Public Class FrmMain
    Inherits System.Windows.Forms.Form

    ' -------------------------------------------------------
    ' Variabili locali (erano Dim a livello modulo in VB6)
    ' -------------------------------------------------------
    Private Cont2 As Integer
    Private Contv As Integer
    ' Throttle del controllo spazio libero su disco (una volta al giorno),
    ' vedi TmrOrologio_Tick.
    Private ultimoControlloDisco As DateTime = DateTime.MinValue
    Private OraAtt As Date
    Private Cont3 As Integer
    Private Cont4 As Integer        ' C10: contatore allarme data ACU diversa
    Private Contfreq As Integer
    ' Nome dell'ultimo satellite per cui è stata inizializzata la
    ' propagazione orbitale usata dal messaggio ACU simulato (vedi
    ' TmrOrologio_Tick) — evita di ricalcolare ideep/xnodp ad ogni tick se
    ' il satellite non è cambiato.
    Private s_SatelliteAcuSimulato As String = ""
    ' Riusato ad ogni tick del messaggio ACU simulato — un nuovo Random()
    ' creato ogni volta, a distanza di millisecondi, rischia di essere
    ' seminato con lo stesso valore e produrre numeri identici.
    Private ReadOnly rndSimAcu As New Random()

    ' -------------------------------------------------------
    ' API Windows (erano in WindowsAPI.bas)
    ' -------------------------------------------------------
    <DllImport("user32.dll")>
    Private Shared Function GetSystemMenu(ByVal hWnd As IntPtr, ByVal bRevert As Boolean) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function DeleteMenu(ByVal hMenu As IntPtr, ByVal uPosition As Integer, ByVal uFlags As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    Private Const SC_CLOSE As Integer = &HF060
    Private Const MF_BYCOMMAND As Integer = &H0
    Private Const WM_NCACTIVATE As Integer = &H86

    ' -------------------------------------------------------
    ' Costruttore — imposta MDI container
    ' -------------------------------------------------------
    Public Sub New()
        InitializeComponent()
    End Sub

    ' -------------------------------------------------------
    ' Form Load (era MDIForm_Load)
    ' -------------------------------------------------------
    ''' <summary>
    ''' stpBeep/stpDisco (barra di stato) avevano un'icona nel VB6
    ''' originale (StatusBar.Panels("beep")/Panels("disco"), Picture
    ''' impostata da frmMain.frx) mai portata in .NET — i pannelli
    ''' ToolStripStatusLabel restavano solo testo. Icone originali
    ''' recuperate dal file .frx (formato "lt"/icona VB6: GUID StdPicture
    ''' 16 byte + tag "lt" 2 byte + riservato 2 byte + lunghezza 4 byte +
    ''' dati icona) e incorporate qui in Base64 per non dover distribuire
    ''' file aggiuntivi.
    ''' </summary>
    Private Sub CaricaIconeStatusBar()
        Try
            Dim beepB64 As String = "AAABAAIAICACAAAAAAAwAQAAJgAAACAgEAAAAAAA6AIAAFYBAAAoAAAAIAAAAEAAAAABAAEAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP///wAAAAAAAAIAAAAKAAAAFwAAADcAAAB3AAAA7wAAAe+AAAPvgAAf44AAP+2AAD/tAAA/7YAAP+2AAD/tgAAf44AAA++AAAHvgAAA7wAAAHcAAAA3AAAAFwAAAAoAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP/5////8P///+D////Af///gH///wB///4Af9/8ADff4AA378AAO++AADvvgAA764AAK66AADrvgAA778AAO+/gADfv/AA33/4Af9//AH///4B////Af///4P////D////5////////////////////////////////////////KAAAACAAAABAAAAAAQAEAAAAAACAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL8AAL8AAAC/vwC/AAAAvwC/AL+/AADAwMAAgICAAAAA/wAA/wAAAP//AP8AAAD/AP8A//8AAP///wAAAAAAAAAACAAAAAAAAAAAAAAAAAAAAHAAAAAAAAAAAAAAAAAAAIBwgAAAAAAAAAAAAAAAAAcIdwAAAAAAAAAAAAAAAAB3B3cAAAAAAAAAAAAAAAAHeAd3AAAAAAAAAAAAAAAAd3CHd4gAAAAAAAAAAAAAB3ewh3dwAAAAAAAAAACAAHd7cId3cAAAAAAAAAAIB3e3t7AAd3AAAAAAAAAAAHt7e3twiAfwAAAAAAAAAAC3t7e3sIgPyAAAAAAADAAAu7u7u7B4B/gMAAAMAAAMAL+/v7+w+AdwAAAMAAAAAAD7+/v78PgHcAAAAAAAAAAID7+/v7AAd3AAAAAAAAAAAIAA+/vwh3dwAAAAAAAAAAAAAA+/sId3cAAAAAAAAAAAAAAA+/CHd4gAAAAAAAAAAAAAAA+3B3cAAAAAAAAAAAAAAAAA+wd3AAAAAAAAAAAAAAAAAA8IdwAAAAAAAAAAAAAAAAAAcHCAAAAAAAAAAAAAAAAAAABwAAAAAAAAAAAAAAAAAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP/4////8P///+B////Af///gH///wB///4AP9/8ADffwAA374AAO++AADvvgAA764AAK66AADrvgAA774AAO+/AADfv/AA33/4AP9//AH///4B////Af///4H////D////4////////////////////////////////////////"
            Dim discoB64 As String = "AAABAAEAEBAAAAAAAABoBQAAFgAAACgAAAAQAAAAIAAAAAEACAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAAAAgIAAgAAAAIAAgACAgAAAwMDAAMDcwADwyqYABAQEAAgICAAMDAwAERERABYWFgAcHBwAIiIiACkpKQBVVVUATU1NAEJCQgA5OTkAgHz/AFBQ/wCTANYA/+zMAMbW7wDW5+cAkKmtAAAAMwAAAGYAAACZAAAAzAAAMwAAADMzAAAzZgAAM5kAADPMAAAz/wAAZgAAAGYzAABmZgAAZpkAAGbMAABm/wAAmQAAAJkzAACZZgAAmZkAAJnMAACZ/wAAzAAAAMwzAADMZgAAzJkAAMzMAADM/wAA/2YAAP+ZAAD/zAAzAAAAMwAzADMAZgAzAJkAMwDMADMA/wAzMwAAMzMzADMzZgAzM5kAMzPMADMz/wAzZgAAM2YzADNmZgAzZpkAM2bMADNm/wAzmQAAM5kzADOZZgAzmZkAM5nMADOZ/wAzzAAAM8wzADPMZgAzzJkAM8zMADPM/wAz/zMAM/9mADP/mQAz/8wAM///AGYAAABmADMAZgBmAGYAmQBmAMwAZgD/AGYzAABmMzMAZjNmAGYzmQBmM8wAZjP/AGZmAABmZjMAZmZmAGZmmQBmZswAZpkAAGaZMwBmmWYAZpmZAGaZzABmmf8AZswAAGbMMwBmzJkAZszMAGbM/wBm/wAAZv8zAGb/mQBm/8wAzAD/AP8AzACZmQAAmTOZAJkAmQCZAMwAmQAAAJkzMwCZAGYAmTPMAJkA/wCZZgAAmWYzAJkzZgCZZpkAmWbMAJkz/wCZmTMAmZlmAJmZmQCZmcwAmZn/AJnMAACZzDMAZsxmAJnMmQCZzMwAmcz/AJn/AACZ/zMAmcxmAJn/mQCZ/8wAmf//AMwAAACZADMAzABmAMwAmQDMAMwAmTMAAMwzMwDMM2YAzDOZAMwzzADMM/8AzGYAAMxmMwCZZmYAzGaZAMxmzACZZv8AzJkAAMyZMwDMmWYAzJmZAMyZzADMmf8AzMwAAMzMMwDMzGYAzMyZAMzMzADMzP8AzP8AAMz/MwCZ/2YAzP+ZAMz/zADM//8AzAAzAP8AZgD/AJkAzDMAAP8zMwD/M2YA/zOZAP8zzAD/M/8A/2YAAP9mMwDMZmYA/2aZAP9mzADMZv8A/5kAAP+ZMwD/mWYA/5mZAP+ZzAD/mf8A/8wAAP/MMwD/zGYA/8yZAP/MzAD/zP8A//8zAMz/ZgD//5kA///MAGZm/wBm/2YAZv//AP9mZgD/Zv8A//9mACEApQBfX18Ad3d3AIaGhgCWlpYAy8vLALKysgDX19cA3d3dAOPj4wDq6uoA8fHxAPj4+ADw+/8ApKCgAICAgAAAAP8AAP8AAAD//wD/AAAA/wD/AP//AAD///8ACgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCuvr6+vr6+vr6+vr6+vrCgrr8P/////////////v7esK6/Hr6+vr6+vr6+vr7+3rCuvx7wcHBwcH7+/v7+/t6wrr8AcHBwcHBwfvMwLv7esK6////////////////+3rCgrr7+/v7+/v7+/v7+/v6woKCuvr6+vr6+vr6+vr6+sKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCv//mQD//8wA////AIADAAAAATMAAACZAAAAzAAAAP8AAADMAAAAAACAAJkAwAGZAP//zAD//wAA//8zAP//ZgA="
            Dim dataB64 As String = "AAABAAEAICAQAAAAAADoAgAAFgAAACgAAAAgAAAAQAAAAAEABAAAAAAAgAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAAAAgIAAgAAAAIAAgACAgAAAgICAAMDAwAAAAP8AAP8AAAD//wD/AAAA/wD/AP//AAD///8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACIh3iAAAAAAAAAAAAAAAiIiPB4iIgAAAAAAAAAAAiHeIj/iIiHeAAAAAAAAACI8HiIiIiI8HiAAAAAAAAIiP+IiIiIiP+IiAAAAAAAiIiIiIiIiIiIiIiAAAAACIiIiIiIiIiIiIiIiAAAAAh3iJiIiIiIiIiIiHcAAACPB4iJiIiIiIiIiI8HgAAAj/iIiJiIiIiIiIiP+IAAAIiIiIiJiIiIiIiIiIiAAAiIiIiIiJiIiIiIiIiIiAAIiIiIiIiJiIiIiIiIiIgACHeIiIiIiJiIiIiIiIh3AA8HiIiIiIiAAAAAAIiPBwAP+IiIiIiIgIiIiIiIj/gACIiIiIiIiICIiIiIiIiIAAiIiIiIiIiAiIiIiIiIiAAAiIiIiIiIgIiIiIiIiIAAAIh3iIiIiICIiIiIiHeAAACPB4iIiIiAiIiIiI8HgAAAD/iIiIiIgIiIiIiP+AAAAAiIiIiIiICIiIiIiIgAAAAAiIiIiIiAiIiIiIiAAAAAAAiIh3iIgIiIh3iIAAAAAAAAiPB4iICIiPB4gAAAAAAAAAj/iIiHeIj/iAAAAAAAAAAACIiI8HiIiAAAAAAAAAAAAAAAiP+IgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD///////Af//+AA//+AAD//AAAf/gAAD/wAAAf4AAAD8AAAAfAAAAHgAAAA4AAAAOAAAADAAAAAQAAAAEAAAABAAAAAQAAAAEAAAABAAAAAYAAAAOAAAADgAAAA8AAAAfAAAAH4AAAD/AAAB/4AAA//AAAf/4AAP//gAP///Af/w=="
            Using msBeep As New IO.MemoryStream(Convert.FromBase64String(beepB64))
                stpBeep.Image = New Icon(msBeep).ToBitmap()
            End Using
            Using msDisco As New IO.MemoryStream(Convert.FromBase64String(discoB64))
                stpDisco.Image = New Icon(msDisco).ToBitmap()
            End Using
            Using msData As New IO.MemoryStream(Convert.FromBase64String(dataB64))
                stpData.Image = New Icon(msData).ToBitmap()
            End Using
        Catch ex As Exception
            WriteError(0, "CaricaIconeStatusBar: " & ex.Message)
        End Try
    End Sub

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MainForm = Me
        ' C10: ACU (e PCU/INTELSAT) su seriale — InvioACU della libreria
        ' comune scrive sulla porta tramite questo aggancio.
        InvioSerialeHook = AddressOf ScriviSeriale
        ' Bordo fisso in rilievo su tutti i pulsanti di comando, in tutti i form.
        BordoPulsanti.Attiva()
        ' Simulazione ACU: esiste solo nella configurazione di build
        ' "Simulazione" (sviluppo senza hardware). Nella versione operativa
        ' (Debug/Release) il codice non viene compilato e il flag resta False,
        ' così anche il beacon simulato in Acquire (libreria comune) è spento.
#If SIMULAZIONE Then
        b_SimulazioneACU = True
#Else
        b_SimulazioneACU = False
#End If
        ' STARTUP_TIMING: stessa strumentazione temporanea già usata su
        ' BTS1 per trovare il vero collo di bottiglia dell'avvio (da
        ' rimuovere una volta trovato) — qui non c'era mai stata, quindi
        ' Inizializza/AggNorad/apertura DB restavano invisibili nei log.
        Dim swStartup As New System.Diagnostics.Stopwatch()
        swStartup.Start()
        Dim lastCheckpoint As Long = 0
        Dim Checkpoint As Action(Of String) =
            Sub(nome As String)
                Dim ora As Long = swStartup.ElapsedMilliseconds
                WriteError(0, "STARTUP_TIMING " & nome & ": " & (ora - lastCheckpoint) & " ms (totale " & ora & " ms)")
                lastCheckpoint = ora
            End Sub
        Try
            ' Rimuove il pulsante di chiusura dalla titleBar
            Dim hMenu As IntPtr = GetSystemMenu(Me.Handle, False)
            DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND)
            SendMessage(Me.Handle, WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero)
            SendMessage(Me.Handle, WM_NCACTIVATE, New IntPtr(1), IntPtr.Zero)

            CaricaIconeStatusBar()

            SceltaFont = "Arial"
            g_Server = LeggiFileIni("Server", "directory")
            g_Log = False
            b_StatusPC1 = True

            ' Database: Orbite.db (SQLite) invece di Orbite.mdb (DAO) —
            ' stessa migrazione già fatta per BTS1, qui si riusa
            ' direttamente il file Orbite.db copiato da BTS1 (già con lo
            ' schema corretto, nessuna FixSat2line* necessaria qui).
            g_Database = Path.Combine(Application.StartupPath, "Orbite.db")
            Dim _sqliteConn As New System.Data.SQLite.SQLiteConnection("Data Source=" & g_Database & ";Version=3;")
            _sqliteConn.Open()
            dbOrbite = New DAODatabase(_sqliteConn)
            Checkpoint("Apertura DB")

            ' Reset tabella Fasi (era "UPDATE [Fasi] set Ultima = vbUnchecked")
            Dim CmdSQL As New System.Data.SQLite.SQLiteCommand("UPDATE Fasi SET Ultima = 0", _sqliteConn)
            CmdSQL.ExecuteNonQuery()
            Checkpoint("Reset tabella Fasi")

            NoPhasing = False
            b_Sat = False
            b_Checked = False

            Inizializza()
            Checkpoint("Inizializza")

            ' Aggiorna i TLE (righe spuntate in Aggiorna nella tabella URL) già
            ' all'avvio del programma, non solo al cambio di giorno (vedi
            ' TmrOrologio_Tick) o all'uscita dal dialogo URL: altrimenti
            ' spuntare le caselle non ha alcun effetto visibile finché non si
            ' riapre e richiude quel dialogo. Stesso punto di BTS1.
            FrmNorad.AggNorad()
            Checkpoint("AggNorad")

            OraAgg = If(s_Ora = "G", DateTime.UtcNow, DateTime.Now)
            OraAtt = OraAgg
            PTK = 0
            i_Status = 0
            DeltaTime = 0
            i_AsseXGrafico = 10
            SatChange = True

            ' Apre il form di ricezione come finestra figlia MDI
            Checkpoint("Prima di FrmRicezione.Show")
            FrmRicezione.MdiParent = Me
            FrmRicezione.WindowState = FormWindowState.Maximized
            frmRicezioneInstance = FrmRicezione
            FrmRicezione.Show()
            Checkpoint("FrmRicezione.Show")
        Catch ex As Exception
            stpMessaggio.Text = "An error occurred during the initialization - Check the correct configuration"
            TmrMsg.Enabled = True
            WriteError(0, "frmMain - Main " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Timer MSG — cancella messaggio statusBar dopo timeout
    ' -------------------------------------------------------
    Private Sub TmrMSG_Tick(sender As Object, e As EventArgs) Handles TmrMsg.Tick
        stpMessaggio.Text = ""
        TmrMsg.Enabled = False
    End Sub

    ' -------------------------------------------------------
    ' Timer Beep — segnale acustico di allarme (era: solo se
    ' mnuFile(2).Checked, cioè la voce di menu "Beep" abilitata)
    ' -------------------------------------------------------
    Private Sub TmrBeep_Tick(sender As Object, e As EventArgs) Handles TmrBeep.Tick
        If frmRicezioneInstance IsNot Nothing AndAlso frmRicezioneInstance.BeepAbilitato() Then
            ' Console.Beep è sincrono e blocca il thread che lo chiama per
            ' tutta la durata richiesta (qui 200ms) — chiamato ogni 500ms sul
            ' thread UI, e in ambienti remoti/virtualizzati (es. noVNC, senza
            ' altoparlante fisico) il driver audio può farlo durare molto più
            ' dei 200ms nominali, anche secondi. Spostato su un thread
            ' separato così non blocca mai il thread UI.
            Task.Run(Sub() Console.Beep(4000, 200))
        End If
        TmrBeep.Enabled = True
    End Sub

    ' -------------------------------------------------------
    ' Timer Phase — gestione fasi ricevitore
    ' -------------------------------------------------------
    Public Sub TmrPhase_Tick(sender As Object, e As EventArgs) Handles TmrPhase.Tick
        Try
            TmrPhase.Enabled = True
            If i_ric = 3 Then
                NoPhasing = False
                Exit Sub
            End If
            If i_ric = 0 And Not Interroga Then i_ric = 1
            If i_ric = 0 Then
                If Interroga Then MonComponent(ComponentCodeIF1)
                Interroga = False
            ElseIf i_ric = 1 Then
                MonComponent(ComponentCode)
            ElseIf i_ric = 2 Then
                If Not NoPhasing Then
                    Phasing()
                End If
            End If
            i_ric += 1
            If i_ric > 2 Then i_ric = 0
        Catch ex As Exception
            WriteError(0, "TmrPhase " & i_ric & " " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Form Resize — usa FormResizer (BTS1) invece dello scaling
    ' manuale per-controllo del VB6 originale (MDIForm_Resize).
    ' -------------------------------------------------------
    Private Sub FrmMain_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        FormResizer.Init(Me, New Size(1431, 557))
        FormResizer.Resize(Me)
    End Sub

    Private Sub FrmMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        FormResizer.Resize(Me)
    End Sub

    ' -------------------------------------------------------
    ' Inizializzazione porte seriali e socket (8 canali)
    ' -------------------------------------------------------
    Public Sub IniPort_Control()
        Try
            CollegaRicezioneSocket()
            ' Le connessioni LAN (ConnectAsync.Wait(2000) in SocketManager, ramo
            ' Listen="S" = canale che si connette come client) venivano
            ' tentate in sequenza, un canale alla volta: con più canali a
            ' socket e nessun dispositivo raggiungibile, l'attesa totale era
            ' la SOMMA dei singoli timeout (stesso collo di bottiglia trovato
            ' e corretto su BTS1, osservato nei log STARTUP_TIMING). Qui il
            ' lavoro di rete (non l'aggiornamento della UI, mai thread-safe:
            ' vedi TentaConnessioneLan) viene lanciato in parallelo su più
            ' canali: l'attesa totale diventa il MASSIMO dei singoli timeout
            ' (~2s) invece della somma.
            Dim connessioni As New Dictionary(Of Integer, Task(Of Boolean))
            For Ind As Integer = 1 To NumCom
                If Port_Control(Ind).Socket = "N" Then
                    PortReset(Ind)
                ElseIf Port_Control(Ind).Listen = "S" AndAlso (Ind = ACU OrElse Ind = MAC OrElse Ind = GRF) Then
                    Dim idx As Integer = Ind
                    connessioni(idx) = Task.Run(Function() TentaConnessioneLan(idx))
                Else
                    LanReset(Ind)
                End If
            Next
            ' I sotto-canali RX LanMon/LanPhase (vedi LanReset1) hanno lo
            ' stesso identico collo di bottiglia dei canali sopra — altri due
            ' ConnectAsync.Wait(2000) in sequenza, invisibili finché non si
            ' guarda qui: entrano nello stesso batch parallelo.
            Dim taskLanMon As Task(Of Boolean) = Nothing
            Dim taskLanPhase As Task(Of Boolean) = Nothing
            If LanRx(LanMon).Listen = "S" Then taskLanMon = Task.Run(Function() TentaConnessioneLanRX(LanMon))
            If LanRx(LanPhase).Listen = "S" Then taskLanPhase = Task.Run(Function() TentaConnessioneLanRX(LanPhase))
            Dim tutteLeConnessioni As New List(Of Task)(connessioni.Values)
            If taskLanMon IsNot Nothing Then tutteLeConnessioni.Add(taskLanMon)
            If taskLanPhase IsNot Nothing Then tutteLeConnessioni.Add(taskLanPhase)
            ' Esiti applicati sul thread UI (LanReset/LanReset1 aggiornano la
            ' UI, non sono thread-safe).
            Dim applicaEsiti As Action =
                Sub()
                    For Each kv In connessioni
                        LanReset(kv.Key, kv.Value.Result)
                        tentativoConnessioneLanInCorso(kv.Key) = False
                    Next
                    LanReset1(LanMon, If(taskLanMon IsNot Nothing, CType(taskLanMon.Result, Boolean?), Nothing))
                    LanReset1(LanPhase, If(taskLanPhase IsNot Nothing, CType(taskLanPhase.Result, Boolean?), Nothing))
                End Sub
            If tutteLeConnessioni.Count = 0 Then
                applicaEsiti()
            Else
                ' Prima Task.WaitAll: il thread UI restava fermo fino al
                ' timeout di connessione (2 s per canale non raggiungibile,
                ' ~3,6 s misurati) prima di mostrare la finestra. Ora la
                ' finestra compare subito e gli esiti si applicano appena
                ' pronti - stesso schema di LanResetAsync, già usato a
                ' runtime ad ogni timeout di canale. Finché il tentativo
                ' iniziale è in volo, LanResetAsync non ne lancia un secondo
                ' sullo stesso canale.
                For Each kv In connessioni
                    tentativoConnessioneLanInCorso(kv.Key) = True
                Next
                Task.WhenAll(tutteLeConnessioni).ContinueWith(
                    Sub(t)
                        Try
                            Me.BeginInvoke(applicaEsiti)
                        Catch ex As Exception
                            WriteError(0, "frmMain - IniPortControl esiti " & ex.Message)
                        End Try
                    End Sub)
            End If
        Catch ex As Exception
            WriteError(0, "frmMain - IniPortControl " & ex.Message)
        End Try
        ' Server HTTP di accesso remoto: non ancora portato per FOC6
        ' (AntennaHttpServer è specifico di BTS1) — da aggiungere qui
        ' quando si implementerà il richiamo remoto anche per FOC6.
    End Sub

    ' -------------------------------------------------------
    ' Collega gli eventi di SocketManager alla pipeline di analisi
    ' protocollo reale (stesso aggancio di BTS1 — vedi commento lì
    ' su OnMainReceived/OnMacReceived/OnRXReceived/OnDCReceived).
    ' -------------------------------------------------------
    Private Sub CollegaRicezioneSocket()
        SocketManager.OnMainReceived = AddressOf GestisciDatiMain
        SocketManager.OnMacReceived = AddressOf GestisciDatiMac
        SocketManager.OnRXReceived = AddressOf GestisciDatiRX
        SocketManager.OnDCReceived = AddressOf GestisciDatiDC
        If SocketManager.OnMainReceived Is Nothing OrElse SocketManager.OnMacReceived Is Nothing OrElse
           SocketManager.OnRXReceived Is Nothing OrElse SocketManager.OnDCReceived Is Nothing Then
            WriteError(0, "CollegaRicezioneSocket: uno o più callback di ricezione non risultano agganciati")
        End If
    End Sub

    Private Sub GestisciDatiMain(ind As Integer, data As String)
        Try
            frmRicezioneInstance.Invoke(Sub()
                                            Port_Control(ind).Buffer &= data
                                            frmRicezioneInstance.AnalisiOrdini(ind, Port_Control(ind).Buffer)
                                        End Sub)
        Catch ex As Exception
            WriteError(0, "GestisciDatiMain: " & ex.Message)
        End Try
    End Sub

    Private Sub GestisciDatiMac(ind As Integer, data As String)
        Try
            frmRicezioneInstance.Invoke(Sub()
                                            Port_Control(ind).Buffer &= data
                                            frmRicezioneInstance.AnalisiOrdini(ind, Port_Control(ind).Buffer)
                                        End Sub)
        Catch ex As Exception
            WriteError(0, "GestisciDatiMac: " & ex.Message)
        End Try
    End Sub

    Private Sub GestisciDatiDC(ind As Integer, data As String)
        Try
            frmRicezioneInstance.Invoke(Sub()
                                            Port_Control(ind).Buffer &= data
                                            frmRicezioneInstance.AnalisiOrdini(ind, Port_Control(ind).Buffer)
                                        End Sub)
        Catch ex As Exception
            WriteError(0, "GestisciDatiDC: " & ex.Message)
        End Try
    End Sub

    Private Sub GestisciDatiRX(ind As Integer, data As String)
        Try
            frmRicezioneInstance.Invoke(Sub()
                                            PortaRx = ind
                                            InputBuffer3 &= data
                                            FrmRicevitore.LblInputRx.Text = InputBuffer3
                                            FrmRicevitore.LblInputRx_Change()
                                        End Sub)
        Catch ex As Exception
            WriteError(0, "GestisciDatiRX: " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Reset porta seriale (era MSCOMM32 → ora SerialPort)
    ' -------------------------------------------------------
    Public Sub PortReset(Ind As Integer)
        Dim Port As Integer = Canali(Ind).Port_IN
        If Port = 0 Then Exit Sub

        If ComPort(Ind) IsNot Nothing AndAlso ComPort(Ind).IsOpen Then
            ComPort(Ind).Close()
        End If

        If ComPort(Ind) Is Nothing Then
            ComPort(Ind) = New IO.Ports.SerialPort()
        End If
        ComPort(Ind).PortName = "COM" & Canali(Ind).Port_IN
        Dim settings() As String = Port_Control(Ind).Port_settings.Split(","c)
        If settings.Length = 4 Then
            ComPort(Ind).BaudRate = CInt(settings(0))
            ComPort(Ind).Parity = FrmRicezione.ParseParity(settings(1))
            ComPort(Ind).DataBits = CInt(settings(2))
            ComPort(Ind).StopBits = FrmRicezione.ParseStopBits(settings(3))
        End If
        ComPort(Ind).Encoding = System.Text.Encoding.GetEncoding(28591)
        Port_Control(Ind).Buffer = ""

        RemoveHandler ComPort(Ind).DataReceived, AddressOf frmRicezioneInstance.SerialDataReceived
        AddHandler ComPort(Ind).DataReceived, AddressOf frmRicezioneInstance.SerialDataReceived

        Try
            ComPort(Ind).Open()
            Port_Control(Ind).Port_State = PCS_NORMAL
            If Ind = ACU Then
                Port_Control(Ind).Rec_State = PCR_STANDBY
            Else
                Port_Control(Ind).Rec_State = PCR_WAIT
            End If
        Catch ex As Exception
            Port_Control(Ind).Port_State = PCS_FAIL
            ' C10: spia rossa (VB6 PCS_FAIL_COL = vbRed), non il colore di stop di FOC6.
            Select Case Ind
                Case CNL_1_IN
                    frmRicezioneInstance.SetLabelColor("LblConfCNL1in", Color.Red)
                Case CNL_2_IN
                    frmRicezioneInstance.SetLabelColor("LblConfCNL2in", Color.Red)
                Case RX
                    frmRicezioneInstance.SetLabelColor("LblConfCNL3in", Color.Red)
                Case GRF
                    frmRicezioneInstance.SetLabelColor("LblConfCNL4in", Color.Red)
                    ScriviAlarm("GRF Com Error", 1)
                Case INTELSAT
                    frmRicezioneInstance.SetLabelColor("LblConfCNL5in", Color.Red)
                Case PCU
                    frmRicezioneInstance.SetLabelColor("LblConfCNL6in", Color.Red)
                    ScriviAlarm("PCU Com Error", 1)
            End Select
        End Try

        If Ind = MAC Then nSock = MAC
    End Sub

    ' -------------------------------------------------------
    ' Reset connessione LAN/Socket (era MSWINSCK → TcpClient)
    ' ACU/MAC/RX/GRF riusano SocketManager come in BTS1. I canali
    ' 5-8 (INTELSAT/PCU/LNA/SCU) sono specifici di FOC6 e non hanno
    ' ancora un'implementazione di trasporto reale: PCU in VB6 è
    ' comunque dormiente (SNMP via frmManager, righe commentate),
    ' per gli altri serve definire il protocollo prima di collegare
    ' un vero socket — segnalato e saltato, non inventato.
    ' -------------------------------------------------------
    ''' <summary>
    ''' Tenta la connessione TCP (client) per un canale LAN ACU/MAC/GRF,
    ''' senza toccare alcun controllo della UI: estratta da LanReset per
    ''' poterla lanciare in Task.Run in parallelo su più canali durante
    ''' l'avvio (vedi IniPort_Control) — SetLabelColor/SetTimeoutInterval in
    ''' LanReset non sono thread-safe, qui restano fuori apposta.
    ''' </summary>
    Private Function TentaConnessioneLan(Ind As Integer) As Boolean
        Select Case Ind
            Case ACU
                SocketManager.Connect(Ind, Port_Control(Ind).IPAdress, Port_Control(Ind).LanPort)
                Threading.Thread.Sleep(200)
                Return SocketManager.IsConnected(Ind)
            Case MAC
                SocketManager.ConnectMac(Ind, Port_Control(Ind).IPAdress, Port_Control(Ind).LanPort)
                Return SocketManager.IsMacConnected(Ind)
            Case GRF
                SocketManager.ConnectDC(Ind, Port_Control(Ind).IPAdress, Port_Control(Ind).LanPort)
                Threading.Thread.Sleep(200)
                Return SocketManager.IsDCConnected(Ind)
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>
    ''' connessoPrecalcolato: se valorizzato (da IniPort_Control, che lancia
    ''' TentaConnessioneLan in parallelo su più canali prima di chiamare
    ''' LanReset per ciascuno), salta un secondo tentativo di connessione e
    ''' usa direttamente l'esito già noto. Nothing (default) per tutte le
    ''' altre chiamate — comportamento identico a prima, connette qui.
    ''' </summary>
    ''' <summary>
    ''' Come ConnettiRXSenzaBloccare (Gestione.vb) ma per LanReset: chiamata
    ''' da TmrTimeoutMSG(ACU/MAC/GRF)_Timer (frmRicezione.vb) e da
    ''' AnalisiMAC (frmRicezione.Protocollo.vb) ogni volta che un canale va
    ''' in timeout — cosa che, senza hardware reale collegato, succede in
    ''' continuazione. LanReset(Ind), senza connessoPrecalcolato, faceva
    ''' ripartire da zero TentaConnessioneLan (SocketManager.Connect/
    ''' ConnectMac/ConnectDC, ".Wait(2000)" interno) DIRETTAMENTE sul
    ''' thread UI: un blocco fino a 2s ad ogni timeout di canale,
    ''' ripetuto periodicamente, con qualunque click o comando dell'
    ''' operatore in coda dietro finché non finiva — il ritardo "poco
    ''' sotto il secondo e oltre" osservato su clic di EXE/Elabora/altri
    ''' comandi, casuale a seconda di quando il click cade rispetto al
    ''' prossimo timeout. Stesso tentativo di connessione, ma su un
    ''' thread separato: il chiamante prosegue subito, il risultato
    ''' arriva a LanReset (via Invoke, per restare sul thread UI per
    ''' SetLabelColor) quando è pronto.
    ''' </summary>
    ' Un tentativo di connessione per canale già in corso (stesso motivo
    ' di tentativoConnessioneRXInCorso in Gestione.vb): senza questa
    ' guardia, un canale che va ripetutamente in timeout (Time_out basso)
    ' potrebbe accumulare più thread in volo contemporaneamente invece di
    ' aspettare l'esito del precedente.
    Private ReadOnly tentativoConnessioneLanInCorso As Boolean() = New Boolean(8) {}

    Public Sub LanResetAsync(Ind As Integer)
        If Port_Control(Ind).Listen = "S" AndAlso (Ind = ACU OrElse Ind = MAC OrElse Ind = GRF) Then
            If tentativoConnessioneLanInCorso(Ind) Then Return
            tentativoConnessioneLanInCorso(Ind) = True
            Dim t As New Threading.Thread(Sub()
                                               Dim connesso As Boolean = TentaConnessioneLan(Ind)
                                               Try
                                                   Me.Invoke(Sub() LanReset(Ind, connesso))
                                               Catch
                                               Finally
                                                   tentativoConnessioneLanInCorso(Ind) = False
                                               End Try
                                           End Sub)
            t.IsBackground = True
            t.Start()
        Else
            LanReset(Ind)
        End If
    End Sub

    Public Sub LanReset(Ind As Integer, Optional connessoPrecalcolato As Boolean? = Nothing)
        Try
            If Ind = 3 Then Exit Sub

            Select Case Ind
                Case ACU
                    If Port_Control(Ind).Listen = "S" Then
                        Dim connesso As Boolean = If(connessoPrecalcolato.HasValue, connessoPrecalcolato.Value, TentaConnessioneLan(Ind))
                        If Not connesso Then
                            Port_Control(Ind).Port_State = PCS_FAIL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL1in", PCS_STOP_COL)
                            ScriviAlarm("ACU Error", 1)
                        Else
                            Port_Control(Ind).Port_State = PCS_NORMAL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL1in", PCS_NORMAL_COL)
                        End If
                    Else
                        SocketManager.Listen(0, Port_Control(0).LanPort)
                    End If
                    Port_Control(Ind).Buffer = ""
                    Port_Control(Ind).Rec_State = PCR_WAIT

                Case MAC
                    If Port_Control(Ind).Listen = "S" Then
                        Dim connesso As Boolean = If(connessoPrecalcolato.HasValue, connessoPrecalcolato.Value, TentaConnessioneLan(Ind))
                        If Not connesso Then
                            Port_Control(Ind).Port_State = PCS_FAIL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL2in", PCS_STOP_COL)
                            ScriviAlarm("MCS Error", 1)
                        Else
                            Port_Control(Ind).Port_State = PCS_NORMAL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL2in", PCS_NORMAL_COL)
                        End If
                    Else
                        SocketManager.ListenMac(0, Port_Control(Ind).LanPort)
                    End If

                Case GRF
                    If Port_Control(Ind).Listen = "S" Then
                        Dim connesso As Boolean = If(connessoPrecalcolato.HasValue, connessoPrecalcolato.Value, TentaConnessioneLan(Ind))
                        If Not connesso Then
                            Port_Control(Ind).Port_State = PCS_FAIL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL3in", PCS_STOP_COL)
                            ScriviAlarm("D/C error", 1)
                        Else
                            Port_Control(Ind).Port_State = PCS_NORMAL
                            frmRicezioneInstance.SetLabelColor("LblConfCNL3in", PCS_NORMAL_COL)
                        End If
                    Else
                        SocketManager.ListenDC(0, Port_Control(0).LanPort)
                    End If
                    g_IPDC = Port_Control(Ind).IPAdress

                ' INTELSAT (5) e PCU (6): nel VB6 di C10 LanReset non fa
                ' nulla per questi canali (sono seriali).

            End Select

            If Ind = CNL_1_IN Or Ind = CNL_2_IN Or Ind = GRF Then
                Dim timeout As Integer = 30000
                If Ind = ACU AndAlso b_Parametri Then
                    timeout = 900       ' VB6: lettura parametri ACU in corso
                ElseIf IsNumeric(Port_Control(Ind).Time_out) Then
                    timeout = CInt(Val(Port_Control(Ind).Time_out) * 1000)   ' prima CInt(0.4) = 0
                End If
                frmRicezioneInstance.SetTimeoutInterval(Ind, timeout)
            End If

            If Ind = ACU Then
                Port_Control(Ind).Rec_State = PCR_STANDBY
            Else
                Port_Control(Ind).Rec_State = PCR_WAIT
            End If
            Port_Control(Ind).Buffer = ""
        Catch ex As Exception
            WriteError(0, "LanReset " & Ind & " " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Proprietà di accesso alla StatusStrip
    ' -------------------------------------------------------
    Public Property MessaggioStatus As String
        Get
            Return stpMessaggio.Text
        End Get
        Set(value As String)
            stpMessaggio.Text = value
        End Set
    End Property

    Public Property DataStatus As String
        Get
            Return stpData.Text
        End Get
        Set(value As String)
            stpData.Text = value
        End Set
    End Property

    Public Property DiscoStatus As String
        Get
            Return stpDisco.Text
        End Get
        Set(value As String)
            stpDisco.Text = value
        End Set
    End Property

    Public Property ProgramStatus As String
        Get
            Return stpProgram.Text
        End Get
        Set(value As String)
            stpProgram.Text = value
        End Set
    End Property

    Public Property PrintStatus As String
        Get
            Return stpPrint.Text
        End Get
        Set(value As String)
            stpPrint.Text = value
        End Set
    End Property

    Public Property BeepStatus As String
        Get
            Return stpBeep.Text
        End Get
        Set(value As String)
            stpBeep.Text = value
        End Set
    End Property

    ' -------------------------------------------------------
    ' Riferimento all'istanza di frmRicezione
    ' -------------------------------------------------------
    Public frmRicezioneInstance As FrmRicezione

    ' -------------------------------------------------------
    ' Timer orologio principale (era tmrOrologio_Timer)
    ' -------------------------------------------------------
    Private Sub TmrOrologio_Tick(sender As Object, e As EventArgs) Handles TmrOrologio.Tick
        Try
            OraAtt = If(s_Ora = "G", DateTime.UtcNow, DateTime.Now)
            Dim i_OffsetOre As Double = If(s_Ora = "G", (DateTime.Now - DateTime.UtcNow).TotalHours, 0)

            Dim DatePc As String = OraAtt.ToString("dd/MM/yy")
            Dim DateRep As String = OraAtt.ToString("_dd")
            Dim MyMonth As String = MonthName(OraAtt.Month)

            ' Pannello "data" = ora ACU, scritto solo dal parser (VB6): non più riscritto qui con l'ora PC a ogni tick.

            ' Cambio giorno — rotazione file log
            If OraAtt < OraAgg Then
                FrmNorad.AggNorad(forza:=True)

                Dim MyWeekDay As Integer = Weekday(OraAtt)
                If MyWeekDay = 1 Then
                    If Not Directory.Exists(g_DirBackUp) Then
                        Directory.CreateDirectory(g_DirBackUp)
                    End If
                    For Each fi As String In Directory.GetFiles(Application.StartupPath)
                        File.Copy(fi, Path.Combine(g_DirBackUp, Path.GetFileName(fi)), True)
                    Next
                End If

                Dim monthDir As String = Path.Combine(Application.StartupPath, MyMonth)
                If Not Directory.Exists(monthDir) Then
                    Directory.CreateDirectory(monthDir)
                End If

                CreaFilesLog(MyMonth, DateRep)
                For Each rel In {f_Log, f_Log1, f_Log2, f_Log3}
                    File.WriteAllText(Path.Combine(Application.StartupPath, rel), "")
                Next
            End If
            OraAgg = OraAtt

            ' DriveInfo.AvailableFreeSpace gira una API Windows che
            ' interroga il filesystem — su questa macchina, con tutto
            ' l'I/O di oggi (log, antivirus), può occasionalmente
            ' bloccarsi per secondi. Veniva eseguita ad OGNI tick (ogni
            ' 50ms, 20 volte al secondo): lo spazio libero su disco non
            ' cambia abbastanza in fretta da giustificarlo — un controllo
            ' al giorno (indicazione dell'utente) basta e avanza.
            If (DateTime.Now - ultimoControlloDisco).TotalHours >= 24 Then
                ultimoControlloDisco = DateTime.Now
                Try
                    Dim di As New DriveInfo(Application.StartupPath.Substring(0, 1))
                    Dim freeGb As Double = Math.Round(di.AvailableFreeSpace / 1073741824, 3)
                    stpDisco.Text = freeGb & " Gb"
                Catch
                End Try
            End If

            If OraACU > CDate("00:00:00") Then
                If IsDate(OraACU) Then
                    Dim OraPc As Date = Now.AddHours(-i_OffsetOre)
                    Dim diffSec As Double = Math.Abs(DateDiff(DateInterval.Second, OraPc, OraACU))
                    ' C10: g_Irig = "A" (impostato dal parser ACU quando l'ora
                    ' viene dall'orologio ACU, non da IRIG) -> l'ora/la data
                    ' dell'ACU si riallineano al PC con CmdTime/CmdDate.
                    If diffSec > 10 And Not b_Parametri And g_Irig = "A" Then
                        Attesa(0.2)
                        CmdTime()
                    ElseIf OraPc.Date <> OraACU.Date And Not b_Parametri And g_Irig = "A" Then
                        Attesa(0.2)
                        CmdDate()
                    ElseIf diffSec > 10 And Not b_Parametri Then       ' C10: 10 s (FOC6: 3 s)
                        If Cont3 > 1800 Then
                            ScriviAlarm("Time ACU different", 1)
                            Cont3 = 0
                        Else
                            Cont3 += 1
                        End If
                    Else
                        Cont3 = 0
                    End If
                    ' Controllo della data ACU (VB6: DatePc <> DateAcu, contatore Cont4)
                    If OraPc.Date <> OraACU.Date And Not b_Parametri And g_Irig <> "A" Then
                        If Cont4 > 1800 Then
                            ScriviAlarm("Time ACU different", 1)
                            Cont4 = 0
                        Else
                            Cont4 += 1
                        End If
                    Else
                        Cont4 = 0
                    End If
                End If
            End If
            OraACU = CDate("00:00:00")

            MilSec = CLng(Now.Minute * 60000L + Now.Second * 1000L + Now.Millisecond)

            Dim Port As Integer = Canali(ACU).Port_IN
            If i_Cont < 2 Then
                If i_Status <> OP_PROGRAMTRACK And i_Status <> OP_AUGMENTED Then
                    i_Cont = 1
                End If
            End If

            Select Case i_Cont
                Case 0
                    If i_Status = OP_PROGRAMTRACK Or i_Status = OP_AUGMENTED Then
                        Dim MilSecC1 As Long
                        If MilSec < MilSecC Then
                            MilSecC1 = MilSec + MilOra - MilSecC
                        Else
                            MilSecC1 = MilSec - MilSecC
                        End If
                        If MilSecC1 >= i_IntCom * 1000 Then
                            Dim dataCalc As Date = Now.AddHours(-i_OffsetOre).AddSeconds(DeltaTime)
                            d_DataInizioProg = ""
                            stpProgram.Text = d_DataInizioProg
                            ' C10 (VB6): su seriale non si invia mentre una
                            ' risposta ACU è in ricezione.
                            Dim seriale As Boolean = Port_Control(ACU).Socket <> "Y" AndAlso Port_Control(ACU).Socket <> "S"
                            If Not seriale OrElse Port_Control(ACU).Rec_State <> PCR_RECEIVE Then
                                MilSecC = MilSec
                                b_Ricezione(ACU) = False
                                b_int(ACU) = True
                                Calcolo_Coordinate(dataCalc, Now.Millisecond)
                                If seriale Then Port_Control(ACU).Rec_State = PCR_WAIT
                            End If
                        End If
                    End If

                Case 1
                    Dim MilSecA1 As Long
                    If MilSec < MilSecA Then
                        MilSecA1 = MilSec + MilOra - MilSecA
                    Else
                        MilSecA1 = MilSec - MilSecA
                    End If
                    If MilSecA1 >= i_Inter * 1000 Then
                        ' C10: nuova richiesta di stato solo se la precedente
                        ' ha avuto risposta (evita interrogazioni sovrapposte).
                        If Not b_Parametri AndAlso Not b_Ricezione(ACU) Then
                            b_Ricezione(ACU) = True
                            b_int(ACU) = True
                            InvioACU(Chr(STX) & "07*2F" & Chr(ETX))
                            Port_Control(ACU).Rec_State = PCR_WAIT
                            MilSecA = MilSec
                        End If
                    End If
            End Select

            If i_Cont < 2 Then
                i_Cont += 1
                If i_Cont > 1 Then i_Cont = 0
            End If

            ' Interrogazione generatore di frequenza / Down Converter (C10):
            ' richiesta frequenza; con la finestra D/C aperta (b_down) anche
            ' le attenuazioni A e B a rotazione (Contv).
            Dim MilSecF1 As Long
            If MilSec < MilSecF Then
                MilSecF1 = MilSec + MilOra - MilSecF
            Else
                MilSecF1 = MilSec - MilSecF
            End If
            If MilSecF1 >= i_InterFre * 1000 Then
                MilSecF = MilSec
                b_Ricezione(GRF) = True
                b_int(GRF) = True
                If b_down Then
                    Select Case Contv
                        Case 0
                            frmRicezioneInstance.CmdGRF()
                        Case 1
                            FrmDownConverter.CmdATA()
                        Case 2
                            FrmDownConverter.CmdATB()
                    End Select
                    Contv += 1
                    If Contv > 2 Then Contv = 0
                Else
                    frmRicezioneInstance.CmdGRF()
                End If
                If frmRicezioneInstance IsNot Nothing Then
                    frmRicezioneInstance.TmrTimeoutMSG(GRF).Enabled = False
                    frmRicezioneInstance.TmrTimeoutMSG(GRF).Enabled = True
                End If
            End If

            ' Interrogazione polarizzatore (C10, seriale PCU): ogni i_InterPol s.
            Dim MilSecP1 As Long
            If MilSec < MilSecP Then
                MilSecP1 = CLng(MilSec + MilOra - MilSecP)
            Else
                MilSecP1 = CLng(MilSec - MilSecP)
            End If
            If MilSecP1 >= i_InterPol * 1000 Then
                MilSecP = MilSec
                b_Ricezione(PCU) = True
                b_int(PCU) = True
                If frmRicezioneInstance IsNot Nothing Then
                    frmRicezioneInstance.CmdPol()
                    frmRicezioneInstance.TmrTimeoutMSG(PCU).Enabled = True
                End If
            End If

            Dim MilSecL1 As Long
            If MilSec < MilSecL Then
                MilSecL1 = MilSec + MilOra - MilSecL
            Else
                MilSecL1 = MilSec - MilSecL
            End If
            If MilSecL1 >= i_Log * 1000 Then
                MilSecL = MilSec
                ScriviLOG3()
            End If

            Cont2 += 1
            If Cont2 > 10 Then
                Cont2 = 0

                ' Voce di menu "Acquire Trace" visibile solo con uno Spiral/
                ' Automatic Acquire attivo. Sincronizzata qui (battito neutro
                ' dell'orologio principale, non legato a ricevitore o spirale)
                ' così resta coerente con lo stato reale qualunque sia il modo
                ' in cui l'Acquire finisce (timeout, Annulla, completamento),
                ' senza dover intercettare ogni singolo punto di uscita. Stesso
                ' fix già presente in BTS1\frmMain.vb, mancante qui.
                If frmRicezioneInstance IsNot Nothing Then
                    frmRicezioneInstance.MnuAcquireTrace.Visible = (i_Status = OP_SPIRALACQUIRE OrElse i_Status = OP_AUTOMATICAQUIRE)
                End If

                If g_ChangePhase = "Y" Then
                    If g_NewLna Then
                        InvioFrequenza()
                        g_NewLna = False
                        g_NewFrequency = True
                    End If
                Else
                    g_NewLna = False
                End If
                If g_FreqNew Then
                    CambioFreq(i_FreqNew)
                    g_FreqNew = False
                End If

                ' ========================================================
                ' Simula la ricezione dall'ACU per poter
                ' testare FOC6 (anche da remoto via noVNC) senza hardware
                ' reale collegato. Stesso meccanismo già in uso su BTS1
                ' (frmMain.vb): il messaggio simulato entra nello stesso
                ' buffer e passa dallo stesso parser (AnalisiOrdini /
                ' ValoreCampoMAC) dei dati veri — nessun bypass. Formato a
                ' campi fissi identico a BTS1 (FOC6 fu allineato apposta al
                ' protocollo BTS1, vedi commenti in Gestione.vb). Simulazione
                ' compilata SOLO nella configurazione "Simulazione": serve per testare senza
                ' hardware ACU reale collegato (anche da remoto via noVNC).
#If SIMULAZIONE Then
                If frmRicezioneInstance IsNot Nothing Then
                    Dim azSim As Double, elSim As Double
                    Dim daTLE As Boolean = False
                    ' Se è selezionato un satellite reale (Algoritmo = 1,
                    ' TLE/SGP4), propaga la sua orbita vera con lo stesso
                    ' motore del tracking reale — Az/El simulati seguono
                    ' quindi una traiettoria plausibile invece di
                    ' un'oscillazione arbitraria.
                    If s_Satellite <> "" AndAlso Algoritmo = 1 Then
                        Try
                            If s_SatelliteAcuSimulato <> s_Satellite Then
                                If Convert_Satellite_Data(s_Satellite) Then
                                    s_SatelliteAcuSimulato = s_Satellite
                                Else
                                    s_SatelliteAcuSimulato = ""
                                End If
                            End If
                            If s_SatelliteAcuSimulato = s_Satellite Then
                                Dim posV As Vector = NewVector(), velV As Vector = NewVector()
                                Dim ts1 As time_set
                                Dim nowSim As Date = NowUTC()
                                With ts1
                                    .yr = Year(nowSim) : .mo = Month(nowSim) : .dy = nowSim.Day
                                    .hr = Hour(nowSim) : .mi = Minute(nowSim) : .se = Second(nowSim)
                                    .hu = nowSim.Millisecond
                                End With
                                Dim Timen As Double = Julian_Date(ts1)
                                Dim tsincex As Double = (Timen - julian_epoch) * xmnpda
                                If ideep = 0 Then PosSGP4(tsincex, posV, velV) Else PosSDP4(tsincex, posV, velV)
                                Convert_Sat_State(posV, velV)
                                Calculate_Obs(posV, velV, Timen, obs_set)
                                azSim = obs_set.Valori(1) * RadGradi
                                elSim = obs_set.Valori(2) * RadGradi
                                If azSim < 0 Then azSim += 360
                                If azSim > 360 Then azSim -= 360
                                daTLE = True
                            End If
                        Catch ex As Exception
                            WriteError(0, "FrmMain.TmrOrologio_Tick: simulazione ACU da TLE - " & ex.Message)
                            daTLE = False
                        End Try
                    End If
                    If Not daTLE Then
                        s_SatelliteAcuSimulato = ""
                        ' Segue l'ultimo comando inviato (Az/El a video)
                        ' invece di un'oscillazione scorrelata, utile per
                        ' testare realisticamente l'aggancio/clamp
                        ' dell'offset. Rumore casuale ±1.5×Max_Offset così
                        ' l'errore a volte sta dentro soglia e a volte no.
                        If IsNumeric(frmRicezioneInstance.LblAzimuth.Text) AndAlso
                           IsNumeric(frmRicezioneInstance.LblElevazione.Text) Then
                            Dim ampiezza As Double = Math.Abs(Max_Offset) * 1.5
                            azSim = CDbl(frmRicezioneInstance.LblAzimuth.Text) + (rndSimAcu.NextDouble() * 2.0 - 1.0) * ampiezza
                            elSim = CDbl(frmRicezioneInstance.LblElevazione.Text) + (rndSimAcu.NextDouble() * 2.0 - 1.0) * ampiezza
                            If azSim < 0 Then azSim += 360
                            If azSim > 360 Then azSim -= 360
                        Else
                            ' Nessun comando ancora inviato: oscillazione
                            ' di ripiego solo per avere un valore plausibile.
                            Dim tSec As Double = NowUTC().TimeOfDay.TotalSeconds
                            azSim = 180.0 + 5.0 * Math.Sin(tSec / 3.0)
                            elSim = 45.0 + 2.0 * Math.Cos(tSec / 3.0)
                        End If
                    End If
                    ' Frame simulato nel formato ACU di C10 (= FOC6/C03/K09 VB6,
                    ' NON BTS1): stesse posizioni lette da Gestione.ValoreCampoMAC.
                    Dim sAz As String = CInt(azSim * 1000).ToString("000000")   ' pos 33-38
                    Dim sEl As String = CInt(elSim * 1000).ToString("00000")    ' pos 39-43
                    ' Livello beacon: SEMPRE da d_SimBeaconLevel
                    ' (AvanzaSimulazioneBeacon, Ricezione.vb) invertendo la
                    ' formula del parser C10: s_Lev = (V - 10) * Pendenza + Valore0,
                    ' con V = CalcoloVolt(3 byte). Livello mostrato, BCN Level
                    ' e bit di lock restano così la stessa fonte.
                    Dim vLev As Double = If(Pendenza <> 0, (d_SimBeaconLevel - Valore0) / Pendenza + 10, 10)
                    Dim nowAcu As Date = NowUTC()
                    Dim modo As String = Chr(&H40 Or BIT6)                      ' pos 20: Stand-by, REMOTE
                    Dim attiviA0 As String = Chr(&H40 Or BIT1 Or BIT2 Or BIT3 Or BIT4)   ' nessun fault/limite (bit attivi a 0)
                    Dim emergenza As String = Chr(&H40 Or BIT3 Or BIT4)         ' pos 26: nessun emergency stop
                    Dim drive As String = Chr(&H40 Or BIT1 Or BIT3 Or BIT4)     ' pos 27: drive/freni ok
                    Dim lnaPol As String = Chr(&H40 Or BIT1 Or BIT3)            ' pos 30: LNA 1, polarizzatore in posizione
                    ' pos 31: lock = bit4 (letto da AggiornaBeaconLock), bit1 = 0 -> lineare
                    Dim lockByte As String = Chr(&H40 Or If(b_Lock, BIT4, 0))
                    Dim rxByte As String = Chr(&H40 Or BIT1 Or BIT3)            ' pos 48: RX ok, segnale ok
                    Dim messaggio As String =
                        Chr(2) & "0069" &                            ' pos 1-5: STX + lunghezza totale (69)
                        "*" &                                       ' pos 6
                        "@" &                                       ' pos 7: bit1=0 -> ora da orologio ACU
                        nowAcu.DayOfYear.ToString("000") &          ' pos 8-10: giorno giuliano
                        nowAcu.ToString("HHmmssfff") &              ' pos 11-19
                        modo &                                      ' pos 20
                        "Q" & "A" &                                 ' pos 21-22: backup, switches
                        "@" &                                       ' pos 23: SW prelimit
                        attiviA0 & attiviA0 &                       ' pos 24-25: fault, HW limit
                        emergenza & drive &                         ' pos 26-27
                        "@" & "@" &                                 ' pos 28-29: pin
                        lnaPol & lockByte &                         ' pos 30-31
                        "@" &                                       ' pos 32: memorie
                        sAz & sEl &                                 ' pos 33-43
                        "+000" &                                    ' pos 44-47: polarizzazione
                        rxByte &                                    ' pos 48
                        CodificaVolt(vLev) &                        ' pos 49-51: livello somma
                        CodificaVolt(10) & CodificaVolt(10) &       ' pos 52-57: errori Az/El = 0
                        CodificaVolt(10) & CodificaVolt(10) &       ' pos 58-63: velocità = 0
                        "@" & "@" & "@"                             ' pos 64-66
                    Dim cksSim As Integer = 0
                    For Each c As Char In messaggio : cksSim = cksSim Xor AscW(c) : Next
                    Dim frameSimulato As String = messaggio & (cksSim And &HFF).ToString("X2") & Chr(3)    ' + checksum XOR reale + ETX (VerDatiMAC)
                    Port_Control(ACU).Buffer &= frameSimulato
                    frmRicezioneInstance.AnalisiOrdini(ACU, Port_Control(ACU).Buffer)
                    ' Va chiamata DOPO AnalisiOrdini, non prima: AnalisiOrdini
                    ' (via AggiornaBeaconLock, quando il canale RX non è
                    ' collegato) copia s_Lev1 = s_Lev, cioè il valore
                    ' decodificato dal campo Lev2 del frame appena iniettato
                    ' sopra - un numero intero (CInt), quindi più grezzo del
                    ' Double preciso calcolato qui. Chiamata prima, la
                    ' piccola oscillazione post-aggancio veniva sovrascritta
                    ' subito dal valore intero arrotondato, apparendo
                    ' "bloccata" su un unico valore invece di muoversi.
                    ' permettiAvvicinamento:=False SOLO quando una ricerca
                    ' Acquire è attiva (i_Spiral >= 1): qui non deve far
                    ' avanzare la fase di ricerca (quello lo fa solo
                    ' Calcolo_Aquire, al proprio ritmo, altrimenti il livello
                    ' avanzerebbe due volte) — solo mantenere la piccola
                    ' oscillazione una volta già agganciato, altrimenti dopo
                    ' Augmented si blocca. FUORI da un Acquire (i_Spiral < 1)
                    ' Calcolo_Aquire non viene mai chiamato, quindi non c'è
                    ' rischio di doppio avanzamento: permesso avanzare anche
                    ' qui, altrimenti il beacon resta fermo per sempre a
                    ' -95 fuori da una ricerca (segnalato: "il valore beacon
                    ' rimane a -95 e non si incrementa").
                    AvanzaSimulazioneBeacon(permettiAvvicinamento:=(i_Spiral < 1))
                    ' Il watchdog TmrTimeoutMSG(ACU) viene "nutrito" solo dal
                    ' vero callback di ricezione; qui si inietta il frame
                    ' direttamente nel buffer, quindi va resettato a mano
                    ' per non far scattare "Time out ACU" nonostante la
                    ' telemetria simulata arrivi regolarmente.
                    If frmRicezioneInstance.TmrTimeoutMSG IsNot Nothing AndAlso frmRicezioneInstance.TmrTimeoutMSG(ACU) IsNot Nothing Then
                        frmRicezioneInstance.TmrTimeoutMSG(ACU).Enabled = False
                        frmRicezioneInstance.TmrTimeoutMSG(ACU).Enabled = True
                    End If
                End If
#End If
                ' ========================================================
            End If

            ' Controllo congruità valore comandato/ricevuto della frequenza
            ' beacon (era "If Contfreq > 50" — stesso contatore/soglia VB6).
            Contfreq += 1
            If Contfreq > 50 Then
                Contfreq = 0
                If frmRicezioneInstance IsNot Nothing Then
                    frmRicezioneInstance.ControllaFrequenzaBeacon()
                End If
            End If

            TmrOrologio.Enabled = True
        Catch ex As Exception
            TmrOrologio.Enabled = True
            WriteError(0, "Orologio " & ex.Message)
        End Try
    End Sub

#If SIMULAZIONE Then
    ''' <summary>
    ''' Inverso di CalcoloVolt (simulazione ACU C10): tensione 0..20 V nei
    ''' 3 byte del protocollo (nibble basso di ciascun carattere, |0x40).
    ''' </summary>
    Private Shared Function CodificaVolt(volt As Double) As String
        Dim n As Integer = CInt(Math.Round(volt / Conv_Volt))
        n = Math.Max(0, Math.Min(4095, n))
        Return Chr(&H40 Or (n And &HF)) & Chr(&H40 Or ((n >> 4) And &HF)) & Chr(&H40 Or ((n >> 8) And &HF))
    End Function
#End If
End Class
