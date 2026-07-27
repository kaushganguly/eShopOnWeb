# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects (6 source, 4 test), centralized package management via Directory.Packages.props

### Selected Strategy
**All-At-Once** — All 10 projects upgraded simultaneously in a single atomic operation.
**Rationale**: 10 projects all on net8.0, modern-to-modern TFM bump, centralized package management simplifies the upgrade scope.

## Tasks

### 01-prerequisites: Validate .NET 10 SDK and update toolchain configuration

Ensure the .NET 10 SDK is installed and update global.json to pin the correct SDK version. The current global.json pins an 8.0.x SDK — this needs to be updated to a 10.0.x SDK before any project changes can be compiled against net10.0.

Also verify that the .NET 10 SDK is available in the build environment and that the SDK constraint in global.json will be compatible with net10.0 projects.

**Done when**: .NET 10 SDK confirmed available, global.json updated to pin a net10.0-compatible SDK version, `dotnet --version` reports .NET 10.x.

---

### 02-upgrade-all: Upgrade all projects to net10.0 and update all packages

The core upgrade task. Update the centrally-managed TargetFramework from net8.0 to net10.0 in Directory.Packages.props, then update all NuGet package versions to .NET 10 compatible versions.

The following packages need version updates: Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.WebAssembly (and DevServer, Server, Authentication), Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Identity.UI, Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools, Microsoft.Extensions.Identity.Core, Microsoft.Extensions.Logging.Configuration, Microsoft.VisualStudio.Web.CodeGeneration.Design, System.Net.Http.Json, System.Text.Json, and Azure.Identity (security vulnerability fix).

The package Microsoft.VisualStudio.Azure.Containers.Tools.Targets is incompatible with net10.0 and must be removed or replaced. System.Security.Claims (NuGet.0003) should be removed as its functionality is included in the framework reference.

After updating the TFM and packages, fix all compilation errors from API breaking changes flagged in the assessment. Web.csproj and PublicApi.csproj have binary-incompatible APIs (Api.0001). ApplicationCore.csproj and Web.csproj have source-incompatible APIs (Api.0002). Multiple projects have behavioral changes (Api.0003) that may need attention. Deprecated packages (AutoMapper.Extensions.Microsoft.DependencyInjection, System.IdentityModel.Tokens.Jwt, MSTest.TestAdapter, MSTest.TestFramework, xunit, xunit.runner.console) should be reviewed and updated where replacements exist.

**Done when**: Directory.Packages.props updated to net10.0, all Microsoft.* and EF Core packages updated to 10.0.x, incompatible packages resolved, all 10 projects compile without errors, solution builds with 0 errors.

---

### 03-validate: Run full test suite and finalize

Run all four test projects (UnitTests, FunctionalTests, IntegrationTests, PublicApiIntegrationTests) to confirm no regressions were introduced by the upgrade. Fix any test compilation or runtime failures.

Commit all changes to the working branch (upgrade-dotnet-10) once the solution builds cleanly and all tests pass.

**Done when**: `dotnet build` succeeds with 0 errors and 0 warnings on all projects, all tests pass (UnitTests, FunctionalTests, IntegrationTests, PublicApiIntegrationTests), changes committed to upgrade-dotnet-10 branch.
