# Task 04: Web Storefront Stack Upgrade

## Objective
Upgrade the storefront application (`src/Web`) and its downstream test suites to `net10.0`.

## Projects in Scope
- `src/Web/Web.csproj` — main ASP.NET Core web frontend
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`

## Assessment Findings

### API Issues (Web)
| Rule | Location | Action |
|------|----------|--------|
| Api.0002 (Source Incompatible) | `ConfigureCookieSettings.cs` - `TimeSpan.FromMinutes(int)` | Fixed with explicit `(double)` cast to avoid overload ambiguity in .NET 10 |
| Api.0001 (Binary Incompatible) | `ConfigureWebServices.cs` - `services.Configure<T>(IConfiguration)` | Built and compiled fine on net10.0 — no runtime breakage observed |
| Api.0001 (Binary Incompatible) | `ConfigureCoreServices.cs` - `IConfiguration.Get<T>()` | Built fine — null-safety handled by existing `?? new CatalogSettings()` pattern |
| Api.0001 (Binary Incompatible) | `Program.cs` - `IConfiguration.GetValue(...)`, `configSection.Get<T>()`, `services.Configure<T>()` | Built fine without code changes |
| Api.0003 (Behavioral) | Various `ReadAsStringAsync()`, `Uri`, `AddConsole`, `UseExceptionHandler` | Behavioral only, no code changes required |

### Package Changes
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` from Web.csproj — not used in any source files (deprecated, has security vulnerability GHSA-rvv3-g6hj-g44x)
- Added explicit `NuGet.Packaging` and `NuGet.Protocol` references to Web.csproj — fix transitive vulnerability warnings (NU1901)
- Updated `Directory.Packages.props` version variables: `AspNetVersion` 8.0.2→10.0.10, `EntityFramworkCoreVersion` 8.0.2→10.0.10, `VSCodeGeneratorVersion` 8.0.0→10.0.2
- Updated `Microsoft.Extensions.Caching.Memory` 9.0.18→10.0.10 (EF Core 10 requires 10.x)
- Updated xunit 2.7.0→2.9.3 (deprecated), xunit.runner.visualstudio 2.5.6→2.8.2, Microsoft.NET.Test.Sdk 17.9.0→17.13.0

### Test Code Fixes
- `UnitTests/CustomerOrdersWithItemsSpecification.cs`: `Assert.Equal(1, count)` → `Assert.Single(collection)` (xUnit2013 analyzer)
- `UnitTests/BasketRemoveEmptyItems.cs`: `Assert.Equal(0, count)` → `Assert.Empty(collection)` (xUnit2013 analyzer)
- `IntegrationTests/GetById.cs`: Added `Assert.NotNull(orderFromRepo)` before property access (CS8602)
- `IntegrationTests/GetByIdWithItemsAsync.cs`: Added null guard, used null-forgiving operator on LINQ `SingleOrDefault()` (CS8602)
- `IntegrationTests/SetQuantities.cs`: `Assert.Equal(0, count)` → `Assert.Empty(collection)`, `null` → `null!` for non-nullable logger param (xUnit2013, CS8625)
- `FunctionalTests/FunctionalTests.csproj`: Removed obsolete `DotNetCliToolReference dotnet-xunit 2.3.1`
- `PublicApiIntegrationTests/ProgramTest.cs`: Fixed `WebApplicationFactory<Program>` ambiguity (both PublicApi and Web expose a `Program` class in global namespace now they share net10.0) → `WebApplicationFactory<AuthenticateEndpoint>`

## Completion Criteria Met
- [x] All 4 projects target `net10.0`
- [x] Security/deprecated packages resolved
- [x] Source-incompatible APIs fixed
- [x] Full solution build: **0 errors, 0 warnings**
- [x] UnitTests: 44 passed
- [x] IntegrationTests: 3 passed
- [x] FunctionalTests: 12 passed
- [x] PublicApiIntegrationTests: 15 passed (previously blocked)
