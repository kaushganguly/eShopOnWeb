# 01-prerequisites: Verify SDK and update global.json

## Objective
Verify .NET 10 SDK is installed and update `global.json` to target it, unblocking all subsequent build operations.

## Done When
- `global.json` references .NET 10 SDK (`10.0.x` with `rollForward: latestFeature`)
- `dotnet --version` returns a `10.x` version

## Findings

### Installed SDKs
All installed SDK versions at task execution time:
- 8.0.128, 8.0.206, 8.0.319, 8.0.422 (at `/usr/share/dotnet/sdk`)
- 9.0.118, 9.0.205, 9.0.315
- **10.0.109, 10.0.204, 10.0.301** ✅

Highest .NET 10 SDK available: **10.0.301**

### File Modified
- **`/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`**
  - `sdk.version`: `"8.0.x"` → `"10.0.x"`
  - `sdk.rollForward`: unchanged (`"latestFeature"`)

### Verification
- `dotnet --version` (run from repo root): **10.0.301** ✅

## Status
✅ Complete
