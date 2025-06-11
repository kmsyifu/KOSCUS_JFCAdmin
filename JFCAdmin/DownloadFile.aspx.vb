
Partial Class JFCAdmin_DownloadFile
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Session("vFileName"))
            Response.ContentType = "Application/x-msexcel"

            Response.ContentEncoding = System.Text.Encoding.Unicode
            Response.TransmitFile(Server.MapPath(Session("vFileName")))
            Response.Flush()
        End If
    End Sub
End Class
