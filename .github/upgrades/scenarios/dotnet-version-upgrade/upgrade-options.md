# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all net8.0 (modern .NET), 1 incompatible package, binary/source API breaking changes in PublicApi and Web, 5-tier dependency depth

## Strategy

### Upgrade Strategy
All projects are on net8.0 (modern .NET), 10 projects (≤15), with ambiguous depth signals (5 tiers including tests). Per default logic for ≤15 projects with ambiguous signals, All-at-Once is recommended.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach; no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. Use for large solutions or CI-green constraints. |

## Compatibility

### Unsupported Packages
1 incompatible package found (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) with no compatible version for net10.0.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task. Small count (1) makes this practical. |
| Defer Resolution | Generate stubs and create follow-up tasks for real replacements. |
| Compatibility Mode | Keep reference with NU1701 suppressed. May cause runtime failures. |

### Unsupported API Handling
Binary-incompatible (Api.0001) and source-incompatible (Api.0002) APIs flagged in PublicApi and Web for net10.0.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. Modern-to-modern upgrade; changes expected to be minor. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes for follow-up tasks. |
