# Scenario Instructions

## Scenario

- **Scenario**: dotnet-version-upgrade
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Projects**: ApplicationCore, Infrastructure, PublicApi, BlazorAdmin, BlazorShared, Web, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end, never pause for user input

### Technical Preferences

- Target Framework: net10.0 (LTS)
- Working Branch: app-modernize-net10-upgrade

## Decisions

- Upgrade all 10 projects in eShopOnWeb.sln from net8.0 to net10.0
- Update SDK in global.json to .NET 10.0.x
- Update all NuGet packages to net10.0-compatible versions
- Resolve any API breaking changes between .NET 8 and .NET 10
