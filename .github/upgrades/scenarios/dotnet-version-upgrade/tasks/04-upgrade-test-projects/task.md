# 04-upgrade-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests to net10.0

Update the 4 test projects (`tests/UnitTests`, `tests/IntegrationTests`,
`tests/FunctionalTests`, `tests/PublicApiIntegrationTests`) to net10.0 (inherited from
the central `Directory.Packages.props` change in task 01) and fix any xUnit/MSTest
analyzer-flagged assertion patterns and runtime issues surfaced by the newer test SDK
(`Microsoft.NET.Test.Sdk` 18.10.1, `xunit` 2.9.3, `MSTest` 4.4.1).

**Done when**: All 4 test projects target `net10.0`, build with 0 errors/warnings, and
`dotnet test` passes for all of them.

## Research Findings

- **xUnit analyzer fixes** (`xunit` 2.9.3 ships stricter analyzers,
  `xUnit2013`/`xUnit2017`-style rules): replaced `Assert.Equal(0, collection.Count)` with
  `Assert.Empty(collection)`, and `Assert.Equal(1, collection.Count)` with
  `Assert.Single(collection)` in:
  - `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
  - `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
  - `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
    (2 occurrences)
  These are the more idiomatic/analyzer-recommended assertions and are behaviorally
  equivalent — no semantic change to what's being tested.
- **MSTest `[DataTestMethod]` merge**: `MSTest` 4.x merged `[DataTestMethod]` into
  `[TestMethod]` (a `DataRow`-decorated method no longer needs the separate attribute) —
  updated `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`.
- **`PublicApiIntegrationTests` — `Program` type ambiguity / WASM assembly clash**: this
  project references both `PublicApi.csproj` (for its `Program` entry point, used by
  `WebApplicationFactory<Program>`) and `Web.csproj` (only for
  `Microsoft.eShopWeb.Web.ViewModels` types used in JSON deserialization assertions).
  Two distinct problems surfaced on net10.0:
  1. Both `PublicApi` and `Web` define a top-level `Program` class (implicit
     entry-point classes), causing an ambiguous-reference compile error once both
     assemblies are referenced normally. Fixed via extern alias: added
     `<Aliases>global,PublicApiAlias</Aliases>` to the `PublicApi` `ProjectReference`
     in `PublicApiIntegrationTests.csproj`, `extern alias PublicApiAlias;` in
     `ProgramTest.cs`, and qualified the type as `PublicApiAlias::Program`.
  2. `Web.csproj` transitively references `BlazorAdmin` (a Blazor WebAssembly app),
     which pulls mono/wasm-runtime-targeted assemblies into this test project's build
     output. `MinimalApi.Endpoint`'s `AddEndpoints()` (invoked from `PublicApi`'s
     `Program.cs`) reflection-scans every assembly in the test host's base directory,
     and loading the WASM-targeted assemblies on desktop CoreCLR throws a
     `TypeLoadException` ("incorrectly aligned or overlapped by a non-object field")
     under .NET 10's stricter struct-layout validation. Fixed by adding an MSBuild
     target (`RemoveBlazorWebAssemblyAssembliesFromOutput`, `AfterTargets`
     `ResolveReferences;ResolveProjectReferences`) to `PublicApiIntegrationTests.csproj`
     that strips `BlazorAdmin`/`*WebAssembly*`/`Blazored.*` assemblies from
     `@(ReferenceCopyLocalPaths)` — `PublicApi`'s `Program.cs` does not depend on any of
     these at runtime, so excluding them from the test host's output directory is safe
     and keeps the reflection scan limited to genuinely relevant assemblies.
- No code changes were needed in `tests/FunctionalTests` beyond the TFM/package version
  inheritance from `Directory.Packages.props`.

## Validation

- `dotnet build`/`dotnet test` for all 4 test projects — see progress-details.md for
  results.
