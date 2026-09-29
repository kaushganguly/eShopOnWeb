# 04-upgrade-test-projects — Progress Details

## Summary

All 4 test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) build
cleanly on net10.0 and all tests pass. TFM/package versions were already resolved by the shared
`Directory.Packages.props` changes made in task 02. This task focused on fixing analyzer warnings
surfaced by the upgraded test tooling, and a genuine .NET 10 runtime regression discovered only via
`dotnet test` execution (not caught by static assessment).

## Changes made

### 1. xUnit2013 analyzer warnings (upgraded `xunit.analyzers` via `xunit` 2.9.3 bump)
Fixed "Do not use Assert.Equal() to check for collection size. Use Assert.Single/Assert.Empty instead."
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` — 2×
  `Assert.Equal(1, ...)` → `Assert.Single(...)`.
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` — `Assert.Equal(0,...)`
  → `Assert.Empty(...)`.
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` — `Assert.Equal(0,...)` →
  `Assert.Empty(...)`.

### 2. MSTEST0044 analyzer warning (upgraded `MSTest.TestFramework`/`TestAdapter` to 4.4.1)
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` —
  `[DataTestMethod]` (obsolete alias) → `[TestMethod]` on `SuccessFullMutipleParallelCall`.

### 3. CS0433 `Program` type ambiguity (net10 SDK regression) — root cause + fix
`PublicApiIntegrationTests.csproj` originally referenced both `PublicApi.csproj` and `Web.csproj`.
`ProgramTest.cs` used the bare, unqualified `Program` type with `WebApplicationFactory<Program>`.

Root cause, confirmed via reflection probe comparing the compiled `Web.dll` under net8.0 vs net10.0:
- **net8.0**: the compiler-generated top-level-statements `Program` class in `Web.dll` is `internal`
  (`Type.IsNotPublic == true`) — not visible outside the assembly, so `Program` resolved unambiguously
  to `PublicApi.Program` (which has an explicit `public partial class Program { }` marker at
  `PublicApi/Program.cs:181` for testability).
- **net10.0**: the same top-level-statements `Program` class in `Web.dll` is now `public`
  (`Type.IsPublic == true`) with no source change — a compiler/SDK behavior change between the .NET 8
  and .NET 10 toolchains. This made `Web.Program` and `PublicApi.Program` both publicly visible from
  `PublicApiIntegrationTests`, causing `CS0433: The type 'Program' exists in both 'PublicApi...' and
  'Web...'`.

Verified via `git stash` + rebuild on the original (pre-upgrade) net8.0 code that this ambiguity does
**not** exist there — confirmed regression, not a pre-existing latent bug.

**Fix applied**: Investigated whether the `Web.csproj` reference was even needed by this test project.
Found exactly one file depending on a `Web` type: `CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
used `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel` purely as a loose JSON-deserialization
target for a PublicApi response (duck-typed via matching `CatalogItems` property), when the PublicApi
project already exposes an equivalent, purpose-built response type
`Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints.ListPagedCatalogItemResponse` (same shape: a
`CatalogItems` collection) that is used elsewhere in the same test file. Replaced the `Web.ViewModels`
usage with the PublicApi response type and removed the `Web.csproj` `ProjectReference` entirely from
`PublicApiIntegrationTests.csproj`, restoring an unambiguous `Program` reference (resolves to
`PublicApi.Program`) — no `extern alias` workaround was needed once the unnecessary dependency was
removed.

Two other files in this test project (`AuthEndpoints/AuthenticateEndpointTest.cs`,
`CatalogItemEndpoints/CreateCatalogItemEndpointTest.cs`, `DeleteCatalogItemEndpointTest.cs`) depend on
`BlazorShared.Models.CatalogItem` (a plain DTO), so a direct `ProjectReference` to
`src/BlazorShared/BlazorShared.csproj` was added in place of the transitive one that used to come via
`Web.csproj`.

### 4. Secondary regression uncovered by the same investigation: unmaintained `BlazorInputFile` package
While diagnosing the `Program` ambiguity, an intermediate fix (using `extern alias` to keep the `Web`
reference) surfaced a **second, more severe** net10 runtime regression at `dotnet test` time (not caught
by build or by `generate_dotnet_upgrade_assessment`):

```
System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
Could not load type 'ReadRequest' from assembly 'BlazorInputFile, ... because it contains an object
field at offset 4 that is incorrectly aligned or overlapped by a non-object field.
```

and, after removing `BlazorInputFile`, the same failure mode recurred against a **first-party** WASM
assembly:

```
Could not load type 'WebAssembly.JSInterop.JSCallInfo' from assembly
'Microsoft.JSInterop.WebAssembly, Version=10.0.12.0, ...' because it contains an object field at
offset 4 that is incorrectly aligned or overlapped by a non-object field.
```

Cause: `PublicApi/Program.cs` calls `builder.Services.AddEndpoints()` (from the third-party
`MinimalApi.Endpoint` package), which enumerates **every currently-loaded assembly in the AppDomain**
via `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())` to discover endpoint classes.
Because `PublicApiIntegrationTests` also referenced `Web.csproj`, and `Web.csproj` references
`BlazorAdmin.csproj` (a `Microsoft.NET.Sdk.BlazorWebAssembly` project), the WASM-targeted assemblies
`BlazorAdmin.dll`, its (now-removed) dependency `BlazorInputFile.dll`, and the framework
`Microsoft.JSInterop.WebAssembly.dll` all end up loaded into the same in-process test host
(`WebApplicationFactory<Program>` runs the app in-process). Both `BlazorInputFile.dll` (an unmaintained,
single-version 2019-era package built for early Mono/Blazor, latest available version confirmed via
NuGet API is still 0.2.0 — no fix exists) and `Microsoft.JSInterop.WebAssembly.dll` (Microsoft's own
WASM-interop assembly, never intended to run under the standard CoreCLR JIT) use struct layouts that
.NET 10's stricter runtime type-loading validation now rejects when loaded outside a browser-wasm
(Mono) runtime. This was **not surfaced by `generate_dotnet_upgrade_assessment`** (static analysis) —
only discovered by actually executing `dotnet test`.

Since removing the unneeded `Web.csproj` reference (see item 3 above) already eliminates
`BlazorAdmin`/`Microsoft.JSInterop.WebAssembly` from the `PublicApiIntegrationTests` process, this
second issue is fully resolved as a side effect. Additionally, `BlazorInputFile` itself was found to be
genuinely dead code:
- `grep` confirmed the only source-level uses were `src/BlazorShared/Models/CatalogItem.cs`
  (`DataToBase64(IFileListEntry)`, never called from any `.cs`/`.razor` file in the repo) and a bare
  `@using BlazorInputFile` in `src/BlazorAdmin/_Imports.razor` (no razor page in the repo uses
  `<InputFile>`/`IFileListEntry`).
- Removed as unused, unmaintained (0.2.0 confirmed as latest on NuGet, released ~2019, superseded by the
  first-party `Microsoft.AspNetCore.Components.Forms.InputFile`/`IBrowserFile` since .NET 5), and
  incompatible with net10 runtime type-loading:
  - Removed `PackageReference Include="BlazorInputFile"` from `src/BlazorAdmin/BlazorAdmin.csproj` and
    `src/BlazorShared/BlazorShared.csproj`.
  - Removed `PackageVersion Include="BlazorInputFile"` from `Directory.Packages.props`.
  - Removed `@using BlazorInputFile` from `src/BlazorAdmin/_Imports.razor`.
  - Removed the unused `DataToBase64(IFileListEntry)` method (and its now-unneeded `using
    System.Threading.Tasks;` / `using BlazorInputFile;`) from `src/BlazorShared/Models/CatalogItem.cs`.

## Build results (all clean, 0 errors)

| Project | Errors | Warnings |
|---|---|---|
| UnitTests | 0 | 0 |
| IntegrationTests | 0 | 0 |
| FunctionalTests | 0 | 0 (only unavoidable NU1901, see task 03 notes) |
| PublicApiIntegrationTests | 0 | 0 (only unavoidable NU1901) |

## Test results (all passing)

| Project | Passed | Failed | Total |
|---|---|---|---|
| UnitTests | 44 | 0 | 44 |
| IntegrationTests | 3 | 0 | 3 |
| FunctionalTests | 12 | 0 | 12 |
| PublicApiIntegrationTests | 15 | 0 | 15 |
| **Total** | **74** | **0** | **74** |

Verified with a full-solution `dotnet build eShopOnWeb.sln` (Build succeeded, 0 errors) and full-solution
`dotnet test eShopOnWeb.sln` (74/74 passed) after all changes.

## Files modified in this task

- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
- `tests/PublicApiIntegrationTests/ProgramTest.cs`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/BlazorAdmin/_Imports.razor`
- `src/BlazorShared/BlazorShared.csproj`
- `src/BlazorShared/Models/CatalogItem.cs`
- `Directory.Packages.props` (removed `BlazorInputFile` PackageVersion entry)
