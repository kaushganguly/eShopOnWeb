# .NET Version Upgrade: net8.0 → net10.0

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all modern .NET (net8.0), TFM managed centrally in Directory.Packages.props — single property change upgrades all projects atomically.

### Execution Constraints
- All 10 projects are upgraded simultaneously in one atomic pass — no tier ordering, no multi-targeting
- Update Directory.Packages.props first (TFM + all version variables + package versions), then fix API issues
- Build and fix all compilation errors in a single bounded pass — do not iterate
- Validate build (0 errors, 0 warnings) after all changes before running tests
- Commit strategy: After Each Task (each task leaves a buildable, committable state)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline
