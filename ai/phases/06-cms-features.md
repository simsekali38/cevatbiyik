# Phase 06 — CMS Features & SEO Enhancements

## Objective

Implement the owner's revised Phase 06 scope: article categories/tags, secure image media,
scheduled publication, page/article metadata, XML sitemap/robots and accurate structured data.
Status: completed, owner-approved closure and commit. Dedicated MySQL/Plesk rollout validation remains a deployment gate.

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
- Reuse Phase 05 Identity/admin and core CRUD services.
- Separate categories from practice areas; one optional category and up to 20 tags per article.
- Stable unique slugs, soft deletion and active public taxonomy filters.
- Validate JPEG/PNG/WebP, limit bytes/pixels, orient/re-encode WebP and resize to 1600 px.
- Admin media list/upload and reference-aware soft removal, transactional audit and file compensation.
- Draft = inactive; scheduled = active with future UTC PublishedAt; published = active with elapsed date.
- Page SEO for five known public routes; article SEO and cover/OG image selectors.
- Dynamic sitemap index, bounded 1000-entry children; robots follows Site:AllowIndexing.
- Article/BreadcrumbList JSON-LD; LegalService only on contact with verified facts and explicit enablement.
- Real MVC/Identity/SQLite/native image tests; optional isolated visual review; MySQL-specific test.

## Acceptance and verification (2026-10-09)

- Release build: 0 errors, 0 warnings.
- Tests: 69 unit + 60 integration passed = 129; 4 dedicated MySQL tests skipped without CEVATBIYIK_TEST_MYSQL.
- NuGet audit including transitive packages: no known vulnerable packages reported.
- CmsFeatures migration and SQL generated/reviewed; no live database update or deployment.
- Mobile review at 390 px: media, article editor and public article filters fit without horizontal overflow.
- Review/rollout: docs/qa/PHASE_06_REVIEW.md and docs/cms/CMS_SETUP.md.

## Remaining roadmap, preserved

Profile editing, arbitrary page content and FAQ CMS were in the original broader guide.
They are deferred to Phase 08 content completion under ADR-019; they are not implemented or closed here.
Phase 07 retains document/private-file storage, retention/recovery and hosting checks.
Phase 09 retains redirects, broader schema/SEO audit and production indexing checks.
Owner approved Phase 06 closure and commit after reviewing 129 passing tests and the 0-error/0-warning build.
Phase 07 is next; implementation does not advance automatically. Database update success was not independently verified here.
