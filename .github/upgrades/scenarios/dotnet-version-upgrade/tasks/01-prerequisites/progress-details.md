# 01-prerequisites Progress Details

## What Was Done
- Read scenario instructions and task definition
- Inspected `global.json` and found it pinned to `8.0.x` with `rollForward` set to `latestFeature`
- Verified installed SDKs and confirmed .NET 10 SDKs were available (`10.0.109`, `10.0.204`, `10.0.301`)
- Updated `global.json` to use SDK version `10.0.301`
- Changed `rollForward` to `latestMajor`
- Verified active SDK with `dotnet --version`
- Ran `dotnet restore /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln`

## Files Modified
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/.github/upgrades/scenarios/dotnet-version-upgrade/tasks/01-prerequisites/task.md`

## Build/Restore Status
- `dotnet --version`: `10.0.301`
- `dotnet restore`: succeeded (exit code 0)
- Restore emitted pre-existing NuGet vulnerability warnings:
  - `System.Text.Json` 8.0.3 (`NU1903`)
  - `Azure.Identity` 1.10.4 (`NU1902`)

## Notes
- The updated `global.json` now references an actually installed .NET 10 SDK version.
- Done-when criteria satisfied for SDK selection, `global.json` update, and restore validation.
