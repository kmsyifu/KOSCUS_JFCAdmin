Imports System.Data
Imports Common

Partial Class Common_ShowItemDtl
    Inherits System.Web.UI.UserControl
    Private mDB As New DBHelper

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    End Sub

    Public Sub showItemDtl()
        Dim r As DataReader
        Dim avgCsWgt, avgEaWgt As Decimal
        Dim customerID As String
        Dim custGrpItemID As String
        Dim imgUrl As String
        Dim custGrpID As String
        Dim hideEaQty As Boolean

        'Init
        invisibleAllPnl()
        pnlPopItemDtl.Visible = True
        customerID = Session("SHOWITEMDTL_ARG1")
        custGrpItemID = Session("SHOWITEMDTL_ARG2")
        hideEaQty = Common.HideEaQty(mDB, customerID)

        'Set properties
        custGrpID = GetCustGrpID(mDB, customerID)
        pnlPopOrderForm.Visible = IIf(GetNrItemGrpInCustGrp(mDB, custGrpID) > 1, True, False)
        pnlPopVendor.Visible = IIf(GetNrCustGrpVendor(mDB, custGrpID) > 1, True, False)
        pnlPopShowItemDtl.Visible = Common.ShowItemDtl(mDB, customerID)
        pnlPopShowPrice.Visible = IIf(Common.ApplyInEmailOrCustomer(mDB, customerID, "HidePrice") = True, False, True)

        'Get item info
        r = mDB.GetReader(GetSQL_itemDtl(mDB, customerID, custGrpItemID))
        If r.Read() Then
            lblPopCustGrpItemID.Text = CString(r("custGrpItem_id"))
            imgUrl = GetItemImgUrl(CString(r("itemType")), CString(r("item_id")))
            imgPopItem.ImageUrl = imgUrl
            imgPopItem2.ImageUrl = imgUrl
            lblPopItemDesc.Text = CString(r("itemDesc_view"))
            lblPopJfcRemark.Text = CString(r("jfcRemark"))
            lblPopItemIdNr.Text = CString(r("itemIdNr_view"))
            lblPopItemGrpName.Text = CString(r("itemGrpName"))
            lblPopVendorName.Text = CString(r("vendorName"))
            lblPopCtgName.Text = CString(r("ctgName_view"))
            lblPopCsPrice.Text = String.Format("{0:C}", Val(r("csprice"))) & " / cs"
            lblPopCsPrice.Visible = IIf(Val(r("csprice")) <> 0, True, False)
            lblPopEaPrice.Text = String.Format("{0:C}", Val(r("eaprice"))) & IIf(Val(r("lbPrice_orderByQty")) <> 0, " / pc", " / ea")
            lblPopEaPrice.Visible = IIf(hideEaQty = False AndAlso Val(r("eaprice")) <> 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
            lblPopMinSalCnt_view.Text = IIf(IsMobile(), "<br />", "") & "min order: " & Val(r("minSalCnt")) & " ea"
            lblPopMinSalCnt_view.Visible = IIf(hideEaQty = False AndAlso Val(r("minSalCnt")) > 1 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False) ' And Val(r("eaprice")) <> 0
            lblPopLbPrice_OrderByWgt.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByWgt"))) & " / lb"
            lblPopLbPrice_OrderByWgt.Visible = IIf(Val(r("lbPrice_OrderByWgt")) <> 0, True, False)
            lblPopLbPrice_OrderByQty.Text = String.Format("{0:C}", Val(r("lbPrice_OrderByQty"))) & " / lb"
            lblPopLbPrice_OrderByQty.Visible = IIf(Val(r("lbPrice_OrderByQty")) <> 0, True, False)
            If lblPopLbPrice_OrderByQty.Visible = True Then
                lblPopCsPrice.Text = IIf(IsMobile(), "<br />", "") & lblPopCsPrice.Text
                'lblPopCsPrice.Text = IIf(IsMobile(), "<br />", "") & "(" & lblPopCsPrice.Text & ")"
                lblPopEaPrice.Text = lblPopEaPrice.Text
                'lblPopEaPrice.Text = "(" & lblPopEaPrice.Text & ")"
            End If
            lblPopAskPrice.Visible = IIf(Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0 AndAlso Val(r("lbPrice_OrderByWgt")) = 0 AndAlso Val(r("lbPrice_OrderByQty")) = 0, True, False)
            pnlPopEaPerCs.Visible = IIf(hideEaQty = False, True, False)
            lblPopEaPerCs.Text = Val(r("eaPerCs")) & " ea / cs"
            lblPopEaPerCs.Visible = IIf(Val(r("eaPerCs")) <> 0 AndAlso Val(r("eaPerCs")) <> 1 AndAlso Val(r("lbPrice_OrderByQty")) = 0 AndAlso Val(r("minSalCnt")) <> Val(r("eaPerCs")), True, False)
            lblPopQtyHistory.Text = GetQtyHistory(mDB, customerID, custGrpItemID, 3)
            lblPopQtyHistoryTitle.Visible = IIf(lblPopQtyHistory.Text = String.Empty, False, True)
            lblPopQtyHistory.Visible = IIf(lblPopQtyHistory.Text = String.Empty, False, True)
            avgCsWgt = Val(r("avgcswgt")) 'GetAvgWgt(Val(r("csPrice")), Val(r("lbPrice_OrderByQty")))
            avgEaWgt = Val(r("avgeawgt")) 'GetAvgWgt(Val(r("eaPrice")), Val(r("lbPrice_OrderByQty")))
            lblPopAvgCsWgt.Text = String.Format("{0:N}", avgCsWgt) & " lbs / cs"
            lblPopAvgCsWgt.Visible = IIf(avgCsWgt <> 0 AndAlso (Val(r("csprice")) <> 0 OrElse (Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0)), True, False)
            lblPopAvgEaWgt.Text = String.Format("{0:N}", avgEaWgt) & " lbs / pc"
            lblPopAvgEaWgt.Visible = IIf(avgEaWgt <> 0 AndAlso (Val(r("eaprice")) <> 0 OrElse (Val(r("csprice")) = 0 AndAlso Val(r("eaprice")) = 0)), True, False)
            lblPopAvgWgtTitle.Visible = lblPopAvgCsWgt.Visible OrElse lblPopAvgEaWgt.Visible
            lblPopAvailQty.Visible = False
            If Session("SYS_SESSION_USERTYPE") = "JFCADMIN" AndAlso CString(r("jfcitem")) = "Y" Then
                lblPopAvailQty.Visible = True
                lblPopAvailQty.Text = CString(r("qtyAvail"))
            End If
            If pnlPopShowItemDtl.Visible = True Then
                'lblPopGrossWeight.Text = String.Format("{0:N2}", Val(r("grossWeight_lbs"))) & " lbs, " & String.Format("{0:N3}", Val(r("grossWeight_kg"))) & " kg"
                lblPopGrossWeight.Text = CString(r("grossWeight_lbs")) & " lbs, " & CString(r("grossWeight_kg")) & " kg"
                'lblPopGrossWeight.Visible = IIf(Val(r("grossWeight_lbs")) = 0, False, True)
                'lblPopCaseVol.Text = String.Format("{0:N2}", Val(r("caseVol_ft3"))) & " ft³, " & String.Format("{0:N3}", Val(r("caseVol_m3"))) & " m³"
                lblPopCaseVol.Text = CString(r("caseVol_ft3")) & " ft³, " & CString(r("caseVol_m3")) & " m³"
                'lblPopCaseVol.Visible = IIf(Val(r("caseVol_ft3")) = 0, False, True)
                'lblPopPallet.Text = Val(r("block")) & " x " & Val(r("high")) & " = " & Val(r("block")) * Val(r("high")) 'TODO: Add after JETS stores correct values
                'lblPopPallet.Visible = IIf(Val(r("block")) = 0 AndAlso Val(r("high")) = 0, False, True)
                lblPopIngredient.Text = CString(r("ingredient"))
            End If
        End If
        r.close()
        'Show popup
        popDialog.Show()

    End Sub

    'Error occurs when uncomment below
    'Protected Sub btnPopClose_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPopClose.Click
    'invisibleAllPnl()
    '    popDialog.Dispose()
    'End Sub

    Private Sub invisibleAllPnl()
        pnlPopItemDtl.Visible = False
    End Sub
End Class
