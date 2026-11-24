# Security fixes - KOSCUS_JFCAdmin (branch `improve-security`)

Version KOSCUS_JFCADMIN024, not released yet. Short notes are in `ReleaseNotes.txt`.
Numbers (#18, #6, #5) are the item numbers from the security review list.

| # | Problem | Fix | Commit |
|---|---|---|---|
| 18 | Admin role checked only on first page load; `.ashx` handlers not checked | Check on every request in `Global.asax` | `c7f36df` |
| - | `Login/ShowFile.aspx` served any file, e.g. `Web.config` | Only `.pdf` in `Documents\` | `2ccff8f` |
| 6 | Customer Items Import saved any file inside the site | Only `.xls/.xlsx`, saved in `App_Data`, deleted after import | `cff77a8` |
| 5 | Customer-typed text shown as HTML (stored XSS) | HTML encode on display | `69bd3b4` |
| 16 | `BranchSetting.IsUrlValid` could fetch any URL with the server's credentials (SSRF) | Removed (never called) | `1fff50c` |
| 14 | "Remember me" kept the plain password in a 60-day cookie | Signed token cookie, no password | see [#14](#14-password-in-a-60-day-cookie) |

---

## #18 Admin role check on every request

**Problem**
- `JFCAdmin.master.vb` and `JFCAdmin_Mobile.master.vb` checked `Session("SYS_SESSION_USERTYPE") = "JFCADMIN"` only
  inside `If IsPostBack = False`. A postback (button click) skipped the check.
- `Common/Suggestion.ashx` and `Common/Display.ashx` had no check. The forms login cookie is set right after the
  password (`Login.aspx.vb`, before the verification code), so a password alone was enough to call them.

**Fix**
- New `App_Code/AdminAccess.vb` (`AdminAccess.CheckRequest`), called from `Global.asax`
  `Application_PostAcquireRequestState` on every request.
  - Guarded: every `.aspx`, `.ashx`, `.asmx` under `~/JFCAdmin/` and `~/Common/`, and every `.ashx` anywhere.
  - Not guarded: `~/Login/`, `~/Errors/`, `~/Default.aspx`, static files.
  - Not logged in, or not JFC admin: pages redirect to the login page (with a message), handlers return HTTP 401.
- `Suggestion.ashx` and `Display.ashx` implement `IReadOnlySessionState`, so the check can see the login. A guarded
  handler without session state is always refused.
- Both master pages also check the role before `If IsPostBack = False` (second guard), and URL-encode `ReturnUrl`.

**Adding a new page or handler:** nothing to do for pages in `JFCAdmin\` or `Common\`. A new `.ashx` must implement
`IReadOnlySessionState` (or `IRequiresSessionState`), otherwise every call gets 401.

**Not covered:** `AjaxFileUploadHandler.axd` (AjaxControlToolkit, `.axd`).

**Test**
- As JFC admin: every menu page, item search boxes (Customer Items, Customer Settings, Order Detail), Print, Privacy Policy.
- Logged out: open `Common/Suggestion.ashx?code=ITEM&id=...` -> 401. Open a JFCAdmin page -> login page.
- Session timed out, then click a button on an open admin page -> login page ("session has timed out").

---

## ShowFile.aspx (found while working on #18)

**Problem:** `Login/ShowFile.aspx` needs no login and used `NAME` as given:
`Login/ShowFile.aspx?name=..\Web.config&type=text/plain` downloaded `Web.config` / `Secrets.config`.
`TYPE` let the caller choose any content type. `Common/ShowFile.aspx` had the same code.

**Fix:** both pages take only the file name part (`Path.GetFileName`), allow only `.pdf`, always send
`application/pdf`, and return "not found" for anything else. The only links are the Privacy Policy links.

**Test:** Privacy Policy link on the login page and in the admin footer opens the PDF;
`Login/ShowFile.aspx?name=..\Web.config` shows "Could not find file".

---

## #6 Customer Items - Import

**Problem:** `btnImport_Click` (`JFCAdmin/CustItem.aspx.vb`) saved the upload under the user's file name in
`Files\TemporaryFiles\` (inside the site) and never deleted it. Any file type was accepted, so an `.aspx` could be
uploaded and then opened (run) on the server; `..\` in the name could write outside the folder.

**Fix**
- Server check: only `.xls` / `.xlsx`, max 5 MB (`IMPORT_MAX_BYTES`).
- Saved as `App_Data\Upload\<GUID>.xls(x)`; `App_Data` is never served. Deleted in `Finally`, also when the import fails.
- A file the Excel library cannot open shows "Could not read the Excel file" instead of the error page.
- Page: file picker and validator accept `.xlsx` too.
- New `Files\Web.config` with `<handlers accessPolicy="Read" />`: nothing under `Files\` can run as code.

**Deploy:** empty `Files\TemporaryFiles\` on each server (old uploads are still reachable until removed).
`Files/TemporaryFiles/5_20210319(5).xls` is in git and may hold customer data.

**Test:** Export, change order, Import `.xls` -> works. Save the export as `.xlsx` and import -> works.
A `.txt` renamed to `.xls` -> "Could not read the Excel file". After an import `App_Data\Upload\` is empty.

---

## #5 Stored XSS (HTML encoding)

**Problem:** ASP.NET `Label`, `Literal`, `HyperLink` and `LinkButton` write `.Text` as raw HTML (`TextBox` and
`BoundField` encode). Order message, PO number (`custOrderNr`), reference and vendor note come from customers
(app API #70, portal). A message like `<img src=x onerror=alert(1)>` ran in the salesman's / admin's browser on
Order Detail and in the order email.

**Fix**
- New `Common.HtmlText(value)` = `HttpUtility.HtmlEncode(CString(value))`.
- Code-behind, 58 assignments `lblX.Text = CString(r("field"))` -> `HtmlText(r("field"))`:
  `Common/OrderDtl.ascx.vb`, `Common/EmailBody.ascx.vb`, `Common/ShowItemDtl.ascx.vb`,
  `JFCAdmin/CustSetting.aspx.vb`, `JFCAdmin/BranchSetting.aspx.vb`.
- Markup, 17 bindings `Text='<%# Eval("x") %>'` -> `Text='<%#: Eval("x") %>'` (encoded):
  `Balance.ascx`, `EmailBody.ascx`, `OrderDtl.ascx`, `PaymentDtl.ascx`, `Print.aspx`, `BranchSetting.aspx`,
  `CustItem.aspx`, `CustSetting.aspx`.

**Not changed on purpose**
- `itemIdNr_view`: built in SQL with `<br />`. JFC item numbers only.
- Labels whose text is read back and saved: `CustSetting` `lblCustomerName` (chat room table, removal email),
  ship name / item description link buttons. Encoding would save `&amp;`. They hold JFC data only.

**Rule for new code:** display a DB value with `HtmlText(...)` or `<%#: Eval(...) %>`. Use plain `CString` only when
the value must contain HTML, and then make sure nobody but JFC can type it.

**Test:** in the app or portal put `<b>x</b><img src=x onerror=alert(1)>` in the cart message and PO number, place
the order. In JFCAdmin Order Detail and in the order email the text shows as typed (no bold, no popup).
A name or address with `&` (e.g. "Smith & Co") still looks normal.

---

## #16 BranchSetting SSRF

**Problem:** `IsUrlValid(url)` in `JFCAdmin/BranchSetting.aspx.vb` sent a GET request from the web server to any URL it
was given:
- with the server's own Windows network credentials (`request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials`),
  so a URL pointing at an internal host or an attacker's server could receive the server's identity;
- it could reach internal-only hosts (server-side request forgery);
- it set `ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls` (TLS 1.0 only) for the whole application
  each time it ran, which breaks or weakens every other HTTPS call the site makes.

**Finding:** the function is not called anywhere in the site (pages, markup, handlers). Git history shows it came in
with the first commit ("Add project files") and was never called since. It was the only outbound web request in the
site.

**Fix:** removed the function. A comment in its place says what to do if a URL check is ever needed again:
`https` only, a fixed list of allowed hosts, no proxy credentials, and never change `SecurityProtocol`.
`IsValidUrl` (format check only, no request) is unchanged.

**Deploy:** check that the published `BranchSetting.aspx.vb` on the server matches the repo (search it for
`IsUrlValid`); if the server copy calls it, that copy differs from git and must be reviewed first.

**Test:** Branch Settings - save a branch, add / update a homepage image, Promo Item, Order Reminder, Delivery
Master: everything works as before.

---

## #14 Password in a 60-day cookie

Same change as the customer portal (KOSCUS `Documents/SecurityFixes.md`, #14); the code in `Login/Login.aspx.vb` is
the same except for the signing purpose (`KOSCUS_JFCAdmin.Login.RememberMe.v1`), so a token from one site can't be
used on the other.

**Problem:** with "Remember me" checked, `KOSCUS_PASSWD` kept the password as typed for 60 days and the login page
wrote it back into the password box (page source). Emailed links with `AUTO_LOGIN=Y` logged in with that password.

**Fix (no database change)**
- New cookie `KOSCUS_REMEMBER` = `MachineKey.Protect("email|expiresUtcTicks|SHA-256 fingerprint of webuser.password")`:
  encrypted and signed, never the password, `HttpOnly`, `Secure` on https, `SameSite=Lax`, 60 days, renewed at login.
- Login page: email filled, password box "(saved - click Login)", Login with an empty password box logs in.
  `AUTO_LOGIN=Y` links (order / item emails from the API) log in at once with a valid saved login.
- Ends after 60 days, on any password change, or when Login is clicked with "Remember me" unchecked.
- Old `KOSCUS_PASSWD` deleted when the login page is opened and at every login.

**Effect on users:** JFC admins with a saved login type the password once after the release.

**Things to know:** `machineKey` is `AutoGenerate`; another server / app pool without a loaded user profile means users
type the password again (nothing breaks). Logout does not end the saved login (same as before).

**Test**
1. Log in with "Remember me": cookies `KOSCUS_REMEMBER` (HttpOnly) and `KOSCUS_EMAIL`, no `KOSCUS_PASSWD`.
2. Close the browser, open the login page: email filled, no password in the page source, Login works.
3. Open an `AUTO_LOGIN=Y` link from an order email with a saved login: goes straight to the page.
4. Change the password: the saved login no longer works. Log in with "Remember me" unchecked: cookies deleted.
