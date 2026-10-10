# Phase 11: Implement contact forms, anti-spam, email notifications and analytics/GTM event architecture.

## Objective

Implement contact forms, anti-spam, email notifications and analytics/GTM event architecture.

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
- contact form
- validation
- antiforgery
- rate limiting/anti-spam
- admin submissions
- configurable SMTP
- analytics/GTM config
- event hooks
- cookie consent if needed

## Implementation — 2026-10-10

Implemented / locally verified; owner closure approved and commit authorized. ADR-024 covers bounded durable notifications and basic analytics consent.
Modules/configuration: ../docs/contact/CONTACT_ANALYTICS_SETUP.md. Verification: ../docs/qa/PHASE_11_REVIEW.md.
No live migration, mail delivery, analytics activation, deployment or Phase 12 work. User-owned config preserved.
