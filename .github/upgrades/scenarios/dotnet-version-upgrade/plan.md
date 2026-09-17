# .NET 8.0 → .NET 10.0 Upgrade Plan

## Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.

**Rationale**: 10 projects, all on modern .NET 8.0 with SDK-style format, clear dependency structure, 26 package updates needed with no critical blockers, and all low-difficulty projects. Assessment shows ~61 lines of code to modify (0.5% of codebase) with mostly behavioral API changes. This is an ideal candidate for atomic upgrade.

## Project Overview

### Libraries (Foundation Layer)
- src/ApplicationCore/ApplicationCore.csproj - Core domain models and interfaces
- src/Infrastructure/Infrastructure.csproj - Data access and external services
- src/BlazorShared/BlazorShared.csproj - Shared Blazor components and logic

### Applications
- src/Web/Web.csproj - ASP.NET Core MVC web application
- src/PublicApi/PublicApi.csproj - ASP.NET Core REST API
- src/BlazorAdmin/BlazorAdmin.csproj - Blazor WebAssembly admin portal

### Tests
- tests/UnitTests/UnitTests.csproj - Unit tests
- tests/IntegrationTests/IntegrationTests.csproj - Integration tests
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj - API integration tests
- tests/FunctionalTests/FunctionalTests.csproj - End-to-end functional tests

## Assessment Summary

| Category | Details |
|----------|---------|
| Total Projects | 10 (all require upgrade) |
| Total Packages | 48 (26 upgrades needed) |
| Package Status | 22 compatible, 7 incompatible, 19 upgrade-recommended |
| API Issues | 9 binary incompatible, 3 source incompatible, 49 behavioral changes |
| Code Impact | ~61+ lines to modify (~0.5% of codebase) |
| Risk Level | 🟢 Low - all projects modern .NET, SDK-style, straightforward upgrade |

## Upgrade Tasks

### 01-prerequisites: Verify SDK compatibility and prepare environment

Verify that the .NET 10 SDK is available and compatible with the project structure. Update global.json to specify .NET 10.0.x version. This is a prerequisite before any project files are modified to ensure all tooling supports the target framework.

The global.json file currently specifies .NET 8.0. It must be updated to allow .NET 10.0 SDK, which is required for both compilation and runtime of net10.0 projects. Check that no local tool dependencies conflict with .NET 10 availability.

**Done when**:
- global.json is updated to SDK version 10.0.0 or later
- `dotnet --version` confirms .NET 10.0 or compatible SDK is available
- No tooling conflicts are detected

### 02-update-tfm-all-projects: Update target framework in all projects

Update the TargetFramework property from net8.0 to net10.0 in all 10 project files. This includes:
- 3 library projects (ApplicationCore, Infrastructure, BlazorShared)
- 3 application projects (Web, PublicApi, BlazorAdmin)
- 4 test projects (UnitTests, IntegrationTests, PublicApiIntegrationTests, FunctionalTests)

No code changes required in this task — only project file modification. The assessment shows all projects use SDK-style format with straightforward TargetFramework properties, so this is a mechanical update.

**Done when**:
- All 10 .csproj files have TargetFramework set to net10.0
- No other project file changes introduced

### 03-update-nuget-packages: Upgrade all NuGet packages to .NET 10 compatible versions

Upgrade all 26 packages that require updates to versions compatible with .NET 10. This includes:
- Microsoft.* and ASP.NET Core packages (currently 8.x versions)
- Entity Framework Core packages
- Other framework-dependent packages

The assessment identified 7 packages as currently incompatible with net10.0 and 19 packages recommended for upgrade. Use `dotnet package upgrade` or manual package reference updates to bring all packages to their latest net10.0-compatible versions. 

Key packages to focus on:
- Microsoft.AspNetCore.* (web, API, Blazor)
- Microsoft.EntityFrameworkCore.* (data access)
- Microsoft.Extensions.* (configuration, DI, logging, etc.)
- Other infrastructure packages

**Done when**:
- All packages updated in all projects
- No packages report incompatibility with net10.0
- NuGet restore completes without warnings
- No security vulnerabilities in updated packages

### 04-fix-api-compatibility: Address API breaking changes and behavioral updates

Fix all binary and source-incompatible API issues identified in the assessment (9 binary incompatible, 3 source incompatible, 49 behavioral changes).

Primary issues to address:
- **ConfigurationBinder.Get/Configure methods** (4+ issues): These APIs changed signatures in .NET 10. Update calls to use new overloads.
- **System.Uri constructor** (2 issues in source-incompatible): Uri constructor now requires explicit scheme validation. Review and fix affected code.
- **System.Exception serialization** (2 source-incompatible): BinaryFormatter removal affects exception serialization. Update if serialization is used.
- **System.TimeSpan.FromMinutes** (1 source-incompatible): Parameter type changes. Update affected calls.
- **Behavioral changes** (HttpContent, Uri handling, logging, etc.): Test at runtime; most are non-breaking but may need minor adjustments.

The assessment shows these issues concentrated in:
- src/Web/Web.csproj (15 API issues)
- tests/FunctionalTests/FunctionalTests.csproj (29 API issues)
- src/PublicApi/PublicApi.csproj (5 API issues)
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj (7 API issues)

**Done when**:
- All binary incompatible APIs fixed (9 issues)
- All source incompatible APIs fixed (3 issues)
- Solution builds without compilation errors
- All behavioral changes reviewed and addressed
- No new warnings introduced

### 05-validate-solution: Build solution and run full test suite

Build the entire solution to verify no compilation errors or warnings exist. Then run the complete test suite:
- Unit tests (UnitTests)
- Integration tests (IntegrationTests, PublicApiIntegrationTests)
- Functional tests (FunctionalTests)

Verify all tests pass with .NET 10.0. This is the final validation step confirming the upgrade is successful and no regressions were introduced.

**Done when**:
- Solution builds completely with 0 errors and 0 warnings
- All 4 test suites run successfully with all tests passing
- No behavioral regressions detected
- Application can start and basic functionality confirmed

## Execution Timeline

| Phase | Tasks | Key Activities |
|-------|-------|-----------------|
| Setup | 01 | Update SDK, verify tooling |
| Core Upgrade | 02-03 | Update all TFMs and packages |
| Fixing | 04 | Address API compatibility issues |
| Validation | 05 | Build, test, verify |

**Total Estimated Duration**: 2-4 hours depending on API fix complexity

## Known Risks & Mitigations

| Risk | Assessment Signals | Mitigation |
|------|-------------------|-----------|
| API compatibility issues | 9 binary + 3 source incompatible | Systematic fix-and-test approach per API issue category |
| HttpContent behavioral changes | 32 usages detected | Review and test HTTP-based features (Web, PublicApi projects) |
| Configuration binding changes | 8+ issues detected | Update DI/configuration setup in startup files |
| Package compatibility | 7 incompatible packages | Verify latest versions are net10.0 compatible during upgrade |
| Test failures | 36 API issues in test projects | Run all test suites after fixes; may need test code updates |

## Success Criteria

✅ All projects target net10.0
✅ All 48 packages present and compatible with net10.0
✅ Solution builds without errors or warnings
✅ All unit tests pass
✅ All integration tests pass
✅ All functional tests pass
✅ No deprecated packages
✅ No security vulnerabilities
✅ Application starts and basic functionality verified
