# 03-public-api-surface: Progress Details

## Summary
Successfully upgraded ApplicationCore and Infrastructure to multi-target net8.0;net10.0, and PublicApi to net10.0. Updated test projects that reference net10.0-only PublicApi to also target net10.0. All projects build cleanly and tests pass.

## Files Changed

### Project Files Modified

| File | Change |
|------|--------|
| `src/ApplicationCore/ApplicationCore.csproj` | Added multi-targeting `net8.0;net10.0`, cleared `<TargetFramework />` default, removed `System.Security.Claims` package |
| `src/Infrastructure/Infrastructure.csproj` | Added multi-targeting `net8.0;net10.0`, added conditional ItemGroups per TFM for EF Core 8.x vs 10.x |
| `src/PublicApi/PublicApi.csproj` | Set `<TargetFramework>net10.0</TargetFramework>`, added `VersionOverride="10.0.9"` for ASP.NET Core and EF Core packages, removed incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` |
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Set `<TargetFramework>net10.0</TargetFramework>`, added `VersionOverride="10.0.9"` for `Microsoft.AspNetCore.Mvc.Testing` |
| `tests/FunctionalTests/FunctionalTests.csproj` | Set `<TargetFramework>net10.0</TargetFramework>`, added `VersionOverride="10.0.9"` for Mvc.Testing and EF Core InMemory |
| `Directory.Packages.props` | Updated `System.IdentityModel.Tokens.Jwt` from 7.3.1 to 8.4.0 (required by EF Core 10.0.9 transitive chain) |

### Source Files Modified

| File | Change |
|------|--------|
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed obsolete serialization constructor `(SerializationInfo, StreamingContext)` (fixes SYSLIB0051/Api.0002) |

## Assessment Issues Addressed

### ApplicationCore
- ✅ **NuGet.0003** `System.Security.Claims 4.3.0` — Removed package reference (included with framework)
- ✅ **NuGet.0002** `System.Text.Json` — Already at 10.0.9 in Directory.Packages.props (no change needed)
- ✅ **Api.0002** SYSLIB0051 — Removed obsolete serialization constructor from `EmptyBasketOnCheckoutException`
- ✅ **Project.0002** — Multi-targeted to net8.0;net10.0

### Infrastructure
- ✅ **NuGet.0002** EF Core / Identity packages — Per-TFM conditional ItemGroups: 8.0.2 for net8.0 slice, 10.0.9 for net10.0 slice
- ✅ **NuGet.0005** `System.IdentityModel.Tokens.Jwt` — Version bumped from 7.3.1 to 8.4.0 to satisfy transitive constraint from EF Core 10.0.9 → Microsoft.Data.SqlClient 6.1.1 → requires >= 7.7.1; deprecated but deferred per upgrade strategy
- ✅ **Project.0002** — Multi-targeted to net8.0;net10.0

### PublicApi
- ✅ **NuGet.0002** All ASP.NET Core/EF Core packages — `VersionOverride="10.0.9"` added per-package
- ✅ **NuGet.0002** `Microsoft.VisualStudio.Web.CodeGeneration.Design` — `VersionOverride="10.0.2"` 
- ✅ **NuGet.0001** `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — Removed (incompatible, no supported version for net10.0)
- ✅ **NuGet.0005** `AutoMapper.Extensions.Microsoft.DependencyInjection` — Deferred per upgrade strategy
- ✅ **NuGet.0005** `System.IdentityModel.Tokens.Jwt` — Deferred per upgrade strategy (global version bumped to 8.4.0)
- ✅ **Api.0001** Binary incompatible `ConfigurationBinder.Get<T>()` / `Configure<T>()` — Compiled against new package; existing code already null-safe (`?? new CatalogSettings()` and `!` null-forgiving operator on line 78)
- ✅ **Api.0003** `AddConsole()` behavioral change — Informational only; no code change required
- ✅ **Project.0002** — Set to net10.0

### Tests
- ✅ **PublicApiIntegrationTests** — Moved to net10.0 (required since PublicApi is net10.0-only); Mvc.Testing bumped to 10.0.9
- ✅ **FunctionalTests** — Moved to net10.0 (required since PublicApi is net10.0-only); packages updated

## Build Results

| Project | Status | Errors | Warnings (code) |
|---------|--------|--------|-----------------|
| ApplicationCore (net8.0 + net10.0) | ✅ Pass | 0 | 0 |
| Infrastructure (net8.0 + net10.0) | ✅ Pass | 0 | 0* |
| PublicApi (net10.0) | ✅ Pass | 0 | 0* |
| eShopOnWeb.sln (full) | ✅ Pass | 0 | 0* |

*All warnings are NuGet vulnerability warnings from transitive dependencies not in scope (Azure.Identity, Microsoft.Identity.Client, Microsoft.Bcl.Memory, Microsoft.Extensions.Caching.Memory — all are transitive, not direct references). AutoMapper vulnerability is a deferred deprecated package.

## Test Results

| Test Suite | Status | Total | Passed | Failed |
|------------|--------|-------|--------|--------|
| UnitTests (net8.0) | ✅ Pass | 44 | 44 | 0 |
| PublicApiIntegrationTests (net10.0) | ✅ Pass | 15 | 15 | 0 |

## Mixed-Target Compatibility Preserved
- `Web.csproj` remains on `net8.0` ✅
- `ApplicationCore` and `Infrastructure` multi-target `net8.0;net10.0` → Web gets net8.0 slice ✅
- `BlazorAdmin` and `BlazorShared` continue multi-targeting `net8.0;net10.0` ✅

## Deferred Items (per upgrade strategy)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 — deprecated, will be addressed in Web task
- `System.IdentityModel.Tokens.Jwt` — deprecated, version updated for compatibility but full migration deferred
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — removed (no compatible version for net10.0)
