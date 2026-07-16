# 02-foundation-libs: Upgrade Foundation Libraries to net10.0

## Objective

Upgrade the four foundation projects from `net8.0` to `net10.0`:
- `src/BlazorShared/BlazorShared.csproj` (Level 0)
- `src/ApplicationCore/ApplicationCore.csproj` (Level 1)
- `src/BlazorAdmin/BlazorAdmin.csproj` (Level 1)
- `src/Infrastructure/Infrastructure.csproj` (Level 2)

## Changes Made

### Directory.Packages.props (Central Package Management)
- `<TargetFramework>` updated from `net8.0` → `net10.0`
- `AspNetVersion` updated from `8.0.2` → `10.0.10`
- `SystemExtensionVersion` updated from `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion` updated from `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion` updated from `8.0.0` → `10.0.2`
- `Azure.Identity` updated from `1.10.4` → `1.21.0` (security fix)
- `System.Text.Json` updated from `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt` updated from `7.3.1` → `8.19.2` (required by JwtBearer 10.0.10 transitive dependency)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` package reference (now included in framework)
- Removed `System.Text.Json` package reference (now included in framework)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` package reference (now included in framework)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor (source incompatible API in .NET 10, removed from base Exception class)

## Build Result
All four foundation projects build successfully with 0 errors and 0 warnings targeting `net10.0`.
