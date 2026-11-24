# Customer Settings: only admin roles can change settings

Date: 11/24/2026. Branches: version-2.2, version-2.3, version-2.4, improve-security.

## Problem

A sales rep (any JFC user whose `webUser_vendor.userCompany_id` is not `JFC_BRANCH`, `JFC_HO` or `KMS_SUPPORT`)
could change and save customer settings on **Customer Settings** (`JFCAdmin/CustSetting.aspx`).

How it happened:

1. On first load, `Page_Load` locks `pnlCustSetting` and `pnlUser` and hides `btnSaveCust`, `btnUpdateChnage`,
   `btnAddSchedule` for these users.
2. The **Delivery Schedule** grid (`pnlSchedule`) is **outside** `pnlCustSetting`, so its master dropdowns and the
   Send Reminder / Notify check boxes stayed editable, and its **Update Changes** button (`btnUpdateSchedule`) was not hidden.
3. Changing a master dropdown posts back (`AutoPostBack`). On every postback `Page_Load` called `SetUpdateButton(True)`,
   which turned **Update Changes** on for everyone.
4. `btnSaveCust_Click` (shared by all three save buttons) did not check the role, so the click saved the schedule and
   the settings.

## Fix (`JFCAdmin/CustSetting.aspx.vb`)

- New `IsJfcAdminGroup()` reads `ViewState("JFC_ADMINGROUP")` (set in `Page_Load` from the role; ViewState is MAC-protected).
- New `DenyIfNotAdmin()`: if not admin, turns the save buttons off, shows "You do not have permission to change
  customer settings." and returns True.
- `Page_Load`: `btnUpdateSchedule` is hidden for non-admin users; on postback `SetUpdateButton(True)` only runs for admins.
- `grd_RowDataBound`: for non-admin users the `lstDeliveryMaster` dropdown and the `ChkSendReminder` / `ckbEmail`
  check boxes are disabled (look only).
- Server-side check (`If DenyIfNotAdmin() Then Exit Sub`) at the top of: `btnSaveCust_Click` (Save / Update Changes x3),
  `btnPopSaveDelivery_Click`, `btnPopDeleteDelivery_Click`, `grdActiveSchedule_RowCommand` (DELETE_ROW),
  `btnAddschedule_Click`, `lstDeliveryMaster_SelectedIndexChanged`.

Roles that can change settings: `JFC_BRANCH` and `KMS_SUPPORT` only.

## Update 11/24/2026: JFC_HO is look-only too

The first fix kept `JFC_HO` in the edit list (it was allowed before). Only `JFC_BRANCH` and `KMS_SUPPORT` should
change customer settings, so `jfcRole = "JFC_HO"` was removed from the role check in `Page_Load`. JFC_HO users now
see Customer Settings the same way as sales reps (grayed out, no save / add / delete), and the server-side checks
refuse their saves. JFC_HO keeps its other rights (Order Form, Branch Settings, Tool, branch lists) - unchanged.
The user list (Add / delete user) keeps its own rules (`btnPopSaveUser_Click`) and was not changed.

## Deploy

Code-behind only: deploy `JFCAdmin/CustSetting.aspx.vb` (or the compiled site). No DB change, no config change.

## How to test

1. Log in as a sales rep. Open Customer Settings for one customer.
   - Settings panel and user list are grayed out; no Save / Update Changes / Add Schedule buttons.
   - Delivery Schedule: master dropdowns and check boxes are grayed out; no Update Changes button; no delete (X) icons.
   - Change the ship-to dropdown or the customer: still no save button appears.
2. Log in as JFC_HO: same as step 1 (look only).
3. Log in as JFC_BRANCH (and KMS_SUPPORT): change a delivery master, Update Changes turns green, save works
   as before; add / delete schedule works.
4. Optional: as a sales rep or JFC_HO, re-enable the button with browser dev tools and click it: the page shows the permission
   message and nothing is saved.

## Still open

- Nothing for this page.
