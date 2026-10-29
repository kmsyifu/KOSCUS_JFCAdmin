#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\EmailPassword.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","3BF1457CDA072E220F207A256F326439DC56BC9C")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\EmailPassword.aspx.vb",1)
Imports System.Data
Imports Common

Partial Class Login_EmailPassword
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
        Dim SQL, newpassword, newsalt, message As String
        Dim errMSg As String = ""
        Dim email As String = CString(txtEmail.Text).Trim.ToLower

        'Show msg such as changing password
        If Session("MSG") <> "" Then
            MessageSimpleBox(Session("MSG"))
            Session("MSG") = ""
        End If

        'Init
        DataBind()
        'ViewState("NewUser") = CString(Request.QueryString("NewUser"))
        btnSubmit.Attributes("onclick") = "SubmitForm();"
        If IsPostBack = False Then Exit Sub

        Using DB As New DBHelper()
            lblError.Text = ""

            If email = "" Then
                lblError.Text = "Please enter email address"
                Exit Sub
            End If

            SQL = "select email from webuser where email = :1"
            If CString(DB.GetScalar(SQL, email)) = String.Empty Then
                lblError.Text = "Email address cannot be found."  'If you are our existing customer but don't have a customer portal account, please register <a class='linkcolor' href='http://shop.jfc.com/Login/NewUser.aspx'>here</a>. If you are new to JFC, please contact us at <a target='_blank' class='linkcolor' href='http://jfc.com/#/contact-us'>jfc.com</a>."
                Exit Sub
            End If
            'r = DB.GetDataRow("select * from webuser where email = :email", txtUserName.Text)
            'If ViewState("NewUser") <> "Y" And r Is Nothing Then
            '    lblError.Text = "Email address not found"
            '    Exit Sub
            'End If

            'Send email
            newpassword = Common.CreateSalt(16)
            newsalt = Common.CreateSalt(64)
            message = "Please click this link to set your password." & vbCrLf & vbCrLf & "http://" & Request.Url.Host & IIf(Request.Url.Port = 80 OrElse Request.Url.Port = 443, "", ":" & Request.Url.Port) & _
                      Request.ApplicationPath & "/Login/Login.aspx?username=" & _
                      HttpUtility.UrlEncode(email) & "&password=" & HttpUtility.UrlEncode(newpassword)
            If Common.SendMail(SYSTEM_EMAIL, email, Nothing, "Set your password", message, Nothing, False, errMSg) > 0 Then
                lblError.Text = "Email sending error (" & errMSg & ")"
                Exit Sub
            End If

            'Create new webuser record 
            'If ViewState("NewUser") = "Y" And r Is Nothing Then
            '    SQL = "INSERT INTO webuser (email, user_id, bid, cid, defaulturl) VALUES (:1, :2, :3, :4, :5)"
            '    newid = DB.GetScalar("SELECT NVL(MAX(user_id), 0)+1 FROM webuser")
            '    DB.ExecuteNonQuery(Sql, txtUserName.Text, newid, "00", "000", "/ABC/Profile.aspx")
            '    r = DB.GetDataRow("select * from webuser where email = :email", txtUserName.Text)
            'End If

            'Reset password with one time password
            newpassword = Common.CreatePasswordHash(newpassword, newsalt)
            'TODO: Move to DBUpdate.vb later
            DB.ExecuteNonQuery("UPDATE webuser SET PASSWORD=:1, SALT=:2, PASSWORDRESET=:3 WHERE EMAIL=:4", _
                               newpassword, newsalt, "Y", email)
            lblMessage.Text = "Email has been sent to " & email & ". "
            lnkLogin2.Text = "Go back to login page"
            lnkLogin2.NavigateUrl = "~/Login/Login.aspx"
            pnlSetPasswd.Visible = False

        End Using
    End Sub


    'Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    'Response.Redirect("~/Login/Login.aspx")
    'End Sub
End Class


#End ExternalSource
