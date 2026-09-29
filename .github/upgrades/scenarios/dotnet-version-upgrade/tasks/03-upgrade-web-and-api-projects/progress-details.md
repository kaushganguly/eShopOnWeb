# Progress: 03-upgrade-web-and-api-projects

## Summary

Verified Web, PublicApi, and BlazorAdmin compile clean against net10.0 (TFM/shared package bumps already
applied solution-wide in task 02). Found and fixed two issues that only surfaced once fully rebuilt:
an `AutoMapper` API breaking change and two NU1510 "package will not be pruned" warnings.

## Files changed

- `Directory.Packages.props`
  - Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.1, deprecated, and pulling in
    vulnerable `AutoMapper` 12.0.1 — NU1903 **high severity** DoS advisory GHSA-rvv3-g6hj-g44x) with the
    `AutoMapper` package directly at 16.2.0 (the DI extension methods were merged into the main package;
    matches the assessment's NuGet.0005 recommendation).
  - Removed the `System.Net.Http.Json` PackageVersion entry (framework-provided in net10, was only used
    by BlazorAdmin and triggered NU1510).
- `src/Web/Web.csproj` — `PackageReference` swapped from `AutoMapper.Extensions.Microsoft.DependencyInjection`
  to `AutoMapper` (was present but unused in this project — no `.cs` file references AutoMapper types here;
  kept parity with the dependency swap rather than removing outright, since removal wasn't in assessment scope).
- `src/PublicApi/PublicApi.csproj` — same `PackageReference` swap (this project actively uses AutoMapper
  via `MappingProfile` and `AddAutoMapper`).
- `src/PublicApi/Program.cs` — `AddAutoMapper(typeof(MappingProfile).Assembly)` no longer compiles under
  AutoMapper 16.x: the single-`Assembly`-argument overload was removed; all overloads now require an
  explicit `Action<IMapperConfigurationExpression>` configuration delegate. Confirmed available overloads
  via a scratch reflection probe against the real `AutoMapper.dll` (16.2.0). Fixed by changing to
  `builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);` (no-op config delegate +
  assembly scan, functionally identical to the old call).
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed `<PackageReference Include="System.Net.Http.Json" />`
  (NU1510: framework-provided in net10; `System.Net.Http.Json` namespace usages in
  `CustomAuthStateProvider.cs`, `Services/CatalogLookupDataService.cs`, `_Imports.razor` remain valid
  without an explicit package reference).

## Assessment findings reviewed (no source changes required)

- Api.0001 "binary incompatible" findings for `ConfigurationBinder.Get<T>()`,
  `OptionsConfigurationServiceCollectionExtensions.Configure<T>()`, and `ConfigurationBinder.GetValue(Type, string)`
  in `Web/Configuration/ConfigureCoreServices.cs`, `Web/Configuration/ConfigureWebServices.cs`, `Web/Program.cs`,
  and `PublicApi/Program.cs` — these describe binary-only breaks (pre-built DLL calling an old overload);
  since the whole solution is recompiled from source against net10.0, the compiler resolves the current
  overloads with no code changes needed. Confirmed: builds succeed with 0 errors.
- Api.0003 behavioral-change findings (`HttpContent.ReadAsStringAsync()` in health checks and
  `BlazorAdmin/Services/HttpService.cs`, `UseExceptionHandler(string)`, `ConsoleLoggerExtensions.AddConsole()`,
  `new Uri(...)` in `BlazorAdmin/Program.cs`) — all remain valid APIs in net10.0; existing usage patterns
  are already correct (full-string reads, well-formed absolute base-address URIs). No code changes needed.
- NuGet.0001 (mandatory): `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — already resolved in
  task 02 (1.19.6 → 1.23.0).
- NuGet.0004/0005 (Azure.Identity vulnerability, System.IdentityModel.Tokens.Jwt deprecated) — already
  resolved in task 02.

## New finding discovered during build (not in original assessment)

- `dotnet build` surfaced NU1901 (low severity) advisories for **NuGet.Packaging 6.12.1** and
  **NuGet.Protocol 6.12.1** (GHSA-g4vj-cjjj-v7hg), transitive dependencies of
  `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2. Verified 10.0.2 is the latest stable version
  available on NuGet.org (no newer release fixes this transitive dependency yet). This package is a
  design-time-only scaffolding tool (`dotnet aspnet-codegenerator`), not part of the deployed runtime
  output. **No action taken** — no fix is currently available upstream; documented here as a known,
  unavoidable residual low-severity finding for final validation/summary reporting.

## Validation

- `dotnet build src/Web/Web.csproj` → Build succeeded, 0 errors, 2 unavoidable NU1901 warnings (see above).
- `dotnet build src/PublicApi/PublicApi.csproj` → Build succeeded, 0 errors, same 2 NU1901 warnings.
- `dotnet build src/BlazorAdmin/BlazorAdmin.csproj` → Build succeeded, 0 warnings, 0 errors.

## Deferred to later tasks

- Test project package upgrades + full test run → task 04.
- Full solution build + `dotnet test` + final vulnerability re-check → task 05.
