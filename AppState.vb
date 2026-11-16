Imports System.Runtime.CompilerServices
Imports System.Data.SQLite

Public Module AppState
    Public MainForm As FrmMain

    ' Variabili di stato generali
    Public b_Calcola As Boolean
    Public b_Parametri As Boolean
    ' Dimensionati a 8 (non 5 come BTS1): FOC6 usa canali fino a
    ' SCU=8 (Predisposizioni.vb: ACU=1, MAC=2, GRF=4, LNA=7, SCU=8).
    Public b_Ricezione(8) As Boolean
    Public b_Sat As Boolean
    Public b_Checked As Boolean
    Public b_StatusPC1 As Boolean
    Public b_int(8) As Boolean
    Public b_down As Boolean
    Public b_ClosePre As Boolean
    Public b_ClosePreset As Boolean

    ' Configurazione
    ' "Satellite" (variabile separata, solo in BTS1) e "s_Satellite"
    ' (ComuniIntelsatIntersat1\Predisposizioni.vb) dovevano rappresentare la
    ' stessa cosa ma venivano aggiornate in punti diversi del codice, non
    ' sempre insieme (es.: selezionare un satellite in FrmSatelliti
    ' aggiornava solo s_Satellite, mentre CmdLoad_Click in frmRicezione
    ' leggeva "Satellite" — bug diagnosticato l'8/10/2026: "Carica"
    ' continuava a usare il satellite precedente). "Satellite" è stata
    ' eliminata: tutto il codice BTS1 usa ora solo s_Satellite.
    Public g_Server As String = ""
    Public g_Database As String = ""
    Public g_Log As String = ""
    Public g_DirBackUp As String = ""
    Public g_IPDC As String = ""
    ' VB6 di FOC6 confronta g_Irig con "A" (If g_Irig = "A" Then):
    ' selettore sincronizzazione orario (IRIG automatico vs manuale),
    ' non un flag booleano come erroneamente portato in precedenza.
    Public g_Irig As String = ""
    Public g_3Riga As String = ""

    ' Parametri antenna
    Public AzOffset1 As Double
    Public ElOffset1 As Double
    Public AzOffset As Double
    Public ElOffset As Double
    Public AzCalc As Double
    Public ElCalc As Double
    Public LATSTA11 As Double
    Public LONSTA11 As Double
    Public i_Off As Integer
    Public i_Status As Integer
    Public i_StatusDiff As Integer
    Public i_ProgTrack As Integer
    Public i_Cont As Integer
    Public i_InterFre As Integer
    Public i_XT As Integer
    Public i_YT As Integer
    Public i_Log As Integer
    Public i_FreqNew As Integer
    Public i_AsseXGrafico As Integer

    ' Timing
    Public OraAgg As Date
    Public OraACU As Date
    Public MilSec As Long
    Public MilSecF As Long
    Public MilSecL As Long
    Public Const MilOra As Long = 3600000
    Public d_DataInizioProg As String
    Public PTK As Double
    Public DeltaTime As Double
    Public TimeZoneOff As Double
    Public i_Offset As Integer

    ' Frequenze
    Public g_FreqNew As Boolean
    Public g_NewFrequency As Boolean
    Public g_NewLna As Boolean
    Public g_ChangePhase As String = "N"

    ' Rete
    Public nSock As Integer
    Public intSockCnt As Integer
    Public LanPortRx As Integer

    ' Segnali
    Public s_Lev1 As String
    Public NumProMax As Long

    ' System.Math.tanti operative
    ' FOC-C10: 6 canali (VB6 Prog0347-FOCC10\Gestione.bas: NumCom = 6;
    ' SMOD1574.ini sezioni [Canale1]..[Canale6]): 1=ACU, 2=MCS, 3=RX,
    ' 4=GRF/FREQUENZIMETRO, 5=INTELSAT, 6=POLARIZZAZIONE. Gli array
    ' restano dimensionati come in FOC6 (fino a 9): gli indici 7-9 non
    ' sono usati.
    ' (Nota originale FOC6:) FOC6 ha 9 canali, non 4 come BTS1 — confermato dal file ini
    ' legacy (SMOD15741.ini, sezioni [Canale1]..[Canale9]) e dal
    ' control array cmbMaster/txtIndLan/... (Index 1..9) nel VB6
    ' originale di FOC6 (frmConfigurazione.Frm): 1=ACU, 2=MCS, 3=RX,
    ' 4=GRF/FREQUENZIMETRO, 5=INTELSAT, 6=POLARIZZAZIONE, 7=LNA,
    ' 8=FILTER, 9=MODALITA' POLARIZZAZIONE.
    Public Const NumCom As Integer = 6
    Public Const CNL_1_IN As Integer = 1
    Public Const CNL_2_IN As Integer = 2
    Public Const CNL_3_IN As Integer = 3
    Public Const CNL_4_IN As Integer = 4


    ' System.Math.tanti Socket
    Public Const sckConnected As Integer = 7
    Public Const sckConnecting As Integer = 6
    Public Const sckListening As Integer = 8
    Public Const sckError As Integer = 11

    ' System.Math.tanti Windows
    Public Const vbUnchecked As Integer = 0
    Public Const vbChecked As Integer = 1

    ' Database
    Public dbOrbite As Object
    Public NoPhasing As Boolean
    Public b_Stars As Boolean
    Public Algoritmo As Integer
    Public grado As Integer
    Friend Canali(0 To 9) As Canale_rec
    ' Arrays / scalars
    Public g_AzTable As Double
    Public g_ElTable As Double

    ' Serial ports (replace VB6 MSCOMM32 OCX array) — inizializzati a Nothing, allocati con ReDim
    Public ComPort(9) As System.IO.Ports.SerialPort   ' 9 come FOC6: indici 7-9 restano Nothing

    ' Missing constants (DAO compat + port state values)
    Public Const dbOpenDynaset As Integer = 2
    Public Const dbOpenSnapshot As Integer = 4
    ' BUG CORRETTO: questi due valori erano 0/1, in collisione con
    ' PCR_RECEIVE=1 (definito in Predisposizioni.vb) — ogni "Rec_State =
    ' PCR_WAIT" scriveva lo stesso valore di PCR_RECEIVE, e il controllo
    ' "Rec_State <> PCR_RECEIVE" in TmrSpiralAcquire_Tick risultava sempre
    ' falso, bloccando Calcolo_Aquire per sempre. Valori corretti dal VB6
    ' originale (Predisposizioni.bas): PCR_WAIT=0, PCR_RECEIVE=1, PCR_STANDBY=2.
    Public Const PCR_STANDBY As Integer = 2
    Public Const PCR_WAIT As Integer = 0
    Public Const PCS_OK As Integer = 1
    Public Const PCS_FAIL As Integer = 3
    Public Const PCS_NOT_OP As Integer = 6
    Public Const PCS_NORMAL As Integer = 1
    Public ReadOnly PCS_STOP_COL As System.Drawing.Color = System.Drawing.Color.Yellow   ' VB6: vbYellow (non collegato)

    ' Antenna commanded positions (from ACU telemetry) — not in Predisposizioni
    Public AzComandato As Double
    Public ElComandato As Double

    ' Receiver state — not in Predisposizioni
    Public BeaconLevel As Double
    Public g_Phase As Double

    ' Down Converter — not in Predisposizioni
    Public g_FreqDC As Double


    ' VB6 color constant aliases
    Public ReadOnly vbRed As System.Drawing.Color = System.Drawing.Color.Red
    Public ReadOnly vbGreen As System.Drawing.Color = System.Drawing.Color.Lime   ' vbGreen = RGB(0,255,0)
    Public ReadOnly vbYellow As System.Drawing.Color = System.Drawing.Color.Yellow
    Public ReadOnly vbBlue As System.Drawing.Color = System.Drawing.Color.Blue

    ' Protocollo binario RX (Ricevitore.bas): "Global Const rx_Header = 1234567890" /
    ' "Global Const rx_Postamble = -1234567890". Erano rimasti stub a 0 ("values to
    ' be set from config", mai fatto): ogni frame Phasing/InvioPhase/MonComponent
    ' partiva con un header 0 invece del valore reale, e la validazione in
    ' frmRicevitore.LblInputRx_Change ("If rx_Buffer = rx_Header") non avrebbe mai
    ' potuto corrispondere a una risposta reale del DTR.
    Public Const rx_Header As Integer = 1234567890
    Public Const rx_Postamble As Integer = -1234567890

    ' -------------------------------------------------------
    ' Sostituto di DbExecute(sql) — chiamata con late
    ' binding non trova extension method su As Object
    ' -------------------------------------------------------
    ' Restituisce l'orario corrente corretto per il fuso orario (UTC se Ora=G)
    Public Function NowUTC() As DateTime
        Return DateTime.UtcNow
    End Function

    Public Sub DbExecute(sql As String)
        Try
            Dim conn As SQLiteConnection = CType(dbOrbite, DAODatabase).Connection
            Using Cmd As New SQLiteCommand(sql, conn)
                Cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            WriteError(0, "DbExecute: " & ex.Message & " SQL=" & sql)
        End Try
    End Sub

End Module

