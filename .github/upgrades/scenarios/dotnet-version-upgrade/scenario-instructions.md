# Scenario Instructions — dotnet-version-upgrade

## Scope
Upgrade the eShopOnWeb solution (`eShopOnWeb.sln`) — ApplicationCore, BlazorAdmin, BlazorShared,
Infrastructure, PublicApi, Web, and all test projects — from .NET 8.0 to .NET 10.0.

## Parameters
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0 (LTS)
- Provisioning/Deploy: N/A (upgrade only)
- generateNewUnitTests: false — do not add new unit tests, only ensure existing tests pass
- securityComplianceCheck: true — resolve known vulnerable packages during upgrade

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user review at any stage (assessment, planning,
  task breakdown). Only stop if genuinely blocked (missing info, unresolvable error).
- Source control: working branch `upgrade/dotnet10` created from `main` (was clean).

## Decisions
- Centrally managed package versions via `Directory.Packages.props` — update TFM centrally where
  possible (global.json, Directory.Build.props if present) rather than per-project where it is DRY.
