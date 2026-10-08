# Architecture

## Overview

A modular monolithic ASP.NET Core MVC application.

### Layers

`Web -> Application -> Domain`

`Infrastructure` implements persistence and external integrations and is consumed through application abstractions.

## Projects

### CevatBiyik.Domain

Pure domain concepts.

### CevatBiyik.Application

Use cases, DTOs, validation, service abstractions.

### CevatBiyik.Infrastructure

EF Core, MySQL, Identity persistence, filesystem storage, email, external services.

### CevatBiyik.Web

MVC controllers, Razor, admin area, middleware, static assets.

## Why MVC/Razor?

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
