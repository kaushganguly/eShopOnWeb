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
**Rationale**: 10 projects all on net8.0 (modern .NET), ≤15 projects, mechanical TFM bump to net10.0. Ambiguous depth signal resolved to All-at-Once per the ≤15 projects default.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Update Directory.Packages.props for all package version bumps (CPM already in use)
- Validate full solution build after all changes applied
- Fix all API breaking changes inline (no deferred stubs)
- Remove/replace incompatible packages inline
- Commit all changes as a single commit at end
