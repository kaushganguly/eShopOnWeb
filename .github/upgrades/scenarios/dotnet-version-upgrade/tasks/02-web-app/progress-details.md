# 02-web-app progress details

## Files modified
- `src/Web/Web.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `src/Web/Program.cs`
- `src/Web/Views/Manage/ShowRecoverCodes.cshtml`
- `src/Web/Views/Shared/_CookieConsentPartial.cshtml`
- `src/Web/Views/Shared/_LoginPartial.cshtml`
- `src/Web/Views/Order/Detail.cshtml`
- `src/BlazorAdmin/Pages/CatalogItemPage/List.razor`
- `src/BlazorAdmin/Pages/CatalogItemPage/Create.razor`
- `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`
- `src/BlazorAdmin/Pages/CatalogItemPage/Details.razor`
- `src/BlazorAdmin/Pages/CatalogItemPage/Delete.razor`

## Build result
### `dotnet build src/Web/Web.csproj`
- Errors: 0
- Warnings: 6
- Warning sources: shared referenced projects (`ApplicationCore`, `BlazorAdmin`) via NU1510 package-pruning warnings

### `dotnet build tests/FunctionalTests/FunctionalTests.csproj`
- Errors: 0
- Warnings: 18
- Warning sources:
  - direct/transitive vulnerability warnings surfaced on `FunctionalTests` through `PublicApi` (`AutoMapper` 12.0.1, `NuGet.Packaging` 6.12.1, `NuGet.Protocol` 6.12.1)
  - shared referenced project NU1510 warnings from `ApplicationCore` and `BlazorAdmin`

## Changes summary
- Added explicit `net10.0` target frameworks to the scoped Web and FunctionalTests projects.
- Updated Web configuration binding code to .NET 10-friendly APIs and null handling.
- Fixed Razor/Blazor parsing issues exposed by .NET 10 source generation by:
  - normalizing self-closing component/image tags
  - simplifying the admin catalog list component markup structure
  - restructuring Razor views with block-safe markup/script placement

## Issues encountered
- Initial Web/FunctionalTests builds failed because referenced Blazor and Razor files were parsed more strictly under .NET 10.
- Final builds are error-free, but warning-free completion is blocked by out-of-scope dependency warnings in shared referenced projects and transitive `PublicApi` packages.
