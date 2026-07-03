# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: Top-Down

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

### Modernization
- Nullable Reference Types: Leave Disabled

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: 5-tier dependency depth exceeds the ≤3-tier threshold for All-at-Once; Web (29 issues) and PublicApi (17 issues) are high-risk apps that benefit from app-first ordering.

### Execution Constraints
- Upgrade applications (Web, PublicApi) before consolidating libraries
- Library dependencies upgraded inline as part of each application task (no standalone multi-targeting for modern-to-modern)
- Validate full solution build after each application task before proceeding
- Test projects upgraded alongside their primary application
- Phase 2 (consolidation/final validation) only begins after all applications are upgraded

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
