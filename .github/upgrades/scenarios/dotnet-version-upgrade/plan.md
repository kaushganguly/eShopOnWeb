# .NET 10 Upgrade Plan — eShopOnWeb

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects, all SDK-style, centrally-managed TFM and packages; straightforward modern-to-modern bump with package version updates and a small set of inline API fixes.

**Selected Strategy**: All-At-Once — All 10 projects upgraded simultaneously in a single operation. All projects are on net8.0 with TFM managed centrally in Directory.Build.props and package versions in Directory.Packages.props. A single atomic update covers the entire solution efficiently.

## Tasks

### 01-prerequisites: Verify and update SDK toolchain

Ensure the .NET 10 SDK is available and that `global.json` is updated to pin a .NET 10-compatible SDK version. The current `global.json` pins a .NET 8 SDK; updating it is a prerequisite before any project files are touched so that the toolchain resolves correctly throughout the upgrade.

Also verify the `Directory.Build.props` `TargetFramework` property and all version variables (`AspNetVersion`, `EntityFramworkCoreVersion`, `SystemExtensionVersion`, `VSCodeGeneratorVersion`) are identified for the subsequent update task. No project file changes are made in this task.

**Done when**: `dotnet --version` reports a .NET 10 SDK, `global.json` specifies a .NET 10 SDK, and the solution restores cleanly against the updated toolchain.

### 02-upgrade-solution: Upgrade TFM, packages, and fix breaking API changes

This is the core upgrade task. The TFM is centrally declared in `Directory.Packages.props` (read by all 10 projects), and all package versions are managed there via version variables.

Scope of changes:
- `Directory.Packages.props`: Change `TargetFramework` to `net10.0`; update all version variables and explicit package versions to net10-compatible versions (19 packages need upgrading)
- Remove `System.Security.Claims 4.3.0` — included in framework reference; remove from project files that reference it
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` — no net10-compatible version; Visual Studio tooling package not required for build or runtime
- Upgrade `System.Text.Json` → 10.0.10; `Azure.Identity` → 1.21.0 (security vulnerability fix)
- Fix binary-incompatible `ConfigurationBinder.Get<T>` and `OptionsConfigurationServiceCollectionExtensions.Configure<T>` usages in `src/Web/Configuration/ConfigureWebServices.cs`, `src/Web/Configuration/ConfigureCoreServices.cs`, and `src/PublicApi/Program.cs`
- Fix source-incompatible `System.Exception(SerializationInfo, StreamingContext)` constructor in `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — remove the obsolete serialization constructor override
- Fix source-incompatible `TimeSpan.FromMinutes(double)` in `src/Web/Configuration/ConfigureCookieSettings.cs`
- Build full solution; fix any remaining compilation errors

**Done when**: `dotnet build eShopOnWeb.sln` completes with 0 errors and 0 warnings across all 10 projects, and all 10 projects target `net10.0`.

### 03-validate: Run unit tests and confirm upgrade success

Run the unit test suite to validate runtime behavior is preserved. Execute `tests/UnitTests/` project. Behavioral API changes flagged as `Api.0003` (e.g., `HttpContent.ReadAsStringAsync`, `AddConsole` logging, `UseExceptionHandler`) are non-breaking at the API level but test coverage confirms no regressions.

**Done when**: All unit tests in `tests/UnitTests/` pass with 0 failures. Build remains clean with 0 errors and 0 warnings.
