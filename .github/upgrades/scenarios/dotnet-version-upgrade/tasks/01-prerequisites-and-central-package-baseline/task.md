# 01-prerequisites-and-central-package-baseline

## Objective
Prepare SDK and central package versions for the .NET 10.0 upgrade.

## Research Findings

### Current State (before changes)
- `global.json`: SDK version `8.0.x` with `latestFeature` rollforward
- `Directory.Packages.props`: TargetFramework `net8.0`, AspNetVersion `8.0.2`, EntityFrameworkCore `8.0.2`, etc.
- Installed .NET 10 SDKs: 10.0.109, 10.0.204, 10.0.301, 10.0.302 → using **10.0.302** (latest)

### Issues Found
1. `ApplicationCore.csproj` has `PackageReference` for `System.Security.Claims` — must be removed from csproj since we removed from central packages
2. `PublicApi.csproj` has `PackageReference` for `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — must be removed from csproj since we removed from central packages
3. `System.IdentityModel.Tokens.Jwt` pinned at 7.3.1 but `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.10` transitively requires `>= 8.19.2` → updated to `8.19.2`

## Changes Made

### global.json
- SDK version: `8.0.x` → `10.0.302`

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.10`
- `SystemExtensionVersion`: `8.0.0` → `10.0.10`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.10`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0`
- `System.Text.Json`: `8.0.3` → `10.0.10`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.19.2` (transitive conflict fix)
- **Removed**: `System.Security.Claims` (included in framework)
- **Removed**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, dev-only)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` (package removed from central)

### src/PublicApi/PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` (package removed from central)

## Restore Status
`dotnet restore eShopOnWeb.sln` — **SUCCESS** (all 10 projects restored, warnings only)
