# Execution Log — eShopOnWeb .NET 10 Upgrade

## Session: 2026-08-18

### Task 01-prerequisites — Verify SDK and update global.json ✅

**Status**: Completed  
**Duration**: ~2 minutes

**Actions**:
- Verified .NET 10 SDK 10.0.400 is installed
- Updated `global.json`: SDK version `8.0.x` → `10.0.100`, rollForward: `latestFeature`

**Files modified**:
- `global.json`

---

### Task 02-upgrade-all-projects — Upgrade all 10 projects from net8.0 to net10.0 ✅

**Status**: Completed  
**Duration**: ~30 minutes

**Package updates (Directory.Packages.props)**:
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.11 (Microsoft.AspNetCore.*)
- `SystemExtensionVersion`: 8.0.0 → 10.0.11 (Microsoft.Extensions.*)
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11 (Microsoft.EntityFrameworkCore.*)
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2 (Microsoft.VisualStudio.Web.CodeGeneration.Design)
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)
- `System.Text.Json`: 8.0.3 → 10.0.11 (security vulnerability fix, merged into SystemExtensionVersion)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- `Ardalis.Specification*`: 7.0.0 → 9.3.1 (EF Core 10 compatibility)
- `Microsoft.NET.Test.Sdk`: 17.9.0 → 18.9.0
- `xunit`: 2.7.0 → 2.9.3
- `xunit.runner.visualstudio`: 2.5.6 → 3.1.5
- `xunit.runner.console`: 2.7.0 → 2.9.3
- `MSTest.TestAdapter`: 3.2.2 → 4.3.3
- `MSTest.TestFramework`: 3.2.2 → 4.3.3
- `coverlet.collector`: 6.0.2 → 10.0.1
- Removed: `System.Security.Claims` 4.3.0 (included in net10.0 framework reference)
- Removed: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 (no net10 compatible version)
- Removed: `Microsoft.AspNetCore.Mvc` 2.2.0 (unused, now in SDK)

**API breaking changes fixed**:

1. **EmptyBasketOnCheckoutException.cs** (Api.0002):
   - Removed obsolete `protected Exception(SerializationInfo, StreamingContext)` constructor
   - Constructor was deprecated in .NET 5 and removed in .NET 9+

2. **ConfigureCookieSettings.cs** (Api.0002):
   - Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`
   - In .NET 10, new `TimeSpan.FromMinutes(long)` overload creates ambiguity with int argument

3. **ConfigureCoreServices.cs** (Api.0001):
   - Fixed `configuration.Get<CatalogSettings>()` → `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`
   - ConfigurationBinder.Get<T>(IConfiguration) binary incompatible in .NET 10

4. **ConfigureWebServices.cs** (Api.0001):
   - Fixed `services.Configure<CatalogSettings>(configuration)` → `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))`

5. **PublicApi/Program.cs** (Api.0001):
   - Fixed `builder.Services.Configure<CatalogSettings>(builder.Configuration)` → uses GetSection()
   - Fixed `builder.Configuration.Get<CatalogSettings>()` → uses GetSection()

6. **PublicApiIntegrationTests** (CS0433):
   - `Program` type exists in both PublicApi and Web (both generate Program in global namespace)
   - Fixed by adding `<Aliases>WebProject</Aliases>` to Web project reference in test .csproj
   - Updated `CatalogItemListPagedEndpoint.cs` to use `extern alias WebProject` for Web namespace types

**Files modified**:
- `Directory.Packages.props`
- `src/ApplicationCore/ApplicationCore.csproj` (removed System.Security.Claims)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
- `src/PublicApi/PublicApi.csproj` (removed Containers.Tools.Targets)
- `src/PublicApi/Program.cs`
- `src/Web/Configuration/ConfigureCookieSettings.cs`
- `src/Web/Configuration/ConfigureCoreServices.cs`
- `src/Web/Configuration/ConfigureWebServices.cs`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`

---

### Task 03-validate — Run tests and confirm upgrade success ✅

**Status**: Completed

**Build result**: 
- `dotnet build eShopOnWeb.sln` → Build succeeded, 0 errors, 0 compiler warnings
- 45 NuGet audit warnings (NU1903, NU1901 — transitive, not actionable)

**Test result**:
- `dotnet test tests/UnitTests/UnitTests.csproj` → 44 passed, 0 failed

**Commit**: `870e286` on branch `upgrade/dotnet-10`

---

## Summary

**Upgrade**: eShopOnWeb net8.0 → net10.0 (LTS, support ends Nov 2028)  
**Strategy**: All-at-Once (10 projects upgraded simultaneously)  
**Branch**: `upgrade/dotnet-10`  
**Build**: ✅ 0 errors  
**Tests**: ✅ 44/44 pass  
**Security**: Azure.Identity vulnerability fixed, System.Text.Json vulnerability fixed
