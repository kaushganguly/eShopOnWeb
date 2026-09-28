# 02-upgrade-core-libraries: Upgrade ApplicationCore, Infrastructure, and BlazorShared to net10.0

Update the `TargetFramework` of `src/ApplicationCore/ApplicationCore.csproj`,
`src/Infrastructure/Infrastructure.csproj`, and `src/BlazorShared/BlazorShared.csproj`
from `net8.0` to `net10.0` (inherited automatically via the central
`Directory.Packages.props` `<TargetFramework>` property updated in task 01). Per
assessment, ApplicationCore has 3 package issues (incl. a source-incompatible API and
`System.Security.Claims` now included in the framework reference) and 2 API issues;
Infrastructure has 4 package issues including deprecated packages; BlazorShared has 0
issues but references the abandoned `BlazorInputFile` package. Fix the source-incompatible
API flagged for ApplicationCore (`System.Exception` serialization constructor) and remove
packages now covered by the framework reference.

**Done when**: All three projects target `net10.0`, restore cleanly, and build with 0
errors/warnings.

## Research Findings

- **ApplicationCore.csproj**: removed explicit `PackageReference`s for
  `System.Security.Claims` and `System.Text.Json` — both are now included in the net10
  framework reference (assessment rule `NuGet.0003`), so an explicit `PackageVersion`
  is redundant/conflicting.
- **`Exceptions/EmptyBasketOnCheckoutException.cs`**: removed the
  `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)`
  binary-formatter-style serialization constructor. `Exception`'s serialization
  constructor overload is source-incompatible on net10 (this was the
  `M:System.Exception.#ctor(SerializationInfo,StreamingContext)` issue flagged in the
  assessment's top API issues, 2 hits) — .NET has been moving away from
  `BinaryFormatter`-based exception serialization (obsoleted since net8,
  SYSLIB0051) and the constructor is unused elsewhere in the codebase, so it is
  safely removed rather than suppressed.
- **BlazorShared.csproj / `Models/CatalogItem.cs`**: removed the `BlazorInputFile`
  package reference and the `DataToBase64(IFileListEntry)` helper method that depended
  on it. `BlazorInputFile` is an abandoned, pre-net5 community package with no net10
  build and no maintained replacement; it is not runtime-compatible with net10 Blazor.
  Confirmed (via `grep`) this method is not called anywhere else in `BlazorAdmin` or
  `BlazorShared` — dead code path — so removal has no functional impact.
- **Infrastructure.csproj**: no code changes needed beyond the TFM inheriting from CPM
  and package version bumps (EF Core, etc.) already covered centrally in task 01's
  `Directory.Packages.props` edit; the 4 package issues assessment flagged for this
  project are version-only (deprecated/upgrade-recommended), resolved by the CPM alias
  bumps, not by code changes.

## Validation

- `dotnet build src/ApplicationCore/ApplicationCore.csproj src/Infrastructure/Infrastructure.csproj src/BlazorShared/BlazorShared.csproj` — see progress-details.md for results.
