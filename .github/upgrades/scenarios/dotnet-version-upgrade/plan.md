# Upgrade Plan: .NET 8.0 → .NET 10.0

**Solution**: eShopOnWeb.sln  
**Target Framework**: net10.0 (LTS, support until November 2028)  
**Projects**: 10 total (6 production + 4 test)

---

## Selected Strategy

**All-At-Once** — All projects upgraded simultaneously in a single atomic operation.

**Rationale**: 10 projects, all on modern .NET 8.0, clear SDK-style project structure with low complexity, mechanical TFM bump with manageable breaking changes.

---

## Tasks

### 01-prerequisites: Prerequisites & SDK Verification

**Objective**: Verify .NET 10.0 SDK is available and validate global.json compatibility before proceeding with project upgrades.

**Scope**:
- Check .NET SDK installation
- Validate global.json for net10.0 compatibility
- Verify build tooling prerequisites

---

### 02-upgrade-all: Upgrade All Projects to net10.0

**Objective**: Upgrade all 10 eShopOnWeb projects from .NET 8.0 to .NET 10.0 in a single atomic operation, including NuGet package updates and API/behavior fixes.

**Scope**:
- Update all 10 project files: TargetFramework net8.0 → net10.0
- Update 26 NuGet packages to net10.0-compatible versions
- Fix 9 binary incompatible APIs
- Fix 3 source incompatible APIs
- Handle 7 incompatible packages (Defer Resolution: create stubs)
- Verify 49 behavioral changes at runtime

**Production Projects**:
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorShared/BlazorShared.csproj
- src/BlazorAdmin/BlazorAdmin.csproj
- src/Infrastructure/Infrastructure.csproj
- src/PublicApi/PublicApi.csproj
- src/Web/Web.csproj

**Test Projects**:
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj

---

### 03-validation: Final Validation

**Objective**: Perform comprehensive validation of the upgraded solution to ensure all projects successfully target net10.0, all dependencies are resolved, and all tests pass.

**Scope**:
- Full solution build validation (0 errors, 0 warnings)
- Unit test execution
- Integration test execution
- Functional test execution
- No regressions in application behavior
- Package vulnerability verification

---

## Execution Constraints

- **Strategy**: All-at-Once (no phasing, no tier-by-tier validation)
- **Unsupported Packages**: Defer Resolution (create stubs if needed)
- **Unsupported API Handling**: Fix Inline (resolve API changes directly)
- **Solution State**: May be temporarily broken during atomic operation; restored to green upon completion
- **Validation**: Full build + test suite after all changes applied

---

## Success Criteria

✅ All projects targeting net10.0  
✅ Solution builds without errors or warnings  
✅ All unit tests pass  
✅ All integration tests pass  
✅ All functional tests pass  
✅ No unresolved API references  
✅ Package security vulnerabilities addressed
