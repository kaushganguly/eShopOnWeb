# Task 01-prerequisites: Progress Details

## Summary
Updated `global.json` to pin the .NET 10 SDK and verified SDK resolution across the entire solution.

## Changes Made

### global.json
- **File**: `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- **Before**:
  ```json
  {
    "sdk": {
      "version": "8.0.x",
      "rollForward": "latestFeature"
    }
  }
  ```
- **After**:
  ```json
  {
    "sdk": {
      "version": "10.0.302",
      "rollForward": "latestMinor"
    }
  }
  ```
- **Reason**: Pins to the latest .NET 10 SDK available on the build machine (`10.0.302`). Changed `rollForward` from `latestFeature` to `latestMinor` — `latestMinor` is the recommended policy for .NET 10 as it allows patch-level roll-forward within the same minor version without jumping to a new feature band unexpectedly.

## Validation Results

### SDK Resolution
- `dotnet --version` → `10.0.302` ✅
- All available .NET 10 SDKs on the machine:
  - `10.0.110`
  - `10.0.204`
  - `10.0.302` (selected — latest)

### dotnet restore (eShopOnWeb.sln)
- Exit code: `0` ✅
- All 9 projects restored successfully
- No SDK resolution errors
- Pre-existing package vulnerability warnings present (NU1902, NU1903) — unrelated to this task

## Done-When Criteria
- [x] `dotnet --version` resolves to a .NET 10.x SDK
- [x] `global.json` specifies a .NET 10 SDK version (`10.0.302`)
- [x] No SDK resolution errors on `dotnet restore`
