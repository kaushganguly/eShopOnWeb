# Scenario Instructions: .NET Version Upgrade

## Scenario
- **Scenario ID**: dotnet-version-upgrade
- **Goal**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- Target: net10.0 (LTS, support ends Nov 2028)
- All 10 projects (6 source + 4 test) must be upgraded
- Directory.Packages.props manages centralized package versions
- global.json pins the SDK version — must be updated to .NET 10 SDK
- All framework-coupled NuGet packages must be updated to .NET 10 compatible versions

## Decisions
- Working branch: upgrade/dotnet-10
- Commit strategy: Single Commit at End (All-at-Once strategy)

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET), all SDK-style, low complexity, CPM already in place.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Update global.json and Directory.Packages.props as first steps
- Update all TFMs, then package versions, then fix API issues
- Validate full solution build after all changes
- Resolve Microsoft.VisualStudio.Azure.Containers.Tools.Targets incompatibility inline (remove or replace)
- Fix all binary/source incompatible API changes inline during execution
