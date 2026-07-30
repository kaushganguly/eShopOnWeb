# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style net8.0 projects targeting net10.0, with incompatible/deprecated packages across the solution and binary/source API incompatibilities concentrated in Web and PublicApi.

## Strategy

### Upgrade Strategy
This modern-to-modern upgrade spans 10 projects and includes security/package pressure plus API incompatibilities in the entry-point applications, so an incremental app-first strategy is relevant.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout. |
| All-at-Once | Upgrade all projects simultaneously in a single atomic pass with no multi-targeting overhead, but accept temporary solution breakage during the transition. |

## Compatibility

### Unsupported Packages
The assessment surfaced 7 incompatible package issues across the solution, including deprecated or unsupported package references that need explicit resolution for net10.0.

| Value | Description |
|-------|-------------|
| Resolve Inline | Research and resolve each incompatible package in the same task by replacing, removing, or rewriting usage immediately. |
| **Defer Resolution** (selected) | Make projects compile first by removing or conditioning incompatible packages and creating follow-up work for the real replacements. |
| Compatibility Mode | Keep the .NET Framework reference path with suppression-based compatibility handling for limited transitive or Windows-only cases. |

### Unsupported API Handling
The assessment found binary and source incompatible API changes, with the highest concentration in Web and PublicApi, so API-resolution behavior must be defined.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API changes in the same task, including complex ones, so no stub cleanup is deferred. |
| Defer Complex Changes | Apply simple replacements now and use minimal stubs plus follow-up tasks for complex API migrations. |
