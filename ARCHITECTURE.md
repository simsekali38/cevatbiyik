# Architecture

## Phase 12 Web security boundary

SecurityHeadersMiddleware owns final-response baseline/nonce and ConsentCspMiddleware performs legal/consent reads inside
exception handling. Explicit proxy trust and pre-static throttling stay in Web; Application/Domain dependencies unchanged.
Private attachments retain stricter CSP. No schema/package change. ADR-025 and SECURITY_HARDENING describe configuration,
hosting gates and unresolved tracked/historical credential finding; no production readiness claim from local tests.

## Phase 08 public content boundary

LawyerProfile/PageContent/FaqItem add review-gated content through Application ports and existing EF repositories.
One fixed profile and four immutable page paths avoid a general-purpose route CMS. FAQ reads are paged at 12;
layout uses bounded public projections. Admin Areas reuse Identity/CSRF/audit transactions. Public Razor always
encodes plain text; native FAQ disclosure needs no JavaScript. Existing brand SVG/Bootstrap/media remain shared.
No content seeds, sensitive profile fields, new dependency, API or project reference. See ADR-021.

## Phase 07 file boundary

PrivateDocument is distinct from public MediaAsset. Application defines IDocumentStorage, IMalwareScanner,
IFileCatalog/IManagedFiles and use cases; Infrastructure implements private filesystem storage, Defender and bounded EF projections.
Web owns Admin document/reconciliation Razor screens and protected attachment endpoints. See ADR-020 and docs/media/MEDIA_FILES.md.
Soft deletion retains files; recovery verifies contents. Old public image URLs remain public. Private roots are outside wwwroot.
Quarantine operations record audit intent and are not distributed transactions. DB + both storage trees require one frozen backup batch.
No new package, API, app, subdomain or framework change.

## Phase 06 CMS and SEO boundary

Domain owns taxonomy/join/media/page-SEO entities. Application owns CMS DTOs/validators/use cases and
IImageStorage/ISitemapService ports. Infrastructure supplies EF, bounded scalar sitemap projections and
SkiaSharp filesystem re-encoding. Web owns protected Razor modules, metadata/XML/robots and HTML-safe JSON-LD.
No new project references, API or background publication worker. DB/audit share a transaction;
filesystem writes are compensated, not distributed-transaction atomic. See ADR-019 and docs/cms/CMS_SETUP.md.
Generic reads remain bounded; dedicated scalar sitemap batches are capped at 1000.

## Phase 05 Identity and admin boundary

Infrastructure owns ApplicationUser/Identity EF stores in the shared AppDbContext.
Application defines IAdminAuthenticationService, IAuditLogService, IAdminOperation and
dashboard services without referencing ASP.NET Identity. Web owns cookie/policy configuration,
Admin-area authorization convention and Razor controllers/views. Linear references are preserved.
Core mutations reuse Application services and commit with audit in one DB transaction.
See ADR-018 and docs/security/ADMIN_SETUP.md; no second app, API or subdomain.

## Overview

A modular monolithic ASP.NET Core MVC application.

### Layers

Project references: `Web -> Infrastructure -> Application -> Domain` (ADR-012).

`Infrastructure` implements persistence and external integrations and is consumed through application abstractions.

## Projects

### CevatBiyik.Domain

Pure domain concepts.

### CevatBiyik.Application

Use cases, DTOs, validation, service abstractions.

Phase 03 provides Article, PracticeArea, Contact and Setting services through Application interfaces.
FluentValidation runs explicitly in write services. Application owns IRepository<T> and ISlugService;
Infrastructure supplies EfRepository<T> against the scoped AppDbContext. No EF types are used by Application.
Read pages are bounded to 100 records. Soft deletion is implemented by business services, not physical Remove calls.
Public article queries are explicit; generic Get/List operations are reserved for authenticated admin entry points.

### CevatBiyik.Infrastructure

EF Core, MySQL, Identity persistence, filesystem storage, email, external services.

### CevatBiyik.Web

MVC controllers, Razor, admin area, middleware, static assets.

## Why MVC/Razor?

Phase 04 public controllers use public Application projections, never unrestricted admin Get/List methods.
PublicSiteFilter loads active allowlisted settings; the error layout uses static fallback without persistence access.
Typed Web models support homepage, filters/pagination and contact. All content is plain text/Razor-encoded.
Bootstrap assets live in wwwroot/lib/bootstrap; no runtime CDN dependency is required.

- SEO friendly
- low JavaScript requirement
- excellent fit for content-heavy site
- simple Plesk deployment
- one application
- simple authentication
- easy caching

## Admin

`/admin` via ASP.NET Core Areas.

## API

Not required initially.

Future API can live at `/api/v1/` if required.

## Phase 09 implemented SEO

Application owns RedirectService, PublicRouteService and bounded SeoAuditService; Web owns HTTP middleware,
central SeoPolicy and Razor Admin screens. Infrastructure persists RedirectRule and optional PracticeArea SEO.
Existing metadata/JSON-LD/sitemap services are reused. No new dependency/API/application was introduced.
Decision and operating limits: ADR-022 and docs/seo/SEO_SETUP.md.

## Phase 10 local content boundary

Application owns bounded LocationService and LocalContentQuality; Domain owns Kayseri hierarchy and manual content associations.
Infrastructure persists three tables with restricted target/parent/media FKs and split reads. Web owns encoded public/Admin Razor,
links and optional fact-gated schemas. No new package/API/app/domain; see ADR-023 and docs/seo/LOCAL_SEO_SETUP.md.

## Phase 11 contact/external integration boundary

Application owns contact eligibility/retry and notification ports; Domain stores durable metadata on ContactMessage.
Infrastructure owns atomic EF lease store, validated TLS SMTP adapter and bounded hosted processing. Web owns IP/trap/CSRF,
protected consent, Razor preferences, nonce/CSP loader and minimal events. No new package/API/app/project reference.
ADR-024 and docs/contact/CONTACT_ANALYTICS_SETUP.md describe delivery semantics and legal/hosting activation gates.
