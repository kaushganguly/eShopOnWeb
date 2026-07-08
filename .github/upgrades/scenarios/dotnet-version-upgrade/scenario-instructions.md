# .NET Version Upgrade — eShopOnWeb to net10.0

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
**Selected**: Top-Down (Application-First)
**Rationale**: All 10 projects are already SDK-style net8.0 projects with centralized package management, but the dependency chain is five levels deep and the highest-risk work is concentrated in the three ASP.NET Core entry points. Upgrading applications incrementally contains the 21 mandatory incidents, isolates the binary/source incompatibilities in PublicApi and Web, and keeps the repository buildable while shared projects carry only temporary coexistence support.

### Execution Constraints
- Upgrade applications in this order: BlazorAdmin, then PublicApi, then Web.
- Introduce temporary net8.0/net10.0 coexistence support only when an upgraded application still has a downstream net8.0 consumer, starting from the lowest dependency level needed for that application slice.
- Keep test updates inside the application task that caused them, and after each application task confirm both the new net10.0 slice and the remaining net8.0 consumers still compile.
- Do not begin consolidation until BlazorAdmin, PublicApi, and Web all build on net10.0 and their dependent tests are green.
- Defer incompatible package replacement unless it blocks build or runtime behavior, but fix unsupported APIs inline within the owning task and treat the Azure.Identity security update as an early package move.
