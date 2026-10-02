# Upgrade Plan: eShopOnWeb — net8.0 → net10.0

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all currently on net8.0 (modern .NET), no .NET Framework projects, shallow dependency graph (≤3 tiers), low difficulty per assessment, project count well within the all-at-once threshold (≤15-20 projects).

### Execution Constraints
- Single atomic upgrade — all projects (ApplicationCore, BlazorShared, Infrastructure, Web, PublicApi, BlazorAdmin, and all 4 test projects) updated together in one pass
- Update TFMs and package versions together, then do one bounded build-and-fix pass (not an iterative retry loop)
- Validate full solution build only after the entire upgrade pass completes
- Testing happens after the atomic upgrade task completes successfully, as a separate final validation task
- Commit strategy: Single commit at end (matches All-at-Once strategy default)

## Upgrade Options
- **Upgrade Strategy**: All-At-Once (see rationale above)
- **Test Coverage**: Skip — assessment reports `TestCoverageRecommended=False` for all 10 projects; `generateNewUnitTests=false` per user's stated success criteria
- **Package Management**: Central Package Management (already in use via `Directory.Packages.props`) — retained as-is, no conversion needed
- **Security Vulnerabilities**: Included in the upgrade by default — NuGet.0004 findings in ApplicationCore and Web will be resolved by upgrading to non-vulnerable, net10-compatible package versions

## Scope

**Libraries**
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorShared/BlazorShared.csproj
- src/Infrastructure/Infrastructure.csproj

**Applications**
- src/Web/Web.csproj (ASP.NET Core, Razor Pages + Blazor Server host)
- src/PublicApi/PublicApi.csproj (ASP.NET Core Minimal API)
- src/BlazorAdmin/BlazorAdmin.csproj (Blazor WebAssembly)

**Test Projects**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Key Assessment Signals
- 118 issues total (21 mandatory, 83 potential, 14 optional) across 10 projects
- Security vulnerabilities flagged (NuGet.0004) in ApplicationCore and Web
- Deprecated packages (NuGet.0005) in Infrastructure, PublicApi, Web, and all test projects
- Binary-incompatible packages (NuGet.0001) in PublicApi and Web
- Behavioral API changes (Api.0003) in BlazorAdmin, PublicApi, Web, and 3 test projects
- Source-incompatible API (Api.0002) in ApplicationCore
- Central Package Management already in place via `Directory.Packages.props` — retained
- All projects already SDK-style — no conversion needed

---

## Tasks

### 01-prerequisites: Verify SDK toolchain and update pinned SDK version

Verify the .NET 10 SDK is installed and available in the build environment, then update `global.json` to pin a net10.0-compatible SDK version. Confirm the existing Central Package Management setup in `Directory.Packages.props` is structurally ready to receive net10-compatible package version bumps (no version changes yet in this task — that happens in task 02). Also check CI configuration (`.github/workflows/*`, `azure-pipelines*.yml` if present) for any hardcoded SDK/runtime version pins that will need updating in lockstep.

**Done when**: .NET 10 SDK confirmed installed via `validate_dotnet_sdk_installation`; `global.json` pins a valid net10.0 SDK version confirmed via `validate_dotnet_sdk_in_globaljson`; CI SDK version pins inventoried.

### 02-upgrade-all-projects: Upgrade all projects to net10.0 (TFMs, packages, breaking-change fixes)

Upgrade all 10 projects (ApplicationCore, BlazorShared, Infrastructure — class libraries; Web, PublicApi, BlazorAdmin — applications; UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests — test projects) from `net8.0` to `net10.0` simultaneously. This is a modern-to-modern TFM bump (net8→net10): no SDK-style conversion, no multi-targeting, no System.Web migration is required.

Update `TargetFramework` in every `.csproj` to `net10.0`. Bump all NuGet package versions centrally in `Directory.Packages.props` to net10-compatible versions, prioritizing the assessment's flagged packages: the 2 security-vulnerable packages (NuGet.0004, in ApplicationCore and Web), the deprecated packages (NuGet.0005, in Infrastructure/PublicApi/Web/all test projects), and the binary-incompatible packages (NuGet.0001, in PublicApi and Web). Pay particular attention to ASP.NET Core packages (Microsoft.AspNetCore.*), EF Core packages (Microsoft.EntityFrameworkCore.*) in Infrastructure/ApplicationCore, and Blazor WebAssembly packages in BlazorAdmin/BlazorShared, since these carry the behavioral API changes (Api.0003) flagged in BlazorAdmin, PublicApi, Web, and 3 test projects, plus the source-incompatible API (Api.0002) in ApplicationCore.

Research starting points: query the assessment per-project (`query_dotnet_assessment`) for the full issue list in ApplicationCore, PublicApi, and Web first (highest issue density), review each project's EF Core DbContext/migrations usage in Infrastructure, review minimal-API and middleware registration code in PublicApi/Web for ASP.NET Core breaking changes, and review BlazorAdmin's `Program.cs`/WASM hosting setup for Blazor breaking changes.

**Done when**: All 10 projects target `net10.0`; `dotnet restore` succeeds; solution builds with 0 errors and 0 warnings.

### 03-final-validation: Final validation — build, tests, security check

Run full solution build and the complete test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) to confirm the upgrade is behavior-preserving. Re-check for known-vulnerable NuGet packages post-upgrade (no new/remaining vulnerabilities). Document any deferred recommendations (e.g., optional-severity assessment issues not addressed in this pass).

**Done when**: `passBuild=true`, `passUnitTests=true` (all test projects pass), `securityComplianceCheck=true` (no known vulnerable packages remain).
