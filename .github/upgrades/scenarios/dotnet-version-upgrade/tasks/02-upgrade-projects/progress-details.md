## Files Modified

- `Directory.Packages.props` — TargetFramework, 4 version variables, 3 package version updates, 3 package removals
- `src/ApplicationCore/ApplicationCore.csproj` — Removed `System.Security.Claims` PackageReference (now in-framework); removed `System.Text.Json` PackageReference (framework-included in net10.0)
- `src/BlazorAdmin/BlazorAdmin.csproj` — Removed `System.Net.Http.Json` PackageReference (framework-included in net10.0)
- `src/PublicApi/PublicApi.csproj` — Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — Removed obsolete serialization constructor
- `src/Web/Configuration/ConfigureWebServices.cs` — Fixed `Configure<CatalogSettings>(configuration)` → `Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))`
- `src/Web/Configuration/ConfigureCoreServices.cs` — Fixed `configuration.Get<CatalogSettings>()` → `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>()`
- `src/Web/Configuration/ConfigureCookieSettings.cs` — Fixed obsolete `TimeSpan.FromMinutes(double)` → `new TimeSpan(0, ValidityMinutesPeriod, 0)`
- `src/PublicApi/Program.cs` — Fixed `Configure<CatalogSettings>(builder.Configuration)` and `builder.Configuration.Get<CatalogSettings>()` to use `.GetSection(nameof(CatalogSettings))`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — Added `extern alias WebAlias` to Web project reference to resolve `Program` type ambiguity (CS0433) caused by both `PublicApi` and `Web` exporting a top-level `Program` class in .NET 10
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — Added `extern alias WebAlias;` directive and updated `using Microsoft.eShopWeb.Web.ViewModels` to `using WebAlias::Microsoft.eShopWeb.Web.ViewModels`

## Build Result

- Errors: 0
- Warnings: 18 (pre-existing NuGet security advisories: AutoMapper 12.0.1 NU1903 high severity [GHSA-rvv3-g6hj-g44x], NuGet.Packaging/Protocol NU1901 low severity — all transitive/third-party, require AutoMapper 13+ upgrade which is a separate breaking-change migration)
- Projects built: all 10

## Test Result

- Not run in this task (handled by 03-final-validation)

## Changes Summary

1. **TFM bump**: `net8.0` → `net10.0` in `Directory.Packages.props` (all 10 projects inherit from here)
2. **Version variable updates**: AspNetVersion `8.0.2→10.0.10`, SystemExtensionVersion `8.0.0→10.0.10`, EntityFramworkCoreVersion `8.0.2→10.0.10`, VSCodeGeneratorVersion `8.0.0→10.0.2`
3. **Package security/compatibility updates**: `Azure.Identity` 1.10.4→1.21.0, `System.Text.Json` 8.0.3→10.0.10, `System.IdentityModel.Tokens.Jwt` 7.3.1→8.22.0
4. **Package removals**: Removed `System.Security.Claims` (in-framework), `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 version), `Microsoft.AspNetCore.Mvc` 2.2.0 (incompatible)
5. **Framework-redundant package removals**: Removed explicit `System.Text.Json` ref from ApplicationCore and `System.Net.Http.Json` from BlazorAdmin (both now included in net10.0 framework)
6. **API fixes**: Serialization constructor removal, IConfiguration.Get<T>() → GetSection().Get<T>(), Configure<T>(IConfiguration) → Configure<T>(IConfigurationSection), TimeSpan.FromMinutes → new TimeSpan constructor
7. **CS0433 Program type fix**: extern alias added for PublicApiIntegrationTests to resolve ambiguity introduced by .NET 10's accessible Program class from top-level statements

## Issues Encountered

1. **CS0433 Program type conflict**: In .NET 10, both `PublicApi` and `Web` top-level programs generate an accessible `Program` class. The `PublicApiIntegrationTests` project references both, causing ambiguity. Fixed with `extern alias WebAlias` on the `Web` project reference so `Program` unambiguously refers to `PublicApi`'s entry point.

2. **NU1010 System.Security.Claims**: After removing the `PackageVersion` entry from `Directory.Packages.props`, CPM requires the corresponding `PackageReference` in `ApplicationCore.csproj` also be removed. Fixed by removing the `PackageReference` from the project file.

3. **Remaining warnings (AutoMapper/NuGet transitive)**: AutoMapper 12.0.1 has a pre-existing high severity vulnerability (GHSA-rvv3-g6hj-g44x). Fix requires upgrading to AutoMapper 13+ which is a breaking change migration beyond TFM upgrade scope. Documented for follow-up.
