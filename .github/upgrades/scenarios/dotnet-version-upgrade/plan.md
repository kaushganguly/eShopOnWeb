# Upgrade Plan — .NET 8.0 to .NET 10.0

## Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all already on .NET 8.0 (modern, SDK-style), clear dependency structure with ≤3 tiers (ApplicationCore/Infrastructure/BlazorShared → Web/PublicApi/BlazorAdmin → test projects), only 1 incompatible package and 1 security vulnerability. Well within All-at-Once thresholds.

## Upgrade Options

- **Upgrade Strategy**: All-at-Once
- **Test Coverage**: Skip (generateNewUnitTests=false per success criteria; existing tests must keep passing)

## Projects in Scope

**Libraries / Core**
- src/ApplicationCore/ApplicationCore.csproj
- src/Infrastructure/Infrastructure.csproj
- src/BlazorShared/BlazorShared.csproj

**Applications**
- src/Web/Web.csproj
- src/PublicApi/PublicApi.csproj
- src/BlazorAdmin/BlazorAdmin.csproj

**Tests**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Tasks

### 01-prerequisites: Verify toolchain and update SDK/build configuration

Verify the .NET 10 SDK is installed and update `global.json` to pin a .NET 10 SDK version. Update the centrally-managed `TargetFramework` in `Directory.Packages.props` (and any per-project TFM overrides) from `net8.0` to `net10.0`. Update Docker base images referenced in `docker-compose.yml`, `docker-compose.override.yml`, and `docker-compose.dcproj`, and any CI/CD workflow files (e.g. under `.github/workflows/`) that pin a .NET SDK/runtime version, to .NET 10 equivalents.

This is prerequisite/infrastructure work that all subsequent tasks depend on — no project code changes here, only SDK, central package management, and container/CI configuration.

**Done when**: `global.json` specifies a .NET 10 SDK version installed on the machine; `Directory.Packages.props` (or equivalent central file) declares `net10.0`; Docker and CI/CD files reference .NET 10 base images/SDKs where applicable.

### 02-core-libraries: Upgrade ApplicationCore, Infrastructure, and BlazorShared to net10.0

Update the target framework of `src/ApplicationCore/ApplicationCore.csproj`, `src/Infrastructure/Infrastructure.csproj`, and `src/BlazorShared/BlazorShared.csproj` to `net10.0`, and bump their NuGet package references to net10.0-compatible versions (e.g. `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Azure.Identity` → 1.21.0 to resolve the moderate-severity CVE in the current 1.10.4, `Microsoft.Extensions.Logging.Configuration`). Remove `System.Security.Claims` from Infrastructure/wherever referenced since its functionality is now included with the framework reference (NuGet.0003). ApplicationCore has 1 source-incompatible API issue (Api.0002) to research and fix.

These are the foundation projects referenced by the applications and tests, so they must build cleanly on net10.0 before downstream projects are upgraded.

**Done when**: All three projects target `net10.0`, restore and build with 0 errors, package versions are net10.0-compatible with no incompatible/vulnerable packages remaining, and the Api.0002 source-incompatibility in ApplicationCore is resolved.

### 03-web-apps: Upgrade Web, PublicApi, and BlazorAdmin applications to net10.0

Update the target framework of `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, and `src/BlazorAdmin/BlazorAdmin.csproj` to `net10.0`. These are the highest-risk projects: PublicApi has a binary-incompatible package issue (NuGet.0001) plus binary/source/behavioral API issues (Api.0001/Api.0002/Api.0003); Web has binary/source/behavioral API issues plus a security vulnerability and deprecated packages (Azure.Identity, System.IdentityModel.Tokens.Jwt); BlazorAdmin has a behavioral API change. Bump all ASP.NET Core / EF Core / Blazor package references (currently 8.0.2) to their net10.0-aligned versions (10.0.12 per assessment), remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0 per NuGet.0001, no compatible replacement identified) or find a net10-compatible alternative, and replace `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated) with the current recommended package if a straightforward drop-in exists.

Research starting points: check ASP.NET Core Identity API surface changes between 8→10, Minimal API / MediatR pipeline behavioral changes, JWT bearer authentication configuration changes, and Blazor WebAssembly hosting model changes for BlazorAdmin.

**Done when**: All three application projects target `net10.0`, build with 0 errors, all mandatory (Api.0001, NuGet.0001, NuGet.0003) and potential (Api.0002, Api.0003) issues from the assessment are resolved or explicitly documented as non-issues, and no known security vulnerabilities remain in their package graphs.

### 04-test-projects: Upgrade test projects to net10.0

Update the target framework of `tests/UnitTests/UnitTests.csproj`, `tests/IntegrationTests/IntegrationTests.csproj`, `tests/FunctionalTests/FunctionalTests.csproj`, and `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` to `net10.0`. Bump `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.AspNetCore.Mvc.Testing`, and other 8.0.2-pinned test packages to their net10.0-aligned versions. FunctionalTests and PublicApiIntegrationTests have behavioral API changes (Api.0003) to verify against; all four test projects reference deprecated packages (MSTest.TestAdapter/TestFramework, xunit, xunit.runner.console) that should be updated to current versions compatible with net10.0.

No new tests are to be generated (per success criteria `generateNewUnitTests=false`) — only update existing test project references and fix compilation/behavioral breaks so existing tests keep passing.

**Done when**: All four test projects target `net10.0`, build with 0 errors, and package references are current/non-deprecated where net10.0-compatible versions exist.

### 05-final-validation: Full solution build and test validation

Build the entire solution (`eShopOnWeb.sln`, and `Everything.sln` if it differs in scope) targeting net10.0 across all 10 projects, resolve any remaining cross-project build errors or warnings, and run the full test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests). Re-verify no security-vulnerable or incompatible packages remain anywhere in the solution (re-check Directory.Packages.props against the original assessment's vulnerability/incompatibility list).

**Done when**: Both solutions build with 0 errors and 0 warnings; all existing unit, integration, and functional tests pass; no known package vulnerabilities remain (Azure.Identity and any others flagged in assessment.md resolved).
