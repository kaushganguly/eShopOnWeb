# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade **eShopOnWeb** from **.NET 8.0** to **.NET 10.0 LTS** to benefit from long-term support, performance improvements, and the latest platform features.

## Source & Target Versions

| | Version |
|---|---|
| **Source** | .NET 8.0 (`net8.0`) |
| **Target** | .NET 10.0 LTS (`net10.0`) |

## Projects in Solution

| Project | Path |
|---|---|
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

The upgrade is centrally managed via `Directory.Packages.props`, which defines `<TargetFramework>net8.0</TargetFramework>` and package versions. The upgrade will:

1. **Update the target framework** from `net8.0` to `net10.0` in `Directory.Packages.props`
2. **Update the SDK version** in `global.json` from `8.0.x` to `10.0.x`
3. **Update NuGet package versions** — all `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, `Azure.*`, and other packages pinned to 8.0.x versions will be updated to their .NET 10-compatible releases
4. **Resolve breaking changes** — address any API or behavior changes introduced between .NET 8 and .NET 10
5. **Validate** — ensure the solution builds and all unit tests pass

## Tasks

| ID | Description | Status |
|---|---|---|
| 001-upgrade-dotnet-to-net10 | Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 | pending |
