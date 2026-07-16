# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: All-at-Once
- **Package Management**: Keep Central Package Management as-is
- **Nullable Annotations**: Defer nullable annotation expansion
- **Validation Scope**: Full solution restore, build, and test after single-pass upgrade

## Strategy
**Selected**: All-at-Once
**Rationale**: TargetFramework is centrally managed in Directory.Build.props (single file change covers all 10 projects). All projects are on modern .NET 8.0 (no Framework migration). CPM is already in use. Simple TFM bump with package updates.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Update Directory.Build.props TargetFramework first, then update global.json SDK version
- Update all packages in Directory.Packages.props in one task
- Fix any API breaking changes after package updates
- Validate with full solution build and all tests before completing

## Source Control
- **Source Branch**: main
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
