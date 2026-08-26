# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary
Upgraded the entire eShopOnWeb solution from .NET 8.0 to .NET 10.0 (latest LTS). Key changes:

- **global.json**: Updated SDK version from `8.0.x` to `10.0.x`
- **Directory.Packages.props**: Updated `TargetFramework` to `net10.0`; updated all NuGet package versions including `Azure.Identity` (1.10.4 → 1.21.0) and `System.IdentityModel.Tokens.Jwt` (7.3.1 → 8.19.2); removed packages now built into .NET 10 (`System.Security.Claims`, `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`)
- **ApplicationCore.csproj**: Removed `System.Security.Claims` and `System.Text.Json` package refs (now in framework)
- **BlazorAdmin.csproj**: Removed `System.Net.Http.Json` (built into net10.0)
- **PublicApi.csproj**: Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)
- **EmptyBasketOnCheckoutException.cs**: Removed obsolete `SerializationInfo`/`StreamingContext` constructor (removed in .NET 9+)
- **ConfigureCookieSettings.cs**: Fixed `TimeSpan.FromMinutes()` ambiguity with explicit `(double)` cast
- **ProgramTest.cs**: Resolved `Program` type ambiguity by switching to `WebApplicationFactory<AuthenticateEndpoint>`

**Build result**: ✅ Succeeded (0 errors, 36 warnings — all security advisories)  
**Unit tests**: ✅ 44/44 passed
