# .NET Version Upgrade — eShopOnWeb (net8.0 → net10.0)

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET). Straightforward modern-to-modern TFM bump. CPM already active. Under 15 projects, ambiguous tier depth → All-at-Once default.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Validate full solution build after all changes applied
- CPM is active in Directory.Packages.props — update package versions there (version variables + PackageVersion entries), not in individual project files
- Fix all compilation errors before marking 02-upgrade-projects complete
- Testing comes after build succeeds (Task 03)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline
