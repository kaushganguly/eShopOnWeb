# Progress: 02-upgrade-core-libraries

## Changes Made

1. **`src/ApplicationCore/ApplicationCore.csproj`** — removed explicit `PackageReference`
   entries for `System.Security.Claims` and `System.Text.Json` (now provided by the
   net10.0 shared framework).
2. **`src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`** — removed the
   obsolete `SerializationInfo`/`StreamingContext` protected constructor (source
   incompatible on modern .NET, part of the legacy `BinaryFormatter` serialization
   pattern that has been deprecated since .NET 8 / SYSLIB0051).
3. **`src/BlazorShared/BlazorShared.csproj`** — removed the `BlazorInputFile` package
   reference (abandoned, no net10-compatible build).
4. **`src/BlazorShared/Models/CatalogItem.cs`** — removed the now-orphaned
   `DataToBase64(IFileListEntry)` helper and its `BlazorInputFile`/unused `Task` usings.
5. **`src/Infrastructure/Infrastructure.csproj`** — no code changes required; picks up
   net10.0 target and updated EF Core / package versions automatically via the central
   `Directory.Packages.props` changes made in task 01.

All three projects now target `net10.0` via the central `Directory.Packages.props`
`<TargetFramework>` property (no per-project TFM overrides exist in this repo).

## Validation

```
dotnet build src/ApplicationCore/ApplicationCore.csproj    → Build succeeded, 0 Warning(s), 0 Error(s)
dotnet build src/Infrastructure/Infrastructure.csproj       → Build succeeded, 0 Warning(s), 0 Error(s)
dotnet build src/BlazorShared/BlazorShared.csproj            → Build succeeded, 0 Warning(s), 0 Error(s)
```

All three core library projects build cleanly on net10.0 with zero warnings and zero
errors.
