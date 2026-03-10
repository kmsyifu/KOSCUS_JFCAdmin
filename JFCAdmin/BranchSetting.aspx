<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="BranchSetting.aspx.vb" Inherits="BranchSetting"  EnableEventValidation="false" %>
<%@ Import Namespace="Common" %>
<%--%@ Register Tagprefix="uc" Tagname="OrderHistory" Src="~/Common/OrderHistory.ascx" --%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
   <script type="text/javascript">
          
            function uploadcomplete()  
            {  
                alert("successfully uploaded!");  
            }
     function toggleCheckboxes(source) {
        var checkBoxList = document.getElementById('<%= cblBranch.ClientID %>');
        var checkboxes = checkBoxList.getElementsByTagName('input');
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].type == 'checkbox') {
                checkboxes[i].checked = source.checked;
            }
        }
       }
       function validateFileSize() {
           var fileInput = document.getElementById('<%= fuHomepage.ClientID %>');
           if (fileInput.files.length > 0) {
               var file = fileInput.files[0];
               var maxSize = 4 * 1024 * 1024; // 4 MB
               if (file.size > maxSize) {
                   alert("The file is too large. Please upload a file smaller than 4 MB.");
                   return false; // cancel postback
               }
           }
           return true;
       }
          
     function applySortable() {
            var timer; // Variable to store timer ID
            // Function to initialize sortable with delay
            function initSortableWithDelay() {
               
                $(".drag_drop_grid").sortable({
                    items: 'tr:not(tr:first-child)',
                    cursor: 'pointer',
                    axis: 'y',
                    dropOnEmpty: false,
                    delay: 1000,
                    start: function (e, ui) {
                        ui.item.addClass("selected");
                    },
                    stop: function (e, ui) {
                        ui.item.removeClass("selected");
                        // Clear the timer when dragging stops
                        clearTimeout(timer);
                        // Collecting data from all items except the first child
                        var items = [];
                        $(".drag_drop_grid tr:not(tr:first-child)").each(function () {
                            var itemData = {
                                item_id: $(this).find('td:eq(1) [name="img_id"]').val() // Assuming 'id' is the identifier of each item
                                // Add other data you want to send here, e.g., additional attributes or text within these rows
                            };
                            items.push(itemData);
                        });
                        var dataToSend = JSON.stringify({ items: items });

                        // Fetching other values like id2 from lblCustomerID and id3 from dropbox
                        var id = $('#<%= lstBranch.ClientID %>').val();
                        var id2 = $('#<%= lstImgCtg.ClientID %>').val();

                        $.ajax({
                            type: 'POST',
                            dataType: "text",
                            url: '../Common/Suggestion.ashx?id=' + id + '&id2=' + id2 + '&id3=&id4=&code=HOMEIMGLIST',
                            data: dataToSend,
                            success: function (data) {
                                // Handle success response
                                callback(data);
                            },
                            error: function () {
                                // Handle error
                                alert('Error occurred');
                            }
                        });
                       
                    }
                });
            }
            // Callback function to handle AJAX success response
            function callback(data) {
                // Implement your logic here, e.g., update the UI based on the response
                console.log('Success:', data);
                // Additional success handling logic can be added here
            }
            initSortableWithDelay();

        }                      
        $(document).ready(function () {
           var initialLoad = true;
            //$("#tabs").tabs();
            var activeTabIndex = parseInt($('#<%= hfActiveTab.ClientID %>').val()) || 0;
           applySortable();
            $("#tabs").tabs({
                 active: activeTabIndex,
                 activate: function (event, ui) {                 
                    var activeTab = ui.newTab.index();
                   // var url = "BranchSetting.aspx/UpdateViewState";
                     $('#<%= hfActiveTab.ClientID %>').val(activeTab);
                     showProgress();
                    __doPostBack('TabChanged', activeTab);
//                    $.ajax({
//                        url: url,
//                        type: 'POST',
//                        data: JSON.stringify({ tab: activeTab }),
//                        contentType: "application/json; charset=utf-8",
//                        dataType: "json",
//                        success: function (response) {
//                            console.log("ViewState updated successfully");
//                        },
//                        error: function (xhr, status, error) {
//                            console.error("Error updating ViewState: " + error);
//                        }
//                    });
                }
            });
             $("#tabs").show();
            $("#tabs").removeClass("ui-widget");
            $('#tabs').css('display', 'block');
           //Display tabs when page finish loading
          
            $(".datepicker").datepicker();
        
           $(".resetCursor").mouseup(function () {
             this.setSelectionRange(0,0);    
            });
            
    
         
           
            //.keydown(function(){alert('a');});
             //$(".CODE_ITEM").on('autocompleteselect', function (e, ui) {
               //function(){alert('a');}
              //});
            <%= JavaScript %>
        });
   </script>
    <h1>Branch Settings&nbsp;&nbsp;</h1>
    <asp:Panel ID="pnlSearchCust"   CssClass="visiblePnl" runat="server" visiable="false"> 
      <asp:Label ID="lblGetCustCompanyType" runat="server" Text='Branch'  CssClass="gray" />
      <asp:DropDownList ID="lstBranch" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' OnChange="javascript:showProgress();" runat="server" AutoPostBack="True" />
      <asp:Label ID = "lblCustomerID" CssClass="visiblePnl" runat="server" Visible ="false"/>
    </asp:Panel>
    <br />
    
    <asp:Label ID="lblTitle" runat="server"  Text='Please select a customer.'  CssClass=" gray" Visible ="false"  />

     <asp:HiddenField ID="hfActiveTab" runat="server" />
      <asp:HiddenField ID="hfPrevTab" runat="server" />
    <div id="Alltab"  runat ="server"  >
        <div id="tabs" style="display: none;"> <%--Display tabs when page finish loading--%> 
            <ul>
                <li><a href="#tabPromotion">Promo Items</a></li>
                <li><a href="#tabHomePageIMG">Homepage Images</a></li>
                <li><a href="#tabSchedule">Delivery Master</a></li>
                <li><a href="#tabNotification">Notification</a></li>
            </ul>
   <%----------------------------------------------------------------------------------------------------------------------
            Promotional Item
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel ID="pnlTabPromotion" runat="server">
        <div id="tabPromotion">
             <h2>Promo Items</h2>
              <asp:Panel ID = "pnlPromotionalItemlist" runat="server">
              <asp:LinkButton runat="server" ID="lnkDeletePromoItem" CssClass=" cellLink" ForeColor="#4169E1" >Delete checked items</asp:LinkButton>
              <asp:GridView ID="grdPromotionalItemlist" runat="server" DataSourceID="dsPromotionItem" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                           <asp:TemplateField   HeaderStyle-CssClass="th center4"  ItemStyle-CssClass="center4" >
                             <HeaderTemplate  >  
                                         <asp:CheckBox ID="ChkAll"  onclick="javascript:checkAll(this);"  runat="server" /> 
                              </HeaderTemplate>  
                            <ItemTemplate >
                                <asp:CheckBox CssClass=" size2" ID="chkRow" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>
                           <asp:TemplateField HeaderText="Item Code" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                               
                                <ItemTemplate>
                                 <asp:Label ID="lnkItemId" CssClass=" rowLblVal" runat ="server" Text='<%# Eval("ITEM_ID")%>'></asp:Label>
                                    <asp:linkButton ID="lblItemId" CssClass=" cellLink" ForeColor="#4169E1" Tooltip="Edit Item Code" runat="server" Text='<%# Eval("ITEM_ID")%>' CommandName="DETAIL" CommandArgument='<%# Eval("ITEM_ID")%>'  
                                    Visible ="false"/>
                                   
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:BoundField HeaderText="Item Description" DataField="ITEMDESC" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" /> 
                             <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete promo item" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  CommandArgument='<%# Cstring(Eval("ITEM_ID")) %>' runat="server" Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
              
                    <asp:SqlDataSource ID="dsPromotionItem" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="btnPromotionAdd" CssClass="midButton" runat="server" Text="Add Item" />
                    <br /><br />            
              </asp:Panel>

                <%--Popup window to Item detail--%>
                        <asp:LinkButton ID="btnPopupTmp_Promotion"  runat="server" />
                        <ajaxToolKit:ModalPopupExtender ID="popDialog_Promotion" runat="server"
                                    BehaviorID="popDialog_Promotion" 
                                    TargetControlID="btnPopupTmp_Promotion"
                                    PopupControlID="pnlPopPromotionDtl"
                                    BackgroundCssClass="modalBackground" >
                            <Animations>
                                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                            </Animations>
                        </ajaxToolKit:ModalPopupExtender>
                        <asp:Panel ID="pnlPopPromotionDtl" DefaultButton="btnPopSavePromotion" style="display:none;" runat="server" CssClass="popup_dialog">
                            <div class="inner">
                                <asp:ImageButton ID="popClosePromotion" CssClass="popClose" CausesValidation="False" runat="server" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' />
                                <h1><asp:Label ID="lblPopTitle" runat="server" Text="Add"/> Promo Item</h1>
                                <br />
                                 <asp:Panel runat="server" ID ="pnlDelPromotionwarning">
                               
                                 <asp:Label ID="lblPromotionwarning" CssClass=" red bold  " runat="server" />
                                 <br />
                                 <br />
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlDelProTable">              
                                    <table>
                                        <tr>
                                            <td class="rowHdr"> Items #</td>
                                            <td><asp:Textbox ID="txtPromoItems" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' Text=""  runat="server"  autocomplete="off"/>
                                            </td>
                                          </tr>
                                        <tr>
                                            <td colspan="2" class="red">
                                               <asp:Label ID="lblDescription" CssClass="rowLblVal " Text=""  runat="server" ></asp:Label>
                                               <asp:Label ID="lblPreItemId" Visible ="false" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="center">
                                                <asp:Button ID="btnPopSavePromotion" CssClass="midButton_green" runat="server" Text="Update" />
                                                <asp:Button ID="btnPopDeletePromotion" CssClass="midButton" runat="server" Text="Delete" />
                                             </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                            </div>
                        </asp:Panel>
        </div>
        </asp:Panel>

        <%----------------------------------------------------------------------------------------------------------------------
            Homepage Images Picture
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel runat="server" ID="pnlTabHomePageIMG" >
            <div id="tabHomePageIMG">
                <asp:Panel runat="server" ID="pnlList_HomePageIMG">
                  
                    <h2>Homepage Images</h2>
                    <%--Images info--%>
                    <br />
                 
                    Image Category&nbsp;&nbsp;<asp:DropDownList ID="lstImgCtg" CssClass='<%# "drpList " & IIf(IsMobile(), "size12", "size24") %>' OnChange="javascript:showProgress();" runat="server" AutoPostBack="True" /> 
                    <br />
                     <br />
                    <%--List of delivery Master--%>
                    <asp:GridView ID="grdHomepageImg" runat="server" DataSourceID="dsHomepageImg" CssClass="drag_drop_grid STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                            <asp:TemplateField HeaderText="Image" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" >
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgHomepage" Tooltip="View item info" runat="server" CommandName="DETAIL" CommandArgument='<%# Eval("IMGNAME")%>'  
                                    CssClass="cellImg"  ImageUrl='<%# GetHomePageImg(Eval("JFCBRANCHNR"), Cstring(Eval("FOLDERPATH")).Substring(Cstring(Eval("FOLDERPATH")).LastIndexOf("\") + 1)) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size1"  ItemStyle-CssClass="size1" >
                            <ItemTemplate>
                                <input type="hidden" name="img_id"  value='<%# Eval("HOMEPAGEIMG_ID") %>'  />
                            </ItemTemplate>

                        </asp:TemplateField>
                            <asp:BoundField HeaderText="Img Name" DataField="IMGNAME" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            <asp:BoundField HeaderText="User" DataField="CreateUser" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" Visible ="false" />
                            <asp:BoundField HeaderText="Date" DataField="CreateDate" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" Visible ="false" />
                            <asp:BoundField HeaderText="Item ID" DataField="item_id" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16"  />
                             <asp:BoundField HeaderText="URL" DataField="REDIRECTURL" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"/>
                            <asp:TemplateField HeaderText="Order" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" Visible="false"  >
                               <ItemTemplate>
                                    <asp:Label id = "lblOrder" runat="server" Text='<%# Eval("DISPORDER")%>'  ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Order <br/> (Hold 1 sec to move)" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" >
                             
                            <ItemTemplate>
                            <asp:Image ID="btnSort" CssClass="xsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("sort.png") %>' />
                             <%--<asp:ImageButton ID="btnUp" CommandName="UP" CssClass="xsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("up.jpg") %>' CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />
                             <asp:ImageButton ID="btnDown" CommandName="DOWN" CssClass="xsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("down.png") %>' CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" />--%>
                           </ItemTemplate>
                       </asp:TemplateField>
                            <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete Homepage Image" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  CommandArgument='<%# Cstring(Eval("FOLDERPATH")) %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsHomepageImg" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="btnAddHomepageImg" CssClass="midButton" runat="server" Text="Add Image"  />
                    <br /><br />            

                      <%--asp:TemplateField HeaderText="Send Reminder" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>' Visible='<%# CString(Eval("sendReminder")) = "Y" %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField --%>
                
                </asp:Panel>

                <%--Detail of HomePageImg--%>
                <asp:Panel runat="server" ID="pnlDtl_HomePageImg" >
                    <asp:LinkButton ID="lnkBack_HomePageImg" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to homepage image list..." ></asp:LinkButton>
                    <br /><br />
                    <h2><asp:Label ID="lblDtlTitle_HomePageImg" runat="server" Text='Detail of HomePage Image' /></h2>
                    <asp:Panel runat="server" ID="pnlDtlEdit_HomePageImg" DefaultButton="btnSaveHomePageImg">
                        <%--May need to remove later (not used)--%>
                      
                        <%--Add / edit HomePageImg--%>
                        <asp:Panel ID="pnlSingleHomePageImg" CssClass="visiblePnl leftOnlyContent" runat="server">
                            <h1>Homepage Image</h1>
                           
                            <asp:Label ID="lblHomePageID" visible="false" runat="server"/>
                            <table>
                                <tbody>
                                <tr>
                                  <td class="rowHdr">Image Category</td>
                                    <td><asp:Label ID="lblHomeCtg" visible="True" runat="server"/> </td>
                                </tr>
                                <asp:Panel runat="server" ID="pnlHomePage">
                                    <tr>
                                        <td class="rowHdr">Image</td>
                                        <td><asp:Image ID="ImgHomepage" CssClass="largeGrdImg"   runat="server" /> 
                                        <br />  <asp:FileUpload   ID="fuHomepage"
                                        AllowedFileTypes="png,jpg"
                                        MaximumNumberOfFiles= "1"
                                        UseAbsoluteHandlerPath = "false" 
                                        runat="server"  /> <br />
                                        <asp:RegularExpressionValidator ID="uplValidator" runat="server" ControlToValidate="fuHomepage" ForeColor="Red" 
                                        ErrorMessage="Image format has to be .png or .jpg" ValidationGroup = "Homepageupload" ValidationExpression="(?i)(.+\.(png|jpg|jpeg)$)">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        </asp:RegularExpressionValidator><br /></td>
                                        
                                    </tr>
                                </asp:Panel>
                                
                                <tr>
                                    <td class="rowHdr">Name</td>
                                    <td><asp:Textbox ID="txtHomePageName" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size32", "size48") %>' runat="server" autocomplete="off" /></td>
                                </tr>
                                 <asp:Panel runat="server" ID="pnlReceipt">
                                      <tr>
                                        <td class="rowHdr"><asp:Label ID ="lblURL" runat = "server" Text ="URL or Item ID" /></td>
                                        <td><asp:Textbox ID="txtImgUrl" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size32") %>' runat="server" autocomplete="off" />
                                           <asp:Label ID ="lblOr" runat = "server" Text ="&nbsp; or &nbsp;" /> <asp:Textbox ID="txtItemId" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size8", "size8") %>' runat="server" autocomplete="off" />
                                        </td>
                                       </tr>
                                 </asp:Panel>
                                  <asp:Panel runat="server" ID="pnlitem">
                                      <%--<tr>
                                        <td class="rowHdr">Item ID</td>
                                        <td></td>
                                       </tr>--%>
                                 </asp:Panel>
                                 <asp:Panel runat = "server" ID="pnlBranchList">
                                 <tr>
                                        <td class="rowHdr">Branch</td>
                                        <td> <asp:CheckBox  ID="cbAll" runat ="server" CssClass= "checkbox" text="All" class="fontsize-1" onclick="toggleCheckboxes(this)" /> <br />
                                        <asp:CheckBoxList ID="cblBranch" runat="server"></asp:CheckBoxList>
                                        </td>
                                 </tr>
                                 </asp:Panel>
                                <tr>
                                    <td class="rowHdr">Create on</td>
                                    <td><asp:Label ID="lblCreateDateName" CssClass="rowLblVal" runat="server" /><asp:Label ID="lblShipPointID_schedule" Visible="false" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                </tbody>
                            </table>
                            <br />
                        </asp:Panel>
                       
                        <br />
                 
                        <%-- <asp:Label ID="Label27" CssClass="red" runat="server" Text="* Required"/>
                        asp:CheckBox  ID="chkEmailMaster" runat ="server" CssClass= "checkbox" text="Send email notification to customer about this change." class="fontsize-1"/ --%>
                        <br />
                        <asp:Button ID="btnSaveHomePageImg" ValidationGroup = "Homepageupload" CssClass="midButton_green" runat="server" Text="Update Changes" OnClientClick="return validateFileSize();" />
                        <asp:Button ID="btnDeleteHomePageImg" CssClass="midButton" runat="server" Text="Delete" />
                    </asp:Panel>
                    <br />
                </asp:Panel>
             </div>
         </asp:Panel>

         <%----------------------------------------------------------------------------------------------------------------------
            Notification
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel ID="pnlTabNotification" runat="server">
        <div id="tabNotification">
             <h2>Order Reminder</h2>
              <asp:Panel ID = "pnlorderremind" runat="server">
              <asp:GridView ID="grdOrderReminder" runat="server" DataSourceID="dsOrderReminder" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                           <asp:TemplateField HeaderText="Hours Before Deadline" HeaderStyle-CssClass="th center12" ItemStyle-CssClass="center12" >
                                <ItemTemplate>
                                    <asp:linkButton ID="lnkHoursprior" CssClass=" cellLink" ForeColor="#4169E1" Tooltip="Edit Hours prior" runat="server" Text='<%# Eval("Settingvalue1")%>' CommandName="DETAIL" CommandArgument='<%# Eval("Settingvalue1")%>'  
                                   />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete Hours prior" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  CommandArgument='<%# Cstring(Eval("Settingvalue1")) %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
              
              <asp:SqlDataSource ID="dsOrderReminder" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="BtnOrderReminder" CssClass="midButton" runat="server" Text="Add Order Reminder" />
                    <br /><br />            
             
                      
              </asp:Panel>

                <%--Popup window to Item detail--%>
                        <asp:LinkButton ID="btnPopupTmp_OrderDtl"  runat="server" />
                        <ajaxToolKit:ModalPopupExtender ID="popDialog_OrderReminder" runat="server"
                                    BehaviorID="popDialog_OrderReminder" 
                                    TargetControlID="btnPopupTmp_OrderDtl"
                                    PopupControlID="pnlPopOrderDtl"
                                    BackgroundCssClass="modalBackground" >
                            <Animations>
                                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                            </Animations>
                        </ajaxToolKit:ModalPopupExtender>
                        <asp:Panel ID="pnlPopOrderDtl" DefaultButton="btnPopSaveOrder" style="display:none;" runat="server" CssClass="popup_dialog">
                            <div class="inner">
                                <asp:ImageButton ID="btnPopClose_Order" CssClass="popClose" CausesValidation="False" runat="server" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' />
                                <h1><asp:Label ID="lblPopHeader_Order" runat="server" Text="Add/Edit"/> Order Reminder Info</h1>
                                <br />
                                 <asp:Panel runat="server" ID ="pnlPopWarningOrder">
                               
                                 <asp:Label ID="lblPopWarning" CssClass=" red bold  " runat="server" />
                                   <br />
                                 <br />
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlPopDtlEdit_Order">              
                                    <table>
                                        <tr>
                                            <td class="rowHdr"> Hours Before Deadline</td>
                                            <td><asp:Textbox ID="txtHourprior" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' Text=""  runat="server"  autocomplete="off"/>
                                          
                                            </td>
                                               
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="red">
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="center">
                                                <asp:Button ID="btnPopSaveOrder" CssClass="midButton_green" runat="server" Text="Update" />
                                                <asp:Button ID="btnPopDeleteOrder" CssClass="midButton" runat="server" Text="Delete" />
                                                </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                            </div>
                        </asp:Panel>
        </div>
        </asp:Panel>
         <%----------------------------------------------------------------------------------------------------------------------
            Master Deliver
        ----------------------------------------------------------------------------------------------------------------------%>
        <%----------------------------------------------------------------------------------------------------------------------
            Manage delivery schedule master
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel runat="server" ID="pnlTabSchedule" >
            <div id="tabSchedule">
                <asp:Panel runat="server" ID="pnlList_schedule">
                    <%--Schedule delivery--%>
                    <h2>Delivery Schedule Master</h2>
                    <asp:CheckBox runat="server" ID="chkShowAll_schedule" Visible="false" class="fontsize-1" Text="&nbsp;Show all customers'" AutoPostBack="true" />
                    <%--Customer info
               
                    <asp:DropDownList ID="lstCustCompany_schedule" CssClass="drpList" runat="server" AutoPostBack="True" />--%>
                    <asp:Panel runat="server" ID="Panel21" Visible="false" style="display:inline;">
                        <asp:Label ID="Label20" runat="server" Text='<%# GetCustCompanyType() & " Group" %>' CssClass="gray" />
                        <asp:DropDownList ID="lstCustGrp_schedule" CssClass="largeDrpList" runat="server" AutoPostBack="True" OnChange="javascript:showProgress();" />
                        <asp:Label ID="Label22" runat="server" Text='<%# GetCustCompanyType() %>' CssClass="gray" />
                        <asp:DropDownList ID="lstCust_schedule" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                          <asp:Label ID="lblShipPoint" runat="server" Text="Shipping Destination" CssClass="gray" />
                    <asp:DropDownList ID="lstShipPoint" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                    </asp:Panel>
                    <br />
                    <%--List of delivery schedule--%>
                    <asp:GridView ID="grdActiveSchedule" runat="server" DataSourceID="dsActiveSchedule" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                            <asp:BoundField HeaderText="ID" DataField="DELIVERYMASTER_ID" Visible="false" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />
                            <asp:BoundField HeaderText="JFC Branch" DataField="JFCBRANCHNR" Visible="false" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" /> 
                            <asp:TemplateField HeaderText="Name" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" >
                                <ItemTemplate>
                                    <asp:Label Visible="false"  runat="server" ID="lblDeliverySchedID2" Text='<%# Eval("DELIVERYMASTERNAME") %>' />
                                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkShipName" Text='<%# Eval("DELIVERYMASTERNAME") %>' CommandName="EDIT_ROW" CommandArgument='<%# Eval("DELIVERYMASTER_ID")  %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                                                                          
                            <asp:BoundField HeaderText="Frequency" DataField="FREQUENCY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            <asp:BoundField HeaderText="Delivery Day" DataField="DELIVERYDAY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            <asp:BoundField HeaderText="Order By Day" DataField="orderByDay" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />
                            <asp:BoundField HeaderText="Order By Time" DataField="orderByTime" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />
                            <asp:TemplateField HeaderText="Send Reminder" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16" Visible="false">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>'  runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField HeaderText="Starting" DataField="STARTDATE" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" Visible="false" />
                            <asp:BoundField HeaderText="Ending" DataField="ENDDATE" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" Visible="false" />--%>
                            <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete Master" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  CommandArgument='<%# Eval("DELIVERYMASTER_ID") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsActiveSchedule" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="btnAddSchedule" CssClass="midButton" runat="server" Text="Add Master" />
                    <br /><br />            

                    <%--Calendar--%>
                    <asp:Panel runat="server" Visible='<%# IIf(IsMobile(), "False", "True") %>' ID="Panel1" >
                        <asp:Calendar ID="cldSchedule" runat="server" BackColor="#FFFFFF" 
                            BorderColor="#C0C0C0" DayNameFormat="Short" DayStyle-CssClass="left top rowHdr cellImg"
                            Font-Size="8pt" ForeColor="#666666" Height="600px" Width="1000px"
                            ShowGridLines="True" NextPrevFormat="CustomText" NextMonthText="Next >&nbsp;" PrevMonthText="&nbsp;< Previous">
                            <TitleStyle BackColor="#FFFFFF" BorderColor="#C0C0C0" Font-Size="9pt" Height="30px" ForeColor="#FF6600" />
                            <NextPrevStyle ForeColor="#4169E1" Font-Size="9pt" />
                            <DayHeaderStyle BackColor="#FFFFCC" ForeColor="#666666" BorderColor="#C0C0C0" Height="20px" Font-Bold="false"/>
                            <DayStyle BorderColor="#C0C0C0" />
                            <OtherMonthDayStyle BackColor="#F7F7F7" ForeColor="#CC9966" />
                            <TodayDayStyle BackColor="#FFFFCC" ForeColor="#000000" />
                            <SelectedDayStyle BackColor="#FFFFCC" />
                            <SelectorStyle BackColor="#FFFFCC" />
                        </asp:Calendar>
                        <br />
                        <asp:LinkButton ID="lnkDelivery" style="display:none;" runat="server" onclick="lnkDelivery_Click" />
                       

                        <%--Popup window to delivery detail--%>
                        <asp:LinkButton ID="btnPopupTmp_schedule"  runat="server" />
                        <ajaxToolKit:ModalPopupExtender ID="popDialog_schedule" runat="server"
                                    BehaviorID="popDialog_schedule" 
                                    TargetControlID="btnPopupTmp_schedule"
                                    PopupControlID="pnlPopSchedDtl"
                                    BackgroundCssClass="modalBackground" >
                            <Animations>
                                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                            </Animations>
                        </ajaxToolKit:ModalPopupExtender>
                        <asp:Panel ID="pnlPopSchedDtl" DefaultButton="btnPopSaveDelivery" style="display:none;" runat="server" CssClass="popup_dialog">
                            <div class="inner">
                                <asp:ImageButton ID="btnPopClose" CssClass="popClose theButtonClass" CausesValidation="False" runat="server"   ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  />
                                <h1><asp:Label ID="lblPopHeader_schedule" runat="server" Text="Edit"/> Delivery Master</h1>
                                <br />
                                <asp:Panel runat="server" ID="pnlPopDtlEdit_schedule">
                                 <asp:Panel runat="server" ID ="pnlPopSheduleWarning" >
                                 <asp:Label ID="lblPopSheduleWarning" CssClass=" red bold  " Text="If you update or delete, customers' delivery date will be affected too." runat="server" />
                                   <br />
                                    <br />
                                  </asp:Panel>              
                                    <table>
                                        <tr style="display:none;">
                                            <td class="rowHdr">Name</td>
                                            <td><asp:Label ID="lblPopStarCustGrpVendor" CssClass="red" runat="server" Text="*"/>
                                                <asp:Label ID="lblPopMasterName" CssClass="rowLblVal" runat="server" />
                                                <asp:Label ID="lblPopMasterID" CssClass="rowLblVal" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr">Delivery On</td>
                                            <td><asp:TextBox ID="txtPopDeliveryDate" ValidationGroup="validgroup" CssClass="stdTxt size12" Text="01/01/2013" runat="server" autocomplete="off" />&nbsp;(mm/dd/yyyy) <%--Text required so that validator wouldn't work by default--%>
                                              
                                                <ajaxToolKit:CalendarExtender ID="calPopDeliveryDate"  Format="MM/dd/yyyy" TargetControlID="txtPopDeliveryDate" runat="server"  />
                                                <asp:Label ID="Label30" CssClass="red" runat="server" Text="*"/>&nbsp;&nbsp;&nbsp;
                                                <asp:CustomValidator ValidationGroup="validgroup" id="vldPopDeliveryDate" CssClass="red" runat="server" ValidateEmptyText="true" ControlToValidate="txtPopDeliveryDate"  ErrorMessage="Invalid date" ClientValidationFunction="validateDate" />
                                                <asp:Label ID="lblPopOldDeliveryDate" Visible="false" runat="server" /></td>
                                        </tr>
                                        <tr> 
                                            <td class="rowHdr">Order by</td>
                                            <td><asp:TextBox ID="txtPopOrderByDate"  ValidationGroup="validgroup" CssClass="stdTxt size12" Text="01/01/2013" runat="server" autocomplete="off"  />&nbsp;(mm/dd/yyyy) <%--Text required so that validator wouldn't work by default--%>
                                                <ajaxToolKit:CalendarExtender ID="calPopOrderByDate" Format="MM/dd/yyyy" TargetControlID="txtPopOrderByDate" runat="server" />
                                                <asp:Label ID="Label29" CssClass="red" runat="server" Text="*"/>&nbsp;&nbsp;&nbsp;
                                                <asp:CustomValidator ValidationGroup="validgroup" id="vldPopOrderByDate" CssClass="red" runat="server" ValidateEmptyText="true"  ControlToValidate="txtPopOrderByDate" ErrorMessage="Invalid date" ClientValidationFunction="validateDate" /></td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr"></td>
                                            <td><asp:DropDownList ID="lstPopOrderByTime" CssClass="drpList size12" runat="server" />
                                                <asp:DropDownList ID="lstPopOrderByAmPm" CssClass="drpList size8" runat="server" />
                                                <asp:Label ID="Label32" CssClass="red" runat="server" Text="*"/></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="red"><asp:Label ID="Label33" CssClass="red" runat="server" Text="* Required"/></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="center"><asp:Button ID="btnPopSaveDelivery" CssClass="midButton_green" runat="server" Text="Update"  ValidationGroup="validgroup" />
                                                <asp:Button ID="btnPopDeleteDelivery" CssClass="midButton" runat="server" Text="Delete" CausesValidation="false" /></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                            </div>
                        </asp:Panel>
                        <br />
                    </asp:Panel>
                </asp:Panel>

                <%--Detail of Master--%>
                <asp:Panel runat="server" ID="pnlDtl_schedule" >
                    <asp:LinkButton ID="lnkBack_schedule" CssClass="linkLabel" ForeColor="#4169E1" runat="server" Text="<< Go back to the list..." ></asp:LinkButton>
                    <br /><br />
                    <h2><asp:Label ID="lblDtlTitle_schedule" runat="server" Text='Detail of Delivery Schedule Master' /></h2>
                    <asp:Panel runat="server" ID="pnlDtlEdit_schedule" DefaultButton="btnSaveSchedule">
                        <%--May need to remove later (not used)--%>
                        <%--Add / edit schedule--%>
                        <asp:Panel ID="pnlSingleSchedule" CssClass="visiblePnl leftOnlyContent" runat="server">
                            <h1>Delivery Schedule Master</h1>
                           
                            <table>
                                <tbody>
                                <tr style="display:none;">
                                    <td class="rowHdr"><%# GetCustCompanyType()%></td>
                                    <td><asp:Label ID="lblCustomerName_schedule" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr style="display:none;">
                                    <td class="rowHdr">Delivery Master ID</td>
                                    <td><asp:Label ID="lblJfcBranchNr_schedule" CssClass="rowLblVal" runat="server" />
                                        <asp:Label ID="lblMasterID" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Delivery Master Name</td>
                                    <td><asp:textbox ID="txtMasterName"  CssClass='<%# "rowLblVal  stdTxt " & IIf(IsMobile(), "size32", "size48") %>' runat="server" />
                                    <asp:Label ID="Label2" CssClass="red" runat="server" Text="*"/>
                                    </td>

                                </tr>
                                <tr >
                                    <td class="rowHdr">Frequency</td>
                                    <td><asp:DropDownList ID="lstFrequency" CssClass='<%# "drpList " & IIf(IsMobile(), "size32", "size48") %>' runat="server" />
                                    <asp:Label ID="Label1" CssClass="red" runat="server" Text="*"/>
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td class="rowHdr">Delivery On</td>
                                    <td><asp:DropDownList ID="lstDelivery" CssClass='<%# "drpList " & IIf(IsMobile(), "size32", "size48") %>' runat="server" />
                                    <asp:Label ID="Label3" CssClass="red" runat="server" Text="*"/></td>
                                </tr>
                                <tr >
                                    <td class="rowHdr" >Order by Day</td>
                                    <td><asp:DropDownList ID="lstOrderbyDate" CssClass='<%# "drpList " & IIf(IsMobile(), "size32", "size48") %>' runat="server" /><asp:Label ID="Label25" CssClass="red" runat="server" Text="*"/></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Order by Time</td>
                                    <td><asp:DropDownList ID="lstOrderbyTime" CssClass="cellLbl drpList size12" runat="server" />&nbsp;&nbsp;<asp:DropDownList ID="lstAMPM" CssClass="cellLbl drpList size12" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Start Date</td>
                                    <td><asp:TextBox ID="txtStartDate"  ValidationGroup="validgroup" CssClass="stdTxt size12" Text="01/01/2013" runat="server" autocomplete="off"  />&nbsp;(mm/dd/yyyy) <%--Text required so that validator wouldn't work by default--%>
                                                <ajaxToolKit:CalendarExtender ID="CExtenderStartDate" Format="MM/dd/yyyy" TargetControlID="txtStartDate" runat="server" />
                                                <asp:Label ID="Label4" CssClass="red" runat="server" Text="*"/>&nbsp;&nbsp;&nbsp;
                                                <asp:CustomValidator ValidationGroup="validgroup" id="CValidator" CssClass="red" runat="server" ValidateEmptyText="true"  ControlToValidate="txtStartDate" ErrorMessage="Invalid date" ClientValidationFunction="validateDate" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Create User</td>
                                    <td> <asp:Label ID="lblCreateUser"  CssClass="rowLblVal" runat="server" /> </td>
                                </tr>
                                                                
                                <tr style="display:none;">
                                    <td class="rowHdr">Order By</td>
                                    <td><asp:Label ID="lblOrderBy" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                               
                          
                                </tbody>
                            </table>
                            <br />
                        </asp:Panel>
                        <asp:Label ID="Label27" CssClass="red" runat="server" Text="* Required"/>
                        <br />
                        <asp:Button ID="btnSaveSchedule" CssClass="midButton_green" runat="server" Text="Update"  ValidationGroup= "validgroup" />
                        <asp:Button ID="btnDeleteSchedule" CssClass="midButton" runat="server" Text="Delete Master" />

                        <asp:CheckBox  ID="chkEmailMaster" runat ="server" CssClass= "checkbox" text="Send email notification to customer about this change." class="fontsize-1" Visible ="false"/>
                     
                       
                    </asp:Panel>
                    <br />
                    <br /><br />
                    <asp:Panel runat="server" ID="pnlDtlAssignedCust">
                      <h2>Assigned Customers</h2>
                    <asp:Panel runat="server" CssClass="visiblePnl" DefaultButton = "btnSearch">
                      
                          <asp:Label ID="lblSortBy" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Sort by" %>' CssClass="gray" />
                          <asp:DropDownList ID="lstOrderBy_order" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                           <asp:Label ID="lblJFCAccount" runat="server" CssClass="gray" Text='<%# IIf(IsMobile(), "<br />", "") & "JFC Account #"  %>'  />
                            <asp:TextBox ID="txtJfcAccountNum" CssClass="largeTxt size8" runat="server" autocomplete="off" />
                             <asp:Label ID="lblState" runat="server" CssClass="gray" Text='<%# IIf(IsMobile(), "<br />", "") & "State"  %>'  />
                            <asp:TextBox ID="txtState" CssClass="largeTxt size8" runat="server" autocomplete="off" />
                              <asp:Label ID="lblorderFrom" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Delivery Date from" %>' CssClass="gray" />
                            <asp:TextBox ID="txtDeliveryDateFrom" CssClass="largeTxt size14 datepicker" runat="server" autocomplete="off" />
                            <asp:Label ID="lblorderto" runat="server" Text='<%# IIf(IsMobile(), "<br />", "") & "Delivery Date to" %>' CssClass="gray" />
                            <asp:TextBox ID="txtDeliveryDateTo" CssClass="largeTxt size14 datepicker" runat="server" autocomplete="off" />
                            <asp:button runat ="server" ID="btnSearch" CssClass =" midButton" Text = "Search"></asp:button>
                 
                    </asp:Panel>
                      <asp:GridView ID="grdDtlCustDelSchMaster" runat="server" DataSourceID="dsDtlCustDelSchMaster" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" 
                         ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="true" PageSize="50" >
                        <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="<<" LastPageText=">>" Position="Bottom"/>
                        <Columns>
                         <asp:TemplateField HeaderText="Order by" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size8" 
                            HeaderStyle-HorizontalAlign="Left"   HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"  Visible ="false">
                          <ItemTemplate>
                             <asp:label runat="server" ID="lblcustomerID" Text ='<%# Eval("customer_id")%>'    />
                             <asp:label runat="server" ID="lblcustGrpVendorID" Text ='<%# Eval("custGrpVendor_id")%>'  />
                             <asp:label runat="server" ID="lblshipPointID" Text ='<%# Eval("shipPoint_id") %>'  />
                          </ItemTemplate>
                    </asp:TemplateField>

                       <asp:BoundField HeaderText="JFC Account #" DataField="jfcCustomerNr" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size8 cellLbl" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="20px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                       <asp:BoundField HeaderText="Customer Name" DataField="customerName" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="100px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" Visible= "false" />
                         <asp:BoundField HeaderText="Shipping Destination" DataField="shipName" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24 cellLbl" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="100px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                        <asp:BoundField HeaderText="State" DataField="State" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4 cellLbl" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="20px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />

                         <asp:TemplateField HeaderText="Delivery on" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" 
                            HeaderStyle-HorizontalAlign="Left"   HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                          <ItemTemplate>
                            <asp:TextBox runat="server" ID="txtDeliveryOn"  CssClass="stdTxt size12 datepicker" text='<%# Eval("deliveryDate")%>' autocomplete="off" />
                             <ajaxToolkit:FilteredTextBoxExtender ID="FilterTxtExt_DeliveryOn" runat="server" FilterType="Numbers, Custom" ValidChars="/" TargetControlID="txtDeliveryOn" /> 
                             <asp:label runat="server" ID="lblDeliveryOn" Text ='<%# Eval("deliveryDate")%>'  Visible ="false"  />
                          </ItemTemplate>
                    </asp:TemplateField>
                      <asp:TemplateField HeaderText="Order by" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" 
                            HeaderStyle-HorizontalAlign="Left"   HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                          <ItemTemplate>
                            <asp:TextBox runat="server" ID="txtOrderby"  CssClass="stdTxt size12 datepicker" Text = '<%# Eval("orderByDate")%>'  autocomplete="off" />
                             <ajaxToolkit:FilteredTextBoxExtender ID="FilterTxtExt_Orederby" runat="server" FilterType="Numbers, Custom" ValidChars="/" TargetControlID="txtOrderby" /> 
                             <asp:label runat="server" ID="lblOrderby" Text ='<%# Eval("orderByDate")%>'  Visible ="false"  />
                             <asp:DropDownList runat="server" ID="lstOrderTime"  CssClass="drpList size10"   />
                             <asp:label runat="server" ID="lblOrderTime" Text ='<%# Left(COMMON.CString(Eval("orderByTime")),5)%>'  Visible ="false"  />
                             <asp:DropDownList ID="lstOrderByAmPm"   CssClass="drpList size8"  runat="server" />
                             <asp:label runat="server" ID="lblOrderByAmPm" Text ='<%# Right(COMMON.CString(Eval("orderByTime")),2)%>' Visible ="false"  />
                          </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Send Reminder" HeaderStyle-CssClass="th size8" ItemStyle-CssClass="size8" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="30px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                    <ItemTemplate>
                        <asp:DropDownList ID="lstSendReminder"   CssClass="drpList"  runat="server" />
                         <asp:label runat="server" ID="lblSendReminder" Text ='<%# Eval("sendReminder")%>'  Visible ="false"  />
                        
                      
                    </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Email" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="30px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                        <ItemTemplate>
                          <asp:CheckBox ID="ChkSendEmail" text=" Send Email" runat="server" /> 
                         </ItemTemplate>
                     </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" Tooltip="Remove item" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' 
                                     CommandArgument='<%# Eval("customer_id") & "," & Eval("custGrpVendor_id") & "," & Eval("shipPoint_id") & "," & Eval("deliverydate")  %>' runat="server"  />
                                </ItemTemplate>
                     </asp:TemplateField>
                     </Columns>        
                     <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found" />&nbsp;&nbsp;        
                     </EmptyDataTemplate>
                    </asp:GridView> 
                     <asp:SqlDataSource ID="dsDtlCustDelSchMaster" runat="server"></asp:SqlDataSource>
                     <br /><asp:Label ID="Label10" CssClass="red" runat="server" Text="Please update for each page" /><br />
                    <asp:Button ID="btnSaveCustDelSchMaster" CssClass="midButton_green" runat="server" Text="Update"  ValidationGroup="validgroup" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </asp:Panel>
      </div> 
    </div>  
   
</asp:Content>
