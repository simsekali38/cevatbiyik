# Article list and detail design transfer — 2026-10-11

## Scope and references

AGENTS.md, CLAUDE.md, MASTER_PROMPT.md, ADR-017, BRAND_GUIDELINES.md and frontend,
accessibility, SEO and architecture skills were reviewed. Clean Git baseline: 60d4a6c.
Verified local references: design/Makaleler-export.png and Makale Detayı-export.png.
Their typography, cream surfaces, cards and reading-column structure were adapted in Razor;
reference placeholder text, fictional claims and external dependencies were not copied.

Owner clarification: editorial content must come from CMS; fixed interface labels are allowed.
Titles, excerpts, categories, tags, cover media, dates and article bodies use current CMS DTOs.
No new static introduction, legal disclaimer, quotation or CTA explanation was added.
Missing optional excerpts/category labels containing CONTENT_REQUIRED are suppressed.

## Changes

- project/src/CevatBiyik.Web/Views/Articles/Index.cshtml: warm heading, native labeled search,
  existing area/category/tag filters, image cards, category links, actual publication dates,
  responsive grid and a bounded numeric page window retaining all existing query filters.
- project/src/CevatBiyik.Web/Views/Articles/Detail.cshtml: complete breadcrumb, category link,
  actual publication/update dates, 860px reading column, sanitized rich text, tags,
  contact action and conditional related-article navigation.
- project/src/CevatBiyik.Web/Controllers/ArticlesController.cs: one bounded existing published search
  for related articles when a category or practice-area relation exists. Both assigned
  relationships are matched; current article is excluded; maximum three suggestions.
- project/src/CevatBiyik.Web/wwwroot/css/site.css and generated site.min.css: article-scoped styles.
  Header/footer, logo, home, other public pages and Admin retain their current implementation.
- project/tests/CevatBiyik.IntegrationTests/ArticleDesignTests.cs: related-publication, real dates,
  sanitization-compatible headings, multi-filter pagination, canonical/noindex and placeholders.
- project/tests/CevatBiyik.BrowserTests/phase14.test.cjs: article screenshots, overflow, native labels,
  breadcrumb, CTA, canonical and axe checks at 390px and 1440px.
- CHANGELOG.md: this transfer and validation record.

No package, domain/application/infrastructure, migration, database or hosting configuration change.
No new architectural boundary: existing controller/services and rich-content sanitizer are reused;
no additional ADR is needed.

## Validation

Executed `project/tools/test.ps1 -Browser` in the isolated fixture after final code changes:
- Release build: zero warnings/errors.
- .NET: 211 unit + 207 integration passed (418 total); 11 opt-in MySQL tests skipped.
- JavaScript analytics: 6 passed.
- Playwright: 6 E2E + 6 accessibility/design + 5 Admin UX passed (17 total).
- Asset freshness, dotnet format style IDE0011, security scanner synthetic self-tests (8),
  publish verifier tests (4), and git diff --check passed.
- Test-only process overrides: AllowedHosts=* and Site__BaseUrl=https://cevatbiyik.av.tr;
  these isolate inherited hosting config without changing any real appsettings file.
- Screenshots personally inspected: artifacts/test-runs/articles-list-{390,1440}.png
  and article-detail-{390,1440}.png. Warm surfaces, aligned cards/filters, readable detail
  column and inherited local emblem verified; no horizontal overflow at either width.
- Existing browser suite additionally checks article reflow at 320px. Automated axe checks
  reported zero violations for the tested article states; search PRG and CMS publication,
  canonical/JSON-LD/security regression suites also passed.

Initial new test source had a quote/encoding error, repaired before the final successful run.
No production/demo database was read or changed. No commit or deployment performed.

## Remaining differences / content requirements

The fixture uses isolated TEST_ONLY content, never production data. Missing covers are omitted,
not replaced by fabricated article imagery. Real approved covers and substantial CMS bodies
are needed for content-complete visual review. Related links only appear when matching public
records exist; the newest first page is sampled, not a semantic recommendation engine.
The reference TOC is optional and no existing TOC feature was found; no fake section titles
or new heading-ID/content-processing behavior was introduced. CMS h2/h3/h4 rendering remains safe.
The CTA intentionally contains only the contact interface action, without a hardcoded legal pitch.
Axe and viewport checks are regression evidence, not a complete WCAG certification.
