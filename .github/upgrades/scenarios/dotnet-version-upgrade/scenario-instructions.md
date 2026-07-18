# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade-1
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all net8.0, all SDK-style, all low complexity (🟢), clear dependency structure. Single atomic operation is appropriate.

### Execution Constraints
- Single atomic upgrade — all projects updated together; validate full solution build after upgrade
- Update all project files (TargetFramework, imports) first, then all package references
- Run `dotnet restore` after package updates
- Build solution and fix all compilation errors in a single bounded pass
- Testing comes AFTER the atomic upgrade completes successfully
