# Phase 17: Final launch checklist, monitoring, backup/restore verification and post-launch documentation.

## Objective

Final launch checklist, monitoring, backup/restore verification and post-launch documentation.

## Current status — 2026-10-11

Owner requested documentation/templates only. Final launch runbook, live smoke
checklist, rollback/monitoring and restore evidence guidance prepared under
docs/deployment/. No deployment, rotation, migration apply, restore or external
notification performed. Production remains NO-GO pending real evidence/owner GO.
See docs/qa/PHASE_17_REVIEW.md. Owner approved closure of the requested
documentation/templates scope (2026-10-11); actual production launch remains pending.

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
- final backup
- final deployment
- verify domain/canonical
- verify admin
- verify contact
- verify sitemap/robots
- verify analytics
- verify uploads
- verify error pages
- document rollback
- document post-launch monitoring
