# Progress Details: 02-upgrade-projects

## Changes Made

### Directory.Packages.props
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.11 (covers all Microsoft.AspNetCore.* packages)
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)
- `System.Text.Json`: 8.0.3 → 10.0.11
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 REMOVED; replaced with `AutoMapper` 16.2.0 (AutoMapper 13+ merges DI extensions; fixes high severity vulnerability)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 REMOVED (no .NET 10 compatible version)
- `System.Security.Claims` 4.3.0 REMOVED (included in net10.0 framework reference)
- `System.Net.Http.Json` version entry left in CPM but reference removed from BlazorAdmin (included in framework)
- `System.Text.Json` version entry left in CPM but reference removed from ApplicationCore (included in framework)
- `Microsoft.AspNetCore.Mvc` 2.2.0 REMOVED (unused, included in SDK)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (included in framework)
- Removed `System.Text.Json` PackageReference (included in framework)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (included in framework)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible)
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper` (16.2.0 built-in DI)

### src/Web/Web.csproj
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` (unused in Web project)

### src/PublicApi/Program.cs
- Changed `AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))` (AutoMapper 16.x API)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `Exception(SerializationInfo, StreamingContext)` protected constructor (removed in .NET 9+, binary serialization of exceptions is deprecated)

### src/Web/Configuration/ConfigureCookieSettings.cs
- Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` to resolve ambiguity with new `TimeSpan.FromMinutes(long)` overload added in .NET 9

### tests/FunctionalTests/FunctionalTests.csproj
- Removed `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` (legacy, not supported in modern .NET)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Removed `<ProjectReference>` for `Web.csproj` (caused ambiguous `Program` type with .NET 10's publicly-generated top-level statement classes)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Removed `using Microsoft.eShopWeb.Web.ViewModels;` (no longer needed)
- Changed `FromJson<CatalogIndexViewModel>()` → `FromJson<ListPagedCatalogItemResponse>()` (both have `CatalogItems` property; using PublicApi's own type)

## Verification
- `dotnet build eShopOnWeb.sln` exits with 0 errors, 0 actionable warnings
- All 74 tests pass: 44 unit + 3 integration + 12 functional + 15 API integration
