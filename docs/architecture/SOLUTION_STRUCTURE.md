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
│   └── CevatBiyik.IntegrationTests
├── docs
├── skills
└── phases
```

Project references (ADR-012):

`CevatBiyik.Web` → `CevatBiyik.Infrastructure` → `CevatBiyik.Application` → `CevatBiyik.Domain`

Target framework is defined only in `Directory.Build.props` (`net8.0`). Package versions are defined only in `Directory.Packages.props`.

Phase 03 layout:

```text
src/CevatBiyik.Application/
  DTOs/         input and read models (no EF entities returned)
  Interfaces/   business services, slug helper, IRepository<T> port
  Services/     business rules and explicit soft deletion
  Validation/   FluentValidation validators and plain-text input policy
  Exceptions/   safe NotFoundException and ConflictException
src/CevatBiyik.Infrastructure/Persistence/
  EfRepository.cs   MySQL implementation of the Application port
```

## Phase 05 additions

Domain/Entities/AuditLog; Application auth/audit/transaction/dashboard ports and services; Infrastructure/Identity implementations and IdentityAdmin migration; Web/Areas/Admin controllers/models/Razor views and Security/AdminAreaConvention. Infrastructure references Microsoft.AspNetCore.App and Identity EF; Domain/Application remain framework-independent. Tests add SQLite solely for relational Identity/cookie integration.
