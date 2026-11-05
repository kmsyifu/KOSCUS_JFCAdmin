<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="OrderHistory.aspx.vb" Inherits="JFCAdmin_OrderHistory" %>
<%@ Register Tagprefix="uc" Tagname="OrderHistory" Src="~/Common/OrderHistory.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<asp:Panel ID="pnlEnterKey" DefaultButton="btnEnterNoop" runat="server"><%--Enter in a box with no default button of its own does nothing (before: the menu's Logout button was used -> Login.aspx)--%>
<asp:Button ID="btnEnterNoop" runat="server" style="display:none;" UseSubmitBehavior="false" OnClientClick="return false;" TabIndex="-1" />
    <%--<h1>Orders</h1>--%>
    <uc:OrderHistory id="ctrOrderHistory" runat="Server" />
</asp:Panel><%--pnlEnterKey--%>
</asp:Content>
