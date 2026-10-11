# Task 01-prerequisites: Verify toolchain and update global.json / central TFM

> **Note**: `start_task`/`break_down_task` MCP tools returned
> `Task '01-prerequisites' not found in plan.md or tasks.md. Available top-level tasks: `
> (empty) consistently across many retries — the same backend outage affecting
> `generate_dotnet_upgrade_assessment` (`Orchestrator channel was not available`). This
> task folder/file was created manually to preserve the workflow's traceability contract.
> Content mirrors `plan.md`'s task 01.

## Objective
Verify the .NET 10.0 SDK is installed and update `global.json` (currently pinned to
`8.0.x` with `rollForward: latestFeature`) to target the installed `10.0.x` SDK. Update
the centrally-managed `<TargetFramework>` property in `Directory.Packages.props` from
`net8.0` to `net10.0` — this single change propagates to all 10 projects since none of
them declare a project-level `TargetFramework`.

## Research findings
- .NET SDKs installed on this machine (via `dotnet --list-sdks`): 8.0.131, 8.0.206,
  8.0.319, 8.0.425, 9.0.121, 9.0.205, 9.0.318, **10.0.112, 10.0.204, 10.0.303, 10.0.401**.
  `dotnet --version` reports `10.0.401` as the active/default SDK — .NET 10 is available.
- `global.json` currently: `{"sdk": {"version": "8.0.x", "rollForward": "latestFeature"}}`.
- `Directory.Packages.props` currently: `<TargetFramework>net8.0</TargetFramework>` inside
  the root `<PropertyGroup>`, alongside `ManagePackageVersionsCentrally=true`.
- Confirmed (via `get_project_dependencies` per project and direct csproj inspection) that
  none of the 10 project files declare their own `<TargetFramework>` — all inherit from
  `Directory.Packages.props`, which is auto-imported like a `Directory.Build.props` when
  CPM is enabled.

## Done when
- [x] .NET 10 SDK confirmed installed (10.0.401 present).
- [x] `global.json` updated to `"version": "10.0.x"`.
- [x] `Directory.Packages.props` `<TargetFramework>` updated to `net10.0`.
