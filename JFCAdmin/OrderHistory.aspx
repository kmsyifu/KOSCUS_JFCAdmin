<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="OrderHistory.aspx.vb" Inherits="JFCAdmin_OrderHistory" %>
<%@ Register Tagprefix="uc" Tagname="OrderHistory" Src="~/Common/OrderHistory.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <%--<h1>Orders</h1>--%>
    <uc:OrderHistory id="ctrOrderHistory" runat="Server" />
</asp:Content>
