# Progress Details — 02-upgrade-all-projects

## Summary
All 10 projects in the eShopOnWeb solution have been upgraded from net8.0 to net10.0. The solution builds with 0 errors and 0 warnings.

## Files Modified

### Project files — TargetFramework updated to net10.0
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorAdmin/BlazorAdmin.csproj
- src/BlazorShared/BlazorShared.csproj
- src/Infrastructure/Infrastructure.csproj
- src/PublicApi/PublicApi.csproj
- src/Web/Web.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- tests/UnitTests/UnitTests.csproj

### Directory.Packages.props — Package version updates
- AspNetVersion: 8.0.2 → 10.0.0 (all Microsoft.AspNetCore.* packages)
- EntityFramworkCoreVersion: 8.0.2 → 10.0.0 (all EF Core packages)
- SystemExtensionVersion: 8.0.0 → 10.0.0 (Microsoft.Extensions.*)
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- Azure.Identity: 1.10.4 → 1.21.0 (security vulnerability fix)
- System.Text.Json: 8.0.3 → 10.0.0
- System.IdentityModel.Tokens.Jwt: 7.3.1 → 8.12.1
- xunit / xunit.runner.*: 2.7.0 → 2.9.3 / 3.1.0
- MSTest.TestAdapter / MSTest.TestFramework: 3.2.2 → 3.8.3
- coverlet.collector: 6.0.2 → 6.0.4
- **AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1** → replaced with **AutoMapper 16.2.0** (deprecated package, functionality merged into AutoMapper 13+; also fixes high severity vulnerability GHSA-rvv3-g6hj-g44x)
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** → removed (no net10.0 compatible version)
- **System.Security.Claims** → removed (now included in framework reference)
- Added NuGet.Packaging 7.6.0 transitive pin (fixes NU1901)
- Added NuGet.Protocol 7.6.0 transitive pin (fixes NU1901)
- Enabled CentralPackageTransitivePinningEnabled=true

### Source code changes
- src/ApplicationCore/ApplicationCore.csproj: Removed explicit System.Text.Json PackageReference (now in framework)
- src/BlazorAdmin/BlazorAdmin.csproj: Removed explicit System.Net.Http.Json PackageReference (now in framework)
- src/Web/Web.csproj: Updated AutoMapper.Extensions.Microsoft.DependencyInjection → AutoMapper
- src/PublicApi/PublicApi.csproj: Updated AutoMapper.Extensions.Microsoft.DependencyInjection → AutoMapper
- src/PublicApi/Program.cs: Updated AddAutoMapper call to use AutoMapper 16 API (cfg.AddMaps overload)
- src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs: Removed obsolete BinaryFormatter serialization constructor (removed in .NET 9+)
- tests/PublicApiIntegrationTests: Fixed ambiguous Program type via extern alias
- tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs: xUnit2013 fix — Assert.Equal(0, ...) → Assert.Empty()
- tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs: xUnit2013 fix — Assert.Equal(1, ...) → Assert.Single() (2 occurrences)
- tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs: xUnit2013 fix — Assert.Equal(0, ...) → Assert.Empty()

## Build Result
- `dotnet build eShopOnWeb.sln`: **0 errors, 0 warnings**
