'Imports Common

Partial Class Errors_Error
    Inherits System.Web.UI.Page

    'reason=busy: a database query was canceled for running too long (set by Global.asax)
    'reason=connection: the database connection was lost (set by Global.asax)
    'reason=dbdown: the database could not be reached (set by Global.asax)
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim busy As Boolean = (Request.QueryString("reason") = "busy")
        Dim connection As Boolean = (Request.QueryString("reason") = "connection")
        Dim dbDown As Boolean = (Request.QueryString("reason") = "dbdown")
        phBusy.Visible = busy
        phConnection.Visible = connection
        phDbDown.Visible = dbDown
        phError.Visible = Not busy AndAlso Not connection AndAlso Not dbDown
    End Sub

    'Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    '    Dim URL As String = Request.Url.ToString.ToLower
    '    If URL.IndexOf("localhost") > -1 Then Exit Sub

    '    Using DB As New DBHelper
    '        'Send email when running time error occurs
    '        Dim httpUnhandledException As HttpUnhandledException = New HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError())
    '        Dim emailBody As String = "User: " & GetUserEmail(Nothing) & "<br />" & httpUnhandledException.GetHtmlErrorMessage()
    '        SendMail(SYSTEM_EMAIL, Common.GetSupportEmail(), Nothing, "KOSCUS_JFCAdmin Error", emailBody, Nothing, True, Nothing)
    '    End Using
    'End Sub
End Class
