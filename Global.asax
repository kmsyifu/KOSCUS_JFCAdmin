<%@ Application Language="VB" %>
<%@ Import Namespace="Common" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
        RequestTiming.AppStartTime = DateTime.Now   ' slow request log: shows when a slow request came right after a site (re)start
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        
        Dim lastError As Exception = Server.GetLastError()
        If lastError Is Nothing Then Exit Sub

        Dim URL As String = Request.Url.ToString
        'Session is Nothing for errors raised before session state loads (e.g. 404s) - GetUserEmail would then throw
        'and the error email would never be sent
        Dim userEmail As String = "(no session)"
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then userEmail = CStr(GetUserEmail(Nothing))
        Catch
        End Try

        'Always save the error to App_Data\ErrorLog first, so the details are kept even if the email cannot be sent
        WriteErrorLog("ERROR" & vbCrLf & "URL: " & URL & vbCrLf & "User: " & userEmail & vbCrLf & lastError.ToString())

        If URL.ToLower.IndexOf("localhost") > -1 Then Exit Sub

        'Send email when running time error occurs
        Try
            Dim httpUnhandledException As HttpUnhandledException = New HttpUnhandledException(lastError.Message, lastError)
            Dim emailBody As String = "User: " & userEmail & "<br />" & httpUnhandledException.GetHtmlErrorMessage()
            Dim errMsg As String = ""
            'yfu@kmsnet.com also gets the error email on production / beta (on other servers SendMail already sends to yfu)
            Dim result As Integer = SendMail(SYSTEM_EMAIL, Common.GetSupportEmail() & ",yfu@kmsnet.com", Nothing, "KOSCUS_JFCAdmin Error", emailBody, Nothing, True, errMsg)
            If result <> 0 Then WriteErrorLog("ERROR EMAIL NOT SENT (code " & result & "): " & errMsg)
        Catch ex As Exception
            WriteErrorLog("ERROR EMAIL NOT SENT: " & ex.ToString())
        End Try

        'Lost database connection (e.g. ORA-03113: a dead connection from the pool, which also happens in the pages'
        'SqlDataSource controls that don't go through DBHelper): drop all pooled connections, then load the page again once
        '(GET only; the dbretry=1 parameter stops a loop). A postback, or a second failure, shows "connection lost, please try again".
        'AJAX handlers (.ashx, e.g. Common/Suggestion.ashx): answer with a plain error (HTTP 500) so the page's ajax error
        'handler runs. Before, the redirect to the HTML error page came back as "success" and pages put the whole
        'error page into a label (e.g. the customer name in Customer Settings - Add / Register Customer).
        If Request.Path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase) Then
            If DBHelper.IsConnectionLost(lastError) Then DBHelper.ClearConnectionPools()
            Server.ClearError()
            Response.Clear()
            Response.TrySkipIisCustomErrors = True
            Response.StatusCode = 500
            Response.ContentType = "text/plain"
            Response.Write("Server error")
            Context.ApplicationInstance.CompleteRequest()
            Exit Sub
        End If

        If DBHelper.IsConnectionLost(lastError) Then
            DBHelper.ClearConnectionPools()
            Server.ClearError()
            If Request.HttpMethod = "GET" AndAlso Request.QueryString("dbretry") <> "1" Then
                Dim retryURL As String = Request.RawUrl & IIf(Request.RawUrl.Contains("?"), "&", "?") & "dbretry=1"
                WriteErrorLog("DB CONNECTION LOST - pools cleared, page loaded again: " & retryURL)
                Response.Redirect(retryURL, False)
            Else
                WriteErrorLog("DB CONNECTION LOST - pools cleared, user asked to try again")
                Response.Redirect("~/Errors/Error.aspx?reason=connection", False)
            End If
            Context.ApplicationInstance.CompleteRequest()
            Exit Sub
        End If

        'A query canceled for running too long (DBHelper.QUERY_TIMEOUT_SECONDS): show "the database is busy, please try again"
        'instead of the general error message
        If IsDBQueryTimeout(lastError) Then
            Server.ClearError()
            Response.Redirect("~/Errors/Error.aspx?reason=busy", False)
            Context.ApplicationInstance.CompleteRequest()
        End If
    End Sub

    'True if the error (or any error inside it) is a canceled slow query
    Private Function IsDBQueryTimeout(ByVal ex As Exception) As Boolean
        While ex IsNot Nothing
            If TypeOf ex Is DBQueryTimeoutException Then Return True
            ex = ex.InnerException
        End While
        Return False
    End Function

    'Appends a line to App_Data\ErrorLog\ErrorLog_yyyyMMdd.txt (one file per day; App_Data is never served to browsers).
    'Never throws - a logging failure must not hide the original error.
    'filePrefix: "SlowLog" for the slow request log (same folder, its own file)
    Private Sub WriteErrorLog(ByVal text As String, Optional ByVal filePrefix As String = "ErrorLog")
        Try
            Dim folder As String = Server.MapPath("~/App_Data/ErrorLog")
            System.IO.Directory.CreateDirectory(folder)
            Dim file As String = System.IO.Path.Combine(folder, filePrefix & "_" & DateTime.Now.ToString("yyyyMMdd") & ".txt")
            Dim entry As String = "==== " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " ====" & vbCrLf & text & vbCrLf & vbCrLf
            SyncLock GetType(HttpApplication)
                System.IO.File.AppendAllText(file, entry)
            End SyncLock
        Catch
        End Try
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
        'If Request.Path.ToUpper.IndexOf("LOGIN.ASPX") < 0 Then
        'Response.Redirect(Request.ApplicationPath & "/Login/Login.aspx")
        'End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub
    
    'Added by jzhou to avoid click jacking
    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        HttpContext.Current.Response.AddHeader("x-frame-options", "DENY")
        RequestTiming.Start(Context)
    End Sub

    ' ── Slow request log: App_Data\ErrorLog\SlowLog_yyyyMMdd.txt, every request of RequestTiming.SLOW_REQUEST_SECONDS
    '    or longer, with the time of each part (site start / page compile, session wait, DB connect, DB queries, other).
    '    Never throws: the log must not break a page.

    'The page is ready to run (compiled if it changed / the site just started); next the session is loaded
    Sub Application_PostMapRequestHandler(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim t As RequestTiming = RequestTiming.Current()
            If t IsNot Nothing Then t.HandlerReadyMs = t.Clock.ElapsedMilliseconds
        Catch
        End Try
    End Sub

    'Session loaded: waits here while another request of the same user holds the session lock
    Sub Application_PostAcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim t As RequestTiming = RequestTiming.Current()
            If t Is Nothing Then Return
            t.SessionReadyMs = t.Clock.ElapsedMilliseconds
            If Context.Session IsNot Nothing Then t.UserEmail = CString(Context.Session("SYS_SESSION_EMAIL"))
        Catch
        End Try
    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim t As RequestTiming = RequestTiming.Current()
            If t Is Nothing OrElse t.Clock.ElapsedMilliseconds < RequestTiming.SLOW_REQUEST_SECONDS * 1000L Then Return
            WriteErrorLog(t.Report(Request), "SlowLog")
        Catch
        End Try
    End Sub
</script>