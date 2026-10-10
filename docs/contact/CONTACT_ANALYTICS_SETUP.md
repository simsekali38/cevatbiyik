# Phase 11 — Contact, notifications and consent-controlled analytics

Implemented 2026-10-10; owner closure approved and commit authorized. ADR-024 preserves .NET 8/MySQL 8, SSR Razor, one app and linear references.
No real SMTP delivery, migration update, deployment, production legal content or analytics activation was performed.
User-owned appsettings.json remains untouched by this phase. Missing real facts/notice/provider details remain CONTENT_REQUIRED.

## Modules

- Application/Interfaces/IContactNotifications.cs: notification policy, sender and durable-store ports.
- Application/Services/ContactService.cs: validated persistence and notification eligibility in one SaveAsync;
  read/unread, soft deletion and exhausted-job retry through existing audited Admin operations.
- Domain/Entities/ContactMessage.cs: six notification metadata fields, no duplicate message payload.
- Infrastructure/Email/: validated SMTP options, constrained sender, EF atomic lease store and hosted worker.
- Web/Controllers/ContactController.cs and Home/Contact.cshtml: encoded fields, honeypot, CSRF and limits.
- Web/Analytics/: runtime availability and versioned Data Protection consent.
- Web/Controllers/ConsentController.cs, Views/Shared/_AnalyticsConsent.cshtml, wwwroot/js/analytics.js and site.js.
- Tests: ContactNotificationTests.cs, ContactAnalyticsTests.cs, dedicated MySqlPersistenceTests case and
  tests/CevatBiyik.BrowserTests/analytics.test.cjs (Node built-in runner; no npm installation).

## Contact flow

Existing production collection still requires Site:ContactFormEnabled, reviewed non-placeholder Site:ContactNotice
and a published/reviewed complete /kvkk page. Contact permission is separate from optional analytics permission.
Use explicit binding, FluentValidation, single-mailbox email checks, safe plain text, Razor encoding and antiforgery.
Honeypot Website must remain empty; an occupied trap gives generic PRG feedback without persistence or a success event.
Trap is outside Tab order and aria-hidden. It is a heuristic, not a CAPTCHA or guaranteed bot detector.
POST limit 32 KiB, form key/value count/length limits retained; five attempts per remote IP in a sliding minute
(six segments), no queue, HTTP 429 + Retry-After: 60. Invalid forms/traps count as attempts.
The connection IP is normalized so IPv4 and mapped IPv6 share a partition. X-Forwarded-For is not trusted here.
Confirm actual IIS/Plesk client-IP topology before launch; a known proxy needs explicit trusted forwarding configuration,
never blanket forwarding trust. Limiter is in-process and resets on recycle; shared NAT users share a budget.
/admin/contact-messages retains Admin authorization, CSRF, encoded message display, read/unread and confirmed soft deletion.

## SMTP configuration — disabled by default

Configure deployment settings or user-secrets, not SiteSettings/CMS, public views or source control.
Smtp:Enabled=false is the absent-key default. Options are read/validated once at startup; change then recycle safely.
Enabled config requires Host, Port=587, UserName, Password, From and Recipient (single mailboxes).
Only authenticated mandatory STARTTLS on 587 is supported; no plaintext fallback, implicit TLS 465, OAuth2 or
certificate validation bypass. This narrow System.Net.Mail.SmtpClient adapter adds no dependency. Microsoft recommends
other clients for new/general-purpose mail implementations; add a maintained client via a future ADR if the provider
requires unsupported authentication/TLS. Confirm STARTTLS, valid certificate chain and port access with Plesk/provider.

Development templates (fill locally; never send real secrets in chat):

```powershell
dotnet user-secrets set "Smtp:Host" "<SMTP_HOST>" --project src/CevatBiyik.Web
dotnet user-secrets set "Smtp:Port" "587" --project src/CevatBiyik.Web
dotnet user-secrets set "Smtp:UserName" "<SMTP_USERNAME>" --project src/CevatBiyik.Web
dotnet user-secrets set "Smtp:Password" "<SMTP_PASSWORD>" --project src/CevatBiyik.Web
dotnet user-secrets set "Smtp:From" "<VERIFIED_FROM_EMAIL>" --project src/CevatBiyik.Web
dotnet user-secrets set "Smtp:Recipient" "<APPROVED_ADMIN_EMAIL>" --project src/CevatBiyik.Web
# Enable only after migration/provider and authorized recipient checks:
dotnet user-secrets set "Smtp:Enabled" "true" --project src/CevatBiyik.Web
```

Production equivalents: Smtp__Host, Smtp__Port, Smtp__UserName, Smtp__Password, Smtp__From,
Smtp__Recipient, Smtp__Enabled in private Plesk environment configuration. Do not run user-secrets list in shared logs.
Delivery contains only a fixed subject/plain-text notice and record ID; no customer name, email, phone, subject,
message, IP, Reply-To or clickable URL. Recipient and From never come from the public form.
A form success confirms DB persistence, not email delivery. No request fire-and-forget tasks.

## Durable notification lifecycle

NotificationPending is set only for new messages created with SMTP enabled, in the same record/save as the message.
Existing rows default false; enabling SMTP does not send historical messages. Disabled SMTP does not process pending work.
Worker first waits 30 seconds, then claims at most one eligible message each 30-second tick using indexed bounded
candidate reads (10 IDs) and a SQL compare-and-set two-minute lease with random lease ID. At most six claimed attempts.
Sender timeout 30 seconds; failures retain pending state with 2, 4, 8, 16, 32, 60-minute backoff. There is no unbounded retry.
A crash/recycle leaves a recoverable lease. An expired job can be reclaimed; stale lease completion cannot overwrite
another worker's claim. Soft-deleted messages are excluded, including a fresh check immediately before SMTP.
Deletion cannot recall mail already in flight. SMTP acceptance followed by a DB failure can cause duplicate notification;
this is at-least-once delivery, not exactly-once. The record ID helps identify duplicates; no duplicate contact rows are created.
Exhausted status is visible in Admin. Correct config, wait for any lease to expire, then confirmed/CSRF-protected
retry resets attempts through the existing transaction+audit (ContactNotificationRetried). Disabled SMTP rejects retry.
No credentials, message bodies or exception details are written by this worker; infrastructure failures log a generic warning.

IIS/Plesk idle shutdown means a hosted worker cannot run continuously without an active process.
Use provider-supported AlwaysRunning/preload/idle settings and verify actual restart/backlog behavior before launch.
Pending DB work survives downtime. This is one application, not a separate worker service or external mail queue.
Back up contact rows and notification metadata together; restoring an older DB may replay notifications.
Set a reviewed data-retention policy for contact rows separately; no automatic hard purge was introduced.

## Analytics configuration and legal gate

Absent Analytics:Enabled is false. Choose exactly one ID: GtmId (GTM-...) OR Ga4Id (G-...). Both/nonmatching IDs fail closed.
Additional required config: Analytics:LegalReviewConfirmed=true, ConsentVersion (1–32 ASCII alphanumeric/_/-),
Notice (20–1000 plain-text characters, no placeholder/markup/control characters). Notice must explain actual provider,
purpose, cookies/durations, third parties and transfers with lawyer review; the application does not invent legal text.
Published/reviewed complete /kvkk and /cerez-politikasi pages are mandatory in every environment.
A complete page/review flag is an editorial assertion, not an automatic legal compliance certificate.
Provider selection, actual retention, transfer assessment, final notices and container review remain owner/lawyer rollout gates.
Config examples (public IDs are not credentials; keep activation false until review):

```json
{
  "Analytics": {
    "Enabled": false,
    "LegalReviewConfirmed": false,
    "GtmId": "",
    "Ga4Id": "",
    "ConsentVersion": "v1",
    "Notice": "CONTENT_REQUIRED"
  }
}
```

Keep these deployment settings separate from user-owned appsettings edits. Environment keys use Analytics__... .
Change ConsentVersion when purpose/provider/notice scope changes so prior acceptance is invalidated.
Consent cookie __Host-CevatBiyik.Consent is Data Protection protected, Secure/HttpOnly/Strict, host-only, root path,
180-day Max-Age plus server-side age/future/version checks. Unknown, malformed, expired or old consent is never acceptance.
Persist Data Protection keys privately with appropriate ACLs across Plesk recycles (also needed by existing Identity).
Key loss fails closed and requires another choice. Consent POST is CSRF-protected, body-limited and rate-limited;
redirects are local and query/fragment-free. No public analytics API was introduced.

## Banner, CSP and event contract

Native details banner in normal document flow before the header: unknown choice is expanded; stored preference remains
accessible and changeable. Equal-weight reject/allow actions; no preselected acceptance, consent wall or contact coupling.
It does not cover focused page content. Short viewport uses bounded internal scrolling. No JavaScript is needed to choose.
Withdrawal writes denial, deletes common GA cookies on host/parent domains and navigates to a page without analytics.
Only functional preference state is broadcast via localStorage after navigation; other tabs reload on a changed state.
If browser storage is blocked, preference still works in the current tab/next navigation; other tabs need refresh.
Cookies outside visible scope/other provider cookies cannot be reliably deleted here; only reviewed GA-only containers
are supported. Consent expiry/config changes apply on navigation; already-running third-party code cannot be recalled.

Server renders analytics.js only for currently available analytics + protected allow cookie. No noscript GTM iframe,
remote loader, cookieless consent ping or event buffer before acceptance. Admin/error pages never contain analytics.
Remote loader copies the per-request nonce; script IDs are allowlisted, never arbitrary script HTML/URLs.
Only opted-in public responses allow narrow Google script/connect/image origins; no unsafe-inline/eval or wildcard.
Configured public HTML uses no-store to avoid reusing a consent-dependent response. Existing canonical/robots/schema remain.
Actual Google/tag-container behavior and outbound endpoints require staging verification; CSP blocks unapproved tags.

Events: phone_click for tel:, email_click for mailto:, whatsapp_click for approved WhatsApp domains,
map_click/cta_click through explicit data-analytics-event attributes. Existing header contact CTA is tagged.
No WhatsApp/map facts or links were invented. Add hooks only to approved visible links.
contact_submit comes from a one-time server success marker after persistence, not a form click/invalid/trap submission.
Only whitelisted event names are emitted; no href/text/name/email/phone/message/IDs/search query/form payload.
Direct GA4 disables automatic pageview, sends origin+pathname (no query) and empty referrer, advertising/signals disabled.
GTM must use reviewed custom events, clean supplied page_location and blank page_referrer; do NOT forward Page URL,
Referrer, click text/URLs, DOM/form values or user-provided variables. Disable GA4 enhanced measurement/form capture,
automatic site-search/outbound events and Google Signals; exclude all advertising/remarketing/Custom HTML tags.
An arbitrary GTM container can read browser data; CSP/our hook payload rules do not sanitize arbitrary third-party tags.
No live Google requests were made during verification.

## Migration and verification

Migration: 20261010200603_ContactNotifications (six columns + one index; no seeds/destructive Up; previous rows not queued).
Reviewed generated MySQL SQL: docs/database/CONTACT_NOTIFICATIONS.sql. MySQL DDL can implicitly commit;
backup/quiesce first, apply to dedicated test DB, validate, then perform owner-approved rollout. Down loses notification metadata.

```powershell
dotnet build -c Release --no-restore -m:1 -p:UseSharedCompilation=false
dotnet test -c Release --no-restore -m:1 -p:UseSharedCompilation=false
node --test tests/CevatBiyik.BrowserTests/analytics.test.cjs
dotnet format style --verify-no-changes --no-restore --severity warn --diagnostics IDE0011
.\artifacts\ef8\dotnet-ef.exe migrations script LocalSeo ContactNotifications --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release
# After dedicated MySQL testing/backup and local authorization, not run by the agent:
.\artifacts\ef8\dotnet-ef.exe database update ContactNotifications --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web
```

Dedicated opt-in tests require CEVATBIYIK_TEST_MYSQL pointing to cevatbiyik_test_*; never use the production DB.
SQLite/fake sender tests do not establish real MySQL locks, provider delivery/TLS, Plesk lifecycle or real GTM behavior.
See docs/qa/PHASE_11_REVIEW.md for measured results and environment-dependent gates.

## Primary references

- [Microsoft SMTP client](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-8.0)
- [Mandatory STARTTLS behavior](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient.enablessl?view=net-8.0)
- [Google CSP and nonce](https://developers.google.com/tag-platform/security/guides/csp)
- [KVKK cookie guidance](https://www.kvkk.gov.tr/Icerik/7353/Cerez-Uygulamalari-Hakkinda-Rehber)
- [KVKK specific/free consent](https://www.kvkk.gov.tr/Icerik/2037/Acik-Riza-Alirken-Dikkat-Edilecek-Hususlar)
