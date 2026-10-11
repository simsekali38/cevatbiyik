# Public layout and homepage — design transfer review

Date: 2026-10-11
Scope: shared public layout, homepage, public CSS and targeted browser regression.

## References and implementation

The owner explicitly selected `Av. Cevat Bıyık _ Hukuk & Danışmanlık-export (3).png`.
The existing HTML was verified as `design/uxmagic-homepage.html`; its content was
used as visual reference only. Inner PNGs were reviewed for consistency, not implemented.

Implemented light public navigation, thin navy information strip, two-column hero,
paper/beige sections, restrained gold borders/shadows, approach cards, practice areas,
reviewed profile, process placeholder, existing latest articles and contact CTA.
Mobile hero becomes one column below Bootstrap's lg breakpoint. The unchanged local
SVG retains its aspect ratio and dimensions; no external image/font/icon/CDN was added.

Public header rules are scoped with `.public-site`; Admin shares site.css and retains
its existing dark header. No Admin view/style, inner-page view, controller, application,
domain, persistence, package or migration change was made in this task.
Metadata, canonical, OpenGraph, JSON-LD/nonce, consent and real form submission remain
in their existing implementations. Profile and homepage introduction reuse only public
CMS projections; the latest article section preserves access to existing content.

## Content gates

No approved local hero photograph was found. The visual column uses the local emblem
and an explicit CONTENT_REQUIRED notice, not the reference's remotely hosted photograph.
Missing approach/profile/process facts remain CONTENT_REQUIRED. The three process rows
explain website navigation, not an invented lawyer workflow or service guarantee.
The contact card links to the existing validated form instead of introducing a mock form.

## Verification

Final `project/tools/test.ps1 -Browser` run completed successfully (exit 0):
- Release build: 0 errors, 0 warnings.
- .NET: 211 unit + 203 integration = 414 passed; 11 database opt-in skips.
- JavaScript: 6/6 passed.
- Browser: 6 E2E + 5 accessibility/layout + 5 Admin UX = 16/16 passed.
- All four homepage widths passed overflow/local asset/navigation/Axe checks.
- Generated asset check, format verification, 8 secret scanner synthetic self-tests,
  and 4 publish verifier self-tests passed.
- `git diff --check` passed. Existing staged changes were preserved; new work is unstaged.

The first restricted test run could not connect to testhost. A subsequent run encountered
existing hosting-specific AllowedHosts/canonical values. Files were left untouched; the
final command uses process-only fixture settings, not deployable production overrides:

```powershell
# In a temporary PowerShell session; these values must not be copied to hosting JSON.
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
pwsh -NoProfile -File project/tools/test.ps1 -Browser
```

The initial browser pass caught shared header CSS affecting Admin menu contrast. Rules
were scoped to public markup and the full suite was rerun. No Admin design was changed.
Four-width screenshots contain explicitly synthetic fixture records, not production data:
`artifacts/test-runs/home-design-{320,390,768,1440}.png`.
All four screenshots were visually inspected for logo proportions, hero reflow, spacing,
card alignment and footer. Automated checks cover overflow, navigation, local assets,
single heading/landmarks and Axe AA rules. This is not a WCAG certification or a new
Core Web Vitals/Lighthouse measurement. Database opt-in cases were intentionally omitted.

## Remaining work outside this scope

Supply approved hero photography and approved approach/process text. Review the completed
public design with actual approved CMS content. Article/practice/detail/about redesign,
profile-photo CMS fields and related FAQ/article features belong to separately authorized
follow-up work. No commit, database update or deployment was performed.
