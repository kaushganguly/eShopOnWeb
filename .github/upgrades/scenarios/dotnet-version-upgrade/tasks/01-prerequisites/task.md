# 01-prerequisites: Verify toolchain and global.json compatibility

## Objective
Verify the .NET 10 SDK is installed and available to the build (`global.json`, CI toolchain). Confirm the repository's `global.json` (if present) allows/targets a .NET 10 SDK version, and update it if it pins an older SDK.

## Research findings
- `dotnet --list-sdks` shows SDKs 8.0.x, 9.0.x, and 10.0.112/10.0.204/10.0.303/10.0.401 installed on this machine.
- `global.json` at repo root pinned `"version": "8.0.x"` with `rollForward: latestFeature` — this would prevent `dotnet` from resolving to a .NET 10 SDK.

## Action taken
- Updated `global.json` `sdk.version` from `8.0.x` to `10.0.x` (kept `rollForward: latestFeature`).
- Verified `dotnet --version` now resolves to `10.0.401`.

## Done when
`dotnet --list-sdks` shows a .NET 10 SDK available, and `global.json` does not block it. — **Met.**
