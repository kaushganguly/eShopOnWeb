# Scenario Instructions: .NET Version Upgrade

## Scenario

Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Flow Mode

Automatic — run end-to-end without pausing for user input.

### Execution Style

- Accept all defaults
- No confirmation prompts

## Decisions

- Target framework: net10.0 (LTS, support until Nov 2028)
- Working branch: upgrade/dotnet-10
- Commit strategy: commit after each task
