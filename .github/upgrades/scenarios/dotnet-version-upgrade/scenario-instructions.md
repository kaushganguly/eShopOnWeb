# Scenario Instructions: .NET Version Upgrade

## Scenario
- **From**: net8.0 (.NET 8)
- **To**: net10.0 (.NET 10)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Input Mode**: solution

## User Preferences

### Execution Style
- Flow Mode: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- Target Framework: net10.0
- SDK Version: 10.0.x (use 10.0.400 which is installed)

## Decisions
- Upgrade all projects: Web, ApplicationCore, Infrastructure, PublicApi, BlazorAdmin, BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Update global.json SDK version to 10.0.x
- Update Directory.Packages.props TargetFramework and all NuGet package versions to .NET 10-compatible versions
