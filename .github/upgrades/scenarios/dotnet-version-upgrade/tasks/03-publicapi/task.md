# 03-publicapi: Upgrade PublicApi to .NET 10

## Findings
- `src/PublicApi/PublicApi.csproj` was implicitly building as `net10.0` via central props, but it did not declare its own target framework. The task now sets it explicitly.
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` likewise relied on central props and now explicitly targets `net10.0`.
- `Program.cs` used the configuration binder APIs flagged by the assessment (`ConfigurationBinder.Get<T>` and `Services.Configure<T>(IConfiguration)`). The PublicApi startup path now binds the small settings objects explicitly from configuration keys instead of those older binder overloads.
- `AutoMapper.Extensions.Microsoft.DependencyInjection` was deprecated and both the legacy DI package and newer `AutoMapper` package line still produced vulnerability warnings. PublicApi now removes AutoMapper entirely and uses explicit DTO mapping in the API layer.
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` and `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` were design-time/tooling packages only for PublicApi. They were removed from `src/PublicApi/PublicApi.csproj` because they were not required to build/run the API and contributed upgrade noise/vulnerability warnings.
- The `MinimalApi.Endpoint` assembly scanner attempted to enumerate every loaded assembly during integration tests and failed on the legacy `BlazorInputFile` dependency. PublicApi now registers/maps endpoint classes from its own assembly only.
- `tests/PublicApiIntegrationTests` used deprecated MSTest 3.2.2 packages. Central package versions now use MSTest 4.3.3.
- `System.IdentityModel.Tokens.Jwt` was already upgraded during earlier work; it is bumped again to 8.22.0 to keep PublicApi on the latest stable line.

## Planned changes
1. Set explicit `net10.0` target frameworks in the PublicApi and PublicApiIntegrationTests project files.
2. Replace deprecated AutoMapper usage in PublicApi with explicit mapping logic.
3. Remove unused PublicApi-only design-time packages that produced transitive restore warnings.
4. Update PublicApi-related central package versions and align the integration test project to PublicApi only.
5. Build and run PublicApi integration tests, then record results in `progress-details.md`.
