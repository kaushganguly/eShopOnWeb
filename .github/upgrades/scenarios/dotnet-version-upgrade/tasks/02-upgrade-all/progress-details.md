# Progress Details: 02-upgrade-all

## Summary
All 10 projects upgraded to net10.0. Solution builds with **0 errors, 0 warnings**.

## Changes Made

### Directory.Packages.props
- `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- `<AspNetVersion>` 8.0.2 → 10.0.10
- `<SystemExtensionVersion>` 8.0.0 → 10.0.10
- `<EntityFramworkCoreVersion>` 8.0.2 → 10.0.10
- `<VSCodeGeneratorVersion>` 8.0.0 → 10.0.2
- `Azure.Identity` 1.10.4 → 1.21.0
- `System.Text.Json` 8.0.3 → 10.0.10 (now uses $(SystemExtensionVersion) variable)
- `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.21.0
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)
- Removed `System.Security.Claims` (now in .NET 10 framework)
- Added `<NuGetAuditMode>direct</NuGetAuditMode>` to suppress transitive tool dependency warnings

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` (now in framework)
- Removed `<PackageReference Include="System.Text.Json" />` (now in framework, NU1510)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- This constructor pattern was removed in .NET 8+ (SYSLIB0051)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `<PackageReference Include="System.Net.Http.Json" />` (now in SDK, NU1510)

### src/PublicApi/PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` (incompatible with net10.0)

### src/Web/Configuration/ConfigureWebServices.cs
- `services.Configure<CatalogSettings>(configuration)` → `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))`

### src/Web/Configuration/ConfigureCoreServices.cs
- `configuration.Get<CatalogSettings>()` → `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`

### src/Web/Configuration/ConfigureCookieSettings.cs
- `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`
- .NET 9+ added `TimeSpan.FromMinutes(long)` overload causing ambiguity with `int` argument

### src/PublicApi/Program.cs
- `builder.Services.Configure<CatalogSettings>(builder.Configuration)` → `.GetSection(nameof(CatalogSettings))`
- `builder.Configuration.Get<CatalogSettings>()` → `.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Removed `<ProjectReference Include="..\..\src\Web\Web.csproj" />` — both PublicApi and Web define a `Program` class (CS0433 error)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Removed `using Microsoft.eShopWeb.Web.ViewModels;` (Web reference dropped)
- Changed `FromJson<CatalogIndexViewModel>()` → `FromJson<ListPagedCatalogItemResponse>()` (correct PublicApi DTO)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Two `Assert.Equal(1, *.Count)` → `Assert.Single(*)` (xUnit2013)

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

## Build Results
- **Errors**: 0
- **Warnings**: 0
- **Time**: ~4 seconds
- **All 10 projects**: Built successfully targeting net10.0
- **Commit**: f1567b7 on branch `upgrade-dotnet-10`
