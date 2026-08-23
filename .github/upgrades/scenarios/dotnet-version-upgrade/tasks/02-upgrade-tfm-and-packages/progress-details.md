# Task Progress: 02-upgrade-tfm-and-packages

## What Changed

### Directory.Packages.props
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.19.2 (required by JwtBearer 10.x transitive deps)
- **Removed**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 (incompatible with net10.0)
- **Removed**: `System.Security.Claims` 4.3.0 (included in net10.0 framework reference)
- **Removed**: `System.Net.Http.Json` and `System.Text.Json` explicit entries (framework-included, NU1510)
- **Removed**: `Microsoft.AspNetCore.Mvc` 2.2.0 (unused entry)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (included in net10.0 framework)
- Removed `System.Text.Json` PackageReference (included in net10.0 framework)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `Exception(SerializationInfo, StreamingContext)` constructor (SYSLIB0051)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (included in net10.0 framework)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProject</Aliases>` to Web project reference to disambiguate `Program` type

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProject;` and updated `using` to `WebProject::Microsoft.eShopWeb.Web.ViewModels`

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Fixed xUnit2013: `Assert.Equal(1, ...)` → `Assert.Single(...)`

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Fixed xUnit2013: `Assert.Equal(0, ...)` → `Assert.Empty(...)`

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Fixed xUnit2013: `Assert.Equal(0, ...)` → `Assert.Empty(...)`

## Known Remaining Warnings

18 security advisory warnings (NU1903/NU1901) from transitive deps:
- AutoMapper 12.0.1 (GHSA-rvv3-g6hj-g44x) — no compatible upgrade path without major breaking changes
- NuGet.Packaging/NuGet.Protocol 6.12.1 (GHSA-g4vj-cjjj-v7hg) — from Microsoft.VisualStudio.Web.CodeGeneration.Design transitive dependency

## Status: ✅ Complete
