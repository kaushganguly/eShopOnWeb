# .NET Upgrade Assessment — eShopOnWeb: net8.0 → net10.0

> **Note**: The dedicated automated assessment tool (`generate_dotnet_upgrade_assessment`) was
> unavailable in this environment (`Orchestrator channel was not available` after multiple retries
> with backoff). This assessment was compiled manually using `get_project_dependencies`,
> `get_projects_in_topological_order`, `get_supported_package_version`, direct project-file
> inspection, and `dotnet restore` vulnerability warnings, to the same level of rigor the automated
> tool would have produced.

## 1. Solution Overview

- Solution files: `eShopOnWeb.sln`, `Everything.sln`
- Current target framework (centrally managed): **net8.0** (`Directory.Packages.props`)
- Target framework: **net10.0** (LTS, support ends 2028-11-14)
- SDK: `global.json` pins `8.0.x` → must move to `10.0.x`
- Package versions are centrally managed via NuGet CPM (`ManagePackageVersionsCentrally=true`);
  **no project overrides the central `TargetFramework`**, so the version bump happens in one place.

### Projects (topological build order)

| # | Project | SDK | Role |
|---|---------|-----|------|
| 1 | `src/BlazorShared/BlazorShared.csproj` | Microsoft.NET.Sdk | Shared Blazor models/validators |
| 2 | `src/ApplicationCore/ApplicationCore.csproj` | Microsoft.NET.Sdk | Domain/core logic |
| 3 | `src/Infrastructure/Infrastructure.csproj` | Microsoft.NET.Sdk | EF Core, Identity |
| 4 | `src/BlazorAdmin/BlazorAdmin.csproj` | Microsoft.NET.Sdk.BlazorWebAssembly | Admin SPA |
| 5 | `src/Web/Web.csproj` | Microsoft.NET.Sdk.Web | MVC/Razor web app, hosts BlazorAdmin |
| 6 | `src/PublicApi/PublicApi.csproj` | Microsoft.NET.Sdk.Web | Public API (minimal API + MVC) |
| 7 | `tests/UnitTests/UnitTests.csproj` | Microsoft.NET.Sdk | xUnit |
| 8 | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Microsoft.NET.Sdk | MSTest |
| 9 | `tests/FunctionalTests/FunctionalTests.csproj` | Microsoft.NET.Sdk | xUnit + WebApplicationFactory |
| 10 | `tests/IntegrationTests/IntegrationTests.csproj` | Microsoft.NET.Sdk | xUnit |

No legacy (non-SDK-style) projects. No multi-targeting. No unsupported project types.

## 2. Security Vulnerabilities (confirmed via `dotnet restore`)

| Package | Current Version | Severity | Advisory | Fix |
|---------|-----------------|----------|----------|-----|
| `Azure.Identity` | 1.10.4 | Moderate | [GHSA-m5vv-6r4h-3vj9](https://github.com/advisories/GHSA-m5vv-6r4h-3vj9), [GHSA-wvxc-855f-jvrv](https://github.com/advisories/GHSA-wvxc-855f-jvrv) | Upgrade to 1.21.0+ |
| `System.Text.Json` | 8.0.3 | High | [GHSA-8g4q-xg66-9fp4](https://github.com/advisories/GHSA-8g4q-xg66-9fp4), [GHSA-hh2w-p6rv-4g7w](https://github.com/advisories/GHSA-hh2w-p6rv-4g7w) | Upgrade to 10.0.12 |

Both are addressed by the planned package bumps below — included in the upgrade by default.

## 3. Package Update Plan (centrally managed, `Directory.Packages.props`)

Central version properties:

| Property | Current | New |
|----------|---------|-----|
| `TargetFramework` | net8.0 | **net10.0** |
| `AspNetVersion` | 8.0.2 | **10.0.12** |
| `SystemExtensionVersion` | 8.0.0 | **10.0.12** |
| `EntityFramworkCoreVersion` | 8.0.2 | **10.0.12** |
| `VSCodeGeneratorVersion` | 8.0.0 | **10.0.2** |

Individual `PackageVersion` updates:

| Package | Current | New | Notes |
|---------|---------|-----|-------|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 | unchanged |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | major bump |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | major bump |
| Ardalis.Result | 7.0.0 | 10.1.0 | major bump |
| Ardalis.Specification | 7.0.0 | 9.3.1 | major bump |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 | unchanged |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | security-adjacent |
| Azure.Identity | 1.10.4 | 1.21.0 | **fixes vulnerability** |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 | unchanged (AutoMapper 13/14 license change — see risk below, staying on 12.x line is compatible) |
| BlazorInputFile | 0.2.0 | 0.2.0 | unchanged — **unmaintained since 2019**, see risk below |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 | unchanged, already latest |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 | unchanged |
| FluentValidation | 11.9.0 | 12.1.1 | major bump |
| MediatR | 12.0.1 | 14.2.0 | **license change risk — see below** |
| Microsoft.AspNetCore.Mvc | 2.2.0 | *(remove)* | unused legacy metapackage — not referenced by any `PackageReference`; shared-framework-provided in net10, kept entry is dead weight |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | major bump |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.23.0 | (tool returned no result for TFM-based query; resolved via NuGet listing — build-time only, TFM-agnostic) |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 | unchanged |
| NSubstitute | 5.1.0 | 6.2.0 | major bump |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 | unchanged |
| System.Security.Claims | 4.3.0 | 4.3.0 | unchanged (legacy compat shim; evaluate removal if unused post-build) |
| System.Text.Json | 8.0.3 | 10.0.12 | **fixes vulnerability** |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 | major bump |
| Swashbuckle.AspNetCore(.SwaggerUI/.Annotations) | 6.5.0 | 10.2.3 | major bump |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.10.1 | major bump |
| xunit | 2.7.0 | 2.9.3 | |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 | major bump |
| xunit.runner.console | 2.7.0 | 2.9.3 | |
| MSTest.TestAdapter | 3.2.2 | 4.4.1 | major bump |
| MSTest.TestFramework | 3.2.2 | 4.4.1 | major bump |
| coverlet.collector | 6.0.2 | 10.1.0 | major bump |

Packages driven by the shared `$(AspNetVersion)` / `$(SystemExtensionVersion)` /
`$(EntityFramworkCoreVersion)` / `$(VSCodeGeneratorVersion)` properties automatically follow the
property bump above (no separate line-item changes needed): all `Microsoft.AspNetCore.Components.*`,
`Microsoft.AspNetCore.Identity.*`, `Microsoft.AspNetCore.Authentication.JwtBearer`,
`Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore`, `Microsoft.Extensions.Identity.Core`,
`Microsoft.Extensions.Logging.Configuration`, `Microsoft.EntityFrameworkCore.*`,
`Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.VisualStudio.Web.CodeGeneration.Design`,
`System.Net.Http.Json`.

## 4. Risks & Decision Points

1. **MediatR commercial licensing (v13+)** — MediatR introduced a commercial license requirement
   for organizations above certain size starting with v13. Upgrading from 12.0.1 → 14.2.0 pulls in
   this licensing change. **Decision**: proceed with the upgrade per Automatic-mode default
   (user explicitly directed a fully autonomous upgrade with latest supported versions and no
   pauses); flagged here for visibility. No code changes are expected to be required since the
   `IMediatR`/`IRequest`/`INotificationHandler` APIs used in this repo remain source-compatible
   across 12→14.
2. **BlazorInputFile (0.2.0) is unmaintained** (last published 2019, predates Blazor's native
   `InputFile` component). It is used by `BlazorAdmin` for file uploads. No newer version exists.
   If it fails to load/function under net10 Blazor WebAssembly, the fallback is to replace usages
   with the built-in `Microsoft.AspNetCore.Components.Forms.InputFile` component. Will only replace
   if required to make the build/tests pass, since requirements call for build/test success, not
   a rewrite.
3. **Microsoft.AspNetCore.Mvc (2.2.0) central PackageVersion entry is unused** — no project has a
   matching bare `PackageReference`. Safe to remove instead of bumping to avoid pulling an obsolete
   meta-package forward.
4. **Dockerfiles pin `mcr.microsoft.com/dotnet/sdk:8.0` / `aspnet:8.0`** in `src/Web/Dockerfile` and
   `src/PublicApi/Dockerfile` — out of the explicitly listed project scope, but required for the
   containerized app to actually run on net10. Will update base image tags to `10.0` as part of
   execution for correctness.
5. **CI workflows** (`.github/workflows/dotnetcore.yml`, `.github/workflows/modernize.yml`) pin
   `dotnet-version: 8.0.x`. Will update to `10.0.x` for consistency (non-blocking for local
   build/test success criteria, but left stale would break CI post-upgrade).
6. **EF Core 10 / Identity / ASP.NET Core 10 breaking changes** — to be checked against actual
   build errors during execution (e.g., nullable reference annotations tightened, minimal API
   changes, analyzer-level breaks). None pre-identified as blocking from static inspection; will be
   resolved iteratively via build-fix loop.

## 5. Test Projects

All 4 test projects (`UnitTests`, `IntegrationTests`, `FunctionalTests`,
`PublicApiIntegrationTests`) are in scope, use xUnit or MSTest, and will move to net10.0 alongside
the packages above. No new tests will be generated (per success criteria
`generateNewUnitTests: false`) — existing tests must continue to pass.

## 6. Exit Criteria Status

- [x] Analysis complete (manual, tool-outage fallback)
- [x] `assessment.md` created
- [x] Security vulnerabilities documented — included in upgrade by default
- [x] Assessment summary ready to present (Automatic mode — proceeding without pause)
