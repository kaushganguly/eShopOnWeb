# Upgrade Plan: eShopOnWeb .NET 8 → .NET 10

**Target Framework**: net10.0 (LTS, support ends Nov 2028)
**Strategy**: Single-pass direct upgrade — all 10 projects upgraded together
**Build tool**: `dotnet build` (all SDK-style projects targeting .NET only)

---

## 01-update-sdk-and-packages: Update SDK version and package references

### Objective
Update global.json and Directory.Packages.props to target .NET 10 and use compatible package versions.

### Scope
- `/global.json`
- `/Directory.Packages.props`

### Steps
1. Update `global.json`: SDK version `8.0.x` → `10.0.x`
2. Update `Directory.Packages.props`:
   - `<TargetFramework>` from `net8.0` → `net10.0`
   - `<AspNetVersion>` from `8.0.2` → `10.0.11`
   - `<SystemExtensionVersion>` from `8.0.0` → `10.0.11`
   - `<EntityFramworkCoreVersion>` from `8.0.2` → `10.0.11`
   - `<VSCodeGeneratorVersion>` from `8.0.0` → `10.0.2`
   - `Azure.Identity` from `1.10.4` → `1.21.0`
   - `System.Text.Json` from `8.0.3` → `10.0.11`
   - `System.IdentityModel.Tokens.Jwt` from `7.3.1` → `8.22.0`
   - Remove `System.Security.Claims 4.3.0` (built into net10.0 framework)
   - Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` (incompatible)

---

## 02-fix-source-incompatible-apis: Fix source-incompatible API breaking changes

### Objective
Fix source-incompatible APIs that were removed or changed between .NET 8 and .NET 10.

### Scope
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

### Steps
1. Remove the obsolete `Exception(SerializationInfo, StreamingContext)` constructor override
   from `EmptyBasketOnCheckoutException`. This constructor was removed in .NET 9+.
2. Remove associated `using System.Runtime.Serialization` if no longer needed.

---

## 03-build-and-fix: Build solution and fix compilation errors, run tests

### Objective
Ensure the solution builds cleanly on net10.0 and all unit tests pass.

### Scope
- All projects in `eShopOnWeb.sln`
- `tests/UnitTests/UnitTests.csproj`

### Steps
1. Run `dotnet build eShopOnWeb.sln` to identify compilation errors
2. Fix any errors from ConfigurationBinder API changes (nullable annotations)
3. Fix any other compilation errors discovered
4. Fix all build warnings in modified projects
5. Run `dotnet test tests/UnitTests/UnitTests.csproj --no-build` after successful build
6. Fix any test failures
