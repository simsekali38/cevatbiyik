# Phase Order

Completed phases: 00–11 (owner-approved closure). Database rollout execution and Plesk validation are not implied by code acceptance.
Phase 07 closure and commit approved by the owner; dedicated MySQL/Plesk checks remain rollout gates.
Phase 08 implemented/locally verified; owner closure approved (2026-10-10). Phase 09 implemented/locally verified, owner closure approved (2026-10-10). Phase 10 implemented/locally verified, owner closure approved (2026-10-10). Phase 11 implemented/locally verified; owner closure approved. Remaining implementation phases: 12–17.
Numbering is unique; see ADR-016/018/019/020/021/022/023/024.

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
| 12 | [12-security-hardening.md](12-security-hardening.md) | Pending |
| 13 | [13-testing.md](13-testing.md) | Pending |
| 14 | [14-performance-accessibility.md](14-performance-accessibility.md) | Pending |
| 15 | [15-plesk-deployment.md](15-plesk-deployment.md) | Pending |
| 16 | [16-production-audit.md](16-production-audit.md) | Pending |
| 17 | [17-launch.md](17-launch.md) | Pending |

Phase 08 covers remaining profile/page-content/FAQ CMS and FAQ/legal/location/public-site completion (ADR-019).
Phase 07 retains document/private-file, retention/recovery and hosting checks; Phase 09 retains redirects/broader SEO/indexing review.
Bringing forward image/core SEO in Phase 06 does not close Phase 07/09 or automatically advance the project.
