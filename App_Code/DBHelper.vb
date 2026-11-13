Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OracleClient
Imports System.Collections.Generic
Imports Microsoft.Win32

Public Enum DBConst
    NULL
    SYSDATE
End Enum


Public Enum DataBaseType
    ORACLE
    SQLSERVER
End Enum


Public Class DBHelper
    Implements IDisposable
    Private dbtype As DataBaseType
    Private oracon As OracleConnection
    Private sqlcon As SqlConnection
    Private oratrans As OracleTransaction
    Private sqltrans As SqlTransaction
    Private procret As Collection


    Public Sub New()
        Select Case GetConfig("DataBaseType")
            Case "ORACLE"
                dbtype = DataBaseType.ORACLE

            Case "SQLSERVER"
                dbtype = DataBaseType.SQLSERVER
            Case Else
                Throw New Exception("DBTYPE IS NOT SET")
        End Select
        Connect()
    End Sub


    Public Sub New(ByVal DataBaseType As DataBaseType)
        dbtype = DataBaseType
        Connect()
    End Sub


    Public Function GetConnectionString() As String
        Select Case dbtype
            Case DataBaseType.ORACLE
                'Return "User Id=" & GetConfig("DBUSER") & ";Password=" & GetConfig("DBPASSWD") & ";Data Source=" & GetConfig("TNSNAME") & ";Min Pool Size=10;Connection Lifetime=20;Pooling=FALSE"
                'Min Pool Size 10 -> 2: fewer idle connections kept all the time (a firewall / the server cuts idle ones -> ORA-03113).
                'Connection Lifetime 20 -> 300 sec: a connection older than this is closed when it goes back to the pool; at 20 sec
                'a busy site logged in to Oracle again every few pages.
                Return "User Id=" & GetConfig("DBUSER") & ";Password=" & GetConfig("DBPASSWD") & ";Data Source=" & GetConfig("TNSNAME") & ";Min Pool Size=2;Connection Lifetime=300;" 'Pooling=False -> Pooling=True 'Max Pool Size=200;
            Case DataBaseType.SQLSERVER
                Return "MultipleActiveResultSets=True;Persist Security Info=True;Pooling=false;Max Pool Size=250;Min Pool Size=5;" & _
                       "server=" & GetConfig("DBServer") & ";database=" & GetConfig("DEFAULTDB") & ";User=" & GetConfig("DBUSER") & ";Password=" & GetConfig("DBPASSWD")
        End Select
        Return ""
    End Function


    Private Sub Connect()
        Select Case dbtype
            Case DataBaseType.ORACLE
                ConnectToOracle()
                'oracon = New OracleConnection(GetConnectionString())
                'oracon.Open()
            Case DataBaseType.SQLSERVER
                sqlcon = New SqlConnection(GetConnectionString())
                sqlcon.Open()
        End Select
    End Sub
    'True while ConnectToOracle runs: its check query is counted as connect time, not as a query (slow request log)
    Private connecting As Boolean = False

    Private Sub ConnectToOracle()
        'Slow request log (RequestTiming): time to get a working connection, including dead connection retries
        Dim clock As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Dim retryCount As Integer = 0
        connecting = True
        Try
            ConnectToOracleRetry(retryCount)
        Finally
            connecting = False
            RequestTiming.AddConnect(clock.ElapsedMilliseconds, retryCount)
        End Try
    End Sub

    Private Sub ConnectToOracleRetry(ByRef retryCount As Integer)
        Dim cs As String = GetConnectionString()
        Dim maxRetries As Integer = 2
        Dim delay As Integer = 1000 ' Initial delay in milliseconds

        Do
            Try
                oracon = New OracleConnection(cs)
                oracon.Open()
                Dim a As Integer = GetScalar("SELECT 1 FROM dual")
                If retryCount > 0 Then
                    'Logging only: a failed insert must not fail the (working) connection
                    Try
                        ExecuteNonQuery("INSERT INTO EVENTLOG(LOG_ID, EVENTDESC, eventArg1, CREATEUSER, CREATEDATE) VALUES (log_seq.NEXTVAL, 'ORA-03113 at Connect', 'KOSCUS_JFCAdmin', 'SYSTEM', SYSDATE)")
                    Catch
                    End Try
                End If
                Exit Do

            Catch ex As Exception
                'Always give this attempt's connection back to the pool. Before, an error after Open (the check query failing,
                'the last ORA-03113 retry, the 60 second query cancel) threw from the constructor without closing it: the
                'caller's Using never got the object, so the connection stayed taken until garbage collection. After a few
                'database hiccups all pooled connections were taken ("Timeout expired ... max pool size was reached").
                Dim oex As OracleException = TryCast(ex, OracleException)
                Dim lost As Boolean = (oex IsNot Nothing AndAlso IsConnectionLostCode(oex.Code)) ' e.g. ORA-03113: end-of-file on communication channel
                If lost Then
                    'The other idle connections in the pool are most likely dead too (cut by the server / network):
                    'drop them all, so neither this retry nor the pages' SqlDataSource controls get one of them
                    Try
                        If oracon IsNot Nothing Then OracleConnection.ClearPool(oracon)
                    Catch
                    End Try
                End If
                DiscardConnection()
                If Not lost Then Throw
                retryCount += 1
                If retryCount > maxRetries Then Throw New Exception("Failed to connect after multiple retries", ex)
                Threading.Thread.Sleep(delay) ' Exponential backoff
                delay *= 2
            End Try
        Loop While retryCount <= maxRetries
    End Sub

    'Oracle errors meaning the connection itself is dead (not a problem with the query):
    'ORA-00028 session killed, ORA-01012 not logged on, ORA-03113 end-of-file on communication channel,
    'ORA-03114 not connected to ORACLE, ORA-03135 connection lost contact, ORA-12571 TNS packet writer failure
    Public Shared Function IsConnectionLostCode(ByVal code As Integer) As Boolean
        Select Case code
            Case 28, 1012, 3113, 3114, 3135, 12571
                Return True
        End Select
        Return False
    End Function

    'True if the error (or any error inside it) is a lost database connection, from DBHelper or from a SqlDataSource
    Public Shared Function IsConnectionLost(ByVal ex As Exception) As Boolean
        While ex IsNot Nothing
            Dim oex As OracleException = TryCast(ex, OracleException)
            If oex IsNot Nothing AndAlso IsConnectionLostCode(oex.Code) Then Return True
            ex = ex.InnerException
        End While
        Return False
    End Function

    'True if the error (or any error inside it) is a failure to open a new connection - the database / listener is down
    'or cannot be reached (not a dead pooled connection, see IsConnectionLost):
    'ORA-12170 connect timeout, ORA-12514 / 12505 listener does not know the service / SID, ORA-12518 / 12519 / 12520
    'listener could not hand off / no handler, ORA-12528 / 12526 instance blocking new connections, ORA-12537 connection
    'closed, ORA-12541 no listener, ORA-12543 destination host unreachable, ORA-12560 protocol adapter error,
    'ORA-01033 initialization or shutdown in progress, ORA-01034 ORACLE not available, ORA-01089 immediate shutdown
    Public Shared Function IsConnectFailed(ByVal ex As Exception) As Boolean
        While ex IsNot Nothing
            Dim oex As OracleException = TryCast(ex, OracleException)
            If oex IsNot Nothing Then
                Select Case oex.Code
                    Case 1033, 1034, 1089, 12170, 12505, 12514, 12518, 12519, 12520, 12526, 12528, 12537, 12541, 12543, 12560
                        Return True
                End Select
            End If
            ex = ex.InnerException
        End While
        Return False
    End Function

    'True if the error (or any error inside it) is "Timeout expired. The timeout period elapsed prior to obtaining a
    'connection from the pool ... max pool size was reached": every pooled connection was in use, e.g. held by requests
    'stuck waiting on the database (a hung connect / query cannot be canceled), so a new page could not get one
    Public Shared Function IsPoolTimeout(ByVal ex As Exception) As Boolean
        While ex IsNot Nothing
            If TypeOf ex Is InvalidOperationException AndAlso
               If(ex.Message, "").IndexOf("connection from the pool", StringComparison.OrdinalIgnoreCase) > -1 Then Return True
            ex = ex.InnerException
        End While
        Return False
    End Function

    'Drops every pooled connection, so the next pages open new ones instead of getting a dead one
    Public Shared Sub ClearConnectionPools()
        Try
            OracleConnection.ClearAllPools()
        Catch
        End Try
    End Sub

    'Closes the connection of a failed connect attempt (never throws), so it goes back to the pool
    Private Sub DiscardConnection()
        If oracon Is Nothing Then Return
        Try
            oracon.Close()
        Catch
        End Try
        Try
            oracon.Dispose()
        Catch
        End Try
        oracon = Nothing
    End Sub

    Public Sub Close()
        Select Case dbtype
            Case DataBaseType.ORACLE
                If oracon Is Nothing Then Return 'already closed
                oracon.Close()
                oracon.Dispose()
                oracon = Nothing
            Case DataBaseType.SQLSERVER
                If sqlcon Is Nothing Then Return 'already closed
                sqlcon.Close()
                sqlcon.Dispose()
                sqlcon = Nothing
        End Select
    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose
        Close()
    End Sub


    Public Shared Function GetConfig(ByVal index As String) As String
        Select Case index.ToUpper
            Case "DATABASETYPE"
                Return "ORACLE"
            Case "TNSNAME"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\WOW6432Node\KMS\KOSCUS")
                'Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\KOSCUS")
                Dim strCon As String = CStr(rKey.GetValue("DBServer"))
                rKey.Close()
                Return strCon
            Case "DBUSER"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\WOW6432Node\KMS\KOSCUS")
                'Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\KOSCUS")
                Dim strUser As String = CStr(rKey.GetValue("DBUser"))
                rKey.Close()
                Return strUser
            Case "DBPASSWD"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\WOW6432Node\KMS\KOSCUS")
                'Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\KOSCUS")
                Dim strPasswd As String = CStr(rKey.GetValue("DBPasswd"))
                rKey.Close()
                Return strPasswd
        End Select
        Throw New Exception("NO CONFIG")
    End Function


    Public Sub BeginTransaction()
        Select Case dbtype
            Case DataBaseType.ORACLE
                oratrans = oracon.BeginTransaction()
            Case DataBaseType.SQLSERVER
                sqltrans = sqlcon.BeginTransaction()
        End Select
    End Sub


    Public Sub CommitTransaction()
        Select Case dbtype
            Case DataBaseType.ORACLE
                oratrans.Commit()
                oratrans = Nothing
            Case DataBaseType.SQLSERVER
                sqltrans.Commit()
                sqltrans = Nothing
        End Select
    End Sub


    Public Sub RollbackTransaction()
        Select Case dbtype
            Case DataBaseType.ORACLE
                oratrans.Rollback()
                oratrans = Nothing
            Case DataBaseType.SQLSERVER
                sqltrans.Rollback()
                sqltrans = Nothing
        End Select
    End Sub


    Public Function GetDBType() As DataBaseType
        Return dbtype
    End Function


    Private Function GetCommand(ByVal SQL As String, ByVal ParamArray Args() As Object) As IDbCommand
        Dim i As Integer

        Select Case dbtype
            Case DataBaseType.ORACLE
                Dim cmd As New OracleCommand(SQL, oracon)
                If oratrans IsNot Nothing Then cmd.Transaction = oratrans
                For Each param As String In GetSQLParam(SQL)
                    cmd.Parameters.Add(param, GetOracleType(Args(i)))
                    cmd.Parameters(param).Value = GetOracleValue(Args(i))
                    i += 1
                Next
                Return cmd
            Case DataBaseType.SQLSERVER
                Dim cmd As New SqlCommand(SQL, sqlcon)
                If sqltrans IsNot Nothing Then cmd.Transaction = sqltrans
                Return cmd
        End Select
        Return Nothing
    End Function


    Private Function getSQLParam(ByVal SQL As String) As String()
        Dim ret As New List(Of String)
        Dim p As Integer

        While SQL.IndexOf(":") >= 0
            'Skip :, if following naming alphabet
            If SQL.IndexOf(":") > 0 Then
                Dim a As String = SQL.Substring(SQL.IndexOf(":") - 1, 1)
                If a >= "A" And a <= "Z" Or a >= "a" And a <= "z" Or a >= "0" And a <= "9" Or a = "_" Then
                    SQL = (SQL & " ").Substring(SQL.IndexOf(":") + 1)
                    Continue While
                End If
            End If

            'Cut off until first ':'
            SQL = SQL.Substring(SQL.IndexOf(":"))
            p = 1
            While p < SQL.Length
                Dim c As String = SQL.Substring(p, 1)
                If c >= "A" And c <= "Z" Or c >= "a" And c <= "z" Or c >= "0" And c <= "9" Or c = "_" Then
                    p += 1
                Else
                    Exit While
                End If
            End While

            ret.Add(SQL.Substring(0, p))
            If SQL.Length > p + 1 Then
                SQL = SQL.Substring(p + 1)
            Else
                SQL = ""
            End If
        End While

        Return ret.ToArray
    End Function


    Private Shared Function GetOracleType(ByVal o As Object) As System.Data.OracleClient.OracleType
        If TypeOf o Is String Then Return OracleType.VarChar
        If TypeOf o Is DateTime Then Return OracleType.DateTime
        Return OracleType.Number
    End Function


    Private Shared Function GetOracleValue(ByVal o As Object) As Object
        Dim a As DateTime

        If o = Nothing Then Return DBNull.Value 'Added by jzhou in case Args in ExecuteNonQuery is nothing

        Select Case GetOracleType(o)
            Case OracleType.VarChar
                If o = "" Then Return DBNull.Value Else Return o
            Case OracleType.DateTime
                If o = a Then Return DBNull.Value Else Return o
            Case Else
                If o = Integer.MinValue OrElse o = Integer.MaxValue OrElse o = Double.MaxValue OrElse o = Double.MinValue OrElse _
                   o = Decimal.MaxValue OrElse o = Decimal.MinValue Then Return DBNull.Value Else Return o
        End Select
    End Function


    'Longest time (seconds) one query may run. A stuck query is canceled so it frees its connection and the
    'user's session, instead of blocking the user's next pages and using up the connection pool.
    Public Const QUERY_TIMEOUT_SECONDS As Integer = 60

    'Runs one database call with the QUERY_TIMEOUT_SECONDS limit.
    'System.Data.OracleClient ignores CommandTimeout (always 0), so a timer calls cmd.Cancel() instead.
    'A canceled query throws DBQueryTimeoutException (the original Oracle error is its InnerException).
    Private Function RunWithTimeout(Of T)(ByVal cmd As IDbCommand, ByVal work As Func(Of T)) As T
        Dim lockObj As New Object
        Dim done As Boolean = False
        Dim timedOut As Boolean = False
        Dim onTimeout As System.Threading.TimerCallback =
            Sub(state As Object)
                SyncLock lockObj
                    If done Then Return
                    timedOut = True
                    Try
                        cmd.Cancel()
                    Catch
                    End Try
                End SyncLock
            End Sub

        Dim clock As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Using tmr As New System.Threading.Timer(onTimeout, Nothing, QUERY_TIMEOUT_SECONDS * 1000, System.Threading.Timeout.Infinite)
            Try
                Return work()
            Catch ex As Exception When timedOut
                Dim sqlText As String = If(cmd.CommandText, "")
                If sqlText.Length > 1000 Then sqlText = sqlText.Substring(0, 1000) & "..."
                Throw New DBQueryTimeoutException("The database query was canceled after " & QUERY_TIMEOUT_SECONDS & " seconds. SQL: " & sqlText, ex)
            Finally
                SyncLock lockObj
                    done = True
                End SyncLock
                'Slow request log (RequestTiming); the connect check query is in the connect time
                If Not connecting Then RequestTiming.AddQuery(clock.ElapsedMilliseconds, cmd.CommandText)
            End Try
        End Using
    End Function


    Public Function ExecuteNonQuery(ByVal SQL As String, ByVal ParamArray Args() As Object) As Integer
        Dim cmd As IDbCommand = GetCommand(SQL, Args)
        Return RunWithTimeout(cmd, Function() cmd.ExecuteNonQuery())
    End Function


    Public Function GetReader(ByVal SQL As String, ByVal ParamArray Args() As Object) As DataReader
        Select Case dbtype
            Case DataBaseType.ORACLE
                Dim cmd As IDbCommand = GetCommand(SQL, Args)
                Return New DataReader(RunWithTimeout(cmd, Function() cmd.ExecuteReader()))
            Case DataBaseType.SQLSERVER
                Return New DataReader(GetDataTable(SQL, Args))
        End Select
        Return Nothing
    End Function


    Public Function GetDataTable(ByVal SQL As String, ByVal ParamArray Args() As Object) As DataTable
        Dim dt As New DataTable
        Select Case dbtype
            Case DataBaseType.ORACLE
                Dim cmd As IDbCommand = GetCommand(SQL, Args)
                Dim da As OracleDataAdapter = New OracleDataAdapter(DirectCast(cmd, OracleCommand))
                RunWithTimeout(cmd, Function() da.Fill(dt))
            Case DataBaseType.SQLSERVER
                Dim cmd As IDbCommand = GetCommand(SQL, Args)
                Dim da As SqlDataAdapter = New SqlDataAdapter(DirectCast(cmd, SqlCommand))
                RunWithTimeout(cmd, Function() da.Fill(dt))
        End Select
        Return dt
    End Function


    Public Function GetDataRow(ByVal SQL As String, ByVal ParamArray Args() As Object) As DataRow
        Dim dt As DataTable = GetDataTable(SQL, Args)
        If dt.Rows.Count > 0 Then Return dt.Rows(0) Else Return Nothing
    End Function


    Public Function GetScalar(ByVal SQL As String, ByVal ParamArray Args() As Object) As Object
        Return GetScalarEx(SQL, DBNull.Value, Nothing, Args)
    End Function


    Public Function GetScalarEx(ByVal SQL As String, ByVal NVL As Object, ByVal NoRecordVal As Object, ByVal ParamArray Args() As Object) As Object
        Dim ret As Object = Nothing
        Dim cmd As IDbCommand = GetCommand(SQL, Args)

        ret = RunWithTimeout(cmd, Function() cmd.ExecuteScalar())
        If ret Is Nothing Then ret = NoRecordVal
        If IsDBNull(ret) Then ret = NVL
        If ret IsNot Nothing AndAlso ret.GetType Is GetType(String) Then ret = RTrim(ret)
        Return ret
    End Function


    Public Function GetMultiScalar(ByVal SQL As String, Optional ByRef p1 As Object = Nothing, Optional ByRef p2 As Object = Nothing, Optional ByRef p3 As Object = Nothing, _
                                Optional ByRef p4 As Object = Nothing, Optional ByRef p5 As Object = Nothing, Optional ByRef p6 As Object = Nothing, Optional ByRef p7 As Object = Nothing) As Boolean
        Dim ret As Boolean
        Dim r As DataReader = GetReader(SQL)
        ret = r.Read()
        If ret = True Then
            If p1 IsNot Nothing Then p1 = r(0)
            If p2 IsNot Nothing Then p2 = r(1)
            If p3 IsNot Nothing Then p3 = r(2)
            If p4 IsNot Nothing Then p4 = r(3)
            If p5 IsNot Nothing Then p5 = r(4)
            If p6 IsNot Nothing Then p6 = r(5)
            If p7 IsNot Nothing Then p7 = r(6)
        End If
        r.close()
        Return ret
    End Function


    Public Function GetSessionID() As String
        Select Case dbtype
            Case DataBaseType.ORACLE
                Return GetScalar("select userenv('SESSIONID') from dual").ToString
            Case DataBaseType.SQLSERVER
                Return GetScalar("select @@spid").ToString
        End Select

        Return ""
    End Function

    Public Function GetSequence(ByVal SequenceName As String, ByVal SequenceLen As Integer, Optional ByVal CreateFlag As Boolean = False) As String
        Dim ret As String
        Dim SQL As String = ""
        Dim i As Integer

        Try
            ret = GetScalar("SELECT var_value FROM param WHERE var_name = 'SEQUENCE' AND var_index = " & SqlEsc(SequenceName) & " FOR UPDATE") 'Added FOR UPDATE to lock the table for other transactions
            If ret <> "" AndAlso Integer.TryParse(ret, i) = True Then
                ret = (i + 1).ToString
                ExecuteNonQuery("UPDATE param SET var_value = " & SqlEsc(ret) & " WHERE var_name = 'SEQUENCE' AND VAR_INDEX = " & SqlEsc(SequenceName))
            Else
                'Create a sequence
                If CreateFlag = True Then
                    If CreateFlag = True Then
                        ExecuteNonQuery("INSERT INTO param (var_name, var_index, var_value) VALUES ('SEQUENCE', " & SqlEsc(SequenceName) & ", '1')")
                        ret = "1"
                    Else
                        Throw New Exception("INTERNAL ERROR: SEQUENCE " & SequenceName & " is not registered")
                    End If
                End If
                Return Nothing
            End If
            'Add prefix 0
            If SequenceLen > ret.Length Then ret = New String("0"c, SequenceLen - ret.Length) & ret
            Return ret
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetColumnType(ByVal TableName As String, ByVal ColumnName As String) As String
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then
            Select Case row(0)("DATA_TYPE").ToString.ToUpper
                Case "CHAR", "VARCHAR", "VARCHAR2", "NCHAR", "NVARCHAR", "NVARCHAR2"
                    Return "CHAR"
                Case "DATE", "DATETIME"
                    Return "DATE"
                Case "NUMBER", "NUMERIC", "DECIMAL"
                    Return "NUMBER"
                Case Else
                    Throw New Exception("Unrealized datatype: " & row(0)("DATA_TYPE"))
            End Select
        End If
        Throw New Exception("GetColumnType error: " & TableName & ", " & ColumnName)
    End Function


    Public Function GetColumnSize(ByVal TableName As String, ByVal ColumnName As String) As Integer
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then Return row(0).Item("DATA_LENGTH")
        Throw New Exception("GetColumnSize error: " & TableName & ", " & ColumnName)
    End Function


    Public Function GetColumnPrecision(ByVal TableName As String, ByVal ColumnName As String) As Integer
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then Return NVL(row(0).Item("DATA_PRECISION"), 20)
        Throw New Exception("GetColumnPrecision error: " & TableName & ", " & ColumnName)
    End Function


    Public Function GetColumnScale(ByVal TableName As String, ByVal ColumnName As String) As Integer
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then Return NVL(row(0).Item("DATA_SCALE"), 10)
        Throw New Exception("GetColumnScale error: " & TableName & ", " & ColumnName)
    End Function



    Public Function GetColumnNullable(ByVal TableName As String, ByVal ColumnName As String) As Boolean
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then Return IIf(row(0).Item("NULLABLE") = "Y", True, False)
        Throw New Exception("GetColumnNullable error: " & TableName & ", " & ColumnName)
    End Function


    Public Function GetColumnDefaultValue(ByVal TableName As String, ByVal ColumnName As String) As Object
        Dim row() As DataRow

        GetColumnInfo(TableName)
        row = DirectCast(HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET"), DataSet).Tables(TableName.ToUpper).Select("COLUMN_NAME='" & ColumnName.ToUpper & "'")
        If row.Length > 0 Then Return row(0).Item("DATA_DEFAULT")
        Throw New Exception("GetColumnDefaultValue error: " & TableName & ", " & ColumnName)
    End Function


    Private Sub GetColumnInfo(ByVal TableName As String)
        Dim SQL As String = ""
        Dim dt As DataTable
        Dim ds As DataSet

        ds = HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET")
        If ds Is Nothing OrElse ds.Tables.Contains(TableName.ToUpper) = False Then
            If ds Is Nothing Then
                ds = New DataSet
                HttpContext.Current.Session("SYS_SESSION_COLUMNINFO_DATASET") = ds
            End If
            Select Case dbtype
                Case DataBaseType.ORACLE
                    SQL = "select column_name, data_type, data_length, data_precision, data_scale, nullable, data_default " & _
                          "from user_tab_columns where table_name = '" & TableName.ToUpper & "'"
                Case DataBaseType.SQLSERVER
                    SQL = "select column_name, data_type, character_maximum_length data_length, numeric_precision data_precision, numeric_scale data_scale, " & _
                          "case is_nullable when 'YES' then 'Y' else 'N' end nullable, column_default data_default " & _
                          "from INFORMATION_SCHEMA.COLUMNS where table_name = '" & TableName.ToUpper & "'"
            End Select
            dt = GetDataTable(SQL)
            dt.TableName = TableName.ToUpper
            ds.Tables.Add(dt)
        End If
    End Sub


    Public Function SqlEsc(ByVal strVal As String, Optional ByVal blnQuote As Boolean = True, Optional ByVal strEscapeLetter As String = "") As String
        strVal = Replace(strVal, "'", "''")
        If strEscapeLetter <> "" Then
            strVal = Replace(strVal, "_", strEscapeLetter & "_")
            strVal = Replace(strVal, "%", strEscapeLetter & "%")
        End If

        If blnQuote Then strVal = "'" & strVal & "'"
        Return strVal
    End Function


    Public Function SqlEsc(ByVal TableName As String, ByVal ColumnName As String, ByVal Value As Object) As String
        If Value Is Nothing Or IsDBNull(Value) = True Then Return "null"
        If TypeOf Value Is DBConst AndAlso Value = DBConst.NULL Then Return "null"
        If TypeOf Value Is DBConst AndAlso Value = DBConst.SYSDATE Then Return "sysdate"

        Select Case GetColumnType(TableName, ColumnName)
            Case "CHAR"
                Return "'" & Value.ToString.Replace("'", "''") & "'"
            Case "NUMBER"
                Return Value.ToString
            Case "DATE"
                Dim d As Date
                If TypeOf Value Is Date Then
                    d = Value
                Else
                    If Date.TryParseExact(Value.ToString, "d", Nothing, System.Globalization.DateTimeStyles.AllowWhiteSpaces, d) = False Then
                        If Date.TryParseExact(Value.ToString, "G", Nothing, System.Globalization.DateTimeStyles.AllowWhiteSpaces, d) = False Then
                            Throw New Exception
                        End If
                    End If
                End If
                Return DateTimeConv(d)
        End Select
        Return ""
    End Function


    Public Function SqlEscKeywords(ByVal ColumnName As String, ByVal strKeywords As String) As String
        Dim key As String, keys() As String = strKeywords.Split(" "c)
        Dim ret As String = " and ("

        If strKeywords.Trim = "" Then Return " "

        For Each key In keys
            If key.Trim <> "" Then
                key = Replace(key.Trim, "_", "|_")
                key = Replace(key, "%", "|%")

                If ret <> " and (" Then ret &= " and "
                ret = ret & "(" & ColumnName & " like '" & key & "%' escape '|' or " & ColumnName & " like '% " & key & "%' escape '|')"
            End If
        Next

        Return ret & ") "
    End Function


    Public Function DateTimeConv(ByVal datVal As DateTime) As String
        Select Case dbtype
            Case DataBaseType.ORACLE
                Return "to_date('" & Format(datVal, "yyyyMMdd HHmmss") & "', 'YYYYMMDD HH24MISS')"
            Case DataBaseType.SQLSERVER
                Return "convert(datetime, '" & Format(datVal, "yyyy-MM-dd HH:mm:ss") & "', 120)"
        End Select
        Return ""
    End Function


    Public Function NVL(ByVal Val As Object, ByVal Val2 As Object) As Object
        If IsDBNull(Val) Then
            Return Val2
        Else
            Return Val
        End If
    End Function

End Class


'Thrown when a query runs longer than DBHelper.QUERY_TIMEOUT_SECONDS and is canceled.
'Global.asax shows the "database is busy" message for it.
Public Class DBQueryTimeoutException
    Inherits Exception
    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class


Public Class DataReader
    Private r As IDataReader
    Private t As DataTable
    Dim rowidx As Integer = -1


    Public Sub New(ByVal reader As IDataReader)
        r = reader
    End Sub


    Public Sub New(ByVal datatable As DataTable)
        t = datatable
    End Sub


    Public Function Read() As Boolean
        If r IsNot Nothing Then Return r.Read()
        If t IsNot Nothing Then
            rowidx += 1
            Return (rowidx <= t.Rows.Count - 1)
        End If
        Return False
    End Function


    Public Sub close()
        If r IsNot Nothing Then r.Close()
    End Sub


    Default Public Overloads ReadOnly Property Item(ByVal ordinal As Integer) As Object
        Get
            If r IsNot Nothing Then
                If r.Item(ordinal).GetType Is GetType(String) Then
                    Return RTrim(r.Item(ordinal))
                Else
                    Return r.Item(ordinal)
                End If
            End If

            If t IsNot Nothing Then
                If rowidx < 0 Then Throw New Exception("Datareader was read before ""read"" is called")
                If t.Rows(rowidx)(ordinal).GetType Is GetType(String) Then
                    Return RTrim(t.Rows(rowidx)(ordinal))
                Else
                    Return t.Rows(rowidx)(ordinal)
                End If
            End If
            Return Nothing
        End Get
    End Property


    Default Public Overloads ReadOnly Property Item(ByVal name As String) As Object
        Get
            If r IsNot Nothing Then
                If r.Item(name).GetType Is GetType(String) Then
                    Return RTrim(r.Item(name))
                Else
                    Return r.Item(name)
                End If

            End If

            If t IsNot Nothing Then
                If rowidx < 0 Then Throw New Exception("Datareader was read before ""read"" is called")
                If t.Rows(rowidx)(name).GetType Is GetType(String) Then
                    Return RTrim(t.Rows(rowidx)(name))
                Else
                    Return t.Rows(rowidx)(name)
                End If
            End If
            Return Nothing
        End Get
    End Property
End Class
