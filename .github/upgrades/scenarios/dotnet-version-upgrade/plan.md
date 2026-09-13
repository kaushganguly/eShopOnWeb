# .NET 10 LTS Upgrade Plan - eShopOnWeb

**Target Framework**: net10.0  
**Strategy**: Centralized framework update with staged package upgrades  
**Status**: In Progress

## Overview

This plan outlines the upgrade of eShopOnWeb from .NET 8 (net8.0) to .NET 10 LTS (net10.0). Assessment identified 10 projects requiring upgrade, 26 NuGet packages with recommended updates, and 61+ lines of code requiring compatibility fixes.

## Phase 1: Framework and SDK Configuration

### Task 1.1: Update Global SDK Version
- **Scope**: Update global.json SDK pin from 8.x to 10.x
- **Impact**: Affects all projects
- **Effort**: Low
- **Dependencies**: None

### Task 1.2: Update Project Target Frameworks
- **Scope**: Update all 10 projects from net8.0 to net10.0
- **Projects**:
  - src/ApplicationCore/ApplicationCore.csproj
  - src/BlazorAdmin/BlazorAdmin.csproj
  - src/BlazorShared/BlazorShared.csproj
  - src/Infrastructure/Infrastructure.csproj
  - src/PublicApi/PublicApi.csproj
  - src/Web/Web.csproj
  - tests/FunctionalTests/FunctionalTests.csproj
  - tests/IntegrationTests/IntegrationTests.csproj
  - tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
  - tests/UnitTests/UnitTests.csproj
- **Effort**: Low
- **Dependencies**: Task 1.1

## Phase 2: NuGet Package Upgrades

### Task 2.1: Upgrade ASP.NET Core Packages
- **Scope**: Upgrade Microsoft.AspNetCore.* packages to 10.0.x
- **Packages**:
  - Microsoft.AspNetCore.Authentication.JwtBearer (8.0.2 → 10.0.12)
  - Microsoft.AspNetCore.Components.Authorization (8.0.2 → 10.0.12)
  - Microsoft.AspNetCore.Components.WebAssembly (8.0.2 → 10.0.12)
  - Microsoft.AspNetCore.OpenApi (8.0.2 → 10.0.12)
- **Projects**: PublicApi, Web, BlazorAdmin
- **Effort**: Medium
- **Dependencies**: Task 1.2

### Task 2.2: Upgrade Deprecated Packages
- **Scope**: Remove or upgrade deprecated packages
- **Packages**:
  - AutoMapper.Extensions.Microsoft.DependencyInjection (deprecated)
  - BlazorInputFile (requires migration to InputFile API)
- **Effort**: Medium
- **Dependencies**: Task 2.1

### Task 2.3: Address Security Vulnerabilities
- **Scope**: Upgrade packages with known vulnerabilities
- **Packages**:
  - Azure.Identity (1.10.4 → 1.21.0)
  - Serilog.AspNetCore (if present)
- **Effort**: Low
- **Dependencies**: Task 2.1

### Task 2.4: Upgrade Entity Framework Core
- **Scope**: Update EF Core to 10.0.x
- **Packages**:
  - Microsoft.EntityFrameworkCore.* (7.0.x → 10.0.x)
- **Projects**: Infrastructure, PublicApi, Web
- **Effort**: Medium
- **Dependencies**: Task 2.1

## Phase 3: API Compatibility Fixes

### Task 3.1: Fix Source-Incompatible APIs
- **Scope**: Resolve source incompatibilities identified in assessment
- **Issues**: 3 issues identified
- **Effort**: Medium
- **Dependencies**: Task 2.1

### Task 3.2: Fix Binary-Incompatible APIs
- **Scope**: Resolve binary incompatibilities
- **Issues**: 9 issues identified (mainly in PublicApi, Web, FunctionalTests)
- **Effort**: High
- **Dependencies**: Task 3.1

### Task 3.3: Address Behavioral Changes
- **Scope**: Test and adapt to behavioral changes in .NET 10
- **Issues**: 49 behavioral changes identified
- **Effort**: Medium
- **Dependencies**: Task 3.1

## Phase 4: Validation and Testing

### Task 4.1: Build Verification
- **Scope**: Ensure solution builds without errors or warnings
- **Effort**: Low
- **Dependencies**: Task 3.3

### Task 4.2: Unit Test Execution
- **Scope**: Run all unit tests to verify functionality
- **Projects**: UnitTests, PublicApiIntegrationTests, IntegrationTests, FunctionalTests
- **Effort**: Medium
- **Dependencies**: Task 4.1

### Task 4.3: Integration Test Execution
- **Scope**: Run integration tests
- **Effort**: Medium
- **Dependencies**: Task 4.1

## Success Criteria

- ✅ All 10 projects target net10.0
- ✅ All 26 NuGet package upgrades applied
- ✅ Solution builds without errors or warnings
- ✅ All unit tests pass
- ✅ All integration tests pass
- ✅ No security vulnerabilities remain

## Estimated Effort

- **Total Tasks**: 10
- **Total Estimated LOC Changes**: 61+ lines
- **Est. Time**: 2-4 hours

## Risk Assessment

- **Low Risk**: Upgrading between LTS versions (.NET 8 LTS → .NET 10 LTS)
- **Medium Risk**: API compatibility issues (9 binary incompatible, 3 source incompatible)
- **Mitigation**: Comprehensive test execution post-upgrade

## Notes

- All projects are already SDK-style format
- No legacy projects requiring format conversion
- eShopOnWeb follows modern .NET best practices
- Breaking changes are limited to specific API incompatibilities
