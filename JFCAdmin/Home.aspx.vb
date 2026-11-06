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

        'Only open a DB connection on first load - postbacks don't use it here
        If IsPostBack = False Then
            Using DB As New DBHelper
                'Init
                DataBind() 'Fill in customer company type
                email = GetUserEmail(DB)
                'Set properties
                'Read the user's JFC role once (IsKmsSupport / IsJfcBranchAdmin / IsJfcHoAdmin run the same query)
                Dim jfcRole As String = CString(DB.GetScalar("SELECT userCompany_id FROM webUser_vendor WHERE vendorCompany_id = 'JFC' and email = :1", email))
                kmsSupport = (jfcRole = "KMS_SUPPORT")
                'pnlLinks.Visible = kmsSupport
                pnlUpdate.Visible = kmsSupport
                'Dim useApp As Boolean = Common.UseApp(DB, email)
                pnlCustSetting.Visible = True
                pnlCustItem.Visible = True
                'Yi Fu 12/10/2020
                Dim jfcbranchAdmin As Boolean = (jfcRole = "JFC_BRANCH" OrElse jfcRole = "JFC_HO" OrElse kmsSupport)
                pnlBranchsetting.Visible = jfcbranchAdmin
                pnlOrderForm.Visible = jfcbranchAdmin   'same people as Branch Settings
                pnltool.Visible = jfcbranchAdmin

                pnlMenu.Visible = False
                pnlMenu_m.Visible = False
                If IsMobile() = True Then
                    pnlMenu_m.Visible = True
                    'pnlSwitchAccount_m.Visible = IIf(GetNrUserType(DB, GetUserEmail(DB)) < 2, False, True)
                Else
                    pnlMenu.Visible = True
                End If

                'Show available updates (the update panel is only visible to KMS support, so skip the lookup for everyone else)
                If kmsSupport Then showAvailUpdate(DB, kmsSupport)
            End Using
        End If
        'showCustList()
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
            'Compare as numbers (was a string compare), and do nothing if the param is missing
            If nextUpdateYear <> "" AndAlso Val(nextUpdateYear) <= Val(nextYear) Then
                DB.BeginTransaction()
                'Update delivery
                UpdateDeliveryCelendar(DB, nextUpdateYear, False)
                'Increase NEXT_UPDATE_DELIVERY_YEAR in Param
                DBUpdate.UpdateParam(DB, "UPDATE", "NEXT_UPDATE_DELIVERY_YEAR", (Val(nextUpdateYear) + 1).ToString)
                DB.CommitTransaction()

                'Calendar updated
                showAvailUpdate(DB, IsKmsSupport(DB, GetUserEmail(DB)))
                InsertEventLog(DB, "Saved calendar", nextUpdateYear, Nothing, Nothing, True)
                MessageLine("CONFIRM", "Successfully updated")
            End If
        End Using
    End Sub

    'Uses the caller's DB connection (used to open its own while the caller's was still open)
    Private Sub showAvailUpdate(ByVal DB As DBHelper, ByVal kmsSupport As Boolean)
        Dim nextUpdateYear As String
        Dim nextYear As String

        'Update next year's delivery calendar
        nextUpdateYear = GetParam(DB, "UPDATE", "NEXT_UPDATE_DELIVERY_YEAR")
        nextYear = Today.AddYears(1).Year.ToString
        btnUpdateCalendar.Visible = False
        lblUpdateCmd.Text = "No update is needed"
        lblUpdateCmd.CssClass = "black"
        'Compare as numbers (was a string compare), and treat a missing param as "no update"
        If nextUpdateYear <> "" AndAlso Val(nextUpdateYear) <= Val(nextYear) Then
            btnUpdateCalendar.Visible = kmsSupport
            lblUpdateCmd.Text = "The following updates are ready. Please click each available button below to update"
            lblUpdateCmd.CssClass = "red"
        End If
    End Sub

End Class