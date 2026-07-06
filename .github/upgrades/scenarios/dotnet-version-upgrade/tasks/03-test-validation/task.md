# 03-test-validation: Test Validation

## Objective

Run the full test suite to confirm functional correctness after the .NET 8 → .NET 10 upgrade.

## Scope

| Project | Path | Framework | Test Runner |
|---|---|---|---|
| UnitTests | tests/UnitTests/UnitTests.csproj | net10.0 | xunit |
| IntegrationTests | tests/IntegrationTests/IntegrationTests.csproj | net10.0 | xunit |
| FunctionalTests | tests/FunctionalTests/FunctionalTests.csproj | net10.0 | xunit + Microsoft.AspNetCore.Mvc.Testing |
| PublicApiIntegrationTests | tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj | net10.0 | MSTest + Microsoft.AspNetCore.Mvc.Testing |

## Test Structure

### UnitTests (44 tests)
- `ApplicationCore/Entities` — domain entity tests
- `ApplicationCore/Extensions` — extension method tests
- `ApplicationCore/Services` — application service unit tests
- `ApplicationCore/Specifications` — specification pattern tests
- `Builders` — test builder helpers
- `MediatorHandlers` — MediatR handler unit tests
- `Web` — web layer unit tests
- Dependencies: ApplicationCore.csproj, Web.csproj, NSubstitute (mocking)

### IntegrationTests (3 tests)
- `Repositories/BasketRepositoryTests` — basket repository against in-memory EF Core
- `Repositories/OrderRepositoryTests` — order repository against in-memory EF Core
- Dependencies: Infrastructure.csproj, UnitTests.csproj, Microsoft.EntityFrameworkCore.InMemory

### FunctionalTests (12 tests)
- `Web/` — ASP.NET Core MVC functional tests via WebApplicationFactory
- `PublicApi/` — Public API functional tests via WebApplicationFactory
- Uses in-memory EF Core; no external database required
- Dependencies: ApplicationCore.csproj, PublicApi.csproj, Web.csproj

### PublicApiIntegrationTests (15 tests)
- `AuthEndpoints/` — authentication API endpoint tests
- `CatalogItemEndpoints/` — catalog CRUD endpoint tests
- Uses MSTest + WebApplicationFactory with appsettings.test.json
- Dependencies: PublicApi.csproj, Web.csproj

## Findings

All test projects target `net10.0` and build cleanly. No test failures were introduced by the .NET 8 → .NET 10 upgrade. All 74 tests pass across all four projects.

## Done Criteria

- [x] `dotnet test tests/UnitTests/UnitTests.csproj` passes with 0 failures (44/44)
- [x] `dotnet test tests/IntegrationTests/IntegrationTests.csproj` passes with 0 failures (3/3)
- [x] `dotnet test tests/FunctionalTests/FunctionalTests.csproj` passes with 0 failures (12/12)
- [x] `dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` passes with 0 failures (15/15)

