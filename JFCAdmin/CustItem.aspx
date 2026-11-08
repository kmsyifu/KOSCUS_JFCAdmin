<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="CustItem.aspx.vb" Inherits="JFCAdmin_CustItem"  %>
<%@ Import Namespace="Common" %>
<%--%@ Register Tagprefix="uc" Tagname="OrderHistory" Src="~/Common/OrderHistory.ascx" --%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/jquery.maskedinput.min.js" type="text/javascript"></script> 
  
   <script type="text/javascript">
        
         function getCustomerName(){
              var id = document.getElementById('<%=txtPopJFCAccount.ClientID %>').value;
              var id2 = document.getElementById('<%=txtPopJFCNum.ClientID %>').value;
              if(id.length == 2 && id2.length == 4)
              {
                    loadData(id,id2,'','','JFCBranch',function(data) {
                    //processing the data
                    document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML = data;
                   
                   });
             }
             else{
                 document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML ='';
             }       
        }

        function handleEnterKey(event) {
            if (event.keyCode === 13) { // Check if Enter key is pressed
                event.preventDefault(); // Prevent default form submission
        
                // Manually trigger the button click or postback
                //document.getElementById('<%= Button1.ClientID %>').click();
                // Or use __doPostBack('<%= Button1.UniqueID %>', '');
            }
        }


        function clickGetItem(txtItem) {
           var enterd = txtItem.val();
          
                var lines = enterd.split(',');
                
                if(lines[2] in window ){
                  
                     
                }
                else{
                    if (lines[2].length >10)
                    {
                       $('#<%=btnGetItem.ClientID%>').click();
                    }
                 }
        }
          function ClientValidate(sender, args) {
           // var v = document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML;
            var id = document.getElementById('<%=txtPopJFCAccount.ClientID %>').value;
            var id2 = document.getElementById('<%=txtPopJFCNum.ClientID %>').value;
            if(id.length == 2 && id2.length == 4)
              {
                   loadData(id,id2,'','','JFCBranch',function(data) {
                    //processing the data
                   document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML = data;
                    
                   if (!data) 
                   {
                    args.IsValid = false;
                    }
                    else{
                  
                    }
                   });
                  
             }
             else{
                 document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML ='';
                 args.IsValid = false;
                 //alert(id2);
             }       
          }
       

        function loadData(id,id2,id3,id4,code,callback)
         {
            
              $.ajax({
				        // try to leverage ajaxQueue plugin to abort previous requests
				        // limit abortion to this input
				        type: 'POST',
				        dataType: "text",
                        url:'../Common/Suggestion.ashx?id='+ id +'&id2=' + id2 +'&id3=' + id3 + '&id4=' + id4 + '&code='+ code +'',
				        data:  '{}' ,
                        async: false,
				        success: function(data) {
                            callback(data);
				        },
                        error: function() {
                            alert('Error occured');
                        }
			        });
         }  
        
        function GetGP(){
            var id = '<%=lblCustomerID.text%>';
            var id2 = '<%=lblPopEditItem.text %>';
            var id3 = document.getElementById('<%=txtPopPrice.ClientID %>').value;
            var id4 = document.getElementById('<%=lstPopPriceUnit.ClientID %>').value;
              if(id3.length > 0)
              {
                   loadData(id,id2,id3,id4,'GETGP',function(data) {
                    //processing the data
                   if (!data) 
                   {
                    
                   }
                    else{
                        document.getElementById('<%=lblPopGP.ClientID%>').innerHTML = data;
                    }
                   });
                 
             }
             else{
                 document.getElementById('<%=lblPopGP.ClientID%>').innerHTML ='';
                 //alert(id2);
             }  
        }
        function TriggerTextBox(event, textBoxId) {
            if (event.keyCode === 13) { // Check if Enter key is pressed
                document.getElementById(textBoxId).focus();
                return false;
            }
        }
        // Buy Again display order: saves the rows in the order they are shown now (used by drag & drop and by the mobile up / down buttons)
        function saveBuyAgainOrder() {
            var items = [];
            $(".drag_drop_grid tr").each(function () {
                var itemId = $(this).find('input[name="item_id"]').val();
                if (itemId) items.push({ item_id: itemId });
            });
            var dataToSend = JSON.stringify({ items: items });
            var id = '<%=lblCustomerID.text%>';
            $("#buyAgainOrderMsg").text("Saving...");

            $.ajax({
                type: 'POST',
                dataType: "text",
                url: '../Common/Suggestion.ashx?id=' + id + '&id2=&id3=&id4=&code=ITEMLIST',
                data: dataToSend,
                success: function (data) {
                    $("#buyAgainOrderMsg").text("Order saved");
                },
                error: function () {
                    $("#buyAgainOrderMsg").text("");
                    alert('Error occurred');
                }
            });
        }

        // Mobile: drag & drop does not work on a touch screen, so each row has up / down buttons.
        // Taps are saved together 1 second after the last one.
        var buyAgainSaveTimer;
        function moveBuyAgainRow(btn, dir) {
            var row = $(btn).closest("tr");
            var other = dir < 0 ? row.prev("tr") : row.next("tr");
            if (other.length === 0 || other.find('input[name="item_id"]').length === 0) return false; // already first / last
            if (dir < 0) row.insertBefore(other); else row.insertAfter(other);
            row.addClass("buyAgainMoved");
            setTimeout(function () { row.removeClass("buyAgainMoved"); }, 600);
            $("#buyAgainOrderMsg").text("");
            clearTimeout(buyAgainSaveTimer);
            buyAgainSaveTimer = setTimeout(saveBuyAgainOrder, 1000);
            return false;
        }

        // Mobile: touch and hold a Buy Again row for 1 second, then drag it up / down; the order is saved when the finger lifts.
        // A quick swipe still scrolls the page (the hold is canceled if the finger moves first).
        function initBuyAgainTouchDrag() {
            var HOLD_MS = 1000, MOVE_CANCEL_PX = 10, EDGE_PX = 60;
            var grid = $(".drag_drop_grid").get(0);
            if (!grid || !("ontouchstart" in window)) return;
            $(grid).addClass("touchDragGrid"); // no text selection / copy menu on a long press
            var holdTimer = null, dragRow = null, startX = 0, startY = 0, lastY = 0, scrollTimer = null;

            function dataRow(el) {
                var row = $(el).closest("tr");
                return row.find('input[name="item_id"]').length ? row : null;
            }
            function cancelHold() { clearTimeout(holdTimer); holdTimer = null; }
            function moveTo(y) {
                // Put the dragged row before / after the row under the finger
                var el = document.elementFromPoint(startX, y);
                var target = el ? dataRow(el) : null;
                if (!target || target[0] === dragRow[0]) return;
                if (dragRow.index() < target.index()) dragRow.insertAfter(target); else dragRow.insertBefore(target);
            }
            function stopDrag() {
                clearInterval(scrollTimer); scrollTimer = null;
                dragRow.removeClass("buyAgainDragging");
                dragRow = null;
                clearTimeout(buyAgainSaveTimer);
                saveBuyAgainOrder();
            }

            grid.addEventListener("touchstart", function (e) {
                if (e.touches.length !== 1 || $(e.target).closest("button, input, select").length) return;
                var row = dataRow(e.target);
                if (!row) return;
                startX = e.touches[0].clientX; startY = lastY = e.touches[0].clientY;
                cancelHold();
                holdTimer = setTimeout(function () {
                    holdTimer = null;
                    dragRow = row.addClass("buyAgainDragging");
                    $("#buyAgainOrderMsg").text("");
                    if (navigator.vibrate) navigator.vibrate(40);
                    // Scroll the page while the finger is near the top / bottom of the screen
                    scrollTimer = setInterval(function () {
                        var step = lastY < EDGE_PX ? -12 : (lastY > window.innerHeight - EDGE_PX ? 12 : 0);
                        if (step !== 0) { window.scrollBy(0, step); moveTo(lastY); }
                    }, 30);
                }, HOLD_MS);
            }, { passive: true });

            grid.addEventListener("touchmove", function (e) {
                var t = e.touches[0];
                if (dragRow) {
                    e.preventDefault(); // drag the row, not the page
                    lastY = t.clientY;
                    moveTo(lastY);
                } else if (holdTimer && (Math.abs(t.clientX - startX) > MOVE_CANCEL_PX || Math.abs(t.clientY - startY) > MOVE_CANCEL_PX)) {
                    cancelHold(); // the user is scrolling
                }
            }, { passive: false });

            function touchEnd(e) {
                cancelHold();
                if (dragRow) {
                    if (e.cancelable) e.preventDefault(); // no click on the item link after a drag
                    stopDrag();
                }
            }
            grid.addEventListener("touchend", touchEnd, { passive: false });
            grid.addEventListener("touchcancel", touchEnd, { passive: false });
            // No copy / link menu from the long press
            grid.addEventListener("contextmenu", function (e) { if (dragRow || holdTimer) e.preventDefault(); });
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
                        saveBuyAgainOrder();
                    }
                });
            }
            initSortableWithDelay();

        }  
        $(document).ready(function () {
            //$("#tabs").tabs();
            $("#emailNoteRow").toggle($("#ckbPopEmail").is(":checked"))
            $("#tabs").tabs();
            $("#tabs").removeClass("ui-widget");
            $('#tabs').css('display', 'block'); //Display tabs when page finish loading
            $(".CODE_ITEM").autocomplete('../Common/Suggestion.ashx?id=<%=lblCustomerID.text%>&id2=&id3=&id4=JFCADMIN&code=ITEM', 
            { minChars: 1, autoFill: false, scroll: false, max: 30}).focus(function(){ clickGetItem($('#<%=txtItem.ClientID%>'));});
            $(".disableEnter").keypress(function(event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                }
            });
            //$('#<%=txtPopPrice.ClientID%>').keypress( function(){GetGP();}).focus(function(){GetGP();});
            //$('<%=lstPopPriceUnit.ClientID %>').change( function(){alert('aa');});
            
           
        
           $(".resetCursor").mouseup(function () {
             this.setSelectionRange(0,0);    
            });

           $('#<%=txtJfcBranchNr.ClientID%>').mask("99");
            $.mask.definitions['h'] = "[A-Za-z0-9]";
           $('#<%=txtJfcCustomerNr.ClientID%>').mask("hhhh");
           applySortable();
           if (<%= IIf(IsMobile(), "true", "false") %>) initBuyAgainTouchDrag();
//           $(".drag_drop_grid").sortable({
//                items: 'tr:not(tr:first-child)',
//                cursor: 'pointer',
//                axis: 'y',
//                dropOnEmpty: false,
//                start: function (e, ui) {
//                ui.item.addClass("selected");
//                },
//                stop: function (e, ui) {
//                ui.item.removeClass("selected");
//                $( '#<%=btnUpdateSortOrder.ClientID%>' ).click();
//               }
//            });

            $("#ckbPopEmail").change(function () {
                    $("#emailNoteRow").toggle($(this).is(":checked"))

            });

                      
          <%= JavaScript %>
        });
    </script>
    <h1>Customer Items&nbsp;&nbsp;</h1>
    <asp:Panel ID="pnlSearchCust"  DefaultButton ="btnSearchCust" CssClass="visiblePnl" runat="server"> 
      <asp:Label ID="lblGetCustCompanyType" runat="server" Text='<%# GetCustCompanyType()%>'  CssClass="gray" />
      <asp:DropDownList ID="lstCustCompany" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' OnChange="javascript:showProgress();" runat="server" AutoPostBack="True"  />
      or<asp:TextBox ID="txtJfcBranchNr" CssClass="largeTxt size4" runat="server"  autocomplete="off"  />
        -<asp:TextBox ID="txtJfcCustomerNr" CssClass="largeTxt size8 resetCursor" runat="server"  autocomplete="off"  />
        <asp:ImageButton ID="btnSearchCust" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server" />
      <asp:Label ID = "lblCustomerID" CssClass="visiblePnl" runat="server" Visible ="false"/>
    </asp:Panel>
    <br />
    
     <asp:Label ID="lblTitle" runat="server"  Text='Please select a customer.'  CssClass=" gray" />
    <div id="Alltab"  runat ="server"  >
    <div id="tabs" style="display: none;"> <%--Display tabs when page finish loading--%> 
        <ul>
            <li><a href="#tabCustItem">Buy Again</a></li>
            <li><a href="#tabPromoItem">Promo Items</a></li>
        </ul>
    
          
        <%----------------------------------------------------------------------------------------------------- m-----------------
            Manage customer items
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel runat="server" ID="pnlTabCustItem" >
            <div id="tabCustItem">
                
                 <%-----Price Inquiry--------%>
                 <h4><asp:Label ID="lblWarning" runat="server" CssClass="red " /></h4>
                 
                 <br /> 
                <h2>Price Inquiry &nbsp;&nbsp;  </h2>
                
                 <asp:RadioButton ID="rdbCurrCust"  GroupName="PriceInquiry"   runat="server" Text=" Current customer" OnChange="javascript:showProgress();"  AutoPostBack ="true" /> &nbsp;
                <asp:RadioButton ID="rdbAllRegCust"  GroupName="PriceInquiry"   runat="server" Text=" All regular customers" OnChange="javascript:showProgress();"   AutoPostBack ="true" />  <%= IIf(IsMobile(), "<br />", "&nbsp;")%>
                <asp:RadioButton ID="rdbAllExpCust"  GroupName="PriceInquiry"   runat="server" Text=" All exception price customers" OnChange="javascript:showProgress();"  AutoPostBack ="true"  /> 
                <br />
                <br />
                <asp:Panel runat="server" ID="PanelPriceInquireList"  onkeydown="handleEnterKey(event)" ><%--DefaultButton = "Button1" "--%>
                    
                    <asp:ImageButton ID ="ImgbtnPriceInquirydelete" runat ="server" CssClass="xsImgBtn"  ImageUrl='<%# GetWebImgUrl("trash_black.png") %>' />
                    <asp:LinkButton ID="lnkPriceInquirydelete" runat="server" CssClass=" cellLink" ForeColor="#4169E1">Delete checked items</asp:LinkButton>
                     <%= IIf(IsMobile(), "<br />", "&nbsp;&nbsp;&nbsp;")%>
                     <asp:ImageButton ID ="ImgbtnPriceInquiryCopy"  runat ="server" CssClass="xsImgBtn "  ImageUrl='<%# GetWebImgUrl("dollarsign.png") %>'/>
                    <asp:linkbutton ID="lnkPriceInquiryCopy"    CssClass=" linkLabel" ForeColor="#4169E1" runat="server" Text="Use reg price for checked items"   />
                    <asp:GridView ID="grdPriceInquire" runat="server" DataSourceID="dsPriceInquire" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" 
                    ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="true" PageSize="25"  OnPageIndexChanging="grdPriceInquire_PageIndexChanging">
                        <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="<<" LastPageText=">>" Position="Bottom"/>
                        <Columns>
                             <asp:TemplateField   HeaderStyle-CssClass="th center4"  ItemStyle-CssClass="center4">
                             <HeaderTemplate  >  
                                         <asp:CheckBox ID="ChkAll"  onclick="javascript:checkAll(this);"  runat="server" /> 
                              </HeaderTemplate>  
                            <ItemTemplate >
                                <asp:CheckBox ID="chkRow" CssClass=" size2" runat="server" />
                            </ItemTemplate>
                          </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer ID" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                                <ItemTemplate>
                                   
                                    <asp:Label  runat="server" ID="lblCustItemID" Text='<%# CString(Eval("custgrp_id")).Insert(2,"-") %>' />
                                    <asp:Label  runat="server" ID="lblcustomerID" Text='<%# Eval("customer_id") %>'  Visible = "false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Name" DataField="customername" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="100px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                            <asp:TemplateField HeaderText="Item ID" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CssClass= "cellLink"  ForeColor="#4169E1" ID="lblItemID" Text='<%# CString(Eval("item_id")) %>' 
                                    CommandName="EDIT_ROW" CommandArgument='<%# Eval("custGrpItem_id") & "," & Eval("customer_id") %>' />
                                    <asp:Label Visible="false" runat="server" ID="lblCustGrpItemID" Text='<%# Eval("custGrpItem_id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                       <asp:BoundField HeaderText="Description" DataField="itemDesc" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="288px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" />
                      
                     <asp:TemplateField HeaderText="Cost" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                    <ItemTemplate>
                     <asp:Label runat="server" ID="lblCost" Text="" />
                        <asp:Label runat="server" ID="lblcsCost" Text='<%# CString(Eval("csCost"))%>' Visible = "false"/>
                        <asp:Label runat="server" ID="lbleaCost" Text='<%# CString(Eval("eaCost")) %>' Visible = "false" />
                         <asp:Label runat="server" ID="lbllbCost" Text='<%# CString(Eval("lbCost"))%>' Visible = "false" />
                        <asp:Label runat="server" ID="lblscaledItem" Text='<%# CString(Eval("scaledItem")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lblminSalCnt" Text='<%# CString(Eval("minSalCnt")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lblavgcswgt" Text='<%# CString(Eval("avgcswgt")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lblavgeawgt" Text='<%# CString(Eval("avgeawgt")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lbleapercs" Text='<%# CString(Eval("eapercs")) %>' Visible = "false" />
                         <asp:Label runat="server" ID="lbllbsperEa" Text='<%# CString(Eval("lbsPerEa")) %>' Visible = "false" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bottom Price" HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblbtmPrice" Text=""  />
                        <asp:Label runat="server" ID="lblbtmcsPrice" Text='<%# CString(Eval("btmcsPrice"))%>' Visible = "false"/>
                    </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Reg Price"  HeaderStyle-CssClass="th number16" ItemStyle-CssClass="number16" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"   >
                    <ItemTemplate >
                     <asp:Label runat="server" ID="lblRegPrice"     />
                        <asp:Label runat="server" ID="lblRegcsPrice"  Text='<%# CString(Eval("RegcsPrice")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lblRegeaPrice" Text='<%# CString(Eval("RegeaPrice")) %>' Visible = "false" />
                        <asp:Label runat="server" ID="lblReglbPrice" Text='<%# CString(Eval("ReglbPrice"))%>' Visible = "false" />
                       </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" 
                        HeaderStyle-HorizontalAlign="Left"   HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" >
                      <ItemTemplate>
                        <asp:TextBox runat="server" ID="txtPopPrice"  CssClass="stdTxt size8"  autocomplete="off" />
                        <ajaxToolkit:FilteredTextBoxExtender  ID="FilteredTextBoxExtender1" runat="server" 
                             FilterType="Numbers, Custom" ValidChars="." 
                             TargetControlID="txtPopPrice" />
                         <asp:DropDownList ID="lstPriceUnit"   CssClass="drpList" runat="server" />
                      </ItemTemplate>
                    </asp:TemplateField>
 
                    <asp:BoundField HeaderText="Create Date" DataField="CreateDate_FMT" HeaderStyle-CssClass="th size64" ItemStyle-CssClass="size64 cellLbl gray" 
                             HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="88px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                              ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px"   /> 
                    
                    <asp:TemplateField HeaderText="Avail" HeaderStyle-CssClass="th number10" ItemStyle-CssClass="number10" 
                        HeaderStyle-HorizontalAlign="Right" HeaderStyle-Width="60px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Height="30px" Visible = "false">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCsAvail" Text='<%# CString(Eval("csAvail")) & "<br />" %>' />
                        <asp:Label runat="server" ID="lblEaAvail" Text='<%# CString(Eval("eaAvail")) & "<br />" %>' />
                    </ItemTemplate>
                    </asp:TemplateField>
           
                    <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size4" ItemStyle-CssClass="size4" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="24px" HeaderStyle-ForeColor="#FF6600" HeaderStyle-Height="30px"
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Height="30px" Visible = "false">
                       <ItemTemplate>
                            <asp:Label runat="server" ID="lblCs" Text="cs" /><br />
                             <asp:Label runat="server" ID="lblEa" Text="ea" />
                       </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" Tooltip="Remove item" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' CommandArgument='<%# Eval("custGrpItem_id")%>' runat="server"  />
                                </ItemTemplate>
                     </asp:TemplateField>
                     </Columns>        
                        <EmptyDataTemplate>
                            
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found" Visible ="false"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsPriceInquire" runat="server"></asp:SqlDataSource>
                    <br /> <asp:Label ID="lblPriceInquireMsg" CssClass="red" runat="server" Text="Please add items for each page" /><br />
                      <asp:Button ID="btnAddPriceInquire" OnClientClick="javascript:showProgress();" CssClass="midButton" runat="server" Text="Add Items & Send Email"  />
                      <asp:Button ID="Button1" Visible ="false" CssClass="midButton" runat="server" Text=""  />
                </asp:Panel>
                <br />
                <br />
                <asp:HiddenField ID="lblTextChanged_custItem" runat="server" /> <%--Can be access from both server and client side--%>
                <h2>Buy Again &nbsp;&nbsp; </h2>
                <style type="text/css">
                    .buyAgainMove { display: inline-block; width: 34px; height: 30px; margin: 1px 0; padding: 0; border: 1px solid #C8C8C8; border-radius: 4px;
                        background: #FFFFFF; color: #E78F08; font-size: 14px; line-height: 28px; text-align: center; cursor: pointer; }
                    .buyAgainMove:active { background: #FDE9C6; }
                    tr.buyAgainMoved td { background: #FDE9C6 !important; }
                    tr.buyAgainDragging td { background: #F9B84A !important; color: #FFFFFF; }
                    .touchDragGrid tr { -webkit-touch-callout: none; -webkit-user-select: none; user-select: none; }
                    #buyAgainOrderMsg { margin-left: 8px; color: #2E7D32; font-weight: bold; }
                </style>
                <asp:PlaceHolder runat="server" Visible='<%# IsMobile() %>'>
                    <span class="gray">Change display order: touch and hold a row for 1 second, then drag it; or tap &#9650; / &#9660;.</span><span id="buyAgainOrderMsg"></span>
                    <br />
                </asp:PlaceHolder>
                
                   
                    <asp:Panel  runat="server" Visible='<%# Not IsMobile() %>'>
                        <asp:Label ID="Label1" runat="server" CssClass=" midLabel" Text="Change Display Order:&nbsp;&nbsp;&nbsp;(1) " /> 
                        <asp:linkbutton ID="btnExportExcel"    CssClass=" linkLabel" ForeColor="#4169E1" runat="server" Text="Export"  UseSubmitBehavior="false" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label2" runat="server" CssClass=" midLabel" Text="&nbsp;&nbsp;&nbsp;(2) Change order in excel file &nbsp;&nbsp;&nbsp;(3) Select file " />
                       <asp:FileUpload   ID="fileUpExcel"
                                            AllowedFileTypes="xls"
                                            MaximumNumberOfFiles= "1"
                                            UseAbsoluteHandlerPath = "false" 
                                            
                                            runat="server"  /> 
                                            
                       <asp:Label ID="Label3" runat="server" CssClass=" midLabel" Text="&nbsp;&nbsp;&nbsp;(4) " />
                        <asp:linkbutton ID="btnImport" ForeColor="#4169E1" ValidationGroup = "Homepageupload" CssClass="linkLabel" runat="server" Text="Import"  />
                         &nbsp;&nbsp;&nbsp;
                        <asp:RegularExpressionValidator ID="uplValidator" runat="server" ControlToValidate="fileUpExcel" ForeColor="Red" 
                            ErrorMessage="format has to be .xls" ValidationGroup = "Homepageupload" ValidationExpression="(.+\.xls)">
                        </asp:RegularExpressionValidator>
                     
                      </asp:Panel> 
                    <br />
                    <br />
                  
  
                <%--Search item--%>
           
                <asp:Panel ID="pnlSearchItem" CssClass='<%# "visiblePnl " & IIf(IsMobile(), "", "leftContent") %>' DefaultButton="btnSearch_custItem" runat="server">
                    <h1>Search Items</h1>
                    <asp:Label ID="Label18" runat="server" Text="Sort by" CssClass="gray" />
                    <asp:DropDownList ID="lstOrderBy_custItem" CssClass="largeDrpList" OnChange="javascript:showProgress();" runat="server" AutoPostBack="True" /><asp:Label ID="lblPrevOrderBy_custItem" Visible="false" runat="server" />
                    <asp:Panel runat="server" ID="Panel2" Visible ="false" >
                        <asp:Label ID="Label16" runat="server" Text='<%# GetCustCompanyType() & " Group" %>' CssClass="gray" />
                        <asp:DropDownList ID="lstCustGrp_custItem" CssClass="largeDrpList" runat="server" AutoPostBack="True" OnChange="javascript:showProgress();" /><asp:Label ID="lblPrevCustGrp_custItem" Visible="false" runat="server" />
                        <asp:Label ID="Label17" runat="server" Text='<%# GetCustCompanyType() %>' CssClass="gray" />
                        <asp:DropDownList ID="lstCust_custItem" CssClass="largeDrpList" runat="server" AutoPostBack="True" OnChange="javascript:showProgress();" /><asp:Label ID="lblPrevCust_custItem" runat="server" />
                        <asp:Label ID="Label45" runat="server" Text="Buy Again" CssClass="gray" />
                        <asp:DropDownList ID="lstItemGrpSearch_custItem" CssClass="largeDrpList" runat="server" AutoPostBack="True"  OnChange="javascript:showProgress();"/>
                        <asp:Label ID="Label47" runat="server" Text="Vendor" CssClass="gray" />
                        <asp:DropDownList ID="lstVendorSearch_custItem" CssClass="largeDrpList" runat="server" AutoPostBack="True"  OnChange="javascript:showProgress();"/>
                    </asp:Panel>
                    <asp:Label ID="Label46" runat="server" Text="Category" CssClass="gray" />
                    <asp:DropDownList ID="lstCtgSearch_custItem" CssClass="largeDrpList" runat="server" AutoPostBack="True"  OnChange="javascript:showProgress();" />
                   <%-- <asp:Label runat="server" Text="Order Form" CssClass="gray" />
                        <asp:DropDownList ID="lstOrderForm" CssClass="largeDrpList"
                                          runat="server" AutoPostBack="True"
                                          OnSelectedIndexChanged="lstOrderForm_SelectedIndexChanged"
                                          OnChange="javascript:showProgress();" />
                        <asp:ImageButton ID="btnEditOrderForm" runat="server" Visible="true"
                                         CssClass="xsImgBtn" ToolTip="Manage order forms"
                                         ImageUrl='<%# GetWebImgUrl("pencil.png") %>'
                                         OnClick="btnEditOrderForm_Click" />--%>
                     <br />
                    <asp:TextBox ID="txtItemSearch_custItem" CssClass="largeTxt size48" runat="server"  autocomplete="off" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="watItemSearch_custItem" runat="server" TargetControlID="txtItemSearch_custItem" WatermarkText="Search by item" WatermarkCssClass='watermark largeTxt size48' />
                    <asp:ImageButton ID="btnSearch_custItem" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server" />
                    <br />
                </asp:Panel>
                <%--= IIf(IsMobile(), "<br class='turnOffFloat' />", "")--%>
                <%-- ── Inline editor panel ── --%>
             <%-- Hidden trigger button (same pattern as btnPopupTmp_UserDtl) --%>
         <%--   <asp:LinkButton ID="btnPopupTmp_OrderForm" runat="server" />

            <ajaxToolKit:ModalPopupExtender ID="popDialog_OrderForm" runat="server"
                BehaviorID="popDialog_OrderForm"
                TargetControlID="btnPopupTmp_OrderForm"
                PopupControlID="pnlPopOrderForm"
                BackgroundCssClass="modalBackground">
                <Animations>
                    <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                    <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                </Animations>
            </ajaxToolKit:ModalPopupExtender>--%>

          <%--  <asp:Panel ID="pnlPopOrderForm" style="display:none;"
                       runat="server" CssClass="popup_dialog">
                <div class="inner">

                    <asp:ImageButton ID="btnPopCloseOrderForm" CssClass="popClose"
                                     CausesValidation="False" runat="server"
                                     ImageUrl='<%# GetWebImgUrl("close.jpg") %>'
                                     OnClick="btnPopCloseOrderForm_Click" />

                    <h1>Manage Order Forms</h1>
                    <br />

                    <asp:Label ID="lblPopOFMsg" CssClass="red" runat="server" /><br />

                    <%-- All order forms in a grid — edit / delete each row --%>
                  <%--  <asp:GridView ID="grdOF" runat="server"
                                  CssClass="STDTABLE" AutoGenerateColumns="false"
                                  GridLines="Horizontal" ShowHeaderWhenEmpty="true"
                                  DataKeyNames="orderForm_id"
                                  OnRowEditing="grdOF_RowEditing"
                                  OnRowUpdating="grdOF_RowUpdating"
                                  OnRowCancelingEdit="grdOF_RowCancelingEdit"
                                  OnRowDeleting="grdOF_RowDeleting">
                        <Columns>

                            <asp:TemplateField HeaderText="Order Form Name"
                                               HeaderStyle-CssClass="th size48"
                                               ItemStyle-CssClass="size48">
                                <ItemTemplate>
                                    <asp:Label runat="server"
                                               Text='<%# Eval("orderFormName") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditOFName" runat="server"
                                                 CssClass="stdTxt size32 disableEnter"
                                                 Text='<%# Eval("orderFormName") %>'
                                                 autocomplete="off" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-CssClass="th size12"
                                               ItemStyle-CssClass="size12">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CssClass="cellLink"
                                                    ForeColor="#4169E1"
                                                    CommandName="Edit" Text="Edit" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:LinkButton runat="server" CssClass="cellLink"
                                                    ForeColor="green"
                                                    CommandName="Update" Text="Save" />
                                    &nbsp;
                                    <asp:LinkButton runat="server" CssClass="cellLink"
                                                    ForeColor="#999"
                                                    CommandName="Cancel" Text="Cancel"
                                                    CausesValidation="false" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-CssClass="th center4"
                                               ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CssClass="xxsImgBtn"
                                                     CommandName="Delete"
                                                     ToolTip="Delete"
                                                     ImageUrl='<%# GetWebImgUrl("close.jpg") %>'
                                                     OnClientClick="return confirm('Delete this order form and all its items?');" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label runat="server" CssClass="gray"
                                       Text="No order forms yet." />
                        </EmptyDataTemplate>
                    </asp:GridView>--%>

                   

                    <%-- Add new --%>
                   <%-- <table>
                        <tr>
                            <td class="rowHdr">New Name</td>
                            <td>
                                <asp:TextBox ID="txtNewOFName" runat="server"
                                             CssClass="stdTxt size32 disableEnter"
                                             autocomplete="off" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="center">
                                <asp:Button ID="btnAddOF" runat="server"
                                            CssClass="midButton_green" Text="Add"
                                            OnClick="btnAddOF_Click"
                                            OnClientClick="javascript:showProgress();" />
                            </td>
                        </tr>
                    </table>--%>
        <%--            <br />

                </div>
            </asp:Panel>--%>

                <%--Add item--%>
                <asp:Panel ID="pnlNewItem" CssClass="visiblePnl" style='<%# IIf(IsMobile(), "", "margin-left:533px") %>' runat="server">
                    <h1>Add / Update Item</h1>
                        <asp:Panel ID="pnlGetItem" DefaultButton="btnGetItem" runat="server">
                            <asp:TextBox ID="txtItem" CssClass='<%# "stdTxt CODE_ITEM " & IIf(IsMobile(), "size32", "size72") %>'  runat="server"  />
                            <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtItem" WatermarkText="Search by item # or keyword" WatermarkCssClass='<%# "watermark stdTxt " & IIf(IsMobile(), "size32", "size72") %>' />
                            <asp:ImageButton ID="btnClearItem" Tooltip="Clear" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' runat="server" /><%--clearText.png--%>
                            <asp:Button ID="btnGetItem" OnClientClick="javascript:showProgress();" CssClass="midButton" runat="server" Text="Get Item" style="display: none;" />
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
                                    <asp:TextBox runat="server" ID="txtNewCsQty" onFocus="this.select()" CssClass="cellQtyTxt"  autocomplete="off" /><%--TextMode="Number"--%>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty" />
                                    <asp:Label runat="server" ID="lblNewCsQty" Text='cs' CssClass="cellLbl" />&nbsp;&nbsp;
                                    <asp:TextBox runat="server" ID="txtNewEaQty" onFocus="this.select()" CssClass="cellQtyTxt"  autocomplete="off" /><%--TextMode="Number"--%>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Numbers" TargetControlID="txtNewEaQty" />
                                    <asp:Label runat="server" ID="lblNewEaQty" Text='ea' CssClass="cellLbl" />&nbsp;&nbsp;
                                    <asp:TextBox runat="server" ID="txtNewLbs" onFocus="this.select()" CssClass="cellQtyTxt"  autocomplete="off" /><%--TextMode="Number"--%>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtNewLbs" />
                                    <asp:Label runat="server" ID="lblNewLbs" Text='lbs' CssClass="cellLbl" />&nbsp;&nbsp;
                                    <asp:LinkButton runat="server" CssClass="cellLink" ID="lnkNewShowVpSp" Text="vp/sp"  />&nbsp;&nbsp;
                                    <asp:Panel ID="pnlNewVpSp" runat="server" style="display:inline;"> 
                                        <br />
                                        <asp:TextBox runat="server" ID="txtNewCsQty_vp" onFocus="this.select()" CssClass="cellQtyTxt green"  autocomplete="off" /><%--TextMode="Number"--%>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty_vp" />
                                        <asp:Label runat="server" ID="lblNewCsQty_vp" Text='vp' CssClass="cellLbl green" />&nbsp;&nbsp;
                                        <asp:TextBox runat="server" ID="txtNewEaQty_vp" onFocus="this.select()" CssClass="cellQtyTxt green"  autocomplete="off" /><%--TextMode="Number"--%>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" FilterType="Numbers" TargetControlID="txtNewEaQty_vp" />
                                        <asp:Label runat="server" ID="lblNewEaQty_vp" Text='vp' CssClass="cellLbl green" />&nbsp;&nbsp;
                                        <asp:TextBox runat="server" ID="txtNewVpMemo" onFocus="this.select()" CssClass="stdTxt size24 green"  autocomplete="off" />
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtNewVpMemo" WatermarkText="VP memo" WatermarkCssClass="watermark stdTxt size24" />&nbsp;&nbsp;
                                        <br />
                                        <asp:TextBox runat="server" ID="txtNewCsQty_sp" onFocus="this.select()" CssClass="cellQtyTxt green"  autocomplete="off" /><%--TextMode="Number"--%>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" runat="server" FilterType="Numbers" TargetControlID="txtNewCsQty_sp" />
                                        <asp:Label runat="server" ID="lblNewCsQty_sp" Text='sp' CssClass="cellLbl green" />&nbsp;&nbsp;
                                        <asp:TextBox runat="server" ID="txtNewEaQty_sp" onFocus="this.select()" CssClass="cellQtyTxt green"   autocomplete="off" /><%--TextMode="Number"--%>
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
                <br />

                        
                <%-- Item List--%>
                <%--<asp:Image ID="Image3" CssClass="xsImgBtn" runat="server" ImageUrl='<%# GetWebImgUrl("info.jpg") %>' />&nbsp;<asp:Label ID="Label23" CssClass="gray" runat="server" Text='<%# "Only items that belong to the " & CString(GetCustCompanyType()).ToLower & " group can be added here. If text box is available, please enter price only if different from the customer group value in gray." %>' /><br />--%>
                    <asp:Panel runat="server" ID="PanelItemList"><%--DefaultButton="btnSaveList_custItem"--%>
                     <asp:ImageButton ID ="btnCopyprice" runat ="server" CssClass="xsImgBtn"  ImageUrl='<%# GetWebImgUrl("copyprice.png") %>' />
                     <asp:linkbutton ID="lnkCopyPrice"    CssClass=" linkLabel" ForeColor="#4169E1" runat="server" Text='<%# IIf(IsMobile(), "Copy selected items", "Copy selected items to another customer")  %> '    />
                     
                     <%= IIf(IsMobile(), "<br />", "&nbsp;&nbsp;&nbsp;")%>
                     <asp:ImageButton ID ="btnCopy"  runat ="server" CssClass="xsImgBtn"  ImageUrl='<%# GetWebImgUrl("copy.png") %>'/>
                    <asp:linkbutton ID="lnkCopy"    CssClass=" linkLabel" ForeColor="#4169E1" runat="server" Text='<%# IIf(IsMobile(), "Copy selected items (all info)", "Copy selected items to another customer (including customer item ID, desc, remark) ")  %> '   />
                    
                   
                    <asp:LinkButton ID="btnPopupTmp_UserDtl"  runat="server" />
                        <ajaxToolKit:ModalPopupExtender ID="popDialog_User" runat="server"
                                    BehaviorID="popDialog_User" 
                                    TargetControlID="btnPopupTmp_UserDtl"
                                    PopupControlID="pnlPopUserDtl"
                                    BackgroundCssClass="modalBackground" >
                            <Animations>
                                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                            </Animations>
                        </ajaxToolKit:ModalPopupExtender>
                        <asp:Panel ID="pnlPopUserDtl" DefaultButton="btnPopSaveUser" style="display:none;" runat="server" CssClass="popup_dialog">
                            <div class="inner">
                                <asp:ImageButton ID="btnPopClose_User" CssClass="popClose"  CausesValidation="False" runat="server" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' />
                                <h1><asp:Label ID="lblPopTitleUser" runat="server" Text="Copy all info to New Customer"/> </h1>
                                <asp:Panel runat="server" ID ="pnlPopWarningUser" Visible ="false">
                                <br />
                                 <asp:Label ID="lblPopWarningUser" CssClass=" red" runat="server" />
                                 <%-- asp:RequiredFieldValidator ID="RFValidatorJFCAccount"  runat="server" CssClass="red" ValidationGroup="validgroup"  
                                             controltovalidate="txtPopJFCNum" errormessage="Please enter a JFC account number!" />--%>
                                <br />
                                <asp:CustomValidator id="CVPopJFCNum" ControlToValidate="txtPopJFCNum" ClientValidationFunction="ClientValidate"
                                            Display="Static"
                                            ErrorMessage="Invalid. Please enter a correct 4 digit account number."
                                            ForeColor="red"
                                            ont-Name="verdana" ValidationGroup="validgroupUser"   
                                            Font-Size="10pt"
                                            runat="server"/>
                                 
                                </asp:Panel>
                                <br />
                                <asp:Panel runat="server" ID="pnlPopDtlEdit_User">              
                                    <table>
                                        <tr>
                                            <td class="rowHdr">JFC Account #</td>
                                            <td> 
                                                <asp:TextBox ID="txtPopJFCAccount" CssClass='stdTxt size4' runat="server" ValidationGroup ="validgroupUser" onchange="javascript:getCustomerName(); "  autocomplete="off" /> -
					                            <asp:TextBox ID="txtPopJFCNum" autocomplete="off" CssClass = '<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>'  ValidationGroup ="validgroupUser" runat="server" onchange="javascript:getCustomerName(); "  />
                                            </td>
                                        </tr>
                                        <tr>
                                        <td></td>
                                        <td></td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr">Customer Name</td>
                                             <td >
                                                <asp:Label ID="lblPopCustomerName" CssClass=" cellLbl bold" runat="server" />
                                 
                                            </td>
                                        </tr>
                                                      
                                        <tr>
                                            <td colspan="2" class="center">
                                                <asp:Button ID="btnPopSaveUser" CssClass="midButton_green" runat="server" Text="Copy"   ValidationGroup="validgroupUser" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                            </div>
                        </asp:Panel>
                   
                    <asp:GridView ID="grdCustItem" runat="server" DataSourceID="dsCustItem" CssClass="drag_drop_grid  STDTABLE" AutoGenerateColumns="false" 
                    GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"  >
                        <PagerStyle CssClass="pgtr" HorizontalAlign="left" ForeColor="#4169E1"  />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="<<" LastPageText=">>" Position="Bottom"/>
                        <Columns>
                        <asp:TemplateField   HeaderStyle-CssClass="th center4"  ItemStyle-CssClass="center4" >
                             <HeaderTemplate  >  
                                         <asp:CheckBox ID="ChkAll"  onclick="javascript:checkAll(this);"  runat="server" /> 
                              </HeaderTemplate>  
                            <ItemTemplate >
                                <asp:CheckBox CssClass=" size2" ID="chkRow" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>

                        <asp:TemplateField HeaderText="" HeaderStyle-CssClass="th size1"  ItemStyle-CssClass="size1" >
                        <ItemTemplate>
                            <input type="hidden" name="item_id"  value='<%# Eval("custGrpItem_id") %>'  />
                            <asp:PlaceHolder runat="server" Visible='<%# IsMobile() %>'><%--Mobile: move the row up / down (drag & drop needs a mouse)--%>
                                <button type="button" class="buyAgainMove" title="Move up" onclick="return moveBuyAgainRow(this, -1);">&#9650;</button><br />
                                <button type="button" class="buyAgainMove" title="Move down" onclick="return moveBuyAgainRow(this, 1);">&#9660;</button>
                            </asp:PlaceHolder>
                        </ItemTemplate>

                        </asp:TemplateField>
                        
                            <asp:TemplateField HeaderText="Item ID" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="lblItemID" Text='<%# Eval("item_id") %>' />--%>
                                    <asp:LinkButton runat="server" CssClass= "cellLink"  ForeColor="#4169E1" ID="lblItemID" Text='<%# IIf(CString(Eval("item_id_n")) ="", CString(Eval("item_id")),CString(Eval("item_id_n"))) %>' CommandName="EDIT_ROW" CommandArgument='<%# Eval("custGrpItem_id")%>' />
                                    <asp:Label runat="server" ID="lblCustNewItemID" Visible='<%# IIf(CString(Eval("item_id_n")) ="", false, true) %>' Text='<%# "(" & CString(Eval("item_id")) & ")" %>' ></asp:Label>
                                    <asp:Label runat="server" ID="lblItemIdN" Visible="false" Text='<%# "(" & CString(Eval("item_id_n")) & ")" %>' ></asp:Label>
                                    <asp:Label Visible="false" runat="server" ID="lblCustGrpItemID" Text='<%# Eval("custGrpItem_id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--'<%# iif(Eval("item_id") <> "","cellLink grey","cellLink") %>>' <asp:BoundField HeaderText="Item ID" DataField="ITEM_ID" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" />--%>
                            <%--<asp:BoundField HeaderText="Cust Item #" DataField="ITEMNR" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" />--%>
                            <%--<asp:BoundField HeaderText="Order Form" DataField="ITEMGRPNAME" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />--%>  
                                <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblItemDesc" CssClass= "cellLbl " Text='<%# IIf(CString(Eval("itemDesc_view_n")) ="", CString(Eval("itemDesc_view")),CString(Eval("itemDesc_view_n")) ) %>' />
                                     <asp:Label runat="server" ID="lblItemDescO" CssClass= "cellLbl " Visible='<%# IIf(CString(Eval("itemDesc_view_n")) ="", false, true) %>' Text=' <%# "(" & Eval("itemDesc_view") & ")" %> ' />
                                      <asp:Label runat="server" ID="lblItemDescN" CssClass= "cellLbl " Visible="false"  Text=' <%#  Eval("itemDesc_view_n")  %> ' />
                                    <asp:Label runat="server" ID="lblJfcRemark" CssClass="cellLbl green" Text='<%# "<br />" & CString(Eval("jfcRemark")) %>' Visible='<%# CString(Eval("jfcRemark")) <> "" %>' />
                                   
                                </ItemTemplate>
                            </asp:TemplateField>
                                          
                            <asp:BoundField HeaderText="Category" DataField="ctgName_view" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24"   />                
                            <%--<asp:TemplateField HeaderText="Category" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCtgName" Text='<%# Eval("ctgName_view") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <%--<asp:BoundField HeaderText="Vendor" DataField="VENDORNAME" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />--%>
                            <%--<asp:TemplateField HeaderText="JFC Description" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" >
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkItemDesc" Text='<%# Eval("itemDesc") %>' CommandName="EDIT_ROW" CommandArgument='<%# Eval("custGrpItem_id")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <%--<asp:BoundField HeaderText="Customer Description" DataField="CUSTITEMDESC" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size48" />--%>
                            <%--<asp:BoundField HeaderText="Description" DataField="itemDesc_view" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />--%>
                            
                            <%--<asp:TemplateField HeaderText="Customer Description<br />JFC Description (In Italic)" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblItemDesc" Text='<%# Eval("itemDesc_view") %>' />
                                    <asp:Label runat="server" ID="lblCustItemDesc" Text='<%# Eval("custItemDesc") %>' />
                                    <asp:Label runat="server" ID="lblNewline2" Text="<br />" Visible='<%# CString(Eval("custItemDesc")) <> "" %>' />
                                    <asp:Label runat="server" CssClass="italic" ID="lblItemDesc" Text='<%# Eval("itemDesc") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Ea/Cs" HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6">
                                <ItemTemplate>
                                    <%--<asp:TextBox runat="server" ID="txtEaPerCs" CssClass="stdTxt number6" Text='<%# IIf(Val(Eval("defEaPerCs")) = Val(Eval("eaPerCs")), "", Eval("eaPerCs", "{0}")) %>' Visible='<%# Val(Eval("defEaPerCs")) <> 0 and Val(Eval("defEaPerCs")) <> 1 %>' />
                                    <asp:Label runat="server" ID="lblDefEaPerCs" CssClass="number6 gray" Text='<%# Eval("defEaPerCs") %>' Visible='<%# Val(Eval("defEaPerCs")) <> 0 and Val(Eval("defEaPerCs")) <> 1 %>' />--%>
                                    <asp:Label runat="server" ID="lblEaPerCs" CssClass="number6" Text='<%# Eval("eaPerCs") %>' Visible='<%# Val(Eval("eaPerCs")) <> 0 and Val(Eval("eaPerCs")) <> 1 and Val(Eval("lbPrice_OrderByQty")) = 0 %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Case Price" HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCsPrice" CssClass="number6 "  Text='<%# IIf(Val(Eval("csprice_n")) <> 0, Eval("csPrice_n", "{0:N}"),Eval("csPrice", "{0:N}")) %>'  />
                                    
                                    <%--<asp:TextBox runat="server" ID="txtCsPrice" CssClass="stdTxt number6" Text='<%# Eval("csPrice", "{0:N}") %>' Visible='<%# Val(Eval("csprice")) <> 0 %>' />--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Each Price" HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEaPrice" CssClass="number6 "  Text='<%# IIf(Val(Eval("eaPrice_n")) <> 0, Eval("eaPrice_n", "{0:N}"),Eval("eaPrice", "{0:N}")) %>'   />
                                    <%--<asp:TextBox runat="server" ID="txtEaPrice" CssClass="stdTxt number6" Text='<%# Eval("eaPrice", "{0:N}") %>' Visible='<%# Val(Eval("eaprice")) <> 0 %>' />--%>
                                    <%--<asp:Label runat="server" ID="lblMinSalCnt_view" CssClass="gray" Visible='<%# Val(Eval("minSalCnt")) > 1%>' Text='<%# "<br />min " & CString(Eval("minSalCnt")) & " ea" %>' />--%> <%--and Eval("defEaprice") <> "0"--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Lb Price" HeaderStyle-CssClass="th number6" ItemStyle-CssClass="number6">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblLbPrice_orderByWgt" CssClass="number6" Text='<%# Eval("lbPrice_orderByWgt", "{0:N}") %>' Visible='<%# Val(Eval("lbPrice_orderByWgt")) <> 0 %>' />
                                    <%--<asp:Label runat="server" ID="lblLbPrice_orderByQty" CssClass='<%# "number6 " & IIf(Val(Eval("lbPrice_orderByQty_n"))=0, "", "red") %>' Text='<%# Eval("lbPrice_orderByQty", "{0:N}") %>' Visible='<%# Val(Eval("lbPrice_orderByQty")) <> 0 %>' />--%>
                                     <asp:Label runat="server" ID="lblLbPrice_orderByQty" CssClass="number6 " Text='<%# IIf(Val(Eval("lbPrice_orderByQty_n")) <> 0, Eval("lbPrice_orderByQty_n", "{0:N}"),Eval("lbPrice_orderByQty", "{0:N}")) %>'   />
                                     <asp:Label runat="server" ID="lblenable" Visible ="false"  CssClass="number6" Text='<%# Eval("enabled") %>' />
                                    <%--<asp:TextBox runat="server" ID="txtLbPrice_orderByWgt" CssClass="stdTxt number6" Text='<%# Eval("lbPrice_orderByWgt", "{0:N}") %>' Visible='<%# Val(Eval("lbPrice_orderByWgt")) <> 0 %>' />
                                    <asp:TextBox runat="server" ID="txtLbPrice_orderByQty" CssClass="stdTxt number6" Text='<%# Eval("lbPrice_orderByQty", "{0:N}") %>' Visible='<%# Val(Eval("lbPrice_orderByQty")) <> 0 %>' />--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" Tooltip="Remove item" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' CommandArgument='<%# Eval("custGrpItem_id")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                        <EmptyDataTemplate>
                            <%--<asp:Image ID="imgNotFound" CssClass="cellSImg" runat="server" ImageUrl='<%# GetWebImgUrl("warning.png") %>' />&nbsp;&nbsp;--%>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsCustItem" runat="server"></asp:SqlDataSource>
                    
                    <asp:Button ID="btnUpdateSortOrder" runat="server" Text="" OnClick="btnUpdateSortOrder_Click"  style="display: none;" />
                   
                     
                        <br />
                      

                    <%--Popup window to Item detail--%>
                    <asp:LinkButton ID="btnPopupTmp_ItemDtl"  runat="server" />
                    <ajaxToolKit:ModalPopupExtender ID="popDialog_Item" runat="server"
                                    BehaviorID="popDialog_Item" 
                                    TargetControlID="btnPopupTmp_ItemDtl"
                                    PopupControlID="pnlPopItemDtl"
                                    BackgroundCssClass="modalBackground" >
                            <Animations>
                                <OnShown><FadeIn Duration="0.5" Fps="25" /></OnShown>
                                <OnHiding><FadeOut Duration="0.3" Fps="25" /></OnHiding>
                            </Animations>
                        </ajaxToolKit:ModalPopupExtender>
                    <%--Item Info popup. Same controls as before (IDs, server code unchanged); new layout.--%>
                    <style type="text/css">
                        .iip.popup_dialog { width: 560px; max-width: 94vw; padding: 0; border: 1px solid #E0E0E0; border-radius: 8px; overflow: hidden;
                            box-shadow: 0 16px 40px rgba(0, 0, 0, 0.28); font-family: Arial, Helvetica, sans-serif; background: #FFFFFF; text-align: left; }
                        .iip .iip-header { position: relative; padding: 14px 48px 14px 20px; background: #F6A828; background-image: linear-gradient(to bottom, #F9B84A, #F09618);
                            border-bottom: 1px solid #E78F08; }
                        .iip .iip-header h1 { margin: 0; padding: 0; border: none; font-size: 1.25em; font-weight: bold; color: #FFFFFF; text-align: left; }
                        .iip .iip-close { position: absolute; top: 50%; right: 12px; transform: translateY(-50%); width: 30px; height: 30px; padding: 0;
                            border: none; border-radius: 15px; background: transparent; color: #FFFFFF; font-size: 22px; line-height: 30px; cursor: pointer; }
                        .iip .iip-close:hover { background: rgba(255, 255, 255, 0.25); }
                        .iip .iip-body { max-height: calc(100vh - 170px); overflow-y: auto; -webkit-overflow-scrolling: touch; padding: 16px 20px 4px 20px; }
                        .iip .iip-alert { margin: 0 0 14px 0; padding: 10px 12px; border: 1px solid #E0E0E0; border-left: 4px solid #F09618; border-radius: 4px;
                            background: #FAFAFA; font-weight: bold; line-height: 1.4; }
                        .iip .iip-summary { margin: 0 0 16px 0; padding: 12px 14px; border: 1px solid #EEEEEE; border-radius: 6px; background: #FAFAFA; }
                        .iip .iip-itemno { display: inline-block; margin: 0 0 6px 0; padding: 2px 8px; border-radius: 4px; background: #FDE9C6; color: #9A5A00;
                            font-size: 0.9em; font-weight: bold; }
                        .iip .iip-desc { display: block; color: #222222; font-size: 1.05em; font-weight: bold; line-height: 1.35; overflow-wrap: break-word; }
                        .iip .iip-stats { margin: 10px 0 0 0; }
                        .iip .iip-stat { display: inline-block; margin: 0 16px 4px 0; color: #777777; font-size: 0.85em; vertical-align: top; }
                        .iip .iip-stat b { color: #333333; font-size: 1.1em; }
                        .iip .iip-section { margin: 0 0 8px 0; padding: 0 0 4px 0; border-bottom: 1px solid #EEEEEE; color: #E78F08; font-size: 0.8em;
                            font-weight: bold; letter-spacing: 0.04em; text-transform: uppercase; }
                        .iip .iip-prices { display: table; width: 100%; margin: 0 0 8px 0; border-collapse: separate; border-spacing: 0 6px; }
                        .iip .iip-price { display: table-row; }
                        .iip .iip-price .iip-k { display: table-cell; width: 120px; padding: 0 10px 0 0; color: #777777; font-size: 0.9em; vertical-align: top; }
                        .iip .iip-price .iip-v { display: table-cell; color: #222222; font-weight: bold; vertical-align: top; overflow-wrap: break-word; }
                        .iip .iip-field { margin: 0 0 14px 0; }
                        .iip .iip-label { display: block; margin: 0 0 5px 0; color: #555555; font-size: 0.85em; font-weight: bold; }
                        .iip .iip-input { box-sizing: border-box; height: 38px; margin: 0; padding: 6px 10px; border: 1px solid #C8C8C8; border-radius: 4px;
                            font-size: 16px; color: #222222; background: #FFFFFF; vertical-align: middle; }
                        .iip .iip-input:focus { outline: none; border-color: #F09618; box-shadow: 0 0 0 3px rgba(240, 150, 24, 0.2); }
                        .iip .iip-wide { width: 100%; }
                        .iip .iip-short { width: 160px; }
                        .iip .iip-price-input { width: 120px; }
                        .iip select.iip-input { width: auto; min-width: 80px; padding: 6px; }
                        .iip .iip-gp { display: inline-block; margin-left: 8px; font-weight: bold; vertical-align: middle; }
                        .iip .iip-check { display: inline-block; padding: 4px 0; color: #333333; cursor: pointer; }
                        .iip .iip-check input { width: 18px; height: 18px; margin: 0 8px 0 0; vertical-align: middle; }
                        .iip .iip-footer { padding: 12px 20px; border-top: 1px solid #EEEEEE; background: #FAFAFA; text-align: right; }
                        .iip .iip-btn { box-sizing: border-box; min-width: 90px; height: 36px; margin: 0 0 0 8px; padding: 0 16px; border-radius: 4px;
                            font-family: Arial, Helvetica, sans-serif; font-size: 0.95em; font-weight: bold; cursor: pointer; text-shadow: none; }
                        .iip .iip-btn-cancel { border: 1px solid #C8C8C8; background: #FFFFFF; color: #444444; }
                        .iip .iip-btn-cancel:hover { background: #F0F0F0; }
                        .iip .iip-btn-delete { float: left; margin: 0; border: 1px solid #E8B4B0; background: #FFFFFF; color: #C62828; }
                        .iip .iip-btn-delete:hover { background: #FDECEA; }
                        .iip .iip-btn-primary { border: 1px solid #2E7D32; background: #43A047; background-image: linear-gradient(to bottom, #4CAF50, #388E3C); color: #FFFFFF; }
                        .iip .iip-btn-primary:hover { background: #2E7D32; }
                    </style>
                    <asp:Panel ID="pnlPopItemDtl"  style="display:none;" runat="server" CssClass="popup_dialog iip">
                            <div class="iip-header">
                                <h1><asp:Label ID="lblPopHeader_Item" runat="server" Text="View/Add/Edit"/> Item Info</h1>
                                <%--Same as the Close button (refreshes the list)--%>
                                <button type="button" class="iip-close" title="Close" onclick="$('#<%= btnPopCloseItem.ClientID %>').click(); return false;">&times;</button>
                            </div>
                            <div class="iip-body">
                                <asp:Panel runat="server" ID ="pnlPopWarningItem" CssClass="iip-alert">
                                    <asp:Label ID="lblPopWarningItem" runat="server" />
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlPopDtlEdit_Item">
                                    <%--Item--%>
                                    <div class="iip-summary">
                                        <span class="iip-itemno">Item # <asp:Label ID="lblPopEditItem" runat="server" /></span>
                                        <asp:Label ID="lblPopEditDesc" CssClass="iip-desc" runat="server" />
                                        <div class="iip-stats">
                                            <span class="iip-stat">Each / Case<br /><b><asp:Label ID="lblPopEditEachCase" runat="server" /></b></span>
                                            <asp:Panel runat ="server" ID="pnlPoppcpercs" CssClass="iip-stat">Piece / Case<br /><b><asp:Label ID="lblPopEditPcperCs" runat="server" /></b></asp:Panel>
                                            <span class="iip-stat">Inventory<br /><b><asp:Label ID="lblPopEditInventory" runat="server" /></b></span>
                                        </div>
                                    </div>

                                    <%--Prices--%>
                                    <div class="iip-section">Price</div>
                                    <div class="iip-prices">
                                        <div class="iip-price"><span class="iip-k">Cost</span><span class="iip-v"><asp:Label ID="lblPopEditCost" runat="server" /></span></div>
                                        <div class="iip-price"><span class="iip-k">Bottom Price</span><span class="iip-v"><asp:Label ID="lblPopEditBtm" runat="server" /></span></div>
                                        <div class="iip-price"><span class="iip-k">Regular Price</span><span class="iip-v"><asp:Label ID="lblPopEditReg" runat="server" /></span></div>
                                        <div class="iip-price"><span class="iip-k">Current Price</span><span class="iip-v">
                                            <asp:Label ID="lblPopEditPriceOriginal" runat="server" />
                                            <asp:Label runat="server" ID="lblPopEditNewLbCost" Visible="false" />
                                            <asp:Label runat="server" ID="lblPopEditNewCsCost" Visible="false" />
                                            <asp:Label runat="server" ID="lblPopEditNewEaCost" Visible="false" />
                                        </span></div>
                                    </div>
                                    <div class="iip-field">
                                        <asp:Label ID="lblPopTitlePriceNew" CssClass="iip-label" runat="server" Text="New Price" />
                                        <asp:TextBox runat="server" ID="txtPopPrice" onchange="javascript:GetGP(); " CssClass="iip-input iip-price-input disableEnter" autocomplete="off" placeholder="0.00" inputmode="decimal" />
                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender11" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtPopPrice" />
                                        <asp:DropDownList ID="lstPopPriceUnit" onchange="javascript:GetGP();" CssClass="iip-input" runat="server" />
                                        <asp:Label runat="server" ID="lblPopGP" CssClass="iip-gp red"/>
                                    </div>

                                    <%--Customer's own item # / description--%>
                                    <div class="iip-section">Customer Item</div>
                                    <div class="iip-field">
                                        <label class="iip-label" for="<%= txtPopEditItem.ClientID %>">Customer Item #</label>
                                        <asp:TextBox ID="txtPopEditItem" CssClass="iip-input iip-short disableEnter" runat="server" autocomplete="off" placeholder="Optional" />
                                        <asp:Label ID="lblPopItem" runat="server" />
                                    </div>
                                    <div class="iip-field">
                                        <label class="iip-label" for="<%= txtPopEditDesc.ClientID %>">Customer Description</label>
                                        <asp:TextBox ID="txtPopEditDesc" CssClass="iip-input iip-wide disableEnter" Text="" runat="server" autocomplete="off" placeholder="Optional" /><%--Text required so that validator wouldn't work by default--%>
                                        <asp:Label ID="lblPopDesc" Visible="false" runat="server" />
                                    </div>
                                    <div class="iip-field">
                                        <label class="iip-label" for="<%= txtPopEditRemark.ClientID %>">Remark</label>
                                        <asp:TextBox ID="txtPopEditRemark" CssClass="iip-input iip-wide disableEnter" autocomplete="off" Text="" runat="server" />
                                    </div>

                                    <%--Email--%>
                                    <div class="iip-section">Email</div>
                                    <div class="iip-field">
                                        <label class="iip-check"><asp:CheckBox ID="ckbPopEmail" ClientIDMode="Static" runat ="server" />Send email to customer</label>
                                    </div>
                                    <div class="iip-field" id="emailNoteRow">
                                        <label class="iip-label" for="<%= txtNote.ClientID %>">Email Message</label>
                                        <asp:TextBox runat="server" ID="txtNote" CssClass="iip-input iip-wide disableEnter" autocomplete="off" />
                                    </div>
                                </asp:Panel>
                            </div>
                            <div class="iip-footer">
                                <asp:Button ID="btnPopDeleteItem" CssClass="iip-btn iip-btn-delete" runat="server" Text="Delete" OnClientClick="javascript:showProgress();"/>
                                <asp:Button ID="btnPopCloseItem" CssClass="iip-btn iip-btn-cancel" runat="server" Text="Close" OnClientClick="javascript:showProgress();"/>
                                <asp:Button ID="btnPopSaveItem" CssClass="iip-btn iip-btn-primary" runat="server" Text="Update" OnClientClick="javascript:showProgress();"  />
                            </div>
                        </asp:Panel>
                    <br />
                    <%--<asp:Label ID="Label11" CssClass="red" runat="server" Text="Please use 'Update Changes' button for each page"/>
                    <br />
                    <asp:Button ID="btnSaveList_custItem" CssClass="midButton" runat="server" Text="Update Changes" />
                    <asp:Button ID="btnPrint" OnClientClick='javascript:showPopupWindow("../Common/Print.aspx");' CssClass="midButton" runat="server" Text="Print Order Form" />
                    <br />--%>
                </asp:Panel>
                     
            </div>
           
        </asp:Panel>
         <%--  --%>

        <asp:Panel runat="server" ID="pnlTabPromoItem">
        <div id="tabPromoItem">
         
                 <h2>Promo Items </h2>
              <asp:Panel ID = "pnlPromoitemlst" runat="server">
              <asp:LinkButton ID="lnkDeletePromoItems" runat="server" CssClass=" cellLink" ForeColor="#4169E1">Delete checked items</asp:LinkButton>
              <asp:GridView ID="grdPromoItemlst" runat="server" DataSourceID="dsPromoItem" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                           <asp:TemplateField   HeaderStyle-CssClass="th center4"  ItemStyle-CssClass="center4" >
                             <HeaderTemplate  >  
                                         <asp:CheckBox ID="ChkAll"  onclick="javascript:checkAll(this);"  runat="server" /> 
                              </HeaderTemplate>  
                            <ItemTemplate >
                                <asp:CheckBox ID="chkRow" CssClass="center4" runat="server" />
                            </ItemTemplate>
                    </asp:TemplateField>
                           <asp:TemplateField HeaderText="Item Code" HeaderStyle-CssClass="th size12" ItemStyle-CssClass="size12" >
                               
                                <ItemTemplate>
                                 <asp:Label ID="lblItemId" CssClass=" " runat ="server" Text='<%# Eval("ITEM_ID")%>'></asp:Label>
                                                          
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:BoundField HeaderText="Item Description" DataField="ITEMDESC" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" /> 
                             <asp:BoundField HeaderText="Setting" DataField="Setting" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" /> 
                             <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>'  CommandArgument='<%# Cstring(Eval("ITEM_ID")) %>' runat="server" Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
              
                    <asp:SqlDataSource ID="dsPromoItem" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="btnPromoAdd" CssClass="midButton" runat="server" Text="Add Item" />
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
            
  
    </div> 
    </div>  
</asp:Content>
