#ExternalChecksum("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\ShowFile.aspx.vb","{ff1816ec-aa5e-4d10-87f7-6f4963833460}","1B5098B1711F44D66E57D0BB87EB053EAEFFC6DC")

#ExternalSource("C:\Users\yfu\source\repos\KOSCUS_JFCAdmin\Login\ShowFile.aspx.vb",1)
Imports System.IO
Imports Common

Partial Class Common_ShowFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            Dim fileName As String = Request.QueryString("NAME")
            Dim type As String = IIf(Request.QueryString("TYPE") = String.Empty, "application/pdf", Request.QueryString("TYPE"))
            Dim path As String = MapPath(DOC_DIR & fileName)
            Dim fileLength As String = ""
            Dim myFileInfo As New FileInfo(path)
            If myFileInfo.Exists Then
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


#End ExternalSource
