# 03-final-validation: Run full test suite

Execute the complete test suite to verify that the upgraded solution is functionally correct after the .NET 8 → .NET 10 upgrade.

## Objective

All existing tests should pass with no regressions. Any failures must be either fixed (if caused by the upgrade) or documented (if pre-existing or environmental).

## Test Projects

| Project | Path |
|---------|------|
| UnitTests | tests/UnitTests/UnitTests.csproj |
| IntegrationTests | tests/IntegrationTests/IntegrationTests.csproj |
| FunctionalTests | tests/FunctionalTests/FunctionalTests.csproj |
| PublicApiIntegrationTests | tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj |

## Test Scope Notes

- **UnitTests**: Pure unit tests covering domain entities and services. Expected to pass in any environment.
- **IntegrationTests**: Tests that exercise the data access layer with an in-memory or SQLite database. May use EF Core in-memory provider.
- **FunctionalTests**: End-to-end tests against the web application using WebApplicationFactory. May require SQL Server or other external services.
- **PublicApiIntegrationTests**: Integration tests for the public API surface. May also require external services.

## Environmental Considerations

- Tests requiring SQL Server (SqlException / connection refused) are classified as **environmental failures**, not upgrade regressions.
- The build already succeeds with 0 errors and 0 warnings. Using `--no-build` for test run.

## Done When

All test projects build and all tests pass, or pre-existing/environmental failures are documented.
