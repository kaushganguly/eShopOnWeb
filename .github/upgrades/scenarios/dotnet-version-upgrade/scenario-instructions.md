# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
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
**Rationale**: 10 projects, all net8.0, all Low difficulty, ≤15 projects, ≤3-tier depth — a single atomic pass is the fastest approach with no multi-targeting overhead.

### Execution Constraints
- Single atomic upgrade — all 10 projects updated together in one pass
- Validate full solution build after all projects are upgraded
- Fix all API breaking changes inline (no stubs/deferred tasks)
- Resolve the 1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) inline
- Single commit at end (All-at-Once strategy recommended commit approach)
