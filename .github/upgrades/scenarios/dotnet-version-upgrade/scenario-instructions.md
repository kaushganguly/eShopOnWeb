# Scenario Instructions: .NET Version Upgrade

## Scenario
Upgrade eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0)

## Parameters
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution
- **Working Branch**: upgrade-to-net10

## User Preferences

### Execution Style
- Flow Mode: Automatic (no pauses, fully autonomous end-to-end execution)

## Decisions
- Target .NET 10 (LTS, support ends Nov 2028)
- All projects in the solution to be upgraded
