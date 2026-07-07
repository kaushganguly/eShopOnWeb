# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0)
**Scope**: 10 projects — 3 web/API apps, 3 libraries, 4 test projects; all SDK-style with centralized package management (Directory.Packages.props)

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single atomic operation.
**Rationale**: 10 projects, all on modern .NET (net8.0), all SDK-style, centralized package management. TargetFramework is defined once in Directory.Packages.props, making a single-file update sufficient for all projects.

```
Applications:  [Web]  [PublicApi]
Libraries:     [ApplicationCore] [Infrastructure] [BlazorAdmin] [BlazorShared]
Tests:         [FunctionalTests] [IntegrationTests] [PublicApiIntegrationTests] [UnitTests]
```

## Tasks

### 01-prerequisites: Verify SDK and update global.json

The project uses a global.json pinned to the 8.0.x SDK. Before upgrading, verify that the .NET 10 SDK is installed and update global.json to target it. This unblocks all subsequent build operations.

**Done when**: global.json references .NET 10 SDK (10.x), and `dotnet --version` returns a 10.x version.

---

### 02-upgrade-all-projects: Upgrade all projects to .NET 10

All 10 projects use centralized package management via Directory.Packages.props, where `TargetFramework` and version variables are defined. Update this single file to set net10.0 and update all package versions. Then remove incompatible package references from individual project files and fix API breaking changes in source code.

**Scope**: All 10 projects — Directory.Packages.props controls TFM and package versions centrally.

**Key changes required**:

1. **Directory.Packages.props** — single file governing all projects:
   - Set `TargetFramework` to `net10.0`
   - Set `AspNetVersion` to `10.0.9`
   - Set `EntityFramworkCoreVersion` to `10.0.9`
   - Set `SystemExtensionVersion` to `10.0.9`
   - Set `VSCodeGeneratorVersion` to `10.0.2`
   - Update `Azure.Identity` from `1.10.4` to `1.21.0` (security vulnerability fix)
   - Update `System.Text.Json` from `8.0.3` to `10.0.9`
   - Update `System.IdentityModel.Tokens.Jwt` from `7.3.1` to `8.19.1` (deprecated)
   - Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no supported version)
   - Remove `System.Security.Claims` (now included in the framework reference)
   - Remove `Microsoft.AspNetCore.Mvc 2.2.0` (included in framework)

2. **src/PublicApi/PublicApi.csproj** — remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
3. **src/ApplicationCore/ApplicationCore.csproj** — remove `System.Security.Claims` PackageReference

4. **API breaking changes** — fix these source-level issues:
   - `ConfigurationBinder.Get<T>(IConfiguration)` (binary incompatible) in `src/Web/Configuration/ConfigureCoreServices.cs:21`, `src/PublicApi/Program.cs:42,49` — use `configuration.Get<T>()` with updated overload
   - `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` (binary incompatible) in `src/Web/Configuration/ConfigureWebServices.cs:15`, `src/PublicApi/Program.cs:41,48` — verify against .NET 10 breaking changes
   - `TimeSpan.FromMinutes(double)` (source incompatible) in `src/Web/Configuration/ConfigureCookieSettings.cs:25` — use `TimeSpan.FromMinutes(int)` or cast appropriately
   - Behavioral changes in `HttpContent.ReadAsStringAsync()` and `ConsoleLoggerExtensions.AddConsole()` — verify behavior is still correct

**Done when**: Solution builds without errors or warnings in modified projects; all 10 projects target net10.0.

---

### 03-final-validation: Run full test suite

Execute the complete test suite to verify that the upgraded solution is functionally correct. All existing tests should pass with no regressions.

**Done when**: All test projects build and all tests pass (or pre-existing failures are documented).
