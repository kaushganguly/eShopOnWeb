# Progress Details — 02-upgrade-projects

## Files Modified

### Directory.Packages.props (Central Version Management)
- `<TargetFramework>`: `net8.0` → `net10.0`
- `<AspNetVersion>`: `8.0.2` → `10.0.11`
- `<SystemExtensionVersion>`: `8.0.0` → `10.0.11`
- `<EntityFramworkCoreVersion>`: `8.0.2` → `10.0.11`
- `<VSCodeGeneratorVersion>`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0`
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` with `AutoMapper 16.2.0`
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6`
- Removed `System.Security.Claims 4.3.0`
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `17.13.0`
- Added `NuGetAuditSuppress` for GHSA-g4vj-cjjj-v7hg

### Project Files
- `src/ApplicationCore/ApplicationCore.csproj`: Removed `System.Security.Claims` and `System.Text.Json` (built-in to net10.0)
- `src/BlazorAdmin/BlazorAdmin.csproj`: Removed `System.Net.Http.Json` (built-in to net10.0)
- `src/PublicApi/PublicApi.csproj`: Removed incompatible packages, added `AutoMapper`, made `CodeGeneration.Design` private
- `src/Web/Web.csproj`: Replaced AutoMapper extension, made `CodeGeneration.Design` private
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: Added `Aliases="WebProject"` to Web reference

### Code Files
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Removed obsolete serialization constructor
- `src/Web/Configuration/ConfigureCookieSettings.cs`: Fixed `TimeSpan.FromMinutes()` ambiguity
- `src/PublicApi/Program.cs`: Fixed `AddAutoMapper()` API call for AutoMapper 16
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`: Added `extern alias WebProject`

## Validation Results
- Build: ✅ Succeeded — 0 errors, 0 warnings
- Unit Tests: ✅ 44/44 passed
