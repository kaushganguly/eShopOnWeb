# 03-upgrade-web-apps: Upgrade Web, PublicApi, and BlazorAdmin to net10.0

## Objective

Move `Web`, `PublicApi`, and `BlazorAdmin` to `net10.0`, resolve their package issues (including
the Azure.Identity security vulnerability), replace the deprecated AutoMapper DI package, and fix
the source-breaking Swashbuckle/Microsoft.OpenApi v2 API changes flagged by the assessment.

## Research (via query_dotnet_assessment)

### Web.csproj (28 issues)
- `Project.0002` — TFM net8.0 → net10.0. Resolved automatically (central TFM in
  `Directory.Packages.props`, bumped in task 01).
- `NuGet.0002` (×8) — ASP.NET Core / EF Core packages pinned at 8.0.2 via `$(AspNetVersion)` /
  `$(EntityFramworkCoreVersion)` alias properties. Resolved automatically by task 01's alias bump.
- `NuGet.0002` — `Microsoft.VisualStudio.Web.CodeGeneration.Design 8.0.0` → resolved automatically
  via `$(VSCodeGeneratorVersion)` alias (task 01, → `10.0.2`).
- `NuGet.0005` — `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` deprecated;
  functionality merged into `AutoMapper` itself. Action: replace `PackageReference` with plain
  `AutoMapper` (`16.2.0`). Verified: Web doesn't actually call `AddAutoMapper`/use AutoMapper types
  anywhere in code (grep confirmed no usage) — reference kept 1:1 replaced per NuGet.0005 guidance
  since removing unused refs entirely is out of scope for this upgrade.
- `NuGet.0004`/`NuGet.0005` — `Azure.Identity 1.10.4` has a **moderate security vulnerability**.
  Action: bump to `1.21.0` (the tracked CVE fix for this upgrade).
- `NuGet.0005` — `System.IdentityModel.Tokens.Jwt 7.3.1` deprecated → `8.23.0` (same version used
  in task 02 for ApplicationCore/Infrastructure; shared central `PackageVersion`).
- `Api.0001` (Mandatory, ×5) — `ConfigurationBinder.Get<T>()`, `.Configure<T>()`, `.GetValue()`
  calls in `ConfigureWebServices.cs:15`, `ConfigureCoreServices.cs:21`, `Program.cs:97,98,140`.
  Investigated: these are ordinary, source-compatible `Microsoft.Extensions.Configuration` APIs;
  the "binary incompatible" flag refers to the assembly version changing between net8/net10, not a
  source-level signature change. **Resolved automatically by recompiling against the updated
  packages** — confirmed via clean `dotnet build`, no code changes required.
- `Api.0003` (Potential/behavioral, ×9) — `HttpContent.ReadAsStringAsync()` charset-handling change
  (`HomePageHealthCheck.cs:26`, `ApiHealthCheck.cs:25`), `UseExceptionHandler(string)` behavioral
  change (`Program.cs:179`), `System.Uri` parsing behavioral changes (`Program.cs:31,101`),
  `ConsoleLoggerExtensions.AddConsole()` behavioral change (`Program.cs:22`). These are runtime
  behavioral changes, not compile breaks — no source change is required; flagged here for test-time
  awareness. Existing tests will surface any regression during task 04/05 validation.

### PublicApi.csproj (17 issues)
- `Project.0002` + `NuGet.0002` (×9, EF Core/AspNetCore aliases + `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`)
  — resolved automatically via task 01 aliases, except `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
  which isn't alias-driven: bumped `1.19.6` → `1.22.1` (latest stable on nuget.org; this is a
  VS/Docker-tooling package, not a runtime dependency).
- `NuGet.0001` — `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` flagged "no supported
  version found" by the assessment tool's internal matrix; verified directly against nuget.org that
  `1.22.1` is current stable and this package isn't TFM-gated (it's build-time VS/Docker tooling) —
  safe to bump.
- `NuGet.0005` — `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` → replaced with plain
  `AutoMapper 16.2.0` (same as Web). **Code change required**: `AutoMapper 16.2.0`'s
  `AddAutoMapper(Assembly)` overload no longer exists (confirmed via reflection against the
  installed package) — the DI extension now requires a `Action<IMapperConfigurationExpression>`
  first argument. Changed `Program.cs:85`:
  `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);` →
  `builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);`
- `NuGet.0005` — `System.IdentityModel.Tokens.Jwt 7.3.1` → `8.23.0` (shared central version).
- Swashbuckle.AspNetCore `6.5.0` → `10.2.3` (+ `SwaggerUI`/`Annotations` variants, same version).
  **This is a major breaking change** not fully enumerated by the assessment tool as compile errors
  (it only flagged the NuGet version bump) — Swashbuckle v10 depends on `Microsoft.OpenApi` v2,
  which moved all model types out of the `Microsoft.OpenApi.Models` namespace into `Microsoft.OpenApi`,
  replaced concrete `OpenApiSchema` parameters with `IOpenApiSchema` in filter interfaces, and
  removed the mutable `.Reference` property pattern in favor of dedicated `*Reference` types
  (e.g. `OpenApiSecuritySchemeReference`). Researched via Swashbuckle's official
  [v10 migration guide](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/docs/migrating-to-v10.md).
  Code changes:
  - `CustomSchemaFilters.cs`: `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`;
    `ISchemaFilter.Apply(OpenApiSchema, ...)` → `Apply(IOpenApiSchema schema, ...)`, with a
    pattern-match down-cast to the concrete `OpenApiSchema` to mutate `.Properties` (per the
    official migration example for `ISchemaFilter`). Added a null-check for `.Properties`
    (now nullable on the interface) to avoid a new `CS8602` warning.
  - `Program.cs`: `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`. Rewrote the
    `AddSecurityRequirement(...)` call from the old `OpenApiSecurityScheme { Reference = new
    OpenApiReference {...} }` pattern (types removed in v2) to the new
    `AddSecurityRequirement((document) => new OpenApiSecurityRequirement { [new
    OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>() })` `Func<OpenApiDocument,...>`
    overload, per the official Swashbuckle security-configuration docs.
- `Api.0001` (Mandatory, ×4, `Program.cs:31,32,41,42` region — `ConfigurationBinder`/`Configure<T>`/
  logging) — same as Web: resolved automatically by rebuild, no source change needed (confirmed via
  clean build after all other fixes applied).
- Security follow-up: after upgrading `Microsoft.VisualStudio.Web.CodeGeneration.Design` (transitively
  pulling `NuGet.Packaging`/`NuGet.Protocol 6.12.1`), the build produced new `NU1901` low-severity
  vulnerability warnings ([GHSA-g4vj-cjjj-v7hg](https://github.com/advisories/GHSA-g4vj-cjjj-v7hg),
  patched in `6.12.5`). Per workflow rule (fix all warnings), added explicit central
  `PackageVersion` overrides for `NuGet.Packaging`/`NuGet.Protocol` at `6.12.5` plus a direct
  top-level `PackageReference` in both `Web.csproj` and `PublicApi.csproj` to force NuGet to resolve
  the patched transitive version. Confirmed via rebuild: warnings gone.

### BlazorAdmin.csproj (11 issues)
- `Project.0002` + `NuGet.0002` (×several, `Microsoft.AspNetCore.Components.*` /
  `Microsoft.Extensions.Identity.Core` / `Microsoft.Extensions.Logging.Configuration`) — resolved
  automatically via `$(AspNetVersion)`/`$(SystemExtensionVersion)` aliases (task 01).
  `BlazorInputFile` and `Blazored.LocalStorage` have no newer available version on nuget.org
  (`0.2.0` and `4.5.0` respectively remain current/latest) — no change.
  `Blazored.LocalStorage` targets netstandard, so it isn't blocked by the TFM bump.
- `NuGet.0510`-style warning (build-time, not in assessment): `System.Net.Http.Json` — the
  assessment didn't flag this, but the first build attempt surfaced `NU1510` ("will not be pruned")
  because this type is now shipped in the shared framework for `net10.0`. Action: removed the
  explicit `PackageReference`/`PackageVersion` (same treatment as `System.Text.Json` in task 02) —
  usages of `System.Net.Http.Json` types in `CustomAuthStateProvider.cs`/`CatalogLookupDataService.cs`
  keep working via the framework reference.
- `Api.0003` (Potential/behavioral, ×3) — `HttpContent.ReadAsStringAsync()` behavioral change
  (`HttpService.cs:56,90`), `System.Uri` constructor behavioral change (`Program.cs:22`). No source
  change required; noted for test-time awareness (same rationale as Web's Api.0003 items).

## Package version resolution (via get_supported_package_version / nuget.org, target net10.0)

| Package | Old | New | Notes |
|---|---|---|---|
| AutoMapper.Extensions.Microsoft.DependencyInjection → AutoMapper | 12.0.1 | 16.2.0 | package replaced (Web + PublicApi) |
| Azure.Identity | 1.10.4 | 1.21.0 | security fix (moderate CVE) |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 | major breaking change, code fixed |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.2.3 | |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.2.3 | |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.22.1 | build-time VS/Docker tooling |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | |
| NuGet.Packaging (transitive override) | 6.12.1 | 6.12.5 | GHSA-g4vj-cjjj-v7hg fix |
| NuGet.Protocol (transitive override) | 6.12.1 | 6.12.5 | GHSA-g4vj-cjjj-v7hg fix |
| System.Net.Http.Json | 8.0.x | removed (framework-provided) | BlazorAdmin |
| Ardalis.ApiEndpoints, MinimalApi.Endpoint, Ardalis.ListStartupServices, BuildBundlerMinifier, Azure.Extensions.AspNetCore.Configuration.Secrets, Blazored.LocalStorage, BlazorInputFile | — | unchanged | already current/latest for net10.0 |

## Done when

`Web`, `PublicApi`, and `BlazorAdmin` each target `net10.0`, restore cleanly, and build with 0
errors / 0 warnings individually. ✅ Verified (`dotnet build src/Web/Web.csproj`,
`src/PublicApi/PublicApi.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj` — all clean).

Note: `dotnet build eShopOnWeb.sln` still fails at this point due to a pre-existing
`CS0433` ambiguous `Program` type collision in `PublicApiIntegrationTests` and xUnit2013 analyzer
warnings in `UnitTests`/`IntegrationTests` — these are test-project issues, in scope for task
`04-upgrade-test-projects`, not this task.
