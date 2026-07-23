# Task 05: Deferred Package Cleanup — Progress Details

## Status: COMPLETE

## Findings

### AutoMapper HIGH Vulnerability
- `AutoMapper` 12.0.1 (via `AutoMapper.Extensions.Microsoft.DependencyInjection`) had GHSA-rvv3-g6hj-g44x (HIGH severity - DoS via uncontrolled recursion)
- Patched versions: `>= 15.1.1` or `>= 16.1.1`
- **Action**: Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 with `AutoMapper` 15.1.3
- `AutoMapper` 16.x was tested but broke `MinimalApi.Endpoint.AddEndpoints` reflection scan. Used 15.1.3 instead.
- Updated `PublicApi/Program.cs` for AutoMapper 15.x API: `cfg.AddMaps()` syntax

### NuGet.Protocol/Packaging LOW Vulnerability
- `NuGet.Protocol`/`NuGet.Packaging` 6.12.1 had GHSA-g4vj-cjjj-v7hg (LOW severity)
- Patched version: 6.12.5
- Source: Transitive dependency of `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 (latest available)
- **Action**: Added explicit `NuGet.Protocol` 6.12.5 / `NuGet.Packaging` 6.12.5 as `PrivateAssets=all` overrides in `Web.csproj` and `PublicApi.csproj`

### BlazorInputFile — Net10.0 Incompatible + Dead Code
- `BlazorInputFile` 0.2.0 has a struct alignment issue (`ReadRequest` type) incompatible with net10.0
- This caused `ReflectionTypeLoadException` in `PublicApiIntegrationTests` when upgrading test infrastructure
- Investigation found all BlazorInputFile usage was dead code:
  - `CatalogItem.DataToBase64(IFileListEntry)` — defined but never called anywhere
  - `@using BlazorInputFile` in `_Imports.razor` — imported but never used
- **Action**: Fully removed BlazorInputFile from BlazorShared, BlazorAdmin, and Directory.Packages.props; removed dead code

### xunit Packages
- `xunit` 2.7.0 → 2.9.3 (latest v2)
- `xunit.runner.visualstudio` 2.5.6 → 2.8.2 (latest v2-compatible)
- `xunit.runner.console` removed — not needed for `dotnet test`, was bringing in NuGet.Protocol 6.12.1

### MSTest Packages
- `MSTest.TestAdapter` + `MSTest.TestFramework` 3.2.2 (deprecated) → 4.3.2
- Fixed MSTEST0044 warning: `[DataTestMethod]` → `[TestMethod]`

### Microsoft.NET.Test.Sdk
- 17.9.0 → 18.8.1

### System.Security.Claims
- Removed from Directory.Packages.props — no project references it; included in net10.0 framework

### Test Code Warnings Fixed
- xUnit2013: 3 files updated to use `Assert.Empty()` / `Assert.Single()` instead of `Assert.Equal(n, .Count)`
- MSTEST0044: 1 file updated from `[DataTestMethod]` to `[TestMethod]`

## Outcome
- 0 build errors, 0 warnings
- 74/74 tests passing
- All HIGH severity vulnerabilities resolved
- LOW severity NuGet.Protocol vulnerability resolved via explicit version override
- Solution package graph is clean and consistent on net10.0
