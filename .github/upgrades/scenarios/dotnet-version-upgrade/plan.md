# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects — 4 src libraries (ApplicationCore, BlazorAdmin, BlazorAdmin, Infrastructure), 3 src apps (BlazorAdmin, PublicApi, Web), 4 test projects; centralized package management via Directory.Packages.props

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), no .NET Framework migration required, dependency graph is 5 tiers deep but package management is centralized (CPM), so the TFM and version bumps can be applied atomically.

---

## Tasks

### 01-prerequisites: Verify .NET 10 SDK and update global.json

Verify that the .NET 10 SDK is available in the build environment. The `global.json` file currently pins the SDK to `8.0.x` with `rollForward: latestFeature` — this must be updated to allow the .NET 10 SDK. This task must complete before any TFM changes are made.

The `global.json` at the repo root needs the `sdk.version` changed from `8.0.x` to `10.0.x` (or a specific .NET 10 SDK version if pinning is needed). Validate that the installed SDK satisfies the new version constraint before proceeding.

**Done when**: `dotnet --version` shows a .NET 10 SDK, and `global.json` references a .NET 10 SDK version.

---

### 02-upgrade-projects: Upgrade all 10 projects to net10.0

Update all 10 projects to target `net10.0`. The project uses centralized package management (`ManagePackageVersionsCentrally=true`) with `Directory.Packages.props` — this is the primary location for version updates. Individual `.csproj` files do not pin package versions.

**Scope of changes:**

`Directory.Packages.props`: Update the `<TargetFramework>` property from `net8.0` to `net10.0`. Update the version variables: `AspNetVersion` → `10.0.10`, `EntityFramworkCoreVersion` → `10.0.10`, `SystemExtensionVersion` → `10.0.10`, `VSCodeGeneratorVersion` → `10.0.2`. Update `System.Text.Json` from `8.0.3` to `10.0.10`. Update `Azure.Identity` from `1.10.4` to `1.21.0` (security vulnerability fix). Remove `System.Security.Claims` version entry (NuGet.0003 — functionality is now part of the framework reference). Remove or update `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (NuGet.0001 — incompatible with net10.0). Handle deprecated packages: `AutoMapper.Extensions.Microsoft.DependencyInjection`, `System.IdentityModel.Tokens.Jwt`, `xunit`/`MSTest` deprecated versions.

**API breaking changes to fix:**
- `Api.0001` (binary incompatible, 9 occurrences) in `src/PublicApi/PublicApi.csproj` and `src/Web/Web.csproj` — inspect assessment detail for affected types/members and fix consuming code
- `Api.0002` (source incompatible, 3 occurrences) in `src/ApplicationCore/ApplicationCore.csproj` and `src/Web/Web.csproj` — requires code changes to compile after the TFM bump
- `Api.0003` (behavioral changes, 49 occurrences across BlazorAdmin, FunctionalTests, PublicApi, PublicApiIntegrationTests, Web) — review behavioral changes and address any that affect correctness

All 10 `.csproj` files may need their `<TargetFramework>` updated if they override the central property. The central `Directory.Packages.props` `<TargetFramework>` is the primary control point.

**Done when**: All 10 projects show `net10.0` as their target framework, `dotnet restore` succeeds with no errors, and `dotnet build` compiles the solution with 0 errors and 0 warnings.

---

### 03-final-validation: Validate build and run unit tests

After all projects are upgraded, perform a clean full-solution build and run the unit test suite to verify the upgrade is functionally correct.

Run `dotnet build` on the solution and confirm 0 errors and 0 warnings. Run `dotnet test` on all projects that contain tests (UnitTests, IntegrationTests where applicable). Review any test failures — behavioral changes from `Api.0003` may surface as test failures even if the build succeeds.

**Done when**: Solution builds with 0 errors and 0 warnings; all unit tests pass (UnitTests project); no regressions introduced by the upgrade.
