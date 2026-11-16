Imports System.Net
Imports System.Net.Sockets
Imports System.Text

' ============================================================
' SocketManager.vb
' Gestione connessioni TCP in sostituzione di MSWINSCK.OCX
' ============================================================
' Canali gestiti:
'   ACU  (Ind=1) -> sckMain   — controllo antenna
'   MAC  (Ind=2) -> sckMac    — sistema MCS
'   RX   (Ind=3) -> sckRX()   — ricevitore (6 socket)
'   GRF  (Ind=4) -> sckDC     — down converter
' ============================================================

Public Module SocketManager

    ' Encoding.ASCII troncherebbe/corromperebbe (a '?') ogni byte >= 0x80:
    ' i protocolli binari (es. RX verso il ricevitore, frame di stato MAC)
    ' trasmettono byte pieni 0-255 dentro la String, esattamente come
    ' Winsock SendData/GetData in VB6. Latin1 mappa 1:1 ogni valore
    ' 0-255 ed è comunque compatibile con i canali puramente testuali.
    Private ReadOnly _enc As Encoding = Encoding.GetEncoding(28591)

    ' -------------------------------------------------------
    ' Array socket per ogni canale
    ' -------------------------------------------------------
    Private _main(4) As TcpClient
    Private _mac(4) As TcpClient
    Private _rx(6) As TcpClient
    Private _dc(4) As TcpClient

    Private _streamMain(4) As NetworkStream
    Private _streamMac(4) As NetworkStream
    Private _streamRX(6) As NetworkStream
    Private _streamDC(4) As NetworkStream

    ' Listener attivi per canale: senza tracciarli, ogni nuova chiamata a
    ' Listen*/ecc. crea un TcpListener che resta bloccato sulla porta finché
    ' nessun client si connette (es. test senza hardware reale) — il
    ' tentativo successivo (da un watchdog di timeout) fallisce sempre con
    ' "indirizzo già in uso". Si ferma il listener precedente prima di aprirne uno nuovo.
    Private _listenerMain(4) As TcpListener
    Private _listenerMac(4) As TcpListener
    Private _listenerRX(6) As TcpListener
    Private _listenerDC(4) As TcpListener

    ' Buffer di ricezione. RX usa 6 sotto-socket indipendenti (Mon/Ctrl/
    ' Rst/Log/Phase/Cal): un buffer condiviso tra letture asincrone
    ' concorrenti farebbe sovrascrivere i dati a vicenda, quindi uno per
    ' sotto-socket.
    Private _bufMain(4095) As Byte
    Private _bufMac(4095) As Byte
    Private _bufRX(6)() As Byte
    Private _bufDC(4095) As Byte

    ' Delegate per callback ricezione dati
    Public Delegate Sub DataReceivedCallback(ind As Integer, data As String)
    Public OnMainReceived As DataReceivedCallback
    Public OnMacReceived As DataReceivedCallback
    Public OnRXReceived As DataReceivedCallback
    Public OnDCReceived As DataReceivedCallback

    ' -------------------------------------------------------
    ' Connessione ACU (canale principale)
    ' -------------------------------------------------------
    Public Function Connect(ind As Integer, host As String, port As Integer) As Boolean
        Try
            If _main(ind) IsNot Nothing AndAlso _main(ind).Connected Then
                _main(ind).Close()
            End If
            _main(ind) = New TcpClient()
            Dim connectTask = _main(ind).ConnectAsync(host, port)
            If connectTask.Wait(2000) AndAlso _main(ind).Connected Then
                _streamMain(ind) = _main(ind).GetStream()
                AvviaRicezioneMain(ind)
                Return True
            Else
                ' Wait(2000) smette solo di ASPETTARE: il tentativo di
                ' connessione sottostante resta vivo finché non lo si chiude
                ' esplicitamente, altrimenti continua in background fino al
                ' timeout TCP nativo di Windows (spesso 20+ secondi) — con un
                ' nuovo tentativo che riparte ogni ~2s sopra di esso, accumulo
                ' di connessioni pendenti abbandonate che finisce per
                ' intaccare anche il thread UI (ThreadPool/scheduler sotto
                ' carico). Causa quasi certa dei gap di 3+ secondi osservati
                ' in test, ricorrenti ogni pochi secondi per tutta la sessione.
                _main(ind).Close()
                ' TcpClient.Connected su un oggetto chiuso lancia
                ' NullReferenceException invece di restituire False (quirk
                ' noto di .NET Framework) — azzerare il riferimento evita
                ' che IsConnected/SendMain ci inciampino di nuovo (il
                ' Try/Catch lì li protegge comunque, ma niente più eccezioni
                ' "di primo tentativo" rumorose col debugger agganciato).
                _main(ind) = Nothing
            End If
        Catch ex As Exception
            WriteError(0, "SocketManager.Connect ACU " & ind & ": " & ex.Message)
        End Try
        Return False
    End Function

    Public Function IsConnected(ind As Integer) As Boolean
        Try
            Dim c As Net.Sockets.TcpClient = _main(ind)   ' vedi IsRXConnected
            Return c IsNot Nothing AndAlso c.Client IsNot Nothing AndAlso c.Connected
        Catch
            Return False
        End Try
    End Function

    Public Sub Listen(ind As Integer, localPort As Integer)
        Try
            Try
                If _listenerMain(ind) IsNot Nothing Then _listenerMain(ind).Stop()
            Catch
            End Try
            Dim listener As New TcpListener(IPAddress.Any, localPort)
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, True)
            _listenerMain(ind) = listener
            listener.Start()
            AccettaMainAsync(listener, ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.Listen port " & localPort & ": " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' AcceptTcpClientAsync, non AcceptTcpClient() su Task.Run: quest'ultimo
    ''' blocca per sempre un thread del ThreadPool in attesa di una
    ''' connessione che, su un canale "Listen" senza un dispositivo reale
    ''' collegato (es. in test), può non arrivare mai — saturando il pool e
    ''' ritardando di molto (osservato: fino a oltre un minuto) qualunque
    ''' altro lavoro in coda lì, incluso il server HTTP integrato
    ''' (HttpServer\AntennaHttpServer.vb). La versione asincrona non occupa
    ''' alcun thread mentre aspetta.
    ''' </summary>
    Private Async Sub AccettaMainAsync(listener As TcpListener, ind As Integer)
        Try
            _main(ind) = Await listener.AcceptTcpClientAsync()
            _streamMain(ind) = _main(ind).GetStream()
            AvviaRicezioneMain(ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.Listen ACU " & ind & ": " & ex.Message)
        Finally
            listener.Stop()
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Invio dati su ACU
    ' -------------------------------------------------------
    Public Function SendMain(ind As Integer, data As String) As Boolean
        Try
            If _streamMain(ind) Is Nothing OrElse _main(ind) Is Nothing OrElse Not _main(ind).Connected Then Return False
            Dim bytes() As Byte = _enc.GetBytes(data)
            _streamMain(ind).Write(bytes, 0, bytes.Length)
            Return True
        Catch ex As Exception
            WriteError(0, "SocketManager.SendMain " & ind & ": " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub AvviaRicezioneMain(ind As Integer)
        Try
            _streamMain(ind).BeginRead(_bufMain, 0, _bufMain.Length,
                AddressOf CallbackMain, ind)
        Catch ex As Exception
            WriteError(0, "AvviaRicezioneMain " & ind & ": " & ex.Message)
        End Try
    End Sub

    Private Sub CallbackMain(ar As IAsyncResult)
        Dim ind As Integer = CInt(ar.AsyncState)
        Try
            Dim bytesRead As Integer = _streamMain(ind).EndRead(ar)
            If bytesRead > 0 Then
                Dim data As String = _enc.GetString(_bufMain, 0, bytesRead)
                If OnMainReceived IsNot Nothing Then OnMainReceived.Invoke(ind, data)
                AvviaRicezioneMain(ind)
            End If
        Catch ex As Exception
            WriteError(0, "CallbackMain " & ind & ": " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Connessione MAC (canale 2)
    ' -------------------------------------------------------
    Public Function ConnectMac(ind As Integer, host As String, port As Integer) As Boolean
        Try
            If _mac(ind) IsNot Nothing AndAlso _mac(ind).Connected Then
                _mac(ind).Close()
            End If
            _mac(ind) = New TcpClient()
            Dim connectTask = _mac(ind).ConnectAsync(host, port)
            If connectTask.Wait(2000) AndAlso _mac(ind).Connected Then
                _streamMac(ind) = _mac(ind).GetStream()
                AvviaRicezioneMac(ind)
                Return True
            Else
                ' Vedi commento in Connect (ACU): chiude esplicitamente il
                ' tentativo scaduto invece di lasciarlo vivo in background.
                _mac(ind).Close()
                _mac(ind) = Nothing
            End If
        Catch ex As Exception
            WriteError(0, "SocketManager.ConnectMac " & ind & ": " & ex.Message)
        End Try
        Return False
    End Function

    Public Function IsMacConnected(ind As Integer) As Boolean
        Try
            Dim c As Net.Sockets.TcpClient = _mac(ind)   ' vedi IsRXConnected
            Return c IsNot Nothing AndAlso c.Client IsNot Nothing AndAlso c.Connected
        Catch
            Return False
        End Try
    End Function

    Public Sub ListenMac(ind As Integer, localPort As Integer)
        Try
            Try
                If _listenerMac(ind) IsNot Nothing Then _listenerMac(ind).Stop()
            Catch
            End Try
            Dim listener As New TcpListener(IPAddress.Any, localPort)
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, True)
            _listenerMac(ind) = listener
            listener.Start()
            AccettaMacAsync(listener, ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenMac port " & localPort & ": " & ex.Message)
        End Try
    End Sub

    ' AcceptTcpClientAsync invece di AcceptTcpClient() su Task.Run: vedi il
    ' commento su AccettaMainAsync (sopra) — qui per non saturare il
    ' ThreadPool quando il canale MAC/MCS resta "in ascolto" senza che nulla
    ' si connetta mai.
    Private Async Sub AccettaMacAsync(listener As TcpListener, ind As Integer)
        Try
            _mac(ind) = Await listener.AcceptTcpClientAsync()
            _streamMac(ind) = _mac(ind).GetStream()
            AvviaRicezioneMac(ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenMac " & ind & ": " & ex.Message)
        Finally
            listener.Stop()
        End Try
    End Sub

    Public Function SendMac(ind As Integer, data As String) As Boolean
        Try
            If _streamMac(ind) Is Nothing OrElse _mac(ind) Is Nothing OrElse Not _mac(ind).Connected Then Return False
            Dim bytes() As Byte = _enc.GetBytes(data)
            _streamMac(ind).Write(bytes, 0, bytes.Length)
            Return True
        Catch ex As Exception
            WriteError(0, "SocketManager.SendMac " & ind & ": " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub AvviaRicezioneMac(ind As Integer)
        Try
            _streamMac(ind).BeginRead(_bufMac, 0, _bufMac.Length,
                AddressOf CallbackMac, ind)
        Catch ex As Exception
            WriteError(0, "AvviaRicezioneMac " & ind & ": " & ex.Message)
        End Try
    End Sub

    Private Sub CallbackMac(ar As IAsyncResult)
        Dim ind As Integer = CInt(ar.AsyncState)
        Try
            Dim bytesRead As Integer = _streamMac(ind).EndRead(ar)
            If bytesRead > 0 Then
                Dim data As String = _enc.GetString(_bufMac, 0, bytesRead)
                If OnMacReceived IsNot Nothing Then OnMacReceived.Invoke(ind, data)
                AvviaRicezioneMac(ind)
            End If
        Catch ex As Exception
            WriteError(0, "CallbackMac " & ind & ": " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Connessione Ricevitore RX (canale 3 — 6 socket)
    ' -------------------------------------------------------
    Public Function ConnectRX(ind As Integer, host As String, port As Integer) As Boolean
        Try
            If _rx(ind) IsNot Nothing AndAlso _rx(ind).Connected Then
                _rx(ind).Close()
            End If
            _rx(ind) = New TcpClient()
            Dim connectTask = _rx(ind).ConnectAsync(host, port)
            If connectTask.Wait(2000) AndAlso _rx(ind).Connected Then
                _streamRX(ind) = _rx(ind).GetStream()
                _bufRX(ind) = New Byte(4095) {}
                AvviaRicezioneRX(ind)
                Return True
            Else
                ' Vedi commento in Connect (ACU): chiude esplicitamente il
                ' tentativo scaduto invece di lasciarlo vivo in background.
                ' Canale chiamato ogni 400ms (TmrPhase/MonComponent/Phasing):
                ' il più frequente dei quattro, quindi il più esposto
                ' all'accumulo di connessioni pendenti.
                _rx(ind).Close()
                _rx(ind) = Nothing
            End If
        Catch ex As Exception
            WriteError(0, "SocketManager.ConnectRX " & ind & ": " & ex.Message)
        End Try
        Return False
    End Function

    Public Function IsRXConnected(ind As Integer) As Boolean
        Try
            ' Copia locale: _rx(ind) può essere azzerato da ConnectRX su un altro
            ' thread. Dopo Close() TcpClient.Client è Nothing e .Connected lancia
            ' NullReferenceException (intercettata, ma fermava il debugger).
            Dim c As Net.Sockets.TcpClient = _rx(ind)
            Return c IsNot Nothing AndAlso c.Client IsNot Nothing AndAlso c.Connected
        Catch
            Return False
        End Try
    End Function

    Public Sub ListenRX(ind As Integer, localPort As Integer)
        Try
            Try
                If _listenerRX(ind) IsNot Nothing Then _listenerRX(ind).Stop()
            Catch
            End Try
            Dim listener As New TcpListener(IPAddress.Any, localPort)
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, True)
            _listenerRX(ind) = listener
            listener.Start()
            AccettaRXAsync(listener, ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenRX port " & localPort & ": " & ex.Message)
        End Try
    End Sub

    ' AcceptTcpClientAsync invece di AcceptTcpClient() su Task.Run: vedi il
    ' commento su AccettaMainAsync (sopra).
    Private Async Sub AccettaRXAsync(listener As TcpListener, ind As Integer)
        Try
            _rx(ind) = Await listener.AcceptTcpClientAsync()
            _streamRX(ind) = _rx(ind).GetStream()
            _bufRX(ind) = New Byte(4095) {}
            AvviaRicezioneRX(ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenRX " & ind & ": " & ex.Message)
        Finally
            listener.Stop()
        End Try
    End Sub

    Public Function SendRX(ind As Integer, data As String) As Boolean
        Try
            If _streamRX(ind) Is Nothing OrElse _rx(ind) Is Nothing OrElse Not _rx(ind).Connected Then Return False
            Dim bytes() As Byte = _enc.GetBytes(data)
            _streamRX(ind).Write(bytes, 0, bytes.Length)
            Return True
        Catch ex As Exception
            WriteError(0, "SocketManager.SendRX " & ind & ": " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Punto unico di invio verso un sotto-canale LAN del ricevitore (Mon/
    ''' Phase/Ctrl/Cal/...): manda il pacchetto con SendRX e riarma SEMPRE
    ''' il relativo watchdog (tmrTimeoutMSG1(ind)), sul modello di
    ''' InvioACU (Managment.vb) per il canale ACU. Prima la riarmatura era
    ''' sparsa (e in alcuni punti assente, es. InvioPhase/LanCtrl) in ogni
    ''' chiamante di SendRX: un nuovo punto di invio poteva facilmente
    ''' dimenticarsene. FrmRicevitore.tmrTimeoutMSG1 ora è sempre allocato
    ''' (vedi FrmRicevitore.New), quindi il controllo IsNot Nothing resta
    ''' solo come difesa, non più come condizione normale.
    ''' </summary>
    Public Function InvioRX(ind As Integer, data As String) As Boolean
        Dim ok As Boolean = SendRX(ind, data)
        Try
            If FrmRicevitore.tmrTimeoutMSG1 IsNot Nothing AndAlso
               ind >= 0 AndAlso ind < FrmRicevitore.tmrTimeoutMSG1.Length Then
                FrmRicevitore.tmrTimeoutMSG1(ind).Enabled = False
                FrmRicevitore.tmrTimeoutMSG1(ind).Enabled = True
            End If
        Catch ex As Exception
            WriteError(0, "SocketManager.InvioRX " & ind & ": " & ex.Message)
        End Try
        Return ok
    End Function

    Private Sub AvviaRicezioneRX(ind As Integer)
        Try
            _streamRX(ind).BeginRead(_bufRX(ind), 0, _bufRX(ind).Length,
                AddressOf CallbackRX, ind)
        Catch ex As Exception
            WriteError(0, "AvviaRicezioneRX " & ind & ": " & ex.Message)
        End Try
    End Sub

    Private Sub CallbackRX(ar As IAsyncResult)
        Dim ind As Integer = CInt(ar.AsyncState)
        Try
            Dim bytesRead As Integer = _streamRX(ind).EndRead(ar)
            If bytesRead > 0 Then
                Dim data As String = _enc.GetString(_bufRX(ind), 0, bytesRead)
                If OnRXReceived IsNot Nothing Then OnRXReceived.Invoke(ind, data)
                AvviaRicezioneRX(ind)
            End If
        Catch ex As Exception
            WriteError(0, "CallbackRX " & ind & ": " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Connessione Down Converter GRF (canale 4)
    ' -------------------------------------------------------
    Public Function ConnectDC(ind As Integer, host As String, port As Integer) As Boolean
        Try
            If _dc(ind) IsNot Nothing AndAlso _dc(ind).Connected Then
                _dc(ind).Close()
            End If
            _dc(ind) = New TcpClient()
            Dim connectTask = _dc(ind).ConnectAsync(host, port)
            If connectTask.Wait(2000) AndAlso _dc(ind).Connected Then
                _streamDC(ind) = _dc(ind).GetStream()
                AvviaRicezioneDC(ind)
                Return True
            Else
                ' Vedi commento in Connect (ACU): chiude esplicitamente il
                ' tentativo scaduto invece di lasciarlo vivo in background.
                _dc(ind).Close()
                _dc(ind) = Nothing
            End If
        Catch ex As Exception
            WriteError(0, "SocketManager.ConnectDC " & ind & ": " & ex.Message)
        End Try
        Return False
    End Function

    Public Function IsDCConnected(ind As Integer) As Boolean
        Try
            Dim c As Net.Sockets.TcpClient = _dc(ind)   ' vedi IsRXConnected
            Return c IsNot Nothing AndAlso c.Client IsNot Nothing AndAlso c.Connected
        Catch
            Return False
        End Try
    End Function

    Public Sub ListenDC(ind As Integer, localPort As Integer)
        Try
            Try
                If _listenerDC(ind) IsNot Nothing Then _listenerDC(ind).Stop()
            Catch
            End Try
            Dim listener As New TcpListener(IPAddress.Any, localPort)
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, True)
            _listenerDC(ind) = listener
            listener.Start()
            AccettaDCAsync(listener, ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenDC port " & localPort & ": " & ex.Message)
        End Try
    End Sub

    ' AcceptTcpClientAsync invece di AcceptTcpClient() su Task.Run: vedi il
    ' commento su AccettaMainAsync (sopra).
    Private Async Sub AccettaDCAsync(listener As TcpListener, ind As Integer)
        Try
            _dc(ind) = Await listener.AcceptTcpClientAsync()
            _streamDC(ind) = _dc(ind).GetStream()
            AvviaRicezioneDC(ind)
        Catch ex As Exception
            WriteError(0, "SocketManager.ListenDC " & ind & ": " & ex.Message)
        Finally
            listener.Stop()
        End Try
    End Sub

    Public Function SendDC(ind As Integer, data As String) As Boolean
        Try
            If _streamDC(ind) Is Nothing OrElse _dc(ind) Is Nothing OrElse Not _dc(ind).Connected Then Return False
            Dim bytes() As Byte = _enc.GetBytes(data)
            _streamDC(ind).Write(bytes, 0, bytes.Length)
            Return True
        Catch ex As Exception
            WriteError(0, "SocketManager.SendDC " & ind & ": " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub AvviaRicezioneDC(ind As Integer)
        Try
            _streamDC(ind).BeginRead(_bufDC, 0, _bufDC.Length,
                AddressOf CallbackDC, ind)
        Catch ex As Exception
            WriteError(0, "AvviaRicezioneDC " & ind & ": " & ex.Message)
        End Try
    End Sub

    Private Sub CallbackDC(ar As IAsyncResult)
        Dim ind As Integer = CInt(ar.AsyncState)
        Try
            Dim bytesRead As Integer = _streamDC(ind).EndRead(ar)
            If bytesRead > 0 Then
                Dim data As String = _enc.GetString(_bufDC, 0, bytesRead)
                If OnDCReceived IsNot Nothing Then OnDCReceived.Invoke(ind, data)
                AvviaRicezioneDC(ind)
            End If
        Catch ex As Exception
            WriteError(0, "CallbackDC " & ind & ": " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Chiude un singolo sotto-socket RX (richiamata da
    ' FrmRicezione.SckRX_Close, usata a sua volta da frmRicevitore.vb)
    ' -------------------------------------------------------
    Public Sub CloseRX(ind As Integer)
        Try
            If _rx(ind) IsNot Nothing Then _rx(ind).Close()
            _rx(ind) = Nothing
            _streamRX(ind) = Nothing
        Catch ex As Exception
            WriteError(0, "SocketManager.CloseRX " & ind & ": " & ex.Message)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' Chiude tutti i socket
    ' -------------------------------------------------------
    Public Sub CloseAll()
        Dim i As Integer
        For i = 0 To 4
            If _main(i) IsNot Nothing Then _main(i).Close()
            If _mac(i) IsNot Nothing Then _mac(i).Close()
            If _dc(i) IsNot Nothing Then _dc(i).Close()
        Next
        For i = 0 To 6
            If _rx(i) IsNot Nothing Then _rx(i).Close()
        Next
    End Sub

    ' -------------------------------------------------------
    ' Stato connessione generico per compatibilita' con
    ' le costanti sckConnected/sckConnecting del codice VB6
    ' -------------------------------------------------------
    Public Function GetStateMain(ind As Integer) As Integer
        Try
            If _main(ind) Is Nothing Then Return 0
            If _main(ind).Connected Then Return sckConnected
            Return 6
        Catch
            Return 0
        End Try
    End Function

    Public Function GetStateMac(ind As Integer) As Integer
        Try
            If _mac(ind) Is Nothing Then Return 0
            If _mac(ind).Connected Then Return sckConnected
            Return 6
        Catch
            Return 0
        End Try
    End Function

    Public Function GetStateRX(ind As Integer) As Integer
        Try
            If _rx(ind) Is Nothing Then Return 0
            If _rx(ind).Connected Then Return sckConnected
            Return 6
        Catch
            Return 0
        End Try
    End Function

    Public Function GetStateDC(ind As Integer) As Integer
        Try
            If _dc(ind) Is Nothing Then Return 0
            If _dc(ind).Connected Then Return sckConnected
            Return 6
        Catch
            Return 0
        End Try
    End Function

End Module
