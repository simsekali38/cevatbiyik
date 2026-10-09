# Structured Data Policy

Only use schema supported by actual visible content.

Phase 06 Article/BreadcrumbList use encoded JSON-LD/CSP nonce for public article details.
LegalService is contact-only with active visible non-placeholder name/email/phone/address and explicit
Site:LegalServiceSchemaEnabled=true after owner review; default false. No location/ratings/credentials inferred.
Article images use generated public media URLs. Never use UnsafeRelaxedJsonEscaping or raw CMS HTML.

Potential:
- Person
- WebSite
- WebPage
- BreadcrumbList
- Article
- FAQPage
- LegalService / LocalBusiness where accurate

Never:
- fake AggregateRating
- fake Review
- fake awards
- invisible structured content
