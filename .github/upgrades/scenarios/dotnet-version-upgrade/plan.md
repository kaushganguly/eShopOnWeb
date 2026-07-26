# .NET Version Upgrade Plan: net8.0 → net10.0

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects — 6 production (ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web) and 4 test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests). All projects are SDK-style with Central Package Management already in place.

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), SDK-style with CPM in place. One incompatible package to resolve inline. Straightforward TFM bump with package version updates and minor API fixes.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Verify that the .NET 10 SDK is installed and compatible. Update `global.json` to specify a net10.0 SDK version. Confirm that `Directory.Packages.props` uses the correct property names and variable structure for the target version bump. This task sets the baseline toolchain configuration before any project files are modified.

The current `global.json` specifies the .NET SDK version constraint — it must be updated to require a .NET 10 SDK. The `Directory.Packages.props` file defines version variables (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) that centrally drive most package versions — updating these properties is the primary lever for the package upgrade in subsequent tasks.

**Done when**: `global.json` specifies a net10.0-compatible SDK version, .NET 10 SDK is confirmed installed, and `Directory.Packages.props` structure is reviewed and ready for version updates.

---

### 02-core-libraries: Upgrade foundation libraries

Upgrade BlazorShared, ApplicationCore, and Infrastructure from net8.0 to net10.0. These are the foundation libraries that all other projects depend on, so they must be upgraded first even in an all-at-once strategy to establish a clean build baseline.

**BlazorShared** (`src/BlazorShared`): Pure class library with 1 mandatory issue (TFM change only). Lowest risk.

**ApplicationCore** (`src/ApplicationCore`): Class library with 6 issues (2 mandatory). Has source-incompatible APIs flagged — check for BCL API changes between net8.0 and net10.0 in the project's code. Also has a security vulnerability in `Azure.Identity` (upgrade from 1.10.4 → 1.21.0) and `System.Security.Claims` (4.3.0 → included in framework, remove explicit reference).

**Infrastructure** (`src/Infrastructure`): Class library with 5 issues (1 mandatory). Package upgrades for EF Core (`Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`) and deprecated packages to address.

Update `TargetFramework` in each `.csproj` to `net10.0`. Update `Directory.Packages.props` to bump the central version variables for all packages. Fix any source-incompatible API usages found in ApplicationCore.

**Done when**: BlazorShared, ApplicationCore, and Infrastructure all target net10.0, build successfully with 0 errors and 0 warnings, and all mandatory issues from the assessment are resolved.

---

### 03-applications: Upgrade web applications and Blazor frontend

Upgrade BlazorAdmin, PublicApi, and Web from net8.0 to net10.0. These are the most complex projects with the highest number of API compatibility issues.

**BlazorAdmin** (`src/BlazorAdmin`): WebAssembly Blazor application with 11 issues (1 mandatory — TFM change). Has behavioral change flagged. Upgrade `Microsoft.AspNetCore.Components.WebAssembly.*` packages to net10.0 versions.

**PublicApi** (`src/PublicApi`): ASP.NET Core API project with 17 issues (6 mandatory). Has binary-incompatible APIs — `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (1.19.6, incompatible with net10.0 — remove or update the reference). Also has deprecated `Swashbuckle.AspNetCore` (check compatibility with net10.0), deprecated `System.IdentityModel.Tokens.Jwt` (7.3.1), and `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated). Update `Microsoft.AspNetCore.Authentication.JwtBearer` to 10.0.x. Investigate and fix binary-incompatible API usages.

**Web** (`src/Web`): Main ASP.NET Core MVC web application with 29 issues (6 mandatory). Has both binary-incompatible and source-incompatible APIs. Also has `Azure.Identity` security vulnerability. Remove deprecated `Microsoft.VisualStudio.Web.CodeGeneration.Design` reference or upgrade to 10.0.x. Fix all incompatible API usages.

Update `TargetFramework` in each `.csproj` to `net10.0`. Fix breaking API changes inline. Resolve the incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` package (remove or conditionally exclude for net10.0).

**Done when**: BlazorAdmin, PublicApi, and Web all target net10.0, build successfully with 0 errors and 0 warnings, and all mandatory issues are resolved.

---

### 04-test-projects: Upgrade test projects

Upgrade all four test projects to net10.0: FunctionalTests, IntegrationTests, PublicApiIntegrationTests, and UnitTests.

**FunctionalTests** (`tests/FunctionalTests`): 33 issues (1 mandatory). Depends on ApplicationCore, PublicApi, and Web. Has behavioral change flagged. Uses `Microsoft.AspNetCore.Mvc.Testing` — upgrade to 10.0.x. Has deprecated xunit packages (2.7.0) — check if upgrade to xunit v3 is needed or if 2.7.0 still works with net10.0.

**IntegrationTests** (`tests/IntegrationTests`): 3 issues (1 mandatory). Depends on Infrastructure and UnitTests. Upgrade `Microsoft.EntityFrameworkCore.InMemory` to 10.0.x.

**PublicApiIntegrationTests** (`tests/PublicApiIntegrationTests`): 11 issues (1 mandatory). Depends on PublicApi and Web. Has behavioral change flagged. Upgrade `Microsoft.AspNetCore.Mvc.Testing` to 10.0.x.

**UnitTests** (`tests/UnitTests`): 3 issues (1 mandatory). Depends on ApplicationCore and Web. Deprecated packages: MSTest.TestAdapter, MSTest.TestFramework, xunit, xunit.runner.console — update these.

Update `TargetFramework` in each test `.csproj` to `net10.0`. Address deprecated test framework packages.

**Done when**: All 4 test projects target net10.0 and build successfully with 0 errors and 0 warnings.

---

### 05-final-validation: Full solution build and test suite

Perform a full solution build to confirm all 10 projects compile cleanly on net10.0. Run the complete test suite (unit, integration, and functional tests) and ensure all tests pass.

Verify no warnings remain in any modified project. Confirm that `Directory.Packages.props` version variables are consistent and there are no version conflicts. Document any deferred recommendations (e.g., upgrading deprecated xunit packages to v3, switching from AutoMapper to manual mapping).

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors and 0 warnings. All unit and integration tests pass. `dotnet test` exits with code 0.
