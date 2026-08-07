# Task 01: Update SDK and Package Versions — Progress Details

## Changes Made

### global.json
- SDK version: `8.0.x` → `10.0.x` (with `latestFeature` rollForward)

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- `Microsoft.NET.Test.Sdk`: `17.9.0` → `17.13.0`
- Removed `System.Security.Claims` (included in net10.0 framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 compatible version)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (included in framework)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible)
