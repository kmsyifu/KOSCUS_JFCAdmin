<%@ Page Title="" Language="VB" MasterPageFile="Login.master" AutoEventWireup="false" CodeFile="ChangePassword.aspx.vb" Inherits="Login_ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            var myPSPlugin = $("[id$='txtPassword']").password_strength();

            $("[id$='btnSubmit']").click(function () {
                return myPSPlugin.metReq(); //return true or false
            });

            $("[id$='passwordPolicy']").click(function (event) {
                var width = 350, height = 300, left = (screen.width / 2) - (width / 2),
            top = (screen.height / 2) - (height / 2);
                window.open("PasswordPolicy.xml", 'Password_poplicy', 'width=' + width + ',height=' + height + ',left=' + left + ',top=' + top);
                event.preventDefault();
                return false;
            });
        });
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <br />
    New Password:&nbsp;&nbsp;
    <asp:TextBox ID="txtPassword" TextMode="Password" class="stdTxt" runat="server"></asp:TextBox>
    <asp:Label ID="lblErrMsg" runat="server"></asp:Label>
    <br /><br />
    <asp:Button ID="btnSubmit" CssClass="midButton" runat="server" Text="Submit" />
    <br /><br />
    <a id="passwordPolicy" href="#">Password policy</a>
    <%--<table>
        <tr>
            <td><label for="txtPassword">Enter New Password:</label></td>
            <td style="padding-left: 20px;"><asp:TextBox ID="txtPassword" TextMode="Password" class="stdTxt" runat="server"></asp:TextBox></td>
            <td><asp:Label ID="lblErrMsg" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td></td>
            <td style="padding-left: 20px;"><asp:Button ID="btnSubmit" runat="server" Text="Submit" /></td>
        </tr>
    </table>  
    <p style="text-align: left;">
        <a id="passwordPolicy" href="#">Password policy</a> 
    </p>--%>
</asp:Content>

