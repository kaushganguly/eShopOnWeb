# Upgrade Plan: .NET 8 to .NET 10

**Solution**: eShopOnWeb.sln
**Source Framework**: net8.0
**Target Framework**: net10.0 (LTS - support ends Nov 2028)

## Selected Strategy

**All-at-Once** - All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), all rated Low difficulty. Simple TFM bump with package updates and minor API fixes; incremental approach adds overhead without benefit.

## Upgrade Options

| Option | Selected | Category |
|--------|----------|----------|
| Upgrade Strategy | All-at-Once | Strategy |
| Unsupported Packages | Resolve Inline | Compatibility |
| Unsupported API Handling | Fix Inline | Compatibility |

## Projects

Libraries (Levels 0-2):
- src/BlazorShared/BlazorShared.csproj (ClassLibrary)
- src/ApplicationCore/ApplicationCore.csproj (ClassLibrary)
- src/BlazorAdmin/BlazorAdmin.csproj (AspNetCore)
- src/Infrastructure/Infrastructure.csproj (ClassLibrary)

Applications (Level 3):
- src/PublicApi/PublicApi.csproj (AspNetCore)
- src/Web/Web.csproj (AspNetCore)

Test Projects (Levels 4-5):
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

### 01-prerequisites: Verify SDK and update global.json

Verify the .NET 10 SDK is available in the environment. Update global.json to target SDK 10.0.x so the solution uses the correct toolchain. The current global.json pins an 8.0.x SDK version and must be updated before project files are changed to avoid toolchain mismatch.

Check whether the installed .NET 10 SDK is compatible with the global.json rollForward policy. Use the version reported by dotnet --list-sdks for the exact SDK patch version.

**Done when**: dotnet --version resolves to a .NET 10 SDK; global.json specifies a .NET 10 SDK version.

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Upgrade all 10 projects from net8.0 to net10.0. This is the all-at-once core task: update TFMs, bump all NuGet packages, remove the incompatible package, remove the redundant framework-included package, and fix all breaking API changes, then build the full solution to zero errors.

The TargetFramework is defined centrally - verify location and update accordingly. Also update any per-project overrides. Key package changes: update all Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, Microsoft.Extensions.* from 8.0.x to 10.0.x; update Azure.Identity from 1.10.4 to 1.21.0 (security vulnerability fix); update System.Text.Json, System.Net.Http.Json to 10.0.x; update Microsoft.VisualStudio.Web.CodeGeneration.Design to 10.0.2; update System.IdentityModel.Tokens.Jwt from 7.3.1 to 8.22.0; remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible, no net10.0 version); remove System.Security.Claims (functionality included in framework). Fix API breaking changes inline: ConfigurationBinder.Get<T>() and OptionsConfigurationServiceCollectionExtensions.Configure<T>() (binary incompatible) in src/PublicApi/Program.cs and src/Web/Configuration/; TimeSpan.FromMinutes(double) source incompatible in src/Web/Configuration/ConfigureCookieSettings.cs. Review behavioral changes (Api.0003) in HttpContent.ReadAsStringAsync() usages and app.UseExceptionHandler(). Build solution and fix all compilation errors in a single bounded pass.

**Done when**: Solution builds with 0 errors and 0 warnings in modified projects; all unit and integration tests pass.

### 03-final-validation: Full solution validation

Run the full test suite and validate all projects target net10.0. Confirm no net8.0 TFM references remain and no deferred items exist from the upgrade.

**Done when**: dotnet build eShopOnWeb.sln succeeds with 0 errors; dotnet test eShopOnWeb.sln passes all tests; no net8.0 TFM references remain.
