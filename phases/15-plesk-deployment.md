# Phase 15: Prepare production publish, Plesk configuration, environment, MySQL, uploads, HTTPS and backups.

## Objective

Prepare production publish, Plesk configuration, environment, MySQL, uploads, HTTPS and backups.

## Current status — 2026-10-11

Owner-approved phase closure (2026-10-11): local production preparation verified (ADR-032).
Real Plesk rollout/rotation, production migration and restore drill remain pending
and are carried into Phase 16 readiness review. This is not launch approval.
Runbook: docs/deployment/PRODUCTION_RUNBOOK.md; checklist: docs/deployment/PLESK_CHECKLIST.md;
evidence and skips: docs/qa/PHASE_15_REVIEW.md. MySQL8 remains the target; actual reported
MariaDB10.3 EOL/DB TLS and .NET8 support deadline remain hosting decision gates.

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
- create production publish instructions
- verify runtime
- IIS/Plesk settings
- environment variables
- MySQL connection
- upload permissions
- HTTPS
- logging
- backup
- migration procedure
