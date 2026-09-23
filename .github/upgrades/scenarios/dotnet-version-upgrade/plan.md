# Upgrade Plan — eShopOnWeb: net8.0 → net10.0

## Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0 (modern), all SDK-style with Central Package
Management already enabled, low migration difficulty per assessment (🟢 across every
project), ≤ 3-tier dependency depth, no CI-green constraint. A phased/tiered approach adds
overhead without benefit for a mechanical modern-to-modern TFM bump.

## Upgrade Options

- **Strategy > Upgrade Strategy**: All-at-Once — small scope (10 projects), all modern .NET, low difficulty.
- **Compatibility > Unsupported Packages**: Resolve Inline — small number of flagged incompatible packages (7), concentrated in PublicApi/Web; researched and resolved in the same pass.
- **Compatibility > Unsupported API Handling**: Fix Inline — modern-to-modern upgrade; API issues are mechanical (ConfigurationBinder.Get/Configure<T> binary-incompatible calls, HttpContent/Uri/Exception-ctor behavioral changes) with known replacements.
- **Reliability > Test Coverage**: Skip — success criteria specifies generateNewUnitTests=false.

## Scope

All 10 projects in `eShopOnWeb.sln` / `Everything.sln`:

**Libraries**
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/BlazorShared/BlazorShared.csproj`

**Applications**
- `src/Web/Web.csproj` (ASP.NET Core MVC/Razor)
- `src/PublicApi/PublicApi.csproj` (ASP.NET Core minimal/API endpoints)
- `src/BlazorAdmin/BlazorAdmin.csproj` (Blazor WebAssembly)

**Tests**
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

Package versions are centrally managed via `Directory.Packages.props` (CPM already enabled)
— the `TargetFramework` property and `AspNetVersion`/`SystemExtensionVersion`/
`EntityFramworkCoreVersion`/`VSCodeGeneratorVersion` version props live there and drive every
project.

## Tasks

### 01-prerequisites: Verify toolchain and baseline build

Confirm the .NET 10 SDK is installed and available (`global.json`, if present, must allow
net10.0 SDKs), and capture a baseline build/test result on net8.0 before making any changes so
regressions introduced by the upgrade can be distinguished from pre-existing failures. Check
`global.json` for a pinned SDK version and update it if it constrains the SDK to a pre-.NET-10
version.

**Done when**: .NET 10 SDK is confirmed installed, `global.json` (if present) allows it, and a
baseline build/test run result for the current net8.0 solution is recorded for comparison.

### 02-upgrade-all-projects: Upgrade all projects to net10.0 and update centrally managed packages

Update the centrally managed `TargetFramework` in `Directory.Packages.props` from `net8.0` to
`net10.0`, and update every `<TargetFramework>`/`<TargetFrameworks>` entry in all 10 project
files (`ApplicationCore`, `Infrastructure`, `BlazorShared`, `Web`, `PublicApi`, `BlazorAdmin`,
`UnitTests`, `IntegrationTests`, `FunctionalTests`, `PublicApiIntegrationTests`) to net10.0 in
one atomic pass. Bump all centrally managed NuGet package versions in
`Directory.Packages.props` to versions compatible with net10.0 — this includes the
`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, and
`VSCodeGeneratorVersion` version properties (ASP.NET Core / EF Core packages should move to
the 10.x line), plus standalone packages such as `Microsoft.AspNetCore.Mvc` (currently pinned
at the very old `2.2.0` — this is a stale/incompatible pin left over from framework-era Mvc
and should be removed or corrected since ASP.NET Core MVC ships in the shared framework),
`System.Text.Json`, `System.IdentityModel.Tokens.Jwt`, `Azure.Identity`,
`Ardalis.Specification`/`Ardalis.Specification.EntityFrameworkCore`/`Ardalis.Result`,
`MediatR`, `Swashbuckle.AspNetCore*`, `NSubstitute`/`NSubstitute.Analyzers.CSharp`,
`Microsoft.NET.Test.Sdk`, `xunit`/`xunit.runner.visualstudio`/`xunit.runner.console`,
`MSTest.TestAdapter`/`MSTest.TestFramework`, and `coverlet.collector`.

Resolve the packages the assessment flagged as incompatible or deprecated (7 incompatible / 26
recommended-upgrade across the solution, concentrated in `PublicApi` and `Web`) inline —
replace with the vendor's net10.0-compatible successor version, or remove/replace packages
whose functionality now ships in the shared framework (per assessment rule `NuGet.0003`).

Fix the API breaking changes the assessment flagged for the target framework: binary
incompatible `ConfigurationBinder.Get<T>()` / `Configure<T>(IConfiguration)` /
`ConfigurationBinder.GetValue(Type, string)` call sites, the `Exception(SerializationInfo,
StreamingContext)` source-incompatible constructor usage, and behavioral changes around
`System.Net.Http.HttpContent`, `System.Uri` (including the `Uri(string)` constructor), and
`TimeSpan.FromMinutes(double)`. `Web` and `PublicApi` carry the most API issues (15 and 5
respectively per project); `FunctionalTests` carries 29 API issues (mostly `HttpContent`
behavioral-change assertions against in-memory test servers) that must be re-validated once
the target framework changes their behavior.

Restore all package dependencies and build the full solution, fixing every compilation error
in this single bounded pass (not an iterative retry loop) until the solution builds with 0
errors and 0 warnings across all 10 projects.

**Done when**: `Directory.Packages.props` targets net10.0 with all centrally managed package
versions updated to net10.0-compatible releases; every project file targets net10.0; the
solution builds with 0 errors and 0 warnings; no incompatible or deprecated packages remain
referenced.

### 03-final-validation: Run full test suite and confirm no regressions

Run the full test suite (`UnitTests`, `IntegrationTests`, `FunctionalTests`,
`PublicApiIntegrationTests`) against the upgraded net10.0 build and compare results to the
net8.0 baseline captured in task 01. Investigate and fix any test failures caused by the
framework/package upgrade (e.g., behavioral changes in `HttpContent`/`Uri` affecting
`FunctionalTests`' in-memory HTTP assertions). Confirm no known security vulnerabilities
remain in the centrally managed package set (assessment rule `NuGet.0004`).

**Done when**: All test projects pass with the same (or better) pass rate as the net8.0
baseline; no build errors or warnings remain; no security-vulnerable package versions remain
in `Directory.Packages.props`.
