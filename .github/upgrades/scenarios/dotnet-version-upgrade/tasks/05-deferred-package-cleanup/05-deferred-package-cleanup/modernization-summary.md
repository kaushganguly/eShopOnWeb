# 05-deferred-package-cleanup: Modernization Summary

## Overview
Resolved all deferred package and shared cleanup work after all application surfaces were moved to net10.0. All incompatible, deprecated, and framework-included packages have been cleaned up and the solution package graph is consistent on net10.0.

## Changes Made

### 1. AutoMapper — HIGH Severity Vulnerability Fixed
- **Removed**: `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 (deprecated/abandoned, CVE GHSA-rvv3-g6hj-g44x)
- **Added**: `AutoMapper` 15.1.3 directly (patched version; 15.1.1+ fixes the vulnerability)
- **Updated**: `PublicApi/Program.cs` line 85 — `services.AddAutoMapper(assembly)` → `services.AddAutoMapper(cfg => cfg.AddMaps(assembly))` to use the AutoMapper 15.x fluent API
- **Projects**: `src/PublicApi/PublicApi.csproj`, `src/Web/Web.csproj` (Web had an unused reference, now removed)

### 2. BlazorInputFile — Incompatible with net10.0, Removed as Dead Code
- **Root cause**: `BlazorInputFile` 0.2.0 has a `ReadRequest` struct with alignment incompatible with net10.0. This caused `ReflectionTypeLoadException` in `MinimalApi.Endpoint.AddEndpoints` during PublicApiIntegrationTests.
- **Finding**: `BlazorInputFile` was only used in dead code (`CatalogItem.DataToBase64` method was never called, `@using BlazorInputFile` in `_Imports.razor` was unused).
- **Removed from**: `BlazorShared.csproj`, `BlazorAdmin.csproj`, `_Imports.razor`, `Directory.Packages.props`
- **Removed dead code**: `DataToBase64(IFileListEntry)` method and associated `using` statements from `BlazorShared/Models/CatalogItem.cs`

### 3. NuGet.Protocol / NuGet.Packaging — LOW Severity Transitive Vulnerability Fixed
- **Issue**: `NuGet.Protocol`/`NuGet.Packaging` 6.12.1 (LOW severity, GHSA-g4vj-cjjj-v7hg) — transitive dependency of `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (latest, no update available)
- **Fix**: Added explicit `NuGet.Protocol` 6.12.5 and `NuGet.Packaging` 6.12.5 references (with `PrivateAssets=all`) in `Web.csproj` and `PublicApi.csproj` to force the patched version
- **Patched version**: 6.12.5 fixes the `>= 6.12.0, <= 6.12.4` vulnerable range

### 4. xunit — Upgraded to Eliminate Transitive Vulnerability Source
- `xunit` 2.7.0 → **2.9.3**
- `xunit.runner.visualstudio` 2.5.6 → **2.8.2** (latest v2-compatible; v3.x requires xunit v3 test code)
- **Removed** `xunit.runner.console` — not needed when running tests via `dotnet test`; was the source of transitive `NuGet.Protocol` 6.12.1 dependency

### 5. MSTest — Upgraded from Deprecated 3.2.2 to 4.3.2
- `MSTest.TestAdapter` 3.2.2 → **4.3.2**
- `MSTest.TestFramework` 3.2.2 → **4.3.2**

### 6. Microsoft.NET.Test.Sdk — Upgraded
- 17.9.0 → **18.8.1**

### 7. System.Security.Claims — Removed (Framework-Included)
- Removed from `Directory.Packages.props` — no project references this package; it is included in the net10.0 framework

### 8. Test Code Quality — xUnit Analyzer Warnings Fixed
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: `Assert.Equal(0, .Count)` → `Assert.Empty()`
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`: Two `Assert.Equal(1, .Count)` → `Assert.Single()` 
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: `Assert.Equal(0, .Count)` → `Assert.Empty()`
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`: `[DataTestMethod]` → `[TestMethod]` (MSTEST0044 fix)

## Files Modified
| File | Change |
|------|--------|
| `Directory.Packages.props` | Replace AutoMapper.Extensions with AutoMapper 15.1.3; upgrade xunit, MSTest, SDK; remove BlazorInputFile, System.Security.Claims, xunit.runner.console; add NuGet.Protocol/Packaging 6.12.5 |
| `src/PublicApi/PublicApi.csproj` | Replace AutoMapper.Extensions with AutoMapper; add NuGet.Protocol/Packaging overrides |
| `src/PublicApi/Program.cs` | Update AddAutoMapper() call to use cfg.AddMaps() syntax |
| `src/Web/Web.csproj` | Remove unused AutoMapper.Extensions; add NuGet.Protocol/Packaging overrides |
| `src/BlazorShared/BlazorShared.csproj` | Remove BlazorInputFile package reference |
| `src/BlazorShared/Models/CatalogItem.cs` | Remove DataToBase64 dead code method and BlazorInputFile/Tasks using statements |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Remove BlazorInputFile package reference |
| `src/BlazorAdmin/_Imports.razor` | Remove @using BlazorInputFile |
| `tests/UnitTests/UnitTests.csproj` | Remove xunit.runner.console reference |
| `tests/UnitTests/.../BasketRemoveEmptyItems.cs` | Fix xUnit2013 warning |
| `tests/UnitTests/.../CustomerOrdersWithItemsSpecification.cs` | Fix 2× xUnit2013 warnings |
| `tests/IntegrationTests/.../SetQuantities.cs` | Fix xUnit2013 warning |
| `tests/PublicApiIntegrationTests/.../CatalogItemListPagedEndpoint.cs` | Fix MSTEST0044 warning |

## Build & Test Results
- **Build**: ✅ 0 errors, 0 warnings
- **Tests**: ✅ 74/74 passing (44 UnitTests + 3 IntegrationTests + 12 FunctionalTests + 15 PublicApiIntegrationTests)

## Remaining Known Issues
None — all deferred package issues have been resolved.
