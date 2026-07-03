# Upgrade Options — eShopOnWeb

Assessment: 10 projects on net8.0 (6 src + 4 test), 117 issues (21 mandatory), 5-tier dependency depth, incompatible packages in PublicApi, binary/source incompatible APIs in Web and PublicApi.

## Strategy

### Upgrade Strategy
Solution has 5-tier dependency depth (exceeds the ≤3-tier threshold for All-at-Once) and multiple high-risk applications (Web: 29 issues, PublicApi: 17 issues). Top-Down keeps applications upgradeable first while libraries follow.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications (Web, PublicApi) first; shared libraries get upgraded alongside. Phase 2 removes any legacy targets. |
| All-at-Once | Upgrade all projects simultaneously in a single pass. |

## Compatibility

### Unsupported Packages
PublicApi references `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` which has no compatible version for net10.0.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the 1 incompatible package within the same task. Remove incompatible reference and replace or remove consuming code. |
| Defer Resolution | Generate stubs and create follow-up tasks for real resolution. |

### Unsupported API Handling
Web and PublicApi have binary/source-incompatible APIs (ConfigurationBinder.Get<T>, OptionsConfigurationServiceCollectionExtensions.Configure<T>).

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change — including complex ones — within the same task. No deferred work. |
| Defer Complex Changes | Apply simple replacements inline; stub complex ones for follow-up tasks. |

## Modernization

### Nullable Reference Types
10 projects exceed the ≤5-project threshold for enabling nullable during migration.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Do not enable nullable reference types. Enable separately after migration as a distinct effort. |
| Enable Nullable Reference Types | Add `<Nullable>enable</Nullable>` to all project files. |
