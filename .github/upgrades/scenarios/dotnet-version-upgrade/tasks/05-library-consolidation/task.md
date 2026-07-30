# 05-library-consolidation

## Findings
- `Directory.Packages.props` already centralizes the solution target framework at `net10.0`, so the scoped projects inherit .NET 10 without per-project TFM edits.
- `src/ApplicationCore/ApplicationCore.csproj` still referenced `System.Security.Claims` and `System.Text.Json`, which produced `NU1510` because both APIs are provided by the .NET 10 shared framework.
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` still exposed the obsolete formatter-based serialization constructor, triggering `SYSLIB0051` under .NET 10.
- Remaining solution warnings were xUnit analyzer `xUnit2013` violations in unit and integration tests that asserted collection counts with `Assert.Equal`.
- Package review showed no current deprecated packages in `Infrastructure`, but test dependencies and selected library packages were behind current compatible releases in `Directory.Packages.props`.

## Planned changes
1. Remove unnecessary ApplicationCore package references and dead central package versions.
2. Remove the obsolete serialization constructor from `EmptyBasketOnCheckoutException`.
3. Update xUnit assertions to analyzer-compliant collection assertions.
4. Refresh centrally managed package versions used by the scoped library and test projects.
5. Rebuild the full solution and run unit/integration tests until warnings and errors are cleared.
