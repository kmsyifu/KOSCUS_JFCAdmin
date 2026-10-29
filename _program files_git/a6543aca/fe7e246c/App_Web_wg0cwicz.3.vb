#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\Balance.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","D8287DEFBBBB7273FF761DD8C949C31BB9570BD9")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\Balance.aspx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate

Partial Class JFCAdmin_Balance
    Inherits System.Web.UI.Page
    Protected JavaScript As String = ""

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Dim p As Page = TryCast(Me.Context.Handler, Page)
        If p IsNot Nothing Then
            ' set master page
            If IsMobile() = True Then
                p.MasterPageFile = "JFCAdmin_Mobile.master"
            Else
                p.MasterPageFile = "JFCAdmin.master"
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'If page is refreshed, show the current page without executing the previous event
        'If IsPostBack = False Then
        '    'First load
        '    Session("POST_ID_" & Page.GetType().Name) = "1001"
        '    ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        'Else
        '    If Common.CString(ViewState("POST_ID_" & Page.GetType().Name)) = Common.CString(Session("POST_ID_" & Page.GetType().Name)) Then
        '        'Postback
        '        Session("POST_ID_" & Page.GetType().Name) = Common.CString(Convert.ToInt16(Session("POST_ID_" & Page.GetType().Name)) + 1)
        '        ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        '    Else
        '        'Refreshed 'Response.Write("refreshed")
        '        ViewState("POST_ID_" & Page.GetType().Name) = Common.CString(Session("POST_ID_" & Page.GetType().Name))
        '        Response.Redirect(Request.RawUrl)
        '    End If
        'End If

        'Show confirmation msg if needed
        If Session("MSG") <> "" Then
            If Session("MSG_TYPE") <> "" Then
                MessageLine(Session("MSG_TYPE"), Session("MSG"))
                Session("MSG_TYPE") = ""
            Else
                MessageLine("CONFIRM", Session("MSG"))
            End If
            Session("MSG") = ""
        End If

        If IsPostBack = False Then
            'Init
            DataBind()
            'Pass parameters to Balance
            Dim availTabs As New List(Of Integer)(New Integer() {Balance_Tab.BALANCE, Balance_Tab.PAYMENT, Balance_Tab.DOWNLOAD})
            Session("BALANCE_AVAIL_TABS") = availTabs
            'If [Enum].TryParse(Request.QueryString("TAB"), Session("BALANCE_TAB")) = False OrElse [Enum].IsDefined(TAB.GetType(), Session("BALANCE_TAB")) = False Then Session("BALANCE_TAB") = CustDtl_Tab.VENDOR
            Session("BALANCE_TAB") = IIf(Request.QueryString("TAB") <> "", Request.QueryString("TAB"), Balance_Tab.BALANCE)
            Session("BALANCE_CMD") = "JFCADMIN"

            'Select tab
            setJavaScript(Session("BALANCE_TAB"))
        End If
    End Sub

    Protected Sub ctrBalance_PreRender(sender As Object, e As System.EventArgs) Handles ctrBalance.PreRender
        'Select tab
        setJavaScript(Session("BALANCE_TAB"))
    End Sub

     Private Sub setJavaScript(ByVal tab As CustDtl_Tab)
        JavaScript = "$('#tabs').tabs('select', " & convertToLocalTab(tab) & ");" 'selectSubmenu(" & tab & ");"
        'JavaScript &= "scrollTo(0, 0);" 'TODO: Doesn't work
    End Sub

    Private Function convertToLocalTab(ByVal tab As CustDtl_Tab) As Integer
        Select Case tab
            Case Balance_Tab.BALANCE
                Return 0
            Case Balance_Tab.PAYMENT
                Return 1
            Case Balance_Tab.DOWNLOAD
                Return 2
        End Select
        Return 0
    End Function

    'Used for sending grdview through email in ctrBalance
    'As calling GridView.RenderControl(htmlTextWriter), the page raises an exception that a Server-Control was rendered outside of a Form. Avoid this execption by overriding VerifyRenderingInServerForm
    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub
End Class


#End ExternalSource
