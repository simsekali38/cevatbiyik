# Phase 09 — SEO and redirects

Implementation: 2026-10-10. Owner closure approved (2026-10-10). ADR-022 governs scope; ADR-019/021 are preserved.

## Modules / files

- Domain: RedirectRule; nullable PracticeArea SEO/OG properties.
- Application: RedirectDtos, IRedirectService/IPublicRouteService, RedirectValidator, RedirectService/PublicRouteService;
  ISeoAuditService/SeoAuditService/SeoQuality; existing SeoInput, PracticeArea and media-reference rules.
- Infrastructure: RedirectRuleConfiguration, PracticeArea mappings/relations and SeoRedirects migration.
- Web: SeoPolicy/SeoPresentation, RedirectMiddleware, public filter/layout/sitemap,
  Admin Redirects/Seo controllers/views and existing shared SEO editor/area editor.
- Tests: SeoBusinessTests, SeoEnhancementTests, model constraints and dedicated opt-in MySQL coverage.

## Migration

From repository root; keep connection details in Web user-secrets. Use compatible EF 8.0.29 in artifacts/ef8.

```powershell
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore
.\artifacts\ef8\dotnet-ef.exe migrations script PublicSiteContent SeoRedirects --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release --no-build --output database/SEO_REDIRECTS.sql
```

Validate migration/services with `CEVATBIYIK_TEST_MYSQL` against a dedicated `cevatbiyik_test_*` MySQL 8 database;
see `../database/MIGRATIONS.md`. Never use production credentials/database for tests.
After dedicated validation and a coordinated backup, apply the reviewed migration to the intended local database:

```powershell
.\artifacts\ef8\dotnet-ef.exe database update SeoRedirects --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release
```

The agent did not run database update. Up adds RedirectRules and six nullable PracticeAreas fields, two indexes,
one restricted FK; no existing-data drops/seeds. Down loses redirect/area SEO data. MySQL DDL is not fully atomic.

## Redirect management

1. Open `/admin/redirects`, add a legacy source such as `/eski-yazi` and currently published target.
2. Use only clean root-relative paths. No external URL, query, fragment, wildcard, encoded/Unicode path or protected root.
3. A published source cannot be overridden; fixed public roots cannot be sources. Self loops/chains are rejected.
4. Select active and save. GET/HEAD return 301 directly to the target; source query parameters are discarded.
5. Edit the destination or disable/re-enable the existing record. Source stays reserved and immutable.
6. If target publication is removed, the rule stops redirecting; restore publication or choose another valid target.
   Existing public source content has priority even if an old rule remains in storage.

URLs/route labels are Razor-encoded. Role/CSRF/confirmation and audit transactions protect mutations.
Application-level no-store reduces stale browser caching, but already remembered permanent redirects/search results
may outlive later rule changes. Manual source rules complement existing stable slugs; no automatic slug mutation.

## SEO editor and warnings

`/admin/page-seo`, Article and PracticeArea editors share SeoInput. Area now includes metadata/OG image selection
and bounded media-choice paging. Blank optional fields retain fallback; unsafe HTML/placeholders/control characters
are rejected. Media selected by an area cannot be deleted while referenced, including deleted records.

`/admin/seo` shows indexing/environment/canonical-origin/LegalService settings and bounded content suggestions.
It includes drafts, nine fixed routes and up to 12 articles + 12 areas per page; it is not a full-site crawler.
Inactive fixed-page metadata is treated as fallback. Title checks account for the fallback site-name suffix.
Warnings also appear in the shared editor on page load; save/reopen or use the panel to refresh suggestions.
30–60 title / 70–160 description ranges are project heuristics, not Google rules. Optional OG images need not be supplied.
Keywords have no promised ranking effect. Do not add keyword stuffing, fabricated claims/reviews or doorway pages.
Google may choose different title/snippet text: [titles](https://developers.google.com/search/docs/appearance/title-link),
[snippets](https://developers.google.com/search/docs/appearance/snippet).

## Indexing rollout checklist

- Keep Site:AllowIndexing=false on staging/unfinished content. It wins over page-level policies in meta/headers.
- Review the HTTPS root in Site:BaseUrl; paths, credentials, query/fragment are invalid. Request Host is never trusted.
- Review actual approved profile/legal/FAQ/contact content, published article/area states, canonical and redirects.
- With indexing on, sitemap includes only current canonical publications; no draft/scheduled/deleted/redirect-source/filter URLs.
  Profile/legal/FAQ require reviewed complete content. Contact requires complete visible name/email/phone/address.
- Incomplete contact is noindex; LegalService also requires Site:LegalServiceSchemaEnabled and valid complete visible facts.
  Production contact submission still requires reviewed published KVKK and existing feature/notice/consent gates.
- robots blocks Admin/error paths, links the sitemap when enabled, and disallows all crawling when disabled.
- Existing Article/FAQ/Breadcrumb JSON-LD stays HTML-safe, CSP-nonced and tied to visible content.
- Human review, dedicated MySQL, Plesk/runtime/private-storage and backup gates precede production deployment.
  Indexing is not automatically enabled by this phase and live Search Console was not accessed.

robots/noindex are not access control or an immediate deindex guarantee. For already indexed URLs, robots blocking
can prevent a crawler from observing noindex; use the approved removal workflow and keep staging authenticated.
[Google noindex guidance](https://developers.google.com/search/docs/crawling-indexing/block-indexing).

## Verification

See `../qa/PHASE_09_REVIEW.md`. Optional localhost Razor preview export is the existing CEVATBIYIK_CMS_PREVIEW
test mechanism; it creates ignored TEST_ONLY artifacts and must not be deployed.
