Imports System.IO
Imports Common

Partial Class Common_ShowFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            'Only a plain .pdf file name in ~/Documents/. Before, "..\Web.config" or "..\Secrets.config" was served to
            'anyone (this page needs no login), and TYPE could set any content type.
            Dim fileName As String = System.IO.Path.GetFileName(CString(Request.QueryString("NAME")))
            Dim type As String = "application/pdf"
            Dim fileLength As String = ""
            Dim myFileInfo As FileInfo = Nothing
            If fileName <> String.Empty AndAlso System.IO.Path.GetExtension(fileName).ToLowerInvariant() = ".pdf" Then
                myFileInfo = New FileInfo(MapPath(DOC_DIR & fileName))
            End If
            Dim path As String = If(myFileInfo Is Nothing, "", myFileInfo.FullName)
            If myFileInfo IsNot Nothing AndAlso myFileInfo.Exists Then
                fileLength = CString(myFileInfo.Length)
                Response.Clear()

                Response.AddHeader("Accept-Ranges", "ascii")
                Response.AddHeader("Content-Length", fileLength)
                Response.AddHeader("Cache-Control", "post-check=0, pre-check=0")
                Response.ContentType = type
                Response.AddHeader("Content-Disposition", "inline; filename=""" + fileName + """")
                Response.AddHeader("Cache-Control", "no-store, no-cache, must-revalidate")
                Response.AddHeader("Pragma", "no-cache")

                Response.Flush()
                Response.WriteFile(path)
                Response.End()
            Else
                MessageBox("Could not find file " & fileName, "Error")
            End If


           



        End If
    End Sub
End Class
