# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 (LTS)
**Scope**: 10 projects (4 libraries, 2 web applications, 4 test projects), all SDK-style

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all net8.0, all SDK-style, all 🟢 Low difficulty. Clear dependency structure with ≤ 3 tiers.

## Tasks

### 01-prerequisites: Verify and update SDK prerequisites

Ensure the .NET 10 SDK is available and update the global.json SDK pinning so that the solution uses a compatible toolchain. The solution uses `global.json` to pin the SDK version, which must be updated before any project file changes. Verify that the .NET 10 SDK is installed on the build machine.

Also update the `Directory.Packages.props` `TargetFramework` property used as a central default, and update any MSBuild version variables (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) to their net10.0-compatible values in preparation for the package upgrade task.

**Done when**: `global.json` references a .NET 10-compatible SDK version; `dotnet --version` resolves to a .NET 10 SDK; `Directory.Packages.props` `TargetFramework` property set to net10.0.

---

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Upgrade all 10 projects from net8.0 to net10.0. This is the core atomic upgrade task covering:

**TFM changes**: Update TargetFramework in all 10 project files (BlazorShared, ApplicationCore, Infrastructure, BlazorAdmin, Web, PublicApi, UnitTests, PublicApiIntegrationTests, FunctionalTests, IntegrationTests). Note that BlazorAdmin and BlazorShared are Blazor WebAssembly projects and may use `net10.0` as TFM, but double-check for `netstandard2.1` or `blazorwasm` TFM variants.

**Package updates**: Update all packages to net10.0-compatible versions using the Central Package Management file (Directory.Packages.props). Key packages requiring version bumps: Microsoft.AspNetCore.* (8.0.2 → 10.x), Microsoft.EntityFrameworkCore.* (8.0.2 → 10.x), Microsoft.Extensions.* (8.0.x → 10.x), Azure.Identity (1.10.4 → 1.21.0, security vulnerability), Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.0 → 10.x). Remove or replace incompatible package `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 compatible version). Remove `System.Security.Claims` (now included in framework). Address deprecated packages: `AutoMapper.Extensions.Microsoft.DependencyInjection`, `System.IdentityModel.Tokens.Jwt`, `xunit` 2.x, `xunit.runner.console`.

**API breaking changes to fix inline**:
- `ConfigurationBinder.Get<T>(IConfiguration)` → binary incompatible (4 occurrences in FunctionalTests)
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` → binary incompatible (4 occurrences in PublicApi, Web)
- `Exception(SerializationInfo, StreamingContext)` constructor → source incompatible in ApplicationCore (2 occurrences)
- `TimeSpan.FromMinutes(double)` → source incompatible (1 occurrence)
- `Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue(IConfiguration, Type, string)` → binary incompatible (1 occurrence)
- Behavioral changes in `HttpContent`, `System.Uri`, `ILoggingBuilder.AddConsole` — review at runtime; may not require code changes

**Done when**: All 10 projects target net10.0; solution restores without errors; solution builds with 0 errors and 0 warnings in modified projects; all incompatible/deprecated packages resolved.

---

### 03-validate: Final validation — build and unit tests

Run the full solution build and unit test suite to confirm the upgrade is complete and correct. Fix any remaining compilation warnings that were introduced during the upgrade. Document any behavioral API changes that require runtime observation (HttpContent, System.Uri) in the project README or a known-issues note.

**Done when**: `dotnet build` succeeds with 0 errors on the full solution; `dotnet test tests/UnitTests/UnitTests.csproj` passes with 0 failures; no build warnings in upgraded projects.
