# 04-test-projects — Progress Details

## Summary
Upgraded all 4 test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) to build and
pass on net10.0. Fixed one real compile-breaking regression surfaced by the upgrade (ambiguous `Program` type) and
cleaned up analyzer warnings from newer test-package versions.

## Changes made

### Directory.Packages.props
- `xunit`: 2.7.0 → 2.9.3 (deprecated-package flag; latest 2.x — avoided the v2→v3 migration, out of scope)
- `xunit.runner.console`: 2.7.0 → 2.9.3 (kept in lockstep with `xunit`)
- `MSTest.TestAdapter` / `MSTest.TestFramework`: 3.2.2 → 3.11.1 (deprecated-package flag; latest 3.x patch — avoided the 4.x major jump `get_supported_package_version` offered, to keep this test upgrade low-risk)
- `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory` already resolved centrally via `$(AspNetVersion)`/`$(EntityFramworkCoreVersion)` — no action needed here.

### Real compile-break fix: ambiguous `Program` type (tests/PublicApiIntegrationTests)
`dotnet build` surfaced **CS0433**: `The type 'Program' exists in both 'PublicApi...' and 'Web...'`. This is a new
issue introduced by the net10.0 upgrade — the net8.0 baseline build (verified by temporarily stashing all changes)
did not exhibit it. `PublicApiIntegrationTests.csproj` references both `PublicApi.csproj` and `Web.csproj`; both
expose an (unnamespaced, global-scope) top-level-statement `Program` class — `PublicApi/Program.cs` explicitly
declares `public partial class Program { }` for `WebApplicationFactory<Program>` testability, and `Web`'s implicit
top-level `Program` class became visible/ambiguous to the test assembly under net10.0.

**Fix**: Added `<Aliases>WebAssembly</Aliases>` to the `Web.csproj` `ProjectReference` in
`PublicApiIntegrationTests.csproj`, and added `extern alias WebAssembly;` + qualified `using WebAssembly::...`
in the two files that needed disambiguation:
- `tests/PublicApiIntegrationTests/ProgramTest.cs` — unqualified `Program` now resolves only to PublicApi's.
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — updated its
  `using Microsoft.eShopWeb.Web.ViewModels;` to `using WebAssembly::Microsoft.eShopWeb.Web.ViewModels;` (this file
  needs `CatalogIndexViewModel` from the `Web` project).

### Analyzer-warning cleanup (from newer test-package versions)
- **xUnit2013** ("Do not use Assert.Equal() to check for collection size"), surfaced by the `xunit` bump pulling in
  a newer xunit.analyzers version:
  - `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` — `Assert.Equal(0, ...Count)` → `Assert.Empty(...)`
  - `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` — two occurrences of `Assert.Equal(1, ...Count)` → `Assert.Single(...)`
  - `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` — `Assert.Equal(0, ...Count)` → `Assert.Empty(...)`
- **MSTEST0044** ("'DataTestMethod' is obsolete. Use 'TestMethod' instead."), surfaced by the `MSTest.TestAdapter` 3.11.1 bump's new analyzer:
  - `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — `[DataTestMethod]` → `[TestMethod]` (kept the existing `[DataRow(...)]` attributes, which MSTest 3.x supports directly on `[TestMethod]`).

## API/behavioral issues reviewed (no code changes required)
All Api.0003 (behavioral-change) occurrences across the 4 test projects (`HttpContent.ReadAsStringAsync()`,
`System.Uri.ToString()` in HTTP-integration test assertions) resolved cleanly by recompilation with no source
changes — confirmed by full test-suite runs below.

## Validation
- `dotnet restore eShopOnWeb.sln` — 0 errors.
- `dotnet build` for all 4 test projects individually — **all 0 Errors**; only the residual, accepted NU1901
  (low-severity, transitive, no-fix-available) warnings inherited from `Microsoft.VisualStudio.Web.CodeGeneration.Design`
  via the Web/PublicApi project references. No project-specific warnings remain (all xUnit2013/MSTEST0044 fixed).
- `dotnet test tests/UnitTests/UnitTests.csproj` — **44/44 passed**.
- `dotnet test tests/IntegrationTests/IntegrationTests.csproj` — **3/3 passed**.
- `dotnet test tests/FunctionalTests/FunctionalTests.csproj` — **12/12 passed**.
- `dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — **15/15 passed**.
- **Total: 74/74 tests passed (100%).**
