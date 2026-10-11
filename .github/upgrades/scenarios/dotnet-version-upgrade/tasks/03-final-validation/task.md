# Task 03-final-validation: Full solution build + test verification

> **Note**: Same MCP backend outage as tasks 01/02 — tracked manually.

## Objective
Confirm the whole solution builds clean (0 errors, 0 warnings) and the full test suite passes
on net10.0, satisfying the scenario's success criteria: `passBuild=true`, `passUnitTests=true`,
`securityComplianceCheck=true` (`generateNewUnitTests=false` — no new tests were authored, only
existing tests had cosmetic analyzer-driven fixes applied).

## Validation steps performed
1. `dotnet restore eShopOnWeb.sln` — clean, no NU1102/NU1903/NU1901/NU1902 warnings remaining
   (AutoMapper CVE resolved, NuGet.Packaging/Protocol transitively pinned to patched 6.12.5).
2. `dotnet build eShopOnWeb.sln -c Release` — **0 errors, 0 warnings** across all 10 projects
   (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, UnitTests,
   IntegrationTests, FunctionalTests, PublicApiIntegrationTests).
3. `dotnet test eShopOnWeb.sln -c Release` — **74/74 tests passed, 0 failed, 0 skipped**:
   - UnitTests: 44/44
   - IntegrationTests: 3/3
   - FunctionalTests: 12/12
   - PublicApiIntegrationTests: 15/15 (after fixing a genuine net10.0 regression — see
     `tasks/02-upgrade-all-projects/progress-details.md` for the `Microsoft.JSInterop.WebAssembly`
     TypeLoadException root cause and fix).
4. Confirmed via a `git stash` control run against the original net8.0 code that the
   PublicApiIntegrationTests failure was a genuine upgrade regression (passed 15/15 pre-upgrade),
   not a pre-existing flaky/broken test — so the fix applied is a true compatibility fix, not a
   workaround masking a real problem.

## Done when
- [x] `dotnet build` succeeds with 0 errors and 0 warnings.
- [x] `dotnet test` passes with 0 failures across all 4 test projects.
- [x] No known unresolved HIGH/CRITICAL security advisories in `dotnet restore` output.
