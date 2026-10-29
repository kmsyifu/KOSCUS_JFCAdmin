<%@ WebHandler Language="VB" Class="Search_VB" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text
Imports Common
Imports Newtonsoft.Json.Linq

Public Class Search_VB : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SQL, prefixtext, code, id, id2, id3, id4 As String
        Dim r As DataReader = Nothing
        Dim itemDtl As String
        Dim priceGrpID As String
        'Dim customerID As String = HttpContext.Current.Session("SYS_SESSION_CUSTID")

        prefixtext = context.Request.QueryString("q")
        code = CString(context.Request.QueryString("code")).ToUpper
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
                    '', ' || g.itemGrpName ||   And (cc.enabled IS NULL OR cc.enabled = 'Y')
                    ' "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.CUSTGRPITEM_ID AND cc.customer_id = " & DB.SqlEsc(id) & " " &
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
                Case "ITEM_JFCBRANCH"
                    'Branch-level item list (id = jfcBranchNr, id2 = price group of an exception form), used by JFCAdmin/OrderForm. Last word is the 5-char item_id.
                    priceGrpID = IIf(id2 <> "", id2, Common.GetPriceGrpIDByBranch(DB, id))
                    SQL = "SELECT '#' || a.item_id || ', ' || NVL(p.itemDesc_branch, a.itemDesc) || ', ' || a.item_id AS itemDtl " &
                          "FROM jets_price p " &
                          "INNER JOIN jfcItem a ON a.item_id = p.itemcode " &
                          "WHERE p.pricegpid = :1 " &
                          "  AND (a.item_id LIKE :2 OR UPPER(NVL(p.itemDesc_branch, a.itemDesc)) LIKE :3) " &
                          "ORDER BY itemDtl"

                    ' "  AND (p.enabled IS NULL OR p.enabled = 'Y') " & _
                    r = DB.GetReader(SQL, priceGrpID, prefixtext & "%", "%" & Replace(prefixtext.ToUpper, " ", "%") & "%")
                    While r.Read
                        itemDtl = CString(r("itemDtl"))
                        itemDtl = Replace(itemDtl, "<", "(")
                        itemDtl = Replace(itemDtl, ">", ")")
                        sb.Append(itemDtl).Append(Environment.NewLine)
                    End While
                    r.close()
                    context.Response.Write(sb.ToString)
                Case "JFCBRANCH"

                    SQL = "SELECT c.Name FROM  jho.customer@jho c " & _
                    " WHERE c.bid = :1 AND c.customerid = :2"
                    Dim Name = CString(DB.GetScalar(SQL, id, id2))
                    sb.Append(Name)
                    context.Response.Write(sb.ToString)
                Case "VAILDEMAIL"
                    Dim msg As String = ""
                    If Common.IsEmail(id) = False Then
                        msg = "Error! Your email address " & id & " is invalid"
                    End If
                    If id.Contains(",") Then
                        msg = "Error! Your email address cannot contain ','" 'Doesn't allow multiple email address separated by "," for auto signup

                    End If
                    SQL = "SELECT u.email FROM webuser_customer uc, customer c, webuser u, jfcVendor v " & _
                            "WHERE uc.email = u.email AND uc.customer_id = c.customer_id AND c.jfcBranchNr = v.jfcBranchNr AND v.jfcBranchNr = :1 AND c.jfcCustomerNr = :2 AND uc.email = :3"
                    If CString(DB.GetScalar(SQL, id2, id3, id)) <> String.Empty Then
                        msg = "Error! This customer already exists. "
                    End If

                    If IsProduction() = True OrElse IsProdBeta() = True Then
                        'TODO: Remove OrElse userEmail = "yfukushima@jfc.com" later
                        'Get sales rep email (to address) and branch email (cc address)
                        SQL = "SELECT s.email1 AS salesmanEmail, b.branchid, CASE WHEN b.branchid = '45' THEN '45' ELSE b.parent_branchid END AS parentBranchID " & _
                                "FROM jho.branch@jho b, jho.customer@jho c, jho.salesman@jho s " & _
                                "WHERE b.branchid = c.bid AND c.bid = s.bid AND c.cid = s.cid AND c.salesmanid = s.salesmanid AND b.branchid = :1 AND c.customerid = :2"
                        r = DB.GetReader(SQL, id2, id3)
                        'If r.Read() Then
                        Dim salesmanEmail = CString(r("salesmanEmail"))
                        'jfcBranchNr = CString(r("branchid"))
                        'End If

                        If Not (salesmanEmail.Contains(id) OrElse SUPPORT_EMAIL.Contains(id) OrElse id = "yfukushima@jfc.com") Then
                            msg = "Error! You are not assigned to be the sales rep of this account " & id2 & " - " & id3 & ".<br /> You only can add your customers' account."
                            'pnlPopWarningUser.Visible = True
                            'popDialog_User.Show()
                            'Exit Sub
                        End If
                    End If


                    SQL = "select email From signupRequest where email = :1 and jfcbranchnr =:2 and jfccustomernr =:3 and AUTOSIGNUPSYNCDATE is null and createdate > sysdate - 15/1440  "
                    Dim IsSignupEmail = CString(DB.GetScalar(SQL, id, id2, id3))
                    If IsSignupEmail <> String.Empty Then
                        msg = "Error! New customer " & id & " has been added.<br /> Please wait 15 minutes for data to be copied over."
                    End If
                    sb.Append(msg)
                    context.Response.Write(sb.ToString)
                Case "GETGP"
                    Dim msg As String = ""
                    SQL = SetPrice(id4, Val(id3), id2, id)
                    r = DB.GetReader(SQL)
                    If r.Read() Then
                        msg = Get_GP(Val(r("eaPerCs")), Val(r("cscost")), Val(r("lbPrice_OrderByQty")), Val(r("csPrice")))
                    End If

                    sb.Append(msg)
                    context.Response.Write(sb.ToString)
                Case "ITEM_BRANCH"
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
                    '', ' || g.itemGrpName ||   And (cc.enabled IS NULL OR cc.enabled = 'Y')
                    ' "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.CUSTGRPITEM_ID AND cc.customer_id = " & DB.SqlEsc(id) & " " &
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
                Case "HOMEIMGLIST"
                    Dim json As String
                    Using reader = New System.IO.StreamReader(context.Request.InputStream)
                        json = reader.ReadToEnd()
                    End Using
                    Dim items As JArray = New JArray
                    If json <> "" Then


                        Dim requestData = JObject.Parse(json)
                        ' Accessing the 'items' array
                        items = DirectCast(requestData("items"), JArray)
                    End If

                    Dim sortNumber As Integer = 1
                    DB.BeginTransaction()
                    For Each item As JObject In items

                        SQL = "UPDATE " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " SET  DISPORDER = " & sortNumber & "  WHERE HOMEPAGEIMG_ID = :1  and JFCBRANCHNR =:2 and IMGCTG = :3"
                        DB.ExecuteNonQuery(SQL, item("item_id").ToString, id, id2)
                        'DBUpdate.InsertCustItemDispOrder(DB, id2, item("item_id").ToString(), sortNumber)
                        sortNumber += 1
                        ' Process each itemId as needed
                    Next
                    DB.CommitTransaction()

                    context.Response.ContentType = "text/plain"
                    context.Response.Write("Data received successfully")

                Case "ITEMLIST"
                    Dim json As String
                    Using reader = New System.IO.StreamReader(context.Request.InputStream)
                        json = reader.ReadToEnd()
                    End Using
                    Dim items As JArray = New JArray
                    If json <> "" Then


                        Dim requestData = JObject.Parse(json)
                        ' Accessing the 'items' array
                        items = DirectCast(requestData("items"), JArray)
                    End If

                    Dim sortNumber As Integer = 1
                    DBUpdate.DeleteCustItemDispOrder(DB, id)
                    For Each item As JObject In items

                        DBUpdate.InsertCustItemDispOrder(DB, id, item("item_id").ToString(), sortNumber)
                        sortNumber += 1
                        ' Process each itemId as needed
                    Next


                    context.Response.ContentType = "text/plain"
                    context.Response.Write("Data received successfully")
                Case "BRANCHSETTING"

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