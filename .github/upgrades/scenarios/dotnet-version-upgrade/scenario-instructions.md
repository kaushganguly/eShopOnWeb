# Scenario Instructions: .NET Version Upgrade

## Scenario

- **Scenario ID**: dotnet-version-upgrade
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 (LTS)

## User Preferences

### Flow Mode

- **Mode**: Automatic — run end-to-end without pausing for user input

### Execution Style

- Fully autonomous execution — accept all defaults
- Do NOT pause for review or confirmation at any point

## Upgrade Options

| Option | Category | Selected Value |
|--------|----------|----------------|
| Upgrade Strategy | Strategy | All-at-Once |
| Unsupported Packages | Compatibility | Resolve Inline |
| Unsupported API Handling | Compatibility | Fix Inline |

## Strategy

**Selected**: All-at-Once
**Rationale**: 10 projects all on net8.0, all rated Low difficulty, no .NET Framework, no CI-green constraint, ≤15 projects.

### Execution Constraints

- Single atomic upgrade — all projects updated together in one pass
- Update TFMs, packages, and code fixes in one bounded pass per task
- Validate full solution build after all changes applied
- Testing comes after the atomic upgrade completes successfully

## Source Control

- **Working Branch**: upgrade/dotnet-net10
- **Commit Strategy**: Commit after each completed task
