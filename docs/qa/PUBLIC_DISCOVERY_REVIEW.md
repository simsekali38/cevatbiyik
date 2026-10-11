# Sitemap / AI discovery / favicon review — 2026-10-12

## Scope and result

Implemented local source changes only; no deployment, commit, migration, external DB
test or production configuration edit. Existing owner changes, including deleted
design references, are preserved. Discovery routes reuse existing public content
queries and indexed location rules. No new NuGet/npm dependency.

## Live observations

Read-only Edge HTTPS checks of `https://cevatbiyik.aliasyazilim.com`:

| Endpoint | HTTP | Result |
|---|---|---|
| /sitemap.xml | 200 | application/xml; sitemapindex; correct subdomain; three child files |
| /sitemaps/pages.xml | 200 | urlset; 9 URLs |
| /sitemaps/articles/1.xml | 200 | urlset; 12 URLs |
| /sitemaps/practice-areas/1.xml | 200 | urlset; 16 URLs |
| /robots.txt | 200 | text/plain; disallow admin/errors; correct sitemap URL |

Raw response XML was inspected, not the browser viewer's HTML DOM. There is no
evidence of a live sitemap fault at inspection time. The index intentionally links
child files instead of displaying all content URLs. Browser appearance alone does
not establish an XML error. Earlier web/urllib fetches failed; browser HTTPS checks
succeeded without bypassing certificate validation. Checks did not audit legal
approval of live content or prove Google indexing.

## Changes

- `Seo/PublicDiscoveryService.cs`: reviewed known page-route eligibility shared by
  sitemap and AI documents; prevents arbitrary CMS paths entering page discovery.
- `Controllers/SeoController.cs`: explicit UTF-8 XML declaration, shared eligibility,
  no-store; existing origin, chunks, publication/indexing gates retained.
- `Controllers/DiscoveryController.cs`: gated project-specific paginated JSON catalog
  and concise Markdown llms document; public settings only, no private records.
- `Program.cs`: scoped presentation service registration.
- Admin SEO controller/view: effective canonical origin and indexing guidance/links.
- Public/admin layouts and shared `_Favicons.cshtml`: local icon variants; public head
  additionally links the discovery documents.
- `wwwroot/favicon.ico`, brand PNG32/Apple180: faithfully rendered existing SVG.
  ICO sizes 16/32/48/64 verified with Pillow; Apple icon visually inspected.
- `tools/build-favicon.cjs` and `.py`: repeatable local source rasterization.
- `DiscoveryFileTests.cs`: four regressions covering disabled/enabled discovery,
  published/future/deleted/private records, invalid routes, pagination, icon links/assets.
- ADR-038, changelog and `docs/seo/PUBLIC_DISCOVERY.md` explain operation.

## Verification and limitations

Build: Release, 0 warnings / 0 errors. Focused discovery + SEO integration selection:
28 passed. `git diff --check` reports no whitespace errors (Git emits line-ending
conversion notices for some pre-existing files).

Full-suite attempts exposed unrelated working-copy/configuration issues:

1. Local enabled but invalid SMTP configuration prevented host startup. Test-process
   `Smtp__Enabled=false` avoids sending mail and leaves source/server settings intact.
2. `DevelopmentConfigurationTests` fails because the local JSON provider contains
   `AllowedHosts="*"`, whereas the test explicitly requires `localhost`. Environment
   overrides cannot fix this provider-level assertion. No secret JSON was edited.
3. During this task, `SmtpNotificationOptions.cs` was externally changed to remove
   the port-587 validation. Two existing unit cases now fail: ports 465 and 25 are
   expected to be rejected. This unrelated edit is preserved, not claimed fixed.

With test-process wildcard host/official canonical origin/SMTP disabled and the
single local-JSON-dependent test excluded: 209 unit tests passed, 2 unit tests failed;
246 integration tests passed, 11 MySQL/migration tests explicitly skipped.
Therefore the complete .NET suite is NOT green. A narrower AllowedHosts experiment
also rejected fixture-specific proxy hosts; these artificial-host failures are not
evidence of a production sitemap regression.

Logs: ignored `artifacts/test-runs/discovery-*.log`; TRX under each test project's
TestResults. Do not use temporary test filters as a production release acceptance.

Reproduction from `project/` (test process only):

```powershell
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
$env:Smtp__Enabled = 'false'
dotnet test tests/CevatBiyik.IntegrationTests -c Release --no-restore --filter 'FullyQualifiedName~DiscoveryFileTests|FullyQualifiedName~SeoEnhancementTests|FullyQualifiedName~RemainingSeoTests' -m:1 -p:UseSharedCompilation=false
pwsh -NoProfile -File tools/test.ps1 -Browser
```

The focused command passed. The unmodified full script stops on the independent
issues above. For diagnostic continuation only, temporary copies of test.ps1 were
used to exclude the DevelopmentConfiguration test, then to execute the remaining
JS/Python/style/browser stages without re-running .NET. These copies are removed;
the committed runner and existing tests are not weakened. Browser results are
reported separately below, not as a green full-script exit.

Remaining stages completed successfully:

- JavaScript analytics: 6 passed.
- Playwright: 21 passed (6 E2E, 9 responsive/accessibility, 5 admin UX, 1 full CMS).
- Asset parity and 93-record offline seed parity: passed, no external database.
- Secret scanner synthetic self-tests: 8 passed; not a new Git-history secret audit.
- Publish Python checks: 5 passed.
- IDE0011 format verification: passed.
- Focused 28-test SEO/discovery selection was repeated after the external edits:
  28 passed, 0 failures/skips.

Browser continuation log: `artifacts/test-runs/discovery-browser.log`.
Temporary diagnostic runner files were removed after completion. Existing retained
browser evidence was regenerated normally; it was not deleted by the cleanup.

## Safe artifact cleanup

Removed 494 known reproducible preview-host bin/obj files: 112,149,915 logical bytes
(106.95 MiB; not a measured disk-free delta). Retained 17 unknown/source/configuration
files and original preview sources. Releases/rollback, phase14/16 evidence, phase07
publish, EF8 and migration artifacts were not cleanup targets. Unknown root files
remain. New verification output was generated normally after cleanup.

Audit plans/actions are ignored `artifacts/SEO_DISCOVERY_CLEANUP_*20261012.json`.

## Follow-up

Resolve the separate local hostname/SMTP working-copy issues before claiming a fully
green release. Publish this tested feature only through the existing release process;
verify the new URLs and favicon afterwards. Keep indexing disabled on demo content
unless reviewed and approved. No production settings were switched by this task.
