# 02-upgrade-all: Upgrade all projects to net10.0 and update all packages

## Objective
Update the centrally-managed TargetFramework from net8.0 to net10.0 in Directory.Packages.props,
update all NuGet package versions to .NET 10 compatible versions, and fix all API breaking changes.

## Scope
- All 10 projects in eShopOnWeb.sln
- Directory.Packages.props (central package management)
- Individual .csproj files requiring fixes
- Source files with API breaking changes

## Research Findings

### Framework Update
- `Directory.Packages.props`: `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- Version variables updated: AspNetVersion 8.0.2→10.0.10, SystemExtensionVersion 8.0.0→10.0.10, EntityFramworkCoreVersion 8.0.2→10.0.10, VSCodeGeneratorVersion 8.0.0→10.0.2

### Package Updates Applied
| Package | From | To |
|---------|------|----|
| Microsoft.AspNetCore.* | 8.0.2 | 10.0.10 (via AspNetVersion) |
| Microsoft.EntityFrameworkCore.* | 8.0.2 | 10.0.10 (via EntityFramworkCoreVersion) |
| Microsoft.Extensions.Identity.Core | 8.0.2 | 10.0.10 |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.10 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 |
| System.Net.Http.Json | 8.0.0 | 10.0.10 |
| System.Text.Json | 8.0.3 | 10.0.10 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.21.0 |

### Packages Removed
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**: Incompatible with net10.0 — removed from Directory.Packages.props and src/PublicApi/PublicApi.csproj
- **System.Security.Claims**: Now included in net10.0 framework — removed from Directory.Packages.props and src/ApplicationCore/ApplicationCore.csproj
- **System.Net.Http.Json** PackageReference in BlazorAdmin: Now included in .NET 10 SDK (NU1510 warning)
- **System.Text.Json** PackageReference in ApplicationCore: Now included in .NET 10 SDK (NU1510 warning)

### API Breaking Changes Fixed

#### EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- Pattern removed in .NET 8+ (SYSLIB0051)

#### ConfigureWebServices.cs
- `services.Configure<CatalogSettings>(configuration)` → `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))`

#### ConfigureCoreServices.cs
- `configuration.Get<CatalogSettings>()` → `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`

#### PublicApi/Program.cs
- `builder.Services.Configure<CatalogSettings>(builder.Configuration)` → `.GetSection(nameof(CatalogSettings))`
- `builder.Configuration.Get<CatalogSettings>()` → `.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`

#### ConfigureCookieSettings.cs
- `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`
- Fixes ambiguity between `FromMinutes(double)` and new `FromMinutes(long)` overload in .NET 9+

### Additional Fixes (Build Warnings)
- **NuGetAuditMode=direct**: Added to suppress transitive tool dependency vulnerability noise (NuGet.Packaging/Protocol from code gen tool)
- **CS0433**: Removed `Web` project reference from `PublicApiIntegrationTests.csproj` (both PublicApi and Web define `Program` class)
- **CatalogItemListPagedEndpoint test**: Replaced `CatalogIndexViewModel` (Web project type) with `ListPagedCatalogItemResponse` (correct PublicApi DTO)
- **xUnit2013**: Fixed 3 test files to use `Assert.Empty`/`Assert.Single` instead of `Assert.Equal(0/1, collection.Count)`

## Files Modified
- Directory.Packages.props
- src/ApplicationCore/ApplicationCore.csproj
- src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- src/BlazorAdmin/BlazorAdmin.csproj
- src/PublicApi/PublicApi.csproj
- src/PublicApi/Program.cs
- src/Web/Configuration/ConfigureWebServices.cs
- src/Web/Configuration/ConfigureCoreServices.cs
- src/Web/Configuration/ConfigureCookieSettings.cs
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs

## Build Result
✅ Build succeeded: 0 errors, 0 warnings
