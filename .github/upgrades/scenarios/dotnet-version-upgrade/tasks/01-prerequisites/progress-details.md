# Progress Details: 01-prerequisites

## Changes Made
1. `global.json`: `sdk.version` changed from `8.0.x` to `10.0.x` (kept `rollForward: latestFeature`).
2. `Directory.Packages.props`: central `<TargetFramework>` changed from `net8.0` to `net10.0`.
   This single property drives the TFM for all 10 projects (none declare `TargetFramework`
   individually) — confirmed via `AppModUpgrade-validate_dotnet_sdk_in_globaljson` ("no changes
   needed") and `AppModUpgrade-validate_dotnet_sdk_installation` ("Compatible SDK found").

## Validation
- `dotnet --version` → `10.0.401` (installed SDK, matches global.json `10.0.x` + `rollForward:
  latestFeature`).
- `dotnet restore eShopOnWeb.sln` → succeeded for all 11 restore targets (10 projects + solution
  metadata). Only warnings emitted:
  - NuGet security-vulnerability advisories (NU1901-NU1904) for AutoMapper, Azure.Identity,
    Microsoft.Extensions.Caching.Memory, Microsoft.Identity.Client, NuGet.Common,
    NuGet.Packaging, NuGet.Protocol, System.Text.Json — these are addressed by package version
    bumps in tasks 02-04 (per assessment's NuGet.0004 "contains security vulnerability" findings).
  - NU1510 "PackageReference will not be pruned" for `System.Net.Http.Json` (BlazorAdmin) and
    `System.Security.Claims`/`System.Text.Json` (ApplicationCore) — these packages' functionality
    is now included with the framework reference on net10.0 (assessment's NuGet.0003 finding).
    Will remove these explicit PackageReferences in tasks 02/03.
- No build errors. No TFM resolution errors. Toolchain foundation is ready for tasks 02-04.

## Notes for Downstream Tasks
- Tasks 02/03/04 do NOT need to touch `TargetFramework` per-project — it's centralized. Focus
  solely on package version bumps (`Directory.Packages.props` `<PackageVersion>` entries and the
  `AspNetVersion`/`SystemExtensionVersion`/`EntityFramworkCoreVersion`/`VSCodeGeneratorVersion`
  central version properties) and source code fixes for breaking/behavioral API changes.
- Remove now-redundant explicit PackageReferences flagged by NU1510 (`System.Net.Http.Json` in
  BlazorAdmin; `System.Security.Claims`, `System.Text.Json` in ApplicationCore) as part of tasks
  02/03.
