# 04-test-projects: 04-test-projects

Execute task 04-test-projects.

## Scope

Build and fix all 4 test projects on net10.0:
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Research Findings

### xUnit analyzer rule tightening (xUnit2013)
Upgrading `xunit`/`xunit.runner.visualstudio` from 2.7.0/2.5.6 to 2.9.3/4.0.0 activated the
`xUnit2013` analyzer rule ("Do not use Assert.Equal() to check for collection size"). This
surfaced as new warnings (not errors) in 2 projects:
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
  (2 occurrences: `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)`;
  `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)`)
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
  (`Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`)
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
  (`Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`)

All fixed by using the semantically-correct assertion instead of suppressing the warning.

### CS0433 ambiguous `Program` type in PublicApiIntegrationTests (net10.0-specific breaking change)
`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` references both
`src/PublicApi/PublicApi.csproj` and `src/Web/Web.csproj` (the latter needed for
`Microsoft.eShopWeb.Web.ViewModels` types used by test code). `ProgramTest.cs` uses
`WebApplicationFactory<Program>` intending to target `PublicApi`'s explicit
`public partial class Program { }`.

On **net8.0** this compiled without ambiguity because C# top-level-statement programs
generate an *internal* `Program` class by default — `Web`'s implicit `Program` was not
accessible outside its own assembly, so only `PublicApi.Program` (explicitly declared
`public`) was visible to the test project.

On **net10.0**, verified via reflection (`Assembly.GetType("Program").IsPublic`) that the
SDK/compiler now generates the implicit top-level-statement `Program` class as **public**
by default. This makes `Web.Program` and `PublicApi.Program` both accessible from the test
project simultaneously, producing `CS0433: The type 'Program' exists in both 'PublicApi...'
and 'Web...'`. Confirmed via git-stash A/B rebuild on net8.0 vs net10.0 that this is a
genuine net8→net10 compiler/SDK behavior change, not a pre-existing issue.

**Fix applied** (no suppression, disambiguates properly): added a project-reference alias so
`PublicApi`'s assembly is imported under both `global` (to keep all existing unqualified
`Microsoft.eShopWeb.PublicApi.*` usages working) and a named alias `PublicApiAlias`:
```xml
<ProjectReference Include="..\..\src\PublicApi\PublicApi.csproj">
  <Aliases>global,PublicApiAlias</Aliases>
</ProjectReference>
```
Then in `ProgramTest.cs`, added `extern alias PublicApiAlias;` and changed
`WebApplicationFactory<Program>` → `WebApplicationFactory<PublicApiAlias::Program>` to
explicitly and unambiguously target `PublicApi`'s `Program` class.

### MSTEST0044 analyzer warning (PublicApiIntegrationTests, MSTest framework)
`tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` used
`[DataTestMethod]` which is obsolete in the upgraded MSTest analyzer package (newer MSTest
merged `DataTestMethod` behavior into `[TestMethod]`, which now supports `[DataRow]` directly).
Fixed by replacing `[DataTestMethod]` with `[TestMethod]`.

### Runtime failure: `BlazorInputFile` type-layout incompatibility (net10.0-specific, dead code removed)
Before reaching the `Microsoft.JSInterop.WebAssembly` issue above, an earlier run of
`PublicApiIntegrationTests` failed all 15 tests with a similar `ReflectionTypeLoadException`,
this time against the long-abandoned third-party package `BlazorInputFile` 0.2.0 (last released
~2018, predates Blazor's built-in `InputFile` component):
```
Could not load type 'ReadRequest' from assembly 'BlazorInputFile, ...' because it contains an
object field at offset 4 that is incorrectly aligned or overlapped by a non-object field.
```
Same root mechanism as the JSInterop issue: `AddEndpoints()`'s AppDomain-wide `GetTypes()` scan
now fails hard on old assemblies with borderline type layouts that net10.0's stricter CLR
verification rejects (tolerated on net8.0).

Investigated usage: `BlazorInputFile` was referenced by `src/BlazorShared/BlazorShared.csproj`
and `src/BlazorAdmin/BlazorAdmin.csproj`, imported in `src/BlazorAdmin/_Imports.razor`, and used
by exactly one method, `CatalogItem.DataToBase64(IFileListEntry)` in
`src/BlazorShared/Models/CatalogItem.cs`. Repo-wide grep confirmed **zero callers** of
`DataToBase64` and **zero** `<InputFile>`/`IFileListEntry` usage in any `.razor` markup — fully
dead code, safe to delete outright rather than suppress or work around.

**Fix applied**: removed the dead `DataToBase64` method and its `using BlazorInputFile;`/
`using System.Threading.Tasks;` imports from `CatalogItem.cs`; removed
`@using BlazorInputFile` from `BlazorAdmin/_Imports.razor`; removed the `BlazorInputFile`
`PackageReference` from both `BlazorAdmin.csproj` and `BlazorShared.csproj`; removed the
`BlazorInputFile` `PackageVersion` entry from `Directory.Packages.props`. This eliminated the
`BlazorInputFile.dll` from ever being loaded, in addition to being a legitimate dead-code cleanup.

### Runtime failure: `Microsoft.JSInterop.WebAssembly` ReflectionTypeLoadException (net10.0-specific)
After all 4 projects built cleanly, running `dotnet test` on `PublicApiIntegrationTests`
initially **failed all 15 tests** at runtime (not build time) with:
```
System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
Could not load type 'WebAssembly.JSInterop.JSCallInfo' from assembly
'Microsoft.JSInterop.WebAssembly, Version=10.0.12.0' because it contains an object field at
offset 4 that is incorrectly aligned or overlapped by a non-object field.
```
thrown from `MinimalApi.Endpoint.Extensions.IServiceCollectionExtensions.AddEndpoints()`
(a third-party NuGet package used by `PublicApi/Program.cs`), which unconditionally executes
`AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())` with no
`ReflectionTypeLoadException` handling.

**Root cause chain** (confirmed via git-stash A/B test on net8.0 vs net10.0 and reflection probes):
1. `PublicApiIntegrationTests.csproj` had a `ProjectReference` to `src/Web/Web.csproj`,
   originally added only to use one POCO type, `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel`,
   for JSON-deserializing the `/api/catalog-items` response in `CatalogItemListPagedEndpoint.cs`
   (the adjacent test method in the same file already used the equivalent, already-available
   `PublicApi` DTO `ListPagedCatalogItemResponse` instead — this was leftover inconsistency).
2. Because of that reference, `Web.dll`'s full transitive dependency graph — including
   `BlazorAdmin.dll` (a Blazor **WebAssembly** SDK project) and its dependency
   `Microsoft.JSInterop.WebAssembly.dll` — was copied into `PublicApiIntegrationTests`'s own
   `bin/Debug/net10.0/` output and got loaded into the shared test-host `AppDomain` (ASP.NET Core
   hosting/`WebApplicationFactory` loads the full dependency-context closure of the test process
   for MVC ApplicationPart/RelatedAssembly discovery).
3. `Microsoft.JSInterop.WebAssembly.dll` is built exclusively for the Mono/WASM browser runtime.
   On **net8.0**, CoreCLR's type loader tolerated calling `GetTypes()` on it; on **net10.0**,
   stricter type-layout validation in the CLR now throws `ReflectionTypeLoadException` for this
   assembly's interop types when loaded into a normal (non-WASM) CoreCLR process — a genuine
   net8→net10 runtime breaking change, verified not present in the net8.0 baseline.
4. `FunctionalTests` (which also references `Web.csproj`/`BlazorAdmin.csproj` and therefore also
   has this DLL in its output) did NOT hit the failure because its only PublicApi-hosting test
   fixture (`ApiTestFixture`/`TestApiApplication`) is defined but its sole consuming test class
   (`AuthenticateEndpoint.cs`) is entirely commented out — so `AddEndpoints()`'s AppDomain-wide
   scan is never actually triggered from that project.

**Fix applied** (root-cause fix, no suppression, package/vendor code unmodified):
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`:
  changed `ReturnsFirst10CatalogItems` to deserialize into `ListPagedCatalogItemResponse`
  (already used one test below it, defined directly in `PublicApi`) instead of
  `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel`; removed the now-unnecessary
  `using Microsoft.eShopWeb.Web.ViewModels;`.
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: removed the
  `ProjectReference` to `src/Web/Web.csproj` entirely (no longer needed by any file in this
  project — verified via repo-wide grep for `Microsoft.eShopWeb.Web.` usages).
- Reverted the now-unnecessary `extern alias`/`Aliases` workaround for the `Program` type
  ambiguity (see below) in `ProgramTest.cs` and the `.csproj`, since removing the `Web`
  reference eliminates that ambiguity at its source too.
- Verified `Microsoft.JSInterop.WebAssembly.dll` is no longer present in
  `tests/PublicApiIntegrationTests/bin/Debug/net10.0/` after a clean rebuild, and all 15
  `PublicApiIntegrationTests` now pass.

### (Superseded) CS0433 ambiguous `Program` type note
Earlier in this task, before the root-cause fix above, `PublicApiIntegrationTests` (which then
still referenced both `PublicApi.csproj` and `Web.csproj`) hit `CS0433: The type 'Program'
exists in both 'PublicApi...' and 'Web...'`, because **net10.0's compiler/SDK now generates the
implicit top-level-statement `Program` class as `public` by default** (verified via reflection:
`net8.0` → `internal`, `net10.0` → `public`) — a second confirmed net8→net10 breaking change.
This was initially worked around with an `extern alias`, but once the `Web.csproj` reference was
removed entirely (see above), the ambiguity disappeared naturally and the workaround was reverted
for simplicity. This class of ambiguity would still need the `extern alias` technique in any
other project that must legitimately reference two class libraries which both use top-level
statements.

## Result
All 4 test projects build with 0 warnings / 0 errors on net10.0. Full solution build
(`dotnet build eShopOnWeb.sln`) also confirmed clean: 0 Warnings, 0 Errors. Full test suite
(`dotnet test eShopOnWeb.sln`) passes: UnitTests 44/44, IntegrationTests 3/3,
FunctionalTests 12/12, PublicApiIntegrationTests 15/15 — 74/74 total.
