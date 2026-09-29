# 03-upgrade-web-and-api-projects: Upgrade Web, PublicApi, BlazorAdmin to net10.0

## Scope

Projects: `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj`.

TFM and shared package versions were already bumped solution-wide in task 02 (via
`Directory.Packages.props`, since `TargetFramework` and the ASP.NET/EF Core/test package versions are
centrally defined there). This task focuses on: (a) verifying those projects compile clean against
net10.0, (b) the one incompatible package (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`,
already bumped to 1.23.0 in task 02), (c) the flagged Azure.Identity vulnerability (already bumped to
1.21.0 in task 02), and (d) reviewing/resolving the Api.0001/Api.0003 findings below.

## Assessment findings (query_dotnet_assessment)

- **Web.csproj** (28 issues, 20 returned) — package upgrades already covered by shared props (task 02).
  Remaining code-level findings:
  - Api.0003 (behavioral change) x2: `HttpContent.ReadAsStringAsync()` in `HealthChecks/HomePageHealthCheck.cs:26`
    and `HealthChecks/ApiHealthCheck.cs:25` — behavioral changes to HttpContent buffering; no action needed
    unless a build/test failure surfaces an actual regression (health check reads full body as string
    either way).
  - Api.0001 (binary incompatible, mandatory) x3: `ConfigurationBinder.Get<T>()` (ConfigureCoreServices.cs:21),
    `OptionsConfigurationServiceCollectionExtensions.Configure<T>()` (ConfigureWebServices.cs:16),
    `ConfigurationBinder.GetValue(Type, string)` (Program.cs:140). These flags describe a scenario where a
    *pre-built* binary calling these APIs would need the old assembly version; since we recompile the
    entire solution from source against net10.0, the compiler re-resolves the correct current overloads —
    no source changes needed. Will confirm via build (no CS errors expected).
  - Api.0003: `UseExceptionHandler(string)` (Program.cs:179) — still valid in net10, no action needed.
- **PublicApi.csproj** (17 issues) — same shared package-driven upgrades apply.
  - NuGet.0001 (mandatory, now resolved in task 02): `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
    bumped 1.19.6 → 1.23.0.
  - Api.0001 x4: `ConfigurationBinder.Get<T>()` / `Configure<T>()` calls in `Program.cs` (lines 41-49) — same
    reasoning as Web.csproj; recompiling from source resolves current overloads, no source change expected.
  - Api.0003: `ConsoleLoggerExtensions.AddConsole()` (Program.cs:31) — still valid, no action needed.
- **BlazorAdmin.csproj** (11 issues) — package upgrades already covered by shared props.
  - Api.0003 x2: `HttpContent.ReadAsStringAsync()` in `Services/HttpService.cs` (lines 56, 90) — behavioral
    change only, code already awaits and reads full string; no action needed.
  - Api.0003: `new Uri(builder.HostEnvironment.BaseAddress)` (Program.cs:22) — `System.Uri` behavioral change
    (stricter parsing in modern .NET); BaseAddress from Blazor WASM host environment is always a
    well-formed absolute URI, so no action expected. Will verify at runtime is out of scope (no WASM host
    available in this environment) — confirmed via build only.

## Plan

1. No additional `Directory.Packages.props` changes needed (already done in task 02).
2. Build Web, PublicApi, BlazorAdmin individually; fix any actual compiler errors/warnings that surface
   (the Api.0001/0003 findings above are expected to be no-ops on rebuild, but must be confirmed).
3. Record actual outcomes (errors fixed, or confirmed no-op) in progress-details.md.
