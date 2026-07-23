# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: Top-Down
- **Unsupported Packages**: Defer Resolution
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down
**Rationale**: All 10 projects are already SDK-style net8.0 projects, but the solution still has a 5-level dependency graph, 19 recommended package upgrades, 7 incompatible or deprecated package findings, a security vulnerability in Azure.Identity, and concentrated API risk in the Web, PublicApi, and BlazorAdmin entry points. An application-first sequence upgrades the business-visible hosts in controlled slices while deferring non-blocking package replacement work until the net10.0 baseline is stable.

### Execution Constraints
- Upgrade application entry points before broad shared-library cleanup; only make shared-project changes that are required by the current application task.
- Keep test-project fixes inside the application task that introduces them; do not create standalone test-migration tasks.
- Do not start deferred package cleanup until BlazorAdmin, Web, PublicApi, and their attached test suites are running on net10.0.
- Treat deprecated or incompatible package replacement as deferred work unless it blocks the current application from building or running on net10.0.
- Keep the commit strategy at After Each Task so each application slice and the final cleanup remain independently reviewable.
