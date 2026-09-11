# Upgrade Plan: eShopOnWeb to .NET 10 LTS

## Executive Summary

Upgrade eShopOnWeb solution from .NET 8 (net8.0) to .NET 10 LTS (net10.0) across 10 projects in a single atomic upgrade operation.

**Key Metrics**:
- Total Projects: 10
- All SDK-style projects
- All on net8.0 target framework
- Assessment identified 26 package updates needed, 119 total issues including API changes

## Selected Strategy

**All-At-Once** — All projects upgraded simultaneously in a single operation.

**Rationale**: 10 projects all on modern .NET 8.0, clear dependency structure, straightforward TFM bump with package version and code fixes.

## Upgrade Options Applied

| Option | Selected | Rationale |
|--------|----------|-----------|
| Upgrade Strategy | All-At-Once | 10 modern .NET projects, mechanical framework upgrade |
| Unsupported Packages | Defer Resolution | 7 incompatible packages requiring stub approach for compilation |
| Unsupported API Handling | Fix Inline | 12 API changes across modern-to-modern migration fixed directly |

## Projects to Upgrade

### Application Projects
- src/ApplicationCore/ApplicationCore.csproj (Class Library)
- src/Infrastructure/Infrastructure.csproj (Class Library)
- src/BlazorAdmin/BlazorAdmin.csproj (Blazor WebAssembly)
- src/BlazorShared/BlazorShared.csproj (Razor Class Library)
- src/Web/Web.csproj (ASP.NET Core MVC)
- src/PublicApi/PublicApi.csproj (ASP.NET Core API)

### Test Projects
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Task Breakdown

### Task 1: Prerequisites and Toolchain Setup
**Description**: Verify .NET SDK compatibility and global.json configuration

**What needs to happen**:
1. Verify .NET 10 SDK is installed and accessible
2. Validate global.json configuration for .NET 10 compatibility
3. Review and document any breaking API or package changes from assessment

**Success Criteria**:
- .NET 10 SDK is installed and working
- global.json is compatible or will be updated as part of TFM upgrade
- Assessment findings are reviewed and understood

### Task 2: Upgrade All Projects - Target Framework and Packages
**Description**: Upgrade target framework to net10.0 and update all packages across all 10 projects

**What needs to happen**:
1. Update TargetFramework in all 10 .csproj files from net8.0 to net10.0
2. Update Directory.Packages.props (centralized package version definitions) with compatible versions
3. Update NuGet package versions across all projects (26 packages need updating)
4. Apply deferred resolution stubs for 7 incompatible packages
5. Update ASP.NET Core, Blazor, EF Core, and test framework packages
6. Restore dependencies and resolve dependency conflicts
7. Build solution and fix all compilation errors (API incompatibilities, missing APIs)
8. Fix behavioral changes introduced by framework upgrade

**Success Criteria**:
- All projects target net10.0
- Solution builds with zero errors
- All package versions are compatible with .NET 10
- Incompatible packages have resolution stubs or are deferred to follow-up tasks

### Task 3: Validation and Testing
**Description**: Validate the upgraded solution through comprehensive build and test execution

**What needs to happen**:
1. Clean build of entire solution
2. Run all unit tests in UnitTests project
3. Run integration tests in IntegrationTests project
4. Run functional tests in FunctionalTests project
5. Run PublicApi integration tests in PublicApiIntegrationTests project
6. Verify Blazor WebAssembly compilation
7. Verify ASP.NET Core Web and PublicApi projects start correctly
8. Document any deferred work for follow-up

**Success Criteria**:
- Solution builds with zero errors
- All unit tests pass (passBuild=true, passUnitTests=true)
- All integration tests pass
- All functional tests pass
- No warnings in build output
- Blazor and ASP.NET Core projects validate correctly

## Dependency Order

All projects are upgraded simultaneously — no tier-based ordering required. The dependency graph (ApplicationCore → Web/PublicApi/Infrastructure, etc.) is handled within the single upgrade task.

## Risk Assessment

**Low**: This is a modern-to-modern TFM upgrade with well-supported ecosystem. Breaking changes are limited and primarily in API removals/changes, which have known replacements or workarounds.

**Mitigations**:
- Assessment has identified all breaking changes and API issues upfront
- Deferred resolution stubs allow incremental API fix-up after framework upgrade
- Comprehensive test suite validates behavior after upgrade
- No architectural changes required — only code-level adaptations

## Notes

- Central package management (Directory.Packages.props) must be updated to support consistent package versions across all projects
- Side-by-side web migration not required — all projects can be upgraded in place
- Build validation is critical after each step to catch breaking changes early
