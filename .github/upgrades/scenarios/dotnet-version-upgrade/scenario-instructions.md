# .NET Version Upgrade Scenario Instructions

## Upgrade Configuration

- **Target Framework**: net10.0
- **Solution Path**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Flow Mode**: Automatic (no pauses, accept defaults)

## Projects to Upgrade

1. src/ApplicationCore/ApplicationCore.csproj
2. src/BlazorAdmin/BlazorAdmin.csproj
3. src/BlazorShared/BlazorShared.csproj
4. src/Infrastructure/Infrastructure.csproj
5. src/PublicApi/PublicApi.csproj
6. src/Web/Web.csproj
7. tests/FunctionalTests/FunctionalTests.csproj
8. tests/IntegrationTests/IntegrationTests.csproj
9. tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
10. tests/UnitTests/UnitTests.csproj

## Assessment Summary

- **Total Issues**: 119 (21 Mandatory, 84 Potential, 14 Optional)
- **Key Issues**:
  - All 10 projects need target framework update
  - 32 NuGet packages need upgrades
  - 49 API behavioral changes to verify
  - 12 deprecated packages to handle
  - Binary and source incompatibilities to resolve

## Execution Strategy

1. Update global.json SDK version to .NET 10
2. Update project target frameworks
3. Upgrade NuGet packages
4. Resolve API incompatibilities
5. Build and test

## User Preferences

- None at this time

## Decisions

- Using Automatic flow mode for end-to-end execution
- No pauses for confirmation
