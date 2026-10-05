# Progress — 01-prerequisites

## Changes made
- `global.json`: SDK version `8.0.x` → `10.0.x` (kept `rollForward: latestFeature`).

## Validation
- `dotnet --info` resolves to SDK `10.0.401` from the repo root (global.json respected).
- `dotnet --list-sdks` confirms multiple 10.0.x SDKs installed, satisfying the roll-forward policy.

## Status: Complete
