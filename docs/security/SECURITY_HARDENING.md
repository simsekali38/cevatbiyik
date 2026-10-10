# Phase 12 security operations

Implementation: ADR-025. One .NET 8 MVC/Razor application; no new runtime dependency, migration or database update.

## Middleware and headers

ForwardedHeaders → SecurityHeaders → ExceptionHandler/HSTS → StatusCodePages → HTTPS redirect →
routing → rate limiter → consent CSP → upload path allowlist → public StaticFiles → authentication/authorization → SEO redirects/endpoints.
SecurityHeaders uses a random 24-byte nonce per response. OnStarting restores headers cleared during error handling.
CSP rejects inline/eval scripts, objects and frames; only the existing consent/legal-gated Google origins may be added.
Private document attachment keeps its stricter sandbox/default-src none, no-store/private and digest/rescan checks.
All application responses (including static/404/429/login/HTTPS redirects) have DENY, nosniff, strict-origin-when-cross-origin
and camera/microphone/geolocation/payment/USB denial. Admin and errors stay no-store/noindex. IIS-generated responses are
outside this middleware: verify equivalent headers at hosting level without duplicate/conflicting CSP policies.

Non-development HTTPS HSTS: 180 days. Localhost remains excluded. includeSubDomains/preload intentionally disabled until
all affected hosts/certificates are verified. Development HTTP remains available; production HTTPS redirect targets port 443.

## Trusted proxies / Plesk

Default explicit trusted peers: 127.0.0.1 and ::1 (local IIS/ANCM). No trusted network range or trust-all mode.
Additional verified peer IPs: Security:TrustedProxies array, max 16; malformed addresses, CIDRs and unspecified addresses
fail startup. Only X-Forwarded-For and X-Forwarded-Proto, matching value counts, one hop. X-Forwarded-Host is never consumed.
Do not enable a global trust-all forwarded-header workaround. Configure ONLY actual immediate proxy addresses.

Example environment key (replace before use, never leave the placeholder):

```powershell
$env:Security__TrustedProxies__0 = "<VERIFIED_IMMEDIATE_PROXY_IP>"
```

Current web.config uses IIS inprocess. Native IIS server normally supplies connection IP/scheme. An additional proxy must
strip/overwrite incoming client X-Forwarded-* and append a consistent validated pair. Confirm inprocess/outofprocess mode,
header behavior, direct backend access restrictions and exact AllowedHosts/IIS domain bindings with hosting staff.
Do not log real form data, cookies, credentials or persistent IP identifiers during checks. Trusted-IP lookup is a bounded
allowlist; no reverse DNS or per-request external lookup. Canonical still uses configured Site:BaseUrl.

## Cookies and limits

Admin: __Host-CevatBiyik.Admin, Path=/, no Domain, Secure, HttpOnly, SameSite=Strict; nonpersistent browser session,
30-minute ticket lifetime without sliding renewal, five-minute stamp checks plus current active/Admin-role validation.
Consent keeps its separate protected/versioned expiring __Host cookie. Antiforgery remains HttpOnly/Strict, Secure outside Development.

- Login POST: 10/IP/fixed minute. Identity additionally locks after five failed attempts.
- Contact POST: 5/IP/sliding minute, six segments; failed CSRF attempts count.
- Consent POST: 20/IP/fixed minute.
- Extension-bearing paths and /uploads: shared 120/IP/fixed minute BEFORE StaticFiles, including missing/disallowed files.

All queues disabled; rejection 429 with Retry-After: 60. IPv4 and IPv4-mapped IPv6 normalize to the same key.
Unknown IPs share one conservative bucket. Limits are process-local and reset on recycle; not a DDoS guarantee or distributed
limiter. Shared NAT visitors share limits. Measure crawler/asset traffic; upstream IIS protections and actual Plesk IP behavior
must be verified. Error-page re-execution is extensionless and does not consume a second static permit.

## Upload/auth review

Unchanged ADR-019/020 pipeline: images accept decoded still JPEG/PNG/WebP only, re-encode WebP, 5 MiB, <=8000/axis,
<=16 million pixels, output <=1600/axis. Arbitrary names never determine paths; ADS/control names rejected. MIME mismatch,
SVG/scripts/EXE/corruption/byte/pixel bombs rejected. Generated public paths use full-string anchors; trailing newline and
mixed-case upload-directory bypasses are denied. Do not place confidential material in the public image store.
PDF only, 5 MiB, classic xref/EOF and conservative escaped-name active-content gate, private root outside wwwroot, fail-closed
fresh Defender scan. Downloads require active Admin, verify digest, rescan, use attachment and sandbox/no-store.
Private keys/paths, junction/reparse points, orphan traversal and partial uploads retain prior checks. 6 MiB MVC/IIS envelopes
preserved. Local tests use controlled scanner fakes, not live malware, real Defender or a complete PDF CDR/parser.
Plesk native Skia smoke test, patched Defender/signatures, ACL/no-execute, IIS limits and consistent backups remain mandatory;
see docs/deployment/PLESK_MEDIA_RUNBOOK.md. No antivirus guarantee for public image decoding or unknown threats is asserted.

## Dependency and secret audit

```powershell
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
python tools/security_scan.py --self-test
python tools/security_scan.py --history
```

NuGet audit on 2026-10-10: six projects, no known vulnerable direct/transitive packages reported by nuget.org. No package
added/updated. Re-run with working network before release; a clean advisory result is not proof against unknown flaws.
Local verification used SDK 8.0.425 / ASP.NET Core runtime 8.0.31. Hosting runtime is unverified; accepted .NET 8 support-end
risk R-01 remains, with upgrade required before unsupported production operation.

Scanner reads Git-tracked plus nonignored untracked text and optionally all distinct reachable historical text blobs;
it reports path/line/category only, never values. It checks common provider tokens/private keys, literal credentials,
password-bearing connections and URI userinfo. Synthetic fixtures are explicitly reviewed. This heuristic has no complete
entropy/provider coverage, OCR/binary, ignored-file or remote-ref coverage; human review and provider-aware scanning remain needed.
Exit 1 means a finding and must never be reported as a clean scan.

**SH-12-01 — unresolved credential finding / production release blocker:** a nonempty database password is present at
src/CevatBiyik.Web/appsettings.json:10 (ConnectionStrings:DefaultConnection) and reachable history blob b5d0f85ae705.
Usability/external exposure was not tested. Owner instructed on 2026-10-10: leave the file untouched and report the finding.
Do not copy values into logs/commands/reports. Owner must rotate the credential, move the replacement into user-secrets/Plesk,
clean tracked config and coordinate separate authorized history remediation if necessary. Deleting current text alone cannot
revoke a historical credential. Preserve unrelated owner config edits; never silently rewrite Git history or remote refs.
Until resolved, the scan intentionally exits 1. No production deployment approved by local test success.

## Primary references reviewed

- [Microsoft forwarded header trust and middleware order](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-8.0).
- [Unknown proxies ignored from ASP.NET Core 8.0.17](https://learn.microsoft.com/en-us/aspnet/core/breaking-changes/8/forwarded-headers-unknown-proxies).
- [Microsoft package vulnerability auditing](https://learn.microsoft.com/en-us/nuget/concepts/auditing-packages).
- [Microsoft rate limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0).
