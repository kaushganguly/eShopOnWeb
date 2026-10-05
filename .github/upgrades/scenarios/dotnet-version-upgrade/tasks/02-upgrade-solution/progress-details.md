# Progress — 02-upgrade-solution

## Summary

All 10 projects migrated from `net8.0` to `net10.0`. NuGet packages bumped to net10.0-compatible
versions via `Directory.Packages.props` (central package management). Fixed all resulting
compile errors, a runtime regression, and all build warnings. Final state: solution builds with
0 errors / 0 code warnings (only pre-existing/deferred NuGet security-advisory warnings for one
package remain — see "Deferred risks" below). All 74 tests across all 4 test projects pass.

## Files modified

### Core upgrade
- `global.json` — SDK `8.0.x` → `10.0.x`.
- `Directory.Packages.props` — central `TargetFramework` → `net10.0`; version properties
  (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`)
  bumped to net10.0-compatible releases; ~20 individual `PackageVersion` bumps (Ardalis.* family,
  FluentValidation, MediatR, Swashbuckle.*, full test stack, Azure.Identity, etc.); removed unused
  `Microsoft.AspNetCore.Mvc` entry and 3 now-redundant entries pruned by the .NET 9+ SDK
  (`System.Security.Claims`, `System.Text.Json`, `System.Net.Http.Json` — now shipped in the
  shared framework); removed `BlazorInputFile` (dead package, see below); added explicit
  `NuGet.Packaging`/`NuGet.Protocol` version override to `6.12.5` (security fix, see below).
- `src/ApplicationCore/ApplicationCore.csproj` — removed redundant `PackageReference` for
  `System.Security.Claims`, `System.Text.Json` (NU1510 pruning).
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed redundant `PackageReference` for
  `System.Net.Http.Json` (NU1510 pruning); removed `BlazorInputFile` reference.
- `src/BlazorShared/BlazorShared.csproj` — removed `BlazorInputFile` reference.
- `src/PublicApi/PublicApi.csproj`, `src/Web/Web.csproj` — added explicit `NuGet.Packaging` /
  `NuGet.Protocol` `PackageReference` entries to force resolution to the patched `6.12.5` (both
  projects pull `Microsoft.VisualStudio.Web.CodeGeneration.Design`, whose own dependency graph
  resolves the vulnerable `6.12.1`).
- `src/Web/Dockerfile`, `src/PublicApi/Dockerfile` — base images `8.0` → `10.0`.
- `.github/workflows/dotnetcore.yml`, `.github/workflows/modernize.yml` — CI `dotnet-version`
  `8.0.x` → `10.0.x`.

### Breaking-change fixes (compile errors)
- `src/PublicApi/Program.cs`:
  - `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;` (Swashbuckle.AspNetCore 10.x
    now depends on Microsoft.OpenApi v2, which moved/merged the `.Models` namespace into the root
    `Microsoft.OpenApi` namespace).
  - Replaced the manual `OpenApiSecurityScheme { Reference = new OpenApiReference {...} }` pattern
    (no longer valid — `OpenApiSecurityScheme` has no `Reference` property in v2) with the new
    `OpenApiSecuritySchemeReference("Bearer")` reference type.
  - `AddSecurityRequirement(new OpenApiSecurityRequirement {...})` → wrapped in a factory delegate
    `AddSecurityRequirement(_ => new OpenApiSecurityRequirement {...})` — the Swashbuckle v10 API
    changed this overload to accept `Func<OpenApiDocument, OpenApiSecurityRequirement>`.
- `src/PublicApi/CustomSchemaFilters.cs`:
  - `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;`.
  - `ISchemaFilter.Apply(OpenApiSchema schema, ...)` → `Apply(IOpenApiSchema schema, ...)` — the
    interface signature changed to the new `IOpenApiSchema` interface-based model in v2.
  - Added a null-guard for `schema.Properties` — OpenAPI.NET v2 no longer auto-initializes
    collections on model objects (performance change), so `Properties` can be `null`.

### Runtime-regression fixes (discovered via `dotnet test`, not caught by build)
1. **Ambiguous `Program` type (CS0433)** — `tests/PublicApiIntegrationTests/ProgramTest.cs` uses
   `WebApplicationFactory<Program>`. Under `net8.0`, the compiler-generated top-level-statement
   `Program` class defaults to `internal`, so only `PublicApi/Program.cs`'s explicit
   `public partial class Program { }` was visible/ambiguous-free. Under the `net10.0` SDK, the
   compiler-generated top-level `Program` class in `src/Web/Program.cs` became implicitly
   `public` too, causing an ambiguous-type clash with `PublicApi`'s intentionally-public `Program`
   in any project (like this test project) referencing both. **Fix**: added an explicit
   `internal partial class Program { }` declaration at the end of `src/Web/Program.cs` to restore
   the net8.0 behavior and keep `PublicApi`'s `Program` as the sole public candidate.
2. **`BlazorInputFile` runtime load failure** — `ReflectionTypeLoadException` at `PublicApi`
   startup under test ("Could not load type 'ReadRequest' ... incorrectly aligned"). Root cause:
   `BlazorInputFile` 0.2.0 (unmaintained since 2019) ships an old assembly format incompatible
   with the stricter .NET 10 CLR type loader. Investigated actual usage: the package was **dead
   code** — only an unused `@using BlazorInputFile` in `src/BlazorAdmin/_Imports.razor` and an
   unused `DataToBase64(IFileListEntry fileItem)` helper method in
   `src/BlazorShared/Models/CatalogItem.cs` with zero call sites anywhere in the repo. **Fix**:
   removed the package entirely (`Directory.Packages.props`, `BlazorAdmin.csproj`,
   `BlazorShared.csproj`) and deleted the two now-unused references (`_Imports.razor` using
   directive; the whole `DataToBase64` method and its `using System.Threading.Tasks;` import in
   `CatalogItem.cs`).
3. **`Microsoft.JSInterop.WebAssembly` runtime load failure** — same symptom
   (`ReflectionTypeLoadException`, "incorrectly aligned ... non-object field") surfaced next,
   this time for `Microsoft.JSInterop.WebAssembly` (part of the Blazor WebAssembly client
   runtime). Root cause: `tests/PublicApiIntegrationTests` references both `PublicApi.csproj` and
   `Web.csproj`; `Web.csproj` references `BlazorAdmin.csproj` (a `Microsoft.NET.Sdk.BlazorWebAssembly`
   project), which transitively copies browser-wasm-only assemblies into the test project's own
   build output and runtime dependency manifest. ASP.NET Core's MVC `ApplicationPartManager`
   eagerly loads every assembly listed in the host's dependency context at startup (triggered by
   `WebApplicationFactory<Program>` booting the `PublicApi` host in-process); loading these
   browser-wasm assemblies on a server-side CoreCLR now throws under .NET 10 (previously
   tolerated under net8.0's looser type loading). None of the tests in this project use
   `BlazorAdmin` or Blazor WebAssembly APIs — only `Microsoft.eShopWeb.Web.ViewModels` and
   `BlazorShared.Models` types are referenced. **Fix**: added an MSBuild `Target` to
   `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
   (`ExcludeBlazorWebAssemblyRuntimeAssemblies`, running `AfterTargets="ResolveReferences;ResolveProjectReferences"`)
   that strips `BlazorAdmin`, `Microsoft.JSInterop.WebAssembly`,
   `Microsoft.AspNetCore.Components.WebAssembly`,
   `Microsoft.AspNetCore.Components.WebAssembly.Authentication`, and `Blazored.LocalStorage` from
   `ReferenceCopyLocalPaths` / `ReferencePath` / `ReferenceDependencyPaths`, so they are never
   copied into this test project's output or deps.json.

### Build-warning cleanup (all projects now build with 0 code warnings)
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` —
  `[DataTestMethod]` → `[TestMethod]` (MSTEST0044: `DataTestMethod` is obsolete in the newer
  MSTest analyzer version).
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — removed the obsolete
  `SerializationInfo`/`StreamingContext` formatter-based-serialization constructor (SYSLIB0051).
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` (×2),
  `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`,
  `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` — replaced
  `Assert.Equal(1, collection.Count)` / `Assert.Equal(0, collection.Count)` with
  `Assert.Single(collection)` / `Assert.Empty(collection)` (xUnit2013 analyzer rule, newer
  xunit.analyzers version bundled with the upgraded xunit/xunit.runner.visualstudio packages).

## Security vulnerabilities

| Package | Version | Severity | Status |
|---|---|---|---|
| Azure.Identity | 1.10.4 → 1.21.0 | Moderate | ✅ Fixed |
| System.Text.Json | 8.0.3 → framework-provided | High | ✅ Fixed |
| NuGet.Packaging / NuGet.Protocol | 6.12.1 → 6.12.5 (explicit override) | Low | ✅ Fixed |
| AutoMapper | 12.0.1 (unchanged) | High | ⚠️ Deferred (see below) |

### Deferred risk: AutoMapper GHSA-rvv3-g6hj-g44x (High, DoS via uncontrolled recursion)

- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 is the **last version ever
  published** of that package (no newer release exists to pair with a newer `AutoMapper`).
- The advisory's first-patched versions are `15.1.1` (or `16.1.1` for the 16.x branch).
- Verified via the official NuGet package pages that **AutoMapper 13.0.1 and all versions ≥ 13
  through the current latest (16.2.0) document a commercial license-key model** ("How do I set
  the license key?" / contact support if you're a paying customer) for `AddAutoMapper(...)` DI
  registration — this is a licensing/business decision that is out of scope for an autonomous
  framework upgrade to make unilaterally on the user's behalf.
- Decision: kept `AutoMapper` / `AutoMapper.Extensions.Microsoft.DependencyInjection` at `12.0.1`
  (last MIT-licensed version) and documented this as an accepted/deferred risk rather than
  force an upgrade with licensing implications. **Recommend the user evaluate an AutoMapper
  commercial license (or an alternative mapping library) in a follow-up, scoped piece of work.**

## Build & test verification

- `dotnet build eShopOnWeb.sln -c Release` → **Build succeeded**, 0 errors, 0 code warnings
  (12 NuGet security-advisory warnings remain, all for the deferred AutoMapper risk above).
- `dotnet test eShopOnWeb.sln -c Release --no-build` → **all 74 tests pass** across all 4 test
  projects (UnitTests: 44, IntegrationTests: 3, FunctionalTests: 12, PublicApiIntegrationTests: 15).
- `dotnet list eShopOnWeb.sln package --vulnerable --include-transitive` → only the deferred
  AutoMapper entry remains.
