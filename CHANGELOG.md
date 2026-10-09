# Changelog

All notable project changes should be recorded here.

## [Unreleased]

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
