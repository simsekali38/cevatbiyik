# Phase 08 verification — 2026-10-10

Status: Implementation verified locally; owner closure approved (2026-10-10). Commit authorized by owner; no deployment/database update performed.

## Scope and architecture

Phase 00 and AGENTS/architecture/security/brand/relevant local skills were reviewed before implementation.
ADR-017 theme/emblem and ADR-019/020 CMS/media/Identity boundaries are preserved; ADR-021 records new decisions.
.NET 8/MySQL 8, linear references, one SSR MVC app, filesystem storage and Plesk constraints are unchanged.
No package/API/subdomain/CDN added. Existing source photos are not public assets. Unknown facts remain CONTENT_REQUIRED.
Accepted .NET 8 support risk remains recorded in ADR-011 and must be revisited before production rollout.

## Automated validation

- Release build: 0 errors, 0 warnings.
- Unit: 102 passed; integration: 83 passed. Total **185 passed**, **6 skipped** dedicated MySQL tests.
- Skips are explicit: no CEVATBIYIK_TEST_MYSQL connection was supplied. New real-provider content test is available.
- New coverage: review/publication gate, placeholders, immutable unique page paths, deactivate/re-enable,
  FAQ order/pagination/soft-delete, unsafe markup rejection, dynamic publication/sitemap and Admin role/CSRF flows.
- Nine public entry routes return HTTP 200/text/html with one H1, skip target, local emblem and legal footer.
- Invalid FAQ paging returns 400; legacy markup stays encoded in Razor and safe parseable JSON-LD.
- Production contact tests cover missing/published reviewed KVKK and direct POST enforcement.
- Nine theme foreground/background checks meet AA text/control thresholds; source-photo/external SVG checks pass.
- PublicSiteContent MySQL SQL/model generation adds three tables/three indexes without seeds or existing-data drops.

Commands:

```powershell
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore --logger "trx;LogFilePrefix=phase08-final"
dotnet format style CevatBiyik.sln --no-restore --diagnostics IDE0011 --verify-no-changes
git diff --check
```

## Browser verification

Real Razor was exported from an isolated test host with TEST_ONLY content, served on localhost and inspected
through the in-app Chromium browser. No real lawyer/legal facts or source photographs entered previews.

390 × 844 viewport: all 14 outputs had scrollWidth = clientWidth = 375 px (15 px vertical scrollbar), no horizontal
overflow: home, profile, FAQ, KVKK, cookies, disclaimer, contact, article list/detail, area list/detail,
and Profile/FAQ/PageContent Admin editors. This is fixture-based responsive baseline evidence, not all-device certification.
1024 px profile also had no overflow and the menu remained collapsed; 1440 px home was visually inspected.
Existing SVG was clear at mobile/header and 120 px profile sizes; desktop header/hero remained brand-consistent.

Actual keyboard input: Tab reached the skip link; Enter focused `#icerik`; Tab reached the first FAQ summary;
Enter expanded/collapsed native details. Summary had visible navy solid focus; header toggler had visible gold focus.
Enter expanded Bootstrap mobile navigation; Tab reached Hakkımda. Accessibility tree exposed expanded/collapsed buttons.
FAQ disclosure works without JavaScript by native HTML semantics; mobile menu uses the existing local Bootstrap bundle.
Temporary viewport was reset and preview servers stopped after review.

Measured actual CSS token pairs:

| Foreground / background | Ratio | Threshold |
|---|---:|---:|
| Navy / paper | 13.96:1 | 4.5:1 |
| Gold ink / paper | 5.48:1 | 4.5:1 |
| Gold ink / white | 5.81:1 | 4.5:1 |
| Muted text / paper | 5.92:1 | 4.5:1 |
| Gold / navy | 7.42:1 | 4.5:1 |
| Control border / white | 4.25:1 | 3:1 |

Local screenshots: phase08-faq-390.png, phase08-profile-390.png, phase08-home-1440.png under
`C:/Users/simse/.codex/visualizations/2026/10/09/01a121da-c4d5-74b2-8918-d3d3c259dba8/`.
These show TEST_ONLY fixtures and are review evidence, not publishable site content.

## Remaining rollout / human checks

- Dedicated MySQL 8 migration/persistence validation and local database update; see PUBLIC_CONTENT_SETUP.md.
- Lawyer-approved biography/bar/vision/legal/FAQ texts, actual contact settings and explicit human publication review.
- Real screen-reader/device/full WCAG audit, stress content and performance audit remain Phase 14 scope.
- Existing Windows/Plesk/native/Defender/ACL/backup restore gates remain Phase 07/15 rollout requirements.
- FAQPage does not promise Google rich results; broader indexing/redirect work remains Phase 09.
- Concurrent user-owned appsettings.json edits were preserved and excluded from Phase 08 implementation.
