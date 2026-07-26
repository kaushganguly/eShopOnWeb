# .NET Version Upgrade: net8.0 → net10.0

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package — Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET), CPM already in use, all SDK-style. Tier depth is 4 levels (manageable). Single incompatible package resolves inline.

### Execution Constraints
- Single atomic upgrade — all projects updated together; validate full solution build after upgrade
- Update TFMs across all projects, bump package versions, fix all breaking API changes in one pass
- Test projects are updated alongside their production counterparts
- No multi-targeting needed — all projects move to net10.0 simultaneously
- Resolve inline: remove/replace `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` during upgrade

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
