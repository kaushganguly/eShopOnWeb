# Task 02-core-libraries: Progress Details

## Summary

Upgraded BlazorShared, ApplicationCore, and Infrastructure to net10.0. All three projects build with 0 errors and 0 warnings.

## Changes Made

### TargetFramework
All three projects inherit `net10.0` from `Directory.Packages.props` (which already had `<TargetFramework>net10.0</TargetFramework>` set globally). No individual project file changes were needed for TFM.

### `src/ApplicationCore/ApplicationCore.csproj`
- **Removed** `<PackageReference Include="System.Security.Claims" />` — included in the net10.0 framework (NU1510 warning)
- **Removed** `<PackageReference Include="System.Text.Json" />` — included in the net10.0 framework (NU1510 warning)

### `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
- **Removed** the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor — this API (`Exception(SerializationInfo, StreamingContext)`) is obsolete in .NET 10 (SYSLIB0051). Binary serialization via `ISerializable` is no longer supported; the constructor was dead code with no callers.

## Build Results

| Project | Errors | Warnings |
|---------|--------|----------|
| BlazorShared | 0 | 0 |
| ApplicationCore | 0 | 0 |
| Infrastructure | 0 | 0 |

## Issues Encountered and Resolved

1. **NU1510 — System.Security.Claims**: Package is now part of the net10.0 framework, no longer needed as an explicit reference. Removed from ApplicationCore.csproj.
2. **NU1510 — System.Text.Json**: Package is now part of the net10.0 framework, no longer needed as an explicit reference. Removed from ApplicationCore.csproj.
3. **SYSLIB0051 — Obsolete serialization constructor**: `Exception(SerializationInfo, StreamingContext)` is obsolete in .NET 10. Removed the overriding constructor from `EmptyBasketOnCheckoutException` as binary formatter-based serialization is no longer supported.

## Files Modified

- `src/ApplicationCore/ApplicationCore.csproj`
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
