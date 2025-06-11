Imports System.Data
Imports Common
Imports DBUpdate
Imports AjaxControlToolkit
Imports System.IO
Imports Winnovative.ExcelLib

Partial Class Tool
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


        Dim email As String

        Using DB As New DBHelper
            'Post process for modal dialog box
            If Request("DialogResult") = "YES" Then
                dialogResult_yes(ViewState("DIALOG_CMD"), ViewState("DIALOG_ARG1"))
            ElseIf Request("DialogResult") = "NO" Then
                dialogResult_no(ViewState("DIALOG_CMD"))
            End If


            If IsPostBack = False Then
                'Init
                DataBind() 'Fill in customer company type

                ''Show company list
                email = GetUserEmail(DB)
                If IsJfcHoAdmin(DB, email) Or IsKmsSupport(DB, email) Then
                    ShowBranchList(DB, email, True, lstBranch, True, LST_ALL_STR)
                Else
                    ShowBranchList(DB, email, False, lstBranch)
                End If


                'Get parameters
                ViewState("Tool_TAB") = IIf(Request.QueryString("TAB") <> "", Request.QueryString("TAB"), Tool_Tab.Report)


                'Select tab
                setJavaScript(ViewState("Tool_TAB"))

                '--------------------------------------------------------------------------------------------------------------------
                'Keyword Form
                '--------------------------------------------------------------------------------------------------------------------
                ShowList(DB, "select Report_id as id, Reportname AS name from report order by dispOrder", lstKeywordReport, False)

                ViewState("IsExcepPriceCustomer") = Common.IsExcepPriceCustomer(DB, ViewState("CUSTSETTING_CUSTID"))

                pnlDateFromTo.Visible = IIf(CString(DB.GetScalar("select FILTERBYDATE from report where Report_id = :1", lstKeywordReport.SelectedValue)) = "Y", True, False)
                pnlItemCode.Visible = IIf(CString(DB.GetScalar("select FILTERBYITEM from report where Report_id = :1", lstKeywordReport.SelectedValue)) = "Y", True, False)
            End If
        End Using
    End Sub


    Private Sub dialogResult_yes(ByVal dialogCmd As String, ByVal dialogArg1 As String)
        Dim id As String = dialogArg1

        Using DB As New DBHelper
            Select Case dialogCmd
                'Case "DELETE_HOMEPAGE"
                '    setJavaScript(Tool_Tab.Report)
                '    deleteHomepageImg(DB, Path.GetFileNameWithoutExtension(id), lstBranch.SelectedValue)
                '    DeleteHomePageFile(lstBranch.SelectedValue, id)
                '    InsertEventLog(DB, "Deleted Homepageimg", Nothing, Nothing, Nothing, True)
                '    MessageLine("CONFIRM", "Successfully deleted")

                'Case "DELETE_PRICEINQUIRY"
                '    setJavaScript(MTool_Tab.Report)

                '    InsertEventLog(DB, "Deleted Price Inquiry's item", Nothing, Nothing, Nothing, True)
                '    MessageLine("CONFIRM", "Successfully deleted")
                Case Else

            End Select
        End Using
    End Sub

    Private Sub dialogResult_no(ByVal dialogCmd As String)
        Select Case dialogCmd
            Case "DELETE_CUSTITEM"
                setJavaScript(Tool_Tab.Report)

            Case Else
        End Select
    End Sub

    Private Sub setJavaScript(ByVal tab As Tool_Tab)
        JavaScript = "$('#tabs').tabs('select', " & convertToLocalTab(tab) & ");" 'selectSubmenu(" & tab & ");"
        'JavaScript &= "scrollTo(0, 0);" 'TODO: Doesn't work
    End Sub

    Private Function convertToLocalTab(ByVal tab As Tool_Tab) As Integer
        Select Case tab
            Case Tool_Tab.Report
                Return 0


        End Select
        Return 0
    End Function



#Region "Report"
    Protected Sub lstKeywordReport_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstKeywordReport.SelectedIndexChanged
        Dim ShowDateTime As Boolean
        If lstKeywordReport.SelectedValue IsNot Nothing Then
            Using DB As New DBHelper
                ShowDateTime = IIf(DB.GetScalar("select FILTERBYDATE from report where Report_id = :1", lstKeywordReport.SelectedValue) = "Y", True, False)
                pnlDateFromTo.Visible = ShowDateTime

                pnlItemCode.Visible = IIf(CString(DB.GetScalar("select FILTERBYITEM from report where Report_id = :1", lstKeywordReport.SelectedValue)) = "Y", True, False)

                If Not pnlItemCode.Visible Then
                    txtItemcode.Text = ""
                End If
            End Using
        End If
        
    End Sub

    Protected Sub btnExcel_Click(send As Object, e As EventArgs) Handles btnExportExcel.Click

        setJavaScript(Tool_Tab.Report)
        Dim rtSQL, wheresql As String
        Dim r As DataRow
        Dim ds As New DataSet

        If Not pnlDateFromTo.Visible Then
            txtFromDate.Text = ""
            txtToDate.Text = ""
        End If
       
        Using DB As New DBHelper
            If DateTime.TryParseExact(txtFromDate.Text, DATE_FMT, Nothing, Nothing, Nothing) = False And txtFromDate.Text <> "" Then
                MessageBox("From date has to be in " & DATE_FMT.ToLower & " format", "Error", DB)
                Exit Sub
            End If
            If DateTime.TryParseExact(txtToDate.Text, DATE_FMT, Nothing, Nothing, Nothing) = False And txtToDate.Text <> "" Then
                MessageBox("To date has to be in " & DATE_FMT.ToLower & " format", "Error", DB)
                Exit Sub
            End If
            r = DB.GetDataRow("select * from Report where Report_id = :1 ", lstKeywordReport.SelectedValue)
            '"and t.bid =:1"
            wheresql = "where 1 = 1"

            rtSQL = CString(r("sql"))

            If txtFromDate.Text <> "" Then
                wheresql &= " And  TRUNC(t.Createdate) >= to_date( " & DB.SqlEsc(txtFromDate.Text) & ",'MM/DD/YYYY')"
            End If
            If txtToDate.Text <> "" Then
                wheresql &= " And TRUNC(t.Createdate) <= to_date( " & DB.SqlEsc(txtToDate.Text) & ",'MM/DD/YYYY')"
            End If
            If lstBranch.SelectedValue <> "" Then
                wheresql &= " And t.bid = " & DB.SqlEsc(lstBranch.SelectedValue)
            End If
            'rtSQL = String.Format("select * from ({0}) t " & wheresql, CString(r("sql")))
            If pnlItemCode.Visible And String.IsNullOrWhiteSpace(txtItemcode.Text) Then
                'ClientScript.RegisterStartupScript(Me.GetType(), "stopShowProgress", "stopShowProgress()", True)
                MessageBox("Item # can not be empty.", "Error")
                Exit Sub
            End If
        

            If txtItemcode.Text <> "" Then
                Dim str As String = ""
                Dim Itemcodes As String() = txtItemcode.Text.Split(",")

                For Each itemcode In Itemcodes
                    str &= DB.SqlEsc(itemcode) & ","
                Next

                str = str.Remove(str.Length - 1, 1)

                If rtSQL.Contains("{0}") Then
                    rtSQL = String.Format(rtSQL, str)
                    'wheresql = ""
                Else
                    wheresql &= " And t.item_id in (" & str & ") "
                End If

            End If

            rtSQL = String.Format("select * from ({0}) t " & wheresql, rtSQL)




            Dim dt As DataTable = DB.GetDataTable(rtSQL, lstBranch.SelectedValue)
            dt.TableName = CString(r("REPORTNAME"))
            ds.Tables.Add(dt)
            exportExcel(ds)

            'Dim csm As ClientScriptManager = Page.ClientScript
            'csm.

        End Using
    End Sub


    Protected Sub exportExcel(ds As DataSet)
        Dim book As New ExcelWorkbook(ExcelWorkbookFormat.Xlsx_2007)

        book.LicenseKey = "ORIIGQoIGQgPDxkLFwkZCggXCAsXAAAAAA=="

        Dim vFileName As String = lstKeywordReport.SelectedItem.ToString().Replace(" ", "") & " " & DateTime.Now.ToString("yyyyMMdd") & ".xlsx"
        Dim lstHeader As New List(Of String)
        Dim vMaxCol As Integer
        MessageBox("Item # can not be empty.", "Error")
        For i As Integer = 0 To ds.Tables.Count - 1
            Dim sheet As ExcelWorksheet

            book.Worksheets.AddWorksheet()
            sheet = book.Worksheets(i)
            Dim dt = ds.Tables(i)
            vMaxCol = dt.Columns.Count + 20
            sheet.Name = dt.TableName
            'sheet.encoding = Encoding.UTF8

            'sheet(1, 1).Text = "JFC INTERNATIONAL INC."

            sheet(1, 1).Text = "Date: " & DateTime.Now.ToShortDateString
            sheet(2, 1).Text = "Report: " & dt.TableName & "  "
            sheet(3, 1).Text = "Branch: " & lstBranch.SelectedItem.Text
            Dim strlist As List(Of String) = New List(Of String)
            If txtFromDate.Text <> "" Then
                strlist.Add("From: " & txtFromDate.Text & " ")
            End If
            If txtToDate.Text <> "" Then
                strlist.Add("To: " & txtToDate.Text & " ")
            End If
            If txtItemcode.Text <> "" Then
                strlist.Add("Item#: " & txtItemcode.Text & " ")
            End If
            For j As Integer = 0 To strlist.Count() - 1
                sheet(j + 4, 1).Text = strlist(j)
            Next
            Dim k As Long = 0
            Try
                For cnt = 0 To dt.Columns.Count - 1
                    With dt.Columns(cnt)

                        SetExcelSheetTextVal(sheet, 8, cnt + 1, CString(.ColumnName))
                    End With
                Next
                For cntRow = 0 To dt.Rows.Count - 1
                    With dt.Rows(cntRow)
                        For cnt = 0 To dt.Columns.Count - 1
                            'If .GetType.ToString() = "String" Then
                            k = cntRow
                            SetExcelSheetTextVal(sheet, cntRow + 9, cnt + 1, CString(.Item(cnt)))
                            'Else
                            '   SetExcelSheetNumericVal(sheet, cntRow + 9, cnt + 1, .Item(cnt))
                            'End If
                        Next
                        '                SetExcelSheetCodeType2(sheet, 6 + cntRow, .Cells(0).Text, .Cells(2).Text, vMaxCol)
                    End With
                Next
            Catch ex2 As IndexOutOfRangeException
                MessageBox("There are too many data to download. Please specify a date range", "ERROR")
            Catch ex As Exception
                MessageBox(ex.Message, "ERROR")

            End Try

            sheet.AutofitColumns()

        Next

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MyFunction", "javascript:window.location.href = 'Tool.aspx';", True)
        Response.Clear()
        Response.ContentType = "Application/x-msexcel"
        Response.AddHeader("Content-Disposition", "attachment; filename=" + vFileName)
        'Response.ContentEncoding = System.Text.Encoding.UTF8

        Dim cookie As HttpCookie = New HttpCookie("ExcelDownloadFlag", "Flag")

        cookie.Expires = DateTime.Now.AddDays(1)
        cookie.HttpOnly = False

        Response.AppendCookie(cookie)
        book.Save(Response.OutputStream)
        book.Close()
        'MessageBox("Item # can not be empty.", "Error")
        'Response.TransmitFile(Server.MapPath(vFileName))
        'Response.WriteFile(vFileName)
        'Response.Redirect("Tool.aspx", False)
        'Response.Flush()

        Response.End()


        '

    End Sub

    Private Sub SetExcelSheetCodeType(ByRef pSheet As ExcelWorksheet, pRow As Integer, pTextVal As String, pSrtAcctGroup As String, pMaxCol As Integer)
        Dim vText As String = ""

        Select Case pTextVal

            'Case
            '    If CInt(pSrtAcctGroup) > 7 Then
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.FillType = ExcelCellFillType.SolidFill
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.SolidFillOptions.BackColor = Drawing.Color.Magenta
            '    Else
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.FillType = ExcelCellFillType.SolidFill
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.SolidFillOptions.BackColor = Drawing.Color.Orange
            '    End If

            'Case "1"
            '    If CInt(pSrtAcctGroup) > 8 Then
            '        vText = "Group"
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.FillType = ExcelCellFillType.SolidFill
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.SolidFillOptions.BackColor = Drawing.Color.LightSalmon
            '    Else
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.FillType = ExcelCellFillType.SolidFill
            '        vText = "Code3"
            '        pSheet(pRow, 1, pRow, pMaxCol).Style.Fill.SolidFillOptions.BackColor = Drawing.Color.LightBlue
            '    End If
            Case "2" : vText = "Code6"
        End Select
        'pSheet(pRow, 2).Text = vText
    End Sub


    Public Function RemoveSpecialMarks(ByVal input As String) As String
        ' Define regular expression pattern to match special mark characters
        Dim pattern As String = "[^\w\s.,!?'/:\-\\]"

        ' Remove special mark characters using regular expression
        Dim output As String = Regex.Replace(input, pattern, "")

        ' Return the modified string
        Return output
    End Function
    Private Sub SetExcelSheetTextVal(ByRef pSheet As ExcelWorksheet, pRow As Integer, pCol As Integer, pTextVal As String)
        ' Create a string with special characters
        'Dim inputString As String = "Special Character: ü"

        ' Convert the string to UTF-8 encoding
        'Dim utf8Encoding As Encoding = New UTF8Encoding()
        'Dim encodedBytes As Byte() = utf8Encoding.GetBytes(pTextVal)

        ' Convert the encoded bytes to a UTF-8 encoded string
        'Dim encodedString As String = utf8Encoding.GetString(encodedBytes)

        pSheet(pRow, pCol).Text = RemoveSpecialMarks(pTextVal)
    End Sub

    Private Sub SetExcelSheetNumericVal(ByRef pSheet As ExcelWorksheet, pRow As Integer, pCol As Integer, pTextVal As String, Optional ByVal pDecimalPoint As Integer = 2)
        If pTextVal = "&nbsp;" OrElse pTextVal = "" Then
            'pSheet(pRow, pCol).NumberValue = ""
        Else
            pSheet(pRow, pCol).NumberValue = pTextVal
            Select Case pDecimalPoint
                Case 0
                    pSheet(pRow, pCol).Style.Number.NumberFormatString = "#,##0;(#,##0);0"
                Case 1
                    pSheet(pRow, pCol).Style.Number.NumberFormatString = "#,##0.0;(#,##0.0);0"
                Case Else
                    pSheet(pRow, pCol).Style.Number.NumberFormatString = "#,##0.00;(#,##0.00);0"
            End Select
        End If
    End Sub
#End Region

    'Used for sending grdview through email in ctrOrderDtl
    'As calling GridView.RenderControl(htmlTextWriter), the page raises an exception that a Server-Control was rendered outside of a Form. Avoid this execption by overriding VerifyRenderingInServerForm
    Overrides Sub VerifyRenderingInServerForm(control As Control)
        'Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time. 
    End Sub


   
End Class
