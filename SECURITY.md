# Security Baseline

## Phase 06 uploads and JSON-LD

Public uploads accept decoded/re-encoded JPEG/PNG/WebP, byte/pixel limits, random paths and validated alt text.
SVG/documents/executables are rejected. App middleware permits generated WebP paths; IIS adds extension/verb allowlists.
Taxonomy/media/SEO require Admin/CSRF/audit; failed saves/audits compensate filesystem writes.
JSON-LD uses default HTML-safe encoding and CSP nonce in a controlled TagHelper, never raw CMS HTML.
Hosting ACL/native-assets/request-filtering/body-limit checks remain mandatory: docs/cms/CMS_SETUP.md.

## Authentication

ASP.NET Core Identity.

Phase 05 implements the Admin role with secure host-only session cookies, lockout,
CSRF and login rate limiting. Every Admin route has server-side authorization except
explicit login/access-denied. Live role/active checks revoke stale access immediately.
No public registration endpoint. See [admin setup](docs/security/ADMIN_SETUP.md).
CRUD audit and mutation share a transaction; no credentials/request bodies in AuditLogs.

## Authorization

Server-side authorization for every admin action.

## Input

Validate all external input.

## Output

Encode by default. Sanitize rich HTML.

## Database

Use EF Core parameterization. Never concatenate SQL from user input.

## Uploads

Validate extension, MIME, size, content and path. Never trust filename.

## CSRF

Use antiforgery protection on state-changing MVC forms.
Phase 04 validates explicit contact/search POST tokens; search redirects without the token in the URL.
Contact uses field allowlisting, server validation, a 32 KiB request limit and five attempts/IP/minute.
Verb-independent error re-execution preserves 400/429. Production collection requires explicit enablement and reviewed notice text.

## Rate limiting

Protect login, contact and expensive endpoints.

## Secrets

Never commit secrets.

Use Plesk environment/configuration for production secrets.

## Headers

Evaluate:
- CSP
- HSTS
- X-Content-Type-Options
- Referrer-Policy
- Permissions-Policy
- frame protection

## Logging

Never log:
- passwords
- tokens
- secrets
- full connection strings

## Dependency security

Run package vulnerability checks before release.
