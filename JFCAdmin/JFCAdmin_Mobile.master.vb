Imports System.Data
Imports Common

Partial Class JFCAdmin_JFCAdmin_Mobile
    Inherits System.Web.UI.MasterPage
    Protected JavaScript2 As String

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If IsInSession() = False Then
            'Timeout
            Session("MSG_TYPE") = "ERROR"
            Session("MSG") = "Your current session has timed out. Please relogin."
            Response.Redirect("~/Login/Login.aspx?ReturnUrl=" & Request.Url.PathAndQuery)
            Exit Sub
        End If

        Using DB As New DBHelper
            If IsPostBack = False Then
                'Check error
                If Session("SYS_SESSION_USERTYPE") <> "JFCADMIN" Then
                    Session("MSG_TYPE") = "ERROR"
                    Session("MSG") = "You have to log in as a JFC administrator to access the page you requested. Please relogin."
                    Response.Redirect("~/Login/Login.aspx?ReturnUrl=" & Request.Url.PathAndQuery)
                End If

                ''Not to cache so that user cannot re-enter using back button after logout
                'Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
                'Response.Cache.SetCacheability(HttpCacheability.NoCache)
                'Response.Cache.SetNoStore()
            End If
        End Using
    End Sub

    Protected Sub lnkFullSite_Click(sender As Object, e As System.EventArgs) Handles lnkFullSite.Click
        Using DB As New DBHelper
            Session("SITE_MODE") = "FULL"
            DBUpdate.InsertEventLog(DB, "Switched site mode", Session("SITE_MODE"), Request.Browser.Browser & " " & Request.Browser.Version, Nothing, True)
            'Refresh page
            Response.Redirect(Request.RawUrl)
        End Using
    End Sub

    Protected Function GetJavaScript() As String
        Return ""
    End Function

End Class


