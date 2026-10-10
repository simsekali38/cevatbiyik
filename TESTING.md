# Testing Strategy

## Phase 13 repeatable suite

`pwsh -NoProfile -File tools/test.ps1` runs Release build, .NET/TRX, JS, scanner self-tests and style checks.
Add `-Browser` for isolated HTTPS Edge journeys, or explicit `-MySql` / `-MigrationMySql` for disposable schemas.
`-HostedMySql` reads the integration project's local user-secrets for the owner-approved temporary hosting target;
this is explicitly MariaDB compatibility diagnosis, not genuine MySQL8 evidence. Run its empty-schema migration stage first.
Prerequisites, coverage and limits: [TEST_SUITE.md](docs/testing/TEST_SUITE.md).
`-SecurityScan` remains a failing release gate while SH-12-01 is open; default regression success does not waive it.

## Unit

Domain rules, services, validation.

Run `dotnet test CevatBiyik.sln` from the repository root. ApplicationServiceTests cover Turkish slugs,
reserved/deleted URL collisions, stable URLs on edits, soft deletion, pagination bounds, publication timing/area visibility,
contact markup/encoded-markup rejection, consent/email validation, read idempotency, setting-key conflicts and inactive placeholders.
These use real validators and a small in-memory repository fake, not an EF InMemory substitute.
PersistenceTests validate MySQL model/SQL and migration drift without opening a database connection.

For real MySQL tests, set `CEVATBIYIK_TEST_MYSQL` to a dedicated empty MySQL 8 database named `cevatbiyik_test_*`.
Run `dotnet test tests/CevatBiyik.IntegrationTests --filter FullyQualifiedName~MySqlPersistenceTests`.
The tests apply migrations only after checking that database name, then roll back test data transactions.
Without that variable they are explicitly skipped; passing unit tests does not imply live database coverage.
Details and connection templates are in `docs/database/MIGRATIONS.md`.

## Integration

Phase 06 adds CmsBusinessTests, CmsFlowTests, ImageStorageTests and a dedicated MySQL CMS test.
Coverage: taxonomy/SEO/publication, native image validation/resizing, audit rollback/file compensation, XML/JSON-LD.
Verified: 129 passed, 4 MySQL skipped without CEVATBIYIK_TEST_MYSQL. Optional responsive Razor snapshot review
uses CEVATBIYIK_CMS_PREVIEW=1; see docs/cms/CMS_SETUP.md.

PublicUiTests/ContactFlowTests exercise real MVC, Razor, Application and validation with isolated PublicWebFactory repositories.
They cover 200 text/html public routes, detail/404, filter/400, encoded legacy markup, configured canonical host, local assets,
CSRF/400, validation feedback, PRG/TempData, overposting prevention, rate limit/429 and production notice gates.
PublicQueryTests cover filtering before paging, publication/SortOrder ordering and safe public settings.
These UI tests never connect to the developer's database or insert test content into it.

- database
- Identity
- repositories/data access
- contact flow
- file storage

## E2E

- public navigation
- admin login
- article creation
- publishing
- SEO metadata
- media upload
- contact submission

## Security tests

- unauthorized admin access
- CSRF
- XSS
- upload abuse
- path traversal
- injection
- rate limiting

## SEO tests

- sitemap
- robots
- canonical
- title
- description
- H1
- schema
- noindex/sitemap consistency
- broken links

## Accessibility

Keyboard navigation
Forms
Focus
Semantic HTML
Contrast
ARIA correctness

## Performance

Lighthouse
Core Web Vitals
TTFB
database query performance
bundle size
image weight

## Phase 05 verification

AdminBusinessTests cover dashboard publication/unread counts and idempotent read/unread timestamp behavior. AdminFlowTests exercise real Identity EF stores and cookies against isolated in-memory SQLite: role/inactive/2FA rejection, lockout, rate limit, antiforgery, safe redirects/logout, CRUD validation/publishing/overposting/soft-delete, audit rollback/immutability and bootstrap policy/idempotence. MySqlPersistenceTests also has an opt-in Identity/password/role/audit test against a cevatbiyik_test_ DB. SQLite is test-only and does not replace MySQL provider verification.

## Phase 11 contact / consent tests

ContactNotificationTests and ContactAnalyticsTests cover eligibility/validation, secret-free notice, worker failure/cancellation,
lease recovery/stale completion/deletion/exhaustion, protected consent/CSRF/redirects/expiry/version, legal gates, nonce/CSP,
Admin retry audit and one-time saved-message event markers. A dedicated opt-in MySQL case covers claim/complete behavior.
Node built-in event tests: `node --test tests/CevatBiyik.BrowserTests/analytics.test.cjs` (6 passing; no npm dependency).
Measured full Release run: 155 unit + 127 integration = 282 passing, 9 MySQL skipped; build 0 errors/0 warnings.
Fake transports/DOM and SQLite do not replace real provider/Plesk/browser tag-container rollout tests. See PHASE_11_REVIEW.

## Phase 12 security verification

SecurityHardeningTests exercise headers on HTML/static/404/authorization/HTTPS/429, fresh nonce, HSTS,
unknown/trusted/asymmetric forwarding, fake forwarded-IP limiter bypass, IPv4-mapped login normalization, contact CSRF
budget, all Admin modules, invalid proxy configuration, image MIME/EXE/script/ADS and pixel-product bombs.
Existing Identity/session/lockout/revocation, private PDF malware-fake/tamper/audit/retention and SEO/consent regressions run.
FileLibraryTests expand escaped active-PDF-name attacks; document tests assert stricter sandbox and headers after 500.
`python tools/security_scan.py --self-test` verifies eight synthetic cases; `--history` intentionally exits 1 for open SH-12-01.
No real malware/Defender/hosting/production database test was run. See docs/qa/PHASE_12_REVIEW.md.
