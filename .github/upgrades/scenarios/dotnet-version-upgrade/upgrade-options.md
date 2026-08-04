# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all net8.0 → net10.0, all SDK-style, Low difficulty; 1 incompatible package, 9 binary-incompatible APIs, 3 source-incompatible APIs

## Strategy

### Upgrade Strategy
All projects are on net8.0 (modern .NET), 10 projects (≤15), all Low difficulty, ≤3-tier dependency depth — no CI-green or multi-team constraints.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 10 projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. Appropriate for this small-scope, low-risk mechanical TFM bump. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries so the solution stays buildable throughout. |

## Compatibility

### Unsupported Packages
Microsoft.VisualStudio.Azure.Containers.Tools.Targets (v1.19.6) has no compatible version for net10.0; 1 package requires inline resolution.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same task. Remove or replace the reference and rewrite any consuming code. No deferred work. |
| Defer Resolution | Generate minimal stubs to keep the project compiling and create follow-up tasks for the real replacement. |

### Unsupported API Handling
9 binary-incompatible APIs (ConfigurationBinder.Get/Configure/GetValue) and 3 source-incompatible APIs (Exception serialization ctor, TimeSpan.FromMinutes) detected; this is a modern-to-modern upgrade where changes are minor.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. Leaves no deferred work or stubs to clean up later. |
| Defer Complex Changes | Apply simple replacements inline; generate minimal stubs for complex changes and create resolution subtasks. |
