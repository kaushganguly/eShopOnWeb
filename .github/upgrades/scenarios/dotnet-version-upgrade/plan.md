# .NET Version Upgrade Plan — eShopOnWeb

## Overview

**Target**: Upgrade all projects from net8.0 to net10.0
**Scope**: 10 projects (3 ASP.NET Core apps, 4 class libraries, 3 test projects), all SDK-style on modern .NET

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all net8.0 (modern .NET), straightforward TFM bump with centralized package management (Directory.Packages.props).

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Verify that .NET 10 SDK is installed and update global.json to target the .NET 10 SDK. The repo uses a global.json file that pins the SDK version, currently pointing to 8.0.x. This task ensures the toolchain is ready before any project changes begin.

Check for installed .NET 10 SDK via `dotnet --list-sdks`. If absent, stop and report missing SDK. Update `global.json` to set `sdk.version` to a .NET 10.0.x release. Confirm `dotnet --version` reports a 10.0.x SDK after the change.

**Done when**: `global.json` references a .NET 10.0.x SDK version and `dotnet --version` confirms the SDK is active.

---

### 02-core-upgrade: Upgrade all projects to net10.0

Update the central framework target and all NuGet packages across all 10 projects. The solution uses Central Package Management (CPM) via `Directory.Packages.props`, meaning TFM and package versions are managed in one place.

**Framework**: Change `<TargetFramework>net8.0</TargetFramework>` to `net10.0` in `Directory.Packages.props` (or in any project file where TFM is overridden). Verify each of the 10 project files targets the right TFM after the change.

**Package updates** (in `Directory.Packages.props`): Update all versioned `PackageVersion` entries for Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, Microsoft.Extensions.*, System.Net.Http.Json, and System.Text.Json from 8.0.x to 10.0.9. Update Azure.Identity from 1.10.4 to 1.21.0 (security vulnerability fix). Update Microsoft.VisualStudio.Web.CodeGeneration.Design from 8.0.0 to 10.0.2.

**Package removals**: Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 from `src/PublicApi/PublicApi.csproj` — it is incompatible with .NET 10 and has no supported replacement version. Remove the `System.Security.Claims` 4.3.0 `PackageVersion` entry — its functionality is now included in the framework reference and the explicit package must be removed.

**API breaking changes**: The following `Microsoft.Extensions.Configuration` and `Microsoft.Extensions.DependencyInjection` APIs changed between .NET 8 and .NET 10 (marked binary incompatible by the assessment):
- `ConfigurationBinder.Get<T>()` — called in `src/Web/Configuration/ConfigureCoreServices.cs:21`, `src/Web/Program.cs:98`, `src/PublicApi/Program.cs:42`, `src/PublicApi/Program.cs:49`
- `ConfigurationBinder.GetValue(typeof(string), key)` — called in `src/Web/Program.cs:140`
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>(services, configuration)` — called in `src/Web/Configuration/ConfigureWebServices.cs:15`, `src/Web/Program.cs:97`, `src/PublicApi/Program.cs:41`, `src/PublicApi/Program.cs:48`

Research the exact .NET 10 breaking change for each API, apply the recommended code fix inline, and confirm the solution compiles.

**Deprecated packages** (optional/low priority): `AutoMapper.Extensions.Microsoft.DependencyInjection`, `System.IdentityModel.Tokens.Jwt`, `xunit`, `xunit.runner.console` are flagged deprecated — assess if replacements are needed for the build to succeed; if they still compile and function, note them as follow-up items rather than blocking the upgrade.

After all changes, restore packages and build the full solution. Fix any remaining compilation errors.

**Done when**: All 10 projects target net10.0, all Microsoft.* package versions updated to 10.0.9, Azure.Identity updated to 1.21.0, incompatible packages removed, API breaking changes fixed, and `dotnet build eShopOnWeb.sln` exits with 0 errors and 0 warnings in modified projects.

---

### 03-test-validation: Run unit tests and confirm passing

Run the unit test suite to confirm functional correctness after the upgrade. This task runs after the solution successfully builds.

Execute `dotnet test` targeting the test projects in scope: `tests/UnitTests`, `tests/IntegrationTests`. Investigate and fix any test failures caused by the .NET 10 upgrade (behavioral API changes, package changes, or test infrastructure issues). Document any tests that cannot pass due to environmental constraints (e.g., integration tests requiring a database) as known skips.

**Done when**: `dotnet test tests/UnitTests/UnitTests.csproj` passes with 0 failures. Integration test failures are either resolved or documented with clear justification.
