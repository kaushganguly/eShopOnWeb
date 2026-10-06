# .NET Upgrade Assessment — eShopOnWeb: net8.0 → net10.0

> **Note on methodology**: The automated `generate_dotnet_upgrade_assessment` tool was unavailable
> in this environment (orchestrator channel connection failure, retried repeatedly over several
> minutes). This assessment was produced manually via direct solution/project analysis,
> `dotnet restore`/`build` output, and per-package `get_supported_package_version` queries, to avoid
> blocking the upgrade.

## Solution Overview

- Solution: `eShopOnWeb.sln` (10 projects; a secondary `Everything.sln` also exists but is not in scope)
- Current target framework: **net8.0** (defined centrally, see below)
- Target framework: **net10.0** (LTS, EOL 2028-11-14)
- SDK pinned via `global.json`: `8.0.x` (rollForward: latestFeature) → must become `10.0.x`
- Central Package Management is enabled (`Directory.Packages.props`, `ManagePackageVersionsCentrally=true`)

### Target Framework Mechanism (important)

Unusually, **no project file sets `<TargetFramework>` directly**. Instead, `Directory.Packages.props`
defines a single property used nowhere else:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Grep across the repo confirms this is the **only** place `TargetFramework` is set. This means the
entire solution's TFM can be bumped by changing **one line** in `Directory.Packages.props`, plus the
two version-alias properties also defined there (`AspNetVersion`, `EntityFramworkCoreVersion`) that
drive most `Microsoft.AspNetCore.*` / `Microsoft.EntityFrameworkCore.*` package versions.

### Projects In Scope (topological order)

| # | Project | Type | Notes |
|---|---------|------|-------|
| 1 | `src/BlazorShared/BlazorShared.csproj` | Microsoft.NET.Sdk (classlib) | Shared DTOs/validators |
| 2 | `src/ApplicationCore/ApplicationCore.csproj` | Microsoft.NET.Sdk (classlib) | Domain/business logic |
| 3 | `src/Infrastructure/Infrastructure.csproj` | Microsoft.NET.Sdk (classlib) | EF Core, Identity |
| 4 | `src/BlazorAdmin/BlazorAdmin.csproj` | Microsoft.NET.Sdk.BlazorWebAssembly | Admin SPA |
| 5 | `src/Web/Web.csproj` | Microsoft.NET.Sdk.Web | Main MVC/Razor Pages app, hosts BlazorAdmin |
| 6 | `src/PublicApi/PublicApi.csproj` | Microsoft.NET.Sdk.Web | Public Web API (minimal API + Ardalis endpoints) |
| 7 | `tests/UnitTests/UnitTests.csproj` | Microsoft.NET.Sdk (test) | xUnit |
| 8 | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Microsoft.NET.Sdk (test) | MSTest + WebApplicationFactory |
| 9 | `tests/FunctionalTests/FunctionalTests.csproj` | Microsoft.NET.Sdk (test) | xUnit + WebApplicationFactory |
| 10 | `tests/IntegrationTests/IntegrationTests.csproj` | Microsoft.NET.Sdk (test) | xUnit, NSubstitute |

No legacy (non-SDK-style) projects were found — SDK-style conversion is not needed.

## Baseline Build Status (before upgrade)

`dotnet restore` + `dotnet build -c Release` on the current `net8.0` baseline: **succeeds**, 0 errors,
13 pre-existing warnings (see below). This confirms the starting point is healthy before changes.

### Pre-existing warnings (not introduced by this upgrade, but in-scope projects — must be fixed per workflow rules)

- `SYSLIB0051` in `ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — obsolete
  serialization-constructor `Exception(SerializationInfo, StreamingContext)`.
- `xUnit2013` (×3) in `UnitTests` and `IntegrationTests` — `Assert.Equal(count, collection.Count)`
  should be `Assert.Empty`/`Assert.Single`.

## Security Vulnerabilities (NU1902/NU1903) — will be fixed by this upgrade

| Package | Current Version | Severity | Advisory |
|---------|-----------------|----------|----------|
| `System.Text.Json` | 8.0.3 | High | GHSA-8g4q-xg66-9fp4 |
| `System.Text.Json` | 8.0.3 | High | GHSA-hh2w-p6rv-4g7w |
| `Azure.Identity` | 1.10.4 | Moderate | GHSA-m5vv-6r4h-3vj9 |
| `Azure.Identity` | 1.10.4 | Moderate | GHSA-wvxc-855f-jvrv |

These are addressed automatically by upgrading to the net10-compatible versions below.

## Package Upgrade Matrix (current → proposed, for net10.0)

Resolved via `get_supported_package_version` against net10.0 for each consuming project.

| Package | Current | Proposed | Risk |
|---|---|---|---|
| `Microsoft.AspNetCore.*` (Components.*, Identity.*, Authentication.JwtBearer, Diagnostics.EntityFrameworkCore) | 8.0.2 (`$(AspNetVersion)`) | **10.0.12** | Low — first-party, in lockstep with runtime |
| `Microsoft.EntityFrameworkCore.*` (SqlServer, InMemory, Tools) | 8.0.2 (`$(EntityFramworkCoreVersion)`) | **10.0.12** | Medium — EF Core 9/10 have minor behavior changes (see risks) |
| `Microsoft.Extensions.Identity.Core` | 8.0.2 | **10.0.12** | Low |
| `Microsoft.Extensions.Logging.Configuration` | 8.0.0 (`$(SystemExtensionVersion)`) | **10.0.12** | Low |
| `System.Net.Http.Json` | 8.0.0 | **10.0.12** | Low |
| `System.Text.Json` | 8.0.3 | **10.0.12** | Low — also resolves CVEs |
| `System.Security.Claims` | 4.3.0 | 4.3.0 (unchanged) | None — legacy compat shim, harmless |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | **8.23.0** | Low-Medium — major version bump; watch for Microsoft.IdentityModel breaking changes |
| `Ardalis.Specification` | 7.0.0 | **9.3.1** | Medium — 2 major versions; check spec evaluator API |
| `Ardalis.Specification.EntityFrameworkCore` | 7.0.0 | **9.3.1** | Medium — keep in lockstep with `Ardalis.Specification` |
| `Ardalis.Result` | 7.0.0 | **10.1.0** | Medium — 3 major versions; check `Result<T>` API surface used in endpoints |
| `Ardalis.GuardClauses` | 4.0.1 | **5.0.0** | Low-Medium — major version |
| `Ardalis.ApiEndpoints` | 4.1.0 | 4.1.0 (unchanged) | None |
| `Ardalis.ListStartupServices` | 1.1.4 | 1.1.4 (unchanged) | None |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | 12.0.1 (unchanged) | **Note**: AutoMapper 13+ moved to a commercial license (Eazy license) — staying on 12.0.1 (last free/MIT version) is intentional and correct; do not bump major version |
| `MediatR` | 12.0.1 | **14.2.0** | Medium-High — 2 major versions; MediatR 12→13 dropped `IRequestHandler` auto-registration defaults changed and introduced licensing notice (community edition still free); verify DI registration (`AddMediatR`) call-site syntax |
| `MinimalApi.Endpoint` | 1.3.0 | 1.3.0 (unchanged) | None |
| `Swashbuckle.AspNetCore` (+ `.SwaggerUI`, `.Annotations`) | 6.5.0 | **10.2.3** | Medium — major jump; verify Swagger generation still works with minimal API endpoints |
| `FluentValidation` | 11.9.0 | **12.1.1** | Low-Medium — major version; FluentValidation 12 removed some deprecated APIs |
| `Blazored.LocalStorage` | 4.5.0 | 4.5.0 (unchanged — already latest; package appears unmaintained since ~2023, but is netstandard2.0-based and should load fine on net10 Blazor WASM) | Low |
| `BlazorInputFile` | 0.2.0 | 0.2.0 (unchanged — already latest; **unmaintained since ~2019**) | **High** — pre-dates Blazor's built-in `InputFile`/`IBrowserFile`; verify it still functions under net10 Blazor WASM runtime, otherwise replace usages with `Microsoft.AspNetCore.Components.Forms.InputFile` |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.0 (`$(VSCodeGeneratorVersion)`) | **10.0.2** | Low — scaffolding tool, not shipped |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | 1.19.6 | **1.23.0** (latest stable; tool reported no "net10" specific version but newer stable exists on NuGet) | Low — VS/Docker tooling import, not TFM-sensitive |
| `Microsoft.Web.LibraryManager.Build` | 2.1.175 | **3.0.114** | Low |
| `BuildBundlerMinifier` | 3.2.449 | 3.2.449 (unchanged) | None |
| `Azure.Extensions.AspNetCore.Configuration.Secrets` | 1.3.1 | **1.5.2** | Low — also brings in updated `Azure.Identity` |
| `Azure.Identity` | 1.10.4 | **1.21.0** | Low — resolves CVEs above |
| **Test packages** | | | |
| `Microsoft.NET.Test.Sdk` | 17.9.0 | **18.10.1** | Low |
| `xunit` | 2.7.0 | **2.9.3** | Low (staying on xUnit v2, not migrating to v3) |
| `xunit.runner.visualstudio` | 2.5.6 | **4.0.0** | Low |
| `xunit.runner.console` | 2.7.0 | **2.9.3** | Low |
| `NSubstitute` | 5.1.0 | **6.2.0** | Low-Medium — major version, check mocking syntax still compiles |
| `NSubstitute.Analyzers.CSharp` | 1.0.17 | 1.0.17 (unchanged) | None |
| `MSTest.TestAdapter` / `MSTest.TestFramework` | 3.2.2 | **4.4.1** | Low |
| `coverlet.collector` | 6.0.2 | **10.1.0** | Low |

## Risks & Issues Identified

1. **BlazorInputFile package (High)** — abandoned community package pre-dating Blazor's native file
   input support. Must verify it still works at runtime under net10's Blazor WASM; if not, plan a
   fallback to `InputFile`/`IBrowserFile` from `Microsoft.AspNetCore.Components.Forms`.
2. **Multi-major-version package bumps** (Ardalis.* family, MediatR, Swashbuckle, FluentValidation,
   NSubstitute) — each skips 2+ major versions. Breaking API changes are possible; must build and fix
   per project after bumping, not assume source compatibility.
3. **AutoMapper licensing** — must NOT bump past 12.0.1 (13+ requires a commercial license). Keep
   pinned.
4. **`global.json` SDK pin** — currently `8.0.x`; must move to `10.0.x` or build will use an old SDK
   even after project TFM changes (`10.0.401` SDK is installed and available in this environment).
5. **EF Core InMemory provider version skew** — moving 3 major EF Core versions; InMemory provider
   behavior (e.g., validation of transactions, warnings-as-errors defaults) may shift; integration/unit
   tests relying on InMemory must be re-verified.
6. **Docker/container tooling** (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`,
   `docker-compose.dcproj`) — out of scope for functional app code, but version bump should be
   validated doesn't break `docker-compose` build definitions referenced at the solution level.
7. **Pre-existing build warnings** (SYSLIB0051, xUnit2013 ×3) in touched projects (ApplicationCore,
   UnitTests, IntegrationTests) must be fixed as part of this upgrade per workflow quality rules (fix
   all warnings in modified projects, not just newly introduced ones).

## Test Coverage (for post-upgrade validation)

Four test projects cover: domain/unit tests (`UnitTests`), repository/EF Core integration
(`IntegrationTests`), Web/PublicApi functional tests via `WebApplicationFactory` (`FunctionalTests`),
and PublicApi integration tests via MSTest (`PublicApiIntegrationTests`). All must pass after upgrade
(`passUnitTests=true` success criterion).

## Success Criteria Mapping

- `passBuild=true` → solution must build with 0 errors on net10.0 (all 10 projects)
- `generateNewUnitTests=false` → no new tests will be authored; existing tests must be preserved and pass
- `passUnitTests=true` → all 4 test projects must pass after upgrade
- `securityComplianceCheck=true` → the 4 known vulnerabilities (System.Text.Json, Azure.Identity) must be resolved by the package bumps above

## Recommended Strategy

Single-wave, whole-solution upgrade (small solution, strong central package management, single TFM
property to change) rather than phased/per-project waves:

1. Update `global.json` SDK version to `10.0.x`.
2. Update `Directory.Packages.props`: `TargetFramework` → `net10.0`, bump `AspNetVersion` /
   `EntityFramworkCoreVersion` / `VSCodeGeneratorVersion` / `SystemExtensionVersion` aliases to
   `10.0.x`, and bump the individually-versioned packages per the matrix above.
3. Restore, build, and fix compilation/breaking-API issues project by project in topological order.
4. Fix pre-existing warnings in touched projects.
5. Run all 4 test projects; fix failures.
6. Re-run vulnerability scan (`dotnet list package --vulnerable`) to confirm compliance.
