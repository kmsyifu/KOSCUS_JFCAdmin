#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","7A06CC9E22D530D51A8D02258D02562AF27DEF43")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\Login.aspx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate

Partial Class Login_Login
    Inherits System.Web.UI.Page

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim r As DataRow
        Dim dr As DataReader
        Dim SessionID As Integer
        Dim ResetPassword As Boolean = False
        Dim SQL As String
        Dim customerID As String = ""
        'Dim vendorID As String = ""
        Dim vendorCompanyID As String = ""
        Dim email As String
        Dim usedFlatPassword As Boolean = False

        'Show msg such as time out
        If Session("MSG") <> "" Then
            If Session("MSG_TYPE") <> "" Then
                'MessageLine(Session("MSG_TYPE"), Session("MSG"))
                MessageSimpleBox(Session("MSG"))
                Session("MSG_TYPE") = ""
            Else
                MessageLine("CONFIRM", Session("MSG"))
            End If
            Session("MSG") = ""
        End If

        'Check if user clicked a link from their email
        If Request.QueryString("cmd") <> "" Then
            Session("CONFIRM_CMD") = Request.QueryString("cmd")
            Session("CONFIRM_ID") = Request.QueryString("id")
            Session("CONFIRM_PASSWORD") = Request.QueryString("password")
            Response.Redirect("Confirm.aspx")
        End If

        DataBind()

        'Show notification if any
        pnlNotice.Visible = IIf(LOGIN_NOTICE = String.Empty, False, True)

        'Get arguments
        ViewState("CUSTCOMPANYID") = Request.QueryString("CUSTCOMPANYID")
        ViewState("USERNAME") = Request.QueryString("USERNAME")
        ViewState("PASSWORD") = Request.QueryString("PASSWORD")
        ViewState("HASHED") = Common.CString(Request.QueryString("HASHED")).ToUpper
        ViewState("AUTO_LOGIN") = Common.CString(Request.QueryString("AUTO_LOGIN")).ToUpper
        ViewState("USER_TYPE") = Common.CString(Request.QueryString("TYPE")).ToUpper
        ViewState("PAGE") = Request.QueryString("PAGE")
        ViewState("RETURN_URL") = Request.QueryString("ReturnUrl")
        ViewState("SITE_MODE") = Session("SITE_MODE")

        If ViewState("CUSTCOMPANYID") <> "" Then
            'Login as a administrator
            lblCmd.Text = "Please re-enter your password to login as an administrator..."
            btnLogin.Text = "Login as Administrator"
            txtUserName.Text = ViewState("USERNAME")
        End If

        'Version check
        'Dim version As String = System.IO.File.ReadAllText("c:\inetpub\wwwroot\koscus_version.txt").ToUpper
        'If version.IndexOf(vbCrLf) > -1 Then version = version.Substring(0, version.IndexOf(vbCrLf))
        'If Request.ApplicationPath.ToUpper <> "/" & version Then Response.Redirect("/" & version & "/Login/Login.aspx")

        Using DB As New DBHelper()

            'iPad
            'If Request.UserAgent.ToLower.Contains("ipad") Or Request.QueryString("ipad") IsNot Nothing Then
            '    Response.Redirect("iPad_login.aspx")
            '    Exit Sub
            'End If

            'Dump all previous session
            Session.Clear()

            'Restore session
            Session("SITE_MODE") = ViewState("SITE_MODE")

            'If arguments are set (reset password)
            If ViewState("USERNAME") IsNot Nothing And ViewState("PASSWORD") IsNot Nothing Then
                txtUserName.Text = ViewState("USERNAME")
                txtPassword.Text = ViewState("PASSWORD")
                chkRememberMe.Checked = IsMobile()
            Else
                'RememberMe by cookie
                If IsPostBack = False And Request.Cookies("KOSCUS_EMAIL") IsNot Nothing Then
                    txtUserName.Text = Request.Cookies("KOSCUS_EMAIL").Value
                    txtPassword.Attributes("value") = Request.Cookies("KOSCUS_PASSWD").Value
                    If ViewState("AUTO_LOGIN") = "Y" Then txtPassword.Text = txtPassword.Attributes("value")
                    chkRememberMe.Checked = True
                End If

                'YI FU 10/30/2020

                'If IsPostBack = False And Request.Cookies("KOSCUS_LOGIN") IsNot Nothing Then
                '    txtUserName.Text = Request.Cookies("KOSCUS_LOGIN")("RememberMe")
                '    chkRememberMe.Checked = True
                'End If
            End If

            If Not IsPostBack And Request.Cookies("KOSCUS_DEVICECODE") IsNot Nothing Then
                ViewState("DeviceCode") = Request.Cookies("KOSCUS_DEVICECODE").Value
            End If
            'Focus set
            If txtUserName.Text = "" Then
                txtUserName.Focus()
                Exit Sub
            End If

            If txtPassword.Text = "" Then
                txtPassword.Focus()
                Exit Sub
            End If

            'Password check
            email = CString(txtUserName.Text).Trim.ToLower
            r = DB.GetDataRow("select * from webuser where email = :email", email)
            If r Is Nothing Then GoTo fail
            If Val(r("user_id")) <= 0 Then GoTo fail

            'Check flat password, onetime password, and regular password
            If GetUserParam("PROFILE", "FLATPWD", r("user_id")) <> "" AndAlso GetUserParam("PROFILE", "FLATPWD", r("user_id")) = txtPassword.Text.Trim Then GoTo PASS
            If GetUserParam("PROFILE", "ONETIMEPWD", r("user_id")) <> "" AndAlso GetUserParam("PROFILE", "ONETIMEPWD", r("user_id")) = txtPassword.Text.Trim Then
                DB.ExecuteNonQuery("delete from sys_app_userparam where appid='PROFILE' and paramid='ONETIMEPWD' and userid=:1", r("user_id"))
                ResetPassword = True
                GoTo pass
            End If
            'If IsDBNull(r("flatpassword")) = False AndAlso r("flatpassword") = txtPassword.Text.Trim Then GoTo PASS
            'If IsDBNull(r("password")) = False AndAlso IsDBNull(r("salt")) = False AndAlso r("password") = Common.CreatePasswordHash(txtPassword.Text.Trim, r("salt")) Then GoTo PASS
            'GoTo fail
            If verifyPassword(DB, email, txtPassword.Text.Trim, IIf(ViewState("HASHED") = "Y", True, False), usedFlatPassword) = False Then GoTo fail
            If email <> "demo@jfc.com" Then
                If usedFlatPassword = True Then 'IsProduction() = True AndAlso 
                    'Redirect user to set password instead of using falt password
                    Session("MSG") = "The password we provided cannot be used any more. Please follow the instruction on this page to set your own password."
                    DBUpdate.DeleteFlatPassword(DB, email)
                    Response.Redirect("EmailPassword.aspx")
                Else
                    DBUpdate.DeleteFlatPassword(DB, email)
                End If
            End If
PASS:
            'Get New session #
            DB.BeginTransaction()
            DB.ExecuteNonQuery("lock table sys_session in exclusive mode")
            SessionID = DB.GetScalar("select nvl(max(rid), 0)+1 from sys_session")
            DB.ExecuteNonQuery("insert into sys_session(rid, userid, logindate) values(:1, :2, sysdate)", SessionID, r("user_id"))
            DB.CommitTransaction()

            'Create session info
            FormsAuthentication.SetAuthCookie(email, False)
            Session("SYS_SESSION_ID") = SessionID
            Session("SYS_SESSION_USERID") = r("user_id")
            Session("SYS_SESSION_EMAIL") = r("email")
            Session("SYS_SCREEN_WIDTH") = Request.QueryString("screenwidth")
            Session("SYS_SCREEN_HEIGHT") = Request.QueryString("screenheight")
            'Response.Cookies("KOSCUS_LOGIN")("RememberMe") = IIf(chkRememberMe.Checked, email, "")
            'Response.Cookies("KOSCUS_LOGIN").Expires = DateTime.Now.AddDays(60)
            If chkRememberMe.Checked Then
                Response.Cookies("KOSCUS_EMAIL").Expires = DateTime.Now.AddDays(60)
                Response.Cookies("KOSCUS_PASSWD").Expires = DateTime.Now.AddDays(60)
            Else
                Response.Cookies("KOSCUS_EMAIL").Expires = DateTime.Now.AddDays(-1)
                Response.Cookies("KOSCUS_PASSWD").Expires = DateTime.Now.AddDays(-1)
            End If
            Response.Cookies("KOSCUS_EMAIL").Value = email
            Response.Cookies("KOSCUS_PASSWD").Value = txtPassword.Text.Trim


            'Leave event
            'Common.Log(Me, DB, "00", "", "", "", 0, 0, 0, s.IP, s.UserName)

            'Session("SYS_SESSION_DEFAULTURL") = Request.ApplicationPath() & DB.GetScalarEx("select value from sys_app_userparam where appid='PROFILE' and paramid='DEFAULTURL' and userid=:1", "", "", GetUserID)

            If ViewState("CUSTCOMPANYID") <> "" Then
                'Customer admin login
                SQL = "SELECT a.custCompany_id, b.type_id FROM webUser_customer a, custCompany b " & _
                        "WHERE a.custCompany_id = b.custCompany_id AND a.email = :1 AND a.customer_id = 'ADMIN' AND a.custCompany_id = :2"
                dr = DB.GetReader(SQL, email, ViewState("CUSTCOMPANYID"))
                If dr.Read() = True Then
                    Session("SYS_SESSION_CUSTCOMPANYID") = CString(dr("custCompany_id"))
                    Session("SYS_SESSION_CUSTCOMPANYTYPEID") = CString(dr("type_id"))
                    Session("SYS_SESSION_CUSTCOMPANYTYPE") = CaptalizeFirstLetter(GetCodeMaster(DB, "CUSTCOMPANY_TYPE", CString(dr("type_id"))))
                    Session("SYS_SESSION_USERTYPE") = "CUSTADMIN"
                    Session("SYS_SESSION_DEFAULTURL") = "~/CustAdmin/Home.aspx"
                End If
                dr.close()
                InsertEventLog(DB, "Logged in", email, Request.Browser.Browser & " " & Request.Browser.Version, IIf(IsMobile(), "MOBILE", "FULL"), True)
                'YI FU 10/30/2020
            ElseIf GetNrDeviceCode(DB, email, DB.NVL(ViewState("DeviceCode"), " ")) = 0 And email <> "demo@jfc.com" Then
                Session("CONFIRM_CMD") = "VERIFY_OTP"
                Session("CONFIRM_ID") = email
                If String.IsNullOrEmpty(ViewState("DeviceCode")) Then
                    Session("CONFIRM_PASSWORD") = System.Guid.NewGuid().ToString()
                    Response.Cookies("KOSCUS_DEVICECODE").Value = Session("CONFIRM_PASSWORD")
                    Response.Cookies("KOSCUS_DEVICECODE").Expires = DateTime.Now.AddYears(10)
                Else
                    Session("CONFIRM_PASSWORD") = ViewState("DeviceCode")
                End If

                Session("Confirm_refresh") = False
                Session("SYS_SESSION_DEFAULTURL") = "~/Login/Confirm.aspx"

            ElseIf GetNrUserType(DB, email) > 0 Then
                Session("SYS_SESSION_DEFAULTURL") = "~/Login/LoginType.aspx?" & IIf(ViewState("USER_TYPE") <> String.Empty, "&type=" & ViewState("USER_TYPE"), "") & IIf(ViewState("PAGE") <> String.Empty, "&page=" & ViewState("PAGE"), "") '& IIf(ViewState("RETURN_URL") <> String.Empty, "&ReturnUrl=" & ViewState("RETURN_URL"), "")
            End If

            If Session("SYS_SESSION_DEFAULTURL") = "" Then
                GoTo fail
            End If

            'Reset password
            If DB.NVL(r("PASSWORDRESET"), "") = "Y" Then
                Response.Redirect("ChangePassword.aspx")
                Exit Sub
            End If

            'Go to the next page
            Response.Redirect(Session("SYS_SESSION_DEFAULTURL"))
            Exit Sub
FAIL:
            txtPassword.Text = ""
            txtPassword.Focus()
            'MessageBox("Email address or password is invalid", "Error", DB)
            MessageSimpleBox("Email address or password is invalid. If you are our existing customer but don't have a customer portal account, please use 'New User...' link to register. If you are new to JFC, please contact us at http://jfc.com/#/contact-us.", DB)
        End Using
    End Sub

    Private Function verifyPassword(ByVal DB As DBHelper, ByVal email As String, ByVal passwd As String, ByVal isHashedPasswd As Boolean, ByRef usedFlatPasswd As Boolean) As Boolean
        Dim SQL As String
        Dim r As DataReader
        Dim storedPasswd As String = ""
        Dim storedFlatPasswd As String = ""
        Dim storedSalt As String = ""
        Dim hash As String

        'Get user info
        SQL = "select * from webuser where email = :1"
        r = DB.GetReader(SQL, email)
        If r.Read() Then
            storedPasswd = IIf(isHashedPasswd = True, Common.CString(r("password")).Replace("+", " "), Common.CString(r("password")))
            storedFlatPasswd = Common.CString(r("flatPassword"))
            storedSalt = Common.CString(r("salt"))
        End If
        r.close()

        hash = IIf(isHashedPasswd = True, passwd, Common.CreatePasswordHash(passwd, storedSalt))
        If Not (storedPasswd = hash OrElse (storedFlatPasswd <> String.Empty AndAlso storedFlatPasswd = passwd)) Then
            'Incorrect password
            Return False
        End If
        usedFlatPasswd = IIf(storedFlatPasswd <> String.Empty AndAlso storedFlatPasswd = passwd, True, False)
        'Correct password
        Return True
    End Function


End Class

#End ExternalSource
