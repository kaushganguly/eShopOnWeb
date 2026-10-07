# Task 02 — Progress Details

## Status: ✅ Complete

## Summary
Upgraded all NuGet packages across the solution to net10-compatible versions and
fixed every resulting build error and warning. The solution now builds with
**0 warnings, 0 errors** on a clean rebuild, and `dotnet restore` reports **zero**
`NU1xxx` (outdated/vulnerable package) warnings.

## Files Modified

### Package manifest
- `Directory.Packages.props` — all package versions bumped to net10-compatible
  releases; removed `BlazorInputFile` and `Microsoft.AspNetCore.Mvc` (orphaned/dead);
  removed `System.Net.Http.Json`, `System.Security.Claims`, `System.Text.Json`
  (now framework-provided, were causing NU1510); replaced
  `AutoMapper.Extensions.Microsoft.DependencyInjection` with direct `AutoMapper`
  (16.2.0); added `NuGet.Packaging`/`NuGet.Protocol` 6.12.5 pins to remediate a
  transitive low-severity vulnerability (GHSA-g4vj-cjjj-v7hg) from
  `Microsoft.VisualStudio.Web.CodeGeneration.Design`.

### BlazorInputFile removal (dead code)
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed `BlazorInputFile` and
  `System.Net.Http.Json` package references.
- `src/BlazorShared/BlazorShared.csproj` — removed `BlazorInputFile` package reference.
- `src/BlazorAdmin/_Imports.razor` — removed `@using BlazorInputFile`.
- `src/BlazorShared/Models/CatalogItem.cs` — removed `using BlazorInputFile;` and the
  dead `DataToBase64(IFileListEntry)` method.

### Framework-provided package cleanup
- `src/ApplicationCore/ApplicationCore.csproj` — removed `System.Security.Claims` and
  `System.Text.Json` package references (NU1510).

### AutoMapper DI package swap
- `src/PublicApi/PublicApi.csproj` — replaced
  `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper`.
- `src/PublicApi/Program.cs` — updated `AddAutoMapper(typeof(MappingProfile).Assembly)`
  to `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` (new required config
  delegate parameter).
- `src/Web/Web.csproj` — same AutoMapper reference swap (kept for consistency; this
  project's usage of the package was already minimal/indirect).

### NuGet.Packaging/Protocol transitive vulnerability pin
- `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj` — added explicit
  `PackageReference` for `NuGet.Packaging`/`NuGet.Protocol` (versions resolved from
  the `6.12.5` `PackageVersion` pin in `Directory.Packages.props`) alongside their
  existing `Microsoft.VisualStudio.Web.CodeGeneration.Design` reference.

### Microsoft.OpenApi / Swashbuckle API migration (Swashbuckle 6.5.0 → 10.2.3)
- `src/PublicApi/CustomSchemaFilters.cs` — `Microsoft.OpenApi.Models` → `Microsoft.OpenApi`
  namespace; `ISchemaFilter.Apply(OpenApiSchema, ...)` → `Apply(IOpenApiSchema, ...)`;
  added a null-check on `schema.Properties` (now nullable on the interface).
- `src/PublicApi/Program.cs` — same namespace change; replaced the
  `OpenApiSecurityScheme { Reference = new OpenApiReference {...} }` pattern with
  `OpenApiSecuritySchemeReference("Bearer", doc)` via the new
  `AddSecurityRequirement(Func<OpenApiDocument, OpenApiSecurityRequirement> ...)`
  overload.

### Test fixes
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` —
  `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013).
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` —
  same xUnit2013 fix.
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
  — two `Assert.Equal(1, x.Count)` → `Assert.Single(x)` fixes (xUnit2013).
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — removed the
  obsolete `SerializationInfo`/`StreamingContext` constructor (SYSLIB0051).
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
  — fixed `ReturnsFirst10CatalogItems` to deserialize the actual PublicApi response
  model (`ListPagedCatalogItemResponse`) instead of an unrelated `Web` MVC view model
  (`CatalogIndexViewModel`) that only coincidentally shared a property name; replaced
  `[DataTestMethod]` with `[TestMethod]` (MSTEST0044, obsolete in current MSTest).
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — removed the
  now-unnecessary `ProjectReference` to `src/Web/Web.csproj`. This reference
  transitively pulled in `BlazorAdmin` → `Microsoft.AspNetCore.Components.WebAssembly`
  → `Microsoft.JSInterop.WebAssembly`, which crashed with a `TypeLoadException` when
  reflected over by ASP.NET Core's `ApplicationPartManager` / `MinimalApi.Endpoint`'s
  `AddEndpoints()` under the net10 runtime (confirmed as a genuine upgrade regression,
  not pre-existing, via a `git stash` A/B test against the original net8.0 code).

## Build & Restore Verification
```
dotnet restore eShopOnWeb.sln --force   →  0 warnings, 0 errors
dotnet clean eShopOnWeb.sln -c Release
dotnet build eShopOnWeb.sln -c Release  →  Build succeeded. 0 Warning(s). 0 Error(s).
```

## Notable Decisions Requiring Attention (see final report)
1. **MediatR capped at 12.4.1** (not latest) — avoids v13+ commercial licensing change.
2. **AutoMapper bumped to 16.2.0** (not capped like MediatR) — prioritizes fixing a
   known DoS vulnerability (GHSA-rvv3-g6hj-g44x, unpatched in all MIT-licensed
   releases) over avoiding AutoMapper's own 15.0.0+ licensing change, since
   `securityComplianceCheck=true` is an explicit success criterion. This is an
   intentional inconsistency with the MediatR decision, made because the two
   packages' trade-offs differ (MediatR has no equivalent unpatched-vulnerability
   forcing function).
3. **BlazorInputFile removed** — confirmed dead code, not a functional regression.
