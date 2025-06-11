<%@ Page Language="VB" MasterPageFile="JFCAdmin.master" AutoEventWireup="false" CodeFile="Tool.aspx.vb" Inherits="Tool"  %>
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
         function jsShowHideProgress() {
          var myVar = setInterval(function () {
                    var modal = $('<div />');
                    modal.addClass("modal");
                    $('body').append(modal);
                    var loading = $(".loading");
                    loading.show();
                    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                    loading.css({ top: top, left: left });
                    if (IsCookieValid())
                      { loading.hide();
                        $('.modal').hide();
                      clearInterval(myVar);}
              }, 200);
              //stopShowProgress();
            deleteCookie();
        }
        function stopShowProgress(){
           clearTimeout(myVar);
        }
           
        // cookies
        function deleteCookie() {
            var cook = getCookie('ExcelDownloadFlag');
            if (cook != "") {
                document.cookie = "ExcelDownloadFlag=;Path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC";
            }
        }

        function IsCookieValid() {
            var cook = getCookie('ExcelDownloadFlag');
            return cook != '';
        }

        function getCookie(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                console.log(c);
                while (c.charAt(0) == ' ') {
                    c = c.substring(1);
                }
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        }                      
        $(document).ready(function () {
           
            //$("#tabs").tabs();
            $('#tabs').tabs();
            $("#tabs").removeClass("ui-widget");
            $('#tabs').css('display', 'block'); //Display tabs when page finish loading
          
           
        
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
    
    <div id="Alltab"  runat ="server"  >
        <div id="tabs" style="display: none;"> <%--Display tabs when page finish loading--%> 
            <ul>
                <li><a href="#tabReport">Reports</a></li>
            </ul>
    

          <%----------------------------------------------------------------------------------------------------------------------
            Homepage Images Picture
        ----------------------------------------------------------------------------------------------------------------------%>
         <asp:Panel ID="pnlTabReport" runat="server">
            <div id="tabReport">
             <h2>Reports </h2>
            <br />
     
                <asp:Panel ID="pnlpage_Keyword" runat="server" DefaultButton ="btnExportExcel" >
                          
                      <table>
                       <tr>
                                            <td class="rowHdr">Report</td>
                                            <td><asp:DropDownList ID="lstKeywordReport" AutoPostBack="true" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' 
                                             runat="server" /></td>
                                        </tr>
                                        <%--OnChange="javascript:showProgress();" --%>
                                        <tr>
                                            <td class="rowHdr"><asp:Label ID="lblBranch" runat="server" Text='Branch'  CssClass="gray" /></td>
                                            <td><asp:DropDownList ID="lstBranch" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>'   runat="server"  /></td>
                                        </tr>
                                        <asp:Panel runat="server" ID = "pnlDateFromTo">
                                        <tr> 
                                            <td class="rowHdr">From</td>
                                            <td>
                                              <asp:TextBox ID="txtFromDate" ValidationGroup="validgroup" CssClass="stdTxt size12" Text="" runat="server" autocomplete="off" />&nbsp;(mm/dd/yyyy) 
                                              <ajaxToolKit:CalendarExtender ID="calFromDate"  Format="MM/dd/yyyy" TargetControlID="txtFromDate" runat="server"   />&nbsp;&nbsp;&nbsp;
                                              
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr">To</td>
                                            <td> <asp:TextBox ID="txtToDate" ValidationGroup="validgroup" CssClass="stdTxt size12" Text="" runat="server" autocomplete="off"/>&nbsp;(mm/dd/yyyy) 
                                                <ajaxToolKit:CalendarExtender ID="calToDate"  Format="MM/dd/yyyy" TargetControlID="txtToDate" runat="server"   />&nbsp;&nbsp;&nbsp;
                                                 
                                            </td>
                                        </tr>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlItemCode">
                                         <tr>
                                            <td class="rowHdr">Item #</td>
                                            <td>
                                                <asp:TextBox ID="txtItemcode" CssClass='<%# "stdTxt " & IIf(IsMobile(), "size24", "size48") %>'  Text="" runat="server" autocomplete="off"/>&nbsp;
                                                <asp:Label ID="Label1" CssClass="red" runat="server" Text="*"/> &nbsp;                                          
                                            </td>
                                        </tr>
                                        <tr>
                                           <td class="rowHdr"></td>
                                            <td><span class="gray">&nbsp;Use "," to enter multiple items.</span></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="red"><asp:Label ID="Label33" CssClass="red" runat="server" Text="* Required"/></td>
                                        </tr>
                                        </asp:Panel>
                                        <tr><td colspan="2">
                                            <asp:button ID= "btnExportExcel" runat = "server" CssClass=" midButton" OnClientClick="javascript:jsShowHideProgress();"  Text= "Download"  />
                                        </td></tr>
                                        </table>
                                </asp:Panel>
                                        <%-- tr>
                                            <td colspan="2" class="red"><asp:Label ID="Label33" CssClass="red" runat="server" Text="* Required"/></td>
                                            
                                        </tr>-<tr>
                                            <td class="rowHdr">Send Reminder</td> OnClientClick="javascript:jsShowHideProgress();" 
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="rowHdr">Schedule Name</td>
                                            <td></td>
                                        </tr>-%>
                                        <tr>
                                            <td colspan="2" class="center">
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                   <asp:Label ID="lblKeywordReportName" runat="server" Text='Report'  CssClass="gray" />
                   
                   <br />
                    <%--  <asp:Label ID="lblBranch" runat="server" Text='Branch'  CssClass="gray" />
                    <asp:DropDownList ID="lstBranch" CssClass='<%# "drpList " & IIf(IsMobile(), "size24", "size48") %>' OnChange="javascript:showProgress();" runat="server" AutoPostBack="True" />
                    
                    <br />
                    <asp:Label Id ="lblFromdate" runat="server" CssClass="gray" Text ='From'></asp:Label>
                    <asp:TextBox ID="txtFromDate" ValidationGroup="validgroup" CssClass="stdTxt size12" Text="01/01/2013" runat="server" />&nbsp;(mm/dd/yyyy) <%--Text required so that validator wouldn't work by default-
                    <ajaxToolKit:CalendarExtender ID="calFromDate"  Format="MM/dd/yyyy" TargetControlID="txtFromDate" runat="server"  />&nbsp;&nbsp;&nbsp;
                   
                    
                     <br />
                     <asp:Label Id ="lblTodate" runat="server" CssClass="gray" Text ='To'></asp:Label>
                     <asp:TextBox ID="txtToDate" ValidationGroup="validgroup" CssClass="stdTxt size12" Text="01/01/2013" runat="server" />&nbsp;(mm/dd/yyyy) 
                     <ajaxToolKit:CalendarExtender ID="calToDate"  Format="MM/dd/yyyy" TargetControlID="txtToDate" runat="server"  />&nbsp;&nbsp;&nbsp;
                            <br /> 
                     <br />
                   
                    <asp:ImageButton ID="imgbtnExcel" runat="server" ImageUrl='<%# GetWebImgUrl("Excel.png") %>' Tooltip="Download Report" CssClass=" lImgBtn" Visible ="false"  />

                </asp:Panel>--%>
              
            </div>
         </asp:Panel>
      </div> 
    </div>  
</asp:Content>
