# .NET 10 Upgrade Plan — eShopOnWeb

## Overview

**Target**: eShopOnWeb solution — 10 projects upgraded from net8.0 to net10.0 (LTS, support ends Nov 2028)
**Scope**: Medium solution with centrally managed packages (Directory.Packages.props), multiple ASP.NET Core and Blazor projects

## Tasks

### 01-sdk-and-packages: Update SDK, TFM, and NuGet packages

Update `global.json` to use the .NET 10 SDK (10.0.x with latestFeature rollForward). Update `Directory.Packages.props` to change the central `TargetFramework` property from `net8.0` to `net10.0`, and update all version variables: `AspNetVersion` → `10.0.11`, `SystemExtensionVersion` → `10.0.11`, `EntityFramworkCoreVersion` → `10.0.11`, `VSCodeGeneratorVersion` → `10.0.2`. Also update `Azure.Identity` to `1.21.0` (security vulnerability fix) and `System.Text.Json` to `10.0.11`. Remove `System.Security.Claims` package reference (its functionality is now included in the .NET 10 framework reference). Remove the `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` package entry (incompatible, no supported version available). Also remove the package reference from `src/PublicApi/PublicApi.csproj`.

**Done when**: `global.json` targets .NET 10 SDK, `Directory.Packages.props` uses net10.0 TFM with updated package versions, PublicApi.csproj no longer references the incompatible container tools package.

### 02-fix-api-breaking-changes: Fix API breaking changes

Fix the binary and source incompatible API changes identified in the assessment. 

Remove the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor in `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — the `Exception(SerializationInfo, StreamingContext)` protected constructor was removed in .NET 9 and is no longer available.

Fix `ConfigurationBinder.Get<T>()` usages in `src/Web/Configuration/ConfigureCoreServices.cs` and `src/Web/Program.cs`, `src/PublicApi/Program.cs` — the return type is now nullable `T?`. Fix `Configure<T>(IServiceCollection, IConfiguration)` usages in `src/Web/Configuration/ConfigureWebServices.cs` and Program files — this overload changed in .NET 10. Fix `ConfigurationBinder.GetValue(Type, string)` in `src/Web/Program.cs`.

**Done when**: All flagged source and binary incompatible usages are updated.

### 03-build-and-fix: Build and fix compilation errors

Run `dotnet build` against the full solution. Resolve all remaining compiler errors and eliminate all warnings in modified projects. Common patterns to fix: nullable reference type warnings, changed return types, removed APIs.

**Done when**: `dotnet build eShopOnWeb.sln` exits with code 0 and zero errors.

### 04-run-tests: Run unit tests and validate

Run `dotnet test` for the UnitTests project. Fix any failing tests caused by behavioral changes introduced by the .NET 10 upgrade.

**Done when**: All unit tests in `tests/UnitTests` pass.
