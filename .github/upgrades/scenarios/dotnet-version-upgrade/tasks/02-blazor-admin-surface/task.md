# 02-blazor-admin-surface: Upgrade BlazorAdmin and BlazorShared for the first net10.0 entry point

This task upgrades the simplest application path first: BlazorAdmin with its BlazorShared dependency. BlazorShared has only the framework-target change, while BlazorAdmin carries 7 package issues and 3 API incidents, all in a relatively shallow dependency slice. Because Web still references both projects from net8.0, this task must preserve cross-project compatibility while BlazorAdmin moves ahead as the pilot application for the top-down rollout.

Research should focus on the WebAssembly package set, System.Net.Http.Json, logging/configuration version alignment, and whether BlazorAdmin itself needs temporary dual-targeting while Web remains on net8.0. Validation for this task is not limited to the upgraded app; it also needs a mixed-target check proving that the unchanged Web project can still consume the shared Blazor components until its own task begins.

**Done when**: BlazorAdmin builds and runs on net10.0, BlazorShared provides the compatibility required for the mixed-target phase, and the remaining net8.0 Web path still compiles against the upgraded Blazor projects.

---

## Research Findings

### Global TargetFramework Issue
`Directory.Packages.props` originally set `<TargetFramework>net8.0</TargetFramework>` as a global
property. This prevented per-project `TargetFrameworks` (plural) from enabling multi-targeting, because
the .NET SDK's cross-targeting logic gates on `'$(TargetFramework)' == ''`. Fix: move the default TFM
to `Directory.Build.props` (a regular import, not the NuGet global import chain), and in project files
that need to multi-target, clear the inherited property with `<TargetFramework />` before setting
`<TargetFrameworks>`.

### Package Split Strategy
WASM client packages (Authorization, WebAssembly, WebAssembly.Authentication, WebAssembly.DevServer,
Extensions.Identity.Core) are net10.0-only at version 10.0.9. Web (net8.0) does NOT reference these
directly — Web only uses `WebAssembly.Server` (the host-side package, stays at AspNetVersion=8.0.2).
Solution: introduce `<BlazorWasmVersion>10.0.9</BlazorWasmVersion>` in Directory.Packages.props and
point the five WASM-only packages at it; use `VersionOverride="8.0.2"` in BlazorAdmin's net8.0 slice.

### Multi-Target Split in BlazorAdmin.csproj
BlazorAdmin needs `TargetFrameworks=net8.0;net10.0` because:
- Web (net8.0, the WASM host) requires a compatible project reference (NuGet NU1201 otherwise)
- net10.0 is the actual WASM output that runs in the browser

Two conditional `<ItemGroup>` blocks handle the version split:
- `Condition="'$(TargetFramework)' == 'net8.0'"` → VersionOverride="8.0.2" for all net10.0-only packages
- `Condition="'$(TargetFramework)' == 'net10.0'"` → uses BlazorWasmVersion from Directory.Packages.props

### System.Net.Http.Json Removed from BlazorAdmin
NU1510 warning: package is redundant because System.Net.Http.Json is built into .NET 10 (and into the
.NET 8 WASM BCL). Removed from `<PackageReference>` list. No code change needed — the `using` directive
and `ReadFromJsonAsync` API are available from the framework itself.

### API Incidents (3 × Api.0003 Behavioral Changes)
- 2 × `T:System.Net.Http.HttpContent` in HttpService.cs: "Streaming HTTP responses enabled by default
  in browser HTTP clients." Fixed by replacing `ReadAsStringAsync() + JsonSerializer.Deserialize<>()`
  with `ReadFromJsonAsync<>()` (stream-native, the canonical .NET 10 WASM HTTP pattern).
- 1 × `T:System.Uri` in Program.cs: "URI length limits removed." Informational only — no code change
  required; URI construction is unaffected.

### BlazorShared Multi-Target
BlazorShared (0 package issues, 0 API issues) multi-targets `net8.0;net10.0`:
- net8.0 slice: consumed by ApplicationCore (net8.0), Web (net8.0), BlazorAdmin net8.0 slice
- net10.0 slice: consumed by BlazorAdmin net10.0 slice (matching TFM for clean resolution)
FluentValidation 11.9.0 and BlazorInputFile 0.2.0 are ✅ Compatible per assessment.

## Affected Files
- `Directory.Build.props` (new): default TargetFramework=net8.0
- `Directory.Packages.props`: removed TargetFramework, added BlazorWasmVersion, updated 5 package entries
- `src/BlazorAdmin/BlazorAdmin.csproj`: multi-target, conditional package split, removed System.Net.Http.Json
- `src/BlazorShared/BlazorShared.csproj`: multi-target net8.0;net10.0
- `src/BlazorAdmin/Services/HttpService.cs`: ReadFromJsonAsync replaces ReadAsStringAsync+Deserialize
