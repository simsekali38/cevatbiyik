# cevodbtest demo data — manual test-only tool

Phase 13 was committed as `27b03f0`; this utility is a separate owner-requested follow-up, not Phase 14.
No Domain, AppDbContext model, migration, production DI or appsettings change. No additional dependency.

## Files and boundaries

- `tests/CevatBiyik.IntegrationTests/DemoDataSeeder.cs`: explicit seed/cleanup/status and transactional ownership manifest.
- `tests/CevatBiyik.E2EHost/DemoCommand.cs`: guarded manual command using only the test project's local user-secrets.
- `tests/CevatBiyik.IntegrationTests/DemoDataTests.cs`: eight target/isolation/idempotency/rollback/cascade tests.

Requires the exact owner-approved host/account and **Database=cevodbtest**, plus `--confirm cevodbtest`.
`cevodb`, any other database and unconfirmed commands are rejected. There is no application startup hook or Admin action.
The seed service repeats target validation before opening a transaction; only in-memory SQLite is additionally accepted for tests.
The command requires observed MariaDB 10.3.39 and no pending migrations. It never migrates or drops the schema.
Database/config exception details and secrets are suppressed; only fixed messages, operation/type and numeric DB codes appear.

## Commands

From repository root; the test connection is already saved under `Testing:MySql:ConnectionString` in the integration project's
local `cevat-biyik-integration-tests` user-secrets. Do not copy the password to the repository.

```powershell
dotnet build CevatBiyik.sln -c Release
```

Load the demo dataset:

```powershell
dotnet tests/CevatBiyik.E2EHost/bin/Release/net8.0/CevatBiyik.E2EHost.dll --demo seed --confirm cevodbtest
```

Inspect whether the manifest exists:

```powershell
dotnet tests/CevatBiyik.E2EHost/bin/Release/net8.0/CevatBiyik.E2EHost.dll --demo status --confirm cevodbtest
```

Clean only owned demo records:

```powershell
dotnet tests/CevatBiyik.E2EHost/bin/Release/net8.0/CevatBiyik.E2EHost.dll --demo cleanup --confirm cevodbtest
```

Seed repeated twice does not duplicate or overwrite records. Cleanup repeated after success is a no-op.
Do not run ordinary persistence tests while demo records are present: those tests intentionally expect isolated empty data.
Clean first, then `pwsh -NoProfile -File tools/test.ps1 -HostedMySql`.

## Dataset

3 articles (published/draft/future scheduled), 2 practice areas, Kayseri → Melikgazi/Kocasinan hierarchy,
1 FAQ, 1 active LawyerProfile, 2 media metadata records and 6 location/content links.
All titles/text are explicitly **TEST_ONLY DEMO**; there are no legal claims, real client details or invented lawyer facts.
Profile key `demo-test-only-v1` is <=20 characters and never overwrites `primary`. Profile/FAQ/location review stays false;
profile/bar information stays CONTENT_REQUIRED and FactsVerified=false. These records do not bypass publication/indexing gates.
The active profile is a test entity, not a verified replacement for the public primary profile.
Media records are inactive **metadata-only** placeholders: no physical file, thumbnail, public cover or uploaded card image is created.
Use the existing secure media manager for real image pipeline demonstrations; missing-file reconciliation can identify placeholders.

## Cleanup safety

The inactive `SiteSettings` key `Demo.TestOnly.v1.Manifest` stores exact inserted IDs in the same serializable transaction.
It is operational test metadata, not a static EF seed or a production setting. Conflicting unowned markers reject seed.
Cleanup checks exact IDs plus demo identity and linked ownership, includes soft-deleted records, deletes dependents before parents,
and removes the manifest last. Changed/deleted ownership or restrictive non-demo references abort and roll back all changes.
Small bounded OR-by-ID predicates avoid MariaDB 10.3 parameterized collection translation limitations.
No prefix-wide/table-wide wipe, TRUNCATE, foreign-key disabling, DROP/EnsureDeleted, identity/account reset or file deletion.
Preserve the manifest until cleanup succeeds. If demo ownership is edited, restore it or inspect manually; do not broaden deletion.

## MariaDB / SSL notes

Both databases are reported by the owner on MariaDB 10.3 without SSL; only the test endpoint/version was inspected by this tool.
For this isolated command, Pomelo uses `new MariaDbServerVersion(new Version(10, 3, 39))`, matching the actual engine.
Production remains unchanged: its existing MySqlServerVersion(8) assumption needs a separate architecture/hosting review.
Do not relabel MariaDB results as genuine MySQL8 coverage or regenerate production migrations to match this demo.
[Pomelo server-version configuration](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options).

The supplied connection's `SslMode=Preferred` tries TLS and falls back to an unencrypted connection when unsupported.
Required/Verify modes will fail on this host until SSL support is configured; do not silently relax production security.
Keep utf8mb4 and existing utf8mb4_unicode_ci; avoid MySQL8-only collations/features on this old MariaDB target.
[MySqlConnector SSL modes](https://mysqlconnector.net/api/mysqlconnector/mysqlsslmodetype/).

The initial live dry-run exposed non-strict varchar truncation. The tool now sets **STRICT_TRANS_TABLES only on its session**,
preserves existing session modes and uses a valid-length profile key. No global SQL mode or production connection is changed.
Transactional cleanup relies on InnoDB; MySQL/MariaDB DDL implicit commits are irrelevant because this command does DML only.
Hosting engine/version, encrypted DB transport and SH-12-01 remain separate production rollout decisions.

## Verification

Eight new automated demo cases passed. Live seed → seed → cleanup → cleanup succeeded;
the final state contains no owned demo manifest or rows, with tables/migration history preserved.
Full suite results are recorded in the follow-up entry in CHANGELOG. Demo utility changes remain separate from `27b03f0`.

Final follow-up verification: Release build 0 errors/0 warnings; 173 unit + 197 integration = **370 passed**,
1 empty-schema migration case skipped (already passed before demo work), 6 JS + 8 scanner self-tests passed; style/diff checks passed.
Read-only inspection: 8 migrations retained, 0 manifest rows and 0 rows in demo-managed tables.
SH-12-01 remains the only secret-scan finding in config/history; it was not changed or waived by this tool.
