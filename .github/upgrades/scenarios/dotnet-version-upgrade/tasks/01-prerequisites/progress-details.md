# Task 01-prerequisites: Progress Details

## Status: Complete

## SDK Discovered
- **Installed .NET 10 SDKs**: 10.0.110, 10.0.204, 10.0.302
- **Active SDK** (used by updated global.json): `10.0.302`
- **Installed .NET 10 Runtimes**: 10.0.8, 10.0.10 (Microsoft.AspNetCore.App and Microsoft.NETCore.App)
- **Confirmed `dotnet --version`**: `10.0.302`

## Files Changed

### 1. `global.json`
- Changed `"version": "8.0.x"` → `"version": "10.0.302"`
- Retained `"rollForward": "latestFeature"` policy (accepts any newer 10.0.3xx SDK)

### 2. `Directory.Packages.props`
Version variable updates:
| Property | Old Value | New Value |
|---|---|---|
| `TargetFramework` | `net8.0` | `net10.0` |
| `AspNetVersion` | `8.0.2` | `10.0.10` |
| `SystemExtensionVersion` | `8.0.0` | `10.0.10` |
| `EntityFramworkCoreVersion` | `8.0.2` | `10.0.10` |
| `VSCodeGeneratorVersion` | `8.0.0` | `10.0.2` |

Individual package version updates:
| Package | Old Version | New Version | Reason |
|---|---|---|---|
| `Azure.Identity` | `1.10.4` | `1.21.0` | Security vulnerability fix |
| `System.Text.Json` | `8.0.3` | `10.0.10` | .NET 10 alignment |
| `System.IdentityModel.Tokens.Jwt` | `7.3.1` | `8.19.2` | Required by `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.10` → `Microsoft.IdentityModel.Protocols.OpenIdConnect 8.19.2` (transitive dependency conflict discovered during restore) |

Packages commented out:
| Package | Reason |
|---|---|
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` | Incompatible with net10.0; removed from CPM and from PublicApi.csproj |
| `Microsoft.AspNetCore.Mvc 2.2.0` | Legacy 2.x standalone package; MVC is bundled in the ASP.NET Core framework for net10.0; no project files reference this CPM entry |

### 3. `src/PublicApi/PublicApi.csproj`
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` — version no longer defined in CPM after being commented out; package is incompatible with net10.0.

## Restore Verification
- `dotnet restore eShopOnWeb.sln` completes with **no errors**
- Remaining warnings are pre-existing advisory notices for transitive dependencies (`AutoMapper`, `NuGet.Packaging`, `NuGet.Protocol`) — not introduced by this task and not blockers.

## Notes for Subsequent Tasks
- Project files still have `DockerDefaultTargetOS` and `DockerfileContext` properties (PublicApi.csproj) that reference Docker workflow — acceptable as-is since they're just properties, not package refs.
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` (version `10.0.2`) remains in CPM and is referenced by `PublicApi.csproj` — verify this package resolves correctly in subsequent build task.
- `AutoMapper` 12.0.1 has a known high-severity vulnerability — consider updating `AutoMapper.Extensions.Microsoft.DependencyInjection` to a newer version in a future task if needed.
