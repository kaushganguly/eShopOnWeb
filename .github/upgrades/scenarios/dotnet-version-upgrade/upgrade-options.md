# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all net8.0, all SDK-style, 5-tier dependency graph, 1 truly incompatible package, API breaking changes detected

## Strategy

### Upgrade Strategy
10 projects all on modern .NET (net8.0), no .NET Framework projects present; All-at-Once is the appropriate strategy for a clean net8→net10 bump.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. |
| Top-Down | Upgrade entry-point applications first, multi-targeting shared libraries; useful when CI must stay green or for 15+ project solutions. |

## Compatibility

### Unsupported Packages
1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) with no compatible version; small enough to resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task; removes or replaces packages. |
| Defer Resolution | Create minimal stubs and follow-up tasks for real replacements. |

### Unsupported API Handling
Binary and source-incompatible API changes detected in Web, PublicApi, ApplicationCore; modern-to-modern upgrades typically have minor breaking changes that can be fixed inline.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. |
| Defer Complex Changes | Apply simple replacements inline; stub complex ones for follow-up tasks. |
