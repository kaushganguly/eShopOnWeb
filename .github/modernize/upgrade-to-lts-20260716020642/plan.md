# .NET Upgrade Plan: eShopOnWeb

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0 (LTS)**.

The user explicitly requested an upgrade to the latest LTS version. .NET 10.0 is the current latest LTS, providing long-term support, improved performance, and access to modern runtime and framework APIs.

## Source Version

- **Current .NET version**: .NET 8.0 (`net8.0`)
- **SDK version**: 8.0.x (defined in `global.json`)

## Target Version

- **Target .NET version**: .NET 10.0 (`net10.0`)

## Projects in Solution

| Project | Path |
|---------|------|
| Web | `src/Web/Web.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

## Upgrade Scope

1. **Target Framework Moniker (TFM)**: Update `<TargetFramework>` from `net8.0` to `net10.0` in `Directory.Packages.props` and all `.csproj` files.
2. **SDK version**: Update `global.json` to target the .NET 10.0 SDK.
3. **NuGet package updates**: Update all package versions (ASP.NET Core, Entity Framework Core, Azure SDK, and third-party packages) to versions compatible with .NET 10.0.
4. **API compatibility**: Resolve any breaking changes or deprecated APIs introduced between .NET 8.0 and .NET 10.0.
5. **Build validation**: Ensure all projects compile successfully after the upgrade.
6. **Test validation**: Ensure all existing unit and integration tests pass after the upgrade.
