# Changelog

All notable project changes should be recorded here.

## [Unreleased]

### Phase 10 — Kayseri / local SEO (2026-10-10)
- Added reviewed Location province/district/region hierarchy and manual practice-area/article association management.
- Added encoded public/Admin Razor pages, hierarchical/reciprocal links, reserved slugs and audited protected soft deletion.
- Added bounded runtime thin/similar/unverified/ancestor/related-publication checks; weak pages noindex and excluded from discovery/sitemaps.
- Reused single-office LegalService with verified areaServed; optional reviewed Person, safe JSON-LD and existing global indexing policy.
- Added LocalSeo migration/reviewed SQL with longtext/text MySQL safety, ADR-023, intake rules and verification/setup reports.
- Release: 0 errors/0 warnings; 139 unit + 114 integration = 253 passed, 8 dedicated MySQL tests skipped.
- Four new screens verified at 390 px without horizontal overflow; labels/Tab/Enter focus and desktop Admin checked.
- Owner closure approved (2026-10-10); commit authorized. No live migration/deployment/schema or indexing activation. User-owned appsettings.json edits preserved.

### Phase 09 — SEO and redirect management (2026-10-10)
- Added publication-aware safe same-site 301 rules and audited Admin management; rejected external/protected paths and chains.
- Added PracticeArea SEO/OpenGraph and media reference protection; reused safe schema and existing sitemap/robots.
- Centralized canonical origin and indexing precedence, aligned complete contact facts with sitemap/noindex/schema.
- Added bounded Admin SEO warnings and automated metadata/canonical/schema/redirect/indexing regressions.
- Added SeoRedirects migration/reviewed SQL, ADR-022, SEO setup and verification report; no new dependencies.
- Release: 0 errors/0 warnings; 124 unit + 104 integration = 228 passed, 7 dedicated MySQL tests skipped.
- Three Admin screens checked at 390 px without horizontal overflow. Owner closure approved (2026-10-10); commit authorized; no live migration/deploy.
- Preserved user-owned appsettings.json edits.

### Phase 08 — Public Razor site and reviewed CMS (2026-10-10)
- Preserved ADR-017 navy/gold tokens and clean SVG; added shared FAQ/focus/footer/mobile components and article cover cards.
- Added reviewed profile/vision/bar, homepage introduction, fixed legal pages and paged FAQ Admin CMS.
- Publication requires complete plain text and explicit review; no lawyer facts, legal text, source photos or credentials seeded.
- Added /sss and three legal routes, safe visible-content FAQPage JSON-LD, reviewed sitemap inclusion and metadata choices.
- Production contact now additionally requires published reviewed KVKK; existing consent/feature gates remain.
- Added PublicSiteContent migration/reviewed SQL, ADR-021, CMS instructions and Phase 08 verification report.
- Verified Release build: 0 errors/0 warnings; 102 unit + 83 integration = 185 passed; 6 dedicated MySQL tests skipped.
- Browser baseline: 14 outputs at 390 px without horizontal overflow; actual Tab/Enter skip/menu/FAQ focus and AA token contrast verified.
- Owner approved Phase 08 closure and commit on 2026-10-10; no database update or deployment. User-owned appsettings.json edits preserved and excluded from the commit.

### Phase 07 — Media and private files (2026-10-10)
- Preserved ADR-019 image pipeline; tightened filename/ADS/control-character/output-byte and filesystem reparse checks.
- Added PrivateDocument, fail-closed Defender PDF storage outside wwwroot, SHA-256 verification and Admin-only audited attachment downloads.
- Added soft deletion/recovery, bounded reconciliation including deleted records and abandoned upload parts, 48h orphan grace and private reversible quarantine.
- Added Admin document/files Razor modules, explicit CSRF/confirmation, IIS 6 MiB limit and private App_Data publish exclusion.
- Added MediaFiles migration and reviewed SQL without applying it to cevodb; no new NuGet dependencies.
- Added ADR-020, media setup, Plesk ACL/native/Defender gates and coordinated DB/public/private backup/restore runbook.
- Verified Release build: 0 errors/0 warnings; 86 unit + 67 integration = 153 passed; 5 dedicated MySQL tests skipped (no test connection).
- Local publish succeeded: Windows x64 Skia/native assets, IIS limit and private App_Data exclusion checked. No production deployment or hosting restore drill.
- Owner approved Phase 07 closure and commit on 2026-10-10; concurrent appsettings.json edits were preserved and are outside this phase's changes.

### Phase 06 — CMS Features & SEO Enhancements
- Owner approved phase closure and commit after reviewing 129 passing tests and the 0-error/0-warning build.
- Added separate categories/tags, join mapping, stable slugs, soft deletion and paged public/admin choices.
- Added secure image media: type/byte/pixel checks, EXIF orientation, WebP re-encoding/1600 px resize, alt text and reference-aware removal.
- Added cover/article SEO/OG and fixed-page metadata editors; preserved UTC draft/scheduled/published visibility.
- Added bounded sitemap index/children, robots gate, safe JSON-LD Article/BreadcrumbList and opt-in verified LegalService.
- Added CSP nonce, IIS upload allowlist and filesystem compensation when save/audit fails.
- Fixed malformed multipart error re-execution preserving antiforgery 400.
- Added CmsFeatures migration/reviewed SQL; 129 tests passed, 4 dedicated MySQL tests skipped.
- Release build 0 warnings/errors; NuGet audit clean; 390 px Razor snapshot review passed.
- Single guide renamed 06-cms-features.md; ADR-019 and rollout/QA docs updated. Live database update and deployment were not independently verified.
- Original profile/page-content/FAQ CMS remains Phase 08; Phase 07/09 remaining scopes preserved.

### Phase 05 — Identity, Auth & Admin Panels
- Added ASP.NET Core Identity/MySQL persistence, Admin role/policy, secure session cookies and protected Admin Area.
- Added login/logout with CSRF, lockout, IP rate limit, generic errors and local redirects; explicit first-admin CLI provisioning.
- Added dashboard statistics, articles/practice-area CRUD, message read/unread/deletion and allowlisted contact/SEO settings.
- Added transactional mutation/audit service, append-only audit events and IdentityAdmin migration plus reviewed SQL.
- Added real Identity/cookie/CRUD relational integration tests and dashboard/read-toggle unit tests.
- Renamed the single Phase 05 guide to 05-identity-auth.md; updated ADR-018, setup/security/database/route documentation.
- Database update and first real administrator provisioning remain local rollout steps; no credentials or live data seeded.

### Business-card visual identity
- Applied navy/gold CSS tokens, dark header/footer, accessible focus and responsive brand typography.
- Replaced temporary CB text marks with a local font-free SVG adaptation of the card emblem; added static SVG delivery coverage.
- Added docs/design/BRAND_GUIDELINES.md from the owner's two card photos: navy/gold palette, logo, typography and measured contrast pairs.
- Linked project rules, requirements, content intake and public UI guides to ADR-017; updated the brand-assets open question.
- Documented safe source-image handling; UI adaptation now implemented, no database changes.

### Phase roadmap correction
- Removed duplicate phase numbers: completed 00–04 remain; pending scopes now use 05–17.
- Identity/admin is next (05), followed by CMS (06); no pending scope was removed.
- Updated guide filenames/headings, manifest, phase status table and forward references; recorded ADR-016.

### Phase 04 — Public Web UI & Razor Views
- Added public homepage/about/contact, article/practice-area controllers and encoded Razor list/detail views.
- Added locally hosted responsive Bootstrap 5.3.8, skip link, keyboard navigation, visible focus and server-validation feedback.
- Added public projection queries, title/excerpt search, practice-area filter, SQL-side count/paging/sorting and active setting allowlist.
- Added canonical/OpenGraph/dynamic metadata, default noindex, CSP and security headers.
- Added contact CSRF, request/rate limits, TempData/PRG and production notice gate; search uses antiforgery POST/GET redirect.
- Fixed POST status re-execution to preserve 400/429; corrected narrow-screen gutter overflow.
- Added public-query unit tests and isolated MVC/Razor/security integration tests.
- Recorded Phase 03 acceptance, ADR-015, phase guide mapping and owner-approved Discovery-first workflow.

### Phase 03 — Application & Service Layer
- Added nine DTOs, four application service interfaces/implementations, repository port and EF/MySQL adapter.
- Added Turkish slug generation, bounded collision suffixes, stable URLs on edit, pagination and explicit soft deletion.
- Added published-article filtering, UTC publication validation, contact consent/read processing and public-setting key allowlist.
- Added FluentValidation 12.1.1 and scoped service/validator DI without ASP.NET auto-validation or EF dependencies in Application.
- Added business unit tests and opt-in real MySQL service/repository integration test.
- Added `phases/03-application.md`, updated the phase sequence, architecture and ADR-014; Identity/admin scope remains pending.
- Recorded owner confirmation that Phase 02 migration was applied to `cevodb` and Phase 02 is closed.

### Phase 02 — Domain & Database (2026-10-09)
- Added BaseEntity, Article, PracticeArea, ContactMessage and SiteSetting.
- Added MySQL AppDbContext, per-entity mappings, UTF-8 storage, unique slugs/setting keys, listing indexes and restrictive article/practice-area foreign key.
- Added scoped DI, UTC timestamps, soft-delete query filters and inactive defaults without content seeds.
- Added InitialCreate migration, model snapshot and project-local dotnet-ef 8.0.29 manifest.
- Added model/SQL/DI regression tests and opt-in real MySQL persistence test.
- Documented migration commands, secret configuration, seed policy and ADR-013. Owner confirmed the migration succeeded against `cevodb`; automated live persistence tests remain opt-in.

### Phase 01 — Foundation (2026-10-09)
- Added `CevatBiyik.sln` with `src` (Web, Application, Domain, Infrastructure) and `tests` (UnitTests, IntegrationTests).
- Added `global.json`, `Directory.Build.props` (single `TargetFramework`, nullable, analyzers, NuGet audit), `Directory.Packages.props` (central package versions), `.editorconfig`, `.gitignore`, `.gitattributes`.
- Project references: Web -> Infrastructure -> Application -> Domain (ADR-012).
- Added DI skeletons (`AddApplication` via `AddInfrastructure`), `ErrorController` (`/hata/{code}`), production exception handler, HSTS, noindex-by-default layout.
- Added architecture/DI unit tests and homepage/404 integration tests.
- Canonical `Site:BaseUrl` set to `https://cevatbiyik.av.tr`. Connection string key exists empty; value via user-secrets.
- Added `docs/ENVIRONMENTS.md`, `wwwroot/uploads/.gitkeep`.

### Phase 00 — Discovery (2026-10-09)
- Added `PROJECT_AUDIT.md`; updated `OPEN_QUESTIONS.md`.
- Decision: target framework .NET 8 LTS (user-confirmed; see ADR-008).
- Decision: production domain `cevatbiyik.av.tr` (ADR-009).
- Database: remote MySQL 8; connection string via `dotnet user-secrets` (ADR-010).
