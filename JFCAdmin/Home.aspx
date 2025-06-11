<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="Home.aspx.vb" Inherits="JFCAdmin_Home" %>
<%@ Import Namespace="Common" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:Panel ID="pnlMenu" runat="server">
        <h1><asp:Label ID="Label1" runat="server" Text="Welcome to JFC Salesman Site" /></h1>
        <%--<asp:Panel ID="pnlLinks" runat="server">
            <asp:Panel ID="Panel6" CssClass="visiblePnl leftContent" Width="350px" Height="160px" runat="server">
                <h1>JFC Settings</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a1" class="linkLabel" href="~/JFCAdmin/JFCAdmin.aspx?tab=0">Change JFC's profile</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a3" class="linkLabel" href="~/JFCAdmin/JFCAdmin.aspx?tab=1">Edit user accounts for JFC</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a8" class="linkLabel" href="~/JFCAdmin/JFCAdmin.aspx?tab=2">Edit JFC's branches</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a18" class="linkLabel" href="~/JFCAdmin/JFCAdmin.aspx?tab=3">Edit customer companies</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a19" class="linkLabel" href="~/JFCAdmin/JFCAdmin.aspx?tab=4">Edit vendor companies</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="Panel1" CssClass="visiblePnl leftContent" Width="350px" Height="160px" runat="server">
                <h1>Customer Settings in General</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="aItems" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=0">Change customer company's profile and settings</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="aOrderForm" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=1">Edit user accounts for customer</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a11" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=5">Edit customer's categories</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="Panel7" CssClass="visiblePnl" style="margin-left:765px" Width="350px" Height="160px" runat="server">
                <h1>Customer Settins for Customer Groups</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="aCart" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=2">View customer groups and edit their information</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a10" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=4">Edit vendors for customer group</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a12" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=6">Edit order forms for customer group</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a13" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=7">Edit customer group's items</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <br />
            <asp:Panel ID="Panel5" CssClass="visiblePnl leftContent" Width="350px" Height="160px" runat="server">
                <h1>Customer Settings for Customers in Group</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a9" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=3">View customers and edit their information</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a14" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=8">Add or remove customer's items from customer group</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a17" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=8">Print order form of customer items</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a15" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=9">Edit delivery date</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a16" class="linkLabel" href="~/JFCAdmin/CustAdmin.aspx?tab=9">Edit delivery schedule</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="Panel3" CssClass="visiblePnl leftContent" Width="350px" Height="160px" runat="server">
                <h1>Orders / Payment</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a6" class="linkLabel" href="~/JFCAdmin/OrderHistory.aspx">View orders</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a5" class="linkLabel" href="~/JFCAdmin/Balance.aspx?tab=0">View transaction history with JFC</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a20" class="linkLabel" href="~/JFCAdmin/Balance.aspx?tab=1">View web payment history</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a7" class="linkLabel" href="~/JFCAdmin/Balance.aspx?tab=2">Download document</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="Panel2" CssClass="visiblePnl" style="margin-left:765px" Width="350px" Height="160px" runat="server">
                <h1>Other</h1>
                <table>
                    <tbody>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a4" class="linkLabel" href="~/JFCAdmin/MyAccount.aspx?tab=0">Change my profile</a></td>
                        </tr>
                        <tr>
                            <td class="rowHdr"><a runat="server" id="a2" class="linkLabel" href="~/Login/Login.aspx">Log out</a></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <br /><br /><hr /><br />
        </asp:Panel>--%>
        <br /><br />
    </asp:Panel>

    <asp:Panel ID="pnlMenu_m" runat="server">
        <h1><asp:Label ID="Label2" runat="server" Text="JFC Salesman Site" /></h1>
        <%--<asp:Panel ID="pnlJFCAdmin" runat="server">
            <ul class="menuLink"><li><a runat="server" id="a21" href="~/JFCAdmin/JFCAdmin.aspx" class='linkcolor'>JFC Settings</a></li></ul>
        </asp:Panel>
        <asp:Panel ID="pnlCustAdmin" runat="server">
            <ul class="menuLink"><li><a runat="server" id="aCustAdmin" href="~/JFCAdmin/CustAdmin.aspx" class='linkcolor'>Customer Settings</a></li></ul>
        </asp:Panel>--%>
        <ul class="menuLink"><li><a runat="server" id="aOrderHistory" href="~/JFCAdmin/OrderHistory.aspx" class='linkcolor'>Orders</a></li></ul>
        <%--<ul class="menuLink"><li><a runat="server" id="aDeliverySched" href="~/JFCAdmin/CustSetting.aspx" class='linkcolor'>Delivery Schedule</a></li></ul>--%>
        <asp:Panel ID="pnlCustSetting" runat="server">
        <ul class="menuLink" ><li><a runat="server" id="aCustSetting" href="~/JFCAdmin/CustSetting.aspx" class='linkcolor'>Customer Settings</a></li></ul>
        </asp:Panel>
        <asp:Panel ID="pnlCustItem" runat="server">
                <ul class="menuLink">  <li><a runat="server" id="aCustItem" href="~/JFCAdmin/CustItem.aspx" class='linkcolor'>Customer Items </a></li></ul>  
          </asp:Panel>
        <asp:Panel ID="pnlBranchSetting" runat="server">
                <ul class="menuLink">  <li><a runat="server" id="aBranchSetting" href="~/JFCAdmin/BranchSetting.aspx" class='linkcolor'>Branch Settings </a></li></ul>  
          </asp:Panel>
          <asp:Panel ID="pnlTool" runat="server">
                <ul class="menuLink">  <li><a runat="server" id="atool" href="~/JFCAdmin/tool.aspx" class='linkcolor'>Tools</a></li></ul>  
          </asp:Panel>
        <%--<ul class="menuLink"><li><a runat="server" id="aBalance" href="~/JFCAdmin/Balance.aspx" class='linkcolor'>Payment</a></li></ul>--%>
        <%--<ul class="menuLink_small"><li><a runat="server" id="aMyAccount" href="~/JFCAdmin/MyAccount.aspx" class='linkcolor'>My Account</a></li></ul>--%>
        <ul class="menuLink_small"><li><a runat="server" id="a21" href="~/Login/Login.aspx" class='linkcolor'>Logout</a></li></ul>
        <%--<ul class="menuLink"><li><asp:LinkButton runat="server" id="lnkFullSite" Cssclass='linkcolor' Text="Full Site" /></li></ul>--%>
        <br /><br />
    </asp:Panel>

    <asp:Panel ID="pnlUpdate" runat="server">
        <h2>Available Updates</h2>
        <asp:Label ID="lblUpdateCmd" runat="server" />
        <br /><br />
        <asp:Button ID="btnUpdateCalendar" CssClass="midButton" runat="server" Text="Update Calendar for Next Year" />
    </asp:Panel>
</asp:Content>

