# Scenario Instructions: .NET Version Upgrade

## Scenario
- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8 to .NET 10 (LTS)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0

## User Preferences

### Flow Mode
- **Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences
- **Target Framework**: net10.0 (LTS, support ends Nov 2028)
- **Working Branch**: upgrade/dotnet-10
- **Commit Strategy**: Commit after each task

## Decisions
- Automatic flow mode selected — no pauses for review
- Target .NET 10 (LTS) as specified in task requirements
