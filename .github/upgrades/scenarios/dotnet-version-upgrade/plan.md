# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0

> Note: `plan-generation` skill content could not be loaded in this environment (skill lookup
> tool returned no results for any query, including `scenario-initialization`, `plan`, and
> `task-execution` — the same orchestrator-dependent subsystem that also failed for
> `generate_dotnet_upgrade_assessment`). This plan was authored directly from `assessment.md`
> findings and the scenario's documented Simple-upgrade defaults (single-pass strategy — this
> solution is small, fully SDK-style, fully modern-.NET, with no circular dependencies and no
/legacy packages.config projects, so a wave/phased strategy is unnecessary overhead).

## Strategy

**Single-pass, dependency-ordered upgrade.** Because central package management means the
target framework and shared package versions are controlled from one file
(`Directory.Packages.props`), the TFM bump and package-version bumps are done together in one
pass across the whole solution, then the solution is built and tests run; remaining fixes are
applied iteratively per-project in topological build order:

1. `BlazorShared`
2. `ApplicationCore`
3. `Infrastructure`
4. `BlazorAdmin`
5. `Web`
6. `PublicApi`
7. `UnitTests`
8. `PublicApiIntegrationTests`
9. `FunctionalTests`
10. `IntegrationTests`

## Tasks

- [ ] 01-global-config: Update global.json and Directory.Packages.props to net10.0
- [ ] 02-build-core-libs: Build and fix BlazorShared, ApplicationCore, Infrastructure
- [ ] 03-build-web-apps: Build and fix BlazorAdmin, Web, PublicApi
- [ ] 04-build-and-run-tests: Build and fix all 4 test projects, run tests
- [ ] 05-full-solution-validation: Full solution build + test + security compliance pass

### 01-global-config
Update solution-wide configuration before touching any project:
- `global.json`: `sdk.version` → `10.0.x` or latest installed 10.x SDK, keep `rollForward: latestFeature`
- `Directory.Packages.props`: `TargetFramework` → `net10.0`; `AspNetVersion` → `10.0.x`; `EntityFramworkCoreVersion` → `10.0.x`; `SystemExtensionVersion` → `10.0.x`; `VSCodeGeneratorVersion` → `10.0.x`
- Bump explicit `PackageVersion` entries per `assessment.md` table (Ardalis.*, Azure.*, FluentValidation, Microsoft.Web.LibraryManager.Build, Microsoft.VisualStudio.Web.CodeGeneration.Design, NSubstitute, System.IdentityModel.Tokens.Jwt, Swashbuckle.AspNetCore(+SwaggerUI/Annotations), Microsoft.NET.Test.Sdk, xunit*, MSTest.*)
- Keep `MediatR` pinned at `12.0.1` (commercial licensing risk above v12), `AutoMapper.Extensions.Microsoft.DependencyInjection`, `Ardalis.ApiEndpoints`, `Ardalis.ListStartupServices`, `BuildBundlerMinifier`, `MinimalApi.Endpoint`, `NSubstitute.Analyzers.CSharp`, `Blazored.LocalStorage`, `BlazorInputFile`, `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`, `coverlet.collector` unchanged (no net10-specific need identified)

### 02-build-core-libs
Build `BlazorShared` → `ApplicationCore` → `Infrastructure` and fix any compile errors/binding
redirects/EF Core breaking changes surfaced by the bump.

### 03-build-web-apps
Build `BlazorAdmin` (Blazor WASM), `Web` (MVC/Razor/Identity), `PublicApi` (minimal API +
Swashbuckle) and fix breaking changes (ASP.NET Core Identity, Swashbuckle API changes, Blazor
WASM hosting model changes).

### 04-build-and-run-tests
Build and run `UnitTests`, `PublicApiIntegrationTests`, `FunctionalTests`, `IntegrationTests`;
fix compile errors (MSTest 3→4 analyzer changes, xUnit bumps) and any runtime test failures
caused by behavioral changes between .NET 8 and .NET 10.

### 05-full-solution-validation
`dotnet build eShopOnWeb.sln` (warm, from clean) with zero errors/warnings in modified projects,
`dotnet test` across all 4 test projects, confirm success criteria: passBuild, passUnitTests,
securityComplianceCheck (no known-vulnerable package versions left in `Directory.Packages.props`).

## Success Criteria
- [ ] All projects target `net10.0`
- [ ] Central package versions updated per assessment table
- [ ] Solution builds with 0 errors / 0 warnings (in touched projects)
- [ ] All unit/integration/functional tests pass
- [ ] No known-vulnerable package versions remain
