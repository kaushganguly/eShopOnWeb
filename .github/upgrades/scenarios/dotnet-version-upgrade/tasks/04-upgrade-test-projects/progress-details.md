# Progress Details — 04-upgrade-test-projects

## Files modified

### Package/version files
- `Directory.Packages.props`
  - Test package versions bumped: `Microsoft.NET.Test.Sdk` 17.9.0→18.10.1, `xunit` 2.7.0→2.9.3,
    `xunit.runner.visualstudio` 2.5.6→4.0.0, `xunit.runner.console` 2.7.0→2.9.3,
    `NSubstitute` 5.1.0→6.2.0, `MSTest.TestAdapter`/`MSTest.TestFramework` 3.2.2→4.4.1,
    `coverlet.collector` 6.0.2→10.0.1.
  - Removed `BlazorInputFile` `PackageVersion` entry (abandoned/dead package, see task.md #3).

### Test project files
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
  - Added `<Aliases>global,PublicApiAlias</Aliases>` to the `PublicApi` `ProjectReference` (fixes
    CS0433 ambiguous `Program`).
  - Added `RemoveBlazorWebAssemblyAssembliesFromOutput` MSBuild target excluding
    `*WebAssembly*`/`BlazorAdmin`/`Blazored.*` assemblies from `ReferenceCopyLocalPaths` (fixes
    runtime `TypeLoadException` from Blazor WASM assemblies).
- `tests/PublicApiIntegrationTests/ProgramTest.cs`
  - Added `extern alias PublicApiAlias;`; changed both `WebApplicationFactory<Program>` usages to
    `WebApplicationFactory<PublicApiAlias::Program>`.
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
  - `[DataTestMethod]` → `[TestMethod]` (MSTEST0044 fix for MSTest 4.x).
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
  - `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (×2 occurrences,
    xUnit2013 fix).
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
  - `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013 fix).
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
  - `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013 fix).

### Dead-code / abandoned-package cleanup (BlazorInputFile removal)
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed `<PackageReference Include="BlazorInputFile" />`.
- `src/BlazorShared/BlazorShared.csproj` — removed `<PackageReference Include="BlazorInputFile" />`.
- `src/BlazorAdmin/_Imports.razor` — removed unused `@using BlazorInputFile`.
- `src/BlazorShared/Models/CatalogItem.cs` — removed unused `using BlazorInputFile;` /
  `using System.Threading.Tasks;` and the dead `DataToBase64(IFileListEntry)` method (zero callers
  anywhere in the solution).

## Validation performed

1. `dotnet build eShopOnWeb.sln` — **Build succeeded, 0 Warning(s), 0 Error(s)** across all 11
   projects (10 app/test projects + implicit restore of shared props).
2. `dotnet test eShopOnWeb.sln --no-build`:
   - `UnitTests.dll` — 44/44 passed
   - `IntegrationTests.dll` — 3/3 passed
   - `FunctionalTests.dll` — 12/12 passed
   - `PublicApiIntegrationTests.dll` — 15/15 passed
   - **Total: 74/74 passed, 0 failed, 0 skipped**
3. `dotnet list <project> package --vulnerable --include-transitive` run across all 10 projects
   (solution-level command only reported 6 of them; ran remaining 4 — PublicApi, BlazorAdmin,
   BlazorShared, PublicApiIntegrationTests — individually) — **no vulnerable packages found**.

## Notable investigation (not a code change, but load-bearing for the fix)

Confirmed via a throwaway reflection console app (`/tmp/inspect3`, referencing `MinimalApi.Endpoint`
1.3.0 as a real `PackageReference`) that `IServiceCollectionExtensions.AddEndpoints()` only exposes
the parameterless `AddEndpoints(IServiceCollection)` overload — there is no way to scope which
assemblies it scans from the calling code, which is why the fix had to remove the offending
assemblies from the test project's own build output instead.
