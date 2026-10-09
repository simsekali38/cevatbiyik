# ERD Concept

## Implemented in Phase 02

```text
PracticeAreas (1) ---- (0..*) Articles
                            PracticeAreaId nullable, ON DELETE RESTRICT
ContactMessages
SiteSettings
```

All four tables: Id (bigint identity), CreatedAt (UTC datetime(6)), UpdatedAt (nullable UTC datetime(6)), IsActive (false), IsDeleted (false).
Unique indexes: Articles.Slug, PracticeAreas.Slug, SiteSettings.Key.
Listing indexes: Articles(IsDeleted, IsActive, PublishedAt), PracticeAreas(IsDeleted, IsActive, SortOrder), ContactMessages(IsDeleted, IsRead, CreatedAt).
An additional Articles.PracticeAreaId index supports the foreign key.
No seed rows are inserted. Profile, media, SEO, Identity and classification tables below remain roadmap items.

## Future concept

```text
ApplicationUser
   │
   ├── AuditLog
   └── Article (author)

Profile
   └── Media (profile image)

PracticeArea
   ├── Media
   └── FAQ / Article relations as needed

Location
   └── PracticeArea relation where relevant

Article
   ├── Category
   ├── Tags
   ├── Media
   └── SEO

Page
   └── SEO

Media
SEO
Redirect
ContactSubmission
SiteSetting
AuditLog
```

Use join tables for many-to-many relationships.
