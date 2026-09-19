# .NET 10.0 Upgrade Plan

## Overview

Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 across 10 projects.

**Target Framework**: net10.0 (LTS — Support ends Nov 2028)

**Projects**: 10 total
- Core: BlazorShared, ApplicationCore, Infrastructure, BlazorAdmin, Web, PublicApi
- Tests: UnitTests, PublicApiIntegrationTests, FunctionalTests, IntegrationTests

**Key Changes**:
- All project target frameworks: net8.0 → net10.0
- 19 NuGet packages require upgrade (mostly Microsoft AspNetCore/EntityFrameworkCore 8.0.2 → 10.0.12)
- API compatibility issues: 9 binary incompatible, 3 source incompatible, 49 behavioral changes
- Estimated LOC modifications: 61+ (0.5% of codebase)

## Upgrade Strategy

**Single-shot upgrade** to .NET 10.0 across all projects in dependency order:

1. **Phase 1: Framework Upgrade (Projects 1-6)**
   - Upgrade target frameworks in library projects first (BlazorShared, ApplicationCore, Infrastructure)
   - Then upgrade application projects (BlazorAdmin, Web, PublicApi)

2. **Phase 2: Test Projects (Projects 7-10)**
   - Upgrade test projects after core projects

3. **Phase 3: NuGet Package Upgrade**
   - After all target frameworks upgraded
   - Upgrade packages to .NET 10.0 compatible versions

4. **Phase 4: Code Fixes & Testing**
   - Fix breaking changes in API calls
   - Validate build success
   - Run unit tests
   - Run integration tests

## Tasks

### 1. Upgrade BlazorShared Target Framework
- **Project**: src/BlazorShared/BlazorShared.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: None (library, no dependencies)
- **Difficulty**: Low

### 2. Upgrade ApplicationCore Target Framework
- **Project**: src/ApplicationCore/ApplicationCore.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: BlazorShared
- **Difficulty**: Low
- **Issues**: 2 API issues to fix

### 3. Upgrade Infrastructure Target Framework
- **Project**: src/Infrastructure/Infrastructure.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: ApplicationCore
- **Difficulty**: Low

### 4. Upgrade BlazorAdmin Target Framework
- **Project**: src/BlazorAdmin/BlazorAdmin.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: BlazorShared, Infrastructure, ApplicationCore
- **Difficulty**: Low
- **Issues**: 3 API issues to fix

### 5. Upgrade Web Target Framework
- **Project**: src/Web/Web.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: ApplicationCore, Infrastructure
- **Difficulty**: Low
- **Issues**: 15 API issues to fix (largest in core apps)

### 6. Upgrade PublicApi Target Framework
- **Project**: src/PublicApi/PublicApi.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: ApplicationCore, Infrastructure
- **Difficulty**: Low
- **Issues**: 5 API issues to fix, binary incompatibilities

### 7. Upgrade UnitTests Target Framework
- **Project**: tests/UnitTests/UnitTests.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: ApplicationCore
- **Difficulty**: Low

### 8. Upgrade PublicApiIntegrationTests Target Framework
- **Project**: tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: PublicApi
- **Difficulty**: Low
- **Issues**: 7 API issues to fix

### 9. Upgrade FunctionalTests Target Framework
- **Project**: tests/FunctionalTests/FunctionalTests.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: Web
- **Difficulty**: Low
- **Issues**: 29 API issues to fix (most in tests)

### 10. Upgrade IntegrationTests Target Framework
- **Project**: tests/IntegrationTests/IntegrationTests.csproj
- **Change**: net8.0 → net10.0
- **Type**: Framework Upgrade
- **Dependencies**: ApplicationCore, Infrastructure
- **Difficulty**: Low

### 11. Upgrade NuGet Packages
- **Packages to Upgrade** (19 packages):
  - Microsoft.AspNetCore.* (8.0.2 → 10.0.12)
  - Microsoft.EntityFrameworkCore.* (8.0.2 → 10.0.12)
  - Microsoft.Extensions.* (8.0.x → 10.0.12)
  - System.Net.Http.Json (8.0.0 → 10.0.12)
  - System.Text.Json (8.0.3 → 10.0.12)
  - Azure.Identity (1.10.4 → 1.21.0)
  - Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.0 → 10.0.2)
- **Type**: Package Upgrade
- **Applies to**: All projects

### 12. Fix Code Compatibility Issues
- **Focus Areas**:
  - System.Net.Http.HttpContent behavioral changes (52.5% of API issues)
  - System.Uri behavioral changes (18.0% of API issues)
  - Configuration binding breaking changes (binary incompatible)
  - Exception serialization changes (source incompatible)
- **Type**: Code Fixes
- **Affected Files**: 32 files with incidents

### 13. Validate Build and Tests
- **Build**: Solution compiles without errors
- **Unit Tests**: All pass
- **Integration Tests**: All pass
- **Functional Tests**: All pass

## Execution Constraints

- **Order**: Follow topological dependency order for framework upgrades
- **NuGet Timing**: Upgrade packages after all framework changes
- **Validation**: Build and test after each phase
- **Rollback**: If critical issues found, revert to .NET 8.0

## Success Criteria

✅ All 10 projects target net10.0
✅ All 19 packages upgraded to .NET 10.0 compatible versions
✅ Solution builds without errors
✅ All unit tests pass
✅ All integration tests pass
✅ All functional tests pass
✅ No regressions in functionality

