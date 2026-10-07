## Changes Made
- `global.json`: SDK version `8.0.x` → `10.0.401` (kept `rollForward: latestFeature`).
- `Directory.Packages.props`: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`.

## Verification
- `dotnet --info` confirms SDK `10.0.401` resolves cleanly (Host version 10.0.12, no NETSDK1045 errors).

## Result
Success. All 10 projects now retarget to net10.0 via the single centrally-managed property (no
per-project csproj changes needed — confirmed no project overrides `TargetFramework`).
