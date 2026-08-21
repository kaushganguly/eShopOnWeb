# Progress Details: 001-upgrade-dotnet-to-net10

## Summary
Successfully upgraded eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0).

## Changes Made

### global.json
- Updated SDK version from `8.0.x` to `10.0.x` (with `latestFeature` rollForward)

### Directory.Packages.props
- Changed `TargetFramework` from `net8.0` to `net10.0`
- Updated all version properties:
  - `AspNetVersion`: 8.0.2 → 10.0.11
  - `SystemExtensionVersion`: 8.0.0 → 10.0.11
  - `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
  - `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- Updated NuGet packages:
  - `Ardalis.GuardClauses`: 4.0.1 → 5.0.0
  - `Ardalis.Specification`: 7.0.0 → 9.3.1
  - `Ardalis.Specification.EntityFrameworkCore`: 7.0.0 → 9.3.1
  - `Ardalis.Result`: 7.0.0 → 10.1.0
  - `Azure.Identity`: 1.10.4 → 1.21.0
  - `Azure.Extensions.AspNetCore.Configuration.Secrets`: 1.3.1 → 1.5.2
  - `FluentValidation`: 11.9.0 → 12.1.1
  - `MediatR`: 12.0.1 → 14.2.0
  - `Microsoft.Web.LibraryManager.Build`: 2.1.175 → 3.0.114
  - `NSubstitute`: 5.1.0 → 6.2.0
  - `System.Text.Json`: 8.0.3 → 10.0.11 (in-box)
  - `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
  - `Swashbuckle.AspNetCore`: 6.5.0 → 6.9.0 (kept on v6 to avoid OpenApi 2.x breaking changes)
  - `Swashbuckle.AspNetCore.SwaggerUI`: 6.5.0 → 6.9.0
  - `Swashbuckle.AspNetCore.Annotations`: 6.5.0 → 6.9.0
  - `Microsoft.NET.Test.Sdk`: 17.9.0 → 18.9.0
  - `xunit`: 2.7.0 → 2.9.3
  - `xunit.runner.visualstudio`: 2.5.6 → 3.1.5
  - `xunit.runner.console`: 2.7.0 → 2.9.3
  - `MSTest.TestAdapter`: 3.2.2 → 4.3.3
  - `MSTest.TestFramework`: 3.2.2 → 4.3.3
  - `coverlet.collector`: 6.0.2 → 10.0.1

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (now in-box in .NET 10)
- Removed `System.Text.Json` PackageReference (now in-box in .NET 10)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (now in-box in .NET 10)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProject</Aliases>` to the Web project reference to resolve ambiguous `Program` type
  (both PublicApi and Web define `partial class Program`, which is now a compile error in .NET 10)

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProject;` to use the aliased Web project
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` to `using WebProject::Microsoft.eShopWeb.Web.ViewModels;`

## Breaking Changes Encountered and Fixed

1. **Swashbuckle 10.x → 6.9.0**: Swashbuckle 10.x uses Microsoft.OpenApi 2.x which removed the `Microsoft.OpenApi.Models` namespace. Downgraded to 6.9.0 (which uses OpenApi 1.x) to avoid extensive refactoring.

2. **Ambiguous `Program` type**: .NET 10 now errors on ambiguous types that previously just warned. Both PublicApi and Web have `public partial class Program {}`. Fixed with extern alias.

3. **In-box packages**: `System.Security.Claims`, `System.Text.Json`, and `System.Net.Http.Json` are now part of the .NET 10 base class library and should not be referenced explicitly.

## Test Results
- Unit Tests: 44/44 passed ✅
- Build: Succeeded with 0 errors ✅

## Known Warnings (non-blocking)
- `NU1903`: AutoMapper 12.0.1 has a known high severity vulnerability. Upgrade to AutoMapper 13+ would require significant breaking changes beyond TFM upgrade scope.
- `NU1901`: NuGet.Packaging/NuGet.Protocol have low severity vulnerabilities (transitive, from tooling packages).
