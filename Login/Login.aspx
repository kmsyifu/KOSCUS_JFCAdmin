<%@ Page Title="Log In" Language="VB" AutoEventWireup="false"  MasterPageFile="Login.Master" CodeFile="Login.aspx.vb" Inherits="Login_Login" %>
<%@ Import Namespace="Common" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(function () {
            $("input[name=screenwidth]").val(screen.width);
            $("input[name=screenheight]").val(screen.height);

            // Eye button: switch the password box between hidden and shown.
            // Plain DOM .type (jQuery 1.4 does not allow changing an input's type).
            $("#btnShowPwd").click(function () {
                var pwd = document.getElementById("<%= txtPassword.ClientID %>");
                var show = (pwd.type === "password");
                pwd.type = show ? "text" : "password";
                var label = show ? "Hide password" : "Show password";
                $(this).toggleClass("shown", show).attr("title", label).attr("aria-label", label);
                pwd.focus();
            });
        });
    </script>

    <style type="text/css">
        .style1
        {
            width: 371px;
        }
        /* Password box with the show/hide (eye) button inside its right edge.
           Here, not in Login.css, so it works even when the browser still has an older Login.css cached. */
        .pwdWrap               { position: relative; display: block; }
        .pwdWrap .stdTxt       { padding-right: 42px !important; }
        .pwdWrap .stdTxt::-ms-reveal { display: none; } /* Edge's own eye - only ours */
        .pwdEye
        {
            position: absolute; top: 0; bottom: 0; right: 4px; margin: auto 0;
            width: 32px; height: 32px; padding: 0; border: 0; border-radius: 4px;
            background: transparent; color: #888888; cursor: pointer; line-height: 0;
        }
        .pwdEye:hover, .pwdEye:focus { color: DarkOrange; outline: none; }
        .pwdEye svg            { width: 20px; height: 20px; vertical-align: middle; }
        .pwdEye .eyeOff        { display: none; }
        .pwdEye.shown .eyeOn   { display: none; }
        .pwdEye.shown .eyeOff  { display: inline; }
    </style>

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <%-- Centered login card (styles: .loginCard in Styles/Login.css, mobile overrides in Login_Mobile.css) --%>
    <asp:Panel ID="pnlNotice" runat="server" CssClass="noticePnl loginNotice">
        <asp:Label ID="lblNotice" runat="server" CssClass="red" Text='<%# LOGIN_NOTICE %>' />
    </asp:Panel>
    <div class="info loginCard">
        <div>
            <h2 class="loginTitle">Log In</h2>
            <asp:Label ID="lblCmd" CssClass="cmd" runat="server" Text="Please enter your email address and password to login." /><%--&nbsp;&nbsp;<a href="NewUser.aspx" style="color:Red;" class="bold italic">(New User? Click here to apply!)</a>--%>
            <table cellpadding="5" class="stdTable">
                <tbody>
                    <tr>
                        <td style="width:35%" align="right">Email Address</td>
                        <td><asp:TextBox ID="txtUserName" class="stdTxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td align="right">Password</td>
                        <td>
                            <span class="pwdWrap">
                                <asp:TextBox ID="txtPassword" class="stdTxt" TextMode="Password" runat="server"></asp:TextBox>
                                <%--Eye button: shows / hides the password. type="button" so it never submits (Enter still logs in)--%>
                                <button type="button" id="btnShowPwd" class="pwdEye" title="Show password" aria-label="Show password" tabindex="-1">
                                    <svg class="eyeOn" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                                    <svg class="eyeOff" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/><path d="M14.12 14.12a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>
                                </button>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:CheckBox ID="chkRememberMe" CssClass="checkbox" Text="Remember me" runat="server" />
                            <br /><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                            <%--<a href="NewUser.aspx">New user...</a><%= IIf(IsMobile(), "<br />", "&nbsp;&nbsp;&nbsp;")%>--%>
                            <a href="EmailPassword.aspx">Set password / Forgot password...  </a>
                        </td>
                    </tr>
               </tbody>
           </table>
        </div>

        <div class="submit">
            <%--<input type="submit" class="midButton" value="Login"/>--%>
            <asp:Button ID="btnLogin" CssClass="midButton" Text="Login" runat="server" />
            <input type="hidden" name="screenwidth" value="1024" />
            <input type="hidden" name="screenheight" value="768" />
        </div>
    </div>
    <%--<br /><br /><br />
    <asp:Panel runat="server" Visible='<%# Not IsMobile() %>'>
        <asp:Label ID="Label1" CssClass="red" runat="server" Text="This customer portal is for JFC's customers only. If you are a JFC's customer and interested in signing up, please fill out " />
        <a target="_blank" class='linkcolor' href='http://shop.jfc.com/Login/SignUp.htm'>this form</a>
        <asp:Label ID="Label2" CssClass="red" runat="server" Text=" and submit to your sales representative." />
    </asp:Panel>--%>
    <%--<embed src="SignUp.pdf" width="500" height="375">
    <iframe src="http://docs.google.com/gview?url=http://shop.jfc.com/Login/SignUp.pdf&embedded=true" style="width:718px; height:700px;" frameborder="0"></iframe>--%>
</asp:Content>