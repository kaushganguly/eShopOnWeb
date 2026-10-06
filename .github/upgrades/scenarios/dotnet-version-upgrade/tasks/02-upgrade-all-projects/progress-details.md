# Task 02 — Upgrade all projects to net10.0: Progress Details

## Summary
All 10 projects in the solution now target `net10.0` (via the single `TargetFramework` property in
`Directory.Packages.props`, which is the one place the whole solution's TFM is controlled — there is no
`Directory.Build.props` and no project sets `TargetFramework` individually). All NuGet package versions were
bumped to .NET-10-compatible releases, all known security vulnerabilities were resolved, all NU1510
(prunable reference) warnings were eliminated, and all resulting compilation/breaking-API issues were fixed.
Final result: **clean solution build — 0 errors, 0 warnings** (`dotnet build eShopOnWeb.sln -c Release`), from
a fully clean `bin`/`obj` state.

## Changes made

### 1. `Directory.Packages.props`
- `TargetFramework`: `net8.0` → `net10.0`.
- Version aliases bumped: `AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion` → `10.0.12`;
  `VSCodeGeneratorVersion` → `10.0.2`.
- ~20 individual `PackageVersion` entries bumped to latest .NET-10-supported versions (Ardalis.* family,
  MediatR, Swashbuckle.AspNetCore, FluentValidation, NSubstitute, Azure.Identity, System.Text.Json,
  Microsoft.VisualStudio.Azure.Containers.Tools.Targets, etc. — full matrix in `assessment.md`).
- **AutoMapper**: replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` (stuck at 12.0.1, carrying
  HIGH-severity CVE GHSA-rvv3-g6hj-g44x with no fix in the 12.x line) with plain `AutoMapper` at `16.2.0`
  (the DI extension method `AddAutoMapper` was folded into the core package starting at v13.0.0).
- **NU1510 cleanup**: removed now-redundant `PackageVersion` entries for `System.Net.Http.Json`,
  `System.Security.Claims`, `System.Text.Json` (all now ambient via the net10 shared framework) once their
  `PackageReference`s were removed from the consuming projects.
- **NU1901 fix**: added explicit `PackageVersion` overrides for `NuGet.Packaging` and `NuGet.Protocol` at
  `7.9.0` to resolve a low-severity transitive vulnerability (GHSA-g4vj-cjjj-v7hg) pulled in via
  `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (the latest available version of that scaffolding
  package, which itself still depends on a vulnerable 6.12.1 range of the NuGet client libraries).

### 2. `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`
- Swapped `PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection"` → `AutoMapper`.
- Added direct `PackageReference`s for `NuGet.Packaging` / `NuGet.Protocol` to force the patched version
  (7.9.0) to win over the vulnerable transitive version pulled in by
  `Microsoft.VisualStudio.Web.CodeGeneration.Design`.

### 3. `src/ApplicationCore/ApplicationCore.csproj`
- Removed redundant `PackageReference`s for `System.Security.Claims` and `System.Text.Json` (NU1510 — both
  ship in the net10 shared framework now).

### 4. `src/BlazorAdmin/BlazorAdmin.csproj`
- Removed redundant `PackageReference` for `System.Net.Http.Json` (NU1510, same reason).

### 5. `src/PublicApi/Program.cs`
- `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;` (Microsoft.OpenApi v2 namespace change).
- `AddSecurityRequirement(...)` rewritten from the old `OpenApiSecurityRequirement` dictionary-of-schemes
  pattern (with a `.Reference = new OpenApiReference {...}` assignment) to the new
  `AddSecurityRequirement(document => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] })`
  pattern required by Swashbuckle.AspNetCore v10 / Microsoft.OpenApi v2 (per the project's official
  [migration guide](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/docs/migrating-to-v10.md)).
- `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)`
  — AutoMapper 13+'s merged-in `AddAutoMapper` overloads all require an `Action<IMapperConfigurationExpression>`
  as the first parameter; there is no longer a bare `Assembly`-only overload.

### 6. `src/PublicApi/CustomSchemaFilters.cs`
- `ISchemaFilter.Apply(OpenApiSchema schema, ...)` → `Apply(IOpenApiSchema schema, ...)` (interface signature
  changed in Swashbuckle.AspNetCore v10 to accept the new `IOpenApiSchema` interface).
- Added a pattern-match/cast to the concrete `OpenApiSchema` type before mutating `.Properties`, per the
  Swashbuckle v10 migration guide ("properties are only mutable on the concrete type").
- Added a null guard on `.Properties` to avoid a possible-null-reference warning.

### 7. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` + `ProgramTest.cs`
- **New ambiguity surfaced only after the net10 upgrade**: the implicit top-level-statements `Program` class
  in `src/Web/Program.cs` is now `public` (confirmed via reflection: `IsPublic == true`), whereas previously
  only `src/PublicApi/Program.cs` exposed an explicit `public partial class Program { }`. Because this test
  project has `ProjectReference`s to both `Web.csproj` and `PublicApi.csproj`, the unqualified `Program` type
  used by `WebApplicationFactory<Program>` became ambiguous (`CS0433`).
  - Fix: added `<Aliases>global,PublicApiAssembly</Aliases>` to the `PublicApi` `ProjectReference` (keeping
    the assembly visible under the default `global` alias for all other files, while also exposing it under
    an explicit `PublicApiAssembly` extern alias).
  - In `ProgramTest.cs`: added `extern alias PublicApiAssembly;` and
    `using ProgramEntryPoint = PublicApiAssembly::Program;`, then changed
    `WebApplicationFactory<Program>` → `WebApplicationFactory<ProgramEntryPoint>` to unambiguously target the
    `PublicApi` entry point (which is what this test project's integration tests actually exercise).
  - No other file in this project referenced `Program` directly (`ProgramTest.NewClient` is used everywhere
    else), so this was the only change needed.

### 8. Pre-existing warnings fixed (per "fix all warnings in touched projects" rule)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: removed the obsolete
  formatter-based-serialization constructor (`SerializationInfo`/`StreamingContext`) that triggered
  `SYSLIB0051`. This constructor is not needed — target-framework BinaryFormatter-based exception
  serialization is obsolete/unsupported in modern .NET and this constructor was never exercised.
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` (×2),
  `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`,
  `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: replaced
  `Assert.Equal(1, collection.Count)` / `Assert.Equal(0, collection.Count)` with `Assert.Single(collection)` /
  `Assert.Empty(collection)` to fix `xUnit2013` analyzer warnings — behaviorally identical assertions.
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`: replaced the
  obsolete `[DataTestMethod]` attribute with `[TestMethod]` (MSTest now treats `DataTestMethod` as an alias
  of `TestMethod` and flags it via `MSTEST0044`) — no behavior change, `[DataRow]` attributes work identically
  with `[TestMethod]`.

## Verification
- `dotnet restore eShopOnWeb.sln` from a clean NuGet/bin/obj state: **no NU19xx vulnerability warnings, no
  NU1510 warnings**.
- `dotnet build eShopOnWeb.sln -c Release` from a clean `bin`/`obj` state: **Build succeeded. 0 Warning(s).
  0 Error(s).**

## Open item flagged for final summary
`AutoMapper` 13.0.0+ (we are now on 16.2.0) requires a commercial license for organizations above a certain
size/revenue threshold (AutoMapper's dual-licensing model, same as MediatR). This is a necessary trade-off to
resolve the HIGH-severity CVE in the AutoMapper 12.x line (no fix exists below 15.1.1), and `securityComplianceCheck=true`
is an explicit, non-negotiable success criterion for this upgrade — but it is a business-relevant licensing
change that must be explicitly flagged to the user/organization, since it wasn't a factor before this upgrade.
