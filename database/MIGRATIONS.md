# Migration Policy

Phase 13 adds guarded opt-in MySQL targets and a separate **empty** `cevatbiyik_test_migrations_*`
Up/Down/Up test. No production migration is added or applied. Read
[test database commands and safety](../docs/testing/TEST_SUITE.md#explicit-mysql-8-opt-in) before enabling either test target.

Owner-approved hosted `cevodbtest` exception: 8 migrations and legacy-contact Up/Down/Up passed on MariaDB 10.3.39,
not MySQL8. Final inspection found no synthetic data rows; schema remains for owner cleanup.
Hosted credentials are only in the test project's local user-secrets; production connection/provider unchanged.

- EF Core migrations are source-controlled.
- Review generated SQL for destructive changes.
- Production migrations are applied in controlled deployment.
- Backup before destructive schema changes.
- Never manually change production schema without recording the decision.

## Phase 02 — local setup (PowerShell, repository root)

```powershell
dotnet tool restore
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Server=CONTENT_REQUIRED;Port=3306;Database=CONTENT_REQUIRED;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;' --project project/src/CevatBiyik.Web
```

Replace each CONTENT_REQUIRED locally. Credentials are never written to tracked settings. User-secrets are a local development store, not encrypted production storage. Follow the server certificate/TLS requirements of the actual hosting provider.

InitialCreate is already generated in the repository as source; do not run migrations add again for that migration. Its generation command was:

```powershell
dotnet tool run dotnet-ef migrations add InitialCreate --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --output-dir Persistence/Migrations
```

Generate reviewable SQL without applying it:

```powershell
dotnet tool run dotnet-ef migrations script 0 InitialCreate --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --output artifacts/InitialCreate.sql
```

After SQL review and validation on a dedicated MySQL 8 test database, apply to the intended local development database:

```powershell
dotnet tool run dotnet-ef database update --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web
```

The application does not call Migrate or EnsureCreated at startup. InitialCreate creates four tables, seven indexes (including the FK index), utf8mb4 storage and a restrictive FK; Down drops these tables and loses their data. Never roll back without a backup.

## Persistence validation

`dotnet test CevatBiyik.sln` runs model/SQL and web tests without connecting to MySQL. The real persistence test explicitly skips unless CEVATBIYIK_TEST_MYSQL is provided.
Use a dedicated empty MySQL 8 database whose name starts with `cevatbiyik_test_`; the test applies migrations and rolls back its test data transaction. The schema remains for repeat runs. Do not provide production credentials.

```powershell
$env:CEVATBIYIK_TEST_MYSQL = 'Server=CONTENT_REQUIRED;Database=cevatbiyik_test_phase02;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;'
dotnet test project/tests/CevatBiyik.IntegrationTests --filter FullyQualifiedName~MySqlPersistenceTests
Remove-Item Env:CEVATBIYIK_TEST_MYSQL
```

Seed strategy: no HasData or automatic inserts. Add only approved, non-sensitive system data in future explicit migrations. Legal content, profile facts, contact details and credentials are never fabricated or seeded.
Fluent API required/max-length mappings constrain storage; application-level validation, HTML sanitization, authorization and consent enforcement belong to later form/CMS phases.

## Phase 05 — IdentityAdmin

Migration 20261009194432_IdentityAdmin adds Identity/AuditLogs without changing content tables or seeding users. SQL is in IDENTITY_ADMIN.sql; database update was not executed by the agent. Run the reviewed setup sequence in ../security/ADMIN_SETUP.md. Never seed an admin password into a migration or run Migrate automatically during production startup.

## Phase 06 — CmsFeatures

20261009202119_CmsFeatures adds five CMS tables, eight nullable article fields, nine indexes and six FKs.
Reviewed SQL: CMS_FEATURES.sql. No content/admin seed or existing-data drop. Down loses CMS data.
MySQL DDL is not fully atomic despite the transaction wrapper. Generation used EF 8.0.29 in artifacts/ef8,
preserving existing EF 10 manifest edits. Compatible commands/MySQL test instructions: ../cms/CMS_SETUP.md.
No database update was executed here.

## Phase 07 — MediaFiles

Adds PrivateDocuments (18 model entity types including Identity), unique StoragePath and digest/size/soft-delete metadata.
Generated/reviewed SQL: MEDIA_FILES.sql. No seeds or destructive Up commands. Down drops document metadata.
Do not regenerate the existing migration. Use artifacts/ef8/dotnet-ef.exe (8.0.29) for this EF 8 app.
No database update was executed by the agent. Dedicated MySQL 8 validation and a combined DB/files backup precede rollout.
Commands and operational gates: ../media/MEDIA_FILES.md and ../deployment/PLESK_MEDIA_RUNBOOK.md.

## Phase 08 — PublicSiteContent

20261009214118_PublicSiteContent adds LawyerProfiles, PageContents, FaqItems and three indexes.
21 model entity types including Identity; shared UTC/soft-delete fields and explicit review flags.
PUBLIC_SITE_CONTENT.sql generated with EF 8.0.29. No seeds, existing-data alteration or destructive Up commands.
Down drops these tables and loses content. MySQL DDL is not fully atomic; validate on dedicated MySQL 8 first.
No cevodb database update was performed. Commands/publication setup: ../cms/PUBLIC_CONTENT_SETUP.md.

## Phase 09 — SeoRedirects

20261009221754_SeoRedirects adds RedirectRules, six nullable PracticeArea fields, unique source/media indexes
and one restrictive media FK. 22 model entity types. SQL: SEO_REDIRECTS.sql, generated with EF 8.0.29.
Up has no seeds/data drops; Down loses redirect/area SEO data. MySQL DDL is not fully atomic.
No database update performed. Dedicated MySQL validation and coordinated backups precede application.
Commands and indexing gates: ../seo/SEO_SETUP.md.

## Phase 10 — LocalSeo

20261009224849_LocalSeo adds three local tables, eight indexes and six FKs (25 entity types). LOCAL_SEO.sql uses EF 8.0.29.
Reviewed longtext/text provider types avoid utf8mb4 varchar row-size overflow. Up has no seeds/data drops; Down loses local data.
No update to cevodb was performed. Dedicated MySQL 8 verification and coordinated backups precede rollout; see ../seo/LOCAL_SEO_SETUP.md.

## Phase 11 — ContactNotifications

Migration 20261010200603_ContactNotifications adds six contact notification fields and one index; defaults do not queue old rows.
Reviewed SQL: CONTACT_NOTIFICATIONS.sql. No live database update performed. Run dedicated test DB first, backup/quiesce,
keep SMTP disabled during migration and verify worker behavior before activation. MySQL DDL may implicitly commit.
Down drops notification progress; contact messages remain. Setup commands: ../contact/CONTACT_ANALYTICS_SETUP.md.
