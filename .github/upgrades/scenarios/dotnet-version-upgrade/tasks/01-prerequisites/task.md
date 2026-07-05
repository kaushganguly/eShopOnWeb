# 01-prerequisites: 01-prerequisites

## Objective
Verify the .NET 10 SDK is installed, update `global.json` to an installed .NET 10 SDK version, and confirm solution restore succeeds with the updated SDK selection.

## Execution Summary
- Confirmed installed SDKs include `10.0.109`, `10.0.204`, and `10.0.301`
- Updated `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json` from .NET 8 SDK pinning to `10.0.301`
- Set `rollForward` to `latestMajor`
- Verified `dotnet --version` resolves to `10.0.301`
- Ran `dotnet restore` on `eShopOnWeb.sln`; restore succeeded with existing NuGet vulnerability warnings only
