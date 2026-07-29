# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 (LTS, supported through Nov 2028)
**Scope**: 10 projects — ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web, FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests

## Tasks

### 01-update-sdk-and-packages: Update SDK and NuGet packages

Update global.json to target the .NET 10 SDK and update all centrally managed package versions in Directory.Packages.props. The TargetFramework property is defined centrally in Directory.Packages.props (used by all 10 projects), so a single update propagates to the entire solution.

Key changes:
- global.json: sdk.version `8.0.x` → `10.0.x`
- Directory.Packages.props: `<TargetFramework>` net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.10 (all Microsoft.AspNetCore.* and related packages)
- SystemExtensionVersion: 8.0.0 → 10.0.10 (System.Net.Http.Json, Microsoft.Extensions.*)
- EntityFramworkCoreVersion: 8.0.2 → 10.0.10
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- System.Text.Json: 8.0.3 → 10.0.10
- Azure.Identity: 1.10.4 → 1.21.0 (security vulnerability fix)
- System.IdentityModel.Tokens.Jwt: 7.3.1 → 8.21.0

**Done when**: global.json references .NET 10 SDK; Directory.Packages.props declares net10.0 and updated package versions.

---

### 02-fix-project-files: Remove incompatible and redundant package references

Two project files reference packages that must be removed:
- `src/PublicApi/PublicApi.csproj`: Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (NuGet.0001 — no version supports net10.0)
- `src/ApplicationCore/ApplicationCore.csproj`: Remove `System.Security.Claims` (NuGet.0003 — functionality built into .NET 10 framework reference)
- `Directory.Packages.props`: Remove the `PackageVersion` entries for both of these packages

**Done when**: Neither package appears in any project file or Directory.Packages.props.

---

### 03-fix-api-breaking-changes: Resolve breaking API changes

Fix source and binary-incompatible API usages identified in the assessment:

1. **`EmptyBasketOnCheckoutException.cs`** (Api.0002 — Source incompatible): The `protected Exception(SerializationInfo, StreamingContext)` constructor was removed in .NET 9+. Remove the protected serialization constructor from `EmptyBasketOnCheckoutException`.

2. **`ConfigureWebServices.cs`, `ConfigureCoreServices.cs`, `PublicApi/Program.cs`** (Api.0001 — Binary incompatible): `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` and `ConfigurationBinder.Get<T>(IConfiguration)` changed signatures. Update calls to use `GetSection`-based binding or the options-accepting overloads.

3. **Behavioral changes** (Api.0003 — Potential): Review `HttpContent.ReadAsStringAsync()` usages in health checks — these are informational and may not require code changes.

**Done when**: No source compilation errors from removed/changed APIs; project builds cleanly.

---

### 04-build-and-validate: Build solution and run unit tests

Restore packages, build the full solution, and execute the unit test suite. Fix any remaining compilation errors or test failures introduced by the .NET 10 upgrade.

**Done when**: `dotnet build eShopOnWeb.sln` exits with code 0 (no errors); all unit tests in `tests/UnitTests` pass.

---
