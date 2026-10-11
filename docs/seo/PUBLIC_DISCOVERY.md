# Public discovery files

The existing MVC application serves `/sitemap.xml`, `/robots.txt`, `/llms.txt` and
`/ai-catalog.json`. No database migration or additional package is required.

## Sitemap and environment settings

`/sitemap.xml` is a sitemap index, not a flat list of every page. Follow its `<loc>`
links to `/sitemaps/pages.xml`, `/sitemaps/articles/1.xml`,
`/sitemaps/practice-areas/1.xml` and, when eligible, `/sitemaps/locations.xml`.
Article and practice-area chunks contain at most 1,000 entries each.
An XML file need not look like a designed HTML page in a browser.

All absolute links use `Site:BaseUrl`. A deployment whose canonical address is the
temporary subdomain should use `https://cevatbiyik.aliasyazilim.com`; the final
deployment should use its verified official domain. Request Host does not override
this setting. Admin → SEO shows the effective canonical origin.

`Site:AllowIndexing=false` deliberately returns an empty sitemap, disables child
sitemaps, returns an empty disabled AI catalog and a minimal llms notice, and makes
robots disallow crawling. Enable only after content/legal review and launch approval.
Demo data is not publication approval. This change does not alter server settings.

Public page discovery shares existing profile/legal/FAQ review gates and accepts only
known CMS page routes. Article/practice-area queries reuse existing active, deletion,
publication-date and related-area filters. Location URLs use existing indexability
rules. Admin paths, messages, private documents and private settings are excluded.
Responses are no-store; no persistent generated file needs manual refreshing.

## AI documents

`llms.txt` is a concise Markdown directory: site name/description from public CMS
settings, available pages, up to ten working areas and five recent published articles,
and links to the complete catalog, sitemap and robots. It does not manufacture `.md`
versions of HTML pages. The [llms.txt proposal](https://llmstxt.org/) is a convention,
not a guarantee of indexing, ranking or AI citation.

`ai-catalog.json` is a project-defined format (`schema_version: "1.0"`). Fields include
`enabled`, `language`, `name`, `description`, `base_url`, `sitemap`, `llms`, `page`,
`total_pages`, `next`, `content_scope`, `legal_notice`, and `resources`.
Resource entries carry `kind`, `url`, `updated`, and a title where available.
No private content bodies or personal contact messages are exposed.

Use `?page=2` etc. Each page contains up to 1,000 article and 1,000 practice-area URLs;
static page/location entries appear on the first page. Follow `next` until null.
Invalid page values return 400; pages beyond the enabled catalog return 404.
This file is not an external standardized AI API. Existing robots/noindex policies
remain necessary and are not access control for sensitive information.

## Brand icons

Source remains `project/src/CevatBiyik.Web/wwwroot/images/brand/cevat-biyik-emblem.svg`.
Shared `_Favicons.cshtml` supplies SVG, ICO (16/32/48/64), PNG (32), Apple PNG (180)
for both layouts. Rebuild from `project/` with `node tools/build-favicon.cjs` using
the already installed Playwright/Edge and Python/Pillow. No source card photo is used.

## Artifact cleanup

Only orphan `artifacts/cms-preview-host/bin` and `obj` reproducible outputs were
removed: 494 files, 112,149,915 logical bytes (106.95 MiB). Seventeen unknown/source/
configuration files in those output directories were retained, as were original
preview sources. Releases/rollback packages, phase14/16 evidence, phase07 publish,
EF8 tooling and migration artifacts remain. Unknown root scratch files were not
deleted merely because they appear unused. New regression evidence remains too.
Ignored JSON action/plan records are under artifacts with prefix
`SEO_DISCOVERY_CLEANUP_` and date `20261012`.

## Deployment verification

After publishing the tested application, inspect all four discovery URLs and one
child sitemap, compare canonical domain with deployment intent, and inspect favicon
in a fresh browser session. This task does not publish or change production settings.
