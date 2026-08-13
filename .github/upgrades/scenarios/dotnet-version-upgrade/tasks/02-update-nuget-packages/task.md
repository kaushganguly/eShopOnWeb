# 02-update-nuget-packages: Update NuGet packages to .NET 10 compatible versions

## Objective
Update version variables and individual package versions in Directory.Packages.props, and remove incompatible packages.

## Changes Required

### Directory.Packages.props version variables
- `AspNetVersion`: 8.0.2 → 10.0.11
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2

### Individual package versions
- `Azure.Identity`: 1.10.4 → 1.21.0
- `System.Text.Json`: 8.0.3 → 10.0.11
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0

### Packages to remove
- `System.Security.Claims` 4.3.0 (now in framework reference)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 (incompatible, no supported version)

### Project files to update
- `src/PublicApi/PublicApi.csproj`: remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
