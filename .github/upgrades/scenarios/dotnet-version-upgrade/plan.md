# .NET Version Upgrade Plan: net8.0 → net10.0

## Summary

Upgrade all 10 projects in the eShopOnWeb solution from net8.0 to net10.0 (LTS).

- **Solution**: eShopOnWeb.sln
- **Source**: net8.0
- **Target**: net10.0
- **Strategy**: Solution-wide upgrade — all projects in dependency order
- **Issues**: 119 total (21 mandatory, 84 potential, 14 optional)

## Tasks

### 01-global-json-sdk
Update global.json SDK version from 8.0.x to 10.0.x.

**Files**: global.json

### 02-central-package-versions
Update Central Package Management (Directory.Packages.props):
- TargetFramework property: net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.11
- SystemExtensionVersion: 8.0.0 → 10.0.11
- EntityFrameworkCoreVersion: 8.0.2 → 10.0.11
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- Azure.Identity: 1.10.4 → 1.21.0 (security vulnerability fix)
- System.Text.Json: 8.0.3 → 10.0.11
- Remove System.Security.Claims (now included in framework)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible)

**Files**: Directory.Packages.props

### 03-remove-incompatible-package-references
Remove package references for incompatible/removed packages from individual project files:
- Remove System.Security.Claims reference from ApplicationCore.csproj
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets references from PublicApi.csproj and Web.csproj

**Files**: src/ApplicationCore/ApplicationCore.csproj, src/PublicApi/PublicApi.csproj, src/Web/Web.csproj

### 04-fix-serialization-api
Remove obsolete ISerializable protected constructor from EmptyBasketOnCheckoutException 
(SerializationInfo/StreamingContext constructor is removed in .NET 10).

**Files**: src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs

### 05-fix-configuration-binding-api
Fix binary-incompatible ConfigurationBinder.Get<T> and Configure<T>(IConfiguration) usages 
in Program.cs and configuration classes. In .NET 10, binding directly to root IConfiguration 
was changed; use GetSection().Get<T>() or Bind() patterns.

**Files**: 
- src/Web/Configuration/ConfigureCoreServices.cs
- src/Web/Configuration/ConfigureWebServices.cs  
- src/Web/Program.cs
- src/PublicApi/Program.cs

### 06-build-and-fix
Build the solution and fix any remaining compilation errors or warnings.

**Files**: Solution-wide

### 07-run-unit-tests
Run the unit tests to verify the upgrade is successful.

**Files**: tests/UnitTests/
