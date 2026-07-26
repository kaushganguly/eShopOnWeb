# 01-prerequisites: Verify SDK and update global.json

## Objective

Verify that the .NET 10 SDK is installed and compatible. Update `global.json` to specify a net10.0 SDK version. Confirm that `Directory.Packages.props` uses the correct property names and variable structure for the target version bump.

## Scope

- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json` — update SDK version to .NET 10
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props` — review and update version variables

## Context

The `global.json` specifies the .NET SDK version constraint — it must be updated to require a .NET 10 SDK. The `Directory.Packages.props` file defines version variables (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) that centrally drive most package versions — updating these properties is the primary lever for the package upgrade in subsequent tasks. Also includes the `TargetFramework` property which needs to be updated to `net10.0`.

## Done when

- `global.json` specifies a net10.0-compatible SDK version (.NET 10.0.x)
- .NET 10 SDK is confirmed installed
- `Directory.Packages.props` has `TargetFramework` updated to `net10.0`
- `Directory.Packages.props` version variables are updated for .NET 10.0 packages (AspNetVersion=10.0.x, EntityFramworkCoreVersion=10.0.x, etc.)
