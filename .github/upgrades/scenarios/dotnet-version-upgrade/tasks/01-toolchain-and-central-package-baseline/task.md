# 01-toolchain-and-central-package-baseline

## Objective
Verify .NET 10 prerequisites and prepare central package management for the top-down upgrade of eShopOnWeb from net8.0 to net10.0.

## Scope
- `global.json` — SDK version pin
- `Directory.Packages.props` — central version variables and package version pins

## Research Findings

### .NET SDK
Available SDKs on machine:
- 8.0.x (8.0.128, 8.0.206, 8.0.319, 8.0.422)
- 9.0.x (9.0.118, 9.0.205, 9.0.315)
- **10.0.x (10.0.109, 10.0.204, 10.0.301)** ✅

.NET 10 SDK is present. Latest available: `10.0.301`.

### global.json
Original pin: `8.0.x` with `rollForward: latestFeature`
Action: Update to `10.0.100` with `rollForward: latestFeature` — this picks the highest 10.0.x feature band available (10.0.301).

### Directory.Packages.props — Version Variables
| Variable | Before | After |
|---|---|---|
| `TargetFramework` | `net8.0` | `net10.0` |
| `AspNetVersion` | `8.0.2` | `10.0.9` |
| `SystemExtensionVersion` | `8.0.0` | `10.0.9` |
| `EntityFramworkCoreVersion` (typo preserved) | `8.0.2` | `10.0.9` |
| `VSCodeGeneratorVersion` | `8.0.0` | `10.0.2` |

### Directory.Packages.props — Pinned Package Versions
| Package | Before | After | Reason |
|---|---|---|---|
| `Azure.Identity` | `1.10.4` | `1.21.0` | Security fix |
| `System.Text.Json` | `8.0.3` | `10.0.9` | Align with .NET 10 |
| `System.IdentityModel.Tokens.Jwt` | `7.3.1` | `8.0.1` | Transitive NU1605 downgrade conflict — `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9` → `Microsoft.IdentityModel.Protocols.OpenIdConnect 8.0.1` → requires `>= 8.0.1` |

### Restore Result
`dotnet restore eShopOnWeb.sln` — **success** (exit 0).

Warnings only (pre-existing, not blocking):
- `NU1903`: AutoMapper 12.0.1 known vulnerability — deferred per strategy
- `NU1901`: NuGet.Packaging/NuGet.Protocol known vulnerability — pre-existing
- `NU1510`: System.Security.Claims / System.Text.Json / System.Net.Http.Json unnecessary on net10.0 — to be cleaned in later per-project tasks

### Deferred Packages (not blocking)
- `BlazorInputFile 0.2.0` — no .NET 10 compatible release; deferred
- `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` — vulnerability; deferred
- `Microsoft.AspNetCore.Mvc 2.2.0` (hardcoded, legacy) — not consuming `$(AspNetVersion)`; deferred
- `Ardalis.*` packages — compatible as-is, no changes needed
- `Swashbuckle.AspNetCore` — compatible as-is

## Affected Files
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`

## Execution Steps
1. Update `global.json` SDK pin from `8.0.x` → `10.0.100` (latestFeature rollforward picks 10.0.301)
2. Update version variables in `Directory.Packages.props`
3. Update `Azure.Identity` and `System.Text.Json` hard-pinned versions
4. Fix `System.IdentityModel.Tokens.Jwt` downgrade conflict: `7.3.1` → `8.0.1`
5. Run `dotnet restore eShopOnWeb.sln` — verify exit 0
