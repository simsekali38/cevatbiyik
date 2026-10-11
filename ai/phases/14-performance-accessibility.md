# Phase 14: Measure and improve Core Web Vitals and WCAG 2.2 AA compliance.

## Objective

Measure and improve Core Web Vitals and WCAG 2.2 AA compliance.

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
- Lighthouse measurement
- Core Web Vitals review
- image optimization
- caching
- query optimization
- keyboard testing
- focus testing
- contrast
- semantic review


## Implementation review — 2026-10-11

Implemented and locally verified; owner closure approved (2026-10-11). See [Phase 14 review](../../docs/qa/PHASE_14_REVIEW.md),
[measurement guide](../../docs/performance/MEASUREMENT_GUIDE.md) and ADR-030.
Before/after Lighthouse, static asset compression/cache safety, scoped query snapshots,
keyboard/modal/sticky-focus fixes and regression coverage are complete. No schema migration.
Lab measurements are not field Core Web Vitals certification or a full WCAG conformance statement.
