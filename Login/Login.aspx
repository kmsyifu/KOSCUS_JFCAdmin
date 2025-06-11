<%@ Page Title="Log In" Language="VB" AutoEventWireup="false"  MasterPageFile="Login.Master" CodeFile="Login.aspx.vb" Inherits="Login_Login" %>
<%@ Import Namespace="Common" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(function () {
            $("input[name=screenwidth]").val(screen.width);
            $("input[name=screenheight]").val(screen.height);
        });
    </script>

    <style type="text/css">
        .style1
        {
            width: 371px;
        }
    </style>

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <br /> 
    <asp:Panel ID="pnlNotice" runat="server" CssClass="noticePnl" Style="margin:0px 100px">
        <asp:Label ID="lblNotice" runat="server" CssClass="red" Text='<%# LOGIN_NOTICE %>' />
    </asp:Panel>
    <br /><br />
    <div class="info">
        <div>
            <asp:Label ID="lblCmd" CssClass="cmd" runat="server" Text="Please enter your email address and password to login." /><%--&nbsp;&nbsp;<a href="NewUser.aspx" style="color:Red;" class="bold italic">(New User? Click here to apply!)</a>--%>
            <br /><br /><br />
            <table cellpadding="5" class="stdTable">
                <tbody>
                    <tr>
                        <td style="width:35%" align="right">Email Address</td>
                        <td><asp:TextBox ID="txtUserName" class="stdTxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td align="right">Password</td>
                        <td><asp:TextBox ID="txtPassword" class="stdTxt" TextMode="Password" runat="server"></asp:TextBox></td>
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
        <br /><br />

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