# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects targeting net10.0 with a 5-tier dependency graph, 1 incompatible package, 12 binary/source API breaks, and 49 behavioral changes.

## Strategy

### Upgrade Strategy
This modern-to-modern upgrade spans 10 projects with a 5-tier dependency graph and multiple mandatory issues, so incremental buildability is the safer default.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout. |
| All-at-Once | Upgrade all projects simultaneously in a single atomic pass with no multi-targeting overhead. |

## Compatibility

### Unsupported Packages
The assessment found 1 incompatible package for net10.0, which is small enough to research and resolve during normal upgrade work.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package in the same task by removing, replacing, or rewriting usage as needed. |
| Defer Resolution | Make the project compile with temporary stubs and create follow-up tasks for the real package replacement. |
| Compatibility Mode | Keep the .NET Framework reference path with compatibility shims for edge cases such as transitive-only dependencies. |

### Unsupported API Handling
The assessment identified 9 binary incompatible and 3 source incompatible API changes, but this remains a modern-to-modern upgrade where fixing changes inline is the default.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones, with no deferred stub cleanup. |
| Defer Complex Changes | Apply simple replacements inline and use temporary stubs plus follow-up tasks for complex API migrations. |
