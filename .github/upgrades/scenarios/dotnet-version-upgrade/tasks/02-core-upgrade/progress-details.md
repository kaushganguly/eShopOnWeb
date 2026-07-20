# Progress Details: 02-core-upgrade

## Status: COMPLETED

## Build Result
- **Errors**: 0
- **Warnings**: 0
- **Unit Tests**: 44 passed, 0 failed

## Changes Made

### Directory.Packages.props
- `<TargetFramework>` changed from `net8.0` → `net10.0`
- `<AspNetVersion>` updated to `10.0.10`
- `<SystemExtensionVersion>` updated to `10.0.10`
- `<EntityFramworkCoreVersion>` updated to `10.0.10`
- `<VSCodeGeneratorVersion>` updated to `10.0.2`
- `Azure.Identity` updated from `1.10.4` → `1.21.0`
- `System.Text.Json` updated from `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt` updated from `7.3.1` → `8.19.2`
- REMOVED `System.Security.Claims` (Version="4.3.0")
- REMOVED `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (Version="1.19.6")
- REMOVED `Microsoft.AspNetCore.Mvc` (Version="2.2.0")
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 → replaced with `AutoMapper` 15.1.3 (fixes GHSA-rvv3-g6hj-g44x vulnerability)
- `Microsoft.NET.Test.Sdk` updated from `17.9.0` → `18.8.1`
- Added `NuGet.Packaging` 6.12.5 (fixes GHSA-g4vj-cjjj-v7hg vulnerability)
- Added `NuGet.Protocol` 6.12.5 (fixes GHSA-g4vj-cjjj-v7hg vulnerability)

### Project File Changes

**src/ApplicationCore/ApplicationCore.csproj**
- Removed `<PackageReference Include="System.Security.Claims" />`
- Removed `<PackageReference Include="System.Text.Json" />` (NU1510 - now built into framework)

**src/BlazorAdmin/BlazorAdmin.csproj**
- Removed `<PackageReference Include="System.Net.Http.Json" />` (NU1510 - now built into framework)

**src/PublicApi/PublicApi.csproj**
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />`
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper`
- Added `PrivateAssets="all"` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- Added `NuGet.Packaging` and `NuGet.Protocol` direct references (version pinning)

**src/Web/Web.csproj**
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper`
- Added `PrivateAssets="all"` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- Added `NuGet.Packaging` and `NuGet.Protocol` direct references (version pinning)

### Code Fixes

**src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs**
- Removed protected serialization constructor `(SerializationInfo, StreamingContext)` — removed in .NET 9+

**src/Web/Configuration/ConfigureWebServices.cs**
- `services.Configure<CatalogSettings>(configuration)` → `services.AddOptions<CatalogSettings>().Bind(configuration)`

**src/Web/Configuration/ConfigureCoreServices.cs**
- `configuration.Get<CatalogSettings>()` → `configuration.GetSection("").Get<CatalogSettings>()`

**src/Web/Configuration/ConfigureCookieSettings.cs**
- `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`

**src/PublicApi/Program.cs**
- `builder.Services.Configure<CatalogSettings>(builder.Configuration)` → `builder.Services.AddOptions<CatalogSettings>().Bind(builder.Configuration)`
- `builder.Configuration.Get<CatalogSettings>()` → `builder.Configuration.GetSection("").Get<CatalogSettings>()`
- `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` → `builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>())`

**src/Web/Program.cs**
- Added `internal partial class Program { }` — fixes CS0433 conflict with PublicApi's public Program class in PublicApiIntegrationTests

### Test Fixes (pre-existing xUnit2013 warnings)

**tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs**
- `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)`
- `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)`

**tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs**
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

**tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs**
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

## Issues Resolved
- NU1902: Azure.Identity vulnerability → fixed by upgrading to 1.21.0
- NU1903: System.Text.Json vulnerability → fixed by upgrading to 10.0.10
- NU1903: AutoMapper vulnerability (GHSA-rvv3-g6hj-g44x) → fixed by upgrading to 15.1.3
- NU1901: NuGet.Packaging/Protocol vulnerability (GHSA-g4vj-cjjj-v7hg) → fixed by pinning to 6.12.5
- NU1510: Redundant System.Text.Json and System.Net.Http.Json references → removed
- SYSLIB0051: ObsoleteAttribute serialization constructor → removed
- xUnit2013: Collection size assertions → fixed
- CS0433: Ambiguous Program type → fixed with internal partial class declaration
