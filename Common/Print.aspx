<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Print.aspx.vb" Inherits="Common_Print" %>
<%@ Import Namespace="Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Styles/Site.css?v=<%= Common.VERSION %>" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Koscus.css?v=<%= Common.VERSION %>" rel="stylesheet" type="text/css" />
</head>
<body class="body_print">
    <form id="form1" runat="server">
        <asp:Panel ID="pnlOrderForm" runat="server" >
            <p class='bold'>Order Form</p>
            <asp:GridView ID="grdOrderForm" runat="server" DataSourceID="dsOrderForm" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeader="false" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false" >
                <Columns>
                    <asp:BoundField HeaderText="Category" DataField="ctgName_view" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16 bold top" />
                    <asp:TemplateField HeaderText="Item #" HeaderStyle-CssClass="th">
                        <ItemTemplate>
                            <asp:Label runat="server" CssClass="cellLbl bold" ID="lblVendorName" Text='<%# Eval("vendorName_view") %>' /><br />
                            <asp:Label runat="server" CssClass="cellLbl" ID="lblItemIdNrView" Text='<%# Eval("itemIdNr_view") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:BoundField HeaderText="Item Description" DataField="itemDesc_view" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size48 cellLbl" />--%>
                    <asp:TemplateField HeaderText="Item Description" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" >
                        <ItemTemplate>
                            <asp:Label runat="server" CssClass="cellLbl" ID="lblItemDesc" Text='<%# Eval("itemDesc_view") %>' />
                            <asp:Label runat="server" ID="lblJfcRemark" CssClass="cellLbl" Text='<%# "<br />(" & CString(Eval("jfcRemark")) & ")" %>' Visible='<%# CString(Eval("jfcRemark")) <> "" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl" />
                    <asp:TemplateField HeaderText="Ea/Cs" HeaderStyle-CssClass="th number12" ItemStyle-CssClass="number12" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblEaPerCs" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# Eval("eaPerCs") & " ea/cs<br />"%>' />
                            <asp:Label runat="server" ID="lblAvgCsWgt" Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0 AndAlso Val(Eval("csprice")) <> 0 AndAlso Val(Eval("avgcswgt")) <> 0 %>' Text='<%# Eval("avgcswgt", "{0:N}") & " lbs/cs<br />" %>' />
                            <asp:Label runat="server" ID="lblAvgEaWgt" Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0 AndAlso Val(Eval("eaprice")) <> 0 AndAlso Val(Eval("avgeawgt")) <> 0 %>' Text='<%# Eval("avgeawgt", "{0:N}") & " lbs/pc<br />" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 cellLbl bold" >
                        <ItemTemplate>
                            <%--<asp:Label runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csprice")) <> "0"%>' />--%>
                            <asp:Label runat="server" ID="lblCsPrice" Text='<%# Eval("csprice", "{0:C}") %>' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                            <%--<asp:Label runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csprice")) <> "0"%>' />--%><asp:Label ID="Label1" runat="server" Text='<br />' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                            <%--<asp:Label runat="server" Text='(' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />--%>
                            <asp:Label runat="server" ID="lblEaPrice" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                            <%--<asp:Label runat="server" Text=')' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />--%>
                            <asp:Label ID="Label2" runat="server" Text='<br />' Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                            <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" Text='<%# Eval("lbPrice_OrderByWgt", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                            <asp:Label runat="server" ID="lblLbPrice_OrderByQty" Text='<%# Eval("lbPrice_OrderByQty", "{0:C}") %>' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' CssClass="italic" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size8" ItemStyle-CssClass="size8 cellLbl" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCs" Text='/cs<br />' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                            <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/pc<br />", "/ea<br />") %>' Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                            <asp:Label runat="server" ID="lblLb_orderByWgt" Text='/lb' Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                            <asp:Label runat="server" ID="lblLb_orderByQty" Text='/lb' Visible='<%# CString(Eval("lbPrice_OrderByQty")) <> "0"%>' CssClass="italic" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12 cellLbl" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCs" Text='<%# "<br />" & CString(Eval("shipDate_fmt")) %>' Visible='<%# NOT (CString(Eval("lastCsQty")) = "" AndAlso CString(Eval("lastEaQty")) = "" AndAlso CString(Eval("lastLbs")) = "") %>' />
                        </ItemTemplate><%--'<br />Last qty'--%>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCsQty" CssClass="cellLbl" Visible='<%# CString(Eval("csprice")) <> "0"%>' Text='[____] cs' /><br />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label runat="server" ID="lblLastCsQty" CssClass="gray" Visible='<%# CString(Eval("csprice")) <> "0"%>' Text='<%# Eval("lastCsQty")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Each" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblEaQty" CssClass="cellLbl" Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='[____]' />
                            <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "pc", "ea") %>' Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs"))%>' /><br />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label runat="server" ID="lblLastCsQty" CssClass="gray" Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs"))%>' Text='<%# Eval("lastEaQty")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Lb" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblLbs" CssClass="cellLbl"  Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0" %>' Text='[____] lbs' /><br />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label runat="server" ID="lblLastCsQty" CssClass="gray" Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' Text='<%# Eval("lastLbs")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="dsOrderForm" runat="server"></asp:SqlDataSource>
            <br />   
        </asp:Panel>    
    </form>
</body>
</html>
