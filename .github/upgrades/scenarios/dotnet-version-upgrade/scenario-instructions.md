# Scenario Instructions: dotnet-version-upgrade

## Scope
- Solution: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
- Target Framework: net10.0 (from net8.0)
- Projects in scope: Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, and associated test projects (UnitTests, IntegrationTests, FunctionalTests, etc.)

## Success Criteria
- passBuild = true
- generateNewUnitTests = false
- passUnitTests = true
- securityComplianceCheck = true

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user review/approval at any stage. Only pause if genuinely blocked (missing info, ambiguous decision with major consequences).
- Task requester explicitly requested fully autonomous end-to-end execution.

### Technical Preferences
- Central package management (`Directory.Packages.props`) is used — update centrally managed versions there.
- SDK version constraint in `global.json` must be updated to match target framework's SDK band.

## Decisions
- Source control: created and switched to working branch `upgrade/dotnet10` from `main` (working tree was clean) before initializing scenario.
- Automatic mode + user explicitly said "never present options, accept all defaults" — upgrade options below were resolved using recommended defaults without pausing for confirmation.

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all already on .NET 8.0 (modern, SDK-style), ≤3-tier dependency depth, only 1 incompatible package and no high-risk migrations. Small scope, mechanical TFM bump.

### Execution Constraints
- Single atomic upgrade — all projects updated together (grouped into libraries/web-apps/tests tasks for organization, not tiered gating)
- Update all project files (TargetFramework, imports) and all package references across all projects
- Restore dependencies, then build solution and fix all compilation errors in one bounded pass (not an iterative retry loop)
- Testing comes AFTER the atomic upgrade completes successfully — full solution build must be green first
- Verify: solution builds with 0 errors and 0 warnings before running the test suite

### Commit Strategy
- **Selected**: Single Commit at End (matches All-at-Once strategy default — one atomic upgrade, one commit)

