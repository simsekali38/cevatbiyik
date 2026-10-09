# Migration Policy

- EF Core migrations are source-controlled.
- Review generated SQL for destructive changes.
- Production migrations are applied in controlled deployment.
- Backup before destructive schema changes.
- Never manually change production schema without recording the decision.

## Phase 02 — local setup (PowerShell, repository root)

```powershell
dotnet tool restore
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Server=CONTENT_REQUIRED;Port=3306;Database=CONTENT_REQUIRED;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;' --project src/CevatBiyik.Web
```

Replace each CONTENT_REQUIRED locally. Credentials are never written to tracked settings. User-secrets are a local development store, not encrypted production storage. Follow the server certificate/TLS requirements of the actual hosting provider.

InitialCreate is already generated in the repository as source; do not run migrations add again for that migration. Its generation command was:

```powershell
dotnet tool run dotnet-ef migrations add InitialCreate --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --output-dir Persistence/Migrations
```

Generate reviewable SQL without applying it:

```powershell
dotnet tool run dotnet-ef migrations script 0 InitialCreate --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --output artifacts/InitialCreate.sql
```

After SQL review and validation on a dedicated MySQL 8 test database, apply to the intended local development database:

```powershell
dotnet tool run dotnet-ef database update --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web
```

The application does not call Migrate or EnsureCreated at startup. InitialCreate creates four tables, seven indexes (including the FK index), utf8mb4 storage and a restrictive FK; Down drops these tables and loses their data. Never roll back without a backup.

## Persistence validation

`dotnet test CevatBiyik.sln` runs model/SQL and web tests without connecting to MySQL. The real persistence test explicitly skips unless CEVATBIYIK_TEST_MYSQL is provided.
Use a dedicated empty MySQL 8 database whose name starts with `cevatbiyik_test_`; the test applies migrations and rolls back its test data transaction. The schema remains for repeat runs. Do not provide production credentials.

```powershell
$env:CEVATBIYIK_TEST_MYSQL = 'Server=CONTENT_REQUIRED;Database=cevatbiyik_test_phase02;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;'
dotnet test tests/CevatBiyik.IntegrationTests --filter FullyQualifiedName~MySqlPersistenceTests
Remove-Item Env:CEVATBIYIK_TEST_MYSQL
```

Seed strategy: no HasData or automatic inserts. Add only approved, non-sensitive system data in future explicit migrations. Legal content, profile facts, contact details and credentials are never fabricated or seeded.
Fluent API required/max-length mappings constrain storage; application-level validation, HTML sanitization, authorization and consent enforcement belong to later form/CMS phases.
