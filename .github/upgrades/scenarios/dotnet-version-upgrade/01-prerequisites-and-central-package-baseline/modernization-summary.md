# Modernization Summary: 01-prerequisites-and-central-package-baseline

## Task
Prepare SDK and central package versions for the .NET 10.0 upgrade.

## Files Changed

| File | Change |
|------|--------|
| `global.json` | SDK version `8.0.x` → `10.0.302` |
| `Directory.Packages.props` | TargetFramework `net8.0` → `net10.0`; all package versions updated to .NET 10 equivalents; removed incompatible packages |
| `src/ApplicationCore/ApplicationCore.csproj` | Removed `<PackageReference Include="System.Security.Claims" />` |
| `src/PublicApi/PublicApi.csproj` | Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` |

## Package Version Changes

| Package | Old Version | New Version |
|---------|------------|-------------|
| AspNetVersion (variable) | 8.0.2 | 10.0.10 |
| SystemExtensionVersion (variable) | 8.0.0 | 10.0.10 |
| EntityFramworkCoreVersion (variable) | 8.0.2 | 10.0.10 |
| VSCodeGeneratorVersion (variable) | 8.0.0 | 10.0.2 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| System.Text.Json | 8.0.3 | 10.0.10 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.19.2 |
| System.Security.Claims | 4.3.0 | **REMOVED** (in-framework) |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | **REMOVED** (incompatible) |

## Build/Restore Status
`dotnet restore eShopOnWeb.sln` — **SUCCESS** (all 10 projects restored)

## Notes
- `System.IdentityModel.Tokens.Jwt` was upgraded from 7.3.1 → 8.19.2 to resolve a transitive downgrade conflict introduced by `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.10`
- Individual `.csproj` files for `ApplicationCore` and `PublicApi` were updated to remove references to packages deleted from central management
- Non-blocking warnings remain for deprecated packages (AutoMapper, xunit, MSTest) — flagged for later tasks
