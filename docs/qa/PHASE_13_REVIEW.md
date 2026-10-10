# Phase 13 verification — 2026-10-11

Status: Implemented / locally verified; owner closure approved; commit authorized. Phase 14 not started.
No production migration, deployment or SMTP/Google activation. Owner-provided test connection is in separate local test user-secrets;
production appsettings.json remains untouched.

## Owner-provided hosting verification addendum

The owner supplied disposable `cevodbtest` after initial local verification. Actual server: **10.3.39-MariaDB**, not MySQL8.
TLS Required connection failed before any mutation: server does not support SSL. Supplied default Preferred mode was then used
only for this synthetic test target. Test credentials were saved in the integration project's own local user-secrets, never Git/output.
Exact host/database/account guard and five additional guard cases added; every provider test checks server version before migration.
Hosted compatibility opt-in accepts only the observed server; production MySqlServerVersion(8) settings are unchanged.

Two **sequential** runner invocations passed:

| Invocation | Result |
| --- | --- |
| `tools/test.ps1 -HostedMySql -MigrationMySql` on empty schema | 173 unit + 180 integration = 353 passed, 10 persistence skips; legacy contact Up/Down/Up passed |
| `tools/test.ps1 -HostedMySql` after schema setup | 173 unit + 189 integration = **362 passed**, 1 migration skip (already passed separately) |
| Distinct .NET cases across the two stages | **363 passed** = 173 unit + 179 local integration + 10 hosted persistence + 1 hosted migration |
| Build/JS/scanner/style | 0 errors / 0 warnings; 6 JS and 8 scanner self-tests passed; style passed |
| Final read-only hosting inspection | 26 tables, 8 applied migrations, **0 data rows** excluding migration history |

The live run exposed a pre-existing FK test setup issue: deleting a tracked optional principal allowed EF to null its navigation
before SQL, so it did not test the database Restrict constraint. The test now clears tracking and deletes an unlinked principal stub,
then independently checks duplicate slug rejection. Targeted and full hosted reruns passed; no production code change.
The migration test removes only its synthetic legacy contact on success; ordinary persistence changes roll back.
Test schema remains for owner inspection/cleanup; no database/account deletion performed.

These results validate the supplied **MariaDB compatibility target**, not genuine MySQL8. Genuine MySQL8, hosting TLS support,
and existing SH-12-01 remain rollout gates. Original 4 browser E2E results remain valid; no browser feature changed in this addendum.
Initial local verification below is retained as the baseline. Updated operations: [TEST_SUITE.md](../testing/TEST_SUITE.md).

## Contract review

Reviewed AGENTS, Phase 00, manifest/PHASE_ORDER, CLAUDE/MASTER_PROMPT, current ADRs including ADR-025,
PROJECT_AUDIT/R-01, OPEN_QUESTIONS, architecture/testing/security/hosting rules and relevant testing/QA/database/security/
architecture/backend/SEO/devops skills. .NET8/MySQL8/Plesk, linear production references, one Razor app and filesystem storage retained.
SH-12-01 remains report-only by owner instruction; no appsettings/history edits or secret values printed.

## Delivered files

| Files | Result |
| --- | --- |
| UnitTests/RegressionGapTests.cs | 8 cases: bounded slug collision exhaustion, contact length/control/encoded markup boundaries, all MySQL migration Up+Down/no seeds/model drift |
| IntegrationTests/MySqlTestGuard.cs | 8 cases: strict explicit safe database target and redacted invalid-config errors |
| IntegrationTests/MySqlPersistenceTests.cs, MySqlRegressionTests.cs | Guard every existing provider connection, new contact/job rollback and separate empty-schema Up/Down/Up legacy-contact test |
| IntegrationTests/WorkflowRegressionTests.cs | 2 linked real MVC/Identity/SQLite journeys: contact/save/queue/admin read-unread/soft-delete/audit and draft/scheduled/published/deleted article |
| IntegrationTests/AdminWebFactory.cs | Optional explicit fixture content root and quiet fixture-only logging; normal test semantics preserved |
| E2EHost project, solution | Local HTTPS gateway around actual MVC fixture, temporary certificate, SQLite/Identity/fake scanner and synthetic content |
| BrowserTests/e2e.test.cjs, package.json/package-lock.json | 4 actual Edge journeys, fresh contexts, external network blocking; pinned test-only Playwright |
| tools/test.ps1, .gitignore | Repeatable failure-propagating runner, explicit MySQL flags, owned hidden fixture lifecycle and ignored Node dependencies |
| TESTING, ARCHITECTURE, DECISIONS, CHANGELOG, phase guides, database/testing/QA docs | ADR-026, safety, coverage matrix, commands/results/limitations |

No production source feature code changed; the pre-existing user-owned appsettings.json modification is outside Phase 13.

## Initial local results (before hosting target was supplied)

| Verification | Result |
| --- | --- |
| Release build, all seven projects | 0 errors / 0 warnings |
| Unit | 173 passed |
| Integration | 174 passed, 11 explicit MySQL skips |
| Total .NET | **347 passed**, 0 failed; +18 passing cases from Phase 12 |
| Real Edge browser E2E, 390px | **4 passed** |
| JS analytics tests | **6 passed** |
| Scanner synthetic self-tests | **8 passed** |
| IDE0011 style verification | Passed |
| npm audit --package-lock-only | 0 vulnerabilities |
| Working tree / reachable history secret scan | Exit 1, SH-12-01: config password at appsettings.json:10 and historical blob b5d0f85ae705; values redacted |

Executed commands:

```powershell
pwsh -NoProfile -File tools/test.ps1 -Browser
node --test --test-concurrency=1 tests/CevatBiyik.BrowserTests/e2e.test.cjs
npm audit --package-lock-only --prefix tests/CevatBiyik.BrowserTests
python tools/security_scan.py --history
git diff --check
```

Local Playwright resolved through the bundled NODE_PATH; installed Edge, not downloaded browser binaries.
New fixture project restore reused cached NuGet packages with audit disabled for that one restore command only;
the central NuGetAudit=true policy remains unchanged. No NuGet package was added/upgraded; Phase 12's successful
direct/transitive advisory audit remains the baseline. New Node lockfile generation and audit contacted the official npm registry.

The browser tests verified real HTML navigation, secure cookies, JS consent/withdrawal, contact success focus and no pre-consent
network loading. Article create/publish and CMS module navigation run through Razor forms. Existing integration suites cover
other CRUD, media/private documents, SEO redirect/local content and queue retries/recovery. Contact completion in the linked
workflow exercises the durable store, not an SMTP delivery. No numerical coverage percentage or full accessibility audit claimed.

## Explicit skips and rollout gates

- No CEVATBIYIK_TEST_MYSQL / CEVATBIYIK_TEST_MYSQL_MIGRATIONS supplied; 10 persistence + 1 destructive test-schema migration
  transition skipped. No production connection used. Offline MySQL SQL/model and SQLite journeys passed, not live MySQL.
- SH-12-01 is unresolved and blocks production; repository scan is intentionally not green or allowlisted.
- Real Defender/native Skia, SMTP delivery, external Google, IIS/Plesk proxy/limits/ACLs/backups require hosting verification.
- R-01 .NET8 support/upgrade remains the existing launch gate. No phase advance/deployment/commit implied by this report.

Commands, schema safety and detailed coverage: [TEST_SUITE.md](../testing/TEST_SUITE.md).
