<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="OrderForm.aspx.vb" Inherits="JFCAdmin_OrderForm" %>
<%@ Import Namespace="Common" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        function loadData(id, id2, id3, id4, code, callback) {
            $.ajax({
                type: 'POST',
                dataType: "text",
                url: '../Common/Suggestion.ashx?id=' + id + '&id2=' + id2 + '&id3=' + id3 + '&id4=' + id4 + '&code=' + code + '',
                data: '{}',
                async: false,
                success: function (data) {
                    callback(data);
                },
                error: function () {
                    alert('Error occured');
                }
            });
        }

        $(document).ready(function () {
            $(".disableEnter").keypress(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                }
            });

            <%= JavaScript %>
        });
    </script>

    

    <%-- ══════════════════════════════════════════════
         Branch / search bar  (mirrors CustItem top bar)
    ═══════════════════════════════════════════════════ --%>
    <asp:Panel ID="pnlBranch"  CssClass="visiblePnl" runat="server">
        <h1>Order Form&nbsp;&nbsp;</h1>
        <asp:Label ID="lblGetBranchType" runat="server" Text="Branch" CssClass="gray" />
        <asp:DropDownList ID="lstBranch" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>'
            OnChange="javascript:showProgress();" runat="server"
            AutoPostBack="True" OnSelectedIndexChanged="lstBranch_SelectedIndexChanged" />
        &nbsp;
        <asp:Label ID="lblBranchID" CssClass="visiblePnl" runat="server" Visible="false" />
    </asp:Panel>    
    <br />
        <asp:Label ID="lblTitle" runat="server" Text="Please select a branch." CssClass="gray" />
    
 


    <%-- ══════════════════════════════════════════════
         Main area — shown after branch is selected
    ═══════════════════════════════════════════════════ --%>
    <div id="divMain" runat="server">

        <%-- ─────────────────────────────────────────
             LIST view
        ──────────────────────────────────────────── --%>
        <asp:Panel ID="pnlList" runat="server">

            <asp:Label ID="lblListMsg" CssClass="red" runat="server" /><br />

            <h2>Order Forms</h2>
               
            <asp:Panel ID="pnlSearch" DefaultButton="btnSearch" CssClass="visiblePnl" runat="server">
           
            <asp:Label ID="lblSearch" runat="server" Text="Search" CssClass="gray" />
               <asp:TextBox ID="txtSearchOrderForm" CssClass="largeTxt size48" runat="server" autocomplete="off" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="watSearchOrderForm" runat="server"
                    TargetControlID="txtSearchOrderForm"
                    WatermarkText="Search by Order Form"
                    WatermarkCssClass="watermark largeTxt size48" />
                &nbsp;
                <asp:Label runat="server" Text="Customer #" CssClass="gray" />
                <asp:TextBox ID="txtSearchCustNr" CssClass="stdTxt size8" runat="server" autocomplete="off" />
                <asp:ImageButton ID="btnSearch" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server" />
        
            </asp:Panel>

            <asp:GridView ID="grdOrderForms" runat="server"
                CssClass="STDTABLE" AutoGenerateColumns="false"
                GridLines="Horizontal" ShowHeaderWhenEmpty="true"
                DataKeyNames="orderForm_id"
                OnRowCommand="grdOrderForms_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Name"
                        HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1"
                                CommandName="OPEN_FORM"
                                CommandArgument='<%# Eval("orderForm_id") %>'
                                Text='<%# Eval("orderFormName") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField HeaderText="Exception Price Group" DataField="JFCPriceGrpID_Orig"
                        HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48 cellLbl gray"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px" />

                    <asp:TemplateField HeaderStyle-CssClass="th center4" ItemStyle-CssClass="center4"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ForeColor="red" CssClass="cellLink"
                                CommandName="DELETE_FORM"
                                CommandArgument='<%# Eval("orderForm_id") %>'
                                Text="x"
                                OnClientClick="return confirm('Delete this order form and all its items?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label runat="server" CssClass="gray" Text="No order forms found." />
                </EmptyDataTemplate>
            </asp:GridView>

            <br />
            <asp:Button ID="btnAddForm" runat="server" CssClass="midButton"
                Text="Add Order Form"
                OnClientClick="javascript:showProgress();" />

        </asp:Panel>

        <%-- ─────────────────────────────────────────
             DETAIL view  (Add / Update)
        ──────────────────────────────────────────── --%>
        <asp:Panel ID="pnlDetail" runat="server" Visible="false">
            <asp:LinkButton ID="lnkBack_order" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to order Forms..." ></asp:LinkButton>
           
            <br />
            <asp:Label ID="lblDetailMsg" CssClass="red" runat="server" /><br />
            <asp:HiddenField ID="hfOrderFormID" runat="server" />

            <h2><asp:Label ID="lblDetailTitle" runat="server" /></h2>
            <asp:Panel ID="pnlOrderDetail" CssClass="visiblePnl" runat="server">
            <h1>Detail of Order Form</h1>
            <table>
                <tr>
                    <td class="rowHdr">Order Form Name</td>
                    <td>
                        <asp:TextBox ID="txtFormName" CssClass="stdTxt size48 disableEnter"
                            runat="server" autocomplete="off" />
                    </td>
                </tr>
                <tr>
                    <td class="rowHdr">Exception Price Group</td>
                    <td>
                        <asp:DropDownList ID="lstExcepPriceGrp" CssClass="largeDrpList" runat="server"
                            OnChange="javascript:showProgress();" AutoPostBack="True" />
                    </td>
                </tr>
                <tr>
                    <td class="rowHdr">Created on</td>
                    <td><asp:Label ID="lblCreatedOn" CssClass="cellLbl gray" runat="server" /></td>
                </tr>
                <tr>
                    <td class="rowHdr">Last Updated</td>
                    <td><asp:Label ID="lblLastUpdated" CssClass="cellLbl gray" runat="server" /></td>
                </tr>
            </table>
            </asp:Panel>

            <br />
            <%--<hr />--%>
            <br />
            <%-- ── Assigned Customers ── --%>
            <h3>Assigned Customers</h3>

            <%-- All customers of the selected exception price group; checked = saved to the form --%>
            <asp:GridView ID="grdCustomers" runat="server"
                CssClass="STDTABLE" AutoGenerateColumns="false"
                GridLines="Horizontal" ShowHeaderWhenEmpty="true"
                DataKeyNames="customer_id">
                <Columns>
                    <asp:TemplateField HeaderStyle-CssClass="th center4" ItemStyle-CssClass="center4"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <HeaderTemplate>
                            <asp:CheckBox ID="ChkAllCust" onclick="javascript:checkAll(this);" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCustRow" runat="server"
                                Checked='<%# CBool(Eval("isAssigned")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Customer ID"
                        HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# CString(Eval("customer_id")).Insert(2, "-") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField HeaderText="Name" DataField="customerName"
                        HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px" />
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label runat="server" CssClass="gray" Text="No customers in this exception price group." />
                </EmptyDataTemplate>
            </asp:GridView>

            <br />
          <%--  <hr />--%>

            <%-- ── Items ── --%>
            <h3>Items</h3>

            <asp:Panel ID="pnlSearchItem" CssClass='<%# "visiblePnl " & IIf(IsMobile(), "", "leftContent") %>'
                DefaultButton="btnSearchItem" runat="server">
                <b>Search Items</b><br />
                <asp:Label runat="server" Text="Sort by" CssClass="gray" />
                <asp:DropDownList ID="lstSortBy" CssClass="largeDrpList"
                    OnChange="javascript:showProgress();" runat="server"
                    AutoPostBack="True" OnSelectedIndexChanged="lstSortBy_SelectedIndexChanged" />
                &nbsp;
                <asp:Label runat="server" Text="Category" CssClass="gray" />
                <asp:DropDownList ID="lstCtgSearch" CssClass="largeDrpList"
                    OnChange="javascript:showProgress();" runat="server"
                    AutoPostBack="True" OnSelectedIndexChanged="lstCtgSearch_SelectedIndexChanged" />
                <br />
                <asp:TextBox ID="txtItemSearch" CssClass="largeTxt size48" runat="server" autocomplete="off" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="watItemSearch" runat="server"
                    TargetControlID="txtItemSearch"
                    WatermarkText="Search by item"
                    WatermarkCssClass="watermark largeTxt size48" />
                <asp:ImageButton ID="btnSearchItem" CssClass="xsImgBtn"
                    ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server"
                    OnClick="btnSearchItem_Click" />
                <br />
            </asp:Panel>

            <br style="clear:both;" />



            <asp:Label ID="lblItemMsg" CssClass="red" runat="server" />
            <asp:Label ID="lblItemPendingMsg" CssClass="green" runat="server" />

            <asp:GridView ID="grdItems" runat="server"
                CssClass="STDTABLE" AutoGenerateColumns="false"
                GridLines="Horizontal" ShowHeaderWhenEmpty="true"
                DataKeyNames="item_id"
                OnRowDataBound="grdItems_RowDataBound"
                OnRowCommand="grdItems_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderStyle-CssClass="th center4" ItemStyle-CssClass="center4"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <HeaderTemplate>
                            <asp:CheckBox ID="ChkAllItems" onclick="javascript:checkAll(this);" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkItemRow" runat="server"
                                Checked='<%# CBool(Eval("isAssigned")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Item ID"
                        HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1"
                                CommandName="ITEM_DETAIL"
                                CommandArgument='<%# Eval("item_id") %>'
                                Text='<%# IIf(CString(Eval("itemNr")) <> "", CString(Eval("itemNr")), CString(Eval("item_id"))) %>' />
                            <asp:Label runat="server" CssClass="gray"
                                Text='<%# IIf(CString(Eval("itemNr")) <> "", " (" & CString(Eval("item_id")) & ")", "") %>' />
                            <asp:Label runat="server" ID="lblAvailMsg" CssClass="green" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Description"
                        HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblItemDesc"
                                Text='<%# Eval("itemDesc_view") %>' />
                            <asp:Label runat="server" ID="lblAvailNote" CssClass="green"
                                Text='<%# IIf(CString(Eval("availMsg")) <> "", "<br />" & CString(Eval("availMsg")), "") %>'
                                Visible='<%# CString(Eval("availMsg")) <> "" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField HeaderText="Category" DataField="ctgName_view"
                        HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-HorizontalAlign="Left"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px" />

                    <asp:TemplateField HeaderText="Ea/Cs"
                        HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# IIf(Val(Eval("eaPerCs")) > 1 AndAlso Val(Eval("lbPrice_OrderByQty")) = 0, CString(Eval("eaPerCs")), "") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Case Price"
                        HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# IIf(Val(Eval("csPrice")) <> 0, Eval("csPrice", "{0:N2}"), "") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Each Price"
                        HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# IIf(Val(Eval("eaPrice")) <> 0, Eval("eaPrice", "{0:N2}"), "") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Lb Price"
                        HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6"
                        HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# IIf(Val(Eval("lbPrice_OrderByQty")) <> 0, Eval("lbPrice_OrderByQty", "{0:N2}"), "") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField ItemStyle-CssClass="center4"
                        HeaderStyle-Height="30px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:ImageButton ID="btnRemoveItem" Tooltip="Remove item"
                                CommandName="REMOVE_ITEM" CssClass="xxsImgBtn"
                                ImageUrl='<%# GetWebImgUrl("close.jpg") %>'
                                CommandArgument='<%# Eval("item_id") %>'
                                runat="server"
                                Visible='<%# CBool(Eval("isAssigned")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label runat="server" CssClass="gray" Text="Not found" />
                </EmptyDataTemplate>
            </asp:GridView>

            <br />
            <asp:Button ID="btnSaveForm" runat="server" CssClass="midButton_green"
                Text="Add Form"
                OnClick="btnSaveForm_Click"
                OnClientClick="javascript:showProgress();" />
            &nbsp;
            <asp:Button ID="btnDeleteForm" runat="server" CssClass="midButton"
                Text="Delete Form" Visible="false"
                OnClick="btnDeleteForm_Click"
                OnClientClick="return confirm('Delete this order form and all its items?');" />
            &nbsp;
          

        </asp:Panel>

    </div>

</asp:Content>
