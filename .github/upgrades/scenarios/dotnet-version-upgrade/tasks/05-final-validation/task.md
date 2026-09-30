# 05-final-validation: 05-final-validation

Execute task 05-final-validation.

## Scope
Full-solution validation after tasks 01-04: clean restore, clean build, full test suite run
across both `eShopOnWeb.sln` and `Everything.sln`, plus a sweep for any remaining net8.0
references in non-project files (Dockerfiles, CI workflows, IDE launch configs, dotnet tool
manifests) that should be updated for consistency even though they don't affect
`dotnet build`/`dotnet test` success criteria directly.

## Research / Findings

### Full clean rebuild (removed all bin/obj, re-restored, rebuilt from scratch)
- `dotnet restore eShopOnWeb.sln` — succeeded, all 11 projects restored against net10.0 +
  updated Directory.Packages.props versions.
- `dotnet build eShopOnWeb.sln --no-restore` — **0 Warnings, 0 Errors**, all 11 projects.
- `dotnet build Everything.sln --no-restore` — also **0 Warnings, 0 Errors** (superset solution
  used by the richnav.yml CI workflow).

### Full test suite (dotnet test eShopOnWeb.sln --no-build)
| Project | Passed | Failed | Skipped | Total |
|---|---|---|---|---|
| UnitTests | 44 | 0 | 0 | 44 |
| IntegrationTests | 3 | 0 | 0 | 3 |
| FunctionalTests | 12 | 0 | 0 | 12 |
| PublicApiIntegrationTests | 15 | 0 | 0 | 15 |
| **Total** | **74** | **0** | **0** | **74** |

### Non-project files still referencing net8.0 (found via repo-wide grep, fixed for consistency)
These don't affect `dotnet build`/`dotnet test` (success criteria), but were left inconsistent
with the net10.0 upgrade and are part of "ensure the solution builds successfully" in spirit:
- `src/Web/Dockerfile`: `mcr.microsoft.com/dotnet/sdk:8.0` / `aspnet:8.0` → `10.0`
- `src/PublicApi/Dockerfile`: same base-image bump → `10.0`
- `src/Web/.config/dotnet-tools.json`: local `dotnet-ef` tool `8.0.0` → `10.0.12` (matches the
  `Microsoft.EntityFrameworkCore.*` package version now used via `$(EntityFramworkCoreVersion)`
  in `Directory.Packages.props`)
- `.vscode/launch.json`: debug `program` path `bin/Debug/net8.0/Web.dll` → `net10.0`
- `.github/workflows/dotnetcore.yml` ("eShopOnWeb Build and Test" CI): `dotnet-version: '8.0.x'`
  → `'10.0.x'`
- `.github/workflows/richnav.yml` ("eShopOnWeb - Code Index" CI): `dotnet-version: 8.0.x` →
  `10.0.x`

**Intentionally left unchanged**: `.github/workflows/modernize.yml` — this is the meta workflow
that bootstraps the Modernize CLI tool itself (not the application's build/test CI) and
deliberately installs both `8.0.x` and `10.0.x` SDKs side-by-side for tooling compatibility;
changing it is out of scope for this application-level upgrade.

## Result
- Full clean restore + build of both solutions: 0 Warnings, 0 Errors.
- Full test suite: 74/74 passed, 0 failed, 0 skipped.
- Success criteria met: `passBuild=true`, `passUnitTests=true` (and integration/functional/API
  integration tests all pass as well, exceeding the stated criteria).
