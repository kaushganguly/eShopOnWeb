# Plan — Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | **All-At-Once** | 10 projects (6 src + 4 test), all SDK-style, all already on `net8.0`, Central Package Management in use, dependency graph only 3 tiers deep, no .NET Framework projects. Well within All-At-Once "ideal" criteria. |
| Test Coverage | **Skip** | `generateNewUnitTests=false` per task success criteria — existing tests must keep passing, no new tests generated. |

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0 today, simple dependency structure (BlazorShared → ApplicationCore → Infrastructure → BlazorAdmin → Web → PublicApi, plus 4 test projects), Central Package Management already centralizes every version in one file.

## Projects In Scope (single group — no tiers)

**Libraries**
- `src/BlazorShared/BlazorShared.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`

**Applications**
- `src/Web/Web.csproj`
- `src/PublicApi/PublicApi.csproj`

**Tests**
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

## Tasks

### 01-prerequisites-and-tfm: Verify toolchain and retarget to net10.0

Verify the .NET 10 SDK is installed (confirmed: `10.0.401` and other 10.0.x feature bands present
alongside 8.0.x/9.0.x) and update `global.json`'s SDK pin from `8.0.x` to `10.0.x` (or relax
`rollForward` as needed) so the installed SDKs resolve correctly. Update the single centrally-managed
`<TargetFramework>` in `Directory.Packages.props` from `net8.0` to `net10.0` — this retargets all 10
projects in one edit since none of them override the TFM individually. No project file changes are
needed for the TFM itself; only `Directory.Packages.props` and `global.json`.

**Done when**: `global.json` targets a `10.0.x` SDK band; `Directory.Packages.props` declares
`<TargetFramework>net10.0</TargetFramework>`; `dotnet --info` confirms a resolvable SDK.

### 02-upgrade-packages-and-fix-build: Update all NuGet package versions and resolve build issues

Update every `PackageVersion` in `Directory.Packages.props` to its net10-compatible release across
all families referenced by the solution: ASP.NET Core / Blazor (`$(AspNetVersion)` group: Components.*,
Identity.*, JwtBearer, Diagnostics.EntityFrameworkCore, Mvc.Testing → 10.0.12), EF Core
(`$(EntityFramworkCoreVersion)` group: InMemory, SqlServer, Tools → 10.0.12), System.Extensions
(`$(SystemExtensionVersion)` group: Logging.Configuration, Net.Http.Json, Text.Json → 10.0.12),
Azure SDK (Azure.Identity → 1.21.0, Azure.Extensions.AspNetCore.Configuration.Secrets → 1.5.2),
Ardalis.* (GuardClauses → 5.0.0, Result → 10.1.0, Specification[.EntityFrameworkCore] → 9.3.1 —
these two must move together, they're built against the same EF Core major), FluentValidation
(→ 12.1.1), Swashbuckle.AspNetCore(.SwaggerUI/.Annotations) (→ 10.2.3), System.IdentityModel.Tokens.Jwt
(→ 8.23.0), Microsoft.Web.LibraryManager.Build (→ 3.0.114), Microsoft.VisualStudio.Web.CodeGeneration.Design
(`$(VSCodeGeneratorVersion)` → 10.0.2, separate release line from ASP.NET Core itself), Microsoft.VisualStudio.Azure.Containers.Tools.Targets
(→ 1.23.0), and test packages (NSubstitute → 6.2.0, Microsoft.NET.Test.Sdk → 18.10.1, xunit → 2.9.3,
xunit.runner.visualstudio → 3.1.5, xunit.runner.console → 2.9.3, MSTest.TestAdapter/TestFramework → 4.4.1,
coverlet.collector → 10.1.0). Cap MediatR at **12.4.1** (last MIT-licensed 12.x release) rather than the
newer commercially-licensed 13.x/14.x lines, to avoid introducing a licensing change as a side effect of
this TFM upgrade.

Remove the abandoned `BlazorInputFile` package (0.2.0, no net10 asset, last published pre-Blazor-WASM-GA)
along with its only two usages — the `@using BlazorInputFile` in `src/BlazorAdmin/_Imports.razor` and the
unused `CatalogItem.DataToBase64(IFileListEntry)` helper in `src/BlazorShared/Models/CatalogItem.cs` —
confirmed dead code with no `.razor` markup referencing an `InputFile`/`IFileListEntry` anywhere in the
solution. Also remove the orphaned `Microsoft.AspNetCore.Mvc` 2.2.0 `PackageVersion` entry (not referenced
by any project — vestigial ASP.NET Core 2.x entry predating the current package set).

After updating `Directory.Packages.props`, restore and build the full solution
(`eShopOnWeb.sln`) and fix all resulting compilation errors and warnings in a single bounded pass —
this is one pass, not an iterative retry loop. Expected risk areas per the assessment: Blazor WASM
hosting API shifts between ASP.NET Core 8 and 10 in `src/Web/Program.cs` and `src/BlazorAdmin`, and
EF Core 8→10 behavior changes surfaced through `Ardalis.Specification.EntityFrameworkCore` in
`src/Infrastructure`. Treat all build warnings in touched projects as errors to fix (per workflow
rules) — do not suppress with `NoWarn`/pragmas.

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors and 0 warnings across all 10
projects; no project references `BlazorInputFile` or the orphaned `Microsoft.AspNetCore.Mvc` entry;
`Ardalis.Specification` and `Ardalis.Specification.EntityFrameworkCore` are pinned to the same 9.3.1
version.

### 03-validate: Run full test suite and finalize

Run all four test projects (`UnitTests`, `IntegrationTests`, `FunctionalTests`,
`PublicApiIntegrationTests`) against the upgraded solution. Fix any test failures caused by the
framework/package upgrade (API shape changes, behavior differences) without adding new tests
(`generateNewUnitTests=false`). Confirm no dependency conflicts remain (`dotnet restore` clean,
no NU1xxx warnings) and no known security vulnerabilities remain in the updated package set.

**Done when**: `dotnet test` passes for all 4 test projects with 0 failures; `dotnet build` remains
clean (0 errors/warnings); solution fully targets `net10.0`.

## Strategy Execution Constraints
- Single atomic upgrade — all 10 projects updated together (TFM + packages in one edit each to
  `Directory.Packages.props` / `global.json`).
- Validate with a full solution build after the upgrade — one bounded fix pass, not iterative
  back-and-forth.
- Testing happens after the atomic upgrade completes successfully, as its own final task.
