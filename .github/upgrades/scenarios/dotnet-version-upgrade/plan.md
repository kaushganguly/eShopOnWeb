# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects (6 source, 4 test), all on net8.0 SDK-style format

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single atomic operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), mechanical TFM bump. Small scope qualifies for All-at-Once under the ≤15 project rule.

## Tasks

### 01-prerequisites: Verify .NET 10 SDK and update global.json

Verify that the .NET 10 SDK is installed and accessible on the build machine. Update `global.json` to reference the .NET 10 SDK version so the entire solution uses the correct toolchain. This is a prerequisite gate — if the SDK is not available, all subsequent tasks will fail.

The project currently uses a `global.json` that pins an SDK version. Update the `sdk.version` field to a .NET 10 compatible version (e.g., `10.0.100` or the latest available 10.x SDK). Also verify that the `rollForward` policy is appropriate for .NET 10.

**Done when**: `dotnet --version` resolves to a .NET 10.x SDK, and `global.json` specifies a .NET 10 SDK version with no SDK resolution errors.

---

### 02-upgrade-all-projects: Upgrade all projects to net10.0 with package updates

Upgrade all 10 projects from net8.0 to net10.0 in a single atomic pass. This is the core upgrade task covering three interconnected areas:

**TFM updates**: Update `TargetFramework` in `Directory.Packages.props` (currently `net8.0`) and in each individual project file to `net10.0`. All projects (BlazorShared, ApplicationCore, Infrastructure, BlazorAdmin, Web, PublicApi, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) are updated together.

**Package updates**: The solution uses Central Package Management (CPM) via `Directory.Packages.props`. Update all version properties and `PackageVersion` entries: Microsoft ASP.NET Core packages (8.0.x → 10.0.x), Entity Framework Core packages (8.0.x → 10.0.x), Microsoft.Extensions packages (8.0.x → 10.0.x), Azure.Identity (security vulnerability fix: 1.10.4 → 1.21.0), and Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.0 → 10.0.x). Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no net10.0 version available). Remove `System.Security.Claims` (functionality included in the framework reference). Handle deprecated packages: `System.IdentityModel.Tokens.Jwt` (deprecated — check if direct usage exists or can be dropped), `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated — keep for now unless replaced), xunit/MSTest deprecated versions.

**API breaking changes**: Assessment detected Api.0001 (binary incompatible) in PublicApi and Web, and Api.0002 (source incompatible) in ApplicationCore and Web. Investigate and resolve all API-level breaking changes inline. Specifically: research the flagged APIs, identify replacement patterns, and apply fixes in the affected project files. The assessment also flags behavioral changes (Api.0003) in BlazorAdmin, FunctionalTests, and PublicApiIntegrationTests — these must be reviewed and addressed.

**Done when**: All 10 projects reference net10.0, all package versions are compatible with net10.0, the solution restores without errors, and the solution builds with 0 errors.

---

### 03-validate: Validate solution build and run test suite

Perform final validation of the fully upgraded solution. Run a complete solution build to confirm zero errors and no warnings in upgraded projects. Then execute the unit test suite to confirm functional correctness.

Address any remaining test failures due to behavioral API changes (Api.0003 flags in BlazorAdmin, FunctionalTests, PublicApiIntegrationTests). Document any packages that remain deprecated but have no available replacements (AutoMapper.Extensions.Microsoft.DependencyInjection, xunit runner variants) as post-upgrade recommendations.

**Done when**: Solution builds with 0 errors, all unit tests pass (UnitTests project), and any test failures are documented with root cause analysis.

---
