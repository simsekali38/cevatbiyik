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
No seed rows are inserted. Phase 05 adds Identity/AuditLogs.

## Phase 06 implementation

```text
ArticleCategory (1) ---- (0..*) Article   [optional CategoryId, RESTRICT]
Article (1) ---- (*) ArticleTagLink (*) ---- (1) ArticleTag
                       composite key ArticleId/TagId
MediaAsset (1) ---- (*) Article         [optional CoverMediaId/OpenGraphMediaId, RESTRICT]
MediaAsset (1) ---- (*) PageSeo         [optional OpenGraphMediaId, RESTRICT]
```

Unique slugs on category/tag, unique StoragePath on media and fixed Path on PageSeo.
Four new BaseEntity types have UTC/soft-delete fields; join has composite FKs and matching parent query filters.
Physical Article cascade affects join rows only; application deletion stays soft. Metadata limits: title/OG-title 120,
descriptions 320, keywords 300. No source filename/blob stored. Profile/page-content/FAQ/redirect/location remain roadmap.

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
