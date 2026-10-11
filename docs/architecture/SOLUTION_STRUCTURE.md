# Solution Structure

```text
CevatBiyik.sln
├── src
│   ├── CevatBiyik.Web
│   ├── CevatBiyik.Application
│   ├── CevatBiyik.Domain
│   └── CevatBiyik.Infrastructure
├── tests
│   ├── CevatBiyik.UnitTests
│   ├── CevatBiyik.IntegrationTests
│   ├── CevatBiyik.E2EHost       test-only loopback HTTPS gateway
│   └── CevatBiyik.BrowserTests  Node/Playwright development-only tests
├── docs
├── skills
└── phases
```

Project references (ADR-012):

`CevatBiyik.Web` → `CevatBiyik.Infrastructure` → `CevatBiyik.Application` → `CevatBiyik.Domain`

Target framework is defined only in `Directory.Build.props` (`net8.0`). Package versions are defined only in `Directory.Packages.props`.

ADR-026: E2EHost references the existing integration fixture solely under tests; production references remain unchanged.
Node development dependencies are pinned in BrowserTests/package.json and package-lock.json, not deployed to Plesk.

Phase 03 layout:

```text
project/src/CevatBiyik.Application/
  DTOs/         input and read models (no EF entities returned)
  Interfaces/   business services, slug helper, IRepository<T> port
  Services/     business rules and explicit soft deletion
  Validation/   FluentValidation validators and plain-text input policy
  Exceptions/   safe NotFoundException and ConflictException
project/src/CevatBiyik.Infrastructure/Persistence/
  EfRepository.cs   MySQL implementation of the Application port
```

## Phase 05 additions

Domain/Entities/AuditLog; Application auth/audit/transaction/dashboard ports and services; Infrastructure/Identity implementations and IdentityAdmin migration; Web/Areas/Admin controllers/models/Razor views and Security/AdminAreaConvention. Infrastructure references Microsoft.AspNetCore.App and Identity EF; Domain/Application remain framework-independent. Tests add SQLite solely for relational Identity/cookie integration.
