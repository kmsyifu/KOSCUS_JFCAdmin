<%@ Page Language="VB" MasterPageFile="Login.master" AutoEventWireup="false" CodeFile="Confirm.aspx.vb" Inherits="Login_Confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
 <script src="../Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        // Close current window without confirming with user
        function closeWindow() {
            window.open('', '_self', '');
            window.close();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <h2><asp:Label ID="lblTitle" runat="server"  Visible ="false"/></h2>
   <br />
    <asp:Label ID="lblConfirmMsg" runat="server" /> <asp:LinkButton ID="lnkResend"  runat="server" Text="request a new verification code." Visible = "false"></asp:LinkButton>
    <asp:Label ID="lblErrorMsg" runat="server" CssClass="red" />
    <br /><br /><br />
    
    <br />
    <asp:Label ID="lblTxtTitle" runat="server" Text = "Secure verification code"   CssClass = "bold " Visible="false"/>
    <br />
    <asp:Label ID="lblOTPErrorMsg" runat="server" CssClass="red"  Visible ="false" />
    <br />
    <asp:TextBox ID="txtOTP" runat ="server" CssClass="stdTxt" Visible="false"  />

    <br />
    
    <asp:Button ID="btnSubmit" CssClass="midButton" runat="server" Text="Submit" Visible="false" />
    <asp:Button ID="btnClose" CssClass="midButton" runat="server" Text="Close" OnClientClick="closeWindow()" />
</asp:Content>
