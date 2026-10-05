# Task 02-upgrade-solution: Upgrade all projects to net10.0 and update packages

> Note: MCP task orchestration tools (`start_task`) remain unavailable (orchestrator channel
> issue). Proceeding per plan.md with manually-tracked task.md/progress-details.md artifacts.

## Objective

Upgrade all 10 projects from net8.0 to net10.0 in a single atomic pass via the centrally
managed `Directory.Packages.props`, update all package versions per `assessment.md` section 3,
remove the dead `Microsoft.AspNetCore.Mvc` PackageVersion entry, update Dockerfiles and CI
`dotnet-version` pins, then build-and-fix in one bounded pass.

## Research (from assessment.md)

- Central properties: `TargetFramework`→net10.0, `AspNetVersion`→10.0.12,
  `SystemExtensionVersion`→10.0.12, `EntityFramworkCoreVersion`→10.0.12,
  `VSCodeGeneratorVersion`→10.0.2.
- Individual package bumps: Ardalis.GuardClauses 5.0.0, Ardalis.Specification(.EntityFrameworkCore)
  9.3.1, Ardalis.Result 10.1.0, FluentValidation 12.1.1, MediatR 14.2.0, Swashbuckle.AspNetCore(.SwaggerUI/.Annotations)
  10.2.3, Microsoft.NET.Test.Sdk 18.10.1, xunit 2.9.3, xunit.runner.visualstudio 4.0.0,
  xunit.runner.console 2.9.3, MSTest.TestAdapter/TestFramework 4.4.1, coverlet.collector 10.1.0,
  NSubstitute 6.2.0, System.IdentityModel.Tokens.Jwt 8.23.0, Azure.Identity 1.21.0,
  Azure.Extensions.AspNetCore.Configuration.Secrets 1.5.2, Microsoft.Web.LibraryManager.Build
  3.0.114, Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.23.0.
- Unchanged: Ardalis.ApiEndpoints 4.1.0, Ardalis.ListStartupServices 1.1.4, BlazorInputFile 0.2.0,
  Blazored.LocalStorage 4.5.0 (already latest), BuildBundlerMinifier 3.2.449,
  NSubstitute.Analyzers.CSharp 1.0.17, MinimalApi.Endpoint 1.3.0, System.Security.Claims 4.3.0.
- Remove: `Microsoft.AspNetCore.Mvc` 2.2.0 (unused, no matching `PackageReference` anywhere).
- Dockerfiles: `src/Web/Dockerfile`, `src/PublicApi/Dockerfile` pin `mcr.microsoft.com/dotnet/sdk:8.0`
  / `aspnet:8.0` → update to `10.0`.
- CI: `.github/workflows/dotnetcore.yml`, `.github/workflows/modernize.yml` pin
  `dotnet-version: 8.0.x` → `10.0.x`.
- Risks: MediatR license-line change (proceeding per Automatic default); BlazorInputFile staleness
  (replace only if build/runtime breaks surface); EF Core/ASP.NET Core 10 breaking changes to be
  resolved via build-fix loop.

**Done when**: `Directory.Packages.props` + `global.json` target net10.0; `dotnet build
eShopOnWeb.sln` succeeds with 0 errors/0 warnings; Dockerfiles/CI updated.
