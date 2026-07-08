# 04-web-storefront: Upgrade Web and its dependent test surface to net10.0

## Objective
Upgrade `src/Web/Web.csproj` from net8.0 to net10.0, and update `tests/UnitTests` and `tests/IntegrationTests` to follow. Web is the highest-risk application project with 13 package issues and 15 API issues.

## Scope

### Projects
| Project | Current TFM | Target TFM |
|---|---|---|
| `src/Web/Web.csproj` | net8.0 | net10.0 |
| `tests/UnitTests/UnitTests.csproj` | net8.0 | net10.0 |
| `tests/IntegrationTests/IntegrationTests.csproj` | net8.0 | net10.0 |

### Assessment Findings

#### Package Issues (Web)
- **NuGet.0002** (upgrade needed): AspNetCore.* 8.0.2 → 10.0.9, EF Core 8.0.2 → 10.0.9, CodeGeneration.Design 8.0.0 → 10.0.2, WebAssembly.Server 8.0.2 → 10.0.9
- **NuGet.0004** (security): Azure.Identity 1.10.4 → 1.21.0 — already fixed in task 01 (global: 1.21.0)
- **NuGet.0005** (deprecated, deferred): AutoMapper.Extensions.Microsoft.DependencyInjection, System.IdentityModel.Tokens.Jwt

#### API Issues (Web)
- **Api.0001** (binary incompatible, fix inline): DynamicallyAccessedMembers removed from configuration APIs — does NOT cause compilation errors in standard (non-AOT) builds; addressed by annotation/comment
  - `configuration.Get<CatalogSettings>()` — `ConfigureCoreServices.cs`
  - `services.Configure<CatalogSettings>(configuration)` — `ConfigureWebServices.cs`
  - `builder.Configuration.GetValue(typeof(string), "CatalogBaseUrl")` — `Program.cs`
  - `configSection.Get<BaseUrlConfiguration>()` — `Program.cs`
  - `builder.Services.Configure<BaseUrlConfiguration>(configSection)` — `Program.cs`
- **Api.0002** (source incompatible, fix inline): `TimeSpan.FromMinutes(ValidityMinutesPeriod)` where `ValidityMinutesPeriod` is `const int` — new `FromMinutes(long)` overload added in .NET 10. Cast to explicit `(double)` to ensure deterministic overload resolution.
  - `ConfigureCookieSettings.cs` line 26
- **Api.0003** (behavioral, informational): HttpContent.ReadAsStringAsync streaming, Uri length limits, UseExceptionHandler behavior, Console logging — no code changes required for server-side ASP.NET Core

#### Package Issues (IntegrationTests)
- **NuGet.0002**: Microsoft.EntityFrameworkCore.InMemory 8.0.2 → 10.0.9 (VersionOverride in csproj)
- **NuGet.0005** (deprecated, deferred): xunit

#### Package Issues (UnitTests)
- **NuGet.0005** (deprecated, deferred): xunit, xunit.runner.console

## Key Design Decisions
- Use `VersionOverride` in Web.csproj (not global changes) for AspNetCore/EF Core packages — shared projects still multi-target and use the global AspNetVersion=8.0.2
- The multi-target coexistence model in ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared is retired in practice once Web is on net10.0 — cleanup deferred to task 05
- Azure.Identity was already updated to 1.21.0 in task 01 — verify still correct in Directory.Packages.props
- TimeSpan.FromMinutes: cast ValidityMinutesPeriod to double to avoid ambiguous overload in .NET 10
