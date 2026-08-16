# Scenario Instructions

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0)
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input

### Technical Preferences

- Target framework: net10.0 (LTS)
- All 10 projects in solution must be upgraded
- TargetFramework is defined centrally in Directory.Packages.props
- SDK version is pinned in global.json — must be updated
- All NuGet packages coupled to framework version must be updated

## Decisions

- Working branch: upgrade/net10.0
- Commit strategy: commit changes as tasks complete
