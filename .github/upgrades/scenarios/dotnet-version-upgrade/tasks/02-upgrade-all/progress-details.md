# Progress Details

## Scope
Upgraded all eShopOnWeb projects from `net8.0` to `net10.0` through central package management in `Directory.Packages.props`, then resolved build breaks and warnings until the full solution built cleanly.

## Research
- Attempted to use `query_dotnet_assessment` for per-project issue extraction, but the tool returned `StateManager is not initialized` in this session.
- Used `.github/upgrades/scenarios/dotnet-version-upgrade/assessment.md` directly to capture the reported package, API, and behavioral-change findings and recorded them in `task.md` before code changes.

## Configuration and package upgrades
- Updated central TFM to `net10.0`.
- Updated central package version properties:
  - `AspNetVersion` → `10.0.9`
  - `SystemExtensionVersion` → `10.0.9`
  - `EntityFramworkCoreVersion` → `10.0.9`
  - `VSCodeGeneratorVersion` → `10.0.2`
- Patched `Azure.Identity` to `1.21.0`.
- Updated `System.Text.Json` central version to `10.0.9`.
- Updated `System.IdentityModel.Tokens.Jwt` to `8.19.1` to resolve net10 transitive downgrade failures.
- Updated xUnit packages:
  - `xunit` → `2.9.3`
  - `xunit.runner.visualstudio` → `3.1.5`
  - `xunit.runner.console` → `2.9.3`
- Replaced deprecated/vulnerable `AutoMapper.Extensions.Microsoft.DependencyInjection` central usage with `AutoMapper` `16.2.0`.

## Package removals and project cleanup
- Removed central package entries for:
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
  - `System.Security.Claims`
  - `Microsoft.AspNetCore.Mvc`
- Removed direct `System.Security.Claims` reference from `src/ApplicationCore/ApplicationCore.csproj`.
- Removed direct `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference from `src/PublicApi/PublicApi.csproj`.
- Removed direct `System.Text.Json` reference from `src/ApplicationCore/ApplicationCore.csproj` because the framework now provides it without warnings.
- Removed direct `System.Net.Http.Json` reference from `src/BlazorAdmin/BlazorAdmin.csproj` because the framework now provides it without warnings.
- Removed `Microsoft.VisualStudio.Web.CodeGeneration.Design` references from `src/PublicApi/PublicApi.csproj` and `src/Web/Web.csproj` to eliminate vulnerable transitive dependencies while leaving the central version property upgraded.
- Removed the unused `AutoMapper.Extensions.Microsoft.DependencyInjection` reference from `src/Web/Web.csproj`.
- Replaced the `AutoMapper.Extensions.Microsoft.DependencyInjection` reference in `src/PublicApi/PublicApi.csproj` with `AutoMapper`.

## Code fixes
- Updated `src/PublicApi/Program.cs` to the current AutoMapper DI registration API:
  - `AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly)`
- Added `src/PublicApi/PublicApiAssemblyMarker.cs` and switched `tests/PublicApiIntegrationTests/ProgramTest.cs` to `WebApplicationFactory<PublicApiAssemblyMarker>` to avoid the `Program` type ambiguity introduced by two top-level-host applications.
- Removed the obsolete serialization constructor from `EmptyBasketOnCheckoutException` to clear `SYSLIB0051` on net10.
- Replaced collection-count assertions flagged by xUnit analyzers with `Assert.Single`/`Assert.Empty` where required.

## Validation
- Baseline build before changes: succeeded with 13 warnings and 0 errors.
- Final build after changes: `dotnet build eShopOnWeb.sln --nologo` succeeded with **0 warnings** and **0 errors**.

## Files changed
- `Directory.Packages.props`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/PublicApi/Program.cs`
- `src/PublicApi/PublicApi.csproj`
- `src/PublicApi/PublicApiAssemblyMarker.cs`
- `src/Web/Web.csproj`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
- `tests/PublicApiIntegrationTests/ProgramTest.cs`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02-upgrade-all/task.md`
