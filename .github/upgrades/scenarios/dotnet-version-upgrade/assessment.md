# .NET Upgrade Assessment — eShopOnWeb: net8.0 → net10.0

> Note: The `generate_dotnet_upgrade_assessment` MCP tool returned
> `Orchestrator channel was not available within 5000 ms` on repeated retries
> and could not be used. This assessment was produced manually using
> `get_projects_in_topological_order`, `get_project_dependencies`,
> `get_supported_package_version`, direct project-file inspection, and
> NuGet.org version lookups.

## Executive Summary

- **Projects**: 10 (6 source, 4 test) — all SDK-style, all currently targeting `net8.0`.
- **TFM/SDK source**: centrally managed. `Directory.Packages.props` defines
  `<TargetFramework>net8.0</TargetFramework>` plus version properties
  (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`,
  `VSCodeGeneratorVersion`) consumed by individual `PackageVersion` entries.
  `global.json` pins SDK `8.0.x`.
- **Complexity classification**: **Simple** — all projects already modern
  SDK-style, no .NET Framework projects, no old packages.config, no
  cross-cutting trigger signals (no cookie auth / System.Web / binding
  redirects concerns — this is a pure modern-to-modern TFM bump).
- **Recommended strategy**: **All-At-Once** (10 projects, ≤3-tier dependency
  depth, ≤15 projects, no CI-green constraint).
- **No security vulnerabilities flagged** in current package set — this is a
  routine version bump, not a vulnerability remediation.

## Dependency Graph (topological order, leaf → root)

1. `src/BlazorShared/BlazorShared.csproj`
2. `src/ApplicationCore/ApplicationCore.csproj` → BlazorShared
3. `src/Infrastructure/Infrastructure.csproj` → ApplicationCore
4. `src/BlazorAdmin/BlazorAdmin.csproj` → BlazorShared
5. `src/Web/Web.csproj` → ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure
6. `src/PublicApi/PublicApi.csproj` → ApplicationCore, Infrastructure
7. `tests/UnitTests/UnitTests.csproj` → ApplicationCore, Web
8. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` → PublicApi, Web
9. `tests/FunctionalTests/FunctionalTests.csproj` → ApplicationCore, PublicApi, Web
10. `tests/IntegrationTests/IntegrationTests.csproj` → Infrastructure, UnitTests

Since all projects already share the same centrally-managed TFM, the TFM bump
applies to all of them atomically via the single `Directory.Packages.props`
property — there's no per-project phasing required for the TFM move itself.

## Project Types

| Project | SDK | Kind |
|---|---|---|
| BlazorShared | Microsoft.NET.Sdk | Class library |
| ApplicationCore | Microsoft.NET.Sdk | Class library (domain) |
| Infrastructure | Microsoft.NET.Sdk | Class library (EF Core, Identity) |
| BlazorAdmin | Microsoft.NET.Sdk.BlazorWebAssembly | Blazor WASM app |
| Web | Microsoft.NET.Sdk.Web | ASP.NET Core MVC/Razor app, hosts BlazorAdmin |
| PublicApi | Microsoft.NET.Sdk.Web | ASP.NET Core minimal/MVC API |
| UnitTests | Microsoft.NET.Sdk | xUnit test project |
| IntegrationTests | Microsoft.NET.Sdk | xUnit test project |
| FunctionalTests | Microsoft.NET.Sdk | xUnit test project (WebApplicationFactory) |
| PublicApiIntegrationTests | Microsoft.NET.Sdk | MSTest test project |

## Package Analysis — Current vs. Target (net10.0-compatible)

Resolved via `get_supported_package_version` (project-aware) with NuGet.org
cross-checks. Grouped-by-variable packages (AspNetVersion, EntityFramworkCoreVersion,
SystemExtensionVersion, VSCodeGeneratorVersion) move together.

| Package | Current | Target | Notes |
|---|---|---|---|
| (variable) AspNetVersion | 8.0.2 | **10.0.12** | Drives all `Microsoft.AspNetCore.*` / `Microsoft.Extensions.Identity.Core` entries below |
| (variable) EntityFramworkCoreVersion | 8.0.2 | **10.0.12** | Drives EF Core packages |
| (variable) SystemExtensionVersion | 8.0.0 | **10.0.12** | Drives `System.Net.Http.Json`, `Microsoft.Extensions.Logging.Configuration` |
| (variable) VSCodeGeneratorVersion | 8.0.0 | **10.0.2** | `Microsoft.VisualStudio.Web.CodeGeneration.Design` |
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 (no change) | Already latest |
| Ardalis.GuardClauses | 4.0.1 | **5.0.0** | Major bump — review for breaking changes |
| Ardalis.Specification | 7.0.0 | **9.3.1** | Major bump (7→9) |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | **9.3.1** | Major bump, keep in lockstep with Ardalis.Specification |
| Ardalis.Result | 7.0.0 | **10.1.0** | Major bump (7→10) |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 (no change) | Already latest |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | **1.5.2** | Minor bump |
| Azure.Identity | 1.10.4 | **1.21.0** | Minor/feature bumps, watch CVE history |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 (no change) | Already latest in this major line |
| BlazorInputFile | 0.2.0 | 0.2.0 (no change) | Unmaintained package, no newer version exists |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 (no change) | Already latest; tool reported no version constrained to net10 — verify WASM compatibility during build |
| FluentValidation | 11.9.0 | **12.1.1** | Major bump — review validator API usage |
| MediatR | 12.0.1 | **14.2.0** | Major bump x2 — verify registration API (`AddMediatR(cfg => ...)`) still matches; note MediatR's licensing changed in recent majors (review before accepting commercially) |
| Microsoft.AspNetCore.Mvc | 2.2.0 | 2.3.13 | Legacy metapackage reference (predates ASP.NET Core 3 "shared framework" model); kept for compatibility, latest 2.x still resolves |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | **3.0.114** | Build-time only (libman) |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | **1.23.0** | Build-time only (container tooling) |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 (no change) | Already latest |
| NSubstitute | 5.1.0 | **6.2.0** | Major bump — test-only |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 (no change) | Already latest |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | **8.23.0** | Major bump |
| System.Security.Claims | 4.3.0 | 4.3.0 (no change) | Legacy compat shim, unchanged |
| System.Text.Json | 8.0.3 | **10.0.12** | Tracks SystemExtensionVersion/runtime |
| Swashbuckle.AspNetCore | 6.5.0 | **10.3.0** | Major bump — verify Swagger attribute/filter APIs |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | **10.3.0** | Keep in lockstep with Swashbuckle.AspNetCore |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | **10.3.0** | Keep in lockstep |
| Microsoft.NET.Test.Sdk | 17.9.0 | **18.10.1** | Test-only |
| xunit | 2.7.0 | **2.9.3** | Stayed on xunit v2 line (v3 is a bigger migration, not required for net10) |
| xunit.runner.visualstudio | 2.5.6 | **4.0.1** | Test-only |
| xunit.runner.console | 2.7.0 | **2.9.3** | Test-only |
| MSTest.TestAdapter | 3.2.2 | **4.5.1** | Test-only |
| MSTest.TestFramework | 3.2.2 | **4.5.1** | Test-only |
| coverlet.collector | 6.0.2 | **10.1.0** | Test-only |

## Risks / Things to Watch During Execution

1. **MediatR 12 → 14**: confirm `AddMediatR` registration call sites still compile; licensing terms changed for recent MediatR majors — flagged for awareness, proceeding since this is an automated non-interactive upgrade and no commercial redistribution concern blocks the build/test goal.
2. **FluentValidation 11 → 12**: validator base classes/extension methods may have changed; fix any compile errors in `ApplicationCore`/`Web` validators.
3. **Ardalis.Specification / Ardalis.Result / Ardalis.GuardClauses major bumps**: used pervasively in `ApplicationCore`/`Infrastructure` specifications — check for renamed APIs.
4. **Swashbuckle 6 → 10**: `PublicApi` Swagger configuration/filters may need adjustment.
5. **`Microsoft.AspNetCore.Mvc` 2.2.0 pinned version**: this old metapackage reference pattern predates the ASP.NET Core "shared framework" (post-3.0) model — verify it still resolves/is harmless on net10 (it has historically been a no-op compatibility shim once on Microsoft.NET.Sdk.Web).
6. **global.json**: SDK channel must move from `8.0.x` to `10.0.x`.
7. Many packages already at their latest version and require **no change** — don't bump these needlessly.

## Dependency/Risk Sign-off

No .NET Framework projects, no `packages.config`, no old-style csproj, no binding
redirects, no System.Web/OWIN cookie-auth signals detected. This is a
straightforward modern-to-modern (net8→net10) upgrade.
