# 01-prerequisites: Verify and update SDK prerequisites

Verify the .NET 10 SDK is installed and update `global.json` to allow the .NET 10 SDK.

## Research Findings

### Files to Modify
- `global.json` — update SDK version from `8.0.x` to `10.0.x`

### Decisions Made
- `validate_dotnet_sdk_installation` confirmed .NET 10 SDK is available
- Updated `global.json` version to `10.0.x` (keeping `latestFeature` rollForward)
- Verified: `dotnet --version` returns `10.0.302`

## Done When
- `global.json` specifies a .NET 10 SDK version
- `dotnet --version` confirms the SDK is available ✅
