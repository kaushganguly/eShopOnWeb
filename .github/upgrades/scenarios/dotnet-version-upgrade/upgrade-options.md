# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0 → net10.0), 119 issues (21 mandatory), 1 incompatible package (PublicApi), binary/source API breaks in PublicApi and Web, centralized package management via Directory.Packages.props

## Strategy

### Upgrade Strategy
10 projects all on net8.0 with centralized TFM + package management — a single Directory.Packages.props change updates all projects simultaneously. 2 high-risk projects (binary-incompatible: PublicApi, Web), ≤15 projects total, making a single atomic pass practical.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 10 projects simultaneously in a single atomic pass. Fastest approach; leverages centralized Directory.Packages.props for TFM and package version changes in one edit. |
| Top-Down | Upgrade entry-point apps first with multi-targeting on shared libs, then consolidate. More overhead for this solution size. |

## Compatibility

### Unsupported Packages
1 incompatible package detected (NuGet.0001 in PublicApi — Microsoft.AspNetCore.Mvc 2.2.0). Small count allows inline resolution without deferred stubs.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the 1 incompatible package within the upgrade task. Remove the old reference and replace with the framework-included equivalent or compatible version. |
| Defer Resolution | Generate minimal stubs and create follow-up tasks. Overkill for a single package. |

### Unsupported API Handling
Binary-incompatible APIs detected in PublicApi (9 occurrences) and Web (Source-incompatible in ApplicationCore and Web). Modern-to-modern upgrade — API changes are expected to be minor and few.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API breaking changes within the same upgrade task. Applicable replacements applied directly with no deferred work. Appropriate for modern-to-modern upgrades with few flagged changes. |
| Defer Complex Changes | Stub complex replacements and create follow-up resolution tasks. Not recommended here given low API change count. |

## Modernization

### Nullable Reference Types
BlazorAdmin, BlazorShared, and IntegrationTests do not have nullable enabled. With 10 projects and ongoing breaking-change work, enabling nullable would generate unmanageable warnings during migration.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Do not enable nullable on projects that don't already have it. Maintains existing null handling. Can be enabled separately post-migration as a dedicated effort. |
| Enable Nullable Reference Types | Add `<Nullable>enable</Nullable>` to all projects. Not recommended during an active framework migration. |
