<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Error.aspx.vb" Inherits="Errors_Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:PlaceHolder ID="phError" runat="server">
        <h1>An error has occurred</h1>
        <p>An unexpected error occurred on this website. The website administrator has been notified.</p>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phBusy" runat="server" Visible="false">
        <h1>The database is busy</h1>
        <p>Your request took too long and was stopped. Please wait a moment and try again.</p>
        <p><a href="javascript:history.back()">Go back</a></p>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phConnection" runat="server" Visible="false">
        <h1>The connection to the database was lost</h1>
        <p>The connection has been reset. Please try again.</p>
        <p><a href="javascript:history.back()">Go back</a></p>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phDbDown" runat="server" Visible="false">
        <h1>The database is not available</h1>
        <p>The website cannot reach the database right now. Please wait a few minutes and try again.</p>
        <p><a href="javascript:history.back()">Go back</a></p>
        </asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
