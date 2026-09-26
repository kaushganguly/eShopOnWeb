# Progress Details: 03-upgrade-web-apps

## Files modified

- `Directory.Packages.props`
  - `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.1) → replaced with `AutoMapper` (16.2.0)
  - `Azure.Identity` 1.10.4 → 1.21.0 (security fix)
  - `Swashbuckle.AspNetCore` / `Swashbuckle.AspNetCore.SwaggerUI` / `Swashbuckle.AspNetCore.Annotations` 6.5.0 → 10.2.3
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 → 1.22.1
  - `Microsoft.Web.LibraryManager.Build` 2.1.175 → 3.0.114
  - Added `NuGet.Packaging` and `NuGet.Protocol` PackageVersion overrides at 6.12.5 (fixes GHSA-g4vj-cjjj-v7hg
    pulled in transitively by `Microsoft.VisualStudio.Web.CodeGeneration.Design`)
  - Removed `System.Net.Http.Json` PackageVersion entry (no longer referenced by any project)
- `src/Web/Web.csproj`
  - `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`
  - Added explicit `PackageReference` for `NuGet.Packaging` / `NuGet.Protocol` (forces resolution to patched version)
- `src/PublicApi/PublicApi.csproj`
  - `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`
  - Bumped `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (via central version)
  - Added explicit `PackageReference` for `NuGet.Packaging` / `NuGet.Protocol`
- `src/PublicApi/Program.cs`
  - `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;`
  - `AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)`
  - Rewrote `AddSecurityRequirement(...)` to use the new `Func<OpenApiDocument, OpenApiSecurityRequirement>`
    overload and `OpenApiSecuritySchemeReference` (Microsoft.OpenApi v2 breaking change)
- `src/PublicApi/CustomSchemaFilters.cs`
  - `using Microsoft.OpenApi.Models;` → `using Microsoft.OpenApi;`
  - `ISchemaFilter.Apply(OpenApiSchema, ...)` → `Apply(IOpenApiSchema schema, ...)` with a safe
    down-cast to the concrete `OpenApiSchema` type and a null-check on `.Properties`
- `src/BlazorAdmin/BlazorAdmin.csproj`
  - Removed the now-redundant `System.Net.Http.Json` `PackageReference` (framework-provided in net10.0)

## Validation

- `dotnet build src/BlazorAdmin/BlazorAdmin.csproj` → Build succeeded, 0 Warnings, 0 Errors
- `dotnet build src/PublicApi/PublicApi.csproj` → Build succeeded, 0 Warnings, 0 Errors
- `dotnet build src/Web/Web.csproj` → Build succeeded, 0 Warnings, 0 Errors
- `dotnet build eShopOnWeb.sln` → fails only on pre-existing test-project issues out of this task's
  scope (`CS0433` ambiguous `Program` in `PublicApiIntegrationTests`, `xUnit2013` analyzer warnings
  in `UnitTests`/`IntegrationTests`) — deferred to task `04-upgrade-test-projects`.

## Deferred / follow-up items (moved to task 04)

- `PublicApiIntegrationTests/ProgramTest.cs(10)`: `CS0433` — ambiguous `Program` type between
  `PublicApi.Program` and `Web.Program` (both are top-level-statement-generated internal `Program`
  classes now visible to the test project via `InternalsVisibleTo` from two different assemblies).
- `xUnit2013` analyzer warnings in `UnitTests`/`IntegrationTests` (`Assert.Equal` → `Assert.Single`/`Assert.Empty`).
