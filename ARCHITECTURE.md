# Architecture

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
