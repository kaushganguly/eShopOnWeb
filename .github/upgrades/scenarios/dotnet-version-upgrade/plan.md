# Plan: eShopOnWeb .NET 8.0 → .NET 10.0 Upgrade

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | **All-at-Once** | 10 projects, all net8.0+/SDK-style, ≤3-tier dependency graph, no CI-green constraint — default per strategy decision table for modern-to-modern upgrades with ≤15 projects |
| Test Coverage | **Skip** | Default (opt-in only); matches explicit user success criterion `generateNewUnitTests=false` |

### Selected Strategy
**All-At-Once** — All 10 projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all currently on .NET 8.0, SDK-style, CPM-enabled, shallow
(≤3-tier) dependency graph, no CI-green constraint — a straightforward TFM + package bump
with mechanical code fixes, better suited to one atomic pass than incremental tiers.

## Projects (single group — no tiers/phases)

**Applications**
- `src/Web/Web.csproj` (ASP.NET Core MVC/Razor host, also serves BlazorAdmin)
- `src/PublicApi/PublicApi.csproj` (ASP.NET Core minimal/REST API)
- `src/BlazorAdmin/BlazorAdmin.csproj` (Blazor WebAssembly client)

**Libraries**
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/BlazorShared/BlazorShared.csproj`

**Tests**
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

All 10 projects are already SDK-style — no SDK-style conversion task needed.

## Tasks

### 01-prerequisites: Verify toolchain and update global.json / central TFM


Verify the .NET 10.0 SDK is installed and update `global.json` (currently pinned to
`8.0.x` with `rollForward: latestFeature`) to target the installed `10.0.x` SDK. Update
the centrally-managed `<TargetFramework>` property in `Directory.Packages.props` from
`net8.0` to `net10.0` — this single change propagates to all 10 projects since none of
them declare a project-level `TargetFramework` (confirmed during assessment; CPM's
`Directory.Packages.props` is implicitly imported like a `Directory.Build.props`). Do not
touch package versions in this task — that is covered by task 02.

**Done when**: .NET 10 SDK is confirmed installed/available, `global.json` targets
`10.0.x`, and `Directory.Packages.props` declares `net10.0` as the shared target
framework.

### 02-upgrade-all-projects: Bump packages and fix build/code across the solution

Update every `PackageVersion` entry in `Directory.Packages.props` to the latest version
supported on `net10.0`, per the assessment's package table — including the shared
`AspNetVersion` (8.0.2→10.0.12), `EntityFramworkCoreVersion` (8.0.2→10.0.12), and
`SystemExtensionVersion` (8.0.0→10.0.12) MSBuild properties that drive multiple packages
at once, plus individually-versioned packages (Ardalis.* family, AutoMapper.Extensions.*,
Azure.Identity/Azure.Extensions.AspNetCore.Configuration.Secrets, FluentValidation,
Swashbuckle.* family, System.IdentityModel.Tokens.Jwt, test tooling: Microsoft.NET.Test.Sdk,
xunit/xunit.runner.visualstudio, MSTest.TestAdapter/TestFramework, coverlet.collector,
NSubstitute/NSubstitute.Analyzers.CSharp).

Remove the unmaintained `BlazorInputFile` package reference and its `@using` in
`src/BlazorAdmin/_Imports.razor` — confirmed unused anywhere in the codebase (only
imported, never referenced), and it has no net10.0-compatible build. Remove the legacy
`<DotNetCliToolReference Include="dotnet-xunit" .../>` entry from
`tests/FunctionalTests/FunctionalTests.csproj` — this mechanism is unsupported in modern
SDK-style projects. Confirm whether the orphan `Microsoft.AspNetCore.Mvc` 2.2.0
`PackageVersion` entry in `Directory.Packages.props` is referenced by any project; remove
it if not, since it predates ASP.NET Core's current package layout and could conflict
with net10.0 resolution.

Restore all packages, then build the full solution and fix all resulting compilation
errors and warnings in one bounded pass — including the AutoMapper 12→14 major-version
registration API (verify `AddAutoMapper(...)` calls in `Web`/`PublicApi` startup code
still compile) and the Swashbuckle 6.5→10.x major jump (verify `AddSwaggerGen` /
`UseSwagger` / `UseSwaggerUI` / `[SwaggerOperation]` usages in `PublicApi`). Pay particular
attention to the Blazor WebAssembly hosting pipeline in `BlazorAdmin` + `Web` (static web
assets / `Microsoft.AspNetCore.Components.WebAssembly.Server` wiring), since Blazor's
hosting model evolved across net8→net9→net10.

Also bump the .NET base image tags (`mcr.microsoft.com/dotnet/aspnet:8.0`,
`.../sdk:8.0`, etc.) in `docker-compose.yml`, `docker-compose.override.yml`, and any
per-project `Dockerfile`s to the `10.0` equivalents, so container builds stay consistent
with the new TFM and the security-compliance check isn't undermined by stale images.

**Done when**: `dotnet build` on `eShopOnWeb.sln` succeeds with 0 errors and 0 warnings
across all 10 projects, all package versions in `Directory.Packages.props` are current
net10.0-supported versions (or explicitly justified exceptions noted), no references to
`BlazorInputFile` or `DotNetCliToolReference` remain, and Docker base images reference
`10.0` tags.

### 03-final-validation: Run full test suite and close out the upgrade

Run all four test projects (`UnitTests`, `IntegrationTests`, `FunctionalTests`,
`PublicApiIntegrationTests`) against the upgraded solution and fix any failures caused
by the framework/package bumps (behavioral changes in EF Core, AutoMapper, Swashbuckle,
ASP.NET Core Identity, JWT handling, etc.) — per `generateNewUnitTests=false`, fix
existing tests only, do not author new test files. Re-verify the full solution build is
still clean after test fixes. Document any deferred recommendations (e.g., package
versions that had no exact net10.0-targeted release and were pinned to the closest
supported version, or dev-tooling packages like `Microsoft.Web.LibraryManager.Build` /
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` with no net10-specific listing
found).

**Done when**: All test projects pass (`dotnet test` green across the solution), the
solution builds with 0 errors/0 warnings, and `scenario-instructions.md` / task
progress notes capture any deferred/exception items for future follow-up.
