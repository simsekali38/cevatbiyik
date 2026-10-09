# Phase Order

Completed phases: 00–06 (owner-approved closure). Database rollout execution and Plesk validation are not implied by code acceptance.
Phase 06 closure and commit approved by the owner; dedicated MySQL/Plesk checks remain rollout gates.
Remaining implementation phases: 07–17. Numbering is unique; see ADR-016/018/019.

| Phase | Guide | Status |
|---|---|---|
| 00 | [00-discovery.md](00-discovery.md) | Completed |
| 01 | [01-foundation.md](01-foundation.md) | Completed |
| 02 | [02-domain-database.md](02-domain-database.md) | Completed |
| 03 | [03-application.md](03-application.md) | Completed |
| 04 | [04-public-ui.md](04-public-ui.md) | Completed |
| 05 | [05-identity-auth.md](05-identity-auth.md) | Completed (owner approved) |
| 06 | [06-cms-features.md](06-cms-features.md) | Completed (owner approved) |
| 07 | [07-media-files.md](07-media-files.md) | Implemented and locally verified; owner closure pending |
| 08 | [08-public-site.md](08-public-site.md) | Pending |
| 09 | [09-seo.md](09-seo.md) | Pending |
| 10 | [10-local-seo.md](10-local-seo.md) | Pending |
| 11 | [11-contact-analytics.md](11-contact-analytics.md) | Pending |
| 12 | [12-security-hardening.md](12-security-hardening.md) | Pending |
| 13 | [13-testing.md](13-testing.md) | Pending |
| 14 | [14-performance-accessibility.md](14-performance-accessibility.md) | Pending |
| 15 | [15-plesk-deployment.md](15-plesk-deployment.md) | Pending |
| 16 | [16-production-audit.md](16-production-audit.md) | Pending |
| 17 | [17-launch.md](17-launch.md) | Pending |

Phase 08 covers remaining profile/page-content/FAQ CMS and FAQ/legal/location/public-site completion (ADR-019).
Phase 07 retains document/private-file, retention/recovery and hosting checks; Phase 09 retains redirects/broader SEO/indexing review.
Bringing forward image/core SEO in Phase 06 does not close Phase 07/09 or automatically advance the project.
