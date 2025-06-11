<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ShowItemDtl.ascx.vb" Inherits="Common_ShowItemDtl" %>
<%@ Import Namespace="Common" %>

<asp:LinkButton ID="btnPopupTmp"  runat="server" />
<ajaxToolKit:ModalPopupExtender ID="popDialog" runat="server"
            BehaviorID="popDialog" 
            TargetControlID="btnPopupTmp"
            PopupControlID="pnlPopItemDtl"
            CancelControlID="btnPopClose"
            BackgroundCssClass="modalBackground" >
    <Animations>
        <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
        <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
    </Animations>
</ajaxToolKit:ModalPopupExtender>
<%--CancelControlID="btnPopClose"--%><%--Error occurs when placing this inside--%>

<asp:Panel ID="pnlPopItemDtl" style="display:none;" runat="server" CssClass="popup_dialog fontsize2">
    <div class="inner">
        <%--<asp:ImageButton ID="btnPopClose" CssClass="popClose" runat="server" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' />--%>
        <asp:LinkButton ID="btnPopClose" CssClass="popClose" runat="server" Text="x" /><%--Use LinkButton instead of ImageButton because progress bar shows after closed in some browser.--%>
        <h1>Item Info</h1>
        <table>
            <asp:Panel ID="Panel1" Visible='<%# Not IsMobile() %>' runat="server">
                <tr>
                    <td rowspan="18"><asp:Image ID="imgPopItem" CssClass="popImg" runat="server" /></td><%--rowspan="15"--%>
                </tr>
                <tr><td colspan="2"><br /><br /></td></tr>
            </asp:Panel>
            <asp:Panel ID="Panel2" Visible='<%# IsMobile() %>' runat="server">
                <tr>
                    <td colspan="2"><asp:Image ID="imgPopItem2" CssClass="cellXLImg" runat="server" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td colspan="2"><asp:Label runat="server" ID="lblPopItemDesc" CssClass="cellLbl bold midLabel" />
                <asp:Label Visible="false" runat="server" ID="lblPopCustGrpItemID" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Label runat="server" ID="lblPopJfcRemark" CssClass="cellLbl green" /></td>
            </tr>
            <tr>
                <td class="gray right">Item #:</td>
                <td><asp:Label runat="server" ID="lblPopItemIdNr" CssClass="cellLbl" /></td>
            </tr>
            <asp:Panel ID="pnlPopOrderForm" runat="server">
                <tr>
                    <td class="gray right">Order Form:</td>
                    <td><asp:Label runat="server" ID="lblPopItemGrpName" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlPopVendor" runat="server">
                <tr>
                    <td class="gray right">Vendor:</td>
                    <td><asp:Label runat="server" ID="lblPopVendorName" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray right">JFC Category:</td>
                <td><asp:Label runat="server" ID="lblPopCtgName" CssClass="cellLbl" /></td>
            </tr>
            <asp:Panel ID="pnlPopEaPerCs" runat="server">
                <tr>
                    <td class="gray right">Each / Case:</td>
                    <td><asp:Label runat="server" ID="lblPopEaPerCs" CssClass="cellLbl" /></td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray right"><asp:Label runat="server" ID="lblPopAvgWgtTitle" Text="Average Weight:" /></td>
                <td><asp:Label runat="server" ID="lblPopAvgCsWgt" CssClass="cellLbl" />
                    <asp:Label runat="server" ID="lblPopAvgEaWgt" CssClass="cellLbl" /></td>
            </tr>
            <asp:Panel ID="pnlPopShowItemDtl" runat="server">
                <tr>
                    <td class="gray right">Weight:</td>
                    <td><asp:Label runat="server" ID="lblPopGrossWeight" CssClass="cellLbl" /></td>
                </tr>
                <tr>
                    <td class="gray right">Volume:</td>
                    <td><asp:Label runat="server" ID="lblPopCaseVol" CssClass="cellLbl" /></td>
                </tr>
                <%--<tr>
                    <td class="gray right">Pallet Tie / Height:</td>
                    <td><asp:Label runat="server" ID="lblPopPallet" CssClass="cellLbl" /></td>
                </tr>--%>
                <asp:Panel ID="Panel3" Visible='<%# Not IsMobile() %>' runat="server">
                    <tr>
                        <td class="gray right">Ingredient:</td>
                        <td><asp:Label runat="server" ID="lblPopIngredient" CssClass="cellLbl" /></td>
                    </tr>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="pnlPopShowPrice" runat="server">
                <tr>
                    <td class="gray right">Price:</td>
                    <td><asp:Label runat="server" ID="lblPopLbPrice_OrderByQty" CssClass="cellLbl green bold" />
                        <asp:Label runat="server" ID="lblPopCsPrice" CssClass="cellLbl red bold" /> 
                        <asp:Label runat="server" ID="lblPopEaPrice" CssClass="cellLbl red bold" /> 
                        <asp:Label runat="server" ID="lblPopLbPrice_OrderByWgt" CssClass="cellLbl red bold" /> 
                        <asp:Label runat="server" ID="lblPopAskPrice" Text='Ask' CssClass="cellLbl red bold" />
                        <asp:Label runat="server" ID="lblPopMinSalCnt_view" CssClass="cellLbl green" /> 
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="gray right top"><asp:Label runat="server" ID="lblPopQtyHistoryTitle" Text="Order History:" /></td>
                <td><asp:Label runat="server" ID="lblPopQtyHistory" CssClass="gray" /></td>
            </tr>
            <tr>   
                <td></td> 
                <td><asp:Label ID="lblPopAvailQty" CssClass="cellLbl green" runat="server" /></td>
            </tr>
            <tr><td colspan="2" class="gray"><br />Note: The information above is for reference only.</td></tr>
        </table>
        <br />
    </div>
</asp:Panel>
