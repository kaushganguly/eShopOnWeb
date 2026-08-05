# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0

## Parameters
- **inputMode**: solution
- **paths**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **targetFramework**: net10.0

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end, never pause for user confirmation

### Technical Preferences
- Upgrade to net10.0 (LTS, supported until Nov 2028)

## Decisions
- Working branch: upgrade/dotnet-10
- Source control: git, commit pending changes strategy: auto-commit after each task
