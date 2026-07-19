## Sub-Agent Report: task-worker

### Status
- **status**: complete

### Summary
Updated `Directory.Packages.props` to target net10.0, bumped all framework-versioned packages, and removed three incompatible packages. Removed the corresponding `<PackageReference>` entries from individual project files. Fixed a newly-surfaced CS0433 build error (ambiguous `Program` type between `PublicApi` and `Web` assemblies) by aliasing the `Web` project reference in `PublicApiIntegrationTests.csproj` and adding `extern alias WebProject;` in the one test file that uses Web view-model types.

Key changes:
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.10
- `SystemExtensionVersion`: 8.0.0 → 10.0.10
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.10
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0
- `System.Text.Json`: 8.0.3 → 10.0.10
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.19.2
- Removed `System.Security.Claims` (now part of net10.0 framework)
- Removed `Microsoft.AspNetCore.Mvc` 2.2.0 (incompatible legacy package)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0-compatible version)

### Files Modified
- `Directory.Packages.props`
- `src/ApplicationCore/ApplicationCore.csproj` — removed `System.Security.Claims` reference
- `src/PublicApi/PublicApi.csproj` — removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — added alias on Web project reference to resolve CS0433
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — added `extern alias WebProject;` for Web.ViewModels usage

### Build Status
- `dotnet restore eShopOnWeb.sln`: **succeeded** (warnings only — known vulnerability advisories)
- `dotnet build eShopOnWeb.sln --no-restore`: **succeeded** (warnings only — vulnerability advisories and obsolete API warnings)
