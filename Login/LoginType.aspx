<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="Login.Master" CodeFile="LoginType.aspx.vb" Inherits="Login_LoginType" %>
<%--<%@ Register TagPrefix="uc" Assembly="DropDownChosen" Namespace="CustomDropDown" %>--%>
 
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
    <%--<asp:Panel ID="pnlUserType_drp" CssClass="info" runat="server" DefaultButton="btnSelect" >
        <br /><br /><br /><br />    
        <div>
            <asp:Label ID="lblCmd" CssClass="cmd" runat="server" Text="You have multiple accounts. Please select one..." />
            <br /><br />
            <table cellpadding="5" class="stdTable">
                <tbody>
                    <tr>
                        <td style="width:30%" align="right">Login As:&nbsp;&nbsp;</td>
                        <asp:DropDownList ID="lstUserType" class="drpList" runat="server" /></td>
                    </tr>
                </tbody>
           </table>
       </div>
       <br /><br />

       <div class="submit">
            <asp:Button ID="btnSelect" CssClass="midButton" runat="server" Text="OK" />
            <input type="hidden" name="screenwidth" value="1024" />
            <input type="hidden" name="screenheight" value="768" />
        </div>
    </asp:Panel>--%>

    <asp:Panel ID="pnlUserType_lst" CssClass="cmd" runat="server" >
        <br /><br />    
        <asp:Label ID="Label3" runat="server" Text="You have multiple accounts. Please select one..." /><br /><br />
        <asp:GridView ID="grdUserType" runat="server" DataSourceID="dsUserType" AutoGenerateColumns="False" CssClass="STDTABLE_NOBORDER" GridLines="None" ShowHeader="false" AllowPaging="False">
            <Columns>
                <asp:TemplateField ItemStyle-CssClass="size32">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblUserTypeID" Text='<%# Eval("id") %>' Visible="false" />
                        <asp:LinkButton runat="server" CssClass="cellLink" ID="lnkUserTypeName" Text='<%# Eval("name") %>' ToolTip='Login' CommandName="LOGIN" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />
                        <br />
                        <asp:Label runat="server" ID="Label1" CssClass="fontsize-1 gray" Text='<%# IIf(Common.IsMobile() = True, Common.CString(Eval("partAddress")), Common.CString(Eval("fullAddress"))) & IIf(Common.CString(Eval("fullAddress")) <> "", "&nbsp;&nbsp;&nbsp;&nbsp;", "") %>' />
                        <asp:Label runat="server" ID="Label2" CssClass="fontsize-1 gray" Text='<%# Eval("balance") %>' />
                        <asp:Label runat="server" ID="Label4" CssClass="fontsize-1 red" Text='<%# IIf(Common.Val(Eval("jfcAdminPendingOrders")) = 0, "", "You have " & Common.Val(Eval("jfcAdminPendingOrders")) & " order(s) to confirm") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsUserType" runat="server" />
    </asp:Panel>

    <%--Put below in Home.aspx instaed--%>
    <%--<asp:Panel ID="pnlNotification" runat="server">
        <br /><br />
        <asp:Panel ID="pnlCustNotification" runat="server" >
            <asp:Panel ID="pnlPaperless" runat="server" >
                <p><span class="bold">Do you want to go paperless by receiving statements by email only (no mail)?</span>&nbsp;&nbsp;<asp:Image ID="Image1" CssClass="xsImgBtn" ImageUrl='<%# Common.GetWebImgUrl("paperless.png") %>' runat="server" /></p>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp; 
                <asp:RadioButton ID="radPaperlessYes" CssClass="radiobtn" runat="server" GroupName="papaerless" Text="Go paperless" />&nbsp;&nbsp;&nbsp;&nbsp; 
                <asp:RadioButton ID="radPaperlessNo" CssClass="radiobtn" runat="server" GroupName="papaerless" Text="No, thank you" />&nbsp;&nbsp;&nbsp;&nbsp; 
                <br /><br />
            </asp:Panel>
            <asp:Panel ID="pnlEmailInvoice" runat="server" >
                <p><span class="bold">Do you want to receive copy of invoices by email?</span></p>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp; 
                <asp:RadioButton ID="radEmailInvoiceYes" CssClass="radiobtn" runat="server" GroupName="emailInvoice" Text="Receive copy of invoices by email" />&nbsp;&nbsp;&nbsp;&nbsp; 
                <asp:RadioButton ID="radEmailInvoiceNo" CssClass="radiobtn" runat="server" GroupName="emailInvoice" Text="No, thank you" />&nbsp;&nbsp;&nbsp;&nbsp; 
                <br /><br />
            </asp:Panel>
            <br /><br />
            <asp:Button ID="btnSubmit" CssClass="midButton" Text="Submit" runat="server" />&nbsp;&nbsp;
            <asp:LinkButton runat="server" CssClass="linkLabel bottom" ID="lnkNoChange" Text="Ask me later" />   
        </asp:Panel>  
    </asp:Panel>--%>
</asp:Content>