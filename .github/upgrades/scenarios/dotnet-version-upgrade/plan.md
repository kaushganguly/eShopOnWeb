# .NET 8.0 to .NET 10.0 LTS Upgrade Plan

**Target Framework**: net10.0 (LTS, support until Nov 2028)  
**Solution**: eShopOnWeb.sln  
**Strategy**: Centralized (single target framework for all projects)  
**Status**: Generated for execution

## Overview

Upgrade the eShopOnWeb solution (10 projects) from .NET 8.0 to .NET 10.0 LTS. This includes:
- Core libraries (ApplicationCore, Infrastructure, BlazorShared)
- Web applications (Web, PublicApi, BlazorAdmin)
- Test projects (UnitTests, FunctionalTests, IntegrationTests, PublicApiIntegrationTests)

## Execution Phases

### Phase 1: Prepare Environment
- **P1-001**: Update global.json to support .NET 10.0 SDK
- **P1-002**: Validate .NET 10.0 SDK is installed

### Phase 2: Update Core Projects (Bottom-Up)
Core libraries must be updated first as they are dependencies for other projects.

- **P2-001**: Update BlazorShared target framework to net10.0
- **P2-002**: Update ApplicationCore target framework to net10.0
- **P2-003**: Update Infrastructure target framework to net10.0

### Phase 3: Update Application Projects
Application projects depend on core libraries.

- **P3-001**: Update BlazorAdmin target framework to net10.0
- **P3-002**: Update PublicApi target framework to net10.0
- **P3-003**: Update Web target framework to net10.0

### Phase 4: Update Test Projects
Test projects can be updated after their dependencies.

- **P4-001**: Update UnitTests target framework to net10.0
- **P4-002**: Update FunctionalTests target framework to net10.0
- **P4-003**: Update IntegrationTests target framework to net10.0
- **P4-004**: Update PublicApiIntegrationTests target framework to net10.0

### Phase 5: Upgrade NuGet Packages
Update all NuGet packages to versions compatible with .NET 10.0.

- **P5-001**: Update deprecated and incompatible packages across all projects
- **P5-002**: Fix API compatibility issues (binary, source, behavioral)
- **P5-003**: Address security vulnerabilities

### Phase 6: Build & Test Validation
Verify the upgrade is complete and successful.

- **P6-001**: Clean build of entire solution
- **P6-002**: Run unit tests
- **P6-003**: Run integration tests
- **P6-004**: Final verification and commit

## Dependencies

```
BlazorShared (leaf)
ApplicationCore (leaf)
├── Infrastructure
├── BlazorAdmin (depends on BlazorShared)
├── PublicApi
├── Web
└── Test Projects (depend on various core libraries)
```

## Success Criteria

✅ All projects target net10.0  
✅ Solution builds without errors  
✅ All unit tests pass  
✅ All integration tests pass  
✅ No deprecated package warnings  
✅ No security vulnerabilities  
✅ Changes committed to working branch  

## Known Issues to Address

1. **Binary incompatibilities** (Web, PublicApi) - Requires API compatibility reviews
2. **Source incompatibilities** (ApplicationCore, Web) - Code changes needed
3. **Behavioral changes** (Multiple projects) - Testing required
4. **Package vulnerabilities** (ApplicationCore, Web) - Package updates needed
5. **Deprecated packages** (Multiple projects) - Replacement needed

