# 01-prerequisites: Verify toolchain and update global.json

Verify the .NET 10.0 SDK is installed on the build machine and update `global.json`
(previously pinned to `8.0.x` with `rollForward: latestFeature`) to target the .NET 10
SDK band. Also update `Directory.Packages.props`'s central `<TargetFramework>` property
(currently `net8.0`) and the version-alias properties (`AspNetVersion`=8.0.2,
`SystemExtensionVersion`=8.0.0, `EntityFramworkCoreVersion`=8.0.2,
`VSCodeGeneratorVersion`=8.0.0) that drive most package versions across the solution via
central package management (CPM).

**Done when**: `global.json` targets a .NET 10 SDK version installed on the machine, and
`dotnet --version`/`dotnet restore` succeed against it.

## Research Findings

- Installed SDKs on build machine (`dotnet --list-sdks`): 8.0.131, 8.0.206, 8.0.319,
  8.0.425, 9.0.121, 9.0.205, 9.0.318, 10.0.112, 10.0.204, 10.0.303, **10.0.401** (latest
  .NET 10 SDK — used to satisfy `10.0.x` pin).
- `validate_dotnet_sdk_installation(net10.0)` → Compatible SDK found.
- `global.json` originally pinned `"version": "8.0.x", "rollForward": "latestFeature"`.
  `rollForward: latestFeature` only rolls within the same major.minor SDK band, so this
  pin would have kept builds on .NET 8 SDKs even after project TFMs move to net10.0.
  Updated to `"version": "10.0.x"` (rollForward left as `latestFeature`, resolves to
  10.0.401 on this machine).
- `Directory.Packages.props` (repo root) uses Central Package Management
  (`ManagePackageVersionsCentrally=true`) with a central
  `<TargetFramework>net8.0</TargetFramework>` property and 4 version-alias MSBuild
  properties that drive the bulk of package versions across all 10 projects. Since
  `Directory.Packages.props` is auto-imported by the NuGet/SDK targets and no individual
  `.csproj` declares its own `<TargetFramework>`, updating this single file's
  `<TargetFramework>` retargets all 10 projects to `net10.0` at once:
  - `AspNetVersion` = 8.0.2 → 10.0.12 (drives all `Microsoft.AspNetCore.*`/`Microsoft.Extensions.Identity.*` packages)
  - `SystemExtensionVersion` = 8.0.0 → 10.0.12 (drives `Microsoft.Extensions.Logging.Configuration`)
  - `EntityFramworkCoreVersion` = 8.0.2 → 10.0.12 (drives all `Microsoft.EntityFrameworkCore.*` packages)
  - `VSCodeGeneratorVersion` = 8.0.0 → 10.0.2 (drives `Microsoft.VisualStudio.Web.CodeGeneration.Design`)
- Per assessment: 10 projects total, all currently net8.0, all SDK-style; 48 NuGet
  packages total, 26 need upgrade, 1 has a confirmed security vulnerability
  (`Azure.Identity` 1.10.4, used by `Web.csproj` → needs ≥1.21.0).
- Per-project package version bumps not covered by the 4 CPM aliases (e.g.
  `Ardalis.*`, `AutoMapper`, `Swashbuckle.AspNetCore`, `FluentValidation`, `MediatR`,
  test packages) and code-level API fixes are handled in tasks 02-04, scoped by
  project group.
