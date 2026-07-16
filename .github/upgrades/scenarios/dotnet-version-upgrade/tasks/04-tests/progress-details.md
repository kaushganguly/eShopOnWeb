# Task 04-tests: Progress Details

## Status: Complete

## Execution Summary

### Phase 1: Research
Read all relevant project files:
- `Directory.Packages.props` — confirmed net10.0 TFM set globally; identified stale package versions
- All 4 test `.csproj` files — none had explicit TFM (inherited from Directory.Packages.props)

### Phase 2: Package Version Updates (Directory.Packages.props)
| Package | Old Version | New Version |
|---------|-------------|-------------|
| xunit | 2.7.0 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.6 | 2.8.2 |
| xunit.runner.console | 2.7.0 | 2.9.3 |
| MSTest.TestAdapter | 3.2.2 | 4.3.2 |
| MSTest.TestFramework | 3.2.2 | 4.3.2 |
| BlazorInputFile | 0.2.0 | REMOVED |

### Phase 3: Project File Fixes

**FunctionalTests.csproj**
- Removed `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` — deprecated, unsupported in .NET 10

**PublicApiIntegrationTests.csproj**
- Removed `<ProjectReference Include="..\..\src\Web\Web.csproj" />` — caused `CS0433` build error because both PublicApi and Web expose a global-namespace `Program` class via top-level statements

**CatalogItemListPagedEndpoint.cs**
- Replaced `CatalogIndexViewModel` (Web.ViewModels) with `ListPagedCatalogItemResponse` (PublicApi) — necessary after removing Web project reference; both have compatible `CatalogItems` collections for the test assertions

### Phase 4: xUnit Analyzer Warning Fixes
Fixed 4 `xUnit2013` warnings:
- 2 in `CustomerOrdersWithItemsSpecification.cs`: `Assert.Equal(1, ...)` → `Assert.Single(...)`
- 1 in `BasketRemoveEmptyItems.cs`: `Assert.Equal(0, ...)` → `Assert.Empty(...)`
- 1 in `SetQuantities.cs`: `Assert.Equal(0, ...)` → `Assert.Empty(...)`

### Phase 5: BlazorInputFile Migration (Unplanned but Required)

**Root cause**: `BlazorInputFile` 0.2.0 has a struct layout incompatibility with .NET 10's stricter object field alignment rules. When `WebApplicationFactory<Program>` started the PublicApi in `PublicApiIntegrationTests`, `MinimalApi.Endpoint.Extensions.IServiceCollectionExtensions.AddEndpoints()` scanned all assemblies via reflection and threw:
```
Could not load type 'ReadRequest' from assembly 'BlazorInputFile, Version=1.0.0.0...' 
because it contains an object field at offset 4 that is incorrectly aligned.
```

**Dependency chain**: PublicApiIntegrationTests → PublicApi → ApplicationCore → BlazorShared → BlazorInputFile

**Fix applied**:
- `BlazorShared/Models/CatalogItem.cs`: Changed `DataToBase64(IFileListEntry)` → `DataToBase64(Stream)`. No callers exist in the codebase, so this is a safe change. Callers should pass `IBrowserFile.OpenReadStream()` or any stream.
- Removed `BlazorInputFile` package references from `BlazorShared.csproj` and `BlazorAdmin.csproj`
- Removed `@using BlazorInputFile` from `BlazorAdmin/_Imports.razor`
- Removed `BlazorInputFile` from `Directory.Packages.props`

## Build Results
| Project | Errors | Warnings (CS only) |
|---------|--------|---------------------|
| UnitTests | 0 | 0 |
| IntegrationTests | 0 | 0 |
| FunctionalTests | 0 | 0 |
| PublicApiIntegrationTests | 0 | 0 |
| Full Solution | 0 | 0 (only NU vulnerability warnings) |

## Test Results
| Project | Total | Passed | Failed |
|---------|-------|--------|--------|
| UnitTests | 44 | 44 | 0 |
| IntegrationTests | 3 | 3 | 0 |
| FunctionalTests | 12 | 12 | 0 |
| PublicApiIntegrationTests | 15 | 15 | 0 |
| **Total** | **74** | **74** | **0** |

## Notes
- All remaining build warnings are `NU1901`/`NU1903` NuGet vulnerability warnings for transitive dependencies (`AutoMapper` 12.0.1 and `NuGet.Packaging`/`NuGet.Protocol` 6.12.1). These are in production libraries and outside the scope of this test upgrade task.
- The `BlazorInputFile` migration was an unexpected scope expansion required to fix runtime failures in `PublicApiIntegrationTests`.
