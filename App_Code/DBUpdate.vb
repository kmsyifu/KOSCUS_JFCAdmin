Imports Microsoft.VisualBasic
Imports Common

Public Class DBUpdate

    '--------------------------------------------------------------------------------------------------------------------
    'Update web user
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub UpdateWebUser(ByVal DB As DBHelper, ByVal email As String, ByVal firstName As String, ByVal lastName As String, ByVal phone As String, ByVal cellular As String, ByVal fax As String, ByVal timeZone As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Modify existing company info
        SQL = "UPDATE webuser SET firstname = :1, lastName = :2, phone = :3, cellular = :4, fax = :5, timeZone = :6 " & _
                "WHERE email = :7"
        DB.ExecuteNonQuery(SQL, firstName, lastName, phone, cellular, fax, timeZone, email)
        InsertEventLog(DB, "UPDATE webUser", email, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdateWebUserFlatPassword(ByVal DB As DBHelper, ByVal email As String, ByVal flatPassword As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Modify existing company info
        SQL = "UPDATE webuser SET flatPassword = :1 WHERE email = :2"
        DB.ExecuteNonQuery(SQL, flatPassword, email)
        InsertEventLog(DB, "UPDATE flatPassword", email, flatPassword, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub UpSertWebUser(ByVal DB As DBHelper, ByVal cmd As String, ByVal email As String, ByVal firstName As String, ByVal lastName As String, ByVal phone As String, _
    '                                ByVal cellular As String, ByVal fax As String, ByVal timeZone As String, ByVal custCompanyID As String, ByVal userCompanyID As String, _
    '                                ByVal isAdmin As Boolean, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String
    '    Dim userID As Integer
    '    Dim userType As String = IIf(isAdmin = True, "ADMIN", "NEW")

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    If cmd = "INSERT_CUSTOMER" Then
    '        'Add a new web user (Email is in lower case)
    '        SQL = "SELECT email FROM webuser WHERE email = :1"
    '        If CString(DB.GetScalar(SQL, email)) = "" Then
    '            userID = Val(DB.GetSequence("USER_ID", 0))
    '            SQL = "INSERT INTO webuser (email, user_id, passwordReset, firstName, lastName, phone, cellular, fax, timeZone) " & _
    '                    "VALUES (:1," & userID & ",'N',:2,:3,:4,:5,:6,:7)"
    '            DB.ExecuteNonQuery(SQL, email, firstName, lastName, phone, cellular, fax, timeZone)
    '            InsertEventLog(DB, "INSERT INTO webuser", email, Nothing, Nothing, False, False)
    '        End If
    '        SQL = "INSERT INTO webuser_customer (email, customer_id, custCompany_id, userCompany_id) VALUES (:1,:2,:3,:4)"
    '        DB.ExecuteNonQuery(SQL, email, userType, custCompanyID, userCompanyID)
    '        InsertEventLog(DB, "INSERT INTO webuser_customer", email, userType, custCompanyID, False, False)
    '    ElseIf cmd = "UPDATE_CUSTOMER" Then
    '        'Modify existing web user
    '        SQL = "UPDATE webuser SET firstName = :1, lastName = :2, phone = :3, cellular = :4, fax = :5, timeZone = :6 " & _
    '                "WHERE email = :7"
    '        DB.ExecuteNonQuery(SQL, firstName, lastName, phone, cellular, fax, timeZone, email)
    '        SQL = "DELETE FROM webuser_customer WHERE email = :1 AND customer_id IN ('ADMIN', 'NEW') AND custCompany_id = :2"
    '        DB.ExecuteNonQuery(SQL, email, custCompanyID)
    '        InsertEventLog(DB, "DELETE FROM webuser_customer", email, Nothing, custCompanyID, False, False)
    '        SQL = "INSERT INTO webuser_customer (email, customer_id, custCompany_id, userCompany_id) VALUES (:1,:2,:3,:4)"
    '        DB.ExecuteNonQuery(SQL, email, userType, custCompanyID, userCompanyID)
    '        InsertEventLog(DB, "DELETE FROM webuser_customer", email, userType, custCompanyID, False, False)
    '    End If

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    'Public Shared Sub DeleteWebUser(ByVal DB As DBHelper, ByVal email As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Delete user and its detail
    '    SQL = "DELETE FROM webuser_customer WHERE email = :1"
    '    DB.ExecuteNonQuery(SQL, email)
    '    InsertEventLog(DB, "DELETE FROM webuser_customer", email, Nothing, Nothing, False, False)
    '    SQL = "DELETE FROM webuser WHERE email = :1"
    '    DB.ExecuteNonQuery(SQL, email)
    '    InsertEventLog(DB, "DELETE FROM webuser", email, Nothing, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub DeleteFlatPassword(ByVal DB As DBHelper, ByVal email As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete user and its detail
        SQL = "UPDATE webuser set flatPassword = '' WHERE email = :1"
        DB.ExecuteNonQuery(SQL, email)
        InsertEventLog(DB, "UPDATE webuser", email, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Update customer
    '--------------------------------------------------------------------------------------------------------------------

    Public Shared Sub DeleteUserRemovalRequest(ByVal DB As DBHelper, ByVal email As String, ByVal jfcBranchNr As String, ByVal jfcCustomerNr As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM UserRemovalRequest " & _
                        "WHERE EMAIL = :1 AND jfcBranchNr = :2 AND jfcCustomerNr = :3"
        DB.ExecuteNonQuery(SQL, email, jfcBranchNr, jfcCustomerNr)
        InsertEventLog(DB, "DELETE FROM UserRemovalRequest", email, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub SendUserRemovalRequest(ByVal DB As DBHelper, ByVal email As String, ByVal jfcnumber As String, customername As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "Select* from UserRemovalRequest where EMAIL = :1 AND jfcBranchNr = :2 AND jfcCustomerNr =:3"
        Dim r As DataReader = DB.GetReader(SQL, email, jfcnumber.Substring(0, 2), jfcnumber.Substring(2))
        If r.Read() Then
            SQL = "UPDATE UserRemovalRequest SET JFCBRANCHNR = :1, JFCCUSTOMERNR = :2, STATUS = :3, CUSTOMERNAME =:4,CREATEUSER=:5, CREATEDATE = sysdate " & _
               "Where Email =:6 AND jfcBranchNr = :7 AND jfcCustomerNr = :8"
            DB.ExecuteNonQuery(SQL, jfcnumber.Substring(0, 2), jfcnumber.Substring(2), "N", customername, GetUserEmail(DB), email, jfcnumber.Substring(0, 2), jfcnumber.Substring(2))
        Else

            SQL = "INSERT INTO UserRemovalRequest (JFCBRANCHNR, JFCCUSTOMERNR, STATUS, EMAIL,CUSTOMERNAME,CREATEUSER,CREATEDATE) " & _
                      "VALUES (:1,:2,:3,:4,:5,:6,sysdate)"
            DB.ExecuteNonQuery(SQL, jfcnumber.Substring(0, 2), jfcnumber.Substring(2), "N", email, customername, GetUserEmail(DB))
        End If

        InsertEventLog(DB, "Send UserRemovalRequest ", email, jfcnumber.Substring(0, 2), jfcnumber.Substring(2), False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Update Price Inquiry
    '--------------------------------------------------------------------------------------------------------------------

    'Public Shared Sub UpdatePriceInquiry(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Mark price inquiry as sent
    '    SQL = "UPDATE webUser_priceInquiry SET sent ='Y' WHERE email = :1 AND customer_id = :2"
    '    DB.ExecuteNonQuery(SQL, email, customerID)
    '    InsertEventLog(DB, "UPDATE webUser_priceInquiry", email, customerID, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub UpdatePriceInquiryResponse(ByVal DB As DBHelper, ByVal CUSTGRPITEM_ID As String, ByVal customerID As String, Optional ResponseType As String = "Y", Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Mark price inquiry as sent
        SQL = "UPDATE webUser_priceInquiry SET RESPONDED ='" & ResponseType & "' WHERE CUSTGRPITEM_ID = :1 AND customer_id = :2"
        DB.ExecuteNonQuery(SQL, CUSTGRPITEM_ID, customerID)
        InsertEventLog(DB, "UPDATE webUser_priceInquiry", CUSTGRPITEM_ID, customerID, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub DeletePriceInquiry(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String, ByVal custGrpItemID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Remove customer's favorite item
    '    SQL = "DELETE FROM webUser_priceInquiry WHERE sent = 'N' AND email = :1 AND customer_id = :2 AND custGrpItem_id = :3"
    '    DB.ExecuteNonQuery(SQL, email, customerID, custGrpItemID)
    '    InsertEventLog(DB, "DELETE FROM webUser_priceInquiry", email, customerID, custGrpItemID, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    'Public Shared Sub DeleteAllPriceInquiry(ByVal DB As DBHelper, ByVal email As String, ByVal customerID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Remove customer's favorite item
    '    SQL = "DELETE FROM webUser_priceInquiry WHERE sent = 'N' AND email = :1 AND customer_id = :2"
    '    DB.ExecuteNonQuery(SQL, email, customerID)
    '    InsertEventLog(DB, "DELETE FROM webUser_priceInquiry", email, customerID, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Update Delivery Schedule
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub UpSertDeliverySched(ByVal DB As DBHelper, ByRef deliverySchedID As String, ByRef deliverySchedLineNr As Integer, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, _
                                          ByVal deliveryMasterID As String, ByVal frequency As String, ByVal startDateVal As Date, ByVal endDateVal As Date, _
                                          ByVal deliveryDay As String, ByVal orderByDay As String, ByVal orderByTime As String, ByVal sendReminder As String, ByVal year As String, Optional isOneTrans As Boolean = True, Optional isUpate As Boolean = False)
        Dim SQL As String
        Dim deliveryDayCode, orderByDayCode As Integer
        Dim skipWeek As Boolean
        Dim curDateVal As Date
        Dim i As Integer
        Dim nDiffDays As Integer

        If isOneTrans = True Then DB.BeginTransaction()

        If year = "" Then
            If deliverySchedID = AUTO OrElse deliverySchedID = "" Then
                'Add a delivery schedule
                deliverySchedID = DB.GetSequence("DELIVERYSCHED_ID", DELIVERYSCHED_ID_LEN)
            End If
            'Insert delivery schedule
            SQL = "INSERT INTO customer_deliverySched (deliverySched_id, deliverySchedLineNr, customer_id, custGrpVendor_id, shipPoint_id, deliveryMaster_id, frequency, startDate, endDate, deliveryDay, orderByDay, orderByTime, sendReminder) " & _
                    "VALUES (:1," & deliverySchedLineNr & ",:2,:3,:4,:5,:6,TO_DATE(:7,:8), " & _
                                IIf(endDateVal = Nothing, "NULL, ", "NULL, ") & _
                                ":9,:10,:11,:12)"
            'IIf(endDateVal = Nothing, "NULL, ", "TO_DATE(" & DB.SqlEsc(Format(endDateVal, DATE_FMT)) & ", " & DB.SqlEsc(DATE_FMT) & "), ") & _
            DB.ExecuteNonQuery(SQL, deliverySchedID, customerID, custGrpVendorID, shipPointID, deliveryMasterID, frequency, Format(startDateVal, DATE_FMT), DATE_FMT, deliveryDay, orderByDay, orderByTime, sendReminder)
            'InsertEventLog(DB, "INSERT INTO customer_deliverySched", deliverySchedID, CString(deliverySchedLineNr), Nothing, False, False)
            '2024/1/2 YI FU
            'Else
            ''Modify existing delivery schdule
            'SQL = "UPDATE customer_deliverySched SET custGrpVendor_id = :1, frequency = :2, startDate = TO_DATE(:3, :4)" & _
            '            ", endDate = " & IIf(endDateVal = Nothing, "NULL", "TO_DATE(" & DB.SqlEsc(Format(endDateVal, DATE_FMT)) & ", " & DB.SqlEsc(DATE_FMT) & ")") & _
            '            ", deliveryDay = :5, orderByDay = :6, orderByTime = :7, sendReminder = :8 " & _
            '            "WHERE deliverySched_id = :9"
            'DB.ExecuteNonQuery(SQL, custGrpVendorID, frequency, Format(startDateVal, DATE_FMT), DATE_FMT, deliveryDay, orderByDay, orderByTime, sendReminder, deliverySchedID)
            'InsertEventLog(DB, "UPDATE customer_deliverySched", deliverySchedID, Nothing, Nothing, False, False)

            ''Delete delivery dates
            'DeleteDeliveryDate(DB, deliverySchedID, False)
            'End If
        Else
            'Delete delivery dates in the specified year
            DeleteDeliveryDateInYear(DB, deliverySchedID, year, False)
        End If

        '10/22/2021 add startdateVal In DeliveryMaster_ID.
        SQL = "Select DELIVERYMASTER_ID ,TO_CHAR(deliveryDate, :1) deliveryDate, TO_CHAR(orderByDate, :2) orderByDate from Jfcdeliverymaster_Date where DELIVERYMASTER_ID = :3 and deliveryDate >=:4"
        Dim r As DataReader = DB.GetReader(SQL, DATE_FMT, DATETIME_SQLFMT, deliveryMasterID, startDateVal)
        Dim hasValue As Boolean = False
        While r.Read()
            InsertDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, CString(r("DELIVERYDATE")), CString(r("ORDERBYDATE")), sendReminder, deliverySchedID, deliverySchedLineNr, False)
            hasValue = True
        End While
        r.close() 'Added by jzhou on 1/3/2024

        If Not hasValue Then

            'Get difference between order by day and delivery day
            Select Case frequency.ToUpper
                Case "DAILY"
                    nDiffDays = IIf(orderByDay = "Same day", 0, Left(orderByDay, 1))
                Case "WEEKLY", "BI-WEEKLY", "EVERY FOUR WEEKS"
                    SQL = "SELECT code_id FROM codeMaster WHERE codeDesc = :1"
                    deliveryDayCode = Val(DB.GetScalar(SQL, deliveryDay))
                    SQL = "SELECT code_id FROM codeMaster WHERE codeDesc = :2"
                    orderByDayCode = Val(DB.GetScalar(SQL, orderByDay))
                    i = orderByDayCode
                    For nDiffDays = 0 To 6 'Sunday to Saturday
                        If i = deliveryDayCode Then Exit For
                        i += 1
                        If i = 7 Then i = 0
                    Next
            End Select

            'Start with startDateVal or today whichever is larger 
            curDateVal = IIf(DateTime.Compare(startDateVal, Date.Now) > 0, startDateVal, Date.Today)
            'curDateVal = startDateVal
            'Assign ending date to be the last day of next year if it doesn't exist
            If endDateVal = Nothing Then endDateVal = New Date(Now.Year + 2, 1, 1).AddDays(-1)
            If isUpate Then
                endDateVal = New Date(startDateVal.Year + 2, 1, 1).AddDays(-1)
            End If
            'If frequency.ToUpper = "ONE-TIME" Then
            '    InsertDeliveryDate(DB,customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), sendReminder, deliverySchedID, deliverySchedLineNr, False)
            'Else
            'Insert delivery dates
            skipWeek = False
            While curDateVal <= endDateVal
                Select Case frequency.ToUpper
                    Case "DAILY"
                        InsertDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, sendReminder, deliverySchedID, deliverySchedLineNr, False)
                    Case "WEEKLY"
                        If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                            InsertDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, sendReminder, deliverySchedID, deliverySchedLineNr, False)
                            curDateVal = curDateVal.AddDays(1)
                            Continue While
                        End If
                    Case "BI-WEEKLY"
                        If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                            If skipWeek = False Then
                                InsertDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, sendReminder, deliverySchedID, deliverySchedLineNr, False)
                                skipWeek = True
                                curDateVal = curDateVal.AddDays(1)
                                Continue While
                            Else
                                skipWeek = False
                            End If
                        End If
                    Case "EVERY FOUR WEEKS"
                        If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                            InsertDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, sendReminder, deliverySchedID, deliverySchedLineNr, False)
                            curDateVal = curDateVal.AddDays(28)
                            Continue While
                        End If

                End Select
                curDateVal = curDateVal.AddDays(1)
            End While
        End If

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteDeliverySched(ByVal DB As DBHelper, ByVal deliverySchedID As String, ByVal deliveryschedlinenr As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        DeleteDeliveryDate(DB, deliverySchedID, deliveryschedlinenr, False)

        'Delete delivery schedule
        SQL = "DELETE FROM customer_deliverySched WHERE deliverySched_id = :1 and deliveryschedlinenr = :2"
        DB.ExecuteNonQuery(SQL, deliverySchedID, deliveryschedlinenr)
        'InsertEventLog(DB, "DELETE FROM customer_deliverySched", deliverySchedID, deliveryschedlinenr, Nothing, False, False)
        'yi FU 01/02/2024
        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteAllDeliverySched(ByVal DB As DBHelper, ByVal customerid As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        DeleteAllDeliveryDate(DB, customerid, False)

        'Delete delivery schedule
        SQL = "DELETE FROM customer_deliverySched WHERE CUSTOMER_ID = :1 "
        DB.ExecuteNonQuery(SQL, customerid)
        InsertEventLog(DB, "DELETE FROM customer_all_deliverySched", customerid, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub DeleteDeliverySchedInCust(ByVal DB As DBHelper, ByVal customerID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    SQL = "DELETE FROM customer_deliverySched WHERE customer_id = :1"
    '    DB.ExecuteNonQuery(SQL, customerID)
    '    InsertEventLog(DB, "DELETE FROM customer_deliverySched", customerID, Nothing, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    'Public Shared Sub DeleteDeliverySchedInVendor(ByVal DB As DBHelper, ByVal custGrpVendorID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    SQL = "DELETE FROM customer_deliverySched WHERE custGrpVendor_id = :1"
    '    DB.ExecuteNonQuery(SQL, custGrpVendorID)
    '    InsertEventLog(DB, "DELETE FROM customer_deliverySched", custGrpVendorID, Nothing, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub InsertDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String, _
                                         ByVal orderByDateTime As String, ByVal sendReminder As String, ByVal deliverySchedID As String, ByVal deliverySchedLineNr As Integer, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'TODO: Add a parameter skipJfcHoliday. If it's is true, don't insert if it's JFC holiday (exclue Saturday and Sunday and only if the vendor is JFC vendor. custGrp_vendor.vendor_id IN (SELECT vendor_id FROM jfcVendor). 
        'Don't skip if JFC calendar doesn't exist. Need to update the dates when uploading the JFC calendar)

        'If date already exists, delete
        'If ExistDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, deliveryDate) = True Then Return False
        DeleteDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, deliveryDate, False)

        SQL = "INSERT INTO customer_deliveryDate (customer_id, custGrpVendor_id, shipPoint_id, " & _
                " deliveryDate, orderByDate, sendReminder, deliverySched_id, deliverySchedLineNr) " & _
                "VALUES (:1,:2,:3, TO_DATE(:4,:5), TO_DATE(:6,:7),:8,:9," & deliverySchedLineNr & ")"
        DB.ExecuteNonQuery(SQL, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT, orderByDateTime, DATETIME_SQLFMT, sendReminder, deliverySchedID)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdateDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal oldDeliveryDate As String, ByVal newDeliveryDate As String, ByVal orderByDateTime As String, ByVal sendReminder As String, Optional isOneTrans As Boolean = True, Optional keeplinerNumber As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        If oldDeliveryDate <> newDeliveryDate Then
            'If date already exists, delete
            'If ExistDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, newDeliveryDate) = True Then Return False
            DeleteDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, newDeliveryDate, False)
        End If

        SQL = "UPDATE customer_deliveryDate SET deliveryDate = TO_DATE(:1, :2), orderByDate = TO_DATE(:3, :4), sendReminder = :5 " & IIf(keeplinerNumber, "", ",DELIVERYSCHEDLINENR = NULL ") &
                " WHERE customer_id = :6 AND shipPoint_id = :7 AND custGrpVendor_id = :8 AND deliveryDate = TO_DATE(:9, :10)"
        DB.ExecuteNonQuery(SQL, newDeliveryDate, DATE_FMT, orderByDateTime, DATETIME_SQLFMT, sendReminder, customerID, shipPointID, custGrpVendorID, oldDeliveryDate, DATE_FMT)
        InsertEventLog(DB, "UPDATE customer_deliveryDate", customerID, shipPointID, custGrpVendorID, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteDeliveryDate(ByVal DB As DBHelper, ByVal deliverySchedID As String, ByVal deliveryschedlinenr As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        'If deleteFutureOnly = True Then
        '    SQL = "DELETE FROM customer_deliveryDate WHERE deliveryDate >= TRUNC(SYSDATE) AND deliverySched_id = " & DB.SqlEsc(deliverySchedID)
        'Else
        SQL = "DELETE FROM customer_deliveryDate WHERE deliverySched_id = :1 AND deliveryschedlinenr =:2 AND deliveryDate >= TRUNC(SYSDATE)"
        'End If
        DB.ExecuteNonQuery(SQL, deliverySchedID, deliveryschedlinenr)
        InsertEventLog(DB, "DELETE FROM customer_deliveryDate", deliverySchedID, deliveryschedlinenr, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteAllDeliveryDate(ByVal DB As DBHelper, ByVal customerid As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM customer_deliveryDate WHERE CUSTOMER_ID = :1 AND deliveryDate >= TRUNC(SYSDATE)"
        'End If
        DB.ExecuteNonQuery(SQL, customerid)
        InsertEventLog(DB, "DELETE FROM customer_alldeliveryDate", customerid, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteDeliveryDate(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpVendorID As String, ByVal shipPointID As String, ByVal deliveryDate As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM customer_deliveryDate WHERE customer_id = :1 AND custGrpVendor_id =:2 And shipPoint_id = :3 AND deliveryDate = TO_DATE(:4, :5)"
        DB.ExecuteNonQuery(SQL, customerID, custGrpVendorID, shipPointID, deliveryDate, DATE_FMT)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteDeliveryDateInYear(ByVal DB As DBHelper, ByVal deliverySchedID As String, ByVal year As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim firstDayOfYear As Date = New DateTime(year, 1, 1)
        Dim lastDayOfYear As Date = New DateTime(year + 1, 1, 1).AddDays(-1)

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM customer_deliveryDate " & _
                    "WHERE deliverySched_id = :1" & _
                    " AND deliveryDate >= TO_DATE(:2, :3) " & _
                    " AND deliveryDate <= TO_DATE(:4, :5)"
        DB.ExecuteNonQuery(SQL, deliverySchedID, Format(firstDayOfYear, DATE_FMT), DATE_FMT, Format(lastDayOfYear, DATE_FMT), DATE_FMT)
        'InsertEventLog(DB, "DELETE FROM customer_deliveryDate", deliverySchedID, year, Nothing, False, False)
        'YI FU 01/02/2024
        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub DeleteDeliveryDateInCust(ByVal DB As DBHelper, ByVal customerID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    SQL = "DELETE FROM customer_deliveryDate WHERE customer_id = :1"
    '    DB.ExecuteNonQuery(SQL, customerID)
    '    InsertEventLog(DB, "DELETE FROM customer_deliveryDate", customerID, Nothing, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    'Public Shared Sub DeleteDeliveryDateInVendor(ByVal DB As DBHelper, ByVal custGrpVendorID As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    SQL = "DELETE FROM customer_deliveryDate WHERE custGrpVendor_id = :1"
    '    DB.ExecuteNonQuery(SQL, custGrpVendorID)
    '    InsertEventLog(DB, "DELETE FROM customer_deliveryDate", custGrpVendorID, Nothing, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub UpSertMasterDelivery(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal BranchNr As String, MasterName As String, ByVal frequency As String, _
                                           ByVal startDateVal As Date, ByVal endDateVal As Date, ByVal deliveryDay As String, ByVal orderByDay As String, ByVal orderByTime As String,
                                           ByVal year As String, Optional isOneTrans As Boolean = True, Optional isUpdate As Boolean = False)
        Dim SQL As String
        Dim deliveryDayCode, orderByDayCode As Integer
        Dim skipWeek As Boolean
        Dim curDateVal As Date
        Dim i As Integer
        Dim nDiffDays As Integer
        Dim r As DataReader
        If isOneTrans = True Then DB.BeginTransaction()

        If year = "" Then
            If deliveryMasterID = AUTO OrElse deliveryMasterID = "" Then
                'Add a delivery schedule
                deliveryMasterID = DB.GetSequence("DELIVERYMASTER_ID", DELIVERYMASTER_ID_LEN)
            Else
                DeleteAllMasterDeliveryDateAfterStartdate(DB, deliveryMasterID, startDateVal, False)
            End If

            'Insert delivery schedule
            SQL = "INSERT INTO Jfcdeliverymaster (DELIVERYMASTER_ID, JFCBRANCHNR, DELIVERYMASTERNAME, frequency, deliveryDay, orderByDay, orderByTime,CREATEUSER,CREATEDATE) " & _
                    "VALUES (:1,:2,:3,:4,:5,:6,:7,:8,sysdate)"
            DB.ExecuteNonQuery(SQL, deliveryMasterID, BranchNr, MasterName, frequency, deliveryDay, orderByDay, orderByTime, GetUserEmail(DB))
            'InsertEventLog(DB, "INSERT INTO Jfcdeliverymaster", deliveryMasterID, "", Nothing, False, False)
            ' 01/02/2024 YI FU
        Else
            DeleteMasterDeliveryDateInYear(DB, deliveryMasterID, year, False)

        End If

        'Get difference between order by day and delivery day
        Select Case frequency.ToUpper
            Case "DAILY"
                nDiffDays = IIf(orderByDay = "Same day", 0, Left(orderByDay, 1))
            Case "WEEKLY", "BI-WEEKLY", "EVERY FOUR WEEKS"
                SQL = "SELECT code_id FROM codeMaster WHERE codeDesc = :1"
                deliveryDayCode = Val(DB.GetScalar(SQL, deliveryDay))
                SQL = "SELECT code_id FROM codeMaster WHERE codeDesc = :2"
                orderByDayCode = Val(DB.GetScalar(SQL, orderByDay))
                i = orderByDayCode
                For nDiffDays = 0 To 6 'Sunday to Saturday
                    If i = deliveryDayCode Then Exit For
                    i += 1
                    If i = 7 Then i = 0
                Next

        End Select

        'Start with startDateVal or today whichever is larger 
        '2/2/2023
        curDateVal = IIf(DateTime.Compare(startDateVal, Date.Now) > 0, startDateVal, Date.Today)
     

        'Assign ending date to be the last day of next year if it doesn't exist
        If endDateVal = Nothing Then endDateVal = New Date(Now.Year + 2, 1, 1).AddDays(-1)
        'If isUpdate Then
        '    endDateVal = New Date(startDateVal.Year + 1, 1, 1).AddDays(-1)
        'End If
        'If frequency.ToUpper = "ONE-TIME" Then
        '    InsertDeliveryDate(DB,customerID, custGrpVendorID, shipPointID, Format(curDateVal, DATE_FMT), sendReminder, deliverySchedID, deliverySchedLineNr, False)
        'Else
        'Insert delivery dates
        skipWeek = False
        While curDateVal <= endDateVal
            Select Case frequency.ToUpper
                Case "DAILY"
                    InsertMasterDeliveryDate(DB, deliveryMasterID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, False)
                Case "WEEKLY"
                    If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                        InsertMasterDeliveryDate(DB, deliveryMasterID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, False)
                        curDateVal = curDateVal.AddDays(1)
                        Continue While
                    End If
                Case "BI-WEEKLY"
                    If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                        If skipWeek = False Then
                            InsertMasterDeliveryDate(DB, deliveryMasterID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, False)
                            skipWeek = True
                            curDateVal = curDateVal.AddDays(1)
                            Continue While
                        Else
                            skipWeek = False
                        End If
                    End If
                Case "EVERY FOUR WEEKS"
                    If DateTime.Parse(curDateVal).DayOfWeek = deliveryDayCode Then
                        InsertMasterDeliveryDate(DB, deliveryMasterID, Format(curDateVal, DATE_FMT), Format(curDateVal.AddDays(nDiffDays * -1), DATE_FMT) & " " & orderByTime, False)
                        curDateVal = curDateVal.AddDays(28)
                        Continue While
                    End If
            End Select
            curDateVal = curDateVal.AddDays(1)
        End While


        If year = "" Then
            'Update each customer with this master scheclude
            SQL = "select * from customer_deliverySched where DELIVERYMASTER_ID = :1"
            r = DB.GetReader(SQL, deliveryMasterID)
            While r.Read()
                DeleteDeliverySched(DB, CString(r("DELIVERYSCHED_ID")), CString(r("DELIVERYSCHEDLINENR")), False)
                UpSertDeliverySched(DB, CString(r("DELIVERYSCHED_ID")), CString(r("DELIVERYSCHEDLINENR")), CString(r("CUSTOMER_ID")), CString(r("CUSTGRPVENDOR_ID")),
                                    CString(r("SHIPPOINT_ID")), deliveryMasterID, frequency, startDateVal, endDateVal, deliveryDay, orderByDay,
                                    orderByTime, CString(r("SENDREMINDER")), year, False, isUpdate)
            End While
        End If

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteDeliveryMasterDate(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal deliveryDate As String, ByVal orderByDateTime As String)
        Dim SQL = "DELETE FROM Jfcdeliverymaster_Date WHERE DELIVERYMASTER_ID = :1 AND DELIVERYDATE =  TO_DATE(:2,:3) AND ORDERBYDATE = TO_DATE(:4,:5)"
        DB.ExecuteNonQuery(SQL, deliveryMasterID, deliveryDate, DATE_FMT, orderByDateTime, DATETIME_SQLFMT)
    End Sub

    Public Shared Sub InsertMasterDeliveryDate(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal deliveryDate As String, _
                                        ByVal orderByDateTime As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        If isOneTrans = True Then DB.BeginTransaction()
        DeleteDeliveryMasterDate(DB, deliveryMasterID, deliveryDate, orderByDateTime)

        SQL = "INSERT INTO Jfcdeliverymaster_Date (DELIVERYMASTER_ID, DELIVERYDATE, ORDERBYDATE) " & _
                "VALUES (:1, TO_DATE(:2,:3), TO_DATE(:4,:5))"
        DB.ExecuteNonQuery(SQL, deliveryMasterID, deliveryDate, DATE_FMT, orderByDateTime, DATETIME_SQLFMT)


        If isOneTrans Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdateMasterDeliveryDate(ByVal DB As DBHelper, ByVal DeliveryMasterID As String, ByVal oldDeliveryDate As String, ByVal newDeliveryDate As String,
                                               ByVal orderByDateTime As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'If oldDeliveryDate <> newDeliveryDate Then
        '    If date already exists, delete
        '        If ExistDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, newDeliveryDate) = True Then Return False
        '        DeleteMasterDeliveryDate(DB, customerID, custGrpVendorID, shipPointID, newDeliveryDate, False)
        '    End If

        SQL = "UPDATE Jfcdeliverymaster_Date SET deliveryDate = TO_DATE(:1, :2), orderByDate = TO_DATE(:3, :4) " & _
                "WHERE DELIVERYMASTER_ID =:5 AND deliveryDate = TO_DATE(:6, :7)"
        DB.ExecuteNonQuery(SQL, newDeliveryDate, DATE_FMT, orderByDateTime, DATETIME_SQLFMT, DeliveryMasterID, oldDeliveryDate, DATE_FMT)
        InsertEventLog(DB, "UPDATE Jfcdeliverymaster_Date", DeliveryMasterID, newDeliveryDate, orderByDateTime, False, False)

        SQL = "select * from customer_deliverySched c, customer_deliverydate d where c.deliverysched_id = d.deliverysched_id " &
            "and d.deliverydate = to_date(:1,:2) and c.deliverymaster_id = :3"

        Dim r = DB.GetReader(SQL, oldDeliveryDate, DATE_FMT, DeliveryMasterID)
        While r.Read()
            UpdateDeliveryDate(DB, CString(r("CUSTOMER_ID")), CString(r("CUSTGRPVENDOR_ID")), CString(r("SHIPPOINT_ID")), oldDeliveryDate, newDeliveryDate, orderByDateTime, CString(r("SENDREMINDER")), False)
        End While
        If isOneTrans = True Then DB.CommitTransaction()
    End Sub


    Public Shared Sub DeleteAllMasterDeliveryDate(ByVal DB As DBHelper, ByVal deliveryMasterID As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        'If deleteFutureOnly = True Then
        '    SQL = "DELETE FROM customer_deliveryDate WHERE deliveryDate >= TRUNC(SYSDATE) AND deliverySched_id = " & DB.SqlEsc(deliverySchedID)
        'Else
        SQL = "DELETE FROM Jfcdeliverymaster_Date WHERE DELIVERYMASTER_ID = :1  "
        DB.ExecuteNonQuery(SQL, deliveryMasterID)
        InsertEventLog(DB, "DELETE FROM Jfcdeliverymaster_Date", deliveryMasterID, Nothing, Nothing, Nothing, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteMasterDeliveryDate(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal deliveryDate As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        'If deleteFutureOnly = True Then
        '    SQL = "DELETE FROM customer_deliveryDate WHERE deliveryDate >= TRUNC(SYSDATE) AND deliverySched_id = " & DB.SqlEsc(deliverySchedID)
        'Else
        SQL = "DELETE FROM Jfcdeliverymaster_Date WHERE DELIVERYMASTER_ID = :1 and DELIVERYDATE = to_date(:2,:3)"
        DB.ExecuteNonQuery(SQL, deliveryMasterID, deliveryDate, DATE_FMT)
        InsertEventLog(DB, "DELETE FROM Jfcdeliverymaster_Date", deliveryMasterID, Nothing, Nothing, Nothing, False)
        SQL = "select * from customer_deliverySched c, customer_deliverydate d where c.deliverysched_id = d.deliverysched_id " &
           "and d.deliverydate = to_date(:1,:2) and c.deliverymaster_id = :3"

        Dim r = DB.GetReader(SQL, deliveryDate, DATE_FMT, deliveryMasterID)
        While r.Read()
            DeleteDeliveryDate(DB, CString(r("CUSTOMER_ID")), CString(r("CUSTGRPVENDOR_ID")), CString(r("SHIPPOINT_ID")), deliveryDate, False)
        End While

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteMasterDeliveryDateInYear(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal year As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim firstDayOfYear As Date = New DateTime(year, 1, 1)
        Dim lastDayOfYear As Date = New DateTime(year + 1, 1, 1).AddDays(-1)

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM Jfcdeliverymaster_Date " & _
                    "WHERE DELIVERYMASTER_ID = :1" & _
                    " AND deliveryDate >= TO_DATE(:2, :3) " & _
                    " AND deliveryDate <= TO_DATE(:4, :5)"
        DB.ExecuteNonQuery(SQL, deliveryMasterID, Format(firstDayOfYear, DATE_FMT), DATE_FMT, Format(lastDayOfYear, DATE_FMT), DATE_FMT)
        'InsertEventLog(DB, "DELETE FROM customer_deliveryDate", deliveryMasterID, year, Nothing, False, False)
        'YI FU 01/02/
        If isOneTrans = True Then DB.CommitTransaction()
        'SQL = "DELETE FROM Jfcdeliverymaster_Date WHERE DELIVERYMASTER_ID = :1  "
        'DB.ExecuteNonQuery(SQL, deliveryMasterID)
        'InsertEventLog(DB, "DELETE FROM Jfcdeliverymaster_Date", deliveryMasterID, Nothing, Nothing, Nothing, False, False)

        'If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteAllMasterDeliveryDateAfterStartdate(ByVal DB As DBHelper, ByVal deliveryMasterID As String, ByVal startDate As Date, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        'If deleteFutureOnly = True Then
        '    SQL = "DELETE FROM customer_deliveryDate WHERE deliveryDate >= TRUNC(SYSDATE) AND deliverySched_id = " & DB.SqlEsc(deliverySchedID)
        'Else
        SQL = "DELETE FROM Jfcdeliverymaster_Date WHERE DELIVERYMASTER_ID = :1 AND deliveryDate >= TO_DATE(:2, :3)  "
        DB.ExecuteNonQuery(SQL, deliveryMasterID, Format(startDate, DATE_FMT), DATE_FMT)
        InsertEventLog(DB, "DELETE FROM Jfcdeliverymaster_Date startdate", deliveryMasterID, startDate, Nothing, Nothing, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteMasterDeliverySched(ByVal DB As DBHelper, ByVal MasterID As String, Optional isOneTrans As Boolean = True, Optional DeleteSched As Boolean = False)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete delivery dates
        'DeleteDeliveryDate(DB, deliverySchedID, deliveryschedlinenr, False)

        'Delete delivery schedule
        SQL = "DELETE FROM jfcdeliverymaster WHERE DELIVERYMASTER_ID =:1"
        DB.ExecuteNonQuery(SQL, MasterID)
        'Update each customer with this master scheclude
        If DeleteSched Then

            SQL = "select * from customer_deliverySched where DELIVERYMASTER_ID = :1"
            Dim r = DB.GetReader(SQL, MasterID)
            While r.Read()
                DeleteDeliverySched(DB, CString(r("DELIVERYSCHED_ID")), CString(r("DELIVERYSCHEDLINENR")), False)
            End While
        End If
        InsertEventLog(DB, "DELETE FROM jfcdeliverymaster", MasterID, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Update Payment account
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub UpSertPaymentAcc(ByVal DB As DBHelper, ByVal customerID As String, ByRef accNickName As String, ByVal paymentTypeID As String, ByVal accName As String, ByVal routingNr As String, ByVal bankAccNr As String, _
                                       ByVal cardTypeID As String, ByVal cardNr As String, ByVal expMonth As String, ByVal expYear As String, ByVal securityCode As String, ByVal tempAcc As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)
        Dim newAccNickName As String = String.Empty

        If isOneTrans = True Then DB.BeginTransaction()

        'Craete a nick name
        Select Case paymentTypeID
            Case "C"
                newAccNickName = accName & " Checking XXXXXXXXX-" & Right(bankAccNr, 4)
            Case "S"
                newAccNickName = accName & " Savings XXXXXXXXX-" & Right(bankAccNr, 4)
            Case "CARD"
                newAccNickName = accName & " " & cardTypeID & " XXXXXXXXX-" & Right(cardNr, 4)
        End Select

        If accNickName = AUTO OrElse accNickName = "" Then
            'Add a new payment account (encrypt account #, routing #, credit card # and security code)
            SQL = "INSERT INTO customer_payAcc (customer_id, accNickName, paymentType_id, accName, routingNr, bankAccNr " & _
                    ", cardType_id, cardNr, expMonth, expYear, securityCode, tempAcc, createUser, createDate) " & _
                    "VALUES (:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,SYSDATE)"
            DB.ExecuteNonQuery(SQL, customerID, newAccNickName, paymentTypeID, accName, Encrypt256(routingNr), Encrypt256(bankAccNr), _
                               cardTypeID, Encrypt256(cardNr), expMonth, expYear, Encrypt256(securityCode), tempAcc, userEmail)
            InsertEventLog(DB, "INSERT INTO customer_payAcc", customerID, newAccNickName, Nothing, False, False)
        Else
            'Modify existing payment account (encrypt account #, routing #, credit card # and security code)
            SQL = "UPDATE customer_payAcc SET accNickName = :1, accName = :2, routingNr = :3, expMonth = :4, expYear = :5, securityCode = :6 " & _
                    "WHERE customer_id = :7 AND accNickName = :8"
            DB.ExecuteNonQuery(SQL, newAccNickName, accName, Encrypt256(routingNr), expMonth, expYear, Encrypt256(securityCode), customerID, accNickName)
            InsertEventLog(DB, "UPDATE customer_payAcc", customerID, accNickName, newAccNickName, False, False)
        End If

        accNickName = newAccNickName

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub DeletePaymentAcc(ByVal DB As DBHelper, ByVal customerID As String, ByVal accNickName As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Delete this payment account
    '    SQL = "DELETE FROM customer_payAcc WHERE customer_id = :1 AND accNickName = :2"
    '    DB.ExecuteNonQuery(SQL, customerID, accNickName)
    '    InsertEventLog(DB, "DELETE FROM customer_payAcc", customerID, accNickName, Nothing, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub DeleteTempPaymentAcc(ByVal DB As DBHelper, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete this payment account
        SQL = "DELETE FROM customer_payAcc WHERE tempAcc = 'Y'"
        DB.ExecuteNonQuery(SQL)
        InsertEventLog(DB, "DELETE FROM customer_payAcc", "tempAcc = 'Y'", Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub InsertPayHdr(ByVal DB As DBHelper, ByVal paymentID As String, ByVal customerID As String, ByVal accNickName As String, ByVal advancePayAmt As Decimal, ByVal paymentAmt As Decimal, ByVal message As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Add payment header
        SQL = "INSERT INTO customer_payHdr (payment_id, customer_id, accNickName, paymentStatus_id, advancePayAmt, paymentAmt, message, createUser, createDate) " & _
                "VALUES (:1,:2,:3,'N'," & advancePayAmt & "," & paymentAmt & ",:4,:5,SYSDATE)"
        DB.ExecuteNonQuery(SQL, paymentID, customerID, accNickName, Left(message, 256), userEmail)
        InsertEventLog(DB, "INSERT INTO customer_payHdr", paymentID, customerID, accNickName, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub InsertPayDtl(ByVal DB As DBHelper, ByVal paymentID As String, ByVal arNumber As String, ByVal paymentLineAmt As Decimal, ByVal note As String, ByVal arDate As String, ByVal arType As String, ByVal sectionCode As String, _
                                   ByVal arAmt As Decimal, ByVal recvAmt As Decimal, ByVal discAmt As Decimal, ByVal pendingAmt As Decimal, ByVal webDiscAmt As Decimal, ByVal reference As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Add payment detail
        SQL = "INSERT INTO customer_payDtl (payment_id, arNumber, paymentLineAmt, note, arDate, arType, sectionCode, arAmt, recvAmt, discAmt, pendingAmt, webDiscAmt, reference) " & _
                "VALUES (:1,:2," & paymentLineAmt & ",:3,TO_DATE(:4,:5),:6,:7," & arAmt & "," & recvAmt & "," & discAmt & "," & pendingAmt & "," & webDiscAmt & ",:8)"
        DB.ExecuteNonQuery(SQL, paymentID, arNumber, Left(note, 256), arDate, DATE_FMT, arType, sectionCode, reference)
        InsertEventLog(DB, "INSERT INTO customer_payDtl", paymentID, arNumber, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdatePayHdr(ByVal DB As DBHelper, ByVal paymentID As String, ByVal traceNr As String, ByVal authorizationCode As String, ByVal transactionType As String, ByVal approvedAmt As Decimal, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "UPDATE customer_payHdr SET traceNr = :1, authorizationCode = :2, transactionType = :3, approvedAmt = " & approvedAmt & ", modUser = :4, modDate = SYSDATE " & _
                "WHERE payment_id = " & DB.SqlEsc(paymentID)
        DB.ExecuteNonQuery(SQL, traceNr, authorizationCode, transactionType, userEmail)
        InsertEventLog(DB, "UPDATE customer_payHdr", paymentID, traceNr, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdatePaymentStatus(ByVal DB As DBHelper, ByVal paymentID As String, ByVal paymentStatusID As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "UPDATE customer_payHdr SET paymentStatus_id = :1, modUser = :2, modDate = SYSDATE WHERE payment_id = " & DB.SqlEsc(paymentID)
        DB.ExecuteNonQuery(SQL, paymentStatusID, userEmail)
        InsertEventLog(DB, "UPDATE customer_payHdr", paymentID, paymentStatusID, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub UpdateCustAutoPayType(ByVal DB As DBHelper, ByVal customerID As String, ByVal autoPayType As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    SQL = "UPDATE customer SET autoPayType = :1, jetsDataModDate = SYSDATE WHERE customer_id = :2"
    '    DB.ExecuteNonQuery(SQL, autoPayType, customerID)
    '    InsertEventLog(DB, "UPDATE customer", customerID, "autoPayType", autoPayType, False, False)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    'Public Shared Sub UpdateCustAutoPayAcc(ByVal DB As DBHelper, ByVal customerID As String, ByVal autoPayAccNickName As String, Optional isOneTrans As Boolean = True)
    '    Dim SQL As String

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Init
    '    SQL = "UPDATE customer_payAcc SET usedForAutoPay = 'N' WHERE customer_id = :1"
    '    DB.ExecuteNonQuery(SQL, customerID)
    '    InsertEventLog(DB, "UPDATE customer_payAcc", customerID, "usedForAutoPay", "N", False, False)
    '    'Set usedForAutoPay
    '    If autoPayAccNickName <> String.Empty Then
    '        SQL = "UPDATE customer_payAcc SET usedForAutoPay = 'Y' WHERE customer_id = :1 AND accNickName = :2"
    '        DB.ExecuteNonQuery(SQL, customerID, autoPayAccNickName)
    '        InsertEventLog(DB, "UPDATE customer_payAcc", customerID, "usedForAutoPay", autoPayAccNickName, False, False)
    '    End If

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub DeletePayment(ByVal DB As DBHelper, ByVal paymentID As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete payment detail
        SQL = "DELETE FROM customer_paydtl WHERE payment_id = :1"
        DB.ExecuteNonQuery(SQL, paymentID)
        InsertEventLog(DB, "DELETE customer_paydtl", paymentID, Nothing, Nothing, False, False)

        'Delete payment header
        SQL = "DELETE FROM customer_payHdr WHERE payment_id = :1"
        DB.ExecuteNonQuery(SQL, paymentID)
        InsertEventLog(DB, "DELETE customer_payHdr", paymentID, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Update Order
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Function UpSertItemInOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal customerID As String, ByVal custGrpItemID As String, ByVal csQty As Integer, ByVal eaQty As Integer, ByVal lbs As Decimal, _
                                           ByVal csQty_vp As Integer, ByVal eaQty_vp As Integer, ByVal csQty_sp As Integer, ByVal eaQty_sp As Integer, ByVal vpMemo As String, _
                                           Optional ByVal csPrice As Decimal = -1, Optional ByVal eaPrice As Decimal = -1, Optional ByVal lbPrice_OrderByQty As Decimal = -1, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        Dim lineNr, newLineNr As Integer
        Dim userEmail As String = GetUserEmail(DB)
        Dim r As DataReader
        Dim jfcBranchNr As String = ""
        Dim jfcCustomerNr As String = ""
        Dim priceGrpID As String

        'Init
        vpMemo = Left(vpMemo, 40)

        'Get JFC info
        Common.GetJFCBranchAndCustomerNr(DB, customerID, jfcBranchNr, jfcCustomerNr)
        priceGrpID = Common.GetPriceGrpID(DB, jfcBranchNr, jfcCustomerNr)

        If isOneTrans = True Then DB.BeginTransaction()

        'Add item in order if the item doesn't exists; otherwise, update qty of the existing item
        SQL = "SELECT webLineNr FROM sodtl WHERE webOrder_id = :1 AND custGrpItem_id = :2"
        lineNr = Val(DB.GetScalar(SQL, webOrderID, custGrpItemID))
        If lineNr = 0 Then
            'Do nothing if qty are 0
            If csQty = 0 AndAlso eaQty = 0 AndAlso lbs = 0 AndAlso csQty_vp = 0 AndAlso eaQty_vp = 0 AndAlso csQty_sp = 0 AndAlso eaQty_sp = 0 Then Return False
            'Find the next line #
            SQL = "SELECT MAX(webLineNr) OVER () max_webLineNr FROM sodtl WHERE webOrder_id = :1 FOR UPDATE OF webLineNr"
            newLineNr = Val(DB.GetScalar(SQL, webOrderID)) + 1
            'Get item info
            SQL = "SELECT i.item_id, i.itemGrp_id, i.itemNr, i.custItemDesc, i.minSalCnt, i.jfcitem, ci.eaPerCs, " & _
                     " " & IIf(csPrice = -1, "ci.csPrice", csPrice & " AS csPrice") & ", " & _
                     " " & IIf(eaPrice = -1, "ci.eaPrice", eaPrice & " AS eaPrice") & ", ci.lbPrice_OrderByWgt, " & _
                     " " & IIf(lbPrice_OrderByQty = -1, "ci.lbPrice_OrderByQty", lbPrice_OrderByQty & " AS lbPrice_OrderByQty") & ", " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN NVL(p.itemDesc_branch, a.itemDesc) ELSE NULL END AS itemDesc, " & _
                     " CASE WHEN i.jfcitem = 'Y' THEN a.pointPerCs ELSE NULL END pointPerCs, " & _
                     " ROUND(" & IIf(csPrice = -1, "ci.csPrice", csPrice) & "*" & csQty & " + " & IIf(eaPrice = -1, "ci.eaPrice", eaPrice) & "*" & eaQty & " + ci.lbPrice_OrderByWgt*" & lbs & ", 2) lineAmt " & _
                      "FROM " & IIf(IsSmallCustomer(DB, customerID) = True, IIf(IsExcepPriceCustomer(DB, customerID), "v_excepcust_custgrpitem_small", "v_customer_custGrpItem_small") & " ci, v_custgrp_item_small i", "customer_custGrpItem ci, custgrp_item i") & " " & _
                      "LEFT OUTER JOIN jfcItem a ON a.item_id = i.item_id " & _
                      "LEFT OUTER JOIN jets_price p ON i.jfcItem = 'Y' AND p.itemcode = i.item_id AND p.pricegpid = " & DB.SqlEsc(priceGrpID) & " " &
                      "WHERE i.custGrpItem_id = ci.custGrpItem_id " & _
                      "  AND ci.customer_id = :1 AND ci.custGrpItem_id = :2 " & _
                      "  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID))
            '"  AND i.custgrp_id = " & DB.SqlEsc(GetCustGrpID(DB, customerID)) & _ 'is added to improve the performance
            '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.csPrice FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.csPrice END AS csPrice, " & _
            '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.eaPrice FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.eaPrice END AS eaPrice, " & _
            '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.lbPrice_OrderByWgt FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.lbPrice_OrderByWgt END AS lbPrice_OrderByWgt, " & _
            '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT b.lbPrice_OrderByQty FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ci.lbPrice_OrderByQty END AS lbPrice_OrderByQty, " & _
            '" CASE WHEN i.jfcitem = 'Y' THEN (SELECT ROUND(b.csPrice*" & csQty & " + b.eaPrice*" & eaQty & " + b.lbPrice_OrderByWgt*" & lbs & ", 2) FROM jfcPrice b WHERE b.priceGrp_id = ci.priceGrp_id AND b.item_id = i.item_id) ELSE ROUND(ci.csPrice*" & csQty & " + ci.eaPrice*" & eaQty & " + ci.lbPrice_OrderByWgt*" & lbs & ", 2) END AS lineAmt " & _
            r = DB.GetReader(SQL, customerID, custGrpItemID)
            If r.Read() Then
                'Add item
                SQL = "INSERT INTO sodtl (webOrder_id, webLineNr, custGrpItem_id, " & _
                         " item_id, itemGrp_id, itemDesc, itemNr, custItemDesc, " & _
                         " csQty, eaQty, lbs, csQty_vp, eaQty_vp, csQty_sp, eaQty_sp, vpMemo, " & _
                         " csPrice, eaPrice, lbPrice_orderByWgt, lbPrice_orderByQty, " & _
                         " eaPerCs, minSalCnt, pointPerCs, lineAmt, linePoint) " & _
                         "VALUES (:1,:2,:3,:4,:5,:6,:7,:8" & _
                         "  ," & csQty & ", " & eaQty & ", " & lbs & ", " & csQty_vp & ", " & eaQty_vp & ", " & csQty_sp & ", " & eaQty_sp & ", :9" & _
                         "  ," & Val(r("csPrice")) & ", " & Val(r("eaPrice")) & ", " & Val(r("lbPrice_orderByWgt")) & ", " & Val(r("lbPrice_orderByQty")) & _
                         "  ," & Val(r("eaPerCs")) & ", " & Val(r("minSalCnt")) & ", 0, " & Val(r("lineAmt")) & ", 0)"
                DB.ExecuteNonQuery(SQL, webOrderID, newLineNr, custGrpItemID, CString(r("item_id")), CString(r("itemGrp_id")), CString(r("itemDesc")), CString(r("itemNr")), CString(r("custItemDesc")), vpMemo)
                InsertEventLog(DB, "INSERT INTO sohdr", webOrderID, newLineNr, Nothing, False, False)
                UpSertLastOrderedQty(DB, customerID, custGrpItemID, csQty, eaQty, lbs, False)
            End If
            r.close()
        Else
            If csQty = 0 AndAlso eaQty = 0 AndAlso lbs = 0 AndAlso csQty_vp = 0 AndAlso eaQty_vp = 0 AndAlso csQty_sp = 0 AndAlso eaQty_sp = 0 Then
                'Delete item
                SQL = "DELETE FROM sodtl WHERE webOrder_id = :1 AND webLineNr = " & lineNr
                DB.ExecuteNonQuery(SQL, webOrderID)
                InsertEventLog(DB, "DELETE FROM sodtl", webOrderID, lineNr, Nothing, False, False)
            Else
                'Update item qty
                SQL = "UPDATE sodtl SET csPrice = " & IIf(csPrice = -1, "csPrice", csPrice) & ", eaPrice = " & IIf(eaPrice = -1, "eaPrice", eaPrice) & ", lbPrice_orderByQty = " & IIf(lbPrice_OrderByQty = -1, "lbPrice_orderByQty", lbPrice_OrderByQty) & _
                        ", csQty = " & csQty & ", eaQty = " & eaQty & ", lbs = " & lbs & ", csQty_vp = " & csQty_vp & ", eaQty_vp = " & eaQty_vp & ", csQty_sp = " & csQty_sp & ", eaQty_sp = " & eaQty_sp & ", vpMemo = :1 " & _
                        ", lineAmt = ROUND(" & IIf(csPrice = -1, "csPrice", csPrice) & "*" & csQty & " + " & IIf(eaPrice = -1, "eaPrice", eaPrice) & "*" & eaQty & " + lbPrice_OrderByWgt*" & lbs & ", 2) " & _
                        " WHERE weborder_id = :2 AND webLineNr = " & lineNr
                DB.ExecuteNonQuery(SQL, vpMemo, webOrderID)
                InsertEventLog(DB, "UPDATE sodtl", webOrderID, lineNr, Nothing, False, False)
                UpSertLastOrderedQty(DB, customerID, custGrpItemID, csQty, eaQty, lbs, False)
            End If
        End If

        'Update modfied user and date
        SQL = "UPDATE sohdr SET modUser = :1, modDate = SYSDATE WHERE weborder_id = :2"
        DB.ExecuteNonQuery(SQL, userEmail, webOrderID)

        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Sub DeleteItemInOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal custGrpItemID As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete item from order (not deleting sohdr even if sodtl is empty)
        SQL = "DELETE FROM sodtl WHERE webOrder_id = :1 AND custGrpItem_id = :2"
        DB.ExecuteNonQuery(SQL, webOrderID, custGrpItemID)
        InsertEventLog(DB, "DELETE FROM sodtl", webOrderID, custGrpItemID, Nothing, False, False)

        'Update modfied user and date
        SQL = "UPDATE sohdr SET modUser = :1, modDate = SYSDATE WHERE weborder_id = :2"
        DB.ExecuteNonQuery(SQL, userEmail, webOrderID)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    'Public Shared Sub DeleteAllItemsInOrder(ByVal DB As DBHelper, ByVal webOrderID As String, Optional ByVal isOneTrans As Boolean = True)
    '    Dim SQL As String
    '    Dim userEmail As String = GetUserEmail(DB)

    '    If isOneTrans = True Then DB.BeginTransaction()

    '    'Delete item from order
    '    SQL = "DELETE FROM sodtl WHERE webOrder_id = :1"
    '    DB.ExecuteNonQuery(SQL, webOrderID)
    '    InsertEventLog(DB, "DELETE FROM sodtl", webOrderID, Nothing, Nothing, False, False)

    '    'Update modfied user and date
    '    SQL = "UPDATE sohdr SET modUser = :1, modDate = SYSDATE WHERE weborder_id = :2"
    '    DB.ExecuteNonQuery(SQL, userEmail, webOrderID)

    '    If isOneTrans = True Then DB.CommitTransaction()
    'End Sub

    Public Shared Sub UpdateOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal deliveryDate As String, ByVal custOrderNr As String, ByVal message As String, ByVal vendorNote As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim r As DataReader
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Update existing order
        SQL = "SELECT SUM(lineAmt) AS totalAmt, SUM(linePoint) AS totalPoint FROM sodtl WHERE webOrder_id = :1"
        r = DB.GetReader(SQL, webOrderID)
        If r.Read() Then
            SQL = "UPDATE sohdr SET deliveryDate = TO_DATE(:1, :2), custOrderNr = :3, message = :4, vendorNote = :5, totalAmt = " & Val(r("totalAmt")) & ", totalPoint = " & Val(r("totalPoint")) & ", modUser = :6, modDate = SYSDATE " & _
                     "WHERE webOrder_id = :7"
            DB.ExecuteNonQuery(SQL, deliveryDate, DATE_FMT, custOrderNr, message, vendorNote, userEmail, webOrderID)
            InsertEventLog(DB, "UPDATE sohdr", webOrderID, Nothing, Nothing, False, False)
        End If
        r.close()

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdateOrderState(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal orderState As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Mark shopping cart as ordered
        'SQL = "UPDATE sohdr SET orderState = :1" & IIf(orderState = "U" OrElse orderState = "D", ", revision = revision + 1", "") & ", modUser = :2, modDate = SYSDATE WHERE webOrder_id = :3"
        SQL = "UPDATE sohdr SET orderState = :1" &
                IIf(orderState = "U" OrElse orderState = "D", ", revision = revision + 1", "") &
                IIf(orderState = "O", ", approveUser = " & DB.SqlEsc(userEmail) & ", approveDate = SYSDATE", "") &
                ", modUser = :2, modDate = SYSDATE " &
                "WHERE webOrder_id = :3"
        DB.ExecuteNonQuery(SQL, orderState, userEmail, webOrderID)
        InsertEventLog(DB, "UPDATE sohdr", webOrderID, orderState, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpdateOrderTotal(ByVal DB As DBHelper, ByVal webOrderID As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim r As DataReader
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        'Update existing order
        SQL = "SELECT SUM(lineAmt) AS totalAmt, SUM(linePoint) AS totalPoint FROM sodtl WHERE webOrder_id = :1"
        r = DB.GetReader(SQL, webOrderID)
        If r.Read() Then
            SQL = "UPDATE sohdr SET totalAmt = " & Val(r("totalAmt")) & ", totalPoint = " & Val(r("totalPoint")) & ", modUser = :1, modDate = SYSDATE " & _
                     "WHERE webOrder_id = :2"
            DB.ExecuteNonQuery(SQL, userEmail, webOrderID)
            InsertEventLog(DB, "UPDATE sohdr", webOrderID, Nothing, Nothing, False, False)
        End If
        r.close()

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub DeleteOrder(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal deleteFromDB As Boolean, Optional ByVal isOneTrans As Boolean = True)
        'Dim SQL As String
        Dim userEmail As String = GetUserEmail(DB)

        If isOneTrans = True Then DB.BeginTransaction()

        If deleteFromDB = True Then
            'Shouldn't enter here
            ''Delete order from DB
            'SQL = "DELETE sohdr WHERE webOrder_id = :1"
            'DB.ExecuteNonQuery(SQL, webOrderID)
            'InsertEventLog(DB, "DELETE FROM sohdr", webOrderID, Nothing, Nothing, False, False)
        Else
            'Mark order as deleted
            UpdateOrderState(DB, webOrderID, "D", False)
        End If

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub InsertOrderHistory(ByVal DB As DBHelper, ByVal webOrderID As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        'Delete the same revision if exists (Before sales rep approves the order, revision stays the same)
        SQL = "DELETE FROM sohdr_hist WHERE webOrder_id = :1 AND revision IN (SELECT revision FROM sohdr WHERE webOrder_id = :2)"
        DB.ExecuteNonQuery(SQL, webOrderID, webOrderID)
        InsertEventLog(DB, "DELETE FROM sohdr_hist", webOrderID, Nothing, Nothing, False, False)

        SQL = "DELETE FROM sodtl_hist WHERE webOrder_id = :1 AND revision IN (SELECT revision FROM sohdr WHERE webOrder_id = :2)"
        DB.ExecuteNonQuery(SQL, webOrderID, webOrderID)
        InsertEventLog(DB, "DELETE FROM sodtl_hist", webOrderID, Nothing, Nothing, False, False)

        'Copy order header to the history
        SQL = "INSERT INTO sohdr_hist (webOrder_id, revision, orderState, customer_id, custGrpVendor_id, shipPoint_id," & _
                " custOrderNr, message, vendorNote, totalAmt, totalPoint, orderDate, deliveryDate, cart_id, syncedRevision, syncedRevision_jfcSubBranch, syncDate, createUser, createDate) " & _
                "SELECT webOrder_id, revision, orderState, customer_id, custGrpVendor_id, shipPoint_id," & _
                " custOrderNr, message, vendorNote, totalAmt, totalPoint, orderDate, deliveryDate, cart_id, syncedRevision, syncedRevision_jfcSubBranch, syncDate, createUser, createDate " & _
                "FROM sohdr WHERE webOrder_id = :1"
        DB.ExecuteNonQuery(SQL, webOrderID)
        InsertEventLog(DB, "INSERT INTO sohdr_hist", webOrderID, Nothing, Nothing, False, False)

        'Copy order detail to the history
        SQL = "INSERT INTO sodtl_hist (webOrder_id, webLineNr, revision, custGrpItem_id, item_id, itemGrp_id," & _
                " itemDesc, itemNr, custItemDesc, csQty, eaQty, lbs," & _
                " csQty_vp, eaQty_vp, csQty_sp, eaQty_sp, vpMemo, " & _
                " csPrice, eaPrice, lbPrice_OrderByWgt, lbPrice_OrderByQty, eaPerCs, minSalCnt, pointPerCs, lineAmt, linePoint) " & _
                "SELECT d.webOrder_id, d.webLineNr, h.revision, d.custGrpItem_id, d.item_id, d.itemGrp_id," & _
                " d.itemDesc, d.itemNr, d.custItemDesc, d.csQty, d.eaQty, d.lbs," & _
                " d.csQty_vp, d.eaQty_vp, d.csQty_sp, d.eaQty_sp, d.vpMemo," & _
                " d.csPrice, d.eaPrice, d.lbPrice_OrderByWgt, d.lbPrice_OrderByQty, d.eaPerCs, d.minSalCnt, d.pointPerCs, d.lineAmt, d.linePoint " & _
                "FROM sodtl d, sohdr h WHERE d.webOrder_id = h.webOrder_id AND h.webOrder_id = :1"
        DB.ExecuteNonQuery(SQL, webOrderID)
        InsertEventLog(DB, "INSERT INTO sodtl_hist", webOrderID, Nothing, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpSertOrderEmail(ByVal DB As DBHelper, ByVal webOrderID As String, ByVal emailType As String, ByVal state As String, ByVal modUser As String, _
                                        Optional ByVal email As String = "", Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim onetimePasswd As String

        If isOneTrans = True Then DB.BeginTransaction()

        'State: N (not sent), S (sent), R (received by vendor)
        Select Case state
            Case "N"
                'Create email queue
                onetimePasswd = IIf(emailType = "VENDOR", Common.CreateSalt(16), "")
                SQL = "INSERT INTO so_email (webOrder_id, emailType, state, email, sentCount, onetimePassword, modUser, modDate) " & _
                        "VALUES (:1,:2,:3,:4,0,:5,:6,SYSDATE)"
                DB.ExecuteNonQuery(SQL, webOrderID, emailType, state, email, onetimePasswd, modUser)
                InsertEventLog(DB, "INSERT INTO so_email", webOrderID, emailType, state, False, False)
            Case "S"
                'Update email state to be sent
                SQL = "UPDATE so_email SET state = :1, sentCount = sentCount + 1, modUser = :2, modDate = SYSDATE WHERE webOrder_id = :3 AND emailType = :4"
                DB.ExecuteNonQuery(SQL, state, modUser, webOrderID, emailType)
                InsertEventLog(DB, "UPDATE so_email", webOrderID, emailType, state, False, False)
            Case "R"
                'Update email state to be received by vendor
                SQL = "UPDATE so_email SET state = :1, modUser = :2, modDate = SYSDATE WHERE webOrder_id = :3 AND emailType = :4"
                DB.ExecuteNonQuery(SQL, state, modUser, webOrderID, emailType)
                InsertEventLog(DB, "UPDATE so_email", webOrderID, emailType, state, False, False)
            Case "RESEND"
                'Increase sent count
                SQL = "UPDATE so_email SET sentCount = sentCount + 1 WHERE webOrder_id = :1 AND emailType = :2"
                DB.ExecuteNonQuery(SQL, webOrderID, emailType)
                'Update email state to be sent if it hasn't been sent
                SQL = "UPDATE so_email SET state = 'S', modUser = :1, modDate = SYSDATE WHERE webOrder_id = :2 AND emailType = :3 AND state = 'N'"
                DB.ExecuteNonQuery(SQL, modUser, webOrderID, emailType)
                InsertEventLog(DB, "UPDATE so_email", webOrderID, emailType, "S", False, False)
        End Select

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub UpSertLastOrderedQty(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, ByVal csQty As Integer, ByVal eaQty As Integer, ByVal lbs As Decimal, _
                                           Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        Dim email As String = GetUserEmail(DB)

        'Do nothing for JFC items. Use synced qty from JETS instead for JFC items
        If IsJfcItem(DB, custGrpItemID) = True Then Exit Sub

        If isOneTrans = True Then DB.BeginTransaction()

        'For non JFC items, add qty if the item doesn't exists; otherwise, update qty of the existing item
        SQL = "SELECT custGrpItem_id FROM customer_lastOrderedQty WHERE customer_id = :1 AND custGrpItem_id = :2"
        If CString(DB.GetScalar(SQL, customerID, custGrpItemID)) = String.Empty Then
            SQL = "INSERT INTO customer_lastOrderedQty (customer_id, custGrpItem_id, csQty, eaQty, pcQty, lbs, shipDate, modUser, modDate) " & _
                    "VALUES (:1,:2," & csQty & "," & eaQty & ",NULL," & lbs & ",SYSDATE,:3,SYSDATE)"
            DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, email)
            InsertEventLog(DB, "INSERT INTO customer_lastOrderedQty", customerID, custGrpItemID, Nothing, False, False)
        Else
            SQL = "UPDATE customer_lastOrderedQty SET csQty = " & csQty & ", eaQty = " & eaQty & ", lbs = " & lbs & ", shipDate = SYSDATE, modUser = :1, modDate = SYSDATE " & _
                    "WHERE customer_id = :2 AND custGrpItem_id = :3"
            DB.ExecuteNonQuery(SQL, email, customerID, custGrpItemID)
            InsertEventLog(DB, "UPDATE customer_lastOrderedQty", customerID, custGrpItemID, Nothing, False, False)
        End If

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Insert DoNotText
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub InsertDoNoText(ByVal DB As DBHelper, ByVal cellular As String, ByVal reason As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        SQL = "SELECT cellular FROM DoNotText WHERE cellular = :1"
        If CString(DB.GetScalar(SQL, cellular)) <> String.Empty Then Exit Sub

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "INSERT INTO doNoText (cellular, reason, createDate) VALUES (:1,:2,SYSDATE)"
        DB.ExecuteNonQuery(SQL, cellular, reason)
        InsertEventLog(DB, "INSERT INTO doNoText", cellular, reason, Nothing, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Insert signup request
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub InsertSignupRequest(ByVal DB As DBHelper, ByVal jfcBranchNr As String, ByVal jfcCustomerNr As String, ByVal customerName As String, ByVal contactName As String, ByVal email As String, ByVal phone As String, ByVal registrationCode As String, ByVal salesmanEmail As String, ByVal jfcVendorEmail As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "INSERT INTO signupRequest (jfcBranchNr, jfcCustomerNr, status, customerName, contactName, email, phone, registrationCode, salesmanEmail, jfcVendorEmail, createDate,CREATEUSER) " & _
                            "VALUES (:1,:2,'N',:3,:4,:5,:6,:7,:8,:9,SYSDATE,:10)"
        DB.ExecuteNonQuery(SQL, jfcBranchNr, jfcCustomerNr, customerName, contactName, email, phone, registrationCode, salesmanEmail, jfcVendorEmail, GetUserEmail(DB))
        InsertEventLog(DB, "INSERT INTO signupRequest", jfcBranchNr & "-" & jfcCustomerNr, email, salesmanEmail, False, False)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '--------------------------------------------------------------------------------------------------------------------
    'Insert Eventlog
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub InsertEventLog(ByVal DB As DBHelper, ByVal eventDesc As String, ByVal eventArg1 As String, ByVal eventArg2 As String, ByVal eventArg3 As String, ByVal allowView As Boolean, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String
        'Dim logID As String

        If isOneTrans = True Then DB.BeginTransaction()

        'logID = DB.GetSequence("LOG_ID", LOG_ID_LEN)
        SQL = "INSERT INTO eventlog (log_id, eventDesc, eventArg1, eventArg2, eventArg3 " &
                    ", userType, custCompany_id, vendorCompany_id" &
                    ", customer_id, allowView, logTerminal, createUser, createdate) " &
                    "VALUES (log_seq.NEXTVAL,:2,:3,:4,:5,:6,:7,:8,:9," & IIf(allowView = True, "'Y'", "'N'") & ",:10,:11,SYSDATE)"
        'TODO: Insert IP address (The code below will create error in local host)
        '"," & DB.SqlEsc(HttpContext.Current.Session("SYS_SESSION_CUSTID")) & "," & IIf(allowView = True, "'Y'", "'N'") & "," & DB.SqlEsc(HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")) & "," & DB.SqlEsc(GetUserEmail(DB)) & ",SYSDATE)"
        DB.ExecuteNonQuery(SQL, Left(eventDesc, EVENT_DESC_ARG_LEN), Left(eventArg1, EVENT_DESC_ARG_LEN), Left(eventArg2, EVENT_DESC_ARG_LEN), Left(eventArg3, EVENT_DESC_ARG_LEN),
                           CString(HttpContext.Current.Session("SYS_SESSION_USERTYPE")), CString(HttpContext.Current.Session("SYS_SESSION_CUSTCOMPANYID")), CString(HttpContext.Current.Session("SYS_SESSION_VENDORCOMPANYID")),
                           CString(HttpContext.Current.Session("SYS_SESSION_CUSTID")), CString(HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")), GetUserEmail(DB))

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    '---------------------------
    'Insert YI FU 092020
    '---------------------------
    Public Shared Function UpSertCustItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, ByVal Enabledvalue As String, ByVal CustItemCode As String,
                                          ByVal CUSTITEMDESC As String, ByVal CUSTREMARK As String, ByVal CUSTCSPRICE As Decimal, ByVal CUSTEAPRICE As Decimal, ByVal CUSTLBPRICE_ORDERBYWGT As Decimal,
                                           ByVal lbPrice_OrderByQty As Decimal, ByVal IsNew As Boolean, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        'That is for updating the Customer item Price
        SQL = "Select ITEM_ID from jfcCustomerPrice where CUSTOMER_ID = :1 and ITEM_ID =:2"
        Dim ITEM_ID = DB.GetScalar(SQL, customerID, custGrpItemID.Substring(custGrpItemID.Length - 5, 5))
        If ITEM_ID = "" And IsNew And (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
            SQL = "INSERT INTO jfcCustomerPrice(CUSTOMER_ID,ITEM_ID,ENABLED,CSPRICE,EAPRICE,LBPRICE_ORDERBYWGT,LBPRICE_ORDERBYQTY)" & _
           "VALUES ({0},{1},{2},{3},{4},{5},{6})"
            DB.ExecuteNonQuery(String.Format(SQL, DB.SqlEsc(customerID), DB.SqlEsc(custGrpItemID.Substring(custGrpItemID.Length - 5, 5)), DB.SqlEsc(Enabledvalue), CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty))
            InsertEventLog(DB, "Insert jfcCustomerPrice", customerID, custGrpItemID, Nothing, False, False)
        ElseIf (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
            If CUSTCSPRICE <> 0 Or CUSTEAPRICE <> 0 Or CUSTLBPRICE_ORDERBYWGT <> 0 Then
                SQL = "update jfcCustomerPrice set CSPRICE = {0}, EAPRICE = {1}, LBPRICE_ORDERBYWGT = {2}, LBPRICE_ORDERBYQTY = {3} " & _
                      "where  customer_id = {4} and item_id = {5} "

                DB.ExecuteNonQuery(String.Format(SQL, CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty, DB.SqlEsc(customerID), DB.SqlEsc(custGrpItemID.Substring(custGrpItemID.Length - 5, 5))))
                InsertEventLog(DB, "Update jfcCustomerPrice", customerID, DB.SqlEsc(custGrpItemID), Nothing, False, False)
            End If
        End If


        'So We know this item don't have any customer setting.
        SQL = "Select CUSTGRPITEM_ID from customer_custitem where CUSTOMER_ID = :1 and CUSTGRPITEM_ID =:2"
        Dim _custGrpItemId = DB.GetScalar(SQL, customerID, custGrpItemID)
        If _custGrpItemId = "" And Not IsAllNull(CustItemCode, CUSTITEMDESC, CUSTREMARK, CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty) Then
            If (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
                SQL = "INSERT INTO customer_custitem (CUSTOMER_ID, CUSTGRPITEM_ID, ENABLED, " & _
                " CUSTITEMCODE, CUSTITEMDESC, CUSTREMARK, CUSTCSPRICE, CUSTEAPRICE, " & _
                " CUSTLBPRICE_ORDERBYWGT,CUSTLBPRICE_ORDERBYQTY,ITEM_ID,IsNew,MODUSER,MODDATE) " & _
                "VALUES (:1,:2,:3,:4,:5,:6," & CUSTCSPRICE & "," & CUSTEAPRICE & "," & CUSTLBPRICE_ORDERBYWGT & "," & lbPrice_OrderByQty & ",:7,:8,:9,SYSDATE)"
                DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, Enabledvalue, CustItemCode, CUSTITEMDESC, CUSTREMARK, custGrpItemID.Substring(custGrpItemID.Length - 5, 5), IIf(IsNew, "Y", ""), GetUserEmail(DB))

            Else
                SQL = "INSERT INTO customer_custitem (CUSTOMER_ID, CUSTGRPITEM_ID, ENABLED, " & _
                   " CUSTITEMCODE, CUSTITEMDESC, CUSTREMARK,ITEM_ID,IsNew,MODUSER,MODDATE) " & _
                   "VALUES (:1,:2,:3,:4,:5,:6,:7,:8,:9,SYSDATE)"  '" & DB.SqlEsc(CUSTITEMDESC) & "
                DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, Enabledvalue, CustItemCode, CUSTITEMDESC, CUSTREMARK, custGrpItemID.Substring(custGrpItemID.Length - 5, 5), IIf(IsNew, "Y", ""), GetUserEmail(DB))


            End If
            InsertEventLog(DB, "Insert customer_custitem", customerID, custGrpItemID, Nothing, False, False)


        ElseIf IsAllNull(CustItemCode, CUSTITEMDESC, CUSTREMARK, CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty) Then
            SQL = "delete from customer_custitem WHERE CUSTOMER_ID = :1 AND CUSTGRPITEM_ID = :2 "
            DB.ExecuteNonQuery(SQL, customerID, custGrpItemID)

            InsertEventLog(DB, "Delete customer_custitem", customerID, custGrpItemID, Nothing, False, False)
        ElseIf (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
            SQL = "UPDATE customer_custitem SET CUSTITEMCODE = :1, CUSTITEMDESC = :2 ,CUSTREMARK = :3, CUSTCSPRICE = " & CUSTCSPRICE & _
                ",CUSTEAPRICE = " & CUSTEAPRICE & ", CUSTLBPRICE_ORDERBYWGT= " & CUSTLBPRICE_ORDERBYWGT & ", CUSTLBPRICE_ORDERBYQTY = " & lbPrice_OrderByQty & ", " & _
                "ENABLED = :4, MODUSER = :5, MODDATE = SYSDATE WHERE CUSTOMER_ID = :6 AND CUSTGRPITEM_ID = :7 "
            DB.ExecuteNonQuery(SQL, CustItemCode, CUSTITEMDESC, CUSTREMARK, Enabledvalue, GetUserEmail(DB), customerID, custGrpItemID)
            InsertEventLog(DB, "UPDATE customer_custitem", customerID, custGrpItemID, Nothing, False, False)
        Else
            SQL = "UPDATE customer_custitem SET CUSTITEMCODE = :1, CUSTITEMDESC = :2 ,CUSTREMARK = :3, ENABLED = :4, MODUSER = :5, MODDATE = SYSDATE WHERE CUSTOMER_ID = :6 AND CUSTGRPITEM_ID = :7 "
            DB.ExecuteNonQuery(SQL, CustItemCode, CUSTITEMDESC, CUSTREMARK, Enabledvalue, GetUserEmail(DB), customerID, custGrpItemID)
            InsertEventLog(DB, "UPDATE customer_custitem", customerID, custGrpItemID, Nothing, False, False)

        End If



        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpSertCustItemCopyPrice(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, ByVal Enabledvalue As String,
                                                   ByVal CustItemCode As String, ByVal CUSTITEMDESC As String, ByVal CUSTREMARK As String,
                                                   ByVal CUSTCSPRICE As Decimal, ByVal CUSTEAPRICE As Decimal, ByVal CUSTLBPRICE_ORDERBYWGT As Decimal,
                                           ByVal lbPrice_OrderByQty As Decimal, ByVal IsNew As Boolean, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        'That is for updating the Customer item Price
        SQL = "Select ITEM_ID from jfcCustomerPrice where CUSTOMER_ID = :1 and ITEM_ID =:2"
        Dim ITEM_ID = DB.GetScalar(SQL, customerID, custGrpItemID.Substring(custGrpItemID.Length - 5, 5))
        If ITEM_ID = "" And IsNew And (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
            SQL = "INSERT INTO jfcCustomerPrice(CUSTOMER_ID,ITEM_ID,ENABLED,CSPRICE,EAPRICE,LBPRICE_ORDERBYWGT,LBPRICE_ORDERBYQTY)" & _
           "VALUES ({0},{1},{2},{3},{4},{5},{6})"
            DB.ExecuteNonQuery(String.Format(SQL, DB.SqlEsc(customerID), DB.SqlEsc(custGrpItemID.Substring(custGrpItemID.Length - 5, 5)), DB.SqlEsc(Enabledvalue), CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty))
            InsertEventLog(DB, "Insert jfcCustomerPrice", customerID, custGrpItemID, Nothing, False, False)
        ElseIf (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
            If CUSTCSPRICE <> 0 Or CUSTEAPRICE <> 0 Or CUSTLBPRICE_ORDERBYWGT <> 0 Then
                SQL = "update jfcCustomerPrice set CSPRICE = {0}, EAPRICE = {1}, LBPRICE_ORDERBYWGT = {2}, LBPRICE_ORDERBYQTY = {3} " & _
                      "where  customer_id = {4} and item_id = {5} "

                DB.ExecuteNonQuery(String.Format(SQL, CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty, DB.SqlEsc(customerID), DB.SqlEsc(custGrpItemID.Substring(custGrpItemID.Length - 5, 5))))
                InsertEventLog(DB, "Update jfcCustomerPrice", customerID, DB.SqlEsc(custGrpItemID), Nothing, False, False)
            End If
        End If


        'So We know this item don't have any customer setting.
        SQL = "Select CUSTGRPITEM_ID from customer_custitem where CUSTOMER_ID = :1 and CUSTGRPITEM_ID =:2"
        Dim _custGrpItemId = DB.GetScalar(SQL, customerID, custGrpItemID)
        If _custGrpItemId = "" And Not IsAllNull(CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty) Then
            If (CUSTCSPRICE > 0 OrElse lbPrice_OrderByQty > 0 OrElse CUSTEAPRICE > 0) Then
                SQL = "INSERT INTO customer_custitem (CUSTOMER_ID, CUSTGRPITEM_ID, ENABLED, " & _
                "  CUSTCSPRICE, CUSTEAPRICE, " & _
                " CUSTLBPRICE_ORDERBYWGT,CUSTLBPRICE_ORDERBYQTY,ITEM_ID,IsNew,MODUSER,MODDATE) " & _
                "VALUES (:1,:2,:3," & CUSTCSPRICE & "," & CUSTEAPRICE & "," & CUSTLBPRICE_ORDERBYWGT & "," & lbPrice_OrderByQty & ",:4,:5,:6,SYSDATE)"
                DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, Enabledvalue, custGrpItemID.Substring(custGrpItemID.Length - 5, 5), IIf(IsNew, "Y", ""), GetUserEmail(DB))

            Else
                SQL = "INSERT INTO customer_custitem (CUSTOMER_ID, CUSTGRPITEM_ID, ENABLED, " & _
                   "ITEM_ID,IsNew,MODUSER,MODDATE) " & _
                   "VALUES (:1,:2,:3,:4,:5,:6,SYSDATE)"  '" & DB.SqlEsc(CUSTITEMDESC) & "
                DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, Enabledvalue, custGrpItemID.Substring(custGrpItemID.Length - 5, 5), IIf(IsNew, "Y", ""), GetUserEmail(DB))


            End If
            InsertEventLog(DB, "Insert customer_custitem", customerID, custGrpItemID, Nothing, False, False)


        ElseIf IsAllNull(CustItemCode, CUSTITEMDESC, CUSTREMARK, CUSTCSPRICE, CUSTEAPRICE, CUSTLBPRICE_ORDERBYWGT, lbPrice_OrderByQty) Then
            SQL = "delete from customer_custitem WHERE CUSTOMER_ID = :1 AND CUSTGRPITEM_ID = :2 "
            DB.ExecuteNonQuery(SQL, customerID, custGrpItemID)

            InsertEventLog(DB, "Delete customer_custitem", customerID, custGrpItemID, Nothing, False, False)
        Else
            SQL = "UPDATE customer_custitem SET  CUSTCSPRICE = " & CUSTCSPRICE & _
                ",CUSTEAPRICE = " & CUSTEAPRICE & ", CUSTLBPRICE_ORDERBYWGT= " & CUSTLBPRICE_ORDERBYWGT & ", CUSTLBPRICE_ORDERBYQTY = " & lbPrice_OrderByQty & ", " & _
                "ENABLED = :1, MODUSER = :2, MODDATE = SYSDATE WHERE CUSTOMER_ID = :3 AND CUSTGRPITEM_ID = :4 "
            DB.ExecuteNonQuery(SQL, Enabledvalue, GetUserEmail(DB), customerID, custGrpItemID)
            InsertEventLog(DB, "UPDATE customer_custitem", customerID, custGrpItemID, Nothing, False, False)
        End If

        If isOneTrans = True Then DB.CommitTransaction()

        Return True
    End Function

    Public Shared Function deleteCustCustItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "delete from customer_custitem WHERE CUSTOMER_ID = :1 AND CUSTGRPITEM_ID = :2 "
        DB.ExecuteNonQuery(SQL, customerID, custGrpItemID)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function DeleteCustomerCustItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "Select CUSTGRPITEM_ID from customer_custitem where CUSTOMER_ID = :1 and CUSTGRPITEM_ID =:2"
        Dim _custGrpItemId = DB.GetScalar(SQL, customerID, custGrpItemID)
        If _custGrpItemId = "" Then
            SQL = "INSERT INTO customer_custitem (CUSTOMER_ID, CUSTGRPITEM_ID, ENABLED, " & _
                         " CUSTITEMCODE, CUSTITEMDESC, CUSTREMARK, CUSTCSPRICE, CUSTEAPRICE, " & _
                         " CUSTLBPRICE_ORDERBYWGT,CUSTLBPRICE_ORDERBYQTY,ITEM_ID,MODUSER,MODDATE) " & _
                         "VALUES (:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,SYSDATE)"
            DB.ExecuteNonQuery(SQL, customerID, custGrpItemID, "N", "", "", "", "", "", "", "", custGrpItemID.Substring(custGrpItemID.Length - 5, 5), GetUserEmail(DB))
        Else
            SQL = "UPDATE customer_custitem SET CUSTITEMCODE = '', CUSTITEMDESC = '',CUSTREMARK = '', CUSTCSPRICE = '',CUSTEAPRICE = '', CUSTLBPRICE_ORDERBYWGT= '', CUSTLBPRICE_ORDERBYQTY = '',ENABLED = 'N', MODUSER = :1, MODDATE = SYSDATE WHERE CUSTOMER_ID = :2 AND CUSTGRPITEM_ID = :3 "
            DB.ExecuteNonQuery(SQL, GetUserEmail(DB), customerID, custGrpItemID)
        End If

        InsertEventLog(DB, "Delete customer_custitem", customerID, custGrpItemID, Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpdateCustSetting(ByVal db As DBHelper, ByVal customerID As String, ByVal AllowCustOrder As String, HideItemPage As String, HidePaymentPage As String, HidePrice As String, anytimedelivery As String, TextPhones As String, ShowOutofStock As String, ShowAllExcepGrpItem As String, shipvia As String, shipdateoffset As Decimal, minOrderAmt As Integer, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        If isOneTrans Then db.BeginTransaction()
        SQL = "Update customer SET allowCustOrder = :1, hideItemPage =:2,hidePaymentPage =:3, hidePrice = :4, anytimedelivery = :5, textphones = :6, SHOWOUTOFSTOCK = :7,ShowAllExcepGrpItem = :8, SHIPVIA = :9, ShipDateOffset = " & shipdateoffset & ", minOrderAmt = " & minOrderAmt & " where customer_id = :11"
        db.ExecuteNonQuery(SQL, AllowCustOrder, HideItemPage, HidePaymentPage, HidePrice, anytimedelivery, TextPhones, ShowOutofStock, ShowAllExcepGrpItem, shipvia, customerID)
        If AllowCustOrder = "Y" Then
            SQL = "Update customer SET AllowOrderCreateDate = sysdate where customer_id = :1 and AllowOrderCreateDate is null"
            db.ExecuteNonQuery(SQL, customerID)
        End If
        InsertEventLog(db, "Update customer", customerID, Nothing, Nothing, False, False)
        If isOneTrans Then db.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpdateUserInList(ByVal db As DBHelper, ByVal email As String, ByVal customerID As String, ByVal txtPhone As String, firstName As String, lastName As String, ByVal hideCustOrder As String, hideItemPage As String, hidePaymentPage As String, hidePrice As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        Dim needUpdate As Boolean = False
        If isOneTrans Then db.BeginTransaction()

        'Update webuser
        SQL = "Update webuser SET cellular =:1 ,FIRSTNAME=:2,LASTNAME=:3 where email =:4"
        db.ExecuteNonQuery(SQL, txtPhone, firstName, lastName, email)
        InsertEventLog(db, "Update Webuser", email, Nothing, Nothing, False, False)

        'Update webuser_customer
        SQL = "Update webuser_customer SET "
        If hideCustOrder IsNot Nothing Then
            SQL &= " hideCustOrder = " & db.SqlEsc(hideCustOrder) & ","
            needUpdate = True
        End If
        If hideItemPage IsNot Nothing Then
            SQL &= " hideItemPage = " & db.SqlEsc(hideItemPage) & ","
            needUpdate = True
        End If
        If hidePaymentPage IsNot Nothing Then
            SQL &= " hidePaymentPage = " & db.SqlEsc(hidePaymentPage) & ","
            needUpdate = True
        End If
        If hidePrice IsNot Nothing Then
            SQL &= " hidePrice = " & db.SqlEsc(hidePrice) & ","
            needUpdate = True
        End If
        If needUpdate = True Then
            SQL = SQL.Substring(0, SQL.Length - 1)
            SQL &= " where email =:1 AND customer_id = :2"
            'Sql = "Update webuser SET hideCustOrder =:1, hideItemPage = :2, hidePaymentPage = :3, hidePrice = :4 where email =:5"
            db.ExecuteNonQuery(SQL, email, customerID)
            InsertEventLog(db, "Update Webuser_customer", email, customerID, Nothing, False, False)
        End If

        If isOneTrans Then db.CommitTransaction()
        Return True
    End Function

    '--------------------------------------------------------------------------------------------------------------------
    'Insert Or Update OTP YI FU 10/30/2020
    '--------------------------------------------------------------------------------------------------------------------
    Public Shared Sub UpSertWebUserOTP(ByVal db As DBHelper, ByVal email As String, ByVal deviceCode As String, ipaddress As String, ByVal otp As String, notiftype As String, Optional isOneTrans As Boolean = True)
        Dim SQL As String
        If isOneTrans = True Then db.BeginTransaction()
        If email <> String.Empty And otp <> String.Empty Then
            'Delete the current param if exists
            SQL = "DELETE FROM webuser_otp " &
                "WHERE email = " & db.SqlEsc(email) & " AND deviceCode = " & db.SqlEsc(deviceCode)
            db.ExecuteNonQuery(SQL)

            SQL = "INSERT INTO webuser_otp (email,ENABLED ,OTP, NOTIFTYPE,deviceCode,IPADDRESS,CREATEDATE,EXPIREDATE) " &
                    "VALUES (" & db.SqlEsc(email) & ",'Y'," & db.SqlEsc(otp) & "," & db.SqlEsc(notiftype) & "," & db.SqlEsc(deviceCode) & "," & db.SqlEsc(ipaddress) & ",sysdate, sysdate + (1/24/60 * 20))"
            db.ExecuteNonQuery(SQL)
        End If
        If isOneTrans = True Then db.CommitTransaction()
    End Sub


    Public Shared Sub UpdateVerifyOTP(ByVal DB As DBHelper, ByVal email As String, otp As String, Optional isOneTrans As Boolean = True)
        If isOneTrans = True Then DB.BeginTransaction()
        DB.ExecuteNonQuery("Update webuser_otp set otp = '', ENABLED='N',CREATEDATE = SYSDATE where EMAIL=:1 and otp = :2",
                                  email, otp)
        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Function UpsertHomepage(ByVal DB As DBHelper, ByVal HOMEPAGEIMG_ID As String, ByVal IMGNAME As String, ByVal FOLDERPATH As String, ByVal JFCBRANCHNR As String,
                                          ByVal CREATEUSER As String, ByVal ImgUrl As String, ByVal Item_id As String, ByVal ImgCtg As String, Optional ByVal DISPORDER As Integer = 0, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "Select HOMEPAGEIMG_ID from " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " where HOMEPAGEIMG_ID = :1 and JFCBRANCHNR = :2 "
        Dim IMG_ID = DB.GetScalar(SQL, HOMEPAGEIMG_ID, JFCBRANCHNR)
        If IMG_ID = "" Then
           

            SQL = "SELECT MAX(dispOrder) OVER () max_dispOrder FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE JFCBRANCHNR = :1 AND ImgCtg =:2 FOR UPDATE OF dispOrder"
            DISPORDER = Val(DB.GetScalar(SQL, JFCBRANCHNR, ImgCtg)) + 1
            SQL = "INSERT INTO " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " (HOMEPAGEIMG_ID, IMGNAME, FOLDERPATH, " & _
                         " JFCBRANCHNR, CREATEUSER, REDIRECTURL, Item_id, ImgCtg, CREATEDATE, " & _
                         "DISPORDER ) " & _
                         "VALUES (:1,:2,:3,:4,:5,:6,:7,:8,sysdate,:9)"
            DB.ExecuteNonQuery(SQL, HOMEPAGEIMG_ID, IMGNAME, FOLDERPATH, JFCBRANCHNR, CREATEUSER, ImgUrl, Item_id, ImgCtg, DISPORDER)
        Else
            SQL = "UPDATE " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " SET IMGNAME = '" & IMGNAME & "',FOLDERPATH = " & DB.SqlEsc(FOLDERPATH) &
                ", JFCBRANCHNR = " & DB.SqlEsc(JFCBRANCHNR) & ", MODDATE = sysdate, MODUSER= " & DB.SqlEsc(CREATEUSER) &
                ", REDIRECTURL = " & DB.SqlEsc(ImgUrl) & ", Item_id = " & DB.SqlEsc(Item_id) & ", ImgCtg = " & DB.SqlEsc(ImgCtg) &
                " WHERE HOMEPAGEIMG_ID = :1  "
            DB.ExecuteNonQuery(SQL, HOMEPAGEIMG_ID)
        End If

        InsertEventLog(DB, "InsertUpdate HOMEPAGEIMG", HOMEPAGEIMG_ID, "", Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpdateHomePage(ByVal DB As DBHelper, ByVal HOMEPAGEIMG_ID As String, ByVal IMGNAME As String,
                                          ByVal CREATEUSER As String, ImgUrl As String, Item_id As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "UPDATE " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " SET IMGNAME = '" & IMGNAME & "', MODDATE = sysdate, MODUSER= " & DB.SqlEsc(CREATEUSER) &
                 ", REDIRECTURL = " & DB.SqlEsc(ImgUrl) & ", Item_id = " & DB.SqlEsc(Item_id) & " WHERE HOMEPAGEIMG_ID = :1  "
        DB.ExecuteNonQuery(SQL, HOMEPAGEIMG_ID)
        InsertEventLog(DB, "Update HOMEPAGEIMG", HOMEPAGEIMG_ID, IMGNAME, Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpsertHomePageOrder(ByVal DB As DBHelper, ByVal CurIMG_ID As String, ByVal CurOrder As String, ByVal PrevIMG_ID As String, ByVal PrevOrder As String,
                                               ByVal JFCBRANCHNR As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "UPDATE " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " SET  DISPORDER = " & PrevOrder & "  WHERE HOMEPAGEIMG_ID = :1  and JFCBRANCHNR =:2 "
        DB.ExecuteNonQuery(SQL, CurIMG_ID, JFCBRANCHNR)
        SQL = "UPDATE " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " SET  DISPORDER = " & CurOrder & "  WHERE HOMEPAGEIMG_ID = :1   and JFCBRANCHNR =:2 "
        DB.ExecuteNonQuery(SQL, PrevIMG_ID, JFCBRANCHNR)


        'InsertEventLog(DB, "Update HOMEPAGEIMG Order", CurIMG_ID, PrevOrder, Nothing, False, False)
        'InsertEventLog(DB, "Update HOMEPAGEIMG Order", PrevIMG_ID, CurOrder, Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function DeleteHomePageImg(ByVal DB As DBHelper, ByVal HOMEPAGEIMG_ID As String, JFCBRANCHNR As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        If isOneTrans = True Then DB.BeginTransaction()
        Dim SQL = "DELETE FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE HOMEPAGEIMG_ID = :1 and JFCBRANCHNR =:2 "
        DB.ExecuteNonQuery(SQL, HOMEPAGEIMG_ID, JFCBRANCHNR)

        InsertEventLog(DB, "Delete HOMEPAGEIMG", HOMEPAGEIMG_ID, "", Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function


    Public Shared Function DeleteHomePageAllImg(ByVal DB As DBHelper, ByVal HOMEPAGEIMGNAME As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        If isOneTrans = True Then DB.BeginTransaction()
        Dim SQL = "DELETE FROM " & IIf(IsProduction(), "HOMEPAGEIMG", "HOMEPAGEIMG_TEST") & " WHERE IMGNAME = :1  "
        DB.ExecuteNonQuery(SQL, HOMEPAGEIMGNAME)

        InsertEventLog(DB, "Delete HOMEPAGEIMG", HOMEPAGEIMGNAME, "", Nothing, False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function DeleteOrderReminder(ByVal DB As DBHelper, ByVal hoursprior As String, JFCBRANCHNR As String, Optional ByVal CodeGrp As String = "ORDER_REMINDER", Optional ByVal isOneTrans As Boolean = True) As Boolean
        If isOneTrans = True Then DB.BeginTransaction()
        Dim SQL = "DELETE FROM CodeSetting WHERE Settingvalue1  = :1 and setting_id  =:2 and settinggrp = 'ORDER_REMINDER' "
        DB.ExecuteNonQuery(SQL, hoursprior, JFCBRANCHNR)
        InsertEventLog(DB, "Delete CODEMASTER", hoursprior, JFCBRANCHNR, "ORDER_REMINDER", False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpsertOrderReminder(ByVal DB As DBHelper, ByVal prehoursprior As Nullable(Of Decimal), ByVal curhoursprior As Decimal, JFCBRANCHNR As String, Optional ByVal CodeGrp As String = "ORDER_REMINDER", Optional ByVal isOneTrans As Boolean = True) As Boolean
        If isOneTrans = True Then DB.BeginTransaction()
        Dim SQL As String
        If prehoursprior.HasValue Then
            If prehoursprior.Value = curhoursprior Then
                Return True
            End If
        End If

        SQL = "Select Settingvalue1 from CodeSetting  where setting_id = :1 and Settinggrp = 'ORDER_REMINDER' and Settingvalue1 = :2 "
        Dim Settingvalue1 = DB.GetScalar(SQL, JFCBRANCHNR, curhoursprior)
        If Settingvalue1 <> "" Then
            Throw New FormatException("This hour already exists. Please enter another one.")
        End If

        If prehoursprior.HasValue Then
            SQL = "Update CodeSetting set Settingvalue1 = :1  where setting_id  =:2 and Settinggrp = 'ORDER_REMINDER' and Settingvalue1 =:3"
            DB.ExecuteNonQuery(SQL, curhoursprior, JFCBRANCHNR, prehoursprior)

            InsertEventLog(DB, "Update CODEMASTER", curhoursprior, JFCBRANCHNR, "ORDER_REMINDER", False, False)

        Else

            SQL = "INSERT INTO CodeSetting (Settinggrp,setting_id,Settingvalue1) values('ORDER_REMINDER',:1,:2)"
            DB.ExecuteNonQuery(SQL, JFCBRANCHNR, curhoursprior)
            InsertEventLog(DB, "Insert CODEMASTER", curhoursprior, JFCBRANCHNR, "ORDER_REMINDER", False, False)
        End If

        If isOneTrans = True Then DB.CommitTransaction()

        Return True
    End Function

    Public Shared Sub DeleteCustItemDispOrder(ByVal DB As DBHelper, ByVal customerID As String, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "DELETE FROM customer_itemDispOrder WHERE customer_id = :1"
        DB.ExecuteNonQuery(SQL, customerID)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Sub InsertCustItemDispOrder(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, ByVal dispOrder As Integer, Optional ByVal isOneTrans As Boolean = True)
        Dim SQL As String

        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "INSERT INTO customer_itemDispOrder (customer_id, custGrpItem_id, dispOrder) VALUES (:1,:2," & dispOrder & ")"
        DB.ExecuteNonQuery(SQL, customerID, custGrpItemID)

        If isOneTrans = True Then DB.CommitTransaction()
    End Sub

    Public Shared Function UpsertPromoItem(ByVal DB As DBHelper, ByVal preItemID As String, ByVal curItemID As String, ByVal JFCBranchNr As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String
        If preItemID = curItemID Then
            Return True
        End If
        If isOneTrans = True Then DB.BeginTransaction()
        SQL = "select Item_ID from jfcprice where Item_ID=:1 and NVL(SUBSTR(PRICEGRP_ID,0,2),'')=:2"
        Dim itemid As String = DB.GetScalar(SQL, curItemID, JFCBranchNr)
        If String.IsNullOrWhiteSpace(itemid) Then
            Throw New FormatException("This item # does not exist. Please enter another one.")
        End If

        SQL = "Select Item_ID from JFCPromoItem  where Item_ID = :1 and JFCBranchNr = :2 "
        Dim item_ID = DB.GetScalar(SQL, curItemID, JFCBranchNr)
        If item_ID <> "" Then
            Throw New FormatException("This item # already exists. Please enter another one.")
        End If
        SQL = "delete from  customer_promoitem p where p.item_id = :1 and p.customer_id in (select c.customer_id from customer c where c.jfcbranchnr = :2)"
        DB.ExecuteNonQuery(SQL, curItemID, JFCBranchNr)
        If preItemID <> "" Then

            SQL = "Update JFCPromoItem set Item_ID = :1  where JFCBranchNr = :2  and Item_ID  =:3 "
            DB.ExecuteNonQuery(SQL, curItemID, JFCBranchNr, preItemID)

            InsertEventLog(DB, "Update JFCPromoItem", curItemID, JFCBranchNr, "JFCPromoItem", False, False)

        Else

            SQL = "INSERT INTO JFCPromoItem (JFCBranchNr,Item_ID) values(:1,:2)"
            DB.ExecuteNonQuery(SQL, JFCBranchNr, curItemID)
            InsertEventLog(DB, "Insert JFCPromoItem", curItemID, JFCBranchNr, "JFCPromoItem", False, False)
        End If
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function DeletePromoItem(ByVal DB As DBHelper, ByVal ItemID As String, JFCBRANCHNR As String, Optional ByVal isOneTrans As Boolean = True) As Boolean
        If isOneTrans = True Then DB.BeginTransaction()
        Dim SQL = "DELETE FROM JFCPromoItem WHERE Item_ID = :1 and JFCBranchNr  =:2 "
        DB.ExecuteNonQuery(SQL, ItemID, JFCBRANCHNR)
        InsertEventLog(DB, "Delete JFCPromoItem", ItemID, JFCBRANCHNR, "OJFCPromoItem", False, False)
        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function UpSertCustPromoItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal custGrpItemID As String, ByVal Enabledvalue As String, ByVal CustItemID As String,
                                            Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()
        SQL = " select jp.item_id from jfcprice jp,customer c where NVL(SUBSTR(PRICEGRP_ID,0,2),'') = c.jfcbranchnr and jp.item_id =:1 and c.customer_id = :2"
        Dim itemid As String = DB.GetScalar(SQL, CustItemID, customerID)
        If String.IsNullOrWhiteSpace(itemid) Then
            Throw New FormatException("This item # does not exist. Please enter another one.")
        End If


        SQL = "Select ITEM_ID from Customer_PromoItem where CUSTOMER_ID = :1 and ITEM_ID =:2"
        Dim ITEM_ID = DB.GetScalar(SQL, customerID, CustItemID) 'custGrpItemID.Substring(custGrpItemID.Length - 5, 5)
        SQL = "Select ITEM_ID from JFCPromoItem j, customer c where j.jfcbranchnr = c.jfcbranchnr and c.CUSTOMER_ID = :1 and ITEM_ID =:2"
        Dim BranchItem_ID = DB.GetScalar(SQL, customerID, CustItemID) 'custGrpItemID.Substring(custGrpItemID.Length - 5, 5)
        If ITEM_ID = "" And BranchItem_ID = "" Then

            SQL = "INSERT INTO Customer_PromoItem(CUSTOMER_ID,ITEM_ID,ENABLED,CREATEUSER,CREATEDATE)" & _
           "VALUES (:1,:2,:3,:4,SYSDATE)"
            DB.ExecuteNonQuery(SQL, customerID, CustItemID, Enabledvalue, GetUserEmail(DB))
            InsertEventLog(DB, "Insert Customer_PromoItem", customerID, custGrpItemID, ITEM_ID, False, False)
        ElseIf BranchItem_ID = "" And ITEM_ID <> "" Then
            SQL = "UPDATE Customer_PromoItem SET ITEM_ID = :1, ENABLED = :2 ,CREATEUSER = :3, CREATEDATE = SYSDATE Where CUSTOMER_ID = :4 and ITEM_ID = :5"
            DB.ExecuteNonQuery(SQL, ITEM_ID, "Y", GetUserEmail(DB), customerID, CustItemID)
            InsertEventLog(DB, "UPDATE Customer_PromoItem", customerID, custGrpItemID, ITEM_ID, False, False)
        ElseIf BranchItem_ID <> "" Then
            SQL = "DELETE FROM Customer_PromoItem WHERE Item_ID = :1 and CUSTOMER_ID  =:2 "
            DB.ExecuteNonQuery(SQL, CustItemID, customerID)
            InsertEventLog(DB, "Delete Customer_PromoItem", CustItemID, customerID, "JFCPromoItem", False, False)

        End If

        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Function DeleteCustPromoItem(ByVal DB As DBHelper, ByVal customerID As String, ByVal CustItemID As String,
                                            Optional ByVal isOneTrans As Boolean = True) As Boolean
        Dim SQL As String


        If isOneTrans = True Then DB.BeginTransaction()

        SQL = "Select ITEM_ID from Customer_PromoItem where CUSTOMER_ID = :1 and ITEM_ID =:2"
        Dim ITEM_ID = DB.GetScalar(SQL, customerID, CustItemID)
        If ITEM_ID <> "" Then
            SQL = "UPDATE Customer_PromoItem SET ITEM_ID = :1, ENABLED = :2 ,CREATEUSER = :3, CREATEDATE = SYSDATE Where CUSTOMER_ID = :4 and ITEM_ID = :5"
            DB.ExecuteNonQuery(SQL, ITEM_ID, "N", GetUserEmail(DB), customerID, ITEM_ID)
            InsertEventLog(DB, "Delete Customer_PromoItem", customerID, ITEM_ID, "", False, False)
        Else
            SQL = "INSERT INTO Customer_PromoItem(CUSTOMER_ID,ITEM_ID,ENABLED,CREATEUSER,CREATEDATE)" & _
            "VALUES (:1,:2,:3,:4,SYSDATE)"
            DB.ExecuteNonQuery(SQL, customerID, CustItemID, "N", GetUserEmail(DB))
            InsertEventLog(DB, "Insert Customer_PromoItem", customerID, ITEM_ID, Nothing, False, False)
        End If

        If isOneTrans = True Then DB.CommitTransaction()
        Return True
    End Function

    Public Shared Sub UpdateParam(ByVal DB As DBHelper, ByVal name As String, ByVal index As String, ByVal value As String)
        Dim SQL As String

        SQL = "UPDATE param SET var_value = :1 WHERE var_name = :2 AND var_index = :3"
        DB.ExecuteNonQuery(SQL, value, name, index)
    End Sub
End Class
