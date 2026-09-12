# .NET 8 to .NET 10 Upgrade Plan

## Overview

Upgrade the eShopOnWeb application from .NET 8.0 to .NET 10.0 (LTS). This plan addresses:
- Central framework version update in Directory.Packages.props
- SDK version update in global.json
- Package version updates for .NET 10 compatibility
- API compatibility fixes
- Build and test validation

## Strategy

**Sequential Framework Update**: Update central configuration first, then build and validate incrementally.

1. **Central Configuration (Directory.Packages.props & global.json)**
   - Update TargetFramework to net10.0
   - Update SDK version to 10.0.x
   - Update package versions for .NET 10 compatibility

2. **Package Updates**
   - Update AspNetCore-related packages to 10.0.x
   - Update System.* packages for .NET 10
   - Update Entity Framework Core packages
   - Upgrade deprecated packages

3. **Validation & Fixes**
   - Build solution
   - Fix any compilation errors
   - Run tests

## Affected Projects

- ApplicationCore (ClassLibrary)
- BlazorAdmin (AspNetCore)
- BlazorShared (ClassLibrary)
- Infrastructure (ClassLibrary)
- PublicApi (AspNetCore)
- Web (AspNetCore)
- FunctionalTests (ClassLibrary)
- IntegrationTests (ClassLibrary)
- PublicApiIntegrationTests (ClassLibrary)
- UnitTests (ClassLibrary)

## Task Breakdown

### Phase 1: Central Configuration (1 task)
- **01-update-central-config**: Update Directory.Packages.props and global.json

### Phase 2: Build Validation (1 task)
- **02-build-and-fix**: Build solution and fix compilation errors

### Phase 3: Test Validation (1 task)
- **03-run-tests**: Run unit tests

## Success Criteria

✅ All projects target net10.0
✅ Solution builds without errors
✅ All tests pass
✅ No security vulnerabilities
