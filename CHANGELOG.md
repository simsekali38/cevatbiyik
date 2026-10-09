# Changelog

All notable project changes should be recorded here.

## [Unreleased]

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
