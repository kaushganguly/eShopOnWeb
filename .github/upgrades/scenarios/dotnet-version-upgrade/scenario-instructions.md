# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade-net10
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

### Modernization
- Nullable Reference Types: Leave Disabled

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0, 4 code tiers, ≤2 high-risk changes, no CI-green constraint — standard mechanical TFM bump.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Validate full solution build after all changes are applied
- All package references updated to net10.0-compatible versions in the same pass
- Incompatible packages resolved inline (remove/replace) rather than stubbed
- API breaking changes fixed inline in the same task
