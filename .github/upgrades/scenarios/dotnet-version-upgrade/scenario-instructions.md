# Scenario Instructions: .NET Version Upgrade

## Scenario Details

- **Scenario**: dotnet-version-upgrade
- **Description**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Target Framework**: net10.0
- **Input Mode**: solution

## User Preferences

### Execution Style

- **Flow Mode**: Automatic — run end-to-end without pausing for user input
- **Confirmation**: All defaults accepted, no user prompts required

### Technical Preferences

- Target framework: net10.0 (LTS, support ends Nov 2028)

## Decisions

- Upgrade all 10 projects to net10.0: Web, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, PublicApi, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- Update all NuGet packages to net10.0-compatible versions
- Resolve any breaking API changes between .NET 8 and .NET 10
- Success criteria: build passes, unit tests pass

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all on net8.0, all SDK-style, ≤15 projects, no CI-green constraint — atomic upgrade is ideal and fastest.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Operation sequence: update all TFMs → update all package references → restore → build and fix all compilation errors → verify 0 errors
- Testing comes AFTER the atomic upgrade completes successfully
- Full solution build validation after upgrade with 0 errors required
- No tier ordering — all projects upgraded simultaneously
