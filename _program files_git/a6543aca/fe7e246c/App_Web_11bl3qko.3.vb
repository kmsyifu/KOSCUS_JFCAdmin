#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\Print.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","B6F69880B7C1D4D41C13A67A42DD143FC6A7B53E")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\Print.aspx.vb",1)
Imports Common

Partial Class Common_Print
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'Using DB As New DBHelper
        '    If IsPostBack = False Then
        '        Select Case Session("PRINT_TYPE")
        '            Case "ORDER_FORM"
        '                pnlOrderForm.Visible = True
        '                showOrderForm()
        '        End Select

        '        'Print this form
        '        CallJavaScript(Me.Page, "window.print();")
        '        'Dim strScript As String = "<script language=JavaScript>"
        '        'strScript += "window.print();"
        '        'strScript += "</" + "script>"
        '        'If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
        '        '    ClientScript.RegisterStartupScript(Me.GetType(), "clientScript", strScript)
        '        'End If
        '    End If
        'End Using
    End Sub

    'Private Sub showOrderForm()
    '    Using DB As New DBHelper
    '        Dim SQL As String
    '        Dim email As String = GetUserEmail(DB)
    '        Dim customerID As String = Session("PRINT_ARG1") 'Session("SYS_SESSION_CUSTID")
    '        Dim custGrpID As String = GetCustGrpID(DB, customerID)
    '        Dim jfcBranchNr As String = ""
    '        Dim jfcCustomerNr As String = ""
    '        Dim nrCustGrpVendor As Integer
    '        Dim useCustomerCtg As Boolean = Common.UseCustomerCtg(DB, customerID)

    '        'Get info
    '        nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)

    '        'Set properties
    '        grdOrderForm.Columns(GetColumnIndex(grdOrderForm, "ITEMGRPNAME")).Visible = IIf(GetNrItemGrpInCustGrp(DB, custGrpID) > 1, True, False)
    '        grdOrderForm.Columns(1).HeaderStyle.CssClass = IIf(nrCustGrpVendor > 1, "th size32", "th size12") 'Vendor and Item #
    '        grdOrderForm.Columns(1).ItemStyle.CssClass = IIf(nrCustGrpVendor > 1, "size32", "size12 cellLbl") 'Vendor and Item #
    '        If Common.ApplyInEmailOrCustomer(DB, customerID, "HidePrice") = True Then
    '            grdOrderForm.Columns(5).Visible = False 'Price
    '            grdOrderForm.Columns(6).Visible = False '/cs
    '        End If

    '        'Show items
    '        'c.dispOrder ctgDispOrder, c.ctgName, 
    '        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
    '        SQL = "SELECT i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.dispOrder itemGrpDispOrder, g.itemGrpName,  " & _
    '             " i.item_id, " & IIf(IsSmallCustomer() = True, "cp.custItem", "i.itemNr") & " AS itemNr, i.CustItemDesc, v.custGrpVendor_id, i.minSalCnt, i.jfcRemark, " & _
    '             " v.vendorName, " & IIf(nrCustGrpVendor > 1, "v.vendorName", "NULL") & " AS vendorName_view, " & _
    '             " ci.eaPerCs, ci.csPrice, ci.eaPrice, ci.lbPrice_OrderByWgt, ci.lbPrice_OrderByQty, " & _
    '             " q.csQty AS lastCsQty, CASE WHEN NVL(q.pcQty, 0) > 0 THEN q.pcQty ELSE q.eaQty END AS lastEaQty, q.lbs AS lastLbs, " & _
    '             " TO_CHAR(q.shipDate, " & DB.SqlEsc(SHORTDATE_FMT) & ") AS shipDate_fmt, " & _
    '             " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " & _
    '             " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " & _
    '             " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
    '             " CASE WHEN " & IIf(IsSmallCustomer() = True, "cp.custItem", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(IsSmallCustomer() = True, "cp.custItem", "i.itemNr") & " || '<br />(' || i.item_id || ')' END AS itemIdNr_view, " & _
    '             " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL THEN a.itemDesc ELSE i.custItemDesc END AS itemDesc_view, " & _
    '             " jctgGrp.ctgGrpDesc AS ctgName_view, jctgGrp.dispOrder AS ctgDispOrder, " & _
    '             IIf(useCustomerCtg = True, " NVL(customCtg.dispOrder, 0)", " 0") & " AS itemDispOrder " & _
    '             "FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(IsSmallCustomer() = True, IIf(IsExcepPriceCustomer(), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
    '             "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
    '             IIf(useCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
    '                                         "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id ", "") & _
    '             IIf(useCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & DB.SqlEsc(customerID) & ", i.item_id, 'Y') " & _
    '                                        "INNER JOIN customer_ctg customCtg ON customCtg.ctgGrp_id = jctgGrp.ctgGrp_id AND customCtg.customer_id = " & DB.SqlEsc(customerID) & " AND customCtg.item_id = i.item_id ", "") & _
    '             "LEFT OUTER JOIN webUser_favoriteItem f ON f.custGrpItem_id = i.custGrpItem_id AND f.email = " & DB.SqlEsc(email) & " AND f.customer_id = " & DB.SqlEsc(customerID) & " " & _
    '             "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " & _
    '             "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " & _
    '             "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & DB.SqlEsc(customerID) & " " & _
    '             Session("PRINT_WHERECLAUSE") & " " & _
    '             "ORDER BY ctgDispOrder, ctgName_view, itemDispOrder, v.vendorName, itemDesc_view, itemGrpDispOrder"
    '        '" CASE WHEN i.jfcitem = 'Y' AND c.ctgName IS NULL THEN jctgGrp.ctgGrpDesc ELSE c.ctgName END ctgName_view " & _
    '        '"LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
    '        '"LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " & _
    '        '"LEFT OUTER JOIN custCompany_ctg c ON c.ctg_id = i.ctg_id " & _
    '        '"LEFT OUTER JOIN jfcCtg jctg ON jctg.ctg_id = a.ctg_id " & _
    '        '" CASE WHEN (SELECT DISTINCT(d.custGrpItem_id) FROM sodtl d WHERE d.custGrpItem_id = i.custGrpItem_id AND d.webOrder_id IN (SELECT h.webOrder_id FROM sohdr h WHERE h.createUser = " & DB.SqlEsc(email) & " AND h.customer_id = " & DB.SqlEsc(customerID) & ")) IS NOT NULL THEN 'ON' ELSE 'OFF' END ordered " & _
    '        'Session("PRINT_ORDERBYCLAUSE")
    '        dsOrderForm.ProviderName = "System.Data.OracleClient"
    '        dsOrderForm.ConnectionString = DB.GetConnectionString
    '        dsOrderForm.SelectCommand = SQL
    '        grdOrderForm.DataSourceID = "dsOrderForm"
    '        grdOrderForm.DataBind()
    '        'If Session("PRINT_ARG2") = "Category" Then MergeGrdCells(grdOrderForm, 0, True)
    '        MergeGrdCells(grdOrderForm, 0, True)
    '    End Using
    'End Sub
End Class


#End ExternalSource
