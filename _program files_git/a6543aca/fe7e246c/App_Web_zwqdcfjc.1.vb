#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\DownloadFile.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","92C9DB212AF7ACF1753EEABDB403157C68C14EC6")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\JFCAdmin\DownloadFile.aspx.vb",1)

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


#End ExternalSource
