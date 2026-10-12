# MailKit / MimeKit notification review — 2026-10-12

Owner requested MimeKit migration and supplied a 465/Username/To/UseSsl example.
MailKit 4.17.0 is the only new direct package; MimeKit is its dependency. Adapter
remains in Infrastructure behind IContactEmailSender. No migration, DB operation,
production activation, real SMTP connection, commit or deployment was performed.

## Behavior

- MimeMessage with fixed subject, plain UTF-8 text and record ID only.
- Configured From/Recipient only; no visitor-controlled recipient/Reply-To/header.
- Authenticated mandatory STARTTLS/587 or TLS-on-connect/465, default certificate
  validation, no plaintext fallback. Unsupported ports and UseSsl=false are rejected.
- To aliases Recipient (Recipient wins); Username casing works. Enabled must be true.
- Existing queued notifications, leases/retries and generic worker logging remain.
- Connect/authenticate/send/disconnect are asynchronous with cancellation; 30-second
  transport timeout plus existing worker cancellation. After successful SendAsync,
  expected QUIT network/protocol/cancellation errors do not retry accepted mail.
  Existing SMTP-success/DB-failure replay risk remains at-least-once delivery.
- Local/server private JSON files are untouched. Restart after settings changes.

## Verification

From project/:

```powershell
dotnet restore CevatBiyik.sln --disable-parallel
dotnet build CevatBiyik.sln -c Release --no-restore -m:1 -p:UseSharedCompilation=false
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
$env:Smtp__Enabled = 'false'
dotnet test CevatBiyik.sln -c Release --no-build --no-restore -m:1 --filter 'FullyQualifiedName!~DevelopmentConfigurationTests'
dotnet test tests/CevatBiyik.IntegrationTests -c Release --no-build --no-restore --filter 'FullyQualifiedName~DevelopmentConfigurationTests'
dotnet list src/CevatBiyik.Infrastructure package --vulnerable --include-transitive
dotnet format style CevatBiyik.sln --verify-no-changes --no-restore --diagnostics IDE0011
```

Release build: 0 errors, 0 warnings. 216 unit and 246 integration tests passed (462).
11 opt-in MySQL/migration tests skipped; no external database verification.
Separate DevelopmentConfiguration test still fails on existing local JSON
AllowedHosts="*" instead of localhost. This is not a MailKit regression and local
configuration was not edited. The full suite cannot be called green.
NuGet audit found no vulnerable packages in Infrastructure, including transitives.
Style and git diff whitespace checks passed.

Tests cover MIME round-trip/Turkish encoding (normalizing MIME terminal CRLF), fixed
headers/recipient/body privacy, valid/invalid ports, settings aliases, TLS-disable
rejection, disabled/cancelled delivery before connection, and existing queue flows.
No real server certificate/authentication/inbox delivery was tested; perform one
authorized new contact submission after deployment and verify admin notification
status and target inbox. Messages created with SMTP disabled are not backfilled.

Logs: ignored artifacts/test-runs/mailkit-build-final.log, mailkit-tests-final.log
and mailkit-localconfig.log. Setup: docs/contact/CONTACT_ANALYTICS_SETUP.md. ADR-039
supersedes ADR-024's legacy transport while retaining durable queue design.
