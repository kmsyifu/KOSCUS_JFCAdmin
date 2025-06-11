<%@ WebHandler Language="VB" Class="Display_VB" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text
Imports Common

Public Class Display_VB : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SQL, prefixtext, code, id, id2, id3, id4 As String
        Dim r As DataReader = Nothing
        Dim itemDtl As String
        Dim priceGrpID As String
        'Dim customerID As String = HttpContext.Current.Session("SYS_SESSION_CUSTID")

        prefixtext = context.Request.QueryString("q")
        code = context.Request.QueryString("code").ToUpper
        id = context.Request.QueryString("ID")
        id2 = context.Request.QueryString("ID2")
        id3 = context.Request.QueryString("ID3")
        id4 = context.Request.QueryString("ID4")

        Using DB As New DBHelper
            Dim sb As StringBuilder = New StringBuilder
            
            Select Case code
                Case "ITEM"
                    'Get JFC #
                    Dim jfcBranchNr As String = ""
                    Dim jfcCustomerNr As String = ""
                    Dim smallCustomer As Boolean = IsSmallCustomer(DB, id)
                    GetJFCBranchAndCustomerNr(DB, id, jfcBranchNr, jfcCustomerNr)
                    priceGrpID = Common.GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)
        
                    'Get item list
                    'SQL = "SELECT v.vendorName || ', Item#' || (CASE WHEN i.itemNr IS NULL THEN i.item_id ELSE i.itemNr END) " & _
                    SQL = "SELECT '#' || (CASE WHEN i.itemNr IS NULL THEN i.item_id ELSE i.itemNr END) " & _
                             " || ', ' || (CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END) " & _
                             " || ', ' || TRIM(LEADING '0' FROM i.custGrpItem_id) AS itemDtl, " & _
                             " i.custGrp_id, i.vendor_id, i.item_id, i.itemNr, g.itemGrpName " & _
                             "FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(smallCustomer = True, IIf(IsExcepPriceCustomer(DB, id), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
                             "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                             "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(priceGrpID) & " " &
                             "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = :1 AND cp.customerid = :2 " & _
                             "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                             "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                             "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                             "  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, id)) & _
                             "  AND ci.customer_id = :3" & _
                             IIf(id2 = "", "", " AND v.custGrpVendor_id = " & DB.SqlEsc(id2)) & _
                             IIf(id3 = "", "", " AND i.itemGrp_id = " & DB.SqlEsc(id3)) & _
                             IIf(id4 = "JFCADMIN", "", " AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) ") & _
                             "  AND (i.item_id LIKE :4 OR UPPER(i.itemNr) LIKE :5 OR UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE :6 OR UPPER(i.CustItemDesc) LIKE :7) " & _
                             "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                             "ORDER BY itemDtl"
                    '"  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
                    '"  AND (p.outOfStock IS NULL OR p.outOfStock = 'N') " & _
                    '"  ORDER BY v.vendorName, g.itemGrpName, i.itemNr, i.item_id"
                    '" || (CASE WHEN i.itemNr IS NOT NULL THEN ', Cust#' || i.itemNr END) || ', Web#' || i.item_id || ', ' " & _
                    '', ' || g.itemGrpName || 
                    r = DB.GetReader(SQL, jfcBranchNr, jfcCustomerNr, id, prefixtext & "%", prefixtext.ToUpper & "%", "%" & Replace(prefixtext.ToUpper, " ", "%") & "%", "%" & Replace(prefixtext.ToUpper, " ", "%") & "%")
                    While r.Read
                        itemDtl = CString(r("itemDtl"))
                        itemDtl = Replace(itemDtl, "<", "(")
                        itemDtl = Replace(itemDtl, ">", ")")
                        'If multiple order forms exist for a item, show order form name
                        SQL = "SELECT COUNT (*) FROM custGrp_item " & _
                                "WHERE custGrp_id = :1 AND vendor_id = :2" & _
                                "  AND (item_id = :3 OR itemNr = :4)"
                        If id3 = "" AndAlso Val(DB.GetScalar(SQL, CString(r("custGrp_id")), CString(r("vendor_id")), CString(r("item_id")), CString(r("itemNr")))) > 1 Then
                            'itemDtl = Replace(itemDtl, "Item#", CString(r("itemGrpName")) & ", Item#")
                            itemDtl = CString(r("itemGrpName")) & ", " & itemDtl
                        End If
                        sb.Append(itemDtl).Append(Environment.NewLine)
                    End While
                    r.close()
                    context.Response.Write(sb.ToString)
                Case Else
                    Exit Sub
            End Select
        End Using
    End Sub
    
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class