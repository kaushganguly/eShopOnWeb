# Modernization Summary: Task 02-upgrade

## Objective
Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0, update all NuGet packages, and fix breaking API changes.

## Changes Made

### 1. Directory.Packages.props — TFM and Version Variables
- `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- `<AspNetVersion>8.0.2</AspNetVersion>` → `10.0.10`
- `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` → `10.0.10`
- `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` → `10.0.10`
- `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` → `10.0.2`

### 2. Directory.Packages.props — Security & Compatibility Updates
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.19.2` (deprecated, upgrade recommended)
- `System.Text.Json`: `8.0.3` → `$(SystemExtensionVersion)` (now `10.0.10`)
- Removed `System.Security.Claims` (built into .NET 10 framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0)

### 3. Directory.Packages.props — Test Package Updates
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `18.8.1`
- `xunit`: `2.7.0` → `2.9.3`
- `xunit.runner.visualstudio`: `2.5.6` → `3.1.5`
- `xunit.runner.console`: `2.7.0` → `2.9.3`

### 4. Project File Updates
- `src/ApplicationCore/ApplicationCore.csproj`: Removed `System.Security.Claims` and `System.Text.Json` (now implicit in .NET 10)
- `src/BlazorAdmin/BlazorAdmin.csproj`: Removed `System.Net.Http.Json` (implicit in .NET 10)
- `src/PublicApi/PublicApi.csproj`: Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`

### 5. Breaking API Fix — SYSLIB0051 (Obsolete Serialization)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Removed the `SerializationInfo`/`StreamingContext` constructor that is obsolete in .NET 10

### 6. Breaking API Fix — Ambiguous TimeSpan.FromMinutes
- `src/Web/Configuration/ConfigureCookieSettings.cs`: Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` to resolve ambiguity between `int→long` and `int→double` overloads in .NET 10

### 7. Breaking API Fix — Ambiguous Program type in Tests
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: Added `<Aliases>global,PublicApiRef</Aliases>` to the PublicApi project reference
- `tests/PublicApiIntegrationTests/ProgramTest.cs`: Changed from ambiguous `WebApplicationFactory<Program>` to `WebApplicationFactory<PublicApiRef::Program>` using extern alias, resolving the conflict with the `Program` class from `Web` assembly

### 8. xUnit Analyzer Fixes (xUnit2013)
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`: `Assert.Equal(1, ...)` → `Assert.Single(...)` (×2)
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

## Build Results
- **Errors**: 0
- **Code warnings**: 0 (no CS, SYSLIB, or xUnit warnings)
- **NuGet vulnerability warnings**: 36 (NU1901/NU1903 from transitive AutoMapper 12.0.1 and NuGet.Packaging 6.12.1 — pre-existing, require separate upgrade task to fix)

## Test Results
- UnitTests: **44/44 passed**
- IntegrationTests: **3/3 passed**
- PublicApiIntegrationTests: **15/15 passed**

## Notes
- The Api.0001 breaking changes (`OptionsConfigurationServiceCollectionExtensions.Configure<T>` and `ConfigurationBinder.Get<T>`) did NOT cause actual compilation errors in .NET 10. The code compiled and all tests pass without changes to these callers.
- The AutoMapper vulnerability (GHSA-rvv3-g6hj-g44x) in AutoMapper 12.0.1 is a pre-existing issue. Upgrading AutoMapper from 12.x to 13.x+ requires separate API migration work and is not in scope for this TFM upgrade task.
