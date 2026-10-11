# Phase 11 verification — 2026-10-10

Status: Implemented / locally verified; owner closure approved and commit authorized. No live migration/provider delivery/Google activation/
deployment or Phase 12 work. User-owned appsettings.json change preserved and not inspected/modified by this phase.

## Contract and scope

Phase 00/PROJECT_AUDIT/current ADRs/accepted .NET 8 risk, AGENTS/CLAUDE/MASTER_PROMPT, manifest/order, hosting/linear
references and relevant backend/security/architecture/database/frontend/accessibility/Admin/testing/QA/legal-content skills reviewed.
ADR-017 navy/gold and existing emblem retained. SMTP/config/legal facts are not invented. One app/MySQL8/no new dependency/API.

- Form: explicit binding, safe mailbox/plain text/encoding, CSRF, 32 KiB limit, nonpersistent keyboard-hidden honeypot.
- Five attempts per normalized connection IP/sliding minute, invalid/trap requests counted, 429 and Retry-After.
- Durable one-record queue, bounded SQL claims, recovery/stale lease guard, deleted-row recheck, six attempts/backoff/timeout.
- Disabled-default SMTP with mandatory STARTTLS/587 and fixed metadata-only mail, private secrets and generic worker logging.
- Admin status/retry/confirmation/CSRF and audit; existing read/unread/soft-delete behavior preserved.
- Fail-closed configuration/legal/consent gates; version/age/future/tamper validation, secure protected cookie, CSRF/redirect limits.
- Native normal-flow changeable banner with equal-weight buttons; no consent wall or form coupling; GA-cookie withdrawal.
- Nonce-compatible GTM OR GA4; no script/ping/event queue before consent, no analytics on Admin/errors.
- Whitelisted event-only hooks and one-time DB-save contact marker; no URL/form/customer payloads, clean direct-GA location/referrer.

## Executed verification

```powershell
dotnet build -c Release --no-restore -m:1 -p:UseSharedCompilation=false
dotnet test -c Release --no-restore -m:1 -p:UseSharedCompilation=false --logger "trx;LogFilePrefix=phase11-verified"
node --test project/tests/CevatBiyik.BrowserTests/analytics.test.cjs
dotnet format style --verify-no-changes --no-restore --severity warn --diagnostics IDE0011
```

- Full Release: **155 unit + 127 integration = 282 passed; 9 dedicated MySQL tests skipped** (no CEVATBIYIK_TEST_MYSQL).
- **6 Node tests passed**, built-in test/vm/DOM fixtures with no network or third-party libraries.
- Release build **0 errors / 0 warnings**. Full style/import/braces check passed; existing LocalSeoFlowTests import order normalized.
- Existing persistence model/SQL/no-pending-model-change test passed after ContactNotifications generation.
- Generated/reviewed database/CONTACT_NOTIFICATIONS.sql: six additive fields/one index, no seeds/destructive Up,
  no retroactive queuing; no database update. SQLite relational claims verified; dedicated MySQL claim test explicitly skipped.
- Nonce comparison test fixed to decode Razor HTML attribute encoding (base64 +); actual CSP/browser nonce behavior preserved.
- Public/Admin/Identity, CMS, media/private documents, reviewed pages, SEO/sitemap/local links and previous tests remained green.

## Visual/accessibility check

Computer-use browser inspected actual test-only Razor exports on a loopback static preview. No production DB/contact data,
SMTP or Google request; preview blocks remote connections/forms. Temporary browser viewport reset and server stopped.
390×844: document client/scroll width 375 (15px scrollbar), no horizontal overflow on contact and consent pages.
1440×900: client/scroll1425, no horizontal overflow. Native summary Enter toggles, reject→Tab moves to allow,
visible solid 2.4px focus. Fields have labels; honeypot aria-hidden and tabindex=-1. Brand navy/paper contrast is
existing 13.96:1; reject/allow equal outline style. Final banner uses normal flow, so it does not obscure content focus.
Screenshot (isolated fixture): C:/Users/simse/.codex/visualizations/2026/10/09/01a121da-c4d5-74b2-8918-d3d3c259dba8/phase11-consent-390.png.
These are baseline checks, not complete WCAG or legal compliance certification.

## Rollout gates / known limits

1. Dedicated MySQL8 migration/claims/concurrency check and backup before authorized rollout. DDL is not fully transactional.
2. Real approved SMTP provider/certificate/STARTTLS/587 delivery, from-domain authorization, SPF/DKIM/DMARC and Plesk port limits.
3. IIS/Plesk AlwaysRunning/preload/idle/recycle/backlog behavior; in-process worker/limiter require actual hosting verification.
4. Trusted client-IP/proxy topology; no blanket X-Forwarded-For trust. Shared NAT and process-recycle budgets remain limits.
5. Final lawyer-approved KVKK/cookie/transfer/retention/provider notice and GTM/GA4 container/property review; switches still off.
6. Real browser/network tag verification under staging CSP, automatic capture disabled, private persistent Data Protection keys.
7. SMTP is at-least-once; failures after provider acceptance can replay, already sent/in-flight mail cannot be recalled.
8. Arbitrary GTM tags cannot be sanitized here; advertising/custom HTML excluded. Cross-tab updates need localStorage;
   cookie scopes/provider cookies may limit cleanup. Existing loaded external code/config changes need navigation/refresh.

Operating details/templates/primary references: ../contact/CONTACT_ANALYTICS_SETUP.md.
