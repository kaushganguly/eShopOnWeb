# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0

**Selected Strategy: All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all already on .NET 8.0 (modern, SDK-style), ≤3-tier dependency depth, only 1 incompatible package and no high-risk migrations (no OData, no Newtonsoft→STJ, no LINQ-to-SQL). Small scope with a mechanical TFM bump fits All-At-Once better than an incremental strategy.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects, all net8.0 SDK-style, ≤3-tier depth, no CI-green constraint — incremental strategy adds overhead without benefit |
| Unsupported Packages | Resolve Inline | Only 1 package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) has no compatible version — small enough to resolve within the same task |
| Unsupported API Handling | Fix Inline | Modern-to-modern upgrade (net8.0→net10.0); API issues are mostly behavioral changes (HttpContent, Uri) and a few binary-incompatible config binder calls — mechanical fixes, no complex/architectural replacements needed |

## Project List

**Class Libraries**
- src/ApplicationCore/ApplicationCore.csproj
- src/Infrastructure/Infrastructure.csproj
- src/BlazorShared/BlazorShared.csproj

**Web / API Applications**
- src/Web/Web.csproj (ASP.NET Core, Blazor Server host)
- src/PublicApi/PublicApi.csproj (ASP.NET Core Web API)
- src/BlazorAdmin/BlazorAdmin.csproj (Blazor WebAssembly)

**Test Projects**
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Verify the .NET 10 SDK is installed and available to the build toolchain, and update `global.json`'s SDK version constraint from the .NET 8 SDK band to a .NET 10 SDK band (rollForward policy preserved). This is a prerequisite gate for every subsequent task — if the SDK is missing, halt and report rather than proceeding with a build that cannot succeed.

**Done when**: `global.json` specifies a .NET 10 SDK version, and `dotnet --version`/SDK discovery confirms a matching SDK is installed and resolvable from the repo root.

### 02-upgrade-class-libraries: Upgrade ApplicationCore, Infrastructure, and BlazorShared to net10.0

Update `TargetFramework` to `net10.0` in ApplicationCore.csproj, Infrastructure.csproj, and BlazorShared.csproj, and bump their centrally-managed package versions in `Directory.Packages.props` (EF Core, Ardalis.Specification.EntityFrameworkCore, Identity EF Core packages, System.Text.Json, System.IdentityModel.Tokens.Jwt). Assessment flagged these three projects with low difficulty and few issues (ApplicationCore: 3 package issues + 2 API issues including a `System.Exception` serialization-constructor source-incompatibility; Infrastructure: 4 package issues, no API issues; BlazorShared: no issues at all). These are foundation libraries consumed by every other project in the solution, so getting them building cleanly first de-risks the rest of the upgrade even though there is no strict tiering under All-at-Once.

**Done when**: All three projects target `net10.0`, their package references resolve at the bumped versions with no restore errors, and each project builds with 0 errors and 0 warnings.

### 03-upgrade-web-and-api-projects: Upgrade Web, PublicApi, and BlazorAdmin to net10.0

Update `TargetFramework` to `net10.0` for Web.csproj, PublicApi.csproj, and BlazorAdmin.csproj, and bump their package references (ASP.NET Core Identity/JwtBearer/Components packages, EF Core SqlServer/Tools/InMemory/Diagnostics, Azure.Identity — which also carries a flagged security vulnerability at 1.10.4, upgrade to the assessment-suggested 1.21.0+ — AutoMapper.Extensions.Microsoft.DependencyInjection which is deprecated, and Microsoft.VisualStudio.Web.CodeGeneration.Design). These three are the highest-risk projects in the assessment: Web has 13 package issues and 14 API issues, PublicApi has 11 package issues and 5 API issues (including the one binary-incompatible, no-replacement package `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — resolve inline per the confirmed Unsupported Packages option, likely by removing/updating the reference since it is a build-time Docker tooling package rather than a runtime API surface), and BlazorAdmin has 7 package issues and 3 API issues. Most flagged API issues are behavioral changes in `System.Net.Http.HttpContent` and `System.Uri`, plus binary-incompatible `ConfigurationBinder.Get<T>`/`Configure<T>` overloads — fix these inline per the confirmed Unsupported API Handling option. Research starting points: review `Program.cs`/`Startup` configuration binding calls in Web and PublicApi, and check BlazorAdmin's WebAssembly authentication/DI wiring after the Components package bumps.

**Done when**: All three projects target `net10.0`, the flagged Azure.Identity vulnerability is resolved, the incompatible Docker tooling package reference is resolved, all API compatibility issues are fixed (not stubbed), and each project builds with 0 errors and 0 warnings.

### 04-upgrade-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests to net10.0

Update `TargetFramework` to `net10.0` for all four test projects and bump their package references (Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory, xunit/xunit.runner.console which are flagged deprecated, MSTest.TestAdapter/TestFramework which are flagged deprecated in PublicApiIntegrationTests, and System.IdentityModel.Tokens.Jwt). FunctionalTests carries the most API risk (29 API issues, largely the same `HttpContent`/`Uri` behavioral changes exercised through `WebApplicationFactory`-based HTTP calls) and 3 package issues; PublicApiIntegrationTests has 7 API issues and 3 package issues; IntegrationTests and UnitTests have package-only issues with no API issues. Since these projects reference ApplicationCore, Infrastructure, Web, and PublicApi, they must be rebuilt against the upgraded project outputs from tasks 02 and 03.

**Done when**: All four test projects target `net10.0`, package references resolve at bumped versions, each project builds with 0 errors and 0 warnings, and the full test suite passes (`dotnet test` across the solution).

### 05-final-validation: Full solution build and test validation

Run a full solution build (`dotnet build` on `eShopOnWeb.sln`) and the complete test suite (`dotnet test`) after all prior tasks complete, confirming there are 0 errors, 0 warnings, and all tests pass. Re-run the NuGet vulnerability check to confirm the Azure.Identity CVE is resolved and no new vulnerabilities were introduced by the version bumps. Document any deferred recommendations (e.g., further package updates beyond the minimum needed for net10.0 compatibility) as follow-up notes rather than blocking items.

**Done when**: `eShopOnWeb.sln` builds with 0 errors and 0 warnings, `dotnet test` reports all tests passing, and no known security vulnerabilities remain in the package set.
