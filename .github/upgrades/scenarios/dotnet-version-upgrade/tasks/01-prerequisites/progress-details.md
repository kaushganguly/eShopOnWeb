# Task 01-prerequisites — Progress Details

## Actions Taken
- Confirmed .NET 10 SDK is installed on the build machine: `10.0.401` (also 10.0.112, 10.0.204, 10.0.303 present).
- `global.json` at repo root pins `"version": "8.0.x"` with `rollForward: latestFeature` — this will be updated to `10.0.x` as part of task 02 (project TFM upgrade) since it gates which SDK `dotnet build`/`dotnet test` resolve to.
- Captured baseline build + test results on net8.0 (before any upgrade changes):
  - `dotnet build eShopOnWeb.sln -c Release` → **Build succeeded**, 0 errors, 13 warnings (2 pre-existing NuGet vulnerability warnings: `System.Text.Json` 8.0.3 NU1903 high severity, `Azure.Identity` 1.10.4 NU1902 moderate severity; 1 SYSLIB0051 obsolete-API warning in `ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`; assorted xUnit2013 test-style warnings).
  - `dotnet test eShopOnWeb.sln -c Release --no-build`:
    - UnitTests: 44 passed / 44 total
    - IntegrationTests: 3 passed / 3 total
    - FunctionalTests: 12 passed / 12 total
    - PublicApiIntegrationTests: 15 passed / 15 total
    - **Total baseline: 74/74 tests passing**

This baseline is the comparison point for task 03 (final validation).

## Notes
- The MCP task-tracking tool (`start_task`/`break_down_task`) did not resolve tasks from `plan.md` in this environment (verified with both the full plan and a minimal single-task reproduction) — task execution proceeded directly against `plan.md`, with progress tracked manually in this file and `execution-log.md`, per the Automatic-mode requirement to complete the upgrade end-to-end without pausing.
