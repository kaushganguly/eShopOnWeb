# Task 01-toolchain-baseline — Progress Details

## Status: Complete

## Summary
Verified .NET 10 SDK availability, updated `global.json` to select the .NET 10 SDK, updated the CI workflow to use .NET 10, and confirmed a clean baseline build of the solution (all 10 projects still targeting net8.0 at this point, as expected).

---

## Research Findings

### .NET SDKs Installed
```
8.0.129, 8.0.206, 8.0.319, 8.0.423
9.0.119, 9.0.205, 9.0.316
10.0.110, 10.0.204, 10.0.302   ← selected after update
```
**Active SDK after change**: `10.0.302` (via `latestFeature` roll-forward from `10.0.100` floor).

### Project Inventory (10 total, all SDK-style)
| Project | Type |
|---|---|
| src/ApplicationCore | SDK (library) |
| src/BlazorAdmin | SDK.Web (Blazor WASM host) |
| src/BlazorShared | SDK (library) |
| src/Infrastructure | SDK (library) |
| src/PublicApi | SDK.Web (API app) |
| src/Web | SDK.Web (MVC app) |
| tests/FunctionalTests | SDK (test) |
| tests/IntegrationTests | SDK (test) |
| tests/PublicApiIntegrationTests | SDK (test) |
| tests/UnitTests | SDK (test) |

**TFM pattern**: All projects inherit `<TargetFramework>net8.0</TargetFramework>` from `Directory.Packages.props` (central property — not in individual csproj files). This means a single-line edit in `Directory.Packages.props` will switch all projects at once when later tasks upgrade the TFM.

### Package Management
Central Package Management (CPM) is active via `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`). Version variables used:
- `$(AspNetVersion)` → `8.0.2` (covers all `Microsoft.AspNetCore.*`)
- `$(SystemExtensionVersion)` → `8.0.0` (covers `Microsoft.Extensions.*`, `System.Net.Http.Json`)
- `$(EntityFramworkCoreVersion)` → `8.0.2` (covers all `Microsoft.EntityFrameworkCore.*`)
- `$(VSCodeGeneratorVersion)` → `8.0.0`

These four variables plus `<TargetFramework>` are the primary levers for the version upgrade — updating them in `Directory.Packages.props` propagates to all consuming projects.

### CI/CD Analysis
| Workflow | Old pinning | Action |
|---|---|---|
| `.github/workflows/dotnetcore.yml` | `dotnet-version: '8.0.x'` | **Updated** to `10.0.x` |
| `.github/workflows/modernize.yml` | Already had `8.0.x` + `10.0.x` | No change needed |
| `.github/dependabot.yml` | No SDK pinning | No change needed |

---

## Changes Made

### `global.json`
```diff
-    "version": "8.0.x",
+    "version": "10.0.100",
     "rollForward": "latestFeature"
```
Note: `"8.0.x"` is not a valid semver for global.json; the new value `"10.0.100"` is a valid floor version with `latestFeature` rolling forward to `10.0.302` (the latest installed .NET 10 feature band).

### `.github/workflows/dotnetcore.yml`
```diff
-        dotnet-version: '8.0.x'
+        dotnet-version: '10.0.x'
```

---

## Baseline Build Result (net8.0 projects, .NET 10 SDK)

**Restore**: ✅ All 10 projects restored successfully.  
**Build**: ✅ `Build succeeded` — 0 errors, 9 warnings.

### Pre-existing Warnings (baseline, not introduced by this task)
| Warning | Location | Disposition |
|---|---|---|
| NU1903: `System.Text.Json` 8.0.3 — high severity CVE | ApplicationCore.csproj | Fix in package upgrade tasks (bump to 10.x) |
| NU1902: `Azure.Identity` 1.10.4 — moderate severity CVE | Web.csproj | Fix in package upgrade tasks |
| SYSLIB0051: `Exception(SerializationInfo, StreamingContext)` obsolete | ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs:12 | Fix when upgrading ApplicationCore TFM |
| xUnit2013: Use `Assert.Single`/`Assert.Empty` instead of `Assert.Equal` for size checks (×4) | UnitTests, IntegrationTests | Fix when upgrading test projects |

All warnings are pre-existing and arise from package/API compatibility with .NET 10's stricter analyzers. None are introduced by this task's changes. They are tracked here for targeted resolution in subsequent tasks.

---

## Build Tool Decision
**All projects**: `dotnet build` — SDK-style, no WPF/WinForms/XAML/COM/`.resx`-with-images, no `net4xx` TFMs. Cross-platform `dotnet build` is correct throughout the upgrade.

---

## Done-When Checklist
- [x] .NET 10 SDK (`10.0.302`) is selected and available
- [x] `global.json` updated — no longer pins to `8.0.x`
- [x] CI (`dotnetcore.yml`) updated to install `10.0.x`
- [x] Verified prerequisite baseline: full solution restores and builds clean with 0 errors under .NET 10 SDK
- [x] Package families and TFM central control points identified for subsequent tasks
