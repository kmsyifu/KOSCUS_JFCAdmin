#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\OrderHistory.ascx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","78FD737F16DD3B43EECBDA78168A1C3A4576F681")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\OrderHistory.ascx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate

Partial Class Common_OrderHistory
    Inherits System.Web.UI.UserControl
    Private mDB As New DBHelper

    '--------------------------------------------------------------------------------------------------------------------
    'Parameters passed by caller
    'Session("ORDER_HISTORY_CMD"): Allow user to edit order for "CUST" and "JFCADMIN", view only for "VENDOR" and "CUSTADMIN".
    'Session("ORDER_HISTORY_CUSTID") : Used only for "CUST"
    'Session("ORDER_HISTORY_CUSTCOMPANYID"): Used only for "CUST" and "CUSTADMIN"
    '--------------------------------------------------------------------------------------------------------------------

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim SQL As String
        Dim r As DataReader

        If IsPostBack = False Then
            'Init
            DataBind() 'Fill in customer company type
            chkShowNewOrder.Checked = False
            chkShowWebOrder.Checked = False
            Select Case Session("ORDER_HISTORY_CMD")
                Case "CUST"
                    ''lblOrderHelp.Text = "Click an order to view the detail or edit recent orders"
                    'lblCustCompany_order.Visible = False
                    'lstCustCompany_order.Visible = False
                    lblCustCompany_order.Visible = False
                    lstCustCompany_order.Visible = False
                    lblCustGrp_order.Visible = False
                    lstCustGrp_order.Visible = False
                    lblCustomer_order.Visible = False
                    lstCustomer_order.Visible = False
                    If IsMobile() Then pnlSearchOrder.Visible = False
                    'pnlCustomer.Visible = False
                Case "CUSTADMIN"
                    ''lblOrderHelp.Text = "Click an order to view the detail"
                    lblCustCompany_order.Visible = False
                    lstCustCompany_order.Visible = False
                    If SMALL_CUST_ONLY = True Then
                        lblCustGrp_order.Text = GetCustCompanyType()
                        lblCustomer_order.Visible = False
                        lstCustomer_order.Visible = False
                    End If
                Case "JFCADMIN"
                    'lblOrderHelp.Text = "Click an order to view the detail or edit recent orders"
                    chkShowNewOrder.Checked = True
                    chkShowWebOrder.Checked = True
                    If SMALL_CUST_ONLY = True Then
                        lblCustCompany_order.Text = IIf(IsMobile(), "<br />Customer", "Customer")
                        lblCustGrp_order.Visible = False
                        lstCustGrp_order.Visible = False
                        lblCustomer_order.Visible = False
                        lstCustomer_order.Visible = False
                    End If
                    txtOrderDateFrom.Text = Today.Date.AddMonths(-3).ToString(DATE_FMT)
            End Select

            'Show list of customer company
            Common.ShowCustCompanyList(mDB, GetUserEmail(mDB), IIf(Session("ORDER_HISTORY_CMD") = "CUSTADMIN", True, False), lstCustCompany_order, IIf(Session("ORDER_HISTORY_CMD") = "JFCADMIN", True, False), LST_ALL_STR)
            If Session("ORDER_HISTORY_CMD") = "CUST" OrElse Session("ORDER_HISTORY_CMD") = "CUSTADMIN" Then lstCustCompany_order.Text = Session("ORDER_HISTORY_CUSTCOMPANYID")
            
            'Common.ShowCustCompanyList(mDB, GetUserEmail(mDB), True, lstCustCompany_order, True, LST_ALL_STR)
            'Show list of customer group and customer
            'lstCustCompany_order_SelectedIndexChanged(Nothing, Nothing)
            'Show list of customer group
            Common.ShowCustGrpList(mDB, lstCustCompany_order.Text, lstCustGrp_order, IIf(Session("ORDER_HISTORY_CMD") = "JFCADMIN", True, False), LST_ALL_STR)
            'Show list of customer
            Common.ShowCustomerList(mDB, lstCustGrp_order.Text, lstCustomer_order, IIf(Session("ORDER_HISTORY_CMD") = "JFCADMIN", True, False), LST_ALL_STR)

            'Show list of order status
            'ShowCodeMasterList(mDB, "ORDERSTATE", lstOrderState_order, False, True, LST_ALL_STR)

            ''Restrict access for JFC Admin if customer company, customer group or customer has been set for the user
            'If Session("ORDER_HISTORY_CMD") = "JFCADMIN" Then
            '    SQL = "SELECT * FROM webuser_vendor WHERE vendorCompany_id = 'JFC' AND email = " & mDB.SqlEsc(GetUserEmail(mDB))
            '    r = mDB.GetReader(SQL)
            '    If r.Read() Then
            '        lstCustCompany_order.Text = CString(r("custCompany_id"))
            '        lstCustCompany_order.Enabled = IIf(CString(r("custCompany_id")) = String.Empty, True, False)
            '        Common.ShowCustGrpList(mDB, lstCustCompany_order.Text, lstCustGrp_order)
            '        lstCustGrp_order.Text = CString(r("custGrp_id"))
            '        lstCustGrp_order.Enabled = IIf(CString(r("custGrp_id")) = String.Empty, True, False)
            '        Common.ShowCustomerList(mDB, lstCustGrp_order.Text, lstCustomer_order)
            '        lstCustomer_order.Text = CString(r("customer_id"))
            '        lstCustomer_order.Enabled = IIf(CString(r("customer_id")) = String.Empty, True, False)
            '    End If
            '    r.close()
            'End If

            'Show sort by
            'chkShowNewOrder_CheckedChanged also need change Items Text
            lstOrderBy_order.Items.Clear()
            lstOrderBy_order.Items.Add(New ListItem("Delivery date: Old to new", "deliveryDate, createDate, webOrder_id, jetsOrderNr"))
            lstOrderBy_order.Items.Add(New ListItem("Delivery date: New to old", "deliveryDate DESC, createDate DESC, webOrder_id DESC, jetsOrderNr DESC"))
            lstOrderBy_order.Items.Add(New ListItem("Order date: New to old", "createDate DESC, webOrder_id DESC, jetsOrderNr DESC"))
            lstOrderBy_order.Items.Add(New ListItem("Customer #", "jfcBranchAndCustomerNr, createDate DESC, webOrder_id DESC, jetsOrderNr DESC"))
            lstOrderBy_order.Items.Add(New ListItem("Sales Rep", "jfccontactname, createDate DESC, webOrder_id DESC, jetsOrderNr DESC"))


            'Get argument for search textbox
            If Request.QueryString("orderID") <> "" Then
                txtOrder_order.Text = Request.QueryString("orderID")
            End If

            'Show order history
            showOrderList()

            'Get argument for redirecting to order detail page
            If Request.QueryString("webOrderID") <> "" Then
                'Error check
                SQL = "SELECT * FROM sohdr WHERE webOrder_id = " & mDB.SqlEsc(Request.QueryString("webOrderID")) & _
                        IIf(Session("ORDER_HISTORY_CMD") = "JFCADMIN", "", " AND customer_id IN (SELECT customer_id FROM webuser_customer WHERE email = " & mDB.SqlEsc(GetUserEmail(mDB)) & ")")
                r = mDB.GetReader(SQL)
                If r.Read() Then
                    'Show order detail
                    showOrderDtl(Common.CString(r("webOrder_id")), Common.CString(r("customer_id")), True)
                Else
                    MessageBox("Specified order could not be found", "Error", mDB)
                End If
                r.close()
                Exit Sub
            End If
        End If
    End Sub

    Protected Sub lstOrderBy_order_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_order.SelectedIndexChanged
        showOrderList()
    End Sub

    'Protected Sub lstOrderState_order_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderState_order.SelectedIndexChanged
    '    showOrderList()
    'End Sub

    Protected Sub lstCustCompany_order_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany_order.SelectedIndexChanged
        ShowCustGrpList(mDB, lstCustCompany_order.Text, lstCustGrp_order, IIf(lstCustCompany_order.Text = String.Empty, True, False), LST_ALL_STR)
        lstCustGrp_order_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustGrp_order_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustGrp_order.SelectedIndexChanged
        ShowCustomerList(mDB, lstCustGrp_order.Text, lstCustomer_order, IIf(lstCustCompany_order.Text = String.Empty, True, False), LST_ALL_STR)
        lstCustomer_order_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustomer_order_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustomer_order.SelectedIndexChanged
        showOrderList()
    End Sub

    Protected Sub grdOrder_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdOrder.RowCommand
        Dim row As GridViewRow = Nothing
        Dim orderID As String
        Dim customerID As String
        Dim index As Integer
        Dim lnk As LinkButton
        Dim lbl As Label




        If e.CommandName = "Page" Then
            'Page is changed

            showOrderList()
            'Don't remember scroll position
            Page.MaintainScrollPositionOnPostBack = False


        ElseIf e.CommandName = "EDIT_WEB_ROW" Then
            'Show order detail
            index = Convert.ToInt32(e.CommandArgument)
            lnk = grdOrder.Rows.Item(index).FindControl("lnkWebOrderID")
            orderID = lnk.Text
            lbl = grdOrder.Rows.Item(index).FindControl("lblCustomerID")
            customerID = lbl.Text
            showOrderDtl(orderID, customerID, True)
            'showOrderDtl(Convert.ToString(e.CommandArgument), customerID, IIf(e.CommandName = "EDIT_WEB_ROW", True, False))
        ElseIf e.CommandName = "EDIT_JETS_ROW" Then
            'Show order detail
            index = Convert.ToInt32(e.CommandArgument)
            lnk = grdOrder.Rows.Item(index).FindControl("lnkJetsOrderID")
            orderID = lnk.Text
            lbl = grdOrder.Rows.Item(index).FindControl("lblCustomerID")
            customerID = lbl.Text
            showOrderDtl(orderID, customerID, False)
            'ElseIf e.CommandName = "DOWNLOAD" Then
            '    'Download invoice
            '    index = Convert.ToInt32(e.CommandArgument)
            '    lbl = grdOrder.Rows.Item(index).FindControl("lblOrderNrWoLink")
            '    orderNr = lbl.Text
            '    lbl = grdOrder.Rows.Item(index).FindControl("lblCustomerID")
            '    customerID = lbl.Text
            '    SQL = "SELECT directory, fileName FROM customer_document " & _
            '                "WHERE UPPER(docType) = 'INVOICE' AND customer_id = " & mDB.SqlEsc(customerID) & " AND docName = " & mDB.SqlEsc(orderNr)
            '    r = mDB.GetReader(SQL)
            '    If r.Read Then
            '        rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
            '    End If
            '    If rtn = False Then MessageBox("Cannot open the document", "Error", mDB)
        End If
    End Sub

    Protected Sub grdOrder_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdOrder.RowDataBound
        'Highlight the mourse-over row and make it selectable
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.CssClass = "selectRow"
            e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
            'e.Row.Attributes.Add("onmouseover", "this.className='selectRowClickHighlight';")
            'Dim lnk As LinkButton = e.Row.FindControl("lnkSelectRow")
            'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackClientHyperlink(lnk, ""))
        End If
    End Sub

    Protected Sub btnSearch_order_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Dim dateVal As Date

        'Error check
        If txtOrderDateFrom.Text <> "" AndAlso DateTime.TryParseExact(txtOrderDateFrom.Text, DATE_FMT, Nothing, Nothing, dateVal) = False Then
            MessageBox("Order Date has to be in " & DATE_FMT.ToLower & " format", "Error", mDB)
            Exit Sub
        End If
        If txtOrderDateTo.Text <> "" AndAlso DateTime.TryParseExact(txtOrderDateTo.Text, DATE_FMT, Nothing, Nothing, dateVal) = False Then
            MessageBox("Order date has to be in " & DATE_FMT.ToLower & " format", "Error", mDB)
            Exit Sub
        End If

        'Show order list
        showOrderList()
    End Sub

    Protected Sub chkShowWebOrder_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkShowWebOrder.CheckedChanged
        showOrderList()
    End Sub

    Protected Sub chkShowNewOrder_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkShowNewOrder.CheckedChanged

        If chkShowNewOrder.Checked Then
            If lstOrderBy_order.Items.FindByText("Delivery date: Old to new") IsNot Nothing Then
                lstOrderBy_order.SelectedValue = lstOrderBy_order.Items.FindByText("Delivery date: Old to new").Value
            End If
            'lstOrderBy_order.Items.FindByText("Delivery date: Old to new").Selected = True
        Else
            If lstOrderBy_order.Items.FindByText("Order date: New to old") IsNot Nothing Then
                lstOrderBy_order.SelectedValue = lstOrderBy_order.Items.FindByText("Order date: New to old").Value
            End If

            

        End If

        showOrderList()
    End Sub

    Protected Sub lnkBack_order_Click(sender As Object, e As System.EventArgs) Handles lnkBack_order.Click
        'Return back to customer list
        'showOrderList()
        pnlList_order.Visible = True
        pnlDtl_order.Visible = False
        'If Session("CHANGE_NEWSTATUS") Then
        '    showOrderList()
        'End If
    End Sub

    Protected Sub ctrOrderDtl_PreRender(sender As Object, e As System.EventArgs) Handles ctrOrderDtl.PreRender
        Select Case Session("ORDERDTL_RETURNCMD")
            Case "DELETED", "UPDATED"
                Session("ORDERDTL_RETURNCMD") = ""
                'Return back to order list
                showOrderList()
            Case "EMAIL"
                Session("ORDERDTL_RETURNCMD") = ""
        End Select
    End Sub

    Private Function convertOrderStateFromWebToJets(ByVal webOrderState As String) As String
        Select Case webOrderState
            Case "N", "O", "P", "A", "C", "U"
                Return "'O', 'E', 'H'"
            Case "S"
                Return "'S'"
            Case "I"
                Return "'R'"
            Case "D"
                Return "'U', 'D'"
        End Select
        Return String.Empty
    End Function

    Private Sub showOrderList()
        Dim SQL As String
        Dim joinWeb As String = ""
        Dim joinJets As String = ""
        Dim customers As String = ""
        Dim r As DataReader

        'Show order list
        pnlList_order.Visible = True
        pnlDtl_order.Visible = False

        'Set properties
        'grdOrder.Columns(GetColumnIndex(grdOrder, "customerName")).HeaderText = GetCustCompanyType()
        If Session("ORDER_HISTORY_CMD") = "CUST" Then
            grdOrder.Columns(GetColumnIndex(grdOrder, "vendorname")).Visible = IIf(GetNrCustGrpVendor(mDB, GetCustGrpID(mDB, Session("ORDER_HISTORY_CUSTID")), Nothing) > 1, True, False) 'Order Description (vendor and next delivery date)
        Else
            grdOrder.Columns(GetColumnIndex(grdOrder, "vendorname")).Visible = False 'TODO: Uncomment this later
        End If
        If Session("ORDER_HISTORY_CMD") = "CUST" AndAlso Common.ApplyInEmailOrCustomer(mDB, Session("ORDER_HISTORY_CUSTID"), "HidePrice") = True Then
            grdOrder.Columns(GetColumnIndex(grdOrder, "totalamt")).Visible = False
        End If
        If IsMobile() = True Then
            grdOrder.Columns(0).Visible = False 'Order ID
            grdOrder.Columns(GetColumnIndex(grdOrder, "jfcBranchAndCustomerNr")).Visible = False
            grdOrder.Columns(GetColumnIndex(grdOrder, "custordernr")).Visible = False
            grdOrder.Columns(GetColumnIndex(grdOrder, "vendorName")).Visible = False
            'grdOrder.Columns(GetColumnIndex(grdOrder, "totalamt")).Visible = False
            grdOrder.Columns(GetColumnIndex(grdOrder, "message_view")).Visible = False
            grdOrder.Columns(GetColumnIndex(grdOrder, "createUser")).Visible = False
        End If

        If lstCustCompany_order.Items.Cast(Of ListItem)().All(Function(x) x.Text.Contains("05-") Or x.Text.Contains(LST_ALL_STR)) Then
            grdOrder.Columns(GetColumnIndex(grdOrder, "REFERENCE")).Visible = True
            grdOrder.Columns(GetColumnIndex(grdOrder, "ShipDate_fmt")).Visible = False
            grdOrder.Columns(GetColumnIndex(grdOrder, "message_view")).Visible = False
        End If


        'Get customer ID
        If Session("ORDER_HISTORY_CMD") = "CUST" Then
            customers = mDB.SqlEsc(Session("ORDER_HISTORY_CUSTID"))
        ElseIf Session("ORDER_HISTORY_CMD") = "CUSTADMIN" Then
            customers = mDB.SqlEsc(lstCustomer_order.Text)
        ElseIf lstCustomer_order.Text <> String.Empty Then
            customers = "(1, " & mDB.SqlEsc(lstCustomer_order.Text) & ")"
            'customers = mDB.SqlEsc(lstCustomer_order.Text) 'Error occurred as customers are too long
        Else
            'All customer is selected or customer ID is specified for JFCAdmin
            'SQL = "SELECT '''' || REPLACE(WM_CONCAT(customer_id), ',', ''',''') || '''' FROM webuser_customer WHERE customer_id <> 'ADMIN' AND email = " & mDB.SqlEsc(GetUserEmail(mDB))
            'customers = CString(mDB.GetScalar(SQL))
            'SQL = "SELECT '''' || customer_id || ''',' AS customerID FROM webuser_customer WHERE customer_id <> 'ADMIN' AND email = " & mDB.SqlEsc(GetUserEmail(mDB)) & " GROUP BY customer_id" 'Error occurred as customers are too long
            SQL = "SELECT '(1, ''' || customer_id || '''),' AS customerID " & _
                    "FROM webuser_customer " & _
                    "WHERE customer_id <> 'ADMIN' AND email = " & mDB.SqlEsc(GetUserEmail(mDB)) & _
                    IIf(chkShowWebOrder.Checked = True, "  AND customer_id IN (SELECT customer_id FROM customer WHERE allowCustOrder = 'Y') ", "") & _
                    IIf(txtJfcCustomerNr_order.Text <> String.Empty, " AND customer_id IN (SELECT customer_id FROM customer WHERE jfcCustomerNr = " & mDB.SqlEsc(txtJfcCustomerNr_order.Text) & ") ", "") & _
                    "GROUP BY customer_id"
            r = mDB.GetReader(SQL)
            While r.Read()
                customers &= CString(r("customerID"))
            End While
            r.close()
            If customers <> String.Empty Then
                'Remove the last comma
                customers = Left(customers, customers.Length - 1)
            End If
        End If

        Select Case Session("ORDER_HISTORY_CMD")
            'Case "CUST"
            '    whereClauseWeb = "INNER JOIN customer c ON h.customer_id = c.customer_id " & _
            '                    "WHERE h.orderState <> 'D' AND h.customer_id = " & mDB.SqlEsc(Session("ORDER_HISTORY_CUSTID"))
            'Case "CUSTADMIN"
            '    whereClauseWeb = "INNER JOIN customer c ON h.customer_id = c.customer_id AND h.orderState <> 'D' " & _
            '                    "INNER JOIN custGrp g ON c.custGrp_id = g.custGrp_id AND g.custCompany_id = " & mDB.SqlEsc(Session("ORDER_HISTORY_CUSTCOMPANYID")) & " " & _
            '                    "WHERE 1 = 1 "
            'Case "JFCADMIN"
            '    whereClauseWeb = "INNER JOIN customer c ON h.customer_id = c.customer_id AND h.orderState <> 'D' " & _
            '                    "INNER JOIN custGrp g ON g.custGrp_id = c.custGrp_id " & _
            '                    "INNER JOIN webUser_customer u ON u.custCompany_id = g.custCompany_id AND u.customer_id = 'ADMIN' AND u.email = " & mDB.SqlEsc(GetUserEmail(mDB)) & _
            '                    IIf(lstCustCompany_order.Text <> String.Empty, " AND u.custCompany_id = " & mDB.SqlEsc(lstCustCompany_order.Text), "") & " " & _
            '                    "WHERE 1 = 1 "
            Case "CUST"
                joinWeb = " INNER JOIN customer c ON h.customer_id = c.customer_id AND h.customer_id = " & customers 'mDB.SqlEsc(Session("ORDER_HISTORY_CUSTID"))
                joinJets = " INNER JOIN customer c ON j.jfcBranchNr = c.jfcBranchNr AND j.jfcCustomerNr = c.jfcCustomerNr AND c.customer_id = " & customers 'mDB.SqlEsc(Session("ORDER_HISTORY_CUSTID"))
            Case "CUSTADMIN"
                joinWeb = " INNER JOIN customer c ON h.customer_id = c.customer_id AND h.customer_id = " & customers & _
                            " INNER JOIN custGrp g ON c.custGrp_id = g.custGrp_id AND g.custCompany_id = " & mDB.SqlEsc(Session("ORDER_HISTORY_CUSTCOMPANYID"))
                joinJets = " INNER JOIN customer c ON j.jfcBranchNr = c.jfcBranchNr AND j.jfcCustomerNr = c.jfcCustomerNr AND c.customer_id = " & customers & _
                             " INNER JOIN custGrp g ON c.custGrp_id = g.custGrp_id AND g.custCompany_id = " & mDB.SqlEsc(Session("ORDER_HISTORY_CUSTCOMPANYID"))
                'Case "JFCADMIN"
            Case Else
                
                joinWeb = " INNER JOIN customer c ON h.customer_id = c.customer_id " & _
                            " INNER JOIN webUser_customer u ON u.customer_id = c.customer_id AND u.userCompany_id = 'JFC' AND u.email = " & mDB.SqlEsc(GetUserEmail(mDB)) '& _


                joinJets = " INNER JOIN customer c ON j.jfcBranchNr = c.jfcBranchNr AND j.jfcCustomerNr = c.jfcCustomerNr " & _
                            " INNER JOIN webUser_customer u ON u.customer_id = c.customer_id AND u.userCompany_id = 'JFC' AND u.email = " & mDB.SqlEsc(GetUserEmail(mDB)) '& _
        End Select

        'SQL = String.Format("SELECT h.weborder_id,code.codeDesc AS orderState, h.customer_id, v.vendorname, h.custordernr, h.totalamt, h.orderdate, " & _
        '                       " h.deliverydate, h.createDate, h.createUser, cust.customerName, SUBSTR(h.message, 1, 30) || CASE WHEN LENGTH(h.message) > 30 THEN '...' END AS message_view " & _
        '                       "FROM sohdr h " & _
        '                       "LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE' AND h.orderstate = code.code_id " & _
        '                       "LEFT OUTER JOIN customer cust ON cust.customer_id = h.customer_id " & _
        '                       "INNER JOIN custgrp_vendor v on h.custgrpvendor_id = v.custgrpvendor_id " & _
        '                       whereClauseWeb & _
        '                       IIf(lstOrderState_order.Text <> "", " AND h.orderState = " & mDB.SqlEsc(lstOrderState_order.Text), "") & _
        '                       IIf(txtOrder_order.Text <> "", " AND (h.webOrder_id = " & mDB.SqlEsc(txtOrder_order.Text) & " OR h.custOrderNr = " & mDB.SqlEsc(txtOrder_order.Text) & ")", "") & _
        '                       IIf(txtOrderDateFrom.Text <> "", " AND h.orderDate >= TO_DATE(" & mDB.SqlEsc(txtOrderDateFrom.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") & _
        '                       IIf(txtOrderDateTo.Text <> "", " AND h.orderDate <= TO_DATE(" & mDB.SqlEsc(txtOrderDateTo.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") _
        '                    )
        'SQL = String.Format("SELECT h.orderID, code.codeDesc AS orderState, h.customer_id, v.vendorname, h.custordernr, h.totalamt, h.orderdate, h.deliveryDate, h.createDate, h.createUser, cust.customerName, message_view" & _
        '                       "FROM (SELECT weborder_id AS orderID, customer_id, custordernr, totalamt, orderdate, " & _
        '                       " deliverydate, createDate, createUser, SUBSTR(h.message, 1, 30) || CASE WHEN LENGTH(h.message) > 30 THEN '...' END AS message_view " & _
        '                       "FROM sohdr " & _
        '                       "UNION " & _
        '                       "SELECT ... FROM sohdr_jets ) h" & _
        '                       "LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE' AND h.orderstate = code.code_id " & _
        '                       "LEFT OUTER JOIN customer cust ON cust.customer_id = h.customer_id " & _
        '                       "INNER JOIN custgrp_vendor v on h.custgrpvendor_id = v.custgrpvendor_id " & _
        '                       whereClauseWeb & _
        '                       IIf(lstOrderState_order.Text <> "", " AND h.orderState = " & mDB.SqlEsc(lstOrderState_order.Text), "") & _
        '                       IIf(txtOrder_order.Text <> "", " AND (h.webOrder_id = " & mDB.SqlEsc(txtOrder_order.Text) & " OR h.custOrderNr = " & mDB.SqlEsc(txtOrder_order.Text) & ")", "") & _
        '                       IIf(txtOrderDateFrom.Text <> "", " AND h.orderDate >= TO_DATE(" & mDB.SqlEsc(txtOrderDateFrom.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") & _
        '                       IIf(txtOrderDateTo.Text <> "", " AND h.orderDate <= TO_DATE(" & mDB.SqlEsc(txtOrderDateTo.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") _
        '                    )
        'Show web orders before invoiced and show JETS orders for non web orders and invoiced orders
        SQL = String.Format("SELECT o.weborder_id, o.jetsOrderNr, CASE WHEN o.weborder_id IS NOT NULL THEN 'Y' ELSE 'N' END AS isWebOrder, o.orderState, o.orderStateDesc, o.vendorname, o.custordernr, o.totalamt, " & _
                               " o.customer_id, o.jfcBranchNr, o.jfcCustomerNr, o.jfcBranchNr || '-' || o.jfcCustomerNr AS jfcBranchAndCustomerNr, " & _
                               " o.createUser, o.createDate,o.REFERENCE, o.customerName, o.message_view, o.fileName, o.directory, o.deliveryDate, o.jfccontactname, " & _
                               IIf(IsMobile(), "TO_CHAR(o.orderdate, " & mDB.SqlEsc(SHORTDATE_FMT) & ")", "TO_CHAR(o.orderdate, " & mDB.SqlEsc(DATE_FMT) & ")") & " AS orderDate_fmt, " & _
                               IIf(IsMobile(), "TO_CHAR(o.deliveryDate, " & mDB.SqlEsc(SHORTDATE_FMT) & ")", "TO_CHAR(o.deliveryDate, " & mDB.SqlEsc(DATE_FMT) & ")") & " AS deliveryDate_fmt, " & _
                               IIf(IsMobile(), "TO_CHAR(o.deliveryDate - o.SHIPDATEOFFSET, " & mDB.SqlEsc(SHORTDATE_FMT) & ")", "TO_CHAR(o.deliveryDate - o.SHIPDATEOFFSET, " & mDB.SqlEsc(DATE_FMT) & ")") & " AS ShipDate_fmt " & _
                               "FROM (SELECT h.weborder_id, NULL AS jetsOrderNr, h.orderstate, code.codeDesc AS orderStateDesc, v.vendorname, h.custordernr, h.totalamt, h.orderdate, h.customer_id, c.jfcBranchNr, c.jfcCustomerNr, c.SHIPDATEOFFSET, " & _
                               "  CASE WHEN h.orderState NOT IN ('D', 'I', 'S') THEN h.deliveryDate ELSE (SELECT MAX(jh.deliveryDate) FROM sohdr_jets jh WHERE jh.webOrder_id = h.webOrder_id) END AS deliveryDate, " & _
                               "  h.createUser, h.createDate, h.REFERENCE, c.customerName, SUBSTR(h.message, 1, 30) || CASE WHEN LENGTH(h.message) > 30 THEN '...' END AS message_view, NULL AS fileName, NULL AS directory, c.jfccontactname " & _
                               " FROM sohdr h " & _
                               " LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE' AND h.orderstate = code.code_id " & _
                               " INNER JOIN custgrp_vendor v on h.custgrpvendor_id = v.custgrpvendor_id " & _
                               " " & joinWeb & _
                               " WHERE 1 = 1 AND h.orderState <> 'I' " & _
                               IIf(chkShowNewOrder.Checked = True, " AND h.orderState = 'N'", "") & _
                               "UNION " & _
                               " SELECT j.webOrder_id, j.orderNr AS jetsOrderNr, j.orderstate, code.codeDesc AS orderStateDesc, v.vendorname, j.custordernr, " & _
                               "  CASE WHEN j.orderState = 'R' THEN j.totalAmt_ship ELSE j.totalAmt END AS totalAmt, j.orderdate, c.customer_id, c.jfcBranchNr, c.jfcCustomerNr,0 as SHIPDATEOFFSET, " & _
                               "  j.deliverydate, j.createUser, j.createDate, j.REFERENCE, c.customerName, NULL AS message_view, NULL AS fileName, NULL AS directory, c.jfccontactname " & _
                               " FROM sohdr_jets j " & _
                               " LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE_JETS' AND j.orderstate = code.code_id " & _
                               " INNER JOIN jfcVendor v on j.jfcBranchNr = v.jfcBranchNr " & _
                               " " & joinJets & _
                               " WHERE (j.orderState NOT IN ('U', 'D') OR (j.orderState IN ('U', 'D') AND j.webOrder_id IS NOT NULL)) AND (j.webOrder_id is NULL OR j.orderState IN ('R','U','D')) " & _
                               IIf(chkShowWebOrder.Checked = True, " AND j.webOrder_id IS NOT NULL", "") & _
                               IIf(chkShowNewOrder.Checked = True, " AND 1 <> 1", "") & _
                               ") o WHERE 1 = 1" & _
                               IIf(lstCustomer_order.Text <> "", " and o.customer_id = " & mDB.SqlEsc(lstCustomer_order.Text), "") & _
                               IIf(txtJfcCustomerNr_order.Text <> "", " AND o.jfcCustomerNr = " & mDB.SqlEsc(txtJfcCustomerNr_order.Text), "") & _
                               IIf(txtOrder_order.Text <> "", " AND (o.weborder_id = " & mDB.SqlEsc(txtOrder_order.Text) & " OR o.jetsOrderNr = " & mDB.SqlEsc(txtOrder_order.Text) & " OR o.custOrderNr = " & mDB.SqlEsc(txtOrder_order.Text) & ")", "") & _
                               IIf(txtOrderDateFrom.Text <> "", " AND o.orderDate >= TO_DATE(" & mDB.SqlEsc(txtOrderDateFrom.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") & _
                               IIf(txtOrderDateTo.Text <> "", " AND o.orderDate <= TO_DATE(" & mDB.SqlEsc(txtOrderDateTo.Text) & ", " & mDB.SqlEsc(DATE_FMT) & ")", "") & _
                               IIf(txtJfccontactname.Text <> "", " AND o.jfccontactname like " & mDB.SqlEsc("%" & txtJfccontactname.Text.ToUpper & "%"), "") & ""
                        ) '&  '" FETCH FIRST " & grdOrder.PageIndex * 50 & " ROWS ONLY"

        'IIf(txtJfcBranchNr_order.Text <> "", " AND o.jfcBranchNr = " & mDB.SqlEsc(txtJfcBranchNr_order.Text), "") & _
        'h.orderState <> 'D' 'Hide canceled orders
        'Added "AND h.orderState <> 'I'" and "(xxx OR j.orderState = 'R')" to show JETS order for invoiced orders
        '"  CASE WHEN jh.deliveryDate IS NULL THEN h.deliveryDate ELSE jh.deliveryDate END AS deliveryDate, " & _
        '" LEFT OUTER JOIN sohdr_jets jh on jh.webOrder_id = h.webOrder_id " & _
        'IIf(lstOrderState_order.Text <> "", " AND j.orderState IN (" & convertOrderStateFromWebToJets(lstOrderState_order.Text) & ")", "") & _
        '" LEFT OUTER JOIN customer_document d ON j.orderNr = d.docName AND UPPER(d.docType) = 'INVOICE' AND c.customer_id = d.customer_id " & _

        dsOrder.ProviderName = "System.Data.OracleClient"
        dsOrder.ConnectionString = mDB.GetConnectionString
        dsOrder.SelectCommand = SQL
        grdOrder.DataSourceID = "dsOrder"
        'grdOrder.Sort("weborder_id", SortDirection.Descending)
        'grdOrder.Sort(IIf(chkShowNewOrder.Checked = True, "deliveryDate, createDate, webOrder_id, jetsOrderNr", lstOrderBy_order.SelectedValue), Nothing)
        grdOrder.Sort(lstOrderBy_order.SelectedValue, Nothing)
        If Session("ORDER_HISTORY_CMD") = "CUST" Then grdOrder.Columns(GetColumnIndex(grdOrder, "customerName")).Visible = False
        grdOrder.DataBind()
        lblTotalOrderAmt.Text = String.Format("Total {0} Orders", CType(dsOrder.Select(DataSourceSelectArguments.Empty), DataView).Count)
        ViewState("GRDORDER_NUMBER") = grdOrder.PagerSettings.PageButtonCount
    End Sub

    'Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
    '    grdOrder.PageIndex = e.NewPageIndex
    '    'grdOrder.PagerSettings.PageButtonCount = ViewState("GRDORDER_NUMBER")
    '    showOrderList()
    '    'bindGrid()
    'End Sub

    Private Sub showOrderDtl(ByVal orderID As String, ByVal customerID As String, ByVal showWebOrder As Boolean)
        'Show detail of the order
        pnlList_order.Visible = False
        pnlDtl_order.Visible = True

        'Pass parameters to orderDtl
        If showWebOrder = True Then
            Session("ORDERDTL_MODE") = IIf(Session("ORDER_HISTORY_CMD") = "CUSTADMIN" OrElse Common.AllowCustOrder(mDB, customerID) = False, "VIEW", "EDIT")
        Else
            Session("ORDERDTL_MODE") = "VIEW_JETS"
        End If
        Session("ORDERDTL_CMD") = Session("ORDER_HISTORY_CMD")
        Session("ORDERDTL_ORDERID") = orderID
        Session("ORDERDTL_CUSTID") = customerID
        ctrOrderDtl.pageLoad()
    End Sub

End Class


#End ExternalSource
