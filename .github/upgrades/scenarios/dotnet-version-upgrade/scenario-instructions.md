# Scenario Instructions — .NET Version Upgrade

## Scope
Upgrade the eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS) across all projects:
ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared, UnitTests,
IntegrationTests, FunctionalTests, PublicApiIntegrationTests.

- Solution files: eShopOnWeb.sln / Everything.sln
- Target framework: net10.0
- Update global.json SDK version
- Update centrally managed TargetFramework and package versions in Directory.Packages.props
- Update NuGet dependencies for .NET 10 compatibility
- Resolve breaking API changes across all projects

## Success Criteria
- passBuild = true
- generateNewUnitTests = false (do not generate new unit tests)
- passUnitTests = true
- securityComplianceCheck = true (no known vulnerable packages)

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — never pause for user confirmation/review at any stage. Only stop if genuinely blocked.
- Do not generate new unit tests; only fix/update existing tests as needed to pass.

### Source Control
- Working branch: `upgrade-to-lts-20261012085819` (already created and checked out from `main`, which was clean).
- Commit strategy: commit progress as needed on the working branch.

### Technical Preferences
- Target framework: net10.0 (LTS, support ends 2028-11-14) — chosen directly from user request, no alternative considered.
