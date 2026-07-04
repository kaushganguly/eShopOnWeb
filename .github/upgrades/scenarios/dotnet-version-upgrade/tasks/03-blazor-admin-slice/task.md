# 03-blazor-admin-slice: Upgrade BlazorAdmin and BlazorShared to net10.0

## Objective
Upgrade the `BlazorAdmin` WebAssembly client together with `BlazorShared`, which is its only direct project dependency and also a dependency of `Web`. Leave the hosted relationship in a state that the later `Web` task can consume.

## Scope

### Projects
- `src/BlazorShared/BlazorShared.csproj` — 0 code issues (framework-only change via Directory.Packages.props)
- `src/BlazorAdmin/BlazorAdmin.csproj` — 7 package issues, 3 API issues, 2 pre-existing Razor RZ1021 bugs

### Key Research Findings

#### BlazorShared
- No changes required beyond the global `TargetFramework=net10.0` (already set in Directory.Packages.props by task 01)
- Built clean on first attempt: 0 errors, 0 warnings

#### BlazorAdmin
- **Package issue (NU1510)**: `System.Net.Http.Json` is now part of the base `net10.0` framework; explicit `PackageReference` triggers NU1510 warning. Fix: remove from csproj.
- **Razor RZ1021 (pre-existing, unmasked by .NET 10 recompile)**: In .NET 10's Razor compiler, Blazor component elements placed DIRECTLY as the first element of an `@if`/`else` code block trigger RZ1021. Fix: wrap component(s) in a `<div>`.
- **Razor void element compliance**: `<img>` tags inside `@if`/`@foreach` code blocks must be self-closing. Fixed across 5 affected files.

### Affected Files
| File | Change |
|------|--------|
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed explicit `System.Net.Http.Json` PackageReference |
| `src/BlazorAdmin/Pages/CatalogItemPage/List.razor` | Wrapped Spinner/modal components in div; self-closed img |
| `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` | Self-closed img |
| `src/BlazorAdmin/Pages/CatalogItemPage/Delete.razor` | Self-closed img |
| `src/BlazorAdmin/Pages/CatalogItemPage/Create.razor` | Self-closed img |
| `src/BlazorAdmin/Pages/CatalogItemPage/Details.razor` | Self-closed img |

## Build Validation
- `BlazorShared`: 0 errors, 0 warnings
- `BlazorAdmin`: 0 errors, 0 warnings
