# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET). Per default logic for ≤15 projects with ambiguous depth signals (5 tiers), All-at-Once is recommended to avoid multi-targeting overhead.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Update all TargetFramework values and package references before attempting a build
- After all changes: `dotnet restore` then `dotnet build`, fix all compilation errors in one bounded pass
- Validate full solution build (0 errors, 0 warnings in modified projects) before running tests
- Testing follows successful build; do not run tests until build is clean

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
