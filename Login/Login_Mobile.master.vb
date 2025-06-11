Partial Class Login_Login_Mobile
    Inherits System.Web.UI.MasterPage
    'Protected JavaScript2 As String

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
    End Sub

    Protected Sub lnkFullSite_Click(sender As Object, e As System.EventArgs) Handles lnkFullSite.Click
        Session("SITE_MODE") = "FULL"
        'Refresh page
        Response.Redirect(Request.RawUrl)
    End Sub

    Protected Function GetJavaScript() As String
        Return ""
    End Function

End Class

