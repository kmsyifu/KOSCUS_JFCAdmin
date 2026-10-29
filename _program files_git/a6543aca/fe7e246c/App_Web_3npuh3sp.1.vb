#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login_Mobile.master.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","C4C8AD2D94FAA9E5B7C262AFAEB1B1BC306BC84A")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login_Mobile.master.vb",1)
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



#End ExternalSource
