# .NET Version Upgrade Plan — eShopOnWeb

## Overview

**Target**: Upgrade all 10 projects from net8.0 to net10.0
**Scope**: 10 projects — 3 ASP.NET Core apps (Web, PublicApi, BlazorAdmin), 3 class libraries (ApplicationCore, Infrastructure, BlazorShared), 4 test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET). Straightforward modern-to-modern upgrade with no Framework→Core boundary crossing. CPM already in use via Directory.Packages.props, making batch package updates clean.

## Tasks

### 01-prerequisites: Verify SDK and Update global.json

Confirm the .NET 10 SDK is available in the build environment and update `global.json` to reference the .NET 10 SDK. The current `global.json` specifies a .NET 8 SDK; this needs to be updated before any project files are changed so the build toolchain resolves correctly.

The solution uses Central Package Management (`Directory.Packages.props`) with version variables (`AspNetVersion`, `EntityFramworkCoreVersion`, `SystemExtensionVersion`, `VSCodeGeneratorVersion`). Confirm these variables will be updated in the next task.

**Done when**: `dotnet --version` reports a .NET 10.x SDK; `global.json` (if present) references the .NET 10 SDK; solution restores without SDK resolution errors.

---

### 02-upgrade-projects: Upgrade All Projects to net10.0

Update the central `TargetFramework` property in `Directory.Packages.props` from `net8.0` to `net10.0`, and update all package version variables and `PackageVersion` entries to their net10.0-compatible versions.

**Package updates required** (via Directory.Packages.props):
- `AspNetVersion` variable: `8.0.2` → `10.0.10` (affects: Microsoft.AspNetCore.*, Microsoft.Extensions.Identity.Core, Microsoft.AspNetCore.Mvc.Testing)
- `EntityFramworkCoreVersion` variable: `8.0.2` → `10.0.10` (affects: Microsoft.EntityFrameworkCore.* packages)
- `SystemExtensionVersion` variable: `8.0.0` → `10.0.10` (affects: System.Net.Http.Json, Microsoft.Extensions.Logging.Configuration)
- `VSCodeGeneratorVersion` variable: `8.0.0` → `10.0.2` (Microsoft.VisualStudio.Web.CodeGeneration.Design)
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)

**Incompatible package to resolve** (Api.0001 in PublicApi):
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` v1.19.6: incompatible with net10.0 — research whether a compatible version exists; if not, remove from PublicApi.csproj or Web.csproj where referenced.

**Package cleanup**:
- `System.Security.Claims` v4.3.0: functionality included in framework reference — remove from consuming projects.

**API breaking changes to fix** (Api.0001/Api.0002 in Web.csproj and PublicApi.csproj):
- Investigate each binary/source-incompatible API flagged in the assessment for Web and PublicApi projects.
- Apply fixes inline — rename, replace, or rewrite affected code.

**API behavioral changes** (Api.0003 — potential issues):
- Review behavioral changes flagged across BlazorAdmin, FunctionalTests, PublicApi, PublicApiIntegrationTests, Web.
- Assess whether behavioral differences require code adjustments.

After all edits, restore and build the entire solution. Fix all compilation errors before marking complete.

**Done when**: All 10 projects target `net10.0`; `Directory.Packages.props` uses net10.0-compatible package versions; solution builds with 0 errors and 0 warnings in modified projects; incompatible package removed or replaced.

---

### 03-final-validation: Run Tests and Confirm Upgrade

Run the full test suite (UnitTests, IntegrationTests) to confirm no regressions were introduced. Address any test failures introduced by the .NET 10 upgrade (behavioral changes, changed defaults, etc.).

Document any deferred items (optional deprecated packages like xunit, MSTest, AutoMapper) that do not block the build or tests, as post-upgrade recommendations.

**Done when**: `dotnet build` reports 0 errors across all projects; all unit tests pass; integration tests pass (or failures are documented with explanation); upgrade is committed to the working branch.
