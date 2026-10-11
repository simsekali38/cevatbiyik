# Phase 05 — Identity, Auth & Admin Panels

## Objective

Implement Identity, admin area, authorization, audit logs and dashboard foundation.

## Mandatory process

1. Read `AGENTS.md`.
2. Read the relevant skill files.
3. Inspect current implementation.
4. Produce an implementation plan.
5. Implement only the scope of this phase.
6. Run relevant tests.
7. Run build.
8. Review security.
9. Review SEO where applicable.
10. Update documentation.
11. Update `CHANGELOG.md`.
12. Report files changed, tests run, risks and remaining work.

## Constraints

- Do not invent lawyer information.
- Do not introduce unnecessary dependencies.
- Do not break existing features.
- Do not claim success without verification.

## Deliverables

Document concrete files, code, tests and configuration produced during this phase.

## Exit Criteria

The phase is complete only when its implementation and verification criteria are satisfied.

## Specific tasks
- configure ASP.NET Core Identity
- secure admin login
- create Admin Area
- add authorization policy
- add audit logging
- add dashboard skeleton
- protect every admin route server-side

## Owner-expanded scope and acceptance

- Role is `Admin`; ASP.NET Core Identity 8.0.29 persists on the existing MySQL AppDbContext.
- `/admin/login` and POST `/admin/logout`: antiforgery, generic failures, local redirects,
  Secure/HttpOnly/Strict cookie, account lockout and per-IP login rate limit.
- All Admin controllers are protected by convention and role/policy; only login/access-denied are anonymous.
- Dashboard counts total/public articles, areas and unread contacts; recent audit events are bounded to ten.
- Articles and practice areas support create/edit/soft-delete; articles retain draft/scheduled/published behavior.
- Contact panel supports read/unread with processed timestamp clearing and soft-delete.
- Settings supports allowlisted contact values and site/SEO description, with validation and encoding.
- Writes and audit append use one transaction; explicit delete confirmation and PRG feedback.
- No registration, API/JWT, auto-migration, automatic administrator seed or secret in source.
- Unit and isolated real Identity/relational/cookie integration tests pass; MySQL skips are explicit.

## Status and local rollout

Implemented and locally verified. The migration and first real administrator have NOT
been applied/provisioned against cevodb by the agent. Formal DB rollout closure remains pending.
Setup commands, security options, scope limits and hosting gates:
[ADMIN_SETUP.md](../../docs/security/ADMIN_SETUP.md). Architecture decision: ADR-018.

Migration files: `20261009194432_IdentityAdmin` and updated AppDbContextModelSnapshot.
SQL: [IDENTITY_ADMIN.sql](../../database/IDENTITY_ADMIN.sql).

```powershell
dotnet build project/CevatBiyik.sln
dotnet test project/CevatBiyik.sln
dotnet ef migrations has-pending-model-changes --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web
dotnet list package --vulnerable --include-transitive
```

Expected: no errors/warnings, all non-MySQL tests pass, model matches migration, no reported vulnerable packages.

Executed verification (2026-10-09): build 0 errors/0 warnings; 57 unit + 49 integration
tests passed (106 total), 3 optional MySQL tests skipped because CEVATBIYIK_TEST_MYSQL is unset.
Model check reports no pending changes; package audit reports no known vulnerabilities;
targeted whitespace checks pass. Browser login review at 390 px passes overflow/keyboard/focus checks.
Optional MySQL tests require a dedicated `cevatbiyik_test_` DB; they do not use cevodb.
No automatic transition to Phase 06. Its profile/pages/categories/tags/FAQ scope remains pending;
the core CRUD from this phase is reused rather than implemented again.

Owner approved Phase 05 closure and commit on 2026-10-09 after reviewing 106 passing tests. Database rollout/provisioning is not implied by that code acceptance.
