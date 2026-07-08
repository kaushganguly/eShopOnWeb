# Progress Details: 02-blazor-admin-surface

## Summary

Upgraded BlazorAdmin to net10.0 (primary browser WASM output) with a net8.0 compatibility slice so the
existing Web host project (still net8.0) can maintain its project reference during the mixed-target phase.
BlazorShared was multi-targeted to `net8.0;net10.0` to serve both the net8.0 downstream consumers
(ApplicationCore, Web) and the net10.0 BlazorAdmin slice. Three API incidents were addressed: two HTTP
streaming behavioral changes fixed by switching to `ReadFromJsonAsync`, one URI informational change
requiring no code modification.

A root-cause infrastructure fix was required: `TargetFramework=net8.0` was removed from
`Directory.Packages.props` (where it acted as a pseudo-global property, blocking multi-targeting) and
placed in a new `Directory.Build.props` (a regular import that project files can clear and override).
The pattern `<TargetFramework /> + <TargetFrameworks>…</TargetFrameworks>` in project files clears the
inherited default and enables cross-targeting.

## Files Changed

### `Directory.Build.props` (new file)
| Change | Detail |
|--------|--------|
| Created | Default `<TargetFramework>net8.0</TargetFramework>` for all projects |
| Purpose | Replaces the pseudo-global property that was in Directory.Packages.props; allows per-project overrides via `<TargetFramework />` + `<TargetFrameworks>` |

### `Directory.Packages.props`
| Field | Before | After |
|-------|--------|-------|
| `<TargetFramework>` property | `net8.0` (blocked multi-targeting) | Removed |
| `<BlazorWasmVersion>` property | (did not exist) | `10.0.9` |
| `Microsoft.AspNetCore.Components.Authorization` version | `$(AspNetVersion)` → 8.0.2 | `$(BlazorWasmVersion)` → 10.0.9 |
| `Microsoft.AspNetCore.Components.WebAssembly` version | `$(AspNetVersion)` → 8.0.2 | `$(BlazorWasmVersion)` → 10.0.9 |
| `Microsoft.AspNetCore.Components.WebAssembly.Authentication` version | `$(AspNetVersion)` → 8.0.2 | `$(BlazorWasmVersion)` → 10.0.9 |
| `Microsoft.AspNetCore.Components.WebAssembly.DevServer` version | `$(AspNetVersion)` → 8.0.2 | `$(BlazorWasmVersion)` → 10.0.9 |
| `Microsoft.Extensions.Identity.Core` version | `$(AspNetVersion)` → 8.0.2 | `$(BlazorWasmVersion)` → 10.0.9 |
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | unchanged | `$(AspNetVersion)` → 8.0.2 (Web host, stays net8.0 compatible) |

### `src/BlazorAdmin/BlazorAdmin.csproj`
| Change | Detail |
|--------|--------|
| Target frameworks | Single (inherited net8.0) → `net8.0;net10.0` (cleared default with `<TargetFramework />`) |
| Package split | net8.0 slice: `VersionOverride="8.0.2"` for 5 net10.0-only packages |
| Package split | net10.0 slice: uses BlazorWasmVersion (10.0.9) from Directory.Packages.props |
| `System.Net.Http.Json` | Removed (NU1510: redundant — built into .NET 8/10 BCL) |

### `src/BlazorShared/BlazorShared.csproj`
| Change | Detail |
|--------|--------|
| Target frameworks | Single (inherited net8.0) → `net8.0;net10.0` (cleared default with `<TargetFramework />`) |
| Packages | Unchanged (FluentValidation 11.9.0, BlazorInputFile 0.2.0 are TFM-agnostic compatible) |

### `src/BlazorAdmin/Services/HttpService.cs`
| Change | Detail |
|--------|--------|
| Added `using System.Net.Http.Json;` | Required for `ReadFromJsonAsync` extension method |
| `FromHttpResponseMessage<T>` | `ReadAsStringAsync() + JsonSerializer.Deserialize<T>()` → `ReadFromJsonAsync<T>()` |
| `HttpPost<T>` error branch | `ReadAsStringAsync() + JsonSerializer.Deserialize<ErrorDetails>()` → `ReadFromJsonAsync<ErrorDetails>()` |
| `_toastService.ShowToast` call | `exception.Message` → `exception?.Message` (null-safe after ReadFromJsonAsync) |

## API Incidents Resolved

| Incident | File | Rule | Resolution |
|----------|------|------|-----------|
| Streaming HTTP responses (HttpContent) | Services/HttpService.cs:91 | Api.0003 | Replaced with ReadFromJsonAsync — stream-native, eliminates buffering via ReadAsStringAsync |
| Streaming HTTP responses (HttpContent) | Services/HttpService.cs:57 | Api.0003 | Same fix in error-response branch |
| URI length limits removed (Uri) | Program.cs:22 | Api.0003 | Informational behavioral change; no code modification needed |

## Build Results

| Project | TFM(s) | Errors | Warnings |
|---------|--------|--------|---------|
| BlazorShared | net8.0, net10.0 | 0 | 0 |
| BlazorAdmin | net8.0, net10.0 | 0 | 0 |
| Web (mixed-target check) | net8.0 | 0 | 1* |
| Full solution | net8.0 (all), net10.0 (Blazor) | 0 | 5* |

\* SYSLIB0051 (ApplicationCore) and xUnit2013 (test projects) are pre-existing warnings scoped to
future tasks; none are in the projects modified by this task.

## Test Results

| Suite | Total | Passed | Failed | Skipped |
|-------|-------|--------|--------|---------|
| UnitTests | 44 | 44 | 0 | 0 |
| IntegrationTests | 3 | 3 | 0 | 0 |
| FunctionalTests | 12 | 12 | 0 | 0 |
| PublicApiIntegrationTests | 15 | 15 | 0 | 0 |
| **Total** | **74** | **74** | **0** | **0** |

## Mixed-Target Compatibility Verified

- Web (net8.0) → BlazorAdmin (net8.0 slice): project reference resolves ✓
- Web (net8.0) → BlazorShared (net8.0 slice): project reference resolves ✓
- ApplicationCore (net8.0) → BlazorShared (net8.0 slice): project reference resolves ✓
- BlazorAdmin (net10.0) → BlazorShared (net10.0 slice): matching TFM ✓
- `dotnet build src/Web/Web.csproj`: Build succeeded, 0 errors ✓

## Package Version Watchlist for Subsequent Tasks

| Package | Current | Scope |
|---------|---------|-------|
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | 8.0.2 | Web TFM task — update when Web moves to net10.0 |
| BlazorAdmin net8.0 VersionOverrides | 8.0.2 | Remove when Web TFM task completes (BlazorAdmin can drop net8.0 slice) |
| BlazorShared net8.0 TFM | retained | Remove when last net8.0 consumer (ApplicationCore/Web) upgrades |
