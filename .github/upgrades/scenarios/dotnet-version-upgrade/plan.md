# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 (LTS)
**Scope**: 10 projects — 6 application projects (ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, PublicApi, Web) and 4 test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), TFM managed centrally in Directory.Packages.props. Upgrading the central config updates all projects atomically.

---

## Tasks

### 01-prerequisites: Verify SDK and toolchain for net10.0

Verify that the .NET 10 SDK is installed and that `global.json` is compatible with net10.0. The current `global.json` pins the SDK to a .NET 8 version (`8.0.x`) which must be updated to a .NET 10 SDK version (`10.0.x`). This task also validates that the SDK version in global.json matches an actually-installed version.

No source code changes — only toolchain validation and global.json update.

**Done when**: `dotnet --version` reports a .NET 10 SDK, `global.json` specifies a .NET 10 SDK version, and `dotnet restore` succeeds on the solution.

---

### 02-upgrade-all: Upgrade all projects to net10.0

Update `Directory.Packages.props` to set `<TargetFramework>net10.0</TargetFramework>` and bump all version variables and package versions to their .NET 10-compatible equivalents. This single file change upgrades all 10 projects since TFM is centrally managed.

Key changes in Directory.Packages.props:
- `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- `<AspNetVersion>` → `10.0.x` (affects 10 Microsoft.AspNetCore.* packages)
- `<EntityFramworkCoreVersion>` → `10.0.x` (affects 3 EF Core packages)
- `<SystemExtensionVersion>` → `10.0.x`
- `<VSCodeGeneratorVersion>` → `10.0.x`
- `Azure.Identity` → upgrade from 1.10.4 to 1.21.0 (security vulnerability)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` → remove (incompatible with net10.0, no compatible version available)
- `System.Security.Claims` → remove (functionality included in net10.0 framework reference)
- `System.Text.Json` → 10.0.x
- `System.Net.Http.Json` → 10.0.x

After central config updates, fix all breaking API changes across affected projects. The assessment flagged mandatory API issues in:
- `Web.csproj` — 6 mandatory issues (binary and source incompatible APIs, behavioral changes); also has deprecated packages
- `PublicApi.csproj` — 6 mandatory issues (binary incompatible APIs, behavioral changes); also has deprecated packages
- `ApplicationCore.csproj` — 2 mandatory issues (source incompatible APIs)
- `BlazorAdmin.csproj` — 1 mandatory issue (behavioral change)
- `BlazorShared.csproj` — 1 mandatory issue (TFM change only, no code issues)
- Test projects — 1 mandatory issue each (TFM change and/or deprecated packages)

Also handle the `xunit` and `xunit.runner.console` deprecated packages in test projects, and `AutoMapper.Extensions.Microsoft.DependencyInjection` and `System.IdentityModel.Tokens.Jwt` deprecated packages.

Research starting points: query assessment per-project for specific API rule violations (Api.0001 binary incompatible, Api.0002 source incompatible) to identify exact APIs needing changes before modifying code.

**Done when**: All projects target net10.0, all package versions are .NET 10-compatible, `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` and `System.Security.Claims` are removed from Directory.Packages.props and individual project files, Azure.Identity vulnerability is patched, and solution builds with zero errors and zero warnings in all modified projects.

---

### 03-validation: Final build and test suite validation

Run the full solution build to confirm zero errors and zero warnings. Execute the unit test suite (`UnitTests`) to confirm all tests pass. Verify that Blazor WebAssembly projects (BlazorAdmin, BlazorShared) are compatible with .NET 10 WASM.

Document any packages that remain deprecated but have no compatible replacement (e.g., xunit if no upgrade path is available) as recommendations for future action.

**Done when**: `dotnet build` reports 0 errors and 0 warnings across the solution, `dotnet test` for UnitTests reports all tests passing, and any non-resolvable deprecation warnings are documented.

---
