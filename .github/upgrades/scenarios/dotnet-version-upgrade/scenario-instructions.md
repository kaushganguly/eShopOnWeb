# .NET Version Upgrade

## Strategy
**Selected**: All-at-Once — all 10 projects upgraded simultaneously.
**Rationale**: 10 projects, all on net8.0 (modern .NET), ≤15 project scope.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- TFM + packages + code fixes done in one task (no phasing)
- Validate full solution build after upgrade completes
- Commit after each task (git commit strategy)

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
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
- **Working Branch**: dotnet-version-upgrade-net10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
