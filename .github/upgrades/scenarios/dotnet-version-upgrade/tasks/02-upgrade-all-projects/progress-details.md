# Progress Details: 02-upgrade-all-projects

## Summary
Upgraded all 10 projects from net8.0 to net10.0 via centralized package management. Build succeeded with 0 errors, 0 warnings.

## Changes Made

### 1. Directory.Packages.props — Central TFM and Version Bump
- `TargetFramework` net8.0 → net10.0
- `AspNetVersion` 8.0.2 → 10.0.9
- `SystemExtensionVersion` 8.0.0 → 10.0.9
- `EntityFramworkCoreVersion` 8.0.2 → 10.0.9
- `VSCodeGeneratorVersion` 8.0.0 → 10.0.2
- `Azure.Identity` 1.10.4 → 1.21.0
- `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.19.1
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10)
- Removed `System.Security.Claims` (in framework)
- Removed `Microsoft.AspNetCore.Mvc 2.2.0` (unused, in framework)
- Removed `System.Text.Json` PackageVersion (NU1510: in framework for net10)
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` → `AutoMapper 15.1.3`
  - Reason: GHSA-rvv3-g6hj-g44x vulnerability fixed in 15.x; DI integration merged into main package
- Added `CentralPackageTransitivePinningEnabled=true`
- Added `NuGet.Packaging 7.6.0` transitive pin (GHSA-g4vj-cjjj-v7hg fix)
- Added `NuGet.Protocol 7.6.0` transitive pin (GHSA-g4vj-cjjj-v7hg fix)

### 2. src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`

### 3. src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference
- Removed `System.Text.Json` PackageReference (NU1510)

### 4. src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (NU1510)

### 5. src/Web/Web.csproj
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`

### 6. src/PublicApi/Program.cs
- Line 85: `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)`
  → `builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))`
  AutoMapper 15+ API: `AddAutoMapper(Assembly)` overload removed

### 7. src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed protected serialization constructor (SYSLIB0051 obsolete in .NET 9+)

### 8. tests/PublicApiIntegrationTests/ProgramTest.cs
- `WebApplicationFactory<Program>` → `WebApplicationFactory<MappingProfile>`
  Added `using Microsoft.eShopWeb.PublicApi;`
  Reason: CS0433 ambiguity — both Web and PublicApi have top-level `Program` class

### 9. tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (2 instances, xUnit2013)

### 10. tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

### 11. tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

## Issues Encountered and Resolved

| Issue | Resolution |
|-------|-----------|
| AutoMapper 12-14.x: GHSA-rvv3-g6hj-g44x vulnerability | Upgraded to AutoMapper 15.1.3 which fixes it; DI now built into core package |
| AutoMapper 15+ changed `AddAutoMapper` API | Updated to action-based API: `cfg => cfg.AddMaps(assembly)` |
| CS0433: ambiguous `Program` type in PublicApiIntegrationTests | Used `MappingProfile` (PublicApi-only type) as `WebApplicationFactory<T>` type parameter |
| SYSLIB0051: obsolete serialization constructor | Removed the protected ctor (binary serialization removed in .NET 5+) |
| NU1510: System.Text.Json, System.Net.Http.Json | Removed explicit PackageReferences; types available via framework |
| xUnit2013: Assert.Equal for collection size | Updated to Assert.Single/Assert.Empty as recommended |
| NU1901: NuGet.Packaging/Protocol vulnerability | Pinned to 7.6.0 via CentralPackageTransitivePinningEnabled |

## Build Verification
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Projects built for net10.0: BlazorShared, ApplicationCore, Infrastructure, PublicApi, Web, BlazorAdmin, UnitTests, FunctionalTests, IntegrationTests, PublicApiIntegrationTests
