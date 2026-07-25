# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects on net8.0, single TFM bump to net10.0, SDK-style, CPM active. Mechanical upgrade with known fixes.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Update Directory.Packages.props centrally (TargetFramework property + all package versions)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (no compatible version for net10.0)
- Fix API changes inline: Exception serialization constructors, ConfigurationBinder.Get<T>(), OptionsConfigurationServiceCollectionExtensions.Configure<T>(), TimeSpan.FromMinutes()
- Validate full solution build after all changes; then run unit tests

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
