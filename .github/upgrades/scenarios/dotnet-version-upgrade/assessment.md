# Assessment — Upgrade eShopOnWeb to .NET 10.0

> **Note**: The automated `generate_dotnet_upgrade_assessment` tool was unavailable in this
> environment (orchestrator channel connection errors on every retry). This assessment was
> produced manually using `get_project_dependencies`, `get_supported_package_version`,
> `get_projects_in_topological_order`, direct inspection of project/`Directory.Packages.props`
> files, and NuGet registry lookups for packages the tool could not resolve.

## Scope

Solution: `/home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`
Current target framework: `net8.0` (centrally defined once, in `Directory.Packages.props`,
and inherited by every project — no project overrides `TargetFramework`)
Target framework: `net10.0` (LTS, EOL 2028-11-14)
Current SDK pin: `global.json` → `8.0.x` (`rollForward: latestFeature`)
~11,000 lines of C# across 10 projects.

## Project Inventory & Dependency Order

(topological order from `get_projects_in_topological_order`)

| # | Project | Type | Notes |
|---|---------|------|-------|
| 1 | `src/BlazorShared/BlazorShared.csproj` | Razor Class Library (`Sdk="Microsoft.NET.Sdk"`) | Shared DTOs/validators used by Blazor admin + ApplicationCore |
| 2 | `src/ApplicationCore/ApplicationCore.csproj` | Class library | Domain/business logic |
| 3 | `src/Infrastructure/Infrastructure.csproj` | Class library | EF Core, Identity |
| 4 | `src/BlazorAdmin/BlazorAdmin.csproj` | Blazor WebAssembly (`Sdk="Microsoft.NET.Sdk.BlazorWebAssembly"`) | Admin SPA |
| 5 | `src/Web/Web.csproj` | ASP.NET Core MVC/Razor Pages (`Sdk="Microsoft.NET.Sdk.Web"`) | Main web app, hosts BlazorAdmin WASM |
| 6 | `src/PublicApi/PublicApi.csproj` | ASP.NET Core minimal/API endpoints (`Sdk="Microsoft.NET.Sdk.Web"`) | Public API |
| 7 | `tests/UnitTests/UnitTests.csproj` | xUnit | References ApplicationCore + Web |
| 8 | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | MSTest + `WebApplicationFactory` | References PublicApi + Web |
| 9 | `tests/FunctionalTests/FunctionalTests.csproj` | xUnit + `WebApplicationFactory` | References ApplicationCore, PublicApi, Web |
| 10 | `tests/IntegrationTests/IntegrationTests.csproj` | xUnit | References Infrastructure + UnitTests |

All 10 projects are SDK-style already (no legacy-format conversion needed). TFM is **not**
set per-project — it is defined once as `<TargetFramework>net8.0</TargetFramework>` in
`Directory.Packages.props`, which every project implicitly imports (CPM entrypoint). This
means the TFM bump is a **single-line, solution-wide change** rather than 10 separate edits —
but it must be verified that this unusual pattern continues to resolve correctly for the
Blazor WebAssembly project's special SDK targets after the version bump.

## Central Package Management

`ManagePackageVersionsCentrally=true` via `Directory.Packages.props`. Version "pivot"
properties used throughout:
- `AspNetVersion` = `8.0.2` → bump to `10.0.12`
- `SystemExtensionVersion` = `8.0.0` → bump to `10.0.2` (verified via `Microsoft.Extensions.Logging.Configuration` / `System.Net.Http.Json` resolution for net10.0)
- `EntityFramworkCoreVersion` = `8.0.2` → bump to `10.0.2` (used by EFCore.InMemory/Tools); **note**: `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.AspNetCore.Identity.EntityFrameworkCore` resolve to `10.0.12`, one minor line ahead of the `10.0.2` EFCore.Tools/InMemory resolution — see Risk below.
- `VSCodeGeneratorVersion` = `8.0.0` → bump to `10.0.2`

## Package Upgrade Table (current → net10.0-supported)

| Package | Current | → Target | Source |
|---|---|---|---|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 (unchanged) | tool |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | tool |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | tool |
| Ardalis.Result | 7.0.0 | 10.1.0 | tool |
| Ardalis.Specification | 7.0.0 | 9.3.1 | tool |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 (unchanged) | tool |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | tool |
| Azure.Identity | 1.10.4 | 1.21.0 | tool |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 (unchanged) | tool |
| BlazorInputFile | 0.2.0 | 0.2.0 (only version on NuGet; `.NETStandard2.0`/`2.1` package, abandoned since 2019) | tool + nuspec |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 (already latest on NuGet; dependency groups only declared through net8.0 — NuGet nearest-TFM fallback applies for net10.0, verify at restore) | tool returned "no supported version"; NuGet registry confirms 4.5.0 is latest |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 (unchanged; build-time only tool) | tool |
| FluentValidation | 11.9.0 | 12.1.1 | tool |
| MediatR | 12.0.1 | 14.2.0 | tool |
| Microsoft.AspNetCore.Components.* (all, via `$(AspNetVersion)`) | 8.0.2 | 10.0.12 | tool |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.2 | 10.0.12 | tool |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.2 | 10.0.12 | tool |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.2 | 10.0.12 | tool |
| Microsoft.AspNetCore.Identity.UI | 8.0.2 | 10.0.12 | tool |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | 10.0.12 | tool |
| Microsoft.Extensions.Identity.Core | 8.0.2 | 10.0.12 | tool |
| Microsoft.Extensions.Logging.Configuration (via `$(SystemExtensionVersion)`) | 8.0.0 | 10.0.2 | tool |
| Microsoft.VisualStudio.Web.CodeGeneration.Design (via `$(VSCodeGeneratorVersion)`) | 8.0.0 | 10.0.2 | tool |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | tool |
| Microsoft.EntityFrameworkCore.InMemory (via `$(EntityFramworkCoreVersion)`) | 8.0.2 | 10.0.2 | tool |
| Microsoft.EntityFrameworkCore.SqlServer (via `$(EntityFramworkCoreVersion)`) | 8.0.2 | 10.0.12 | tool |
| Microsoft.EntityFrameworkCore.Tools (via `$(EntityFramworkCoreVersion)`) | 8.0.2 | 10.0.2 | tool |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.21.0 (latest stable; tool reported "no supported version" because this is a `.NETFramework4.7.2`-only VS design-time tooling package with no runtime TFM dependency group — safe, build-time only) | tool "no supported" + NuGet registry |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 (unchanged) | tool |
| NSubstitute | 5.1.0 | 6.2.0 | tool |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 (unchanged) | tool |
| System.Net.Http.Json (via `$(SystemExtensionVersion)`) | 8.0.0 | 10.0.2 | tool |
| System.Security.Claims | 4.3.0 | 4.3.0 (unchanged — legacy compat shim, still resolvable) | tool |
| System.Text.Json | 8.0.3 | 10.0.2 | tool |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 | tool |
| Swashbuckle.AspNetCore | 6.5.0 | 10.3.0 | tool |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.3.0 | tool |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.3.0 | tool |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.10.1 | tool |
| xunit | 2.7.0 | 2.9.3 | tool |
| xunit.runner.visualstudio | 2.5.6 | 2.9.3 | tool |
| xunit.runner.console | 2.7.0 | 2.9.3 | tool |
| MSTest.TestAdapter | 3.2.2 | 4.5.1 | tool |
| MSTest.TestFramework | 3.2.2 | 4.5.1 | tool |
| coverlet.collector | 6.0.2 | 10.1.0 | tool |

## Security Vulnerabilities

No CVEs were surfaced by the available tooling in this environment (the dedicated
vulnerability-aware assessment tool was unavailable). As a general compliance measure for
`securityComplianceCheck=true`, every package above is being moved to its latest version
supported on `net10.0`, which resolves any vulnerabilities that would have existed in the
.NET 8-era pinned versions (notably older `System.IdentityModel.Tokens.Jwt`,
`Azure.Identity`, and `System.Text.Json` releases have known historical CVEs fixed in
later versions — all superseded by the versions above).

## Risks / Things to Watch During Execution

1. **TFM defined in `Directory.Packages.props` rather than per-project** — unusual pattern.
   Must confirm the Blazor WebAssembly SDK (`Microsoft.NET.Sdk.BlazorWebAssembly`) and
   `Microsoft.NET.Sdk.Web` both still correctly resolve `TargetFramework` from this import
   after the bump (no reason it shouldn't, but it's atypical so needs a build check for
   every project, not just the ones that "look different").
2. **EF Core version skew**: `EntityFramworkCoreVersion` resolves `InMemory`/`Tools` to
   `10.0.2` while `SqlServer` and `Identity.EntityFrameworkCore` independently resolve to
   `10.0.12` (their own supported-version lookups, likely because they ship on the
   ASP.NET Core versioning train rather than the EF Core runtime train). Need to pick one
   consistent, mutually-compatible version set per package (not all forced through a single
   property) to avoid assembly version mismatches — update per-package rather than
   assuming one shared property value works for all four.
3. **`Blazored.LocalStorage`** has no net9/net10-specific dependency group (max declared is
   net8.0) — expected to resolve fine via NuGet's nearest-compatible-TFM fallback, but
   restore/build must be checked explicitly for this package.
4. **`BlazorInputFile`** is an abandoned (2019) package pinned to `.NETStandard2.0`/`2.1,`
   depending on preview-era `Microsoft.AspNetCore.Components` 3.1 preview packages. No newer
   version exists. Must verify it still functions under net10.0 Blazor WebAssembly hosting;
   if runtime issues appear, this is a candidate for replacement with built-in
   `InputFile`/`IBrowserFile` (native since .NET 5) — flag for manual judgment call during
   execution, not blocking the assessment.
5. **`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`** and
   **`Microsoft.VisualStudio.Web.CodeGeneration.Design`** are VS tooling/design-time
   packages, not runtime dependencies — upgrading them is low risk but unrelated to the
   app's actual net10.0 runtime behavior.
6. **`WebApplicationFactory`-based integration/functional tests** (`FunctionalTests`,
   `PublicApiIntegrationTests`) are sensitive to ASP.NET Core minimal hosting and test host
   changes between major versions — expect to re-validate these after the bump.
7. **`global.json`** pins SDK to `8.0.x`; must be updated to a `10.0.x` band consistent with
   the installed SDK (`10.0.401` confirmed installed in this environment) so `dotnet build`
   targets the correct toolchain.
8. **ASP.NET Core / EF Core 9 and 10 behavioral/breaking changes** (e.g., stricter nullability
   handling in model binding, Identity cookie/auth defaults, EF Core query translation
   changes, minimal API routing tweaks) will need to be checked against compiler/build/test
   output rather than assumed — to be handled reactively during the Execution stage based on
   actual build/test failures.

## Environment

- .NET SDKs installed: 8.0.131/8.0.206/8.0.319/8.0.425, 9.0.121/9.0.205/9.0.318,
  **10.0.112/10.0.204/10.0.303/10.0.401** — compatible SDK for net10.0 confirmed via
  `validate_dotnet_sdk_installation`.
- Git: clean working tree; working branch `upgrade/net10` created off `main` before
  initializing this scenario.

## Checklist

- [x] Analysis complete (manual, tool-assisted)
- [x] assessment.md created
- [x] No CVE data available from tooling; package versions bumped to latest-supported as the
      compliance baseline (documented above)
- [x] Assessment summary to be presented to user; **Automatic mode** — proceeding directly to
      planning without waiting for approval
