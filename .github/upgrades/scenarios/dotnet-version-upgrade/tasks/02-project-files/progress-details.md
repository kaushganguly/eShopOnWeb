# Progress Details: 02-project-files

## Changes Made

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` (now included in framework)

### src/PublicApi/PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` (no net10-compatible version)
