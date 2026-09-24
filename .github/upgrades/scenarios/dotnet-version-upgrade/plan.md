# Upgrade Plan: eShopOnWeb — .NET 8.0 → .NET 10.0

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Upgrade Strategy | All-at-Once | 10 projects, all already on net8.0 (modern .NET), assessment shows all projects at 🟢 Low difficulty, ≤3-tier dependency depth, no CI-green constraint stated — small scope, mechanical TFM bump. |

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on .NET 8.0, clear dependency structure (ApplicationCore/BlazorShared/Infrastructure as libraries; Web/PublicApi/BlazorAdmin as apps; four test projects), no .NET Framework projects, no incompatible packages blocking upgrade.

## Project List

**Libraries**
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorShared/BlazorShared.csproj
- src/Infrastructure/Infrastructure.csproj

**Applications**
- src/Web/Web.csproj
- src/PublicApi/PublicApi.csproj
- src/BlazorAdmin/BlazorAdmin.csproj

**Test Projects**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Tasks

### 01-prerequisites: Verify SDK toolchain and update global.json

Verify the .NET 10 SDK is installed/available in the build environment, and update `global.json` at the repo root to reference a .NET 10 SDK version (matching the installed SDK, using appropriate `rollForward` policy). Also review `Directory.Packages.props` and `Directory.Build.props` for any centrally managed `TargetFramework`/`LangVersion` settings that need to move from `net8.0` to `net10.0`.

The assessment shows all 10 projects currently target `net8.0` and are SDK-style with centrally managed package versions (CPM via `Directory.Packages.props`). This task establishes the toolchain foundation the rest of the upgrade depends on — no code changes here.

**Done when**: `global.json` specifies a .NET 10 SDK version, `dotnet --version`/`dotnet --list-sdks` confirms a matching SDK is available, and `dotnet restore` on the solution succeeds against the updated global.json.

### 02-core-libraries: Upgrade ApplicationCore, BlazorShared, and Infrastructure to net10.0

Update the `TargetFramework` for `src/ApplicationCore/ApplicationCore.csproj`, `src/BlazorShared/BlazorShared.csproj`, and `src/Infrastructure/Infrastructure.csproj` from `net8.0` to `net10.0`, and bump their centrally managed NuGet package versions (Entity Framework Core, Microsoft.Extensions.* packages, Ardalis.Specification, Autofac, etc.) in `Directory.Packages.props` to .NET 10-compatible versions.

Assessment signals: ApplicationCore has 3 package issues and 2 API issues (🟢 Low difficulty, ~2+ LOC impact) including a source-incompatible API and a NuGet package whose functionality is now included with the framework reference. Infrastructure has 4 package issues including deprecated packages (likely older EF Core / Identity related packages) but no API issues reported. BlazorShared has 0 issues — only needs the TFM bump. These are the foundational libraries consumed by the Web, PublicApi, and BlazorAdmin apps, so they must be upgraded first within this atomic pass so downstream projects can restore against them.

Research starting points: check `assessment/projects/ApplicationCore.md`, `assessment/projects/Infrastructure.md`, and `assessment/projects/BlazorShared.md` for the specific package/API issue details; check EF Core DbContext and migration code in Infrastructure for provider package version alignment; review `M:System.Exception.#ctor(SerializationInfo,StreamingContext)` (source-incompatible, appears in the aggregate API issues list) which commonly affects custom exception types implementing the serialization constructor.

**Done when**: All three projects target `net10.0`, restore successfully, and build without errors using package versions compatible with .NET 10.

### 03-web-apps: Upgrade Web, PublicApi, and BlazorAdmin to net10.0

Update the `TargetFramework` for `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj`, and `src/BlazorAdmin/BlazorAdmin.csproj` from `net8.0` to `net10.0`, bump their NuGet package versions (ASP.NET Core-related packages, Swashbuckle/OpenAPI, Blazor packages, Autofac.Extensions.DependencyInjection, etc.), and fix breaking/behavioral API changes surfaced by the assessment.

Assessment signals: Web has the highest impact — 13 package issues and 15 API issues (~15+ LOC impact), including binary-incompatible, source-incompatible, and behavioral-change APIs, plus a package with a **security vulnerability** and deprecated packages. PublicApi has 11 package issues and 5 API issues, including a binary-incompatible NuGet package and a package that is fully incompatible with net10.0. BlazorAdmin has 7 package issues and 3 API issues (behavioral change). Known frequent API issues across the solution include `System.Net.Http.HttpContent` (behavioral change, 32 occurrences — likely `HttpClient`/`HttpContent` header or content-length handling changes), `System.Uri` behavioral changes (11 occurrences), `ConfigurationBinder.Get<T>`/`Configure<T>` binary-incompatible changes (4 each — check strongly-typed options binding calls), `ConsoleLoggerExtensions.AddConsole` behavioral change, and `ExceptionHandlerExtensions.UseExceptionHandler(string)` behavioral change (check `Program.cs`/`Startup.cs` exception handling middleware registration in Web and PublicApi).

Research starting points: open `assessment/projects/Web.md`, `assessment/projects/PublicApi.md`, and `assessment/projects/BlazorAdmin.md` for the full per-project issue lists and exact API references; open `assessment/nuget/aggregate-packages.md` for the specific incompatible/vulnerable/deprecated package names and recommended versions; open `assessment/api-issues/most-frequent-api-issues.md` for the complete API issue list with file/line references (or query via `query_dotnet_assessment`); check `Program.cs` and `Startup.cs` in Web and PublicApi for middleware pipeline, configuration binding, and logging setup since these are the hot spots implied by the API issue list.

**Done when**: All three projects target `net10.0`, restore and build successfully, the flagged security-vulnerable and deprecated packages are replaced with secure, non-deprecated .NET 10-compatible versions, and all binary/source-incompatible API usages are fixed (behavioral-change APIs reviewed and adjusted where they affect correctness).

### 04-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests to net10.0

Update the `TargetFramework` for `tests/UnitTests/UnitTests.csproj`, `tests/IntegrationTests/IntegrationTests.csproj`, `tests/FunctionalTests/FunctionalTests.csproj`, and `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` from `net8.0` to `net10.0`, and bump their test framework and related NuGet package versions (xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory, coverlet, etc.) to .NET 10-compatible versions.

Assessment signals: FunctionalTests has the largest test-side impact — 3 package issues, **29 API issues** (~29+ LOC impact), and a deprecated package; this is the highest-risk test project and likely uses `WebApplicationFactory`/`HttpClient` test helpers that intersect with the `HttpContent`/`Uri` behavioral-change APIs called out at the solution level. PublicApiIntegrationTests has 3 package issues and 7 API issues plus a behavioral change flag. IntegrationTests has 2 package issues and a deprecated package but 0 API issues. UnitTests has 2 package issues (one deprecated) and 0 API issues — lowest risk. Per the "test work belongs to the task that caused it" rule, only test-project-specific work (test project TFM/package bumps, and fixes to test code itself) lives here; production-code fixes needed to make tests pass belong to tasks 02/03.

Research starting points: open `assessment/projects/FunctionalTests.md` and `assessment/projects/PublicApiIntegrationTests.md` for the detailed API issue lists (likely `HttpClient`/`HttpContent`/`Uri` usage in test helpers and assertions); confirm the deprecated packages flagged in IntegrationTests and UnitTests via `assessment/nuget/aggregate-packages.md` and replace with current equivalents; verify `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio` versions are compatible with net10.0.

**Done when**: All four test projects target `net10.0`, restore and build successfully with updated package versions, and the full test suite (`dotnet test` across all four projects) passes.

### 05-final-validation: Full solution build and test validation

Run a full solution build (`dotnet build` on `eShopOnWeb.sln`) and full test run (`dotnet test`) across all 10 projects after the atomic upgrade is complete. Confirm there are 0 build errors and 0 build warnings introduced by the upgrade (fix any warnings in touched projects). Confirm no remaining security-vulnerable package versions are referenced anywhere in the solution (re-check `Directory.Packages.props` against the assessment's vulnerability findings). Document any deferred recommendations (e.g., optional modernization opportunities not required for the upgrade itself) in `progress-details.md`.

**Done when**: `dotnet build` succeeds with 0 errors and 0 warnings for all 10 projects targeting `net10.0`, `dotnet test` passes for all four test projects, and no known-vulnerable package versions remain in `Directory.Packages.props`.
