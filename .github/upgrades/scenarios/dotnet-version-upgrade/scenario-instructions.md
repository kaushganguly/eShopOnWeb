# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options

### Strategy
- **Upgrade Strategy**: Top-Down

### Compatibility
- **Unsupported Packages**: Defer Resolution (7 incompatible packages)
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade-net10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down
**Rationale**: The solution is already on modern .NET, but its 5-level dependency graph and the highest-risk API and package work being concentrated in Web and PublicApi make an application-first plan the safest incremental path.

### Execution Constraints
- Upgrade applications in priority order: Web, PublicApi, then BlazorAdmin.
- Keep `global.json` and `Directory.Packages.props` as the single source of truth for the SDK, target framework, and shared package versions.
- Apply package removals, incompatible-package deferrals, and API fixes inside the task that upgrades the affected project; do not create standalone test-only tasks.
- Only begin remaining shared-library and dependent-test consolidation after all three applications build on net10.0.
- Remove any temporary compatibility conditions before final validation.
