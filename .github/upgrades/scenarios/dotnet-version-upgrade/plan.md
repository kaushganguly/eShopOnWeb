# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0

## Overview

Upgrade all 10 projects in the eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

- **Solution**: eShopOnWeb.sln
- **Target Framework**: net10.0
- **Strategy**: In-place upgrade (all projects upgraded simultaneously using central packages)

## Assessment Summary

- 10 projects, all targeting net8.0
- 119 issues (21 mandatory, 84 potential, 14 optional)
- Central package management via Directory.Packages.props
- Centrally defined TargetFramework — single update propagates to all projects

### Key Issues

| Category | Count | Notes |
|---|---|---|
| Target framework update | 10 | All projects via Directory.Packages.props |
| NuGet package upgrades | 32 | Most via central version vars |
| Incompatible package | 1 | Microsoft.VisualStudio.Azure.Containers.Tools.Targets |
| Package in framework | 1 | System.Security.Claims → remove |
| Source incompatible APIs | 3 | SerializationInfo ctor, TimeSpan.FromMinutes ambiguity |
| Binary incompatible APIs | 9 | IConfiguration.Get<T>, Configure<T>(IConfiguration) |
| Deprecated packages | 12 | AutoMapper.Extensions, xunit, MSTest, System.IdentityModel |

## Tasks

### 01-update-sdk-and-tfm
Update global.json (SDK 10.0.x) and Directory.Packages.props (net10.0 + all package versions).

**Files**:
- global.json
- Directory.Packages.props

**Changes**:
- SDK: 8.0.x → 10.0.x
- TargetFramework: net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.11
- SystemExtensionVersion: 8.0.0 → 10.0.11
- EntityFramworkCoreVersion: 8.0.2 → 10.0.11
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- Azure.Identity: 1.10.4 → 1.21.0
- System.Text.Json: 8.0.3 → 10.0.11
- System.IdentityModel.Tokens.Jwt: 7.3.1 → 8.22.0
- Remove System.Security.Claims (included in net10.0 framework)

### 02-fix-source-incompatibilities
Fix source-level breaking changes that prevent compilation.

**Files**:
- src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- src/ApplicationCore/ApplicationCore.csproj
- src/Web/Configuration/ConfigureCookieSettings.cs
- src/PublicApi/PublicApi.csproj

**Changes**:
- Remove obsolete protected serialization constructor from EmptyBasketOnCheckoutException
- Remove System.Security.Claims PackageReference from ApplicationCore.csproj
- Fix TimeSpan.FromMinutes ambiguity (cast int to double)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets from PublicApi.csproj (incompatible)

### 03-build-and-validate
Build solution and run unit tests to verify upgrade.

**Steps**:
- dotnet restore
- dotnet build --no-incremental
- dotnet test tests/UnitTests (unit tests)
