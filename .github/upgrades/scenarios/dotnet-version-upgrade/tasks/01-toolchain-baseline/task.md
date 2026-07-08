# 01-toolchain-baseline: Align SDK and centralized package management for net10.0

## Objective
Establish the shared baseline before any application TFM is changed. Update global.json for .NET 10 SDK, and update Directory.Packages.props with security-fixed and compatible package versions. TargetFramework remains net8.0 — per-app TFM changes happen in subsequent tasks.

## Scope
- `/global.json` — SDK version alignment to .NET 10
- `/Directory.Packages.props` — centralized package version updates

## Research Findings

### .NET SDK Availability
| SDK | Version | Status |
|-----|---------|--------|
| Latest installed | 10.0.301 | ✅ Available |
| Previous | 9.0.315, 9.0.205, 9.0.118 | Available |
| Current pinned | 8.0.422 (from global.json 8.0.x) | Active (but non-standard wildcard) |

Note: `dotnet --version` in repo root returns 10.0.301 despite global.json pinning 8.0.x, because "8.0.x" is not a valid semver for global.json — the rollForward policy escalates it to the latest installed SDK.

### Directory.Build.props
No `Directory.Build.props` file exists. TargetFramework and version properties are defined directly in `Directory.Packages.props`.

### Baseline Build State (pre-change)
- 0 errors, 13 warnings
- NU1902: Azure.Identity 1.10.4 — moderate vulnerability (2 advisories)
- NU1903: System.Text.Json 8.0.3 — high vulnerability (2 advisories)  
- SYSLIB0051: EmptyBasketOnCheckoutException.cs obsolete serialization ctor (scoped to app tasks)
- xUnit2013: 4 test assertion style warnings (scoped to test tasks)

### Package Update Plan

#### Safe to Update Now (net8.0 compatible at new version)
These packages are multi-targeted or framework-agnostic:
| Package | Current | New | Reason |
|---------|---------|-----|--------|
| Azure.Identity | 1.10.4 | 1.21.0 | Security fix (GHSA-m5vv, GHSA-wvxc) |
| System.Text.Json | 8.0.3 | 10.0.9 | Security fix (GHSA-8g4q, GHSA-hh2w) |
| System.Net.Http.Json | 8.0.0 (via SystemExtensionVersion) | 10.0.9 | Runtime package, multi-targeted |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.9 | Extensions package |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 | Code gen tool |

#### Requires TFM Change (net10.0 only at new version)
These must stay pinned until the owning app task changes TFM:
| Property/Package | Current | Target | Gated By |
|---------|---------|--------|----------|
| AspNetVersion (Microsoft.AspNetCore.*) | 8.0.2 | 10.0.9 | BlazorAdmin, PublicApi, Web TFM tasks |
| EntityFramworkCoreVersion (Microsoft.EntityFrameworkCore.*) | 8.0.2 | 10.0.9 | Infrastructure, PublicApi, Web TFM tasks |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | 10.0.9 | Test projects TFM tasks |

#### Deferred Packages Watchlist
| Package | Current | Status | Condition to Re-Evaluate |
|---------|---------|--------|--------------------------|
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | Incompatible — no net10.0 assets | Re-evaluate when PublicApi task runs; remove if not needed or update when compatible version ships |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Deprecated — no recommended replacement | Re-evaluate at Web task; consider direct AutoMapper v13+ or remove |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | Deprecated — superseded by Microsoft.IdentityModel.JsonWebTokens | Re-evaluate at PublicApi task where JWT is used |
| xunit | 2.7.0 | Deprecated — xunit v3 is the replacement | Re-evaluate at test tasks; xunit v3 has breaking changes requiring test project updates |
| xunit.runner.console | 2.7.0 | Deprecated — xunit v3 runner needed for v3 | Re-evaluate alongside xunit upgrade |

## Files Changed
- `/global.json` — SDK version updated from 8.0.x/latestFeature to 10.0.100/latestFeature
- `/Directory.Packages.props` — Azure.Identity and System.Text.Json updated; SystemExtensionVersion bumped for System.Net.Http.Json; VSCodeGeneratorVersion bumped; AspNetVersion and EntityFramworkCoreVersion remain at 8.0.x pending TFM tasks
