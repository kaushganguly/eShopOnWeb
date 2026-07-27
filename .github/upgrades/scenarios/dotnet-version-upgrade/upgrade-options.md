# Upgrade Options — eShopOnWeb

Assessment: 10 projects (net8.0), 119 issues (21 mandatory), 1 incompatible package, binary/source API breaking changes in Web and PublicApi

## Strategy

### Upgrade Strategy
All projects target net8.0 (modern .NET), 10 projects with clear dependency structure — mechanical TFM bump is the right approach.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 10 projects simultaneously in a single atomic pass. Fastest approach; solution may be temporarily broken until all projects are updated. |
| Top-Down | Upgrade entry-point applications first, temporarily multi-targeting shared libraries. More overhead for this solution size. |

## Compatibility

### Unsupported Packages
`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` is incompatible with net10.0 (1 package, no compatible version).

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve the incompatible package within the same task. For 1 package, inline resolution is efficient. |
| Defer Resolution | Generate stub, create follow-up task. Unnecessary overhead for a single package. |

### Unsupported API Handling
Api.0001 (binary incompatible) flagged for Web.csproj and PublicApi.csproj; Api.0002 (source incompatible) for ApplicationCore and Web.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API changes in the same task, including complex ones. Modern-to-modern upgrade; changes are expected to be manageable. |
| Defer Complex Changes | Generate stubs for complex API changes, create resolution subtasks. Not needed for this scale. |
