# Scenario Instructions: .NET Version Upgrade

## Scenario Parameters

- **Scenario**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Source Framework**: net8.0
- **Working Branch**: upgrade/dotnet-net10

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input
- Accept all defaults and complete the entire upgrade autonomously

### Technical Preferences

- Target: net10.0 (LTS, support ends Nov 2028)
- Upgrade all projects in the eShopOnWeb solution:
  - ApplicationCore
  - BlazorShared
  - BlazorAdmin
  - Infrastructure
  - PublicApi
  - Web
  - UnitTests
  - IntegrationTests
  - FunctionalTests
  - PublicApiIntegrationTests
- Update global.json SDK version to 10.0.x
- Update all NuGet package references to net10.0-compatible versions
- Resolve any breaking API changes between .NET 8 and .NET 10

## Success Criteria

- Build passes (passBuild: true)
- Unit tests pass (passUnitTests: true)

## Decisions

- Task ID assigned: 001-upgrade-dotnet-to-net10
- Flow mode: Automatic (fully autonomous execution)
