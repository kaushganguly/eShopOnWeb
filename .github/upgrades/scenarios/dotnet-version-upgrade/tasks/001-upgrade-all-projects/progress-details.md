# Progress Details — 001-upgrade-all-projects

Status: **Completed**

## Summary

Upgraded the eShopOnWeb solution (all 10 projects) from .NET 8.0 to .NET 10.0 (LTS).
Full solution builds with 0 errors / 0 warnings (except two documented, accepted-risk
NuGet security advisories — see assessment.md). All 74 tests across UnitTests,
IntegrationTests, FunctionalTests, and PublicApiIntegrationTests pass.

## Changes Made

### SDK / TFM / Central Package Management
- `global.json`: SDK version `8.0.x` → `10.0.x`.
- `Directory.Packages.props`:
  - `TargetFramework` → `net10.0`.
  - `AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion` → `10.0.12`.
  - `VSCodeGeneratorVersion` → `10.0.2`.
  - Bumped all other centrally-managed package versions to their latest .NET-10-compatible
    releases (per `get_supported_package_version` / NuGet.org lookups).
  - Removed `Microsoft.AspNetCore.Mvc` 2.2.0 (orphaned/unused entry).
  - Removed `System.Net.Http.Json`, `System.Security.Claims`, `System.Text.Json` explicit
    `PackageVersion` entries — now shipped in the shared framework under net10.0 (fixes
    new NU1510 "will not be pruned" warnings).
  - Removed `BlazorInputFile` 0.2.0 (abandoned 2019-era package; see below).

### Licensing-aware package pinning (deliberate deviation from "always take latest")
- **MediatR** pinned to `12.5.0` (last Apache-2.0 version). `13.0.0+` requires a paid
  commercial license key.
- **AutoMapper** pinned to `14.0.0` (last MIT version). `15.0.0+` requires a paid
  commercial license key.
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` (which pins AutoMapper to
  an exact `[12.0.1]`, incompatible with CPM version bumping) with a direct reference to
  the core `AutoMapper` package in `src/Web/Web.csproj` and `src/PublicApi/PublicApi.csproj`
  — `AddAutoMapper(...)` has shipped in the core package since v12.

### Project file cleanup
- `src/ApplicationCore/ApplicationCore.csproj`: removed redundant `System.Security.Claims`
  / `System.Text.Json` `PackageReference`s (framework-provided under net10.0).
- `src/BlazorAdmin/BlazorAdmin.csproj`: removed redundant `System.Net.Http.Json`
  `PackageReference`; removed `BlazorInputFile` `PackageReference` (see below).
- `src/BlazorShared/BlazorShared.csproj`: removed `BlazorInputFile` `PackageReference`.
- `src/Web/Dockerfile`, `src/PublicApi/Dockerfile`: base images `8.0` → `10.0`.

### Breaking API changes fixed (Microsoft.OpenApi 2.x / Swashbuckle.AspNetCore 10.x)
- `src/PublicApi/CustomSchemaFilters.cs`: `Microsoft.OpenApi.Models` namespace removed →
  updated to `Microsoft.OpenApi`; `ISchemaFilter.Apply` parameter type `OpenApiSchema` →
  `IOpenApiSchema`; added null-check for `schema.Properties` (now nullable).
- `src/PublicApi/Program.cs`: updated `using Microsoft.OpenApi.Models` → `using
  Microsoft.OpenApi`; `OpenApiSecurityScheme.Reference` removed — replaced with the new
  `OpenApiSecuritySchemeReference` type; `SwaggerGenOptions.AddSecurityRequirement` now
  takes a `Func<OpenApiDocument, OpenApiSecurityRequirement>` instead of a direct
  `OpenApiSecurityRequirement` instance.

### Removed abandoned `BlazorInputFile` package (new regression under .NET 10)
- `BlazorInputFile` (v0.2.0, unmaintained since ~2019, predates Blazor's built-in
  `InputFile` component) ships a type (`ReadRequest`) with a struct layout that .NET 10's
  stricter runtime type-load validation rejects with a `TypeLoadException` ("incorrectly
  aligned or overlapped by a non-object field") the first time any code reflects over all
  loaded assemblies (see below). The only consumer, `BlazorShared.Models.CatalogItem
  .DataToBase64(IFileListEntry)`, was dead code (never called anywhere in the solution).
  Removed the package reference from `BlazorAdmin.csproj` / `BlazorShared.csproj`, the
  `@using BlazorInputFile` from `src/BlazorAdmin/_Imports.razor`, and the dead method +
  `using BlazorInputFile;` from `src/BlazorShared/Models/CatalogItem.cs`.

### Fixed `MinimalApi.Endpoint.AddEndpoints()` AppDomain-wide reflection scan (new regression under .NET 10)
- `MinimalApi.Endpoint` 1.3.0's `AddEndpoints()` extension hardcodes
  `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())` — a scan over
  *every* assembly loaded in the process, with no override to scope it. Once
  `BlazorInputFile` was removed, the *same* class of failure resurfaced for
  `Microsoft.JSInterop.WebAssembly` (a genuine, actively-maintained Microsoft assembly
  that is only valid to load inside a browser/mono-wasm runtime). It gets pulled into the
  `PublicApiIntegrationTests` process because that test project references both
  `PublicApi.csproj` and `Web.csproj`, and `Web` references `BlazorAdmin`, a Blazor
  WebAssembly project. Under .NET 8 the CLR's looser type-load validation tolerated
  reflecting over this assembly; .NET 10 does not.
  Fix: replaced `builder.Services.AddEndpoints();` in `src/PublicApi/Program.cs` with a
  local `AddPublicApiEndpoints(builder.Services)` helper that scopes the `IEndpoint`
  implementation scan to `typeof(Program).Assembly` only (all 7 `IEndpoint` implementations
  already live in `PublicApi`, confirmed via repo-wide search) — avoiding the AppDomain-wide
  scan entirely while keeping the existing `MinimalApi.Endpoint` package (still used for its
  `IEndpoint` interface, `MapEndpoints()`, and `AddConfigurationFile()` extensions).

### CS0433 ambiguous `Program` type (new regression under .NET 10)
- Under .NET 10, the SDK-generated implicit top-level-statement `Program` class became
  **public** (confirmed empirically via reflection: `internal` under net8.0 SDK,
  `public` under net10.0 SDK — a tooling/compiler default change, not something toggled
  in source). This collided with `PublicApi`'s own explicit `public partial class Program
  { }` marker when both assemblies are referenced by
  `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
  (`WebApplicationFactory<Program>` became ambiguous — CS0433).
  Fix: aliased the `Web.csproj` `ProjectReference` in
  `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` with
  `<Aliases>WebAssembly</Aliases>` (dropping it from the default "global" alias so its
  `Program` type no longer participates in unqualified name resolution), and updated the
  one file that genuinely needs `Web`'s types
  (`CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`) to declare
  `extern alias WebAssembly;` and `using WebAssembly::Microsoft.eShopWeb.Web.ViewModels;`.
  `ProgramTest.cs`'s bare `WebApplicationFactory<Program>` now resolves unambiguously to
  `PublicApi`'s `Program`.

### Warnings introduced by the version bumps — fixed
- `MSTEST0044` (`DataTestMethod` obsolete, new MSTest 4.5.1 analyzer): replaced
  `[DataTestMethod]` with `[TestMethod]` in `CatalogItemListPagedEndpoint.cs`
  (`[DataRow]` already works with plain `[TestMethod]` in current MSTest).
- `xUnit2013` (`Assert.Equal(1, x.Count)` / `Assert.Equal(0, x.Count)`, surfaced by the
  xunit 2.7.0 → 2.9.3 bump pulling in a newer `xunit.analyzers`): replaced with
  `Assert.Single(...)` / `Assert.Empty(...)` in
  `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`,
  `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`, and
  `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`.

## Validation

- `dotnet build eShopOnWeb.sln -c Release` → **Build succeeded, 0 errors, 0 warnings**
  (other than the two documented/accepted NU1901/NU1903 advisories below).
- `dotnet test eShopOnWeb.sln -c Release` → **74/74 tests passed**
  (UnitTests 44, IntegrationTests 3, FunctionalTests 12, PublicApiIntegrationTests 15).
- `dotnet list package --vulnerable --include-transitive` → only two residual advisories,
  both without any fix available on the free-licensed version line:
  - `AutoMapper` 14.0.0 — GHSA-rvv3-g6hj-g44x (High, DoS via unbounded recursion). All
    patched versions (`15.1.1+`, excluding the `16.0.0–16.1.0` re-introduction) require
    AutoMapper's commercial license tier.
  - `NuGet.Packaging` / `NuGet.Protocol` 6.12.1 — GHSA-g4vj-cjjj-v7hg (Low, "Defense in
    Depth"), transitive via the dev-time-only `Microsoft.VisualStudio.Web
    .CodeGeneration.Design` 10.0.2 scaffolding tool (not shipped in production); advisory
    lists `patched_versions: None` for every affected range.

## Tooling note

Several AppModUpgrade MCP tools (`generate_dotnet_upgrade_assessment`, `start_task`,
`break_down_task`, `validate_dotnet_sdk_in_globaljson`) failed throughout this session with
"Orchestrator channel was not available within 5000 ms" — an environment-level
connectivity issue, confirmed non-transient across multiple retries/backoffs and input
modes. `assessment.md`, `plan.md`, and `tasks.md` were therefore authored manually, and
execution was carried out directly via file edits + `dotnet` CLI rather than through the
tool-driven task tracker.
