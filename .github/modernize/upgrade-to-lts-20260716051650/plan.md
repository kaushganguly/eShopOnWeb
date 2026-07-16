# .NET Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0 LTS**.

The user has explicitly requested an upgrade to the latest LTS version. The current version (net8.0) is functional but .NET 10 is the latest Long-Term Support release, providing improved performance, security, and extended support lifetime.

---

## Current State

| Property | Value |
|----------|-------|
| **Source .NET Version** | net8.0 |
| **Target .NET Version** | net10.0 |
| **SDK Version (global.json)** | 8.0.x |
| **Package Version Management** | Central (Directory.Build.props / Directory.Packages.props) |

---

## Projects in Solution

| Project | Type | Path |
|---------|------|------|
| ApplicationCore | Class Library | `src/ApplicationCore/ApplicationCore.csproj` |
| Infrastructure | Class Library | `src/Infrastructure/Infrastructure.csproj` |
| Web | ASP.NET Core Web App (MVC + Blazor) | `src/Web/Web.csproj` |
| PublicApi | ASP.NET Core Web API | `src/PublicApi/PublicApi.csproj` |
| BlazorAdmin | Blazor WebAssembly | `src/BlazorAdmin/BlazorAdmin.csproj` |
| BlazorShared | Class Library | `src/BlazorShared/BlazorShared.csproj` |
| UnitTests | xUnit Test Project | `tests/UnitTests/UnitTests.csproj` |
| IntegrationTests | xUnit Test Project | `tests/IntegrationTests/IntegrationTests.csproj` |
| FunctionalTests | xUnit Test Project | `tests/FunctionalTests/FunctionalTests.csproj` |
| PublicApiIntegrationTests | MSTest Integration Test Project | `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` |

---

## Upgrade Scope

1. **TargetFramework**: Update `<TargetFramework>net8.0</TargetFramework>` to `net10.0` in `Directory.Build.props`
2. **SDK version**: Update `global.json` from `8.0.x` to `10.0.x`
3. **NuGet packages**: Update all versioned packages in `Directory.Build.props` to their net10.0-compatible versions, including:
   - All `Microsoft.AspNetCore.*` packages (AspNetVersion → 10.x)
   - All `Microsoft.EntityFrameworkCore.*` packages (EntityFrameworkCoreVersion → 10.x)
   - All `Microsoft.Extensions.*` packages (SystemExtensionVersion → 10.x)
   - Other third-party packages as needed for compatibility
4. **API compatibility**: Address any breaking changes introduced between .NET 8 and .NET 10
5. **Build and test validation**: Ensure all projects compile and all unit/integration/functional tests pass

---

## Tasks

| # | Task | Status |
|---|------|--------|
| 1 | Upgrade eShopOnWeb from net8.0 to net10.0 | pending |
