# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS

## User Preferences

### Flow Mode

- **Mode**: Automatic — run end-to-end without pausing for user input.

### Technical Preferences

- Target framework: net10.0 (LTS)
- Upgrade all projects in the solution

## Decisions

- Upgrade strategy: in-place upgrade of all projects simultaneously
- Working branch: upgrade/dotnet10
