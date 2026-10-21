# .NET Upgrade Plan — eShopOnWeb

## Summary

| | |
|---|---|
| **Source .NET version** | .NET 8.0 (`net8.0`) |
| **Target .NET version** | .NET 10.0 (`net10.0`) — latest LTS |
| **Upgrade type** | Modern .NET to modern .NET (no SDK-style conversion required) |

## Projects in Solution

All projects currently target `net8.0` via the centrally managed `TargetFramework` property in `Directory.Packages.props`:

1. `src\ApplicationCore\ApplicationCore.csproj`
2. `src\BlazorAdmin\BlazorAdmin.csproj`
3. `src\BlazorShared\BlazorShared.csproj`
4. `src\Infrastructure\Infrastructure.csproj`
5. `src\PublicApi\PublicApi.csproj`
6. `src\Web\Web.csproj`
7. `tests\FunctionalTests\FunctionalTests.csproj`
8. `tests\IntegrationTests\IntegrationTests.csproj`
9. `tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj`
10. `tests\UnitTests\UnitTests.csproj`

## Upgrade Scope

- Update the central `TargetFramework` property (`Directory.Packages.props`) from `net8.0` to `net10.0` and propagate to all 10 projects.
- Update centrally managed NuGet package versions (ASP.NET Core, Entity Framework Core, Azure SDK, Identity, MediatR, Ardalis.* libraries, etc.) in `Directory.Packages.props` to versions compatible with .NET 10.0.
- Review and update API usages affected by breaking changes between .NET 8 and .NET 10 (ASP.NET Core, EF Core, Blazor WebAssembly/Server hosting model, Identity).
- Validate build configuration, analyzers, and language version settings remain compatible with `net10.0`.
- Rebuild the solution and run the existing unit, integration, and functional test suites to confirm no regressions.

## Why This Upgrade Is Needed

The user explicitly requested upgrading the application to the latest LTS version of .NET. .NET 10.0 is the current latest LTS release, providing continued long-term support, security updates, performance improvements, and compatibility with the latest Azure SDKs and tooling.
