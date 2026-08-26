# Scenario Instructions: .NET Version Upgrade

## Scenario

Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution
- **Working Branch**: upgrade/net10

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input.

## Decisions

- Target framework: net10.0 (LTS, support ends Nov 2028)
- All projects in eShopOnWeb.sln to be upgraded
- Success criteria: build passes, unit tests pass
