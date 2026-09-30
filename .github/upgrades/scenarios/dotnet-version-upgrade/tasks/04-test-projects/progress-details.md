# Task 04: test-projects — Progress Details

## Summary
Upgraded all 4 test projects (UnitTests, IntegrationTests, FunctionalTests,
PublicApiIntegrationTests) to build and pass on net10.0, fixing both compile-time warnings
(new xUnit analyzer rules) and two distinct net10.0 CLR/compiler runtime breaking changes
that only surfaced when actually *running* the tests (not just building).

## Files Modified

### Warning fixes (xUnit2013 analyzer — Assert.Equal(N, X.Count) → Assert.Single/Assert.Empty)
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`

### MSTest analyzer fix (MSTEST0044 — DataTestMethod obsolete)
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
  (`[DataTestMethod]` → `[TestMethod]`)

### Dead-code removal (BlazorInputFile — unblocks net10.0 CLR type-layout runtime failure)
- `src/BlazorShared/Models/CatalogItem.cs` (removed unused `DataToBase64(IFileListEntry)`)
- `src/BlazorAdmin/_Imports.razor` (removed `@using BlazorInputFile`)
- `src/BlazorAdmin/BlazorAdmin.csproj` (removed `PackageReference`)
- `src/BlazorShared/BlazorShared.csproj` (removed `PackageReference`)
- `Directory.Packages.props` (removed `BlazorInputFile` `PackageVersion`)

### Test project dependency cleanup (removes net10.0 JSInterop.WebAssembly runtime failure + CS0433 ambiguity)
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
  (switched `ReturnsFirst10CatalogItems` from `Web`'s `CatalogIndexViewModel` to `PublicApi`'s
  own `ListPagedCatalogItemResponse`, matching the sibling test method in the same file)
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
  (removed unneeded `ProjectReference` to `src/Web/Web.csproj`)
- `tests/PublicApiIntegrationTests/ProgramTest.cs` (kept plain `WebApplicationFactory<Program>`;
  no longer ambiguous once the `Web` reference was removed)

## Root Causes Diagnosed (net8.0 → net10.0 breaking changes)
1. **xUnit analyzer tightening**: `xunit`/`xunit.runner.visualstudio` 2.9.3/4.0.0 activated
   `xUnit2013` more aggressively than 2.7.0/2.5.6.
2. **Top-level-statement `Program` visibility change**: net10.0's compiler/SDK generates the
   implicit `Program` class from top-level statements as `public` (confirmed via reflection);
   net8.0 generated it as `internal`. This can create `CS0433` ambiguity whenever a test project
   references two class libraries that both use top-level statements and only one of them
   declares an explicit `public partial class Program {}` — the previously-inaccessible-but-
   present internal `Program` in the other library becomes visible and collides.
3. **Stricter CLR type-layout validation on `GetTypes()`**: net10.0's runtime now throws
   `ReflectionTypeLoadException` for older/WASM-only assemblies (`BlazorInputFile` 0.2.0,
   `Microsoft.JSInterop.WebAssembly`) whose type layouts were tolerated by net8.0's loader.
   This surfaces specifically when code performs an `AppDomain.CurrentDomain.GetAssemblies()
   .SelectMany(a => a.GetTypes())`-style scan (as the third-party `MinimalApi.Endpoint`
   package's `AddEndpoints()` does) in a process where such assemblies happen to be loaded.

All three were root-cause-fixed (semantically correct assertions, dead-code removal, and
removing an unnecessary cross-project reference) rather than suppressed.

## Validation
- `dotnet build eShopOnWeb.sln` → **0 Warnings, 0 Errors** (all 11 projects, including all 4
  test projects).
- `dotnet test eShopOnWeb.sln --no-build`:
  - UnitTests: 44/44 passed
  - IntegrationTests: 3/3 passed
  - FunctionalTests: 12/12 passed
  - PublicApiIntegrationTests: 15/15 passed
  - **Total: 74/74 passed, 0 failed, 0 skipped**
