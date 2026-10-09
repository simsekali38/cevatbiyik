# Architecture Decision Records

## ADR-001 — Server-rendered ASP.NET Core MVC

Status: Accepted (2026-10-09, Phase 01)

Decision:
Use ASP.NET Core MVC + Razor Views rather than SPA frontend.

Reason:
SEO, performance, simplicity, single deployment, Plesk compatibility.

## ADR-002 — Single Domain

Status: Accepted (2026-10-09, Phase 01)

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

## ADR-008 — Target Framework: .NET 8 LTS

Status: Accepted (2026-10-09, Phase 00/01). Accepted risk, upgrade plan required.

Decision:
Target .NET 8 LTS (`net8.0`) for the initial implementation. The hosting provider confirmed .NET 8 support on the Plesk server.

Reason:
Hosting compatibility and stability (owner decision, confirmed twice).

Risk:
Microsoft ends support for .NET 8 on 2026-11-10 (no further security fixes or technical support; existing apps keep running). The site will contain a contact form and an admin panel, so running on an unsupported runtime is not acceptable long term. .NET 10 LTS is supported until November 2028.

Mitigation / upgrade plan:
- `TargetFramework` is defined only in `Directory.Build.props`; NuGet package versions only in `Directory.Packages.props`.
- Ask the hosting provider whether the .NET 10 Hosting Bundle can be installed and when.
- Choose packages (EF Core MySQL provider, etc.) that support both .NET 8 and .NET 10; record this in the Phase 02 ADR.
- Upgrade to .NET 10 before launch (Phase 13/14) if hosting allows; otherwise immediately after hosting support lands. Upgrade steps: change `net8.0` to `net10.0`, update `global.json`, bump package versions, run build + all tests, re-verify on Plesk.
- Revisit this ADR at Phase 13 (Plesk Deployment) and at Phase 14 (Production Audit). An unsupported runtime at launch is a Phase 14 blocker unless the owner explicitly accepts it again.

## ADR-012 — Linear project references

Status: Accepted (2026-10-09, Phase 01)

Decision:
Project references are strictly `CevatBiyik.Web` → `CevatBiyik.Infrastructure` → `CevatBiyik.Application` → `CevatBiyik.Domain`.

`AddInfrastructure` registers Application services so the composition root does not reference Application directly. Controllers must not take Infrastructure concrete types.

Reason:
Matches the Phase 01 instruction and keeps a single, testable dependency direction.
