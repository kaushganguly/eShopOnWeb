# Upgrade Options — eShopOnWeb

Assessment: 10 projects (all net8.0, modern .NET), 1 incompatible package, binary/source API changes in Web and PublicApi, security vulnerability in Azure.Identity

## Strategy

### Upgrade Strategy
All 10 projects target net8.0 (modern .NET), solution has ≤15 projects and ≤5-tier depth with no CI-green or multi-team constraints — straightforward mechanical TFM bump.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, multi-targeting shared libraries to keep solution buildable. Use for 15+ projects or CI-green constraints. |

## Compatibility

### Unsupported Packages
Assessment found 1 incompatible package (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`) with no compatible version for net10.0. Small enough to resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task — removes old reference or finds replacement. No deferred work. |
| Defer Resolution | Generate minimal stubs to keep project building, then create follow-up tasks for real replacements. |
| Compatibility Mode | Keeps .NET Framework reference with NU1701 suppression. Risk of runtime failures. |

### Unsupported API Handling
Binary and source incompatible APIs flagged in src/Web and src/PublicApi (Api.0001, Api.0002). Modern-to-modern upgrade (net8.0 → net10.0) with minor API changes.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No stubs to clean up later. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create resolution subtasks. |
