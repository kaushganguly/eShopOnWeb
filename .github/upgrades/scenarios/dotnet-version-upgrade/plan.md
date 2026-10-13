# Upgrade Plan — eShopOnWeb: .NET 8.0 → .NET 10.0

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | All-at-Once | 10 projects (≤15), all already net8.0 (net6.0+), 3-tier dependency depth, no high-risk cross-stack migrations, no CI-green constraint |
| Test Coverage | Skip | Explicit user success criteria `generateNewUnitTests=false` |

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0 (modern, net6.0+), centrally managed single
`<TargetFramework>` property via CPM (`Directory.Packages.props`), clear 3-tier dependency
structure, no .NET Framework projects, no CI-green constraint.

## Projects In Scope

**Libraries**: BlazorShared, ApplicationCore, Infrastructure
**Applications**: BlazorAdmin (Blazor WASM), PublicApi (ASP.NET Core Web API), Web (ASP.NET Core MVC/Razor Pages)
**Tests**: UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests

## Tasks

### 01-prerequisites: Verify toolchain and bump the centrally managed target framework

Verify the .NET 10 SDK is installed and compatible, then update `global.json` (`sdk.version`
from `8.0.x` to a `10.0.x` SDK, keeping `rollForward: latestFeature`) and the single centrally
managed `<TargetFramework>` property in `Directory.Packages.props` from `net8.0` to `net10.0`.
Because this repo uses NuGet Central Package Management with one shared `TargetFramework`
property, this is the single edit that retargets all 10 projects at once — no per-project
`<TargetFramework>` elements exist to change individually. Also remove the stale, unreferenced
`Microsoft.AspNetCore.Mvc` version `2.2.0` entry from `Directory.Packages.props` (confirmed via
repository search to have zero `PackageReference` consumers — dead entry left over from an
earlier package cleanup, and pinned to a version far too old to resolve against net10 anyway).

**Done when**: `global.json` targets a .NET 10 SDK; `Directory.Packages.props`
`<TargetFramework>` is `net10.0`; `dotnet --list-sdks` / `validate_dotnet_sdk_installation`
confirms a compatible SDK is available; the stale `Microsoft.AspNetCore.Mvc` PackageVersion
entry is removed.

### 02-core-libraries: Upgrade ApplicationCore, BlazorShared, Infrastructure, BlazorAdmin packages and fix breaking changes

Bump package versions in `Directory.Packages.props` for the packages consumed by these four
projects and fix any resulting compile errors. Per the assessment, the highest-risk bumps here
are **Ardalis.Result** (7.0.0→10.1.0, 3 majors), **Ardalis.Specification** /
**Ardalis.Specification.EntityFrameworkCore** (7.0.0→9.3.1, 2 majors), **Ardalis.GuardClauses**
(4.0.1→5.0.0), and the EF Core packages (**Microsoft.EntityFrameworkCore.InMemory/.SqlServer/.Tools**,
8.0.2→10.0.12) used by `Infrastructure`. These are core to the domain model in `ApplicationCore`
(specifications, guard clauses, `Result<T>` return types) and to data access in `Infrastructure`
(DbContext, migrations, Identity EF store) — review their current usage before bumping, since
`ApplicationCore` and `Infrastructure` are depended on by every other project in the solution.
`BlazorAdmin`'s ASP.NET Core Components/Identity packages move in lockstep with the shared
`AspNetVersion` property (8.0.2→10.0.12) with low expected risk. Also apply applicable C#
language-version modernizations surfaced while touching these files (`#skill:modernizing-csharp-version`).

**Done when**: `ApplicationCore`, `BlazorShared`, `Infrastructure`, and `BlazorAdmin` each build
with 0 errors and 0 warnings on `net10.0` against the updated package versions.

### 03-web-and-api-apps: Upgrade Web and PublicApi packages and fix breaking changes

Bump package versions for `Web` and `PublicApi` and fix resulting compile/runtime issues. The
two highest-risk package bumps in the whole solution land here: **MediatR** 12.0.1→14.2.0 (two
majors — verify `AddMediatR` registration call and `IRequestHandler<,>`/`INotificationHandler<>`
usage still compile, and confirm current MediatR licensing terms are acceptable) and
**Swashbuckle.AspNetCore** (+`.SwaggerUI`/`.Annotations`) 6.5.0→10.3.0 (four majors — re-verify
`AddSwaggerGen`/`UseSwagger`/`UseSwaggerUI` configuration in `PublicApi/Program.cs`). Also bump
**System.IdentityModel.Tokens.Jwt** 7.3.1→8.23.0 (used in `Infrastructure/Identity/IdentityTokenClaimService.cs`
via `JwtSecurityTokenHandler`, consumed from `PublicApi`'s JWT bearer auth), **FluentValidation**
11.9.0→12.2.0 (via `BlazorShared`, surfaced in `Web`), and the remaining `AspNetVersion`-linked
packages, **Azure.Identity**, **Azure.Extensions.AspNetCore.Configuration.Secrets**, dev-time
tools (`Microsoft.VisualStudio.Web.CodeGeneration.Design`, `Microsoft.Web.LibraryManager.Build`,
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — verify latest stable if the resolver
had no net10-specific recommendation). Research MediatR and Swashbuckle breaking-change notes
before editing call sites.

**Done when**: `Web` and `PublicApi` each build with 0 errors and 0 warnings on `net10.0`;
`PublicApi`'s Swagger UI configuration and JWT bearer authentication wiring compile and are
verified by the existing `PublicApiIntegrationTests`/`FunctionalTests`.

### 04-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests and restore a green test suite

Bump test-infrastructure packages and fix any resulting compile/runtime issues:
**Microsoft.NET.Test.Sdk** 17.9.0→18.10.1, **xunit** 2.7.0→2.9.3 (stays on the 2.x line),
**xunit.runner.visualstudio** 2.5.6→4.0.1, **xunit.runner.console** 2.7.0→2.9.3, **NSubstitute**
5.1.0→6.2.0 (major bump — spot-check for removed/renamed APIs across substitute setups),
**Microsoft.AspNetCore.Mvc.Testing** 8.0.2→10.0.12 (`WebApplicationFactory`-based tests in
`FunctionalTests`/`PublicApiIntegrationTests`), **MSTest.TestAdapter**/**MSTest.TestFramework**
3.2.2→4.5.1 (major bump, `PublicApiIntegrationTests` only), and **coverlet.collector**
6.0.2→10.1.0. This task depends on `02-core-libraries` and `03-web-and-api-apps` completing
first since all four test projects reference `ApplicationCore`, `Web`, `Infrastructure`, and/or
`PublicApi`. Run the full test suite after upgrading and fix any failing tests caused by the
underlying package/API changes (not by writing new tests — `generateNewUnitTests=false`).

**Done when**: all four test projects build with 0 errors/warnings on `net10.0`, and
`dotnet test` passes 100% of existing tests across the solution.

### 05-final-validation: Full solution build, full test run, and security/compliance check

Build the entire solution (`eShopOnWeb.sln`) from a clean state and confirm 0 errors/warnings
across all 10 projects. Run the complete test suite (`UnitTests`, `IntegrationTests`,
`FunctionalTests`, `PublicApiIntegrationTests`) and confirm 100% pass. Run a NuGet
vulnerability/security check (`dotnet list package --vulnerable --include-transitive`) across
the solution to satisfy `securityComplianceCheck=true`, and document any remaining
recommendations (e.g., packages where the resolver found no net10-specific guidance, such as
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`).

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors and 0 warnings;
`dotnet test` passes all tests; `dotnet list package --vulnerable --include-transitive`
reports no known vulnerabilities.
