# Task 04-tests: Upgrade Test Projects to net10.0

## Objective
Upgrade all test projects from net8.0 to net10.0, update deprecated test packages, and fix any compatibility issues introduced by the upgrade.

## Scope
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `Directory.Packages.props` (Central Package Management)
- `src/BlazorShared/Models/CatalogItem.cs` (BlazorInputFile removal)
- `src/BlazorShared/BlazorShared.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/BlazorAdmin/_Imports.razor`

## Changes Made

### Directory.Packages.props
- Upgraded `xunit` from 2.7.0 → 2.9.3
- Upgraded `xunit.runner.visualstudio` from 2.5.6 → 2.8.2
- Upgraded `xunit.runner.console` from 2.7.0 → 2.9.3
- Upgraded `MSTest.TestAdapter` from 3.2.2 → 4.3.2
- Upgraded `MSTest.TestFramework` from 3.2.2 → 4.3.2
- Removed `BlazorInputFile` 0.2.0 (deprecated; incompatible with .NET 10 at runtime)

### Test Project Changes
- `tests/FunctionalTests/FunctionalTests.csproj`: Removed deprecated `DotNetCliToolReference` for `dotnet-xunit` 2.3.1
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: Removed reference to `src/Web/Web.csproj` (caused `Program` type ambiguity since both PublicApi and Web expose a global `Program` class)
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`: Replaced `CatalogIndexViewModel` (from Web.ViewModels) with `ListPagedCatalogItemResponse` (from PublicApi); removed `using Microsoft.eShopWeb.Web.ViewModels`

### BlazorInputFile Migration
- `src/BlazorShared/Models/CatalogItem.cs`: Replaced `IFileListEntry` (from deprecated BlazorInputFile) with `Stream` parameter in `DataToBase64`. BlazorInputFile 0.2.0 has a struct layout incompatibility with .NET 10 that caused runtime failures when `MinimalApi.Endpoint` scanned assemblies during WebApplicationFactory startup.
- `src/BlazorShared/BlazorShared.csproj`: Removed `BlazorInputFile` package reference
- `src/BlazorAdmin/BlazorAdmin.csproj`: Removed `BlazorInputFile` package reference
- `src/BlazorAdmin/_Imports.razor`: Removed `@using BlazorInputFile` import

### xUnit2013 Analyzer Fixes
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`: Changed `Assert.Equal(1, count)` → `Assert.Single(collection)` (2 instances)
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: Changed `Assert.Equal(0, count)` → `Assert.Empty(collection)`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: Changed `Assert.Equal(0, count)` → `Assert.Empty(collection)`

## Target Framework
All test projects inherit `net10.0` via `Directory.Packages.props` `<TargetFramework>net10.0</TargetFramework>` property — no explicit TFM changes needed in individual test project files.

## Results
- Build: 0 errors across all 4 test projects
- Unit tests: 44/44 passed
- Integration tests: 3/3 passed
- Functional tests: 12/12 passed
- PublicApi integration tests: 15/15 passed
- Total: 74/74 tests passed
