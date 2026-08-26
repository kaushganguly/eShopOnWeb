# .NET Upgrade Plan: eShopOnWeb

## Summary

Upgrade the eShopOnWeb solution from **.NET 8.0** to **.NET 10.0** (latest LTS).

## Source and Target Versions

| | Version |
|---|---|
| **Source** | .NET 8.0 (`net8.0`) |
| **Target** | .NET 10.0 (`net10.0`) |

## Reason

The user explicitly requested an upgrade to the latest LTS version. .NET 10.0 is the current latest LTS, succeeding .NET 8.0. Upgrading ensures access to the latest performance improvements, security patches, language features, and long-term support through the .NET 10 lifecycle.

## Projects in Solution

| Project | Path |
|---------|------|
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| Web | `src/Web/Web.csproj` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

## Upgrade Scope

- Update `TargetFramework` from `net8.0` to `net10.0` in `Directory.Packages.props` and all `.csproj` files
- Update the .NET SDK version in `global.json` from `8.0.x` to `10.0.x`
- Update all NuGet package versions to .NET 10-compatible releases (ASP.NET Core, Entity Framework Core, etc.)
- Resolve any breaking API changes or deprecated APIs introduced between .NET 8 and .NET 10
- Ensure all projects build successfully and all unit/integration tests pass

## Tasks

1. **Upgrade .NET to net10.0** — Update the entire solution from net8.0 to net10.0, including TFM changes, SDK update, NuGet package updates, and API compatibility fixes.
