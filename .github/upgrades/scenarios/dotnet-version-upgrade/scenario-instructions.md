# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: Top-Down
- **Unsupported Packages**: Resolve Inline
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: The solution has 3 ASP.NET Core entry points across a 5-level dependency graph, and upgrading BlazorAdmin before PublicApi before Web isolates the incompatible package, security updates, and heavier behavioral API changes into manageable validation slices.

### Execution Constraints
- Upgrade entry-point applications in priority order: BlazorAdmin, then PublicApi, then Web.
- Update supporting libraries only when they are required by the application slice being upgraded; do not front-load separate library-only upgrade work.
- Keep test project updates inside the application task that forces them, and validate impacted consumers after each slice before advancing.
- Do not move to final solution validation until all three applications and every dependent test project target net10.0.
- Resolve incompatible packages, security package updates, and unsupported API changes inline within the owning task rather than deferring stubs or cleanup work.
