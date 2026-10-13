# Scenario Instructions — dotnet-version-upgrade

## Scenario Parameters
- Solution: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln` (also mirrored by `Everything.sln`, same project set)
- Target Framework: `net10.0` (from .NET 8.0)
- Scope: ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Working branch: `upgrade/net10`

## User Preferences

### Execution Style
- **Flow Mode: Automatic** — do not pause for user confirmation at any stage. Only stop if genuinely blocked (missing info, ambiguous decision with major consequences, unrecoverable error).
- Task requested with explicit success criteria: passBuild=true, generateNewUnitTests=false, passUnitTests=true, securityComplianceCheck=true.

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | All-at-Once | Simple classification: 10 projects (≤15), all already net8.0 (net6.0+), 3-tier dependency depth, no high-risk cross-stack migrations, no CI-green constraint |
| Test Coverage | Skip | Default + explicit user success criteria `generateNewUnitTests=false` |

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects (≤15), all already net8.0 (net6.0+), CPM with single shared
`TargetFramework` property, 3-tier dependency depth, no high-risk cross-stack migrations
(no .NET Framework, no System.Web), no CI-green constraint.

### Execution Constraints
- Single atomic upgrade — bump the central `TargetFramework` once (Directory.Packages.props), then update all package versions together
- No tier/dependency-order phasing — group tasks by logical concern (libraries, web/api apps, tests), not by dependency tier
- Build and fix all compilation errors in one bounded pass per task — not an open-ended retry loop
- Testing happens after the atomic package/TFM upgrade completes (task 04, after 02/03)
- Commit Strategy: Single Commit at End (per All-at-Once default) — this repo is a git repo on branch `upgrade/net10`

## Decisions
- Proceeding directly with target framework net10.0 as explicitly requested by the user; skipped pre-initialization confirmation pause since all required parameters (target framework, flow mode) were already provided in the initial request.
- The dedicated `generate_dotnet_upgrade_assessment` tool repeatedly failed with "Orchestrator channel was not available" (7 retries over ~3 minutes). Fell back to manually assembling `assessment.md` using `get_dotnet_upgrade_options`, `get_supported_package_version` (per-package, per-consuming-project), and direct repository inspection — all of which worked normally. Proceeding with Automatic-mode judgment per workflow rules (no `assessment.json` exists, so `query_dotnet_assessment` is unavailable for this scenario; assessment.md carries full detail instead).
- Upgrade Options confirmed without pausing (Automatic mode): All-at-Once strategy, Test Coverage = Skip — both are the tool's own default-logic recommendations, so no ambiguous/user-facing decision was overridden.
