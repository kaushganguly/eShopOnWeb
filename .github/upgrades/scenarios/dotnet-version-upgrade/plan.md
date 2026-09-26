# Upgrade Plan — eShopOnWeb: .NET 8.0 → .NET 10.0

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Strategy | All-at-Once | 10 projects, all already net8.0 (modern-to-modern), low difficulty across the board, no CI-green constraint, no urgency signal — small scope default |
| Test Coverage | Skip | User explicitly set generateNewUnitTests=false |

#### Selected Strategy

**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0, low difficulty for every project per assessment, clear/shallow dependency structure (ApplicationCore/BlazorShared → Infrastructure/BlazorAdmin → Web/PublicApi, plus 4 test projects), one security vulnerability (Azure.Identity) to remediate as part of the bump.

## Project List

**Libraries**
- src/ApplicationCore/ApplicationCore.csproj
- src/Infrastructure/Infrastructure.csproj
- src/BlazorShared/BlazorShared.csproj

**Applications**
- src/Web/Web.csproj
- src/PublicApi/PublicApi.csproj
- src/BlazorAdmin/BlazorAdmin.csproj

**Test Projects**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Tasks

### 01-prerequisites: Verify toolchain and update global.json

Verify the .NET 10.0 SDK is installed on the build machine and update `global.json` (currently pinned to `8.0.x` with `rollForward: latestFeature`) to target the .NET 10 SDK band. Also update `Directory.Packages.props`'s central `<TargetFramework>` property (currently `net8.0`) and the version-alias properties (`AspNetVersion`=8.0.2, `SystemExtensionVersion`=8.0.0, `EntityFramworkCoreVersion`=8.0.2, `VSCodeGeneratorVersion`=8.0.0) that drive most package versions across the solution via central package management (CPM).

**Done when**: `global.json` targets a .NET 10 SDK version installed on the machine, and `dotnet --version`/`dotnet restore` succeed against it.

### 02-upgrade-core-libraries: Upgrade ApplicationCore, Infrastructure, and BlazorShared to net10.0

Update the `TargetFramework` of `src/ApplicationCore/ApplicationCore.csproj`, `src/Infrastructure/Infrastructure.csproj`, and `src/BlazorShared/BlazorShared.csproj` from `net8.0` to `net10.0`. Per assessment, ApplicationCore has 3 package issues (incl. a source-incompatible API and `System.Security.Claims` now included in the framework reference) and 2 API issues; Infrastructure has 4 package issues including deprecated packages; BlazorShared has no issues. Bump CPM-governed package versions (via `Directory.Packages.props` in task 01) for packages referenced by these projects — including EF Core packages (currently 8.0.2) — to net10.0-compatible versions, and remove/replace any package whose functionality is now provided by the framework reference (e.g., `System.Security.Claims`). Fix the source-incompatible API flagged for ApplicationCore (`System.Exception` serialization constructor, per assessment's top API issues).

**Done when**: All three projects target `net10.0`, restore cleanly, and build with 0 errors/warnings.

### 03-upgrade-web-apps: Upgrade Web, PublicApi, and BlazorAdmin to net10.0

Update the `TargetFramework` of `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, and `src/BlazorAdmin/BlazorAdmin.csproj` from `net8.0` to `net10.0`. These are the highest-risk projects per assessment: Web has 13 package issues and 14 API issues (binary-incompatible + behavioral changes, incl. security-vulnerable `Azure.Identity` 1.10.4 → 1.21.0+, plus deprecated packages); PublicApi has 11 package issues and 5 API issues (binary incompatible + incompatible NuGet package + deprecated packages); BlazorAdmin has 7 package issues and 3 API issues (behavioral change). Known risk areas flagged by assessment across these projects and their dependents: `System.Net.Http.HttpContent` (32 hits) and `System.Uri` (11 hits) behavioral changes, `ConfigurationBinder.Get<T>`/`.Configure<T>`/`.GetValue` binary-incompatible signature changes, `ConsoleLoggerExtensions.AddConsole` behavioral change, and `UseExceptionHandler(string)` behavioral change. Research each flagged call site in these projects before changing code; update `appsettings*.json`/`launchSettings.json` only if the TFM bump requires it.

**Done when**: All three projects target `net10.0`, the Azure.Identity vulnerability is remediated (package ≥ 1.21.0), and the projects build with 0 errors/warnings.

### 04-upgrade-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests to net10.0

Update the `TargetFramework` of all four test projects from `net8.0` to `net10.0`. FunctionalTests has the highest test-project risk (3 package issues, 29 API issues — behavioral changes, largely from `HttpContent`/`Uri` usage in `WebApplicationFactory`-based tests) and a deprecated package; PublicApiIntegrationTests has 3 package issues and 7 API issues; IntegrationTests and UnitTests have low package-only issues (deprecated packages, no API issues). Bump test-related CPM package versions (`Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory`, `NSubstitute`, `coverlet.collector`, etc.) to net10.0-compatible versions, replacing any deprecated package the assessment flagged. Adapt existing tests only as needed to compile/pass against net10.0 and updated dependencies — do not author new tests (Test Coverage option = Skip).

**Done when**: All four test projects target `net10.0`, build with 0 errors/warnings, and all existing tests pass.

### 05-final-validation: Full solution build and test validation

Restore and build the entire `eShopOnWeb.sln` solution (all 10 projects) targeting `net10.0`, confirm 0 errors and 0 warnings, run the full test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) and confirm all tests pass, and re-check for any remaining known-vulnerable package versions. Document any deferred/out-of-scope recommendations surfaced by the assessment that were not part of this upgrade's scope.

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors/warnings and `dotnet test` passes for all test projects.
