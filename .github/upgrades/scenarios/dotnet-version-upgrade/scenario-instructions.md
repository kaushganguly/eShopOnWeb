# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: Top-Down
- **Unsupported Packages**: Defer Resolution
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: Modern .NET (net8.0 → net10.0) upgrade with 10 projects. Applications (Web, PublicApi, BlazorAdmin) have the highest complexity (binary/source API incompatibilities) and are upgraded first. Shared libraries (ApplicationCore, Infrastructure) are upgraded in Phase 2 after applications are stable.

### Execution Constraints
- Applications upgraded before libraries (apps first, then library consolidation)
- Each task includes its directly related test projects
- Build must succeed after each task before proceeding
- Fix all build warnings — no suppression without explicit approval
- Commit after each task
