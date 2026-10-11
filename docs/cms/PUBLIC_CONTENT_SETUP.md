# Phase 08 — Public content CMS

Implementation date: 2026-10-10. Owner phase closure approved (2026-10-10). ADR-021 governs this module.

## Files and modules

- Domain: `Entities/LawyerProfile.cs`, `PageContent.cs`, `FaqItem.cs`.
- Application: `DTOs/PublicContentDtos.cs`, `Interfaces/IPublicContent.cs`, three content services,
  `Validation/PublicContentValidators.cs` and DI registrations.
- Infrastructure: three Fluent configurations, AppDbContext sets and PublicSiteContent migration/snapshot.
- Web: `/admin/profile`, `/admin/page-contents`, `/admin/faq`, public FAQ/legal controllers and Razor views.
- Shared layout/CSS: existing navy/gold theme, SVG, mobile navigation, dynamic footer, focus and native FAQ.
- Tests: PublicContentTests/PublicDesignTests/PublicContentFlowTests and opt-in MySQL coverage.

## Database preparation

Run from the repository root. Keep the connection in Web user-secrets; never paste credentials into tracked files.
Use EF 8.0.29 from `artifacts/ef8`, preserving the separately managed tool manifests.

```powershell
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore
.\artifacts\ef8\dotnet-ef.exe migrations script MediaFiles PublicSiteContent --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release --no-build --output database/PUBLIC_SITE_CONTENT.sql
```

Review SQL and validate on a dedicated MySQL 8 test database whose name starts with `cevatbiyik_test_`.
Use `database/MIGRATIONS.md` for the environment-variable test connection procedure. Never use production
credentials for tests. SQL/model/SQLite checks do not certify a real MySQL update.

After dedicated validation and a coordinated backup, apply the reviewed migration to the intended local database:

```powershell
.\artifacts\ef8\dotnet-ef.exe database update PublicSiteContent --project project/src/CevatBiyik.Infrastructure --startup-project project/src/CevatBiyik.Web --configuration Release
```

The agent did not execute this database update. MySQL DDL can commit implicitly; the SQL transaction wrapper
does not guarantee rollback of every schema command. Down drops these three tables and loses their content.
Production deployment requires the existing Plesk, private-storage and backup/restore gates too.

## Admin usage

1. Open **Profil**. Enter confirmed biography, vision and bar association. There is no identity/registry field.
2. Open **Sayfa içerikleri**. Choose homepage introduction, KVKK/aydınlatma, cookie policy or disclaimer.
   Existing paths cannot be renamed. Use **Devre dışı bırak** to remove a page from publication; edit/re-enable it later.
3. Open **SSS**. Create a question/answer and order number. Public/Admin lists use 12 rows per page; deletion is soft.
4. Keep incomplete work inactive. `CONTENT_REQUIRED` is allowed in drafts. HTML/entity-encoded markup is rejected.
5. Review the facts/legal text and select the review checkbox plus publication checkbox before saving.
   The review checkbox is deliberately unchecked on every edit, including already published content.
6. Verify the public output. Plain text preserves line breaks, uses Razor encoding and does not execute HTML.
7. Use existing **Sayfa SEO** for public metadata, now including `/sss` and the three legal routes.

No biography, legal text, FAQ, contact facts or credentials are seeded. Test previews contain **TEST_ONLY** fixtures
from isolated SQLite/in-memory test hosts; they must never become production content.

## Public behavior

`/hakkimda` displays reviewed profile/vision/bar information. `/sss` uses native accessible disclosure,
visible focus and paging; FAQPage JSON-LD uses only the current page's visible reviewed items.
Legal routes show reviewed text or CONTENT_REQUIRED. Empty profile/legal/FAQ pages are noindex and excluded
from sitemap. FAQ pagination/filter state stays noindex under the existing SEO rules.
Dynamic footer contact values are read from existing public settings; mailto/tel links require valid values.
The existing consent-notice and contact feature gates remain; Production additionally requires published reviewed KVKK.
Disable KVKK publication and production form submission stops with HTTP 503, including direct POST attempts.

Google FAQ rich results are limited to authoritative government/health sites; this legal site's schema does not
guarantee such a result. [Google announcement](https://developers.google.com/search/blog/2023/08/howto-faq-changes).

## Local visual review

Optional isolated Razor export (no real database or lawyer facts):

```powershell
$env:CEVATBIYIK_CMS_PREVIEW = '1'
try {
    dotnet test project/tests/CevatBiyik.IntegrationTests -c Release --filter FullyQualifiedName~AdminCms_PublishesReviewedContent
} finally {
    Remove-Item Env:CEVATBIYIK_CMS_PREVIEW
}
```

HTML/local assets go to ignored `artifacts/cms-preview`. Serve that directory on localhost for visual testing;
do not deploy it. Public links use application routes, so standalone HTML previews are for rendering/keyboard checks,
while HTTP integration tests verify the actual routes. Evidence: `docs/qa/PHASE_08_REVIEW.md`.
