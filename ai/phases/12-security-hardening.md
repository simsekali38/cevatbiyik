# Phase 12: Perform security hardening, headers, rate limiting, validation, upload review and dependency audit.

## Objective

Perform security hardening, headers, rate limiting, validation, upload review and dependency audit.

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
- security headers
- CSP strategy
- HSTS where appropriate
- cookie settings
- rate limiting
- upload penetration checks
- auth review
- authorization review
- dependency audit
- secret scan

## Phase 12 implementation record — 2026-10-10

Implemented / locally verified; owner closure approved (2026-10-10); commit authorized. ADR-025, Web security/consent middleware, proxy allowlist/static
limiter/path guards, expanded security tests and redacted project/tools/security_scan.py produced.
Detailed files/results/commands: docs/qa/PHASE_12_REVIEW.md; hosting controls: docs/security/SECURITY_HARDENING.md.
No migration/dependency/deployment. SH-12-01 unresolved: owner explicitly requested leave credential-bearing appsettings.json
and Git history untouched, report only. This finding prevents a clean security scan/production release assertion.
No Phase 13 started.
