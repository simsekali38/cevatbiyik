# Architecture Decision Records

## ADR-022 — Local permanent redirects and consistent SEO publication policy

Status: Implemented (2026-10-10, Phase 09); owner closure approved (2026-10-10).

Preserve ADR-019/021 metadata, safe JSON-LD, bounded sitemap/robots, reviewed content and stable slugs.
No new package, project reference, framework, API, app, domain, CDN or fabricated legal/contact content.

RedirectRule stores unique immutable SourcePath and DestinationPath (200 each) with shared UTC/soft-delete fields.
Admin edits/disable/re-enable reuse Identity, explicit binding, CSRF, confirmation and transactional audit.
Sources are reserved by the unique index; no hard deletion. One indexed lookup per eligible GET/HEAD request,
then bounded public-route checks. No unbounded redirect graph loading or per-row diagnostics queries.
Allow only clean root-relative ASCII paths, normalized invariant lowercase; no external host, query, fragment,
encoded separator, backslash, empty/dot segment or protected Admin/error/static/upload/sitemap/robots route.
Fixed public roots cannot be redirect sources. A destination must be currently published public content.
Published source content wins; chains/self loops/unpublished targets fail validation. Runtime revalidates persisted
paths/publication, so direct legacy data cannot create open redirects or loops. GET/HEAD only; never redirect POST.
Return relative 301 with no-store; discard source query strings. HTTP is handled by existing HTTPS middleware.
This phase does not add wildcard/domain redirects, arbitrary Unicode/encoded legacy paths or automatic slug changes.

PracticeArea gains six nullable SEO/OG fields, including a restricted MediaAsset FK, using existing SeoInput,
validators/EditorTemplate and encoded rendering. Referenced media cannot be removed, including references from
deleted areas. Area detail receives BreadcrumbList; no unsupported legal-service facts are invented.
Metadata rejects HTML/placeholders/control characters; invalid legacy metadata falls back to ordinary page text.
OG image paths are constrained to the existing GUID WebP pipeline. Safe JSON/CSP nonce remains unchanged.

One canonical origin validator accepts an HTTPS root without credentials/path/query/fragment; invalid config
falls back to the approved domain and appears as an Admin warning. Request Host never determines canonical/schema.
Global AllowIndexing=false wins over every page-level policy: public meta + X-Robots-Tag are noindex/nofollow,
robots disallows crawling and sitemap is empty/children absent. Errors/Admin stay noindex when indexing is enabled.
Filtered pages, incomplete profile/legal/FAQ and incomplete contact remain noindex. Approved sitemap coverage
includes contact only when visible name/email/phone/address are complete and syntactically valid, independently
of optional LegalService schema. Human contact-data approval is still the existing CMS/intake responsibility.
LegalService additionally requires its explicit reviewed configuration flag. Contact submission retains ADR-021 KVKK gates.

Admin /admin/seo reads bounded fixed-page + 12 article + 12 area metadata and shows indexing/origin/schema settings.
Missing/short/long/fallback/invalid fields are suggestions, not ranking guarantees or mandatory Google limits.
The 30–60 title and 70–160 description character ranges are project editorial heuristics; search display is device-dependent.
NoIndex/crawl restrictions do not secure a staging site or instantly remove already indexed URLs; use access control
for staging and an explicit Search Console/removal workflow when needed, without silently enabling indexing here.

SeoRedirects migration adds one table, six nullable columns, two indexes and one restrictive FK, with no content/credential seeds.
Generated SQL/model/SQLite checks passed; dedicated MySQL 8/hosting validation remains a rollout gate. No cevodb update/deployment.
Setup/evidence: docs/seo/SEO_SETUP.md and docs/qa/PHASE_09_REVIEW.md.
Sources: [Google redirects](https://developers.google.com/search/docs/crawling-indexing/301-redirects),
[Google titles](https://developers.google.com/search/docs/appearance/title-link),
[Google snippets](https://developers.google.com/search/docs/appearance/snippet),
[Google noindex](https://developers.google.com/search/docs/crawling-indexing/block-indexing),
[Microsoft open redirects](https://learn.microsoft.com/en-us/aspnet/core/security/preventing-open-redirects?view=aspnetcore-8.0).

## ADR-021 — Reviewed public content, native FAQ and shared Razor design

Status: Implemented (2026-10-10, Phase 08); owner closure approved (2026-10-10).

Preserve ADR-017 navy/gold tokens and the existing local SVG emblem. No source card photos, identity numbers,
registry numbers or fabricated lawyer/legal facts are copied into public assets, seed data or schema.
Preserve .NET 8/MySQL 8, one MVC app, linear references, local Bootstrap and filesystem media (ADR-019/020).

Add LawyerProfile (one fixed primary key), PageContent (four allowlisted immutable paths) and FaqItem.
Domain has no persistence/UI dependencies. Application owns DTOs, review/publication rules and repository-based
services; Infrastructure maps three tables; Web owns Admin Areas/public controllers and encoded Razor views.
No automatic seed or legal text generation. Inactive drafts may contain CONTENT_REQUIRED. Publication requires
an explicit review flag and complete plain text. Each Admin edit resets the review checkbox so the editor must
review the changed text again. Review is a human assertion, not automated legal validation.
Public projections filter inactive/unreviewed/deleted records and reject incomplete placeholder text.
Fixed pages are disabled/re-enabled; FAQ uses soft deletion. Profile is edited/deactivated through its single row.
All Admin mutations reuse role authorization, antiforgery, explicit binding, validation and transactional audit.

Paths: homepage introduction /, /kvkk, /cerez-politikasi, /yasal-uyari. Profile stays /hakkimda; FAQ stays /sss.
No generic arbitrary route CMS or rich HTML editor. Dynamic footer labels/contact/bar information remain encoded.
Production contact submission additionally requires a reviewed published /kvkk page, alongside existing feature
and consent-notice gates. Missing profile/legal content stays CONTENT_REQUIRED and noindex; sitemap includes
these routes only after reviewed publication. PageSeo metadata allowlist includes FAQ/legal routes.

FAQ is ordered by SortOrder/Id, paged at 12 rows and uses native details/summary with visible focus.
FAQPage JSON-LD represents only that page's visible reviewed questions/answers and uses the existing safe JSON
encoder/CSP nonce. This does not promise Google rich results: Google restricts FAQ rich results to authoritative
government/health sites. Source: [Google FAQ changes](https://developers.google.com/search/blog/2023/08/howto-faq-changes).
Broader SEO/redirects remain Phase 09; full accessibility/performance audit remains Phase 14.

PublicSiteContent adds three tables and three indexes, without credential/content seeds or destructive Up operations.
SQL generation/model tests and SQLite MVC flow tests do not replace dedicated MySQL 8 migration validation.
No migration was applied to cevodb, no deployment or automatic next-phase advancement occurred.
Setup and evidence: docs/cms/PUBLIC_CONTENT_SETUP.md and docs/qa/PHASE_08_REVIEW.md.

## ADR-020 — Private documents, retained files and bounded reconciliation

Status: Implemented (2026-10-10, requested Phase 07 scope); owner phase closure approved (2026-10-10).

Preserve ADR-019 MediaAsset/IMediaService/FileImageStorage and filesystem storage, .NET 8/MySQL 8, one Razor app.
Add separate PrivateDocument with SHA-256 and shared soft-delete fields; no client filename on disk.
Application owns document/scan/catalog/maintenance ports; Infrastructure owns EF/filesystem/Defender adapters.
Accept only conservative PDF documents outside wwwroot. Extension/MIME/signature/classic-xref/content-name checks,
byte limit, mandatory fresh Defender scan on upload/download and digest validation precede release.
Unsupported/active/encrypted/object-stream PDFs are rejected. The content gate is not a full parser or CDR;
zero unknown malware is not guaranteed. Missing/failing scanner rejects operations; no production clean stub.
No new package dependency; preserve tested SkiaSharp pipeline. Filesystem roots reject reparse points;
NTFS ACL must exclude untrusted writers. Same-principal/local-operator TOCTOU is outside this boundary.

Admin-only attachment endpoints with CSRF-protected mutations/audit. Upload DB/audit failure compensates files.
Soft deletion retains files; private downloads immediately stop. Recovery verifies files before reactivation.
Public image old URLs remain public as ADR-019 specifies; confidential content belongs only in private storage.
Reconciliation uses 100-row keyset/scalar and 100-file bounded-memory batches, includes deleted DB rows and
abandoned temporary files; one batch IN lookup avoids N+1 DB queries. Missing/corrupt records are reported.
Orphans older than 48h can be moved, after a fresh DB/age check, into non-public quarantine without overwrite.
Quarantine audit records intent before a non-transactional filesystem move. Crash/failure requires re-scan.
No automatic hard purge: legal retention duration is CONTENT_REQUIRED; files remain recoverable for operator review.
Backups freeze all writers and capture DB + uploads + entire private root under the same batch.
Migration adds PrivateDocuments only. Generated SQL is reviewed; no cevodb update or deployment was run.
Real MySQL/Plesk/Defender/ACL/restore drill remain rollout gates; see docs/media/MEDIA_FILES.md and PLESK_MEDIA_RUNBOOK.md.
Source: [Microsoft Defender CLI](https://learn.microsoft.com/en-us/defender-endpoint/command-line-arguments-microsoft-defender-antivirus).

## ADR-019 — Revised Phase 06 CMS/media/SEO scope

Status: Accepted (2026-10-09, owner's revised Phase 06 instructions).

Keep .NET 8, MySQL 8, Windows/Plesk, linear project references, Razor, one app/domain and filesystem storage.
ArticleCategory is separate from PracticeArea; ArticleTag uses composite ArticleTagLink. Add MediaAsset and PageSeo.
Keep soft-delete/reserved stable slugs; deleted taxonomy labels are hidden in public output, not physically cascaded.
PageSeo supports five fixed routes; disable/re-enable reusable records instead of deleting and reserving these fixed paths forever.
Article metadata is stored with the article. Blank fields use existing fallback; content remains encoded plain text.

Use Infrastructure-only SkiaSharp 3.119.1: MIT license, .NET 8 support and Windows native assets.
Re-encode still JPEG/PNG/WebP to random WebP paths: 5 MiB, 8000 px/axis, 16 MP input limits; 1600 px output longest edge; two concurrent decodes.
Uploads are public images, never legal/client documents. Soft removal retains files; referenced assets cannot be removed.
DB mutation/audit are transactional; compensate filesystem writes on DB/audit failure. MySQL/filesystem are not an atomic distributed transaction.
Process termination can leave orphan files; reconciliation belongs to Phase 07. Scheduled visibility needs no background worker.

ISitemapService projects bounded scalar rows in Infrastructure without loading article bodies or relations.
Sitemap/robots follow indexing gate and configured HTTPS origin; child sitemaps hold at most 1000 entries.
Placeholder profile/contact routes are omitted until content completion. Filtered article URLs are noindex and absent from sitemap.
JSON-LD uses default HTML-safe System.Text.Json encoding in one controlled TagHelper, with a per-response CSP nonce.
Article/BreadcrumbList represent visible published content; LegalService requires visible non-placeholder contact values and explicit reviewed configuration.
No fake reviews, qualifications, locations or production content are seeded.

Images/core SEO are brought forward from 07/09 as requested; those phases are not closed.
Original profile/page-content/FAQ CMS moves to remaining Phase 08 work. The single Phase 06 guide is 06-cms-features.md.
Migration is generated/SQL-reviewed, not applied by the agent. Dedicated MySQL/Plesk validation remains a rollout gate.
Sources: [SkiaSharp package/license/dependencies](https://www.nuget.org/packages/SkiaSharp/3.119.1),
[Microsoft IIS extension allowlist](https://learn.microsoft.com/en-us/iis/configuration/system.webServer/security/requestFiltering/fileExtensions/).

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
