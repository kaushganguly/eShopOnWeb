# Upgrade Plan: .NET 8.0 → .NET 10.0

## Objective
Upgrade all projects in the eShopOnWeb solution from `net8.0` to `net10.0`.

## Strategy
Direct upgrade (net8.0 → net10.0) — all projects upgraded in dependency order.

## Target Framework
- **From**: net8.0
- **To**: net10.0 (LTS, support ends Nov 2028)

## Projects (in dependency order)
1. `src/BlazorShared` — no dependencies
2. `src/ApplicationCore` — depends on BlazorShared
3. `src/Infrastructure` — depends on ApplicationCore
4. `src/BlazorAdmin` — depends on BlazorShared
5. `src/PublicApi` — depends on ApplicationCore, Infrastructure
6. `src/Web` — depends on ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure
7. `tests/UnitTests` — depends on ApplicationCore, Web
8. `tests/IntegrationTests` — depends on Infrastructure, UnitTests
9. `tests/FunctionalTests` — depends on ApplicationCore, PublicApi, Web
10. `tests/PublicApiIntegrationTests` — depends on PublicApi, Web

## Tasks

### 01-central-config — Update Central Configuration
Update `Directory.Packages.props` (TFM + package versions) and `global.json` (SDK version).

**Changes to Directory.Packages.props:**
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.11
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0
- `System.Text.Json`: 8.0.3 → 10.0.11
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- `Swashbuckle.AspNetCore`: 6.5.0 → 8.1.1
- `Swashbuckle.AspNetCore.SwaggerUI`: 6.5.0 → 8.1.1
- `Swashbuckle.AspNetCore.Annotations`: 6.5.0 → 8.1.1

**Changes to global.json:**
- SDK version: "8.0.x" → "10.0.400"

### 02-remove-incompatible-packages — Remove/Replace Incompatible Packages
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from `PublicApi.csproj` (no .NET 10 version)
- Remove `System.Security.Claims` from `ApplicationCore.csproj` (included in framework)

### 03-fix-breaking-api-changes — Fix Source-Incompatible API Changes
- Remove deprecated serialization constructor from `ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
- Fix `TimeSpan.FromMinutes` ambiguity in `Web/Configuration/ConfigureCookieSettings.cs`

### 04-build-and-test — Build and Validate
- Restore packages, build solution, run unit tests
- Fix any remaining compilation errors
