# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Flow Mode
Automatic — run end-to-end without pausing for user input.

### Technical Preferences
- Target framework: net10.0 (LTS, supported until Nov 2028)
- Update SDK in global.json to .NET 10.0.x
- Update Directory.Build.props TFM from net8.0 → net10.0
- Update all framework-versioned NuGet packages in Directory.Packages.props

### Execution Style
- Fully autonomous execution
- Never pause for confirmation
- Accept all defaults

## Decisions

- Flow mode: Automatic
- Working branch: upgrade/dotnet-10
- Target: net10.0 LTS
