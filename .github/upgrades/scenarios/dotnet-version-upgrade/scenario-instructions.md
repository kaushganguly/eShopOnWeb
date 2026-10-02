# Scenario Instructions: .NET Version Upgrade

## Scope
- Solution: `eShopOnWeb.sln` / `Everything.sln`
- Projects: Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, and all test projects
- Source framework: net8.0
- Target framework: net10.0
- Central Package Management via `Directory.Packages.props`

## Preferences

### Flow Mode
**Automatic** — do not pause for user review at any stage. Surface assessment/plan/progress inline and proceed automatically. Only pause on hard blockers (missing info, irreconcilable ambiguity, destructive/irreversible decisions).

### Technical Preferences
- Target framework: net10.0 for all projects (including test and Blazor WASM projects)
- Update NuGet packages (via `Directory.Packages.props`) to versions compatible with .NET 10
- Resolve breaking changes in ASP.NET Core, EF Core, and Blazor WebAssembly between .NET 8 and .NET 10
- Update pinned SDK versions in `global.json` and CI configuration as needed
- No new unit tests should be generated (generateNewUnitTests=false) — only fix/update existing tests as needed for compatibility
- Security compliance check required (no known vulnerable packages after upgrade)

### Success Criteria
- passBuild=true
- generateNewUnitTests=false
- passUnitTests=true
- securityComplianceCheck=true

## Source Control
- Git repo: yes
- Working branch: `upgrade/dotnet10` (created from `main`, was clean at start)
- Commit strategy: commit at logical checkpoints on the working branch

## Decisions
- Proceeding end-to-end without pausing for review per user's explicit "Automatic flow mode" instruction, since all required parameters (target framework, scope) were provided in the initial request.
- MediatR: pinned to 12.5.0 (latest version before the project introduced a commercial licensing requirement in v13+) instead of the newest available 14.2.0, to avoid adding a licensing dependency as a side effect of the framework upgrade. All other packages upgraded to latest net10-compatible stable versions.
- AutoMapper: replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` (stuck at 12.0.1, transitively pulling vulnerable AutoMapper 12.0.1 — CVE-2026-32933 / GHSA-rvv3-g6hj-g44x, high severity DoS) with a direct reference to the main `AutoMapper` package at 16.2.0 (patched; fix is in 15.1.1+). The DI extension package's functionality (`AddAutoMapper`) is now built into the main package. Unlike MediatR, AutoMapper has no viable pre-license-change version that also fixes this high-severity vulnerability, so the newer (licensed) version was used to satisfy the security compliance requirement. Updated call sites: `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` → `builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` (new required overload signature) in `src/PublicApi/Program.cs`. Removed the now-unused `AutoMapper.Extensions.Microsoft.DependencyInjection` reference from `src/Web/Web.csproj` (AutoMapper was not actually used in the Web project).
- Removed `System.Security.Claims` PackageReference from ApplicationCore — functionality is now included via framework reference on net10.0 (assessment rule NuGet.0003).
- Removed unused orphan `Microsoft.AspNetCore.Mvc` PackageVersion entry from Directory.Packages.props — not referenced by any project (superseded by framework-provided MVC on modern ASP.NET Core; only `Microsoft.AspNetCore.Mvc.Testing` is used, which is upgraded normally via `$(AspNetVersion)`).
- Removed legacy binary-serialization constructor (`SerializationInfo`/`StreamingContext`) from `EmptyBasketOnCheckoutException` — obsolete pattern flagged by assessment (Api.0002), unused, and inconsistent with the project's other exception types which don't implement it.
- Removed redundant `System.Net.Http.Json` (BlazorAdmin) and `System.Text.Json` (ApplicationCore) explicit `PackageReference`/`PackageVersion` entries — flagged by NU1510 as framework-provided/prunable on net10.0.
- Pinned `NuGet.Packaging`/`NuGet.Protocol` to `7.9.0` (latest stable) via explicit direct `PackageReference` in `Web.csproj` and `PublicApi.csproj` (alongside a central `PackageVersion`), overriding the vulnerable transitive `6.12.1` versions pulled in by `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (GHSA-g4vj-cjjj-v7hg, low severity; no newer version of CodeGeneration.Design exists that resolves this transitively, so a direct version override was used).
- `Swashbuckle.AspNetCore` 10.2.3 now depends on `Microsoft.OpenApi` 2.x, which moved all model types from namespace `Microsoft.OpenApi.Models` to `Microsoft.OpenApi` and changed several APIs:
  - `ISchemaFilter.Apply` now takes `IOpenApiSchema` instead of `OpenApiSchema` — updated `CustomSchemaFilters.cs`.
  - `OpenApiSecurityScheme` no longer has a `Reference` property; security scheme references are now expressed via `OpenApiSecuritySchemeReference` constructed with a reference id and the host `OpenApiDocument`. `SwaggerGenOptions.AddSecurityRequirement` now takes a `Func<OpenApiDocument, OpenApiSecurityRequirement>` instead of a plain `OpenApiSecurityRequirement` — updated `src/PublicApi/Program.cs` accordingly.
- Removed the abandoned `BlazorInputFile` package (last published 2019, v0.2.0, no net10-compatible release) from `BlazorShared` and `BlazorAdmin`. It was only used for an orphaned `@using` in `BlazorAdmin/_Imports.razor` and a dead, never-called `CatalogItem.DataToBase64(IFileListEntry)` helper in `BlazorShared/Models/CatalogItem.cs` (both removed). Under the net10 runtime this package's `ReadRequest` struct failed CLR struct-layout validation (`ReflectionTypeLoadException`) when reflected over, breaking `PublicApiIntegrationTests`. Since the dead code had no callers, outright removal was safe and preferred over a replacement (e.g. built-in Blazor `InputFile`) which would have been speculative/unused as well.
- `Web/Program.cs`: added an explicit `internal partial class Program { }` declaration. On .NET 10 the compiler-generated top-level-statement `Program` class in `Web` became `public` by default (it was `internal` on net8.0 with identical source), causing `CS0433` ambiguity in `PublicApiIntegrationTests` (`WebApplicationFactory<Program>`) because `PublicApi`'s `Program` is also public. Restoring `Web.Program` to `internal` (its net8.0 behavior) resolves the ambiguity without any functional change to `Web`.
- `PublicApi/Program.cs`: replaced the call to `MinimalApi.Endpoint`'s `IServiceCollection.AddEndpoints()` (which scans `AppDomain.CurrentDomain.GetAssemblies()` — i.e., every assembly loaded into the process) with an explicit scan of `typeof(Program).Assembly` only. All `IEndpoint` implementations live exclusively in the `PublicApi` assembly, so behavior is unchanged in production. This was necessary because, under the `PublicApiIntegrationTests` in-process test host (which also references `Web.csproj`, pulling in Blazor WebAssembly-targeted assemblies like `Microsoft.JSInterop.WebAssembly`), the domain-wide scan attempted to reflect over WASM-targeted assemblies and hit CLR struct-layout validation errors (`ReflectionTypeLoadException`) new/stricter in .NET 10. Scoping the scan to the single assembly that actually declares endpoints avoids this entirely.


## Upgrade Options
- Strategy: **All-At-Once** — 10 projects, all currently net8.0 (modern), no .NET Framework projects, dependency graph shallow (≤3 tiers), low complexity/difficulty per assessment. Upgrading all projects simultaneously in a single pass.
- Test Coverage: Skip — assessment reports TestCoverageRecommended=False for all projects; generateNewUnitTests=false per success criteria.
- Package Management: Central Package Management already in use via `Directory.Packages.props` — retained, no conversion needed.
- Security vulnerabilities (NuGet.0004 findings in ApplicationCore and Web): included in the upgrade by default — packages will be updated to non-vulnerable versions compatible with net10.0.

