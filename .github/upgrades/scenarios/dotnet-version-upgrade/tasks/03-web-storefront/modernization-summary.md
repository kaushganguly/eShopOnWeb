# Task 03-web-storefront: Modernization Summary

## Objective
Upgrade the customer-facing Web project and its dependencies to net10.0, remediate the Azure.Identity security vulnerability, and fix all mandatory API incompatibilities.

## Changes Made

### 1. Directory.Packages.props — Global version updates
- `TargetFramework`: `net8.0` → `net10.0` (global default)
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix — NuGet.0004)
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.19.2` (required by ASP.NET Core 10 transitive deps)

### 2. Target Framework upgrades (explicit overrides)
The following project files received an explicit `<TargetFramework>net10.0</TargetFramework>` entry:
- `src/Web/Web.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/BlazorShared/BlazorShared.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/PublicApi/PublicApi.csproj` (upgraded alongside Web since FunctionalTests references both and package versions require net10.0)

### 3. ApplicationCore — Framework-included package removal (NU1510 warnings)
Removed redundant NuGet references that are now built into the .NET 10.0 framework:
- Removed `System.Security.Claims` from `ApplicationCore.csproj` — now framework-included
- Removed `System.Text.Json` from `ApplicationCore.csproj` — now framework-included

### 4. ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs — Source incompatibility fix
Removed deprecated protected serialization constructor:
```csharp
// REMOVED — SYSLIB0051 in .NET 9+, error in .NET 10
protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context) : base(info, context) {}
```
`BinaryFormatter`-based deserialization is not supported in .NET 10.

### 5. tests/FunctionalTests/FunctionalTests.csproj — Obsolete tool reference removal
Removed `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` — this CLI tool format was deprecated in .NET Core 2.2+ and is incompatible with modern .NET SDKs.

## API Issues Addressed
| Issue | Rule | Location | Fix |
|-------|------|----------|-----|
| Binary incompatible configuration binding | Api.0001 | Web/Program.cs, ConfigureCoreServices.cs, ConfigureWebServices.cs | No source change needed — recompile against net10.0 sufficient |
| Source incompatible TimeSpan.FromMinutes(double) | Api.0002 | Web/Configuration/ConfigureCookieSettings.cs | No source change needed — `int` constant works with both double and int overloads |
| Source incompatible Exception serialization ctor | Api.0002 | ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs | Removed deprecated constructor |
| Azure.Identity security vulnerability | NuGet.0004 | Web.csproj | Upgraded to 1.21.0 |
| ASP.NET Core package upgrades | NuGet.0002 | Multiple projects | Upgraded all to 10.0.10 |
| EF Core package upgrades | NuGet.0002 | Multiple projects | Upgraded all to 10.0.10 |

## Test Results
- **UnitTests**: 44/44 passed ✅ (net10.0)
- **FunctionalTests**: 12/12 passed ✅ (net10.0)
- **Build warnings**: 0 in all touched projects

## Done-When Verification
- [x] Web targets net10.0
- [x] Azure.Identity vulnerability remediated (1.21.0)
- [x] Mandatory API issues fixed (serialization ctor removed, package versions updated)
- [x] UnitTests pass (44/44)
- [x] FunctionalTests pass (12/12)
