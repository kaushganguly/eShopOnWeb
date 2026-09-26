# Progress — 01-prerequisites

## Changes Made

- `global.json`: `sdk.version` changed from `8.0.x` to `10.0.x` (kept `rollForward: latestFeature`).
- `Directory.Packages.props`: central property group updated:
  - `TargetFramework`: `net8.0` → `net10.0`
  - `AspNetVersion`: `8.0.2` → `10.0.12`
  - `SystemExtensionVersion`: `8.0.0` → `10.0.12`
  - `EntityFramworkCoreVersion`: `8.0.2` → `10.0.12`
  - `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
  - All versions resolved via `get_supported_package_version` against representative consuming
    projects (BlazorAdmin, Infrastructure, PublicApi, Web) targeting `net10.0`.

## Validation

- `dotnet --version` from repo root resolves to `10.0.401` (matches the `10.0.x` global.json band;
  confirms the installed .NET 10 SDK is picked up correctly).
- No project `.csproj` files were touched in this task — TFM bumps happen in tasks 02-04.

## Notes for Later Tasks

- Individual project `<TargetFramework>net8.0</TargetFramework>` entries still need to change to
  `net10.0` per project (tasks 02-04) — the CPM alias property change here does not by itself
  retarget projects.
- Non-alias package versions (Ardalis.*, MediatR, FluentValidation, Swashbuckle, xunit, etc.) are
  still on their old pinned versions in `Directory.Packages.props` — these will be resolved and
  bumped during tasks 02-04 as each project group is upgraded.
