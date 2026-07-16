# Modernization Summary: 02-foundation-libs

## Task
Upgrade foundation library projects from net8.0 to net10.0.

## Projects Upgraded
- `src/BlazorShared/BlazorShared.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/Infrastructure/Infrastructure.csproj`

## Changes

### Central Package Management (`Directory.Packages.props`)
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.19.2` (transitive dependency requirement)

### Source Code Changes
- **ApplicationCore.csproj**: Removed `System.Security.Claims` (framework-included) and `System.Text.Json` (framework-included) package references
- **BlazorAdmin.csproj**: Removed `System.Net.Http.Json` (framework-included) package reference
- **EmptyBasketOnCheckoutException.cs**: Removed obsolete `protected Exception(SerializationInfo, StreamingContext)` constructor (source-incompatible in .NET 10)

## Build Results
All four projects build successfully targeting `net10.0` with 0 errors and 0 warnings.
