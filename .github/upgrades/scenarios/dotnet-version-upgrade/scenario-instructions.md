# Scenario Instructions — dotnet-version-upgrade

## Scope
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln (also mirrored by Everything.sln)
- Target Framework: net10.0 (from net8.0)
- Projects in scope: ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Centrally managed packages via Directory.Packages.props (Central Package Management) — update TargetFramework and package versions there.

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user confirmation at any stage; only stop if genuinely blocked.
- Source control: working branch `upgrade-dotnet10-20260923073959` created from `main`. Commit as work progresses.

### Technical Preferences
- Target framework: net10.0 for all projects (LTS).
- Update centrally managed NuGet packages (Directory.Packages.props) to versions compatible with net10.0.
- Success criteria: build passes, unit tests pass, no security vulnerabilities, do NOT generate new unit tests (generateNewUnitTests=false).

## Decisions
- Skipped interactive pre-init confirmation because task request already specified target framework (net10.0), scope (all listed projects), and flow mode (Automatic).
- Automatic mode: upgrade options below were auto-confirmed with recommended defaults (no user blocking) since the task explicitly requires fully autonomous execution.

## Upgrade Options
- Strategy > Upgrade Strategy: **All-at-Once** — 10 projects, all net8.0 (modern), low difficulty across the board, ≤15 projects, no CI-green constraint mentioned. Small scope, mechanical TFM bump.
- Compatibility > Unsupported Packages: **Resolve Inline** — small number of incompatible packages (7 total flagged, concentrated in PublicApi/Web), All-at-Once strategy selected (no per-tier buildability constraint).
- Compatibility > Unsupported API Handling: **Fix Inline** — modern-to-modern upgrade (net8.0→net10.0), API issues are mostly behavioral changes (HttpContent, Uri) and a handful of binary-incompatible config binder calls — mechanical fixes, no complex replacements.
- Reliability > Test Coverage: **Skip** — success criteria specifies generateNewUnitTests=false.

## Strategy
**Selected**: All-at-Once
**Rationale**: Small solution (10 projects, 12k LOC), all already on net8.0 (modern .NET), all SDK-style with Central Package Management already in place, low difficulty across every project per assessment. No dependency-tier validation benefit for a modern-to-modern bump.

### Execution Constraints
- Single atomic upgrade — update Directory.Packages.props TFM/net10.0 and all project TFMs together.
- Validate full solution build after the upgrade is applied (not per-project gating).
- Resolve unsupported/incompatible packages inline as part of the same pass (no deferred stub tasks expected given small count).
- Fix API breaking changes inline (mechanical fixes: ConfigurationBinder.Get, Configure<T>, HttpContent/Uri behavioral changes, exception serialization ctor).
- Commit strategy: Single Commit at End (matches All-at-Once default) unless issues require incremental commits for safety — will commit per completed task group for traceability.

