# .NET Version Upgrade: eShopOnWeb .NET 8 → .NET 10

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
**Rationale**: 10 projects, all net8.0 (modern .NET), ≤15 projects, mechanical TFM bump with no CI-green or multi-team constraints.

### Execution Constraints
- Single atomic upgrade — all projects updated in one pass
- Update TFM centrally via Directory.Packages.props (already uses CPM)
- Update global.json SDK from 8.0.x to 10.0.x
- Update all Microsoft.* packages from 8.0.x to 10.0.x via Directory.Packages.props
- Resolve incompatible packages inline (remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Fix all API breaking changes inline (no stubs)
- Validate: full solution build + unit tests must pass after upgrade
