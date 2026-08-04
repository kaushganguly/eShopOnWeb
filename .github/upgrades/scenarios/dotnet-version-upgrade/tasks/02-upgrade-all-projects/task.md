# 02-upgrade-all-projects: Upgrade all projects to net10.0

## Scope
All 10 projects: ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web, FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests

## Changes Required

### Directory.Packages.props (centralized)
- TargetFramework: net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.10
- SystemExtensionVersion: 8.0.0 → 10.0.10
- EntityFramworkCoreVersion: 8.0.2 → 10.0.10
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- Azure.Identity: 1.10.4 → 1.21.0 (security fix)
- System.Text.Json: 8.0.3 → 10.0.10
- System.IdentityModel.Tokens.Jwt: 7.3.1 → 8.22.0 (deprecated)
- Remove: System.Security.Claims (included in framework)
- Remove: Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible, no supported version)
- Remove: Microsoft.AspNetCore.Mvc 2.2.0 (unreferenced)

### Project file changes
- src/ApplicationCore/ApplicationCore.csproj: remove System.Security.Claims PackageReference
- src/PublicApi/PublicApi.csproj: remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets PackageReference

### Source code changes
1. EmptyBasketOnCheckoutException.cs: Remove serialization constructor (protected ctor with SerializationInfo removed in .NET 10)
2. ConfigureCookieSettings.cs: Fix TimeSpan.FromMinutes ambiguity (cast to long)
3. PublicApi/Program.cs: ConfigurationBinder.Get<T>() return type changes — may need null-check update
