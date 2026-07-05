# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), 6-tier dependency graph, 1 incompatible package, 9 binary/source-incompatible API occurrences, CPM already in use

## Strategy

### Upgrade Strategy
TFM is managed centrally in Directory.Packages.props across all 10 modern .NET projects (net8.0); ambiguous signals (≤15 projects but 6-tier depth) default to All-at-Once for ≤15 projects.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously; single TFM property change in Directory.Packages.props upgrades the entire solution. |
| Top-Down | Upgrade entry-point apps first, temporarily multi-targeting shared libraries — not applicable when TFM is centrally managed. |

## Compatibility

### Unsupported Packages
1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) has no compatible version for net10.0; small count (1) favors inline resolution.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task; remove or replace the package without deferring. |
| Defer Resolution | Generate minimal stubs and create follow-up tasks for real replacements. |

### Unsupported API Handling
Binary-incompatible APIs detected in Web.csproj (6 mandatory issues) and PublicApi.csproj (6 mandatory issues); Fix Inline is default for modern-to-modern upgrades with minor breaking changes.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred work. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create follow-up subtasks. |
