# Upgrade Plan: .NET 8 → .NET 10

## Selected Strategy

**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0, all rated Low difficulty. No .NET Framework projects, no CI-green constraint. Assessment confirms straightforward TFM bump + package updates + minor API fixes.

## Upgrade Options

| Option | Category | Selected Value |
|--------|----------|----------------|
| Upgrade Strategy | Strategy | All-at-Once |
| Unsupported Packages | Compatibility | Resolve Inline |
| Unsupported API Handling | Compatibility | Fix Inline |

## Projects in Scope

### Application Projects (6)
- src/ApplicationCore/ApplicationCore.csproj — ClassLibrary (Low risk: 3 pkg issues, 2 API issues)
- src/BlazorShared/BlazorShared.csproj — ClassLibrary (Low risk: no issues)
- src/BlazorAdmin/BlazorAdmin.csproj — AspNetCore Blazor WASM (Low risk: 7 pkg issues, 3 API issues)
- src/Infrastructure/Infrastructure.csproj — ClassLibrary (Low risk: 4 pkg issues, 0 API issues)
- src/PublicApi/PublicApi.csproj — AspNetCore API (Low risk: 11 pkg issues, 5 API issues, 1 incompatible pkg)
- src/Web/Web.csproj — AspNetCore Web (Low risk: 13 pkg issues, 15 API issues, security vuln)

### Test Projects (4)
- tests/FunctionalTests/FunctionalTests.csproj — 3 pkg issues, 29 API issues (behavioral)
- tests/IntegrationTests/IntegrationTests.csproj — 2 pkg issues
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj — 3 pkg issues, 7 API issues
- tests/UnitTests/UnitTests.csproj — 2 pkg issues

## Task List

### 01-prerequisites: Verify SDK and Update global.json

Ensure the .NET 10 SDK is available in the environment and update `global.json` to target SDK 10.x.
Also verify that `Directory.Build.props` and `Directory.Packages.props` are the right files to update
(since CPM is already in use in this solution).

The `global.json` at the repo root currently pins to the .NET 8 SDK. It must be updated to the .NET 10
SDK version. The project uses Centralized Package Management (CPM) via `Directory.Packages.props` with
variables like `$(AspNetVersion)`, `$(EntityFramworkCoreVersion)`, `$(SystemExtensionVersion)`, and
`$(VSCodeGeneratorVersion)` — these must all be updated when packages are bumped.

**Done when**: `dotnet --version` reports a .NET 10 SDK, `global.json` references a valid .NET 10 SDK
version, and no project files have been modified yet.

---

### 02-upgrade-tfm-and-packages: Upgrade Target Frameworks and NuGet Packages

Update the `TargetFramework` property in `Directory.Build.props` (or individual project files if set
there) from `net8.0` to `net10.0` across all 10 projects. Update all version-aligned Microsoft packages
in `Directory.Packages.props` by changing `$(AspNetVersion)`, `$(EntityFramworkCoreVersion)`,
`$(SystemExtensionVersion)`, and `$(VSCodeGeneratorVersion)` to their .NET 10 equivalents (10.0.x).

**Package changes required:**
- Microsoft.AspNetCore.* packages: 8.0.2 → 10.0.x
- Microsoft.EntityFrameworkCore.* packages: 8.0.2 → 10.0.x
- Microsoft.Extensions.* packages: 8.0.0 → 10.0.x
- Azure.Identity: 1.10.4 → 1.21.0 (security vulnerability fix)
- Microsoft.VisualStudio.Web.CodeGeneration.Design: 8.0.0 → 10.0.x
- System.Net.Http.Json: 8.0.0 → 10.0.x
- System.Text.Json: 8.0.3 → 10.0.x
- System.Security.Claims: 4.3.0 → **REMOVE** (included in framework reference for net10.0)
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets: 1.19.6 → **REMOVE** or find compatible version (incompatible with net10.0)

After updating project files and packages, restore dependencies and build the solution. Fix all
compilation errors in a single bounded pass. Key breaking changes to resolve:
- `ConfigurationBinder.Get<T>(IConfiguration)` binary incompatibility (9 occurrences in Web and PublicApi)
- `Exception(SerializationInfo, StreamingContext)` constructor removed in net10 (source incompatible, ApplicationCore and FunctionalTests)
- `TimeSpan.FromMinutes(double)` source incompatible change
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>` binary incompatible

**Done when**: `dotnet build` succeeds with 0 errors and 0 warnings across the entire solution.

---

### 03-validate: Final Validation

Run the full test suite to confirm the upgrade preserves existing behavior. Execute all 4 test
projects: UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests.

Document any behavioral changes related to `System.Net.Http.HttpContent` (32 occurrences, behavioral)
or `System.Uri` (11 occurrences, behavioral) that manifest during test runs. These are .NET 10
behavioral changes that may cause test failures requiring runtime adjustments.

Commit the completed upgrade to the working branch.

**Done when**: All test projects pass (or failing tests are documented with remediation), solution
builds clean, and changes are committed to the `upgrade/dotnet-net10` branch.
