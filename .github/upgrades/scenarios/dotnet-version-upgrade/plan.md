# .NET Version Upgrade Plan: net8.0 → net10.0

## Overview
Upgrade the eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

**Solution**: eShopOnWeb.sln  
**Target Framework**: net10.0  
**Strategy**: In-place upgrade (all projects together)

## Assessment Summary
- 10 projects to upgrade
- Key issues: 21 mandatory, 84 potential, 14 optional
- Incompatible packages: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (remove), `System.Security.Claims` (remove, included in framework)
- API breaking changes in: ApplicationCore, Web, PublicApi

## Task List

### 01-update-sdk-and-tfm
**Update global.json and Directory.Packages.props**
- Update `global.json` SDK version from `8.0.x` to `10.0.x`
- Update `Directory.Packages.props` version properties:
  - `TargetFramework` → `net10.0`
  - `AspNetVersion` → `10.0.10`
  - `SystemExtensionVersion` → `10.0.10`
  - `EntityFramworkCoreVersion` → `10.0.10`
  - `VSCodeGeneratorVersion` → `10.0.2`
  - `Azure.Identity` → `1.21.0` (security fix)
  - `System.Text.Json` → `10.0.10`
  - `System.IdentityModel.Tokens.Jwt` → `8.22.0`
  - Remove `System.Security.Claims` (included in framework)
  - Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)

### 02-fix-api-breaking-changes
**Fix API Breaking Changes**
- Remove obsolete serialization constructor from `EmptyBasketOnCheckoutException`
- Fix `Configure<T>(IConfiguration)` binary incompatibility in Web and PublicApi
- Fix `ConfigurationBinder.Get<T>()` binary incompatibility

### 03-build-and-fix
**Build and resolve any remaining compilation errors**

### 04-run-tests
**Run tests and fix any test failures**
