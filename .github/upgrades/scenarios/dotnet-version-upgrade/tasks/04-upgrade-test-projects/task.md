# 04-upgrade-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests to net10.0

## Scope

Projects: `tests/UnitTests/UnitTests.csproj`, `tests/IntegrationTests/IntegrationTests.csproj`,
`tests/FunctionalTests/FunctionalTests.csproj`, `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`.

TFM and all shared test package versions (`Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`,
`xunit.runner.console`, `MSTest.TestAdapter`/`MSTest.TestFramework`, `coverlet.collector`,
`Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory`) were already bumped
solution-wide in task 02 via `Directory.Packages.props`.

## Assessment findings (query_dotnet_assessment)

- UnitTests.csproj (3 issues): TFM (mandatory, done); `xunit`/`xunit.runner.console` deprecated →
  already bumped to 2.9.3 in task 02 shared props (xUnit v2 is in maintenance mode, security-fixes-only;
  full v3 migration is out of scope for this version-upgrade task — no user request to change test
  framework, only to move to net10.0).
- IntegrationTests.csproj (3 issues): TFM (done); `Microsoft.EntityFrameworkCore.InMemory` → 10.0.12 (done
  via shared props); `xunit` deprecated notice (same as above, staying on v2.9.3).
- FunctionalTests.csproj (33 issues, 20 shown): TFM + package bumps (done via shared props). Remaining are
  all Api.0003 behavioral-change notices for `HttpContent.ReadAsStringAsync()` and `System.Uri.ToString()`
  across `Web/Pages/Basket/*.cs`, `Web/Pages/HomePageOnGet.cs` — same category as tasks 02/03, no source
  change required (existing patterns remain valid on net10.0).
- PublicApiIntegrationTests.csproj (11 issues): TFM + package bumps (done); `MSTest.TestAdapter`/
  `MSTest.TestFramework` deprecated → already bumped to 4.4.1 in task 02 shared props; remaining Api.0003
  `HttpContent.ReadAsStringAsync()` notices, no source change required.

## Plan

1. No further `Directory.Packages.props` changes expected — all test package versions already resolved
   in task 02.
2. Build each of the 4 test projects individually; fix any compiler errors/warnings that surface.
3. Run `dotnet test` across all 4 projects; fix any failing tests caused by the upgrade (net10 runtime
   behavior changes), consistent with `generateNewUnitTests=false` (no new tests to be authored — only
   fix breakage caused directly by the upgrade).
4. Record results in progress-details.md.
