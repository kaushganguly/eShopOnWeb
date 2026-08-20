# Scenario Instructions: .NET Version Upgrade

## Scenario
- **Scenario ID**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Working Branch**: upgrade/dotnet10

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- Target framework: net10.0 (LTS)
- Upgrade entire solution (all 10 projects)

## Decisions
- Automatic flow mode confirmed
- Target: net10.0 (LTS, supported until Nov 2028)
- Strategy: upgrade all projects in topological order
