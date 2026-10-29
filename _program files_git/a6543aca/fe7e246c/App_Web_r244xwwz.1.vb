#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\Home.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","FDFBF5B41478BA97AC84CD92C7C9B7CFA0B3E667")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\Home.aspx.vb",1)
Imports Common
Imports DBUpdate

Partial Class JFCAdmin_Home
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Dim p As Page = TryCast(Me.Context.Handler, Page)
        If p IsNot Nothing Then
            ' set master page
            If IsMobile() = True Then
                p.MasterPageFile = "JFCAdmin_Mobile.master"
            Else
                p.MasterPageFile = "JFCAdmin.master"
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim kmsSupport As Boolean
        Dim email As String
        'If page is refreshed, show the current page without executing the previous event
        'If IsPostBack = False Then
        '    'First load
        '    Session("POST_ID_" & Page.GetType().Name) = "1001"
        '    ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        'Else
        '    If Common.CString(ViewState("POST_ID_" & Page.GetType().Name)) = Common.CString(Session("POST_ID_" & Page.GetType().Name)) Then
        '        'Postback
        '        Session("POST_ID_" & Page.GetType().Name) = Common.CString(Convert.ToInt16(Session("POST_ID_" & Page.GetType().Name)) + 1)
        '        ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        '    Else
        '        'Refreshed 'Response.Write("refreshed")
        '        ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        '        Response.Redirect(Request.RawUrl)
        '    End If
        'End If

        Using DB As New DBHelper
            If IsPostBack = False Then
                'Init
                DataBind() 'Fill in customer company type
                email = GetUserEmail(DB)
                'Set properties
                kmsSupport = IsKmsSupport(DB, email)
                'pnlLinks.Visible = kmsSupport
                pnlUpdate.Visible = kmsSupport
                'Dim useApp As Boolean = Common.UseApp(DB, email)
                pnlCustSetting.Visible = True
                pnlCustItem.Visible = True
                'Yi Fu 12/10/2020
                Dim jfcbranchAdmin As Boolean = IsJfcBranchAdmin(DB, email) Or IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email)
                pnlBranchsetting.Visible = jfcbranchAdmin
                pnltool.Visible = jfcbranchAdmin

                pnlMenu.Visible = False
                pnlMenu_m.Visible = False
                If IsMobile() = True Then
                    pnlMenu_m.Visible = True
                    'pnlSwitchAccount_m.Visible = IIf(GetNrUserType(DB, GetUserEmail(DB)) < 2, False, True)
                Else
                    pnlMenu.Visible = True
                End If

                'Show available updates
                showAvailUpdate()
            End If
            'showCustList()
        End Using
    End Sub

    'Protected Sub lnkFullSite_Click(sender As Object, e As System.EventArgs) Handles lnkFullSite.Click
    '    Session("SITE_MODE") = "FULL"
    '    'Refresh page
    '    Response.Redirect(Request.RawUrl)
    'End Sub

    Protected Sub btnUpdateCalendar_Click(sender As Object, e As System.EventArgs) Handles btnUpdateCalendar.Click
        Dim nextUpdateYear As String
        Dim nextYear As String

        Using DB As New DBHelper
            nextUpdateYear = GetParam(DB, "UPDATE", "NEXT_UPDATE_DELIVERY_YEAR")
            nextYear = Today.AddYears(1).Year.ToString
            If nextUpdateYear <= nextYear Then
                DB.BeginTransaction()
                'Update delivery
                UpdateDeliveryCelendar(DB, nextUpdateYear, False)
                'Increase NEXT_UPDATE_DELIVERY_YEAR in Param
                DBUpdate.UpdateParam(DB, "UPDATE", "NEXT_UPDATE_DELIVERY_YEAR", (Val(nextUpdateYear) + 1).ToString)
                DB.CommitTransaction()

                'Calendar updated
                showAvailUpdate()
                InsertEventLog(DB, "Saved calendar", nextUpdateYear, Nothing, Nothing, True)
                MessageLine("CONFIRM", "Successfully updated")
            End If
        End Using
    End Sub

    Private Sub showAvailUpdate()
        Dim nextUpdateYear As String
        Dim nextYear As String

        Using DB As New DBHelper
            'Update next year's delivery calendar
            nextUpdateYear = GetParam(DB, "UPDATE", "NEXT_UPDATE_DELIVERY_YEAR")
            nextYear = Today.AddYears(1).Year.ToString
            btnUpdateCalendar.Visible = False
            lblUpdateCmd.Text = "No update is needed"
            lblUpdateCmd.CssClass = "black"
            If nextUpdateYear <= nextYear Then
                btnUpdateCalendar.Visible = IsKmsSupport(DB, GetUserEmail(DB))
                lblUpdateCmd.Text = "The following updates are ready. Please click each available button below to update"
                lblUpdateCmd.CssClass = "red"
            End If
        End Using
    End Sub

End Class

#End ExternalSource
