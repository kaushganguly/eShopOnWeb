# Progress Details: 03-web-and-api-hosts

## Summary
Upgraded Web, PublicApi, and BlazorAdmin to build cleanly on net10.0 (TFM already
switched centrally in task 02). Fixed breaking API changes from the Swashbuckle/Microsoft.OpenApi
major version bump, migrated off the deprecated `AutoMapper.Extensions.Microsoft.DependencyInjection`
package (which was pinned to a vulnerable transitive `AutoMapper` 12.0.1), removed NU1510
"package will not be pruned" warnings, and removed an unused scaffolding tool package that was
the sole source of two low-severity NU1901 transitive-dependency vulnerability warnings.

## Changes Made

### `src/PublicApi/CustomSchemaFilters.cs`
- `Microsoft.OpenApi.Models` namespace was collapsed into the root `Microsoft.OpenApi` namespace in Microsoft.OpenApi 2.x (pulled in by Swashbuckle.AspNetCore 10.2.3). Updated `using` accordingly.
- `ISchemaFilter.Apply` now takes `IOpenApiSchema` (interface) instead of the concrete `OpenApiSchema` class — updated method signature.
- `schema.Properties` is now nullable (`IDictionary<string, IOpenApiSchema>?`) — added a null-conditional check to fix a new CS8602 nullable-dereference warning.

### `src/PublicApi/Program.cs`
- Updated `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;` (namespace consolidation, see above).
- `OpenApiReference` class was **removed entirely** in Microsoft.OpenApi 2.x, replaced by typed reference classes. Replaced the old
  `new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }, ... }`
  pattern with `new OpenApiSecuritySchemeReference("Bearer", document)`.
- `SwaggerGenOptions.AddSecurityRequirement` overload changed from accepting a plain `OpenApiSecurityRequirement` instance to requiring a `Func<OpenApiDocument, OpenApiSecurityRequirement>` factory (so references can be resolved against the host document) — wrapped the call in a `document => ...` lambda.
- Replaced `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` with `builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` after migrating off `AutoMapper.Extensions.Microsoft.DependencyInjection` (see below) — the convenience single-assembly overload was removed from the newer core `AutoMapper` package; the closest matching overload requires an explicit (no-op) `Action<IMapperConfigurationExpression>` configuration delegate.

### `AutoMapper` package migration (`Directory.Packages.props`, `src/PublicApi/PublicApi.csproj`, `src/Web/Web.csproj`)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 pins an **exact** dependency on `AutoMapper` `[12.0.1]` (bracket = exact version, no override possible via central package management) which NuGet flagged with **NU1903 (high severity vulnerability, GHSA-rvv3-g6hj-g44x)**. This DI-extension package has not been updated since 12.0.1 and has no newer release.
- Since AutoMapper's core package has included its own `AddAutoMapper` DI extension methods since v12+, migrated `PublicApi.csproj` from `AutoMapper.Extensions.Microsoft.DependencyInjection` to the core `AutoMapper` package directly, bumped to **16.2.0** (net10.0-compatible, fixes the vulnerability).
- `Web.csproj` referenced `AutoMapper.Extensions.Microsoft.DependencyInjection` but never called `AddAutoMapper` or used any AutoMapper API anywhere in its code (verified via grep) — removed the unused reference entirely rather than migrating it.
- Removed the `AutoMapper.Extensions.Microsoft.DependencyInjection` `PackageVersion` entry from `Directory.Packages.props`, added an `AutoMapper` entry (16.2.0).

### `src/BlazorAdmin/BlazorAdmin.csproj` + `Directory.Packages.props`
- NuGet raised **NU1510** ("PackageReference System.Net.Http.Json will not be pruned... does not need to be referenced explicitly") because this package is now part of the net10.0 shared framework. Removed the explicit `PackageReference` and the corresponding `PackageVersion` entry (re-added once, then correctly removed again after briefly clobbering the entry that BlazorAdmin.csproj still needed at the time — verified via grep that no project references it anymore after the BlazorAdmin.csproj edit).

### `Microsoft.VisualStudio.Web.CodeGeneration.Design` removal (`src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, `Directory.Packages.props`)
- This is a design-time-only ASP.NET Core scaffolding tool package (used by the `dotnet-aspnet-codegenerator` CLI / Visual Studio "Add Scaffolded Item" wizard). Not referenced by any application code (verified via grep for `CodeGeneration`/`Scaffolding` usages — none found).
- Even at its latest net10.0-compatible version (10.0.2), it transitively pulls `NuGet.Packaging`/`NuGet.Protocol` 6.12.1, which NuGet flags with **NU1901 (low severity, GHSA-g4vj-cjjj-v7hg)**. No newer version of the scaffolding package exists to resolve this upstream issue.
- Since the package provides no runtime functionality and is unused, removed the `PackageReference` from both `Web.csproj` and `PublicApi.csproj`, the `PackageVersion` entry, and the now-unused `VSCodeGeneratorVersion` property from `Directory.Packages.props`, eliminating the warnings without suppressing them.

## Validation
- `dotnet build src/PublicApi/PublicApi.csproj` → **Build succeeded, 0 Warning(s), 0 Error(s)**
- `dotnet build src/BlazorAdmin/BlazorAdmin.csproj` → **Build succeeded, 0 Warning(s), 0 Error(s)**
- `dotnet build src/Web/Web.csproj` (also builds BlazorAdmin as a project reference) → **Build succeeded, 0 Warning(s), 0 Error(s)**

## Files Modified
- `Directory.Packages.props`
- `src/PublicApi/PublicApi.csproj`
- `src/PublicApi/Program.cs`
- `src/PublicApi/CustomSchemaFilters.cs`
- `src/Web/Web.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
