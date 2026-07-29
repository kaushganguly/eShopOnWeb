# Progress Details: 01-update-sdk-and-packages

## Status: Complete

## Summary

Updated `global.json` and `Directory.Packages.props` to target .NET 10. All changes are in two centrally managed files — no individual project files required modification.

## Files Changed

### 1. `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`

| Property | Before | After |
|----------|--------|-------|
| `sdk.version` | `"8.0.x"` | `"10.0.x"` |
| `sdk.rollForward` | `"latestFeature"` | `"latestFeature"` (unchanged) |

### 2. `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`

**Version variables (PropertyGroup):**

| Property | Before | After |
|----------|--------|-------|
| `TargetFramework` | `net8.0` | `net10.0` |
| `AspNetVersion` | `8.0.2` | `10.0.10` |
| `SystemExtensionVersion` | `8.0.0` | `10.0.10` |
| `EntityFramworkCoreVersion` | `8.0.2` | `10.0.10` |
| `VSCodeGeneratorVersion` | `8.0.0` | `10.0.2` |

**Individual PackageVersion entries:**

| Package | Before | After |
|---------|--------|-------|
| `Azure.Identity` | `1.10.4` | `1.21.0` |
| `System.Text.Json` | `8.0.3` | `10.0.10` |
| `System.IdentityModel.Tokens.Jwt` | `7.3.1` | `8.21.0` |

## Cascade Effect

The version variable updates in `Directory.Packages.props` automatically propagate to all dependent packages via MSBuild property references (`$(AspNetVersion)`, etc.), affecting:
- 10 ASP.NET / Blazor / Identity packages via `$(AspNetVersion)`
- 2 Microsoft.Extensions packages via `$(SystemExtensionVersion)`
- 3 EF Core packages via `$(EntityFramworkCoreVersion)`
- 1 VS code generator package via `$(VSCodeGeneratorVersion)`

## Build / Test

Deferred to task 04 (restore + build) and task 05 (tests).

## Issues

None encountered.
