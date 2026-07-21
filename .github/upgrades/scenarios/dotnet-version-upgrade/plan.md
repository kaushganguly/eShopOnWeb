# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb solution from net8.0 to net10.0 LTS
**Scope**: 10 projects — 4 source libraries/apps (BlazorShared, ApplicationCore, BlazorAdmin, Infrastructure), 2 web applications (Web, PublicApi), 4 test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET). Per strategy defaults for ≤15 projects with ambiguous depth signals, All-at-Once applies. Single atomic pass avoids multi-targeting overhead.

---

## Tasks

### 01-prerequisites: Verify .NET 10 SDK and update global.json

Confirm that the .NET 10 SDK is installed and meets the requirements for targeting net10.0. Update `global.json` to specify the .NET 10 SDK version (replacing the current net8.0 SDK pin). This must complete before any project file changes are made, since SDK version constraints in global.json can prevent the build from recognizing net10.0 targets.

Check `global.json` in the repository root. Update the `sdk.version` value to a .NET 10 SDK version that is installed on the build machine. Confirm the SDK is present by running `dotnet --list-sdks`.

**Done when**: `global.json` specifies a .NET 10 SDK version and `dotnet --version` confirms the .NET 10 SDK is active in the repo directory.

---

### 02-upgrade-all-projects: Upgrade all 10 projects to net10.0

Update every project in the solution to target net10.0. This is the core upgrade task covering TFM changes, NuGet package updates, removal or replacement of incompatible packages, and any code fixes required for breaking API changes.

**Projects in scope**: BlazorShared, ApplicationCore, BlazorAdmin, Infrastructure, Web, PublicApi, FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests. The `Directory.Packages.props` file (CPM already in use) must be updated to set net10.0-compatible package versions.

**Assessment highlights** (119 total issues, 21 mandatory):
- **TFM change** (all 10 projects): Update `<TargetFramework>` from `net8.0` to `net10.0` in each `.csproj` and in `Directory.Packages.props` if it sets a TFM.
- **Package upgrades** (19 packages): Microsoft.AspNetCore.* packages (currently 8.0.2 → 10.0.x), Microsoft.EntityFrameworkCore.* (8.0.2 → 10.0.x), Microsoft.Extensions.* (8.0.x → 10.0.x), System.Text.Json (8.0.3 → 10.0.x), System.Net.Http.Json (8.0.0 → 10.0.x), Azure.Identity (security vulnerability fix: 1.10.4 → 1.21.0), Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.0 → 10.0.x).
- **Incompatible package** (1): `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 has no net10.0-compatible version — remove it or replace with compatible tooling.
- **Deprecated packages** (6): `AutoMapper.Extensions.Microsoft.DependencyInjection`, `MSTest.TestAdapter`, `MSTest.TestFramework`, `System.IdentityModel.Tokens.Jwt`, `xunit`, `xunit.runner.console` — evaluate upgrades; xunit v3 or MSTest SDK may be needed.
- **Framework-included package** (1): `System.Security.Claims` 4.3.0 is now included in the framework reference — remove the explicit package reference.
- **Binary-incompatible APIs** (Api.0001 in PublicApi, Web): Research which specific BCL/ASP.NET APIs were removed between net8 and net10, and fix call sites.
- **Source-incompatible APIs** (Api.0002 in ApplicationCore, Web): Identify and update any APIs that changed signatures between net8 and net10.
- **Behavioral changes** (Api.0003 in BlazorAdmin, FunctionalTests, PublicApi, PublicApiIntegrationTests, Web): Identify behavioral API changes and adjust code accordingly.

After updating all project files and packages, restore dependencies (`dotnet restore`) and then build the full solution. Fix all compilation errors in a single bounded pass before proceeding.

**Done when**: All 10 projects have `<TargetFramework>net10.0</TargetFramework>`, all package references are updated to net10.0-compatible versions, `dotnet build eShopOnWeb.sln` reports 0 errors and 0 warnings in modified projects.

---

### 03-final-validation: Run tests and verify full solution health

Run the complete test suite against the upgraded solution to confirm no functional regressions were introduced. This covers all four test projects: UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests.

Document any packages that remain deprecated (e.g., xunit v2 → v3 migration deferred) as follow-up recommendations. Verify all security vulnerabilities (Azure.Identity CVE) have been addressed by the package upgrades.

**Done when**: `dotnet test eShopOnWeb.sln` passes all tests (or all previously-passing tests pass), with 0 test failures. Security vulnerability for Azure.Identity resolved.

---
