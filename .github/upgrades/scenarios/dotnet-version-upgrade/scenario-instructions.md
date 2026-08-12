# Scenario Instructions: .NET Version Upgrade

## Parameters

- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Assessment Input Mode**: solution
- **Assessment Paths**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Working Branch**: upgrade/dotnet-net10

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — do NOT pause for user input at any point
- Run end-to-end without asking for confirmation at any stage

### Technical Preferences

- Upgrade all projects from net8.0 to net10.0
- Update Directory.Packages.props for central package management
- Update global.json SDK version pin
- Fix all breaking API changes between .NET 8 and .NET 10

## Decisions

- Target: net10.0 (LTS, support ends Nov 2028)
- Strategy: Upgrade all projects in one pass
- Source control: branch upgrade/dotnet-net10
