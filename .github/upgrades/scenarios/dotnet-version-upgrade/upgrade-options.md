# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), 5-tier dependency graph, CPM enabled, 1 incompatible package, API breaking changes in Web and PublicApi

## Strategy

### Upgrade Strategy
All projects target net8.0 (modern .NET). With 10 projects, CPM already in place, and a mechanical TFM bump, All-at-Once is efficient.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. |

## Compatibility

### Unsupported Packages
One incompatible package identified: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` has no compatible version for net10.0.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve within the same task. Remove incompatible package, no deferred work. |
| Defer Resolution | Generate stubs and create follow-up tasks. |

### Unsupported API Handling
Breaking API changes in Web, PublicApi, ApplicationCore: serialization constructors removed, configuration binding method changed.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task. Modern-to-modern upgrade with few changes. |
| Defer Complex Changes | Apply simple replacements inline; stub complex ones for follow-up tasks. |
