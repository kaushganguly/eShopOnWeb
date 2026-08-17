# .NET 8 → .NET 10 Upgrade Plan

## Objective
Upgrade all 10 projects in eShopOnWeb from `net8.0` to `net10.0` (LTS).

## Strategy
**Direct upgrade** — upgrade all projects simultaneously from net8.0 to net10.0.
All projects are SDK-style, low difficulty, no multi-targeting needed.

## Execution Constraints
- Target Framework: `net10.0`
- Solution: `eShopOnWeb.sln`
- Working Branch: `upgrade/dotnet10`
- Flow Mode: Automatic

## Tasks

### 01-update-sdk-and-tfm
Update `global.json` SDK version to `10.0.x` and set `TargetFramework` to `net10.0` in `Directory.Packages.props`.

**Files:**
- `global.json`
- `Directory.Packages.props`

**Changes:**
- `global.json`: set `sdk.version` to `10.0.x`
- `Directory.Packages.props`: set `<TargetFramework>net10.0</TargetFramework>`

---

### 02-update-nuget-packages
Update all NuGet package versions in `Directory.Packages.props` to .NET 10 compatible versions, remove incompatible packages and packages now included in framework.

**Changes in Directory.Packages.props:**
- `AspNetVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.Net.Http.Json`: `8.0.0` → `10.0.11`
- `Azure.Identity`: `1.10.4` → `1.21.0`
- Remove `System.Security.Claims` 4.3.0 (now included in framework reference)

**Changes in PublicApi.csproj:**
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible with net10.0, no new version)

---

### 03-fix-api-breaking-changes
Fix source-incompatible and binary-incompatible API issues.

**Files to change:**
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
  - Remove obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
    (Exception(SerializationInfo, StreamingContext) was removed in .NET 10)
- `src/Web/Configuration/ConfigureCookieSettings.cs`
  - Fix `TimeSpan.FromMinutes(ValidityMinutesPeriod)` — cast to long to avoid ambiguity with new .NET 10 overloads
- `src/ApplicationCore/ApplicationCore.csproj`
  - Remove explicit reference to `System.Security.Claims` if it exists (it's handled in Directory.Packages.props)

---

### 04-build-and-validate
Build the full solution and run all tests to confirm the upgrade is complete.

**Steps:**
1. `dotnet restore eShopOnWeb.sln`
2. `dotnet build eShopOnWeb.sln --no-restore`
3. `dotnet test eShopOnWeb.sln --no-build`
4. Fix any remaining compilation errors or test failures.
