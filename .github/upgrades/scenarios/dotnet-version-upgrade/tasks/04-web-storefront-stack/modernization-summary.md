# Modernization Summary: 04-web-storefront-stack

## Task Overview
Upgraded the Web storefront application and all remaining test suites from `net8.0` to `net10.0`.

## Files Changed

### Project Files
| File | Change |
|------|--------|
| `Directory.Packages.props` | Updated `AspNetVersion` 8.0.2→10.0.10, `EntityFramworkCoreVersion` 8.0.2→10.0.10, `VSCodeGeneratorVersion` 8.0.0→10.0.2, `Microsoft.Extensions.Caching.Memory` 9.0.18→10.0.10, xunit 2.7.0→2.9.3, xunit.runner.visualstudio 2.5.6→2.8.2, Microsoft.NET.Test.Sdk 17.9.0→17.13.0 |
| `src/Web/Web.csproj` | Added `net10.0` TFM, removed deprecated `AutoMapper.Extensions.Microsoft.DependencyInjection`, added explicit `NuGet.Packaging`/`NuGet.Protocol` to fix vulnerability warnings |
| `tests/FunctionalTests/FunctionalTests.csproj` | Added `net10.0` TFM, removed obsolete `dotnet-xunit` CLI tool reference |
| `tests/UnitTests/UnitTests.csproj` | Added `net10.0` TFM |
| `tests/IntegrationTests/IntegrationTests.csproj` | Added `net10.0` TFM, added `Nullable`/`ImplicitUsings` settings |

### Source Code Changes
| File | Change |
|------|--------|
| `src/Web/Configuration/ConfigureCookieSettings.cs` | Fixed `TimeSpan.FromMinutes(int)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` (Api.0002: new `long` overload causes ambiguity in .NET 10) |
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` | `Assert.Equal(1, count)` → `Assert.Single()` (xUnit2013) |
| `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` | `Assert.Equal(0, count)` → `Assert.Empty()` (xUnit2013) |
| `tests/IntegrationTests/Repositories/OrderRepositoryTests/GetById.cs` | Added null assertions before property access (CS8602 nullable) |
| `tests/IntegrationTests/Repositories/OrderRepositoryTests/GetByIdWithItemsAsync.cs` | Added null guard + null-forgiving operators (CS8602 nullable) |
| `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` | `Assert.Empty()`, `null!` for non-nullable param (xUnit2013, CS8625) |
| `tests/PublicApiIntegrationTests/ProgramTest.cs` | `WebApplicationFactory<Program>` → `WebApplicationFactory<AuthenticateEndpoint>` to resolve CS0433 ambiguity (both assemblies now on net10.0 share a `Program` type in global namespace) |

## Build Status
- **Errors**: 0
- **Warnings**: 0
- Full solution build (`eShopOnWeb.sln`): ✅ Clean

## Test Results
| Test Suite | Passed | Failed | Total |
|------------|--------|--------|-------|
| UnitTests | 44 | 0 | 44 |
| IntegrationTests | 3 | 0 | 3 |
| FunctionalTests | 12 | 0 | 12 |
| PublicApiIntegrationTests | 15 | 0 | 15 (previously blocked) |
| **Total** | **74** | **0** | **74** |

## Security & Deprecation Findings Resolved
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 (GHSA-rvv3-g6hj-g44x): Removed (not used in Web source)
- `NuGet.Packaging` vulnerability (NU1901): Fixed via explicit version pin
- `xunit` 2.7.0 deprecated: Updated to 2.9.3
- `xunit.runner.console` 2.7.0 deprecated: Updated to 2.9.3
