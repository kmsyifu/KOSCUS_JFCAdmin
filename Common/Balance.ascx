<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Balance.ascx.vb" Inherits="Common_Balance" %>
<%@ Register Tagprefix="uc" Tagname="PaymentDtl" Src="~/Common/PaymentDtl.ascx" %>
<%@ Import Namespace="Common" %>

<script type="text/javascript">
    function calculateTotalPayAmt() {
        var chkCol, payAmtCol, starCol, webDiscAmtCol, noWebDiscCol;
        var payAmt, balance, advancePayAmt;
        var totalPayAmt = 0;
        var str;

        //If AllowCustInputPayAmt is "N", don't calculate total payment amount while entering adance payment. Instead use Apply button
        /*if ("<%=hidAllowCustInputPayAmt.Value %>" == "N") {
            return;
        }*/

        //Get total payment amount from gridview
        var grd = document.getElementById('<%=grdPayDtl.ClientID %>');
        if (grd == null) { return; }
        for (i = 1; i < grd.rows.length; i++) { // Start with 1st row
            chkCol = grd.rows[i].cells[0]; //Checkbox
            for (k = 0; k < chkCol.childNodes.length; k++) {
                if (chkCol.childNodes[k].type == "image" && chkCol.childNodes[k].src.substr(chkCol.childNodes[k].src.length - "checked.png".length) == "checked.png") {
                    if ("<%=hidIsMobile.Value %>" == "Y") {
                        // Get total payment amount from remaining balance
                        balCol = grd.rows[i].cells[7]; //Balance
                        balance = (balCol.innerText || balCol.textContent).replace(/\$|,/g, ''); //Remove $ and comma
                        if (!isNaN(balance) && balance != "") totalPayAmt += parseFloat(balance);
                    } else if ("<%=hidAllowCustInputPayAmt.Value %>" == "N") {
                        // Get total payment amount from remaining balance
                        balCol = grd.rows[i].cells[10]; //Balance
                        balance = (balCol.innerText || balCol.textContent).replace(/\$|,/g, ''); //Remove $ and comma
                        if (!isNaN(balance) && balance != "") totalPayAmt += parseFloat(balance);
                    } else {
                        // Get total payment amount from input pay amount
                        balCol = grd.rows[i].cells[10]; //Balance
                        balance = (balCol.innerText || balCol.textContent).replace(/\$|,/g, ''); //Remove $ and comma
                        payAmtCol = grd.rows[i].cells[11]; //Payment Amount
                        for (j = 0; j < payAmtCol.childNodes.length; j++) {
                            if (payAmtCol.childNodes[j].type == "text") {
                                payAmt = payAmtCol.childNodes[j].value.replace(/\,/g, '');  //Remove comma
                                if (!isNaN(payAmt) && payAmt != "") {
                                    totalPayAmt += parseFloat(payAmt);
                                    // If payAmt < balance, mark note as required field
                                    webDiscAmtCol = grd.rows[i].cells[9]; //Web Discount
                                    starCol = grd.rows[i].cells[13]; //Star
                                    str = parseFloat(balance) > parseFloat(payAmt) ? '*' : '';
                                    starCol.innerText = str;
                                    starCol.textContent = str;
                                    if (grd.rows[i].cells[14] != undefined) {
                                        // If payAmt < balance, show a warning for no web discount
                                        webDiscAmtCol.style.textDecoration = parseFloat(balance) > parseFloat(payAmt) ? 'line-through' : 'none';
                                        // webDiscAmtCol.style.visibility = parseFloat(balance) > parseFloat(payAmt) ? 'hidden' : '';
                                        noWebDiscCol = grd.rows[i].cells[14]; //No web discount warning
                                        str = parseFloat(balance) > parseFloat(payAmt) ? 'No Discount' : '';
                                        noWebDiscCol.innerText = str;
                                        noWebDiscCol.textContent = str;
                                    }
                                }
                            }
                        }
                    }
                }
            } // end of for k
        } // end of for i

        //Add advance payment amount if any
        var txt = document.getElementById('<%=txtAdvancePayAmt.ClientID %>');
        if (txt != null) {
            // txtAdvancePayAmt is visible 
            advancePayAmt = txt.value.replace(/\,/g, '');  //Remove comma
            if (!isNaN(advancePayAmt) && advancePayAmt != "") {
                totalPayAmt += parseFloat(advancePayAmt);
            }
        }
        document.getElementById('<%= lblPayAmt.ClientID %>').innerHTML = "$" + totalPayAmt.toFixed(2).toString();
    }

    //        function btnDLClick() {
    //            if (document.getElementById("MainContent_chkOrderHistory").checked ||
    //                document.getElementById("MainContent_chkInvoice").checked ||
    //                document.getElementById("MainContent_chkCreditMemo").checked ||
    //                document.getElementById("MainContent_chkPriceList").checked) {
    //                hideMsg();
    //                return true;
    //            }
    //            showMsg("ERROR", "Please select any document to download");
    //            return false;
    //        }

    //        function resizeContentArea() {
    //            var pnlDlHeight = getBrowserHeight() - 400;
    //            if (pnlDlHeight < 350) { pnlDlHeight = 350; }
    //            document.getElementById("MainContent_pnlDL").style.height = pnlDlHeight; <%--MOD by JZHOU--%>
    //        }

    //        window.onload = function(){ resizeContentArea(); }
    //        window.onresize = function(){ resizeContentArea(); }
</script>

<%----------------------------------------------------------------------------------------------------------------------
    Show balance
----------------------------------------------------------------------------------------------------------------------%>
<asp:Panel runat="server" ID="pnlTabBalance">
    <div id="tabBalance" onkeydown = "return (event.keyCode!=13)"><%--Disable the enter key form submission on all controls in this div--%>
        <%--List of transaction--%>
        <asp:Panel runat="server" ID="pnlList_balance">
            <h2>Transaction History with JFC</h2>
            <%--Payment confirmation--%>
            <asp:Panel ID="pnlAftPayment" runat="server">
                <asp:Panel ID="pnlAftPay" CssClass="visiblePnl background" Height="120px" runat="server"><%--Width="978px"--%> 
                    <h1 class="bold">Thank you! Your payment has been approved.</h1>
                    <br />
                    <asp:Label ID="lblErrorMsg" runat="server" CssClass="red bold" />
                    <asp:Label ID="lblConfirmMsg" runat="server" CssClass="green" />
                </asp:Panel>
                <br />
            </asp:Panel>

            <%--Select customer--%>
            <asp:Panel runat="server" CssClass="visiblePnl" ID="pnlCustomer"><%--Width="978px"--%> 
                <asp:Label ID="lblCustCompany_balance" runat="server" Text="Company" CssClass="gray" />
                <asp:DropDownList ID="lstCustCompany_balance" CssClass="drpList" runat="server" AutoPostBack="True" />
                <asp:Label ID="lblCustGrp_balance" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType() & " Group" %>' CssClass="gray" />
                <asp:DropDownList ID="lstCustGrp_balance" CssClass="drpList" runat="server" AutoPostBack="True" />
                <asp:Label ID="lblCustomer_balance" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType()%>' CssClass="gray" />
                <asp:DropDownList ID="lstCustomer_balance" CssClass="drpList" runat="server" AutoPostBack="True" />
            </asp:Panel>
            <%= IIf(Session("BALANCE_CMD") = "CUST", "", "<br />")%>

            <%--Current Balance--%>
            <asp:Panel ID="pnlBalance" CssClass="visiblePnl" runat="server"><%--Width="978px"--%> 
                <h1>Balance for JFC</h1>
                <label class="rowHdr right">Balance</label>&nbsp;
                <asp:Label ID="lblCurBalance" CssClass="red bold fontsize6" runat="server" />&nbsp;<asp:Label ID="lblCurPendingAmt" runat="server" />
                <%= IIf(IsMobile(), "<br /><br />", "&nbsp;&nbsp;&nbsp;&nbsp;") %>
                <asp:Button ID="btnPay" CssClass="midButton" Text="View & Pay Balance" runat="server" />&nbsp;&nbsp;
                <asp:Button ID="btnAutoPay" CssClass="midButton" Text="Set Auto Payment" runat="server" />
                <br />
            </asp:Panel>
            <br />

            <%--Pending transactions--%>
            <asp:Panel runat="server" ID="pnlPendingTrans" >
                <h3><%= "Pending Web Payments" & IIf(IsMobile(), "<br />", "&nbsp;")%>
                    <asp:Image ID="Image1" CssClass="xxsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("info.jpg") %>' />
                    <span class='fontsize-2'>It may take 1 to 2 business days to process your payment.</span><%--<%= IIf(IsMobile(), "<br />", "&nbsp;")%>
                    <asp:Image ID="Image2" CssClass="xxsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("cancel.png") %>' />
                    <span class='fontsize-2'>Go to 'Web Payment History' tab to void recent & non-processed payment.</span>--%>
                </h3>
                <asp:GridView ID="grdPending" runat="server" DataSourceID="dsPending" 
                    AutoGenerateColumns="False" CssClass="STDTABLE_SFONT" GridLines="Horizontal" 
                    ShowHeaderWhenEmpty="False" AllowPaging="True" PageSize="50"><%--Width="1000px"--%> 
                    <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
                    <PagerSettings Mode="NumericFirstLast" Position="Bottom" FirstPageText="<<" LastPageText=">>" />
                    <Columns>
                        <asp:BoundField DataField="createDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Transaction Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                        <asp:BoundField DataField="payment_id" HeaderText="Payment ID" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                        <%--<asp:TemplateField HeaderText="Payment ID" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" >
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnkPaymentID" CommandName="VIEW_ROW" CommandArgument='<%# Eval("payment_id")%>' CssClass="cellLink" ForeColor="#4169E1" Text='<%# Eval("payment_id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                         <asp:BoundField DataField="transType" HeaderText="Type" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                        <asp:BoundField DataField="approvedAmt" DataFormatString="{0:$#,#.00}" HeaderText="Amount" HeaderStyle-CssClass="th number24" ItemStyle-CssClass="number24" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="dsPending" runat="server" />
                <br /><br />
            </asp:Panel>

            <%--Posted Transactions--%>
            <h3><asp:Label ID="Label18" runat="server" Text="Transactions" /><%--&nbsp;&nbsp;
                <span class='red'><asp:Label ID="lblBegBalanceTitle" runat="server" Text="Beginning Balance: " /><asp:Label ID="lblBegBalance" runat="server" />&nbsp;&nbsp; 
                    <asp:Label ID="lblCurBalanceTitle" runat="server" Text="Balance: " /><asp:Label ID="lblCurBalance2" runat="server" />
                    <asp:Label ID="lblEndBalanceTitle" runat="server" Text="Ending Balance: " /><asp:Label ID="lblEndBalance" runat="server" />
                </span>--%>
            </h3>
            <asp:Label ID="lblPeriod" runat="server" Text="Period" CssClass="gray" />
            <asp:DropDownList ID="lstPeriod_balance" CssClass="drpList" runat="server" AutoPostBack="True" />
            <asp:Label ID="lblOrderBy_balance" runat="server" Text="Sort by" CssClass="gray" />
            <asp:DropDownList ID="lstOrderBy_balance" CssClass="drpList" runat="server" AutoPostBack="True" />
            <br /><br />
            <asp:GridView ID="grdBalance" runat="server" DataSourceID="dsBalance" 
                AutoGenerateColumns="False" CssClass="STDTABLE_SFONT" GridLines="Horizontal" 
                ShowHeaderWhenEmpty="False" AllowPaging="True" PageSize="50"><%--Width="1000px"--%> 
                <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
                <PagerSettings Mode="NumericFirstLast" Position="Bottom" FirstPageText="<<" LastPageText=">>" />
                <Columns>
                    <asp:BoundField DataField="transdate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Transaction Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                    <asp:BoundField DataField="transType" HeaderText="Type" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                    <%--<asp:ButtonField DataTextField="reference" HeaderText="Reference" CommandName="DOWNLOAD" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32 cellLink" ItemStyle-ForeColor="#4169E1"/>--%>
                    <asp:TemplateField HeaderText="Reference" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRefWoLink" CssClass="cellLbl" Text='<%# Eval("reference") %>' Visible='<%# Not ((CString(Eval("allowCustDownloadInvoice")) = "Y" OrElse Session("BALANCE_CMD") = "JFCADMIN") AndAlso CString(Eval("fileName")) <> "") %>' />
                            <asp:LinkButton runat="server" ID="lnkRefWLink" CommandName="DOWNLOAD" CommandArgument='<%# Eval("reference")%>' CssClass="cellLink" ForeColor="#4169E1" 
                                ToolTip='<%# "Download " & CString(Eval("fileName")) %>' Text='<%# Eval("reference") %>' Visible='<%#  (CString(Eval("allowCustDownloadInvoice")) = "Y" OrElse Session("BALANCE_CMD") = "JFCADMIN") AndAlso CString(Eval("fileName")) <> "" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="amt" DataFormatString="{0:$#,#.00}" HeaderText="Amount" HeaderStyle-CssClass="th number24" ItemStyle-CssClass="number24" />
                    <asp:BoundField DataField="balance" DataFormatString="{0:$#,#.00}" HeaderText="Balance" HeaderStyle-CssClass="th number24" ItemStyle-CssClass="number24" />
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:SqlDataSource ID="dsBalance" runat="server" />
        </asp:Panel>

        <%--Detail of pending payment--%>
        <%--<asp:Panel runat="server" ID="pnlPaymentDtl_balance" >
            <asp:LinkButton ID="lnkPaymentDtlBack_balance" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to transaction history..." ></asp:LinkButton>
            <br /><br />
            <uc:PaymentDtl id="ctrPaymentDtl_balance" runat="Server" />
        </asp:Panel>--%>

        <%--New Payment--%>
        <asp:Panel runat="server" ID="pnlNewPayment_balance">
            <asp:LinkButton ID="lnkNewPaymentBack_balance" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to transaction history..." ></asp:LinkButton>
            <br /><br />
            <h2>Pay Balance for JFC&nbsp;&nbsp;<asp:Label ID="Label1" Text="(SANDBOX)" CssClass="bold red" Visible='<%# Not IsProduction() %>' runat="server" /></h2>
            <%--Payment Header1--%>
            <asp:Panel ID="pnlPayDtl" CssClass="visiblePnl" runat="server">
                <%--<table>
                    <tbody>
                        <tr>
                            <td class="rowHdr">Account Name</td>
                            <td><asp:Label ID="lblCustomerName" CssClass="rowLblVal" runat="server" />
                                (<asp:Label ID="lblJFCBranchNr" CssClass="rowLblVal" runat="server" />-<asp:Label ID="lblJFCCustomerNr" CssClass="rowLblVal" runat="server" />)</td>
                            <td class="rowHdr right size24">Balance</td>
                            <td><asp:Label ID="lblBalance" CssClass="rowLblVal bold" runat="server" /><asp:Label ID="lblPendingAmt" CssClass="rowLblVal" runat="server" /></td>
                            <td class="right size24"><asp:CheckBox runat="server" ID="chkSelectLiquor" Text="&nbsp;Show liquor only" CssClass="green" AutoPostBack="true" /></td>
                       </tr>
                    </tbody>
                </table>--%>
                <asp:Label ID="Label3" CssClass="rowHdr" runat="server" Text="Account Name" />
                <asp:Label ID="Label5" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") %>' />
                <asp:Label ID="lblCustomerName" CssClass="rowLblVal" runat="server" />(<asp:Label ID="lblJFCBranchNr" CssClass="rowLblVal" runat="server" />-<asp:Label ID="lblJFCCustomerNr" CssClass="rowLblVal" runat="server" />)
                <asp:Label ID="Label19" runat="server" Text='<%# IIf(IsMobile(), "<br /><br />", "&nbsp;&nbsp;&nbsp;&nbsp;") %>' />
                <asp:Label ID="Label2" CssClass="rowHdr" runat="server" Text="Balance" />
                <asp:Label ID="Label20" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") %>' />
                <asp:Label ID="lblBalance" CssClass="rowLblVal bold" runat="server" /><asp:Label ID="lblPendingAmt" CssClass="rowLblVal" runat="server" />&nbsp;&nbsp;
                <asp:Label ID="Label11" runat="server" Text='<%# IIf(IsMobile(), "<br /><br />", "") %>' />
                <asp:CheckBox runat="server" ID="chkSelectLiquor" Text="&nbsp;Show liquor only" CssClass="green" AutoPostBack="true" />&nbsp;&nbsp;
                <asp:Label ID="Label9" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") %>' />
                <asp:CheckBox runat="server" ID="chkHideLiquorInterest" Text="&nbsp;Hide liquor interest" CssClass="green" AutoPostBack="true" />&nbsp;&nbsp;
                <asp:Label ID="Label8" runat="server" Text='<%# IIf(IsMobile(), "<br /><br />", "") %>' />
            </asp:Panel>
            <br />

            <%--Payment detail--%>
            <asp:GridView ID="grdPayDtl" runat="server" DataSourceID="dsPayDtl" 
                AutoGenerateColumns="False" CssClass="SMALLTABLE" GridLines="Horizontal" 
                ShowHeaderWhenEmpty="False" AllowPaging="false">
                <Columns>
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th center10" ItemStyle-CssClass="center10" >
                        <HeaderTemplate>
                            <%--<asp:CheckBox ID="chkAll" runat="server" onclick="javascript:checkAll(this);" />--%>
                            <%--<asp:ImageButton ID="btnAll" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl_OnOff("CHECKED", "OFF") %>' CommandName="CHANGE_ALL_ROW" runat="server" />--%><%--TODO: Uncomment this row later to allow check all--%>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnCheck" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl_OnOff("CHECKED", IIf(Eval("checkRow") = "Y", "ON", "OFF")) %>' CommandName="CHANGE_ROW" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" runat="server" />
                            <asp:CheckBox runat="server" ID="chkSelect" Checked='<%# Eval("checkRow") = "Y"  %>' Visible="false" />
                            <asp:Label runat="server" ID="lblArNumber" Visible="false" Text='<%# Eval("arNumber") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblArDate" Text='<%# IIf(IsMobile(), Eval("arDate_mobileview"), Eval("arDate_view"))%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="arType_view" HeaderText="Type" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
                    <asp:BoundField DataField="isLiquor" HeaderText="Liquor" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 green" />
                    <%--<asp:TemplateField HeaderText="Liquor" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16">
                        <ItemTemplate>
                            <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>' Visible='<%# CString(Eval("isLiquor")) = "Y" %>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="Reference" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRefWoLink" CssClass="cellLbl" Text='<%# Eval("reference") %>' Visible='<%# CString(Eval("fileName")) = "" %>' />
                            <asp:LinkButton runat="server" ID="lnkRefWLink" CommandName="DOWNLOAD" CommandArgument='<%# Eval("reference")%>' CssClass="cellLink" ForeColor="#4169E1" 
                                ToolTip='<%# "Download " & CString(Eval("fileName")) %>' Text='<%# Eval("reference") %>' Visible='<%# CString(Eval("fileName")) <> "" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="aging" HeaderText="Aging" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" />
                    <asp:BoundField DataField="arAmt" DataFormatString="{0:$#,#.00}" HeaderText="Invoice Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" />
                    <%--<asp:BoundField DataField="recvAndDiscAmt" DataFormatString="{0:$#,#.00}" HeaderText="Paid Amount (Confirmed)" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" />--%>
                    <asp:TemplateField HeaderText="Paid Amount (Confirmed)" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRecvAndDiscAmt" Text='<%# Eval("recvAndDiscAmt", "{0:$#,#.00}")%>' Visible='<%# Common.Val(Eval("recvAndDiscAmt")) <> 0 %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Paid Amount (Pending)" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPendingAmt" Text='<%# Eval("pendingAmt", "{0:$#,#.00}")%>' Visible='<%# Common.Val(Eval("pendingAmt")) <> 0 %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" >
                        <HeaderTemplate>
                            <asp:Label ID="lblWebDiscHdr" Text="Web Early Payment Discount" runat="server" />
                            <asp:ImageButton ID="btnInfo_webDisc" CssClass="xxsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("question.png") %>' CommandName="INFO_WEBDISC" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblWebDiscAmt_View" Text='<%# Eval("webDiscAmt_view", "{0:$#,#.00}")%>' Visible='<%# Common.Val(Eval("webDiscAmt_view")) <> 0 %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remaining Balance" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20 red" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblBalanceAmt" Text='<%# Eval("balance", "{0:$#,#.00}")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Payment Amount" HeaderStyle-CssClass="th number20" ItemStyle-CssClass="number20" >
                        <ItemTemplate>
                            <asp:TextBox runat="server" ID="txtPayAmt" Text='<%# Eval("balance", "{0:N}")%>' Visible='<%# Eval("checkRow") = "Y" %>' OnKeyUp="javascript:calculateTotalPayAmt()" CssClass="stdTxt number12" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Note" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" >
                        <ItemTemplate>
                            <asp:TextBox runat="server" ID="txtNote" CssClass="stdTxt size32" onblur="javascript:replaceBrackets(this)"  Visible='<%# Eval("checkRow") = "Y" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th" ItemStyle-CssClass="red" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblStar_note" CssClass="" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th" ItemStyle-CssClass="red" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblNoWebDisc" CssClass="" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:SqlDataSource ID="dsPayDtl" runat="server" />
            <br />

            <%--Payment Header and submit payment button--%>
            <asp:Panel ID="pnlPayHdr" CssClass="visiblePnl" runat="server"><%--Height="290px"--%> 
                <table>
                    <tbody>
                        <tr>
                            <asp:Panel ID="Panel1" Visible='<%# Not IsMobile() %>' runat="server">
                                <%--<td class="rowHdr right top size32" rowspan="3">Message to JFC</td>--%>
                                <td class="rowHdr top" rowspan="4">Message to JFC</td>
                                <td class="top" rowspan="4"><asp:TextBox ID="txtMessage" CssClass="stdTxt size48" onblur="javascript:replaceBrackets(this)" TextMode="multiline" rows="6" runat="server" /></td>                        
                                <td class="rowHdr right top size32 bold">Payment Amount</td>
                            </asp:Panel>
                            <asp:Panel ID="Panel2" Visible='<%# IsMobile() %>' runat="server">
                                <td class="rowHdr top">Payment Amount</td>
                            </asp:Panel>
                            <td><asp:Label ID="lblPayAmt" CssClass="rowLblVal bold fontsize6 red" runat="server" />&nbsp;&nbsp;
                                <asp:TextBox ID="txtInputPayAmt" Visible="false" CssClass="stdTxt number12 fontsize6" runat="server" />
                                <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtInputPayAmt" WatermarkText="New amount" WatermarkCssClass="watermark stdTxt center12" />
                                <asp:LinkButton ID="lnkApplyInputPayAmt" Visible="false" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Apply" />
                                <asp:CheckBox ID="chkInputPayAmt" Checked="false" AutoPostBack="true" CssClass="checkbox" runat="server" /><asp:Label ID="lblInputPayAmt" runat="server" Text="Pay different amount"/>&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><asp:CheckBox ID="chkAddAdvancePayment" Checked="false" AutoPostBack="true" CssClass="checkbox" runat="server" /><asp:Label ID="lblAddAdvancePayment" runat="server" Text="Add advance payment"/>&nbsp;
                                <asp:TextBox ID="txtAdvancePayAmt" Visible="false" OnKeyUp="javascript:calculateTotalPayAmt()" CssClass="stdTxt number10" runat="server" />
                                <asp:HiddenField ID="hidAllowCustInputPayAmt" runat="server" /><asp:HiddenField ID="hidIsMobile" runat="server" />
                                <%--<asp:LinkButton ID="lnkApplyAdvancePayAmt" Visible="false" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Apply" />--%>
                                <%--<asp:Label ID="lblAllowCustInputPayAmt" Visible="false" runat="server" />--%>
                            </td>
                            <asp:Panel ID="Panel3" Visible='<%# Not IsMobile() %>' runat="server"><td></td><td></td></asp:Panel>
                        </tr>
                        <tr>
                            <td><br /></td><td></td>
                            <asp:Panel ID="Panel5" Visible='<%# Not IsMobile() %>' runat="server"><td></td><td></td></asp:Panel>
                        </tr>
                        <tr>
                            <asp:Panel ID="Panel8" Visible='<%# Not IsMobile() %>' runat="server">
                                <td class="rowHdr right top size32"><%= IIf(ALLOW_CARD_PAYMENT = True, "Account", "Bank Account")%></td>
                            </asp:Panel>
                            <asp:Panel ID="Panel9" Visible='<%# IsMobile() %>' runat="server">
                                <td class="rowHdr top"><%= IIf(ALLOW_CARD_PAYMENT = True, "Account", "Bank Account")%></td>
                            </asp:Panel>
                            <td><asp:DropDownList ID="lstPaymentAcc" CssClass='<%# "drpList " & IIf(IsMobile(), "size20", "size48") %>' runat="server" /><asp:Label ID="Label10" CssClass="red" runat="server" Text="*" /><br />
                                <asp:LinkButton ID="lnkAddPaymentAcc" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Use another account..." />&nbsp;&nbsp;
                                <asp:Label ID="Label21" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") %>' />
                                <asp:LinkButton ID="lnkEditPaymentAcc" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Edit accounts..." /></td>
                            <asp:Panel ID="Panel6" Visible='<%# Not IsMobile() %>' runat="server"><td></td><td></td></asp:Panel>
                        </tr>
                    </tbody>
                </table>
                <asp:Label ID="Label4" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") %>' />
                <asp:Label ID="lblReturnedCheckFee" CssClass="red" runat="server" /><br />
                <br />
                <asp:Label ID="Label6" CssClass="red" runat="server" Text='<%# IIf(IsMobile(), "", "* Required<br />") %>' />              
                <asp:Button runat="server" ID="btnSubmitPayment" CssClass="midButton" Text="Submit Payment" />
                <asp:LinkButton ID="lnkAutoPay" CssClass="linkLabel bottom" ForeColor="#4169E1" runat="server" Text="Set auto payment..." /><br />
                <asp:Label runat="server" ID="lblResults" Visible="false" />
                <br />
            </asp:Panel>
            <br class="turnOffFloat" />
        </asp:Panel>

        <%--Auto Payment--%>
        <asp:Panel runat="server" ID="pnlAutoPay_balance" >
            <asp:LinkButton ID="lnkAutoPayBack_balance" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to transaction history..." ></asp:LinkButton>
            <br /><br />
            <h2>Auto Payment Settings</h2>
            <asp:Panel runat="server" ID="pnlViewAutoPay">
                <asp:Label ID="lblAutoPayType" runat="server" />
                <br /><br />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlEditAutoPay">
                <p>Please choose one...</p>
                <asp:RadioButtonList ID="radAutoPayType" CssClass="radiobtn" AutoPostBack="true" runat="server" />
                <%--<p>Please note that as refunds cannot be issued through auto payment, credit will remain on your account if the payment amount is negative.</p>--%>
                <p>Please note that when a credit memo is issued, it will not be refunded to your bank immediately. Instead, its negative balance will be included in your next auto payment for your future invoices.</p>
                <asp:Panel runat="server" ID="pnlAutoPayMoreInput">
                    <hr />
                    <p>Please select a <%= IIf(ALLOW_CARD_PAYMENT = True, "account", "bank account")%>...</p>
                    <asp:DropDownList ID="lstAutoPayAcc" CssClass="drpList size48" runat="server" /><br />
                    <asp:LinkButton ID="lnkAddAutoPayAcc" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Use another account..." />&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkEditAutoPayAcc" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="Edit accounts..." />
                    <br /><br />
                    <p class='red'>Terms and Conditions</p>
                    <p class='red'>By requesting this Auto Payment service, you authorize us to automatically transfer the amount of the payment you requested above, from your bank account indicated above, and to JFC. You acknowledge that the origination of automatic payment transactions to JFC must comply with the provisions of U.S. law. This authorization is to remain in full force and in effect until we have received written or online notification from you to cancel this authorization.</p>
                    <asp:CheckBox ID="chkAgreeAutoPay" CssClass="checkbox green" Text="I agree to the Terms and Conditions" runat="server" />
                </asp:Panel>
                <asp:Button ID="btnSaveAutoPay" CssClass="midButton" runat="server" Text="Update Changes" />
            </asp:Panel>
        </asp:Panel>

        <%--Popup for entering new account--%>
        <asp:LinkButton ID="btnPopupTmp"  runat="server" />
        <ajaxToolKit:ModalPopupExtender ID="popDialog" runat="server"
                    BehaviorID="popDialog" 
                    TargetControlID="btnPopupTmp"
                    PopupControlID="pnlPopNewAcc"
                    CancelControlID="btnPopClose"
                    BackgroundCssClass="modalBackground" >
            <Animations>
                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
            </Animations>
        </ajaxToolKit:ModalPopupExtender>
        <%--CancelControlID="btnPopClose"--%><%--Error occurs when placing this inside TODO: Add this code and reopen for error--%>
        <asp:Panel ID="pnlPopNewAcc" style="display:none;" runat="server" CssClass="popup_dialog">
            <div class="inner">
                <asp:LinkButton ID="btnPopClose" CssClass="popClose" runat="server" Text="x" /><%--Use LinkButton instead of ImageButton because progress bar shows after closed in some browser.--%>
                <asp:Label ID="lblPopNewAccCmd" runat="server" Visible="false" />
                <h1>New Account</h1>
                <table>
                    <tr>
                        <td class="bold red" colspan="2"><asp:Label ID="lblPopErrMsg" CssClass="red" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="rowHdr">Payment Type</td>
                        <td><asp:DropDownList ID="lstPopPaymentType" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size32") %>' AutoPostBack='<%# Common.ALLOW_CARD_PAYMENT %>' runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="rowHdr">Name on Account</td>
                        <td><asp:TextBox ID="txtPopAccName" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>' runat="server" />
                            <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtPopAccName" WatermarkText="Name on your bank account" WatermarkCssClass='<%# "watermark stdTxt " & IIf(IsMobile(), "size24", "size48") %>' /></td>
                    </tr>
                    <asp:Panel ID="pnlPopBankAcc" runat="server">
                        <tr>
                            <td class="rowHdr">Routing # (9 digits)</td>
                            <td><asp:TextBox ID="txtPopRoutingNr" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>' runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="rowHdr">Bank Account # (Up to 17 digits)</td>
                            <td><asp:TextBox ID="txtPopBankAccNr" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>' runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="rowHdr">Confirm Bank Account #</td>
                            <td><asp:TextBox ID="txtPopBankAccNr2" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>' runat="server" /></td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlPopCard" runat="server">
                        <tr>
                            <td class="rowHdr">Card Type</td>
                            <td><asp:DropDownList ID="lstPopCardType" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="rowHdr">Card Number</td>
                            <td><asp:TextBox ID="txtPopCardNr" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>' runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="rowHdr">Expiration Date</td>
                            <td><asp:DropDownList ID="lstPopMonth" CssClass="drpList size12" runat="server" /><asp:DropDownList ID="lstPopYear" CssClass="drpList size16" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="rowHdr">Security Code (Back of Card)</td>
                            <td><asp:TextBox ID="txtPopSecurityCode" CssClass="stdTxt size12" runat="server" /></td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td class="rowHdr"></td>
                        <td><asp:CheckBox ID="chkPopRememberAcc" Checked="true" CssClass="checkbox" Text="Remember this Account" runat="server" /></td>
                    </tr>
                    <tr><td colspan="2"><br /></td></tr>
                    <tr>
                        <td colspan="2" class="center"><asp:Image ID="imgPopCheck" ImageAlign="top" Width='280px' Height='60px' runat="server" ImageUrl='<%# GetWebImgUrl("eCheck.jpg") %>' />&nbsp;&nbsp;
                                <asp:Label ID="Label22" runat="server" Text='<%# IIf(IsMobile(), "<br /><br />", "") %>' />
                            <asp:Button runat="server" ID="btnPopAddPaymentAcc" CssClass="midButton" Text="Use this Account" /><%--&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkPopCancelPaymentAcc" CssClass="linkLabel middle" ForeColor="#4169E1" runat="server" Text="Cancel" />--%></td>
                    </tr>
                </table>
                <br />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

<%----------------------------------------------------------------------------------------------------------------------
    Payment History
----------------------------------------------------------------------------------------------------------------------%>
<asp:Panel runat="server" ID="pnlTabPayment">
    <div id="tabPayment">
        <%--List of transaction--%>
        <asp:Panel runat="server" ID="pnlList_payment">
            <h2>Web Payment History with JFC</h2>
            <asp:Panel ID="pnlSearchPayment" CssClass="visiblePnl" DefaultButton="btnSearch_payment" runat="server">
                <asp:Label ID="lblCustCompany_payment" runat="server" Text="Company" CssClass="gray" />
                <asp:DropDownList ID="lstCustCompany_payment" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                <asp:Label ID="lblCustGrp_payment" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType() & " Group" %>' CssClass="gray" />
                <asp:DropDownList ID="lstCustGrp_payment" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                <asp:Label ID="lblCustomer_payment" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType()%>' CssClass="gray" />
                <asp:DropDownList ID="lstCustomer_payment" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                <%--<asp:Label ID="lblDocType" runat="server" Text="Document Type" CssClass="gray" />
                <asp:DropDownList ID="lstDocType" CssClass="drpList" runat="server" AutoPostBack="True" />--%>
                <asp:Panel ID="Panel7" style="display:inline;" Visible='<%# Not IsMobile() %>' runat="server">
                    <asp:Label ID="lblOrderBy_payment" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Sort by" %>' CssClass="gray" />
                    <asp:DropDownList ID="lstOrderBy_payment" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                    <asp:Label ID="Label44" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Payment ID" %>' CssClass="gray" />
                    <asp:TextBox ID="txtPaymentID_payment" CssClass="largeTxt size20" runat="server" />
                    <asp:ImageButton ID="btnSearch_payment" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server" />
                </asp:Panel>
            </asp:Panel>
            <%= IIf(IsMobile() AndAlso Session("BALANCE_CMD") = "CUST", "", "<br />")%>
            <asp:GridView ID="grdPayment" runat="server" DataSourceID="dsPayment" 
                AutoGenerateColumns="False" CssClass="STDTABLE_SFONT" GridLines="Horizontal" 
                ShowHeaderWhenEmpty="False" AllowPaging="True" PageSize="50">
                <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
                <PagerSettings Mode="NumericFirstLast" Position="Bottom" FirstPageText="<<" LastPageText=">>" />
                <Columns>
                    <asp:BoundField DataField="createDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                    <asp:TemplateField HeaderText="Payment ID" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" >
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkPaymentID" CommandName="VIEW_ROW" CommandArgument='<%# Eval("payment_id")%>' CssClass="cellLink" ForeColor="#4169E1" Text='<%# Eval("payment_id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="status" HeaderText="Status" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                    <asp:BoundField DataField="result" HeaderText="Result" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
                    <asp:BoundField DataField="createUser" HeaderText="Paid by" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                    <asp:BoundField DataField="approvedAmt" DataFormatString="{0:$#,#.00}" HeaderText="Amount" HeaderStyle-CssClass="th number24" ItemStyle-CssClass="number24" />
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:SqlDataSource ID="dsPayment" runat="server" />
        </asp:Panel>

        <%--Detail of proceeded payment--%>
        <asp:Panel runat="server" ID="pnlDtl_payment" >
            <asp:LinkButton ID="lnkBack_payment" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to web payment history..." ></asp:LinkButton>
            <br /><br />
            <uc:PaymentDtl id="ctrPaymentDtl_payment" runat="Server" />
        </asp:Panel>
    </div>
</asp:Panel>

<%----------------------------------------------------------------------------------------------------------------------
    Download document
----------------------------------------------------------------------------------------------------------------------%>
<asp:Panel runat="server" ID="pnlTabDownload">
    <div id="tabDownload">
        <h2>Download Document<%= IIf(IsMobile(), "", " (Invoice and Statement)")%></h2>
        <asp:Panel ID="Panel4" CssClass="visiblePnl" runat="server">
            <asp:Label ID="lblCustCompany_download" runat="server" Text="Company" CssClass="gray" />
            <asp:DropDownList ID="lstCustCompany_download" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
            <asp:Label ID="lblCustGrp_download" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType() & " Group" %>' CssClass="gray" />
            <asp:DropDownList ID="lstCustGrp_download" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
            <asp:Label ID="lblCustomer_download" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & GetCustCompanyType()%>' CssClass="gray" />
            <asp:DropDownList ID="lstCustomer_download" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
            <asp:Label ID="lblDocType" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Document Type" %>' CssClass="gray" />
            <asp:DropDownList ID="lstDocType" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
            <asp:Panel ID="Panel10" style="display:inline;" Visible='<%# Not IsMobile() %>' runat="server">
                <asp:Label ID="lblOrderBy_download" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Sort by" %>' CssClass="gray" />
                <asp:DropDownList ID="lstOrderBy_download" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
            </asp:Panel>
        </asp:Panel>
        <br />
        <asp:GridView ID="grdDownload" runat="server" DataSourceID="dsDownload" 
            AutoGenerateColumns="False" CssClass="STDTABLE_SFONT" GridLines="Horizontal" 
            ShowHeaderWhenEmpty="False" AllowPaging="True" PageSize="50">
            <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
            <PagerSettings Mode="NumericFirstLast" Position="Bottom" FirstPageText="<<" LastPageText=">>" />
            <Columns>
                <asp:BoundField DataField="createDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Date" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                <asp:BoundField DataField="docType" HeaderText="Type" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20" />
                <asp:TemplateField HeaderText="Document" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" >
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkDocName" CommandName="DOWNLOAD" CommandArgument='<%# Eval("docName")%>' CssClass="cellLink" ForeColor="#4169E1" 
                            ToolTip='<%# "Download " & CString(Eval("fileName")) %>' Text='<%# Eval("docName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="vendorName" HeaderText="Vendor" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:SqlDataSource ID="dsDownload" runat="server" />
        <br />
    </div>
</asp:Panel>
