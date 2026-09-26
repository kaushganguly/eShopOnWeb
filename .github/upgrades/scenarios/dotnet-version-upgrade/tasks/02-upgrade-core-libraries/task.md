# 02-upgrade-core-libraries: Upgrade ApplicationCore, Infrastructure, and BlazorShared to net10.0

## Objective

Move `ApplicationCore`, `Infrastructure`, and `BlazorShared` to `net10.0`, resolve their package
issues, and fix the one source-incompatible API flagged by the assessment.

## Research (via query_dotnet_assessment)

### ApplicationCore.csproj (6 issues)
- `Project.0002` — TFM must change net8.0 → net10.0. **Resolved automatically**: this repo has no
  per-project `<TargetFramework>` — all projects inherit `<TargetFramework>` from the CPM-imported
  `Directory.Packages.props`, which task 01 already bumped to `net10.0`. Confirmed via `dotnet build`
  (output now under `bin/Debug/net10.0/`).
- `NuGet.0003` — `System.Security.Claims 4.3.0` — functionality now included in the framework
  reference. Action: remove the `PackageReference` (and its now-unused `PackageVersion`).
- `NuGet.0002` / `NuGet.0004` — `System.Text.Json 8.0.3` — recommended to remove/replace with
  `10.0.12`; also flagged as a security vulnerability (NU1903, GHSA-8g4q-xg66-9fp4, GHSA-hh2w-p6rv-4g7w).
  Since `System.Text.Json` is now part of the shared framework for `net10.0`, action: remove the
  explicit `PackageReference`/`PackageVersion` entirely rather than bumping it — eliminates the
  vulnerability and the NU1510 "will not be pruned" warning in one move.
- `Api.0002` — `System.Exception(SerializationInfo, StreamingContext)` ctor is obsolete
  (SYSLIB0051, binary-formatter-based serialization) in `Exceptions/EmptyBasketOnCheckoutException.cs:12`.
  Action: remove the obsolete `protected` serialization constructor — it's not used anywhere else
  in the repo (verified via repo-wide grep) and this pattern is the standard fix recommended for
  SYSLIB0051 when binary serialization isn't in use.

### Infrastructure.csproj (5 issues)
- `Project.0002` — TFM net8.0 → net10.0 (resolved automatically, same mechanism as above).
- `NuGet.0002` (×3) — `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.InMemory`,
  `Microsoft.EntityFrameworkCore.SqlServer`, all pinned at `8.0.2` via the `$(AspNetVersion)` /
  `$(EntityFramworkCoreVersion)` CPM alias properties — **resolved automatically** by task 01's
  alias bump to `10.0.12`.
- `NuGet.0005` — `System.IdentityModel.Tokens.Jwt 7.3.1` is deprecated; recommended replacement
  `8.23.0`. Action: bump the `PackageVersion` in `Directory.Packages.props`.

### BlazorShared.csproj (1 issue)
- `Project.0002` — TFM net8.0 → net10.0 (resolved automatically).
- No package or API issues per assessment. `BlazorInputFile 0.2.0` and `FluentValidation` were
  checked with `get_supported_package_version` against `net10.0`: `BlazorInputFile` has no newer
  version (stays `0.2.0`), `FluentValidation` has a newer supported version `12.1.1` (from `11.9.0`)
  — bumped for consistency even though not flagged, since a project-wide FluentValidation bump is
  low-risk and keeps all consumers current.

## Package version resolution (via get_supported_package_version, target net10.0)

| Package | Old | New |
|---|---|---|
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 |
| Ardalis.Result | 7.0.0 | 10.1.0 |
| Ardalis.Specification | 7.0.0 | 9.3.1 |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 |
| FluentValidation | 11.9.0 | 12.1.1 |
| BlazorInputFile | 0.2.0 | 0.2.0 (unchanged) |
| System.Security.Claims | 4.3.0 | removed (framework-provided) |
| System.Text.Json | 8.0.3 | removed (framework-provided; resolves NU1903 CVE) |

## Done when

All three projects target `net10.0`, restore cleanly, and build with 0 errors/0 warnings. ✅ Verified.
