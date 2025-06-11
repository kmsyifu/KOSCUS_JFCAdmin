Imports System.Data
Imports Common

Partial Class Login_ChangePassword
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
        If IsInSession() = False Then Response.Redirect("Login.aspx")

        'Init
        DataBind()

    End Sub


    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim passwordSetting As New PasswordSetting
        Dim sbPasswordRegx As StringBuilder = New StringBuilder(String.Empty)
        Dim newsalt, hash As String

        Using DB As New DBHelper

            'min and max
            sbPasswordRegx.Append("(?=^.{" & passwordSetting.MinLength & "," & passwordSetting.MaxLength & "}$)")
            'numbers length
            sbPasswordRegx.Append("(?=(?:.*?\d){" & passwordSetting.NumsLength & "})")
            'a-z characters
            sbPasswordRegx.Append("(?=.*[a-z])")
            'A-Z length
            sbPasswordRegx.Append("(?=(?:.*?[A-Z]){" & passwordSetting.UpperLength & "})")
            'special characters length
            sbPasswordRegx.Append("(?=(?:.*?[" & passwordSetting.SpecialChars & "]){" & passwordSetting.SpecialLength & "})")
            '(?!.*\s) - no spaces
            '[0-9a-zA-Z!@#$%*()_+^&] -- valid characters
            sbPasswordRegx.Append("(?!.*\s)[0-9a-zA-Z" & passwordSetting.SpecialChars & "]*$")

            If Regex.IsMatch(txtPassword.Text, sbPasswordRegx.ToString) Then
                newsalt = Common.CreateSalt(64)
                hash = Common.CreatePasswordHash(txtPassword.Text, newsalt)
                'TODO: Move to DBUpdate.vb later
                DB.ExecuteNonQuery("Update webuser set PASSWORD=:1, SALT=:2, PASSWORDRESET=:3 where user_id=:4", _
                                   hash, newsalt, "N", GetUserID())
                Response.Redirect(Session("SYS_SESSION_DEFAULTURL"))
            Else
                lblErrMsg.Text = "Password Strength Check Error. Please contact JFC"
            End If

        End Using
    End Sub

End Class
