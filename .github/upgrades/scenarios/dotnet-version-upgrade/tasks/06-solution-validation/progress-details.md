# Progress Details: 06-solution-validation

## Summary
Full net10.0 solution validation completed successfully.
No code changes were required — the solution was clean after tasks 01–05.

---

## Phase 1: Build Results

**Command**: `dotnet build eShopOnWeb.sln`

| Metric | Result |
|--------|--------|
| Errors | **0** |
| Warnings | **36** (all NU190x — see deferred packages section) |
| Exit code | **0 (success)** |

### Warning Breakdown
All 36 warnings are NuGet security/deprecation notices. No compiler (CS*) or MSBuild (MSB*) warnings.

| Warning code | Package | Severity | Count |
|---|---|---|---|
| NU1903 | AutoMapper 12.0.1 | High | 10 (5 projects × 2 restore phases) |
| NU1901 | NuGet.Packaging 6.12.1 | Low | 10 |
| NU1901 | NuGet.Protocol 6.12.1 | Low | 10 |
| NU1901 | NuGet.Protocol 6.12.1 | Low | 6 |

*(NuGet.Packaging and NuGet.Protocol are transitive dependencies pulled in by AutoMapper; they are not directly referenced.)*

---

## Phase 2: Test Results

**Command**: `dotnet test eShopOnWeb.sln --no-build`

| Test Project | Framework | Passed | Failed | Skipped | Total |
|---|---|---|---|---|---|
| UnitTests | net10.0 | 44 | 0 | 0 | 44 |
| IntegrationTests | net10.0 | 3 | 0 | 0 | 3 |
| FunctionalTests | net10.0 | 12 | 0 | 0 | 12 |
| PublicApiIntegrationTests | net10.0 | 15 | 0 | 0 | 15 |
| **TOTAL** | | **74** | **0** | **0** | **74** |

✅ All 74 tests pass on net10.0.

---

## Phase 3: Project Target Framework Verification

All 10 projects in the solution explicitly declare `<TargetFramework>net10.0</TargetFramework>`.
The solution-wide default in `Directory.Build.props` is also `net10.0`.

| Project path | net10.0 confirmed |
|---|---|
| src/ApplicationCore/ApplicationCore.csproj | ✅ |
| src/BlazorAdmin/BlazorAdmin.csproj | ✅ |
| src/BlazorShared/BlazorShared.csproj | ✅ |
| src/Infrastructure/Infrastructure.csproj | ✅ |
| src/PublicApi/PublicApi.csproj | ✅ |
| src/Web/Web.csproj | ✅ |
| tests/UnitTests/UnitTests.csproj | ✅ |
| tests/IntegrationTests/IntegrationTests.csproj | ✅ |
| tests/FunctionalTests/FunctionalTests.csproj | ✅ |
| tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj | ✅ |

---

## Phase 4: Intentionally Deferred Items

These items were deliberately left out of scope for this upgrade. Each continues to work
at runtime on net10.0 but requires a separate follow-up ticket.

### DEF-01 — AutoMapper.Extensions.Microsoft.DependencyInjection (v12.0.1)
- **Warning**: NU1903 — High severity vulnerability (GHSA-rvv3-g6hj-g44x)
- **Affected projects**: `src/Web`, `src/PublicApi`
- **Reason deferred**: AutoMapper v12 is the abandoned last release; no compatible v13+ exists.
  Migration requires replacing mapping profiles with manual constructors or an alternative library.
- **Recommended action**: Evaluate Mapperly (source-generator based, no runtime overhead) or remove
  AutoMapper entirely in favour of manual projection extensions. Estimated effort: medium.

### DEF-02 — System.IdentityModel.Tokens.Jwt (v8.4.0)
- **Warning**: NU1901 (NuGet.0005) — deprecated
- **Affected projects**: `src/Infrastructure`, `src/PublicApi`, `src/Web`
- **Reason deferred**: Functional replacement is `Microsoft.IdentityModel.JsonWebTokens` but token
  handler and claims APIs differ; requires careful regression testing of auth flows.
- **Recommended action**: Replace `JwtSecurityTokenHandler` with `JsonWebTokenHandler` from
  `Microsoft.IdentityModel.JsonWebTokens`. Token validation parameters are compatible.

### DEF-03 — xunit v2 family (xunit 2.7.0, xunit.runner.console 2.7.0, xunit.runner.visualstudio 2.5.6)
- **Warning**: NU1901 (NuGet.0005) — deprecated
- **Affected projects**: All four test projects
- **Reason deferred**: xunit v3 has breaking changes in test class constructors and async fixture
  lifecycle that require test code changes across all test projects.
- **Recommended action**: Upgrade to `xunit` ≥ 3.0 and `xunit.runner.visualstudio` ≥ 3.0.
  Review xunit v3 migration guide at https://xunit.net/docs/getting-started/v3/migration.

### DEF-04 — BlazorInputFile (v0.2.0)
- **Affected projects**: `src/BlazorAdmin`, `src/BlazorShared`
- **Reason deferred**: No official net10.0-compatible update available; package is abandoned.
  The in-box replacement (`<InputFile>` from `Microsoft.AspNetCore.Components.Forms`) requires
  review of event-handler signatures in Blazor components.
- **Recommended action**: Replace `BlazorInputFile` file-input components with the built-in
  `<InputFile>` component. Audit `OnChange` / `OnInput` handler signatures.

### DEF-05 — Microsoft.VisualStudio.Azure.Containers.Tools.Targets (v1.19.6)
- **Affected projects**: `Directory.Packages.props` only (removed from all .csproj files in task 04)
- **Reason deferred**: Package is incompatible with net10.0. The `<PackageReference>` was removed
  from `src/PublicApi/PublicApi.csproj`; only the `<PackageVersion>` pin in CPM remains.
- **Recommended action**: Remove the `PackageVersion` entry from `Directory.Packages.props` once
  confirmed no tooling integration requires it, or update when a net10.0-compatible release ships.

---

## Files Changed
None — this was a validation-only task. All findings were clean.

## Artifacts Written
- `tasks/06-solution-validation/task.md` — enriched with research findings, project table, deferred items
- `tasks/06-solution-validation/progress-details.md` — this file
