# Task 02: Upgrade All Projects – Progress Details

## Status: Complete

## Target Framework

All 10 projects now target `net10.0` via the central `Directory.Packages.props` property `<TargetFramework>net10.0</TargetFramework>`.

## Package Changes (Directory.Packages.props)

| Package | Old Version | New Version | Reason |
|---|---|---|---|
| `Azure.Identity` | 1.10.4 | 1.21.0 | Security vulnerability fix; required by EF Core SqlServer 10.0.10 |
| `System.Text.Json` | 8.0.3 | 10.0.10 | Security vulnerability fix; align with .NET 10 |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.19.2 | Required by JwtBearer 10.0.10 (transitive constraint) |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | 1.19.6 | *removed* | Incompatible with net10.0 |
| `System.Security.Claims` | 4.3.0 | *removed* | Functionality included in .NET 10 framework |
| `NuGetAuditSuppress` (GHSA-rvv3-g6hj-g44x) | added | — | AutoMapper vulnerability has no patched version — acknowledged |
| `NuGetAuditSuppress` (GHSA-g4vj-cjjj-v7hg) | added | — | NuGet.Packaging/Protocol via CodeGeneration.Design, no patched version |

## Project File Changes

| File | Change |
|---|---|
| `src/ApplicationCore/ApplicationCore.csproj` | Removed `System.Security.Claims` and `System.Text.Json` references (included in framework) |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed `System.Net.Http.Json` reference (included in framework) |
| `src/PublicApi/PublicApi.csproj` | Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference |

## API Breaking Change Fixes

| File | Change | Reason |
|---|---|---|
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed `SerializationInfo`/`StreamingContext` constructor | Source-incompatible in .NET 10 (BinaryFormatter removed) |
| `src/Web/Configuration/ConfigureCookieSettings.cs` | `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((long)ValidityMinutesPeriod)` | `double` overload source-incompatible in .NET 10 |
| `src/Web/Program.cs` | `GetValue(typeof(string), "CatalogBaseUrl")` → `GetValue<string>("CatalogBaseUrl")` | Non-generic overload binary-incompatible in .NET 10 |

## Other Fixes

| File | Change | Reason |
|---|---|---|
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Added `Aliases="global,PublicApiAlias"` to PublicApi reference | Resolve `Program` type ambiguity |
| `tests/PublicApiIntegrationTests/ProgramTest.cs` | `WebApplicationFactory<Program>` → `WebApplicationFactory<PublicApiAlias::Program>` | Disambiguate `Program` between PublicApi and Web |
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` | `Assert.Equal(1, ...)` → `Assert.Single(...)` | Fix xUnit2013 analyzer warnings |
| `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` | `Assert.Equal(0, ...)` → `Assert.Empty(...)` | Fix xUnit2013 analyzer warnings |
| `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` | `Assert.Equal(0, ...)` → `Assert.Empty(...)` | Fix xUnit2013 analyzer warnings |

## Build Results

- **Errors**: 0
- **Warnings**: 0
- All 10 projects build cleanly targeting net10.0

## Suppressed Advisories (no patch available)

- **GHSA-rvv3-g6hj-g44x** (HIGH): AutoMapper 12.0.1 via `AutoMapper.Extensions.Microsoft.DependencyInjection`. GitHub confirms Patched in: None.
- **GHSA-g4vj-cjjj-v7hg** (LOW): NuGet.Packaging/Protocol 6.12.1 via `Microsoft.VisualStudio.Web.CodeGeneration.Design`. GitHub confirms Patched in: None. Build-tool-only (PrivateAssets=all).

## Test Results

- **UnitTests**: 44 passed, 0 failed
- **FunctionalTests**: 12 passed, 0 failed
- **IntegrationTests**: 3 passed, 0 failed
- **PublicApiIntegrationTests**: 15 passed, 0 failed
- **Total**: 74 passed, 0 failed
