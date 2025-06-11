Imports System.Data
Imports Common
Imports DBUpdate
Imports System.Globalization
Imports System.Threading
Imports System.Resources

Partial Class Common_OrderDtl
    Inherits System.Web.UI.UserControl

    '--------------------------------------------------------------------------------------------------------------------
    'Parameters passed by caller
    'Session("ORDERDTL_CMD"): "CUST", "JFCADMIN", "VENDOR" or "CUSTADMIN".
    'Session("ORDERDTL_MODE"): "VIEW", "EDIT", "VIEW_JETS"
    'Session("ORDERDTL_ORDERID")
    'Session("ORDERDTL_CUSTID"): Needed because Session("ORDERDTL_ORDERID") for Jets order is not unique (primary key is jfcBranchNr and orderNr)
    'Parameters passed back to caller
    'Session("ORDERDTL_RETURNCMD") 
    '--------------------------------------------------------------------------------------------------------------------
    Private rm As ResourceManager = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))
    Dim ci As CultureInfo
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'Post process for modal dialog box
        If Request("DialogResult") = "YES" Then
            dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"), ViewState("DIALOG_ARG2"))
        ElseIf Request("DialogResult") = "NO" Then
            dialogResult_no(ViewState("DIALOG_CMD"))
        End If

        If IsPostBack = False Then
            'If Session("CUSTDTL_CMD") = "VIEW" Then pageLoad()
        End If
    End Sub

    Public Sub pageLoad()
        Dim reason As String = ""

        Using DB As New DBHelper
            'Init textchanged value
            If lblTextChanged.Value <> "YES" Then lblTextChanged.Value = ViewState("TEXT_CHANGED")

            'Get images and customer company type
            DataBind()

            'Set ViewState
            ViewState("ORDER_CMD") = Session("ORDERDTL_CMD")
            ViewState("ORDER_ID") = Session("ORDERDTL_ORDERID")
            ViewState("CUSTOMER_ID") = Session("ORDERDTL_CUSTID")
            'ViewState("TAB") = Request.QueryString("TAB")

            'Set properties
            pnlOrderDtl_View.Visible = False
            pnlOrderDtl_Edit.Visible = False
            'pnlOrderDtl_jetsView.Visible = False
            InvisibleCtrl({lblDeliveryDate, lblCustOrderNr, lblMessage, lblVendorNote})
            InvisibleCtrl({txtDeliveryDate, txtCustOrderNr, txtMessage, txtVendorNote, txtEmailMsg})
            ClearCtrl({txtEmailMsg})
            pnlItem.Visible = False
            'lblTitle2.Visible = False
            btnEmailUser_view.Visible = False
            btnEmailOther_view.Visible = False

            setOrderMode(ViewState("ORDER_ID"), reason)
            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                VisibleCtrl({txtCustOrderNr})
                If ViewState("ORDER_CMD") = "JFCADMIN" Then
                    VisibleCtrl({txtDeliveryDate, txtVendorNote})
                    VisibleCtrl({lblMessage})
                Else
                    VisibleCtrl({lblDeliveryDate, lblVendorNote})
                    VisibleCtrl({txtMessage})
                End If
                pnlOrderDtl_Edit.Visible = True
                If ViewState("ORDER_CMD") = "JFCADMIN" Then pnlItem.Visible = True
                lblTitle.Text = "Edit Order"
                VisibleCtrl({txtEmailMsg})
            ElseIf ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" Then
                VisibleCtrl({lblDeliveryDate, lblCustOrderNr, lblMessage, lblVendorNote})
                pnlOrderDtl_View.Visible = True
                lblTitle.Text = "View Order"
                lblViewOnlyReason.Text = reason
                btnEmailUser_view.Visible = True
                'btnEmailOther_view.Visible = True
            ElseIf ViewState("ORDER_MODE") = "VIEW_JETS_ORDER" Then
                VisibleCtrl({lblDeliveryDate, lblCustOrderNr, lblMessage, lblVendorNote})
                pnlOrderDtl_View.Visible = True
                lblTitle.Text = "View Order"
                'lblTitle2.Visible = True 'Show (Non Web Order)
                lblViewOnlyReason.Text = reason
            End If

            'Init
            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                txtItem.Text = ""
                clearItemDtl()
            End If

            'Show order header
            showOrderHdr()

            'Show item list
            showOrderDtl()
        End Using
    End Sub

    Protected Sub grdItem_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles grdItem_edit.RowCommand, grdItem_view.RowCommand
        Dim index As Integer = 0
        Dim lbl As Label
        Dim lnk As LinkButton
        Dim txt As TextBox
        Dim btn As LinkButton
        Dim custGrpItemID As String
        Dim r As DataReader
        Dim docName As String
        Dim SQL As String
        Dim rtn As Boolean = False
        Dim info As String
        'Dim qtyHistory As String = ""
        Dim errMsg As String = ""

        Using DB As New DBHelper
            If e IsNot Nothing Then
                CheckItemAvail()
                If e.CommandName = "DELETE_ROW" Then
                    'Check error
                    If canEditOrder(DB, ViewState("ORDER_ID"), errMsg) = False Then
                        MessageBox(errMsg, "Error", DB)

                        Exit Sub
                    End If
                    'Confirm
                    ViewState("DIALOG_CMD") = "DELETE_ITEM"
                    index = Convert.ToInt32(e.CommandArgument)
                    lbl = grdItem_edit.Rows.Item(index).FindControl("lblCustGrpItemID")
                    ViewState("DIALOG_ARG1") = lbl.Text
                    MessageBoxYesNo("Are you sure you want to delete this item from your order?", "Confirm Deletion", DB)
                    CheckItemAvail()
                    Exit Sub
                    'ElseIf e.CommandName = "HDR_IMAGE" Then
                    '    btn = grdItem.HeaderRow.FindControl("btnHdrImage")
                    '    btn.ImageUrl = IIf(lblShowImage.Text = "ON", GetWebImgUrl_OnOff("IMAGE", "OFF"), GetWebImgUrl_OnOff("IMAGE", "ON"))
                    '    lblShowImage.Text = IIf(lblShowImage.Text = "ON", "OFF", "ON")
                ElseIf e.CommandName = "DETAIL" Then
                    'Show item detail in popup window
                    custGrpItemID = Convert.ToString(e.CommandArgument)
                    Session("SHOWITEMDTL_ARG1") = lblCustomerID.Text
                    Session("SHOWITEMDTL_ARG2") = custGrpItemID
                    ctrShowItemDtl.showItemDtl()

                    Exit Sub
                    'ElseIf e.CommandName = "QTY_HISTORY" Then
                    '    'Show web ordered qty history in popup window
                    '    custGrpItemID = Convert.ToString(e.CommandArgument)
                    '    MessageBox(GetQtyHistory(DB, lblCustomerID.Text, custGrpItemID), "Ordered Qty History", DB)
                    '    Exit Sub
                ElseIf e.CommandName = "SETPRICE" Then
                    custGrpItemID = Convert.ToString(e.CommandArgument)
                    Session("SHOWITEMDTL_ARG1") = lblCustomerID.Text
                    Session("SHOWITEMDTL_ARG2") = custGrpItemID
                    GetItemInfo(custGrpItemID)
                    setItemPrice()
                    'Don't remember scroll position
                    Page.MaintainScrollPositionOnPostBack = False
                    'addItemInOrder()
                    btnAddItem.Focus()
                    txtNewCsQty.Focus()
                    'SetFocus(Me.Page, btnAddItem)
                    CheckItemAvail()
                    Exit Sub
                ElseIf e.CommandName = "SHOW_VPSP" Then
                    index = Convert.ToInt32(e.CommandArgument)
                    If ViewState("ORDER_CMD") = "JFCADMIN" Then
                        btn = grdItem_edit.Rows.Item(index).FindControl("lnkShowCsQtyVpSp")
                        If btn.Visible = True Then
                            btn.Visible = False
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtCsQty_vp")
                            txt.Visible = True
                            lbl = grdItem_edit.Rows.Item(index).FindControl("lblCsQty_vp")
                            lbl.Visible = True
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtCsQty_sp")
                            txt.Visible = True
                            lbl = grdItem_edit.Rows.Item(index).FindControl("lblCsQty_sp")
                            lbl.Visible = True
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtVpMemo")
                            txt.Visible = True
                        End If
                        btn = grdItem_edit.Rows.Item(index).FindControl("lnkShowEaQtyVpSp")
                        If btn.Visible = True Then
                            btn.Visible = False
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtEaQty_vp")
                            txt.Visible = True
                            lbl = grdItem_edit.Rows.Item(index).FindControl("lblEaQty_vp")
                            lbl.Visible = True
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtEaQty_sp")
                            txt.Visible = True
                            lbl = grdItem_edit.Rows.Item(index).FindControl("lblEaQty_sp")
                            lbl.Visible = True
                            txt = grdItem_edit.Rows.Item(index).FindControl("txtVpMemo")
                            txt.Visible = True
                        End If
                    End If

                    Exit Sub
                ElseIf e.CommandName = "SHOW_COST" Then
                    index = Convert.ToInt32(e.CommandArgument)
                    If ViewState("ORDER_CMD") = "JFCADMIN" Then
                        lnk = grdItem_edit.Rows.Item(index).FindControl("lnkItemIdNrView")
                        info = "#" & lnk.Text
                        lnk = grdItem_edit.Rows.Item(index).FindControl("lnkItemDesc")
                        info &= " - " & lnk.Text
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblCsPrice")
                        info &= "<br /><br />Price: " & IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/cs ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblEaPrice")
                        info &= IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/ea ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblLbPrice_OrderByQty")
                        info &= IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/lb ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblCsCost")
                        info &= "<br />Cost: " & IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/cs ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblEaCost")
                        info &= IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/ea ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblLbCost")
                        info &= IIf(lbl.Visible = True, Replace(lbl.Text, "<br />", "") & "/lb ", "")
                        lbl = grdItem_edit.Rows.Item(index).FindControl("lblGp")
                        info &= "<br />GP: " & lbl.Text
                        MessageBox(info, "Information")
                    End If

                    Exit Sub
                ElseIf e.CommandName = "DOWNLOAD" Then
                    'Download reference
                    docName = Convert.ToString(e.CommandArgument)
                    SQL = "SELECT directory, fileName FROM customer_document " & _
                            "WHERE UPPER(docType) = 'INVOICE' AND docName = :1 AND customer_id = :2 AND custGrpVendor_id = :3"
                    r = DB.GetReader(SQL, docName, lblCustomerID.Text, lblCustGrpVendorID.Text)
                    If r.Read Then
                        rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
                    End If
                    If rtn = False Then MessageBox("Cannot open the document", "Error", DB)

                    Exit Sub
                End If
            End If

            'Show order detail
            showOrderDtl()

        End Using
    End Sub

    Protected Sub grdItem_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdItem_edit.RowDataBound, grdItem_view.RowDataBound
        Dim lbl As Label
        Dim lblprice As Label
        Dim lblbottom As Label
        Dim lblIsNew As Label

        'Highlight the mourse-over row
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.CssClass = "selectRow"
            e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
            'Dim lnk As LinkButton = e.Row.FindControl("lnkSelectRow")
            'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackClientHyperlink(lnk, ""))
        End If

        'Do nothing if the email is not for vendor
        If sender.id <> "grdItem_edit" OrElse ViewState("ORDER_CMD") <> "JFCADMIN" Then Exit Sub

        'Highlight the mourse-over row
        If e.Row.RowType = DataControlRowType.DataRow Then


            lbl = e.Row.FindControl("lblOutOfStock")
            If lbl.Text = "Y" Then
                'Highlight the row for outofstock item pink and change font color of ordered qty and available qty to red 
                e.Row.BackColor = Drawing.Color.FromName("#FFE4E1")
                lbl = e.Row.FindControl("lblCsAvail")
                lbl.ForeColor = Drawing.Color.Red
                lbl = e.Row.FindControl("lblEaAvail")
                lbl.ForeColor = Drawing.Color.Red
                lbl = e.Row.FindControl("lblCs3")
                lbl.ForeColor = Drawing.Color.Red
                lbl = e.Row.FindControl("lblEa3")
                lbl.ForeColor = Drawing.Color.Red
                Exit Sub
            End If

            lblbottom = e.Row.FindControl("lblbtmCs")
            lblprice = e.Row.FindControl("lblCsPrice")
            If Val(lblprice.Text) < Val(lblbottom.Text) Then
                e.Row.BackColor = Drawing.Color.FromName("#FFFF99")
                Exit Sub
            End If

            lbl = e.Row.FindControl("lblPromoitem")
            If lbl.Text <> "" Then
                e.Row.BackColor = Drawing.Color.FromName("#90EE90")
                Exit Sub
            End If


            lblIsNew = e.Row.FindControl("lblIsNew")
            If CString(lblIsNew.Text) = "Y" Then
                e.Row.BackColor = Drawing.Color.FromName("#C3E8F6")
                Exit Sub
            End If



        End If
    End Sub

    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String, ByVal dialogArg2 As String)
        Dim custGrpItemID As String
        Dim webOrderID As String
        Dim emailBody As String = ""
        Dim errMsg As String = ""
        Dim userEmailFailed As Boolean = False
        'Dim vendorEmailFailed As Boolean = False
        Dim userEmail As String
        Dim vendorEmail, customerEmail As String
        Dim orderState As String
        Session("CHANGE_NEWSTATUS") = False
        Using DB As New DBHelper
            Select Case dialogCmd
                Case "DELETE_ITEM"
                    webOrderID = ViewState("ORDER_ID")
                    orderState = GetOrderState(DB, webOrderID)
                    custGrpItemID = dialogArg1
                    DB.BeginTransaction()
                    'Insert order history
                    DBUpdate.InsertOrderHistory(DB, webOrderID, False)
                    DeleteItemInOrder(DB, webOrderID, custGrpItemID, False)
                    'Set order state to 'Order updated but not received' if a confirmed or received order was updated by JFC admin
                    If orderState = "O" OrElse orderState = "C" Then DBUpdate.UpdateOrderState(DB, webOrderID, "U", False)
                    DB.CommitTransaction()
                    InsertEventLog(DB, "Deleted item from order", webOrderID, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                    'Refresh
                    showOrderDtl()
                Case "DELETE_ORDER"
                    webOrderID = ViewState("ORDER_ID")
                    userEmail = GetUserEmail(DB)
                    customerEmail = GetOrderEmail(DB, webOrderID, "CUSTOMER")
                    vendorEmail = GetOrderEmail(DB, webOrderID, "VENDOR")
                    'custOrderNr = GetCustOrderNr(DB, webOrderID)
                    'Mark order as deleted
                    DeleteOrder(DB, webOrderID, False)
                    'DeleteOrder(DB, webOrderID, IIf(ViewState("ORDER_CMD") = "CUST" AndAlso IsJfcOrder(DB, webOrderID) = True, True, False))
                    'CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", "DELETED", txtEmailMsg.Text) 
                    emailBody = dialogArg2
                    If emailBody <> String.Empty Then
                        'Send email to customer
                        'If Common.SendMail(SYSTEM_EMAIL, userEmail, vendorEmail, "Your order '" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty) & "was canceled", emailBody, Nothing, True, errMsg) > 0 Then
                        If Common.SendMail(SYSTEM_EMAIL, customerEmail, vendorEmail, dialogArg1, emailBody, Nothing, True, errMsg) > 0 Then
                            userEmailFailed = True
                        Else
                            'Mark email as sent
                            UpSertOrderEmail(DB, webOrderID, "CUSTOMER", "S", userEmail)
                        End If
                        'If SendOrderEmailToVendor(DB, webOrderID) = True Then
                    End If

                    '    'Get email body for vendor from email template in user control
                    '    emailBody = dialogArg2
                    '    'Send email to vendor
                    '    If Common.SendMail(SYSTEM_EMAIL, vendorEmail, userEmail, "You received a canceled order '" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty), emailBody, Nothing, True, errMsg) > 0 Then
                    '        vendorEmailFailed = True
                    '    Else
                    '        'Mark email state back to sent even if the email was received
                    '        UpSertOrderEmail(DB, webOrderID, "VENDOR", "S", userEmail)
                    '    End If
                    'End If
                    If emailBody <> String.Empty AndAlso userEmailFailed = False Then 'AndAlso vendorEmailFailed = False
                        'Email sent ok
                        InsertEventLog(DB, "Deleted order and sent confirmation email", webOrderID, Nothing, Nothing, True)
                        MessageLine("CONFIRM", "Successfully deleted, and confirmation email for this cancelation has been sent.")
                    Else
                        'Email sent failed
                        'errMsg = "Successfully deleted, but "
                        'If userEmailFailed = True Then errMsg &= "(1) Confirmation email could not be sent to you because " & userEmailErrMsg.ToLower & ". "
                        ''If vendorEmailFailed = True Then errMsg &= "confirmation email could not be sent to your sales rep. Please contact your sales rep."
                        InsertEventLog(DB, "Deleted order", webOrderID, Nothing, Nothing, True)
                        MessageLine("CONFIRM", "Successfully deleted.")
                        'MessageBox(errMsg, "Warning", DB)
                        'Session("MSG_TYPE") = "WARNING"
                        'Session("MSG") = errMsg
                    End If
                    'Go back to order history
                    Session("ORDERDTL_RETURNCMD") = "DELETED"
                Case "TEXT_CHANGED"
                    addItemInOrder()
                Case "ITEM_RESTORE"
                    deleteCustCustItem(DB, lblCustomerID.Text, dialogArg1)
                    btnGetItem_Click(Nothing, Nothing)
                Case "CHANGE_NEWSTATUS"
                    webOrderID = ViewState("ORDER_ID")
                    DBUpdate.UpdateOrderState(DB, webOrderID, "N")
                    Session("ORDERDTL_RETURNCMD") = "UPDATED"

                    InsertEventLog(DB, "Change order Status", webOrderID, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully changed the status")
            End Select
        End Using
        CheckItemAvail()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As System.EventArgs) Handles lnkDelete.Click
        Dim emailBody As String = ""
        Dim emailSubject As String = ""
        Dim custOrderNr As String
        Dim webOrderID As String = ViewState("ORDER_ID")
        Dim customerEmail As String
        Dim errMsg As String = ""

        Using DB As New DBHelper
            'Check error
            If canEditOrder(DB, webOrderID, errMsg) = False Then
                MessageBox(errMsg, "Error", DB)
                Exit Sub
            End If
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_ORDER"
            customerEmail = GetOrderEmail(DB, webOrderID, "CUSTOMER")
            custOrderNr = GetCustOrderNr(DB, webOrderID)

            'Set customer language
            ci = New CultureInfo(Common.GetLangauge(DB, customerEmail))

            Thread.CurrentThread.CurrentCulture = ci
            Thread.CurrentThread.CurrentUICulture = ci

            'Get email body for user from email template in user control
            CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", "DELETED", txtEmailMsg.Text)
            ViewState("DIALOG_ARG1") = String.Format(rm.GetString("Yourorderwascanceled", ci), "'" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(" & rm.GetString("CustPO", ci) & custOrderNr & ") ", String.Empty))
            '"Your order '" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty) & "was canceled"
            ViewState("DIALOG_ARG2") = emailBody

            'Set language back to English
            Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")

            ''Get email body for vendor from email template in user control
            'CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "VENDOR", "DELETED", txtEmailMsg.Text)
            'ViewState("DIALOG_ARG2") = emailBody
            MessageBoxYesNo("Are you sure you want to cancel this order?", "Confirm Cancelation", DB)
        End Using
        CheckItemAvail()

    End Sub

    Protected Sub btnDeleteOrder_Click(sender As Object, e As System.EventArgs) Handles btnDeleteOrder.Click
        Dim emailBody As String = ""
        Dim webOrderID As String = ViewState("ORDER_ID")
        Dim errMsg As String = ""

        Using DB As New DBHelper
            'Check error
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_ORDER"
            ''Get email body for user from email template in user control
            'CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", "DELETED", txtEmailMsg.Text)
            'ViewState("DIALOG_ARG1") = emailBody
            ''Get email body for vendor from email template in user control
            ''CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "VENDOR", "DELETED", txtEmailMsg.Text) 
            'ViewState("DIALOG_ARG2") = emailBody
            MessageBoxYesNo("Are you sure you want to cancel this order?", "Confirm Cancelation", DB)
        End Using
        CheckItemAvail()
    End Sub

    Private Sub dialogResult_no(ByVal dialogCmd As String)
        Select Case dialogCmd
            Case "TEXT_CHANGED"
                ViewState("TEXT_CHANGED") = "YES"
            Case "ITEM_RESTORE"
                txtItem.Text = ""
                clearQtyEntry()
                clearItemDtl()

        End Select
        CheckItemAvail()
    End Sub

    Protected Sub btnInfo_Click(sender As Object, e As System.EventArgs) Handles btnInfo_orderMsg.Click ', btnInfo_totalAmt.Click
        MessageBox(sender.ToolTip, "Information")
        CheckItemAvail()
    End Sub

    Protected Sub btnClearItem_Click(sender As Object, e As System.EventArgs) Handles btnClearItem.Click
        txtItem.Text = ""
        clearQtyEntry()
        clearItemDtl()
        CheckItemAvail()
        'txtItem.Focus 'TODO: Disable autocomplete
    End Sub

    Protected Sub CheckItemAvail()
        For Each row As GridViewRow In grdItem_edit.Rows
            Dim lbl As Label = row.FindControl("lblOutOfStock")
            If lbl.Text = "Y" Then
                row.BackColor = Drawing.Color.FromName("#FFE4E1")
                lbl = row.FindControl("lblCsAvail")
                lbl.ForeColor = Drawing.Color.Red
                lbl = row.FindControl("lblEaAvail")
                lbl.ForeColor = Drawing.Color.Red
                lbl = row.FindControl("lblCs3")
                lbl.ForeColor = Drawing.Color.Red
                lbl = row.FindControl("lblEa3")
                lbl.ForeColor = Drawing.Color.Red
                Continue For
            End If
            Dim lblbottom As Label = row.FindControl("lblbtmCs")
            lbl = row.FindControl("lblPromoitem")
            If lbl.Text <> "" Then
                row.BackColor = Drawing.Color.FromName("#90EE90")

            End If

            Dim lblprice As Label = row.FindControl("lblCsPrice")
            If Val(lblprice.Text) < Val(lblbottom.Text) Then
                row.BackColor = Drawing.Color.FromName("#FFFF99")
                Continue For
            End If


            Dim lblIsNew As Label = row.FindControl("lblIsNew")
            If CString(lblIsNew.Text) = "Y" Then
                row.BackColor = Drawing.Color.FromName("#C3E8F6")
                Continue For
            End If
        Next
    End Sub

    'Protected Sub txtItem_TextChanged(sender As Object, e As System.EventArgs) Handles txtItem.TextChanged
    '    btnGetItem_Click(Nothing, Nothing) 'Doesn't return result when setting AutoPostback="True"
    'End Sub

    Protected Sub btnGetItem_Click(sender As Object, e As System.EventArgs) Handles btnGetItem.Click
        Using DB As New DBHelper
            Dim r As DataReader
            Dim custGrpItemID As String
            Dim email As String = GetUserEmail(DB)
            Dim custGrpID As String
            Dim noPrice As Boolean
            Dim csCost, eaCost, lbCost, gp As Decimal
            Dim btmcsPirce, btmeaPirce, btmlbPirce As Decimal
            Dim avgCsWgt As Decimal = 0
            Dim avgEaWgt As Decimal = 0
            Dim lbsPerEa As Decimal = 0
            Dim minSalCnt As Decimal

            'Check error
            If txtItem.Text.Contains(",") = False Then
                MessageBox("Please select an item from the auto-complete list", "Error", DB)
                Exit Sub
            End If

            'Clear item info
            clearItemDtl()
            clearQtyEntry()

            'Set properties
            If Session("SYS_SESSION_USERTYPE") = "CUSTOMER" OrElse Session("SYS_SESSION_USERTYPE") = "JFCADMIN" Then
                custGrpID = IIf(Session("SYS_SESSION_USERTYPE") = "CUSTOMER", Session("SYS_SESSION_CUSTGRPID"), GetCustGrpID(DB, lblCustomerID.Text))
                pnlNewOrderForm.Visible = IIf(GetNrItemGrpInCustGrp(DB, custGrpID) > 1, True, False)
                pnlNewVendor.Visible = IIf(GetNrCustGrpVendor(DB, custGrpID) > 1, True, False)
                pnlNewShowItemDtl.Visible = Common.ShowItemDtl(DB, lblCustomerID.Text)
                pnlNewShowPrice.Visible = IIf(Session("SYS_SESSION_USERTYPE") = "CUSTOMER" AndAlso Common.ApplyInEmailOrCustomer(DB, lblCustomerID.Text, "HidePrice") = True, False, True)
            End If

            'Get item info
            custGrpItemID = GetLastWord(txtItem.Text, ",").PadLeft(CUSTGRP_ITEM_ID_LEN, "0")
            r = DB.GetReader(GetSQL_itemDtl(DB, lblCustomerID.Text, custGrpItemID, "", CString(ViewState("ORDER_ID"))))
            If r.Read() Then
                pnlAddItem.Visible = True
                imgNewItem.ImageUrl = GetItemImgUrl(CString(r("itemType")), CString(r("item_id")))
                imgNewItem_mobile.ImageUrl = GetItemImgUrl(CString(r("itemType")), CString(r("item_id")))
                lblNewCustGrpItemID.Text = CString(r("custGrpItem_id"))
                lblNewItemDesc.Text = CString(r("itemDesc_view"))
                lblNewJfcRemark.Text = CString(r("jfcRemark"))
                lblNewItemIdNr.Text = CString(r("itemIdNr_view"))
                'lblNewItemID.Text = CString(r("itemID"))
                lblNewItemGrpName.Text = CString(r("itemGrpName"))
                lblNewVendorName.Text = CString(r("vendorName"))
                lblNewCtgName.Text = CString(r("ctgName_view"))
                lblNewCsPrice.Text = Val(r("csprice"))
                lblNewCsPrice_view.Text = String.Format("{0:C}", Val(r("csprice"))) & " / cs"
                lblNewCsPrice_view.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewEaPrice.Text = Val(r("eaprice"))
                lblNewEaPrice_view.Text = String.Format("{0:C}", Val(r("eaprice"))) & IIf(Val(r("lbPrice_orderByQty")) <> 0, " / pc", " / ea")
                lblNewEaPrice_view.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewLbPrice_OrderByWgt.Text = Val(r("lbPrice_OrderByWgt"))
                lblNewLbPrice_OrderByWgt_view.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByWgt"))) & " / lb"
                lblNewLbPrice_OrderByWgt_view.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                lblNewLbPrice_OrderByQty.Text = Val(r("lbPrice_OrderByQty"))
                lblNewLbPrice_OrderByQty_view.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByQty"))) & " / lb"
                lblNewLbPrice_OrderByQty_view.Visible = IIf(Val(r("lbPrice_OrderByQty")) <> 0, True, False)
                If lblNewLbPrice_OrderByQty_view.Visible = True Then
                    lblNewCsPrice_view.Text = IIf(IsMobile(), "<br />", "") & lblNewCsPrice_view.Text
                    lblNewEaPrice_view.Text = lblNewEaPrice_view.Text
                End If
                lnkSetPrice.Visible = False
                lblNewAvailQty.Visible = False
                If ViewState("ORDER_CMD") = "JFCADMIN" AndAlso CString(r("jfcitem")) = "Y" Then
                    lnkSetPrice.Visible = True
                    lblNewAvailQty.Visible = True
                    lblNewAvailQty.Text = CString(r("qtyAvail"))

                    csCost = Val(r("csCost"))
                    btmcsPirce = Val(r("btmcsPrice"))
                    minSalCnt = IIf(Val(r("minSalCnt")) = 0, 1, Val(r("minSalCnt")))
                    lbsPerEa = GetJfcLbsPerEa(DB, lblCustomerID.Text, custGrpItemID)
                    Common.CovertPrice(csCost, eaCost, lbCost, r("eapercs"), minSalCnt, IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID), lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                    Common.CovertPrice(btmcsPirce, btmeaPirce, btmlbPirce, r("eapercs"), minSalCnt, IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID), lbsPerEa, r("avgcswgt"), r("avgeawgt"))

                    ''GetCostAndGp(Val(r("eaPerCs")), Val(r("csCost")), lblNewLbPrice_OrderByQty_view.Visible, Val(r("csPrice")), Val(r("lbPrice_orderByQty")), Val(r("avgeawgt")), avgEaWgt, lbsPerEa, csCost, eaCost, lbCost, gp)
                    'If IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID) = True Then
                    '    'If lblNewLbPrice_OrderByQty_view.Visible = True Then
                    '    'Scaled item
                    '    lbsPerEa = GetJfcLbsPerEa(DB, lblCustomerID.Text, custGrpItemID)
                    '    'If IIf(lbsPerEa = 0, 0.1, lbsPerEa) * Val(r("eaPerCs")) = 1 Then
                    '    '    csCost = 0
                    '    'Else
                    '    '    csCost = IIf(Val(r("eapercs")) = 10, Math.Round(Val(r("csCost")) * IIf(Val(r("avgcswgt")) = 0, 9.9, Val(r("avgcswgt"))), 2), Val(r("csCost"))) 'CASE WHEN p.eapercs = 10 THEN ROUND(" & lbCost & "*nvl(Val(r("p.avgcswgt")), 9.9), 2) ELSE " & lbCost & " END 
                    '    'End If
                    '    'If Val(r("eapercs")) <= 1 OrElse Val(r("minSalCnt")) = Val(r("eaPerCs")) Then
                    '    '    eaCost = 0
                    '    'Else
                    '    '    eaCost = IIf(Val(r("eapercs")) = 10, Math.Round(Val(r("csCost")) * IIf(Val(r("avgeawgt")) = 0, 0.9, Val(r("avgeawgt"))), 2), Math.Round(Math.Round(Val(r("csCost")) / Val(r("eapercs")), 3) * IIf(Val(r("avgeawgt")) = 0, 0.9, Val(r("avgeawgt"))) * 10, 2)) 'CASE WHEN p.eapercs = 10 then round(" & lbPrice & "*nvl(Val(r("p.avgeawgt")), 0.9), 2) ELSE ROUND(ROUND(" & lbPrice & "/p.eapercs, 3)*nvl(Val(r("p.avgeawgt")), 0.9)*10, 2) END
                    '    'End If
                    '    'If Val(r("eaPerCs")) = 0 Then lbCost = 0 Else lbCost = Math.Round(Val(r("csCost")) / Val(r("eapercs")) * 10, 2)
                    '    If lbsPerEa * Val(r("eaPerCs")) = 1 Then
                    '        csCost = Val(r("csCost")) * Val(r("avgcswgt")) '0 'Mod on 8/31/2020
                    '    Else
                    '        csCost = Val(r("csCost"))
                    '    End If
                    '    If Val(r("eaPerCs")) / minSalCnt <= 1 Then
                    '        eaCost = 0
                    '    ElseIf Val(r("avgcswgt")) < Val(r("avgeawgt")) * 2 Then 'Add on 8/31/2020
                    '        eaCost = 0
                    '    Else
                    '        eaCost = Math.Round(Val(r("csCost")) / Val(r("eaPerCs")) / lbsPerEa * IIf(Val(r("avgeawgt")) = 0, 0.9, Val(r("avgeawgt"))), 2)
                    '    End If
                    '    If Val(r("eaPerCs")) / minSalCnt <= 1 Then
                    '        lbCost = 0
                    '    Else
                    '        lbCost = Math.Round(Val(r("csCost")) / Val(r("eapercs")) / lbsPerEa, 2)
                    '    End If
                    '    'If Val(r("csprice")) = 0 Then gp = 1 Else gp = (Val(r("csprice")) - Val(r("csCost"))) / Val(r("csprice"))
                    '    'If Val(r("csprice")) = 0 Then gp = 1 Else gp = (Val(r("csprice")) - csCost) / Val(r("csprice"))
                    '    If Val(r("lbPrice_OrderByQty")) = 0 Then gp = 1 Else gp = (Val(r("lbPrice_OrderByQty")) - lbCost) / Val(r("lbPrice_OrderByQty"))
                    lblNewGp.Text = IIf(gp <> 0, "<br />GP " & Math.Round(gp * 100, 1) & "%", "")
                    'Else
                    '    'Regular item
                    '    csCost = Val(r("csCost"))
                    '    'If Val(r("eaPerCs")) <= 1 OrElse Val(r("minSalCnt")) = Val(r("eaPerCs")) Then
                    '    '    eaCost = 0
                    '    'Else
                    '    '    eaCost = Val(r("csCost")) / Val(r("eaPerCs"))
                    '    'End If
                    '    If Val(r("eaPerCs")) / minSalCnt <= 1 Then
                    '        eaCost = 0
                    '    Else
                    '        eaCost = Math.Round(Val(r("csCost")) / Val(r("eaPerCs")), 2)
                    '    End If
                    '    lbCost = 0
                    '    'If Val(r("csPrice")) = 0 Then gp = 1 Else gp = (Val(r("csPrice")) - Val(r("csCost"))) / Val(r("csPrice"))
                    '    If Val(r("csprice")) = 0 Then gp = 1 Else gp = (Val(r("csprice")) - csCost) / Val(r("csprice"))
                    'lblNewGp.Text = IIf(gp <> 1, "GP " & Math.Round(gp * 100, 1) & "%", "")
                    'End If
                    lblNewCsCost.Text = csCost 'Show this in set price popup (not add item panel)
                    lblNewEaCost.Text = eaCost 'Show this in set price popup (not add item panel)
                    lblNewLbCost.Text = lbCost 'Show this in set price popup (not add item panel)
                    lblNewBtmCs.Text = btmcsPirce
                    lblNewBtmEa.Text = btmeaPirce
                    lblNewBtmLb.Text = btmlbPirce
                End If
                lblNewMinSalCnt.Text = Val(r("minSalCnt"))
                lblNewMinSalCnt_view.Text = IIf(IsMobile(), "<br />", "") & "min order: " & Val(r("minSalCnt")) & " ea"
                lblNewMinSalCnt_view.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("minSalCnt")) > 1 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False) 'And Val(r("eaprice")) <> 0
                'lblNewEaPerCs.Text = Val(r("eaPerCs")) & " ea / cs"
                avgCsWgt = Val(r("avgcswgt")) 'GetAvgWgt(Val(r("csPrice")), Val(r("lbPrice_OrderByQty")))
                avgEaWgt = Val(r("avgeawgt")) 'GetAvgWgt(Val(r("eaPrice")), Val(r("lbPrice_OrderByQty")))
                lblNewAvgCsWgt.Text = String.Format("{0:N}", avgCsWgt) & " lbs / cs"
                lblNewAvgCsWgt.Visible = IIf(avgCsWgt <> 0, True, False) 'IIf(avgCsWgt <> 0 AndAlso Val(r("csprice")) <> 0, True, False)
                lblNewAvgEaWgt.Text = String.Format("{0:N}", avgEaWgt) & " lbs / pc"
                lblNewAvgEaWgt.Visible = IIf(avgEaWgt <> 0, True, False) 'IIf(avgEaWgt <> 0 AndAlso Val(r("eaprice")) <> 0, True, False)
                lblNewAvgWgtTitle.Visible = lblNewAvgCsWgt.Visible OrElse lblNewAvgEaWgt.Visible
                pnlNewEaPerCs.Visible = IIf(lblHideEaQty.Text = "N" AndAlso lblHideEaQty.Text = "N", True, False)
                lblNewEaPerCs.Text = CString(r("eaPerCs"))
                lblNewEaPerCs.Visible = IIf(Val(r("eaPerCs")) <> 0 AndAlso Val(r("eaPerCs")) <> 1 AndAlso Val(r("lbPrice_OrderByQty")) = 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                If pnlNewShowItemDtl.Visible = True Then
                    lblNewGrossWeight.Text = CString(r("grossWeight_lbs")) & " lbs, " & CString(r("grossWeight_kg")) & " kg"
                    'lblNewGrossWeight.Visible = IIf(Val(r("grossWeight_lbs")) = 0, False, True)
                    lblNewCaseVol.Text = CString(r("caseVol_ft3")) & " ft³, " & CString(r("caseVol_m3")) & " m³"
                    'lblNewCaseVol.Visible = IIf(Val(r("caseVol_ft3")) = 0, False, True)
                    'lblNewPallet.Text = Val(r("block")) & " x " & Val(r("high")) & " = " & Val(r("block")) * Val(r("high")) 'TODO: Add after JETS stores correct values
                    'lblNewPallet.Visible = IIf(Val(r("block")) = 0 AndAlso Val(r("high")) = 0, False, True)
                End If
                txtNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                txtNewEaQty.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewEaQty.Text = IIf(Val(r("lbPrice_orderByQty")) <> 0, "pc", "ea")
                lblNewEaQty.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                txtNewLbs.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                lblNewLbs.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                txtNewCsQty.CssClass = IIf(Val(r("lastCsQty")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                txtNewEaQty.CssClass = IIf(Val(r("lastEaQty")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                txtNewLbs.CssClass = IIf(Val(r("lastLbs")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                lblNewQtyHistory.Text = GetQtyHistory(DB, lblCustomerID.Text, custGrpItemID, 1)
                lblNewQtyHistoryTitle.Visible = IIf(lblNewQtyHistory.Text = String.Empty, False, True)
                lblNewQtyHistory.Visible = IIf(lblNewQtyHistory.Text = String.Empty, False, True)
                'btnQtyHistory.Visible = IIf(CString(r("webOrdered")) = "Y", True, False)
                'If CString(r("webOrdered")) = "Y" Then btnQtyHistory.ToolTip = GetQtyHistory(DB, lblCustomerID.Text, custGrpItemID, 10, True)
                'If CString(r("outOfStock")) = "Y" andalso Then btnAddItem.Visible = False
                noPrice = IIf(Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0 AndAlso Val(r("lbPrice_OrderByWgt")) = 0 AndAlso Val(r("lbPrice_OrderByQty")) = 0, True, False)
                lblNewOutOfStock.Visible = IIf(CString(r("outOfStock")) = "Y" AndAlso (ViewState("ORDER_CMD") = "JFCADMIN" OrElse noPrice = True), True, False)
                lblNewArrivDate.Text = "(Est. arrival: " & CString(r("arrivDate_fmt")) & ")"
                lblNewArrivDate.Visible = IIf(CString(r("arrivDate_fmt")) <> "" AndAlso (ViewState("ORDER_CMD") = "JFCADMIN" OrElse noPrice = True), True, False)
                btnAddItem.Visible = True
                If noPrice = True Then
                    If ViewState("ORDER_CMD") <> "JFCADMIN" Then
                        lblNewAskPrice.Visible = True
                    End If
                    btnAddItem.Visible = False
                End If
                lnkNewShowVpSp.Visible = False
                pnlNewVpSp.Visible = False
                If noPrice = False AndAlso ViewState("ORDER_CMD") = "JFCADMIN" AndAlso CString(r("jfcitem")) = "Y" Then
                    'lnkNewShowVpSp.Visible = True Comment out by YIFU 11/16/2020
                End If
                If CString(r("orderEaQty")) <> "" Then
                    'Item already exists in order
                    txtNewCsQty.Text = Val(r("orderCsQty"))
                    txtNewEaQty.Text = Val(r("orderEaQty"))
                    txtNewLbs.Text = Val(r("orderLbs"))
                    txtNewCsQty_vp.Text = Val(r("orderCsQty_vp"))
                    txtNewEaQty_vp.Text = Val(r("orderEaQty_vp"))
                    txtNewCsQty_sp.Text = Val(r("orderCsQty_sp"))
                    txtNewEaQty_sp.Text = Val(r("orderEaQty_sp"))
                    txtNewVpMemo.Text = CString(r("orderVpMemo"))
                    'If CString(r("outOfStock")) <> "Y" Then
                    btnAddItem.Text = IIf(ViewState("ORDER_CMD") = "JFCADMIN", "Update", "Update Qty")
                    lblNewMsg.Text = "This item is already in your order. You may change the qty."
                    'End If
                End If

            ElseIf GetDeleteCustomer_Item(DB, lblCustomerID.Text, custGrpItemID) > 0 Then
                ViewState("DIALOG_CMD") = "ITEM_RESTORE"
                ViewState("DIALOG_ARG1") = custGrpItemID
                MessageBoxYesNo("This item has been deleted from the order form. Would you like to restore?", "Item has been delete", DB)
                Exit Sub
            Else
                r.close()
                MessageBox("Item not found. Please select an item from the auto-complete list", "Error", DB)
                Exit Sub
            End If
            CheckItemAvail()

            'Show item list

            r.close()
            FocusVisibleQty(txtNewCsQty, txtNewEaQty, txtNewLbs)
        End Using
    End Sub

    Private Sub GetItemInfo(custGrpItemID As String)
        Using DB As New DBHelper
            Dim r As DataReader

            Dim email As String = GetUserEmail(DB)
            Dim custGrpID As String
            Dim noPrice As Boolean
            Dim csCost, eaCost, lbCost, gp As Decimal
            Dim btmcsPirce, btmeaPirce, btmlbPirce As Decimal
            Dim avgCsWgt As Decimal = 0
            Dim avgEaWgt As Decimal = 0
            Dim lbsPerEa As Decimal = 0
            Dim minSalCnt As Decimal



            'Clear item info
            clearItemDtl()
            clearQtyEntry()

            'Set properties
            If Session("SYS_SESSION_USERTYPE") = "CUSTOMER" OrElse Session("SYS_SESSION_USERTYPE") = "JFCADMIN" Then
                custGrpID = IIf(Session("SYS_SESSION_USERTYPE") = "CUSTOMER", Session("SYS_SESSION_CUSTGRPID"), GetCustGrpID(DB, lblCustomerID.Text))
                pnlNewOrderForm.Visible = IIf(GetNrItemGrpInCustGrp(DB, custGrpID) > 1, True, False)
                pnlNewVendor.Visible = IIf(GetNrCustGrpVendor(DB, custGrpID) > 1, True, False)
                pnlNewShowItemDtl.Visible = Common.ShowItemDtl(DB, lblCustomerID.Text)
                pnlNewShowPrice.Visible = IIf(Session("SYS_SESSION_USERTYPE") = "CUSTOMER" AndAlso Common.ApplyInEmailOrCustomer(DB, lblCustomerID.Text, "HidePrice") = True, False, True)
            End If

            'Get item info

            r = DB.GetReader(GetSQL_itemDtl(DB, lblCustomerID.Text, custGrpItemID, "", CString(ViewState("ORDER_ID"))))
            If r.Read() Then
                pnlAddItem.Visible = True
                imgNewItem.ImageUrl = GetItemImgUrl(CString(r("itemType")), CString(r("item_id")))
                imgNewItem_mobile.ImageUrl = GetItemImgUrl(CString(r("itemType")), CString(r("item_id")))
                lblNewCustGrpItemID.Text = CString(r("custGrpItem_id"))
                lblNewItemDesc.Text = CString(r("itemDesc_view"))
                lblNewJfcRemark.Text = CString(r("jfcRemark"))
                lblNewItemIdNr.Text = CString(r("itemIdNr_view"))
                'lblNewItemID.Text = CString(r("itemID"))
                lblNewItemGrpName.Text = CString(r("itemGrpName"))
                lblNewVendorName.Text = CString(r("vendorName"))
                lblNewCtgName.Text = CString(r("ctgName_view"))
                lblNewCsPrice.Text = Val(r("csprice"))
                lblNewCsPrice_view.Text = String.Format("{0:C}", Val(r("csprice"))) & " / cs"
                lblNewCsPrice_view.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewEaPrice.Text = Val(r("eaprice"))
                lblNewEaPrice_view.Text = String.Format("{0:C}", Val(r("eaprice"))) & IIf(Val(r("lbPrice_orderByQty")) <> 0, " / pc", " / ea")
                lblNewEaPrice_view.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewLbPrice_OrderByWgt.Text = Val(r("lbPrice_OrderByWgt"))
                lblNewLbPrice_OrderByWgt_view.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByWgt"))) & " / lb"
                lblNewLbPrice_OrderByWgt_view.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                lblNewLbPrice_OrderByQty.Text = Val(r("lbPrice_OrderByQty"))
                lblNewLbPrice_OrderByQty_view.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByQty"))) & " / lb"
                lblNewLbPrice_OrderByQty_view.Visible = IIf(Val(r("lbPrice_OrderByQty")) <> 0, True, False)
                If lblNewLbPrice_OrderByQty_view.Visible = True Then
                    lblNewCsPrice_view.Text = IIf(IsMobile(), "<br />", "") & lblNewCsPrice_view.Text
                    lblNewEaPrice_view.Text = lblNewEaPrice_view.Text
                End If
                lnkSetPrice.Visible = False
                lblNewAvailQty.Visible = False
                If ViewState("ORDER_CMD") = "JFCADMIN" AndAlso CString(r("jfcitem")) = "Y" Then
                    lnkSetPrice.Visible = True
                    lblNewAvailQty.Visible = True
                    lblNewAvailQty.Text = CString(r("qtyAvail"))

                    csCost = Val(r("csCost"))
                    btmcsPirce = Val(r("btmcsPrice"))
                    minSalCnt = IIf(Val(r("minSalCnt")) = 0, 1, Val(r("minSalCnt")))
                    lbsPerEa = GetJfcLbsPerEa(DB, lblCustomerID.Text, custGrpItemID)
                    Common.CovertPrice(csCost, eaCost, lbCost, r("eapercs"), minSalCnt, IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID), lbsPerEa, r("avgcswgt"), r("avgeawgt"))
                    Common.CovertPrice(btmcsPirce, btmeaPirce, btmlbPirce, r("eapercs"), minSalCnt, IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID), lbsPerEa, r("avgcswgt"), r("avgeawgt"))


                    lblNewGp.Text = IIf(gp <> 0, "<br />GP " & Math.Round(gp * 100, 1) & "%", "")

                    lblNewCsCost.Text = csCost 'Show this in set price popup (not add item panel)
                    lblNewEaCost.Text = eaCost 'Show this in set price popup (not add item panel)
                    lblNewLbCost.Text = lbCost 'Show this in set price popup (not add item panel)
                    lblNewBtmCs.Text = btmcsPirce
                    lblNewBtmEa.Text = btmeaPirce
                    lblNewBtmLb.Text = btmlbPirce
                End If
                lblNewMinSalCnt.Text = Val(r("minSalCnt"))
                lblNewMinSalCnt_view.Text = IIf(IsMobile(), "<br />", "") & "min order: " & Val(r("minSalCnt")) & " ea"
                lblNewMinSalCnt_view.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("minSalCnt")) > 1 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False) 'And Val(r("eaprice")) <> 0
                'lblNewEaPerCs.Text = Val(r("eaPerCs")) & " ea / cs"
                avgCsWgt = Val(r("avgcswgt")) 'GetAvgWgt(Val(r("csPrice")), Val(r("lbPrice_OrderByQty")))
                avgEaWgt = Val(r("avgeawgt")) 'GetAvgWgt(Val(r("eaPrice")), Val(r("lbPrice_OrderByQty")))
                lblNewAvgCsWgt.Text = String.Format("{0:N}", avgCsWgt) & " lbs / cs"
                lblNewAvgCsWgt.Visible = IIf(avgCsWgt <> 0, True, False) 'IIf(avgCsWgt <> 0 AndAlso Val(r("csprice")) <> 0, True, False)
                lblNewAvgEaWgt.Text = String.Format("{0:N}", avgEaWgt) & " lbs / pc"
                lblNewAvgEaWgt.Visible = IIf(avgEaWgt <> 0, True, False) 'IIf(avgEaWgt <> 0 AndAlso Val(r("eaprice")) <> 0, True, False)
                lblNewAvgWgtTitle.Visible = lblNewAvgCsWgt.Visible OrElse lblNewAvgEaWgt.Visible
                pnlNewEaPerCs.Visible = IIf(lblHideEaQty.Text = "N" AndAlso lblHideEaQty.Text = "N", True, False)
                lblNewEaPerCs.Text = CString(r("eaPerCs"))
                lblNewEaPerCs.Visible = IIf(Val(r("eaPerCs")) <> 0 AndAlso Val(r("eaPerCs")) <> 1 AndAlso Val(r("lbPrice_OrderByQty")) = 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                If pnlNewShowItemDtl.Visible = True Then
                    lblNewGrossWeight.Text = CString(r("grossWeight_lbs")) & " lbs, " & CString(r("grossWeight_kg")) & " kg"
                    'lblNewGrossWeight.Visible = IIf(Val(r("grossWeight_lbs")) = 0, False, True)
                    lblNewCaseVol.Text = CString(r("caseVol_ft3")) & " ft³, " & CString(r("caseVol_m3")) & " m³"
                    'lblNewCaseVol.Visible = IIf(Val(r("caseVol_ft3")) = 0, False, True)
                    'lblNewPallet.Text = Val(r("block")) & " x " & Val(r("high")) & " = " & Val(r("block")) * Val(r("high")) 'TODO: Add after JETS stores correct values
                    'lblNewPallet.Visible = IIf(Val(r("block")) = 0 AndAlso Val(r("high")) = 0, False, True)
                End If
                txtNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                txtNewEaQty.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewEaQty.Text = IIf(Val(r("lbPrice_orderByQty")) <> 0, "pc", "ea")
                lblNewEaQty.Visible = IIf(lblHideEaQty.Text = "N" AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                txtNewLbs.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                lblNewLbs.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
                txtNewCsQty.CssClass = IIf(Val(r("lastCsQty")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                txtNewEaQty.CssClass = IIf(Val(r("lastEaQty")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                txtNewLbs.CssClass = IIf(Val(r("lastLbs")) > 0, "cellQtyTxt_highlight", "cellQtyTxt")
                lblNewQtyHistory.Text = GetQtyHistory(DB, lblCustomerID.Text, custGrpItemID, 1)
                lblNewQtyHistoryTitle.Visible = IIf(lblNewQtyHistory.Text = String.Empty, False, True)
                lblNewQtyHistory.Visible = IIf(lblNewQtyHistory.Text = String.Empty, False, True)
                'btnQtyHistory.Visible = IIf(CString(r("webOrdered")) = "Y", True, False)
                'If CString(r("webOrdered")) = "Y" Then btnQtyHistory.ToolTip = GetQtyHistory(DB, lblCustomerID.Text, custGrpItemID, 10, True)
                'If CString(r("outOfStock")) = "Y" andalso Then btnAddItem.Visible = False
                noPrice = IIf(Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0 AndAlso Val(r("lbPrice_OrderByWgt")) = 0 AndAlso Val(r("lbPrice_OrderByQty")) = 0, True, False)
                lblNewOutOfStock.Visible = IIf(CString(r("outOfStock")) = "Y" AndAlso (ViewState("ORDER_CMD") = "JFCADMIN" OrElse noPrice = True), True, False)
                lblNewArrivDate.Text = "(Est. arrival: " & CString(r("arrivDate_fmt")) & ")"
                lblNewArrivDate.Visible = IIf(CString(r("arrivDate_fmt")) <> "" AndAlso (ViewState("ORDER_CMD") = "JFCADMIN" OrElse noPrice = True), True, False)
                btnAddItem.Visible = True
                If noPrice = True Then
                    If ViewState("ORDER_CMD") <> "JFCADMIN" Then
                        lblNewAskPrice.Visible = True
                    End If
                    btnAddItem.Visible = False
                End If
                lnkNewShowVpSp.Visible = False
                pnlNewVpSp.Visible = False
                If noPrice = False AndAlso ViewState("ORDER_CMD") = "JFCADMIN" AndAlso CString(r("jfcitem")) = "Y" Then
                    'lnkNewShowVpSp.Visible = True Comment out by YIFU 11/16/2020
                End If
                If CString(r("orderEaQty")) <> "" Then
                    'Item already exists in order
                    txtNewCsQty.Text = Val(r("orderCsQty"))
                    txtNewEaQty.Text = Val(r("orderEaQty"))
                    txtNewLbs.Text = Val(r("orderLbs"))
                    txtNewCsQty_vp.Text = Val(r("orderCsQty_vp"))
                    txtNewEaQty_vp.Text = Val(r("orderEaQty_vp"))
                    txtNewCsQty_sp.Text = Val(r("orderCsQty_sp"))
                    txtNewEaQty_sp.Text = Val(r("orderEaQty_sp"))
                    txtNewVpMemo.Text = CString(r("orderVpMemo"))
                    'If CString(r("outOfStock")) <> "Y" Then
                    btnAddItem.Text = IIf(ViewState("ORDER_CMD") = "JFCADMIN", "Update", "Update Qty")
                    lblNewMsg.Text = "This item is already in your order. You may change the qty."
                    'End If
                End If

            ElseIf GetDeleteCustomer_Item(DB, lblCustomerID.Text, custGrpItemID) > 0 Then
                ViewState("DIALOG_CMD") = "ITEM_RESTORE"
                ViewState("DIALOG_ARG1") = custGrpItemID
                MessageBoxYesNo("This item has been deleted from the order form. Would you like to restore?", "Item has been delete", DB)
                Exit Sub
            Else
                r.close()
                MessageBox("Item not found. Please select an item from the auto-complete list", "Error", DB)
                Exit Sub
            End If
            CheckItemAvail()

            'Show item list

            r.close()
            FocusVisibleQty(txtNewCsQty, txtNewEaQty, txtNewLbs)
        End Using
    End Sub

    Private Sub setItemPrice()
        Dim scaledItem As Boolean
        Dim eaUnit As String

        Using DB As New DBHelper
            If ViewState("ORDER_CMD") <> "JFCADMIN" OrElse IsJfcItem(DB, lblNewCustGrpItemID.Text) = False Then Exit Sub
            'Init
            ClearCtrl({txtPopPrice})
            ClearCtrl({lblPopMsg_setPrice})
            'Check if it's scaled item
            scaledItem = IsJfcScaledItem(DB, lblCustomerID.Text, lblNewCustGrpItemID.Text)
            eaUnit = IIf(scaledItem = True, "pc", "ea")
            'Show price unit (don't show / pc)
            lstPopPriceUnit.Items.Clear()
            If Val(lblNewLbCost.Text) > 0 AndAlso scaledItem = True Then lstPopPriceUnit.Items.Add(New ListItem(" / lb", "LB"))
            If Val(lblNewCsCost.Text) > 0 Then lstPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
            If Val(lblNewEaCost.Text) > 0 AndAlso scaledItem = False Then lstPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))
            'radPopPriceUnit.Items.Clear()
            'radPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
            'If showEa = True Then radPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))
            'radPopPriceUnit.Text = "CS"

            'Show popup
            lblPopCost.Text = IIf(Val(lblNewLbCost.Text) > 0 AndAlso scaledItem = True, String.Format("{0:C}", Val(lblNewLbCost.Text)) & " / lb, ", "") & _
                                IIf(Val(lblNewCsCost.Text) > 0, String.Format("{0:C}", Val(lblNewCsCost.Text)) & " / cs, ", "") & _
                                IIf(Val(lblNewEaCost.Text) > 0 AndAlso scaledItem = False, String.Format("{0:C}", Val(lblNewEaCost.Text)) & " / " & eaUnit, "") 'Don't show cost / pc

            lblPopBtm.Text = IIf(Val(lblNewBtmLb.Text) > 0 AndAlso scaledItem = True, String.Format("{0:C}", Val(lblNewBtmLb.Text)) & " / lb, ", "") & _
                                IIf(Val(lblNewBtmCs.Text) > 0, String.Format("{0:C}", Val(lblNewBtmCs.Text)) & " / cs, ", "") & _
                                IIf(Val(lblNewBtmEa.Text) > 0 AndAlso scaledItem = False, String.Format("{0:C}", Val(lblNewBtmEa.Text)) & " / " & eaUnit, "") 'Don't show cost / pc
            popDialog_setPrice.Show()


        End Using
    End Sub
    Protected Sub lnkSetPrice_Click(sender As Object, e As System.EventArgs) Handles lnkSetPrice.Click
        'Dim showEa As Boolean = IIf(Val(lblNewEaPerCs.Text) > 1 AndAlso Val(lblNewMinSalCnt.Text) <> Val(lblNewEaPerCs.Text), True, False)
        Dim scaledItem As Boolean
        Dim eaUnit As String

        Using DB As New DBHelper
            If ViewState("ORDER_CMD") <> "JFCADMIN" OrElse IsJfcItem(DB, lblNewCustGrpItemID.Text) = False Then Exit Sub
            'Init
            ClearCtrl({txtPopPrice})
            ClearCtrl({lblPopMsg_setPrice})
            'Check if it's scaled item
            scaledItem = IsJfcScaledItem(DB, lblCustomerID.Text, lblNewCustGrpItemID.Text)
            eaUnit = IIf(scaledItem = True, "pc", "ea")
            'Show price unit (don't show / pc)
            lstPopPriceUnit.Items.Clear()
            If Val(lblNewLbCost.Text) > 0 AndAlso scaledItem = True Then lstPopPriceUnit.Items.Add(New ListItem(" / lb", "LB"))
            If Val(lblNewCsCost.Text) > 0 Then lstPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
            If Val(lblNewEaCost.Text) > 0 AndAlso scaledItem = False Then lstPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))
            'radPopPriceUnit.Items.Clear()
            'radPopPriceUnit.Items.Add(New ListItem(" / cs", "CS"))
            'If showEa = True Then radPopPriceUnit.Items.Add(New ListItem(" / " & eaUnit, "EA"))
            'radPopPriceUnit.Text = "CS"

            'Show popup
            lblPopCost.Text = IIf(Val(lblNewLbCost.Text) > 0 AndAlso scaledItem = True, String.Format("{0:C}", Val(lblNewLbCost.Text)) & " / lb, ", "") & _
                                IIf(Val(lblNewCsCost.Text) > 0, String.Format("{0:C}", Val(lblNewCsCost.Text)) & " / cs, ", "") & _
                                IIf(Val(lblNewEaCost.Text) > 0 AndAlso scaledItem = False, String.Format("{0:C}", Val(lblNewEaCost.Text)) & " / " & eaUnit, "") 'Don't show cost / pc

            lblPopBtm.Text = IIf(Val(lblNewBtmLb.Text) > 0 AndAlso scaledItem = True, String.Format("{0:C}", Val(lblNewBtmLb.Text)) & " / lb, ", "") & _
                                IIf(Val(lblNewBtmCs.Text) > 0, String.Format("{0:C}", Val(lblNewBtmCs.Text)) & " / cs, ", "") & _
                                IIf(Val(lblNewBtmEa.Text) > 0 AndAlso scaledItem = False, String.Format("{0:C}", Val(lblNewBtmEa.Text)) & " / " & eaUnit, "") 'Don't show cost / pc
            popDialog_setPrice.Show()
            CheckItemAvail()

        End Using
    End Sub

    'Update price for only JFC item
    Protected Sub btnPopUpdatePrice_Click(sender As Object, e As System.EventArgs) Handles btnPopUpdatePrice.Click
        Dim SQL As String = ""
        Dim r As DataReader
        Dim csPrice, eaPrice, lbPrice As Decimal
        Dim avgCsWgt, avgEaWgt As Decimal
        Dim noPrice As Boolean
        Dim gp As Decimal

        Using DB As New DBHelper
            If ViewState("ORDER_CMD") <> "JFCADMIN" OrElse IsJfcItem(DB, lblNewCustGrpItemID.Text) = False Then Exit Sub

            'Check error
            If Val(txtPopPrice.Text) <= 0 Then
                lblPopMsg_setPrice.Text = "Price has to be positive."
                popDialog_setPrice.Show()
                CheckItemAvail()
                Exit Sub
            End If


            'If lstPopPriceUnit.Text = "CS" Then
            '    csPrice = Val(txtPopPrice.Text)
            '    SQL = "SELECT p.eaPerCs, p.minSalCnt, p.scaledItem, p.avgCsWgt, p.avgEaWgt, p.outOfStock, p.arrivDate, p.csAvail, p.eaAvail, " & _
            '            "NVL(p.csCost, 0) AS csCost, " & _
            '            csPrice & " AS csPrice, " & _
            '            "CASE WHEN p.eaPerCs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
            '            "  WHEN p.scaledItem = 'Y' AND nvl(p.avgcswgt,0) < nvl(p.avgeawgt,0) * 2 THEN 0 " & _
            '            "  WHEN p.scaledItem = 'Y' THEN ROUND(" & csPrice & " / p.eapercs / DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * nvl(p.avgeawgt, 0.9), 2) " & _
            '            "  ELSE ROUND(" & csPrice & " / p.eaPerCs, 2) END AS eaPrice, " & _
            '            "CASE WHEN p.scaledItem = 'N' OR p.eapercs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
            '            "  ELSE ROUND(" & csPrice & " / p.eaPerCs / DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)), 2) END AS lbPrice_orderByQty " & _
            '            "FROM jets_price p, jets_customer jc, customer c " & _
            '            "WHERE jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '            "  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            '    '"  WHEN p.scaledItem = 'Y' AND nvl(p.avgcswgt,0) < nvl(p.avgeawgt,0) * 2 THEN 0 " & 'Add on 8/31/2020
            '    'csPrice & " AS csPrice, " & _
            '    '"CASE WHEN p.eaPerCs <= 1 THEN 0 ELSE " & _
            '    '"  CASE WHEN p.scaledItem = 'Y' THEN " & _
            '    '"    (CASE WHEN p.eapercs = 10 then round(" & csPrice & " * nvl(p.avgeawgt, 0.9) / nvl(p.avgcswgt, 9.9), 2) ELSE ROUND(ROUND(" & csPrice & "/p.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
            '    '"  ELSE " & _
            '    '"    ROUND(" & csPrice & " / p.eaPerCs, 2) " & _
            '    '"  END " & _
            '    '"END AS eaPrice, " & _
            '    '"CASE WHEN p.scaledItem = 'N' THEN 0 ELSE (CASE WHEN p.eaPerCs = 10 THEN ROUND(" & csPrice & " / nvl(p.avgcswgt, 9.9), 2) ELSE ROUND(ROUND(" & csPrice & "/p.eapercs, 3)*10, 2) END) END AS lbPrice_orderByQty " & _
            '    '"FROM jets_customer_price cp, jets_price p, jets_customer jc, customer c " & _
            '    '"WHERE cp.bid = jc.bid And cp.itemcode = p.itemcode And cp.customerid = jc.customerid And jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '    '"  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            'ElseIf lstPopPriceUnit.Text = "EA" Then
            '    eaPrice = Val(txtPopPrice.Text)
            '    SQL = "SELECT p.eaPerCs, p.minSalCnt, p.scaledItem, p.avgCsWgt, p.avgEaWgt, p.outOfStock, p.arrivDate, p.csAvail, p.eaAvail, " & _
            '            "NVL(p.csCost, 0) AS csCost, " & _
            '            "CASE WHEN p.scaledItem = 'Y' and DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN ROUND(" & eaPrice & " * p.eapercs * DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) / nvl(p.avgeawgt, 0.9) * nvl(p.avgcswgt, 0), 2) " & _
            '            "  ELSE ROUND(" & eaPrice & " * p.eapercs * DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) / nvl(p.avgeawgt, 0.9), 2) END AS csPrice, " & _
            '            eaPrice & " AS eaPrice, " & _
            '            "CASE WHEN p.scaledItem = 'N' OR p.eapercs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
            '            "  ELSE ROUND(" & eaPrice & " / DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) , 2) END AS lbPrice_orderByQty " & _
            '            "FROM jets_price p, jets_customer jc, customer c " & _
            '            "WHERE jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '            "  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            '    '"CASE WHEN p.scaledItem = 'Y' and DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN 0 " & 'Mod on 8/31/2020
            '    'eaPrice & " AS eaPrice, " & _
            '    '"CASE WHEN p.eaPerCs <= 1 THEN 0 ELSE " & _
            '    '"  CASE WHEN p.scaledItem = 'Y' THEN " & _
            '    '"    CASE WHEN nvl(p.lbsPerEa, 0.1) * p.eaPerCs = 1 THEN 0 ELSE " & _
            '    '"      (CASE WHEN p.eapercs = 10 then round(" & eaPrice & " * nvl(p.avgcswgt, 9.9) / nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(" & eaPrice & "*p.eapercs, 3)/nvl(p.avgeawgt, 0.9) / 10, 2) END) " & _
            '    '"    END " & _
            '    '"  ELSE " & _
            '    '"    ROUND(" & eaPrice & " * p.eaPerCs, 2) " & _
            '    '"  END " & _
            '    '"END AS csPrice, " & _
            '    '"CASE WHEN p.scaledItem = 'N' THEN 0 ELSE (CASE WHEN p.eaPerCs <= 1 THEN 0 WHEN p.eaPerCs = 10 THEN ROUND(" & eaPrice & " / nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(" & eaPrice & " / nvl(p.avgeawgt, 0.9), 2) END) END AS lbPrice_orderByQty " & _
            '    '"FROM jets_customer_price cp, jets_price p, jets_customer jc, customer c " & _
            '    '"WHERE cp.bid = jc.bid And cp.itemcode = p.itemcode And cp.customerid = jc.customerid And jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '    '"  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            'ElseIf lstPopPriceUnit.Text = "LB" Then
            '    lbPrice = Val(txtPopPrice.Text)
            '    SQL = "SELECT p.eaPerCs, p.minSalCnt, p.scaledItem, p.avgCsWgt, p.avgEaWgt, p.outOfStock, p.arrivDate, p.csAvail, p.eaAvail, " & _
            '            "NVL(p.csCost, 0) AS csCost, " & _
            '            "CASE WHEN p.scaledItem = 'Y' and DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN ROUND(" & lbPrice & " * p.eapercs * DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * nvl(p.avgcswgt, 0), 2) " & _
            '            "  ELSE ROUND(" & lbPrice & " * p.eapercs * DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)), 2) END AS csPrice, " & _
            '            "CASE WHEN p.eaPerCs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
            '            "  WHEN p.scaledItem = 'Y' AND nvl(p.avgcswgt,0) < nvl(p.avgeawgt,0) * 2 THEN 0 " & _
            '            "  WHEN p.scaledItem = 'Y' THEN ROUND(" & lbPrice & " * nvl(p.avgeawgt, 0.9), 2) " & _
            '            "  ELSE ROUND(" & lbPrice & " * DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)), 2) END AS eaPrice, " & _
            '            lbPrice & " AS lbPrice_orderByQty " & _
            '            "FROM jets_price p, jets_customer jc, customer c " & _
            '            "WHERE jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '            "  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            '    '"CASE WHEN p.scaledItem = 'Y' and DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN 0 " & _ 'Mod on 8/31/2020
            '    '"  WHEN p.scaledItem = 'Y' AND nvl(p.avgcswgt,0) < nvl(p.avgeawgt,0) * 2 THEN 0 " & 'Add on 8/31/2020
            '    '"CASE WHEN p.scaledItem = 'Y' THEN " & _
            '    '"  CASE WHEN nvl(p.lbsPerEa, 0.1) * p.eaPerCs = 1 THEN 0 ELSE " & _
            '    '"    (CASE WHEN p.eapercs = 10 THEN ROUND(" & lbPrice & "*nvl(p.avgcswgt, 9.9), 2) ELSE ROUND(ROUND(" & lbPrice & "*p.eapercs, 3)/10, 2) END) " & _
            '    '"  END " & _
            '    '"ELSE 0 " & _
            '    '"END AS csPrice, " & _
            '    '"CASE WHEN p.eaPerCs <= 1 THEN 0 ELSE " & _
            '    '"  CASE WHEN p.scaledItem = 'Y' THEN " & _
            '    '"    (CASE WHEN p.eapercs = 10 then round(" & lbPrice & "*nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(" & lbPrice & "*nvl(p.avgeawgt, 0.9), 2) END) " & _
            '    '"  ELSE 0 " & _
            '    '"  END " & _
            '    '"END AS eaPrice, " & _
            '    'lbPrice & " AS lbPrice_orderByQty " & _
            '    '"FROM jets_customer_price cp, jets_price p, jets_customer jc, customer c " & _
            '    '"WHERE cp.bid = jc.bid And cp.itemcode = p.itemcode And cp.customerid = jc.customerid And jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
            '    '"  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)) & " AND c.customer_id = " & DB.SqlEsc(lblCustomerID.Text)
            'End If
            'YI FU 
            SQL = SetPrice(lstPopPriceUnit.Text, Val(txtPopPrice.Text), DB.SqlEsc(GetItemID(DB, lblNewCustGrpItemID.Text)), DB.SqlEsc(lblCustomerID.Text))
            r = DB.GetReader(SQL)
            If r.Read() Then
                If Val(r("csprice")) < Val(lblNewBtmCs.Text) Then
                    lblPopMsg_setPrice.Text = "Price has to be larger than bottom price."
                    popDialog_setPrice.Show()
                    CheckItemAvail()
                    Exit Sub
                End If
                lblNewCsPrice.Text = Val(r("csprice"))
                lblNewCsPrice_view.Text = String.Format("{0:C}", Val(r("csprice"))) & " / cs"
                lblNewCsPrice_view.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewEaPrice.Text = Val(r("eaprice"))
                lblNewEaPrice_view.Text = String.Format("{0:C}", Val(r("eaprice"))) & IIf(Val(r("lbPrice_orderByQty")) <> 0, " / pc", " / ea")
                lblNewEaPrice_view.Visible = IIf(Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewLbPrice_OrderByQty.Text = Val(r("lbPrice_OrderByQty"))
                lblNewLbPrice_OrderByQty_view.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByQty"))) & " / lb"
                lblNewLbPrice_OrderByQty_view.Visible = IIf(Val(r("lbPrice_OrderByQty")) <> 0, True, False)
                If lblNewLbPrice_OrderByQty_view.Visible = True Then
                    'Scaled item
                    lblNewCsPrice_view.Text = IIf(IsMobile(), "<br />", "") & lblNewCsPrice_view.Text
                    lblNewEaPrice_view.Text = lblNewEaPrice_view.Text
                    'If Val(r("lbPrice_OrderByQty")) = 0 Then gp = 1 Else gp = (Val(r("lbPrice_OrderByQty")) - Val(r("csCost"))) / Val(r("lbPrice_OrderByQty")) 'Val(r("csCost")) = cost / lb for scaled item
                    If Val(r("lbPrice_OrderByQty")) = 0 Then gp = 1 Else gp = (Val(r("lbPrice_OrderByQty")) - Val(lblNewLbCost.Text)) / Val(r("lbPrice_OrderByQty"))
                    lblNewGp.Text = IIf(gp <> 1, "<br />GP " & Math.Round(gp * 100, 1) & "%", "")
                Else
                    'Regular item
                    If Val(r("csPrice")) = 0 Then gp = 1 Else gp = (Val(r("csPrice")) - Val(r("csCost"))) / Val(r("csPrice"))
                    lblNewGp.Text = IIf(gp <> 1, "GP " & Math.Round(gp * 100, 1) & "%", "")
                End If
                lblNewEaPerCs.Visible = IIf(Val(r("eaPerCs")) <> 0 AndAlso Val(r("eaPerCs")) <> 1 AndAlso Val(r("lbPrice_OrderByQty")) = 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                avgCsWgt = Val(r("avgcswgt")) 'GetAvgWgt(Val(r("csPrice")), Val(r("lbPrice_OrderByQty")))
                avgEaWgt = Val(r("avgeawgt")) 'GetAvgWgt(Val(r("eaPrice")), Val(r("lbPrice_OrderByQty")))
                lblNewAvgCsWgt.Text = String.Format("{0:N}", avgCsWgt) & " lbs / cs"
                lblNewAvgCsWgt.Visible = IIf(avgCsWgt <> 0 AndAlso (Val(r("csprice")) <> 0 OrElse (Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0)), True, False)
                lblNewAvgEaWgt.Text = String.Format("{0:N}", avgEaWgt) & " lbs / pc"
                lblNewAvgEaWgt.Visible = IIf(avgEaWgt <> 0 AndAlso (Val(r("eaprice")) <> 0 OrElse (Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0)), True, False)
                lblNewAvgWgtTitle.Visible = lblNewAvgCsWgt.Visible OrElse lblNewAvgEaWgt.Visible
                txtNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                lblNewCsQty.Visible = IIf(Val(r("csprice")) <> 0, True, False)
                txtNewEaQty.Visible = IIf(Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                lblNewEaQty.Text = IIf(Val(r("lbPrice_orderByQty")) <> 0, "pc", "ea")
                lblNewEaQty.Visible = IIf(Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
                noPrice = IIf(Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0 AndAlso Val(r("lbPrice_OrderByWgt")) = 0 AndAlso Val(r("lbPrice_OrderByQty")) = 0, True, False)
                btnAddItem.Visible = True
                pnlNewVpSp.Visible = False
                If noPrice = True Then
                    btnAddItem.Visible = False
                Else
                    'lnkNewShowVpSp.Visible = True Comment out by YIFU 11/16/2020
                End If
            End If
            r.close()
            CheckItemAvail()
            Page.MaintainScrollPositionOnPostBack = False
            btnAddItem.Focus()
            txtNewCsQty.Focus()
        End Using
    End Sub

    Protected Sub lnkNewShowVpSp_Click(sender As Object, e As System.EventArgs) Handles lnkNewShowVpSp.Click
        Using DB As New DBHelper
            If ViewState("ORDER_CMD") <> "JFCADMIN" OrElse IsJfcItem(DB, lblNewCustGrpItemID.Text) = False Then Exit Sub
            lnkNewShowVpSp.Visible = False
            pnlNewVpSp.Visible = True
            txtNewCsQty_vp.Visible = txtNewCsQty.Visible
            lblNewCsQty_vp.Visible = lblNewCsQty.Visible
            txtNewEaQty_vp.Visible = txtNewEaQty.Visible
            lblNewEaQty_vp.Visible = lblNewEaQty.Visible
            txtNewCsQty_sp.Visible = txtNewCsQty.Visible
            lblNewCsQty_sp.Visible = lblNewCsQty.Visible
            txtNewEaQty_sp.Visible = txtNewEaQty.Visible
            lblNewEaQty_sp.Visible = lblNewEaQty.Visible
            txtNewVpMemo.Visible = True
        End Using
        CheckItemAvail()

    End Sub

    'Protected Sub btnQtyHistory_Click(sender As Object, e As System.EventArgs) Handles btnQtyHistory.Click
    '    Using DB As New DBHelper
    '        MessageBox(GetQtyHistory(DB, lblCustomerID.Text, lblNewCustGrpItemID.Text), "Ordered Qty History", DB)
    '    End Using
    'End Sub

    Protected Sub btnAddItem_Click(sender As Object, e As System.EventArgs) Handles btnAddItem.Click
        Using DB As New DBHelper
            'If qty is changed, confirm; otherwise add the item
            If lblTextChanged.Value = "YES" Then
                'Confirm
                ViewState("DIALOG_CMD") = "TEXT_CHANGED"
                MessageBoxYesNo("Changes in qty will be lost if you proceed. Are you sure you want to continue?", "Qty has been changed", DB)
                Exit Sub
            Else
                addItemInOrder()
            End If
        End Using
        CheckItemAvail()

    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As System.EventArgs) Handles btnUpdate.Click, btnConfirmAndEmail.Click ' btnConfirm.Click, 
        Using DB As New DBHelper
            Dim txtCsQty, txtEaQty, txtLbs, txtCsQty_vp, txtEaQty_vp, txtCsQty_sp, txtEaQty_sp, txtVpMemo As TextBox
            Dim lbl As Label
            Dim lnk As LinkButton
            Dim webOrderID As String = ViewState("ORDER_ID")
            Dim emailBody As String = ""
            Dim errMsg As String = ""
            Dim userEmailFailed As Boolean = False
            Dim vendorEmailFailed As Boolean = False
            Dim userEmail As String = GetUserEmail(DB)
            Dim customerEmail As String
            Dim minSalCnt As Integer
            Dim itemIdNr_view As String
            Dim subject As String
            'Dim orderState As String
            Dim vendorEmail, custOrderNr As String
            'Dim anytimeDelivery As Boolean = ApplyInCustomer(DB, lblCustomerID.Text, "AnytimeDelivery")
            Dim deliveryDateRequired As Boolean = Not (ApplyInCustomer(DB, lblCustomerID.Text, "AnytimeDelivery") AndAlso lblJfcBranchNr.Text = "05") 'Not required for EXP
            Dim jfcBranchNr As String = ""

            GetJFCBranchAndCustomerNr(DB, lblCustomerID.Text, jfcBranchNr, Nothing)
            CheckItemAvail()

            If canEditOrder(DB, webOrderID, errMsg) = False Then
                MessageBox(errMsg, "Error", DB)
                Exit Sub
            End If
            If GetNrItemInOrder(DB, webOrderID) = 0 Then
                MessageBox("Order is empty. Please add at least one item to your order.", "Error", DB)
                Exit Sub
            End If
            'orderState = GetOrderState(DB, webOrderID)
            'If ViewState("ORDER_CMD") = "CUST" Then
            '    If orderState <> "N" Then
            '        'Cannot edit if order state NOT IN ('N') for customer
            '        MessageBox("You cannot edit this order because this order has been " & IIf(orderState = "D", "canceled", "processed") & ".", "Error", DB)
            '        Exit Sub
            '    End If
            If ViewState("ORDER_CMD") = "JFCADMIN" Then
                'If orderState = "D" OrElse orderState = "S" OrElse orderState = "I" Then
                '    'Cannot edit if order state IN ('D','S','I') for JFC admin
                '    MessageBox("You cannot edit this order because this order has been " & GetCodeMaster(DB, "ORDERSTATE", orderState) & ".", "Error", DB)
                '    Exit Sub
                'End If
                ''If anytimeDelivery = False OrElse (anytimeDelivery = True AndAlso txtDeliveryDate.Text <> String.Empty) Then
                If deliveryDateRequired = True OrElse (deliveryDateRequired = False AndAlso txtDeliveryDate.Text <> String.Empty) Then
                    'Check delivery date
                    Dim dateVal As Date = Nothing
                    If DateTime.TryParseExact(txtDeliveryDate.Text, DATE_FMT, Nothing, Nothing, dateVal) = False Then
                        MessageBox("Please enter ship date in " & DATE_FMT.ToLower & " format.", "Error", DB)
                        Exit Sub
                    End If
                    If dateVal < FormatDateTime(Now.AddDays(GetServerDayOffset(DB, jfcBranchNr)), Microsoft.VisualBasic.DateFormat.ShortDate) Then 'Convert to date format (e.g. 6/11/2020) without time
                        MessageBox("Ship date cannot be in the past.", "Error", DB)
                        Exit Sub
                    End If
                End If
            End If

            For i As Integer = 0 To grdItem_edit.Rows.Count - 1
                lnk = grdItem_edit.Rows.Item(i).FindControl("lnkItemIdNrView")
                itemIdNr_view = lnk.Text
                'Check qty
                txtCsQty = grdItem_edit.Rows.Item(i).FindControl("txtCsQty")
                txtEaQty = grdItem_edit.Rows.Item(i).FindControl("txtEaQty")
                txtLbs = grdItem_edit.Rows.Item(i).FindControl("txtLbs")
                txtCsQty_vp = grdItem_edit.Rows.Item(i).FindControl("txtCsQty_vp")
                txtEaQty_vp = grdItem_edit.Rows.Item(i).FindControl("txtEaQty_vp")
                txtCsQty_sp = grdItem_edit.Rows.Item(i).FindControl("txtCsQty_sp")
                txtEaQty_sp = grdItem_edit.Rows.Item(i).FindControl("txtEaQty_sp")
                txtVpMemo = grdItem_edit.Rows.Item(i).FindControl("txtVpMemo")
                If IsNumber(txtCsQty.Text, True) = False OrElse IsNumber(txtEaQty.Text, True) = False OrElse IsNumber(txtLbs.Text, True) = False _
                        OrElse IsNumber(txtCsQty_vp.Text, True) = False OrElse IsNumber(txtEaQty_vp.Text, True) = False OrElse IsNumber(txtCsQty_sp.Text, True) = False OrElse IsNumber(txtEaQty_sp.Text, True) = False Then
                    MessageBox("Qty for item #" & itemIdNr_view & " has to be 0 or a positive number", "Error", DB)
                    Exit Sub
                End If



                Dim lblbottom As Label = grdItem_edit.Rows.Item(i).FindControl("lblbtmCs")
                Dim lblprice As Label = grdItem_edit.Rows.Item(i).FindControl("lblCsPrice")
                If Val(lblprice.Text) < Val(lblbottom.Text) Then
                    MessageBox("Price " & lblprice.Text & " for item #" & itemIdNr_view & " has to be larger than the bottom price " & lblbottom.Text, "Error", DB)
                    Exit Sub
                End If


                'If (Val(txtCsQty_vp.Text) + Val(txtCsQty_sp.Text)) > Val(txtCsQty.Text) OrElse (Val(txtEaQty_vp.Text) + Val(txtEaQty_sp.Text)) > Val(txtEaQty.Text) Then
                '    MessageBox("Free qty for item #" & itemIdNr_view & " cannnot exceed ordered qty", "Error", DB)
                '    Exit Sub
                'End If
                lbl = grdItem_edit.Rows.Item(i).FindControl("lblMinSalCnt")
                minSalCnt = IIf(Val(lbl.Text) = 0, 1, Val(lbl.Text))
                If Val(txtEaQty.Text) <> 0 AndAlso Val(txtEaQty.Text) Mod minSalCnt > 0 Then
                    MessageBox("Each qty for item #" & itemIdNr_view & " has to be 0 or multiple of " & minSalCnt & ".", "Error", DB)
                    Exit Sub
                End If
                If (Val(txtCsQty_vp.Text) > 0 OrElse Val(txtEaQty_vp.Text) > 0) AndAlso txtVpMemo.Text = String.Empty Then
                    MessageBox("Please enter VP memo for item #" & itemIdNr_view, "Error", DB)
                    Exit Sub
                End If
            Next

            'Update order header
            updateOrder(webOrderID, IIf(sender.id = "btnConfirmAndEmail", True, False)) 'sender.id = "btnConfirm" OrElse 
            customerEmail = GetOrderEmail(DB, webOrderID, "CUSTOMER")
            vendorEmail = GetOrderEmail(DB, webOrderID, "VENDOR")
            custOrderNr = GetCustOrderNr(DB, webOrderID)
            If ViewState("ORDER_CMD") = "CUST" Then

                ci = New CultureInfo(Common.GetLangauge(DB, userEmail))

                Thread.CurrentThread.CurrentCulture = ci
                Thread.CurrentThread.CurrentUICulture = ci
                'Get email body for customer from email template in user control
                CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", "UPDATED", txtEmailMsg.Text)
                'Send email to customer her / his self
               
                '"Your order '" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty) & "was updated"

                subject = String.Format(rm.GetString("Yourorderwasupdated", ci), "'" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(" & rm.GetString("CustPO", ci) & custOrderNr & ") ", String.Empty))
                If Common.SendMail(SYSTEM_EMAIL, userEmail, vendorEmail, subject, emailBody, Nothing, True, errMsg) > 0 Then
                    userEmailFailed = True
                Else
                    'Mark email as sent
                    UpSertOrderEmail(DB, webOrderID, "CUSTOMER", "S", userEmail)
                End If

                Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
                Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")
                If SendOrderEmailToVendor(DB, webOrderID) = True Then
                    'Get email body for vendor from email template in user control
                    CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "VENDOR", "UPDATED", txtEmailMsg.Text)
                    'Send email from customer to vendor
                    If Common.SendMail(SYSTEM_EMAIL, vendorEmail, userEmail, "[Action Required] You received a updated order '" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty), emailBody, Nothing, True, errMsg) > 0 Then
                        vendorEmailFailed = True
                    Else
                        'Mark email state back to sent even if the email was received
                        UpSertOrderEmail(DB, webOrderID, "VENDOR", "S", userEmail)
                    End If
                End If
                If userEmailFailed = False AndAlso vendorEmailFailed = False Then
                    'Email sent ok
                    InsertEventLog(DB, "Saved order and sent confirmation email", webOrderID, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully updated. Confirmation email for this update has been sent to you and your sales rep.")
                Else
                    'Email sent failed
                    InsertEventLog(DB, "Saved order but failed to send confirmation email to your sales rep", webOrderID, Nothing, Nothing, True)
                    'If userEmailFailed = True Then errMsg &= "(1) Confirmation email could not be sent to you because " & userEmailErrMsg.ToLower & ". "
                    If vendorEmailFailed = True Then
                        errMsg = "Successfully updated, but confirmation email could not be sent to your sales rep. Please resend the email or contact your sales rep."
                        MessageBox(errMsg, "Warning", DB)
                    End If
                End If
            ElseIf ViewState("ORDER_CMD") = "JFCADMIN" Then
                'If sender.id = "btnConfirm" Then
                '    'No need to email customer
                '    InsertEventLog(DB, "Saved order", webOrderID, Nothing, Nothing, True)
                '    MessageLine("CONFIRM", "Successfully saved")
                'Else
                'Get email body for customer from email template in user control
                ci = New CultureInfo(Common.GetLangauge(DB, customerEmail))

                Thread.CurrentThread.CurrentCulture = ci
                Thread.CurrentThread.CurrentUICulture = ci


                CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", IIf(sender.id = "btnConfirmAndEmail", "APPROVED_ORDER", "UPDATED"), txtEmailMsg.Text)
                'Send email from JFC Admin to customer

                If sender.id = "btnConfirmAndEmail" Then
                    subject = String.Format(rm.GetString("KocusAPIMsg10", ci), "'" & webOrderID & "' ", custOrderNr)
                    If custOrderNr = String.Empty Then subject = Regex.Replace(subject, "\([^()]*\)", String.Empty)
                    subject = subject.Replace("  ", " ")
                Else
                    subject = String.Format(rm.GetString("Yourorderwasupdated", ci), "'" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(" & rm.GetString("CustPO", ci) & custOrderNr & ") ", String.Empty))
                End If
                '"Your order '" & webOrderID & "' " &
                '                       IIf(custOrderNr <> String.Empty,
                '                          "(Cust PO #" & custOrderNr & ") ", String.Empty) & "was " & IIf(sender.id = "btnConfirmAndEmail", "processed", "updated"),

                If Common.SendMail(SYSTEM_EMAIL, customerEmail, vendorEmail, subject,
                                        emailBody, Nothing, True, errMsg, vendorEmail) > 0 Then
                    userEmailFailed = True
                Else
                    'Mark email as sent
                    UpSertOrderEmail(DB, webOrderID, "CUSTOMER", "S", userEmail)
                End If
                If userEmailFailed = False Then
                    'Email sent ok
                    InsertEventLog(DB, "Saved order and sent confirmation email", webOrderID, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully saved. Notification email has been sent to your customer.")
                Else
                    'Email sent failed
                    InsertEventLog(DB, "Saved order but failed to send notification email to your customer", webOrderID, Nothing, Nothing, True)
                    errMsg = "Successfully updated, but notification email could not be sent to your customer. Please resend the email."
                    MessageBox(errMsg, "Warning", DB)
                End If
                'End If
            End If
            Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")
            CheckItemAvail()
            'Refresh order header and item list
            'showOrderHdr()
            'grdItem_RowDataCommand(Nothing, Nothing)

            'Go back to order history
            Session("ORDERDTL_RETURNCMD") = "UPDATED"
        End Using
    End Sub

    Protected Sub btnEmail_Click(sender As Object, e As System.EventArgs) Handles btnEmailUser_edit.Click, btnEmailUser_view.Click, btnEmailOther_edit.Click, btnEmailOther_view.Click ', btnEmailUser_jetsView.Click
        Dim emailBody As String = ""
        Dim errMsg As String = ""
        Dim toEmail As String
        Dim replyToEmail As String
        Dim sendToCustomer As Boolean
        Dim webOrderID As String = ViewState("ORDER_ID")
        Dim custOrderNr, orderIdNr As String

        Session("ORDERDTL_RETURNCMD") = "EMAIL"
        Using DB As New DBHelper
            custOrderNr = GetCustOrderNr(DB, webOrderID)
            orderIdNr = "'" & webOrderID & "' " & IIf(custOrderNr <> String.Empty, "(Cust PO #" & custOrderNr & ") ", String.Empty)
            sendToCustomer = IIf((ViewState("ORDER_CMD") = "CUST" AndAlso (sender.id = "btnEmailUser_edit" OrElse sender.id = "btnEmailUser_view")) _
                                    OrElse (ViewState("ORDER_CMD") = "JFCADMIN" AndAlso (sender.id = "btnEmailOther_edit" OrElse sender.id = "btnEmailOther_view")), True, False)
            'Send email to customer
            toEmail = GetOrderEmail(DB, webOrderID, IIf(sendToCustomer = True, "CUSTOMER", "VENDOR"))
            replyToEmail = GetOrderEmail(DB, webOrderID, IIf(sendToCustomer = True, "VENDOR", "CUSTOMER"))
            CreateEmailBody(Page, emailBody, "ORDER", webOrderID, IIf(sendToCustomer = True, "CUSTOMER", "VENDOR"), "RESEND")
            If Common.SendMail(SYSTEM_EMAIL, toEmail, replyToEmail, IIf(sendToCustomer = True, "Your order with JFC " & orderIdNr, "Web order " & orderIdNr & " from " & GetOrderCustName(DB, webOrderID)), emailBody, Nothing, True, errMsg) > 0 Then
                'MessageBox("Email could not be sent to you because " & errMsg.ToLower & ". ", "Error", DB)
                MessageBox("Email could not be sent. ", "Error", DB)
                Exit Sub
            End If
            'Mark email as sent if it hadn't been sent and increase sent count
            UpSertOrderEmail(DB, webOrderID, IIf(sendToCustomer = True, "CUSTOMER", "VENDOR"), "RESEND", toEmail)
            InsertEventLog(DB, "Re-sent email", toEmail, Nothing, Nothing, True)
            MessageLine("CONFIRM", "Successfully emailed.")
        End Using
        CheckItemAvail()

    End Sub

    Private Sub setOrderMode(ByVal webOrderID As String, ByRef viewOnlyReason As String)
        Dim SQL As String
        Dim r As DataReader
        Dim dtStr As String
        Dim orderByDate As Date
        Dim deliveryDate As Date
        Dim errMsg As String = ""
        Dim jfcBranchNr As String = ""
        'Dim orderState As String

        Using DB As New DBHelper
            viewOnlyReason = ""
            'Check if it's JETS order
            If Session("ORDERDTL_MODE") = "VIEW_JETS" Then
                'viewOnlyReason = "This is not a web order. Please contact JFC if you would like to edit this order"
                ViewState("ORDER_MODE") = "VIEW_JETS_ORDER"
                Exit Sub
            End If

            If Session("ORDERDTL_MODE") = "EDIT" Then
                'Check if the order is view only
                SQL = "SELECT customer_id, custGrpVendor_id, TO_CHAR(deliveryDate, :1) deliveryDate, shipPoint_id, orderState, moduser, " & _
                            " (SELECT codeDesc FROM codemaster WHERE codegrp = 'ORDERSTATE' AND code_id = sohdr.orderstate) codeDesc " & _
                            "FROM sohdr " & _
                            "WHERE webOrder_id = :2"
                r = DB.GetReader(SQL, DATE_FMT, CString(ViewState("ORDER_ID")))
                If r.Read() Then
                    DateTime.TryParseExact(CString(r("deliveryDate")), DATE_FMT, Nothing, Nothing, deliveryDate)
                    'Cannot edit for non JFC vendor
                    'If IsJfcBranch(DB, CString(r("custGrpVendor_id"))) = False Then
                    '    If deliveryDate > Now Then viewOnlyReason = "You cannot edit a non JFC order here. Please contact your vendor if you want to make changes to this order."
                    '     Return False
                    'End If
                    If canEditOrder(DB, ViewState("ORDER_ID"), errMsg, CString(r("moduser"))) = False Then
                        If deliveryDate > Now Then viewOnlyReason = errMsg
                        ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" 'IIf(IsJfcOrder(DB, ViewState("ORDER_ID")) = True, "VIEW_JETS_ORDER", "VIEW_ORDER")
                        Exit Sub
                    End If
                    'orderState = CString(r("orderState"))
                    If ViewState("ORDER_CMD") = "CUST" Then
                        'If orderState <> "N" Then
                        '    'Cannot edit if order state NOT IN ('N') for customer
                        '    If deliveryDate > Now Then viewOnlyReason = "You cannot edit this order because this order has been " & IIf(orderState = "D", "canceled", "processed") & "."
                        '    ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" 'IIf(IsJfcOrder(DB, ViewState("ORDER_ID")) = True, "VIEW_JETS_ORDER", "VIEW_ORDER")
                        '    Exit Sub
                        'End If
                        If CString(r("deliveryDate")) <> String.Empty Then
                            'Cannot edit if order by date has past
                            GetJFCBranchAndCustomerNr(DB, CString(r("customer_id")), jfcBranchNr, Nothing)
                            dtStr = GetOrderByDate(DB, CString(r("customer_id")), CString(r("custGrpVendor_id")), CString(r("shipPoint_id")), CString(r("deliveryDate")), DATETIME_SQLFMT)
                            DateTime.TryParseExact(dtStr, DATETIME_VBFMT, Nothing, Nothing, orderByDate)
                            If dtStr <> String.Empty AndAlso orderByDate < Now.AddDays(GetServerDayOffset(DB, jfcBranchNr)) Then
                                If deliveryDate > Now Then viewOnlyReason = "You cannot edit this order because order cut off date for this delivery has already past."
                                ViewState("ORDER_MODE") = "VIEW_WEB_ORDER"
                                Exit Sub
                            End If
                        End If
                        'ElseIf ViewState("ORDER_CMD") = "JFCADMIN" Then
                        '    'If orderState = "D" OrElse orderState = "S" OrElse orderState = "I" Then
                        '    '    'Cannot edit if order state IN ('D','S','I') for JFC admin
                        '    '    If deliveryDate > Now Then viewOnlyReason = "You cannot edit this order because this order has been " & CString(r("codeDesc")).ToLower & "."
                        '    If orderState <> "N" Then
                        '        'Cannot edit if order state NOT IN ('N') for JFC admin
                        '        If deliveryDate > Now Then viewOnlyReason = "You cannot edit this order because this order has been " & IIf(orderState = "D", "canceled", "processed") & "."
                        '        ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" 'IIf(IsJfcOrder(DB, ViewState("ORDER_ID")) = True, "VIEW_JETS_ORDER", "VIEW_ORDER")
                        '        Exit Sub
                        '    End If
                    End If
                End If
            End If
            'Can edit if ORDERDTL_MODE is not view only
            ViewState("ORDER_MODE") = IIf(Session("ORDERDTL_MODE") = "VIEW", "VIEW_WEB_ORDER", "EDIT_WEB_ORDER")
        End Using
        CheckItemAvail()

    End Sub

    Private Sub clearQtyEntry()
        txtNewCsQty.Text = ""
        txtNewCsQty_vp.Text = ""
        txtNewCsQty_sp.Text = ""
        txtNewEaQty.Text = ""
        txtNewEaQty_vp.Text = ""
        txtNewEaQty_sp.Text = ""
        txtNewLbs.Text = ""
        txtNewVpMemo.Text = ""
    End Sub

    Private Sub clearItemDtl()
        pnlAddItem.Visible = False
        'EnableCtrl({txtNewCsQty, txtNewEaQty, txtNewLbs})
        btnAddItem.Text = "Add to Order"
        imgNewItem.ImageUrl = GetWebImgUrl("empty.jpg")
        imgNewItem_mobile.ImageUrl = GetWebImgUrl("empty.jpg")
        lblNewItemDesc.Text = ""
        lblNewJfcRemark.Text = ""
        lblNewItemIdNr.Text = ""
        lblNewItemGrpName.Text = ""
        lblNewVendorName.Text = ""
        lblNewCtgName.Text = ""
        lblNewEaPerCs.Text = ""
        lblNewAvgCsWgt.Text = ""
        lblNewAvgEaWgt.Text = ""
        lblNewCsPrice_view.Text = ""
        lblNewEaPrice_view.Text = ""
        lblNewLbPrice_OrderByWgt_view.Text = ""
        lblNewLbPrice_OrderByQty_view.Text = ""
        lblNewCsPrice.Text = ""
        lblNewEaPrice.Text = ""
        lblNewLbPrice_OrderByWgt.Text = ""
        lblNewLbPrice_OrderByQty.Text = ""
        lblNewCustGrpItemID.Text = ""
        lblNewMsg.Text = ""
        lblNewAskPrice.Visible = False
        btnAddItem.Visible = True
        'btnQtyHistory.Visible = False
    End Sub

    Private Function canEditOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByRef errMsg As String, Optional ByVal email As String = "") As Boolean
        Dim orderState As String = GetOrderState(DB, webOrderID)
        Dim userEmail As String = Common.GetUserEmail(DB)
        If orderState <> "N" Then
            'Cannot edit if order state NOT IN ('N')
            errMsg = "You cannot edit this order because this order has been " & IIf(orderState = "D", "canceled", "processed") & "."

            If orderState = "P" AndAlso (userEmail = email OrElse Common.IsJfcBranchAdmin(DB, userEmail) OrElse Common.IsKmsSupport(DB, userEmail)) Then
                lblViewSFA.Text = "This order has been downloaded in SFA by " & email &
                    ". If you DON'T see this order in your SFA, you can change back to Order Placed status or delete this order."
                lblViewSFA.Visible = True
                btnChangeOrder.Visible = True
                btnDeleteOrder.Visible = True
            Else
                lblViewSFA.Visible = False
                btnChangeOrder.Visible = False
                btnDeleteOrder.Visible = False
            End If

            Return False

        End If
        Return True
    End Function

    Private Sub addItemInOrder()
        Dim msg As String
        Dim itemGrpNameInOrder As String = ""
        Dim orderState As String
        Dim minSalCnt As Integer
        Dim errMsg As String = ""

        Using DB As New DBHelper
            'Error check
            If canEditOrder(DB, ViewState("ORDER_ID"), errMsg) = False Then lblNewMsg.Text = errMsg : Exit Sub 'Cannot edit if order state NOT IN ('N') for JFC admin
            If lblNewCustGrpItemID.Text = "" Then lblNewMsg.Text = "Please enter an item. Make sure to click on ""Get Item"" button after entering an item" : Exit Sub
            If lblNewVendorName.Text <> lblVendorName.Text Then lblNewMsg.Text = "Error! Only " & lblVendorName.Text & " item can be added" : Exit Sub
            If AllowMixItemGrp(DB, GetOrderCustCompanyID(DB, ViewState("ORDER_ID"))) = False Then
                'Check if order form are the same as ones in cart
                If IsSameItemGrpInOrder(DB, ViewState("ORDER_ID"), lblNewCustGrpItemID.Text, itemGrpNameInOrder) = False Then
                    'MessageBox("Item(s) that belong to order form """ & itemGrpNameInCart & """ exist in the order. You can only add items from the same order form.", "Error", DB)
                    lblNewMsg.Text = "Error! Items that belong to order form """ & itemGrpNameInOrder & """ are in the order. You can only add items from the same order form."
                    Exit Sub
                End If
            End If
            If Val(txtNewCsQty.Text) = 0 AndAlso Val(txtNewEaQty.Text) = 0 AndAlso Val(txtNewLbs.Text) = 0 AndAlso Val(txtNewCsQty_vp.Text) = 0 AndAlso Val(txtNewEaQty_vp.Text) = 0 AndAlso Val(txtNewCsQty_sp.Text) = 0 AndAlso Val(txtNewEaQty_sp.Text) = 0 Then
                lblNewMsg.Text = "Error! Please enter qty"
                Exit Sub
            End If
            If IsNumber(txtNewCsQty.Text, True) = False OrElse IsNumber(txtNewEaQty.Text, True) = False OrElse IsNumber(txtNewLbs.Text, True) = False _
                OrElse IsNumber(txtNewCsQty_vp.Text, True) = False OrElse IsNumber(txtNewEaQty_vp.Text, True) = False OrElse IsNumber(txtNewCsQty_sp.Text, True) = False OrElse IsNumber(txtNewEaQty_sp.Text, True) = False Then
                'MessageBox("Qty has to be 0 or a positive number", "Error", DB)
                lblNewMsg.Text = "Error! Qty has to be 0 or a positive number"
                Exit Sub
            End If
            minSalCnt = IIf(Val(lblNewMinSalCnt.Text) = 0, 1, Val(lblNewMinSalCnt.Text))
            If Val(txtNewEaQty.Text) <> 0 AndAlso Val(txtNewEaQty.Text) Mod minSalCnt > 0 Then
                lblNewMsg.Text = "Error! Each qty has to be 0 or multiple of " & minSalCnt & "."
                Exit Sub
            End If
            If (Val(txtNewCsQty_vp.Text) > 0 OrElse Val(txtNewEaQty_vp.Text) > 0) AndAlso txtNewVpMemo.Text = String.Empty Then
                lblNewMsg.Text = "Error! Please enter VP memo."
                Exit Sub
            End If

            'Get order info
            orderState = GetOrderState(DB, ViewState("ORDER_ID"))
            DB.BeginTransaction()

            'Insert order history
            DBUpdate.InsertOrderHistory(DB, ViewState("ORDER_ID"), False)
            'Add item
            DBUpdate.UpSertItemInOrder(DB, ViewState("ORDER_ID"), lblCustomerID.Text, lblNewCustGrpItemID.Text, Val(txtNewCsQty.Text), Val(txtNewEaQty.Text), Val(txtNewLbs.Text), _
                                       Val(txtNewCsQty_vp.Text), Val(txtNewEaQty_vp.Text), Val(txtNewCsQty_sp.Text), Val(txtNewEaQty_sp.Text), txtNewVpMemo.Text, Val(lblNewCsPrice.Text), Val(lblNewEaPrice.Text), Val(lblNewLbPrice_OrderByQty.Text), False)
            'Set order state to 'Order updated but not received' if a confirmed or received order was updated by JFC Admin
            If orderState = "O" OrElse orderState = "C" Then DBUpdate.UpdateOrderState(DB, ViewState("ORDER_ID"), "U", False)
            'Update the total amount and total point for this order
            DBUpdate.UpdateOrderTotal(DB, ViewState("ORDER_ID"), False)

            DB.CommitTransaction()

            'Refresh item list and total amount
            showOrderDtl()
            msg = IIf(btnAddItem.Text = "Add to Order", "Successfully added the item", "Successfully updated the qty")

            'Clear item info
            txtItem.Text = ""
            clearQtyEntry()
            clearItemDtl()
            'Page.SetFocus(txtItem)

            'txtItem.Focus() 'TODO: Cannot use autocomplete with focused

            InsertEventLog(DB, "Saved item in order", ViewState("ORDER_ID"), Nothing, Nothing, True)
            MessageLine("CONFIRM", msg)
        End Using
        CheckItemAvail()

    End Sub

    Private Sub updateOrder(ByVal webOrderID As String, ByVal confirmOrder As Boolean)
        Dim lbl As Label
        Dim txtCsQty, txtEaQty, txtLbs, txtCsQty_vp, txtEaQty_vp, txtCsQty_sp, txtEaQty_sp, txtVpMemo As TextBox
        Dim lbs As Decimal
        Dim custGrpItemID As String
        Dim orderState As String
        'Dim totalAmt As Decimal

        Using DB As New DBHelper
            DB.BeginTransaction()

            'Insert order history
            DBUpdate.InsertOrderHistory(DB, webOrderID, False)

            'Delete items in order (cannot delete all items because price can be changed by JFC admin)
            'DeleteAllItemsInOrder(DB, webOrderID, False)

            'Add items in order
            'totalAmt = 0
            If ViewState("ORDER_CMD") = "CUST" Then txtVendorNote.Text = ""
            For i As Integer = 0 To grdItem_edit.Rows.Count - 1
                txtCsQty = grdItem_edit.Rows.Item(i).FindControl("txtCsQty")
                txtEaQty = grdItem_edit.Rows.Item(i).FindControl("txtEaQty")
                txtLbs = grdItem_edit.Rows.Item(i).FindControl("txtLbs")
                lbs = Val(txtLbs.Text)
                'If ViewState("ORDER_CMD") = "JFCADMIN" Then 'Commented out because need to get qty even if it's invisible
                txtCsQty_vp = grdItem_edit.Rows.Item(i).FindControl("txtCsQty_vp")
                txtEaQty_vp = grdItem_edit.Rows.Item(i).FindControl("txtEaQty_vp")
                txtCsQty_sp = grdItem_edit.Rows.Item(i).FindControl("txtCsQty_sp")
                txtEaQty_sp = grdItem_edit.Rows.Item(i).FindControl("txtEaQty_sp")
                txtVpMemo = grdItem_edit.Rows.Item(i).FindControl("txtVpMemo")
                'End If
                lbl = grdItem_edit.Rows.Item(i).FindControl("lblCustGrpItemID")
                custGrpItemID = lbl.Text
                'lbl = grdItem_edit.Rows.Item(i).FindControl("lblLineAmt")
                'totalAmt += Val(lbl.Text)
                DBUpdate.UpSertItemInOrder(DB, webOrderID, lblCustomerID.Text, custGrpItemID, Val(txtCsQty.Text), Val(txtEaQty.Text), lbs, Val(txtCsQty_vp.Text), Val(txtEaQty_vp.Text), Val(txtCsQty_sp.Text), Val(txtEaQty_sp.Text), txtVpMemo.Text, -1, -1, -1, False)
                If ViewState("ORDER_CMD") = "CUST" AndAlso IsJfcScaledItem(DB, lblCustomerID.Text, custGrpItemID) = True AndAlso (Val(txtCsQty.Text) > 0 OrElse Val(txtEaQty.Text) > 0) Then
                    txtVendorNote.Text &= "#" & GetItemID(DB, custGrpItemID) & " " & IIf(Val(txtCsQty.Text) > 0, CString(Val(txtCsQty.Text)) & "cs ", "") & IIf(Val(txtEaQty.Text) > 0, CString(Val(txtEaQty.Text)) & "pc ", "")
                End If
            Next

            Dim DeliveryDate = DateTime.Parse(txtDeliveryDate.Text).AddDays(GetShipdateOffset(DB, lblCustomerID.Text))

            'Update order info and total amount txtDeliveryDate.Text
            DBUpdate.UpdateOrder(DB, webOrderID, DeliveryDate.ToShortDateString(), txtCustOrderNr.Text, txtMessage.Text, txtVendorNote.Text, False)

            orderState = GetOrderState(DB, webOrderID)
            If orderState = "N" Then
                If confirmOrder = True Then DBUpdate.UpdateOrderState(DB, webOrderID, "O", False)
            ElseIf orderState = "O" OrElse orderState = "C" Then
                'Set order state to 'Order updated but not received' if a confirmed or received order was updated by JFC Admin
                DBUpdate.UpdateOrderState(DB, webOrderID, "U", False)
            End If

            DB.CommitTransaction()
        End Using
    End Sub

    Private Sub showOrderHdr()
        Dim SQL As String
        Dim r As DataReader
        Dim nrItems As Integer
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim serverDayOffset As Decimal
        Dim jfcVendorTimeZone As String = ""

        Using DB As New DBHelper
            GetJFCBranchAndCustomerNr(DB, ViewState("CUSTOMER_ID"), jfcBranchNr, jfcCustomerNr)
            serverDayOffset = GetServerDayOffset(DB, jfcBranchNr, jfcVendorTimeZone)
            'Show web orders before invoiced and show JETS orders for non web orders and invoiced orders
            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" OrElse ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" Then
                'Web order
                'YI FU 8/10/2021
                SQL = "SELECT h.weborder_id, NULL AS jetsOrderNr, h.orderstate, h.customer_id, h.custGrpVendor_id, v.vendorname, h.custordernr, h.message, h.vendorNote, h.totalamt, code.codeDesc, h.shipPoint_id, " & _
                            " cust.customerName, cust.custGrp_id, cust.jfcBranchNr, cust.jfcCustomerNr, cg.custGrpName, cg.custCompany_id, cust.SHIPDATEOFFSET, " & _
                            " TO_CHAR(h.orderdate, :1) orderDate, " & _
                            " TO_CHAR(h.deliveryDate - cust.SHIPDATEOFFSET, :2) shipDate, TO_CHAR(h.deliveryDate - cust.SHIPDATEOFFSET, :3) shipDate_view, " & _
                            " TO_CHAR(h.deliveryDate, :2) deliverypDate, TO_CHAR(h.deliveryDate, :3) deliveryDate_view," & _
                            " TO_CHAR(h.createDate + " & serverDayOffset & ", :4) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.createUser IS NOT NULL THEN ' by ' || h.createUser END createDateTimeUser, " & _
                            " TO_CHAR(h.modDate + " & serverDayOffset & ", :5) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.modUser IS NOT NULL THEN ' by ' || h.modUser END modDateUser, " & _
                            " TO_CHAR(h.modDate + " & serverDayOffset & ", :6) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " modDate, " & _
                            " TO_CHAR(h.ApproveDate + " & serverDayOffset & ", :7) || CASE WHEN h.ApproveUser IS NOT NULL THEN ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' by ' || h.ApproveUser END ApproveDateTimeUser, " & _
                            " h.SfaDevice, h.Reference " & _
                            "FROM sohdr h " & _
                            "LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE' AND h.orderstate = code.code_id " & _
                            "LEFT OUTER JOIN customer cust ON cust.customer_id = h.customer_id " & _
                            "LEFT OUTER JOIN custGrp cg ON cg.custGrp_id = cust.custGrp_id " & _
                            "INNER JOIN custgrp_vendor v on h.custgrpvendor_id = v.custgrpvendor_id " & _
                            "WHERE h.webOrder_id = :8"
                r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, DAYDATE_LONG_SQLFMT, DATE_FMT, DAYDATE_LONG_SQLFMT, DATETIME_SQLFMT, DATE_FMT, DATETIME_SQLFMT, DATETIME_SQLFMT, CString(ViewState("ORDER_ID")))
            Else
                'YI FU 8/10/2021
                'Jets order
                SQL = "SELECT h.webOrder_id, h.orderNr AS jetsOrderNr, h.orderstate, cust.customer_id, v.custGrpVendor_id, v.vendorname, h.custordernr, NULL AS message, NULL AS vendorNote, code.codeDesc, NULL AS shipPoint_id, " & _
                            " cust.customerName, cust.custGrp_id, h.jfcBranchNr, h.jfcCustomerNr, cg.custGrpName, cg.custCompany_id, cust.SHIPDATEOFFSET," & _
                            " CASE WHEN h.orderState = 'R' THEN h.totalAmt_ship ELSE h.totalAmt END AS totalAmt, " & _
                            " TO_CHAR(h.orderdate, :1) orderDate, " & _
                            " TO_CHAR(h.deliveryDate, :2) shipDate, TO_CHAR(h.deliveryDate, :3) shipDate_view, " & _
                            " TO_CHAR(h.createDate, :4) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.createUser IS NOT NULL THEN ' by ' || h.createUser END createDateTimeUser, " & _
                            " TO_CHAR(h.modDate, :5) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN h.modUser IS NOT NULL THEN ' by ' || h.modUser END modDateUser, " & _
                            " TO_CHAR(h.modDate, :6) || ' ' || " & DB.SqlEsc(jfcVendorTimeZone) & " modDate,'' as ApproveDateTimeUser,'' as SfaDevice, h.Reference " & _
                            "FROM sohdr_jets h " & _
                            "LEFT OUTER JOIN codemaster code ON code.codegrp = 'ORDERSTATE_JETS' AND h.orderstate = code.code_id " & _
                            "INNER JOIN customer cust ON cust.jfcBranchNr = h.jfcBranchNr AND cust.jfcCustomerNr = h.jfcCustomerNr " & _
                            "INNER JOIN custGrp cg ON cg.custGrp_id = cust.custGrp_id " & _
                            "INNER JOIN jfcVendor jv on h.jfcBranchNr = jv.jfcBranchNr " & _
                            "INNER JOIN custGrp_vendor v on v.custGrp_id = cg.custGrp_id AND v.vendor_id = jv.vendor_id " & _
                            "WHERE h.jfcBranchNr = :7 AND h.orderNr = :8"
                r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, DAYDATE_LONG_SQLFMT, DATETIME_SQLFMT, DATE_FMT, DATETIME_SQLFMT, jfcBranchNr, CString(ViewState("ORDER_ID")))

            End If
            If r.Read() Then
                lblCustomerID.Text = CString(r("customer_id"))
                lblCustGrpVendorID.Text = CString(r("custGrpVendor_id"))
                lblUserType.Text = ViewState("ORDER_CMD")
                lblHideEaQty.Text = IIf(HideEaQty(DB, lblCustomerID.Text) = True, "Y", "N")
                lblTitle2.Visible = IIf(CString(r("weborder_id")) = "", True, False) 'Show / hide "(Non Web Order)"
                lblOrderID.Text = IIf(CString(r("jetsOrderNr")) = "", CString(r("weborder_id")), CString(r("jetsOrderNr")) & IIf(CString(r("weborder_id")) = "", "", " (" & CString(r("weborder_id")) & ")"))
                lblOrderState.Text = CString(r("codeDesc"))
                'lblOrderDate.Text = CString(r("orderDate"))
                lblVendorName.Text = CString(r("vendorName"))
                lblShipPointID.Text = CString(r("shipPoint_id"))
                lblCreateDateTimeUser.Text = CString(r("createDateTimeUser"))
                lblSFA.Text = CString(r("SfaDevice"))
                lblApproveDateTimeUser.Text = CString(r("ApproveDateTimeUser"))
                lblModDateUser.Text = CString(r("modDate")) 'CString(r("modDateUser"))
                lblCustGrpName.Text = CString(r("custGrpName"))
                lblCustomerName.Text = CString(r("customerName"))
                lblJfcBranchNr.Text = CString(r("jfcBranchNr"))
                lblJfcCustomerNr.Text = CString(r("jfcCustomerNr"))
                lnkCurrentBalance.Text = "AR Balance: $" & GetCurrentBalance(DB, CString(r("customer_id")))
                lnkCurrentBalance.NavigateUrl = GetCurrentDirectory() & "Balance.aspx?customer_id=" & CString(r("customer_id")) & "&cmd=show_dtl"
                txtDeliveryDate.Text = CString(r("shipDate"))
                lblDeliveryDate.Text = CString(r("shipDate_view"))
                lblShipOffset.Text = IIf((ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" OrElse ViewState("ORDER_MODE") = "VIEW_WEB_ORDER") And Val(r("SHIPDATEOFFSET")) <> 0,
                                             IIf(IsMobile(), "<br />", "") & "Delivery Date: +" & CString(r("SHIPDATEOFFSET")) & " Day(s)", "")
                txtCustOrderNr.Text = CString(r("custOrderNr"))
                lblCustOrderNr.Text = CString(r("custOrderNr"))
                If CString(r("jfcBranchNr")) = "05" Then
                    lblreference.Text = CString(r("Reference"))
                    pnlreference.Visible = True
                Else
                    pnlreference.Visible = False
                End If

                txtMessage.Text = CString(r("message"))
                lblMessage.Text = CString(r("message"))
                txtVendorNote.Text = CString(r("vendorNote"))
                lblVendorNote.Text = CString(r("vendorNote"))
                If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                    'txtCustOrderNr.Text = CString(r("custOrderNr"))
                    'txtMessage.Text = CString(r("message"))
                ElseIf ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" Then
                    'lblCustOrderNr.Text = CString(r("custOrderNr"))
                    'lblMessage.Text = CString(r("message"))
                    nrItems = Val(DB.GetScalar("SELECT COUNT(*) FROM sodtl WHERE webOrder_id = " & DB.SqlEsc(ViewState("ORDER_ID"))))
                    'lblNrItem.Text = nrItems & " items"
                    lblTotalAmt.Text = String.Format("{0:c}", Val(r("totalAmt")))
                    'lblOrderMsg.Text = nrItems & " items exist in this order. Total amount is <span class='red'>" & lblTotalAmt.Text & "</span>"
                ElseIf ViewState("ORDER_MODE") = "VIEW_JETS_ORDER" Then
                    'lblCustOrderNr.Text = CString(r("custOrderNr"))
                    'lblMessage.Text = CString(r("message"))
                    nrItems = Val(DB.GetScalar("SELECT COUNT(*) FROM sodtl_jets WHERE jfcBranchNr = " & DB.SqlEsc(jfcBranchNr) & " AND orderNr = " & DB.SqlEsc(ViewState("ORDER_ID"))))
                    'lblTotalAmt.Text = String.Empty
                End If
                'Set more properties
                pnlVendorName.Visible = IIf(GetNrCustGrpVendor(DB, CString(r("custGrp_id"))) > 1, True, False)
                pnlShowPrice.Visible = IIf(ViewState("ORDER_CMD") <> "JFCADMIN" AndAlso Common.ApplyInEmailOrCustomer(DB, CString(r("customer_id")), "HidePrice") = True, False, True)
                pnlCustGrpName.Visible = IIf(GetNrCustGrp(DB, CString(r("custCompany_id"))) > 1, True, False)
                pnlCustShipPoint.Visible = IIf(GetNrCustShipPoint(DB, CString(r("customer_id"))) > 1, True, False)
                pnlSFA.Visible = IIf(ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" OrElse ViewState("ORDER_MODE") = "VIEW_WEB_ORDER", True, False)
                pnlApproveOn.Visible = IIf(ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" OrElse ViewState("ORDER_MODE") = "VIEW_WEB_ORDER", True, False)
            End If
            r.close()

            'Show order form if company doesn't allow mixed order form and not a JETS order
            InvisibleCtrl({lblTitleItemGrp, lblItemGrpName})
            If AllowMixItemGrp(DB, GetOrderCustCompanyID(DB, ViewState("ORDER_ID"))) = False AndAlso ViewState("ORDER_MODE") <> "VIEW_JETS_ORDER" Then
                VisibleCtrl({lblTitleItemGrp, lblItemGrpName})
                lblItemGrpID.Text = GetItemGrpIDInOrder(DB, ViewState("ORDER_ID"))
                lblItemGrpName.Text = GetItemGrpName(DB, lblItemGrpID.Text)
            End If

            'Show shipping address
            SQL = "SELECT shipPoint_id, shipName, address1, address2, city, state, zip, country " & _
                    "FROM customer_shipPoint " & _
                    "WHERE customer_id = :1" & _
                     IIf(ViewState("ORDER_MODE") <> "VIEW_JETS_ORDER", " AND shipPoint_id = " & DB.SqlEsc(lblShipPointID.Text), "") 'JETS order doesn't store shippoint. Assume only one exists.
            r = DB.GetReader(SQL, lblCustomerID.Text)
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

        End Using
    End Sub

    Private Sub showOrderDtl()
        Dim SQL As String
        Dim lbl As Label
        Dim txt As TextBox
        Dim totalAmt, totalCost, totalGrossWeight_kg, totalCaseVol_m3 As Decimal
        Dim totalGpStr As String = ""
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim orderState As String
        Dim SQL_eaCost, SQL_lbCost As String
        Dim customerID As String = ViewState("CUSTOMER_ID")
        Dim showItemDtl As Boolean
        Dim hidePrice As Boolean

        Using DB As New DBHelper
            hidePrice = ViewState("ORDER_CMD") <> "JFCADMIN" AndAlso Common.ApplyInEmailOrCustomer(DB, customerID, "HidePrice")

            'Set properties
            grdItem_edit.Columns(10).Visible = False 'Available cs and ea
            grdItem_edit.Columns(11).Visible = False 'cs and ea for availability
            grdItem_edit.Columns(12).Visible = False 'Out of stock
            grdItem_edit.Columns(13).Visible = False 'Cost and GP
            grdItem_edit.Columns(14).Visible = False '/cs for cost
            If ViewState("ORDER_CMD") = "JFCADMIN" AndAlso IsJfcOrder(DB, ViewState("ORDER_ID")) = True Then
                grdItem_edit.Columns(10).Visible = True 'Available cs and ea
                grdItem_edit.Columns(11).Visible = True 'cs and ea for availability
                grdItem_edit.Columns(13).Visible = True 'Cost and GP
                grdItem_edit.Columns(14).Visible = True '/cs for cost
            End If
            If hidePrice = True Then
                grdItem_edit.Columns(4).HeaderText = "" 'Price
                grdItem_edit.Columns(9).HeaderText = "" 'Amount
                grdItem_view.Columns(2).Visible = False 'Price
                grdItem_view.Columns(3).Visible = False '/cs for price
                grdItem_view.Columns(6).Visible = False 'Amount
            End If
            If IsMobile() Then
                If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                    'pnlOrderHdr.Width = 567
                    'pnlItem.Width = 567
                    'pnlOrderDtl_Edit.Width = 700
                    grdItem_edit.Columns(0).Visible = False 'Image
                    If ViewState("ORDER_CMD") <> "JFCADMIN" Then grdItem_edit.Columns(1).Visible = False 'Item #
                    'grdItem_edit.Columns(2).Visible = False 'Item description
                    grdItem_edit.Columns(3).Visible = False 'Ea/Cs
                    grdItem_edit.Columns(13).Visible = False 'Cost and gp
                    grdItem_edit.Columns(14).Visible = False '/cs for cost
                Else
                    'grdItem_view.Columns(0).Visible = False 'Item #
                    'grdItem_view.Columns(3).Visible = False '/cs for price
                    grdItem_view.Columns(7).Visible = False 'Invoice
                End If
            Else
                pnlOrderHdr.Width = 500
                pnlOrderHdr.Height = 410
                If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                    pnlOrderDtl_Edit.Width = 1153 '1123
                    pnlItem.Width = 597 '567
                    pnlItem.Height = 410
                    grdItem_edit.Columns(0).Visible = False 'Image
                End If
            End If
            If Common.ALLOW_LB_ORDER = False Then
                If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                    grdItem_edit.Columns(8).Visible = False
                End If
            End If
            If lblHideEaQty.Text = "Y" Then
                grdItem_edit.Columns(3).Visible = False 'Ea/Cs
                grdItem_edit.Columns(7).Visible = False 'Each Qty
            End If
            'If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
            'Else
            '    grdItem_view.Columns(7).Visible = ALLOW_CUST_DOWNLOAD_INVOICE 'Invoice
            'End If

            'Show items in this order
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" OrElse ViewState("ORDER_MODE") = "VIEW_WEB_ORDER" Then
                'Web order (remainingInStockQty = InStockQty - 1 for scaled items)
                SQL_eaCost = "(CASE WHEN d.eaPerCs <= 1 THEN 0 ELSE " & _
                                "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN d.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/d.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                                " ELSE (CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / d.eaPerCs, 2) END) END) " & _
                             "END)"
                Dim SQL_eabtm = "(CASE WHEN d.eaPerCs <= 1 THEN 0 ELSE " & _
                                "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN d.eapercs = 10 then round(NVL(p.btmcsprice, 0) * nvl(p.avgeawgt, 0.9), 2) ELSE ROUND(ROUND(NVL(p.btmcsprice, 0)/d.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                                " ELSE (CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.btmcsprice, 0) / d.eaPerCs, 2) END) END) " & _
                             "END)"

                'SQL_eaCost = "(CASE WHEN d.eaPerCs <= 1 THEN 0 ELSE " & _
                '                "(CASE WHEN NVL(p.scaledItem, 'N') = 'Y' THEN (CASE WHEN d.eapercs = 10 then round(NVL(p.csCost, 0) * nvl(p.avgeawgt, 0.9) / nvl(p.avgcswgt, 9.9), 2) ELSE ROUND(ROUND(NVL(p.csCost, 0)/d.eapercs, 3)*nvl(p.avgeawgt, 0.9)*10, 2) END) " & _
                '                " ELSE (CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / d.eaPerCs, 2) END) END) " & _
                '             "END)"
                SQL_lbCost = "(CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / d.eaPerCs * 10, 2) END)"

                Dim SQL_lbbtm = "(CASE WHEN NVL(d.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.btmcsprice, 0) / d.eaPerCs * 10, 2) END)"

                SQL = "SELECT d.webLineNr, d.custGrpItem_id, g.itemGrpName, d.minSalCnt, TRIM(i.jfcRemark || ' ' || cc.custRemark) AS jfcRemark, " & _
                            " d.csQty, d.csQty_vp, d.csQty_sp, NVL(d.csQty_vp, 0) + NVL(d.csQty_sp, 0) AS csQty_vpsp, d.eaQty, d.eaQty_vp, d.eaQty_sp, NVL(d.eaQty_vp, 0) + NVL(d.eaQty_sp, 0) AS eaQty_vpsp, d.vpMemo, d.Lbs, " & _
                            " d.eaPerCs, d.csPrice, d.eaPrice, d.lbPrice_orderByWgt, d.lbPrice_orderByQty, d.lineAmt, " & _
                            " NVL(p.csCost, 0) AS csCost, " & _
                            " " & SQL_eaCost & " AS eaCost, " & _
                            " " & SQL_lbCost & " AS lbCost, " & _
                            " NVL(p.btmcsprice, 0) AS btmcsprice, " & _
                            " " & SQL_eabtm & " AS btmeaprice, " & _
                            " " & SQL_lbbtm & " AS btmlbprice, " & _
                            " NVL(vc.item_id,'') AS promoItemId, " & _
                            " CASE WHEN cc.isNew = 'Y' OR (SELECT CASE WHEN ci.csPrice = 0 and ci.eaPrice = 0 and ci.lbPrice_orderByWgt = 0 and ci.lbPrice_orderByQty = 0 THEN 'Y' ELSE 'N' END FROM " & IIf(Common.IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci WHERE ci.customer_id = " & DB.SqlEsc(customerID) & " and ci.custgrpitem_id = d.custGrpItem_id) = 'Y' THEN 'Y' ELSE 'N' END AS IsNew, " &
                            " ROUND(p.csCost * d.csQty + " & SQL_eaCost & " * d.eaQty, 2) AS lineCost, " & _
                            " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0  ) avgeawgt, " & _
                            " CASE WHEN i.jfcItem = 'Y' THEN " & _
                            "   CASE WHEN d.lbPrice_orderByQty > 0 THEN " & _
                            "     (NVL(p.csAvail, 0) * NVL(d.eaPerCs, 1) + NVL(p.eaAvail, 0)) - 1" & _
                            "   ELSE " & _
                            "     (NVL(p.csAvail, 0) * NVL(d.eaPerCs, 1) + NVL(p.eaAvail, 0)) - (d.csQty * NVL(d.eaPerCs, 1) + d.eaQty) " & _
                            "   END " & _
                            " ELSE 0 END AS remainingInStockQty, " & _
                            " CASE WHEN i.jfcItem = 'Y' THEN p.csAvail ELSE NULL END AS csAvail_view, " & _
                            " CASE WHEN i.jfcItem = 'Y' THEN p.eaAvail ELSE NULL END AS eaAvail_view, " & _
                            " CASE WHEN d.lbPrice_orderByQty <> 0 THEN ROUND((d.lbPrice_orderByQty - " & SQL_lbCost & ") / d.lbPrice_orderByQty, 2) " & _
                            "   ELSE (CASE WHEN NVL(d.csPrice, 0) = 0 THEN 1 ELSE ROUND((d.csPrice - NVL(p.csCost, 0)) / d.csPrice, 2) END) END gp, " & _
                            " CASE WHEN d.itemNr IS NULL THEN d.item_id ELSE d.itemNr || '<br />(' || d.item_id || ')' END AS itemIdNr_view, " & _
                            " CASE WHEN d.custItemDesc IS NULL THEN d.itemDesc ELSE d.custItemDesc || ' (' || d.itemDesc || ')' END AS itemDesc_view, " & _
                            " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, d.item_id, " & _
                            " jd.orderNr AS invoice, " & _
                            " doc.fileName, doc.directory, " & _
                            " CASE WHEN ADD_MONTHS(h.orderDate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & " THEN 'Y' ELSE 'N' END AS allowCustDownloadInvoice, " & _
                            " TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2) * d.csQty, 99999.99) AS lineGrossWeight_kg, " & _
                            " TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3) * d.csQty, 99999.999) AS lineCaseVol_m3, " & _
                            IIf(Common.ShowItemDtl(DB, customerID) = True AndAlso ViewState("ORDER_MODE") = "EDIT_WEB_ORDER", " TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2), 99999.99) || ' kg, ' || TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3), 99999.999) || ' m³'", "NULL") & " AS itemDtl, " & _
                            IIf(ViewState("ORDER_CMD") <> "JFCADMIN" AndAlso hidePrice = True, "'Y'", "'N'") & " AS hidePrice " & _
                           "FROM custGrp_itemGrp g, sohdr h, sodtl d " & _
                            "LEFT OUTER JOIN sodtl_jets jd ON jd.weborder_id = d.weborder_id AND jd.weblinenr = d.weblinenr " & _
                            "LEFT OUTER JOIN " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " i ON d.custGrpItem_id = i.custGrpItem_id " & _
                            "LEFT OUTER JOIN customer_document doc ON jd.orderNr = doc.docName AND UPPER(doc.docType) = 'INVOICE' AND doc.customer_id = " & DB.SqlEsc(lblCustomerID.Text) & " AND doc.custGrpVendor_id = " & DB.SqlEsc(lblCustGrpVendorID.Text) & " " & _
                            "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " & _
                            "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " & _
                            "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                            "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.custGrpItem_id AND cc.customer_id = " & DB.SqlEsc(customerID) & " " & _
                            "LEFT OUTER JOIN v_customer_promoitem vc ON vc.item_id = i.item_id AND (vc.customer_id = " & DB.SqlEsc(customerID) & " or vc.customer_id is null) and vc.jfcbranchnr = " & DB.SqlEsc(jfcBranchNr) & " " & _
                           "WHERE h.webOrder_id = d.webOrder_id AND d.itemGrp_id = g.itemGrp_id " & _
                            "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _
                            "  AND d.webOrder_id = " & DB.SqlEsc(ViewState("ORDER_ID")) & " " & _
                            "ORDER BY d.webLineNr"
                '" cc.ISNEW as IsNew," & _
                '"  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
                '" CASE WHEN NVL(d.csPrice, 0) = 0 THEN 1 ELSE (CASE WHEN d.lbPrice_orderByQty <> 0 THEN ROUND((d.lbPrice_orderByQty - NVL(cp.csCost, 0)) / d.lbPrice_orderByQty, 2) ELSE ROUND((d.csPrice - NVL(cp.csCost, 0)) / d.csPrice, 2) END) END gp, " & _
                '" CASE WHEN NVL(d.csPrice, 0) = 0 THEN 1 ELSE ROUND((d.csPrice - NVL(cp.csCost, 0)) / d.csPrice, 2) END gp, " & _
                '"LEFT OUTER JOIN jets_price p ON p.itemcode = cp.itemcode AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " & _
            Else
                'Jets order
                SQL = "SELECT d.orderlineNr, NULL AS custGrpItem_id, NULL AS itemGrpName, NULL AS minSalCnt, NULL AS eaPerCs, " & _
                            " d.csQty_vp, d.csQty_sp, NVL(d.csQty_vp, 0) + NVL(d.csQty_sp, 0) AS csQty_vpsp, d.eaQty_vp, d.eaQty_sp, NVL(d.eaQty_vp, 0) + NVL(d.eaQty_sp, 0) AS eaQty_vpsp, d.vpMemo, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.csQty_ship ELSE d.csQty END AS csQty, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.eaQty_ship ELSE d.eaQty END AS eaQty, " & _
                            " NULL AS lbs, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.csPrice_ship ELSE d.csPrice END AS csPrice, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.eaPrice_ship ELSE d.eaPrice END AS eaPrice, " & _
                            " NULL AS lbPrice_orderByWgt, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.lbPrice_orderByQty_ship ELSE d.lbprice_orderByQty END AS lbPrice_orderByQty, " & _
                            " CASE WHEN h.orderState = 'R' THEN d.lineAmt_ship ELSE d.lineAmt END AS lineAmt, " & _
                            " d.item_id AS itemIdNr_view, " & _
                            " d.lineDesc AS itemDesc_view, " & _
                            " d.orderNr AS invoice, doc.fileName, doc.directory, 'JFCItems' AS itemType, d.item_id, " & _
                            " CASE WHEN ADD_MONTHS(h.orderDate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & " THEN 'Y' ELSE 'N' END AS allowCustDownloadInvoice " & _
                            "FROM sohdr_jets h, sodtl_jets d " & _
                            "LEFT OUTER JOIN customer_document doc ON d.orderNr = doc.docName AND UPPER(doc.docType) = 'INVOICE' AND doc.customer_id = " & DB.SqlEsc(lblCustomerID.Text) & " AND doc.custGrpVendor_id = " & DB.SqlEsc(lblCustGrpVendorID.Text) & " " & _
                            "WHERE h.jfcBranchNr = d.jfcBranchNr AND h.orderNr = d.orderNr " & _
                            "  AND d.jfcBranchNr = " & DB.SqlEsc(jfcBranchNr) & " AND d.orderNr = " & DB.SqlEsc(ViewState("ORDER_ID")) & " " & _
                            "ORDER BY d.orderlineNr"
                '" CASE WHEN h.orderState = 'R' THEN d.Lbs_ship ELSE d.lbs END AS lbs, " & _
                '" CASE WHEN h.orderState = 'R' THEN d.lbPrice_orderByWgt_ship ELSE d.lbprice_orderByWgt END AS lbPrice_orderByWgt, " & _
            End If
            'subSQLCs = "(SELECT NVL(SUM(csQty_ship),0) FROM sodtl_jets WHERE webOrder_id = d.webOrder_id AND webLineNr = d.webLineNr GROUP BY webLineNr)"
            'subSQLEa = "(SELECT NVL(SUM(eaQty_ship),0) FROM sodtl_jets WHERE webOrder_id = d.webOrder_id AND webLineNr = d.webLineNr GROUP BY webLineNr)"
            'jetsSQL = "SELECT webLineNr, itemIdNr_view, itemDesc_view, itemGrpName, csPrice, eaPrcie, lbPrice_orderByWgt, csQty, eaQty, lbs, csQty_ship, eaQty_ship, lbs_ship, itemIdNr_view, itemDesc_view, " & _
            '            " outOfStockCs, outOfStockEa, lineAmt, invoice, jfcAdditional " & _
            '            "FROM " & _
            '            "(SELECT j.webLineNr, g.itemGrpName, j.csPrice, j.eaPrcie, j.lbPrice_orderByWgt, d.csQty, d.eaQty, d.lbs, j.csQty_ship, j.eaQty_ship, j.lbs_ship," & _
            '            " CASE WHEN d.itemNr IS NULL THEN d.item_id ELSE d.itemNr || '<br />(' || d.item_id || ')' END AS itemIdNr_view, " & _
            '            " CASE WHEN d.custItemDesc IS NULL THEN d.itemDesc ELSE d.custItemDesc END AS itemDesc_view " & _
            '            " CASE WHEN NVL(d.csQty,0) - " & subSQLCs & " <= 0 THEN '0' ELSE TO_CHAR(NVL(d.csQty,0) - " & subSQLCs & ") || ' ca' END outOfStockCs," & _
            '            " CASE WHEN NVL(d.eaQty,0) - " & subSQLEa & " <= 0 THEN '0' ELSE TO_CHAR(NVL(d.eaQty,0) - " & subSQLEa & ") || ' ea' END outOfStockEa," & _
            '            " j.lineAmt, j.orderNr AS invoice, NULL AS jfcAdditional " & _
            '            " FROM sodtl d, sodtl_jets j, custGrp_itemGrp g " & _
            '            " WHERE d.itemGrp_id = g.itemGrp_id " & _
            '            " AND d.webOrder_id = j.webOrder_id AND d.webLineNr = j.webLineNr " & _
            '            " AND d.webOrder_id = '" & ViewState("ORDER_ID") & "'" & _
            '            "UNION " & _
            '            "SELECT webLineNr, NULL AS itemGrpName, item_id itemIdNr_view, lineDesc itemDesc_view, csPrice, eaPrice, lbPrice_orderByWgt, " & _
            '            " csQty_ship AS csQty, eaQty_ship AS eaQty, lbs_ship AS lbs, csQty_ship AS csQty_ship, eaQty_ship AS eaQty_ship, lbs_ship AS lbs_ship, " & _
            '            " NULL AS outOfStockCs, NULL AS outOfStockEa, lineAmt, orderNr AS invoice, 'Yes' AS jfcAdditional " & _
            '            " FROM sodtl_jets WHERE webOrder_id = '" & ViewState("ORDER_ID") & "'" & _
            '            " AND webLineNr > 1000" & _
            '            ") a " & _
            '            "ORDER BY d.webLineNr"
            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                dsItem_edit.ProviderName = "System.Data.OracleClient"
                dsItem_edit.ConnectionString = DB.GetConnectionString
                dsItem_edit.SelectCommand = SQL
                grdItem_edit.DataSourceID = "dsItem_edit"
                grdItem_edit.DataBind()
            Else
                dsItem_view.ProviderName = "System.Data.OracleClient"
                dsItem_view.ConnectionString = DB.GetConnectionString
                dsItem_view.SelectCommand = SQL
                grdItem_view.DataSourceID = "dsItem_view"
                grdItem_view.DataBind()
            End If

            If ViewState("ORDER_MODE") = "EDIT_WEB_ORDER" Then
                'Set properties
                orderState = GetOrderState(DB, ViewState("ORDER_ID"))
                If ViewState("ORDER_CMD") = "JFCADMIN" AndAlso orderState = "N" Then
                    'btnConfirm.Visible = True
                    btnConfirmAndEmail.Visible = True
                    btnUpdate.Visible = False
                Else
                    'btnConfirm.Visible = False
                    btnConfirmAndEmail.Visible = False
                    btnUpdate.Visible = True
                End If

                'Add handler to qty textbox
                lblTextChanged.Value = "NO"
                ViewState("TEXT_CHANGED") = "NO"
                For i As Integer = 0 To grdItem_edit.Rows.Count - 1
                    'Check delivery date
                    txt = grdItem_edit.Rows.Item(i).FindControl("txtCsQty")
                    txt.Attributes.Add("OnKeyUp", "return textValueChanged('YES')")
                Next

                'Update order form if company doesn't allow mixed order form
                If AllowMixItemGrp(DB, GetOrderCustCompanyID(DB, ViewState("ORDER_ID"))) = False Then
                    lblItemGrpID.Text = GetItemGrpIDInOrder(DB, ViewState("ORDER_ID"))
                    lblItemGrpName.Text = GetItemGrpName(DB, lblItemGrpID.Text)
                End If

                'Update order info and message
                showItemDtl = Common.ShowItemDtl(DB, customerID)
                totalAmt = 0 : totalCost = 0 : totalGrossWeight_kg = 0 : totalCaseVol_m3 = 0
                For i As Integer = 0 To grdItem_edit.Rows.Count - 1
                    lbl = grdItem_edit.Rows.Item(i).FindControl("lblLineAmt")
                    totalAmt += Val(lbl.Text)
                    lbl = grdItem_edit.Rows.Item(i).FindControl("lblLineCost")
                    totalCost += Val(lbl.Text)
                    If showItemDtl = True Then
                        lbl = grdItem_edit.Rows.Item(i).FindControl("lblLineGrossWeight_kg")
                        totalGrossWeight_kg += Val(lbl.Text)
                        lbl = grdItem_edit.Rows.Item(i).FindControl("lblLineCaseVol_m3")
                        totalCaseVol_m3 += Val(lbl.Text)
                    End If
                Next
                'lblNrItem.Text = grdItem_edit.Rows.Count & " items"
                lblTotalAmt.Text = String.Format("{0:c}", totalAmt)
                If totalAmt <> 0 AndAlso totalCost <> 0 Then
                    totalGpStr = FormatPercent(Math.Round((totalAmt - totalCost) / totalAmt, 2), 0)
                End If
                'lblOrderMsg.Text = grdItem_edit.Rows.Count & " items exist in this order. Total amount is <span class='red'>" & lblTotalAmt.Text & "</span>"
                If hidePrice = True Then
                    lblOrderMsg.Text = "Total: " & grdItem_edit.Rows.Count & " items"
                    btnInfo_orderMsg.Visible = False
                Else
                    lblOrderMsg.Text = "Total amount (" & grdItem_edit.Rows.Count & " items): <span class='red'>" & lblTotalAmt.Text & "</span>" & IIf(ViewState("ORDER_CMD") = "JFCADMIN", ", Total GP: <span class='red'>" & totalGpStr & "</span>", "")
                End If
                lblOrderMsg.Text &= IIf(showItemDtl = True, "&nbsp;&nbsp;&nbsp;&nbsp;<span class='green'>" & totalGrossWeight_kg & " kg, " & totalCaseVol_m3 & " m³</span>", "")
            ElseIf ViewState("ORDER_MODE") = "VIEW_JETS_ORDER" Then
                'Update order info
                totalAmt = 0
                For i As Integer = 0 To grdItem_view.Rows.Count - 1
                    lbl = grdItem_view.Rows.Item(i).FindControl("lblLineAmt")
                    totalAmt += Val(lbl.Text)
                Next
                lblTotalAmt.Text = String.Format("{0:c}", totalAmt)
            End If

        End Using
    End Sub


    Protected Overrides Sub OnPreRender(e As System.EventArgs)
        MyBase.OnPreRender(e)
    End Sub

    Protected Sub btnChangeOrder_Click(sender As Object, e As System.EventArgs) Handles btnChangeOrder.Click
        Using DB As New DBHelper

            ViewState("DIALOG_CMD") = "CHANGE_NEWSTATUS"
            ''Get email body for user from email template in user control
            'CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "CUSTOMER", "DELETED", txtEmailMsg.Text)
            'ViewState("DIALOG_ARG1") = emailBody
            ''Get email body for vendor from email template in user control
            'CreateEmailBody(Page, emailBody, "ORDER", webOrderID, "VENDOR", "DELETED", txtEmailMsg.Text)
            'ViewState("DIALOG_ARG2") = emailBody
            MessageBoxYesNo("Are you sure you want to change back to order placed status?", "Confirm Cancelation", DB)
        End Using


    End Sub

End Class
