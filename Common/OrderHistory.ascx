<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderHistory.ascx.vb" Inherits="Common_OrderHistory" %>
<%@ Register Tagprefix="uc" Tagname="OrderDtl" Src="~/Common/OrderDtl.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="Common" %>
<script type="text/javascript">


    $(document).ready(function () {
        $(".datepicker").datepicker();
    });
   
   
</script> 
<%--List of order--%>
<asp:Panel runat="server" ID="pnlList_order" >
    <h1>Orders</h1>
    <asp:Panel ID="pnlSearchOrder" CssClass="visiblePnl" DefaultButton="lnkTemp" runat="server"> 
        <%--<h1>Search Orders</h1>--%>
        <%--<asp:Label ID="lblCustCompany_order" runat="server" Text="Company" CssClass="gray" />
        <asp:DropDownList ID="lstCustCompany_order" CssClass="drpList" runat="server" AutoPostBack="True" />--%>
        <%--<asp:Label ID="Label2" runat="server" Text='Vendor' CssClass="gray" />
        <asp:DropDownList ID="lstVendor_order" CssClass="drpList size24" runat="server" AutoPostBack="True" />--%>
         <asp:Label ID="Label1" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Sort by" %>' CssClass="gray" />
        <asp:DropDownList ID="lstOrderBy_order" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
        <asp:Label ID="lblCustCompany_order" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Company" %>'  CssClass="gray" />
        <asp:DropDownList ID="lstCustCompany_order" CssClass='<%# "largeDrpList " & IIf(IsMobile(), "size24", "size48") %>' runat="server"  />
        <asp:Label ID="lblCustGrp_order" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType() & " Group" %>' CssClass="gray" />
        <asp:DropDownList ID="lstCustGrp_order" CssClass="largeDrpList" runat="server" />
        <asp:Label ID="lblCustomer_order" runat="server" Text="largeDrpList" CssClass="gray" />
        <asp:DropDownList ID="lstCustomer_order"  CssClass='<%# "largeDrpList " & IIf(IsMobile(), "size32", "size48") %>' runat="server" />
        <asp:Label ID="Label12" runat="server" CssClass="gray" Text='<%# IIf(IsMobile(), "<br />Customer #", "Customer #")  %>'  />
        <%--<asp:TextBox ID="txtJfcBranchNr_order" CssClass="largeTxt size4" runat="server" />---%><asp:TextBox ID="txtJfcCustomerNr_order" CssClass="largeTxt size8" runat="server" autocomplete="off" />
        <asp:Label ID="Label10" CssClass="gray" runat="server" Text='<%# IIf(IsMobile(), "<br />Sales Rep", "Sales Rep")  %>'  />
        <asp:TextBox ID="txtJfccontactname" CssClass="largeTxt size24" runat="server" autocomplete="off" />
        
            <%--<asp:Label ID="Label3" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Order Status" %>' CssClass="gray" />
            <asp:DropDownList ID="lstOrderState_order" CssClass="drpList" runat="server" AutoPostBack="True" />--%>
            <asp:Label ID="Label44" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Order ID or PO #" %>' CssClass="gray" />
            <asp:TextBox ID="txtOrder_order" CssClass="largeTxt size20" runat="server" autocomplete="off" />
           <asp:Label ID="Label7" runat="server" Text='<%# IIf(IsMobile(), "", "<br />")  %>'  />
            <%--<ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtOrder_order" WatermarkText="Search by order ID or PO #" WatermarkCssClass="watermark stdTxt size24" />--%><%--TODO: Error occurs when update or delete order in orderDtl.ascx.vb--%>
            <asp:Label ID="Label2" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Order Date from" %>' CssClass="gray" />
            <asp:TextBox ID="txtOrderDateFrom" CssClass="largeTxt size14 datepicker" runat="server" autocomplete="off" />
            <%--ajaxToolKit:CalendarExtender ID="calOrderDateFrom" Format="MM/dd/yyyy" TargetControlID="txtOrderDateFrom" runat="server" /> <%--TODO: Error occurs when update or delete order in orderDtl.ascx.vb--%>
            <asp:Label ID="Label4" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Order Date to" %>' CssClass="gray" />
            <asp:TextBox ID="txtOrderDateTo" CssClass="largeTxt size14 datepicker" runat="server" autocomplete="off" />
             <asp:Label ID="Label8" runat="server" Text='<%# IIf(IsMobile(), "<br />", "")  %>'  />
            <asp:CheckBox runat="server" ID="chkShowWebOrder" CssClass="checkbox" /><asp:Label ID="Label3" runat="server" Text="Web/App orders only" />
             <asp:Label ID="Label9" runat="server" Text='<%# IIf(IsMobile(), "<br />", "")  %>'  />
            <asp:CheckBox runat="server" ID="chkShowNewOrder" CssClass="checkbox"  /><asp:Label ID="Label5" runat="server" Text="Non-approved / Non SFA downloaded orders only" CssClass="red" />
             &nbsp;&nbsp; <asp:Label ID="Label6" runat="server" Text='<%# IIf(IsMobile(), "<br /><br />", "")  %>'  />

             <asp:button runat ="server" ID="btnSearch" CssClass =" midButton" Text = "Search"></asp:button>
             <asp:linkbutton runat ="server" ID="lnkTemp" CssClass ="" Text =""   ></asp:linkbutton>
          <asp:ImageButton ID="btnSearch_order" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server"  Visible ="false"  />&nbsp;&nbsp;
            <asp:Panel ID="Panel5" Visible='<%# Session("ORDER_HISTORY_CMD") = "JFCADMIN" %>' runat="server">
           
            </asp:Panel>
            <%-- ajaxToolKit:CalendarExtender ID="calOrderDateTo" Format="MM/dd/yyyy" TargetControlID="txtOrderDateTo" runat="server" /> --%>
        
          
            
        
       
    </asp:Panel>
    <%= IIf(IsMobile() AndAlso Session("ORDER_HISTORY_CMD") = "CUST", "", "<br />")%>
    <asp:Label ID="lblTotalOrderAmt" runat="server" CssClass =" gray" Text=""  />
    <asp:GridView ID="grdOrder" runat="server" DataSourceID="dsOrder" 
        AutoGenerateColumns="False" CssClass="STDTABLE" GridLines="Horizontal" 
        ShowHeaderWhenEmpty="false" AllowPaging="true"   PageSize="50">
        <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
        <PagerSettings Mode="NumericFirstLast" Position="Bottom" FirstPageText="<<" LastPageText=">>" />
        <Columns>
            <asp:TemplateField HeaderText="Order ID" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkWebOrderID" Text='<%# Eval("weborder_id") %>' Visible='<%# CString(Eval("jetsOrderNr")) = "" %>' ToolTip='View order detail' CommandName="EDIT_WEB_ROW" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkJetsOrderID" Text='<%# Eval("jetsOrderNr") %>' Visible='<%# CString(Eval("jetsOrderNr")) <> "" %>' ToolTip='View order detail' CommandName="EDIT_JETS_ROW" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />
                    <asp:Label runat="server" ID="lblCustomerID" Text='<%# Eval("customer_id") %>' Visible="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="orderdate_fmt" HeaderText="Order Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />--%>
            <asp:TemplateField HeaderText="Order Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblOrderDate" Visible='<%# Not IsMobile() %>' Text='<%# Eval("orderDate_fmt") %>' />
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkOrderDate" Text='<%# Eval("orderDate_fmt") %>' Visible='<%# IsMobile() %>' ToolTip='View order detail' CommandName='<%# IIf(CString(Eval("jetsOrderNr")) = "", "EDIT_WEB_ROW", "EDIT_JETS_ROW") %>' CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Web Order" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12">
                <ItemTemplate>
                    <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>' Visible='<%# CString(Eval("isWebOrder")) = "Y" %>' runat="server" />
                    <%--<asp:Label runat="server" ID="lblWebOrderID" Text='<%# Eval("weborder_id") %>' />--%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="jfcBranchAndCustomerNr" HeaderText="Customer #" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
            <asp:BoundField DataField="customerName" HeaderText="Name" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
            <asp:BoundField DataField="ShipDate_fmt" HeaderText="Ship Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
            <%--<asp:BoundField DataField="orderstate" HeaderText="Order Status" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />--%>
            <asp:TemplateField HeaderText="Order Status" HeaderStyle-CssClass="th size32">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblOrderState" Text='<%# Eval("orderstateDesc") %>' CssClass='<%# IIf(CString(Eval("orderstate")) = "N" and Session("ORDER_HISTORY_CMD") = "JFCADMIN", "red", "") %>'/>
                </ItemTemplate>
            </asp:TemplateField>            
            <asp:BoundField DataField="custordernr" HeaderText="PO #" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
            <asp:BoundField DataField="REFERENCE" HeaderText="Reference" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" Visible ="false" />
            <asp:BoundField DataField="vendorname" HeaderText="Vendor Name" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
            <asp:BoundField DataField="totalamt" DataFormatString="{0:C}" HeaderText="Total Amount" HeaderStyle-CssClass="th number24" ItemStyle-CssClass="number24" />
            <asp:BoundField DataField="message_view" HeaderText="Message" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
            <asp:BoundField DataField="jfccontactname" HeaderText="Sales Rep" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />
            <asp:BoundField DataField="createUser" HeaderText="Created by" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsOrder" runat="server" />
    <br />
</asp:Panel>
<%--Detail of order--%>
<asp:Panel runat="server" ID="pnlDtl_order" >
    <asp:LinkButton ID="lnkBack_order" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to orders..." ></asp:LinkButton>
    <%# IIf(IsMobile(), "<br /><br />", "<br />") %>
    <uc:OrderDtl id="ctrOrderDtl" runat="Server" />
</asp:Panel>
<br />
