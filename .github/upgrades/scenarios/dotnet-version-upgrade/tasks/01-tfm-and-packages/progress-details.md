# Task 01 Progress Details: Update TFM and Packages

## Changes Made

### global.json
- Updated SDK version from `8.0.x` to `10.0.x`

### Directory.Packages.props
- Updated `TargetFramework` from `net8.0` to `net10.0`
- Updated `AspNetVersion` from `8.0.2` to `10.0.11`
- Updated `SystemExtensionVersion` from `8.0.0` to `10.0.11`
- Updated `EntityFramworkCoreVersion` from `8.0.2` to `10.0.11`
- Updated `VSCodeGeneratorVersion` from `8.0.0` to `10.0.2`
- Updated `Azure.Identity` from `1.10.4` to `1.21.0` (security fix)
- Updated `System.Text.Json` version to use `$(SystemExtensionVersion)` = `10.0.11`
- Updated `System.IdentityModel.Tokens.Jwt` from `7.3.1` to `8.22.0`
- Removed `System.Security.Claims` entry (now included in framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` entry (incompatible)
- Removed `Microsoft.AspNetCore.Mvc` v2.2.0 entry (not needed)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `PackageReference` for `System.Security.Claims` (included in net10.0 framework)

### src/PublicApi/PublicApi.csproj
- Removed `PackageReference` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0)
