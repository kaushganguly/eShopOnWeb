# Progress Details — 02-upgrade-all-projects

## Summary
All 10 projects upgraded from net8.0 to net10.0. Solution builds with 0 errors and 0 warnings. All 44 unit tests pass.

## Changes Made

### Directory.Packages.props (central configuration)
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0` (deprecated)
- `System.Text.Json`: `8.0.3` → centrally managed via `$(SystemExtensionVersion)` = `10.0.10`
- `System.Net.Http.Json`: managed via `$(SystemExtensionVersion)` = `10.0.10`
- Removed: `System.Security.Claims` (included in .NET framework reference)
- Removed: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no compatible version for net10.0)
- Removed: `Microsoft.AspNetCore.Mvc 2.2.0` (unreferenced)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated) → replaced with `AutoMapper 16.2.0` (core package includes DI extensions)
- `xunit`: `2.7.0` → `2.9.3`
- `xunit.runner.visualstudio`: `2.5.6` → `2.8.2`
- `xunit.runner.console`: `2.7.0` → `2.9.3`
- `MSTest.TestAdapter`: `3.2.2` → `3.7.3`
- `MSTest.TestFramework`: `3.2.2` → `3.7.3`
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `17.13.0`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`: Added `PrivateAssets=all` to prevent transitive vulnerability propagation
- Added: `NuGet.Packaging 6.14.3` (override patched version for NU1901 vulnerability)
- Added: `NuGet.Protocol 6.14.3` (override patched version for NU1901 vulnerability)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (included in framework)
- Removed `System.Text.Json` PackageReference (NU1510: included in framework reference)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (NU1510: included in framework reference)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible)
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper` (core package)
- Added `NuGet.Packaging` and `NuGet.Protocol` with `PrivateAssets=all` (vulnerability fix)

### src/Web/Web.csproj
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` (not used in source code)
- Added `NuGet.Packaging` and `NuGet.Protocol` with `PrivateAssets=all` (vulnerability fix)

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed `protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context)` constructor
- This protected serialization constructor was removed in .NET 10 (source incompatible API change)

### src/PublicApi/Program.cs
- Updated `AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))`
- AutoMapper 16.x changed the DI registration API (assembly scanning via configuration expression)

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<MappingProfile>`
- Both PublicApi and Web projects have a `Program` class; the ambiguity caused CS0433. Using `MappingProfile` from `Microsoft.eShopWeb.PublicApi` namespace as the discriminating type.

## Validation
- `dotnet build eShopOnWeb.sln` → **Build succeeded. 0 Warning(s). 0 Error(s)** ✅
- `dotnet test tests/UnitTests` → **44/44 tests passed** ✅
- All output DLLs target `net10.0` ✅
