#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\PaymentDtl.ascx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","EB0AD5385DEEB95463A1BD94A73B8FAE42BBFE3E")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Common\PaymentDtl.ascx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates

Partial Class Common_PaymentDtl
    Inherits System.Web.UI.UserControl
    Private mDB As New DBHelper
    Private SSL_Certificate_Validation_Errors As String

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'Post process for modal dialog box
        If Request("DialogResult") = "YES" Then
            dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"))
        ElseIf Request("DialogResult") = "NO" Then
            dialogResult_no(ViewState("DIALOG_CMD"))
        End If

        If IsPostBack = False Then

        End If
    End Sub

    Public Sub pageLoad(ByVal paymentID As String)
        Dim SQL As String
        Dim dt As DataTable
        Dim jfcBranchNr As String = ""
        Dim serverDayOffset As Decimal
        Dim jfcVendorTimeZone As String = ""

        'Get customer info
        SQL = "SELECT customer_id FROM customer_payHdr WHERE payment_id = :1"
        lblCustomerID.Text = CString(mDB.GetScalar(SQL, paymentID))
        GetJFCBranchAndCustomerNr(mDB, lblCustomerID.Text, jfcBranchNr, Nothing)
        serverDayOffset = GetServerDayOffset(mDB, jfcBranchNr, jfcVendorTimeZone)

        'Set properties
        grdPayDtl.Columns(5).Visible = IIf(GetCustWebDiscType(mDB, lblCustomerID.Text) = String.Empty, False, True) 'Web Discount
        grdPayDtl.Columns(7).Visible = AllowCustInputPayAmt(mDB, lblCustomerID.Text) 'Note
        If IsMobile() = True Then
            grdPayDtl.Columns(3).Visible = False 'Reference
            grdPayDtl.Columns(5).Visible = False 'Web Discount
            grdPayDtl.Columns(7).Visible = False 'Note
        Else
            pnlPayHdr.Width = 500
        End If
        btnVoidPayment.Visible = canVoidPayment(paymentID) 'Allow to void if payment has not been processed and made within 2 hours.
        lblPaymentID.Text = paymentID

        'Show payment header
        SQL = "SELECT p.*, c.customerName, c.jfcBranchNr, jfcCustomerNr, code2.codeDesc AS status " & _
                " , CASE WHEN paymentAmt <> approvedAmt THEN '($' || approvedAmt || ' approved)' ELSE '' END AS approvedAmtDesc " & _
                " , CASE WHEN p.paymentStatus_id IN ('E', 'B') THEN code.codeDesc ELSE '' END AS result " & _
                " , TO_CHAR(p.createDate + " & serverDayOffset & ", :1) || ' ' || " & mDB.SqlEsc(jfcVendorTimeZone) & " || ' ' || CASE WHEN p.createUser IS NOT NULL THEN ' by ' || p.createUser END createDateTimeUser " & _
                "FROM customer c, customer_payHdr p " & _
                "LEFT OUTER JOIN codemaster code ON code.codegrp = 'FORTE_SETTLERESULT' AND p.forteSettleResultCode = code.code_id " & _
                "LEFT OUTER JOIN codemaster code2 ON code2.codegrp = 'PAYMENTSTATUS' AND p.paymentStatus_id = code2.code_id " & _
                "WHERE p.customer_id = c.customer_id AND p.payment_id = :2"
        dt = mDB.GetDataTable(SQL, DATETIME_SQLFMT, paymentID)
        frmPayHdr.DataSource = dt
        frmPayHdr.DataBind()

        'Show payment detail
        SQL = "SELECT b.*, d.fileName, d.directory, " & _
                " CASE WHEN b.arType = 'SERVICE CHARGE' AND b.sectionCode = '5' THEN 'Interest' WHEN b.sectionCode = '5' THEN 'Liquor' ELSE '' END isLiquor, " & _
                " SUBSTR(CASE WHEN b.arType = 'SERVICE CHARGE' AND b.sectionCode = '5' THEN 'LIQUOR INTEREST' ELSE b.arType END, 1 " & IIf(IsMobile(), ",3", "") & ") arType_view, " & _
                " CASE WHEN ADD_MONTHS(b.arDate, " & Common.CUST_INVOICE_DOWNLOAD_MONTH & ") > SYSDATE AND " & IIf(Common.ALLOW_CUST_DOWNLOAD_INVOICE = True, "(1 = 1)", "(1 = 2)") & " THEN 'Y' ELSE 'N' END AS allowCustDownloadInvoice " & _
                "FROM customer_payHdr a, customer_payDtl b " & _
                "LEFT OUTER JOIN customer_document d ON b.reference = d.docName AND UPPER(d.docType) = 'INVOICE' AND d.customer_id = " & mDB.SqlEsc(lblCustomerID.Text) & " " & _
                "WHERE a.payment_id = b.payment_id AND a.payment_id = " & mDB.SqlEsc(paymentID)
        '" CASE WHEN b.sectionCode = '5' THEN 'Y' ELSE 'N' END isLiquor " & _
        dsPayDtl.ProviderName = "System.Data.OracleClient"
        dsPayDtl.ConnectionString = mDB.GetConnectionString
        dsPayDtl.SelectCommand = SQL
        grdPayDtl.DataSourceID = "dsPayDtl"
        grdPayDtl.Sort("arDate, arNumber", SortDirection.Ascending)
        grdPayDtl.DataBind()
    End Sub

    Protected Sub grdPayDtl_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPayDtl.RowCommand
        'Dim row As GridViewRow = Nothing
        Dim docName As String
        Dim SQL As String
        Dim r As DataReader
        Dim rtn As Boolean = False

        Session("BALANCE_TAB") = Balance_Tab.PAYMENT
        If e.CommandName = "DOWNLOAD" Then
            'Download reference
            'row = grdDownload.Rows(CType(e.CommandArgument, Integer))
            'docName = CType(row.Cells(2).Controls(0), LinkButton).Text
            docName = Convert.ToString(e.CommandArgument)
            SQL = "SELECT directory, fileName FROM customer_document WHERE customer_id = :1 AND docName = :2"
            r = mDB.GetReader(SQL, lblCustomerID.Text, docName)
            If r.Read Then
                rtn = DownloadFile(CString(r("directory")), CString(r("fileName")))
            End If
            If rtn = False Then MessageBox("Cannot open the document", "Error", mDB)
        End If
    End Sub

    Protected Sub btnVoidPayment_Click(sender As Object, e As System.EventArgs) Handles btnVoidPayment.Click
        voidPayment(lblPaymentID.Text)
        'Cannot confirm because runtime error occurs while calling CreateEmailBody()
        'ViewState("DIALOG_CMD") = "VOID_PAYMENT"
        'ViewState("DIALOG_ARG1") = lblPaymentID.Text
        'MessageBoxYesNo("Are you sure you want to void this payment?", "Confirm", mDB)
        Exit Sub
    End Sub

    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        'Dim paymentID As String

        Select Case dialogCmd
            'Case "VOID_PAYMENT"
            '    paymentID = dialogArg1
            '    voidPayment(paymentID)
        End Select
    End Sub

    Private Sub dialogResult_no(ByVal dialogCmd As String)
    End Sub

    'Only allow to void if it's a bank account payment, has not been processed and made within 2 hours.  
    Private Function canVoidPayment(ByVal paymentID As String) As Boolean
        Dim SQL As String = "SELECT payment_id FROM customer_payHdr WHERE paymentStatus_id = 'P' AND ForteProcessDate IS NULL AND jfcInitPrintDate IS NULL AND transactionType = :1 AND createDate > SYSDATE - 2/24 AND payment_id = :2"
        Return IIf(CString(mDB.GetScalar(SQL, PAYMENT_TRANSACTION_TYPE_BANKACC, paymentID)) <> String.Empty, True, False)
    End Function

    'Using Forte Advanced Gateway Inerface (AGI) Direct Socket Interface
    'http://www.forte.net/devdocs/pdf/agi_integration.pdf

    Private Sub voidPayment(ByVal paymentID As String)
        Dim myTCPClient As New Sockets.TcpClient
        Dim mySSLStream As SslStream = Nothing
        Dim paramAry As String() = Nothing
        Dim SQL As String
        Dim r As DataReader
        Dim customerID As String = lblCustomerID.Text
        Dim customerName As String = GetCustName(mDB, customerID)
        Dim responseCode, responseDesc As String
        Dim emailBody As String = ""
        Dim errMsg As String = ""
        Dim userEmail As String = GetUserEmail(mDB)
        Dim emailSent As Boolean
        Dim vendorEmail As String
        Dim isProduction As Boolean = Common.IsProduction()
        Dim jfcBranchNr As String = ""

        Session("BALANCE_TAB") = Balance_Tab.PAYMENT

        'Check error
        If canVoidPayment(paymentID) = False Then
            MessageBox("You cannot void this payment any more.", "Error", mDB)
            Exit Sub
        End If

        'Get payment info
        Common.GetJFCBranchAndCustomerNr(mDB, customerID, jfcBranchNr, Nothing)
        SQL = "SELECT * FROM customer_payHdr WHERE payment_id = :1"
        r = mDB.GetReader(SQL, paymentID)
        If r.Read() Then
            paramAry = {"pg_merchant_id=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_MERCHANT_ID", jfcBranchNr), PAYMENT_SANDBOX_MERCHANT_ID), _
                        "pg_password=" & IIf(isProduction, GetCodeMaster(mDB, "FORTE_PASSWORD", jfcBranchNr), PAYMENT_SANDBOX_PASSWORD), _
                        "pg_transaction_type=24", _
                        "pg_original_authorization_code=" & CString(r("authorizationCode")), _
                        "pg_original_trace_number=" & CString(r("traceNr")), _
                        "endofdata"}
        End If
        r.close()

        'Init
        lblResults.Text = String.Empty

        'Void payment
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

        'Check void result
        responseCode = GetFortePaymentRtnCode(lblResults.Text, "PG_RESPONSE_CODE")
        If Left(responseCode, 1) <> "A" Then
            responseDesc = GetFortePaymentRtnCode(lblResults.Text, "PG_RESPONSE_DESCRIPTION")
            MessageBox("Could not void this payment. The error message from the payment processor was '" & responseCode & ": " & responseDesc & "'.", "Error", mDB)
            Exit Sub
        End If

        'Payment voided
        UpdatePaymentStatus(mDB, paymentID, "V")

        'Get email body for user from email template in user control
        vendorEmail = GetJfcEmail(mDB, customerID, False)
        CreateEmailBody(Page, emailBody, "PAY", paymentID, "You have voided your payment.", "CUSTOMER")
        'Send confirmation email to user
        If Common.SendMail(SYSTEM_EMAIL, userEmail, vendorEmail, "Payment Voided (" & paymentID & ")", emailBody, Nothing, True, errMsg) > 0 Then
            'Email sent failed
            emailSent = False
        Else
            'Email sent OK
            emailSent = True
        End If

        'Send email to vendor.
        CreateEmailBody(Page, emailBody, "PAY", paymentID, "The following payment has been voided by your customer.", "JFC")
        Common.SendMail(SYSTEM_EMAIL, vendorEmail, userEmail, "Payment '" & paymentID & "' has been voided", emailBody, Nothing, True, errMsg)

        'Refresh payment detail
        InsertEventLog(mDB, "Payment was voided" & If(emailSent = True, " and confirmation email sent.", ""), customerID, paymentID, Nothing, True)
        MessageLine("CONFIRM", "Successfully voided")
        pageLoad(paymentID)
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
End Class


#End ExternalSource
