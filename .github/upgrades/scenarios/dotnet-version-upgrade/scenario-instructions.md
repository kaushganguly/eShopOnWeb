# Scenario Instructions

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Flow Mode

Automatic — run end-to-end without pausing for user input.

### Technical Preferences

- Target framework: net10.0 (LTS)
- All projects in the solution must be upgraded
- Source-control branch: upgrade/dotnet-10

## Decisions

- Use net10.0 as the target framework (LTS, support ends Nov 2028)
- Automatic flow mode — no pauses for confirmation
