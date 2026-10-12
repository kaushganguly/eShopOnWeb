# Assessment — eShopOnWeb .NET 8.0 → .NET 10.0

> Note: The automated `generate_dotnet_upgrade_assessment` tool was unavailable in this session
> (orchestrator channel connectivity error, confirmed after repeated retries across different
> input modes). This assessment was produced via direct inspection of the repository
> (csproj files, `Directory.Packages.props`, `global.json`, `dotnet list package --vulnerable`,
> and NuGet.org metadata) to avoid blocking the upgrade.

## Scope

10 projects, all SDK-style, all centrally managed (`ManagePackageVersionsCentrally=true`),
single shared `TargetFramework=net8.0` declared once in `Directory.Packages.props` and
inherited by every project (no per-project `<TargetFramework>` overrides, no `Directory.Build.props`).

| Project | Type | Path |
|---|---|---|
| BlazorShared | Razor class library | src/BlazorShared |
| ApplicationCore | Class library | src/ApplicationCore |
| Infrastructure | Class library | src/Infrastructure |
| BlazorAdmin | Blazor WASM | src/BlazorAdmin |
| Web | ASP.NET Core MVC/Razor Pages | src/Web |
| PublicApi | ASP.NET Core minimal/API endpoints | src/PublicApi |
| UnitTests | xunit test project | tests/UnitTests |
| IntegrationTests | xunit test project | tests/IntegrationTests |
| FunctionalTests | xunit test project | tests/FunctionalTests |
| PublicApiIntegrationTests | MSTest test project | tests/PublicApiIntegrationTests |

Dependency graph (topological order, leaf → root):
BlazorShared → ApplicationCore → Infrastructure → BlazorAdmin → Web → PublicApi →
UnitTests → PublicApiIntegrationTests → FunctionalTests → IntegrationTests

## Complexity Classification: Simple

All projects target `net8.0` (modern, ≥ net5.0), all SDK-style, no .NET Framework projects,
shallow dependency graph, no identified risky package migrations (no OData, no
Newtonsoft→STJ, no old-style csproj). → **All-At-Once strategy**, single task.

## Known Vulnerabilities (pre-upgrade, `dotnet list package --vulnerable --include-transitive`)

| Package | Version | Severity | Advisory |
|---|---|---|---|
| AutoMapper (transitive via AutoMapper.Extensions.Microsoft.DependencyInjection) | 12.0.1 | High | GHSA-rvv3-g6hj-g44x |
| Microsoft.Extensions.Caching.Memory | 8.0.0 | High | GHSA-qj66-m88j-hmgj |
| Microsoft.Identity.Client | 4.56.0 | Low/Moderate | GHSA-x674-v45j-fwxw, GHSA-m5vv-6r4h-3vj9 |
| NuGet.Common / NuGet.Packaging / NuGet.Protocol (transitive via CodeGeneration.Design) | 6.3.1 | High/Critical | GHSA-6qmf-mmc7-6c2p, GHSA-68w7-72jg-6qpp |
| System.Formats.Asn1 | 5.0.0 | High | GHSA-447r-wph3-92pm |
| System.Text.Json | 8.0.3 | High | GHSA-hh2w-p6rv-4g7w, GHSA-8g4q-xg66-9fp4 |
| Azure.Identity | 1.10.3/1.10.4 | Moderate | GHSA-wvxc-855f-jvrv, GHSA-m5vv-6r4h-3vj9 |

All are addressed by bumping System.Text.Json, Azure.Identity, Microsoft.EntityFrameworkCore.*,
Microsoft.VisualStudio.Web.CodeGeneration.Design and replacing
`AutoMapper.Extensions.Microsoft.DependencyInjection` with the (patched) core `AutoMapper`
package (see plan.md).

## Licensing Note (important — must not regress to a commercial dependency)

Two packages in use (both from the same author, "LuckyPenny Software") converted to
commercial/dual licensing partway through their major-version history:

- **MediatR**: free (Apache-2.0) through `12.5.0`; `13.0.0+` requires a paid license key.
  → Pin to `12.5.0` (latest free release), do **not** take `13+`/`14+`.
- **AutoMapper**: free (MIT) through `14.0.0`; `15.0.0+` requires a paid license key.
  → Pin to `14.0.0` (latest free release), do **not** take `15+`/`16+`.

## Target Package Versions (net10.0)

Resolved via `get_supported_package_version` (MCP tool) and cross-checked against NuGet.org
where the tool returned "no supported version" or where a license-tier concern applied:

| Package | Current | Target |
|---|---|---|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 |
| Ardalis.Specification(.EntityFrameworkCore) | 7.0.0 | 9.3.1 |
| Ardalis.Result | 7.0.0 | 10.1.0 |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | **removed** — replaced by `AutoMapper` 14.0.0 |
| BlazorInputFile | 0.2.0 | 0.2.0 |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 |
| FluentValidation | 11.9.0 | 12.2.0 |
| MediatR | 12.0.1 | 12.5.0 (license pin, see above) |
| Microsoft.AspNetCore.Mvc (orphan, unreferenced) | 2.2.0 | **removed** |
| Microsoft.AspNetCore.* (AspNetVersion) | 8.0.2 | 10.0.12 |
| Microsoft.Extensions.* (SystemExtensionVersion) | 8.0.0 | 10.0.12 |
| Microsoft.EntityFrameworkCore.* (EntityFramworkCoreVersion) | 8.0.2 | 10.0.12 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design (VSCodeGeneratorVersion) | 8.0.0 | 10.0.2 |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.23.0 |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 |
| NSubstitute | 5.1.0 | 6.2.0 |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 |
| System.Net.Http.Json | 8.0.0 | 10.0.12 |
| System.Security.Claims | 4.3.0 | 4.3.0 |
| System.Text.Json | 8.0.3 | 10.0.12 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 |
| Swashbuckle.AspNetCore(.SwaggerUI/.Annotations) | 6.5.0 | 10.3.0 |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | 10.0.12 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.10.1 |
| xunit | 2.7.0 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.6 | 4.0.1 |
| xunit.runner.console | 2.7.0 | 2.9.3 |
| MSTest.TestAdapter / MSTest.TestFramework | 3.2.2 | 4.5.1 |
| coverlet.collector | 6.0.2 | 10.1.0 |

## Other Risks Identified

- `global.json` pins SDK `8.0.x` → must move to `10.0.x`.
- `src/Web/Dockerfile` and `src/PublicApi/Dockerfile` reference `mcr.microsoft.com/dotnet/sdk:8.0`
  and `mcr.microsoft.com/dotnet/aspnet:8.0` base images → must bump to `10.0`.
- No old-style csproj, no `packages.config` (except the unrelated `docker-compose.dcproj`, which
  is not a .NET project and is out of scope).
- `AddAutoMapper(...)` is called in `src/PublicApi/Program.cs` — confirmed compatible with
  calling it from the core `AutoMapper` package directly after removing the DI extension package.

## Checklist

- [x] Analysis complete (manual, due to tool unavailability)
- [x] assessment.md created
- [x] Security vulnerabilities documented — included in upgrade by default
- [x] Assessment summary surfaced (Automatic mode — proceeding directly to planning/execution)
