# Phase 05 — Admin setup and security

## Implementation

ASP.NET Core Identity 8.0.29 is persisted by AppDbContext on MySQL 8.
ApplicationUser is in Infrastructure; Domain/Application do not reference ASP.NET Identity.
The only initial role is `Admin`; the `AdminOnly` policy and Admin-area convention
require it. Only login and access-denied pages allow anonymous access.
No registration, user-management, password-reset or second-factor enrollment endpoint is exposed.
Accounts with 2FA already enabled are denied rather than bypassing their second factor.

Passwords: minimum 12 characters, 4 distinct characters, digit, lowercase, uppercase
and non-alphanumeric character. Email is unique. Five incorrect passwords lock an account
for 15 minutes; ten login POSTs per remote IP/minute return 429 thereafter, with no queue.
Unknown names receive a dummy hash check. Login errors are generic and passwords are not redisplayed.
Rate limiting uses the direct remote IP; trusted proxy configuration must be verified on Plesk.

Cookie: host-only `__Host-CevatBiyik.Admin`, Path=/, HttpOnly, Secure=Always,
SameSite=Strict, session cookie, 30-minute authentication ticket, no sliding expiry.
HTTPS is required even in Development. Security stamps are validated every five minutes;
active status and live Admin role are checked on every authenticated request.
Return URLs are local only. Logout is POST with antiforgery; GET does not log out.
Admin responses are no-store and noindex. Every state-changing form includes an antiforgery token.

## Database rollout (not executed by the agent)

The existing InitialCreate migration remains intact. IdentityAdmin adds seven Identity
tables and AuditLogs; it does not alter the four content tables or seed credentials.
Review the generated [SQL](../database/IDENTITY_ADMIN.sql) and back up the target DB first.
MySQL DDL is not fully transactional: use a reviewed backup/restore plan rather than
assuming a failed migration can be rolled back automatically.

```powershell
dotnet build CevatBiyik.sln
dotnet test CevatBiyik.sln
dotnet ef database update IdentityAdmin --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web
```

Expected new tables: AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims,
AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens, AuditLogs.
The real MySQL tests require CEVATBIYIK_TEST_MYSQL and a dedicated database whose name
starts with `cevatbiyik_test_`; they migrate that test DB and roll back test records.
SQLite tests validate real Identity/cookies/repositories but do not replace MySQL migration verification.

## First administrator (explicit CLI only)

Replace the placeholders locally. Do not paste credentials into chat or commit them.

```powershell
dotnet user-secrets set "AdminBootstrap:UserName" "<ADMIN_USER_NAME>" --project src/CevatBiyik.Web
dotnet user-secrets set "AdminBootstrap:Email" "<ADMIN_EMAIL>" --project src/CevatBiyik.Web
dotnet user-secrets set "AdminBootstrap:Password" "<STRONG_UNIQUE_PASSWORD>" --project src/CevatBiyik.Web
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/CevatBiyik.Web --no-launch-profile -- --provision-admin
dotnet user-secrets remove "AdminBootstrap:Password" --project src/CevatBiyik.Web
dotnet user-secrets remove "AdminBootstrap:UserName" --project src/CevatBiyik.Web
dotnet user-secrets remove "AdminBootstrap:Email" --project src/CevatBiyik.Web
dotnet run --project src/CevatBiyik.Web --launch-profile https
```

Provisioning creates the role/account and an audit event in one transaction, then exits
without starting the HTTP server. It does not migrate the DB automatically.
An existing active Admin account is left unchanged; an existing non-admin is never promoted.
It does not reset an existing password. In Production use protected environment variables
instead of user-secrets and remove bootstrap variables afterward.
Use `/admin/login` over HTTPS after provisioning. Development certificates are configured
with the normal local .NET certificate workflow; do not bypass browser certificate warnings.

## Management scope

| Route | Behavior |
|---|---|
| /admin | SQL counts for total/published articles, practice areas and unread messages; last 10 audit events |
| /admin/articles | Create/update/delete; active flag + UTC publication time encode draft/scheduled/published behavior; slugs stay stable on update |
| /admin/practice-areas | Create/update/delete, active state and sort order |
| /admin/contact-messages | Encoded message display; read/unread toggle, timestamp clearing and soft-delete |
| /admin/settings | Allowlisted contact values, Site.Name and Site.Description (SEO description); create/update/soft-delete |

Lists are bounded to 20 records/page. The article area selector shows the first 100 areas
plus the current selected area; searchable larger catalogs belong to the later CMS refinement.
All deletes require a checked confirmation and keep records through soft-delete.
Published content follows the existing public safety filters, including hidden practice areas.
Editor content is plain text with FluentValidation and Razor encoding. No rich HTML editor is added.
Full per-content SEO metadata, categories/tags, profile/pages/FAQ and expanded publishing
states remain Phase 06/09 scope. Canonical origin, indexing enablement, SMTP and other
security-sensitive configuration are not editable as public SiteSettings.

## Audit and hosting

Domain AuditLog records only action, actor ID and UTC timestamp; no password, token,
IP, username submitted on failure, request body or legal/contact message is copied.
Events cover successful/failed login, logout, provisioning and admin mutations.
CRUD mutation and audit append share one DB transaction: failed audit rolls back the mutation.
Login success audit is saved before the cookie is issued. Audit modification/deletion through
AppDbContext throws. This is application-level protection; DB operators can still alter records.
Per-entity IDs/change diffs and retention/export are later audit enhancements, not implemented here.

Plesk rollout must protect/persist Data Protection keys outside wwwroot, verify HTTPS,
trusted proxy/IP handling and backups. No deployment or live credentials were created in this phase.

## Verification record

Build/test, migration SQL/model checks, package audit and formatter results are recorded in
the phase guide. AdminWebFactory uses an isolated in-memory SQLite database, real Identity
and real relational repositories; no fake authentication handler is used.
Browser login review at 390 px: no horizontal overflow; Tab moves username to password,
visible focus and card-derived brand logo are present. Dashboard and CRUD are rendered and
exercised through authenticated integration tests; no real administrator login was performed.

Primary references: [Microsoft Identity configuration](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-8.0),
[Identity EF package](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore/8.0.29).
The test-only SQLite bundle is pinned to 3.0.3 to avoid the vulnerable 2.1.6 native dependency:
[package](https://www.nuget.org/packages/SQLitePCLRaw.bundle_e_sqlite3/3.0.3),
[advisory](https://github.com/advisories/GHSA-2m69-gcr7-jv3q).
