# Progress Details: 01-prerequisites

## Summary
Verified .NET 10 SDK availability and updated SDK toolchain configuration files to target net10.0.

## Changes Made

### global.json
**File**: `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`

| Field | Before | After |
|-------|--------|-------|
| `sdk.version` | `8.0.x` | `10.0.100` |
| `sdk.rollForward` | `latestFeature` | `latestFeature` (unchanged) |

The `rollForward: latestFeature` policy means the SDK resolver will pick the highest patch version within the latest available feature band ≥ 10.0.100. On this machine that resolves to `10.0.301`.

### Directory.Packages.props
**File**: `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`

| Property | Before | After |
|----------|--------|-------|
| `<TargetFramework>` | `net8.0` | `net10.0` |
| `<AspNetVersion>` | `8.0.2` | `10.0.9` |

The `AspNetVersion` property is used by all `Microsoft.AspNetCore.*` packages and `Microsoft.AspNetCore.Mvc.Testing` (test project). Updating it here propagates the version bump to all consumers via CPM.

Note: `<SystemExtensionVersion>`, `<EntityFramworkCoreVersion>`, and `<VSCodeGeneratorVersion>` were **not** updated in this task — they will be addressed in the per-project upgrade tasks once framework targets are updated and compatibility is validated.

## Validation
- `dotnet --version` → `10.0.301` ✅
- `global.json` compatible with .NET 10 SDK ✅
- `Directory.Packages.props` targets `net10.0` ✅
- `Directory.Packages.props` uses `AspNetVersion 10.0.9` ✅

## Build Status
- No build executed (configuration-only task)
