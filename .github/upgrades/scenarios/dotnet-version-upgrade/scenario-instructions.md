# Scenario Instructions — dotnet-version-upgrade

## Scenario Parameters

- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0 (.NET 10, LTS)
- Scope: ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared, and test projects
- Central Package Management: Directory.Packages.props defines shared TargetFramework/package versions
- Success Criteria: passBuild=true, generateNewUnitTests=false, passUnitTests=true, securityComplianceCheck=true

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user input at any point; proceed end-to-end unless genuinely blocked.

### Source Control
- Working branch: `upgrade/dotnet10` (created from `main`, working tree was clean at start).

## Upgrade Options

### Strategy
- **Upgrade Strategy**: All-at-Once — 10 projects, all already on net8.0 (modern-to-modern), all 🟢 Low difficulty, no CI-green constraint, no framework boundary crossing. Small scope, mechanical TFM bump; incremental approach adds overhead without benefit.

### Test Coverage
- **Test Coverage**: Skip — task explicitly requires `generateNewUnitTests=false`; rely on existing unit/integration/functional tests for validation.
