# 02-upgrade-projects: Upgrade All Projects to net10.0

## Objective
Upgrade all 10 eShopOnWeb projects from net8.0 to net10.0.

## Research Findings

### TFM Update
- `Directory.Packages.props` holds the central `<TargetFramework>` property used by all projects
- Updated from `net8.0` to `net10.0`

### Package Updates in Directory.Packages.props
- `AspNetVersion`: `8.0.2` → `10.0.11` (all Microsoft.AspNetCore.* and Microsoft.Extensions.*)
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
- `System.Text.Json`: `8.0.3` → `10.0.11` (security fix)
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0` (deprecated fix)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` → replaced with `AutoMapper 16.2.0` (deprecated pkg + vulnerability fix)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no net10.0 version)
- Removed `System.Security.Claims` (now framework-included in net10.0)
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `17.13.0`
- Added `NuGetAuditSuppress` for GHSA-g4vj-cjjj-v7hg (NuGet.Packaging/Protocol transitive from scaffolding tool, dev-time only)

### Breaking API Changes Fixed
1. `EmptyBasketOnCheckoutException.cs`: Removed obsolete `Exception(SerializationInfo, StreamingContext)` constructor (removed in .NET 9+)
2. `ConfigureCookieSettings.cs`: Cast `ValidityMinutesPeriod` to `double` in `TimeSpan.FromMinutes()` call (new int overload ambiguity in .NET 10)
3. `PublicApi/Program.cs`: Updated `AddAutoMapper()` API call (AutoMapper 16 requires `Action<IMapperConfigurationExpression>` first param)
4. `ProgramTest.cs` / `CatalogItemListPagedEndpoint.cs`: Added `extern alias WebProject` to disambiguate `Program` type (in .NET 10 ASP.NET Core SDKs, compiler-generated Program class is now public)

### Project-Level Changes
- `ApplicationCore.csproj`: Removed `System.Security.Claims` and `System.Text.Json` references (built-in to net10.0)
- `BlazorAdmin.csproj`: Removed `System.Net.Http.Json` reference (built-in to net10.0)
- `PublicApi.csproj`: Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` and `AutoMapper.Extensions.Microsoft.DependencyInjection`; added `AutoMapper`; added `PrivateAssets` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `Web.csproj`: Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper`; added `PrivateAssets` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `PublicApiIntegrationTests.csproj`: Added `Aliases="WebProject"` to Web project reference

## Status: Complete
