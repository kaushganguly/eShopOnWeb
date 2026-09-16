# .NET 8.0 to .NET 10.0 Upgrade Plan

**Solution**: eShopOnWeb  
**Target Framework**: net10.0 (LTS, Support until 2028-11-14)  
**Current Framework**: net8.0  
**Assessment**: 119 total issues (21 Mandatory, 84 Potential, 14 Optional)

---

## Upgrade Strategy

### Approach: Dependency-Ordered Project Upgrade
Projects will be upgraded in topological dependency order to ensure that consumers can resolve updated dependencies cleanly.

### Phases

**Phase 1: Foundation Layer (No Dependencies)**
- BlazorShared: Lightweight Blazor component library

**Phase 2: Core Libraries**
- ApplicationCore: Domain models and business logic
- Infrastructure: Data access and external services

**Phase 3: Consumers**
- BlazorAdmin: Blazor Server-side application
- Web: ASP.NET Core MVC web application
- PublicApi: ASP.NET Core REST API

**Phase 4: Test Projects**
- UnitTests: All unit tests
- PublicApiIntegrationTests: API integration tests
- FunctionalTests: Functional UI tests
- IntegrationTests: Database/service integration tests

---

## Key Issues Summary

### Mandatory Fixes (Must Complete)
- **10x** Update target framework (net8.0 → net10.0) across all projects
- **9x** Binary incompatible APIs in Web.csproj and PublicApi.csproj
- **3x** Source incompatible APIs in Web.csproj and ApplicationCore.csproj
- **1x** Remove framework-included package (NuGet.0003)
- **1x** Replace incompatible package (NuGet.0001)

### Recommended (For Quality)
- **32x** Update NuGet packages to net10.0-compatible versions
- **49x** Address behavioral changes in .NET 10.0 APIs

### Optional (Best Practice)
- **12x** Remove deprecated packages (NuGet.0005)
- **2x** Fix security vulnerabilities (NuGet.0004)

## Task Breakdown

### Main Task
**001-upgrade-dotnet-to-net10**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0

### Subtasks (in Dependency Order)

#### 001.01-blazor-shared: Upgrade BlazorShared to net10.0
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Validate: Build and package
- Estimate: 15 min

#### 001.02-app-core: Upgrade ApplicationCore to net10.0
- Update: TargetFramework → net10.0
- Fix: Source incompatible APIs (Api.0002)
- Update: NuGet dependencies
- Estimate: 30 min

#### 001.03-infrastructure: Upgrade Infrastructure to net10.0
- Update: TargetFramework → net10.0
- Remove/Replace: Deprecated packages
- Update: NuGet dependencies
- Estimate: 25 min

#### 001.04-blazor-admin: Upgrade BlazorAdmin to net10.0
- Update: TargetFramework → net10.0
- Fix: Behavioral changes (Api.0003)
- Update: Blazor packages
- Estimate: 25 min

#### 001.05-web-app: Upgrade Web to net10.0
- Update: TargetFramework → net10.0
- Fix: Binary incompatible APIs (Api.0001)
- Fix: Source incompatible APIs (Api.0002)
- Fix: Behavioral changes (Api.0003)
- Remove: Framework-included packages (NuGet.0003)
- Fix: Security vulnerabilities (NuGet.0004)
- Remove: Deprecated packages (NuGet.0005)
- Estimate: 45 min

#### 001.06-public-api: Upgrade PublicApi to net10.0
- Update: TargetFramework → net10.0
- Replace: Incompatible package (NuGet.0001)
- Fix: Binary incompatible APIs (Api.0001)
- Fix: Behavioral changes (Api.0003)
- Remove: Deprecated packages (NuGet.0005)
- Estimate: 40 min

#### 001.07-unit-tests: Upgrade UnitTests to net10.0
- Update: TargetFramework → net10.0
- Update: Test framework packages
- Remove: Deprecated packages
- Run: Tests
- Estimate: 20 min

#### 001.08-public-api-integration-tests: Upgrade PublicApiIntegrationTests to net10.0
- Update: TargetFramework → net10.0
- Fix: Behavioral changes if any
- Update: Test dependencies
- Run: Tests
- Estimate: 20 min

#### 001.09-functional-tests: Upgrade FunctionalTests to net10.0
- Update: TargetFramework → net10.0
- Update: Selenium packages
- Remove: Deprecated packages
- Run: Tests
- Estimate: 20 min

#### 001.10-integration-tests: Upgrade IntegrationTests to net10.0
- Update: TargetFramework → net10.0
- Update: Test dependencies
- Run: Tests
- Estimate: 20 min

#### 001.11-full-solution-build: Full solution build and validation
- Build: All projects (zero warnings)
- Run: All unit tests
- Run: All integration tests
- Verify: No security vulnerabilities
- Estimate: 30 min

---


### Foundation Layer

**01-blazor-shared**: Upgrade BlazorShared (No API compatibility issues expected)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Validate: Build and package
- Estimate: 15 min

### Core Libraries

**02-app-core**: Upgrade ApplicationCore (2x API compatibility issues)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Fix: Source incompatible APIs (Api.0002)
- Validate: Build, no warnings
- Estimate: 30 min

**03-infrastructure**: Upgrade Infrastructure (NuGet updates, deprecated packages)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Remove/Replace: Deprecated packages
- Validate: Build, no warnings
- Estimate: 25 min

### Web Tier

**04-blazor-admin**: Upgrade BlazorAdmin (Behavioral changes expected)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies (Blazor WebAssembly)
- Fix: Behavioral changes (Api.0003)
- Validate: Build, no warnings
- Estimate: 25 min

**05-web-app**: Upgrade Web (Multiple API compatibility issues)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Fix: Binary incompatible APIs (Api.0001)
- Fix: Source incompatible APIs (Api.0002)
- Fix: Behavioral changes (Api.0003)
- Remove: Framework-included packages
- Validate: Build, no warnings
- Estimate: 45 min

**06-public-api**: Upgrade PublicApi (Multiple API compatibility issues)
- Update: TargetFramework → net10.0
- Update: NuGet dependencies
- Fix: Binary incompatible APIs (Api.0001)
- Fix: Behavioral changes (Api.0003)
- Replace: Incompatible packages (NuGet.0001)
- Validate: Build, no warnings
- Estimate: 40 min

### Test Projects

**07-unit-tests**: Upgrade UnitTests
- Update: TargetFramework → net10.0
- Update: Test framework packages
- Fix: Behavioral changes if any
- Validate: Build, no warnings
- Estimate: 20 min

**08-public-api-integration-tests**: Upgrade PublicApiIntegrationTests
- Update: TargetFramework → net10.0
- Update: Test dependencies
- Fix: Behavioral changes
- Validate: Build, no warnings
- Estimate: 20 min

**09-functional-tests**: Upgrade FunctionalTests
- Update: TargetFramework → net10.0
- Update: Test dependencies
- Fix: Behavioral changes
- Validate: Build, no warnings
- Estimate: 20 min

**10-integration-tests**: Upgrade IntegrationTests
- Update: TargetFramework → net10.0
- Update: Test dependencies
- Validate: Build, no warnings
- Estimate: 20 min

### Final Validation

**11-full-solution-build**: Full solution build and all tests
- Build all projects warning-free
- Run all unit tests
- Run all integration tests
- Verify no security vulnerabilities
- Estimate: 30 min

---

## Success Criteria

✅ All 10 projects target net10.0  
✅ All mandatory API compatibility issues resolved  
✅ Zero build warnings  
✅ All unit tests pass  
✅ All integration tests pass  
✅ No security vulnerabilities  

---

## Risk Mitigation

- **Behavioral Changes**: Carefully review Api.0003 changes in affected projects
- **API Breaking Changes**: Use compiler diagnostics to identify all breaking changes
- **Package Compatibility**: Verify all NuGet packages support net10.0
- **Test Coverage**: Run full test suite after each phase

---

*Plan generated from assessment of 10 projects with 119 identified issues*
