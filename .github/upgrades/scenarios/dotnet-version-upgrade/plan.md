# .NET Version Upgrade Plan: net8.0 → net10.0

Upgrade all 10 projects in eShopOnWeb solution from .NET 8 to .NET 10 (LTS). Strategy: single-pass upgrade using central package management (Directory.Packages.props already in use).

---

### 01-central-config: Update central configuration and package versions

Update the two central files that control the TFM and package versions for the entire solution. `global.json` pins the SDK and `Directory.Packages.props` drives all `PackageVersion` entries through CPM variables. All version bumps are applied here, and two entries are removed: `System.Security.Claims` (now part of the framework, NuGet.0003) and `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10-compatible version exists, NuGet.0001).

**Done when**: `global.json` targets the 10.0.x SDK; `Directory.Packages.props` sets `TargetFramework=net10.0` and all version variables match the net10-compatible values listed in the assessment.

---

### 02-project-files: Remove incompatible package references from project files

Two individual project files carry `<PackageReference>` entries that reference packages removed from `Directory.Packages.props` in the previous task. Those stale references must be deleted so restore does not error.

**Done when**: `ApplicationCore.csproj` no longer references `System.Security.Claims`; `PublicApi.csproj` no longer references `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`.

---

### 03-api-fixes: Fix breaking API changes between .NET 8 and .NET 10

Four source files contain API usages that are source- or binary-incompatible with .NET 10.

1. `EmptyBasketOnCheckoutException.cs` — remove the obsolete `Exception(SerializationInfo, StreamingContext)` protected constructor; this overload was removed from .NET 10 (Api.0002).
2. `ConfigureCookieSettings.cs` — `TimeSpan.FromMinutes` gained a new `long` overload in .NET 9 creating an ambiguity for the `const int` literal argument; fix by casting to `long` (Api.0002).
3. `ConfigureWebServices.cs` — `services.Configure<T>(IConfiguration)` is binary-incompatible in net10; replace with `services.Configure<T>(configuration.GetSection(""))` or bind explicitly (Api.0001).
4. `ConfigureCoreServices.cs` — `configuration.Get<T>()` (extension method) is binary-incompatible; replace with `configuration.GetSection("").Get<T>()` or the typed `GetValue<T>` pattern (Api.0001).
5. `PublicApi/Program.cs` — same `Configure<T>(IConfiguration)` and `Get<T>()` usages at lines 41-50 (Api.0001).

**Done when**: All five files compile without errors against net10.0.

---

### 04-build-validate: Build solution and run unit tests

Restore, build, and run unit tests to confirm the upgrade is complete and no regressions were introduced.

**Done when**: `dotnet build eShopOnWeb.sln` exits with code 0 and zero warnings in modified projects; `dotnet test tests/UnitTests/UnitTests.csproj` passes all tests.
