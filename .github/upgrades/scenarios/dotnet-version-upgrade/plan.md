# Upgrade Plan: .NET 8 → .NET 10

## Overview

Upgrade all 10 projects in eShopOnWeb from net8.0 to net10.0 (LTS).

**Source**: net8.0  
**Target**: net10.0  
**Strategy**: In-place upgrade — update TFM, packages, and fix breaking changes

## Key Changes Required

1. **global.json** — Update SDK version from `8.0.x` to `10.0.x`
2. **Directory.Packages.props** — Update `<TargetFramework>`, version properties, and package versions
3. **Source code** — Fix source-incompatible APIs (obsolete serialization constructor, TimeSpan ambiguity, ConfigurationBinder usage)
4. **Package cleanup** — Remove `System.Security.Claims` (included in framework), remove incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
5. **Build & test** — Verify clean build and unit tests pass

## Tasks

### 01-sdk-tfm-packages
Update SDK version in global.json and all package versions + TFM in Directory.Packages.props.

**Files**: global.json, Directory.Packages.props, src/ApplicationCore/ApplicationCore.csproj, src/PublicApi/PublicApi.csproj

**Changes**:
- global.json: `8.0.x` → `10.0.x`
- Directory.Packages.props `<TargetFramework>`: `net8.0` → `net10.0`
- `<AspNetVersion>`: `8.0.2` → `10.0.11`
- `<SystemExtensionVersion>`: `8.0.0` → `10.0.11`  
- `<EntityFramworkCoreVersion>`: `8.0.2` → `10.0.11`
- `<VSCodeGeneratorVersion>`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- Remove `System.Security.Claims` entry (included in net10.0 framework)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` entry (no net10.0-compatible version)
- Remove `System.Security.Claims` PackageReference from ApplicationCore.csproj
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference from PublicApi.csproj

### 02-fix-source-breaks
Fix source-incompatible API usages that will cause compile errors under net10.0.

**Files**:
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — Remove obsolete `(SerializationInfo, StreamingContext)` constructor (SYSLIB0051, removed in .NET 10)
- `src/Web/Configuration/ConfigureCookieSettings.cs` — `TimeSpan.FromMinutes(double)` vs new `TimeSpan.FromMinutes(int)` overload ambiguity fix
- `src/Web/Configuration/ConfigureCoreServices.cs` — Fix `configuration.Get<CatalogSettings>()` (ConfigurationBinder extension moved to opt-in in .NET 10)
- `src/Web/Configuration/ConfigureWebServices.cs` — Fix `services.Configure<CatalogSettings>(configuration)` 
- `src/Web/Program.cs` — Fix `configSection.Get<BaseUrlConfiguration>()`, `builder.Configuration.GetValue(typeof(string), "CatalogBaseUrl")`, `builder.Services.Configure<BaseUrlConfiguration>(configSection)`
- `src/PublicApi/Program.cs` — Fix `builder.Configuration.Get<CatalogSettings>()`, `configSection.Get<BaseUrlConfiguration>()`, `builder.Services.Configure<CatalogSettings>()` / `builder.Services.Configure<BaseUrlConfiguration>(configSection)`

### 03-build-and-test
Build the solution and run unit tests to validate the upgrade.
