# Progress: 02-upgrade-class-libraries

## Summary

Upgraded `ApplicationCore`, `Infrastructure`, and `BlazorShared` (plus the shared centrally-managed
package file) from net8.0 to net10.0. Because this repo centralizes `TargetFramework` as a shared
MSBuild property inside `Directory.Packages.props`, this single edit updates the TFM for **all**
projects in the solution simultaneously (matches the All-at-Once strategy).

## Files changed

- `Directory.Packages.props`
  - `<TargetFramework>` net8.0 → net10.0
  - `$(AspNetVersion)` 8.0.2 → 10.0.12
  - `$(SystemExtensionVersion)` 8.0.0 → 10.0.12
  - `$(EntityFramworkCoreVersion)` 8.0.2 → 10.0.12
  - `$(VSCodeGeneratorVersion)` 8.0.0 → 10.0.2
  - `Azure.Identity` 1.10.4 → 1.21.0 (fixes flagged security vulnerability)
  - `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0 (deprecated package, moved to LTS line)
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 → 1.23.0 (latest stable; resolves the
    one "incompatible package" flagged in assessment — build-time-only VS Docker debug tooling, no
    runtime code dependency)
  - Removed `System.Text.Json` PackageVersion entry entirely (framework-provided in net10, package
    reference triggered NU1510 "will not be pruned" warning)
  - Removed `System.Security.Claims` PackageVersion entry (functionality merged into framework;
    namespace still usable without the package)
  - Test package versions bumped (shared across all test projects, used starting task 04):
    `Microsoft.NET.Test.Sdk` → 18.10.1, `xunit` → 2.9.3, `xunit.runner.visualstudio` → 4.0.0,
    `xunit.runner.console` → 2.9.3, `MSTest.TestAdapter`/`MSTest.TestFramework` → 4.4.1,
    `coverlet.collector` → 10.1.0
- `src/ApplicationCore/ApplicationCore.csproj`
  - Removed `<PackageReference Include="System.Security.Claims" />` (redundant, functionality in framework)
  - Removed `<PackageReference Include="System.Text.Json" />` (redundant post-build warning fix, NU1510)
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
  - Removed the obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)`
    binary-formatter serialization constructor (Api.0002 source-incompatible finding). Confirmed via
    `grep` this pattern is not used anywhere else in the codebase.

## Validation

- `dotnet build src/ApplicationCore/ApplicationCore.csproj` → Build succeeded, 0 warnings, 0 errors
  (also builds BlazorShared as a dependency).
- `dotnet build src/Infrastructure/Infrastructure.csproj` → Build succeeded, 0 warnings, 0 errors
  (also builds ApplicationCore + BlazorShared as dependencies).
- All three target projects now compile clean against net10.0.

## Deferred to later tasks

- Web/PublicApi/BlazorAdmin-specific package upgrades and API fixes → task 03.
- Test project package upgrades + full test run → task 04.
- Full solution build + `dotnet test` + final vulnerability re-check → task 05.
