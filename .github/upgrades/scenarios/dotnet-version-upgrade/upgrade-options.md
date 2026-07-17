# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), all SDK-style, 1 incompatible package, 9 binary/source-incompatible API usages, 19 packages with recommended upgrades

## Strategy

### Upgrade Strategy
All 10 projects are on net8.0 with centrally-managed TFM (Directory.Build.props) and packages (Directory.Packages.props); a single atomic pass covers the entire solution efficiently.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass; all TFM and package changes happen together. |
| Top-Down | Upgrade entry-point applications first with temporary multi-targeting of shared libraries; consolidate in a second phase. |

## Compatibility

### Unsupported Packages
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` has no compatible version for net10.0 and must be removed (1 incompatible package — small enough to resolve inline).

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same task; remove reference and adjust any consuming code. |
| Defer Resolution | Generate a minimal stub to keep the project compiling and create a follow-up task for the real resolution. |

### Unsupported API Handling
Binary-incompatible overloads of `ConfigurationBinder.Get<T>` and `OptionsConfigurationServiceCollectionExtensions.Configure<T>` detected in Web and PublicApi; source-incompatible serialization constructor in ApplicationCore/Exceptions and `TimeSpan.FromMinutes` in Web. All are mechanical fixes with known replacements.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred stubs, no follow-up tasks. |
| Defer Complex Changes | Apply simple replacements inline; for complex changes generate a stub and create a resolution subtask. |
