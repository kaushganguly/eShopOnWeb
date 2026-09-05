# Upgrade Plan: eShopOnWeb — net8.0 → net10.0

## Solution Overview

- **Solution**: eShopOnWeb.sln
- **Projects**: 10 (6 source, 4 test)
- **Current TFM**: net8.0
- **Target TFM**: net10.0 (LTS)
- **Package Management**: CPM (ManagePackageVersionsCentrally=true)

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0 (modern .NET), all SDK-style, CPM in use, clear dependency structure with ≤ 5 dependency tiers.

### Dependency Graph

```
Level 4 (tests): [UnitTests] [IntegrationTests] [FunctionalTests] [PublicApiIntegrationTests]
                      ↓               ↓                  ↓                     ↓
Level 3 (apps):              [Web] [PublicApi]
                                  ↓
Level 2:                    [Infrastructure]
                                  ↓
Level 1:                [ApplicationCore] [BlazorAdmin]
                                  ↓
Level 0:                     [BlazorShared]
```

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Upgrade Strategy | All-at-Once | All projects on modern .NET, ≤15 projects, low complexity |
| Package Management | CPM (existing) | ManagePackageVersionsCentrally already in use |

---

## Tasks

### 01-prerequisites: Verify Prerequisites and Update SDK

Update `global.json` to use .NET 10 SDK and verify the SDK is available. The current `global.json` specifies `8.0.x`; this needs to be updated to `10.0.x` to enable the upgrade. All 10 projects target net8.0, and the .NET 10 SDK (10.0.400) is already installed on this machine.

This is a preparatory task required before TFM or package changes will succeed cleanly.

**Done when**: `global.json` references .NET 10 SDK version, and `dotnet --version` reports a 10.x SDK.

---

### 02-upgrade-projects: Upgrade All Projects to net10.0

Update all 10 projects from net8.0 to net10.0. This is the core upgrade task. It covers:

1. **TFM update** in `Directory.Packages.props` (the `<TargetFramework>` property used by all projects) and individual `.csproj` files where necessary.
2. **Package updates** via `Directory.Packages.props`:
   - Bump `<AspNetVersion>` to `10.0.11` (Microsoft.AspNetCore.\*, Microsoft.Extensions.\*, Microsoft.EntityFrameworkCore.\*)
   - Bump `<EntityFramworkCoreVersion>` to `10.0.11`
   - Bump `<SystemExtensionVersion>` to `10.0.11`
   - Bump `<VSCodeGeneratorVersion>` to `10.0.2`
   - Update `Azure.Identity` from `1.10.4` to `1.21.0` (security vulnerability fix)
   - Update `System.Text.Json` from `8.0.3` to `10.0.11` (security vulnerability fix)
   - Update `System.IdentityModel.Tokens.Jwt` from `7.3.1` to `8.22.0` (deprecated)
   - Remove `System.Security.Claims` 4.3.0 — now built into the framework
   - Remove or replace `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 (incompatible, no net10.0 support)
3. **API breaking changes** to fix:
   - `ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` line 11: Remove obsolete serialization constructor (`Exception(SerializationInfo, StreamingContext)` was removed in .NET 9+)
   - `src/PublicApi/Program.cs` lines 41-49: `IConfiguration.Get<T>()` and `Configure<T>(IConfiguration)` now return nullable — add null checks
   - `src/Web/Configuration/ConfigureCoreServices.cs` line 21: Same `IConfiguration.Get<T>()` null-safety fix
   - `src/Web/Configuration/ConfigureWebServices.cs` line 15: Same `Configure<T>(IConfiguration)` fix
   - `src/Web/Configuration/ConfigureCookieSettings.cs` line 25: `TimeSpan.FromMinutes(double)` now returns an `int`-parameter overload check — verify usage is still correct
   - Behavioral changes (Api.0003): `HttpContent.ReadAsStringAsync()`, `UseExceptionHandler`, `AddConsole` — verify no functional issues

**Known risks**:
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` has no net10.0-compatible version — it may need to be removed or pinned as a non-framework-bound tool reference
- `AutoMapper.Extensions.Microsoft.DependencyInjection` is deprecated but still functional — leave as-is unless it blocks compilation
- MSTest packages (3.2.2) and xunit (2.7.0) deprecated — may need updating to latest

**Done when**: All 10 projects show `net10.0` TFM, all package versions updated, `dotnet restore` and `dotnet build` succeed with 0 errors and 0 warnings on modified projects.

---

### 03-validate: Final Validation

Run the full solution build and unit test suite to confirm the upgrade is complete and correct.

- `dotnet build eShopOnWeb.sln` — 0 errors, 0 warnings
- `dotnet test tests/UnitTests/UnitTests.csproj` — all tests pass

**Done when**: Build succeeds with 0 errors, unit tests pass.
