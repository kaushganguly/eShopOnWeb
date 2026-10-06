# .NET Version Upgrade Plan — eShopOnWeb

## Overview

**Target**: eShopOnWeb solution — upgrade from net8.0 to net10.0
**Scope**: 10 projects (small solution), Central Package Management enabled; single
`Directory.Packages.props` property controls the TFM for the whole solution

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects (≤15), all already on net8.0 (net6.0+), CPM makes the TFM a single-property change, no CI-green or multi-team constraint |
| Test Coverage | Skip | Task requirement explicitly states `generateNewUnitTests=false` |

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all already on net8.0 (net6.0+), Central Package Management
defines the TFM in one place (`Directory.Packages.props`), no .NET Framework boundary, no
CI-green or multi-team constraint signaled.

## Tasks

### 01-prerequisites: Verify SDK toolchain and update global.json

Confirm the .NET 10 SDK is installed and available in this environment (`dotnet --list-sdks`),
then update `global.json` so the pinned SDK version moves from `8.0.x` to `10.0.x` (keeping
`rollForward: latestFeature` or adjusting as needed). This is a prerequisite for every
subsequent task — the solution cannot build against `net10.0` TFMs until the SDK pin matches.
No project files change in this task.

**Done when**: `global.json` specifies a `10.0.x` SDK version, and `dotnet --version` run from
the repo root resolves to a .NET 10 SDK.

---

### 02-upgrade-all-projects: Upgrade TFM and package references across the solution, fix breaking changes

Upgrade all 10 projects (`BlazorShared`, `ApplicationCore`, `Infrastructure`, `BlazorAdmin`,
`Web`, `PublicApi`, `UnitTests`, `PublicApiIntegrationTests`, `FunctionalTests`,
`IntegrationTests`) together in a single pass, per the All-at-Once strategy. Scope:

Change the single `<TargetFramework>` property in `Directory.Packages.props` from `net8.0`
to `net10.0` (the only place the TFM is declared in the whole solution — confirmed during
assessment), and bump the `AspNetVersion`, `EntityFramworkCoreVersion`,
`VSCodeGeneratorVersion`, and `SystemExtensionVersion` alias properties to their `10.0.x`
equivalents. Bump the individually-versioned `PackageVersion` entries per the assessment's
package matrix, notably: `System.Text.Json` (resolves 2 High CVEs), `Azure.Identity` (resolves
2 Moderate CVEs), `System.IdentityModel.Tokens.Jwt`, the `Ardalis.*` family (`Specification`,
`Specification.EntityFrameworkCore`, `Result`, `GuardClauses`), `MediatR`,
`Swashbuckle.AspNetCore` (+ SwaggerUI/Annotations), `FluentValidation`,
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`, `Microsoft.Web.LibraryManager.Build`,
and the test packages (`Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`,
`xunit.runner.console`, `NSubstitute`, `MSTest.TestAdapter`/`TestFramework`,
`coverlet.collector`). Do **not** bump `AutoMapper.Extensions.Microsoft.DependencyInjection`
past `12.0.1` — later versions require a commercial license; this is an intentional pin.

Restore and build the full solution; fix all resulting compilation errors and breaking-API
usages in one bounded pass. Known risk areas from the assessment: multi-major-version jumps in
the `Ardalis.*` family, `MediatR`, `Swashbuckle.AspNetCore`, `FluentValidation`, and
`NSubstitute` may have source-breaking API changes; `BlazorInputFile` (abandoned community
package, version unchanged) must be verified to still function under net10 Blazor
WebAssembly — if it does not, replace usages with the built-in
`Microsoft.AspNetCore.Components.Forms.InputFile`/`IBrowserFile`. Also fix all pre-existing
build warnings in touched projects (not just newly introduced ones): the `SYSLIB0051` obsolete
serialization-constructor warning in `ApplicationCore`, and the `xUnit2013` assertion-style
warnings in `UnitTests` and `IntegrationTests`.

**Done when**: `dotnet build eShopOnWeb.sln -c Release` succeeds with 0 errors and 0 warnings
across all 10 projects, all projects report `net10.0` as their effective TFM, and
`AutoMapper.Extensions.Microsoft.DependencyInjection` remains pinned at `12.0.1`.

---

### 03-final-validation: Run full test suite and confirm security compliance

Run all four test projects (`UnitTests`, `IntegrationTests`, `FunctionalTests`,
`PublicApiIntegrationTests`) against the upgraded net10.0 build and fix any failures caused by
the upgrade (EF Core InMemory provider behavior shifts across major versions, Ardalis/MediatR
API changes, etc.) without weakening test assertions. Re-run a vulnerability scan across the
solution to confirm the previously-identified `System.Text.Json`/`Azure.Identity` CVEs are
resolved and no new vulnerable packages were introduced.

**Done when**: all tests in all 4 test projects pass, and a vulnerability scan reports no known
vulnerabilities.
