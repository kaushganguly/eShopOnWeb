# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects targeting net10.0, with a 5-level dependency graph, 7 incompatible packages, 19 package upgrades, and binary/source API changes in PublicApi and Web.

## Strategy

### Upgrade Strategy
Ten modern .NET projects with a 5-level dependency graph and multiple high-risk package/API findings make an incremental app-first migration the safer default.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout. |
| All-at-Once | Upgrade all projects simultaneously in a single atomic pass. |

## Compatibility

### Unsupported Packages
The assessment reports 7 incompatible packages for net10.0, which is above the inline-resolution threshold and favors deferring research-heavy replacements.

| Value | Description |
|-------|-------------|
| Resolve Inline | Research and resolve each incompatible package within the same task so no deferred package work remains. |
| **Defer Resolution** (selected) | Remove or condition incompatible package references, use minimal stubs when needed, and create follow-up tasks for the real replacements. |
| Compatibility Mode | Keep a .NET Framework reference path for limited transitive cases, accepting higher runtime risk. |

### Unsupported API Handling
The assessment flags 9 binary-incompatible and 3 source-incompatible API changes, but this remains a modern-to-modern upgrade where fixes should stay inline with each project task.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones, so no API stub cleanup is deferred. |
| Defer Complex Changes | Apply simple replacements inline and use minimal compilable stubs plus follow-up subtasks for complex API changes. |
