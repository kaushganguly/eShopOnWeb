# Progress Details: .NET 10 Upgrade

## Summary
Successfully upgraded eShopOnWeb from net8.0 to net10.0.

## Files Modified

| File | Change |
|------|--------|
| `global.json` | SDK version 8.0.x → 10.0.x |
| `Directory.Packages.props` | TargetFramework net8.0 → net10.0; all package versions updated |
| `src/ApplicationCore/ApplicationCore.csproj` | Removed in-box package refs (System.Security.Claims, System.Text.Json) |
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Fixed SYSLIB0051: removed obsolete serialization constructor base call |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed in-box package ref (System.Net.Http.Json) |
| `src/PublicApi/CustomSchemaFilters.cs` | Swashbuckle 10 breaking change: `OpenApiSchema` → `IOpenApiSchema`; `Microsoft.OpenApi.Models` → `Microsoft.OpenApi` |
| `src/PublicApi/Program.cs` | Swashbuckle 10 breaking change: `AddSecurityRequirement` now takes `Func<OpenApiDocument, OpenApiSecurityRequirement>`; `OpenApiSecuritySchemeReference` replaces old Reference pattern; namespace `Microsoft.OpenApi.Models` → `Microsoft.OpenApi` |
| `tests/FunctionalTests/FunctionalTests.csproj` | Removed deprecated `DotNetCliToolReference` for `dotnet-xunit` |
| `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` | xUnit2013: `Assert.Equal(0, ...)` → `Assert.Empty(...)` |
| `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` | Added `extern alias WebProject` to resolve ambiguous `Program` type; `[DataTestMethod]` → `[TestMethod]` (MSTEST0044) |
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Added `<Aliases>WebProject</Aliases>` to Web project reference to disambiguate `Program` type |
| `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` | xUnit2013: `Assert.Equal(0, ...)` → `Assert.Empty(...)` |
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` | xUnit2013: `Assert.Equal(1, ...)` → `Assert.Single(...)` |

## Package Version Changes

### Microsoft Framework Packages
| Package Variable | Before | After |
|----------------|--------|-------|
| TargetFramework | net8.0 | net10.0 |
| AspNetVersion | 8.0.2 | 10.0.11 |
| SystemExtensionVersion | 8.0.0 | 10.0.0 |
| EntityFramworkCoreVersion | 8.0.2 | 10.0.11 |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 |

### Third-Party Packages
| Package | Before | After |
|---------|--------|-------|
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 |
| Ardalis.Specification | 7.0.0 | 9.3.1 |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 |
| Ardalis.Result | 7.0.0 | 10.1.0 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 |
| FluentValidation | 11.9.0 | 12.1.1 |
| MediatR | 12.0.1 | 14.2.0 |
| NSubstitute | 5.1.0 | 6.2.0 |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.2.3 |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.2.3 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 |
| System.Text.Json | 8.0.3 | 10.0.0 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.9.0 |
| MSTest.TestAdapter | 3.2.2 | 4.3.3 |
| MSTest.TestFramework | 3.2.2 | 4.3.3 |
| coverlet.collector | 6.0.2 | 10.0.1 |
| xunit | 2.7.0 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 |
| xunit.runner.console | 2.7.0 | 2.9.3 |

## Validation Results

| Check | Result |
|-------|--------|
| dotnet --version | 10.0.400 |
| dotnet build eShopOnWeb.sln | ✅ 0 errors |
| Compiler warnings | ✅ 0 compiler/analyzer warnings |
| UnitTests (44 tests) | ✅ 44 passed |
| IntegrationTests (3 tests) | ✅ 3 passed |

## Known Remaining Items

### NuGet Security Advisories (NU190x)
These are advisory-only NuGet vulnerability notifications, not compiler warnings:
- **AutoMapper 12.0.1** (GHSA-rvv3-g6hj-g44x, high) — `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 is the latest stable version of this package. Migrating to AutoMapper 13+ requires a separate refactoring effort.
- **NuGet.Packaging/NuGet.Protocol 6.12.1** (low) — These are internal .NET CLI tooling packages not controllable by application code.
