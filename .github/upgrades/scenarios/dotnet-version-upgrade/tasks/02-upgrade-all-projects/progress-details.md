# Task 02 — Progress Details

## Status: ✅ Complete

## What actually changed (final, verified state)

### Central Package Management / target framework
- `Directory.Packages.props`:
  - `TargetFramework` → `net10.0`.
  - `AspNetVersion` / `SystemExtensionVersion` / `EntityFramworkCoreVersion` → `10.0.12`.
  - `VSCodeGeneratorVersion` → `10.0.2`.
  - Added `<CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>`
    so transitive packages can be security-pinned explicitly (used for NuGet.Packaging/Protocol,
    see below).
  - All ~45 `PackageVersion` entries bumped to the latest NuGet-verified net10.0-compatible
    stable release (verified directly against `api.nuget.org`, not the degraded
    `get_supported_package_version` tool — see task.md for the list of tool-reported
    fabricated versions that were caught and corrected).
  - Removed obsolete/orphan entries: `BlazorInputFile`, `System.Text.Json`,
    `System.Net.Http.Json`, `System.Security.Claims`, orphan `Microsoft.AspNetCore.Mvc` 2.2.0.
  - Migrated `AutoMapper.Extensions.Microsoft.DependencyInjection` (orphaned at 12.0.1, which
    pulled a HIGH severity DoS CVE, GHSA-rvv3-g6hj-g44x) → core `AutoMapper` package `16.2.0`
    (patched; DI registration folded into core package since v13).
  - Added explicit transitive pins `NuGet.Packaging`/`NuGet.Protocol` → `6.12.5` (patches
    low-severity advisory GHSA-g4vj-cjjj-v7hg pulled in by
    `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2, the latest available version of
    that design-time-only scaffolding package).
  - `MediatR` bumped only within the free/Apache-2.0 12.x line, to `12.5.0` — the real latest
    (`14.2.0`) requires the same kind of commercial license as AutoMapper v13+, and there was no
    security reason to force that decision onto the user; flagged for manual review instead.

### Project files
- `src/PublicApi/PublicApi.csproj`: `AutoMapper.Extensions.Microsoft.DependencyInjection` →
  `AutoMapper`.
- `src/Web/Web.csproj`: removed unused `AutoMapper.Extensions.Microsoft.DependencyInjection`
  reference (no AutoMapper usage exists in this project) — this had been silently unused.
- `src/BlazorAdmin/BlazorAdmin.csproj`, `src/BlazorShared/BlazorShared.csproj`: removed
  `BlazorInputFile` (abandoned package, no net10 build; the single real usage was dead code).
- `src/ApplicationCore/ApplicationCore.csproj`: removed `System.Text.Json` and
  `System.Security.Claims` (both BCL-shipped on net10, NuGet flagged as prunable).
- `tests/UnitTests/UnitTests.csproj`: removed `xunit.runner.console` (obsolete tooling).
- `tests/FunctionalTests/FunctionalTests.csproj`: removed legacy `DotNetCliToolReference`.
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: added a post-
  `ResolveReferences` MSBuild target to stop `Microsoft.JSInterop.WebAssembly.dll` /
  `Microsoft.AspNetCore.Components.WebAssembly.dll` from being copied to the test output (see
  "Runtime/test regression" below for why).

### Code fixes (API/behavioral compatibility)
- `src/PublicApi/CustomSchemaFilters.cs`: Swashbuckle 6.5→10.3 moved the OpenAPI object model
  from `Microsoft.OpenApi.Models` to `Microsoft.OpenApi` and changed `ISchemaFilter.Apply`'s
  parameter from the concrete `OpenApiSchema` to the new `IOpenApiSchema` interface, whose
  `Properties` dictionary is nullable — updated signature/using and added a null guard.
- `src/PublicApi/Program.cs`:
  - Same `Microsoft.OpenApi.Models` → `Microsoft.OpenApi` namespace move for `OpenApiInfo`,
    `OpenApiSecurityScheme`, `OpenApiSecurityRequirement`.
  - `AddAutoMapper(Assembly)` no longer exists on the core `AutoMapper` package — all overloads
    now require an `Action<IMapperConfigurationExpression>` first argument — changed to
    `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)`.
  - `OpenApiSecurityRequirement` is now a dictionary keyed by `OpenApiSecuritySchemeReference`
    (not `OpenApiSecurityScheme` + a `.Reference` property, which no longer exists) — replaced
    with `new OpenApiSecuritySchemeReference("Bearer")`.
  - `AddSecurityRequirement` now takes a `Func<OpenApiDocument, OpenApiSecurityRequirement>`
    factory instead of a plain value — wrapped in a lambda.
- `src/Web/Program.cs`: added an explicit `internal partial class Program { }` declaration.
  Newer SDKs changed the default accessibility of the compiler-generated top-level-statements
  `Program` class from `internal` to `public`; since `PublicApi/Program.cs` already explicitly
  opts into `public partial class Program { }` for its own test factory, having *both*
  `Web`'s and `PublicApi`'s `Program` classes become `public` caused `CS0433` ("Program exists
  in both Web and PublicApi") in `PublicApiIntegrationTests`, which references both assemblies.
  Restoring Web's original (net8) internal visibility resolves the ambiguity without touching
  the test project.
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: removed the obsolete
  binary-serialization constructor (`SerializationInfo`/`StreamingContext`) — flagged
  `SYSLIB0051`; this legacy ISerializable pattern is unsupported/unnecessary on modern .NET and
  was never called anywhere in the codebase.

### Test fixes (warning-driven, not behavioral)
- `tests/UnitTests/.../CustomerOrdersWithItemsSpecification.cs`,
  `tests/UnitTests/.../BasketRemoveEmptyItems.cs`,
  `tests/IntegrationTests/.../SetQuantities.cs`: replaced `Assert.Equal(1, x.Count)` /
  `Assert.Equal(0, x.Count)` with `Assert.Single(x)` / `Assert.Empty(x)` per xUnit2013
  analyzer guidance (purely cosmetic, same assertions).
- `tests/PublicApiIntegrationTests/.../CatalogItemListPagedEndpoint.cs`: `[DataTestMethod]` →
  `[TestMethod]` (MSTest now supports `[DataRow]` directly on `[TestMethod]`;
  `[DataTestMethod]` is obsolete, MSTEST0044).

### Docker
- `src/Web/Dockerfile`, `src/PublicApi/Dockerfile`: base images `8.0` → `10.0`.

### Runtime/test regression found and fixed: `Microsoft.JSInterop.WebAssembly` TypeLoadException
On net10.0, `PublicApiIntegrationTests` (which project-references both `PublicApi.csproj` for
the API host under test and `Web.csproj` purely for its `Web.ViewModels` DTOs) started failing
**all 15 tests** with:
```
System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
Could not load type 'WebAssembly.JSInterop.JSCallInfo' from assembly
'Microsoft.JSInterop.WebAssembly' ... because it contains an object field at offset 4 that is
incorrectly aligned or overlapped by a non-object field.
```
Root cause: `MinimalApi.Endpoint`'s `AddEndpoints()` (called in `PublicApi/Program.cs`) scans
**every assembly currently loaded in the AppDomain** for endpoint types. Because the test
project references `Web.csproj`, MSBuild copies Web's full transitive dependency set —
including Blazor WebAssembly-only assemblies like `Microsoft.JSInterop.WebAssembly.dll` — into
the test output. On net10's runtime, loading that assembly's types outside an actual Blazor
WASM host throws (apparently due to a struct-layout incompatibility in that package that a
stricter net10 type loader now surfaces; it is NOT one of our own types). Confirmed via a
pre-upgrade control run (`git stash` back to net8) that this passed 15/15 before — a genuine
regression from the version bump, not a pre-existing failure.

Fix: added a `RemoveBlazorWebAssemblyAssets` MSBuild target to
`PublicApiIntegrationTests.csproj` that strips `Microsoft.JSInterop.WebAssembly.dll` and
`Microsoft.AspNetCore.Components.WebAssembly.dll` out of `@(ReferenceCopyLocalPaths)` right
after `ResolveReferences`. Neither assembly is needed by these API-only tests (they only need
Web's plain `ViewModels` POCOs), so excluding them from the test's own output keeps them out of
the AppDomain entirely and the `AddEndpoints()` scan no longer touches them. Verified: all 15
`PublicApiIntegrationTests` pass after the fix.

## Build/test status at completion of this task
- `dotnet restore eShopOnWeb.sln` — success, 0 warnings.
- `dotnet build eShopOnWeb.sln -c Release` — **0 errors, 0 warnings** (all 10 projects).
- `dotnet test eShopOnWeb.sln -c Release` — **44 + 3 + 12 + 15 = 74 passed, 0 failed, 0 skipped**
  (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests).

## Flagged for user/legal follow-up (not blocking, cannot be resolved autonomously)
1. **AutoMapper commercial licensing**: core `AutoMapper` package (now at 16.2.0, replacing the
   retired DI-extensions companion package) introduced a paid license tier for qualifying
   commercial use starting at v13 (see automapper.io). This upgrade intentionally prioritized
   closing the HIGH severity DoS CVE in 12.0.1 over avoiding the license change, per the
   explicit `securityComplianceCheck=true` success criterion — but the organization should
   confirm licensing compliance before shipping.
2. **MediatR**: deliberately left on the last free/Apache-2.0 release (12.5.0) rather than the
   real latest (14.2.0), to avoid opting the organization into MediatR's commercial license
   (mediatr.io) without an explicit decision. No CVE forces this jump, so it was not treated as
   a security requirement. Revisit when/if the user confirms they hold or will acquire a
   MediatR/AutoMapper commercial license.
3. **NSubstitute major bump 5→6** (`UnitTests`): untested for behavioral breaking changes beyond
   "all 44 UnitTests pass" — recommend a closer look if NSubstitute usage patterns expand.
