# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects with a 6-level dependency graph; 1 incompatible package without a supported replacement, 1 framework-included package to remove, and 12 binary/source-incompatible API findings.

## Strategy

### Upgrade Strategy
The assessment shows a modern-to-modern upgrade across 10 projects with a 6-level dependency graph, so an incremental strategy is the safer default.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout. |
| All-at-Once | Upgrade all projects in one atomic pass with no multi-targeting, accepting temporary solution breakage during the migration. |

## Compatibility

### Unsupported Packages
The assessment surfaced one mandatory incompatible package with no supported target-framework version in `src/PublicApi/PublicApi.csproj`, so a package-resolution approach is required.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package in the same task by removing it, replacing it, or rewriting consuming code with no deferred follow-up work. |
| Defer Resolution | Make the project compile first with minimal stubs, then create follow-up tasks to replace the incompatible package later. |
| Compatibility Mode | Keep the .NET Framework reference assemblies and suppress compatibility warnings for limited transitive-only scenarios, with runtime risk. |

### Unsupported API Handling
The assessment identified 9 binary-incompatible and 3 source-incompatible API issues, primarily in `PublicApi` and `Web`, so an explicit handling mode is needed.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve each API change in the same task, including complex changes, so no stub cleanup remains afterward. |
| Defer Complex Changes | Apply simple replacements now, but use temporary stubs and follow-up subtasks for complex API migrations. |
