# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all on net8.0, SDK-style; 1 incompatible package, 9 binary/source-incompatible API occurrences across PublicApi and Web; centralized package management active.

## Strategy

### Upgrade Strategy
Solution has 10 projects, all on modern .NET (net8.0), with a 5-tier dependency graph. Incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) will be removed; API changes are limited to configuration binding and serialization patterns that are mechanically fixable inline.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass — fastest path for a modern-to-modern TFM bump on a 10-project solution. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries — adds overhead without benefit for net8→net10. |

## Compatibility

### Unsupported Packages
One incompatible package found: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` has no compatible version for net10.0. Small count (1 package) — resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Remove/replace incompatible package within the same task; no deferred work. Appropriate for 1 incompatible package. |
| Defer Resolution | Generate minimal stubs and create follow-up tasks — unnecessary overhead for a single package removal. |

### Unsupported API Handling
Binary/source-incompatible APIs found in Web and PublicApi: ConfigurationBinder.Get<T>(), OptionsConfigurationServiceCollectionExtensions.Configure<T>(), TimeSpan.FromMinutes(), and Exception serialization constructors. All are mechanical fixes suitable for inline resolution in a modern-to-modern upgrade.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. Leaves no deferred work — appropriate for modern-to-modern with few API changes. |
| Defer Complex Changes | Apply simple replacements inline and stub complex ones — unnecessary for this small set of known fixes. |
