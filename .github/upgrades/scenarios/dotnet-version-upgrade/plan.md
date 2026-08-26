# Upgrade Plan: .NET 8 → .NET 10

## Objective
Upgrade all projects in eShopOnWeb.sln from net8.0 to net10.0 (LTS).

## Strategy
In-place upgrade — update all projects together since all are in the same solution and tightly coupled.

## Dependency Order
1. BlazorShared (Level 0)
2. ApplicationCore, BlazorAdmin (Level 1)
3. Infrastructure (Level 2)
4. PublicApi, Web (Level 3)
5. FunctionalTests, PublicApiIntegrationTests, UnitTests (Level 4)
6. IntegrationTests (Level 5)

## Tasks

### 01-global-json
Update global.json SDK version from 8.0.x to 10.0.x.

### 02-directory-packages
Update Directory.Packages.props:
- TargetFramework: net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.11
- SystemExtensionVersion: 8.0.0 → 10.0.11
- EntityFramworkCoreVersion: 8.0.2 → 10.0.11
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- Azure.Identity: 1.10.4 → 1.21.0 (security fix)
- System.Text.Json: 8.0.3 → 10.0.11
- Remove System.Security.Claims (included in framework)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible, no successor)

### 03-fix-project-references
- Remove System.Security.Claims PackageReference from ApplicationCore.csproj
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets PackageReference from PublicApi.csproj

### 04-fix-api-breaking-changes
Fix source-incompatible APIs:
- Remove obsolete SerializationInfo/StreamingContext constructor from EmptyBasketOnCheckoutException.cs
- Fix TimeSpan.FromMinutes() ambiguity in ConfigureCookieSettings.cs
- Fix Configure<CatalogSettings>(configuration) patterns if they cause build errors
- Fix ConfigurationBinder.Get<T>() nullable return patterns if needed

### 05-build-and-fix
Build the solution and fix any remaining compilation errors.

### 06-run-unit-tests
Run unit tests and fix any test failures.
