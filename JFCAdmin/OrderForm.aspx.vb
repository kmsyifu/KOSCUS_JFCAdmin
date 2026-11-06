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
                'IsJfcHoAdmin and IsKmsSupport run the same query (and "Or" ran both) - read the role once
                Dim jfcRole As String = CString(DB.GetScalar("SELECT userCompany_id FROM webUser_vendor WHERE vendorCompany_id = 'JFC' and email = :1", email))
                ' Same people as Branch Settings (menu link is hidden for everyone else): branch / HO admin, KMS support
                If jfcRole <> "JFC_BRANCH" AndAlso jfcRole <> "JFC_HO" AndAlso jfcRole <> "KMS_SUPPORT" Then
                    Response.Redirect("Home.aspx")
                End If
                If jfcRole = "JFC_HO" OrElse jfcRole = "KMS_SUPPORT" Then
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
            ' Customer # search: JFC customer # (e.g. 1234 or 30-1234) or custGrp_id, not the internal customer_id
            Dim searchCust As String = txtSearchCustNr.Text.Trim().ToUpper().Replace("-", "").Replace(" ", "")
            Dim custLike As String = DB.SqlEsc("%" & searchCust & "%")

            Dim SQL As String =
                "SELECT DISTINCT f.orderForm_id, f.orderFormName, f.JFCPriceGrpID_Orig " &
                "FROM   orderForm f " &
                "LEFT JOIN orderForm_Customer fc ON fc.orderForm_id = f.orderForm_id " &
                "LEFT JOIN customer c ON c.customer_id = fc.customer_id " &
                "WHERE  f.jfcBranchNr = :1 " &
                IIf(searchForm <> "", " AND UPPER(f.orderFormName) LIKE " & DB.SqlEsc("%" & searchForm.ToUpper & "%"), "") &
                IIf(searchCust <> "", " AND (UPPER(c.jfcCustomerNr) LIKE " & custLike &
                                      " OR UPPER(c.jfcBranchNr || c.jfcCustomerNr) LIKE " & custLike &
                                      " OR UPPER(c.custGrp_id) LIKE " & custLike & ")", "") &
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
                    MessageBoxYesNo("Are you sure you want to delete this order form?", "Confirm Deletion", DB)
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
        ViewState("CustChecks") = Nothing
        ViewState("ExcepPriceGrpID") = ""
        grdCustomers.PageIndex = 0
        grdItems.PageIndex = 0
        txtCustSearch.Text = ""
        hfOrderFormID.Value = orderFormID
        Dim isNew As Boolean = (orderFormID = "")
        pnlBranch.Visible = False
        Using DB As New DBHelper
            ' Titles / button text
            If isNew Then
                lblDetailTitle.Text = "Add Exception Price Group Order Form"
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
                    "TO_CHAR(createDate, 'MM/DD/YYYY HH:MI AM') || ' PT by ' || createUser AS createDateFmt, " &
                    "TO_CHAR(modDate,    'MM/DD/YYYY HH:MI AM') || ' PT by ' || modUser   AS modDateFmt " &
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
                lstSortBy.Items.Add(New ListItem("Selected", "isAssigned DESC, item_id"))   ' selected first, then by item #
                lstSortBy.Items.Add(New ListItem("Category", "ctgDispOrder, itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item Description", "itemDesc_view"))
                lstSortBy.Items.Add(New ListItem("Item #", "item_id"))
            End If
            ' Each time the detail page opens: sort by "Selected", empty search text, category All (below)
            lstSortBy.SelectedIndex = 0
            txtItemSearch.Text = ""

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
            lstCtgSearch.SelectedIndex = 0   ' All

            'Bind both grids on this connection (each used to open its own)
            BindCustomersGrid(orderFormID, False, DB)
            BindItemsGrid(orderFormID, False, DB)
        End Using
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

    ' captureChecks: remember the grid's current checkbox state before rebinding, so unsaved
    ' check/uncheck edits survive search and page changes until Save.
    'dbIn: the caller's connection, if it has one open (otherwise a connection is opened here)
    Private Sub BindCustomersGrid(orderFormID As String, Optional captureChecks As Boolean = True, Optional dbIn As DBHelper = Nothing)
        If captureChecks Then CaptureCustChecks()
        Dim DB As DBHelper = If(dbIn, New DBHelper)
        Try
            Dim searchText As String = IIf(watCustSearch.WatermarkText = txtCustSearch.Text, "", txtCustSearch.Text.Trim())
            Dim dt As DataTable = GetCustomersTable(DB, orderFormID, searchText)
            grdCustomers.DataSource = dt
            SetPageInRange(grdCustomers, dt.Rows.Count)
            grdCustomers.DataBind()
        Finally
            If dbIn Is Nothing Then DB.Dispose()
        End Try
    End Sub

    ' Customers of the selected exception price group, sorted by customer #.
    ' isSaved = in orderForm_Customer, isAssigned = what the checkbox shows (saved + unsaved check edits).
    ' searchText: customer # (e.g. 1234, 30-1234, 301234) or part of the name; "" = all customers of the group.
    Private Function GetCustomersTable(DB As DBHelper, orderFormID As String, searchText As String) As DataTable
        Dim searchCond As String = ""
        If searchText <> "" Then
            Dim nameLike As String = DB.SqlEsc("%" & searchText.ToUpper & "%")
            Dim custNr As String = searchText.ToUpper.Replace("-", "").Replace(" ", "")
            Dim nrLike As String = DB.SqlEsc("%" & custNr & "%")
            searchCond = " AND (UPPER(c.customerName) LIKE " & nameLike &
                         " OR UPPER(c.jfcCustomerNr) LIKE " & nrLike &
                         " OR UPPER(c.jfcBranchNr || c.jfcCustomerNr) LIKE " & nrLike &
                         " OR UPPER(c.custGrp_id) LIKE " & nrLike & ") "
        End If
        Dim SQL As String =
            "SELECT c.customer_id, c.customerName, " &
            "CASE WHEN c.jfcCustomerNr IS NOT NULL THEN c.jfcBranchNr || '-' || c.jfcCustomerNr " &
            "     ELSE SUBSTR(c.custGrp_id, 1, 2) || '-' || SUBSTR(c.custGrp_id, 3) END AS custNr_view, " &
            "CASE WHEN fc.customer_id IS NOT NULL THEN 1 ELSE 0 END AS isAssigned, " &
            "CASE WHEN fc.customer_id IS NOT NULL THEN 1 ELSE 0 END AS isSaved " &
            "FROM   customer c " &
            "LEFT   JOIN orderForm_Customer fc " &
            "         ON fc.customer_id = c.customer_id AND fc.orderForm_id = :1 " &
            "WHERE  c.jfcBranchNr = :2 " &
            "  AND  c.JFCPRICEGRPID_ORIG = :3 " & searchCond &
            "ORDER  BY LPAD(NVL(c.jfcCustomerNr, SUBSTR(c.custGrp_id, 3)), 20, '0'), c.customerName"   ' customer # in number order (12 before 100)
        Dim dt As DataTable = DB.GetDataTable(SQL,
            IIf(orderFormID = "", "~~NONE~~", orderFormID),
            lblBranchID.Text,
            SelectedExcepGrpID())
        Dim checks As Dictionary(Of String, Boolean) = CustChecks()
        For Each dr As DataRow In dt.Rows
            Dim custID As String = CString(dr("customer_id"))
            If checks.ContainsKey(custID) Then dr("isAssigned") = IIf(checks(custID), 1, 0)
        Next
        Return dt
    End Function

    ' Customer check state edited since the form was opened (only rows that differ from orderForm_Customer): customer_id -> checked
    Private Function CustChecks() As Dictionary(Of String, Boolean)
        If ViewState("CustChecks") Is Nothing Then
            ViewState("CustChecks") = New Dictionary(Of String, Boolean)
        End If
        Return CType(ViewState("CustChecks"), Dictionary(Of String, Boolean))
    End Function

    Private Sub CaptureCustChecks()
        Dim checks As Dictionary(Of String, Boolean) = CustChecks()
        For Each row As GridViewRow In grdCustomers.Rows
            Dim chk = CType(row.FindControl("chkCustRow"), CheckBox)
            If chk IsNot Nothing Then
                Dim custID As String = CString(grdCustomers.DataKeys(row.RowIndex)("customer_id"))
                If chk.Checked = (Val(grdCustomers.DataKeys(row.RowIndex)("isSaved")) = 1) Then
                    checks.Remove(custID)
                Else
                    checks(custID) = chk.Checked
                End If
            End If
        Next
        ViewState("CustChecks") = checks
    End Sub

    Protected Sub grdCustomers_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdCustomers.PageIndexChanging
        CaptureCustChecks()
        grdCustomers.PageIndex = e.NewPageIndex
        BindCustomersGrid(hfOrderFormID.Value, False)
    End Sub

    Protected Sub btnSearchCust_Click(sender As Object, e As EventArgs) Handles btnSearchCust.Click
        CaptureCustChecks()
        grdCustomers.PageIndex = 0
        BindCustomersGrid(hfOrderFormID.Value, False)
    End Sub

    ' After a search the list can be shorter: keep the page index inside the page count
    Private Sub SetPageInRange(grd As GridView, rowCount As Integer)
        Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(rowCount / grd.PageSize)))
        If grd.PageIndex >= pageCount Then grd.PageIndex = pageCount - 1
    End Sub

    Protected Sub lstExcepPriceGrp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstExcepPriceGrp.SelectedIndexChanged
        ' Customers and items both depend on the exception price group.
        ' Start the item list fresh: drop unsaved checks made under the previous group.
        ViewState("ItemChecks") = Nothing
        ViewState("CustChecks") = Nothing
        grdCustomers.PageIndex = 0
        grdItems.PageIndex = 0
        Using DB As New DBHelper
            BindCustomersGrid(hfOrderFormID.Value, False, DB)
            BindItemsGrid(hfOrderFormID.Value, False, DB)
        End Using
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  ITEMS GRID
    ' ════════════════════════════════════════════════════════════════

    ' captureChecks: remember the grid's current checkbox state before rebinding, so unsaved
    ' check/uncheck edits survive search, sort and add/remove postbacks until Save.
    'dbIn: the caller's connection, if it has one open (otherwise a connection is opened here)
    Private Sub BindItemsGrid(orderFormID As String, Optional captureChecks As Boolean = True, Optional dbIn As DBHelper = Nothing)
        If captureChecks Then CaptureItemChecks()
        Dim DB As DBHelper = If(dbIn, New DBHelper)
        Try
            Dim grpChanged As Boolean
            Dim dt As DataTable = GetItemsTable(DB, orderFormID, True, grpChanged)
            ' Sorted after isAssigned is set, so "Selected" shows unsaved checks at the top
            dt.DefaultView.Sort = lstSortBy.SelectedValue
            grdItems.DataSource = dt.DefaultView
            SetPageInRange(grdItems, dt.Rows.Count)
            grdItems.DataBind()

            Dim changes As Integer = 0
            For Each dr As DataRow In dt.Rows
                If Val(dr("isAssigned")) <> Val(dr("isSaved")) Then changes += 1
            Next
            lblItemPendingMsg.Text = IIf(changes > 0,
                changes & " unsaved item change(s) - click """ & btnSaveForm.Text & """ to save.", "")
            If grpChanged Then
                lblItemPendingMsg.Text &= IIf(lblItemPendingMsg.Text <> "", "<br />", "") &
                    "Exception Price Group changed - items saved for the previous group will be removed when you click """ & btnSaveForm.Text & """."
            End If
        Finally
            If dbIn Is Nothing Then DB.Dispose()
        End Try
    End Sub

    ' Items of the form's exception price group. isSaved = in orderForm_Item, isAssigned = what the checkbox
    ' shows (saved + unsaved check edits). forList = True: the grid list (search / category apply; saved and
    ' checked items are always listed). forList = False: only saved and checked items (Save - every page).
    Private Function GetItemsTable(DB As DBHelper, orderFormID As String, forList As Boolean, ByRef grpChanged As Boolean) As DataTable
        Dim jfcBranchNr As String = lblBranchID.Text
        Dim searchText As String = IIf(watItemSearch.WatermarkText = txtItemSearch.Text, "", txtItemSearch.Text.Trim())
        Dim ctgFilter As String = lstCtgSearch.SelectedValue
        Dim priceGrpID As String = GetFormPriceGrpID(DB)

        ' Same prices as v_excepcust_custgrpitem_small: jfcCustomerPrice rows whose customer_id is the
        ' exception price group (customer.jfcPriceGrpID_orig). Those rows are the group's item list.
        ' jets_price (branch price group) only supplies the branch description / category / ea per cs.
        ' Every listed item must be in that group (enabled, non-zero price). Search text / category only
        ' narrow the list; checked and saved items of the group are always listed so Save
        ' (delete + re-insert checked rows) never drops them.
        Dim groupCond As String = "cp.item_id IS NOT NULL AND cp.enabled = 'Y'" &
                                  " AND NOT (NVL(cp.csPrice, 0) = 0 AND NVL(cp.eaPrice, 0) = 0 AND NVL(cp.lbPrice_orderByQty, 0) = 0 AND NVL(cp.lbPrice_orderByWgt, 0) = 0)"
        Dim searchSQL As String = " AND (UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE {0} OR a.itemDesc_ja LIKE {0} OR UPPER(a.itemDesc_long) LIKE {0} " &
                                  " OR UPPER(a.itemWebDesc) LIKE {0} OR UPPER(a.salesPoint) LIKE {0} OR UPPER(a.item_id) = {1}) "
        Dim searchCond As String = ""
        searchWords = New List(Of String)
        If Not forList Then searchText = ""
        For Each word As String In searchText.ToUpper.Split(New Char() {" ", ",", "."})
            If word.Trim <> "" Then
                searchCond &= String.Format(searchSQL, DB.SqlEsc("%" & word & "%"), DB.SqlEsc(word))
                searchWords.Add(word.Trim)
            End If
        Next
        If forList AndAlso ctgFilter <> "" AndAlso ctgFilter <> LST_ALL_STR Then
            searchCond &= " AND NVL(jctgGrp2.ctgGrpDesc, NVL(jctgGrp.ctgGrpDesc, 'Other')) = " & DB.SqlEsc(ctgFilter)
        End If

        ' Unsaved edits: isAssigned = what the checkbox shows, isSaved = what is in orderForm_Item.
        ' isAssigned is set from ItemChecks() after the query (no IN list, Oracle allows max 1000 per list).
        Dim checkedCond As String = ItemInCond(DB, "a.item_id", True)

        Dim SQL As String =
            "SELECT a.item_id, NULL AS itemNr, " &
            "NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc_view, " &
            "NVL(jctgGrp2.ctgGrpDesc, NVL(jctgGrp.ctgGrpDesc, 'Other')) AS ctgName_view, " &
            "NVL(jctgGrp2.dispOrder, NVL(jctgGrp.dispOrder, 99)) AS ctgDispOrder, " &
            "p.eaPerCs, NVL(cp.csPrice, 0) AS csPrice, NVL(cp.eaPrice, 0) AS eaPrice, NVL(cp.lbPrice_orderByQty, 0) AS lbPrice_OrderByQty, NVL(cp.lbPrice_orderByWgt, 0) AS lbPrice_OrderByWgt, " &
            "'' AS availMsg, " &
            IIf(searchWords.Count > 0, "a.itemDesc_ja, a.itemDesc_long, a.itemWebDesc, a.salesPoint, ",
                "NULL AS itemDesc_ja, NULL AS itemDesc_long, NULL AS itemWebDesc, NULL AS salesPoint, ") &
            "CASE WHEN fi.item_id IS NOT NULL THEN 1 ELSE 0 END AS isAssigned, " &
            "CASE WHEN fi.item_id IS NOT NULL THEN 1 ELSE 0 END AS isSaved " &
            "FROM   jfcItem a " &
            "LEFT   JOIN jfcCustomerPrice cp ON cp.item_id = a.item_id AND cp.customer_id = :1 " &
            "LEFT   JOIN jets_price p ON p.itemcode = a.item_id AND p.pricegpid = :2 " &
            "LEFT   JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " &
            "LEFT   JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " &
            "LEFT   JOIN jfcSubCtg jSubCtg2 ON jSubCtg2.ctg_id = p.ctg_id_branch AND jSubCtg2.subCtg_id = p.subCtg_id_branch " &
            "LEFT   JOIN jfcCtgGrp jctgGrp2 ON jctgGrp2.ctgGrp_id = jSubCtg2.ctgGrp_id " &
            "LEFT   JOIN orderForm_Item fi ON fi.item_id = a.item_id AND fi.orderForm_id = :3 " &
            "WHERE  " & groupCond &
            "  AND  (fi.item_id IS NOT NULL" &
            IIf(checkedCond <> "", " OR " & checkedCond, "") &
            IIf(forList, " OR (1 = 1" & searchCond & ")", "") & ") "

        ' Saved items belong to the form's saved group; after switching groups none are checked
        ' (Save then replaces them with the items checked under the new group).
        grpChanged = (orderFormID <> "" AndAlso SelectedExcepGrpID() <> CString(ViewState("ExcepPriceGrpID")))

        ' DBHelper binds args in the order the :n placeholders appear in the SQL (not by number)
        Dim dt As DataTable = DB.GetDataTable(SQL,
            SelectedExcepGrpID(),
            priceGrpID,
            IIf(orderFormID = "" OrElse grpChanged, "~~NONE~~", orderFormID))
        Dim checks As Dictionary(Of String, Boolean) = ItemChecks()
        For Each dr As DataRow In dt.Rows
            Dim itemID As String = CString(dr("item_id"))
            If checks.ContainsKey(itemID) Then dr("isAssigned") = IIf(checks(itemID), 1, 0)
        Next
        Return dt
    End Function

    ' Upper-cased item search words of the last BindItemsGrid (for the match tag)
    Private searchWords As New List(Of String)

    Protected Sub grdItems_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdItems.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        ' Mark items added (checked) but not saved yet
        Dim drv As DataRowView = CType(e.Row.DataItem, DataRowView)
        If Val(drv("isAssigned")) = 1 AndAlso Val(drv("isSaved")) = 0 Then
            CType(e.Row.FindControl("lblAvailMsg"), Label).Text = " (new)"
        End If
        CType(e.Row.FindControl("lblMatchTag"), Label).Text = MatchTagHtml(drv)
    End Sub

    ' During an item search: for words not found in the shown description / item ID but found in a hidden field
    ' (same fields as the search SQL), a short piece of the text around the match with the word in bold.
    ' No field name (was "Japanese" / "Long description" / "Web description" / "Sales point").
    Private Function MatchTagHtml(drv As DataRowView) As String
        If searchWords.Count = 0 Then Return ""
        Dim desc As String = CString(drv("itemDesc_view")).ToUpper
        Dim itemID As String = CString(drv("item_id")).ToUpper
        Dim fields As String() = {"itemDesc_ja", "itemDesc_long", "itemWebDesc", "salesPoint"}
        Dim html As New List(Of String)
        Dim usedFields As New List(Of String)
        For Each word As String In searchWords
            If desc.Contains(word) OrElse itemID = word Then Continue For
            For Each f As String In fields
                Dim text As String = CString(drv(f))
                Dim pos As Integer = text.ToUpper.IndexOf(word)
                If pos < 0 Then Continue For
                If Not usedFields.Contains(f) Then
                    usedFields.Add(f)
                    html.Add("<span class=""gray"" style=""font-size:smaller;"">" & Snippet(text, pos, word.Length) & "</span>")
                End If
                Exit For
            Next
        Next
        If html.Count = 0 Then Return ""
        Return "<br />" & String.Join("<br />", html)
    End Function

    ' About 60 characters of text around the match, the matched part in bold (HTML encoded)
    Private Function Snippet(text As String, pos As Integer, len As Integer) As String
        text = text.Replace(vbCr, " ").Replace(vbLf, " ")
        Dim start As Integer = Math.Max(0, pos - 25)
        Dim finish As Integer = Math.Min(text.Length, pos + len + 35)
        Return IIf(start > 0, "...", "") &
               HttpUtility.HtmlEncode(text.Substring(start, pos - start)) &
               "<b>" & HttpUtility.HtmlEncode(text.Substring(pos, len)) & "</b>" &
               HttpUtility.HtmlEncode(text.Substring(pos + len, finish - pos - len)) &
               IIf(finish < text.Length, "...", "")
    End Function

    ' Item check state edited since the form was opened (only rows that differ from orderForm_Item): item_id -> checked
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
                Dim itemID As String = CString(grdItems.DataKeys(row.RowIndex)("item_id"))
                If chk.Checked = (Val(grdItems.DataKeys(row.RowIndex)("isSaved")) = 1) Then
                    checks.Remove(itemID)
                Else
                    checks(itemID) = chk.Checked
                End If
            End If
        Next
        ViewState("ItemChecks") = checks
    End Sub

    ' "(col IN (...) OR col IN (...))" for item_ids with the given check state, 1000 per list (Oracle limit); "" if none
    Private Function ItemInCond(DB As DBHelper, column As String, checked As Boolean) As String
        Dim ids As New List(Of String)
        For Each kv As KeyValuePair(Of String, Boolean) In ItemChecks()
            If kv.Value = checked Then ids.Add(DB.SqlEsc(kv.Key))
        Next
        If ids.Count = 0 Then Return ""
        Dim lists As New List(Of String)
        For i As Integer = 0 To ids.Count - 1 Step 1000
            lists.Add(column & " IN (" & String.Join(",", ids.GetRange(i, Math.Min(1000, ids.Count - i))) & ")")
        Next
        Return "(" & String.Join(" OR ", lists) & ")"
    End Function

    Protected Sub lstSortBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstSortBy.SelectedIndexChanged
        CaptureItemChecks()
        grdItems.PageIndex = 0
        BindItemsGrid(hfOrderFormID.Value, False)
    End Sub

    Protected Sub lstCtgSearch_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstCtgSearch.SelectedIndexChanged
        CaptureItemChecks()
        grdItems.PageIndex = 0
        BindItemsGrid(hfOrderFormID.Value, False)
    End Sub

    Protected Sub btnSearchItem_Click(sender As Object, e As EventArgs) Handles btnSearchItem.Click
        CaptureItemChecks()
        grdItems.PageIndex = 0
        BindItemsGrid(hfOrderFormID.Value, False)
    End Sub

    Protected Sub grdItems_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdItems.PageIndexChanging
        CaptureItemChecks()
        grdItems.PageIndex = e.NewPageIndex
        BindItemsGrid(hfOrderFormID.Value, False)
    End Sub

    ' ════════════════════════════════════════════════════════════════
    '  SAVE / DELETE FORM
    ' ════════════════════════════════════════════════════════════════

    Protected Sub btnSaveForm_Click(sender As Object, e As EventArgs) Handles btnSaveForm.Click
        lblDetailMsg.Text = ""
        Dim formName As String = txtFormName.Text.Trim()
        If formName = "" Then
            ShowSaveError("Please enter an Order Form Name.")
            Return
        End If

        Dim excepGrpID As String = SelectedExcepGrpID()
        If excepGrpID = "" Then
            ShowSaveError("Please select an Exception Price Group.")
            Return
        End If

        Dim jfcBranchNr As String = lblBranchID.Text
        Dim orderFormID As String = hfOrderFormID.Value
        Dim isNew As Boolean = (orderFormID = "")

        ' Checks on the shown pages (other pages are already in ViewState)
        CaptureCustChecks()
        CaptureItemChecks()

        Using DB As New DBHelper
            ' Duplicate check: same name in the branch, ignoring upper / lower case and spaces around it
            Dim dupSQL As String = IIf(isNew,
                "SELECT COUNT(*) FROM orderForm WHERE jfcBranchNr = :1 AND UPPER(TRIM(orderFormName)) = UPPER(:2)",
                "SELECT COUNT(*) FROM orderForm WHERE jfcBranchNr = :1 AND UPPER(TRIM(orderFormName)) = UPPER(:2) AND orderForm_id <> :3")
            Dim dupCount As Integer
            If isNew Then
                dupCount = Val(DB.GetScalar(dupSQL, jfcBranchNr, formName))
            Else
                dupCount = Val(DB.GetScalar(dupSQL, jfcBranchNr, formName, orderFormID))
            End If
            If dupCount > 0 Then
                ShowSaveError("Order form name """ & formName & """ already exists. Please enter a different name.", DB)
                Return
            End If

            ' Checked customers and items of every page (not only the rows shown): saved rows plus check edits.
            ' Read before the deletes below (isSaved comes from the rows being replaced). A new form has no saved rows.
            Dim grpChanged As Boolean
            Dim dtCust As DataTable = GetCustomersTable(DB, IIf(isNew, "", orderFormID), "")
            Dim dtItem As DataTable = GetItemsTable(DB, IIf(isNew, "", orderFormID), False, grpChanged)

            ' At least one customer and one item
            If dtCust.Select("isAssigned = 1").Length = 0 Then
                ShowSaveError("Please select at least one customer.", DB)
                Return
            End If
            If dtItem.Select("isAssigned = 1").Length = 0 Then
                ShowSaveError("Please select at least one item.", DB)
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
            For Each dr As DataRow In dtCust.Rows
                If Val(dr("isAssigned")) = 1 Then
                    DB.ExecuteNonQuery(
                        "INSERT INTO orderForm_Customer (orderForm_id, customer_id) VALUES (:1, :2)",
                        orderFormID, CString(dr("customer_id")))
                End If
            Next

            ' Save checked items
            DB.ExecuteNonQuery(
                "DELETE FROM orderForm_Item WHERE orderForm_id = :1", orderFormID)
            For Each dr As DataRow In dtItem.Rows
                If Val(dr("isAssigned")) = 1 Then
                    DB.ExecuteNonQuery(
                        "INSERT INTO orderForm_Item (orderForm_id, item_id) VALUES (:1, :2)",
                        orderFormID, CString(dr("item_id")))
                End If
            Next

            DB.CommitTransaction()
            InsertEventLog(DB, IIf(isNew, "INSERT", "UPDATE") & " orderForm",
                orderFormID, formName, Nothing, True, False)
        End Using

        ' Back to the order form list with the top message bar (same as CustItem)
        ShowListPanel()
        MessageLine("CONFIRM", "Order form """ & formName & """ " & IIf(isNew, "added", "updated") & " successfully.")
    End Sub

    ' Save not done: popup message, and the same text in red at the top of the detail panel
    Private Sub ShowSaveError(message As String, Optional DB As DBHelper = Nothing)
        lblDetailMsg.Text = HttpUtility.HtmlEncode(message)
        MessageBox(message, "Error", DB)
    End Sub

    Protected Sub btnDeleteForm_Click(sender As Object, e As EventArgs) Handles btnDeleteForm.Click
        ViewState("DIALOG_CMD") = "DELETE_ORDERFORM"
        ViewState("DIALOG_ARG1") = hfOrderFormID.Value
        Using DB As New DBHelper
            MessageBoxYesNo("Are you sure you want to delete this order form?", "Confirm Deletion", DB)
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
