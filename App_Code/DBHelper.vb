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
                Return "User Id=" & GetConfig("DBUSER") & ";Password=" & GetConfig("DBPASSWD") & ";Data Source=" & GetConfig("TNSNAME") & ";Min Pool Size=10;Connection Lifetime=20;" 'Pooling=False -> Pooling=True 'Max Pool Size=200;
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
    Private Sub ConnectToOracle()
        Dim cs As String = GetConnectionString()
        Dim retryCount As Integer = 0
        Dim maxRetries As Integer = 2
        Dim delay As Integer = 1000 ' Initial delay in milliseconds

        Do
            Try
                oracon = New OracleConnection(cs)
                oracon.Open()
                Dim a As Integer = GetScalar("SELECT 1 FROM dual")
                If retryCount > 0 Then
                    ExecuteNonQuery("INSERT INTO EVENTLOG(LOG_ID, EVENTDESC, eventArg1, CREATEUSER, CREATEDATE) VALUES (log_seq.NEXTVAL, 'ORA-03113 at Connect', 'KOSCUS_JFCAdmin', 'SYSTEM', SYSDATE)")
                End If
                Exit Do

            Catch ex As System.Data.OracleClient.OracleException
                If ex.Code = 3113 Then ' ORA-03113: end-of-file on communication channel
                    retryCount += 1
                    If retryCount <= maxRetries Then
                        'OracleConnection.ClearPool(New OracleConnection(cs))
                        Me.Close()
                        Threading.Thread.Sleep(delay) ' Exponential backoff
                        delay *= 2
                    Else
                        Throw New Exception("Failed to connect after multiple retries", ex)
                    End If
                Else
                    Throw ' Re-throw unexpected Oracle exceptions
                End If
            Catch ex As Exception
                Throw
            End Try
        Loop While retryCount <= maxRetries
    End Sub

    Public Sub Close()
        Select Case dbtype
            Case DataBaseType.ORACLE
                oracon.Close()
                oracon.Dispose()
                oracon = Nothing
            Case DataBaseType.SQLSERVER
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


    Public Function ExecuteNonQuery(ByVal SQL As String, ByVal ParamArray Args() As Object) As Integer
        Dim cmd As IDbCommand = GetCommand(SQL, Args)
        Return cmd.ExecuteNonQuery()
    End Function


    Public Function GetReader(ByVal SQL As String, ByVal ParamArray Args() As Object) As DataReader
        Select Case dbtype
            Case DataBaseType.ORACLE
                Dim cmd As IDbCommand = GetCommand(SQL, Args)
                Return New DataReader(cmd.ExecuteReader)
            Case DataBaseType.SQLSERVER
                Return New DataReader(GetDataTable(SQL, Args))
        End Select
        Return Nothing
    End Function


    Public Function GetDataTable(ByVal SQL As String, ByVal ParamArray Args() As Object) As DataTable
        Dim dt As New DataTable
        Select Case dbtype
            Case DataBaseType.ORACLE
                Dim da As OracleDataAdapter = New OracleDataAdapter(GetCommand(SQL, Args))
                da.Fill(dt)
            Case DataBaseType.SQLSERVER
                Dim da As SqlDataAdapter = New SqlDataAdapter(GetCommand(SQL, Args))
                da.Fill(dt)
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

        ret = cmd.ExecuteScalar()
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
