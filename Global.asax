<%@ Application Language="VB" %>
<%@ Import Namespace="Common" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        
        Dim URL As String = Request.Url.ToString.ToLower
        If URL.IndexOf("localhost") > -1 Then Exit Sub
        
        'Using DB As New DBHelper
        'Send email when running time error occurs
        Dim httpUnhandledException As HttpUnhandledException = New HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError())
        'Session is Nothing for errors raised before session state loads (e.g. 404s) - GetUserEmail would then throw
        'and the error email would never be sent
        Dim userEmail As String = If(HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing, CStr(GetUserEmail(Nothing)), "(no session)")
        Dim emailBody As String = "User: " & userEmail & "<br />" & httpUnhandledException.GetHtmlErrorMessage()
        SendMail(SYSTEM_EMAIL, Common.GetSupportEmail(), Nothing, "KOSCUS_JFCAdmin Error", emailBody, Nothing, True, Nothing)
        'End Using
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
    End Sub
</script>