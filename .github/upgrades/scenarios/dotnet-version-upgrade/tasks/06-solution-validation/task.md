# 06-solution-validation: Validate the complete net10.0 solution

## Objective
Verify the end state of the net10.0 migration across the full solution:
full build passes, all 74 automated tests pass, and intentionally deferred
incompatible/deprecated packages are documented for follow-up tracking.

## Scope
- `eShopOnWeb.sln` (all 10 projects)
- All four test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)

## Research Findings

### All Projects Target net10.0
| Project | TargetFramework |
|---------|----------------|
| src/ApplicationCore | net10.0 (explicit) |
| src/BlazorAdmin | net10.0 (explicit) |
| src/BlazorShared | net10.0 (explicit) |
| src/Infrastructure | net10.0 (explicit) |
| src/PublicApi | net10.0 (explicit) |
| src/Web | net10.0 (explicit) |
| tests/UnitTests | net10.0 (explicit) |
| tests/IntegrationTests | net10.0 (explicit) |
| tests/FunctionalTests | net10.0 (explicit) |
| tests/PublicApiIntegrationTests | net10.0 (explicit) |

Note: `Directory.Build.props` also sets `<TargetFramework>net10.0</TargetFramework>` as the solution-wide default.

### Deferred / Incompatible Packages

The following packages were intentionally deferred during the upgrade because they
are either deprecated with no drop-in replacement in scope, or removed from compatibility
with net10.0. They continue to function at runtime but should be addressed in follow-up work.

#### 1. AutoMapper.Extensions.Microsoft.DependencyInjection (v12.0.1)
- **Warning**: NU1903 — high severity vulnerability (GHSA-rvv3-g6hj-g44x)
- **Used in**: `src/Web`, `src/PublicApi` (and transitively test projects)
- **Status**: Deprecated; the AutoMapper package itself is abandoned.
- **Recommendation**: Migrate object-to-object mapping to manual constructors/extension methods or
  an actively maintained alternative (e.g. Mapperly, manual DI registration).

#### 2. System.IdentityModel.Tokens.Jwt (v8.4.0)
- **Warning**: NU1901 (NuGet.0005) — deprecated
- **Used in**: `src/Infrastructure`, `src/PublicApi`, `src/Web`
- **Status**: Superseded by `Microsoft.IdentityModel.Tokens` (same org, active).
- **Recommendation**: Replace `System.IdentityModel.Tokens.Jwt` with `Microsoft.IdentityModel.JsonWebTokens`
  (the modern successor) or rely on ASP.NET Core's built-in `JwtBearerHandler`.

#### 3. xunit (v2.7.0) / xunit.runner.console (v2.7.0) / xunit.runner.visualstudio (v2.5.6)
- **Warning**: NU1901 (NuGet.0005) — deprecated (xunit v2 family)
- **Used in**: `tests/UnitTests`, `tests/IntegrationTests`, `tests/FunctionalTests`, `tests/PublicApiIntegrationTests`
- **Status**: xunit v2 is deprecated; xunit v3 is the current release.
- **Recommendation**: Upgrade all test projects to xunit v3 (`xunit` ≥ 3.0, `xunit.runner.visualstudio` ≥ 3.0).
  xunit v3 has source-breaking changes (test class constructors, async fixtures) requiring test code updates.

#### 4. BlazorInputFile (v0.2.0)
- **Used in**: `src/BlazorAdmin`, `src/BlazorShared`
- **Status**: Community package for Blazor file-input before the feature was in-box; no update to net10.0.
- **Recommendation**: Replace with the built-in `<InputFile>` component from `Microsoft.AspNetCore.Components.Forms`
  (available since .NET 5). The API is similar but not identical — review event handler signatures.

#### 5. Microsoft.VisualStudio.Azure.Containers.Tools.Targets (v1.19.6)
- **Used in**: `src/PublicApi` (reference retained in `Directory.Packages.props` but `<PackageReference>` removed from .csproj)
- **Status**: Incompatible with net10.0; removed from active projects in task 04.
- **Recommendation**: Remove the `PackageVersion` entry from `Directory.Packages.props` when no longer needed for tooling compatibility, or update when a net10.0-compatible version is released.

### NuGet Security Warnings Remaining
All 36 remaining build warnings are NU190x security/deprecation notices from the five deferred packages above.
No compiler warnings (CS*, MSB*) remain.

## Execution Notes
- Build: 0 errors, 36 warnings (all NU190x — deferred, documented above)
- Tests: 74 total — 74 passed, 0 failed, 0 skipped
- No code fixes were required; the solution was already in a clean state after tasks 01–05
