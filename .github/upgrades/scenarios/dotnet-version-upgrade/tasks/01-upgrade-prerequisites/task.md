# 01-upgrade-prerequisites: Verify net10 Baseline & Shared Upgrade Assumptions

## Objective

Confirm that the .NET 10 SDK and `global.json` constraints support the target framework (`net10.0`), capture baseline restore/build/test commands, and map solution-wide risks to owning upgrade tasks.

## Scope

- Solution: `eShopOnWeb.sln`
- All 10 SDK-style projects (already on `net8.0`, no SDK-style conversion needed)
- `global.json` — SDK version pin
- `Directory.Packages.props` — central package management

## Findings

### SDK / global.json

| Item | Before | After |
|------|--------|-------|
| `global.json` version | `8.0.x` (latestFeature) | `10.0.100` (latestFeature) |
| Resolved SDK | 8.0.423 | 10.0.302 |

**Status**: ✅ Updated. `.NET 10.0.302` is installed and resolves correctly.

### TFM Inventory (all projects currently `net8.0`)

TFM is inherited from `Directory.Packages.props` property `<TargetFramework>net8.0</TargetFramework>`. Each application task will update this per-project (the shared property in `Directory.Packages.props` will be updated last, after all projects migrate).

| Project | Type | Upgrade Task |
|---------|------|-------------|
| `src/BlazorShared` | Library | 02-blazoradmin-slice |
| `src/BlazorAdmin` | Blazor WASM App | 02-blazoradmin-slice |
| `src/ApplicationCore` | Library | 03-publicapi-slice |
| `src/Infrastructure` | Library | 03-publicapi-slice |
| `src/PublicApi` | Web API | 03-publicapi-slice |
| `src/Web` | ASP.NET Core MVC + Blazor | 04-web-slice |
| `tests/UnitTests` | Test | 03-publicapi-slice (references Web) |
| `tests/IntegrationTests` | Test | 03-publicapi-slice |
| `tests/FunctionalTests` | Test | 04-web-slice (references PublicApi + Web) |
| `tests/PublicApiIntegrationTests` | Test | 03-publicapi-slice |

### Package Hotspots (from assessment)

32 package upgrades, 12 deprecated packages, 2 security vulnerabilities mapped to owning tasks:

#### Security Vulnerabilities
| Package | Version | Severity | Advisory | Owning Task |
|---------|---------|----------|----------|-------------|
| `System.Text.Json` | 8.0.3 | HIGH | GHSA-8g4q-xg66-9fp4, GHSA-hh2w-p6rv-4g7w | 03-publicapi-slice (ApplicationCore) |
| `Azure.Identity` | 1.10.4 | MODERATE | GHSA-m5vv-6r4h-3vj9, GHSA-wvxc-855f-jvrv | 04-web-slice (Web) |

#### Key Packages Needing Version Bumps for net10.0
| Package | Current Version | Notes | Owning Task |
|---------|----------------|-------|-------------|
| `Microsoft.AspNetCore.*` (all) | 8.0.2 | Must bump to 10.x for net10.0 | 02/03/04 slices |
| `Microsoft.EntityFrameworkCore.*` | 8.0.2 | Must bump to 10.x for net10.0 | 03-publicapi-slice |
| `Swashbuckle.AspNetCore` | 6.5.0 | Incompatible with net10.0 — replace with Microsoft.AspNetCore.OpenApi or upgrade | 03-publicapi-slice |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | May need bump for net10.0 | 03/04 slices |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | Deprecated — inline fix | 03/04 slices |
| `BlazorInputFile` | 0.2.0 | Potentially incompatible with net10.0 WASM | 02-blazoradmin-slice |
| `BuildBundlerMinifier` | 3.2.449 | Deprecated/incompatible | 04-web-slice |
| `MinimalApi.Endpoint` | 1.3.0 | Compatibility check needed | 03-publicapi-slice |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.0 | Must bump | 03/04 slices |

#### Configuration-Binding API Breaks
- `IConfiguration.Bind()` changes affect both `PublicApi` and `Web` — must be fixed inline in tasks 03 and 04.

### Baseline Build/Test Commands

```bash
# Restore
dotnet restore eShopOnWeb.sln

# Build (net8.0 baseline)
dotnet build eShopOnWeb.sln --no-restore

# Unit tests
dotnet test tests/UnitTests/UnitTests.csproj --no-build

# Integration tests
dotnet test tests/IntegrationTests/IntegrationTests.csproj --no-build

# Functional tests
dotnet test tests/FunctionalTests/FunctionalTests.csproj --no-build

# PublicApi integration tests
dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj --no-build

# All tests
dotnet test eShopOnWeb.sln --no-build
```

## Baseline Results (net8.0, .NET 10 SDK)

| Command | Result |
|---------|--------|
| `dotnet restore` | ✅ Success (4 security warnings) |
| `dotnet build` | ✅ Success — 0 errors, 9 warnings |
| `dotnet test UnitTests` | ✅ 44 passed |
| `dotnet test IntegrationTests` | ✅ 3 passed |
| `dotnet test FunctionalTests` | ✅ 12 passed |

## Done Criteria — Status

- [x] .NET 10 SDK resolves (10.0.302)
- [x] `global.json` updated to `10.0.100` with `latestFeature` rollForward
- [x] Baseline restore/build/test commands identified and verified
- [x] Package and API hotspots mapped to owning upgrade tasks
