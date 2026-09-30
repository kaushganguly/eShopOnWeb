# Progress Details: 02-shared-packages-and-libraries

## Summary
Bumped the shared `TargetFramework` and all centrally-managed NuGet package versions in
`Directory.Packages.props` from net8.0 to net10.0, fixed the resulting build issues in the
class library projects (ApplicationCore, BlazorShared, Infrastructure), and validated a clean
(0 warning / 0 error) build for each.

## Changes Made

### `Directory.Packages.props`
- `<TargetFramework>` : `net8.0` → `net10.0` (applies to all 10 projects in the solution, since none override it locally)
- Shared version properties bumped:
  - `AspNetVersion`: `8.0.2` → `10.0.12`
  - `SystemExtensionVersion`: `8.0.0` → `10.0.12`
  - `EntityFramworkCoreVersion`: `8.0.2` → `10.0.12`
  - `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- Individually-versioned packages bumped (full before/after table recorded in `task.md`), notably:
  - `Azure.Identity` 1.10.4 → 1.21.0 (fixes assessment-flagged vulnerability)
  - `System.Text.Json` 8.0.3 → 10.0.12 (fixes assessment-flagged vulnerability) — later removed as an explicit reference (see below)
  - `Ardalis.*` family bumped to latest net10-compatible versions
  - `MediatR` 12.0.1 → 14.2.0, `FluentValidation` 11.9.0 → 12.1.1
  - `xunit` 2.7.0 → 2.9.3, `xunit.runner.visualstudio` 2.5.6 → 4.0.0 (verified compatible with xunit 1.9.2+ per nuget.org package description), `xunit.runner.console` 2.7.0 → 2.9.3
  - `Microsoft.NET.Test.Sdk` 17.9.0 → 18.10.1, `MSTest.TestAdapter`/`TestFramework` 3.2.2 → 4.4.1, `coverlet.collector` 6.0.2 → 10.1.0
  - `Swashbuckle.AspNetCore`/`SwaggerUI`/`Annotations` 6.5.0 → 10.2.3
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 → 1.22.1 (assessment-flagged incompatible package; confirmed 1.22.1 is latest stable via nuget.org)
  - `Microsoft.Web.LibraryManager.Build` 2.1.175 → 3.0.114
  - `NSubstitute` 5.1.0 → 6.2.0
  - `Microsoft.AspNetCore.Mvc` hardcoded `2.2.0` (unused, no project references it) switched to `$(AspNetVersion)` for consistency
- Removed unused `PackageVersion` entries for `System.Security.Claims` and `System.Text.Json` after removing their `PackageReference` from ApplicationCore.csproj (see below — NU1510 "auto pruned" warning).

### `src/ApplicationCore/ApplicationCore.csproj`
- Removed explicit `PackageReference` for `System.Security.Claims` and `System.Text.Json` — NuGet's NU1510 warning indicated both are automatically part of the net10.0 shared framework and no longer need explicit references.

### `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
- Removed the obsolete binary-serialization constructor (`SerializationInfo`/`StreamingContext`) that triggered a SYSLIB0051 warning. Binary serialization support was removed/obsoleted starting in .NET; the constructor was unused elsewhere in the codebase (grep confirmed no callers), so it was deleted outright rather than suppressed.

## Validation
- `dotnet build src/ApplicationCore/ApplicationCore.csproj` → **Build succeeded, 0 Warning(s), 0 Error(s)** (also builds BlazorShared as a project reference)
- `dotnet build src/Infrastructure/Infrastructure.csproj` → **Build succeeded, 0 Warning(s), 0 Error(s)**

## Files Modified
- `Directory.Packages.props`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
