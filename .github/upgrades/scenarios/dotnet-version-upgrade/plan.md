# Upgrade Plan: eShopOnWeb → .NET 10

## Overview
Upgrade all 10 projects in the eShopOnWeb solution from `net8.0` to `net10.0` (LTS, support ends Nov 2028).

**Solution**: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
**Assessment**: [assessment.md](assessment.md)

## Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), all SDK-style, all Low difficulty, CPM via Directory.Packages.props already in place.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects, all modern .NET, all SDK-style, low complexity |
| Unsupported Packages | Resolve Inline | Only 1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) |
| Unsupported API Handling | Fix Inline | Modern-to-modern upgrade; changes are minor and few |

---

## Projects

**Libraries (Levels 0-2)**:
- `src/BlazorShared/BlazorShared.csproj` (ClassLibrary) — 1 mandatory issue
- `src/ApplicationCore/ApplicationCore.csproj` (ClassLibrary) — 2 mandatory issues, security vulnerability in Azure.Identity
- `src/BlazorAdmin/BlazorAdmin.csproj` (AspNetCore/WASM) — 1 mandatory issue, 7 package upgrades
- `src/Infrastructure/Infrastructure.csproj` (ClassLibrary) — 1 mandatory issue, 4 package upgrades

**Applications (Levels 3)**:
- `src/PublicApi/PublicApi.csproj` (AspNetCore) — 6 mandatory issues, incompatible Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- `src/Web/Web.csproj` (AspNetCore) — 6 mandatory issues, 13 package issues, security vuln

**Tests (Levels 4-5)**:
- `tests/FunctionalTests/FunctionalTests.csproj` — 1 mandatory issue
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — 1 mandatory issue
- `tests/UnitTests/UnitTests.csproj` — 1 mandatory issue
- `tests/IntegrationTests/IntegrationTests.csproj` — 1 mandatory issue

---

## Tasks

### 01-prerequisites: Verify prerequisites and update global.json SDK pin

The eShopOnWeb solution pins its .NET SDK version in `global.json`. Before any TFM or package changes can succeed, the SDK pin must be updated to a .NET 10 SDK version. This task also verifies that a compatible .NET 10 SDK is installed in the environment.

Key files: `global.json`, `Directory.Packages.props` (inspect current state).

**Done when**: `global.json` pins a .NET 10 SDK, `dotnet --version` confirms .NET 10 SDK is active, and `dotnet restore` completes without SDK compatibility errors.

---

### 02-upgrade-projects: Upgrade all TFMs, packages, and fix API incompatibilities

Update the centralized package management file (`Directory.Packages.props`) and all individual project files to target `net10.0`. Update all framework-coupled NuGet packages to their .NET 10 compatible versions. Resolve the 1 incompatible package and fix all binary/source-incompatible API changes.

**Scope**: All 10 projects (6 source + 4 test).

**Assessment context**:
- `Directory.Packages.props` centrally manages versions — most package version bumps happen there
- 19 packages need version upgrades (all Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, Microsoft.Extensions.*, etc. → `10.0.11`)
- 1 incompatible package: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 — remove or find compatible replacement
- 1 security vulnerability: `Azure.Identity` 1.10.4 → 1.21.0
- Deprecated packages: `AutoMapper.Extensions.Microsoft.DependencyInjection`, `System.IdentityModel.Tokens.Jwt`, `xunit`, `xunit.runner.console`, `MSTest.TestAdapter`, `MSTest.TestFramework` — upgrade to latest compatible versions
- `System.Security.Claims` 4.3.0 — included with framework reference, remove explicit package reference
- Binary incompatible APIs (9 total): `ConfigurationBinder.Get<T>`, `OptionsConfigurationServiceCollectionExtensions.Configure<T>`, `ConfigurationBinder.GetValue` — likely signature changes
- Source incompatible APIs (3): `Exception(SerializationInfo, StreamingContext)` constructor removed, `TimeSpan.FromMinutes(double)` changed signature
- Behavioral changes (49): mainly `HttpContent`, `Uri`, `ConsoleLogger.AddConsole` — review and update if needed

**Research starting points**:
- Check `Directory.Packages.props` for current package version structure
- Inspect `src/PublicApi/PublicApi.csproj` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` usage
- Query assessment per-project for exact file/line locations of API issues: `query_dotnet_assessment` with project scope
- Check if any project overrides the centrally managed TFM or has additional package references

**Done when**: All 10 projects target `net10.0`, all package versions updated to .NET 10 compatible versions, incompatible package resolved, all API incompatibilities fixed, and `dotnet build` completes with 0 errors and 0 warnings in all modified projects.

---

### 03-final-validation: Full solution build and test suite

Run the complete solution build and all test suites to confirm the upgrade is complete and no regressions introduced.

**Scope**: Full solution — all 10 projects.

**Test suites**:
- `tests/UnitTests` — unit tests
- `tests/IntegrationTests` — integration tests
- `tests/FunctionalTests` — functional tests
- `tests/PublicApiIntegrationTests` — public API integration tests

**Done when**: `dotnet build eShopOnWeb.sln` exits with code 0 (no errors, no warnings), and `dotnet test eShopOnWeb.sln` shows all tests passing with 0 failures.
