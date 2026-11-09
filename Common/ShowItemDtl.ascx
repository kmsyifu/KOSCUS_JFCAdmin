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

<asp:Panel ID="pnlPopItemDtl" style="display:none;" runat="server" CssClass="popup_dialog kp kp-wide">
    <div class="kp-header">
        <h1>Item Info</h1>
        <asp:LinkButton ID="btnPopClose" CssClass="kp-close" runat="server" Text="&times;" ToolTip="Close" /><%--Use LinkButton instead of ImageButton because progress bar shows after closed in some browser.--%>
    </div>
    <div class="kp-body kp-scroll">
        <div class="kp-itemview">
            <asp:Panel ID="Panel1" Visible='<%# Not IsMobile() %>' CssClass="kp-itemimg" runat="server">
                <asp:Image ID="imgPopItem" CssClass="popImg" runat="server" />
            </asp:Panel>
            <div class="kp-iteminfo">
                <asp:Panel ID="Panel2" Visible='<%# IsMobile() %>' CssClass="kp-media" runat="server">
                    <asp:Image ID="imgPopItem2" CssClass="cellXLImg" runat="server" />
                </asp:Panel>
                <div class="kp-summary">
                    <asp:Label runat="server" ID="lblPopItemDesc" CssClass="kp-title" />
                    <asp:Label Visible="false" runat="server" ID="lblPopCustGrpItemID" />
                    <asp:Label runat="server" ID="lblPopJfcRemark" CssClass="green" />
                </div>
                <div class="kp-kv">
                    <div class="kp-row"><span class="kp-k">Item #</span><span class="kp-v"><asp:Label runat="server" ID="lblPopItemIdNr" /></span></div>
                    <asp:Panel ID="pnlPopOrderForm" CssClass="kp-row" runat="server">
                        <span class="kp-k">Order Form</span><span class="kp-v"><asp:Label runat="server" ID="lblPopItemGrpName" /></span>
                    </asp:Panel>
                    <asp:Panel ID="pnlPopVendor" CssClass="kp-row" runat="server">
                        <span class="kp-k">Vendor</span><span class="kp-v"><asp:Label runat="server" ID="lblPopVendorName" /></span>
                    </asp:Panel>
                    <div class="kp-row"><span class="kp-k">JFC Category</span><span class="kp-v"><asp:Label runat="server" ID="lblPopCtgName" /></span></div>
                    <asp:Panel ID="pnlPopEaPerCs" CssClass="kp-row" runat="server">
                        <span class="kp-k">Each / Case</span><span class="kp-v"><asp:Label runat="server" ID="lblPopEaPerCs" /></span>
                    </asp:Panel>
                    <div class="kp-row"><span class="kp-k"><asp:Label runat="server" ID="lblPopAvgWgtTitle" Text="Average Weight" /></span><span class="kp-v"><asp:Label runat="server" ID="lblPopAvgCsWgt" />
                        <asp:Label runat="server" ID="lblPopAvgEaWgt" /></span></div>
                    <asp:Panel ID="pnlPopShowItemDtl" CssClass="kp-rowgroup" runat="server">
                        <div class="kp-row"><span class="kp-k">Weight</span><span class="kp-v"><asp:Label runat="server" ID="lblPopGrossWeight" /></span></div>
                        <div class="kp-row"><span class="kp-k">Volume</span><span class="kp-v"><asp:Label runat="server" ID="lblPopCaseVol" /></span></div>
                        <%--<div class="kp-row"><span class="kp-k">Pallet Tie / Height</span><span class="kp-v"><asp:Label runat="server" ID="lblPopPallet" /></span></div>--%>
                        <asp:Panel ID="Panel3" Visible='<%# Not IsMobile() %>' CssClass="kp-row" runat="server">
                            <span class="kp-k">Ingredient</span><span class="kp-v"><asp:Label runat="server" ID="lblPopIngredient" /></span>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="pnlPopShowPrice" CssClass="kp-row" runat="server">
                        <span class="kp-k">Price</span>
                        <span class="kp-v"><asp:Label runat="server" ID="lblPopLbPrice_OrderByQty" CssClass="green bold" />
                            <asp:Label runat="server" ID="lblPopCsPrice" CssClass="red bold" />
                            <asp:Label runat="server" ID="lblPopEaPrice" CssClass="red bold" />
                            <asp:Label runat="server" ID="lblPopLbPrice_OrderByWgt" CssClass="red bold" />
                            <asp:Label runat="server" ID="lblPopAskPrice" Text='Ask' CssClass="red bold" />
                            <asp:Label runat="server" ID="lblPopMinSalCnt_view" CssClass="green" /></span>
                    </asp:Panel>
                    <div class="kp-row"><span class="kp-k"><asp:Label runat="server" ID="lblPopQtyHistoryTitle" Text="Order History" /></span><span class="kp-v"><asp:Label runat="server" ID="lblPopQtyHistory" CssClass="gray" />
                        <asp:Label ID="lblPopAvailQty" CssClass="green" runat="server" /></span></div>
                </div>
                <div class="kp-small">Note: The information above is for reference only.</div>
            </div>
        </div>
    </div>
</asp:Panel>
