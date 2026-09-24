# 03-web-apps: Upgrade Web, PublicApi, BlazorAdmin to net10.0

## Scope
Projects: `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj`.
TargetFramework already centrally set to net10.0 via `Directory.Packages.props` (task 01). This task
resolves package-version and API-compatibility issues specific to these three projects.

## Research findings (assessment + get_supported_package_version + source review)

### Package changes required
| Package | Project(s) | Old | New | Reason |
|---|---|---|---|---|
| `AspNetVersion` (central prop) | Web, PublicApi, BlazorAdmin | 8.0.2 | 10.0.12 | Already bumped task 02 — applies here too |
| `SystemExtensionVersion` (central prop, drives `System.Net.Http.Json`, `Microsoft.Extensions.Logging.Configuration`) | BlazorAdmin | 8.0.0 | 10.0.12 | net10.0 compatibility (`get_supported_package_version`) |
| `VSCodeGeneratorVersion` (central prop, drives `Microsoft.VisualStudio.Web.CodeGeneration.Design`) | Web, PublicApi | 8.0.0 | 10.0.2 | net10.0 compatibility (`get_supported_package_version`) |
| `Azure.Identity` | Web | 1.10.4 | 1.21.0 | Assessment: moderate vulnerability + deprecated version; 1.21.0 confirmed supported for net10.0 |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | PublicApi | 1.19.6 | **removed** | Assessment NuGet.0001 Mandatory: "No supported version found" for net10.0 (confirmed via `get_supported_package_version` — no version resolves). This is a VS design-time-only package for Docker debugging tooling; runtime Dockerfiles (`src/Web/Dockerfile`, `src/PublicApi/Dockerfile`) and `docker-compose.dcproj` do not depend on it. Safe to remove. |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` (Web, unused ref) | Web | 12.0.1 | **removed** | `dotnet restore` confirmed Web has no actual `IMapper`/AutoMapper usage in code — package reference was dead. Removed entirely. |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper` | PublicApi | 12.0.1 | 16.2.0 | `dotnet restore` surfaced **NU1903 high-severity vulnerability GHSA-rvv3-g6hj-g44x / CVE-2026-32933** (uncontrolled recursion DoS) affecting AutoMapper < 15.1.1 (and 16.0.0–16.1.0). Per GitHub Advisory API, first patched versions are 15.1.1 / 16.1.1. `get_supported_package_version` resolved 16.2.0 for net10.0 (patched). Replaced the deprecated split DI package with plain `AutoMapper` (DI extensions now built into the base package per NuGet.0005 recommendation) and updated `PublicApi/Program.cs`'s `AddAutoMapper(typeof(MappingProfile).Assembly)` call to the new overload signature `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` (the old single-assembly-argument overload no longer exists in the merged package; verified via AutoMapper's `ServiceCollectionExtensions.cs` source). Note: current AutoMapper versions use a commercial license model for organizations above a revenue threshold (free for individuals/small orgs) — flagged for user awareness in final report; required here to satisfy the security-compliance success criterion. |
| `System.IdentityModel.Tokens.Jwt` | Web, PublicApi | (central, done in task 02) | 8.23.0 | Already bumped centrally; verified these projects pick it up via `$(...)`-free central `PackageVersion` |

### API/behavioral issues reviewed (assessment Api.0001/0002/0003)
All of the following were reviewed by reading the flagged source lines. None require source-code changes — they are
either (a) "Binary incompatible" issues whose stated remediation is simply recompilation against the new SDK (which
this task performs), or (b) "Behavioral change" issues (runtime-only, no compile impact) that don't affect this
codebase's usage pattern. Confirmed clean via full project builds (see Validation below).

- `Web/Configuration/ConfigureWebServices.cs:16`, `ConfigureCoreServices.cs:22` — `IConfiguration.Configure<T>()`/`Get<T>()` — Api.0001 binary incompatible; resolved by recompilation, signatures unchanged in net10.0.
- `Web/Configuration/ConfigureCookieSettings.cs:26` — `TimeSpan.FromMinutes(ValidityMinutesPeriod)` where `ValidityMinutesPeriod` is `const int` — Api.0002 source incompatible ("New TimeSpan.From*() overloads that take integers", .NET 9). New `int`/`long` overload now binds instead of `double` overload; for a whole-number constant (60) result is numerically identical. Compiles and behaves correctly — verified via build.
- `Web/HealthChecks/HomePageHealthCheck.cs:27`, `ApiHealthCheck.cs:26` — `HttpContent.ReadAsStringAsync()` — Api.0003 behavioral change (charset/encoding detection refinement). Usage here is a simple `.Contains()` substring check on ASCII/UTF8 HTML content — unaffected.
- `Web/Program.cs:179` — `UseExceptionHandler("/Error")` — Api.0003 behavioral change (re-execution pipeline nuance in ASP.NET Core 9+). No custom error-handling logic depends on old behavior; standard usage, no change needed.
- `PublicApi/Program.cs:42,43,49,50` — same `Configure<T>`/`Get<T>` pattern as Web — resolved by recompilation.
- `PublicApi/Program.cs:32` — `builder.Logging.AddConsole()` — Api.0003 behavioral change (console formatter defaults) — no custom console formatting configured, unaffected.
- `BlazorAdmin/Services/HttpService.cs:57,91` — `JsonSerializer.Deserialize` + `HttpContent.ReadAsStringAsync` — Api.0003 behavioral change, same charset-detection nuance — deserialization targets are simple DTOs (`ErrorDetails`, generic `T`), unaffected.
- `BlazorAdmin/Program.cs:23` — `new Uri(builder.HostEnvironment.BaseAddress)` — Api.0003 `System.Uri` behavioral change (stricter parsing) — `BaseAddress` is always a well-formed absolute URI supplied by the WASM host; unaffected.

## Validation plan
1. `dotnet build` for `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj` individually.
2. Fix any real compiler errors/warnings surfaced (expected: none beyond the above, per research).
3. Target 0 Warnings / 0 Errors for all three projects, consistent with task 01/02 standard.


## Additional fix (BlazorAdmin)
- `System.Net.Http.Json` — removed explicit PackageReference (NU1510: automatically available via net10.0 Blazor WebAssembly shared framework, same pattern as task 01/02's `System.Text.Json` removal).
