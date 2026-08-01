# 02-upgrade-projects: Upgrade all projects to net10.0

Update all project configuration files and fix breaking changes across all 10 projects. This is the core upgrade task covering TFM changes, package version updates, removal of incompatible/redundant packages, and source-level API fixes.

## Research Findings

### Projects Affected (10 total)
All projects inherit `TargetFramework` from `Directory.Packages.props` (no `Directory.Build.props` exists). Individual project files have no `<TargetFramework>` element — it's set globally via the CPM setup.

- `src/ApplicationCore/ApplicationCore.csproj` — 6 issues: source incompatible API (serialization constructor), package to remove (System.Security.Claims), package to update (System.Text.Json), security vuln (System.Text.Json)
- `src/BlazorAdmin/BlazorAdmin.csproj` — 11 issues: package upgrades, behavioral changes
- `src/BlazorShared/BlazorShared.csproj` — 1 issue: TFM update only
- `src/Infrastructure/Infrastructure.csproj` — 5 issues: package upgrades, deprecated packages
- `src/PublicApi/PublicApi.csproj` — 17 issues: binary incompatible APIs, incompatible package (MSVS Azure Containers Tools), deprecated packages
- `src/Web/Web.csproj` — 29 issues: binary+source incompatible APIs, security vuln (Azure.Identity), deprecated packages
- `tests/FunctionalTests/FunctionalTests.csproj` — package upgrades, deprecated packages
- `tests/IntegrationTests/IntegrationTests.csproj` — package upgrades, deprecated packages
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — package upgrades, deprecated packages
- `tests/UnitTests/UnitTests.csproj` — deprecated packages, TFM update

### Files to Modify

#### Directory.Packages.props (primary configuration file)
- Change `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
- Update `<AspNetVersion>8.0.2</AspNetVersion>` → `10.0.10`
- Update `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` → `10.0.10`
- Update `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` → `10.0.10`
- Update `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` → `10.0.2`
- Update `Azure.Identity` version `1.10.4` → `1.21.0` (security vuln fix)
- Update `System.Text.Json` version `8.0.3` → `10.0.10` (security vuln + version alignment)
- Update `System.IdentityModel.Tokens.Jwt` version `7.3.1` → `8.22.0` (deprecated, move to latest)
- Remove `System.Security.Claims` 4.3.0 — included in net10.0 framework reference (NuGet.0003)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 — no compatible net10.0 version (NuGet.0001)
- Remove `Microsoft.AspNetCore.Mvc` 2.2.0 — old version not compatible with net10.0

#### Source Code API Fixes

**`src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`** (Api.0002, line 11)
- Serialization constructor `protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context) : base(info, context)` is obsolete in .NET 9+
- The `ISerializable` pattern (protected serialization ctor + `[Serializable]`) was removed/obsoleted in .NET 9
- Remove the protected serialization constructor and its `using` statements

**`src/Web/Configuration/ConfigureWebServices.cs`** (Api.0001, line 15)
- `services.Configure<CatalogSettings>(configuration)` — `IServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` changed behavior
- Fix: Use `services.Configure<CatalogSettings>(configuration.GetSection(nameof(CatalogSettings)))` or bind a specific section

**`src/Web/Configuration/ConfigureCoreServices.cs`** (Api.0001, line 21)
- `configuration.Get<CatalogSettings>()` — `ConfigurationBinder.Get<T>(IConfiguration)` signature changed
- Fix: Use `configuration.Get<CatalogSettings>(new BinderOptions { ErrorOnUnknownConfiguration = false }) ?? new CatalogSettings()`
- Or simpler: bind to the section: `configuration.GetSection(nameof(CatalogSettings)).Get<CatalogSettings>() ?? new CatalogSettings()`

**`src/Web/Configuration/ConfigureCookieSettings.cs`** (Api.0002, line 25)
- `TimeSpan.FromMinutes(ValidityMinutesPeriod)` where ValidityMinutesPeriod is a double — `TimeSpan.FromMinutes(double)` is now obsolete in .NET 9+
- Fix: Cast to int `TimeSpan.FromMinutes((int)ValidityMinutesPeriod)` or keep double but use `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` with compiler pragma, or just change the variable type

**`src/PublicApi/Program.cs`** (Api.0001, lines 41-49)
- `builder.Services.Configure<CatalogSettings>(builder.Configuration)` — same issue as Web
- `builder.Configuration.Get<CatalogSettings>()` — same issue as Web
- `builder.Services.Configure<BaseUrlConfiguration>(configSection)` — same Configure<T>(IConfiguration) issue
- `configSection.Get<BaseUrlConfiguration>()` — same Get<T>() issue
- Fix: Apply same patterns as Web fixes above

**`src/PublicApi/PublicApi.csproj`**
- Remove `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` from individual project file

### Packages to Update
| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| AspNetVersion packages | 8.0.2 | 10.0.10 | via variable in Directory.Packages.props |
| EntityFrameworkCore packages | 8.0.2 | 10.0.10 | via variable in Directory.Packages.props |
| System.Text.Json | 8.0.3 | 10.0.10 | security vuln + version alignment |
| Azure.Identity | 1.10.4 | 1.21.0 | security vulnerability fix |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 | deprecated, move to latest |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 | via variable |
| System.Net.Http.Json | 8.0.0 | 10.0.10 | via variable |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.10 | via variable |

### Packages to Remove
| Package | Reason |
|---------|--------|
| System.Security.Claims 4.3.0 | Included in net10.0 framework reference (NuGet.0003) |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6 | No compatible net10.0 version (NuGet.0001) |
| Microsoft.AspNetCore.Mvc 2.2.0 | Old version incompatible with net10.0 |

### AutoMapper deprecation
- `AutoMapper.Extensions.Microsoft.DependencyInjection` is deprecated — functionality included in AutoMapper directly
- Assessment says to keep same version (12.0.1) but note: the package still works, just deprecated warning
- Keep at 12.0.1 for now (it's still functional, just deprecated)

## Done When
- All 10 projects target net10.0
- All package versions updated in Directory.Packages.props
- All mandatory API breaking changes resolved inline
- Full solution builds without errors

## Execution Results

### Build: ✅ PASSED
- Errors: 0
- Warnings: 20 (all pre-existing third-party security advisories and xUnit analyzer suggestions)
- All 10 projects built targeting `net10.0`

### Additional Issues Found and Fixed
1. **CS0433 Program type ambiguity** — .NET 10 exposes `Program` from both `PublicApi` and `Web` to `PublicApiIntegrationTests`. Fixed with `extern alias WebAlias` on the `Web` project reference.
2. **NU1010 System.Security.Claims** — CPM requires removing the `PackageReference` from `ApplicationCore.csproj` after removing the `PackageVersion` entry. Fixed.
