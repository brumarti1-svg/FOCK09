Namespace My
    ' Per MyApplication sono disponibili gli eventi seguenti:
    ' Startup: generato all'avvio dell'applicazione, prima della creazione del form di avvio.
    ' Shutdown: generato dopo la chiusura di tutti i form dell'applicazione. Questo evento non viene generato se l'applicazione termina in modo anomalo.
    ' UnhandledException: generato se nell'applicazione si verifica un'eccezione non gestita.
    ' StartupNextInstance: generato all'avvio di un'applicazione a istanza Singola se l'applicazione è già attiva.
    ' NetworkAvailabilityChanged: generato quando la connessione di rete viene connessa o disconnessa.
    Partial Friend Class MyApplication

        ''' <summary>
        ''' Senza questo handler, un'eccezione non gestita su QUALSIASI
        ''' thread (non solo quello UI) termina l'intero processo — osservato
        ''' con Lextm.SharpSnmpLib.Messaging.TimeoutException nonostante le
        ''' chiamate SNMP (frmManager.vb) siano già dentro Try/Catch: la
        ''' libreria usa operazioni di rete che possono sollevare l'eccezione
        ''' su un thread diverso da quello che ha avviato la richiesta,
        ''' fuori dalla portata di quei Try/Catch. e.ExitApplication = False
        ''' registra l'errore e lascia l'app in esecuzione invece di
        ''' chiuderla, coerente con lo spirito "No Response"/retry già usato
        ''' altrove per gli errori SNMP transitori.
        ''' </summary>
        Private Sub MyApplication_UnhandledException(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            Try
                WriteError(0, "UnhandledException: " & e.Exception.GetType().Name & " - " & e.Exception.Message)
            Catch
            End Try
            e.ExitApplication = False
        End Sub
    End Class
End Namespace
