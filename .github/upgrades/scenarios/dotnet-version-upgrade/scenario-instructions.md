# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end, do not pause for user input

### Technical Preferences
- Target Framework: net10.0 (LTS)
- Strategy: upgrade all projects in the solution

## Decisions
- Working branch: upgrade/net10-upgrade
- Source control: git, commit changes after each task
- All 10 projects to be upgraded: ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
