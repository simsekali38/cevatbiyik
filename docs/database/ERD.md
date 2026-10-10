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
descriptions 320, keywords 300. No source filename/blob stored. At Phase 06 closure profile/page-content/FAQ
were deferred to Phase 08; these are now implemented below. Redirect/location remain roadmap.

## Phase 08 implemented content

LawyerProfiles: unique Key (20), Biography (5000), Vision (2000), BarAssociation (150), IsReviewed.
PageContents: unique immutable Path (100), Title (200), Body (12000), IsReviewed.
FaqItems: Question (250), Answer (4000), SortOrder, IsReviewed; composite visibility/order index.
All inherit UTC timestamps/IsActive/IsDeleted. These records are independent; no artificial article/area relation,
personal identity field or fabricated content seed. Fixed-page metadata uses existing PageSeo by allowlisted path.
Profile/page/FAQ are implemented; location/redirect relations below remain future concepts.

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

## Phase 09 concrete relations

PracticeAreas.OpenGraphMediaId -> MediaAssets.Id (nullable, Restrict).
RedirectRules stores unique SourcePath -> DestinationPath; paths are validated by Application, not foreign keys.
Sources remain reserved when disabled/deleted. Shared UTC/active/deleted columns apply.

## Phase 10 concrete relations

Locations.ParentId -> Locations.Id (nullable Restrict); Locations.OpenGraphMediaId -> MediaAssets.Id (nullable Restrict).
LocationPracticeAreas: unique (LocationId, PracticeAreaId), target Restrict. LocationArticles: unique (LocationId, ArticleId), target Restrict.
Location-to-join FKs Cascade for relation orphan cleanup; location content itself is soft-deleted through Application.
