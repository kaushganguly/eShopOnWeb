# 03-upgrade-web-apps: Upgrade Web, PublicApi, and BlazorAdmin to net10.0

Update `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, and
`src/BlazorAdmin/BlazorAdmin.csproj` to net10.0 (inherited from the central
`Directory.Packages.props` change in task 01) and resolve breaking API changes from the
major package bumps used by these three projects: `Swashbuckle.AspNetCore` (Swagger/
OpenAPI generation, now on the `Microsoft.OpenApi` v2/OpenApi-object-model rewrite),
`AutoMapper` (major version bump, dropped the separate DI-extension package), and
`Azure.Identity` (security-fix bump 1.10.4 → 1.21.0, the confirmed vulnerability from
the assessment).

**Done when**: All three projects target `net10.0`, build with 0 errors/warnings, and
the Swagger/OpenAPI security-scheme registration + AutoMapper DI registration continue
to function correctly against the new package APIs.

## Research Findings

- **`Swashbuckle.AspNetCore` → 10.2.3 family**: this major version moved to the new
  `Microsoft.OpenApi` v2 object model, which introduces `IOpenApiSchema` (interface,
  not the old concrete `OpenApiSchema` sealed usage) and a new
  `OpenApiSecuritySchemeReference` type replacing the old
  `OpenApiSecurityScheme { Reference = new OpenApiReference { ... } }` pattern:
  - `src/PublicApi/CustomSchemaFilters.cs`: `ISchemaFilter.Apply` now receives
    `IOpenApiSchema` instead of `OpenApiSchema`. Added a pattern-match/guard
    (`schema is not OpenApiSchema openApiSchema`) to access the concrete `.Properties`
    dictionary, plus a null-check on `.Properties` (nullable in the new model).
  - `src/PublicApi/Program.cs`: `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`
    (namespace moved). `AddSecurityRequirement` now takes a factory
    `(document) => new OpenApiSecurityRequirement()` instead of a plain instance, and the
    security scheme reference is now constructed via
    `new OpenApiSecuritySchemeReference("Bearer", document)` instead of the old
    `OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }, ... }`
    boilerplate.
- **`AutoMapper` major bump (→16.2.0)**: the separate
  `AutoMapper.Extensions.Microsoft.DependencyInjection` package was merged into the main
  `AutoMapper` package and no longer exists as a standalone NuGet id (assessment flagged
  this as a package-removal issue). Replaced the `PackageReference` in both
  `Web.csproj` and `PublicApi.csproj`. The DI registration call signature also changed:
  `AddAutoMapper(typeof(MappingProfile).Assembly)` → 
  `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` (now requires an explicit
  configuration action parameter even when empty) — updated in `PublicApi/Program.cs`.
- **`Azure.Identity`**: bumped 1.10.4 → 1.21.0 in `Directory.Packages.props` (task 01) —
  this was the confirmed CVE-flagged package from the assessment; no source code changes
  were required in `Web` for this bump (API-compatible).
- **`NuGet.Packaging` / `NuGet.Protocol`**: added as explicit `PackageReference`s to both
  `Web.csproj` and `PublicApi.csproj` — these are transitive dependencies of
  `Microsoft.VisualStudio.Web.CodeGeneration.Design`/tooling packages that CPM now
  requires to be pinned explicitly (central package management enforces explicit
  versions for all packages that appear in the restore graph with a floating/transitive
  version once any project declares an explicit version constraint elsewhere in the
  graph).
- **`BlazorAdmin.csproj` / `_Imports.razor`**: removed `BlazorInputFile` package
  reference and `@using BlazorInputFile` import (companion cleanup to the `BlazorShared`
  removal in task 02 — same abandoned package, no longer referenced now that
  `CatalogItem.DataToBase64` was removed). Also removed the explicit
  `System.Net.Http.Json` package reference (now provided by the net10.0 framework
  reference, same pattern as `System.Text.Json` in task 02).

## Validation

- `dotnet build src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`,
  `src/BlazorAdmin/BlazorAdmin.csproj` — see progress-details.md for results.
