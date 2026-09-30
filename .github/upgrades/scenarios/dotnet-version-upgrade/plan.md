# .NET Version Upgrade Plan

## Overview

**Target**: eShopOnWeb solution — retarget all 10 projects from .NET 8.0 to .NET 10.0
**Scope**: 10 projects (3 class libraries, 3 application hosts, 4 test projects), ~12k LOC, centrally managed packages via `Directory.Packages.props`

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all currently on .NET 8.0 (already modern), all SDK-style with centrally managed packages, dependency graph is shallow (ApplicationCore → Infrastructure/BlazorShared → Web/PublicApi/BlazorAdmin → test projects), and every project is rated 🟢 Low difficulty in the assessment. No .NET Framework projects, no side-by-side migration needed.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-At-Once | 10 projects, all net8.0 SDK-style, shallow dependency graph, uniformly Low difficulty. |

## Tasks

### 01-prerequisites: Verify toolchain and global.json compatibility

Verify the .NET 10 SDK is installed and available to the build (`global.json`, CI toolchain). Confirm the repository's `global.json` (if present) allows/targets a .NET 10 SDK version, and update it if it pins an older SDK. This is a fast, low-risk gate that must pass before any project file changes are made, since the whole solution will be retargeted in one pass.

**Done when**: `dotnet --list-sdks` shows a .NET 10 SDK available, and `global.json` (if present) does not block it.

---

### 02-shared-packages-and-libraries: Update Directory.Packages.props and upgrade class libraries

Update the shared `TargetFramework` in `Directory.Packages.props` and any project-level `<TargetFramework>` overrides from `net8.0` to `net10.0` across all 10 projects. Update centrally managed NuGet package versions to .NET-10-compatible releases, including ASP.NET Core shared framework references, EF Core (`Microsoft.EntityFrameworkCore.*`), ASP.NET Core Identity, Ardalis libraries (`Ardalis.Specification`, `Ardalis.GuardClauses`, `Ardalis.ApiEndpoints`, etc.), xUnit (`xunit`, `xunit.runner.visualstudio`), and Azure SDK packages (`Azure.Identity` — flagged in the assessment as containing a security vulnerability at 1.10.4, must move to a patched 1.2x+ release). Also replace/remove the flagged-incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference in `PublicApi.csproj` if it blocks the build on .NET 10.

Then retarget the class library projects — ApplicationCore, BlazorShared, and Infrastructure (deprecated packages flagged) — to `net10.0` and fix any resulting compile errors, including source-incompatibility around custom `Exception` types using the legacy `SerializationInfo`/`StreamingContext` constructor pattern.

**Done when**: ApplicationCore, BlazorShared, and Infrastructure target `net10.0`, `Directory.Packages.props` package versions are updated, and these three projects build with 0 errors.

---

### 03-web-and-api-hosts: Upgrade Web, PublicApi, and BlazorAdmin applications

Retarget the three application projects to `net10.0`: Web (ASP.NET Core MVC/Razor host), PublicApi (ASP.NET Core Web API host, includes an incompatible NuGet package), and BlazorAdmin (Blazor WebAssembly admin app). Fix breaking API changes flagged by the assessment across these hosts, in particular: `HttpContent`/`HttpClient` behavioral changes (the most frequent issue across the solution), `System.Uri` behavioral changes (including the `Uri(string)` and `Uri(string, UriKind)` constructors), `ConfigurationBinder.Get<T>()` / `Configure<T>()` binary-incompatible overload changes, `ConsoleLoggerExtensions.AddConsole` behavioral change, and `ExceptionHandlerExtensions.UseExceptionHandler(string)` behavioral change. Review `Program.cs` configuration binding, HttpClient usage, and exception-handling middleware registration in each host after the retarget.

**Done when**: Web, PublicApi, and BlazorAdmin target `net10.0`, and all three build with 0 errors.

---

### 04-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests

Retarget the four test projects to `net10.0` and update xUnit and test-infrastructure package versions to .NET-10-compatible releases. FunctionalTests carries the heaviest API-issue load in the solution — largely stemming from the same `HttpContent`/`Uri` behavioral-change APIs used by `WebApplicationFactory`-based in-memory test clients, so expect adjustments to HTTP response assertions and URI construction in test helpers. PublicApiIntegrationTests also has flagged API issues. IntegrationTests and UnitTests have package-only issues (deprecated package references). Update test project references to the retargeted class libraries and application hosts from tasks 02/03 as needed.

**Done when**: All four test projects target `net10.0`, reference updated package versions, and build with 0 errors.

---

### 05-final-validation: Full solution build and test suite validation

Restore the full solution, build `eShopOnWeb.sln` end-to-end with 0 errors and 0 warnings, then run the complete automated test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) and confirm all tests pass. Document any deferred recommendations as notes rather than new tasks.

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors/0 warnings, and `dotnet test` across all four test projects passes with 0 failures.
