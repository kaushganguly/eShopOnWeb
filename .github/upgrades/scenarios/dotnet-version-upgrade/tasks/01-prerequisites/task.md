# 01-prerequisites: Verify SDK and update global.json

## Objective
Confirm that the .NET 10 SDK is installed and available. Update `global.json` to specify a net10.0-compatible SDK version. Verify `Directory.Packages.props` for centrally managed `TargetFramework`.

## Research Findings

### Installed .NET SDKs
The following SDKs are installed on this machine:
- 8.0.129, 8.0.206, 8.0.319, 8.0.423
- 9.0.119, 9.0.205, 9.0.316
- **10.0.110, 10.0.204, 10.0.302** ✅ .NET 10 is available (latest: `10.0.302`)

### global.json (current)
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```
- Uses `8.0.x` with `rollForward: latestFeature` → resolves to `8.0.423`
- **Must be updated** to target .NET 10 SDK before TFM bump

### Directory.Packages.props
- `ManagePackageVersionsCentrally` is `true` — Central Package Management is active
- **`TargetFramework` is defined centrally** as `net8.0` (in `<PropertyGroup>`) — this is a custom convention used by this repo; updating it here will propagate to all projects that reference `$(TargetFramework)` from this file
- Version variables that need updating for .NET 10:
  - `AspNetVersion` = `8.0.2` → needs `10.0.x`
  - `SystemExtensionVersion` = `8.0.0` → needs `10.0.x`
  - `EntityFramworkCoreVersion` = `8.0.2` → needs `10.0.x` (note: typo in prop name is intentional/pre-existing)
  - `VSCodeGeneratorVersion` = `8.0.0` → needs `10.0.x`
  - Package `System.Text.Json` = `8.0.3` → needs update
  - Several test SDK packages (xunit, MSTest) may need updates
- **`TargetFramework` and version variables will be updated in a later task** (02-central-package-management or equivalent TFM task)

### Files to Change in This Task
- `global.json` — update `version` to `10.0.302`, keep `rollForward: latestFeature`

## Execution Steps
1. Update `global.json` SDK version to `10.0.302` with `rollForward: latestFeature`
2. Verify the SDK resolves correctly via `dotnet --version`
3. Do NOT change `Directory.Packages.props` in this task — that is scoped to the TFM bump task

## Done When
- `global.json` specifies a .NET 10-compatible SDK version
- `dotnet --version` confirms a .NET 10 SDK is in use
- Build infrastructure is ready for the TFM bump in subsequent tasks
