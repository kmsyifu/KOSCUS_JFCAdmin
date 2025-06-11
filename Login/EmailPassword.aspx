<%@ Page Title="" Language="VB" MasterPageFile="Login.master" AutoEventWireup="false" CodeFile="EmailPassword.aspx.vb" Inherits="Login_EmailPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript">
        function SubmitForm() {
            $("#message").html('Sending email. Please wait...');
            $("#submitButton").empty();
            document.forms[0].submit();
        };
    </script> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:Panel ID="pnlSetPasswd" runat="server">
        <span class="bold">Set your password</span><br /><br /><br />
        <span>Please enter your email address and click the submit button. You will receive an email with a link to set your password.</span>
        <br /><br /><br />
        <span>Email Address&nbsp;&nbsp;</span>
        <asp:TextBox ID="txtEmail" class="stdTxt" runat="server" /><br /><br />
        <asp:Label ID="lblError" runat="server" ForeColor="#CC0000" />
        <br /><br />
        <div id="message" style="color: Navy; font-weight: bold;"></div>
        <span class="submitButton">
            <asp:Button ID="btnSubmit" CssClass="midButton" runat="server" Text="Submit" />&nbsp;
            <asp:LinkButton ID="lnkLogin" runat="server" Text="Back to login page" PostBackUrl="~/Login/Login.aspx" CssClass="bottom" />
        </span>
    </asp:Panel>
    <asp:Label ID="lblMessage" runat="server" Text="" /><asp:HyperLink ID="lnkLogin2" runat="server" />

</asp:Content>

