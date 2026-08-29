# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0** (latest LTS). The user explicitly requested an upgrade to the latest LTS version.

## Current State

| Property | Value |
|----------|-------|
| Source .NET version | `net8.0` (.NET 8) |
| Target .NET version | `net10.0` (.NET 10 LTS) |
| SDK version (global.json) | `8.0.x` |
| Project type | Modern .NET (SDK-style) — no SDK-style conversion required |

## Projects in Solution

| Project | Path | SDK |
|---------|------|-----|
| Web | `src/Web/Web.csproj` | `Microsoft.NET.Sdk.Web` |
| PublicApi | `src/PublicApi/PublicApi.csproj` | `Microsoft.NET.Sdk.Web` |
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` | `Microsoft.NET.Sdk` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` | `Microsoft.NET.Sdk` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` | `Microsoft.NET.Sdk.BlazorWebAssembly` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` | `Microsoft.NET.Sdk` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` | `Microsoft.NET.Sdk` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` | `Microsoft.NET.Sdk` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` | `Microsoft.NET.Sdk` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | `Microsoft.NET.Sdk` |

## Upgrade Scope

1. **Target Framework Moniker (TFM)**: Update `<TargetFramework>` in `Directory.Packages.props` from `net8.0` to `net10.0`
2. **global.json**: Update SDK version from `8.0.x` to `10.0.x`
3. **NuGet packages**: Update all package versions to versions compatible with `net10.0`, including ASP.NET Core, Entity Framework Core, and Azure SDK packages
4. **CI workflow**: Update `.github/workflows/dotnetcore.yml` to use .NET `10.0.x`
5. **API compatibility**: Address any breaking changes between .NET 8 and .NET 10

## Tasks

| # | Task | Status |
|---|------|--------|
| 1 | Upgrade eShopOnWeb from net8.0 to net10.0 | Pending |
