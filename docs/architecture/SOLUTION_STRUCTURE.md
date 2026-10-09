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
