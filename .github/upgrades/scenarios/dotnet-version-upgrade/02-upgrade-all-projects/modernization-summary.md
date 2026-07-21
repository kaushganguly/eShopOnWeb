# Task 02-upgrade-all-projects: Modernization Summary

## Objective
Upgrade all 10 projects in the eShopOnWeb solution from `net8.0` to `net10.0`.

## Changes Made

### 1. Project Files — TargetFramework Updated (10 files)
All project files updated from `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`:
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/BlazorShared/BlazorShared.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/PublicApi/PublicApi.csproj`
- `src/Web/Web.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `tests/UnitTests/UnitTests.csproj`

### 2. Directory.Packages.props — Package Version Upgrades
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.0` (all `Microsoft.AspNetCore.*` packages)
- `SystemExtensionVersion`: `8.0.0` → `10.0.0` (`Microsoft.Extensions.*`, `System.Net.Http.Json`)
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.0` (all EF Core packages)
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2` (`Microsoft.VisualStudio.Web.CodeGeneration.Design`)
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.0`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.12.1`
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `17.13.0`
- `xunit`: `2.7.0` → `2.9.3`
- `xunit.runner.visualstudio`: `2.5.6` → `3.1.0`
- `xunit.runner.console`: `2.7.0` → `2.9.3`
- `MSTest.TestAdapter/TestFramework`: `3.2.2` → `3.8.3`
- `coverlet.collector`: `6.0.2` → `6.0.4`
- **REMOVED** `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0)
- **REMOVED** `System.Security.Claims` (now built into .NET 10 framework)

### 3. src/PublicApi/PublicApi.csproj
- Removed `PackageReference` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 version)

### 4. src/ApplicationCore/ApplicationCore.csproj
- Removed `PackageReference` for `System.Security.Claims` (included in .NET 10 framework)

### 5. src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- This constructor relied on `System.Exception(SerializationInfo, StreamingContext)` which was removed in .NET 9/10 as part of BinaryFormatter deprecation
- Verified: no BinaryFormatter usage exists anywhere in the codebase

### 6. tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>global,PublicApiRef</Aliases>` to the `PublicApi` project reference
- Resolves ambiguous `Program` type (both `PublicApi` and `Web` projects use top-level statements, both emit a global-namespace `Program` class)

### 7. tests/PublicApiIntegrationTests/ProgramTest.cs
- Added `extern alias PublicApiRef;` directive
- Updated `WebApplicationFactory<Program>` to `WebApplicationFactory<PublicApiRef::Program>` to unambiguously reference the PublicApi's `Program` class

## Outcome

| Criterion | Status |
|-----------|--------|
| All 10 projects on `net10.0` | ✅ |
| `dotnet build eShopOnWeb.sln` — 0 errors | ✅ |
| No compiler warnings (CS*) | ✅ |
| Unit tests: 44/44 passed | ✅ |
