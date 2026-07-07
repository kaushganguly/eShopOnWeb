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
**Rationale**: 10 projects all on modern .NET (net8.0); no .NET Framework projects; straightforward TFM bump with mechanical package version updates.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Operation sequence: update TFMs → update packages → restore → build → fix all errors
- Validate full solution build after upgrade (0 errors, 0 warnings in modified projects)
- Testing comes AFTER the atomic upgrade completes successfully
- Single commit at the end when all tasks complete
