# Upgrade Options — eShopOnWeb

Assessment: 10 projects (all net8.0, SDK-style, CPM in use), 1 incompatible package, API changes in 3 projects, 119 total issues.

## Strategy

### Upgrade Strategy
All 10 projects are on modern .NET (net8.0). Solution has ≤15 projects with clear dependency structure and CPM already in place. One incompatible package to resolve inline.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach for this modern-to-modern upgrade; no multi-targeting overhead needed. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. Better for large solutions or strict CI-green requirements. |

## Compatibility

### Unsupported Packages
Assessment identified 1 incompatible package (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`) with no compatible net10.0 version. Small count — resolve inline during upgrade task.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task. Remove or replace the incompatible reference during the upgrade. |
| Defer Resolution | Generate minimal stubs to keep project building; create follow-up tasks for real replacements. |

### Unsupported API Handling
Assessment flagged binary-incompatible APIs in PublicApi and Web, and source-incompatible APIs in ApplicationCore and Web. Modern-to-modern upgrades typically have minor API changes — fix them inline.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred work, no stubs to clean up. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create follow-up tasks. |
