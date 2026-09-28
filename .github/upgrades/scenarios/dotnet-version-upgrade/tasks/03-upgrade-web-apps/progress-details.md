# Progress: 03-upgrade-web-apps

## Changes Made

1. **`src/PublicApi/CustomSchemaFilters.cs`** — updated `ISchemaFilter.Apply` to the new
   `IOpenApiSchema` parameter type (Swashbuckle/Microsoft.OpenApi v2), with a type guard
   and null-check for `.Properties`.
2. **`src/PublicApi/Program.cs`** — `using Microsoft.OpenApi.Models` → `using Microsoft.OpenApi`;
   `AddAutoMapper` call updated to the new signature requiring a config action;
   `AddSecurityRequirement` updated to the new document-factory + `OpenApiSecuritySchemeReference` API.
3. **`src/PublicApi/PublicApi.csproj`** — replaced
   `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper`; added
   explicit `NuGet.Packaging`/`NuGet.Protocol` package references required by CPM.
4. **`src/Web/Web.csproj`** — same `AutoMapper` package replacement; added
   `NuGet.Packaging`/`NuGet.Protocol` explicit references.
5. **`src/BlazorAdmin/BlazorAdmin.csproj`** and **`_Imports.razor`** — removed
   `BlazorInputFile` package reference/import and the now-unused explicit
   `System.Net.Http.Json` reference (framework-provided on net10.0).

All three projects target `net10.0` via the central `Directory.Packages.props`
`<TargetFramework>` property.

## Validation

```
dotnet build src/PublicApi/PublicApi.csproj      → Build succeeded, 0 Warning(s), 0 Error(s)
dotnet build src/Web/Web.csproj                   → Build succeeded, 0 Warning(s), 0 Error(s)
dotnet build src/BlazorAdmin/BlazorAdmin.csproj   → Build succeeded, 0 Warning(s), 0 Error(s)
```

All three web-facing projects (and their Blazor WASM output) build cleanly on net10.0
with zero warnings and zero errors, including the `Azure.Identity` security fix
(1.10.4 → 1.21.0) applied centrally in task 01.
