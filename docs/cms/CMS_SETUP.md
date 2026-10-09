# Phase 06 — CMS usage and local rollout

## Files and boundaries

| Layer | Files |
|---|---|
| Domain | Entities/ArticleCategory.cs, ArticleTag.cs, ArticleTagLink.cs, MediaAsset.cs, PageSeo.cs; Article.cs CMS fields |
| Application | DTOs/CmsDtos.cs; Interfaces/ICmsServices.cs, ISitemapService.cs; Services/TaxonomyService.cs, MediaService.cs, PageSeoService.cs; ArticleService.cs; Validation/CmsValidators.cs |
| Infrastructure | Persistence/Configurations/CmsConfigurations.cs; Persistence/SitemapService.cs; Media/FileImageStorage.cs; CmsFeatures migration |
| Web | Areas/Admin/Controllers/{Taxonomy,Media,PageSeo}Controller.cs; admin Razor views/editor template; Seo/; Controllers/SeoController.cs; public article filters/detail/layout |
| Tests | CmsBusinessTests.cs, CmsFlowTests.cs, ImageStorageTests.cs; CMS MySQL test and isolated factories |

References remain Web → Infrastructure → Application → Domain. No API, SPA, subdomain or second app.

## Local migration

Phase 05 commit: `24ab00a`. Phase 06 migration already generated: `20261009202119_CmsFeatures`.
Do not run migrations add again. Review ../database/CMS_FEATURES.sql and back up the intended database.
SQL adds 8 nullable article columns, 5 tables, 9 indexes and 6 foreign keys; only migration history is inserted.
MySQL DDL can implicitly commit despite the transaction wrapper. Down deletes CMS data; avoid rollback without a recovery plan.

Local tool manifests contain EF 10 edits outside this phase's changes; the app remains EF/.NET 8.
This run used a separate EF 8.0.29 tool in ignored artifacts/ef8, preserving those edits.
On a fresh checkout install once; skip installation when the tool exists:

```powershell
dotnet tool install dotnet-ef --version 8.0.29 --tool-path artifacts/ef8
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet build -c Release
dotnet test -c Release
.\artifacts\ef8\dotnet-ef.exe migrations has-pending-model-changes --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web
```

Configure the local connection if not already set; replace placeholders locally, never in tracked files:

```powershell
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Server=CONTENT_REQUIRED;Port=3306;Database=cevodb;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;' --project src/CevatBiyik.Web
```

First validate on a dedicated MySQL 8 test database (name must start cevatbiyik_test_):

```powershell
$env:CEVATBIYIK_TEST_MYSQL = 'Server=CONTENT_REQUIRED;Database=cevatbiyik_test_phase06;User ID=CONTENT_REQUIRED;Password=CONTENT_REQUIRED;'
dotnet test tests/CevatBiyik.IntegrationTests --filter FullyQualifiedName~MySqlPersistenceTests
Remove-Item Env:CEVATBIYIK_TEST_MYSQL
```

After review/test, apply to the intended local development database:

```powershell
.\artifacts\ef8\dotnet-ef.exe database update CmsFeatures --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web
```

This applies missing preceding IdentityAdmin too. First real administrator setup: ../security/ADMIN_SETUP.md.
No automatic Migrate/EnsureCreated in app startup. The agent did not update cevodb or deploy.

## Editor workflow

- `/admin/taxonomy/categories` and `/admin/taxonomy/tags`: create/edit/soft-delete, stable slugs, activation.
- `/admin/media`: JPEG/PNG/WebP upload, required alt text, public files, 20-row paging.
- Article editor: category, up to 20 tags, cover, SEO. Choices page in groups of 20, preserving saved selections; save before changing choice pages.
- Inactive = draft; active + future UTC date = scheduled; active + elapsed UTC date = published. Visibility changes on read without cron/worker.
- Public filters: legacy `category` = PracticeAreaId; `topic` = ArticleCategoryId; `tag` = tag slug. Empty filters show all visible articles.
- `/admin/page-seo`: metadata for `/`, `/hakkimda`, `/iletisim`, `/makaleler`, `/uzmanlik-alanlari`; disable/re-enable records.
- Blank SEO uses fallbacks; article OG image falls back to cover. Keywords are optional, not a ranking promise.

Writes require Admin role, antiforgery, explicit validation and transactional audit. Deletions require confirmation.
Deleted/inactive taxonomy labels are hidden; articles keep existing practice-area/publication visibility rules.
Media referenced by articles/SEO, including deleted records, cannot be removed. Unreferenced media are soft removed; files remain public for old URLs.
Retention/recovery/orphans and private documents remain Phase 07 work. Back up database and uploads together.

## Plesk/filesystem safety

Only `/uploads/images/{32 lowercase hex}.webp` is served by the app. Client filenames are discarded; originals never reach wwwroot.
IIS defense in depth: tracked wwwroot/uploads/web.config permits only `.webp` and GET/HEAD, disables directory browsing.
Create uploads/images; grant application-pool Modify there only, Read on parent/config, no execution permission.
Check delegated requestFiltering with hosting: locked sections can cause IIS 500.19. Do not silently remove protections.
IIS request limit must allow at least 6 MiB; MVC upload limit is 6 MiB, file limit 5 MiB. Other admin actions keep 256 KiB.
Verify native Windows Skia assets and published runtime architecture. Preserve uploads on publish, never mirror-delete them.
Check real WebP GET and prohibited extension/verb denial on Plesk before launch.

## SEO gates

Site:BaseUrl is trusted HTTPS origin, never request Host. Site:AllowIndexing remains false:
robots disallows all; sitemap is empty; content remains noindex.
When true, /sitemap.xml points to /sitemaps/pages.xml and 1000-entry article/practice-area children.
Entries are active/nondeleted/public; future/draft articles and articles attached to hidden practice areas are excluded.
Static sitemap includes home and two content lists. Unknown profile/contact/legal pages remain omitted until Phase 08/09 review.
Robots is not access control; /admin stays authenticated, noindex/no-store.

Article/breadcrumb JSON-LD represents public details. LegalService appears only on contact when active non-placeholder
name/email/phone/address are visible and Site:LegalServiceSchemaEnabled=true is explicitly set after owner review. Default false.
No other profession/location/rating facts are inferred. Do not enable indexing/schema to bypass missing content review.

## Optional visual review

```powershell
$env:CEVATBIYIK_CMS_PREVIEW = '1'
dotnet test tests/CevatBiyik.IntegrationTests --filter 'FullyQualifiedName~TaxonomyAndArticleSeo|FullyQualifiedName~MediaUpload'
Remove-Item Env:CEVATBIYIK_CMS_PREVIEW
python -m http.server 5086 --bind 127.0.0.1 --directory artifacts/cms-preview
```

Open media.html, article-editor.html, page-seo.html, categories.html and articles.html locally.
Snapshots use real Razor and test fixtures; links/forms are not a live admin app. Never publish artifacts.
