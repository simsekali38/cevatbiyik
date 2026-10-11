# Phase 12 verification — 2026-10-10

Status: Implemented / locally verified; owner closure approved (2026-10-10); commit authorized. **SH-12-01 unresolved: security scan is not clean.**
No migration, live database update/deployment/provider activation or Phase 13 work. Phase 12 commit authorized. Owner instructed to leave
appsettings.json and Git history untouched and report the finding. No credential value is included in this report.

## Contract and delivered files

Reviewed Phase 00/PROJECT_AUDIT, current ADRs/accepted R-01, AGENTS/CLAUDE/MASTER_PROMPT, manifest/PHASE_ORDER,
OPEN_QUESTIONS, architecture/security/deployment and architecture/backend/security/testing/QA/devops skills.
.NET8/MySQL8/Plesk/linear references, one SSR app/filesystem and existing brand/content/consent/upload gates preserved.

| File | Concrete change |
| --- | --- |
| project/src/CevatBiyik.Web/Program.cs | Explicit cookie Path, normalized login, global static limiter, bounded proxy trust/HSTS and corrected middleware order/upload anchors |
| project/src/CevatBiyik.Web/Security/SecurityHeadersMiddleware.cs | Nonce and final-response headers; preserve sandbox/consent overrides; Admin/error no-store/noindex |
| project/src/CevatBiyik.Web/Security/ConsentCspMiddleware.cs | Reuse existing explicit opt-in CSP inside exception handling and after limiter |
| project/tests/CevatBiyik.IntegrationTests/SecurityHardeningTests.cs | 36 header/proxy/limit/auth/image attack cases |
| project/tests/CevatBiyik.IntegrationTests/AdminFlowTests.cs | Cookie host-prefix attributes/ticket lifetime plus expired/disabled Admin document-access test |
| project/tests/CevatBiyik.IntegrationTests/DocumentFlowTests.cs | Sandbox preserved on download; security headers restored after audit failure/500 |
| project/tests/CevatBiyik.UnitTests/FileLibraryTests.cs | 10 more active/escaped PDF name attacks |
| project/tools/security_scan.py, .gitignore | Redacted standard-library working-tree/reachable-history scanner, eight self-tests; ignore local Python cache |
| docs/security/SECURITY_HARDENING.md | Configuration, controls, hosting verification, explicit limitations and primary references |
| DECISIONS/SECURITY/ARCHITECTURE/DEPLOYMENT/TESTING/CHANGELOG/OPEN_QUESTIONS and phase guides | ADR-025, findings, current status and verification record |

## Executed verification

```powershell
dotnet build project/CevatBiyik.sln -c Release --no-restore -m:1 -p:UseSharedCompilation=false
dotnet test project/CevatBiyik.sln -c Release --no-restore -m:1 -p:UseSharedCompilation=false --logger "trx;LogFilePrefix=phase12-verified"
node --test project/tests/CevatBiyik.BrowserTests/analytics.test.cjs
dotnet format CevatBiyik.sln style --verify-no-changes --no-restore --severity warn --diagnostics IDE0011
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
python project/tools/security_scan.py --self-test
python project/tools/security_scan.py --history
git -c core.safecrlf=false diff --check -- . ':!project/src/CevatBiyik.Web/appsettings.json'
```

- Release build **0 errors / 0 warnings**. Local SDK 8.0.425 / ASP.NET Core runtime 8.0.31.
- Final suite **165 unit + 164 integration = 329 passed; 9 dedicated MySQL skipped**, because no CEVATBIYIK_TEST_MYSQL.
- **6 JavaScript tests passed**, no change to consent/event JS. **8 scanner synthetic self-tests passed**.
- Full style/import/braces verification passed at warning severity; changed-file whitespace normalized; diff whitespace check passed.
- Live nuget.org audit **no known vulnerable direct/transitive package across six projects**; no package added/updated.
  Initial sandbox DNS failure retried with authorized network access and succeeded; failure was not called a clean audit.
- Existing EF model/no-pending-change, generated MySQL migration SQL and relational persistence tests passed; no new migration needed.
- Existing Identity/session/lockout/revocation, CRUD/audit rollback, upload/PDF malware-fake/integrity/retention/reconciliation,
  contact/honeypot/validation/CSRF, consent/JSON-LD/canonical/sitemap/robots/redirect/local-page regressions remain green.
- Header cases cover HTML/static/404/upload rejection/HTTPS redirect/authorization/429/500. Fresh nonce and HTTPS HSTS checked.
- Unknown proxy cannot forge scheme/IP; trusted proxy can forward only matching one-hop For/Proto; forwarded Host ignored;
  invalid allowlist fails startup; mapped IPv4 cannot gain another login bucket. Static and contact invalid requests consume budgets.
- Admin action descriptors are enumerated: every GET/POST route challenges anonymous access before action execution; only login/access-denied may be anonymous. Expired and disabled sessions cannot download private documents.
- New active-PDF cases include escaped JavaScript/Launch and external/form/rich-media names. Images cover valid PNG with spoofed MIME,
  EXE/JS/ADS and product >16 million pixels despite valid per-axis dimensions; prior byte/axis/truncated/re-encoding tests retained.
- No source-card reference/identity/registry field found in public Views/wwwroot text during review; no source image published.

## SH-12-01 — unresolved credential finding

Redacted final scan examined 373 selected tracked/nonignored working text files and 691 distinct reachable Git text blobs; values never printed.
One credential finding appears in two locations: appsettings.json:10 and history blob b5d0f85ae705, ConnectionStrings:DefaultConnection.
Nonempty DB password observed; usability, remote publication and compromise were not tested. This is not a provider-token
revocation assessment. Scanner exit **1** intentionally reflects the unresolved finding; eight self-tests passing do not clear it.
Owner response 2026-10-10: **"Dosyaya dokunma; bulguyu raporla"**. File/user config edits and all Git history preserved.
Required before production: rotate credential; replacement only in user-secrets/Plesk; remove tracked credential; separately
coordinate authorized history remediation. Removing a current file does not revoke a historical credential. No clean-secret-scan
or production-security-completion claim. Scanner heuristics lack complete entropy/provider, OCR/binary/ignored-file/remote-ref coverage.

## Explicit environment-dependent gates and remaining work

1. SH-12-01 owner credential rotation/private configuration/history decision; critical production release blocker.
2. Dedicated MySQL8 tests (9 skipped), real remote grants/TLS and controlled prior migration rollout; no production DB touched.
3. Actual IIS/Plesk mode, immediate trusted proxy addresses, header stripping/counts, backend isolation and exact host binding/AllowedHosts.
4. HTTPS/certificates/redirects and application + IIS-generated headers; HSTS deployment and all-host verification before preload/subdomain policy.
5. Patched Windows Skia native loading, real Defender executable/signatures/detection/fail-closed smoke, private ACL/no-execute,
   IIS/MVC 6 MiB versus 5 MiB file rejection, frozen DB/files backup and restore. Test scanner fakes are not real malware tests.
6. Process-local limiter recycle/NAT/crawler capacity and upstream abuse controls; no distributed/DDoS guarantee.
7. Accepted .NET8 support-end R-01 and verified supported/patched Hosting Bundle before launch; SMTP/Google/legal rollout gates retained.

This is automated/application baseline verification, not an external penetration-test certification or legal compliance assertion.
Owner phase closure approved (2026-10-10); commit authorized; no later phase started.
