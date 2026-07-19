# 02-upgrade-tfm-and-packages: Update TFM and NuGet packages in Directory.Packages.props

## Objective
Update TargetFramework to net10.0 and all package versions to net10.0-compatible versions in Directory.Packages.props.
Remove incompatible packages and security-vulnerable packages.

## Scope Inventory
- **Files modified**: Directory.Packages.props, src/PublicApi/PublicApi.csproj, src/Web/Web.csproj (if needed)
- **All 10 projects** are affected via centralized Directory.Packages.props

## Version Variables to Update (Directory.Packages.props)
| Variable | Old | New |
|---|---|---|
| TargetFramework | net8.0 | net10.0 |
| AspNetVersion | 8.0.2 | 10.0.10 |
| SystemExtensionVersion | 8.0.0 | 10.0.10 |
| EntityFramworkCoreVersion | 8.0.2 | 10.0.10 |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 |

## Individual Package Version Updates (Directory.Packages.props)
| Package | Old | New |
|---|---|---|
| Azure.Identity | 1.10.4 | 1.21.0 (security vulnerability fix) |
| System.Text.Json | 8.0.3 | 10.0.10 (security + upgrade) |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.19.2 (deprecated → LTS) |

## Packages to Remove (Directory.Packages.props)
- `System.Security.Claims` 4.3.0 — functionality included in net10.0 framework reference (NuGet.0003)
- `Microsoft.AspNetCore.Mvc` 2.2.0 — incompatible legacy package; ASP.NET Core MVC types are part of framework in net10.0 (NuGet.0001)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 — no compatible version for net10.0 (NuGet.0001)

## Project File Changes
- `src/PublicApi/PublicApi.csproj`: Remove PackageReference for Microsoft.VisualStudio.Azure.Containers.Tools.Targets (if present) and Microsoft.AspNetCore.Mvc (if present)
- `src/Web/Web.csproj`: Remove PackageReference for Microsoft.VisualStudio.Azure.Containers.Tools.Targets (if present)

## Done When
- Directory.Packages.props has TargetFramework=net10.0 and all updated versions
- `dotnet restore eShopOnWeb.sln` succeeds with no NU* errors
- No references to incompatible packages remain

