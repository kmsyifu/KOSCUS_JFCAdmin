#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\EmailBody.ascx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","C19C9FF35E7E4E06E8FCF1E2B308B27353E58E5D")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\EmailBody.ascx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate
Imports System.Resources
Imports System.Globalization
Imports System.Threading

Partial Class Common_EmailBody
    Inherits System.Web.UI.UserControl

    '--------------------------------------------------------------------------------------------------------------------
    'Parameters passed by caller
    'Session("EMAILBODY_CMD"): "ORDER", "PRICE_INQUIRY"
    'Session("EMAILBODY_ARG1")
    '--------------------------------------------------------------------------------------------------------------------
    Private rm As ResourceManager = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))
    Private ci As CultureInfo
    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load 'Doesn't get called from PlaceOrder.aspx.vb in Mobile
        'If IsPostBack = False Then
        'Init
        pnlOrder.Visible = False
        pnlPriceInquiry.Visible = False
        pnlPay.Visible = False
        pnlNewItem.Visible = False
        PnlUpdate.Visible = False
        PnlMaster.Visible = False
        PnlPriceInquireList.Visible = False
        DataBind() 'Get images and customer company type
        Dim rm = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))
        Dim ci As CultureInfo = Thread.CurrentThread.CurrentCulture
        'Enable tabs
        Select Case Session("EMAILBODY_CMD")
            Case "ORDER"
                pnlOrder.Visible = True
            Case "PRICE_INQUIRY"
                pnlPriceInquiry.Visible = True
            Case "PAY"
                pnlPay.Visible = True
            Case "NEW_ITEM"
                pnlNewItem.Visible = True
            Case "ITEM_DTL"
                PnlUpdate.Visible = True
            Case "MASTER"
                PnlMaster.Visible = True
            Case "PRICE_INQUIRY_LIST"
                PnlPriceInquireList.Visible = True
        End Select

        If pnlOrder.Visible = True Then
            showOrder()
        ElseIf pnlPriceInquiry.Visible = True Then
            showPriceInquiry()
        ElseIf pnlPay.Visible = True Then
            showPay()
        ElseIf pnlNewItem.Visible = True Then
            showNewItem()
        ElseIf PnlUpdate.Visible = True Then
            ShowItemDtl()
        ElseIf PnlMaster.Visible Then
            ShowMasterDtl()
        ElseIf PnlPriceInquireList.Visible Then
            showPriceInquirelist()
        End If
        'End If
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------
    'Order
    '---------------------------------------------------------------------------------------------------------------------------
    Protected Sub grdItem_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdItem.RowDataBound
        Dim lbl As Label
        Dim lblbottom As Label
        Dim lblprice As Label
        Try
            Dim emailType As String = Session("EMAILBODY_ARG2")

            'Do nothing if the email is not for vendor
            If emailType <> "VENDOR" Then Exit Sub

            'Highlight the mourse-over row
            If e.Row.RowType = DataControlRowType.DataRow Then
                lbl = e.Row.FindControl("lblOutOfStock")
                If lbl.Text = "Y" Then
                    'Highlight the row for outofstock item pink and change font color of ordered qty and available qty to red 
                    e.Row.BackColor = Drawing.Color.FromName("#FFE4E1")
                    lbl = e.Row.FindControl("lblCsQty")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblEaQty")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblLbs")
                    lbl = e.Row.FindControl("lblCs")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblEa")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblLb")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblCsAvail")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblEaAvail")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblCs2")
                    lbl.ForeColor = Drawing.Color.Red
                    lbl = e.Row.FindControl("lblEa2")
                    lbl.ForeColor = Drawing.Color.Red

                    'lblbottom = e.Row.FindControl("lblbtmCs")
                    'lblprice = e.Row.FindControl("lblCsPrice")
                    'If Val(lblprice.Text) < Val(lblbottom.Text) Then
                    '    e.Row.BackColor = Drawing.Color.Orange
                    'End If
                End If
            End If
        Catch ex As Exception
            Exit Sub
        End Try



    End Sub

    Private Sub showOrder()
        Dim SQL As String
        Dim r As DataReader
        Dim link As String
        Dim nrItems As Integer
        Dim webOrderID As String = Session("EMAILBODY_ARG1")
        Dim emailType As String = Session("EMAILBODY_ARG2")
        Dim action As String = Session("EMAILBODY_ARG3")
        Dim emailMsg As String = Session("EMAILBODY_ARG4")
        Dim tmp As String
        Dim vendorEmail As String
        Dim isJfcBranch As Boolean
        Dim hidePrice As Boolean
        Dim customerID As String
        Dim jfcBranchNr As String = ""
        Dim serverDayOffset As Decimal
        Dim jfcVendorTimeZone As String = ""

        Using DB As New DBHelper
            'Get customer info
            SQL = "SELECT customer_id FROM sohdr WHERE weborder_id = :1"
            customerID = CString(DB.GetScalar(SQL, webOrderID))
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, Nothing)
            serverDayOffset = GetServerDayOffset(DB, jfcBranchNr, jfcVendorTimeZone)

            'Show order header
            SQL = "SELECT h.weborder_id, h.orderstate, h.customer_id, h.custGrpVendor_id, v.vendorname, h.custordernr, h.message, h.totalamt, code.codeDesc, h.shipPoint_id, " & _
                    " cust.customerName, cust.custGrp_id, cust.jfcBranchNr, cust.jfcCustomerNr, cg.custGrpName, cg.custCompany_id, " & _
                    " e.onetimePassword, TO_CHAR(h.approveDate, :1) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " approveDate, " & _
                    " TO_CHAR(h.orderdate, :2) orderDate, " & _
                    " TO_CHAR(h.deliveryDate, :3) deliveryDate, " & _
                    " TO_CHAR(h.createDate + " & serverDayOffset & ", :4) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.createUser IS NOT NULL THEN ' by ' || h.createUser END createDateTimeUser, " & _
                    " h.createUser, TO_CHAR(h.createDate, :5) createDate, " & _
                    " TO_CHAR(h.modDate + " & serverDayOffset & ", :6) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.modUser IS NOT NULL THEN ' by ' || h.modUser END modDateUser, " & _
                    " h.modUser, TO_CHAR(h.modDate + " & serverDayOffset & ", :7) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " modDate " & _
                    "FROM so_email e, sohdr h " & _
                    "LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE' AND h.orderstate = code.code_id " & _
                    "LEFT OUTER JOIN customer cust ON cust.customer_id = h.customer_id " & _
                    "LEFT OUTER JOIN custGrp cg ON cg.custGrp_id = cust.custGrp_id " & _
                    "INNER JOIN custgrp_vendor v on h.custgrpvendor_id = v.custgrpvendor_id " & _
                    "WHERE h.webOrder_id = e.webOrder_id AND emailType = 'VENDOR' AND h.webOrder_id = :8"
            r = DB.GetReader(SQL, DATETIME_SQLFMT, DATE_FMT, DAYDATE_LONG_SQLFMT, DATETIME_SQLFMT, DATETIME_SQLFMT, DATE_FMT, DATETIME_SQLFMT, webOrderID)
            If r.Read() Then
                lblCustomerID.Text = CString(r("customer_id"))
                lblCustGrpVendorID.Text = CString(r("custGrpVendor_id"))
                lblWebOrderID.Text = CString(r("weborder_id"))
                lblOrderState.Text = IIf(action = "DELETED", rm.GetString("Canceled", ci), Common.Translate(CString(r("codeDesc"))))
                'IIf(action = "DELETED", "Canceled", CString(r("codeDesc")))
                'lblOrderDate.Text = CString(r("orderDate"))
                lblVendorName.Text = CString(r("vendorName"))
                lblShipPointID.Text = CString(r("shipPoint_id"))
                lblDeliveryDate.Text = DateTranslate(CString(r("deliveryDate")))
                'CString(r("deliveryDate"))
                lblCreateDateTimeUser.Text = CString(r("createDateTimeUser"))
                lblModDateUser.Text = CString(r("modDate")) 'CString(r("modDateUser"))
                lblCustGrpName.Text = CString(r("custGrpName"))
                lblCustomerName.Text = CString(r("customerName"))
                lblJfcBranchNr.Text = CString(r("jfcBranchNr"))
                isJfcBranch = Common.IsJfcBranch(DB, lblCustGrpVendorID.Text)
                lblJfcCustomerNr.Text = CString(r("jfcCustomerNr"))
                lblCustOrderNr.Text = CString(r("custOrderNr"))
                lblMessage.Text = CString(r("message"))
                nrItems = Val(DB.GetScalar("SELECT COUNT(*) FROM sodtl WHERE webOrder_id = " & DB.SqlEsc(webOrderID)))
                'lblNrItem.Text = nrItems & " items"
                lblTotalAmt.Text = String.Format("{0:c}", Val(r("totalAmt")))
                'lblOrderMsg.Text = nrItems & " items exist in this order. Total amount is <span style='font-weight: bold; color: #FF0000;'>" & lblTotalAmt.Text & "</span>"

                lblOrderMsg.Text = rm.GetString("TotalAmount", ci) & " (" & nrItems & " " & rm.GetString("Items", ci) &
                    "): <span class='red' style='font-weight: bold; color: #FF0000;'>" & lblTotalAmt.Text & "</span>"

                '"Total amount (" & nrItems & " items): <span class='red' style='font-weight: bold; color: #FF0000;'>" & lblTotalAmt.Text & "</span>"

                'Set properties
                pnlJfcLogo_order.Visible = True
                'lblMsg_order.Visible = True
                pnlVendorName.Visible = IIf(GetNrCustGrpVendor(DB, CString(r("custGrp_id"))) > 1, True, False)
                pnlCustGrpName.Visible = IIf(GetNrCustGrp(DB, CString(r("custCompany_id"))) > 1, True, False)
                pnlCustShipPoint.Visible = IIf(GetNrCustShipPoint(DB, CString(r("customer_id"))) > 1, True, False)

                'Show order message
                Select Case action
                    Case "CREATED", "RESEND"
                        If emailType = "CUSTOMER" Then
                            lblMsg_order.Text = "Thank you for placing an order. Your order has been sent to your sales rep. "
                        Else
                            lblMsg_order.Text = "An order has been created by " & CString(r("createUser")) & ". "
                        End If
                    Case "UPDATED"
                        If emailType = "CUSTOMER" Then
                            lblMsg_order.Text = "Your order has been updated. "
                        Else
                            lblMsg_order.Text = "An order has been updated by " & CString(r("modUser")) & ". " '" at " & CString(r("modDate")) & 
                        End If
                    Case "DELETED"
                        If emailType = "CUSTOMER" Then
                            lblMsg_order.Text = rm.GetString("Yourorderhasbeencanceled", ci) '"Your order has been canceled. "
                        Else
                            lblMsg_order.Text = rm.GetString("Anorderhasbeencanceled", ci)  '"An order has been canceled. " 'by " & CString(r("createUser")) & ". " '" at " & CString(r("createDate")) & 
                        End If
                    Case "APPROVED_ORDER"
                        'CString(r("approveDate"))
                        lblMsg_order.Text = rm.GetString("Yourorderhasbeenprocessedat", ci) & " "
                        '"Your order has been processed at " & CString(r("approveDate")) & ". "
                    Case "APPROVED_CANCELATION"
                        lblMsg_order.Text = rm.GetString("Yourorderhasbeencanceled", ci) '"Your order has been canceled. "
                End Select
                If emailMsg <> String.Empty Then lblMsg_order.Text &= "<br /><br />Message: <span style='font-weight: bold'>" & emailMsg & "</span>"
                If action <> "APPROVED_ORDER" AndAlso action <> "APPROVED_CANCELATION" Then
                    If emailType = "CUSTOMER" Then
                        'If action <> "DELETED" Then lblMsg_order.Text &= "If you need to make changes or check the status of your order, please visit our website at shop.jfc.com."
                    ElseIf emailType = "VENDOR" Then
                        If isJfcBranch = False Then
                            'Non JFC vendor
                            If action = "CREATED" Then
                                tmp = "order"
                            ElseIf action = "DELETED" Then
                                tmp = "cancelation"
                            Else
                                tmp = "update"
                            End If
                            lblMsg_order.Text &= IIf(emailMsg <> String.Empty, "<br /><br />", "") & "Please click the following link to confirm that you have approved this " & tmp & "."
                            link = "http://" & Request.Url.Host & IIf(Request.Url.Port = 80 OrElse Request.Url.Port = 443, "", ":" & Request.Url.Port) & _
                                      Request.ApplicationPath & "/Login/Login.aspx?cmd=confirm_order&id=" & HttpUtility.UrlEncode(CString(r("weborder_id"))) & "&password=" & HttpUtility.UrlEncode(CString(r("onetimePassword")))
                            lblLink_order.Text = "<a href='" & link & "'>" & link & "</a><br /><br />"
                            lblLink2_order.Text = lblLink_order.Text
                        Else
                            'JFC vendor
                            pnlJfcLogo_order.Visible = False
                            'lblMsg_order.Visible = False
                            If action = "CREATED" OrElse action = "RESEND" OrElse action = "UPDATED" Then
                                vendorEmail = GetOrderEmail(DB, webOrderID, "VENDOR")
                                'If vendorEmail.Contains(",") = False Then
                                'Show link to the order history page
                                link = "http://" & Request.Url.Host & IIf(Request.Url.Port = 80 OrElse Request.Url.Port = 443, "", ":" & Request.Url.Port) & _
                                   Request.ApplicationPath & "/Login/Login.aspx?auto_login=Y&type=JFCADMIN&page=OrderHistory.aspx?webOrderID=" & webOrderID
                                'Request.ApplicationPath & "/Login/Login.aspx?hashed=Y&type=JFCADMIN&page=OrderHistory.aspx&username=" & vendorEmail & "&password=" & GetUserPasswd(DB, vendorEmail)
                                lblLink_order.Text = "<a href='" & link & "' style='font-size: 1.1em; font-weight: bold'>Click here to view details and approve this order</a><br /><br />"
                                lblLink2_order.Text = lblLink_order.Text
                                'End If
                            End If
                        End If
                    End If
                End If
            End If
            r.close()

            'Get info
            hidePrice = Common.ApplyInEmailOrCustomer(DB, lblCustomerID.Text, "HidePrice")

            'Show order form if the company doesn't allow mixed order form
            InvisibleCtrl({lblTitleItemGrp, lblItemGrpName})
            If AllowMixItemGrp(DB, GetOrderCustCompanyID(DB, webOrderID)) = False Then
                VisibleCtrl({lblTitleItemGrp, lblItemGrpName})
                lblItemGrpName.Text = GetItemGrpName(DB, GetItemGrpIDInOrder(DB, webOrderID))
            End If

            'Show shipping address
            SQL = "SELECT shipPoint_id, shipName, address1, address2, city, state, zip, country " & _
                    "FROM customer_shipPoint " & _
                    "WHERE customer_id = :1" & _
                    " AND shipPoint_id = :2"
            r = DB.GetReader(SQL, lblCustomerID.Text, lblShipPointID.Text)
            If r.Read() Then
                lblShipName.Text = CString(r("shipName"))
                lblAddress1.Text = CString(r("address1"))
                lblAddress2.Text = CString(r("address2"))
                lblCity.Text = CString(r("city"))
                lblState.Text = CString(r("state"))
                lblZip.Text = CString(r("zip"))
                'lblCountry.Text = CString(r("country"))
            End If
            r.close()

            'Hide some properties if it's sent to customer (not JFC)
            grdItem.Columns(8).Visible = False 'Out of stock
            If Not (emailType = "VENDOR" AndAlso isJfcBranch = True) Then
                grdItem.Columns(6).Visible = False 'Available cs and ea
                grdItem.Columns(7).Visible = False 'cs and ea
                If hidePrice = True Then
                    pnlShowPrice.Visible = False
                    grdItem.Columns(2).Visible = False 'Price
                    grdItem.Columns(5).Visible = False 'Amount
                    lblOrderMsg.Text = "Total: " & nrItems & " items"
                End If
            End If
            Dim SQL_eabtm = "(CASE WHEN d.eaPerCs <= 1 THEN 0 ELSE " & _
                               "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN d.eapercs = 10 then round(NVL(p.btmcsprice, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.btmcsprice, 0)/d.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                               " ELSE (CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.btmcsprice, 0) / d.eaPerCs, 2) END) END) " & _
                            "END)"

            Dim SQL_lbbtm = "(CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.btmcsprice, 0) / d.eaPerCs * 10, 2) END)"

            'Show item list from sodtl (remainingInStockQty = InStockQty - 1 for scaled items)
            SQL = "SELECT d.webLineNr, d.custGrpItem_id, g.itemGrpName, d.item_id, d.itemNr, d.custItemDesc, " & _
                    " d.csQty, d.csQty_vp, d.csQty_sp, NVL(d.csQty_vp, 0) + NVL(d.csQty_sp, 0) AS csQty_vpsp, d.eaQty, d.eaQty_vp, d.eaQty_sp, NVL(d.eaQty_vp, 0) + NVL(d.eaQty_sp, 0) AS eaQty_vpsp, d.vpMemo, d.Lbs, " & _
                    " d.eaPerCs, d.pointPerCs, d.csPrice, d.eaPrice, d.lbPrice_orderByWgt, d.lbPrice_orderByQty, d.lineAmt,  " & _
                    " CASE WHEN i.jfcItem = 'Y' THEN " & _
                    "   CASE WHEN d.lbPrice_orderByQty > 0 THEN " & _
                    "     (NVL(p.csAvail, 0) * NVL(d.eaPerCs, 1) + NVL(p.eaAvail, 0)) - 1" & _
                    "   ELSE " & _
                    "     (NVL(p.csAvail, 0) * NVL(d.eaPerCs, 1) + NVL(p.eaAvail, 0)) - (d.csQty * NVL(d.eaPerCs, 1) + d.eaQty) " & _
                    "   END " & _
                    " ELSE 0 END AS remainingInStockQty, " & _
                    "NVL(p.btmcsprice, 0) as btmcsprice, " & _
                    SQL_eabtm & " as btmeaprice, " & _
                    SQL_lbbtm & " as btmlbprice, " & _
                    " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                    " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view, " & _
                    " CASE WHEN d.itemNr IS NULL THEN d.item_id ELSE d.itemNr END AS itemIdNr_view, " & _
                    " CASE WHEN d.custItemDesc IS NULL THEN d.itemDesc ELSE d.custItemDesc END AS itemDesc_view " & _
                    "FROM custGrp_itemGrp g, " & IIf(IsSmallCustomer(DB, lblCustomerID.Text) = True, "v_custgrp_item_small", "custgrp_item") & " i, sodtl d " & _
                    "LEFT OUTER JOIN jets_price p ON p.itemcode = d.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, lblJfcBranchNr.Text, lblJfcCustomerNr.Text)) & " " & _
                    "WHERE d.itemGrp_id = g.itemGrp_id AND d.custGrpItem_id = i.custGrpItem_id " & _
                    "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, lblCustomerID.Text)) & _
                    "  AND d.webOrder_id = " & DB.SqlEsc(webOrderID) & " " & _
                    "ORDER BY d.webLineNr"
            '"  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, lblCustomerID.Text)) & _ 'is added to improve the performance
            '"LEFT OUTER JOIN jets_customer_price cp ON cp.itemcode = d.item_id AND cp.bid = " & DB.SqlEsc(lblJfcBranchNr.Text) & " AND cp.customerid = " & DB.SqlEsc(lblJfcCustomerNr.Text) & " " & _
            dsItem.ProviderName = "System.Data.OracleClient"
            dsItem.ConnectionString = DB.GetConnectionString
            dsItem.SelectCommand = SQL
            grdItem.DataSourceID = "dsItem"
            grdItem.DataBind()
        End Using
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------
    'Price Inquiry
    '---------------------------------------------------------------------------------------------------------------------------
    Private Sub showPriceInquiry()
        Dim SQL As String
        Dim r As DataReader
        Dim email As String = Session("EMAILBODY_ARG1")
        Dim customerID As String = Session("EMAILBODY_ARG2")
        Dim emailType As String = Session("EMAILBODY_ARG3")
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""

        Using DB As New DBHelper
            'Get info
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)

            If emailType = "CUSTOMER" Then
                lblMsg_priceInquiry.Text = "Thank you for submitting the price inquiry. Your sales rep will contact you as soon as this is received."
            Else
                SQL = "SELECT * FROM customer where customer_id = :1"
                r = DB.GetReader(SQL, customerID)
                If r.Read() Then
                    lblMsg_priceInquiry.Text = "Customer: " & CString(r("customerName")) & " (" & GetJfcBranchCustNr(DB, customerID) & ")" & _
                                "<br />Phone: " & CString(r("phone")) & _
                                "<br />Fax: " & CString(r("fax")) & _
                                "<br /><br />The following price inquiry was submitted by " & email & " from " & CString(r("customerName")) & ". Please contact the customer as soon as possible."
                End If
                r.close()
            End If

            'Set properties
            If SMALL_CUST_ONLY = True Then
                grdPriceInquiry.Columns(GetColumnIndex(grdPriceInquiry, "itemNr")).Visible = False
                grdPriceInquiry.Columns(GetColumnIndex(grdPriceInquiry, "custItemDesc")).Visible = False
            End If

            'Only show available cs and ea if it's sent to JFC
            If emailType = "CUSTOMER" Then
                grdPriceInquiry.Columns(7).Visible = False 'Available cs and ea
                grdPriceInquiry.Columns(8).Visible = False 'cs and ea
                lbllink_priceInquiry.Visible = False

            End If

            'Show price inquiry list
            SQL = "SELECT p.custGrpItem_id, v.custGrpVendor_id, v.vendorName, i.item_id, i.itemNr,  NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc, i.custItemDesc, '$[________]' AS blank, p.csAvail, p.eaAvail, " & _
                        " CASE WHEN i.itemNr IS NULL THEN i.item_id ELSE i.itemNr END AS itemIdNr_view, " & _
                        " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL THEN  NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view " & _
                        "FROM webuser_priceInquiry p, custGrp_vendor v, " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " i " & _
                        "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                        "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " & _
                        "WHERE p.custGrpItem_id = i.custGrpItem_id " & _
                        "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id" & _
                        "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _
                        "  AND p.sent = 'N' AND p.email = " & DB.SqlEsc(email) & " AND p.customer_id = " & DB.SqlEsc(customerID) & " " & _
                        "ORDER BY v.vendorName, itemIdNr_view"
            '"  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
            '"LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " & _
            dsPriceInquiry.ProviderName = "System.Data.OracleClient"
            dsPriceInquiry.ConnectionString = DB.GetConnectionString
            dsPriceInquiry.SelectCommand = SQL
            grdPriceInquiry.DataSourceID = "dsPriceInquiry"
            grdPriceInquiry.DataBind()

            'vendorEmail = GetOrderEmail(DB, webOrderID, "VENDOR")
            ''If vendorEmail.Contains(",") = False Then
            ''Show link to the order history page
            Dim link = "http://" & Request.Url.Host & IIf(Request.Url.Port = 80 OrElse Request.Url.Port = 443, "", ":" & Request.Url.Port) & _
              Request.ApplicationPath & "/Login/Login.aspx?auto_login=Y&type=JFCADMIN&page=CustItem.aspx?cust_company=" & jfcBranchNr & jfcCustomerNr & "&tab=0"

            lbllink_priceInquiry.Text = "<a href='" & link & "' style='font-size: 1.1em; font-weight: bold'>Click here to view details of the price inquiry</a><br /><br />"
            'lblLink2_order.Text = lblLink_order.Text
        End Using
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------
    'Payment
    '---------------------------------------------------------------------------------------------------------------------------
    Private Sub showPay()
        Dim SQL As String
        Dim SQL_pendingAmt, SQL_webDiscAmt As String
        Dim dt As DataTable
        Dim paymentID As String = Session("EMAILBODY_ARG1")
        Dim sendToType As String = Session("EMAILBODY_ARG3")
        Dim customerID As String
        Dim webDiscType As String

        Using DB As New DBHelper
            'Get msg
            lblMsg_pay.Text = Session("EMAILBODY_ARG2")
            'Get customer info
            SQL = "SELECT customer_id FROM customer_payHdr WHERE payment_id = :1"
            customerID = CString(DB.GetScalar(SQL, paymentID))
            webDiscType = GetCustWebDiscType(DB, customerID)

            'Set properties
            'grdPayDtl.Columns(GetColumnIndex(grdPayDtl, "note")).Visible = AllowCustInputPayAmt(DB, customerID)
            grdPayDtl.Columns(4).Visible = IIf(webDiscType = String.Empty OrElse webDiscType = "Z", False, True) 'Web Discount

            'Show payment header
            SQL = "SELECT p.*, c.customerName, c.jfcBranchNr, jfcCustomerNr, code2.codeDesc AS status " & _
                    " , CASE WHEN paymentAmt <> approvedAmt THEN '($' || approvedAmt || ' approved)' ELSE '' END AS approvedAmtDesc " & _
                    " , CASE WHEN p.paymentStatus_id IN ('E', 'B') THEN code.codeDesc ELSE '' END AS result " & _
                    " , TO_CHAR(p.createDate, :1) || CASE WHEN p.createUser IS NOT NULL THEN ' by ' || p.createUser END createDateTimeUser " & _
                    "FROM customer c, customer_payHdr p " & _
                    "LEFT OUTER JOIN codemaster code ON code.codegrp = 'FORTE_SETTLERESULT' AND p.forteSettleResultCode = code.code_id " & _
                    "LEFT OUTER JOIN codemaster code2 ON code2.codegrp = 'PAYMENTSTATUS' AND p.paymentStatus_id = code2.code_id " & _
                    "WHERE p.customer_id = c.customer_id AND p.payment_id = :2"
            dt = DB.GetDataTable(SQL, DATETIME_SQLFMT, paymentID)
            frmPayHdr.DataSource = dt
            frmPayHdr.DataBind()

            'Show payment detail
            SQL = "SELECT b.*, " & _
                    " CASE WHEN b.arType = 'SERVICE CHARGE' AND b.sectionCode = '5' THEN 'Interest' WHEN b.sectionCode = '5' THEN 'Liquor' ELSE '' END isLiquor, " & _
                    " CASE WHEN b.arType = 'SERVICE CHARGE' AND b.sectionCode = '5' THEN 'LIQUOR INTEREST' ELSE b.arType END arType_view " & _
                    "FROM customer_payHdr a, customer_payDtl b " & _
                    "WHERE a.payment_id = b.payment_id AND a.payment_id = " & DB.SqlEsc(paymentID)
            '" CASE WHEN b.sectionCode = '5' THEN 'Yes' ELSE '' END isLiquor " & _
            dsPayDtl.ProviderName = "System.Data.OracleClient"
            dsPayDtl.ConnectionString = DB.GetConnectionString
            dsPayDtl.SelectCommand = SQL
            grdPayDtl.DataSourceID = "dsPayDtl"
            grdPayDtl.Sort("arDate, arNumber", SortDirection.Ascending)
            grdPayDtl.DataBind()

            If sendToType = "CUSTOMER" Then
                pnlBalanceDtl.Visible = False
            Else
                'Show balance detail only for JFC Salesperson
                SQL_pendingAmt = "SELECT NVL(SUM(pd.paymentLineAmt), 0) FROM customer_payhdr ph, customer_paydtl pd WHERE ph.payment_id = pd.payment_id AND ph.paymentStatus_id IN ('P', 'F') AND ph.customer_id = a.customer_id AND pd.arNumber = a.arNumber"
                SQL_webDiscAmt = "SELECT NVL(SUM(NVL(pd.webDiscAmt, 0)), 0) FROM customer_payhdr ph, customer_paydtl pd WHERE ph.payment_id = pd.payment_id AND ph.paymentStatus_id IN ('P', 'F') AND ph.customer_id = a.customer_id AND pd.arNumber = a.arNumber"
                SQL = "SELECT a.*, (" & SQL_pendingAmt & ") AS pendingAmt, " & _
                        " CASE WHEN a.arType = 'SERVICE CHARGE' AND a.sectionCode = '5' THEN 'Interest' WHEN a.sectionCode = '5' THEN 'Liquor' ELSE '' END isLiquor, " & _
                        " CASE WHEN a.arType = 'SERVICE CHARGE' AND a.sectionCode = '5' THEN 'LIQUOR INTEREST' ELSE a.arType END arType_view, " & _
                        " a.recvAmt + a.discAmt AS recvAndDiscAmt, a.arAmt - a.recvAmt - a.discAmt - (" & SQL_pendingAmt & ") AS balance " & _
                        "FROM customer c, customer_jfcAR a " & _
                        "LEFT OUTER JOIN customer_document d ON a.reference = d.docName AND UPPER(d.docType) = 'INVOICE' AND a.customer_id = d.customer_id " & _
                        "WHERE a.customer_id = c.customer_id AND a.customer_id = " & DB.SqlEsc(customerID) & _
                        "  AND a.arAmt - a.recvAmt - a.discAmt - (" & SQL_pendingAmt & ") - (" & SQL_webDiscAmt & ") <> 0"
                '" CASE WHEN sectionCode = '5' THEN 'Y' ELSE '' END isLiquor, " & _
                dsBalanceDtl.ProviderName = "System.Data.OracleClient"
                dsBalanceDtl.ConnectionString = DB.GetConnectionString
                dsBalanceDtl.SelectCommand = SQL
                grdBalanceDtl.DataSourceID = "dsBalanceDtl"
                grdBalanceDtl.Sort("arDate, arNumber", SortDirection.Ascending)
                grdBalanceDtl.DataBind()
                If grdBalanceDtl.Rows.Count = 0 Then pnlBalanceDtl.Visible = False
            End If
        End Using
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------
    'New Item
    '---------------------------------------------------------------------------------------------------------------------------
    Private Sub showNewItem()
        Dim index As Integer = 0
        Dim SQL As String
        Dim customerID As String = Session("EMAILBODY_ARG1")
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim r As DataReader
        Dim useCustomerCtg As Boolean

        Using DB As New DBHelper
            Dim smallCustomer As Boolean = IsSmallCustomer(DB, customerID)
            Dim hidePrice As Boolean = Common.ApplyInEmailOrCustomer(DB, customerID, "HidePrice")

            'Get JFC #
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)

            'Show new items
            useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            'Mod from NVL(p.newItemDate, NVL(p.openAcctDate_branch, a.openAcctDate)) to p.newItemDate by jzhou on 6/11/2025
            SQL = "SELECT * FROM (" &
                    "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY p.newItemDate DESC) As no, " &
                    " i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.itemGrpName, i.item_id, i.CustItemDesc, v.vendorName, " &
                    " " & IIf(hidePrice = True, "0 AS csPrice, 0 AS eaPrice, 0 AS lbPrice_OrderByWgt, 0 AS lbPrice_OrderByQty", "ci.csPrice, ci.eaPrice, ci.lbPrice_OrderByWgt, ci.lbPrice_OrderByQty") & ", " &
                    " ci.eaPerCs, p.outOfStock, TO_CHAR(p.arrivDate, " & DB.SqlEsc(SHORTDATE_FMT) & ") AS arrivDate_fmt, TRIM(i.jfcRemark || ' ' || cc.custRemark) AS jfcRemark, " &
                    " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " &
                    " CASE WHEN " & IIf(smallCustomer = True, "cc.custItemCode", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(smallCustomer = True, "cc.custItemCode", "i.itemNr") & " END AS itemIdNr_view, " &
                    " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL THEN SUBSTR(NVL(p.itemDesc_branch, a.itemDesc), 1, 48) ELSE SUBSTR(i.custItemDesc, 1, 48) END AS itemDesc_view, " &
                    " jctgGrp.ctgGrpDesc AS ctgName_view, jctgGrp.ctgGrpDesc AS jfcCtgName, " &
                    " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " &
                    " CASE WHEN ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0 THEN 'Y' ELSE 'N' END askPrice " &
                    "FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(smallCustomer = True, IIf(IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " &
                    "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " &
                    IIf(useCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " &
                                                "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id ", "") &
                    IIf(useCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & DB.SqlEsc(customerID) & ", i.item_id, 'N') ", "") &
                    "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " &
                    "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " &
                    "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & DB.SqlEsc(customerID) & " " &
                    "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.custGrpItem_id AND cc.customer_id = " & DB.SqlEsc(customerID) & " " &
                    "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " &
                    "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " &
                    "  AND i.custGrpItem_id = ci.custGrpItem_id " &
                    "  AND g.custgrp_id = " & DB.SqlEsc(Common.GetCustGrpID(DB, customerID)) &
                    "  AND ci.customer_id = " & DB.SqlEsc(customerID) &
                    "  AND i.jfcitem = 'Y' " &
                    "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " &
                    "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') " &
                    ") z WHERE no <= " & NR_NEW_ITMES &
                  ") ORDER BY ctgName_view, vendorName, itemDesc_view, itemGrpName"
            'Copy image to a folder under Login
            r = DB.GetReader(SQL)
            While r.Read()
                Common.CopyNewItemImg(CString(r("itemType")), CString(r("item_id")))
            End While
            r.close()

            'Show new items
            dsNewItem.ProviderName = "System.Data.OracleClient"
            dsNewItem.ConnectionString = DB.GetConnectionString
            dsNewItem.SelectCommand = SQL
            grdNewItem.DataSourceID = "dsNewItem"
            grdNewItem.DataBind()

        End Using
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------
    'Update Item
    '---------------------------------------------------------------------------------------------------------------------------
    Private Sub ShowItemDtl()

        Dim customerID As String = Session("EMAILBODY_ARG1")
        Dim custGrpID As String = Session("EMAILBODY_ARG3")
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim useCustomerCtg As Boolean
        Dim nrCustGrpVendor As Integer
        Dim isSmallCust As Boolean
        Dim GroupItemID As String = Session("EMAILBODY_ARG4")
        Dim customerEmail As String = Session("EMAILBODY_ARG2")
        Dim msg As String = Session("EMAILBODY_ARG5")
        Dim NoteMsg As String = Session("EMAILBODY_ARG6")
        Using DB As New DBHelper
            'Get msg

            'Get customer info
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            isSmallCust = Common.IsSmallCustomer(DB, customerID)
            useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            Dim SQL_Select = "SELECT /*+rule*/ " & _
                     " v.vendorName, " & IIf(nrCustGrpVendor > 1, "v.vendorName", "NULL") & " AS vendorName_view, " & _
                     " jctgGrp.ctgGrpDesc AS ctgName_view, jctgGrp.dispOrder AS ctgDispOrder, " & _
                     IIf(useCustomerCtg, " NVL(customCtg.dispOrder, 0)", " 0") & " AS itemDispOrder, " & _
                     " q.shipDate, i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.itemGrpName," & _
                     " g.dispOrder itemGrpDispOrder, " & _
                     IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " AS itemNr, i.CustItemDesc, v.custGrpVendor_id," & _
                     " i.minSalCnt,  cp.webCustRemark, " & _
                     " ci.eaPerCs, p.scaledItem, " & _
                     " q.csQty AS lastCsQty, CASE WHEN NVL(q.pcQty, 0) > 0 THEN q.pcQty ELSE q.eaQty END AS lastEaQty, q.lbs AS lastLbs," & _
                     " CASE WHEN q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0 THEN 'Y' ELSE 'N' END ordered, " & _
                     " CASE WHEN NVL(q.csQty, 0) > 0 OR NVL(q.eaQty, 0) > 0 OR NVL(q.lbs, 0) > 0 THEN 'Y' ELSE 'N' END AS lastQtyExist, " & _
                     " TO_CHAR(q.shipDate, " & DB.SqlEsc(SHORTDATE_FMT) & ") AS shipDate_fmt, cc.ENABLED," & _
                     " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
                     " CASE WHEN " & IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(isSmallCust = True, "cp.custItem", "i.itemNr") & " || '<br />(' || i.item_id || ')' END AS itemIdNr_view, " & _
                     " CASE WHEN ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0 THEN 'Y' ELSE 'N' END askPrice, " & _
                     IIf(Common.ShowItemDtl(DB, customerID) = True, " TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2), 99999.99) || ' kg, ' || TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3), 99999.999) || ' m³'", "NULL") & " AS itemDtl, "

            Dim sql_from = " FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(isSmallCust, IIf(IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
           "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
           IIf(useCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
                                       "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id ", "") & _
           IIf(useCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & DB.SqlEsc(customerID) & ", i.item_id, 'Y') " & _
                                      "INNER JOIN customer_ctg customCtg ON customCtg.ctgGrp_id = jctgGrp.ctgGrp_id AND customCtg.customer_id = " & DB.SqlEsc(customerID) & " AND customCtg.item_id = i.item_id ", "") & _
           "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " & _
           "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " & _
           "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & DB.SqlEsc(customerID) & " " & _
           "LEFT OUTER JOIN customer_custItem cc ON cc.customer_id = " & DB.SqlEsc(customerID) & " and i.custGrpItem_id = cc.CUSTGRPITEM_ID  "
            Dim SQL_eaCost = "(CASE WHEN ci.eaPerCs <= 1 THEN 0 ELSE " & _
                                  "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN ci.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/ci.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                                  " ELSE (CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs, 2) END) END) " & _
                               "END)"
            Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"

            Dim whereClause = "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                          "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                          "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                          "  AND g.custgrp_id = " & DB.SqlEsc(custGrpID) & _
                          "  AND ci.customer_id = " & DB.SqlEsc(customerID) & " " & _
                          "  AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) " & _
                          "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                          "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') "
            SQL_Select &= " NVL(p.csCost, 0) AS csCost, " & _
                            " " & SQL_eaCost & " AS eaCost, " & _
                            " " & SQL_lbCost & " AS lbCost, " & _
                              " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view," & _
                " CASE WHEN ci.lbPrice_orderByQty <> 0 THEN ROUND((ci.lbPrice_orderByQty - " & SQL_lbCost & ") / ci.lbPrice_orderByQty, 2) " & _
                " ELSE (CASE WHEN NVL(ci.csPrice, 0) = 0 THEN 1 ELSE ROUND((ci.csPrice - NVL(p.csCost, 0)) / ci.csPrice, 2) END) END gp," & _
                " cc.CUSTITEMCODE as item_id,i.item_id as item_id_o, cc.CUSTREMARK as jfcRemark,cc.CUSTCSPRICE as csPrice, " & _
                " ci.csPrice as csPrice_o, cc.CUSTEAPRICE as eaPrice, ci.eaPrice as eaPrice_o," & _
                " cc.CUSTLBPRICE_ORDERBYWGT as lbPrice_OrderByWgt,ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt_o,  cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty,ci.lbPrice_OrderByQty as lbPrice_OrderByQty_o," & _
                " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
            '" CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(" & IIf(IsMobile(), "a.itemDesc_short", "a.itemDesc") & ", a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
            whereClause &= "AND i.custGrpItem_id = '" & GroupItemID & "'"

            'Common.ApplyInEmailOrCustomer(DB, customerID, "HidePrice", customerEmail)


            Dim r = DB.GetReader(SQL_Select & sql_from & whereClause)
            If r.Read() Then
                If msg = String.Empty Then


                    lblMsg_Item.Text = rm.GetString("Youriteminfohasbeenaddedupdated", ci)  '"Your item info has been added / updated."
                    lblItemPriceNew.Text = Displayformat(Val(r("lbPrice_OrderByQty")), Val(r("csPrice")), Val(r("eaPrice")), DB.NVL(r("scaledItem"), "N") = "Y")
                    Dim hidePrice = Common.ApplyInEmailOrCustomer(DB, lblCustomerID.Text, "HidePrice", customerEmail)
                    If hidePrice Then
                        pnlItemPriceNew.Visible = False
                        'pnlItemPriceOriginal.Visible = False
                    End If
                Else
                    '"A item has been added. <br /> <br />  <span style='font-weight: bold; color: #FF0000;'>" & msg & "</span>"
                    'rm.GetString("Itemshavebeenadded", ci) &
                    lblMsg_Item.Text = "<br />  <span style='font-weight: bold; color: #FF0000;'>" & msg & "</span>"
                    lblItemPriceNew.Text = Displayformat(Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")), Val(r("eaPrice_o")), DB.NVL(r("scaledItem"), "N") = "Y")
                End If

                If Not String.IsNullOrWhiteSpace(NoteMsg) Then
                    lblMsg_Item.Text &= "<br /> <br /> " & rm.GetString("Message", ci) & ": <span style='font-weight: bold; background-color: #ffffad;' >" & NoteMsg & "</span>"
                End If

                lblItemCode.Text = CString(r("item_id_o"))
                lblItemCodeNew.Text = CString(r("item_id"))
                lblItemDesc.Text = CString(r("itemDesc_view"))
                lblItemDescNew.Text = CString(r("custItem_Desc"))
                lblItemRemark.Text = CString(r("jfcRemark"))
                lblItemEachCase.Text = CString(r("eaPerCs"))
                'lblItemPriceOriginal.Text = Displayformat(Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")), Val(r("eaPrice_o")), DB.NVL(r("scaledItem"), "N") = "Y")


            End If
            'frmItemHdr.DataSource = dt
            'frmItemHdr.DataBind()
            'lblItemCost.text = "as"

        End Using

    End Sub
    '---------------------------------------------------------------------------------------------------------------------------
    'Master DTL
    '---------------------------------------------------------------------------------------------------------------------------
    Private Sub ShowMasterDtl()

        Dim deliverySchedID As String = Session("EMAILBODY_ARG1")

        Dim NewSched As Boolean = Session("EMAILBODY_ARG2")
        'Dim SendReminder As String = Session("EMAILBODY_ARG3")
        Using DB As New DBHelper
            'Get msg

            'Get customer info


            Dim sql = "SELECT g.custGrpName, c.customerName, s.shipName,s.address1,s.address2,s.state,s.city,s.country,s.zip, d.deliverySched_id, d.customer_id, d.shipPoint_id, d.custGrpVendor_id, d.deliveryMaster_id, " & _
                         " d.frequency, d.deliveryDay, d.orderByDay, d.orderByTime, d.sendReminder, c.jfcBranchNr, c.jfcCustomerNr,  v.vendorname, " & _
                         " TO_CHAR(startDate, :1) startDate, TO_CHAR(endDate, :2) endDate,  jm.frequency as frequency, jm.deliveryday as deliveryday, jm.orderbyday as orderbyday, jm.orderbytime as orderbytime " & _
                         "FROM customer_deliverySched d, custGrp g, customer c, customer_shipPoint s,JFCDeliveryMaster jm, custgrp_vendor v  " & _
                         "WHERE d.customer_id = c.customer_id AND g.custGrp_id = c.custGrp_id and v.custgrpvendor_id = d.custgrpvendor_id" & _
                         "  AND d.shipPoint_id = s.shipPoint_id AND d.customer_id = s.customer_id AND jm.deliverymaster_id  = d.deliverymaster_id" & _
                         "  AND d.deliverySched_id = :3 and d.deliveryschedlinenr = :4"
            Dim r = DB.GetReader(sql, DATE_FMT, DATE_FMT, deliverySchedID.Substring(0, deliverySchedID.Length - 1), deliverySchedID.Last.ToString())


            If r.Read() Then
                lblMsg_Master.Text = rm.GetString("Dear", ci) & " " & CString(r("customerName")) & " (" & CString(r("jfcBranchNr")) & "-" & CString(r("jfcCustomerNr")) & "), <br /> <br />"

                If NewSched Then
                    lblMsg_Master.Text &= " " & rm.GetString("Yourdeliveryschedulehasbeenadded", ci) & " " & rm.GetString("Pleaseseethefollowingforyournewschedule", ci)
                    '" Your delivery schedule has been added. Please see the following for your new schedule."
                Else
                    lblMsg_Master.Text &= " " & rm.GetString("Yourdeliveryschedulehasbeenupdated", ci) & " " & rm.GetString("Pleaseseethefollowingforyournewschedule", ci)
                    '" Your delivery schedule has been updated. Please see the following for your new schedule."
                End If


                lblMasterCustomer.Text = CString(r("customerName"))
                lblMasterJFCAcc.Text = CString(r("jfcBranchNr")) & "-" & CString(r("jfcCustomerNr"))
                lblMasterShipDest.Text = CString(r("shipName"))
                lblMasterVendor.Text = CString(r("vendorname"))
                lblMasterSendReminder.Text = IIf(CString(r("sendReminder")) = "Y", "Yes", "No")
                lblMasterAddr1.Text = CString(r("address1"))
                lblMasterAddr2.Text = CString(r("address2"))
                lblMasterCity.Text = CString(r("city"))
                lblMasterState.Text = CString(r("state"))
                lblMasterZip.Text = CString(r("zip"))
                lblMasterCountry.Text = CString(r("country"))
                Dim NewMaster = getMasterdetel(DB, CString(r("deliveryMaster_id")))
                'Dim newMaster = getMasterdetel(DB, CString(Session("EMAILBODY_ARG2")))
                lblMasterFrequency.Text = NewMaster(0)
                lblMasterDeliveryOn.Text = NewMaster(1)
                lblMasterOrderby.Text = NewMaster(2)
                'lblMaster.Text = NewMaster(3)
            End If
            'frmItemHdr.DataSource = dt
            'frmItemHdr.DataBind()
            'lblItemCost.text = "as"

        End Using

    End Sub

    '
    '
    '
    Private Sub showPriceInquirelist()
        'Dim customerID As String = Session("EMAILBODY_ARG1")
        'Dim custGrpID As String = Session("EMAILBODY_ARG3")
        Dim custInfo As CustomerInfo = Session("EMAILBODY_ARG1")

        Dim GroupItemIDlist As List(Of String) = Session("EMAILBODY_ARG2")
        'Dim customerEmail As String = Session("EMAILBODY_ARG3")
        Dim msg As String = Session("EMAILBODY_ARG3")
        Dim NoteMsg As String = Session("EMAILBODY_ARG4")

        Using DB As New DBHelper
            'Get msg

            'Get customer info
            'GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            'nrCustGrpVendor = GetNrCustGrpVendor(DB, custGrpID)
            'isSmallCust = Common.IsSmallCustomer(DB, customerID)
            'useCustomerCtg = Common.UseCustomerCtg(DB, customerID)
            Dim SQL_Select = "SELECT /*+rule*/ " & _
                     " v.vendorName, " & IIf(custInfo.nrCustGrpVendor > 1, "v.vendorName", "NULL") & " AS vendorName_view, " & _
                     " jctgGrp.ctgGrpDesc AS ctgName_view, jctgGrp.dispOrder AS ctgDispOrder, " & _
                     IIf(custInfo.useCustomerCtg, " NVL(customCtg.dispOrder, 0)", " 0") & " AS itemDispOrder, " & _
                     " q.shipDate, i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.itemGrpName," & _
                     " g.dispOrder itemGrpDispOrder, " & _
                     IIf(custInfo.isSmallCust = True, "cp.custItem", "i.itemNr") & " AS itemNr, i.CustItemDesc, v.custGrpVendor_id," & _
                     " i.minSalCnt,  cp.webCustRemark, " & _
                     " ci.eaPerCs, p.scaledItem, " & _
                     " q.csQty AS lastCsQty, CASE WHEN NVL(q.pcQty, 0) > 0 THEN q.pcQty ELSE q.eaQty END AS lastEaQty, q.lbs AS lastLbs," & _
                     " CASE WHEN q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0 THEN 'Y' ELSE 'N' END ordered, " & _
                     " CASE WHEN NVL(q.csQty, 0) > 0 OR NVL(q.eaQty, 0) > 0 OR NVL(q.lbs, 0) > 0 THEN 'Y' ELSE 'N' END AS lastQtyExist, " & _
                     " TO_CHAR(q.shipDate, " & DB.SqlEsc(SHORTDATE_FMT) & ") AS shipDate_fmt, cc.ENABLED," & _
                     " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
                     " CASE WHEN " & IIf(custInfo.isSmallCust = True, "cp.custItem", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(custInfo.isSmallCust = True, "cp.custItem", "i.itemNr") & " || '<br />(' || i.item_id || ')' END AS itemIdNr_view, " & _
                     " CASE WHEN ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0 THEN 'Y' ELSE 'N' END askPrice, " & _
                     IIf(Common.ShowItemDtl(DB, custInfo.customerID) = True, " TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2), 99999.99) || ' kg, ' || TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3), 99999.999) || ' m³'", "NULL") & " AS itemDtl, "

            Dim sql_from = " FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(custInfo.isSmallCust, IIf(custInfo.IsExcepPriceCustomer, "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
           "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
           IIf(custInfo.useCustomerCtg = False, "LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
                                       "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id ", "") & _
           IIf(custInfo.useCustomerCtg = True, "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & DB.SqlEsc(custInfo.customerID) & ", i.item_id, 'Y') " & _
                                      "INNER JOIN customer_ctg customCtg ON customCtg.ctgGrp_id = jctgGrp.ctgGrp_id AND customCtg.customer_id = " & DB.SqlEsc(custInfo.customerID) & " AND customCtg.item_id = i.item_id ", "") & _
           "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(custInfo.jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(custInfo.jfcCustomerNr) & " " & _
           "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(custInfo.PriceGrpID) & " " & _
           "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " & _
           "LEFT OUTER JOIN customer_custItem cc ON cc.customer_id = " & DB.SqlEsc(custInfo.customerID) & " and i.custGrpItem_id = cc.CUSTGRPITEM_ID  "
            Dim SQL_eaCost = "(CASE WHEN ci.eaPerCs <= 1 THEN 0 ELSE " & _
                                  "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN ci.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/ci.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                                  " ELSE (CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs, 2) END) END) " & _
                               "END)"
            Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"

            Dim whereClause = " WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " & _
                          "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " & _
                          "  AND i.custGrpItem_id = ci.custGrpItem_id " & _
                          "  AND g.custgrp_id = " & DB.SqlEsc(custInfo.custGrpID) & _
                          "  AND ci.customer_id = " & DB.SqlEsc(custInfo.customerID) & " " & _
                          "  AND NOT (ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0) " & _
                          "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " & _
                          "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') "
            SQL_Select &= " NVL(p.csCost, 0) AS csCost, " & _
                            " " & SQL_eaCost & " AS eaCost, " & _
                            " " & SQL_lbCost & " AS lbCost, " & _
                              " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view," & _
                " CASE WHEN ci.lbPrice_orderByQty <> 0 THEN ROUND((ci.lbPrice_orderByQty - " & SQL_lbCost & ") / ci.lbPrice_orderByQty, 2) " & _
                " ELSE (CASE WHEN NVL(ci.csPrice, 0) = 0 THEN 1 ELSE ROUND((ci.csPrice - NVL(p.csCost, 0)) / ci.csPrice, 2) END) END gp," & _
                " cc.CUSTITEMCODE as item_id,i.item_id as item_id_o, cc.CUSTREMARK as jfcRemark,cc.CUSTCSPRICE as csPrice, " & _
                " ci.csPrice as csPrice_o, cc.CUSTEAPRICE as eaPrice, ci.eaPrice as eaPrice_o," & _
                " cc.CUSTLBPRICE_ORDERBYWGT as lbPrice_OrderByWgt,ci.lbPrice_OrderByWgt as lbPrice_OrderByWgt_o,  cc.CUSTLBPRICE_ORDERBYQTY as lbPrice_OrderByQty,ci.lbPrice_OrderByQty as lbPrice_OrderByQty_o," & _
                " CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
            '" CASE WHEN i.jfcitem = 'Y' AND i.custItemDesc IS NULL  THEN NVL(" & IIf(IsMobile(), "a.itemDesc_short", "a.itemDesc") & ", a.itemDesc) ELSE i.custItemDesc END AS itemDesc_view, cc.CUSTITEMDESC as custItem_Desc"
            whereClause &= " AND i.custGrpItem_id in (" & String.Join(",", GroupItemIDlist.Select(Function(x) DB.SqlEsc(x))) & ")"

            'Common.ApplyInEmailOrCustomer(DB, customerID, "HidePrice", customerEmail)


            'Dim r = DB.GetReader(SQL_Select & sql_from & whereClause)
            'lblMsg_PriceInquirelist.Text = "A item has been added. <br /> <br />  <span style='font-weight: bold; color: #FF0000;'>" & msg & "</span>"
            SqlPriceInquirelist.ProviderName = "System.Data.OracleClient"
            SqlPriceInquirelist.ConnectionString = DB.GetConnectionString
            SqlPriceInquirelist.SelectCommand = SQL_Select & sql_from & whereClause
            grdPriceInquirelist.DataSourceID = "SqlPriceInquirelist"
            grdPriceInquirelist.DataBind()
            '"Items have been added. rm.GetString("Itemshavebeenadded", ci) & "<br />
            lblMsg_PriceInquireListAdd.Text = "<br />  <span style='font-weight: bold; color: #FF0000;'>" & msg & "</span>"
            If Not String.IsNullOrWhiteSpace(NoteMsg) Then
                lblMsg_PriceInquirelist.Text &= "<br /> <br /> " & rm.GetString("Message", ci) & ": <span style='font-weight: bold; background-color: #ffffad;' >" & NoteMsg & "</span>"
            End If

            'If r.Read() Then
            '    If msg = String.Empty Then    
            '        lblMsg_Item.Text = "Your item info has been added / updated."
            '        lblItemPriceNew.Text = Displayformat(Val(r("lbPrice_OrderByQty")), Val(r("csPrice")), Val(r("eaPrice")), DB.NVL(r("scaledItem"), "N") = "Y")
            '        Dim hidePrice = Common.ApplyInEmailOrCustomer(DB, lblCustomerID.Text, "HidePrice", customerEmail)
            '        If hidePrice Then
            '            pnlItemPriceNew.Visible = False
            '            'pnlItemPriceOriginal.Visible = False
            '        End If
            '    Else
            '        lblMsg_Item.Text = "A item has been added. <br /> <br />  <span style='font-weight: bold; color: #FF0000;'>" & msg & "</span>"
            '        lblItemPriceNew.Text = Displayformat(Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")), Val(r("eaPrice_o")), DB.NVL(r("scaledItem"), "N") = "Y")
            '    End If
            '    'lblItemCode.Text = CString(r("item_id_o"))
            '    'lblItemCodeNew.Text = CString(r("item_id"))
            '    'lblItemDesc.Text = CString(r("itemDesc_view"))
            '    'lblItemDescNew.Text = CString(r("custItem_Desc"))
            '    'lblItemRemark.Text = CString(r("jfcRemark"))
            '    'lblItemEachCase.Text = CString(r("eaPerCs"))
            '    ''lblItemPriceOriginal.Text = Displayformat(Val(r("lbPrice_OrderByQty_o")), Val(r("csPrice_o")), Val(r("eaPrice_o")), DB.NVL(r("scaledItem"), "N") = "Y")
            'End If
            'frmItemHdr.DataSource = dt
            'frmItemHdr.DataBind()
            'lblItemCost.text = "as"
        End Using
    End Sub
    Private Function getMasterdetel(db As DBHelper, MasterID As String) As String()
        Dim Sql = "SELECT jd.deliveryMaster_id,jd.DELIVERYMASTERNAME, jd.frequency, jd.deliveryDay, jd.orderByDay, jd.orderByTime, " & _
                   " c.deliverySched_id, c.customer_id, c.shipPoint_id, c.sendReminder, to_char(NVL(c.startDate, SYSDATE), " & db.SqlEsc(DATE_FMT) & ") startDate, to_char(c.endDate, " & db.SqlEsc(DATE_FMT) & ") endDate " & _
                   "FROM JFCDeliveryMaster jd, customer_deliverySched c " & _
                   "WHERE jd.deliveryMaster_id = c.deliveryMaster_id (+) " & _
                   "  AND jd.deliveryMaster_id = " & MasterID

        Dim r = db.GetReader(Sql)
        Dim result(3) As String
        result(0) = ""
        result(1) = ""
        result(2) = ""
        result(3) = ""
        If r.Read() Then
            result(0) = CString(r("frequency"))
            result(1) = CString(r("deliveryDay"))
            result(2) = CString(r("orderByDay")) & " " & CString(r("orderByTime"))
            result(3) = CString(r("DELIVERYMASTERNAME"))
        End If

        Return result

    End Function
End Class


#End ExternalSource
