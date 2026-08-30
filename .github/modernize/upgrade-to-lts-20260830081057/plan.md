# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0** (latest LTS), as explicitly requested by the user.

| | |
|---|---|
| **Source Version** | .NET 8.0 (`net8.0`) |
| **Target Version** | .NET 10.0 (`net10.0`) |
| **Upgrade Trigger** | User explicitly requested upgrade to latest LTS |

---

## Projects in Solution

| Project | Path |
|---------|------|
| Web | `src/Web/Web.csproj` |
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

---

## Upgrade Scope

The target framework is defined centrally in `Directory.Build.props` (`<TargetFramework>net8.0</TargetFramework>`), so the TFM change applies to all projects at once.

Key areas of work:
1. **Target Framework Moniker (TFM)**: Update `net8.0` → `net10.0` in `Directory.Build.props` and `global.json`
2. **NuGet Package Updates**: Bump all ASP.NET Core, Entity Framework Core, and related packages to their .NET 10-compatible versions
3. **API Compatibility**: Address any breaking changes between .NET 8 and .NET 10 (runtime behavior, removed/changed APIs)
4. **SDK Update**: Update `global.json` SDK version to `10.0.x`

---

## Tasks

| # | Task | Status |
|---|------|--------|
| 1 | Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 | pending |
