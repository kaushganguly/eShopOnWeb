# 05-library-and-test-consolidation

## Objective

Remove temporary net8.0 compatibility from shared libraries and remaining tests and confirm a clean single-target net10.0 state.

## Scope

- Shared libraries: `ApplicationCore`, `Infrastructure`, `BlazorShared`
- Application layer: `BlazorAdmin`, `Web`, `PublicApi`
- Test projects: `UnitTests`, `IntegrationTests`, `FunctionalTests`, `PublicApiIntegrationTests`
- Central configuration: `Directory.Packages.props`

## Research Findings

### Target Framework State

All projects in the solution already target `net10.0` exclusively via the central `Directory.Packages.props`:

```xml
<TargetFramework>net10.0</TargetFramework>
```

No individual project file overrides this. No multi-targeting was introduced anywhere.

### net8.0 References

- **Source files**: Zero `net8` references in any `.csproj` or `.props` source file.
- **Obj/ artifacts**: Two generated `nuget.g.props` files in `obj/` contain net8 strings — these are build-system cache files, not source artifacts.

### Conditional Compilation

No `#if NET8_0` or `<Condition>` blocks gating net8.0 were introduced in any prior task. Nothing to remove.

### Orphaned PackageVersion Declarations

Three packages in `Directory.Packages.props` were no longer referenced by any project:

| Package | Reason |
|---|---|
| `System.Security.Claims` 4.3.0 | Inbox on net10.0; removed from ApplicationCore per NU1510 |
| `System.Net.Http.Json` 10.0.9 | Inbox on net10.0; removed from BlazorAdmin |
| `System.Text.Json` 10.0.9 | Inbox on net10.0; removed from ApplicationCore per NU1510 |

## Changes Made

1. **`Directory.Packages.props`**: Removed orphaned `PackageVersion` declarations for `System.Net.Http.Json`, `System.Security.Claims`, and `System.Text.Json`.

## Affected Files

- `Directory.Packages.props`
