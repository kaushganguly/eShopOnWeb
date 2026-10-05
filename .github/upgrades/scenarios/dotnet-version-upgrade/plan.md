# .NET Upgrade Plan — eShopOnWeb: net8.0 → net10.0

## Upgrade Options

**Strategy**
- **Upgrade Strategy**: All-at-Once — 10 SDK-style projects, all already on net8.0 (modern-to-modern),
  no .NET Framework, no CI-green constraint, single-session autonomous run; small scope with
  centrally-managed packages makes an atomic TFM bump the lowest-overhead approach.
  - Alternative — Top-Down: would stage entry-point apps first and multi-target libraries;
    unnecessary overhead for a 10-project solution with centralized package management.

**Selected Strategy**: All-At-Once — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0, clear dependency structure, centrally managed
package versions via `Directory.Packages.props`.

## Projects (single group — no tiers/phases)

**Libraries**
- `src/BlazorShared/BlazorShared.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`

**Applications**
- `src/BlazorAdmin/BlazorAdmin.csproj` (Blazor WebAssembly)
- `src/Web/Web.csproj` (ASP.NET Core MVC/Razor, hosts BlazorAdmin)
- `src/PublicApi/PublicApi.csproj` (ASP.NET Core minimal API/MVC)

**Tests**
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

## Tasks

### 01-prerequisites: Verify toolchain and update SDK pin

Verify the .NET 10 SDK is installed and update `global.json` from `8.0.x` to `10.0.x` (keep
`rollForward: latestFeature`). Confirm `dotnet --list-sdks` shows a compatible 10.x SDK before
any project file changes begin. This is a prerequisite gate for every other task — if the SDK is
missing, stop and report rather than editing project files against an SDK that cannot build them.

**Done when**: `global.json` targets `10.0.x` and `dotnet --list-sdks` confirms a matching SDK is
installed; `dotnet --info` runs cleanly from the repo root.

### 02-upgrade-solution: Upgrade all projects to net10.0 and update packages

Upgrade all 10 projects (`BlazorShared`, `ApplicationCore`, `Infrastructure`, `BlazorAdmin`, `Web`,
`PublicApi`, `UnitTests`, `IntegrationTests`, `FunctionalTests`, `PublicApiIntegrationTests`) from
net8.0 to net10.0 in a single atomic pass. None of the projects override the central
`TargetFramework`, so the primary change is in `Directory.Packages.props`
(`TargetFramework`, `AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`,
`VSCodeGeneratorVersion` all → net10.0/10.0.12/10.0.2 as assessed) plus the per-package version
bumps listed in `assessment.md` section 3 (Ardalis.* family, FluentValidation, MediatR,
Swashbuckle.*, NSubstitute, xunit/MSTest/coverlet test stack, System.Text.Json, Azure.Identity,
System.IdentityModel.Tokens.Jwt, Microsoft.Web.LibraryManager.Build,
Microsoft.VisualStudio.Azure.Containers.Tools.Targets). Remove the unused
`Microsoft.AspNetCore.Mvc` 2.2.0 central `PackageVersion` entry (dead weight, not referenced by any
project). This also resolves the two confirmed security vulnerabilities
(`Azure.Identity` 1.10.4, `System.Text.Json` 8.0.3).

Known risks to research/watch for while executing: (1) MediatR 12→14 commercial licensing change
(proceed per Automatic-mode default; verify no behavioral/API break for the
`IRequest`/`INotificationHandler` usage in `ApplicationCore`/`Web`/`PublicApi`); (2) `BlazorInputFile`
0.2.0 is unmaintained (last published 2019) and used in `BlazorAdmin` for file uploads — if it
fails to build/run under net10 Blazor WebAssembly, fall back to the built-in
`Microsoft.AspNetCore.Components.Forms.InputFile` component for the affected component(s); (3) EF
Core 10 / ASP.NET Core 10 / Identity breaking changes affecting `Infrastructure`, `Web`,
`PublicApi` — resolve via the single bounded build-fix pass below; (4) update
`src/Web/Dockerfile` and `src/PublicApi/Dockerfile` base images from `8.0` to `10.0` tags, and
`.github/workflows/dotnetcore.yml` / `.github/workflows/modernize.yml` `dotnet-version` pins from
`8.0.x` to `10.0.x`, for consistency with the new target (outside the strict project list but
required so the upgraded app actually runs in its container/CI).

Operation sequence: (1) update `global.json` (covered by task 01) and `Directory.Packages.props`;
(2) `dotnet restore` the solution; (3) build and fix all compilation errors/warnings across all
projects in one bounded pass (not an open-ended retry loop) — treat warnings as errors per
workflow rules, no `#pragma warning disable`/`NoWarn` without explicit approval.

**Done when**: `Directory.Packages.props` and `global.json` target net10.0 / `10.0.x`; all 10
`.csproj` files build against net10.0 (no project overrides `TargetFramework`, so this is implicit
once the central property changes); `dotnet build eShopOnWeb.sln` succeeds with 0 errors and 0
warnings in the touched projects; Dockerfiles and CI workflow `dotnet-version` pins updated to
10.0.

### 03-final-validation: Full solution build and test suite

Run a full solution build and the complete test suite (`UnitTests`, `IntegrationTests`,
`FunctionalTests`, `PublicApiIntegrationTests`) against net10.0 to confirm no regressions. No new
unit tests are generated (per success criteria — existing coverage must continue to pass as-is).
Re-run `dotnet restore`/`dotnet list package --vulnerable` to confirm the two previously-flagged
vulnerabilities (Azure.Identity, System.Text.Json) are resolved and no new ones were introduced by
the package bumps. Document any deferred recommendations (e.g., BlazorInputFile replacement) if
discovered-but-not-required during task 02.

**Done when**: `dotnet build eShopOnWeb.sln -c Release` succeeds with 0 errors/0 warnings;
`dotnet test` passes for all 4 test projects; `dotnet list package --vulnerable` shows no known
vulnerabilities; `progress-details.md` for task 02 records the final file list and any deferred
items.
