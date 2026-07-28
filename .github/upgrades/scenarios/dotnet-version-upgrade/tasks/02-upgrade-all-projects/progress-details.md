# Progress Details: 02-upgrade-all-projects

## Summary

All 10 projects successfully upgraded from net8.0 to net10.0. Solution builds with 0 errors and 0 compiler warnings.

## Build Result

- **Errors**: 0
- **Compiler Warnings (CS/MSB)**: 0
- **NuGet Security Audit Warnings**: 36 (NU1901/NU1903 — see Known Issues below)
- **Build Time**: ~4 seconds (incremental), ~10 seconds (full restore)

## Files Changed

### Configuration Files

1. **`Directory.Packages.props`**
   - `TargetFramework`: `net8.0` → `net10.0`
   - `AspNetVersion`: `8.0.2` → `10.0.10`
   - `SystemExtensionVersion`: `8.0.0` → `10.0.10`
   - `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
   - `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
   - `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
   - `System.Text.Json`: `8.0.3` → `10.0.10` (via SystemExtensionVersion)
   - `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.19.2` (required by JwtBearer 10.0.10 transitive dep)
   - `xunit`: `2.7.0` → `2.9.3` (deprecated version update)
   - `xunit.runner.visualstudio`: `2.5.6` → `2.8.2`
   - `xunit.runner.console`: `2.7.0` → `2.9.3`
   - `MSTest.TestAdapter`: `3.2.2` → `3.7.3` (deprecated version update)
   - `MSTest.TestFramework`: `3.2.2` → `3.7.3`
   - **Removed**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no net10.0 version)
   - **Removed**: `System.Security.Claims` (functionality included in framework reference)
   - **Removed**: `Microsoft.AspNetCore.Mvc` 2.2.0 (unused entry, old incompatible version)

### Project Files

2. **`src/PublicApi/PublicApi.csproj`**
   - Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />`

3. **`src/ApplicationCore/ApplicationCore.csproj`**
   - Removed `<PackageReference Include="System.Security.Claims" />` (included in framework)
   - Removed `<PackageReference Include="System.Text.Json" />` (included in framework, NU1510 fix)

4. **`src/BlazorAdmin/BlazorAdmin.csproj`**
   - Removed `<PackageReference Include="System.Net.Http.Json" />` (included in framework, NU1510 fix)

5. **`tests/FunctionalTests/FunctionalTests.csproj`**
   - Removed `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` (legacy tool not supported in modern SDK)

6. **`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`**
   - Added `<Aliases>WebApp</Aliases>` to Web `ProjectReference` to resolve `CS0433` ambiguous `Program` class

### Source Code Fixes

7. **`src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`**
   - Removed the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
   - Reason: SYSLIB0051 — `Exception(SerializationInfo, StreamingContext)` serialization constructor was removed in .NET 9+ (source incompatible)

8. **`src/Web/Configuration/ConfigureCookieSettings.cs`**
   - Changed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((long)ValidityMinutesPeriod)`
   - Reason: SYSLIB0106 — `TimeSpan.FromMinutes(double)` is obsolete-as-error in .NET 9+

9. **`tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`**
   - Added `extern alias WebApp;` directive
   - Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using WebApp::Microsoft.eShopWeb.Web.ViewModels;`
   - Reason: CS0433 — Both `PublicApi` and `Web` assemblies expose a public `Program` class in the global namespace; the extern alias routes `Program` to `PublicApi`'s entry point for `WebApplicationFactory<Program>`

10. **`tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`**
    - Changed `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)`
    - Changed `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)`
    - Reason: xUnit2013 analyzer warning — use Assert.Single for single-element collection checks

11. **`tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`**
    - Changed `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
    - Reason: xUnit2013 analyzer warning — use Assert.Empty for empty collection checks

12. **`tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`**
    - Changed `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
    - Reason: xUnit2013 analyzer warning — use Assert.Empty for empty collection checks

## Build Iterations

1. **Iteration 1**: NU1605 package downgrade errors for `System.IdentityModel.Tokens.Jwt` — `JwtBearer 10.0.10` requires `>= 8.19.2` but pinned at `7.3.1` → Fixed by upgrading to `8.19.2`
2. **Iteration 2**: `CS0433` ambiguous `Program` class in `PublicApiIntegrationTests` → Fixed with `extern alias`
3. **Iteration 3**: Build succeeded with 0 errors, 0 compiler warnings

## Known Issues (Not Fixed)

### NuGet Security Audit Warnings (36 warnings, NU1901/NU1903)

These security audit warnings remain and require follow-up work:

**AutoMapper 12.0.1 — High Severity (GHSA-rvv3-g6hj-g44x)**
- Projects affected: Web, PublicApi, FunctionalTests, PublicApiIntegrationTests, UnitTests, IntegrationTests
- Root cause: `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 depends on `AutoMapper` 12.0.1 which has a vulnerability
- Fix requires: Upgrade to AutoMapper 13+ by replacing `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper` 13+ directly (has breaking API changes). This is a non-trivial refactor outside the scope of this TFM upgrade.

**NuGet.Packaging 6.12.1 and NuGet.Protocol 6.12.1 — Low Severity (GHSA-g4vj-cjjj-v7hg)**
- Projects affected: Web, PublicApi, FunctionalTests, PublicApiIntegrationTests, UnitTests, IntegrationTests
- Root cause: Transitive dependency from `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2
- Fix requires: Waiting for `Microsoft.VisualStudio.Web.CodeGeneration.Design` to update its NuGet dependency, or removing the package entirely

## Validation

- All 10 project binaries confirmed in `bin/Debug/net10.0/` output directories
- TargetFramework is `net10.0` in `Directory.Packages.props` (inherited by all projects)
- No individual project overrides TargetFramework
- Solution restores without errors
- Solution builds with 0 errors, 0 compiler warnings
