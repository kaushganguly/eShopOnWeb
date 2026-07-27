# .NET Version Upgrade: net8.0 → net10.0

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects all on net8.0, modern-to-modern TFM bump, clear dependency structure with manageable scope.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Update TargetFramework in Directory.Packages.props and SDK in global.json first
- Update all package references across all projects in one pass
- Fix all compilation errors after TFM and package updates
- Validate full solution build with 0 errors before running tests

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
