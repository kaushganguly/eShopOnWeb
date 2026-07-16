# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: Top-Down
- **Unsupported Packages**: Resolve Inline
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: 10 projects on net8.0, all SDK-style, modern .NET upgrade is straightforward. Top-Down selected since applications (Web, PublicApi) have the most issues and can be upgraded first.

### Execution Constraints
- All projects upgrade to net10.0 (no multi-targeting needed for modern .NET upgrade)
- Upgrade in bottom-to-top dependency order within phases: foundation libs → applications → tests
- Validate each phase builds before proceeding to next
- Fix all API breaking changes inline as part of each task
- Commit after each task completes
