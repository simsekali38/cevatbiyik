# Phase 13: Implement unit/integration/E2E tests and regression suite.

Status: Implemented / locally verified (2026-10-11), owner closure approved (2026-10-11).
363 distinct .NET cases passed across sequential local/hosted runs (latest run: 362 passed, migration verified separately).
4 actual browser E2E + 6 JS + 8 scanner self-tests passed; build 0 errors/0 warnings.
Hosted target is MariaDB 10.3.39 without SSL, not genuine MySQL8; actual MySQL8 verification remains a rollout gate.
SH-12-01 remains an open production blocker; no config/history edit, production migration or Phase 14 advance.
Concrete deliverables/results: [PHASE_13_REVIEW.md](../docs/qa/PHASE_13_REVIEW.md).
Repeatable commands/coverage/safe MySQL opt-in: [TEST_SUITE.md](../docs/testing/TEST_SUITE.md).

## Objective

Implement unit/integration/E2E tests and regression suite.

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
- unit tests
- integration tests
- critical E2E flows
- SEO checks
- security regression checks
- migration tests
