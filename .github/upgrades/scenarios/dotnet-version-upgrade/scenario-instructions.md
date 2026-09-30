# Scenario Instructions — dotnet-version-upgrade

## Scenario Parameters

- Solution: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln` (also mirrored by `Everything.sln`, same project set)
- Target Framework: **net10.0** (upgrading from net8.0)
- Projects in scope: ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, Web, PublicApi, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Success Criteria: passBuild=true, passUnitTests=true

## User Preferences

### Execution Style

- **Flow Mode: Automatic** — do not pause for user review/approval at any stage. Surface assessment/plan/progress inline and keep proceeding. Only stop if genuinely blocked (missing info, unresolvable build errors after reasonable effort).
- No environment configuration overrides provided.

### Technical Preferences

- Update centrally managed NuGet package versions (Directory.Packages.props) to releases compatible with .NET 10: ASP.NET Core, EF Core, Identity, Ardalis libraries (Specification, GuardClauses, etc.), xUnit, Azure SDK packages.
- Resolve breaking API changes across class libraries, ASP.NET Core web/API projects, and the Blazor WebAssembly admin project.

## Decisions

- Working branch `upgrade/dotnet10` created from `main` (clean tree) before scenario initialization.

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | All-At-Once | 10 projects total, all currently net8.0 (already modern), all SDK-style, dependency graph ≤ 3 tiers deep (ApplicationCore → Infrastructure/BlazorShared → Web/PublicApi/BlazorAdmin → test projects), all projects rated 🟢 Low difficulty in assessment. Fits All-At-Once criteria exactly (≤15 projects, no .NET Framework, ≤2 high-risk package migrations). |

Automatic mode: options confirmed by agent without pausing (per user's explicit "never pause" directive); surfaced to user inline in response.

## Strategy
**Selected**: All-At-Once
**Rationale**: Small, uniformly modern (.NET 8) solution with shallow dependency graph and low per-project difficulty ratings across the board — ideal for a single atomic upgrade pass rather than tiered rollout.

### Execution Constraints
- Single atomic upgrade — update TargetFramework + package versions across all 10 projects together, no tiering
- Sequence: (1) update TFMs/imports, (2) update centrally-managed package versions in Directory.Packages.props, (3) restore, (4) build solution and fix all compile errors in one bounded pass, (5) run full test suite (Unit/Integration/Functional/PublicApiIntegration)
- No manual/visual validation steps; only automated build + test validation
- Commit Strategy: Single Commit at End (matches All-At-Once)
