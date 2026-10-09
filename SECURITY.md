# Security Baseline

## Authentication

ASP.NET Core Identity.

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
