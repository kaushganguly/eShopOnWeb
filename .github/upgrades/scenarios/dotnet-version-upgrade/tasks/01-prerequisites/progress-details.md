# Progress: 01-prerequisites

## Changes Made

1. **`global.json`** — updated `sdk.version` from `8.0.x` to `10.0.x` (kept
   `rollForward: latestFeature`). Verified `dotnet --version` now resolves to
   `10.0.401` (the latest installed .NET 10 SDK).
2. **`Directory.Packages.props`** — updated the central `<TargetFramework>` property
   from `net8.0` to `net10.0` and the four CPM version-alias properties:
   - `AspNetVersion`: 8.0.2 → 10.0.12
   - `SystemExtensionVersion`: 8.0.0 → 10.0.12
   - `EntityFramworkCoreVersion`: 8.0.2 → 10.0.12
   - `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2

## Validation

- `validate_dotnet_sdk_installation(net10.0)` → Compatible SDK found (10.0.401 available).
- `dotnet --version` → `10.0.401` (confirms `global.json` roll-forward now resolves to a
  .NET 10 SDK).

## Notes

- Per-project package version bumps (the remaining packages not driven by the 4 CPM
  aliases, including the `Azure.Identity` security fix) and code-level API fixes are
  handled in tasks 02-04, scoped by project group.
