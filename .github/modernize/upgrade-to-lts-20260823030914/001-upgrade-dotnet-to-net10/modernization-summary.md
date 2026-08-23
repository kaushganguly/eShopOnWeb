# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary

Upgraded the entire eShopOnWeb solution (6 application projects + 4 test projects) from **.NET 8.0 to .NET 10.0 LTS**.

### Key changes made:
| File | Change |
|---|---|
| `global.json` | SDK version updated from `8.0.x` to `10.0.x` |
| `Directory.Packages.props` | All version-aligned NuGet packages bumped; 4 obsolete packages removed/updated |
| `src/ApplicationCore/ApplicationCore.csproj` | Removed redundant framework-included packages |
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed obsolete serialization constructor (SYSLIB0051) |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed redundant `System.Net.Http.Json` reference |
| `src/PublicApi/PublicApi.csproj` | Removed incompatible `Containers.Tools.Targets` package |
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Added `extern alias` for Web project to fix `Program` ambiguity |
| `tests/PublicApiIntegrationTests/.../CatalogItemListPagedEndpoint.cs` | Updated to use `WebProject::` alias |
| 3 test files | Fixed xUnit2013 warnings (`Assert.Single`/`Assert.Empty`) |

### Test results:
| Suite | Passed | Total |
|---|---|---|
| UnitTests | ✅ 44 | 44 |
| IntegrationTests | ✅ 3 | 3 |
| FunctionalTests | ✅ 12 | 12 |
| PublicApiIntegrationTests | ✅ 15 | 15 |
| **Total** | **✅ 74** | **74** |

### Known residual issues:
18 security advisory warnings from transitive packages (AutoMapper 12.x, NuGet.Packaging/Protocol) that cannot be resolved without breaking API changes outside this upgrade scope.
