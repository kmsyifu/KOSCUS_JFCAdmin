<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentDtl.ascx.vb" Inherits="Common_PaymentDtl" %>
<%@ Import Namespace="Common" %>

<h2>View Payment</h2>
<%--Payment Header--%>
<asp:Panel ID="pnlPayHdr" CssClass="visiblePnl" runat="server"><%--Width="500px"--%> 
    <h1>Payment Info</h1>
    <asp:Label ID="lblPaymentID" Visible="False" runat="server" />
    <asp:Label ID="lblCustomerID" Visible="False" runat="server" />
    <asp:FormView ID="frmPayHdr" RunAt="server">
        <ItemTemplate>
            <table>
                <tbody>
                    <tr>
                        <td class="rowHdr right">Payment ID</td>
                        <td><%# Eval("payment_id")%></td>
                    </tr>
                    <%--<asp:Panel ID="Panel3" runat="server" Visible='<%# NOT IsMobile() %>'>--%>
                    <tr>
                        <td class="rowHdr right">Account Name</td>
                        <td><%# Eval("customerName")%></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">JFC Account #</td>
                        <td><%# Eval("jfcBranchNr")%>-<%# Eval("jfcCustomerNr")%></td>
                    </tr>
                    <%--</asp:Panel>--%>
                    <tr>
                        <td class="rowHdr right">Total Paid Amount</td>
                        <td><span class='bold red'><%# Eval("paymentAmt", "{0:$#,#.00}")%>&nbsp;<%# Eval("approvedAmtDesc")%></span></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Advance Payment</td>
                        <td><span class='bold'><%# Eval("advancePayAmt", "{0:$#,#.00}")%></span></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Status</td>
                        <td><span class='bold'><%# Eval("status")%></span></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Result</td>
                        <td><%# Eval("result")%></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Message to JFC</td>
                        <td><%# Eval("message")%></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Account</td>
                        <td><%# Eval("accNickName")%></td>
                    </tr>
                    <tr>
                        <td class="rowHdr right">Paid on</td>
                        <td><%# Eval("createDateTimeUser")%></td>
                    </tr>
                </tbody>
            </table>
        </ItemTemplate>
    </asp:FormView>
</asp:Panel>
<br />

<%--Payment detail--%>
<asp:GridView ID="grdPayDtl" runat="server" DataSourceID="dsPayDtl" 
    AutoGenerateColumns="False" CssClass="STDTABLE" GridLines="Horizontal" 
    ShowHeaderWhenEmpty="false" AllowPaging="false">
    <Columns>
        <asp:BoundField DataField="arDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
        <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />
        <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
        <%--<asp:TemplateField HeaderText="Liquor" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16">
            <ItemTemplate>
                <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>' Visible='<%# CString(Eval("isLiquor")) = "Y" %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>--%>
        <asp:TemplateField HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" >
            <ItemTemplate>
                <asp:Label runat="server" ID="lblRefWoLink" CssClass="cellLbl" Text='<%# Eval("reference") %>' Visible='<%# NOT (CString(Eval("allowCustDownloadInvoice")) = "Y" AndAlso CString(Eval("fileName")) <> "") %>' />
                <asp:LinkButton runat="server" ID="lnkRefWLink" CommandName="DOWNLOAD" CommandArgument='<%# Eval("reference")%>' CssClass="cellLink" ForeColor="#4169E1" 
                    ToolTip='<%# "Download " & CString(Eval("fileName")) %>' Text='<%# Eval("reference") %>' Visible='<%# CString(Eval("allowCustDownloadInvoice")) = "Y" AndAlso CString(Eval("fileName")) <> "" %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" />
        <%--<asp:BoundField DataField="discAmt" DataFormatString="{0:$#,#.00}" HeaderText="Discount Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" />--%>
        <asp:TemplateField HeaderText="Web Discount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" >
            <ItemTemplate>
                <asp:Label runat="server" ID="lblWebDiscAmt" Text='<%# Eval("webDiscAmt", "{0:$#,#.00}")%>' Visible='<%# Common.Val(Eval("webDiscAmt")) <> 0 %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="paymentLineAmt" DataFormatString="{0:$#,#.00}" HeaderText="Paid Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20 red bold" />
        <asp:BoundField DataField="note" HeaderText="Note" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
    </Columns>
    <EmptyDataTemplate>
        <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
    </EmptyDataTemplate>
</asp:GridView>
<asp:SqlDataSource ID="dsPayDtl" runat="server" />
<br />
<asp:Button ID="btnVoidPayment" CssClass="midButton" runat="server" Text="Void Payment" />
<asp:Label runat="server" ID="lblResults" Visible="false" />
