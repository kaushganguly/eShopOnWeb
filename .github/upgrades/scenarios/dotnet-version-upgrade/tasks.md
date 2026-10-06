# Tasks — dotnet-version-upgrade (eShopOnWeb net8.0 → net10.0)

> **Note**: The `start_task`/`break_down_task`/`complete_task` MCP tools returned errors
> consistent with an unavailable backend service in this environment (the same orchestrator
> channel failure seen for `generate_dotnet_upgrade_assessment`), even after multiple retries
> with increasing delays. Task tracking for this run is maintained manually in this file and in
> each task's `tasks/{id}/progress-details.md`, mirroring the structure those tools would have
> produced.

- ✅ 01-prerequisites: Verify SDK toolchain and update global.json
- ✅ 02-upgrade-all-projects: Upgrade TFM and package references across the solution, fix breaking changes
- ✅ 03-final-validation: Run full test suite and confirm security compliance
