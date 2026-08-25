# Scenario Instructions

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution
- **Working Branch**: upgrade/dotnet-10

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input
- **Commit Strategy**: commit after each task

## Decisions

- Target framework confirmed: net10.0 (LTS, support ends Nov 2028)
- All 10 projects in eShopOnWeb solution to be upgraded
- TargetFramework centrally managed in Directory.Packages.props
- SDK version in global.json to be updated to 10.0.x
- All NuGet packages to be updated to .NET 10-compatible versions
