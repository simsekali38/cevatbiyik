# Database Blueprint

## DBMS

MySQL 8.x.

## Main entities

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
