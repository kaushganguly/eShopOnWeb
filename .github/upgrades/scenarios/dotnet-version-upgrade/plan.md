# Upgrade Plan — eShopOnWeb: net8.0 → net10.0

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Strategy | All-At-Once | 10 projects, all already modern (net8.0), ≤15 projects, ≤3-tier dependency depth, no CI-green constraint |

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0, clear 3-tier dependency structure, no .NET Framework projects, no high-risk package migrations.

## Projects (single group — all upgraded together)

**Libraries**
- src/BlazorShared/BlazorShared.csproj
- src/ApplicationCore/ApplicationCore.csproj
- src/Infrastructure/Infrastructure.csproj

**Applications**
- src/BlazorAdmin/BlazorAdmin.csproj
- src/Web/Web.csproj
- src/PublicApi/PublicApi.csproj

**Tests**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Tasks

### 01-prerequisites: Verify toolchain and SDK readiness

Confirm the .NET 10 SDK is installed and available to the build, and update `global.json` to pin the SDK channel to `10.0.x` (from `8.0.x`). This is a prerequisite gate for every subsequent task — all project builds depend on the correct SDK being resolvable. No application code changes are required for this task.

**Done when**: `global.json` specifies a `10.0.x` SDK version and `dotnet --version`/`dotnet build` can resolve that SDK in this environment.

### 02-upgrade-all-projects: Upgrade TFM and package references across the solution

Upgrade the centrally managed `<TargetFramework>` in `Directory.Packages.props` from `net8.0` to `net10.0`, and bump the version properties (`AspNetVersion`, `EntityFramworkCoreVersion`, `SystemExtensionVersion`, `VSCodeGeneratorVersion`) and individual `PackageVersion` entries to their net10-compatible versions identified in assessment.md (e.g., Ardalis.* major bumps, FluentValidation 11→12, MediatR 12→14, Swashbuckle 6→10, System.IdentityModel.Tokens.Jwt 7→8, test packages NSubstitute/xunit.runner.visualstudio/MSTest/coverlet). This covers all 10 projects (6 source + 4 test) in one pass since they share the single centrally-managed TFM property — no per-project phasing.

Known risks from the assessment: Ardalis.Specification/Ardalis.Result/Ardalis.GuardClauses major version bumps may rename APIs used throughout `ApplicationCore`/`Infrastructure` specifications; FluentValidation 11→12 may change validator base classes; MediatR 12→14 registration API should be re-verified; Swashbuckle 6→10 Swagger configuration in `PublicApi` may need adjustment. Research starting points: search for `ISpecification`/`Specification<T>` usage, `AbstractValidator<T>` usage, `AddMediatR` call sites, and `SwaggerGen`/`[SwaggerOperation]` usage before editing.

**Done when**: All 10 projects target `net10.0`, `dotnet restore` succeeds with no version conflicts, and `dotnet build` on the solution succeeds with 0 errors and 0 warnings in the touched projects.

### 03-final-validation: Build and test the upgraded solution

Run a full solution build to confirm 0 errors/0 warnings, then run all four test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) and confirm all tests pass. Document any deferred recommendations (e.g., packages intentionally left unchanged, or follow-up items like MediatR licensing review) in progress notes.

**Done when**: `dotnet build` succeeds solution-wide with 0 errors/warnings, and `dotnet test` passes for all test projects (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests).

## Execution Record (post-upgrade)

### 01-prerequisites — ✅ Done
`global.json` SDK channel bumped `8.0.x` → `10.0.x`. .NET 10 SDK confirmed available in the environment.

### 02-upgrade-all-projects — ✅ Done
- Centrally managed `<TargetFramework>` bumped `net8.0` → `net10.0` in `Directory.Packages.props`; all 10 projects (6 source + 4 test) now target `net10.0`.
- Package versions bumped across the board (ASP.NET Core, EF Core, Ardalis.*, FluentValidation, MediatR 14.2.0, Swashbuckle.AspNetCore 10.3.0 / Microsoft.OpenApi 2.7.5, AutoMapper 16.2.0, System.IdentityModel.Tokens.Jwt 8.x, MSTest/coverlet/xunit test tooling).
- `BlazorInputFile` (NuGet `BlazorInputFile` 0.2.0) **removed entirely** — a years-abandoned, pre-.NET5 Blazor package whose IL layout fails .NET 10's stricter CLR type-loader checks (`ReflectionTypeLoadException`, "object field at offset 4 incorrectly aligned"). Confirmed dead code in this repo (only an uncalled `DataToBase64` helper, no `.razor` usage) — removed from both `.csproj` files, `Directory.Packages.props`, `_Imports.razor`, and the `<script>` tag in `Pages/Admin/Index.cshtml`; dead helper method removed from `BlazorShared/Models/CatalogItem.cs`.

### 03-final-validation — ✅ Done
Full clean rebuild (`dotnet build-server shutdown` + delete all `obj`/`bin` + `dotnet restore` + `dotnet build`) → **0 Warnings, 0 Errors**.
`dotnet test eShopOnWeb.sln --no-build` → **74/74 passed** (UnitTests 44, IntegrationTests 3, FunctionalTests 12, PublicApiIntegrationTests 15).

## Notable Breaking-Change Fixes (beyond mechanical package bumps)

1. **AutoMapper 16.2.0** — `AddAutoMapper(Assembly)` overload removed; changed to `AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)` in `PublicApi/Program.cs`.
2. **Microsoft.OpenApi v2 (via Swashbuckle.AspNetCore 10.3.0)** — `Microsoft.OpenApi.Models` namespace and `OpenApiReference`/plain `OpenApiSecurityRequirement` instance API removed. Rewrote the Swagger Bearer-auth security requirement in `PublicApi/Program.cs` to use `OpenApiSecuritySchemeReference` and the `Func<OpenApiDocument, OpenApiSecurityRequirement>` overload of `AddSecurityRequirement`. Added a null-check on `schema.Properties` in `PublicApi/CustomSchemaFilters.cs` (now `IOpenApiSchema.Properties`, nullable).
3. **.NET 10 SDK top-level-statement `Program` visibility** — empirically observed (not found documented) that an undecorated top-level-statements `Program` class now compiles `public`, causing `CS0433` ambiguity between `PublicApi.Program` and `Web.Program` in `WebApplicationFactory<Program>`-based tests. Fixed by appending `internal partial class Program { }` to `src/Web/Program.cs` to force it back to `internal` (mirroring `PublicApi/Program.cs`'s existing explicit `public partial class Program {}`).
4. **`PublicApiIntegrationTests` → `Web.csproj` ProjectReference removed** — `MinimalApi.Endpoint` 1.3.0's `AddEndpoints()` only has a parameterless overload that scans every assembly loaded in the AppDomain. The test project's `ProjectReference` to `Web.csproj` transitively pulled in `BlazorAdmin.csproj` (Blazor WebAssembly), loading WASM-targeted assemblies (e.g. `Microsoft.JSInterop.WebAssembly`) into the in-process test host, which failed CLR type-loading under .NET 10's stricter layout validation when reflectively scanned. The only consumer of the reference was `CatalogItemListPagedEndpoint.cs`'s use of `Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel` purely for JSON-shape deserialization in one test — swapped to the already-available `ListPagedCatalogItemResponse` (defined in `PublicApi`) and removed the `Web.csproj` reference entirely.
5. Minor Razor markup fixes: self-closed `<br/>` in `ShowRecoverCodes.cshtml`, self-closed two `<img>` tags in `_LoginPartial.cshtml` (harmless/correct but NOT the root cause of a large cascade of confusing Razor compiler errors seen mid-session — that cascade was actually caused by stale `dotnet build-server`/VBCSCompiler cached generator state; resolved via `dotnet build-server shutdown` + deleting `obj`/`bin`).
6. `[DataTestMethod]` → `[TestMethod]` in `CatalogItemListPagedEndpoint.cs` (MSTEST0044 obsolete-API warning fix; `[DataRow]` now works directly on `[TestMethod]`).

## Known Limitation Encountered This Session
The AppModUpgrade MCP orchestrator tools (`generate_dotnet_upgrade_assessment`, `start_task`/`complete_task`, etc.) returned "Orchestrator channel not available" errors throughout this session and could not be used for formal task tracking. Execution proceeded via direct file edits and `dotnet build`/`dotnet test` validation, with this plan.md/assessment.md/scenario-instructions.md maintained manually as the artifact record in their place.
