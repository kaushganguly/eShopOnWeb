# Modernization Summary: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0

**Task ID**: 001-upgrade-dotnet-to-net10  
**Status**: ✅ Complete  
**Build**: ✅ Succeeded (0 warnings, 0 errors)  
**Unit Tests**: ✅ 44/44 passed  
**Integration Tests**: ✅ 3/3 passed  

---

## Changes Made

### 1. SDK & Target Framework

| File | Change |
|------|--------|
| `global.json` | SDK version: `8.0.x` → `10.0.x` |
| `Directory.Packages.props` | `<TargetFramework>net8.0</TargetFramework>` → `net10.0` |

### 2. Microsoft Package Upgrades (Directory.Packages.props)

| Variable / Package | Old Version | New Version |
|--------------------|-------------|-------------|
| `AspNetVersion` | 8.0.2 | 10.0.9 |
| `SystemExtensionVersion` | 8.0.0 | 10.0.9 |
| `EntityFramworkCoreVersion` | 8.0.2 | 10.0.9 |
| `VSCodeGeneratorVersion` | 8.0.0 | 10.0.2 |
| `System.Text.Json` | 8.0.3 | 10.0.9 (via SystemExtensionVersion) |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.19.1 |

All `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, and `Microsoft.AspNetCore.Mvc.Testing` packages updated via the version variables above.

### 3. Third-Party Package Upgrades

| Package | Old Version | New Version | Reason |
|---------|-------------|-------------|--------|
| `Ardalis.Specification` | 7.0.0 | 9.3.1 | EF Core 10 compatibility |
| `Ardalis.Specification.EntityFrameworkCore` | 7.0.0 | 9.3.1 | EF Core 10 compatibility |
| `Azure.Identity` | 1.10.4 | 1.21.0 | Latest stable |
| `Azure.Extensions.AspNetCore.Configuration.Secrets` | 1.3.1 | 1.5.1 | Latest stable |
| `Swashbuckle.AspNetCore` (+ SwaggerUI, Annotations) | 6.5.0 | 10.2.3 | Microsoft.OpenApi 2.x compat |
| **`AutoMapper`** (new direct dep) | — | **15.1.1** | **Fixes CVE-2026-32933 (DoS)** |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | **removed** | Built into AutoMapper 15.x |

### 4. Test Package Upgrades

| Package | Old Version | New Version |
|---------|-------------|-------------|
| `Microsoft.NET.Test.Sdk` | 17.9.0 | 18.7.0 |
| `xunit` | 2.7.0 | 2.9.3 |
| `xunit.runner.visualstudio` | 2.5.6 | 2.8.2 |
| `xunit.runner.console` | 2.7.0 | 2.9.3 |
| `MSTest.TestAdapter` | 3.2.2 | 4.2.3 |
| `MSTest.TestFramework` | 3.2.2 | 4.2.3 |
| `coverlet.collector` | 6.0.2 | 10.0.1 |

---

## Breaking Change Fixes

### Microsoft.OpenApi 2.x (Swashbuckle 10.x)
The `Microsoft.OpenApi.Models` namespace was removed; types moved to `Microsoft.OpenApi`.

**`src/PublicApi/CustomSchemaFilters.cs`**
- `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`
- `ISchemaFilter.Apply(OpenApiSchema, ...)` → `ISchemaFilter.Apply(IOpenApiSchema, ...)`
- Added null guard for `schema.Properties`

**`src/PublicApi/Program.cs`**
- `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`
- `AddSecurityRequirement(new OpenApiSecurityRequirement { new OpenApiSecurityScheme { Reference = ... }, ... })` → `AddSecurityRequirement(document => new OpenApiSecurityRequirement { new OpenApiSecuritySchemeReference("Bearer", document), ... })`

### AutoMapper 15.x (AddAutoMapper API)
`AutoMapper.Extensions.Microsoft.DependencyInjection` removed; `AddAutoMapper` built into AutoMapper 15.x with new signature.

**`src/PublicApi/Program.cs`**
- `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` → `builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))`

### CS0433 Program Type Ambiguity
Both `PublicApi` and `Web` export a `Program` class (top-level statements). Fixed by aliasing the `Web` assembly reference.

**`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`**
- Added `<Aliases>WebProject</Aliases>` to Web project reference

**`tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`**
- Added `extern alias WebProject;`
- `using Microsoft.eShopWeb.Web.ViewModels` → `using WebProject::Microsoft.eShopWeb.Web.ViewModels`

### SYSLIB0051 – Obsolete Serialization Constructor
**`src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`**
- Removed `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor (BinaryFormatter pattern obsolete in .NET 10)

---

## Package Removals & Cleanups

| File | Removed Package | Reason |
|------|----------------|--------|
| `src/ApplicationCore/ApplicationCore.csproj` | `System.Security.Claims`, `System.Text.Json` | Included in .NET 10 runtime (NU1510) |
| `src/BlazorAdmin/BlazorAdmin.csproj` | `System.Net.Http.Json` | Included in .NET 10 runtime (NU1510) |
| `src/Web/Web.csproj` | `AutoMapper.Extensions.Microsoft.DependencyInjection` | Not used in Web project |
| `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj` | `Microsoft.VisualStudio.Web.CodeGeneration.Design` | Dev scaffolding tool pulled vulnerable NuGet.Packaging 6.12.1 |
| `tests/FunctionalTests/FunctionalTests.csproj` | `DotNetCliToolReference dotnet-xunit` | Deprecated, not supported in .NET 10 |

---

## Test Results

```
UnitTests:        Passed  44/44  (net10.0)
IntegrationTests: Passed   3/3   (net10.0)
```

---

## Security Fixes

| Advisory | Package | Severity | Resolution |
|----------|---------|----------|------------|
| GHSA-rvv3-g6hj-g44x (CVE-2026-32933) | AutoMapper < 15.1.1 | High | Upgraded to AutoMapper 15.1.1 (built-in DI extension) |
| GHSA-g4vj-cjjj-v7hg | NuGet.Packaging 6.12.1 | Low | Removed `Microsoft.VisualStudio.Web.CodeGeneration.Design` dev tool |
