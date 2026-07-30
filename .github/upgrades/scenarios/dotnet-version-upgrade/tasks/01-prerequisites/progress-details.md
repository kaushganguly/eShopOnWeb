# Progress Details: 01-prerequisites

## Summary
Updated `global.json` to use .NET 10 SDK (`10.0.302`) and verified the solution restores cleanly.

## Changes Made

### `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- **Before**: `"version": "8.0.x"`, `"rollForward": "latestFeature"`
- **After**: `"version": "10.0.302"`, `"rollForward": "latestMinor"`

`rollForward: latestMinor` allows any future .NET 10 SDK patch release ≥ 10.0.302, which is appropriate for a version upgrade scenario.

## Validation

### SDK Version
```
$ dotnet --version
10.0.302
```
✅ .NET 10 SDK is active.

### Restore Result
```
$ dotnet restore eShopOnWeb.sln
```
✅ All 11 projects restored successfully (exit code 0).

### Restore Warnings (pre-existing, not introduced by this task)
- `NU1902`: `Azure.Identity` 1.10.4 — moderate severity vulnerability (will be addressed when packages are upgraded)
- `NU1903`: `System.Text.Json` 8.0.3 — high severity vulnerability (will be addressed when packages are upgraded)

These warnings exist in the current codebase before the upgrade and will be resolved as package versions are bumped to .NET 10-compatible releases in subsequent tasks.

## Done Criteria
- [x] `global.json` updated to reference .NET 10 SDK (`10.0.302`)
- [x] `dotnet --version` shows `10.0.302` (a .NET 10 SDK)
- [x] Solution restores cleanly — no errors, SDK version constraint no longer blocks .NET 10
