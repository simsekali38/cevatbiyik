# Security Baseline

## Phase 08 reviewed content

Profile/page/FAQ Admin mutations inherit role authorization, CSRF, explicit binding and transactional audit.
Publish requires complete plain text plus review assertion; edits reset the review checkbox. Missing legal/profile
content is noindex and CONTENT_REQUIRED. Production contact additionally requires reviewed published KVKK.
Razor encoding and the default JSON serializer prevent legacy markup from executing in HTML/FAQ schema.
Root source card photographs remain outside wwwroot; no identity/registry fields or content seeds are introduced.
Human legal/content review remains mandatory before publication. See ADR-021 and PUBLIC_CONTENT_SETUP.md.

## Phase 07 private storage

Private PDF documents live outside wwwroot, require fresh fail-closed Defender scan and download digest verification.
Admin role/policy, CSRF-protected mutations, attachment/no-store/nosniff/sandbox and append-only action audit apply.
PDF content policy is conservative and is not a full parser/CDR or guarantee against unknown threats.
Public images remain re-encoded WebP; filename/ADS/control-character/reparse checks are stricter.
Deleted rows retain files; old public image URLs remain accessible. No confidential document may use public media storage.
Orphan quarantine is private/reversible with 48h grace and current DB recheck; no automatic hard purge.
Hosting ACL/Defender/real IIS/native/consistent restore checks: docs/deployment/PLESK_MEDIA_RUNBOOK.md.

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

## Phase 09 redirect and metadata controls

Admin role, explicit antiforgery and transactional audit protect redirect mutations. Only normalized same-site
public paths are accepted; protected/static paths, external URLs, ambiguous encoding, chains and loops are rejected.
GET/HEAD redirects discard queries and recheck publication; current content takes priority. Metadata stays encoded,
JSON-LD keeps safe serialization/CSP, and global indexing denial overrides page settings. Robots is not access control.
See ADR-022 and docs/seo/SEO_SETUP.md.

## Phase 10 local content controls

Admin role/policy, explicit binding, antiforgery, confirmation and transaction/audit protect Location mutations. All text is
validated/encoded; JSON-LD keeps the safe serializer/CSP. Review and verified-fact gates prevent placeholder schema/indexing.
Soft deletion retains local content; referenced media is protected. Noindex is not access control. ADR-023 records safeguards.

## Phase 11 contact and tracking controls

Web-only trap, strict email mailbox check and normalized connection-IP sliding limit complement existing CSRF/encoding/limits.
SMTP uses secrets outside CMS, fixed headers/recipient, mandatory validated STARTTLS/587, metadata-only notice and generic logs.
Consent is protected/versioned/expiring, Secure/HttpOnly/Strict/host-only; scripts/CSP external origins require current explicit opt-in,
complete reviewed legal pages and configuration. Admin/errors excluded; no pre-consent ping or PII in event hooks.
GTM container/property/transfer/legal review, Plesk uptime/IP topology and real MySQL/SMTP checks remain rollout gates; see ADR-024.
