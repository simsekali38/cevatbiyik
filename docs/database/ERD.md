# ERD Concept

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
