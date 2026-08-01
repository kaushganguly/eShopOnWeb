# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), 5-tier dependency graph, 1 incompatible package, API breaking changes in Web/PublicApi, security vulnerabilities in Azure.Identity

## Strategy

### Upgrade Strategy
All projects are on net8.0 (modern .NET), 10 projects total (≤15), with 1 incompatible package and API changes across 2 projects. Ambiguous signals (depth=5 but project count=10) default to All-at-Once for solutions ≤15 projects.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach with no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first with temporary multi-targeting of shared libraries. |

## Compatibility

### Unsupported Packages
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (v1.19.6) has no compatible version for net10.0. Only 1 incompatible package — small enough to resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same upgrade task. Remove or replace the reference. |
| Defer Resolution | Generate a minimal stub to keep the project building, create a follow-up task for real resolution. |

### Unsupported API Handling
Binary-incompatible APIs in PublicApi and Web; source-incompatible APIs in ApplicationCore and Web. Modern-to-modern upgrade (net8.0 → net10.0) — changes are expected to be minor.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred work. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create resolution subtasks. |

## Modernization

### Nullable Reference Types
Target is net10.0 (supports nullable); 10 projects exceeds the ≤5 threshold — enabling nullable would generate unmanageable warnings during migration.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Does not enable nullable. Maintains existing null handling. Enable separately after migration. |
| Enable Nullable Reference Types | Adds `<Nullable>enable</Nullable>` to project files for compile-time null safety. |
