# 02-upgrade-solution: 02-upgrade-solution

Execute task 02-upgrade-solution.

## Research Findings

- `Directory.Packages.props` centrally declares `TargetFramework` and package versions for all 10 projects; current baseline is `net8.0` with `AspNetVersion=8.0.2`, `EntityFramworkCoreVersion=8.0.2`, `SystemExtensionVersion=8.0.0`, and `VSCodeGeneratorVersion=8.0.0`.
- Baseline build on `eShopOnWeb.sln` succeeded on .NET 8 but produced 13 warnings: 4 package vulnerability warnings (`Azure.Identity` and `System.Text.Json`), 1 obsolete serialization warning (`SYSLIB0051` in `EmptyBasketOnCheckoutException`), and 4 xUnit analyzer warnings repeated across 3 test projects.
- `System.Security.Claims` is declared centrally and referenced only by `src/ApplicationCore/ApplicationCore.csproj`; it can be removed safely because the framework now provides it.
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` is declared centrally and referenced only by `src/PublicApi/PublicApi.csproj`; it is Visual Studio container tooling only and can be removed without affecting build/runtime.
- `CatalogSettings` is bound from the root configuration (`CatalogBaseUrl` is a root-level key in `src/Web/appsettings*.json` and `src/PublicApi/appsettings*.json`), so the .NET 10 fix must preserve root binding rather than switch to a non-existent `CatalogSettings` section.
- `BaseUrlConfiguration` is bound from the `baseUrls` section in `src/Web/appsettings*.json`, `src/PublicApi/appsettings*.json`, and `src/BlazorAdmin/wwwroot/appsettings*.json`; all `Configure<T>`/`Get<T>` usages around that section need the same .NET 10-safe binding pattern.
- `TimeSpan.FromMinutes(ValidityMinutesPeriod)` in `ConfigureCookieSettings` should be changed to the integral overload to avoid .NET 10 obsolescence.
- Existing xUnit analyzer warnings come from collection-size assertions in:
  - `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
  - `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
  - `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
