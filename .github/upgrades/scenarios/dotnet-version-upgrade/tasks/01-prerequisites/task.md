# 01-prerequisites: Prerequisites — .NET 10 SDK Verification & global.json Update

## Objective
Verify that .NET 10 SDK is installed and update `global.json` to pin the SDK to a .NET 10.0.x release, ensuring the toolchain is ready before any project file or code changes begin.

## Research Findings

### Available .NET SDKs (from `dotnet --list-sdks`)
- 8.0.128, 8.0.206, 8.0.319, 8.0.422 (8.0.x — current pin before this task)
- 9.0.118, 9.0.205, 9.0.315
- **10.0.109, 10.0.204, 10.0.301** ← .NET 10 SDKs available

### Selected SDK Version
`10.0.301` — latest available .NET 10 SDK on this machine.

### File to Modify
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
  - Before: `"version": "8.0.x"`
  - After:  `"version": "10.0.301"`

## Steps
1. ✅ Read current `global.json` — confirmed version was `8.0.x`
2. ✅ Run `dotnet --list-sdks` — confirmed `10.0.301` is installed
3. ✅ Update `global.json` `sdk.version` → `10.0.301`
4. ✅ Run `dotnet --version` to verify active SDK

## Done Criteria
- `global.json` references `10.0.301`
- `dotnet --version` reports `10.0.x`
