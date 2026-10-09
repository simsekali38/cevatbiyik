# Phase 04 — Public Web UI & Razor Views

## Scope

Owner-requested successor to accepted Phase 03 Application. Phase 00 and ADR-008/012/013/014 remain in force.
Uses original 08-public-site.md's frontend/security/SEO/accessibility rules. Identity/admin, 06-cms.md and remaining FAQ/legal/location pages stay pending.

## Acceptance criteria

- SSR Razor: /, /hakkimda, /makaleler, /makaleler/{slug}, /uzmanlik-alanlari, /uzmanlik-alanlari/{slug}, GET/POST /iletisim.
- Public list/detail queries exclude drafts, future publications, deleted/inactive data and articles under hidden practice areas.
- Article title/excerpt search; category means PracticeAreaId until a real ArticleCategory schema exists.
- Filtering/count/paging in SQL, 12 rows/page; articles ordered by publication date, areas by SortOrder with stable Id tie-breaks.
- Active allowlisted public settings; no unrestricted admin queries in controllers.
- Local Bootstrap 5.3.8 with MIT license, mobile navigation, labels, skip link, focus and server-validation feedback.
- Encoded title/description/canonical/OpenGraph; canonical uses configured HTTPS origin and indexing defaults off.
- Explicit antiforgery in both forms. Search POST → GET redirect keeps tokens out of URL logs.
- Contact POST → server validation → save → TempData/GET redirect; CSRF, rate/request limits and encoded feedback.
- Production collection requires explicit enablement and reviewed non-placeholder notice; no fake lawyer content or email delivery.
- Integration coverage for public 200 text/html routes, hidden detail/404, invalid filter/400, XSS, CSRF, PRG and rate limit/429.
- Build, tests, format and desktop/mobile keyboard checks; environment-dependent skips reported honestly.

## Verification

```powershell
dotnet build CevatBiyik.sln
dotnet test CevatBiyik.sln
dotnet format whitespace CevatBiyik.sln --verify-no-changes --no-restore --include src/CevatBiyik.Web src/CevatBiyik.Application/Interfaces src/CevatBiyik.Application/Services src/CevatBiyik.Application/DTOs src/CevatBiyik.Application/Validation src/CevatBiyik.Infrastructure/Persistence/EfRepository.cs tests/CevatBiyik.UnitTests/PublicQueryTests.cs tests/CevatBiyik.IntegrationTests/PublicWebFactory.cs tests/CevatBiyik.IntegrationTests/PublicUiTests.cs tests/CevatBiyik.IntegrationTests/ContactFlowTests.cs
```

No schema migration is required. PublicWebFactory uses real MVC/Razor/Application with isolated fake repositories.
Two real MySQL tests separately require CEVATBIYIK_TEST_MYSQL. Full WCAG/Lighthouse, Plesk trusted-proxy/HTTPS,
approved factual/legal content and original Identity/admin/CMS/Phase 07 SEO remain launch gates.
