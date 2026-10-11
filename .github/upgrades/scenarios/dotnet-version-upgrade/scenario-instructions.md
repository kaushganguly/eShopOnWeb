# Scenario Instructions — .NET Version Upgrade

## Scope
Upgrade eShopOnWeb solution (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin,
BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)
from .NET 8.0 to .NET 10.0.

- Solution file: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
- Target framework: `net10.0`
- Centrally managed target framework / package versions: `Directory.Packages.props` (CPM enabled)

## Success Criteria
- passBuild = true
- generateNewUnitTests = false
- passUnitTests = true
- securityComplianceCheck = true (no known vulnerable package versions after upgrade)

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user review/approval at any stage. Only stop if
  genuinely blocked (missing info, unresolved ambiguous decision, or unrecoverable build/test
  failure needing user direction).
- Task requested explicit TaskId `001-upgrade-dotnet-to-net10` — tracked as the umbrella task for
  this upgrade in the external caller's system; internal task breakdown still follows plan.md /
  tasks.md per the dotnet-version-upgrade scenario's own task IDs.

### Technical Preferences
- Target framework: net10.0 (LTS, confirmed via get_dotnet_upgrade_options; available options were
  net9.0 STS, net10.0 LTS, net11.0 Preview — net10.0 selected per explicit user request).
- generateNewUnitTests = false → do not author brand-new unit test files as part of this upgrade;
  only keep existing tests passing (fix broken tests due to API/behavior changes).

## Decisions
- Working branch `upgrade/dotnet10` created from `main` (clean tree) before `initialize_scenario`,
  per source-control pre-initialization rule.
- Dedicated `generate_dotnet_upgrade_assessment` tool failed consistently
  (`Orchestrator channel was not available within 5000 ms`) after multiple retries with increasing
  backoff (5s, 10s, 30s, 45s, 60s, 90s waits). Fell back to a manually-assembled `assessment.md`
  using `get_project_dependencies`, `get_supported_package_version`,
  `get_projects_in_topological_order`, and direct repo inspection.
- Upgrade complexity classified as **Simple** (all 10 projects already on net8.0/SDK-style, CPM
  enabled, no .NET Framework projects) → only Strategy + Test Coverage options evaluated.
- `start_task`/`break_down_task`/`complete_task` plan-parsing tools also failed throughout the
  session (`Task 'X' not found in plan.md or tasks.md. Available top-level tasks: ` — empty list
  — regardless of plan.md formatting), confirming the same backend outage affects task
  orchestration, not just assessment. Fell back to manually creating `tasks/{id}/task.md`,
  `progress-details.md`, and `tasks.md`, and tracking completion state in this file instead.
- **AutoMapper security/licensing decision**: `dotnet restore` surfaced NU1903 — AutoMapper
  12.0.1 (pulled transitively via the now-orphaned `AutoMapper.Extensions.Microsoft.DependencyInjection`
  companion package) has a HIGH severity DoS CVE (GHSA-rvv3-g6hj-g44x, unbounded recursive-mapping
  stack overflow), fixed in 15.1.1/16.1.1+. AutoMapper (same author/model as MediatR) introduced a
  commercial license tier starting at v13 for qualifying commercial use (automapper.io). Given the
  explicit `securityComplianceCheck=true` success criterion, **decided to upgrade** to the core
  `AutoMapper` package at `16.2.0` (patched) rather than leave the CVE unresolved, and flagged the
  licensing change prominently in the final summary and in
  `tasks/02-upgrade-all-projects/progress-details.md` for the user/organization's legal review —
  this is not something an autonomous agent can resolve on the organization's behalf.
- **MediatR**: left on the latest free/Apache-2.0 12.x release (`12.5.0`) rather than jumping to
  the real latest `14.2.0`, since v13+ requires the same kind of commercial license and there was
  no CVE forcing the move (unlike AutoMapper). Flagged for the user to revisit if/when they decide
  on licensing.
- **`Microsoft.JSInterop.WebAssembly` TypeLoadException regression** (PublicApiIntegrationTests):
  confirmed via a `git stash` control run that all 15 tests passed before the upgrade and failed
  after, root-caused to `MinimalApi.Endpoint`'s `AddEndpoints()` scanning every assembly loaded in
  the AppDomain (including Web's transitive Blazor-WASM-only dependencies, newly incompatible with
  net10's stricter type loader) — fixed by excluding those WASM-only assemblies from the test
  project's own output via an MSBuild target, not by patching the third-party package. See
  `tasks/02-upgrade-all-projects/progress-details.md` for full detail.
- Final state: `dotnet build eShopOnWeb.sln -c Release` → 0 errors, 0 warnings;
  `dotnet test eShopOnWeb.sln -c Release` → 74/74 passed, 0 failed, 0 skipped. All four success
  criteria (`passBuild`, `generateNewUnitTests=false` respected, `passUnitTests`,
  `securityComplianceCheck`) satisfied.

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | **All-at-Once** | 10 projects, all net8.0+/SDK-style, ≤3-tier dependency graph, no CI-green constraint communicated — default per strategy.md decision table (≤15 projects, modern-to-modern) |
| Test Coverage | **Skip** | Default (opt-in only); also matches explicit user success criterion `generateNewUnitTests=false` |

(Confirmed automatically in Automatic flow mode — surfaced to user, not blocked on approval.)

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all on .NET 8.0, SDK-style, CPM-enabled, shallow dependency
graph, no CI-green constraint — straightforward TFM + package bump better suited to one
atomic pass than incremental tiers.

### Execution Constraints
- Update all project files (central TFM in `Directory.Packages.props`, `global.json`) before
  touching package versions.
- Update all package references across all projects in one pass (shared MSBuild properties
  `AspNetVersion`/`EntityFramworkCoreVersion`/`SystemExtensionVersion` + individual packages).
- Restore, then build the full solution and fix all compilation errors/warnings in a single
  bounded pass — not an iterative retry loop.
- Testing happens AFTER the atomic upgrade build is clean (task 03), not interleaved.
- No tier ordering / no "first upgrade X then Y" sequencing — all 10 projects move together.
