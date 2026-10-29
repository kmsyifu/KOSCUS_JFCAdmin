#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\LoginType.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","78771DCB9A141E9B7B7A49535BC822381B4DFA36")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\LoginType.aspx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate

Partial Class Login_LoginType
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Dim p As Page = TryCast(Me.Context.Handler, Page)
        If p IsNot Nothing Then
            ' set master page
            If IsMobile() = True Then
                p.MasterPageFile = "Login_Mobile.master"
            Else
                p.MasterPageFile = "Login.master"
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim SQL As String
        'Dim dt As DataTable
        Dim userEmail As String
        Dim lbl As Label

        If IsInSession() = False Then Response.Redirect("Login.aspx")

        'Improve speed by disabling ViewState on this page (TODO: Doesn't work)
        'Page.EnableViewState = False

        Using DB As New DBHelper
            userEmail = GetUserEmail(DB)
            If IsPostBack = False Then
                'Init
                DataBind()
                'Get arguments
                ViewState("USER_TYPE") = Common.CString(Request.QueryString("type")).ToUpper
                ViewState("PAGE") = Request.QueryString("page")
                ViewState("RETURN_URL") = Request.QueryString("ReturnUrl")

                'Set properties
                pnlUserType_lst.Visible = True
                'pnlNotification.Visible = False

                ''Show dropdownlist of user type [id = usertype & companyid & iif(customer, custgrpid & customerid, '')]
                'SQL = "SELECT * FROM " & _
                '        "(SELECT 'CUST ' || u.custCompany_id || ' ' || u.customer_id AS id," & _
                '        " CASE WHEN u.customer_id = 'ADMIN' THEN c.custCompanyName || ' ' || 'ADMIN' " & _
                '        " ELSE (SELECT customerName || ' (' || a.jfcBranchNr || '-' || a.jfcCustomerNr || ')' FROM customer WHERE customer_id = u.customer_id) " & _
                '        "      || ' - Balance ' || (SELECT TO_CHAR(NVL(SUM(arAmt - recvAmt - discAmt), 0), '$999,999.00') FROM customer_jfcAR WHERE customer_id = u.customer_id) END AS name, 2 as dispOrder " & _
                '        "FROM webuser_customer u, custCompany c, customer a " & _
                '        "WHERE u.custCompany_id = c.custCompany_id AND u.customer_id <> 'NEW' AND u.customer_id = a.customer_id (+) AND u.email = " & DB.SqlEsc(userEmail) & _
                '        "UNION " & _
                '        "SELECT 'VENDOR ' || u.vendorCompany_id AS id," & _
                '        " c.vendorCompanyName || ' ' || 'ADMIN' AS name, 1 as dispOrder " & _
                '        "FROM webuser_vendor u, vendorCompany c " & _
                '        "WHERE u.vendorCompany_id = c.vendorCompany_id AND u.email = " & DB.SqlEsc(userEmail) & _
                '        ") WHERE 1 = 1 " & _
                '        IIf(ViewState("USER_TYPE") = "JFCADMIN", " AND id = 'VENDOR JFC' ", "") & _
                '        "ORDER BY dispOrder, name"
                ''        "SELECT 'VENDOR ' || vendorCompany_id || ' ' || u.vendor_id id," & _
                ''        " CASE WHEN vendor_id = 'ADMIN' THEN vendorCompany_id || ' ' || 'ADMIN' " & _
                ''        " ELSE (SELECT MAX(vendorName) FROM custGrp_vendor WHERE vendor_id = u.vendor_id) END AS name " & _
                ''        "FROM webuser_vendor u " & _
                'dt = DB.GetDataTable(SQL)
                'dt.Rows.InsertAt(dt.NewRow(), 0)
                'dt.Rows(0).Item(0) = "" 'id
                'dt.Rows(0).Item(1) = LST_CMD_STR 'name
                'lstUserType.DataSource = dt
                'lstUserType.DataValueField = "id" 'lstUserType.Text is id
                'lstUserType.DataTextField = "name"
                'lstUserType.DataBind()

                SQL = "SELECT * FROM " & _
                        "(SELECT 'VENDOR ' || u.vendorCompany_id AS id," & _
                        " 'Manage your accounts' AS name, " & _
                        " 1 as dispOrder, " & _
                        " NULL AS partAddress, NULL as fullAddress, NULL AS balance, " & _
                        " CASE WHEN u.vendorCompany_id = 'JFC' THEN " & _
                        "   (SELECT COUNT(o.webOrder_id) FROM webuser_customer u, customer a, sohdr o WHERE u.customer_id <> 'NEW' AND u.customer_id = a.customer_id AND a.customer_id = o.customer_id AND o.orderstate = 'N' AND u.email = " & DB.SqlEsc(userEmail) & ") " & _
                        "   ELSE NULL END AS jfcAdminPendingOrders " & _
                        "FROM webuser_vendor u, vendorCompany c " & _
                        "WHERE u.vendorCompany_id = c.vendorCompany_id AND u.email = " & DB.SqlEsc(userEmail) & _
                        ") WHERE 1 = 1 " & _
                        IIf(ViewState("USER_TYPE") = "JFCADMIN", " AND id = 'VENDOR JFC' ", "") & _
                        "ORDER BY dispOrder, name"
                '" c.vendorCompanyName || ' ' || 'ADMIN' AS name, " & _ 'TODO: Replace name to this later after adding more vendors such as PMAI
                dsUserType.ProviderName = "System.Data.OracleClient"
                dsUserType.ConnectionString = DB.GetConnectionString
                dsUserType.SelectCommand = SQL
                grdUserType.DataSourceID = "dsUserType"
                grdUserType.DataBind()

                'Select if only item
                If grdUserType.Rows.Count = 1 Then
                    lbl = grdUserType.Rows.Item(0).FindControl("lblUserTypeID")
                    selectUserType(DB, lbl.Text)
                    'selectUserType(DB, lbl.Text, True)
                End If
            End If

            'Used for drop down list
            'SelectIfOneItem(lstUserType)

            'If lstUserType.SelectedValue = "" Then
            '    'User type has not been selected
            '    If lstUserType.Items.FindByValue("VENDOR JFC") IsNot Nothing Then
            '        'For JFC Admin, select it by default
            '        'lstUserType.Items.FindByValue("VENDOR JFC").Selected = True 'TODO: Error occurs
            '        lstUserType.SelectedIndex = 1
            '    Else
            '        SetFocus(lstUserType)
            '    End If
            '    Exit Sub
            'End If

            'btnSelect_Click(Nothing, Nothing)
        End Using
    End Sub

    'Protected Sub btnSelect_Click(sender As Object, e As System.EventArgs) Handles btnSelect.Click
    '    Dim SQL As String
    '    Dim loginInfo As String()

    '    Using DB As New DBHelper()
    '        If lstUserType.Text = String.Empty Then
    '            MessageSimpleBox("Please select an account", DB)
    '            Exit Sub
    '        End If

    '        'Find URL
    '        loginInfo = Split(lstUserType.SelectedValue)
    '        If loginInfo(0) = "CUST" AndAlso loginInfo(2) = "ADMIN" Then
    '            'Customer admin login
    '            Session("SYS_SESSION_CUSTCOMPANYID") = loginInfo(1)
    '            SQL = "SELECT type_id FROM custCompany WHERE custCompany_id = :1"
    '            Session("SYS_SESSION_CUSTCOMPANYTYPEID") = CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTCOMPANYID"))))
    '            Session("SYS_SESSION_CUSTCOMPANYTYPE") = CaptalizeFirstLetter(GetCodeMaster(DB, "CUSTCOMPANY_TYPE", Session("SYS_SESSION_CUSTCOMPANYTYPEID")))
    '            Session("SYS_SESSION_USERTYPE") = "CUSTADMIN"
    '            Session("SYS_SESSION_DEFAULTURL") = "~/CustAdmin/Home.aspx"
    '        ElseIf loginInfo(0) = "CUST" AndAlso loginInfo(2) <> "ADMIN" Then
    '            'Customer login
    '            Session("SYS_SESSION_CUSTCOMPANYID") = loginInfo(1)
    '            Session("SYS_SESSION_CUSTGRPID") = GetCustGrpID(DB, loginInfo(2))
    '            Session("SYS_SESSION_CUSTID") = loginInfo(2)
    '            SQL = "SELECT type_id FROM custCompany WHERE custCompany_id = :1"
    '            Session("SYS_SESSION_CUSTCOMPANYTYPEID") = CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTCOMPANYID"))))
    '            Session("SYS_SESSION_CUSTCOMPANYTYPE") = CaptalizeFirstLetter(GetCodeMaster(DB, "CUSTCOMPANY_TYPE", Session("SYS_SESSION_CUSTCOMPANYTYPEID")))
    '            Session("SYS_SESSION_USERTYPE") = "CUSTOMER"
    '            Session("SYS_SESSION_DEFAULTURL") = "~/Customer/Home.aspx"
    '        ElseIf loginInfo(0) = "VENDOR" AndAlso loginInfo(1) = "JFC" Then
    '            'JFC admin login
    '            Session("SYS_SESSION_VENDORCOMPANYID") = loginInfo(1)
    '            Session("SYS_SESSION_USERTYPE") = "JFCADMIN"
    '            Session("SYS_SESSION_DEFAULTURL") = "~/JFCAdmin/" & IIf(ViewState("PAGE") <> "", ViewState("PAGE"), "Home.aspx")
    '        ElseIf loginInfo(0) = "VENDOR" AndAlso loginInfo(1) <> "JFC" Then
    '            'Non JFC vendor login
    '            Session("SYS_SESSION_VENDORCOMPANYID") = loginInfo(1)
    '            Session("SYS_SESSION_USERTYPE") = "VENDOR"
    '            Session("SYS_SESSION_DEFAULTURL") = "~/Vendor/Home.aspx"
    '        Else
    '            MessageSimpleBox("Cannot find your user type. Please contact JFC.", DB)
    '            Exit Sub
    '        End If

    '        'Move to next page
    '        InsertEventLog(DB, "Logged in", GetUserEmail(DB), Request.Browser.Browser & " " & Request.Browser.Version, IIf(IsMobile(), "MOBILE", "FULL"), True)
    '        If ViewState("RETURN_URL") <> String.Empty Then
    '            If ViewState("RETURN_URL").ToUpper.Contains("/" & Session("SYS_SESSION_USERTYPE") & "/") = True AndAlso System.IO.File.Exists(Server.MapPath(RemoveQueryStr(ViewState("RETURN_URL")))) = True Then
    '                Response.Redirect(ViewState("RETURN_URL"))
    '                Exit Sub
    '            End If
    '        End If
    '        Response.Redirect(Session("SYS_SESSION_DEFAULTURL"))
    '    End Using
    'End Sub

    Protected Sub grdUserType_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdUserType.RowDataBound
        'Highlight the mourse-over row and make it selectable
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.CssClass = "selectRow"
            e.Row.Attributes.Add("onmouseout", "this.className='selectRow';")
            e.Row.Attributes.Add("onmouseover", "this.className='selectRowHighlight';")
            'e.Row.Attributes.Add("onmouseover", "this.className='selectRowClickHighlight';")
            'Dim lnk As LinkButton = e.Row.FindControl("lnkSelectRow")
            'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackClientHyperlink(lnk, ""))
        End If
    End Sub

    Protected Sub grdUserType_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdUserType.RowCommand
        Dim row As GridViewRow = Nothing
        Dim index As Integer
        Dim lbl As Label

        Using DB As New DBHelper
            If e.CommandName = "LOGIN" Then
                index = Convert.ToInt32(e.CommandArgument)
                lbl = grdUserType.Rows.Item(index).FindControl("lblUserTypeID")
                selectUserType(DB, lbl.Text)
                'selectUserType(DB, lbl.Text, True)
            End If
        End Using
    End Sub

    'Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit.Click
    '    Dim userType As String = Session("SYS_SESSION_USERTYPE")
    '    Dim customerID As String = Session("SYS_SESSION_CUSTID")

    '    Using DB As New DBHelper
    '        'Check error
    '        If pnlPaperless.Visible = True Then
    '            If radPaperlessYes.Checked = False AndAlso radPaperlessNo.Checked = False Then
    '                MessageSimpleBox("Please select either '" & radPaperlessYes.Text & "' or '" & radPaperlessNo.Text & "'.", DB)
    '                Exit Sub
    '            End If
    '        End If
    '        If pnlEmailInvoice.Visible = True Then
    '            If radEmailInvoiceYes.Checked = False AndAlso radEmailInvoiceNo.Checked = False Then
    '                MessageSimpleBox("Please select either '" & radEmailInvoiceYes.Text & "' or '" & radEmailInvoiceNo.Text & "'.", DB)
    '                Exit Sub
    '            End If
    '        End If

    '        DB.BeginTransaction()

    '        'Set paperless
    '        If pnlPaperless.Visible = True Then DBUpdate.UpdatePaperless(DB, customerID, IIf(radPaperlessYes.Checked = True, "Y", "N"), False)
    '        If pnlEmailInvoice.Visible = True Then DBUpdate.UpdateEmailInvoice(DB, customerID, IIf(radEmailInvoiceYes.Checked = True, "Y", "N"), False)

    '        'Turn off ShowNotification
    '        If userType = "CUSTOMER" Then
    '            DBUpdate.UpdateShowNotification(DB, userType, customerID, "N", False)
    '        End If

    '        DB.CommitTransaction()

    '        'Move to next page
    '        selectUserType(DB, Nothing, False)
    '    End Using
    'End Sub

    'Protected Sub lnkNoChange_Click(sender As Object, e As System.EventArgs) Handles lnkNoChange.Click
    '    Using DB As New DBHelper
    '        'Move to next page
    '        selectUserType(DB, Nothing, False)
    '    End Using
    'End Sub

    'Private Sub selectUserType(ByVal DB As DBHelper, ByVal userTypeID As String, ByVal getLoginInfo As Boolean)
    Private Sub selectUserType(ByVal DB As DBHelper, ByVal userTypeID As String)
        Dim SQL As String
        Dim loginInfo As String()

        'If getLoginInfo = True Then
        'Find URL
        loginInfo = Split(userTypeID)
        If loginInfo(0) = "CUST" AndAlso loginInfo(2) = "ADMIN" Then
            'Customer admin login
            Session("SYS_SESSION_CUSTCOMPANYID") = loginInfo(1)
            SQL = "SELECT type_id FROM custCompany WHERE custCompany_id = :1"
            Session("SYS_SESSION_CUSTCOMPANYTYPEID") = CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTCOMPANYID"))))
            Session("SYS_SESSION_CUSTCOMPANYTYPE") = CaptalizeFirstLetter(GetCodeMaster(DB, "CUSTCOMPANY_TYPE", Session("SYS_SESSION_CUSTCOMPANYTYPEID")))
            Session("SYS_SESSION_USERTYPE") = "CUSTADMIN"
            Session("SYS_SESSION_DEFAULTURL") = "~/CustAdmin/Home.aspx"
        ElseIf loginInfo(0) = "CUST" AndAlso loginInfo(2) <> "ADMIN" Then
            'Customer login
            Session("SYS_SESSION_CUSTCOMPANYID") = loginInfo(1)
            Session("SYS_SESSION_CUSTGRPID") = GetCustGrpID(DB, loginInfo(2))
            Session("SYS_SESSION_CUSTID") = loginInfo(2)
            Session("SYS_SESSION_USERCOMPANYID") = loginInfo(3)
            SQL = "SELECT jfcPriceGrpID_orig FROM customer WHERE customer_id = :1"
            Session("SYS_SESSION_ISEXCEPPRICECUST") = IIf(Right(CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTID")))), 4) = "-REG", "N", "Y")
            SQL = "SELECT type_id FROM custCompany WHERE custCompany_id = :1"
            Session("SYS_SESSION_CUSTCOMPANYTYPEID") = CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTCOMPANYID"))))
            Session("SYS_SESSION_CUSTCOMPANYTYPE") = CaptalizeFirstLetter(GetCodeMaster(DB, "CUSTCOMPANY_TYPE", Session("SYS_SESSION_CUSTCOMPANYTYPEID")))
            Session("SYS_SESSION_USERTYPE") = "CUSTOMER"
            Session("SYS_SESSION_DEFAULTURL") = "~/Customer/Home.aspx"
            ''Show notification if any
            'SQL = "SELECT showNotification FROM customer WHERE customer_id = :1"
            'If CString(DB.GetScalar(SQL, CString(Session("SYS_SESSION_CUSTID")))) = "Y" Then
            '    showNotification(DB)
            '    Exit Sub
            'End If
        ElseIf loginInfo(0) = "VENDOR" AndAlso loginInfo(1) = "JFC" Then
            'JFC admin login
            Session("SYS_SESSION_VENDORCOMPANYID") = loginInfo(1)
            Session("SYS_SESSION_USERTYPE") = "JFCADMIN"
            Session("SYS_SESSION_DEFAULTURL") = "~/JFCAdmin/" & IIf(ViewState("PAGE") <> "", ViewState("PAGE"), "Home.aspx")
        ElseIf loginInfo(0) = "VENDOR" AndAlso loginInfo(1) <> "JFC" Then
            'Non JFC vendor login
            Session("SYS_SESSION_VENDORCOMPANYID") = loginInfo(1)
            Session("SYS_SESSION_USERTYPE") = "VENDOR"
            Session("SYS_SESSION_DEFAULTURL") = "~/Vendor/Home.aspx"
        Else
            MessageSimpleBox("Cannot find your user type. Please contact JFC.", DB)
            Exit Sub
        End If
        'End If

        'Move to next page
        InsertEventLog(DB, "Logged in", GetUserEmail(DB), Request.Browser.Browser & " " & Request.Browser.Version, IIf(IsMobile(), "MOBILE", "FULL"), True)
        If ViewState("RETURN_URL") <> String.Empty Then
            If ViewState("RETURN_URL").ToUpper.Contains("/" & Session("SYS_SESSION_USERTYPE") & "/") = True AndAlso System.IO.File.Exists(Server.MapPath(RemoveQueryStr(ViewState("RETURN_URL")))) = True Then
                Response.Redirect(ViewState("RETURN_URL"))
                Exit Sub
            End If
        End If
        Response.Redirect(Session("SYS_SESSION_DEFAULTURL"))
    End Sub

    'Private Sub showNotification(ByVal DB As DBHelper)
    '    Dim showPnl As Boolean = False
    '    Dim userType As String = Session("SYS_SESSION_USERTYPE")
    '    Dim customerID As String = Session("SYS_SESSION_CUSTID")

    '    'Set properties
    '    pnlUserType_lst.Visible = False
    '    pnlNotification.Visible = True
    '    pnlCustNotification.Visible = False
    '    If userType = "CUSTOMER" Then
    '        pnlCustNotification.Visible = True
    '    End If

    '    If pnlCustNotification.Visible = True Then
    '        pnlPaperless.Visible = False
    '        If Common.Paperless(DB, customerID) = False Then
    '            pnlPaperless.Visible = True
    '            showPnl = True
    '        End If
    '        pnlEmailInvoice.Visible = False
    '        If Common.EmailInvoice(DB, customerID) = False Then
    '            pnlEmailInvoice.Visible = True
    '            showPnl = True
    '        End If
    '    End If

    '    If showPnl = False Then
    '        'No need to show notification
    '        'Turn off ShowNotification
    '        If userType = "CUSTOMER" Then
    '            DBUpdate.UpdateShowNotification(DB, userType, customerID, "N")
    '        End If
    '        'Move to next page
    '        selectUserType(DB, Nothing, False)
    '    End If
    'End Sub

 End Class


#End ExternalSource
