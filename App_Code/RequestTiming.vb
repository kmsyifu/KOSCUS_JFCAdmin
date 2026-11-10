Imports System.Diagnostics

'Where the time of one request went, for the slow request log (App_Data\ErrorLog\SlowLog_yyyyMMdd.txt, written by
'Global.asax at the end of every request that takes SLOW_REQUEST_SECONDS or longer).
'Kept in HttpContext.Items, so every request has its own. Global.asax marks the steps, DBHelper adds the time spent
'connecting to the database and running queries. SqlDataSource lists on the pages do not go through DBHelper:
'their time is in "Other".
Public Class RequestTiming
    'A request that takes this long or longer is written to the slow request log
    Public Const SLOW_REQUEST_SECONDS As Integer = 10

    'When the site (application) last started: set in Application_Start
    Public Shared AppStartTime As DateTime = DateTime.MinValue

    Private Const ITEM_KEY As String = "RequestTiming"

    Public ReadOnly Clock As Stopwatch = Stopwatch.StartNew()
    Public ReadOnly StartTime As DateTime = DateTime.Now
    Public HandlerReadyMs As Long = -1      ' request start -> page ready to run (includes compiling the page)
    Public SessionReadyMs As Long = -1      ' request start -> session loaded (includes waiting for the session lock)
    Public UserEmail As String = ""

    Public ConnectCount As Integer = 0
    Public ConnectMs As Long = 0
    Public SlowestConnectMs As Long = 0
    Public ConnectRetries As Integer = 0    ' lost connection (ORA-03113 etc.) retries in DBHelper

    Public QueryCount As Integer = 0
    Public QueryMs As Long = 0
    Public SlowestQueryMs As Long = 0
    Public SlowestQuerySQL As String = ""

    Public Shared Sub Start(ByVal ctx As HttpContext)
        ctx.Items(ITEM_KEY) = New RequestTiming
    End Sub

    'This request's timing (Nothing outside a web request)
    Public Shared Function Current() As RequestTiming
        Dim ctx As HttpContext = HttpContext.Current
        If ctx Is Nothing Then Return Nothing
        Return TryCast(ctx.Items(ITEM_KEY), RequestTiming)
    End Function

    Public Shared Sub AddConnect(ByVal ms As Long, ByVal retries As Integer)
        Dim t As RequestTiming = Current()
        If t Is Nothing Then Return
        t.ConnectCount += 1
        t.ConnectMs += ms
        t.ConnectRetries += retries
        If ms > t.SlowestConnectMs Then t.SlowestConnectMs = ms
    End Sub

    Public Shared Sub AddQuery(ByVal ms As Long, ByVal sql As String)
        Dim t As RequestTiming = Current()
        If t Is Nothing Then Return
        t.QueryCount += 1
        t.QueryMs += ms
        If ms > t.SlowestQueryMs Then
            t.SlowestQueryMs = ms
            sql = If(sql, "")
            t.SlowestQuerySQL = If(sql.Length > 500, sql.Substring(0, 500) & "...", sql)
        End If
    End Sub

    'The log entry: total time, each part, and which part is the likely cause
    Public Function Report(ByVal request As HttpRequest) As String
        Dim totalMs As Long = Clock.ElapsedMilliseconds
        Dim startMs As Long = Math.Max(0, HandlerReadyMs)
        Dim sessionMs As Long = IIf(SessionReadyMs >= 0 AndAlso HandlerReadyMs >= 0, SessionReadyMs - HandlerReadyMs, 0)
        Dim otherMs As Long = Math.Max(0, totalMs - startMs - sessionMs - ConnectMs - QueryMs)

        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("SLOW REQUEST " & Sec(totalMs) & "   " & request.HttpMethod & " " & request.RawUrl)
        sb.AppendLine("Started: " & StartTime.ToString("HH:mm:ss") & "   User: " & IIf(UserEmail <> "", UserEmail, "(none)") &
                      "   Clicked: " & PostbackSource(request))
        If AppStartTime <> DateTime.MinValue Then
            Dim sinceStart As TimeSpan = StartTime - AppStartTime
            sb.AppendLine("Site started: " & AppStartTime.ToString("yyyy-MM-dd HH:mm:ss") &
                          IIf(sinceStart.TotalMinutes < 5, "   <- " & CInt(sinceStart.TotalSeconds) & " sec before this request (site was started / restarted)", ""))
        End If
        sb.AppendLine("  1. Start + page compile        : " & Sec(startMs))
        sb.AppendLine("  2. Waiting for session         : " & Sec(sessionMs) & "   (another request of the same user still running, or session server slow)")
        sb.AppendLine("  3. DB connect                  : " & Sec(ConnectMs) & "   (" & ConnectCount & " x, slowest " & Sec(SlowestConnectMs) &
                      ", lost connection retries " & ConnectRetries & ")")
        sb.AppendLine("  4. DB queries (DBHelper)       : " & Sec(QueryMs) & "   (" & QueryCount & " x, slowest " & Sec(SlowestQueryMs) & ")")
        If SlowestQuerySQL <> "" Then sb.AppendLine("     Slowest query: " & SlowestQuerySQL.Replace(vbCr, " ").Replace(vbLf, " "))
        sb.AppendLine("  5. Other                       : " & Sec(otherMs) & "   (page code, SqlDataSource lists, email, sending the page)")

        'Biggest part = likely cause
        Dim parts As New List(Of KeyValuePair(Of Long, String)) From {
            New KeyValuePair(Of Long, String)(startMs, "1 - site start / page compile (first request after the site started or files changed)"),
            New KeyValuePair(Of Long, String)(sessionMs, "2 - waited for another request of the same user (session lock)"),
            New KeyValuePair(Of Long, String)(ConnectMs, IIf(ConnectRetries > 0, "3 - dead pooled DB connection (ORA-03113, retried)", "3 - slow DB connect")),
            New KeyValuePair(Of Long, String)(QueryMs, "4 - slow DB query (see slowest query)"),
            New KeyValuePair(Of Long, String)(otherMs, "5 - other (page code / SqlDataSource lists / email)")}
        parts.Sort(Function(a, b) b.Key.CompareTo(a.Key))
        sb.Append("Likely cause: " & parts(0).Value)
        Return sb.ToString()
    End Function

    Private Shared Function Sec(ByVal ms As Long) As String
        Return (ms / 1000.0).ToString("0.0") & " sec"
    End Function

    'What started a postback: the __doPostBack control (link, dropdown, grid command) or an image button; "" for a page open
    Private Shared Function PostbackSource(ByVal request As HttpRequest) As String
        If request.HttpMethod <> "POST" Then Return "(page open)"
        Dim target As String = CString(request.Form("__EVENTTARGET"))
        If target <> "" Then Return target
        For Each key As String In request.Form.AllKeys
            If key IsNot Nothing AndAlso key.EndsWith(".x") Then Return key.Substring(0, key.Length - 2)
        Next
        Return "(button / Enter key)"
    End Function

    Private Shared Function CString(ByVal o As Object) As String
        If o Is Nothing OrElse IsDBNull(o) Then Return ""
        Return o.ToString()
    End Function
End Class
