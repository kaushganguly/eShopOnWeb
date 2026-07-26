# 04-test-projects: Progress Details

## Status: Complete

## Build Results

| Project | Errors | Warnings |
|---------|--------|----------|
| UnitTests | 0 | 0 |
| IntegrationTests | 0 | 0 |
| FunctionalTests | 0 | 0 |
| PublicApiIntegrationTests | 0 | 0 |

## Files Changed

### UnitTests fixes (xUnit2013 warnings)

**`tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`**
- Line 19: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
  - xUnit2013: use `Assert.Empty` instead of `Assert.Equal(0, ...)` for collection size check

**`tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`**
- Line 22: `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)`
- Line 35: `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)`
  - xUnit2013: use `Assert.Single` instead of `Assert.Equal(1, ...)` for single-item collection checks

### IntegrationTests fixes (xUnit2013 warning)

**`tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`**
- Line 38: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
  - xUnit2013: use `Assert.Empty` instead of `Assert.Equal(0, ...)` for collection size check

### PublicApiIntegrationTests fixes (CS0433 ambiguous Program)

**`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`**
- Added `<Aliases>WebApp</Aliases>` to the Web project reference
- This scopes all types from Web.dll behind the `WebApp` extern alias, eliminating the ambiguity between `PublicApi::Program` and `Web::Program` in the global namespace

**`tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`**
- Added `extern alias WebApp;` at top of file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using WebApp::Microsoft.eShopWeb.Web.ViewModels;`
- This is the only file that uses types from the Web assembly; all other files in PublicApiIntegrationTests use types from PublicApi or ApplicationCore

### FunctionalTests
- Already built with 0 errors and 0 warnings — no changes required.

## Issues Encountered and Resolution

### CS0433 — Program ambiguity in PublicApiIntegrationTests

**Root cause**: Both `src/PublicApi/Program.cs` and `src/Web/Program.cs` declare an implicit or explicit `Program` class in the global namespace (top-level statements). `PublicApiIntegrationTests.csproj` references both projects, so `WebApplicationFactory<Program>` in `ProgramTest.cs` was ambiguous.

**Fix chosen**: extern alias (`<Aliases>WebApp</Aliases>` on the Web project reference). This is the least invasive fix:
- No changes needed to production source code (PublicApi/Program.cs or Web/Program.cs)
- `Program` in the default global scope now unambiguously refers to `PublicApi`'s Program (the correct one for these API integration tests)
- Only `CatalogItemListPagedEndpoint.cs` needed updating since it's the only file using `Microsoft.eShopWeb.Web.ViewModels`

### xUnit2013 warnings

**Root cause**: Older code used `Assert.Equal(0, collection.Count)` and `Assert.Equal(1, collection.Count)` which xUnit analyzers flag in favor of `Assert.Empty` / `Assert.Single`.

**Fix**: Updated 4 assertion sites across UnitTests and IntegrationTests to use the semantically correct overloads.
