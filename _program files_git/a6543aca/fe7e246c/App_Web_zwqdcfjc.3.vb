#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\JFCAdmin.master.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","C4ACB13EA97EB4D2E06D79AE0D917217FB42FCAD")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\JFCAdmin.master.vb",1)
Imports System.Data
Imports Common


Partial Class JFCAdmin_JFCAdmin
    Inherits System.Web.UI.MasterPage
    Protected JavaScript2 As String

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Dim kmsSupport As Boolean
        Dim email As String
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
                email = GetUserEmail(DB)
                'Show / hide links on header
                'lnkUser.Text = GetUserEmail(DB) & " (JFC Administrator)"
                lblUser.Text = GetUserEmail(DB)
                'If GetNrUserType(DB, GetUserEmail(DB)) < 2 Then
                '    btnSwitchUserType.Visible = False
                '    lnkSwitchUserType.Visible = False
                'End If

                'Set properties
                kmsSupport = IsKmsSupport(DB, email)
                'Only if the user is KMS Support, show JFC settings in the menu
                'pnlJFCAdmin.Visible = kmsSupport
                'pnlCustAdmin.Visible = kmsSupport
                'pnlPayment.Visible = jfcHoAdmin
                'JFC LA 
                'Dim useApp As Boolean = Common.UseApp(DB, email)
                pnlCustSetting.Visible = True 'useApp
                pnlCustItem.Visible = True 'useApp
                'Yi Fu 12/10/2020
                Dim jfcbranchAdmin As Boolean = IsJfcBranchAdmin(DB, email) OrElse IsJfcHoAdmin(DB, email) OrElse IsKmsSupport(DB, email)
                pnlBranchSetting.Visible = jfcbranchAdmin
                pnlTool.Visible = jfcbranchAdmin


                'Highlight current menu
                Select Case GetCurrentPageName()
                    Case "home.aspx"
                        aHome.Attributes("class") = "menuSelected"
                    Case "orderhistory.aspx"
                        aOrderHistory.Attributes("class") = "menuSelected"
                    Case "custsetting.aspx"
                        aCustSetting.Attributes("class") = "menuSelected"
                    Case "branchsetting.aspx"
                        aBranchSetting.Attributes("class") = "menuSelected"
                    Case "custitem.aspx"
                        aCustItem.Attributes("class") = "menuSelected"
                    Case "tool.aspx"
                        aTool.Attributes("class") = "menuSelected"
                        'Case "deliverysched.aspx"
                        '    aDeliverySched.Attributes("class") = "menuSelected"
                        'Case "jfcadmin.aspx"
                        '    aJFCAdmin.Attributes("class") = "menuSelected"
                        'Case "custadmin.aspx"
                        '    aCustAdmin.Attributes("class") = "menuSelected"
                        'Case "balance.aspx"
                        '    aBalance.Attributes("class") = "menuSelected"
                        'Case "MyAccount.aspx"
                        '    aMyAccount.Attributes("class") = "menuSelected"
                End Select
            End If
        End Using
    End Sub

    Protected Sub lnkMobileSite_Click(sender As Object, e As System.EventArgs) Handles lnkMobileSite.Click
        Using DB As New DBHelper
            Session("SITE_MODE") = "MOBILE"
            DBUpdate.InsertEventLog(DB, "Switched site mode", Session("SITE_MODE"), Request.Browser.Browser & " " & Request.Browser.Version, Nothing, True)
            'Refresh page
            Response.Redirect(Request.RawUrl)
        End Using
    End Sub

    Protected Function GetJavaScript() As String
        Return ""
    End Function

End Class




#End ExternalSource
