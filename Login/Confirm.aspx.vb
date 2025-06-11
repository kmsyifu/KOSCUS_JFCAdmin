Imports System.Data
Imports Common
Imports DBUpdate

Partial Class Login_Confirm
    Inherits System.Web.UI.Page

    '--------------------------------------------------------------------------------------------------------------------
    'Parameters passed by caller
    'Session("CONFIRM_CMD")
    'Session("CONFIRM_ID")
    'Session("CONFIRM_PASSWORD")
     '--------------------------------------------------------------------------------------------------------------------

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Dim p As Page = TryCast(Me.Context.Handler, Page)
        If p IsNot Nothing Then
            ' set master page
            If IsMobile() = True Then
                p.MasterPageFile = "Login_Mobile.master"
            Else
                p.MasterPageFile = "Login.master"
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If IsInSession() = False Then Response.Redirect("Login.aspx")
        'Init
        DataBind()

        Dim SQL As String
        Dim cmd As String = CString(Session("CONFIRM_CMD")).ToUpper
        Dim id As String = Session("CONFIRM_ID")
        Dim passwd As String = Session("CONFIRM_PASSWORD")
        Session("CONFIRM_IPADDRESS") = getclinetIP()
        Dim orderState As String

        If IsPostBack = False Then
            Using DB As New DBHelper()
                Select Case cmd
                    Case "CONFIRM_ORDER"
                        ClearCtrl({lblErrorMsg, lblConfirmMsg})
                        'Error check
                        SQL = "SELECT onetimePassword FROM so_email WHERE webOrder_id = :1 AND emailType = 'VENDOR'"
                        If passwd <> CString(DB.GetScalar(SQL, id)) Then
                            'Invalid link
                            lblErrorMsg.Text = "Invalid link! You cannot confirm the order with the link. Please contact your customer."
                            Exit Sub
                        End If
                        orderState = GetOrderState(DB, id)
                        If orderState = "D" Then
                            lblConfirmMsg.Text = "You have successfully confirmed cancelation on an order <" & id & ">. Thank you!"
                        ElseIf orderState = "U" Then
                            lblConfirmMsg.Text = "You have successfully confirmed a updated order <" & id & ">. Thank you!"
                        Else
                            lblConfirmMsg.Text = "You have successfully confirmed an order <" & id & ">. Thank you!"
                        End If
                    Case "VERIFY_OTP"
                        btnClose.Visible = False
                        lnkResend.Visible = True
                        BtnSubmit.Visible = True
                        lblTxtTitle.Visible = True
                        lblTitle.Visible = True
                        TxtOTP.Visible = True
                        SQL = "select count(*) from webuser_otp WHERE email = " & DB.SqlEsc(id) & " AND ENABLED = 'Y' And IPADDRESS = " &
                            DB.SqlEsc(Session("CONFIRM_IPADDRESS")) & "  And EXPIREDATE > sysdate "
                        If Not Session("Confirm_refresh") And DB.GetScalar(SQL) = 0 Then
                            SentOTP(id, passwd, Session("CONFIRM_IPADDRESS"))
                            Session("Confirm_refresh") = True
                        End If
                        lblTitle.Text = "Enter your verification code"
                        lblConfirmMsg.Text = "A verification code has been sent to " & id & ". Please enter the code that you received within 20 minutes. If you do not receive the code within 10 minutes, please "
                End Select
            End Using
        Else

            Session("Confirm_refresh") = True
        End If

    End Sub

    'Cannot create email body with Page_Load()
    Protected Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload
        Dim emailBody As String = ""
        Dim errMsg As String = ""
        Dim id As String = Session("CONFIRM_ID")
        Dim cmd As String = CString(Session("CONFIRM_CMD")).ToUpper
        'Dim orderState As String
        'Dim subject As String
        'Dim customerEmail As String
        'Dim vendorEmail As String

        If IsPostBack = False Then
            Select Case cmd
                Case "VERIFY_OTP"
                    'Case "SKIP_ORDER"
                Case Else
                    'Using DB As New DBHelper()
                    '    If IsJfcOrder(DB, id) = False Then
                    '        orderState = GetOrderState(DB, id)
                    '        customerEmail = GetOrderEmail(DB, id, "CUSTOMER")
                    '        vendorEmail = GetOrderEmail(DB, id, "VENDOR")
                    '        If orderState = "D" Then
                    '            'Mark email as received for non JFC vendor's order
                    '            UpSertOrderEmail(DB, id, "VENDOR", "R", "VENDOR")
                    '            'Send customer a email stating the cancelation was received for non JFC vendor's order
                    '            CreateEmailBody(Page, emailBody, "ORDER", id, "CUSTOMER", "APPROVED_CANCELATION")
                    '            If SendMail(SYSTEM_EMAIL, customerEmail, vendorEmail, "Your order cancelation '" & id & "' was processed", emailBody, Nothing, True, errMsg) > 0 Then
                    '                'Error occurred
                    '            End If
                    '        ElseIf orderState = "N" OrElse orderState = "O" OrElse orderState = "U" Then
                    '            'Mark email and order as received for non JFC vendor's order
                    '            UpSertOrderEmail(DB, id, "VENDOR", "R", "VENDOR")
                    '            UpdateOrderState(DB, id, "C")
                    '            'Send customer an email stating the order was received for non JFC vendor's order
                    '            CreateEmailBody(Page, emailBody, "ORDER", id, "CUSTOMER", "APPROVED_ORDER")
                    '            subject = IIf(orderState = "N" OrElse orderState = "O", "Your order <" & id & "> was processed", "Your updated order '" & id & "' was processed")
                    '            If SendMail(SYSTEM_EMAIL, customerEmail, vendorEmail, subject, emailBody, Nothing, True, errMsg) > 0 Then
                    '                'Error occurred
                    '            End If
                    '        End If
                    '    End If
                    '    InsertEventLog(DB, "Confirmed order by vendor", id, Nothing, Nothing, True)

                    'End Using
            End Select
        End If
    End Sub

    'As calling GridView.RenderControl(htmlTextWriter), the page raises an exception that a Server-Control was rendered outside of a Form. Avoid this execption by overriding VerifyRenderingInServerForm
    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub

    Protected Sub lnkResend_Click(sender As Object, e As System.EventArgs) Handles lnkResend.Click
        SentOTP(Session("CONFIRM_ID"), Session("CONFIRM_PASSWORD"), Session("CONFIRM_IPADDRESS"))
    End Sub
   

    Private Sub SentOTP(email As String, DeviceCode As String, ip As String)
        Dim errMsg As String = ""
        Dim code As String = Common.randomString(6, "0123456789")
        Dim Message = " Please use the following verification code to log in. <br /> <br /> " &
            " Secure verification code:" & code &
            "<br /> <br /> Please note that this code will expire in 20 minutes."
        If Common.SendMail(Common.SYSTEM_EMAIL, email, Nothing, "One time verification code", Message, Nothing, True, errMsg) > 0 Then

        End If

        Using DB As New DBHelper()
            DBUpdate.UpSertWebUserOTP(DB, email, DeviceCode, ip, code, "EMAIL")
        End Using

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles BtnSubmit.Click
        Dim cmd As String = CString(Session("CONFIRM_CMD")).ToUpper
        Dim id As String = Session("CONFIRM_ID")
        Select Case cmd
            Case "VERIFY_OTP"

                Using DB As New DBHelper
                    Dim msg As String = String.Empty
                    If Not Regex.IsMatch(TxtOTP.Text, "^\d+$") Then
                        msg = "Only numbers are allowed"
                        lblOTPErrorMsg.Visible = True
                        lblOTPErrorMsg.Text = msg
                        Exit Sub
                    End If
                    Dim email = Session("CONFIRM_ID")
                    If Common.verifyotp(DB, email, TxtOTP.Text, msg) Then
                        Session("SYS_SESSION_DEFAULTURL") = "~/Login/LoginType.aspx?" & IIf(ViewState("USER_TYPE") <> String.Empty, "type=" & ViewState("USER_TYPE"), "") &
                               IIf(ViewState("CUSTOMERID") <> String.Empty, "&custid=" & ViewState("CUSTOMERID"), "") & IIf(ViewState("PAGE") <> String.Empty, "&page=" & ViewState("PAGE"), "") '& IIf(ViewState("RETURN_URL") <> String.Empty, "&ReturnUrl=" & ViewState("RETURN_URL"), "")
                        Response.Redirect(Session("SYS_SESSION_DEFAULTURL"))
                    Else
                        lblOTPErrorMsg.Visible = True
                        lblOTPErrorMsg.Text = msg
                    End If

                End Using
                'Case "SKIP_ORDER"
                '    Using DB As New DBHelper
                '        Dim msg As String = String.Empty
                '        Dim IDs As String() = HttpUtility.UrlDecode(id).Split("-")
                '        UpdateDeliveryDate(DB, IDs(0), IDs(1), IDs(2), IDs(3), IDs(3), IDs(4), "N")

                '    End Using
        End Select

    End Sub

    Protected Function getclinetIP() As String
        Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
        Dim ipAddress = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(ipAddress) Then
            Return ipAddress

        End If
        Return IIf(context.Request.ServerVariables("REMOTE_ADDR") = "::1", "local", context.Request.ServerVariables("REMOTE_ADDR"))
    End Function
End Class
