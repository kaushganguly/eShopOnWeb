# 01-prerequisites: Verify Prerequisites and Update SDK

## Objective
Update `global.json` to target .NET 10 SDK so that subsequent TFM and package changes use the correct toolchain.

## Findings
- **Current global.json**: SDK version `8.0.x` with `rollForward: latestFeature`
- **Installed SDK**: `10.0.400` (already available on this machine)
- **Required change**: Update `global.json` to `10.0.x`

## Files to Modify
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
