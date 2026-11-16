Imports System.Data.SQLite
Imports System.Data

' Wrapper DAO compatibile sopra SQLite.
' Permette di usare OpenRecordset/FindFirst/BOF/EOF/AddNew/Edit/Update
' senza modificare i 50+ call site originali.

Public Class DAODatabase
    Private ReadOnly _conn As SQLiteConnection

    Public Sub New(conn As SQLiteConnection)
        _conn = conn
    End Sub

    Public ReadOnly Property Connection As SQLiteConnection
        Get
            Return _conn
        End Get
    End Property

    Public Function OpenRecordset(sql As String, Optional type As Integer = 2) As DAORecordset
        If Not sql.TrimStart().ToUpper().StartsWith("SELECT") Then
            sql = "SELECT * FROM [" & sql & "]"
        End If
        Return New DAORecordset(_conn, sql)
    End Function
End Class

Public Class DAORecordset
    Private ReadOnly _conn As SQLiteConnection
    Private ReadOnly _baseSql As String
    Private _table As DataTable
    Private _row As Integer
    Private _noMatch As Boolean
    Private _pending As Dictionary(Of String, Object)
    Private _origValues As Dictionary(Of String, Object)
    Private _isNew As Boolean
    ' rowid SQLite per ogni riga di _table: usato in UPDATE/DELETE per evitare
    ' la WHERE clause multi-campo che fallisce silenziosamente quando i valori
    ' originali non corrispondono esattamente (tipo, encoding, ecc.)
    Private _rowids As New List(Of Long)()

    Public Sub New(conn As SQLiteConnection, sql As String)
        _conn = conn
        _baseSql = sql
        Reload()
    End Sub

    Private Sub Reload()
        _rowids.Clear()
        Try
            _table = New DataTable()
            ' Aggiunge rowid alla query (alias __rowid__) per UPDATE/DELETE precisi.
            ' rowid è l'identificatore interno di SQLite, sempre disponibile.
            Dim rowidSql As String = AddRowidToSql(_baseSql)
            Using Cmd As New SQLiteCommand(rowidSql, _conn)
                Using dr As SQLiteDataReader = Cmd.ExecuteReader()
                    ' Indice della colonna __rowid__ (potrebbe non esserci)
                    Dim rowidIdx As Integer = -1
                    Dim colTypes As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                    For i As Integer = 0 To dr.FieldCount - 1
                        Dim n = dr.GetName(i)
                        If n = "__rowid__" Then
                            rowidIdx = i
                        Else
                            _table.Columns.Add(n, GetType(Object))
                            colTypes(n) = dr.GetDataTypeName(i)?.ToUpper()
                        End If
                    Next
                    Do While dr.Read()
                        _rowids.Add(If(rowidIdx >= 0 AndAlso Not dr.IsDBNull(rowidIdx),
                                       dr.GetInt64(rowidIdx), -1L))
                        Dim vals(dr.FieldCount - 1 - If(rowidIdx >= 0, 1, 0)) As Object
                        Dim vi As Integer = 0
                        For i As Integer = 0 To dr.FieldCount - 1
                            If i = rowidIdx Then Continue For
                            Dim ct As String = ""
                            colTypes.TryGetValue(dr.GetName(i), ct)
                            vals(vi) = If(dr.IsDBNull(i), DBNull.Value, ReadCol(dr, i, ct))
                            vi += 1
                        Next
                        _table.Rows.Add(vals)
                    Loop
                End Using
            End Using
            _row = If(_table.Rows.Count > 0, 0, -1)
        Catch
            _table = New DataTable()
            _rowids.Clear()
            _row = -1
        End Try
    End Sub

    ' Aggiunge "rowid AS __rowid__" a query "SELECT * FROM [table]...".
    Private Function AddRowidToSql(sql As String) As String
        Dim t = sql.TrimStart()
        If Not t.ToUpper().StartsWith("SELECT *") Then Return sql
        Dim fromPos = t.ToUpper().IndexOf(" FROM ")
        If fromPos < 0 Then Return sql
        Return "SELECT *, rowid AS __rowid__" & t.Substring(fromPos)
    End Function

    ' Legge il valore corretto dalla colonna, gestendo i casi problematici
    ' di System.Data.SQLite:
    '
    ' REAL → GetValue() in certi scenari restituisce 0 invece del valore
    '         corretto. Si usa GetDouble() esplicitamente che chiama
    '         sqlite3_column_double() direttamente.
    '
    ' INTEGER con valore REAL memorizzato → GetValue() tronca a Int64 (es. 0).
    '         Fix: se GetDouble() differisce, il tipo reale è REAL.
    '
    ' BOOLEAN → GetValue() converte a True/False; GetDouble() bypassa questo.
    Private Function ReadCol(dr As SQLiteDataReader, i As Integer, colType As String) As Object
        ' Per colonne REAL usa GetDouble() che chiama sqlite3_column_double() direttamente,
        ' bypassando eventuali conversioni di tipo intermedie (es. virgola italiana).
        ' Il Try/Catch è necessario: se GetDouble() lancia eccezione (es. VerifyType fallisce
        ' per colonne con tipo dichiarato non standard), senza catch l'eccezione arriva al
        ' Catch esterno di Reload() che resetta _table vuoto → BOF = True → form non si aprono.
        If String.Equals(colType, "REAL", StringComparison.OrdinalIgnoreCase) Then
            Try
                Return dr.GetDouble(i)
            Catch
                Return dr.GetValue(i)
            End Try
        End If
        Return dr.GetValue(i)
    End Function

    Public ReadOnly Property BOF As Boolean
        Get
            Return _table.Rows.Count = 0 OrElse _row < 0
        End Get
    End Property

    Public ReadOnly Property EOF As Boolean
        Get
            Return _table.Rows.Count = 0 OrElse _row >= _table.Rows.Count
        End Get
    End Property

    Public ReadOnly Property NoMatch As Boolean
        Get
            Return _noMatch
        End Get
    End Property

    Default Public Property Item(fieldName As String) As Object
        Get
            If _pending IsNot Nothing AndAlso _pending.ContainsKey(fieldName) Then
                Return _pending(fieldName)
            End If
            If _row >= 0 AndAlso _row < _table.Rows.Count Then
                Dim v = _table.Rows(_row)(fieldName)
                Return If(v Is DBNull.Value, Nothing, v)
            End If
            Return Nothing
        End Get
        Set(value As Object)
            If _pending Is Nothing Then _pending = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            _pending(fieldName) = value
        End Set
    End Property

    Public Sub FindFirst(criteria As String)
        _noMatch = True
        Try
            ' Converte criteri DAO in SQL WHERE: sostituisce #date# con 'date'
            Dim sqlWhere = System.Text.RegularExpressions.Regex.Replace(criteria, "#([^#]*)#", "'$1'")
            ' Converte val(campo) → CAST(campo AS REAL)
            sqlWhere = System.Text.RegularExpressions.Regex.Replace(sqlWhere, "(?i)val\(([^)]+)\)", "CAST($1 AS REAL)")

            ' Ottiene il rowid della riga cercata direttamente da SQLite.
            ' Approccio rowid invece di RowsMatch campo-per-campo: evita il problema
            ' per cui DataAdapter.Fill() usa GetValue() che restituisce 0 per colonne
            ' REAL, rendendo il confronto stringa impossibile con i valori corretti
            ' in _table (letti tramite GetDouble()).
            Dim tbl = TableName()
            Dim lower = _baseSql.ToUpper()
            Dim orderIdx = lower.IndexOf("ORDER BY")
            Dim base = If(orderIdx >= 0, _baseSql.Substring(0, orderIdx).TrimEnd(), _baseSql)

            ' ORDER BY rowid DESC: in caso di righe duplicate per lo stesso criterio
            ' (es. stesso Sat_Name inserito più volte), restituisce sempre la più recente.
            Dim rowidQuery As String
            If lower.Contains("WHERE") Then
                rowidQuery = base & " AND (" & sqlWhere & ") ORDER BY rowid DESC LIMIT 1"
            Else
                rowidQuery = base & " WHERE " & sqlWhere & " ORDER BY rowid DESC LIMIT 1"
            End If
            ' Sostituisce SELECT * con SELECT rowid
            Dim upperRq = rowidQuery.TrimStart().ToUpper()
            Dim fromPos = upperRq.IndexOf(" FROM ")
            If fromPos >= 0 Then
                rowidQuery = "SELECT rowid" & rowidQuery.TrimStart().Substring(fromPos)
            End If

            Dim matchRowid As Long = -1L
            Using Cmd As New SQLiteCommand(rowidQuery, _conn)
                Dim res = Cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                    matchRowid = CLng(res)
                End If
            End Using

            If matchRowid < 0 Then Return  ' Riga non trovata nel database

            ' Cerca il rowid in _rowids (popolato da Reload())
            For i As Integer = 0 To _rowids.Count - 1
                If _rowids(i) = matchRowid Then
                    _row = i
                    _noMatch = False
                    Return
                End If
            Next

            ' Non in cache: ricarica e riprova
            Reload()
            For i As Integer = 0 To _rowids.Count - 1
                If _rowids(i) = matchRowid Then
                    _row = i
                    _noMatch = False
                    Return
                End If
            Next
        Catch
        End Try
    End Sub

    Public Sub MoveNext()
        If _row < _table.Rows.Count Then _row += 1
    End Sub

    Public Sub MoveFirst()
        _row = If(_table.Rows.Count > 0, 0, -1)
    End Sub

    Public Sub MovePrevious()
        If _row > 0 Then _row -= 1
    End Sub

    Public Sub AddNew()
        _isNew = True
        _pending = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        _origValues = Nothing
    End Sub

    Public Sub Edit()
        _isNew = False
        _pending = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        ' Salva valori originali per la WHERE clause dell'UPDATE
        _origValues = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        If _row >= 0 AndAlso _row < _table.Rows.Count Then
            For Each col As DataColumn In _table.Columns
                _origValues(col.ColumnName) = _table.Rows(_row)(col.ColumnName)
            Next
        End If
    End Sub

    Public Sub Update()
        If _pending Is Nothing OrElse _pending.Count = 0 Then Return
        Try
            Dim tbl = TableName()
            If _isNew Then
                Dim cols = String.Join(", ", _pending.Keys.Select(Function(k) "[" & k & "]"))
                Dim parms = String.Join(", ", _pending.Keys.Select(Function(k) "@p_" & k))
                Using Cmd As New SQLiteCommand("INSERT INTO [" & tbl & "] (" & cols & ") VALUES (" & parms & ")", _conn)
                    For Each kv In _pending
                        Cmd.Parameters.AddWithValue("@p_" & kv.Key, If(kv.Value Is Nothing, DBNull.Value, kv.Value))
                    Next
                    Cmd.ExecuteNonQuery()
                End Using
            Else
                Dim setCl = String.Join(", ", _pending.Keys.Select(Function(k) "[" & k & "] = @new_" & k))
                ' Usa rowid per l'UPDATE: è l'identificatore interno di SQLite,
                ' sempre univoco e stabile. Evita la WHERE multi-campo che fallisce
                ' silenziosamente quando tipi o encoding non corrispondono esattamente.
                Dim rowid As Long = If(_row >= 0 AndAlso _row < _rowids.Count, _rowids(_row), -1L)
                Dim sql As String
                If rowid >= 0 Then
                    sql = "UPDATE [" & tbl & "] SET " & setCl & " WHERE rowid = @__rowid__"
                Else
                    Dim whereCl = String.Join(" AND ", _origValues.Keys.Select(Function(k) "[" & k & "] = @old_" & k))
                    sql = "UPDATE [" & tbl & "] SET " & setCl & " WHERE " & whereCl
                End If
                Using Cmd As New SQLiteCommand(sql, _conn)
                    For Each kv In _pending
                        Cmd.Parameters.AddWithValue("@new_" & kv.Key, If(kv.Value Is Nothing, DBNull.Value, kv.Value))
                    Next
                    If rowid >= 0 Then
                        Cmd.Parameters.AddWithValue("@__rowid__", rowid)
                    Else
                        For Each kv In _origValues
                            Cmd.Parameters.AddWithValue("@old_" & kv.Key, If(kv.Value Is DBNull.Value, DBNull.Value, kv.Value))
                        Next
                    End If
                    Cmd.ExecuteNonQuery()
                End Using
            End If
        Catch ex As Exception
            WriteError(0, "DAORecordset.Update: " & ex.Message)
        End Try
        _isNew = False
        _pending = Nothing
        _origValues = Nothing
        Reload()
    End Sub

    Public Sub Delete()
        If _row < 0 OrElse _row >= _table.Rows.Count Then Return
        Try
            Dim tbl = TableName()
            Dim rowid As Long = If(_row < _rowids.Count, _rowids(_row), -1L)
            Dim sql As String
            If rowid >= 0 Then
                sql = "DELETE FROM [" & tbl & "] WHERE rowid = @__rowid__"
            Else
                Dim whereCl = String.Join(" AND ", _table.Columns.Cast(Of DataColumn)().Select(Function(c) "[" & c.ColumnName & "] = @d_" & c.ColumnName))
                sql = "DELETE FROM [" & tbl & "] WHERE " & whereCl
            End If
            Using Cmd As New SQLiteCommand(sql, _conn)
                If rowid >= 0 Then
                    Cmd.Parameters.AddWithValue("@__rowid__", rowid)
                Else
                    For Each col As DataColumn In _table.Columns
                        Cmd.Parameters.AddWithValue("@d_" & col.ColumnName, _table.Rows(_row)(col.ColumnName))
                    Next
                End If
                Cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            WriteError(0, "DAORecordset.Delete: " & ex.Message)
        End Try
        Reload()
    End Sub

    Public Sub Close()
        _table?.Clear()
    End Sub

    Public ReadOnly Property Fields As DAOFields
        Get
            Return New DAOFields(Me, _table)
        End Get
    End Property

    Friend ReadOnly Property TableData As DataTable
        Get
            Return _table
        End Get
    End Property

    Private Function TableName() As String
        Dim sql = _baseSql.Trim()
        Dim fromIdx = sql.ToUpper().IndexOf("FROM ")
        If fromIdx >= 0 Then
            Dim rest = sql.Substring(fromIdx + 5).Trim().Trim("["c, "]"c)
            Dim sp = rest.IndexOf(" "c)
            Return If(sp > 0, rest.Substring(0, sp), rest).Trim("["c, "]"c)
        End If
        Return sql
    End Function
End Class

Public Class DAOFields
    Private ReadOnly _rs As DAORecordset
    Private ReadOnly _table As DataTable

    Public Sub New(rs As DAORecordset, table As DataTable)
        _rs = rs
        _table = table
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            Return If(_table IsNot Nothing, _table.Columns.Count, 0)
        End Get
    End Property

    Default Public ReadOnly Property Item(indexOrName As Object) As DAOField
        Get
            If TypeOf indexOrName Is Integer Then
                Return New DAOField(_rs, _table.Columns(CInt(indexOrName)))
            Else
                Return New DAOField(_rs, _table.Columns(indexOrName.ToString()))
            End If
        End Get
    End Property
End Class

Public Class DAOField
    Private ReadOnly _rs As DAORecordset
    Private ReadOnly _col As DataColumn

    Public Sub New(rs As DAORecordset, col As DataColumn)
        _rs = rs
        _col = col
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return _col.ColumnName
        End Get
    End Property

    Public Property Value As Object
        Get
            Return _rs(_col.ColumnName)
        End Get
        Set(value As Object)
            ' Instrada sulla proprietà Item di DAORecordset (quella che scrive
            ' davvero in _pending): prima di questo fix Value era ReadOnly, quindi
            ' "rs.Fields(...).Value = x" compilava solo per il late binding di
            ' Object e falliva a runtime, silenziosamente, dentro un Try/Catch
            ' vuoto (vedi FrmUrl.vb) — la spunta "Aggiorna" non veniva mai salvata.
            _rs(_col.ColumnName) = value
        End Set
    End Property
End Class
