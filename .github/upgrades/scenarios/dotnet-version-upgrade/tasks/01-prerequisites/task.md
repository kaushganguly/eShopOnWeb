# 01-prerequisites: Verify SDK and update global.json

Verify the .NET 10 SDK is installed and available to the build toolchain, and update
`global.json`'s SDK version constraint from the .NET 8 SDK band to a .NET 10 SDK band
(rollForward policy preserved). This is a prerequisite gate for every subsequent task —
if the SDK is missing, halt and report rather than proceeding with a build that cannot
succeed.

**Done when**: `global.json` specifies a .NET 10 SDK version, and `dotnet --version`/SDK
discovery confirms a matching SDK is installed and resolvable from the repo root.

## Research Findings

- Installed SDKs on this machine (`dotnet --list-sdks`): 8.0.131, 8.0.206, 8.0.319, 8.0.425,
  9.0.121, 9.0.205, 9.0.318, 10.0.112, 10.0.204, 10.0.303, **10.0.401** (latest .NET 10 SDK).
- `validate_dotnet_sdk_installation(targetFramework=net10.0)` → "Compatible SDK found".
- Original `global.json`:
  ```json
  { "sdk": { "version": "8.0.x", "rollForward": "latestFeature" } }
  ```
- No other SDK/toolchain constraints found in the repo (no `.config/dotnet-tools.json`
  pinning an incompatible tool version affecting the build).

## Change Made

Updated `global.json` `sdk.version` from `8.0.x` to `10.0.x`, keeping
`rollForward: latestFeature` unchanged so it resolves to the newest installed 10.0.x
feature band (10.0.401).

## Validation

- `validate_dotnet_sdk_in_globaljson(targetFramework=net10.0)` → "Success: no changes were
  needed in global.json" (confirms the updated global.json is compatible with the net10.0
  target).
- `dotnet --version` from repo root now resolves to `10.0.401`.
