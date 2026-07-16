# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0** (latest LTS). The user explicitly requested upgrading to the latest LTS version. .NET 10.0 provides long-term support, improved performance, and the latest platform features.

## Source Version

- **Framework**: .NET 8.0 (`net8.0`)
- **SDK**: 8.0.x (pinned in `global.json`)
- **Framework defined in**: `Directory.Packages.props` (`<TargetFramework>net8.0</TargetFramework>`)

## Target Version

- **Framework**: .NET 10.0 (`net10.0`)
- **SDK**: 10.0.x

## Projects in Solution

| Project | Path |
|---------|------|
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| BlazorAdmin | `src/BlazorAdmin/BlazorAdmin.csproj` |
| BlazorShared | `src/BlazorShared/BlazorShared.csproj` |
| Web | `src/Web/Web.csproj` |
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` |
| PublicApi | `src/PublicApi/PublicApi.csproj` |
| UnitTests | `tests/UnitTests/UnitTests.csproj` |
| PublicApiIntegrationTests | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |
| FunctionalTests | `tests/FunctionalTests/FunctionalTests.csproj` |
| IntegrationTests | `tests/IntegrationTests/IntegrationTests.csproj` |

## Upgrade Scope

1. **Target Framework Moniker (TFM)**: Update `<TargetFramework>` from `net8.0` to `net10.0` in `Directory.Packages.props` (and any individual `.csproj` files that override it).
2. **SDK version**: Update `global.json` to use the .NET 10 SDK (`10.0.x`).
3. **NuGet packages**: Update all version properties in `Directory.Packages.props` — including `AspNetVersion`, `EntityFrameworkCoreVersion`, `SystemExtensionVersion`, `VSCodeGeneratorVersion` — to their .NET 10-compatible releases.
4. **API compatibility**: Address any breaking changes between .NET 8 and .NET 10 (Blazor WebAssembly, ASP.NET Core, EF Core, Identity).
5. **Build validation**: Ensure all 10 projects compile and all existing tests pass.

## Tasks

| # | Task | Status |
|---|------|--------|
| 1 | Upgrade eShopOnWeb from net8.0 to net10.0 | Pending |
