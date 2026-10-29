#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\BranchSetting.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","4DF68418B3971482DC3575FBB8D77E6BA0C68598")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\BranchSetting.aspx.vb",1)
Imports System.Data
Imports Common
Imports DBUpdate
Imports AjaxControlToolkit
Imports System.IO
Imports Winnovative.ExcelLib
Imports System.Globalization
Imports System.Threading
Imports System.Resources
Imports System.Net
Imports System.Web.Script.Serialization
Imports System.Web.Services

Partial Class BranchSetting
    Inherits System.Web.UI.Page
    Protected JavaScript As String = ""
    Private rm As ResourceManager = New ResourceManager("Resources.Strings", System.Reflection.Assembly.Load("App_GlobalResources"))
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

        Dim SQL As String
        Dim email As String

        Using DB As New DBHelper
            'Post process for modal dialog box
            If Request("DialogResult") = "YES" Then
                dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"))
            ElseIf Request("DialogResult") = "NO" Then
                dialogResult_no(ViewState("DIALOG_CMD"))
            End If

            'If Session("MASTER_TAB") Is Nothing Then
            '    Session("MASTER_TAB") = If(Not String.IsNullOrEmpty(TAB), TAB, "0")
            'End If



            If IsPostBack = False Then
                'Init
              

                DataBind() 'Fill in customer company type

                ''Show company list
                email = GetUserEmail(DB)
                If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                    ShowBranchList(DB, email, True, lstBranch)
                Else
                    ShowBranchList(DB, email, False, lstBranch)
                End If

                lstImgCtg.Items.Clear()
                lstImgCtg.Items.Add(New ListItem("Promotion", "PROMOTION"))
                lstImgCtg.Items.Add(New ListItem("New Items", "NEW")) 'Added vendorName to show formHdr in correct place
                lstImgCtg.Items.Add(New ListItem("Recipe", "RECIPE"))


                'Get parameters
                If Request.QueryString("branch") <> "" Then lstBranch.Text = Request.QueryString("branch")
                ViewState("MASTER_TAB") = IIf(Request.QueryString("TAB") <> "", Request.QueryString("TAB"), ViewState("MASTER_TAB"))
                ViewState("IMGCTG_ID") = IIf(Request.QueryString("IMGCTG_ID") <> "", Request.QueryString("IMGCTG_ID"), Nothing)
                lstImgCtg.SelectedValue = ViewState("IMGCTG_ID")
                hfActiveTab.Value = ViewState("MASTER_TAB")


                Select ViewState("MASTER_TAB")
                    Case Master_Tab.PROMOTIONITEM
                        showPromoItemsList()
                      
                    Case Master_Tab.HOMEPAGEIMG
                        showHomePageIMGList()
                    Case Master_Tab.NOTIFICATION
                        showOrderReminderList()
                    Case Master_Tab.DELIVERYMASTER

                        showDeliveryMasterList()
                    Case Else
                        showPromoItemsList()
                        showHomePageIMGList()
                        showOrderReminderList()
                        showDeliveryMasterList()
                End Select













                '--------------------------------------------------------------------------------------------------------------------
                'Keyword Form
                '--------------------------------------------------------------------------------------------------------------------

                'ViewState("IsExcepPriceCustomer") = Common.IsExcepPriceCustomer(DB, ViewState("CUSTSETTING_CUSTID"))

                'Dim eventTarget As String = If(Request("__EVENTTARGET"), String.Empty)
                'Dim eventArgument As String = If(Request("__EVENTARGUMENT"), String.Empty)

                'If eventTarget = "MyPostbackControl" Then
                '    ' Handle the postback and use the passed value
                '    ViewState("MASTER_TAB") = eventArgument
                'End If
                'Select Case ViewState("MASTER_TAB")
                '    Case Master_Tab.PROMOTIONITEM
                '        showPromoItemsList()
                '    Case Master_Tab.HOMEPAGEIMG
                '        showHomePageIMGList()
                '    Case Master_Tab.NOTIFICATION
                '        showOrderReminderList()
                '    Case Master_Tab.DELIVERYMASTER
                '        'Select tab
                '        showDeliveryMasterList()
                '    Case Else
                '        showPromoItemsList()
                'End Select

            Else
                Dim eventTarget As String = If(Request("__EVENTTARGET"), String.Empty)
                Dim eventArgument As String = If(Request("__EVENTARGUMENT"), String.Empty)

                If eventTarget = "TabChanged" Then
                    Dim tabIndex As Integer
                    If Integer.TryParse(eventArgument, tabIndex) Then
                        ' Handle tab change here
                        hfActiveTab.Value = tabIndex.ToString()
                        ViewState("MASTER_TAB") = tabIndex
                        If hfPrevTab.Value <> tabIndex.ToString() Then



                            hfPrevTab.Value = tabIndex.ToString()

                            Select Case ViewState("MASTER_TAB")
                                Case Master_Tab.PROMOTIONITEM
                                    showPromoItemsList()

                                Case Master_Tab.HOMEPAGEIMG
                                    showHomePageIMGList()
                                Case Master_Tab.NOTIFICATION
                                    showOrderReminderList()
                                Case Master_Tab.DELIVERYMASTER

                                    showDeliveryMasterList()
                                Case Else
                                    showPromoItemsList()
                                    showHomePageIMGList()
                                    showOrderReminderList()
                                    showDeliveryMasterList()
                            End Select
                        End If

                        ' You can also run any other logic you need here

                    End If
                End If


            End If
            'setJavaScript(ViewState("MASTER_TAB"))

        End Using
    End Sub

    Protected Sub lstBranch_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstBranch.SelectedIndexChanged
        'If lstCustCompany.Text <> "" Then
        '    lblTitle.Visible = False
        '    Alltab.Visible = True
        'End If
        ViewState("IMGCTG_ID") = lstImgCtg.SelectedValue
        Response.Redirect("BranchSetting.aspx?branch=" & lstBranch.Text & "&tab=" & ViewState("MASTER_TAB") & "&IMGCTG_ID=" & ViewState("IMGCTG_ID"))

    End Sub

    Protected Sub lstImgCtg_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstImgCtg.SelectedIndexChanged
        'If lstCustCompany.Text <> "" Then
        '    lblTitle.Visible = False
        '    Alltab.Visible = True
        'End If
        ViewState("IMGCTG_ID") = lstImgCtg.SelectedValue
        showHomePageIMGList()
        'Response.Redirect("BranchSetting.aspx?branch=" & lstBranch.Text & "&tab=" & ViewState("MASTER_TAB"))

    End Sub

    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        Dim id As String = dialogArg1

        Using DB As New DBHelper
            Select Case dialogCmd
                Case "DELETE_HOMEPAGE"
                    'setJavaScript(Master_Tab.HOMEPAGEIMG)
                    DeleteHomePageImg(DB, Path.GetFileNameWithoutExtension(id), lstBranch.SelectedValue)
                    DeleteHomePageFile(lstBranch.SelectedValue, id)
                    InsertEventLog(DB, "Deleted Homepageimg", Nothing, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                    showHomePageIMGList()
                Case "DELETE_HOMEPAGE_ALL"
                    DeleteHomePageAllImg(DB, Path.GetFileNameWithoutExtension(id))
                    DeleteHomePageFile(lstBranch.SelectedValue, id)
                    InsertEventLog(DB, "Deleted Homepageimg", Nothing, Nothing, Nothing, True)
                    MessageLine("CONFIRM", "Successfully deleted")
                    showHomePageIMGList()
                Case "DELETE_ORDERREMINDER"
                    'setJavaScript(Master_Tab.NOTIFICATION)
                    DeleteOrderReminder(DB, id, lstBranch.SelectedValue)
                    MessageLine("CONFIRM", "Successfully deleted")
                    showOrderReminderList()
                Case "SAVE_DELIVERYMASTER"
                    SaveSchedule()
                Case "DELETE_DELIVERYMASTER"
                    'setJavaScript(Master_Tab.DELIVERYMASTER)
                    DeleteSchedule(id)
                    showDeliveryMasterList()
                Case "DELETE_PROMOTIONITEM"
                    'setJavaScript(Master_Tab.PROMOTIONITEM)
                    For Each row As GridViewRow In grdPromotionalItemlist.Rows
                        Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                        Dim lblItem = CType(row.FindControl("lnkItemId"), Label)
                        If CheckBoxchckrw.Checked Then
                            DBUpdate.DeletePromoItem(DB, lblItem.Text, lstBranch.Text)
                        End If
                    Next
                    showPromoItemsList()
                Case "DELETE_SCHEDULE"
                    'setJavaScript(Master_Tab.DELIVERYMASTER)
                    deleteDtlCustDelSchMaster(id)

            End Select
        End Using
    End Sub

    Private Sub dialogResult_no(ByVal dialogCmd As String)
        Select Case dialogCmd
            Case "DELETE_CUSTITEM"
                'setJavaScript(Master_Tab.HOMEPAGEIMG)
            Case "DELETE_ORDERREMINDER"
                'setJavaScript(Master_Tab.NOTIFICATION)
            Case "DELETE_DELIVERYMASTER"
                'setJavaScript(Master_Tab.DELIVERYMASTER)
                showDeliveryMasterList()
            Case "DELETE_PROMOTIONITEM"
                showPromoItemsList()
            Case "SAVE_DELIVERYMASTER", "DELETE_SCHEDULE"
                'setJavaScript(Master_Tab.DELIVERYMASTER)

            Case Else
        End Select
    End Sub

    Private Sub setJavaScript(ByVal tab As Master_Tab)
        'JavaScript = "$('#tabs').tabs('select', " & Common.convertToLocalTab(tab) & ");" 'selectSubmenu(" & tab & ");"

        Dim script As String = "$(document).ready(function(){{ $('#tabs').tabs('option', 'active', " & tab & "); }});"
        ClientScript.RegisterStartupScript(Me.GetType(), "SetActiveTab", script, True)
        'ViewState("MASTER_TAB") = tab
        'ViewState("MASTER_TAB") = tab
        'JavaScript &= "scrollTo(0, 0);" 'TODO: Doesn't work
    End Sub

    'Private Function convertToLocalTab(ByVal tab As [Enum]) As Integer
    '    'Select Case tab
    '    '    Case Master_Tab.HOMEPAGEIMG
    '    '        Return Convert.ToInt32(tab)
    '    '    Case Master_Tab.DELIVERYMASTER
    '    '        Return 1
    '    '    Case Master_Tab.NOTIFICATION
    '    '        Return 2
    '    'End Select
    '    'Return 0
    '    Return Convert.ToInt32(tab)
    'End Function

#Region "Promote Picture"
    Protected Sub btnAddHomepageImg_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddHomepageImg.Click
        Using DB As New DBHelper
            showHomePageIMGDTL("")
            ImgHomepage.Visible = False

        End Using

    End Sub

    Protected Sub lnkBack_HomePageImg_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBack_HomePageImg.Click
        showHomePageIMGList()
    End Sub

    Private Sub showHomePageIMGList()

        Dim SQL As String
        Dim firstRow As GridViewRow
        Dim lastRow As GridViewRow
        Dim upBtn As ImageButton
        Dim downBtn As ImageButton
        Using DB As New DBHelper
            'Show schedule list


            pnlList_HomePageIMG.Visible = True
            pnlDtl_HomePageImg.Visible = False
            SQL = "SELECT HOMEPAGEIMG_ID, IMGNAME,FOLDERPATH,REDIRECTURL, JFCBRANCHNR,DISPORDER,CREATEUSER, MODUSER,CREATEDATE,item_id," &
                "CASE WHEN LENGTH(REDIRECTURL) > 20 THEN SUBSTR(REDIRECTURL, 1, 20) || '...' ELSE REDIRECTURL END AS REDIRECTURL FROM " &
                IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE " & _
                   IIf(lstBranch.Text = LST_ALL_STR, "1=1 and ImgCtg = " & DB.SqlEsc(lstImgCtg.Text), " JFCBRANCHNR = '" & lstBranch.SelectedValue & "' and ImgCtg = " & DB.SqlEsc(lstImgCtg.Text))
            dsHomepageImg.ProviderName = "System.Data.OracleClient"
            dsHomepageImg.ConnectionString = DB.GetConnectionString
            dsHomepageImg.SelectCommand = SQL
            grdHomepageImg.DataSourceID = "dsHomepageImg"
            grdHomepageImg.Sort("DISPORDER", SortDirection.Ascending)
            grdHomepageImg.DataBind()

            If grdHomepageImg.Rows.Count > 0 Then
                'Disable up button on first row and down button on last row
                'firstRow = grdHomepageImg.Rows(0)
                'upBtn = DirectCast(firstRow.FindControl("btnUp"), ImageButton)
                'upBtn.Visible = False
                'lastRow = grdHomepageImg.Rows(grdHomepageImg.Rows.Count - 1)
                'downBtn = DirectCast(lastRow.FindControl("btnDown"), ImageButton)
                'downBtn.Visible = False
            End If
        End Using

    End Sub

    Private Sub showHomePageIMGDTL(HomePageName As String)

        Dim SQL As String
        pnlList_HomePageIMG.Visible = False
        pnlDtl_HomePageImg.Visible = True
        ClearSubControl(pnlDtl_HomePageImg)
        txtItemId.CssClass = IIf(IsMobile(), "size8", "size12")
        Using DB As New DBHelper
            Select Case lstImgCtg.Text
                Case "NEW"
                    pnlitem.Visible = False
                    pnlReceipt.Visible = True
                    txtItemId.Visible = True
                    lblURL.Text = "URL Or Item ID"

                    lblOr.Visible = True
                Case "PROMOTION"
                    pnlitem.Visible = False
                    pnlReceipt.Visible = True
                    txtItemId.Visible = True
                    lblOr.Visible = True
                    lblURL.Text = "URL Or Item ID"
                    'txtItemId.CssClass = IIf(IsMobile(), "size32", "size48")
                Case "RECIPE"
                    pnlitem.Visible = False
                    txtItemId.Visible = True
                    lblURL.Text = "URL Or/And Item ID"
                    lblOr.Text = " or/and "
                    lblOr.Visible = True
                    'txtItemId.CssClass = IIf(IsMobile(), "size32", "size48")
                    pnlReceipt.Visible = True
            End Select
            cblBranch.DataTextField = "Value"
            cblBranch.DataValueField = "Key"
            cblBranch.DataSource = addCheckbox(lstBranch.Items)
            cblBranch.DataBind()
            Dim email = GetUserEmail(DB)

            If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                cbAll.Visible = True
                pnlBranchList.Visible = True
            Else
                cbAll.Visible = False
                pnlBranchList.Visible = False
            End If
            lblHomeCtg.Text = lstImgCtg.SelectedItem.Text

            If String.IsNullOrEmpty(HomePageName) Then
                ViewState("HomePageId") = ""
                txtHomePageName.Text = ""
                lblCreateDateName.Text = ""
                txtItemId.Text = ""
                txtImgUrl.Text = ""
                ImgHomepage.ImageUrl = ""  'GetHomePageImg(lstBranchNr.SelectedValue, HomePageID)
                btnSaveHomePageImg.Text = "Add"
                btnDeleteHomePageImg.Visible = False
                Dim objListItem As ListItem
                objListItem = cblBranch.Items.FindByValue(lstBranch.Text)

                If objListItem IsNot Nothing Then

                    objListItem.Selected = True
                End If
                'cblBranch.DataTextField = "custCompanyName"

                'cblBranch.DataValueField = "custCompany_id"

            Else

                SQL = "SELECT * FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE IMGNAME = '" & HomePageName & "' and ImgCtg = " & DB.SqlEsc(lstImgCtg.Text) & " order by JFCBRANCHNR"

                Dim r = DB.GetReader(SQL)

                'ViewState("HomePageId") = CString(r("HOMEPAGEIMG_ID"))

                While r.Read()
                    ImgHomepage.Visible = True
                    txtHomePageName.Text = CString(r("IMGNAME"))
                    'lstBranchNr.SelectedValue = Val(r("JFCBRANCHNR"))
                    lblCreateDateName.Text = CString(r("CREATEDATE")) & "  by " & CString(r("CREATEUSER"))
                    Dim filename = CString(r("FOLDERPATH"))
                    ImgHomepage.ImageUrl = GetHomePageImg(CString(r("JFCBRANCHNR")), Path.GetFileName(filename))


                    'filename.Substring(filename.LastIndexOf("\") + 1))
                    txtItemId.Text = CString(r("Item_id"))
                    txtImgUrl.Text = CString(r("REDIRECTURL"))
                    btnSaveHomePageImg.Text = "Update"
                    btnDeleteHomePageImg.Visible = True
                    Dim objListItem As ListItem
                    objListItem = cblBranch.Items.FindByValue(CString(r("JFCBRANCHNR")))
                    If lstBranch.Text = CString(r("JFCBRANCHNR")) Then
                        ViewState("HomePageId") = CString(r("HOMEPAGEIMG_ID"))
                    End If
                    If objListItem IsNot Nothing Then
                        objListItem.Value = CString(Path.GetFileName(filename))
                        objListItem.Selected = True
                    End If

                End While

            End If


        End Using

    End Sub

    Private Function addCheckbox(ByVal BranchList As ListItemCollection) As SortedList(Of String, String)
        Dim addCB_data As SortedList(Of String, String) = New SortedList(Of String, String)()
      
        For index As Integer = 0 To BranchList.Count - 1

            addCB_data.Add(BranchList(index).Value, BranchList(index).Text)
        Next
        Return addCB_data

    End Function


    Protected Sub btnSaveHomePageImg_Click(send As Object, e As EventArgs) Handles btnSaveHomePageImg.Click
        Dim sb As StringBuilder = New StringBuilder()
        Try
            Using DB As New DBHelper
                Dim HomePageName As String = Left(CString(txtHomePageName.Text), 64)

                'If fuHomepage.HasFile Then
                ' Or (Not String.IsNullOrEmpty(ViewState("HomePageId")) And txtHomePageName.Text <> "") 
                'HOMEPAGEIMG_ID = :1 and
                Dim Sql = "Select HOMEPAGEIMG_ID from " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " where  IMGNAME = :1 order by ABS(jfcbranchnr - " & lstBranch.Text & ")"
                Dim IMG_ID = CString(DB.GetScalar(Sql, HomePageName))
                'If IMG_ID = "" Then

                'Dim filetype = Path.GetExtension(fuHomepage.FileName)
                If HomePageName = "" Then
                    Throw New FormatException("Please enter an image name")
                End If
                'If (IsImageUrl(ImgHomepage.ImageUrl) And Not fuHomepage.HasFile) Then
                '    Throw New FormatException("Please select an image file with .jpg or .png")
                'End If
                If IsImageUrl(fuHomepage.FileName) And btnSaveHomePageImg.Text = "Add" Then
                    Throw New FormatException("Please select an image file with .jpg or .png")
                End If

                If fuHomepage.HasFile And IsImageUrl(fuHomepage.FileName) And btnSaveHomePageImg.Text = "Update" Then
                    Throw New FormatException("Please select an image file with .jpg or .png")
                End If

                Dim Objcheckbox As ListItem = cblBranch.Items.FindByText(lstBranch.SelectedItem.Text)

                If IMG_ID <> "" And IMG_ID <> Path.GetFileNameWithoutExtension(Objcheckbox.Value) Then
                    Throw New FormatException("Please enter a different name.")
                End If


                If ((Not IsValidUrlFormat(txtImgUrl.Text) And txtImgUrl.Text <> "") Or (Not IsValidItemId(txtItemId.Text) And txtItemId.Text <> "")) And lstImgCtg.Text <> "RECIPE" Then

                    Throw New FormatException("Please enter a valid URL or 5 digit item ID.")
                ElseIf Not IsValidUrlFormat(txtImgUrl.Text) And lstImgCtg.Text = "RECIPE" Then

                    Throw New FormatException("Please enter a valid URL.")
                End If
                If txtImgUrl.Text <> "" And txtItemId.Text <> "" And lstImgCtg.Text = "PROMOTION" Then

                    Throw New FormatException("Please enter either URL or item ID.")
                End If

                Dim email = GetUserEmail(DB)
                Dim disOrder As Nullable(Of Integer) = Nothing
                Dim compressedImage As Byte() = Nothing



                'If fuHomepage.HasFile Then
                '    Dim compressedImage As Byte() = Common.CompressIfLarge(fuHomepage.PostedFile.InputStream)


                '    For Each item As ListItem In cblBranch.Items
                '        If item.Selected Then
                '            If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                '                Sql = "SELECT Min(dispOrder) OVER () max_dispOrder FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE JFCBRANCHNR = :1 AND ImgCtg =:2 FOR UPDATE OF dispOrder"
                '                disOrder = Val(DB.GetScalar(Sql, item.Text.Substring(0, 2), lstImgCtg.Text)) - 1

                '            End If

                '            SaveImgForEachBranch(DB, item.Text.Substring(0, 2), Path.GetFileNameWithoutExtension(item.Value), fuHomepage.FileName, compressedImage, disOrder)
                '        ElseIf item.Selected And item.Value.Length > 3 Then
                '            If UpdateHomePage(DB, Path.GetFileNameWithoutExtension(item.Value), HomePageName, GetUserEmail(DB), txtImgUrl.Text, txtItemId.Text) Then
                '                MessageLine("CONFIRM", "Successfully saved")
                '            End If
                '        ElseIf item.Selected And item.Value.Length < 3 Then
                '            If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                '                Sql = "SELECT Min(dispOrder) OVER () max_dispOrder FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE JFCBRANCHNR = :1 AND ImgCtg =:2 FOR UPDATE OF dispOrder"
                '                disOrder = Val(DB.GetScalar(Sql, item.Text.Substring(0, 2), lstImgCtg.Text)) - 1

                '            End If
                '            CopyImgForOneBranchToOther(DB, item.Text.Substring(0, 2), Path.GetFileNameWithoutExtension(item.Value), ImgHomepage.ImageUrl, disOrder)
                '        ElseIf Not item.Selected And item.Value.Length > 3 Then

                '            DeleteHomePageImg(DB, Path.GetFileNameWithoutExtension(item.Value), item.Text.Substring(0, 2))
                '            DeleteHomePageFile(item.Text.Substring(0, 2), item.Value)
                '        End If


                '    Next
                'End If



                ' Compress image if file is uploaded
                If fuHomepage.HasFile Then
                    compressedImage = Common.CompressIfLarge(fuHomepage.PostedFile.InputStream)
                End If

                ' Process each branch checkbox
                For Each item As ListItem In cblBranch.Items
                    If item.Selected Then
                        ' Handle cases where branch is selected
                        If fuHomepage.HasFile Then
                            ' New image uploaded - save/update with new image
                            If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                                Sql = "SELECT Min(dispOrder) OVER () max_dispOrder FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE JFCBRANCHNR = :1 AND ImgCtg =:2 FOR UPDATE OF dispOrder"
                                disOrder = Val(DB.GetScalar(Sql, item.Text.Substring(0, 2), lstImgCtg.Text)) - 1
                            End If
                            SaveImgForEachBranch(DB, item.Text.Substring(0, 2), Path.GetFileNameWithoutExtension(item.Value), fuHomepage.FileName, compressedImage, disOrder)
                        ElseIf item.Value.Length > 3 Then
                            ' No new image, but updating existing record (item_id, URL, etc.)
                            If UpdateHomePage(DB, Path.GetFileNameWithoutExtension(item.Value), HomePageName, GetUserEmail(DB), txtImgUrl.Text, txtItemId.Text) Then
                                MessageLine("CONFIRM", "Successfully saved")
                            End If
                        ElseIf item.Value.Length < 3 Then
                            ' Copy image from another branch
                            If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                                Sql = "SELECT Min(dispOrder) OVER () max_dispOrder FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE JFCBRANCHNR = :1 AND ImgCtg =:2 FOR UPDATE OF dispOrder"
                                disOrder = Val(DB.GetScalar(Sql, item.Text.Substring(0, 2), lstImgCtg.Text)) - 1
                            End If
                            CopyImgForOneBranchToOther(DB, item.Text.Substring(0, 2), Path.GetFileNameWithoutExtension(item.Value), ImgHomepage.ImageUrl, disOrder)
                        End If
                    ElseIf Not item.Selected And item.Value.Length > 3 Then
                        ' Branch unchecked - delete the image
                        DeleteHomePageImg(DB, Path.GetFileNameWithoutExtension(item.Value), item.Text.Substring(0, 2))
                        DeleteHomePageFile(item.Text.Substring(0, 2), item.Value)
                    End If
                Next

                'Dim filename = Path.GetFileNameWithoutExtension(fuHomepage.FileName)
                'Dim ImgUrl As String = txtImgUrl.Text
                'Dim itemID As String = txtItemId.Text
                'Dim ImgCtg As String = lstImgCtg.Text
                'If itemID.Length > 5 Then
                '    Throw New FormatException("Please enter charactor not more than 5")
                'End If
                'If String.IsNullOrEmpty(ViewState("HomePageId")) Then

                '    ViewState("HomePageId") = DB.GetSequence("HOMEPAGE_ID", HOMEPAGE_ID_LED, True)

                'End If
                'Dim HomepageName As String
                'If String.IsNullOrEmpty(txtHomePageName.Text) Then
                '    HomepageName = filename
                'Else
                '    HomepageName = txtHomePageName.Text
                'End If

                ''Dim filepath = GetHomePageFilePath(lstBranchNr.SelectedValue) & ViewState("HomePageId") & "." & str(1)

                ''ViewState("HomePageId") 
                'Dim filepath = Server.MapPath("~/Images/HomePageImg/" & lstBranch.Text & "/") & HomepageName & filetype

                'UpsertHomepage(DB, ViewState("HomePageId"), HomepageName, filepath, lstBranch.Text, GetUserEmail(DB), ImgUrl, itemID, ImgCtg)
                'ImgHomepage.ImageUrl = GetHomePageImg(lstBranch.Text, CString(HomepageName & filetype))
                'fuHomepage.SaveAs(filepath)
                'fuHomepage.SaveAs(GetHomepageFilePath(lstBranch.Text) & HomepageName & filetype)
                'ImgHomepage.Visible = True
                'btnSaveHomePageImg.Text = "Update Image"
                'MessageLine("CONFIRM", "Successfully saved")


                'ElseIf Not String.IsNullOrEmpty(ViewState("HomePageId")) And txtHomePageName.Text <> "" Then




                showHomePageIMGList()
            End Using
        Catch ex As FormatException

            MessageBox(ex.Message, "Error")
        Finally

        End Try



        'DB.GetSequence("HOMEPAGEIMG_ID", HomePage_ID_LED)
    End Sub

    Public Function IsValidUrlFormat(url As String) As Boolean
        Dim uriResult As Uri = Nothing
        'If Not url.StartsWith("http://") AndAlso Not url.StartsWith("https://") Then
        'url = "https://" & url
        'End If
        Return Uri.TryCreate(url, UriKind.Absolute, uriResult) AndAlso (uriResult.Scheme = Uri.UriSchemeHttp OrElse uriResult.Scheme = Uri.UriSchemeHttps)
    End Function
    Public Function IsWellFormedUriString(url As String) As Boolean

        Return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)
    End Function

    Private Function IsUrlValid(url As String) As Boolean
        If String.IsNullOrEmpty(url) Then
            Return False
        End If

        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            request.Method = "GET"
            request.KeepAlive = False
            request.Timeout = 5000 ' 1 seconds timeout
            request.Proxy = WebRequest.DefaultWebProxy
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Return response.StatusCode = HttpStatusCode.OK
            End Using
        Catch ex As WebException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function IsValidItemId(itemId As String) As Boolean
        Dim regex As New Regex("^\d{5}$")
        Return regex.IsMatch(itemId)
    End Function
    Private Function IsImageUrl(url As String) As Boolean
        If url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) Or
        url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) Or
            url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) Or
            url.EndsWith(".JPG", StringComparison.OrdinalIgnoreCase) Or
        url.EndsWith(".JPEG", StringComparison.OrdinalIgnoreCase) Or
            url.EndsWith(".PNG", StringComparison.OrdinalIgnoreCase) Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub SaveImgForEachBranch(DB As DBHelper, branchNr As String, IMG_ID As String, fileName_url As String, compressedImage As Byte(), disOrder As Nullable(Of Integer))
        Dim filetype = Path.GetExtension(fileName_url)

        Dim filename = Path.GetFileNameWithoutExtension(fileName_url)

        Dim ImgUrl As String = txtImgUrl.Text
        Dim itemID As String = txtItemId.Text
        Dim ImgCtg As String = lstImgCtg.Text
        If itemID.Length > 5 Then
            Throw New FormatException("Please enter charactor not more than 5")
        End If
        If IMG_ID.Length < 3 Then

            IMG_ID = DB.GetSequence("HOMEPAGE_ID", HOMEPAGE_ID_LED, True)

        End If
        Dim HomepageName As String
        If String.IsNullOrEmpty(Left(txtHomePageName.Text, 64)) Then
            HomepageName = filename
        Else
            HomepageName = Left(txtHomePageName.Text, 64)
        End If

        'Dim filepath = GetHomePageFilePath(lstBranchNr.SelectedValue) & ViewState("HomePageId") & "." & str(1)

        'ViewState("HomePageId") 
        Dim filepath = Server.MapPath("~/Images/HomePageImg/" & branchNr & "/") & IMG_ID & filetype

        UpsertHomepage(DB, IMG_ID, HomepageName, filepath, branchNr, GetUserEmail(DB), ImgUrl, itemID, ImgCtg, disOrder)
        ImgHomepage.ImageUrl = GetHomePageImg(branchNr, CString(IMG_ID & filetype))
        File.WriteAllBytes(filepath, compressedImage)
        File.WriteAllBytes(GetHomepageFilePath(branchNr) & IMG_ID & filetype, compressedImage)
        'fuHomepage.SaveAs(filepath)
        'fuHomepage.SaveAs(GetHomepageFilePath(branchNr) & IMG_ID & filetype)
        ImgHomepage.Visible = True
        btnSaveHomePageImg.Text = "Update"
        MessageLine("CONFIRM", "Successfully saved")
    End Sub

    Private Sub CopyImgForOneBranchToOther(DB As DBHelper, branchNr As String, IMG_ID As String, fileName_url As String, disOrder As Nullable(Of Integer))


        Dim filetype = Path.GetExtension(fileName_url)

        Dim filename = Path.GetFileNameWithoutExtension(fileName_url)
        Dim ImgUrl As String = txtImgUrl.Text
        Dim itemID As String = txtItemId.Text
        Dim ImgCtg As String = lstImgCtg.Text
        If itemID.Length > 5 Then
            Throw New FormatException("Please enter charactor not more than 5")
        End If
        If IMG_ID.Length < 3 Then

            IMG_ID = DB.GetSequence("HOMEPAGE_ID", HOMEPAGE_ID_LED, True)

        End If
        Dim HomepageName As String
        If String.IsNullOrEmpty(txtHomePageName.Text) Then
            HomepageName = filename
        Else
            HomepageName = Left(txtHomePageName.Text, 64)
        End If
        Dim filepath = Server.MapPath("~/Images/HomePageImg/" & branchNr & "/") & IMG_ID & filetype
        Dim orginalfilepath = Server.MapPath("~/Images/HomePageImg/" & GetSecondLastFolderName(fileName_url) & "/") & filename & filetype
        UpsertHomepage(DB, IMG_ID, HomepageName, filepath, branchNr, GetUserEmail(DB), ImgUrl, itemID, ImgCtg, disOrder)
        'ImgHomepage.ImageUrl = GetHomePageImg(branchNr, CString(IMG_ID & filetype))

        Try
            File.Copy(orginalfilepath, filepath, True)
            File.Copy(GetHomepageFilePath(GetSecondLastFolderName(fileName_url)) & filename & filetype, GetHomepageFilePath(branchNr) & IMG_ID & filetype, True) ' Copying file to destination directory

        Catch ex As Exception
            Console.WriteLine("Error occurred while copying the file: " & ex.Message)
        End Try
    End Sub

    Function GetSecondLastFolderName(ByVal Url As String) As String
        Dim directoryInfo As DirectoryInfo = New DirectoryInfo(Path.GetDirectoryName(Url))

        If directoryInfo.Parent IsNot Nothing AndAlso directoryInfo.Parent.Parent IsNot Nothing Then
            Return directoryInfo.Name
        Else
            Return String.Empty
        End If
    End Function

    Protected Sub grdHomepageImg_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdHomepageImg.RowCommand
        Using DB As New DBHelper

            'setJavaScript(Master_Tab.HOMEPAGEIMG)
            If e.CommandName = "Page" Then
                'Page is changed

                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False
            ElseIf e.CommandName = "UP" Or e.CommandName = "DOWN" Then

                Dim index = CInt(e.CommandArgument)
                Dim down As ImageButton
                Dim up As ImageButton
                Dim downlbl As Label
                Dim uplbl As Label

                Dim Downrow = grdHomepageImg.Rows.Item(CInt(If(e.CommandName = "UP", index - 1, index + 1)))
                down = Downrow.FindControl("imgHomepage")
                downlbl = Downrow.FindControl("lblOrder")
                Dim downorder As String = downlbl.Text
                'Downrow.Cells(Common.GetColumnIndexByName(Downrow, "DISPORDER")).Text
                Dim downImg_id = down.CommandArgument

                Dim Uprow = grdHomepageImg.Rows.Item(index)
                up = Uprow.FindControl("imgHomepage")
                Dim upImg_id = up.CommandArgument
                uplbl = Uprow.FindControl("lblOrder")
                Dim uporder As String = uplbl.Text
                'Uprow.Cells(Common.GetColumnIndexByName(Uprow, "DISPORDER")).Text
                UpsertHomePageOrder(DB, downImg_id, downorder, upImg_id, uporder, lstBranch.SelectedValue)
                showHomePageIMGList()

            ElseIf e.CommandName = "DELETE_ROW" Then
                Dim filename As String = Path.GetFileName(CString(e.CommandArgument))
                deleteHOMEPAGEIMGRow(filename)

            ElseIf e.CommandName = "DETAIL" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()

                showHomePageIMGDTL(e.CommandArgument.ToString())

            End If
        End Using

    End Sub

    Private Sub deleteHOMEPAGEIMGRow(ByVal HOMEPAGEIMGID As String)
        Dim msg As String

        Using DB As New DBHelper
            'setJavaScript(Master_Tab.HOMEPAGEIMG)
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_HOMEPAGE"
            ViewState("DIALOG_ARG1") = HOMEPAGEIMGID
            '" & GetImgName(DB, HOMEPAGEIMGName, lstBranch.SelectedValue) & "

            msg = "Are you sure you want to remove the image " & DB.SqlEsc(GetImgName(DB, Path.GetFileNameWithoutExtension(HOMEPAGEIMGID), lstBranch.SelectedValue)) & " ?"

            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub

    Protected Sub btnDeleteHomePageImg_Click(send As Object, e As EventArgs) Handles btnDeleteHomePageImg.Click
        Dim a = lstBranch.SelectedItem.Text
        Dim Objcheckbox = cblBranch.Items.FindByText(a)
        Dim msg As String
        Using DB As New DBHelper
            '" & GetImgName(DB, HOMEPAGEIMGName, lstBranch.SelectedValue) & "
            If Not pnlBranchList.Visible Then
                ViewState("DIALOG_CMD") = "DELETE_HOMEPAGE"
                ViewState("DIALOG_ARG1") = Objcheckbox.Value
                msg = "Are you sure you want to remove this image " & txtHomePageName.Text & " ?"
            Else
                ViewState("DIALOG_CMD") = "DELETE_HOMEPAGE_ALL"
                ViewState("DIALOG_ARG1") = GetImgName(DB, Path.GetFileNameWithoutExtension(Objcheckbox.Value), lstBranch.SelectedValue)
                msg = "Are you sure you want to remove the image " & DB.SqlEsc(txtHomePageName.Text) & " from all  branches?"
            End If
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using

        'deleteHOMEPAGEIMGRow(Objcheckbox.Value)
    End Sub
    'Protected Sub afuHomepage_UploadComplete(sender As Object, e As ar) h
    '    txtHomePageName.Text = e.FileName.ToString
    '    Dim fileNametoupload As String = Server.MapPath("~/123/1") & e.FileName.ToString()
    '    afuHomepage.SaveAs(fileNametoupload)
    'End Sub
#End Region

#Region "Notification"
    Private Sub showOrderReminderList()
        Dim SQL As String
        'setJavaScript(Master_Tab.NOTIFICATION)

        Using DB As New DBHelper
            'Show schedule list

            SQL = "SELECT * FROM codesetting where settinggrp = 'ORDER_REMINDER' " & _
                   IIf(lstBranch.Text = LST_ALL_STR, "", "AND setting_id = " & DB.SqlEsc(lstBranch.SelectedValue) & "")
            SQL &= " order by TO_NUMBER(Settingvalue1)"
            dsOrderReminder.ProviderName = "System.Data.OracleClient"
            dsOrderReminder.ConnectionString = DB.GetConnectionString
            dsOrderReminder.SelectCommand = SQL
            grdOrderReminder.DataSourceID = "dsOrderReminder"
            'grdOrderReminder.Sort("Settingvalue1", SortDirection.Ascending)
            grdOrderReminder.DataBind()


        End Using

    End Sub

    Private Sub showOrderReminderDtl(ByVal prehoursprior As Nullable(Of Decimal))
        'setJavaScript(Master_Tab.NOTIFICATION)
        txtHourprior.Text = ""
        lblPopWarning.Text = ""
        pnlPopWarningOrder.Visible = False

        If prehoursprior.HasValue Then
            txtHourprior.Text = prehoursprior.Value
            ViewState("prehoursprior") = prehoursprior.Value
            lblPopHeader_Order.Text = "Edit"
            btnPopSaveOrder.Text = "Update"
            btnPopDeleteOrder.Visible = True
        Else

            btnPopDeleteOrder.Visible = False
            btnPopSaveOrder.Text = "Add"
            lblPopHeader_Order.Text = "Add"
            ViewState("prehoursprior") = Nothing
        End If
        popDialog_OrderReminder.Show()
    End Sub


    Protected Sub addOrderReminder_Click(sender As Object, ByVal e As System.EventArgs) Handles BtnOrderReminder.Click
        showOrderReminderDtl(Nothing)

    End Sub

    Protected Sub btnPopSaveOrder_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopSaveOrder.Click
        'setJavaScript(Master_Tab.NOTIFICATION)
        Dim num As Integer
        Using DB As New DBHelper
            Try
                If Val(txtHourprior.Text) <= 0 Or Val(txtHourprior.Text) > 24 Or Not Integer.TryParse(txtHourprior.Text, num) Then
                    Throw New Exception("Please enter a valid integer number. 1 to 24")
                End If

                UpsertOrderReminder(DB, ViewState("prehoursprior"), Val(txtHourprior.Text), lstBranch.SelectedValue)
            Catch ex As Exception

                pnlPopWarningOrder.Visible = True
                lblPopWarning.Text = ex.Message

                popDialog_OrderReminder.Show()
                Exit Sub
            End Try

        End Using
        showOrderReminderList()
    End Sub

    Protected Sub btnPopDeleteOrder_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopDeleteOrder.Click
        deleteOrderReminderRow(ViewState("prehoursprior"))
    End Sub
    Protected Sub btnPopCloseOrder_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopClose_Order.Click
        'setJavaScript(Master_Tab.NOTIFICATION)
    End Sub
    Private Sub deleteOrderReminderRow(ByVal Hourprior As String)
        Dim msg As String

        Using DB As New DBHelper
            'setJavaScript(Master_Tab.NOTIFICATION)
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_ORDERREMINDER"
            ViewState("DIALOG_ARG1") = Hourprior

            msg = "Are you sure you want to remove this hour?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub
    Protected Sub grdOrderReminder_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdOrderReminder.RowCommand
        Using DB As New DBHelper

            'setJavaScript(Master_Tab.NOTIFICATION)
            If e.CommandName = "Page" Then
                'Page is changed

                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False

            ElseIf e.CommandName = "DELETE_ROW" Then
                deleteOrderReminderRow(e.CommandArgument.ToString())

            ElseIf e.CommandName = "DETAIL" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()

                showOrderReminderDtl(e.CommandArgument.ToString())

            End If
        End Using

    End Sub
#End Region

#Region "Delivery Master"
    'select * from JFCDeliveryMaster
    Private Sub showDeliveryMasterList()
        Dim SQL As String
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        pnlDtl_schedule.Visible = False
        pnlList_schedule.Visible = True
        Using DB As New DBHelper
            'Show schedule list

            SQL = "select * from JFCDeliveryMaster where (1=1)" & IIf(lstBranch.Text = LST_ALL_STR, "", "AND JFCBRANCHNR = " & DB.SqlEsc(lstBranch.SelectedValue) & "")
            SQL &= " order by DELIVERYMASTERNAME"  ',TO_NUMBER(DELIVERYMASTER_ID)
            dsActiveSchedule.ProviderName = "System.Data.OracleClient"
            dsActiveSchedule.ConnectionString = DB.GetConnectionString
            dsActiveSchedule.SelectCommand = SQL
            grdActiveSchedule.DataSourceID = "dsActiveSchedule"
            'grdOrderReminder.Sort("Settingvalue1", SortDirection.Ascending)
            grdActiveSchedule.DataBind()


        End Using


    End Sub



    Private Sub showDtlCustDelSchMaster(MasterID As String)
        Dim SQL As String
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        'pnlDtl_schedule.Visible = False
        'pnlList_schedule.Visible = True
        Using DB As New DBHelper
            'Show schedule list

            SQL = "SELECT c.jfcCustomerNr, g.custGrpName, c.customerName, s.shipName,UPPER(s.state) as STATE,TO_CHAR(dd.deliverydate, " & DB.SqlEsc(DATE_FMT) & ") as deliverydate,TO_CHAR(dd.orderByDate, " & DB.SqlEsc(DATE_FMT) & ") orderByDate, " & _
                "TO_CHAR(dd.orderByDate, " & DB.SqlEsc(TIME_SQLFMT) & ") orderByTime,dd.orderByDate as orderDatetime,dd.sendreminder, d.shipPoint_id,  d.deliverySched_id, d.customer_id," & _
             "d.custGrpVendor_id, d.deliveryMaster_id, d.frequency, d.sendReminder, c.jfcBranchNr, c.jfcCustomerNr," & _
             "TO_CHAR(startDate) startDate, TO_CHAR(endDate) endDate,  jm.frequency as frequency, jm.deliveryday as deliveryday, jm.orderbyday as orderbyday," & _
             "jm.orderbytime as orderbytime,dd.deliverydate as dliveryOrder FROM customer_deliverySched d, custGrp g, customer c, customer_shipPoint s,JFCDeliveryMaster jm, customer_deliverydate dd " & _
            "WHERE(d.customer_id = c.customer_id And g.custGrp_id = c.custGrp_id) AND d.shipPoint_id = s.shipPoint_id AND d.customer_id = s.customer_id " & _
           " AND jm.deliverymaster_id  = d.deliverymaster_id AND DD.deliverySched_id = D.deliverySched_id AND dd.deliveryschedlinenr = d.deliveryschedlinenr  AND d.deliverymaster_id = " & DB.SqlEsc(MasterID) & _
             IIf(txtJfcAccountNum.Text <> "", " and c.jfcCustomerNr = " & DB.SqlEsc(txtJfcAccountNum.Text), "") & _
             IIf(txtState.Text <> "", " AND UPPER(s.state) = " & DB.SqlEsc(txtState.Text.ToUpper), "") & _
             IIf(txtDeliveryDateFrom.Text <> "", " AND dd.deliverydate >= TO_DATE(" & DB.SqlEsc(txtDeliveryDateFrom.Text) & ", " & DB.SqlEsc(DATE_FMT) & ")", "") & _
             IIf(txtDeliveryDateTo.Text <> "", " AND dd.deliverydate <= TO_DATE(" & DB.SqlEsc(txtDeliveryDateTo.Text) & ", " & DB.SqlEsc(DATE_FMT) & ")", "")

            'SQL = "select * from JFCDeliveryMaster where (1=1)" & IIf(lstBranch.Text = LST_ALL_STR, "", "AND JFCBRANCHNR = " & DB.SqlEsc(lstBranch.SelectedValue) & "")
            'SQL &= " order by DELIVERYMASTERNAME"  ',TO_NUMBER(DELIVERYMASTER_ID)
            dsDtlCustDelSchMaster.ProviderName = "System.Data.OracleClient"
            dsDtlCustDelSchMaster.ConnectionString = DB.GetConnectionString
            dsDtlCustDelSchMaster.SelectCommand = SQL
            grdDtlCustDelSchMaster.DataSourceID = "dsDtlCustDelSchMaster"
            grdDtlCustDelSchMaster.Sort(lstOrderBy_order.SelectedValue, Nothing)
            'grdOrderReminder.Sort("Settingvalue1", SortDirection.Ascending)
            'grdDtlCustDelSchMaster.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing; 
            ' or even better, use .DisableResizing. Most time consuming enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
            '  set it to false if not needed
            'grdDtlCustDelSchMaster.rowh()

            grdDtlCustDelSchMaster.DataBind()


        End Using


    End Sub
    Protected Sub grdDtlCustDelSchMaster_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdDtlCustDelSchMaster.RowCommand
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        If e.CommandName = "Page" Then
            'Page is changed

            showDtlCustDelSchMaster(lblMasterID.Text)
            'Don't remember scroll position
            Page.MaintainScrollPositionOnPostBack = False

        ElseIf e.CommandName = "DELETE_ROW" Then
            'Delete schedule
            deleteDtlCustDelSchMasterRow(Convert.ToString(e.CommandArgument))
        End If
    End Sub
    Private Sub deleteDtlCustDelSchMasterRow(ByVal deliverySchedID As String)
        Using DB As New DBHelper
            'Confirm
            ViewState("DIALOG_CMD") = "DELETE_SCHEDULE"
            ViewState("DIALOG_ARG1") = deliverySchedID
            MessageBoxYesNo("Are you sure you want to delete this delivery schedule?", "Confirm Deletion", DB)
        End Using
    End Sub

    Private Sub deleteDtlCustDelSchMaster(deliveryCustMasterID As String)
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        'Delete delivery date
        Using DB As New DBHelper
            Dim IDs As String() = deliveryCustMasterID.Split(",")
            DeleteDeliveryDate(DB, IDs(0), IDs(1), IDs(2), IDs(3))

            'Show transaction month
            'DateTime.TryParseExact(IDs(3), DATE_FMT, Nothing, Nothing, dt)
            'showScheduleList()
            showDtlCustDelSchMaster(lblMasterID.Text)
            InsertEventLog(DB, "Deleted delivery date", IDs(3), Nothing, Nothing, True)
            MessageLine("CONFIRM", "Successfully deleted")
        End Using
    End Sub
    Protected Sub grdDtlCustDelSchMaster_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdDtlCustDelSchMaster.RowDataBound
        Dim lst As DropDownList
        Dim txt As TextBox
        Dim str As String

        Using DB As New DBHelper
            If e.Row.RowType = DataControlRowType.DataRow Then
                'Show and select hide order
                'lst = CType(e.Row.FindControl("lstPMAM"), DropDownList)
                'ShowCodeMasterListLocal(DB, "AMPM", lst, False)
                'IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                'str = CType(e.Row.FindControl("lstPMAM"), Label).Text
                'str = IIf(str = "AM", "1", "2")
                'If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                lst = CType(e.Row.FindControl("lstOrderTime"), DropDownList)
                ShowCodeMasterListLocal(DB, "TIMEOFDAY", lst, True)
                str = CType(e.Row.FindControl("lblOrderTime"), Label).Text
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                lst = CType(e.Row.FindControl("lstOrderByAmPm"), DropDownList)
                ShowCodeMasterListLocal(DB, "AMPM", lst, True)
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                str = CType(e.Row.FindControl("lblOrderByAmPm"), Label).Text

                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True

                lst = CType(e.Row.FindControl("lstSendReminder"), DropDownList)
                ShowCodeMasterListLocal(DB, "YESNO", lst, True)
                IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")
                str = CType(e.Row.FindControl("lblSendReminder"), Label).Text
                str = IIf(str = "Y", "Yes", "No")
                If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True


                'Show and select hide order
                'lst = CType(e.Row.FindControl("lstHidePrice"), DropDownList)
                'ShowCodeMasterListLocal(DB, "HIDEDISPLAY", lst, False)
                'str = CType(e.Row.FindControl("lblHidePrice"), Label).Text
                'str = IIf(str = "Y", "Y", "N")
                'If lst.Items.FindByValue(str) IsNot Nothing Then lst.Items.FindByValue(str).Selected = True
                'IIf(IsMobile(), lst.CssClass = "cellLbl drpList size4", "")

                'txt = CType(e.Row.FindControl("txtCell"), TextBox)
                'IIf(IsMobile(), txt.CssClass = "cellLbl size4", "")

            End If
        End Using
    End Sub

    Protected Sub lstOrderBy_order_TextChanged(sender As Object, e As System.EventArgs) Handles lstOrderBy_order.TextChanged
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        showDtlCustDelSchMaster(lblMasterID.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        Dim dateVal As Date

        'Error check
        If txtDeliveryDateFrom.Text <> "" AndAlso DateTime.TryParseExact(txtDeliveryDateFrom.Text, DATE_FMT, Nothing, Nothing, dateVal) = False Then
            MessageBox("Delivery Date has to be in " & DATE_FMT.ToLower & " format", "Error")
            Exit Sub
        End If
        If txtDeliveryDateTo.Text <> "" AndAlso DateTime.TryParseExact(txtDeliveryDateFrom.Text, DATE_FMT, Nothing, Nothing, dateVal) = False Then
            MessageBox("Delivery date has to be in " & DATE_FMT.ToLower & " format", "Error")
            Exit Sub
        End If
        showDtlCustDelSchMaster(lblMasterID.Text)
    End Sub
    Protected Sub btnSaveCustDelSchMaster_Click(sender As Object, e As System.EventArgs) Handles btnSaveCustDelSchMaster.Click
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        Dim DeliveryDate As DateTime
        Dim Orderdate As DateTime
        Dim IsSendEmail As Boolean = False
        Using DB As New DBHelper
            For Each row As GridViewRow In grdDtlCustDelSchMaster.Rows
                Dim TxtdeliverOn = CType(row.FindControl("txtDeliveryOn"), TextBox)
                Dim TxtOrderdate = CType(row.FindControl("txtOrderby"), TextBox)
                Dim lstOrdertime = CType(row.FindControl("lstOrderTime"), DropDownList)
                Dim lstOrderAMPM = CType(row.FindControl("lstOrderByAmPm"), DropDownList)


                If DateTime.TryParse(TxtdeliverOn.Text, DeliveryDate) And
                    DateTime.TryParse(TxtOrderdate.Text & " " & lstOrdertime.Text & " " & lstOrderAMPM.Text, Orderdate) Then
                    If DeliveryDate < Orderdate Then

                        MessageBox("Delivery date has to be later than the order by date for #" & row.Cells(1).Text & " delivering on " & TxtdeliverOn.Text & ".", "Error", DB)
                        SetFocus(TxtdeliverOn)
                        TxtdeliverOn.Focus()

                        Exit Sub
                    End If
                End If


            Next

            For Each row As GridViewRow In grdDtlCustDelSchMaster.Rows
                Dim TxtdeliverOn = CType(row.FindControl("txtDeliveryOn"), TextBox)
                Dim TxtOrderdate = CType(row.FindControl("txtOrderby"), TextBox)
                Dim lstOrdertime = CType(row.FindControl("lstOrderTime"), DropDownList)
                Dim lstOrderAMPM = CType(row.FindControl("lstOrderByAmPm"), DropDownList)
                Dim lblOldDelivery = CType(row.FindControl("lblDeliveryOn"), Label)
                Dim lblOldOrderby = CType(row.FindControl("lblOrderby"), Label)
                Dim lblOldOrderTime = CType(row.FindControl("lblOrderTime"), Label)
                Dim lblOldOrderAMPM = CType(row.FindControl("lblOrderByAmPm"), Label)
                Dim lstSendReminder = CType(row.FindControl("lstSendReminder"), DropDownList)
                Dim checkemail = CType(row.FindControl("ChkSendEmail"), CheckBox)

                Dim lblcustomerID = CType(row.FindControl("lblcustomerID"), Label)
                Dim lblcustGrpVendorID = CType(row.FindControl("lblcustGrpVendorID"), Label)
                Dim lblshipPointID = CType(row.FindControl("lblshipPointID"), Label)
                UpdateDeliveryDate(DB, lblcustomerID.Text, lblcustGrpVendorID.Text, lblshipPointID.Text, lblOldDelivery.Text,
                                   TxtdeliverOn.Text, TxtOrderdate.Text & " " & lstOrdertime.Text & " " & lstOrderAMPM.Text, IIf(lstSendReminder.Text = "Yes", "Y", "N"))

                If checkemail.Checked Then
                    Dim emailBody As String = ""
                    Dim Sql = "SELECT u.email AS customerEmail, c.customer_id, c.customerName, c.jfcBranchNr, c.jfcCustomerNr,u.language ," &
                        "CASE WHEN NVL(c.hidePrice, ' ') <> 'Y' AND NVL(uc.hideprice, ' ') <> 'Y' THEN 'N' ELSE 'Y' END AS hidePrice " &
                        "FROM webuser u, webuser_customer uc, customer c " &
                        "WHERE(u.email = uc.email And uc.customer_id = c.customer_id) " &
                        "AND uc.usercompany_id <> 'JFC' AND c.allowCustOrder = 'Y' AND NVL(uc.hideCustOrder, ' ') <> 'Y' " &
                        "AND c.customer_id = '" & lblcustomerID.Text & "' order by c.customer_id"
                    Dim dt = DB.GetDataTable(Sql)
                    Dim sql2 As String = "SELECT email, jfcBranchNr FROM jfcVendor WHERE jfcBranchNr = :1"
                    Dim r2 As DataReader
                    r2 = DB.GetReader(sql2, lstBranch.Text)
                    Dim VendorEmail As String = ""
                    If r2.Read() Then
                        VendorEmail = CString(r2("email"))
                    End If
                    r2.close()


                    '2023
                    For Each dr In dt.Rows
                        Dim ci = New CultureInfo(IIf(CString(dr("language")) <> "", CString(dr("language")), "en-US").ToString)
                        Thread.CurrentThread.CurrentCulture = ci
                        Thread.CurrentThread.CurrentUICulture = ci
                        '"Dear " Your delivery info has been updated. Old Delivery Deadline New
                        emailBody = rm.GetString("Dear", ci) & " " & CString(dr("customerName")) & " (" & CString(dr("jfcBranchNr")) & "-" & CString(dr("jfcCustomerNr")) & _
                                    "), <br />  <br /> " & rm.GetString("Yourdeliveryinfohasbeenupdated", ci) & "  <br /> <br />" & _
                                    rm.GetString("Old", ci) & " <br />" & rm.GetString("Delivery", ci) & ": " & lblOldDelivery.Text & "<br />" & _
                                    rm.GetString("Deadline", ci) & ":  " & lblOldOrderby.Text & " " & lblOldOrderTime.Text & " " & lblOldOrderAMPM.Text & "<br /><br />" & _
                                    rm.GetString("New", ci) & " <br /><span style=""font-weight:bold""> " & rm.GetString("Delivery", ci) & ": " & TxtdeliverOn.Text & " <br />" & _
                                    rm.GetString("Deadline", ci) & ": " & TxtOrderdate.Text & " " & lstOrdertime.Text & " " & lstOrderAMPM.Text & _
                                    "</span> <br /> <br />   " & rm.GetString("Bestregards", ci) & ",<br /> " & Common.JFC_COMPANY_NAME
                        'CreateEmailBody(Page, emailBody, "MASTER", deliverySchedID & deliveryschedlinenr, newSchedule)
                        Dim errmsg As String = ""
                        'CString(dr("customerEmail"))

                        If Common.SendMail(SYSTEM_EMAIL, IIf(IsProdBeta() Or IsProduction(), CString(dr("customerEmail")), "yfu@kmsnet.com,jzhou@kmsnet.com"), VendorEmail, rm.GetString("Yourdeliveryinfowasupdated", ci), emailBody, Nothing, True, errmsg) > 0 Then
                            'Email sent failed
                            'emailSent = False

                        End If
                        IsSendEmail = True
                    Next
                End If


                'UpdateDeliveryDate(DB, , lblPopCustGrpVendorID.Text, lstShipPoint.Text, lblPopOldDeliveryDate.Text, txtPopDeliveryDate.Text, txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text, IIf(lstPopSendReminder.Text = "Yes", "Y", "N"))

            Next
            InsertEventLog(DB, "Updated delivery date for delivery Master", lblMasterID.Text, Nothing, Nothing, True)
            If IsSendEmail Then
                MessageLine("CONFIRM", "Successfully saved and send email")
            Else
                MessageLine("CONFIRM", "Successfully saved")
            End If

        End Using

    End Sub

    Private Sub showDtlDeliveryMaster(MasterID As String)

        'setJavaScript(Master_Tab.DELIVERYMASTER)
        Dim SQL As String
        pnlList_schedule.Visible = False
        pnlDtl_schedule.Visible = True
        ClearSubControl(pnlDtlEdit_schedule)

        Using DB As New DBHelper
            ShowCodeMasterList(DB, "DELIVERY_FREQ", lstFrequency, True)
            lstFrequency.Items.Remove("Daily")
            lstFrequency.Text = "Weekly" 'Set weekly by default
            ShowCodeMasterList(DB, "DAYOFWEEK", lstOrderbyDate, True)
            ShowCodeMasterList(DB, "DAYOFWEEK", lstDelivery, True)
            'lst = CType(e.Row.FindControl("lstOrderByDiff"), DropDownList)
            'ShowCodeMasterList(DB, "DAYDIFF", lst, True, True)
            ShowCodeMasterListLocal(DB, "TIMEOFDAY", lstOrderbyTime, True)
            ShowCodeMasterListLocal(DB, "AMPM", lstAMPM, True)

            lstOrderBy_order.Items.Clear()
            lstOrderBy_order.Items.Add(New ListItem("JFC Account #", "jfcCustomerNr, dliveryOrder, orderByDate"))
            lstOrderBy_order.Items.Add(New ListItem("Customer Name", "customerName, dliveryOrder, orderByDate"))
            lstOrderBy_order.Items.Add(New ListItem("Delivery date", "dliveryOrder, orderByDate, jfcCustomerNr"))

            txtDeliveryDateFrom.Text = Today.ToString(DATE_FMT)
            txtDeliveryDateTo.Text = Today.AddDays(7).ToString(DATE_FMT)
            If String.IsNullOrEmpty(MasterID) Then
                ViewState("MasterID") = ""
                txtMasterName.Text = ""
                lblMasterID.Text = ""
                btnSaveSchedule.Text = "Add Master"
                btnDeleteSchedule.Visible = False
                txtStartDate.Text = Today.Date.ToString(DATE_FMT)
                pnlDtlAssignedCust.Visible = False
            Else
                SQL = "select m.*, min(d.deliverydate) as startdate from JFCDeliveryMaster m, jfcdeliverymaster_date d where d.DELIVERYMASTER_ID = m.DELIVERYMASTER_ID and m.JFCBRANCHNR = " &
                    DB.SqlEsc(lstBranch.SelectedValue) & "" &
                   " and m.DELIVERYMASTER_ID = :1 group by m.deliverymaster_id,m.jfcbranchnr,m.deliverymastername," &
                   "m.frequency,m.deliveryday,m.orderbyday,m.orderbytime,m.createuser,m.createdate,m.moduser,m.moddate"
                Dim r = DB.GetReader(SQL, MasterID)

                ViewState("MasterID") = MasterID
                If r.Read() Then
                    txtMasterName.Text = CString(r("DELIVERYMASTERNAME"))
                    lblMasterID.Text = MasterID
                    lstFrequency.Text = CString(r("FREQUENCY"))
                    lstDelivery.Text = CString(r("DELIVERYDAY"))
                    lstOrderbyDate.Text = CString(r("ORDERBYDAY"))
                    lstOrderbyTime.Text = Left(CString(r("orderByTime")), 5)
                    lstAMPM.Text = Right(CString(r("orderByTime")), 2)
                    lblCreateUser.Text = CString(r("CreateUser"))
                    txtStartDate.Text = Date.Parse(CString(r("startdate"))).Date.ToString(DATE_FMT)
                    btnSaveSchedule.Text = "Update Master"
                    btnDeleteSchedule.Visible = True


                    showDtlCustDelSchMaster(MasterID)
                    pnlDtlAssignedCust.Visible = True
                End If
            End If
        End Using

    End Sub
    Private MonthData As DataTable

    Protected Sub cldSchedule_DayRender(sender As Object, e As System.Web.UI.WebControls.DayRenderEventArgs) Handles cldSchedule.DayRender



        'If hfActiveTab.Value = CType(Master_Tab.DELIVERYMASTER, Integer).ToString() And pnlList_schedule.Visible Then
        '    Using DB As New DBHelper
        '        ShowDayEventInMasterCalendar(DB, Page, e, lstBranch.Text, False, lnkDelivery)
        '    End Using
        'End If
        ' Load data once per month
        If hfActiveTab.Value = CType(Master_Tab.DELIVERYMASTER, Integer).ToString() AndAlso pnlList_schedule.Visible Then
            If MonthData Is Nothing OrElse MonthData.Rows.Count = 0 Then
                ' Fetch data for the entire month once
                Using DB As New DBHelper
                    MonthData = Common.GetMonthlyData(DB, lstBranch.Text, e.Day.Date)
                End Using
            End If

            ' Render day events from the cached MonthData
            ShowDayEvents(e, False)
        End If

    End Sub


    Private Sub ShowDayEvents(ByRef e As System.Web.UI.WebControls.DayRenderEventArgs, viewOnly As Boolean)
        Dim dayEvents = MonthData.Select("deliveryDate = #" & e.Day.Date.ToString("MM/dd/yyyy") & "#")

        For Each row As DataRow In dayEvents
            Dim headtexts As String() = row("DELIVERYMASTERNAME").ToString().Split(":")
            Dim headname As String = If(headtexts.Length >= 1, headtexts(0).Split(" ")(0), "")

            If viewOnly OrElse e.Day.IsOtherMonth Then
                Dim lbl As New Label()
                lbl.Text = "<br/>" & headname & ": " & row("Orderbydate").ToString()
                e.Cell.Controls.Add(lbl)
            Else
                Dim lnk As New HyperLink()
                lnk.Text = "<br/>" & headname & ": " & row("Orderbydate").ToString()
                lnk.ForeColor = System.Drawing.Color.Navy
                lnk.NavigateUrl = Page.ClientScript.GetPostBackClientHyperlink(lnkDelivery, Format(e.Day.Date, DATE_FMT) & "," & row("DELIVERYMASTER_ID").ToString(), True)
                e.Cell.Font.Bold = True
                e.Cell.Controls.Add(lnk)
            End If
        Next
    End Sub

    Protected Sub lnkDelivery_Click(sender As Object, e As System.EventArgs)
        Dim SQL As String
        Dim str As String
        Dim tok As String()
        Dim dateStr As String
        'Dim custGrpVendorID As String
        'Dim orderdate As String
        Dim r As DataReader
        Dim dt As DataTable
        Dim dr As DataRow
        Dim MasterID As String
        Using DB As New DBHelper
            'setJavaScript(Master_Tab.DELIVERYMASTER)



            str = CString(Request.Form("__EVENTARGUMENT"))
            tok = CString(str).Split(",")
            dateStr = tok(0)
            'orderdate = tok(1)
            MasterID = tok(1)

            ShowCodeMasterList(DB, "TIMEOFDAY", lstPopOrderByTime, True)
            ShowCodeMasterList(DB, "AMPM", lstPopOrderByAmPm, True)

            'Get delivery info
            SQL = "select m.DELIVERYMASTER_ID,m.DELIVERYMASTERNAME, TO_CHAR(d.deliveryDate, :1) deliveryDate, TO_CHAR(d.orderByDate, :2) orderByDate, TO_CHAR(d.orderByDate, :3) orderByTime " & _
                "from jfcdeliverymaster m, jfcdeliverymaster_date d where m.deliverymaster_id = d.deliverymaster_id " & _
                 "and m.jfcbranchnr = :4 and d.deliverydate = TO_DATE(:5, :6)  and m.DELIVERYMASTER_ID = :7 "

            r = DB.GetReader(SQL, DATE_FMT, DATE_FMT, TIME_SQLFMT, lstBranch.Text, dateStr, DATE_FMT, MasterID)
            If r.Read() Then
                lblPopMasterName.Text = CString(r("DELIVERYMASTERNAME"))
                lblPopMasterID.Text = CString(r("DELIVERYMASTER_ID"))
                'lblPopVendorName2.Text = vendorName
                'lblPopCustGrpVendorID.Text = CString(r("custGrpVendor_id"))
                txtPopDeliveryDate.Text = CString(r("deliveryDate"))
                lblPopOldDeliveryDate.Text = CString(r("deliveryDate"))
                'lblPopDeliveryDate.Text = CString(r("deliveryDate"))
                txtPopOrderByDate.Text = CString(r("orderByDate"))
                lstPopOrderByTime.Text = Left(CString(r("orderByTime")), 5)
                lstPopOrderByAmPm.Text = Right(CString(r("orderByTime")), 2)
                ViewState("oldDeliveryOrderDate") = CString(r("orderByDate"))
                ViewState("oldDeliveryOrderTime") = CString(r("orderByTime"))
                lblPopSheduleWarning.Text = "If you update or delete, customers' delivery date will be affected too."
                'lblPopOrderByDate.Text = CString(r("orderByDate")) & " " & CString(r("orderByTime"))

                'lblPopSchedule2.Text = lblPopSchedule.Text
            End If
            r.close()

            popDialog_schedule.Show()

            Exit Sub
        End Using
    End Sub

    Protected Sub grdActiveSchedule_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdActiveSchedule.RowCommand
        Using DB As New DBHelper

            'setJavaScript(Master_Tab.DELIVERYMASTER)
            If e.CommandName = "Page" Then
                'Page is changed
                'showDtlCustDelSchMaster(lblMasterID.Text)
                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False

            ElseIf e.CommandName = "DELETE_ROW" Then
                'deleteOrderReminderRow(e.CommandArgument.ToString())

                ViewState("DIALOG_CMD") = "DELETE_DELIVERYMASTER"
                ViewState("DIALOG_ARG1") = CString(e.CommandArgument)
                Dim msg = " Delivery schedules for customers who use this master will be delete too. Are you sure you want to continue?"

                MessageBoxYesNo(msg, "Confirm Deletion", DB)
            ElseIf e.CommandName = "EDIT_ROW" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()

                showDtlDeliveryMaster(CString(e.CommandArgument))

            End If
        End Using

    End Sub

    Protected Sub lnkBack_schedule_Click(sender As Object, e As System.EventArgs) Handles lnkBack_schedule.Click, btnPopClose.Click
        'setJavaScript(Master_Tab.DELIVERYMASTER)
        'Return back to order form list
        pnlList_schedule.Visible = True
        pnlDtl_schedule.Visible = False
        showDeliveryMasterList()
    End Sub

    Protected Sub SaveSchedule()

        Dim newMaster As Boolean '= pnlMultiSchedule.Visible
        Dim startDateVal As Date
        Dim endDateVal As Date = Nothing
        Dim startDateStr As String
        Dim endDateStr As String


        Dim orderbydate As String = ""
        Dim orderbytime As String = ""
        Using DB As New DBHelper
            'setJavaScript(Master_Tab.DELIVERYMASTER)

            newMaster = IIf(lblMasterID.Text = AUTO OrElse lblMasterID.Text = "", True, False)

            'Check error

            DB.BeginTransaction()

            If newMaster = False Then
                'Delete delivery schedule and the dates
                DeleteMasterDeliverySched(DB, lblMasterID.Text, False)

            End If
            Dim startdate As DateTime = DateTime.Parse(txtStartDate.Text)
            startDateStr = Format(startdate.Date, Common.DATE_FMT)
            DateTime.TryParseExact(startDateStr, DATE_FMT, Nothing, Nothing, startDateVal)
            'txt = grdDeliveryMaster.Rows.Item(i).FindControl("txtEndDate")
            endDateStr = ""
            If endDateStr <> "" Then DateTime.TryParseExact(endDateStr, DATE_FMT, Nothing, Nothing, endDateVal)

            UpSertMasterDelivery(DB, lblMasterID.Text, lstBranch.Text, txtMasterName.Text, lstFrequency.Text, startDateVal, endDateVal, lstDelivery.Text, lstOrderbyDate.Text, lstOrderbyTime.Text & " " & lstAMPM.Text, "", False)
            DB.CommitTransaction()

            'Saved
            InsertEventLog(DB, "Saved delivery master schedule", lblMasterID.Text, Nothing, Nothing, True)


            MessageLine("CONFIRM", "Successfully saved")
            showDeliveryMasterList()
        End Using
    End Sub

    Private Sub DeleteSchedule(MasterID As String)
        Using DB As New DBHelper
            DeleteAllMasterDeliveryDate(DB, MasterID, True)
            DeleteMasterDeliverySched(DB, MasterID, True, True)
            MessageLine("CONFIRM", "Successfully deleted")
        End Using
    End Sub

    Protected Sub btnSaveSchedule_Click(sender As Object, e As System.EventArgs) Handles btnSaveSchedule.Click

        Using DB As New DBHelper
            'setJavaScript(Master_Tab.DELIVERYMASTER)
            'Confirm
            If txtMasterName.Text = "" Then
                MessageBox("Please enter a Name", "Error", DB)

                Exit Sub
            End If
            If lstDelivery.Text = "" Then
                MessageBox("Please select delivery schedule date", "Error", DB)
                Exit Sub
            End If

            If lstOrderbyDate.Text = "" Then
                MessageBox("Please select order schedule date", "Error", DB)
                Exit Sub
            End If

            If lstOrderbyTime.Text = "" Then
                MessageBox("Please select order schedule time", "Error", DB)
                Exit Sub
            End If

            If Date.Parse(txtStartDate.Text) > Today.Date.AddYears(1) Then
                MessageBox("Start date has to be within a year.", "Error", DB)
                Exit Sub
            End If


            For Each row As GridViewRow In grdActiveSchedule.Rows
                Dim lnkBtn As LinkButton
                lnkBtn = row.Cells(2).FindControl("lnkShipName")
                ''Dim DELIVERYDAY As String = row.Cells(GetColumnIndexByName(row, "DELIVERYDAY")).Text
                Dim orderdate As String = row.Cells(5).Text
                Dim ordertime As String = row.Cells(6).Text
                Dim deliverydate As String = row.Cells(4).Text
                Dim frequency As String = row.Cells(3).Text
                If lnkBtn.CommandArgument <> lblMasterID.Text Then
                    If lnkBtn.Text = txtMasterName.Text Then
                        MessageBox("The name of this delivery schedule master already exists. Please input another name.", "Error", DB)
                        Exit Sub
                    End If

                    If orderdate = lstOrderbyDate.Text And deliverydate = lstDelivery.Text And
                                     ordertime = lstOrderbyTime.Text & " " & lstAMPM.Text And
                                     frequency = lstFrequency.Text And
                                     lstFrequency.Text.ToUpper <> "BI-WEEKLY" And
                                     lstFrequency.Text.ToUpper <> "EVERY FOUR WEEKS" Then
                        MessageBox("This delivery schedule master already exists. Please select another one.", "Error", DB)
                        Exit Sub
                    End If
                End If

            Next

            If btnSaveSchedule.Text = "Update Master" Then
                ViewState("DIALOG_CMD") = "SAVE_DELIVERYMASTER"
                ViewState("DIALOG_ARG1") = ""

                Dim msg = " Delivery schedules for customers who use this master will be updated too. Are you sure you want to continue?"
                MessageBoxYesNo(msg, "Confirm Save", DB)
            Else
                SaveSchedule()
            End If

        End Using


    End Sub

    Protected Sub cldSchedule_VisibleMonthChanged(sender As Object, e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles cldSchedule.VisibleMonthChanged
        'setJavaScript(Master_Tab.DELIVERYMASTER)
    End Sub

    Protected Sub btnDeleteSchedule_Click(sender As Object, e As System.EventArgs) Handles btnDeleteSchedule.Click

        Using DB As New DBHelper
            'setJavaScript(Master_Tab.DELIVERYMASTER)
            'Confirm

            ViewState("DIALOG_CMD") = "DELETE_DELIVERYMASTER"
            ViewState("DIALOG_ARG1") = lblMasterID.Text
            Dim msg = " Delivery schedules for customers who use this master will be delete too. Are you sure you want to continue?"

            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using

    End Sub

    Protected Sub btnPopSaveDelivery_Click(sender As Object, e As System.EventArgs) Handles btnPopSaveDelivery.Click
        Dim cmd As String
        Dim DeliveryDate As DateTime
        Dim Orderdate As DateTime

        If DateTime.TryParse(txtPopDeliveryDate.Text, DeliveryDate) And DateTime.TryParse(txtPopOrderByDate.Text, Orderdate) Then

            If DeliveryDate < Orderdate.Date Then
                lblPopSheduleWarning.Text = "Please enter delivery date that is later than order by date"
                'setJavaScript(Master_Tab.DELIVERYMASTER)
                popDialog_schedule.Show()
                Exit Sub
            End If

            Using DB As New DBHelper

                'setJavaScript(Master_Tab.DELIVERYMASTER)

                UpdateMasterDeliveryDate(DB, lblPopMasterID.Text, lblPopOldDeliveryDate.Text, txtPopDeliveryDate.Text,
                                         txtPopOrderByDate.Text & " " & lstPopOrderByTime.Text & " " & lstPopOrderByAmPm.Text)

                showDeliveryMasterList()

                InsertEventLog(DB, "Saved delivery date", txtPopDeliveryDate.Text, Nothing, Nothing, True)
                MessageLine("CONFIRM", "Successfully saved")
            End Using
        End If
    End Sub

    Protected Sub btnPopDeleteDelivery_Click(sender As Object, e As System.EventArgs) Handles btnPopDeleteDelivery.Click


        Using DB As New DBHelper
            'setJavaScript(Master_Tab.DELIVERYMASTER)
            'Delete delivery date
            ' DeleteDeliveryDate(DB, lstCust_schedule.Text, lblPopCustGrpVendorID.Text, lstShipPoint.Text, txtPopDeliveryDate.Text)
            DeleteMasterDeliveryDate(DB, lblPopMasterID.Text, txtPopDeliveryDate.Text, True)
            'Show transaction month

            'showScheduleList()
            InsertEventLog(DB, "Deleted delivery date", txtPopDeliveryDate.Text, Nothing, Nothing, True)
            MessageLine("CONFIRM", "Successfully deleted")
        End Using
    End Sub

    Protected Sub btnAddSchedule_Click(sender As Object, e As System.EventArgs) Handles btnAddSchedule.Click
        showDtlDeliveryMaster("")
    End Sub
#End Region

#Region "Promotional Item"
    Private Sub showPromoItemsList()
        Dim SQL As String
        'setJavaScript(Master_Tab.PROMOTIONITEM)

        Using DB As New DBHelper
            'Show schedule list

            SQL = "SELECT a.item_ID, NVL(p.itemDesc_branch, a.itemDesc) AS itemDesc " &
                    "FROM jfcPromoItem j, jfcItem a " &
                    "LEFT OUTER JOIN jets_price p ON /*i.jfcItem = 'Y' AND*/ p.itemcode = a.item_id AND p.pricegpid = " & DB.SqlEsc(lstBranch.SelectedValue & "-WR") & " " &
                    "where a.item_id = j.item_id " & _
                    "  AND j.JFCBranchNr = " & DB.SqlEsc(lstBranch.SelectedValue)
            'IIf(lstBranch.Text = LST_ALL_STR, "", " AND j.JFCBranchNr = " & DB.SqlEsc(lstBranch.SelectedValue) & "")
            SQL &= " order by TO_NUMBER(j.Item_ID)"
            dsPromotionItem.ProviderName = "System.Data.OracleClient"
            dsPromotionItem.ConnectionString = DB.GetConnectionString
            dsPromotionItem.SelectCommand = SQL
            grdPromotionalItemlist.DataSourceID = "dsPromotionItem"
            'grdOrderReminder.Sort("Settingvalue1", SortDirection.Ascending)
            grdPromotionalItemlist.DataBind()

        End Using

    End Sub

    Private Sub showPromoItemDtl(ByVal ItemID As String)
        'setJavaScript(Master_Tab.PROMOTIONITEM)
        txtPromoItems.Text = ""
        lblPromotionwarning.Text = ""
        pnlDelPromotionwarning.Visible = False
        If String.IsNullOrWhiteSpace(ItemID) Then
            btnPopDeletePromotion.Visible = False
            btnPopSavePromotion.Text = "Add"
            lblPopTitle.Text = "Add"
            lblPreItemId.Text = ""
        Else
            txtPromoItems.Text = ItemID
            lblPreItemId.Text = ItemID
            lblPopTitle.Text = "Edit"
            btnPopSavePromotion.Visible = False
            btnPopDeletePromotion.Visible = True
        End If
        'txtHourprior.Text = ""
        'lblPopWarning.Text = ""
        'pnlPopWarningOrder.Visible = False

        'If prehoursprior.HasValue Then
        '    txtHourprior.Text = prehoursprior.Value
        '    ViewState("prehoursprior") = prehoursprior.Value
        '    lblPopHeader_Order.Text = "Edit"
        '    btnPopSaveOrder.Text = "Update"
        '    btnPopDeleteOrder.Visible = True
        'Else

        '    btnPopDeleteOrder.Visible = False
        '    btnPopSaveOrder.Text = "Add"
        '    lblPopHeader_Order.Text = "Add"
        '    ViewState("prehoursprior") = Nothing
        'End If
        popDialog_Promotion.Show()
    End Sub

    Protected Sub lnkDeletePromoItem_Click(Sender As Object, ByVal e As System.EventArgs) Handles lnkDeletePromoItem.Click
        Dim checkselected As Boolean = False
        Dim msg As String
        Using DB As New DBHelper
            For Each row As GridViewRow In grdPromotionalItemlist.Rows
                Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)
                Dim lblItem = CType(row.FindControl("lnkItemId"), Label)
                If CheckBoxchckrw.Checked Then

                    'DeletePromoItem(DB, lblItem.Text, lstBranch.Text)
                    checkselected = True
                End If
            Next
            If Not checkselected Then
                MessageBox("Please selected at least one promo item.", "Error", DB)
                Exit Sub
            End If

            'setJavaScript(Master_Tab.PROMOTIONITEM)
            '        'Confirm
            ViewState("DIALOG_CMD") = "DELETE_PROMOTIONITEM"


            msg = "Are you sure you want to remove this Promotional Item?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub
    Protected Sub addPromoItem_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPromotionAdd.Click
        showPromoItemDtl("")

    End Sub

    Protected Sub btnPopSavePromoItem_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopSavePromotion.Click
        'setJavaScript(Master_Tab.PROMOTIONITEM)

        Using DB As New DBHelper


            Try
                DBUpdate.UpsertPromoItem(DB, lblPreItemId.Text, CString(txtPromoItems.Text), CString(lstBranch.SelectedValue))
                '        If Val(txtHourprior.Text) <= 0 Or Val(txtHourprior.Text) > 24 Or Not Integer.TryParse(txtHourprior.Text, num) Then
                '            Throw New Exception("Please enter a valid integer number. 1 to 24")
                '        End If
            Catch ex As Exception
                pnlDelPromotionwarning.Visible = True
                lblPromotionwarning.Text = ex.Message
                popDialog_Promotion.Show()
                Exit Sub

            End Try

        End Using
        showPromoItemsList()
    End Sub

    Protected Sub btnPopDeletePromotion_Click(sender As Object, ByVal e As System.EventArgs) Handles btnPopDeletePromotion.Click
        'deleteOrderReminderRow()
        deletePromotionRow(lblPreItemId.Text)
    End Sub
    Protected Sub btnPopClosePromotion_Click(sender As Object, ByVal e As System.EventArgs) Handles popClosePromotion.Click
        'setJavaScript(Master_Tab.PROMOTIONITEM)
    End Sub
    Private Sub deletePromotionRow(ByVal ItemId As String)
        Dim msg As String

        Using DB As New DBHelper
            'setJavaScript(Master_Tab.PROMOTIONITEM)
            '        'Confirm
            ViewState("DIALOG_CMD") = "DELETE_PROMOTIONITEM"
            ViewState("DIALOG_ARG1") = ItemId

            msg = "Are you sure you want to remove this Promotional Item?"
            MessageBoxYesNo(msg, "Confirm Deletion", DB)
        End Using
    End Sub
    Protected Sub grdPromotionalItemlist_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPromotionalItemlist.RowCommand
        Using DB As New DBHelper

            'setJavaScript(Master_Tab.PROMOTIONITEM)
            If e.CommandName = "Page" Then
                'Page is changed

                'Don't remember scroll position
                'Page.MaintainScrollPositionOnPostBack = False

            ElseIf e.CommandName = "DELETE_ROW" Then
                deletePromotionRow(e.CommandArgument.ToString())

            ElseIf e.CommandName = "DETAIL" Then
                'ViewState("Current_custGrpItemID") = e.CommandArgument.ToString()

                showPromoItemDtl(e.CommandArgument.ToString())

            End If
        End Using

    End Sub
    'Protected Sub chckchanged(sender As Object, e As EventArgs)
    '    Dim chckheader As CheckBox = CType(grdPromotionalItemlist.HeaderRow.FindControl("ChkAll"), CheckBox)

    '    For Each row As GridViewRow In grdPromotionalItemlist.Rows
    '        Dim CheckBoxchckrw = CType(row.FindControl("chkRow"), CheckBox)

    '        If chckheader.Checked = True Then
    '            CheckBoxchckrw.Checked = True
    '        Else
    '            CheckBoxchckrw.Checked = False
    '        End If
    '    Next
    'End Sub
#End Region
    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)
        Page.ClientScript.GetPostBackEventReference(Me, String.Empty)
    End Sub

    'Used for sending grdview through email in ctrOrderDtl
    'As calling GridView.RenderControl(htmlTextWriter), the page raises an exception that a Server-Control was rendered outside of a Form. Avoid this execption by overriding VerifyRenderingInServerForm
    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub

    <WebMethod()> _
    Public Shared Function UpdateViewState(ByVal tab As Integer) As String
        HttpContext.Current.Session("MASTER_TAB") = tab
        Dim instance As New BranchSetting

        ' Manually call Page_Load or the relevant logic
        'Dim dummyArgs As New EventArgs()
        'instance.Page_Load(instance, dummyArgs)
        Return New JavaScriptSerializer().Serialize(New With {.status = "success"})
    End Function

End Class


#End ExternalSource
