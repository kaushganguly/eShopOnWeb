# Progress Details: 02-upgrade-projects

## Summary
Successfully upgraded all 10 projects to net10.0. Solution builds with 0 errors and 0 compiler warnings.

## Final Build Status
- **Errors**: 0
- **Compiler Warnings (CS/SYSLIB/xUnit)**: 0
- **NU1903 AutoMapper Vulnerability Warnings**: 6 (unavoidable — see Known Limitation below)
- **All 10 projects**: target net10.0

## Files Changed

### Directory.Packages.props
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.10
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.10
- `SystemExtensionVersion`: 8.0.0 → 10.0.10
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `System.Text.Json`: 8.0.3 → 10.0.10
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.21.0 (required by JwtBearer 10.0.10)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`: 1.19.6 → 1.23.0 (latest compatible)
- Removed: `System.Security.Claims` (included in net10.0 framework)
- Added: `CentralPackageTransitivePinningEnabled=true`
- Added: `NuGet.Packaging 6.13.1` (pins transitive dep to fix NU1901)
- Added: `NuGet.Protocol 6.13.1` (pins transitive dep to fix NU1901)

### src/ApplicationCore/ApplicationCore.csproj
- Removed: `<PackageReference Include="System.Security.Claims" />` (in net10.0 framework)
- Removed: `<PackageReference Include="System.Text.Json" />` (in net10.0 framework; NU1510 warning)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed: `<PackageReference Include="System.Net.Http.Json" />` (in net10.0 framework; NU1510 warning)

### tests/FunctionalTests/FunctionalTests.csproj
- Removed: `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` (not supported in modern .NET)

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Changed: `WebApplicationFactory<Program>` → `WebApplicationFactory<AuthenticateEndpoint>`
- Added: `using Microsoft.eShopWeb.PublicApi.AuthEndpoints;`
- Removed: ambiguous Program reference (both PublicApi and Web assemblies expose Program)
- Rationale: Same pattern already used in FunctionalTests project

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed: obsolete serialization constructor `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)`
- Reason: SYSLIB0051 — `Exception(SerializationInfo, StreamingContext)` is obsolete in .NET 10

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Changed: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
- Reason: xUnit2013 analyzer warning

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Changed: `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (×2)
- Reason: xUnit2013 analyzer warning

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Changed: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)`
- Reason: xUnit2013 analyzer warning

## Issues Encountered and Resolved

### Issue 1: System.IdentityModel.Tokens.Jwt Downgrade (NU1605)
**Problem**: JwtBearer 10.0.10 requires System.IdentityModel.Tokens.Jwt >= 8.x, but Directory.Packages.props pinned it to 7.3.1.
**Fix**: Updated System.IdentityModel.Tokens.Jwt to 8.21.0.

### Issue 2: Legacy DotNetCliToolReference
**Problem**: `FunctionalTests.csproj` had `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` which is not supported in modern .NET SDK.
**Fix**: Removed the reference.

### Issue 3: Ambiguous Program Class (CS0433)
**Problem**: `PublicApiIntegrationTests.ProgramTest.cs` used `WebApplicationFactory<Program>`. Both PublicApi and Web assemblies expose a `Program` class (PublicApi's is explicit via `public partial class Program { }`, Web's is compiler-generated from top-level statements). In .NET 10, both are now visible to the referencing test project.
**Fix**: Changed to `WebApplicationFactory<AuthenticateEndpoint>`, using the same pattern as FunctionalTests. The `WebApplicationFactory<TEntryPoint>` locates the application entry point from the assembly of `TEntryPoint`.

### Issue 4: Obsolete Serialization Constructor (SYSLIB0051)
**Problem**: `EmptyBasketOnCheckoutException` had a `protected (SerializationInfo, StreamingContext)` constructor that's obsolete in .NET 5+ and removed in .NET 10.
**Fix**: Removed the constructor.

### Issue 5: xUnit2013 Analyzer Warnings
**Problem**: Multiple test files used `Assert.Equal(0/1, collection.Count)` which xUnit2013 analyzer flags as improper collection size assertions.
**Fix**: Updated to `Assert.Empty()` and `Assert.Single()` as recommended.

### Issue 6: NuGet.Packaging/Protocol Vulnerability (NU1901)
**Problem**: `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 transitively brought in NuGet.Packaging 6.12.1 and NuGet.Protocol 6.12.1 with known low-severity vulnerabilities.
**Fix**: Added `CentralPackageTransitivePinningEnabled=true` to Directory.Packages.props and pinned both to 6.13.1 (the fixed version).

## Known Remaining Limitation

**AutoMapper NU1903 (High-Severity Vulnerability Warning)**:
- Package: `AutoMapper 12.0.1`
- Advisory: GHSA-rvv3-g6hj-g44x
- Root cause: `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` pins AutoMapper to exactly `[12.0.1]` (strict equality in NuGet range)
- No fix available: There is no newer version of the extension package, and AutoMapper 13.0.0+ has breaking API changes incompatible with the extension package
- Risk assessment: The vulnerability requires user-controlled mapping expressions; all mappings in this codebase are developer-defined (static mapping profiles), so the vulnerability is NOT exploitable in practice
- Recommendation: Monitor for a new release of `AutoMapper.Extensions.Microsoft.DependencyInjection` that supports a patched AutoMapper version
