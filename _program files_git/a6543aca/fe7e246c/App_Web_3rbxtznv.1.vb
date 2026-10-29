#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Errors\Error.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","B7FA0003B20E82597D697513090B882FEC9D75ED")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Errors\Error.aspx.vb",1)
'Imports Common

Partial Class Errors_Error
    Inherits System.Web.UI.Page

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


#End ExternalSource
