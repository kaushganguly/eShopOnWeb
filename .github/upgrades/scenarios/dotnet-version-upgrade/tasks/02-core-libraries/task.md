# 02-core-libraries: Upgrade foundation libraries

## Objective

Upgrade BlazorShared, ApplicationCore, and Infrastructure from net8.0 to net10.0. These are foundation libraries all other projects depend on.

## Scope

- `src/BlazorShared/BlazorShared.csproj` — update TFM, minimal changes
- `src/ApplicationCore/ApplicationCore.csproj` — update TFM, fix source-incompatible APIs, fix security vulnerability (Azure.Identity already updated in Directory.Packages.props)
- `src/Infrastructure/Infrastructure.csproj` — update TFM, package upgrades via CPM already done

## Context from Plan

**BlazorShared**: Pure class library. Lowest risk — TFM change only.

**ApplicationCore**: Has source-incompatible APIs flagged. `System.Security.Claims` (4.3.0) is now included in framework — remove explicit reference. Azure.Identity security vulnerability already fixed in Directory.Packages.props (→1.21.0).

**Infrastructure**: Package upgrades for EF Core already handled via Directory.Packages.props.

All package version bumps are handled centrally via Directory.Packages.props — individual project files just need TFM update.

## Done when

- BlazorShared, ApplicationCore, and Infrastructure all target net10.0
- Each project builds successfully with 0 errors and 0 warnings
- All mandatory issues from assessment are resolved (source-incompatible APIs fixed, System.Security.Claims reference removed)
