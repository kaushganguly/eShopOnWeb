## Objective
Verify the .NET 10 SDK is installed and retarget the whole solution from net8.0 to net10.0 via the
single centrally-managed `<TargetFramework>` in `Directory.Packages.props`.

## Research

- Installed SDKs (`dotnet --list-sdks`): 8.0.131, 8.0.206, 8.0.319, 8.0.425, 9.0.121, 9.0.205,
  9.0.318, 10.0.112, 10.0.204, 10.0.303, 10.0.401 — multiple 10.0.x feature bands available.
- `global.json` currently pins `"version": "8.0.x", "rollForward": "latestFeature"` → must move to
  `10.0.x` (use the highest installed band, `10.0.401`, with `rollForward: latestFeature` preserved)
  so solution resolution isn't accidentally pinned to a 8.x SDK still present on this machine.
- `Directory.Packages.props` has the **only** `<TargetFramework>` declaration in the whole repo
  (`grep -rn "TargetFramework" src/*/*.csproj tests/*/*.csproj Directory.Packages.props` confirms no
  project overrides it) — a single edit there retargets all 10 projects (6 src + 4 test).

## Steps
1. Update `global.json` SDK version to `10.0.401` (keep `rollForward: latestFeature`).
2. Update `Directory.Packages.props` `<TargetFramework>net8.0</TargetFramework>` → `net10.0`.
3. Confirm `dotnet --info` / `dotnet restore` resolve against the net10 SDK without error.

## Done when
- `global.json` targets a `10.0.x` SDK band.
- `Directory.Packages.props` declares `<TargetFramework>net10.0</TargetFramework>`.
- SDK resolution verified (no NETSDK1045 "not supported by current SDK" errors).
