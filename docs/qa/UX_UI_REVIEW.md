# UX/UI follow-up verification — 2026-10-11

Owner-requested project UX work after Phase 13. Rich text and safe HTML explicitly approved.
Phase 14 is not started or closed. Contract reviewed: AGENTS.md, manifest/phase order, Phase 00,
frontend/admin/accessibility/security/architecture/backend skills, brand guidelines and ADR-017/019/021.

## Implemented

- Shared navy/gold admin shell: grouped navigation, active state, labelled mobile offcanvas,
  skip link, responsive forms/tables, dashboard totals and high-contrast header actions.
- Long CMS editors: article/area body, profile biography/vision, page body, FAQ answer, location body.
  Self-hosted Quill, native toolbar, keyboard exit, semantic lists, constrained link dialog,
  undo/redo, counters, expansion and server limits including normalized HTML size.
- Owner screenshot follow-up: Turkish menu strip, grouped SVG icon toolbar, alignment controls,
  content preview and read-only source view. No new editor dependency. Alignment classes are limited
  to three fixed values on five block tags; arbitrary CSS/classes still fail validation.
- Article content/publication columns, collapsible SEO preview, plain field counters,
  UTC/Türkiye display, explicit deletion choices, current-page search and upload feedback.
- Dirty warning without draft storage, submit feedback and duplicate submission prevention.
  Review checkbox is cleared when editing content; server approval requirements retained.
- Public typography and controlled rich rendering. Plain legacy content keeps encoding/line breaks.
  Metadata/contact stay plain; FAQ schema and local quality compare visible text.
- Validation summaries render only on errors, avoiding MVC's empty inline-style list item under CSP.
  JavaScript-off navigation, textarea editing and server validation remain available.
- ADR-029, changelog, usage instructions and vendored license/source/version documented.

## Reproduce

```powershell
npm ci --ignore-scripts --prefix project/tests/CevatBiyik.BrowserTests
pwsh -NoProfile -File project/tools/test.ps1 -Browser
# Optional guarded hosted persistence, connection already in test user-secrets:
pwsh -NoProfile -File project/tools/test.ps1 -Browser -HostedMySql
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
```

## Evidence

Release build: 0 errors, 0 warnings. Unit 196 + integration 188 = 384 passing local .NET tests.
Hosted run: unit 196 + integration 198 = 394 passing; 10 opt-in persistence tests verified against
cevodbtest only. Empty-schema migration upgrade/downgrade test skipped: populated fixture database
must not be cleared. Local run skips all 11 database-dependent cases by design.
Hosted server is MariaDB 10.3.39 with SSL disabled; this is not proof of MySQL 8 compatibility.
No schema migration or production database write is required by this change.

Six existing analytics JS tests, eight synthetic secret-scan self-tests and IDE0011 style checks pass.
NuGet direct/transitive audit and npm locked test-tool audit report no known vulnerabilities.
Browser evidence: six Edge/Playwright journeys cover public/consent/contact, article creation with
real rich save/render, logout/document denial, CSRF/rate limits, mobile menu/editor/link validation,
Tab exit, empty-content feedback, live SEO title, native menus/Escape, safe preview/source,
alignment persistence/public rendering, no page exceptions/CSP violations and JS-off fallback.
390px overflow assertions and 1440px/390px screenshots are in artifacts/test-runs/admin-editor-*.png.
This is a WCAG baseline review, not an external conformance certification.

## Remaining accepted constraints

SH-12-01 remains open under the owner's instruction; appsettings.json and Git history were not modified.
No invented lawyer data, public source-card photos, new API/subdomain, CDN, draft localStorage,
production deployment or automatic next-phase transition. Owner approved committing this follow-up on 2026-10-11.
