# 04-upgrade-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests to net10.0

## Objective

Move all four test projects to `net10.0`, resolve their package issues (xunit / MSTest / coverlet /
Microsoft.NET.Test.Sdk / NSubstitute), and fix a real compile-time and two real runtime blockers
that surfaced only once the whole solution + tests were built and executed together on .NET 10.

## Research (via query_dotnet_assessment)

### UnitTests.csproj (3 issues)
- `Project.0002` — TFM net8.0 → net10.0. Resolved automatically (central TFM bump in task 01).
- `NuGet.0002` — `Microsoft.EntityFrameworkCore.InMemory` pinned via `$(EntityFramworkCoreVersion)`
  alias. Resolved automatically by task 01's alias bump.
- `NuGet.0005` — `xunit`/`xunit.runner.console` 2.7.0 deprecated. Assessment's own recommendation
  is to stay on xunit v2 (not migrate to v3, which is a bigger breaking change out of scope for a
  pure framework-version upgrade). Action: bump `xunit`/`xunit.runner.console` to `2.9.3`.

### IntegrationTests.csproj (3 issues)
- `Project.0002` — TFM bump, automatic.
- `NuGet.0002` — `Microsoft.EntityFrameworkCore.InMemory`, alias-driven, automatic.
- `NuGet.0005` — `xunit` 2.7.0 → `2.9.3` (same as UnitTests).

### FunctionalTests.csproj (33 issues, 20 shown by assessment)
- `Project.0002` — TFM bump, automatic.
- `NuGet.0002` (×2) — `Microsoft.AspNetCore.Mvc.Testing` / `Microsoft.EntityFrameworkCore.InMemory`,
  alias-driven, automatic.
- `NuGet.0005` — `xunit` 2.7.0 → `2.9.3`.
- `Api.0003` (~29 issues, Potential/behavioral) — `HttpContent.ReadAsStringAsync()` charset-handling
  changes and `System.Uri` parsing changes across `Web/Pages/HomePageOnGet.cs`,
  `Web/Pages/Basket/IndexTest.cs`, `Web/Pages/Basket/CheckoutTest.cs`,
  `Web/Pages/Basket/BasketPageCheckout.cs`. Investigated: these are behavioral-change advisories,
  not compile breaks; no code change required. Confirmed empirically — all 12 FunctionalTests pass
  unmodified after the TFM/package bumps.

### PublicApiIntegrationTests.csproj (11 issues)
- `Project.0002` — TFM bump, automatic.
- `NuGet.0002` — `Microsoft.AspNetCore.Mvc.Testing`, alias-driven, automatic.
- `NuGet.0005` (×2) — `MSTest.TestAdapter`/`MSTest.TestFramework` 3.2.2 deprecated → bumped to
  `4.4.1` (latest stable, matches assessment recommendation).
- `Api.0003` (×7, Potential/behavioral) — `HttpContent.ReadAsStringAsync()` charset issues. Same
  category as FunctionalTests; confirmed no code change needed once tests actually ran.

## Package version resolution (via get_supported_package_version, verified per-project-path)

| Package | Old | New | Notes |
|---|---|---|---|
| `Microsoft.NET.Test.Sdk` | 17.9.0 | 18.10.1 | Verified against nuget.org flat-container index |
| `xunit` | 2.7.0 | 2.9.3 | Staying on v2 per assessment guidance |
| `xunit.runner.visualstudio` | 2.5.6 | 4.0.0 | Verified latest stable (not a `-pre` prerelease) |
| `xunit.runner.console` | 2.7.0 | 2.9.3 | |
| `NSubstitute` | 5.1.0 | 6.2.0 | Verified latest stable |
| `NSubstitute.Analyzers.CSharp` | 1.0.17 | 1.0.17 | Already latest |
| `MSTest.TestAdapter` | 3.2.2 | 4.4.1 | |
| `MSTest.TestFramework` | 3.2.2 | 4.4.1 | |
| `coverlet.collector` | 6.0.2 | 10.0.1 | Verified latest stable |

All versions applied centrally in `Directory.Packages.props` (test projects consume via bare
`PackageReference` + CPM, no per-project version overrides needed).

## Real blockers discovered only when building/running the full solution + tests together

These three issues were **not** flagged by the assessment tool (which analyzes projects
independently) — they only surface when the whole dependency graph is built and executed, which is
exactly why task 05 (final validation) exists. Investigated and fixed here since they block
`PublicApiIntegrationTests` entirely.

### 1. CS0433 — ambiguous `Program` type (compile-time)

`PublicApiIntegrationTests.csproj` has `ProjectReference`s to **both** `PublicApi.csproj` (for the
API host under test, via `WebApplicationFactory<Program>`) and `Web.csproj` (needed only for the
`Microsoft.eShopWeb.Web.ViewModels` types used to deserialize API responses in
`CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`). Both assemblies contain an implicit
top-level-statement `Program` class in the global namespace; on .NET 10 both resolve as `public`,
so `WebApplicationFactory<Program>` in `ProgramTest.cs` became ambiguous
(`error CS0433: The type 'Program' exists in both 'PublicApi...' and 'Web...'`). This dual
`ProjectReference` setup pre-dates this upgrade (present since the "Feature/dotnet7 migration"
commit) — the ambiguity was latent and only surfaced now.

**Fix**: aliased the `PublicApi` `ProjectReference` (`<Aliases>global,PublicApiAlias</Aliases>` —
keeping `global` so every other file's normal unqualified access to `Microsoft.eShopWeb.PublicApi.*`
types keeps working) and updated only `ProgramTest.cs` (the sole file that references the bare
`Program` symbol) to add `extern alias PublicApiAlias;` and use
`WebApplicationFactory<PublicApiAlias::Program>`. No other file in the project references `Program`
directly, so this is a minimal, fully-scoped fix.

### 2. TypeLoadException — Blazor WebAssembly assemblies loaded into the test host (runtime)

Once the compile error was fixed, all 15 `PublicApiIntegrationTests` failed at runtime with
`ReflectionTypeLoadException` / `TypeLoadException: ... contains an object field at offset 4 that
is incorrectly aligned or overlapped by a non-object field`, first for `BlazorInputFile.ReadRequest`
and then (after that package was removed, see item 3) for `WebAssembly.JSInterop.JSCallInfo` in
`Microsoft.JSInterop.WebAssembly`.

Root cause: `PublicApi`'s `Program.cs` calls `builder.Services.AddEndpoints()` from the
`MinimalApi.Endpoint` NuGet package (1.3.0), which reflection-scans **every assembly file present in
the test host's base directory** (`MinimalApi.Endpoint.Extensions.IServiceCollectionExtensions.
AddEndpoints`) and calls `.GetTypes()` on each. Because `PublicApiIntegrationTests.csproj`
references `Web.csproj`, and `Web.csproj` itself references the `BlazorAdmin` Blazor WebAssembly
client project, MSBuild copies `BlazorAdmin.dll` and its entire transitive WASM-runtime closure
(`Microsoft.AspNetCore.Components.WebAssembly*.dll`, `Microsoft.JSInterop.WebAssembly.dll`,
`Blazored.LocalStorage.dll`) into the test project's output folder. Those assemblies are compiled
for the mono/wasm runtime; loading them via reflection on desktop CoreCLR has always been fragile,
and .NET 10's stricter struct-layout validation turns that fragility into a hard `TypeLoadException`
that aborts the entire `GetTypes()` scan — which is why *every* test failed (the API host itself
never starts). Confirmed via `Type.IsPublic`/package inspection that `MinimalApi.Endpoint` 1.3.0
exposes only the parameterless `AddEndpoints(IServiceCollection)` overload — there is no
assembly-list filter we can pass in.

None of this is new to the .NET 10 upgrade in terms of *architecture* (Web has referenced
BlazorAdmin, and PublicApiIntegrationTests has referenced Web, since well before this upgrade) — but
either the previous .NET 8 runtime tolerated the WASM assemblies' layout during reflection, or the
combination of package version bumps in task 03 changed exactly which WASM assembly got far enough
to be probed. Either way, `PublicApi.Program.cs` (the only code path actually executed by
`WebApplicationFactory`) never uses `BlazorAdmin`, `Blazored.LocalStorage`, or any
`*.WebAssembly*` assembly — confirmed via `using` inspection — so removing them from the test
project's own build output is safe and behavior-preserving.

**Fix**: added an MSBuild target to `PublicApiIntegrationTests.csproj` (runs `AfterTargets=
"ResolveReferences;ResolveProjectReferences"`) that strips `ReferenceCopyLocalPaths` entries whose
filename contains `WebAssembly`, equals `BlazorAdmin`, or starts with `Blazored.` from the test
project's copy-to-output set. `BlazorShared.dll` is deliberately **kept** — `PublicApi.Program.cs`
itself has `using BlazorShared;` and calls `AddConfigurationFile(...)`, a `BlazorShared` extension
method, so it's a genuine runtime dependency (confirmed by re-adding the exclusion and seeing
`FileNotFoundException: BlazorShared` before narrowing the exclusion list).

### 3. BlazorInputFile — dead, unmaintained package (secondary contributor to #2)

`BlazorInputFile` (last published version `0.2.0`, pre-Blazor-GA era, superseded by the built-in
`<InputFile>` component since .NET 5) was referenced by `BlazorAdmin.csproj` and
`BlazorShared.csproj`, and its `ReadRequest` type was the *first* assembly to trip the
`TypeLoadException` above. Confirmed via grep that its only consumer,
`BlazorShared.Models.CatalogItem.DataToBase64(IFileListEntry)`, has zero callers anywhere in the
solution (dead code), and its only other reference is an unused `@using BlazorInputFile` in
`BlazorAdmin/_Imports.razor`. Removed the package reference from both `.csproj` files, the
`PackageVersion` from `Directory.Packages.props`, the dead `DataToBase64` method + its `using
BlazorInputFile;`/`using System.Threading.Tasks;` (both now unused) from `CatalogItem.cs`, and the
unused `@using BlazorInputFile` from `_Imports.razor`. This is a genuine, safe modernization
cleanup (dead + abandoned + runtime-incompatible package), not just a side-effect workaround.

## Warnings fixed (workflow rule 12 — fix all warnings in touched projects)

- `xUnit2013` (×3) — `Assert.Equal(1, x.Count)` / `Assert.Equal(0, x.Count)` → `Assert.Single(x)` /
  `Assert.Empty(x)` in `UnitTests/.../CustomerOrdersWithItemsSpecification.cs` (×2),
  `UnitTests/.../BasketRemoveEmptyItems.cs` (×1), `IntegrationTests/.../SetQuantities.cs` (×1).
- `MSTEST0044` — `[DataTestMethod]` obsolete in MSTest 4.x → replaced with `[TestMethod]` in
  `PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`
  (`[DataRow]` works directly with `[TestMethod]` in current MSTest).

## Validation

- `dotnet build eShopOnWeb.sln` → **0 Warning(s), 0 Error(s)** (all 11 projects).
- `dotnet test eShopOnWeb.sln --no-build` → **44 UnitTests + 3 IntegrationTests + 12
  FunctionalTests + 15 PublicApiIntegrationTests = 74/74 passed**, 0 failed, 0 skipped.
- `dotnet list eShopOnWeb.sln package --vulnerable --include-transitive` (+ individually for
  PublicApi/BlazorAdmin/BlazorShared/PublicApiIntegrationTests, which the solution-level command
  silently omitted) → **no vulnerable packages** in any of the 10 projects.
