# Repeatable test suite — Phase 13

Run from the repository root with PowerShell 7, the pinned .NET 8 SDK, Node >=20 and Python 3.

```powershell
dotnet restore CevatBiyik.sln
pwsh -NoProfile -File tools/test.ps1
```

The runner builds Release, runs unit/integration tests with TRX, six JS analytics tests, eight scanner self-tests
and the existing IDE0011 style check. Failures stop execution. Default runs temporarily clear both MySQL test
variables and restore them in finally. TRX files are in ignored TestResults; fixture logs in artifacts/test-runs.

## Real browser journeys

```powershell
npm ci --ignore-scripts --prefix tests/CevatBiyik.BrowserTests
pwsh -NoProfile -File tools/test.ps1 -Browser
```

Playwright 1.62.1 is locked in package-lock.json as a development-only dependency. Installed Edge is the default;
`$env:CEVATBIYIK_BROWSER_CHANNEL='chrome'` selects installed Chrome. No browser download is performed automatically.
The verified local run reused bundled Playwright 1.62.1 via NODE_PATH and installed Edge; npm ci supplies it in a normal checkout.

The runner owns **https://localhost:57413**, refuses an occupied port and stops its own process in finally.
E2EHost requires `--fixture-only`, binds loopback, checks host/client and proxies to the existing real MVC TestServer.
It reuses temporary SQLite, actual Identity and a fake scanner. Configuration sources are cleared and content is synthetic TEST_ONLY;
no developer database, production content, SMTP delivery or deployment. The temporary certificate is never installed/trusted
in an OS store; only the fixed loopback readiness probe and browser fixture context ignore its trust error.

Fresh browser contexts isolate cookies/storage. Reserved per-context test IPs isolate limiter buckets; X-Test-Connection-IP
is interpreted only in the test host. External browser requests are aborted, including the post-consent GTM loader attempt.
No Google code executes. Direct APIRequestContext checks use only the fixed fixture URL.
Four scenarios run serially against a fresh fixture: rerunning against prior CMS creations is unsupported.

Coverage: mobile public navigation/search/article/FAQ, actual CSP/JSON-LD, consent refusal/allow/withdrawal,
contact PRG/focus/one-time success, admin cookies/login/article publish/CMS navigation/logout,
private-document denial, CSRF, robots/noindex/empty sitemap and login 429.
This is not complete visual/WCAG certification or external analytics delivery verification.
Isolation follows [Playwright browser contexts](https://playwright.dev/docs/browser-contexts)
and [BrowserContext API](https://playwright.dev/docs/api/class-browsercontext).

## Explicit MySQL 8 opt-in

### Owner-provided hosted compatibility target

The owner supplied disposable `cevodbtest`. Server inspection found **MariaDB 10.3.39, not MySQL 8**, and SSL is unsupported.
This is a separate compatibility diagnosis, never proof of MySQL 8 or a change to production/provider architecture.
Only the exact approved host/database/account tuple is additionally allowed; explicitly disabled TLS is rejected.
The owner's supplied default Preferred SSL mode falls back to an unencrypted test connection on this server.
No real personal content is inserted. Production TLS/connection settings are untouched.

The integration project has its own UserSecretsId. Store `Testing:MySql:ConnectionString` with `dotnet user-secrets set`
for that project locally; never place the value in appsettings, scripts or Git. The local value has already been saved.
On Windows, the runner reads that project's local secrets without printing them:

```powershell
# First, only while the schema is entirely empty:
pwsh -NoProfile -File tools/test.ps1 -HostedMySql -MigrationMySql
# Then, reuse the resulting latest schema for rollback-isolated persistence tests:
pwsh -NoProfile -File tools/test.ps1 -HostedMySql
```

Run sequentially. The migration test deletes only its synthetic legacy contact on success, retaining the schema.
It still refuses an already migrated/nonempty schema; do not repeat it without a freshly provisioned disposable schema.
The hosted flag explicitly opts into the observed MariaDB version; ordinary opt-in tests reject non-MySQL8 servers before migration.
When the owner removes the database/account, remove this test secret too:

```powershell
dotnet user-secrets remove 'Testing:MySql:ConnectionString' --project tests/CevatBiyik.IntegrationTests
```

The agent does not automatically drop the database or revoke the hosting account.

### Genuine MySQL 8 target

Provision disposable schemas and a restricted test account; set credentials locally, never commit them.

```powershell
$env:CEVATBIYIK_TEST_MYSQL = 'Server=localhost;Database=cevatbiyik_test_regression;User ID=TEST_USER;Password=TEST_PASSWORD;'
pwsh -NoProfile -File tools/test.ps1 -MySql
Remove-Item Env:CEVATBIYIK_TEST_MYSQL
```

The guard rejects malformed/missing connections and names outside `cevatbiyik_test_[a-z0-9_]+` (<=64 chars)
before connection/migration. No fallback to appsettings/user-secrets. Ten persistence cases apply migrations and roll back
test-data transactions, including contact/job rollback. Use a schema with no prior contact/jobs, never production.
MySQL DDL can implicitly commit: migration execution is not covered by data rollback.

The separate migration transition test requires an **entirely empty** schema on every run:

```powershell
$env:CEVATBIYIK_TEST_MYSQL_MIGRATIONS = 'Server=localhost;Database=cevatbiyik_test_migrations_run01;User ID=TEST_USER;Password=TEST_PASSWORD;'
pwsh -NoProfile -File tools/test.ps1 -MigrationMySql
Remove-Item Env:CEVATBIYIK_TEST_MYSQL_MIGRATIONS
```

It rejects any existing tables, migrates to Phase 10, inserts a synthetic legacy contact, upgrades ContactNotifications,
downgrades that migration and upgrades again. Contact text survives and old rows are not retroactively queued.
Down loses notification progress in this disposable schema; this is not a production rollback procedure.
The schema is retained for inspection, never automatically deleted. Use a different database from the ordinary persistence suite.
Both switches can be combined. Without variables **11 live MySQL tests skip**; offline/SQLite checks do not prove provider behavior.

## Targeted checks and release gates

```powershell
dotnet test tests/CevatBiyik.IntegrationTests -c Release --filter Category=Workflow
dotnet test tests/CevatBiyik.IntegrationTests -c Release --filter FullyQualifiedName~SecurityHardeningTests
dotnet test tests/CevatBiyik.IntegrationTests -c Release --filter FullyQualifiedName~SeoEnhancementTests
python tools/security_scan.py --history
npm audit --package-lock-only --prefix tests/CevatBiyik.BrowserTests
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
```

`tools/test.ps1 -SecurityScan` adds the redacted working-tree/history scan and **fails on findings**.
SH-12-01 remains a release blocker, never allowlisted. Default success tests the scanner itself, not repository cleanliness.
Phase 12's direct/transitive NuGet audit remains the baseline; Phase 13 adds no runtime/NuGet dependency.
Never publish tests, gateway binaries, fake content, source business cards or local configuration to Plesk.

## Coverage map

| Scope | Automated evidence |
| --- | --- |
| Services, validation, slug collisions, scheduling, soft deletion | ApplicationServiceTests, CmsBusinessTests, RegressionGapTests |
| Identity/roles/lockout, CRUD and audits | AdminFlowTests, CmsFlowTests, PublicContentFlowTests, WorkflowRegressionTests |
| Public routes/search/FAQ/legal placeholders | PublicUiTests, PublicContentFlowTests, browser visitor journey |
| Contact/anti-spam/CSRF/consent/events | ContactFlowTests, ContactAnalyticsTests, analytics.test.cjs, browser journeys |
| Notification claims/retries/recovery/admin retry | ContactAnalyticsTests, WorkflowRegressionTests; no SMTP delivery |
| Upload abuse/private documents/retention/recovery | ImageStorageTests, DocumentFlowTests, FileLibraryTests, SecurityHardeningTests |
| Metadata/schema/canonical/sitemap/robots/redirects | SeoEnhancementTests, CmsFlowTests, browser journeys |
| Local content/links/doorway/noindex | LocalSeoFlowTests and local SEO unit tests |
| CSP/proxy/headers/cookies/access | SecurityHardeningTests, AdminFlowTests, browser journeys |
| Offline MySQL SQL/model/all migration Up+Down/no seeds | PersistenceTests, RegressionGapTests |
| Live MySQL persistence/constraints/claims/migration transitions | MySqlPersistenceTests, MySqlMigrationRegressionTests, explicit opt-in |

No code-coverage percentage is claimed. Real MySQL, Defender/SMTP, IIS/Plesk proxy/limits/ACLs/native libraries
and external Google delivery remain environment-dependent verification.
