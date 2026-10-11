# Remaining work — starting assessment

Date: 2026-10-11 (Europe/Istanbul). Branch: main. HEAD: 60d4a6c.
Working tree contains the preceding uncommitted article/practice-area changes; no staged
changes. Preserve them. No automatic commit/push, live publishing or database writes.

Verified completed work: shared public shell/home, article list/detail and practice-area
list/detail. Latest executed regression: 420 .NET, 6 JS, 18 browser passed; Release
0 warnings/errors. Eleven real DB tests were opt-in skips: 10 persistence tests and one
migration upgrade/downgrade test. They mutate schemas/data and are NOT permitted by this
request's read-only DB scope even if credentials exist. Previous dedicated cevodbtest
MariaDB 10.3.39 diagnosis is recorded, not current MySQL 8 production evidence.

Work order:
A: retain verified practice-area transfer; no redesign/model additions.
B: About page with existing reviewed biography/vision/baro; local emblem placeholder.
C: Contact, FAQ and legal-page styling, preserve form/consent/backend behavior.
D: public responsive/focus/contrast consistency, scope styles away from Admin.
E: metadata and visible-content consistency, legacy placeholder fallback fix and SEO audit.
F: regression/security/dependency/secret/performance audit, smallest justified fixes only.
G: read-only DB diagnosis if safely configured; hosting/backup/restore checklist.
H: full regression, visual evidence and final readiness report with explicit limits.

Initial gaps: no profile media/education/experience fields; no linked practice-area FAQ
collection; no /gizlilik route (current legal CMS paths are /kvkk, /cerez-politikasi,
/yasal-uyari). Do not fabricate these fields/texts or create a migration for appearance.
Raw placeholders occur in About/contact/FAQ/legal fallback views. Existing legacy
practice descriptions may propagate CONTENT_REQUIRED into SEO; normal CMS validator
blocks new publication, but safe metadata fallback should handle historical rows.

SH-12-01: owner previously reports password changed; this is accepted as owner evidence,
not independent old-credential revocation proof. Historical secrets are not erased by
rotation. Inspect current ignored/tracked configuration without printing secret values.
Actual hosting SMTP, ACL/native libraries, restore RPO/RTO, content approval, supported
DB/TLS and launch GO remain external evidence gates. Real hero/portrait are missing.

Source: pasted owner request, root guides, current ADRs, design references/reviews,
existing code/test suite and Phase 14–17 operations documents. This record does not
claim production readiness or that blocked external actions were performed.
