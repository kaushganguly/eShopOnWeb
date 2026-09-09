# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution
- **Paths**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln

## User Preferences

### Flow Mode
Automatic — run end-to-end without pausing for user input. Only stop if genuinely blocked.

### Execution Style
- Fully autonomous execution
- Accept all defaults
- Do not pause for confirmation

## Source Control
- **Repository**: kaushganguly/eShopOnWeb
- **Working Branch**: upgrade/dotnet10
- **Base Branch**: main
- **Commit Strategy**: commit after each task

## Decisions

- Target framework confirmed: net10.0 (LTS, GA)
- Flow mode: Automatic
