Imports System.IO
Imports Common

Partial Class Common_ShowFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            'Only a plain .pdf file name in ~/Documents/. Before, "..\Web.config" or "..\Secrets.config" was served,
            'and TYPE could set any content type.
            Dim fileName As String = System.IO.Path.GetFileName(CString(Request.QueryString("NAME")))
            Dim type As String = "application/pdf"
            If fileName = String.Empty OrElse System.IO.Path.GetExtension(fileName).ToLowerInvariant() <> ".pdf" Then
                Response.StatusCode = 404
                Response.End()
                Exit Sub
            End If
            Dim path As String = MapPath(DOC_DIR & fileName)
            Dim myFileInfo As New FileInfo(path)
            If Not myFileInfo.Exists Then
                Response.StatusCode = 404
                Response.End()
                Exit Sub
            End If
            Dim fileLength As String = CString(myFileInfo.Length)

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
        End If
    End Sub
End Class
