# Execution Notes — eShopOnWeb .NET 8.0 → 10.0 Upgrade

> **Note on tooling**: The AppModUpgrade `start_task`/`break_down_task` task-tracking
> tools could not parse the tasks defined in this folder's `plan.md`/`tasks.md`
> (a parsing defect, unrelated to formatting). Execution was therefore carried out
> directly (file edits + `dotnet restore`/`build`/`test`), following the researched
> plan faithfully. This file documents what was actually done, in place of the
> `progress-details.md` that `complete_task` would normally have produced per task.

## Final status

- **Build**: `dotnet build eShopOnWeb.sln` → **0 errors, 0 warnings**, all 10 real
  projects (ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin,
  BlazorShared, UnitTests, IntegrationTests, FunctionalTests,
  PublicApiIntegrationTests) targeting `net10.0`.
- **Tests**: All 74 existing tests pass (44 UnitTests + 3 IntegrationTests +
  12 FunctionalTests + 15 PublicApiIntegrationTests). No new tests were generated
  (per requested success criteria `generateNewUnitTests=false`).
- **Security**: `dotnet list package --vulnerable --include-transitive` reports
  zero known vulnerable packages for all 10 real projects.
- **Legacy `docker-compose.dcproj`**: left completely untouched (still net8-era,
  no TargetFramework declared) — see explanation below.

## Key changes

1. **`global.json`**: SDK version `8.0.x` → `10.0.x`.
2. **`Directory.Packages.props`** (Central Package Management):
   - `TargetFramework` property **removed from this file** and instead declared
     explicitly per real project (see "docker-compose.dcproj trap" below for why).
   - All ~46 centrally-managed package versions bumped to their latest
     net10.0-compatible, non-vulnerable versions (e.g. `Azure.Identity` → 1.21.0,
     `System.Text.Json` → 10.0.12 (then removed entirely — see below),
     `FluentValidation` → 12.1.1, `MediatR` → 14.2.0, `Swashbuckle.AspNetCore` →
     10.x, `Microsoft.EntityFrameworkCore.*` → 10.x, etc.)
   - `System.Security.Claims`, `System.Net.Http.Json`, `System.Text.Json` package
     references removed from projects where the types are now part of the
     framework/shared runtime under net10.0 (no longer separate NuGet packages).
   - `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated at 12.0.1)
     replaced with the core `AutoMapper` package (16.2.0), which now bundles the
     DI extensions itself and patches a DoS vulnerability
     (GHSA-rvv3-g6hj-g44x).
   - Added a `GlobalPackageReference` (conditioned to exclude `.dcproj` projects)
     pinning `NuGet.Packaging`/`NuGet.Protocol` to 6.12.5 to remediate a
     transitive advisory (GHSA-g4vj-cjjj-v7hg) pulled in by
     `Microsoft.VisualStudio.Web.CodeGeneration.Design`.

3. **`docker-compose.dcproj` NU1105 trap** (the trickiest issue in this upgrade):
   This legacy "Container Orchestrator Support" project has zero real
   `PackageReference` items, so NuGet normally skips target-framework validation
   for it entirely. However, once `Directory.Packages.props` declares a
   `TargetFramework` (even conditioned) or any `GlobalPackageReference` that
   applies to it, NuGet starts validating its TFM — and its bundled
   `Microsoft.Docker.Sdk` tooling only recognizes TFMs up to ~net8.0, failing
   restore with `NU1105: Invalid target framework` for net9.0/net10.0.
   **Fix**: moved `TargetFramework` out of `Directory.Packages.props` into each
   of the 10 real `.csproj` files individually, and conditioned the
   `GlobalPackageReference` entries with
   `Condition="'$(MSBuildProjectExtension)' != '.dcproj'"` so nothing is ever
   injected into `docker-compose.dcproj`. The file itself was left completely
   unmodified.

4. **Swashbuckle.AspNetCore 10.x / Microsoft.OpenApi 2.x breaking changes**
   (`src/PublicApi/Program.cs`, `src/PublicApi/CustomSchemaFilters.cs`):
   - Namespace `Microsoft.OpenApi.Models` → `Microsoft.OpenApi`.
   - `ISchemaFilter.Apply(OpenApiSchema, ...)` → `Apply(IOpenApiSchema, ...)`
     (added a null-check for the now-nullable `IOpenApiSchema.Properties`).
   - `OpenApiReference` replaced by typed reference classes; rewrote the JWT
     `AddSecurityRequirement` registration to use
     `new OpenApiSecuritySchemeReference("Bearer", document)` inside a
     `Func<OpenApiDocument, OpenApiSecurityRequirement>` (Swashbuckle's
     `AddSecurityRequirement` extension signature changed accordingly).

5. **AutoMapper 16.x breaking change** (`src/PublicApi/Program.cs`): the
   assembly-only `AddAutoMapper(Assembly)` overload no longer exists; changed to
   `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)`.

6. **Obsolete API removal** (`src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`):
   removed the obsolete formatter-based serialization constructor
   (`SerializationInfo`/`StreamingContext`) to fix SYSLIB0051.

7. **Test style warnings**: replaced `Assert.Equal(1, x.Count)` /
   `Assert.Equal(0, x.Count)` with `Assert.Single(x)` / `Assert.Empty(x)`
   (xUnit2013) in `CustomerOrdersWithItemsSpecification.cs`,
   `BasketRemoveEmptyItems.cs`, `SetQuantities.cs`. Replaced an obsolete
   `[DataTestMethod]` with `[TestMethod]` (MSTEST0044) in
   `CatalogItemListPagedEndpoint.cs`.

8. **`CS0433` Program-type ambiguity** (`tests/PublicApiIntegrationTests`):
   `PublicApiIntegrationTests.csproj` references both `PublicApi.csproj` and
   `Web.csproj`. Both now expose a public top-level-statement `Program` class,
   which is ambiguous for `WebApplicationFactory<Program>`. Verified via
   reflection this is a genuine net10 regression (confirmed absent on the net8.0
   baseline via `git stash`). Fixed by giving the `Web` project reference an
   MSBuild alias (`Aliases="WebLib"`) and using `extern alias WebLib;` +
   `using WebLib::Microsoft.eShopWeb.Web.ViewModels;` in the one file
   (`CatalogItemListPagedEndpoint.cs`) that needed a `Web` view-model type, so
   the unqualified `Program` in `ProgramTest.cs` now resolves unambiguously to
   `PublicApi.Program`.

9. **Blazor WebAssembly assemblies crashing endpoint discovery at test-run time**
   (`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`): with the
   `Program` ambiguity fixed, `WebApplicationFactory<Program>` still failed to
   start under net10.0 with `ReflectionTypeLoadException` ("... object field ...
   incorrectly aligned") when `MinimalApi.Endpoint`'s `AddEndpoints()` enumerated
   all loaded assemblies in the process. The offending assemblies were
   `BlazorInputFile` (abandoned package, referenced by `BlazorShared` for a
   legacy file-upload model) and the `Microsoft.AspNetCore.Components.WebAssembly*`
   / `Microsoft.JSInterop.WebAssembly` assemblies pulled in transitively via
   `Web` → `BlazorAdmin` (a Blazor WebAssembly project). Under .NET 10's
   stricter runtime type-loader validation, reflecting over these WASM-oriented
   assemblies from a normal server-hosted process throws. None of them are
   actually needed by the API tests (only two `Web` ViewModel types are, handled
   via the alias above), so an MSBuild target
   (`RemoveBlazorAssembliesFromOutput`, running `AfterTargets="ResolveReferences"`)
   excludes `BlazorInputFile`, `BlazorAdmin`, `Blazored.LocalStorage`, and any
   `Microsoft.AspNetCore.Components.WebAssembly*`/`Microsoft.JSInterop.WebAssembly*`
   assembly from `@(ReferenceCopyLocalPaths)` for this test project only — they
   are never copied into its output directory, so they can't be loaded/reflected
   over during the test run.

## Deferred / follow-up recommendations (not addressed, out of scope for this upgrade)

- **`BlazorInputFile`** (v0.2.0, last published ~2019) is an abandoned package
  still referenced by `BlazorShared`/`BlazorAdmin` for file-upload support. It
  continues to *function* for BlazorAdmin's actual WASM runtime use case (the
  reflection failure only manifested when the assembly was loaded into a
  non-WASM host process for endpoint scanning, which the fix above avoids), but
  it should be replaced with the built-in `InputFile` component
  (`Microsoft.AspNetCore.Components.Forms`, official since .NET 5) at some
  point — this was not done here since it requires a Razor/UI-level refactor of
  `BlazorAdmin`'s upload screens, is outside strict upgrade scope, and there is
  no existing Blazor UI test coverage to validate a behavior-preserving rewrite
  against.
- Consider trimming the `PublicApiIntegrationTests` → `Web` project reference to
  only the two ViewModel types actually needed, e.g. by moving
  `CatalogIndexViewModel`/`CatalogItemViewModel` (or a slim DTO) into a shared
  project without the Blazor dependency chain, to avoid needing the
  `extern alias`/assembly-exclusion workarounds altogether.
