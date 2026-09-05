# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style
- **Flow Mode**: Automatic — run end-to-end without pausing for user input

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET), SDK-style, CPM in use, ≤15 projects.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Validate full solution build after upgrade
- Fix all build errors in a single bounded pass
- Tests run after the atomic upgrade completes

## Upgrade Options
| Option | Selected |
|--------|----------|
| Upgrade Strategy | All-at-Once |
| Package Management | CPM (existing) |

## Decisions
- Target framework: net10.0 (LTS, support ends Nov 2028)
- Upgrade all projects in the eShopOnWeb solution
- Use existing CPM in Directory.Packages.props for all package version updates
