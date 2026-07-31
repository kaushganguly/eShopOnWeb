# Upgrade Options — eShopOnWeb

Assessment: 10 projects on net8.0 (modern .NET), all SDK-style, 1 incompatible package, API breaking changes in 2 projects (Api.0001, Api.0002), security vulnerability in Azure.Identity

## Strategy

### Upgrade Strategy
All projects are on net8.0 (modern .NET), solution has 10 projects (≤15), and the upgrade is a mechanical TFM bump with no .NET Framework migration required.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 10 projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade entry-point apps first, temporarily multi-targeting shared libraries. Adds overhead not warranted for this small solution. |

## Compatibility

### Unsupported Packages
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (v1.19.6) has no compatible version for net10.0. Only 1 incompatible package — small enough to resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same task. Remove or replace. |
| Defer Resolution | Generate a stub to keep the project building and create a follow-up task. Not needed for 1 package. |

### Unsupported API Handling
API breaking changes detected: Api.0001 (binary incompatible) in PublicApi and Web; Api.0002 (source incompatible) in ApplicationCore and Web. This is a modern-to-modern upgrade — changes are expected to be minor.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API changes in the same task, including complex ones. Leaves no deferred work. |
| Defer Complex Changes | Apply simple replacements inline; stub complex ones for later tasks. |

## Modernization

### Nullable Reference Types
Some projects already have `<Nullable>enable</Nullable>` (ApplicationCore, Infrastructure, PublicApi, Web, PublicApiIntegrationTests); others do not. Solution has 10 projects (>5) — volume of warnings would be unmanageable during migration.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Do not enable nullable project-wide. Projects that already have it keep it; others are not modified. |
| Enable Nullable Reference Types | Add `<Nullable>enable</Nullable>` to all project files. May require significant code updates. |
