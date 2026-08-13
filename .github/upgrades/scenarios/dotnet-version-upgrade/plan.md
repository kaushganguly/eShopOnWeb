# Upgrade Plan: eShopOnWeb .NET 8 → .NET 10

## Summary

Upgrade all projects in eShopOnWeb from net8.0 to net10.0 (LTS). 10 projects in scope. Strategy: In-Place upgrade.

## Strategy

In-Place upgrade: update TFM and packages in-place, then fix breaking API changes.

## Task List

### 01-update-tfm-and-sdk
**Update target framework and SDK version**
- Update `Directory.Packages.props`: set `TargetFramework` to `net10.0` and version variables
- Update `global.json`: change SDK version from `8.0.x` to `10.0.x`

### 02-update-nuget-packages
**Update NuGet packages to .NET 10 compatible versions**
- Update `Directory.Packages.props` version variables:
  - `AspNetVersion`: 8.0.2 → 10.0.11
  - `SystemExtensionVersion`: 8.0.0 → 10.0.11
  - `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
  - `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- Update individual packages:
  - `Azure.Identity`: 1.10.4 → 1.21.0
  - `System.Text.Json`: 8.0.3 → 10.0.11
  - `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- Remove incompatible packages:
  - `System.Security.Claims` (now in framework)
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with .NET 10)
- Remove `PackageReference` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from `src/PublicApi/PublicApi.csproj`

### 03-fix-api-breaking-changes
**Fix source-incompatible API changes between .NET 8 and .NET 10**
- Remove obsolete serialization constructor from `EmptyBasketOnCheckoutException.cs`
- Fix `TimeSpan.FromMinutes` ambiguity in `ConfigureCookieSettings.cs`

### 04-build-and-validate
**Build solution and run unit tests**
- Restore NuGet packages
- Build entire solution
- Fix any remaining compilation errors
- Run unit tests

## Execution Constraints

- Execute tasks in order (01 → 02 → 03 → 04)
- Fix all build warnings
- Do not pause for user review
