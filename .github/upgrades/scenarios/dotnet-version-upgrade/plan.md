# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0  
**Scope**: 10 projects (6 source, 4 test), centrally managed TFM and packages via Directory.Build.props / Directory.Packages.props

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.  
**Rationale**: 10 projects, all on net8.0, TargetFramework centrally managed in Directory.Build.props (single-line change covers all projects), Central Package Management already in use.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Verify that the .NET 10 SDK is available on the machine and update global.json to pin the SDK to 10.0.x. The current global.json pins `8.0.x` with `rollForward: latestFeature`. The .NET 10 SDK (10.0.302) is confirmed installed. The SDK version constraint must be updated so the toolchain picks up .NET 10 features.

No source code changes are needed in this task — the only file touched is `global.json`.

**Done when**: `global.json` references the .NET 10 SDK version string (e.g., `10.0.x` or the pinned SDK version), and `dotnet --version` resolves to a .NET 10 SDK.

---

### 02-upgrade: Update TFM, packages, and fix breaking API changes

This is the core upgrade task covering all 10 projects in a single pass. All projects share a centrally managed TargetFramework in `Directory.Build.props` and package versions in `Directory.Packages.props`, so the scope of file changes is well-contained.

**TFM update** (`Directory.Build.props`): Change `<TargetFramework>net8.0</TargetFramework>` to `net10.0`. Update the `AspNetVersion`, `SystemExtensionVersion`, `EntityFrameworkCoreVersion`, and `VSCodeGeneratorVersion` property variables to their .NET 10-compatible values.

**Package updates** (`Directory.Packages.props`): 19 packages have recommended upgrades — mainly `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*` packages from `8.0.x` → `10.0.10`, and `Microsoft.VisualStudio.Web.CodeGeneration.Design` to `10.0.2`. Additionally:
- `Azure.Identity` must be upgraded from `1.10.4` → `1.21.0` (security vulnerability)
- `System.IdentityModel.Tokens.Jwt` should be upgraded from `7.3.1` → `8.19.2` (deprecated, upgrade recommended)
- `System.Text.Json` and `System.Net.Http.Json` → `10.0.10`
- `System.Security.Claims` must be **removed** (functionality included in framework reference — NuGet.0003)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` must be **removed** — incompatible with net10.0, no supported version exists (NuGet.0001)

**Breaking API changes** to fix in source code (mandatory issues):
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` (Api.0001) — binary incompatible. Affected files: `src/Web/Configuration/ConfigureWebServices.cs:15` and `src/PublicApi/Program.cs:48`. Replace `services.Configure<T>(configuration)` with `services.Configure<T>(configuration.GetSection(nameof(T)))` or bind to a specific named section.
- `ConfigurationBinder.Get<T>(IConfiguration)` (Api.0001) — binary incompatible. Affected files: `src/Web/Configuration/ConfigureCoreServices.cs:21` and `src/PublicApi/Program.cs:42,49`. The non-nullable overload changed; update call sites to use the correct overload.
- `TimeSpan.FromMinutes(double)` (Api.0002) — source incompatible. Affected file: `src/Web/Configuration/ConfigureCookieSettings.cs:25`. Verify usage and fix if compilation fails.

Behavioral changes (Api.0003, potential — monitor, fix only if tests fail): `HttpContent.ReadAsStringAsync()`, `UseExceptionHandler(string)`, `AddConsole()`, `LoggingBuilder.AddConsole` in health check files and `Program.cs`.

**Done when**: The solution restores, builds with 0 errors and 0 warnings across all 10 projects, and all tests pass (unit, integration, functional, API integration).

---

### 03-validation: Full solution build and test verification

Run the complete test suite after the core upgrade task. This task serves as the final checkpoint: confirm zero build errors, zero build warnings in modified projects, and all tests green. Document any deferred recommendations (e.g., deprecated-but-compatible packages that can be upgraded in a follow-up) in a brief note.

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors; all test projects pass via `dotnet test eShopOnWeb.sln`.
