# Upgrade Plan: .NET 8.0 → .NET 10.0

## Overview
Upgrade all 10 projects in eShopOnWeb from net8.0 to net10.0 LTS.

## Strategy
In-place upgrade: update all projects simultaneously.

## Tasks

### 01-update-sdk-and-packages
**Update global.json, Directory.Packages.props, and remove incompatible packages**

- Update `global.json` SDK version to `10.0.x`
- Update `Directory.Packages.props`:
  - Set `<TargetFramework>net10.0</TargetFramework>`
  - Update `<AspNetVersion>10.0.10</AspNetVersion>`
  - Update `<SystemExtensionVersion>10.0.10</SystemExtensionVersion>`
  - Update `<EntityFramworkCoreVersion>10.0.10</EntityFramworkCoreVersion>`
  - Update `<VSCodeGeneratorVersion>10.0.2</VSCodeGeneratorVersion>`
  - Update `Azure.Identity` to `1.21.0`
  - Update `System.Text.Json` to `10.0.10`
  - Update `System.Net.Http.Json` to `10.0.10`
  - Update `System.IdentityModel.Tokens.Jwt` to `8.22.0`
  - Remove `System.Security.Claims` (now included with framework)
  - Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no supported version for .NET 10)
- Remove `System.Security.Claims` from `ApplicationCore.csproj`
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from `PublicApi.csproj`

### 02-fix-api-breaking-changes
**Fix source-incompatible and binary-incompatible API usages**

- `ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Remove obsolete `SerializationInfo/StreamingContext` constructor (removed in .NET 9+)
- `Web/Configuration/ConfigureCookieSettings.cs`: Fix `TimeSpan.FromMinutes(double)` which is now obsolete (use `(long)` cast)
- Verify ConfigurationBinder.Get<T> and Configure<T>(IConfiguration) APIs still compile

### 03-build-and-fix
**Build the solution and fix any remaining errors**

- Run `dotnet build` on the solution
- Fix any compilation errors found

### 04-test
**Run unit tests to verify correctness**

- Run `dotnet test` on UnitTests and IntegrationTests projects
