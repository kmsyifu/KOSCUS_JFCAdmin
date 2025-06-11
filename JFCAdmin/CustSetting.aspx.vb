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
            grdUser.Columns(5).Visible = False
        Else
            grdUser.Columns(5).Visible = True
        End If

        If lstHidePaymentPage.Text = "Hide" Then
            grdUser.Columns(6).Visible = False
        Else
            grdUser.Columns(6).Visible = True
        End If
        If lstHidePrice.Text = "Hide" Then
            grdUser.Columns(7).Visible = False
        Else
            grdUser.Columns(7).Visible = True

        End If
        If lstAllowCustOrder.Text = "Allow" Then
            grdUser.Columns(4).Visible = True
        Else
            grdUser.Columns(4).Visible = False
        End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim SQL As String
        Dim email As String
        'Dim a = HiddevActiveTab.Value
        'ClientScript.RegisterStartupScript(Me.GetType(), "selecttab", "$('#tabs').tabs({ selected: " + HiddevActiveTab.Value + " });", True)
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
                'ViewState("CUSTSETTING_CMD") = "EDIT_BY_JFCADMIN"
                'ViewState("CUSTSETTING_CUSTCOMPANYID") = lstCustCompany.Text

                'Select tab
                setJavaScript(ViewState("CUSTSETTING_TAB"))

                'Get customer ID
                SQL = "SELECT c.customer_id FROM customer c, webuser_customer w WHERE c.customer_id = w.customer_id AND w.customer_id <> 'ADMIN' AND w.email = :1 AND w.custCompany_id = :2"
                lblCustomerID.Text = CString(DB.GetScalar(SQL, email, lstCustCompany.Text))
                'ViewState("CUSTSETTING_CUSTID") 
                'ViewState("CUSTOMER_INFO") = New CustomerInfo(lblCustomerID.Text)
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
                '--------------------------------------------------------------------------------------------------------------------
                'Customer info
                '--------------------------------------------------------------------------------------------------------------------
                'Show customer info
                showCustInfo()
                pnlCalendar.Visible = False

                '--------------------------------------------------------------------------------------------------------------------
                'Delivery Schedule
                '--------------------------------------------------------------------------------------------------------------------
                'Show dropdownlist

                'Common.ShowCustGrpList(DB, lstCustCompany.Text, lstCustGrp_schedule)
                ''Show customers
                'Common.ShowCustomerList(DB, lstCustGrp_schedule.Text, lstCust_schedule)
                ''Show shipping points
                Common.ShowShipPointList(DB, lblCustomerID.Text, lstShipPoint)

                'Show delivery Schedule
                showScheduleList()

                '--------------------------------------------------------------------------------------------------------------------
                'Order Form
                '--------------------------------------------------------------------------------------------------------------------
                'Disable lstAllExcepGrpItem 
                'Dim custIs = ViewState("CUSTOMER_INFO")
                If Common.IsExcepPriceCustomer(DB, lblCustomerID.Text) Then
                    lstAllExcepGrpItem.Enabled = True
                Else
                    lstAllExcepGrpItem.Enabled = False
                End If

                'ViewState("IsExcepPriceCustomer") = Common.IsExcepPriceCustomer(DB, ViewState("CUSTSETTING_CUSTID"))
            Else
                SetUpdateButton(True)
                'SetEnableButton()
            End If

            'btnUpdateChnage.CssClass = "midButton_gray"
            'btnUpdateChnage.Enabled = False

            'btnSaveCust.CssClass = "midButton_gray"
            'btnSaveCust.Enabled = False

            'btnUpdateSchedule.CssClass = "midButton_gray"
            'btnUpdateSchedule.Enabled = False

        End Using
    End Sub


    'Protected Sub ctrCustSetting_PreRender(sender As Object, e As System.EventArgs) Handles ctrCustSetting.PreRender
    '    'Select tab
    '    setJavaScript(ViewState("CUSTSETTING_TAB"))
    'End Sub


    Protected Sub lstCustCompany_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany.SelectedIndexChanged
        If lstCustCompany.Text <> "" Then
            lblTitle.Visible = False
            Alltab.Visible = True
        End If

        Response.Redirect("CustSetting.aspx?cust_company=" & lstCustCompany.Text & "&tab=" & ViewState("CUSTSETTING_TAB"))

    End Sub

    Protected Sub btnSearch_order_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchCust.Click

        'For Each d In lstCustCompany.Items
        '    Dim a = d
        'Next
        If lstCustCompany.Items.FindByValue(txtJfcBranchNr.Text & txtJfcCustomerNr.Text) Is Nothing Then
            MessageBox("Portal account for " & txtJfcBranchNr.Text & " - " & txtJfcCustomerNr.Text.ToUpper & " doesn't exist. Please use the ""Register Customer"" button to register.", "Error!")
            Exit Sub
        End If


        Response.Redirect("CustSetting.aspx?cust_company=" & txtJfcBranchNr.Text & txtJfcCustomerNr.Text.ToUpper & "&tab=" & ViewState("CUSTSETTING_TAB"))
    End Sub



    'Protected Sub btnRefresh_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnRefresh.Click
    '    Response.Redirect("CustSetting.aspx?cust_company=" & lstCustCompany.Text & "&tab=" & ViewState("CUSTSETTING_TAB"))
    'End Sub

    Private Sub setJavaScript(ByVal tab As CustSetting_Tab)
        JavaScript = "$('#tabs').tabs('select', " & convertToLocalTab(tab) & ");" 'selectSubmenu(" & tab & ");"
        'JavaScript &= "scrollTo(0, 0);" 'TODO: Doesn't work
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

    '--------------------------------------------------------------------------------------------------------------------
    'Customer info
    '--------------------------------------------------------------------------------------------------------------------
    'TODO: Need to implement by YI FU
    'Protected Sub BtnUpdateChange_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnUpdateChnage.Click
    '    Dim customerID As String = ViewState("CUSTSETTING_CUSTID")
    '    'RegularExpressionValidator1.Validate()
    '    'If RegularExpressionValidator1.IsValid Then




    '    Using DB As New DBHelper
    '        setJavaScript(CustSetting_Tab.CUST)
    '        Dim sql As String = "select count(*) from customer_deliveryDate WHERE CUSTOMER_ID = :1 AND deliveryDate >= TRUNC(SYSDATE)"
    '        Dim num As Integer = Val(DB.GetScalar(sql, customerID))
    '        If lstDeliveryAnytime.Text = "Yes" And num > 0 Then
    '            setJavaScript(CustSetting_Tab.CUST)
    '            ViewState("DIALOG_CMD") = "DELETE_ALLSCHEDULE"
    '            ViewState("DIALOG_ARG1") = ""
    '            MessageBoxYesNo("Your Delivery Anytime is set as Yes. Do you want to delete the delivery schedule? If not, your Delivery Anytime will be set as No.", "Confirm Deletion", DB)
    '        End If
    '        Dim AllowCustOrder = IIf(CString(lstAllowCustOrder.SelectedValue) = "Allow", "Y", "")
    '        Dim HideItemPage = IIf(CString(lstHideItemPage.SelectedValue) = "Hide", "Y", "")
    '        Dim HidePaymentPage = IIf(CString(lstHidePaymentPage.SelectedValue) = "Hide", "Y", "")
    '        Dim HidePrice = IIf(CString(lstHidePrice.SelectedValue) = "Hide", "Y", "")
    '        Dim anytimedelivery = IIf(CString(lstDeliveryAnytime.SelectedValue) = "Yes", "Y", "")
    '        Dim outofstock = IIf(CString(lstOutofStock.SelectedValue) = "Show", "Y", "")
    '        Dim ShowAllExcepGrpItem = IIf(CString(lstAllExcepGrpItem.SelectedValue) = "Yes", "Y", "")
    '        Dim Textphones = txtTextPhones.Text.TrimEnd(",")
    '        Dim shipVia = CString(txtShipvia.Text.Trim)
    '        UpdateCustSetting(DB, customerID, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice, anytimedelivery, Textphones, outofstock, ShowAllExcepGrpItem, shipVia)
    '        MessageLine("CONFIRM", "Successfully saved")
    '    End Using
    'End Sub


    Protected Sub btnSaveCust_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveCust.Click, btnUpdateChnage.Click, btnUpdateSchedule.Click


        For Each row As GridViewRow In grdActiveSchedule.Rows

            If row.RowType = DataControlRowType.DataRow Then
                Dim lstDeliveryMaster As DropDownList = CType(row.FindControl("lstDeliveryMaster"), DropDownList)

                'row.FindControl()
                If lstDeliveryMaster.Text = "" Then
                    MessageBox("Please select delivery schedule master", "Error")
                    Exit Sub
                End If


            End If
        Next



        Dim customerID As String = lblCustomerID.Text
        'RegularExpressionValidator1.Validate()
        'If RegularExpressionValidator1.IsValid Then



        Using DB As New DBHelper
            setJavaScript(CustSetting_Tab.CUST)
            Dim sql As String = "select count(*) from customer_deliveryDate WHERE CUSTOMER_ID = :1 AND deliveryDate >= TRUNC(SYSDATE)"
            Dim num As Integer = Val(DB.GetScalar(sql, customerID))
            If lstDeliveryAnytime.Text = "Yes" And num > 0 Then
                setJavaScript(CustSetting_Tab.CUST)
                ViewState("DIALOG_CMD") = "DELETE_ALLSCHEDULE"
                ViewState("DIALOG_ARG1") = ""
                MessageBoxYesNo("Your Delivery Anytime is set as Yes. Do you want to delete the delivery schedule? If not, your Delivery Anytime will be set as No.", "Confirm Deletion", DB)
            End If
            Dim AllowCustOrder = IIf(CString(lstAllowCustOrder.SelectedValue) = "Allow", "Y", "")
            Dim HideItemPage = IIf(CString(lstHideItemPage.SelectedValue) = "Hide", "Y", "")
            Dim HidePaymentPage = IIf(CString(lstHidePaymentPage.SelectedValue) = "Hide", "Y", "")
            Dim HidePrice = IIf(CString(lstHidePrice.SelectedValue) = "Hide", "Y", "")
            Dim anytimedelivery = IIf(CString(lstDeliveryAnytime.SelectedValue) = "Yes", "Y", "")
            Dim OutofStock As String = IIf(CString(lstOutofStock.SelectedValue) = "Show", "Y", "")
            Dim ShowAllExcepGrpItem As String = IIf(CString(lstAllExcepGrpItem.SelectedValue) = "Yes", "Y", "")
            Dim Textphones = txtTextPhones.Text.TrimEnd(",")

            Dim Shipvia = CString(txtShipvia.Text.Trim())
            Dim Shipdateoffset As Decimal = Val(txtShipDateOffset.Text)
            Dim minOrderAmt As Integer = Val(txtMinOrderAmt.Text)
            UpdateCustSetting(DB, customerID, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice, anytimedelivery, Textphones, OutofStock, ShowAllExcepGrpItem, Shipvia, Shipdateoffset, minOrderAmt)

            For i As Integer = 0 To grdUser.Rows.Count - 1
                Dim lblemail = CType(grdUser.Rows.Item(i).FindControl("lblEmail"), Label).Text
                Dim lstHideCustOrder = IIf(CType(grdUser.Rows.Item(i).FindControl("lstHideCustOrder"), DropDownList).SelectedValue = "Y", "Y", "")
                Dim lstHideItemPage = If(HideItemPage = "Y", Nothing, IIf(CType(grdUser.Rows.Item(i).FindControl("lstHideItemPage"), DropDownList).SelectedValue = "Y", "Y", ""))
                Dim lstHidePrice = If(HidePrice = "Y", Nothing, IIf(CType(grdUser.Rows.Item(i).FindControl("lstHidePrice"), DropDownList).SelectedValue = "Y", "Y", ""))
                Dim lstHidePaymentPage = If(HidePaymentPage = "Y", Nothing, IIf(CType(grdUser.Rows.Item(i).FindControl("lstHidePaymentPage"), DropDownList).SelectedValue = "Y", "Y", ""))
                Dim txtCell = CType(grdUser.Rows.Item(i).FindControl("txtCell"), TextBox).Text
                Dim FirstName = CType(grdUser.Rows.Item(i).FindControl("txtfirstName"), TextBox).Text
                Dim lastName = CType(grdUser.Rows.Item(i).FindControl("txtlastName"), TextBox).Text

                UpdateUserInList(DB, lblemail, customerID, txtCell, FirstName, lastName, lstHideCustOrder, lstHideItemPage, lstHidePaymentPage, lstHidePrice)

            Next
            '        'txtEaPerCs = grdCustItem.Rows.Item(i).FindControl("txtEaPerCs")
            '        txtCsPrice = grdCustItem.Rows.Item(i).FindControl("txtCsPrice")
            '        txtEaPrice = grdCustItem.Rows.Item(i).FindControl("txtEaPrice")
            '        txtLbPrice_wgt = grdCustItem.Rows.Item(i).FindControl("txtlbPrice_orderByWgt")
            '        txtLbPrice_qty = grdCustItem.Rows.Item(i).FindControl("txtlbPrice_orderByQty")
            '        If (txtCsPrice.Visible = True AndAlso IsNumber(txtCsPrice.Text, True) = False) OrElse (txtEaPrice.Visible = True AndAlso IsNumber(txtEaPrice.Text, True) = False) _
            '                    OrElse (txtLbPrice_wgt.Visible = True AndAlso IsNumber(txtLbPrice_wgt.Text, True) = False) OrElse (txtLbPrice_qty.Visible = True AndAlso IsNumber(txtLbPrice_qty.Text, True) = False) Then
            '            MessageBox("Each/case and price has to be numeric", "Error", mDB)
            '            Exit Sub
            '        End If
            '    Next
            MessageLine("CONFIRM", "Successfully saved")
        End Using



        For Each row As GridViewRow In grdActiveSchedule.Rows

            If row.RowType = DataControlRowType.DataRow Then
                Dim lstDeliveryMaster As DropDownList = CType(row.FindControl("lstDeliveryMaster"), DropDownList)
                'If lstDeliveryMaster.SelectedIndex = 0 Then

                '    Exit Sub
                'End If



                'Dim masterValue As String = CType(row.FindControl("lnkdeliveryMasterName"), LinkButton).Text
                Dim lnkdeliveryMasterName As LinkButton = CType(row.FindControl("lnkdeliveryMasterName"), LinkButton)
                Dim DeliverOn As String = row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text
                Dim Frequency As String = row.Cells(GetColumnIndexByName(row, "FREQUENCY")).Text
                Dim Orderby As String = row.Cells(GetColumnIndexByName(row, "orderByDayTime")).Text
                Dim IsReminder As Boolean = CType(row.FindControl("ChkSendReminder"), CheckBox).Checked
                Dim IsEmail As Boolean = CType(row.FindControl("ckbEmail"), CheckBox).Checked
                'Dim lblDeliveryOn As String = CType(row.FindControl("lnkdeliveryMasterName"), DropDownList).Text

                SaveSchedule(lnkdeliveryMasterName.CommandArgument, lstDeliveryMaster, Frequency, DeliverOn, Orderby, IsReminder, IsEmail)
                'row.FindControl()



            End If
        Next
        btnUpdateChnage.CssClass = "midButton_gray"
        btnUpdateChnage.Enabled = False

        btnSaveCust.CssClass = "midButton_gray"
        btnSaveCust.Enabled = False

        btnUpdateSchedule.CssClass = "midButton_gray"
        btnUpdateSchedule.Enabled = False
        'SaveSchedule(TotalDeliverySchedID As String, lstDeliveryMaster As DropDownList, Frequency As String, DeliveryOn As String, orderby As String, IsReminder As Boolean, IsEmail As Boolean)
        'End If
    End Sub

    Protected Sub lstHide_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstHideItemPage.SelectedIndexChanged, lstHidePaymentPage.SelectedIndexChanged, lstHidePrice.SelectedIndexChanged, lstAllowCustOrder.SelectedIndexChanged

        Using DB As New DBHelper

            Dim customerID As String = lblCustomerID.Text
            'Show user list for the customer
            Dim Sql = "SELECT u.email,u.CELLULAR,u.firstName,u.lastName, c.hideCustOrder, c.hideItemPage, c.hidePaymentPage, c.hidePrice " & _
                       "FROM webuser u, webuser_customer c " & _
                       "LEFT OUTER JOIN UserRemovalRequest ur ON ur.email = c.email AND ur.jfcBranchNr || ur.jfcCustomerNr = c.custCompany_id " &
                       " WHERE u.email = c.email " & _
                       " AND c.userCompany_id = c.custCompany_id " & _
                       " AND c.customer_id = :1 and (ur.status <> 'N' or ur.status is null) " & _
                       "ORDER BY email"
            'IIf(ViewState("CUSTSETTING_CMD") = "EDIT_BY_CUSTADMIN", " AND u.email NOT IN (SELECT email FROM webUser_vendor WHERE vendorCompany_id = 'JFC')", "") & _
            Dim dt = DB.GetDataTable(Sql, customerID)
            'ViewState("GRD_USER") = dt
            grdUser.DataSource = dt
            grdUser.DataBind()
        End Using
        CheckUserHideColumn()


        'showCustInfo()
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
                Dim Sql = "SELECT c.Name FROM  jho.customer@jho c " & _
                   " WHERE c.bid = :1 AND c.customerid = :2"
                Dim r = db.GetReader(Sql, txtPopJFCAccount.Text, txtPopJFCNum.Text)
                If r.Read() Then
                    lblPopCustomerName.Text = CString(r("Name"))

                End If
            End Using
        End If
        popDialog_User.Show()
        'If ViewState("GRD_USER") IsNot Nothing Then
        'Dim dtCurTable As DataTable = DirectCast(ViewState("GRD_USER"), DataTable)
        'Dim drCurRow As DataRow = Nothing

        'If dtCurTable.Rows.Count > 0 Then
        '    For i As Integer = 0 To dtCurTable.Rows.Count - 1
        '        'Dim lst As DropDownList = DirectCast(grdUser.Rows(i).Cells(0).FindControl("lstEmail"), DropDownList)
        '        Dim lbl As Label = DirectCast(grdUser.Rows(i).Cells(0).FindControl("lblEmail"), Label)
        '        Dim lbl1 As Label = DirectCast(grdUser.Rows(i).Cells(1).FindControl("lblFirstName"), Label)
        '        Dim lbl2 As Label = DirectCast(grdUser.Rows(i).Cells(2).FindControl("lblLastName"), Label)
        '        Dim lbl3 As Label = DirectCast(grdUser.Rows(i).Cells(3).FindControl("lblPhone"), Label)
        '        Dim lbl4 As Label = DirectCast(grdUser.Rows(i).Cells(4).FindControl("lblCellular"), Label)
        '        Dim lbl5 As Label = DirectCast(grdUser.Rows(i).Cells(5).FindControl("lblFax"), Label)
        '        drCurRow = dtCurTable.NewRow()
        '        'dtCurTable.Rows(i)("email") = lst.Text
        '        dtCurTable.Rows(i)("email") = lbl.Text
        '        dtCurTable.Rows(i)("firstName") = lbl1.Text
        '        dtCurTable.Rows(i)("lastName") = lbl2.Text
        '        dtCurTable.Rows(i)("phone") = lbl3.Text
        '        dtCurTable.Rows(i)("cellular") = lbl4.Text
        '        dtCurTable.Rows(i)("fax") = lbl5.Text
        '    Next

        '    dtCurTable.Rows.Add(drCurRow)
        '    ViewState("GRD_USER") = dtCurTable
        '    grdUser.DataSource = dtCurTable
        '    grdUser.DataBind()
        'End If
        'Else
        'Response.Write("ViewState 'GRD_USER' is null")
        'End If
        'setUserPrevData()
    End Sub

    'Protected Sub btnInfo_Click(sender As Object, e As System.EventArgs) Handles btnInfo_jfcEmail.Click
    '    MessageBox(sender.ToolTip, "Information")
    'End Sub

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
                pnlCustInfo.Height = 400
                pnlCustSetting.Width = 500
                pnlCustSetting.Height = 400
            End If

            Dim autoPayTypeALL = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_ALL")
            Dim autoPayTypeNONLIQ = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_NONLIQ")
            Dim autoPayTypeBEER = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_BEER")
            Dim autoPayTypeWINE = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_WINE")
            Dim autoPayTypeLIQUOR = GetCustAutoPayType(DB, customerID, "AUTOPAYTYPEID_LIQUOR")

            'Show dropdownlist (TODO: Complete later)
            ShowCodeMasterListLocal(DB, "ALLOWNOT", lstAllowCustOrder, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHideItemPage, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHidePaymentPage, True)
            ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lstHidePrice, True)
            ShowCodeMasterListLocal(DB, "HIDE_N_SHOW_Y", lstOutofStock, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstAllExcepGrpItem, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstDeliveryAnytime, True)

            'Show customer info and settings
            SQL = "SELECT c.* FROM customer c WHERE c.customer_id = :1"
            r = DB.GetReader(SQL, customerID)
            If r.Read() Then
                'Customer info
                'lblCustomerID.Text = CString(r("customer_id"))
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
                'lblAutoPayType.Text = CString(r("AutoPayType"))
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

                lblWebDiscType.Text = CString(r("WebDiscType"))

                lblJfcPhone.Text = foldlongString(CString(r("jfcPhone")), New Char() {","})
                'Customer settings
                lstAllowCustOrder.Text = IIf(CString(r("allowCustOrder")) = "Y", "Allow", "Not Allow")
                lstHideItemPage.Text = IIf(CString(r("hideItemPage")) = "Y", "Hide", "Show")
                lstHidePaymentPage.Text = IIf(CString(r("hidePaymentPage")) = "Y", "Hide", "Show")
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
            SQL = "SELECT shipName, address1, address2, city, state, zip, country " & _
                       "FROM customer_shipPoint " & _
                       "WHERE customer_id = " & DB.SqlEsc(customerID) & " " & _
                       "ORDER BY shipName"
            dsShipPoint.ProviderName = "System.Data.OracleClient"
            dsShipPoint.ConnectionString = DB.GetConnectionString
            dsShipPoint.SelectCommand = SQL
            grdShipPoint.DataSourceID = "dsShipPoint"
            grdShipPoint.DataBind()

            'Show user list for the customer
            SQL = "SELECT u.email,u.CELLULAR,u.firstName,u.lastName, c.hideCustOrder, c.hideItemPage, c.hidePaymentPage, c.hidePrice " & _
                       "FROM webuser u, webuser_customer c " & _
                       "LEFT OUTER JOIN UserRemovalRequest ur ON ur.email = c.email AND ur.jfcBranchNr || ur.jfcCustomerNr = c.custCompany_id " &
                       " WHERE u.email = c.email " & _
                       " AND c.userCompany_id = c.custCompany_id " & _
                       " AND c.customer_id = :1 AND (ur.status <> 'N' or ur.status is null) " & _
                       "ORDER BY email"
            'IIf(ViewState("CUSTSETTING_CMD") = "EDIT_BY_CUSTADMIN", " AND u.email NOT IN (SELECT email FROM webUser_vendor WHERE vendorCompany_id = 'JFC')", "") & _
            dt = DB.GetDataTable(SQL, customerID)
            'ViewState("GRD_USER") = dt
            grdUser.DataSource = dt
            grdUser.DataBind()
            'If lstHideItemPage.Text = "Hide" Then
            '    grdUser.Columns(5).Visible = False
            'Else
            '    grdUser.Columns(5).Visible = True
            'End If

            'If lstHidePaymentPage.Text = "Hide" Then
            '    grdUser.Columns(6).Visible = False
            'Else
            '    grdUser.Columns(6).Visible = True
            'End If
            'If lstHidePrice.Text = "Hide" Then
            '    grdUser.Columns(7).Visible = False

            'Else
            '    grdUser.Columns(7).Visible = True
            '    'grdUser.RowHeaderColumn
            'End If
            'If lstAllowCustOrder.Text = "Allow" Then
            '    grdUser.Columns(4).Visible = True
            'Else
            '    grdUser.Columns(4).Visible = False
            'End If
            CheckUserHideColumn()
            'If grdUser.Rows.Count = 0 Then setUserInitialRow()
            If lstDeliveryAnytime.SelectedValue = "Yes" Then
                pnlSchedule.Visible = False
            Else
                pnlSchedule.Visible = True
            End If
        End Using
    End Sub



    'Protected Sub lstCustGrp_schedule_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCustGrp_schedule.SelectedIndexChanged
    '    Using DB As New DBHelper
    '        setJavaScript(CustSetting_Tab.SCHEDULE)
    '        'Show customer and ship point list
    '        Common.ShowCustomerList(DB, lstCustGrp_schedule.Text, lstCust_schedule)
    '        Common.ShowShipPointList(DB, lstCust_schedule.Text, lstShipPoint)
    '        'Show schedule
    '        showScheduleList()
    '    End Using
    'End Sub

    'Protected Sub lstCust_schedule_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCust_schedule.SelectedIndexChanged
    '    Using DB As New DBHelper
    '        setJavaScript(CustSetting_Tab.SCHEDULE)
    '        'Show customer and ship point list
    '        Common.ShowShipPointList(DB, lstCust_schedule.Text, lstShipPoint)
    '        showScheduleList()
    '    End Using
    'End Sub

    Protected Sub lstShipPoint_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstShipPoint.SelectedIndexChanged
        'setJavaScript(CustSetting_Tab.SCHEDULE)
        showScheduleList()
        SetUpdateButton(False)
    End Sub
    Private MonthData As DataTable
    Protected Sub cldSchedule_DayRender(sender As Object, e As System.Web.UI.WebControls.DayRenderEventArgs) Handles cldSchedule.DayRender

        'Using DB As New DBHelper

        '    ShowDayEventInCalendar(DB, Page, e, lblCustomerID.Text, lstShipPoint.Text, Not ViewState("JFC_ADMINGROUP"), lnkDelivery)
        'End Using

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

        'Dim startDate As DateTime = New DateTime(currentDate.Year, currentDate.Month, 1)
        Dim endDate As DateTime = currentDate.AddMonths(1).AddDays(15)

        ' Execute the query for the entire month range
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
            SQL = "SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = :1 " 'AND vendorName = :2
            custGrpVendorID = CString(DB.GetScalar(SQL, lstCustCompany.Text))

            'Show vendors
            SQL = "SELECT custGrpVendor_id, vendorName " & _
                    "FROM custGrp_vendor " & _
                    "WHERE custGrp_id = :1 " & _
                    "ORDER BY vendorName"
            'IIf(ViewState("CUSTSETTING_CMD") <> "EDIT_BY_JFCADMIN", " AND vendor_id NOT IN (SELECT vendor_id FROM jfcVendor) ", " ") & _
            dt = DB.GetDataTable(SQL, lstCustCompany.Text) 'lstCustCompany.Text
            dr = dt.NewRow
            'dr("vendorName") = LST_CMD_STR
            'dt.Rows.InsertAt(dr, 0)
            lstPopCustGrpVendor.DataSource = dt
            lstPopCustGrpVendor.DataValueField = "custGrpVendor_id"
            lstPopCustGrpVendor.DataTextField = "vendorName"
            lstPopCustGrpVendor.DataBind()
            'SelectIfOneItem(lstPopCustGrpVendor)

            ShowCodeMasterListLocal(DB, "TIMEOFDAY", lstPopOrderByTime, True)
            ShowCodeMasterListLocal(DB, "AMPM", lstPopOrderByAmPm, True)
            ShowCodeMasterListLocal(DB, "YESNO", lstPopSendReminder, True)

            'Prepare popup
            pnlPopDtlEdit_schedule.Visible = True
            'pnlPopDtlView_schedule.Visible = False
            btnPopDeleteDelivery.Visible = True
            InvisibleCtrl({lstPopCustGrpVendor})
            InvisibleCtrl({lblPopStarCustGrpVendor, lblPopVendorName})
            ClearCtrl({txtPopDeliveryDate, txtPopOrderByDate})
            ClearCtrl({lblPopCustGrpVendorID, lblPopVendorName, lblPopSchedule}) ', lblPopVendorName2, lblPopDeliveryDate, lblPopOrderByDate, lblPopSendReminder,  lblPopSchedule2})
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
                'If ViewState("CUSTSETTING_CMD") <> "EDIT_BY_JFCADMIN" AndAlso IsJfcBranch(DB, custGrpVendorID) = True Then
                '    lblPopHeader_schedule.Text = "View"
                '    pnlPopDtlEdit_schedule.Visible = False
                '    pnlPopDtlView_schedule.Visible = True
                'Else
                lblPopHeader_schedule.Text = "Edit"
                btnPopDeleteDelivery.Visible = True
                btnPopSaveDelivery.Text = "Update"
                VisibleCtrl({lblPopVendorName})
                'End If

                'Get delivery info
                SQL = "SELECT d.custGrpVendor_id, TO_CHAR(d.deliveryDate, :1) deliveryDate, TO_CHAR(d.orderByDate, :2) orderByDate, " & _
                        "TO_CHAR(d.orderByDate, :3) orderByTime, d.sendReminder, s.deliverySched_id, s.frequency, s.deliveryDay, s.orderByDay, s.orderByTime " & _
                        "FROM customer_deliveryDate d " & _
                        "LEFT OUTER JOIN customer_deliverySched s ON s.deliverySched_id = d.deliverySched_id " & _
                        "WHERE d.customer_id = :4 AND d.shipPoint_id = :5" & _
                        "  AND d.custGrpVendor_id = :6 AND d.deliveryDate = TO_DATE(:7, :8)"
                r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, TIME_SQLFMT, lblCustomerID.Text, lstShipPoint.Text, custGrpVendorID, dateStr, DATE_FMT)
                If r.Read() Then
                    lblPopVendorName.Text = vendorName
                    'lblPopVendorName2.Text = vendorName
                    lblPopCustGrpVendorID.Text = CString(r("custGrpVendor_id"))
                    txtPopDeliveryDate.Text = CString(r("deliveryDate"))
                    lblPopOldDeliveryDate.Text = CString(r("deliveryDate"))
                    'lblPopDeliveryDate.Text = CString(r("deliveryDate"))
                    txtPopOrderByDate.Text = CString(r("orderByDate"))
                    lstPopOrderByTime.Text = Left(CString(r("orderByTime")), 5)
                    lstPopOrderByAmPm.Text = Right(CString(r("orderByTime")), 2)
                    ViewState("oldDeliveryOrderDate") = CString(r("orderByDate"))
                    ViewState("oldDeliveryOrderTime") = CString(r("orderByTime"))
                    'lblPopOrderByDate.Text = CString(r("orderByDate")) & " " & CString(r("orderByTime"))
                    lstPopSendReminder.Text = IIf(CString(r("sendReminder")) = "Y", "Yes", "No")
                    'lblPopSendReminder.Text = IIf(CString(r("sendReminder")) = "Y", "Yes", "No")
                    lblPopSchedule.Text = IIf(CString(r("deliverySched_id")) = "", "N/A", _
                                              CString(r("frequency")) & ", " & "Delivery on " & CString(r("deliveryDay")) & ", " & "Order by " & CString(r("orderByDay")) & " " & CString(r("orderByTime")))
                    'lblPopSchedule2.Text = lblPopSchedule.Text
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

    'Protected Sub btnPopClose_Item_Click(sender As Object, e As System.EventArgs) Handles btnPopClose_Item.Click
    '    setJavaScript(CustSetting_Tab.CUSTITEM)
    'End Sub

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
                        'Dear Your delivery info has been updated Old Delivery
                        emailBody = rm.GetString("Dear", ci) & " " & CString(dr("customerName")) & " (" & CString(dr("jfcBranchNr")) & "-" & CString(dr("jfcCustomerNr")) & _
                                    "), <br />  <br /> " & rm.GetString("Yourdeliveryinfohasbeenupdated", ci) & "  <br /> <br />" & _
                                    rm.GetString("Old", ci) & " <br />" & rm.GetString("Delivery", ci) & ": " & lblPopOldDeliveryDate.Text & "<br />" & _
                                    rm.GetString("Deadline", ci) & ":  " & ViewState("oldDeliveryOrderDate") & " " & ViewState("oldDeliveryOrderTime") & "<br /><br />" & _
                                    rm.GetString("New", ci) & " <br /><span style=""font-weight:bold""> " & rm.GetString("Delivery", ci) & ": " & txtPopDeliveryDate.Text & " <br />" & _
                                    rm.GetString("Deadline", ci) & ": " & txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text & _
                                    "</span><br /> <br />   " & rm.GetString("Bestregards", ci) & ",<br />" & Common.JFC_COMPANY_NAME
                        'CreateEmailBody(Page, emailBody, "MASTER", deliverySchedID & deliveryschedlinenr, newSchedule)
                        Dim errmsg As String = ""
                        'CString(dr("customerEmail"))
                        If Common.SendMail(SYSTEM_EMAIL, IIf(IsProdBeta() Or IsProduction(), CString(dr("customerEmail")), "yfu@kmsnet.com"), VendorEmail, rm.GetString("Yourdeliveryinfohasbeenupdated", ci), emailBody, Nothing, True, errmsg) > 0 Then
                            'Email sent failed
                            'emailSent = False

                        End If
                    Next
                End If
                'If updated = False Then
                '    MessageBox("Cannot be " & cmd & ". Delivery date " & txtPopDeliveryDate.Text & " already exists for " & IIf(cmd = "added", lstPopCustGrpVendor.SelectedItem.Text, GetVendorName(DB, lblPopCustGrpVendorID.Text)), "Error", DB)
                '    Exit Sub
                'End If

                'Show transaction month
                'DateTime.TryParseExact(txtPopDeliveryDate.Text, DATE_FMT, Nothing, Nothing, dt)
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

            'Show transaction month
            DateTime.TryParseExact(txtPopDeliveryDate.Text, DATE_FMT, Nothing, Nothing, dt)
            'showScheduleList()
            InsertEventLog(DB, "Deleted delivery date", txtPopDeliveryDate.Text, Nothing, Nothing, True)
            MessageLine("CONFIRM", "Successfully deleted")
        End Using
    End Sub

    Protected Sub cldSchedule_VisibleMonthChanged(sender As Object, e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles cldSchedule.VisibleMonthChanged
        'setJavaScript(CustSetting_Tab.SCHEDULE)
    End Sub



    Protected Sub grdActiveSchedule_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdActiveSchedule.RowCommand
        'setJavaScript(CustSetting_Tab.SCHEDULE)
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
                'SetEnableButton()      'showScheduleList()
            End If
        End Using
    End Sub

    Protected Sub btnAddschedule_Click(sender As Object, e As System.EventArgs) Handles btnAddSchedule.Click

        'Dim row = CType(grdActiveSchedule.Rows(0), GridViewRow)
        Dim dt As DataTable = GetTableWithNodata()
        Dim dr As DataRow
        Using db As New DBHelper
            'Dim Sql = "select anytimedelivery from customer where customer_id = :1"
            Dim anytime As String = lstDeliveryAnytime.Text 'CString(db.GetScalar(Sql, lblCustomerID.Text))

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
                'dsActiveSchedule
                Dim dv As New DataView(dt)
                'dv.Sort
                'grdActiveSchedule.Columns.Clear()
                grdActiveSchedule.DataSourceID = ""
                grdActiveSchedule.DataSource = dt

                'grdActiveSchedule.Sort("STARTDATE", SortDirection.Descending)
                grdActiveSchedule.DataBind()
                'showScheduleDtl("")
            End If
        End Using
        'If lstDeliveryAnytime.Text = "Yes" Then
        '    MessageBox("Delivery Anytime has been set. No need to setup delivery schedule.", "Error")
        'Else

        'End If

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
    'Protected Sub lnkBack_schedule_Click(sender As Object, e As System.EventArgs) Handles lnkBack_schedule.Click
    '    setJavaScript(CustSetting_Tab.SCHEDULE)
    '    'Return back to order form list
    '    pnlList_schedule.Visible = True
    '    pnlDtl_schedule.Visible = False
    '    showScheduleList()
    'End Sub

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
                'Dim lblDeliveryOn As String = CType(row.FindControl("lnkdeliveryMasterName"), DropDownList).Text


                'If masterValue = lstDeliveryMaster.SelectedItem.ToString() Then
                '    MessageBox("This delivery schedule master name already exists, Please enter another one.", "Error")
                '    'lstDeliveryMaster.SelectedIndex = 0
                '    Exit Sub
                'End If
                If String.Compare(lstDeliveryMasterID, (CType(row.FindControl("lstDeliveryMaster"), DropDownList)).ClientID, True) = 0 Then

                    If lstDeliveryMaster.SelectedValue <> "" Then
                        Dim SQL As String
                        '    'Dim customerID As String

                        Using DB As New DBHelper

                            SQL = "SELECT jd.deliveryMaster_id, jd.frequency, jd.deliveryDay, jd.orderByDay || ' ' || jd.orderByTime AS orderByDayTime, " & _
                                    " c.deliverySched_id, c.customer_id, c.shipPoint_id, c.sendReminder, to_char(NVL(c.startDate, SYSDATE), " & DB.SqlEsc(DATE_FMT) & ") startDate, to_char(c.endDate, " & DB.SqlEsc(DATE_FMT) & ") endDate " & _
                                    "FROM JFCDeliveryMaster jd, customer_deliverySched c " & _
                                    "WHERE jd.deliveryMaster_id = c.deliveryMaster_id (+) " & _
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
                        'row.FindControl()
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

    Protected Sub chkShowAll2_schedule_CheckedChanged(sender As Object, e As System.EventArgs) 'TODO Handles chkShowAll2_schedule.CheckedChanged
        setJavaScript(CustSetting_Tab.SCHEDULE)
        'chkShowAll_schedule.Checked = chkShowAll2_schedule.Checked 'If uncomment, must refresh grdActiveSchedule 
        If lstDeliveryAnytime.Text = "Yes" Then
            MessageBox("Delivery Anytime, no need to setup delivery schedule.", "Error")
        Else
            'showScheduleDtl("")
        End If

    End Sub

    'Protected Sub grdMultiSchedule_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdMultiSchedule.RowDataBound
    '    Dim lst As DropDownList
    '    Dim txt As TextBox
    '    Dim SQL As String
    '    Dim dt As DataTable
    '    Dim dr As DataRow
    '    Dim custGrpID As String

    '    Using DB As New DBHelper
    '        If e.Row.RowType = DataControlRowType.DataRow Then
    '            'Show vendors
    '            custGrpID = CType(e.Row.FindControl("lblCustGrpID"), Label).Text
    '            lst = CType(e.Row.FindControl("lstCustGrpVendor"), DropDownList)
    '            SQL = "SELECT custGrpVendor_id, vendorName " & _
    '                    "FROM custGrp_vendor " & _
    '                    "WHERE custGrp_id = :1 " & _
    '                    "ORDER BY vendorName"
    '            dt = DB.GetDataTable(SQL, custGrpID)
    '            dr = dt.NewRow
    '            dr("vendorName") = ""
    '            dt.Rows.InsertAt(dr, 0)
    '            lst.DataSource = dt
    '            lst.DataValueField = "custGrpVendor_id"
    '            lst.DataTextField = "vendorName"
    '            lst.DataBind()
    '            SelectIfOneItem(lst)

    '            'Show frequency, delivery day, order by day (Monday, Tuesday, ...) and time
    '            lst = CType(e.Row.FindControl("lstFrequency"), DropDownList)
    '            ShowCodeMasterList(DB, "DELIVERY_FREQ", lst, True, True)
    '            lst.Text = "Weekly" 'Set weekly by default
    '            lst = CType(e.Row.FindControl("lstDeliveryDay"), DropDownList)
    '            ShowCodeMasterList(DB, "DAYOFWEEK", lst, True, True)
    '            lst = CType(e.Row.FindControl("lstOrderByDay"), DropDownList)
    '            ShowCodeMasterList(DB, "DAYOFWEEK", lst, True, True)
    '            lst = CType(e.Row.FindControl("lstOrderByDiff"), DropDownList)
    '            ShowCodeMasterList(DB, "DAYDIFF", lst, True, True)
    '            lst.Visible = False 'Make day difference invisible by default
    '            lst = CType(e.Row.FindControl("lstOrderByTime"), DropDownList)
    '            ShowCodeMasterListLocal(DB, "TIMEOFDAY", lst, True)
    '            lst = CType(e.Row.FindControl("lstOrderByAmPm"), DropDownList)
    '            ShowCodeMasterListLocal(DB, "AMPM", lst, True)
    '            lst = CType(e.Row.FindControl("lstSendReminder"), DropDownList)
    '            ShowCodeMasterListLocal(DB, "YESNO", lst, True)
    '            txt = CType(e.Row.FindControl("txtStartDate"), TextBox)
    '            txt.Text = Format(Date.Today, Common.DATE_FMT)
    '        End If
    '    End Using
    'End Sub


    'Not used
    Protected Sub lstGrdFreq_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim lstFrequency, lstDeliveryDay, lstOrderByDay, lstOrderByDiff As DropDownList
        'Dim gvrow As GridViewRow = CType(sender, DropDownList).NamingContainer
        'Dim rowindex As Integer = CType(gvrow, GridViewRow).RowIndex

        ''Get list
        'lstFrequency = grdMultiSchedule.Rows.Item(rowindex).FindControl("lstFrequency")
        'lstDeliveryDay = grdMultiSchedule.Rows.Item(rowindex).FindControl("lstDeliveryDay")
        'lstOrderByDay = grdMultiSchedule.Rows.Item(rowindex).FindControl("lstOrderByDay")
        'lstOrderByDiff = grdMultiSchedule.Rows.Item(rowindex).FindControl("lstOrderByDiff")

        ''Set properties
        'InvisibleCtrl({lblTitleDeliveryDay, lblStarDeliveryDay})
        'InvisibleCtrl({lstDeliveryDay, lstOrderByDay, lstOrderByDiff})
        'Select Case lstFrequency.Text.ToUpper
        '    Case "DAILY"
        '        VisibleCtrl({lstOrderByDiff})
        '    Case Else
        '        VisibleCtrl({lstDeliveryDay, lstOrderByDay})
        'End Select
    End Sub


    Protected Sub SaveSchedule(TotalDeliverySchedID As String, lstDeliveryMaster As DropDownList, Frequency As String, DeliveryOn As String, orderby As String, IsReminder As Boolean, IsEmail As Boolean)
        Dim SQL As String
        Dim newSchedule As Boolean '= pnlMultiSchedule.Visible
        Dim startDateVal As Date
        Dim endDateVal As Date = Nothing
        Dim startDateStr As String
        Dim endDateStr As String
        Dim sendReminder As String
        Dim lst As DropDownList
        Dim txt As TextBox
        Dim r As DataReader
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
            SQL = "SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = :1 " 'AND vendorName = :2
            Dim custGrpVendorID = CString(DB.GetScalar(SQL, lstCustCompany.Text))
            'Edit new or existing schedule
            'Check error  --ask
            'If lstCustGrpVendor_schedule.Text = "" Then
            '    MessageBox("Please select a vendor", "Error", DB)

            '    Exit Sub
            'End If


            'For Each row As GridViewRow In grdActiveSchedule.Rows
            '    Dim lnkBtn, lnkmaster As LinkButton
            '    lnkBtn = row.Cells(1).FindControl("lnkShipName")
            '    'Dim DELIVERYDAY As String = row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text
            '    lnkmaster = row.Cells(3).FindControl("lnkdeliveryMasterName")
            '    If lnkBtn.CommandArgument <> lblDeliverySchedID.Text Then
            '        If lnkmaster.Text = lstDeliveryMaster.SelectedItem.ToString() Then
            '            MessageBox("This delivery schedule master name already exists, Please enter another one.", "Error", DB)
            '            Exit Sub
            '        End If

            '        If lnkmaster.Text.Contains(lblDeliveryOn.Text) Then
            '            MessageBox("This delivery day already exists, Please select another one.", "Error", DB)
            '            Exit Sub
            '        End If

            '    End If

            'Next


            DB.BeginTransaction()

            If newSchedule = False Then
                'Delete delivery schedule and the dates
                DeleteDeliverySched(DB, deliverySchedID, deliveryschedlinenr, False)
            End If


            'txt = grdDeliveryMaster.Rows.Item(i).FindControl("txtStartDate")
            startDateStr = Format(Date.Today, Common.DATE_FMT)
            DateTime.TryParseExact(startDateStr, DATE_FMT, Nothing, Nothing, startDateVal)
            'txt = grdDeliveryMaster.Rows.Item(i).FindControl("txtEndDate")
            endDateStr = ""
            If endDateStr <> "" Then DateTime.TryParseExact(endDateStr, DATE_FMT, Nothing, Nothing, endDateVal)
            'lst = lstSendReminder
            sendReminder = IIf(IsReminder, "Y", "N")

            If orderby <> "" Then
                Dim Orderbysplit = orderby.Split(" ")
                orderbydate = Orderbysplit(0)
                orderbytime = Orderbysplit(1) & " " & Orderbysplit(2)
            End If


            UpSertDeliverySched(DB, deliverySchedID, deliveryschedlinenr, lblCustomerID.Text, custGrpVendorID, lstShipPoint.Text, lstDeliveryMaster.Text, _
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
                    '2023
                    CreateEmailBody(Page, emailBody, "MASTER", deliverySchedID & deliveryschedlinenr, newSchedule)
                    Dim errmsg As String = ""
                    'CString(dr("customerEmail"))
                    '"Your delivery schedule was updated (#" & DB.NVL(deliverySchedID, "") & ")"
                    If Common.SendMail(SYSTEM_EMAIL, IIf(Common.IsDev(), "yfu@kmsnet.com, jzhou@kmsnet.com", CString(dr("customerEmail"))), VendorEmail, String.Format(rm.GetString("Yourdeliveryschedulewasupdated", ci), DB.NVL(deliverySchedID, "")), emailBody, Nothing, True, errmsg) > 0 Then
                        'Email sent failed
                        'emailSent = False
                    Else
                        'Email sent OK
                        'emailSent = True
                    End If
                Next
                MessageLine("CONFIRM", "Successfully saved and send email to customer")
            Else
                MessageLine("CONFIRM", "Successfully saved")
            End If
            showScheduleList()
        End Using
    End Sub

    'Protected Sub lstFrequency_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstFrequency.SelectedIndexChanged
    '    setJavaScript(CustSetting_Tab.SCHEDULE)
    '    InvisibleCtrl({lblTitleDeliveryDay, lblStarDeliveryDay})
    '    InvisibleCtrl({lstDeliveryDay, lstOrderByDay, lstOrderByDiff})
    '    Select Case lstFrequency.Text.ToUpper
    '        'Case "ONE-TIME"
    '        '    lblStartDate.Text = "Date"
    '        '    txtEndDate.Visible = False
    '        '    lblEndDate.Visible = False
    '        '    lblEndDateFmt.Visible = False
    '        '    lblDay.Visible = False
    '        '    lstDay.Visible = False
    '        '    lblDayStar.Visible = False
    '        Case "DAILY"
    '            VisibleCtrl({lstOrderByDiff})
    '        Case Else
    '            VisibleCtrl({lblTitleDeliveryDay, lblStarDeliveryDay})
    '            VisibleCtrl({lstDeliveryDay, lstOrderByDay})
    '    End Select
    'End Sub

    'Protected Sub lstDeliveryMaster_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstDeliveryMaster.SelectedIndexChanged
    '    Dim SQL As String
    '    'Dim customerID As String

    '    Using DB As New DBHelper
    '        setJavaScript(CustSetting_Tab.SCHEDULE)

    '        ''pnlDeliverySingleDtl.Visible = False
    '        'If lstDeliveryMaster.Text = String.Empty Then
    '        '    'pnlDeliverySingleDtl.Visible = True
    '        '    pnlDeliveryMultiDtl.Visible = False
    '        'Else
    '        '    'pnlDeliverySingleDtl.Visible = False
    '        '    pnlDeliveryMultiDtl.Visible = True
    '        'End If

    '        'Dim deliverySchediD = lblDeliverySchedID.Text.Substring(0, DeliverySchedID_LEN)
    '        'Dim deliveryschedlinenr = lblDeliverySchedID.Text.Substring(DeliverySchedID_LEN)
    '        SQL = "SELECT jd.deliveryMaster_id, jd.frequency, jd.deliveryDay, jd.orderByDay, jd.orderByTime, " & _
    '                " c.deliverySched_id, c.customer_id, c.shipPoint_id, c.sendReminder, to_char(NVL(c.startDate, SYSDATE), " & DB.SqlEsc(DATE_FMT) & ") startDate, to_char(c.endDate, " & DB.SqlEsc(DATE_FMT) & ") endDate " & _
    '                "FROM JFCDeliveryMaster jd, customer_deliverySched c " & _
    '                "WHERE jd.deliveryMaster_id = c.deliveryMaster_id (+) " & _
    '                "  AND jd.deliveryMaster_id = " & DB.SqlEsc(lstDeliveryMaster.Text)

    '        Dim r = DB.GetReader(SQL)
    '        If r.Read() Then
    '            lblFrequency.Text = CString(r("frequency")) 'CString(r("deliveryMaster_id"))
    '            lblDeliveryOn.Text = CString(r("deliveryDay"))
    '            lblOrderBy.Text = CString(r("orderByDay")) & " " & CString(r("orderByTime"))
    '            'lblOrderbyTime.Text = CString(r("orderByTime"))
    '        End If


    '    End Using
    'End Sub

    'Protected Sub btnSaveSchedule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveSchedule.Click
    '    Dim SQL As String
    '    Dim newSchedule As Boolean '= pnlMultiSchedule.Visible
    '    Dim startDateVal As Date
    '    Dim endDateVal As Date = Nothing
    '    Dim startDateStr As String
    '    Dim endDateStr As String
    '    Dim sendReminder As String
    '    Dim lst As DropDownList
    '    Dim txt As TextBox
    '    Dim r As DataReader
    '    Dim deliverySchedID As String
    '    Dim deliveryschedlinenr As Integer
    '    Dim orderbydate As String = ""
    '    Dim orderbytime As String = ""
    '    Using DB As New DBHelper
    '        setJavaScript(CustSetting_Tab.SCHEDULE)

    '        newSchedule = IIf(lblDeliverySchedID.Text = AUTO OrElse lblDeliverySchedID.Text = "", True, False)
    '        If newSchedule Then
    '            deliverySchedID = ""
    '            deliveryschedlinenr = 0
    '        Else
    '            deliverySchedID = lblDeliverySchedID.Text.Substring(0, DeliverySchedID_LEN)
    '            deliveryschedlinenr = CInt(lblDeliverySchedID.Text.Substring(DeliverySchedID_LEN))
    '        End If

    '        'Edit new or existing schedule
    '        'Check error
    '        If lstCustGrpVendor_schedule.Text = "" Then
    '            MessageBox("Please select a vendor", "Error", DB)

    '            Exit Sub
    '        End If
    '        If lstDeliveryMaster.Text = "" Then
    '            MessageBox("Please select delivery schedule master", "Error", DB)
    '            Exit Sub
    '        End If



    '        For Each row As GridViewRow In grdActiveSchedule.Rows
    '            Dim lnkBtn, lnkmaster As LinkButton
    '            lnkBtn = row.Cells(1).FindControl("lnkShipName")
    '            'Dim DELIVERYDAY As String = row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text
    '            lnkmaster = row.Cells(3).FindControl("lnkdeliveryMasterName")
    '            If lnkBtn.CommandArgument <> lblDeliverySchedID.Text Then
    '                If lnkmaster.Text = lstDeliveryMaster.SelectedItem.ToString() Then
    '                    MessageBox("This delivery schedule master name already exists, Please enter another one.", "Error", DB)
    '                    Exit Sub
    '                End If

    '                If lnkmaster.Text.Contains(lblDeliveryOn.Text) Then
    '                    MessageBox("This delivery day already exists, Please select another one.", "Error", DB)
    '                    Exit Sub
    '                End If

    '            End If

    '        Next


    '        DB.BeginTransaction()

    '        If newSchedule = False Then
    '            'Delete delivery schedule and the dates
    '            DeleteDeliverySched(DB, deliverySchedID, deliveryschedlinenr, False)
    '        End If


    '        'txt = grdDeliveryMaster.Rows.Item(i).FindControl("txtStartDate")
    '        startDateStr = Format(Date.Today, Common.DATE_FMT)
    '        DateTime.TryParseExact(startDateStr, DATE_FMT, Nothing, Nothing, startDateVal)
    '        'txt = grdDeliveryMaster.Rows.Item(i).FindControl("txtEndDate")
    '        endDateStr = ""
    '        If endDateStr <> "" Then DateTime.TryParseExact(endDateStr, DATE_FMT, Nothing, Nothing, endDateVal)
    '        lst = lstSendReminder
    '        sendReminder = IIf(lst.Text = "Yes", "Y", "N")

    '        If lblOrderBy.Text <> "" Then
    '            Dim Orderbysplit = lblOrderBy.Text.Split(" ")
    '            orderbydate = Orderbysplit(0)
    '            orderbytime = Orderbysplit(1) & " " & Orderbysplit(2)
    '        End If


    '        UpSertDeliverySched(DB, deliverySchedID, deliveryschedlinenr, lblCustomerID_schedule.Text, lstCustGrpVendor_schedule.Text, lblShipPointID_schedule.Text, lstDeliveryMaster.Text, _
    '                               lblFrequency.Text, startDateVal, endDateVal, lblDeliveryOn.Text, orderbydate, orderbytime, sendReminder, "", False)
    '        DB.CommitTransaction()

    '        'Saved
    '        InsertEventLog(DB, "Saved delivery schedule", lblDeliverySchedID.Text, Nothing, Nothing, True)


    '        If chkEmailMaster.Checked Then
    '            Dim emailBody As String = ""
    '            SQL = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr, " &
    '                "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
    '                "FROM webuser u, webuser_customer uc, customer c " &
    '                "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
    '                "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
    '                "AND c.customer_id = '" & CString(lblCustomerID.Text) & "' order by c.customer_id"
    '            Dim dt = DB.GetDataTable(SQL)
    '            Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
    '            Dim r2 As DataReader
    '            r2 = DB.GetReader(sql2, txtPopJFCAccount.Text)
    '            Dim VendorEmail As String = ""
    '            If r2.Read() Then
    '                VendorEmail = CString(r2("email"))

    '            End If
    '            r2.close()
    '            For Each dr In dt.Rows

    '                CreateEmailBody(Page, emailBody, "MASTER", deliverySchedID & deliveryschedlinenr, newSchedule)
    '                Dim errmsg As String = ""
    '                'CString(dr("customerEmail"))
    '                If Common.SendMail(SYSTEM_EMAIL, CString(dr("customerEmail")), VendorEmail, "Your delivery schedule was updated (#" & DB.NVL(deliverySchedID, "") & ")", emailBody, Nothing, True, errmsg) > 0 Then
    '                    'Email sent failed
    '                    'emailSent = False
    '                Else
    '                    'Email sent OK
    '                    'emailSent = True
    '                End If
    '            Next
    '            MessageLine("CONFIRM", "Successfully saved and send email to customer")
    '        Else
    '            MessageLine("CONFIRM", "Successfully saved")
    '        End If
    '        showScheduleList()
    '    End Using
    'End Sub

    Private Sub showScheduleList()
        Dim SQL As String

        Using DB As New DBHelper
            'Show schedule list
            'pnlList_schedule.Visible = True
            'pnlDtl_schedule.Visible = False

            'Set properties
            'lblShipPoint.Visible = False
            'lstShipPoint.Visible = False
            'If lstShipPoint.Items.Count > 1 Then
            '    'Show shipping point
            '    lblShipPoint.Visible = True
            '    lstShipPoint.Visible = True
            'End If
            grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "endDate")).Visible = False
            'If chkShowAll_schedule.Checked = False Then
            '    'grdActiveSchedule.Columns(10).Visible = False 'Delete
            'End If
            If IsMobile() Then
                'grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "customerName")).Visible = False
                grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "vendorName")).Visible = False
                'grdActiveSchedule.Columns(7).Visible = False 'Send reminder
                grdActiveSchedule.Columns(GetColumnIndex(grdActiveSchedule, "startDate")).Visible = False

                grdActiveSchedule.Columns(10).Visible = False 'Delete
            End If

            'Show active shipping schedule
            SQL = "SELECT m.custCompanyName, g.custGrpName, c.customerName, s.shipName, c.custGrp_id, d.customer_id, v.vendorName, d.deliverySched_id,d.deliveryschedlinenr,d.deliveryMaster_id, jh.deliveryMasterName, d.frequency, d.sendReminder, " & _
                        " to_char(d.startDate, " & DB.SqlEsc(DATE_FMT) & ") startDate, to_char(d.endDate, " & DB.SqlEsc(DATE_FMT) & ") endDate, d.deliveryDay, d.orderByDay || ' ' || d.orderByTime AS orderByDayTime " & _
                        "FROM customer_deliverySched d, JFCDeliveryMaster jh, customer c, customer_shipPoint s, custGrp_vendor v, webUser_customer u, custGrp g, custCompany m " & _
                        "WHERE d.deliveryMaster_id = jh.deliveryMaster_id (+) " & _
                        "  AND d.customer_id = c.customer_id " & _
                        "  AND d.shipPoint_id = s.shipPoint_id AND d.customer_id = s.customer_id " & _
                        "  AND u.custCompany_id = g.custCompany_id AND g.custGrp_id = c.custGrp_id AND c.customer_id = u.customer_id AND u.custCompany_id = m.custCompany_id " & _
                        "  AND u.email = " & DB.SqlEsc(GetUserEmail(DB)) & " " & _
                        "  AND d.customer_id = " & DB.SqlEsc(lblCustomerID.Text) & " AND d.shipPoint_id = " & DB.SqlEsc(lstShipPoint.Text) & _
                        "  AND (d.endDate >= TRUNC(SYSDATE) OR d.endDate is NULL) " & _
                        "  AND d.custGrpVendor_id = v.custGrpVendor_id"
            dsActiveSchedule.ProviderName = "System.Data.OracleClient"
            dsActiveSchedule.ConnectionString = DB.GetConnectionString
            dsActiveSchedule.SelectCommand = SQL
            grdActiveSchedule.DataSourceID = "dsActiveSchedule"
            'grdActiveSchedule.Sort("STARTDATE", SortDirection.Descending)
            grdActiveSchedule.DataBind()

            'btnAddSchedule.Visible = IIf(chkShowAll_schedule.Checked = False AndAlso grdActiveSchedule.Rows.Count > 0, False, True)
        End Using
    End Sub

    'Private Sub showScheduleDtl(ByVal deliverySchedID As String)
    '    Dim SQL As String
    '    Dim r As DataReader
    '    Dim dt As DataTable
    '    Dim dr As DataRow

    '    Using DB As New DBHelper
    '        'Show detail of delivery schedule
    '        pnlList_schedule.Visible = False
    '        pnlDtl_schedule.Visible = True
    '        chkEmailMaster.Checked = False

    '        'Set properties
    '        'pnlDtlView_schedule.Visible = False
    '        pnlDtlEdit_schedule.Visible = False
    '        'pnlMultiSchedule.Visible = False
    '        pnlSingleSchedule.Visible = False
    '        pnlCustGrpName_schedule.Visible = False
    '        'pnlDeliverySingleDtl.Visible = False
    '        btnDeleteSchedule.Visible = False
    '        btnSaveSchedule.Visible = False
    '        If deliverySchedID = "" Then
    '            'New schedule
    '            pnlDtlEdit_schedule.Visible = True
    '            'pnlMultiSchedule.Visible = True 'TODO: May need to change later 
    '            pnlSingleSchedule.Visible = True
    '            lblDtlTitle_schedule.Text = "Create Delivery Schedule" ' for " & lstCust_schedule.SelectedItem.Text & IIf(lstShipPoint.Items.Count > 1, " (" & lstShipPoint.SelectedItem.Text & ")", "")
    '            btnSaveSchedule.Visible = True
    '            btnSaveSchedule.Text = "Save Schedule"
    '        Else
    '            'Edit schedule
    '            pnlSingleSchedule.Visible = True
    '            'InvisibleCtrl({lblTitleDeliveryDay, lblStarDeliveryDay})
    '            'InvisibleCtrl({lstDeliveryDay, lstOrderByDiff})
    '            'ClearCtrl({txtStartDate, txtEndDate})
    '            lblDtlTitle_schedule.Text = "Detail of Delivery Schedule" ' for " & lstCust_schedule.SelectedItem.Text & IIf(lstShipPoint.Items.Count > 1, " (" & lstShipPoint.SelectedItem.Text & ")", "")
    '            btnSaveSchedule.Text = "Update Changes"
    '            pnlDtlEdit_schedule.Visible = True
    '            btnSaveSchedule.Visible = True
    '            btnDeleteSchedule.Visible = True
    '        End If
    '        If IsMobile() Then
    '        Else
    '            pnlSingleSchedule.Width = 500
    '        End If


    '        ShowCodeMasterListLocal(DB, "YESNO", lstSendReminder, True)

    '        'Show delivery schedule info
    '        lblCustGrpName_schedule.Text = lstCustGrp_schedule.SelectedItem.Text
    '        lblCustomerName_schedule.Text = lstCust_schedule.SelectedItem.Text
    '        lblShipPointName_schedule.Text = lstShipPoint.SelectedItem.Text
    '        lblCustomerID_schedule.Text = lstCust_schedule.Text
    '        GetJFCBranchAndCustomerNr(DB, lblCustomerID_schedule.Text, lblJfcBranchNr_schedule.Text, lblJfcCustomerNr_schedule.Text)
    '        'lblJfcBranchNr_schedule.Text = jfcBranchNr
    '        'lblJfcCustomerNr_schedule.Text = jfcCustomerNr
    '        lblShipPointID_schedule.Text = lstShipPoint.Text
    '        'Show vendor
    '        SQL = "SELECT custGrpVendor_id, vendorName " & _
    '            "FROM custGrp_vendor " & _
    '            "WHERE custGrp_id = :1 " & _
    '            "ORDER BY vendorName"
    '        dt = DB.GetDataTable(SQL, lstCustGrp_schedule.Text)
    '        dr = dt.NewRow
    '        dr("vendorName") = LST_CMD_STR
    '        dt.Rows.InsertAt(dr, 0)
    '        lstCustGrpVendor_schedule.DataSource = dt
    '        lstCustGrpVendor_schedule.DataValueField = "custGrpVendor_id"
    '        lstCustGrpVendor_schedule.DataTextField = "vendorName"
    '        lstCustGrpVendor_schedule.DataBind()
    '        SelectIfOneItem(lstCustGrpVendor_schedule)
    '        'Show delivery master
    '        SQL = "SELECT deliveryMaster_id, deliveryMasterName FROM JFCDeliveryMaster WHERE jfcBranchNr = :1 ORDER BY deliveryMasterName"
    '        dt = DB.GetDataTable(SQL, lblJfcBranchNr_schedule.Text)
    '        dr = dt.NewRow
    '        dr("deliveryMasterName") = LST_CMD_STR '"No Master"
    '        dt.Rows.InsertAt(dr, 0)
    '        lstDeliveryMaster.DataSource = dt
    '        lstDeliveryMaster.DataValueField = "deliveryMaster_id"
    '        lstDeliveryMaster.DataTextField = "deliveryMasterName"
    '        lstDeliveryMaster.DataBind()

    '        If deliverySchedID = "" Then
    '            'New schedule
    '            lblDeliverySchedID.Text = AUTO
    '            lstCustGrpVendor_schedule.Enabled = True
    '            lstDeliveryMaster_SelectedIndexChanged(Nothing, Nothing)
    '            lblFrequency.Text = ""
    '            lblOrderBy.Text = ""
    '            lblDeliveryOn.Text = ""
    '            '    SQL = "SELECT m.custCompanyName, g.custGrpName, c.customer_id, c.customerName, s.shipPoint_id, s.shipName, c.custGrp_id, NULL vendorName, " & _
    '            '                " NULL startDate, NULL endDate, NULL frequency, NULL deliveryDay, NULL orderByDayTime, s.shipName " & _
    '            '                "FROM customer c, customer_shipPoint s, webUser_customer u, custGrp g, custCompany m " & _
    '            '                "WHERE c.customer_id = s.customer_id AND u.custCompany_id = g.custCompany_id AND g.custGrp_id = c.custGrp_id AND c.customer_id = u.customer_id AND u.custCompany_id = m.custCompany_id " & _
    '            '                "  " & IIf(chkShowAll2_schedule.Checked = True, "", " AND c.customer_id = " & DB.SqlEsc(lstCust_schedule.Text) & " AND s.shipPoint_id = " & DB.SqlEsc(lstShipPoint.Text)) & _
    '            '                "  AND u.email = " & DB.SqlEsc(GetUserEmail(DB))
    '            '    dsMultiSchedule.ProviderName = "System.Data.OracleClient"
    '            '    dsMultiSchedule.ConnectionString = DB.GetConnectionString
    '            '    dsMultiSchedule.SelectCommand = SQL
    '            '    grdMultiSchedule.DataSourceID = "dsMultiSchedule"
    '            '    grdMultiSchedule.Sort("customerName, shipName, vendorName, frequency", SortDirection.Ascending)
    '            '    grdMultiSchedule.DataBind()
    '        Else
    '            'Edit schedule
    '            lblDeliverySchedID.Text = deliverySchedID
    '            'Show delivery info
    '            SQL = "SELECT g.custGrpName, c.customerName, s.shipName, d.deliverySched_id, d.customer_id, d.shipPoint_id, d.custGrpVendor_id, d.deliveryMaster_id, " & _
    '                     " d.frequency, d.deliveryDay, d.orderByDay, d.orderByTime, d.sendReminder, c.jfcBranchNr, c.jfcCustomerNr, " & _
    '                     " TO_CHAR(startDate, :1) startDate, TO_CHAR(endDate, :2) endDate,  jm.frequency as frequency, jm.deliveryday as deliveryday, jm.orderbyday as orderbyday, jm.orderbytime as orderbytime " & _
    '                     "FROM customer_deliverySched d, custGrp g, customer c, customer_shipPoint s,JFCDeliveryMaster jm  " & _
    '                     "WHERE d.customer_id = c.customer_id AND g.custGrp_id = c.custGrp_id " & _
    '                     "  AND d.shipPoint_id = s.shipPoint_id AND d.customer_id = s.customer_id AND jm.deliverymaster_id  = d.deliverymaster_id" & _
    '                     "  AND d.deliverySched_id = :3 and d.deliveryschedlinenr = :4"
    '            r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, deliverySchedID.Substring(0, deliverySchedID.Length - 1), deliverySchedID.Last.ToString())
    '            If r.Read() Then
    '                lstCustGrpVendor_schedule.Enabled = False
    '                If lstCustGrpVendor_schedule.Items.FindByValue(CString(r("custGrpVendor_id"))) IsNot Nothing Then lstCustGrpVendor_schedule.Text = CString(r("custGrpVendor_id"))
    '                If lstDeliveryMaster.Items.FindByValue(CString(r("deliveryMaster_id"))) IsNot Nothing Then lstDeliveryMaster.Text = CString(r("deliveryMaster_id"))
    '                lstDeliveryMaster_SelectedIndexChanged(Nothing, Nothing)
    '                lblFrequency.Text = CString(r("frequency")) 'CString(r("deliveryMaster_id"))
    '                lblDeliveryOn.Text = CString(r("deliveryDay"))
    '                lblOrderBy.Text = CString(r("orderByDay")) & " " & CString(r("orderBytime"))
    '                'lblOrderbyTime.Text = 
    '                'lstFrequency.Text = CString(r("frequency"))
    '                'lstFrequency_SelectedIndexChanged(Nothing, Nothing)
    '                'If lstDeliveryDay.Visible = True Then lstDeliveryDay.Text = CString(r("deliveryDay"))
    '                'If lstOrderByDay.Visible = True Then lstOrderByDay.Text = CString(r("orderByDay"))
    '                'If lstOrderByDiff.Visible = True Then lstOrderByDiff.Text = CString(r("orderByDay"))
    '                'lstOrderByTime.Text = Left(CString(r("orderByTime")), 5)
    '                'lstOrderByAmPm.Text = Right(CString(r("orderByTime")), 2)
    '                lstSendReminder.Text = IIf(CString(r("sendReminder")) = "Y", "Yes", "No")
    '                'txtStartDate.Text = CString(r("startDate"))
    '                'txtEndDate.Text = CString(r("endDate"))
    '            Else
    '                showScheduleList()
    '                MessageBox("Schedule no longer exists", "Error", DB)
    '                r.close()
    '                Exit Sub
    '            End If
    '            r.close()
    '        End If
    '    End Using
    'End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Show order form
    '--------------------------------------------------------------------------------------------------------------------


    '--------------------------------------------------------------------------------------------------------------------
    'Common routine
    '--------------------------------------------------------------------------------------------------------------------

    Protected Sub grd_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdActiveSchedule.RowDataBound
        'Highlight the mourse-over row and make it selectable
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
            'Dim chkSendReminder As CheckBox = e.Row.FindControl("ChkSendReminder")
            If Not ViewState("JFC_ADMINGROUP") Then
                linkbtn.ForeColor = Drawing.Color.Black
                linkbtn.Enabled = False
                Imgbtn.Visible = False

            End If
            'chkSendReminder.Checked = True

            Using DB As New DBHelper
                sql = "SELECT deliveryMaster_id, deliveryMasterName FROM JFCDeliveryMaster WHERE jfcBranchNr = :1 ORDER BY deliveryMasterName"
                dt = DB.GetDataTable(sql, lblJfcBranchNr.Text)
                dr = dt.NewRow
                dr("deliveryMasterName") = LST_CMD_STR '"No Master"
                dt.Rows.InsertAt(dr, 0)
                lst = CType(e.Row.FindControl("lstDeliveryMaster"), DropDownList)
                lst.DataSource = dt
                lst.DataValueField = "deliveryMaster_id"
                lst.DataTextField = "deliveryMasterName"
                lst.DataBind()
                Str = CType(e.Row.FindControl("lnkdeliveryMasterName"), LinkButton).Text
                If lst.Items.FindByValue(Str) IsNot Nothing Then lst.Items.FindByValue(Str).Selected = True
            End Using
            'If sender.ID = "grdCustGrpItem" Then
            '    'Cannot click row
            '    e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
            'Else
            '    'Can click row
            '    e.Row.Attributes.Add("onmouseover", "this.className='selectRowClickHighlight';")
            '    'e.Row.ToolTip = "Click to select row"
            '    Dim lnk As LinkButton = e.Row.FindControl("lnkSelectRow")
            '    e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackClientHyperlink(lnk, ""))
            'End If
        End If
    End Sub

    'Protected Sub btnInfo_Click(sender As Object, e As System.EventArgs) Handles btnInfo_VendorEmail.Click
    '    MessageBox(sender.ToolTip, "Information")
    'End Sub
    'YI FU 09162020

    'Private Sub btnGetItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetItem.Click

    '    setJavaScript(CustSetting_Tab.CUSTITEM)

    '    If txtItem.Text.Contains(",") = False Then
    '        MessageBox("Please select an item from the auto-complete list", "Error")
    '        Exit Sub
    '    End If


    '    Dim custGrpItemID = GetLastWord(txtItem.Text, ",").PadLeft(CUSTGRP_ITEM_ID_LEN, "0")
    '    ViewState("Current_custGrpItemID") = custGrpItemID
    '    showCustItemDel(custGrpItemID)
    '    txtItem.Text = ""

    'End Sub


    'Private Sub btnClearItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearItem.Click
    '    setJavaScript(CustSetting_Tab.CUSTITEM)
    '    txtItem.Text = ""
    '    'clearQtyEntry()
    '    'clearItemDtl()
    'End Sub

    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        Dim id As String = dialogArg1

        Using DB As New DBHelper
            Select Case dialogCmd
                Case "DELETE_CUSTITEM"
                    'TODO: Implement later
                    ''Disable item
                    'DeleteCCCustItem(DB, lstCust_custItem.Text, id)
                    ''Return to customer's item list
                    'setJavaScript(CustSetting_Tab.CUSTITEM)
                    'DeleteCustomerCustItem(DB, lblCustomerID.Text, id)
                    ''showCustItemList()

                    'InsertEventLog(DB, "Deleted customer's item", Nothing, Nothing, Nothing, True)
                    'MessageLine("CONFIRM", "Successfully deleted")
                    'Case "DE"
                Case "DELETE_SCHEDULE"
                    setJavaScript(CustSetting_Tab.SCHEDULE)
                    'Delete delivery schedule and dates
                    DeleteDeliverySched(DB, id.Substring(0, DeliverySchedID_LEN), id.Substring(DeliverySchedID_LEN)) ', IIf(lstModFutureOnly.Visible = True AndAlso lstModFutureOnly.Text = "Yes", True, False))
                    'Return to customer's item list
                    showScheduleList()
                    InsertEventLog(DB, "Deleted delivery schedule", id, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                Case "DELETE_ALLSCHEDULE"
                    setJavaScript(CustSetting_Tab.CUST)
                    DeleteAllDeliverySched(DB, lblCustomerID.Text)
                    'Return to customer's item list
                    showScheduleList()
                    InsertEventLog(DB, "Deleted all delivery schedule", id, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                Case "DELETE_USER"
                    'DeleteDeliverySched(DB, ) ', IIf(lstModFutureOnly.Visible = True AndAlso lstModFutureOnly.Text = "Yes", True, False))
                    'Return to customer's item list
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
                    Dim Textphones = txtTextPhones.Text.TrimEnd(",")
                    Dim outofstock = IIf(CString(lstOutofStock.SelectedValue) = "Show", "Y", "")
                    Dim ShowAllExcepGrpItem = IIf(CString(lstAllExcepGrpItem.SelectedValue) = "Yes", "Y", "")
                    Dim shipVia = CString(txtShipvia.Text)
                    Dim shipOffset = Val(txtShipDateOffset.Text)
                    Dim minOrderAmt = Val(txtMinOrderAmt.Text)
                    UpdateCustSetting(db, lblCustomerID.Text, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice, anytimedelivery, Textphones, outofstock, ShowAllExcepGrpItem, shipVia, shipOffset, minOrderAmt)
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

                'Show and select hide order
                lst = CType(e.Row.FindControl("lstHideItemPage"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHideItemPage"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                'Show and select hide order
                lst = CType(e.Row.FindControl("lstHidePaymentPage"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHidePaymentPage"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")

                'Show and select hide order
                lst = CType(e.Row.FindControl("lstHidePrice"), DropDownList)
                ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                str = CType(e.Row.FindControl("lblHidePrice"), Label).Text
                str = IIf(str = "Y", "Y", "N")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")

                txt = CType(e.Row.FindControl("txtCell"), TextBox)
                IIf(IsMobile(), txt.CssClass = "cellLbl size4", "")

            End If
        End Using
    End Sub

    Protected Sub grdUser_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdUser.RowCommand
        'setJavaScript(CustSetting_Tab.SCHEDULE)
        If e.CommandName = "DELETE_ROW" Then
            'Delete schedule
            'Dim gvr As GridViewRow = CType(CType(e.CommandSource, ImageButton).NamingContainer, GridViewRow)
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
        'ClearSubControl(popDialog_User)
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
                    Dim Sql = "SELECT c.Name FROM  jho.customer@jho c " & _
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
        'Dim jfcParentBranchNr As String = ""
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

                    SQL = "SELECT c.Name FROM  jho.customer@jho c " & _
                        " WHERE c.bid = :1 AND c.customerid = :2"
                    lblPopCustomerName.Text = CString(DB.GetScalar(SQL, txtPopJFCAccount.Text, txtPopJFCNum.Text))
                End If
            End If

            ''Dim have = lstCustCompany.Items.Cast(Of ListItem).Any(Function(x) x.Text.Substring(0, 2).Contains(txtPopJFCAccount.Text))
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
                lblPopWarningUser.Text = "Error! Your email address cannot contain ','" 'Doesn't allow multiple email address separated by "," for auto signup
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


            SQL = "SELECT u.email FROM webuser_customer uc, customer c, webuser u, jfcVendor v " & _
                    "WHERE uc.email = u.email AND uc.customer_id = c.customer_id AND c.jfcBranchNr = v.jfcBranchNr AND v.jfcBranchNr = :1 AND c.jfcCustomerNr = :2 AND uc.email = :3"
            If CString(DB.GetScalar(SQL, txtPopJFCAccount.Text, txtPopJFCNum.Text, email)) <> String.Empty Then
                lblPopWarningUser.Text = "Error! This customer already exists. "
                pnlPopWarningUser.Visible = True
                popDialog_User.Show()
                Exit Sub
            End If



            '

            If IsProduction() = True OrElse IsProdBeta() = True Then
                'Get sales rep email (to address) and branch email (cc address)
                If SUPPORT_EMAIL.Contains(userEmail) OrElse IsJfcBranchAdmin(DB, userEmail) Then
                    SQL = "SELECT s.email1 AS salesmanEmail, b.branchid, CASE WHEN b.branchid = '45' THEN '45' ELSE b.parent_branchid END AS parentBranchID " & _
                       "FROM jho.branch@jho b, jho.customer@jho c LEFT JOIN jho.salesman@jho s ON c.salesmanid = s.salesmanid " & _
                       "WHERE b.branchid = c.bid AND c.bid = s.bid AND c.cid = s.cid AND b.branchid = :1 AND c.customerid = :2 " & _
                       "group by s.email1, b.branchid,b.parent_branchid"
                Else
                    SQL = "SELECT s.email1 AS salesmanEmail, b.branchid, CASE WHEN b.branchid = '45' THEN '45' ELSE b.parent_branchid END AS parentBranchID " & _
                       "FROM jho.branch@jho b, jho.customer@jho c, jho.salesman@jho s " & _
                       "WHERE b.branchid = c.bid AND c.bid = s.bid AND c.cid = s.cid AND c.salesmanid = s.salesmanid AND b.branchid = :1 AND c.customerid = :2 " & _
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
                'jfcBranchNr = CString(r("jfcBranchNr"))
            End If
            r.close()

            SQL = "SELECT email FROM webuser WHERE email = :1"
            isNewWebUser = IIf(CString(DB.GetScalar(SQL, email)) = String.Empty, True, False)

            'Save request info
            'InsertSignupRequest(DB, jfcParentBranchNr, txtJfcCustomerNr.Text, txtBusinessName.Text, txtName.Text, email, txtCell.Text, txtRegistrationCode.Text.ToUpper, salesmanEmail, jfcVendorEmail)
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


        'SetUpdateButton(False)
    End Sub
    'Protected Sub txtPopJFCAccount_textchanged() Handles txtPopJFCNum.TextChanged  ', txtPopJFCAccount.TextChanged
    '    lblPopCustomerName.Text = ""
    '    pnlPopWarningUser.Visible = False
    '    If txtPopJFCAccount.Text <> "" And txtPopJFCAccount.Text <> "" Then
    '        Using db As New DBHelper
    '            Dim Sql = "SELECT c.Name FROM  jho.customer@jho c " & _
    '               " WHERE c.bid = :1 AND c.customerid = :2"
    '            Dim r = db.GetReader(Sql, txtPopJFCAccount.Text, txtPopJFCNum.Text)
    '            If r.Read() Then
    '                lblPopCustomerName.Text = CString(r("Name"))

    '            End If
    '        End Using

    '    End If
    '    popDialog_User.Show()
    'End Sub
    'Protected Sub btnPopCloseUser_Click(sender As Object, e As System.EventArgs) Handles btnPopClose_User.Click
    '    setJavaScript(CustSetting_Tab.CUST)
    'End Sub

    'Protected Sub ClientValidate_PopJFCNum(sender As Object, args As ServerValidateEventArgs)
    '    Dim v As String = lblPopCustomerName.Text
    '    If String.IsNullOrEmpty(v) Then
    '        args.IsValid = False
    '    Else
    '        args.IsValid = True
    '    End If

    'End Sub

    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub
End Class
