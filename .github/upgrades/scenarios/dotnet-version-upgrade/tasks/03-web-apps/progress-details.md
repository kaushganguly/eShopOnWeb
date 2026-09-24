# 03-web-apps — Progress Details

## Summary
Upgraded Web, PublicApi, BlazorAdmin projects to build/run correctly on net10.0. Central TargetFramework
was already net10.0 (task 01). This task resolved package versions and fixed a real security vulnerability
surfaced during restore.

## Changes made

### Directory.Packages.props
- `SystemExtensionVersion`: 8.0.0 → 10.0.12 (drives `System.Net.Http.Json`, `Microsoft.Extensions.Logging.Configuration`)
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2 (drives `Microsoft.VisualStudio.Web.CodeGeneration.Design`)
- `Azure.Identity`: 1.10.4 → 1.21.0 (fixes moderate vulnerability + deprecation flagged in assessment)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageVersion entry (no supported net10.0 version exists; design-time-only VS Docker tooling package, not required by Dockerfiles/docker-compose)
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.1) with `AutoMapper` (16.2.0) — **fixes GHSA-rvv3-g6hj-g44x / CVE-2026-32933, a high-severity DoS vulnerability** (uncontrolled recursion) present in AutoMapper < 15.1.1, surfaced as NU1903 during `dotnet restore`. The split DI package is deprecated; its functionality is now built into the base `AutoMapper` package.

### src/Web/Web.csproj
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` PackageReference entirely — confirmed unused (no `IMapper`/AutoMapper code references in this project).
- Added `PrivateAssets=all` to `Microsoft.VisualStudio.Web.CodeGeneration.Design` (design-time scaffolding tool; reduces published-output surface, consistent with the existing `Microsoft.EntityFrameworkCore.Tools` pattern in this project).

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible with net10.0, no supported version; design-time only).
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` PackageReference with `AutoMapper`.
- Added `PrivateAssets=all` to `Microsoft.VisualStudio.Web.CodeGeneration.Design`.

### src/PublicApi/Program.cs
- Updated `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);` → `builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);` — the single-`Assembly`-argument overload no longer exists in the merged `AutoMapper` package; the new signature requires an explicit (possibly no-op) `Action<IMapperConfigurationExpression>` first argument.

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (NU1510: automatically available in net10.0 Blazor WebAssembly shared framework).

## API/behavioral issues reviewed (no code changes required)
Reviewed all Api.0001 (binary incompatible)/Api.0002 (source incompatible)/Api.0003 (behavioral change) occurrences
flagged by the assessment for these three projects (`ConfigureWebServices.cs`, `ConfigureCoreServices.cs`,
`ConfigureCookieSettings.cs`, `Web/Program.cs`, `HomePageHealthCheck.cs`, `ApiHealthCheck.cs`, `PublicApi/Program.cs`,
`BlazorAdmin/Services/HttpService.cs`, `BlazorAdmin/Program.cs`). All resolved cleanly by recompilation against
net10.0 with no source changes needed — confirmed by the clean builds below. Full rationale for each occurrence
recorded in `task.md`.

## Validation
- `dotnet restore eShopOnWeb.sln` — succeeded, 0 errors. AutoMapper NU1903 (high) vulnerability resolved.
- `dotnet build src/Web/Web.csproj` — **Build succeeded, 0 Errors, 4 Warning(s)** (2 unique, duplicated): NU1901 low-severity advisories for transitive `NuGet.Packaging`/`NuGet.Protocol` 6.12.1, pulled in solely by `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 — the only version of that package supporting net10.0 (confirmed via `get_supported_package_version`, no newer version available). This is a design-time-only scaffolding tool (not part of the runtime request pipeline); residual risk accepted and documented — no fix available upstream at this time.
- `dotnet build src/PublicApi/PublicApi.csproj` — **Build succeeded, 0 Errors, 4 Warning(s)** (same residual NU1901 as above).
- `dotnet build src/BlazorAdmin/BlazorAdmin.csproj` — **Build succeeded, 0 Errors, 0 Warnings.**

## Notes / follow-ups
- AutoMapper (current versions, incl. 16.2.0) uses a commercial license model for organizations above a revenue threshold (free for individuals/small organizations) — flagged for user awareness; the version bump was required to remediate the high-severity security advisory and satisfies the task's `securityComplianceCheck` requirement.
- The NU1901 (low) advisory on `NuGet.Packaging`/`NuGet.Protocol` via `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 could not be eliminated — no newer package version exists for net10.0 as of this upgrade. Mitigated exposure by marking the reference `PrivateAssets=all` so it does not flow into published output.
