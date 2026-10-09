# Admin Panel Blueprint

Phase 06 adds /admin/taxonomy/categories, /admin/taxonomy/tags, /admin/media and /admin/page-seo.
Article editors include taxonomy/cover/SEO. Every write inherits Admin policy/CSRF and transactional audit.
Workflows and rollout: ../cms/CMS_SETUP.md. Profile/page-content/FAQ remain Phase 08 work.

## Implemented in Phase 05

- /admin/login (GET/POST), /admin/logout (POST), /admin/access-denied
- /admin dashboard counts and recent security audit
- /admin/articles and /admin/practice-areas: list/create/edit/save/delete
- /admin/contact-messages: list/read toggle/delete
- /admin/settings: allowlisted contact/site/SEO description list/create/edit/save/delete

These routes use Admin role authorization, server-side validation, antiforgery,
PRG notifications and confirmed soft-delete. The blueprint below describes later
CMS screens; it is not a list of currently implemented routes.
Setup and verification: [ADMIN_SETUP.md](../security/ADMIN_SETUP.md).

## Routes

/admin
/admin/profile
/admin/practice-areas
/admin/pages
/admin/articles
/admin/articles/create
/admin/categories
/admin/tags
/admin/faqs
/admin/locations
/admin/media
/admin/seo
/admin/redirects
/admin/contact-submissions
/admin/settings
/admin/users
/admin/audit-logs

## Dashboard widgets

- published articles
- drafts
- recent submissions
- SEO warnings
- recent admin actions

## Safety

- destructive confirmation
- antiforgery
- authorization
- audit log
