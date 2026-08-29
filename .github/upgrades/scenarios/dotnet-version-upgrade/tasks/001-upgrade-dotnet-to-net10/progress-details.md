# Progress Details: 001-upgrade-dotnet-to-net10

## Summary

Upgraded eShopOnWeb from .NET 8.0 (net8.0) to .NET 10.0 (net10.0). All 10 projects now target net10.0.

**Result**: ✅ Build: 0 errors | Tests: 74/74 passed

---

## Files Modified

### Configuration Files
| File | Change |
|------|--------|
| `Directory.Packages.props` | TargetFramework net8.0 → net10.0; all version variables updated |
| `global.json` | SDK version 8.0.x → 10.0.x |
| `.github/workflows/dotnetcore.yml` | dotnet-version 8.0.x → 10.0.x |

### Project Files
| File | Change |
|------|--------|
| `src/ApplicationCore/ApplicationCore.csproj` | Removed System.Security.Claims, System.Text.Json (bundled in .NET 10) |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed BlazorInputFile, System.Net.Http.Json |
| `src/BlazorAdmin/_Imports.razor` | Removed @using BlazorInputFile |
| `src/BlazorShared/BlazorShared.csproj` | Removed BlazorInputFile package |
| `tests/FunctionalTests/FunctionalTests.csproj` | Removed obsolete DotNetCliToolReference |
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Removed Web project reference (caused WASM assembly conflict) |

### Source Code Fixes
| File | Change |
|------|--------|
| `src/PublicApi/CustomSchemaFilters.cs` | Migrated from OpenApiSchema → IOpenApiSchema; Microsoft.OpenApi.Models → Microsoft.OpenApi |
| `src/PublicApi/Program.cs` | Migrated from OpenApiReference + ReferenceType → OpenApiSecuritySchemeReference; Microsoft.OpenApi.Models → Microsoft.OpenApi; AddSecurityRequirement now uses Func overload |
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed obsolete SerializationInfo/StreamingContext constructor (SYSLIB0051) |
| `src/BlazorShared/Models/CatalogItem.cs` | Removed DataToBase64 method (IFileListEntry from BlazorInputFile was incompatible; method was unused) |

### Test Fixes
| File | Change |
|------|--------|
| `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs` | Assert.Equal(0, Count) → Assert.Empty (xUnit2013) |
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs` | Assert.Equal(1, Count) × 2 → Assert.Single (xUnit2013) |
| `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs` | Assert.Equal(0, Count) → Assert.Empty (xUnit2013) |
| `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` | Replaced CatalogIndexViewModel with correct ListPagedCatalogItemResponse; [DataTestMethod] → [TestMethod] |

---

## Package Version Changes

### Framework-versioned packages (via Directory.Packages.props variables)
| Variable | Old | New |
|----------|-----|-----|
| AspNetVersion | 8.0.2 | 10.0.11 |
| SystemExtensionVersion | 8.0.0 | 10.0.11 |
| EntityFramworkCoreVersion | 8.0.2 | 10.0.11 |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 |

### Third-party packages updated
| Package | Old | New |
|---------|-----|-----|
| Ardalis.Specification | 7.0.0 | 9.3.1 |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 |
| Ardalis.Result | 7.0.0 | 10.1.0 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| FluentValidation | 11.9.0 | 12.1.1 |
| MediatR | 12.0.1 | 14.2.0 |
| NSubstitute | 5.1.0 | 6.2.0 |
| System.Text.Json | 8.0.3 | 10.0.11 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.2.3 |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.2.3 |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.23.0 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.9.0 |
| xunit | 2.7.0 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 |
| xunit.runner.console | 2.7.0 | 2.9.3 |
| MSTest.TestAdapter | 3.2.2 | 4.3.3 |
| MSTest.TestFramework | 3.2.2 | 4.3.3 |
| coverlet.collector | 6.0.2 | 10.0.1 |
| Microsoft.AspNetCore.Mvc | 2.2.0 | 2.3.12 |
| BlazorInputFile | 0.2.0 | **REMOVED** (incompatible with .NET 10) |

---

## Breaking Changes Encountered and Fixed

### 1. Swashbuckle.AspNetCore 6→10 (Microsoft.OpenApi 2.x)
- **Issue**: `Microsoft.OpenApi.Models` namespace removed; `OpenApiSchema` → `IOpenApiSchema`; `OpenApiReference` removed
- **Fix**: Updated CustomSchemaFilters.cs and Program.cs to use new API

### 2. BlazorInputFile 0.2.0
- **Issue**: `ReadRequest` struct has illegal overlapping fields (`[StructLayout(Explicit)]`); .NET 10 throws `ReflectionTypeLoadException`
- **Fix**: Removed package; migrated `IFileListEntry` to `IBrowserFile` (but method was unused, so removed entirely)

### 3. Microsoft.JSInterop.WebAssembly (transitive from Blazor WASM)
- **Issue**: `JSCallInfo` struct has same illegal layout issue when loaded in tests
- **Fix**: Removed Web project reference from PublicApiIntegrationTests; fixed test to use correct API response type

### 4. SYSLIB0051 - Obsolete Serialization Constructor
- **Issue**: `Exception(SerializationInfo, StreamingContext)` obsolete in .NET 10
- **Fix**: Removed the constructor from EmptyBasketOnCheckoutException

---

## Remaining Warnings (not actionable)
- **NU1903** AutoMapper 12.0.1: Transitive from AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1 which has exact version lock; cannot upgrade without breaking changes
- **NU1901** NuGet.Packaging/NuGet.Protocol 6.12.1: Transitive from Microsoft.VisualStudio.Web.CodeGeneration.Design (dev tool)
