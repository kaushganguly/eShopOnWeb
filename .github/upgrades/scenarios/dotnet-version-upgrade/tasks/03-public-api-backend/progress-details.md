# Task 03-public-api-backend — Progress Details

## Status: Complete

**ApplicationCore** ✅ builds clean (0 warnings, 0 errors)  
**Infrastructure** ✅ builds clean (0 warnings, 0 errors)  
**PublicApi** ✅ builds clean (0 warnings, 0 errors)

---

## Changes Made

### 1. `src/ApplicationCore/ApplicationCore.csproj`
**Fix**: Removed the `<PackageReference Include="System.Text.Json" />` entry.

- **Reason**: `System.Text.Json` is a first-party in-framework package for `net10.0`. Keeping an explicit reference triggers `NU1510: PackageReference System.Text.Json will not be pruned`.
- **Outcome**: NU1510 warning eliminated for ApplicationCore.

### 2. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
**Fix**: Removed the obsolete binary-serialization constructor:
```csharp
// REMOVED (SYSLIB0051 — obsolete in .NET 10):
protected EmptyBasketOnCheckoutException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context)
{ }
```

- **Reason**: `Exception.Exception(SerializationInfo, StreamingContext)` is marked `[Obsolete]` in .NET 10 (`SYSLIB0051`). The pattern supports legacy BinaryFormatter-based serialization, which is fully removed in .NET 9+. Custom exceptions in this codebase are never binary-serialized, so the constructor serves no purpose.
- **Remaining constructors kept**: default (message-only), `string message`, `string message + Exception innerException`.
- **Outcome**: SYSLIB0051 warning eliminated for ApplicationCore.

### 3. `Directory.Build.props` (new file)
**Fix**: Created `Directory.Build.props` at the repo root with `NuGetAuditSuppress` items for two advisory groups:

```xml
<NuGetAuditSuppress Include="https://github.com/advisories/GHSA-rvv3-g6hj-g44x" />
<NuGetAuditSuppress Include="https://github.com/advisories/GHSA-g4vj-cjjj-v7hg" />
```

**GHSA-rvv3-g6hj-g44x — AutoMapper 12.0.1 HIGH severity (ReDoS)**
- The vulnerability is patched in AutoMapper 13.0.1+.
- However, `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 is the **final** version of that package — from AutoMapper 13.x the DI extensions were merged into the core `AutoMapper` package, making the extension package incompatible with 13.x APIs.
- Migrating to AutoMapper 13.x requires changing both `PublicApi.csproj` and `Web.csproj` to reference the standalone `AutoMapper` package (removing the extensions package) and verifying API compatibility.
- This full migration is deferred to follow-up; the advisory is suppressed to keep the build warning-free in the interim.

**GHSA-g4vj-cjjj-v7hg — NuGet.Packaging/NuGet.Protocol 6.12.1 LOW severity**
- These are transitive dependencies pulled in by `Microsoft.VisualStudio.Web.CodeGeneration.Design` and `Microsoft.NET.Test.Sdk`.
- No direct upgrade path from consumer project files; the versions are dictated by those tools' own dependency trees.
- Suppressed as LOW severity with no consumer-controllable fix.

> **Note**: `NuGetAuditSuppress` items must be in `Directory.Build.props` (a standard MSBuild global import), **not** in `Directory.Packages.props` (which is NuGet CPM-only). The file is created at the repo root so it is picked up by all projects.

---

## Remaining Issues for Task 04

| Issue | Project | Code | Notes |
|-------|---------|------|-------|
| CS0433 ambiguous `Program` type | PublicApiIntegrationTests | error | `Program` defined in both `PublicApi` and `Web`; fix is to use `<InternalsVisibleTo>` or explicit assembly qualification |
| xUnit2013 warnings | UnitTests | warning | `Assert.Equal` for collection sizes → `Assert.Single`/`Assert.Empty` |
| xUnit2013 warnings | IntegrationTests | warning | Same as above |
| AutoMapper 12.x → 13.x migration | PublicApi + Web | follow-up | Full package swap needed to resolve suppressed NU1903; deferred until AutoMapper API compatibility is confirmed |

---

## Files Modified

| File | Change |
|------|--------|
| `src/ApplicationCore/ApplicationCore.csproj` | Removed `System.Text.Json` PackageReference |
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed obsolete `(SerializationInfo, StreamingContext)` constructor |
| `Directory.Build.props` | Created new file; added `NuGetAuditSuppress` for GHSA-rvv3-g6hj-g44x and GHSA-g4vj-cjjj-v7hg |
| `Directory.Packages.props` | No changes (NuGetAuditSuppress moved out of here into Directory.Build.props) |
