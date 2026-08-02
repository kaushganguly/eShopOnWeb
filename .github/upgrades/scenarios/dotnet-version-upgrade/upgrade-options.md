# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), 4 code tiers, 1 incompatible package (no replacement), 19 packages need upgrade, 2 security vulnerabilities

## Strategy

### Upgrade Strategy
All projects target net8.0 (modern .NET), 10 projects (≤15), 2 high-risk API changes, depth 4 code tiers — All-at-Once is the appropriate default.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. More complex but keeps solution buildable throughout. |

## Compatibility

### Unsupported Packages
1 package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) has no compatible version for net10.0. Resolve Inline applies since count is ≤3.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task. Removes old reference and adds replacement or rewrites consuming code. |
| Defer Resolution | Make project compile without the package by generating minimal stubs, then create follow-up tasks. |
| Compatibility Mode | Keeps reference and suppresses NU1701. May cause runtime failures. |

### Unsupported API Handling
Binary and source incompatible APIs detected in PublicApi and Web (Api.0001, Api.0002). Modern-to-modern upgrade — changes are expected to be minor.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred work. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create resolution subtasks. |

## Modernization

### Nullable Reference Types
10 projects exceeds the ≤5 threshold for enabling nullable during migration. Defer to a separate effort.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Does not enable nullable. Maintains existing null handling. Enable separately after migration. |
| Enable Nullable Reference Types | Adds `<Nullable>enable</Nullable>` to project files. May require code updates. |
