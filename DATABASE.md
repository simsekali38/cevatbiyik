# Database Blueprint

## Phase 08 implemented persistence

PublicSiteContent adds LawyerProfiles, PageContents and FaqItems (21 model entity types including Identity).
All three use shared UTC timestamps/soft-delete fields and IsReviewed; active defaults to false in storage.
Unique indexes reserve primary profile Key and fixed page Path. FAQ has a composite visibility/order index.
No personal identity/registry fields, seeds, blobs, existing-data changes or foreign-key cascades are added.
SQL: docs/database/PUBLIC_SITE_CONTENT.sql. Migration has not been applied to cevodb by the agent.
Setup: docs/cms/PUBLIC_CONTENT_SETUP.md; dedicated MySQL validation remains a rollout gate.

## DBMS

MySQL 8.x.

## Main entities

Phase 02 implements only Article, PracticeArea, ContactMessage and SiteSetting plus shared BaseEntity fields.
The sections below are the broader roadmap, not the implemented schema. ContactMessage is the implemented name for planned ContactSubmission.
Current details: `docs/database/ERD.md`, ADR-013 and `docs/database/MIGRATIONS.md`.

Phase 05 adds Identity/AuditLogs. Phase 06 adds ArticleCategories, ArticleTags, ArticleTagLinks,
MediaAssets, PageSeo and nullable article taxonomy/media/SEO fields (ADR-019, CmsFeatures).
Profile, arbitrary page content, FAQ, redirects and locations remain roadmap.

### Profile

id
name
professional_title
short_bio
full_bio
profile_image_id
bar_association
bar_registration
education
experience
languages
phone
email
whatsapp
office_address
working_hours
created_at
updated_at

### PracticeArea

id
name
slug
short_description
content
hero_media_id
status
sort_order
created_at
updated_at

### Page

id
title
slug
content
status
template
created_at
updated_at

### Location

id
name
slug
city
district
description
status
created_at
updated_at

### Article

id
title
slug
excerpt
content
cover_media_id
author_id
status
published_at
updated_at
created_at

### ArticleCategory

id
name
slug
description

### ArticleTag

id
name
slug

### FAQ

id
question
answer
sort_order
status

### Media

id
original_name
stored_name
path
mime_type
extension
size
width
height
alt_text
caption
uploaded_by
created_at

### SeoMetadata

id
entity_type
entity_id
title
description
canonical_url
robots
og_title
og_description
og_image_id
include_in_sitemap
created_at
updated_at

### Redirect

id
source
destination
status_code
is_active
created_at
updated_at

### ContactSubmission

id
name
phone
email
message
consent
status
created_at
processed_at

### SiteSetting

id
key
value
data_type
updated_at

### AuditLog

id
user_id
action
entity_type
entity_id
metadata
ip_address
user_agent
created_at

## Indexes

At minimum:
- unique slugs
- published status + date for articles
- active redirects by source
- SEO entity_type + entity_id
- contact status + created_at
- audit user + created_at
- media path/stored_name where appropriate

Use foreign keys and cascading behavior intentionally.

## Phase 05 implemented persistence

AppDbContext now derives from IdentityDbContext<ApplicationUser>. IdentityAdmin adds the seven standard AspNet Identity tables and AuditLogs. AuditLog stores Id, UTC CreatedAt, nullable ActorId (255) and Action (64), with a CreatedAt/Id index and no soft-delete. ApplicationUser adds IsActive. No credential seed or content-table alteration. See docs/security/ADMIN_SETUP.md and docs/database/IDENTITY_ADMIN.sql. Migration application to cevodb remains pending.

## Phase 09 — SeoRedirects

Adds RedirectRules with unique reserved SourcePath (200), DestinationPath (200), UTC and soft-delete fields.
PracticeAreas gains six nullable SEO/OG fields; OpenGraphMediaId references MediaAssets with restrictive deletion.
22 model entity types including Identity. No seed or existing-data drop in Up.
Reviewed SQL: docs/database/SEO_REDIRECTS.sql. Migration has not been applied here.

## Phase 10 — LocalSeo

Locations, LocationPracticeAreas and LocationArticles bring the model to 25 entity types. Unique reserved Slug, unique
association pairs, publication/parent/media indexes; eight new indexes and six FKs. Body longtext (12000 application chars),
LocalDetails text (4000). No seeds or existing-table alteration. Reviewed unapplied SQL: docs/database/LOCAL_SEO.sql.

## Phase 11 notification metadata

ContactNotifications adds six fields and one pending-work index to ContactMessages. Old rows default not pending.
Message + queue eligibility persist in one row/save. Conditional SQL lease claims and lease-ID completion allow bounded
retry/recovery; soft deletion excludes work. No contact payload duplication/seeds. MySQL SQL and rollout notes in docs/database/.
