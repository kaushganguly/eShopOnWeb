# 02-core-upgrade: Upgrade All Projects to net10.0

## Objective
Upgrade all 10 eShopOnWeb projects from net8.0 to net10.0 by updating Directory.Packages.props, removing incompatible packages, and fixing API breaking changes.

## Scope
- **Directory.Packages.props** – TFM variable, version variables, individual package versions, remove System.Security.Claims
- **src/PublicApi/PublicApi.csproj** – remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets PackageReference
- **src/Web/Configuration/ConfigureCoreServices.cs** – ConfigurationBinder.Get<T>() nullable fix
- **src/Web/Configuration/ConfigureWebServices.cs** – Configure<T>(IConfiguration) overload fix
- **src/Web/Program.cs** – ConfigurationBinder.Get<T>() and GetValue nullable fixes
- **src/PublicApi/Program.cs** – Configure<T>(IConfiguration) and Get<T>() fixes

## Research Findings

### TargetFramework Location
`TargetFramework` is defined only in `Directory.Packages.props` line 4 as a property variable (`<TargetFramework>net8.0</TargetFramework>`). No individual .csproj files set it explicitly — they all inherit via the `$(TargetFramework)` property from this central file.

### Projects (10 total)
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorAdmin/BlazorAdmin.csproj
- src/BlazorShared/BlazorShared.csproj
- src/Infrastructure/Infrastructure.csproj
- src/PublicApi/PublicApi.csproj
- src/Web/Web.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- tests/UnitTests/UnitTests.csproj

### Directory.Packages.props Version Variables
- `$(AspNetVersion)` = 8.0.2 → 10.0.9 (covers: Microsoft.AspNetCore.* packages, Microsoft.Extensions.Identity.Core, Microsoft.AspNetCore.Mvc.Testing)
- `$(SystemExtensionVersion)` = 8.0.0 → 10.0.9 (covers: Microsoft.Extensions.Logging.Configuration, System.Net.Http.Json)
- `$(EntityFramworkCoreVersion)` = 8.0.2 → 10.0.9 (covers: Microsoft.EntityFrameworkCore.*)
- `$(VSCodeGeneratorVersion)` = 8.0.0 → 10.0.2 (covers: Microsoft.VisualStudio.Web.CodeGeneration.Design)

### Packages with Explicit Versions (not via variables)
- Azure.Identity: 1.10.4 → 1.21.0
- System.Text.Json: 8.0.3 → 10.0.9

### Packages to Remove
- `System.Security.Claims` Version 4.3.0 — included in .NET 10 framework
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — no net10.0-compatible version; remove from PublicApi.csproj PackageReference AND Directory.Packages.props PackageVersion

### API Breaking Changes Found
1. `ConfigureWebServices.cs:16` — `services.Configure<CatalogSettings>(configuration)` passes full IConfiguration
2. `PublicApi/Program.cs:42` — `builder.Services.Configure<CatalogSettings>(builder.Configuration)` passes full IConfiguration
3. `Web/Program.cs:99` — `configSection.Get<BaseUrlConfiguration>()` → returns T? in .NET 10 (null-forgiving already at line 104)
4. `PublicApi/Program.cs:50` — `configSection.Get<BaseUrlConfiguration>()` → returns T? (null-forgiving already at line 78)
5. `ConfigureCoreServices.cs:22` — `configuration.Get<CatalogSettings>() ?? new CatalogSettings()` → should work with T? and ?? fallback
6. `Web/Program.cs:141` — `builder.Configuration.GetValue(typeof(string), "CatalogBaseUrl") as string` → returns object?, cast to string is fine

## Steps
1. Update Directory.Packages.props
2. Remove incompatible package from PublicApi.csproj
3. Fix API breaking changes
4. Build and fix errors iteratively
