# Assessment: eShopOnWeb .NET 8.0 → .NET 10.0 Upgrade

> **Note on tooling**: The dedicated `generate_dotnet_upgrade_assessment` MCP tool was unavailable
> during this session (`Orchestrator channel was not available ... Extender is not connected to an
> orchestrator peer`, confirmed after multiple retries with increasing backoff). This assessment
> was produced manually using `get_project_dependencies`, `get_supported_package_version`,
> `get_projects_in_topological_order`, and direct repository inspection, per the scenario's
> fallback guidance ("generic Assessor is the fallback if the dedicated one is unavailable").

## Solution Overview

- Solution file: `eShopOnWeb.sln` (also `Everything.sln` includes the same projects + docker-compose)
- Current target framework: **net8.0**, centrally defined in `Directory.Packages.props`
  (`<TargetFramework>net8.0</TargetFramework>` — implicitly imported into every project since
  `ManagePackageVersionsCentrally` is enabled, acting like a shared `Directory.Build.props`).
- Target framework: **net10.0** (LTS, confirmed via `get_dotnet_upgrade_options`; alternatives were
  net9.0 STS and net11.0 Preview — net10.0 selected per explicit user request).
- Package versions are centrally managed via `Directory.Packages.props` (CPM) — all project files use
  version-less `<PackageReference Include="..." />`.
- `global.json` pins SDK to `8.0.x` with `rollForward: latestFeature` — must be updated to `10.0.x`.

### Projects (in topological build order)

| # | Project | SDK | Role |
|---|---------|-----|------|
| 1 | `src/BlazorShared/BlazorShared.csproj` | `Microsoft.NET.Sdk` | Shared Blazor DTOs/config |
| 2 | `src/ApplicationCore/ApplicationCore.csproj` | `Microsoft.NET.Sdk` | Domain/business logic |
| 3 | `src/Infrastructure/Infrastructure.csproj` | `Microsoft.NET.Sdk` | EF Core, persistence |
| 4 | `src/BlazorAdmin/BlazorAdmin.csproj` | `Microsoft.NET.Sdk.BlazorWebAssembly` | Admin WASM app |
| 5 | `src/Web/Web.csproj` | `Microsoft.NET.Sdk.Web` | Main MVC/Razor web app, hosts BlazorAdmin |
| 6 | `src/PublicApi/PublicApi.csproj` | `Microsoft.NET.Sdk.Web` | Public minimal/REST API |
| 7 | `tests/UnitTests/UnitTests.csproj` | `Microsoft.NET.Sdk` | xUnit unit tests |
| 8 | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | `Microsoft.NET.Sdk` | MSTest integration tests for PublicApi |
| 9 | `tests/FunctionalTests/FunctionalTests.csproj` | `Microsoft.NET.Sdk` | xUnit functional/WebApplicationFactory tests |
| 10 | `tests/IntegrationTests/IntegrationTests.csproj` | `Microsoft.NET.Sdk` | xUnit EF Core integration tests |

All 10 projects are already **SDK-style** — no legacy-format conversion needed.

## Package Analysis — Current vs. Actually Applied (net10.0) Versions

> **Correction note**: the table below was originally populated via
> `get_supported_package_version(..., targetFrameworks="net10.0")`. That tool was later found
> to return multiple fabricated/non-existent versions (confirmed when `dotnet restore` failed
> with NU1102 "package/version not found" for several of them). **All versions below are the
> final, actually-applied values, each independently re-verified against the live NuGet v3
> flat-container API** (`https://api.nuget.org/v3-flatcontainer/{id}/index.json`) before being
> written to `Directory.Packages.props`. Where the tool's original (wrong) suggestion differed
> meaningfully, it is noted.

| Package | Pre-upgrade (net8.0) | **Applied (net10.0)** | Notes |
|---|---|---|---|
| Microsoft.AspNetCore.* (via `$(AspNetVersion)`) | 8.0.2 | **10.0.12** | Components.Authorization, Identity.EntityFrameworkCore, Authentication.JwtBearer, Diagnostics.EntityFrameworkCore, Identity.UI, Mvc.Testing — all via the shared `AspNetVersion` MSBuild property |
| Microsoft.EntityFrameworkCore.* (via `$(EntityFramworkCoreVersion)`) | 8.0.2 | **10.0.12** | InMemory, SqlServer, Tools |
| System.Net.Http.Json / Microsoft.Extensions.Logging.Configuration (via `$(SystemExtensionVersion)`) | 8.0.0 | **10.0.12** | `System.Net.Http.Json` explicit reference later **removed** (BCL-shipped, NU1510) |
| Microsoft.VisualStudio.Web.CodeGeneration.Design (via `$(VSCodeGeneratorVersion)`) | 8.0.0 | **10.0.2** | tool originally suggested the non-existent "9.3.1" — a genuine net10-targeted `10.0.2` build exists |
| Ardalis.ApiEndpoints | 4.1.0 | **4.1.0** (no change) | tool originally suggested non-existent "9.3.1" |
| Ardalis.GuardClauses | 4.0.1 | **5.0.0** | |
| Ardalis.Result | 7.0.0 | **10.1.0** | |
| Ardalis.Specification | 7.0.0 | **9.3.1** | kept in lockstep with `.EntityFrameworkCore` variant |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | **9.3.1** | |
| Ardalis.ListStartupServices | 1.1.4 | **1.1.4** (no change) | already latest |
| AutoMapper.Extensions.Microsoft.DependencyInjection → **AutoMapper** | 12.0.1 | **removed → `AutoMapper` 16.2.0** | companion package orphaned at 12.0.1 (pulls a HIGH severity DoS CVE, GHSA-rvv3-g6hj-g44x); migrated to the core package, which has bundled DI registration since v13. **Commercial license tier applies from v13+ — flagged for legal review.** |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | **1.5.2** | tool originally suggested non-existent "1.21.0" |
| Azure.Identity | 1.10.4 | **1.21.0** | **security-relevant** — multiple CVEs fixed since 1.10.4 |
| MediatR | 12.0.1 | **12.5.0** | deliberately kept on the last free/Apache-2.0 12.x line (not the real latest 14.2.0) — v13+ requires a commercial license (mediatr.io); no CVE forces the jump, decision flagged for user |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | **3.0.114** | |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | **1.23.0** | |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | **8.23.0** | |
| FluentValidation | 11.9.0 | **12.2.0** | no breaking changes hit during build/test |
| Blazored.LocalStorage | 4.5.0 | **4.5.0** (no change) | tool originally suggested non-existent "4.5.1" |
| **BlazorInputFile** | 0.2.0 | **removed** | unmaintained since 2019, no net10 build; only usage was dead code (`CatalogItem.DataToBase64`, never called) — removed package refs, the `@using`, and the dead method entirely |
| MinimalApi.Endpoint | 1.3.0 | **1.3.0** (no change) | already latest for net10 |
| NSubstitute | 5.1.0 | **6.2.0** | major version bump; all 44 UnitTests pass unchanged |
| NSubstitute.Analyzers.CSharp | 1.0.17 | **1.0.17** (no change) | already latest |
| System.Security.Claims | 4.3.0 | **removed** | BCL-shipped on net10 (NU1510 "will not be pruned") |
| System.Text.Json | 8.0.3 (explicit override) | **removed** | BCL-shipped on net10 |
| Swashbuckle.AspNetCore / .SwaggerUI / .Annotations | 6.5.0 | **10.3.0** | major jump — required code fixes: `Microsoft.OpenApi.Models` namespace moved to `Microsoft.OpenApi`, `ISchemaFilter.Apply` now takes `IOpenApiSchema`, `OpenApiSecurityRequirement`/`AddSecurityRequirement` API reshaped (see `tasks/02-upgrade-all-projects/progress-details.md`) |
| Microsoft.NET.Test.Sdk | 17.9.0 | **18.10.1** | |
| xunit | 2.7.0 | **2.9.3** | |
| xunit.runner.visualstudio | 2.5.6 | **4.0.1** | |
| xunit.runner.console | 2.7.0 | **removed** | obsolete standalone runner, unneeded with `xunit.runner.visualstudio` + `dotnet test` |
| MSTest.TestAdapter / MSTest.TestFramework | 3.2.2 | **4.5.1** | |
| coverlet.collector | 6.0.2 | **10.1.0** | tool originally suggested non-existent "6.2.0"; real latest is 10.1.0 |
| Microsoft.AspNetCore.Mvc (orphan, PublicApi-era legacy) | 2.2.0 | **removed** | confirmed unreferenced by any `.csproj` |
| BuildBundlerMinifier | 3.2.449 | **3.2.449** (no change) | build-time-only tool, Release-only condition in Web.csproj, works fine on net10 |
| NuGet.Packaging / NuGet.Protocol (transitive) | n/a | **pinned to 6.12.5** | explicit transitive pin via `CentralPackageTransitivePinningEnabled`; patches a low-severity advisory (GHSA-g4vj-cjjj-v7hg) pulled in by `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (the latest available version of that design-time-only package) |

## Security Vulnerabilities (by default, included in upgrade)

| Package | Pre-upgrade Version | Concern | Resolution |
|---|---|---|---|
| Azure.Identity | 1.10.4 | Multiple CVEs patched in 1.11+ (elevation-of-privilege / token-cache issues) | Upgraded to 1.21.0 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | Known advisories around JWT validation fixed in 8.x | Upgraded to 8.23.0 |
| AutoMapper (via `.Extensions.Microsoft.DependencyInjection` 12.0.1) | 12.0.1 | **HIGH severity** DoS (GHSA-rvv3-g6hj-g44x), unbounded recursive-mapping stack overflow | Migrated to core `AutoMapper` 16.2.0 (patched); **introduces a commercial license tier for qualifying commercial use — flagged for legal review, not resolved autonomously** |
| NuGet.Packaging / NuGet.Protocol (transitive, via scaffolding tool) | 6.12.1 | Low severity "defense in depth" advisory (GHSA-g4vj-cjjj-v7hg) | Explicit transitive pin to 6.12.5 |

These were addressed as part of the upgrade (default behavior — not opted out by the user,
and explicitly required by the `securityComplianceCheck=true` success criterion).

## Risks & Special Handling

1. **Centralized TFM via `Directory.Packages.props`** — the `<TargetFramework>net8.0</TargetFramework>`
   property must be changed to `net10.0` in this single file; it propagates to every project
   automatically. No per-project `TargetFramework` edits should be needed, but must verify after the
   change that each project actually inherits it (none of the 10 csproj files declare their own TFM).
2. **BlazorAdmin (`Microsoft.NET.Sdk.BlazorWebAssembly`)** — Blazor WASM had notable hosting-model
   changes between net8 → net9 → net10 (e.g., WASM AOT defaults, `Microsoft.AspNetCore.Components.WebAssembly.Server`
   static web assets pipeline). Needs careful verification Web.csproj (host) + BlazorAdmin (client) still
   wire up correctly, and that `Microsoft.AspNetCore.Components.WebAssembly.DevServer` isn't needed at
   runtime (dev-only).
3. **BlazorInputFile package (abandoned, pre-2020)** — remove package + `@using`; unused in code, so
   zero functional risk, but it is a blocking compatibility risk if left in place (no net10 assets).
4. **Legacy `DotNetCliToolReference` in FunctionalTests.csproj** — `dotnet-xunit` CLI tool is from
   the old `project.json`/early SDK-style era; unsupported today and should be removed entirely (xunit
   test discovery runs via `xunit.runner.visualstudio` instead).
5. **AutoMapper 12 → 14 major jump** — AutoMapper 12/13 changed from attribute/profile auto-scan
   defaults and introduced licensing changes around v13/v14 (community vs. commercial) — must confirm
   `AutoMapper.Extensions.Microsoft.DependencyInjection` v14 registration API (`AddAutoMapper(...)`) is
   unchanged in `Web`/`PublicApi` startup code.
6. **Swashbuckle 6.5 → 10.x major jump** — verify `AddSwaggerGen`/`UseSwagger`/`UseSwaggerUI` calls and
   any `[SwaggerOperation]`/annotations still compile; check Program.cs / startup extension methods in
   `PublicApi`.
7. **Possibly-orphaned `Microsoft.AspNetCore.Mvc` 2.2.0 `PackageVersion` entry** — confirm unused,
   remove from `Directory.Packages.props` if so (keeping it would otherwise force resolving an
   incompatible net10 dependency).
8. **`global.json` SDK pin (`8.0.x`)** — must bump to `10.0.x` or the `dotnet` CLI itself will refuse
   to build/restore once csproj TFMs target net10.0.
9. **Azure Functions / isolated-worker**: not applicable — no Functions projects in this solution.
10. **Docker/compose files** (`docker-compose.yml`, `docker-compose.override.yml`, `docker-compose.dcproj`,
    and per-project `Dockerfile`s if present) likely pin `mcr.microsoft.com/dotnet/aspnet:8.0` /
    `sdk:8.0` base images — must bump to `10.0` tags for container builds to stay consistent (lower
    priority than code/package changes, but required for `securityComplianceCheck` + runnable containers).

## Code Metrics (approximate, from repository layout)

- 10 projects total (6 src, 4 test), already SDK-style, CPM-enabled.
- No legacy `packages.config`, no `.vbproj`, no native/C++ projects.
- No Azure Functions projects, no Aspire AppHost, no WinForms/WPF.

## Success Criteria Mapping

- `passBuild=true` → all 10 projects must build clean (0 errors, 0 warnings) against net10.0.
- `generateNewUnitTests=false` → do not author new test files; only fix existing tests broken by
  the framework/package bumps above.
- `passUnitTests=true` → `UnitTests`, `IntegrationTests`, `FunctionalTests`,
  `PublicApiIntegrationTests` must all pass after the upgrade.
- `securityComplianceCheck=true` → Azure.Identity and System.IdentityModel.Tokens.Jwt (and any other
  flagged packages) must land on patched versions; no known-vulnerable package versions left in
  `Directory.Packages.props`.

## Next Step

Proceed to **Planning** stage — build `plan.md` with a task breakdown covering: (1) centralized TFM +
global.json bump, (2) package version bumps in `Directory.Packages.props` grouped by shared property
vs. individual packages, (3) removal of obsolete/orphan packages (BlazorInputFile, stale
Microsoft.AspNetCore.Mvc 2.2.0 entry if confirmed unused, legacy `DotNetCliToolReference`), (4) per-project
build/fix passes in topological order, (5) test validation, (6) Docker base image bump.
