Imports System.Data
Imports System.IO
Imports System.Collections.Generic
Imports System.Xml
Imports Microsoft.Win32
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Net.Mail 'Aspose.Email.Mail 
Imports DBUpdate
Imports Twilio 'For texting
Imports Twilio.Rest.Api.V2010.Account 'For texting
Imports System.Globalization
Imports System.Threading
Imports System.Resources
Imports System.Drawing
Imports System.Drawing.Imaging

Public Structure SessionInfo
    Public AssemblerID As String
    Public UserName As String
    Public LabelPrinter As String
    Public LaserPrinter As String
    Public WhsID As String
    Public IP As String
    Public ShipSlipPick As Boolean
End Structure

Public Enum CustAdmin_Tab
    COMPANY = 0
    USERACC
    CUSTGRP
    CUST
    VENDOR
    CTG
    ITEMGRP
    CUSTGRPITEM
    CUSTITEM
    SCHEDULE
End Enum

Public Enum JfcAdmin_Tab
    JFC = 0
    USERACC
    JFCBRANCH
    CUSTCOMPANY
    VENDORCOMPANY
End Enum

Public Enum CustSetting_Tab
    CUST = 0
    SCHEDULE
    CUSTITEM
End Enum

Public Enum CustItem_Tab
    CUSTITEM = 0
    PROMOITEM
End Enum

Public Enum Master_Tab
    PROMOTIONITEM = 0
    HOMEPAGEIMG
    DELIVERYMASTER
    NOTIFICATION
End Enum
Public Enum Tool_Tab
    Report = 0
End Enum

Public Enum CustDtl_Tab
    CUST = 0
    VENDOR
    ITEM
    SCHEDULE
End Enum

Public Enum MyAccount_Tab
    USERACC = 0
    PAYMENTACC
End Enum

Public Enum Balance_Tab
    BALANCE = 0
    PAYMENT
    DOWNLOAD
End Enum

Public Enum Init_Level
    CONTENT = 0
    DYNAMIC_SEARCH
    ALL
End Enum

Public Class Common
    Public Const VERSION As String = "007"
    Public Shared CURRENT_YEAR As String = Now.Year
    Public Const SYSTEM_EMAIL As String = "JFC App <web@jfc.com>" '"JFC Customer Portal <web@jfc.com>"
    Public Const SYSTEM_TEXT As String = "+13238949338"
    Public Const JFC_COMPANY_NAME As String = "JFC"
    Public Const SUPPORT_EMAIL As String = "kumeda@kmsnet.com, jzhou@kmsnet.com, yfu@kmsnet.com"
    Public Const DEMO_EMAIL As String = "demo@jfc.com"
    Public Const WEB_URL As String = "https://shop.jfc.com/"
    Public Const WEB_JFCADMIN_URL As String = "https://shopadmin.jfc.com/"
    Public Const APP_URL As String = "https://app.jfc.com/"
    Public Const LOGIN_NOTICE As String = "" '"This system will be unavailable for maintenance on Friday, September 5th from 7:30AM to 9:30AM Pacific Time."
    Public Const AUTO As String = "Auto"
    Public Const DOC_DIR As String = "~\Documents\"
    Public Const DATE_FMT As String = "MM/dd/yyyy"
    Public Const SHORTDATE_FMT As String = "MM/dd"
    Public Const DATETIME_VBFMT As String = "MM/dd/yyyy hh:mm tt"
    Public Const SHORTDATETIME_VBFMT As String = "MM/dd/yy hh:mmtt"
    Public Const DATETIME_SQLFMT As String = "mm/dd/yyyy hh:mi AM"
    Public Const SHORTDATETIME_SQLFMT As String = "mm/dd/yy hh:miAM"
    Public Const SHORTDATETIME_DELFMT As String = "hh:miAM mm/dd"
    Public Const DAY_SQLFMT As String = "FMddth"
    Public Const TIME_SQLFMT As String = "hh:miAM"
    Public Const DAYDATE_SQLFMT As String = "Dy mm/dd"
    Public Const DAYDATE_LONG_SQLFMT As String = "Dy mm/dd/yyyy"
    Public Const CUSTGRP_ITEM_ID_LEN As Integer = 12
    Public Const CUSTGRP_VENDOR_ID_LEN As Integer = 12
    Public Const DeliverySchedID_LEN As Integer = 12
    Public Const CUSTOMER_ID_LEN As Integer = 6
    Public Const VENDOR_ID_LEN As Integer = 6
    Public Const ITEM_ID_LEN As Integer = 5
    Public Const ITEMGRP_ID_LEN As Integer = 6
    Public Const CTG_ID_LEN As Integer = 6
    Public Const DELIVERYSCHED_ID_LEN As Integer = 12
    Public Const DELIVERYMASTER_ID_LEN As Integer = 4
    Public Const CART_ID_LEN As Integer = 11
    'Public Const WEBORDER_ID_LEN As Integer = 5 '8
    Public Const LOG_ID_LEN As Integer = 12
    Public Const EVENT_DESC_ARG_LEN As Integer = 128
    Public Const HOMEPAGE_ID_LED As Integer = 6
    Public Const PAYMENT_ID_LEN As Integer = 8 'When update, update Batch_FortePaymentSync too

    Public Const LST_CMD_STR As String = "Please select..."
    Public Const LST_ALL_STR As String = "All"
    Public Const DEFAULT_RETURNED_CHECK_FEE As String = "$25"
    Public Const DEFAULT_ITEMGRP_NAME As String = "ORDER FORM A"
    Public Const RECENT_ORDER_NR As Integer = 5
    Public Const NR_NEW_ITMES As Integer = 12
    Public Const ORDER_BY_DATE_BUFFER As Decimal = 0.02 '30 minutes (unit is day)

    'Payment
    'Change @discountType in WebPayment.rpt and batch_fortePaymentSync program as well for any changes
    '1% DISCOUNT FOR WITHIN 31 DAYS (NON-LIQUOR)
    Public Const WebDiscDays_A As Integer = 31
    Public Const WebdiscRate_A As Decimal = 1
    '5% DISCOUNT FOR WITHIN 10 DAYS (NON-LIQUOR)
    Public Const WebDiscDays_B As Integer = 10
    Public Const WebdiscRate_B As Decimal = 5
    '5% DISCOUNT FOR AUTO PAY (NON-LIQUOR, used to employees) 
    Public Const WebDiscRate_Z As Decimal = 5

    'Payment using Forte
    Public Const PAYMENT_TRANSACTION_TYPE_BANKACC As String = "20"
    Public Const PAYMENT_TRANSACTION_TYPE_CARD As String = "10"
    Public Const PAYMENT_SSL_SERVER As String = "www.paymentsgateway.net"
    Public Const PAYMENT_MAX_TRANSACTION_AMT As Decimal = 50000
    'Public Const PAYMENT_MAX_TRANSACTION_AMT_TMP As Decimal = 10000
    'JFC Account
    Public Const PAYMENT_SSL_PORT As Integer = 5050
    'Public Const PAYMENT_MERCHANT_ID As String = "173624" 'Moved to FORTE_MERCHANT_ID in codemaster
    'Public Const PAYMENT_PASSWORD As String = "pvWo48BG6x7L" 'Moved to FORTE_PASSWORD in codemaster
    'Public Const PAYMENT_API_LOGIN_ID As String = "gJiWR32fbQ" 
    'Public Const PAYEMNT_SECURE_TRANSACTION_KEY As String = "65bFeRT25qHo" 
    'KMS Account
    'Public Const PAYMENT_SSL_PORT As Integer = 5050
    'Public Const PAYMENT_MERCHANT_ID As String = "166332"
    'Public Const PAYMENT_PASSWORD As String = "tloFF4W373F"
    ''Public Const PAYMENT_API_LOGIN_ID As String = "YXb1fEadcV"
    ''Public Const PAYEMNT_SECURE_TRANSACTION_KEY As String = "jr9TiJRxR3ov1F"
    'Sandbox
    Public Const PAYMENT_SANDBOX_SSL_PORT As Integer = 6050
    Public Const PAYMENT_SANDBOX_MERCHANT_ID As String = "165208"
    Public Const PAYMENT_SANDBOX_PASSWORD As String = "wi8GX35Cu"
    'Public Const PAYMENT_SANDBOX_API_LOGIN_ID As String = "3mQy4GuRr"
    'Public Const PAYEMNT_SANDBOX_SECURE_TRANSACTION_KEY As String = "omvEuPQCnU8Vlm"

    'Text using Twilio
    Public Shared ReadOnly TEXT_ACCOUNT_ID As String = System.Configuration.ConfigurationManager.AppSettings("TwilioAccountSid")
    Public Shared ReadOnly TEXT_AUTH_TOKEN As String = System.Configuration.ConfigurationManager.AppSettings("TwilioAuthToken")

    'Flag
    Public Const SHOW_MYSTORE_IN_MENU As Boolean = False
    Public Const SHOW_ITEM_IMG_IN_DEFAULT As Boolean = True
    Public Const ALLOW_CARD_PAYMENT As Boolean = False
    Public Const ENABLE_MOBILE_VERSION As Boolean = True
    Public Const SMALL_CUST_ONLY As Boolean = True 'True for current phase
    Public Const ALLOW_CUST_DOWNLOAD_INVOICE As Boolean = True 'True for current phase
    Public Const CUST_INVOICE_DOWNLOAD_MONTH As Integer = 6
    Public Const ALLOW_CUST_INPUT_PAY_AMT As Boolean = True 'True for current phase
    'Public Const CHECK_DELIVERY_DATE As Boolean = True 'True for current phase
    Public Const ALLOW_LB_ORDER As Boolean = False 'False for current phase
    'Public Const ALLOW_ORDER As Boolean = False 'False for current phase

    'Debug
    Public Const DEBUG As Boolean = False 'False for production
    Public Const TEST_MOBILE As Boolean = False 'False for production


    Public Shared Function Translate(ByVal Trans As String) As String
        Dim rm = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))
        Dim ci = Thread.CurrentThread.CurrentCulture
        rm.IgnoreCase = True

        If String.IsNullOrEmpty(Trans) Then
            Return ""
        Else
            Dim TransID = Trans.Replace(" ", "").Replace("&", "").Replace(",", "")
            Dim result As String = rm.GetString(TransID, ci)
            If String.IsNullOrWhiteSpace(result) Then
                Return Trans
            End If
            Return result
        End If
    End Function

    Public Shared Function DateTranslate(ByVal dateStr As String) As String
        Dim Chars() As Char = {" "}
        Dim words As String() = dateStr.Split(Chars, 2)
        If words.Count > 0 Then


            Dim Tras As String = Translate(words.First)
            If String.IsNullOrWhiteSpace(Tras) Then
                Return dateStr
            Else
                Return String.Join(" ", Tras, words(1))
            End If
        End If
        Return dateStr

    End Function


    Public Shared Sub MessageLine(ByVal type As String, message As String)
        Dim s As System.Web.UI.Page = HttpContext.Current.Handler
        Dim script As String

        script = "SystemLineMessage = '" & Replace(message, "'", "\'") & "'; SystemLineType = '" & type & "';"
        s.ClientScript.RegisterStartupScript(s.GetType(), "MessageLine", script, True)
    End Sub

    Public Shared Sub MessageBox(message As String, title As String, Optional DB As DBHelper = Nothing, Optional URL As String = "")
        Dim s As System.Web.UI.Page = HttpContext.Current.Handler
        Dim script As String

        If DB IsNot Nothing Then InsertEventLog(DB, "MessageBox", title, message, Nothing, True)

        script = "SystemDialogTitle = '" & Replace(title, "'", "\'") & "'; SystemDialogMessage = '" & Replace(message, "'", "\'") & "'; SystemDialogReturnURL = '" & URL & "'; "
        s.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    End Sub

    Public Shared Sub MessageBoxOKCancel(message As String, title As String, Optional DB As DBHelper = Nothing)
        Dim s As System.Web.UI.Page = HttpContext.Current.Handler
        Dim script As String

        If DB IsNot Nothing Then InsertEventLog(DB, "MessageBoxOKCancel", title, message, Nothing, True)

        script = "SystemDialogTitle = '" & Replace(title, "'", "\'") & "'; SystemDialogMessage = '" & Replace(message, "'", "\'") & "'; SystemDialogButton = 'OKCancel'; "
        s.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    End Sub

    'Public Shared Sub MessageBoxYesNo(message As String, title As String, Optional DB As DBHelper = Nothing)
    '    Dim s As System.Web.UI.Page = HttpContext.Current.Handler
    '    Dim script As String

    '    If DB IsNot Nothing Then InsertEventLog(DB, "MessageBoxYesNo", title, message, Nothing, True)

    '    script = "SystemDialogTitle = '" & Replace(title, "'", "\'") & "'; SystemDialogMessage = '" & Replace(message, "'", "\'") & "'; SystemDialogButton = 'YesNo'; "
    '    s.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    'End Sub

    Public Shared Sub MessageBoxYesNo(message As String, title As String, Optional DB As DBHelper = Nothing)
        Dim s As System.Web.UI.Page = HttpContext.Current.Handler
        Dim script As String
        If DB IsNot Nothing Then InsertEventLog(DB, "MessageBoxYesNo", title, message, Nothing, True)

        ' Properly escape special characters for JavaScript
        Dim escapedTitle As String = EscapeJavaScriptString(title)
        Dim escapedMessage As String = EscapeJavaScriptString(message)

        script = "SystemDialogTitle = '" & escapedTitle & "'; SystemDialogMessage = '" & escapedMessage & "'; SystemDialogButton = 'YesNo'; "
        s.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    End Sub

    ''' <summary>
    ''' Escapes a string to be safely used in JavaScript string literals
    ''' Handles quotes, backslashes, newlines, and other special characters
    ''' </summary>
    Private Shared Function EscapeJavaScriptString(input As String) As String
        If String.IsNullOrEmpty(input) Then Return ""

        ' Escape in correct order: backslash first, then other characters
        Return input.Replace("\", "\\") _
               .Replace("'", "\'") _
               .Replace("""", "\""") _
               .Replace(vbCrLf, "\n") _
               .Replace(vbLf, "\n") _
               .Replace(vbCr, "\n") _
               .Replace(vbTab, "\t") _
               .Replace(Chr(8), "\b") _
               .Replace(Chr(12), "\f") _
               .Replace(Chr(13), "\r")
    End Function

    Public Shared Sub MessageSimpleBox(ByVal message As String, Optional DB As DBHelper = Nothing)
        Dim s As System.Web.UI.Page = HttpContext.Current.Handler
        Dim strScript As String

        If DB IsNot Nothing Then InsertEventLog(DB, "MessageSimpleBox", message, Nothing, Nothing, True)

        strScript = "alert(""" & message & """);"
        s.ClientScript.RegisterStartupScript(s.GetType(), "ScriptManager", strScript, True)
    End Sub

    'The 2 ShowMsg() below doesn't appear on top of UI jQuery tabs
    'Public Shared Sub ShowMsg(ByVal page As System.Web.UI.Page, ByVal type As String, ByVal message As String)
    '    Dim strScript As String = "showMsg(""" & type & """, """ & message & """);"
    '    ScriptManager.RegisterStartupScript(page, page.GetType(), "MsgManager", strScript, True)
    'End Sub

    'Public Shared Sub ShowMsg(ByVal page As System.Web.UI.UserControl, ByVal type As String, ByVal message As String)
    '    Dim strScript As String = "showMsg(""" & type & """, """ & message & """);"
    '    ScriptManager.RegisterStartupScript(page, page.GetType(), "MsgManager", strScript, True)
    'End Sub

    Public Shared Sub CallJavaScript(ByVal page As System.Web.UI.Page, ByVal strScript As String)
        If Not page.ClientScript.IsStartupScriptRegistered(page.GetType(), "ScriptManager") Then
            ScriptManager.RegisterStartupScript(page, page.GetType(), "ScriptManager", strScript, True)
        End If
    End Sub

    Public Shared Function GetCurrentPageName() As String
        Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(HttpContext.Current.Request.Url.AbsolutePath)
        Dim sRet As String = oInfo.Name
        Return sRet.ToLower
    End Function

    Public Shared Function GetCurrentDirectory() As String
        'Return Path.GetDirectoryName(HttpContext.Current.Request.Url.AbsolutePath) & "/" 'Doesn't work in FireFox
        Return HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().LastIndexOf("/")) & "/"
    End Function

    Public Shared Function GetCurrentUrlWoQueryStr() As String
        Dim str As String() = HttpContext.Current.Request.Url.AbsoluteUri.Split("?")
        Return str(0)
    End Function

    Public Shared Function RemoveQueryStr(ByVal url As String) As String
        Dim str As String() = url.Split("?")
        Return str(0)
    End Function

    Public Shared Function GetSubDomain(ByVal url As Uri) As String
        Dim host As String
        Dim lastIndex, index As Integer
        If url.HostNameType = UriHostNameType.Dns Then
            host = url.Host
            If host.Split(".").Length > 2 Then
                lastIndex = host.LastIndexOf(".")
                index = host.LastIndexOf(".", lastIndex - 1)
                Return Common.CString(host.Substring(0, index)).ToLower
            End If
        End If
        Return String.Empty
    End Function

    Public Shared Sub SelectIfOneItem(ByVal lst As DropDownList)
        If lst.Items.Count = 2 Then
            lst.SelectedIndex = 1
        End If
    End Sub

    Public Shared Function IsNumber(ByVal str As String, Optional ByVal allowEmpty As Boolean = False, Optional ByVal allowNegative As Boolean = False) As Boolean
        If str.Trim = "" Then Return allowEmpty
        Return IsNumeric(str) AndAlso (allowNegative OrElse Val(str) >= 0)
    End Function

    Public Shared Function IsEmail(ByVal str As String, Optional ByVal allowEmpty As Boolean = False) As Boolean
        Dim emailAry As String()
        If str.Trim = "" Then Return allowEmpty
        Try
            emailAry = str.Split(New Char() {","c})
            For Each str In emailAry
                If str.Trim <> "" Then Dim email As New System.Net.Mail.MailAddress(str.Trim)
            Next
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Shared Function IsMobile() As Boolean
        If TEST_MOBILE = True Then Return True
        If ENABLE_MOBILE_VERSION = False Then Return False
        If HttpContext.Current.Session("SITE_MODE") = "FULL" Then
            Return False
        ElseIf HttpContext.Current.Session("SITE_MODE") = "MOBILE" Then
            Return True
        End If
        'Return HttpContext.Current.Request.Browser.IsMobileDevice
        'If HttpContext.Current.Request.Browser.IsMobileDevice = True Then
        Dim userAgent As String = CString(HttpContext.Current.Request.UserAgent).ToLower()
        If HttpContext.Current.Request.Browser.IsMobileDevice OrElse userAgent.Contains("iphone;") OrElse userAgent.Contains("windows phone") OrElse userAgent.Contains("android") Then
            HttpContext.Current.Session("SITE_MODE") = "MOBILE"
            Return True
        Else
            HttpContext.Current.Session("SITE_MODE") = "FULL"
            Return False
        End If
    End Function

    Public Shared Function IsProduction() As Boolean
        Return IIf(GetSubDomain(HttpContext.Current.Request.Url) = "shopadmin", True, False)
    End Function

    Public Shared Function IsDev() As Boolean
        'Return false for koscus beta
        Return IIf(HttpContext.Current.Request.Url.ToString.ToLower.IndexOf("localhost") > -1 OrElse HttpContext.Current.Request.Url.Host = "10.0.201.104", True, False)
    End Function

    Public Shared Function IsProdBeta() As Boolean
        'Return false for koscus beta
        Return IIf(GetSubDomain(HttpContext.Current.Request.Url) = "shopadmintest", True, False)
        'IIf(HttpContext.Current.Request.Url.Host = "192.168.207.5", True, False)
    End Function

    Public Shared Sub ClearCtrl(ByVal arrayTxt() As TextBox)
        For i As Integer = 0 To arrayTxt.Length - 1
            arrayTxt(i).Text = Nothing
        Next
    End Sub

    Public Shared Sub ClearCtrl(ByVal arrayLbl() As Label)
        For i As Integer = 0 To arrayLbl.Length - 1
            arrayLbl(i).Text = Nothing
        Next
    End Sub

    Public Shared Sub VisibleCtrl(ByVal arrayTxt() As TextBox)
        For i As Integer = 0 To arrayTxt.Length - 1
            arrayTxt(i).Visible = True
        Next
    End Sub

    Public Shared Sub VisibleCtrl(ByVal arrayLbl() As Label)
        For i As Integer = 0 To arrayLbl.Length - 1
            arrayLbl(i).Visible = True
        Next
    End Sub

    Public Shared Sub VisibleCtrl(ByVal arrayLst() As DropDownList)
        For i As Integer = 0 To arrayLst.Length - 1
            arrayLst(i).Visible = True
        Next
    End Sub

    Public Shared Sub InvisibleCtrl(ByVal arrayTxt() As TextBox)
        For i As Integer = 0 To arrayTxt.Length - 1
            arrayTxt(i).Visible = False
        Next
    End Sub

    Public Shared Sub InvisibleCtrl(ByVal arrayLbl() As Label)
        For i As Integer = 0 To arrayLbl.Length - 1
            arrayLbl(i).Visible = False
        Next
    End Sub

    Public Shared Sub InvisibleCtrl(ByVal arrayLst() As DropDownList)
        For i As Integer = 0 To arrayLst.Length - 1
            arrayLst(i).Visible = False
        Next
    End Sub

    'Public Shared Sub InitLastUpdate()
    '    HttpContext.Current.Session("UPDATE") = HttpContext.Current.Server.UrlEncode(System.DateTime.Now.ToString())
    'End Sub

    'Public Shared Function GetLastUpdate() As String
    '    Return HttpContext.Current.Session("UPDATE").ToString()
    'End Function

    'Public Shared Sub DisableCtrl(ByVal arrayTxt() As TextBox)
    '    For i As Integer = 0 To arrayTxt.Length - 1
    '        DisableCtrl(arrayTxt(i))
    '    Next
    'End Sub

    'Public Shared Sub DisableCtrl(ByVal txt As TextBox)
    '    txt.ReadOnly = True
    '    txt.Enabled = False
    '    txt.BorderStyle = BorderStyle.None
    '    txt.BackColor = Drawing.Color.White
    '    'txt.ForeColor = Drawing.Color.Orange 'Doesn't work. Stay Gray
    'End Sub

    'Public Shared Sub EnableCtrl(ByVal arrayTxt() As TextBox)
    '    For i As Integer = 0 To arrayTxt.Length - 1
    '        EnableCtrl(arrayTxt(i))
    '    Next
    'End Sub

    'Public Shared Sub EnableCtrl(ByVal txt As TextBox)
    '    txt.ReadOnly = False
    '    txt.Enabled = True
    '    txt.BorderStyle = BorderStyle.Inset
    '    'txt.BackColor = Drawing.Color.White
    'End Sub

    'Public Shared Sub DisableCtrl(ByVal lst As DropDownList)
    '    lst.Enabled = False
    '    'lst.BorderStyle = BorderStyle.None
    '    lst.BackColor = Drawing.Color.White
    '    'lst.ForeColor = Drawing.Color.Black 'Doesn't work. Stay Gray
    'End Sub

    Public Shared Function DuplicatedTxt(ByVal grd As GridView, ByVal txtName As String) As Boolean
        Dim txt, tmpTxt As TextBox

        For i As Integer = 0 To grd.Rows.Count - 1
            txt = DirectCast(grd.Rows(i).Cells(0).FindControl(txtName), TextBox)
            If txt.Text = "" Then Continue For
            For t As Integer = i + 1 To grd.Rows.Count - 1
                tmpTxt = DirectCast(grd.Rows(t).Cells(0).FindControl(txtName), TextBox)
                If txt.Text = tmpTxt.Text Then Return True 'Found duplicated value in txtName
            Next
        Next
        'No duplicated value in txtName
        Return False
    End Function

    Public Shared Function DuplicatedLst(ByVal grd As GridView, ByVal lstName As String) As Boolean
        Dim lst, tmpLst As DropDownList

        For i As Integer = 0 To grd.Rows.Count - 1
            lst = DirectCast(grd.Rows(i).Cells(0).FindControl(lstName), DropDownList)
            If lst.Text = "" Then Continue For
            For t As Integer = i + 1 To grd.Rows.Count - 1
                tmpLst = DirectCast(grd.Rows(t).Cells(0).FindControl(lstName), DropDownList)
                If lst.Text = tmpLst.Text Then Return True 'Found duplicated value in txtName
            Next
        Next
        'No duplicated value in txtName
        Return False
    End Function

    Public Shared Sub FocusVisibleQty(ByVal txtCsQty As TextBox, ByVal txtEaQty As TextBox, ByVal txtLbs As TextBox)
        If txtCsQty.Visible = True AndAlso txtCsQty.CssClass = "cellQtyTxt_highlight" Then txtCsQty.Focus() : Exit Sub
        If txtEaQty.Visible = True AndAlso txtEaQty.CssClass = "cellQtyTxt_highlight" Then txtEaQty.Focus() : Exit Sub
        If txtLbs.Visible = True AndAlso txtLbs.CssClass = "cellQtyTxt_highlight" Then txtLbs.Focus() : Exit Sub
        If txtCsQty.Visible = True Then txtCsQty.Focus() : Exit Sub
        If txtEaQty.Visible = True Then txtEaQty.Focus() : Exit Sub
        If txtLbs.Visible = True Then txtLbs.Focus() : Exit Sub
    End Sub

    Public Shared Sub FocusInvalidQty(ByVal txtCsQty As TextBox, ByVal txtEaQty As TextBox, ByVal txtLbs As TextBox)
        If IsNumber(txtCsQty.Text, True) = False Then txtCsQty.Focus() Else If IsNumber(txtEaQty.Text, True) = False Then txtEaQty.Focus() Else If IsNumber(txtLbs.Text, True) = False Then txtLbs.Focus()
    End Sub

    'Public Shared Function IsDefCustGrp(ByVal DB As DBHelper, ByVal custGrpID As String) As Boolean
    '    Dim SQL As String

    '    SQL = "SELECT defaultCustGrp FROM custGrp WHERE custGrp_id = " & DB.SqlEsc(custGrpID)
    '    Return IIf(CString(DB.GetScalar(SQL)) = "Y", True, False)
    'End Function

    Public Shared Function GetFortePaymentRtnCode(ByVal rtnStr As String, ByVal cmd As String) As String
        Dim index As Integer
        index = rtnStr.ToUpper.IndexOf(cmd & "=")
        If index < 0 Then Return String.Empty
        'Return rtnStr.Substring(index + cmd.Length + 1, codeLen)
        Return rtnStr.Substring(index + cmd.Length + 1).Split(vbLf)(0)
    End Function


    Public Shared Function GetMimeType(ByVal fileName As String) As String
        Dim mimeType As String
        Dim extension As String = CString(System.IO.Path.GetExtension(fileName))

        Select Case extension.Replace(".", "").ToLower
            Case "pdf"
                mimeType = "application/pdf"
            Case "ppt"
                mimeType = "application/vnd.ms-powerpoint"
            Case "pptx"
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"
            Case "doc"
                mimeType = "application/msword"
            Case "docx"
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case "rtf"
                mimeType = "application/rtf"
            Case "xls"
                mimeType = "application/vnd.ms-excel"
            Case "xlsx"
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case "csv"
                mimeType = "text/csv"
            Case "zip"
                mimeType = "application/zip"
            Case "jpg", "jpeg"
                mimeType = "image/jpeg"
            Case "png"
                mimeType = "image/png"
            Case "gif"
                mimeType = "image/gif"
            Case "bmp"
                mimeType = "image/bmp"
            Case "svg"
                mimeType = "image/svg+xml"
            Case "tiff", "tif"
                mimeType = "image/tiff"
            Case "bmp"
                mimeType = "image/bmp"
            Case "txt"
                mimeType = "text/plain"
            Case "css"
                mimeType = "text/css"
            Case "html"
                mimeType = "text/html"
            Case "htm"
                mimeType = "text/html"
            Case "mpeg"
                mimeType = "video/mpeg"
            Case "avi"
                mimeType = "video/msvideo, video/avi, video/x-msvideo"
            Case "qt"
                mimeType = "video/quicktime"
            Case "wav"
                mimeType = "audio/wav, audio/x-wav"
            Case "midi"
                mimeType = "audio/x-midi"
            Case "mp3"
                mimeType = "audio/mpeg"
            Case "au"
                mimeType = "audio/basic"
            Case "ogg"
                mimeType = "audio/vorbis, application/ogg"
            Case "swf"
                mimeType = "application/x-shockwave-flash"
            Case "xml"
                mimeType = "application/xml"
            Case Else
                mimeType = "application/octet-stream"
        End Select
        Return mimeType
    End Function

    Public Shared Function DownloadFile(ByVal directory As String, ByVal fileName As String) As Boolean
        Dim path As String = Nothing
        Dim bytes As Byte() = Nothing

        path = directory & "/" & fileName
        If My.Computer.FileSystem.FileExists(path) = True Then
            bytes = System.IO.File.ReadAllBytes(path)
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ClearHeaders()
            'HttpContext.Current.Response.AddHeader("Content-Type", "application/octet-stream") 'Doesn't work in Android
            HttpContext.Current.Response.AddHeader("Content-Type", GetMimeType(fileName)) 'application/pdf
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName.ToUpper & """")
            'HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=""" & fileName.ToUpper & """") 'Open file on browser
            HttpContext.Current.Response.AddHeader("Content-Length", bytes.Length.ToString())
            HttpContext.Current.Response.BinaryWrite(bytes)
            HttpContext.Current.Response.Flush()
            HttpContext.Current.Response.End()
            'Doesn't execute anything after Response.End()
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function IsToday(ByVal DB As DBHelper, ByVal jfcBranchNr As String, ByVal dateStr As String, ByVal dateFmt As String) As Boolean
        Dim SQL As String
        Dim result As String
        SQL = "SELECT 'Y' FROM DUAL WHERE " & DB.SqlEsc(dateStr) & " = TO_CHAR(SYSDATE + " & GetServerDayOffset(DB, jfcBranchNr) & ", :1) "
        result = CString(DB.GetScalar(SQL, dateFmt))
        Return IIf(result = "Y", True, False)
    End Function

    Public Shared Function IsKmsSupport(ByVal DB As DBHelper, ByVal email As String) As Boolean
        Dim SQL As String
        SQL = "SELECT userCompany_id FROM webUser_vendor WHERE vendorCompany_id = 'JFC' and email = :1"
        Return IIf(CString(DB.GetScalar(SQL, email)) = "KMS_SUPPORT", True, False)
    End Function

    Public Shared Function IsJfcHoAdmin(ByVal DB As DBHelper, ByVal email As String) As Boolean
        Dim SQL As String
        SQL = "SELECT userCompany_id FROM webUser_vendor WHERE vendorCompany_id = 'JFC' and email = :1"
        Return IIf(CString(DB.GetScalar(SQL, email)) = "JFC_HO", True, False)
    End Function

    Public Shared Function IsJfcBranchAdmin(ByVal DB As DBHelper, ByVal email As String) As Boolean
        Dim SQL As String
        SQL = "SELECT userCompany_id FROM webUser_vendor WHERE vendorCompany_id = 'JFC' and email = :1"
        Return IIf(CString(DB.GetScalar(SQL, email)) = "JFC_BRANCH", True, False)
    End Function

    'YI FU 11/17/2020
    Public Shared Function UseApp(ByVal DB As DBHelper, ByVal email As String) As Boolean
        Dim SQL As String
        'SQL = "select count(*) from  webuser_customer uc, customer c where uc.customer_id = c.customer_id  and c.jfcbranchnr IN ('30','40','45') and email = :1 "
        SQL = "select count(*) from  webuser_customer uc, customer c, jfcVendor v where uc.customer_id = c.customer_id and c.jfcBranchNr = v.jfcBranchNr and v.useApp = 'Y' and uc.email = :1"
        Return IIf(Val(DB.GetScalar(SQL, email)) > 0, True, False)
    End Function

    Public Shared Function GetLangauge(ByVal DB As DBHelper, ByVal email As String) As String
        Dim langauge As String = Common.CString(DB.GetScalar("select Language from WebUser where email = :1", email))

        Return IIf(langauge = "", "en-US", langauge)

    End Function


    Public Shared Function IsJfcBranch(ByVal DB As DBHelper, ByVal custGrpVendorID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT v.vendor_id FROM custGrp_vendor c, jfcVendor v WHERE c.vendor_id = v.vendor_id AND c.custGrpVendor_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, custGrpVendorID)) = "", False, True)
    End Function

    Public Shared Function IsJfcOrder(ByVal DB As DBHelper, ByVal webOrderID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT custGrpVendor_id FROM sohdr WHERE webOrder_id = :1"
        Return IsJfcBranch(DB, CString(DB.GetScalar(SQL, webOrderID)))
    End Function

    Public Shared Function IsJfcItem(ByVal DB As DBHelper, ByVal custGrpItemID As String) As Boolean
        Dim SQL As String
        Dim custGrpID As String = ""

        SplitSCustGrpItemID(custGrpItemID, custGrpID, Nothing)
        SQL = "SELECT jfcItem FROM v_custgrp_item_small WHERE custgrp_id = " & DB.SqlEsc(custGrpID) & " AND custGrpItem_id = :1"
        '"  AND custgrp_id = " & DB.SqlEsc(custGrpID) & _ 'is added to improve the performance
        If CString(DB.GetScalar(SQL, custGrpItemID)) = "Y" Then Return True
        SQL = "SELECT jfcItem FROM custgrp_item WHERE custGrpItem_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, custGrpItemID)) = "Y", True, False)
    End Function

    Public Shared Function IsJfcScaledItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT p.scaledItem FROM jets_price p, jets_customer jc, customer c, " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " i " &
                    "WHERE jc.bid = c.jfcBranchNr AND jc.customerid = c.jfcCustomerNr " &
                    "  AND p.pricegpid = jc.pricegpid AND i.item_id = p.itemcode AND i.jfcItem = 'Y' " &
                    "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) &
                    "  AND i.custGrpItem_id = :1 AND c.customer_id = :2"
        '"  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
        Return IIf(CString(DB.GetScalar(SQL, custGrpItemID, customerID)) = "Y", True, False)
    End Function

    Public Shared Function GetShipdateOffset(DB As DBHelper, ByVal CustomerID As String) As Decimal
        Dim SQL As String = "Select SHIPDATEOFFSET from customer where customer_id = :1"
        Return Val(DB.GetScalar(SQL, CustomerID))
    End Function
    Public Shared Function SendOrderEmailToVendor(ByVal DB As DBHelper, ByVal webOrderID As String) As Boolean
        Dim SQL As String

        If IsJfcOrder(DB, webOrderID) = False Then
            'Send email for non JFC vendor
            Return True
        Else
            'Send email if flag is set for JFC
            SQL = "SELECT h.webOrder_id FROM sohdr h, custGrp_vendor c, jfcVendor v " &
                    "WHERE h.custGrpVendor_id = c.custGrpVendor_id AND c.vendor_id = v.vendor_id AND v.sendSoEmail = 'Y' AND h.webOrder_id = :1"
            Return IIf(CString(DB.GetScalar(SQL, webOrderID)) <> "", True, False)
        End If
    End Function

    Public Shared Function ApplyInCustomer(ByVal DB As DBHelper, ByVal customerID As String, ByVal columnName As String) As Boolean
        Dim SQL As String
        SQL = "SELECT " & columnName & " FROM customer WHERE customer_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, customerID)) = "Y", True, False)
    End Function

    'Used for DisallowPay, HideItemPage, HidePaymentPage, HidePrice
    Public Shared Function ApplyInEmailOrCustomer(ByVal DB As DBHelper, ByVal customerID As String, ByVal columnName As String, Optional ByVal email As String = "") As Boolean
        Dim SQL As String

        If email = String.Empty Then email = GetUserEmail(DB)

        SQL = "SELECT " & columnName & " FROM webUser_customer WHERE email = :1 AND customer_id = :2"
        If CString(DB.GetScalar(SQL, email, customerID)) = "Y" Then Return True

        SQL = "SELECT " & columnName & " FROM customer WHERE customer_id = :1"
        If CString(DB.GetScalar(SQL, customerID)) = "Y" Then Return True

        Return False
    End Function

    Public Shared Function AllowMixItemGrp(ByVal DB As DBHelper, ByVal custCompanyID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT allowMixItemGrp FROM custCompany WHERE custCompany_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, custCompanyID)) = "Y", True, False)
    End Function

    Public Shared Function AllowCustInputPayAmt(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        If IsMobile() = True Then
            Return False
        Else
            Return ApplyInCustomer(DB, customerID, "AllowCustInputPayAmt")
        End If
    End Function

    Public Shared Function AllowCustOrder(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim SQL As String
        Dim email As String = GetUserEmail(DB)

        If SUPPORT_EMAIL.Contains(GetUserEmail(DB)) Then Return True
        If ApplyInCustomer(DB, customerID, "AllowCustOrder") = False Then Return False
        SQL = "SELECT hideCustOrder FROM webUser_customer WHERE email = :1 AND customer_id = :2"
        If CString(DB.GetScalar(SQL, email, customerID)) = "Y" Then Return False

        Return True
    End Function

    Public Shared Function UseCustomerCtg(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim SQL As String
        SQL = "SELECT item_id FROM customer_ctg WHERE customer_id = " & DB.SqlEsc(customerID)
        Return IIf(CString(DB.GetScalar(SQL)) = "", False, True)
    End Function

    'Public Shared Function AllowEverydayOrder(ByVal DB As DBHelper, ByVal custCompanyID As String) As Boolean
    '    Dim SQL As String

    '    SQL = "SELECT allowEverydayOrder FROM custCompany WHERE custCompany_id = " & DB.SqlEsc(custCompanyID)
    '    Return IIf(CString(DB.GetScalar(SQL)) = "Y", True, False)
    'End Function

    Public Shared Sub SplitSCustGrpItemID(ByVal custGrpItemID As String, ByRef custGrpID As String, ByRef itemID As String)
        custGrpID = Mid(custGrpItemID, 2, 6)
        itemID = Mid(custGrpItemID, 8, 5)
    End Sub

    Public Shared Function GetColumnIndex(ByVal grd As GridView, ByVal colName As String) As Integer
        Dim index As String = -1
        For Each col As DataControlField In grd.Columns
            If TypeOf col Is System.Web.UI.WebControls.BoundField Then
                If CType(col, BoundField).DataField.ToUpper = colName.ToUpper Then
                    index = grd.Columns.IndexOf(col)
                    Exit For
                End If
            End If
        Next
        Return index
    End Function

    Public Shared Function GetVendorID(ByVal DB As DBHelper, ByVal custGrpID As String, ByVal vendorName As String) As String
        Dim SQL As String

        SQL = "SELECT vendor_id FROM custGrp_vendor WHERE custGrp_id = :1 AND vendorName = :2"
        Return CString(DB.GetScalar(SQL, custGrpID, vendorName))
    End Function

    Public Shared Function GetItemGrpID(ByVal DB As DBHelper, ByVal custGrpID As String, ByVal itemGrpName As String) As String
        Dim SQL As String

        SQL = "SELECT itemGrp_id FROM custGrp_itemGrp WHERE custGrp_id = :1 AND itemGrpName = :2"
        Return CString(DB.GetScalar(SQL, custGrpID, itemGrpName))
    End Function

    Public Shared Function GetCtgID(ByVal DB As DBHelper, ByVal custCompanyID As String, ByVal ctgName As String) As String
        Dim SQL As String

        SQL = "SELECT ctg_ID FROM custCompany_ctg WHERE custCompany_id = :1 AND ctgName = :2"
        Return CString(DB.GetScalar(SQL, custCompanyID, ctgName))
    End Function

    Public Shared Function GetCustUserType(ByVal DB As DBHelper, ByVal email As String, ByVal custCompanyID As String) As String
        Dim SQL As String
        Dim userType As String

        SQL = "SELECT customer_id FROM webuser_customer WHERE customer_id = 'ADMIN' AND email = :1 AND custCompany_id = :2"
        userType = CString(DB.GetScalar(SQL, email, custCompanyID))
        If userType = "ADMIN" Then
            Return "ADMIN"
        Else
            Return "CUSTOMER"
        End If
    End Function

    Public Shared Function GetOrderCustCompanyID(ByVal DB As DBHelper, ByVal webOrderID As String) As String
        Dim SQL As String

        SQL = "SELECT cg.custCompany_id FROM sohdr h, customer c, custGrp cg WHERE h.customer_id = c.customer_id AND c.custGrp_id = cg.custGrp_id AND h.webOrder_id = :1"
        Return CString(DB.GetScalar(SQL, webOrderID))
    End Function

    Public Shared Function GetCustCompanyType() As String
        Return IIf(HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYTYPE") <> "", HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYTYPE"), "Customer")
    End Function

    Public Shared Function GetCustWebDiscType(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String

        SQL = "SELECT webDiscType FROM customer WHERE customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    'Not used any more
    'Public Shared Function GetCustWebDiscRate(ByVal DB As DBHelper, ByVal customerID As String) As Decimal
    '    Dim SQL As String

    '    SQL = "SELECT webDiscRate FROM customer WHERE customer_id = :1"
    '    Return Common.Val(DB.GetScalar(SQL, customerID))
    'End Function

    Public Shared Function GetCustAutoPayType(ByVal DB As DBHelper, ByVal customerID As String, ByVal ColumnName As String) As String
        Dim SQL As String
        SQL = "SELECT " & ColumnName & " FROM customer WHERE customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    'Public Shared Function GetCustAutoPayType(ByVal DB As DBHelper, ByVal customerID As String) As String
    '    Dim SQL As String

    '    SQL = "SELECT autoPayType FROM customer WHERE customer_id = :1"
    '    Return CString(DB.GetScalar(SQL, customerID))
    'End Function

    'Public Shared Function GetCustAllowAutoPayType(ByVal DB As DBHelper, ByVal customerID As String) As String
    '    Dim SQL As String

    '    SQL = "SELECT allowAutoPayType FROM customer WHERE customer_id = :1"
    '    Return CString(DB.GetScalar(SQL, customerID))
    'End Function

    Public Shared Function GetCustAutoPayAccNickName(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String

        SQL = "SELECT accNickName FROM customer_payAcc WHERE customer_id = :1 AND usedForAutoPay = 'Y'"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    Public Shared Function GetCustGrpName(ByVal DB As DBHelper, ByVal custGrpID As String) As String
        Dim SQL As String

        SQL = "SELECT custGrpName FROM custGrp WHERE custGrp_id = :1"
        Return CString(DB.GetScalar(SQL, custGrpID))
    End Function

    Public Shared Function GetCustName(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String

        SQL = "SELECT customerName FROM customer WHERE customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    Public Shared Function GetVendorName(ByVal DB As DBHelper, ByVal custGrpVendorID As String) As String
        Dim SQL As String

        SQL = "SELECT vendorName FROM custGrp_vendor WHERE custGrpVendor_id = :1"
        Return CString(DB.GetScalar(SQL, custGrpVendorID))
    End Function

    Public Shared Function GetJFCVendorName(ByVal DB As DBHelper, ByVal vendorID As String) As String
        Dim SQL As String

        SQL = "SELECT vendorName FROM jfcVendor WHERE vendor_id = :1"
        Return CString(DB.GetScalar(SQL, vendorID))
    End Function

    Public Shared Function GetJfcBranchCustNr(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String

        SQL = "SELECT jfcBranchNr || '-' || jfcCustomerNr FROM customer WHERE customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    Public Shared Sub GetJFCBranchAndCustomerNr(ByVal DB As DBHelper, ByVal customerID As String, ByRef jfcBranchNr As String, ByRef jfcCustomerNr As String)
        Dim SQL As String
        Dim r As DataReader
        SQL = "SELECT jfcBranchNr, jfcCustomerNr FROM customer WHERE customer_id = :1"
        r = DB.GetReader(SQL, customerID)
        If r.Read() Then
            jfcBranchNr = CString(r("jfcBranchNr"))
            jfcCustomerNr = CString(r("jfcCustomerNr"))
        End If
        r.close()
    End Sub

    Public Shared Function GetCustomerID(ByVal DB As DBHelper, ByRef jfcBranchNr As String, ByRef jfcCustomerNr As String) As String
        Dim SQL As String
        Dim r As DataReader
        SQL = "SELECT customer_id FROM customer WHERE  jfcBranchNr = :1 and jfcCustomerNr = :2 "
        r = DB.GetReader(SQL, jfcBranchNr, jfcCustomerNr)
        If r.Read() Then
            Return CString(r("customer_id"))
        Else
            Return ""
        End If
        r.close()
    End Function
    Public Shared Function GetCtgName(ByVal DB As DBHelper, ByVal ctgID As String) As String
        Dim SQL As String

        SQL = "SELECT ctgName FROM custCompany_ctg WHERE ctg_id = :1"
        Return CString(DB.GetScalar(SQL, ctgID))
    End Function

    Public Shared Function GetItemGrpName(ByVal DB As DBHelper, ByVal itemGrpID As String) As String
        Dim SQL As String

        SQL = "SELECT itemGrpName FROM custGrp_itemGrp WHERE itemGrp_id = :1"
        Return CString(DB.GetScalar(SQL, itemGrpID))
    End Function

    Public Shared Function GetUserPasswd(ByVal DB As DBHelper, ByVal email As String) As String
        Dim SQL As String
        SQL = "SELECT password FROM webuser WHERE email = :1"
        Return CString(DB.GetScalar(SQL, email))
    End Function

    Public Shared Function GetWebUserCustParam(ByVal DB As DBHelper, ByVal paramName As String, ByVal email As String, ByVal customerID As String, ByVal valueIfEmpty As String) As String
        Dim SQL As String
        Dim value As String
        SQL = "SELECT paramValue FROM webUser_custParam " &
                "WHERE paramName = " & DB.SqlEsc(paramName) & " AND email = " & DB.SqlEsc(email) & " AND customer_id = " & DB.SqlEsc(customerID)
        value = CString(DB.GetScalar(SQL))
        Return IIf(value <> String.Empty, value, valueIfEmpty)
    End Function

    Public Shared Function GetItemID(ByVal DB As DBHelper, ByVal custGrpItemID As String) As String
        Dim SQL As String
        Dim itemID As String
        Dim custGrpID As String = ""

        SplitSCustGrpItemID(custGrpItemID, custGrpID, Nothing)
        SQL = "SELECT item_id FROM v_custgrp_item_small WHERE custgrp_id = " & DB.SqlEsc(custGrpID) & " AND custGrpItem_id = :1"
        itemID = CString(DB.GetScalar(SQL, custGrpItemID))
        '"custgrp_id = " & DB.SqlEsc(custGrpID) & _ 'is added to improve the performance
        If itemID <> String.Empty Then Return itemID
        SQL = "SELECT item_id FROM custgrp_item WHERE custGrpItem_id = :1"
        Return CString(DB.GetScalar(SQL, custGrpItemID))
    End Function

    Public Shared Function GetJfcEmail(ByVal DB As DBHelper, ByVal customerID As String, ByVal onlyCustJfcEmail As Boolean) As String
        Dim SQL As String
        Dim r As DataReader
        SQL = "SELECT c.jfcEmail AS custJfcEmail, CASE WHEN c.jfcEmail IS NULL THEN j.email ELSE c.jfcEmail END AS jfcEmail " &
                "FROM customer c, jfcVendor j " &
                "WHERE c.jfcBranchNr = j.jfcBranchNr AND c.customer_id = :1"
        r = DB.GetReader(SQL, customerID)
        If r.Read() Then
            Return IIf(onlyCustJfcEmail = True, CString(r("custJfcEmail")), CString(r("jfcEmail")))
        Else
            Return String.Empty
        End If
    End Function

    Public Shared Function GetCustomerEmailForPayment(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String
        SQL = "SELECT WM_CONCAT(w.email) " &
                "FROM webUser_customer wc, customer c, webUser w " &
                "WHERE wc.custCompany_id = wc.userCompany_id AND wc.customer_id = c.customer_id AND wc.email = w.email " &
                "  AND NVL(c.HidePaymentPage, ' ') <> 'Y' AND NVL(wc.HidePaymentPage, ' ') <> 'Y' AND c.customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    Public Shared Function GetOrderEmail(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal emailType As String) As String
        Dim SQL As String
        Dim r As DataReader
        Dim orderEmail As String = ""
        If emailType = "CUSTOMER" Then
            SQL = "SELECT email, modUser FROM so_email WHERE webOrder_id = :1 AND emailType = :2"
            r = DB.GetReader(SQL, webOrderID, emailType)
            If r.Read() Then
                orderEmail = CString(r("email"))
                ''Put created user and last modified user together in email address if they are different
                ''modUser could be a JFC user if order was created or updated by JFC Admin
                'orderEmail = IIf(CString(r("email")) = CString(r("modUser")), CString(r("email")), CString(r("email")) & "," & CString(r("modUser")))
            End If
            r.close()
        Else
            SQL = "SELECT email FROM so_email WHERE webOrder_id = :1 AND emailType = :2"
            orderEmail = CString(DB.GetScalar(SQL, webOrderID, emailType))
        End If
        Return orderEmail
    End Function

    Public Shared Function GetCustOrderNr(ByVal DB As DBHelper, ByVal webOrderID As String) As String
        Dim SQL As String
        SQL = "SELECT custOrderNr FROM sohdr WHERE webOrder_id = :1"
        Return CString(DB.GetScalar(SQL, webOrderID))
    End Function

    Public Shared Function GetOrderCustName(ByVal DB As DBHelper, ByVal webOrderID As String) As String
        Dim SQL As String

        SQL = "SELECT c.customerName FROM sohdr o, customer c WHERE o.customer_id = c.customer_id AND o.weborder_id = :1"
        Return CString(DB.GetScalar(SQL, webOrderID))
    End Function

    Public Shared Function GetOrderState(ByVal DB As DBHelper, ByVal webOrderID As String) As String
        Dim SQL As String
        SQL = "SELECT orderState FROM sohdr WHERE webOrder_id = :1"
        Return CString(DB.GetScalar(SQL, webOrderID))
    End Function

    'Public Shared Function GetJetsOrderTotalAmt(ByVal DB As DBHelper, ByVal webOrderID As String) As Decimal
    '    Dim SQL As String
    '    SQL = "SELECT SUM(totalAmt) FROM sohdr_jets WHERE webOrder_id = " & DB.SqlEsc(webOrderID)
    '    Return Val(DB.GetScalar(SQL))
    'End Function

    Public Shared Function GetCurrentBalance(ByVal DB As DBHelper, ByVal customerID As String) As Decimal
        Dim SQL As String
        SQL = "SELECT SUM(arAmt - recvAmt - discAmt) FROM customer_jfcAR WHERE customer_id = " & DB.SqlEsc(customerID)
        Return Val(DB.GetScalar(SQL))
        'Return Val(DB.GetScalar("SELECT balance FROM (select balance from customer_JfcTransaction " & _
        '                     "WHERE customer_id = :1 order by seq desc) WHERE rownum = 1", customerID))
    End Function

    Public Shared Function GetPendingAmt(ByVal DB As DBHelper, ByVal customerID As String) As Decimal
        Return Val(DB.GetScalar("SELECT SUM(approvedAmt) FROM customer_payHdr WHERE customer_id = :1 AND paymentStatus_id IN ('P', 'F')", customerID))
    End Function

    Public Shared Function GetReturnedCheckFee(ByVal DB As DBHelper, ByVal jfcBranchNr As String) As String
        Dim fee As String = GetCodeMaster(DB, "RETURNED_CHECK_FEE", jfcBranchNr)
        Return IIf(fee = "", DEFAULT_RETURNED_CHECK_FEE, fee)
    End Function

    Public Shared Function GetPriceGrpID(ByVal DB As DBHelper, ByVal jfcBranchNr As String, ByVal jfcCustomerNr As String) As String
        Dim SQL As String

        SQL = "SELECT priceGpID FROM jets_customer cp WHERE bid = :1 AND customerid = :2"
        Return CString(DB.GetScalar(SQL, jfcBranchNr, jfcCustomerNr))
    End Function

    Public Shared Function GetPriceGrpDesc(ByVal DB As DBHelper, ByVal priceGrpID As String) As String
        Dim SQL As String

        SQL = "SELECT priceGrpDesc FROM jfcPriceGrp WHERE priceGrp_id = :1"
        Return CString(DB.GetScalar(SQL, priceGrpID))
    End Function

    Public Shared Sub UpdateDeliveryCelendar(ByVal DB As DBHelper, ByVal year As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim r As DataReader
        Dim firstDayOfYear As Date = New DateTime(year, 1, 1)
        Dim lastDayOfYear As Date = New DateTime(year + 1, 1, 1).AddDays(-1)

        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "select m.*, max(d.deliverydate)as lastdeliverydate from jfcdeliverymaster m,jfcdeliverymaster_date d where m.deliverymaster_id = d.deliverymaster_id  " &
               "group by m.deliverymaster_id,m.jfcbranchnr,m.deliverymastername," &
               "m.frequency,m.deliveryday,m.orderbyday,m.orderbytime,m.createuser,m.createdate,m.moduser,m.moddate"
        r = DB.GetReader(SQL)
        While r.Read()
            '
            UpSertMasterDelivery(DB, CString(r("DELIVERYMASTER_ID")), CString(r("JFCBRANCHNR")), CString(r("DELIVERYMASTERNAME")), CString(r("frequency")), r("lastdeliverydate"),
                                 lastDayOfYear, CString(r("deliveryDay")), CString(r("orderByDay")), CString(r("orderByTime")),
                                 year, False, True)
        End While
        r.close()

        'Update delivery date in the specified year for delivery schedule with empty ending date
        SQL = "SELECT * FROM customer_deliverySched WHERE endDate IS NULL"
        r = DB.GetReader(SQL)
        While r.Read()
            'UpSertMasterDelivery(DB, , , )
            UpSertDeliverySched(DB, CString(r("deliverySched_id")), Val(r("deliverySchedLineNr")), CString(r("customer_id")), CString(r("custGrpVendor_id")), CString(r("shipPoint_id")),
                                CString(r("deliveryMaster_id")), CString(r("frequency")),
                                firstDayOfYear, lastDayOfYear, CString(r("deliveryDay")), CString(r("orderByDay")), CString(r("orderByTime")), CString(r("sendReminder")),
                                year, False, True)
        End While
        r.close()

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Function CreateCustGrpID(ByVal DB As DBHelper, ByVal custCompanyID As String) As String
        Dim SQL As String

        SQL = "SELECT SUBSTR(MAX(custCompany_id), 1, 4) || LPAD(TO_CHAR(TO_NUMBER(MAX(SUBSTR(custGrp_id, 5, 2))) + 1) , 2, '0') FROM custGrp WHERE custCompany_id = :1"
        Return CString(DB.GetScalar(SQL, custCompanyID))
    End Function

    Public Shared Sub CreateEmailBody(ByVal page As Page, ByRef emailBody As String, ByVal cmd As String, ByVal ParamArray args() As Object)

        Dim info As CultureInfo = Thread.CurrentThread.CurrentCulture
        If Not info.NumberFormat.IsReadOnly Then
            Dim numberInfo As NumberFormatInfo = CultureInfo.CreateSpecificCulture("en-US").NumberFormat
            info.NumberFormat = numberInfo
            info.DateTimeFormat = CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat
            Thread.CurrentThread.CurrentUICulture = info
            Thread.CurrentThread.CurrentCulture = info
        End If

        'Set session
        HttpContext.Current.Session("EMAILBODY_CMD") = cmd
        For i As Integer = 0 To args.Length - 1

            HttpContext.Current.Session("EMAILBODY_ARG" & i + 1) = args(i)
            'HttpContext.Current.Session("EMAILBODY_ARG2") = arg2
            'HttpContext.Current.Session("EMAILBODY_ARG3") = arg3
            'HttpContext.Current.Session("EMAILBODY_ARG4") = arg4
        Next


        'create email body
        Dim ctrl As Control = page.LoadControl("~/Common/EmailBody.ascx")
        page.Controls.Add(ctrl)
        Dim writer As System.IO.StringWriter = New System.IO.StringWriter()
        Dim hWriter As HtmlTextWriter = New HtmlTextWriter(writer)
        ctrl.RenderControl(hWriter)
        emailBody = writer.ToString()
        'Remove control so that it won't be shown on page
        page.Controls.Remove(ctrl)
    End Sub

    Public Shared Function IsSmallCustomer(Optional ByVal DB As DBHelper = Nothing, Optional ByVal customerID As String = "", Optional ByVal custGrpID As String = "") As Boolean
        Dim SQL As String
        Dim custCompanyTypeID As String

        'Return False 'For debug

        If DB Is Nothing Then
            Return IIf(HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYTYPEID") = "R" OrElse HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYTYPEID") = "S", True, False)
        ElseIf customerID <> String.Empty Then
            SQL = "SELECT cc.type_id FROM customer c, custGrp g, custCompany cc WHERE c.custGrp_id = g.custGrp_id AND g.custCompany_id = cc.custCompany_id AND c.customer_id = :1"
            custCompanyTypeID = CString(DB.GetScalar(SQL, customerID))
            Return IIf(custCompanyTypeID = "R" OrElse custCompanyTypeID = "S", True, False)
        Else
            SQL = "SELECT cc.type_id FROM custGrp g, custCompany cc WHERE g.custCompany_id = cc.custCompany_id AND g.custGrp_id = :1"
            custCompanyTypeID = CString(DB.GetScalar(SQL, custGrpID))
            Return IIf(custCompanyTypeID = "R" OrElse custCompanyTypeID = "S", True, False)
        End If
    End Function

    Public Shared Function IsExcepPriceCustomer(Optional ByVal DB As DBHelper = Nothing, Optional ByVal customerID As String = "") As Boolean
        Dim SQL As String

        'Return False 'For debug

        If DB Is Nothing Then
            Return IIf(HttpContext.Current.Session("SYS_SESSION_ISEXCEPPRICECUST") = "N", False, True)
        ElseIf customerID <> String.Empty Then
            SQL = "SELECT jfcPriceGrpID_orig FROM customer WHERE customer_id = :1"
            Return IIf(Right(CString(DB.GetScalar(SQL, customerID)), 4) = "-REG", False, True)
        Else
            Return False
        End If
    End Function

    Public Shared Function PdfStatement(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT PdfStatement FROM customer WHERE customer_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, customerID)) = "Y", True, False)
    End Function

    Public Shared Function EmailInvoice(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT invoiceEmail FROM customer WHERE customer_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, customerID)) <> "", True, False)
    End Function

    Public Shared Function ShowItemDtl(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim SQL As String

        SQL = "SELECT showItemDtl FROM customer WHERE customer_id = :1"
        Return IIf(CString(DB.GetScalar(SQL, customerID)) = "Y", True, False) 'OrElse IsDev() = True
    End Function

    Public Shared Function HideEaQty(ByVal DB As DBHelper, ByVal customerID As String) As Boolean
        Dim jfcBranchNr As String = ""
        'Dim SQL As String
        'SQL = "SELECT hideEaQty FROM customer WHERE customer_id = :1" 'TODO: Later store HideEaQty in customer table
        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, Nothing)
        Return IIf(jfcBranchNr = "05", True, False)
    End Function

    Public Shared Function GetCustCompanyTypeID(ByVal DB As DBHelper, ByVal custCompanyID As String) As String
        Dim SQL As String

        SQL = "SELECT type_id FROM custCompany WHERE custCompany_id = :1"
        Return CString(DB.GetScalar(SQL, custCompanyID))
    End Function

    Public Shared Function GetCustCompanyID(ByVal DB As DBHelper, ByVal custGrpID As String) As String
        Dim SQL As String

        SQL = "SELECT custGrp_id FROM custGrp WHERE custGrp_id = :1"
        Return CString(DB.GetScalar(SQL, custGrpID))
    End Function

    Public Shared Function GetCustGrpID(ByVal DB As DBHelper, ByVal customerID As String) As String
        Dim SQL As String

        SQL = "SELECT custGrp_id FROM customer WHERE customer_id = :1"
        Return CString(DB.GetScalar(SQL, customerID))
    End Function

    Public Shared Function GetCustGrpVendorID(ByVal DB As DBHelper, ByVal custGrpID As String, ByVal vendorID As String) As String
        Dim SQL As String
        SQL = "SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = :1 AND vendor_id = :2"
        Return CString(DB.GetScalar(SQL, custGrpID, vendorID))
    End Function

    Public Shared Function GetCustGrpItemID(ByVal DB As DBHelper, ByVal custGrpID As String, ByVal vendorID As String, ByVal itemGrpID As String, ByVal itemID As String) As String
        Dim SQL As String

        SQL = "SELECT custGrpItem_id FROM " & IIf(IsSmallCustomer(DB, Nothing, custGrpID) = True, "v_custgrp_item_small", "custgrp_item") & " WHERE custGrp_id = :1" &
                    " AND vendor_id = :2 AND itemGrp_id = :3 AND item_id = :4"
        Return CString(DB.GetScalar(SQL, custGrpID, vendorID, itemGrpID, itemID))
    End Function

    'Public Shared Function GetCustGrpItemID(ByVal DB As DBHelper, ByVal customerID As String, itemID As String) As String
    '    Dim SQL As String
    '    Dim custGrpId As String = GetCustGrpID(DB, customerID)
    '    SQL = "SELECT custGrpItem_id FROM " & IIf(IsSmallCustomer(DB, Nothing, custGrpId), "v_custgrp_item_small", "custgrp_item") & " WHERE custGrp_id = :1" & _
    '                "  AND item_id = :2"
    '    Return CString(DB.GetScalar(SQL, custGrpId, itemID))
    'End Function

    Public Shared Function GetCartID(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String) As String
        Dim SQL As String
        SQL = "SELECT cart_id FROM sohdr_cart WHERE createUser = :1 AND customer_id = :2 AND cartState = 'W'"
        Return CString(DB.GetScalar(SQL, email, customerID))
    End Function

    Public Shared Function GetNrCustGrpVendor(ByVal DB As DBHelper, ByVal custGrpID As String, Optional ByRef custGrpVendorID As String = "") As Integer
        Dim SQL As String
        Dim nrCustGrpVendor As Integer

        SQL = "SELECT COUNT(*) FROM custGrp_vendor WHERE custGrp_id = " & DB.SqlEsc(custGrpID)
        nrCustGrpVendor = Val(DB.GetScalar(SQL))
        If nrCustGrpVendor = 1 Then custGrpVendorID = CString(DB.GetScalar("SELECT custGrpVendor_id FROM custGrp_vendor WHERE custGrp_id = " & DB.SqlEsc(custGrpID)))
        Return nrCustGrpVendor
    End Function

    Public Shared Function GetNrCustGrp(ByVal DB As DBHelper, ByVal custCompanyID As String) As Integer
        Dim SQL As String
        SQL = "SELECT COUNT(*) FROM custGrp WHERE custCompany_id = " & DB.SqlEsc(custCompanyID)
        Return Val(DB.GetScalar(SQL))
    End Function

    Public Shared Function GetNrUserType(ByVal DB As DBHelper, ByVal email As String) As Integer
        Dim SQL As String
        Dim nUserTypeCust, nUserTypeVendor As Integer

        SQL = "SELECT COUNT(*) FROM webuser_customer WHERE customer_id <> 'NEW' AND email = :1"
        nUserTypeCust = Val(DB.GetScalar(SQL, email))
        SQL = "SELECT COUNT(*) FROM webuser_vendor WHERE email = :1"
        nUserTypeVendor = Val(DB.GetScalar(SQL, email))
        Return nUserTypeCust + nUserTypeVendor
    End Function

    Public Shared Function GetNrCustShipPoint(ByVal DB As DBHelper, ByVal customerID As String) As Integer
        Dim SQL As String
        SQL = "SELECT COUNT(*) FROM customer_shipPoint WHERE customer_id = " & DB.SqlEsc(customerID)
        Return Val(DB.GetScalar(SQL))
    End Function

    Public Shared Function GetNrItemInCart(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String) As Integer
        Dim SQL As String
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""

        'Get JFC #
        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
        'Get number of items in cart
        SQL = "SELECT COUNT(*) " &
                    "FROM sohdr_cart h, sodtl_cart d, " & IIf(IsSmallCustomer(DB, customerID) = True, IIf(IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " &
                    "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = :1 AND cp.customerid = :2 " &
                    "WHERE h.cart_id = d.cart_id AND h.createUser = :3 AND h.customer_id = :4 AND h.cartState = 'W'" &
                    "  AND d.custGrpItem_id = i.custGrpItem_id " &
                    "  AND i.custGrpItem_id = ci.custGrpItem_id AND ci.customer_id = h.customer_id " &
                    "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) &
                    "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') "
        '"  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
        '"  AND (p.outOfStock IS NULL OR p.outOfStock = 'N') " & _
        Return Val(DB.GetScalar(SQL, jfcBranchNr, jfcCustomerNr, email, customerID))
    End Function

    Public Shared Function GetNrItemInOrder(ByVal DB As DBHelper, ByVal webOrderID As String) As Integer
        Dim SQL As String
        SQL = "SELECT COUNT(*) FROM sodtl WHERE webOrder_id = :1"
        Return Val(DB.GetScalar(SQL, webOrderID))
    End Function

    Public Shared Function GetNrPriceInquiry(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String) As Integer
        Dim SQL As String
        SQL = "SELECT COUNT(*) FROM webUser_priceInquiry p, " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " i " &
                    "WHERE p.sent = 'N' AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & " AND p.email = :1 AND p.customer_id = :2 AND p.custGrpItem_id = i.custGrpItem_id" 'In case item gets removed
        '"  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
        Return Val(DB.GetScalar(SQL, email, customerID))
    End Function

    Public Shared Function GetNrItemGrpInCustGrp(ByVal DB As DBHelper, ByVal custGrpID As String) As Integer
        Dim SQL As String
        SQL = "SELECT COUNT(*) FROM custGrp_itemGrp WHERE custGrp_id = :1"
        Return Val(DB.GetScalar(SQL, custGrpID))
    End Function

    Public Shared Function GetJfcLbsPerEa(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String) As Decimal
        Dim SQL As String
        Dim lbsPerEa As Decimal
        'Dim r As DataReader

        SQL = "SELECT p.lbsPerEa " &
                "FROM jets_price p, jets_customer jc, customer c " &
                "WHERE jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " &
                "  AND p.pricegpid = jc.pricegpid AND p.itemcode = " & DB.SqlEsc(GetItemID(DB, custGrpItemID)) & " AND c.customer_id = " & DB.SqlEsc(customerID)
        lbsPerEa = Val(DB.GetScalar(SQL))
        Return IIf(lbsPerEa = 0, 0.1, lbsPerEa)
        'r = DB.GetReader(SQL)
        'If r.Read() Then
        '    'avgCsWgt = Val(r("avgCsWgt"))
        '    'avgEaWgt = Val(r("avgEaWgt"))
        '    lbsPerEa = Val(r("lbsPerEa"))
        'End If
        'r.close()
    End Function

    Public Shared Function GetAvgWgt(ByVal price As Decimal, ByVal lbPriceOrderByQty As Decimal) As Decimal
        If lbPriceOrderByQty = 0 Then
            Return 0
        Else
            Return Math.Round(price / lbPriceOrderByQty, 2)
        End If
    End Function

    'Public Shared Function GetSQL_CustGrpList(ByVal custCompanyID As String) As String
    '    Dim SQL As String
    '    'SQL = "SELECT c.custGrp_id, c.custGrpName FROM webuser_customer u, custGrp c WHERE u.custCompany_id = c.custCompany_id " & _
    '    '        " AND u.customer_id = 'ADMIN' AND u.email = " & DB.SqlEsc(GetUserEmail()) & " ORDER BY c.custGrpName"
    '    SQL = "SELECT custGrp_id, custGrpName FROM custGrp WHERE custCompany_id = '" & custCompanyID & "' ORDER BY custGrpName"
    '    Return SQL
    'End Function

    Public Shared Function GetSQL_itemDtl(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, Optional ByVal cartID As String = "", Optional ByVal webOrderID As String = "") As String
        Dim SQL As String
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim smallCustomer As Boolean = IsSmallCustomer(DB, customerID)

        'Get JFC #
        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)

        'Don't get eaCost and gp here 
        SQL = "SELECT i.custGrpItem_id, i.custGrp_ID, i.vendor_id, i.itemGrp_id, g.itemGrpName, i.item_id, " & IIf(smallCustomer = True, "cc.custItemCode", "i.itemNr") & " AS itemNr, cc.CustItemDesc, " &
                      " v.vendorName, i.minSalCnt, TRIM(i.jfcRemark || ' ' || cc.custRemark) AS jfcRemark, i.jfcitem, " &
                      " ci.eaPerCs, ci.csPrice, ci.eaPrice, ci.lbPrice_OrderByWgt, ci.lbPrice_OrderByQty, p.outOfStock, TO_CHAR(p.arrivDate, " & DB.SqlEsc(SHORTDATE_FMT) & ") AS arrivDate_fmt, " &
                      " q.csQty AS lastCsQty, CASE WHEN NVL(q.pcQty, 0) > 0 THEN q.pcQty ELSE q.eaQty END AS lastEaQty, q.lbs AS lastLbs, " &
                      " CASE WHEN q.custGrpItem_id IS NOT NULL OR cp.regcsprice > 0 THEN 'Y' ELSE 'N' END ordered, " &
                      " NVL(p.csAvail, 0) || ' cs ' || NVL(p.eaAvail, 0) || ' ea available' AS qtyAvail, " &
                      " NVL(p.csCost, 0) AS csCost, " &
                      " NVL(p.BTMCSPRICE,0) AS btmcsPrice, " &
                      " NVL(vc.item_id,'') AS promoItemId, " &
                      " CASE WHEN ci.csPrice = 0 THEN 1 ELSE ROUND((ci.csPrice - NVL(p.csCost, 0)) / ci.csPrice, 2) END gp, " &
                      " nvl(p.avgcswgt, 0) avgcswgt, nvl(p.avgeawgt, 0) avgeawgt, " &
                      " TO_CHAR(ROUND(NVL(a.grossWeight, 0), 2), 99999.99) AS grossWeight_lbs, TO_CHAR(ROUND(NVL(a.grossWeight, 0) / 2.2046, 2), 99999.99) AS grossWeight_kg, " &
                      " TO_CHAR(ROUND(NVL(a.caseWidth * a.caseDepth * a.caseHight, 0) / 1728, 3), 99999.999) AS caseVol_ft3, TO_CHAR(ROUND(ROUND(NVL(a.caseWidth, 0) * NVL(a.caseDepth, 0) * NVL(a.caseHight, 0) / 12 / 12 / 12, 8) / 35.314, 3), 99999.999) AS caseVol_m3, " &
                      " NVL(p.block, 0) AS block, NVL(p.high, 0) AS high, NVL(p.ingredient_branch, a.ingredient) ingredient, " &
                      " CASE WHEN i.jfcitem = 'Y' THEN 'JFCItems' ELSE 'OtherItems' END itemType, " &
                      " CASE WHEN " & IIf(smallCustomer = True, "cc.custItemCode", "i.itemNr") & " IS NULL THEN i.item_id ELSE " & IIf(smallCustomer = True, "cc.custItemCode", "i.itemNr") & " || ' (JFC item #' || i.item_id || ')' END AS itemIdNr_view, " &
                      " CASE WHEN i.jfcitem = 'Y' AND cc.custItemDesc IS NULL THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE cc.custItemDesc || ' (' || NVL(p.itemDesc_branch, a.itemDesc) || ')' END AS itemDesc_view, " &
                      " jctgGrp.ctgGrpDesc AS ctgName_view, jctgGrp.ctgGrpDesc AS jfcCtgName, " &
                      " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " &
                      IIf(cartID <> "", " CASE WHEN cd.csQty IS NOT NULL THEN cd.csQty END AS cartCsQty, ", "'' cartCsQty, ") &
                      IIf(cartID <> "", " CASE WHEN cd.eaQty IS NOT NULL THEN cd.eaQty END AS cartEaQty, ", "'' cartEaQty, ") &
                      IIf(cartID <> "", " CASE WHEN cd.lbs IS NOT NULL THEN cd.lbs END AS cartLbs, ", "'' cartLbs, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.csQty IS NOT NULL THEN od.csQty END AS orderCsQty, ", "'' orderCsQty, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.eaQty IS NOT NULL THEN od.eaQty END AS orderEaQty, ", "'' orderEaQty, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.lbs IS NOT NULL THEN od.lbs END AS orderLbs, ", "'' orderLbs, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.csQty_vp IS NOT NULL THEN od.csQty_vp END AS orderCsQty_vp, ", "'' orderCsQty_vp, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.eaQty_vp IS NOT NULL THEN od.eaQty_vp END AS orderEaQty_vp, ", "'' orderEaQty_vp, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.csQty_sp IS NOT NULL THEN od.csQty_sp END AS orderCsQty_sp, ", "'' orderCsQty_sp, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.eaQty_sp IS NOT NULL THEN od.eaQty_sp END AS orderEaQty_sp, ", "'' orderEaQty_sp, ") &
                      IIf(webOrderID <> "", " CASE WHEN od.vpMemo IS NOT NULL THEN od.vpMemo END AS orderVpMemo ", "'' orderVpMemo ") &
                      "FROM custGrp_itemGrp g, custGrp_vendor v, " & IIf(smallCustomer = True, IIf(IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " &
                      "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " &
                      "LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = GetCtgGrpID(" & DB.SqlEsc(customerID) & ", i.item_id, 'N') " &
                      "LEFT OUTER JOIN jets_customer_price cp ON i.jfcItem = 'Y' AND cp.itemcode = i.item_id AND cp.bid = " & DB.SqlEsc(jfcBranchNr) & " AND cp.customerid = " & DB.SqlEsc(jfcCustomerNr) & " " &
                      "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)) & " " &
                      "LEFT OUTER JOIN customer_lastOrderedQty q ON q.custGrpItem_id = i.custGrpItem_id AND q.customer_id = " & DB.SqlEsc(customerID) & " " &
                      "LEFT OUTER JOIN customer_custItem cc ON cc.custGrpItem_id = i.custGrpItem_id AND cc.customer_id = " & DB.SqlEsc(customerID) & " " &
                      "LEFT OUTER JOIN v_customer_promoitem vc ON vc.item_id = i.item_id AND (vc.customer_id = " & DB.SqlEsc(customerID) & " or vc.customer_id is null) and vc.jfcbranchnr = " & DB.SqlEsc(jfcBranchNr) & " " &
                      IIf(cartID <> "", "LEFT OUTER JOIN sodtl_cart cd ON cd.custGrpItem_id = i.custGrpItem_id AND cd.cart_id = " & DB.SqlEsc(cartID) & " ", "") &
                      IIf(webOrderID <> "", "LEFT OUTER JOIN sodtl od ON od.custGrpItem_id = i.custGrpItem_id AND od.webOrder_id = " & DB.SqlEsc(webOrderID) & " ", "") &
                      "WHERE i.custGrp_id = g.custGrp_id AND i.itemGrp_id = g.itemGrp_id " &
                      "  AND i.custGrp_id = v.custGrp_id AND i.vendor_id = v.vendor_id " &
                      "  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) &
                      "  AND i.custGrpItem_id = ci.custGrpItem_id " &
                      "  AND (cp.enabled IS NULL OR cp.enabled = 'Y') " &
                      "  AND (cc.enabled IS NULL OR cc.enabled = 'Y') " &
                      "  AND ci.customer_id = " & DB.SqlEsc(customerID) & " AND ci.custGrpItem_id = " & DB.SqlEsc(custGrpItemID) & " "
        '" CASE WHEN i.jfcitem = 'Y' THEN jctgGrp.ctgGrpDesc ELSE NULL END jfcCtgName, " & _
        '" CASE WHEN i.jfcitem = 'Y' AND c.ctgName IS NULL THEN jctgGrp.ctgGrpDesc ELSE c.ctgName END ctgName_view, " & _
        '"LEFT OUTER JOIN jfcSubCtg jSubCtg ON jSubCtg.ctg_id = a.ctg_id AND jSubCtg.subCtg_id = a.subCtg_id " & _
        '"LEFT OUTER JOIN jfcCtgGrp jctgGrp ON jctgGrp.ctgGrp_id = jSubCtg.ctgGrp_id " & _
        '"LEFT OUTER JOIN custCompany_ctg c ON c.ctg_id = i.ctg_id " & _
        '"LEFT OUTER JOIN jfcCtg jctg ON jctg.ctg_id = a.ctg_id " & _
        '"  AND g.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
        '" CASE WHEN ((SELECT DISTINCT(d.custGrpItem_id) FROM sodtl d WHERE d.custGrpItem_id = i.custGrpItem_id AND d.webOrder_id IN (SELECT h.webOrder_id FROM sohdr h WHERE h.customer_id = ci.customer_id)) IS NOT NULL) THEN 'Y' ELSE 'N' END webOrdered, " & _
        '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.csPrice FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.csPrice END AS csPrice, " & _
        '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.eaPrice FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.eaPrice END AS eaPrice, " & _
        '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.lbPrice_OrderByWgt FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.lbPrice_OrderByWgt END AS lbPrice_OrderByWgt, " & _
        '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.lbPrice_OrderByQty FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.lbPrice_OrderByQty END AS lbPrice_OrderByQty, " & _
        '" NVL(p.csCost, 0) AS csCost, CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs, 2) END eaCost, " & _
        '" CASE WHEN ci.csPrice = 0 THEN 1 ELSE ROUND((ci.csPrice - NVL(p.csCost, 0)) / ci.csPrice, 2) END gp, " & _
        Return SQL
    End Function

    Public Shared Function ExistCardNr(ByVal DB As DBHelper, ByVal customerID As String, ByVal cardNr As String) As Boolean
        Dim SQL As String = "SELECT cardNr FROM customer_payAcc WHERE customer_id = :1 AND cardNr = :2"
        Return IIf(CString(DB.GetScalar(SQL, customerID, Encrypt256(cardNr))) = "", False, True)
    End Function

    Public Shared Function ExistBankAccNr(ByVal DB As DBHelper, ByVal customerID As String, ByVal bankAccNr As String) As Boolean
        Dim SQL As String = "SELECT bankAccNr FROM customer_payAcc WHERE customer_id = :1 AND bankAccNr = :2"
        Return IIf(CString(DB.GetScalar(SQL, customerID, Encrypt256(bankAccNr))) = "", False, True)
    End Function

    Public Shared Function ExistDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String) As Boolean
        Dim SQL As String
        SQL = "SELECT customer_id FROM customer_deliveryDate WHERE customer_id = :1 AND custGrpVendor_id = :2" &
                            " AND shipPoint_id = :3 AND deliveryDate = TO_DATE(:4, :5)"
        Return IIf(CString(DB.GetScalar(SQL, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT)) = "", False, True)
    End Function

    Public Shared Function ExistOrderOnDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String) As Boolean
        Dim SQL As String
        Dim isVendorJfc As Boolean
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""

        SQL = "SELECT vendorCompany_id FROM custGrp_vendor WHERE custGrpVendor_id = :1"
        isVendorJfc = IIf(CString(DB.GetScalar(SQL, custGrpVendorID)) = "JFC", True, False)
        SQL = "SELECT webOrder_id FROM sohdr WHERE customer_id = :1 AND custGrpVendor_id = :2" &
                            " AND shipPoint_id = :3 AND deliveryDate = TO_DATE(:4, :5) AND orderState <> 'D'"
        If isVendorJfc = True Then
            GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
            SQL &= " UNION SELECT orderNr FROM sohdr_jets WHERE jfcBranchNr = :6 AND jfcCustomerNr = :7 AND deliveryDate = TO_DATE(:8, :9)"
        End If
        Return IIf(CString(DB.GetScalar(SQL, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT, jfcBranchNr, jfcCustomerNr, deliveryDate, DATE_FMT)) = "", False, True)
    End Function

    Public Shared Function SendOrderReminder(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String) As Boolean
        Dim SQL As String
        SQL = "SELECT customer_id FROM customer_deliveryDate WHERE customer_id = :1 AND custGrpVendor_id = :2" &
                            " AND shipPoint_id = :3 AND deliveryDate = TO_DATE(:4, :5) AND sendReminder = 'Y'"
        Return IIf(CString(DB.GetScalar(SQL, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT)) = "", False, True)
    End Function

    Public Shared Function GetNextDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal dateFmt As String, Optional ByVal hoursLater As Integer = 0) As String
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)
        Dim jfcBranchNr As String = ""

        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, Nothing)
        'If userEmail = String.Empty Then
        SQL = "SELECT TO_CHAR(MIN(deliveryDate), :1) FROM customer_deliveryDate " &
                    "WHERE customer_id = :2 AND custGrpVendor_id = :3 AND shipPoint_id = :4 AND orderByDate > SYSDATE + " & GetServerDayOffset(DB, jfcBranchNr) & " + " & hoursLater / 24
        Return CString(DB.GetScalar(SQL, dateFmt, customerID, custGrpVendorID, shipPointID))
    End Function

    Public Shared Function GetPrevDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal dateFmt As String) As String
        Dim SQL As String
        Dim jfcBranchNr As String = ""

        GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, Nothing)
        SQL = "SELECT TO_CHAR(MAX(deliveryDate), :1) FROM customer_deliveryDate " &
                        "WHERE customer_id = :2 AND custGrpVendor_id = :3 AND shipPoint_id = :4 AND orderByDate < SYSDATE + " & GetServerDayOffset(DB, jfcBranchNr)
        Return CString(DB.GetScalar(SQL, dateFmt, customerID, custGrpVendorID, shipPointID))
    End Function

    Public Shared Function GetOrderByDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String, ByVal dateTimeFmt As String) As String
        Dim SQL As String
        SQL = "SELECT TO_CHAR(orderByDate, :1) FROM customer_deliveryDate WHERE customer_id = :2 AND custGrpVendor_id = :3" &
                            " AND shipPoint_id = :4 AND deliveryDate = TO_DATE(:5, :6)"
        Return CString(DB.GetScalar(SQL, dateTimeFmt, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT))
    End Function

    'Public Shared Function GetDeliveryDateFromOrderByDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal orderByDate As String, ByVal dateTimeFmt As String) As String
    '    Dim SQL As String
    '    SQL = "SELECT TO_CHAR(deliveryDate, :1) FROM customer_deliveryDate WHERE customer_id = :2 AND custGrpVendor_id = :3" & _
    '                        " AND shipPoint_id = :4 AND orderByDate = TO_DATE(:5, :6)"
    '    Return CString(DB.GetScalar(SQL, dateTimeFmt, customerID, custGrpVendorID, shipPointID, orderByDate, DATE_FMT))
    'End Function

    Public Shared Function GetNextDeliveryInfo(ByVal DB As DBHelper, ByVal customerID As String, ByVal shipPointID As String, Optional ByVal showOrderByDate As Boolean = True) As String
        Dim SQL As String
        Dim r As DataReader
        Dim deliveryDate, deliveryDateInDateFmt, orderByDate, orderByTime As String
        Dim orderByDateTime As String = ""
        Dim i As Integer
        Dim info As String = ""
        Dim noDateStr As String = IIf(ApplyInCustomer(DB, customerID, "AnytimeDelivery") = True, "Earliest delivery date", "N/A")
        Dim mobile As Boolean = IsMobile()
        Dim custGrpID As String = GetCustGrpID(DB, customerID)
        Dim jfcBranchNr As String = ""

        'If shipPointID is empty, use the smallest shipPointID for the customer
        If shipPointID = String.Empty Then
            SQL = "SELECT shipPoint_ID FROM customer_shipPoint WHERE customer_id = " & DB.SqlEsc(customerID) & " ORDER BY shipPoint_id"
            shipPointID = CString(DB.GetScalar(SQL))
        End If

        SQL = "SELECT custGrpVendor_id, vendorName FROM custGrp_vendor " &
             "WHERE custGrp_id = " & DB.SqlEsc(custGrpID) &
             "ORDER BY vendorName"
        r = DB.GetReader(SQL)
        'info = "Next Delivery" & IIf(mobile, "<br />", " - ")
        i = 0
        While r.Read
            deliveryDate = GetNextDeliveryDate(DB, customerID, CString(r("custGrpVendor_id")), shipPointID, IIf(mobile, DAYDATE_SQLFMT, DAYDATE_SQLFMT))
            deliveryDateInDateFmt = GetNextDeliveryDate(DB, customerID, CString(r("custGrpVendor_id")), shipPointID, DATE_FMT)
            orderByDate = GetOrderByDate(DB, customerID, CString(r("custGrpVendor_id")), shipPointID, deliveryDateInDateFmt, IIf(mobile, DAYDATE_SQLFMT, DAYDATE_SQLFMT))
            orderByTime = GetOrderByDate(DB, customerID, CString(r("custGrpVendor_id")), shipPointID, deliveryDateInDateFmt, TIME_SQLFMT)
            If showOrderByDate = True AndAlso orderByDate <> "" Then
                GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, Nothing)
                If IsToday(DB, jfcBranchNr, orderByDate, IIf(mobile, DAYDATE_SQLFMT, DAYDATE_SQLFMT)) = True Then
                    orderByDateTime = "(Order by TODAY " & orderByTime & ")"
                Else
                    orderByDateTime = "(Order by " & orderByDate & " " & orderByTime & ")"
                End If
            End If
            info &= IIf(i = 0, "", ", ") & IIf(GetNrCustGrpVendor(DB, custGrpID) > 1, CString(r("vendorName")) & ": ", "") & IIf(deliveryDate = "", noDateStr, deliveryDate & " delivery " & orderByDateTime)
            i += 1
        End While
        r.close()
        Return info
    End Function

    'Public Shared Function GetUserTimeZone(ByVal DB As DBHelper, Optional ByVal userEmail As String = "") As String
    '    Dim SQL As String

    '    SQL = "SELECT timeZone FROM webUser WHERE email = :1"
    '    Return CString(DB.GetScalar(SQL, IIf(userEmail <> "", userEmail, GetUserEmail(DB))))
    'End Function

    ''Get user's time zone difference from server
    'Public Shared Function GetServerDayOffset(ByVal DB As DBHelper, Optional ByVal userEmail As String = "") As Decimal
    '    Dim userTimeZone As String = GetUserTimeZone(DB, userEmail)
    '    Dim info As TimeZoneInfo
    '    Dim userDate As Date

    '    'Use server time if user time zone doesn't exist
    '    If userTimeZone = String.Empty Then Return 0

    '    'Find day offset
    '    info = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone)
    '    userDate = TimeZoneInfo.ConvertTime(Date.Now, info)
    '    Return (userDate - Date.Now).TotalDays
    'End Function

    ''Get vendor's time difference from server (jfcVendor.serverTimeOffset)
    'Public Shared Function GetServerDayOffset(ByVal DB As DBHelper, ByVal jfcBranchNr As String) As Decimal
    '    Dim SQL As String = "SELECT ROUND(NVL(serverTimeOffset, 0) / 24, 4) FROM jfcVendor WHERE jfcBranchNr = :1"
    '    Return Val(DB.GetScalar(SQL, jfcBranchNr))
    'End Function

    'Get vendor's time difference from server (jfcVendor.serverTimeOffset) and the vendor's time zone
    Public Shared Function GetServerDayOffset(ByVal DB As DBHelper, ByVal jfcBranchNr As String, Optional ByRef timeZone As String = "") As Decimal
        Dim r As DataReader
        Dim SQL As String = "SELECT ROUND(NVL(serverTimeOffset, 0) / 24, 4) AS serverDayOffset, timeZone FROM jfcVendor WHERE jfcBranchNr = :1"
        Dim serverDayOffset As Decimal = 0

        r = DB.GetReader(SQL, jfcBranchNr)
        If r.Read() Then
            serverDayOffset = Common.Val(r("serverDayOffset"))
            timeZone = Common.CString(r("timeZone"))
        End If
        r.close()
        Return serverDayOffset 'Val(DB.GetScalar(SQL, jfcBranchNr))
    End Function

    Public Shared Function GetWebImgUrl(ByVal fileName As String) As String
        Dim url As String
        url = "~/Images/Web/" & fileName
        Return url
    End Function

    Public Shared Function CompressIfLarge(originalStream As Stream, Optional targetSizeKB As Integer = 100, Optional resizeThresholdKB As Integer = 100) As Byte()

        Using msOriginal As New MemoryStream()
            originalStream.CopyTo(msOriginal)

            ' Check original file size
            Dim originalSizeKB As Integer = CInt(msOriginal.Length / 1024)

            If originalSizeKB <= resizeThresholdKB Then
                Return msOriginal.ToArray()
            End If

            msOriginal.Position = 0

            If Not IsValidImageFormat(msOriginal) Then
                Throw New FormatException("Uploaded file is not a supported image.")
            End If

            msOriginal.Position = 0

            Dim quality As Integer = 90
            Dim compressedBytes As Byte()

            Using originalImage As Image = Image.FromStream(msOriginal)
                Dim ratioX As Double = CDbl("720") / originalImage.Width
                Dim ratioY As Double = CDbl("1600") / originalImage.Height
                Dim ratio As Double = Math.Min(ratioX, ratioY)

                Dim newWidth As Integer = CInt(originalImage.Width * ratio)
                Dim newHeight As Integer = CInt(originalImage.Height * ratio)

                Using bitmap As New Bitmap(newWidth, newHeight)
                    Using g As Graphics = Graphics.FromImage(bitmap)
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.DrawImage(originalImage, 0, 0, bitmap.Width, bitmap.Height)

                        Dim encoderParams As New EncoderParameters(1)
                        Dim jpegEncoder As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/jpeg")

                        Do
                            Using msCompressed As New MemoryStream()
                                encoderParams.Param(0) = New EncoderParameter(Encoder.Quality, quality)
                                bitmap.Save(msCompressed, jpegEncoder, encoderParams)
                                compressedBytes = msCompressed.ToArray()

                                If compressedBytes.Length <= targetSizeKB * 1024 OrElse quality <= 20 Then
                                    Exit Do
                                Else
                                    quality -= 5
                                End If
                            End Using
                        Loop
                    End Using
                End Using
            End Using

            Return compressedBytes
        End Using
    End Function

    Private Shared Function IsValidImageFormat(imgStream As Stream) As Boolean
        Try
            Using img As Image = Image.FromStream(imgStream, False, True)
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Public Shared Function GetHomePageImg(ByVal Branch As String, ByVal fileID As String) As String
        Dim url As String
        Dim filepath As String = "C:\\inetpub\\wwwroot\\KOSCUS_JFCAdmin\\Images\\HomePageImg\\" & Branch & "\\"
        If Not System.IO.Directory.Exists(filepath) Then
            System.IO.Directory.CreateDirectory(filepath)
        End If
        url = "~/Images/HomePageImg/" & Branch & "/" & fileID
        If System.IO.File.Exists(HttpContext.Current.Server.MapPath(url)) Then
            Return url
        Else
            'Return "~/Images/Web/noImg.gif"
            Return "~/Images/Web/noImg.jpg"
        End If
    End Function

    Public Shared Function GetHomepageFilePath(ByVal Branch As String) As String
        Dim filepath As String
        If IsProdBeta() Or IsProduction() Then

            filepath = "C:\\inetpub\\wwwroot\\KOSCUSAPI\\Images\\HomePageImg\\" & Branch & "\\"
        Else
            filepath = "C:\\inetpub\\wwwroot\\KOSCUS_JFCAdmin\\Images\\HomePageImg\\" & Branch & "\\"
            ' "\\\\JHOS2BDC01\\kms$\\10 source code\\24 KOSCUS API\\Images\\HomePageImg\\" & Branch & "\\"
        End If
        If (Not System.IO.Directory.Exists(filepath)) Then
            System.IO.Directory.CreateDirectory(filepath)
        End If
        Return filepath
    End Function

    Public Shared Sub DeleteHomePageFile(Branch As String, fileID As String)

        Dim Path1 = GetHomepageFilePath(Branch) & fileID
        Dim url = "~/Images/HomePageImg/" & Branch & "/" & fileID
        Dim Path2 = HttpContext.Current.Server.MapPath(url)
        If File.Exists(Path2) Then
            My.Computer.FileSystem.DeleteFile(Path2)
        End If
        If File.Exists(Path1) Then
            My.Computer.FileSystem.DeleteFile(Path1)

        End If



    End Sub

    Public Shared Function GetItemImgUrl(ByVal imgType As String, ByVal itemID As String) As String
        Dim url As String
        url = "~/Images/Items/" & imgType & "/item" & itemID & ".jpg"
        If System.IO.File.Exists(HttpContext.Current.Server.MapPath(url)) Then
            Return url
        Else
            'Return "~/Images/Web/noImg.gif"
            Return "~/Images/Web/noImg.jpg"
        End If
    End Function

    Public Shared Function GetNewItemImgFolderName() As String
        Return Format(Today, "yyyyMM") & "_" & Common.CString(Common.Val(Format(Today, "yyyyMM")) * 25 Mod 1234)
    End Function

    Public Shared Function GetNewItemImgUrl(ByVal itemID As String) As String
        Dim partUrl As String
        Dim file As String
        Dim folderName As String = GetNewItemImgFolderName()

        file = "C:\\inetpub\\wwwroot\\KOSCUS\\Login\\Images\\NewItems\\" & folderName & "\\item" & itemID & ".jpg"
        If IsDev() = True Then
            partUrl = "http://" & HttpContext.Current.Request.Url.Host & IIf(HttpContext.Current.Request.Url.Port = 80 OrElse HttpContext.Current.Request.Url.Port = 443, "", ":" & HttpContext.Current.Request.Url.Port) &
                    HttpContext.Current.Request.ApplicationPath & "/Login/Images/NewItems/"
        Else
            partUrl = "http://shop.jfc.com/Login/Images/NewItems/" 'Can't use "http://192.168.207.5/xxx" because image won't show on Gmail
        End If
        If System.IO.File.Exists(file) Then
            Return partUrl & folderName & "/item" & itemID & ".jpg"
        Else
            Return partUrl & "noImg.jpg"
        End If
    End Function

    Public Shared Sub CopyNewItemImg(ByVal imgType As String, ByVal itemID As String)
        Dim path As String
        Dim srcImgFile As String
        Dim destImgFile As String

        'Do nothing if source image doesn't exist
        If IsDev() = True Then
            srcImgFile = "C:\\inetpub\\wwwroot\\KOSCUS\\Images\\Items\\" & imgType & "\\item" & itemID & ".jpg"
        Else
            srcImgFile = "D:\\KOSCUS_JFCItems\\item" & itemID & ".jpg"
        End If
        If System.IO.File.Exists(srcImgFile) = False Then Exit Sub

        'Create a directory if doesn't exist
        path = "C:\\inetpub\\wwwroot\\KOSCUS\\Login\\Images\\NewItems\\" & GetNewItemImgFolderName() & "\" 'Can't use "D:\\xxx" for 192.168.207.5 because image won't show on Gmail
        If System.IO.Directory.Exists(path) = False Then
            Try
                System.IO.Directory.CreateDirectory(path)
            Catch err As IOException
                Console.WriteLine(err.Message)
            End Try
        End If

        'Do nothing if destination file already exists
        destImgFile = path & "item" & itemID & ".jpg"
        If System.IO.File.Exists(destImgFile) = True Then Exit Sub

        'Copy image file
        Try
            System.IO.File.Copy(srcImgFile, destImgFile)
        Catch err As System.IO.IOException
            Console.WriteLine(err.Message)
        End Try
    End Sub

    Public Shared Function GetWebImgUrl_OnOff(ByVal name As String, ByVal opt As String) As String
        Dim url As String = ""

        Select Case name
            Case "FAVORITE"
                url = "~/Images/Web/" & IIf(opt = "ON", "favorite.jpg", "nonFavorite.jpg")
            Case "FAVORITE2"
                url = "~/Images/Web/" & IIf(opt = "ON", "favorite.png", "nonFavorite.png")
            Case "ORDERED"
                url = "~/Images/Web/" & IIf(opt = "ON", "ordered.png", "nonOrdered.png")
            Case "REST_ITEM", "AVAILABLE"
                url = "~/Images/Web/" & IIf(opt = "ON", "linked.png", "nonLinked.png")
            Case "CHECKED"
                url = "~/Images/Web/" & IIf(opt = "ON", "checked.png", "nonChecked.jpg")
            Case "CHECKED2"
                url = "~/Images/Web/" & IIf(opt = "ON", "checked.png", "nonChecked.png")
            Case "IMAGE"
                url = "~/Images/Web/" & IIf(opt = "ON", "image.png", "noImage.png")
        End Select

        Return url
    End Function

    Public Shared Function GetCustomerIDFromCartID(ByVal DB As DBHelper, ByVal cartID As String) As String
        Dim SQL As String

        SQL = "SELECT customer_id FROM sohdr_cart WHERE cart_id = :1"
        Return CString(DB.GetScalar(SQL, cartID))
    End Function

    Public Shared Function GetItemGrpIDInCart(ByVal DB As DBHelper, ByVal cartID As String) As String
        Dim SQL As String
        Dim customerID As String

        customerID = GetCustomerIDFromCartID(DB, cartID)
        SQL = "SELECT i.itemGrp_id FROM sodtl_cart c, " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " i " &
                "WHERE c.custGrpItem_id = i.custGrpItem_id AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & " AND c.cart_id = :1"
        '"i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
        Return CString(DB.GetScalar(SQL, cartID))
    End Function

    Public Shared Function GetItemGrpIDInOrder(ByVal DB As DBHelper, ByVal webOrderID As String) As String
        Dim SQL As String

        SQL = "SELECT itemGrp_id FROM sodtl WHERE webOrder_id = :1"
        Return CString(DB.GetScalar(SQL, webOrderID))
    End Function

    Public Shared Function IsSameItemGrpInCart(ByVal DB As DBHelper, ByVal cartID As String, ByVal custGrpItemID As String, ByRef itemGrpNameInCart As String) As Boolean
        Dim SQL As String
        Dim itemGrpID As String

        'Yes if cart is empty
        If GetNrItemInCart(DB, GetUserEmail(DB), HttpContext.Current.Session("SYS_SESSION_CUSTID")) = 0 Then Return True
        'Check if different order form exists in cart
        SQL = "SELECT itemGrp_id FROM custGrp_item WHERE custGrpItem_id = :1"
        itemGrpID = CString(DB.GetScalar(SQL, custGrpItemID))
        SQL = "SELECT g.itemGrpName FROM sodtl_cart c, custGrp_item i, custGrp_itemGrp g " &
                "WHERE c.custGrpItem_id = i.custGrpItem_id AND i.itemGrp_id <> :1 AND c.cart_id = :2 AND i.itemGrp_id = g.itemGrp_id"
        itemGrpNameInCart = CString(DB.GetScalar(SQL, itemGrpID, cartID))
        Return IIf(itemGrpNameInCart = "", True, False)
    End Function

    Public Shared Function IsSameItemGrpInOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal custGrpItemID As String, ByRef itemGrpNameInOrder As String) As Boolean
        Dim SQL As String
        Dim itemGrpID As String

        'Yes if order is empty
        If GetNrItemInOrder(DB, webOrderID) = 0 Then Return True
        'Check if different order form exists in order
        SQL = "SELECT itemGrp_id FROM custGrp_item WHERE custGrpItem_id = :1"
        itemGrpID = CString(DB.GetScalar(SQL, custGrpItemID))
        SQL = "SELECT g.itemGrpName FROM sodtl o, custGrp_itemGrp g " &
                "WHERE o.itemGrp_id = g.itemGrp_id AND o.itemGrp_id <> :1 AND o.webOrder_id = :2"
        itemGrpNameInOrder = CString(DB.GetScalar(SQL, itemGrpID, webOrderID))
        Return IIf(itemGrpNameInOrder = "", True, False)
    End Function

    Public Shared Sub ShowCustCompanyList(ByVal DB As DBHelper, ByVal email As String, ByVal custAdminOnly As Boolean, ByRef lst As DropDownList, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim dt As DataTable
        Dim SQL As String
        Dim dr As DataRow

        SQL = "SELECT u.custCompany_id, " & IIf(SMALL_CUST_ONLY = True, "SUBSTR(c.custCompany_id, 1, 2) || '-' || SUBSTR(c.custCompany_id, 3, 4)", "c.custCompany_id") & " || ' ' || c.custCompanyName AS custCompanyName " &
                "FROM webUser_customer u, custCompany c, customer a " &
                "WHERE u.custCompany_id = c.custCompany_id AND u.customer_id = a.customer_id AND u.email = :1 " & IIf(custAdminOnly = True, " AND u.userCompany_id = 'ADMIN' ", " AND u.userCompany_id = 'JFC' ") &
                "GROUP BY u.custCompany_id, c.custCompany_id, c.custCompanyName " &
                "ORDER BY c.custCompany_id, c.custCompanyName"
        dt = DB.GetDataTable(SQL, email)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("custCompanyName") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        lst.DataValueField = "custCompany_id"
        lst.DataTextField = "custCompanyName"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowBranchList(ByVal DB As DBHelper, ByVal email As String, ByVal custAdminOnly As Boolean, ByRef lst As DropDownList, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim dt As DataTable
        Dim SQL As String
        Dim dr As DataRow


        If custAdminOnly Then
            SQL = "SELECT u.jfcbranchnr as custCompany_id, u.jfcbranchnr || ' ' || j.vendorname AS custCompanyName " &
               "FROM  customer u, jfcvendor j " &
               "WHERE u.jfcbranchnr = j.jfcbranchnr and j.jfcbranchnr <> '99' " &
               "GROUP BY u.jfcbranchnr ,j.vendorname " &
               "ORDER BY u.jfcbranchnr, j.vendorname "

        Else
            SQL = "SELECT SUBSTR(u.custCompany_id, 1, 2) as custCompany_id, SUBSTR(u.custCompany_id, 1, 2) || ' ' || j.vendorname AS custCompanyName " &
               "FROM  webUser_customer u, jfcvendor j " &
               "WHERE SUBSTR(u.custCompany_id, 1, 2) = j.jfcbranchnr AND u.email = :1 AND u.userCompany_id = 'JFC' " &
               "GROUP BY SUBSTR(u.custCompany_id, 1, 2) ,j.vendorname " &
               "ORDER BY SUBSTR(u.custCompany_id, 1, 2), j.vendorname "
        End If

        dt = DB.GetDataTable(SQL, email)
        If insertFirstLine = True And dt.Rows.Count > 0 Then
            dr = dt.NewRow
            dr("custCompanyName") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        lst.DataValueField = "custCompany_id"
        lst.DataTextField = "custCompanyName"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowCustGrpList(ByVal DB As DBHelper, ByVal custCompanyID As String, ByRef lst As DropDownList, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String
        Dim dt As DataTable
        Dim dr As DataRow

        SQL = "SELECT custGrp_id, custGrpName FROM custGrp WHERE custCompany_id = :1 ORDER BY custGrpName"
        dt = DB.GetDataTable(SQL, custCompanyID)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("custGrpName") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        lst.DataValueField = "custGrp_id"
        lst.DataTextField = "custGrpName"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowCustomerList(ByVal DB As DBHelper, ByVal custGrpID As String, ByRef lst As DropDownList, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String
        Dim dt As DataTable
        Dim dr As DataRow

        SQL = "SELECT customer_id, customerName AS customerName_view FROM customer WHERE custGrp_id = :1 ORDER BY customerName_view" '|| ' (' || jfcBranchNr || '-' || jfcCustomerNr || ')' 
        dt = DB.GetDataTable(SQL, custGrpID)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("customerName_view") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        lst.DataValueField = "customer_id"
        lst.DataTextField = "customerName_view"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowShipPointList(ByVal DB As DBHelper, ByVal customerID As String, ByRef lst As DropDownList, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String
        Dim dt As DataTable
        Dim dr As DataRow

        'Show dropdownlist of shipping point
        SQL = "SELECT shipPoint_id, shipName " &
                "FROM customer_shipPoint " &
                "WHERE customer_id = :1 " &
                "ORDER BY shipName"
        dt = DB.GetDataTable(SQL, customerID)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("shipName") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        lst.DataValueField = "shipPoint_id" 'lstCustGrp_item.Text is custGrp_ID
        lst.DataTextField = "shipName"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowCodeMasterList(ByVal DB As DBHelper, ByVal codeGrp As String, ByRef lst As DropDownList,
                                         ByVal nameOnly As Boolean, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String
        Dim dt As DataTable
        Dim dr As DataRow

        Select Case codeGrp
            Case "ORDERSTATE"
                SQL = "SELECT code_id, codeDesc FROM codeMaster WHERE codeGrp = 'ORDERSTATE' AND code_id <> 'D' ORDER BY dispOrder"
                dt = DB.GetDataTable(SQL)
            Case "PAYMENTTYPE"
                SQL = "SELECT code_id, codeDesc FROM codeMaster " &
                        "WHERE codeGrp = :1 " &
                        IIf(ALLOW_CARD_PAYMENT = False, " AND code_id <> 'CARD' ", "") &
                        "ORDER BY dispOrder"
                dt = DB.GetDataTable(SQL, codeGrp)
            Case "USERFLAG"
                If IsMobile() Then
                    SQL = "SELECT code_id, decode(code_id,'Y','Y',' ') as codeDesc FROM codeMaster WHERE codeGrp = :1 ORDER BY dispOrder"
                Else
                    SQL = "SELECT code_id, codeDesc FROM codeMaster WHERE codeGrp = :1 ORDER BY dispOrder"
                End If

                dt = DB.GetDataTable(SQL, codeGrp)
            Case "HIDEDISPLAY"
                'If IsMobile() Then
                '    SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id,'1','H',' ') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'Else
                SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id,'1','Hide','Show') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'End If

                dt = DB.GetDataTable(SQL)
            Case "HIDE_N_SHOW_Y"

                SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id,'1','Show','Hide') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"

                dt = DB.GetDataTable(SQL)
            Case "ALLOWNOT"
                'If IsMobile() Then
                'SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id,'1','A',' ') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'Else
                SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id, '1','Allow','Not Allow') As codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'End If

                dt = DB.GetDataTable(SQL)
            Case "NALLOWNOT"
                'If IsMobile() Then
                'SQL = "SELECT decode(code_id,'1','N','Y') as code_id, decode(code_id,'1','A',' ') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'Else
                SQL = "SELECT decode(code_id,'1','N','Y') as code_id, decode(code_id, '1','Allow','Not Allow') As codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'End If

                dt = DB.GetDataTable(SQL)
            Case Else
                SQL = "SELECT code_id, codeDesc FROM codeMaster WHERE codeGrp = :1 ORDER BY dispOrder"
                dt = DB.GetDataTable(SQL, codeGrp)
        End Select
        If insertFirstLine = True Then
            'dt.Rows.InsertAt(dt.NewRow(), 0)
            'dt.Rows(0).Item(0) = firstLineStr
            dr = dt.NewRow
            dr("codeDesc") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        If nameOnly = True Then
            lst.DataValueField = "codeDesc"
        Else
            lst.DataValueField = "code_id"
            lst.DataTextField = "codeDesc"
        End If
        lst.DataBind()
    End Sub

    Public Shared Sub ShowCodeMasterListLocal(ByVal DB As DBHelper, ByVal codeGrp As String, ByRef lst As DropDownList,
                                         ByVal nameOnly As Boolean, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String
        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add("code_id")
        dt.Columns.Add("codeDesc")
        Select Case codeGrp

            Case "HIDEDISPLAY"

                dr = dt.NewRow
                dr("code_id") = "Y"
                dr("codeDesc") = "Hide"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "N"
                dr("codeDesc") = "Show"
                dt.Rows.Add(dr)

            Case "HIDE_N_SHOW_Y"

                dr = dt.NewRow
                dr("code_id") = "Y"
                dr("codeDesc") = "Show"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "N"
                dr("codeDesc") = "Hide"
                dt.Rows.Add(dr)
                'SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id,'1','Show','Hide') as codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"

                'dt = DB.GetDataTable(SQL)
            Case "ALLOWNOT"
                dr = dt.NewRow
                dr("code_id") = "Y"
                dr("codeDesc") = "Allow"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "N"
                dr("codeDesc") = "Not Allow"
                dt.Rows.Add(dr)
                'SQL = "SELECT decode(code_id,'1','Y','N') as code_id, decode(code_id, '1','Allow','Not Allow') As codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                ''End If
                'dt = DB.GetDataTable(SQL)
            Case "NALLOWNOT"
                dr = dt.NewRow
                dr("code_id") = "N"
                dr("codeDesc") = "Allow"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "Y"
                dr("codeDesc") = "Not Allow"
                dt.Rows.Add(dr)
                'SQL = "SELECT decode(code_id,'1','N','Y') as code_id, decode(code_id, '1','Allow','Not Allow') As codeDesc FROM codeMaster WHERE codeGrp = 'YESNO' ORDER BY dispOrder"
                'dt = DB.GetDataTable(SQL)
            Case "YESNO"
                dr = dt.NewRow
                dr("code_id") = "1"
                dr("codeDesc") = "Yes"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "2"
                dr("codeDesc") = "No"
                dt.Rows.Add(dr)
            Case "AMPM"
                dr = dt.NewRow
                dr("code_id") = "1"
                dr("codeDesc") = "AM"
                dt.Rows.Add(dr)
                dr = dt.NewRow
                dr("code_id") = "2"
                dr("codeDesc") = "PM"
                dt.Rows.Add(dr)
            Case "TIMEOFDAY"
                For i = 1 To 48 '24
                    dr = dt.NewRow
                    dr("code_id") = i.ToString
                    Dim time = New DateTime(2000, 1, 1, 0, 0, 0)
                    dr("codeDesc") = time.AddMinutes((i - 1) * 15).ToString("hh:mm") 'time.AddMinutes((i - 1) * 30).ToString("hh:mm")
                    dt.Rows.Add(dr)
                Next

                'SQL = "SELECT code_id, codeDesc FROM codeMaster WHERE codeGrp = :1 ORDER BY dispOrder"
                'dt = DB.GetDataTable(SQL, codeGrp)
        End Select
        If insertFirstLine = True Then
            'dt.Rows.InsertAt(dt.NewRow(), 0)
            'dt.Rows(0).Item(0) = firstLineStr
            dr = dt.NewRow
            dr("codeDesc") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        If nameOnly = True Then
            lst.DataValueField = "codeDesc"
        Else
            lst.DataValueField = "code_id"
            lst.DataTextField = "codeDesc"
        End If
        lst.DataBind()
    End Sub

    Public Shared Sub ShowList(ByVal DB As DBHelper, ByVal cmd As String, ByVal id As String, ByRef lst As DropDownList,
                                      ByVal nameOnly As Boolean, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")
        Dim SQL As String = ""
        Dim dt As DataTable
        Dim dr As DataRow

        Select Case cmd
            Case "ITEMGRP"
                SQL = "SELECT itemGrp_id AS id, itemGrpName AS name FROM custGrp_itemGrp WHERE custGrp_id = :1 ORDER BY dispOrder"
            Case "CTG"
                SQL = "SELECT c.ctg_id AS id, c.ctgName AS name FROM custCompany_ctg c, custGrp g " &
                             "WHERE c.custCompany_id = g.custCompany_id AND g.custGrp_id = :1 " &
                             "ORDER BY c.dispOrder"
            Case "VENDOR"
                SQL = "SELECT vendor_id AS id, vendorName AS name FROM custGrp_vendor WHERE custGrp_id = :1 ORDER BY vendorName"
            Case "CUSTGRP_VENDOR"
                SQL = "SELECT custGrpVendor_id AS id, vendorName AS name FROM custGrp_vendor WHERE custGrp_id = :1 ORDER BY vendorName"
            Case "JFC_BRANCH_ALPHA"
                SQL = "SELECT jfcBranchAlpha AS id, jfcBranchAlpha || ' - ' || vendorName AS name FROM jfcVendor WHERE jfcBranchAlpha <> '' OR jfcBranchAlpha IS NOT NULL ORDER BY jfcBranchAlpha"

        End Select
        dt = DB.GetDataTable(SQL, id)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("name") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        If nameOnly = True Then
            lst.DataValueField = "name"
        Else
            lst.DataValueField = "id"
            lst.DataTextField = "name"
        End If
        lst.DataBind()
    End Sub

    Public Shared Sub ShowPaymentAccList(ByVal DB As DBHelper, ByVal customerID As String, ByRef lst As DropDownList)
        Dim SQL As String
        Dim dt As DataTable
        Dim dr As DataRow

        'Show dropdownlist of account list
        SQL = "SELECT accNickName FROM customer_payAcc " &
                "WHERE customer_id = :1 " &
                IIf(ALLOW_CARD_PAYMENT = False, " AND paymentType_id <> 'CARD' ", "") &
                "ORDER BY createDate DESC"
        dt = DB.GetDataTable(SQL, customerID)
        dr = dt.NewRow
        dr("accNickName") = LST_CMD_STR
        dt.Rows.InsertAt(dr, 0)
        lst.DataSource = dt
        lst.DataValueField = "accNickName"
        lst.DataBind()
    End Sub

    Public Shared Sub ShowTimeZoneList(ByRef lst As DropDownList)
        Dim timeZones As ObjectModel.ReadOnlyCollection(Of TimeZoneInfo) = TimeZoneInfo.GetSystemTimeZones()
        Dim dt2 As New DataTable()
        dt2.Columns.Add("id")
        dt2.Columns.Add("name")
        dt2.Rows.Add(String.Empty, LST_CMD_STR)
        For Each theTimeZone As TimeZoneInfo In timeZones
            dt2.Rows.Add(theTimeZone.Id.ToString(), theTimeZone.DisplayName.ToString())
        Next
        lst.DataValueField = "id"
        lst.DataTextField = "name"
        lst.DataSource = dt2
        lst.DataBind()
    End Sub

    Public Shared Function GetQtyHistory(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, Optional ByVal maxCount As Integer = 10, Optional ByVal shownAsToolTip As Boolean = False) As String
        Dim SQL As String
        Dim r As DataReader
        Dim qtyHistory As String = ""
        Dim i As Integer

        'Show ordered qty from web order for non JFC items and from JETS for JFC items
        SQL = "SELECT createDate, createDate_fmt, qty, invoiceNr " &
                "FROM (SELECT h.createDate AS createDate, TO_CHAR(h.createDate, :1) AS createDate_fmt, CASE WHEN d.csQty > 0 THEN d.csQty || ' cs ' END || CASE WHEN d.eaQty > 0 THEN d.eaQty || ' ea ' END || CASE WHEN d.lbs > 0 THEN d.lbs || ' lb' END qty, NULL AS invoiceNr " &
                " FROM sohdr h, sodtl d, custGrp_item i " &
                " WHERE h.webOrder_id = d.webOrder_id AND d.custGrpItem_id = i.custGrpItem_id AND i.jfcItem = 'N' AND d.custGrpItem_id = :2 AND h.customer_id = :3 " &
                " UNION " &
                " SELECT NVL(jh.shipDate, jh.createDate) AS createDate, TO_CHAR(NVL(jh.shipDate, jh.createDate), :4) AS createDate_fmt, CASE WHEN jd.csQty > 0 THEN jd.csQty || ' cs ' END || CASE WHEN jd.eaQty > 0 AND jd.lbs = 0 THEN jd.eaQty || ' ea ' END || CASE WHEN jd.pcQty > 0 AND jd.lbs > 0 THEN jd.pcQty || ' pc ' END || CASE WHEN jd.lbs > 0 THEN '(' || jd.lbs || ' lb)' END qty, jh.orderNr AS invoiceNr " &
                " FROM sohdr_jets jh, sodtl_jets jd, customer c " &
                " WHERE jh.jfcBranchNr = jd.jfcBranchNr AND jh.orderNr = jd.orderNr AND jd.item_id IN (SELECT item_id FROM " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " WHERE jfcItem = 'Y' AND custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & " AND custGrpItem_id = :5) " &
                "   AND jh.jfcBranchNr = c.jfcBranchNr AND jh.jfcCustomerNr = c.jfcCustomerNr AND c.customer_id = :6 " &
                ") ORDER BY createDate DESC"
        r = DB.GetReader(SQL, DATE_FMT, custGrpItemID, customerID, DATE_FMT, custGrpItemID, customerID)
        '"  AND custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance

        ''Show ordered qty from web orders and JETS orders (non web orders)
        ''Show qty from deleted web and JETS orders because customer_lastOrderedQty doesn't get updated when deleting order
        'SQL = "SELECT createDate, createDate_fmt, qty " & _
        '        "FROM (SELECT h.createDate AS createDate, TO_CHAR(h.createDate, :1) AS createDate_fmt, CASE WHEN d.csQty > 0 THEN d.csQty || ' case ' END || CASE WHEN d.eaQty > 0 THEN d.eaQty || ' each ' END || CASE WHEN d.lbs > 0 THEN d.lbs || ' lb' END qty " & _
        '        " FROM sohdr h, sodtl d " & _
        '        " WHERE h.webOrder_id = d.webOrder_id AND d.custGrpItem_id = :2 AND h.customer_id = :3 " & _
        '        " UNION " & _
        '        " SELECT jh.createDate AS createDate, TO_CHAR(jh.createDate, :4) AS createDate_fmt, CASE WHEN jd.csQty > 0 THEN jd.csQty || ' case ' END || CASE WHEN jd.eaQty > 0 THEN jd.eaQty || ' each ' END || CASE WHEN jd.lbs > 0 THEN jd.lbs || ' lb' END qty " & _
        '        " FROM sohdr_jets jh, sodtl_jets jd, customer c " & _
        '        " WHERE jh.jfcBranchNr = jd.jfcBranchNr AND jh.orderNr = jd.orderNr AND jh.webOrder_id is NULL AND jd.item_id IN (SELECT item_id FROM " & IIf(IsSmallCustomer(DB, customerID) = True, "v_custgrp_item_small", "custgrp_item") & " WHERE jfcItem = 'Y' AND custGrpItem_id = :5) " & _
        '        "   AND jh.jfcBranchNr = c.jfcBranchNr AND jh.jfcCustomerNr = c.jfcCustomerNr AND c.customer_id = :6 " & _
        '        ") ORDER BY createDate DESC"
        'r = DB.GetReader(SQL, DATE_FMT, custGrpItemID, customerID, DATE_FMT, custGrpItemID, customerID)
        i = 0
        While r.Read()
            qtyHistory &= CString(r("qty")) & " on " & CString(r("createDate_fmt")) & ", Inv#" & CString(r("invoiceNr")) & IIf(shownAsToolTip = True, vbCrLf, "<br />")
            If i = maxCount - 1 Then Exit While
            i += 1
        End While
        r.close()
        Return qtyHistory
    End Function
    'To improve performance
    'If ViewState("LST_SCHEDULE") IsNot Nothing Then
    '    lst = DirectCast(ViewState("LST_SCHEDULE"), String(,))
    '    lst = ViewState("LST_SCHEDULE")

    '    e.Day.IsSelectable = False

    '    If e.Day.IsOtherMonth = False Then
    '        If lst(0, e.Day.Date.Day) <> Nothing Then
    '            e.Cell.Font.Bold = True
    '            lbl.Text = lst(0, e.Day.Date.Day)
    '            e.Cell.Controls.Add(lbl)
    '        End If
    '        If lst(1, e.Day.Date.Day) <> Nothing Then
    '            e.Cell.Font.Bold = True
    '            lbl.Text = lst(1, e.Day.Date.Day)
    '            e.Cell.Controls.Add(lbl)
    '        End If
    '    End If
    'Else
    '    Response.Write("ViewState 'LST_SCHEDULE' is null")
    'End If

    'Get order by date
    'SQL = "SELECT WM_CONCAT(v.vendorName) vendorList " & _

    'SQL = "SELECT WM_CONCAT(v.vendorName || ' (order by ' || TO_CHAR(d.orderByDate, :1) ||')') vendorList " & _
    '        "FROM customer_deliveryDate d, custGrp_vendor v " & _
    '        "WHERE d.custGrpVendor_id = v.custGrpVendor_id " & _
    '        "  AND d.shipPoint_id = :2" & _
    '        "  AND d.customer_id = :3" & _
    '        "  AND TRUNC(d.orderByDate) = to_date(" & DB.SqlEsc(e.Day.Date) & ", :4) " & _
    '        "ORDER BY vendorName"

    'r = DB.GetReader(SQL, TIME_SQLFMT, shipPointID, customerID, DATE_FMT)
    'If r.Read() Then
    '    If CString(r("vendorList")).Trim <> "" Then
    '        'e.Cell.Font.Bold = True
    '        tok = CString(r("vendorList")).Split(",")
    '        For i As Integer = 0 To tok.Length() - 1
    '            lbl = New Label()
    '            lbl.Text = "<br/>Order: " & tok(i)
    '            lbl.Font.Bold = True
    '            If e.day.isotherMonth = False Then lbl.ForeColor = Drawing.Color.Black
    '            e.Cell.Controls.Add(lbl)
    '        Next
    '    End If
    'End If
    'r.close()
    Public Shared Sub ShowDayEventInCalendar(ByVal DB As DBHelper, ByVal page As System.Web.UI.Page, ByRef e As Object, ByVal customerID As String, ByVal shipPointID As String, ByVal viewOnly As Boolean,
                                           Optional ByVal lnkDelivery As LinkButton = Nothing)
        Dim SQL As String
        Dim r As DataReader
        Dim lbl As New Label
        Dim lnk As New HyperLink
        Dim img As System.Web.UI.WebControls.Image
        Dim tok As String()
        'Dim lst(1, 31) As String
        Dim info As CultureInfo = Thread.CurrentThread.CurrentCulture
        'If Not info.NumberFormat.IsReadOnly Then
        'Dim numberInfo As NumberFormatInfo = CultureInfo.CreateSpecificCulture("ja-JP").NumberFormat
        'info.NumberFormat = numberInfo
        'info.DateTimeFormat = CultureInfo.CreateSpecificCulture("ja-JP").DateTimeFormat
        'Thread.CurrentThread.CurrentUICulture = New CultureInfo("ja-JP")
        'Thread.CurrentThread.CurrentCulture = New CultureInfo("ja-JP")
        'End If
        e.Day.IsSelectable = False



        SQL = "SELECT WM_CONCAT(' (Order by ' || TO_CHAR(d.orderByDate, :1) ||')') vendorList " &
                "FROM customer_deliveryDate d, custGrp_vendor v " &
                "WHERE d.custGrpVendor_id = v.custGrpVendor_id " &
                "  AND d.shipPoint_id = :2" &
                "  AND d.customer_id = :3" &
                "  AND d.deliveryDate = to_date(" & DB.SqlEsc(Format(e.Day.Date, DATE_FMT)) & ", :4) " &
                "ORDER BY vendorName"
        r = DB.GetReader(SQL, SHORTDATETIME_DELFMT, shipPointID, customerID, DATE_FMT) '
        If r.Read() Then
            If CString(r("vendorList")).Trim <> "" Then
                'e.Cell.ForeColor = Drawing.Color.Red
                'e.Cell.Font.Underline = True
                tok = CString(r("vendorList")).Split(",")
                For i As Integer = 0 To tok.Length() - 1
                    If viewOnly = True OrElse e.Day.IsOtherMonth = True Then
                        lbl = New Label()
                        lbl.Text = "<br/>Delivery: <br/>" & tok(i)
                        e.Cell.Controls.Add(lbl)
                    Else
                        lnk = New HyperLink()
                        lnk.Text = "<br/>Delivery: <br/>" & tok(i)
                        lnk.ForeColor = System.Drawing.Color.Navy
                        'lnkDelivery_Click() gets called when clicking
                        lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & tok(i), True)
                        e.Cell.Font.Bold = True
                        e.Cell.Controls.Add(lnk)
                    End If
                Next
            End If
        End If
        r.close()
        'Get delivery date
        'SQL = "SELECT WM_CONCAT(v.vendorName) vendorList " & _
        '        "FROM customer_deliveryDate d, custGrp_vendor v " & _
        '        "WHERE d.custGrpVendor_id = v.custGrpVendor_id " & _
        '        "  AND d.shipPoint_id = :1" & _
        '        "  AND d.customer_id = :2" & _
        '        "  AND d.deliveryDate = to_date(" & DB.SqlEsc(e.Day.Date) & ", :3) " & _
        '        "ORDER BY vendorName"
        'r = DB.GetReader(SQL, shipPointID, customerID, DATE_FMT)
        'If r.Read() Then
        '    If CString(r("vendorList")).Trim <> "" Then
        '        'e.Cell.ForeColor = Drawing.Color.Red
        '        'e.Cell.Font.Underline = True
        '        tok = CString(r("vendorList")).Split(",")
        '        For i As Integer = 0 To tok.Length() - 1
        '            If viewOnly = True OrElse e.Day.IsOtherMonth = True Then
        '                lbl = New Label()
        '                lbl.Text = "<br/>Deliv: " & tok(i)
        '                e.Cell.Controls.Add(lbl)
        '            Else
        '                lnk = New HyperLink()
        '                lnk.Text = "<br/>Deliv: " & tok(i)
        '                lnk.ForeColor = System.Drawing.Color.Navy
        '                'lnkDelivery_Click() gets called when clicking
        '                lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & tok(i), True)
        '                e.Cell.Font.Bold = True
        '                e.Cell.Controls.Add(lnk)
        '            End If
        '        Next
        '    End If
        'End If
        'r.close()

        If viewOnly = False AndAlso e.Day.IsOtherMonth = False And lnk.Text = "" And lbl.Text = "" Then
            'Show add icon
            e.Cell.Controls.Add(New LiteralControl("<br />"))
            lnk = New HyperLink()
            'lnkDelivery_Click() gets called when clicking
            lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & ",", True)
            lnk.ToolTip = "Add a delivery"
            img = New System.Web.UI.WebControls.Image()
            img.ImageUrl = GetWebImgUrl("add.png")
            img.CssClass = "xxsImgBtn"
            img.BorderStyle = BorderStyle.None
            lnk.Controls.Add(img)
            'lnk.ImageUrl = GetWebImgUrl("add.png")
            'lnk.CssClass = "xxsImgBtn" 'Cannot set width and height for HyperLink
            e.Cell.Controls.Add(lnk)
        End If

    End Sub

    'SQL = "SELECT WM_CONCAT(' (Order by ' || TO_CHAR(d.orderByDate, :1) ||')') masterList, WM_CONCAT(m.DELIVERYMASTERNAME) as Namelist, WM_CONCAT(m.DELIVERYMASTER_ID) as MasterID  " & _
    '        "FROM jfcdeliverymaster_date d, jfcdeliverymaster m " & _
    '        "WHERE d.DELIVERYMASTER_ID = m.DELIVERYMASTER_ID AND m.jfcbranchnr = :2 " & _
    '        "AND d.deliveryDate = to_date(" & DB.SqlEsc(e.Day.Date) & ", :3) " & _
    '        "ORDER BY m.DELIVERYMASTERNAME  "

    'r = DB.GetReader(SQL, SHORTDATETIME_DELFMT, BranchNr, DATE_FMT)
    'If r.Read() Then
    '    If CString(r("masterList")).Trim <> "" Then
    '        'e.Cell.ForeColor = Drawing.Color.Red
    '        'e.Cell.Font.Underline = True
    '        tok = CString(r("masterList")).Split(",")
    '        'tok2 = CString(r("orderlist")).Split(",")
    '        tok3 = CString(r("Namelist")).Split(",")
    '        tok4 = CString(r("MasterID")).Split(",")
    '        For i As Integer = 0 To tok.Length() - 1
    '            Dim headtexts As String() = CString(tok3(i)).Split(":")
    '            Dim headname As String = ""
    '            If headtexts.Length >= 1 Then
    '                headname = headtexts(0).Split(" ")(0)
    '            End If
    '            If viewOnly = True OrElse e.Day.IsOtherMonth = True Then
    '                lbl = New Label()
    '                lbl.Text = "<br/>" & headname & ": " & tok(i)
    '                e.Cell.Controls.Add(lbl)
    '            Else
    '                lnk = New HyperLink()
    '                lnk.Text = "<br/>" & headname & ": " & tok(i)
    '                lnk.ForeColor = System.Drawing.Color.Navy
    '                'lnkDelivery_Click() gets called when clicking
    '                lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & tok4(i), True) 'Format(e.Day.Date, DATE_FMT) & "," & tok2(i) & ","
    '                e.Cell.Font.Bold = True
    '                e.Cell.Controls.Add(lnk)
    '            End If
    '        Next
    '    End If
    'End If
    Public Shared Sub ShowDayEventInMasterCalendar(ByVal DB As DBHelper, ByVal page As System.Web.UI.Page, ByRef e As Object, ByVal BranchNr As String, ByVal viewOnly As Boolean,
                                           Optional ByVal lnkDelivery As LinkButton = Nothing)
        Dim SQL As String
        Dim r As DataReader
        Dim lbl As New Label
        Dim lnk As New HyperLink
        'Dim img As Image
        'Dim tok As String()
        'Dim tok2 As String()
        'Dim tok3 As String()
        'Dim tok4 As String()
        'Dim lst(1, 31) As String

        e.Day.IsSelectable = False




        SQL = "SELECT ' (Order by ' || TO_CHAR(d.orderByDate, :1) ||')' AS Orderbydate, m.DELIVERYMASTERNAME, m.DELIVERYMASTER_ID " &
             "FROM jfcdeliverymaster_date d, jfcdeliverymaster m " & _
              "WHERE d.DELIVERYMASTER_ID = m.DELIVERYMASTER_ID AND m.jfcbranchnr = :2 " & _
              "AND d.deliveryDate = to_date(" & DB.SqlEsc(e.Day.Date) & ", :3) " & _
               "ORDER BY m.DELIVERYMASTERNAME  "
        r = DB.GetReader(SQL, SHORTDATETIME_DELFMT, BranchNr, DATE_FMT)
        While r.Read()
            If CString(r("Orderbydate")).Trim <> "" Then


                Dim headtexts As String() = CString(r("DELIVERYMASTERNAME")).Split(":")
                Dim headname As String = ""
                If headtexts.Length >= 1 Then
                    headname = headtexts(0).Split(" ")(0)
                End If
                If viewOnly = True OrElse e.Day.IsOtherMonth = True Then
                    lbl = New Label()
                    lbl.Text = "<br/>" & headname & ": " & CString(r("Orderbydate"))
                    e.Cell.Controls.Add(lbl)
                Else
                    lnk = New HyperLink()
                    lnk.Text = "<br/>" & headname & ": " & CString(r("Orderbydate"))
                    lnk.ForeColor = System.Drawing.Color.Navy
                    'lnkDelivery_Click() gets called when clicking
                    lnk.NavigateUrl = page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & CString(r("DELIVERYMASTER_ID")), True) 'Format(e.Day.Date, DATE_FMT) & "," & tok2(i) & ","
                    e.Cell.Font.Bold = True
                    e.Cell.Controls.Add(lnk)
                End If

            End If
        End While
        r.close()


    End Sub
    ' Fetches data for the entire month from the database
    Public Shared Function GetMonthlyData(ByVal DB As DBHelper, ByVal BranchNr As String, ByVal currentDate As DateTime) As DataTable
        Dim SQL As String = "SELECT ' (Order by ' || TO_CHAR(d.orderByDate, :1) ||')' AS Orderbydate, " &
                            "m.DELIVERYMASTERNAME, m.DELIVERYMASTER_ID, d.deliveryDate " &
                            "FROM jfcdeliverymaster_date d, jfcdeliverymaster m " &
                            "WHERE d.DELIVERYMASTER_ID = m.DELIVERYMASTER_ID AND m.jfcbranchnr = :2 " &
                            "AND d.deliveryDate BETWEEN TO_DATE(:3, :4) AND TO_DATE(:5, :6) " &
                            "ORDER BY m.DELIVERYMASTERNAME"


        Dim endDate As DateTime = currentDate.AddMonths(1).AddDays(15)

        ' Execute the query for the full month range
        Return DB.GetDataTable(SQL, SHORTDATETIME_DELFMT, BranchNr, currentDate.ToString(DATE_FMT), DATE_FMT, endDate.ToString(DATE_FMT), DATE_FMT)
    End Function

    ' Process and display events for each day


    Public Shared Sub SetGridFormat(grid As GridView)
        For Each col As DataControlField In grid.Columns
            If TypeOf (col) Is BoundField Then
                Dim c As BoundField = col
                If c.DataFormatString = "{0:d}" Then
                    c.DataFormatString = "{0}"
                    c.DataFormatString = "{0:" & GetSysParam("DATEFORMAT_FULL") & "}"
                End If
            End If
        Next
    End Sub

    Public Shared Function NullDate() As Date
        Dim ret As Date
        Return ret
    End Function


    Public Shared Function GetSysParam(ParamID As String) As String
        Dim ret As String
        Dim hash As Hashtable = HttpContext.Current.Session("SYS_PARAM_HASH")

        If hash Is Nothing Then
            hash = New Hashtable
            HttpContext.Current.Session("SYS_PARAM_HASH") = hash
        End If

        If hash.ContainsKey(ParamID) Then
            Return hash(ParamID)
        Else
            Using DB As New DBHelper
                ret = DB.GetScalarEx("select value from sys_param_company where paramid=:1 and cid=:2", "", "", ParamID, GetCID())
                If ret = "" Then ret = DB.GetScalarEx("select value from param where paramid=:1", "", "", ParamID)
                hash(ParamID) = ret
                Return ret
            End Using
        End If
    End Function

    Public Shared Function CaptalizeFirstLetter(ByVal word As String) As String
        If word = String.Empty Then Return word
        Return word.ToUpper().Substring(0, 1) & word.ToLower().Substring(1)
    End Function

    Public Shared Function ParseDate(ByVal s As String) As Date
        Dim ret As Date
        If s.Trim = "" Then
            Return ret
        End If
        If ParseDate(s, ret) = False Then Throw New Exception("ParseDate error: " & s)
        Return ret
    End Function


    Public Shared Function ParseDate(ByVal s As String, ByRef ret As Date) As Boolean
        Dim year, month, day As Integer
        Dim m As Match
        Dim datestr As String

        m = Regex.Match(s, "^(\d+)[./](\d+)$")
        If m.Success = True Then
            year = DateTime.Now.Year
        Else
            m = Regex.Match(s, "^(\d+)[./](\d+)[./](\d+)")
            If m.Success = False Then Return False
        End If

        Select Case GetSysParam("DATEFORMAT_FULL")
            Case "dd/MM/yyyy"
                day = Val(m.Groups(1).Value)
                month = Val(m.Groups(2).Value)
                If year = 0 Then year = Val(m.Groups(3).Value)
            Case Else
                month = Val(m.Groups(1).Value)
                day = Val(m.Groups(2).Value)
                If year = 0 Then year = Val(m.Groups(3).Value)
        End Select

        datestr = Format(year, "0000") & Format(month, "00") & Format(day, "00")
        Return DateTime.TryParseExact(datestr, "yyyyMMdd", Nothing, Nothing, ret)
    End Function


    Public Shared Function ParseTime(ByVal baseday As Date, ByVal TimeExpression As String) As Date
        Dim ret As Date
        If TimeExpression = "" Then Return ret
        If ParseTime(baseday, TimeExpression, ret) = False Then Throw New Exception("ParseTime error: " & TimeExpression)
        Return ret
    End Function


    Public Shared Function ParseTime(ByVal baseday As Date, ByVal TimeExpression As String, ByRef ret As Date) As Boolean
        Dim d As Date
        Dim offset As Integer = 0
        Dim m As RegularExpressions.Match
        Dim s As String

        TimeExpression = CString(TimeExpression).Trim.ToLower
        m = Regex.Match(TimeExpression, "^(\d\d?)[:., /]?(\d?\d?)\s*((?:pm)?(?:am)?)$")
        If m.Success = False Then Return False

        If m.Groups(3).Value = "pm" Then offset = 12
        s = Format(Val(m.Groups(1).Value), "00") & Format(Val(m.Groups(2).Value), "00")
        If DateTime.TryParseExact(Format(baseday, "yyyyMMdd") & s, "yyyyMMddHHmm", Nothing, Nothing, d) Then
            ret = d.AddHours(offset)
            Return True
        End If
        Return False
    End Function


    Public Shared Function GetFirstWord(ByVal s As String, Optional ByVal separator As String = " ") As String
        s = CString(s).Trim
        If s = "" OrElse s.IndexOf(separator) < 0 Then Return s
        Return s.Substring(0, s.IndexOf(separator))
    End Function

    Public Shared Function GetLastWord(ByVal s As String, Optional ByVal separator As String = " ") As String
        Dim ary As String() = WordsToArray(s, separator)
        If ary.Length = 0 Then
            Return ""
        Else
            Return ary(ary.Length - 1)
        End If
    End Function

    Public Shared Function GetRestWords(ByVal s As String, Optional ByVal separator As String = " ") As String
        s = CString(s).Trim
        If s = "" OrElse s.IndexOf(separator) < 0 OrElse s.IndexOf(separator) = s.Length - 1 Then Return ""
        Return s.Substring(s.IndexOf(separator) + 1)
    End Function


    Public Shared Function WordsToArray(ByVal s As String, Optional ByVal separator As String = " ") As String()
        Dim ret As New List(Of String)
        Dim head, tail As String

        While s.Trim.Length > 0
            s = s.Trim
            head = GetFirstWord(s, separator)
            tail = GetRestWords(s, separator)
            ret.Add(head)
            s = tail
        End While

        Return ret.ToArray
    End Function


    Public Shared Function ValidateValues(ByVal DB As DBHelper, ByVal lblError As Label, ByVal ParamArray param() As Object) As Boolean
        Dim i, ret As Integer
        Dim datatype As String
        Dim datasize As Integer
        Dim datascale As Integer
        Dim value, CID, BID, fieldname, def_fieldname As String
        Dim errmark As Label
        Dim nullable As Boolean
        Dim d As Date
        Dim f As Double

        CID = GetEffectiveCID()
        BID = GetEffectiveBID()
        lblError.Text = ""

        For i = param.GetLowerBound(0) To param.GetUpperBound(0) - 6 Step 7
            datatype = param(i)
            value = CString(param(i + 1)).Trim
            datasize = param(i + 2)
            datascale = param(i + 3)
            nullable = param(i + 4)
            fieldname = param(i + 5)
            errmark = param(i + 6)

            def_fieldname = ""
            Select Case datatype
                Case "ABCACTIVITY_SALES"
                    def_fieldname = "ACTIVITY ID"
                    ret = DB.GetScalar("select count(*) from ABC_ACTIVITY_SALES where ACTIVITYID = :a", value)
                Case "CURRENCY"
                    def_fieldname = "CURRENCY"
                    ret = DB.GetScalar("select count(*) from CODE where CID =:1 and CODETYPE='CURRENCY' and code_id = :2", CID, value)
                Case "CUSTOMERID"
                    def_fieldname = "CUSTOMER ID"
                    ret = DB.GetScalar("select count(*) from CUSTOMER where BID = :a and CUSTOMERID = :b", BID, value)
                Case "CUSTOMERID_AND_GROUPID"
                    def_fieldname = "CUSTOMER ID/GROUP ID"
                    ret = DB.GetScalar("select count(*) from CUSTOMER where BID = :a and CUSTOMERID = :b", BID, value)
                    If ret = 0 Then ret = DB.GetScalar("select count(*) from CUSTGROUP where GROUPID = :a", value)
                Case "EXPENSE_USAGEID"
                    def_fieldname = "USAGE"
                    ret = DB.GetScalar("select count(*) from CODE where cid=:1 and codetype = 'EXPENSE_USAGE' and code_id = :2", CID, value)
                Case "GLAN"
                    def_fieldname = "GLAN"
                    ret = DB.GetScalar("select count(*) from  where cid=:1 and codetype = 'EXPENSE_USAGE' and code_id = :2", CID, value)
                Case "NUMBER"
                    ret = 0
                    If Double.TryParse(value, f) = True AndAlso Math.Round(f, datascale) = f Then
                        If f.ToString.Length - IIf(f.ToString.IndexOf(".") < 0, 0, 1) - IIf(f.ToString.IndexOf("-") < 0, 0, 1) <= datasize Then ret = 1
                    End If
                Case "VARCHAR"
                    If value.Length > datasize Then ret = 0 Else ret = 1
                Case "DATE"
                    def_fieldname = "Date"
                    If ParseDate(value, d) = True Then ret = 1 Else ret = 0
                Case "TIME"
                    def_fieldname = "TIME"
                    If ParseTime(Now, value, d) = True Then ret = 1 Else ret = 0
                Case Else
                    Throw New Exception("ValidateValues error: wrong type (" & datatype & ")")
            End Select

            If value = "" Then
                If nullable = True Then Continue For
                If fieldname = "" Then fieldname = def_fieldname
                If lblError IsNot Nothing Then lblError.Text = "Please enter " & fieldname
                If errmark IsNot Nothing Then errmark.Text = "(Error)"
                Return False
            End If

            If ret = 0 Then
                If fieldname = "" Then fieldname = def_fieldname
                If lblError IsNot Nothing Then lblError.Text = "Please enter correct value in " & fieldname
                If errmark IsNot Nothing Then errmark.Text = "(Error)"
                Return False
            End If

            If errmark IsNot Nothing Then errmark.Text = ""
        Next

        Return True
    End Function

    Public Shared Function IsInSession() As Boolean
        Return HttpContext.Current.Session("SYS_SESSION_USERID") IsNot Nothing
    End Function

    Public Shared Function GetUserID() As Decimal
        Return HttpContext.Current.Session("SYS_SESSION_USERID")
    End Function

    Public Shared Function GetUserFullName(Optional ByVal UserID As Integer = -1) As String
        Return GetUserParam("PROFILE", "FULLNAME", UserID)
    End Function

    Public Shared Function GetSupportEmail() As String
        Return IIf(IsProduction() = True, "web@jfc.com", "jzhou@kmsnet.com")
    End Function

    Public Shared Function GetUserEmail(ByVal DB As DBHelper, Optional ByVal UserID As Integer = -1) As String
        If UserID = -1 Then
            Return HttpContext.Current.Session("SYS_SESSION_EMAIL")
        Else
            Return CString(DB.GetScalarEx("select email from webuser where user_id = :1", "", "", UserID))
        End If
    End Function

    Public Shared Function GetUserCompanyName(ByVal DB As DBHelper, ByVal cmd As String) As String
        Dim SQL As String
        Dim companyName As String = ""

        If cmd = "CUSTOMER" Then
            SQL = "SELECT custCompanyName FROM custCompany WHERE custCompany_id = :1"
            companyName = CString(DB.GetScalar(SQL, CString(HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYID"))))
        ElseIf cmd = "VENDOR" Then
            SQL = "SELECT vendorCompanyName FROM vendorCompany WHERE vendorCompany_id = :1"
            companyName = CString(DB.GetScalar(SQL, CString(HttpContext.Current.Session("SYS_SESSION_VENDORCOMPANYID"))))
        End If
        Return companyName
    End Function

    Public Shared Function GetCodeMaster(ByVal DB As DBHelper, ByVal codeGrp As String, ByVal codeID As String) As String
        Return CString(DB.GetScalar("SELECT codeDesc FROM codemaster WHERE codeGrp = '" & codeGrp & "' AND code_id = '" & codeID & "'"))
    End Function

    Public Shared Function GetMaxDataLen(ByVal DB As DBHelper, ByVal tableName As String, ByVal columnName As String) As Integer
        Return Val(DB.GetScalar("SELECT data_length FROM all_tab_cols WHERE table_name = '" & tableName.ToUpper & "' AND column_name = '" & columnName.ToUpper & "'"))
    End Function

    Public Shared Function GetBID() As String
        Return GetUserParam("PROFILE", "BID")
    End Function

    Public Shared Function GetCID() As String
        Return GetUserParam("PROFILE", "CID")
    End Function

    Public Shared Function GetEffectiveBID() As String
        Dim dir As String = System.IO.Path.GetDirectoryName(HttpContext.Current.Request.FilePath)
        dir = System.IO.Path.GetFileName(dir).ToUpper
        Dim v As String = HttpContext.Current.Session("EFFECTIVE_BID_" & dir)
        If v = "" Then Return GetBID() Else Return v
    End Function

    Public Shared Function GetEffectiveCID() As String
        Dim dir As String = System.IO.Path.GetDirectoryName(HttpContext.Current.Request.FilePath)
        Dim v As String = HttpContext.Current.Session("EFFECTIVE_CID_" & dir.Substring(1).ToUpper)
        If v = "" Then Return GetCID() Else Return v
    End Function


    Public Shared Function GetUserParam(ByVal AppID As String, ByVal ParamID As String, Optional ByVal UserID As Integer = -1) As String
        Dim ret As String
        Dim hash As Hashtable = HttpContext.Current.Session("SYS_USER_PARAM_HASH")

        If UserID > -1 Then
            Using DB As New DBHelper
                Return DB.GetScalarEx("select value from sys_app_userparam where appid = :1 and userid = :2 and paramid = :3 order by disporder", "", "", _
                                       AppID, UserID, ParamID)
            End Using
        End If

        If hash Is Nothing Then
            hash = New Hashtable
            HttpContext.Current.Session("SYS_USER_PARAM_HASH") = hash
        End If

        If hash.ContainsKey(AppID & "|" & ParamID) Then
            Return hash(AppID & "|" & ParamID)
        Else
            Using DB As New DBHelper
                ret = DB.GetScalarEx("select value from sys_app_userparam where appid = :1 and userid = :2 and paramid = :3 order by disporder", "", "", _
                                   AppID, GetUserID, ParamID)
                hash(AppID & "|" & ParamID) = ret
                Return ret
            End Using
        End If
    End Function

    Public Shared Function GetParam(ByVal DB As DBHelper, ByVal name As String, ByVal index As String) As String
        Dim SQL As String
        Dim rtnStr As String

        'Get value
        SQL = "SELECT var_value FROM param WHERE var_name = :1 AND var_index = :2"
        rtnStr = CString(DB.GetScalar(SQL, name, index))
        'If rtnStr = "" Then ErrBox("Cannot find name '" & name & "' or index '" & index & "'", True)
        Return CString(rtnStr)
    End Function

    Public Shared Function CreatePasswordHash(ByVal pwd As String, ByVal salt As String) As String
        Dim sha256 As New SHA256CryptoServiceProvider
        Dim saltandPwd As String = String.Concat(pwd, salt)
        Dim ret As String = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltandPwd)))
        Return ret
    End Function

    Public Shared Function CreateSalt(ByVal size As Integer) As String
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider
        Dim buff As Byte() = New Byte(size) {}
        rng.GetBytes(buff)

        Return Convert.ToBase64String(buff)
    End Function

    Private Const AesIV256 As String = "!QAZ2WSX#EDC4RFV"
    Private Const AesKey256 As String = "5TGB&YHN7UJM(IK<5TGB&YHN7UJM(IK<"

    'AES Encrypt
    Public Shared Function Encrypt256(text As String) As String
        ' AesCryptoServiceProvider
        Dim aes As New AesCryptoServiceProvider()

        If text = String.Empty Then Return text
        aes.BlockSize = 128
        aes.KeySize = 256
        aes.IV = Encoding.UTF8.GetBytes(AesIV256)
        aes.Key = Encoding.UTF8.GetBytes(AesKey256)
        aes.Mode = CipherMode.CBC
        aes.Padding = PaddingMode.PKCS7

        ' Convert string to byte array
        Dim src As Byte() = Encoding.Unicode.GetBytes(text)

        ' encryption
        Using encrypt As ICryptoTransform = aes.CreateEncryptor()
            Dim dest As Byte() = encrypt.TransformFinalBlock(src, 0, src.Length)
            ' Convert byte array to Base64 strings
            Return Convert.ToBase64String(dest)
        End Using
    End Function

    'AES decryption
    Public Shared Function Decrypt256(text As String) As String
        ' AesCryptoServiceProvider
        Dim aes As New AesCryptoServiceProvider()

        If text = String.Empty Then Return text
        aes.BlockSize = 128
        aes.KeySize = 256
        aes.IV = Encoding.UTF8.GetBytes(AesIV256)
        aes.Key = Encoding.UTF8.GetBytes(AesKey256)
        aes.Mode = CipherMode.CBC
        aes.Padding = PaddingMode.PKCS7

        ' Convert Base64 strings to byte array
        Dim src As Byte() = System.Convert.FromBase64String(text)

        ' decryption
        Using decrypt As ICryptoTransform = aes.CreateDecryptor()
            Dim dest As Byte() = decrypt.TransformFinalBlock(src, 0, src.Length)
            Return Encoding.Unicode.GetString(dest)
        End Using
    End Function

    Public Shared Sub PreProcess(ByVal DB As DBHelper, ByVal s As System.Web.UI.Page)
        'Check session timeout
        'If SessionCheck = True And s.Session("SessionInfo") Is Nothing Then
        's.Response.Redirect("/Account/Login.aspx")
        'Exit Sub
        'End If
        SetupControls(DB, s)
    End Sub


    Public Shared Sub PostProcess(ByVal s As System.Web.UI.Page, Optional ByVal ScrollByCursor As Boolean = False, Optional ByVal smallTextBox As Boolean = False, Optional ByVal PostByCursor As Boolean = False)
        'Register common java script
        'Dim script As String = File.ReadAllText(HttpContext.Current.Server.MapPath("Include/common.js"))
        's.ClientScript.RegisterStartupScript(s.GetType(), "Common", Script, True)
    End Sub


    Public Shared Function FocusedTextBox(ByVal s As System.Web.UI.Page) As String
        Return s.Request.Form("HIDDEN_FocusedTextBox")
    End Function


    Private Shared Sub SetupControls(ByVal DB As DBHelper, ByVal s As Control)
        Dim classname As String
        Dim dt As DataTable

        For Each c As Control In s.Controls
            classname = ""
            dt = Nothing

            If c.GetType Is GetType(TextBox) Then
                DirectCast(c, TextBox).Attributes("onFocus") &= "TextBoxOnFocus();"
                DirectCast(c, TextBox).Attributes("onBlur") &= "TextBoxOnBlur();"
            End If

            If c.GetType Is GetType(TextBox) Then classname = CType(c, TextBox).CssClass
            If c.GetType Is GetType(DropDownList) Then classname = CType(c, DropDownList).CssClass

            If c.GetType Is GetType(DropDownList) And dt IsNot Nothing Then
                With CType(c, DropDownList)
                    .DataSource = dt
                    .DataValueField = "code_id"
                    .DataTextField = "CODEDESC"
                    .DataBind()
                End With
            End If

            If c.Controls.Count > 0 Then SetupControls(DB, c)
        Next
    End Sub


    'Public Shared Function GetUserSetting(ByVal DB As DBHelper, ByVal SettingName As String, Optional ByVal NoSetValue As String = "") As String
    '    Return DB.GetScalarEx("select value from sys_user_setting where userid = :1 and settingname = :2", "", NoSetValue, GetUserID, SettingName)
    'End Function

    'Public Shared Sub MessageBoxSimple(ByVal message As String)
    '    Dim s As System.Web.UI.Page = HttpContext.Current.Handler
    '    Dim script As String

    '    script = "alert(""" & message & """);"
    '    s.ClientScript.RegisterStartupScript(s.GetType(), "MessageBox", script, True)
    'End Sub

    Public Shared Sub SetSubmitOnEnter(ByVal s As System.Web.UI.Page)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetSubmitOnEnter", "SubmitOnEnter = true;", True)
    End Sub

    Public Shared Sub SetReturnURL(ByVal s As System.Web.UI.Page, ByVal URL As String)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetReturnURL", "ReturnURL = '" & URL & "';", True)
    End Sub


    Public Shared Sub SetBgColor(ByVal s As System.Web.UI.Page, ByVal color As String)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetBg", "document.bgColor = '" & color & "';", True)
    End Sub


    Public Shared Sub SetFocus(ByVal s As System.Web.UI.Page, ByVal t As TextBox)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetFocus", "<script FOR=window EVENT=onload>document.form1." & t.UniqueID & ".focus();</script>", False)
        t.Attributes.Add("onfocusin", " select();")
    End Sub


    Public Shared Sub SetFocus(ByVal s As System.Web.UI.Page, ByVal t As Button)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetFocus", "<script FOR=window EVENT=onload>document.form1." & t.ID & ".focus();</script>", False)
    End Sub


    Public Shared Sub SetFocus(ByVal s As System.Web.UI.Page, ByVal t As DropDownList)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetFocus", "<script FOR=window EVENT=onload>document.form1." & t.ID & ".focus();</script>", False)
        t.Attributes.Add("onfocusin", " select();")
    End Sub


    Public Shared Sub SetFocus(ByVal s As System.Web.UI.Page, ByVal t As ListBox)
        s.ClientScript.RegisterStartupScript(s.GetType(), "SetFocus", "<script FOR=window EVENT=onload>document.form1." & t.ID & ".focus();</script>", False)
    End Sub


    Public Shared Function PrevPage(ByVal s As System.Web.UI.Page) As String
        Dim ret As String = ""

        If s.Request.ServerVariables("HTTP_REFERER") IsNot Nothing Then
            ret = s.Request.ServerVariables("HTTP_REFERER")
            If ret.IndexOf("/") > -1 Then ret = ret.Substring(ret.LastIndexOf("/") + 1)
        End If

        Return ret
    End Function


#Region "Config"

    Public Shared Function GetConfig(ByVal index As String) As String
        Select Case index.ToUpper
            Case "DATABASETYPE"
                Return "ORACLE"
            Case "TNSNAME"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\JetsWMS")
                Dim strCon As String = CStr(rKey.GetValue("DBServer"))
                rKey.Close()
                Return strCon
            Case "DBUSER"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\JetsWMS")
                Dim strUser As String = CStr(rKey.GetValue("DBUser"))
                rKey.Close()
                Return strUser
            Case "DBPASSWD"
                Dim rKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\KMS\JetsWMS")
                Dim strPasswd As String = CStr(rKey.GetValue("DBPasswd"))
                rKey.Close()
                Return strPasswd
        End Select
        Throw New Exception("NO CONFIG")
    End Function


#End Region


#Region "Utility"

    Public Shared Function MakeList(ByVal d As DataTable, ByVal Separator As String) As String
        Dim ret As String = ""
        For Each r As DataRow In d.Rows
            If ret <> "" Then ret &= Separator
            ret &= r(0)
        Next
        Return ret
    End Function


    'Public Shared Function Val(ByVal s As Object) As Decimal
    '    Dim ret As Decimal
    '    Try
    '        If s Is Nothing Then Return 0
    '        If IsDBNull(s) Then Return 0
    '        If Decimal.TryParse(s, ret) = False Then Return 0
    '        Return ret
    '    Catch
    '        Return 0
    '    End Try
    'End Function

    Public Shared Function Val(ByVal v As Object) As Decimal
        Return Val(v, False)
    End Function


    Public Shared Function Val(ByVal v As Object, ByVal RemoveNonNumericString As Boolean) As Decimal
        Dim i As Integer
        Dim s, ret As String

        If v Is Nothing Then Return 0
        If IsDBNull(v) Then Return 0

        If RemoveNonNumericString = False Then
            If Not IsNumeric(v) Then Return 0
            Return CDec(v)
        Else
            s = v.ToString
            ret = ""
            For i = 0 To s.Length - 1
                If IsNumeric(s.Substring(i, 1)) = True Or _
                   (ret = "" And (s.Substring(i, 1) = "+" Or s.Substring(i, 1) = "-")) Or _
                   (ret.IndexOf("."c) = -1 And s.Substring(i, 1) = ".") Then
                    ret &= s.Substring(i, 1)
                End If
            Next
            If Not IsNumeric(ret) Then Return 0
            Return CDec(ret)
        End If
    End Function

    Public Shared Function CString(ByVal v As Object) As String
        Dim ret As String

        Try
            ret = v.ToString
        Catch ex As Exception
            ret = ""
        End Try
        Return ret
    End Function

    Public Shared Function L(ByVal s As String) As String
        Return s
    End Function


    Public Shared Function DateFormat(ByVal d As Object) As String
        If IsDBNull(d) Or d Is Nothing Then Return ""
        If Year(d) < 1910 Then Return ""
        Return CType(d, Date).ToString(GetSysParam("DATEFORMAT_FULL"))
    End Function


    Public Shared Function DateTimeFormat(ByVal d As Object) As String
        If IsDBNull(d) Or d Is Nothing Then Return ""
        Return CType(d, Date).ToShortDateString & " " & CType(d, Date).ToShortTimeString
    End Function


    Public Shared Function GetSessionInfo(ByVal s As System.Web.UI.Page) As SessionInfo
        Return CType(s.Session("SessionInfo"), SessionInfo)
    End Function

    Public Shared Function FoldString(ByVal s As String, ByVal foldlen As Integer, ByVal linenum As Integer) As String
        Dim i, p As Integer
        Dim line As String = ""

        For i = 1 To linenum
            s = Trim(s)
            If s.Length < foldlen Then
                line = s
                Exit For
            End If
            p = s.Substring(0, foldlen).LastIndexOf(" ")
            If p < 0 Then
                line = s.Substring(0, foldlen)
                s = ""
            Else
                line = s.Substring(0, p)
                s = s.Substring(p + 1)
            End If
        Next

        Return line
    End Function

    'Convert from object to string
    'Public Shared Function CString(ByVal value As Object) As String
    '    If value Is Nothing Or IsDBNull(value) Then
    '        Return ""
    '    Else
    '        Return value.ToString().Trim
    '    End If
    'End Function

    ''Convert from object to int
    'Public Shared Function objToInt(ByVal value As Object) As Integer
    '    If value Is Nothing Or IsDBNull(value) Then
    '        Return Nothing
    '    ElseIf value.ToString = "" Or IsNumeric(value) = False Then
    '        Return Nothing
    '    Else
    '        Return CInt(Fix(value))
    '    End If
    'End Function

    ''Convert from object to double
    'Public Shared Function objToDbl(ByVal value As Object) As Double
    '    If value Is Nothing Or IsDBNull(value) Then
    '        Return Nothing
    '    ElseIf value.ToString = "" Or IsNumeric(value) = False Then
    '        Return Nothing
    '    Else
    '        Return CDbl(value)
    '    End If
    'End Function


    Public Shared Function AddPrefixZero(ByVal str As String, ByVal fullLen As Integer) As String
        Return StrDup(IIf(fullLen - str.Length() > 0, fullLen - str.Length(), 0), "0") & str
    End Function

    'Remove all the 0s as prefix 
    Public Shared Function RemovePrefixZero(ByVal str As String) As String
        For i As Integer = 0 To str.Length - 1
            If str(i) = "0"c Then
                'Skip 0s
                Continue For
            Else
                Return Microsoft.VisualBasic.Right(str, str.Length - i)
            End If
        Next
        'If str only contains 0
        Return ""
    End Function

    Public Shared Sub MergeGrdCells(ByVal grd As GridView, ByVal colNr As Integer, Optional ByVal canSelectRow As Boolean = False)
        Dim r As Integer
        Dim curRow, preRow As GridViewRow

        r = grd.Rows.Count - 2
        While r >= 0
            curRow = grd.Rows(r)
            preRow = grd.Rows(r + 1)
            If canSelectRow = True Then curRow.Cells(colNr).BackColor = Drawing.Color.White
            'Only merge cell for colNr th column
            If curRow.Cells(colNr).Text <> "" AndAlso curRow.Cells(colNr).Text = preRow.Cells(colNr).Text Then
                If preRow.Cells(colNr).RowSpan < 2 Then
                    curRow.Cells(colNr).RowSpan = 2
                    preRow.Cells(colNr).Visible = False
                Else
                    curRow.Cells(colNr).RowSpan = preRow.Cells(colNr).RowSpan + 1
                    preRow.Cells(colNr).Visible = False
                End If
            End If
            r -= 1
        End While
    End Sub

    Public Shared Sub SetupDropDownList(ByVal DB As DBHelper, ByVal list As DropDownList, ByVal CodeType As String, ByVal EmptyEntry As Boolean)
        Dim scope As String
        Dim dt As DataTable

        scope = DB.GetScalar("select SCOPE from CODE_CODETYPE where CODETYPE = :1", CodeType)
        Select Case scope
            Case "C"
                dt = DB.GetDataTable("select code_id, CODEDESC from CODE where CODETYPE=:1 and cid=:2 order by DISPORDER", CodeType, GetCID)
            Case "B"
                dt = DB.GetDataTable("select code_id, CODEDESC from CODE where CODETYPE=:0 and cid=:1 and bid=:2 order by DISPORDER", CodeType, GetCID, GetBID)
            Case Else
                dt = DB.GetDataTable("select code_id, CODEDESC from CODE where CODETYPE=:0 order by DISPORDER", CodeType)
        End Select
        If EmptyEntry = True Then
            dt.Rows.InsertAt(dt.NewRow(), 0)
            dt.Rows(0).Item(0) = ""
            dt.Rows(0).Item(1) = ""
        End If
        list.DataSource = dt
        list.DataValueField = "code_id"
        list.DataTextField = "CODEDESC"
        list.DataBind()
    End Sub


    Public Shared Sub SetupDropDownList(ByVal DB As DBHelper, ByVal list As DropDownList, ByVal EmptyEntry As Boolean, ByVal SQL As String, ByVal ParamArray args() As Object)
        Dim dt As DataTable
        dt = DB.GetDataTable(SQL, args)
        list.DataTextField = dt.Columns(0).ColumnName
        If dt.Columns.Count = 1 Then
            list.DataValueField = dt.Columns(0).ColumnName
        Else
            list.DataValueField = dt.Columns(1).ColumnName
        End If
        If EmptyEntry Then
            dt.Rows.InsertAt(dt.NewRow(), 0)
            dt.Rows(0).Item(0) = ""
            If dt.Columns.Count > 1 Then dt.Rows(0).Item(1) = ""
        End If
        list.DataSource = dt
        list.DataBind()
    End Sub


#End Region
    Public Shared Sub NoEnterSubmit(f As System.Web.UI.HtmlControls.HtmlForm)
        f.Attributes("onkeydown") = HttpUtility.HtmlEncode("if(event.keyCode==13){ if(window.event.srcElement.type!='submit' && window.event.srcElement.type!='textarea'){return false;}}")
    End Sub

    Public Shared Function SendMail(ByVal FromAddress As String, ByVal ToAddress As String, ByVal ReplyToAddress As String, ByVal Subject As String, ByVal Body As String, ByVal Attachment() As String, _
                                    ByVal IsBodyHtml As Boolean, ByRef ErrMsg As String, Optional ByVal CcAddress As String = Nothing) As Integer
        Dim m As New MailMessage
        Dim emailAry As String()

        'Init
        ErrMsg = String.Empty
        If ReplyToAddress = String.Empty Then ReplyToAddress = GetSupportEmail()

        'From address
        If FromAddress = "" Then
            ErrMsg = "From address is not specified"
            Return 1
        End If
        Try
            m.From = New MailAddress(FromAddress)
        Catch ex As Exception
            ErrMsg = "From address (" & FromAddress & ") is incorrect (exception: " & ex.Message & ")"
            Return 2
        End Try

        'To address
        If ToAddress = "" Then
            ErrMsg = "Mail recepient is not specified"
            Return 3
        End If
        Try
            If IsProduction() OrElse IsProdBeta() Then
                emailAry = ToAddress.Split(New Char() {","c})
                For Each email In emailAry
                    If email.Trim <> "" Then m.To.Add(New MailAddress(email.Trim))
                Next
                'm.To.Add(New MailAddress(ToAddress))
            Else
                m.To.Add(New MailAddress("jzhou@kmsnet.com"))
                m.To.Add(New MailAddress("yfu@kmsnet.com"))
            End If
        Catch ex As Exception
            ErrMsg = "To address (" & ToAddress & ") is incorrect (exception: " & ex.Message & ")"
            Return 4
        End Try

        If ReplyToAddress <> "" Then
            'Reply to address
            Try
                emailAry = ReplyToAddress.Split(New Char() {","c})
                For Each email In emailAry
                    If email.Trim <> "" Then m.ReplyToList.Add(New MailAddress(email.Trim))
                Next
                'm.ReplyToList.Add(New MailAddress(ReplyToAddress))
            Catch ex As Exception
                ErrMsg = "Reply To address (" & ReplyToAddress & ") is incorrect (exception: " & ex.Message & ")"
                Return 5
            End Try
        End If

        'Subject, body and bcc
        m.Subject = Subject
        m.Body = Body
        m.IsBodyHtml = IsBodyHtml
        'If CcAddress <> String.Empty Then m.CC.Add(New MailAddress(CcAddress))
        If CcAddress <> "" Then
            'CC address
            Try
                emailAry = CcAddress.Split(New Char() {","c})
                For Each email In emailAry
                    If email.Trim <> "" Then m.CC.Add(New MailAddress(email.Trim))
                Next
                'm.ReplyToList.Add(New MailAddress(ReplyToAddress))
            Catch ex As Exception
                ErrMsg = "CC address (" & CcAddress & ") is incorrect (exception: " & ex.Message & ")"
                Return 5
            End Try
        End If
        If IsProduction() OrElse IsProdBeta() Then
            m.Bcc.Add(New MailAddress("weblog@jfc.com"))
            m.Bcc.Add(New MailAddress("ml2@kmsnet.com"))
        End If

        'Attachement
        If Attachment IsNot Nothing Then
            For Each s As String In Attachment
                Try
                    m.Attachments.Add(New Attachment(s))
                Catch ex As Exception
                    ErrMsg = "Attachment (" & s & ") error: " & ex.Message
                    Return 6
                End Try
            Next
        End If

        'Send email
        'Try
        '    Dim sc As New SmtpClient("smtp.gmail.com")
        '    sc.Credentials = New System.Net.NetworkCredential("ml@kmsnet.com", "umedaken")
        '    sc.EnableSsl = True
        '    sc.Port = 587
        '    sc.Send(m)
        '    Return 0
        'Catch ex As Exception
        '    'ErrMsg = "Sending email caused error (" & ex.Message & ")"
        '    'Return 7
        'End Try
        ''Send failed. Try again.
        Try
            'Dim sc As New SmtpClient("smtp.gmail.com")
            'sc.Credentials = New System.Net.NetworkCredential("ml2@kmsnet.com", "umedaken")
            'sc.EnableSsl = True
            'sc.Port = 587
            Dim sc As New System.Net.Mail.SmtpClient("smtp3.kmsnet.com")
            sc.Port = 25
            sc.Send(m)
            Return 0
        Catch ex As Exception
            ErrMsg = "Sending email caused error (" & ex.Message & ")"
            Return 7
        End Try
    End Function

    Public Shared Function SendText(ByVal DB As DBHelper, ByVal fromText As String, ByVal toText As String, ByVal textBody As String, ByRef errMsg As String) As Integer
        Dim newToText As String = toText
        Dim SQL As String

        'Init
        errMsg = String.Empty

        'Conver ToText to correct format
        newToText = newToText.Replace(" ", "")
        newToText = newToText.Replace("(", "")
        newToText = newToText.Replace(")", "")
        newToText = newToText.Replace("-", "")
        newToText = "+" & IIf(newToText.Length = 10, "1", "") & newToText
        'ToText = "+16269278049" for testing

        'Check error
        If newToText.Length <> 12 Then
            errMsg = "Cell phone # " & toText & " is invalid"
            Return 1
        End If
        SQL = "SELECT cellular FROM DoNotText WHERE cellular = :1"
        If CString(DB.GetScalar(SQL, toText)) <> String.Empty Then
            errMsg = "Cell phone # " & toText & " is in Do Not Text list"
            Return 2
        End If

        Try
            'Send text
            System.Net.ServicePointManager.SecurityProtocol = DirectCast(3072, System.Net.SecurityProtocolType) 'Enable TLS 1.2 
            'System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12 'Not supported in .NET 4.0
            TwilioClient.Init(TEXT_ACCOUNT_ID, TEXT_AUTH_TOKEN)
            Dim message As MessageResource = MessageResource.Create(
                body:=textBody,
                from:=New Twilio.Types.PhoneNumber(fromText),
                to:=New Twilio.Types.PhoneNumber(toText)
            )
            Return 0
        Catch ex As Exception
            errMsg = ex.Message()
            DBUpdate.InsertDoNoText(DB, toText, errMsg)
            Return 3
        End Try
    End Function

    'ADD By YI FU 9/4/2020-- Function  setPrice  customer_custItem
    Public Shared Function SetPrice(ByVal PriceUnit As String, Price As Decimal, ItemID As String, customerID As String) As String

        Dim sb As New StringBuilder
        sb.Append("SELECT p.eaPerCs, p.minSalCnt, p.scaledItem, p.avgCsWgt, p.avgEaWgt, p.outOfStock, p.arrivDate, p.csAvail, p.eaAvail, NVL(p.btmcsPrice, 0) AS btmcsPrice, ")
        sb.Append("NVL(p.csCost, 0) AS csCost,")
        If PriceUnit = "CS" Then


            sb.AppendFormat(" {0} AS csPrice, CASE WHEN p.eaPerCs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
                      "WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs != 1 THEN ROUND({0} / (p.eapercs * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1))) * nvl(p.avgeawgt, 0.9), 2) " & _
                      "WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN " & _
                      "ROUND( {0} / nvl(p.avgcswgt, 0) * nvl(p.avgeawgt, 0.9),2) " & _
                      "ELSE ROUND({0} / p.eaPerCs, 2) END AS eaPrice, CASE WHEN p.scaledItem = 'N' OR p.eapercs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
                      "WHEN p.scaledItem = 'Y' and DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN ROUND( {0} *(p.eapercs*p.lbsperea)/ nvl(p.avgcswgt, 0),2) " & _
                      "ELSE ROUND({0} / (p.eaPerCs * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1))), 2) END AS lbPrice_orderByQty, " & _
                      "Case WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs != 1 Then p.eapercs * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) / nvl(p.avgeawgt, 0.9) " & _
                      "WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 Then nvl(p.avgcswgt, 0)/nvl(p.avgeawgt, 0.9) ELSE 0 END as psPerCs ", Price)

        ElseIf PriceUnit = "EA" Then
            sb.AppendFormat("CASE WHEN p.scaledItem = 'Y' and DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN 0 " & _
                    "  ELSE ROUND( {0} * p.eapercs, 2) END AS csPrice, " & _
                    " {0} AS eaPrice, CASE WHEN p.scaledItem = 'N' OR p.eapercs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
                    "  ELSE ROUND({0} / DECODE(nvl(p.lbsPerEa, 0.1),0,0.1,nvl(p.lbsPerEa, 0.1)) , 2) END AS lbPrice_orderByQty ", Price)

        ElseIf PriceUnit = "LB" Then
            sb.AppendFormat("CASE WHEN p.scaledItem = 'Y' and DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 THEN {0}/(p.eapercs*p.lbsperea)* nvl(p.avgcswgt, 0) " & _
                    " ELSE ROUND({0} * p.eapercs * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)), 2) END AS csPrice, " & _
                    " CASE WHEN p.eaPerCs / DECODE(p.minsalcnt,0,1,p.minsalcnt) <= 1 THEN 0 " & _
                    "  WHEN p.scaledItem = 'Y' THEN ROUND({0} * nvl(p.avgeawgt, 0.9), 2) " & _
                    "  ELSE ROUND({0} * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)), 2) END AS eaPrice, " & _
                     "{0} AS lbPrice_orderByQty, " & _
                     "Case WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs != 1 Then " & _
                     "(p.eapercs * DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) / nvl(p.avgeawgt, 0.9)) " & _
                     "WHEN p.scaledItem = 'Y' AND DECODE(p.lbsPerEa,0,0.1,nvl(p.lbsPerEa, 0.1)) * p.eaPerCs = 1 Then " & _
                     "nvl(p.avgcswgt, 0)/nvl(p.avgeawgt, 0.9) ELSE 0 END as psPerCs ", Price)
        Else
            sb.Append("0 as csPrice, 0 as eaPrice, 0 as lbPrice_orderByQty ")

        End If

        sb.AppendFormat(" FROM jets_price p, jets_customer jc, customer c WHERE jc.bid = c.jfcBranchNr And jc.customerid = c.jfcCustomerNr " & _
                        "  AND p.pricegpid = jc.pricegpid " & " AND p.itemcode = {0} AND c.customer_id = {1}", ItemID, customerID)

        Return sb.ToString

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
    End Function

    Public Shared Function IsAllNull(ParamArray obj() As Object) As Boolean
        For Each o In obj
            If TypeOf o Is String Then
                If Not String.IsNullOrWhiteSpace(o) Then
                    Return False
                End If
            End If
            If TypeOf o Is Decimal OrElse TypeOf o Is Double OrElse TypeOf o Is Integer Then
                If o <> 0 Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    Public Shared Sub ClearSubControl(ByVal c As Control)
        If c.HasControls Then
            For Each subcontrol As Control In c.Controls
                If TypeOf subcontrol Is TextBox Then
                    CType(subcontrol, TextBox).Text = ""
                ElseIf TypeOf subcontrol Is Label Then
                    CType(subcontrol, Label).Text = ""
                End If


            Next
        End If
    End Sub

    Public Shared Sub ShowList(ByVal DB As DBHelper, ByVal sql As String, ByRef lst As DropDownList, _
                                      ByVal nameOnly As Boolean, Optional ByVal insertFirstLine As Boolean = False, Optional ByVal firstLineStr As String = "")

        Dim dt As DataTable
        Dim dr As DataRow



        dt = DB.GetDataTable(sql)
        If insertFirstLine = True Then
            dr = dt.NewRow
            dr("name") = firstLineStr
            dt.Rows.InsertAt(dr, 0)
        End If
        lst.DataSource = dt
        If nameOnly = True Then
            lst.DataValueField = "name"
        Else
            lst.DataValueField = "id"
            lst.DataTextField = "name"
        End If
        lst.DataBind()
    End Sub

    Public Shared Function Get_GP(eaPerCs As Integer, csCost As Decimal, lbPrice_orderByQty As Decimal, csPrice As Decimal) As String
        'Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"
        Dim lbCost As Decimal = If(eaPerCs = 0, 0, Math.Round(csCost / eaPerCs * 10, 4))
        Dim result As String = ""
        If lbPrice_orderByQty <> 0 Then
            result = "GP: " & Math.Round((lbPrice_orderByQty - lbCost) / lbPrice_orderByQty, 4).ToString("P1")
        ElseIf csPrice = 0 Then
            result = ""
        Else
            result = "GP: " & Math.Round((csPrice - csCost) / csPrice, 4).ToString("P1")
        End If


        Return result

    End Function

    Public Shared Function Displayformat(OrderByQty As Decimal, cs As Decimal, ea As Decimal, scaledItem As Boolean) As String
        Return IIf(OrderByQty > 0 AndAlso scaledItem = True, String.Format("{0:C}", OrderByQty) & " / lb, ", "") & _
                                       IIf(cs > 0, String.Format("{0:C}", cs) & " / cs, ", "") & _
                                       IIf(ea > 0, String.Format("{0:C}", ea) & " / " & IIf(scaledItem, "pc", "ea"), "") 'Don't show cost / pc

    End Function
    Public Shared Function Displayformatgdv(OrderByQty As Decimal, cs As Decimal, ea As Decimal, scaledItem As Boolean) As String
        Return IIf(OrderByQty > 0 AndAlso scaledItem = True, String.Format("{0:C}", OrderByQty) & "/lb<br />", "") & _
                                       IIf(cs > 0, String.Format("{0:C}", cs) & "/cs<br />", "") & _
                                       IIf(ea > 0, String.Format(" {0:C}", ea) & "/" & IIf(scaledItem, "pc", "ea"), "") 'Don't show cost / pc

    End Function
    Public Shared Function foldlongString(str As String, breakChar() As Char, Optional maxStrlen As Integer = 40) As String
        Dim strs = str.Split(breakChar, maxStrlen)

        Dim i As Integer = 1
        Dim result As New StringBuilder
        Dim newline As String = ""
        Dim addchar = " "
        If breakChar.Count = 1 Then
            If breakChar(0) = "," Then
                addchar = ","
            End If
        End If
        For Each s As String In strs
            newline &= s & addchar
            If newline.Length > maxStrlen And s.Length < maxStrlen - 10 Then
                result.Append(newline & "<br />")
                newline = String.Empty
            ElseIf s.Length > maxStrlen - 10 Then
                result.Append(newline.Replace(s, "<br />") & s & "<br />")

                newline = String.Empty
            End If
        Next
        If newline.LastOrDefault = "," Then newline = newline.Remove(newline.Length - 1)
        result.Append(newline)
        Return result.ToString
    End Function

    'YI FU 10/30/2020
    Public Shared Function randomString(ByVal length As Integer, Optional pool As String = "abcdefghijklmnopqrstuvwxyz0123456789") As String
        Dim rand As Random = New Random()
        Dim builder As StringBuilder = New StringBuilder()
        For i = 0 To length - 1
            Dim c As String = pool(rand.Next(0, pool.Length))
            builder.Append(c)
        Next
        Return builder.ToString()
    End Function

    Public Shared Function verifyotp(ByVal DB As DBHelper, ByVal email As String, OTPcode As String, ByRef rtnMessage As String) As Boolean


        Dim Sql = "select email from webuser_otp where email = :1 and otp = :2 and EXPIREDATE >= sysdate"
        If CString(DB.GetScalar(Sql, email, OTPcode)) = String.Empty Then
            rtnMessage = "Secure verification code is not correct ."
            Return False
        End If
        DBUpdate.UpdateVerifyOTP(DB, email, OTPcode)
        InsertEventLog(DB, "Verify one time password", email, OTPcode, Nothing, True)
        Return True
    End Function
    'YI FU 10/30/2020
    Public Shared Function GetNrDeviceCode(ByVal DB As DBHelper, ByVal email As String, ByVal DeviceCode As String) As Integer
        Dim SQL As String
        Dim nDevice As Integer
        If DeviceCode Is Nothing Then
            Return 0
        End If

        SQL = "SELECT COUNT(*) FROM webuser_otp WHERE enabled = 'N' and email =:1 AND DeviceCode = :2"
        nDevice = Val(DB.GetScalar(SQL, email, DeviceCode))

        Return nDevice
    End Function

    Public Shared Function GetDeleteCustomer_Item(ByVal DB As DBHelper, CustomerID As String, custGrpItemID As String) As Integer
        Dim SQL As String
        Dim EnabledNr As Integer


        SQL = "SELECT COUNT(*) FROM CUSTOMER_CUSTITEM WHERE ENABLED = 'N' and CUSTOMER_ID =:1 AND CUSTGRPITEM_ID = :2"
        EnabledNr = Val(DB.GetScalar(SQL, CustomerID, custGrpItemID))

        Return EnabledNr
    End Function

    Public Shared Function GetImgName(ByVal DB As DBHelper, ByVal HomepageImgID As String, BranchID As String) As String
        Dim SQL As String
        Dim IMGNAME As String
        Dim custGrpID As String = ""


        SQL = "SELECT IMGNAME FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE HOMEPAGEIMG_ID =:1 AND JFCBRANCHNR = :2"
        IMGNAME = CString(DB.GetScalar(SQL, HomepageImgID, BranchID))

        Return IMGNAME
    End Function

    Public Shared Sub CovertPrice(ByRef CsPrice As Decimal, ByRef eaPrice As Decimal, ByRef lbPrice As Decimal,
                                eaPerCs As Decimal, minSalCnt As Decimal, scaledItem As Boolean, lbsPerEa As Decimal, avgcswgt As String, avgeawgt As String)
        minSalCnt = IIf(minSalCnt = 0, 1, minSalCnt)
        Dim OcsPrice = Val(CsPrice)

        If scaledItem Then
            If lbsPerEa * Val(eaPerCs) = 1 Then
                CsPrice = Val(CsPrice) * Val(avgcswgt) '0 'Mod on 8/31/2020
            End If


            If Val(eaPerCs) / minSalCnt <= 1 Then
                eaPrice = 0
            ElseIf Val(avgcswgt) < Val(avgeawgt) * 2 Then 'Add on 8/31/2020
                eaPrice = 0
            ElseIf eaPerCs = 10 Then
                eaPrice = Math.Round(OcsPrice * IIf(avgeawgt = 0, 0.9, avgeawgt), 2)
            Else
                eaPrice = Math.Round(Math.Round(OcsPrice / Val(eaPerCs), 3) * IIf(Val(avgeawgt) = 0, 0.9, Val(avgeawgt)) * 10, 2)
            End If
            If Val(eaPerCs) / minSalCnt <= 1 Then
                lbPrice = 0
            Else
                lbPrice = IIf(eaPerCs = 0, 0, Math.Round(Val(OcsPrice) / Val(eaPerCs) * 10, 2))
            End If

            'If Val(r("lbPrice_OrderByQty")) = 0 Then gp = 1 Else gp = (Val(r("lbPrice_OrderByQty")) - lbCost) / Val(r("lbPrice_OrderByQty"))

        Else
            'Regular(Item)


            If Val(eaPerCs) / minSalCnt <= 1 Then
                eaPrice = 0
            Else
                eaPrice = Math.Round(Val(CsPrice) / Val(eaPerCs), 2)
            End If
            lbPrice = 0

            'If Val(r("csprice")) = 0 Then gp = 1 Else gp = (Val(r("csprice")) - csCost) / Val(r("csprice"))

        End If

    End Sub


    Public Shared Function GetColumnIndexByName(ByVal row As GridViewRow, ByVal columnName As String) As Integer
        Dim columnIndex As Integer = 0

        For Each cell As DataControlFieldCell In row.Cells

            If TypeOf cell.ContainingField Is BoundField Then
                If (CType(cell.ContainingField, BoundField)).DataField.Equals(columnName) Then Exit For
            End If

            columnIndex += 1
        Next

        Return columnIndex
    End Function



    Public Shared Function WrapText(ByVal text As String, ByVal number As Integer, Optional NewLineText As String = "<br />") As String

        Dim originalLines As String() = text.Split(New String() {" "}, StringSplitOptions.None)

        Dim wrappedLines As String = ""
        Dim actualLine As StringBuilder = New StringBuilder()
        Dim actualWidth As Double = 0
        For Each Item As String In originalLines
            actualLine.Append(Item & " ")
            actualWidth += Item.Length + 1
            If actualWidth > number Then
                wrappedLines &= actualLine.ToString() & NewLineText
                actualLine.Clear()
                actualWidth = 0
            End If


        Next
        If actualLine.Length > 0 Then
            wrappedLines &= actualLine.ToString() & NewLineText

        End If

        Return wrappedLines

    End Function

    Public Shared Function convertToLocalTab(ByVal tab As [Enum]) As Integer

        Return Convert.ToInt32(tab)
    End Function
End Class


Public Class PasswordSetting
    'password age , 80, 180, 360 days
    Public Duration As Integer

    'password minimum length
    Public MinLength As Integer

    'password maximum length
    Public MaxLength As Integer

    'password Numbers length
    Public NumsLength As Integer

    'password Upper letter length
    Public UpperLength As Integer

    'password Special character length
    Public SpecialLength As Integer

    'password valid special characters
    Public SpecialChars As String

    Public Sub New()
        Dim xmlDoc As XmlDocument = New XmlDocument
        xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Login/PasswordPolicy.xml"))
        For Each node As XmlNode In xmlDoc.ChildNodes
            For Each node2 As XmlNode In node.ChildNodes
                Duration = Integer.Parse(node2("duration").InnerText)
                MinLength = Integer.Parse(node2("minLength").InnerText)
                MaxLength = Integer.Parse(node2("maxLength").InnerText)
                NumsLength = Integer.Parse(node2("numsLength").InnerText)
                SpecialLength = Integer.Parse(node2("specialLength").InnerText)
                UpperLength = Integer.Parse(node2("upperLength").InnerText)
                SpecialChars = node2("specialChars").InnerText
            Next
        Next
    End Sub



End Class

<Serializable()>
Public Class CustomerInfo
    Property customerID As String
    Property jfcBranchNr As String = ""
    Property jfcCustomerNr As String = ""
    Property custGrpID As String
    Property nrCustGrpVendor As Integer
    Property isSmallCust As String
    Property useCustomerCtg As Boolean
    Property IsExcepPriceCustomer As Boolean
    Property PriceGrpID As String
    Property IsApplyInCustomer As Boolean
    Sub New()

    End Sub


    Sub New(_customerID As String)
        Using db As New DBHelper
            customerID = _customerID
            Common.GetJFCBranchAndCustomerNr(db, _customerID, jfcBranchNr, jfcCustomerNr)
            custGrpID = Common.GetCustGrpID(db, _customerID)
            nrCustGrpVendor = Common.GetNrCustGrpVendor(db, custGrpID)
            isSmallCust = Common.IsSmallCustomer(db, customerID)
            useCustomerCtg = Common.UseCustomerCtg(db, customerID)
            IsExcepPriceCustomer = Common.IsExcepPriceCustomer(db, customerID)
            PriceGrpID = Common.GetPriceGrpID(db, jfcBranchNr, jfcCustomerNr)
            IsApplyInCustomer = Common.ApplyInCustomer(db, customerID, "ShowAllExcepGrpItem")
        End Using
    End Sub

    Sub New(_custGrpID As String, ByVal IsCustGrpID As Boolean)
        Using db As New DBHelper
            custGrpID = _custGrpID
            'Common.GetJFCBranchAndCustomerNr(db, _customerID, jfcBranchNr, jfcCustomerNr)
            custGrpID = Common.GetCustGrpID(db, _customerID)
            nrCustGrpVendor = Common.GetNrCustGrpVendor(db, custGrpID)
            isSmallCust = Common.IsSmallCustomer(db, customerID)
            useCustomerCtg = Common.UseCustomerCtg(db, customerID)
            IsExcepPriceCustomer = Common.IsExcepPriceCustomer(db, customerID)
            PriceGrpID = Common.GetPriceGrpID(db, jfcBranchNr, jfcCustomerNr)
            IsApplyInCustomer = Common.ApplyInCustomer(db, customerID, "ShowAllExcepGrpItem")
        End Using
    End Sub

End Class
