# .NET 10 Upgrade Plan — eShopOnWeb

## Overview

Upgrade all 10 projects in the eShopOnWeb solution from `net8.0` to `net10.0` using the All-at-Once strategy. All projects are currently on net8.0 (modern .NET), so this is a straightforward TFM bump with package updates and inline API fixes.

**Strategy**: All-At-Once — All projects upgraded simultaneously in a single operation. 10 projects, all on net8.0, clear dependency structure.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects on net8.0; straightforward modern-to-modern TFM bump; single atomic pass is fastest |
| Unsupported Packages | Resolve Inline | 1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets in PublicApi) |
| Unsupported API Handling | Fix Inline | Modern-to-modern upgrade; few API changes; fix inline in same task |

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Confirm the .NET 10 SDK is installed and compatible with the solution. Update `global.json` to
require the .NET 10.0.x SDK. The solution uses Central Package Management via `Directory.Packages.props`.
The current `global.json` pins an 8.0.x SDK and must be updated to a .NET 10 rollForward policy.

**Done when**: `global.json` references .NET 10.0.x SDK; `dotnet --version` shows a 10.x SDK;
solution loads without SDK compatibility errors.

---

### 02-upgrade-all-projects: Upgrade all 10 projects from net8.0 to net10.0

Update all 10 projects to target `net10.0` in a single atomic pass. Projects affected:
BlazorShared (L0), ApplicationCore (L1), BlazorAdmin (L1), Infrastructure (L2), Web (L3),
PublicApi (L3), UnitTests (L4), FunctionalTests (L4), PublicApiIntegrationTests (L4), IntegrationTests (L5).

Work items:
1. TFM: Update TargetFramework net8.0 → net10.0 in every .csproj
2. Packages via Directory.Packages.props: update Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*,
   Microsoft.Extensions.*, System.Text.Json (8.0.3 has security vuln → 10.x), Azure.Identity (1.10.4
   has security vuln → 1.21.0+), System.IdentityModel.Tokens.Jwt (7.3.1 → 8.22.0+), and all other
   8.0.x Microsoft packages to net10-compatible versions
3. Remove System.Security.Claims 4.3.0 from ApplicationCore (now in framework reference)
4. Handle Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6 in PublicApi (incompatible, no net10 version — remove or find compatible alternative)
5. Fix Api.0001: ConfigurationBinder.Get<T>(IConfiguration) and Configure<T>(IServiceCollection, IConfiguration)
   in src/Web/Configuration/ConfigureCoreServices.cs (line 21), src/Web/Configuration/ConfigureWebServices.cs
   (line 15), src/PublicApi/Program.cs (lines 41-49) — use IConfigurationSection overload
6. Fix Api.0002: TimeSpan.FromMinutes() in src/Web/Configuration/ConfigureCookieSettings.cs (line 25)
7. Fix Api.0002: Remove obsolete Exception(SerializationInfo, StreamingContext) constructor in
   src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs (line 11)
8. Review Api.0003 behavioral changes in HttpContent.ReadAsStringAsync() and UseExceptionHandler

**Done when**: All 10 projects target net10.0; `dotnet build eShopOnWeb.sln` completes with 0 errors
and 0 warnings in modified files; all package references resolve successfully.

---

### 03-validate: Run unit tests and confirm upgrade success

Execute the unit test suite and verify the upgrade is complete and functional.
Run `dotnet test tests/UnitTests/UnitTests.csproj`. Fix any test failures caused by behavioral
changes between .NET 8 and .NET 10. Commit all changes to the working branch `upgrade/dotnet-10`.

**Done when**: `dotnet build eShopOnWeb.sln` shows 0 errors; unit tests pass; changes committed to `upgrade/dotnet-10`.
