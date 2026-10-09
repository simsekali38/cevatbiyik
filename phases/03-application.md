# Phase 03 — Application & Service Layer

## Scope

Closure: owner accepted Phase 03 after zero-error/zero-warning build and 54 passed tests.

User-requested Application phase added after Phase 02 closure. The original
`05-identity-admin-foundation.md` remains a separate pending scope; this phase does not implement Identity or admin routes.

## Deliverables

- ArticleDto, CreateArticleDto, UpdateArticleDto; PracticeAreaDto, CreatePracticeAreaDto;
  ContactMessageDto, CreateContactMessageDto; SiteSettingDto, UpdateSiteSettingDto.
- IArticleService, IPracticeAreaService, IContactService, ISettingService, ISlugService and implementations.
- IRepository<T> port and EF/MySQL adapter, without EF references in Application.
- FluentValidation 12.1.1 with explicit asynchronous validation inside write services.
- Scoped DI, bounded pagination, soft-delete, stable slugs and safe conflict/not-found errors.
- Business unit tests; optional MySQL application-service integration test.

## Acceptance and limits

Build and unit/web regression tests must pass. Live persistence tests require an explicitly configured
dedicated test database; see TESTING.md. No database migration or production data change is required for this phase.
Mutation and unrestricted read services are internal admin use cases and need authorization at their future Web entry points.
Only Article.ListPublishedAsync is currently a public publication query. The other read methods include drafts/private data.
Content inputs are plain text: no rich HTML editor or sanitization package is introduced.
Razor must encode all DTO text; Html.Raw must never render these fields.
Contact messages are immutable after submission except read/processed state and soft deletion.
CreatePracticeAreaDto is also the complete update input; UpdateSiteSettingDto is used by both create and update.
Settings accept only the documented public keys. Future key types and richer content require explicit validation changes.
Rate limiting, anti-forgery, authentication, legal editorial review, consent wording and retention/purge policies remain in their respective phases.
