Imports System.IO
Imports Common

Partial Class Common_ShowFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim fileName As String = Request.QueryString("NAME")
            Dim type As String = IIf(Request.QueryString("TYPE") = String.Empty, "application/pdf", Request.QueryString("TYPE"))
            Dim path As String = MapPath(DOC_DIR & fileName)
            Dim fileLength As String
            Dim myFileInfo As New FileInfo(path)
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
        End If
    End Sub
End Class
