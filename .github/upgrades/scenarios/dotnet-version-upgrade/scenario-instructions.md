# .NET Version Upgrade — eShopOnWeb to .NET 10

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: All 10 projects use centrally-managed TFM (Directory.Build.props) and packages (Directory.Packages.props); a single atomic change covers the entire solution efficiently. The net8.0→net10.0 bump is a mechanical upgrade with manageable API issues.

### Execution Constraints
- Single atomic upgrade — all 10 projects updated together in one pass
- Update Directory.Packages.props for all package version bumps (centrally managed)
- Update TFM + SDK version in Directory.Build.props / global.json first
- Fix all API breaking changes inline (no stubs, no deferred tasks)
- Remove incompatible package Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- Remove System.Security.Claims (now included in framework)
- Validate full solution build after upgrade; run unit tests as final validation
