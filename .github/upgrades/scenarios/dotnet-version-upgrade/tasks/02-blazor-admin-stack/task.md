# 02-blazor-admin-stack: Upgrade BlazorAdmin and BlazorShared to net10.0

## Objective

Upgrade `src/BlazorAdmin/BlazorAdmin.csproj` and `src/BlazorShared/BlazorShared.csproj` from net8.0 to net10.0, align the Blazor WebAssembly package set to 10.0.10, and verify the slice builds successfully.

## Research Findings

### Project Structure

**BlazorAdmin** (`Microsoft.NET.Sdk.BlazorWebAssembly`) — entry-point Blazor WASM app.
- 9 direct package references (previously), now 8 after removing the redundant `System.Net.Http.Json`
- References BlazorShared as a project dependency
- Key files: `Program.cs` (startup/DI), `Services/HttpService.cs` (HTTP + JSON deserialization)

**BlazorShared** (`Microsoft.NET.Sdk`) — shared UI contracts library.
- 2 direct package references: `BlazorInputFile`, `FluentValidation`
- No API issues flagged in assessment

### Assessment Issues (BlazorAdmin)

| Category | Count | Details |
|---|---|---|
| Package Issues | 7 | All 7 packages needed upgrading to 10.0.10 |
| API Issues | 3 | All behavioral changes (low impact) |

**Behavioral API changes reviewed:**
1. **`System.Net.Http.HttpContent`** — behavioral change in response content reading. `HttpService.cs` uses `ReadAsStringAsync()` + manual `JsonSerializer.Deserialize<T>`. Reviewed: no code change required; behavior is compatible with .NET 10.
2. **`System.Uri`** — behavioral change in URI parsing/normalization. `Program.cs` uses `new Uri(builder.HostEnvironment.BaseAddress)`. Reviewed: `HostEnvironment.BaseAddress` always returns a well-formed absolute URI; no code change required.
3. **`Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole`** — behavioral change in console output formatting. No explicit `AddConsole` call found in BlazorAdmin; logging is wired through `AddConfiguration`. No code change required.

### Package Decisions

| Package | Old Version | New Version | Notes |
|---|---|---|---|
| `Microsoft.AspNetCore.Components.Authorization` | 8.0.2 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `Microsoft.AspNetCore.Components.WebAssembly` | 8.0.2 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `Microsoft.AspNetCore.Components.WebAssembly.Authentication` | 8.0.2 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `Microsoft.AspNetCore.Components.WebAssembly.DevServer` | 8.0.2 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `Microsoft.Extensions.Identity.Core` | 8.0.2 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `Microsoft.Extensions.Logging.Configuration` | 8.0.0 | 10.0.10 | BlazorAdmin-exclusive → hardcoded |
| `System.Net.Http.Json` | 8.0.0 | n/a | **Removed** — included in net10.0 framework (NU1510 warning) |
| `Blazored.LocalStorage` | 4.5.0 | unchanged | ✅ Compatible |
| `BlazorInputFile` | 0.2.0 | unchanged | ✅ Compatible |
| `FluentValidation` | 11.9.0 | unchanged | ✅ Compatible |

**Note**: `$(AspNetVersion)` and `$(SystemExtensionVersion)` variables are intentionally left at 8.0.2/8.0.0 for packages used by Web, PublicApi, Infrastructure, and test projects not yet upgraded. Only packages exclusively used by this slice were updated per task instructions.

## Steps Executed

1. Added `<TargetFramework>net10.0</TargetFramework>` to `BlazorAdmin.csproj`
2. Added `<TargetFramework>net10.0</TargetFramework>` to `BlazorShared.csproj`
3. Updated 6 BlazorAdmin-exclusive package versions to 10.0.10 in `Directory.Packages.props`
4. Removed `System.Net.Http.Json` explicit reference from `BlazorAdmin.csproj` (now in framework)
5. Verified behavioral API changes — no code modifications required

## Build Result

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
