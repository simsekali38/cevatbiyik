# Public homepage refinement — second pass

Date: 2026-10-11
Scope: targeted public homepage/shell visual refinement and owner-requested local JSON configuration.

## Comparison with approved reference

Reviewed PUBLIC_HOME_TRANSFER_REVIEW.md, original 1440/390 screenshots, the selected
`Av. Cevat Bıyık _ Hukuk & Danışmanlık-export (3).png`, and verified existing
`uxmagic-homepage.html`. The reference HTML remains a visual guide, not production code.

- First pass: compact dark hero with an oversized emblem; second pass: 600px desktop
  two-column composition, constrained text measure, two-line lawyer name and a lighter
  ivory brand surface with an unchanged local emblem and navy caption.
- Aligned public containers/header/footer, reduced public mark sizes to 56px (48px
  mobile), and added Ana sayfa with accurate current-page state.
- Unified card spacing, gold border/radius, section rhythm and process-row typography.
  CMS rich text remains complete, with a readable line length rather than clipping.
- Missing content is an honest empty state. Raw CONTENT_REQUIRED values in missing
  contact details and optional CMS summaries are suppressed in this scope. No fabricated
  experience, practice area, biography, office photograph or service guarantee was added.

Existing reviewed introduction/profile and published article/practice links remain;
metadata, canonical, JSON-LD/CSP, consent and the existing protected contact flow remain.
No Admin/inner-page view, domain/application/infrastructure model, migration or package
changes. Public CSS remains scoped so the shared Admin dark header is preserved.

## Local configuration (ADR-034)

Web UserSecretsId was removed. The https profile explicitly uses Development. Runtime
loads appsettings.json then appsettings.Development.json; environment/CLI overrides retain
normal higher priority. The existing base JSON connection was copied privately to the
local Development JSON; no user-secrets value was printed/read/copied and no database
was contacted by this task. Existing hosting JSON remains unchanged.

The real Development JSON stays on disk, is ignored/untracked and excluded from publish.
Only that file was removed from the Git index; pre-existing staged files were preserved.
A credential-free template is under docs/deployment/templates. Old user-secrets files and
IntegrationTests' independent opt-in secrets remain untouched.

```powershell
dotnet run --project src/CevatBiyik.Web --launch-profile https
```

Open https://localhost:7080. Stop/restart the application to apply configuration changes.

## Verification

Final full regression command completed with exit 0:
- Release build: 0 errors / 0 warnings.
- .NET: 211 unit + 205 integration = 416 passed; 11 DB opt-in skips (427 total).
- JS: 6/6 passed.
- Browser: 6 E2E + 5 layout/accessibility + 5 Admin UX = 16/16 passed.
- Asset freshness, format, 8 scanner synthetic self-tests and 4 publish-boundary tests passed.
- All 320/390/768/1440px homepage reflow and Axe checks passed. Four final screenshots
  were visually reviewed against the original desktop/mobile images and the reference.
- git diff --check passed; index identities confirmed all previously staged files unchanged,
  except the explicitly authorized untracking of real Development JSON.

Full verification command (temporary test shell only, not hosting settings):

```powershell
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
pwsh -NoProfile -File tools/test.ps1 -Browser
```

The Development test uses isolated in-memory repositories and verifies local JSON provider,
absence of the legacy secrets provider/assembly attribute, and successful localhost response.
A separate regression keeps public links while hiding unreviewed profile/placeholder summaries.
The first Development fixture attempt needed a test-only PrivateDocument repository for
Development service validation. Import ordering and a hidden-mobile-menu test selector
were corrected before the final rerun; runtime authorization was not weakened.

Original images remain at artifacts/test-runs/home-design-{width}.png, with an additional
preserved copy under home-refinement-before. New images use home-refined-{width}.png.
The fixture contains synthetic TEST_ONLY data and an analytics notice; it is not production
content. Browser audits do not constitute a WCAG certificate or a new Core Web Vitals
measurement. Database opt-in cases are outside this task; no migration was applied.

## Remaining visual differences and approved photograph requirements

The emblem composition is deliberately an interim visual, not a substitute for real
photography. Reference marketing copy was not copied without editorial approval. Published
article links and the actual full footer/legal links are retained, unlike the shorter mockup.
Real CMS text length and number of records will change section heights; missing records do
not become fabricated three-card examples. Missing approach/profile/process facts still
need editorial review in the CMS/approved content workflow.

Supply a rights-cleared, explicitly approved real portrait or actual office photograph:
- Original at least about 1200 x 1400 pixels, without enlargement, watermark or baked-in text.
- Composition that works in an approximately square/portrait desktop half-column and a
  separately checked mobile crop; keep any face/important detail clear of the bottom caption.
- No client names, case documents, identity numbers or other confidential details visible.
- Confirm subject/use approval and describe the image accurately for accessible alt text.
- Prepare a local optimized WebP with explicit dimensions using the existing media limits;
  review loading priority/size when the real hero image is introduced.

No photo, new person/office visual, inner-page redesign, commit or deployment was generated
in this pass. Those require separately authorized follow-up scope.

## Screenshot comparison evidence

Full-page screenshot heights (fixture content, not a performance score):

| Width | First pass height | Refined height |
|---|---|---|
| 320px | 6544px | 6378px |
| 390px | 6078px | 5874px |
| 768px | 4828px | 4937px |
| 1440px | 4081px | 4402px |

Desktop hero now has the intended stronger vertical composition; on phones the brand
placeholder is shorter and missing-contact tokens no longer lengthen the page. The real
consent notice remains visible in screenshots, unlike the design mockup, as required.

## Changed files in this pass

- src/CevatBiyik.Web/Views/Home/Index.cshtml
- src/CevatBiyik.Web/Views/Shared/_Layout.cshtml
- src/CevatBiyik.Web/wwwroot/css/site.css and generated site.min.css
- src/CevatBiyik.Web/CevatBiyik.Web.csproj and Properties/launchSettings.json
- src/CevatBiyik.Web/appsettings.Development.json (local, ignored, untracked, still present)
- src/CevatBiyik.Web/.gitignore (root ignore edits made concurrently were preserved)
- tests/CevatBiyik.BrowserTests/phase14.test.cjs
- tests/CevatBiyik.IntegrationTests/DevelopmentConfigurationTests.cs
- tests/CevatBiyik.IntegrationTests/PublicHomeRefinementTests.cs
- docs/deployment/templates/appsettings.Development.template.json
- docs/ENVIRONMENTS.md, DECISIONS.md (ADR-034), CHANGELOG.md and this review

During final review the root .gitignore rules were removed by a concurrent edit after
our initial update. That edit was preserved. A new Web-scoped .gitignore independently
protects the three real JSON files; git check-ignore confirms the local configuration is
excluded. No credentials were staged. This guard changes no runtime or visual behavior.
