# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input
- **Working Branch**: upgrade/dotnet-10

### Technical Preferences

- **Target Framework**: net10.0 (LTS, support ends Nov 2028)
- **Source Control**: Working branch `upgrade/dotnet-10`

## Decisions

- Upgrade all projects in eShopOnWeb solution to net10.0
- Projects: ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Update target framework, SDK version in global.json, and all compatible NuGet packages
