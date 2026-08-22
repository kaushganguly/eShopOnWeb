# .NET Version Upgrade Scenario Instructions

## Scenario Parameters

- **Scenario ID**: dotnet-version-upgrade
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Solution Path**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Input Mode**: solution

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — never pause for user input, run end-to-end autonomously

### Technical Preferences

- Target framework: net10.0 (LTS)
- Upgrade entire solution including all test projects

## Decisions

- Upgrade all projects in the solution: Web, ApplicationCore, Infrastructure, PublicApi, BlazorShared, BlazorAdmin, UnitTests, IntegrationTests, PublicApiIntegrationTests, FunctionalTests
- Update global.json SDK version to 10.0.x
- Update Directory.Packages.props TargetFramework and all version variables
- Resolve any breaking API changes between .NET 8 and .NET 10
