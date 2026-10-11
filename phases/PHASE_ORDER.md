# Phase Order

Completed phases: 00–15 (owner-approved closure). Database rollout execution and Plesk validation are not implied by code acceptance.
Phase 07 closure and commit approved by the owner; dedicated MySQL/Plesk checks remain rollout gates.
Phase 08 implemented/locally verified; owner closure approved (2026-10-10). Phase 09 implemented/locally verified, owner closure approved (2026-10-10). Phase 10 implemented/locally verified, owner closure approved (2026-10-10). Phase 11 implemented/locally verified; owner closure approved. Phase 12 owner closure approved (2026-10-10); SH-12-01 remains an unresolved production blocker. Remaining later phases: 16–17.
Phase 13 implemented / locally verified (2026-10-11); owner closure approved (2026-10-11). Hosted MariaDB compatibility passed; genuine MySQL8 pending. Phase 14 implemented / locally measured and regression verified; owner closure approved (2026-10-11). Phase 15 owner closure approved (2026-10-11); local preparation verified, real hosting exit gates carried into Phase 16. Later unstarted phases: 16–17.
Numbering is unique; see ADR-016/018/019/020/021/022/023/024/025/026.

| Phase | Guide | Status |
|---|---|---|
| 00 | [00-discovery.md](00-discovery.md) | Completed |
| 01 | [01-foundation.md](01-foundation.md) | Completed |
| 02 | [02-domain-database.md](02-domain-database.md) | Completed |
| 03 | [03-application.md](03-application.md) | Completed |
| 04 | [04-public-ui.md](04-public-ui.md) | Completed |
| 05 | [05-identity-auth.md](05-identity-auth.md) | Completed (owner approved) |
| 06 | [06-cms-features.md](06-cms-features.md) | Completed (owner approved) |
| 07 | [07-media-files.md](07-media-files.md) | Completed (owner approved) |
| 08 | [08-public-site.md](08-public-site.md) | Implemented / owner approved |
| 09 | [09-seo.md](09-seo.md) | Implemented / owner closure approved (2026-10-10) |
| 10 | [10-local-seo.md](10-local-seo.md) | Completed / owner-approved closure |
| 11 | [11-contact-analytics.md](11-contact-analytics.md) | Completed / owner-approved closure |
| 12 | [12-security-hardening.md](12-security-hardening.md) | Completed / owner-approved closure; SH-12-01 production blocker open |
| 13 | [13-testing.md](13-testing.md) | Implemented / locally and hosted MariaDB verified; owner closure approved (2026-10-11); genuine MySQL8 pending |
| 14 | [14-performance-accessibility.md](14-performance-accessibility.md) | Implemented / measured; owner closure approved (2026-10-11); field CWV and complete assistive-technology audit are rollout gates |
| 15 | [15-plesk-deployment.md](15-plesk-deployment.md) | Owner closure approved (2026-10-11); local preparation verified; Plesk/rotation/migration/restore and support/TLS gates carried into Phase 16 |
| 16 | [16-production-audit.md](16-production-audit.md) | Pending |
| 17 | [17-launch.md](17-launch.md) | Pending |

Phase 08 covers remaining profile/page-content/FAQ CMS and FAQ/legal/location/public-site completion (ADR-019).
Phase 07 retains document/private-file, retention/recovery and hosting checks; Phase 09 retains redirects/broader SEO/indexing review.
Bringing forward image/core SEO in Phase 06 does not close Phase 07/09 or automatically advance the project.
