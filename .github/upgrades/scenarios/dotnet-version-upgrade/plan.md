# Upgrade Plan — eShopOnWeb net8.0 → net10.0

**Flow Mode**: Automatic (no pauses; proceed end-to-end, surface progress only)
**Strategy**: Single-phase, whole-solution upgrade (Simple classification — all SDK-style,
already on a modern TFM, CPM already in place)

> Note: The MCP task-tracking tool (`start_task` / `break_down_task`) could not locate tasks in
> this file or in `tasks.md` in this session regardless of format tried (flat checklist, numbered
> list, bold IDs). Execution proceeded directly against this plan with manual progress tracking
> (session-local todo list) and is recorded in `execution-log.md`.

## Tasks

1. **01-update-sdk-and-tfm** — Update `global.json` SDK pin to `10.0.x` and the centrally
   managed `<TargetFramework>` in `Directory.Packages.props` from `net8.0` to `net10.0`.
2. **02-update-package-versions** — Update all `PackageVersion` entries in
   `Directory.Packages.props` to their net10.0-compatible versions per assessment.md, including
   security-relevant bumps (Azure.Identity, System.IdentityModel.Tokens.Jwt, System.Text.Json).
3. **03-update-docker-base-images** — Bump `mcr.microsoft.com/dotnet/sdk:8.0` /
   `mcr.microsoft.com/dotnet/aspnet:8.0` to `10.0` in `src/Web/Dockerfile` and
   `src/PublicApi/Dockerfile`.
4. **04-build-and-fix-applicationcore-chain** — Restore & build `BlazorShared` ->
   `ApplicationCore` -> `Infrastructure`, fixing any compile errors from Ardalis.* / EF Core major
   version bumps.
5. **05-build-and-fix-web-publicapi-blazoradmin** — Restore & build `BlazorAdmin`, `Web`,
   `PublicApi`, fixing breaking changes from MediatR 12->14, Swashbuckle 6->10, AutoMapper, Azure
   SDK, Blazor WASM package updates.
6. **06-build-and-fix-tests** — Restore & build all test projects (`UnitTests`,
   `PublicApiIntegrationTests`, `FunctionalTests`, `IntegrationTests`), fixing breaking changes from
   xunit/MSTest/NSubstitute/coverlet major bumps.
7. **07-run-tests-and-validate** — Run the full test suite (`dotnet test` on the solution),
   fix any failures, confirm solution builds warning-free, and record results.

## Execution Notes
- CPM means package version edits are all in one file (`Directory.Packages.props`); task 2 is a
  single edit covering every project.
- Build order follows topological dependency order from `get_projects_in_topological_order`.
- generateNewUnitTests = false -> do not add new tests; only fix existing ones to pass.
- securityComplianceCheck = true -> vulnerable packages must be upgraded (done in task 2).
