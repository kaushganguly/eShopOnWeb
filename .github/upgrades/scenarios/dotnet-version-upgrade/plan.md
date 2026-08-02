# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects — 6 source projects (ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, PublicApi, Web) and 4 test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0, 4 code tiers, ≤2 high-risk API changes, no CI-green constraint.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Confirm that the .NET 10 SDK is installed and available. Update `global.json` to specify a net10.0-compatible SDK version, and verify that `Directory.Packages.props` contains the centrally managed `TargetFramework` property (if applicable). This task ensures the toolchain is ready before any project files are modified.

The solution uses Central Package Management (`Directory.Packages.props`). Check whether `TargetFramework` is defined centrally or per-project, and validate that no `global.json` SDK constraint will block the upgrade.

**Done when**: The .NET 10 SDK is confirmed installed; `global.json` specifies a net10.0-compatible SDK; build infrastructure is ready for the TFM bump.

---

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Update the `TargetFramework` for all 10 projects from `net8.0` to `net10.0`. Update all NuGet package references to versions compatible with net10.0, including the 19 packages with recommended upgrades (Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, Microsoft.Extensions.*, System.Text.Json, Azure.Identity, etc.).

Key concerns:
- **Incompatible package**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no compatible version) — remove or replace inline.
- **Security vulnerabilities**: `Azure.Identity` (1.10.4 → 1.21.0) and `System.Security.Claims` (remove — included in framework reference).
- **Binary incompatible APIs** (Api.0001) in `PublicApi` and `Web` — fix inline.
- **Source incompatible APIs** (Api.0002) in `ApplicationCore` and `Web` — fix inline.
- **Behavioral changes** (Api.0003) in `BlazorAdmin`, `FunctionalTests`, `PublicApi`, `PublicApiIntegrationTests`, `Web` — fix inline.
- **Deprecated packages**: `AutoMapper.Extensions.Microsoft.DependencyInjection`, `MSTest.*`, `xunit`, `System.IdentityModel.Tokens.Jwt` — update to supported versions.
- `System.Security.Claims` (4.3.0) is now included in the framework reference — remove the explicit package reference.

All projects are upgraded together in one atomic pass per the All-at-Once strategy. Use the assessment to identify all affected files before making changes.

**Done when**: All 10 projects target `net10.0`; all package references are updated to compatible versions; the solution restores, builds, and compiles with 0 errors and 0 warnings in all modified projects.

---

### 03-final-validation: Run full test suite and document results

After the upgrade, run the full test suite across all test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests). Address any test failures introduced by the net10.0 upgrade. Verify no behavioral regressions were introduced by the behavioral-change fixes.

Document any deferred items (e.g., deprecated test framework migration from xunit 2.x to xunit 3.x if needed, MSTest migration).

**Done when**: All unit tests pass; all integration tests pass; the full solution builds without errors or warnings; any deferred recommendations are documented.

---
