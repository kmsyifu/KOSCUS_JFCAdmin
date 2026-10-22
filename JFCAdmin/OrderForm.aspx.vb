Imports System.Data
Imports Common
Imports DBUpdate

Partial Class JFCAdmin_OrderForm
    Inherits System.Web.UI.Page

    Protected JavaScript As String = ""

    ' ════════════════════════════════════════════════════════════════
    '  PAGE LIFECYCLE
    ' ════════════════════════════════════════════════════════════════

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Dim p As Page = TryCast(Me.Context.Handler, Page)
        If p IsNot Nothing Then
            p.MasterPageFile = IIf(IsMobile(), "JFCAdmin_Mobile.master", "JFCAdmin.master")
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Request("DialogResult") = "YES" Then
            dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"))
        ElseIf Request("DialogResult") = "NO" Then
            dialogResult_no(ViewState("DIALOG_CMD"))
        End If

        If IsPostBack = False Then
            DataBind()

            Using DB As New DBHelper
                Dim email As String = GetUserEmail(DB)
                ' Reuse same helper as CustItem — fills branch dropdown
                If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                    ShowBranchList(DB, email, True, lstBranch)
                Else
                    ShowBranchList(DB, email, False, lstBranch)
                End If

                If Request.QueryString("BRANCH") <> "" Then
                    lstBranch.Text = Request.QueryString("BRANCH")
                End If

                If lstBranch.Text = "" Then
                    divMain.Visible = False
                    lblTitle.Visible = True
                Else
                    lblTitle.Visible = False
                    lblBranchID.Text = Left(lstBranch.Text, 2)  ' jfcBranchNr
                End If
            End Using

            If lstBranch.Text <> "" Then
                ShowListPanel()
            End If
        End If
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  BRANCH DROPDOWN
    ' ════════════════════════════════════════════════════════════════

    Protected Sub lstBranch_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBranch.SelectedIndexChanged
        lblBranchID.Text = Left(lstBranch.Text, 2)
        divMain.Visible = True
        lblTitle.Visible = False
        txtSearchOrderForm.Text = ""
        txtSearchCustNr.Text = ""
        ShowListPanel()
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        lblBranchID.Text = Left(lstBranch.Text, 2)
        ShowListPanel()
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  LIST PANEL
    ' ════════════════════════════════════════════════════════════════

    Private Sub ShowListPanel()
        pnlList.Visible = True
        pnlDetail.Visible = False
        lblListMsg.Text = ""
        BindOrderFormGrid()
    End Sub

    Private Sub BindOrderFormGrid()
        Using DB As New DBHelper
            Dim jfcBranchNr As String = lblBranchID.Text
            Dim searchForm As String = IIf(watSearchOrderForm.WatermarkText = txtSearchOrderForm.Text, "", txtSearchOrderForm.Text.Trim())
            Dim searchCust As String = txtSearchCustNr.Text.Trim()

            Dim SQL As String =
                "SELECT DISTINCT f.orderForm_id, f.orderFormName  " &
                "FROM   orderForm f " &
                "LEFT JOIN orderForm_Customer fc ON fc.orderForm_id = f.orderForm_id " &
                "WHERE  f.jfcBranchNr = :1 " &
                IIf(searchForm <> "", " AND UPPER(f.orderFormName) LIKE " & DB.SqlEsc("%" & searchForm.ToUpper & "%"), "") &
                IIf(searchCust <> "", " AND fc.customer_id LIKE " & DB.SqlEsc("%" & searchCust & "%"), "") &
                " ORDER BY  f.orderFormName"

            grdOrderForms.DataSource = DB.GetDataTable(SQL, jfcBranchNr)
            grdOrderForms.DataBind()
        End Using
    End Sub

    Protected Sub grdOrderForms_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdOrderForms.RowCommand
        Select Case e.CommandName
            Case "OPEN_FORM"
                OpenDetailPanel(e.CommandArgument.ToString())
            Case "DELETE_FORM"
                ViewState("DIALOG_CMD") = "DELETE_ORDERFORM"
                ViewState("DIALOG_ARG1") = e.CommandArgument.ToString()
                Using DB As New DBHelper
                    MessageBoxYesNo("Are you sure you want to delete this order form and all its items?", "Confirm Deletion", DB)
                End Using
        End Select
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  ADD BUTTONS
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnAddExcep_Click(sender As Object, e As EventArgs) Handles btnAddExcep.Click
        OpenDetailPanel("", "E")
    End Sub

    Protected Sub btnAddReg_Click(sender As Object, e As EventArgs) Handles btnAddReg.Click
        OpenDetailPanel("", "R")
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  DETAIL PANEL — open
    ' ════════════════════════════════════════════════════════════════

    Private Sub OpenDetailPanel(orderFormID As String, Optional formType As String = "")
        pnlList.Visible = False
        pnlDetail.Visible = True
        lblDetailMsg.Text = ""
        lblItemMsg.Text = ""
        txtItem.Text = ""
        hfOrderFormID.Value = orderFormID
        Dim isNew As Boolean = (orderFormID = "")
        pnlBranch.Visible = False
        Using DB As New DBHelper
            ' Load formType from DB when editing
            If formType = "" AndAlso Not isNew Then
                formType = CString(DB.GetScalar(
                    "SELECT JFCPRICEGRPID_ORIG FROM orderForm WHERE orderForm_id = :1", orderFormID))
            End If
            hfFormType.Value = formType
            ViewState("FormType") = formType

            ' Titles / button text
            If isNew Then
                lblDetailTitle.Text = IIf(formType = "E",
                    "Add Order Form for Exception Price Group Customers",
                    "Add Order Form for Regular Customers")
                btnSaveForm.Text = "Add Form"
                btnDeleteForm.Visible = False
                txtFormName.Text = ""
                lblCreatedOn.Text = ""
                lblLastUpdated.Text = ""
            Else
                lblDetailTitle.Text = IIf(formType = "E",
                    "Update Order Form for Exception Price Group Customers",
                    "Update Order Form for Regular Customers")
                btnSaveForm.Text = "Update Form"
                btnDeleteForm.Visible = True

                Dim r As DataReader = DB.GetReader(
                    "SELECT orderFormName, excepPriceGrpId, " &
                    "TO_CHAR(createDate, 'MM/DD/YYYY HH:MI AM') || ' PDT by ' || createUser AS createDateFmt, " &
                    "TO_CHAR(modDate,    'MM/DD/YYYY HH:MI AM') || ' PDT by ' || modUser   AS modDateFmt " &
                    "FROM branch_orderForm WHERE orderForm_id = :1", orderFormID)
                If r.Read() Then
                    txtFormName.Text = CString(r("orderFormName"))
                    lblCreatedOn.Text = CString(r("createDateFmt"))
                    lblLastUpdated.Text = CString(r("modDateFmt"))
                    ViewState("ExcepPriceGrpID") = CString(r("excepPriceGrpId"))
                End If
                r.close()
            End If

            ' Exception Price Group DDL
            pnlExcepGrpRow.Visible = (formType = "E")
            If formType = "E" Then
                LoadExcepPriceGrpDDL(DB, lblBranchID.Text)
                If Not isNew Then
                    Dim savedGrp As String = CString(ViewState("ExcepPriceGrpID"))
                    If lstExcepPriceGrp.Items.FindByValue(savedGrp) IsNot Nothing Then
                        lstExcepPriceGrp.SelectedValue = savedGrp
                    End If
                End If
            End If

            ' Add Customer search panel — Regular type only
            pnlAddCustSearch.Visible = (formType = "R")
            txtAddCust.Text = ""
            lblAddCustMsg.Text = ""

            ' Sort by DDL init
            If lstSortBy.Items.Count = 0 Then
                lstSortBy.Items.Add(New ListItem("Custom", "custdisporder, itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Category", "ctgDispOrder, itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item Description", "itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item #", "item_id"))
            End If

            ' Category DDL
            If lstCtgSearch.Items.Count = 0 Then
                'Common.ShowList(DB, "CTG_BRANCH", lblBranchID.Text, lstCtgSearch, True, True, LST_ALL_STR)
            End If
        End Using

        BindCustomersGrid(orderFormID, formType)
        BindItemsGrid(orderFormID)
    End Sub

    Private Sub LoadExcepPriceGrpDDL(DB As DBHelper, jfcBranchNr As String)
        lstExcepPriceGrp.Items.Clear()
        Dim dt As DataTable = DB.GetDataTable(
            "SELECT DISTINCT JFCPRICEGRPID_ORIG AS id, JFCPRICEGRPID_ORIG AS name " &
            "FROM   customer " &
            "WHERE  jfcBranchNr = :1 " &
            "  AND  JFCPRICEGRPID_ORIG IS NOT NULL " &
            "  AND  SUBSTR(JFCPRICEGRPID_ORIG, -4) != '-REG' " &
            "ORDER  BY JFCPRICEGRPID_ORIG", jfcBranchNr)
        For Each dr As DataRow In dt.Rows
            lstExcepPriceGrp.Items.Add(New ListItem(CString(dr("name")), CString(dr("id"))))
        Next
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  CUSTOMERS GRID
    ' ════════════════════════════════════════════════════════════════

    Private Sub BindCustomersGrid(orderFormID As String, formType As String)
        Using DB As New DBHelper
            Dim SQL As String

            If formType = "E" Then
                ' Show ALL exception-price customers for this branch; pre-check assigned ones
                Dim excepGrpID As String = IIf(lstExcepPriceGrp.Items.Count > 0, lstExcepPriceGrp.SelectedValue, "")
                SQL = "SELECT c.customer_id, c.customerName, " &
                      "CASE WHEN fc.customer_id IS NOT NULL THEN 1 ELSE 0 END AS isAssigned " &
                      "FROM   customer c " &
                      "LEFT   JOIN orderForm_Customer fc " &
                      "         ON fc.customer_id = c.customer_id AND fc.orderForm_id = :1 " &
                      "WHERE  c.jfcBranchNr = :2 " &
                      "  AND  c.JFCPRICEGRPID_ORIG = :3 " &
                      "ORDER  BY c.customerName"
                grdCustomers.DataSource = DB.GetDataTable(SQL,
                    IIf(orderFormID = "", "~~NONE~~", orderFormID),
                    lblBranchID.Text,
                    excepGrpID)
            Else
                ' Regular: show only already-assigned customers with x remove button
                If orderFormID = "" Then
                    grdCustomers.DataSource = New DataTable()
                Else
                    SQL = "SELECT c.customer_id, c.customerName, 1 AS isAssigned " &
                          "FROM   customer c, orderForm_Customer fc " &
                          "WHERE  fc.customer_id = c.customer_id AND fc.orderForm_id = :1 " &
                          "ORDER  BY c.customerName"
                    grdCustomers.DataSource = DB.GetDataTable(SQL, orderFormID)
                End If
            End If

            grdCustomers.DataBind()

            ' Show x button only for Regular type
            For Each row As GridViewRow In grdCustomers.Rows
                Dim lnk = CType(row.FindControl("lnkRemoveCust"), LinkButton)
                If lnk IsNot Nothing Then lnk.Visible = (formType = "R")
            Next
        End Using
    End Sub

    Protected Sub btnAddCust_Click(sender As Object, e As EventArgs) Handles btnAddCust.Click
        ' Add a Regular customer to the form (only valid when form already exists)
        Dim orderFormID As String = hfOrderFormID.Value
        Dim custSearch As String = txtAddCust.Text.Trim()
        lblAddCustMsg.Text = ""

        If custSearch = "" Then
            lblAddCustMsg.Text = "Please enter a customer name or ID."
            Return
        End If
        If orderFormID = "" Then
            lblAddCustMsg.Text = "Please save the form first before adding customers."
            Return
        End If

        Using DB As New DBHelper
            Dim customerID As String = CString(DB.GetScalar(
                "SELECT customer_id FROM customer WHERE jfcBranchNr = :1 " &
                "AND (UPPER(customerName) LIKE " & DB.SqlEsc("%" & custSearch.ToUpper & "%") &
                "  OR customer_id LIKE " & DB.SqlEsc("%" & custSearch & "%") & ") " &
                "AND ROWNUM = 1", lblBranchID.Text))

            If customerID = "" Then
                lblAddCustMsg.Text = "Customer not found."
                Return
            End If

            Dim exists As Integer = Val(DB.GetScalar(
                "SELECT COUNT(*) FROM orderForm_Customer WHERE orderForm_id = :1 AND customer_id = :2",
                orderFormID, customerID))
            If exists = 0 Then
                DB.ExecuteNonQuery(
                    "INSERT INTO orderForm_Customer (orderForm_id, customer_id) VALUES (:1, :2)",
                    orderFormID, customerID)
            End If
        End Using
        txtAddCust.Text = ""
        BindCustomersGrid(orderFormID, hfFormType.Value)
    End Sub

    Protected Sub grdCustomers_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdCustomers.RowCommand
        If e.CommandName = "REMOVE_CUST" Then
            Dim orderFormID As String = hfOrderFormID.Value
            If orderFormID <> "" Then
                Using DB As New DBHelper
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderForm_Customer WHERE orderForm_id = :1 AND customer_id = :2",
                        orderFormID, e.CommandArgument.ToString())
                End Using
                BindCustomersGrid(orderFormID, hfFormType.Value)
            End If
        End If
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  ITEMS GRID
    ' ════════════════════════════════════════════════════════════════

    Private Sub BindItemsGrid(orderFormID As String)
        Using DB As New DBHelper
            Dim jfcBranchNr As String = lblBranchID.Text
            Dim searchText As String = IIf(watItemSearch.WatermarkText = txtItemSearch.Text, "", txtItemSearch.Text.Trim())
            Dim ctgFilter As String = lstCtgSearch.SelectedValue
            Dim sortBy As String = lstSortBy.SelectedValue
            'Dim priceGrpID As String = GetPriceGrpIDByBranch(DB, jfcBranchNr)

            'Dim SQL As String =
            '    "SELECT i.custGrpItem_id, i.item_id, i.itemNr, " &
            '    "NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc_view, " &
            '    "NVL(jctgGrp.ctgGrpDesc, 'Other') AS ctgName_view, " &
            '    "NVL(jctgGrp.dispOrder, 99) AS ctgDispOrder, " &
            '    "ci.eaPerCs, ci.csPrice, ci.eaPrice, ci.lbPrice_OrderByQty, " &
            '    "NVL(a.availMsg, '') AS availMsg, " &
            '    "CASE WHEN fi.custGrpItem_id IS NOT NULL THEN 1 ELSE 0 END AS isAssigned, " &
            '    "NVL(co.disporder, 0) AS custdisporder " &
            '    "FROM   custgrp_item i " &
            '    "JOIN   custGrp_itemGrp g ON g.custGrp_id = i.custGrp_id AND g.itemGrp_id = i.itemGrp_id " &
            '    "JOIN   customer_custGrpItem ci ON ci.custGrpItem_id = i.custGrpItem_id " &
            '    "LEFT   JOIN jfcItem a ON a.item_id = i.item_id " &
            '    "LEFT   JOIN jets_price p ON p.itemcode = i.item_id AND p.pricegpid = :2 " &
            '    "LEFT   JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " &
            '    "LEFT   JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " &
            '    "LEFT   JOIN branch_orderFormItems fi ON fi.custGrpItem_id = i.custGrpItem_id " &
            '    "           AND fi.orderForm_id = :3 " &
            '    "LEFT   JOIN customer_itemDispOrder co ON co.custGrpItem_id = i.custGrpItem_id " &
            '    "WHERE  i.custGrp_id = :1 " &
            '    "  AND  (p.enabled IS NULL OR p.enabled = 'Y') " &
            '    IIf(searchText <> "",
            '        " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE " & DB.SqlEsc("%" & searchText.ToUpper & "%") &
            '        "  OR  UPPER(i.item_id) = " & DB.SqlEsc(searchText.ToUpper) & ")", "") &
            '    IIf(ctgFilter <> "" AndAlso ctgFilter <> LST_ALL_STR,
            '        " AND NVL(jctgGrp.ctgGrpDesc, 'Other') = " & DB.SqlEsc(ctgFilter), "") &
            '    " ORDER BY " & sortBy

            'grdItems.DataSource = DB.GetDataTable(SQL,
            '    jfcBranchNr,
            '    "",
            '    IIf(orderFormID = "", "~~NONE~~", orderFormID))
            'grdItems.DataBind()
        End Using
    End Sub

    Protected Sub grdItems_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdItems.RowDataBound
        ' Could highlight assigned rows etc.
    End Sub

    Protected Sub grdItems_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdItems.RowCommand
        If e.CommandName = "REMOVE_ITEM" Then
            Dim orderFormID As String = hfOrderFormID.Value
            If orderFormID <> "" Then
                Using DB As New DBHelper
                    DB.ExecuteNonQuery(
                        "DELETE FROM branch_orderFormItems WHERE orderForm_id = :1 AND custGrpItem_id = :2",
                        orderFormID, e.CommandArgument.ToString())
                End Using
            End If
            BindItemsGrid(orderFormID)
        End If
    End Sub

    Protected Sub lstSortBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstSortBy.SelectedIndexChanged
        BindItemsGrid(hfOrderFormID.Value)
    End Sub

    Protected Sub lstCtgSearch_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstCtgSearch.SelectedIndexChanged
        BindItemsGrid(hfOrderFormID.Value)
    End Sub

    Protected Sub btnSearchItem_Click(sender As Object, e As EventArgs) Handles btnSearchItem.Click
        BindItemsGrid(hfOrderFormID.Value)
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  ADD ITEM (autocomplete box, same as CustItem)
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnGetItem_Click(sender As Object, e As EventArgs) Handles btnGetItem.Click
        If txtItem.Text.Contains(",") = False Then
            lblItemMsg.Text = "Please select an item from the auto-complete list."
            Return
        End If

        Dim custGrpItemID As String = GetLastWord(txtItem.Text, ",").PadLeft(CUSTGRP_ITEM_ID_LEN, "0")
        Dim orderFormID As String = hfOrderFormID.Value

        If orderFormID = "" Then
            lblItemMsg.Text = "Please save the form first before adding items."
            txtItem.Text = ""
            Return
        End If

        Using DB As New DBHelper
            Dim exists As Integer = Val(DB.GetScalar(
                "SELECT COUNT(*) FROM branch_orderFormItems WHERE orderForm_id = :1 AND custGrpItem_id = :2",
                orderFormID, custGrpItemID))
            If exists = 0 Then
                DB.ExecuteNonQuery(
                    "INSERT INTO branch_orderFormItems (orderForm_id, custGrpItem_id) VALUES (:1, :2)",
                    orderFormID, custGrpItemID)
                InsertEventLog(DB, "INSERT branch_orderFormItems", orderFormID, custGrpItemID, Nothing, True, False)
            End If
        End Using

        txtItem.Text = ""
        BindItemsGrid(orderFormID)
    End Sub

    Protected Sub btnClearItem_Click(sender As Object, e As EventArgs) Handles btnClearItem.Click
        txtItem.Text = ""
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  SAVE / DELETE FORM
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnSaveForm_Click(sender As Object, e As EventArgs) Handles btnSaveForm.Click
        Dim formName As String = txtFormName.Text.Trim()
        If formName = "" Then
            lblDetailMsg.Text = "Please enter an Order Form Name."
            Return
        End If

        Dim jfcBranchNr As String = lblBranchID.Text
        Dim formType As String = hfFormType.Value
        Dim orderFormID As String = hfOrderFormID.Value
        Dim isNew As Boolean = (orderFormID = "")

        Using DB As New DBHelper
            ' Duplicate check
            Dim dupSQL As String = IIf(isNew,
                "SELECT COUNT(*) FROM branch_orderForm WHERE jfcBranchNr = :1 AND orderFormName = :2",
                "SELECT COUNT(*) FROM branch_orderForm WHERE jfcBranchNr = :1 AND orderFormName = :2 AND orderForm_id <> :3")
            Dim dupCount As Integer
            If isNew Then
                dupCount = Val(DB.GetScalar(dupSQL, jfcBranchNr, formName))
            Else
                dupCount = Val(DB.GetScalar(dupSQL, jfcBranchNr, formName, orderFormID))
            End If
            If dupCount > 0 Then
                lblDetailMsg.Text = """" & formName & """ already exists."
                Return
            End If

            Dim userEmail As String = GetUserEmail(DB)
            Dim excepGrpID As String = IIf(formType = "E" AndAlso lstExcepPriceGrp.Items.Count > 0,
                lstExcepPriceGrp.SelectedValue, "")

            DB.BeginTransaction()

            If isNew Then
                orderFormID = DB.GetSequence("ORDERFORM_ID", CUSTGRP_ITEM_ID_LEN)
                DB.ExecuteNonQuery(
                    "INSERT INTO branch_orderForm " &
                    "  (orderForm_id, jfcBranchNr, orderFormName, formType, excepPriceGrpId, createUser, createDate, modUser, modDate) " &
                    "VALUES (:1, :2, :3, :4, :5, :6, SYSDATE, :6, SYSDATE)",
                    orderFormID, jfcBranchNr, formName, formType, excepGrpID, userEmail)
                hfOrderFormID.Value = orderFormID
            Else
                DB.ExecuteNonQuery(
                    "UPDATE branch_orderForm " &
                    "SET    orderFormName = :1, excepPriceGrpId = :2, modUser = :3, modDate = SYSDATE " &
                    "WHERE  orderForm_id  = :4",
                    formName, excepGrpID, userEmail, orderFormID)
            End If

            ' Save assigned customers from checkbox grid (Exception type)
            If formType = "E" Then
                DB.ExecuteNonQuery(
                    "DELETE FROM orderForm_Customer WHERE orderForm_id = :1", orderFormID)
                For Each row As GridViewRow In grdCustomers.Rows
                    Dim chk = CType(row.FindControl("chkCustRow"), CheckBox)
                    If chk IsNot Nothing AndAlso chk.Checked Then
                        Dim custID As String = CString(grdCustomers.DataKeys(row.RowIndex).Value)
                        DB.ExecuteNonQuery(
                            "INSERT INTO orderForm_Customer (orderForm_id, customer_id) VALUES (:1, :2)",
                            orderFormID, custID)
                    End If
                Next
            End If

            ' Save checked items
            DB.ExecuteNonQuery(
                "DELETE FROM branch_orderFormItems WHERE orderForm_id = :1", orderFormID)
            For Each row As GridViewRow In grdItems.Rows
                Dim chk = CType(row.FindControl("chkItemRow"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    Dim itemID As String = CString(grdItems.DataKeys(row.RowIndex).Value)
                    DB.ExecuteNonQuery(
                        "INSERT INTO branch_orderFormItems (orderForm_id, custGrpItem_id) VALUES (:1, :2)",
                        orderFormID, itemID)
                End If
            Next

            DB.CommitTransaction()
            InsertEventLog(DB, IIf(isNew, "INSERT", "UPDATE") & " branch_orderForm",
                orderFormID, formName, Nothing, True, False)
        End Using

        ' Reload detail with fresh timestamps
        OpenDetailPanel(orderFormID)
        lblDetailMsg.Text = IIf(isNew, "Order form added successfully.", "Order form updated successfully.")
        lblDetailMsg.ForeColor = Drawing.Color.Green
    End Sub

    Protected Sub btnDeleteForm_Click(sender As Object, e As EventArgs) Handles btnDeleteForm.Click
        ViewState("DIALOG_CMD") = "DELETE_ORDERFORM"
        ViewState("DIALOG_ARG1") = hfOrderFormID.Value
        Using DB As New DBHelper
            MessageBoxYesNo("Are you sure you want to delete this order form and all its items?", "Confirm Deletion", DB)
        End Using
    End Sub

    Protected Sub lnkBack_order_Click(sender As Object, e As EventArgs) Handles lnkBack_order.Click
        ShowListPanel()
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  DIALOG RESULTS
    ' ════════════════════════════════════════════════════════════════

    Private Sub dialogResult_yes(dialogCmd As String, dialogArg1 As String)
        Select Case dialogCmd
            Case "DELETE_ORDERFORM"
                Using DB As New DBHelper
                    DB.BeginTransaction()
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderFormItem WHERE orderForm_id = :1", dialogArg1)
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderForm_Customer WHERE orderForm_id = :1", dialogArg1)
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderForm WHERE orderForm_id = :1", dialogArg1)
                    DB.CommitTransaction()
                    InsertEventLog(DB, "DELETE branch_orderForm", dialogArg1, Nothing, Nothing, True, False)
                End Using
                ShowListPanel()
                MessageLine("CONFIRM", "Order form deleted.")
        End Select
    End Sub

    Private Sub dialogResult_no(dialogCmd As String)
        Select Case dialogCmd
            Case "DELETE_ORDERFORM"
                ' stay on current panel
        End Select
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  HELPERS
    ' ════════════════════════════════════════════════════════════════

    Private Function GetPriceGrpIDByBranch(DB As DBHelper, jfcBranchNr As String) As String
        Return CString(DB.GetScalar(
            "SELECT pricegpid FROM jfcVendor WHERE jfcBranchNr = :1", jfcBranchNr))
    End Function

    Overrides Sub VerifyRenderingInServerForm(control As Control)
    End Sub

End Class
