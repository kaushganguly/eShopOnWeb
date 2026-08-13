# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Working Branch**: upgrade/dotnet-10

## User Preferences

### Flow Mode
Automatic — run end-to-end without pausing for user input.

### Technical Preferences
- Target framework: net10.0 (LTS)
- Update all NuGet packages to .NET 10 compatible versions
- Fix all breaking API changes between .NET 8 and .NET 10

## Decisions

- Automatic flow mode selected — no pauses for user review
- All projects in scope: src/Web, src/PublicApi, src/BlazorAdmin, src/BlazorShared, src/Infrastructure, src/ApplicationCore, tests/UnitTests, tests/IntegrationTests, tests/FunctionalTests, tests/PublicApiIntegrationTests
