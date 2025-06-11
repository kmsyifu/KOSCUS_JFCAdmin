<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderDtl.ascx.vb" Inherits="Common_OrderDtl" %>
<%@ Register Tagprefix="uc" Tagname="ShowItemDtl" Src="~/Common/ShowItemDtl.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="Common" %>

<script type="text/javascript">

    function clickGetItem(txtItem) {
        var enterd = txtItem.val();
        var lines = enterd.split(',');
        if (lines[2] in window) {
        }
        else {
            //txtEmail.val(lines[1]);
            if (lines[2].length > 10) {
                
                $('#<%=btnGetItem.ClientID%>').click();
            }
        }
    }
    // Auto completion for item
    $(document).ready(function () {
        //$(".CODE_ITEM").autocomplete('<%=Request.ApplicationPath() %>/Common/Suggestion.ashx?id=<%=lblCustomerID.text%>&id2=<%=lblCustGrpVendorID.text%>&id3=<%=lblItemGrpID.text%>&code=ITEM', { minChars: 1, autoFill: false, scroll: false, max: 30 });
        $(".CODE_ITEM").autocomplete('../Common/Suggestion.ashx?id=<%=lblCustomerID.text%>&id2=<%=lblCustGrpVendorID.text%>&id3=<%=lblItemGrpID.text%>&id4=<%=lblUserType.text%>&code=ITEM',
         { minChars: 1, autoFill: false, scroll: false, max: 30 }).focus(function () { clickGetItem($('#<%=txtItem.ClientID%>')); });
    });
    // Detect value changed in textbox
    function textValueChanged(cmd) {
        document.getElementById('<%=lblTextChanged.ClientID%>').value = cmd;
    };
</script> 

<%--<asp:LinkButton ID="lnkBack" CssClass="linkLabel" runat="server" Text="<< Go back to orders..." ></asp:LinkButton>--%>
<h1><asp:Label ID="lblTitle" Text="View / Edit Order" runat="server" />&nbsp;<asp:Label ID="lblTitle2" CssClass="red" Text="(Non Web Order)" runat="server" /></h1>

<%--Order info--%>
<asp:Panel ID="pnlOrderHdr" CssClass='<%# "visiblePnl " & IIf(IsMobile(), "", "leftContent") %>' runat="server">
    <h1>Order Info</h1>
    <asp:Label ID="lblCustomerID" runat="server" Visible="false" />
    <asp:Label ID="lblCustGrpVendorID" runat="server" Visible="false" />
    <asp:Label ID="lblUserType" runat="server" Visible="false" /><%--Used for Suggestion.ashx--%>
    <asp:Label runat="server" ID="lblHideEaQty" Visible="false" />
    <table>
        <tbody>
            <tr>
                <td class="gray">Order ID</td>
                <td><asp:Label ID="lblOrderID" CssClass="cellLbl" runat="server" /></td>
            </tr>
            <%--<tr>
                <td>Order Date</td>
                <td><asp:Label ID="lblOrderDate" CssClass="cellLbl bold" runat="server" /></td>
            </tr>--%>
            <tr>
                <td class="gray">Status</td>
                <td><asp:Label ID="lblOrderState" CssClass="cellLbl bold" runat="server" /></td>
            </tr>
            <tr>
                <td class="gray">Ship Date</td>
                <td><asp:TextBox ID="txtDeliveryDate" CssClass="stdTxt size24" runat="server" autocomplete="off"  /><ajaxToolKit:CalendarExtender ID="calDeliveryDate" Format="MM/dd/yyyy" TargetControlID="txtDeliveryDate" runat="server" />
                    <asp:Label ID="lblDeliveryDate" CssClass="cellLbl bold" runat="server" />&nbsp; <asp:Label ID="lblShipOffset" CssClass="cellLbl" runat="server" /></td>
            </tr>
            <asp:Panel ID="pnlVendorName" runat="server">
                <tr>
                    <td class="gray">Vendor</td>
                    <td><asp:Label ID="lblVendorName" CssClass="cellLbl bold" runat="server" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray"><asp:Label ID="lblTitleItemGrp" runat="server" Text="Order Form"/></td>
                <td><asp:Label ID="lblItemGrpName" CssClass="cellLbl" runat="server" /><asp:Label ID="lblItemGrpID" visible="false" runat="server" /></td>
            </tr>
            <asp:Panel ID="pnlShowPrice" runat="server">
                <tr>
                    <td class="gray">Total Amount<%--&nbsp;<asp:ImageButton ID="btnInfo_totalAmt" CssClass="xsImgBtn red" runat="server" ImageUrl='<%# GetWebImgUrl("question.png") %>' ToolTip="This is not an invoice amount. Additional charges such as sales tax, crv, etc may apply separately." />--%></td>
                    <td><asp:Label ID="lblTotalAmt" CssClass="cellLbl bold red" runat="server" /><%--(<asp:Label ID="lblNrItem" CssClass="cellLbl bold" runat="server" />)--%></td>
                </tr>
                <%--<tr>
                    <td></td>
                    <td><label class="red">This is not an invoice amount. Additional charges such as sales tax,<br />crv, etc may apply separately.</label></td>
                </tr>--%>
            </asp:Panel>
            <tr>
                <td class="gray">Message</td>
                <td><asp:TextBox ID="txtMessage" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size32", "size48") %>' runat="server"  autocomplete="off" /><asp:Label ID="lblMessage" CssClass="cellLbl bold red" runat="server" /></td>
            </tr>
            <asp:Panel ID="Panel2" runat="server" Visible='<%# NOT IsMobile() %>'>
                <tr>
                    <td class="gray">Customer PO #</td>
                    <td><asp:TextBox ID="txtCustOrderNr" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size32", "size48") %>' runat="server" autocomplete="off"  /><asp:Label ID="lblCustOrderNr" CssClass="cellLbl bold" runat="server" /></td>
                </tr>
                
            </asp:Panel>
            <asp:Panel ID = "pnlreference" runat ="server">
                <tr>
                    <td class="gray">Reference</td>
                    <td><asp:Label ID="lblreference" CssClass='<%# "cellLbl " & IIf(IsMobile(), "size32", "size48") %>' runat="server" autocomplete="off"  /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" Visible='<%# NOT (IsMobile() AndAlso Session("ORDERDTL_CMD") = "CUST") %>'>
                <tr>
                    <td class="gray">Name</td>
                    <td><asp:Label ID="lblCustomerName" CssClass="cellLbl bold" runat="server" /><asp:Panel ID="pnlCustGrpName" style="display:inline;" runat="server"><%= IIf(IsMobile(), "<br />", "")%>(<asp:Label ID="lblCustGrpName" CssClass="cellLbl" runat="server" />)</asp:Panel></td>
                </tr>
                <tr>
                    <td class="gray">Account #</td><%--<%# GetCustCompanyType() & " #"%>--%>
                    <td><asp:Label ID="lblJfcBranchNr" CssClass="cellLbl" runat="server" />-<asp:Label ID="lblJfcCustomerNr" CssClass="cellLbl" runat="server" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="Panel4" runat="server" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" %>'>
                <tr>
                    <td class="gray"></td>
                    <td><asp:HyperLink ID="lnkCurrentBalance" target="_blank" CssClass="linkLabel" runat="server" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlCustShipPoint" runat="server">
                <tr>
                    <td class="gray">Ship to</td>
                    <td><asp:Label ID="lblShipName" CssClass="cellLbl bold" runat="server" /><asp:Label ID="lblShipPointID" Visible="false" CssClass="cellLbl bold" runat="server" /></td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="gray right">Address:</td>
                    <td class="gray"><asp:Label ID="lblAddress1" CssClass="cellLbl" runat="server" /><asp:Label ID="lblAddress2" runat="server" CssClass="cellLbl" /></td>
                </tr>
                <tr>
                    <td class="gray right">City:</td>
                    <td class="gray"><asp:Label ID="lblCity" CssClass="cellLbl" runat="server" /></td>
                </tr>
                <tr>
                    <td class="gray right">State Zip:</td>
                    <td class="gray"><asp:Label ID="lblState" CssClass="cellLbl" runat="server"/><asp:Label ID="lblZip" runat="server" CssClass="cellLbl" /></td>
                </tr>
                <%--<tr>
                    <td class="gray right">Country:</td>
                    <td class="gray"><asp:Label ID="lblCountry" CssClass="cellLbl" runat="server" /></td>
                </tr>--%>
            </asp:Panel>
            <asp:Panel ID="pnlSFA" runat="server" >
            <tr>
                <td class="gray">SFA </td>
                <td><asp:Label ID="lblSFA" CssClass="cellLbl" runat="server" /></td>
            </tr>
             </asp:Panel>
            <tr>
                <td class="gray">Ordered on</td>
                <td><asp:Label ID="lblCreateDateTimeUser" CssClass="cellLbl" runat="server" /></td>
            </tr>
             <asp:Panel ID="pnlApproveOn" runat="server" >
            <tr>
                <td class="gray">Approved on</td>
                <td><asp:Label ID="lblApproveDateTimeUser" CssClass="cellLbl" runat="server" /></td>
            </tr>
            </asp:Panel>
            <asp:Panel ID="Panel1" runat="server" Visible='<%# NOT IsMobile() %>'>
                <tr>
                    <td class="gray">Last Updated</td>
                    <td><asp:Label ID="lblModDateUser" CssClass="cellLbl" runat="server" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="Panel7" runat="server" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" %>'>
                <tr>
                    <td class="gray">Msg on JETS</td>
                    <td><asp:TextBox ID="txtVendorNote" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size32", "size64") %>' runat="server" autocomplete="off"  /><asp:Label ID="lblVendorNote" CssClass="cellLbl" runat="server" /></td>
                </tr>
            </asp:Panel>
            <tr><td colspan="2"></td></tr>
        </tbody>
    </table>
</asp:Panel>
<%= IIf(IsMobile(), "<br class='turnOffFloat' />", "")%>

<%--Add item--%>
<asp:Panel ID="pnlItem" CssClass="visiblePnl" style='<%# IIf(IsMobile(), "", "margin-left:533px") %>' runat="server">
    <h1>Add Item to Order</h1>
        <asp:Panel ID="pnlGetItem" DefaultButton="btnGetItem" runat="server">
            <asp:TextBox ID="txtItem" CssClass='<%# "stdTxt CODE_ITEM " & IIf(IsMobile(), "size32", "size72") %>' runat="server"  />
            <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtItem" WatermarkText="Search by item # or keyword" WatermarkCssClass='<%# "watermark stdTxt " & IIf(IsMobile(), "size32", "size72") %>' />
            <asp:ImageButton ID="btnClearItem" Tooltip="Clear" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' runat="server" /><%--clearText.png--%>
            <asp:Button ID="btnGetItem" CssClass="midButton" runat="server" Text="Get Item" style="display: none;"/>
            <br />
        </asp:Panel>
        <asp:Panel ID="pnlAddItem" DefaultButton="btnAddItem" runat="server">
            <table>
            <asp:Panel ID="Panel5" Visible='<%# Not IsMobile() %>' runat="server">
                <tr>
                    <td rowspan="19"><asp:Image ID="imgNewItem" CssClass="largeGrdImg" runat="server" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="Panel6" Visible='<%# IsMobile() %>' runat="server">
                <tr>
                    <td colspan="2" class="center"><asp:Image ID="imgNewItem_mobile" CssClass="largeGrdImg" runat="server" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td colspan="2"><asp:Label runat="server" ID="lblNewItemDesc" CssClass="cellLbl bold" />
                    <asp:Label Visible="false" runat="server" ID="lblNewCustGrpItemID" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Label runat="server" ID="lblNewJfcRemark" CssClass="cellLbl green" /></td>
            </tr>
            <tr>
                <td class="gray">Item:</td>
                <td><asp:Label runat="server" ID="lblNewItemIdNr" CssClass="cellLbl" /></td>
               
            </tr>
            <asp:Panel ID="pnlNewOrderForm" runat="server">
                <tr>
                    <td class="gray">Order Form:</td>
                    <td><asp:Label runat="server" ID="lblNewItemGrpName" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlNewVendor" runat="server">
                <tr>
                    <td class="gray">Vendor:</td>
                    <td><asp:Label runat="server" ID="lblNewVendorName" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray">JFC Category:</td>
                <td><asp:Label runat="server" ID="lblNewCtgName" CssClass="cellLbl" /></td>
            </tr>
            <asp:Panel ID="pnlNewEaPerCs" runat="server">
                <tr>
                    <td class="gray">Each / Case:</td>
                    <td><asp:Label runat="server" ID="lblNewEaPerCs" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray"><asp:Label runat="server" ID="lblNewAvgWgtTitle" Text="Average Weight:" /></td>
                <td><asp:Label runat="server" ID="lblNewAvgCsWgt" CssClass="cellLbl" />
                    <asp:Label runat="server" ID="lblNewAvgEaWgt" CssClass="cellLbl" /></td>
            </tr>
            <asp:Panel ID="pnlNewShowItemDtl" runat="server">
                <tr>
                    <td class="gray">Weight:</td>
                    <td><asp:Label runat="server" ID="lblNewGrossWeight" CssClass="cellLbl" /></td>
                </tr>
                <tr>
                    <td class="gray">Volume:</td>
                    <td><asp:Label runat="server" ID="lblNewCaseVol" CssClass="cellLbl" /></td>
                </tr>
                <%--<tr>
                    <td class="gray">Pallet Tie / Height:</td>
                    <td><asp:Label runat="server" ID="lblNewPallet" CssClass="cellLbl" /></td>
                </tr>--%>
            </asp:Panel>
            <%--<asp:Panel ID="Panel5" Visible='<%# Session("ORDERDTL_CMD") <> "JFCADMIN" %>' runat="server">--%>
            <asp:Panel ID="pnlNewShowPrice" runat="server">
                <tr>
                    <td class="gray">Price:</td>
                    <td><asp:Label runat="server" ID="lblNewLbPrice_OrderByQty_view" CssClass="cellLbl green bold" /><asp:Label runat="server" ID="lblNewLbPrice_OrderByQty" Visible="false" />
                        <asp:Label runat="server" ID="lblNewCsPrice_view" CssClass="cellLbl red bold" /><asp:Label runat="server" ID="lblNewCsPrice" Visible="false" />
                        <asp:Label runat="server" ID="lblNewEaPrice_view" CssClass="cellLbl red bold" /><asp:Label runat="server" ID="lblNewEaPrice" Visible="false" /> 
                        <asp:Label runat="server" ID="lblNewLbPrice_OrderByWgt_view" CssClass="cellLbl red bold" /><asp:Label runat="server" ID="lblNewLbPrice_OrderByWgt" Visible="false" />
                        <asp:Label runat="server" ID="lblNewMinSalCnt_view" CssClass="cellLbl green" /><asp:Label runat="server" ID="lblNewMinSalCnt" Visible="false" />
                        <asp:Label runat="server" ID="lblNewAskPrice" Text='Ask' CssClass="cellLbl red bold" />
                        <asp:Label runat="server" ID="lblNewCsCost" Visible="false" /><asp:Label runat="server" ID="lblNewEaCost" Visible="false" /><asp:Label runat="server" ID="lblNewLbCost" Visible="false" />
                        <asp:Label runat="server" ID="lblNewBtmCs" Visible="false" /><asp:Label runat="server" ID="lblNewBtmEa" Visible="false" /><asp:Label runat="server" ID="lblNewBtmLb" Visible="false" />
                        <asp:Label runat="server" ID="lblNewGp" CssClass="cellLbl gray" />
                        <asp:LinkButton runat="server" ID="lnkSetPrice" CssClass="cellLink" Text="Set Price" />
                    </td>
                </tr>
            </asp:Panel>
           <%-- </asp:Panel>--%>
            <%--<asp:Panel ID="Panel6" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" %>' runat="server">
                <tr>
                    <td class="gray">Price:</td>
                    <td><asp:TextBox runat="server" ID="txtNewCsPrice" onFocus="this.select()" CssClass="stdTxt number8 bold" />
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtNewCsPrice" />
                        <asp:Label runat="server" ID="Label6" Text='/ cs' CssClass="cellLbl bold" />&nbsp;&nbsp;
                        <asp:Label runat="server" ID="lblNewEaPrice_edit" CssClass="cellLbl bold" /></td>
                </tr>
            </asp:Panel>--%>
            <tr>
                <td class="gray"><asp:Label runat="server" ID="lblNewQtyHistoryTitle" Text="Last Ordered Qty:" /></td>
                <td><asp:Label runat="server" ID="lblNewQtyHistory" CssClass="cellLbl gray" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Label ID="lblNewAvailQty" CssClass="cellLbl green" runat="server" />
                    <asp:Label ID="lblNewOutOfStock" CssClass="cellLbl green bold" Text='Out of Stock' runat="server" />
                    <asp:Label ID="lblNewArrivDate" CssClass="cellLbl green" runat="server" />
                </td>
            </tr>
            <tr><td colspan="2"></td></tr><tr><td colspan="2"></td></tr>
            <tr>
                <td class="gray" colspan="2">                  
                    <asp:TextBox runat="server" ID="txtNewCsQty" onFocus="this.select()" CssClass="cellQtyTxt" autocomplete="off"  /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty" />
                    <asp:Label runat="server" ID="lblNewCsQty" Text='cs' CssClass="cellLbl" />&nbsp;&nbsp;
                    <asp:TextBox runat="server" ID="txtNewEaQty" onFocus="this.select()" CssClass="cellQtyTxt" autocomplete="off"  /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Numbers" TargetControlID="txtNewEaQty" />
                    <asp:Label runat="server" ID="lblNewEaQty" Text='ea' CssClass="cellLbl" />&nbsp;&nbsp;
                    <asp:TextBox runat="server" ID="txtNewLbs" onFocus="this.select()" CssClass="cellQtyTxt" autocomplete="off"  /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtNewLbs" />
                    <asp:Label runat="server" ID="lblNewLbs" Text='lbs' CssClass="cellLbl" />&nbsp;&nbsp;
                    <asp:LinkButton runat="server" CssClass="cellLink" ID="lnkNewShowVpSp" Text="vp/sp" Visible ="false" />&nbsp;&nbsp;
                    <asp:Panel ID="pnlNewVpSp" runat="server" style="display:inline;"> 
                        <br />
                        <asp:TextBox runat="server" ID="txtNewCsQty_vp" onFocus="this.select()" CssClass="cellQtyTxt green" autocomplete="off"  /><%--TextMode="Number"--%>
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty_vp" />
                        <asp:Label runat="server" ID="lblNewCsQty_vp" Text='vp' CssClass="cellLbl green" />&nbsp;&nbsp;
                        <asp:TextBox runat="server" ID="txtNewEaQty_vp" onFocus="this.select()" CssClass="cellQtyTxt green" /><%--TextMode="Number"--%>
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" FilterType="Numbers" TargetControlID="txtNewEaQty_vp" />
                        <asp:Label runat="server" ID="lblNewEaQty_vp" Text='vp' CssClass="cellLbl green" />&nbsp;&nbsp;
                        <asp:TextBox runat="server" ID="txtNewVpMemo" onFocus="this.select()" CssClass="stdTxt size24 green" autocomplete="off"  />
                        <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtNewVpMemo" WatermarkText="VP memo" WatermarkCssClass="watermark stdTxt size24" />&nbsp;&nbsp;
                        <br />
                        <asp:TextBox runat="server" ID="txtNewCsQty_sp" onFocus="this.select()" CssClass="cellQtyTxt green" autocomplete="off"  /><%--TextMode="Number"--%>
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty_sp" />
                        <asp:Label runat="server" ID="lblNewCsQty_sp" Text='sp' CssClass="cellLbl green" />&nbsp;&nbsp;
                        <asp:TextBox runat="server" ID="txtNewEaQty_sp" onFocus="this.select()" CssClass="cellQtyTxt green"  autocomplete="off"  /><%--TextMode="Number"--%>
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender12" runat="server" FilterType="Numbers" TargetControlID="txtNewEaQty_sp" />
                        <asp:Label runat="server" ID="lblNewEaQty_sp" Text='sp' CssClass="cellLbl green" />&nbsp;&nbsp;
                    </asp:Panel>
                    <%--<asp:ImageButton ID="btnQtyHistory" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("pencil.png") %>' runat="server" />&nbsp;&nbsp;&nbsp;--%>
                    <asp:Button ID="btnAddItem" CssClass="midButton" runat="server" Text="Add to Order"/>
                </td>
            </tr>
            <tr>
                <td class="red" colspan="2"><asp:Label runat="server" ID="lblNewMsg" CssClass="cellLbl" /></td>                  
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<%--<br class='turnOffFloat' />--%>

<asp:Panel ID="pnlOrderDtl_Edit" DefaultButton="btnUpdate" runat="server">
    <%= IIf(IsMobile(), "", "<br class='turnOffFloat' /><br />")%>
    <%--Order detail--%>
    <asp:HiddenField ID="lblTextChanged" runat="server" /> <%--Can be access from both server and client side--%>
    <asp:GridView ID="grdItem_edit" runat="server" DataSourceID="dsItem_edit" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="False" AllowSorting="false" AllowPaging="false">
        <%--<HeaderStyle CssClass="th tdLink left" />--%> <%--Doesn't work. Put HeaderStyle-CssClass="th left" in each column instead--%>
        <Columns>
            <asp:TemplateField HeaderText="Image" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" >
                <ItemTemplate>
                    <asp:ImageButton ID="imgItem" Tooltip="View item info" runat="server" CommandName="DETAIL" CommandArgument='<%# Eval("custGrpItem_id")%>' CssClass="cellImg" ImageUrl='<%# GetItemImgUrl(CString(Eval("itemType")), CString(Eval("item_id"))) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item #" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkItemIdNrView" Text='<%# Eval("itemIdNr_view") %>' CommandName="DETAIL" CommandArgument='<%# Eval("custGrpItem_id")%>' />
                   <asp:Label runat="server" ID="lblPromoitem" CssClass="cellLbl" Text='<%# Eval("promoItemId") %>' Visible ="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="th size72" ItemStyle-CssClass="size72" >
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkItemDesc" Tooltip="View item info" Text='<%# Eval("itemDesc_view") %>' CommandName="DETAIL" CommandArgument='<%# Eval("custGrpItem_id")%>' />
                    <asp:Label runat="server" ID="lblJfcRemark" CssClass="cellLbl green" Text='<%# "<br />" & CString(Eval("jfcRemark")) %>' Visible='<%# CString(Eval("jfcRemark")) <> "" %>' />
                    <asp:Label runat="server" ID="lblItemDtl" CssClass="cellLbl green" Text='<%# "<br />" & CString(Eval("itemDtl")) %>' Visible='<%# CString(Eval("itemDtl")) <> "" %>' />
                    <asp:TextBox autocomplete="off"  runat="server" ID="txtVpMemo" onFocus="this.select()" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso (Val(Eval("csQty_vpsp")) > 0 OrElse Val(Eval("eaQty_vpsp")) > 0) %>' Text='<%# Eval("vpMemo")%>' CssClass="cellTxt stdTxt green" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtVpMemo" WatermarkText="VP memo" WatermarkCssClass="watermark cellTxt stdTxt" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl" />--%>
            <asp:TemplateField HeaderText="Ea/Cs" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEaPerCs" CssClass="cellLbl" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# CString(Eval("eaPerCs")) & " ea/cs<br />" %>' />
                    <asp:Label runat="server" ID="lblAvgCsWgt" Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0 AndAlso Val(Eval("csprice")) <> 0 AndAlso Val(Eval("avgcswgt")) <> 0 %>' Text='<%# Eval("avgcswgt", "{0:N}") & " lbs/cs<br />" %>' CssClass="green" />
                    <asp:Label runat="server" ID="lblAvgEaWgt" Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0 AndAlso Val(Eval("eaprice")) <> 0 AndAlso Val(Eval("avgeawgt")) <> 0 %>' Text='<%# Eval("avgeawgt", "{0:N}") & " lbs/pc<br />" %>' CssClass="green" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 cellLbl" >
                <ItemTemplate>
                    <%--<asp:Label runat="server" Text='(' Visible='<%# IsMobile() = False and CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csprice")) <> "0"%>' />--%>
                    <asp:Label runat="server" ID="lblCsPrice" Text='<%# Eval("csprice", "{0:C}") %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso CString(Eval("csprice")) <> "0"%>' />
                    <%--<asp:Label runat="server" Text=')' Visible='<%# IsMobile() = False and CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("csprice")) <> "0"%>' />--%><asp:Label ID="Label1" runat="server" Text='<br />' Visible='<%# CString(Eval("csprice")) <> "0"%>' />
                    <%--<asp:Label runat="server" Text='(' Visible='<%# IsMobile() = False and CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />--%>
                    <asp:Label runat="server" ID="lblEaPrice" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso lblHideEaQty.Text = "N" AndAlso CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <%--<asp:Label runat="server" Text=')' Visible='<%# IsMobile() = False and CString(Eval("lbPrice_OrderByQty")) <> "0" and CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />--%>
                    <asp:Label ID="Label2" runat="server" Text='<br />' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso lblHideEaQty.Text = "N" AndAlso  CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" Text='<%# CString(Eval("lbPrice_OrderByWgt", "{0:C}")) & "<br />" %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso Common.ALLOW_LB_ORDER = True AndAlso CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByQty" Text='<%# CString(Eval("lbPrice_OrderByQty", "{0:C}")) & "<br />" %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso CString(Eval("lbPrice_OrderByQty")) <> "0"%>' CssClass="green" />
                    <asp:Label runat="server" ID="lblMinSalCnt_view" CssClass="green normal" Visible='<%# lblHideEaQty.Text = "N" AndAlso Val(Eval("minSalCnt")) > 1 AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# IIf(IsMobile(), "min:" & CString(Eval("minSalCnt")) & "ea", "min " & CString(Eval("minSalCnt")) & " ea") & "<br />" %>' /><%-- and CString(Eval("eaprice")) <> "0"--%>
                    <asp:Label runat="server" ID="lblMinSalCnt" Text='<%# Eval("minSalCnt") %>' Visible="false" />
                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkItemSetPrice" Text="Edit"  CommandName="SETPRICE" CommandArgument='<%# Eval("custGrpItem_id")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size10" ItemStyle-CssClass="size10 cellLbl" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text='/cs<br />' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso CString(Eval("csprice")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblEa" Text='<%# IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "/pc<br />", "/ea<br />") %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso lblHideEaQty.Text = "N" AndAlso CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <asp:Label runat="server" ID="lblLb_orderByWgt" Text='/lb<br />' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso Common.ALLOW_LB_ORDER = True AndAlso CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' />
                    <asp:Label runat="server" ID="lblLb_orderByQty" Text='/lb<br />' Visible='<%# CString(Eval("hidePrice")) <> "Y" AndAlso CString(Eval("lbPrice_OrderByQty")) <> "0"%>' CssClass="green" />
                    <asp:Label ID="Label8" runat="server" Text='<br />' Visible='<%# Val(Eval("minSalCnt")) > 1 and CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <asp:Label ID="Label4" runat="server" Text='<br />' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Case" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:TextBox autocomplete="off"  runat="server" ID="txtCsQty" Visible='<%# CString(Eval("csprice")) <> "0"%>' onFocus="this.select()" Text='<%# Eval("csQty")%>' CssClass="cellQtyTxt" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers" TargetControlID="txtCsQty" />
                    <asp:Label runat="server" ID="Label15" CssClass="cellLbl gray" onFocus="this.select()" Visible='<%# CString(Eval("csprice")) <> "0"%>' Text='<%# IIf(IsMobile(), "<br/>", "") & "cs&nbsp;&nbsp;<br />" %>' />
                    <asp:LinkButton runat="server" CssClass="cellLink" ID="lnkShowCsQtyVpSp" Visible='false' Text='<%# "vp/sp" & IIf(IsMobile(), "", "&emsp;&emsp;") %>' 
                        CommandName="SHOW_VPSP" CommandArgument='<%# CType(Container, GridViewRow).RowIndex %>'  /> <%--<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso CString(Eval("csprice")) <> "0" AndAlso Val(Eval("csQty_vpsp")) = 0 %> --%>
                    <asp:TextBox autocomplete="off"  runat="server" ID="txtCsQty_vp" onFocus="this.select()" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("csQty_vpsp")) > 0 %>' Text='<%# Eval("csQty_vp")%>' CssClass="cellQtyTxt green" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" FilterType="Numbers" TargetControlID="txtCsQty_vp" />
                    <asp:Label runat="server" ID="lblCsQty_vp" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("csQty_vpsp")) > 0 %>' Text='<%# IIf(IsMobile(), "<br/>", "") & "vp&nbsp;&nbsp;<br />" %>' />
                    <asp:TextBox autocomplete="off"  runat="server" ID="txtCsQty_sp" onFocus="this.select()" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("csQty_vpsp")) > 0 %>' Text='<%# Eval("csQty_sp")%>' CssClass="cellQtyTxt green" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" FilterType="Numbers" TargetControlID="txtCsQty_sp" />
                    <asp:Label runat="server" ID="lblCsQty_sp" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("csQty_vpsp")) > 0 %>' Text='<%# IIf(IsMobile(), "<br/>", "") & "sp&nbsp;&nbsp;<br />" %>' />
                    <asp:Label runat="server" ID="Label7" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") <> "JFCADMIN" AndAlso Val(Eval("csQty_vpsp")) > 0 %>' Text='<%# CString(Eval("csQty_vpsp")) & " cs_free " & IIf(IsMobile(), "", "&nbsp;&nbsp;cs&nbsp;&nbsp;") & "<br />" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Each" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:TextBox runat="server" ID="txtEaQty" Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' onFocus="this.select()" Text='<%# Eval("eaQty")%>' CssClass="cellQtyTxt" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Numbers" TargetControlID="txtEaQty" />
                    <asp:Label runat="server" ID="Label16" CssClass="cellLbl gray" Visible='<%# CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# IIf(IsMobile(), "<br/>", "") & IIf(CString(Eval("lbPrice_OrderByQty")) <> "0", "pc", "ea") & "&nbsp;&nbsp;<br />" %>' />
                    <asp:LinkButton runat="server" CssClass="cellLink" ID="lnkShowEaQtyVpSp"  Text='<%# "vp/sp" & IIf(IsMobile(), "", "&emsp;&emsp;") %>' 
                        CommandName="SHOW_VPSP" CommandArgument='<%# CType(Container, GridViewRow).RowIndex %>' Visible="false"  /> <%-- Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso CString(Eval("eaprice")) <> "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) AndAlso Val(Eval("eaQty_vpsp")) = 0 %>' --%>
                    <asp:TextBox runat="server" ID="txtEaQty_vp" onFocus="this.select()" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# Eval("eaQty_vp")%>' CssClass="cellQtyTxt green" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" FilterType="Numbers" TargetControlID="txtEaQty_vp" />
                    <asp:Label runat="server" ID="lblEaQty_vp" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# IIf(IsMobile(), "<br/>", "") & "vp&nbsp;&nbsp;<br />" %>' />
                    <asp:TextBox runat="server" ID="txtEaQty_sp" onFocus="this.select()" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# Eval("eaQty_sp")%>' CssClass="cellQtyTxt green" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" runat="server" FilterType="Numbers" TargetControlID="txtEaQty_sp" />
                    <asp:Label runat="server" ID="lblEaQty_sp" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# IIf(IsMobile(), "<br/>", "") & "sp&nbsp;&nbsp;<br />" %>' />
                    <asp:Label runat="server" ID="Label10" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") <> "JFCADMIN" AndAlso Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# CString(Eval("eaQty_vpsp")) & " ea_free " & IIf(IsMobile(), "", "&nbsp;&nbsp;ea&nbsp;&nbsp;") & "<br />" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Lb" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:TextBox autocomplete="off"  runat="server" ID="txtLbs" Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0" %>' onFocus="this.select()" Text='<%# Eval("lbs")%>' CssClass="cellQtyTxt" /><%--TextMode="Number"--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtLbs" />
                    <asp:Label runat="server" ID="Label17" CssClass="cellLbl gray" Visible='<%# CString(Eval("lbPrice_OrderByWgt")) <> "0"%>' Text='<%# IIf(IsMobile(), "<br/>", "") & "lbs" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:TemplateField HeaderStyle-CssClass="th center6" ItemStyle-CssClass="center6" >
                <ItemTemplate>
                    <asp:ImageButton ID="btnQtyHistory" ToolTip="Click to view previously ordered qty" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("pencil.png") %>' CommandName="QTY_HISTORY" CommandArgument='<%# Eval("custGrpItem_id")%>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblLineAmt" CssClass="cellLbl red bold" Text='<%# Eval("lineAmt", "{0:C}") %>' Visible='<%# CString(Eval("hidePrice")) <> "Y" %>' />
                    <asp:Label runat="server" ID="lblLineCost" Visible="false" Text='<%# Eval("lineCost") %>' />
                    <asp:LinkButton runat="server" CssClass="cellLink" ID="btnShowCost" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" AndAlso IsMobile() AndAlso Val(Eval("gp")) <> 1 %>' Text='<%# "<br />" & Format(Eval("gp"), "P0") %>' CommandName="SHOW_COST" CommandArgument='<%# CType(Container, GridViewRow).RowIndex %>' />
                    <asp:Label runat="server" ID="lblLineGrossWeight_kg_view" CssClass="cellLbl green" Text='<%# "<br />" & CString(Eval("lineGrossWeight_kg")) & " kg" %>' Visible='<%# CString(Eval("itemDtl")) <> "" %>' />
                    <asp:Label runat="server" ID="lblLineCaseVol_m3_view" CssClass="cellLbl green" Text='<%# "<br />" & CString(Eval("lineCaseVol_m3")) & " m³" %>' Visible='<%# CString(Eval("itemDtl")) <> "" %>' />
                    <asp:Label runat="server" ID="lblLineGrossWeight_kg" Text='<%# Eval("lineGrossWeight_kg") %>' Visible="false" />
                    <asp:Label runat="server" ID="lblLineCaseVol_m3" Text='<%# Eval("lineCaseVol_m3") %>' Visible="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Avail" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail_view")) & "<br />" %>' />
                    <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail_view")) & "<br />" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs3" Text="cs" /><br />
                    <asp:Label runat="server" ID="lblEa3" Text="ea" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Out Of Stock" >
                <ItemTemplate>
                    <%--<asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text="Yes" Visible='<%# Val(Eval("remainingInStockQty")) < 0 %>' />--%>
                    <asp:Label runat="server" ID="lblOutOfStock" CssClass="cellLbl" Text='<%# IIf(Val(Eval("remainingInStockQty")) <= 0, "Y", "") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cost<br />GP" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 cellLbl" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCsCost" Text='<%# CString(Eval("cscost", "{0:C}")) & "<br />" %>' Visible='<%# Val(Eval("cscost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) = 0 %>' />
                    <asp:Label runat="server" ID="lblEaCost" Text='<%# CString(Eval("eacost", "{0:C}")) & "<br />" %>' Visible='<%# Val(Eval("eacost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) = 0 AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <asp:Label runat="server" ID="lblLbCost" Text='<%# CString(Eval("lbCost", "{0:C}")) & "<br />" %>' Visible='<%# Val(Eval("lbcost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) <> 0 %>' />
                    <asp:Label runat="server" ID="lblGp" Text='<%# Format(Eval("gp"), "P1") %>' Visible='<%# Val(Eval("gp")) <> 1 %>' CssClass="gray" />
                    <asp:Label runat= "server" ID="lblbtmCs"  Text='<%# CString(Eval("btmcsprice", "{0:C}"))  %>' Visible ="false" />
                    <asp:Label runat= "server" ID="lblbtmEa"  Text='<%# CString(Eval("btmeaprice", "{0:C}"))  %>' Visible ="false" />
                    <asp:Label runat= "server" ID="lblbtmlb"  Text='<%# CString(Eval("btmlbprice", "{0:C}"))  %>' Visible ="false" />
                    <asp:Label runat= "server" ID="lblIsNew"  Text='<%# CString(Eval("IsNew"))  %>' Visible ="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size10" ItemStyle-CssClass="size10 cellLbl" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs2" Text='/cs<br />' Visible='<%# Val(Eval("cscost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) = 0 %>' />
                    <asp:Label runat="server" ID="lblEa2" Text='<%# IIf(Val(Eval("lbPrice_OrderByQty")) <> 0, "/pc", "/ea") & "<br />"%>' Visible='<%# Val(Eval("eacost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) = 0 AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' />
                    <asp:Label runat="server" ID="Label6" Text='/lb<br />' Visible='<%# Val(Eval("lbcost")) <> 0 AndAlso Val(Eval("lbPrice_orderByQty")) <> 0 %>' />
                    <asp:Label runat="server" ID="Label3" Text='<br />' Visible='<%# Val(Eval("gp")) <> 1 %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-CssClass="th center8" ItemStyle-CssClass="center8" >
                <ItemTemplate>
                    <asp:Label Visible="false" ID="lblCustGrpItemID" Text='<%# Eval("custGrpItem_id")%>' runat="server" />
                    <asp:ImageButton ID="btnDelete" Tooltip="Remove item from order" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" runat="server" /><%--trash.png--%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsItem_edit" runat="server"></asp:SqlDataSource>
    <asp:Label ID="lblOrderMsg" CssClass="midLabel" runat="server" />&nbsp;<asp:ImageButton ID="btnInfo_orderMsg" CssClass="xsImgBtn red" runat="server" ImageUrl='<%# GetWebImgUrl("question.png") %>' ToolTip="This is not an invoice amount. Additional charges such as sales tax, crv, etc may apply separately." />
    <br /><br />
    <table>
    <tr>
    <td> <asp:Label ID="lblcolor1" CssClass="cellLbl " BackColor="#FFE4E1" Text="&nbsp;&nbsp;&nbsp;&nbsp;" runat="server" />
    </td>
    <td> <asp:Label ID="lbldescription1" CssClass="cellLbl" runat="server" Text="Out of stock" />
    </td>
    </tr>
    <tr>
    <td>
    <asp:Label ID="lblcolor2" CssClass="cellLbl" BackColor="#FFFF99" Text="&nbsp;&nbsp;&nbsp;&nbsp;" runat="server" />
    </td>
    <td>
    <asp:Label ID="lbldescription2" CssClass="cellLbl " runat="server" Text="Item price is less than bottom price" />
    </td>
    </tr>
    <tr>
    <td> <asp:Label ID="lblcolor3" CssClass="cellLbl " BackColor="#C3E8F6" Text="&nbsp;&nbsp;&nbsp;&nbsp;" runat="server" />
    </td>
    <td> <asp:Label ID="lbldescription3" CssClass="cellLbl" runat="server" Text="New Item" />
    </td>
    </tr>
    <tr>
    <td> <asp:Label ID="lblcolor4" CssClass="cellLbl " BackColor="#90EE90" Text="&nbsp;&nbsp;&nbsp;&nbsp;" runat="server" />
    </td>
    <td> <asp:Label ID="lbldescription4" CssClass="cellLbl" runat="server" Text="Promo Item" />
    </td>
    </tr>
    </table>
    <asp:TextBox autocomplete="off"  ID="txtEmailMsg" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size64", "size72") %>' TextMode="MultiLine" Rows="3" runat="server" />
    <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat="server" TargetControlID="txtEmailMsg" WatermarkText="Message to be sent in email" WatermarkCssClass='<%# "watermark stdTxt " & IIf(IsMobile(), "size64", "size72") %>' />
    <br /><br />
    
    <%--<asp:Button ID="btnConfirm" CssClass="midButton_green largeBtn" runat="server" Text="Confirm Order" />--%>
    <asp:Button ID="btnConfirmAndEmail" CssClass="midButton_green" runat="server" Text="Approve Order" />
    <asp:Button ID="btnUpdate" CssClass="midButton_green" runat="server" Text="Update Order" />
    <%--<asp:Button ID="btnDelete" CssClass="midButton" runat="server" Text="Delete Order" />--%>
    <asp:LinkButton ID="lnkDelete" CssClass="linkLabel bottom" Text="Delete Order" runat="server" />
    <asp:Button ID="btnEmailUser_edit" Visible="False" CssClass="midButton" runat="server" Text="Email Me" /><%--Always hide this button--%>
    <asp:Button ID="btnEmailOther_edit" Visible="False" CssClass="midButton" runat="server" Text='<%# "Email " & IIf(Session("ORDERDTL_CMD") = "JFCADMIN", "Customer", "Vendor") %>' /><%--Always hide this button--%>
</asp:Panel>

<asp:Panel ID="pnlOrderDtl_View" style='<%# IIf(IsMobile(), "", "margin-left:533px") %>' runat="server">
    <%--Order detail--%>
    <asp:GridView ID="grdItem_view" runat="server" DataSourceID="dsItem_view" CssClass="STDTABLE" AutoGenerateColumns="false"  GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
        <%--<HeaderStyle CssClass="th tdLink left" />--%> <%--Doesn't work. Put HeaderStyle-CssClass="th left" in each column instead--%>
        <Columns>
            <%--Commented out because user cannot view the item details when clicking image for JETS orders as custGrpItem_id is empty--%>
            <%--<asp:TemplateField HeaderText="Image" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" >
                <ItemTemplate>
                    <asp:ImageButton ID="imgItem" Tooltip="View item info" runat="server" CommandName="DETAIL" CommandArgument='<%# Eval("custGrpItem_id")%>' CssClass="cellImg" ImageUrl='<%# GetItemImgUrl(CString(Eval("itemType")), CString(Eval("item_id"))) %>' />
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:BoundField HeaderText="Item #" DataField="itemIdNr_view" HtmlEncode="false" HeaderStyle-CssClass="th size10" ItemStyle-CssClass="size10 cellLbl gray" />
            <%--<asp:BoundField HeaderText="Description" DataField="itemDesc_view" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" />--%>
            <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblItemDesc" CssClass="cellLbl" Text='<%# Eval("itemDesc_view") %>' />
                    <asp:Label runat="server" ID="Label12" CssClass="cellLbl green" Visible='<%# Session("ORDERDTL_CMD") = "JFCADMIN" %>' Text='<%# "<br />" & CString(Eval("vpMemo")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
           <%--<asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size20" ItemStyle-CssClass="size20 cellLbl gray" />--%>
            <%--<asp:TemplateField HeaderText="Each/Case" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16 gray" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEaPerCs" CssClass="cellLbl" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='<%# Eval("eaPerCs")%>' />
                    <asp:Label runat="server" ID="Label0" Visible='<%# CString(Eval("eaPerCs")) <> "0" and CString(Eval("eaPerCs")) <> "1" and CString(Eval("lbPrice_OrderByQty")) = "0" AndAlso Val(Eval("minSalCnt")) <> Val(Eval("eaPerCs")) %>' Text='ea/cs' />
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="th number12" ItemStyle-CssClass="number12" >
                <ItemTemplate>
                    <%--<asp:Label runat="server" Text='(' Visible='<%# IsMobile() = False and Val(Eval("lbPrice_OrderByQty")) <> 0 and Val(Eval("csQty")) <> 0%>' />--%>
                    <asp:Label runat="server" ID="lblCsPrice" Text='<%# Eval("csprice", "{0:C}") %>' Visible='<%# Val(Eval("csQty")) <> 0%>' />
                    <%--<asp:Label runat="server" Text=')' Visible='<%# IsMobile() = False and Val(Eval("lbPrice_OrderByQty")) <> 0 and Val(Eval("csQty")) <> 0%>' />--%><asp:Label ID="Label1" runat="server" Text='<br />' Visible='<%# Val(Eval("csQty")) <> 0%>' />
                    <%--<asp:Label runat="server" Text='(' Visible='<%# IsMobile() = False and Val(Eval("lbPrice_OrderByQty")) <> 0 and Val(Eval("eaQty")) <> 0%>' />--%>
                    <asp:Label runat="server" ID="lblEaPrice" Text='<%# Eval("eaprice", "{0:C}") %>' Visible='<%# CString(Eval("eaQty")) <> "0"%>' />
                    <%--<asp:Label runat="server" Text=')' Visible='<%# IsMobile() = False and Val(Eval("lbPrice_OrderByQty")) <> 0 and Val(Eval("eaQty")) <> 0%>' />--%>
                    <asp:Label ID="Label2" runat="server" Text='<br />' Visible='<%# Val(Eval("eaQty")) <> 0%>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByWgt" Text='<%# Eval("lbPrice_OrderByWgt", "{0:C}") %>' Visible='<%# Val(Eval("lbPrice_OrderByWgt")) <> 0 %>' />
                    <asp:Label runat="server" ID="lblLbPrice_OrderByQty" Text='<%# Eval("lbPrice_OrderByQty", "{0:C}") %>' Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0 %>' CssClass="green" />
                   
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size6" ItemStyle-CssClass="size6 gray" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs" Text="/cs<br />" Visible='<%# Val(Eval("csQty")) <> 0%>' />
                    <asp:Label runat="server" ID="lblEa" Text='<%# IIf(Val(Eval("lbPrice_OrderByQty")) <> 0, "/pc", "/ea") & "<br />" %>' Visible='<%# Val(Eval("eaQty")) <> 0%>' />
                    <asp:Label runat="server" ID="lblLb_orderByWgt" Text="/lb" Visible='<%# Val(Eval("lbPrice_OrderByWgt")) <> 0%>' />
                    <asp:Label runat="server" ID="lblLb_orderByQty" Text="/lb" Visible='<%# Val(Eval("lbPrice_OrderByQty")) <> 0%>' />
                    </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Qty" HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6 bold" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCsQty" Text='<%# CString(Eval("csQty")) & "<br />" %>' Visible='<%# Val(Eval("csQty")) <> 0%>' />
                    <asp:Label runat="server" ID="Label7" CssClass="green" Text='<%# CString(Eval("csQty_vpsp")) & "<br />" %>' Visible='<%# Val(Eval("csQty_vpsp")) > 0 %>' />
                    <asp:Label runat="server" ID="lblEaQty" Text='<%# CString(Eval("eaQty")) & "<br />" %>' Visible='<%# Val(Eval("eaQty")) <> 0%>'/>
                    <asp:Label runat="server" ID="Label9" CssClass="green" Text='<%# CString(Eval("eaQty_vpsp")) & "<br />" %>' Visible='<%# Val(Eval("eaQty_vpsp")) > 0 %>' />
                    <asp:Label runat="server" ID="lblLbs" Text='<%# CString(Eval("lbs")) & "<br />" %>' Visible='<%# Val(Eval("lbs")) <> 0%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th" ItemStyle-CssClass="bold" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCs2" Text="cs<br />" Visible='<%# Val(Eval("csQty")) <> 0 %>' />
                    <asp:Label runat="server" ID="Label10" CssClass="green" Visible='<%# Val(Eval("csQty_vpsp")) > 0 %>' Text="cs_free<br />" />
                    <asp:Label runat="server" ID="lblEa2" Text='<%# IIf(Val(Eval("lbPrice_OrderByQty")) <> 0, "pc", "ea") & "<br />" %>' Visible='<%# Val(Eval("eaQty")) <> 0%>' />
                    <asp:Label runat="server" ID="Label11" CssClass="green" Visible='<%# Val(Eval("eaQty_vpsp")) > 0 %>' Text='<%# IIf(Val(Eval("lbPrice_OrderByQty")) <> 0, "pc_free", "ea_free") & "<br />" %>' />
                    <asp:Label runat="server" ID="lblLb2" Text="lb<br />" Visible='<%# Val(Eval("lbs")) <> 0%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="th number12" ItemStyle-CssClass="number12 red bold" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblLineAmt" CssClass="cellLbl" Text='<%# Eval("lineAmt", "{0:C}") %>' />
                    
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField HeaderText="Invoice" DataField="INVOICE" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl gray" />--%>
            <asp:TemplateField HeaderText="Invoice" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblRefWoLink" CssClass="cellLbl" Text='<%# Eval("invoice") %>' Visible='<%# NOT ((CString(Eval("allowCustDownloadInvoice")) = "Y" OrElse Session("ORDERDTL_CMD") = "JFCADMIN") AndAlso CString(Eval("fileName")) <> "") %>' />
                    <asp:LinkButton runat="server" ID="lnkRefWLink" CommandName="DOWNLOAD" CommandArgument='<%# Eval("invoice")%>' CssClass="cellLink" ForeColor="#4169E1" 
                        ToolTip='<%# "Download " & CString(Eval("fileName")) %>' Text='<%# Eval("invoice") %>' Visible='<%# (CString(Eval("allowCustDownloadInvoice")) = "Y" OrElse Session("ORDERDTL_CMD") = "JFCADMIN") AndAlso CString(Eval("fileName")) <> "" %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:SqlDataSource ID="dsItem_view" runat="server"></asp:SqlDataSource>
    <asp:Label ID="lblViewOnlyReason" runat="server" CssClass="red" />
    <br />
    <asp:Button ID="btnEmailUser_view" CssClass="midButton" runat="server" Text="Email Me" />
    <asp:Button ID="btnEmailOther_view" Visible="False" CssClass="midButton" runat="server" Text='<%# "Email " & IIf(Session("ORDERDTL_CMD") = "JFCADMIN", "Customer", "Vendor") %>' /><%--Always hide this button--%>
     <br />
       <br />
         <br />
           
      <asp:Label ID="lblViewSFA" runat="server" CssClass="red" />
       <br />
      <br />
      <asp:Button ID="btnChangeOrder" CssClass="midButton" runat="server" Text="Change Back to Order Placed Status" />
        
    <asp:Button ID="btnDeleteOrder"  CssClass="midButton" runat="server" Text="Delete Order" />
     <br class="turnOffFloat" />
   
</asp:Panel>

<%--Popup window to show item detail--%>
<uc:ShowItemDtl id="ctrShowItemDtl" runat="Server" />

<%--Popup window to set or change price for JFC Admin--%>
<asp:LinkButton ID="btnPopupTmp_setPrice"  runat="server" />
<ajaxToolKit:ModalPopupExtender ID="popDialog_setPrice" runat="server"
            BehaviorID="popDialog_setPrice" 
            TargetControlID="btnPopupTmp_setPrice"
            PopupControlID="pnlPopSetPrice"
            CancelControlID="btnPopClose_setPrice"
            BackgroundCssClass="modalBackground">
    <Animations>
        <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
        <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
    </Animations>
</ajaxToolKit:ModalPopupExtender>
<asp:Panel ID="pnlPopSetPrice" DefaultButton="btnPopUpdatePrice" style="display:none;" runat="server" CssClass="popup_dialog fontsize2">
    <div class="inner">
        <asp:LinkButton ID="btnPopClose_setPrice" CssClass="popClose" runat="server" Text="x" /><%--Use LinkButton instead of ImageButton because progress bar shows after closed in some browser.--%>
        <h1>Set Price</h1>
        <table>
            <tr><td colspan="2"><br /></td></tr>
            <tr>
                <td class="gray left size8">Cost</td>
                <td><asp:Label runat="server" ID="lblPopCost" /></td>
            </tr>
             <tr>
                <td class="gray left size8">Bottom</td>
                <td><asp:Label runat="server" ID="lblPopBtm" /></td>
            </tr>
            <tr>
                <td class="gray left">Price</td>
                <td><asp:TextBox runat="server" ID="txtPopPrice" onFocus="this.select()" CssClass="stdTxt size12" autocomplete="off"  /><%--&nbsp;/ cs--%>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender11" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtPopPrice" /> 
                    <asp:DropDownList ID="lstPopPriceUnit" CssClass="drpList" runat="server" />
                </td>
            </tr>
            <%--<tr>
                <td class="gray left">Unit</td>
                <td><asp:RadioButtonList ID="radPopPriceUnit" CssClass="radiobtn" runat="server" /></td>
            </tr>--%>
            <tr>
                <td colspan="2" class="red"><asp:Label runat="server" ID="lblPopMsg_setPrice" /></td>                  
            </tr>
            <tr><td colspan= "2" class="red" >After clicking OK, please click Update.</td></tr>
            <tr><td colspan="2"><br /></td></tr>
            <tr>
                <td colspan="2"><center><asp:Button ID="btnPopUpdatePrice" CssClass="midButton" runat="server" Text="OK"/></center></td>
            </tr>
        </table>     
        <br />
        <%--<div class="base">
        </div>--%>
    </div>
</asp:Panel>
