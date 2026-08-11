# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Working Branch**: upgrade/dotnet-10

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences

- Target framework: net10.0 (LTS, support ends Nov 2028)

## Decisions

- Upgrade all 6 source projects and 4 test projects from net8.0 to net10.0
- Update SDK version in global.json to 10.0.x
- Update all NuGet packages to .NET 10-compatible versions
- Resolve any breaking changes between .NET 8 and .NET 10
