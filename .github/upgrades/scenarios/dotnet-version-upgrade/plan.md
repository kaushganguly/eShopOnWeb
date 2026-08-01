# .NET Version Upgrade Plan

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single atomic pass.
**Rationale**: 10 projects, all on net8.0, clear dependency structure, ≤15 projects scope.

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects — 3 web apps (Web, PublicApi, BlazorAdmin), 3 libraries (ApplicationCore, Infrastructure, BlazorShared), 4 test projects

## Tasks

### 01-prerequisites: Verify and update SDK prerequisites

Verify the .NET 10 SDK is installed and update `global.json` to allow the .NET 10 SDK. The current `global.json` pins `8.0.x` with `latestFeature` rollForward. Update the version constraint to `10.0.x` so the build system uses the correct SDK.

All projects inherit `TargetFramework` from `Directory.Packages.props` (an unusual but valid CPM setup used in this repo). No `Directory.Build.props` exists — all shared properties flow through `Directory.Packages.props`.

**Done when**: `global.json` specifies a .NET 10 SDK version and `dotnet --version` confirms the SDK is available.

---

### 02-upgrade-projects: Upgrade all projects to net10.0

Update all project configuration files and fix breaking changes across all 10 projects. This is the core upgrade task covering TFM changes, package version updates, removal of incompatible/redundant packages, and source-level API fixes.

**TFM and version variables** (in `Directory.Packages.props`):
- Change `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- Update `<AspNetVersion>` → `10.0.10`
- Update `<SystemExtensionVersion>` → `10.0.10`
- Update `<EntityFramworkCoreVersion>` → `10.0.10`
- Update `<VSCodeGeneratorVersion>` → `10.0.2`

**Package updates** (in `Directory.Packages.props`):
- `Azure.Identity` 1.10.4 → 1.21.0 (security vulnerability fix, NuGet.0004)
- `System.Text.Json` 8.0.3 → 10.0.10 (security vulnerability fix + version alignment)
- `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.22.0 (deprecated, move to latest)
- Remove `System.Security.Claims` 4.3.0 — functionality included in net10.0 framework reference (NuGet.0003)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 — no compatible version for net10.0 (NuGet.0001); this is a build-time tooling package not required at runtime
- Remove `Microsoft.AspNetCore.Mvc` 2.2.0 — incompatible old version; ASP.NET Core MVC is now part of the framework
- `Microsoft.Extensions.Logging.Configuration` → update to use `$(SystemExtensionVersion)`
- `System.Net.Http.Json` → update to use `$(SystemExtensionVersion)`

**API breaking changes** (source code fixes):

1. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` (Api.0002):
   - Serialization constructor `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` is obsolete/removed in .NET 9+
   - Remove the protected serialization constructor and `[Serializable]` attribute if present

2. `src/Web/Configuration/ConfigureWebServices.cs` (Api.0001, line 15):
   - `services.Configure<CatalogSettings>(configuration)` — `Configure<T>(IConfiguration)` overload behavior changed
   - Replace with `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))` or bind using `configuration.Bind(settings)` pattern

3. `src/Web/Configuration/ConfigureCoreServices.cs` (Api.0001, line 21):
   - `configuration.Get<CatalogSettings>()` — extension method signature changed; add explicit options parameter
   - Replace with `configuration.Get<CatalogSettings>(options => options.ErrorOnUnknownConfiguration = false) ?? new CatalogSettings()`

4. `src/Web/Configuration/ConfigureCookieSettings.cs` (Api.0002, line 25):
   - `TimeSpan.FromMinutes(double)` — obsolete in favor of integer overload
   - Replace with `TimeSpan.FromMinutes((int)ValidityMinutesPeriod)` or `TimeSpan.FromMinutes(60)` if the value is a constant

5. `src/PublicApi/Program.cs` (Api.0001, lines 41-49):
   - `builder.Services.Configure<CatalogSettings>(builder.Configuration)` and `configSection.Get<BaseUrlConfiguration>()` — same breaking changes as Web
   - Apply same patterns as above

**Done when**: All 10 projects target net10.0, all package versions updated in `Directory.Packages.props`, all API breaking changes resolved inline, and the full solution builds without errors.

---

### 03-final-validation: Validate build and tests

Run a full solution build to confirm zero compilation errors, then execute the unit test suite. This validates the upgrade is complete and no behavioral regressions were introduced by the API changes.

Ensure no build warnings remain in modified projects. Document any deferred items (behavioral changes flagged as Api.0003 that require runtime verification).

**Done when**: `dotnet build eShopOnWeb.sln` exits with 0 errors, `dotnet test` for unit tests passes all tests.
