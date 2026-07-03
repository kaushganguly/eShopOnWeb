# 03-upgrade-publicapi: Upgrade PublicApi and Test Projects

## Objective
Upgrade PublicApi project and all test projects (FunctionalTests, PublicApiIntegrationTests, IntegrationTests) to net10.0. Fix incompatible packages, deprecated APIs, and resolve compilation errors.

## Research Findings

### Package State (before changes)
- `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` — deprecated + HIGH vulnerability (GHSA-rvv3-g6hj-g44x)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` — incompatible with net10.0 (NuGet.0001)
- `NuGet.Packaging/NuGet.Protocol 6.12.1` — transitive LOW vulnerability (GHSA-g4vj-cjjj-v7hg) from Microsoft.VisualStudio.Web.CodeGeneration.Design
- `Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2` — upgraded correctly in task 02

### API Changes
- AutoMapper 15.x removed `AddAutoMapper(Assembly)` overload — replaced with `AddAutoMapper(cfg => cfg.AddProfile<T>())`
- All other APIs in PublicApi were already compatible with net10.0

### Test Project Issues
- `PublicApiIntegrationTests` — CS0433 ambiguous type error: both `PublicApi` and `Web` projects define a `partial class Program { }` in the global namespace. The test project referenced both.

### Projects State at Task Start
- All test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests) had no explicit TargetFramework — correctly inheriting `net10.0` from Directory.Packages.props.
- PublicApi.csproj had no explicit TargetFramework — inheriting correctly.

## Changes Made

### Directory.Packages.props
1. Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` with `AutoMapper 15.1.3` (safe version: 15.1.1+ is patched per GHSA-rvv3-g6hj-g44x)
2. Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` entry
3. Added `NuGet.Packaging 6.12.5` override (patched version per GHSA-g4vj-cjjj-v7hg)
4. Added `NuGet.Protocol 6.12.5` override

### src/PublicApi/PublicApi.csproj
1. Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
2. Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`
3. Added `PrivateAssets=all` + `IncludeAssets` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`
4. Added explicit `NuGet.Packaging` + `NuGet.Protocol` PackageReferences with `PrivateAssets=all` to pin safe versions

### src/PublicApi/Program.cs
- Changed `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` → `builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>())` — required for AutoMapper 15.x (Assembly scanning overload removed)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>Web</Aliases>` to the `Web` project reference — prevents the `Program` type from both `PublicApi` and `Web` from colliding in the global namespace (CS0433)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias Web;` at top of file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using Web::Microsoft.eShopWeb.Web.ViewModels;` — required because Web project is now aliased

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Changed `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` — fixes xUnit2013 analyzer warning

## Build Results
- 0 errors, 0 warnings across entire solution
- All projects target net10.0

## Test Results
- UnitTests: 44/44 passed
- IntegrationTests: 3/3 passed
