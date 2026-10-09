# Plan — Upgrade eShopOnWeb to .NET 10.0

## Upgrade Options
- **Strategy**: All-at-Once
- **Test Coverage**: Skip (user explicitly requested `generateNewUnitTests=false`)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all already on .NET 8.0 (SDK-style, CPM already in place), clear
dependency structure, purely mechanical TFM bump to .NET 10.0 with no CI-green or
multi-team constraint.

## Tasks

### 01-prerequisites: Verify toolchain and update global.json

Confirm the .NET 10 SDK is available in the environment (already confirmed: 10.0.401 is
installed alongside several 10.0.x, 9.0.x, and 8.0.x SDKs) and update `global.json` from
`"version": "8.0.x"` to a `10.0.x` band compatible with the installed SDK, keeping
`"rollForward": "latestFeature"`. No project files are touched in this task — it only
verifies the toolchain and updates the SDK pin so that every later task builds against
.NET 10.

**Done when**: `global.json` targets a `10.0.x` SDK version and `dotnet --version` /
`dotnet build` resolve against an installed 10.0.x SDK without a "SDK not found" error.

### 02-central-package-management: Upgrade TargetFramework and package versions in Directory.Packages.props

Update the single `<TargetFramework>net8.0</TargetFramework>` property (defined once in
`Directory.Packages.props` and inherited by all 10 projects — this repo does not set
`TargetFramework` per-project) to `net10.0`. Bump the version "pivot" properties
(`AspNetVersion` 8.0.2→10.0.12, `SystemExtensionVersion` 8.0.0→10.0.2,
`EntityFramworkCoreVersion` 8.0.2→10.0.2, `VSCodeGeneratorVersion` 8.0.0→10.0.2) and every
individually-pinned `PackageVersion` entry to its net10.0-supported version per the
assessment's Package Upgrade Table (e.g., Ardalis.* packages, FluentValidation 11→12,
MediatR 12→14, NSubstitute 5→6, Microsoft.NET.Test.Sdk 17→18, MSTest.* 3→4,
coverlet.collector 6→10, Swashbuckle.* 6→10, System.IdentityModel.Tokens.Jwt 7→8).

Known risk flagged in the assessment: `Microsoft.EntityFrameworkCore.SqlServer` and
`Microsoft.AspNetCore.Identity.EntityFrameworkCore` resolve independently to `10.0.12`
while `Microsoft.EntityFrameworkCore.InMemory`/`.Tools` resolve to `10.0.2` for net10.0 —
do not force all four through one shared property; set each `PackageVersion` to its own
correct resolved version and verify no assembly version conflicts surface at restore/build.
Also watch `Blazored.LocalStorage` (latest 4.5.0 has no net9/net10-specific dependency
group — relies on NuGet's nearest-TFM fallback) and
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (bump 1.19.6→1.21.0; this is a
design-time-only package, not a runtime dependency, so no code impact expected).

**Done when**: `Directory.Packages.props` has `TargetFramework=net10.0` and every
`PackageVersion` entry matches the assessment's target column; `dotnet restore` on the
solution succeeds with no NU1605/NU1608 version-conflict errors.

### 03-upgrade-projects: Build, fix compilation errors, and resolve behavioral changes across all projects

With the TFM and package versions updated, run `dotnet build` (or `dotnet build
eShopOnWeb.sln`) across the whole solution and fix all resulting compilation errors and
warnings in a single bounded pass — not an iterative "rebuild, fix, rebuild again" loop.
This covers all 10 projects together (no phasing): `BlazorShared`, `ApplicationCore`,
`Infrastructure`, `BlazorAdmin` (Blazor WebAssembly — watch for WASM-hosting or
`Microsoft.AspNetCore.Components.WebAssembly.*` API changes between 8→10), `Web`
(ASP.NET Core MVC/Razor Pages, hosts the BlazorAdmin WASM app, Identity +
EF Core SqlServer), and `PublicApi` (minimal/Ardalis API endpoints, Swashbuckle, JWT
bearer auth).

Expect to encounter and resolve ASP.NET Core / EF Core 9-and-10-era breaking/behavioral
changes reactively based on actual build output — e.g., any stricter nullable model-binding
behavior, Identity cookie/auth default changes, EF Core query-translation changes, minimal
API routing adjustments, and any fallout from the `System.Text.Json` 8→10 bump. Also
specifically verify the abandoned `BlazorInputFile` package (last published 2019, pinned to
preview-era `Microsoft.AspNetCore.Components` 3.1 dependencies) still functions under
net10.0 Blazor WebAssembly; if it does not, replace its usage with the framework's built-in
`InputFile`/`IBrowserFile` (available natively since .NET 5) as a targeted code fix within
this same task rather than a separate task.

Per `#skill:` convention, this task is a direct TFM/package migration with no OData,
HTTP-contract, or shared-database side-by-side concerns — no additional skill pin applies.

**Done when**: `dotnet build` for the whole solution (all 10 projects) completes with 0
errors and 0 warnings (fix all warnings in touched projects, not just new ones).

### 04-test-projects: Upgrade and validate test projects

Update and validate the four test projects as part of the same atomic pass:
`tests/UnitTests` (xUnit, NSubstitute, references ApplicationCore + Web),
`tests/IntegrationTests` (xUnit, references Infrastructure + UnitTests),
`tests/FunctionalTests` (xUnit, `WebApplicationFactory`, references ApplicationCore,
PublicApi, Web), and `tests/PublicApiIntegrationTests` (MSTest, `WebApplicationFactory`,
references PublicApi + Web). `WebApplicationFactory`-based tests (`FunctionalTests`,
`PublicApiIntegrationTests`) are specifically sensitive to ASP.NET Core minimal-hosting and
test-host behavior changes between major versions — re-validate their fixtures
(`WebTestFixture.cs`, `ApiTestFixture.cs`, `ProgramTest.cs`) explicitly rather than assuming
they still work after a clean build.

**Done when**: `dotnet test` across all four test projects passes with 0 failing tests
(matching success criterion `passUnitTests=true`), and the projects build with 0 warnings.

### 05-final-validation: Full solution build, full test run, and security/compliance check

Run a full solution build (`dotnet build eShopOnWeb.sln`) and full test run (`dotnet test`)
as the final gate. Confirm no package in `Directory.Packages.props` is left on a version with
a known vulnerability (per `securityComplianceCheck=true` — every package was already moved
to its latest net10.0-supported version in task 02, which supersedes the .NET 8-era pins that
had historical CVEs, e.g. older `System.IdentityModel.Tokens.Jwt`/`Azure.Identity`/
`System.Text.Json` releases). Document any deferred/out-of-scope recommendations (e.g., if
`BlazorInputFile` replacement was deferred rather than completed in task 03).

**Done when**: Full solution builds with 0 errors/0 warnings, full test suite passes,
and `scenario-instructions.md` / `execution-log.md` records confirmation that the security
compliance check was satisfied.
