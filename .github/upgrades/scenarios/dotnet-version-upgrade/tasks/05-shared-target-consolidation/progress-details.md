# Progress Details — 05-shared-target-consolidation

## Summary
Collapsed all multi-targeted shared projects from `net8.0;net10.0` to `net10.0` only, updated the
central configuration files to reflect the final steady state, and removed all per-project
VersionOverride entries that were only necessary during the phased upgrade.

## Files Modified

### Central configuration
| File | Change |
|------|--------|
| `Directory.Build.props` | Changed default `TargetFramework` from `net8.0` → `net10.0`; updated comment |
| `Directory.Packages.props` | `AspNetVersion` 8.0.2 → 10.0.9; `EntityFramworkCoreVersion` 8.0.2 → 10.0.9; `VSCodeGeneratorVersion` 8.0.0 → 10.0.2; removed `BlazorWasmVersion` (replaced all `$(BlazorWasmVersion)` refs with `$(AspNetVersion)`); updated comments |

### Shared library projects (multi-targeting → net10.0)
| File | Change |
|------|--------|
| `src/ApplicationCore/ApplicationCore.csproj` | Removed `<TargetFramework />` clear + `<TargetFrameworks>net8.0;net10.0</TargetFrameworks>`; added `<TargetFramework>net10.0</TargetFramework>`; removed `System.Text.Json` package reference (bundled with net10.0, NU1510) |
| `src/BlazorShared/BlazorShared.csproj` | Same multi-targeting collapse; comment removed |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed multi-targeting; removed conditional `net8.0` ItemGroup (VersionOverride 8.0.2); removed conditional `net10.0` ItemGroup — collapsed into single unconditional ItemGroup; all WASM packages now version-resolved centrally via `AspNetVersion=10.0.9` |
| `src/Infrastructure/Infrastructure.csproj` | Removed multi-targeting; removed conditional `net8.0` ItemGroup (VersionOverride 8.0.2); removed conditional `net10.0` ItemGroup (VersionOverride 10.0.9) — collapsed into single unconditional ItemGroup; EF Core packages now version-resolved centrally via `EntityFramworkCoreVersion=10.0.9` |

### App projects (VersionOverride cleanup)
| File | Change |
|------|--------|
| `src/Web/Web.csproj` | Removed all `VersionOverride="10.0.9"` and `VersionOverride="10.0.2"` (now resolved centrally); updated comments |
| `src/PublicApi/PublicApi.csproj` | Removed all `VersionOverride="10.0.9"` and `VersionOverride="10.0.2"` (now resolved centrally); updated comments |

### Test projects (VersionOverride cleanup)
| File | Change |
|------|--------|
| `tests/IntegrationTests/IntegrationTests.csproj` | Removed `VersionOverride="10.0.9"` from `Microsoft.EntityFrameworkCore.InMemory`; updated comment |
| `tests/FunctionalTests/FunctionalTests.csproj` | Removed `VersionOverride="10.0.9"` from `Microsoft.AspNetCore.Mvc.Testing` and `Microsoft.EntityFrameworkCore.InMemory`; updated comments |
| `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` | Removed `VersionOverride="10.0.9"` from `Microsoft.AspNetCore.Mvc.Testing`; updated comment |

## Build Results
- **Errors**: 0
- **Warnings**: 18 — all NU190x (pre-existing security notices for deferred/deprecated packages)
  - NU1903: `AutoMapper` 12.0.1 — high severity vulnerability (deferred, NuGet.0005)
  - NU1901: `NuGet.Packaging` 6.12.1 — low severity vulnerability (transitive, not a project dependency)
  - NU1901: `NuGet.Protocol` 6.12.1 — low severity vulnerability (transitive, not a project dependency)
- **No new warnings introduced by this task**

## Test Results
| Suite | Passed | Failed | Skipped | Total |
|-------|--------|--------|---------|-------|
| UnitTests | 44 | 0 | 0 | 44 |
| IntegrationTests | 3 | 0 | 0 | 3 |
| FunctionalTests | 12 | 0 | 0 | 12 |
| PublicApiIntegrationTests | 15 | 0 | 0 | 15 |
| **Total** | **74** | **0** | **0** | **74** |

## Additional Notes

### System.Text.Json removed from ApplicationCore
After collapsing to single net10.0 target, `System.Text.Json` was flagged as NU1510
("will not be pruned, likely unnecessary") because it is a framework-included package in net10.0.
It was removed. No code changes were needed — the framework provides it implicitly.

### BlazorWasmVersion property removed
`BlazorWasmVersion=10.0.9` was identical to the new `AspNetVersion=10.0.9`. All WASM packages
(`Microsoft.AspNetCore.Components.*`, `Microsoft.Extensions.Identity.Core`) now resolve centrally
through `AspNetVersion`. The `BlazorWasmVersion` property was removed from `Directory.Packages.props`.

### Deferred packages unchanged
The following deprecated packages remain at their current versions per the established upgrade strategy:
- `System.IdentityModel.Tokens.Jwt` 8.4.0 (NuGet.0005)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 (NuGet.0005)
- `xunit` / `xunit.runner.*` (NuGet.0005)
- `BlazorInputFile` 0.2.0 (third-party, no net10.0 release available)

### Final state: every project on net10.0
All 11 projects in the solution now build against a single `net10.0` target:
- `Directory.Build.props` default = net10.0
- All projects either rely on the default or explicitly set `<TargetFramework>net10.0</TargetFramework>`
- No conditional ItemGroups remain for multi-targeting compatibility
- No VersionOverride entries remain for non-deferred packages
