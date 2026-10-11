# Task 02-upgrade-all-projects: Bump packages and fix build/code across the solution

> **Note**: Same MCP backend outage as task 01 — created/tracked manually.

## Research & Corrections

The `get_supported_package_version` MCP tool (same degraded orchestrator backend as the
assessment tool) returned **incorrect/non-existent versions** for several packages
(e.g., `Ardalis.ApiEndpoints` 9.3.1 — does not exist, real latest is 4.1.0;
`Azure.Extensions.AspNetCore.Configuration.Secrets` 1.21.0 — does not exist, real latest
is 1.5.2; `AutoMapper.Extensions.Microsoft.DependencyInjection` 14.2.0 — does not exist;
`Blazored.LocalStorage` 4.5.1 — does not exist; `coverlet.collector` 6.2.0 — does not
exist). All package versions were **re-verified directly against the NuGet v3
flat-container API** (`https://api.nuget.org/v3-flatcontainer/{id}/index.json`) before
being applied.

## Security Finding: AutoMapper DoS (GHSA-rvv3-g6hj-g44x, HIGH severity)

`dotnet restore` surfaced `NU1903: Package 'AutoMapper' 12.0.1 has a known high severity
vulnerability` (unbounded recursive-mapping stack overflow / DoS, fixed in 15.1.1 and
16.1.1). Per `securityComplianceCheck=true`, this was fixed by migrating from the retired
`AutoMapper.Extensions.Microsoft.DependencyInjection` companion package to the core
`AutoMapper` package (v16.2.0) — AutoMapper folded `AddAutoMapper(...)` DI registration
into the core package starting at v13, so the companion package is obsolete.

**⚠️ Flagged for user/legal review**: AutoMapper (same author/model as MediatR) introduced
a commercial license tier starting at v13 for qualifying commercial use (license key via
automapper.io). This upgrade prioritizes closing the HIGH severity DoS vulnerability per
the explicit `securityComplianceCheck=true` success criterion; the licensing implication
is **not** something this automated run can resolve on the organization's behalf and
should be reviewed with legal/procurement before production release.

## MediatR: deliberately pinned to last Apache-2.0 release (12.x line)

`MediatR` has a newer `14.2.0` on NuGet, but v13+ requires the same kind of commercial
license (mediatr.io) for qualifying commercial use. No CVE was reported against the
current 12.x line, so — unlike AutoMapper — there is no security requirement forcing the
jump. Bumped only within the free line to the latest 12.x patch, **12.5.0**, and left the
v13+ commercial-license decision to the user.

## Changes Made

- `Directory.Packages.props`: all package versions bumped to the latest NuGet-verified
  stable release supporting net10.0 (see file diff); `TargetFramework` → `net10.0`;
  shared `AspNetVersion`/`EntityFramworkCoreVersion`/`SystemExtensionVersion` → `10.0.12`;
  `VSCodeGeneratorVersion` → `10.0.2`.
- Removed `BlazorInputFile` (abandoned, pre-.NET-Core-3 package, no net10 build) from
  `Directory.Packages.props`, `src/BlazorAdmin/BlazorAdmin.csproj`,
  `src/BlazorShared/BlazorShared.csproj`, and the `@using` in
  `src/BlazorAdmin/_Imports.razor`. Removed the single real usage
  (`CatalogItem.DataToBase64(IFileListEntry)` in `src/BlazorShared/Models/CatalogItem.cs`)
  after confirming via repo-wide search it was dead code (never called).
- Removed the orphan `Microsoft.AspNetCore.Mvc` 2.2.0 `PackageVersion` entry — confirmed
  unreferenced by any project.
- Removed `System.Text.Json` explicit `PackageVersion`/`PackageReference` from
  `ApplicationCore.csproj` — BCL ships it automatically on net10 (NuGet itself flagged
  this and `System.Security.Claims`/`System.Net.Http.Json` as NU1510 "will not be pruned"
  warnings; the latter two were intentionally left in place since removing them is a
  cosmetic cleanup outside this task's scope, not a build blocker).
- Removed `xunit.runner.console` from `tests/UnitTests/UnitTests.csproj` and the legacy
  `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` from
  `tests/FunctionalTests/FunctionalTests.csproj` — both are obsolete tooling mechanisms
  unsupported/unnecessary in modern SDK-style test projects.
- Migrated AutoMapper registration from `AutoMapper.Extensions.Microsoft.DependencyInjection`
  to core `AutoMapper` package (see security finding above) in
  `src/PublicApi/PublicApi.csproj`.
- `global.json`: SDK version `8.0.x` → `10.0.x`.
- `src/Web/Dockerfile`, `src/PublicApi/Dockerfile`: base images `dotnet/sdk:8.0` /
  `dotnet/aspnet:8.0` → `10.0`.

## Done when
- [ ] `dotnet restore` succeeds with no NU1102 (missing package) errors.
- [ ] `dotnet build` succeeds with 0 errors, 0 warnings across all 10 projects.
