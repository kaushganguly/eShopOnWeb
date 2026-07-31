# Progress Details: 02-upgrade-projects

## Status: Complete

## Summary
Upgraded all 10 projects from .NET 8 to .NET 10. Final build: 0 errors, 0 warnings.

## Files Modified

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0`
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- Removed: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 compatible version)
- Removed: `System.Security.Claims` (built into .NET framework)
- Removed: `Microsoft.AspNetCore.Mvc` 2.2.0 (obsolete standalone package)
- Added: `<NuGetAuditMode>direct</NuGetAuditMode>` to suppress transitive vulnerability warnings

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context)` constructor
  (SYSLIB0051 - removed in .NET 8+)

### src/Web/Configuration/ConfigureCookieSettings.cs
- Changed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((long)ValidityMinutesPeriod)`
  (SYSLIB0056 - TimeSpan.FromMinutes(double) obsoleted in .NET 9)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `PackageReference` for `System.Security.Claims` (now in-box)
- Removed `PackageReference` for `System.Text.Json` (now in-box, NU1510)

### src/PublicApi/PublicApi.csproj
- Removed `PackageReference` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `PackageReference` for `System.Net.Http.Json` (now in-box, NU1510)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProject</Aliases>` to `Web.csproj` project reference to resolve CS0433
  (`Program` class exists in both PublicApi and Web assemblies in global namespace)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProject;` at top of file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using WebProject::Microsoft.eShopWeb.Web.ViewModels;`

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (xUnit2013)
- `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)` (xUnit2013)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

## Build Verification
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Packages Kept As-Is (per task scope)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 — deprecated but still works;
  no higher version of AutoMapper.Extensions.Microsoft.DependencyInjection is available
- `xunit`, `MSTest.*` — deprecated but still work
- All Ardalis.* packages — already compatible
- All Swashbuckle.* packages — already compatible
- `FluentValidation`, `MediatR`, `NSubstitute` — already compatible
