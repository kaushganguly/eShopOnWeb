# 03-final-validation: Run full test suite and document results

## Objective
After the upgrade to net10.0, run the full test suite across all test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests). Address any test failures. Verify no behavioral regressions.

**Done when**: All unit tests pass; all integration tests pass; the full solution builds without errors or warnings; any deferred recommendations are documented.

## Scope

### Test Projects
- `tests/UnitTests/UnitTests.csproj` — no external dependencies, runs standalone
- `tests/IntegrationTests/IntegrationTests.csproj` — uses in-memory EF Core, no external DB required
- `tests/FunctionalTests/FunctionalTests.csproj` — uses `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory)
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — uses `Microsoft.AspNetCore.Mvc.Testing`

### Environment
- .NET SDK: 10.0.302
- Target framework: net10.0

## Research Findings
- UnitTests.csproj: uses xunit + NSubstitute; references ApplicationCore and Web
- IntegrationTests.csproj: uses in-memory EF Core; references Infrastructure and UnitTests
- FunctionalTests.csproj: uses Microsoft.AspNetCore.Mvc.Testing + in-memory EF; references Web, PublicApi, ApplicationCore
- PublicApiIntegrationTests.csproj: uses Microsoft.AspNetCore.Mvc.Testing; references PublicApi etc.
- FunctionalTests has a `DotNetCliToolReference` to `dotnet-xunit 2.3.1` which is a legacy tool reference — this should be harmless with the SDK runner but may emit a warning.

## Execution Plan
1. Full solution build (no-incremental) — confirm clean compile
2. Run UnitTests — should pass without infrastructure
3. Run IntegrationTests — should pass (in-memory)
4. Run FunctionalTests — should pass (in-proc)
5. Run PublicApiIntegrationTests — should pass (in-proc)
6. Fix any failures
7. Write progress-details.md

