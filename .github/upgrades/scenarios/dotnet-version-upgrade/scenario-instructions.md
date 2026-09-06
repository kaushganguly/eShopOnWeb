# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Source Control
- **Working Branch**: upgrade/dotnet-10
- **Base Branch**: main

## Decisions
- Target framework: net10.0 (LTS, support ends Nov 2028)
- All projects in solution to be upgraded
- NuGet packages to be updated to .NET 10-compatible releases
