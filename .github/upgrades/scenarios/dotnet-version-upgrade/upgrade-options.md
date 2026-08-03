# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects targeting net10.0, with a 5-level dependency graph, 7 incompatible packages, and 12 binary/source incompatible API findings.

## Strategy

### Upgrade Strategy
This option is required for every upgrade, and the assessment shows a modern-to-modern upgrade with 10 projects and a 5-level dependency graph.

| Value | Description |
|-------|-------------|
| All-at-Once | Upgrade all projects in one atomic pass for the fastest end-to-end migration. |
| **Top-Down** (selected) | Upgrade entry-point apps first and keep the solution buildable by temporarily multi-targeting shared libraries. |

## Compatibility

### Unsupported Packages
This option is relevant because the assessment reports 7 incompatible packages for net10.0, including Microsoft.VisualStudio.Azure.Containers.Tools.Targets with no supported upgrade path.

| Value | Description |
|-------|-------------|
| Resolve Inline | Research and replace each incompatible package during the same upgrade task with no deferred follow-up work. |
| **Defer Resolution** (selected) | Remove or condition incompatible references, use minimal stubs when needed, and create follow-up work for final replacements. |
| Compatibility Mode | Keep the old framework reference temporarily for transitive-only cases, accepting higher runtime risk. |

### Unsupported API Handling
This option is relevant because the assessment flags 9 binary-incompatible and 3 source-incompatible API issues across the solution.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Apply simple replacements and resolve complex API changes within the same upgrade tasks. |
| Defer Complex Changes | Fix simple API changes now and use temporary stubs plus follow-up tasks for complex replacements. |
