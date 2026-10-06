# 01-prerequisites — Progress Details

## What changed

- `global.json`: `sdk.version` changed from `8.0.x` to `10.0.x` (kept `rollForward: latestFeature`).

## Validation

- `dotnet --version` (run from repo root) resolves to `10.0.401`, confirming the .NET 10 SDK
  installed in this environment is now selected via `global.json`.

## Files modified

- `global.json`
