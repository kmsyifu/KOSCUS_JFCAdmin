Imports System.Data
Imports Common
Imports DBUpdate
Imports System.Globalization
Imports System.Threading
Imports System.Resources

Partial Class JFCAdmin_CustSetting
    Inherits System.Web.UI.Page
    Protected JavaScript As String = ""
    Private rm As ResourceManager = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))

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

    Private Sub CheckUserHideColumn()
        If lstHideItemPage.Text = "Hide" Then
            grdUser.Columns(6).Visible = False
        Else
            grdUser.Columns(6).Visible = True
        End If

        If lstHidePaymentPage.Text = "Hide" Then
            grdUser.Columns(7).Visible = False
        Else
            grdUser.Columns(7).Visible = True
        End If
        If lstHidePrice.Text = "Hide" Then
            grdUser.Columns(8).Visible = False
        Else
            grdUser.Columns(8).Visible = True
        End If
        If lstAllowCustOrder.Text = "Allow" Then
            grdUser.Columns(4).Visible = True
        Else
            grdUser.Columns(4).Visible = False
        End If
        If lstAllowChat.Text = "Allow" OrElse lstAllowChat.Text = "Read Only" Then
            grdUser.Columns(5).Visible = True
        Else
            grdUser.Columns(5).Visible = False
        End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim SQL As String
        Dim email As String

        Using DB As New DBHelper
            'Post process for modal dialog box
            If Request("DialogResult") = "YES" Then
                dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"))
            ElseIf Request("DialogResult") = "NO" Then
                dialogResult_no(ViewState("DIALOG_CMD"))
            End If

            '10/22/2021 Postback Default button Gray.
            If IsPostBack = False Then
                'Init
                DataBind() 'Fill in customer company type

                'Show company list
                email = GetUserEmail(DB)
                ShowCustCompanyList(DB, email, False, lstCustCompany, True, Common.LST_CMD_STR)
                If Request.QueryString("CUST_COMPANY") <> "" Then lstCustCompany.Text = Request.QueryString("CUST_COMPANY")
                If lstCustCompany.Items.Count > 1 Then
                    txtJfcBranchNr.Text = Left(lstCustCompany.Items(1).Text, 2)
                End If
                If lstCustCompany.Text = "" Then
                    Alltab.Visible = False
                    lblTitle.Visible = True
                Else
                    lblTitle.Visible = False
                End If

                'Get parameters
                ViewState("CUSTSETTING_TAB") = IIf(Request.QueryString("TAB") <> "", Request.QueryString("TAB"), CustSetting_Tab.CUST)

                'Select tab
                setJavaScript(ViewState("CUSTSETTING_TAB"))

                'Get customer ID
                SQL = "SELECT c.customer_id FROM customer c, webuser_customer w WHERE c.customer_id = w.customer_id AND w.customer_id <> 'ADMIN' AND w.email = :1 AND w.custCompany_id = :2"
                lblCustomerID.Text = CString(DB.GetScalar(SQL, email, lstCustCompany.Text))

                If Not (IsJfcBranchAdmin(DB, email) Or IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email)) Then
                    pnlCustSetting.Enabled = False
                    btnUpdateChnage.Visible = False
                    btnAddCust.Visible = False
                    pnlUser.Enabled = False
                    btnAddUser.Visible = False
                    btnSaveCust.Visible = False
                    btnAddSchedule.Visible = False
                    ViewState("JFC_ADMINGROUP") = False


                Else
                    ViewState("JFC_ADMINGROUP") = True
                End If
                If Not IsKmsSupport(DB, email) Then
                    Dim isLA As Boolean = (Left(lstCustCompany.Text, 2) = "30")
                    pnlChatControls.Visible = isLA
                End If

                '--------------------------------------------------------------------------------------------------------------------
                'Customer info
                '--------------------------------------------------------------------------------------------------------------------
                showCustInfo()
                pnlCalendar.Visible = False

                '--------------------------------------------------------------------------------------------------------------------
                'Delivery Schedule
                '--------------------------------------------------------------------------------------------------------------------
                Common.ShowShipPointList(DB, lblCustomerID.Text, lstShipPoint)

                'Show delivery Schedule
                showScheduleList()

                '--------------------------------------------------------------------------------------------------------------------
                'Order Form
                '--------------------------------------------------------------------------------------------------------------------
                If Common.IsExcepPriceCustomer(DB, lblCustomerID.Text) Then
                    lstAllExcepGrpItem.Enabled = True
                Else
                    lstAllExcepGrpItem.Enabled = False
                End If
            Else
                SetUpdateButton(True)
            End If
        End Using
    End Sub


    Protected Sub lstCustCompany_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany.SelectedIndexChanged
        If lstCustCompany.Text <> "" Then
            lblTitle.Visible = False
            Alltab.Visible = True
        End If

        Response.Redirect("CustSetting.aspx?cust_company=" & lstCustCompany.Text & "&tab=" & ViewState("CUSTSETTING_TAB"))
    End Sub

    Protected Sub btnSearch_order_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchCust.Click
        If lstCustCompany.Items.FindByValue(txtJfcBranchNr.Text & txtJfcCustomerNr.Text) Is Nothing Then
            MessageBox("Portal account for " & txtJfcBranchNr.Text & " - " & txtJfcCustomerNr.Text.ToUpper & " doesn't exist. Please use the ""Register Customer"" button to register.", "Error!")
            Exit Sub
        End If

        Response.Redirect("CustSetting.aspx?cust_company=" & txtJfcBranchNr.Text & txtJfcCustomerNr.Text.ToUpper & "&tab=" & ViewState("CUSTSETTING_TAB"))
    End Sub


    Private Sub setJavaScript(ByVal tab As CustSetting_Tab)
        JavaScript = "$('#tabs').tabs('select', " & convertToLocalTab(tab) & ");"
    End Sub


    Private Function convertToLocalTab(ByVal tab As CustSetting_Tab) As Integer
        Select Case tab
            Case CustSetting_Tab.CUST
                Return 0
            Case CustSetting_Tab.SCHEDULE
                Return 1
            Case CustSetting_Tab.CUSTITEM
                Return 2
        End Select
        Return 0
    End Function


    '====================================================================================================================
    ' CHANGE DETECTION
    '
    ' Each editable area gets a "signature" string built from its current control values. The signature at load time is
    ' stashed in ViewState. On save, anything whose signature still matches is skipped entirely - no DB round trip.
    '
    ' Snapshots are stored as flat text (email<TAB>signature<LF>...) rather than a Dictionary, because Dictionary in
    ' ViewState forces slow BinaryFormatter serialization and bloats the page.
    '====================================================================================================================


    ' Safe accessors. NOTE: setting Columns(n).Visible = False after DataBind() only suppresses rendering -
    ' it does NOT remove the cell/control from the tree, so FindControl still succeeds. Callers that care
    ' whether a column is actually shown must check grdUser.Columns(n).Visible explicitly (see GetUserSignature).
    Private Function DdlVal(row As GridViewRow, id As String) As String
        Dim c = TryCast(row.FindControl(id), DropDownList)
        Return If(c Is Nothing, "", c.SelectedValue)
    End Function

    Private Function TxtVal(row As GridViewRow, id As String) As String
        Dim c = TryCast(row.FindControl(id), TextBox)
        Return If(c Is Nothing, "", c.Text.Trim())
    End Function

    Private Function LblVal(row As GridViewRow, id As String) As String
        Dim c = TryCast(row.FindControl(id), Label)
        Return If(c Is Nothing, "", c.Text)
    End Function

    Private Function ChkVal(row As GridViewRow, id As String) As Boolean
        Dim c = TryCast(row.FindControl(id), CheckBox)
        Return (c IsNot Nothing AndAlso c.Checked)
    End Function

    Private Function GetSettingsSignature() As String
        Return String.Join("|", New String() {
            lstAllowCustOrder.SelectedValue, lstHideItemPage.SelectedValue,
            lstHidePaymentPage.SelectedValue, lstHidePrice.SelectedValue,
            lstHideHomePage.SelectedValue, lstAllowChat.SelectedValue,
            lstDeliveryAnytime.SelectedValue, lstOutofStock.SelectedValue,
            lstAllExcepGrpItem.SelectedValue, txtTextPhones.Text.Trim(),
            txtShipvia.Text.Trim(), txtShipDateOffset.Text.Trim(),
            txtMinOrderAmt.Text.Trim()})
    End Function
    Private Function GetUserSignature(row As GridViewRow) As String
        Return String.Join("|", New String() {
        TxtVal(row, "txtCell"), TxtVal(row, "txtfirstName"), TxtVal(row, "txtlastName"),
        If(grdUser.Columns(4).Visible, DdlVal(row, "lstHideCustOrder"), ""),
        If(grdUser.Columns(6).Visible, DdlVal(row, "lstHideItemPage"), ""),
        If(grdUser.Columns(7).Visible, DdlVal(row, "lstHidePaymentPage"), ""),
        If(grdUser.Columns(8).Visible, DdlVal(row, "lstHidePrice"), ""),
        If(grdUser.Columns(5).Visible, DdlVal(row, "lstHideChat"), "")})
    End Function
    Private Function GetUserOldSignature(row As GridViewRow) As String
        Return String.Join("|", New String() {
        TxtVal(row, "txtCell"), TxtVal(row, "txtfirstName"), TxtVal(row, "txtlastName"),
        If(grdUser.Columns(4).Visible, DdlVal(row, "lstHideCustOrder"), ""),
        If(grdUser.Columns(6).Visible, DdlVal(row, "lstHideItemPage"), ""),
        If(grdUser.Columns(7).Visible, DdlVal(row, "lstHidePaymentPage"), ""),
        If(grdUser.Columns(8).Visible, DdlVal(row, "lstHidePrice"), ""),
        DdlVal(row, "lstHideChat")})
    End Function


    Private Function GetScheduleSignature(row As GridViewRow) As String
        Return DdlVal(row, "lstDeliveryMaster") & "|" &
               row.Cells(GetColumnIndexByName(row, "FREQUENCY")).Text & "|" &
               row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text & "|" &
               row.Cells(GetColumnIndexByName(row, "orderByDayTime")).Text & "|" &
               If(ChkVal(row, "ChkSendReminder"), "Y", "N") & "|" &
               If(ChkVal(row, "ckbEmail"), "Y", "N")
    End Function

    Private Sub SnapshotSettings()
        ViewState("SNAP_SETTING") = GetSettingsSignature()
    End Sub

    Private Sub SnapshotUsers()
        Dim sb As New System.Text.StringBuilder()
        For Each row As GridViewRow In grdUser.Rows
            If row.RowType = DataControlRowType.DataRow Then
                sb.Append(LblVal(row, "lblEmail")).Append(ControlChars.Tab) _
                  .Append(GetUserOldSignature(row)).Append(ControlChars.Lf)
            End If
        Next
        ViewState("SNAP_USER") = sb.ToString()
    End Sub

    Private Sub SnapshotSchedule()
        Dim sb As New System.Text.StringBuilder()
        For Each row As GridViewRow In grdActiveSchedule.Rows
            If row.RowType = DataControlRowType.DataRow Then
                Dim lnk = TryCast(row.FindControl("lnkdeliveryMasterName"), LinkButton)
                If lnk IsNot Nothing AndAlso lnk.CommandArgument <> "" Then
                    sb.Append(lnk.CommandArgument).Append(ControlChars.Tab) _
                      .Append(GetScheduleSignature(row)).Append(ControlChars.Lf)
                End If
            End If
        Next
        ViewState("SNAP_SCHED") = sb.ToString()
    End Sub

    ' Returns the stored signature for a key, or Nothing if there is no snapshot for it (= treat as changed).
    Private Function GetSnap(ByVal key As String, ByVal viewStateKey As String) As String
        Dim raw = CStr(ViewState(viewStateKey))
        If String.IsNullOrEmpty(raw) OrElse String.IsNullOrEmpty(key) Then Return Nothing
        For Each line As String In raw.Split(ControlChars.Lf)
            Dim p = line.Split(ControlChars.Tab)
            If p.Length = 2 AndAlso p(0) = key Then Return p(1)
        Next
        Return Nothing
    End Function


    '--------------------------------------------------------------------------------------------------------------------
    'Save - only writes the parts that actually changed
    '--------------------------------------------------------------------------------------------------------------------
    Protected Sub btnSaveCust_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveCust.Click, btnUpdateChnage.Click, btnUpdateSchedule.Click

        'Validate schedule masters first (cheap, no DB)
        For Each row As GridViewRow In grdActiveSchedule.Rows
            If row.RowType = DataControlRowType.DataRow Then
                If CType(row.FindControl("lstDeliveryMaster"), DropDownList).Text = "" Then
                    MessageBox("Please select delivery schedule master", "Error")
                    Exit Sub
                End If
            End If
        Next

        Dim customerID As String = lblCustomerID.Text
        Dim settingsChanged As Boolean = (GetSettingsSignature() <> CStr(ViewState("SNAP_SETTING")))
        Dim anySaved As Boolean = False
        Dim oldChatEnabled As Boolean
        Dim newChatEnabled As Boolean
        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.CUST)

            '--- Customer settings --------------------------------------------------------------------------------------
            If settingsChanged Then
                anySaved = True

                Dim sql As String = "select count(*) from customer_deliveryDate WHERE CUSTOMER_ID = :1 AND deliveryDate >= TRUNC(SYSDATE)"
                If lstDeliveryAnytime.Text = "Yes" AndAlso Val(DB.GetScalar(sql, customerID)) > 0 Then
                    ViewState("DIALOG_CMD") = "DELETE_ALLSCHEDULE"
                    ViewState("DIALOG_ARG1") = ""
                    MessageBoxYesNo("Your Delivery Anytime is set as Yes. Do you want to delete the delivery schedule? If not, your Delivery Anytime will be set as No.", "Confirm Deletion", DB)
                End If

                Dim oldAllowChat As String = CString(DB.GetScalar(
                    "SELECT NVL(AllowChat,'N') FROM customer WHERE customer_id = :1", customerID))

                Dim AllowChat As String
                Select Case CString(lstAllowChat.SelectedValue)
                    Case "Read Only"
                        AllowChat = "R"
                    Case "Allow"
                        AllowChat = "Y"
                    Case Else
                        AllowChat = ""
                End Select

                Dim AllowCustOrder = IIf(CString(lstAllowCustOrder.SelectedValue) = "Allow", "Y", "")
                Dim HideItemPage = IIf(CString(lstHideItemPage.SelectedValue) = "Hide", "Y", "")
                Dim HidePaymentPage = IIf(CString(lstHidePaymentPage.SelectedValue) = "Hide", "Y", "")
                Dim HidePrice = IIf(CString(lstHidePrice.SelectedValue) = "Hide", "Y", "")
                Dim HideHomePage = IIf(CString(lstHideHomePage.SelectedValue) = "Hide", "Y", "")
                Dim anytimedelivery = IIf(CString(lstDeliveryAnytime.SelectedValue) = "Yes", "Y", "")
                Dim OutofStock As String = IIf(CString(lstOutofStock.SelectedValue) = "Show", "Y", "")
                Dim ShowAllExcepGrpItem As String = IIf(CString(lstAllExcepGrpItem.SelectedValue) = "Yes", "Y", "")
                Dim Textphones = txtTextPhones.Text.TrimEnd(",")
                Dim Shipvia = CString(txtShipvia.Text.Trim())
                Dim Shipdateoffset As Decimal = Val(txtShipDateOffset.Text)
                Dim minOrderAmt As Integer = Val(txtMinOrderAmt.Text)

                UpdateCustSetting(DB, customerID, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice,
                                  anytimedelivery, Textphones, OutofStock, ShowAllExcepGrpItem, Shipvia,
                                  Shipdateoffset, minOrderAmt, HideHomePage, AllowChat)

                Dim loginUser As String = GetUserEmail(DB)
                oldChatEnabled = (oldAllowChat = "Y" OrElse oldAllowChat = "R")
                newChatEnabled = (AllowChat = "Y" OrElse AllowChat = "R")

                If Not oldChatEnabled AndAlso newChatEnabled Then
                    InsertChatRoomAdd(DB, customerID, lblJfcBranchNr.Text, lblJfcCustomerNr.Text, lblCustomerName.Text, loginUser)
                ElseIf oldChatEnabled AndAlso Not newChatEnabled Then
                    InsertChatRoomDel(DB, customerID, lblJfcBranchNr.Text, lblJfcCustomerNr.Text, lblCustomerName.Text, loginUser)
                End If
            End If

            '--- Users: only changed rows -------------------------------------------------------------------------------
            Dim loginEmail As String = Nothing
            Dim custHideItemPage As Boolean = (lstHideItemPage.SelectedValue = "Hide")
            Dim custHidePrice As Boolean = (lstHidePrice.SelectedValue = "Hide")
            Dim custHidePayment As Boolean = (lstHidePaymentPage.SelectedValue = "Hide")

            For Each row As GridViewRow In grdUser.Rows
                If row.RowType <> DataControlRowType.DataRow Then Continue For

                Dim email = LblVal(row, "lblEmail")
                Dim sig = GetUserSignature(row)
                Dim oldSig = GetSnap(email, "SNAP_USER")

                'Untouched row - skip the DB write entirely
                If oldSig IsNot Nothing AndAlso oldSig = sig AndAlso oldChatEnabled = newChatEnabled Then Continue For

                anySaved = True
                If loginEmail Is Nothing Then loginEmail = GetUserEmail(DB)

                Dim hideCustOrder = IIf(DdlVal(row, "lstHideCustOrder") = "Y", "Y", "")
                Dim hideItemPage = If(custHideItemPage, Nothing, IIf(DdlVal(row, "lstHideItemPage") = "Y", "Y", ""))
                Dim hidePrice = If(custHidePrice, Nothing, IIf(DdlVal(row, "lstHidePrice") = "Y", "Y", ""))
                Dim hidePaymentPage = If(custHidePayment, Nothing, IIf(DdlVal(row, "lstHidePaymentPage") = "Y", "Y", ""))
                Dim cell = TxtVal(row, "txtCell")
                Dim firstName = TxtVal(row, "txtfirstName")
                Dim lastName = TxtVal(row, "txtlastName")
                Dim hideChat = IIf(DdlVal(row, "lstHideChat") = "Y" Or Not grdUser.Columns(5).Visible, "Y", "")
                'Pull old firstName/lastName out of the snapshot signature (fields: cell|firstName|lastName|...)
                Dim oldFirstName As String = ""
                Dim oldLastName As String = ""
                Dim oldHideChat As String = ""
                If oldSig IsNot Nothing Then
                    Dim oldFields = oldSig.Split("|"c)
                    If oldFields.Length > 2 Then
                        oldFirstName = oldFields(1)
                        oldLastName = oldFields(2)
                        oldHideChat = oldFields(7)
                    End If
                End If
                Dim nameChanged As Boolean = (oldFirstName <> firstName OrElse oldLastName <> lastName)

                oldHideChat = IIf(oldHideChat = "Y", "Y", "")

                UpdateUserInList(DB, email, customerID, cell, firstName, lastName,
                                 hideCustOrder, hideItemPage, hidePaymentPage, hidePrice, hideChat)

                If hideChat = "Y" Or (Not newChatEnabled And oldChatEnabled <> newChatEnabled) Then
                    InsertChatWebuserCmd(DB, email, customerID, lblJfcBranchNr.Text, lblJfcCustomerNr.Text, "DEL", "CUST", firstName, lastName, loginEmail)
                ElseIf oldHideChat <> hideChat And hideChat = "" Or (newChatEnabled And oldChatEnabled <> newChatEnabled) Then
                    InsertChatWebuserCmd(DB, email, customerID, lblJfcBranchNr.Text, lblJfcCustomerNr.Text, "ADD", "CUST", firstName, lastName, loginEmail)
                ElseIf nameChanged And hideChat = "" Then
                    InsertChatWebuserCmd(DB, email, "", "", "", "UPD", "CUST", firstName, lastName, loginEmail)
                End If
            Next
        End Using

        '--- Schedule: only changed rows --------------------------------------------------------------------------------
        Dim schedChanged As Boolean = False
        For Each row As GridViewRow In grdActiveSchedule.Rows
            If row.RowType <> DataControlRowType.DataRow Then Continue For

            Dim lnk As LinkButton = CType(row.FindControl("lnkdeliveryMasterName"), LinkButton)
            Dim key = lnk.CommandArgument
            Dim sig = GetScheduleSignature(row)
            Dim oldSig = GetSnap(key, "SNAP_SCHED")

            'Untouched existing row - skip. Rows with no key are newly added, so always save them.
            If oldSig IsNot Nothing AndAlso oldSig = sig Then Continue For

            schedChanged = True
            anySaved = True

            SaveSchedule(key,
                         CType(row.FindControl("lstDeliveryMaster"), DropDownList),
                         row.Cells(GetColumnIndexByName(row, "FREQUENCY")).Text,
                         row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text,
                         row.Cells(GetColumnIndexByName(row, "orderByDayTime")).Text,
                         ChkVal(row, "ChkSendReminder"),
                         ChkVal(row, "ckbEmail"))
        Next

        'Rebind the schedule grid ONCE, not once per row (showScheduleList also re-snapshots)
        If schedChanged Then showScheduleList()

        MessageLine("CONFIRM", If(anySaved, "Successfully saved", "No changes to save"))
        SetUpdateButton(False)

        'Refresh snapshots so a second click of Update doesn't save everything again
        SnapshotSettings()
        SnapshotUsers()
        If Not schedChanged Then SnapshotSchedule()
    End Sub


    Protected Sub lstHide_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstHideItemPage.SelectedIndexChanged, lstHidePaymentPage.SelectedIndexChanged, lstHidePrice.SelectedIndexChanged, lstAllowCustOrder.SelectedIndexChanged, lstAllowChat.SelectedIndexChanged

        Using DB As New DBHelper
            Dim customerID As String = lblCustomerID.Text
            'Show user list for the customer
            Dim Sql = "SELECT u.email,u.CELLULAR,u.firstName,u.lastName, c.hideCustOrder, c.hideItemPage, c.hidePaymentPage, c.hidePrice, c.hideChat " &
                       "FROM webuser u, webuser_customer c " &
                       "LEFT OUTER JOIN UserRemovalRequest ur ON ur.email = c.email AND ur.jfcBranchNr || ur.jfcCustomerNr = c.custCompany_id " &
                       " WHERE u.email = c.email " &
                       " AND c.userCompany_id = c.custCompany_id " &
                       " AND c.customer_id = :1 and (ur.status <> 'N' or ur.status is null) " &
                       "ORDER BY email"
            Dim dt = DB.GetDataTable(Sql, customerID)
            'Only when lstAllowChat itself changed AND its new value is Allow or Read Only:
            'default every user's hideChat to "" (not hidden) in-memory, overriding the DB value.
            'For Not Allow, or any other control triggering this handler, hideChat stays as-is from the DB.
            If sender Is lstAllowChat AndAlso (lstAllowChat.Text = "Allow" OrElse lstAllowChat.Text = "Read Only") Then
                For Each dr As DataRow In dt.Rows
                    dr("hideChat") = ""
                Next
            End If
            grdUser.DataSource = dt
            grdUser.DataBind()
        End Using

        CheckUserHideColumn()
        SnapshotUsers()
        'The grid was just rebound from the DB, so any in-grid edits are gone. Re-snapshot to match what is on screen.
        'NOTE: SNAP_SETTING is deliberately NOT refreshed here - changing this dropdown IS a change that must be saved.

    End Sub


    Private Sub SetUpdateButton(isEnable As Boolean)
        If isEnable Then
            btnUpdateChnage.CssClass = "midButton_green"
            btnUpdateChnage.Enabled = True

            btnSaveCust.CssClass = "midButton_green"
            btnSaveCust.Enabled = True

            btnUpdateSchedule.CssClass = "midButton_green"
            btnUpdateSchedule.Enabled = True
        Else
            btnUpdateChnage.CssClass = "midButton_gray"
            btnUpdateChnage.Enabled = False

            btnSaveCust.CssClass = "midButton_gray"
            btnSaveCust.Enabled = False

            btnUpdateSchedule.CssClass = "midButton_gray"
            btnUpdateSchedule.Enabled = False
        End If
    End Sub


    Protected Sub lstDeliveryAnytime_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstDeliveryAnytime.SelectedIndexChanged
        If lstDeliveryAnytime.SelectedValue = "Yes" Then
            pnlSchedule.Visible = False
        Else
            pnlSchedule.Visible = True
        End If
    End Sub


    Protected Sub btnAddUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddUser.Click
        setJavaScript(CustSetting_Tab.CUST)

        lblPopTitleUser.Text = "Add New User"
        txtPopJFCAccount.Text = lstCustCompany.SelectedValue.Substring(0, 2)
        txtPopJFCNum.Text = lstCustCompany.SelectedValue.Substring(2)
        txtPopJFCAccount.Enabled = False
        txtPopJFCNum.Enabled = False
        pnlPopWarningUser.Visible = False
        txtPopEmail.Text = ""
        lblPopWarningUser.Text = ""
        lblPopCustomerName.Text = ""
        If IsProduction() = True OrElse IsProdBeta() = True Then
            Using db As New DBHelper
                Dim Sql = "SELECT c.Name FROM  jho.customer@jho c " &
                   " WHERE c.bid = :1 AND c.customerid = :2"
                Dim r = db.GetReader(Sql, txtPopJFCAccount.Text, txtPopJFCNum.Text)
                If r.Read() Then
                    lblPopCustomerName.Text = CString(r("Name"))
                End If
            End Using
        End If
        popDialog_User.Show()
    End Sub


    Private Sub showCustInfo()
        Dim SQL As String
        Dim r As DataReader
        Dim dt As DataTable
        Dim customerID As String = lblCustomerID.Text

        Using DB As New DBHelper
            'Set Properties
            If IsMobile() Then
                grdShipPoint.Columns(GetColumnIndex(grdShipPoint, "country")).Visible = False 'Country
            Else
                pnlCustInfo.Width = 500
                pnlCustInfo.Height = 450
                pnlCustSetting.Width = 500
                pnlCustSetting.Height = 450
            End If

            Dim autoPayTypeALL = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_ALL")
            Dim autoPayTypeNONLIQ = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_NONLIQ")
            Dim autoPayTypeBEER = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_BEER")
            Dim autoPayTypeWINE = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_WINE")
            Dim autoPayTypeLIQUOR = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_LIQUOR")

            'Show dropdownlist
            ShowCodeMasterListLocal(DB, "ALLOWNOT", lstAllowCustOrder, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHideItemPage, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHidePaymentPage, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHidePrice, True)
            ShowCodeMasterListLocal(DB, "HIDE_N_SHOW_Y", lstOutofStock, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstAllExcepGrpItem, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstDeliveryAnytime, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHideHomePage, True)
            ShowCodeMasterListLocal(DB, "ALLOWNOT_CHAT", lstAllowChat, True)

            'Show customer info and settings
            SQL = "SELECT * FROM customer c left join codemaster m on m.code_id = c.autopaytype AND CODEGRP = 'AUTOPAY_TYPE'  WHERE c.customer_id = :1 "
            r = DB.GetReader(SQL, customerID)
            If r.Read() Then
                'Customer info
                lblCustomerName.Text = CString(r("customerName"))
                lblJfcBranchNr.Text = CString(r("jfcBranchNr"))
                lblJfcCustomerNr.Text = CString(r("jfcCustomerNr"))
                lblPhone.Text = CString(r("phone"))
                lblFax.Text = CString(r("fax"))
                lblAddress1.Text = CString(r("address1"))
                lblAddress2.Text = CString(r("address2"))
                lblCity.Text = CString(r("city"))
                lblState.Text = CString(r("state"))
                lblZip.Text = CString(r("zip"))
                lblCountry.Text = CString(r("country"))
                lblJfcContactName.Text = CString(r("jfcContactName"))
                lblJfcEmail.Text = foldlongString(CString(r("jfcEmail")), New Char() {","})

                Dim autoPaytype As String = CString(r("CODEDESC"))
                Dim NetsDay As String = CString(r("NETDAYS"))
                Dim NetsDay_LIQUOR As String = CString(r("NETDAYS_LIQUOR"))
                lblAutoPayType.Text = String.Empty
                If autoPayTypeALL <> "" Then
                    lblAutoPayType.Text &= String.Format("{0}: {1}, ", "All Invoice", autoPayTypeALL)
                End If
                If autoPayTypeNONLIQ <> "" Then
                    lblAutoPayType.Text &= String.Format("{0}: {1}, ", "Non Liquor Invoice", autoPayTypeNONLIQ)
                End If
                If autoPayTypeBEER <> "" Then
                    lblAutoPayType.Text &= String.Format("{0}: {1}, ", "Beer Invoice", autoPayTypeBEER)
                End If
                If autoPayTypeWINE <> "" Then
                    lblAutoPayType.Text &= String.Format("{0}: {1}, ", "Wine Invoice", autoPayTypeWINE)
                End If
                If autoPayTypeLIQUOR <> "" Then
                    lblAutoPayType.Text &= String.Format("{0}: {1}", "Liquor Invoice", autoPayTypeLIQUOR)
                End If
                If autoPaytype <> "" Then
                    lblAutoPayType.Text &= String.Format(Translate(autoPaytype), NetsDay, If(NetsDay_LIQUOR = "", NetsDay, NetsDay_LIQUOR))
                End If

                lblWebDiscType.Text = CString(r("WebDiscType"))
                lblJfcPhone.Text = foldlongString(CString(r("jfcPhone")), New Char() {","})

                'Customer settings
                lstAllowCustOrder.Text = IIf(CString(r("allowCustOrder")) = "Y", "Allow", "Not Allow")
                Dim allowChatVal As String = CString(r("allowChat"))
                lstAllowChat.Text = If(allowChatVal = "R", "Read Only", If(allowChatVal = "Y", "Allow", "Not Allow"))
                lstHideItemPage.Text = IIf(CString(r("hideItemPage")) = "Y", "Hide", "Show")
                lstHidePaymentPage.Text = IIf(CString(r("hidePaymentPage")) = "Y", "Hide", "Show")
                lstHideHomePage.Text = IIf(CString(r("hideHomePage")) = "Y", "Hide", "Show")
                lstHidePrice.Text = IIf(CString(r("hidePrice")) = "Y", "Hide", "Show")
                lstOutofStock.Text = IIf(CString(r("SHOWOUTOFSTOCK")) = "Y", "Show", "Hide")
                lstAllExcepGrpItem.Text = IIf(CString(r("ShowAllExcepGrpItem")) = "Y", "Yes", "No")
                lstDeliveryAnytime.Text = IIf(CString(r("anytimedelivery")) = "Y", "Yes", "No")

                txtTextPhones.Text = CString(r("TextPhones"))
                txtMinOrderAmt.Text = CString(r("MINORDERAMT"))
                txtShipvia.Text = CString(r("SHIPVIA"))
                txtShipDateOffset.Text = CString(r("ShipDateOffset"))
            End If
            r.close()

            'Show shipping point for the customer
            SQL = "SELECT shipName, address1, address2, city, state, zip, country " &
                       "FROM customer_shipPoint " &
                       "WHERE customer_id = " & DB.SqlEsc(customerID) & " " &
                       "ORDER BY shipName"
            dsShipPoint.ProviderName = "System.Data.OracleClient"
            dsShipPoint.ConnectionString = DB.GetConnectionString
            dsShipPoint.SelectCommand = SQL
            grdShipPoint.DataSourceID = "dsShipPoint"
            grdShipPoint.DataBind()

            'Show user list for the customer
            SQL = "SELECT u.email,u.CELLULAR,u.firstName,u.lastName, c.hideCustOrder, c.hideItemPage, c.hidePaymentPage, c.hidePrice, c.hidechat " &
                       "FROM webuser u, webuser_customer c " &
                       "LEFT OUTER JOIN UserRemovalRequest ur ON ur.email = c.email AND ur.jfcBranchNr || ur.jfcCustomerNr = c.custCompany_id " &
                       " WHERE u.email = c.email " &
                       " AND c.userCompany_id = c.custCompany_id " &
                       " AND c.customer_id = :1 AND (ur.status <> 'N' or ur.status is null) " &
                       "ORDER BY email"
            dt = DB.GetDataTable(SQL, customerID)
            grdUser.DataSource = dt
            grdUser.DataBind()

            CheckUserHideColumn()

            'Baseline for change detection - must be AFTER DataBind and AFTER CheckUserHideColumn,
            'because hidden columns have no controls and would produce a different signature.
            SnapshotSettings()
            SnapshotUsers()

            If lstDeliveryAnytime.SelectedValue = "Yes" Then
                pnlSchedule.Visible = False
            Else
                pnlSchedule.Visible = True
            End If
        End Using
    End Sub


    Protected Sub lstShipPoint_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstShipPoint.SelectedIndexChanged
        showScheduleList()
        SetUpdateButton(False)
    End Sub


    Private MonthData As DataTable

    Protected Sub cldSchedule_DayRender(sender As Object, e As System.Web.UI.WebControls.DayRenderEventArgs) Handles cldSchedule.DayRender
        ' Load data once for the entire month
        If MonthData Is Nothing OrElse MonthData.Rows.Count = 0 Then
            Using DB As New DBHelper
                MonthData = GetMonthlyData(DB, lblCustomerID.Text, lstShipPoint.Text, e.Day.Date)
            End Using
        End If

        ' Render day events from the cached MonthData
        RenderDayEvents(Page, e, lblCustomerID.Text, lstShipPoint.Text, Not ViewState("JFC_ADMINGROUP"), lnkDelivery)
    End Sub


    ' Fetches data for the entire month from the database
    Private Function GetMonthlyData(ByVal DB As DBHelper, ByVal customerID As String, ByVal shipPointID As String, ByVal currentDate As DateTime) As DataTable
        Dim SQL As String = "SELECT WM_CONCAT(' (Order by ' || TO_CHAR(d.orderByDate, :1) ||')') AS vendorList, " &
                            "d.deliveryDate " &
                            "FROM customer_deliveryDate d, custGrp_vendor v " &
                            "WHERE d.custGrpVendor_id = v.custGrpVendor_id " &
                            "  AND d.shipPoint_id = :2" &
                            "  AND d.customer_id = :3" &
                            "  AND d.deliveryDate BETWEEN TO_DATE(:4, :5) AND TO_DATE(:6, :7) " &
                            "GROUP BY d.deliveryDate " &
                            "ORDER BY d.deliveryDate"

        Dim endDate As DateTime = currentDate.AddMonths(2)

        Return DB.GetDataTable(SQL, SHORTDATETIME_DELFMT, shipPointID, customerID, currentDate.ToString(DATE_FMT), DATE_FMT, endDate.ToString(DATE_FMT), DATE_FMT)
    End Function


    Private Sub RenderDayEvents(ByVal page As System.Web.UI.Page, ByRef e As System.Web.UI.WebControls.DayRenderEventArgs, ByVal customerID As String, ByVal shipPointID As String, ByVal viewOnly As Boolean, ByVal lnkDelivery As LinkButton)
        Dim dayEvents = MonthData.Select("deliveryDate = #" & e.Day.Date.ToString("MM/dd/yyyy") & "#")

        For Each row As DataRow In dayEvents
            Dim vendors As String() = row("vendorList").ToString().Split(",")

            For Each vendor In vendors
                If viewOnly OrElse e.Day.IsOtherMonth Then
                    Dim lbl As New Label()
                    lbl.Text = "<br/>Delivery: <br/>" & vendor
                    e.Cell.Controls.Add(lbl)
                Else
                    Dim lnk As New HyperLink()
                    lnk.Text = "<br/>Delivery: <br/>" & vendor
                    lnk.ForeColor = System.Drawing.Color.Navy
                    lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & vendor, True)
                    e.Cell.Font.Bold = True
                    e.Cell.Controls.Add(lnk)
                End If
            Next
        Next

        ' Add an option to create a new delivery if there are no events
        If viewOnly = False AndAlso e.Day.IsOtherMonth = False AndAlso e.Cell.Controls.Count = 1 Then
            e.Cell.Controls.Add(New LiteralControl("<br />"))
            Dim lnk As New HyperLink()
            lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & ",", True)
            lnk.ToolTip = "Add a delivery"

            Dim img As New Image()
            img.ImageUrl = GetWebImgUrl("add.png")
            img.CssClass = "xxsImgBtn"
            img.BorderStyle = BorderStyle.None
            lnk.Controls.Add(img)

            e.Cell.Controls.Add(lnk)
        End If
    End Sub


    Protected Sub lnkDelivery_Click(sender As Object, e As System.EventArgs)
        Dim SQL As String
        Dim str As String
        Dim tok As String()
        Dim dateStr As String
        Dim custGrpVendorID As String
        Dim vendorName As String
        Dim r As DataReader
        Dim dt As DataTable
        Dim dr As DataRow

        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.SCHEDULE)

            SQL = "select anytimedelivery from customer where customer_id = :1"
            Dim anytime As String = CString(DB.GetScalar(SQL, lblCustomerID.Text))

            If anytime = "Y" Then
                MessageBox("Delivery Anytime has been set. No need to setup delivery date.", "Error")
                Exit Sub
            End If

            str = CString(Request.Form("__EVENTARGUMENT"))
            tok = CString(str).Split(",")
            dateStr = tok(0)
            vendorName = tok(1)
            SQL = "SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = :1 "
            custGrpVendorID = CString(DB.GetScalar(SQL, lstCustCompany.Text))

            'Show vendors
            SQL = "SELECT custGrpVendor_id, vendorName " &
                    "FROM custGrp_vendor " &
                    "WHERE custGrp_id = :1 " &
                    "ORDER BY vendorName"
            dt = DB.GetDataTable(SQL, lstCustCompany.Text)
            dr = dt.NewRow
            lstPopCustGrpVendor.DataSource = dt
            lstPopCustGrpVendor.DataValueField = "custGrpVendor_id"
            lstPopCustGrpVendor.DataTextField = "vendorName"
            lstPopCustGrpVendor.DataBind()

            ShowCodeMasterListLocal(DB, "TIMEOFDAY", lstPopOrderByTime, True)
            ShowCodeMasterListLocal(DB, "AMPM", lstPopOrderByAmPm, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstPopSendReminder, True)

            'Prepare popup
            pnlPopDtlEdit_schedule.Visible = True
            btnPopDeleteDelivery.Visible = True
            InvisibleCtrl({lstPopCustGrpVendor})
            InvisibleCtrl({lblPopStarCustGrpVendor, lblPopVendorName})
            ClearCtrl({txtPopDeliveryDate, txtPopOrderByDate})
            ClearCtrl({lblPopCustGrpVendorID, lblPopVendorName, lblPopSchedule})
            pnlPopSheduleWarning.Visible = False
            lblPopSheduleWarning.Text = ""

            If custGrpVendorID = "" Or vendorName = "" Then
                'New delivery
                lblPopHeader_schedule.Text = "Add"
                lblPopSchedule.Text = "N/A"
                txtPopDeliveryDate.Text = dateStr
                VisibleCtrl({lstPopCustGrpVendor})
                VisibleCtrl({lblPopStarCustGrpVendor})
                btnPopSaveDelivery.Text = "Add Delivery"
                btnPopDeleteDelivery.Visible = False
                ViewState("oldDeliveryOrderDate") = ""
                ViewState("oldDeliveryOrderTime") = ""
            Else
                'Existing delivery
                lblPopHeader_schedule.Text = "Edit"
                btnPopDeleteDelivery.Visible = True
                btnPopSaveDelivery.Text = "Update"
                VisibleCtrl({lblPopVendorName})

                'Get delivery info
                SQL = "SELECT d.custGrpVendor_id, TO_CHAR(d.deliveryDate, :1) deliveryDate, TO_CHAR(d.orderByDate, :2) orderByDate, " &
                        "TO_CHAR(d.orderByDate, :3) orderByTime, d.sendReminder, s.deliverySched_id, s.frequency, s.deliveryDay, s.orderByDay, s.orderByTime " &
                        "FROM customer_deliveryDate d " &
                        "LEFT OUTER JOIN customer_deliverySched s ON s.deliverySched_id = d.deliverySched_id " &
                        "WHERE d.customer_id = :4 AND d.shipPoint_id = :5" &
                        "  AND d.custGrpVendor_id = :6 AND d.deliveryDate = TO_DATE(:7, :8)"
                r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, TIME_SQLFMT, lblCustomerID.Text, lstShipPoint.Text, custGrpVendorID, dateStr, DATE_FMT)
                If r.Read() Then
                    lblPopVendorName.Text = vendorName
                    lblPopCustGrpVendorID.Text = CString(r("custGrpVendor_id"))
                    txtPopDeliveryDate.Text = CString(r("deliveryDate"))
                    lblPopOldDeliveryDate.Text = CString(r("deliveryDate"))
                    txtPopOrderByDate.Text = CString(r("orderByDate"))
                    lstPopOrderByTime.Text = Left(CString(r("orderByTime")), 5)
                    lstPopOrderByAmPm.Text = Right(CString(r("orderByTime")), 2)
                    ViewState("oldDeliveryOrderDate") = CString(r("orderByDate"))
                    ViewState("oldDeliveryOrderTime") = CString(r("orderByTime"))
                    lstPopSendReminder.Text = IIf(CString(r("sendReminder")) = "Y", "Yes", "No")
                    lblPopSchedule.Text = IIf(CString(r("deliverySched_id")) = "", "N/A",
                                              CString(r("frequency")) & ", " & "Delivery on " & CString(r("deliveryDay")) & ", " & "Order by " & CString(r("orderByDay")) & " " & CString(r("orderByTime")))
                End If
                r.close()
            End If
            ckbPopEmail.Checked = True
            'Show popup
            popDialog_schedule.Show()

            Exit Sub
        End Using
    End Sub


    Protected Sub btnPopClose_Click(sender As Object, e As System.EventArgs) Handles btnPopClose.Click
        setJavaScript(CustSetting_Tab.SCHEDULE)

        'Set date otherwise may not reopen due to the validation
        txtPopDeliveryDate.Text = Format(Today, DATE_FMT)
        txtPopOrderByDate.Text = Format(Today, DATE_FMT)
    End Sub


    Protected Sub btnPopSaveDelivery_Click(sender As Object, e As System.EventArgs) Handles btnPopSaveDelivery.Click
        Dim cmd As String
        Dim DeliveryDate As DateTime
        Dim Orderdate As DateTime

        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.SCHEDULE)
            If DateTime.TryParse(txtPopDeliveryDate.Text, DeliveryDate) And DateTime.TryParse(txtPopOrderByDate.Text, Orderdate) Then

                If DeliveryDate < Orderdate Then
                    pnlPopSheduleWarning.Visible = True
                    lblPopSheduleWarning.Text = "Delivery date has to be later than the order by date"
                    popDialog_schedule.Show()
                    Exit Sub
                End If

                'Save delivery date
                If lstPopCustGrpVendor.Visible = True Then
                    'Insert delivery date
                    cmd = "added"
                    InsertDeliveryDate(DB, lblCustomerID.Text, lstPopCustGrpVendor.Text, lstShipPoint.Text, txtPopDeliveryDate.Text, txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text, IIf(lstPopSendReminder.Text = "Yes", "Y", "N"), "", Nothing)
                Else
                    'Update delivery date
                    cmd = "updated"
                    UpdateDeliveryDate(DB, lblCustomerID.Text, lblPopCustGrpVendorID.Text, lstShipPoint.Text, lblPopOldDeliveryDate.Text, txtPopDeliveryDate.Text, txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text, IIf(lstPopSendReminder.Text = "Yes", "Y", "N"), True, False)
                End If

                If ckbPopEmail.Checked Then
                    Dim emailBody As String = ""
                    Dim Sql = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr, u.language ," &
                        "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
                        "FROM webuser u, webuser_customer uc, customer c " &
                        "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
                        "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
                        "AND c.customer_id = '" & CString(lblCustomerID.Text) & "' order by c.customer_id"
                    Dim dt = DB.GetDataTable(Sql)
                    Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
                    Dim r2 As DataReader
                    r2 = DB.GetReader(sql2, txtPopJFCAccount.Text)
                    Dim VendorEmail As String = ""
                    If r2.Read() Then
                        VendorEmail = CString(r2("email"))
                    End If
                    r2.close()

                    For Each dr In dt.Rows
                        Dim ci = New CultureInfo(IIf(CString(dr("language")) <> "", CString(dr("language")), "en-US").ToString)
                        Thread.CurrentThread.CurrentCulture = ci
                        Thread.CurrentThread.CurrentUICulture = ci
                        emailBody = rm.GetString("Dear", ci) & " " & CString(dr("customerName")) & " (" & CString(dr("jfcBranchNr")) & "-" & CString(dr("jfcCustomerNr")) &
                                    "), <br />  <br /> " & rm.GetString("Yourdeliveryinfohasbeenupdated", ci) & "  <br /> <br />" &
                                    rm.GetString("Old", ci) & " <br />" & rm.GetString("Delivery", ci) & ": " & lblPopOldDeliveryDate.Text & "<br />" &
                                    rm.GetString("Deadline", ci) & ":  " & ViewState("oldDeliveryOrderDate") & " " & ViewState("oldDeliveryOrderTime") & "<br /><br />" &
                                    rm.GetString("New", ci) & " <br /><span style=""font-weight:bold""> " & rm.GetString("Delivery", ci) & ": " & txtPopDeliveryDate.Text & " <br />" &
                                    rm.GetString("Deadline", ci) & ": " & txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text &
                                    "</span><br /> <br />   " & rm.GetString("Bestregards", ci) & ",<br />" & Common.JFC_COMPANY_NAME
                        Dim errmsg As String = ""
                        If Common.SendMail(SYSTEM_EMAIL, IIf(IsProdBeta() Or IsProduction(), CString(dr("customerEmail")), "yfu@kmsnet.com"), VendorEmail, rm.GetString("Yourdeliveryinfohasbeenupdated", ci), emailBody, Nothing, True, errmsg) > 0 Then
                            'Email sent failed
                        End If
                    Next
                End If

                showScheduleList()
                InsertEventLog(DB, "Saved delivery date", txtPopDeliveryDate.Text, Nothing, Nothing, True)
                MessageLine("CONFIRM", "Successfully saved")
            End If
        End Using
    End Sub


    Protected Sub btnPopDeleteDelivery_Click(sender As Object, e As System.EventArgs) Handles btnPopDeleteDelivery.Click
        Dim dt As Date

        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.SCHEDULE)
            'Delete delivery date
            DeleteDeliveryDate(DB, lblCustomerID.Text, lblPopCustGrpVendorID.Text, lstShipPoint.Text, txtPopDeliveryDate.Text)

            DateTime.TryParseExact(txtPopDeliveryDate.Text, DATE_FMT, Nothing, Nothing, dt)
            InsertEventLog(DB, "Deleted delivery date", txtPopDeliveryDate.Text, Nothing, Nothing, True)
            MessageLine("CONFIRM", "Successfully deleted")
        End Using
    End Sub


    Protected Sub cldSchedule_VisibleMonthChanged(sender As Object, e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles cldSchedule.VisibleMonthChanged
    End Sub


    Protected Sub grdActiveSchedule_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdActiveSchedule.RowCommand
        If e.CommandName = "EDIT_ROW" Then
            'showScheduleDtl(Convert.ToString(e.CommandArgument))
        ElseIf e.CommandName = "DELETE_ROW" Then
            'Delete schedule
            Dim gvr As GridViewRow = CType(CType(e.CommandSource, ImageButton).NamingContainer, GridViewRow)
            deleteScheduleRow(Convert.ToString(e.CommandArgument), gvr.RowIndex)
        End If
    End Sub


    Private Sub deleteScheduleRow(ByVal deliverySchedID As String, ByVal RowIndex As Integer)
        Using DB As New DBHelper
            'Confirm
            If Not String.IsNullOrWhiteSpace(deliverySchedID) Then
                ViewState("DIALOG_CMD") = "DELETE_SCHEDULE"
                ViewState("DIALOG_ARG1") = deliverySchedID
                MessageBoxYesNo("Are you sure you want to delete this delivery schedule?", "Confirm Deletion", DB)
            Else
                Dim dt As DataTable = GetTableWithNodata()
                Dim dr As DataRow

                For Each gvr As GridViewRow In grdActiveSchedule.Rows
                    dr = dt.NewRow()
                    dr(0) = gvr.Cells(GetColumnIndexByName(gvr, "customerName")).Text
                    dr(1) = CType(gvr.FindControl("lblDeliverySchedID"), Label).Text
                    dr(2) = CType(gvr.FindControl("lbldeliveryschedlinenr"), Label).Text
                    dr(3) = CType(gvr.FindControl("lnkShipName"), LinkButton).Text
                    dr(4) = gvr.Cells(GetColumnIndexByName(gvr, "vendorName")).Text
                    dr(5) = CType(gvr.FindControl("lnkdeliveryMasterName"), LinkButton).Text
                    dr(6) = gvr.Cells(GetColumnIndexByName(gvr, "FREQUENCY")).Text.Replace("&nbsp;", "")
                    dr(7) = gvr.Cells(GetColumnIndexByName(gvr, "DELIVERYDAY")).Text.Replace("&nbsp;", "")
                    dr(8) = gvr.Cells(GetColumnIndexByName(gvr, "orderByDayTime")).Text.Replace("&nbsp;", "")
                    dr(9) = If(CType(gvr.FindControl("ChkSendReminder"), CheckBox).Checked, "Y", "N")
                    dr(10) = gvr.Cells(GetColumnIndexByName(gvr, "STARTDATE")).Text
                    dr(11) = gvr.Cells(GetColumnIndexByName(gvr, "ENDDATE")).Text

                    dt.Rows.Add(dr)
                Next
                dt.Rows.RemoveAt(RowIndex)
                dt.AcceptChanges()
                grdActiveSchedule.DataSourceID = ""
                grdActiveSchedule.DataSource = dt
                grdActiveSchedule.DataBind()

                'Re-baseline so the remaining untouched rows are not re-saved
                SnapshotSchedule()
            End If
        End Using
    End Sub


    Protected Sub btnAddschedule_Click(sender As Object, e As System.EventArgs) Handles btnAddSchedule.Click

        Dim dt As DataTable = GetTableWithNodata()
        Dim dr As DataRow

        Using db As New DBHelper
            Dim anytime As String = lstDeliveryAnytime.Text

            If anytime = "Yes" Then
                MessageBox("Delivery Anytime has been set. No need to setup delivery date.", "Error")
                Exit Sub
            Else
                For Each gvr As GridViewRow In grdActiveSchedule.Rows
                    dr = dt.NewRow()
                    dr(0) = gvr.Cells(GetColumnIndexByName(gvr, "customerName")).Text
                    dr(1) = CType(gvr.FindControl("lblDeliverySchedID"), Label).Text
                    dr(2) = CType(gvr.FindControl("lbldeliveryschedlinenr"), Label).Text
                    dr(3) = CType(gvr.FindControl("lnkShipName"), LinkButton).Text
                    dr(4) = gvr.Cells(GetColumnIndexByName(gvr, "vendorName")).Text
                    dr(5) = CType(gvr.FindControl("lnkdeliveryMasterName"), LinkButton).Text
                    dr(6) = gvr.Cells(GetColumnIndexByName(gvr, "FREQUENCY")).Text.Replace("&nbsp;", "")
                    dr(7) = gvr.Cells(GetColumnIndexByName(gvr, "DELIVERYDAY")).Text.Replace("&nbsp;", "")
                    dr(8) = gvr.Cells(GetColumnIndexByName(gvr, "orderByDayTime")).Text.Replace("&nbsp;", "")
                    dr(9) = If(CType(gvr.FindControl("ChkSendReminder"), CheckBox).Checked, "Y", "N")
                    dr(10) = gvr.Cells(GetColumnIndexByName(gvr, "STARTDATE")).Text
                    dr(11) = gvr.Cells(GetColumnIndexByName(gvr, "ENDDATE")).Text

                    dt.Rows.Add(dr)
                Next

                dr = dt.NewRow()
                dr(9) = "Y"
                dt.Rows.Add(dr)

                grdActiveSchedule.DataSourceID = ""
                grdActiveSchedule.DataSource = dt
                grdActiveSchedule.DataBind()

                'Re-baseline. The new blank row has no CommandArgument, so it is not in the snapshot
                'and will always be treated as changed (= saved).
                SnapshotSchedule()
            End If
        End Using
    End Sub


    Public Function GetTableWithNodata() As DataTable
        Dim dt = New DataTable()
        dt.Columns.Add("customerName", GetType(String))
        dt.Columns.Add("deliverySched_id", GetType(String))
        dt.Columns.Add("deliveryschedlinenr", GetType(String))
        dt.Columns.Add("shipName", GetType(String))
        dt.Columns.Add("vendorName", GetType(String))
        dt.Columns.Add("deliveryMaster_id", GetType(String))
        dt.Columns.Add("FREQUENCY", GetType(String))
        dt.Columns.Add("DELIVERYDAY", GetType(String))
        dt.Columns.Add("orderByDayTime", GetType(String))
        dt.Columns.Add("sendReminder", GetType(String))
        dt.Columns.Add("STARTDATE", GetType(String))
        dt.Columns.Add("ENDDATE", GetType(String))
        Return dt
    End Function


    Protected Sub lstDeliveryMaster_SelectedIndexChanged(sender As Object, e As System.EventArgs)
        Dim ddlClientId As String = (CType(sender, DropDownList)).ClientID
        Dim firstcount As Integer = ddlClientId.IndexOf("_")
        Dim lastcount As Integer = ddlClientId.LastIndexOf("_")
        Dim sControl As String = ddlClientId.Substring(lastcount + 1)
        Dim lstDeliveryMasterID As String = String.Format("MainContent_grdActiveSchedule_lstDeliveryMaster_{0}", sControl)
        Dim masterlist As New List(Of String)
        Dim DeliveryDateList As New List(Of String)
        Dim selectDeliveryMaster As New DropDownList
        Dim SelectDeliveryDate As String = ""
        Dim selectRowIndex As Integer = 0

        For Each row As GridViewRow In grdActiveSchedule.Rows

            If row.RowType = DataControlRowType.DataRow Then
                Dim lstDeliveryMaster As DropDownList = CType(row.FindControl("lstDeliveryMaster"), DropDownList)
                Dim masterValue As String = CType(row.FindControl("lstDeliveryMaster"), DropDownList).SelectedItem.Text
                Dim DeliverDate As String = row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text

                If String.Compare(lstDeliveryMasterID, (CType(row.FindControl("lstDeliveryMaster"), DropDownList)).ClientID, True) = 0 Then

                    If lstDeliveryMaster.SelectedValue <> "" Then
                        Dim SQL As String

                        Using DB As New DBHelper
                            SQL = "SELECT jd.deliveryMaster_id, jd.frequency, jd.deliveryDay, jd.orderByDay || ' ' || jd.orderByTime AS orderByDayTime, " &
                                    " c.deliverySched_id, c.customer_id, c.shipPoint_id, c.sendReminder, to_char(NVL(c.startDate, SYSDATE), " & DB.SqlEsc(DATE_FMT) & ") startDate, to_char(c.endDate, " & DB.SqlEsc(DATE_FMT) & ") endDate " &
                                    "FROM JFCDeliveryMaster jd, customer_deliverySched c " &
                                    "WHERE jd.deliveryMaster_id = c.deliveryMaster_id (+) " &
                                    "  AND jd.deliveryMaster_id = " & DB.SqlEsc(lstDeliveryMaster.Text)
                            selectDeliveryMaster = lstDeliveryMaster
                            selectRowIndex = row.RowIndex
                            Dim r = DB.GetReader(SQL)
                            If r.Read() Then
                                CType(row.FindControl("lnkdeliveryMasterName"), LinkButton).Text = lstDeliveryMaster.Text
                                row.Cells(GetColumnIndexByName(row, "FREQUENCY")).Text = CString(r("frequency"))
                                row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text = CString(r("deliveryDay"))
                                row.Cells(GetColumnIndexByName(row, "orderByDayTime")).Text = CString(r("orderByDayTime"))
                                selectDeliveryMaster = lstDeliveryMaster
                                SelectDeliveryDate = CString(r("deliveryDay"))
                            End If
                        End Using
                    Else
                        CType(row.FindControl("lnkdeliveryMasterName"), LinkButton).Text = ""
                        row.Cells(GetColumnIndexByName(row, "FREQUENCY")).Text = ""
                        row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text = ""
                        row.Cells(GetColumnIndexByName(row, "orderByDayTime")).Text = ""
                    End If
                Else
                    masterlist.Add(masterValue)
                    DeliveryDateList.Add(DeliverDate)
                End If
            End If
        Next

        Dim selectrow = grdActiveSchedule.Rows(selectRowIndex)
        If selectRowIndex <> 0 Then

            If masterlist.Contains(selectDeliveryMaster.SelectedItem.Text) Then
                MessageBox("This delivery schedule master name already exists. Please enter another one.", "Error")
                selectDeliveryMaster.SelectedIndex = 0

                selectrow.Cells(GetColumnIndexByName(selectrow, "FREQUENCY")).Text = ""
                selectrow.Cells(GetColumnIndexByName(selectrow, "DELIVERYDAY")).Text = ""
                selectrow.Cells(GetColumnIndexByName(selectrow, "orderByDayTime")).Text = ""
                Exit Sub
            End If

            If DeliveryDateList.Contains(SelectDeliveryDate) Then
                MessageBox("This delivery day already exists. Please select another one.", "Error")
                selectDeliveryMaster.SelectedIndex = 0

                selectrow.Cells(GetColumnIndexByName(selectrow, "FREQUENCY")).Text = ""
                selectrow.Cells(GetColumnIndexByName(selectrow, "DELIVERYDAY")).Text = ""
                selectrow.Cells(GetColumnIndexByName(selectrow, "orderByDayTime")).Text = ""
                Exit Sub
            End If
        End If
    End Sub


    Protected Sub chkShowAll2_schedule_CheckedChanged(sender As Object, e As System.EventArgs)
        setJavaScript(CustSetting_Tab.SCHEDULE)
        If lstDeliveryAnytime.Text = "Yes" Then
            MessageBox("Delivery Anytime, no need to setup delivery schedule.", "Error")
        End If
    End Sub


    'Not used
    Protected Sub lstGrdFreq_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub


    '--------------------------------------------------------------------------------------------------------------------
    'Saves ONE schedule row. The caller decides which rows need this, rebinds the grid once afterwards, and shows the
    'confirmation message - so there is no showScheduleList() or MessageLine() in here any more.
    '--------------------------------------------------------------------------------------------------------------------
    Protected Sub SaveSchedule(TotalDeliverySchedID As String, lstDeliveryMaster As DropDownList, Frequency As String, DeliveryOn As String, orderby As String, IsReminder As Boolean, IsEmail As Boolean)
        Dim SQL As String
        Dim newSchedule As Boolean
        Dim startDateVal As Date
        Dim endDateVal As Date = Nothing
        Dim startDateStr As String
        Dim endDateStr As String
        Dim sendReminder As String
        Dim deliverySchedID As String
        Dim deliveryschedlinenr As Integer
        Dim orderbydate As String = ""
        Dim orderbytime As String = ""

        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.SCHEDULE)

            newSchedule = IIf(TotalDeliverySchedID = AUTO OrElse TotalDeliverySchedID = "", True, False)
            If newSchedule Then
                deliverySchedID = ""
                deliveryschedlinenr = 0
            Else
                deliverySchedID = TotalDeliverySchedID.Substring(0, DeliverySchedID_LEN)
                deliveryschedlinenr = CInt(TotalDeliverySchedID.Substring(DeliverySchedID_LEN))
            End If

            SQL = "SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = :1 "
            Dim custGrpVendorID = CString(DB.GetScalar(SQL, lstCustCompany.Text))

            DB.BeginTransaction()

            If newSchedule = False Then
                'Delete delivery schedule and the dates
                DeleteDeliverySched(DB, deliverySchedID, deliveryschedlinenr, False)
            End If

            startDateStr = Format(Date.Today, Common.DATE_FMT)
            DateTime.TryParseExact(startDateStr, DATE_FMT, Nothing, Nothing, startDateVal)
            endDateStr = ""
            If endDateStr <> "" Then DateTime.TryParseExact(endDateStr, DATE_FMT, Nothing, Nothing, endDateVal)
            sendReminder = IIf(IsReminder, "Y", "N")

            If orderby <> "" Then
                Dim Orderbysplit = orderby.Split(" ")
                orderbydate = Orderbysplit(0)
                orderbytime = Orderbysplit(1) & " " & Orderbysplit(2)
            End If

            UpSertDeliverySched(DB, deliverySchedID, deliveryschedlinenr, lblCustomerID.Text, custGrpVendorID, lstShipPoint.Text, lstDeliveryMaster.Text,
                                   Frequency, startDateVal, endDateVal, DeliveryOn, orderbydate, orderbytime, sendReminder, "", False)
            DB.CommitTransaction()

            'Saved
            InsertEventLog(DB, "Saved delivery schedule", deliverySchedID, Nothing, Nothing, True)

            If IsEmail Then
                Dim emailBody As String = ""
                SQL = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr,u.language ," &
                    "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
                    "FROM webuser u, webuser_customer uc, customer c " &
                    "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
                    "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
                    "AND c.customer_id = '" & CString(lblCustomerID.Text) & "' order by c.customer_id"
                Dim dt = DB.GetDataTable(SQL)
                Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
                Dim r2 As DataReader
                r2 = DB.GetReader(sql2, txtPopJFCAccount.Text)
                Dim VendorEmail As String = ""
                If r2.Read() Then
                    VendorEmail = CString(r2("email"))
                End If
                r2.close()

                For Each dr In dt.Rows
                    Dim ci = New CultureInfo(IIf(CString(dr("language")) <> "", CString(dr("language")), "en-US").ToString)
                    Thread.CurrentThread.CurrentCulture = ci
                    Thread.CurrentThread.CurrentUICulture = ci
                    CreateEmailBody(Page, emailBody, "MASTER", deliverySchedID & deliveryschedlinenr, newSchedule)
                    Dim errmsg As String = ""
                    If Common.SendMail(SYSTEM_EMAIL, IIf(Common.IsDev(), "yfu@kmsnet.com, jzhou@kmsnet.com", CString(dr("customerEmail"))), VendorEmail, String.Format(rm.GetString("Yourdeliveryschedulewasupdated", ci), DB.NVL(deliverySchedID, "")), emailBody, Nothing, True, errmsg) > 0 Then
                        'Email sent failed
                    End If
                Next
            End If
        End Using
    End Sub


    Private Sub showScheduleList()
        Dim SQL As String

        Using DB As New DBHelper
            grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "endDate")).Visible = False

            If IsMobile() Then
                grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "vendorName")).Visible = False
                grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "startDate")).Visible = False
                grdActiveSchedule.Columns(10).Visible = False 'Delete
            End If

            'Show active shipping schedule
            SQL = "SELECT m.custCompanyName, g.custGrpName, c.customerName, s.shipName, c.custGrp_id, d.customer_id, v.vendorName, d.deliverySched_id,d.deliveryschedlinenr,d.deliveryMaster_id, jh.deliveryMasterName, d.frequency, d.sendReminder, " &
                        " to_char(d.startDate, " & DB.SqlEsc(DATE_FMT) & ") startDate, to_char(d.endDate, " & DB.SqlEsc(DATE_FMT) & ") endDate, d.deliveryDay, d.orderByDay || ' ' || d.orderByTime AS orderByDayTime " &
                        "FROM customer_deliverySched d, JFCDeliveryMaster jh, customer c, customer_shipPoint s, custGrp_vendor v, webUser_customer u, custGrp g, custCompany m " &
                        "WHERE d.deliveryMaster_id = jh.deliveryMaster_id (+) " &
                        "  AND d.customer_id = c.customer_id " &
                        "  AND d.shipPoint_id = s.shipPoint_id AND d.customer_id = s.customer_id " &
                        "  AND u.custCompany_id = g.custCompany_id AND g.custGrp_id = c.custGrp_id AND c.customer_id = u.customer_id AND u.custCompany_id = m.custCompany_id " &
                        "  AND u.email = " & DB.SqlEsc(GetUserEmail(DB)) & " " &
                        "  AND d.customer_id = " & DB.SqlEsc(lblCustomerID.Text) & " AND d.shipPoint_id = " & DB.SqlEsc(lstShipPoint.Text) &
                        "  AND (d.endDate >= TRUNC(SYSDATE) OR d.endDate is NULL) " &
                        "  AND d.custGrpVendor_id = v.custGrpVendor_id"
            dsActiveSchedule.ProviderName = "System.Data.OracleClient"
            dsActiveSchedule.ConnectionString = DB.GetConnectionString
            dsActiveSchedule.SelectCommand = SQL
            grdActiveSchedule.DataSourceID = "dsActiveSchedule"
            grdActiveSchedule.DataBind()

            'Baseline for change detection - must be after DataBind
            SnapshotSchedule()
        End Using
    End Sub


    '--------------------------------------------------------------------------------------------------------------------
    'Common routine
    '--------------------------------------------------------------------------------------------------------------------

    Protected Sub grd_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdActiveSchedule.RowDataBound
        'Highlight the mouse-over row and make it selectable
        Dim sql As String
        Dim dt As DataTable
        Dim dr As DataRow
        Dim lst As DropDownList
        Dim Str As String

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.CssClass = "selectRow"
            e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
            Dim linkbtn As LinkButton = e.Row.FindControl("lnkdeliveryMasterName")
            Dim Imgbtn As ImageButton = e.Row.FindControl("btnDelete")

            If Not ViewState("JFC_ADMINGROUP") Then
                linkbtn.ForeColor = Drawing.Color.Black
                linkbtn.Enabled = False
                Imgbtn.Visible = False
            End If

            Using DB As New DBHelper
                sql = "SELECT deliveryMaster_id, deliveryMasterName FROM JFCDeliveryMaster WHERE jfcBranchNr = :1 ORDER BY deliveryMasterName"
                dt = DB.GetDataTable(sql, lblJfcBranchNr.Text)
                dr = dt.NewRow
                dr("deliveryMasterName") = LST_CMD_STR
                dt.Rows.InsertAt(dr, 0)
                lst = CType(e.Row.FindControl("lstDeliveryMaster"), DropDownList)
                lst.DataSource = dt
                lst.DataValueField = "deliveryMaster_id"
                lst.DataTextField = "deliveryMasterName"
                lst.DataBind()
                Str = CType(e.Row.FindControl("lnkdeliveryMasterName"), LinkButton).Text
                If lst.Items.FindByValue(Str) IsNot Nothing Then lst.Items.FindByValue(Str).Selected = True
            End Using
        End If
    End Sub


    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        Dim id As String = dialogArg1

        Using DB As New DBHelper
            Select Case dialogCmd
                Case "DELETE_CUSTITEM"
                    'TODO: Implement later

                Case "DELETE_SCHEDULE"
                    setJavaScript(CustSetting_Tab.SCHEDULE)
                    'Delete delivery schedule and dates
                    DeleteDeliverySched(DB, id.Substring(0, DeliverySchedID_LEN), id.Substring(DeliverySchedID_LEN))
                    showScheduleList()
                    InsertEventLog(DB, "Deleted delivery schedule", id, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")

                Case "DELETE_ALLSCHEDULE"
                    setJavaScript(CustSetting_Tab.CUST)
                    DeleteAllDeliverySched(DB, lblCustomerID.Text)
                    showScheduleList()
                    InsertEventLog(DB, "Deleted all delivery schedule", id, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")

                Case "DELETE_USER"
                    SendUserRemovalRequest(DB, id, lstCustCompany.Text, lblCustomerName.Text, True)
                    showCustInfo()
                    InsertEventLog(DB, "Deleted User", id, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
            End Select
        End Using
    End Sub


    Private Sub dialogResult_no(ByVal dialogCmd As String)
        Select Case dialogCmd
            Case "DELETE_CUSTITEM"
                setJavaScript(CustSetting_Tab.CUSTITEM)

            Case "DELETE_ALLSCHEDULE"
                Using db As New DBHelper
                    setJavaScript(CustSetting_Tab.CUST)
                    lstDeliveryAnytime.SelectedValue = "No"
                    pnlSchedule.Visible = True

                    Dim AllowCustOrder = IIf(CString(lstAllowCustOrder.SelectedValue) = "Allow", "Y", "")
                    Dim HideItemPage = IIf(CString(lstHideItemPage.SelectedValue) = "Hide", "Y", "")
                    Dim HidePaymentPage = IIf(CString(lstHidePaymentPage.SelectedValue) = "Hide", "Y", "")
                    Dim HidePrice = IIf(CString(lstHidePrice.SelectedValue) = "Hide", "Y", "")
                    Dim anytimedelivery = IIf(CString(lstDeliveryAnytime.SelectedValue) = "Yes", "Y", "")
                    Dim HideHomePage = IIf(CString(lstHideHomePage.SelectedValue) = "Hide", "Y", "")
                    Dim Textphones = txtTextPhones.Text.TrimEnd(",")
                    Dim outofstock = IIf(CString(lstOutofStock.SelectedValue) = "Show", "Y", "")
                    Dim ShowAllExcepGrpItem = IIf(CString(lstAllExcepGrpItem.SelectedValue) = "Yes", "Y", "")
                    Dim shipVia = CString(txtShipvia.Text)
                    Dim shipOffset = Val(txtShipDateOffset.Text)
                    Dim minOrderAmt = Val(txtMinOrderAmt.Text)
                    Dim allowchat As String
                    Select Case CString(lstAllowChat.SelectedValue)
                        Case "Read Only"
                            allowchat = "R"
                        Case "Allow"
                            allowchat = "Y"
                        Case Else
                            allowchat = ""
                    End Select

                    UpdateCustSetting(db, lblCustomerID.Text, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice, anytimedelivery, Textphones, outofstock, ShowAllExcepGrpItem, shipVia, shipOffset, minOrderAmt, HideHomePage, allowchat)

                    'Settings were just written here, so re-baseline to avoid saving them a second time
                    SnapshotSettings()
                End Using

            Case Else
        End Select
    End Sub


    Protected Sub grdUser_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdUser.RowDataBound
        Dim lst As DropDownList
        Dim txt As TextBox
        Dim str As String

        Using DB As New DBHelper
            If e.Row.RowType = DataControlRowType.DataRow Then
                'Show and select hide order
                lst = CType(e.Row.FindControl("lstHideCustOrder"), DropDownList)
                ShowCodeMasterListLocal(DB, "NALLOWNOT", lst, False)
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                str = CType(e.Row.FindControl("lblHideCustOrder"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                'Show and select hide item page
                lst = CType(e.Row.FindControl("lstHideItemPage"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHideItemPage"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                'Show and select hide payment page
                lst = CType(e.Row.FindControl("lstHidePaymentPage"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHidePaymentPage"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")

                'Show and select hide price
                lst = CType(e.Row.FindControl("lstHidePrice"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHidePrice"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")

                'Show and select hide chat
                If lstAllowChat.Text = "Read Only" Then
                    lst = CType(e.Row.FindControl("lstHideChat"), DropDownList)
                    ShowCodeMasterListLocal(DB, "READNOT_CHAT", lst, False)
                    str = CType(e.Row.FindControl("lblHideChat"), Label).Text
                    str = IIf(str = "Y", "Y", "N")
                    If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                    IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                Else
                    lst = CType(e.Row.FindControl("lstHideChat"), DropDownList)
                    ShowCodeMasterListLocal(DB, "NALLOWNOT", lst, False)
                    str = CType(e.Row.FindControl("lblHideChat"), Label).Text
                    str = IIf(str = "Y", "Y", "N")
                    If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                    IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                End If

                txt = CType(e.Row.FindControl("txtCell"), TextBox)
                IIf(IsMobile(), txt.CssClass = "cellLbl size4", "")
            End If
        End Using
    End Sub


    Protected Sub grdUser_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdUser.RowCommand
        If e.CommandName = "DELETE_ROW" Then
            deleteUserRow(Convert.ToString(e.CommandArgument))
        End If
    End Sub


    Private Sub deleteUserRow(ByVal email As String)
        Using DB As New DBHelper
            'Confirm
            If Not String.IsNullOrWhiteSpace(email) Then
                ViewState("DIALOG_CMD") = "DELETE_USER"
                ViewState("DIALOG_ARG1") = email
                If grdUser.Rows.Count > 1 Then
                    MessageBoxYesNo("Are you sure you want to delete " & email & " from " & String.Format("{0} ({1}-{2})", lblCustomerName.Text, lblJfcBranchNr.Text, lblJfcCustomerNr.Text) & "?", "Confirm Deletion", DB)
                Else
                    MessageBoxYesNo("All the online information (bank account, online payment history, etc...) for this account will be deleted if there is no user. Are you sure you want to delete " &
                                    email & " from " & String.Format("{0} ({1}-{2})", lblCustomerName.Text, lblJfcBranchNr.Text, lblJfcCustomerNr.Text) & "?", "Confirm Deletion", DB)
                End If
            End If
        End Using
    End Sub


    Protected Sub btnAddCust_Click(sender As Object, e As System.EventArgs) Handles btnAddCust.Click
        lstCustCompany.SelectedIndex = 0

        lblTitle.Visible = True
        Alltab.Visible = False

        txtPopJFCAccount.Text = txtJfcBranchNr.Text
        txtPopJFCNum.Text = txtJfcCustomerNr.Text.ToUpper
        lblPopCustomerName.Text = ""
        txtPopEmail.Text = ""

        If IsProduction() = True OrElse IsProdBeta() = True Then
            If Not String.IsNullOrEmpty(txtPopJFCAccount.Text) And Not String.IsNullOrEmpty(txtPopJFCNum.Text) Then
                Using DB As New DBHelper()
                    Dim Sql = "SELECT c.Name FROM  jho.customer@jho c " &
                        " WHERE c.bid = :1 AND c.customerid = :2"
                    lblPopCustomerName.Text = CString(DB.GetScalar(Sql, txtPopJFCAccount.Text, txtPopJFCNum.Text))
                End Using
            End If
        End If

        txtPopJFCAccount.Enabled = True
        txtPopJFCNum.Enabled = True
        lblPopTitleUser.Text = "Add New Customer"
        pnlPopWarningUser.Visible = False
        lblPopWarningUser.Text = ""

        popDialog_User.Show()
    End Sub


    Protected Sub btnPopSaveUser_Click(sender As Object, e As System.EventArgs) Handles btnPopSaveUser.Click

        Dim SQL, message As String
        Dim errMsg As String = ""
        Dim jfcVendorEmail As String = ""
        Dim jfcBranchNr As String = ""
        Dim email As String = CString(txtPopEmail.Text).Trim.ToLower
        Dim salesmanEmail As String = ""
        Dim r As DataReader
        Dim isNewWebUser As Boolean
        Dim userEmail As String
        pnlPopWarningUser.Visible = False

        Using DB As New DBHelper()
            userEmail = GetUserEmail(DB)
            If txtPopJFCAccount.Text = "" OrElse txtPopJFCNum.Text = "" OrElse txtPopJFCNum.Text.Length <> 4 Then
                lblPopWarningUser.Text = "Error! Please enter a valid JFC Account #."
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If

            If IsProduction() = True OrElse IsProdBeta() = True Then
                If Not String.IsNullOrEmpty(txtPopJFCAccount.Text) And Not String.IsNullOrEmpty(txtPopJFCNum.Text) Then
                    SQL = "SELECT c.Name FROM  jho.customer@jho c " &
                        " WHERE c.bid = :1 AND c.customerid = :2"
                    lblPopCustomerName.Text = CString(DB.GetScalar(SQL, txtPopJFCAccount.Text, txtPopJFCNum.Text))
                End If
            End If

            If Not lstCustCompany.Items.Cast(Of ListItem).Any(Function(x) x.Text.Substring(0, 2).Contains(txtPopJFCAccount.Text)) Then
                pnlPopWarningUser.Visible = True
                lblPopWarningUser.Text = "Error! You can not add to this branch."
                popDialog_User.Show()
                Exit Sub
            End If

            If Common.IsEmail(email) = False Then
                lblPopWarningUser.Text = "Error! Your email address " & email & " is invalid."
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If

            If email.Contains(",") Then
                lblPopWarningUser.Text = "Error! Your email address cannot contain ','"
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If

            If checkUserRemovalRequest(DB, email, txtPopJFCAccount.Text, txtPopJFCNum.Text) Then
                DeleteUserRemovalRequest(DB, email, txtPopJFCAccount.Text, txtPopJFCNum.Text)
                MessageLine("CONFIRM", "Successfully added")
                showCustInfo()
                Exit Sub
            End If

            SQL = "SELECT u.email FROM webuser_customer uc, customer c, webuser u, jfcVendor v " &
                    "WHERE uc.email = u.email AND uc.customer_id = c.customer_id AND c.jfcBranchNr = v.jfcBranchNr AND v.jfcBranchNr = :1 AND c.jfcCustomerNr = :2 AND uc.email = :3"
            If CString(DB.GetScalar(SQL, txtPopJFCAccount.Text, txtPopJFCNum.Text, email)) <> String.Empty Then
                lblPopWarningUser.Text = "Error! This customer already exists. "
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If

            If IsProduction() = True OrElse IsProdBeta() = True Then
                'Get sales rep email (to address) and branch email (cc address)
                If SUPPORT_EMAIL.Contains(userEmail) OrElse IsJfcBranchAdmin(DB, userEmail) Then
                    SQL = "SELECT s.email1 AS salesmanEmail, b.branchid, CASE WHEN b.branchid = '45' THEN '45' ELSE b.parent_branchid END AS parentBranchID " &
                       "FROM jho.branch@jho b, jho.customer@jho c LEFT JOIN jho.salesman@jho s ON c.salesmanid = s.salesmanid " &
                       "WHERE b.branchid = c.bid AND c.bid = s.bid AND c.cid = s.cid AND b.branchid = :1 AND c.customerid = :2 " &
                       "group by s.email1, b.branchid,b.parent_branchid"
                Else
                    SQL = "SELECT s.email1 AS salesmanEmail, b.branchid, CASE WHEN b.branchid = '45' THEN '45' ELSE b.parent_branchid END AS parentBranchID " &
                       "FROM jho.branch@jho b, jho.customer@jho c, jho.salesman@jho s " &
                       "WHERE b.branchid = c.bid AND c.bid = s.bid AND c.cid = s.cid AND c.salesmanid = s.salesmanid AND b.branchid = :1 AND c.customerid = :2 " &
                       "group by s.email1, b.branchid,b.parent_branchid"
                End If

                r = DB.GetReader(SQL, txtPopJFCAccount.Text, txtPopJFCNum.Text)
                If r.Read() Then
                    salesmanEmail = CString(r("salesmanEmail"))
                    jfcBranchNr = CString(r("branchid"))
                End If
                r.close()

                If String.IsNullOrWhiteSpace(salesmanEmail) Then
                    lblPopWarningUser.Text = "Error! There is no salesman email for this account " & txtPopJFCAccount.Text & " - " & txtPopJFCNum.Text & "."
                    pnlPopWarningUser.Visible = True
                    popDialog_User.Show()
                    Exit Sub
                End If

                If Not (SUPPORT_EMAIL.Contains(userEmail) OrElse salesmanEmail.Contains(userEmail) OrElse IsJfcBranchAdmin(DB, userEmail)) Then
                    lblPopWarningUser.Text = "Error! You are not assigned to be the sales rep of this account " &
                        txtPopJFCAccount.Text & " - " & txtPopJFCNum.Text & ".<br /> You only can add your customers' account."
                    pnlPopWarningUser.Visible = True
                    popDialog_User.Show()
                    Exit Sub
                End If
            Else
                salesmanEmail = "yfu@kmsnet.com"
                jfcBranchNr = "30"
            End If

            SQL = "select email From signupRequest where email = :1 and jfcbranchnr =:2 and jfccustomernr =:3 and AUTOSIGNUPSYNCDATE is null and createdate > sysdate - 15/1440  "
            Dim IsSignupEmail = CString(DB.GetScalar(SQL, email, txtPopJFCAccount.Text, txtPopJFCNum.Text))
            If IsSignupEmail <> String.Empty Then
                lblPopWarningUser.Text = "Error! New customer " & email & " has been added.<br /> Please wait 15 minutes for data to be copied over."
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If

            SQL = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
            r = DB.GetReader(SQL, txtPopJFCAccount.Text)
            If r.Read() Then
                jfcVendorEmail = CString(r("email"))
            End If
            r.close()

            SQL = "SELECT email FROM webuser WHERE email = :1"
            isNewWebUser = IIf(CString(DB.GetScalar(SQL, email)) = String.Empty, True, False)

            'Save request info
            InsertSignupRequest(DB, jfcBranchNr, txtPopJFCNum.Text, "JFCADMIN", "JFCADMIN", email, "JFCADMIN", "JFCADMIN", salesmanEmail, jfcVendorEmail)

            InsertEventLog(DB, "Add a new customer signup request", txtPopJFCAccount.Text & txtJfcCustomerNr.Text.ToUpper, email, salesmanEmail, True)
            MessageBox("New customer " & email & " has been added. Please wait 15 minutes for data to be copied over.", "Add Successful")
        End Using
    End Sub


    Protected Function checkUserRemovalRequest(ByVal db As DBHelper, ByVal email As String, ByVal jfcBranchNr As String, ByVal jfcCustomerNr As String) As Boolean
        Dim sql = "Select email from UserRemovalRequest where status = 'N' and  email= :1 AND jfcBranchNr = :2 AND jfcCustomerNr = :3"
        If String.IsNullOrEmpty(db.GetScalar(sql, email, jfcBranchNr, jfcCustomerNr)) Then
            Return False
        Else
            Return True
        End If
    End Function


    Protected Sub btnDisplayCalendar_Click(sender As Object, e As System.EventArgs) Handles btnDisplayCalendar.Click
        If pnlCalendar.Visible Then
            btnDisplayCalendar.Text = "Show Delivery Calendar"
        Else
            btnDisplayCalendar.Text = "Hide Delivery Calendar"
        End If

        pnlCalendar.Visible = Not pnlCalendar.Visible
    End Sub


    '(1) AllowChat: N/null -> Y  - create chat room + add JFC members + chat admin
    Public Sub InsertChatRoomAdd(ByVal DB As DBHelper, ByVal customerID As String, ByVal jfcBranchNr As String,
                              ByVal jfcCustomerNr As String, ByVal customerName As String, ByVal createUser As String)

        'Add room
        DB.ExecuteNonQuery(
        "INSERT INTO chat_customer_cmd (cmd_id, customer_id, jfcBranchNr, jfcCustomerNr, cmd, customerName, createUser, createDate) " &
        "VALUES (chat_customer_cmd_seq.NEXTVAL, :1, :2, :3, 'ADD', :4, :5, SYSDATE)",
        customerID, jfcBranchNr, jfcCustomerNr, customerName, createUser)

        'Add JFC members
        DB.ExecuteNonQuery(
        "INSERT INTO chat_webuser_customer_cmd (cmd_id, email, customer_id, jfcBranchNr, jfcCustomerNr, cmd, userType, firstname, lastName, createUser, createDate) " &
        "SELECT chat_webuser_customer_cmd_seq.NEXTVAL, wc.email, wc.customer_id, :1, :2, 'ADD', 'JFC', w.firstName, w.lastName, :3, SYSDATE " &
        "FROM webuser_customer wc, webuser w " &
        "WHERE wc.email = w.email AND wc.customer_id = :4 AND wc.userCompany_id = 'JFC'",
        jfcBranchNr, jfcCustomerNr, createUser, customerID)

        'Add chat admin
        DB.ExecuteNonQuery(
        "INSERT INTO chat_webuser_customer_cmd (cmd_id, email, customer_id, jfcBranchNr, jfcCustomerNr, cmd, userType, createUser, createDate) " &
        "VALUES (chat_webuser_customer_cmd_seq.NEXTVAL, 'chatadmin@jfc.com', :1, :2, :3, 'ADD', 'JFC', :4, SYSDATE)",
        customerID, jfcBranchNr, jfcCustomerNr, createUser)

        'TODO: Add Customer (non-JFC) members based on each user's hideChat setting on the customer settings page
    End Sub


    '(2) AllowChat: Y -> N/null - remove room
    Public Sub InsertChatRoomDel(ByVal DB As DBHelper, ByVal customerID As String, ByVal jfcBranchNr As String,
                              ByVal jfcCustomerNr As String, ByVal customerName As String, ByVal createUser As String)
        DB.ExecuteNonQuery(
        "INSERT INTO chat_customer_cmd (cmd_id, customer_id, jfcBranchNr, jfcCustomerNr, cmd, customerName, createUser, createDate) " &
        "VALUES (chat_customer_cmd_seq.NEXTVAL, :1, :2, :3, 'DEL', :4, :5, SYSDATE)",
        customerID, jfcBranchNr, jfcCustomerNr, customerName, createUser)

        'Delete JFC members
        DB.ExecuteNonQuery(
            "INSERT INTO chat_webuser_customer_cmd (cmd_id, email, customer_id, jfcBranchNr, jfcCustomerNr, cmd, userType, firstname, lastName, createUser, createDate) " &
            "SELECT chat_webuser_customer_cmd_seq.NEXTVAL, wc.email, wc.customer_id, :1, :2, 'DEL', 'JFC', w.firstName, w.lastName, :3, SYSDATE " &
            "FROM webuser_customer wc, webuser w " &
            "WHERE wc.email = w.email AND wc.customer_id = :4 AND wc.userCompany_id = 'JFC'",
            jfcBranchNr, jfcCustomerNr, createUser, customerID)

        'Delete chat admin
        DB.ExecuteNonQuery(
            "INSERT INTO chat_webuser_customer_cmd (cmd_id, email, customer_id, jfcBranchNr, jfcCustomerNr, cmd, userType, createUser, createDate) " &
            "VALUES (chat_webuser_customer_cmd_seq.NEXTVAL, 'chatadmin@jfc.com', :1, :2, :3, 'DEL', 'JFC', :4, SYSDATE)",
            customerID, jfcBranchNr, jfcCustomerNr, createUser)

        'TODO: Delete Customer (non-JFC) members based on each user's hideChat setting on the customer settings page
    End Sub


    '(3)/(4)/(5) Per-user chat cmd: DEL when hidden, ADD when un-hidden, UPD when name changes
    Public Sub InsertChatWebuserCmd(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String,
                                  ByVal jfcBranchNr As String, ByVal jfcCustomerNr As String, ByVal cmd As String,
                                  ByVal userType As String, ByVal firstName As String, ByVal lastName As String, ByVal createUser As String)
        DB.ExecuteNonQuery(
        "INSERT INTO chat_webuser_customer_cmd (cmd_id, email, customer_id, jfcBranchNr, jfcCustomerNr, cmd, userType, firstname, lastName, createUser, createDate) " &
        "VALUES (chat_webuser_customer_cmd_seq.NEXTVAL, :1, :2, :3, :4, :5, :6, :7, :8, :9, SYSDATE)",
        email, customerID, jfcBranchNr, jfcCustomerNr, cmd, userType, firstName, lastName, createUser)
    End Sub


    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
    End Sub

End Class