# 01-prerequisites: Verify .NET 10 SDK and update global.json

## Objective
Confirm .NET 10 SDK is installed and available. Update `global.json` to be compatible with .NET 10. Update `Directory.Packages.props` to target `net10.0` and use .NET 10 ASP.NET package versions.

## Research Findings

### SDK State
- `dotnet --version` → `10.0.301` ✅
- Available .NET 10 SDKs: `10.0.109`, `10.0.204`, `10.0.301`
- Active SDK is already .NET 10 (10.0.301 is selected by default)

### global.json (before)
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```
- Was pinned to `8.0.x` with `latestFeature` — incompatible with net10.0 toolchain.
- Updated to `10.0.100` + `rollForward: latestFeature` so any 10.0.x SDK is accepted.

### Directory.Packages.props (before)
- `<TargetFramework>net8.0</TargetFramework>` — shared across all projects via CPM
- `<AspNetVersion>8.0.2</AspNetVersion>` — used by all `Microsoft.AspNetCore.*` and test packages
- `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` — used by `Microsoft.Extensions.*` and `System.Net.Http.Json`
- `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` — used by all EF Core packages
- `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` — used by code generation tools

## Files Affected
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`

## Done-When Criteria
- [x] `dotnet --version` reports .NET 10 SDK
- [x] `global.json` specifies SDK version compatible with .NET 10 (`10.0.100` + `latestFeature`)
- [x] `Directory.Packages.props` has `<TargetFramework>net10.0</TargetFramework>`
- [x] `Directory.Packages.props` has `<AspNetVersion>10.0.9</AspNetVersion>`
