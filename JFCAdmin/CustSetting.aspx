<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="CustSetting.aspx.vb" Inherits="JFCAdmin_CustSetting" %>
<%@ Import Namespace="Common" %>
<%--<%@ Register Tagprefix="uc" Tagname="CustDtl" Src="~/Common/CustDtl.ascx" %>--%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

 
    <script src="../Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
  
      <script type="text/javascript">
   
       
         function loadData(id,id2,id3,id4,code,callback)
         {
              //var text =document.getElementById('<%=txtPopJFCNum.ClientID %>').value;
              //var id = txt.value;
              //var id2 = txtid.value;
              $.ajax({
				        // try to leverage ajaxQueue plugin to abort previous requests
				        // limit abortion to this input
				        type: 'POST',
				        dataType: "text",
				        //url: '<%=Request.ApplicationPath()%>/Common/Suggestion.ashx?id=<%=lblCustomerID.text%>&id2=' + txtid +'&id3=&id4=&code=JFCBranch' ,
                        url:'../Common/Suggestion.ashx?id='+ id +'&id2=' + id2 +'&id3=' + id3 + '&id4=' + id4 + '&code='+ code +'',
				        data:  '{}' ,
                        async: false,
				        success: function(data) {
                           //document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML =data;
                            callback(data);
				        },
                        error: function() {
                            alert('Error occured');
                        }
			        });
         }  

        
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
         //if (!data) 
                   // {
                    
                    // ValidatorEnable(myVal, true); 
                    // alert(myVal);
                   // }
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
                    
                    // ValidatorEnable(myVal, true); 
                    // alert('v');
                    }
                    else{
                    //args.IsValid =true;
                     //alert(data);
                    }
                   });
                  
             }
             else{
                 document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML ='';
                 args.IsValid = false;
                 //alert(id2);
             }       
            //if (v == '') {
             //   args.IsValid = false;  // field is empty
             //   alert(v);
            //}
            //else {
            //    args.IsValid =true;
            // do your other validation tests here...
           // }
        }
        function ClientValidateEmail(sender, args){
            var id = document.getElementById('<%=txtPopEmail.ClientID%>').value;
            var id2 = document.getElementById('<%=txtPopJFCAccount.ClientID %>').value;
            var id3 = document.getElementById('<%=txtPopJFCNum.ClientID %>').value;
              if(id.length > 0)
              {
                   loadData(id,id2,id3,'','VAILDEMAIL',function(data) {
                    //processing the data
                   if (!data) 
                   {
                    
                   }
                    else{
                        var errorMessage = data;
                        args.IsValid = false;
                        
                        // ValidatorEnable(myVal, true); 
                        sender.innerText = errorMessage;
                        sender.errormessage = errorMessage;
                        //args.IsValid =true;
                        //alert(data);
                    }
                   });
                 
             }
             else{
                 document.getElementById('<%=lblPopCustomerName.ClientID%>').innerHTML ='';
                 args.IsValid = false;
                 //alert(id2);
             }  
        }

        function EnableButton()
        {

             var button = document.getElementById('<%= btnUpdateChnage.ClientID %>');
             var button2 = document.getElementById('<%= btnSaveCust.ClientID %>');
             var button3 = document.getElementById('<%= btnUpdateSchedule.ClientID %>');
             

            if(Page_IsValid)
            {
                 button.disabled = false;
                button.className  = "midButton_green";

                button2.disabled = false;
                button2.className  = "midButton_green";

                 button3.disabled = false;
                button3.className  = "midButton_green";
            }
            else
            {
                
                button.disabled = true;
                 button2.disabled = true;
                  button3.disabled = true;

            }
        }

        $(document).ready(function () {
            var store = $('#<%=HiddevActiveTab.ClientID%>');
            //$("#tabs").tabs();
            $("#tabs").tabs();
            $('#tabs').bind('tabsselect', function(event, ui) {
                var selectedTab = ui.index;           
                $("#<%= HiddevActiveTab.ClientID %>").val(selectedTab);
            });
            $("#tabs").removeClass("ui-widget");
            $('#tabs').css('display', 'block'); //Display tabs when page finish loading
            

            
            //$('#<%=lstCustCompany.ClientID%>').change(function(){alert(optionValue);});
           $('#<%=txtJfcBranchNr.ClientID%>').mask("99");
           $.mask.definitions['h'] = "[A-Za-z0-9]";
           $('#<%=txtJfcCustomerNr.ClientID%>').mask("hhhh");
           $(".resetCursor").mouseup(function () {
             this.setSelectionRange(0,0);    
            });

           
           // var optionValue = $(this).attr("value");
           // if(optionValue){
            //    $("#tabs").hide();
            //} else{
           //     $("#tabs").show();
           // }
           // alert(optionValue);}).change();
            
            function address_search() {
                alert($(".CODE_ITEM").val());
            }
            
            //.keydown(function(){alert('a');});
             //$(".CODE_ITEM").on('autocompleteselect', function (e, ui) {
               //function(){alert('a');}
              //});
            <%= JavaScript %>
        });
    </script>

    
    <h1>Customer Settings&nbsp;&nbsp;<span class="fontsize-4"><asp:Button ID="btnAddCust" CssClass="midButton" runat="server" Text="Register Customer" /></span></h1>
    <asp:Panel ID="pnlSearchCust" DefaultButton="btnSearchCust" CssClass="visiblePnl" runat="server"> 
        <asp:Label ID="lblGetCustCompanyType" runat="server" Text='<%# GetCustCompanyType()%>'  CssClass="gray" />
        <asp:DropDownList ID="lstCustCompany" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' OnChange="javascript:showProgress();" runat="server" AutoPostBack="True" />
        or<asp:TextBox ID="txtJfcBranchNr" CssClass="largeTxt size4" runat="server" autocomplete="off" />
        -<asp:TextBox ID="txtJfcCustomerNr" CssClass="largeTxt size8 resetCursor" runat="server" autocomplete="off" />
        <asp:ImageButton ID="btnSearchCust" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("search.png") %>' runat="server" />
        <%--<asp:ImageButton ID="btnRefresh" ToolTip="Refresh customer settings" CssClass="xsImgBtn" ImageUrl='<%# GetWebImgUrl("refresh.jpg") %>' runat="server" />--%>&nbsp;&nbsp;
          <%--Popup window to Item detail--%>
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
                                <h1><asp:Label ID="lblPopTitleUser" runat="server" Text="Add New Customer"/> </h1>
                                <asp:Panel runat="server" ID ="pnlPopWarningUser">
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
                                  <br />
                                  <asp:CustomValidator id="CVPopEmail" ControlToValidate="txtPopEmail" ClientValidationFunction="ClientValidateEmail" 
                                            Display="Static"
                                            ErrorMessage="Invalid. Please enter a correct email"
                                            ForeColor="red"
                                            ont-Name="verdana" 
                                            Font-Size="10pt"
                                            ValidationGroup="validgroupUser"   
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
                                            
                                              <%--<asp:RegularExpressionValidator ID="REValidatorJFCAccount"  OnServerValidate="ClientValidate_PopJFCNum" 
                                             runat="server"  ControlToValidate="txtPopJFCNum" Display="None" 
                                             ErrorMessage="Invalid format <br /> Please enter a valid 4 digit account number."
                                             ValidationExpression="^\d{4}$" 
                                             OnServerValidate="validateData"  ValidationGroup ="validgroup" />
                                            <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" PopupPosition="BottomLeft"                                       
                                                TargetControlID="REValidatorJFCAccount"  runat="server"    /--%>
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
                                            <td class="rowHdr">Email</td>
                                            <td><asp:TextBox ID="txtPopEmail" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>'  runat="server" autocomplete="off"  ValidationGroup="validgroupUser"/>
                                          
                                            <%-- asp:RegularExpressionValidator ID="RegularExpressionValidator2"  OnServerValidate="ServerValidation"
                                             runat="server" ValidationGroup="txtEmail" ControlToValidate="txtPopEmail" Display="None" 
                                             ErrorMessage="Invalid format <br /> Please enter a valid email."
                                             ValidationExpression="^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"
                                             OnServerValidate="validateData" />
                                            <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" PopupPosition="BottomLeft"                                       
                                                TargetControlID="RegularExpressionValidator2"   runat="server"     />--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr"></td>
                                            <td>
                                              
                                            </td>
                                        </tr>
                                      
                                        <tr>
                                            <td colspan="2" class="center">
                                                <asp:Button ID="btnPopSaveUser" CssClass="midButton_green" runat="server" Text="Add"   ValidationGroup="validgroupUser" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                            </div>
                        </asp:Panel>
    </asp:Panel>
    <br />
    <asp:Label ID="lblTitle" runat="server"  Text='Please select a customer.'  CssClass=" gray" />
    <div id="Alltab" runat ="server" >
    <div id="tabs" style="display: none;" > <%--Display tabs when page finish loading--%>
       <ul>
            <li><a href="#tabCust">Customer</a></li>
            <%-- <li><a href="#tabSchedule">Delivery Schedule</a></li>
              <li><a href="#tabCustItem" id = "lnkOrderForm">Order Form</a></li>--%>
        </ul>
        
        <%----------------------------------------------------------------------------------------------------------------------
            Manage customer info
        ----------------------------------------------------------------------------------------------------------------------%>
        <asp:Panel runat="server" ID="pnlTabCust" >
            <div id="tabCust">
             
                <h2>Customer</h2>
                <asp:Panel ID="Panel14" runat="server" DefaultButton="btnSaveCust"> 
                    <asp:Label ID="lblCustomerID" visible="false" runat="server" />
                    <%--Customer Info--%>
                    <asp:Panel ID="pnlCustInfo" CssClass='<%# "visiblePnl " & IIf(IsMobile(), "", "leftContent") %>' runat="server">
                        <h1><%# GetCustCompanyType() %>&nbsp;Info</h1>
                        <table>
                            <tbody>
                                <tr>
                                    <td class="rowHdr"><%# GetCustCompanyType() & " Name" %></td>
                                    <td><asp:Label ID="lblCustomerName" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">JFC Account #</td>
                                    <td><asp:Label ID="lblJfcBranchNr" CssClass="rowLblVal" runat="server" />-<asp:Label ID="lblJfcCustomerNr" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Phone</td>
                                    <td><asp:Label ID="lblPhone" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Fax</td>
                                    <td><asp:Label ID="lblFax" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Address 1</td>
                                    <td ><asp:Label ID="lblAddress1" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Address 2</td>
                                    <td><asp:Label ID="lblAddress2" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">City, State</td>
                                    <td><asp:Label ID="lblCity" CssClass="rowLblVal" runat="server" /><asp:Label ID="lblState" CssClass="rowLblVal" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Zip, Country</td>
                                    <td><asp:Label ID="lblZip" CssClass="rowLblVal" runat="server" /><asp:Label ID="lblCountry" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">JFC Representative</td>
                                    <td><asp:Label ID="lblJfcContactName" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">JFC Email</td>
                                    <td><asp:Label ID="lblJfcEmail" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">JFC Phone</td>
                                    <td><asp:Label ID="lblJfcPhone" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr> 
                                    <td class="rowHdr">AutoPay Type<br />(Setup by Customer)</td>
                                    <td><asp:Label ID="lblAutoPayType" CssClass="rowLblVal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Discount Type</td>
                                    <td><asp:Label ID="lblWebDiscType" CssClass="rowLblVal" runat="server" /> &nbsp;&nbsp;</td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                    </asp:Panel>
                    <%= IIf(IsMobile(), "<br class='turnOffFloat' />", "")%>
    
                    <%--Customer Settings--%>
                    <asp:Panel ID="pnlCustSetting" CssClass="visiblePnl" style='<%# IIf(IsMobile(), "", "margin-left:533px") %>' runat="server">
                        <h1><%# GetCustCompanyType() %>&nbsp;Settings</h1>
                        <table>
                            <tbody>
                                <tr>
                                    <td class="rowHdr">Order</td>
                                    <td><asp:DropDownList ID="lstAllowCustOrder" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>'  runat="server"  AutoPostBack="True"  /><span class="red">*</span>
                                    </td>
                                </tr>
                                <tr style="display:none;">
                                    <td class="rowHdr">Item Search Page</td>
                                    <td><asp:DropDownList ID="lstHideItemPage" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server" onchange="javascript:EnableButton();"  AutoPostBack="True" /><span class="red">*</span></td>
                                </tr>
                                <tr style="display:none;">
                                    <td class="rowHdr">Payment Page</td>
                                    <td><asp:DropDownList ID="lstHidePaymentPage" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server" onchange="javascript:EnableButton();"  AutoPostBack="True" /><span class="red">*</span></td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Price</td>
                                    <td><asp:DropDownList ID="lstHidePrice" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server"    AutoPostBack="True" /><span class="red">*</span></td>
                                </tr>
                                      <asp:Panel runat="server" ID="pnlChatControls" Visible="true"> 
                                     <tr>
                                     <td class="rowHdr">Chat</td>
                                       <td><asp:DropDownList ID="lstAllowChat" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server" AutoPostBack="True" /><span class="red">*</span></td>
                                  </tr>
                                  </asp:Panel>
                                  <tr>
                                      <td class="rowHdr">Home Page</td>
                                      <td><asp:DropDownList ID="lstHideHomePage" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server"  onchange="javascript:EnableButton();"  /><span class="red">*</span></td>
                                  </tr>
                                  <tr>
                                    <td class="rowHdr">Delivery Anytime</td>
                                    <td><asp:DropDownList ID="lstDeliveryAnytime" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server"  AutoPostBack="True" /><span class="red">*</span><br />
                                        <span class="gray">If yes, no need to setup delivery schedule.</span></td>
                                 
                                </tr>
                                 <tr>
                                    <td class="rowHdr">Out of Stock</td>
                                    <td><asp:DropDownList ID="lstOutofStock" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' runat="server" onchange="javascript:EnableButton();"  /><span class="red">*</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class= "rowHdr">Show all exception price group items on Buy Again</td>
                                    <td><asp:DropDownList ID="lstAllExcepGrpItem" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size12", "size16") %>' onchange="javascript:EnableButton();" runat="server"  /><span class="red">*</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="rowHdr">Minimum Order Amount</td>
                                    <td><asp:textbox ID="txtMinOrderAmt" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size8", "size16") %>' runat="server" autocomplete="off" onchange="javascript:EnableButton();" />
                                    <asp:RegularExpressionValidator ID="RegExpMinOrderAmt" runat="server" 
                                      ControlToValidate="txtMinOrderAmt" Display="None" ErrorMessage="Invalid format <br /> Please enter a positive integer."
                                      ValidationExpression="^\d+$"  OnServerValidate="validateData" ValidationGroup="validgroupCompany" />
                                      <ajaxToolkit:ValidatorCalloutExtender ID="ValCalloutExtMinOrderAmt" PopupPosition="BottomLeft"                                       
                                      TargetControlID="RegExpMinOrderAmt" runat="server"  /><span class="gray"></span>
                                      
                                    </td>
                                </tr>
                                  <tr>
                                    <td class="rowHdr">Ship Via</td>
                                    <td><asp:textbox ID="txtShipvia" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size16", "size32") %>' runat="server" autocomplete="off" onchange="javascript:EnableButton();" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                      ControlToValidate="txtShipvia" Display="None" ErrorMessage="Invalid format <br /> Please enter a ship via must be less than 21 characters"
                                      ValidationExpression="^.{0,20}$"  OnServerValidate="validateData" ValidationGroup="validgroupCompany" />
                                      <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" PopupPosition="BottomLeft"                                       
                                      TargetControlID="RegularExpressionValidator2" runat="server"  />
                                    </td>
                                </tr>
                                    <tr>
                                    <td class="rowHdr">Ship Date Offset</td>
                                    <td><asp:textbox ID="txtShipDateOffset" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size8", "size16") %>' runat="server" autocomplete="off" onchange="javascript:EnableButton();" />
                                    <asp:RegularExpressionValidator ID="RegExpValOffset" runat="server" 
                                      ControlToValidate="txtShipDateOffset" Display="None" ErrorMessage="Invalid format <br /> Please enter a positive integer."
                                      ValidationExpression="^\d+$"  OnServerValidate="validateData" ValidationGroup="validgroupCompany" />
                                      <ajaxToolkit:ValidatorCalloutExtender ID="ValCalloutOffset" PopupPosition="BottomLeft"                                       
                                      TargetControlID="RegExpValOffset" runat="server"  /> <br />
                                      <span class="gray">Difference in days between ship date and delivery date</span>
                                    </td>
                                </tr>
                                <asp:Panel runat="server" ID="pnlPhoneOrder" Visible="false"> 
                                <tr>
                                    <td class="rowHdr">Phone (Order Reminder)</td>
                                    <td>
                                     
                                    <asp:TextBox ID="txtTextPhones" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size32", "size48") %>' Text="" runat="server" autocomplete="off" 
                                    onchange="javascript:EnableButton();" ValidationGroup="validgroupCompany"/>
                                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                      ControlToValidate="txtTextPhones" Display="None" ErrorMessage="Invalid format <br /> Please enter a phone number in the format: <br /> (###)###-####,...<br />Or ###-###-####,... "
                                      ValidationExpression="(?:((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}(?:,|$))+$"  OnServerValidate="validateData" ValidationGroup="validgroupCompany" />
                                      <ajaxToolkit:ValidatorCalloutExtender ID="ceUsuarioNome" PopupPosition="BottomLeft"                                       
                                      TargetControlID="RegularExpressionValidator1" runat="server"  /><br />
                                      <span class="gray">Use comma to enter multiple phone #.</span>
                                    </td>
                                
                                </tr>
                             
                                </asp:Panel>
                                <tr>
                                    <td> <asp:Button ID="btnUpdateChnage" class="midButton_gray Savebutton" CssClass="midButton_gray Savebutton" runat="server" Text="Update Changes" Enabled="false" ValidationGroup="validgroupCompany" />
                                    </td>
                                    <td style="text-align:right">
                                    <br />
                                   
                                   </td>
                                </tr>
                              
                               
                            </tbody>
                        </table>
                        <br />
                      
                    </asp:Panel>
                    <br />
 
                    <%--Shipping point List--%>
                    <asp:Panel id="pnlShipPoint" runat="server">
                        <h2>Shipping Address</h2>
                        <asp:GridView ID="grdShipPoint" runat="server" DataSourceID="dsShipPoint" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                            <Columns>
                                <asp:BoundField HeaderText="Shipping Name" DataField="SHIPNAME" HeaderStyle-CssClass="th size48" ItemStyle-CssClass="size48" />
                                <asp:BoundField HeaderText="Address 1" DataField="ADDRESS1" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" />
                                <asp:BoundField HeaderText="Address 2" DataField="ADDRESS2" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />
                                <asp:BoundField HeaderText="City" DataField="CITY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                                <asp:BoundField HeaderText="State" DataField="STATE" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                                <asp:BoundField HeaderText="Zip" DataField="ZIP" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                                <asp:BoundField HeaderText="Country" DataField="COUNTRY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:SqlDataSource ID="dsShipPoint" runat="server"></asp:SqlDataSource>
                        <br />
                    </asp:Panel>
                    <asp:Panel id="pnlSchedule" runat="server">
                    
                    <%--Schedule  --%>
                    <br />
                    <h2>Delivery Schedule</h2>
                    <asp:Label ID="lblShipPoint" runat="server" Text="Shipping Destination" CssClass="gray" />
                    <asp:DropDownList ID="lstShipPoint" CssClass="largeDrpList" runat="server" AutoPostBack="True" />
                    <%--<asp:Label ID="Label20" CssClass="helpLabel" runat="server" Text="Click an item from the following list to view or edit" />--%>
                    <br /><br />
                    <%--List of delivery schedule--%>
                    <asp:GridView ID="grdActiveSchedule" runat="server" DataSourceID="dsActiveSchedule" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false">
                        <Columns>
                            <asp:BoundField HeaderText="Customer" DataField="customerName" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"  Visible = "false"/> 
                            
                            <asp:TemplateField HeaderText="Shipping Destination" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32" Visible ="false">
                                <ItemTemplate>
                                    <asp:Label Visible="false" runat="server" ID="lbldeliveryschedlinenr" Text='<%# Eval("deliveryschedlinenr") %>' />
                                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkShipName" Text='<%# Eval("shipName") %>' CommandName="EDIT_ROW" CommandArgument='<%# Eval("deliverySched_id") & Eval("deliveryschedlinenr") %>' />
                                    <%--<asp:LinkButton ID="lnkSelectRow" Visible="false" runat="server" CommandName="EDIT_ROW" CommandArgument='<%# Eval("deliverySched_id")%>' />--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Vendor" DataField="vendorName" HeaderStyle-CssClass="th size32" ItemStyle-CssClass="size32"  Visible ="false"/>
                            
                            <asp:TemplateField HeaderText="Master"  HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" >
                              <ItemTemplate>
                                    <asp:Label Visible="false" runat="server" ID="lblDeliverySchedID" Text='<%# Eval("deliverySched_id") %>' />
                                    
                                    <asp:LinkButton runat="server" CssClass="cellLink" ForeColor="#4169E1" ID="lnkdeliveryMasterName" Text='<%# Eval("deliveryMaster_id") %>' CommandName="EDIT_ROW" CommandArgument='<%# Eval("deliverySched_id") & Eval("deliveryschedlinenr") %>' Visible="false" />
                                    <asp:DropDownList runat ="server" CssClass="cellLbl drpList" ID = "lstDeliveryMaster" OnSelectedIndexChanged="lstDeliveryMaster_SelectedIndexChanged" OnChange="javascript:showProgress();"   AutoPostBack="true" ></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:BoundField HeaderText="Frequency" DataField="FREQUENCY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            <asp:BoundField HeaderText="Delivery Day" DataField="DELIVERYDAY" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" />
                            <asp:BoundField HeaderText="Order By" DataField="orderByDayTime" HeaderStyle-CssClass="th size24" ItemStyle-CssClass="size24" />
                            <asp:TemplateField HeaderText="Send Reminder" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ChkSendReminder" runat ="server" Checked = '<%# CString(Eval("sendReminder")) = "Y" %>' onchange="javascript:EnableButton();"  />
                                    <asp:Image ID="Image1" CssClass="xxxsImgBtn" ImageUrl='<%# GetWebImgUrl("check.png") %>' Visible='false' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Notify Change to Customer" HeaderStyle-CssClass="th center16" ItemStyle-CssClass="center16">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckbEmail" runat ="server" onchange="javascript:EnableButton();"  />
                                    
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Starting" DataField="STARTDATE" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" Visible="false" />
                            <asp:BoundField HeaderText="Ending" DataField="ENDDATE" HeaderStyle-CssClass="th size16" ItemStyle-CssClass="size16" Visible="false" />
                            <asp:TemplateField ItemStyle-CssClass="center4">
                                <ItemTemplate>
                                   <asp:ImageButton ID="btnDelete" Tooltip="Delete delivery schedule" CommandName="DELETE_ROW" CssClass="xxsImgBtn" 
                                   ImageUrl='<%# GetWebImgUrl("close.jpg") %>' 
                                    CommandArgument='<%# Eval("deliverySched_id") & Eval("deliveryschedlinenr") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="dsActiveSchedule" runat="server"></asp:SqlDataSource>
                    <br />
                    <asp:Button ID="btnUpdateSchedule" CssClass="midButton_gray Savebutton" runat="server" Text="Update Changes" Enabled="false" ValidationGroup="validgroupCompany" />
                    <asp:Button ID="btnAddSchedule" CssClass="midButton" runat="server" Text="Add Schedule" />
                    
                    <br /><br />            
                         <asp:LinkButton ID= "btnDisplayCalendar" CssClass=" linkLabel bold" runat ="server"  ForeColor="#4169E1" Text="Show Delivery Calendar" OnClientClick= "javascript:showProgress();" ></asp:LinkButton>
                
              <%--Calendar--%>
                    <asp:Panel runat="server" Visible='<%# IIf(IsMobile(), "False", "True") %>' ID="pnlCalendar"  >
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
                        <asp:LinkButton ID="lnkDelivery" style="display:none;" runat="server" onclick="lnkDelivery_Click" />
                        <br />

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
                                <h1><asp:Label ID="lblPopHeader_schedule" runat="server" Text="View/Add/Edit"/> Delivery</h1>
                                <br />
                                <asp:Panel runat="server" ID="pnlPopDtlEdit_schedule"> 
                                  <asp:Panel runat="server" ID ="pnlPopSheduleWarning" Visible="false" >
                                  <asp:Label ID="lblPopSheduleWarning" CssClass=" red bold  "  runat="server" />
                                   <br />
                                    <br />
                                  </asp:Panel>             
                                    <table>
                                        <tr style="display:none;">
                                            <td class="rowHdr">Vendor</td>
                                            <td><asp:DropDownList ID="lstPopCustGrpVendor" CssClass="drpList size48" runat="server" /><asp:Label ID="lblPopCustGrpVendorID" visible="false" runat="server"/>
                                                <asp:Label ID="lblPopStarCustGrpVendor" CssClass="red" runat="server" Text="*"/>
                                                <asp:Label ID="lblPopVendorName" CssClass="rowLblVal" runat="server" /></td>
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
                                            <td class="rowHdr">Send Reminder</td>
                                            <td><asp:DropDownList ID="lstPopSendReminder" CssClass="drpList size12" runat="server" />
                                                <asp:Label ID="Label37" CssClass="red" runat="server" Text="*"/></td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr">Schedule Name</td>
                                            <td><asp:Label ID="lblPopSchedule" CssClass="rowLblVal" runat="server" /></td>
                                        </tr>
                                        <tr><td colspan="2"><asp:CheckBox  ID="ckbPopEmail" runat ="server" CssClass= "checkbox"  class="fontsize-1"/>Send email to customer (for update only)</td></tr>
                                        <tr>
                                            <td colspan="2" class="red"><asp:Label ID="Label33" CssClass="red" runat="server" Text="* Required"/></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="center"><asp:Button ID="btnPopSaveDelivery" CssClass="midButton_green" runat="server" Text="Update"  ValidationGroup="validgroup" />
                                                <asp:Button ID="btnPopDeleteDelivery" CssClass="midButton" runat="server" Text="Delete Delivery" CausesValidation="false" /></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                              
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                    <br />
                 </asp:Panel>
                    <br />
                    <br />
                   
                    <%--User List--%>
                    <asp:Panel runat="server" ID="pnlUser">
                        <h2>Users </h2>
                        <%--<asp:Label Visible='<%# IIf(IsMobile(), "True", "False") %>' runat="server" ID="lblUsers" CssClass="rowLblVal red"  Text = "Same as customer setting if empty"/>--%>
                        <asp:GridView ID="grdUser" runat="server" CssClass="STDTABLE" AutoGenerateColumns="false" GridLines="Horizontal" ShowHeaderWhenEmpty="false" AllowSorting="false" AllowPaging="false"><%-- ShowFooter="true"--%>
                            <Columns>
                                <asp:TemplateField HeaderText="Email" HeaderStyle-CssClass='th size32 left' ItemStyle-CssClass=" size32 left" >
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblEmail" CssClass="cellLbl" Text='<%# Eval("email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cell Phone (Text)" HeaderStyle-CssClass='th size32 left' ItemStyle-CssClass=" size32 left" >
                                    <ItemTemplate>
                                        <asp:Textbox runat="server" ID="txtCell" CssClass="cellLbl stdTxt" Text='<%# Eval("CELLULAR")%>' onchange="javascript:EnableButton();" autocomplete="off" />
                                          <asp:RegularExpressionValidator ID="revphone" runat="server" 
                                      ControlToValidate="txtCell" Display="None" ErrorMessage="Invalid format <br /> Please enter a phone number in the format: <br /> (###)###-####,...<br />Or ###-###-####,... "
                                      ValidationExpression="(?:((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}(?:,|$))+$"  OnServerValidate="validateData" ValidationGroup="validgroupCompany" />
                                      <ajaxToolkit:ValidatorCalloutExtender ID="validatorcallphone" PopupPosition="Right"                                       
                                      TargetControlID="revphone" runat="server"  /><br />
                                    </ItemTemplate> 
                                </asp:TemplateField>
                                  <asp:TemplateField HeaderText="First Name" HeaderStyle-CssClass='th size10 left' ItemStyle-CssClass=" size10 left" >
                                    <ItemTemplate>
                                        <asp:Textbox runat="server" ID="txtfirstName" CssClass="cellLbl stdTxt size10" Text='<%# Eval("firstName")%>' onchange="javascript:EnableButton();" autocomplete="off" />
                                          
                                    </ItemTemplate> 
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Last Name" HeaderStyle-CssClass='th size10 left' ItemStyle-CssClass=" size10 left" >
                                    <ItemTemplate>
                                        <asp:Textbox runat="server" ID="txtlastName" CssClass="cellLbl stdTxt size10" Text='<%# Eval("lastName")%>' onchange="javascript:EnableButton();" autocomplete="off" />
                                          
                                    </ItemTemplate> 
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order" HeaderStyle-CssClass="th center32" ItemStyle-CssClass="center24" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="lstHideCustOrder" CssClass="cellLbl drpList"  runat="server" onchange="javascript:EnableButton();" />
                                        <asp:Label Visible="false" runat="server" ID="lblHideCustOrder" CssClass="cellLbl" Text='<%# Eval("hideCustOrder") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Chat" HeaderStyle-CssClass="th center32" ItemStyle-CssClass="center24">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="lstHideChat" CssClass="cellLbl drpList" runat="server" onchange="javascript:EnableButton();" />
                                        <asp:Label Visible="false" runat="server" ID="lblHideChat" CssClass="cellLbl" Text='<%# Eval("hideChat") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Search Page" HeaderStyle-CssClass="th center32" ItemStyle-CssClass="center24" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="lstHideItemPage" CssClass="cellLbl drpList" runat="server" onchange="javascript:EnableButton();" />
                                        <asp:Label Visible="false" runat="server" ID="lblHideItemPage" CssClass="cellLbl" Text='<%# Cstring(Eval("hideItemPage")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Page" HeaderStyle-CssClass="th center32" ItemStyle-CssClass="center24" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="lstHidePaymentPage" CssClass="cellLbl drpList " runat="server" onchange="javascript:EnableButton();" />
                                        <asp:Label Visible="false" runat="server" ID="lblHidePaymentPage" CssClass="cellLbl" Text='<%# Cstring(Eval("hidePaymentPage")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="th center32" ItemStyle-CssClass="center24" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="lstHidePrice" CssClass="cellLbl drpList " runat="server" onchange="javascript:EnableButton();" />
                                        <asp:Label Visible="false" runat="server" ID="lblHidePrice" CssClass="cellLbl" Text='<%# Eval("hidePrice") %>' />
                                    </ItemTemplate>
                                    <%--<FooterStyle HorizontalAlign="Right" />
                                    <FooterTemplate>
                                        <asp:Button ID="btnAddUser" CssClass="midButton columnBtn" OnClick="btnAddUser_Click" runat="server" Text="Add Row" />
                                    </FooterTemplate>--%>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="center4">
                                    <ItemTemplate>
                                       <asp:ImageButton ID="btnDelete" Tooltip="Clear row" CommandName="DELETE_ROW" CssClass="xxsImgBtn" ImageUrl='<%# GetWebImgUrl("close.jpg") %>' CommandArgument='<%# Eval("email") %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                           </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="lblNotFound" CssClass="gray" runat="server" Text="Not found"/>&nbsp;&nbsp;        
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <br />
                    </asp:Panel>
                    <asp:Label ID="lblRequired" CssClass="red" runat="server" Text="* Required"/>
                    <br />
                    <asp:Button ID="btnSaveCust" CssClass="midButton_gray Savebutton" runat="server" Text="Update Changes" Enabled="false" ValidationGroup="validgroupCompany"   />
                    <asp:Button ID="btnAddUser" CssClass="midButton" runat="server" Text="Add New User"   />
                    <br /><br />
                </asp:Panel>

           
               
            </div>

        </asp:Panel>

        <%----------------------------------------------------------------------------------------------------------------------
            Manage delivery schedule
        ----------------------------------------------------------------------------------------------------------------------%>
       

      
        <%----------------------------------------------------------------------------------------------------------------------
            Manage customer items
        ----------------------------------------------------------------------------------------------------------------------%>
     </div>
</div>
<asp:HiddenField ID="HiddevActiveTab" runat="server" Value="0" />
</asp:Content>
