# Plan: .NET 8 → .NET 10 Upgrade

**Solution**: eShopOnWeb.sln  
**Source**: net8.0  
**Target**: net10.0  
**Strategy**: Single-shot upgrade (all projects via centralized Directory.Packages.props)

## Tasks

### 001-upgrade-dotnet-to-net10: Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0

**Description**: Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0

**Objective**: Update all 10 projects from net8.0 to net10.0, update all NuGet packages to .NET 10-compatible versions, update global.json and CI workflow, fix any breaking changes, and ensure the solution builds and tests pass.

**Scope**:
- `Directory.Packages.props` — Update `<TargetFramework>`, version variables, all package versions
- `global.json` — Update SDK version to 10.0.x
- `.github/workflows/dotnetcore.yml` — Update dotnet-version to 10.0.x
- All 10 .csproj files — Remove obsolete `<DotNetCliToolReference>` from FunctionalTests.csproj
- Source code — Fix any breaking API changes (Swashbuckle, MediatR, Ardalis, JWT)

**Steps**:
1. Update `Directory.Packages.props`: Change TFM and all version variables/packages
2. Update `global.json` SDK version to `10.0.x`
3. Update CI workflow dotnet-version to `10.0.x`
4. Remove obsolete `<DotNetCliToolReference>` from FunctionalTests.csproj
5. Build and fix all compilation errors
6. Run tests and fix test failures

**Success Criteria**:
- Solution builds without errors or warnings
- All tests pass (UnitTests, IntegrationTests)
