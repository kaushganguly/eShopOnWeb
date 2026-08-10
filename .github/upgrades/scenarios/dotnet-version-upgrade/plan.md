# Plan: eShopOnWeb .NET 8 to .NET 10 Upgrade

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0, all SDK-style, clear dependency structure.

## Tasks

### 01-prerequisites: Verify SDK and Update Global Configuration

Verify the .NET 10 SDK is installed and update global toolchain configuration so the solution can target net10.0. This task updates `global.json` to target the .NET 10 SDK and validates the required SDK version is available in the build environment. No project file changes are made here — this is pure toolchain and configuration setup.

**Done when**: `global.json` references .NET 10 SDK, `dotnet --version` confirms .NET 10 SDK is available.

### 02-upgrade-all-projects: Upgrade All Projects to net10.0

Upgrade all 10 projects to net10.0 in a single atomic pass. This covers: (1) updating `Directory.Packages.props` to set TargetFramework to net10.0 and all version properties to 10.0.x-compatible values, (2) updating individual project `.csproj` files where needed, (3) removing or replacing the incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` package, (4) fixing all binary-incompatible and source-incompatible API calls identified in the assessment. Key signals: Web.csproj has 15 API issues (ConfigurationBinder.Get<T>, Options.Configure<T> binary incompatible, 4 occurrences each); PublicApi.csproj has 5 API issues; ApplicationCore.csproj has source-incompatible Exception serialization constructor; System.Security.Claims should be removed (built into framework); Azure.Identity needs security fix (1.10.4 → 1.21.0).

**Done when**: All projects have TargetFramework=net10.0, all package references target compatible versions, all binary/source-incompatible API usages are fixed, `dotnet build eShopOnWeb.sln` exits with 0 errors.

### 03-final-validation: Validate Build and Unit Tests

Run the full solution build and unit test suite to confirm the upgrade is complete and correct. No code changes are made here — purely validation. Run `dotnet build` to confirm 0 errors, then run `dotnet test` targeting the UnitTests project.

**Done when**: `dotnet build eShopOnWeb.sln` exits with 0 errors and `dotnet test tests/UnitTests/UnitTests.csproj` passes all tests.
