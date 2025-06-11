Partial Class Login_Login
    Inherits System.Web.UI.MasterPage
    'Protected JavaScript2 As String

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
    End Sub

    Protected Sub lnkMobileSite_Click(sender As Object, e As System.EventArgs) Handles lnkMobileSite.Click
        Session("SITE_MODE") = "MOBILE"
        'Refresh page
        Response.Redirect(Request.RawUrl)
    End Sub

    Protected Function GetJavaScript() As String
        Return ""
    End Function

End Class

