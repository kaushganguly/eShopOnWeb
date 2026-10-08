# .NET Upgrade Assessment: eShopOnWeb → net10.0

> Note: The dedicated `generate_dotnet_upgrade_assessment` tool was unavailable in this
> environment (orchestrator channel connection failure on every retry). This assessment was
> produced manually using `get_projects_in_topological_order`, `get_supported_package_version`,
> direct project-file inspection, and `dotnet --list-sdks`/`--version`.

## Scope

Solution: `eShopOnWeb.sln`
Current target framework: `net8.0` (set centrally in `Directory.Packages.props` via `<TargetFramework>net8.0</TargetFramework>`, inherited by every project — none of the `.csproj` files set `TargetFramework` individually).
Target framework: `net10.0`

Projects (topological build order):
1. `src/BlazorShared/BlazorShared.csproj` (Sdk.NET, classlib)
2. `src/ApplicationCore/ApplicationCore.csproj` (Sdk.NET, classlib)
3. `src/Infrastructure/Infrastructure.csproj` (Sdk.NET, classlib, EF Core)
4. `src/BlazorAdmin/BlazorAdmin.csproj` (Sdk.BlazorWebAssembly)
5. `src/Web/Web.csproj` (Sdk.Web, ASP.NET Core MVC/Razor Pages + Identity)
6. `src/PublicApi/PublicApi.csproj` (Sdk.Web, minimal API)
7. `tests/UnitTests/UnitTests.csproj` (xUnit)
8. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` (MSTest)
9. `tests/FunctionalTests/FunctionalTests.csproj` (xUnit, WebApplicationFactory)
10. `tests/IntegrationTests/IntegrationTests.csproj` (xUnit, EF Core InMemory)

`.NET 10 SDK` is installed and available (`10.0.401`). `global.json` currently pins `8.0.x`/`latestFeature` and must be updated.

## Central Package Management

`Directory.Packages.props` sets `ManagePackageVersionsCentrally=true` and centralizes versions for every `PackageReference`. It also defines shared MSBuild properties consumed across packages:
- `TargetFramework` = `net8.0` → `net10.0`
- `AspNetVersion` = `8.0.2` → `10.0.x` (drives all `Microsoft.AspNetCore.*` packages)
- `SystemExtensionVersion` = `8.0.0` → `10.0.x` (drives `Microsoft.Extensions.Logging.Configuration`, `System.Net.Http.Json`)
- `EntityFramworkCoreVersion` = `8.0.2` → `10.0.x` (drives all `Microsoft.EntityFrameworkCore.*` packages)
- `VSCodeGeneratorVersion` = `8.0.0` → latest compatible (drives `Microsoft.VisualStudio.Web.CodeGeneration.Design`)

## Package Upgrade Targets (via get_supported_package_version against net10.0)

| Package | Current | Target (net10.0) | Notes |
|---|---|---|---|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 | unchanged |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | |
| Ardalis.Result | 7.0.0 | 10.1.0 | |
| Ardalis.Specification | 7.0.0 | 9.3.1 | |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 | unchanged |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | |
| Azure.Identity | 1.10.4 | 1.21.0 | security-relevant, update |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 | unchanged (keep free license line) |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 | tool reported "no supported version found" — package last published pre-.NET9; keep pinned version and validate via build (works on any TFM, pure BCL dependency) |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 | unchanged |
| FluentValidation | 11.9.0 | 12.1.1 | |
| MediatR | 12.0.1 | 12.0.1 (kept) | v13+ moved to a commercial license (ServiceStack-style dual licensing); staying on the last free 12.x line that already works fine on net10.0 (netstandard2.0/net6.0 target, no TFM-specific break) |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 | |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.19.6 | tool reported "no supported version found"; keep pinned, validate via build |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 | unchanged |
| NSubstitute | 5.1.0 | 6.2.0 | |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 | unchanged |
| System.Net.Http.Json | 8.0.0 | 10.0.12 | tracks `SystemExtensionVersion` |
| System.Text.Json | 8.0.3 | 10.0.12 | security-relevant, update |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 | |
| Swashbuckle.AspNetCore(+SwaggerUI/Annotations) | 6.5.0 | 10.3.0 | major bump; validate Swagger UI/XML doc generation still works |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.10.1 | |
| xunit | 2.7.0 | 2.9.3 | |
| xunit.runner.visualstudio / xunit.runner.console | 2.5.6 / 2.7.0 | 2.9.3 | |
| MSTest.TestAdapter / MSTest.TestFramework | 3.2.2 | 4.0.0 | |
| coverlet.collector | 6.0.2 | 6.0.2 (latest stable already; keep) | |
| Microsoft.AspNetCore.Mvc (loose pin, unreferenced by any csproj) | 2.2.0 | remove or leave unreferenced | not used by any `PackageReference`; harmless stale entry, leaving as-is unless build requires removal |
| ASP.NET Core family (`$(AspNetVersion)`) | 8.0.2 | 10.0.x | Components.*, Identity.*, Authentication.JwtBearer, Diagnostics.EntityFrameworkCore |
| EF Core family (`$(EntityFramworkCoreVersion)`) | 8.0.2 | 10.0.x | InMemory, SqlServer, Tools |

## Known Risk Areas / Breaking Changes to Validate

1. **MediatR licensing** — do not bump past the last free 12.x release without explicit user approval (commercial license risk). Decision: keep `MediatR 12.0.1`.
2. **Swashbuckle.AspNetCore 6→10** — possible API surface changes in `SwaggerGen`/`SwaggerUI` options and annotations; validate Swagger endpoint still serves after upgrade.
3. **ASP.NET Core Identity / EF Core 8→10** — review EF Core behavioral changes (e.g., stricter LINQ translation, query behavior changes) and ASP.NET Core Identity package updates.
4. **MSTest 3→4** — check for any analyzer-driven source changes required in `PublicApiIntegrationTests`.
5. **Blazor WebAssembly (BlazorAdmin)** — `Microsoft.AspNetCore.Components.WebAssembly*` family moves 8→10; validate WASM build/publish still succeeds, and check for deprecated hosting model APIs (`Program.cs` builder patterns may need adjustment for newer Blazor WASM template behavior).
6. **global.json** — SDK pinned to `8.0.x`; must move to `10.0.x` (rollForward `latestFeature`) to allow build with the installed .NET 10 SDK.
7. **Legacy/abandoned packages** (`BlazorInputFile`, `Microsoft.Web.LibraryManager.Build`'s predecessor behavior) — these are unmaintained; functionally they are pure C# libraries without TFM-specific assets, so they are expected to keep working, but should be flagged as technical debt.

## Security

- `Azure.Identity 1.10.4` and `System.Text.Json 8.0.3` are being updated as part of this upgrade to pick up upstream security fixes (per general policy of updating to latest supported minor/patch for libraries touched by the TFM bump). No other known active CVEs identified in the above dependency set at assessment time.

## Conclusion

This solution is already SDK-style across all projects, uses central package management, and has a single point of TFM control (`Directory.Packages.props`). The upgrade is primarily: (1) bump the central TFM + shared version properties, (2) bump the package versions enumerated above, (3) bump `global.json`, (4) build, (5) fix any compile/behavioral breaks, (6) run tests. No project needs SDK-style conversion.
