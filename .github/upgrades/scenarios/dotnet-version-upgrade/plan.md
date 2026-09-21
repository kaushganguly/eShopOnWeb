# Upgrade Plan — eShopOnWeb to .NET 10.0

## Upgrade Options

### Strategy
**Selected**: All-At-Once — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all already on .NET 8.0 (modern-to-modern upgrade), all 🟢 Low difficulty, ≤3-tier dependency depth, no CI-green constraint. Small scope, mechanical TFM bump — incremental approach adds overhead without benefit.

### Test Coverage
**Selected**: Skip — success criteria explicitly requires `generateNewUnitTests=false`. Existing unit, integration, functional, and PublicApi integration test suites will be used to validate the upgrade.

## Scope

All 10 projects in `eShopOnWeb.sln`, currently net8.0, target net10.0:

**Libraries**
- src/ApplicationCore/ApplicationCore.csproj
- src/Infrastructure/Infrastructure.csproj
- src/BlazorShared/BlazorShared.csproj

**Applications**
- src/Web/Web.csproj (ASP.NET Core)
- src/PublicApi/PublicApi.csproj (ASP.NET Core)
- src/BlazorAdmin/BlazorAdmin.csproj (Blazor WebAssembly)

**Tests**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

Central Package Management is in effect (`Directory.Packages.props`) — the `TargetFramework` and shared package versions are defined centrally and must be updated there.

## Tasks

### 01-prerequisites: Verify toolchain and global.json compatibility

Verify the .NET 10 SDK is installed and available, and that `global.json` (if present) pins an SDK version compatible with net10.0. Confirm Central Package Management setup in `Directory.Packages.props` is well-formed before making changes, and record the current package versions (48 packages, 26 needing upgrade per assessment) as a baseline for comparison after the upgrade.

**Done when**: .NET 10 SDK confirmed installed/available, `global.json` (if present) is compatible with or updated for net10.0, and current package baseline is recorded.

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Update the centrally managed `TargetFramework` in `Directory.Packages.props` (and any per-project `TargetFramework`/`TargetFrameworks` overrides) from `net8.0` to `net10.0` across all 10 projects (ApplicationCore, Infrastructure, BlazorShared, Web, PublicApi, BlazorAdmin, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests). Bump the 19 packages flagged "Upgrade Recommended" and the 7 flagged "Incompatible" in the assessment to versions compatible with net10.0, including the two packages with known security vulnerabilities — Azure.Identity (1.10.4 → 1.21.0, used in Web.csproj) and System.Text.Json (8.0.3 → 10.0.12, used in ApplicationCore.csproj).

Fix the resulting breaking API changes: 9 binary-incompatible and 3 source-incompatible APIs, concentrated in Web.csproj (13 package issues / 15 API issues) and PublicApi.csproj (11 package issues / 5 API issues), plus the 29 API issues in FunctionalTests.csproj and 7 in PublicApiIntegrationTests.csproj. The most frequent flagged APIs are `System.Net.Http.HttpContent` (32 occurrences, behavioral change), `System.Uri` (11 occurrences, behavioral change), `ConfigurationBinder.Get<T>`/`Configure<T>` overload resolution (4 each, binary incompatible), `ConsoleLoggerExtensions.AddConsole` (behavioral change), `Exception` serialization constructor (source incompatible, obsolete in modern .NET), and `UseExceptionHandler(string)` (behavioral change). Research starting points: review `assessment/api-issues/most-frequent-api-issues.md` for the full list with file/line detail via `query_dotnet_assessment`, and check each affected project's assessment doc (`assessment/projects/{Project}.md`) for its specific package and API issue tables.

Also address deprecated packages flagged in Infrastructure, IntegrationTests, FunctionalTests, PublicApiIntegrationTests, UnitTests, PublicApi, and Web projects, and the `System.Security.Claims` package reference in ApplicationCore that is now included via framework reference and can likely be removed.

Restore dependencies and build the full solution, fixing all compilation errors in one bounded pass.

**Done when**: All 10 projects target `net10.0`, all flagged package upgrades (including the 2 security-vulnerable packages) are applied, and the solution builds with 0 errors and 0 warnings.

### 03-final-validation: Full solution validation

Run the full test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) against the upgraded solution and fix any failures caused by the framework/package upgrade. Confirm no build warnings remain in any of the 10 projects. Document any recommendations that are deferred (e.g., further package updates not required for net10.0 compatibility).

**Done when**: Solution builds with 0 errors/0 warnings, and all existing tests in UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests pass on net10.0.
