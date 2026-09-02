# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- Target framework: net10.0 (LTS, support ends Nov 2028)

## Source Control
- **Working Branch**: upgrade/dotnet-10
- **Base Branch**: main
- **Commit Strategy**: Commit after each completed task

## Decisions
- Upgrade all 10 projects: src/Web, src/ApplicationCore, src/Infrastructure, src/BlazorAdmin, src/BlazorShared, src/PublicApi, tests/UnitTests, tests/IntegrationTests, tests/FunctionalTests, tests/PublicApiIntegrationTests
- Update global.json to SDK 10.0.x
- Update all NuGet packages to net10.0-compatible versions
- Resolve any API breaking changes between .NET 8 and .NET 10
