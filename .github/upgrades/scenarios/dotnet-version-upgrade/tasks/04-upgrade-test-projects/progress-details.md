# Progress: 04-upgrade-test-projects

## Changes Made

1. **`tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`** —
   `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`.
2. **`tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`** —
   same `Assert.Empty` fix.
3. **`tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`** —
   `Assert.Equal(1, ...Count)` → `Assert.Single(...)` (2 occurrences).
4. **`tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`** —
   `[DataTestMethod]` → `[TestMethod]` (MSTest 4.x merged the attributes).
5. **`tests/PublicApiIntegrationTests/ProgramTest.cs`** — added `extern alias
   PublicApiAlias;` and qualified `WebApplicationFactory<PublicApiAlias::Program>` to
   resolve the `Program` type ambiguity between `PublicApi` and `Web`.
6. **`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`** — added
   `<Aliases>global,PublicApiAlias</Aliases>` to the `PublicApi` project reference, and a
   `RemoveBlazorWebAssemblyAssembliesFromOutput` MSBuild target to strip
   Blazor-WebAssembly-only assemblies (pulled in transitively via the `Web` project
   reference) from the test host's output — these caused a `TypeLoadException` under
   .NET 10's stricter struct-layout validation when `MinimalApi.Endpoint`'s
   `AddEndpoints()` reflection-scanned the test host's base directory.

All 4 test projects target `net10.0` via the central `Directory.Packages.props`
`<TargetFramework>` property, using the updated test SDK packages
(`Microsoft.NET.Test.Sdk` 18.10.1, `xunit` 2.9.3, `xunit.runner.visualstudio` 4.0.0,
`MSTest` 4.4.1, `coverlet.collector` 10.0.1).

## Validation

```
dotnet build eShopOnWeb.sln            → Build succeeded, 0 Warning(s), 0 Error(s)
dotnet test eShopOnWeb.sln --no-build
  UnitTests.dll                 → Passed! Failed: 0, Passed: 44, Skipped: 0, Total: 44
  IntegrationTests.dll          → Passed! Failed: 0, Passed: 3,  Skipped: 0, Total: 3
  FunctionalTests.dll           → Passed! Failed: 0, Passed: 12, Skipped: 0, Total: 12
  PublicApiIntegrationTests.dll → Passed! Failed: 0, Passed: 15, Skipped: 0, Total: 15
```

**All 74 tests pass across all 4 test projects** on net10.0, with zero build warnings or
errors across the entire solution.
