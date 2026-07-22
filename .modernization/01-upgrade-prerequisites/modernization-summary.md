# Modernization Summary: 01-upgrade-prerequisites

## Task
Verify the net10.0 baseline and shared upgrade assumptions for eShopOnWeb.

## Changes Made

### `global.json`
Updated the SDK version pin from `8.0.x` to `10.0.100` (with `latestFeature` rollForward policy) to allow the .NET 10 SDK to be used for the upgrade.

- **File**: `global.json`
- **Before**: `"version": "8.0.x"`
- **After**: `"version": "10.0.100"` (resolves to SDK `10.0.302`)

## Baseline Results

| Step | Result |
|------|--------|
| `dotnet --version` | `10.0.302` ✅ |
| `dotnet restore` | ✅ Success (4 security warnings) |
| `dotnet build` | ✅ Success — 0 errors, 9 warnings |
| Unit tests (44) | ✅ All passed |
| Integration tests (3) | ✅ All passed |
| Functional tests (12) | ✅ All passed |

## Key Findings

- All 10 projects are SDK-style, already on `net8.0` via `Directory.Packages.props`
- 2 security vulnerabilities confirmed: `System.Text.Json 8.0.3` (HIGH) and `Azure.Identity 1.10.4` (MODERATE)
- `Swashbuckle.AspNetCore 6.5.0` is incompatible with .NET 10 — replacement needed in Task 03
- Configuration binding API breaks appear in both PublicApi and Web — inline fixes in Tasks 03/04
- Upgrade order: BlazorAdmin → PublicApi/Core/Infra → Web
