# .NET Version Upgrade

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

### Modernization
- Nullable Reference Types: Leave Disabled

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects all on net8.0 (modern .NET), CPM centralizes package management, no .NET Framework projects, straightforward TFM + package version bump.

### Execution Constraints
- Upgrade all projects simultaneously — no tier ordering, no phased rollout
- Update Directory.Packages.props first (single source of truth for TFM + versions)
- Run `dotnet build` after all changes; fix all errors in a single bounded pass
- Fix all build warnings — projects must build warning-free
- Tests run only after the full solution builds successfully

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
