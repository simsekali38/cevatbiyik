# URL Map

Implemented in Phase 04:

```text
GET  /
GET  /hakkimda
GET  /makaleler?q={search}&category={PracticeAreaId}&page={page}&categoryPage={chooserPage}
POST /makaleler (antiforgery search → GET redirect)
GET  /makaleler/{slug}
GET  /uzmanlik-alanlari?page={page}
GET  /uzmanlik-alanlari/{slug}
GET  /iletisim
POST /iletisim (antiforgery/validation/rate limit → GET redirect)
```

Nonpublic/missing details return 404. Canonical origin is configured HTTPS, not request Host.
No /hukuk-alanlari route existed previously; owner's /uzmanlik-alanlari is authoritative.

Original planned route set (remaining routes are not implemented):

/
 /hakkimda
 /hukuk-alanlari
 /hukuk-alanlari/{slug}
 /makaleler
 /makaleler/{slug}
 /sss
 /iletisim
 /kvkk
 /gizlilik
 /cerez-politikasi
 /yasal-bilgilendirme
 /kayseri-avukat
 /kayseri/{service-slug}

Admin:

/admin

API future:

/api/v1/

## Admin (Phase 05)

/admin/login GET/POST; /admin/logout POST; /admin/access-denied GET; /admin dashboard. Protected lists: /admin/articles, /admin/practice-areas, /admin/contact-messages, /admin/settings. Editors use create and edit/{id}; writes use save/{id?}, delete/{id} and contact-messages/read/{id}. Admin is noindex/no-store; every write requires antiforgery and Admin role. No /admin/register endpoint.
