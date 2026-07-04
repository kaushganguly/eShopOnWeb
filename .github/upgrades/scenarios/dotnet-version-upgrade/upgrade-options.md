# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style projects, all net8.0 → net10.0; 5 incompatible packages; 12 binary/source incompatible API changes; moderate 6-level dependency graph.

## Strategy

### Upgrade Strategy
This option is relevant because the assessment shows a modern-to-modern upgrade across 10 projects with a 6-level dependency graph, where incremental buildability is the safer default.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout the migration. |
| All-at-Once | Upgrade all projects simultaneously in a single atomic pass; fastest, but the solution may be temporarily broken until all projects are updated. |

## Compatibility

### Unsupported Packages
This option is relevant because the assessment reports 5 incompatible packages for the target framework, which requires an explicit handling strategy.

| Value | Description |
|-------|-------------|
| **Defer Resolution** (selected) | Keep projects compiling by conditioning out or stubbing unresolved incompatible packages during migration, then create follow-up tasks for the real replacements. |
| Resolve Inline | Research and resolve each incompatible package within the same task, replacing or removing it with no deferred work. |
| Compatibility Mode | Keep .NET Framework reference assemblies and suppress NU1701 for transitive or Windows-only dependencies, accepting runtime risk. |

### Unsupported API Handling
This option is relevant because the assessment flags 9 binary incompatible and 3 source incompatible API changes that must be addressed during the upgrade.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones, so no deferred stub cleanup remains. |
| Defer Complex Changes | Apply simple replacements inline, but stub complex API changes and create follow-up resolution subtasks. |
