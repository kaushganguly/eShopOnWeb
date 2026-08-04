# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0** (latest LTS).

The current target framework (`net8.0`) is defined centrally in `Directory.Packages.props`. All projects inherit this setting. The user has explicitly requested an upgrade to the latest LTS version.

## Source and Target Versions

| Property | Value |
|----------|-------|
| **Source .NET version** | net8.0 |
| **Target .NET version** | net10.0 |
| **SDK version (global.json)** | 8.0.x → 10.0.x |

## Projects in Solution

| Project | Path | SDK |
|---------|------|-----|
| Web | `src/Web/Web.csproj` | `Microsoft.NET.Sdk.Web` |
| PublicApi | `src/PublicApi/PublicApi.csproj` | `Microsoft.NET.Sdk.Web` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` | `Microsoft.NET.Sdk.BlazorWebAssembly` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` | `Microsoft.NET.Sdk` |
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` | `Microsoft.NET.Sdk` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` | `Microsoft.NET.Sdk` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` | `Microsoft.NET.Sdk` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` | `Microsoft.NET.Sdk` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` | `Microsoft.NET.Sdk` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | `Microsoft.NET.Sdk` |

## Upgrade Scope

1. **Target Framework**: Update `<TargetFramework>` from `net8.0` to `net10.0` in `Directory.Packages.props`.
2. **SDK Version**: Update `global.json` SDK version from `8.0.x` to `10.0.x`.
3. **Package Versions**: Update all `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, and other version-pinned NuGet packages in `Directory.Packages.props` to versions compatible with .NET 10.
4. **API Compatibility**: Resolve any breaking API changes introduced between .NET 8 and .NET 10.
5. **Blazor WebAssembly**: Ensure `BlazorAdmin` and `BlazorShared` are compatible with .NET 10 Blazor changes.

## Tasks

1. **001-upgrade-dotnet-to-net10** — Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0
