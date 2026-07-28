# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), all modern .NET SDK-style; 1 incompatible package, 19 package upgrades needed, API breaking changes in PublicApi and Web.

## Strategy

### Upgrade Strategy
All projects are on modern .NET (net8.0) with 10 projects total (≤15). Ambiguous signals (5-tier depth but small scope, mechanical TFM bump) — All-at-Once applies for ≤15 projects.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, multi-targeting shared libraries temporarily. |

## Compatibility

### Unsupported Packages
1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) with no compatible version for net10.0.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same task. Small count (1 package) — feasible inline. |
| Defer Resolution | Make the project compile without the package via stubs, then create follow-up tasks. |

### Unsupported API Handling
API breaking changes detected in PublicApi and Web (Api.0001 binary incompatible, Api.0002 source incompatible). Modern-to-modern upgrade with minor changes expected.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred stubs. |
| Defer Complex Changes | Apply simple replacements inline; defer complex changes to follow-up subtasks. |
