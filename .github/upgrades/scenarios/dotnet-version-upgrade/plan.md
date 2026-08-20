# .NET Version Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the eShopOnWeb solution (10 projects) from net8.0 to net10.0 (LTS, supported until Nov 2028).

- **Source**: net8.0
- **Target**: net10.0
- **Strategy**: All-projects upgrade in topological order
- **Flow Mode**: Automatic

## Assessment Summary

- 10 projects, 119 issues (21 mandatory, 84 potential, 14 optional)
- Key issues: TFM changes, NuGet upgrades, 1 incompatible package (VisualStudio.Azure.Containers.Tools.Targets), 1 framework-included package (System.Security.Claims), API breaking changes

## Tasks

## 01-update-central-config

Update `global.json` SDK version and `Directory.Packages.props` central package versions.

**Changes:**
- `global.json`: SDK version `8.0.x` → `10.0.x`
- `Directory.Packages.props`:
  - `TargetFramework`: net8.0 → net10.0
  - `AspNetVersion`: 8.0.2 → 10.0.11
  - `SystemExtensionVersion`: 8.0.0 → 10.0.11
  - `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
  - `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
  - `Azure.Identity`: 1.10.4 → 1.21.0 (security fix)
  - `System.Text.Json`: 8.0.3 → 10.0.11
  - `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
  - Remove `System.Security.Claims` (now included in .NET framework)
  - Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10)

## 02-fix-project-references

Remove incompatible/redundant package references from individual project files.

**Changes:**
- `src/ApplicationCore/ApplicationCore.csproj`: Remove `System.Security.Claims` PackageReference
- `src/PublicApi/PublicApi.csproj`: Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
- `tests/FunctionalTests/FunctionalTests.csproj`: Remove deprecated `DotNetCliToolReference` to `dotnet-xunit`

## 03-fix-breaking-api-changes

Fix source-incompatible API changes between .NET 8 and .NET 10.

**Changes:**
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Remove obsolete serialization constructor `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` (base `Exception` constructor removed in .NET 10)

## 04-build-and-fix

Build the full solution and fix any remaining compilation errors or warnings.

## 05-run-tests

Run all unit and integration tests to verify upgrade correctness.
