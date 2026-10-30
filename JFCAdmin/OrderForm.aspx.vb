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
        pnlBranch.Visible = True
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
                "SELECT DISTINCT f.orderForm_id, f.orderFormName, f.JFCPriceGrpID_Orig " &
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
    '  ADD BUTTON
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnAddForm_Click(sender As Object, e As EventArgs) Handles btnAddForm.Click
        OpenDetailPanel("")
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  DETAIL PANEL — open
    ' ════════════════════════════════════════════════════════════════

    ' Every order form belongs to one exception price group (orderForm.JFCPriceGrpID_Orig)
    Private Sub OpenDetailPanel(orderFormID As String)
        pnlList.Visible = False
        pnlDetail.Visible = True
        lblDetailMsg.Text = ""
        lblItemMsg.Text = ""
        ViewState("ItemChecks") = Nothing
        ViewState("ExcepPriceGrpID") = ""
        hfOrderFormID.Value = orderFormID
        Dim isNew As Boolean = (orderFormID = "")
        pnlBranch.Visible = False
        Using DB As New DBHelper
            ' Titles / button text
            If isNew Then
                lblDetailTitle.Text = "Add Order Form"
                btnSaveForm.Text = "Add Form"
                btnDeleteForm.Visible = False
                txtFormName.Text = ""
                lblCreatedOn.Text = ""
                lblLastUpdated.Text = ""
            Else
                lblDetailTitle.Text = "Update Order Form"
                btnSaveForm.Text = "Update Form"
                btnDeleteForm.Visible = True

                Dim r As DataReader = DB.GetReader(
                    "SELECT orderFormName, JFCPriceGrpID_Orig, " &
                    "TO_CHAR(createDate, 'MM/DD/YYYY HH:MI AM') || ' PDT by ' || createUser AS createDateFmt, " &
                    "TO_CHAR(modDate,    'MM/DD/YYYY HH:MI AM') || ' PDT by ' || modUser   AS modDateFmt " &
                    "FROM orderForm WHERE orderForm_id = :1", orderFormID)
                If r.Read() Then
                    txtFormName.Text = CString(r("orderFormName"))
                    lblCreatedOn.Text = CString(r("createDateFmt"))
                    lblLastUpdated.Text = CString(r("modDateFmt"))
                    ViewState("ExcepPriceGrpID") = CString(r("JFCPriceGrpID_Orig"))
                End If
                r.close()
            End If

            ' Exception Price Group DDL
            LoadExcepPriceGrpDDL(DB, lblBranchID.Text)
            Dim savedGrp As String = CString(ViewState("ExcepPriceGrpID"))
            If lstExcepPriceGrp.Items.FindByValue(savedGrp) IsNot Nothing Then
                lstExcepPriceGrp.SelectedValue = savedGrp
            End If

            ' Sort by DDL init
            If lstSortBy.Items.Count = 0 Then
                lstSortBy.Items.Add(New ListItem("Selected", "isAssigned DESC, itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Category", "ctgDispOrder, itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item Description", "itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item #", "item_id"))
            End If

            ' Category DDL
            If lstCtgSearch.Items.Count = 0 Then
                lstCtgSearch.Items.Add(New ListItem(LST_ALL_STR, LST_ALL_STR))
                Dim dtCtg As DataTable = DB.GetDataTable(
                    "SELECT ctgGrpDesc FROM jfcCtgGrp WHERE ctgGrpDesc IS NOT NULL " &
                    "GROUP BY ctgGrpDesc ORDER BY MIN(dispOrder), ctgGrpDesc")
                For Each dr As DataRow In dtCtg.Rows
                    lstCtgSearch.Items.Add(New ListItem(CString(dr("ctgGrpDesc")), CString(dr("ctgGrpDesc"))))
                Next
                lstCtgSearch.Items.Add(New ListItem("Other", "Other"))
            End If
        End Using

        BindCustomersGrid(orderFormID)
        BindItemsGrid(orderFormID, False)
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

    Private Sub BindCustomersGrid(orderFormID As String)
        Using DB As New DBHelper
            ' All customers of the selected exception price group; pre-check assigned ones
            Dim SQL As String =
                "SELECT c.customer_id, c.customerName, " &
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
                SelectedExcepGrpID())
            grdCustomers.DataBind()
        End Using
    End Sub

    Protected Sub lstExcepPriceGrp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstExcepPriceGrp.SelectedIndexChanged
        ' Customers and item prices both depend on the exception price group
        BindCustomersGrid(hfOrderFormID.Value)
        BindItemsGrid(hfOrderFormID.Value)
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  ITEMS GRID
    ' ════════════════════════════════════════════════════════════════

    ' captureChecks: remember the grid's current checkbox state before rebinding, so unsaved
    ' check/uncheck edits survive search, sort and add/remove postbacks until Save.
    Private Sub BindItemsGrid(orderFormID As String, Optional captureChecks As Boolean = True)
        If captureChecks Then CaptureItemChecks()
        Using DB As New DBHelper
            Dim jfcBranchNr As String = lblBranchID.Text
            Dim searchText As String = IIf(watItemSearch.WatermarkText = txtItemSearch.Text, "", txtItemSearch.Text.Trim())
            Dim ctgFilter As String = lstCtgSearch.SelectedValue
            Dim sortBy As String = lstSortBy.SelectedValue
            Dim priceGrpID As String = GetFormPriceGrpID(DB)

            ' Same list as CustItem GetSQL_CustItem "List" for an exception price customer
            ' (isExcepPriceCustomer = True, showOnlyRecentOrder4ExcepCust = False), but by branch price group
            ' instead of one customer: jets_price/jfcPrice replace the customer's custGrp item and price rows.
            ' Search text / category only narrow that list. Checked and saved items are always listed so
            ' Save (delete + re-insert checked rows) never drops them.
            Dim searchSQL As String = " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE {0} OR a.itemDesc_ja LIKE {0} OR UPPER(a.itemDesc_long) LIKE {0} " &
                                      " OR UPPER(a.itemWebDesc) LIKE {0} OR UPPER(a.salesPoint) LIKE {0} OR UPPER(a.item_id) = {1}) "
            Dim searchCond As String = " AND NOT (NVL(jp.csPrice, 0) = 0 AND NVL(jp.eaPrice, 0) = 0 AND NVL(jp.lbPrice_orderByQty, 0) = 0)"
            For Each word As String In searchText.ToUpper.Split(New Char() {" ", ",", "."})
                If word.Trim <> "" Then searchCond &= String.Format(searchSQL, DB.SqlEsc("%" & word & "%"), DB.SqlEsc(word))
            Next
            If ctgFilter <> "" AndAlso ctgFilter <> LST_ALL_STR Then
                searchCond &= " AND NVL(jctgGrp2.ctgGrpDesc, NVL(jctgGrp.ctgGrpDesc, 'Other')) = " & DB.SqlEsc(ctgFilter)
            End If

            ' Unsaved edits: isAssigned = what the checkbox shows, isSaved = what is in orderForm_Item.
            Dim checkedIDs As String = ItemIDList(DB, True)
            Dim uncheckedIDs As String = ItemIDList(DB, False)

            Dim SQL As String =
                "SELECT a.item_id, NULL AS itemNr, " &
                "NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc_view, " &
                "NVL(jctgGrp2.ctgGrpDesc, NVL(jctgGrp.ctgGrpDesc, 'Other')) AS ctgName_view, " &
                "NVL(jctgGrp2.dispOrder, NVL(jctgGrp.dispOrder, 99)) AS ctgDispOrder, " &
                "p.eaPerCs, jp.csPrice, jp.eaPrice, jp.lbPrice_OrderByQty, " &
                "'' AS availMsg, " &
                "CASE " &
                IIf(checkedIDs <> "", "WHEN a.item_id IN (" & checkedIDs & ") THEN 1 ", "") &
                IIf(uncheckedIDs <> "", "WHEN a.item_id IN (" & uncheckedIDs & ") THEN 0 ", "") &
                "WHEN fi.item_id IS NOT NULL THEN 1 ELSE 0 END AS isAssigned, " &
                "CASE WHEN fi.item_id IS NOT NULL THEN 1 ELSE 0 END AS isSaved " &
                "FROM   jets_price p " &
                "INNER  JOIN jfcItem a ON a.item_id = p.itemcode " &
                "LEFT   JOIN jfcPrice jp ON jp.item_id = a.item_id AND jp.priceGrp_id = p.pricegpid " &
                "LEFT   JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " &
                "LEFT   JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " &
                "LEFT   JOIN jfcSubCtg jSubCtg2 ON jSubCtg2.ctg_id = p.ctg_id_branch AND jSubCtg2.subCtg_id = p.subCtg_id_branch " &
                "LEFT   JOIN jfcCtgGrp jctgGrp2 ON jctgGrp2.ctgGrp_id = jSubCtg2.ctgGrp_id " &
                "LEFT   JOIN orderForm_Item fi ON fi.item_id = a.item_id AND fi.orderForm_id = :1 " &
                "WHERE  p.pricegpid = :2 " &
                "  AND  (fi.item_id IS NOT NULL" &
                IIf(checkedIDs <> "", " OR a.item_id IN (" & checkedIDs & ")", "") &
                " OR (1 = 1" & searchCond & ")) " &
                " ORDER BY " & sortBy

            Dim dt As DataTable = DB.GetDataTable(SQL,
                IIf(orderFormID = "", "~~NONE~~", orderFormID),
                priceGrpID)
            grdItems.DataSource = dt
            grdItems.DataBind()

            Dim changes As Integer = 0
            For Each dr As DataRow In dt.Rows
                If Val(dr("isAssigned")) <> Val(dr("isSaved")) Then changes += 1
            Next
            lblItemPendingMsg.Text = IIf(changes > 0,
                changes & " unsaved item change(s) - click """ & btnSaveForm.Text & """ to save.", "")
        End Using
    End Sub

    Protected Sub grdItems_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdItems.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        ' Mark items added (checked) but not saved yet
        Dim drv As DataRowView = CType(e.Row.DataItem, DataRowView)
        If Val(drv("isAssigned")) = 1 AndAlso Val(drv("isSaved")) = 0 Then
            CType(e.Row.FindControl("lblAvailMsg"), Label).Text = " (new)"
        End If
    End Sub

    Protected Sub grdItems_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdItems.RowCommand
        If e.CommandName = "REMOVE_ITEM" Then
            ' Same as unchecking the row; removed from orderForm_Item on Save
            SetItemCheck(e.CommandArgument.ToString(), False)
            BindItemsGrid(hfOrderFormID.Value, False)
        End If
    End Sub

    ' Item check state edited since the form was opened: item_id -> checked
    Private Function ItemChecks() As Dictionary(Of String, Boolean)
        If ViewState("ItemChecks") Is Nothing Then
            ViewState("ItemChecks") = New Dictionary(Of String, Boolean)
        End If
        Return CType(ViewState("ItemChecks"), Dictionary(Of String, Boolean))
    End Function

    Private Sub CaptureItemChecks()
        Dim checks As Dictionary(Of String, Boolean) = ItemChecks()
        For Each row As GridViewRow In grdItems.Rows
            Dim chk = CType(row.FindControl("chkItemRow"), CheckBox)
            If chk IsNot Nothing Then
                checks(CString(grdItems.DataKeys(row.RowIndex).Value)) = chk.Checked
            End If
        Next
        ViewState("ItemChecks") = checks
    End Sub

    ' Captures the grid first so other unsaved edits are kept
    Private Sub SetItemCheck(itemID As String, checked As Boolean)
        CaptureItemChecks()
        Dim checks As Dictionary(Of String, Boolean) = ItemChecks()
        checks(itemID) = checked
        ViewState("ItemChecks") = checks
    End Sub

    ' Comma-separated, SQL-escaped item_ids with the given check state ("" if none)
    Private Function ItemIDList(DB As DBHelper, checked As Boolean) As String
        Dim ids As New List(Of String)
        For Each kv As KeyValuePair(Of String, Boolean) In ItemChecks()
            If kv.Value = checked Then ids.Add(DB.SqlEsc(kv.Key))
        Next
        Return String.Join(",", ids)
    End Function

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
    '  SAVE / DELETE FORM
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnSaveForm_Click(sender As Object, e As EventArgs) Handles btnSaveForm.Click
        Dim formName As String = txtFormName.Text.Trim()
        If formName = "" Then
            lblDetailMsg.Text = "Please enter an Order Form Name."
            Return
        End If

        Dim excepGrpID As String = SelectedExcepGrpID()
        If excepGrpID = "" Then
            lblDetailMsg.Text = "Please select an Exception Price Group."
            Return
        End If

        Dim jfcBranchNr As String = lblBranchID.Text
        Dim orderFormID As String = hfOrderFormID.Value
        Dim isNew As Boolean = (orderFormID = "")

        Using DB As New DBHelper
            ' Duplicate check
            Dim dupSQL As String = IIf(isNew,
                "SELECT COUNT(*) FROM orderForm WHERE jfcBranchNr = :1 AND orderFormName = :2",
                "SELECT COUNT(*) FROM orderForm WHERE jfcBranchNr = :1 AND orderFormName = :2 AND orderForm_id <> :3")
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

            DB.BeginTransaction()

            If isNew Then
                orderFormID = DB.GetSequence("ORDERFORM_ID", CUSTGRP_ITEM_ID_LEN)
                DB.ExecuteNonQuery(
                    "INSERT INTO orderForm " &
                    "  (orderForm_id, jfcBranchNr, orderFormName, JFCPriceGrpID_Orig, createUser, createDate, modUser, modDate) " &
                    "VALUES (:1, :2, :3, :4, :5, SYSDATE, :6, SYSDATE)",
                    orderFormID, jfcBranchNr, formName, excepGrpID, userEmail, userEmail)
                hfOrderFormID.Value = orderFormID
            Else
                DB.ExecuteNonQuery(
                    "UPDATE orderForm " &
                    "SET    orderFormName = :1, JFCPriceGrpID_Orig = :2, modUser = :3, modDate = SYSDATE " &
                    "WHERE  orderForm_id  = :4",
                    formName, excepGrpID, userEmail, orderFormID)
            End If

            ' Save checked customers
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

            ' Save checked items
            DB.ExecuteNonQuery(
                "DELETE FROM orderForm_Item WHERE orderForm_id = :1", orderFormID)
            For Each row As GridViewRow In grdItems.Rows
                Dim chk = CType(row.FindControl("chkItemRow"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    Dim itemID As String = CString(grdItems.DataKeys(row.RowIndex).Value)
                    DB.ExecuteNonQuery(
                        "INSERT INTO orderForm_Item (orderForm_id, item_id) VALUES (:1, :2)",
                        orderFormID, itemID)
                End If
            Next

            DB.CommitTransaction()
            InsertEventLog(DB, IIf(isNew, "INSERT", "UPDATE") & " orderForm",
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
                        "DELETE FROM orderForm_Item WHERE orderForm_id = :1", dialogArg1)
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderForm_Customer WHERE orderForm_id = :1", dialogArg1)
                    DB.ExecuteNonQuery(
                        "DELETE FROM orderForm WHERE orderForm_id = :1", dialogArg1)
                    DB.CommitTransaction()
                    InsertEventLog(DB, "DELETE orderForm", dialogArg1, Nothing, Nothing, True, False)
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

    ' Exception price group picked in the detail panel ("" when the branch has none)
    Private Function SelectedExcepGrpID() As String
        Return IIf(lstExcepPriceGrp.Items.Count > 0, lstExcepPriceGrp.SelectedValue, "")
    End Function

    ' Same rule as CustItem: exception customers use their own JETS price group (jets_customer.priceGpID),
    ' so the form uses the price group of its exception group's customers.
    Private Function GetFormPriceGrpID(DB As DBHelper) As String
        Return GetPriceGrpIDByExcepGrp(DB, lblBranchID.Text, SelectedExcepGrpID())
    End Function

    Overrides Sub VerifyRenderingInServerForm(control As Control)
    End Sub

End Class
