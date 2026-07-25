# Task 02-upgrade-all-projects: Progress Details

## Summary
Upgraded all 10 eShopOnWeb projects from net8.0 to net10.0 in a single atomic pass.

## Changes Made

### Directory.Packages.props
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.10 (Microsoft.AspNetCore.* packages)
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.10 (Microsoft.EntityFrameworkCore.* packages)
- `SystemExtensionVersion`: 8.0.0 → 10.0.10 (Microsoft.Extensions.*, System.Net.Http.Json)
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2 (Microsoft.VisualStudio.Web.CodeGeneration.Design)
- `System.Text.Json`: 8.0.3 → 10.0.10
- `Azure.Identity`: 1.10.4 → 1.21.0 (security fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.21.0
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 → `AutoMapper` 16.2.0 (security fix: GHSA-rvv3-g6hj-g44x)
- `Microsoft.NET.Test.Sdk`: 17.9.0 → 17.13.0
- Removed: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0)
- Removed: `System.Security.Claims` (now included in net10.0 framework)
- Removed: `System.Security.Claims` from ApplicationCore (included in framework)

### global.json
- SDK version: 8.0.x → 10.0.302

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` package (framework-included)
- Removed `System.Text.Json` package (framework-included in net10.0)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` package (framework-included in net10.0)

### src/Web/Web.csproj
- `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper` (direct)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`: added PrivateAssets=all (build tool only)
- `Microsoft.Web.LibraryManager.Build`: added PrivateAssets=all (build tool only)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper` (direct)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`: added PrivateAssets=all (build tool only)

### src/PublicApi/Program.cs
- Updated `AddAutoMapper` call: `typeof(MappingProfile).Assembly` → `cfg => cfg.AddProfile<MappingProfile>()` (AutoMapper 16 API change)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor (removed in .NET 9+)

### src/Web/Configuration/ConfigureCookieSettings.cs
- Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` (ambiguous in .NET 10 due to new long overload)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProject</Aliases>` to Web project reference to resolve CS0433 ambiguity (both PublicApi and Web now export a `Program` type in .NET 10)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProject;` and updated using for `WebProject::Microsoft.eShopWeb.Web.ViewModels`

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Fixed xUnit2013: `Assert.Equal(1, ...)` → `Assert.Single(...)`

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Fixed xUnit2013: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Fixed xUnit2013: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

## Build Results
- **Build**: ✅ Succeeded with 0 errors
- **Remaining Warnings**: 8 NU1901 (low severity) — NuGet.Packaging/Protocol 6.12.1 from Microsoft.VisualStudio.Web.CodeGeneration.Design tooling transitive deps; not fixable without removing scaffolding tool

## Test Results
- **UnitTests**: ✅ 44/44 passed (net10.0)
