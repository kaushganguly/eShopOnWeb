# Scenario Instructions: dotnet-version-upgrade

## Scope

- Solution: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
- Target framework: net10.0 (from net8.0)
- Projects in scope: ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Update centrally managed package versions in `Directory.Packages.props` to net10-compatible versions.
- Resolve breaking API changes across ASP.NET Core, Entity Framework Core, Identity, and Blazor WebAssembly.

## User Preferences

### Execution Style

- Flow Mode: **Automatic** — do not pause for user review/approval at any stage. Only pause if genuinely blocked (missing info, ambiguous decision with major consequences).
- Task ID for tracking: 001-upgrade-dotnet-to-net10

### Success Criteria

- passBuild = true (solution builds with no errors/warnings)
- generateNewUnitTests = false (do not author new unit tests)
- passUnitTests = true (existing unit tests must pass)
- securityComplianceCheck = true (no known vulnerable package versions)

## Upgrade Options

Auto-confirmed in Automatic mode (no user pause) — recommended defaults accepted as-is:

- **Strategy**: All-at-Once — 10 projects (≤15), all already on net8.0 (modern-to-modern), low complexity, no CI-green constraint signaled.
- **Unsupported Packages**: Resolve Inline — only PublicApi flags NuGet.0001 (incompatible), small count, researched/resolved within the same task.
- **Unsupported API Handling**: Fix Inline — modern-to-modern upgrade (net8.0 → net10.0), few flagged Api.0001/0002/0003 issues, no bottom-up strategy.
- Package Management option not applicable — Directory.Packages.props (CPM) already in use across all projects.
- Test Coverage option not applicable — assessment shows TestCoverageRecommended=False for all projects.
- No .NET Framework projects, no System.Web, no Windows-native APIs, no third-party DI/logging frameworks detected — remaining compatibility/modernization options not applicable.

## Strategy

**Selected**: All-At-Once
**Execution constraints**:
- All 10 projects upgraded together in one pass — no tier ordering, no phased rollout.
- Single bounded build-and-fix pass: update TFM + packages, restore, build, fix all compile errors in one go (not an iterative retry loop).
- Testing happens after the atomic upgrade completes successfully (final-validation task).
- Unsupported Packages: Resolve Inline (no stub/defer tasks).
- Unsupported API Handling: Fix Inline (no stub/defer tasks).

## Decisions

- Working branch `upgrade/net10-20261001083949` created from `main` (clean tree) before initializing scenario.
- Both `eShopOnWeb.sln` and `Everything.sln` exist and reference the same 10 projects; using `eShopOnWeb.sln` as canonical solution for this upgrade, but will update all .csproj files directly (shared by both solutions) so `Everything.sln` also builds against net10.0.
