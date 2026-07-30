# 03-publicapi progress details

## Completed changes
- Set explicit `net10.0` target frameworks in `src/PublicApi/PublicApi.csproj` and `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`.
- Reworked `src/PublicApi/Program.cs` to avoid the configuration binder APIs flagged by the assessment by binding `CatalogSettings` and `BaseUrlConfiguration` directly from configuration values.
- Removed deprecated/vulnerable AutoMapper usage from PublicApi and replaced it with explicit DTO mapping helpers in `src/PublicApi/MappingProfile.cs` plus endpoint updates in the catalog list endpoints.
- Removed unused PublicApi-only tooling package references (`Microsoft.VisualStudio.Web.CodeGeneration.Design`, `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`) from `src/PublicApi/PublicApi.csproj`.
- Replaced MinimalApi package-wide endpoint scanning with PublicApi-assembly-only registration/mapping in `src/PublicApi/EndpointRegistrationExtensions.cs` to avoid `ReflectionTypeLoadException` from the legacy `BlazorInputFile` dependency during integration tests.
- Updated central package versions for `MSTest.TestAdapter`, `MSTest.TestFramework`, and `System.IdentityModel.Tokens.Jwt`.
- Simplified `tests/PublicApiIntegrationTests` to reference only `src/PublicApi`, updated the paged catalog test to deserialize the PublicApi response model, and replaced obsolete `[DataTestMethod]` usage.

## Validation
### Build
- Command: `dotnet build src/PublicApi/PublicApi.csproj`
- Result: Success
- In-scope warnings: 0
- Out-of-scope warnings still emitted by referenced projects: `ApplicationCore` NU1510 warnings and existing `ApplicationCore` SYSLIB0051 warning.

### Tests
- Command: `dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj --no-restore -v minimal`
- Result: Success
- Total: 15
- Passed: 15
- Failed: 0

## Issues encountered
- Upgrading away from the deprecated AutoMapper DI package to AutoMapper 14 still left a vulnerability warning, so the API layer was switched to explicit mapping instead.
- `MinimalApi.Endpoint` scanned all loaded assemblies and failed against `BlazorInputFile` on .NET 10; a local endpoint registration extension fixed this without changing endpoint behavior.
- The integration tests had an ambiguous `Program` type because they also referenced `src/Web`; removing the unused Web reference resolved the conflict and removed unrelated dependency-version conflicts.
