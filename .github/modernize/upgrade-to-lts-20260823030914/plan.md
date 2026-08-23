# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the eShopOnWeb solution from **.NET 8.0** to **.NET 10.0** (latest LTS), as explicitly requested by the user. This ensures long-term support, access to modern runtime improvements, and continued security updates.

## Source & Target Versions

| Property | Value |
|----------|-------|
| **Source framework** | `net8.0` (.NET 8.0) |
| **Target framework** | `net10.0` (.NET 10.0 LTS) |
| **SDK-style conversion required** | No (projects already use SDK-style format) |

## Solution Projects

### Application Projects
| Project | Path |
|---------|------|
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| Web | `src/Web/Web.csproj` |

### Test Projects
| Project | Path |
|---------|------|
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

## Upgrade Scope

The upgrade encompasses:

1. **Target Framework Moniker (TFM)**: Update `TargetFramework` from `net8.0` to `net10.0` in `Directory.Build.props` (and any project files that override it).
2. **SDK version**: Update `global.json` to require .NET 10.0 SDK.
3. **NuGet package updates**: Update all `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, and other packages that publish version-aligned packages for .NET 10.
4. **API compatibility**: Address any breaking changes or deprecated APIs between .NET 8 and .NET 10 in all projects.
5. **Build & test validation**: Ensure all projects compile successfully and all tests pass after the upgrade.

## Tasks

| ID | Task | Status |
|----|------|--------|
| 001-upgrade-dotnet-to-net10 | Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 | pending |
