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
- **Working Branch**: dotnet-version-upgrade-net10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down
**Rationale**: All 10 projects are already SDK-style on modern .NET (net8.0), the dependency graph centers on app entry points (`PublicApi`, `BlazorAdmin`, and `Web`), and temporary library multi-targeting can keep the solution buildable while each app slice moves to net10.0.

### Execution Constraints
- Upgrade application entry points first; do not upgrade shared libraries independently ahead of their consuming app slice.
- Add multi-targeting to shared libraries only when an application task needs it, starting with leaf libraries and then moving up the dependency chain.
- Keep tests aligned with the application or library slice that triggers the change, and validate that slice on the new target before moving to the next app.
- Do not start Phase 2 cleanup until `PublicApi`, `BlazorAdmin`, and `Web` all run on net10.0 and no consumer still needs net8.0.
- Leave unsupported package replacement work deferred unless it blocks restore, build, tests, or the required security update.
