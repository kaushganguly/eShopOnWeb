# Upgrade Plan: eShopOnWeb .NET 8 → .NET 10

## Summary

Upgrade all 10 projects in eShopOnWeb (6 source + 4 test) from `net8.0` to `net10.0` (LTS).

**Strategy**: All-at-once upgrade — central package management via `Directory.Packages.props` 
allows TFM and package versions to be updated in one place. Individual project fixes applied 
as needed.

**Assessment Findings**:
- 119 issues: 21 Mandatory, 84 Potential, 14 Optional
- Binary incompatible APIs: ConfigurationBinder.Get<T>, Configure<T>(IServiceCollection, IConfiguration) — these recompile cleanly
- Source incompatible: TimeSpan.FromMinutes(double) ambiguity with new long overload in .NET 10
- Incompatible package: Microsoft.VisualStudio.Azure.Containers.Tools.Targets (remove)
- Package included in framework: System.Security.Claims (remove)
- Security vulnerability: Azure.Identity 1.10.4 → 1.21.0
- 19 packages need version updates to 10.x-compatible releases

## Tasks

### 01-update-sdk-and-props
**Update global.json and Directory.Packages.props**
- Update global.json SDK version from `8.0.x` to `10.0.x`
- Update `TargetFramework` in Directory.Packages.props from `net8.0` to `net10.0`
- Update version variables: AspNetVersion → `10.0.10`, SystemExtensionVersion → `10.0.10`, EntityFramworkCoreVersion → `10.0.10`, VSCodeGeneratorVersion → `10.0.2`
- Update Azure.Identity from `1.10.4` to `1.21.0` (security vulnerability fix)
- Update System.Text.Json version to `10.0.10`
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets entry from Directory.Packages.props

### 02-fix-project-packages
**Fix per-project incompatible package references**
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference from `src/PublicApi/PublicApi.csproj` (incompatible with .NET 10 — no supported version)
- Remove `System.Security.Claims` PackageReference from `src/ApplicationCore/ApplicationCore.csproj` (functionality included in .NET framework reference)

### 03-fix-api-breaking-changes
**Fix API breaking changes introduced between .NET 8 and .NET 10**
- Fix `TimeSpan.FromMinutes(double)` ambiguity in `src/Web/Configuration/ConfigureCookieSettings.cs` (cast to explicit type to resolve overload ambiguity)
- Attempt build and resolve any remaining compile errors from ConfigurationBinder or other APIs

### 04-validate-build-and-tests
**Build and run tests**
- Run `dotnet build eShopOnWeb.sln` and fix all errors and warnings in modified projects
- Run `dotnet test eShopOnWeb.sln` and ensure all tests pass
