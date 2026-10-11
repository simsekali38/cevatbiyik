# SEO Strategy

## Primary entity

Av. Cevat Bıyık.

## Primary location

Kayseri, Türkiye.

## Search intent

1. Person/brand intent
2. Local service intent
3. Informational legal intent

## Keyword architecture

Do not finalize practice-area keywords until actual services are confirmed.

Potential examples:
- Av. Cevat Bıyık
- Cevat Bıyık Kayseri
- Kayseri avukat
- Kayseri {service} avukatı

## Page architecture

- homepage = personal entity
- profile = identity/trust
- practice areas = service intent
- articles = informational intent
- location/service pages = local commercial intent

## Technical

- canonical
- sitemap
- robots
- metadata
- semantic headings
- schema
- breadcrumbs
- clean URLs
- image optimization
- internal linking

## Local

- Google Business Profile
- NAP consistency
- legitimate citations
- local relevance
- genuine reviews
- relevant backlinks

## Prohibited

- doorway pages
- fake reviews
- fake schema
- keyword stuffing
- hidden text
- mass thin AI pages
- spam backlinks
- fake locations

## Phase 09 implementation

Central SeoPolicy uses configured HTTPS origin rather than request Host. Site:AllowIndexing=false forces
noindex,nofollow metadata/response headers; Admin and error responses remain excluded. Reviewed/public content
controls sitemap inclusion; contact joins only after public contact facts are complete. LegalService retains opt-in.
Practice areas now have SEO/OG and breadcrumbs. Admin SEO checks are bounded, advisory editorial heuristics;
title/description lengths are not Google hard limits. Safe 301 management and automatic regression checks are implemented.
See ADR-022, docs/seo/SEO_SETUP.md and docs/qa/PHASE_09_REVIEW.md.

## Phase 10 local content implementation

One reviewed Kayseri hierarchy with manual links replaces automatic district/service combinations. Bounded local quality checks
withdraw thin/similar/unverified/ancestor-invalid content from indexing and discovery at runtime. Editorial token/similarity limits
are not search-engine thresholds or factual guarantees. Only visible verified contacts/profile feed optional LegalService/Person.
Operational rules and content intake: docs/seo/LOCAL_SEO_SETUP.md and ADR-023.
