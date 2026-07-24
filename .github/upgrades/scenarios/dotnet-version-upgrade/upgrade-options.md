# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all net8.0 (modern .NET), 119 issues (21 mandatory), 1 incompatible package, breaking API changes detected.

## Strategy

### Upgrade Strategy
10 projects all on net8.0 (modern-to-modern), straightforward TFM bump with package updates and API fixes. Under 15 projects with ambiguous tier-depth signals → default to All-at-Once.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. More incremental but adds overhead. |

## Compatibility

### Unsupported Packages
1 incompatible package detected: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (v1.19.6) — no compatible version for net10.0. Small count (1-3) → resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task. Removes old reference or adds replacement. No deferred work. |
| Defer Resolution | Generate minimal stubs to keep project building, create follow-up tasks for real replacements. |

### Unsupported API Handling
Binary and source incompatible APIs detected (Api.0001, Api.0002) across Web.csproj and PublicApi.csproj. Modern-to-modern upgrade — changes are typically minor. Default: Fix Inline.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred stubs. |
| Defer Complex Changes | Apply simple replacements inline; generate stubs for complex changes with follow-up subtasks. |
