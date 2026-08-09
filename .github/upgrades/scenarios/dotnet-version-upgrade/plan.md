# Upgrade Plan: .NET 8 → .NET 10

**Solution**: eShopOnWeb.sln  
**Source**: net8.0  
**Target**: net10.0  
**Strategy**: In-Place (all projects upgraded together)

## Summary

Upgrade all 10 projects in the eShopOnWeb solution from net8.0 to net10.0.

Key changes:
- Update global.json SDK version to 10.0.x
- Update Directory.Packages.props: TFM + package versions + remove incompatible packages
- Remove incompatible packages: `System.Security.Claims` (built into framework), `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10 support)
- Fix API binary-incompatible calls: ConfigurationBinder.Get<T> / Configure<T> usages
- Build and test validation

## Tasks

### 01-global-json — Update global.json SDK version
Update the SDK version in global.json from 8.0.x to 10.0.x

### 02-central-packages — Update Directory.Packages.props
- Change TargetFramework from net8.0 to net10.0
- Bump AspNetVersion to 10.0.10
- Bump SystemExtensionVersion to 10.0.10
- Bump EntityFramworkCoreVersion to 10.0.10
- Bump VSCodeGeneratorVersion to 10.0.2
- Bump Azure.Identity to 1.21.0 (security fix)
- Bump System.Text.Json to 10.0.10
- Remove System.Security.Claims (included in .NET 10 framework)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible)

### 03-remove-packages — Remove package references from project files
- Remove `System.Security.Claims` from ApplicationCore.csproj
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from PublicApi.csproj

### 04-fix-api-changes — Fix API breaking changes
Fix ConfigurationBinder.Get<T> and Configure<T> usages that are flagged as binary incompatible:
- src/Web/Configuration/ConfigureWebServices.cs
- src/Web/Configuration/ConfigureCoreServices.cs
- src/Web/Program.cs
- src/PublicApi/Program.cs

### 05-build-validate — Build and validate
Build the solution and fix any remaining compilation errors.

### 06-test-validate — Run unit tests
Run unit tests and fix any failures.
