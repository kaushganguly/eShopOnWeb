# Scenario Instructions: .NET Version Upgrade

## Scenario

Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 (LTS).

- **Solution**: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
- **Target Framework**: `net10.0`
- **Input Mode**: solution
- **Paths**: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input.

## Upgrade Options

| Option | Selected | Category |
|--------|----------|----------|
| Upgrade Strategy | All-at-Once | Strategy |
| Unsupported Packages | Resolve Inline | Compatibility |
| Unsupported API Handling | Fix Inline | Compatibility |

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0 (modern .NET), all rated Low difficulty — simple TFM bump with package updates and minor API fixes; incremental approach adds overhead without benefit.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Update all TFMs, packages, and fix API issues before building
- Build the full solution once after all changes; fix any compilation errors in that single bounded pass
- Testing comes after the atomic upgrade completes successfully
- Commit once at end (single commit for all-at-once)

## Decisions

- Target framework: net10.0 (LTS, support ends Nov 2028)
- Branch: upgrade/dotnet-10
- Commit strategy: Single Commit at End (All-at-Once)
