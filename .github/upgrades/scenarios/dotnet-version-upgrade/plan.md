# Upgrade Plan: .NET 8 → .NET 10

## Goal
Upgrade the eShopOnWeb solution from net8.0 to net10.0 (LTS, support ends Nov 2028).

## Strategy
**In-place upgrade** — upgrade all projects simultaneously since they are tightly coupled and share `Directory.Packages.props` for central package management.

## Execution Constraints
- Flow Mode: Automatic
- Target Framework: net10.0
- Solution: eShopOnWeb.sln
- Working Branch: upgrade-to-net10

## Assessment Summary
- 10 projects to upgrade
- 119 issues (21 mandatory, 84 potential, 14 optional)
- Key mandatory changes:
  1. TFM update (net8.0 → net10.0) in Directory.Packages.props
  2. SDK version update in global.json (8.0.x → 10.0.x)  
  3. Remove `System.Security.Claims` package (now included in framework)
  4. Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no supported version)
  5. Fix deprecated `Exception(SerializationInfo, StreamingContext)` constructor
  6. Update Microsoft.* versioned packages to 10.0.11
  7. Update Azure.Identity (security vulnerability) to 1.21.0
  8. Update Microsoft.VisualStudio.Web.CodeGeneration.Design to 10.0.2

## Tasks

### 01-update-central-packages
Update `Directory.Packages.props`:
- Change `<TargetFramework>net8.0</TargetFramework>` to `net10.0`
- Change `<AspNetVersion>8.0.2</AspNetVersion>` to `10.0.11`
- Change `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` to `10.0.11`
- Change `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` to `10.0.11`
- Change `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` to `10.0.2`
- Update `Azure.Identity` from `1.10.4` to `1.21.0`
- Update `System.Text.Json` from `8.0.3` to `10.0.11`
- Remove `System.Security.Claims` entry (functionality included in framework)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` entry (incompatible)

### 02-update-sdk-version
Update `global.json`:
- Change SDK version from `8.0.x` to `10.0.x`

### 03-remove-package-refs-from-projects
Remove package references from individual project files:
- Remove `System.Security.Claims` PackageReference from ApplicationCore.csproj
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference from PublicApi.csproj

### 04-fix-source-incompatible-apis
Fix source-incompatible API usages:
- Remove deprecated `Exception(SerializationInfo, StreamingContext)` constructor from EmptyBasketOnCheckoutException.cs
  (SYSLIB0051 — removed in .NET 9/10)

### 05-build-and-fix
Restore, build solution, and fix any remaining compilation errors or warnings.

### 06-run-tests
Run all test projects and ensure they pass.
