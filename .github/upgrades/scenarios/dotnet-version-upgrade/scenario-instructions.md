# Scenario Instructions: .NET Version Upgrade

## Scenario Parameters

- **Solution**: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
- **Target Framework**: `net10.0` (.NET 10 LTS)
- **Source Framework**: `net8.0` (.NET 8 LTS)
- **Working Branch**: `upgrade/dotnet-net10`
- **Input Mode**: `solution`

## User Preferences

### Flow Mode

- **Mode**: Automatic — run end-to-end, only pause when blocked

### Execution Style

- Fully autonomous execution, no user confirmation pauses
- Accept all defaults

## Projects in Scope

All projects in eShopOnWeb.sln:
- ApplicationCore
- Infrastructure
- Web
- PublicApi
- BlazorShared
- BlazorAdmin
- UnitTests
- IntegrationTests
- FunctionalTests
- PublicApiIntegrationTests

## Success Criteria

- passBuild: true
- passUnitTests: true

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets — remove, no compatible version)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: All 10 projects target net8.0 (modern .NET), ≤15 projects, 4-tier dependency graph; simultaneous upgrade is the right approach.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Validate full solution build after all TFM and package changes
- Fix all compilation errors before moving to testing
- No tier ordering required
- Commit once after full solution builds and tests pass
