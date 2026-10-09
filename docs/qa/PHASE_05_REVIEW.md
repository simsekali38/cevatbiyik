# Phase 05 verification — 2026-10-09

## Executed

- dotnet build --no-restore: 0 errors, 0 warnings.
- dotnet test --no-build --no-restore: 57 unit and 49 integration tests passed; 3 MySQL tests skipped.
- dotnet ef migrations has-pending-model-changes: model matches latest migration.
- dotnet ef migrations script InitialCreate IdentityAdmin: SQL generated and reviewed; Up adds eight tables only.
- dotnet list package --vulnerable --include-transitive: no known vulnerable packages reported.
- Targeted dotnet format whitespace apply/verify and git diff --check pass.
- Phase manifest points to one 05-identity-auth.md; no duplicate Phase 05 guide.

## Coverage

Real Identity and relational stores/cookies on isolated SQLite; no authentication bypass handler.
Anonymous access redirects, missing role/inactive/2FA denial, lockout, password rules, rate limit,
secure cookie attributes, local redirects, antiforgery login/logout/CRUD, POST-only logout,
CRUD create/edit/soft-delete/publishing/overposting, message read/unread, allowlisted site settings,
legacy private setting rejection, audit rollback/immutability and first-admin bootstrap idempotence.
Unit tests separately verify dashboard publication/unread counts and read-toggle timestamp behavior.

Browser login page at 390 × 844: clientWidth = scrollWidth = 390; Tab moves UserName to Password;
visible focus, logo and form labels present. No real credentials entered and no production login attempted.
Dashboard/editors are rendered through authenticated HTTP integration tests; their full browser E2E,
WCAG/Lighthouse audit and trusted-proxy/Plesk HTTPS rollout remain later deployment/QA scope.

## Not executed

MySQL-specific migration/password/role integration requires CEVATBIYIK_TEST_MYSQL and a dedicated
cevatbiyik_test_ database. Migration update against cevodb and first real administrator provisioning
remain owner-local setup steps in docs/security/ADMIN_SETUP.md. No live DB changes or credentials seeded.

No deployment or automatic Phase 06 transition. Phase 05 implementation is ready; DB rollout closure is pending.
