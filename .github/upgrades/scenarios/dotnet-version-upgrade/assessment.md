# .NET Upgrade Assessment — eShopOnWeb: net8.0 → net10.0

> Note: The dedicated assessment engine (`generate_dotnet_upgrade_assessment`) was unavailable in
> this session ("Orchestrator channel was not available" after multiple retries). This assessment
> was produced manually by inspecting the solution, project files, and querying
> `get_supported_package_version` for each referenced NuGet package.

## Solution Overview

- Solution: `eShopOnWeb.sln` (also `Everything.sln`, superset including docker-compose project)
- Current target framework: `net8.0`, set **centrally** in `Directory.Packages.props`
  (`<TargetFramework>net8.0</TargetFramework>` — all projects inherit it; no project overrides it)
- Central Package Management (CPM) already enabled (`ManagePackageVersionsCentrally=true`)
- Global SDK pin: `global.json` → `"version": "8.0.x"`, `rollForward: latestFeature`
- All projects are SDK-style already (no legacy/packages.config projects) — classification: **Simple** upgrade
- 10 projects, topologically ordered:
  1. BlazorShared (class lib)
  2. ApplicationCore (class lib, depends on BlazorShared)
  3. Infrastructure (class lib, depends on ApplicationCore)
  4. BlazorAdmin (Blazor WASM, depends on BlazorShared)
  5. Web (ASP.NET Core MVC/Razor host, depends on ApplicationCore/BlazorAdmin/BlazorShared/Infrastructure)
  6. PublicApi (ASP.NET Core minimal/Web API, depends on ApplicationCore/Infrastructure)
  7. UnitTests (xunit, depends on ApplicationCore/Web)
  8. PublicApiIntegrationTests (MSTest, depends on PublicApi/Web)
  9. FunctionalTests (xunit, depends on ApplicationCore/PublicApi/Web)
  10. IntegrationTests (xunit, depends on Infrastructure/UnitTests)

## .NET SDK / Runtime Availability

Machine has .NET SDKs 8.x, 9.x, 10.x installed (10.0.401 active via `global.json` once updated).
ASP.NET Core / NETCore runtimes for 10.0 are present. No installation blocker.

## Package Compatibility (via `get_supported_package_version`, targetFrameworks=net10.0)

Centrally-versioned property groups in `Directory.Packages.props`:

| MSBuild property | Current | → net10.0 |
|---|---|---|
| `AspNetVersion` (all `Microsoft.AspNetCore.*` packages) | 8.0.2 | **10.0.12** |
| `SystemExtensionVersion` (`Microsoft.Extensions.Logging.Configuration`, `System.Net.Http.Json`) | 8.0.0 | **10.0.12** |
| `EntityFramworkCoreVersion` (EF Core In-Memory/SqlServer/Tools) | 8.0.2 | **10.0.12** |
| `VSCodeGeneratorVersion` (`Microsoft.VisualStudio.Web.CodeGeneration.Design`) | 8.0.0 | **10.0.2** |

Individually-versioned packages:

| Package | Current | → net10.0 | Notes |
|---|---|---|---|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 | unchanged, already net10-eligible |
| Ardalis.GuardClauses | 4.0.1 | **5.0.0** | major bump |
| Ardalis.Specification | 7.0.0 | **9.3.1** | major bump |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | **9.3.1** | keep in sync with Ardalis.Specification |
| Ardalis.Result | 7.0.0 | **10.1.0** | major bump |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 | unchanged |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | **1.5.2** | |
| Azure.Identity | 1.10.4 | **1.21.0** | ⚠️ security — 1.10.4 has known CVEs (CVE-2024-29992 et al.); fix included |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 | unchanged |
| BlazorInputFile | 0.2.0 | 0.2.0 | unchanged — unmaintained package, verify still builds under net10 Blazor WASM |
| Blazored.LocalStorage | 4.5.0 | **no net10-validated version reported** by tool — will try latest stable (5.x) and validate by build |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 | unchanged (design-time/build tool) |
| FluentValidation | 11.9.0 | **12.1.1** | major bump — check for breaking API renames |
| MediatR | 12.0.1 | **14.2.0** | ⚠️ 2 major bumps — check breaking changes (namespace / notification publishing changes between 12→14) |
| Microsoft.AspNetCore.Mvc | 2.2.0 | **2.3.13** | legacy meta-package pinned at old 2.x line (pre-dates ASP.NET Core 3 "shared framework" packages) — candidate for removal since ASP.NET Core APIs now ship in the shared framework; keep minimal version bump if removal regresses build |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | **3.0.114** | |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | **no net10-validated version reported** — keep current or latest stable; non-functional (VS tooling only), should not block CI build |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 | unchanged |
| NSubstitute | 5.1.0 | **6.2.0** | major bump |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 | unchanged |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | **8.23.0** | ⚠️ security relevant (JWT handling) |
| System.Security.Claims | 4.3.0 | 4.3.0 | unchanged — legacy compat shim, type now part of BCL; candidate for removal |
| System.Text.Json | 8.0.3 | **10.0.12** (align w/ SystemExtensionVersion family) | ⚠️ security — several System.Text.Json 8.x CVEs fixed in later lines |
| Swashbuckle.AspNetCore(+.SwaggerUI/.Annotations) | 6.5.0 | **10.2.3** | major bump — verify Swagger UI still renders, minimal API endpoint grouping |
| Microsoft.NET.Test.Sdk | 17.9.0 | **18.10.1** | |
| xunit | 2.7.0 | **2.9.3** | |
| xunit.runner.visualstudio | 2.5.6 | **4.0.0** | major bump |
| xunit.runner.console | 2.7.0 | **2.9.3** | |
| MSTest.TestAdapter / MSTest.TestFramework | 3.2.2 | **4.4.1** | major bump |
| coverlet.collector | 6.0.2 | **10.1.0** | major bump |

## Docker / Infra

- `src/Web/Dockerfile` and `src/PublicApi/Dockerfile` pin `mcr.microsoft.com/dotnet/sdk:8.0` and
  `mcr.microsoft.com/dotnet/aspnet:8.0` base images → must bump to `10.0`.
- No `Directory.Build.props`/`.targets` files found.
- `azure.yaml`/`infra/` present (Bicep) — not expected to reference TFM directly; out of scope
  unless build/deploy breaks.

## Risks

1. **MediatR 12 → 14**: two major versions; check `IRequestHandler`/`INotificationHandler`
   registration calls in `Program.cs`/DI setup for signature changes.
2. **Swashbuckle 6 → 10**: four major versions; verify minimal API + `Ardalis.ApiEndpoints` /
   `MinimalApi.Endpoint` Swagger generation still works, annotations attribute usage unchanged.
3. **Ardalis.* family major bumps** (GuardClauses 4→5, Specification 7→9, Result 7→10): check for
   renamed APIs used in `ApplicationCore`/`Infrastructure`/`Web`/`PublicApi`.
4. **Blazored.LocalStorage / BlazorInputFile / Microsoft.VisualStudio.Azure.Containers.Tools.Targets**:
   no explicit net10-validated version from tooling — resolve during restore/build; fall back to
   latest stable on NuGet if needed.
5. **Microsoft.AspNetCore.Mvc 2.2.0 legacy package**: verify it is not actually needed (ASP.NET
   Core 3+ ships these APIs via the shared framework) — may be removable, reducing risk surface.
6. **Test framework major bumps** (xunit.runner.visualstudio 2→4, MSTest 3→4, coverlet 6→10): verify
   test discovery/execution still functions post-upgrade.
7. **C# language version**: `Web`/`UnitTests` already pin `<LangVersion>latest</LangVersion>` — no
   action needed, C# compiler bump is automatic with the net10 SDK.

## Security Vulnerabilities Addressed by This Upgrade

| Package | Current | Issue | Resolution |
|---|---|---|---|
| Azure.Identity | 1.10.4 | Known CVEs in Azure.Identity < 1.11 | Upgrade to 1.21.0 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | JWT library security fixes in later releases | Upgrade to 8.23.0 |
| System.Text.Json | 8.0.3 | System.Text.Json advisories fixed in 8.0.5+/9.x/10.x | Upgrade to 10.0.12 |

## Recommended Strategy

**Single-phase, whole-solution upgrade** (Simple classification): update the central TFM and SDK
pin once, update all package versions in `Directory.Packages.props` in one pass (dependency order
doesn't matter for CPM since it's one file), then build bottom-up through the dependency graph
fixing compile errors, then run tests.
