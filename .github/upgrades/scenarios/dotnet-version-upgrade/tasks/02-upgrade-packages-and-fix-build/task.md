# Task 02 — Upgrade Packages and Fix Build

## Objective
Upgrade all NuGet package versions in `Directory.Packages.props` to net10-compatible
versions and fix all resulting build errors/warnings so the solution builds cleanly
on net10.0.

## Research Notes

### Package version strategy
- ASP.NET Core / Blazor / EF Core families pinned to `10.0.12` (latest GA patch as of
  upgrade date) via existing `$(AspNetVersion)` / `$(EntityFramworkCoreVersion)` /
  `$(SystemExtensionVersion)` MSBuild properties already used for centralized version
  bumps in `Directory.Packages.props`.
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` bumped via `$(VSCodeGeneratorVersion)`
  to `10.0.2`.
- Verified every target version exists and supports `net10.0` via direct NuGet v3
  flat-container API queries (ground truth, since `get_supported_package_version`
  batch-call ordering was unreliable in this session).

### Special-handling packages
- **BlazorInputFile** (0.2.0) — abandoned package, no net10 asset. Confirmed dead code
  (no `.razor` file uses `InputFile`/`IFileListEntry`). Removed entirely from
  `BlazorAdmin.csproj`, `BlazorShared.csproj`, `_Imports.razor`, and
  `BlazorShared/Models/CatalogItem.cs` (dead `DataToBase64` method + unused using).
- **Microsoft.AspNetCore.Mvc** (2.2.0) — orphaned `PackageVersion` entry, unreferenced
  by any project. Removed.
- **MediatR** — capped at **12.4.1** (last MIT-licensed release; v13+ is commercially
  licensed under LuckyPennySoftware). Deliberate decision to avoid introducing a
  licensing change as an upgrade side effect.
- **AutoMapper** — bumped to **16.2.0** (latest). AutoMapper 15.0.0+ changed to a
  non-MIT license, which conflicts with the "avoid licensing changes" principle used
  for MediatR. However, `GHSA-rvv3-g6hj-g44x` (stack-overflow DoS via unbounded
  recursive mapping) affects all versions `< 15.1.1` — i.e. every MIT-licensed release
  is vulnerable. Since `securityComplianceCheck=true` is an explicit success
  criterion, security was prioritized over the licensing preference. This is a
  deliberate deviation from the MediatR decision and is flagged in the final report.
  - `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 hard-pins AutoMapper
    to `[12.0.1]` and is obsolete as of AutoMapper 14+ (DI registration now built into
    the core `AutoMapper` package). Replaced with a direct `AutoMapper` package
    reference in `PublicApi.csproj` and `Web.csproj`.
- **NU1510 cleanup**: `System.Net.Http.Json`, `System.Security.Claims`,
  `System.Text.Json` are framework-provided in net10 — removed explicit
  `PackageReference`s from `BlazorAdmin.csproj` and `ApplicationCore.csproj`.
- **NU1901 (NuGet.Packaging/NuGet.Protocol 6.12.1, GHSA-g4vj-cjjj-v7hg, low severity)**:
  transitive dependency of `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2.
  Vulnerable range `>= 6.12.0, <= 6.12.4`, patched at `6.12.5`. Fixed by adding explicit
  `PackageVersion` entries for `NuGet.Packaging`/`NuGet.Protocol` at `6.12.5` plus
  direct `PackageReference`s in `Web.csproj`/`PublicApi.csproj` (the only two projects
  using the scaffolding tool) to pin the transitive version via Central Package
  Management. Initially attempted via `GlobalPackageReference`, but that applies to
  *every* project in the solution including the non-SDK `docker-compose.dcproj`
  (Docker Compose tooling project), which does not support/understand modern
  `PackageReference`/TFM resolution and failed restore with NU1105. Switched to
  per-project `PackageReference` instead.

### Build-breaking API changes fixed
- **Swashbuckle.AspNetCore 6.5.0 → 10.2.3 / Microsoft.OpenApi 1.x → 2.7.5**: namespace
  moved from `Microsoft.OpenApi.Models` to `Microsoft.OpenApi`; `ISchemaFilter.Apply`
  now takes `IOpenApiSchema` (interface) instead of `OpenApiSchema` (class); security
  scheme references now use `OpenApiSecuritySchemeReference` (constructed from the
  `OpenApiDocument` supplied via the new `Func<OpenApiDocument, OpenApiSecurityRequirement>`
  overload of `AddSecurityRequirement`) instead of the old
  `OpenApiSecurityScheme { Reference = new OpenApiReference {...} }` pattern.
- **AutoMapper 12.0.1 → 16.2.0**: `AddAutoMapper(Assembly)` convenience overload removed;
  all overloads now require an explicit `Action<IMapperConfigurationExpression>` config
  delegate as the first argument, e.g. `AddAutoMapper(cfg => { }, assembly)`.
- **Implicit top-level `Program` class visibility change (net8 → net10)**: confirmed via
  isolated repro that the compiler-generated `Program` class from top-level statements
  is `internal` by default on the net8 SDK but **`public`** by default on the net10 SDK.
  This caused a `CS0433` ambiguous-type error in `PublicApiIntegrationTests` because both
  `PublicApi.dll` (which explicitly declares `public partial class Program { }`) and
  `Web.dll` (which relies on the implicit declaration) now expose a public `Program`
  type in the global namespace, and both assemblies were referenced by the test project.
  Resolved by eliminating the need for the `Web.csproj` reference altogether (see next
  bullet) rather than using `extern alias` (tried first, worked, but was superseded by a
  cleaner fix).
- **PublicApiIntegrationTests test-model mismatch + Blazor WASM interop crash**:
  `CatalogItemListPagedEndpoint.ReturnsFirst10CatalogItems` deserialized the
  `/api/catalog-items` response (actually `PublicApi`'s own
  `ListPagedCatalogItemResponse`/`CatalogItemDto` shape) into `Web`'s MVC
  `CatalogIndexViewModel` — an unrelated view model that only coincidentally shares a
  `CatalogItems` property name. This dependency pulled `Web.csproj` → `BlazorAdmin.csproj`
  → `Microsoft.AspNetCore.Components.WebAssembly` → `Microsoft.JSInterop.WebAssembly`
  (10.0.12) into the test host process. ASP.NET Core's `ApplicationPartManager`
  (`AddControllers()` in `PublicApi/Program.cs`) and `MinimalApi.Endpoint`'s
  `AddEndpoints()` (which does an unguarded
  `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())`) both force
  full reflection over every loaded assembly, including `Microsoft.JSInterop.WebAssembly`.
  On net10 this assembly's `JSCallInfo` struct fails CLR type-loading under the desktop
  (non-browser-wasm) runtime with
  `TypeLoadException: ... object field at offset 4 that is incorrectly aligned or
  overlapped by a non-object field` — a runtime-level incompatibility of a
  browser-targeted assembly being loaded into a non-browser host, newly surfaced by a
  stricter net10 type-loader. (Confirmed via `git stash` that all 15
  `PublicApiIntegrationTests` passed unmodified on net8.0 — this is a genuine upgrade
  regression, not a pre-existing issue.) Fixed at the root cause: switched the test to
  deserialize into `PublicApi`'s own `ListPagedCatalogItemResponse` (the actually
  correct response shape for that endpoint) and removed the now-unnecessary
  `ProjectReference` to `Web.csproj` from `PublicApiIntegrationTests.csproj` entirely,
  eliminating the BlazorAdmin/JSInterop.WebAssembly dependency chain from the test host
  process rather than working around the symptom.
- **MSTest `DataTestMethod` obsolete (MSTEST0044)**: replaced with `[TestMethod]`
  (MSTest 3.x consolidated `DataTestMethod` into `TestMethod`).
- **xUnit2013** (3 pre-existing warnings in touched test projects, not new from the
  upgrade but fixed per "no warnings in touched projects" rule): replaced
  `Assert.Equal(1, x.Count)` / `Assert.Equal(0, x.Count)` with `Assert.Single(x)` /
  `Assert.Empty(x)`.
- **SYSLIB0051** (pre-existing warning in `ApplicationCore`, fixed per the same rule):
  removed the obsolete `SerializationInfo`/`StreamingContext` legacy-serialization
  constructor from `EmptyBasketOnCheckoutException` (formatter-based exception
  serialization is obsolete and unused by this codebase).

## Done When
- [x] `dotnet restore eShopOnWeb.sln` completes with zero `NU1xxx` warnings.
- [x] `dotnet build eShopOnWeb.sln -c Release` (clean rebuild) completes with
      0 errors, 0 warnings.
- [x] All special-handling package decisions documented here and in
      `scenario-instructions.md`.
