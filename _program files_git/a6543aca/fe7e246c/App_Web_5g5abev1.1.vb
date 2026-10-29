#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\Balance.ascx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","A5AA0FDD9624C99BF5D4FC2C66EE15837E197421")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\Balance.ascx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
'Imports TransactionService

Partial Class Common_Balance
    Inherits System.Web.UI.UserControl
    Private mDB As New DBHelper
    Private Const mOldestIndex As Integer = -3
    Private SSL_Certificate_Validation_Errors As String

    '--------------------------------------------------------------------------------------------------------------------
    'Parameters passed by caller
    'Session("BALANCE_AVAIL_TABS")
    'Session("BALANCE_TAB")
    'Session("BALANCE_CMD"): Allow user to edit order for "CUST" and "JFCADMIN", view only for "VENDOR" and "CUSTADMIN".
    'Session("BALANCE_CUSTID") : Used only for "CUST"
    'Session("BALANCE_CUSTCOMPANYID"): Used only for "CUST" and "CUSTADMIN"
    '--------------------------------------------------------------------------------------------------------------------

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim availTabs As New List(Of Integer)
        'Dim customerID As String

        If IsPostBack = False Then
            'Get arguments
            ViewState("CUSTOMER_ID") = Request.QueryString("CUSTOMER_ID")
            ViewState("CUSTGRP_ID") = GetCustGrpID(mDB, ViewState("CUSTOMER_ID"))
            ViewState("CUSTCOMPANY_ID") = GetCustCompanyID(mDB, ViewState("CUSTGRP_ID"))
            ViewState("CMD") = Request.QueryString("CMD")

            'Init
            'Common.InitLastUpdate()
            pnlTabBalance.Visible = False
            pnlTabDownload.Visible = False
            DataBind() 'Fill in customer company type

            'Enable tabs
            availTabs = Session("BALANCE_AVAIL_TABS")
            If availTabs Is Nothing Then Exit Sub
            For Each s In availTabs
                Select Case s
                    Case Balance_Tab.BALANCE
                        pnlTabBalance.Visible = True
                    Case Balance_Tab.PAYMENT
                        pnlTabPayment.Visible = True
                    Case Balance_Tab.DOWNLOAD
                        pnlTabDownload.Visible = True
                End Select
            Next s

            '--------------------------------------------------------------------------------------------------------------------
            'Show account balance
            '--------------------------------------------------------------------------------------------------------------------
            If pnlTabBalance.Visible = True Then
                'Init
                If IsMobile() = False Then
                    pnlAftPay.Width = 978
                    pnlCustomer.Width = 978
                    pnlBalance.Width = 978
                End If
                Select Case Session("BALANCE_CMD")
                    Case "CUST"
                        'lblCustCompany_balance.Visible = False
                        'lstCustCompany_balance.Visible = False
                        'lblCustGrp_balance.Visible = False
                        'lstCustGrp_balance.Visible = False
                        'lblCustomer_balance.Visible = False
                        'lstCustomer_balance.Visible = False
                        pnlCustomer.Visible = False
                    Case "CUSTADMIN"
                        lblCustCompany_balance.Visible = False
                        lstCustCompany_balance.Visible = False
                        If SMALL_CUST_ONLY = True Then
                            lblCustGrp_balance.Text = GetCustCompanyType()
                            lblCustomer_balance.Visible = False
                            lstCustomer_balance.Visible = False
                        End If
                    Case "JFCADMIN"
                        If SMALL_CUST_ONLY = True Then
                            lblCustCompany_balance.Text = "Customer"
                            lblCustGrp_balance.Visible = False
                            lstCustGrp_balance.Visible = False
                            lblCustomer_balance.Visible = False
                            lstCustomer_balance.Visible = False
                        End If
                End Select

                'Show list of period
                Dim yyyymm As New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
                Dim format As String = "MMM yyyy"
                lstPeriod_balance.Items.Clear()
                For i As Integer = 0 To mOldestIndex + 1 Step -1
                    lstPeriod_balance.Items.Add(New ListItem(yyyymm.AddMonths(i).ToString(format), i))
                    'dropDownListPeriod_DL.Items.Add(New ListItem(yyyymm.AddMonths(i).ToString(format), yyyymm.AddMonths(i).ToString("yyyyMM")))
                Next
                'lstPeriod_balance.Items.Add(New ListItem("Older", mOldestIndex))

                'Show list of customer company
                Common.ShowCustCompanyList(mDB, GetUserEmail(mDB), IIf(Session("BALANCE_CMD") = "CUSTADMIN", True, False), lstCustCompany_balance)
                If ViewState("CUSTCOMPANY_ID") <> String.Empty Then
                    lstCustCompany_balance.Text = ViewState("CUSTCOMPANY_ID")
                ElseIf Session("BALANCE_CMD") = "CUST" OrElse Session("BALANCE_CMD") = "CUSTADMIN" Then
                    lstCustCompany_balance.Text = Session("BALANCE_CUSTCOMPANYID")
                End If
                ''Show list of customer group and customer
                'lstCustCompany_balance_SelectedIndexChanged(Nothing, Nothing)
                'Show list of customer group
                Common.ShowCustGrpList(mDB, lstCustCompany_balance.Text, lstCustGrp_balance)
                If ViewState("CUSTGRP_ID") <> String.Empty Then lstCustGrp_balance.Text = ViewState("CUSTGRP_ID")
                'Show list of customer
                Common.ShowCustomerList(mDB, lstCustGrp_balance.Text, lstCustomer_balance)
                If ViewState("CUSTOMER_ID") <> String.Empty Then lstCustomer_balance.Text = ViewState("CUSTOMER_ID")

                'Restrict access for JFC Admin if customer company, customer group or customer has been set for the user
                'If Session("ORDER_HISTORY_CMD") = "JFCADMIN" Then
                '    SQL = "SELECT * FROM webuser_vendor WHERE vendorCompany_id = 'JFC' AND email = " & mDB.SqlEsc(GetUserEmail(mDB))
                '    r = mDB.GetReader(SQL)
                '    If r.Read() Then
                '        lstCustCompany_balance.Text = CString(r("custCompany_id"))
                '        lstCustCompany_balance.Enabled = IIf(CString(r("custCompany_id")) = String.Empty, True, False)
                '        Common.ShowCustGrpList(mDB, lstCustCompany_balance.Text, lstCustGrp_balance)
                '        lstCustGrp_balance.Text = CString(r("custGrp_id"))
                '        lstCustGrp_balance.Enabled = IIf(CString(r("custGrp_id")) = String.Empty, True, False)
                '        Common.ShowCustomerList(mDB, lstCustGrp_balance.Text, lstCustomer_balance)
                '        lstCustomer_balance.Text = CString(r("customer_id"))
                '        lstCustomer_balance.Enabled = IIf(CString(r("customer_id")) = String.Empty, True, False)
                '    End If
                '    r.close()
                'End If

                'Show sort by
                lstOrderBy_balance.Items.Clear()
                lstOrderBy_balance.Items.Add(New ListItem("Date", "seq DESC, transType ASC"))
                lstOrderBy_balance.Items.Add(New ListItem("Type", "transType ASC, seq DESC"))

                'Show account balance
                showBalance()
                If CString(ViewState("CMD")).ToUpper = "SHOW_DTL" Then
                    btnPay_Click(Nothing, Nothing)
                End If
            End If

            '--------------------------------------------------------------------------------------------------------------------
            'Payment History
            '--------------------------------------------------------------------------------------------------------------------
            If pnlTabPayment.Visible = True Then
                'Init
                Select Case Session("BALANCE_CMD")
                    Case "CUST"
                        lblCustCompany_payment.Visible = False
                        lstCustCompany_payment.Visible = False
                        lblCustGrp_payment.Visible = False
                        lstCustGrp_payment.Visible = False
                        lblCustomer_payment.Visible = False
                        lstCustomer_payment.Visible = False
                        If IsMobile() Then pnlSearchPayment.Visible = False
                    Case "CUSTADMIN"
                        lblCustCompany_payment.Visible = False
                        lstCustCompany_payment.Visible = False
                        If SMALL_CUST_ONLY = True Then
                            lblCustGrp_payment.Text = GetCustCompanyType()
                            lblCustomer_payment.Visible = False
                            lstCustomer_payment.Visible = False
                        End If
                    Case "JFCADMIN"
                        If SMALL_CUST_ONLY = True Then
                            lblCustCompany_payment.Text = "Customer"
                            lblCustGrp_payment.Visible = False
                            lstCustGrp_payment.Visible = False
                            lblCustomer_payment.Visible = False
                            lstCustomer_payment.Visible = False
                        End If
                End Select
                'customerID = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_payment.Text)

                'Show list of customer company
                Common.ShowCustCompanyList(mDB, GetUserEmail(mDB), IIf(Session("BALANCE_CMD") = "CUSTADMIN", True, False), lstCustCompany_payment)
                If ViewState("CUSTCOMPANY_ID") <> String.Empty Then
                    lstCustCompany_payment.Text = ViewState("CUSTCOMPANY_ID")
                ElseIf Session("BALANCE_CMD") = "CUST" OrElse Session("BALANCE_CMD") = "CUSTADMIN" Then
                    lstCustCompany_payment.Text = Session("BALANCE_CUSTCOMPANYID")
                End If
                'Show list of customer group and customer
                'lstCustCompany_payment_SelectedIndexChanged(Nothing, Nothing)
                'Show list of customer group
                Common.ShowCustGrpList(mDB, lstCustCompany_payment.Text, lstCustGrp_payment)
                If ViewState("CUSTGRP_ID") <> String.Empty Then lstCustGrp_payment.Text = ViewState("CUSTGRP_ID")
                'Show list of customer
                Common.ShowCustomerList(mDB, lstCustGrp_payment.Text, lstCustomer_payment)
                If ViewState("CUSTOMER_ID") <> String.Empty Then lstCustomer_payment.Text = ViewState("CUSTOMER_ID")
                'Show document type
                'Common.ShowCodeMasterList(mDB, "DOCTYPE", lstDocType, True, True, LST_ALL_STR)

                ''Restrict access for JFC Admin if customer company, customer group or customer has been set for the user
                'If Session("ORDER_HISTORY_CMD") = "JFCADMIN" Then
                '    SQL = "SELECT * FROM webuser_vendor WHERE vendorCompany_id = 'JFC' AND email = " & mDB.SqlEsc(GetUserEmail(mDB))
                '    r = mDB.GetReader(SQL)
                '    If r.Read() Then
                '        lstCustCompany_payment.Text = CString(r("custCompany_id"))
                '        lstCustCompany_payment.Enabled = IIf(CString(r("custCompany_id")) = String.Empty, True, False)
                '        Common.ShowCustGrpList(mDB, lstCustCompany_payment.Text, lstCustGrp_payment)
                '        lstCustGrp_payment.Text = CString(r("custGrp_id"))
                '        lstCustGrp_payment.Enabled = IIf(CString(r("custGrp_id")) = String.Empty, True, False)
                '        Common.ShowCustomerList(mDB, lstCustGrp_payment.Text, lstCustomer_payment)
                '        lstCustomer_payment.Text = CString(r("customer_id"))
                '        lstCustomer_payment.Enabled = IIf(CString(r("customer_id")) = String.Empty, True, False)
                '    End If
                '    r.close()
                'End If

                'Show sort by
                lstOrderBy_payment.Items.Clear()
                lstOrderBy_payment.Items.Add(New ListItem("Date: New to old", "createDate DESC"))
                lstOrderBy_payment.Items.Add(New ListItem("Date: Old to new", "createDate ASC"))

                'Show payment history
                showPaymentHistory()
            End If

            '--------------------------------------------------------------------------------------------------------------------
            'Download document
            '--------------------------------------------------------------------------------------------------------------------
            If pnlTabDownload.Visible = True Then
                'Init
                Select Case Session("BALANCE_CMD")
                    Case "CUST"
                        lblCustCompany_download.Visible = False
                        lstCustCompany_download.Visible = False
                        lblCustGrp_download.Visible = False
                        lstCustGrp_download.Visible = False
                        lblCustomer_download.Visible = False
                        lstCustomer_download.Visible = False
                    Case "CUSTADMIN"
                        lblCustCompany_download.Visible = False
                        lstCustCompany_download.Visible = False
                        If SMALL_CUST_ONLY = True Then
                            lblCustGrp_download.Text = GetCustCompanyType()
                            lblCustomer_download.Visible = False
                            lstCustomer_download.Visible = False
                        End If
                    Case "JFCADMIN"
                        If SMALL_CUST_ONLY = True Then
                            lblCustCompany_download.Text = "Customer"
                            lblCustGrp_download.Visible = False
                            lstCustGrp_download.Visible = False
                            lblCustomer_download.Visible = False
                            lstCustomer_download.Visible = False
                        End If
                End Select
                'customerID = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_download.Text)

                'Show list of customer company
                Common.ShowCustCompanyList(mDB, GetUserEmail(mDB), IIf(Session("BALANCE_CMD") = "CUSTADMIN", True, False), lstCustCompany_download)
                If ViewState("CUSTCOMPANY_ID") <> String.Empty Then
                    lstCustCompany_download.Text = ViewState("CUSTCOMPANY_ID")
                ElseIf Session("BALANCE_CMD") = "CUST" OrElse Session("BALANCE_CMD") = "CUSTADMIN" Then
                    lstCustCompany_download.Text = Session("BALANCE_CUSTCOMPANYID")
                End If
                'Show list of customer group and customer
                'Show list of customer group and customer
                'lstCustCompany_download_SelectedIndexChanged(Nothing, Nothing)
                'Show list of customer group
                Common.ShowCustGrpList(mDB, lstCustCompany_download.Text, lstCustGrp_download)
                If ViewState("CUSTGRP_ID") <> String.Empty Then lstCustGrp_download.Text = ViewState("CUSTGRP_ID")
                'Show list of customer
                Common.ShowCustomerList(mDB, lstCustGrp_download.Text, lstCustomer_download)
                If ViewState("CUSTOMER_ID") <> String.Empty Then lstCustomer_download.Text = ViewState("CUSTOMER_ID")
                'Show document type
                Common.ShowCodeMasterList(mDB, "DOCTYPE", lstDocType, True, True, LST_ALL_STR)

                ''Restrict access for JFC Admin if customer company, customer group or customer has been set for the user
                'If Session("ORDER_HISTORY_CMD") = "JFCADMIN" Then
                '    SQL = "SELECT * FROM webuser_vendor WHERE vendorCompany_id = 'JFC' AND email = " & mDB.SqlEsc(GetUserEmail(mDB))
                '    r = mDB.GetReader(SQL)
                '    If r.Read() Then
                '        lstCustCompany_download.Text = CString(r("custCompany_id"))
                '        lstCustCompany_download.Enabled = IIf(CString(r("custCompany_id")) = String.Empty, True, False)
                '        Common.ShowCustGrpList(mDB, lstCustCompany_download.Text, lstCustGrp_download)
                '        lstCustGrp_download.Text = CString(r("custGrp_id"))
                '        lstCustGrp_download.Enabled = IIf(CString(r("custGrp_id")) = String.Empty, True, False)
                '        Common.ShowCustomerList(mDB, lstCustGrp_download.Text, lstCustomer_download)
                '        lstCustomer_download.Text = CString(r("customer_id"))
                '        lstCustomer_download.Enabled = IIf(CString(r("customer_id")) = String.Empty, True, False)
                '    End If
                '    r.close()
                'End If

                'Show sort by
                lstOrderBy_download.Items.Clear()
                lstOrderBy_download.Items.Add(New ListItem("Date: New to old", "createDate DESC"))
                lstOrderBy_download.Items.Add(New ListItem("Date: Old to new", "createDate ASC"))
                'lstOrderBy_download.Items.Add(New ListItem("Document Type", "docType ASC, createDate DESC"))

                'Show download files
                showDownload()
            End If
        End If
    End Sub

    ''Occurs after postback event is raised
    'Protected Overrides Sub OnPreRender(e As EventArgs)
    '    MyBase.OnPreRender(e) 'Need this?
    '    ViewState("UPDATE") = Common.GetLastUpdate()
    'End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Show account balance
    '--------------------------------------------------------------------------------------------------------------------
    Protected Sub lstCustCompany_balance_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany_balance.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        ShowCustGrpList(mDB, lstCustCompany_balance.Text, lstCustGrp_balance)
        lstCustGrp_balance_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustGrp_balance_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustGrp_balance.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        ShowCustomerList(mDB, lstCustGrp_balance.Text, lstCustomer_balance)
        lstCustomer_balance_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustomer_balance_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustomer_balance.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        showBalance()
    End Sub

    Protected Sub btnPay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPay.Click
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        'Init
        DeleteTempPaymentAcc(mDB, False) 'Delete temp account
        chkAddAdvancePayment.Checked = False
        chkAddAdvancePayment_CheckedChanged(Nothing, Nothing)
        chkInputPayAmt.Checked = False
        chkInputPayAmt_CheckedChanged(Nothing, Nothing)
        'Show new payment
        showPayment(customerID)
    End Sub

    Protected Sub btnAutoPay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAutoPay.Click, lnkAutoPay.Click
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        'Show auto payment
        showAutoPay(customerID)
    End Sub

    'Protected Sub grdPending_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPending.RowCommand
    '    Dim paymentID As String

    '    Session("BALANCE_TAB") = Balance_Tab.BALANCE
    '    If e.CommandName = "VIEW_ROW" Then
    '        'Show order detail
    '        paymentID = Convert.ToString(e.CommandArgument)
    '        showPaymentDtl_balance(paymentID)
    '    End If
    'End Sub

    Protected Sub lstPeriod_balance_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstPeriod_balance.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        showBalance()
    End Sub

    Protected Sub lstOrderBy_balance_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_balance.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        showBalance()
    End Sub

    Protected Sub grdBalance_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdBalance.RowCommand
        'Dim row As GridViewRow = Nothing
        Dim reference As String
        Dim customerID As String
        Dim SQL As String
        Dim r As DataReader
        Dim rtn As Boolean = False

        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        If e.CommandName = "Page" Then
            'Page is changed
            showBalance()
            'Don't remember scroll position
            Page.MaintainScrollPositionOnPostBack = False
        ElseIf e.CommandName = "DOWNLOAD" Then
            'Download reference
            'row = grdBalance.Rows(CType(e.CommandArgument, Integer))
            'reference = CType(row.Cells(2).Controls(0), LinkButton).Text
            reference = Convert.ToString(e.CommandArgument)
            customerID = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
            SQL = "SELECT directory, fileName FROM customer_document " & _
                        "WHERE UPPER(docType) = 'INVOICE' AND customer_id = :1 AND docName = :2"
            r = mDB.GetReader(SQL, customerID, reference)
            If r.Read Then
                rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
            End If
            If rtn = False Then MessageBox("Cannot open the document", "Error", mDB)
        End If
    End Sub

    Protected Sub lnkBack_balance_Click(sender As Object, e As System.EventArgs) Handles lnkNewPaymentBack_balance.Click, lnkAutoPayBack_balance.Click 'lnkPaymentDtlBack_balance.Click,
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        'Return back to transaction list
        pnlList_balance.Visible = True
        pnlNewPayment_balance.Visible = False
        'pnlPaymentDtl_balance.Visible = False
        pnlAutoPay_balance.Visible = False
        pnlAftPayment.Visible = False
    End Sub

    'Protected Sub btnRefreshPayAmt_Click(sender As Object, e As System.EventArgs) Handles btnRefreshPayAmt.Click, lnkRefreshPayAmt.Click
    '    Dim errMsg As String = ""

    '    Session("BALANCE_TAB") = Balance_Tab.BALANCE
    '    If calculateTotalPayAmt(errMsg) = False Then
    '        MessageBox(errMsg, "Error", mDB)
    '        Exit Sub
    '    End If
    'End Sub

    Protected Sub chkLiquor_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkSelectLiquor.CheckedChanged, chkHideLiquorInterest.CheckedChanged
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        showPayment(customerID)
        If chkInputPayAmt.Checked = True AndAlso Common.Val(txtInputPayAmt.Text) > 0 Then
            'Apply / refresh input payment amount
            lnkApplyInputPayAmt_Click(Nothing, Nothing)
        End If
    End Sub

    Protected Sub chkInputPayAmt_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkInputPayAmt.CheckedChanged
        'Dim btn As ImageButton
        Dim txt As TextBox

        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        'Set properties
        txtInputPayAmt.Visible = chkInputPayAmt.Checked
        txtInputPayAmt.Text = String.Empty
        lnkApplyInputPayAmt.Visible = chkInputPayAmt.Checked
        lblPayAmt.CssClass = "rowLblVal fontsize6 " & IIf(chkInputPayAmt.Checked, "normal gray", "bold red")
        chkAddAdvancePayment.Visible = Not chkInputPayAmt.Checked
        lblAddAdvancePayment.Visible = Not chkInputPayAmt.Checked
        txtAdvancePayAmt.Visible = (Not chkInputPayAmt.Checked) AndAlso chkAddAdvancePayment.Checked
        If txtAdvancePayAmt.Visible = False Then ClearCtrl({txtAdvancePayAmt})
        txtAdvancePayAmt.Enabled = Not chkInputPayAmt.Checked
        For i As Integer = 0 To grdPayDtl.Rows.Count - 1
            'btn = grdPayDtl.Rows.Item(i).FindControl("btnCheck")
            'btn.Enabled = Not chkInputPayAmt.Checked
            txt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
            txt.Enabled = Not chkInputPayAmt.Checked
            txt = grdPayDtl.Rows.Item(i).FindControl("txtNote")
            txt.Enabled = Not chkInputPayAmt.Checked
        Next

        'Show total payment amount
        If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
        'calculateTotalPayAmt()
    End Sub

    Protected Sub chkAddAdvancePayment_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkAddAdvancePayment.CheckedChanged
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        txtAdvancePayAmt.Visible = chkAddAdvancePayment.Checked
        If txtAdvancePayAmt.Visible = False Then ClearCtrl({txtAdvancePayAmt})
        txtAdvancePayAmt.Text = String.Empty
        'lnkApplyAdvancePayAmt.Visible = AllowCustInputPayAmt(mDB, customerID) = False AndAlso chkAddAdvancePayment.Checked
        'Show total payment amount
        If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
        'calculateTotalPayAmt()
    End Sub

    Protected Sub lnkApplyInputPayAmt_Click(sender As Object, e As System.EventArgs) Handles lnkApplyInputPayAmt.Click
        Dim chk As CheckBox
        Dim txtPayAmt, txtNote As TextBox
        Dim lbl As Label
        Dim btn As ImageButton
        Dim leftAmt, balanceAmt As Decimal
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)

        Session("BALANCE_TAB") = Balance_Tab.BALANCE

        'Check error
        If Common.Val(txtInputPayAmt.Text) <= 0 Then
            MessageBox("Payment amount has to be a positive number", "Error", mDB)
            Exit Sub
        End If

        'Apply input payment amount to invoices
        leftAmt = Common.Val(txtInputPayAmt.Text)
        For i As Integer = 0 To grdPayDtl.Rows.Count - 1
            'Clear row
            chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
            chk.Checked = False
            btn = grdPayDtl.Rows.Item(i).FindControl("btnCheck")
            btn.ImageUrl = GetWebImgUrl_OnOff("CHECKED", "OFF")
            txtPayAmt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
            txtPayAmt.Visible = False
            ClearCtrl({txtPayAmt})
            txtNote = grdPayDtl.Rows.Item(i).FindControl("txtNote")
            txtNote.Visible = False
            ClearCtrl({txtNote})
            lbl = grdPayDtl.Rows.Item(i).FindControl("lblStar_note")
            ClearCtrl({lbl})
            lbl = grdPayDtl.Rows.Item(i).FindControl("lblNoWebDisc")
            ClearCtrl({lbl})
            grdPayDtl.Rows.Item(i).CssClass = ""
            'Clear advance payment
            chkAddAdvancePayment.Checked = False
            txtAdvancePayAmt.Visible = False
            ClearCtrl({txtAdvancePayAmt})
            'Get row balance
            lbl = grdPayDtl.Rows.Item(i).FindControl("lblBalanceAmt")
            balanceAmt = Common.Val(lbl.Text)
            'Apply input payment amount
            If AllowCustInputPayAmt(mDB, customerID) = True Then
                'User can change payment amount for each row
                If leftAmt > 0 Then
                    chk.Checked = True
                    btn.ImageUrl = GetWebImgUrl_OnOff("CHECKED", "ON")
                    txtPayAmt.Visible = True
                    txtPayAmt.Text = IIf(leftAmt < balanceAmt, leftAmt, balanceAmt)
                    txtNote.Visible = True
                    txtNote.Text = IIf(leftAmt < balanceAmt, "Short pay from entered payment amount", "")
                    grdPayDtl.Rows.Item(i).CssClass = "bold"
                    leftAmt -= Common.Val(txtPayAmt.Text)
                End If
            Else
                'User cannot change payment amount for each row
                If leftAmt > 0 AndAlso leftAmt >= balanceAmt Then
                    chk.Checked = True
                    btn.ImageUrl = GetWebImgUrl_OnOff("CHECKED", "ON")
                    grdPayDtl.Rows.Item(i).CssClass = "bold"
                    leftAmt -= balanceAmt
                End If
            End If
        Next
        If leftAmt > 0 Then
            'Add the left in advance payment
            chkAddAdvancePayment.Checked = True
            lblAddAdvancePayment.Visible = True
            txtAdvancePayAmt.Visible = True
            txtAdvancePayAmt.Text = leftAmt
        End If
        'Set properties
        lblPayAmt.CssClass = "rowLblVal bold fontsize6 red"
        'Show total payment amount
        If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
        'calculateTotalPayAmt()
    End Sub

    'Protected Sub lnkApplyAdvancePayAmt_Click(sender As Object, e As System.EventArgs) Handles lnkApplyAdvancePayAmt.Click
    '    Session("BALANCE_TAB") = Balance_Tab.BALANCE

    '    'Show total payment amount
    '    if Session("BALANCE_CMD") = "CUST" then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
    '    'calculateTotalPayAmt()
    'End Sub

    Protected Sub lnkAddPaymentAcc_Click(sender As Object, e As System.EventArgs) Handles lnkAddPaymentAcc.Click, lnkAddAutoPayAcc.Click
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim curYear As String = DateTime.Now.Year.ToString()

        Session("BALANCE_TAB") = Balance_Tab.BALANCE

        'Show total payment amount (need to call both calculateTotalPayAmt())
        If sender.id = "lnkAddPaymentAcc" AndAlso Session("BALANCE_CMD") = "CUST" Then
            CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
            calculateTotalPayAmt()
        End If

        'Show list for adding a new account
        ShowCodeMasterList(mDB, "PAYMENTTYPE", lstPopPaymentType, False, True, LST_CMD_STR)
        ShowCodeMasterList(mDB, "CARDTYPE", lstPopCardType, False, True, LST_CMD_STR)
        lstPopMonth.Items.Clear()
        For i As Integer = 1 To 12
            lstPopMonth.Items.Add(New ListItem(addPrefixZero(i.ToString, 2)))
        Next
        For i As Integer = curYear To curYear + 20
            lstPopYear.Items.Add(New ListItem(addPrefixZero(i.ToString, 4)))
        Next

        'Set properties
        'pnlAddPaymentAcc.Visible = True
        pnlPopBankAcc.Visible = False
        pnlPopCard.Visible = False
        imgPopCheck.Visible = False
        lblPopNewAccCmd.Text = IIf(sender.id = "lnkAddPaymentAcc", "NEW_PAYMENT", "AUTO_PAY")
        lstPopPaymentType.Text = String.Empty
        lstPopCardType.Text = String.Empty
        lstPopMonth.SelectedIndex = 0
        lstPopYear.SelectedIndex = 0
        If sender.id = "lnkAddAutoPayAcc" Then chkPopRememberAcc.Checked = True
        chkPopRememberAcc.Visible = IIf(sender.id = "lnkAddPaymentAcc", True, False)
        ClearCtrl({txtPopAccName, txtPopRoutingNr, txtPopBankAccNr, txtPopBankAccNr2, txtPopCardNr, txtPopSecurityCode})
        ClearCtrl({lblPopErrMsg})

        If ALLOW_CARD_PAYMENT = False Then
            'Show input feild for checking and saving account
            lstPopPaymentType_SelectedIndexChanged(Nothing, Nothing)
        End If

        'Delete temp account
        DeleteTempPaymentAcc(mDB)
        'Refresh account list
        ShowPaymentAccList(mDB, customerID, lstPaymentAcc)

        'Show popup
        popDialog.Show()
    End Sub

    Protected Sub lstPopPaymentType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstPopPaymentType.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        pnlPopBankAcc.Visible = False
        pnlPopCard.Visible = False
        imgPopCheck.Visible = False
        If lstPopPaymentType.Text = "CARD" Then
            pnlPopCard.Visible = True
        Else
            pnlPopBankAcc.Visible = True
            imgPopCheck.Visible = True
        End If
        If Common.ALLOW_CARD_PAYMENT = True Then popDialog.Show() 'Must have If here
    End Sub

    Protected Sub btnPopAddPaymentAcc_Click(sender As Object, e As System.EventArgs) Handles btnPopAddPaymentAcc.Click
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim accNickName As String = ""
        Dim dtExp, dtFirstDayOfMonth As DateTime

        Session("BALANCE_TAB") = Balance_Tab.BALANCE

        'Check error
        If lstPopPaymentType.Text = String.Empty Then
            lblPopErrMsg.Text = "Please select a payment type"
            popDialog.Show()
            Exit Sub
        End If
        If txtPopAccName.Text.Trim = String.Empty Then
            lblPopErrMsg.Text = "Please enter name on this account"
            popDialog.Show()
            Exit Sub
        End If
        If txtPopAccName.Text.Length > 50 Then
            lblPopErrMsg.Text = "Name on account cannot exceed 50 characters"
            popDialog.Show()
            Exit Sub
        End If
        If lstPopPaymentType.Text = "CARD" Then
            If lstPopCardType.Text = String.Empty Then
                lblPopErrMsg.Text = "Please select a card type"
                popDialog.Show()
                Exit Sub
            End If
            If IsNumber(txtPopCardNr.Text) = False OrElse txtPopCardNr.Text.Length > 16 Then
                lblPopErrMsg.Text = "Invalid card number"
                popDialog.Show()
                Exit Sub
            End If
            If ExistCardNr(mDB, customerID, txtPopCardNr.Text) = True Then
                lblPopErrMsg.Text = "This card exists in your account list. Please go to your account if you need to modify"
                popDialog.Show()
                Exit Sub
            End If
            DateTime.TryParseExact(CString(lstPopMonth.Text) & "/01/" & CString(lstPopYear.Text), DATE_FMT, Nothing, Nothing, dtExp)
            dtFirstDayOfMonth = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            If dtExp < dtFirstDayOfMonth Then
                lblPopErrMsg.Text = "Expiration date cannot be past. "
                popDialog.Show()
                Exit Sub
            End If
            If IsNumber(txtPopSecurityCode.Text) = False OrElse txtPopSecurityCode.Text.Length > 5 Then
                lblPopErrMsg.Text = "Invalid security code"
                popDialog.Show()
                Exit Sub
            End If
        Else
            If IsNumber(txtPopRoutingNr.Text) = False OrElse txtPopRoutingNr.Text.Length <> 9 Then
                lblPopErrMsg.Text = "Invalid routing #"
                popDialog.Show()
                Exit Sub
            End If
            If IsNumber(txtPopBankAccNr.Text) = False OrElse txtPopBankAccNr.Text.Length > 17 Then
                lblPopErrMsg.Text = "Invalid bank account #"
                popDialog.Show()
                Exit Sub
            End If
            If txtPopBankAccNr.Text <> txtPopBankAccNr2.Text Then
                lblPopErrMsg.Text = "Bank account # does not match"
                popDialog.Show()
                Exit Sub
            End If
            If ExistBankAccNr(mDB, customerID, txtPopBankAccNr.Text) = True Then
                lblPopErrMsg.Text = "This bank account # has already exist in your list. Please go to your account if you need to modify."
                popDialog.Show()
                Exit Sub
            End If
        End If

        'Create an payment account
        If lstPopPaymentType.Text = "CARD" Then
            'Credit or debit card
            UpSertPaymentAcc(mDB, customerID, accNickName, lstPopPaymentType.Text, txtPopAccName.Text.Trim, Nothing, Nothing, lstPopCardType.Text, txtPopCardNr.Text, lstPopMonth.Text, lstPopYear.Text, txtPopSecurityCode.Text, IIf(chkPopRememberAcc.Checked = False, "Y", ""))
        Else
            'Savings or checking account
            UpSertPaymentAcc(mDB, customerID, accNickName, lstPopPaymentType.Text, txtPopAccName.Text.Trim, txtPopRoutingNr.Text, txtPopBankAccNr.Text, Nothing, Nothing, Nothing, Nothing, Nothing, IIf(chkPopRememberAcc.Checked = False, "Y", ""))
        End If
        InsertEventLog(mDB, "Added payment account", customerID, accNickName, Nothing, True)
        'MessageLine("CONFIRM", "Successfully added")

        'Hide add payment account
        'pnlAddPaymentAcc.Visible = False

        'Refresh available account list and select the added account
        If lblPopNewAccCmd.Text = "NEW_PAYMENT" Then
            ShowPaymentAccList(mDB, customerID, lstPaymentAcc)
            If lstPaymentAcc.Items.FindByValue(accNickName) IsNot Nothing Then lstPaymentAcc.Items.FindByValue(accNickName).Selected = True
        ElseIf lblPopNewAccCmd.Text = "AUTO_PAY" Then
            ShowPaymentAccList(mDB, customerID, lstAutoPayAcc)
            If lstAutoPayAcc.Items.FindByValue(accNickName) IsNot Nothing Then lstAutoPayAcc.Items.FindByValue(accNickName).Selected = True
        End If
    End Sub

    'Protected Sub lnkCancelPaymentAcc_Click(sender As Object, e As System.EventArgs) Handles lnkCancelPaymentAcc.Click
    '    Session("BALANCE_TAB") = Balance_Tab.BALANCE
    '    'Hide add payment account
    '    pnlAddPaymentAcc.Visible = False
    'End Sub

    Protected Sub grdPayDtl_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdPayDtl.RowDataBound
        Dim btn As ImageButton
        Dim lbl As Label
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim webDiscType As String = GetCustWebDiscType(mDB, customerID)
        'Dim webDiscRate As Decimal = GetCustWebDiscRate(mDB, customerID)
        Dim webDiscDesc_A As String = WebdiscRate_A & "% off if paid within " & WebDiscDays_A & " days"
        Dim webDiscDesc_B As String = WebdiscRate_B & "% off if paid within " & WebDiscDays_B & " days"
   
        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        If grdPayDtl.HeaderRow IsNot Nothing Then
            lbl = grdPayDtl.HeaderRow.FindControl("lblWebDiscHdr")
            btn = grdPayDtl.HeaderRow.FindControl("btnInfo_webDisc")
            If webDiscType = String.Empty OrElse webDiscType = "Z" Then
                'Cannot make the column invisible because of javascript:calculateTotalPayAmt()
                lbl.Text = String.Empty 'Web Discount
                btn.ImageUrl = GetWebImgUrl("empty.jpg")
                btn.ToolTip = String.Empty
            ElseIf webDiscType = "A" Then
                lbl.Text = "Web Early Payment Discount" 'Web Discount
                btn.ImageUrl = GetWebImgUrl("question.png")
                btn.ToolTip = webDiscDesc_A
            ElseIf webDiscType = "B" Then
                lbl.Text = "Web Early Payment Discount" 'Web Discount
                btn.ImageUrl = GetWebImgUrl("question.png")
                btn.ToolTip = webDiscDesc_B
            End If
        End If
    End Sub

    Protected Sub grdPayDtl_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPayDtl.RowCommand
        Dim index As Integer = 0
        Dim chk As CheckBox
        Dim btn As ImageButton
        Dim txt As TextBox
        Dim lbl As Label
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim docName As String
        Dim SQL As String
        Dim r As DataReader
        Dim rtn As Boolean = False
        Dim allowCustInputPayAmt As Boolean = Common.AllowCustInputPayAmt(mDB, customerID)
        Dim balanceAmt, payAmt As Decimal

        Session("BALANCE_TAB") = Balance_Tab.BALANCE

        If e.CommandName = "CHANGE_ROW" Then
            'Checked or unchecked row
            index = Convert.ToInt32(e.CommandArgument)
            chk = grdPayDtl.Rows.Item(index).FindControl("chkSelect")
            btn = grdPayDtl.Rows.Item(index).FindControl("btnCheck")
            'Flip the check
            btn.ImageUrl = IIf(chk.Checked = True, GetWebImgUrl_OnOff("CHECKED", "OFF"), GetWebImgUrl_OnOff("CHECKED", "ON"))
            chk.Checked = IIf(chk.Checked = True, False, True)
            'Visible or invisible textbox
            lbl = grdPayDtl.Rows.Item(index).FindControl("lblBalanceAmt")
            balanceAmt = Common.Val(lbl.Text)
            txt = grdPayDtl.Rows.Item(index).FindControl("txtPayAmt")
            txt.Visible = chk.Checked AndAlso allowCustInputPayAmt
            payAmt = Common.Val(txt.Text)
            txt = grdPayDtl.Rows.Item(index).FindControl("txtNote")
            txt.Visible = chk.Checked AndAlso allowCustInputPayAmt
            grdPayDtl.Rows.Item(index).CssClass = IIf(chk.Checked, "bold", "")
            'lbl = grdPayDtl.Rows.Item(index).FindControl("lblWebDiscAmt_View")
            'lbl.CssClass = IIf(chk.Checked AndAlso balanceAmt > payAmt, "line-through", "nodecor")
            ''lbl.Visible = GetCustWebDiscType(mDB, customerID) <> String.Empty AndAlso balanceAmt <= payAmt
            'lbl = grdPayDtl.Rows.Item(index).FindControl("lblStar_note")
            'lbl.Visible = chk.Checked AndAlso allowCustInputPayAmt
            'lbl = grdPayDtl.Rows.Item(index).FindControl("lblNoWebDisc")
            'lbl.Visible = chk.Checked AndAlso allowCustInputPayAmt
            'Show total payment amount
            'calculateTotalPayAmt()
            If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
            'ElseIf e.CommandName = "CHANGE_PAYAMT" Then
            '    'Show total payment amount
            '    calculateTotalPayAmt()
        ElseIf e.CommandName = "DOWNLOAD" Then
            'Download reference
            'row = grdDownload.Rows(CType(e.CommandArgument, Integer))
            'docName = CType(row.Cells(2).Controls(0), LinkButton).Text
            docName = Convert.ToString(e.CommandArgument)
            SQL = "SELECT directory, fileName FROM customer_document WHERE customer_id = :1 AND docName = :2"
            r = mDB.GetReader(SQL, customerID, docName)
            If r.Read Then
                rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
            End If
            If rtn = False Then MessageBox("Cannot open the document", "Error", mDB)
        ElseIf e.CommandName = "CHANGE_ALL_ROW" Then
            btn = grdPayDtl.HeaderRow.FindControl("btnAll")
            'TODO: Check / uncheck all rows

        ElseIf e.CommandName = "INFO_WEBDISC" Then
            'Show web discount description
            btn = grdPayDtl.HeaderRow.FindControl("btnInfo_webDisc")
            'Show total payment amount
            'calculateTotalPayAmt()
            If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
            MessageBox(btn.ToolTip, "Information")
        End If
    End Sub

    'Using Forte Advanced Gateway Inerface (AGI) Direct Socket Interface
    'http://www.forte.net/devdocs/pdf/agi_integration.pdf
    'Reference:
    'http://msdn.microsoft.com/en-us/magazine/cc163454.aspx#S7
    'http://social.msdn.microsoft.com/Forums/en-US/netfxnetcom/thread/8bcf6b40-619e-4151-9885-22242b7e95b0

    Protected Sub btnSubmitPayment_Click(sender As Object, e As System.EventArgs) Handles btnSubmitPayment.Click
        Dim myTCPClient As New Sockets.TcpClient
        Dim mySSLStream As SslStream = Nothing
        Dim paramAry As String() = Nothing
        Dim payAmt As Decimal
        Dim approvedAmt As Decimal
        Dim SQL As String
        Dim r As DataReader
        Dim dtExp, dtFirstDayOfMonth As DateTime
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim customerName As String = GetCustName(mDB, customerID)
        Dim responseCode, responseDesc, traceNr, authorizationCode, transactionType As String
        Dim paymentID As String
        Dim emailBody As String = ""
        Dim errMsg As String = ""
        Dim msg As String = ""
        Dim userEmail As String = GetUserEmail(mDB)
        Dim emailSent As Boolean
        Dim chk As CheckBox
        Dim txt As TextBox
        Dim lbl As Label
        Dim note As String
        Dim paymentLineAmt As Decimal
        Dim vendorEmail As String
        Dim balanceAmt, pendingAmt, webDiscAmt As Decimal
        Dim isProduction As Boolean = Common.IsProduction()
        Dim jfcBranchNr As String = ""

        Session("BALANCE_TAB") = Balance_Tab.BALANCE
        'If ViewState("UPDATE").ToString() <> GetLastUpdate() Then Exit Sub 'Do nothing if page refreshed

        'Refresh total payment amount (need to call both calculateTotalPayAmt())
        If Session("BALANCE_CMD") = "CUST" Then
            CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();") 'To show * or No Discount in case error occurs
            calculateTotalPayAmt() 'lblPayAmt.Text is empty, so need to call calculateTotalPayAmt()
        End If
        payAmt = Common.Val(lblPayAmt.Text)

        'Check error
        If AllowCustInputPayAmt(mDB, customerID) = True Then
            'Check if reason is written if payment amount is less than remainding balance
            For i As Integer = 0 To grdPayDtl.Rows.Count - 1
                chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
                If chk.Checked = True Then
                    txt = grdPayDtl.Rows.Item(i).FindControl("txtNote")
                    note = txt.Text.Trim
                    lbl = grdPayDtl.Rows.Item(i).FindControl("lblBalanceAmt")
                    txt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
                    If note = String.Empty AndAlso Common.Val(lbl.Text) > Common.Val(txt.Text) Then
                        MessageBox("Please enter reason in note field for each line if payment amount is less than balance.", "Error", mDB)
                        Exit Sub
                    End If
                End If
            Next
            'Check if payment amount is more than remainding balance (cannot combine with above in order to show *)
            For i As Integer = 0 To grdPayDtl.Rows.Count - 1
                chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
                If chk.Checked = True Then
                    lbl = grdPayDtl.Rows.Item(i).FindControl("lblBalanceAmt")
                    txt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
                    If Common.Val(lbl.Text) < Common.Val(txt.Text) Then
                        MessageBox("Payment amount cannot be greater than remainding balance.", "Error", mDB)
                        Exit Sub
                    End If
                End If
            Next
        End If
        If Common.Val(txtAdvancePayAmt.Text) < 0 Then
            MessageBox("Advance Payment amount cannot be negative", "Error", mDB)
            Exit Sub
        End If
        If payAmt < 0 Then
            MessageBox("Payment amount cannot be negative", "Error", mDB)
            Exit Sub
        End If
        If payAmt = 0 Then
            MessageBox("Please check at least one invoice that you would like to pay.", "Error", mDB)
            Exit Sub
        End If
        If chkInputPayAmt.Checked = True AndAlso Common.Val(txtInputPayAmt.Text) <> payAmt Then
            MessageBox("If you would like to pay different amount, please use 'Apply' button to apply the amount you entered to the payment amount.", "Error", mDB)
            Exit Sub
        End If
        If lstPaymentAcc.Text = LST_CMD_STR Then
            MessageBox("Please select " & IIf(ALLOW_CARD_PAYMENT = True, "an account", "a bank account") & " or use another payment account", "Error", mDB)
            Exit Sub
        End If
        GetJFCBranchAndCustomerNr(mDB, customerID, jfcBranchNr, Nothing)
        'If jfcBranchNr = "10" OrElse jfcBranchNr = "30" OrElse jfcBranchNr = "98" Then
        '    maxTransAmt = PAYMENT_MAX_TRANSACTION_AMT
        'Else
        '    maxTransAmt = PAYMENT_MAX_TRANSACTION_AMT_TMP
        'End If
        If payAmt > PAYMENT_MAX_TRANSACTION_AMT Then
            MessageBox("Payment amount cannot exceed $" & PAYMENT_MAX_TRANSACTION_AMT & ". If you wish to pay more than $" & PAYMENT_MAX_TRANSACTION_AMT & ", please divide it to multiple payments.", "Error", mDB)
            Exit Sub
        End If
        SQL = "SELECT payment_id FROM customer_payhdr WHERE customer_id = :1 AND createUser = :2 AND createDate > SYSDATE - 5 / (24 * 60) AND paymentAmt = " & payAmt
        If CString(mDB.GetScalar(SQL, customerID, userEmail)) <> String.Empty Then
            MessageBox("It appears that you have made a payment with the same payment amount within 5 minutes. This payment cannot be processed as it might be duplicate payment.", "Error", mDB)
            Exit Sub
        End If

        'Get a new payment ID
        paymentID = mDB.GetSequence("PAYMENT_ID", PAYMENT_ID_LEN)
        'Get payment account
        SQL = "SELECT * FROM customer_payAcc WHERE customer_id = :1 AND accNickName = :2"
        r = mDB.GetReader(SQL, customerID, lstPaymentAcc.Text)
        If r.Read Then
            If CString(r("paymentType_id")) = "CARD" Then
                'Check expiration date
                DateTime.TryParseExact(CString(r("expMonth")) & "/01/" & CString(r("expYear")), DATE_FMT, Nothing, Nothing, dtExp)
                dtFirstDayOfMonth = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                If dtExp < dtFirstDayOfMonth Then
                    MessageBox("The card you selected has been expired. Please use another account.", "Error", mDB)
                    r.close()
                    Exit Sub
                End If
                'Credit or debit card payment 'Replaced PAYMENT_MERCHANT_ID, PAYMENT_PASSWORD
                paramAry = {"pg_merchant_id=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_MERCHANT_ID", lblJFCBranchNr.Text), PAYMENT_SANDBOX_MERCHANT_ID), _
                            "pg_password=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_PASSWORD", lblJFCBranchNr.Text), PAYMENT_SANDBOX_PASSWORD), _
                            "pg_transaction_type=" & PAYMENT_TRANSACTION_TYPE_CARD, _
                            "pg_total_amount=" & payAmt, _
                            "Ecom_billto_postal_name_first=" & Left(customerName, 25), _
                            "Pg_billto_postal_name_company=" & Left(customerName, 20), _
                            "Ecom_consumerorderid=" & paymentID,
                            "Ecom_Payment_Card_Type=" & CString(r("cardType_id")), _
                            "Ecom_Payment_Card_Name=" & CString(r("accName")), _
                            "Ecom_Payment_Card_Number=" & Decrypt256(CString(r("cardNr"))), _
                            "Ecom_Payment_Card_ExpDate_Month=" & CString(r("expMonth")), _
                            "Ecom_Payment_Card_ExpDate_Year=" & CString(r("expYear")), _
                            "Ecom_Payment_Card_Verification=" & Decrypt256(CString(r("securityCode"))), _
                            "endofdata"}
            Else
                'Savings or checking account payement
                paramAry = {"pg_merchant_id=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_MERCHANT_ID", lblJFCBranchNr.Text), PAYMENT_SANDBOX_MERCHANT_ID), _
                            "pg_password=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_PASSWORD", lblJFCBranchNr.Text), PAYMENT_SANDBOX_PASSWORD), _
                            "pg_transaction_type=" & PAYMENT_TRANSACTION_TYPE_BANKACC, _
                            "pg_total_amount=" & payAmt, _
                            "Ecom_billto_postal_name_first=" & Left(customerName, 25), _
                            "Pg_billto_postal_name_company=" & Left(customerName, 20), _
                            "Ecom_consumerorderid=" & paymentID,
                            "Ecom_Payment_Check_Account_Type=" & CString(r("paymentType_id")), _
                            "Ecom_Payment_Check_Account=" & Decrypt256(CString(r("bankAccNr"))), _
                            "Ecom_Payment_Check_TRN=" & Decrypt256(CString(r("routingNr"))), _
                            "Pg_Entry_Class_Code=WEB", _
                            "endofdata"}

            End If
        End If
        r.close()

        If paramAry Is Nothing Then
            MessageBox("Cannot find your payment account", "Error", mDB)
            Exit Sub
        End If

        'Init
        lblResults.Text = String.Empty

        'Create payment
        mDB.BeginTransaction()

        InsertPayHdr(mDB, paymentID, customerID, lstPaymentAcc.Text, Common.Val(txtAdvancePayAmt.Text), payAmt, txtMessage.Text, False)
        For i As Integer = 0 To grdPayDtl.Rows.Count - 1
            chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
            If chk.Checked = True Then
                txt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
                lbl = grdPayDtl.Rows.Item(i).FindControl("lblBalanceAmt")
                balanceAmt = Common.Val(lbl.Text)
                paymentLineAmt = IIf(AllowCustInputPayAmt(mDB, customerID) = True, Common.Val(txt.Text), balanceAmt)
                If paymentLineAmt <> 0 Then
                    lbl = grdPayDtl.Rows.Item(i).FindControl("lblArNumber")
                    Dim arNumber As String = lbl.Text
                    SQL = "SELECT a.*, TO_CHAR(a.arDate, :1) AS arDate_fmt FROM customer_jfcar a WHERE customer_id = :2 AND arNumber = :3"
                    r = mDB.GetReader(SQL, DATE_FMT, customerID, arNumber)
                    If r.Read() Then
                        txt = grdPayDtl.Rows.Item(i).FindControl("txtNote")
                        note = txt.Text
                        lbl = grdPayDtl.Rows.Item(i).FindControl("lblPendingAmt")
                        pendingAmt = Common.Val(lbl.Text)
                        lbl = grdPayDtl.Rows.Item(i).FindControl("lblWebDiscAmt_View")
                        webDiscAmt = IIf(paymentLineAmt >= balanceAmt, Common.Val(lbl.Text), 0)
                        DBUpdate.InsertPayDtl(mDB, paymentID, CString(r("arNumber")), paymentLineAmt, note, CString(r("arDate_fmt")), CString(r("arType")), CString(r("sectionCode")), Val(r("arAmt")), Val(r("recvAmt")), Val(r("discAmt")), pendingAmt, webDiscAmt, CString(r("reference")), False)
                    End If
                    r.close()
                End If
            End If
        Next

        mDB.CommitTransaction()

        'Process payment
        Try
            Dim achMessage As New System.Text.StringBuilder
            For x As Integer = 0 To paramAry.Length - 1
                achMessage.AppendLine(paramAry(x))
            Next

            'Connect TCP Client
            myTCPClient.Connect(PAYMENT_SSL_SERVER, IIf(isProduction, PAYMENT_SSL_PORT, PAYMENT_SANDBOX_SSL_PORT))

            'Create SSL Stream
            mySSLStream = New SslStream(myTCPClient.GetStream, False, New RemoteCertificateValidationCallback(AddressOf SSLCertificateCallbackValidation))

            'mySSLStream.AuthenticateAsClient(PAYMENT_SSL_SERVER) 'Used for TLS 1.0
            'mySSLStream.AuthenticateAsClient(PAYMENT_SSL_SERVER, Nothing, System.Security.Authentication.SslProtocols.Tls12, True) 'Not supported in .NET 4.0
            mySSLStream.AuthenticateAsClient(PAYMENT_SSL_SERVER, Nothing, DirectCast(3072, SecurityProtocolType), False) 'Used for TLS 1.2, 3072 = System.Security.Authentication.SslProtocols.Tls12


            Dim sendBuffer() As Byte = System.Text.Encoding.ASCII.GetBytes(achMessage.ToString)
            Dim receiveBuffer(1023) As Byte

            mySSLStream.Write(sendBuffer)
            mySSLStream.Flush()
            mySSLStream.Read(receiveBuffer, 0, 1024)

            lblResults.Text += System.Text.Encoding.ASCII.GetChars(receiveBuffer)

        Catch ex As Exception
            lblResults.Text += Environment.NewLine & ex.ToString & Environment.NewLine & SSL_Certificate_Validation_Errors
        Finally
            If mySSLStream IsNot Nothing Then
                mySSLStream.Close()
            End If
        End Try

        'Check payment result
        responseCode = GetFortePaymentRtnCode(lblResults.Text, "PG_RESPONSE_CODE")
        traceNr = GetFortePaymentRtnCode(lblResults.Text, "PG_TRACE_NUMBER")
        authorizationCode = GetFortePaymentRtnCode(lblResults.Text, "PG_AUTHORIZATION_CODE")
        If Left(responseCode, 1) <> "A" Then
            'UpdatePaymentStatus(mDB, paymentID, "D")
            DeletePayment(mDB, paymentID) 'Delete payment as don't want to show in payment history
            responseDesc = GetFortePaymentRtnCode(lblResults.Text, "PG_RESPONSE_DESCRIPTION")
            MessageBox("The transaction was declined. The error message from the payment processor was '" & responseCode & ": " & responseDesc & "'. Please check your payment account or use another account.", "Error", mDB)
            Exit Sub
        End If

        ''--------------------------------------------------------------------------------------------------------------------------------------------------------
        ''Check the status of the transaction
        'Try
        '    Using proxy As New TransactionServiceClient()
        '        Dim ticket As TransactionService.Authentication = Authenticate.GetTransactionAuthTicket(IIf(isProduction, PAYMENT_API_LOGIN_ID, PAYMENT_SANDBOX_API_LOGIN_ID), IIf(isProduction, PAYEMNT_SECURE_TRANSACTION_KEY, PAYEMNT_SANDBOX_SECURE_TRANSACTION_KEY))
        '        Dim tran As Transaction = proxy.getTransaction(ticket, Convert.ToInt32(IIf(isProduction, PAYMENT_MERCHANT_ID, PAYMENT_SANDBOX_MERCHANT_ID)), traceNr)
        '        Dim status As String = tran.Response.Status 'If status = 'Settled', item has been funded to merchant’s bank account. 
        '    End Using
        'Catch ex As Exception
        '    lblResults.Text += Environment.NewLine & ex.ToString & Environment.NewLine
        '    Exit Sub
        'End Try
        ''--------------------------------------------------------------------------------------------------------------------------------------------------------

        'Payment confirmed
        transactionType = GetFortePaymentRtnCode(lblResults.Text, "PG_TRANSACTION_TYPE")
        approvedAmt = Val(GetFortePaymentRtnCode(lblResults.Text, "PG_TOTAL_AMOUNT"))
        mDB.BeginTransaction()
        UpdatePayHdr(mDB, paymentID, traceNr, authorizationCode, transactionType, approvedAmt, False)
        UpdatePaymentStatus(mDB, paymentID, "P", False)
        DeleteTempPaymentAcc(mDB, False) 'Delete temp account
        mDB.CommitTransaction()

        'Show result
        ClearCtrl({lblErrorMsg, lblConfirmMsg})
        If payAmt > approvedAmt Then
            lblErrorMsg.Text = "Thank you for your payment. Your payment of " & String.Format("{0:$#,#.00}", approvedAmt) & " has been approved and " & String.Format("{0:$#,#.00}", payAmt - approvedAmt) & " has been declined."
        Else
            lblConfirmMsg.Text = "Thank you for your payment of " & String.Format("{0:$#,#.00}", approvedAmt) & "."
        End If
        msg = " Your payment ID is " & paymentID & ". Please note that it may take 1 to 2 business days to process your payment. " 'In case you need to void this payment, please go to 'Web Payment History' tab."
        If lblErrorMsg.Text <> String.Empty Then lblErrorMsg.Text &= msg Else lblConfirmMsg.Text &= msg

        'Get email body for user from email template in user control
        vendorEmail = GetJfcEmail(mDB, customerID, False)
        CreateEmailBody(Page, emailBody, "PAY", paymentID, IIf(lblErrorMsg.Text <> String.Empty, lblErrorMsg.Text, lblConfirmMsg.Text), "CUSTOMER")
        'Send confirmation email to user
        If Common.SendMail(SYSTEM_EMAIL, userEmail, vendorEmail, "Payment Confirmation (" & paymentID & ")", emailBody, Nothing, True, errMsg) > 0 Then
            'Email sent failed
            emailSent = False
        Else
            'Email sent OK
            emailSent = True
            msg = "<br /><br />Confirmation email has been sent to you."
            If lblErrorMsg.Text <> String.Empty Then lblErrorMsg.Text &= msg Else lblConfirmMsg.Text &= msg
        End If

        'Send email to vendor
        CreateEmailBody(Page, emailBody, "PAY", paymentID, "The following payment has been submitted by your customer.", "JFC")
        Common.SendMail(SYSTEM_EMAIL, vendorEmail, userEmail, "Payment '" & paymentID & "' has been submitted", emailBody, Nothing, True, errMsg)

        'Return back to transaction list and refresh payment history
        InsertEventLog(mDB, "Payment approved" & IIf(payAmt > approvedAmt, " (partial)", "") & IIf(emailSent = True, " and confirmation email sent.", ""), customerID, lstPaymentAcc.Text, approvedAmt, True)
        showPaymentHistory()
        showBalance(True)
        'Common.InitLastUpdate()
    End Sub

    ''' <summary>
    ''' Validates the SSL Certificate on the Server
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SSLCertificateCallbackValidation( _
      ByVal Sender As Object, ByVal Certificate As X509Certificate, _
      ByVal Chain As X509Chain, ByVal SSLPolicyErrors As SslPolicyErrors) As Boolean

        Try

            If SSLPolicyErrors <> Security.SslPolicyErrors.None Then
                SSL_Certificate_Validation_Errors = SSLPolicyErrors.ToString
                Return False
            Else
                SSL_Certificate_Validation_Errors = Nothing
                Return True
            End If

        Catch ex As Exception
            lblResults.Text += Environment.NewLine & ex.ToString & Environment.NewLine
            Return False
        End Try

    End Function

    Protected Sub radAutoPayType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles radAutoPayType.SelectedIndexChanged
        pnlAutoPayMoreInput.Visible = IIf(radAutoPayType.Text = String.Empty, False, True)
    End Sub

    Protected Sub btnSaveAutoPay_Click(sender As Object, e As System.EventArgs) Handles btnSaveAutoPay.Click
        'Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)

        ''Error check
        'If radAutoPayType.Text <> String.Empty Then
        '    If lstAutoPayAcc.Text = LST_CMD_STR Then
        '        MessageBox("Please select " & IIf(ALLOW_CARD_PAYMENT = True, "an account", "a bank account") & " or use another payment account.", "Error", mDB)
        '        Exit Sub
        '    End If
        '    If chkAgreeAutoPay.Checked = False Then
        '        MessageBox("Please agree to the Terms and Conditions.", "Error", mDB)
        '        Exit Sub
        '    End If
        'End If

        ''Empty payment account if no auto payment is selected
        'If radAutoPayType.Text = String.Empty Then
        '    lstAutoPayAcc.Text = LST_CMD_STR
        'End If

        'mDB.BeginTransaction()

        ''Update customer auto payment type
        'DBUpdate.UpdateCustAutoPayType(mDB, customerID, radAutoPayType.Text, False)
        ''Update bank account for auto payment
        'DBUpdate.UpdateCustAutoPayAcc(mDB, customerID, lstAutoPayAcc.Text, False)

        'mDB.CommitTransaction()

        ''Return back to transaction list
        'InsertEventLog(mDB, "Updated auto payment", customerID, radAutoPayType.Text, lstAutoPayAcc.Text, True)
        'MessageLine("CONFIRM", "Successfully updated")
        'showBalance()
    End Sub

    'Private Function enableAutoPayBtn(ByVal customerID As String) As Boolean
    '    Return IIf(GetCustAllowAutoPayType(mDB, customerID) = String.Empty AndAlso GetCustAutoPayType(mDB, customerID) = String.Empty, False, True)
    'End Function

    Private Function getWhereClause_Balance(ByVal customerID As String, ByVal addMonths As Integer) As String
        Dim current As New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        Dim startDate, endDate As DateTime
        Dim transDateClause As String = Nothing

        If addMonths > mOldestIndex Then
            startDate = current.AddMonths(addMonths)
            endDate = New DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month))
            transDateClause = String.Format("o.transdate BETWEEN TO_DATE('{0}', '{2}') AND TO_DATE('{1}', '{2}')", _
                startDate.ToString(Common.DATE_FMT), endDate.ToString(Common.DATE_FMT), Common.DATE_FMT)
        Else
            startDate = current.AddMonths(mOldestIndex + 1)
            transDateClause = String.Format("o.transdate < TO_DATE('{0}', '{1}')", startDate.ToString(Common.DATE_FMT), Common.DATE_FMT)
        End If

        Return String.Format("WHERE o.customer_id = {0} AND {1}", mDB.SqlEsc(customerID), transDateClause)
        'Return String.Format("WHERE UPPER(o.transtype) <> 'BALANCE' AND o.customer_id = {0} AND {1}", DB.SqlEsc(customerID), transDateClause)
    End Function

    Private Sub calculateTotalPayAmt() 'ByRef errMsg As String
        Dim chk As CheckBox
        Dim txt As TextBox
        Dim lbl As Label
        Dim totalAmt As Decimal = 0
        Dim balanceAmt As Decimal
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim allowCustInputPayAmt As Boolean = Common.AllowCustInputPayAmt(mDB, customerID)

        For i As Integer = 0 To grdPayDtl.Rows.Count - 1
            chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
            If chk.Checked = True Then
                lbl = grdPayDtl.Rows.Item(i).FindControl("lblBalanceAmt")
                balanceAmt = Common.Val(lbl.Text)
                txt = grdPayDtl.Rows.Item(i).FindControl("txtPayAmt")
                If allowCustInputPayAmt = True Then
                    ''Show or hide * for note
                    'lbl = grdPayDtl.Rows.Item(i).FindControl("lblStar_note")
                    'lbl.Text = IIf(balanceAmt > Common.Val(txt.Text), "*", "")
                    'lbl = grdPayDtl.Rows.Item(i).FindControl("lblNoWebDisc")
                    'lbl.Text = IIf(balanceAmt > Common.Val(txt.Text) AndAlso GetCustWebDiscType(mDB, customerID) <> String.Empty, "No Discount", "")
                    If txt.Visible = True AndAlso (IsNumber(txt.Text, True, True) = False) Then
                        txt.Text = ""
                        Continue For
                        'errMsg = "Payment amount has to be a number"
                        'Return False
                    End If
                End If
                totalAmt += IIf(allowCustInputPayAmt = True, Common.Val(txt.Text), balanceAmt)
            End If
        Next
        totalAmt += Common.Val(txtAdvancePayAmt.Text)
        lblPayAmt.Text = String.Format("{0:$#,#.00}", totalAmt)
        'Return True
    End Sub

    Private Sub showBalance(Optional ByVal showPnlAftPayment As Boolean = False)
        Dim SQL As String
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)
        Dim balance As Decimal
        Dim pendingAmt As Decimal
        Dim disAllowPay As Boolean = ApplyInEmailOrCustomer(mDB, customerID, "DisAllowPay")

        'Show transaction list
        pnlList_balance.Visible = True
        pnlNewPayment_balance.Visible = False
        'pnlPaymentDtl_balance.Visible = False
        pnlAutoPay_balance.Visible = False

        'Set properties
        If IsMobile() = True Then
            grdPending.Columns(GetColumnIndex(grdPending, "transType")).Visible = False
            grdBalance.Columns(2).Visible = False 'Reference
        Else
            grdPending.Width = 1000
            grdBalance.Width = 1000
        End If
        pnlAftPayment.Visible = showPnlAftPayment 'Show / hide payment made message
        'InvisibleCtrl({lblCurBalanceTitle, lblCurBalance2, lblEndBalanceTitle, lblEndBalance})
        'If lstPeriod_balance.SelectedValue = 0 Then
        '    VisibleCtrl({lblCurBalanceTitle, lblCurBalance2})
        'Else
        '    VisibleCtrl({lblEndBalanceTitle, lblEndBalance})
        'End If
        btnPay.Visible = IIf(showPnlAftPayment = False AndAlso disAllowPay = False, True, False)
        btnPay.Text = IIf(Session("BALANCE_CMD") = "CUST", "View & Pay Balance", "Details")
        btnAutoPay.Visible = True 'IIf(GetCustAllowAutoPayType(mDB, customerID) <> String.Empty, True, False)
        If btnAutoPay.Visible = True Then
            btnAutoPay.Text = "Auto Payment" 'IIf(Session("BALANCE_CMD") = "CUST", IIf(GetCustAutoPayType(mDB, customerID) = String.Empty, "Set Auto Payment", "Update Auto Payment"), "Auto Payment")
        End If

        'Show pending transaction
        SQL = "SELECT p.*, 'Pending Web Payment' AS transType FROM customer_payHdr p WHERE p.paymentStatus_id IN ('P', 'F') AND p.customer_id = " & mDB.SqlEsc(customerID)
        dsPending.ProviderName = "System.Data.OracleClient"
        dsPending.ConnectionString = mDB.GetConnectionString
        dsPending.SelectCommand = SQL
        'dsPending.SelectParameters.Add("pCustomerID", System.Data.DbType.String, customerID)
        grdPending.DataSourceID = "dsPending"
        grdPending.Sort("createDate", SortDirection.Descending)
        grdPending.DataBind()
        pnlPendingTrans.Visible = IIf(grdPending.Rows.Count > 0, True, False)

        'Show beginning balance
        'balance = Val(mDB.GetScalar("SELECT balance FROM customer_JfcTransaction o " & _
        '               getWhereClause_Balance(customerID, CType(lstPeriod_balance.SelectedValue, Integer)) & _
        '               " AND UPPER(o.transtype) = 'BALANCE'"))
        'lblBegBalance.Text = String.Format("{0:$#,#.00}", balance) '"$" & balance.ToString("###,###.00")
        'Show current balance
        balance = GetCurrentBalance(mDB, customerID)
        pendingAmt = GetPendingAmt(mDB, customerID)
        lblCurBalance.Text = String.Format("{0:$#,#.00}", balance) '"$" & balance.ToString("###,###.00")
        'lblCurBalance2.Text = lblCurBalance.Text
        lblCurPendingAmt.Text = IIf(pendingAmt > 0, IIf(IsMobile(), "<br />", "") & " (Pending Payment: " & String.Format("{0:$#,#.00}", pendingAmt) & ")", "")
        'Show ending balance
        'balance = Val(mDB.GetScalar(String.Format("SELECT balance FROM (select balance from customer_JfcTransaction o " & _
        '                 getWhereClause_Balance(customerID, CType(lstPeriod_balance.SelectedValue, Integer)) & " order by seq desc) where rownum = 1")))
        'lblEndBalance.Text = String.Format("{0:$#,#.00}", balance) '"$" & balance.ToString("###,###.00")

        'Show transaction
        SQL = String.Format("SELECT o.transdate, o.reference, o.transType, o.amt, o.balance, o.seq, d.fileName, d.directory, " & _
                                " CASE WHEN ADD_MONTHS(o.transdate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & " THEN 'Y' ELSE 'N' END AS allowCustDownloadInvoice " & _
                                "FROM customer_JfcTransaction o " & _
                                "LEFT OUTER JOIN customer_document d ON o.reference = d.docName AND UPPER(d.docType) = 'INVOICE' AND o.customer_id = d.customer_id " & _
                                "{0}", _
                                getWhereClause_Balance(customerID, CType(lstPeriod_balance.SelectedValue, Integer))) & _
                                " AND UPPER(o.transtype) <> 'BALANCE'"
        '" CASE WHEN ADD_MONTHS(o.transdate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & " THEN 'Y' ELSE 'N' END AS allowCustDownloadInvoice " & _
        dsBalance.ProviderName = "System.Data.OracleClient"
        dsBalance.ConnectionString = mDB.GetConnectionString
        dsBalance.SelectCommand = SQL
        grdBalance.DataSourceID = "dsBalance"
        grdBalance.Sort(lstOrderBy_balance.SelectedValue, SortDirection.Ascending)
        grdBalance.DataBind()
    End Sub

    'Show payment detail as balance detail
    'Private Sub showPaymentDtl_balance(ByVal paymentID As String)
    '    'Show detail of the peyment
    '    pnlList_balance.Visible = False
    '    pnlNewPayment_balance.Visible = False
    '    pnlPaymentDtl_balance.Visible = True
    '    pnlAutoPay_balance.Visible = False

    '    'Pass parameters to paymentDtl
    '    ctrPaymentDtl_balance.pageLoad(paymentID)
    'End Sub

    'Show new payment as balance detail
    Private Sub showPayment(ByVal customerID As String)
        Dim SQL As String
        Dim balance As Decimal
        Dim pendingAmt As Decimal
        Dim SQL_pendingAmt, SQL_webDiscAmt As String
        Dim errMsg As String = ""
        Dim allowCustInputPayAmt As String = Common.AllowCustInputPayAmt(mDB, customerID)
        Dim chk As CheckBox
        Dim webDiscType As String = GetCustWebDiscType(mDB, customerID)
        Dim userEmail As String = GetUserEmail(mDB)

        'Show payment
        pnlList_balance.Visible = False
        pnlNewPayment_balance.Visible = True
        'pnlPaymentDtl_balance.Visible = False
        pnlAutoPay_balance.Visible = False
        pnlPayHdr.Visible = IIf(Session("BALANCE_CMD") = "CUST", True, False)

        'Set properties
        'pnlAddPaymentAcc.Visible = False
        'imgCheck.Visible = False
        lnkEditPaymentAcc.PostBackUrl = "~/" & IIf(Session("BALANCE_CMD") = "CUST", "CUSTOMER", Session("BALANCE_CMD")) & "/MyAccount.aspx?tab=1"
        If Session("BALANCE_CMD") = "CUST" Then
            grdPayDtl.Columns(11).Visible = allowCustInputPayAmt 'Payment amount
            grdPayDtl.Columns(12).Visible = allowCustInputPayAmt 'Note
        Else
            grdPayDtl.Columns(0).Visible = False 'Check
            grdPayDtl.Columns(11).Visible = False 'Payment amount
            grdPayDtl.Columns(12).Visible = False 'Note
        End If
        If webDiscType = String.Empty OrElse webDiscType = "Z" Then
            'Cannot make the column invisible because of javascript:calculateTotalPayAmt()
            'grdPayDtl.Columns(8).HeaderText = String.Empty 'Properties for Web Discount header is set in grdPayDtl_RowDataBound()
            grdPayDtl.Columns(9).HeaderStyle.CssClass = "" : grdPayDtl.Columns(9).ItemStyle.CssClass = "" 'Web Discount
            grdPayDtl.Columns(14).Visible = False 'No web discount warning
        Else
            'grdPayDtl.Columns(8).HeaderText = "Web Early Payment Discount" 'Properties for Web Discount header is set in grdPayDtl_RowDataBound()
            grdPayDtl.Columns(9).HeaderStyle.CssClass = "th number20" : grdPayDtl.Columns(9).ItemStyle.CssClass = "number20" 'Web Discount
            grdPayDtl.Columns(14).Visible = True 'No web discount warning
        End If
        If IsMobile() = True Then
            'grdPayDtl.Columns(GetColumnIndex(grdPayDtl, "isLiquor")).Visible = False
            grdPayDtl.Columns(4).Visible = False 'Reference
            grdPayDtl.Columns(7).Visible = False 'Paid Amount (Confirmed)
            grdPayDtl.Columns(8).Visible = False 'Paid Amount (Pending)
            chkInputPayAmt.Visible = False
            lblInputPayAmt.Visible = False
        Else
            grdPayDtl.Width = 1056
            pnlPayDtl.Width = 1034
            pnlPayHdr.Width = 1034
        End If

        'Show payment header
        lblCustomerName.Text = GetCustName(mDB, customerID)
        GetJFCBranchAndCustomerNr(mDB, customerID, lblJFCBranchNr.Text, lblJFCCustomerNr.Text)
        balance = GetCurrentBalance(mDB, customerID)
        pendingAmt = GetPendingAmt(mDB, customerID)
        lblBalance.Text = String.Format("{0:$#,#.00}", balance)
        lblPendingAmt.Text = IIf(pendingAmt > 0, " (Pending Payment: " & String.Format("{0:$#,#.00}", pendingAmt) & ")", "")
        'lblPayAmt.Text = IIf(balance - pendingAmt > 0, balance - pendingAmt, 0)
        'Show bank account list
        ShowPaymentAccList(mDB, customerID, lstPaymentAcc)
        hidAllowCustInputPayAmt.Value = IIf(allowCustInputPayAmt = True, "Y", "N") 'Used for javascript:calculateTotalPayAmt()
        hidIsMobile.Value = IIf(IsMobile() = True, "Y", "N") 'Used for javascript:calculateTotalPayAmt()
        lblReturnedCheckFee.Text = IIf(IsMobile(), "", "Please note that ") & GetReturnedCheckFee(mDB, lblJFCBranchNr.Text) & " will be charged for a returned check."
        lnkAutoPay.Visible = True 'IIf(GetCustAllowAutoPayType(mDB, customerID) <> String.Empty AndAlso GetCustAutoPayType(mDB, customerID) = String.Empty, True, False)
        If Session("SYS_SESSION_USERCOMPANYID") = "JFC" AndAlso SUPPORT_EMAIL.Contains(userEmail) = False AndAlso DEMO_EMAIL.Contains(userEmail) = False Then
            btnSubmitPayment.Visible = False
            lnkAutoPay.Visible = False
        End If

        'Show payment detail (change batch_fortePaymentSync program as well for any changes)
        SQL_pendingAmt = "SELECT NVL(SUM(pd.paymentLineAmt), 0) FROM customer_payhdr ph, customer_paydtl pd WHERE ph.payment_id = pd.payment_id AND ph.paymentStatus_id IN ('P', 'F') AND ph.customer_id = a.customer_id AND pd.arNumber = a.arNumber"
        SQL_webDiscAmt = "SELECT NVL(SUM(NVL(pd.webDiscAmt, 0)), 0) FROM customer_payhdr ph, customer_paydtl pd WHERE ph.payment_id = pd.payment_id AND ph.paymentStatus_id IN ('P', 'F') AND ph.customer_id = a.customer_id AND pd.arNumber = a.arNumber"
        SQL = "SELECT /*+rule*/ t.*, " & _
                "CASE WHEN NVL(webDiscAmt, 0) > 0 THEN webDiscAmt WHEN webDisc = 'Y' THEN ROUND(tmpbalance * NVL(discountRate, 0) / 100, 2) ELSE 0 END AS webDiscAmt_view, " & _
                "CASE WHEN NVL(webDiscAmt, 0) > 0 THEN tmpbalance WHEN webDisc = 'Y' THEN tmpbalance - ROUND(tmpbalance * NVL(discountRate, 0) / 100, 2) ELSE tmpbalance END AS balance " & _
                "FROM " & _
                "(SELECT a.*, (" & SQL_pendingAmt & ") AS pendingAmt, (" & SQL_webDiscAmt & ") AS webDiscAmt, " & _
                " TO_CHAR(arDate, " & mDB.SqlEsc(DATE_FMT) & ") arDate_view, TO_CHAR(arDate, " & mDB.SqlEsc(SHORTDATE_FMT) & ") arDate_mobileview, " & _
                " CASE WHEN a.arType = 'SERVICE CHARGE' AND a.sectionCode = '5' THEN 'Interest' WHEN a.sectionCode = '5' THEN 'Liquor' ELSE '' END isLiquor, " & _
                " SUBSTR(CASE WHEN a.arType = 'SERVICE CHARGE' AND a.sectionCode = '5' THEN 'LIQUOR INTEREST' ELSE a.arType END, 1 " & IIf(IsMobile(), ",3", "") & ") arType_view, " & _
                " CASE WHEN NVL(c.arDays, 0) = 0 OR a.arDate < (TRUNC(SYSDATE) - NVL(c.arDays, 0)) OR TRUNC(a.arDate, 'mm') < TRUNC(SYSDATE, 'mm') OR a.sectionCode = '5' THEN 'Y' ELSE 'N' END checkRow, " & _
                " TRUNC(SYSDATE) - TRUNC(a.arDate) AS aging, " & _
                " CASE WHEN c.webDiscType = 'A' AND NVL(a.sectionCode, ' ') <> '5' AND a.arDate >= (TRUNC(SYSDATE) - " & WebDiscDays_A & ") THEN 'Y' WHEN c.webDiscType = 'B' AND NVL(a.sectionCode, ' ') <> '5' AND a.arDate >= (TRUNC(SYSDATE) - " & WebDiscDays_B & ") THEN 'Y' ELSE 'N' END webDisc, " & _
                " c.webDiscType, CASE WHEN c.webDiscType = 'A' THEN " & WebdiscRate_A & " WHEN c.webDiscType = 'B' THEN " & WebdiscRate_B & " END AS discountRate, " & _
                " a.recvAmt + a.discAmt AS recvAndDiscAmt, a.arAmt - a.recvAmt - a.discAmt - (" & SQL_pendingAmt & ") - (" & SQL_webDiscAmt & ") AS tmpbalance, d.fileName, d.directory " & _
                "FROM customer c, customer_jfcAR a " & _
                "LEFT OUTER JOIN customer_document d ON a.reference = d.docName AND UPPER(d.docType) = 'INVOICE' AND a.customer_id = d.customer_id " & _
                "WHERE a.customer_id = c.customer_id AND a.customer_id = " & mDB.SqlEsc(customerID) & _
                IIf(chkSelectLiquor.Checked = True, " AND a.sectionCode = '5'", "") & _
                IIf(chkHideLiquorInterest.Checked = True, " AND NOT (a.arType = 'SERVICE CHARGE' AND a.sectionCode = '5')", "") & _
                ") t WHERE tmpbalance <> 0"
        '"CASE WHEN NVL(webDiscAmt, 0) > 0 THEN webDiscAmt WHEN webDisc = 'Y' AND tmpbalance > 0 THEN ROUND(tmpbalance * NVL(discountRate, 0) / 100, 2) ELSE 0 END AS webDiscAmt_view, " & _
        '"CASE WHEN NVL(webDiscAmt, 0) > 0 THEN tmpbalance WHEN webDisc = 'Y' AND tmpbalance > 0 THEN tmpbalance - ROUND(tmpbalance * NVL(discountRate, 0) / 100, 2) ELSE tmpbalance END AS balance " & _
        dsPayDtl.ProviderName = "System.Data.OracleClient"
        dsPayDtl.ConnectionString = mDB.GetConnectionString
        dsPayDtl.SelectCommand = SQL
        grdPayDtl.DataSourceID = "dsPayDtl"
        grdPayDtl.Sort("arDate, arNumber", SortDirection.Ascending)
        grdPayDtl.DataBind()

        'Set more properties
        For i As Integer = 0 To grdPayDtl.Rows.Count - 1
            chk = grdPayDtl.Rows.Item(i).FindControl("chkSelect")
            grdPayDtl.Rows.Item(i).CssClass = IIf(chk.Checked, "bold", "")
        Next
        'Show total payment amount
        If Session("BALANCE_CMD") = "CUST" Then CallJavaScript(Me.Page, "javascript:calculateTotalPayAmt();")
        'calculateTotalPayAmt()
    End Sub

    'Show auto payment
    Private Sub showAutoPay(ByVal customerID As String)
        'Dim autoPayAccNickName As String
        'Dim autoPayType As String
        Dim webDiscType As String = GetCustWebDiscType(mDB, customerID)
        'Dim allowAutoPayType As String
        Dim autoPayTypeDesc_none As String = "No auto payment"
        Dim autoPayTypeDesc_A As String = "Auto payment (all) - Automatically deduct the total amount within 24 hours after your invoice and credit memo has been created. " & IIf(webDiscType = "Z", WebDiscRate_Z & "% discount is applied for employees.", "")
        Dim autoPayTypeDesc_L As String = "Auto payment (liquor only) - Automatically deduct the total amount for liquor including liquor interest within 24 hours after your invoice has been created."
        Dim userEmail As String = GetUserEmail(mDB)

        'Show payment
        pnlList_balance.Visible = False
        pnlNewPayment_balance.Visible = False
        'pnlPaymentDtl_balance.Visible = False
        pnlAutoPay_balance.Visible = True
        'pnlPayHdr.Visible = IIf(Session("BALANCE_CMD") = "CUST", True, False)

        'Set properties
        pnlViewAutoPay.Visible = False
        pnlEditAutoPay.Visible = False
        chkAgreeAutoPay.Checked = False
        If Session("SYS_SESSION_USERCOMPANYID") = "JFC" AndAlso SUPPORT_EMAIL.Contains(userEmail) = False AndAlso DEMO_EMAIL.Contains(userEmail) = False Then
            btnSaveAutoPay.Visible = False
        End If

        'autoPayType = GetCustAutoPayType(mDB, customerID)
        Dim autoPayTypeALL = GetCustAutoPayType(mDB, customerID, "AUTOPAYTYPEID_ALL")
        Dim autoPayTypeNONLIQ = GetCustAutoPayType(mDB, customerID, "AUTOPAYTYPEID_NONLIQ")
        Dim autoPayTypeBEER = GetCustAutoPayType(mDB, customerID, "AUTOPAYTYPEID_BEER")
        Dim autoPayTypeWINE = GetCustAutoPayType(mDB, customerID, "AUTOPAYTYPEID_WINE")
        Dim autoPayTypeLIQUOR = GetCustAutoPayType(mDB, customerID, "AUTOPAYTYPEID_LIQUOR")

        If Session("BALANCE_CMD") = "CUST" Then
            'pnlEditAutoPay.Visible = True
            'lnkEditAutoPayAcc.PostBackUrl = "~/" & IIf(Session("BALANCE_CMD") = "CUST", "CUSTOMER", Session("BALANCE_CMD")) & "/MyAccount.aspx?tab=1"

            ''Show auto payment type
            'allowAutoPayType = GetCustAllowAutoPayType(mDB, customerID)
            'radAutoPayType.Items.Clear()
            'radAutoPayType.Items.Add(New ListItem(autoPayTypeDesc_none, ""))
            'If allowAutoPayType.Contains("A") Then radAutoPayType.Items.Add(New ListItem(autoPayTypeDesc_A, "A"))
            'If allowAutoPayType.Contains("L") Then radAutoPayType.Items.Add(New ListItem(autoPayTypeDesc_L, "L"))
            'If radAutoPayType.Items.FindByValue(autoPayType) IsNot Nothing Then radAutoPayType.Text = autoPayType Else radAutoPayType.Text = String.Empty
            'radAutoPayType_SelectedIndexChanged(Nothing, Nothing)

            ''Show bank account list
            'ShowPaymentAccList(mDB, customerID, lstAutoPayAcc)
            'autoPayAccNickName = GetCustAutoPayAccNickName(mDB, customerID)
            'If lstAutoPayAcc.Items.FindByValue(autoPayAccNickName) IsNot Nothing Then lstAutoPayAcc.Items.FindByValue(autoPayAccNickName).Selected = True
        Else
            pnlViewAutoPay.Visible = True
            lblAutoPayType.Text = String.Empty
            If autoPayTypeALL <> "" Then
                lblAutoPayType.Text &= String.Format("{0}: {1} <br />", "All Invoice", autoPayTypeALL)
            End If
            If autoPayTypeNONLIQ <> "" Then
                lblAutoPayType.Text &= String.Format("{0}: {1} <br />", "Non Liquor Invoice", autoPayTypeNONLIQ)
            End If
            If autoPayTypeBEER <> "" Then
                lblAutoPayType.Text &= String.Format("{0}: {1} <br />", "Beer Invoice", autoPayTypeBEER)
            End If
            If autoPayTypeWINE <> "" Then
                lblAutoPayType.Text &= String.Format("{0}: {1} <br />", "Wine Invoice", autoPayTypeWINE)
            End If
            If autoPayTypeLIQUOR <> "" Then
                lblAutoPayType.Text &= String.Format("{0}: {1} <br />", "Liquor Invoice", autoPayTypeLIQUOR)
            End If

            'Show auto payment type
            'Select autoPayType
            '    Case String.Empty
            '        lblAutoPayType.Text = autoPayTypeDesc_none
            '    Case "A"
            '        lblAutoPayType.Text = autoPayTypeDesc_A
            '    Case "L"
            '        lblAutoPayType.Text = autoPayTypeDesc_L
            'End Select
        End If

    End Sub



    '--------------------------------------------------------------------------------------------------------------------
    'Payment History
    '--------------------------------------------------------------------------------------------------------------------
    'Protected Sub lstDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstDocType.SelectedIndexChanged
    '    Session("BALANCE_TAB") = Balance_Tab.payment
    '    showPaymentHistory()
    'End Sub

    Protected Sub lstOrderBy_payment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_payment.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        showPaymentHistory()
    End Sub

    Protected Sub lstCustCompany_payment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany_payment.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        ShowCustGrpList(mDB, lstCustCompany_payment.Text, lstCustGrp_payment)
        lstCustGrp_payment_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustGrp_payment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustGrp_payment.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        ShowCustomerList(mDB, lstCustGrp_payment.Text, lstCustomer_payment)
        lstCustomer_payment_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustomer_payment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustomer_payment.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        showPaymentHistory()
    End Sub

    Protected Sub btnSearch_payment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch_payment.Click
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        showPaymentHistory()
    End Sub

    Protected Sub grdPayment_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPayment.RowCommand
        Dim paymentID As String

        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        If e.CommandName = "Page" Then
            'Page is changed
            showPaymentHistory()
            'Don't remember scroll position
            Page.MaintainScrollPositionOnPostBack = False
        ElseIf e.CommandName = "VIEW_ROW" Then
            'Show order detail
            paymentID = Convert.ToString(e.CommandArgument)
            showPaymentDtl(paymentID)
        End If
    End Sub

    Protected Sub lnkBack_payment_Click(sender As Object, e As System.EventArgs) Handles lnkBack_payment.Click
        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        'Return back to payment history
        pnlList_payment.Visible = True
        pnlDtl_payment.Visible = False
    End Sub

    Private Sub showPaymentHistory()
        Dim SQL As String
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_payment.Text)

        'Show payment history
        pnlList_payment.Visible = True
        pnlDtl_payment.Visible = False

        'Set properties
        If IsMobile() = True Then
            grdPayment.Columns(GetColumnIndex(grdPayment, "result")).Visible = False
            grdPayment.Columns(GetColumnIndex(grdPayment, "createUser")).Visible = False
        End If

        'Show document
        SQL = "SELECT p.*, code2.codeDesc AS status " & _
                " , CASE WHEN paymentStatus_id IN ('E', 'B') THEN code.codeDesc ELSE '' END AS result " & _
                "FROM customer_payHdr p " & _
                "LEFT OUTER JOIN codemaster code ON code.codegrp = 'FORTE_SETTLERESULT' AND p.forteSettleResultCode = code.code_id " & _
                "LEFT OUTER JOIN codemaster code2 ON code2.codegrp = 'PAYMENTSTATUS' AND p.paymentStatus_id = code2.code_id " & _
                "WHERE paymentStatus_id <> 'N' AND customer_id = " & mDB.SqlEsc(customerID) & _
                IIf(txtPaymentID_payment.Text <> "", " AND p.payment_id = " & mDB.SqlEsc(addPrefixZero(txtPaymentID_payment.Text, PAYMENT_ID_LEN)), "")
        'IIf(lstDocType.Text = LST_ALL_STR, "", " AND UPPER(d.docType) = " & mDB.SqlEsc(lstDocType.Text.ToUpper))
        dsPayment.ProviderName = "System.Data.OracleClient"
        dsPayment.ConnectionString = mDB.GetConnectionString
        dsPayment.SelectCommand = SQL
        'dsPayment.SelectParameters.Add("pCustomerID", System.Data.DbType.String, customerID)
        grdPayment.DataSourceID = "dsPayment"
        grdPayment.Sort(lstOrderBy_payment.SelectedValue, Nothing)
        grdPayment.DataBind()
    End Sub

    Private Sub showPaymentDtl(ByVal paymentID As String)
        'Show detail of the payment
        pnlList_payment.Visible = False
        pnlDtl_payment.Visible = True

        'Pass parameters to paymentDtl
        ctrPaymentDtl_payment.pageLoad(paymentID)
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Download document
    '--------------------------------------------------------------------------------------------------------------------
    Protected Sub lstDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstDocType.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        showDownload()
    End Sub

    Protected Sub lstOrderBy_download_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_download.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        showDownload()
    End Sub

    Protected Sub lstCustCompany_download_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustCompany_download.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        ShowCustGrpList(mDB, lstCustCompany_download.Text, lstCustGrp_download)
        lstCustGrp_download_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustGrp_download_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustGrp_download.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        ShowCustomerList(mDB, lstCustGrp_download.Text, lstCustomer_download)
        lstCustomer_download_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub lstCustomer_download_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstCustomer_download.SelectedIndexChanged
        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        showDownload()
    End Sub

    Protected Sub grdDownload_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdDownload.RowCommand
        'Dim row As GridViewRow = Nothing
        Dim docName As String
        Dim customerID As String
        Dim SQL As String
        Dim r As DataReader
        Dim rtn As Boolean = False

        Session("BALANCE_TAB") = Balance_Tab.DOWNLOAD
        If e.CommandName = "Page" Then
            'Page is changed
            showDownload()
            'Don't remember scroll position
            Page.MaintainScrollPositionOnPostBack = False
        ElseIf e.CommandName = "DOWNLOAD" Then
            'Download reference
            'row = grdDownload.Rows(CType(e.CommandArgument, Integer))
            'docName = CType(row.Cells(2).Controls(0), LinkButton).Text
            docName = Convert.ToString(e.CommandArgument)
            customerID = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_download.Text)
            SQL = "SELECT directory, fileName FROM customer_document WHERE customer_id = :1 AND docName = :2"
            r = mDB.GetReader(SQL, customerID, docName)
            If r.Read Then
                rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
            End If
            If rtn = False Then MessageBox("Cannot open the document", "Error", mDB)
        End If
    End Sub

    Private Sub showDownload()
        Dim SQL As String
        Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_download.Text)

        'Show document
        SQL = "SELECT d.createDate, UPPER(d.docType) docType, d.docName, d.fileName, d.directory, v.vendorName FROM customer_document d, custGrp_vendor v " & _
                "WHERE d.custGrpVendor_id = v.custGrpVendor_id AND d.customer_id = " & mDB.SqlEsc(customerID) & IIf(lstDocType.Text = LST_ALL_STR, "", " AND UPPER(d.docType) = " & mDB.SqlEsc(lstDocType.Text.ToUpper)) & _
                " AND ADD_MONTHS(d.createDate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)")
        '" AND ((ADD_MONTHS(d.createDate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & ") OR " & IIf(Session("BALANCE_CMD") = "JFCADMIN", "(1 = 1)", "(1 = 2)") & ") "
        dsDownload.ProviderName = "System.Data.OracleClient"
        dsDownload.ConnectionString = mDB.GetConnectionString
        dsDownload.SelectCommand = SQL
        'dsDownload.SelectParameters.Add(New Parameter("pCustomerID", System.Data.DbType.String, customerID))
        'dsDownload.SelectParameters.Add(New Parameter("pDocType", System.Data.DbType.String, lstDocType.Text.ToUpper))
        grdDownload.DataSourceID = "dsDownload"
        grdDownload.Sort(lstOrderBy_download.SelectedValue, Nothing)
        grdDownload.DataBind()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Common routine
    '--------------------------------------------------------------------------------------------------------------------
    'Private Function downloadFile(ByVal directory As String, ByVal fileName As String) As Boolean
    '    Dim path As String = Nothing
    '    Dim bytes As Byte() = Nothing
    '    'Dim customerID As String = IIf(Session("BALANCE_CMD") = "CUST", Session("BALANCE_CUSTID"), lstCustomer_balance.Text)

    '    '    r = DB.GetReader(String.Format("select filename, directory from customer_document where customer_id = {0} and docname = 'Invoice 2/4/2013'", _
    '    '                     DB.SqlEsc(customerID)))

    '    'If r.Read() = True Then
    '    'path = MapPath(Common.DOC_DIR & r("directory") & "\" & r("filename"))
    '    path = MapPath(Common.DOC_DIR & directory & "\" & fileName)
    '    If My.Computer.FileSystem.FileExists(path) = True Then
    '        bytes = System.IO.File.ReadAllBytes(path)
    '        Response.Clear()
    '        Response.ClearHeaders()
    '        Response.AddHeader("Content-Type", "Application/octet-stream")
    '        Response.AddHeader("Content-Length", bytes.Length.ToString())
    '        Response.AddHeader("Content-Disposition", "attachment;   filename=""" + fileName + """")
    '        Response.BinaryWrite(bytes)
    '        Response.Flush()
    '        Response.End()
    '        Return True
    '    Else
    '        Return False
    '    End If
    '    'End If
    '    'r.close()
    'End Function

 End Class


#End ExternalSource
