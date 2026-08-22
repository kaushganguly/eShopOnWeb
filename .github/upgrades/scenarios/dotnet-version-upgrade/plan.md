# .NET Version Upgrade Plan
## eShopOnWeb: net8.0 → net10.0

**Strategy**: In-place upgrade (single-phase)  
**Target**: net10.0 (LTS)

---

## Tasks

- ✅ 01-global-json: Update global.json SDK version
- ✅ 02-directory-packages: Update Directory.Packages.props (TargetFramework + all package versions)
- ✅ 03-fix-functional-tests: Remove deprecated DotNetCliToolReference from FunctionalTests.csproj
- ✅ 04-build-fix: Build solution and fix any compilation errors
- ✅ 05-test-run: Run unit tests to validate upgrade
