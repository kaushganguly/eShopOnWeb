# Progress Details: 01-toolchain-and-central-package-baseline

## Summary
Verified .NET 10 SDK availability, updated SDK pin in `global.json`, and updated all central version variables and package pins in `Directory.Packages.props` for net10.0. Solution restores cleanly.

## Files Modified

### global.json
- `sdk.version`: `8.0.x` → `10.0.100`  
  `rollForward` remains `latestFeature` — resolves to SDK `10.0.301` (highest available).

### Directory.Packages.props

**Version variables (PropertyGroup):**
| Property | Old | New |
|---|---|---|
| `TargetFramework` | `net8.0` | `net10.0` |
| `AspNetVersion` | `8.0.2` | `10.0.9` |
| `SystemExtensionVersion` | `8.0.0` | `10.0.9` |
| `EntityFramworkCoreVersion` | `8.0.2` | `10.0.9` |
| `VSCodeGeneratorVersion` | `8.0.0` | `10.0.2` |

**Pinned package versions (ItemGroup):**
| Package | Old | New | Rationale |
|---|---|---|---|
| `Azure.Identity` | `1.10.4` | `1.21.0` | Security vulnerability fix |
| `System.Text.Json` | `8.0.3` | `10.0.9` | Align with .NET 10 runtime |
| `System.IdentityModel.Tokens.Jwt` | `7.3.1` | `8.0.1` | Fix NU1605 downgrade error — `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9` transitively requires `>= 8.0.1` |

## Restore Validation

```
dotnet restore eShopOnWeb.sln
```

**Result: ✅ Exit code 0 — all 11 projects restored.**

Remaining warnings (non-blocking, pre-existing):
- `NU1903` AutoMapper 12.0.1 — known high severity vulnerability; deferred per upgrade strategy
- `NU1901` NuGet.Packaging / NuGet.Protocol — low severity; pre-existing
- `NU1510` System.Security.Claims, System.Text.Json, System.Net.Http.Json — now in-box on net10.0; unnecessary `PackageReference` pruning warnings; will be resolved during per-project upgrade tasks

## Notes
- `EntityFramworkCoreVersion` property has a typo (missing 'e' in Framework); preserved as-is to avoid breaking references throughout project files.
- `Microsoft.AspNetCore.Mvc 2.2.0` is hardcoded (not using `$(AspNetVersion)`) and left unchanged — it may be intentional or unused; to be reviewed in a later task.
- `BlazorInputFile 0.2.0` has no .NET 10 compatible release; deferred per strategy.
