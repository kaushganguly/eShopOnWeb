# Task 01-prerequisites: Verify SDK and update global.json

## Objective

Verify that the .NET 10 SDK is available on the machine and update `global.json` to pin the SDK to `10.0.x` with `rollForward: latestFeature`.

## Research Findings

### .NET 10 SDK Availability

The following .NET 10 SDK versions are installed at `/usr/share/dotnet/sdk`:
- `10.0.109`
- `10.0.204`
- `10.0.301`
- `10.0.302` ← active (resolved by `dotnet --version`)

### State Before Change
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```

### Required Change
Update `global.json` at the repository root to reference the .NET 10 SDK version family:
```json
{
  "sdk": {
    "version": "10.0.x",
    "rollForward": "latestFeature"
  }
}
```

The `rollForward: latestFeature` policy is preserved — it allows the toolchain to use the latest feature band within the `10.0.x` major.minor, ensuring forward compatibility without locking to a single patch.

## Scope

- **File changed**: `global.json` (repository root)
- **No source code changes required**

## Done Criteria

- [x] `global.json` references `10.0.x` SDK version
- [x] `dotnet --version` resolves to a .NET 10 SDK (`10.0.302`)
