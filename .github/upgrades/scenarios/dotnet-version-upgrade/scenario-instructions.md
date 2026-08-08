# Scenario Instructions — .NET Version Upgrade

## Scenario
- **From**: net8.0
- **To**: net10.0 (LTS)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln

## User Preferences

### Flow Mode
Automatic — run end-to-end without pausing for user input.

### Technical Preferences
- Target framework: net10.0
- Input mode: solution

## Decisions
- Working branch: upgrade/dotnet-10
- Commit strategy: commit after successful build + test
