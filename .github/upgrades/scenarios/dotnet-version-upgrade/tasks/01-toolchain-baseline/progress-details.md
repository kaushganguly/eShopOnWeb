# Progress Details: 01-toolchain-baseline

## Summary
Established the .NET 10 toolchain baseline by updating global.json to the .NET 10 SDK and updating
compatible package versions in Directory.Packages.props. Security vulnerabilities in Azure.Identity
and System.Text.Json were resolved. Framework-specific packages (ASP.NET Core, EF Core) are pinned
at net8.0-compatible versions pending per-app TFM tasks.

## Files Changed

### `/global.json`
| Field | Before | After |
|-------|--------|-------|
| `sdk.version` | `"8.0.x"` (non-standard wildcard) | `"10.0.100"` (valid semver floor for 10.x) |
| `sdk.rollForward` | `"latestFeature"` | `"latestFeature"` (unchanged — picks up 10.0.301) |

**Effect**: The SDK floor is now 10.0.100, with rollForward to the latest 10.0.x feature band
(currently 10.0.301). This matches the installed SDK and ensures consistent behavior across CI/CD
environments that may have different 10.0.x patch levels.

### `/Directory.Packages.props`

#### Version Properties Updated
| Property | Before | After | Notes |
|----------|--------|-------|-------|
| `SystemExtensionVersion` | `8.0.0` | `10.0.9` | System.Net.Http.Json and Microsoft.Extensions.Logging.Configuration are multi-targeted (net8.0+), safe to upgrade now |
| `AspNetVersion` | `8.0.2` | `8.0.2` | **Not changed** — Microsoft.AspNetCore.* 10.0.9 only supports net10.0 (NU1202 confirmed); will change in BlazorAdmin/PublicApi/Web TFM tasks |
| `EntityFramworkCoreVersion` | `8.0.2` | `8.0.2` | **Not changed** — Microsoft.EntityFrameworkCore.* 10.0.9 only supports net10.0 (NU1202 confirmed); will change in Infrastructure/Web/PublicApi TFM tasks |
| `VSCodeGeneratorVersion` | `8.0.0` | `8.0.0` | **Not changed** — Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2 only supports net10.0 (NU1202 confirmed); will change in Web/PublicApi TFM tasks |

#### Direct Package Versions Updated
| Package | Before | After | Reason |
|---------|--------|-------|--------|
| `Azure.Identity` | `1.10.4` | `1.21.0` | Fixes GHSA-m5vv-6r4h-3vj9 (moderate) and GHSA-wvxc-855f-jvrv (moderate); multi-targeted, compatible with net8.0 |
| `System.Text.Json` | `8.0.3` | `10.0.9` | Fixes GHSA-8g4q-xg66-9fp4 (high) and GHSA-hh2w-p6rv-4g7w (high); runtime library, multi-targeted, compatible with net8.0 |

#### Added Comments to Version Properties
Inline comments in Directory.Packages.props document WHY each property is pinned and which app task
will unpin it. This prevents accidental changes and guides future task execution.

## Build Comparison

| Metric | Before | After | Change |
|--------|--------|-------|--------|
| Errors | 0 | 0 | ✅ No change |
| Warnings | 13 | 5 | ✅ -8 warnings (security vulnerabilities eliminated) |
| NU1902 (Azure.Identity) | 2 | 0 | ✅ Fixed |
| NU1903 (System.Text.Json) | 2 | 0 | ✅ Fixed |
| SYSLIB0051 | 1 | 1 | ⏳ Pre-existing; scoped to ApplicationCore app task |
| xUnit2013 | 4 | 4 | ⏳ Pre-existing; scoped to test tasks |

## Test Results

| Suite | Total | Passed | Failed | Skipped |
|-------|-------|--------|--------|---------|
| UnitTests | 44 | 44 | 0 | 0 |
| IntegrationTests | 3 | 3 | 0 | 0 |
| FunctionalTests | 12 | 12 | 0 | 0 |
| PublicApiIntegrationTests | 15 | 15 | 0 | 0 |
| **Total** | **74** | **74** | **0** | **0** |

## Investigation: Why Some Package Updates Were Skipped

During execution, all recommended package version bumps (AspNetVersion 10.0.9, EntityFramworkCoreVersion 10.0.9,
VSCodeGeneratorVersion 10.0.2) were attempted. `dotnet restore` immediately produced NU1202 errors for every
package in those groups because Microsoft ships ASP.NET Core, EF Core, and the code generation tools
as net10.0-only binaries starting from version 10.x:

```
error NU1202: Package Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9 is not compatible with net8.0
error NU1202: Package Microsoft.EntityFrameworkCore.InMemory 10.0.9 is not compatible with net8.0
error NU1202: Package Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2 is not compatible with net8.0
(+ 6 more packages)
```

The only correct fix is to update those packages when their consumer project TFMs change, which is
exactly the role of the subsequent per-app upgrade tasks.

## Deferred Package Watchlist

| Package | Current Version | Reason Deferred | Re-evaluate At |
|---------|-----------------|-----------------|----------------|
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | 1.19.6 | Incompatible with net10.0 — no 10.x release yet | PublicApi TFM task; remove if containerization tooling not needed |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | Deprecated — no maintained replacement in same package | Web TFM task; evaluate AutoMapper v13+ (direct) or inline mapping |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | Deprecated — superseded by `Microsoft.IdentityModel.JsonWebTokens` | PublicApi TFM task where JWT auth is used |
| `xunit` | 2.7.0 | Deprecated — xunit v3 replaces it with breaking changes | Test project tasks; requires xunit.abstractions removal and new test model |
| `xunit.runner.console` | 2.7.0 | Deprecated — xunit v3 runner needed for v3 | Alongside xunit upgrade in test tasks |

## What Subsequent Tasks Must Do

When each app's TFM task runs, it must update Directory.Packages.props version properties:

| Task | Property Changes Required |
|------|--------------------------|
| BlazorAdmin TFM | `AspNetVersion` → 10.0.9, `SystemExtensionVersion` already at 10.0.9 |
| PublicApi TFM | `AspNetVersion` → 10.0.9 (if not already), `EntityFramworkCoreVersion` → 10.0.9, `VSCodeGeneratorVersion` → 10.0.2 |
| Web TFM | `AspNetVersion` → 10.0.9 (if not already), `EntityFramworkCoreVersion` → 10.0.9 (if not already), `VSCodeGeneratorVersion` → 10.0.2 (if not already) |
| Test projects TFM | `AspNetVersion`-driven `Microsoft.AspNetCore.Mvc.Testing` → 10.0.9 (covered by AspNetVersion update above) |

> ⚠️ Top-Down strategy means BlazorAdmin should be first. Since all apps share the same version
> properties, the first app task to update TFM should also bump the relevant property.
> Confirm the build is clean after each property change before proceeding to the next app.
