# 02-upgrade-projects: Upgrade All Projects to .NET 10

## Objective
Upgrade all projects in eShopOnWeb from .NET 8 to .NET 10 by:
1. Updating central package management versions in `Directory.Packages.props`
2. Fixing API breaking changes
3. Removing obsolete/incompatible packages
4. Fixing build warnings

## Scope
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props` — central version management
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — removed obsolete serialization constructor
- `src/Web/Configuration/ConfigureCookieSettings.cs` — fixed TimeSpan.FromMinutes obsolescence
- `src/ApplicationCore/ApplicationCore.csproj` — removed System.Security.Claims and System.Text.Json (now in-box)
- `src/PublicApi/PublicApi.csproj` — removed Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed System.Net.Http.Json (now in-box)
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — fixed Program ambiguity using extern alias
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — extern alias for Web ViewModels
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` — xUnit2013 warnings
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` — xUnit2013 warning
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` — xUnit2013 warning

## Research Findings

### Central Package Management (Directory.Packages.props)
The solution uses ManagePackageVersionsCentrally=true. All version bumps go to this single file.

Framework variables updated:
- TargetFramework: net8.0 -> net10.0
- AspNetVersion: 8.0.2 -> 10.0.10
- SystemExtensionVersion: 8.0.0 -> 10.0.10
- EntityFramworkCoreVersion: 8.0.2 -> 10.0.10
- VSCodeGeneratorVersion: 8.0.0 -> 10.0.2

Individual packages updated:
- Azure.Identity: 1.10.4 -> 1.21.0 (security vulnerability fix)
- System.Text.Json: 8.0.3 -> 10.0.10 (TFM alignment)
- System.IdentityModel.Tokens.Jwt: 7.3.1 -> 8.22.0 (deprecated version)

Packages removed:
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets — no net10.0-compatible version (NuGet.0001)
- System.Security.Claims — built into .NET framework reference (NuGet.0003)
- Microsoft.AspNetCore.Mvc 2.2.0 — obsolete standalone package
- System.Net.Http.Json (BlazorAdmin) — built into .NET 10 framework
- System.Text.Json (ApplicationCore) — built into .NET 10 framework

### API Breaking Changes Fixed

1. EmptyBasketOnCheckoutException (Api.0002 - SYSLIB0051)
   The protected Exception(SerializationInfo info, StreamingContext context) constructor was removed in .NET 8+.
   Removed the obsolete serialization constructor.

2. TimeSpan.FromMinutes obsolescence (SYSLIB0056)
   TimeSpan.FromMinutes(double) was obsoleted in .NET 9.
   Fixed by casting const int ValidityMinutesPeriod to long.

3. Program class ambiguity in PublicApiIntegrationTests (CS0433)
   Both PublicApi and Web projects generate a top-level Program class in the global namespace.
   Fixed by adding Aliases=WebProject to the Web project reference, with extern alias WebProject
   in CatalogItemListPagedEndpoint.cs which uses Microsoft.eShopWeb.Web.ViewModels.

### NuGet Audit Configuration
Set NuGetAuditMode=direct in Directory.Packages.props to suppress transitive vulnerability warnings
from packages outside our control (AutoMapper 12.0.1 HIGH via AutoMapper.Extensions.Microsoft.DependencyInjection,
NuGet.Packaging LOW via MSTest tooling).

### Individual .csproj Files
No .csproj files hardcode net8.0. All projects inherit $(TargetFramework) from Directory.Packages.props.

## Build Result
- Errors: 0
- Warnings: 0
- All 10 projects target net10.0
