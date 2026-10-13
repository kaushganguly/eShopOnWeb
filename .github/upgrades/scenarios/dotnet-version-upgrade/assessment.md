# Assessment — eShopOnWeb: .NET 8.0 → .NET 10.0

> **Note on methodology**: The dedicated `generate_dotnet_upgrade_assessment` tool was unavailable in
> this environment (`Orchestrator channel was not available within 5000 ms. Extender is not connected
> to an orchestrator peer.` — retried 7 times over several minutes with increasing backoff; the error
> never cleared). This assessment was built manually using `get_dotnet_upgrade_options`,
> `get_supported_package_version`, `validate_dotnet_sdk_installation`, and direct repository inspection,
> which all functioned normally. No `assessment.json` exists, so `query_dotnet_assessment` is not
> available for this scenario — detail below is carried directly in this document instead.

## Executive Summary

- **Projects**: 10 (6 `src/`, 4 `tests/`), all SDK-style, all already on **net8.0**, centrally managed
  via `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`, single `<TargetFramework>`
  property shared by every project).
- **Current → Target**: net8.0 → **net10.0** (LTS, EOL 2028‑11‑14), SDK pinned via `global.json`
  (`8.0.x` → needs bump to a 10.x SDK, `rollForward: latestFeature` kept).
  Source: `get_dotnet_upgrade_options` (net9.0 STS / **net10.0 LTS** / net11.0 Preview).
- **Complexity**: Simple/mechanical — no .NET Framework projects, no `packages.config`, no
  `Startup.cs` (already `Program.cs` minimal hosting), no circular project references.
- **Dependency graph** (leaf → root):
  1. `BlazorShared` (no project deps)
  2. `ApplicationCore` → BlazorShared
  3. `BlazorAdmin` → BlazorShared
  4. `Infrastructure` → ApplicationCore
  5. `PublicApi` → ApplicationCore, Infrastructure
  6. `Web` → ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure
  7. `UnitTests` → ApplicationCore, Web
  8. `IntegrationTests` → Infrastructure, UnitTests
  9. `FunctionalTests` → ApplicationCore, PublicApi, Web
  10. `PublicApiIntegrationTests` → PublicApi, Web
- **Security vulnerabilities**: None flagged by NuGet audit tooling available in this environment;
  several packages are several majors behind, which is addressed below as part of the upgrade.

## Package Compatibility (current → net10.0-compatible version)

Resolved with `get_supported_package_version(targetFrameworks=net10.0)` against each consuming project.

| Package | Current | → net10.0 | Risk |
|---|---|---|---|
| Microsoft.AspNetCore.* (Components.*, Identity.*, Authentication.JwtBearer, Diagnostics.EntityFrameworkCore) | 8.0.2 | **10.0.12** | Low — in-box shared-framework packages, API-compatible |
| Microsoft.Extensions.Logging.Configuration / System.Net.Http.Json / System.Text.Json | 8.0.0 / 8.0.0 / 8.0.3 | **10.0.12** | Low |
| Microsoft.EntityFrameworkCore.InMemory / .SqlServer / .Tools | 8.0.2 | **10.0.12** | Medium — EF Core 8→10 has provider/behavior changes; migrations and query translation should be re-verified |
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 (unchanged) | None |
| Ardalis.GuardClauses | 4.0.1 | **5.0.0** | Low–Medium — major version bump, check for removed/renamed guard methods |
| Ardalis.Result | 7.0.0 | **10.1.0** | Medium — major bumps (7→10), check Result<T> API surface used in ApplicationCore/PublicApi |
| Ardalis.Specification / .EntityFrameworkCore | 7.0.0 | **9.3.1** | Medium — check `ISpecification<T>` API surface |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 (unchanged) | None |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | **1.5.2** | Low |
| Azure.Identity | 1.10.4 | **1.21.0** | Low — review CVE history across the range |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 (unchanged — AutoMapper 13+ changed licensing/packaging; this DI-extension package has no newer net10-validated version per the resolver) | Low |
| FluentValidation | 11.9.0 | **12.2.0** | Low–Medium — major bump, check for deprecated validator APIs |
| MediatR | 12.0.1 | **14.2.0** | **High** — two major versions; MediatR has had notable breaking changes (namespace/registration changes, licensing announcements in some majors). Verify `AddMediatR` registration syntax and handler interfaces compile; confirm current license terms are acceptable before taking this version. |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | **10.0.2** | Low — dev-time tool only |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | **3.0.114** | Low — build-time tool only |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | *(resolver found no net10-specific recommendation — tool is TFM-agnostic; verify latest stable on nuget.org during execution)* | Low |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | **8.23.0** | Medium — major bump; `JwtSecurityTokenHandler` usage in `IdentityTokenClaimService` should be reviewed against 8.x changes |
| Swashbuckle.AspNetCore(.SwaggerUI/.Annotations) | 6.5.0 | **10.3.0** | **High** — 4 majors; SwaggerGen/SwaggerUI configuration API has changed across these versions, re-verify `AddSwaggerGen` setup in `PublicApi` |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 (unchanged) | None |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | **10.0.12** | Low |
| Microsoft.NET.Test.Sdk | 17.9.0 | **18.10.1** | Low |
| xunit | 2.7.0 | **2.9.3** | Low — stays on 2.x line (no v3 migration forced) |
| xunit.runner.visualstudio | 2.5.6 | **4.0.1** | Low |
| xunit.runner.console | 2.7.0 | **2.9.3** | Low |
| NSubstitute | 5.1.0 | **6.2.0** | Low–Medium — major bump, spot-check for removed APIs |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 (unchanged) | None |
| MSTest.TestAdapter / MSTest.TestFramework | 3.2.2 | **4.5.1** | Medium — major bump, verify `PublicApiIntegrationTests` still discovers/runs |
| coverlet.collector | 6.0.2 | **10.1.0** | Low |
| Microsoft.AspNetCore.Mvc | 2.2.0 (declared in `Directory.Packages.props`, **not referenced by any project** via `PackageReference`) | N/A — recommend removing this stale/dead entry | None (dead code cleanup) |

## Risks Identified

1. **MediatR 12 → 14** (High): largest semantic-version jump in the set. Must confirm `builder.Services.AddMediatR(...)` registration call and handler base interfaces (`IRequestHandler<,>`, `INotificationHandler<>`) still compile; check MediatR's current licensing terms are acceptable for this repo before finalizing.
2. **Swashbuckle.AspNetCore 6 → 10** (High): `AddSwaggerGen`/`UseSwagger`/`UseSwaggerUI` call sites in `PublicApi` must be re-validated; annotation package API may have changed.
3. **EF Core 8 → 10** (Medium): `Infrastructure` DbContext, migrations, and `Microsoft.EntityFrameworkCore.InMemory`-based test fixtures should be rebuilt/retested; watch for provider behavior changes.
4. **Ardalis.* family majors** (Medium): `Ardalis.Result` (7→10), `Ardalis.Specification`(.EntityFrameworkCore) (7→9), `Ardalis.GuardClauses` (4→5) are all core to `ApplicationCore`'s domain model — compile and run unit tests immediately after bumping.
5. **MSTest 3 → 4** (Medium): only affects `PublicApiIntegrationTests`.
6. **No security vulnerabilities** were identified by resolver output; risk list above is about breaking API changes, not CVEs.

## Projects In Scope

| Project | Path | Type | Current TFM | Target TFM |
|---|---|---|---|---|
| BlazorShared | src/BlazorShared/BlazorShared.csproj | Class library | net8.0 | net10.0 |
| ApplicationCore | src/ApplicationCore/ApplicationCore.csproj | Class library | net8.0 | net10.0 |
| BlazorAdmin | src/BlazorAdmin/BlazorAdmin.csproj | Blazor WebAssembly | net8.0 | net10.0 |
| Infrastructure | src/Infrastructure/Infrastructure.csproj | Class library | net8.0 | net10.0 |
| PublicApi | src/PublicApi/PublicApi.csproj | ASP.NET Core Web API | net8.0 | net10.0 |
| Web | src/Web/Web.csproj | ASP.NET Core MVC/Razor Pages | net8.0 | net10.0 |
| UnitTests | tests/UnitTests/UnitTests.csproj | xUnit test | net8.0 | net10.0 |
| IntegrationTests | tests/IntegrationTests/IntegrationTests.csproj | xUnit test | net8.0 | net10.0 |
| FunctionalTests | tests/FunctionalTests/FunctionalTests.csproj | xUnit test | net8.0 | net10.0 |
| PublicApiIntegrationTests | tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj | MSTest test | net8.0 | net10.0 |

## Strategy Signal Summary (for Planning stage)

- All projects already on `net6.0+` (net8.0), all SDK-style, CPM already enabled.
- 10 projects total (≤15), 3-tier dependency depth, no high-risk cross-stack migrations
  (no .NET Framework, no `System.Web`, no `Startup.cs`).
- → Classified **Simple** upgrade; only the **Upgrade Strategy** option is applicable
  (Test Coverage not requested — `generateNewUnitTests=false` per task success criteria).
- Strategy default-logic match: "All projects on net6.0+" + "≤15 projects + ≤2 high-risk
  migrations + ≤3-tier depth + no CI-green constraint" → **All-at-Once**.
