# eShopOnWeb .NET Version Upgrade Scenario

## Scenario Parameters

- **Scenario ID**: dotnet-version-upgrade
- **Target Framework**: net10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Input Mode**: solution
- **Flow Mode**: Automatic (run end-to-end without pauses)
- **Source Control**: Git repository on main branch

## User Preferences

### Execution Style
- **Automatic Mode**: Enabled — run end-to-end without pausing for approval
- **Security Fixes**: Include all security updates by default

## Decisions

- Target .NET 10.0 LTS (support ends November 2028)
- Upgrade all projects in solution
- Include package updates and security fixes

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Defer Resolution (7 incompatible packages)
- Unsupported API Handling: Fix Inline

