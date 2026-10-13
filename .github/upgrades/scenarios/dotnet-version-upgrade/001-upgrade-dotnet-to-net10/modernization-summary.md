# Modernization Summary — 001-upgrade-dotnet-to-net10

## Task
Upgrade eShopOnWeb (ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web, and all
test projects) from .NET 8.0 to .NET 10.0.

**Success criteria:** passBuild=true, generateNewUnitTests=false, passUnitTests=true, securityComplianceCheck=true
**Flow mode:** Automatic (fully autonomous, no pauses)
**Branch:** `upgrade/net10`

## Result: ✅ SUCCESS — all criteria met

- `dotnet build eShopOnWeb.sln -c Release` → **0 errors, 0 warnings** other than one accepted/documented
  low-severity transitive NuGet advisory (see Security section).
- All 4 test projects pass: **74/74 tests green** (UnitTests 44, IntegrationTests 3, FunctionalTests 12,
  PublicApiIntegrationTests 15). No new tests were written, per `generateNewUnitTests=false` — existing
  tests were only adjusted where the upgrade itself required a source change (analyzer-recommended
  assertion rewrites, obsolete API removal).
- `dotnet list package --vulnerable --include-transitive` → only the one known, no-fix-available, low
  severity advisory remains (documented below as an accepted residual risk).

## What changed

### Toolchain / central version management
- `global.json`: SDK version `8.0.x` → `10.0.x` (kept `rollForward: latestFeature`).
- `Directory.Packages.props`: central `<TargetFramework>` bumped `net8.0` → `net10.0` (single edit
  retargets all 10 projects, since the repo uses NuGet Central Package Management). All package versions
  bumped to their latest net10-compatible releases (ASP.NET Core/EF Core 10.0.12, MediatR 14.2.0,
  Swashbuckle.AspNetCore 10.3.0, Ardalis.* majors, MSTest 3→4, xunit, etc.).
- Removed a stale, unreferenced `Microsoft.AspNetCore.Mvc` 2.2.0 `PackageVersion` entry (dead weight, not
  used by any project).
- Removed `System.Net.Http.Json`, `System.Security.Claims`, `System.Text.Json` explicit package
  references (BlazorAdmin / ApplicationCore) — the .NET 10 SDK now prunes these automatically since
  they ship in the shared framework; keeping them produced `NU1510` warnings.

### Security fix — AutoMapper
`AutoMapper.Extensions.Microsoft.DependencyInjection` is frozen at v12.0.1, which carries a known
**high-severity** vulnerability (GHSA-rvv3-g6hj-g44x). AutoMapper v13+ merged the `AddAutoMapper(...)`
DI extension methods directly into the core `AutoMapper` package, making the separate extension package
obsolete. Fixed by referencing `AutoMapper` 16.2.0 directly in `PublicApi.csproj` (and dropping the now
orphaned reference from `Web.csproj`, which didn't actually use it).

### Breaking API changes fixed

1. **Swashbuckle.AspNetCore 6→10 / Microsoft.OpenApi 1→2** (`src/PublicApi/Program.cs`,
   `src/PublicApi/CustomSchemaFilters.cs`): the `Microsoft.OpenApi.Models` namespace was removed; models
   now live directly under `Microsoft.OpenApi` and are interface-based (`OpenApiSchema` → `IOpenApiSchema`).
   Security-scheme references changed from a `Reference`-property pattern to a dedicated
   `OpenApiSecuritySchemeReference` reference type, and `AddSecurityRequirement` now takes a
   `Func<OpenApiDocument, OpenApiSecurityRequirement>` instead of a plain value. Updated both files to the
   new API shape.
2. **AutoMapper `AddAutoMapper` overload change**: the single-`Assembly`-array overload used in
   `PublicApi/Program.cs` no longer resolves; updated the call to pass an explicit (no-op)
   `Action<IMapperConfigurationExpression>` configuration delegate alongside the assembly, matching the
   new API surface.
3. **Implicit top-level-statement `Program` class visibility changed from `internal` to `public`**
   (confirmed via a side-by-side reflection check against the pre-upgrade net8 build) — this is a
   .NET 10 toolchain/C# compiler behavior change. Because `tests/PublicApiIntegrationTests` references
   both `PublicApi.csproj` (which already declared an explicit `public partial class Program`) and
   `Web.csproj`, Web's now-public implicit `Program` collided with PublicApi's, producing `CS0433`
   (ambiguous `Program`). Fixed by aliasing the `Web.csproj` project reference
   (`Aliases="WebApp"`) and using `extern alias WebApp;` + `using WebApp::...` in the one file
   (`CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`) that genuinely needs a type from
   `Microsoft.eShopWeb.Web.ViewModels`.
4. **`BlazorInputFile` (abandoned, unmaintained package, last published years ago, no newer version
   exists) failed to load under .NET 10's stricter CoreCLR type-layout validation**
   (`ReflectionTypeLoadException` — "contains an object field … incorrectly aligned"), breaking
   `MinimalApi.Endpoint`'s `AddEndpoints()` reflection scan (which enumerates
   `AppDomain.CurrentDomain.GetAssemblies()`). The package's only usage in the codebase
   (`CatalogItem.DataToBase64(IFileListEntry)`) was confirmed dead code — no Razor component in
   `BlazorAdmin` ever calls it. Removed the package entirely and ported the method to the built-in
   `Microsoft.AspNetCore.Components.Forms.IBrowserFile` API (functionally equivalent, no behavior change
   since the method isn't currently invoked anywhere).
5. **`Microsoft.JSInterop.WebAssembly` (Blazor WebAssembly/Mono runtime assembly) hit the same stricter
   .NET 10 type-layout validation** when loaded into the `PublicApiIntegrationTests` process. Root cause:
   that test project references `Web.csproj` solely to deserialize one MVC view-model type
   (`CatalogIndexViewModel`), but `Web.csproj` transitively references `BlazorAdmin.csproj` (a Blazor
   WebAssembly app), pulling Mono/WASM-targeted runtime assemblies into the CoreCLR test process, where
   `MinimalApi.Endpoint`'s full-AppDomain assembly scan trips over them. Verified against a pre-upgrade
   net8 baseline (via a git worktree) that this combination worked before only because the older CoreCLR
   did not enforce the stricter layout check — not because the architecture was sound. Fixed by adding an
   MSBuild target to `PublicApiIntegrationTests.csproj` that strips the Blazor WebAssembly/Mono runtime
   assemblies (`Microsoft.JSInterop.WebAssembly`, `Microsoft.AspNetCore.Components.WebAssembly*`,
   `BlazorAdmin`, `Blazored.LocalStorage`, `Mono.TextTemplating`) from `ReferenceCopyLocalPaths` for this
   test project only — they are never needed since nothing in this test process executes Blazor/BlazorAdmin
   code, only PublicApi's own minimal API host.

### Obsolete-API / warning cleanup (full build+warning scan performed; all resolved)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: removed the
  `SerializationInfo`/`StreamingContext` legacy binary-serialization constructor
  (`SYSLIB0051`-obsolete, unused anywhere in the codebase).
- `tests/UnitTests/.../BasketRemoveEmptyItems.cs`,
  `tests/UnitTests/.../CustomerOrdersWithItemsSpecification.cs`,
  `tests/IntegrationTests/.../SetQuantities.cs`: replaced `Assert.Equal(0/1, collection.Count)` with
  `Assert.Empty`/`Assert.Single` per `xUnit2013` analyzer guidance — same assertions, clearer intent, no
  behavior change.
- `tests/PublicApiIntegrationTests/.../CatalogItemListPagedEndpoint.cs`: replaced the now-obsolete
  `[DataTestMethod]` MSTest attribute with `[TestMethod]` (MSTest unified the two attributes; `MSTEST0044`).

## Security compliance (residual, accepted risk)

One low-severity transitive NuGet advisory remains and has **no available fix**:

| Package | Version | Severity | Advisory | Source |
|---|---|---|---|---|
| `NuGet.Packaging` | 6.12.1 | Low | GHSA-g4vj-cjjj-v7hg | `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (latest stable; next is 11.0.0-preview) |
| `NuGet.Protocol` | 6.12.1 | Low | GHSA-g4vj-cjjj-v7hg | same |

`Microsoft.VisualStudio.Web.CodeGeneration.Design` is a **design-time-only scaffolding tool** (used by
`dotnet aspnet-codegenerator`), never shipped in the published/runtime output of `Web`. Confirmed 10.0.2 is
the latest stable release on NuGet; no newer stable version resolves this. Accepted as a residual risk —
revisit when a newer stable release of the scaffolding tool is published.

## Tooling notes / environment caveats encountered

- The MCP `generate_dotnet_upgrade_assessment` tool repeatedly failed in this environment
  ("Orchestrator channel was not available within 5000 ms") across many retries — the assessment was
  built manually instead (package-by-package via `get_supported_package_version` plus direct repo
  inspection), and is recorded in `assessment.md`.
- The MCP task-tracking tools (`start_task` / `break_down_task`) could not locate tasks from the generated
  `plan.md` in this environment. Execution proceeded directly against the plan (tracked via an internal
  todo list) per the available `execute-upgrade-task` skill's guidance, rather than leaving the upgrade
  blocked.

## Verification performed
- `dotnet build eShopOnWeb.sln -c Release` (clean, from scratch): 0 errors, 0 warnings (besides the
  accepted NU1901 advisory above).
- `dotnet test` on all 4 test projects individually: 74/74 passed, 0 failed, 0 skipped.
- `dotnet list package --vulnerable --include-transitive`: only the one documented, no-fix-available, low
  severity advisory remains.
