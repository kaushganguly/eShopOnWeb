# Task 01-sdk-tfm-packages: Progress Details

## Changes Made

### global.json
- Updated SDK version from `8.0.x` to `10.0.x`

### Directory.Packages.props
- `<TargetFramework>`: `net8.0` → `net10.0`
- `<AspNetVersion>`: `8.0.2` → `10.0.11`
- `<SystemExtensionVersion>`: `8.0.0` → `10.0.11`
- `<EntityFramworkCoreVersion>`: `8.0.2` → `10.0.11`
- `<VSCodeGeneratorVersion>`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0` (resolved NU1605 package downgrade)
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `18.9.0`
- `AutoMapper.Extensions.Microsoft.DependencyInjection` → replaced by `AutoMapper 16.2.0` (deprecated extension merged into AutoMapper 13+)
- Removed: `System.Security.Claims` (now included in net10.0 framework)
- Removed: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 compatible version)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` (included in framework)
- Removed `<PackageReference Include="System.Text.Json" />` (NU1510 - included in framework)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `<PackageReference Include="System.Net.Http.Json" />` (NU1510 - included in framework)

### src/PublicApi/PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` (incompatible)
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`

### src/PublicApi/Program.cs
- Updated `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` to 
  `builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))` (AutoMapper 16.x API)

### src/Web/Web.csproj
- Removed unused `AutoMapper.Extensions.Microsoft.DependencyInjection` reference

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebApp</Aliases>` to Web ProjectReference to resolve `Program` type ambiguity

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebApp;` and `using CatalogIndexViewModel = WebApp::Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel`

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `(SerializationInfo, StreamingContext)` constructor (SYSLIB0051)

### Test files (xUnit2013 warnings)
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`: Changed `Assert.Equal(1, ...)` → `Assert.Single(...)`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: Changed `Assert.Equal(0, ...)` → `Assert.Empty(...)`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: Changed `Assert.Equal(0, ...)` → `Assert.Empty(...)`

## Build Status
✅ Build succeeded — 0 errors, 0 compiler warnings (only low-severity transitive NU1901 NuGet advisories from design-time scaffolding tools remain)
