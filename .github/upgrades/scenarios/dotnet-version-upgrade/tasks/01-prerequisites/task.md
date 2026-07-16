# 01-prerequisites: Prerequisites

## Objective
Verify .NET 10 SDK installation and update `global.json` to reference the .NET 10 SDK so the solution is ready for the upgrade.

## Steps
1. Run `dotnet --list-sdks` to identify installed .NET 10 SDK version
2. Update `global.json` SDK version from `8.0.x` to the installed .NET 10 SDK version
3. Run `dotnet restore` + `dotnet build` to confirm solution builds under .NET 10 tooling

## Changes Made
- **`global.json`**: Updated `sdk.version` from `8.0.x` → `10.0.302` (latest available .NET 10 SDK)

## Outcome
✅ .NET 10 SDK 10.0.302 is installed and active. Solution builds successfully (0 errors, 13 pre-existing warnings).
