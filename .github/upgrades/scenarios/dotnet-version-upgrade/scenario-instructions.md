# .NET Version Upgrade — eShopOnWeb (.NET 8 → .NET 10)

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: Top-Down
- **Unsupported Packages**: Defer Resolution
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade-1
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: All 10 projects are already SDK-style modern .NET projects, but the solution still has 3 application entry points, a 5-level dependency graph, 19 recommended package upgrades, one incompatible package, and the highest API risk concentrated in PublicApi and Web. An application-first rollout keeps the repository buildable and incrementally mergeable while temporary compatibility is introduced only where shared libraries are needed.

### Execution Constraints
- Upgrade application entry points in this order: BlazorAdmin, PublicApi, then Web.
- Add temporary library compatibility only when the next application requires it, starting from leaf libraries and moving upward through the dependency chain.
- Keep the tests for each application aligned with the application task that introduces the framework change, and validate the new target before moving to the next application.
- Do not remove temporary net8.0 targets, conditional package references, or compatibility branches until all applications and dependent tests are on net10.0.
- Finish with full solution restore/build/test validation and explicitly record any deferred package replacements or runtime follow-up checks.
