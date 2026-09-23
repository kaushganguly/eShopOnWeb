# Task 02 — Upgrade all projects to .NET 10.0

## Summary
Upgraded the centrally-managed target framework and all centrally-managed NuGet package
versions from net8.0 to net10.0, and resolved all resulting build breaks across the
solution (ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared,
UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests).

Final result: **solution builds with 0 errors**; only residual warnings are an
unfixable low-severity transitive NuGet advisory (see "Accepted residual risk" below).

## Key discovery: `Directory.Packages.props` drives every project's TFM
No `.csproj` in this repo declares its own `<TargetFramework>`, and there is no
`Directory.Build.props`. The single `<TargetFramework>` property inside
`Directory.Packages.props` is what actually targets all 10 projects. Bumping it there
(net8.0 → net10.0) retargeted the whole solution in one edit — no per-project TFM edits
were required.

## Package/TFM changes (`Directory.Packages.props`)
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion` / `SystemExtensionVersion` / `EntityFramworkCoreVersion`: → `10.0.12`
- `VSCodeGeneratorVersion` (Microsoft.VisualStudio.Web.CodeGeneration.Design): → `10.0.2`
  (latest stable available; see accepted residual risk below)
- ~20 other centrally-managed package versions bumped to their latest net10.0-compatible
  stable release (Swashbuckle.AspNetCore 10.2.3, AutoMapper 16.2.0, Blazored.LocalStorage
  4.5.0, Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.23.0, Azure.Identity
  1.21.0, System.Text.Json 10.0.1, etc.)
- Removed orphan/deprecated/redundant entries:
  - Stray `Microsoft.AspNetCore.Mvc` 2.2.0 entry (unused, would have downgraded pinned MVC)
  - `BlazorInputFile` (abandoned package, no net10-compatible version — replaced with the
    built-in `IBrowserFile` API from `Microsoft.AspNetCore.Components.Web`)
  - `System.Security.Claims`, `System.Text.Json` (ApplicationCore), `System.Net.Http.Json`
    (BlazorAdmin) — all now part of the shared framework in net10, kept as
    PackageReferences would trigger NU1510 "will not be pruned" warnings
  - `AutoMapper.Extensions.Microsoft.DependencyInjection` — genuinely abandoned at 12.0.1
    per NuGet API (confirmed no newer version exists); replaced with plain `AutoMapper`
    16.2.0, which has bundled the `AddAutoMapper` DI extension since v12+
- Added `Microsoft.AspNetCore.Components` / `Microsoft.AspNetCore.Components.Web` (needed
  for `IBrowserFile`, replacing `BlazorInputFile`)

## Per-project file changes
- `src/ApplicationCore/ApplicationCore.csproj`: removed `System.Security.Claims`,
  `System.Text.Json` PackageReferences (now in shared framework)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: removed the
  obsolete `SerializationInfo/StreamingContext` legacy binary-serialization constructor
  (SYSLIB0051 warning) — this was dead code (never invoked, no `[Serializable]` binary
  formatter usage anywhere else in the app), so it was removed rather than suppressed,
  per the anti-suppression rule
- `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`: swapped
  `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`
- `src/PublicApi/Program.cs`:
  - `Microsoft.OpenApi.Models` namespace → `Microsoft.OpenApi` (Microsoft.OpenApi library
    v1→v2 major rewrite pulled in transitively by Swashbuckle.AspNetCore 10.x)
  - `AddAutoMapper(Assembly)` → `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)`
    (AutoMapper 16.x removed the single-Assembly-parameter overload; all overloads now
    require an explicit `configAction` first)
  - Rewrote the Swagger `AddSecurityRequirement` block: the new Microsoft.OpenApi v2 API
    requires `OpenApiSecuritySchemeReference(referenceId, hostDocument, externalResource)`
    and `AddSecurityRequirement` now takes `Func<OpenApiDocument, OpenApiSecurityRequirement>`
    (needs document context to resolve the reference) instead of a plain object
- `src/PublicApi/CustomSchemaFilters.cs`: `ISchemaFilter.Apply(OpenApiSchema, ...)` →
  `ISchemaFilter.Apply(IOpenApiSchema, ...)` (interface-based mutable/reference split in
  Microsoft.OpenApi v2), with null-safe `Properties?.ContainsKey` guards
- `src/BlazorShared/BlazorShared.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj`: removed
  `BlazorInputFile` PackageReference; added `Microsoft.AspNetCore.Components`/`.Web` to
  BlazorShared
- `src/BlazorShared/Models/CatalogItem.cs`: rewrote `DataToBase64` from the abandoned
  `BlazorInputFile.IFileListEntry` API to the built-in `IBrowserFile`/`OpenReadStream()`
- `src/BlazorAdmin/_Imports.razor`: removed `@using BlazorInputFile`
- `tests/FunctionalTests/FunctionalTests.csproj`: removed stale legacy
  `DotNetCliToolReference` element (obsolete tooling construct, unrelated to any package
  version, safe removal)
- Minor test-only warning fixes (xUnit2013 / MSTEST0044), not behavior changes:
  - `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
    (x2): `Assert.Equal(1, ...Count)` → `Assert.Single`, `Assert.Equal(0, ...Count)` →
    `Assert.Empty`
  - `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: same
  - `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: same
  - `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`:
    `[DataTestMethod]` → `[TestMethod]` (MSTest merged these attributes; MSTEST0044)

## Genuine .NET 10 SDK behavior change discovered and fixed
**Top-level-statement `Program` class visibility changed from `internal` to `public`** for
`Microsoft.NET.Sdk.Web` projects targeting net10.0 (verified empirically: identical
`Microsoft.NET.Sdk.Web` project, same installed SDK, only the `<TargetFramework>` value
changed between net8.0/net10.0 — net8.0 produces an `internal` compiler-generated
`Program` type, net10.0 produces a `public` one).

This surfaced a new `CS0433` ambiguous-type error in `PublicApiIntegrationTests`, which
references both `PublicApi.csproj` (already explicitly `public partial class Program {}`)
and, transitively through a `Web.csproj` reference, `Web`'s now-also-public `Program`.

**Resolution (final):** rather than adding an assembly alias to disambiguate, the
`Web.csproj` `ProjectReference` was removed entirely from
`PublicApiIntegrationTests.csproj`. It was only needed for one file
(`CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`) to deserialize a JSON response
into `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel`/`Web`'s
`ListPagedCatalogItemResponse`-shaped view model — but `PublicApi` already defines its own
native `ListPagedCatalogItemResponse` (in `Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints`,
already imported in that file) with an equivalent `CatalogItems` collection shape. Switching
that one test file to deserialize into `PublicApi`'s own response type removed the need for
the `Web` reference altogether, which:
1. Eliminates the `Program` ambiguity at its root (no more cross-referencing two projects
   that both expose a `Program` type)
2. Eliminates a runtime crash this cross-reference was silently causing (see below)

### Runtime crash this also fixed
With the `Web.csproj` reference in place, running `PublicApiIntegrationTests` crashed with:
```
System.Reflection.ReflectionTypeLoadException ... Could not load type
'WebAssembly.JSInterop.JSCallInfo' from assembly 'Microsoft.JSInterop.WebAssembly,
Version=10.0.12.0' because it contains an object field at offset 4 that is incorrectly
aligned or overlapped by a non-object field.
```
Root cause: `PublicApiIntegrationTests` → `Web.csproj` → `BlazorAdmin.csproj` →
`Microsoft.AspNetCore.Components.WebAssembly` (and its `Microsoft.JSInterop.WebAssembly`
dependency) — a browser/WASM-only assembly — ended up copied into the shared test-host
output directory. The third-party `MinimalApi.Endpoint` package's `AddEndpoints()`
extension method calls the un-scoped `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a
=> a.GetTypes())` with no per-assembly exception handling, so it blew up the moment
`Microsoft.JSInterop.WebAssembly` (whose `JSCallInfo` struct apparently fails a stricter
.NET 10 runtime struct-layout validation when loaded outside an actual browser/WASM host)
was present in the AppDomain. Removing the unnecessary `Web.csproj` reference (see above)
means this WASM-only assembly is never loaded into the `PublicApiIntegrationTests` process,
resolving the crash without touching the third-party package or `Web`'s own behavior.

## Accepted residual risk
`Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (latest available stable; next
is `11.0.0-preview`) transitively pulls `NuGet.Packaging`/`NuGet.Protocol` 6.12.1, which
carry a documented **low-severity** advisory
(`GHSA-g4vj-cjjj-v7hg`). This is a design-time-only Visual Studio scaffolding tool
dependency (not part of the shipped/runtime application), and there is no newer stable
version to upgrade to. Documented here as an accepted low-severity residual risk rather
than a build blocker — the originally-flagged Moderate/High severity CVEs
(`Azure.Identity` 1.10.4, `System.Text.Json` 8.0.3) are fully resolved by the version
bumps to 1.21.0 / 10.0.1 respectively.

## Validation
- `dotnet build eShopOnWeb.sln -c Release`: **0 errors**, only the NU1901 warnings above
  remain (24 total, all the same advisory repeated per-project)
- `dotnet test eShopOnWeb.sln -c Release --no-build`: **74/74 passed** (44 UnitTests + 3
  IntegrationTests + 12 FunctionalTests + 15 PublicApiIntegrationTests) — matches the
  net8.0 baseline tally exactly
