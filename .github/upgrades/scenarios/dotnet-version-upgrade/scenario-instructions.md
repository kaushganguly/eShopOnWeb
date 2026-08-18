# Scenario Instructions

## Scenario
- **Scenario**: dotnet-version-upgrade
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Input Mode**: solution
- **Working Branch**: upgrade/dotnet-10

## User Preferences

### Execution Style
- Flow mode: Automatic (no pauses, fully autonomous)
- All defaults accepted

### Technical Preferences
- Target Framework: net10.0 (LTS, support ends Nov 2028)
- Upgrade all projects in solution
- Update global.json SDK version to .NET 10.0.x
- Update all NuGet packages to .NET 10 compatible versions

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets in PublicApi)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET), straightforward TFM bump.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Operation sequence: update TFMs → update packages → restore → build → fix errors
- Validate full solution build after all changes (0 errors, 0 warnings in modified projects)
- Tests run after successful build
- Commit once at end after all tasks complete

## Decisions
- Automatic flow mode: run end-to-end without pausing for user input
- Working branch: upgrade/dotnet-10
- Strategy: All-at-Once (10 modern .NET projects, no Framework migration needed)
