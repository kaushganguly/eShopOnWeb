# Upgrade Options — eShopOnWeb

Assessment: 10 projects (all net8.0, all SDK-style), 26 packages need upgrade, 1 incompatible package, 12 API breaking changes (9 binary incompatible, 3 source incompatible)

## Strategy

### Upgrade Strategy
10 projects all on net8.0 (modern .NET), ≤15 project count, ≤2 high-risk migrations, no CI-green constraint — All-at-Once is ideal.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. Use when CI must stay green or 15+ projects. |

## Compatibility

### Unsupported Packages
1 incompatible package found: Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6 — small count, resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task. No deferred work. |
| Defer Resolution | Create stub and follow-up tasks for incompatible packages. Use when > 3 packages have no known replacement. |

### Unsupported API Handling
Breaking API changes flagged: 9 binary incompatible, 3 source incompatible — modern-to-modern upgrade with minor changes, fix inline.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No stubs to clean up later. |
| Defer Complex Changes | Apply simple replacements inline; defer complex changes to stub resolution subtasks. |
