# Phase 10 — Kayseri / local SEO

Implemented and owner-approved 2026-10-10. ADR-023 extends ADR-019/021/022 without replacing them.

## Modules / file paths

- Domain: project/src/CevatBiyik.Domain/Entities/Location.cs (LocationKind, Location and two association entities).
- Application: DTOs/LocationDtos.cs, Interfaces/ILocationService.cs, Services/LocationService.cs,
  Services/LocalContentQuality.cs, Validation/LocationValidator.cs; DI and media/reference/public-route policies.
- Infrastructure: Persistence/Configurations/LocationConfiguration.cs, AppDbContext/EfRepository,
  Persistence/Migrations/20261009224849_LocalSeo.cs + designer/snapshot.
- Web: Controllers/LocationsController.cs; Areas/Admin/Controllers/LocationsController.cs;
  Views/Locations/{Index,Detail}.cshtml and Admin counterparts; Shared/_RelatedLocations.cshtml;
  existing article/area controllers/details, sitemap, SeoPresentation and profile schema integration.
- Tests: UnitTests/LocalSeoTests.cs, IntegrationTests/LocalSeoFlowTests.cs, model and opt-in MySQL assertions.

## Editorial workflow

1. Open /admin/locations. Create Kayseri as İl without a parent. It starts as an unpublished CONTENT_REQUIRED draft.
2. Draft an original useful overview and verifiable local explanation. Do not invent office, services, court procedure,
   success claims, credentials, district coverage, addresses or opening hours. Collect sources through content intake.
3. Save a draft first if more than the first 20 related content/media choices are needed; edit and use choice pagination.
   Save before changing choice pages. Selected IDs outside the current page remain explicit removable selections.
4. Select at most 20 active practice areas and 20 published articles relevant to this location. This is a manual relevance
   decision; no automatic district × service page generation. Choose optional SEO/OG through the shared editor.
5. Check human review and fact verification only after confirming both legal accuracy and useful local value.
   Publication requires review and no placeholders; factual verification/quality determine indexability separately.
6. Reopen the editor/list to inspect warnings. Filling checkboxes does not override the automatic quality policy.
7. Add districts under Kayseri and regions under a district only when distinct useful approved content exists.
   All ancestors must pass indexability. Slug/type/parent are stable; existing 301 management handles legacy URLs.
8. Delete requires explicit confirmation. Locations are soft-deleted; children must be removed first. Association edits
   remove association rows, not the related articles/areas. Existing reserved slugs cannot be reused by a new location.

## Indexing safeguards

At most 100 nondeleted local records (drafts included); bounded comparisons are intentionally suitable for this project.
Body: 150 tokens/60 distinct; local details: 40 tokens/25 distinct. Turkish normalization and name removal compare
five-word shingles; >=75% overlap of the smaller set marks both active reviewed records similar, including padded copies.
These are editorial heuristics, not Google minimums or semantic/factual guarantees. Human review must reject formulaic,
paraphrased, keyword-stuffed or useless content even when the heuristic passes. Never mass-fill text to reach a count.

Draft/unreviewed/deleted locations: 404. Reviewed published but weak/unverified locations: encoded readable 200 with
noindex,follow. Global Site:AllowIndexing=false remains noindex,nofollow. Weak content has no location discovery,
related backlinks, hierarchy links or sitemap entry. Removal of related publication/ancestors withdraws eligibility.
Noindex is not privacy protection; do not publish confidential content in a local page.

Routes: /konumlar and /konumlar/{slug}; child sitemap: /sitemaps/locations.xml. No eligible locals means absent child
sitemap, no fixed-page sitemap location entry and noindex listing. Parent/child/service/article links use current public data.

## Schema / verified local signals

Existing Site:LegalServiceSchemaEnabled stays opt-in and requires complete syntactically valid visible contact facts.
LegalService is a LocalBusiness subtype. Local pages add only reviewed verified areaServed, reuse the same office,
and never substitute a district for office address. Contact data lives in existing public site settings and remains visible.

Person on /hakkimda requires published reviewed profile, complete public contacts and separate Site:PersonSchemaEnabled.
Default is false; no appsettings/secrets change was made. Configure only after owner-approved intake, e.g. environment
variable Site__PersonSchemaEnabled=true in the intended environment. Do not enable unknown facts or indexing here.
No fake rating/review, credentials, hours, geocoordinates or private registry/identity fields. Safe JSON-LD/CSP remains shared.

## Migration and verification

```powershell
dotnet build -c Release --no-restore -m:1 -p:UseSharedCompilation=false
dotnet test -c Release --no-restore -m:1 -p:UseSharedCompilation=false
.\artifacts\ef8\dotnet-ef.exe migrations script SeoRedirects LocalSeo --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release --no-build --output database/LOCAL_SEO.sql
```

Reviewed SQL adds three empty tables, eight indexes and six FKs; no seeds or destructive Up. Body longtext and details
text avoid utf8mb4 varchar row-size overflow. Down loses local content/association data; MySQL DDL is not fully atomic.
Validate first with CEVATBIYIK_TEST_MYSQL against a dedicated cevatbiyik_test_* database (never cevodb/production).
Back up DB and media/files together before applying reviewed migration to the intended local environment:

```powershell
.\artifacts\ef8\dotnet-ef.exe database update LocalSeo --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release
```

Phase 10 closure and commit were authorized by the owner. The agent did not run database update, deploy, or enable schema/indexing flags. User-owned appsettings
edits are preserved. Tests/review details: ../qa/PHASE_10_REVIEW.md. Phase 11 is not started.

Sources: [Google doorway/scaled spam policies](https://developers.google.com/search/docs/essentials/spam-policies#doorways),
[LegalService](https://schema.org/LegalService), [Person](https://schema.org/Person).
