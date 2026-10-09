# Phase 09 verification — 2026-10-10

Status: Implemented/locally verified, owner closure approved (2026-10-10). Commit authorized by owner; no live database update, deployment or indexing activation.

Phase 00/AGENTS/main architecture/manifest/ADR-019/021 and relevant local SEO/backend/security/database/testing/
architecture/Admin/frontend skills were reviewed. Keep .NET 8/MySQL 8/Windows-Plesk, linear references, one Razor app
and filesystem storage. No new package/API/subdomain/CDN; accepted runtime/hosting risks remain rollout gates.

## Changes

RedirectRule and secure local publication-aware 301 management; GET/HEAD only, query discard, source priority,
no protected routes/external URLs/chains/loops, immutable reserved source and audited disable/re-enable.
PracticeArea SEO/OG and restricted media references; safe metadata validation/rendering and BreadcrumbList.
Bounded Admin SEO panel/shared-editor suggestions. Central origin/canonical and global indexing precedence.
Contact completeness aligns sitemap/noindex/LegalService while preserving separate production submission gates.
Existing FAQ/Article JSON-LD encoder/CSP, reviewed publication, sitemap batches and robots are reused.

## Automated results

- Release build: 0 errors / 0 warnings.
- Unit: **124 passed**. Integration: **104 passed**. Total **228 passed**, **7 MySQL tests skipped**.
- No dedicated CEVATBIYIK_TEST_MYSQL connection was supplied; the new real MySQL test is available, not claimed run.
- Covered unsafe/ambiguous paths, self loops/chains, unique sources, immutable edits, deactivate/re-enable,
  stale/inactive/draft/scheduled targets, current content precedence, GET/HEAD 301 and POST non-redirection.
- Admin authorization/CSRF/confirmation, real SQLite Identity/repositories, redirect audit transaction rollback.
- Metadata HTML/placeholders/control characters, informational suggestions, stable area slug and media-reference safety.
- Global indexing wins in meta/headers; invalid origins fall back independently of request Host.
- Sitemap/robots canonical publication coverage, contact completion gate, Article/Breadcrumb/FAQ safe JSON regressions.
- Existing Phase 00–08 tests pass. SeoRedirects SQL/model constraints verified without applying to cevodb.
- Required IDE0011 format verification and git diff whitespace check pass.

Commands:

```powershell
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore --logger "trx;LogFilePrefix=phase09-final"
dotnet format style CevatBiyik.sln --no-restore --diagnostics IDE0011 --verify-no-changes
.\artifacts\ef8\dotnet-ef.exe migrations script PublicSiteContent SeoRedirects --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release --no-build --output docs/database/SEO_REDIRECTS.sql
git diff --check
```

One initial redirect test caught an incorrect fixed substring offset; using the actual route-prefix length fixed it.
The final test suite passes. No failing test was removed or weakened to hide the problem.

## Local visual check

Actual Razor from isolated TEST_ONLY fixtures exported to ignored artifacts/cms-preview and served on localhost.
390×844: SEO panel, redirect editor and area SEO editor had document scrollWidth=clientWidth=375 (vertical scrollbar),
without page-level horizontal overflow. Responsive table scroll stays within its wrapper. Desktop panel inspected at 1440×900.
Existing navy/gold/emblem and readable labeled server-rendered forms remain. Temporary viewport reset/server stopped.
Screenshots `phase09-redirect-390.png`, `phase09-seo-panel-1440.png` are under
`C:/Users/simse/.codex/visualizations/2026/10/09/01a121da-c4d5-74b2-8918-d3d3c259dba8/`.
These are test evidence, not production content. Actual HTTP flows are tested against the application test server.

## Remaining rollout/human checks

Dedicated MySQL 8 migration/persistence validation, reviewed local database update, current lawyer-approved content,
production domain/runtime/Plesk/ACL/Defender/backups and real Search Console/rich-result review remain operator gates.
No SEO ranking/indexing guarantee or full WCAG certification. FAQ rich-result limitations from ADR-021 remain.
Root card photographs and personal identity fields are untouched. User-owned appsettings.json edits are preserved.
Phase 10 local SEO is not started. Setup and constraints: `../seo/SEO_SETUP.md` and ADR-022.
