# Progress Details: 02-update-nuget-packages

## Changes Made

### Directory.Packages.props
- `AspNetVersion`: 8.0.2 → 10.0.11
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0
- `System.Text.Json`: 8.0.3 → 10.0.11
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- Removed `System.Security.Claims` (now in framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference (now in framework)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
