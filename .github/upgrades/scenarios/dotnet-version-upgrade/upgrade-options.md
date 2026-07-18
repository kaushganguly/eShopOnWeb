# Upgrade Options — eShopOnWeb

Assessment: 10 projects, all net8.0, all SDK-style, 1 incompatible package, 9 binary-incompatible APIs, Central Package Management already in use.

## Strategy

### Upgrade Strategy
All projects are net8.0, SDK-style, and low complexity — ideal for a single atomic upgrade pass.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single operation |
| Bottom-Up | Upgrade leaf dependencies first, then consumers tier by tier |
| Top-Down | Upgrade applications first, add multi-targeting to libraries as needed |

## Compatibility

### Unsupported Packages
1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) — small enough to resolve inline.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task |
| Defer Resolution | Generate minimal stubs to keep compiling, then create follow-up resolution tasks |
| Compatibility Mode | Keep old .NET Framework reference and suppress NU1701; may cause runtime failures |

### Unsupported API Handling
Binary-incompatible APIs detected (ConfigurationBinder.Get, OptionsConfigurationServiceCollectionExtensions.Configure, Exception serialization constructor, TimeSpan.FromMinutes). Modern-to-modern upgrade with few breaking API changes — fix inline.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create resolution subtasks |
