# Progress Details: 01-toolchain-baseline

## Status: ✅ Complete

## Changes Made

### 1. `global.json` — SDK baseline updated to .NET 10

**Before:**
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```

**After:**
```json
{
  "sdk": {
    "version": "10.0.x",
    "rollForward": "latestFeature"
  }
}
```

The `latestFeature` rollForward policy allows the toolchain to use any installed .NET 10 SDK matching the `10.0.x` pattern. Available .NET 10 SDKs on this machine: 10.0.109, 10.0.204, 10.0.301, 10.0.302 (latest selected).

### 2. `tasks/01-toolchain-baseline/task.md` — Enriched with findings

Added:
- SDK availability findings
- Central package management note (`Directory.Packages.props` still at `net8.0`)
- Deferred package backlog with categorized tables (deprecated, incompatible, framework-included, security vulnerability, version-bumps)

## Build Verification

The solution was built with `dotnet build eShopOnWeb.sln --configuration Release` after the global.json change.

**Result: ✅ Build succeeded — 0 errors, 13 warnings (all pre-existing)**

Pre-existing warnings (not introduced by this task):
- `NU1902`: Azure.Identity 1.10.4 moderate vulnerability (deferred to Web app task)
- `NU1903`: System.Text.Json 8.0.3 high vulnerability (deferred to package cleanup task)
- `SYSLIB0051`: Obsolete serialization constructor in EmptyBasketOnCheckoutException (deferred to ApplicationCore task)
- `xUnit2013`: Test assertion style warnings in UnitTests and IntegrationTests (pre-existing)

## Deferred Package Summary

| Category | Count | Examples |
| :--- | :---: | :--- |
| Deprecated | 6 | AutoMapper.Extensions.DI, xunit, MSTest.* |
| Incompatible | 1 | Microsoft.VisualStudio.Azure.Containers.Tools.Targets |
| Framework-included | 1 | System.Security.Claims |
| Security vulnerability | 1 | Azure.Identity |
| Version bumps (with TFM) | 13 | Microsoft.AspNetCore.*, EFCore.*, System.* |

## What Was NOT Changed (intentionally)

- Project `<TargetFramework>` values — still `net8.0`; changed per application task
- NuGet package versions in `Directory.Packages.props` — changed per application task
- Any source code files
