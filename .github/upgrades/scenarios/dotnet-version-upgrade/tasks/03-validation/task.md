# Final Validation

## Objective
Perform comprehensive validation of the upgraded solution to ensure all projects successfully target net10.0, all dependencies are resolved, and all tests pass.

## Scope
- Full solution build validation
- Unit test execution
- Integration test execution
- Functional test execution
- No regressions in application behavior
- Package vulnerability status

## Steps

### Step 1: Clean Build
1. Run: `dotnet clean eShopOnWeb.sln` to remove all build artifacts
2. Run: `dotnet build eShopOnWeb.sln` to perform a fresh build from scratch
3. Verify:
   - Build succeeds with **0 errors**
   - Build completes with **0 warnings**
   - All projects compile successfully
4. Document: Build output summary, any warnings (must be 0)

### Step 2: Unit Tests
1. Run: `dotnet test tests/UnitTests/UnitTests.csproj --verbosity=normal`
2. Verify:
   - All unit tests pass
   - No test failures or skipped tests (unless intentional)
   - Test execution completes successfully
3. Document: Test count, pass/fail summary

### Step 3: Integration Tests
1. Run: `dotnet test tests/IntegrationTests/IntegrationTests.csproj --verbosity=normal`
2. Run: `dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj --verbosity=normal`
3. Verify:
   - All integration tests pass
   - Database/service integrations work correctly
   - API integration tests verify compatibility
4. Document: Test count per project, pass/fail summary

### Step 4: Functional Tests
1. Run: `dotnet test tests/FunctionalTests/FunctionalTests.csproj --verbosity=normal`
2. Verify:
   - All functional tests pass
   - End-to-end scenarios work
   - No regressions in application behavior
3. Document: Test count, pass/fail summary

### Step 5: Framework Verification
1. Verify all project files:
   - All 10 projects have `<TargetFramework>net10.0</TargetFramework>`
   - No projects remain on net8.0
2. Run: `dotnet --version` to confirm runtime version availability
3. Verify global.json allows net10.0 SDK

### Step 6: Dependency Health Check
1. Run: `dotnet package search --take 0` to verify package sources
2. Run: `dotnet list package` to audit current package versions
3. Verify:
   - No deprecated packages remain
   - All known security vulnerabilities addressed
   - Package versions are compatible with net10.0

### Step 7: Documentation Review
1. Review any project-specific documentation
2. Verify README.md updated if needed (target framework refs)
3. Document any breaking changes visible to consumers

## Validation Checklist

- ✅ `dotnet clean` succeeds
- ✅ `dotnet build eShopOnWeb.sln` produces 0 errors, 0 warnings
- ✅ All 10 projects target net10.0
- ✅ UnitTests: 100% pass rate
- ✅ IntegrationTests: 100% pass rate
- ✅ PublicApiIntegrationTests: 100% pass rate
- ✅ FunctionalTests: 100% pass rate
- ✅ No regressions in application behavior
- ✅ No deprecated packages
- ✅ Known security vulnerabilities addressed
- ✅ Documentation updated

## Success Criteria

✅ **Build**: 0 errors, 0 warnings  
✅ **Unit Tests**: All pass  
✅ **Integration Tests**: All pass  
✅ **Functional Tests**: All pass  
✅ **Framework Target**: All 10 projects on net10.0  
✅ **Dependencies**: All packages compatible and secure  
✅ **Behavior**: No regressions or breaking changes  

## Failure Handling

If any validation step fails:
1. Document the failure (error output, test results)
2. Return to task 02-upgrade-all and fix the root cause
3. Re-run validation from Step 1

## Completion Report

When all validations pass, generate a summary:
- Upgrade start time / end time
- Total projects upgraded: 10
- Total packages updated: 26
- Build status: ✅ PASS
- Test status: ✅ PASS
- Framework: net8.0 → net10.0 (100% coverage)
- Recommendation: Upgrade complete, ready for deployment
