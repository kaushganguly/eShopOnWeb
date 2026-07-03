# Progress Details: 02-upgrade-web

## Summary
Upgraded Web project and all dependent libraries (BlazorShared, ApplicationCore, BlazorAdmin, Infrastructure) plus UnitTests to .NET 10 compatible state.

## Files Modified

### Directory.Packages.props
- `EntityFramworkCoreVersion` updated `8.0.2` → `10.0.9`
- `SystemExtensionVersion` updated `8.0.0` → `10.0.0`
- `VSCodeGeneratorVersion` updated `8.0.0` → `10.0.2`
- `Azure.Identity` updated `1.10.4` → `1.14.2` (security fix + required by EF Core SqlServer 10.0.9 → Microsoft.Data.SqlClient 6.1.1 → Azure.Identity >= 1.14.2)
- `System.Text.Json` updated `8.0.3` → `10.0.0`
- `System.IdentityModel.Tokens.Jwt` updated `7.3.1` → `8.0.1` (required by JwtBearer 10.0.9 transitively; was causing NU1605 package downgrade error)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` — included in .NET 10 framework (NU1510 warning)
- Removed `<PackageReference Include="System.Text.Json" />` — included in .NET 10 framework (NU1510 warning)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed `protected EmptyBasketOnCheckoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)` constructor
- This constructor pattern was removed in .NET 9+ as `Exception(SerializationInfo, StreamingContext)` is no longer supported (source incompatible API change)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `<PackageReference Include="System.Net.Http.Json" />` — included in .NET 10 framework (NU1510 warning)

### src/Web/Web.csproj
- Removed `<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />` — not used by Web project (AutoMapper is only used in PublicApi)
- Removed `<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" />` — dev-time scaffolding tool only; its NuGet tooling transitive deps (NuGet.Packaging 6.12.1, NuGet.Protocol 6.12.1) had low severity vulnerabilities

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Line 22: `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (xUnit2013)
- Line 35: `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)` (xUnit2013)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Line 19: `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (xUnit2013)

## Build Results

| Project | Errors | Warnings | Status |
|---|---|---|---|
| BlazorShared | 0 | 0 | ✅ |
| ApplicationCore | 0 | 0 | ✅ |
| BlazorAdmin | 0 | 0 | ✅ |
| Infrastructure | 0 | 0 | ✅ |
| Web | 0 | 0 | ✅ |
| UnitTests | 0 | 0 | ✅ |

## Test Results

| Suite | Total | Passed | Failed | Skipped |
|---|---|---|---|---|
| UnitTests | 44 | 44 | 0 | 0 |

## Notes on Out-of-Scope Issues
- `PublicApiIntegrationTests` has a pre-existing `CS0433` error: ambiguous `Program` type (both PublicApi and Web are referenced from same test project). This is out of scope for task 02.
- `PublicApi` and `FunctionalTests` have NU1903 warnings for `AutoMapper 12.0.1` (high severity vuln). The AutoMapper.Extensions.Microsoft.DependencyInjection package and its usage in PublicApi is out of scope for task 02.
