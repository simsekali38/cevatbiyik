# Changelog

All notable project changes should be recorded here.

## [Unreleased]

### Public homepage refinement and local JSON configuration (2026-10-11)
- Balanced the 600px desktop hero with an ivory brand placeholder, consistent type,
  section spacing, card borders/padding and aligned public header/footer marks.
- Added Ana sayfa navigation with current-page state; retained routes and reviewed CMS.
- Suppressed raw CONTENT_REQUIRED contact/profile/process placeholders in the homepage
  and shared footer; show factual empty states instead of invented lawyer information.
- Web now uses local Development JSON rather than automatic user-secrets (ADR-034).
  Real Development JSON is retained locally, ignored/untracked and excluded from publish;
  an inert template is tracked. Existing hosting configurations are unchanged.
- Added Development-provider/localhost and placeholder/navigation regression coverage.
- Comparison screenshots and final verification: docs/design/PUBLIC_HOME_REFINEMENT_REVIEW.md.

### Local development hostname fix (2026-10-11)
- Explicit localhost/loopback AllowedHosts override in Development only; hosting-only
  base configuration no longer rejects local preview requests. Production files unchanged.

### Public layout and homepage — UXMagic transfer (2026-10-11)
- Applied the explicitly approved export (3) homepage composition with local Bootstrap,
  existing navy/gold tokens, system fonts and the unchanged local SVG emblem.
- Added a light navigation shell/information strip, two-column hero, approach cards,
  reviewed CMS profile, process placeholder and contact CTA; retained published articles.
- Preserved metadata, canonical, JSON-LD/CSP, consent and existing contact submission flow.
- No schema, package, Admin or inner-page view changes. Unapproved hero photography and
  legal/process copy remain CONTENT_REQUIRED; no remote UXMagic runtime/assets copied.
- Added four-width homepage reflow/navigation/Axe checks and screenshots to browser tests.
- Verification and remaining content gates: docs/design/PUBLIC_HOME_TRANSFER_REVIEW.md.

### Admin — hide private documents (2026-10-11)
- Owner-disabled private PDF module: removed navigation/help/manual references;
  document routes and private reconciliation/quarantine unavailable by default.
- Preserved stored files/records and image uploads; no migration or antivirus bypass.
- Added disabled-route/UI regression and retained explicit test-only opt-in for
  existing PDF security cases (ADR-033). Updated browser journeys and launch scope.
- Verification: 414 .NET passed (11 database opt-in skips), 54 targeted auth/document
  regressions passed after warning cleanup; 6 JS and final 15/15 browser checks passed.
  Final Release build 0 errors/warnings; initial browser helper expecting 200 was
  corrected for the intended disabled-route 404 and rerun. Publish verification passed.

### Phase 17 — final launch operational templates (2026-10-11)
- Owner-approved closure and commit for requested documentation/templates scope;
  actual production launch and operational evidence remain pending.
- Added FINAL_LAUNCH_RUNBOOK, LIVE_SMOKE_CHECKLIST and ROLLBACK_MONITORING under
  docs/deployment; preserved ADR-032 server-owned config and separate migration identity.
- Extended RESTORE_DRILL_RECORD with UTC timeline, RPO/RTO calculation, explicitly
  synthetic example, per-component hash/count reconciliation and private evidence rules.
- Added Phase 17 review and updated guide/order links; no runtime/schema/dependency change.
- Documentation-only scope: no live deploy, credential rotation, DB migration apply,
  restore, monitoring setup or external notification. Production remains NO-GO.

### Phase 16 — production readiness audit (2026-10-11)
- Owner-approved closure and commit; open production gates carried into Phase 17
  pre-launch checks without claiming rotation, hosting or restore completion.
- Re-ran Release regression, live authorized MariaDB compatibility, browser journeys,
  current/history secret scans, dependency audits and offline migration/publish checks.
- Added isolated phase16 output to the existing Axe/Lighthouse harness without
  overwriting Phase 14 evidence; retained its accessibility/performance budgets.
- Added production readiness report and operator restore evidence form; clarified
  current server-owned JSON policy in SECURITY.md. No domain/schema/dependency change.
- Production rotation, Plesk rollout and coordinated restore require external evidence;
  phase audit does not approve launch or automatically begin Phase 17.
- Results: 425 .NET, 6 JS, 15 browser, 19 Axe routes with no automated violations;
  four-route Lighthouse median performance/accessibility 100, LCP 1.28–1.43s, CLS 0.
  One empty-schema migration skip; history scanner remains failing with two findings.

### Phase 15 — production publish and Plesk preparation (2026-10-11)
- Owner-approved closure and commit; unresolved hosting gates carried into Phase 16.
- Server-owned base/Production JSON removed from Git index without changing local
  contents; placeholder templates and publish exclusions (ADR-032). SH-12-01 rotation
  remains an operator/hosting gate; Git history unchanged.
- IIS out-of-process/Production, hidden config segments, directory browsing disabled;
  preserve 6 MiB body limit and public WebP filtering.
- Explicit offline MySQL8/MariaDB server dialect, hosted MariaDB test alignment and
  EF8 tool manifest. Optional private DPAPI-encrypted persistent hosting key ring.
- Verified publish script, forbidden-content/native/IIS artifact checks and release
  hash manifest; Plesk/config/rotation/migration/ACL/HTTPS/backups/restore runbook and checklist.
- Verification: 425 .NET (10 hosted MariaDB included), 6 JS + 15 browser, 4 publish-boundary
  tests, 8 scanner self-tests; Release 0 errors/warnings, NuGet audit clean. Publish,
  offline MariaDB SQL/bundle and no-pending-model check passed; one empty-schema skip.
- Current-tree secret scan clean; historical credential + synthetic fixture still
  reported (exit 1). No historical scan clean claim; live rotation pending.
- Actual Plesk rollout, production password rotation/migration, restore drill, MySQL8,
  MariaDB EOL/TLS and .NET8 upgrade decisions remain explicitly pending.

### Guided admin UX and editable HTML (2026-10-11)
- Committed owner-approved Phase 14 as 7ccf8c8 before this follow-up.
- Refined navy/gold admin navigation, cards, tables, responsive spacing and labeled
  SVG edit/delete actions. Protected POST operations use a record-specific native
  confirmation dialog with Cancel/focus return and no-JS fallback.
- Editable HTML source with safe apply/rejection, h4/strike/sub/sup formatting;
  six rich-edit screens covered for ordinary-input focus, plus mobile header clearance.
- Eight-step SSR setup guide at /admin/guide, contextual help and beginner manual
  docs/admin/ADMIN_USER_GUIDE.md; private PDF purpose, upload/download/recovery explained.
- Contact KVKK dialog reuses reviewed sanitized page content without navigation or
  loss of form values, including the contact page's consent-banner/footer links.
  Missing legal content retains the existing submission gate.
- Verification: Release 0 errors/warnings; 413 .NET (10 hosted persistence included),
  6 analytics JS + 15 browser journeys, 8 scanner self-tests and asset/format checks.
  One empty-schema migration opt-in skip; no migration added.
- ADR-031 and docs/qa/ADMIN_UX_REVIEW.md record validation and limits. No migration,
  runtime dependency, production setting or secret changes; SH-12-01 remains open.

### Phase 14 — measured performance and accessibility (2026-10-11)
- Static CSS/JS/SVG Brotli/Gzip, verified fingerprint caching and ETag/304; HTML no-store,
  no shared OutputCache/ResponseCaching and no HTML compression (ADR-030).
- Deterministic minified application assets, conditional CMS editor loading, cover priority/
  footer lazy loading and request-scoped reviewed page/profile/public-setting snapshots.
- Fixed Quill format reads stealing keyboard focus; sticky-header scroll margins and explicit
  modal Tab/Shift+Tab cycling with native Escape/focus return.
- Local mobile Lighthouse before/after: four routes, three runs each; performance 95–96 ->100,
  LCP medians 2.48–2.63 s ->1.28–1.35 s, transfer 71–72% lower, CLS 0.
- Added scoped invalidation/cancellation, fixed EF query budget, compression/cache/nonce/image
  tests and axe/keyboard/reflow/dialog/forced-colors browser checks. Measurement commands and
  limitations are in docs/performance/MEASUREMENT_GUIDE.md and docs/qa/PHASE_14_REVIEW.md.
- Verification: 407 .NET tests (10 hosted persistence included), 6 analytics JS + 10 Playwright,
  8 scanner self-tests; Release 0 errors/warnings, npm audit clean. One empty-schema migration skip.
- No migration or production config changes; SH-12-01 remains open. Owner closure/commit approved (2026-10-11).

### UX/UI and safe rich content (2026-10-11)
- Grouped responsive admin navigation, dashboard cards, split article editing, inline validation,
  save/dirty feedback, current-page list filtering, explicit deletion and upload feedback.
- Local Quill 2.0.3 editor for long CMS fields; Application HTML allowlist and safe Razor rendering;
  plain legacy text and plain contact/metadata preserved, no migration required (ADR-029).
- Collapsible SEO fields with live result preview, character/word counters and UTC/Türkiye preview.
- Owner reference image adapted into Turkish editor menus and grouped SVG icons, paragraph alignment,
  safe content preview and read-only source view; no additional editor dependency.
- Public rich typography and FAQ plain-text schema; security and browser regression coverage.
- Verification: 394 .NET tests across local/hosted runs, 6 analytics JS + 6 browser journeys, 8 scanner self-tests; Release 0 errors/warnings. Empty-schema migration case skipped; NuGet audit clean.
- No production configuration/secret change; SH-12-01 remains open. Phase 14 not started.

### Extended test-only SQL dataset (2026-10-11)
- Final commit validation: Release build 0 errors/0 warnings; 173 unit + 197 integration = 370 passing .NET tests, 1 empty-schema migration skip; 6 JS/8 scanner self-tests and style checks passed. Existing SH-12-01 config excluded from commit.
- Hosted service regression now handles pre-existing Site.Name demo data within a rollback transaction and verifies its original value/publication/deletion flags are restored.
- Marked PDF/ZIP artifacts binary so Git newline conversion cannot invalidate file hashes or PDF cross-reference offsets.
- Expanded PROD_SAMPLE_DATA.sql at the owner's request: all 25 application/Identity tables, 264 INSERTs, 24 articles (18 published/3 draft/3 scheduled), complete taxonomy/media/profile/pages/SEO/contacts/audit and Identity relationships.
- INSERT-only data transaction, strict mode, InnoDB/target/FK checks, rollback handler and independent ownership settings; no model, migration or production configuration changes.
- Published and synthetic reviewed states demonstrate a filled test site; national locations remain noindex fixtures, not an expansion of Kayseri-only Application validation. Demo Identity accounts inactive/locked; notification queue disabled.
- Added 8 real WebP/3 PDF companion files, exact bytes/hashes, ZIP and all-table count manifest. PDFs pass application policy; images decoded; production settings untouched.
- Guarded live MariaDB10.3 verification passes all-table counts, hierarchy/FKs, CMS states, assets, forced failure and duplicate-key rollback. No lasting sample rows; existing migration count preserved. Genuine MySQL8 execution remains unverified.

### Test-only demo data follow-up (2026-10-11)
- Phase 13 committed as 27b03f0. Added guarded manual cevodbtest demo seed/status/cleanup under existing test tools, separate from Phase 14.
- Dataset covers articles/statuses, practice areas, Kayseri districts/links, FAQ, active unreviewed nonprimary profile and inactive metadata-only media.
- Serializable ownership manifest, exact-ID cleanup, rollback/idempotency and target checks; no Domain/model/migration/production configuration changes.
- Eight new demo tests and live seed/seed/cleanup/cleanup passed; strict session mode rejects silent field truncation. ADR-027 and DEMO_DATA guide added.
- Follow-up suite: 173 unit + 197 integration = 370 passed, 1 previously verified empty-schema migration skipped; 6 JS/8 scanner self-tests passed; build 0 errors/0 warnings. Eight migrations preserved, no demo data remains.

### Phase 13 — Test suite and regression journeys (2026-10-11)
- Owner supplied hosted cevodbtest; isolated local test user-secrets, exact target guard (+5 cases), explicit hosted mode and server-version checks added.
- Hosted server is MariaDB 10.3.39 without SSL; 11 DB cases passed as compatibility diagnostics, not genuine MySQL8 coverage. Latest full run: 362 passed; migration passed separately; 363 distinct .NET cases verified.
- Corrected tracked-optional-FK test setup; final hosted inspection: 26 tables, 8 migrations and zero data rows. Schema retained for owner cleanup; production config/provider unchanged.
- Added 18 passing unit/integration gap cases, guarded explicit MySQL targets and two new opt-in rollback/migration-transition cases.
- Added four real isolated HTTPS Edge journeys reusing MVC/SQLite/Identity, fresh sessions and blocked external providers.
- Added locked development-only Playwright 1.62.1 and repeatable PowerShell build/test/JS/style/browser/optional security runner.
- Release build: 0 errors/0 warnings; 173 unit + 174 integration = 347 passed, 11 live MySQL skipped; 4 browser, 6 JS and 8 scanner self-tests passed.
- npm audit: zero vulnerabilities. Redacted secret scan still fails SH-12-01 as expected; user-owned config/history preserved.
- Added ADR-026, coverage/operations guide and Phase 13 review. Owner closure approved; commit authorized; no production migration/deployment/Phase 14 work.

### Phase 12 — Security hardening (2026-10-10)
- Added final-response nonce CSP, DENY/nosniff/Permissions-Policy/referrer enforcement on redirects/static/errors/429; preserved attachment sandbox and consent gates.
- Added explicit bounded one-hop trusted proxy For/Proto processing, symmetric headers, nondevelopment HTTPS HSTS180d and strict upload path end anchors.
- Added shared static-resource 120 normalized IP/minute limiter before StaticFiles; normalized login IP keys; explicitly fixed host-cookie Path=/.
- Added 37 integration attack/security cases, 10 active-PDF unit cases, strengthened cookie/download/error assertions and redacted working-tree/history scanner (8 self-tests).
- NuGet direct/transitive advisory audit: no vulnerable packages reported across six projects; no dependency or migration added.
- Verification: 165 unit + 164 integration = 329 .NET tests passed, 9 dedicated MySQL skipped; 6 JS and 8 scanner self-tests passed.
- Added ADR-025, security operations and Phase 12 report. SH-12-01 remains open: password-bearing config/history, owner instructed report-only; appsettings.json preserved.
- Owner closure approved (2026-10-10); commit authorized. No deployment/DB update/provider activation/Phase 13 work; SH-12-01 remains open.

### Phase 11 — Contact / SMTP / consent-controlled analytics (2026-10-10)
- Tightened contact email validation, added nonpersistent honeypot, sliding IP limiter and Retry-After while retaining CSRF/production legal gates.
- Added durable contact notification metadata/migration, atomic leases/recovery/backoff, mandatory authenticated STARTTLS/587 and generic PII-free notices/logging.
- Added Admin notification status and confirmed audited retry for exhausted jobs; existing read/unread/soft deletion preserved.
- Added protected/versioned opt-in cookie, equal-weight changeable Razor preference banner, legal/config gates and nonce/CSP-compatible GTM or GA4 loading.
- Added minimal allowlisted interaction hooks/one-time saved-contact marker, withdrawal/cross-tab refresh and no pre-consent remote requests.
- Added business/relational/MVC and Node built-in event tests; normalized one existing integration-test import order for full style verification.
- Release build: 0 errors/0 warnings. 155 unit + 127 integration = 282 passed, 9 dedicated MySQL skipped; 6 JavaScript tests passed.
- ADR-024, migration SQL, configuration/operations guide and Phase 11 verification report added. Owner closure approved; commit authorized.
- No live migration/provider delivery/Google activation/deployment; user-owned appsettings.json edits preserved.

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
