# Task 03 — Final Validation — Progress Details

## Goal
Run the full test suite across all four test projects and confirm the security-compliance
success criterion (`securityComplianceCheck=true`) holds for the upgraded (net10.0) solution.

## Summary of outcome
- Solution builds clean: **0 errors, 0 warnings** (verified from a fully clean `bin`/`obj` state).
- All 4 test projects pass in full: **UnitTests 44/44, IntegrationTests 3/3, FunctionalTests 12/12,
  PublicApiIntegrationTests 15/15** — 74/74 tests passing overall.
- `dotnet list package --vulnerable --include-transitive` returns **zero vulnerable packages** for
  all 10 projects in the solution (checked individually, since the solution-level command only
  surfaced 6 of 10 projects in its summary — each of the other 4 was checked directly and confirmed
  clean too).

Two genuine functional regressions were discovered only via test execution (not via build or
restore) and fixed. Both were **predicted as risks** in `assessment.md` before any test was run.

---

## Issue 1 — BlazorInputFile incompatible with .NET 10 (abandoned package)

### Symptom
`PublicApiIntegrationTests` (all 15 tests) failed with:
```
System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
Could not load type 'ReadRequest' from assembly 'BlazorInputFile, Version=0.20.0.0...'
...object field at offset ... is incorrectly aligned or overlapped by a non-object field.
```

### Root cause
`BlazorInputFile` is an **abandoned, pre-release-era (v0.20.0) third-party package**, last published
years ago, never updated for modern .NET. `assessment.md` had already flagged it as a "functional risk
requiring runtime verification" because no replacement/fix exists and its binary layout assumptions
predate CoreCLR's current (stricter, .NET 9+) struct-layout validation rules. On .NET 10, the CLR's
type loader rejects the type outright.

### Investigation
Searched for all usages of the `BlazorInputFile` namespace/package across the solution:
- `src/BlazorAdmin/_Imports.razor` — a single `@using BlazorInputFile` statement.
- `src/BlazorShared/Models/CatalogItem.cs` — a single method, `DataToBase64(IFileListEntry file)`.

Confirmed via repo-wide search that `DataToBase64` had **zero callers** anywhere in the solution
(production code, Razor components, or tests) — it was dead code.

### Fix
- Removed the `@using BlazorInputFile;` directive from `src/BlazorAdmin/_Imports.razor`.
- Removed the dead `DataToBase64(IFileListEntry)` method and its `using BlazorInputFile;` /
  now-unused `using System.Threading.Tasks;` from `src/BlazorShared/Models/CatalogItem.cs`.
- Removed the `BlazorInputFile` `PackageReference` from `src/BlazorAdmin/BlazorAdmin.csproj` and
  `src/BlazorShared/BlazorShared.csproj` (it was referenced directly by both — discovered the second
  reference via a follow-up restore failure after removing only the first).
  Also removed the central `PackageVersion` entry from `Directory.Packages.props`.

### Why this is safe
No functionality is lost — the only caller-free, dead-code usage was removed. File-upload
functionality in BlazorAdmin (if any is actually exercised elsewhere) does not go through this type;
grep confirmed no other file-input code path in the admin UI depends on `BlazorInputFile`.

---

## Issue 2 — Microsoft.JSInterop.WebAssembly struct-layout incompatibility under CoreCLR on .NET 10

### Symptom
After fixing Issue 1, `PublicApiIntegrationTests` (all 15 tests) still failed, now with a *different*
root cause:
```
System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
Could not load file or assembly 'Microsoft.JSInterop.WebAssembly, Version=10.0.12.0...'
...object field at offset ... is incorrectly aligned or overlapped by a non-object field.
  at MinimalApi.Endpoint.Extensions.IServiceCollectionExtensions.AddEndpoints(IServiceCollection services)
```

### Confirming this is a genuine net10-only regression
Stashed all working-tree changes and ran the identical test against the original, unmodified
net8.0 baseline: **passed cleanly (15/15)**. Restored the stashed changes afterwards. This
conclusively rules out a pre-existing bug and confirms the failure is specific to running under
.NET 10's CoreCLR.

### Root cause (confirmed via decompilation + elimination testing)
1. `MinimalApi.Endpoint` 1.3.0 (the latest version on nuget.org — no newer release exists) implements
   `AddEndpoints()` as `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())`, with
   **no assembly filtering whatsoever** and no alternate overload to scope the scan (confirmed via
   `ilspycmd` decompilation of the package DLL). Any single assembly in the process's `AppDomain` that
   throws during `GetTypes()` kills this call — and therefore the whole PublicApi host boot — for
   every test.
2. `PublicApiIntegrationTests.csproj` legitimately needs a `ProjectReference` to `Web.csproj` (for
   shared `Microsoft.eShopWeb.Web.ViewModels` types such as `CatalogItemViewModel`, used by
   `CatalogItemListPagedEndpoint.cs`). `Web.csproj`, in turn, has a direct, legitimate
   `PackageReference` to `Microsoft.AspNetCore.Components.WebAssembly.Server` (required for
   `UseBlazorFrameworkFiles()`, needed to serve the actual hosted Blazor WASM admin app in
   production) **and** a `ProjectReference` to `BlazorAdmin.csproj`. Both chains transitively pull
   `Microsoft.JSInterop.WebAssembly.dll` (and `Microsoft.AspNetCore.Components.WebAssembly.dll`,
   `Microsoft.AspNetCore.Components.WebAssembly.Server.dll`, `Microsoft.AspNetCore.Components.WebAssembly.Authentication.dll`,
   `BlazorAdmin.dll`) into **this test project's own build output directory** — even though none of
   this WASM-hosting machinery is ever exercised by these tests (the test host only boots PublicApi's
   minimal-API `Program`, never `Web`'s).
3. `Microsoft.JSInterop.WebAssembly.dll` is built to run under the **Mono/WebAssembly runtime**, not
   CoreCLR. On .NET 8, CoreCLR's type loader tolerated reflecting over it anyway (confirmed: the same
   physical DLL is present in `Web`'s own net8.0 and net10.0 bin outputs alike — this is not a new
   assembly, only the loader behavior toward it changed). On .NET 10, CoreCLR's tightened
   struct-layout validation throws `TypeLoadException`/`ReflectionTypeLoadException` for it.
4. Confirmed by elimination: deleting only `Microsoft.JSInterop.WebAssembly.dll` from the test output
   still failed (now as `FileNotFoundException` for that same assembly, proving something else in the
   AppDomain — `Web.dll`'s own reference to `Microsoft.AspNetCore.Components.WebAssembly.Server` —
   still tries to resolve it during `GetTypes()`). Deleting the **full WASM-only assembly set**
   (`BlazorAdmin.dll`, `Microsoft.AspNetCore.Components.WebAssembly.dll`,
   `Microsoft.AspNetCore.Components.WebAssembly.Server.dll`,
   `Microsoft.AspNetCore.Components.WebAssembly.Authentication.dll`,
   `Microsoft.JSInterop.WebAssembly.dll`) from the test project's own output directory made all 15
   tests pass.

### Rejected alternative fix
Considered adding `ReferenceOutputAssembly="false"` to `Web.csproj`'s `ProjectReference` to
`BlazorAdmin.csproj` (reasoning: Web only needs BlazorAdmin's static web assets, not its compiled
types). This was **reverted** after discovery that `src/Web/Program.cs` genuinely registers
`BlazorAdmin.Services.ToastService` and `BlazorAdmin.Services.HttpService` in its own DI container
(`using BlazorAdmin.Services;` + `builder.Services.AddScoped<ToastService>(); ... AddScoped<HttpService>();`)
— a real compile-time dependency, not an accidental one. Removing the assembly reference broke
`Web.csproj`'s own build (CS0246). This confirms the fix must be scoped to the **test project's own
output only**, not to `Web.csproj`'s architecture.

### Fix
Added an MSBuild post-build `Target` to `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
that deletes the WASM-only assemblies from the test project's own output directory immediately after
`Build`/`Publish`:
```xml
<Target Name="RemoveWasmOnlyAssembliesFromTestOutput" AfterTargets="Build;Publish">
  <ItemGroup>
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)BlazorAdmin.dll" />
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)BlazorAdmin.pdb" />
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)Microsoft.AspNetCore.Components.WebAssembly.dll" />
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)Microsoft.AspNetCore.Components.WebAssembly.Server.dll" />
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)Microsoft.AspNetCore.Components.WebAssembly.Authentication.dll" />
    <_WasmOnlyTestOutputFilesToRemove Include="$(OutDir)Microsoft.JSInterop.WebAssembly.dll" />
  </ItemGroup>
  <Delete Files="@(_WasmOnlyTestOutputFilesToRemove)" ContinueOnError="true" />
</Target>
```
A detailed rationale comment (explaining all of the above) is included directly above the target in
the `.csproj` file for future maintainers.

### Why this is safe
- `src/Web/bin/...` (Web's own build output) is completely untouched — Web's actual hosted-Blazor-WASM
  functionality in production is unaffected.
- No test code in `PublicApiIntegrationTests` references `BlazorAdmin`, `Blazored.LocalStorage`, or any
  `Microsoft.AspNetCore.Components.WebAssembly*` type — confirmed via grep.
- The test host (`WebApplicationFactory<ProgramEntryPoint>`, aliased to PublicApi's `Program`) never
  calls `UseBlazorFrameworkFiles()` or anything else requiring these assemblies at runtime.
- Verified end-to-end from a fully clean `bin`/`obj` state: the target runs automatically on every
  build with no manual steps required, and all 15 tests pass consistently.

---

## Verification performed
1. Deleted all `bin`/`obj` folders solution-wide; ran `dotnet build eShopOnWeb.sln -c Release` →
   **0 Warning(s), 0 Error(s)**.
2. Ran `dotnet test eShopOnWeb.sln -c Release --no-build --no-restore` →
   **UnitTests 44/44, IntegrationTests 3/3, FunctionalTests 12/12, PublicApiIntegrationTests 15/15**
   (74/74 total).
3. Ran `dotnet list package --vulnerable --include-transitive` against all 10 projects individually
   (the solution-level command only echoed 6 of 10 in its final summary, so each of the remaining 4 —
   PublicApi, BlazorAdmin, BlazorShared, PublicApiIntegrationTests — was checked directly) →
   **zero vulnerable packages found in every project.**

## Success criteria status
- `passBuild=true` — ✅ confirmed (0 errors, 0 warnings, clean rebuild from scratch).
- `generateNewUnitTests=false` — ✅ honored (no new test files were authored; only existing tests
  were fixed for xUnit/MSTest analyzer warnings and ambiguity, per the "fix all warnings" rule).
- `passUnitTests=true` — ✅ confirmed (74/74 tests passing across all 4 test projects).
- `securityComplianceCheck=true` — ✅ confirmed (zero vulnerable packages, direct or transitive,
  across all 10 projects).
