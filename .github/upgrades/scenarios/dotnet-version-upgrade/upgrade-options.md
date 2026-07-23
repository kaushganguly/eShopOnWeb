# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects upgrading to net10.0 with a 5-level dependency graph, 7 incompatible packages, and binary/source incompatible API findings across multiple projects.

## Strategy

### Upgrade Strategy
This option is relevant because the solution is a modern-to-modern upgrade with 10 projects, a 5-level dependency graph, and multiple compatibility signals that favor an incremental, buildable migration path.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-target shared libraries, and consolidate libraries afterward to keep the solution buildable during migration. |
| All-at-Once | Upgrade all projects in a single pass for the fastest path, accepting that the solution may be temporarily broken until all updates are complete. |

## Compatibility

### Unsupported Packages
This option is relevant because the assessment found 7 incompatible packages with no compatible target-framework version, including deprecated and removable dependencies.

| Value | Description |
|-------|-------------|
| Resolve Inline | Research and replace each incompatible package within the same task so no package-resolution work is deferred. |
| **Defer Resolution** (selected) | Remove or condition out incompatible packages, use minimal stubs to keep projects compiling, and create follow-up work for full replacement. |
| Compatibility Mode | Keep the .NET Framework reference assemblies path as a stopgap for limited cases such as transitive or Windows-only dependencies, with runtime risk. |

### Unsupported API Handling
This option is relevant because the assessment found binary incompatible API issues in PublicApi and Web and source incompatible API issues in ApplicationCore and Web.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Apply simple and complex API fixes within the same task so the migration completes without deferred API cleanup work. |
| Defer Complex Changes | Apply simple fixes immediately, then use minimal stubs and follow-up tasks for complex API changes that need separate resolution. |
