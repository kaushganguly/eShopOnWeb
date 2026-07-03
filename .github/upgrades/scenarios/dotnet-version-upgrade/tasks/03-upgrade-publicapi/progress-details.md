# Progress Details: 03-upgrade-publicapi

## Summary
Upgraded PublicApi and all test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests) to net10.0. Resolved package incompatibilities, vulnerability warnings, breaking API changes, and a type-ambiguity compilation error.

## Files Modified

### Directory.Packages.props
- **AutoMapper**: Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` (deprecated, HIGH vulnerability) with `AutoMapper 15.1.3` (safe per GHSA-rvv3-g6hj-g44x advisory — patched at 15.1.1)
- **Removed**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` (incompatible with net10.0, NuGet.0001)
- **Added**: `NuGet.Packaging 6.12.5` and `NuGet.Protocol 6.12.5` version overrides (patched versions for GHSA-g4vj-cjjj-v7hg)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`
- Added `PrivateAssets=all` + `IncludeAssets` to `Microsoft.VisualStudio.Web.CodeGeneration.Design` (design-time only tool)
- Added explicit `NuGet.Packaging` and `NuGet.Protocol` PackageReferences with `PrivateAssets=all` to pin patched versions as overrides

### src/PublicApi/Program.cs
- **AutoMapper 15.x breaking change**: `AddAutoMapper(Assembly)` overload removed in 15.x
  - Old: `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)`
  - New: `builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>())`

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>Web</Aliases>` to the `Web` project reference
  - **Root cause**: Both `PublicApi` and `Web` projects define `public partial class Program { }` in the global namespace. When the test project referenced both, the compiler raised CS0433 (ambiguous type). Aliasing Web removes `Web.Program` from the global namespace, leaving only `PublicApi.Program` accessible as `Program`.

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias Web;` at the top of the file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` → `using Web::Microsoft.eShopWeb.Web.ViewModels;`
  - Required because the Web project reference is now aliased — its types are no longer in the default global namespace

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Fixed xUnit2013 warning: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`

## Build Status
| Project | Errors | Warnings |
|---------|--------|----------|
| PublicApi | 0 | 0 |
| FunctionalTests | 0 | 0 |
| IntegrationTests | 0 | 0 |
| PublicApiIntegrationTests | 0 | 0 |
| **Full solution** | **0** | **0** |

## Test Results
| Test Project | Total | Passed | Failed |
|--------------|-------|--------|--------|
| UnitTests | 44 | 44 | 0 |
| IntegrationTests | 3 | 3 | 0 |

FunctionalTests and PublicApiIntegrationTests require a running server (database) and cannot be run in this environment. They compile successfully and are expected to work when a database is available.

## Issues Encountered and Resolved

1. **AutoMapper 15.x API change**: `AddAutoMapper(Assembly)` overload was removed in AutoMapper 15.x. Fixed by switching to `AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>())`.

2. **AutoMapper vulnerability**: All AutoMapper versions < 15.1.1 have GHSA-rvv3-g6hj-g44x (HIGH severity). Upgraded from 12.0.1 to 15.1.3.

3. **CS0433 type ambiguity**: Both PublicApi and Web projects define `partial class Program`. Fixed by aliasing the Web project reference in PublicApiIntegrationTests and updating the one test file that used `Microsoft.eShopWeb.Web.ViewModels`.

4. **NuGet transitive vulnerability**: NuGet.Packaging/Protocol 6.12.1 (LOW severity) from CodeGeneration.Design. Fixed by adding explicit version pins to 6.12.5 in both Directory.Packages.props and PublicApi.csproj.
