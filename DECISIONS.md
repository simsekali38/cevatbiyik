# Architecture Decision Records

## ADR-018 — Identity and expanded Phase 05 admin scope

Status: Accepted (owner requested Identity/Auth plus core CRUD, 2026-10-09)

Canonical guide is phases/05-identity-auth.md; renamed the foundation guide instead of
introducing another Phase 05. Keep .NET 8, MySQL 8, one MVC app/domain and linear layer references.
Infrastructure owns IdentityDbContext/ApplicationUser and EF Identity 8.0.29; Application
owns auth/audit/transaction ports and dashboard/read-toggle business logic, with no ASP.NET dependency.
Add the ASP.NET shared framework to Infrastructure only because Identity's SignInManager is implemented there.
Role name is Admin. Secure cookie, live role/active validation, lockout, login rate limiting,
antiforgery, local return URLs and blanket Admin-area authorization are documented in ADMIN_SETUP.md.
Only login/access-denied allow anonymous access; no registration API or JWT is introduced.
Provision the first Admin through an explicit CLI transaction reading local secrets, never automatic startup seed.

Core articles/practice areas/contact messages/settings panels reuse Phase 03 services.
CRUD and append-only audit share one transaction; only action/actor/time are stored, no submitted content.
Existing active/published-at and soft-delete rules remain; expanded CMS entities/states stay Phase 06.
Site.Name/Description and allowlisted contact settings are editable; security config stays outside CMS.
IdentityAdmin migration adds eight tables; generated SQL is reviewed without applying it to cevodb.
Tests add SQLite solely for isolated real Identity/relational/cookie workflows, not production storage.
Pin test SQLite native bundle 3.0.3 to remove the default vulnerable dependency; audit remains enabled.
MySQL provider-specific tests stay opt-in against a cevatbiyik_test_ database.


## ADR-017 — Business-card visual identity

Status: Accepted design direction (owner request, 2026-10-09)

Use the owner-supplied 20261008_105655.jpg and 20261008_105703.jpg as visual references.
docs/design/BRAND_GUIDELINES.md defines the navy/gold web palette, typography,
contrast pairs and faithful use of the circular CB/scales/column emblem.
HEX values are a web adaptation of photographed print, not exact print specifications.
This supersedes ADR-015's temporary CB text treatment and the current teal design direction.
Use local assets/system fonts; no new dependency, external font, API or schema change.
Do not publish the source photos or transcribe the personal identity number on the front.
CSS/Razor adaptation implemented on 2026-10-09 with a local, hand-drawn SVG preserving
the reference emblem's circle, CB, scales and column. It is a web adaptation, not an exact
original vector reproduction. The original artwork may replace it at the same asset path.
Completed phases 00–04 and the next functional phase 05 stay unchanged.

## ADR-016 — Unique phase numbering

Status: Accepted (owner requested duplicate phase resolution)

Keep completed phases 00 Discovery, 01 Foundation, 02 Domain/Database,
03 Application and 04 Public UI. Move pending Identity/admin to 05 and CMS to 06;
renumber the remaining original roadmap to 07–17 without dropping scope.
PHASE_ORDER.md and project-manifest.json now list all 18 guides with unique numbers.
Forward references to deployment/security/media phases use the new numbers.
Phase 08 retains remaining public-site scope; Phase 04 completion does not complete it.

## ADR-015 — Public Razor UI and explicit public reads

Status: Accepted (Phase 04 — Public UI)

Follow owner's phase sequence, preserving original Identity/admin/CMS as pending scopes. Public controllers use Application interfaces and typed Web models.
No API, SPA, subdomain or new tables. Public queries exclude drafts, scheduled/deleted/inactive data and articles attached to hidden practice areas.
Filtering/count/paging happen before materialization. Category uses existing PracticeAreaId until CMS introduces real article categories.
Article order: publication date then descending Id; practice areas: SortOrder then ascending Id. Lists show 12 rows; category chooser is also paged.
Repository contracts use framework expressions/counts, never EF/IQueryable. Active public settings use an allowlist plus ordinal recheck after DB projection.
Error layout uses static fallback settings without querying persistence, avoiding recursive database-error failures.

Vendor Bootstrap 5.3.8 locally with its MIT license; use system fonts and small custom CSS/JS. No runtime CDN/font/image service.
The CB text treatment is not an assertion of an approved official logo. Unknown biography/contact facts stay CONTENT_REQUIRED; no fake content is seeded.
All text/meta/form values are Razor-encoded; plain content preserves whitespace without Html.Raw.
Canonical/OpenGraph URLs use configured HTTPS Site:BaseUrl, never request Host or arbitrary query strings.
Indexing defaults off; filtered article URLs remain noindex and canonicalize to clean list/page URLs.

Both forms contain explicit antiforgery tokens. Search uses POST then GET redirect so tokens never enter query strings/request URL logs.
Contact POST validates token/model, shows field errors, saves via IContactService and uses TempData/PRG success.
Five POST attempts per remote IP/minute, no queue, bounded 32 KiB request/form fields.
ErrorController supports original HTTP verbs during status re-execution, preserving CSRF 400 and rate-limit 429.
CSP confines scripts/styles/forms to self and blocks objects/frames; nosniff and referrer headers are applied.
Production collection requires Site:ContactFormEnabled=true and a reviewed Site:ContactNotice without CONTENT_REQUIRED; this flag does not perform legal review.
Configure trusted forwarding for actual IIS/Plesk topology before relying on per-IP limits behind a proxy. Client-provided X-Forwarded-For is not trusted here.

Public integration tests exercise real MVC/Razor/Application with isolated repositories; MySQL persistence tests remain opt-in.
Desktop/mobile and keyboard checks are baseline evidence, not full WCAG certification or Lighthouse audit.

## ADR-014 — Application services and persistence port

Status: Accepted (Phase 03 — Application)

The user's Phase 03 Application scope precedes the originally planned Identity/admin phase;
`phases/03-application.md` documents it without silently claiming the Identity scope complete.
Application defines DTOs, four business services, validators and a small bounded IRepository<T> port.
Infrastructure implements the port with the existing scoped AppDbContext. No EF/IQueryable/DbContext types cross into Application.
EF reads use no tracking for lists, stable descending identity ordering and at most 100 rows per page.
FindAsync tracks entities for updates. Soft-delete hides records; updates cannot implicitly restore them.
Each write invokes FluentValidation.ValidateAndThrowAsync before entity mutation or persistence.
FluentValidation 12.1.1 is the only new package, explicitly requested for validation and compatible with net8.0.
No auto-validation MVC integration, mapper, mediator, mock library or API is added.

Turkish slugs map dotted/dotless I and diacritics into ASCII, use hyphen separators and a 200-character limit.
Collisions include deleted records; suffix search is bounded to 1000 candidates. Editing a title preserves its slug.
The DB unique constraint remains the concurrency arbiter; MySQL duplicate key errors map to a safe ConflictException.
Concurrent create conflicts require retry in a fresh request scope; no automatic transaction retry is implemented.
Concurrent updates currently use last-write-wins; a concurrency token is deferred until an editing requirement warrants it.

Article published reads require active status and an elapsed UTC publication date; associated practice areas must be active and undeleted.
Other Get/List methods are internal admin reads and are not safe anonymous endpoints. Authorization belongs to their future Web entry points.
Publication timing and processed-message time use injectable TimeProvider. EF auditing timestamps remain UTC in AppDbContext.
DTO mapping explicitly restores UTC DateTime.Kind for MySQL datetime values, which do not store timezone metadata.
Contact input requires consent, bounded fields and email format validation. Submitted message text is immutable; MarkRead is idempotent.
All content is plain text; literal/encoded markup and disallowed control characters are rejected, but validation does not replace output encoding.
No Html.Raw rendering is permitted for these DTOs. Rich HTML support will need an explicit sanitizer policy later.
CONTENT_REQUIRED is permitted for inactive editorial drafts and cannot be activated/published.

Settings currently allow only Site.Name, Site.Description, Contact.Name, Contact.Email, Contact.Phone,
Contact.Address, Contact.WorkingHours and Contact.WhatsApp. Values are plain text; Contact.Email requires an email except inactive CONTENT_REQUIRED.
No credentials, connection strings or arbitrary SMTP/API keys can be saved through SettingService.
This allowlist can be extended with key-specific validation when new public settings are required.

## ADR-013 — Phase 02 persistence

Status: Accepted (2026-10-09)

Use Pomelo 8.0.3 with EF Core 8.0.29, MySQL 8.0 and utf8mb4_unicode_ci.
EF Core is referenced explicitly to keep runtime and design/tool patch versions aligned.
Tools and Design are private development dependencies of Web; dotnet-ef is pinned in a local tool manifest.
No server autodetection, startup migration, automatic seed, or additional API is introduced.
The EF 10/provider upgrade must be verified together before ADR-008 migration; EF 10 compatibility is not assumed.

Domain contains plain entities without EF dependencies. AppDbContext and mapping classes live in Infrastructure.
Articles optionally belong to one PracticeArea; hard deletes are restricted by the foreign key.
BaseEntity uses bigint identity keys, UTC application timestamps, inactive defaults and explicit soft deletion.
IsDeleted filters ordinary queries; IsActive is deliberately not filtered so admin can read drafts.
Public article queries must additionally require IsActive and PublishedAt <= current UTC time, and exclude deleted/inactive associated areas.
Remove/RemoveRange remain physical deletes; soft deletion requires setting IsDeleted explicitly.
Unique slugs and setting keys remain reserved after soft deletion. Collation makes uniqueness case/accent insensitive.
SaveChanges stamps timestamps; raw SQL and ExecuteUpdate bypass this and require explicit timestamps.
SiteSetting is for public values only; secrets remain in deployment configuration/user-secrets.
No lawyer information is seeded; unknown editorial facts use CONTENT_REQUIRED and stay inactive.

## ADR-001 — Server-rendered ASP.NET Core MVC

Status: Accepted (2026-10-09, Phase 01)

Decision:
Use ASP.NET Core MVC + Razor Views rather than SPA frontend.

Reason:
SEO, performance, simplicity, single deployment, Plesk compatibility.

## ADR-002 — Single Domain

Status: Accepted (2026-10-09, Phase 01)

Decision:
Public website and admin use the same domain.

Reason:
Simpler deployment and authentication; no unnecessary CORS/subdomain complexity.

## ADR-003 — MySQL 8

Status: Accepted

Decision:
Use MySQL 8 with EF Core.

## ADR-004 — Hosting Filesystem

Status: Accepted

Decision:
Store uploads on hosting filesystem.

Mitigation:
Secure paths, validation, backups and controlled access.

## ADR-005 — No API Initially

Status: Accepted

Decision:
Do not build a separate API until a real client requires it.

## ADR-006 — Bootstrap

Status: Accepted

Decision:
Bootstrap is the primary UI framework.

## ADR-007 — No Cloudflare Dependency

Status: Accepted

Decision:
The project must function without Cloudflare.

## ADR-008 — Target Framework: .NET 8 LTS

Status: Accepted (2026-10-09, Phase 00/01). Accepted risk, upgrade plan required.

Decision:
Target .NET 8 LTS (`net8.0`) for the initial implementation. The hosting provider confirmed .NET 8 support on the Plesk server.

Reason:
Hosting compatibility and stability (owner decision, confirmed twice).

Risk:
Microsoft ends support for .NET 8 on 2026-11-10 (no further security fixes or technical support; existing apps keep running). The site will contain a contact form and an admin panel, so running on an unsupported runtime is not acceptable long term. .NET 10 LTS is supported until November 2028.

Mitigation / upgrade plan:
- `TargetFramework` is defined only in `Directory.Build.props`; NuGet package versions only in `Directory.Packages.props`.
- Ask the hosting provider whether the .NET 10 Hosting Bundle can be installed and when.
- Choose packages (EF Core MySQL provider, etc.) that support both .NET 8 and .NET 10; record this in the Phase 02 ADR.
- Upgrade to .NET 10 before launch (Phase 15/16) if hosting allows; otherwise immediately after hosting support lands. Upgrade steps: change `net8.0` to `net10.0`, update `global.json`, bump package versions, run build + all tests, re-verify on Plesk.
- Revisit this ADR at Phase 15 (Plesk Deployment) and at Phase 16 (Production Audit). An unsupported runtime at launch is a Phase 16 blocker unless the owner explicitly accepts it again.

## ADR-012 — Linear project references

Status: Accepted (2026-10-09, Phase 01)

Decision:
Project references are strictly `CevatBiyik.Web` → `CevatBiyik.Infrastructure` → `CevatBiyik.Application` → `CevatBiyik.Domain`.

`AddInfrastructure` registers Application services so the composition root does not reference Application directly. Controllers must not take Infrastructure concrete types.

Reason:
Matches the Phase 01 instruction and keeps a single, testable dependency direction.
