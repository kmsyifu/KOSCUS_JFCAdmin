Imports System.Web
Imports System.Web.SessionState

'JFC admin access check for every request (called from Global.asax Application_PostAcquireRequestState).
'
'Before, only the master pages checked the role, and only on the first load (If IsPostBack = False), so a postback
'from another session type ran admin button code. The .ashx handlers had no check at all: anyone with the forms login
'cookie (set right after the password, before the verification code) could call them.
'
'Guarded: every .aspx / .ashx / .asmx under ~/JFCAdmin/ and ~/Common/, and every .ashx anywhere.
'Not guarded: ~/Login/, ~/Errors/, ~/Default.aspx, static files (images, css, js).
'A guarded handler must have session state (IRequiresSessionState or IReadOnlySessionState), otherwise it is refused.
Public Class AdminAccess

    Private Shared ReadOnly GUARDED_FOLDERS As String() = {"~/jfcadmin/", "~/common/"}
    Private Shared ReadOnly GUARDED_EXTENSIONS As String() = {".aspx", ".ashx", ".asmx"}

    Public Shared Sub CheckRequest(ByVal ctx As HttpContext)
        If ctx Is Nothing Then Return
        Dim path As String = ctx.Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant()
        Dim ext As String = System.IO.Path.GetExtension(path)
        If Array.IndexOf(GUARDED_EXTENSIONS, ext) < 0 Then Return

        Dim isHandler As Boolean = (ext <> ".aspx")
        Dim inGuardedFolder As Boolean = False
        For Each folder As String In GUARDED_FOLDERS
            If path.StartsWith(folder) Then inGuardedFolder = True
        Next
        If Not inGuardedFolder AndAlso ext <> ".ashx" Then Return

        Dim session As HttpSessionState = ctx.Session
        Dim inSession As Boolean = session IsNot Nothing AndAlso session("SYS_SESSION_USERID") IsNot Nothing
        If inSession AndAlso CStr(session("SYS_SESSION_USERTYPE")) = "JFCADMIN" Then Return

        'Refused
        If isHandler Then
            'AJAX callers get a plain 401 (no redirect to an HTML page)
            ctx.Response.Clear()
            ctx.Response.TrySkipIisCustomErrors = True
            ctx.Response.StatusCode = 401
            ctx.Response.ContentType = "text/plain"
            ctx.Response.Write("Not authorized")
        Else
            If session IsNot Nothing AndAlso Not session.IsReadOnly Then
                session("MSG_TYPE") = "ERROR"
                session("MSG") = IIf(inSession, "You have to log in as a JFC administrator to access the page you requested. Please relogin.",
                                                "Your current session has timed out. Please relogin.")
            End If
            ctx.Response.Redirect("~/Login/Login.aspx?ReturnUrl=" & HttpUtility.UrlEncode(ctx.Request.Url.PathAndQuery), False)
        End If
        ctx.ApplicationInstance.CompleteRequest()
    End Sub

End Class
