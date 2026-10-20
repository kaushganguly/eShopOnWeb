# .NET Upgrade Plan

## Summary

| Item | Value |
|------|-------|
| Source .NET version | .NET 8.0 (`net8.0`) |
| Target .NET version | .NET 10.0 (`net10.0`) |
| Solution | eShopOnWeb.sln |

The target framework for this solution is defined centrally in `Directory.Packages.props` (`<TargetFramework>net8.0</TargetFramework>`), which is consumed by all projects.

## Projects in Solution

| Project | Path | Type |
|---------|------|------|
| ApplicationCore | src\ApplicationCore\ApplicationCore.csproj | Class library |
| Infrastructure | src\Infrastructure\Infrastructure.csproj | Class library |
| PublicApi | src\PublicApi\PublicApi.csproj | ASP.NET Core Web API |
| Web | src\Web\Web.csproj | ASP.NET Core Web App |
| BlazorShared | src\BlazorShared\BlazorShared.csproj | Class library |
| BlazorAdmin | src\BlazorAdmin\BlazorAdmin.csproj | Blazor WebAssembly |
| UnitTests | tests\UnitTests\UnitTests.csproj | Test project |
| IntegrationTests | tests\IntegrationTests\IntegrationTests.csproj | Test project |
| FunctionalTests | tests\FunctionalTests\FunctionalTests.csproj | Test project |
| PublicApiIntegrationTests | tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj | Test project |

## Upgrade Scope

- Update the centrally managed `<TargetFramework>` in `Directory.Packages.props` (and any project-level overrides) from `net8.0` to `net10.0` across all projects in the solution.
- Update NuGet package references (ASP.NET Core, EF Core, Blazor, Azure SDK, and related packages) in `Directory.Packages.props` to versions compatible with .NET 10.
- Resolve any breaking API changes, deprecated APIs, or behavioral differences introduced between .NET 8 and .NET 10.
- Verify build output and runtime configuration (e.g., `global.json`, Dockerfiles, CI/CD pipeline SDK references) are updated to target .NET 10.
- Ensure the solution builds successfully and all existing unit/integration tests pass after the upgrade.

## Rationale

Upgrading to .NET 10 (the current LTS release) ensures continued long-term support, access to the latest performance and security improvements, and compatibility with the latest Azure SDKs and tooling.
