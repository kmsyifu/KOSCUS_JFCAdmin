<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="Balance.aspx.vb" Inherits="JFCAdmin_Balance" %>
<%@ Register Tagprefix="uc" Tagname="Balance" Src="~/Common/Balance.ascx" %>
<%@ Import Namespace="Common" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tabs").tabs();
            $("#tabs").removeClass("ui-widget");
            $('#tabs').css('display', 'block'); //Display tabs when page finish loading
            <%= JavaScript %>
        });
    </script>

    <h1>Payment</h1>

    <div id="tabs" style="display: none;"> <%--Display tabs when page finish loading--%>
       <ul>
            <li><a href="#tabBalance">Balance</a></li>
            <li><a href="#tabPayment">Web Payment History</a></li>
            <li><a href="#tabDownload">Download</a></li>
        </ul>

        <uc:Balance id="ctrBalance" runat="Server" />
     </div>
</asp:Content>
