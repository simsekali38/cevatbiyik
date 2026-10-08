# Architecture Decision Records

## ADR-001 — Server-rendered ASP.NET Core MVC

Status: Proposed

Decision:
Use ASP.NET Core MVC + Razor Views rather than SPA frontend.

Reason:
SEO, performance, simplicity, single deployment, Plesk compatibility.

## ADR-002 — Single Domain

Status: Proposed

Decision:
Public website and admin use the same domain.

Reason:
Simpler deployment and authentication; no unnecessary CORS/subdomain complexity.

## ADR-003 — MySQL 8

Status: Accepted

Decision:
Use MySQL 8 with EF Core.

## ADR-004 — Hosting Filesystem

Status: Accepted

Decision:
Store uploads on hosting filesystem.

Mitigation:
Secure paths, validation, backups and controlled access.

## ADR-005 — No API Initially

Status: Accepted

Decision:
Do not build a separate API until a real client requires it.

## ADR-006 — Bootstrap

Status: Accepted

Decision:
Bootstrap is the primary UI framework.

## ADR-007 — No Cloudflare Dependency

Status: Accepted

Decision:
The project must function without Cloudflare.
