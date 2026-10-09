# Phase 06 validation report — 2026-10-09

Owner approved Phase 06 closure and commit after reviewing this report. This is not live MySQL/Plesk/deployment completion.

## Executed evidence

- Release build: 0 errors, 0 warnings.
- 69 unit + 60 integration passed (129); 4 dedicated MySQL tests skipped without a test connection.
- NuGet vulnerable-package audit including transitive dependencies: none reported by configured source.
- EF 8.0.29 CmsFeatures migration/MySQL SQL generated and reviewed: ../database/CMS_FEATURES.sql.
- Existing architecture/model/SQL drift checks included in passing suite.
- 390 px browser review of real Razor snapshots from isolated SQLite/Identity: media, article editor, public filters fit; images load; saved taxonomy/SEO selections populated.
- Temporary snapshot server stopped. No real admin credentials/legal content/developer database used in UI tests.

## Coverage

Taxonomy create/edit/soft-delete/reserved stable slugs; SQL category/tag filters; tag replacement;
validation/no mutation on bad IDs; scheduled detail/sitemap visibility; page metadata activation/uniqueness;
real MVC, Identity, CSRF and audit; media upload/reference guard;
extension/MIME/decode checks, corruption, byte/pixel limits, re-encoding, dimensions, trailing payload removal;
failed media audit rolls back DB/removes file; path traversal rejected;
XML namespaces/content type/index/visibility/trusted origin; disabled indexing/robots;
metadata and JSON-LD encoding, Article/BreadcrumbList, CSP nonce, LegalService visible-fact gate.

Fixed finding: malformed multipart bodies can stop parameter binding in status-code re-execution.
ErrorController now preserves the trusted route status so antiforgery 400 does not become 500.

## Remaining gates

- Dedicated MySQL migration/persistence tests have not run here; SQLite is not MySQL/native hosting proof.
- Migration is additive/seed-free; Down loses CMS data. MySQL DDL is not fully transactional.
- Windows Skia assets, IIS upload config/ACLs/body limit need Plesk validation.
- TestServer lacks server-level body-limit features; service byte/pixel limits tested, Kestrel/IIS limits need hosting validation.
- Filesystem and DB are not atomic: save/audit failures compensated, abrupt termination can leave orphan files.
- Soft-removed media remain public; no confidential documents accepted. Retention/restore/reconciliation is Phase 07.
- Profile/page-content/FAQ CMS remains Phase 08 under ADR-019; redirects/broader SEO remains Phase 09.
- Unknown contact/profile/legal content stays CONTENT_REQUIRED; indexing/LegalService enablement remain false.
- Existing EF 10 tool-manifest/publish-profile edits preserved; migration uses separate EF 8 tool.
- Phase 05 commit: 24ab00a. Phase 06 commit approved by the owner; no automatic next-phase implementation.

Exact commands/routes/deployment checks: ../cms/CMS_SETUP.md.
