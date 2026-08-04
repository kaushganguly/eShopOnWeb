# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS)
**Scope**: 10 projects — 4 application/library projects (ApplicationCore, Infrastructure, Web, PublicApi), 2 Blazor projects (BlazorAdmin, BlazorShared), and 4 test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)

### Selected Strategy
**All-At-Once** — All 10 projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0, all Low difficulty, ≤3-tier dependency depth, no CI-green or multi-team constraints. A single atomic pass is the fastest approach with no multi-targeting overhead.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Ensure .NET 10 SDK is available and update the solution's global.json to pin to the .NET 10 SDK. The current global.json specifies `"version": "8.0.x"` with `rollForward: latestFeature` — update to `"version": "10.0.x"` (or the specific latest .NET 10 SDK version installed). Verify the .NET 10 SDK is installed on the machine before proceeding. This task must complete before the main upgrade task since the SDK drives the build.

**Done when**: global.json references .NET 10.0 SDK version, `dotnet --version` or `dotnet sdk check` confirms a .NET 10 SDK is available, and `dotnet restore` on the solution succeeds without SDK version errors.

---

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Update the target framework, package versions, and fix all breaking API changes across all 10 projects. Since the solution uses Central Package Management (Directory.Packages.props), the TargetFramework and all package versions are managed centrally:

- Update `<TargetFramework>net8.0</TargetFramework>` to `net10.0` in Directory.Packages.props
- Update version variables: `<AspNetVersion>8.0.2</AspNetVersion>` → `10.0.x`, `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` → `10.0.x`, `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` → `10.0.x`, `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` → `10.0.x`
- Update individually pinned packages: `Azure.Identity` (security vulnerability, → 1.21.0), `System.Text.Json` (→ 10.0.x), `System.Net.Http.Json` (→ 10.0.x)
- Handle `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0) — this VS tooling package should be removed or conditioned to build targets only
- Remove `System.Security.Claims` package — its functionality is included with the .NET 10 framework reference
- Fix binary-incompatible API changes: `ConfigurationBinder.Get<T>(IConfiguration)`, `ConfigurationBinder.Configure<T>(IServiceCollection, IConfiguration)`, `ConfigurationBinder.GetValue(IConfiguration, Type, string)` — these overloads changed signatures; update call sites in Web and PublicApi
- Fix source-incompatible API changes: `Exception(SerializationInfo, StreamingContext)` constructor removed (obsolete serialization ctor — remove or replace with parameterless ctor + custom ISerializable); `TimeSpan.FromMinutes(double)` became a source-breaking overload — update usages
- Review behavioral changes for `System.Net.Http.HttpContent` (32 occurrences) and `System.Uri` (11 occurrences) — most are low-risk behavioral changes but verify functional behavior is preserved
- Address deprecated packages: `AutoMapper.Extensions.Microsoft.DependencyInjection`, `System.IdentityModel.Tokens.Jwt`, `xunit`/`xunit.runner.console`, `MSTest.TestAdapter`/`MSTest.TestFramework` — check for successor packages or updated major versions compatible with net10.0
- Update `Microsoft.VisualStudio.Web.CodeGeneration.Design` from 8.0.0 → 10.0.x

Affected projects: all 10. Assessment flags 32 code files with incidents, ~61+ LOC to modify.

**Done when**: All projects target net10.0, solution builds with 0 errors and 0 warnings, all unit tests pass (`dotnet test tests/UnitTests`).

---

### 03-final-validation: Full solution build and test validation

Run a complete solution build and the full test suite to confirm all projects upgraded successfully and no regressions were introduced. This task verifies the atomic upgrade is complete and sound.

**Done when**: `dotnet build` on the solution succeeds with 0 errors and 0 warnings, `dotnet test` on UnitTests passes, solution is committed to the working branch.
