# 03-applications: Progress Details

## Status: Complete

## Build Results

| Project | Errors | Warnings |
|---------|--------|----------|
| BlazorAdmin | 0 | 0 |
| PublicApi | 0 | 0 |
| Web | 0 | 0 |

## Issues Found and Resolved

### 1. BlazorAdmin: NU1510 — Unnecessary `System.Net.Http.Json` package

**Warning:** `NU1510: PackageReference System.Net.Http.Json will not be pruned. Consider removing this package from your dependencies, as it is likely unnecessary.`

**Root cause:** `System.Net.Http.Json` is bundled in the .NET 10 runtime. Explicitly referencing it causes NuGet to warn that it cannot prune the package.

**Fix:** Removed `<PackageReference Include="System.Net.Http.Json" />` from `src/BlazorAdmin/BlazorAdmin.csproj`. The functionality is available through the .NET 10 SDK.

---

### 2. PublicApi + Web: NU1903 — AutoMapper vulnerability GHSA-rvv3-g6hj-g44x

**Warning:** `NU1903: Package 'AutoMapper' 12.0.1 has a known high severity vulnerability, https://github.com/advisories/GHSA-rvv3-g6hj-g44x`

**Root cause:** `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` (the latest version of the extensions package) depends on `AutoMapper 12.0.1` which has a DoS vulnerability via uncontrolled recursion. The advisory covers AutoMapper < 15.1.1 and 16.0.0 <= ver < 16.1.1.

**Investigation:** 
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 is the latest available. No newer version exists.
- AutoMapper 12+ merged DI support directly into AutoMapper, making the extensions package a thin wrapper.
- AutoMapper 15.1.1+ fixes the vulnerability.
- AutoMapper 15.x changed `AddAutoMapper` API: the `(Assembly)` overload was removed; only `(Action<IMapperConfigurationExpression>)` is supported.
- Web project had `AutoMapper.Extensions.Microsoft.DependencyInjection` referenced but did NOT use AutoMapper in code — safe to remove entirely.

**Fix:**
- `Directory.Packages.props`: Removed `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1`; added `AutoMapper 15.1.3` (latest stable in the 15.x fixed range).
- `src/PublicApi/PublicApi.csproj`: Changed `<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />` → `<PackageReference Include="AutoMapper" />`.
- `src/Web/Web.csproj`: Removed `<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />` entirely (Web does not use AutoMapper).
- `src/PublicApi/Program.cs`: Updated `AddAutoMapper` call from `AddAutoMapper(typeof(MappingProfile).Assembly)` to `AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))` to match the AutoMapper 15.x API.

---

### 3. PublicApi + Web: NU1901 — NuGet.Packaging/Protocol vulnerability GHSA-g4vj-cjjj-v7hg

**Warning:** 
- `NU1901: Package 'NuGet.Packaging' 6.12.1 has a known low severity vulnerability, https://github.com/advisories/GHSA-g4vj-cjjj-v7hg`
- `NU1901: Package 'NuGet.Protocol' 6.12.1 has a known low severity vulnerability, https://github.com/advisories/GHSA-g4vj-cjjj-v7hg`

**Root cause:** `Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2` transitively depends on `NuGet.Packaging 6.12.1` and `NuGet.Protocol 6.12.1`, which have a known low severity vulnerability. The fixed versions are 6.13.2+.

**Fix:**
- `Directory.Packages.props`: Added `PackageVersion` entries for `NuGet.Packaging 6.13.2` and `NuGet.Protocol 6.13.2`.
- `src/PublicApi/PublicApi.csproj`: Added explicit `<PackageReference>` entries for `NuGet.Packaging` and `NuGet.Protocol` with `<PrivateAssets>all</PrivateAssets>` (build-only, not exposed to consumers). This overrides the transitive version from CodeGeneration.Design.
- `src/Web/Web.csproj`: Same explicit overrides added.

---

## Pre-existing Issues (Not Fixed — Out of Scope)

### PublicApiIntegrationTests CS0433

**Error:** `error CS0433: The type 'Program' exists in both 'PublicApi' and 'Web'`

**Status:** Pre-existing. This error existed before any changes in this task (verified by stash test). The `PublicApiIntegrationTests` project references both `PublicApi.csproj` and `Web.csproj`. Both assemblies have a `Program` class (PublicApi has explicit `public partial class Program {}` and Web has an implicit one from top-level statements). This ambiguity is a pre-existing design issue in the test project and is out of scope for this task (applies to test project `tests/PublicApiIntegrationTests/`).

### xUnit2013 Warnings in Test Projects

**Warnings:** `xUnit2013: Do not use Assert.Equal() to check for collection size...` in `tests/UnitTests/` and `tests/IntegrationTests/`.

**Status:** Pre-existing. These are in test projects (out of scope for this task).

---

## Files Changed

| File | Change |
|------|--------|
| `Directory.Packages.props` | Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` with `AutoMapper 15.1.3`; added `NuGet.Packaging 6.13.2` and `NuGet.Protocol 6.13.2` version overrides |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed `System.Net.Http.Json` PackageReference (bundled in net10.0) |
| `src/PublicApi/PublicApi.csproj` | Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`; added `NuGet.Packaging` and `NuGet.Protocol` override references |
| `src/PublicApi/Program.cs` | Updated `AddAutoMapper` call to use `cfg => cfg.AddMaps(...)` for AutoMapper 15.x compatibility |
| `src/Web/Web.csproj` | Removed unused `AutoMapper.Extensions.Microsoft.DependencyInjection`; added `NuGet.Packaging` and `NuGet.Protocol` override references |
