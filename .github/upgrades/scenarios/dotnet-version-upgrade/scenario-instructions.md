# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 (LTS)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Source Framework**: net8.0

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- **Target Framework**: net10.0 (LTS, support ends Nov 2028)
- **Working Branch**: upgrade/dotnet10

## Decisions
- All projects in the solution should be upgraded to net10.0
- TargetFramework is centrally defined in Directory.Packages.props and must be updated there
- global.json must be updated to SDK 10.0.x
- All NuGet packages must be updated to .NET 10 compatible versions
