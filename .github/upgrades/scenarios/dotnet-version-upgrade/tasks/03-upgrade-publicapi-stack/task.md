# 03-upgrade-publicapi-stack: 03-upgrade-publicapi-stack

Execute task 03-upgrade-publicapi-stack.

## Research Findings

### PublicApi.csproj
- **Build status before fix**: Success with 6 warnings (NU1901/NU1903 for AutoMapper, NuGet.Packaging, NuGet.Protocol)
- **NuGet.Packaging/NuGet.Protocol 6.12.1** (NU1901): Transitive vulnerability from `Microsoft.VisualStudio.Web.CodeGeneration.Design`. Fixed by adding direct PackageReferences to force the non-vulnerable 6.12.5 versions.
- **AutoMapper 12.0.1** (NU1903): High severity vulnerability from `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1`. Only version 12.0.1 is available in the local package cache; cannot upgrade without network access.
- No compilation errors or code-level issues found.
- PublicApi already targets `net10.0` (via `<TargetFramework>net10.0</TargetFramework>` in Directory.Packages.props).

### PublicApiIntegrationTests.csproj
- **Build error CS0433**: Both `PublicApi` and `Web` assemblies define `public partial class Program {}`, causing type ambiguity.
  - **Root cause**: `CatalogItemListPagedEndpoint.cs` uses `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel`, requiring a reference to `Web.csproj`.
  - **Fix**: Added `Aliases="WebRef"` to the Web project reference, and updated `CatalogItemListPagedEndpoint.cs` to use `extern alias WebRef;` with `WebRef::Microsoft.eShopWeb.Web.ViewModels`.

## Changes Made
1. `src/PublicApi/PublicApi.csproj` — Added direct `PackageReference` for `NuGet.Packaging` and `NuGet.Protocol` to resolve transitive vulnerability at 6.12.5.
2. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — Added `Aliases="WebRef"` to the `Web.csproj` project reference.
3. `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — Added `extern alias WebRef;` and updated `using` to use aliased namespace.

## Build Results
- `src/PublicApi/PublicApi.csproj`: **Build succeeded** — 2 warnings (NU1903 AutoMapper 12.0.1, unfixable without upgrade)
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: **Build succeeded** — no errors

## Test Results
- `PublicApiIntegrationTests`: **15/15 passed**
