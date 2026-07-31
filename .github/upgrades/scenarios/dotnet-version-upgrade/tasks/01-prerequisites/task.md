# 01-prerequisites: 01-prerequisites

## Objective
Verify the build environment supports the net10.0 upgrade prerequisite and align the repository SDK pinning with an installed .NET 10 SDK.

## Research Findings
- Repository root `global.json` originally pinned the SDK to `8.0.x` with `rollForward: latestFeature`.
- `dotnet --version` in the repo returned `10.0.302`.
- `dotnet --list-sdks` showed installed SDKs including `10.0.110`, `10.0.204`, and `10.0.302`.
- Selected SDK pin: `10.0.302`, because it is installed locally and is the highest available .NET 10 SDK in this environment.
- `validate_dotnet_sdk_installation` confirmed a compatible SDK is present for `net10.0`.

## Execution Summary
- Updated `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json` to pin `sdk.version` to `10.0.302`.
- Kept `rollForward: latestFeature` unchanged.
- Re-verified SDK resolution after the edit.

## Outcome
Task prerequisite satisfied: the environment has a .NET 10 SDK available and `global.json` now references a .NET 10 SDK version.
