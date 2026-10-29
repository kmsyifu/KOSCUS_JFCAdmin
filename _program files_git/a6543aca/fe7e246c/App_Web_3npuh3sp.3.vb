#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login.master.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","6755513A07CD59E1F423CBB8293792F7F988E8C2")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login.master.vb",1)
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



#End ExternalSource
