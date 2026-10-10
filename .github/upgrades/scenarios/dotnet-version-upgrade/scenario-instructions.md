# Scenario Instructions — dotnet-version-upgrade

## Scope
Upgrade eShopOnWeb solution (ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, PublicApi, Web, and associated test projects) from .NET 8.0 to .NET 10.0.

## User Preferences

### Technical Preferences
- Target framework: net10.0 (LTS)
- Upgrade centrally managed TFM/SDK version from net8.0 to net10.0 across all source and test projects
- Update NuGet packages (ASP.NET Core, EF Core, Microsoft and third-party) to versions compatible with .NET 10
- Resolve any API/build incompatibilities introduced by the framework upgrade

### Execution Style
- **Flow Mode: Automatic** — run end-to-end without pausing for user approval; only stop if genuinely blocked.

## Source Control
- Git repo; working branch: `upgrade/net10` (created from `main`, which was clean)

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Strategy | All-At-Once | 10 projects, all already on net8.0 (modern-to-modern), ≤15 projects, ≤3-tier dependency depth, no CI-green constraint — mechanical TFM bump with low overhead |

(`generate_dotnet_upgrade_assessment` MCP tool was unavailable in this session — "Orchestrator channel was not available" — assessment was performed manually using `get_project_dependencies`, `get_supported_package_version`, direct file inspection, and NuGet.org lookups. See assessment.md for details.)

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all already on net8.0 (modern-to-modern upgrade), ≤15 projects, ≤3-tier dependency depth, no CI-green constraint, ≤2 high-risk package migrations — mechanical TFM bump with low overhead.

### Execution Constraints
- Single atomic upgrade — update TFM + all package versions across all projects together, no tiering/phasing
- Update all project files (centrally managed TargetFramework + Directory.Packages.props versions, global.json SDK) first
- Restore, then build solution and fix all compilation errors in one bounded pass (not iterative retry-loop framing)
- Testing happens after the atomic upgrade build succeeds
- Commit Strategy: Single Commit at End (per All-at-Once default)

## Success Criteria
- passBuild=true
- passUnitTests=true
