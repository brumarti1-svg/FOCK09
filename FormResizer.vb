Imports System.Windows.Forms
Imports System.Runtime.InteropServices

' Ridimensionamento proporzionale dei controlli nelle form.
' Utilizzo:
'   1) In Form_Shown:   FormResizer.Init(Me)
'   2) In Form_Resize:  FormResizer.Resize(Me)

Public Module FormResizer

    ' Usati per sospendere il ridisegno della form durante il riposizionamento
    ' dei controlli: SuspendLayout/ResumeLayout evitano ricalcoli di layout
    ' inutili, ma NON impediscono che ogni singola Top/Left/Height/Width
    ' assegnata scateni comunque un ridisegno immediato di quel controllo,
    ' rendendo visibile il riposizionamento "a scatti" mentre il ciclo
    ' scorre gli elementi. WM_SETREDRAW disattiva la pittura della finestra
    ' per tutta la durata del ciclo: l'utente vede solo lo stato finale.
    Private Const WM_SETREDRAW As Integer = &HB

    <DllImport("user32.dll")>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    ' WM_SETREDRAW su un controllo agisce SOLO sulla sua finestra nativa: i
    ' controlli figli (label, pannelli, pulsanti...) sono ciascuno una
    ' finestra separata con un proprio ciclo di ridisegno indipendente, quindi
    ' sospendere solo la form non basta a fermare il disegno "a scatti" dei
    ' controlli al suo interno. Va applicato ricorsivamente a tutti i figli
    ' che hanno già una Handle creata.
    Private Sub SetRedraw(c As Control, attiva As Boolean)
        If c.IsHandleCreated Then
            SendMessage(c.Handle, WM_SETREDRAW, If(attiva, New IntPtr(1), IntPtr.Zero), IntPtr.Zero)
        End If
        For Each child As Control In c.Controls
            SetRedraw(child, attiva)
        Next
    End Sub

    ' Da chiamare in Form_Load (PRIMA che la form diventi visibile), per le
    ' form che al primo avvio sono già Maximized: senza questo, tra Load e
    ' Shown la finestra fa comunque un primo disegno con il layout di
    ' progettazione (i controlli nelle posizioni non ancora riscalate) prima
    ' che Resize() in Shown li sistemi — quel primo fotogramma "grezzo" resta
    ' visibile un istante. Resize() riattiva il ridisegno da sola alla fine
    ' del suo ciclo, quando i controlli sono già nella posizione definitiva:
    ' non serve una Resume separata.
    Public Sub SuppressRedraw(frm As Form)
        SetRedraw(frm, False)
    End Sub

    ' Riattiva il ridisegno sospeso con SuppressRedraw quando NON si vuole
    ' anche riscalare i controlli con Resize() (es. la form ospite MDI, i cui
    ' controlli non fanno parte dello snapshot di questa form figlia).
    Public Sub ResumeRedraw(frm As Form)
        SetRedraw(frm, True)
        frm.Refresh()
    End Sub

    Private Class ControlState
        Public Ctrl As Control
        Public Top As Integer
        Public Left As Integer
        Public Height As Integer
        Public Width As Integer
        Public FontSize As Single
    End Class

    Private Class Snapshot
        Public BaseW As Integer
        Public BaseH As Integer
        ' Spazio occupato da elementi Dock=Top/Bottom/Left/Right (es.
        ' MenuStrip, StatusStrip) AL MOMENTO DI Init — usato solo come
        ' riferimento per calcolare l'offset dei controlli catturati
        ' (i loro Top/Left sono relativi a QUESTA fascia iniziale).
        ' NON va riusato in Resize() per riposizionare: se il menu va a
        ' capo su più righe la sua altezza vera cambia, e va rimisurata
        ' ogni volta (vedi Resize).
        Public BaseDispTop As Integer
        Public BaseDispLeft As Integer
        Public BaseDispW As Integer
        Public BaseDispH As Integer
        Public Items As New List(Of ControlState)
    End Class

    ' Chiave = istanza Form (stabile anche se l'Handle viene ricreato)
    Private ReadOnly _snaps As New Dictionary(Of Form, Snapshot)

    ' Cattura ricorsiva: aggiunge i controlli con Dock=None e Anchor di default
    ' (Top,Left) — esclusi TabControl/TabPage, gestiti nativamente da Dock=Fill.
    ' IMPORTANTE: Panel e GroupBox vengono catturati come qualsiasi altro
    ' controllo (non esclusi) — le coordinate Top/Left dei loro figli sono
    ' relative al Panel/GroupBox stesso, quindi se il contenitore non viene
    ' riscalato insieme a loro con lo stesso rapporto, i figli finiscono
    ' fuori posto rispetto al contenitore (che resta fermo alla dimensione
    ' originale) invece di restarci proporzionalmente dentro.
    ' Un controllo con Anchor diverso dal default (es. ancorato a destra/in
    ' basso) viene già riposizionato nativamente da WinForms quando la form
    ' cambia dimensione: va lasciato stare (e non si ricorre nei suoi figli),
    ' altrimenti la nostra formula proporzionale si sommerebbe allo
    ' spostamento nativo, mandandolo fuori posto o oltre il bordo.
    Private Const DefaultAnchor As AnchorStyles = AnchorStyles.Top Or AnchorStyles.Left

    Private Sub Capture(container As Control, s As Snapshot)
        For Each c As Control In container.Controls
            ' MdiClient (l'area interna che ospita le finestre figlie MDI) va
            ' esclusa: altrimenti si ricadrebbe nei controlli della finestra
            ' figlia (es. frmRicezione dentro frmMain) e li si ri-scalerebbe
            ' una seconda volta con il rapporto sbagliato (quello della form
            ' genitore), sommandosi al ridimensionamento già fatto dalla
            ' finestra figlia stessa.
            ' ToolStrip (MenuStrip/StatusStrip) va escluso perché ignora
            ' l'altezza che gli si impone (mantiene sempre l'altezza naturale
            ' del suo contenuto): scalarne la sola larghezza farebbe sparire
            ' le voci di menu che non ci stanno più, mentre il resto del
            ' contenuto verrebbe posizionato assumendo un'altezza del menu
            ' che in realtà non cambia mai, finendoci sotto. Va reso
            ' Dock=Top/Bottom nativo (vedi Designer) così WinForms lo gestisce
            ' da solo e DisplayRectangle riflette sempre la sua vera altezza.
            Dim isTabContainer = TypeOf c Is TabControl OrElse TypeOf c Is TabPage OrElse
                                  TypeOf c Is MdiClient OrElse TypeOf c Is ToolStrip
            Dim gestitoDaNoi = c.Dock = DockStyle.None AndAlso Not isTabContainer AndAlso c.Anchor = DefaultAnchor
            If gestitoDaNoi Then
                s.Items.Add(New ControlState With {
                    .Ctrl = c,
                    .Top = c.Top, .Left = c.Left,
                    .Height = c.Height, .Width = c.Width,
                    .FontSize = c.Font.Size
                })
            End If
            ' Ricorre nei container Fill (proporzionali alla form) e in quelli
            ' None che gestiamo noi stessi (Anchor di default). MdiClient è
            ' sempre escluso dalla ricorsione, anche se Dock=Fill: le finestre
            ' figlie MDI si ridimensionano da sole (vedi commento sopra).
            If c.Controls.Count > 0 AndAlso Not (TypeOf c Is MdiClient) AndAlso
               (c.Dock = DockStyle.Fill OrElse gestitoDaNoi) Then
                Capture(c, s)
            End If
        Next
    End Sub

    ' Chiama in Form_Shown.
    ' baseSize: dimensione di progettazione (Me.ClientSize nel Designer). Se
    ' omessa, viene usata la ClientSize attuale — ma per le form con
    ' WindowState=Maximized impostato nel Designer, in Shown la form è GIÀ
    ' massimizzata: usare frm.ClientSize come base catturerebbe per errore la
    ' dimensione dello schermo invece di quella di progettazione, e i
    ' controlli non verrebbero mai riscalati verso l'alto. Per queste form
    ' passare sempre esplicitamente baseSize.
    Public Sub Init(frm As Form, Optional baseSize As Size = Nothing)
        Dim s As New Snapshot()
        If baseSize.Width > 0 AndAlso baseSize.Height > 0 Then
            s.BaseW = baseSize.Width
            s.BaseH = baseSize.Height
        Else
            s.BaseW = frm.ClientSize.Width
            s.BaseH = frm.ClientSize.Height
        End If
        ' L'altezza/larghezza occupata dagli elementi Dock=Top/Bottom/Left/Right
        ' (es. MenuStrip) è fissa e non cambia mai col ridimensionamento: la
        ' misuriamo ora (è la stessa in ogni momento) e la sottraiamo dalla
        ' base, così lo spazio "di progettazione" disponibile per il
        ' contenuto scalabile resta coerente con quello disponibile a runtime.
        Dim dockedW As Integer = frm.ClientSize.Width - frm.DisplayRectangle.Width
        Dim dockedH As Integer = frm.ClientSize.Height - frm.DisplayRectangle.Height
        s.BaseDispTop = frm.DisplayRectangle.Top
        s.BaseDispLeft = frm.DisplayRectangle.Left
        s.BaseDispW = s.BaseW - dockedW
        s.BaseDispH = s.BaseH - dockedH
        Capture(frm, s)
        _snaps(frm) = s
    End Sub

    ' Chiama in Form_Resize.
    Public Sub Resize(frm As Form)
        If frm.WindowState = FormWindowState.Minimized Then Return
        Dim s As Snapshot = Nothing
        If Not _snaps.TryGetValue(frm, s) Then Return
        If s.BaseDispW <= 0 OrElse s.BaseDispH <= 0 Then Return
        ' Rapporto calcolato sull'area di contenuto effettiva (esclusa la
        ' fascia fissa occupata da menu/status bar), non sull'intera ClientSize.
        Dim szX As Double = frm.DisplayRectangle.Width / s.BaseDispW
        Dim szY As Double = frm.DisplayRectangle.Height / s.BaseDispH
        ' Rimisurati ORA, non presi dallo snapshot: se il menu è andato a capo
        ' su più righe la fascia occupata è più alta di quella iniziale.
        Dim curDispTop As Integer = frm.DisplayRectangle.Top
        Dim curDispLeft As Integer = frm.DisplayRectangle.Left
        SetRedraw(frm, False)
        frm.SuspendLayout()
        For Each cs In s.Items
            Dim c = cs.Ctrl
            c.Top    = curDispTop + CInt((cs.Top - s.BaseDispTop) * szY)
            c.Left   = curDispLeft + CInt((cs.Left - s.BaseDispLeft) * szX)
            c.Height = CInt(cs.Height * szY)
            c.Width  = CInt(cs.Width  * szX)
            Dim fs As Single = CSng(cs.FontSize * Math.Min(szX, szY))
            If fs >= 1.0F Then
                Try
                    c.Font = New Font(c.Font.FontFamily, fs)
                Catch
                End Try
            End If
        Next
        frm.ResumeLayout()
        SetRedraw(frm, True)
        frm.Refresh()
    End Sub

End Module
