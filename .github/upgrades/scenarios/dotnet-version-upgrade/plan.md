# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0

## Overview

Upgrade all 10 projects in the eShopOnWeb solution from `net8.0` to `net10.0` LTS.

- **Source TFM**: net8.0
- **Target TFM**: net10.0
- **Strategy**: All-at-once (all projects upgraded together; all are modern .NET, SDK-style)
- **Package Management**: CPM (Central Package Management via Directory.Packages.props)

## Project Dependency Order

Level 0: BlazorShared
Level 1: ApplicationCore, BlazorAdmin
Level 2: Infrastructure
Level 3: PublicApi, Web
Level 4: FunctionalTests, PublicApiIntegrationTests, UnitTests
Level 5: IntegrationTests

## Upgrade Options

- **Strategy**: All-at-once
- **Project Approach**: In-place (all projects already on modern .NET)
- **Package Management**: CPM already enabled
- **Unsupported Packages**: Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6 — remove (no compatible version)
- **API Handling**: Fix in-place (all issues are straightforward)

## Tasks

### 01-tfm-sdk-update
Update the target framework and SDK version.
- Update `global.json`: SDK version from `8.0.x` → `10.0.x`
- Update `Directory.Packages.props`: `TargetFramework` property from `net8.0` → `net10.0`

### 02-package-updates
Update all NuGet package versions in Directory.Packages.props.
- `AspNetVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.Net.Http.Json`: version var → `10.0.11` (covered by SystemExtensionVersion bump)
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- Remove `System.Security.Claims 4.3.0` from Directory.Packages.props and ApplicationCore.csproj (now included in framework)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from Directory.Packages.props and PublicApi.csproj (no compatible version for net10.0)

### 03-api-fixes
Fix API breaking changes and source incompatibilities.
- `EmptyBasketOnCheckoutException.cs`: Remove `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor (removed in .NET 9+)
- `ConfigureCoreServices.cs`: Fix `configuration.Get<CatalogSettings>()` (binary-incompatible API)
- `ConfigureWebServices.cs`: Fix `services.Configure<CatalogSettings>(configuration)` (binary-incompatible API)
- `PublicApi/Program.cs`: Fix `builder.Configuration.Get<CatalogSettings>()`, `builder.Services.Configure<CatalogSettings>(builder.Configuration)`, and `configSection.Get<BaseUrlConfiguration>()`, `builder.Services.Configure<BaseUrlConfiguration>(configSection)` (binary-incompatible APIs)
- `ConfigureCookieSettings.cs`: Fix `TimeSpan.FromMinutes(ValidityMinutesPeriod)` where `int` parameter is source-incompatible (add explicit cast)

### 04-build-validate
Build and test the upgraded solution.
- `dotnet build eShopOnWeb.sln`
- `dotnet test eShopOnWeb.sln`
- Fix any remaining build errors
