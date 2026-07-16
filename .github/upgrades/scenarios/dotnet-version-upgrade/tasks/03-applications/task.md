# 03-applications: Upgrade Application Projects to net10.0

## Objective
Upgrade the two ASP.NET Core application projects (`src/PublicApi` and `src/Web`) from net8.0 to net10.0, and remove the incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` package from `PublicApi.csproj`.

## Scope
- `src/PublicApi/PublicApi.csproj`
- `src/Web/Web.csproj`
- Source files in PublicApi and Web projects

## Completed Changes

### 1. TFM (Target Framework Moniker)
The `TargetFramework` was already centrally set to `net10.0` via `Directory.Packages.props` by the previous task (02-shared-libraries). No per-project `<TargetFramework>` element was needed in either csproj file.

### 2. Removed Incompatible Package – PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` from `src/PublicApi/PublicApi.csproj`.
- This package (v1.19.6) has no supported version for net10.0 and is development-time tooling only (no runtime impact).

### 3. Binary-Incompatible APIs — Assessed, No Action Needed
The following APIs were flagged as binary-incompatible in the assessment:
- `ConfigurationBinder.Get<T>(IConfiguration)` — still source-compatible in .NET 10; no compile errors observed.
- `Configure<T>(IServiceCollection, IConfiguration)` — still source-compatible; no compile errors observed.
- `TimeSpan.FromMinutes(double)` in `ConfigureCookieSettings.cs` — no signature change; the `ValidityMinutesPeriod` const is already an `int` (assigned to `const int`), auto-promoted to `double`; no issue.

Existing null-coalescing patterns (`?? new CatalogSettings()`) were already in place where needed.

### 4. Behavioral Changes — Accepted
- `builder.Logging.AddConsole()` behavioral change (PublicApi, Web) — no code change needed per assessment.
- `ReadAsStringAsync()` in health checks — acceptable behavioral change.
- `app.UseExceptionHandler("/Error")` — no action needed.

## Build Results
- **PublicApi**: 0 errors, 0 C# compiler warnings (6 pre-existing NuGet vulnerability notices: NU1903/NU1901)
- **Web**: 0 errors, 0 C# compiler warnings (6 pre-existing NuGet vulnerability notices: NU1903/NU1901)

## Pre-existing NuGet Warnings (Not Introduced by This Task)
- `NU1903`: AutoMapper 12.0.1 — high severity advisory GHSA-rvv3-g6hj-g44x (transitive dep from AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1)
- `NU1901`: NuGet.Packaging/Protocol 6.12.1 — low severity advisory (transitive from tooling packages)

These warnings existed before this task and are out of scope for this migration step.

## Unit Tests
All 44 unit tests pass (net10.0).

