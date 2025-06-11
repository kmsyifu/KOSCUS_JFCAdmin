/************************** Highlight Row when checkbox is checked ***************************/
function highlightRow(objRef) { // TODO: This function doesn't get called
    //Get the Row based on checkbox
    var row = objRef.parentNode.parentNode.parentNode; //objRef.parentNode.parentNode;
    if(objRef.checked)
    {
        //If checked change color to orange
        row.style.backgroundColor = "#C0C0C0";
    }
    else
    {   
        //If not checked change back to original color
        row.style.backgroundColor = "#FFFFFF";
    }
   
    //Get the reference of GridView
    var GridView = row.parentNode;
   
    //Get all input elements in Gridview
    var inputList = GridView.getElementsByTagName("input");
   
    for (var i=0;i<inputList.length;i++)
    {
        //The First element is the Header Checkbox
        var headerCheckBox = inputList[0];
       
        //Based on all or none checkboxes are checked, check/uncheck Header Checkbox
        var checked = true;
        if(inputList[i].type == "checkbox" && inputList[i] != headerCheckBox)
        {
            if(!inputList[i].checked)
            {
                checked = false;
                break;
            }
        }
    }
    headerCheckBox.checked = checked;
}

/************************** Check all checkboxes ***************************/
function checkAll(objRef) 
{
    var GridView = objRef.parentNode.parentNode.parentNode; //objRef.parentNode.parentNode;
    var inputList = GridView.getElementsByTagName("input");
    for (var i = 0; i < inputList.length; i++) 
    {
        //Get the Cell To find out ColumnIndex
        var row = inputList[i].parentNode.parentNode.parentNode;
        if (inputList[i].type == "checkbox" && objRef != inputList[i]) 
        {
            if (objRef.checked) 
            {
                //If checkbox is disabled, ignore
                if (inputList[i].disabled == true) { continue; }
                //If the header checkbox is checked, check all checkboxes and highlight all rows
                //row.style.backgroundColor = "#C0C0C0";
                inputList[i].checked = true;
            }
            else 
            {
                //If checkbox is disabled, ignore
                if (inputList[i].disabled == true) { continue; }
                //If the header checkbox is unchecked, uncheck all checkboxes and change rowcolor back to original
                //row.style.backgroundColor = "#FFFFFF";
                inputList[i].checked = false;
            }
        }
    }
}

/************************** Show / hide message on the top and bottom of page ***************************/
//function replaceTag() {
//    var str = document.getElementById('<%=txtMsg.ClientID%>').innerHTML;
//    var res = str.replace(/</g, "(");
//    var res = res.replace(/>/g, ")");
//    document.getElementById('<%=txtMsg.ClientID%>').innerHTML = res;
//};

/************************** Replace '<' with '(' and '>' with ')' ***************************/
function replaceBrackets(str) {
    str.value = str.value.replace(/\</g, '(');
    str.value = str.value.replace(/\>/g, ')');
}

/************************** Show / hide message on the top and bottom of page ***************************/
function setOpacity(div, opacity) {
    opacity = (opacity == 100) ? 99.999 : opacity;
    // IE/Win
    div.style.filter = "alpha(opacity:" + opacity + ")";
    // Safari<1.2, Konqueror
    div.style.KHTMLOpacity = opacity / 100;
    // Older Mozilla and Firefox
    div.style.MozOpacity = opacity / 100;
    // Safari 1.2, newer Firefox and Mozilla, CSS3
    div.style.opacity = opacity / 100;
}
function fadeIn(objID, opacity) {
    if (document.getElementById) {
        div = document.getElementById(objID);
        //if (opacity <= 100) {
        if (opacity <= 90) {
            setOpacity(div, opacity);
            opacity += 10;
            window.setTimeout("fadeIn('" + objID + "'," + opacity + ")", 100);
        }
    }
}
function fadeOut(objID, opacity) {
    if (document.getElementById) {
        div = document.getElementById(objID);
        if (opacity >= 0) {
            setOpacity(div, opacity);
            opacity -= 10;
            window.setTimeout("fadeOut('" + objID + "'," + opacity + ")", 100);
        } else {
            div.style.visibility = "hidden";
        }
    }
}
//function showMsg(type, msg) {
//    var objID;
//    var div;
//    for (var i = 0; i < 2; i++) {
//        if (i == 0) {
//            // Show msg on top of page
//            objID = "divPageMsg_top";
//            div = document.getElementById(objID);
//            div.style.top = "0px";
//        } else {
//            // Show msg on bottom of page
//            objID = "divPageMsg_btm";
//            div = document.getElementById(objID);
//            div.style.bottom = "0px";
//        }
//        div.style.visibility = "visible";
//        fadeIn(objID, 0);
//        div.style.height = "20px";
//        div.style.width = "100%";
//        div.style.zindex = "200"; //TODO: Can't show div in front of tabs
//        div.style.padding = "10px";
//        div.style.position = "fixed"; // "relative";
//        div.innerHTML = msg;
//        if (type == "ERROR") {
//            div.className = "errorMsg";
//            if (document.getElementById("divHdrLink") != null) { document.getElementById("divHdrLink").style.visibility = "hidden"; } // Hide header links 
//        }
//        else if (type == "WARNING") {
//            div.className = "warnMsg";
//            if (document.getElementById("divHdrLink") != null) { document.getElementById("divHdrLink").style.visibility = "hidden"; } // Hide header links 
//        }
//        else if (type == "CONFIRM") {
//            div.className = "confirmMsg";
//            //setTimeout("hideMsg()", 5000);
//            setTimeout("fadeOut('" + objID + "'," + 90 + ")", 5000);
//        }
//    }
//    //var objID = "tabs";
//    //var div = document.getElementById(objID);
//    //div.style.zindex = 0;
//}

/*function hideMsg() {
    var div = document.getElementById("divPageMsg");

    fadeOut("divPageMsg", 100);
    div.style.visibility = "hidden";
    div.style.height = "0px";
    div.style.padding = "0px";
}*/

/************************** Show/close popup ***************************/
function showPopup(type) {
    //window.showModalDialog("UserDetails.aspx", 'UserDetails', 'height=600,width=600');
    $find(type).show();
}

function closePopup(type) {
    $find(type).hide(); 
    return false;
}

function showPopupWindow(url) {
    w_width = 1024;
    w_height = 640;

    left_pos = (screen.width) ? (screen.width - w_width) / 2 : 100;
    top_pos = (screen.height) ? (screen.height - w_height) / 2 : 100;
    //settings = 'width=' + w_width + ',height=' + w_height + ',top=' + top_pos + ',left=' + left_pos + ',scrollbars=yes,location=yes,directories=yes,status=yes,menubar=yes,toolbar=yes,resizable=yes';
    settings = 'width=' + w_width + ',height=' + w_height + ',top=' + top_pos + ',left=' + left_pos + ',scrollbars=yes,location=no, directories=no, status=no, menubar=yes, toolbar=no, resizable=yes';
    window.open(url, "list", settings);
}

/************************** Show progress ***************************/
function showProgress() {
    setTimeout(function () {
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        var loading = $(".loading");
        loading.show();
        var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
        loading.css({ top: top, left: left });
    }, 200);
}

//function showProgress() {
//    if (document.getElementById) {
//        var progress = document.getElementById('divProgress');
//        var blur = document.getElementById('divBlur');
//        var pageHeight = Math.max(document.body.clientHeight, document.body.scrollHeight, document.body.offsetHeight,
//                       document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight);
//        if (progress == null || blur == null) return false;
//        progress.style.width = '400px';
//        progress.style.height = '150px';
//        progress.style.top = document.documentElement.scrollHeight/3 - progress.style.height.replace('px','')/2 + 'px';
//        progress.style.left = document.body.offsetWidth/2 - progress.style.width.replace('px','')/2 + 'px';
//        //progress.style.top = document.documentElement.clientHeight / 3 - progress.style.height.replace('px', '') / 2 + 'px';
//        //progress.style.left = document.body.offsetWidth / 2 - progress.style.width.replace('px', '') / 2 + 'px';

//        //blur.style.height = document.documentElement.scrollHeight;
//        blur.style.height = pageHeight + 'px';
//    }
//}

/************************** Validation ***************************/
function validateQty(oSrc, args) {
    args.IsValid = !isNaN(parseFloat(args.Value)) && isFinite(args.Value) && args.Value >= 0;
}
function validateDate(oSrc, args) {
    var validformat = /^\d{2}\/\d{2}\/\d{4}$/
    //Basic check for format validity
    if (!validformat.test(args.Value))
        args.IsValid = false;
    else {
        //Detailed check for valid date ranges
        var monthfield = args.Value.split("/")[0]
        var dayfield = args.Value.split("/")[1]
        var yearfield = args.Value.split("/")[2]
        var dayobj = new Date(yearfield, monthfield - 1, dayfield)
        if ((dayobj.getMonth() + 1 != monthfield) || (dayobj.getDate() != dayfield) || (dayobj.getFullYear() != yearfield))
            args.IsValid = false;
        else
            args.IsValid = true;
    }
}

/*function addItemDtl(itemDesc) {
var lbl = document.getElementById('lblItemDesc2');
lbl.value = itemDesc;
}*/

/************************** Open calendar ***************************/
//function calendar_pop(w_url) {
//    w_width = 250;
//    w_height = 250;

//    left_pos = (screen.width) ? (screen.width - w_width) / 2 : 100;
//    top_pos = (screen.height) ? (screen.height - w_height) / 2 : 100;
//    //settings = 'width=' + w_width + ',height=' + w_height + ',top=' + top_pos + ',left=' + left_pos + ',scrollbars=yes,location=yes,directories=yes,status=yes,menubar=yes,toolbar=yes,resizable=yes';
//    settings = 'width=' + w_width + ',height=' + w_height + ',top=' + top_pos + ',left=' + left_pos + ',scrollbars=no, location=no, directories=no, status=no, menubar=no, toolbar=no, resizable=no';
//    window.open(w_url, "calendar", settings);
//}

/************************ Get Browser Height/Width *************************************/
function getBrowserHeight() {
    if (window.innerHeight) {
        return window.innerHeight;
    }
    else if (document.documentElement && document.documentElement.clientHeight != 0) {
        return document.documentElement.clientHeight;
    }
    else if (document.body) {
        return document.body.clientHeight;
    }
    return 0;
}
function getBrowserWidth() {
    if (window.innerWidth) {
        return window.innerWidth;
    }
    else if (document.documentElement && document.documentElement.clientWidth != 0) {
        return document.documentElement.clientWidth;
    }
    else if (document.body) {
        return document.body.clientWidth;
    }
    return 0;
}

/************************ Reset scroll position to top of the page *************************************/
//function resetScrollPosition() {
//    var scrollX = document.getElementById('__SCROLLPOSITIONX');
//    var scrollY = document.getElementById('__SCROLLPOSITIONY');

//    if (scrollX != null && scrollY != null) {
//        scrollX.value = 0;
//        scrollY.value = 0;
//    }
//}

/************************** To add 'TextChange' event on Jquery ***************************/
/*!
* jQuery TextChange Plugin
* http://www.zurb.com/playground/jquery-text-change-custom-event
*
* Copyright 2010, ZURB
* Released under the MIT License
*/
(function (a) {
    a.event.special.textchange = { setup: function () { a(this).data("lastValue", this.contentEditable === "true" ? a(this).html() : a(this).val()); a(this).bind("keyup.textchange", a.event.special.textchange.handler); a(this).bind("cut.textchange paste.textchange input.textchange", a.event.special.textchange.delayedHandler) }, teardown: function () { a(this).unbind(".textchange") }, handler: function () { a.event.special.textchange.triggerIfChanged(a(this)) }, delayedHandler: function () {
        var c = a(this); setTimeout(function () { a.event.special.textchange.triggerIfChanged(c) },
 25)
    }, triggerIfChanged: function (a) { var b = a[0].contentEditable === "true" ? a.html() : a.val(); b !== a.data("lastValue") && (a.trigger("textchange", [a.data("lastValue")]), a.data("lastValue", b)) } 
    }; a.event.special.hastext = { setup: function () { a(this).bind("textchange", a.event.special.hastext.handler) }, teardown: function () { a(this).unbind("textchange", a.event.special.hastext.handler) }, handler: function (c, b) { b === "" && b !== a(this).val() && a(this).trigger("hastext") } }; a.event.special.notext = { setup: function () {
        a(this).bind("textchange",
 a.event.special.notext.handler)
    }, teardown: function () { a(this).unbind("textchange", a.event.special.notext.handler) }, handler: function (c, b) { a(this).val() === "" && a(this).val() !== b && a(this).trigger("notext") } 
    }
})(jQuery);


/************************ Small, private functions *************************************/
$(document).ready(function () {

    /********** Modal message box ***********/
    if (SystemDialogMessage != '') {
        $('form').append('<div id="systemdialog" title="' + SystemDialogTitle + '"><p>' + SystemDialogMessage + '</p></div>');

        if (SystemDialogButton == '') {
            $("#systemdialog").dialog({ modal: true,
                //                close: function (event, ui) {
                //                    if (SystemDialogReturnURL != '') {
                //                        window.location = SystemDialogReturnURL;
                //                    }
                //                }
                buttons: {
                    "OK": function (event) { $("#systemdialog").dialog('close'); }
                }
            });
        }
        if (SystemDialogButton == 'OKCancel') {
            $("#systemdialog").dialog({ modal: true,
                buttons: {
                    "OK": function (event) { $('#DialogResult').val("OK"); document.forms[0].submit(); },
                    "Cancel": function () { $('#DialogResult').val("CANCEL"); document.forms[0].submit(); }
                }
            });
        }
        if (SystemDialogButton == 'YesNo') {
            $("#systemdialog").dialog({ modal: true,
                buttons: {
                    "Yes": function (event) { $('#DialogResult').val("YES"); document.forms[0].submit(); },
                    "No": function () { $('#DialogResult').val("NO"); document.forms[0].submit(); }
                }
            });
        }
        // Make it dispear in 3 seconds
        //var xSeconds = 3;
        //setTimeout(function () { $("#systemdialog").dialog('close'); }, xSeconds * 1000);
    }

    /********** Show message line on top of page ***********/
    if (SystemLineMessage != '') {
        if (SystemLineType == 'CONFIRM') {
            $('form').append('<div id="systemline_confirm">' + SystemLineMessage + '</div>');
            //$('#systemline_confirm').show('', '', 1000);
            fadeIn('systemline_confirm', 0);
            //setTimeout(function () { $("#systemline_confirm").hide(); }, 5000);
            setTimeout("fadeOut('systemline_confirm'," + 90 + ")", 5000);
        } else if (SystemLineType == 'WARNING') {
            $('form').append('<div id="systemline_warning">' + SystemLineMessage + '</div>');
            fadeIn('systemline_warning', 0);
        } else if (SystemLineType == 'ERROR') {
            $('form').append('<div id="systemline_error">' + SystemLineMessage + '</div>');
            fadeIn('systemline_error', 0);
        }
    }

    /********** To change background color on text field **********/
    /* Commented out so that onblur="javascript:replaceBrackets(this)" would work */
    /*for (i = 0; i < document.forms[0].elements.length; i++) {
    var c = document.forms[0].elements[i];
    if (c.type == "text" || c.type == "textarea") {
    c.onfocus = input_focused;
    c.onblur = input_blured;
    if (c == document.activeElement) {
    c.style.backgroundColor = "#DFF";
    c.select();
    }
    }
    }*/

    /********** Dropdown menu **********/
    $("ul.dropdown li").hover(function () {
        $(this).addClass("hover");
        $('ul:first', this).css('visibility', 'visible');
    }, function () {
        $(this).removeClass("hover");
        $('ul:first', this).css('visibility', 'hidden');
    });
    $("ul.dropdown li ul li:has(ul)").find("a:first").append(" &raquo; ");


    $("input[type=image]").each(function () {
        $(this).hover(function () {
            var url = $(this).attr("src");
            if (url.substr(url.length - 5) == "1.png") { url = url.substr(0, url.length - 5) + "2.png"; }
            $(this).attr("src", url);
        }, function () {
            var url = $(this).attr("src");
            if (url.substr(url.length - 5) == "2.png") { url = url.substr(0, url.length - 5) + "1.png"; }
            $(this).attr("src", url);
        });
    });

});

function input_focused() {
    this.style.backgroundColor = "#DFF";
    //this.select();
}

function input_blured() {
    this.style.backgroundColor = "";
}

function TrimInput() {
    for (i = 0; i < document.forms[0].elements.length; i++) {
        var c = document.forms[0].elements[i];
        if (c.type == "text") {
            c.value = jQuery.trim(c.value);
        }
    }
}

function getIndex(input) {
    var index = -1, i = 0, found = false;
    var form = document.forms[0];
    while (i < form.length && index == -1)
        if (form[i] == input) index = i; else i++;
    return index;
}

function ChgKeyCode(e) {
    if (document.all) {
        if (!(event.srcElement.type == "button" || event.srcElement.type == "submit")) {
            if( event.keyCode == 13) {
                event.keyCode = 9;
            }
        }
    } else {
        if (!(e.target.type == "button" || e.target.type == "submit")) {
            if (e.which == 13) {
                var form = document.forms[0];
                form.elements[(getIndex(this) + 1) % form.length].focus();
                return false;
            }
        }
    }
}

(function ($) {
    $.fn.bubbleup = function (options) {
        //Extend the default options of plugin
        var opt = $.extend({}, $.fn.bubbleup.defaults, options), tip = null;

        return this.each(function () {
            var w = $(this).width();
            $(this).mouseover(function () {
                if (opt.tooltip) {
                    tip = $('<div>' + $(this).attr('alt') + '</div>').css({
                        fontFamily: opt.fontFamily,
                        color: opt.color,
                        fontSize: opt.fontSize,
                        fontWeight: opt.fontWeight,
                        position: 'absolute',
                        zIndex: 100000
                    }).remove().css({ top: 0, left: 0, visibility: 'hidden', display: 'block' }).appendTo(document.body);
                    var position = $.extend({}, $(this).offset(), { width: this.offsetWidth, height: this.offsetHeight }), tipWidth = tip[0].offsetWidth, tipHeight = tip[0].offsetHeight;
                    tip.stop().css({
                        top: position.top - tipHeight,
                        left: position.left + position.width / 2 - tipWidth / 2,
                        visibility: 'visible'
                    }).animate({ top: '-=' + (opt.scale / 2 - w / 2) }, opt.inSpeed);
                }

                $(this).closest('li').css({ 'z-index': 100000 });

                $(this).stop().css({ 'z-index': 100000, 'top': 0, 'left': 0, 'width': w }).animate({
                    left: -opt.scale / 2 + w / 2,
                    top: -opt.scale / 2 + w / 2,
                    width: opt.scale
                }, opt.inSpeed);
            }).mouseout(function () {

                $(this).closest('li').css({ 'z-index': 100 });
                $(this).closest('li').next().css({ 'z-index': 0 });
                $(this).closest('li').next().children('img').css({ 'z-index': 0 });

                if (opt.tooltip) { tip.remove() }
                $(this).stop().animate({ left: 0, top: 0, width: w }, opt.outSpeed, function () {
                    $(this).css({ 'z-index': 0 });

                });
            })
        })
    }
    $.fn.bubbleup.defaults = {
        tooltip: false,
        scale: 96,
        fontFamily: 'Helvetica, Arial, sans-serif',
        color: '#333333',
        fontSize: 12,
        fontWeight: 'bold',
        inSpeed: 'fast',
        outSpeed: 'fast'
    }
})(jQuery);
