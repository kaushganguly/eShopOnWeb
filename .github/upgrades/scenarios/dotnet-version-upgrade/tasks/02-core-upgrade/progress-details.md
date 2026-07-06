# Progress Details: 02-core-upgrade

## Summary
All 10 eShopOnWeb projects upgraded from net8.0 to net10.0. Packages updated, incompatible packages removed, API breaking changes fixed, build clean with 0 errors and 0 compiler warnings.

## Files Modified

### Directory.Packages.props
- `<TargetFramework>` changed from `net8.0` → `net10.0`
- `<AspNetVersion>` changed from `8.0.2` → `10.0.9`
- `<SystemExtensionVersion>` changed from `8.0.0` → `10.0.9`
- `<EntityFramworkCoreVersion>` changed from `8.0.2` → `10.0.9`
- `<VSCodeGeneratorVersion>` changed from `8.0.0` → `10.0.2`
- `Azure.Identity` version changed from `1.10.4` → `1.21.0`
- `System.Text.Json` version changed from `8.0.3` → `10.0.9`
- `System.IdentityModel.Tokens.Jwt` version changed from `7.3.1` → `8.0.1` (required by new JwtBearer transitive dependency)
- **Removed** `System.Security.Claims` PackageVersion entry (now part of .NET 10 framework)
- **Removed** `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageVersion entry (no net10.0-compatible version)

### src/ApplicationCore/ApplicationCore.csproj
- **Removed** `System.Security.Claims` PackageReference (framework-included in .NET 10)
- **Removed** `System.Text.Json` PackageReference (framework-included in .NET 10, was generating NU1510 "likely unnecessary" warning)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- **Removed** obsolete serialization constructor `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` (was generating SYSLIB0051 warning about obsolete BinaryFormatter-based serialization)

### src/BlazorAdmin/BlazorAdmin.csproj
- **Removed** `System.Net.Http.Json` PackageReference (framework-included in .NET 10, was generating NU1510 "likely unnecessary" warning)

### src/PublicApi/PublicApi.csproj
- **Removed** `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible with net10.0)

### src/PublicApi/Program.cs
- Line 42: `builder.Services.Configure<CatalogSettings>(builder.Configuration)` → `builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection(nameof(CatalogSettings)))` (fix for OptionsConfigurationServiceCollectionExtensions.Configure<T> API change)
- Line 43: `builder.Configuration.Get<CatalogSettings>()` → `builder.Configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()` (fix for ConfigurationBinder.Get<T>() nullable return change)

### src/Web/Configuration/ConfigureCoreServices.cs
- Line 22: `configuration.Get<CatalogSettings>()` → `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()` (fix for ConfigurationBinder.Get<T>() nullable return change)

### src/Web/Configuration/ConfigureWebServices.cs
- Line 16: `services.Configure<CatalogSettings>(configuration)` → `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))` (fix for OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration) API annotation change)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProjectTypes</Aliases>` to the Web project reference to resolve CS0433 ambiguity (both PublicApi and Web have a `Program` class visible to the test project in .NET 10)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProjectTypes;` at top of file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using WebProjectTypes::Microsoft.eShopWeb.Web.ViewModels;`

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Line 22: `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (fix xUnit2013 warning)
- Line 35: `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)` (fix xUnit2013 warning)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Line 19: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (fix xUnit2013 warning)

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Line 38: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (fix xUnit2013 warning)

## Build Results
- **Errors**: 0
- **Compiler Warnings**: 0
- **NuGet Security Advisories (NU1901/NU1903)**: 12 remaining (AutoMapper 12.0.1 GHSA-rvv3-g6hj-g44x)
  - NuGet.Packaging/NuGet.Protocol 6.12.1 warnings FIXED (overridden to 6.14.3 via explicit PackageVersion + PackageReference in Web.csproj and PublicApi.csproj)
  - AutoMapper 12.0.1: cannot be fixed inline — AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1 (deprecated) + AutoMapper 13.x causes CS0121 ambiguity on AddAutoMapper(). Requires migration to AutoMapper 13.x as a separate follow-up task.
- **Projects built**: All 10 projects via eShopOnWeb.sln, all targeting net10.0

## Issues Encountered and Resolutions

### 1. System.Security.Claims PackageReference Still Present
- **Issue**: After removing `System.Security.Claims` from `Directory.Packages.props`, restore failed with NU1010 because `ApplicationCore.csproj` still had the PackageReference.
- **Resolution**: Removed the PackageReference from `ApplicationCore.csproj`.

### 2. Package Downgrade: System.IdentityModel.Tokens.Jwt
- **Issue**: `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9` requires `System.IdentityModel.Tokens.Jwt >= 8.0.1`, but it was pinned to 7.3.1, causing NU1605 downgrade errors.
- **Resolution**: Updated `System.IdentityModel.Tokens.Jwt` from `7.3.1` → `8.0.1` in `Directory.Packages.props`.

### 3. NU1510 Warnings for Framework-Included Packages
- **Issue**: `System.Text.Json` (ApplicationCore) and `System.Net.Http.Json` (BlazorAdmin) were explicitly referenced but are now included in the .NET 10 framework.
- **Resolution**: Removed the PackageReferences from the respective project files.

### 4. CS0433 Program Type Ambiguity
- **Issue**: In .NET 10, both `PublicApi` (explicit `public partial class Program {}`) and `Web` (auto-generated top-level Program) expose a public `Program` type, causing CS0433 in `PublicApiIntegrationTests` which references both.
- **Resolution**: Added `<Aliases>WebProjectTypes</Aliases>` to the Web project reference in `PublicApiIntegrationTests.csproj` and updated the one file that uses Web types to use `extern alias WebProjectTypes`.

### 5. SYSLIB0051 Warning
- **Issue**: `EmptyBasketOnCheckoutException` contained an obsolete binary serialization constructor.
- **Resolution**: Removed the obsolete `(SerializationInfo, StreamingContext)` constructor.

### 6. xUnit2013 Warnings  
- **Issue**: Several test files used `Assert.Equal(0/1, collection.Count)` instead of `Assert.Empty`/`Assert.Single`.
- **Resolution**: Updated 4 test assertions across 3 test files.
