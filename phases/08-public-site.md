# Phase 08: Implement public Razor site, Bootstrap design system, responsive layouts and accessibility.

## Objective

Implement public Razor site, Bootstrap design system, responsive layouts and accessibility.

## Implementation status — 2026-10-10

Implemented and locally verified; owner closure approved (2026-10-10). 185 tests passed, 6 dedicated MySQL tests skipped;
Release build 0 errors/0 warnings. No database update, deployment or automatic advancement to Phase 09.
Reuse existing clean SVG/theme and article/area/Identity/media/SEO modules. Remaining profile/page/FAQ CMS added.
ADR-021, [CMS setup](../docs/cms/PUBLIC_CONTENT_SETUP.md) and [verification report](../docs/qa/PHASE_08_REVIEW.md)
record files, commands, 390 px/keyboard/contrast evidence and human/hosting rollout gates.

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

- LawyerProfile/PageContent/FaqItem, Application ports/DTOs/validators/services and Admin Razor modules.
- Public profile/FAQ/legal templates, reviewed sitemap/FAQPage, dynamic footer and AA focus/components.
- PublicSiteContent migration/SQL (three tables, no seeds), publication/security/contrast/route tests.
- Browser-tested 390 px layouts and documented review/rollout instructions.

## Exit Criteria

The phase is complete only when its implementation and verification criteria are satisfied.

Local criteria verified in the report. Owner closure approved (2026-10-10); MySQL migration validation, approved
real content and hosting checks are explicitly separate rollout gates. Baseline checks are not full WCAG certification.

## Specific tasks
- Remaining profile/page-content/FAQ CMS is deferred here from original Phase 06 (ADR-019).
- Reuse Identity/admin, article/taxonomy/media and SEO modules; no duplicate foundation.
- create Bootstrap design system
- build responsive Razor layouts
- navigation/footer
- profile page
- practice areas
- article listing/detail
- FAQ
- contact
- legal pages
- WCAG baseline

## Visual identity acceptance

- Follow ../docs/design/BRAND_GUIDELINES.md and ADR-017.
- Apply navy/gold Bootstrap tokens, header/footer and accessible component states.
- Replace the temporary CB text mark with a clean asset faithful to the supplied card emblem.
- Keep source photos and personal identity numbers out of public assets/content.
- Verify logo clarity, mobile layout, keyboard/focus and actual component contrast.
