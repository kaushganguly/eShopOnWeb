# Scenario Instructions — dotnet-version-upgrade

## Scenario Spec

- Repo root: /home/runner/work/eShopOnWeb/eShopOnWeb
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target framework: net10.0 (upgrading from net8.0)
- Scope: Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, and associated test projects
- External Task Id: 001-upgrade-dotnet-to-net10
- Success Criteria: passBuild=true, generateNewUnitTests=false, passUnitTests=true, securityComplianceCheck=true

## User Preferences

### Execution Style

- Flow Mode: **Automatic** — do not pause for user confirmation at any stage; only stop if genuinely blocked. Surface assessment/plan/progress inline without waiting for approval.

### Technical Preferences

- Update NuGet package references to versions compatible with .NET 10.
- Update SDK version in global.json to match target framework.
- Preserve existing functionality; resolve build/API compatibility issues as they arise.
- generateNewUnitTests=false — do not generate new unit tests, only ensure existing tests pass.

## Source Control

- Working branch: upgrade/net10-001-upgrade-dotnet-to-net10 (created from main, clean working tree at start)

## Upgrade Options

- **Upgrade Strategy**: All-at-Once — 10 projects (≤15), all already on net8.0 (modern .NET), no .NET Framework boundary, no CI-green or multi-team constraints, single `Directory.Packages.props` controls the TFM for every project. Default per planning rules for ≤15 projects / ≤3-tier-ish complexity. Auto-confirmed under Automatic flow mode (no user override requested).
- **Test Coverage**: Skip — task requirement explicitly states `generateNewUnitTests=false`.

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all already on net8.0 (modern .NET), Central Package Management makes the TFM a single-property change, no CI-green or multi-team constraint signaled.

### Execution Constraints
- Single atomic upgrade — update TFM + all package references across all projects together, no tier ordering/phasing
- Restore, then build the full solution and fix all compilation errors in one bounded pass (not an iterative retry loop)
- Testing happens AFTER the atomic upgrade build succeeds, not interleaved per-project
- Fix all pre-existing and newly introduced build warnings in every touched project before completion
- Re-run vulnerability scan (`dotnet list package --vulnerable`) after upgrade to confirm `securityComplianceCheck`

### Commit Strategy
**Selected**: Single Commit at End — matches All-at-Once strategy (one atomic upgrade, one commit).
