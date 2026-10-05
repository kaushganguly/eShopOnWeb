# Task 01-prerequisites: Verify toolchain and update SDK pin

> Note: `start_task`/`break_down_task` MCP tools returned "no tasks found" (orchestrator
> channel unavailable — same root cause as `generate_dotnet_upgrade_assessment` earlier).
> This task.md was created manually to preserve the workflow artifact trail.

## Objective

Verify the .NET 10 SDK is installed and update `global.json` from `8.0.x` to `10.0.x` (keep
`rollForward: latestFeature`). Confirm `dotnet --list-sdks` shows a compatible 10.x SDK before
any project file changes begin.

## Research

- `dotnet --list-sdks` already confirmed earlier: SDKs 8.0.x, 9.0.x, and multiple 10.0.x
  (including 10.0.401) are installed. `dotnet --version` resolves to 10.0.401.
- `global.json` currently pins `"version": "8.0.x", "rollForward": "latestFeature"`.

## Plan

- Update `global.json` → `"version": "10.0.x"`, keep `rollForward: latestFeature`.
- Re-verify `dotnet --info` / `dotnet --version` resolves against a 10.x SDK after the change.

**Done when**: `global.json` targets `10.0.x` and `dotnet --list-sdks` confirms a matching SDK is
installed; `dotnet --info` runs cleanly from the repo root.
