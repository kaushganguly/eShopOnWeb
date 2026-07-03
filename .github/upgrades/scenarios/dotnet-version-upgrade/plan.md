# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 (LTS)
**Scope**: 10 projects across src/ (6 projects) and tests/ (4 projects), ~117 issues (21 mandatory)

### Selected Strategy
**Top-Down (Application-First)** — Applications upgraded first with their library dependencies upgraded inline.
**Rationale**: 5-tier dependency depth and multiple high-risk applications (Web: 29 issues, PublicApi: 17 issues with binary-incompatible APIs). Upgrading applications and their dependencies together in application-first order ensures each validated layer is solid before moving to dependents.

---

## Tasks

### 01-prerequisites: Verify .NET 10 SDK and update global.json

Confirm that the .NET 10 SDK is installed and available on this machine. Check `global.json` at the solution root — if it pins the SDK to a version incompatible with net10.0, update it to a .NET 10 SDK version. This gates all subsequent tasks; misconfigured SDK toolchain is the most common silent failure point for a major version upgrade.

Also verify the `Directory.Packages.props` file — it already has `<TargetFramework>net8.0</TargetFramework>` as a property. Update the `<TargetFramework>net8.0</TargetFramework>` to `net10.0` and the `<AspNetVersion>` property to `10.0.9` (the net10.0 GA package version), since these are shared across all projects via centralized package management (CPM).

**Done when**: `dotnet --version` reports a .NET 10 SDK; `global.json` (if present) specifies a compatible SDK; `Directory.Packages.props` has `<TargetFramework>net10.0</TargetFramework>`.

---

### 02-upgrade-web: Upgrade Web application and its library dependencies

Upgrade the Web project (AspNetCore, 29 issues — the most complex project in the solution) along with all libraries it depends on: BlazorShared (Level 0, 1 issue), ApplicationCore (Level 1, 6 issues), BlazorAdmin (Level 1, 11 issues), and Infrastructure (Level 2, 5 issues). These libraries have no incompatible package issues, so upgrading them is primarily a TFM change plus package version bumps.

Key issues to resolve in Web:
- **Binary/source incompatible APIs** (`Api.0001`, `Api.0002`): Review all flagged API usages and apply the known .NET 10 replacements.
- **Behavioral changes** (`Api.0003`): Review and adjust for any behavioral differences flagged by the assessment.
- **Security vulnerabilities** (`NuGet.0004`): Upgrade affected packages to eliminate CVEs.
- **Deprecated packages** (`NuGet.0005`): Replace deprecated packages with their current equivalents.
- **Package upgrades** (`NuGet.0002`): Bump all Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, and other packages to their net10.0-compatible versions.

BlazorAdmin has behavioral-change issues (`Api.0003`) — review and fix console/logging API changes flagged there.

Also upgrade the `UnitTests` project (depends on ApplicationCore and Web, 3 issues — all package-related) as part of this task.

**Done when**: Web, BlazorShared, ApplicationCore, BlazorAdmin, Infrastructure, and UnitTests all target net10.0; solution builds with zero errors and zero warnings for these projects; `dotnet test tests/UnitTests/` passes.

---

### 03-upgrade-publicapi: Upgrade PublicApi and fix binary-incompatible APIs

Upgrade the PublicApi project (AspNetCore, 17 issues including 6 mandatory). The key concern here is the binary-incompatible APIs (`Api.0001`) in `Program.cs`:

- `ConfigurationBinder.Get<T>(IConfiguration)` — this overload changed in .NET 9/10; replace with the correct overload or pattern.
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` — similarly changed; update to the new signature or pattern.

Also handle:
- **Incompatible package** (`NuGet.0001`): `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` has no supported version for net10.0. Remove this package reference (it is a Visual Studio tooling package that is optional for runtime functionality).
- **Deprecated packages** (`NuGet.0005`): `AutoMapper.Extensions.Microsoft.DependencyInjection` functionality is now in AutoMapper itself — remove the extension package and verify AutoMapper still registers correctly. `System.IdentityModel.Tokens.Jwt` — update to `8.x` or the latest compatible version.
- **Package upgrades** (`NuGet.0002`): Bump all Microsoft.AspNetCore.* and Microsoft.EntityFrameworkCore.* packages.

Also upgrade `FunctionalTests` (33 issues — mainly package upgrades and 1 behavioral-change API in the test host setup), `PublicApiIntegrationTests` (9 issues), and `IntegrationTests` (3 issues). Test projects should build and their test suites should pass.

**Done when**: PublicApi targets net10.0; binary-incompatible API usages in Program.cs resolved; incompatible package removed; deprecated packages replaced; FunctionalTests, PublicApiIntegrationTests, and IntegrationTests target net10.0 and build cleanly; `dotnet test tests/FunctionalTests/` and `dotnet test tests/IntegrationTests/` pass (or are documented as skipped for infrastructure reasons).

---

### 04-final-validation: Full solution build and test suite

Perform a complete solution build and run all test suites to confirm the upgrade is fully functional end-to-end. This is the final gate before closing the upgrade.

Steps:
1. `dotnet build eShopOnWeb.sln` — must produce zero errors, zero warnings across all projects.
2. `dotnet test eShopOnWeb.sln` — all test projects must pass.
3. Confirm `Directory.Packages.props` is clean: all package versions are current, no leftover net8.0 references.
4. Confirm all `.csproj` files target net10.0 (either directly or via the shared `<TargetFramework>` property from Directory.Packages.props).

If any warnings remain, fix them before completing this task. Suppressing warnings with `#pragma warning disable` or `<NoWarn>` requires explicit justification.

**Done when**: `dotnet build eShopOnWeb.sln` exits with code 0, zero errors, zero warnings; all test projects pass; all projects target net10.0.
