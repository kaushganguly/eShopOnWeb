# .NET Upgrade Plan: eShopOnWeb

## Overview

Upgrade the eShopOnWeb solution from **.NET 8.0** (`net8.0`) to **.NET 10.0** (`net10.0`), the current Long-Term Support (LTS) release.

## Source Version

- **Framework**: .NET 8.0 (`net8.0`)
- **Defined in**: `Directory.Packages.props` (centrally managed)

## Target Version

- **Framework**: .NET 10.0 (`net10.0`)

## Projects in Solution

| Project | Path |
|---------|------|
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| Web | `src/Web/Web.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

## Upgrade Scope

1. **Target Framework Moniker (TFM)**: Update `<TargetFramework>net8.0</TargetFramework>` to `net10.0` in `Directory.Packages.props`.
2. **NuGet Package Updates**: Update all `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, `Azure.*`, and other packages to versions compatible with .NET 10.
3. **API Compatibility**: Resolve any breaking changes or deprecated APIs between .NET 8 and .NET 10.
4. **Build Verification**: Ensure all projects compile successfully after the upgrade.
5. **Test Verification**: Ensure all unit and integration tests pass after the upgrade.

## Rationale

The user explicitly requested an upgrade to the latest LTS version. .NET 10.0 is the current LTS release, providing long-term support, improved performance, and the latest platform features. Upgrading from .NET 8.0 ensures the application benefits from all improvements introduced in .NET 9 and .NET 10.
