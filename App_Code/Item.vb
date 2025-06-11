Imports Microsoft.VisualBasic

Public Class Item
    Dim _minSalCnt As String
    Property customerId As String
    Property GroupItemID As String
    Property custGrpID As String
    Property useCustomerCtg As Boolean
    Property nrCustGrpVendor As Integer
    Property isSamllCust As Boolean

    Property jfcBranchNr As String
    Property jfcCustomerNr As String
    Property IsJfcScaledItem As Boolean
    Property lbsPerEa As Decimal
    Property scaledItem As Boolean
    Property eaPerCs As Integer
    Property csCost As Decimal
    Property avgcswgt As Decimal
    Property avgeawgt As Decimal

    Property csPrice As Decimal
    Property eaPrice As Decimal
    Property lbPrice_OrderByQty As Decimal

    Property minSalCnt() As Decimal
        Get
            Return _minSalCnt
        End Get
        Set(value As Decimal)
            If value = 0 Then
                _minSalCnt = 1
            Else
                _minSalCnt = value
            End If
        End Set
    End Property

    Sub New()

    End Sub

    Sub New(_customerid As String, _custGrpID As String, _GroupItemID As String)
        customerId = _customerid
        custGrpID = _custGrpID
        GroupItemID = _GroupItemID

        Using DB As New DBHelper
            Common.GetJFCBranchAndCustomerNr(DB, _customerid, jfcBranchNr, jfcCustomerNr)
            nrCustGrpVendor = Common.GetNrCustGrpVendor(DB, custGrpID)
            isSamllCust = Common.IsSmallCustomer(DB, _customerid)
            useCustomerCtg = Common.UseCustomerCtg(DB, _customerid)
            IsJfcScaledItem = Common.IsJfcScaledItem(DB, customerId, GroupItemID)
            lbsPerEa = Common.GetJfcLbsPerEa(DB, customerId, GroupItemID)
            scaledItem = Common.IsJfcScaledItem(DB, customerId, GroupItemID)
        End Using
    End Sub

    Function CostPrice() As String
        Dim eaCost, lbCost As Decimal
        Dim eaUnit = IIf(scaledItem, "pc", "ea")
        If scaledItem = True Then

            If lbsPerEa * eaPerCs = 1 Then
                csCost = csCost * avgcswgt '0 'Mod on 8/31/2020
            End If
            If eaPerCs / minSalCnt <= 1 Then
                eaCost = 0
            ElseIf avgcswgt < avgeawgt * 2 Then 'Add on 8/31/2020
                eaCost = 0
            Else
                eaCost = Math.Round(csCost / eaPerCs / lbsPerEa * IIf(avgeawgt = 0, 0.9, avgeawgt), 2)
            End If
            If eaPerCs / minSalCnt <= 1 Then
                lbCost = 0
            Else
                lbCost = Math.Round(csCost / eaPerCs / lbsPerEa, 2)
            End If

            'If Val(r("lbPrice_OrderByQty")) = 0 Then gp = 1 Else gp = (Val(r("lbPrice_OrderByQty")) - lbCost) / Val(r("lbPrice_OrderByQty"))

        Else
            'Regular item
            'csCost = Val(r("csCost"))

            If eaPerCs / minSalCnt <= 1 Then
                eaCost = 0
            Else
                eaCost = Math.Round(csCost / eaPerCs, 2)
            End If
            lbCost = 0

        End If
        Return Common.Displayformat(lbCost, csCost, eaCost, scaledItem)
    End Function



    Public Function Get_GP() As String
        'Dim SQL_lbCost = "(CASE WHEN NVL(ci.eaPerCs, 0) = 0 THEN 0 ELSE ROUND(NVL(p.csCost, 0) / ci.eaPerCs * 10, 2) END)"
        Dim lbCost As Decimal = If(eaPerCs = 0, 0, Math.Round(csCost / eaPerCs * 10, 2))
        Dim result As String = ""
        If lbPrice_OrderByQty <> 0 Then
            result = "GP: " & Math.Round((lbPrice_OrderByQty - lbCost) / lbPrice_OrderByQty, 2).ToString("P1")
        ElseIf csPrice = 0 Then
            result = ""
        Else
            result = "GP: " & Math.Round((csPrice - csCost) / csPrice, 3).ToString("P1")
        End If


        Return result

    End Function


End Class
