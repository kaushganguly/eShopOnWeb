# 01-prerequisites: Validate .NET 10 SDK and update toolchain configuration

## Objective
Ensure the .NET 10 SDK is installed and update global.json to pin the correct SDK version before any project changes are made.

## Research Findings

### Available .NET SDKs on this machine
- 8.0.129, 8.0.206, 8.0.319, 8.0.423
- 9.0.119, 9.0.205, 9.0.316
- **10.0.110, 10.0.204, 10.0.302** ← .NET 10 available

### Original global.json
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```
Note: `"8.0.x"` is not a valid semver version string. The SDK treated it as an unknown version and the `latestFeature` rollForward resolved it to the latest 8.0 SDK available.

### Updated global.json
```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```
Using `"10.0.100"` as the floor version with `"latestFeature"` rollForward — this resolves to the latest feature band in the 10.0 train, currently **10.0.302**.

## Files Modified
- `global.json` — SDK version updated from `8.0.x` to `10.0.100` with `latestFeature` rollForward

## Verification
- `dotnet --version` → **10.0.302** ✓
