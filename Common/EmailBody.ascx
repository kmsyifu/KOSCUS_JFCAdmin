<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EmailBody.ascx.vb" Inherits="Common_EmailBody" %>
<%@ Import Namespace="Common" %>

<%--
1) Use http://premailer.dialect.ca/ to convert Css to inline with <style type="text/css"></style>
2) Embedded it to code
--%>
<asp:Panel runat="server" ID="pnlOrder">
    <%--Header--%>
    <asp:Panel ID="pnlJfcLogo_order" runat="server">
        <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label><%--&nbsp;&nbsp;<a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%><br /><br /></asp:Panel>
    <%--Message--%>
    <asp:Label ID="lblMsg_order" runat="server" /><br /><br />    
    <asp:Label ID="lblLink_order" runat="server" />
    
    <%--Order info--%>
    <asp:Panel ID="pnlOrderHdr" CssClass="visiblePnl leftContent" runat="server" Width="320px"
        style="width: 320px; margin-right: 10px; background-color: #FFFFFF; padding: 5px 10px; border: thin solid #c0c0c0;"><%--style="width: 350px;  ..."--%> 
        <h1 style="font-size: 11pt; font-weight: normal; color: #FF6600; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #ddd; margin: 0px; padding: 2px;">Order Info</h1>
        <asp:Label ID="lblCustomerID" runat="server" Visible="false" />
        <asp:Label ID="lblCustGrpVendorID" runat="server" Visible="false" />
        <table style="font-size: 9pt">
            <tbody>
                <tr>
                    <td><%=Resources.Strings.WebOrderID%></td>
                    <td><asp:Label ID="lblWebOrderID" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Status%></td>
                    <td><asp:Label ID="lblOrderState" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Delivery%></td>
                    <td><asp:Label ID="lblDeliveryDate" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <asp:Panel ID="pnlVendorName" runat="server">
                    <tr>
                        <td><%=Resources.Strings.Vendor%></td>
                        <td><asp:Label ID="lblVendorName" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td><asp:Label ID="lblTitleItemGrp" runat="server" Text="Order Form"/></td>
                    <td><asp:Label ID="lblItemGrpName" CssClass="cellLbl" runat="server" /></td>
                </tr>
                <asp:Panel ID="pnlShowPrice" runat="server">
                    <tr>
                        <td><%=Resources.Strings.TotalAmount%></td>
                        <td><asp:Label ID="lblTotalAmt" CssClass="cellLbl bold red" runat="server" style="font-weight: bold; color: #FF0000; margin: 3px;"/></td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td><%=Resources.Strings.Message%></td>
                    <td><asp:Label ID="lblMessage" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.CustPO%></td>
                    <td><asp:Label ID="lblCustOrderNr" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Name%></td>
                    <td><asp:Label ID="lblCustomerName" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/><asp:Panel ID="pnlCustGrpName" style="display:inline;" runat="server"><br />(<asp:Label ID="lblCustGrpName" CssClass="cellLbl" runat="server" style="margin: 3px;"/>)</asp:Panel></td>
                </tr>
                <tr>
                    <td class="gray"><%=Resources.Strings.Account%> #</td><%--<%# GetCustCompanyType() & " #"%>--%>
                    <td><asp:Label ID="lblJfcBranchNr" CssClass="cellLbl" runat="server" style="margin: 3px;"/>-<asp:Label ID="lblJfcCustomerNr" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <asp:Panel ID="pnlCustShipPoint" runat="server">
                    <tr>
                        <td><%=Resources.Strings.Shipto%></td>
                        <td><asp:Label ID="lblShipName" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/><asp:Label ID="lblShipPointID" Visible="false" runat="server" /></td>
                    </tr>
                    <tr></tr>
                    <tr>
                        <td class="gray right" style="text-align: right; color: #666666;" align="right"><%=Resources.Strings.Address%>:</td>
                        <td class="gray"><asp:Label ID="lblAddress1" CssClass="cellLbl" runat="server" style="color: #666666; margin: 3px;"/><asp:Label ID="lblAddress2" runat="server" CssClass="cellLbl" style="color: #666666; margin: 3px;"/></td>
                    </tr>
                    <tr>
                        <td class="gray right" style="text-align: right; color: #666666;" align="right"><%=Resources.Strings.City%>:</td>
                        <td class="gray"><asp:Label ID="lblCity" CssClass="cellLbl" runat="server" style="color: #666666; margin: 3px;"/></td>
                    </tr>
                    <tr>
                        <td class="gray right" style="text-align: right; color: #666666;" align="right"><%=Resources.Strings.StateZip%>:</td>
                        <td class="gray"><asp:Label ID="lblState" CssClass="cellLbl" runat="server" style="color: #666666; margin: 3px;"/><asp:Label ID="lblZip" runat="server" CssClass="cellLbl" style="color: #666666; margin: 3px;"/></td>
                    </tr>
                    <%--<tr>
                        <td class="gray right" style="text-align: right; color: #666666;" align="right">Country:</td>
                        <td class="gray"><asp:Label ID="lblCountry" CssClass="cellLbl" runat="server" style="color: #666666; margin: 3px;"/></td>
                    </tr>--%>
                </asp:Panel>
                <tr>
                    <td><%=Resources.Strings.Orderedon%></td>
                    <td><asp:Label ID="lblCreateDateTimeUser" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.LastUpdated%></td>
                    <td><asp:Label ID="lblModDateUser" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr><td colspan="2"></td></tr>
            </tbody>
        </table>
    </asp:Panel>
    <br />

    <asp:Panel ID="pnlOrderDtl" runat="server">
        <%--Order detail--%>
        <asp:GridView ID="grdItem" runat="server" DataSourceID="dsItem" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                cellspacing="0" rules="rows" style="width: 343px; font-size: 9pt; border: thin solid #c0c0c0; border-collapse: collapse;">
            <Columns>
                <asp:BoundField HeaderText="<%$ Resources:Strings, ItemNum %>" DataField="itemIdNr_view" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="144px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"/>
                <asp:BoundField HeaderText="<%$ Resources:Strings, Description %>" DataField="itemDesc_view" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <%--<asp:TemplateField HeaderText="Each/Case" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 gray" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="96px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEaPerCs" CssClass="cellLbl" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# CString(Eval("eaPerCs"))%>' />
                        <asp:Label runat="server" ID="Label0" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='ea/cs' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Price %>" HeaderStyle-CssClass="th number12" ItemStyle-CssClass="number12" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="72px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <%--<asp:Label runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csQty")) <> "0"%>' />--%>
                        <asp:Label runat="server" ID="lblCsPrice" Text='<%# CString(Eval("csprice", "{0:C}")) %>' Visible='<%# CString(Eval("csQty")) <> "0"%>' />
                        <%--<asp:Label runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csQty")) <> "0"%>' />--%>
                        <asp:Label ID="Label1" runat="server" Text='<br />' Visible='<%# CString(Eval("csQty")) <> "0"%>' />
                        <%--<asp:Label runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaQty")) <> "0"%>' />--%>
                        <asp:Label runat="server" ID="lblEaPrice" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("eaQty")) <> "0"%>' />
                        <%--<asp:Label runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaQty")) <> "0"%>' />--%>
                        <asp:Label ID="Label2" runat="server" Text='<br />' Visible='<%# CString(Eval("eaQty")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" Text='<%# Eval("lbPrice_OrderByWgt", "{0:C}") %>' Visible='<%# CString(Eval("lbs")) <> "0"%>' />
                        <%--<asp:Label runat="server" ID="lblLbPrice_OrderByQty" Text='<%# Eval("lbPrice_OrderByQty", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />--%>
                          <asp:Label runat= "server" ID="lblbtmCs"  Text='<%# CString(Eval("btmcsprice", "{0:C}"))  %>' Visible ="false" />
                          <asp:Label runat= "server" ID="lblbtmEa"  Text='<%# CString(Eval("btmeaprice", "{0:C}"))  %>' Visible ="false" />
                          <asp:Label runat= "server" ID="lblbtmlb"  Text='<%# CString(Eval("btmlbprice", "{0:C}"))  %>' Visible ="false" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size6" ItemStyle-CssClass="size6 gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="36px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCs" Text='/cs' Visible='<%# CString(Eval("csQty")) <> "0"%>' /><asp:Label ID="Label3" runat="server" Text='<br />' Visible='<%# CString(Eval("csQty")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/pc", "/ea") %>' Visible='<%# CString(Eval("eaQty")) <> "0"%>' /><asp:Label ID="Label4" runat="server" Text='<br />' Visible='<%# CString(Eval("eaQty")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblLb_orderByWgt" Text='/lb' Visible='<%# CString(Eval("lbs")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblLb_orderByQty" Text='/lb' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Qty %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10 bold" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsQty" Text='<%# CString(Eval("csQty")) & "<br />" %>' Visible='<%# Val(Eval("csQty")) <> 0%>' />
                        <asp:Label runat="server" ID="Label7" CssClass="green" style="color: #008000;" Text='<%# CString(Eval("csQty_vpsp")) & "<br />" %>' Visible='<%# Val(Eval("csQty_vpsp")) > 0 %>' />
                        <asp:Label runat="server" ID="lblEaQty" Text='<%# CString(Eval("eaQty")) & "<br />" %>' Visible='<%# Val(Eval("eaQty")) <> 0 %>'/>
                        <asp:Label runat="server" ID="Label9" CssClass="green" style="color: #008000;" Text='<%# CString(Eval("eaQty_vpsp")) & "<br />" %>' Visible='<%# Val(Eval("eaQty_vpsp")) > 0 %>' />
                        <asp:Label runat="server" ID="lblLbs" Text='<%# Eval("lbs")%>' Visible='<%# Val(Eval("lbs")) <> 0 %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4 bold" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" ItemStyle-Font-Bold="true">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCs" Text='cs<br />' Visible='<%# Val(Eval("csQty")) <> 0 %>' />
                        <asp:Label runat="server" ID="Label10" CssClass="green" style="color: #008000;" Visible='<%# Val(Eval("csQty_vpsp")) > 0 %>' Text="cs_free<br />" />
                        <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "pc", "ea") & "<br />" %>' Visible='<%# Val(Eval("eaQty")) <> 0%>' />
                        <asp:Label runat="server" ID="Label11" CssClass="green" style="color: #008000;" Visible='<%# Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "pc_free", "ea_free") & "<br />" %>' />
                        <asp:Label runat="server" ID="lblLb" Text='lb' Visible='<%# Val(Eval("lbs")) <> 0%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Amount %>" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="96px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLineAmt" CssClass="cellLbl" Text='<%# Eval("lineAmt", "{0:C}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Avail %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail_view")) & "<br />" %>' />
                        <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail_view")) & "<br />" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCs2" Text="cs" /><br />
                        <asp:Label runat="server" ID="lblEa2" Text="ea" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, OutOfStock %>" HeaderStyle-CssClass="th center10" ItemStyle-CssClass="center10 red bold" 
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000">
                    <ItemTemplate>
                        <%--<asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text="Yes" Visible='<%# Val(Eval("remainingInStockQty")) < 0 %>' />--%>
                        <asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text='<%# IIf(Val(Eval("remainingInStockQty")) < 0, "Y", "") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField HeaderText="Invoice" DataField="invoice" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="96px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="<%$ Resources:Strings, Notfound %>"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsItem" runat="server"></asp:SqlDataSource>
         
        <asp:Label ID="lblOrderMsg" style="font-size: 10pt;" runat="server" />
    </asp:Panel>
    <br />
    <%--<asp:Button ID="btnConfirm" CssClass="midButton largeBtn" runat="server" Text="Confirm Order" />--%>

    <asp:Label ID="lblLink2_order" runat="server" />
</asp:Panel>

<asp:Panel runat="server" ID="pnlPriceInquiry" >
    <%--Header--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label><%--&nbsp;&nbsp;<a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%><br /><br /><%--Message--%><asp:Label ID="lblMsg_priceInquiry" runat="server" /> 
    <br />
    <asp:Label ID="lbllink_priceInquiry" runat="server" />
    <br />
    <%--Price Inquiry--%>
    <asp:GridView ID="grdPriceInquiry" runat="server" DataSourceID="dsPriceInquiry" CssClass="STDTABLE" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
            cellspacing="0" rules="rows" style="width: 343px; font-size: 9pt; border: thin solid #c0c0c0; border-collapse: collapse;">
        <Columns>
            <asp:BoundField HeaderText="JFC Item #" DataField="item_id" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="144px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="JFC Item Description" DataField="itemDesc" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Customer Item #" DataField="itemNr" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="144px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Customer Item Description" DataField="custItemDesc" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Case Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="72px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Each Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="72px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Lb Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="72px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="Avail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail")) & "<br />" %>' />
                    <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail")) & "<br />" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text="cs" /><br />
                    <asp:Label runat="server" ID="lblEa" Text="ea" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField HeaderText="Avail" DataField="csAvail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text="cs" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="" DataField="eaAvail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEa" Text="ea" />
                </ItemTemplate>
            </asp:TemplateField>--%>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsPriceInquiry" runat="server"></asp:SqlDataSource>
    <br /><br />
</asp:Panel>

<asp:Panel runat="server" ID="pnlPay">
    <%--Header--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label><%--&nbsp;&nbsp;<a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%><br /><br /><%--Message--%><asp:Label ID="lblMsg_pay" runat="server" />    
    <br /><br />
    <%--Payment info--%>
    <asp:Panel ID="pnlPayHdr" CssClass="visiblePnl leftContent" Width="500px" runat="server"
        style="width: 320px; margin-right: 10px; background-color: #FFFFFF; padding: 5px 10px; border: thin solid #c0c0c0;"><%--width: 500px;--%> 
        <h1 style="font-size: 11pt; font-weight: normal; color: #FF6600; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #ddd; margin: 0px; padding: 2px;">Payment Info</h1>
        <asp:Label ID="lblPaymentID" runat="server" Visible="false" />
        <asp:FormView ID="frmPayHdr" runat="server">
            <ItemTemplate>
                <table style="font-size: 9pt">
                    <tbody>
                        <tr>
                            <td>Payment ID</td>
                            <td><%# Eval("payment_id")%></td>
                        </tr>
                        <tr>
                            <td>Account Name</td>
                            <td><%# Eval("customerName")%></td>
                        </tr>
                        <tr>
                            <td>JFC Account #</td>
                            <td><%# Eval("jfcBranchNr")%>-<%# Eval("jfcCustomerNr")%></td>
                        </tr>
                        <tr>
                            <td>Total Paid Amount</td>
                            <td><span class='bold red' style="font-weight: bold; color: #FF0000;"><%# Eval("paymentAmt", "{0:$#,#.00}")%>&nbsp;<%# Eval("approvedAmtDesc")%></span></td>
                        </tr>
                        <tr>
                            <td>Advance Payment</td>
                            <td><span class='bold' style="font-weight: bold;"><%# Eval("advancePayAmt", "{0:$#,#.00}")%></span></td>
                        </tr>
                        <tr>
                            <td>Status</td>
                            <td><span class='bold' style="font-weight: bold;"><%# Eval("status")%></span></td>
                        </tr>
                        <%--<tr>
                            <td >Result</td>
                            <td><%# Eval("result")%></td>
                        </tr>--%>
                        <tr>
                            <td>Message to JFC</td>
                            <td><%# Eval("message")%></td>
                        </tr>
                        <tr>
                            <td>Account</td>
                            <td><%# Eval("accNickName")%></td>
                        </tr>
                        <tr>
                            <td>Paid on</td>
                            <td><%# Eval("createDateTimeUser")%></td>
                        </tr>
                    </tbody>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </asp:Panel>
    <br />

    <asp:Panel ID="pnlPayDtl" runat="server">
        <%--Payment detail--%>
        <asp:GridView ID="grdPayDtl" runat="server" DataSourceID="dsPayDtl" CssClass="STDTABLE" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                cellspacing="0" rules="rows" style="width: 343px; font-size: 9pt; border: thin solid #c0c0c0; border-collapse: collapse;">
            <Columns>
                <asp:BoundField DataField="arDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="196px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField DataField="reference" HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="note" HeaderText="Note" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
                <asp:TemplateField HeaderText="Web Discount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblWebDiscAmt" CssClass="cellLbl" Text='<%# Eval("webDiscAmt", "{0:C}") %>' Visible='<%# Common.Val(Eval("webDiscAmt")) <> 0 %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="paymentLineAmt" DataFormatString="{0:$#,#.00}" HeaderText="Paid Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"  
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsPayDtl" runat="server"></asp:SqlDataSource>
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlBalanceDtl" runat="server">
        <%--Message--%>
        <asp:Label ID="Label9" Text="Here is the remaining balance." runat="server" />    
        <br /><br />
        <%--Payment detail--%>
        <asp:GridView ID="grdBalanceDtl" runat="server" DataSourceID="dsBalanceDtl" CssClass="STDTABLE" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                cellspacing="0" rules="rows" style="width: 343px; font-size: 9pt; border: thin solid #c0c0c0; border-collapse: collapse;">
            <Columns>
                <asp:BoundField DataField="arDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="196px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField DataField="reference" HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
                <asp:BoundField DataField="balance" DataFormatString="{0:$#,#.00}" HeaderText="Remaining Balance" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"  
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="120px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsBalanceDtl" runat="server"></asp:SqlDataSource>
        <br />
    </asp:Panel>
</asp:Panel>

<asp:Panel ID="pnlNewItem" runat="server">
    <%--Header--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label>
    <%--<asp:Image ID="Image1" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/jfcLogo.gif" />--%><%--<br /><br />
    <a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%>
    <br /><br />
    <%--New JFC Items--%>
    <asp:Label ID="Label3" Text="Here are our new items! For details, please go to " runat="server" /><a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a><asp:Label ID="Label4" Text=" or reply to contact your sales rep." runat="server" />    
    <br /><br />
    <asp:GridView ID="grdNewItem" runat="server" DataSourceID="dsNewItem" AutoGenerateColumns="false" GridLines="None" ShowHeader="false" AllowSorting="false" AllowPaging="false"
            cellspacing="0" rules="rows" style="font-size: 11pt; border: none; border-color: Gray;">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ID="imgItem" CssClass="midGrdImg" Width="160px" Height="160px" runat="server" ImageUrl='<%# GetNewItemImgUrl(CString(Eval("item_id"))) %>' />
                    <%--<asp:Image ID="imgItem" CssClass="midGrdImg" runat="server" ImageUrl="https://shop.jfc.com/Images/Web/noImg.jpg" />Doesn't work here--%>
                    <%--<asp:Image ID="imgItem" CssClass="midGrdImg" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/NewItems/noImg.jpg" />Works--%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblItemDesc" style="margin: 3px;" Text='<%# Eval("itemDesc_view") %>' />
                    <asp:Label runat="server" ID="lblItemIdNr" style="margin: 3px;" Text='<%# Eval("itemIdNr_view") %>' /><br />
                    <%--<asp:Label runat="server" ID="lblItemGrpName" CssClass="cellLbl gray" Text='<%# CString(Eval("itemGrpName")) & "<br />" %>' />
                    <asp:Label runat="server" ID="lblVendorName" CssClass="cellLbl" Text='<%# CString(Eval("vendorName")) & "<br />" %>' />--%>
                    <asp:Label runat="server" ID="lblCtgName" CssClass="cellLbl gray" style="margin: 3px; color: Gray;" Text='<%# Eval("ctgName_view") %>' />
                    <%--<asp:ImageButton ID="btnFavorite" ToolTip="Add or remove from my favorite" CommandName="FAVORITE" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl_OnOff("FAVORITE", CString(Eval("favorite"))) %>' CommandArgument='<%# Eval("custGrpItem_id")%>' runat="server" />
                    <asp:Image ID="imgOrdered" ToolTip="Previously ordered item" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("greenCheck.png") %>' Visible='<%# CString(Eval("ordered")) = "ON" %>' runat="server" />--%><br />
                    <asp:Label ID="Label1" runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblCsPrice" CssClass="cellLbl red bold" style="margin: 3px; color: Red; font-weight: bold;" Text='<%# Eval("csprice", "{0:C}") %>' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblCs" CssClass="cellLbl" Text='/ cs' style="margin: 3px;" Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblEaPrice" CssClass="cellLbl red bold" style="margin: 3px; color: Red; font-weight: bold;" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("eaprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/ pc", "/ ea") %>' Visible='<%# CString(Eval("eaprice")) <> "0"%>' />
                    <asp:Label ID="Label2" runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" CssClass="cellLbl red bold" style="margin: 3px; color: Red; font-weight: bold;" Text='<%# Eval("LbPrice_OrderByWgt", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLb_orderByWgt" CssClass="cellLbl" style="margin: 3px;" Text='/ lb' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLbPrice_orderByQty" CssClass="cellLbl red bold" style="margin: 3px; color: Red; font-weight: bold;" Text='<%# Eval("LbPrice_OrderByQty", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLb_orderByQty" CssClass="cellLbl" style="margin: 3px;" Text='/ lb' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <%--<asp:Label runat="server" ID="lblAskPrice" CssClass="cellLbl red bold" style="margin: 3px; color: Red; font-weight: bold;" Text='Price: Ask' Visible='<%# CString(Eval("askPrice")) = "Y"%>' /><br />--%>
                    <%--<asp:Image ID="imgCart" ToolTip="Exists in cart" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("cart.png") %>' Visible='<%# CString(Eval("cart_id")) <> "" %>' runat="server" />--%>
                    <%--Don't show out of stock for new items--%>
                    <%--<asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl green bold" Text='Out of Stock' Visible='<%# CString(Eval("outOfStock")) = "Y"%>' />--%>
                    <asp:Label runat="server" ID="lblArrivDate" CssClass="cellLbl green" style="margin: 3px; color: Green;" Text='<%# "Est. arrival: " & Eval("arrivDate_fmt") %>' Visible='<%# CString(Eval("arrivDate_fmt")) <> "" %>' />
                    <asp:Label runat="server" ID="lblJfcRemark" CssClass="cellLbl green" style="margin: 3px; color: Green;" Text='<%# Eval("jfcRemark") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsNewItem" runat="server"></asp:SqlDataSource>
    <br /><br />
</asp:Panel>


<asp:Panel ID="PnlUpdate" runat="server">
    <%--Header--%>
    <%--<label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label>&nbsp;&nbsp;--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label><%--&nbsp;&nbsp;<a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%>
    <%--<br /><br /><asp:Image ID="Image2" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/jfcLogo.gif" />--%>
    <br /><br />
    <%--Message--%>
    <asp:Label ID="lblMsg_Item" runat="server" />    
    <br /><br />
    <%--Payment info--%>
    <asp:Panel ID="pnlItemHdr" CssClass="visiblePnl leftContent" Width="500px" runat="server"
        style="width: 320px; margin-right: 10px; background-color: #FFFFFF; padding: 5px 10px; border: thin solid #c0c0c0;"><%--width: 500px;--%> 
        <h1 style="font-size: 11pt; font-weight: normal; color: #FF6600; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #ddd; margin: 0px; padding: 2px;"><%=Resources.Strings.ItemInfo%></h1>
        <asp:Label ID="lblItemID" runat="server"  />
        <table style="font-size: 9pt">
            <tbody>
                <tr>
                    <td><%=Resources.Strings.ItemNum%></td>
                    <td><asp:Label ID="lblItemCode" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.ItemNewNum%></td>
                    <td><asp:Label ID="lblItemCodeNew" CssClass="cellLbl bold" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Description%></td>
                    <td><asp:Label ID="lblItemDesc" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.DescNew%></td>
                    <td><asp:Label ID="lblItemDescNew" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Remark%></td>
                    <td><asp:Label ID="lblItemRemark" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.EachCase%></td>
                    <td><asp:Label ID="lblItemEachCase" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
             
                <asp:Panel ID="pnlItemPriceNew" runat="server">
                    <tr>
                    <%--Price--%>
                        <td><%=Resources.Strings.Price%></td>
                        <td><asp:Label ID="lblItemPriceNew" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
                </asp:Panel>
                 <%-- asp:Panel ID="pnlItemPriceNew" runat="server">
                    <tr>
                        <td>Price New</td>
                        <td><asp:Label ID="lblItemPriceNew" CssClass="cellLbl bold red" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
                </asp:Panel>--%>
              
                <tr><td colspan="2"></td></tr>
            </tbody>
        </table>
    </asp:Panel>
    <br />
</asp:Panel>


<asp:Panel ID="PnlMaster" runat="server">
    <%--Header--%>
    <%--<label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label>&nbsp;&nbsp;--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label>
    <br /><br />
    <%--<asp:Image ID="Image3" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/jfcLogo.gif" />
    <br /><br />--%>
    <%--Message--%>
    <asp:Label ID="lblMsg_Master" runat="server" />    
    <br /><br />
    <%--Payment info--%>
    <asp:Panel ID="PnlMasterContant" CssClass="visiblePnl leftContent" Width="500px" runat="server"
        style="width: 320px; margin-right: 10px; background-color: #FFFFFF; padding: 5px 10px; border: thin solid #c0c0c0;"><%--width: 500px;--%> 
        <h1 style="font-size: 11pt; font-weight: normal; color: #FF6600; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #ddd; margin: 0px; padding: 2px;"><%=Resources.Strings.DeliverySchedule%></h1>
        <asp:Label ID="Label6" runat="server"  />
        <table style="font-size: 9pt">
            <tbody>
                <tr>
                    <td><%=Resources.Strings.Customer %></td>
                    <td><asp:Label ID="lblMasterCustomer" CssClass="cellLbl" runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.JFCAccountNum%></td>
                    <td><asp:Label ID="lblMasterJFCAcc" CssClass="cellLbl " runat="server" style=" margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.ShippingDestination%></td>
                    <td><asp:Label ID="lblMasterShipDest" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Address1%></td>
                    <td><asp:Label ID="lblMasterAddr1" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Address2%></td>
                    <td><asp:Label ID="lblMasterAddr2" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.City%></td>
                    <td><asp:Label ID="lblMasterCity" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.State%></td>
                    <td><asp:Label ID="lblMasterState" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                    <tr>
                    <td><%=Resources.Strings.Zip%></td>
                    <td><asp:Label ID="lblMasterZip" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Country%></td>
                    <td><asp:Label ID="lblMasterCountry" CssClass="cellLbl " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Vendor%></td>
                    <td><asp:Label ID="lblMasterVendor" CssClass="cellLbl  " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.SendReminder%></td>
                    <td><asp:Label ID="lblMasterSendReminder" CssClass="cellLbl  " runat="server" style="margin: 3px;"/></td>
                </tr>
                <tr>
                    <td><%=Resources.Strings.Frequency%></td>
                    <td><asp:Label ID="lblMasterFrequency" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                 </tr>
                     <tr>
                        <td><%=Resources.Strings.DeliveryOn%></td>
                        <td><asp:Label ID="lblMasterDeliveryOn" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
                    <tr>
                        <td><%=Resources.Strings.OrderBy1%></td>
                        <td><asp:Label ID="lblMasterOrderby" CssClass="cellLbl bold " runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
               
                 <%-- asp:Panel ID="pnlItemPriceNew" runat="server">
                    <tr>
                        <td>Price New</td>
                        <td><asp:Label ID="lblItemPriceNew" CssClass="cellLbl bold red" runat="server" style="font-weight: bold; margin: 3px;"/></td>
                    </tr>
                </asp:Panel>--%>
              
                <tr><td colspan="2"></td></tr>
            </tbody>
        </table>
    </asp:Panel>
    <br />
</asp:Panel>



<asp:Panel ID="PnlPriceInquireList" runat="server">
    <%--Header--%>
    <%--<label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label>&nbsp;&nbsp;--%>
    <label style="font-style: italic; color: #FF0000;" class="red italic"><%= Common.JFC_COMPANY_NAME%></label><%--&nbsp;&nbsp;<a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%>
    <%--<br /><br /><asp:Image ID="Image4" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/jfcLogo.gif" />--%>
    <br /><br />
    <%--Message--%>
    <asp:Label ID="lblMsg_PriceInquireListAdd" runat="server" />    
    <br /><br />
    <%--Payment info--%>
    <asp:Panel ID="pnlPriceInquireItems" CssClass="visiblePnl leftContent"  runat="server"><%--width: 500px;--%> 
        <h1 style="font-size: 11pt; font-weight: normal; color: #FF6600; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #ddd; margin: 0px; padding: 2px;"><%=Resources.Strings.ItemInfo%></h1>
        <asp:Label ID="lblPriceInquireItemTitle" runat="server"  />
      

           <%--Order detail--%>
        <asp:GridView ID="grdPriceInquirelist" runat="server" DataSourceID="dsItem" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                cellspacing="0" rules="rows" style="width: 343px; font-size: 9pt; border: thin solid #c0c0c0; border-collapse: collapse;">
            <Columns>
                <asp:BoundField HeaderText="<%$Resources:Strings, ItemNum %>" DataField="item_id_o" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="144px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"/>
                <asp:BoundField HeaderText="<%$Resources:Strings, Description %>" DataField="itemDesc_view" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:TemplateField HeaderText="<%$Resources:Strings, EachCase %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                         HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsQty" Text='<%# CString(Eval("eaPerCs"))  %>' />
                    </ItemTemplate>
                </asp:TemplateField> 
              
                <asp:TemplateField HeaderText="<%$Resources:Strings, Price %>" HeaderStyle-CssClass="th number48" ItemStyle-CssClass="number48" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="200px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true">
                    <ItemTemplate>
                       <asp:Label runat="server" ID="lblCsPrice" Text='<%# Displayformatgdv(Val(Eval("lbPrice_OrderByQty")), Val(Eval("csPrice")), Val(Eval("eaPrice")), Eval("scaledItem")="Y") %>'  />
                       
                    </ItemTemplate>
                </asp:TemplateField>
            
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="<%$Resources:Strings, Notfound %>"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlPriceInquirelist" runat="server"></asp:SqlDataSource>
         
        <asp:Label ID="lblMsg_PriceInquirelist" style="font-size: 10pt;" runat="server" />
    </asp:Panel>
    <br />
    <asp:Label ID="Label8" runat="server" />
    <br />

    </asp:Panel>