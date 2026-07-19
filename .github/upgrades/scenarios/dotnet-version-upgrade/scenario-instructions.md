# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects all on net8.0, centralized management via Directory.Packages.props means TFM + package update is one file change. 2 high-risk projects (PublicApi, Web) but small enough scope for single atomic pass.

### Execution Constraints
- Single atomic upgrade — all projects updated together via Directory.Packages.props
- Validate full solution build after core upgrade task completes
- Fix all breaking API changes inline (no stubs or deferred work)
- Run complete test suite only after solution builds successfully
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

### Modernization
- Nullable Reference Types: Leave Disabled

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade-1
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
