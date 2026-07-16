# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary
Upgraded eShopOnWeb from .NET 8.0 to .NET 10.0 LTS across all 10 projects (6 source, 4 test).

### Changes Made
- **global.json**: SDK version updated from `8.0.x` to `10.0.x` (using installed SDK 10.0.302)
- **Directory.Packages.props**: `TargetFramework` updated `net8.0` → `net10.0`; version variables updated:
  - `AspNetVersion`: 10.0.10
  - `EntityFrameworkCoreVersion`: 10.0.10
  - `SystemExtensionVersion`: 10.0.10
  - `VSCodeGeneratorVersion`: 10.0.2
- **Security fixes**: `Azure.Identity` 1.10.4 → 1.21.0; `AutoMapper.Extensions.DI` 12 replaced with `AutoMapper` 16.2.0 (fixed GHSA-rvv3-g6hj-g44x HIGH severity)
- **Removed**: `System.Security.Claims` (now framework-included), `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)
- **Enabled**: CPM transitive pinning + `NuGet.Packaging/Protocol` 6.13.2 (fixed GHSA-g4vj-cjjj-v7hg LOW severity)
- **API breaking changes fixed**: `TimeSpan.FromMinutes` cast, `SerializationInfo` ctor removed, AutoMapper 16 `AddAutoMapper` API, `extern alias` for `Program` type ambiguity
- **Test upgrades**: xunit 2.7.0 → 2.9.3, Microsoft.NET.Test.Sdk 17.9.0 → 18.8.1
- **xunit assertion modernization**: `Assert.Equal(0,…)` → `Assert.Empty/Single`

### Test Results
| Suite | Passed | Total |
|-------|--------|-------|
| UnitTests | 44 | 44 |
| IntegrationTests | 3 | 3 |
| FunctionalTests | 12 | 12 |
| PublicApiIntegrationTests | 15 | 15 |
| **Total** | **74** | **74** |

Build: 0 errors, 0 warnings.
