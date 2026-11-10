<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EmailBody.ascx.vb" Inherits="Common_EmailBody" %>
<%@ Import Namespace="Common" %>

<%--
Email HTML: every style is inline (no <style> block), layout is tables, so it looks the same in Outlook, Gmail and phone mail apps.
Look: gray page, one white card (max 680px), orange header bar with the company name, info boxes with an orange title,
grids with an orange-tinted header row and light row lines.
Info box label cell:  padding:7px 12px; border-bottom:1px solid #F0F0F0; color:#777777; font-size:13px; vertical-align:top; width:36%;
Info box value cell:  padding:7px 12px; border-bottom:1px solid #F0F0F0; color:#222222; font-size:13px; vertical-align:top;
Only one panel below is shown per email (Page_Init in the code-behind).
--%>
<div style="margin: 0; padding: 20px 0; background-color: #F4F4F4; font-family: Arial, Helvetica, sans-serif; color: #333333;">
<table role="presentation" align="center" width="680" cellpadding="0" cellspacing="0" border="0"
    style="width: 100%; max-width: 680px; margin: 0 auto; background-color: #FFFFFF; border: 1px solid #E5E5E5; border-collapse: collapse; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
<tr><td style="padding: 0;">

<asp:Panel runat="server" ID="pnlOrder">
    <%--Header--%>
    <asp:Panel ID="pnlJfcLogo_order" runat="server">
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
            <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
        </tr></table>
    </asp:Panel>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_order" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <asp:Label ID="lblLink_order" runat="server" />

    <%--Order info--%>
    <asp:Panel ID="pnlOrderHdr" runat="server">
        <asp:Label ID="lblCustomerID" runat="server" Visible="false" />
        <asp:Label ID="lblCustGrpVendorID" runat="server" Visible="false" />
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width: 100%; border: 1px solid #E5E5E5; border-collapse: collapse; font-family: Arial, Helvetica, sans-serif;">
            <tbody>
                <tr><td colspan="2" style="padding: 10px 12px; background-color: #FFF4EC; border-bottom: 2px solid #FF6600; color: #FF6600; font-size: 15px; font-weight: bold;">Order Info</td></tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.WebOrderID%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblWebOrderID" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Status%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblOrderState" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Delivery%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblDeliveryDate" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <%--PlaceHolder (not Panel): a Panel puts a <div> around the row, which is not allowed inside a table--%>
                <asp:PlaceHolder ID="pnlVendorName" runat="server">
                    <tr>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Vendor%></td>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblVendorName" runat="server" style="font-weight: bold;" /></td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><asp:Label ID="lblTitleItemGrp" runat="server" Text="Order Form"/></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemGrpName" runat="server" /></td>
                </tr>
                <asp:PlaceHolder ID="pnlShowPrice" runat="server">
                    <tr>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.TotalAmount%></td>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblTotalAmt" runat="server" style="font-weight: bold; font-size: 15px; color: #CC0000;" /></td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Message%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMessage" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.CustPO%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblCustOrderNr" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Name%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblCustomerName" runat="server" style="font-weight: bold;" /><asp:PlaceHolder ID="pnlCustGrpName" runat="server"><br /><span style="color: #777777;">(<asp:Label ID="lblCustGrpName" runat="server" />)</span></asp:PlaceHolder></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Account%> #</td><%--<%# GetCustCompanyType() & " #"%>--%>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblJfcBranchNr" runat="server" />-<asp:Label ID="lblJfcCustomerNr" runat="server" /></td>
                </tr>
                <asp:PlaceHolder ID="pnlCustShipPoint" runat="server">
                    <tr>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Shipto%></td>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblShipName" runat="server" style="font-weight: bold;" /><asp:Label ID="lblShipPointID" Visible="false" runat="server" />
                            <br /><span style="color: #666666;"><asp:Label ID="lblAddress1" runat="server" /> <asp:Label ID="lblAddress2" runat="server" /></span>
                            <br /><span style="color: #666666;"><asp:Label ID="lblCity" runat="server" />, <asp:Label ID="lblState" runat="server" /> <asp:Label ID="lblZip" runat="server" /></span>
                        </td>
                    </tr>
                    <%--<asp:Label ID="lblCountry" runat="server" />--%>
                </asp:PlaceHolder>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Orderedon%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblCreateDateTimeUser" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.LastUpdated%></td>
                    <td style="padding: 7px 12px; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblModDateUser" runat="server" /></td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <div style="height: 20px; line-height: 20px; font-size: 1px;">&nbsp;</div>

    <asp:Panel ID="pnlOrderDtl" runat="server">
        <%--Order detail--%>
        <asp:GridView ID="grdItem" runat="server" DataSourceID="dsItem" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                CellPadding="8" BorderColor="#E5E5E5"
                style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #333333; border: 1px solid #E5E5E5; border-collapse: collapse;">
            <HeaderStyle BackColor="#FFF4EC" ForeColor="#FF6600" Font-Bold="true" />
            <AlternatingRowStyle BackColor="#FAFAFA" />
            <Columns>
                <asp:BoundField HeaderText="<%$ Resources:Strings, ItemNum %>" DataField="itemIdNr_view" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"/>
                <asp:BoundField HeaderText="<%$ Resources:Strings, Description %>" DataField="itemDesc_view" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <%--<asp:TemplateField HeaderText="Each/Case" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 gray" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEaPerCs" CssClass="cellLbl" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# CString(Eval("eaPerCs"))%>' />
                        <asp:Label runat="server" ID="Label0" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='ea/cs' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Price %>" HeaderStyle-CssClass="th number12" ItemStyle-CssClass="number12" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
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
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCs" Text='/cs' Visible='<%# CString(Eval("csQty")) <> "0"%>' /><asp:Label ID="Label3" runat="server" Text='<br />' Visible='<%# CString(Eval("csQty")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/pc", "/ea") %>' Visible='<%# CString(Eval("eaQty")) <> "0"%>' /><asp:Label ID="Label4" runat="server" Text='<br />' Visible='<%# CString(Eval("eaQty")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblLb_orderByWgt" Text='/lb' Visible='<%# CString(Eval("lbs")) <> "0"%>' />
                        <asp:Label runat="server" ID="lblLb_orderByQty" Text='/lb' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Qty %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10 bold" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
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
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
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
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLineAmt" CssClass="cellLbl" Text='<%# Eval("lineAmt", "{0:C}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, Avail %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail_view")) & "<br />" %>' />
                        <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail_view")) & "<br />" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCs2" Text="cs" /><br />
                        <asp:Label runat="server" ID="lblEa2" Text="ea" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:Strings, OutOfStock %>" HeaderStyle-CssClass="th center10" ItemStyle-CssClass="center10 red bold" 
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000">
                    <ItemTemplate>
                        <%--<asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text="Yes" Visible='<%# Val(Eval("remainingInStockQty")) < 0 %>' />--%>
                        <asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text='<%# IIf(Val(Eval("remainingInStockQty")) < 0, "Y", "") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField HeaderText="Invoice" DataField="invoice" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="<%$ Resources:Strings, Notfound %>"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsItem" runat="server"></asp:SqlDataSource>
        <%--Total line under the grid--%>
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width: 100%; border-collapse: collapse;"><tr>
            <td align="right" style="padding: 10px 12px; background-color: #FAFAFA; border: 1px solid #E5E5E5; border-top: none; text-align: right; font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #333333;"><asp:Label ID="lblOrderMsg" runat="server" /></td>
        </tr></table>
    </asp:Panel>
    <div style="height: 20px; line-height: 20px; font-size: 1px;">&nbsp;</div>
    <%--<asp:Button ID="btnConfirm" CssClass="midButton largeBtn" runat="server" Text="Confirm Order" />--%>

    <asp:Label ID="lblLink2_order" runat="server" />
    </td></tr></table>
</asp:Panel>

<asp:Panel runat="server" ID="pnlPriceInquiry" >
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_priceInquiry" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <asp:Label ID="lbllink_priceInquiry" runat="server" />
    <%--Price Inquiry--%>
    <asp:GridView ID="grdPriceInquiry" runat="server" DataSourceID="dsPriceInquiry" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
            CellPadding="8" BorderColor="#E5E5E5"
            style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #333333; border: 1px solid #E5E5E5; border-collapse: collapse;">
        <HeaderStyle BackColor="#FFF4EC" ForeColor="#FF6600" Font-Bold="true" />
        <AlternatingRowStyle BackColor="#FAFAFA" />
        <Columns>
            <asp:BoundField HeaderText="JFC Item #" DataField="item_id" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="JFC Item Description" DataField="itemDesc" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Customer Item #" DataField="itemNr" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Customer Item Description" DataField="custItemDesc" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32 cellLbl gray" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Case Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Each Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:BoundField HeaderText="Lb Price" DataField="blank" HeaderStyle-CssClass="th center24" ItemStyle-CssClass="center24" 
                    HeaderStyle-HorizontalAlign="Center" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="Avail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail")) & "<br />" %>' />
                    <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail")) & "<br />" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text="cs" /><br />
                    <asp:Label runat="server" ID="lblEa" Text="ea" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField HeaderText="Avail" DataField="csAvail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text="cs" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="" DataField="eaAvail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
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
    </td></tr></table>
</asp:Panel>

<asp:Panel runat="server" ID="pnlPay">
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_pay" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <%--Payment info--%>
    <asp:Panel ID="pnlPayHdr" runat="server">
        <asp:Label ID="lblPaymentID" runat="server" Visible="false" />
        <asp:FormView ID="frmPayHdr" runat="server" Width="100%">
            <ItemTemplate>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width: 100%; border: 1px solid #E5E5E5; border-collapse: collapse; font-family: Arial, Helvetica, sans-serif;">
                    <tbody>
                        <tr><td colspan="2" style="padding: 10px 12px; background-color: #FFF4EC; border-bottom: 2px solid #FF6600; color: #FF6600; font-size: 15px; font-weight: bold;">Payment Info</td></tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Payment ID</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("payment_id")%></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Account Name</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("customerName")%></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">JFC Account #</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("jfcBranchNr")%>-<%# Eval("jfcCustomerNr")%></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Total Paid Amount</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><span style="font-weight: bold; font-size: 15px; color: #CC0000;"><%# Eval("paymentAmt", "{0:$#,#.00}")%>&nbsp;<%# Eval("approvedAmtDesc")%></span></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Advance Payment</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><span style="font-weight: bold;"><%# Eval("advancePayAmt", "{0:$#,#.00}")%></span></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Status</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><span style="font-weight: bold;"><%# Eval("status")%></span></td>
                        </tr>
                        <%--<tr>
                            <td >Result</td>
                            <td><%# Eval("result")%></td>
                        </tr>--%>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Message to JFC</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("message")%></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Account</td>
                            <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("accNickName")%></td>
                        </tr>
                        <tr>
                            <td style="padding: 7px 12px; color: #777777; font-size: 13px; vertical-align: top; width: 36%;">Paid on</td>
                            <td style="padding: 7px 12px; color: #222222; font-size: 13px; vertical-align: top;"><%# Eval("createDateTimeUser")%></td>
                        </tr>
                    </tbody>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </asp:Panel>
    <div style="height: 20px; line-height: 20px; font-size: 1px;">&nbsp;</div>

    <asp:Panel ID="pnlPayDtl" runat="server">
        <%--Payment detail--%>
        <asp:GridView ID="grdPayDtl" runat="server" DataSourceID="dsPayDtl" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                CellPadding="8" BorderColor="#E5E5E5"
                style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #333333; border: 1px solid #E5E5E5; border-collapse: collapse;">
            <HeaderStyle BackColor="#FFF4EC" ForeColor="#FF6600" Font-Bold="true" />
            <AlternatingRowStyle BackColor="#FAFAFA" />
            <Columns>
                <asp:BoundField DataField="arDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField DataField="reference" HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="note" HeaderText="Note" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
                <asp:TemplateField HeaderText="Web Discount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblWebDiscAmt" CssClass="cellLbl" Text='<%# Eval("webDiscAmt", "{0:C}") %>' Visible='<%# Common.Val(Eval("webDiscAmt")) <> 0 %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="paymentLineAmt" DataFormatString="{0:$#,#.00}" HeaderText="Paid Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"  
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsPayDtl" runat="server"></asp:SqlDataSource>
        <div style="height: 20px; line-height: 20px; font-size: 1px;">&nbsp;</div>
    </asp:Panel>

    <asp:Panel ID="pnlBalanceDtl" runat="server">
        <%--Message--%>
        <div style="padding: 0 0 8px 0; font-size: 15px; font-weight: bold; color: #FF6600;"><asp:Label ID="Label9" Text="Here is the remaining balance." runat="server" /></div>
        <%--Payment detail--%>
        <asp:GridView ID="grdBalanceDtl" runat="server" DataSourceID="dsBalanceDtl" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                CellPadding="8" BorderColor="#E5E5E5"
                style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #333333; border: 1px solid #E5E5E5; border-collapse: collapse;">
            <HeaderStyle BackColor="#FFF4EC" ForeColor="#FF6600" Font-Bold="true" />
            <AlternatingRowStyle BackColor="#FAFAFA" />
            <Columns>
                <asp:BoundField DataField="arDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <%--<asp:BoundField DataField="reference" HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />--%>
                <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" />
                <asp:BoundField DataField="balance" DataFormatString="{0:$#,#.00}" HeaderText="Remaining Balance" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20"  
                    HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#FF0000" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsBalanceDtl" runat="server"></asp:SqlDataSource>
    </asp:Panel>
    </td></tr></table>
</asp:Panel>

<asp:Panel ID="pnlNewItem" runat="server">
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <%--<asp:Image ID="Image1" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/jfcLogo.gif" />--%><%--<br /><br />
    <a target="_blank" href='http://shop.jfc.com'>https://shop.jfc.com</a>--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--New JFC Items--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="Label3" Text="Here are our new items! For details, please go to " runat="server" /><a target="_blank" href='http://shop.jfc.com' style="color: #FF6600; font-weight: bold;">https://shop.jfc.com</a><asp:Label ID="Label4" Text=" or reply to contact your sales rep." runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <asp:GridView ID="grdNewItem" runat="server" DataSourceID="dsNewItem" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeader="false" AllowSorting="false" AllowPaging="false"
            CellPadding="12" BorderColor="#EEEEEE"
            style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333; border: 1px solid #EEEEEE; border-collapse: collapse;">
        <RowStyle VerticalAlign="Top" />
        <Columns>
            <asp:TemplateField ItemStyle-Width="172px">
                <ItemTemplate>
                    <asp:Image ID="imgItem" Width="160px" Height="160px" style="display: block; border: 1px solid #EEEEEE;" runat="server" ImageUrl='<%# GetNewItemImgUrl(CString(Eval("item_id"))) %>' />
                    <%--<asp:Image ID="imgItem" CssClass="midGrdImg" runat="server" ImageUrl="https://shop.jfc.com/Images/Web/noImg.jpg" />Doesn't work here--%>
                    <%--<asp:Image ID="imgItem" CssClass="midGrdImg" runat="server" ImageUrl="https://shop.jfc.com/Login/Images/NewItems/noImg.jpg" />Works--%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblItemDesc" style="font-size: 15px; font-weight: bold; color: #222222;" Text='<%# Eval("itemDesc_view") %>' />
                    <asp:Label runat="server" ID="lblItemIdNr" style="color: #777777; font-size: 13px;" Text='<%# Eval("itemIdNr_view") %>' /><br />
                    <%--<asp:Label runat="server" ID="lblItemGrpName" CssClass="cellLbl gray" Text='<%# CString(Eval("itemGrpName")) & "<br />" %>' />
                    <asp:Label runat="server" ID="lblVendorName" CssClass="cellLbl" Text='<%# CString(Eval("vendorName")) & "<br />" %>' />--%>
                    <asp:Label runat="server" ID="lblCtgName" style="color: #999999; font-size: 12px;" Text='<%# Eval("ctgName_view") %>' />
                    <%--<asp:ImageButton ID="btnFavorite" ToolTip="Add or remove from my favorite" CommandName="FAVORITE" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl_OnOff("FAVORITE", CString(Eval("favorite"))) %>' CommandArgument='<%# Eval("custGrpItem_id")%>' runat="server" />
                    <asp:Image ID="imgOrdered" ToolTip="Previously ordered item" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("greenCheck.png") %>' Visible='<%# CString(Eval("ordered")) = "ON" %>' runat="server" />--%><br />
                    <asp:Label ID="Label1" runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblCsPrice" style="color: #CC0000; font-weight: bold; font-size: 15px;" Text='<%# Eval("csprice", "{0:C}") %>' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblCs" CssClass="cellLbl" Text='/ cs' style="margin: 3px;" Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblEaPrice" style="color: #CC0000; font-weight: bold; font-size: 15px;" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("eaprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/ pc", "/ ea") %>' Visible='<%# CString(Eval("eaprice")) <> "0"%>' />
                    <asp:Label ID="Label2" runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" style="color: #CC0000; font-weight: bold; font-size: 15px;" Text='<%# Eval("LbPrice_OrderByWgt", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLb_orderByWgt" CssClass="cellLbl" style="margin: 3px;" Text='/ lb' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLbPrice_orderByQty" style="color: #CC0000; font-weight: bold; font-size: 15px;" Text='<%# Eval("LbPrice_OrderByQty", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLb_orderByQty" CssClass="cellLbl" style="margin: 3px;" Text='/ lb' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' />
                    <%--<asp:Label runat="server" ID="lblAskPrice" style="color: #CC0000; font-weight: bold; font-size: 15px;" Text='Price: Ask' Visible='<%# CString(Eval("askPrice")) = "Y"%>' /><br />--%>
                    <%--<asp:Image ID="imgCart" ToolTip="Exists in cart" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("cart.png") %>' Visible='<%# CString(Eval("cart_id")) <> "" %>' runat="server" />--%>
                    <%--Don't show out of stock for new items--%>
                    <%--<asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl green bold" Text='Out of Stock' Visible='<%# CString(Eval("outOfStock")) = "Y"%>' />--%>
                    <asp:Label runat="server" ID="lblArrivDate" style="color: #2E7D32; font-size: 13px;" Text='<%# "Est. arrival: " & Eval("arrivDate_fmt") %>' Visible='<%# CString(Eval("arrivDate_fmt")) <> "" %>' />
                    <asp:Label runat="server" ID="lblJfcRemark" style="color: #2E7D32; font-size: 13px;" Text='<%# Eval("jfcRemark") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsNewItem" runat="server"></asp:SqlDataSource>
    </td></tr></table>
</asp:Panel>


<asp:Panel ID="PnlUpdate" runat="server">
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_Item" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <%--Item info--%>
    <asp:Panel ID="pnlItemHdr" runat="server">
        <asp:Label ID="lblItemID" runat="server" />
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width: 100%; border: 1px solid #E5E5E5; border-collapse: collapse; font-family: Arial, Helvetica, sans-serif;">
            <tbody>
                <tr><td colspan="2" style="padding: 10px 12px; background-color: #FFF4EC; border-bottom: 2px solid #FF6600; color: #FF6600; font-size: 15px; font-weight: bold;"><%=Resources.Strings.ItemInfo%></td></tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.ItemNum%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemCode" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.ItemNewNum%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemCodeNew" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Description%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemDesc" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.DescNew%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemDescNew" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Remark%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemRemark" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.EachCase%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemEachCase" runat="server" /></td>
                </tr>
                <%--PlaceHolder (not Panel): a Panel puts a <div> around the row, which is not allowed inside a table--%>
                <asp:PlaceHolder ID="pnlItemPriceNew" runat="server">
                    <tr>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Price%></td>
                        <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblItemPriceNew" runat="server" style="font-weight: bold; font-size: 15px; color: #CC0000;" /></td>
                    </tr>
                </asp:PlaceHolder>
            </tbody>
        </table>
    </asp:Panel>
    </td></tr></table>
</asp:Panel>


<asp:Panel ID="PnlMaster" runat="server">
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_Master" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <%--Delivery schedule--%>
    <asp:Panel ID="PnlMasterContant" runat="server">
        <asp:Label ID="Label6" runat="server" />
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width: 100%; border: 1px solid #E5E5E5; border-collapse: collapse; font-family: Arial, Helvetica, sans-serif;">
            <tbody>
                <tr><td colspan="2" style="padding: 10px 12px; background-color: #FFF4EC; border-bottom: 2px solid #FF6600; color: #FF6600; font-size: 15px; font-weight: bold;"><%=Resources.Strings.DeliverySchedule%></td></tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Customer%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterCustomer" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.JFCAccountNum%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterJFCAcc" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.ShippingDestination%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterShipDest" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Address1%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterAddr1" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Address2%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterAddr2" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.City%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterCity" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.State%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterState" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Zip%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterZip" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Country%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterCountry" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Vendor%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterVendor" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.SendReminder%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterSendReminder" runat="server" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.Frequency%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterFrequency" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.DeliveryOn%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterDeliveryOn" runat="server" style="font-weight: bold;" /></td>
                </tr>
                <tr>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #777777; font-size: 13px; vertical-align: top; width: 36%;"><%=Resources.Strings.OrderBy1%></td>
                    <td style="padding: 7px 12px; border-bottom: 1px solid #F0F0F0; color: #222222; font-size: 13px; vertical-align: top;"><asp:Label ID="lblMasterOrderby" runat="server" style="font-weight: bold;" /></td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    </td></tr></table>
</asp:Panel>



<asp:Panel ID="PnlPriceInquireList" runat="server">
    <%--Header--%>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td style="padding: 16px 24px; background-color: #FF6600; color: #FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size: 20px; font-weight: bold; font-style: italic;"><%= Common.JFC_COMPANY_NAME%></td>
    </tr></table>
    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 20px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5; color: #333333;">
    <%--Message--%>
    <div style="font-size: 15px; line-height: 1.6; color: #333333;"><asp:Label ID="lblMsg_PriceInquireListAdd" runat="server" /></div>
    <div style="height: 16px; line-height: 16px; font-size: 1px;">&nbsp;</div>
    <%--Items--%>
    <asp:Panel ID="pnlPriceInquireItems" runat="server">
        <div style="padding: 0 0 8px 0; font-size: 15px; font-weight: bold; color: #FF6600;"><%=Resources.Strings.ItemInfo%></div>
        <asp:Label ID="lblPriceInquireItemTitle" runat="server" />
        <asp:GridView ID="grdPriceInquirelist" runat="server" DataSourceID="dsItem" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"
                CellPadding="8" BorderColor="#E5E5E5"
                style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #333333; border: 1px solid #E5E5E5; border-collapse: collapse;">
            <HeaderStyle BackColor="#FFF4EC" ForeColor="#FF6600" Font-Bold="true" />
            <AlternatingRowStyle BackColor="#FAFAFA" />
            <Columns>
                <asp:BoundField HeaderText="<%$Resources:Strings, ItemNum %>" DataField="item_id_o" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"/>
                <asp:BoundField HeaderText="<%$Resources:Strings, Description %>" DataField="itemDesc_view" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48 cellLbl gray" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                <asp:TemplateField HeaderText="<%$Resources:Strings, EachCase %>" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                         HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsQty" Text='<%# CString(Eval("eaPerCs"))  %>' />
                    </ItemTemplate>
                </asp:TemplateField> 
              
                <asp:TemplateField HeaderText="<%$Resources:Strings, Price %>" HeaderStyle-CssClass="th number48" ItemStyle-CssClass="number48" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
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
        <div style="padding-top: 12px; font-size: 14px;"><asp:Label ID="lblMsg_PriceInquirelist" runat="server" /></div>
    </asp:Panel>
    <asp:Label ID="Label8" runat="server" />
    </td></tr></table>
</asp:Panel>

</td></tr>
</table>
</div>
