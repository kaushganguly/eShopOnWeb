# 04-test-projects: Upgrade test projects

## Objective

Upgrade all 4 test projects to net10.0: FunctionalTests, IntegrationTests, PublicApiIntegrationTests, and UnitTests.

## Scope

- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `tests/UnitTests/UnitTests.csproj`

## Context (already done)

- .NET 10 SDK (10.0.302) installed
- Directory.Packages.props has net10.0 package versions
- All production projects (BlazorShared, ApplicationCore, Infrastructure, BlazorAdmin, PublicApi, Web) already on net10.0 and building clean
- AutoMapper updated to 15.1.3 in Directory.Packages.props
- Microsoft.AspNetCore.Mvc.Testing set to 10.0.10 via AspNetVersion variable

## Known Issues from Assessment

**FunctionalTests**: 33 issues (1 mandatory). Behavioral change flagged. Microsoft.AspNetCore.Mvc.Testing at 10.0.10.

**IntegrationTests**: 3 issues (1 mandatory). Depends on Infrastructure and UnitTests.

**PublicApiIntegrationTests**: 11 issues (1 mandatory). Behavioral change flagged. Known pre-existing issue: CS0433 (Program class ambiguity between PublicApi and Web - both have `Program` type that both test entry points reference).

**UnitTests**: 3 issues (1 mandatory). Uses deprecated MSTest + xunit packages. Has xUnit2013 warnings.

## Done when

- All 4 test projects target net10.0 and build with 0 errors and 0 warnings
- Test projects can be compiled (running tests will be done in task 05)
