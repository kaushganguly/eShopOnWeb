# Upgrade Options — eShopOnWeb

Assessment: 10 SDK-style projects currently on net8.0 with centralized TargetFramework and package management, upgrading to net10.0; assessment surfaced 5 incompatible packages and 12 breaking API changes.

## Strategy

### Upgrade Strategy
This modern-to-modern upgrade spans 10 projects across a 6-level dependency graph, so the assessment complexity favors an incremental strategy that keeps the solution buildable.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, temporarily multi-target shared libraries as needed, and consolidate libraries afterward to preserve incremental buildability. |
| All-at-Once | Upgrade all projects in one atomic pass for the fastest path, accepting that the solution may be broken until every project is updated. |

## Compatibility

### Unsupported Packages
The assessment found 5 incompatible package instances for the target framework, including `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`, which makes package-resolution strategy relevant.

| Value | Description |
|-------|-------------|
| Resolve Inline | Research and replace each incompatible package during the same task so no deferred package work remains. |
| **Defer Resolution** (selected) | Keep projects compiling with minimal stubs or conditioned references first, then create follow-up work for full package replacement. |
| Compatibility Mode | Keep legacy framework references with compatibility shims for limited cases where package APIs are not directly used. |

### Unsupported API Handling
The assessment surfaced 12 binary/source incompatible API issues across multiple projects, so API-handling policy is required for the upgrade plan.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Apply simple and complex API fixes during the main upgrade tasks so no API stub follow-up work is left behind. |
| Defer Complex Changes | Fix simple API changes inline and use minimal compilable stubs plus follow-up tasks for complex replacements. |
