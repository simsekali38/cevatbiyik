# Architecture

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
