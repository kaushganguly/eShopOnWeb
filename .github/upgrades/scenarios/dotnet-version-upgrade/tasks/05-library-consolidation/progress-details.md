# 05-library-consolidation progress details

## Completed work
- Confirmed the scoped libraries and tests already inherit `net10.0` from `Directory.Packages.props`.
- Removed `System.Security.Claims` and `System.Text.Json` from `src/ApplicationCore/ApplicationCore.csproj`, and removed the now-unused central package version entries.
- Removed the obsolete formatter-based serialization constructor from `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` to eliminate `SYSLIB0051`.
- Updated central package versions for `Ardalis.GuardClauses`, `Ardalis.Result`, `Microsoft.NET.Test.Sdk`, `NSubstitute`, `xunit`, `xunit.runner.visualstudio`, and `xunit.runner.console`.
- Reworked test assertions in unit and integration tests to satisfy `xUnit2013` guidance.

## Validation
- `dotnet build eShopOnWeb.sln -nologo` -> succeeded with 0 warnings and 0 errors.
- `dotnet test tests/UnitTests/UnitTests.csproj --no-restore --no-build -nologo -m:1` -> passed (44/44).
- `dotnet test tests/IntegrationTests/IntegrationTests.csproj --no-restore --no-build -nologo -m:1` -> passed (3/3).

## Notes
- `dotnet test` commands launched in parallel initially collided in `src/BlazorAdmin/obj/Debug/net10.0/blazor.build.boot-extension.json`; rerunning the requested test projects sequentially resolved the transient file lock without additional code changes.
- `src/Infrastructure/Infrastructure.csproj`, `tests/IntegrationTests/IntegrationTests.csproj`, and `tests/UnitTests/UnitTests.csproj` required no per-project TFM edits because they already inherit `net10.0` centrally.
