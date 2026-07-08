# 05-shared-target-consolidation: Remove temporary coexistence targets and clean central version drift

## Objective
Collapse multi-targeting from 4 shared projects back to net10.0-only, update Directory.Build.props
default to net10.0, centralize all version properties in Directory.Packages.props, and remove all
per-project VersionOverride entries that were only needed during the phased upgrade.

## Scope

### Projects to collapse (net8.0;net10.0 → net10.0)
- `src/ApplicationCore/ApplicationCore.csproj` — no conditional ItemGroups, straightforward collapse
- `src/BlazorShared/BlazorShared.csproj` — no conditional ItemGroups, straightforward collapse
- `src/BlazorAdmin/BlazorAdmin.csproj` — has net8.0 conditional ItemGroup (VersionOverride 8.0.2) and net10.0 conditional ItemGroup — collapse both into one unconditional ItemGroup
- `src/Infrastructure/Infrastructure.csproj` — has net8.0 conditional ItemGroup (VersionOverride 8.0.2) and net10.0 conditional ItemGroup (VersionOverride 10.0.9) — collapse to unconditional

### Central config changes
- `Directory.Build.props`: change default `TargetFramework` from net8.0 → net10.0
- `Directory.Packages.props`:
  - `AspNetVersion`: 8.0.2 → 10.0.9
  - `EntityFramworkCoreVersion`: 8.0.2 → 10.0.9
  - `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
  - Remove `BlazorWasmVersion` (same value as AspNetVersion after update); replace all `$(BlazorWasmVersion)` refs with `$(AspNetVersion)`

### VersionOverride cleanup (all projects)
Once centralized, the following projects can drop all `VersionOverride="10.0.9"` / `VersionOverride="10.0.2"` entries:
- `src/Web/Web.csproj`
- `src/PublicApi/PublicApi.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

## Deferred packages (no action needed)
- `System.IdentityModel.Tokens.Jwt` 8.4.0 — stays deferred (deprecated, NuGet.0005)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 — stays deferred (deprecated, NuGet.0005)
- `xunit` / `xunit.runner.*` — stays deferred (deprecated, NuGet.0005)
- `BlazorInputFile` 0.2.0 — third-party, no net10.0 update available; stays at current version

## Done when
All 4 shared projects target net10.0 only; no conditional ItemGroups remain; Directory.Packages.props
centralises all 10.0.x versions; no per-project VersionOverride entries for non-deferred packages;
build passes; all 74 tests pass.
