Imports System.Data
Imports Common
Imports DBUpdate
Imports Winnovative.ExcelLib
Imports System.Threading
Imports System.Globalization
Imports System.Resources

Partial Class JFCAdmin_CustItem
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
                ViewState("CustItem_Tab") = IIf(Request.QueryString("TAB") <> "", Request.QueryString("TAB"), CustItem_Tab.CUSTITEM)
                'ViewState("CUSTSETTING_CMD") = "EDIT_BY_JFCADMIN"
                ViewState("CUSTSETTING_CUSTCOMPANYID") = lstCustCompany.Text

                'Select tab
                setJavaScript(ViewState("CustItem_Tab"))

                'Get customer ID
                SQL = "SELECT c.customer_id FROM customer c, webuser_customer w WHERE c.customer_id = w.customer_id AND w.customer_id <> 'ADMIN' AND w.email = :1 AND w.custCompany_id = :2"
                lblCustomerID.Text = CString(DB.GetScalar(SQL, email, ViewState("CUSTSETTING_CUSTCOMPANYID")))

                ViewState("CUSTOMER_INFO") = New CustomerInfo(lblCustomerID.Text)

                '--------------------------------------------------------------------------------------------------------------------
                'Order Form
                '--------------------------------------------------------------------------------------------------------------------
                Dim custoInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
                'Show warning
                'Common.IsExcepPriceCustomer(DB, lblCustomerID.Text) Then '
                If custoInfo.IsExcepPriceCustomer Then

                    'This customer uses an exception price group, you cannot add / delete items or change price for this customer.
                    lblWarning.Text = "This customer uses an exception price group, you cannot add / delete items or change price for this customer."
                    btnAddPriceInquire.Enabled = False
                    ImgbtnPriceInquiryCopy.Visible = False
                    lnkPriceInquiryCopy.Visible = False
                    btnAddPriceInquire.CssClass = "midButton_gray"

                End If
                ViewState("IsExcepPriceCustomer") = custoInfo.IsExcepPriceCustomer 'Common.IsExcepPriceCustomer(DB, lblCustomerID.Text)

                'Show sort by for item list
                lstOrderBy_custItem.Items.Clear()
                lstOrderBy_custItem.Items.Add(New ListItem("Custom", "custdisporder, shipDate DESC, vendorName, ctgDispOrder, itemDispOrder, ctgName_view, vendorName, itemDesc_view, itemGrpDispOrder")) 'Added vendorName to show formHdr in correct place
                lstOrderBy_custItem.Items.Add(New ListItem("Ordered Date", "CASE WHEN shipdate IS NULL THEN '2' ELSE '1' END, shipDate DESC, vendorName, ctgDispOrder, itemDispOrder, ctgName_view, vendorName, itemDesc_view, itemGrpDispOrder")) 'Added vendorName to show formHdr in correct place
                lstOrderBy_custItem.Items.Add(New ListItem("Category", "ctgDispOrder, itemDispOrder, vendorName, itemDesc_view, itemGrpDispOrder"))
                'lstOrderBy_custItem.Items.Add(New ListItem("Vendor", "vendorName, ctgName_view, itemDesc_view, itemGrpDispOrder"))
                lstOrderBy_custItem.Items.Add(New ListItem("Item Description", "itemDesc_view, ctgDispOrder, itemDispOrder, ctgName_view, vendorName, itemGrpDispOrder"))
                lstOrderBy_custItem.Items.Add(New ListItem("Item #", "itemIdNr_view, ctgDispOrder, itemDispOrder, ctgName_view, vendorName, itemGrpDispOrder"))
                'lstOrderBy_custItem.Items.Add(New ListItem("order form", "g.itemGrpName, ctgName_view, v.vendorName, itemIdNr_view"))
                'If customer ctg exists, sort by Category by default
                If custoInfo.useCustomerCtg Then lstOrderBy_custItem.SelectedIndex = 1

                'End If
                'Common.UseCustomerCtg(DB, lblCustomerID.Text) = True Then 

                'Show customer groups
                Common.ShowCustGrpList(DB, ViewState("CUSTSETTING_CUSTCOMPANYID"), lstCustGrp_custItem)
                'Show customers
                Common.ShowCustomerList(DB, lstCustGrp_custItem.Text, lstCust_custItem)
                'Show search feild
                showCustItemSearch()
                'Show add item
                If IsMobile() Then
                Else
                    pnlSearchItem.Width = 400
                    pnlNewItem.Width = 600
                    pnlSearchItem.Height = 110
                    pnlNewItem.Height = 110
                End If
                pnlAddItem.Visible = False
                showPromoItemlst()
                'Show item list
                showCustItemList()
                ViewState("SHOW_ALL_INQUIRE") = False
                rdbCurrCust.Checked = True
                showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))

            End If
            '
        End Using
    End Sub
    ' ════════════════════════════════════════════════════════════════
    '  HELPERS
    ' ════════════════════════════════════════════════════════════════

    'Private Sub BindOFGrid(DB As DBHelper, customerID As String)
    '    grdOF.DataSource = DB.GetDataTable(
    '    "SELECT orderForm_id, orderFormName " &
    '    "FROM   customer_orderForm " &
    '    "WHERE  customer_id = :1 " &
    '    "ORDER  BY orderFormName", customerID)
    '    grdOF.DataBind()
    'End Sub

    'Private Sub LoadOrderFormDDL(DB As DBHelper,
    '                          customerID As String,
    '                          Optional selectID As String = "")
    '    Dim dt As DataTable = DB.GetDataTable(
    '    "SELECT orderForm_id, orderFormName " &
    '    "FROM   customer_orderForm " &
    '    "WHERE  customer_id = :1 " &
    '    "ORDER  BY  orderFormName", customerID)

    '    Dim dr As DataRow = dt.NewRow()
    '    dr("orderForm_id") = ""
    '    dr("orderFormName") = IIf(dt.Rows.Count = 0, "(None)", "(All items)")
    '    dt.Rows.InsertAt(dr, 0)

    '    lstOrderForm.DataSource = dt
    '    lstOrderForm.DataValueField = "orderForm_id"
    '    lstOrderForm.DataTextField = "orderFormName"
    '    lstOrderForm.DataBind()

    '    If selectID <> "" AndAlso
    '   lstOrderForm.Items.FindByValue(selectID) IsNot Nothing Then
    '        lstOrderForm.SelectedValue = selectID
    '    End If

    '    Dim email As String = GetUserEmail(DB)
    '    btnEditOrderForm.Visible =
    '    IsJfcBranchAdmin(DB, email) OrElse
    '    IsJfcHoAdmin(DB, email) OrElse
    '    IsKmsSupport(DB, email)
    'End Sub

    Private Function IsDuplicateName(DB As DBHelper,
                                  customerID As String,
                                  name As String,
                                  Optional excludeID As String = "") As Boolean
        If excludeID = "" Then
            Return Val(DB.GetScalar(
            "SELECT COUNT(*) FROM customer_orderForm " &
            "WHERE customer_id = :1 AND orderFormName = :2",
            customerID, name)) > 0
        Else
            Return Val(DB.GetScalar(
            "SELECT COUNT(*) FROM customer_orderForm " &
            "WHERE customer_id = :1 AND orderFormName = :2 " &
            "  AND orderForm_id <> :3",
            customerID, name, excludeID)) > 0
        End If
    End Function

    ' ════════════════════════════════════════════════════════════════
    '  OPEN POPUP — pencil button
    ' ════════════════════════════════════════════════════════════════

    'Protected Sub btnEditOrderForm_Click(sender As Object,
    '                                  e As ImageClickEventArgs)
    '    Using DB As New DBHelper
    '        grdOF.EditIndex = -1
    '        lblPopOFMsg.Text = ""
    '        txtNewOFName.Text = ""
    '        BindOFGrid(DB, lblCustomerID.Text)
    '    End Using
    '    popDialog_OrderForm.Show()
    'End Sub

    ' ════════════════════════════════════════════════════════════════
    '  CLOSE POPUP
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnPopCloseOrderForm_Click(sender As Object,
                                          e As EventArgs)
        ' ModalPopupExtender hides itself on postback;
        ' nothing extra needed here.
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  MAIN DDL changed
    ' ════════════════════════════════════════════════════════════════

    Protected Sub lstOrderForm_SelectedIndexChanged(sender As Object,
                                                 e As EventArgs)
        ' Re-filter Buy Again grid by lstOrderForm.SelectedValue
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  GRIDVIEW — Edit
    ' ════════════════════════════════════════════════════════════════

    'Protected Sub grdOF_RowEditing(sender As Object,
    '                            e As GridViewEditEventArgs)
    '    grdOF.EditIndex = e.NewEditIndex
    '    lblPopOFMsg.Text = ""
    '    Using DB As New DBHelper
    '        BindOFGrid(DB, lblCustomerID.Text)
    '    End Using
    '    popDialog_OrderForm.Show()
    'End Sub

    'Protected Sub grdOF_RowCancelingEdit(sender As Object,
    '                                  e As GridViewCancelEditEventArgs)
    '    grdOF.EditIndex = -1
    '    lblPopOFMsg.Text = ""
    '    Using DB As New DBHelper
    '        BindOFGrid(DB, lblCustomerID.Text)
    '    End Using
    '    popDialog_OrderForm.Show()
    'End Sub

    ' ════════════════════════════════════════════════════════════════
    '  GRIDVIEW — Update (rename)
    ' ════════════════════════════════════════════════════════════════

    'Protected Sub grdOF_RowUpdating(sender As Object,
    '                              e As GridViewUpdateEventArgs)
    '    Dim orderFormID As String =
    '    grdOF.DataKeys(e.RowIndex).Value.ToString()

    '    Dim newName As String =
    '    CType(grdOF.Rows(e.RowIndex).FindControl("txtEditOFName"),
    '          TextBox).Text.Trim()

    '    Dim customerID As String = lblCustomerID.Text

    '    If newName = "" Then
    '        lblPopOFMsg.Text = "Name cannot be empty."
    '        popDialog_OrderForm.Show()
    '        Return
    '    End If

    '    Using DB As New DBHelper
    '        If IsDuplicateName(DB, customerID, newName, orderFormID) Then
    '            lblPopOFMsg.Text = """" & newName & """ already exists."
    '            popDialog_OrderForm.Show()
    '            Return
    '        End If

    '        DB.BeginTransaction()
    '        DB.ExecuteNonQuery(
    '        "UPDATE customer_orderForm " &
    '        "SET    orderFormName = :1, modUser = :2, modDate = SYSDATE " &
    '        "WHERE  orderForm_id  = :3",
    '        newName, GetUserEmail(DB), orderFormID)
    '        DB.CommitTransaction()

    '        InsertEventLog(DB, "UPDATE customer_orderForm",
    '                   orderFormID, newName, Nothing, True, False)

    '        grdOF.EditIndex = -1
    '        lblPopOFMsg.Text = ""

    '        BindOFGrid(DB, customerID)
    '        LoadOrderFormDDL(DB, customerID, orderFormID)
    '    End Using

    '    MessageLine("CONFIRM", "Renamed to """ & newName & """.")
    '    popDialog_OrderForm.Show()
    'End Sub

    '' ════════════════════════════════════════════════════════════════
    ''  GRIDVIEW — Delete
    '' ════════════════════════════════════════════════════════════════

    'Protected Sub grdOF_RowDeleting(sender As Object,
    '                              e As GridViewDeleteEventArgs)
    '    Dim orderFormID As String =
    '    grdOF.DataKeys(e.RowIndex).Value.ToString()
    '    Dim customerID As String = lblCustomerID.Text

    '    Using DB As New DBHelper
    '        DB.BeginTransaction()

    '        DB.ExecuteNonQuery(
    '        "DELETE FROM customer_orderFormItems WHERE orderForm_id = :1",
    '        orderFormID)

    '        DB.ExecuteNonQuery(
    '        "DELETE FROM customer_orderForm WHERE orderForm_id = :1",
    '        orderFormID)

    '        DB.CommitTransaction()

    '        InsertEventLog(DB, "DELETE customer_orderForm",
    '                   orderFormID, customerID, Nothing, True, False)

    '        grdOF.EditIndex = -1
    '        lblPopOFMsg.Text = ""

    '        BindOFGrid(DB, customerID)
    '        LoadOrderFormDDL(DB, customerID)
    '    End Using

    '    MessageLine("CONFIRM", "Order form deleted.")
    '    popDialog_OrderForm.Show()
    'End Sub

    '' ════════════════════════════════════════════════════════════════
    ''  ADD NEW
    '' ════════════════════════════════════════════════════════════════

    'Protected Sub btnAddOF_Click(sender As Object, e As EventArgs)
    '    Dim name As String = txtNewOFName.Text.Trim()
    '    Dim customerID As String = lblCustomerID.Text

    '    If name = "" Then
    '        lblPopOFMsg.Text = "Please enter a name."
    '        popDialog_OrderForm.Show()
    '        Return
    '    End If

    '    Using DB As New DBHelper
    '        If IsDuplicateName(DB, customerID, name) Then
    '            lblPopOFMsg.Text = """" & name & """ already exists."
    '            popDialog_OrderForm.Show()
    '            Return
    '        End If

    '        Dim newID As String =
    '        DB.GetSequence("ORDERFORM_ID", CUSTGRP_ITEM_ID_LEN)

    '        DB.BeginTransaction()

    '        '"   createUser, createDate) " & GetUserEmail(DB)
    '        DB.ExecuteNonQuery(
    '        "INSERT INTO customer_orderForm " &
    '        "  (orderForm_id, customer_id, orderFormName) " & "VALUES (:1, :2, :3 )",
    '            newID, customerID, name)
    '        DB.CommitTransaction()

    '        InsertEventLog(DB, "INSERT customer_orderForm",
    '                   customerID, newID, name, True, False)

    '        txtNewOFName.Text = ""
    '        lblPopOFMsg.Text = ""

    '        BindOFGrid(DB, customerID)
    '        LoadOrderFormDDL(DB, customerID, newID)
    '    End Using

    '    MessageLine("CONFIRM", "Added """ & name & """.")
    '    popDialog_OrderForm.Show()
    'End Sub
    Protected Sub lstCustCompany_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany.SelectedIndexChanged
        If lstCustCompany.Text <> "" Then
            lblTitle.Visible = False
            Alltab.Visible = True
        End If

        Response.Redirect("CustItem.aspx?cust_company=" & lstCustCompany.Text & "&tab=" & ViewState("CustItem_Tab"))

    End Sub

    Protected Sub btnSearch_order_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchCust.Click

        'For Each d In lstCustCompany.Items
        '    Dim a = d
        'Next
        If lstCustCompany.Items.FindByValue(txtJfcBranchNr.Text & txtJfcCustomerNr.Text) Is Nothing Then
            MessageBox("Portal account for " & txtJfcBranchNr.Text & " - " & txtJfcCustomerNr.Text.ToUpper & " doesn't exist. Please use the ""Register Customer"" button to register.", "Error!")
            Exit Sub
        End If


        Response.Redirect("CustItem.aspx?cust_company=" & txtJfcBranchNr.Text & txtJfcCustomerNr.Text.ToUpper & "&tab=" & ViewState("CustItem_Tab"))
    End Sub

    Protected Sub lstCustGrp_custItem_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCustGrp_custItem.SelectedIndexChanged
        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            If lblTextChanged_custItem.Value = "YES" Then
                'Confirm
                ViewState("DIALOG_CMD") = "TEXT_CHANGED_CUSTGRP_CUSTITEM"
                MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", DB)
                Exit Sub
            Else
                'Show customer list
                Common.ShowCustomerList(DB, lstCustGrp_custItem.Text, lstCust_custItem)
                'Show search field
                showCustItemSearch()
                'Show item list
                showCustItemList()
            End If
        End Using
    End Sub

    Protected Sub lstCust_custItem_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCust_custItem.SelectedIndexChanged
        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            If lblTextChanged_custItem.Value = "YES" Then
                'Confirm
                ViewState("DIALOG_CMD") = "TEXT_CHANGED_CUST_CUSTITEM"
                MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", DB)
                Exit Sub
            Else
                showCustItemList()
            End If
        End Using
    End Sub

    Protected Sub lstOrderBy_custItem_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_custItem.SelectedIndexChanged
        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            'If text or list is changed, confirm; otherwise add the item
            If lstOrderBy_custItem.SelectedIndex = 0 Then
                grdCustItem.CssClass = " drag_drop_grid  STDTABLE"
            Else
                grdCustItem.CssClass = "STDTABLE"
            End If
            If lblTextChanged_custItem.Value = "YES" Then
                'Confirm
                ViewState("DIALOG_CMD") = "TEXT_CHANGED_ORDERBY_CUSTITEM"
                MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", DB)
                Exit Sub
            Else
                'Refresh item list
                showCustItemList()
            End If
        End Using
    End Sub

    Protected Sub grdCustItem_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdCustItem.RowCommand
        Using DB As New DBHelper

            setJavaScript(CustItem_Tab.CUSTITEM)
            If e.CommandName = "Page" Then
                'Page is changed
                showCustItemList()
                'Don't remember scroll position
                Page.MaintainScrollPositionOnPostBack = False
            ElseIf e.CommandName = "DELETE_ROW" Then
                'If SMALL_CUST_ONLY = True AndAlso IsKmsSupport(DB, GetUserEmail(DB)) = False Then
                '    MessageBox("Currently you cannot make any changes here.", "Error", DB)
                '    Exit Sub
                'End If
                'Delete item
                deleteCustItemRow(Convert.ToString(e.CommandArgument))
            ElseIf e.CommandName = "EDIT_ROW" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()
                showCustItemDel(e.CommandArgument.ToString())
                ViewState("PriceInquire") = False
            End If
        End Using
    End Sub

    ''TODO: Code below will detect changes in list after paging but not before
    ''Protected Sub grdCustItem_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdCustItem.PageIndexChanging
    ''    setJavaScript(CustItem_Tab.CUSTITEM)
    ''    If lblTextChanged_custItem.Value = "YES" Then
    ''        'Confirm
    ''        ViewState("DIALOG_CMD") = "TEXT_CHANGED_PAGE_CUSTITEM"
    ''        MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", mDB)
    ''        Exit Sub
    ''    End If
    ''End Sub

    Protected Sub grdCustItem_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdCustItem.RowDataBound
        Dim lbl As Label
        Dim btn As ImageButton
        If e.Row.RowType = DataControlRowType.DataRow Then
            lbl = e.Row.FindControl("lblenable")
            If lbl.Text = "Y" Then
                e.Row.BackColor = Drawing.Color.LightYellow
            End If
            btn = e.Row.FindControl("btnDelete")
            If ViewState("IsExcepPriceCustomer") Then btn.Visible = False

            'e.Row.CssClass = "selectRow"
            'e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            'e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
        End If


    End Sub

    Protected Sub lstSearch_custItem_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstItemGrpSearch_custItem.SelectedIndexChanged, lstCtgSearch_custItem.SelectedIndexChanged, lstVendorSearch_custItem.SelectedIndexChanged
        setJavaScript(CustItem_Tab.CUSTITEM)
        'TODO: Add confirmation
        'If text or list is changed, confirm; otherwise add the item
        'If lblTextChanged_custItem.Value = "YES" Then
        '    'Confirm
        '    ViewState("DIALOG_CMD") = "TEXT_CHANGED_ORDERBY_CUSTITEM"
        '    MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", mDB)
        '    Exit Sub
        'Else
        '    'Refresh item list
        '    showCustItemList()
        'End If
        'Refresh item list
        showCustItemList()
    End Sub


    Protected Sub btnSearch_custItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch_custItem.Click
        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            If lblTextChanged_custItem.Value = "YES" Then
                'Confirm
                ViewState("DIALOG_CMD") = "TEXT_CHANGED_SEARCH_CUSTITEM"
                MessageBoxYesNo("Changes in item list will be lost if you proceed. Are you sure you want to continue?", "Item list has been changed", DB)
                Exit Sub
            Else
                showCustItemList()
            End If
        End Using
    End Sub




    Private Sub deleteCustItemRow(ByVal custGrpItemID As String)
        Dim msg As String

        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_CUSTITEM"
            ViewState("DIALOG_ARG1") = custGrpItemID
            ViewState("PopEditCustomerGrpId") = custGrpItemID
            msg = "Are you sure you want to remove item " & GetItemID(DB, custGrpItemID) & " from " & lstCust_custItem.SelectedItem.Text & "?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub



    Private Sub initTextChanged_custItem()
        lblTextChanged_custItem.Value = "NO"
        ViewState("TEXT_CHANGED_CUSTITEM") = "NO"
        lblPrevCustGrp_custItem.Text = lstCustGrp_custItem.SelectedValue
        lblPrevCust_custItem.Text = lstCust_custItem.SelectedValue
        lblPrevOrderBy_custItem.Text = lstOrderBy_custItem.SelectedValue

    End Sub

    Private Sub showCustItemSearch()
        Using DB As New DBHelper
            'Show order form
            Common.ShowList(DB, "ITEMGRP", lstCustGrp_custItem.Text, lstItemGrpSearch_custItem, True, True, LST_ALL_STR)

            'TODO: Change
            'Show customer category
            'Common.ShowList(DB, "CTG", lstCustGrp_custItem.Text, lstCtgSearch_custItem, True, True, LST_ALL_STR)
            'Dim custGrpID As String = lstCustGrp_custItem.Text
            'Dim customerID As String = lstCust_custItem.Text
            'Dim jfcBranchNr As String = ""
            'Dim jfcCustomerNr As String = ""
            'Dim nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            'Dim isSmallCust = Common.IsSmallCustomer(DB, customerID)
            'Dim useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            'GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)

            Dim custinfo As CustomerInfo = ViewState("CUSTOMER_INFO")

            'GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)

            Dim sql = GetSQL_CustItem(DB, "CTG", custinfo, "")

            Common.ShowList(DB, sql, lstCtgSearch_custItem, True, True, LST_ALL_STR)
            'Show dropdownlist of venodrs
            Common.ShowList(DB, "VENDOR", lstCustGrp_custItem.Text, lstVendorSearch_custItem, True, True, LST_ALL_STR)
        End Using
    End Sub

    'YI FU 9/09/2020
    Private Sub showCustItemDel(ByVal GroupItemID As String, Optional CurCustInfo As CustomerInfo = Nothing)
        Dim SQL As String
        Dim custInfo As CustomerInfo
        If CurCustInfo Is Nothing Then
            custInfo = ViewState("CUSTOMER_INFO")
        Else
            custInfo = CurCustInfo
            ViewState("CUSTOMER_INFO") = CurCustInfo
        End If
        pnlPopWarningItem.Visible = False
        'Dim jfcBranchNr As String = ""
        'Dim jfcCustomerNr As String = ""

        'Dim custGrpID As String = lstCustGrp_custItem.Text
        'Dim customerID As String = lstCust_custItem.Text

        If custInfo.IsExcepPriceCustomer Then
            txtPopPrice.Visible = False
            lstPopPriceUnit.Visible = False
            btnPopDeleteItem.Visible = False
            btnPopSaveItem.Visible = True
            btnPopSaveItem.BackColor = Drawing.Color.Green

        Else
            btnPopDeleteItem.Visible = True
            'lblPopEditPriceNew.Visible = True
            lblPopTitlePriceNew.Visible = True
            txtPopPrice.Visible = True
            lstPopPriceUnit.Visible = True
        End If
        Dim csCost, eaCost, lbCost, gp As Decimal
        Dim btmcsPirce, btmeaPirce, btmlbPirce As Decimal
        Dim RegcsPrice, RegeaPrice, ReglbPrice As Decimal

        ClearSubControl(pnlPopItemDtl)
        txtPopPrice.Text = ""
        txtNote.Text = ""
        ckbPopEmail.Checked = False


        Using DB As New DBHelper

            SQL = GetSQL_CustItem(DB, "Item", custInfo, GroupItemID)

            Dim r As DataReader
            r = DB.GetReader(SQL)
            Dim scaledItem As Boolean
            Dim eaUnit As String

            ViewState("PopEditCustomerGrpId") = GroupItemID
            If r.Read() Then
                lblPopHeader_Item.Text = "Edit"
                btnPopSaveItem.Text = "Update"
                scaledItem = IsJfcScaledItem(DB, lblCustomerID.Text, CString(r("custGrpItem_id")))
                eaUnit = IIf(scaledItem = True, "pc", "ea")
                lblPopEditItem.Text = CString(r("item_id_o"))
                txtPopEditItem.Text = CString(r("item_id"))
                lblPopEditDesc.Text = Common.WrapText(CString(r("itemDesc_view")), IIf(IsMobile(), 20, 40))
                txtPopEditDesc.Text = CString(r("custItem_Desc"))
                txtPopEditRemark.Text = CString(r("jfcRemark"))
                lblPopEditEachCase.Text = IIf(Val(r("eaPerCs")) <> 0 And Val(r("eaPerCs")) <> 1 And Val(r("lbPrice_OrderByQty")) = 0, CString(r("eaPerCs")), "")
                pnlPoppcpercs.Visible = scaledItem
                lblPopEditInventory.Text = CString(r("csAvail_view")) & " /cs,  " & CString(r("eaAvail_view")) & " /ea"
                lblPopEditPcperCs.Text = If(Val(r("eaPrice_o")) <> 0, Math.Round(Val(r("csPrice_o")) / Val(r("eaPrice_o")), 0).ToString, "")



                csCost = Val(r("csCost"))
                btmcsPirce = Val(r("btmcsPrice"))
                RegcsPrice = Val(r("RegcsPrice"))
                RegeaPrice = Val(r("RegeaPrice"))
                ReglbPrice = Val(r("ReglbPrice"))
                Dim minSalCnt = IIf(Val(r("minSalCnt")) = 0, 1, Val(r("minSalCnt")))
                Dim lbsPerEa = IIf(Val(r("lbsPerEa")) = 0, 0.1, Val(r("lbsPerEa")))  ' GetJfcLbsPerEa(DB, lblCustomerID.Text, GroupItemID)
                Common.CovertPrice(csCost, eaCost, lbCost, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                Common.CovertPrice(btmcsPirce, btmeaPirce, btmlbPirce, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                'Common.CovertPrice(RegcsPrice, RegeaPrice, ReglbPrice, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                ViewState("btmcsPirce") = btmcsPirce
                'ViewState("btmeaPirce") = btmeaPirce
                'ViewState("btmlbPirce") = btmlbPirce
                ViewState("ShipDate") = CString(r("shipDate_FMT2"))

                lblPopEditCost.Text = Displayformat(Val(lbCost), Val(csCost), Val(eaCost), scaledItem)
                lblPopEditBtm.Text = Displayformat(Val(btmlbPirce), Val(btmcsPirce), Val(btmeaPirce), scaledItem)
                lblPopEditReg.Text = Displayformat(Val(ReglbPrice), Val(RegcsPrice), Val(RegeaPrice), scaledItem)
                lstPopPriceUnit.Items.Clear()
                If Val(lbCost) > 0 AndAlso scaledItem = True Then lstPopPriceUnit.Items.Add(New ListItem(" / lb", "LB"))
                If Val(csCost) > 0 Then lstPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
                If Val(eaCost) > 0 AndAlso scaledItem = False Then lstPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))


                lblPopEditPriceOriginal.Text = Displayformat(Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")), Val(r("eaPrice_o")), scaledItem) &
                    " " & Get_GP(Val(r("eaPerCs")), Val(r("cscost")), Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")))

                ' "<br /> " & IIf(scaledItem, If(Val(r("eaPrice_o")) <> 0, Math.Round(Val(r("csPrice_o")) / Val(r("eaPrice_o")), 0) & " pc/cs", ""), "")

                'lblPopEditPriceNew.Text = Displayformat(Val(r("lbPrice_OrderByQty")), Val(r("csPrice")), Val(r("eaPrice")), scaledItem) & " " & Get_GP(Val(r("eaPerCs")), Val(r("cscost")), Val(r("lbPrice_OrderByQty")), Val(r("csPrice")))

            Else
                'Dim smallCustomer As Boolean = IsSmallCustomer(DB, lblCustomerID.Text)
                '" || ', ' || TRIM(LEADING '0' FROM i.custGrpItem_id) AS itemDtl, " & _
                SQL = GetSQL_CustItem(DB, "NewItem", custInfo, GroupItemID)


                r = DB.GetReader(SQL, custInfo.jfcBranchNr, custInfo.jfcCustomerNr, custInfo.customerID, GroupItemID)
                If r.Read() Then

                    If custInfo.IsExcepPriceCustomer Then
                        MessageBox("This customer uses an exception price group, you cannot add items", "Error")
                        Exit Sub
                    End If
                    lblPopHeader_Item.Text = "Add"
                    'lblPopEditPriceNew.Visible = False
                    lblPopTitlePriceNew.Visible = False
                    scaledItem = IsJfcScaledItem(DB, custInfo.customerID, GroupItemID)
                    eaUnit = IIf(scaledItem = True, "pc", "ea")
                    lblPopEditItem.Text = CString(r("item_id"))
                    pnlPoppcpercs.Visible = scaledItem
                    lblPopEditDesc.Text = Common.WrapText(CString(r("itemDtl")), IIf(IsMobile(), 20, 40))

                    lblPopEditInventory.Text = CString(r("csAvail_view")) & " /cs,  " & CString(r("eaAvail_view")) & " /ea"
                    txtPopEditDesc.Text = ""
                    txtPopEditItem.Text = ""
                    txtPopEditRemark.Text = ""
                    txtPopPrice.Text = ""

                    csCost = Val(r("csCost"))
                    btmcsPirce = Val(r("btmcsPrice"))

                    RegcsPrice = Val(r("RegcsPrice"))
                    RegeaPrice = Val(r("RegeaPrice"))
                    ReglbPrice = Val(r("ReglbPrice"))
                    Dim minSalCnt = IIf(Val(r("minSalCnt")) = 0, 1, Val(r("minSalCnt")))
                    Dim lbsPerEa = IIf(Val(r("lbsPerEa")) = 0, 0.1, Val(r("lbsPerEa"))) 'lbsPerEa GetJfcLbsPerEa(DB, lblCustomerID.Text, GroupItemID)
                    Common.CovertPrice(csCost, eaCost, lbCost, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                    Common.CovertPrice(btmcsPirce, btmeaPirce, btmlbPirce, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                    'Common.CovertPrice(RegcsPrice, RegeaPrice, ReglbPrice, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))

                    ViewState("btmcsPirce") = btmcsPirce
                    'ViewState("btmeaPirce") = btmeaPirce
                    'ViewState("btmlbPirce") = btmlbPirce
                    ViewState("ShipDate") = "" 'CString(r("shipDate_FMT2"))

                    lblNewGp.Text = IIf(gp <> 1, "GP " & Math.Round(gp * 100, 1) & "%", "")
                    lstPopPriceUnit.Items.Clear()
                    If Val(lbCost) > 0 AndAlso scaledItem = True Then lstPopPriceUnit.Items.Add(New ListItem(" / lb", "LB"))
                    If Val(csCost) > 0 Then lstPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
                    If Val(eaCost) > 0 AndAlso scaledItem = False Then lstPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))

                    lblPopEditCost.Text = Displayformat(Val(lbCost), Val(csCost), Val(eaCost), scaledItem)
                    lblPopEditBtm.Text = Displayformat(Val(btmlbPirce), Val(btmcsPirce), Val(btmeaPirce), scaledItem)
                    lblPopEditReg.Text = Displayformat(Val(ReglbPrice), Val(RegcsPrice), Val(RegeaPrice), scaledItem)
                    lblPopEditPriceOriginal.Text = Displayformat(Val(r("lbPrice_OrderByQty")), Val(r("csPrice")), Val(r("eaPrice")), scaledItem) & " " &
                        Get_GP(Val(r("eaPerCs")), Val(r("cscost")), Val(r("lbPrice_OrderByQty")), Val(r("csPrice")))

                    lblPopEditPcperCs.Text = If(Val(r("eaPrice")) <> 0, Math.Round(Val(r("csPrice")) / Val(r("eaPrice")), 0).ToString, "")
                    btnPopSaveItem.Text = "Add"

                    btnPopDeleteItem.Visible = False

                    If custInfo.IsExcepPriceCustomer Then
                        lblPopHeader_Item.Text = "View"
                        'lblPopEditPriceNew.Visible = False
                        btnPopSaveItem.Text = "View"
                        btnPopSaveItem.Visible = False
                        btnPopDeleteItem.Visible = False
                    End If
                End If

            End If
            If lblPopEditCost.Text = String.Empty Then
                btnPopSaveItem.Visible = False
                pnlPopWarningItem.Visible = True
                lblPopWarningItem.Text = "Error! <br /> No cost information is available."
                lblPopWarningItem.ForeColor = Drawing.Color.Red
            Else
                btnPopSaveItem.Visible = True
            End If




            'If Session("ORDERDTL_CMD") <> "JFCADMIN" OrElse IsJfcItem(DB, lblNewCustGrpItemID.Text) = False Then Exit Sub
            'Init
            'ClearCtrl({txtPopPrice})
            'ClearCtrl({lblPopMsg_setPrice})
            'Check if it's scaled item

            'Show price unit (don't show / pc)

            'Show popup
            popDialog_Item.Show()
        End Using


    End Sub



    Private Function Get_GP(eaPerCs As Integer, csCost As Decimal, lbPrice_orderByQty As Decimal, csPrice As Decimal) As String
        'Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"
        Dim lbCost As Decimal = If(eaPerCs = 0, 0, Math.Round(csCost / eaPerCs * 10, 2))
        Dim result As String = ""
        If lbPrice_orderByQty <> 0 Then
            result = "GP: " & Math.Round((lbPrice_orderByQty - lbCost) / lbPrice_orderByQty, 2).ToString("P1")
        ElseIf csPrice = 0 Then
            result = ""
        Else
            result = "GP: " & Math.Round((csPrice - csCost) / csPrice, 3).ToString("P1")
        End If


        Return result

    End Function

    Public Function Displayformat(OrderByQty As Decimal, cs As Decimal, ea As Decimal, scaledItem As Boolean) As String
        Return IIf(OrderByQty > 0 AndAlso scaledItem = True, String.Format("{0:C}", OrderByQty) & " / lb, ", "") & _
                                       IIf(cs > 0, String.Format("{0:C}", cs) & " / cs, ", "") & _
                                       IIf(ea > 0, String.Format("{0:C}", ea) & " / " & IIf(scaledItem, "pc", "ea"), "") 'Don't show cost / pc

    End Function

    Protected Sub btnPopCloseItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPopCloseItem.Click
        setJavaScript(CustItem_Tab.CUSTITEM)
        showCustItemList()
        txtItem.Text = ""
    End Sub

    Protected Sub btnPopSaveItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPopSaveItem.Click
        Using DB As New DBHelper
            Try
                Dim subject As String
                Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
                setJavaScript(CustItem_Tab.CUSTITEM)
                Dim IsNew = If(btnPopSaveItem.Text = "Add", True, False)

                If IsNew And Val(txtPopPrice.Text) <= 0 Then
                    pnlPopWarningItem.Visible = True
                    lblPopWarningItem.Text = "Error! Please enter a Price to continue."
                    lblPopWarningItem.ForeColor = Drawing.Color.Red
                    popDialog_Item.Show()
                    Exit Sub
                End If
                Dim shipdate As DateTime
                If DateTime.TryParseExact(ViewState("ShipDate"), DATE_FMT, Nothing, Nothing, shipdate) And Not String.IsNullOrWhiteSpace(txtPopPrice.Text) Then
                    If DateTime.Today < shipdate Then
                        pnlPopWarningItem.Visible = True
                        lblPopWarningItem.Text = "Cannot change the item price as this item was ordered recently and ready to be shipped"
                        lblPopWarningItem.ForeColor = Drawing.Color.Red
                        popDialog_Item.Show()
                        Exit Sub
                    End If
                End If


                Dim sql = SetPrice(lstPopPriceUnit.Text, Val(txtPopPrice.Text), lblPopEditItem.Text, custInfo.customerID)
                Dim r = DB.GetReader(sql)
                If r.Read() Then
                    'Mod by jzhou on 1/11/2021
                    'If ViewState("btmcsPirce") > Val(r("csprice")) Then
                    If txtPopPrice.Text <> String.Empty AndAlso ViewState("btmcsPirce") > Val(r("csprice")) Then
                        pnlPopWarningItem.Visible = True
                        lblPopWarningItem.Text = "Error! Please enter a Price larger than bottom price"
                        lblPopWarningItem.ForeColor = Drawing.Color.Red
                        popDialog_Item.Show()
                        Exit Sub
                    End If

                    UpSertCustItem(DB, custInfo.customerID, ViewState("PopEditCustomerGrpId"), "Y", txtPopEditItem.Text, txtPopEditDesc.Text, txtPopEditRemark.Text, Val(r("csprice")), Val(r("eaprice")), 0, Val(r("lbPrice_OrderByQty")), IsNew)

                End If

                Dim emailresponse As String = ""

                If ViewState("PriceInquire") Then

                    UpdatePriceInquiryResponse(DB, ViewState("PopEditCustomerGrpId"), custInfo.customerID)
                    emailresponse = "Please place a new order if you need this item."
                End If


                If ckbPopEmail.Checked Then
                    Dim emailBody As String = ""
                    sql = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr, u.Language , " &
                        "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
                        "FROM webuser u, webuser_customer uc, customer c " &
                        "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
                        "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
                        "AND c.customer_id = '" & custInfo.customerID & "' order by c.customer_id"
                    Dim dt = DB.GetDataTable(sql)
                    Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
                    Dim r2 As DataReader
                    r2 = DB.GetReader(sql2, custInfo.jfcBranchNr)
                    Dim VendorEmail As String = ""
                    If r2.Read() Then
                        VendorEmail = CString(r2("email"))
                        'jfcBranchNr = CString(r("jfcBranchNr"))
                    End If
                    r2.close()
                    '2023
                    For Each dr In dt.Rows
                        Dim ci = New CultureInfo(IIf(CString(dr("language")) <> "", CString(dr("language")), "en-US").ToString)
                        Thread.CurrentThread.CurrentCulture = ci
                        Thread.CurrentThread.CurrentUICulture = ci
                        If ViewState("PriceInquire") Then
                            subject = String.Format(rm.GetString("Yourpriceinquiryregardingitem", ci), lblPopEditItem.Text) '"Your price inquiry regarding item #" & lblPopEditItem.Text
                            emailresponse = rm.GetString("Pleaseplaceaneworderifyouneedthisitem", ci)  '"Please place a new order if you need this item."
                        ElseIf IsNew Then
                            subject = String.Format(rm.GetString("Itemwasadded", ci), lblPopEditItem.Text)  '"Item #" & lblPopEditItem.Text & " was added"
                        Else
                            subject = String.Format(rm.GetString("Youriteminfowasupdated", ci), lblPopEditItem.Text) '"Your item info was updated (#" & lblPopEditItem.Text & ")"
                        End If
                        CreateEmailBody(Page, emailBody, "ITEM_DTL", custInfo.customerID, CString(dr("customerEmail")), custInfo.custGrpID, ViewState("PopEditCustomerGrpId"), emailresponse, txtNote.Text)
                        'Send confirmation email to user
                        'CreateEmailBody(Page, emailBody, "ITEM_DTL", "", "", "", "")
                        Dim errmsg As String = ""


                        If Common.SendMail(SYSTEM_EMAIL, IIf(IsProduction() Or IsProdBeta(), CString(dr("customerEmail")), "yfu@kmsnet.com"), VendorEmail, subject, emailBody, Nothing, True, errmsg) > 0 Then
                            'Email sent failed
                            'emailSent = False
                        Else
                            'Email sent OK
                            'emailSent = True
                        End If
                    Next
                    MessageLine("CONFIRM", "Successfully saved and sent email!")
                    'lblPopWarningItem.Text = "Successfully saved and sent email!"
                Else
                    MessageLine("CONFIRM", "Successfully saved")
                    'lblPopWarningItem.Text = "Successfully saved! "
                End If
                lblPopWarningItem.ForeColor = Drawing.Color.Green
            Catch ex As Exception
                Dim msg As String = ""
                Dim LastIndex As Integer = 0
                If ex.Message.Contains("max") Then LastIndex = ex.Message.LastIndexOf("max")
                Select Case True
                    Case ex.Message.Contains("CUSTITEMCODE")
                        If LastIndex > 0 Then
                            msg = "Item# is too long (" & ex.Message.Substring(LastIndex)
                        Else
                            msg = "Item# is too long!"
                        End If
                    Case ex.Message.Contains("CUSTITEMDESC")
                        If LastIndex > 0 Then
                            msg = "Desc is too long (" & ex.Message.Substring(LastIndex)
                        Else
                            msg = "Desc is too long!"
                        End If

                    Case ex.Message.Contains("CUSTREMARK")
                        If LastIndex > 0 Then
                            msg = "Remark is too long (" & ex.Message.Substring(LastIndex)
                        Else
                            msg = "Remark is too long!"
                        End If


                End Select
                lblPopWarningItem.Text = "Error! <br />" & msg
                lblPopWarningItem.ForeColor = Drawing.Color.Red
            End Try
            'showCustItemDel(ViewState("Current_custGrpItemID"))
            'showCustItemDel(ViewState("PopEditCustomerGrpId"))

            showCustItemList()
            'pnlPopWarningItem.Visible = True

            showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))


        End Using
    End Sub

    Protected Sub btnPopDeleteItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPopDeleteItem.Click
        Using DB As New DBHelper
            txtItem.Text = ""
            If btnPopDeleteItem.Text = "Close" Then
                setJavaScript(CustItem_Tab.CUSTITEM)
            Else
                setJavaScript(CustItem_Tab.CUSTITEM)
                ViewState("DIALOG_CMD") = "DELETE_CUSTITEM"
                ViewState("DIALOG_ARG1") = ViewState("PopEditCustomerGrpId")

                Dim msg = "Are you sure you want to remove item " & GetItemID(DB, ViewState("PopEditCustomerGrpId")) & " from " & lstCust_custItem.SelectedItem.Text & "?"
                MessageBoxYesNo(msg, "Confirm Deletion", DB)
                Exit Sub
            End If

            'If lblTextChanged_custItem.Value = "YES" Then
            '    'Confirm

            'Else
            '    setJavaScript(CustItem_Tab.CUSTITEM)
            '    'showCustItemList()
            'End If
        End Using
    End Sub

    'Protected Sub txtPrice_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPopPrice.TextChanged
    '    Dim price As Decimal = 0
    '    If (Decimal.TryParse(txtPopPrice.Text, price)) Then
    '        Get_GP(
    '    End If
    'End Sub

    Private Sub showCustItemList()
        'If ViewState("CustItem_Tab") = CustItem_Tab.CUSTITEM Then


        Dim SQL As String
        Dim whereClause As String
        'Dim jfcBranchNr As String = ""
        'Dim jfcCustomerNr As String = ""
        'Dim itemDescCol As String
        Dim custGrpID As String = lstCustGrp_custItem.Text
        Dim customerID As String = lstCust_custItem.Text
        Dim useCustomerCtg As Boolean
        'Dim nrCustGrpVendor As Integer
        'Dim isSmallCust As Boolean
        Dim Searchsql As String

        Using DB As New DBHelper
            'Init textchanged value
            If lblTextChanged_custItem.Value <> "YES" Then lblTextChanged_custItem.Value = ViewState("TEXT_CHANGED_CUSTITEM")
            If watItemSearch_custItem.WatermarkText = txtItemSearch_custItem.Text Then txtItemSearch_custItem.Text = String.Empty
            'Set properties

            'Get info
            'GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            'ViewState("jfcBranchNr") = jfcBranchNr

            'nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            'isSmallCust = Common.IsSmallCustomer(DB, customerID)

            Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
            useCustomerCtg = custInfo.useCustomerCtg
            ViewState("jfcCustomerNr") = custInfo.jfcCustomerNr
            Searchsql = " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE {0} OR a.itemDesc_ja LIKE {0} OR UPPER(a.itemDesc_long) LIKE {0} " & _
                        " OR UPPER(cc.CUSTITEMCODE) = {1} OR UPPER(i.item_id) = {1} OR UPPER(cc.CUSTITEMDESC) LIKE {0} OR cc.CUSTITEMDESC LIKE {0}) " 'OR UPPER(a.itemDesc_short) LIKE '%{0}%' 

            'Get part of SQL (added i.custgrp_id = " & mDB.SqlEsc(lstCustGrp_custItem.Text) to improve speed)
            'whereClause = "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
            '                "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
            '                "  AND i.custgrp_id = " & DB.SqlEsc(lstCustGrp_custItem.Text) & _
            '                "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
            '                "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
            '                "  AND ci.customer_id = " & DB.SqlEsc(lstCust_custItem.Text) & _
            '                IIf(lstVendorSearch_custItem.Text <> LST_ALL_STR, "  AND v.vendorName = " & DB.SqlEsc(lstVendorSearch_custItem.Text), "") & _
            '                IIf(lstCtgSearch_custItem.Text <> LST_ALL_STR, "  AND c.ctgName = " & DB.SqlEsc(lstCtgSearch_custItem.Text), "") & _
            '                IIf(lstItemGrpSearch_custItem.Text <> LST_ALL_STR, "  AND g.itemGrpName = " & DB.SqlEsc(lstItemGrpSearch_custItem.Text), "") & _
            '                IIf(txtItemSearch_custItem.Text <> String.Empty, "  AND (i.item_id LIKE " & DB.SqlEsc(txtItemSearch_custItem.Text & "%") & " OR UPPER(i.itemNr) LIKE " & DB.SqlEsc(txtItemSearch_custItem.Text.ToUpper & "%") & " OR UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE " & DB.SqlEsc("%" & txtItemSearch_custItem.Text.ToUpper & "%") & " OR UPPER(i.CustItemDesc) LIKE " & DB.SqlEsc("%" & txtItemSearch_custItem.Text.ToUpper & "%") & ")", "")
            whereClause = "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                            "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                            "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                            "  AND g.custgrp_id = " & DB.SqlEsc(custGrpID) & _
                            "  AND ci.customer_id = " & DB.SqlEsc(customerID) & " " & _
                            "  AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) " & _
                            "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                            "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') " & _
                            IIf(lstVendorSearch_custItem.Text <> LST_ALL_STR, "  AND v.vendorName = " & DB.SqlEsc(lstVendorSearch_custItem.Text), "") & _
                            IIf(lstItemGrpSearch_custItem.Text <> LST_ALL_STR, " AND g.itemGrpName = " & DB.SqlEsc(lstItemGrpSearch_custItem.Text), "") & _
                            IIf(lstCtgSearch_custItem.Text <> LST_ALL_STR, "  AND NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) = " & DB.SqlEsc(lstCtgSearch_custItem.Text), "") & _
                            IIf(txtItemSearch_custItem.Text <> "", String.Format(Searchsql, DB.SqlEsc("%" & Replace(txtItemSearch_custItem.Text.ToUpper, " ", "%") & "%"), DB.SqlEsc(txtItemSearch_custItem.Text.ToUpper)), "")

            'IIf(txtItemSearch_custItem.Text <> "", " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE '%" & Replace(txtItemSearch_custItem.Text.ToUpper, " ", "%") & "%'" & " OR UPPER(a.itemDesc_short) LIKE '%" & Replace(txtItemSearch_custItem.Text.ToUpper, " ", "%") & "%'" & " OR UPPER(a.itemDesc_long) LIKE '%" & Replace(txtItemSearch_custItem.Text.ToUpper, " ", "%") & "%'" & " OR a.itemDesc_ja LIKE '%" & Replace(txtItemSearch_custItem.Text, " ", "%") & "%')", "")
            'IIf(lblShowOnlyRecentOrder.Text = "Y", " AND (q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0) ", "") & _
            'itemDescCol = "a.itemDesc" 'IIf(IsMobile(), "a.itemDesc_short", "a.itemDesc")

            'Store for print order form
            Session("PRINT_TYPE") = "ORDER_FORM"
            Session("PRINT_WHERECLAUSE") = whereClause & " AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) "
            Session("PRINT_ARG1") = lstCust_custItem.Text
            'Session("PRINT_ORDERBYCLAUSE") = orderByClause
            'Session("PRINT_ARG2") = lstOrderBy_custItem.SelectedItem.Text

            'Show customer's item list
            'GetJFCBranchAndCustomerNr(DB, lstCust_custItem.Text, jfcBranchNr, jfcCustomerNr)
            'SQL = "SELECT ci.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.dispOrder itemGrpDispOrder, g.itemGrpName, c.dispOrder ctgDispOrder, c.ctgName, " & _
            '             " i.item_id, i.itemNr, i.CustItemDesc, v.vendorName, i.minSalCnt, " & _
            '             " ci.eaPerCs, ci.csPrice, ci.eaPrice, ci.lbPrice_OrderByWgt, ci.lbPrice_OrderByQty, " & _
            '             " CASE WHEN i.jfcitem = 'Y' THEN jctgGrp.ctgGrpDesc ELSE NULL END ctgDesc, " & _
            '             " CASE WHEN i.jfcitem = 'Y' THEN a.itemDesc ELSE NULL END itemDesc, " & _
            '             " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
            '             " i.defEaPerCs, i.defCsPrice, i.defEaPrice, i.defLbPrice_OrderByWgt, i.defLbPrice_OrderByQty " & _
            '             "FROM custGrp_itemGrp g, custGrp_vendor v, customer_custGrpItem ci, custgrp_item i " & _
            '             "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id AND i.jfcItem = 'Y' " & _
            '             "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
            '             "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " & _
            '             "LEFT OUTER JOIN custCompany_ctg c ON c.ctg_id = i.ctg_id " & _
            '             "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " & _
            '             whereClause
            ''"LEFT OUTER JOIN jfcCtg jctg ON jctg.ctg_id = a.ctg_id " & _
            ''"WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
            ''"  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
            ''"  AND i.custGrpItem_id = ci.custGrpItem_id " & _
            ''"  AND ci.customer_id = " & DB.SqlEsc(lstCust_custItem.Text)
            'dsCustItem.ProviderName = "System.Data.OracleClient"
            'dsCustItem.ConnectionString = DB.GetConnectionString
            'dsCustItem.SelectCommand = SQL
            'grdCustItem.DataSourceID = "dsCustItem"
            'grdCustItem.Sort(lstOrderBy_custItem.SelectedValue, SortDirection.Ascending)
            'grdCustItem.DataBind()
            'useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            'custInfo = New CustomerInfo
            SQL = GetSQL_CustItem(DB, "List", custInfo, "")
            SQL &= " ORDER BY " & lstOrderBy_custItem.SelectedValue

            'End If
            dsCustItem.ProviderName = "System.Data.OracleClient"
            dsCustItem.ConnectionString = DB.GetConnectionString
            dsCustItem.SelectCommand = SQL
            dsCustItem.SelectParameters.Clear()
            If SQL.Contains(":itemdesc") Then
                dsCustItem.SelectParameters.Add(":itemdesc", "%" & txtItemSearch_custItem.Text.ToUpper & "%")
            End If

            grdCustItem.DataSourceID = "dsCustItem"
            'If IsPostBack = False Then grd.Sort("CTGNAME_VIEW, VENDORNAME, ITEMNR, ITEM_ID, ITEMGRPNAME", SortDirection.Ascending)
            grdCustItem.DataBind()
            grdCustItem.Columns(GetColumnIndex(grdCustItem, "ctgName_view")).Visible = Not IsMobile()
            'Init textchanged value
            initTextChanged_custItem()
        End Using
        'End If
    End Sub

    Public Shared Function encode(str As String) As String
        Dim utf8Encode As New System.Text.UTF8Encoding
        Dim encodestring() As Byte
        encodestring = utf8Encode.GetBytes(str)
        Return utf8Encode.GetString(encodestring)
    End Function

    Protected Function GetSQL_CustItem(db As DBHelper, cmd As String, CustInfo As CustomerInfo, ByVal GroupItemID As String) As String

        Dim showOnlyRecentOrder4ExcepCust As Boolean
        Dim custGrpID As String = CustInfo.custGrpID
        Dim isExcepPriceCustomer As Boolean = CustInfo.IsExcepPriceCustomer
        Dim PriceGrpId = CustInfo.PriceGrpID
        Dim nrCustGrpVendor = CustInfo.nrCustGrpVendor
        Dim UseCustomerCtg = CustInfo.useCustomerCtg
        Dim isSmallCust = CustInfo.isSmallCust
        Dim customerid = CustInfo.customerID
        Dim jfcBranchNr = CustInfo.jfcBranchNr
        Dim jfcCustomerNr = CustInfo.jfcCustomerNr
        If isExcepPriceCustomer = False Then
            showOnlyRecentOrder4ExcepCust = False
        Else
            showOnlyRecentOrder4ExcepCust = IIf(CustInfo.IsApplyInCustomer, False, True)
        End If
        ''If there are too many items in exception price group, show only recent ordered items
        'SQL = "SELECT COUNT(*) from " & IIf(IsSmallCustomer() = True, IIf(isExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small"), "customer_custGrpItem") & " " &
        '                "WHERE NOT (csPrice = 0 and eaPrice = 0 and lbPrice_orderByWgt = 0 and lbPrice_orderByQty = 0) AND customer_id = " & db.SqlEsc(customerid)
        'showOnlyRecentOrder = IIf(Val(db.GetScalar(SQL)) > 800, True, False)

        Dim Searchsql = " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE {0} OR a.itemDesc_ja LIKE {0} OR UPPER(a.itemDesc_long) LIKE {0} " &
                            " OR UPPER(a.itemWebDesc) LIKE {0} OR UPPER(a.salesPoint) LIKE {0} " &
                            " OR UPPER(cc.CUSTITEMCODE) = {1} OR UPPER(i.item_id) = {1} OR UPPER(cc.CUSTITEMDESC) LIKE {0} OR cc.CUSTITEMDESC LIKE :itemdesc) " 'OR UPPER(a.itemDesc_short) LIKE '%{0}%' 
        '" Or UPPER(a.itemWebDesc_roman) Like {0}" & 'Relaced it with salesPoint by jzhou on 6/10/2025


        Dim SQL_Select = "SELECT /*+rule*/ " & _
                 " v.vendorName, " & IIf(nrCustGrpVendor > 1, "v.vendorName", "NULL") & " AS vendorName_view, " & _
                 " NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) AS ctgName_view, NVL(jctgGrp2.dispOrder, jctgGrp.dispOrder) AS ctgDispOrder, " & _
                 IIf(UseCustomerCtg, " NVL(customCtg.dispOrder, 0)", "0") & " AS itemDispOrder, " & _
                 " q.shipDate, i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.itemGrpName," & _
                 " g.dispOrder itemGrpDispOrder, " & _
                 IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " AS itemNr, i.CustItemDesc, v.custGrpVendor_id," & _
                 " i.minSalCnt,  cp.webCustRemark, " & _
                 " ci.eaPerCs, " & _
                 " q.csQty AS lastCsQty, CASE WHEN NVL(q.pcQty, 0) > 0 THEN q.pcQty ELSE q.eaQty END AS lastEaQty, q.lbs AS lastLbs," & _
                 " CASE WHEN q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0 THEN 'Y' ELSE 'N' END ordered, " & _
                 " CASE WHEN NVL(q.csQty, 0) > 0 OR NVL(q.eaQty, 0) > 0 OR NVL(q.lbs, 0) > 0 THEN 'Y' ELSE 'N' END AS lastQtyExist, " & _
                 " TO_CHAR(q.shipDate, " & db.SqlEsc(SHORTDATE_FMT) & ") AS shipDate_fmt, TO_CHAR(q.shipDate, " & db.SqlEsc(DATE_FMT) & ") AS shipDate_fmt2," &
                 " cc.ENABLED," & _
                 " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, p.lbsPerEa, " & _
                 " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " & _
                 " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
                 " CASE WHEN " & IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " || '<br />(' || i.item_id || ')' END AS itemIdNr_view, " & _
                 " CASE WHEN ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0 THEN 'Y' ELSE 'N' END askPrice, " & _
                 IIf(Common.ShowItemDtl(db, customerid) = True, " TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2), 99999.99) || ' kg, ' || TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3), 99999.999) || ' m³'", "NULL") & " AS itemDtl, "

        Dim sql_from = " FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(isSmallCust, IIf(isExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
               "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
               IIf(UseCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
                                           "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id ", "") & _
               IIf(UseCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & db.SqlEsc(customerid) & ", i.item_id, 'Y') " & _
                                          "INNER JOIN customer_ctg customCtg ON customCtg.ctgGrp_id = jctgGrp.ctgGrp_id AND customCtg.customer_id = " & db.SqlEsc(customerid) & " AND customCtg.item_id = i.item_id ", "") & _
               "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & db.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & db.SqlEsc(jfcCustomerNr) & " " & _
               "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & db.SqlEsc(PriceGrpId) & " " & _
               IIf(UseCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg2 ON jSubCtg2.ctg_id = p.ctg_id_branch AND jSubCtg2.subCtg_id = p.subCtg_id_branch " &
                                            "LEFT OUTER JOIN jfcCtgGrp jctgGrp2 ON jctgGrp2.ctgGrp_id = jSubCtg2.ctgGrp_id ", "") &
               IIf(UseCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp2 ON jctgGrp2.ctgGrp_id = GetCtgGrpID(" & db.SqlEsc(customerid) & ", i.item_id, 'Y') " & _
                                          "INNER JOIN customer_ctg customCtg ON customCtg.ctgGrp_id = jctgGrp.ctgGrp_id AND customCtg.customer_id = " & db.SqlEsc(customerid) & " AND customCtg.item_id = i.item_id ", "") & _
               "LEFT OUTER JOIN jfcPrice jp ON i.jfcItem ='Y' AND jp.item_id = i.item_id AND jp.pricegrp_id = " & db.SqlEsc(PriceGrpId) & " " & _
               "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & db.SqlEsc(customerid) & " " & _
               "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.CUSTGRPITEM_ID AND cc.customer_id = " & db.SqlEsc(customerid) & " " & _
               "LEFT OUTER JOIN customer_itemDispOrder co ON i.CUSTGRPITEM_ID = co.custgrpitem_id and co.customer_id =  " & db.SqlEsc(customerid) & " "

        Dim SQL_eaCost = "(CASE WHEN ci.eaPerCs <= 1 THEN 0 ELSE " & _
                              "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN ci.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/ci.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                              " ELSE (CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs, 2) END) END) " & _
                           "END)"
        Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"

        Dim whereClause = "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                      "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                      "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                      "  AND g.custgrp_id = " & db.SqlEsc(custGrpID) & _
                      "  AND ci.customer_id = " & db.SqlEsc(customerid) & " " & _
                      "  AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) " & _
                      "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                      "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') " & _
                      IIf(showOnlyRecentOrder4ExcepCust = True, " AND (q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0) ", "")
        Select Case cmd
            Case "CTG"
                SQL_Select = "Select NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) AS name, NVL(jctgGrp2.ctgGrp_id, jctgGrp.ctgGrp_id) AS id "
                whereClause &= "group by NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc), NVL(jctgGrp2.ctgGrp_id, jctgGrp.ctgGrp_id)"

            Case "List"
                SQL_Select &= " i.item_id as item_id,cc.CUSTITEMCODE as item_id_n, TRIM(i.jfcRemark || ' ' || cc.CUSTREMARK || ' ' || cp.webCustRemark) as jfcRemark, cc.CUSTREMARK as jfcRemark_n, " & _
                    "ci.csPrice as csPrice,cc.CUSTCSPRICE as csPrice_n, ci.eaPrice as eaPrice, cc.CUSTEAPRICE as eaPrice_n," & _
                    "ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt, " & _
                    "ci.lbPrice_OrderByQty as lbPrice_OrderByQty, cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty_n, " & _
                    "CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view," & _
                    "cc.CUSTITEMDESC as itemDesc_view_n, co.disporder as custdisporder"
                '"CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(" & IIf(IsMobile(), "a.itemDesc_short", "a.itemDesc") & ", a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view," & _
                whereClause &= IIf(lstVendorSearch_custItem.Text <> LST_ALL_STR, "  AND v.vendorName = " & db.SqlEsc(lstVendorSearch_custItem.Text), "") & _
                      IIf(lstItemGrpSearch_custItem.Text <> LST_ALL_STR, " AND g.itemGrpName = " & db.SqlEsc(lstItemGrpSearch_custItem.Text), "") & _
                      IIf(lstCtgSearch_custItem.Text <> LST_ALL_STR, "  AND NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) = " & db.SqlEsc(lstCtgSearch_custItem.Text), "") & _
                      SearchbyEachWord(db, Searchsql, txtItemSearch_custItem.Text.ToUpper)

                'IIf(txtItemSearch_custItem.Text <> "", String.Format(Searchsql, Replace(txtItemSearch_custItem.Text.ToUpper, " ", "%")), "")
            Case "Item", "Item_Copy"
                SQL_Select &= " NVL(p.csCost, 0) AS csCost, " & _
                             " " & SQL_eaCost & " AS eaCost, " & _
                             " " & SQL_lbCost & " AS lbCost, " & _
                             " NVL(p.btmcsPrice, 0) AS btmcsPrice, " & _
                               " NVL(jp.csprice,0) as regcsPrice, " & _
                                " NVL(jp.eaprice,0) as regeaPrice, " & _
                                 " NVL(jp.lbPrice_orderByQty,0) as reglbPrice, " & _
                               " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                 " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view," & _
                 " CASE WHEN ci.lbPrice_orderByQty <> 0 THEN ROUND((ci.lbPrice_orderByQty - " & SQL_lbCost & ") / ci.lbPrice_orderByQty, 2) " & _
                 " ELSE (CASE WHEN NVL(ci.csPrice, 0) = 0 THEN 1 ELSE ROUND((ci.csPrice - NVL(p.csCost, 0)) / ci.csPrice, 2) END) END gp," & _
                 " cc.CUSTITEMCODE as item_id,i.item_id as item_id_o, cc.CUSTREMARK as jfcRemark,cc.CUSTCSPRICE as csPrice, " & _
                 " ci.csPrice as csPrice_o, cc.CUSTEAPRICE as eaPrice, ci.eaPrice as eaPrice_o," & _
                 " cc.CUSTLBPRICE_ORDERBYWGT as lbPrice_OrderByWgt,ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt_o,  cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty,ci.lbPrice_OrderByQty as lbPrice_OrderByQty_o," & _
                 " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
                '" CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(" & IIf(IsMobile(), "a.itemDesc_short", "a.itemDesc") & ", a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
                whereClause &= IIf(cmd = "Item", " AND i.custGrpItem_id = '" & GroupItemID & "'", " AND i.item_id = '" & GroupItemID.Substring(GroupItemID.Length - 5) & "'")

            Case "NewItem", "NewItem_Copy"
                SQL_Select = "SELECT i.item_id, i.minSalCnt, (CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END) AS itemDtl, p.lbsPerEa," & _
                       " i.custGrp_id,i.custGrpItem_id, i.vendor_id, i.item_id, i.itemNr, g.itemGrpName, ci.eaPerCs,ci.csPrice,ci.eaPrice,ci.lbPrice_OrderByWgt, ci.lbPrice_OrderByQty," & _
                       " NVL(p.csCost, 0) AS csCost, " & _
                       " " & SQL_eaCost & " AS eaCost, " & _
                       " " & SQL_lbCost & " AS lbCost, nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt," & _
                       " NVL(p.btmcsPrice,0) as btmcsPrice, " & _
                       " NVL(jp.csprice,0) as regcsPrice, " & _
                       " NVL(jp.eaprice,0) as regeaPrice, " & _
                       " NVL(jp.lbPrice_orderByQty,0) as reglbPrice, " & _
                       " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                       " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view "
                sql_from = "FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(isSmallCust = True, IIf(isExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
                     "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                     "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = :1 AND cp.customerid = :2 " & _
                     "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & db.SqlEsc(PriceGrpId) & " " & _
                     "LEFT OUTER JOIN jfcPrice jp ON i.jfcItem ='Y' AND jp.item_id = i.item_id AND jp.pricegrp_id = " & db.SqlEsc(PriceGrpId) & " "
                whereClause = "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                             "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                             "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                             "  AND g.custgrp_id = " & db.SqlEsc(custGrpID) & _
                             "  AND ci.customer_id = :3" &
                             IIf(cmd = "NewItem", "  AND i.custGrpItem_id = :4 ", " AND i.item_id = :4 ") & _
                             "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                             " ORDER BY itemDtl "

            Case "ExportList"
                SQL_Select &= " i.item_id as item_id,cc.CUSTITEMCODE as item_id_n, TRIM(i.jfcRemark || ' ' || cc.CUSTREMARK || ' ' || cp.webCustRemark) as jfcRemark, cc.CUSTREMARK as jfcRemark_n, " & _
                   "ci.csPrice as csPrice,cc.CUSTCSPRICE as csPrice_n, ci.eaPrice as eaPrice, cc.CUSTEAPRICE as eaPrice_n," & _
                   "ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt, " & _
                   "ci.lbPrice_OrderByQty as lbPrice_OrderByQty, cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty_n, " & _
                   "CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view," & _
                   "cc.CUSTITEMDESC as itemDesc_view_n, co.disporder as custdisporder, jsm.methdesc as methdesc, NVL(jSubCtg2.subctgdesc, jSubCtg.subctgdesc) subctgdesc, NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) ctgdesc " 'Changed from jctg.ctgdesc to NVL(jctgGrp2.ctgGrpDesc, jctgGrp.ctgGrpDesc) on 7/11/2023

                sql_from &= " LEFT OUTER JOIN JFCStoreMethod jsm on jsm.storemethid = p.storemethid LEFT OUTER JOIN jfcCtg jctg ON jctg.ctg_id = a.ctg_id "

                '    " TRIM(i.jfcRemark || ' ' || cc.CUSTREMARK || ' ' || cp.webCustRemark) as jfcRemark, cc.CUSTREMARK as jfcRemark_n, " & _
                '"ci.csPrice as csPrice,cc.CUSTCSPRICE as csPrice_n, ci.eaPrice as eaPrice, cc.CUSTEAPRICE as eaPrice_n," & _
                '"ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt, " & _
                '"ci.lbPrice_OrderByQty as lbPrice_OrderByQty, cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty_n, " & _


        End Select

        Return SQL_Select & sql_from & whereClause
    End Function

    Protected Sub exportExcel(ds As DataSet)
        Dim book As New ExcelWorkbook(ExcelWorkbookFormat.Xls_2003)

        book.LicenseKey = "ORIIGQoIGQgPDxkLFwkZCggXCAsXAAAAAA=="

        Dim vFileName As String = "CustomerItemDisplayOrder_" & DateTime.Now.ToString("yyyyMMdd") & ".xls"
        Dim lstHeader As New List(Of String)
        Dim vMaxCol As Integer

        For i As Integer = 0 To ds.Tables.Count - 1
            Dim sheet As ExcelWorksheet
            book.Worksheets.AddWorksheet()
            sheet = book.Worksheets(i)
            Dim dt = ds.Tables(i)
            vMaxCol = dt.Columns.Count + 10
            sheet.Name = dt.TableName
            'sheet(1, 1).Text = "JFC INTERNATIONAL INC."
            'sheet(2, 1).Text = "Report: " & dt.TableName
            'Dim str As String = ""
            ''If txtFromDate.Text <> "" Then
            ''    str &= "From:" & txtFromDate.Text & " "
            ''End If
            ''If txtToDate.Text <> "" Then
            ''    str &= "To:" & txtToDate.Text & " "
            ''End If
            'sheet(3, 1).Text = str
            'sheet(4, 1).Text = "Date:" & DateTime.Now.ToShortDateString

            For cnt = 0 To dt.Columns.Count - 1
                With dt.Columns(cnt)
                    SetExcelSheetTextVal(sheet, 1, cnt + 1, .ColumnName)
                End With
            Next
            For cntRow = 0 To dt.Rows.Count - 1
                With dt.Rows(cntRow)
                    For cnt = 0 To dt.Columns.Count - 1
                        'If .GetType.ToString() = "String" Then
                        SetExcelSheetTextVal(sheet, cntRow + 2, cnt + 1, CString(.Item(cnt)))
                        'Else
                        'SetExcelSheetNumericVal(sheet, cntRow + 6, cnt + 1, .Item(cnt))

                        'End If
                    Next
                    '                SetExcelSheetCodeType2(sheet, 6 + cntRow, .Cells(0).Text, .Cells(2).Text, vMaxCol)
                End With
            Next
            sheet.AutofitColumns()
        Next


        Response.Clear()
        Response.ContentType = "Application/x-msexcel"
        Response.AddHeader("Content-Disposition", "attachment; filename=" + vFileName)
        book.Save(Response.OutputStream)

        book.Close()

        Response.End()

    End Sub

    Private Sub SetExcelSheetTextVal(ByRef pSheet As ExcelWorksheet, pRow As Integer, pCol As Integer, pTextVal As String)
        pSheet(pRow, pCol).Text = pTextVal.Replace("&#39;", "'").Replace("&amp;", "&").Replace("<br/>", vbCrLf & " ").Replace("&nbsp;", "")
    End Sub

    'Private Sub uploadExcelSheet()

    '    ''If FileUpload.HasFile Then
    '    'Using db As New DBHelper
    '    '    vFileName = FileUpload.PostedFile.FileName
    '    '    FileUpload.SaveAs(vFileSaveDir + vFileName)
    '    '    book = New ExcelWorkbook(vFileSaveDir + vFileName)
    '    '    sheet = book.Worksheets(book.ActiveSheetIndex)

    '    '    UploadTmpDataTable(sheet, CInt(lstSheet.SelectedValue))


    '    '    book.Close()
    '    '    System.IO.File.Delete(vFileSaveDir + vFileName)


    '    'End Using
    '    'Else
    '    'Dim s As System.Web.UI.Page = HttpContext.Current.Handler
    '    'Dim script As String = "SystemDialogMessage = 'Please select the upload file.';"
    '    's.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    '    'End If
    'End Sub

    Private Sub uploadExcelSheet(Sheet As ExcelWorksheet)
        Dim vCntRow As Integer = 2
        Dim itemlist As List(Of String) = New List(Of String)
        Using db As New DBHelper
            If CString(Sheet(vCntRow, 1).Text).Length = 12 Then

                DeleteCustItemDispOrder(db, lstCust_custItem.Text)
                While (Sheet(vCntRow, 1).Text <> "")

                    Dim itemid As String = Sheet(vCntRow, 1).Text.Split(",")(0)
                    If Not itemlist.Contains(itemid) Then
                        InsertCustItemDispOrder(db, lstCust_custItem.Text, itemid, vCntRow - 7)
                    End If
                    itemlist.Add(itemid)
                    vCntRow += 1
                End While
            Else
                MessageBox("Please select .xls file that you exported ", "Error")
                Exit Sub
            End If

        End Using
    End Sub

    Private Function SearchbyEachWord(db As DBHelper, Sql As String, keyword As String) As String
        Dim result As String = ""
        Dim words As String()
        If Not String.IsNullOrWhiteSpace(keyword.Trim()) Then
            words = keyword.Trim().Split(New Char() {" ", ",", "."})
            For i As Integer = 0 To words.Length - 1
                If Not String.IsNullOrWhiteSpace(words(i)) Then
                    result &= String.Format(Sql, db.SqlEsc("%" & words(i) & "%"), db.SqlEsc(words(i)))
                End If

            Next

        End If
        Return result
    End Function

    Private Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExportExcel.Click
        Dim ds As New DataSet
        setJavaScript(CustItem_Tab.CUSTITEM)
        Using db As New DBHelper
            'Dim custGrpID As String = lstCustGrp_custItem.Text
            'Dim customerID As String = lstCust_custItem.Text
            'Dim jfcBranchNr As String = ""
            'Dim jfcCustomerNr As String = ""
            'Dim nrCustGrpVendor = GetNrCustGrpVendor(db, custGrpID)
            'Dim isSmallCust = Common.IsSmallCustomer(db, customerID)
            'Dim useCustomerCtg = Common.UseCustomerCtg(db, customerID)
            'GetJFCBranchAndCustomerNr(db, customerID, jfcBranchNr, jfcCustomerNr)
            Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
            Dim sql = GetSQL_CustItem(db, "ExportList", custInfo, "")
            sql &= " ORDER BY " & lstOrderBy_custItem.SelectedValue

            Dim dt As New DataTable
            Dim dr As DataRow
            Dim r As DataReader
            r = db.GetReader(sql)
            dt.TableName = "Custom Item List"
            dt.Columns.Add(New DataColumn("ID", GetType(String)))
            dt.Columns.Add(New DataColumn("Item #", GetType(String)))
            dt.Columns.Add(New DataColumn("Item Desc", GetType(String)))
            dt.Columns.Add(New DataColumn("JFC category", GetType(String)))
            dt.Columns.Add(New DataColumn("JFC Sub category", GetType(String)))
            dt.Columns.Add(New DataColumn("Last Order Date", GetType(String)))
            dt.Columns.Add(New DataColumn("Store Method", GetType(String)))

            While r.Read()
                '"SELECT /*+rule*/ " & _
                '         " i.item_id as item_id,cc.CUSTITEMCODE as item_id_n, " & _
                '          "CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view," & _
                '         "cc.CUSTITEMDESC as itemDesc_view_n, " & _
                '         "jctgGrp.ctgGrpDesc AS ctgName_view, " & _
                '         "TO_CHAR(q.shipDate, " & db.SqlEsc(DATE_FMT) & ") AS shipDate_fmt2,
                dr = dt.NewRow()
                dr(0) = CString(r("custGrpItem_id"))
                dr(1) = CString(r("item_id")) & IIf(CString(r("item_id_n")) = String.Empty, "", ", (" & CString(r("item_id_n")) & ")")
                dr(2) = CString(r("itemDesc_view")) & IIf(CString(r("itemDesc_view_n")) = String.Empty, "", ", (" & CString(r("itemDesc_view_n")) & ")")

                dr(3) = CString(r("ctgdesc"))
                dr(4) = CString(r("subctgdesc"))
                dr(5) = CString(r("shipDate_fmt2"))
                dr(6) = CString(r("methdesc"))
                dt.Rows.Add(dr)
            End While
            ds.Tables.Add(dt)

            exportExcel(ds)
        End Using

    End Sub

    Private Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim book As ExcelWorkbook
        Dim sheet As ExcelWorksheet

        Dim vFileSaveDir As String = Request.PhysicalApplicationPath & "Files\TemporaryFiles\" & GetUserID() & "_"
        Dim vFileName As String
        'Dim vCntRow As Integer
        'Dim vYYYYMM As String
        If fileUpExcel.HasFile Then
            vFileName = fileUpExcel.PostedFile.FileName
            fileUpExcel.SaveAs(vFileSaveDir + vFileName)
            book = New ExcelWorkbook(vFileSaveDir + vFileName)
            sheet = book.Worksheets(book.ActiveSheetIndex)
            uploadExcelSheet(sheet)
            showCustItemList()
        Else
            MessageBox("Please select an .xls file in the file upload", "Error")
            Exit Sub
        End If
    End Sub

    Private Sub btnGetItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetItem.Click

        setJavaScript(CustItem_Tab.CUSTITEM)

        If txtItem.Text.Contains(",") = False Then
            MessageBox("Please select an item from the auto-complete list", "Error")
            Exit Sub
        End If


        Dim custGrpItemID = GetLastWord(txtItem.Text, ",").PadLeft(CUSTGRP_ITEM_ID_LEN, "0")
        'ViewState("Current_custGrpItemID") = custGrpItemID
        showCustItemDel(custGrpItemID)
        txtItem.Text = ""
        ViewState("PriceInquire") = False
    End Sub

    Private Sub btnClearItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearItem.Click
        setJavaScript(CustItem_Tab.CUSTITEM)
        txtItem.Text = ""
        'clearQtyEntry()
        'clearItemDtl()
    End Sub

    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        Dim id As String = dialogArg1

        Using DB As New DBHelper
            Select Case dialogCmd
                Case "DELETE_CUSTITEM"
                    Dim Shipdate As DateTime
                    If DateTime.TryParseExact(ViewState("ShipDate"), DATE_FMT, Nothing, Nothing, Shipdate) Then
                        If DateTime.Now < Shipdate Then
                            pnlPopWarningItem.Visible = True
                            lblPopWarningItem.Text = "Cannot delete this item as this item was ordered recently and ready to be shipped"
                            lblPopWarningItem.ForeColor = Drawing.Color.Red
                            popDialog_Item.Show()
                            Exit Sub
                        End If
                    End If
                    setJavaScript(CustItem_Tab.CUSTITEM)
                    DeleteCustomerCustItem(DB, lblCustomerID.Text, id)
                    showCustItemList()

                    InsertEventLog(DB, "Deleted customer's item", Nothing, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                    'Case "DE"
                Case "DELETE_PRICEINQUIRY"
                    setJavaScript(CustItem_Tab.CUSTITEM)
                    'DeleteCustomerCustItem(DB, lblCustomerID.Text, id)
                    For Each row As GridViewRow In grdPriceInquire.Rows
                        Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                        Dim lblItem = CType(row.FindControl("lblCustGrpItemID"), Label)
                        Dim lblcustomerid = CType(row.FindControl("lblcustomerID"), Label)
                        If CheckBoxchckrw.Checked Then

                            UpdatePriceInquiryResponse(DB, lblItem.Text, lblcustomerid.Text, "D")
                        End If
                    Next
                    'UpdatePriceInquiryResponse(DB, ViewState("PopEditCustomerGrpId"), lstCust_custItem.Text, "D")
                    showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))

                    InsertEventLog(DB, "Deleted Price Inquiry's item", Nothing, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                Case "DELETE_PROMOTIONITEM"
                    setJavaScript(CustItem_Tab.PROMOITEM)
                    For Each row As GridViewRow In grdPromoItemlst.Rows
                        Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                        Dim lblItem = CType(row.FindControl("lblItemId"), Label)
                        If CheckBoxchckrw.Checked Then
                            'checkselected = True
                            DeleteCustPromoItem(DB, lblCustomerID.Text, lblItem.Text)
                        End If
                    Next

                    showPromoItemlst()

                Case Else

            End Select
        End Using
    End Sub

    Private Sub dialogResult_no(ByVal dialogCmd As String)
        Select Case dialogCmd
            Case "DELETE_CUSTITEM", "DELETE_PRICEINQUIRY"

                setJavaScript(CustItem_Tab.CUSTITEM)
            Case "DELETE_PROMOTIONITEM"
                setJavaScript(CustItem_Tab.PROMOITEM)

            Case Else
        End Select
    End Sub

    Private Sub setJavaScript(ByVal tab As CustItem_Tab)
        JavaScript = "$('#tabs').tabs('select', " & convertToLocalTab(tab) & ");" 'selectSubmenu(" & tab & ");"
        'JavaScript &= "scrollTo(0, 0);" 'TODO: Doesn't work
    End Sub


    'Private Function convertToLocalTab(ByVal tab As CustItem_Tab) As Integer
    '    Select Case tab
    '        Case CustItem_Tab.CUST
    '            Return 0
    '        Case CustItem_Tab.SCHEDULE
    '            Return 1
    '        Case CustItem_Tab.CUSTITEM
    '            Return 2
    '    End Select
    '    Return 0
    'End Function

    Protected Sub btnUpdateSortOrder_Click(sender As Object, e As EventArgs) Handles btnUpdateSortOrder.Click
        Dim id_item As String() = Request.Form.GetValues("item_id")
        Dim sortNumber As Integer = 1
        Using db As New DBHelper
            If id_item IsNot Nothing Then
                DeleteCustItemDispOrder(db, lstCust_custItem.Text)
                For Each i In id_item
                    InsertCustItemDispOrder(db, lstCust_custItem.Text, i, sortNumber)
                    sortNumber += 1
                Next
            End If
        End Using
        showCustItemList()
    End Sub

    Protected Sub lnkCopy_Click(sender As Object, e As EventArgs) Handles lnkCopyPrice.Click, lnkCopy.Click, btnCopy.Click, btnCopyprice.Click
        Dim IsSelect As Boolean = False
        For Each row As GridViewRow In grdCustItem.Rows
            Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
            If CheckBoxchckrw.Checked Then
                IsSelect = True

            End If

        Next
        If Not IsSelect Then
            MessageBox("Please select at least one item.", "Error")
            Exit Sub
        End If
        If sender.ID = "lnkCopy" Or sender.id = "btnCopy" Then
            lblPopTitleUser.Text = "Copy all info to customer"
        Else
            lblPopTitleUser.Text = "Copy price info to customer"
        End If


        pnlPopWarningUser.Visible = False
        lblPopWarningUser.Text = ""
        ClearSubControl(pnlPopDtlEdit_User)
        txtPopJFCAccount.Text = Left(lstCustCompany.Items(1).Text, 2)
        popDialog_User.Show()
    End Sub

    'Protected Sub lnkCopyPrice_Click(sender As Object, e As EventArgs) Handles lnkCopyPrice.Click
    '    pnlPopWarningUser.Visible = False
    '    lblPopWarningUser.Text = ""
    '    ClearSubControl(pnlPopDtlEdit_User)
    '    lblPopTitleUser.Text = "Copy price info to customer"
    '    popDialog_User.Show()
    'End Sub

    Protected Sub btnPopSaveUser_click(sender As Object, e As EventArgs) Handles btnPopSaveUser.Click
        Using db As New DBHelper
            Dim customerID As String = GetCustomerID(db, txtPopJFCAccount.Text, txtPopJFCNum.Text)
            If customerID = "" Then
                pnlPopWarningUser.Visible = True
                lblPopWarningUser.Text = "Can not find this customer."
                popDialog_User.Show()
                Exit Sub
            End If
            Dim custInfo As CustomerInfo = New CustomerInfo(customerID)
            If custInfo.IsExcepPriceCustomer Then
                pnlPopWarningUser.Visible = True
                lblPopWarningUser.Text = "This customer uses an exception price group. <br />You cannot copy data to this customer."
                popDialog_User.Show()
                Exit Sub
            End If

            If lstCustCompany.Items.FindByValue(txtPopJFCAccount.Text & txtPopJFCNum.Text) Is Nothing Then
                pnlPopWarningUser.Visible = True
                lblPopWarningUser.Text = "You cannot copy data to this customer."
                popDialog_User.Show()
                Exit Sub
            End If

            If lstCustCompany.Text = txtPopJFCAccount.Text & txtPopJFCNum.Text Then
                pnlPopWarningUser.Visible = True
                lblPopWarningUser.Text = "You cannot copy data to the same customer."
                popDialog_User.Show()
                Exit Sub
            End If

            Dim IsSelect As Boolean = False
            'Dim nrCustGrpVendor = GetNrCustGrpVendor(db, txtPopJFCAccount.Text & txtPopJFCNum.Text) 'custGrpID
            'Dim isSmallCust = Common.IsSmallCustomer(db, customerID)
            'Dim UseCustomerCtg = Common.UseCustomerCtg(db, customerID)
            For Each row As GridViewRow In grdCustItem.Rows
                Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                Dim lblGroupItem = CType(row.FindControl("lblCustGrpItemID"), Label)
                Dim lblitemcode = CType(row.FindControl("lblItemIdN"), Label)
                Dim lblitemdesc = CType(row.FindControl("lblItemDescN"), Label)
                Dim lblitemremark = CType(row.FindControl("lblJfcRemark"), Label)

                Dim lblcsprice = CType(row.FindControl("lblCsPrice"), Label)
                Dim lbleaprice = CType(row.FindControl("lblEaPrice"), Label)
                Dim lbllbprice = CType(row.FindControl("lblLbPrice_orderByQty"), Label)
                Dim csPrice, eaprice, lbprice As Decimal
                Dim itemcode, itemdesc, remark As String
                Dim IsAll As Boolean = lblPopTitleUser.Text.Contains("all")
                itemcode = IIf(IsAll, lblitemcode.Text.Replace(")", "").Replace("(", ""), "")
                itemdesc = IIf(IsAll, lblitemdesc.Text, "")
                remark = IIf(IsAll, lblitemremark.Text.Replace("<br />", ""), "")
                csPrice = Decimal.Parse(lblcsprice.Text)
                eaprice = Decimal.Parse(lbleaprice.Text)
                lbprice = Decimal.Parse(lbllbprice.Text)

                If CheckBoxchckrw.Checked Then
                    IsSelect = True
                    CopyItemDel(lblGroupItem.Text, custInfo, itemcode, itemdesc, remark, csPrice, eaprice, lbprice, IsAll)
                End If

            Next
            If IsSelect Then

                MessageLine("CONFIRM", "Successfully copy item info to other customer!")
                showCustItemList()
            End If

        End Using
    End Sub

    Private Sub CopyItemDel(ByVal GroupItemID As String, ByVal custInfo As CustomerInfo, itemcode As String, itemdesc As String, itemremark As String, csPrice As Decimal,
                            eaprice As Decimal, lbprice As Decimal, ByVal copyall As Boolean)
        Dim SQL As String
        Dim r, r2 As DataReader
        'Dim custGrpID As String = lstCustGrp_custItem.Text


        Using DB As New DBHelper

            SQL = GetSQL_CustItem(DB, "Item_Copy", custInfo, GroupItemID)


            r = DB.GetReader(SQL)

            If r.Read() Then
                If copyall Then
                    UpSertCustItem(DB, custInfo.customerID, CString(r("custGrpItem_id")), "Y", itemcode, itemdesc, itemremark,
                              IIf(Val(r("csPrice_o")) = csPrice, Nothing, csPrice), IIf(Val(r("eaPrice_o")) = eaprice, Nothing, eaprice), 0,
                              IIf(Val(r("lbPrice_OrderByQty_o")) = lbprice, Nothing, lbprice), False)

                Else
                    UpSertCustItemCopyPrice(DB, custInfo.customerID, CString(r("custGrpItem_id")), "Y", CString(r("item_id")), CString(r("custItem_Desc")), CString(r("jfcRemark")),
                              IIf(Val(r("csPrice_o")) = csPrice, Nothing, csPrice), IIf(Val(r("eaPrice_o")) = eaprice, Nothing, eaprice), 0,
                              IIf(Val(r("lbPrice_OrderByQty_o")) = lbprice, Nothing, lbprice), False)

                End If


            Else
                SQL = GetSQL_CustItem(DB, "NewItem_Copy", custInfo, GroupItemID)
                r2 = DB.GetReader(SQL, custInfo.jfcBranchNr, custInfo.jfcCustomerNr, custInfo.customerID, GroupItemID.Substring(GroupItemID.Length - 5))
                If r2.Read() Then
                    UpSertCustItem(DB, custInfo.customerID, CString(r2("custGrpItem_id")), "Y", itemcode, itemdesc, itemremark, csPrice, eaprice, 0, lbprice, True)

                End If
                r2.close()
            End If

            r.close()
        End Using

    End Sub

#Region "Price Inquiry"
    'Price Inquiry 11/19/2020

    Private Sub showPirceInquiry(ByVal allcustomer As Boolean)
        'If ViewState("CustItem_Tab") = CustItem_Tab.CUSTITEM Then
        Dim SQL As String = ""
        'Dim custGrpID As String = lstCustGrp_custItem.Text
        'Dim customerID As String = lstCust_custItem.Text
        Dim TotalDt As New DataTable
        Using DB As New DBHelper
            'Init textchanged value
            'If lblTextChanged_custItem.Value <> "YES" Then lblTextChanged_custItem.Value = ViewState("TEXT_CHANGED_CUSTITEM")
            'If watItemSearch_custItem.WatermarkText = txtItemSearch_custItem.Text Then txtItemSearch_custItem.Text = String.Empty
            'Set properties

            'Get info
            'GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            'ViewState("jfcBranchNr") = jfcBranchNr
            'ViewState("jfcCustomerNr") = jfcCustomerNr
            'nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            'isSmallCust = Common.IsSmallCustomer(DB, customerID)
            'Set properties
            'If SMALL_CUST_ONLY = True Then

            'End If

            Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")


            Dim SQL_eaCost = "(CASE WHEN ci.eaPerCs <= 1 THEN 0 ELSE " & _
                                "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN ci.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/ci.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                                " ELSE (CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs, 2) END) END) " & _
                             "END)"
            Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"

            If allcustomer Then

                If rdbAllExpCust.Checked Then
                    SQL = "SELECT c.customer_id FROM webUser_customer u, customer c, webuser_priceInquiry p " &
                   " WHERE u.custcompany_id = c.custgrp_id and c.customer_id = p.customer_id and u.email =  " & DB.SqlEsc(GetUserEmail(DB)) & " " &
                   " AND p.sent = 'Y' AND p.responded is null and (substr(c.JFCPRICEGRPID_ORIG,-4) != '-REG' or c.JFCPRICEGRPID_ORIG is null) " &
                   " GROUP BY c.customer_id"   'and substr(c.JFCPRICEGRPID_ORIG,-4) != '-REG' 
                ElseIf rdbAllRegCust.Checked Then
                    SQL = "SELECT c.customer_id FROM webUser_customer u, customer c, webuser_priceInquiry p " &
                    " WHERE u.custcompany_id = c.custgrp_id and c.customer_id = p.customer_id and u.email =  " & DB.SqlEsc(GetUserEmail(DB)) & " " &
                    " AND p.sent = 'Y' AND p.responded is null and substr(c.JFCPRICEGRPID_ORIG,-4) = '-REG' " &
                    " GROUP BY c.customer_id"   'and substr(c.JFCPRICEGRPID_ORIG,-4)= '-REG' 
                End If

                Dim customerslist = DB.GetDataTable(SQL)
                For Each item As DataRow In customerslist.Rows
                    custInfo = New CustomerInfo(item(0))
                    SQL = "SELECT wp.customer_id,cust.customername, wp.custGrpItem_id, i.custgrp_id, v.custGrpVendor_id, v.vendorName, i.item_id, i.itemNr, NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc, i.custItemDesc, p.csAvail, p.eaAvail, " & _
                   " CASE WHEN i.itemNr IS NULL THEN i.item_id ELSE i.itemNr END AS itemIdNr_view,  TO_CHAR(wp.createdate, " & DB.SqlEsc(DATE_FMT) & ") AS createdate_FMT, " & _
                   "p.csCost, i.minSalCnt,p.lbsPerEa, ci.eaPerCs," & _
                     SQL_eaCost & " AS eaCost, " & _
                     SQL_lbCost & " AS lbCost, p.scaledItem,nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " & _
                      " NVL(p.btmcsPrice,0) as btmcsPrice, " & _
                  " NVL(jp.csprice,0) as regcsPrice, " & _
                  " NVL(jp.eaprice,0) as regeaPrice, " & _
                  " NVL(jp.lbPrice_orderByQty,0) as reglbPrice " & _
                   "FROM webuser_priceInquiry wp, custGrp_vendor v,customer cust, " & IIf(custInfo.isSmallCust, IIf(custInfo.IsExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
                   "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                   "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(custInfo.PriceGrpID) & " " & _
                    "LEFT OUTER JOIN jfcPrice jp ON i.jfcItem ='Y' AND jp.item_id = i.item_id AND jp.pricegrp_id = " & DB.SqlEsc(custInfo.PriceGrpID) & " " & _
                   "WHERE wp.custGrpItem_id = i.custGrpItem_id And i.custGrpItem_id = ci.custGrpItem_id AND cust.customer_id = ci.customer_id" & _
                   "  AND ci.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " &
                   "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id" & _
                   "  AND i.custgrp_id = " & DB.SqlEsc(custInfo.custGrpID) & _
                   "  AND wp.sent = 'Y' AND wp.responded is null AND wp.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " & _
                   " Group by wp.customer_id,cust.customername, wp.custGrpItem_id , i.custgrp_id, v.custGrpVendor_id, v.vendorName, i.item_id, i.itemNr, a.itemDesc, p.itemDesc_branch, i.custItemDesc, p.csAvail, p.eaAvail,v.vendorName,i.itemNr, wp.createdate,ci.eaPerCs,p.csCost,p.avgeawgt,p.avgcswgt,p.scaledItem,i.minSalCnt,p.lbsPerEa,p.btmcsPrice,jp.csprice, jp.eaprice,jp.lbPrice_orderByQty " & _
                   "order by CASE WHEN wp.createdate IS NULL THEN '1' ELSE '2' END, wp.createdate,  v.vendorName, itemIdNr_view"
                    Dim dt As DataTable = DB.GetDataTable(SQL)
                    If TotalDt Is Nothing Or TotalDt.Rows.Count = 0 Then
                        TotalDt = dt.Clone()
                    End If


                    If dt.Rows.Count > 0 Then
                        For Each row As DataRow In dt.Rows
                            TotalDt.Rows.Add(row.ItemArray)
                        Next

                    End If

                Next


            Else
                SQL = "SELECT wp.customer_id,cust.customername,  wp.custGrpItem_id,i.custgrp_id, v.custGrpVendor_id, v.vendorName, i.item_id, i.itemNr, NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc, i.custItemDesc, p.csAvail, p.eaAvail, " & _
                      " CASE WHEN i.itemNr IS NULL THEN i.item_id ELSE i.itemNr END AS itemIdNr_view,  TO_CHAR(wp.createdate, " & DB.SqlEsc(DATE_FMT) & ") AS createdate_FMT, " & _
                      "p.csCost, i.minSalCnt,p.lbsPerEa, ci.eaPerCs," & _
                        SQL_eaCost & " AS eaCost, " & _
                        SQL_lbCost & " AS lbCost, p.scaledItem,nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " & _
                         " NVL(p.btmcsPrice,0) as btmcsPrice, " & _
                     " NVL(jp.csprice,0) as regcsPrice, " & _
                     " NVL(jp.eaprice,0) as regeaPrice, " & _
                     " NVL(jp.lbPrice_orderByQty,0) as reglbPrice " & _
                      "FROM webuser_priceInquiry wp, custGrp_vendor v,customer cust, " & IIf(custInfo.isSmallCust, IIf(custInfo.IsExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
                      "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                      "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(custInfo.PriceGrpID) & " " & _
                       "LEFT OUTER JOIN jfcPrice jp ON i.jfcItem ='Y' AND jp.item_id = i.item_id AND jp.pricegrp_id = " & DB.SqlEsc(custInfo.PriceGrpID) & " " & _
                      "WHERE wp.custGrpItem_id = i.custGrpItem_id And i.custGrpItem_id = ci.custGrpItem_id AND cust.customer_id = ci.customer_id" & _
                      "  AND ci.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " &
                      "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id" & _
                      "  AND i.custgrp_id = " & DB.SqlEsc(custInfo.custGrpID) & _
                      "  AND wp.sent = 'Y' AND wp.responded is null AND wp.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " & _
                      " Group by  wp.customer_id,cust.customername,wp.custGrpItem_id,i.custgrp_id, v.custGrpVendor_id, v.vendorName, i.item_id, i.itemNr, a.itemDesc, p.itemDesc_branch, i.custItemDesc, p.csAvail, p.eaAvail,v.vendorName,i.itemNr, wp.createdate,ci.eaPerCs,p.csCost,p.avgeawgt,p.avgcswgt,p.scaledItem,i.minSalCnt,p.lbsPerEa,p.btmcsPrice,jp.csprice, jp.eaprice,jp.lbPrice_orderByQty " & _
                      "order by CASE WHEN wp.createdate IS NULL THEN '1' ELSE '2' END, wp.createdate,  v.vendorName, itemIdNr_view"
                TotalDt = DB.GetDataTable(SQL)
            End If


            'Show price inquiry list

            'dsPriceInquire.ProviderName = "System.Data.OracleClient"
            'dsPriceInquire.ConnectionString = DB.GetConnectionString
            'dsPriceInquire.SelectCommand = SQL
            'grdPriceInquire.DataSourceID = "dsPriceInquire"
            grdPriceInquire.DataSourceID = ""
            grdPriceInquire.DataSource = TotalDt 'DB.GetDataTable(SQL)
            grdPriceInquire.Columns(4).Visible = Not IsMobile()
            grdPriceInquire.Columns(5).Visible = Not IsMobile()
            grdPriceInquire.Columns(2).Visible = Not IsMobile()
            grdPriceInquire.Columns(8).Visible = Not custInfo.IsExcepPriceCustomer
            'If IsPostBack = False Then grdPriceInquire.Sort("CASE WHEN p.createdate IS NULL THEN '2' ELSE '1' END, p.createdate DESC ", SortDirection.Ascending)
            grdPriceInquire.DataBind()
            If grdPriceInquire.Rows.Count = 0 Then
                PanelPriceInquireList.Visible = False
            Else
                PanelPriceInquireList.Visible = True
            End If
            'grdPriceInquire.Columns(GetColumnIndex(grdPriceInquire, "ctgName_view")).Visible = Not IsMobile()
            'Init textchanged value
            'initTextChanged_custItem()
        End Using
        'End If
    End Sub

    Protected Sub grdPriceInquire_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdPriceInquire.PageIndexChanging
        grdPriceInquire.PageIndex = e.NewPageIndex
        grdPriceInquire.DataBind()
    End Sub
    Protected Sub grdPriceInquire_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPriceInquire.RowCommand
        Using DB As New DBHelper

            setJavaScript(CustItem_Tab.CUSTITEM)
            If e.CommandName = "Page" Then
                'Page is changed
                showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))
                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False
            ElseIf e.CommandName = "DELETE_ROW" Then

                deletePriceInquiryitem(Convert.ToString(e.CommandArgument))
            ElseIf e.CommandName = "EDIT_ROW" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()
                ViewState("PriceInquire") = True
                Dim str() As String = e.CommandArgument.ToString().Split(",")
                showCustItemDel(str(0), New CustomerInfo(str(1)))
                ckbPopEmail.Checked = True
            End If
        End Using
    End Sub

    Protected Sub grdPriceInquire_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdPriceInquire.RowDataBound
        'Dim lbl As Label
        Dim btn As ImageButton
        Dim lblcsCost As Label
        Dim lbleaCost As Label
        Dim lbllbCost As Label
        Dim lblscaledItem As Label
        Dim lstPriceUnit As DropDownList
        Dim lblbtmcsPrice As Label
        Dim btmeaPrice As Decimal
        Dim btmlbPrice As Decimal
        Dim lblRegcsPrice As Label
        Dim lblRegeaPrice As Label
        Dim lblReglbPrice As Label
        Dim lbllbsPerEa As Label
        Dim lblavgcswgt As Label
        Dim lblavgeawgt As Label
        Dim lblminSalCnt As Label
        Dim lbleapercs As Label
        Dim eaUnit As String

        Dim lblCost As Label
        Dim lblbtmPrice As Label
        Dim lblRegPrice As Label
        Dim testlabel As Label
        If e.Row.RowType = DataControlRowType.DataRow Then
            'lbl = e.Row.FindControl("lblenable")
            'If lbl.Text = "Y" Then
            '    e.Row.BackColor = Drawing.Color.LightYellow
            'End If
            btn = e.Row.FindControl("btnDelete")
            btn.Visible = False
            btn.ID = "btnDelete" & e.Row.RowIndex
            lstPriceUnit = e.Row.FindControl("lstPriceUnit")
            lblcsCost = e.Row.FindControl("lblcsCost")
            lbleaCost = e.Row.FindControl("lbleaCost")
            lbllbCost = e.Row.FindControl("lbllbCost")
            lblscaledItem = e.Row.FindControl("lblscaledItem")
            lblbtmcsPrice = e.Row.FindControl("lblbtmcsPrice")
            lblRegcsPrice = e.Row.FindControl("lblRegcsPrice")
            lblRegeaPrice = e.Row.FindControl("lblRegeaPrice")
            lblReglbPrice = e.Row.FindControl("lblReglbPrice")
            lblavgcswgt = e.Row.FindControl("lblavgcswgt")
            lblavgeawgt = e.Row.FindControl("lblavgeawgt")
            lblminSalCnt = e.Row.FindControl("lblminSalCnt")
            lbllbsPerEa = e.Row.FindControl("lbllbsperEa")
            lbleapercs = e.Row.FindControl("lbleapercs")

            lblCost = e.Row.FindControl("lblCost")
            lblbtmPrice = e.Row.FindControl("lblbtmPrice")
            lblRegPrice = e.Row.FindControl("lblRegPrice")

            eaUnit = IIf(lblscaledItem.Text = "Y", "pc", "ea")
            testlabel = e.Row.FindControl("lblCustItemID")
            lstPriceUnit.Items.Clear()
            If Val(lbllbCost.Text) > 0 AndAlso lblscaledItem.Text = "Y" Then lstPriceUnit.Items.Add(New ListItem(" / lb", "LB"))
            If Val(lblcsCost.Text) > 0 Then lstPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
            If Val(lbleaCost.Text) > 0 AndAlso lblscaledItem.Text <> "Y" Then lstPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))


            'csCost = Val(r("csCost"))
            'btmcsPirce = Val(r("btmcsPrice"))
            'RegcsPrice = Val(r("RegcsPrice"))
            Dim minSalCnt = IIf(Val(lblminSalCnt.Text) = 0, 1, Val(lblminSalCnt.Text))
            Dim scaledItem As Boolean = IIf(lblscaledItem.Text = "Y", True, False)

            'Dim lbsPerEa = GetJfcLbsPerEa(DB, lblCustomerID.Text, GroupItemID)
            'Common.CovertPrice(DB, csCost, eaCost, lbCost, r("eapercs"), minSalCnt, scaledItem, lbsPerEa, r("avgcswgt"), r("avgeawgt"))
            Common.CovertPrice(Val(lblbtmcsPrice.Text), btmeaPrice, btmlbPrice, Val(lbleapercs.Text), minSalCnt, scaledItem, Val(lbllbsPerEa.Text), CString(lblavgcswgt.Text), CString(lblavgeawgt.Text))

            'ViewState("ShipDate") = CString(r("shipDate_FMT2"))

            lblCost.Text = Displayformatgdv(Val(lbllbCost.Text), Val(lblcsCost.Text), Val(lbleaCost.Text), scaledItem)
            lblbtmPrice.Text = Displayformatgdv(btmlbPrice, Val(lblbtmcsPrice.Text), btmeaPrice, scaledItem)
            lblRegPrice.Text = Displayformatgdv(Val(lblReglbPrice.Text), Val(lblRegcsPrice.Text), Val(lblRegeaPrice.Text), scaledItem)
            Dim textBox As TextBox = TryCast(e.Row.FindControl("txtPopPrice"), TextBox)

            If textBox IsNot Nothing Then
                textBox.Attributes.Add("onkeydown", "TriggerTextBox(event, '" & textBox.ClientID & "')")
            End If

            'If ViewState("IsExcepPriceCustomer") Then btn.Visible = Falses
            'e.Row.CssClass = "selectRow"
            'e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            'e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
        End If
    End Sub

    Public Sub rdbAllRegCust_click(Sender As Object, ByVal e As System.EventArgs) Handles rdbAllRegCust.CheckedChanged, rdbCurrCust.CheckedChanged, rdbAllExpCust.CheckedChanged
        If rdbAllRegCust.Checked Then
            ViewState("SHOW_ALL_INQUIRE") = True
            btnAddPriceInquire.Enabled = True
            lblPriceInquireMsg.Visible = True
            btnAddPriceInquire.CssClass = "midButton"

            'grdPriceInquire.Columns(8).Visible = True
        ElseIf rdbCurrCust.Checked Then
            ViewState("SHOW_ALL_INQUIRE") = False
            btnAddPriceInquire.Enabled = True
            btnAddPriceInquire.CssClass = "midButton"
            lblPriceInquireMsg.Visible = True
            ImgbtnPriceInquiryCopy.Visible = True
            lnkPriceInquiryCopy.Visible = True

            grdPriceInquire.Columns(8).Visible = True
        ElseIf rdbAllExpCust.Checked Then
            ViewState("SHOW_ALL_INQUIRE") = True
            btnAddPriceInquire.Enabled = False
            btnAddPriceInquire.CssClass = "midButton_gray"
            lblPriceInquireMsg.Visible = False
            ImgbtnPriceInquiryCopy.Visible = False
            lnkPriceInquiryCopy.Visible = False

            grdPriceInquire.Columns(8).Visible = False

        End If
        showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))
    End Sub



    Public Function addPriceInquireItem(CustGrpItemID As String, ItemID As String, PriceUnit As String, Price As String, custInfo As CustomerInfo) As Boolean
        Using DB As New DBHelper

            'Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
            'setJavaScript(CustItem_Tab.CUSTITEM)
            Dim IsNew = True
            Dim sql = SetPrice(PriceUnit, Val(Price), ItemID, custInfo.customerID)
            Dim r = DB.GetReader(sql)
            If r.Read() Then
                'Mod by jzhou on 1/11/2021
                'If ViewState("btmcsPirce") > Val(r("csprice")) Then
                If Val(r("btmcsPrice")) > Val(r("csprice")) Then
                    MessageBox("Item# " & ItemID & ", please enter a price larger than bottom price", "Error!")

                    Return False
                End If


                UpSertCustItem(DB, custInfo.customerID, CustGrpItemID, "Y", "", "", "", Val(r("csprice")), Val(r("eaprice")), 0, Val(r("lbPrice_OrderByQty")), IsNew)
            End If

            Dim emailresponse As String = ""


            UpdatePriceInquiryResponse(DB, CustGrpItemID, custInfo.customerID)
            Return True

        End Using

    End Function

    Protected Sub btnAddPriceInquire_Click(Sender As Object, ByVal e As System.EventArgs) Handles btnAddPriceInquire.Click
        Dim custInfo As CustomerInfo = ViewState("CUSTOMER_INFO")
        Dim GroupItemlist As New List(Of String)

        Dim GroupEmailList As Hashtable = New Hashtable()
        Dim sendemail As Boolean = False
        Dim customerID As String = ""
        If custInfo.IsExcepPriceCustomer And Not ViewState("SHOW_ALL_INQUIRE") Then
            MessageBox("This customer uses an exception price group, you cannot add items", "Error")
            Exit Sub
        End If
        Dim curInfo As CustomerInfo = New CustomerInfo()
        For Each row As GridViewRow In grdPriceInquire.Rows
            Dim txtPrice = CType(row.FindControl("txtPopPrice"), TextBox)
            Dim lstPriceUnit = CType(row.FindControl("lstPriceUnit"), DropDownList)
            Dim lblItemID = CType(row.FindControl("lblItemID"), LinkButton)
            Dim lblCustGrpItemID = CType(row.FindControl("lblCustGrpItemID"), Label)
            Dim lblcustomerID = CType(row.FindControl("lblcustomerID"), Label)

            If customerID <> lblcustomerID.Text Then
                curInfo = New CustomerInfo(lblcustomerID.Text)


                GroupItemlist.Clear()
                GroupEmailList.Add(curInfo, GroupItemlist)
                'GroupEmailList.Add(lblcustomerID.Text, "1")
                'GroupEmailList.Add(lblcustomerID.Text & "2", "2")


                'hashtab.Add("05", 100)

                customerID = lblcustomerID.Text
            End If

            If Val(txtPrice.Text) > 0 And Not curInfo.IsExcepPriceCustomer Then

                If Not addPriceInquireItem(lblCustGrpItemID.Text, lblItemID.Text, lstPriceUnit.Text, txtPrice.Text, curInfo) Then
                    Exit Sub
                End If
                sendemail = True
                DirectCast(GroupEmailList(curInfo), List(Of String)).Add(lblCustGrpItemID.Text)
                'GroupItemlist.Add(lblCustGrpItemID.Text)

            End If
        Next
        Dim checkallnumber As Boolean = False
        '12/2/2021
        For Each custID In GroupEmailList.Keys
            Dim thecustInfo As CustomerInfo = CType(custID, CustomerInfo)
            If GroupEmailList(thecustInfo) IsNot Nothing Then
                If CType(GroupEmailList(thecustInfo), List(Of String)).Count > 0 Then
                    Using db As New DBHelper
                        Dim b As String = ""
                        Dim c As String = ""



                       

                        'If ckbPopEmail.Checked Then
                        Dim emailBody As String = ""
                        Dim Sql = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr, u.language ," &
                            "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
                            "FROM webuser u, webuser_customer uc, customer c " &
                           "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
                            "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
                            "AND c.customer_id = '" & thecustInfo.customerID & "' order by c.customer_id"
                        Dim dt = db.GetDataTable(Sql)
                        Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
                        Dim r2 As DataReader

                        r2 = db.GetReader(sql2, thecustInfo.jfcBranchNr)
                        Dim VendorEmail As String = ""
                        If r2.Read() Then
                            VendorEmail = CString(r2("email"))
                            'jfcBranchNr = CString(r("jfcBranchNr"))
                        End If
                        r2.close()
                        'CreateEmailBody(Page, emailBody, "PRICE_INQUIRY_LIST", custInfo, GroupItemlist, emailresponse, "")
                        Dim errmsg As String = ""
                        'Common.SendMail(SYSTEM_EMAIL, "yfu@kmsnet.com", VendorEmail, subject, emailBody, Nothing, True, errmsg)

                        Dim groupList = GroupEmailList(thecustInfo)
                        For Each dr In dt.Rows
                            Dim ci = New CultureInfo(IIf(CString(dr("language")) <> "", CString(dr("language")), "en-US").ToString)
                            Thread.CurrentThread.CurrentCulture = ci
                            Thread.CurrentThread.CurrentUICulture = ci
                            '2023
                            Dim emailresponse As String = rm.GetString("Pleaseplaceaneworderifyouneedtheseitems", ci) '"Please place a new order if you need these items."
                            CreateEmailBody(Page, emailBody, "PRICE_INQUIRY_LIST", thecustInfo, groupList, emailresponse, "")
                            Dim subject = rm.GetString("Regardingyourpriceinquiry", ci) '"Regarding your price inquiry"

                            'Send confirmation email to user
                            If Common.SendMail(SYSTEM_EMAIL, IIf(IsProduction() Or IsProdBeta(), CString(dr("customerEmail")), "yfu@kmsnet.com"), VendorEmail, subject, emailBody, Nothing, True, errmsg) > 0 Then

                            End If
                        Next
                    End Using
                    MessageLine("CONFIRM", "Successfully saved and sent email!")
                    checkallnumber = True

                End If

            End If
        Next
        If Not checkallnumber Then
            MessageBox("Please enter a price", "Error")
        End If

        showPirceInquiry(ViewState("SHOW_ALL_INQUIRE"))
        showCustItemList()
    End Sub

    Protected Sub lnkPriceInquirydelete_Click(Sender As Object, ByVal e As System.EventArgs) Handles lnkPriceInquirydelete.Click, ImgbtnPriceInquirydelete.Click

        Dim checkselected As Boolean = False
        setJavaScript(CustItem_Tab.CUSTITEM)

        Using DB As New DBHelper

            For Each row As GridViewRow In grdPriceInquire.Rows
                Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                Dim lblItem = CType(row.FindControl("lblCustGrpItemID"), Label)
                If CheckBoxchckrw.Checked Then
                    checkselected = True

                End If
            Next

            If Not checkselected Then
                MessageBox("Please selected at least one price inquire item.", "Error", DB)
            End If
            ViewState("DIALOG_CMD") = "DELETE_PRICEINQUIRY"

            Dim msg = "Are you sure you want to delete item(s) from the price inquiry?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
        ''UpdatePriceInquiryResponse(DB, lblItem.Text, lstCust_custItem.Text, "D")
    End Sub

    Protected Sub lnkPriceInquiryCopy_Click(Sender As Object, ByVal e As System.EventArgs) Handles lnkPriceInquiryCopy.Click, ImgbtnPriceInquiryCopy.Click

        Dim checkselected As Boolean = False
        setJavaScript(CustItem_Tab.CUSTITEM)

        Using DB As New DBHelper



            For Each row As GridViewRow In grdPriceInquire.Rows
                Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                Dim lstPriceUnit = CType(row.FindControl("lstPriceUnit"), DropDownList)
                Dim txtPrice = CType(row.FindControl("txtPopPrice"), TextBox)
                Dim lblRegCsPrice = CType(row.FindControl("lblRegcsPrice"), Label)
                Dim lblRegeaPrice = CType(row.FindControl("lblRegeaPrice"), Label)
                Dim lblReglbPrice = CType(row.FindControl("lblReglbPrice"), Label)
                'Dim lblItem = CType(row.FindControl("lblCustGrpItemID"), Label)
                If CheckBoxchckrw.Checked Then
                    checkselected = True
                    If Val(lblRegCsPrice.Text) > 0 Then
                        lstPriceUnit.SelectedValue = "CS"
                        txtPrice.Text = String.Format("{0:C}", Val(lblRegCsPrice.Text))
                    ElseIf Val(lblReglbPrice.Text) > 0 Then
                        lstPriceUnit.SelectedValue = "LB"
                        txtPrice.Text = String.Format("{0:C}", Val(lblReglbPrice.Text))
                    ElseIf Val(lblRegeaPrice.Text) > 0 Then
                        lstPriceUnit.SelectedValue = "EA"
                        txtPrice.Text = String.Format("{0:C}", Val(lblRegeaPrice.Text))
                    End If
                    'UpdatePriceInquiryResponse(DB, lblItem.Text, lstCust_custItem.Text, "D")
                End If
            Next

            If Not checkselected Then
                MessageBox("Please selected at least one price inquire item.", "Error", DB)
            End If

            'ViewState("DIALOG_CMD") = "DELETE_PRICEINQUIRY"

            'Dim msg = "Are you sure you want to delete item(s) from the price inquiry?"
            'MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub

    Private Sub deletePriceInquiryitem(ByVal custGrpItemID As String)
        Dim msg As String

        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.CUSTITEM)
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_PRICEINQUIRY"
            ViewState("DIALOG_ARG1") = custGrpItemID
            'ViewState("PopEditCustomerGrpId") = custGrpItemID
            msg = "Are you sure you want to remove this item " & GetItemID(DB, custGrpItemID) & " from price inquiry?"
            '" & lstCust_custItem.SelectedItem.Text & "?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub

#End Region

#Region "Promotional Items"

    Private Sub showPromoItemlst()
        'If ViewState("CUSTSETTING_TAB") = CustSetting_Tab.CUSTITEM Then


        Dim SQL As String
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim priceGrpID As String
        Dim custGrpID As String = lstCustGrp_custItem.Text
        Dim customerID As String = lstCust_custItem.Text


        Using DB As New DBHelper
            'Dim nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            'Dim isSmallCust = Common.IsSmallCustomer(DB, customerID)
            'Dim useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            priceGrpID = Common.GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)
            'Show price inquiry list
            SQL = "select o.item_id, o.itemdesc, DECODE(o.CUSTOMER_ID, Null,'Branch','Customer') as Setting from (" &
                  " Select j.item_id,  NVL(p.itemDesc_branch, d.itemdesc) AS itemDesc, c.customer_ID " &
                  " from Jfcpromoitem j " &
                  " Left join Customer_Promoitem c on j.item_id = c.item_id and c.CUSTOMER_ID = " & DB.SqlEsc(customerID) &
                  " LEFT OUTER JOIN jets_price p ON /*i.jfcItem = 'Y' AND*/ p.itemcode = j.item_id AND p.pricegpid = " & DB.SqlEsc(priceGrpID) & " " &
                  " inner join jfcitem d on d.item_id = j.item_id where j.jfcbranchnr = " & DB.SqlEsc(jfcBranchNr) & " and (c.enabled = 'Y' or c.enabled is null) " &
                  "Union " &
                  " select p.item_id, NVL(p.itemDesc_branch, i.itemdesc) AS itemDesc, p.CUSTOMER_ID " &
                  " from Customer_Promoitem p, jfcitem i " &
                  " LEFT OUTER JOIN jets_price p ON /*i.jfcItem = 'Y' AND*/ p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(priceGrpID) & " " &
                  " where i.item_id = p.item_id and p.enabled !='N' and p.CUSTOMER_ID = " & DB.SqlEsc(customerID) &
                ") o  group by o.item_id, o.itemdesc,o.customer_id "

            dsPromoItem.ProviderName = "System.Data.OracleClient"
            dsPromoItem.ConnectionString = DB.GetConnectionString
            dsPromoItem.SelectCommand = SQL
            grdPromoItemlst.DataSourceID = "dsPromoItem"

            'If IsPostBack = False Then grdPriceInquire.Sort("CASE WHEN p.createdate IS NULL THEN '2' ELSE '1' END, p.createdate DESC ", SortDirection.Ascending)
            grdPromoItemlst.DataBind()

        End Using
        'End If
    End Sub

    Private Sub showPromoItemDtl(ByVal ItemID As String)
        setJavaScript(CustItem_Tab.PROMOITEM)
        txtPromoItems.Text = ""
        lblPromotionwarning.Text = ""
        pnlDelPromotionwarning.Visible = False
        If String.IsNullOrWhiteSpace(ItemID) Then
            btnPopDeletePromotion.Visible = False
            btnPopSavePromotion.Text = "Add"
            lblPopTitle.Text = "Add"
            lblPreItemId.Text = ""
            'txtPromoItems.Focus()
        Else
            txtPromoItems.Text = ItemID
            lblPreItemId.Text = ItemID
            lblPopTitle.Text = "Edit"
            btnPopSavePromotion.Visible = False
            btnPopDeletePromotion.Visible = True
        End If
        'AjaxControlToolkit.Utility.SetFocusOnLoad(txtPromoItems)
        popDialog_Promotion.Show()

        'Common.SetFocus(Me.Page, txtPromoItems)

    End Sub

    Protected Sub lnkDeletePromoItem_Click(Sender As Object, ByVal e As System.EventArgs) Handles lnkDeletePromoItems.Click
        Dim checkselected As Boolean = False
        setJavaScript(CustItem_Tab.PROMOITEM)

        Using DB As New DBHelper
            For Each row As GridViewRow In grdPromoItemlst.Rows
                Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                Dim lblItem = CType(row.FindControl("lblItemId"), Label)
                If CheckBoxchckrw.Checked Then
                    checkselected = True
                    'DeleteCustPromoItem(DB, lblCustomerID.Text, lblItem.Text)
                End If
            Next
            If Not checkselected Then
                MessageBox("Please selected at least one promo item.", "Error", DB)
            End If

            '        'Confirm
            ViewState("DIALOG_CMD") = "DELETE_PROMOTIONITEM"

            Dim msg = "Are you sure you want to remove this Promotional Item?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub

    Protected Sub addPromoItem_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPromoAdd.Click
        showPromoItemDtl("")
    End Sub

    Protected Sub btnPopSavePromoItem_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopSavePromotion.Click
        setJavaScript(CustItem_Tab.PROMOITEM)

        Using DB As New DBHelper

            Try
                UpSertCustPromoItem(DB, lblCustomerID.Text, "", "Y", txtPromoItems.Text)
            Catch ex As Exception
                pnlDelPromotionwarning.Visible = True
                lblPromotionwarning.Text = ex.Message
                popDialog_Promotion.Show()
                Exit Sub

            End Try

        End Using
        showPromoItemlst()
    End Sub

    'Protected Sub btnPopDeletePromotion_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopDeletePromotion.Click
    '    'deleteOrderReminderRow()

    '    deletePromotionRow(lblPreItemId.Text)
    'End Sub
    Protected Sub btnPopClosePromotion_Click(sender As Object, ByVal e As System.EventArgs) Handles popClosePromotion.Click
        setJavaScript(CustItem_Tab.PROMOITEM)
    End Sub

    Private Sub deletePromotionRow(ByVal ItemId As String)
        Dim msg As String

        Using DB As New DBHelper
            setJavaScript(CustItem_Tab.PROMOITEM)
            '        'Confirm
            ViewState("DIALOG_CMD") = "DELETE_PROMOTIONITEM"
            ViewState("DIALOG_ARG1") = ItemId

            msg = "Are you sure you want to remove this Promotional Item?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub

    Protected Sub grdPromotionalItemlist_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPromoItemlst.RowCommand
        Using DB As New DBHelper

            setJavaScript(CustItem_Tab.PROMOITEM)
            If e.CommandName = "Page" Then
                'Page is changed

                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False

            ElseIf e.CommandName = "DELETE_ROW" Then
                deletePromotionRow(e.CommandArgument.ToString())

            ElseIf e.CommandName = "DETAIL" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()

                showPromoItemDtl(e.CommandArgument.ToString())

            End If
        End Using

    End Sub

#End Region

    'Used for sending grdview through email in ctrOrderDtl
    'As calling GridView.RenderControl(htmlTextWriter), the page raises an exception that a Server-Control was rendered outside of a Form. Avoid this execption by overriding VerifyRenderingInServerForm

    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub




End Class

